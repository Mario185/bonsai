using System;

namespace clui.Layout
{
  public abstract record LayoutSize
  {
    protected LayoutSize (int value)
    {
      Value = value;
    }

    public int Value { get; set; }
  }
}