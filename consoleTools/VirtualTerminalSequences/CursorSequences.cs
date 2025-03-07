namespace consoleTools.VirtualTerminalSequences
{
  internal class CursorSequences
  {
    /// <summary>
    ///   Stop blinking the cursor
    /// </summary>
    public const string DisableBlinking = CommonSequences.ESC + "[?12l";

    /// <summary>
    ///   Start the cursor blinking
    /// </summary>
    public const string EnableBlinking = CommonSequences.ESC + "[?12h";

    /// <summary>
    ///   Hide the cursor
    /// </summary>
    public const string Hide = CommonSequences.ESC + "[?25l";

    /// <summary>
    ///   Moves cursor to position 1,1
    /// </summary>
    public const string ResetPosition = CommonSequences.ESC + "[1;1H";

    /// <summary>
    ///   Restore Cursor Position from Memory
    /// </summary>
    public const string RestorePosition = CommonSequences.ESC + "8";

    /// <summary>
    ///   Reverse Index – Performs the reverse operation of \n, moves cursor up one line, maintains horizontal position,
    ///   scrolls buffer if necessary
    /// </summary>
    public const string ReverseIndex = CommonSequences.ESC + "M";

    /// <summary>
    ///   Save Cursor Position in Memory
    /// </summary>
    public const string SavePosition = CommonSequences.ESC + "7";

    /// <summary>
    ///   Show the cursor
    /// </summary>
    public const string Show = CommonSequences.ESC + "[?25h";

    /// <summary>
    ///   Cursor moves to <paramref name="n" />th position horizontally in the current line
    /// </summary>
    public static string MoveAbsoluteHorizontally(int n)
    {
      return CommonSequences.ESC + $"[{n}G";
    }

    /// <summary>
    ///   Cursor moves to the <paramref name="n" />th position vertically in the current column
    /// </summary>
    public static string MoveAbsoluteVertically(int n)
    {
      return CommonSequences.ESC + $"[{n}d";
    }

    /// <summary>
    ///   Moves cursor down by <paramref name="rows" />
    /// </summary>
    public static string MoveDown(int rows = 1)
    {
      return CommonSequences.ESC + $"[{rows}B";
    }

    /// <summary>
    ///   Moves cursor left by <paramref name="columns" />
    /// </summary>
    public static string MoveLeft(int columns = 1)
    {
      return CommonSequences.ESC + $"[{columns}D";
    }

    /// <summary>
    ///   Moves cursor right by <paramref name="columns" />
    /// </summary>
    public static string MoveRight(int columns = 1)
    {
      return CommonSequences.ESC + $"[{columns}C";
    }

    /// <summary>
    ///   Cursor moves to <paramref name="x" />; <paramref name="y" />> coordinate within the viewport, where
    ///   <paramref name="x" /> is the column of the <paramref name="y" /> line
    /// </summary>
    public static string MoveTo(int x, int y)
    {
      return CommonSequences.ESC + $"[{y};{x}H";
    }

    /// <summary>
    ///   Moves cursor up by <paramref name="rows" />
    /// </summary>
    public static string MoveUp(int rows = 1)
    {
      return CommonSequences.ESC + $"[{rows}A";
    }

    /// <summary>
    ///   Sets the shape of the cursor
    /// </summary>
    public static string SetShape(CursorShape shape)
    {
      return CommonSequences.ESC + $"[{(int)shape} q";
    }
  }
}