namespace consoleTools.Tests
{
  public class CursorWriterTests : ConsoleWriterTests
  {
    [Test]
    public Task DisableBlinking()
    {
      ConsoleWriterInstance.Cursor.DisableBlinking().Flush();
      return VerifyOutput();
    }

    [Test]
    public Task EnableBlinking()
    {
      ConsoleWriterInstance.Cursor.EnableBlinking().Flush();
      return VerifyOutput();
    }

    [Test]
    public Task Hide()
    {
      ConsoleWriterInstance.Cursor.Hide().Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    public Task MoveAbsoluteHorizontally(int number)
    {
      ConsoleWriterInstance.Cursor.MoveAbsoluteHorizontally(number).Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    public Task MoveAbsoluteVertically(int number)
    {
      ConsoleWriterInstance.Cursor.MoveAbsoluteVertically(number).Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    public Task MoveDown(int number)
    {
      ConsoleWriterInstance.Cursor.MoveDown(number).Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    public Task MoveLeft(int number)
    {
      ConsoleWriterInstance.Cursor.MoveLeft(number).Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    public Task MoveUp(int number)
    {
      ConsoleWriterInstance.Cursor.MoveUp(number).Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    public Task MoveRight(int number)
    {
      ConsoleWriterInstance.Cursor.MoveRight(number).Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(1, 2)]
    [TestCase(10, 11)]
    public Task MoveTo(int x, int y)
    {
      ConsoleWriterInstance.Cursor.MoveTo(x, y).Flush();
      return VerifyOutput();
    }

    [Test]
    public Task ResetPosition()
    {
      ConsoleWriterInstance.Cursor.ResetPosition().Flush();
      return VerifyOutput();
    }

    [Test]
    public Task RestorePosition()
    {
      ConsoleWriterInstance.Cursor.RestorePosition().Flush();
      return VerifyOutput();
    }

    [Test]
    public Task ReverseIndex()
    {
      ConsoleWriterInstance.Cursor.ReverseIndex().Flush();
      return VerifyOutput();
    }

    [Test]
    public Task SavePosition()
    {
      ConsoleWriterInstance.Cursor.SavePosition().Flush();
      return VerifyOutput();
    }

    [Test]
    public Task Show()
    {
      ConsoleWriterInstance.Cursor.Show().Flush();
      return VerifyOutput();
    }

    [Test]
    [TestCase(CursorShape.BarBlinking)]
    [TestCase(CursorShape.BarSteady)]
    [TestCase(CursorShape.BlockBlinking)]
    [TestCase(CursorShape.BlockSteady)]
    [TestCase(CursorShape.Default)]
    [TestCase(CursorShape.UnderlineBlinking)]
    [TestCase(CursorShape.UnderlineSteady)]
    public Task SetShape(CursorShape shape)
    {
      ConsoleWriterInstance.Cursor.SetShape(shape).Flush();
      return VerifyOutput();
    }
  }
}