using System.Collections.Generic;
using System.IO;
using System.Linq;
using bonsai.Apps;
using bonsai.Navigation;

namespace bonsai.CommandHandling
{
  public class CommandHandler
  {
    public static string? GetCommandAndShowSelectionUiOnDemand(string path)
    {
      try
      {
        var attributes = File.GetAttributes(path);
        var isDirectory = (attributes & FileAttributes.Directory) != 0;

        Database.Instance.AddOrUpdate(path, isDirectory);

        IReadOnlyList<Command> availableCommands;
        if (isDirectory)
        {
          if (Settings.Instance.DirectoryCommands.Count <= 0)
            return new DirectoryCommand(path, "Change location", true).GetExecutableAction();
          availableCommands = Settings.Instance.DirectoryCommands.Select(c => c.CloneForExecution(path)).Cast<Command>().ToList();
        }
        else
        {
          var extension = Path.GetExtension(path);
          availableCommands = Settings.Instance.FileCommands.Where(c => c.Extension == extension || c.Extension == "*").Select(f => f.CloneForExecution(path)).Cast<Command>().ToList();
          if (!availableCommands.Any())
            return new FileCommand(path, "Shell decides what to do", extension, true).GetExecutableAction();
        }

        if (availableCommands.Count == 1)
          return availableCommands[0].GetExecutableAction();

        return new CommandSelectionApp(availableCommands, path).Run();
      }
      catch
      {
        // do nothing
      }

      return null;
    }
  }
}