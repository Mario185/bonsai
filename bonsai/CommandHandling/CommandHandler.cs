﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using bonsai.Apps;
using bonsai.Data;

namespace bonsai.CommandHandling
{
  public class CommandHandler
  {
    public static string? GetCommandAndShowSelectionUiOnDemand (string path)
    {
      try
      {
        FileAttributes attributes = File.GetAttributes (path);
        bool isDirectory = (attributes & FileAttributes.Directory) != 0;

        Database.Instance.AddOrUpdate (path, isDirectory);

        List<Command> availableCommands;
        if (isDirectory)
        {
          if (Settings.Instance.DirectoryCommands.Count <= 0)
          {
            return new DirectoryCommand (path, "Change location", true).GetExecutableAction();
          }

          availableCommands = Settings.Instance.DirectoryCommands.Select (c => c.CloneForExecution (path)).Cast<Command>().ToList();
        }
        else
        {
          string extension = Path.GetExtension (path);
          availableCommands = Settings.Instance.FileCommands.Where (c => c.Extension == extension || c.Extension == "*")
              .Select (f => f.CloneForExecution (path)).Cast<Command>().ToList();
          if (availableCommands.Count == 0)
          {
            return new FileCommand (path, "Shell decides what to do", extension, true).GetExecutableAction();
          }
        }

        if (availableCommands.Count == 1)
        {
          return availableCommands[0].GetExecutableAction();
        }

        return new CommandSelectionApp (availableCommands, path).Run();
      }
      catch
      {
        // do nothing
      }

      return null;
    }
  }
}