using System.Drawing;

namespace consoleTools.Tests
{
  public class StyleConsoleWriterTests : ConsoleWriterTests
  {
    [Test]
    public Task SetBackgroundColor()
    {
      ConsoleWriterInstance.Style.BackgroundColor(Color.DarkSalmon).Flush();
      return VerifyOutput();
    }

    [Test]
    public Task SetBackgroundColorNull()
    {
      ConsoleWriterInstance.Style.BackgroundColor(null).Flush();
      return VerifyOutput();
    }

    [Test]
    public Task ResetBackgroundColorNull()
    {
      ConsoleWriterInstance.Style.ResetBackgroundColor().Flush();
      return VerifyOutput();
    }

    [Test]
    public Task Bold()
    {
      ConsoleWriterInstance.Style.Bold().Flush();
      return VerifyOutput();
    }

    [Test]
    public Task ResetBold()
    {
      ConsoleWriterInstance.Style.ResetBold().Flush();
      return VerifyOutput();
    }

    [Test]
    public Task SetForegroundColor()
    {
      ConsoleWriterInstance.Style.ForegroundColor(Color.DarkSalmon).Flush();
      return VerifyOutput();
    }

    [Test]
    public Task SetForegroundColorNull()
    {
      ConsoleWriterInstance.Style.ForegroundColor(null).Flush();
      return VerifyOutput();
    }

    [Test]
    public Task ResetForegroundColorNull()
    {
      ConsoleWriterInstance.Style.ResetForegroundColor().Flush();
      return VerifyOutput();
    }

    [Test]
    public Task ResetStyles()
    {
      ConsoleWriterInstance.Style.ResetStyles().Flush();
      return VerifyOutput();
    }

    [Test]
    public Task Underline()
    {
      ConsoleWriterInstance.Style.Underline().Flush();
      return VerifyOutput();
    }

    [Test]
    public Task ResetUnderline()
    {
      ConsoleWriterInstance.Style.ResetUnderline().Flush();
      return VerifyOutput();
    }
  }
}