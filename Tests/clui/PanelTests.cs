using System.Drawing;
using clui.Controls;
using clui.Extensions;

namespace Tests.clui
{
  public class PanelTests : ConsoleOutputRedirectingTest
  {
    [Fact]
    public Task RenderDoesNothingWithEmptyBackgroundColor()
    {
      Panel panel = new Panel(1.AsFixed(), 1.AsFixed());
      panel.BackgroundColor = null;

      panel.CalculatedHeight = 25;
      panel.CalculatedWidth = 25;

      panel.OnLayoutCalculated();
      panel.Render(ConsoleWriterInstance);
      ConsoleWriterInstance.Flush();

      return VerifyOutput();
    }

    [Fact]
    public Task RenderWithBackgroundColor()
    {
      Panel panel = new Panel(1.AsFixed(), 1.AsFixed());
      panel.BackgroundColor = Color.DarkSalmon;

      panel.CalculatedHeight = 25;
      panel.CalculatedWidth = 25;

      panel.OnLayoutCalculated();
      panel.Render(ConsoleWriterInstance);
      ConsoleWriterInstance.Flush();

      return VerifyOutput();
    }
  }
}
