using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using bonsai.Theme;
using clui.Controls;
using consoleTools;

namespace bonsai.Explorer
{
  internal abstract class FileSystemItem : IListItem
  {
    protected int CurrentDirectoryFullNameLength { get; }
    protected Func<bool> IsFilterActiveFunc { get; }

    private readonly string? _displayNameOverride;

    private SearchMatch[]? _searchMatches = [];

    protected FileSystemItem(int currentDirectoryFullNameLength, Func<bool> isFilterActiveFunc, string? displayNameOverride = null)
    {
      CurrentDirectoryFullNameLength = currentDirectoryFullNameLength;
      IsFilterActiveFunc = isFilterActiveFunc;
      _displayNameOverride = displayNameOverride;
    }

    public abstract string FullName { get; }

    protected abstract string GetIcon();

    protected abstract Color? GetTextColor();


    public void Write(ConsoleWriter writer, int maxLength, bool isFocusedItem)
    {
      if (maxLength <= 1)
        return;

      string icon = GetIcon();
      int iconLength = new StringInfo(icon).LengthInTextElements;

      string display = GetDisplayName();

      int maxLengthForText = Math.Clamp(display.Length, 0, maxLength - iconLength - 1);

      Color? textColor = isFocusedItem ? ThemeManger.Instance.SelectionForegroundColor : GetTextColor();

      writer.Style.ForegroundColor(textColor).Writer
        .Write(icon).Write(' ');

      if (IsFilterActiveFunc() && _searchMatches?.Length > 0)
      {
        for (int i = 0; i < maxLengthForText; i++)
        {
          if (_searchMatches.Any(h => h.MatchStartAt <= i && i < h.MatchEndAt))
            writer.Style.ForegroundColor(Color.LightGreen).Underline();
          else
            writer.Style.ForegroundColor(textColor).ResetUnderscore();

          writer.Write(display[i]);
        }
      }
      else
        writer.WriteTruncated(display, 0, maxLengthForText);
    }

    public virtual string GetDisplayName()
    {
      if (_displayNameOverride != null)
        return _displayNameOverride;

      string display = FullName.Substring(CurrentDirectoryFullNameLength).Trim(Path.DirectorySeparatorChar);
      return display;
    }

    public void SetSearchMatches(SearchMatch[] matches)
    {
      _searchMatches = matches;
    }

    public void ClearSearchMatches()
    {
      _searchMatches = null;
    }
  }
}