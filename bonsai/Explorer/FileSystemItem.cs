﻿using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using bonsai.Theme;
using bonsai.Utilities;
using clui.Controls;
using consoleTools;

namespace bonsai.Explorer
{
  internal abstract class FileSystemItem : IListItem, ISearchableItem
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

    private int? _iconLength;

    private string? _displayName;

    private string? _icon;
    private Color? _textColor;

    public void Write(ConsoleWriter writer, int maxLength, bool isFocusedItem)
    {
      if (maxLength <= 1)
        return;

      if (_icon == null)
      {
        string icon = GetIcon();
        if (!_iconLength.HasValue)
          _iconLength = new StringInfo(icon).LengthInTextElements;
        _icon = icon + " ";
      }

      var display = GetDisplayNameInternal();
      int maxLengthForText = Math.Clamp(display.Length, 0, maxLength - _iconLength!.Value - 1);
      Color? textColor = isFocusedItem ? ThemeManger.Instance.SelectionForegroundColor : (_textColor ??= GetTextColor());
      writer.Style.ForegroundColor(textColor).Writer.Write(_icon);

      if (IsFilterActiveFunc() && _searchMatches?.Length > 0)
      {
        for (int i = 0; i < maxLengthForText; i++)
        {
          if (IsIndexInMatch(i))
            writer.Style.ForegroundColor(Color.LightGreen).Underline();
          else
            writer.Style.ForegroundColor(textColor).ResetUnderscore();
          
          writer.Write(display[i]);
        }
      }
      else
        writer.Write(display.Slice(0, maxLengthForText));
    }

    private bool IsIndexInMatch(int index)
    {
      if (_searchMatches == null || _searchMatches.Length == 0)
        return false;

      for (int i = 0; i < _searchMatches.Length; i++)
      {
        var match = _searchMatches[i];
        if (match.MatchStartAt <= index && index < match.MatchEndAt)
          return true;
      }

      return false;
    }

    public virtual string GetDisplayName()
    {
      if (_displayNameOverride != null)
        return _displayNameOverride;

      FullName.AsSpan(0, CurrentDirectoryFullNameLength).Trim();

      string display = FullName.Substring(CurrentDirectoryFullNameLength).Trim(Path.DirectorySeparatorChar);
      return display;
    }

    private ReadOnlySpan<char> GetDisplayNameInternal()
    {
      if (_displayName == null)
        _displayName = GetDisplayName();

      return _displayName.AsSpan();

    }

    public void SetSearchMatches(SearchMatch[] matches)
    {
      _searchMatches = matches;
    }

    public void ClearSearchMatches()
    {
      _searchMatches = null;
    }

    public string SearchableText => GetDisplayName();
  }
}