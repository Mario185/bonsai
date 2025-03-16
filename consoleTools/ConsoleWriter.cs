using System;
using System.IO;
using System.Text;
using System.Threading;
using consoleTools.SubWriter;

namespace consoleTools
{
  public sealed class ConsoleWriter : IDisposable
  {
    private static readonly Lock s_flushLock = new();
    private readonly StringBuilder _commandBuffer = new((Console.WindowHeight * Console.WindowWidth) * 4);

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
      using(s_flushLock.EnterScope())
      {
        foreach (ReadOnlyMemory<char> chunk in _commandBuffer.GetChunks())
        {
          Console.Out.Write(chunk.Span);
        }

        Console.Out.Flush();
      }

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
      return WriteTruncated(text.AsSpan(), from, len);
    }

    public ConsoleWriter WriteTruncated(ReadOnlySpan<char> span, int from, int len)
    {
      if (span == string.Empty)
      {
        return this;
      }

      if (from >= span.Length || len <= 0)
      {
        return this;
      }

      int clampedLen = Math.Min(len, span.Length - from);
      
      _commandBuffer.Append(span.Slice(from, clampedLen));
      return this;
    }
    public ConsoleWriter Write (char[] c)
    {
      _commandBuffer.Append (c);
      return this;
    }

    public ConsoleWriter Write(char c)
    {
      _commandBuffer.Append(c);
      return this;
    }

    public ConsoleWriter Write(ReadOnlySpan<char> chars)
    {
      _commandBuffer.Append(chars);
      return this;
    }

    /// <summary>
    ///   Clears all commands from the buffer
    /// </summary>
    public void ClearCommandBuffer ()
    {
      _commandBuffer.Clear();
    }

    public void Dispose()
    {

    }
  }
}