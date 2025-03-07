using System;
using consoleTools.VirtualTerminalSequences;

namespace consoleTools
{
  /// <summary>
  ///   An <see cref="IDisposable" /> which activates the alternate screen buffer and
  ///   switches back to the main screen buffer on disposing.
  /// </summary>
  public sealed class AlternateScreenBufferSection : IDisposable
  {
    private readonly ConsoleWriter _consoleWriter = new();

    public AlternateScreenBufferSection()
    {
      _consoleWriter.Write(AlternateScreenBuffer.SwitchToAlternate).Flush();
    }

    public void Dispose()
    {
      _consoleWriter.Write(AlternateScreenBuffer.SwitchToMain).Flush();
    }
  }
}