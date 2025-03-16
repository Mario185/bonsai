using System;
using System.Collections.Concurrent;
using System.Threading;

namespace consoleTools
{
  internal interface IConsoleHandler
  {
    BlockingCollection<ConsoleKeyInfo> KeyQueue { get; }
    void SetInputHandle (IntPtr? inputHandle);

    void StartOperation (CancellationToken cancellationToken);

    void RegisterBufferSizeChange (BufferSizeChangeCallback callback);
    void UnregisterBufferSizeChange (BufferSizeChangeCallback callbackAction);
  }
}