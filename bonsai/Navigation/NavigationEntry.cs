using System;
using clui.Controls;
using consoleTools;

namespace bonsai.Navigation
{
  public record NavigationEntry : IListItem
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
    public void Write (ConsoleWriter writer, int maxLength, bool isFocusedItem)
    {
      writer.WriteTruncated ($"{LastUsed:yyyy-MM-dd HH:mm} {Score, 6} {Path}", 0, maxLength);
    }
  }
}