using System;

namespace bonsai.Utilities
{
  internal class KnownPaths
  {
#if DEBUG
    private static readonly AbsolutePath s_rootPath = AppContext.BaseDirectory;
#else
    private static readonly AbsolutePath s_rootPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
#endif

    internal static readonly AbsolutePath BonsaiConfigFolder = s_rootPath / ".bonsai";
    internal static readonly AbsolutePath ThemesFolder = BonsaiConfigFolder / "themes";
    internal static readonly AbsolutePath LogFolder = BonsaiConfigFolder / "logs";
  }
}