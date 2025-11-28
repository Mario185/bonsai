using System.Collections.Generic;
using System.Text;

namespace clui.New
{
  public abstract class Control
  {
    private Window? _window ;

    public Window? Window
    {
      get => _window ?? Parent?.Window;
      internal set => _window = value;
    }

    public ContainerControl? Parent { get; internal set; }
    public bool Visible { get; set; }

    public int? CalculatedWidth { get; internal set; }
    public int? CalculatedHeight { get; internal set; }

    protected Control()
    {
    }

    protected virtual bool ShouldRenderControl()
    {
      if (!Visible || CalculatedHeight == null || CalculatedWidth == null || CalculatedHeight <= 0 || CalculatedWidth <= 0)
        return false;

      return Parent?.ShouldRenderControl() == true;
    }

  }

  public abstract class ContainerControl : Control
  {
    public ControlCollection Controls { get; }

    protected ContainerControl()
    {
      Controls = new ControlCollection(this);
    }
  }

  public class Panel : ContainerControl
  {
    public Panel() 
    {
    }
  }

  public class Border : ContainerControl
  {
    public Border()
    {
    }
  }


  public class Label : Control
  {
    public Label()
    {
    }
  }
  public class TextBox : Control
  {
    public TextBox()
    {
    }
  }
}