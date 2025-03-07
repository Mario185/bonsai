using System;
using System.Drawing;
using bonsai.Theme;

namespace bonsai.Explorer
{
  internal class ParentDirectoryItem : FileSystemItem
  {
    public ParentDirectoryItem(int currentDirectoryFullNameLength, Func<bool> isFilterActiveFunc)
      : base(currentDirectoryFullNameLength, isFilterActiveFunc, "..")
    {
    }

    public override string FullName { get; } = "..";
    protected override string GetIcon()
    {
      return ThemeManger.Instance.GetFolderIcon("..");
    }

    protected override Color? GetTextColor()
    {
      return ThemeManger.Instance.GetFolderColor("..");
    }
  }
}