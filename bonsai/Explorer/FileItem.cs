using System;
using System.Drawing;
using System.IO;
using bonsai.Theme;

namespace bonsai.Explorer
{
  internal class FileItem : FileSystemItem
  {
    private readonly string _fileName;
    private readonly string _fullName;

    public FileItem(ReadOnlySpan<char> directory, ReadOnlySpan<char> fileName , int currentDirectoryFullNameLength, Func<bool> isFilterActiveFunc, string? displayOverride = null)
      : base(currentDirectoryFullNameLength, isFilterActiveFunc, displayOverride)
    {
      _fullName = Path.Join(directory, fileName);
      _fileName = fileName.ToString();
    }

    public override string FullName => _fullName;

    protected override string GetIcon()
    {
      return ThemeManger.Instance.GetFileIcon(_fileName);
    }

    protected override Color? GetTextColor()
    {
      return ThemeManger.Instance.GetFileColor(_fileName);
    }
  }
}