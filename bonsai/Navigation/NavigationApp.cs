using System.IO;
using System;
using bonsai.Theme;
using clui;
using clui.Controls;
using clui.Extensions;
using consoleTools;
using bonsai.Explorer;

namespace bonsai.Navigation
{
  public class NavigationApp
  {
    public string? Run(string currentDirectory, string originalArg)
    {
      Directory.SetCurrentDirectory(currentDirectory);

      var isExistingPathResult = IsExistingPath(currentDirectory, originalArg);
      if (isExistingPathResult.success)
        return isExistingPathResult.result;

      NavigationDatabase.Instance.CleanUpDatabase();

      var foundEntries = NavigationDatabase.Instance.Search(originalArg);

      if (foundEntries.Length == 0)
        return null;

      if (foundEntries.Length == 1)
      {
        var firstEntry = foundEntries[0];
        return firstEntry.FullName;
      }

      using (var frame = new Frame())
      {
        Panel rootPanel = new Panel(1.AsFraction(), 1.AsFraction());
        frame.AddControls(rootPanel);

        Label searchTermLabel = new Label(1.AsFraction(), 1.AsFixed());

        Border border = new Border(1.AsFraction(), 1.AsFraction());
        border.BorderColor = ThemeManger.Instance.BorderColor;
        searchTermLabel.Text = "Searched  ❯ " + originalArg;

        ScrollableList<FileSystemItem> list = new ScrollableList<FileSystemItem>(ThemeManger.Instance.SelectionForegroundColor, ThemeManger.Instance.SelectionBackgroundColor, 1.AsFraction(), 1.AsFraction());
        rootPanel.BackgroundColor = ThemeManger.Instance.BackgroundColor;
        border.AddControls(list);

        rootPanel.AddControls(searchTermLabel, border);

        frame.EnableBufferSizeChangeWatching();

        list.SetItemList(foundEntries);
        list.SetFocusedIndex(0);

        frame.RenderComplete();
        bool endLoop = false;

        while (!endLoop)
        {
          ConsoleKeyInfo key = ConsoleHandler.Read();

          switch (Settings.Instance.GetInputActionType(key, KeyBindingContext.NavigationApp))
          {
            case ActionType.Exit:
              endLoop = true;
              continue;

            case ActionType.ConfirmSelection:
              if (list.FocusedItem == null)
                break;


              var focusedItem = list.FocusedItem;
              switch (focusedItem)
              {
                case DirectoryItem:
                case FileItem:
                  return focusedItem.FullName;
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

    private static (bool success, string result) IsExistingPath(string currentDirectory, string originalArg)
    {
      var path = originalArg;
      if (!Path.IsPathFullyQualified(path))
        path = Path.GetFullPath(originalArg, currentDirectory);

      if (Directory.Exists(path))
        return (true, path);

      if (File.Exists(path))
        return (true, path);

      return (false, string.Empty);
    }
  }
}