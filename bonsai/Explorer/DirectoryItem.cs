using System;
using System.Drawing;
using System.IO;
using bonsai.Theme;

namespace bonsai.Explorer
{
  internal class DirectoryItem : FileSystemItem
  {
    private readonly string _fullName;

    private readonly string _name;

    public DirectoryItem(ReadOnlySpan<char> fullName, int currentDirectoryFullNameLength, string? displayOverride = null)
      : base(currentDirectoryFullNameLength, displayOverride)
    {
      _fullName = fullName.ToString();
      _name = Path.GetFileName(Path.TrimEndingDirectorySeparator(fullName)).ToString();
    }

    public override string FullName => _fullName;

    protected override string GetIcon()
    {
      return ThemeManger.Instance.GetFolderIcon(_name);
    }

    protected override Color? GetTextColor()
    {
      return ThemeManger.Instance.GetFolderColor(_name);
    }
  }
}