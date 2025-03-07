using System.Drawing;
using consoleTools.VirtualTerminalSequences;

namespace consoleTools.SubWriter
{
  public class StyleWriter : SubWriterBase
  {
    internal StyleWriter(ConsoleWriter consoleWriter)
      : base(consoleWriter)
    {
    }

    public StyleWriter BackgroundColor(Color? color)
    {
      if (!color.HasValue)
        return this;

      Writer.Write(StyleSequences.SetBackgroundColor(color.Value));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="StyleSequences.Bold" />
    /// </summary>
    public StyleWriter Bold()
    {
      Writer.Write(StyleSequences.Bold);
      return this;
    }

    public StyleWriter ForegroundColor(Color? color)
    {
      if (!color.HasValue)
        return this;

      Writer.Write(StyleSequences.SetForegroundColor(color.Value));
      return this;
    }

    public StyleWriter ResetBackgroundColor()
    {
      Writer.Write(StyleSequences.ResetBackgroundColor);
      return this;
    }

    public StyleWriter ResetBold()
    {
      Writer.Write(StyleSequences.ResetBold);
      return this;
    }

    public StyleWriter ResetForegroundColor()
    {
      Writer.Write(StyleSequences.ResetForegroundColor);
      return this;
    }

    public StyleWriter ResetStyles()
    {
      Writer.Write(StyleSequences.Reset);
      return this;
    }

    public StyleWriter ResetUnderscore()
    {
      Writer.Write(StyleSequences.ResetUnderline);
      return this;
    }

    public StyleWriter Underline()
    {
      Writer.Write(StyleSequences.Underline);
      return this;
    }
  }
}