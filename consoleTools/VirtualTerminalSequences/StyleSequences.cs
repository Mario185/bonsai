using System;
using System.Drawing;

namespace consoleTools.VirtualTerminalSequences
{
  internal class StyleSequences
  {
    /// <summary>
    ///   Adds bold style
    /// </summary>
    public const string Bold = CommonSequences.ESC + "[1m";

    public const string Flashing = CommonSequences.ESC + "[5m";

    /// <summary>
    ///   Returns all attributes to the default state prior to modification
    /// </summary>
    public const string Reset = CommonSequences.ESC + "[0m";

    /// <summary>
    ///   Resets the background color the default state prior to modification
    /// </summary>
    public const string ResetBackgroundColor = CommonSequences.ESC + "[49m";

    /// <summary>
    ///   Removes bold style
    /// </summary>
    public const string ResetBold = CommonSequences.ESC + "[22m";

    /// <summary>
    ///   Resets the foreground color the default state prior to modification
    /// </summary>
    public const string ResetForegroundColor = CommonSequences.ESC + "[39m";

    /// <summary>
    ///   Removes underline style
    /// </summary>
    public const string ResetUnderline = CommonSequences.ESC + "[24m";

    /// <summary>
    ///   Adds underline style
    /// </summary>
    public const string Underline = CommonSequences.ESC + "[4m";

    /// <summary>
    ///   Set background color to RGB value specified by <paramref name="color" />
    /// </summary>
    public static void SetBackgroundColor(Color color, Action<string> write)
    {
      write(CommonSequences.ESC);
      write("[48;2;");
      WriteColor(color, write);
      write("m");
    }

    private static void WriteColor(Color color, Action<string> write)
    {
      write(color.R.ToString());
      write(";");
      write(color.G.ToString());
      write(";");
      write(color.B.ToString());
    }

    /// <summary>
    ///   Set foreground color to RGB value specified by <paramref name="color" />
    /// </summary>
    public static void SetForegroundColor(Color color, Action<string> write)
    {
      write(CommonSequences.ESC);
      write("[38;2;");
      WriteColor(color, write);
      write("m");
    }
  }
}