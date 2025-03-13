using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using bonsai.JsonConverter;
using bonsai.Utilities;
using consoleTools;

namespace bonsai.Theme
{
  // ReSharper disable once ClassCannotBeInstantiated
  public sealed class ThemeManger
  {
    private const string c_manifestResourceNamePrefix = "bonsai.Resources.themes.";

    [JsonConstructor]
    private ThemeManger()
    {
    }

    public static ThemeManger Instance { get; private set; } = null!;

    public IconMapping FolderIcons { get; set; } = null!;
    public FileIconMapping FileIcons { get; set; } = null!;
    public ColorMapping FolderColors { get; set; } = null!;
    public FileColorMapping FileColors { get; set; } = null!;

    public Color? BackgroundColor { get; set; }
    public Color? SelectionBackgroundColor { get; set; }
    public Color? SelectionForegroundColor { get; set; }
    public Color? BorderColor { get; set; }
    public Color? SearchLabelTextColor { get; set; }
    public Color? SearchLabelBackgroundColor { get; set; }
    public Color? TopBarTextColor { get; set; }
    public Color? TopBarBackgroundColor { get; set; }
    public Color? OptionEnabledColor { get; set; }
    public Color? OptionDisabledColor { get; set; }

    public string? LoadingSpinnerChars { get; set; }

    public Color? GetFileColor(string fileName)
    {
      if (!FileColors.Named.TryGetValue(fileName, out Color? color))
      {
        if (!FileColors.Extensions.TryGetValue(Path.GetExtension(fileName), out color))
          return FileColors.DefaultColor;
      }

      return color;
    }

    public string GetFileIcon(string fileName)
    {
      if (!FileIcons.Named.TryGetValue(fileName, out string? icon))
      {
        if (!FileIcons.Extensions.TryGetValue(Path.GetExtension(fileName), out icon))
          icon = FileIcons.DefaultIcon;
      }

      return icon ?? string.Empty;
    }

    public Color? GetFolderColor(string folderName)
    {
      if (FolderColors.Named.TryGetValue(folderName, out Color? color))
        return color;

      return FolderColors.DefaultColor;
    }

    public string GetFolderIcon(string folderName)
    {
      if (!FolderIcons.Named.TryGetValue(folderName, out string? icon))
        icon = FolderIcons.DefaultIcon;

      return icon ?? string.Empty;
    }

    public static void LoadTheme(string theme)
    {
      AbsolutePath? themeFilePath = KnownPaths.ThemesFolder / theme;
      if (!File.Exists(themeFilePath))
      {
        if (!DoesResourceContainsTheme(theme))
          throw new FileNotFoundException($"Theme file '{themeFilePath}' does not exist.");

        themeFilePath = null;
      }

      JsonSerializerOptions options = GetJsonSerializerOptions();
      using (Stream stream = themeFilePath == null ? GetThemeFromResources(theme) : File.OpenRead(themeFilePath))
      {
        Instance = JsonSerializer.Deserialize<ThemeManger>(stream, options)!;
      }
    }

    public static void WriteResourceThemesToConfigFolder(bool force)
    {
      AbsolutePath path = KnownPaths.ThemesFolder;
      path.EnsureDirectoryExists();

      IEnumerable<string> resourceThemeNames = typeof(ThemeManger).Assembly.GetManifestResourceNames().Where(n => n.StartsWith(c_manifestResourceNamePrefix))
        .Select(n => n.Substring(c_manifestResourceNamePrefix.Length));

      foreach (string themeName in resourceThemeNames)
      {
        var targetFilePath = path / themeName;
        
        if (File.Exists (targetFilePath) && !force)
          continue;

        using (Stream defaultThemeStream = GetThemeFromResources(themeName))
        using (FileStream fileStream = new(targetFilePath, FileMode.Create))
          defaultThemeStream.CopyTo(fileStream);
      }

      if (force)
      {
        using (var writer = new ConsoleWriter())
        {
          writer.Style.ForegroundColor(Color.ForestGreen)
            .Writer.Write($"Themes saved to '{path}'\n")
            .Style.ResetStyles();
        }
      }
    }

    private static bool DoesResourceContainsTheme(string theme)
    {
      return typeof(ThemeManger).Assembly.GetManifestResourceInfo(c_manifestResourceNamePrefix + theme.ToLower(CultureInfo.InvariantCulture)) != null;
    }

    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
      JsonSerializerOptions options = new()
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new ColorJsonConverter() }
      };
      return options;
    }

    private static Stream GetThemeFromResources(string theme)
    {
      return typeof(ThemeManger).Assembly.GetManifestResourceStream(c_manifestResourceNamePrefix + theme.ToLower(CultureInfo.InvariantCulture))!;
    }
  }
}