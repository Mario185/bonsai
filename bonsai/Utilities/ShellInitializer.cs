using System;
using System.IO;

namespace bonsai.Utilities
{
  internal class ShellInitializer
  {
    public static void WriteInitializationScriptToConsole (string shellType)
    {
      Console.Write (GetInitializationScript (shellType));
    }

    public static string GetInitializationScript (string shellType)
    {
      string scriptName = GetScriptName (shellType);
      EnsureInitScriptFileExists (scriptName);
      return File.ReadAllText (GetInitScriptFilePath (scriptName));
    }

    private static void EnsureInitScriptFileExists (string scriptName)
    {
      AbsolutePath initScriptFilePath = GetInitScriptFilePath (scriptName);
      if (File.Exists (initScriptFilePath))
      {
        return;
      }

      using (StreamReader stream = new(typeof(Program).Assembly.GetManifestResourceStream ("bonsai.Resources.initialization." + scriptName)!))
      {
        File.WriteAllText (initScriptFilePath, stream.ReadToEnd());
      }
    }

    private static AbsolutePath GetInitScriptFilePath (string scriptName)
    {
      AbsolutePath configFolder = KnownPaths.BonsaiConfigFolder;
      configFolder.EnsureDirectoryExists();
      return configFolder / scriptName;
    }

    private static string GetScriptName (string shellType)
    {
      string extension = shellType switch
      {
          "powershell" => "ps1",
          _ => throw new NotSupportedException ($"Shell type: '{shellType}' is not supported.")
      };
      return "init." + extension;
    }
  }
}