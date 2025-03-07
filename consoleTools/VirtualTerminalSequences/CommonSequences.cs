namespace consoleTools.VirtualTerminalSequences
{
  internal class CommonSequences
  {
    /// <summary>
    ///   Escape character to notify the terminal it should treat the value as Virtual Terminal Sequence command
    /// </summary>
    public const string ESC = "\x1b";

    public static string ScrollDown(int n)
    {
      return ESC + $"[{n}T";
    }

    public static string ScrollUp(int n)
    {
      return ESC + $"[{n}S";
    }

    public static string SetScrollingRegion(int top, int bottom)
    {
      return ESC + $"[{top};{bottom}r";
    }
  }
}