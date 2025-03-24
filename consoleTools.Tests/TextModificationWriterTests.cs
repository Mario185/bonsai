namespace consoleTools.Tests
{
  public class TextModificationWriterTests : ConsoleWriterTests
  {
    [Test]
    [TestCase(1)]
    [TestCase(10)]
    public Task DeleteCharacter(int number)
    {
      ConsoleWriterInstance.Text.DeleteCharacter(number).Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    public Task DeleteLines(int number)
    {
      ConsoleWriterInstance.Text.DeleteLines(number).Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    public Task EraseCharacter(int number)
    {
      ConsoleWriterInstance.Text.EraseCharacter(number).Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(EraseType.EraseEntireLine)]
    [TestCase(EraseType.EraseFromCursorToEnd)]
    [TestCase(EraseType.EraseFromLineBeginningToCursors)]
    public Task EraseInDisplay(EraseType eraseType)
    {
      ConsoleWriterInstance.Text.EraseInDisplay(eraseType).Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(EraseType.EraseEntireLine)]
    [TestCase(EraseType.EraseFromCursorToEnd)]
    [TestCase(EraseType.EraseFromLineBeginningToCursors)]
    public Task EraseInLine(EraseType eraseType)
    {
      ConsoleWriterInstance.Text.EraseInLine(eraseType).Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    public Task InsertCharacter(int number)
    {
      ConsoleWriterInstance.Text.InsertCharacter(number).Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    public Task InsertLine(int number)
    {
      ConsoleWriterInstance.Text.InsertLine(number).Flush();
      return VerifyOutput();
    }
  }
}