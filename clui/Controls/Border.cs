using System;
using System.Drawing;
using System.Linq;
using clui.Layout;
using consoleTools;

namespace clui.Controls
{
  public enum BorderTextPosition
  {
    TopLeft,
    TopMiddle,
    TopRight,
    BottomLeft,
    BottomMiddle,
    BottomRight
  }

  public class Border : ControlBase
  {
    private readonly char? _bottomFiller;
    private readonly char? _bottomLeftCorner;
    private readonly char? _bottomRightCorner;
    private readonly char? _leftFiller;
    private readonly char? _rightFiller;
    private readonly char? _topFiller;
    private readonly char? _topLeftCorner;
    private readonly char? _topRightCorner;
    private char[] _bottom;

    private char[] _top;
    private int _effectiveWidth;

    public Border(LayoutSize width, LayoutSize height,
      char? topLeftCorner = '┌',
      char? topFiller = '─',
      char? topRightCorner = '┐',
      char? leftFiller = '│',
      char? rightFiller = '│',
      char? bottomLeftCorner = '└',
      char? bottomFiller = '─',
      char? bottomRightCorner = '┘'
    )
    : base(width, height)
    {
      _topLeftCorner = topLeftCorner;
      _topFiller = topFiller;
      _topRightCorner = topRightCorner;
      _leftFiller = leftFiller;
      _rightFiller = rightFiller;
      _bottomLeftCorner = bottomLeftCorner;
      _bottomFiller = bottomFiller;
      _bottomRightCorner = bottomRightCorner;

      Padding = new Padding(topFiller == null && topRightCorner == null && topLeftCorner == null ? 0 : 1,
        topLeftCorner == null && leftFiller == null && bottomLeftCorner == null ? 0 : 1,
        bottomFiller == null && bottomRightCorner == null && bottomLeftCorner == null ? 0 : 1,
        topRightCorner == null && rightFiller == null && bottomRightCorner == null ? 0 : 1);

      _top = _bottom = [];
    }
   
    public Color? BorderColor { get; set; }
    public Color? TextBackgroundColor { get; set; }

    public string Text { get; set; } = string.Empty;

    public BorderTextPosition TextPosition { get; set; } = BorderTextPosition.TopLeft;

    private void BuildBorder ()
    {
      _top = _topFiller == null ? [] : Enumerable.Repeat (_topFiller.Value, _effectiveWidth).ToArray();
      _bottom = _bottomFiller == null ? [] : Enumerable.Repeat (_bottomFiller.Value, _effectiveWidth).ToArray();
    }

    public override void OnLayoutCalculated ()
    {
      base.OnLayoutCalculated();
      _effectiveWidth = Math.Max (CalculatedWidth!.Value - 2, 0);
      BuildBorder();
    }

    internal override void Render (ConsoleWriter consoleWriter)
    {
      if (!ShouldRenderControl())
        return;

      consoleWriter.Cursor.MoveTo (Position.X, Position.Y);

      consoleWriter.Style
          .ForegroundColor (BorderColor)
          .BackgroundColor (GetEffectiveBackgroundColor());

        if (_topLeftCorner != null)
          consoleWriter.Write (_topLeftCorner.Value);
        
        if (_top.Length > 0)
          consoleWriter.Write (_top);

        if (_topRightCorner != null)
          consoleWriter.Write(_topRightCorner.Value);

      for (int i = Padding.Top; i < CalculatedHeight - Padding.Bottom; i++)
      {
        if (_leftFiller != null)
          consoleWriter.Cursor.MoveTo(Position.X, Position.Y + i).Writer
            .Write(_leftFiller.Value);

        if (_rightFiller != null)
          consoleWriter.Cursor.MoveTo(Position.X + CalculatedWidth!.Value - 1, Position.Y + i)
            .Writer.Write(_rightFiller.Value);
      }

      consoleWriter
          .Cursor.MoveTo (Position.X, Position.Y + CalculatedHeight!.Value - 1);

      if (_bottomLeftCorner != null)
        consoleWriter.Write(_bottomLeftCorner.Value);
      if (_bottom.Length > 0)
        consoleWriter.Write(_bottom);
      if (_bottomRightCorner != null)
        consoleWriter.Write(_bottomRightCorner.Value);
      consoleWriter.Style.ResetStyles();

      if (Text != string.Empty && _effectiveWidth > 0)
      {
        consoleWriter.Style.ForegroundColor (GetEffectiveTextColor()).BackgroundColor (TextBackgroundColor ?? GetEffectiveBackgroundColor());
        int possibleUsedSpace = Math.Min (Text.Length, _effectiveWidth);
        int widthValue = _effectiveWidth - possibleUsedSpace;
        int leftWidth = (int)((widthValue / 2.0) + 0.5);
        switch (TextPosition)
        {
          case BorderTextPosition.TopLeft:
            consoleWriter.Cursor.MoveTo (Position.X + 1, Position.Y);
            break;
          case BorderTextPosition.TopMiddle:
            consoleWriter.Cursor.MoveTo (Position.X + 1 + leftWidth, Position.Y);
            break;
          case BorderTextPosition.TopRight:
            consoleWriter.Cursor.MoveTo (Position.X + 1 + widthValue, Position.Y);
            break;
          case BorderTextPosition.BottomLeft:
            consoleWriter.Cursor.MoveTo (Position.X + 1, Position.Y + CalculatedHeight!.Value - 1);
            break;
          case BorderTextPosition.BottomMiddle:
            consoleWriter.Cursor.MoveTo (Position.X + 1 + leftWidth, Position.Y + CalculatedHeight!.Value - 1);
            break;
          case BorderTextPosition.BottomRight:
            consoleWriter.Cursor.MoveTo (Position.X + 1 + widthValue, Position.Y + CalculatedHeight!.Value - 1);
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }

        consoleWriter.WriteTruncated (Text, 0, possibleUsedSpace);
      }

      consoleWriter.Style.ResetStyles().Flush();
    }
  }
}