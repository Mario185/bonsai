using clui.Controls;
using clui.Extensions;

namespace Tests.clui
{
  public class BorderTests : ConsoleOutputRedirectingTest
  {
    [Fact]
    public Task SimpleRender()
    {
      Border border = new Border(10.AsFixed(), 10.AsFixed());

      border.CalculatedHeight = 25;
      border.CalculatedWidth = 25;

      border.OnLayoutCalculated();
      border.Render(ConsoleWriterInstance);
      ConsoleWriterInstance.Flush();

      return VerifyOutput();
    }

    [Theory]
    [InlineData(BorderTextPosition.TopLeft)]
    [InlineData(BorderTextPosition.TopMiddle)]
    [InlineData(BorderTextPosition.TopRight)]
    [InlineData(BorderTextPosition.BottomLeft)]
    [InlineData(BorderTextPosition.BottomMiddle)]
    [InlineData(BorderTextPosition.BottomRight)]
    public Task RenderWithText(BorderTextPosition textPosition)
    {
      Border border = new Border(10.AsFixed(), 10.AsFixed());
      border.TextPosition = textPosition;
      border.Text = "BORDERTEXT";
      border.CalculatedHeight = 25;
      border.CalculatedWidth = 25;

      border.OnLayoutCalculated();
      border.Render(ConsoleWriterInstance);
      ConsoleWriterInstance.Flush();
      
      return VerifyOutput();
    }
  }
}