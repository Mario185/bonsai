using System.IO;
using System;
using bonsai.CommandHandling;
using bonsai.Theme;
using clui;
using clui.Controls;
using clui.Extensions;
using consoleTools;
using bonsai.Explorer;

namespace bonsai.Navigation
{
  internal class NavigationApp : AppBase, IBonsaiContext
  {
    private readonly string _currentDirectory;
    private readonly string _originalArg;

    public NavigationApp(string currentDirectory, string originalArg)
    {
      _currentDirectory = currentDirectory;
      _originalArg = originalArg;
    }

    protected override IBonsaiContext Context => this;

    protected override string? RunInternal()
    {
      var isExistingPathResult = IsExistingPath(_currentDirectory, _originalArg);
      if (isExistingPathResult.success)
        return isExistingPathResult.result;

      NavigationDatabase.Instance.CleanUpDatabase();

      var foundEntries = NavigationDatabase.Instance.Search(_originalArg);

      if (foundEntries.Length == 0)
        return null;

      if (foundEntries.Length == 1)
      {
        var firstEntry = foundEntries[0];
        return firstEntry.FullName;
      }

      using (var frame = new Frame())
      {
        var list = CreateUi(_originalArg, frame, foundEntries);

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
                  return CommandHandler.GetCommandAndShowSelectionUiOnDemand(focusedItem.FullName);
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
      var rootPanel = new Panel(1.AsFraction(), 1.AsFraction());
      frame.AddControls(rootPanel);

      var searchTermLabel = new Label(1.AsFraction(), 1.AsFixed());

      var border = new Border(1.AsFraction(), 1.AsFraction());
      border.BorderColor = ThemeManger.Instance.BorderColor;
      searchTermLabel.Text = "Searched  ❯ " + originalArg;

      var list = new ScrollableList<FileSystemItem>(ThemeManger.Instance.SelectionForegroundColor, ThemeManger.Instance.SelectionBackgroundColor, 1.AsFraction(), 1.AsFraction());
      rootPanel.BackgroundColor = ThemeManger.Instance.BackgroundColor;
      border.AddControls(list);

      rootPanel.AddControls(searchTermLabel, border);

      frame.EnableBufferSizeChangeWatching();

      list.SetItemList(foundEntries);
      list.SetFocusedIndex(0);
      return list;
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

    public bool IsFilteringActive => true;
  }
}