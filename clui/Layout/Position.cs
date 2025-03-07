using System;

namespace clui.Layout
{
  public record Position
  {
    public static Position operator + (Position position, Position b)
    {
      return new Position (position.X + b.X, position.Y + b.Y);
    }

    public static Position operator - (Position a, Position b)
    {
      return new Position (a.X - b.X, a.Y - b.Y);
    }

    public static Position operator + (Position position, Padding padding)
    {
      return new Position (position.X + padding.Left, position.Y + padding.Top);
    }

    public Position (int x, int y)
    {
      X = x;
      Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }
  }
}