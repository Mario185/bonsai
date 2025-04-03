using System.Runtime.CompilerServices;
using System.Text;
using consoleTools;

namespace Tests
{
  public abstract class ConsoleOutputRedirectingTest : VerifyTestBase, IDisposable
  {
    protected ConsoleOutputRedirectingTest()
    {
      ConsoleOutput = new StringBuilder();
      ConsoleWriterInstance = new ConsoleWriter();
      StringWriter consoleOut = new(ConsoleOutput);
      Console.SetOut(consoleOut);
    }

    public StringBuilder ConsoleOutput { get; private set; }
    public ConsoleWriter ConsoleWriterInstance { get; private set; }

    protected Task VerifyOutput([CallerFilePath] string callerFilePath = "")
    {
      // ReSharper disable once ExplicitCallerInfoArgument
      return Verify(ConsoleOutput, VerifySettings, callerFilePath );
    }

    public void Dispose()
    {
      int outputLengthBeforeDisposing = ConsoleOutput.Length;
      ConsoleWriterInstance.Dispose();

      Assert.Equal(outputLengthBeforeDisposing, ConsoleOutput.Length);
    }
  }
}