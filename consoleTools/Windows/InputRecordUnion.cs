using System;
using System.Runtime.InteropServices;

namespace consoleTools.Windows
{
  [StructLayout (LayoutKind.Explicit)]
  internal struct InputRecordUnion
  {
    [FieldOffset (0)]
    internal KeyEventRecord KeyEvent;
    [FieldOffset (0)]
    internal MouseEventRecord MouseEvent;
    [FieldOffset (0)]
    internal WindowBufferSizeRecord WindowBufferSizeEvent;
    [FieldOffset (0)]
    internal MenuEventRecord MenuEvent;
    [FieldOffset (0)]
    internal FocusEventRecord FocusEvent;
  }
}