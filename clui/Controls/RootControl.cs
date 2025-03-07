using clui.Layout;
using consoleTools;

namespace clui.Controls
{
  public class RootControl : ControlBase
  {
    public RootControl(LayoutSize width, LayoutSize height, Frame associatedFrame) : base(width, height)
    {
      AssociatedFrame = associatedFrame;
      Padding = new Padding(0, 0, 0, 0);
    }

    public Frame AssociatedFrame { get; }

    internal override void Render(ConsoleWriter consoleWriter)
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