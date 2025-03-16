using System;

namespace consoleTools.VirtualTerminalSequences
{
  internal class TextModificationSequences : SequenceBase
  {
    /// <summary>
    ///   Delete <paramref name="number" />> characters at the current cursor position, shifting in space characters from the
    ///   right edge of the screen.
    /// </summary>
    public static void DeleteCharacter (int number, Action<string> writeTo)
    {
      WriteEsc (writeTo);
      writeTo (number.ToString());
      writeTo ("P");
    }

    /// <summary>
    ///   Deletes <paramref name="number" /> lines from the buffer, starting with the row the cursor is on.
    /// </summary>
    public static void DeleteLines (int number, Action<string> writeTo)
    {
      WriteEsc (writeTo);
      writeTo (number.ToString());
      writeTo ("M");
    }

    /// <summary>
    ///   Erase <paramref name="number" /> characters from the current cursor position by overwriting them with a space
    ///   character.
    /// </summary>
    public static void EraseCharacter (int number, Action<string> writeTo)
    {
      WriteEsc (writeTo);
      writeTo (number.ToString());
      writeTo ("X");
    }

    /// <summary>
    ///   Replace all text in the current viewport/screen specified by <paramref name="eraseType" /> with space characters
    /// </summary>
    public static void EraseInDisplay (EraseType eraseType, Action<string> writeTo)
    {
      WriteEsc (writeTo);
      writeTo (((int)eraseType).ToString());
      writeTo ("J");
    }

    /// <summary>
    ///   Replace all text on the line with the cursor specified by <paramref name="eraseType" />> with space characters
    /// </summary>
    public static void EraseInLine (EraseType eraseType, Action<string> writeTo)
    {
      WriteEsc (writeTo);
      writeTo (((int)eraseType).ToString());
      writeTo ("K");
    }

    /// <summary>
    ///   Insert <paramref name="number" /> spaces at the current cursor position, shifting all existing text to the right.
    ///   Text exiting the screen to the right is removed.
    /// </summary>
    public static void InsertCharacter (int number, Action<string> writeTo)
    {
      WriteEsc (writeTo);
      writeTo (number.ToString());
      writeTo ("@");
    }

    /// <summary>
    ///   Inserts <paramref name="number" />> lines into the buffer at the cursor position. The line the cursor is on, and
    ///   lines below it, will be shifted downwards.
    /// </summary>
    public static void InsertLine (int number, Action<string> writeTo)
    {
      WriteEsc (writeTo);
      writeTo (number.ToString());
      writeTo ("L");
    }
  }
}