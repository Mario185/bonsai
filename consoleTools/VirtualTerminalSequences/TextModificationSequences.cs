namespace consoleTools.VirtualTerminalSequences
{
  internal class TextModificationSequences
  {
    /// <summary>
    ///   Delete <paramref name="number" />> characters at the current cursor position, shifting in space characters from the
    ///   right edge of the screen.
    /// </summary>
    public static string DeleteCharacter(int number = 1)
    {
      return CommonSequences.ESC + $"[{number}P";
    }

    /// <summary>
    ///   Deletes <paramref name="number" /> lines from the buffer, starting with the row the cursor is on.
    /// </summary>
    public static string DeleteLines(int number = 1)
    {
      return CommonSequences.ESC + $"[{number}M";
    }

    /// <summary>
    ///   Erase <paramref name="number" /> characters from the current cursor position by overwriting them with a space
    ///   character.
    /// </summary>
    public static string EraseCharacter(int number = 1)
    {
      return CommonSequences.ESC + $"[{number}X";
    }

    /// <summary>
    ///   Replace all text in the current viewport/screen specified by <paramref name="eraseType" /> with space characters
    /// </summary>
    public static string EraseInDisplay(EraseType eraseType)
    {
      return CommonSequences.ESC + $"[{eraseType}J";
    }

    /// <summary>
    ///   Replace all text on the line with the cursor specified by <paramref name="eraseType" />> with space characters
    /// </summary>
    public static string EraseInLine(EraseType eraseType)
    {
      return CommonSequences.ESC + $"[{eraseType}J";
    }

    /// <summary>
    ///   Insert <paramref name="number" /> spaces at the current cursor position, shifting all existing text to the right.
    ///   Text exiting the screen to the right is removed.
    /// </summary>
    public static string InsertCharacter(int number = 1)
    {
      return CommonSequences.ESC + $"[{number}@";
    }

    /// <summary>
    ///   Inserts <paramref name="number" />> lines into the buffer at the cursor position. The line the cursor is on, and
    ///   lines below it, will be shifted downwards.
    /// </summary>
    public static string InsertLine(int number = 1)
    {
      return CommonSequences.ESC + $"[{number}L";
    }
  }
}