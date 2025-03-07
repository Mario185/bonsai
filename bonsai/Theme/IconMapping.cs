using System.Collections.Generic;

namespace bonsai.Theme
{
  public class IconMapping
  {
    public string? DefaultIcon { get; set; }

    public string? Symlink { get; set; }

    public string? Junction { get; set; }

    public Dictionary<string, string?> Named { get; set; } = new();
  }
}