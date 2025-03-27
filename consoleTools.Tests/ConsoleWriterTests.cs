using System.Text;

namespace consoleTools.Tests
{
  public abstract class ConsoleWriterTests : IDisposable
  {
    private readonly VerifySettings _verifySettings;

    protected ConsoleWriterTests()
    {
      _verifySettings = new VerifySettings();
      _verifySettings.DisableDiff();
      _verifySettings.UseDirectory(Path.Combine("verify_snapshots", GetType().Name));

      ConsoleOutput = new StringBuilder();
      ConsoleWriterInstance = new ConsoleWriter();
      StringWriter consoleOut = new(ConsoleOutput);
      Console.SetOut(consoleOut);
    }

    public StringBuilder ConsoleOutput { get; private set; }
    public ConsoleWriter ConsoleWriterInstance { get; private set; }

    protected Task VerifyOutput()
    {
      return Verify(ConsoleOutput, _verifySettings);
    }

    public void Dispose()
    {
      int outputLengthBeforeDisposing = ConsoleOutput.Length;
      ConsoleWriterInstance.Dispose();
      
      Assert.Equal(outputLengthBeforeDisposing, ConsoleOutput.Length);
    }
  }
}