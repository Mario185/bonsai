using System;

namespace consoleTools.SubWriter
{
  public class SubWriterBase
  {
    protected SubWriterBase(ConsoleWriter consoleWriter)
    {
      Writer = consoleWriter;
      ArgumentNullException.ThrowIfNull(consoleWriter);
    }

    /// <summary>
    ///   Gets the original <see cref="ConsoleWriter" />>
    /// </summary>
    public ConsoleWriter Writer { get; }

    public CursorWriter Cursor => Writer.Cursor;
    public StyleWriter Style => Writer.Style;
    public TextModificationWriter Text => Writer.Text;

    public void Flush()
    {
      Writer.Flush();
    }
  }
}