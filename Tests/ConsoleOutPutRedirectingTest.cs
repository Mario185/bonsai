using System.Runtime.CompilerServices;
using System.Text;
using consoleTools;

namespace Tests
{
  public abstract class ConsoleOutputRedirectingTest : VerifyTestBase, IDisposable
  {
    protected ConsoleOutputRedirectingTest()
    {
      FakedConsoleOutput = new StringBuilder();
      FakedConsoleOutputWriter = new(FakedConsoleOutput);
      ConsoleWriterInstance = new ConsoleWriter(FakedConsoleOutputWriter);
    }

    public StringBuilder FakedConsoleOutput { get; }
    public StringWriter FakedConsoleOutputWriter { get; }
    public ConsoleWriter ConsoleWriterInstance { get; }

    protected Task VerifyOutput([CallerFilePath] string callerFilePath = "")
    {
      // ReSharper disable once ExplicitCallerInfoArgument
      return Verify(FakedConsoleOutput, VerifySettings, callerFilePath );
    }

    public void Dispose()
    {
      int outputLengthBeforeDisposing = FakedConsoleOutput.Length;
      ConsoleWriterInstance.Dispose();

      Assert.Equal(outputLengthBeforeDisposing, FakedConsoleOutput.Length);
    }
  }
}