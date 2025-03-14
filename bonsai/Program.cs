using System;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
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
    Console.WriteLine(new ExplorerApp(Directory.GetCurrentDirectory()).Run());
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
      selectedPath = new NavigationApp(currentDirectory, searchArgs).Run ();
      break;

    case "main":
      if (args.Length != 3)
      {
        Console.WriteLine ($"Command '{command}' requires two arguments. 1. Temporary file path, 2. Current location");
        return 1;
      }

      tempFilePath = args[1];
      currentDirectory = args[2];
      selectedPath = new ExplorerApp(currentDirectory).Run();

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

    var selectedCommand = commands[0].GetExecutableAction();
    if (commands.Count > 1)
      selectedCommand = new CommandSelectionApp(commands, selectedPath).Run ();

    if (selectedCommand != null)
    {
      if (File.Exists (tempFilePath))
        File.WriteAllText (tempFilePath, selectedCommand);
      else
        Console.WriteLine (selectedCommand);
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
