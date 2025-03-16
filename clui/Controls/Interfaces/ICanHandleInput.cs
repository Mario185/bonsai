using System;

namespace clui.Controls.Interfaces
{
  public interface ICanHandleInput
  {
    bool HandleInput (ConsoleKeyInfo key);
  }
}