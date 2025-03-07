using System.Collections.Generic;
using System.Drawing;

namespace bonsai.Theme
{
  public class ColorMapping
  {
    public Color? DefaultColor { get; set; }

    public Color? Symlink { get; set; }

    public Color? Junction { get; set; }

    public Dictionary<string, Color?> Named { get; set; } = new();
  }
}