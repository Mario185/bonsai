using System;
using System.Collections.Generic;
using System.Linq;
using bonsai.CommandHandling;
using bonsai.Theme;
using clui;
using clui.Controls;
using clui.Extensions;
using consoleTools;

namespace bonsai.Apps
{
  internal class CommandSelectionApp : AppBase, IBonsaiContext
  {
    private readonly IReadOnlyList<Command> _commands;
    private readonly string _path;

    public CommandSelectionApp (IReadOnlyList<Command> commands, string path)
    {
      _commands = commands;
      _path = path;
    }

    protected override IBonsaiContext Context => this;

    public bool IsFilteringActive => false;

    protected override string? RunInternal ()
    {
      using (Frame frame = new())
      {
        Panel rootPanel = new(1.AsFraction(), 1.AsFraction());
        Border border = new(1.AsFraction(), 1.AsFraction());
        ScrollableList<Command> list = new(
            ThemeManger.Instance.SelectionForegroundColor,
            ThemeManger.Instance.SelectionBackgroundColor,
            1.AsFraction(),
            1.AsFraction());
        Label hintLabel = new(1.AsFraction(), 1.AsFixed());

        frame.AddControls (rootPanel);

        rootPanel.BackgroundColor = ThemeManger.Instance.BackgroundColor;
        hintLabel.Text = $"Multiple commands available for {_path}";
        border.BorderColor = ThemeManger.Instance.BorderColor;

        rootPanel.AddControls (hintLabel);
        border.AddControls (list);
        rootPanel.AddControls (border);

        list.SetItemList (_commands.ToList());
        list.SetFocusedIndex (0);

        frame.RenderComplete();
        frame.EnableBufferSizeChangeWatching();

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
              return list.FocusedItem?.GetExecutableAction();

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