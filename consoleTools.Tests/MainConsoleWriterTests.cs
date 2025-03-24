namespace consoleTools.Tests
{
  public class MainConsoleWriterTests : ConsoleWriterTests
  {
    [Test]
    public Task Write_String()
    {
      ConsoleWriterInstance.Write("Just write").Flush();
      return VerifyOutput();
    }

    [Test]
    public Task Write_Char()
    {
      ConsoleWriterInstance.Write('A').Flush();
      return VerifyOutput();
    }

    [Test]
    public Task Write_CharArray()
    {
      ConsoleWriterInstance.Write(new[] { 'A', 'B' }).Flush();
      return VerifyOutput();
    }

    [Test]
    public Task Write_ReadOnlySpanArray()
    {
      ConsoleWriterInstance.Write("Test".AsSpan()).Flush();
      return VerifyOutput();
    }

    [Test]
    public Task DisposeFlushes()
    {
      using (ConsoleWriter writer = new())
      {
        writer.Write("DisposeFlush");
      }

      return VerifyOutput();
    }

    [Test]
    [TestCase(0, 4)]
    [TestCase(1, 4)]
    [TestCase(4, 4)]
    [TestCase(10, 1)]
    public Task WriteTruncated(int from, int len)
    {
      ConsoleWriterInstance.WriteTruncated("1234567890", from, len).Flush();
      return VerifyOutput();
    }

    [Test]
    public Task WriteTruncatedWithEmptyString()
    {
      ConsoleWriterInstance.WriteTruncated("", 0, 0).Flush();
      return VerifyOutput();
    }

    [Test]
    public Task AlternateScreenBuffer()
    {
      using (new AlternateScreenBufferSection())
      {
        // do nothing
      }

      return VerifyOutput();
    }

    [Test]
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

    [Test]
    public void SubWritersHaveSameInstance()
    {
      Assert.That(ConsoleWriterInstance.Style, Is.SameAs(ConsoleWriterInstance.Cursor.Style));
      Assert.That(ConsoleWriterInstance.Style, Is.SameAs(ConsoleWriterInstance.Text.Style));
      Assert.That(ConsoleWriterInstance.Style, Is.SameAs(ConsoleWriterInstance.Style.Style));
      Assert.That(ConsoleWriterInstance.Style.Writer, Is.SameAs(ConsoleWriterInstance.Text.Writer));
      Assert.That(ConsoleWriterInstance.Style.Writer, Is.SameAs(ConsoleWriterInstance.Cursor.Writer));

      Assert.That(ConsoleWriterInstance.Cursor, Is.SameAs(ConsoleWriterInstance.Cursor.Cursor));
      Assert.That(ConsoleWriterInstance.Cursor, Is.SameAs(ConsoleWriterInstance.Text.Cursor));
      Assert.That(ConsoleWriterInstance.Cursor, Is.SameAs(ConsoleWriterInstance.Style.Cursor));
      Assert.That(ConsoleWriterInstance.Cursor.Writer, Is.SameAs(ConsoleWriterInstance.Text.Writer));
      Assert.That(ConsoleWriterInstance.Cursor.Writer, Is.SameAs(ConsoleWriterInstance.Style.Writer));

      Assert.That(ConsoleWriterInstance.Text, Is.SameAs(ConsoleWriterInstance.Cursor.Text));
      Assert.That(ConsoleWriterInstance.Text, Is.SameAs(ConsoleWriterInstance.Text.Text));
      Assert.That(ConsoleWriterInstance.Text, Is.SameAs(ConsoleWriterInstance.Style.Text));
      Assert.That(ConsoleWriterInstance.Text.Writer, Is.SameAs(ConsoleWriterInstance.Cursor.Writer));
      Assert.That(ConsoleWriterInstance.Text.Writer, Is.SameAs(ConsoleWriterInstance.Style.Writer));
    }
  }
}