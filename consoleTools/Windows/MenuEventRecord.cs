using System;
using System.Runtime.InteropServices;

namespace consoleTools.Windows
{
  [StructLayout (LayoutKind.Sequential)]
  internal struct MenuEventRecord
  {
    internal uint dwCommandId;
  }
}