using System.Drawing;
using System.Text;
using clui.Controls;
using clui.Extensions;
using consoleTools;

namespace Tests.clui
{

  public class PanelTests : VerifyTestBase, IDisposable
  {
    public StringBuilder ConsoleOutput { get; private set; }
    public ConsoleWriter ConsoleWriterInstance { get; private set; }

    public PanelTests()
    {
      ConsoleOutput = new StringBuilder();
      ConsoleWriterInstance = new ConsoleWriter();
      StringWriter consoleOut = new(ConsoleOutput);
      Console.SetOut(consoleOut);
    }


    [Fact]
    public Task RenderDoesNothingWithEmptyBackgroundColor()
    {
      Panel panel = new Panel(1.AsFixed(), 1.AsFixed());
      panel.BackgroundColor = null;

      panel.CalculatedHeight = 25;
      panel.CalculatedWidth = 25;

      panel.Render(ConsoleWriterInstance);
      ConsoleWriterInstance.Flush();

      return Verify(ConsoleOutput, VerifySettings);
    }

    [Fact]
    public Task RenderWithBackgroundColor()
    {
      Panel panel = new Panel(1.AsFixed(), 1.AsFixed());
      panel.BackgroundColor = Color.DarkSalmon;

      panel.CalculatedHeight = 25;
      panel.CalculatedWidth = 25;
      panel.Render(ConsoleWriterInstance);
      ConsoleWriterInstance.Flush();

      return Verify(ConsoleOutput, VerifySettings);
    }

    public void Dispose()
    {
      int outputLengthBeforeDisposing = ConsoleOutput.Length;
      ConsoleWriterInstance.Dispose();

      Assert.Equal(outputLengthBeforeDisposing, ConsoleOutput.Length);
    }
  }
}
