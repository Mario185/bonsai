using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using bonsai.Theme;
using bonsai.Utilities;
using clui;
using clui.Controls;
using clui.Extensions;
using consoleTools;

namespace bonsai.Navigation
{
  internal class EditDatabaseApp : AppBase, IBonsaiContext
  {
    private readonly NavigationEntryDescendingSortComparer _navigationEntryComparer = new();
    private readonly SearchFilterProvider<NavigationEntry> _searchFilterProvider = new ();
    private ScrollableList<NavigationEntry> _listControl = null!;
    private Border _border = null!;
    private TextBox _searchTextBox = null!; 

    private List<NavigationEntry> _unfilteredList = null!;
    protected override IBonsaiContext Context => this;

    public bool IsFilteringActive => _searchFilterProvider.IsFilterActive;

    protected override string? RunInternal()
    {
      using (var frame = new Frame())
      {
        _unfilteredList = NavigationDatabase.Instance.Entries.Order(_navigationEntryComparer).ToList();

        CreateUi (frame);

        _listControl.SetItemList(_unfilteredList);
        _listControl.SetFocusedIndex(0);

        frame.RenderComplete();

        _searchTextBox.OnTextChanged += SearchTextBox_OnTextChanged;

        bool endLoop = false;
        while (!endLoop)
        {
          ConsoleKeyInfo key = ConsoleHandler.Read();

          if (_searchTextBox.HandleInput(key))
            continue;

          switch (Settings.Instance.GetInputActionType (key, KeyBindingContext.EditDatabaseApp))
          {
            case ActionType.Exit:
              endLoop = true;
              continue;

            case ActionType.IncrementScore:
              ChangeScore(1);
              //list.FocusedItem.Score++;

              //navigationEntries.Sort (_navigationEntryComparer);
              //list.SetFocusedIndex (navigationEntries.IndexOf (list.FocusedItem));
              //frame.RenderPartial (list);
              break;

            case ActionType.DecrementScore:
              ChangeScore(-1);
              //list.FocusedItem.Score--;
              //if (list.FocusedItem.Score < 0)
              //  list.FocusedItem.Score = 0;

              //navigationEntries.Sort (_navigationEntryComparer);
              //list.SetFocusedIndex (navigationEntries.IndexOf (list.FocusedItem));
              //frame.RenderPartial (list);
              break;

            case ActionType.DeleteDatabaseEntry:
              if (_listControl.FocusedItem == null)
                continue;

              var index = _listControl.FocusedItemIndex;
              _listControl.Items!.Remove(_listControl.FocusedItem);
              _unfilteredList.Remove(_listControl.FocusedItem);
              _listControl.SetFocusedIndex (index);
              UpdateBorder();
              frame.RenderPartial(_border);
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

    private void UpdateBorder()
    {
      _border.Text = $"❮ {_listControl.Items!.Count} / {_unfilteredList.Count} ❯";
      _border.RootControl.AssociatedFrame.RenderPartial(_border);
    }

    private void SearchTextBox_OnTextChanged(object sender, string text)
    {
      (bool filterChanged, Func<NavigationEntry, SearchMatch[]>? filter, bool canApplyFilterToFilteredList) = _searchFilterProvider.GetFilter(text, false);

      if (!filterChanged)
        return;

      if (_searchFilterProvider.IsFilterActive)
        _listControl.SetItemList(NavigationDatabase.Instance.Entries.Where(e => filter(e).Length > 0).Order(_navigationEntryComparer).ToList());
      else
        _listControl.SetItemList(NavigationDatabase.Instance.Entries.Order(_navigationEntryComparer).ToList());
      _listControl.SetFocusedIndex(0);

      UpdateBorder();
    }

    private void ChangeScore(int value)
    {
      var focusedItem = _listControl.FocusedItem;
      if (focusedItem == null)
        return;

      focusedItem.Score += value;
      if (focusedItem.Score < 0)
        focusedItem.Score = 0;

      if (focusedItem.Score > Settings.Instance.MaxIndividualScore)
        focusedItem.Score = Settings.Instance.MaxIndividualScore;

      var source = _listControl.Items as List<NavigationEntry>;
      
      if (source == null)
        return;
      
      source.Sort(_navigationEntryComparer);
      _listControl.SetFocusedIndex(source.IndexOf(focusedItem));
    }

    private void CreateUi (Frame frame)
    {
      var rootPanel = new Panel(1.AsFraction(), 1.AsFraction());
      frame.AddControls(rootPanel);

      var titleLabel = new Label(1.AsFraction(), 1.AsFixed());

      _border = new Border(1.AsFraction(), 1.AsFraction());
      _border.BorderColor = ThemeManger.Instance.BorderColor;
      _border.TextPosition = BorderTextPosition.BottomLeft;
      titleLabel.Text = "Edit bonsai database";

      Panel topPanel = new(1.AsFraction(), 1.AsFixed());
      topPanel.BackgroundColor = ThemeManger.Instance.TopBarBackgroundColor;
      topPanel.TextColor = ThemeManger.Instance.TopBarTextColor;
      topPanel.AddControls(titleLabel);

      var increment = Settings.Instance.GetInstructionForAction(KeyBindingContext.EditDatabaseApp, ActionType.IncrementScore, "increment");
      var decrement = Settings.Instance.GetInstructionForAction(KeyBindingContext.EditDatabaseApp, ActionType.DecrementScore, "decrement");
      var delete = Settings.Instance.GetInstructionForAction(KeyBindingContext.EditDatabaseApp, ActionType.DeleteDatabaseEntry, "delete");
      var save = Settings.Instance.GetInstructionForAction(KeyBindingContext.EditDatabaseApp, ActionType.SaveDatabaseChanges, "save and exit");
    
      var instructionsLabel = new Label (1.AsFraction(), 1.AsFixed());
      instructionsLabel.Text = $"{increment}   {decrement}   {delete}   {save}";
      instructionsLabel.TextColor = Color.CadetBlue;

      _listControl = new ScrollableList<NavigationEntry>(ThemeManger.Instance.SelectionForegroundColor, ThemeManger.Instance.SelectionBackgroundColor, 1.AsFraction(), 1.AsFraction());
      rootPanel.BackgroundColor = ThemeManger.Instance.BackgroundColor;

      Label title = new Label (1.AsFraction(), 1.AsFixed());
      title.Text = "LAST USED         SCORE PATH";
      title.TextColor = Color.DodgerBlue;
      //"yyyy-MM-dd HH:mm"
      _border.AddControls(title, _listControl);

      var searchPanel = new Panel(1.AsFraction(), 1.AsFixed());
      searchPanel.Flow = ChildControlFlow.Horizontal;
      Label searchLabel = new(9.AsFixed(), 1.AsFixed());
      searchLabel.Text = "Search ❯";
      searchLabel.TextColor = ThemeManger.Instance.SearchLabelTextColor;
      searchLabel.BackgroundColor = ThemeManger.Instance.SearchLabelBackgroundColor;

      _searchTextBox = new TextBox(1.AsFraction(), 1.AsFixed());
      searchPanel.AddControls(searchLabel, _searchTextBox);
      rootPanel.AddControls(topPanel,searchPanel, _border, instructionsLabel);

      frame.EnableBufferSizeChangeWatching();

      

      frame.SetFocus(_searchTextBox);
    }

    
  }

  public class NavigationEntryDescendingSortComparer : IComparer<NavigationEntry>
  {

    public int Compare (NavigationEntry? x, NavigationEntry? y)
    {
      if (x == null || y == null)
        return 0;

      if (x.Score == y.Score)
      {
        if (x.LastUsed == y.LastUsed)
          return string.Compare(x.Path, y.Path, StringComparison.OrdinalIgnoreCase);

        return x.LastUsed < y.LastUsed ? 1 : -1;
      }

      if (x.Score < y.Score)
        return 1;

      return -1;
    }
  }
}