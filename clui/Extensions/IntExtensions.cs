using System;
using clui.Layout;

namespace clui.Extensions
{
  public static class IntExtensions
  {
    public static FractionSize AsFraction (this int value)
    {
      return new FractionSize (value);
    }

    public static FixedSize AsFixed (this int value)
    {
      return new FixedSize (value);
    }
  }
}