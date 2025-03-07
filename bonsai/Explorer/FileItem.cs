using System;
using System.Drawing;
using System.IO;
using bonsai.Theme;

namespace bonsai.Explorer
{
  internal class FileItem : FileSystemItem
  {
    private readonly FileInfo _fileInfo;

    public FileItem(FileInfo fileInfo, int currentDirectoryFullNameLength, Func<bool> isFilterActiveFunc, string? displayOverride = null)
      : base(currentDirectoryFullNameLength, isFilterActiveFunc, displayOverride)
    {
      _fileInfo = fileInfo;
    }

    public override string FullName => _fileInfo.FullName;

    protected override string GetIcon()
    {
      return ThemeManger.Instance.GetFileIcon(_fileInfo.Name);
    }

    protected override Color? GetTextColor()
    {
      return ThemeManger.Instance.GetFileColor(_fileInfo.Name);
    }
  }
}