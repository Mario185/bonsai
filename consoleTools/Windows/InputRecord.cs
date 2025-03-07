using System.Drawing;
using System;
using System.Runtime.InteropServices;

namespace consoleTools.Windows
{

  // Helper structure to represent the union and guarantee the correct alignment for
  // both 32-bit and 64-bit Windows.

  [StructLayout(LayoutKind.Sequential)]
  internal struct InputRecord
  {
    internal ushort EventType;
    internal InputRecordUnion Event;
  };
}