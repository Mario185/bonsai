using System;

namespace consoleTools.VirtualTerminalSequences
{
  internal class CommonSequences : SequenceBase
  {
    /// <summary>
    ///   Escape character to notify the terminal it should treat the value as Virtual Terminal Sequence command
    /// </summary>
    public const string ESC = "\x1b";

    public static void ScrollDown (int n, Action<string> writeTo)
    {
      WriteEsc (writeTo);
      writeTo (n.ToString());
      writeTo ("T");
    }

    public static void ScrollUp (int n, Action<string> writeTo)
    {
      WriteEsc (writeTo);
      writeTo (n.ToString());
      writeTo ("S");
    }

    public static void SetScrollingRegion (int top, int bottom, Action<string> writeTo)
    {
      WriteEsc (writeTo);
      writeTo (top.ToString());
      writeTo (";");
      writeTo (bottom.ToString());
      writeTo ("r");
    }
  }
}