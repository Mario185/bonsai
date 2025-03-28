using System.Collections.Concurrent;
using consoleTools;

namespace Tests.consoleTools
{
  public class ConsoleHandlerTests
  {
    [Fact]
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

      consoleImplementation.KeyQueue.Add(new ConsoleKeyInfo('A', ConsoleKey.A, false, false, false), TestContext.Current.CancellationToken);
      resetEvent.Wait(TestContext.Current.CancellationToken);

      Assert.True(readValue.HasValue);

      Assert.Equal(ConsoleKey.A, readValue.Value.Key);
      Assert.Equal('A', readValue.Value.KeyChar);
    }

    [Fact]
    public void BufferSizeChange()
    {
      int receivedWidth = 0;
      int receivedHeight = 0;
      ManualResetEventSlim resetEvent = new();

      try
      {


        var consoleImplementation = new TestConsoleImplementation();
        ConsoleHandler.Initialize(consoleImplementation);
        ConsoleHandler.StartOperation();



        ConsoleHandler.RegisterBufferSizeChangeCallback(BufferSizeCallBack);
        consoleImplementation.WindowHeight = 12;
        consoleImplementation.WindowWidth = 13;

        Assert.True(resetEvent.Wait(1000, TestContext.Current.CancellationToken));
        resetEvent.Reset();


        Assert.Equal(12, receivedHeight);
        Assert.Equal(13, receivedWidth);

        ConsoleHandler.UnregisterBufferSizeChangeCallback(BufferSizeCallBack);
        consoleImplementation.WindowHeight = 16;
        consoleImplementation.WindowWidth = 17;
        resetEvent.Wait(100, TestContext.Current.CancellationToken);

        Assert.Equal(12, receivedHeight);
        Assert.Equal(13, receivedWidth);

      }
      finally
      {
        ConsoleHandler.CancelOperation();
      }


      void BufferSizeCallBack(int width, int height)
      {
        receivedHeight = height;
        receivedWidth = width;
        resetEvent.Set();
      }
    }

    [Fact]
    public void StartOperationTwiceThrowsException()
    {
      try
      {
        var consoleImplementation = new TestConsoleImplementation();
        ConsoleHandler.Initialize(consoleImplementation);
        ConsoleHandler.StartOperation();

        Assert.Throws<InvalidOperationException>(ConsoleHandler.StartOperation);
      }
      finally
      {
        ConsoleHandler.CancelOperation();
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
