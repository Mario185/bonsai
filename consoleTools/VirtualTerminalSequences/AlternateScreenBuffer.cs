﻿using System;
using System.Linq;
using System.Xml;

namespace consoleTools.VirtualTerminalSequences
{
  internal class AlternateScreenBuffer
  {
    /// <summary>
    ///   Switches to a new alternate screen buffer.
    /// </summary>
    public const string SwitchToAlternate = CommonSequences.ESC + "[?1049h";

    /// <summary>
    ///   Switches to the main buffer.
    /// </summary>
    public const string SwitchToMain = CommonSequences.ESC + "[?1049l";

    /// <summary>
    ///   Switches to a new alternate screen buffer.
    ///   And returns an instance of <see cref="AlternateScreenBufferSection" />.
    /// </summary>
    public static AlternateScreenBufferSection ActivateAlternateScreenBuffer()
    {
      return new AlternateScreenBufferSection();
    }
  }
}