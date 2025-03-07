using System;
using System.Text;
using consoleTools.SubWriter;

namespace consoleTools
{
  public class ConsoleWriter
  {
    private readonly StringBuilder _commandBuffer = new();

    public ConsoleWriter ()
    {
      Style = new StyleWriter (this);
      Cursor = new CursorWriter (this);
      Text = new TextModificationWriter (this);
    }

    public CursorWriter Cursor { get; }
    public StyleWriter Style { get; }
    public TextModificationWriter Text { get; }

    /// <summary>
    ///   Flushes all written commands to the console
    /// </summary>
    public void Flush ()
    {
      Console.Write (_commandBuffer);
      ClearCommandBuffer();
    }

    /// <summary>
    ///   Writes plain text to the buffer.
    /// </summary>
    public ConsoleWriter Write (string text)
    {
      _commandBuffer.Append (text);
      return this;
    }

    public ConsoleWriter WriteTruncated (string text, int from, int len)
    {
      if (text == string.Empty)
        return this;

      int target = Math.Clamp (from + len, 0, text.Length);

      for (int i = from; i < target; i++)
        Write (text[i]);

      return this;
    }

    public ConsoleWriter Write (params char[] c)
    {
      _commandBuffer.Append (c);
      return this;
    }

    /// <summary>
    ///   Clears all commands from the buffer
    /// </summary>
    public void ClearCommandBuffer ()
    {
      _commandBuffer.Clear();
    }
  }
}