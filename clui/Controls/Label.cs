using System;
using clui.Layout;
using consoleTools;

namespace clui.Controls
{
  public class Label : ControlBase
  {
    private string? _text;

    public Label(LayoutSize width, LayoutSize height)
    : base(width,height)
    {
      
    }

    public string? Text
    {
      get => _text;
      set
      {
        if (_text == value)
          return;

        _text = value;
        if (CalculatedHeight != null && CalculatedWidth != null)
          RootControl.AssociatedFrame.RenderPartial(this);
      }
    }

    public bool TruncateLeft { get; set; }


    internal override void Render (ConsoleWriter consoleWriter)
    {
      if (!ShouldRenderControl())
        return;

      consoleWriter.Style.ForegroundColor (GetEffectiveTextColor())
          .BackgroundColor (GetEffectiveBackgroundColor())
          .Cursor.MoveTo (Position.X, Position.Y)
          .Text.EraseCharacter (CalculatedWidth!.Value);

      if (Text != null)
      {
        if (Text.Length > CalculatedWidth!)
        {
          if (TruncateLeft)
          {
            consoleWriter.Write ('…').WriteTruncated (
                Text,
                Text.Length - CalculatedWidth!.Value + 1,
                CalculatedWidth!.Value - 1);
          }
          else
            consoleWriter.WriteTruncated (Text, 0, CalculatedWidth!.Value - 1).Write ('…');
        }
        else
          consoleWriter.Write (Text);
      }

      consoleWriter.Style.ResetStyles();
    }
  }
}