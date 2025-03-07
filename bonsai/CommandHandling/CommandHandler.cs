using System.Collections.Generic;
using System.IO;
using System.Linq;
using bonsai.Navigation;

namespace bonsai.CommandHandling
{
  public class CommandHandler
  {
    public static IReadOnlyList<Command> CreateCommands(string path)
    {
      try
      {
        var attributes = File.GetAttributes(path);
        var isDirectory = (attributes & FileAttributes.Directory) != 0;

        NavigationDatabase.Instance.AddOrUpdate(path, isDirectory);

        if (isDirectory)
        {
          if (Settings.Instance.DirectoryCommands.Count > 0)
            return Settings.Instance.DirectoryCommands.Select(c => c.CloneForExecution(path)).ToArray();

          return [new DirectoryCommand(path, "Change location", true)];
        }

        var extension = Path.GetExtension(path);
        var fileCommands = Settings.Instance.FileCommands.Where(c => c.Extension == extension || c.Extension == "*").Select(f => f.CloneForExecution(path)).ToArray();
        if (fileCommands.Any())
          return fileCommands.ToArray();

        return [new FileCommand(path, "Shell decides what to do", extension, true)];
      }
      catch
      {
        // do nothing
      }

      return [];
    }
  }
}