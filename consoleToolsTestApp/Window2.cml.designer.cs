using global::clui.New;

namespace consoleToolsTestApp
{
  public partial class Window2: global::clui.New.Window
  {
    public global::clui.New.Border Border1 { get; } = new();
    public global::clui.New.Label Label1 { get; } = new();
    public global::clui.New.Panel Panel1 { get; } = new();
    public global::clui.New.Border Border2 { get; } = new();
    public global::clui.New.TextBox TextBox1 { get; } = new();

    protected void Initialize()
    {
      global::clui.New.Panel panel_1 = new();

      this.Controls.Add(this.Border1);
      this.Controls.Add(this.Label1);
      this.Controls.Add(this.Panel1);
      this.Controls.Add(this.TextBox1);

      this.Panel1.Controls.Add(panel_1);
      panel_1.Controls.Add(this.Border2);

    }
  }
}