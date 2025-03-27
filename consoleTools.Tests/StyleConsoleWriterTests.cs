using System.Drawing;

namespace consoleTools.Tests
{
  public class StyleConsoleWriterTests : ConsoleWriterTests
  {
    [Fact]
    public Task SetBackgroundColor()
    {
      ConsoleWriterInstance.Style.BackgroundColor(Color.DarkSalmon).Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task SetBackgroundColorNull()
    {
      ConsoleWriterInstance.Style.BackgroundColor(null).Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task ResetBackgroundColorNull()
    {
      ConsoleWriterInstance.Style.ResetBackgroundColor().Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task Bold()
    {
      ConsoleWriterInstance.Style.Bold().Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task ResetBold()
    {
      ConsoleWriterInstance.Style.ResetBold().Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task SetForegroundColor()
    {
      ConsoleWriterInstance.Style.ForegroundColor(Color.DarkSalmon).Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task SetForegroundColorNull()
    {
      ConsoleWriterInstance.Style.ForegroundColor(null).Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task ResetForegroundColorNull()
    {
      ConsoleWriterInstance.Style.ResetForegroundColor().Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task ResetStyles()
    {
      ConsoleWriterInstance.Style.ResetStyles().Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task Underline()
    {
      ConsoleWriterInstance.Style.Underline().Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task ResetUnderline()
    {
      ConsoleWriterInstance.Style.ResetUnderline().Flush();
      return VerifyOutput();
    }
  }
}