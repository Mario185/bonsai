using System;
using System.Threading;
using clui.Controls;
using clui.Controls.Interfaces;
using clui.Layout;
using consoleTools;

namespace clui
{
  public sealed class Renderer : IDisposable
  {
    private static readonly Lock s_lock = new();

    internal Renderer ()
    {
    }

    public ICanHaveFocus? FocusedControl { get; private set; }

    public ConsoleWriter ConsoleWriter { get; } = new();

    public void Dispose ()
    {
      ConsoleWriter.Dispose();
    }

    public void Render (ControlBase control)
    {
      using (s_lock.EnterScope())
      {
        // we need to flush the hide first otherwise flashing text occurs
        ConsoleWriter.Cursor.Hide().ResetPosition().Flush();

        IHaveVisibleCursor? controlHasVisibleCursor = FocusedControl as IHaveVisibleCursor;

        RenderInternal (control);

        if (controlHasVisibleCursor != null)
        {
          Position? cursorTarget = controlHasVisibleCursor.GetCursorPosition();
          ConsoleWriter.Cursor.MoveTo (cursorTarget.X, cursorTarget.Y);
        }
        else
        {
          ConsoleWriter.Cursor.ResetPosition();
        }

        if (controlHasVisibleCursor != null)
        {
          ConsoleWriter.Cursor.Show();
        }

        ConsoleWriter.Flush();
      }
    }

    private void RenderInternal (ControlBase control)
    {
      if (!control.ShouldRenderControl())
      {
        return;
      }

      control.Render (ConsoleWriter);

      foreach (ControlBase child in control.Controls)
      {
        RenderInternal (child);
      }
    }

    public void SetFocusedControl (ICanHaveFocus? control)
    {
      FocusedControl = control;
    }
  }
}