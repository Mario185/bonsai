using System.Collections.Concurrent;

namespace consoleTools.Tests
{
  [TestFixture]
  internal class ConsoleHandlerTests
  {
    [Test]
    public void Read()
    {
      var consoleImplementation = new TestConsoleImplementation();

      ConsoleHandler.Initialize(consoleImplementation);

      ConsoleKeyInfo? readValue = null;
      ManualResetEventSlim resetEvent = new();
      var readThread = new Thread(() =>
      {
        readValue = ConsoleHandler.Read(true);
        resetEvent.Set();
      });
      readThread.Start();

      consoleImplementation.KeyQueue.Add(new ConsoleKeyInfo('A', ConsoleKey.A, false, false,false));
      resetEvent.Wait();

      Assert.That(readValue.HasValue, Is.True);
      Assert.That(readValue.Value.Key, Is.EqualTo(ConsoleKey.A));
      Assert.That(readValue.Value.KeyChar, Is.EqualTo('A'));
    }

    [Test]
    public void BufferSizeChange()
    {
      var consoleImplementation = new TestConsoleImplementation();
      ConsoleHandler.Initialize(consoleImplementation);
      ConsoleHandler.StartOperation();
      
      ManualResetEventSlim resetEvent = new();

      int receivedWidth = 0;
      int receivedHeight = 0;

      ConsoleHandler.RegisterBufferSizeChangeCallback(BufferSizeCallBack);
      consoleImplementation.WindowHeight = 12;
      consoleImplementation.WindowWidth = 13;

      resetEvent.Wait();
      resetEvent.Reset();
      
      Assert.That(receivedHeight, Is.EqualTo(12));
      Assert.That(receivedWidth, Is.EqualTo(13));

      ConsoleHandler.UnregisterBufferSizeChangeCallback(BufferSizeCallBack);
      consoleImplementation.WindowHeight = 16;
      consoleImplementation.WindowWidth = 17;
      resetEvent.Wait(100);

      Assert.That(receivedHeight, Is.EqualTo(12));
      Assert.That(receivedWidth, Is.EqualTo(13));

      ConsoleHandler.CancelOperation();

      void BufferSizeCallBack(int width, int height)
      {
        receivedHeight = height;
        receivedWidth = width;
        resetEvent.Set();
      }
    }
  }

  public class TestConsoleImplementation : IConsole
  {
    public int WindowHeight { get; set; } = 10;
    public int WindowWidth { get; set; } = 10;

    public BlockingCollection<ConsoleKeyInfo> KeyQueue { get; } = new(new ConcurrentQueue<ConsoleKeyInfo>());


    public ConsoleKeyInfo ReadKey(bool intercept)
    {
      return KeyQueue.Take();
    }
  }
}
