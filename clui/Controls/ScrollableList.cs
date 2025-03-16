using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using clui.Layout;
using consoleTools;

namespace clui.Controls
{
  public class ScrollableList<T> : ControlBase
      where T : class, IListItem
  {
    public event SelectionChangedHandler? OnSelectionChanged;

    public delegate void SelectionChangedHandler (object sender, T selectedItem);

    private readonly Color? _backgroundColorFocused;
    private readonly Color? _foregroundColorFocused;

    public ScrollableList (Color? foregroundColorFocused, Color? backgroundColorFocused, LayoutSize width, LayoutSize height)
        : base (width, height)
    {
      _foregroundColorFocused = foregroundColorFocused;
      _backgroundColorFocused = backgroundColorFocused;

      CurrentVisibleFromIndex = 0;
    }

    public T? FocusedItem { get; private set; }

    public int FocusedItemIndex { get; private set; }

    public int CurrentVisibleFromIndex { get; private set; }

    public IList<T>? Items { get; private set; }

    public void SetItemList (IList<T> listItems)
    {
      ArgumentNullException.ThrowIfNull (listItems);

      if (listItems is INotifyCollectionChanged observableCollection)
      {
        observableCollection.CollectionChanged += ListItemCollectionChanged;
      }

      Items = listItems;

      FocusedItem = null;
      FocusedItemIndex = -1;
    }

    private void ListItemCollectionChanged (object? sender, NotifyCollectionChangedEventArgs e)
    {
      // only when layout is calculated we want to handle the event
      if (CalculatedWidth == null || CalculatedHeight == null)
      {
        return;
      }

      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          if (Items!.Count == 1)
          {
            SetFocusedIndex (0);
          }
          else if (e.NewStartingIndex < CurrentVisibleFromIndex + CalculatedHeight)
          {
            UpdateCurrentVisibleIndex();
            RootControl.AssociatedFrame.RenderPartial (this);
          }

          break;
        case NotifyCollectionChangedAction.Remove:
          if (e.OldItems != null && FocusedItem != null && e.OldItems.Contains (FocusedItem))
          {
            int targetIndex = FocusedItemIndex - 1;
            if (targetIndex < 0)
            {
              targetIndex = 0;
            }

            if (targetIndex < Items!.Count)
            {
              SetFocusedIndex (targetIndex);
            }
            else
            {
              ResetFocus();
            }
          }

          break;
        case NotifyCollectionChangedAction.Replace:
          break;
        case NotifyCollectionChangedAction.Move:
          if (e.OldItems != null && FocusedItem != null && e.OldItems.Contains (FocusedItem))
          {
            SetFocusedItem (FocusedItem);
          }

          break;
        case NotifyCollectionChangedAction.Reset:
          ResetFocus();
          break;
        default:
          throw new ArgumentOutOfRangeException ($"{e.Action} currently not supported.");
      }
    }

    private void ResetFocus ()
    {
      FocusedItemIndex = 0;
      FocusedItem = default;
      UpdateCurrentVisibleIndex();
      RootControl.AssociatedFrame.RenderPartial (this);
    }

    public override void OnLayoutCalculated ()
    {
      base.OnLayoutCalculated();

      // in case layout has been recalculated ensure that the current focused index is visible.
      //SetFocusedIndex (FocusedItemIndex);
    }

    internal override bool ShouldRenderControl ()
    {
      if (Items == null)
      {
        return false;
      }

      return base.ShouldRenderControl();
    }

    internal override void Render (ConsoleWriter consoleWriter)
    {
      consoleWriter.Cursor.MoveTo (Position.X, Position.Y);
      Color? textColor = GetEffectiveTextColor();
      Color? backGroundColor = GetEffectiveBackgroundColor();
      for (int i = 0; i < CalculatedHeight!; i++)
      {
        int effectiveIndex = i + CurrentVisibleFromIndex;
        consoleWriter.Style.BackgroundColor (backGroundColor);
        if (effectiveIndex < Items!.Count)
        {
          T item = Items[effectiveIndex];
          RenderListItem (item, textColor, backGroundColor, consoleWriter);
        }
        else
        {
          consoleWriter.Text.EraseCharacter (CalculatedWidth!.Value);
        }

        if (i < CalculatedHeight - 1)
        {
          consoleWriter.Cursor
              .MoveDown()
              .MoveAbsoluteHorizontally (Position.X);
        }
      }

      consoleWriter.Style.ResetStyles();
    }

    private void RenderListItem (T item, Color? defaultTextColor, Color? defaultBackgroundColor, ConsoleWriter consoleWriter)
    {
      consoleWriter.Style.ForegroundColor (defaultTextColor)
          .BackgroundColor (defaultBackgroundColor);

      bool isFocusedItem = item.Equals (FocusedItem);
      if (isFocusedItem)
      {
        consoleWriter.Style.BackgroundColor (_backgroundColorFocused)
            .ForegroundColor (_foregroundColorFocused);
      }

      consoleWriter.Text.EraseCharacter (CalculatedWidth!.Value);
      item.Write (consoleWriter, CalculatedWidth!.Value, isFocusedItem);

      consoleWriter.Style.ResetStyles();
    }

    public void TrySetCurrentVisibleFromIndex (int value)
    {
      if (Items == null || CalculatedWidth == null || CalculatedHeight == null)
      {
        return;
      }

      CurrentVisibleFromIndex = value;
    }

    public void SetFocusedItem (T item)
    {
      SetFocusedItemInternal (item);
      RootControl.AssociatedFrame.RenderPartial (this);
    }

    public void MoveFocusedIndex (int step, bool wrapAround)
    {
      int index = FocusedItemIndex + step;

      if (wrapAround && Items != null)
      {
        if (index < 0)
        {
          index = Items.Count - 1;
        }

        if (index >= Items.Count)
        {
          index = 0;
        }
      }

      SetFocusedIndex (index);
    }

    public void SetFocusedIndex (int index)
    {
      SetFocusedIndexInternal (index);
      RootControl.AssociatedFrame.RenderPartial (this);
    }

    private void SetFocusedItemInternal (T item)
    {
      ArgumentNullException.ThrowIfNull (item);

      int index = Items!.IndexOf (item);
      if (index < 0)
      {
        throw new ArgumentException ("The given item is not present in the list.");
      }

      SetFocusedIndexInternal (index);
    }

    private void SetFocusedIndexInternal (int index)
    {
      if (Items == null)
      {
        return;
      }

      int clampedIndex = Math.Clamp (index, 0, Math.Max (0, Items.Count - 1));
      if (clampedIndex > Items.Count - 1)
      {
        return;
      }

      FocusedItemIndex = clampedIndex;
      FocusedItem = Items[clampedIndex];

      OnSelectionChanged?.Invoke (this, FocusedItem);
      UpdateCurrentVisibleIndex();
    }

    private void UpdateCurrentVisibleIndex ()
    {
      if (CalculatedWidth == null || CalculatedHeight == null)
      {
        return;
      }

      int currentTo = Math.Min (Items!.Count, CurrentVisibleFromIndex + CalculatedHeight!.Value);
      int newVisibleFrom = CurrentVisibleFromIndex;

      // move up
      if (FocusedItemIndex < CurrentVisibleFromIndex)
      {
        newVisibleFrom = FocusedItemIndex;
      }

      // move down
      if (FocusedItemIndex >= currentTo)
      {
        newVisibleFrom = FocusedItemIndex - CalculatedHeight!.Value + 1;
      }

      if (newVisibleFrom > Items.Count - CalculatedHeight)
      {
        newVisibleFrom = Items.Count - CalculatedHeight!.Value;
      }

      CurrentVisibleFromIndex = Math.Max (0, newVisibleFrom);
    }

    public void SelectPreviousItem ()
    {
      MoveFocusedIndex (-1, true);
    }

    public void SelectNextItem ()
    {
      MoveFocusedIndex (1, true);
    }

    public void SelectPageUp ()
    {
      MoveFocusedIndex (-(CalculatedHeight!.Value - 1), false);
    }

    public void SelectPageDown ()
    {
      MoveFocusedIndex (CalculatedHeight!.Value - 1, false);
    }

    public void SelectFirstItem ()
    {
      MoveFocusedIndex (-FocusedItemIndex, false);
    }

    public void SelectLastItem ()
    {
      MoveFocusedIndex (Items!.Count - FocusedItemIndex, false);
    }
  }

  public interface IListItem
  {
    void Write (ConsoleWriter writer, int maxLength, bool isFocusedItem);
  }
}