namespace clui.Layout
{
  public record FixedSize : LayoutSize
  {
    public static int operator -(FixedSize size, int value)
    {
      return size.Value - value;
    }

    public static int operator +(FixedSize size, int value)
    {
      return size.Value + value;
    }

    public static bool operator <=(FixedSize size, int value)
    {
      return size.Value <= value;
    }

    public static bool operator >=(FixedSize size, int value)
    {
      return size.Value >= value;
    }

    public static bool operator <(FixedSize size, int value)
    {
      return size.Value < value;
    }

    public static bool operator >(FixedSize size, int value)
    {
      return size.Value > value;
    }

    public static int operator +(int value, FixedSize size)
    {
      return size.Value + value;
    }

    public static implicit operator int(FixedSize size)
    {
      return size.Value;
    }

    public FixedSize(int value) : base(value)
    {
    }
  }
}