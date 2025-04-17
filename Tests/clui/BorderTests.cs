using System.Drawing;
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

      Assert.Equal(1, border.Padding.Top);
      Assert.Equal(1, border.Padding.Right);
      Assert.Equal(1, border.Padding.Bottom);
      Assert.Equal(1, border.Padding.Left);

      return VerifyOutput();
    }

    [Theory]
    [InlineData(BorderTextPosition.TopLeft, false)]
    [InlineData(BorderTextPosition.TopMiddle, false)]
    [InlineData(BorderTextPosition.TopRight, false)]
    [InlineData(BorderTextPosition.BottomLeft, false)]
    [InlineData(BorderTextPosition.BottomMiddle, false)]
    [InlineData(BorderTextPosition.BottomRight, false)]
    [InlineData(BorderTextPosition.TopLeft, true)]
    [InlineData(BorderTextPosition.TopMiddle, true)]
    [InlineData(BorderTextPosition.TopRight, true)]
    [InlineData(BorderTextPosition.BottomLeft, true)]
    [InlineData(BorderTextPosition.BottomMiddle, true)]
    [InlineData(BorderTextPosition.BottomRight, true)]
    public Task RenderWithText(BorderTextPosition textPosition, bool setTextColor)
    {
      Border border = new Border(10.AsFixed(), 10.AsFixed());
      border.TextPosition = textPosition;
      border.Text = "BORDERTEXT";

      border.TextColor = setTextColor ? Color.DarkSalmon : null;
      border.TextBackgroundColor = setTextColor ? Color.Aqua : null;
      border.BorderColor = Color.Brown;

      border.CalculatedHeight = 25;
      border.CalculatedWidth = 25;

      border.OnLayoutCalculated();
      border.Render(ConsoleWriterInstance);
      ConsoleWriterInstance.Flush();

      Assert.Equal(1, border.Padding.Top);
      Assert.Equal(1, border.Padding.Right);
      Assert.Equal(1, border.Padding.Bottom);
      Assert.Equal(1, border.Padding.Left);

      return VerifyOutput();
    }

    [Fact]
    public void RenderWithInvalidTextPositionThrowsException()
    {
      Border border = new Border(10.AsFixed(), 10.AsFixed());
      border.TextPosition = (BorderTextPosition)(-1);
      border.Text = "BORDERTEXT";

      border.CalculatedHeight = 25;
      border.CalculatedWidth = 25;

      border.OnLayoutCalculated();
      
      Assert.Throws<ArgumentOutOfRangeException>(() => border.Render(ConsoleWriterInstance));
      ConsoleWriterInstance.Flush();
    }

    [Fact]
    public Task RenderWithNoBorder()
    {
      Border border = new Border(10.AsFixed(), 10.AsFixed(), null,null,null,null,null,null,null,null);
      
      border.CalculatedHeight = 25;
      border.CalculatedWidth = 25;

      border.OnLayoutCalculated();
      border.Render(ConsoleWriterInstance);
      ConsoleWriterInstance.Flush();

      Assert.Equal(0, border.Padding.Top);
      Assert.Equal(0, border.Padding.Right);
      Assert.Equal(0, border.Padding.Bottom);
      Assert.Equal(0, border.Padding.Left);

      return VerifyOutput();
    }

    [Fact]
    public Task VisbleFalseDoesNotRenderAnything()
    {
      Border border = new Border(10.AsFixed(), 10.AsFixed(), null, null, null, null, null, null, null, null);
      border.Visible = false;
      border.CalculatedHeight = 25;
      border.CalculatedWidth = 25;

      border.OnLayoutCalculated();
      border.Render(ConsoleWriterInstance);
      ConsoleWriterInstance.Flush();

      return VerifyOutput();
    }
  }
}