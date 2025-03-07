using System;
using clui.Layout;
using consoleTools;

namespace clui.Controls
{
  public class Panel : ControlBase
  {
    public Panel(LayoutSize width, LayoutSize height)
      : base(width, height)
    {
      Flow = ChildControlFlow.Vertical;
      Padding = new Padding(0, 0, 0, 0);
    }

    internal override void Render (ConsoleWriter consoleWriter)
    {
      if (BackgroundColor == null)
        return;

      consoleWriter.Style.BackgroundColor(GetEffectiveBackgroundColor());

      for (int i = 0; i < CalculatedHeight!; i++)
        consoleWriter.Cursor.MoveTo(Position.X, Position.Y + i).Text.EraseCharacter(CalculatedWidth!.Value!);

      consoleWriter.Style.ResetStyles().Flush();
    }
  }
}