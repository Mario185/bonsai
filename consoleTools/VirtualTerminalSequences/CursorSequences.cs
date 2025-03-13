using System;

namespace consoleTools.VirtualTerminalSequences
{
  internal class SequenceBase
  {
    protected static void WriteEsc(Action<string> writeTo, bool addSquareBracket = true)
    {
      writeTo(CommonSequences.ESC);
      if (addSquareBracket)
        writeTo("[");
    }
  }

  internal class CursorSequences : SequenceBase 
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
    public static void MoveAbsoluteHorizontally(int n, Action<string> writeTo)
    {
      WriteEsc(writeTo);
      writeTo(n.ToString());
      writeTo("G");
    }

    /// <summary>
    ///   Cursor moves to the <paramref name="n" />th position vertically in the current column
    /// </summary>
    public static void MoveAbsoluteVertically(int n, Action<string> writeTo)
    {
      WriteEsc(writeTo);
      writeTo(n.ToString());
      writeTo("d");
    }

    /// <summary>
    ///   Moves cursor down by <paramref name="rows" />
    /// </summary>
    public static void MoveDown(int rows, Action<string> writeTo)
    {
      WriteEsc(writeTo);
      writeTo(rows.ToString());
      writeTo("B");
    }

    /// <summary>
    ///   Moves cursor left by <paramref name="columns" />
    /// </summary>
    public static void MoveLeft(int columns, Action<string> writeTo)
    {
      WriteEsc(writeTo);
      writeTo(columns.ToString());
      writeTo("D");
    }

    /// <summary>
    ///   Moves cursor right by <paramref name="columns" />
    /// </summary>
    public static void MoveRight(int columns, Action<string> writeTo)
    {
      WriteEsc(writeTo);
      writeTo(columns.ToString());
      writeTo("C");
    }

    /// <summary>
    ///   Cursor moves to <paramref name="x" />; <paramref name="y" />> coordinate within the viewport, where
    ///   <paramref name="x" /> is the column of the <paramref name="y" /> line
    /// </summary>
    public static void MoveTo(int x, int y, Action<string> writeTo)
    {
      WriteEsc(writeTo);
      writeTo(y.ToString());
      writeTo(";");
      writeTo(x.ToString());
      writeTo("H");
    }

    /// <summary>
    ///   Moves cursor up by <paramref name="rows" />
    /// </summary>
    public static void MoveUp(int rows, Action<string> writeTo)
    {
      WriteEsc(writeTo);
      writeTo(rows.ToString());
      writeTo("A");
    }

    /// <summary>
    ///   Sets the shape of the cursor
    /// </summary>
    public static void SetShape(CursorShape shape, Action<string> writeTo)
    {
      WriteEsc(writeTo);
      writeTo(((int)shape).ToString());
      writeTo(" q");
    }
  }
}