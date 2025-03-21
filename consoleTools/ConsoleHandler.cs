﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using consoleTools.Windows;

namespace consoleTools
{
  public static class ConsoleHandler
  {
    private static readonly CancellationTokenSource s_cancellationTokenSource;
    private static Lock s_bufferSizeCallbackLock = new();

    public static void RegisterBufferSizeChangeCallback(BufferSizeChangeCallback callbackAction)
    {
      ArgumentNullException.ThrowIfNull (callbackAction);
      using(s_bufferSizeCallbackLock.EnterScope())
      {
        s_consoleHandlerImplementation.RegisterBufferSizeChange (callbackAction);
      }
    }
    public static void UnregisterBufferSizeChangeCallback(BufferSizeChangeCallback callbackAction)
    {
      ArgumentNullException.ThrowIfNull (callbackAction);
      using(s_bufferSizeCallbackLock.EnterScope())
      {
        s_consoleHandlerImplementation.UnregisterBufferSizeChange(callbackAction);
      }
    }

    private static readonly IConsoleHandler s_consoleHandlerImplementation;
    static ConsoleHandler()
    {
      if (RuntimeInformation.IsOSPlatform (OSPlatform.Windows))
        s_consoleHandlerImplementation = new WindowsConsoleHandler(null, s_bufferSizeCallbackLock);

      s_cancellationTokenSource = new CancellationTokenSource();

      if (s_consoleHandlerImplementation == null)
        throw new NotSupportedException("OS " + RuntimeInformation.OSDescription + " is currently not supported.");
    }

    public static void StartOperation()
    {
      s_consoleHandlerImplementation.StartOperation (s_cancellationTokenSource.Token);
    }

    public static void CancelOperation()
    {
      s_cancellationTokenSource.Cancel();
    }

    public static ConsoleKeyInfo Read()
    {
      return s_consoleHandlerImplementation.KeyQueue.Take (s_cancellationTokenSource.Token);
    }
  }
}
