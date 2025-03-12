using System;
using bonsai.Utilities;
using clui.Controls;
using consoleTools;

namespace bonsai.Navigation
{
  public record NavigationEntry : IListItem, ISearchableItem
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
      var text = $"{LastUsed:yyyy-MM-dd HH:mm} {Score,6} {Path}";
      writer.WriteTruncated (text, 0, maxLength);
    }

    public string SearchableText => Path;
  }
}