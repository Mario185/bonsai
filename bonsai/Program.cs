using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using bonsai;
using bonsai.CommandHandling;
using bonsai.Explorer;
using bonsai.Navigation;
using bonsai.Theme;
using clui;
using clui.Controls;
using clui.Extensions;
using clui.Layout;
using consoleTools;


//Debugger.Launch();
//var inputMode = consoleTools.Windows.ConsoleReader.GetInputMode();
//if (inputMode.HasValue)
//{
//  //ConsoleHelper.SetInputMode(InputModeType.ENABLE_EXTENDED_FLAGS);
//  var newMode = (InputModeType.ENABLE_VIRTUAL_TERMINAL_INPUT);


//  var s1 = ConsoleHelper.SetInputMode(newMode);

//  var success = (ConsoleHelper.GetInputMode() & InputModeType.ENABLE_MOUSE_INPUT) != 0;
//}

//Console.OutputEncoding = Encoding.UTF8;
//Console.InputEncoding = Encoding.UTF8;

//IntPtr? fileStreamHandle = null;
//string pipedContent = "";
//if (Console.IsInputRedirected)
//{
//  pipedContent = Console.In.ReadToEnd();

//  // enable console input again
//  string inputPath = "CONIN$";
//  if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
//    inputPath = "/dev/tty";

//  FileStream fileStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096 * 100000);
//  fileStreamHandle = fileStream.SafeFileHandle.DangerousGetHandle();
//  Console.SetIn(new StreamReader(fileStream, Encoding.UTF8));

//}

//while (true)
//{
//  var result = ConsoleHelper.Read(fileStreamHandle);
//  if (result.HasValue &&  result.Value.UnicodeChar == 'q')
//  {
//    break;
//  }

//  if (result.HasValue && result.Value.UnicodeChar != '0')
//  {
//   // Console.Write(result.Value.UnicodeChar + " ");
//  }
//}

//ConsoleHelper.SetInputMode(inputMode.Value);
//return 0;

if (args is ["init", "powershell"])
{
  using (StreamReader stream = new(typeof(Program).Assembly.GetManifestResourceStream("bonsai.Resources.Init_powershell.ps1")!))
  {
    Console.Write(stream.ReadToEnd());
    return 0;
  }
}

//var originalOutputMode = consoleTools.Windows.ConsoleReader.GetOutputMode();
//var originalInputMode = consoleTools.Windows.ConsoleReader.GetInputMode();

var inputEncoding = Console.InputEncoding;
var outputEncoding = Console.OutputEncoding;

try
{
  Settings.LoadSettings();
  ThemeManger.LoadTheme(Settings.Instance.Theme);
  //var ts = Stopwatch.GetTimestamp();

  //NavigationDatabase.Instance.CleanUpDatabase();
  //Console.WriteLine(Stopwatch.GetElapsedTime(ts));

  //Random rnd = new Random();
  //for (int i = 0; i < 1000; i++)
  //{
  //  var rndV = rnd.Next(1, 50);
  //  for (int y = 0; y < rndV; y++)
  //  {
  //    NavigationDatabase.Instance.AddOrUpdate(new DirectoryInfo("D:\\gitea\\bonsai\\bonsai\\bin\\Debug\\net9.0\\.bonsai" + i), false);
  //  }
  //}
  //NavigationDatabase.Instance.Save();


  Console.OutputEncoding = Encoding.UTF8;
  Console.InputEncoding = Encoding.UTF8;

  //if (originalOutputMode.HasValue && (originalOutputMode.Value & OutputModeType.ENABLE_VIRTUAL_TERMINAL_PROCESSING) == 0)
  //{
  //  var newOutputMode = originalOutputMode.Value & OutputModeType.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
  //  consoleTools.Windows.ConsoleReader.SetOutputMode(newOutputMode);
  //}

  
  string? selectedPath;
  ConsoleHandler.StartOperation();

  if (args.Length == 4 && args[0] == "f" && !string.IsNullOrWhiteSpace(args[3]))
  {
    var currentDirectory = args[2];
    var originalArg = args[3];
    selectedPath = new NavigationApp().Run(currentDirectory, originalArg);
  }
  else
  {
    selectedPath = new ExplorerApp().Run();
  }

  if (!string.IsNullOrWhiteSpace(selectedPath))
  {
    var commands = CommandHandler.CreateCommands(selectedPath);
    if (commands.Count == 0)
      return 0;

    var selectedCommand = commands[0];
    if (commands.Count > 1)
      selectedCommand = new CommandSelectionApp().Run(commands, selectedPath);

    if (selectedCommand != null)
    {
      if (args.Length >= 2 && args[0] == "f" && !string.IsNullOrWhiteSpace(args[1]) && File.Exists(args[1]))
        File.WriteAllText(args[1], selectedCommand.GetExecutableAction());
      else
        Console.WriteLine(selectedCommand.Action);
    }
  }

  return 0;
}
catch (Exception ex)
{
  Console.WriteLine(ex.Message);
  Console.WriteLine(ex.StackTrace);

  return 1;
}
finally
{
  ConsoleHandler.CancelOperation();

  Console.InputEncoding = inputEncoding;
  Console.OutputEncoding = outputEncoding;

  //if (originalInputMode.HasValue)
  //  consoleTools.Windows.ConsoleReader.SetInputMode(originalInputMode.Value);

  //if (originalOutputMode.HasValue)
  //  consoleTools.Windows.ConsoleReader.SetOutputMode(originalOutputMode.Value);

}
