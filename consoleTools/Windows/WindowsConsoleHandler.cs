﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;

namespace consoleTools.Windows
{
  internal sealed class WindowsConsoleHandler : IConsoleHandler
  {
    private readonly Lock _bufferSizeRegistrationLock;

    private readonly List<BufferSizeChangeCallback> _bufferSizeChangeCallbacks = new();

    private const int STD_INPUT_HANDLE = -10;
    private const int STD_OUTPUT_HANDLE = -11;
    private const int STD_ERROR_HANDLE = -12;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool ReadConsoleInput(IntPtr hConsoleInput, out InputRecord lpBuffer, uint nLength, out uint lpNumberOfEventsRead);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool GetNumberOfConsoleInputEvents(IntPtr hConsoleInput, out uint lpcNumberOfEvents);

    private IntPtr _inputHandle;
    

    public void SetInputHandle(IntPtr? inputHandle)
    {
      _inputHandle = inputHandle ?? GetStdHandle (STD_INPUT_HANDLE);
    }

    public void StartOperation (CancellationToken cancellationToken)
    {
      var  thread = new Thread (() => Read (cancellationToken));
      thread.Start ();
    }

    public BlockingCollection<ConsoleKeyInfo> KeyQueue { get; } = new(new ConcurrentQueue<ConsoleKeyInfo>());
    public void RegisterBufferSizeChange (BufferSizeChangeCallback callback)
    {
      ArgumentNullException.ThrowIfNull (callback);
      _bufferSizeChangeCallbacks.Add (callback);
    }

    public void UnregisterBufferSizeChange(BufferSizeChangeCallback callback)
    {
      ArgumentNullException.ThrowIfNull(callback);
      _bufferSizeChangeCallbacks.Remove (callback);
    }

    public WindowsConsoleHandler (IntPtr? inputHandle, Lock bufferSizeRegistrationLock)
    {
      _bufferSizeRegistrationLock = bufferSizeRegistrationLock;
      SetInputHandle (inputHandle);
    }

    internal OutputModeType? GetOutputMode()
    {
      IntPtr consoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);
      if (!GetConsoleMode(consoleOutput, out uint uintOutputMode))
        return null;

      var outputMode = (OutputModeType)uintOutputMode;
      return outputMode;
    }

    internal bool SetOutputMode(OutputModeType outputMode)
    {
      IntPtr consoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);
      return SetConsoleMode(consoleOutput, (uint)outputMode);
    }

    internal InputModeType? GetInputMode()
    {
      IntPtr consoleInput = GetStdHandle(STD_INPUT_HANDLE);
      if (!GetConsoleMode(consoleInput, out uint uintInputMode))
        return null;

      var inputMode = (InputModeType)uintInputMode;
      return inputMode;
    }

    internal bool SetInputMode(InputModeType inputMode)
    {
      IntPtr consoleInput = GetStdHandle(STD_INPUT_HANDLE);
      return SetConsoleMode(consoleInput, (uint)inputMode);
    }

    private void Read(CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        if (!GetNumberOfConsoleInputEvents(_inputHandle, out uint lpcNumberOfEvents) || lpcNumberOfEvents <= 0)
          continue;

        if (ReadConsoleInput (_inputHandle, out InputRecord buffer, 1, out uint read) && read > 0)
        {
          var eventType = (EventType)buffer.EventType;

          switch (eventType)
          {
            case EventType.FOCUS_EVENT:
              break;
            case EventType.KEY_EVENT:
              HandleKeyEvent (buffer.Event.KeyEvent, cancellationToken);
              break;
            case EventType.MENU_EVENT:
              break;
            case EventType.MOUSE_EVENT:
              break;
            case EventType.WINDOW_BUFFER_SIZE_EVENT:
              HandleBufferSizeEvent (buffer.Event.WindowBufferSizeEvent, cancellationToken);
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }
    }

    private void HandleBufferSizeEvent (WindowBufferSizeRecord eventWindowBufferSizeEvent, CancellationToken cancellationToken)
    {
      using(_bufferSizeRegistrationLock.EnterScope())
      {
        foreach(var callback in _bufferSizeChangeCallbacks)
        {
          if (cancellationToken.IsCancellationRequested)
            return;

          callback (eventWindowBufferSizeEvent.dwSize.X, eventWindowBufferSizeEvent.dwSize.Y);
        }
      }
    }

    private void HandleKeyEvent(KeyEventRecord eventRecord, CancellationToken cancellationToken)
    {
      if (!eventRecord.bKeyDown || cancellationToken.IsCancellationRequested)
        return;

      ConsoleKey consoleKey = (ConsoleKey)(int)eventRecord.wVirtualKeyCode;
      if (!Enum.IsDefined (consoleKey))
        consoleKey = ConsoleKey.None;

      var ctrlKeyState = eventRecord.dwControlKeyState;
      var keyInfo = new ConsoleKeyInfo (
          eventRecord.UnicodeChar,
          consoleKey,
          (ctrlKeyState & ControlKeyState.SHIFT_PRESSED) != 0,
          (ctrlKeyState & ControlKeyState.LEFT_ALT_PRESSED) != 0,
          (ctrlKeyState & ControlKeyState.LEFT_CTRL_PRESSED) != 0);

      KeyQueue.Add (keyInfo, cancellationToken);
    }
  }
}