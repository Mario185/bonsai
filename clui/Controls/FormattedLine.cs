using System;
using System.Drawing;

namespace clui.Controls
{
  public class FormattedLine
  {
    public FormattedLine (string value, Color? textColor = null, Color? backgroundColor = null)
    {
      Value = value;
      TextColor = textColor;
      BackgroundColor = backgroundColor;
    }

    public string Value { get; set; }

    public Color? TextColor { get; set; }

    public Color? BackgroundColor { get; set; }

    public bool Bold { get; set; }

    public bool Underline { get; set; }

    public int Indent { get; set; }
  }
}