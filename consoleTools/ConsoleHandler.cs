using System;
using System.Collections.Generic;
using System.Threading;

namespace consoleTools
{
  public interface IConsole
  {
    int WindowHeight { get; }
    int WindowWidth { get; }
    ConsoleKeyInfo ReadKey(bool intercept);
  }

  public static class ConsoleHandler
  {
    private static readonly Lock s_bufferSizeCallbackLock = new();
    private static CancellationTokenSource? s_cancellationTokenSource;
    private static readonly List<BufferSizeChangeCallback> s_bufferSizeChangeCallbacks = new();
    private static bool s_operationStarted;

    private static IConsole s_consoleImplementation = null!;

    public static void RegisterBufferSizeChangeCallback (BufferSizeChangeCallback callbackAction)
    {
      ArgumentNullException.ThrowIfNull (callbackAction);
      using (s_bufferSizeCallbackLock.EnterScope())
      {
        s_bufferSizeChangeCallbacks.Add(callbackAction);
      }
    }

    public static void UnregisterBufferSizeChangeCallback (BufferSizeChangeCallback callbackAction)
    {
      ArgumentNullException.ThrowIfNull (callbackAction);
      using (s_bufferSizeCallbackLock.EnterScope())
      {
        s_bufferSizeChangeCallbacks.Remove(callbackAction);
      }
    }

    public static void Initialize(IConsole consoleImplementation)
    {
      s_consoleImplementation = consoleImplementation;
    }

    public static void StartOperation ()
    {
      if (s_operationStarted)
        return;

      s_operationStarted = true;
      s_cancellationTokenSource = new CancellationTokenSource();

      ManualResetEventSlim threadStarted = new ManualResetEventSlim ();

      var thread = new Thread(() => MonitorBufferSizeChange(threadStarted, s_cancellationTokenSource.Token));
      thread.Start();
      threadStarted.Wait();
    }

    public static void CancelOperation ()
    {
      s_cancellationTokenSource?.Cancel();
      s_operationStarted = false;
    }

    public static ConsoleKeyInfo Read (bool intercept)
    {
      return s_consoleImplementation.ReadKey(intercept);
    }

    private static void MonitorBufferSizeChange(ManualResetEventSlim threadStarted, CancellationToken token)
    {
      var height = s_consoleImplementation.WindowHeight;
      var width = s_consoleImplementation.WindowWidth;
      threadStarted.Set();
      while (!token.IsCancellationRequested)
      {
        Thread.Sleep(10);
        if (height != s_consoleImplementation.WindowHeight || width != s_consoleImplementation.WindowWidth)
        {
          height = s_consoleImplementation.WindowHeight;
          width = s_consoleImplementation.WindowWidth;
          using (s_bufferSizeCallbackLock.EnterScope())
          {
            foreach (var bufferSizeChangeCallback in s_bufferSizeChangeCallbacks)
            {
              bufferSizeChangeCallback(width, height);
            }
          }
        }
      }
    }
  }
}