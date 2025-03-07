using System;
using System.Drawing;
using System.IO;
using bonsai.Theme;

namespace bonsai.Explorer
{
  internal class DirectoryItem : FileSystemItem
  {
    private readonly DirectoryInfo _directoryInfo;

    public DirectoryItem(DirectoryInfo directoryInfo, int currentDirectoryFullNameLength, Func<bool> isFilterActiveFunc, string? displayOverride = null)
      : base(currentDirectoryFullNameLength, isFilterActiveFunc, displayOverride)
    {
      _directoryInfo = directoryInfo;
    }

    public override string FullName => _directoryInfo.FullName;

    protected override string GetIcon()
    {
      return ThemeManger.Instance.GetFolderIcon(_directoryInfo.Name);
    }

    protected override Color? GetTextColor()
    {
      return ThemeManger.Instance.GetFolderColor(_directoryInfo.Name);
    }
  }
}