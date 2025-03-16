using System;
using System.Drawing;
using System.IO;
using bonsai.Theme;

namespace bonsai.FileSystemHandling
{
  internal class DriveItem : FileSystemItem
  {
    public DriveItem (DriveInfo driveInfo, int currentDirectoryFullNameLength)
        : base (currentDirectoryFullNameLength)
    {
      Info = driveInfo;
    }

    public override string FullName => Info.RootDirectory.FullName;

    public DriveInfo Info { get; }

    protected override string GetIcon ()
    {
      return ""; //ThemeManger.Instance.GetFolderIcon(_driveInfo.Name);
    }

    protected override Color? GetTextColor ()
    {
      return ThemeManger.Instance.FolderColors.DefaultColor;
    }

    public override string GetDisplayName ()
    {
      return $"{Info.Name} ({Info.VolumeLabel})";
    }
  }
}