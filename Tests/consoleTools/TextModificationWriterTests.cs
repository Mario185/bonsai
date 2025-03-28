using consoleTools;

namespace Tests.consoleTools
{
  public class TextModificationWriterTests : ConsoleWriterTests
  {
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public Task DeleteCharacter(int number)
    {
      ConsoleWriterInstance.Text.DeleteCharacter(number).Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public Task DeleteLines(int number)
    {
      ConsoleWriterInstance.Text.DeleteLines(number).Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public Task EraseCharacter(int number)
    {
      ConsoleWriterInstance.Text.EraseCharacter(number).Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(EraseType.EraseEntireLine)]
    [InlineData(EraseType.EraseFromCursorToEnd)]
    [InlineData(EraseType.EraseFromLineBeginningToCursors)]
    public Task EraseInDisplay(EraseType eraseType)
    {
      ConsoleWriterInstance.Text.EraseInDisplay(eraseType).Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(EraseType.EraseEntireLine)]
    [InlineData(EraseType.EraseFromCursorToEnd)]
    [InlineData(EraseType.EraseFromLineBeginningToCursors)]
    public Task EraseInLine(EraseType eraseType)
    {
      ConsoleWriterInstance.Text.EraseInLine(eraseType).Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public Task InsertCharacter(int number)
    {
      ConsoleWriterInstance.Text.InsertCharacter(number).Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public Task InsertLine(int number)
    {
      ConsoleWriterInstance.Text.InsertLine(number).Flush();
      return VerifyOutput();
    }
  }
}