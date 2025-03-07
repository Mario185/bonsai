using System;
using System.Collections.Generic;
using System.Drawing;
using clui.Layout;
using consoleTools;

namespace clui.Controls
{
  public enum ChildControlFlow
  {
    Vertical = 0,
    Horizontal = 1
  }

  /// <summary>
  ///   Base for all controls.
  /// </summary>
  public abstract class ControlBase
  {
    private readonly List<ControlBase> _controls = [];

    //protected ControlBase (
    //    int desiredWidth = 1,
    //    int desiredHeight = 1,
    //    SizeType widthType = SizeType.Fraction,
    //    SizeType heightType = SizeType.Fraction,
    //    int? minWidth = null,
    //    int? minHeight = null,
    //    int? maxWidth = null,
    //    int? maxHeight = null)
    //    : this (
    //        new LayoutDimension (
    //            new LayoutSize (desiredWidth, widthType),
    //            new LayoutSize (desiredHeight, heightType),
    //            minWidth,
    //            minHeight,
    //            maxWidth,
    //            maxHeight))
    //{
    //}

    protected ControlBase(LayoutSize width, LayoutSize height)
    {
      Width = width;
      Height = height;

      Padding = new Padding(0, 0, 0, 0);
      Position = new Position(0, 0);
    }

    public IReadOnlyList<ControlBase> Controls => _controls;


    public RootControl RootControl
    {
      get
      {
        if (Parent == null)
          return (RootControl)this;

        return Parent.RootControl;
      }
    }
    
    public LayoutSize Width { get; set; }
    public LayoutSize Height { get; set; }


    public int? CalculatedWidth { get; internal set; }
    public int? CalculatedHeight { get; internal set; }

    /// <summary>
    ///   The effective calculated size of the control.
    /// </summary>
    //public FixedDimension? EffectiveDimension { get; internal set; }
    public Padding Padding { get; set; }
    public ControlBase? Parent { get; internal set; }
    public Position Position { get; internal set; }
    public Color? TextColor { get; set; }
    public Color? BackgroundColor { get; set; }

    public Color? FocusedTextColor { get; set; }
    public Color? FocusedBackgroundColor { get; set; }

    public bool Visible { get; set; } = true;
    public ChildControlFlow Flow { get; set; } = ChildControlFlow.Vertical;

    public virtual void OnLayoutCalculated()
    {
    }

    public Color? GetEffectiveBackgroundColor ()
    {
      //if (Renderer.FocusedControl == this)
      //  return FocusedBackgroundColor ?? Parent?.GetEffectiveBackgroundColor();
      
      return BackgroundColor ?? Parent?.GetEffectiveBackgroundColor();
    }

    public Color? GetEffectiveTextColor ()
    {
      //if (Renderer.FocusedControl == this)
      //  return FocusedTextColor ?? Parent?.GetEffectiveTextColor();
      
      return TextColor ?? Parent?.TextColor;
    }

    internal abstract void Render (ConsoleWriter consoleWriter);

    internal virtual bool ShouldRenderControl ()
    {
      if (!Visible || CalculatedWidth == null || CalculatedHeight == null || CalculatedWidth <= 0 || CalculatedHeight <= 0)
        return false;

      if (Parent != null && !Parent.ShouldRenderControl())
        return false;

      return true;
    }

    public void AddControls (params ControlBase[] controls)
    {
      ArgumentNullException.ThrowIfNull (controls);

      foreach (ControlBase? control in controls)
      {
        if (control.Parent != null)
          throw new InvalidOperationException("Controls already has parent and cannot be added to another hierarchy");
        control.Parent = this;
        _controls.Add (control);
      }
    }
  }
}