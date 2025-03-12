using System;
using bonsai.Utilities;
using clui.Controls;
using consoleTools;

namespace bonsai.Navigation
{
  public record NavigationEntry : IListItem, ISearchableItem
  {
    private string? _printableText = null;

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
      writer.WriteTruncated (GetPrintableText(), 0, maxLength);
    }

    private string GetPrintableText()
    {
      if (_printableText == null)
        _printableText = $"{LastUsed:yyyy-MM-dd HH:mm} {Score,6} {Path}";
      return _printableText;
    }

    public string SearchableText => Path;
  }
}