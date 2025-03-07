using System;

namespace consoleTools.Windows
{
  internal struct WindowBufferSizeRecord
  {
    internal Coordinate dwSize;

    internal WindowBufferSizeRecord(short x, short y)
    {
      dwSize = new Coordinate(x, y);
    }
  }
}