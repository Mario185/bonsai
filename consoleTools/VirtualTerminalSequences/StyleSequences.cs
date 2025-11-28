using System;
using System.Drawing;

namespace consoleTools.VirtualTerminalSequences
{
  internal class StyleSequences : SequenceBase
  {
    /// <summary>
    ///   Adds bold style
    /// </summary>
    public const string Bold = CommonSequences.ESC + "[1m";

    /// <summary>
    /// Adds dim style
    /// </summary>
    public const string Dim = CommonSequences.ESC + "[2m";

    /// <summary>
    /// Adds blinking style
    /// </summary>
    public const string Blink = CommonSequences.ESC + "[5m";

    /// <summary>
    /// Adds reverse style
    /// </summary>
    public const string Reverse = CommonSequences.ESC + "[7m";

    /// <summary>
    /// Adds hidden style
    /// </summary>
    public const string Hidden = CommonSequences.ESC + "[8m";

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
    public static void SetBackgroundColor (Color color, Action<string> writeTo)
    {
      WriteEsc (writeTo);
      writeTo ("48;2;");
      WriteColor (color, writeTo);
      writeTo ("m");
    }

    private static void WriteColor (Color color, Action<string> writeTo)
    {
      writeTo (color.R.ToString());
      writeTo (";");
      writeTo (color.G.ToString());
      writeTo (";");
      writeTo (color.B.ToString());
    }

    /// <summary>
    ///   Set foreground color to RGB value specified by <paramref name="color" />
    /// </summary>
    public static void SetForegroundColor (Color color, Action<string> writeTo)
    {
      WriteEsc (writeTo);
      writeTo ("38;2;");
      WriteColor (color, writeTo);
      writeTo ("m");
    }
  }
}