using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using bonsai.Explorer;
using bonsai.Theme;
using clui;
using clui.Controls;
using clui.Extensions;
using consoleTools;

namespace bonsai.Navigation
{
  public class EditDatabaseApp
  {
    public void Run()
    {
      using (var frame = new Frame())
      {
        NavigationEntryDescendingSortComparer navigationEntryComparer = new NavigationEntryDescendingSortComparer();
        List<NavigationEntry> navigationEntries = NavigationDatabase.Instance.Entries.OrderByDescending (e => e.Score).ToList();
        var list = CreateUi (frame, navigationEntries);
          
        frame.RenderComplete();

        bool endLoop = false;
        while (!endLoop)
        {
          ConsoleKeyInfo key = ConsoleHandler.Read();

          switch (Settings.Instance.GetInputActionType (key, KeyBindingContext.EditDatabaseApp))
          {
            case ActionType.Exit:
              endLoop = true;
              continue;

            case ActionType.IncrementScore:
              list.FocusedItem.Score++;

              navigationEntries.Sort (navigationEntryComparer);
              list.SetFocusedIndex (navigationEntries.IndexOf (list.FocusedItem));
              frame.RenderPartial (list);
              break;

            case ActionType.DecrementScore:
              list.FocusedItem.Score--;
              if (list.FocusedItem.Score < 0)
                list.FocusedItem.Score = 0;

              navigationEntries.Sort (navigationEntryComparer);
              list.SetFocusedIndex (navigationEntries.IndexOf (list.FocusedItem));
              frame.RenderPartial (list);
              break;

            case ActionType.DeleteDatabaseEntry:
              var index = list.FocusedItemIndex;
              navigationEntries.Remove (list.FocusedItem);
              list.SetFocusedIndex (index);

              break;
            case ActionType.SaveDatabaseChanges:
              NavigationDatabase.Instance.CleanUpDatabase();
              
              endLoop = true;
              break;
            case ActionType.ConfirmSelection:
              //if (list.FocusedItem == null)
              //  break;


              //var focusedItem = list.FocusedItem;
              //switch (focusedItem)
              //{
              //  case DirectoryItem:
              //  case FileItem:
              //    return focusedItem.FullName;
              //}

              //endLoop = true;
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
    }

    private static ScrollableList<NavigationEntry> CreateUi (Frame frame, List<NavigationEntry> navigationEntries)
    {
      var rootPanel = new Panel(1.AsFraction(), 1.AsFraction());
      frame.AddControls(rootPanel);

      var titleLabel = new Label(1.AsFraction(), 1.AsFixed());

      var border = new Border(1.AsFraction(), 1.AsFraction());
      border.BorderColor = ThemeManger.Instance.BorderColor;
      titleLabel.Text = "Edit database";

      var instructionsLabel = new Label (1.AsFraction(), 1.AsFixed());
      instructionsLabel.Text = "ctrl+\u2191:increment\tctrl+\u2193:decrement\tctrl+s:save and exit";
      instructionsLabel.TextColor = Color.CadetBlue;

      var list = new ScrollableList<NavigationEntry>(ThemeManger.Instance.SelectionForegroundColor, ThemeManger.Instance.SelectionBackgroundColor, 1.AsFraction(), 1.AsFraction());
      rootPanel.BackgroundColor = ThemeManger.Instance.BackgroundColor;

      Label title = new Label (1.AsFraction(), 1.AsFixed());
      title.Text = "LAST USED         SCORE PATH";
      title.TextColor = Color.DodgerBlue;
      //"yyyy-MM-dd HH:mm"
      border.AddControls(title, list);

      rootPanel.AddControls(titleLabel,instructionsLabel, border);

      frame.EnableBufferSizeChangeWatching();

      list.SetItemList(navigationEntries);
      list.SetFocusedIndex(0);
      return list;
    }

  }

  public class NavigationEntryDescendingSortComparer : IComparer<NavigationEntry>
  {

    public int Compare (NavigationEntry? x, NavigationEntry? y)
    {
      if (x.Score == y.Score) return 0;

      if (x.Score < y.Score)
        return 1;

      return -1;
    }
  }
}