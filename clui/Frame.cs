using System;
using clui.Controls;
using clui.Controls.Interfaces;
using clui.Layout;
using consoleTools;

namespace clui
{

  /// <summary>
  /// The root element for every ui
  /// </summary>
  public class Frame : IDisposable
  {
    private readonly LayoutCalculator _layoutCalculator = new();
    private readonly Renderer _renderer = new();

    private readonly RootControl _rootControl;
    private BufferSizeChangeCallback? _bufferSizeChangeCallback;

    public Position Position { get; private set; } = new(1, 1);
    public LayoutSize Width { get; set; } = new FractionSize(1);
    public LayoutSize Height { get; set; } = new FractionSize(1);

    private bool _completeLayoutHasBeenCalculatedAtLeastOnce;

    private readonly AlternateScreenBufferSection? _alternateScreenBufferSection;

    private ICanHaveFocus? _focusedControl;

    public Frame(bool useAlternateScreenBuffer = true)
    {
      _rootControl = new RootControl(Width, Height, this);
      _rootControl.Position = Position;
      if (useAlternateScreenBuffer)
        _alternateScreenBufferSection = new AlternateScreenBufferSection();

    }

    public void EnableBufferSizeChangeWatching()
    {
      if (_bufferSizeChangeCallback != null)
        return;

      _bufferSizeChangeCallback = (_, _) => RenderComplete();
      ConsoleHandler.RegisterBufferSizeChangeCallback(_bufferSizeChangeCallback);
    }

    public void DisableBufferSizeChangeWatching()
    {
      if (_bufferSizeChangeCallback == null)
        return;

      ConsoleHandler.UnregisterBufferSizeChangeCallback(_bufferSizeChangeCallback);
      _bufferSizeChangeCallback = null;
    }

    public void RenderComplete()
    {
      _rootControl.Position = Position;
      _rootControl.Width = Width;
      _rootControl.Height = Height;

      _layoutCalculator.CalculateRootControlLayout(_rootControl);
      _completeLayoutHasBeenCalculatedAtLeastOnce = true;
      _renderer.Render(_rootControl);
    }

    public void RenderPartial(ControlBase controlToRender)
    {
      if (!_completeLayoutHasBeenCalculatedAtLeastOnce)
        return;

      if (_rootControl == null)
        throw new InvalidOperationException("Root control has not been initialized.");

      if (_rootControl != controlToRender.RootControl)
        throw new InvalidOperationException($"{nameof(ControlBase.RootControl)} of {nameof(controlToRender)} does not match the root control of this frame");

      _layoutCalculator.CalculateChildrenLayout(controlToRender);
      _renderer.Render(controlToRender);
    }

    public void SetFocus<T>(T? control)
    where  T : ControlBase, ICanHaveFocus 
    {
      if (_focusedControl != null)
        _focusedControl.OnLostFocus();

      _renderer.SetFocusedControl(control);

      if (control == null)
      {
        _focusedControl = null;
      }
      else
      {
        _focusedControl = control;
        _focusedControl.OnGotFocus();
        RenderPartial(control);
      }
    }

    public void AddControls(params ControlBase[] controls)
    {
      _rootControl.AddControls(controls);
    }
    
    public void Dispose()
    {
      DisableBufferSizeChangeWatching();
      _alternateScreenBufferSection?.Dispose();
    }
  }
}