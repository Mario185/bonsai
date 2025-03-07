using System;
using System.Runtime.InteropServices;

namespace consoleTools.Windows
{
  [StructLayout(LayoutKind.Sequential)]
  internal struct Coordinate
  {
    internal short X;
    internal short Y;

    internal Coordinate(short x, short y)
    {
      this.X = x;
      this.Y = y;
    }
  }
}