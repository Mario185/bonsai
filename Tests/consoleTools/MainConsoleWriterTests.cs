using consoleTools;

namespace Tests.consoleTools
{
  public class MainConsoleWriterTests : ConsoleOutputRedirectingTest
  {
    [Fact]
    public Task Write_String()
    {
      ConsoleWriterInstance.Write("Just write").Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task Write_Char()
    {
      ConsoleWriterInstance.Write('A').Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task Write_CharArray()
    {
      ConsoleWriterInstance.Write(new[] { 'A', 'B' }).Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task Write_ReadOnlySpanArray()
    {
      ConsoleWriterInstance.Write("Test".AsSpan()).Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task DisposeFlushes()
    {
      using (ConsoleWriter writer = new())
      {
        writer.Write("DisposeFlush");
      }

      return VerifyOutput();
    }

    [Theory]
    [InlineData(0, 4)]
    [InlineData(1, 4)]
    [InlineData(4, 4)]
    [InlineData(10, 1)]
    public Task WriteTruncated(int from, int len)
    {
      ConsoleWriterInstance.WriteTruncated("1234567890", from, len).Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task WriteTruncatedWithEmptyString()
    {
      ConsoleWriterInstance.WriteTruncated("", 0, 0).Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task AlternateScreenBuffer()
    {
      using (new AlternateScreenBufferSection())
      {
        // do nothing
      }

      return VerifyOutput();
    }

    [Fact]
    public Task AlternateScreenBuffer_TwiceDoesNothing()
    {
      using (new AlternateScreenBufferSection())
      {
        using (new AlternateScreenBufferSection())
        {
          // do nothing
        }
      }

      return VerifyOutput();
    }

    [Fact]
    public void SubWritersHaveSameInstance()
    {
      Assert.Same(ConsoleWriterInstance.Style, ConsoleWriterInstance.Cursor.Style);
      Assert.Same(ConsoleWriterInstance.Style, ConsoleWriterInstance.Text.Style);
      Assert.Same(ConsoleWriterInstance.Style, ConsoleWriterInstance.Style.Style);
      Assert.Same(ConsoleWriterInstance.Style.Writer, ConsoleWriterInstance.Text.Writer);
      Assert.Same(ConsoleWriterInstance.Style.Writer, ConsoleWriterInstance.Cursor.Writer);

      Assert.Same(ConsoleWriterInstance.Cursor, ConsoleWriterInstance.Cursor.Cursor);
      Assert.Same(ConsoleWriterInstance.Cursor, ConsoleWriterInstance.Text.Cursor);
      Assert.Same(ConsoleWriterInstance.Cursor, ConsoleWriterInstance.Style.Cursor);
      Assert.Same(ConsoleWriterInstance.Cursor.Writer, ConsoleWriterInstance.Text.Writer);
      Assert.Same(ConsoleWriterInstance.Cursor.Writer, ConsoleWriterInstance.Style.Writer);

      Assert.Same(ConsoleWriterInstance.Text, ConsoleWriterInstance.Cursor.Text);
      Assert.Same(ConsoleWriterInstance.Text, ConsoleWriterInstance.Text.Text);
      Assert.Same(ConsoleWriterInstance.Text, ConsoleWriterInstance.Style.Text);
      Assert.Same(ConsoleWriterInstance.Text.Writer, ConsoleWriterInstance.Cursor.Writer);
      Assert.Same(ConsoleWriterInstance.Text.Writer, ConsoleWriterInstance.Style.Writer);
    }
  }
}