using System;
using System.Collections.Concurrent;
using System.Threading;

namespace consoleTools
{
  internal interface IConsoleHandler
  {
    void SetInputHandle (IntPtr? inputHandle);

    void StartOperation (CancellationToken cancellationToken);

    BlockingCollection<ConsoleKeyInfo> KeyQueue { get; }

    void RegisterBufferSizeChange (BufferSizeChangeCallback callback);
    void UnregisterBufferSizeChange(BufferSizeChangeCallback callbackAction);
  }
}
