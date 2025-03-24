using System;

namespace consoleTools.VirtualTerminalSequences
{
  internal class CommonSequences : SequenceBase
  {
    /// <summary>
    ///   Escape character to notify the terminal it should treat the value as Virtual Terminal Sequence command
    /// </summary>
    public const string ESC = "\x1b";
  }
}