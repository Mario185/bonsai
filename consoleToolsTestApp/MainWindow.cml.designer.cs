using global::clui.New;

namespace consoleToolsTestApp
{
  public partial class MainWindow: global::clui.New.Window
  {
    public global::clui.New.Border Border1 { get; } = new();
    public global::clui.New.Label Label1 { get; } = new();
    public global::clui.New.Panel Panel1 { get; } = new();
    public global::clui.New.Border Border2 { get; } = new();
    public global::clui.New.TextBox TextBox1 { get; } = new();

    protected void Initialize()
    {
      global::clui.New.Panel panel_0 = new();

      this.Controls.Add(Border1);
      this.Controls.Add(Label1);
      this.Controls.Add(Panel1);
      this.Controls.Add(TextBox1);

      Panel1.Controls.Add(panel_0);
      panel_0.Controls.Add(Border2);

    }
  }
}