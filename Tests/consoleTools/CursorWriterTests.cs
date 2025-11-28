using consoleTools;

namespace Tests.consoleTools
{
  public class CursorWriterTests : ConsoleOutputRedirectingTest
  {
    [Fact]
    public Task DisableBlinking()
    {
      ConsoleWriterInstance.Cursor.DisableBlinking().Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task EnableBlinking()
    {
      ConsoleWriterInstance.Cursor.EnableBlinking().Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task Hide()
    {
      ConsoleWriterInstance.Cursor.Hide().Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public Task MoveAbsoluteHorizontally(int number)
    {
      ConsoleWriterInstance.Cursor.MoveAbsoluteHorizontally(number).Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public Task MoveAbsoluteVertically(int number)
    {
      ConsoleWriterInstance.Cursor.MoveAbsoluteVertically(number).Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public Task MoveDown(int number)
    {
      ConsoleWriterInstance.Cursor.MoveDown(number).Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public Task MoveLeft(int number)
    {
      ConsoleWriterInstance.Cursor.MoveLeft(number).Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public Task MoveUp(int number)
    {
      ConsoleWriterInstance.Cursor.MoveUp(number).Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public Task MoveRight(int number)
    {
      ConsoleWriterInstance.Cursor.MoveRight(number).Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(10, 11)]
    public Task MoveTo(int x, int y)
    {
      ConsoleWriterInstance.Cursor.MoveTo(x, y).Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task ResetPosition()
    {
      ConsoleWriterInstance.Cursor.ResetPosition().Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task RestorePosition()
    {
      ConsoleWriterInstance.Cursor.RestorePosition().Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task SavePosition()
    {
      ConsoleWriterInstance.Cursor.SavePosition().Flush();
      return VerifyOutput();
    }

    [Fact]
    public Task Show()
    {
      ConsoleWriterInstance.Cursor.Show().Flush();
      return VerifyOutput();
    }

    [Theory]
    [InlineData(CursorShape.BarBlinking)]
    [InlineData(CursorShape.BarSteady)]
    [InlineData(CursorShape.BlockBlinking)]
    [InlineData(CursorShape.BlockSteady)]
    [InlineData(CursorShape.Default)]
    [InlineData(CursorShape.UnderlineBlinking)]
    [InlineData(CursorShape.UnderlineSteady)]
    public Task SetShape(CursorShape shape)
    {
      ConsoleWriterInstance.Cursor.SetShape(shape).Flush();
      return VerifyOutput();
    }
  }
}