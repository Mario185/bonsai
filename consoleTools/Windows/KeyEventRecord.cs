using System;
using System.Runtime.InteropServices;
using consoleTools.Utilities;

namespace consoleTools.Windows
{
  [StructLayout (LayoutKind.Explicit, CharSet = CharSet.Unicode)]
  internal struct KeyEventRecord
  {
    [FieldOffset (0)]
    [MarshalAs (UnmanagedType.Bool)]
    internal bool bKeyDown;
    [FieldOffset (4)]
    [MarshalAs (UnmanagedType.U2)]
    internal ushort wRepeatCount;
    [FieldOffset (6)]
    [MarshalAs (UnmanagedType.U2)]
    internal VirtualKeys wVirtualKeyCode;
    [FieldOffset (8)]
    [MarshalAs (UnmanagedType.U2)]
    internal ushort wVirtualScanCode;
    [FieldOffset (10)]
    internal char UnicodeChar;
    [FieldOffset (12)]
    [MarshalAs (UnmanagedType.U4)]
    internal ControlKeyState dwControlKeyState;
  }
}