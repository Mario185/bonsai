using clui.Layout;

namespace clui.Extensions
{
  public static class IntExtensions
  {
    public static FractionSize AsFraction(this int value) => new(value);
    public static FixedSize AsFixed(this int value) => new(value);
  }
}