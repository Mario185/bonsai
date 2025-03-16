using System;
using System.Drawing;
using System.Text.Json.Serialization;
using bonsai.Apps;
using bonsai.Theme;
using bonsai.Utilities;
using clui.Controls;
using consoleTools;

namespace bonsai.Data
{
  internal class DatabaseEntry : IListItem, ISearchableItem
  {
    private string? _printableText;

    private SearchMatch[]? _searchMatches = [];
    private int _score;

    public DatabaseEntry(string path, bool isDirectory)
    {
      Path = path;
      IsDirectory = isDirectory;
    }

    public int Score
    {
      get => _score;
      set
      {
        if (_score != value)
          _printableText = null;
        _score = value;
      }
    }

    public string Path { get; }
    public bool IsDirectory { get; }
    public DateTime LastUsed { get; set; }

    public void Write (ConsoleWriter writer, int maxLength, bool isFocusedItem)
    {
      if (BonsaiContext.Current?.IsFilteringActive == true)
      {
        var totalLength = GetPrintableText().Length;
        var searchableTextLength = SearchableText.Length;
        var indexOfSearchableText = totalLength - searchableTextLength;
        for (int i = 0; i < totalLength; i++)
        {
          if (i >= indexOfSearchableText && IsIndexInMatch (i - indexOfSearchableText))
            writer.Style.ForegroundColor (Color.LightGreen).Underline();
          else
          {
            if (isFocusedItem)
              writer.Style.ForegroundColor (ThemeManger.Instance.SelectionForegroundColor);
            else
              writer.Style.ResetForegroundColor();
            writer.Style.ResetUnderscore();
          }

          writer.Write (GetPrintableText()[i]);
        }
      }
      else
      {
        writer.WriteTruncated (GetPrintableText(), 0, maxLength);
      }
    }

    private bool IsIndexInMatch(int index)
    {
      if (_searchMatches == null || _searchMatches.Length == 0)
        return false;

      for (int i = 0; i < _searchMatches.Length; i++)
      {
        var match = _searchMatches[i];
        if (match.MatchStartAt <= index && index < match.MatchEndAt)
          return true;
      }

      return false;
    }

    private string GetPrintableText()
    {
      if (_printableText == null)
        _printableText = $"{LastUsed:yyyy-MM-dd HH:mm} {Score,6} {Path}";
      return _printableText;
    }

    [JsonIgnore]
    public string SearchableText => Path;

    public void SetSearchMatches(SearchMatch[] matches)
    {
      _searchMatches = matches;
    }
  }
}