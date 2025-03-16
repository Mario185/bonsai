using System;
using System.Runtime.InteropServices;

namespace consoleTools.Windows
{
  [StructLayout (LayoutKind.Sequential)]
  internal struct MouseEventRecord
  {
    internal Coordinate dwMousePosition;
    internal MouseButtonState dwButtonState;
    internal ControlKeyState dwControlKeyState;
    internal MouseEventFlags dwEventFlags;
  }
}