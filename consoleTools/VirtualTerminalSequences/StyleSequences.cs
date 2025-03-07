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
    public static string SetBackgroundColor(Color color)
    {
      return CommonSequences.ESC + $"[48;2;{color.R};{color.G};{color.B}m";
    }

    /// <summary>
    ///   Set foreground color to RGB value specified by <paramref name="color" />
    /// </summary>
    public static string SetForegroundColor(Color color)
    {
      return CommonSequences.ESC + $"[38;2;{color.R};{color.G};{color.B}m";
    }
  }
}