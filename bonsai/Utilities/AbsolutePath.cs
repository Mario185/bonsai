using System.IO;

namespace bonsai.Utilities
{
  internal class AbsolutePath
  {
    public static implicit operator string(AbsolutePath absolutePath)
    {
      return absolutePath.ToString();
    }

    public static implicit operator AbsolutePath(string path)
    {
      return new AbsolutePath(path);
    }

    public static AbsolutePath operator /(AbsolutePath pathA, AbsolutePath pathB)
    {
      return pathA.Combine(pathB);
    }

    private readonly string _path;

    public AbsolutePath(string path)
    {
      _path = path;
    }

    public AbsolutePath Combine(AbsolutePath path)
    {
      return new AbsolutePath(Path.Combine(this, path));
    }

    public void EnsureDirectoryExists()
    {
      if (!Directory.Exists(this))
      {
        Directory.CreateDirectory(this);
      }
    }

    public override string ToString()
    {
      return _path;
    }
  }
}