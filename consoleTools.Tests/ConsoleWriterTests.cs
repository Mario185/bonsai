using System.Text;

namespace consoleTools.Tests
{
  public abstract class ConsoleWriterTests
  {
    private readonly VerifySettings _verifySettings;

    protected ConsoleWriterTests()
    {
      _verifySettings = new VerifySettings();
      _verifySettings.DisableDiff();
      _verifySettings.UseDirectory(Path.Combine("verify_snapshots", GetType().Name));
    }

    public StringBuilder ConsoleOutput { get; private set; }
    public ConsoleWriter ConsoleWriterInstance { get; private set; }

    [SetUp]
    public void Setup()
    {
      ConsoleOutput = new StringBuilder();
      ConsoleWriterInstance = new ConsoleWriter();
      StringWriter consoleOut = new(ConsoleOutput);
      Console.SetOut(consoleOut);
    }

    [TearDown]
    public void TearDown()
    {
      int outputLengthBeforeDisposing = ConsoleOutput.Length;
      ConsoleWriterInstance.Dispose();
      Assert.That(ConsoleOutput.Length, Is.EqualTo(outputLengthBeforeDisposing), "Test forgot flushing the console writer");
    }

    protected Task VerifyOutput()
    {
      return Verify(ConsoleOutput, _verifySettings);
    }
  }
}