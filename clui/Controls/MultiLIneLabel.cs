using System;
using System.Linq;
using clui.Layout;
using consoleTools;

namespace clui.Controls
{
  public class MultiLIneLabel : ControlBase
  {
    public MultiLIneLabel (LayoutSize width, LayoutSize height)
        : base (width, height)
    {
    }

    public bool StickToBottom { get; set; }

    public bool TruncateLeft { get; set; }

    public FormattedLine?[]? Lines { get; set; }

    internal override void Render (ConsoleWriter consoleWriter)
    {
      if (!ShouldRenderControl() || Lines == null)
      {
        return;
      }

      var filledLines = Lines.Where(l => l != null && !string.IsNullOrWhiteSpace(l.Value)).ToArray();

      int lineToStart = 0;
      if (StickToBottom)
      {
        lineToStart = (CalculatedHeight!.Value) - filledLines.Length;
      }

      consoleWriter.Cursor.MoveTo(Position.X, lineToStart);

      for (int i = 0; i < CalculatedHeight!.Value; i++)
      {
        consoleWriter.Style.ResetStyles().Cursor.MoveTo(Position.X, Position.Y + i);

        consoleWriter.Style.ForegroundColor (GetEffectiveTextColor())
            .BackgroundColor (GetEffectiveBackgroundColor())
            .Text.EraseCharacter (CalculatedWidth!.Value!);

        if (i >= lineToStart && i - lineToStart < filledLines.Length)
        {
          FormattedLine? formattedLine = filledLines[i - lineToStart];
          if (formattedLine?.Value != null)
          {
            string lineText = formattedLine.Value;

            consoleWriter.Style.ForegroundColor(formattedLine.TextColor).BackgroundColor(formattedLine.BackgroundColor);

            if (formattedLine.Bold)
            {
              consoleWriter.Style.Bold();
            }

            if (formattedLine.Underline)
            {
              consoleWriter.Style.Underline();
            }

            consoleWriter.Cursor.MoveAbsoluteHorizontally(Position.X + formattedLine.Indent);

            if (lineText.Length + formattedLine.Indent > CalculatedWidth!)
            {
              if (TruncateLeft)
              {
                consoleWriter.Write('…').WriteTruncated(
                    lineText,
                    (lineText.Length + formattedLine.Indent) - CalculatedWidth!.Value + 1,
                    CalculatedWidth!.Value - 1);
              }
              else
              {
                consoleWriter.WriteTruncated(lineText, 0, CalculatedWidth!.Value - 1 - formattedLine.Indent).Write('…');
              }
            }
            else
            {
              consoleWriter.Write(lineText);
            }
          }
        }

        
      }

      consoleWriter.Style.ResetStyles();
    }
  }
}