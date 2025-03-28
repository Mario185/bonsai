using System.Text;
using consoleTools;

namespace Tests.consoleTools
{
  public abstract class ConsoleWriterTests : VerifyTestBase, IDisposable
  {
    protected ConsoleWriterTests()
    {
      ConsoleOutput = new StringBuilder();
      ConsoleWriterInstance = new ConsoleWriter();
      StringWriter consoleOut = new(ConsoleOutput);
      Console.SetOut(consoleOut);
    }

    public StringBuilder ConsoleOutput { get; private set; }
    public ConsoleWriter ConsoleWriterInstance { get; private set; }

    protected Task VerifyOutput()
    {
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