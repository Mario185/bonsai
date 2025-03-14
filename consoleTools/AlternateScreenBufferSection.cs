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
    private static bool s_isAlternateScreenBufferActive = false;
    private readonly ConsoleWriter _consoleWriter = new();

    private readonly bool _instanceActivatedAlternateScreenBuffer;
    public AlternateScreenBufferSection()
    {
      if (s_isAlternateScreenBufferActive)
        return;

      s_isAlternateScreenBufferActive = true;
      _instanceActivatedAlternateScreenBuffer = true;
      _consoleWriter.Write(AlternateScreenBuffer.SwitchToAlternate).Flush();
    }

    public void Dispose()
    {
      if (_instanceActivatedAlternateScreenBuffer)
        _consoleWriter.Write(AlternateScreenBuffer.SwitchToMain).Flush();

      _consoleWriter.Dispose();
    }
  }
}