using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using bonsai.Apps;
using bonsai.Theme;
using bonsai.Utilities;
using clui.Controls;
using consoleTools;

namespace bonsai.FileSystemHandling
{
  internal abstract class FileSystemItem : IListItem, ISearchableItem
  {
    private readonly string? _displayNameOverride;
    private Color? _textColor;

    private int? _iconLength;

    private SearchMatch[]? _searchMatches = [];

    private string? _displayName;

    private string? _icon;

    protected FileSystemItem (int currentDirectoryFullNameLength, string? displayNameOverride = null)
    {
      CurrentDirectoryFullNameLength = currentDirectoryFullNameLength;
      _displayNameOverride = displayNameOverride;
    }

    protected int CurrentDirectoryFullNameLength { get; }

    public abstract string FullName { get; }

    public void Write (ConsoleWriter writer, int maxLength, bool isFocusedItem)
    {
      if (maxLength <= 1)
      {
        return;
      }

      if (_icon == null)
      {
        string icon = GetIcon();
        if (!_iconLength.HasValue)
        {
          _iconLength = new StringInfo (icon).LengthInTextElements;
        }

        _icon = icon + " ";
      }

      ReadOnlySpan<char> display = GetDisplayNameInternal();
      int maxLengthForText = Math.Clamp (display.Length, 0, maxLength - _iconLength!.Value - 1);
      Color? textColor = isFocusedItem ? ThemeManger.Instance.SelectionForegroundColor : _textColor ??= GetTextColor();
      writer.Style.ForegroundColor (textColor).Writer.Write (_icon);

      if (BonsaiContext.Current!.IsFilteringActive && _searchMatches?.Length > 0)
      {
        for (int i = 0; i < maxLengthForText; i++)
        {
          if (IsIndexInMatch (i))
          {
            writer.Style.ForegroundColor (Color.LightGreen).Underline();
          }
          else
          {
            writer.Style.ForegroundColor (textColor).ResetUnderscore();
          }

          writer.Write (display[i]);
        }
      }
      else
      {
        writer.WriteTruncated (display, 0, maxLengthForText);
      }
    }

    public string SearchableText => GetDisplayName();

    protected abstract string GetIcon ();

    protected abstract Color? GetTextColor ();

    private bool IsIndexInMatch (int index)
    {
      if (_searchMatches == null || _searchMatches.Length == 0)
      {
        return false;
      }

      for (int i = 0; i < _searchMatches.Length; i++)
      {
        SearchMatch match = _searchMatches[i];
        if (match.MatchStartAt <= index && index < match.MatchEndAt)
        {
          return true;
        }
      }

      return false;
    }

    public virtual string GetDisplayName ()
    {
      if (_displayNameOverride != null)
      {
        return _displayNameOverride;
      }

      FullName.AsSpan (0, CurrentDirectoryFullNameLength).Trim();

      string display = FullName.Substring (CurrentDirectoryFullNameLength).Trim (Path.DirectorySeparatorChar);
      return display;
    }

    private ReadOnlySpan<char> GetDisplayNameInternal ()
    {
      _displayName ??= GetDisplayName();

      return _displayName.AsSpan();
    }

    public void SetSearchMatches (SearchMatch[] matches)
    {
      _searchMatches = matches;
    }

    public void ClearSearchMatches ()
    {
      _searchMatches = null;
    }
  }
}