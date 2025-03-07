using System;

namespace bonsai.Navigation
{
  public record NavigationEntry
  {
    public NavigationEntry(string path, bool isDirectory)
    {
      Path = path;
      IsDirectory = isDirectory;
    }

    public int Score { get; set; }
    public string Path { get; }
    public bool IsDirectory { get; }
    public DateTime LastUsed { get; set; }
  }
}