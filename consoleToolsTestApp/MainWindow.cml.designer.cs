using global::clui.New;

namespace consoleToolsTestApp
{
  public partial class MainWindow: global::clui.New.Window
  {
    public global::clui.New.Border Border1 { get; } = new();
    public global::clui.New.Label Label1 { get; } = new();
    public global::clui.New.Label label_1 { get; } = new();
    public global::clui.New.Panel Panel1 { get; } = new();
    public global::clui.New.Border Border2 { get; } = new();
    public global::clui.New.TextBox TextBox1 { get; } = new();

    protected void Initialize()
    {
      global::clui.New.Panel panel_1 = new();
      global::clui.New.Label label_1 = new();
      global::clui.New.Border border_1 = new();
      global::clui.New.Label label_2 = new();
      global::clui.New.Border border_2 = new();
      global::clui.New.Label label_3 = new();
      global::clui.New.Border border_3 = new();
      global::clui.New.Label label_4 = new();
      global::clui.New.Border border_4 = new();

      this.Controls.Add(this.Border1);
      this.Controls.Add(this.Label1);
      this.Controls.Add(this.label_1);
      this.Controls.Add(this.Panel1);
      this.Controls.Add(this.TextBox1);
      this.Controls.Add(label_1);
      this.Controls.Add(border_1);
      this.Controls.Add(label_2);
      this.Controls.Add(border_2);
      this.Controls.Add(label_3);
      this.Controls.Add(border_3);
      this.Controls.Add(label_4);
      this.Controls.Add(border_4);

      this.Panel1.Controls.Add(panel_1);
      panel_1.Controls.Add(this.Border2);

    }
  }
}