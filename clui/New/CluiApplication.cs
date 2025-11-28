using System;
using System.Collections.Generic;
using consoleTools;

namespace clui.New
{
  public class CluiApplication : IDisposable
  {
    private readonly AlternateScreenBufferSection? _alternateScreenBufferSection;
    private BufferSizeChangeCallback? _bufferSizeChangeCallback;
    private readonly List<Window> _windows = new();

    public IReadOnlyList<Window> Windows => _windows;

    public CluiApplication(bool useAlternateScreenBuffer = true)
    {
      if (useAlternateScreenBuffer)
        _alternateScreenBufferSection = new AlternateScreenBufferSection();
    }

    /// <summary>
    /// Shows the given <paramref name="window"/> or brings it to front
    /// </summary>
    /// <param name="window"></param>
    public void ShowWindow(Window window)
    {
      ArgumentNullException.ThrowIfNull(window);

      if (_windows.Contains(window))
      {
        _windows.Remove(window);
        _windows.Add(window);
      }
      else
      {
        window.Application = this;
        _windows.Add(window);
      }
    }

    public void EnableBufferSizeChangeWatching()
    {
      if (_bufferSizeChangeCallback != null)
      {
        return;
      }

      //_bufferSizeChangeCallback = (_, _) => RenderComplete();
      _bufferSizeChangeCallback = (_, _) => {};
      ConsoleHandler.RegisterBufferSizeChangeCallback(_bufferSizeChangeCallback);
    }

    public void DisableBufferSizeChangeWatching()
    {
      if (_bufferSizeChangeCallback == null)
      {
        return;
      }

      ConsoleHandler.UnregisterBufferSizeChangeCallback(_bufferSizeChangeCallback);
      _bufferSizeChangeCallback = null;
    }

    public void Dispose()
    {
      GC.SuppressFinalize(this);
      DisableBufferSizeChangeWatching();
      //Renderer.Dispose();
      _alternateScreenBufferSection?.Dispose();
    }
  }
}
