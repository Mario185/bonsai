using System;

namespace clui.Controls.Interfaces
{
  public interface ICanHaveFocus
  {
    bool HasFocus { get; }
    void OnGotFocus ();
    void OnLostFocus ();
  }
}