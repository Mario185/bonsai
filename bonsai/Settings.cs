using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using bonsai.CommandHandling;
using bonsai.JsonSerialization;
using bonsai.Utilities;
using ConsoleModiferDictionary =
  System.Collections.Generic.Dictionary<System.ConsoleModifiers, (System.Collections.Generic.Dictionary<char, bonsai.ActionType> ByChar,
    System.Collections.Generic.Dictionary<System.ConsoleKey, bonsai.ActionType> ByKey)>;
using KeyBindingContextDictionary =
  System.Collections.Generic.Dictionary<bonsai.KeyBindingContext, System.Collections.Generic.Dictionary<System.ConsoleModifiers, (
    System.Collections.Generic.Dictionary<char, bonsai.ActionType> ByChar, System.Collections.Generic.Dictionary<System.ConsoleKey, bonsai.ActionType> ByKey)>>;

namespace bonsai
{
  internal sealed class Settings
  {
    public static Settings Instance { get; private set; } = null!;

    public static void LoadSettings()
    {
      AbsolutePath settingsPath = GetSettingsFilePath();
      if (!File.Exists(settingsPath))
      {
        WriteDefaultSettingsToFile();
      }

      Settings loadedSettings;
      using (FileStream stream = File.OpenRead(settingsPath))
      {
        loadedSettings = JsonSerializer.Deserialize<Settings>(stream, SettingsGenerationContext.Default.Settings)!;
        if (string.IsNullOrWhiteSpace(loadedSettings.Theme))
        {
          loadedSettings.Theme = "default.json";
        }
      }

      EnsureMissingDefaultSettings(loadedSettings);

      Instance = loadedSettings;
    }

    public static void WriteDefaultSettingsToFile()
    {
      using (Stream defaultSettingsStream = GetDefaultSettings())
      using (FileStream fileStream = new(GetSettingsFilePath(), FileMode.Create))
      {
        defaultSettingsStream.CopyTo(fileStream);
      }
    }

    private static ConsoleModiferDictionary BuildKeyBindingDictionary(IReadOnlyList<KeyBinding> rawBindings)
    {
      ConsoleModiferDictionary keyBindingDictionary = new();
      List<IGrouping<ConsoleModifiers, KeyBinding>> groupedByModifier = rawBindings.GroupBy(k => k.Modifier).ToList();

      foreach (IGrouping<ConsoleModifiers, KeyBinding> group in groupedByModifier)
      {
        (Dictionary<char, ActionType> ByChar, Dictionary<ConsoleKey, ActionType> ByKey) value = new()
        {
          ByChar = new Dictionary<char, ActionType>(),
          ByKey = new Dictionary<ConsoleKey, ActionType>()
        };
        keyBindingDictionary.Add(group.Key, value);

        foreach (KeyBinding entry in group)
        {
          if (entry.Key.HasValue)
          {
            value.ByKey.Add(entry.Key.Value, entry.Action);
          }

          if (entry.KeyChar.HasValue)
          {
            value.ByChar.Add(entry.KeyChar.Value, entry.Action);
          }
        }
      }

      return keyBindingDictionary;
    }

    private static void EnsureMissingDefaultSettings(Settings loadedSettings)
    {
      bool settingsModified = false;
      Settings defaultSettings;

      using (Stream defaultSettingsStream = GetDefaultSettings())
      {
        defaultSettings = JsonSerializer.Deserialize<Settings>(defaultSettingsStream, SettingsGenerationContext.Default.Settings)!;
      }

      foreach (KeyValuePair<KeyBindingContext, List<KeyBinding>> kvp in defaultSettings.KeyBindings)
      {
        List<KeyBinding> loadedKeyBindings = GetOrCreateKeyBindingContextEntry(loadedSettings, kvp.Key);
        settingsModified |= EnsureMissingKeyBindings(kvp.Value, loadedKeyBindings);
      }

      if (settingsModified)
      {
        loadedSettings.Save();
      }
    }

    private static bool EnsureMissingKeyBindings(List<KeyBinding> defaultKeyBindings, List<KeyBinding> loadedKeyBindings)
    {
      bool hadMissingKeyBindings = false;
      foreach (KeyBinding defaultKeybinding in defaultKeyBindings)
      {
        if (loadedKeyBindings.Any(v => v.Action == defaultKeybinding.Action))
          continue;

        loadedKeyBindings.Add(defaultKeybinding);
        hadMissingKeyBindings = true;
      }

      return hadMissingKeyBindings;
    }

    private static Stream GetDefaultSettings()
    {
      return typeof(Settings).Assembly.GetManifestResourceStream("bonsai.Resources.default_settings.json")!;
    }

    private static ActionType? GetInputActionInternal(ConsoleKeyInfo keyInfo, ConsoleModiferDictionary keyBindings)
    {
      if (!keyBindings.TryGetValue(
            keyInfo.Modifiers,
            out (Dictionary<char, ActionType> ByChar, Dictionary<ConsoleKey, ActionType> ByKey) modifierByContext))
      {
        return null;
      }

      if (modifierByContext.ByChar.TryGetValue(keyInfo.KeyChar, out ActionType byChar))
      {
        return byChar;
      }

      if (modifierByContext.ByKey.TryGetValue(keyInfo.Key, out ActionType byKey))
      {
        return byKey;
      }

      return null;
    }

    private static string GetModifierText(ConsoleModifiers modifier)
    {
      return modifier switch
      {
        ConsoleModifiers.None => "",
        ConsoleModifiers.Alt => "⌥",
        ConsoleModifiers.Control => "^",
        ConsoleModifiers.Shift => "⇧",
        _ => ""
      };
    }

    private static List<KeyBinding> GetOrCreateKeyBindingContextEntry(Settings loadedSettings, KeyBindingContext keyBindingContext)
    {
      if (!loadedSettings.KeyBindings.TryGetValue(keyBindingContext, out List<KeyBinding>? loadedKeyBindings))
      {
        loadedKeyBindings = new List<KeyBinding>();
        loadedSettings.KeyBindings[keyBindingContext] = loadedKeyBindings;
      }

      return loadedKeyBindings;
    }

    private static AbsolutePath GetSettingsFilePath()
    {
      AbsolutePath path = KnownPaths.BonsaiConfigFolder;
      path.EnsureDirectoryExists();
      AbsolutePath settingsFilePath = path / "settings.json";
      return settingsFilePath;
    }

    private readonly Lazy<KeyBindingContextDictionary> _keyBindings;

    public Settings()
    {
      _keyBindings =
        new Lazy<KeyBindingContextDictionary>(
          () =>
          {
            KeyBindingContextDictionary result = new();

            foreach (KeyBindingContext context in Enum.GetValues<KeyBindingContext>())
            {
              if (KeyBindings.TryGetValue(context, out List<KeyBinding>? bindings))
              {
                result[context] = BuildKeyBindingDictionary(bindings);
              }
              else
              {
                result[context] = new ConsoleModiferDictionary();
              }
            }

            return result;
          });
    }

    public bool ShowParentDirectoryInList { get; set; }

    public string Theme { get; set; } = "default.json";

    [JsonInclude]
    [JsonPropertyOrder(100)]
    public Dictionary<KeyBindingContext, List<KeyBinding>> KeyBindings { get; set; } =
      new();

    public int MaxTotalScoreInDatabase { get; set; } = 2000;

    public int MaxIndividualScore { get; set; } = 100;

    public int MaxEntryAgeInDays { get; set; } = 300;

    public bool ShowCommandSelectionForDirectNavigation { get; set; }

    [JsonInclude]
    public IReadOnlyList<FileCommand> FileCommands { get; set; } = new List<FileCommand>();

    [JsonInclude]
    public IReadOnlyList<DirectoryCommand> DirectoryCommands { get; set; } = new List<DirectoryCommand>();

    public ActionType GetInputActionType(ConsoleKeyInfo keyInfo, KeyBindingContext keyBindingContext)
    {
      ConsoleModiferDictionary keyBindingsByContext = _keyBindings.Value[keyBindingContext];
      ActionType? result = GetInputActionInternal(keyInfo, keyBindingsByContext);
      if (result.HasValue)
      {
        return result.Value;
      }

      ConsoleModiferDictionary commonKeyBindings = _keyBindings.Value[KeyBindingContext.Common];
      result = GetInputActionInternal(keyInfo, commonKeyBindings);
      return result ?? ActionType.None;
    }

    public string GetInstructionForAction(KeyBindingContext keyBindingContext, ActionType action, string description)
    {
      if (KeyBindings.TryGetValue(keyBindingContext, out List<KeyBinding>? inner))
      {
        KeyBinding? result = inner.Where(k => k.Action == action).OrderByDescending(k => k.Key != null).FirstOrDefault();

        if (result == null)
        {
          return $"[MISSING {keyBindingContext}.{action}]";
        }

        string modifier = GetModifierText(result.Modifier);
        string key = GetKeySymbol(result);

        return $"[{modifier}{key}] {description}";
      }

      return string.Empty;
    }

    private string GetKeySymbol(KeyBinding keyBinding)
    {
      if (keyBinding.Key.HasValue && s_keySymbols.TryGetValue(keyBinding.Key.Value, out string? value))
      {
        return value;
      }

      return (keyBinding.Key?.ToString() ?? keyBinding.KeyChar.ToString()) ?? string.Empty;
    }

    public void Save()
    {
      AbsolutePath settingsPath = GetSettingsFilePath();
      File.WriteAllText(settingsPath, JsonSerializer.Serialize(this, SettingsGenerationContext.Default.Settings));
    }

    private static readonly Dictionary<ConsoleKey, string> s_keySymbols = new()
{
    // Control keys
    { ConsoleKey.Enter,          "↵" },
    { ConsoleKey.Escape,         "⎋" },
    { ConsoleKey.Backspace,      "⌫" },
    { ConsoleKey.Tab,            "↹" },
    { ConsoleKey.Spacebar,       "␣" },

    // Arrows
    { ConsoleKey.UpArrow,        "↑" },
    { ConsoleKey.DownArrow,      "↓" },
    { ConsoleKey.LeftArrow,      "←" },
    { ConsoleKey.RightArrow,     "→" },

    // Function keys
    { ConsoleKey.F1,  "F1" }, { ConsoleKey.F2,  "F2" },
    { ConsoleKey.F3,  "F3" }, { ConsoleKey.F4,  "F4" },
    { ConsoleKey.F5,  "F5" }, { ConsoleKey.F6,  "F6" },
    { ConsoleKey.F7,  "F7" }, { ConsoleKey.F8,  "F8" },
    { ConsoleKey.F9,  "F9" }, { ConsoleKey.F10, "F10" },
    { ConsoleKey.F11, "F11" }, { ConsoleKey.F12, "F12" },

    // Digits
    { ConsoleKey.D0, "0" }, { ConsoleKey.D1, "1" },
    { ConsoleKey.D2, "2" }, { ConsoleKey.D3, "3" },
    { ConsoleKey.D4, "4" }, { ConsoleKey.D5, "5" },
    { ConsoleKey.D6, "6" }, { ConsoleKey.D7, "7" },
    { ConsoleKey.D8, "8" }, { ConsoleKey.D9, "9" },

    // Letters (A–Z): single character already
    { ConsoleKey.A, "a" }, { ConsoleKey.B, "b" }, { ConsoleKey.C, "c" },
    { ConsoleKey.D, "d" }, { ConsoleKey.E, "e" }, { ConsoleKey.F, "f" },
    { ConsoleKey.G, "g" }, { ConsoleKey.H, "h" }, { ConsoleKey.I, "i" },
    { ConsoleKey.J, "j" }, { ConsoleKey.K, "k" }, { ConsoleKey.L, "l" },
    { ConsoleKey.M, "m" }, { ConsoleKey.N, "n" }, { ConsoleKey.O, "o" },
    { ConsoleKey.P, "p" }, { ConsoleKey.Q, "q" }, { ConsoleKey.R, "r" },
    { ConsoleKey.S, "s" }, { ConsoleKey.T, "t" }, { ConsoleKey.U, "u" },
    { ConsoleKey.V, "v" }, { ConsoleKey.W, "w" }, { ConsoleKey.X, "x" },
    { ConsoleKey.Y, "y" }, { ConsoleKey.Z, "z" },

    // Numpad
    { ConsoleKey.NumPad0, "0" }, { ConsoleKey.NumPad1, "1" },
    { ConsoleKey.NumPad2, "2" }, { ConsoleKey.NumPad3, "3" },
    { ConsoleKey.NumPad4, "4" }, { ConsoleKey.NumPad5, "5" },
    { ConsoleKey.NumPad6, "6" }, { ConsoleKey.NumPad7, "7" },
    { ConsoleKey.NumPad8, "8" }, { ConsoleKey.NumPad9, "8" },

    { ConsoleKey.Add,      "+" },
    { ConsoleKey.Subtract, "−" },
    { ConsoleKey.Multiply, "×" },
    { ConsoleKey.Divide,   "÷" },
    { ConsoleKey.Decimal,  "." },

    // Punctuation / OEM keys (these vary by keyboard)
    { ConsoleKey.OemPlus,        "+" },
    { ConsoleKey.OemMinus,       "−" },
    { ConsoleKey.OemComma,       "," },
    { ConsoleKey.OemPeriod,      "." },
    
    // Misc navigation
    { ConsoleKey.Home,      "POS1" },
    { ConsoleKey.End,       "END" },
    { ConsoleKey.PageUp,    "PgU" },
    { ConsoleKey.PageDown,  "PdD" },
    { ConsoleKey.Insert,    "Ins" },
    { ConsoleKey.Delete,    "Del" },

    // Print/Break
    { ConsoleKey.PrintScreen, "PrtSc" },
    { ConsoleKey.Pause,       "Pause" },
};

  }

  public class KeyBinding
  {
    public ConsoleModifiers Modifier { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public char? KeyChar { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleKey? Key { get; set; }
    public ActionType Action { get; set; }
  }

  public enum KeyBindingContext
  {
    Common,
    ExplorerApp,
    NavigationApp,
    EditDatabaseApp
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
    DeleteDatabaseEntry,
    ToggleFilePreview,
    FilePreviewScrollDown,
    FilePreviewScrollUp
  }
}