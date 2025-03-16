using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using bonsai.Data;
using bonsai.Theme;
using bonsai.Utilities;
using clui;
using clui.Controls;
using clui.Extensions;
using consoleTools;

namespace bonsai.Apps
{
  internal class EditDatabaseApp : AppBase, IBonsaiContext
  {
    private readonly NavigationEntryDescendingSortComparer _navigationEntryComparer = new();
    private readonly SearchFilterProvider<DatabaseEntry> _searchFilterProvider = new();
    private Border _border = null!;
    private ScrollableList<DatabaseEntry> _listControl = null!;
    private TextBox _searchTextBox = null!;

    protected override IBonsaiContext Context => this;

    public bool IsFilteringActive => _searchFilterProvider.IsFilterActive;

    protected override string? RunInternal ()
    {
      using (Frame frame = new())
      {
        CreateUi (frame);

        _listControl.SetItemList (Database.Instance.Entries.Order (_navigationEntryComparer).ToList());
        _listControl.SetFocusedIndex (0);

        frame.RenderComplete();

        _searchTextBox.OnTextChanged += SearchTextBox_OnTextChanged;
        UpdateBorder();
        bool endLoop = false;
        while (!endLoop)
        {
          ConsoleKeyInfo key = ConsoleHandler.Read();

          if (_searchTextBox.HandleInput (key))
          {
            continue;
          }

          switch (Settings.Instance.GetInputActionType (key, KeyBindingContext.EditDatabaseApp))
          {
            case ActionType.Exit:
              endLoop = true;
              continue;

            case ActionType.IncrementScore:
              ChangeScore (1);
              break;

            case ActionType.DecrementScore:
              ChangeScore (-1);
              break;

            case ActionType.DeleteDatabaseEntry:
              if (_listControl.FocusedItem == null)
              {
                continue;
              }

              int index = _listControl.FocusedItemIndex;
              _listControl.Items!.Remove (_listControl.FocusedItem);
              Database.Instance.Entries.Remove (_listControl.FocusedItem);
              _listControl.SetFocusedIndex (index);
              UpdateBorder();
              frame.RenderPartial (_border);
              break;
            case ActionType.SaveDatabaseChanges:
              Database.Instance.CleanUpDatabase (true);

              endLoop = true;
              break;
            case ActionType.ConfirmSelection:
              continue;

            case ActionType.ListSelectPreviousItem:
              _listControl.SelectPreviousItem();
              continue;
            case ActionType.ListSelectNextItem:
              _listControl.SelectNextItem();
              continue;
            case ActionType.ListSelectOnePageUp:
              _listControl.SelectPageUp();
              continue;
            case ActionType.ListSelectOnePageDown:
              _listControl.SelectPageDown();
              continue;
            case ActionType.ListSelectFirstItem:
              _listControl.SelectFirstItem();
              continue;
            case ActionType.ListSelectLastItem:
              _listControl.SelectLastItem();
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

    private void UpdateBorder ()
    {
      if (IsFilteringActive)
      {
        _border.Text = $"❮ {_listControl.Items!.Count} / {Database.Instance.Entries.Count} ❯";
      }
      else
      {
        _border.Text = $"❮ {Database.Instance.Entries.Count} ❯";
      }

      _border.RootControl.AssociatedFrame.RenderPartial (_border);
    }

    private void SearchTextBox_OnTextChanged (object sender, string text)
    {
      (bool filterChanged, Func<DatabaseEntry, SearchMatch[]>? filter, bool _) = _searchFilterProvider.GetFilter (text, false);

      if (!filterChanged || filter == null)
      {
        return;
      }

      if (_searchFilterProvider.IsFilterActive)
      {
        _listControl.SetItemList (
            Database.Instance.Entries.Where (
                e =>
                {
                  SearchMatch[] matches = filter (e);
                  e.SetSearchMatches (matches);
                  return matches.Length > 0;
                }).Order (_navigationEntryComparer).ToList());
      }
      else
      {
        _listControl.SetItemList (Database.Instance.Entries.Order (_navigationEntryComparer).ToList());
      }

      _listControl.SetFocusedIndex (0);

      UpdateBorder();
    }

    private void ChangeScore (int value)
    {
      DatabaseEntry? focusedItem = _listControl.FocusedItem;
      if (focusedItem == null)
      {
        return;
      }

      focusedItem.Score += value;
      if (focusedItem.Score < 0)
      {
        focusedItem.Score = 0;
      }

      if (focusedItem.Score > Settings.Instance.MaxIndividualScore)
      {
        focusedItem.Score = Settings.Instance.MaxIndividualScore;
      }

      if (_listControl.Items is not List<DatabaseEntry> source)
      {
        return;
      }

      source.Sort (_navigationEntryComparer);
      _listControl.SetFocusedIndex (source.IndexOf (focusedItem));
    }

    private void CreateUi (Frame frame)
    {
      Panel rootPanel = new(1.AsFraction(), 1.AsFraction());
      frame.AddControls (rootPanel);

      Label titleLabel = new(1.AsFraction(), 1.AsFixed());

      _border = new Border(1.AsFraction(), 1.AsFraction())
      {
        BorderColor = ThemeManger.Instance.BorderColor,
        TextPosition = BorderTextPosition.BottomLeft
      };
      titleLabel.Text = "Edit bonsai database";

      Panel topPanel = new(1.AsFraction(), 1.AsFixed())
      {
        BackgroundColor = ThemeManger.Instance.TopBarBackgroundColor,
        TextColor = ThemeManger.Instance.TopBarTextColor
      };
      topPanel.AddControls (titleLabel);

      string increment = Settings.Instance.GetInstructionForAction (KeyBindingContext.EditDatabaseApp, ActionType.IncrementScore, "increment");
      string decrement = Settings.Instance.GetInstructionForAction (KeyBindingContext.EditDatabaseApp, ActionType.DecrementScore, "decrement");
      string delete = Settings.Instance.GetInstructionForAction (KeyBindingContext.EditDatabaseApp, ActionType.DeleteDatabaseEntry, "delete");
      string save = Settings.Instance.GetInstructionForAction (KeyBindingContext.EditDatabaseApp, ActionType.SaveDatabaseChanges, "save and exit");

      Label instructionsLabel = new(1.AsFraction(), 1.AsFixed())
      {
        Text = $"{increment}   {decrement}   {delete}   {save}",
        TextColor = Color.CadetBlue
      };

      _listControl = new ScrollableList<DatabaseEntry> (
          ThemeManger.Instance.SelectionForegroundColor,
          ThemeManger.Instance.SelectionBackgroundColor,
          1.AsFraction(),
          1.AsFraction());

      rootPanel.BackgroundColor = ThemeManger.Instance.BackgroundColor;

      Label title = new(1.AsFraction(), 1.AsFixed())
      {
        Text = "LAST USED         SCORE PATH",
        TextColor = Color.DodgerBlue
      };
      //"yyyy-MM-dd HH:mm"
      _border.AddControls (title, _listControl);

      Panel searchPanel = new(1.AsFraction(), 1.AsFixed())
      {
        Flow = ChildControlFlow.Horizontal
      };
      Label searchLabel = new(9.AsFixed(), 1.AsFixed())
      {
        Text = "Search ❯",
        TextColor = ThemeManger.Instance.SearchLabelTextColor,
        BackgroundColor = ThemeManger.Instance.SearchLabelBackgroundColor
      };

      _searchTextBox = new TextBox (1.AsFraction(), 1.AsFixed());
      searchPanel.AddControls (searchLabel, _searchTextBox);
      rootPanel.AddControls (topPanel, searchPanel, _border, instructionsLabel);

      frame.EnableBufferSizeChangeWatching();

      frame.SetFocus (_searchTextBox);
    }
  }

  internal class NavigationEntryDescendingSortComparer : IComparer<DatabaseEntry>
  {
    public int Compare (DatabaseEntry? x, DatabaseEntry? y)
    {
      if (x == null || y == null)
      {
        return 0;
      }

      if (x.Score == y.Score)
      {
        if (x.LastUsed == y.LastUsed)
        {
          return string.Compare (x.Path, y.Path, StringComparison.OrdinalIgnoreCase);
        }

        return x.LastUsed < y.LastUsed ? 1 : -1;
      }

      if (x.Score < y.Score)
      {
        return 1;
      }

      return -1;
    }
  }
}