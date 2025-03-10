using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using bonsai.CommandHandling;
using bonsai.JsonConverter;
using bonsai.Utilities;

namespace bonsai
{
  internal sealed class Settings
  {
    private readonly Lazy<Dictionary<KeyBindingContext, Dictionary<ConsoleModifiers, (Dictionary<char, ActionType> ByChar, Dictionary<ConsoleKey, ActionType> ByKey)>>>
      _keyBindings;

    public Settings()
    {
      _keyBindings = new Lazy<Dictionary<KeyBindingContext, Dictionary<ConsoleModifiers, (Dictionary<char, ActionType> ByChar, Dictionary<ConsoleKey, ActionType> ByKey)>>>(() =>
      {
        var result = new Dictionary<KeyBindingContext, Dictionary<ConsoleModifiers, (Dictionary<char, ActionType> ByChar, Dictionary<ConsoleKey, ActionType> ByKey)>>();

        foreach (var context in Enum.GetValues<KeyBindingContext>())
        {
          if (KeyBindings.TryGetValue(context, out var bindings))
            result[context] = BuildKeyBindingDictionary(bindings);
        }

        return result;
      });
    }

    public static Settings Instance { get; private set; } = null!;

    public bool ShowParentDirectoryInList { get; set; }

    public string Theme { get; set; } = "default.json";

    [JsonInclude]
    public IReadOnlyDictionary<KeyBindingContext, IReadOnlyList<KeyBinding>> KeyBindings { get; private set; } = new Dictionary<KeyBindingContext, IReadOnlyList<KeyBinding>>();

    public int MaxTotalScoreInDatabase { get; set; } = 2000;

    public int MaxIndividualScore { get; set; } = 100;

    public int MaxEntryAgeInDays { get; set; } = 300;

    [JsonInclude]
    public IReadOnlyList<FileCommand> FileCommands { get; private set; } = new List<FileCommand>();

    [JsonInclude]
    public IReadOnlyList<DirectoryCommand> DirectoryCommands { get; private set; } = new List<DirectoryCommand>();

    public ActionType GetInputActionType(ConsoleKeyInfo keyInfo, KeyBindingContext keyBindingContext)
    {
      Dictionary<ConsoleModifiers, (Dictionary<char, ActionType> ByChar, Dictionary<ConsoleKey, ActionType> ByKey)> keyBindingsByContext = _keyBindings.Value[keyBindingContext];
      ActionType? result = GetInputActionInternal(keyInfo, keyBindingsByContext);
      if (result.HasValue)
        return result.Value;

      Dictionary<ConsoleModifiers, (Dictionary<char, ActionType> ByChar, Dictionary<ConsoleKey, ActionType> ByKey)>
        commonKeyBindings = _keyBindings.Value[KeyBindingContext.Common];
      result = GetInputActionInternal(keyInfo, commonKeyBindings);
      return result ?? ActionType.None;
    }

    public static void LoadSettings()
    {
      JsonSerializerOptions options = GetJsonSerializerOptions();

      AbsolutePath settingsPath = GetSettingsFilePath();
      if (!File.Exists(settingsPath))
        WriteDefaultSettingsToFile();

      using (FileStream stream = File.OpenRead(settingsPath))
      {
        Instance = JsonSerializer.Deserialize<Settings>(stream, options)!;
        if (string.IsNullOrWhiteSpace(Instance.Theme))
          Instance.Theme = "default.json";
      }
    }

    public static string Serialize()
    {
      JsonSerializerOptions options = GetJsonSerializerOptions();

      return JsonSerializer.Serialize(Instance, options);
    }

    public static void WriteDefaultSettingsToFile()
    {
      using (Stream defaultSettingsStream = GetDefaultSettings())
      using (FileStream fileStream = new(GetSettingsFilePath(), FileMode.Create))
        defaultSettingsStream.CopyTo(fileStream);
    }

    private Dictionary<ConsoleModifiers, (Dictionary<char, ActionType> ByChar, Dictionary<ConsoleKey, ActionType> ByKey)> BuildKeyBindingDictionary(
      IReadOnlyList<KeyBinding> rawBindings)
    {
      Dictionary<ConsoleModifiers, (Dictionary<char, ActionType> ByChar, Dictionary<ConsoleKey, ActionType> ByKey)> keyBindingDictionary = new();
      List<IGrouping<ConsoleModifiers, KeyBinding>> groupedByModifier = rawBindings.GroupBy(k => k.Modifier).ToList();

      foreach (IGrouping<ConsoleModifiers, KeyBinding> group in groupedByModifier)
      {
        (Dictionary<char, ActionType> ByChar, Dictionary<ConsoleKey, ActionType> ByKey) value = new();
        value.ByChar = new Dictionary<char, ActionType>();
        value.ByKey = new Dictionary<ConsoleKey, ActionType>();
        keyBindingDictionary.Add(group.Key, value);

        foreach (KeyBinding entry in group)
        {
          if (entry.Key.HasValue)
            value.ByKey.Add(entry.Key.Value, entry.Action);

          if (entry.KeyChar.HasValue)
            value.ByChar.Add(entry.KeyChar.Value, entry.Action);
        }
      }

      return keyBindingDictionary;
    }

    private static Stream GetDefaultSettings()
    {
      return typeof(Settings).Assembly.GetManifestResourceStream("bonsai.Resources.default_settings.json")!;
    }

    private ActionType? GetInputActionInternal(ConsoleKeyInfo keyInfo,
      Dictionary<ConsoleModifiers, (Dictionary<char, ActionType> ByChar, Dictionary<ConsoleKey, ActionType> ByKey)> keyBindings)
    {
      if (!keyBindings.TryGetValue(keyInfo.Modifiers, out (Dictionary<char, ActionType> ByChar, Dictionary<ConsoleKey, ActionType> ByKey) modifierByContext))
        return null;

      if (modifierByContext.ByChar.TryGetValue(keyInfo.KeyChar, out ActionType byChar))
        return byChar;

      if (modifierByContext.ByKey.TryGetValue(keyInfo.Key, out ActionType byKey))
        return byKey;

      return null;
    }

    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
      JsonSerializerOptions options = new()
      {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters =
        {
          new ColorJsonConverter(), new EnumJsonConverter<ActionType>(ActionType.None), new EnumJsonConverter<ConsoleModifiers>(ConsoleModifiers.None),
          new EnumJsonConverter<ConsoleKey>(ConsoleKey.None)
        }
      };
      return options;
    }

    private static AbsolutePath GetSettingsFilePath()
    {
      AbsolutePath path = KnownPaths.BonsaiConfigFolder;
      path.EnsureDirectoryExists();
      AbsolutePath settingsFilePath = path / "settings.json";
      return settingsFilePath;
    }
  }

  public class KeyBinding
  {
    public ConsoleModifiers Modifier { get; set; }
    public char? KeyChar { get; set; }
    public ConsoleKey? Key { get; set; }
    public ActionType Action { get; set; }
  }

  public enum KeyBindingContext
  {
    Common,
    ExplorerApp,
    NavigationApp,
    EditDatabaseApp,
  }

  public enum ActionType
  {
    None,
    Exit,
    OpenDirectory,
    OpenParentDirectory,
    ConfirmSelection,
    ListSelectPreviousItem,
    ListSelectNextItem,
    ListSelectOnePageUp,
    ListSelectOnePageDown,
    ListSelectFirstItem,
    ListSelectLastItem,
    ToggleShowDetailsPanel,
    ToggleIncludeSubDirectories,
    ToggleRegexSearch,
    IncrementScore,
    DecrementScore,
    SaveDatabaseChanges,
    DeleteDatabaseEntry
  }
}