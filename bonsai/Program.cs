using System;
using System.IO;
using System.Text;
using bonsai;
using bonsai.Apps;
using bonsai.Theme;
using bonsai.Utilities;
using consoleTools;

Encoding originalInputEncoding = Console.InputEncoding;
Encoding originalOutputEncoding = Console.OutputEncoding;

try
{
  Console.OutputEncoding = Encoding.UTF8;
  Console.InputEncoding = Encoding.UTF8;

  Settings.LoadSettings();
  ThemeManger.LoadTheme (Settings.Instance.Theme);

  IConsole console = Console.IsInputRedirected || Console.IsOutputRedirected ? new FakeConsole() : new RealConsole();

  ConsoleHandler.Initialize(console);
  ConsoleHandler.StartOperation();

  if (args.Length == 0)
  {
    Console.WriteLine (new ExplorerApp (Directory.GetCurrentDirectory()).Run());
    return 0;
  }

  string command = args[0];

  string tempFilePath = string.Empty;
  string currentDirectory;
  AppBase? appToStart;
  switch (command)
  {
    case "init":
      if (args.Length != 2)
      {
        Console.WriteLine ($"Command '{command}' requires second argument. e.g powershell");
        return 1;
      }

      ShellInitializer.WriteInitializationScriptToConsole (args[1]);
      ThemeManger.WriteResourceThemesToConfigFolder (false);
      return 0;

    case "editdb":
      appToStart = new EditDatabaseApp();
      break;

    case "writethemes":
      ThemeManger.WriteResourceThemesToConfigFolder (true);
      return 0;

    case "nav":
      if (args.Length != 4)
      {
        Console.WriteLine ($"Command '{command}' requires three arguments. 1. Temporary file path, 2. Current location, 3. Search argument");
        return 1;
      }

      tempFilePath = args[1];
      currentDirectory = args[2];
      string searchArgs = args[3];
      appToStart = new NavigationApp (currentDirectory, searchArgs);
      break;

    case "main":
      if (args.Length != 3)
      {
        Console.WriteLine ($"Command '{command}' requires two arguments. 1. Temporary file path, 2. Current location");
        return 1;
      }

      tempFilePath = args[1];
      currentDirectory = args[2];
      appToStart = new ExplorerApp (currentDirectory);

      break;

    default:
      Console.WriteLine ($"Unknown command '{command}'");
      return 1;
  }

  string? result = appToStart.Run();

  if (!string.IsNullOrWhiteSpace (result))
  {
    if (File.Exists (tempFilePath))
    {
      File.WriteAllText (tempFilePath, result);
    }
    else
    {
      Console.WriteLine (result);
    }
  }

  return 0;
}
catch (Exception ex)
{
  Console.WriteLine (ex.Message);
  Console.WriteLine (ex.StackTrace);

  return 1;
}
finally
{
  ConsoleHandler.CancelOperation();

  Console.InputEncoding = originalInputEncoding;
  Console.OutputEncoding = originalOutputEncoding;
}

public class RealConsole : IConsole
{
  public int WindowHeight => Console.WindowHeight;
  public int WindowWidth => Console.WindowWidth;
  public ConsoleKeyInfo ReadKey(bool intercept)
  {
    return Console.ReadKey(intercept);
  }
}

public class FakeConsole : IConsole
{
  public int WindowHeight { get; } = 10;
  public int WindowWidth { get; } = 10;
  public ConsoleKeyInfo ReadKey(bool intercept)
  {
    return Console.ReadKey(intercept);
  }
}
