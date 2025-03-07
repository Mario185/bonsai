using System.Collections.Generic;

namespace bonsai.Theme
{
  public class FileIconMapping : IconMapping
  {
    public Dictionary<string, string?> Extensions { get; set; } = new();
  }
}