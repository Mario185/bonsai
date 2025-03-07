using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using bonsai.Explorer;
using bonsai.Utilities;

namespace bonsai.Navigation
{
  public class NavigationDatabase
  {
    private const int c_maxAge = 336; // 14 days in hours

    private static readonly Lazy<NavigationDatabase> s_instance = new(Load);

    public static NavigationDatabase Instance => s_instance.Value;

    [JsonInclude]
    public List<NavigationEntry> Entries { get; private set; } = new();

    [JsonInclude]
    public int TopScore { get; private set; }

    public void AddOrUpdate(string fullPath, bool isDirectory, bool autoSave = true)
    {
      string trimmed = fullPath.Trim(Path.DirectorySeparatorChar);
      NavigationEntry? entry = Entries.FirstOrDefault(e => string.Equals(e.Path, trimmed, StringComparison.OrdinalIgnoreCase));
      if (entry == null)
      {
        entry = new NavigationEntry(trimmed, isDirectory);
        Entries.Add(entry);
      }

      UpdateEntry(entry, autoSave);
    }

    public (double recency, double scoreMultiplier, int score) CalculateScore(int value, DateTime lastUsed, DateTime queryTime)
    {
      // we cast TotalHours to int so we get hourly steps
      double recency = Math.Max(0, c_maxAge - (int)(queryTime - lastUsed).TotalHours);

      double scoreMultiplier = Math.Exp(0.018 * recency);
      return (recency, scoreMultiplier, (int)(value * scoreMultiplier));
    }

    public void CleanUpDatabase()
    {
      double ninetyPercentOfMax = Settings.Instance.MaxIndividualScore * .9;
      double factor = ninetyPercentOfMax / TopScore;

      bool needsScoreRecalculation = TopScore > Settings.Instance.MaxIndividualScore;

      DateTime cutOfDate = DateTime.UtcNow.AddDays(-Settings.Instance.MaxEntryAgeInDays);

      bool hasChanges = false;
      foreach (NavigationEntry entry in Entries.ToArray())
      {
        if (entry.LastUsed < cutOfDate || !FileOrDirectoryExist(entry))
        {
          Entries.Remove(entry);
          hasChanges = true;
        }
        else if (needsScoreRecalculation)
        {
          double result = entry.Score * factor;
          if (result < 1)
          {
            Entries.Remove(entry);
            continue;
          }

          entry.Score = (int)result;
          hasChanges = true;
        }
      }

      if (hasChanges)
      {
        TopScore = Entries.Count == 0 ? 0 : Entries.Max(e => e.Score);
        Save();
      }
    }

    public void Save()
    {
      JsonSerializerOptions options = GetJsonSerializerOptions();
      File.WriteAllText(GetDatabaseFilePath(), JsonSerializer.Serialize(Instance, options));
    }

    public void UpdateEntry(NavigationEntry entry, bool autoSave = true)
    {
      entry.LastUsed = DateTime.UtcNow;
      entry.Score++;

      if (entry.Score > TopScore)
        TopScore = entry.Score;

      if (autoSave)
        Save();
    }

    internal FileSystemItem[] Search(string args)
    {
      string[] parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      DateTime queryTime = DateTime.UtcNow;

      List<(FileSystemItem fi, NavigationEntry entry)> foundEntries = new();
      foreach (NavigationEntry entry in Entries)
      {
        string displayName = entry.Path;
        int lastIndex = 0;
        int lastPartStartsAt = displayName.LastIndexOf(Path.DirectorySeparatorChar);
        bool isMatch = false;
        SearchMatch[] matches = new SearchMatch[parts.Length];
        for (int index = 0; index < parts.Length; index++)
        {
          string part = parts[index];
          int currentIndex = displayName.IndexOf(part, lastIndex, StringComparison.OrdinalIgnoreCase);

          if (currentIndex < lastIndex || currentIndex < 0)
          {
            isMatch = false;
            break;
          }

          matches[index] = new SearchMatch(currentIndex, part.Length);
          lastIndex = currentIndex + 1;

          //we are now at the last part of the search term
          if (index == parts.Length - 1)
          {
            // last part of search term has to be in the last part of the entry
            if (currentIndex < lastPartStartsAt)
            {
              isMatch = false;
              break;
            }
          }

          isMatch = true;
        }

        if (!isMatch)
          continue;

        FileSystemItem fileSystemItem;
        if (entry.IsDirectory)
          fileSystemItem = new DirectoryItem(new DirectoryInfo(entry.Path), 0, () => true);
        else
          fileSystemItem = new FileItem(new FileInfo(entry.Path), 0, () => true);
        fileSystemItem.SetSearchMatches(matches);
        foundEntries.Add((fileSystemItem, entry));
      }

      // still needs improvement
      return foundEntries.OrderByDescending(i => CalculateScore(i.entry.Score, i.entry.LastUsed, queryTime)).ThenByDescending(i => i.entry.LastUsed).Select(i => i.fi).ToArray();
    }

    private bool FileOrDirectoryExist(NavigationEntry entry)
    {
      if (entry.IsDirectory)
        return Directory.Exists(entry.Path);

      return File.Exists(entry.Path);
    }

    private static AbsolutePath GetDatabaseFilePath()
    {
      AbsolutePath path = KnownPaths.BonsaiConfigFolder;
      path.EnsureDirectoryExists();
      AbsolutePath databaseFilePath = path / "db.json";
      return databaseFilePath;
    }

    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
      JsonSerializerOptions options = new()
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
      };

      return options;
    }

    private static NavigationDatabase Load()
    {
      JsonSerializerOptions options = GetJsonSerializerOptions();

      AbsolutePath databaseFilePath = GetDatabaseFilePath();
      if (!File.Exists(databaseFilePath))
        return new NavigationDatabase();

      using (FileStream stream = File.OpenRead(databaseFilePath))
        return JsonSerializer.Deserialize<NavigationDatabase>(stream, options)!;
    }
  }
}