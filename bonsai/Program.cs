using System;
using System.IO;
using System.Text;
using bonsai;
using bonsai.CommandHandling;
using bonsai.Explorer;
using bonsai.Navigation;
using bonsai.Theme;
using consoleTools;

var originalInputEncoding = Console.InputEncoding;
var originalOutputEncoding = Console.OutputEncoding;

try
{
  Console.OutputEncoding = Encoding.UTF8;
  Console.InputEncoding = Encoding.UTF8;

  Settings.LoadSettings();
  ThemeManger.LoadTheme (Settings.Instance.Theme);
  ConsoleHandler.StartOperation();

  if (args.Length == 0)
  {
    Console.WriteLine (new ExplorerApp().Run(Directory.GetCurrentDirectory()));
    return 0;
  }

  var command = args[0];

  string tempFilePath;
  string? selectedPath;
  string currentDirectory;

  switch(command)
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

      new EditDatabaseApp().Run();

      return 0;

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
      var searchArgs = args[3];
      selectedPath = new NavigationApp().Run (currentDirectory, searchArgs);
      break;

    case "main":
      if (args.Length != 3)
      {
        Console.WriteLine ($"Command '{command}' requires two arguments. 1. Temporary file path, 2. Current location");
        return 1;
      }

      tempFilePath = args[1];
      currentDirectory = args[2];
      selectedPath = new ExplorerApp().Run (currentDirectory);
      break;

    default:
      Console.WriteLine ($"Unknown command '{command}'");
      return 1;
  }


  if (!string.IsNullOrWhiteSpace (selectedPath))
  {
    var commands = CommandHandler.CreateCommands (selectedPath);
    if (commands.Count == 0)
      return 0;

    var selectedCommand = commands[0];
    if (commands.Count > 1)
      selectedCommand = new CommandSelectionApp().Run (commands, selectedPath);

    if (selectedCommand != null)
    {
      if (File.Exists (tempFilePath))
        File.WriteAllText (tempFilePath, selectedCommand.GetExecutableAction());
      else
        Console.WriteLine (selectedCommand.Action);
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
