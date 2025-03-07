using System;

namespace clui.Layout
{
  public record Padding
  {
    public Padding (int top, int left, int bottom, int right)
    {
      Top = top;
      Left = left;
      Bottom = bottom;
      Right = right;
    }

    public int Top { get; set; }
    public int Left { get; set; }
    public int Bottom { get; set; }
    public int Right { get; set; }
  }
}