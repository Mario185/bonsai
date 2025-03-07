using System.Collections.Generic;
using System.Drawing;

namespace bonsai.Theme
{
  public class FileColorMapping : ColorMapping
  {
    public Dictionary<string, Color?> Extensions { get; set; } = new();
  }
}