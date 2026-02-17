using global::clui.New;

namespace consoleToolsTestApp.A.B
{
  public partial class MainWindow: global::clui.New.Window
  {
    public global::clui.New.Border Border1 { get; } = new();
    public global::clui.New.Label Label1 { get; } = new();
    public global::clui.New.Label label_1 { get; } = new();
    public global::clui.New.Panel Panel1 { get; } = new();
    public global::clui.New.Border Border2 { get; } = new();
    public global::clui.New.TextBox TextBox1 { get; } = new();
    public global::clui.New.TextBox TextBox2 { get; } = new();
    public global::clui.New.TextBox TextBox3 { get; } = new();
    public global::clui.New.TextBox TextBox4 { get; } = new();

    protected void Initialize()
    {
      global::clui.New.Panel ctrl_panel_1 = new();

      this.Controls.Add(this.Border1);
      this.Controls.Add(this.Label1);
      this.Controls.Add(this.label_1);
      this.Controls.Add(this.Panel1);
      this.Controls.Add(this.TextBox1);
      this.Controls.Add(this.TextBox2);
      this.Controls.Add(this.TextBox3);
      this.Controls.Add(this.TextBox4);

      this.Panel1.Controls.Add(ctrl_panel_1);
      ctrl_panel_1.Controls.Add(this.Border2);

    }
  }
}