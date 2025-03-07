namespace clui.Controls.Interfaces
{
  public interface ICanHaveFocus
  {
    void OnGotFocus();
    void OnLostFocus();
    bool HasFocus { get; }
  }
}