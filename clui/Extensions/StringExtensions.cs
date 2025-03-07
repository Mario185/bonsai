using System;

namespace clui.Extensions
{
  public static class StringExtensions
  {
    public static string Truncate (this string value, int length)
    {
      ArgumentNullException.ThrowIfNull (value);

      if (value.Length < length)
        return value;

      return value.Substring (0, length);
    }
  }
}