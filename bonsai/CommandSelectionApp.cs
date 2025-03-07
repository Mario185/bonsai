using System;
using System.Collections.Generic;
using System.Linq;
using bonsai.CommandHandling;
using bonsai.Theme;
using clui;
using clui.Controls;
using clui.Extensions;
using consoleTools;

namespace bonsai
{
  public class CommandSelectionApp
  {
    public Command? Run(IReadOnlyList<Command> commands, string path)
    {
      using (var frame = new Frame())
      {
        Panel p = new Panel(1.AsFraction(), 1.AsFraction());
        frame.AddControls(p);
        p.BackgroundColor = ThemeManger.Instance.BackgroundColor;
        Label l = new Label(1.AsFraction(), 1.AsFixed());
        l.Text = $"Multiple commands available for {path}";
        p.AddControls(l);

        Border b = new Border(1.AsFraction(), 1.AsFraction());
        b.BorderColor = ThemeManger.Instance.BorderColor;

        ScrollableList<Command> list = new ScrollableList<Command>(ThemeManger.Instance.SelectionForegroundColor, ThemeManger.Instance.SelectionBackgroundColor,
          1.AsFraction(), 1.AsFraction());
        b.AddControls(list);
        p.AddControls(b);

        list.SetItemList(commands.ToList());
        list.SetFocusedIndex(0);

        frame.RenderComplete();
        frame.EnableBufferSizeChangeWatching();

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
              return list.FocusedItem;

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
  }
}