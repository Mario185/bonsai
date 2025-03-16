using System;
using System.Drawing;
using bonsai.Theme;

namespace bonsai.FileSystemHandling
{
  internal class ParentDirectoryItem : FileSystemItem
  {
    public ParentDirectoryItem (int currentDirectoryFullNameLength)
        : base (currentDirectoryFullNameLength, "..")
    {
    }

    public override string FullName { get; } = "..";

    protected override string GetIcon ()
    {
      return ThemeManger.Instance.GetFolderIcon ("..");
    }

    protected override Color? GetTextColor ()
    {
      return ThemeManger.Instance.GetFolderColor ("..");
    }
  }
}