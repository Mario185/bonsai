using System;
using System.IO;
using bonsai.CommandHandling;
using bonsai.Data;
using bonsai.FileSystemHandling;
using bonsai.Theme;
using clui;
using clui.Controls;
using clui.Extensions;
using consoleTools;

namespace bonsai.Apps
{
  internal class NavigationApp : AppBase, IBonsaiContext
  {
    private readonly string _currentDirectory;
    private readonly string _originalArg;

    public NavigationApp (string currentDirectory, string originalArg)
    {
      _currentDirectory = currentDirectory;
      _originalArg = originalArg;
    }

    protected override IBonsaiContext Context => this;

    public bool IsFilteringActive => true;

    protected override string? RunInternal ()
    {
      Directory.SetCurrentDirectory (_currentDirectory);
      (bool success, bool isDirectory, string result) = IsExistingPath (_currentDirectory, _originalArg);
      if (success)
      {
        if (isDirectory && !Settings.Instance.ShowCommandSelectionForDirectNavigation)
          return CommandHandler.GetDefaultChangeDirectoryCommand(result);

        return CommandHandler.GetCommandAndShowSelectionUiOnDemand (result);
      }

      Database.Instance.CleanUpDatabase();

      FileSystemItem[] foundEntries = Database.Instance.Search (_originalArg);

      if (foundEntries.Length == 0)
      {
        return null;
      }

      if (foundEntries.Length == 1)
      {
        FileSystemItem firstEntry = foundEntries[0];
        return CommandHandler.GetCommandAndShowSelectionUiOnDemand (firstEntry.FullName);
      }

      using (Frame frame = new())
      {
        ScrollableList<FileSystemItem> list = CreateUi (_originalArg, frame, foundEntries);

        frame.RenderComplete();
        bool endLoop = false;

        while (!endLoop)
        {
          ConsoleKeyInfo key = ConsoleHandler.Read(true);

          switch (Settings.Instance.GetInputActionType (key, KeyBindingContext.NavigationApp))
          {
            case ActionType.Exit:
              endLoop = true;
              continue;

            case ActionType.ConfirmSelection:
              if (list.FocusedItem == null)
              {
                break;
              }

              FileSystemItem? focusedItem = list.FocusedItem;
              switch (focusedItem)
              {
                case DirectoryItem:
                case FileItem:
                  return CommandHandler.GetCommandAndShowSelectionUiOnDemand (focusedItem.FullName);
              }

              endLoop = true;
              continue;

            case ActionType.ListSelectPreviousItem:
              list.SelectPreviousItem();
              continue;
            case ActionType.ListSelectNextItem:
              list.SelectNextItem();
              continue;
            case ActionType.ListSelectOnePageUp:
              list.SelectPageUp();
              continue;
            case ActionType.ListSelectOnePageDown:
              list.SelectPageDown();
              continue;
            case ActionType.ListSelectFirstItem:
              list.SelectFirstItem();
              continue;
            case ActionType.ListSelectLastItem:
              list.SelectLastItem();
              continue;
            case ActionType.None:
            default:
              // do nothing, it is just a fallback
              break;
          }
        }
      }

      return null;
    }

    private static ScrollableList<FileSystemItem> CreateUi (string originalArg, Frame frame, FileSystemItem[] foundEntries)
    {
      Panel rootPanel = new(1.AsFraction(), 1.AsFraction());
      frame.AddControls (rootPanel);

      Label searchTermLabel = new(1.AsFraction(), 1.AsFixed());

      Border border = new(1.AsFraction(), 1.AsFraction())
                      {
                          BorderColor = ThemeManger.Instance.BorderColor
                      };
      searchTermLabel.Text = "Searched  ❯ " + originalArg;

      ScrollableList<FileSystemItem> list = new(
          ThemeManger.Instance.SelectionForegroundColor,
          ThemeManger.Instance.SelectionBackgroundColor,
          1.AsFraction(),
          1.AsFraction());
      rootPanel.BackgroundColor = ThemeManger.Instance.BackgroundColor;
      border.AddControls (list);

      rootPanel.AddControls (searchTermLabel, border);

      frame.EnableBufferSizeChangeWatching();

      list.SetItemList (foundEntries);
      list.SetFocusedIndex (0);
      return list;
    }

    private static (bool success, bool isDirectory, string result) IsExistingPath (string currentDirectory, string originalArg)
    {
      string path = originalArg;
      if (!Path.IsPathFullyQualified (path))
      {
        path = Path.GetFullPath (originalArg, currentDirectory);
      }

      if (Directory.Exists (path))
      {
        return (true, true, path);
      }

      if (File.Exists (path))
      {
        return (true, false, path);
      }

      return (false, false, string.Empty);
    }
  }
}