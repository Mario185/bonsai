using System;
using System.Drawing;
using System.IO;
using bonsai.Theme;

namespace bonsai.Explorer
{
  internal class DriveItem : FileSystemItem
  {
    private readonly DriveInfo _driveInfo;

    public DriveItem(DriveInfo driveInfo, int currentDirectoryFullNameLength, Func<bool> isFilterActiveFunc)
      : base(currentDirectoryFullNameLength, isFilterActiveFunc)
    {
      _driveInfo = driveInfo;
    }

    public override string FullName => _driveInfo.RootDirectory.FullName;

    protected override string GetIcon()
    {
      return ""; //ThemeManger.Instance.GetFolderIcon(_driveInfo.Name);
    }

    protected override Color? GetTextColor()
    {
      return ThemeManger.Instance.FolderColors.DefaultColor;
    }

    public override string GetDisplayName()
    {
      return $"{_driveInfo.Name} ({_driveInfo.VolumeLabel})";
    }

    public DriveInfo Info => _driveInfo;
  }
}