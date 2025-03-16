using System;
using System.Text.RegularExpressions;

namespace bonsai.Utilities
{
  internal record SearchMatch
  {
    public SearchMatch (int matchStartAt, int matchLength)
    {
      MatchStartAt = matchStartAt;
      MatchLength = matchLength;
      MatchEndAt = matchStartAt + matchLength;
    }

    public int MatchStartAt { get; }
    public int MatchLength { get; }
    public int MatchEndAt { get; }
  }

  public interface ISearchableItem
  {
    string SearchableText { get; }
  }

  internal class SearchFilterProvider<TItem>
      where TItem : ISearchableItem
  {
    private bool _wasUsingRegex;
    private string _currentSearchTerm = string.Empty;

    public bool IsFilterActive => !string.IsNullOrWhiteSpace (_currentSearchTerm);

    public (bool filterChanged, Func<TItem?, SearchMatch[]>? filter, bool canApplyFilterToFilteredList) GetFilter (string searchTerm, bool useRegex)
    {
      string trimmed = searchTerm.Trim();
      bool searchTermChanged = !string.Equals (trimmed, _currentSearchTerm, StringComparison.OrdinalIgnoreCase) || useRegex != _wasUsingRegex;

      if (!searchTermChanged)
      {
        return (false, null, false);
      }

      bool wasFilterActive = IsFilterActive;
      bool applyFilterToAlreadyFilteredList =
          wasFilterActive && trimmed.StartsWith (_currentSearchTerm, StringComparison.OrdinalIgnoreCase) && !useRegex;
      _currentSearchTerm = trimmed;
      _wasUsingRegex = useRegex;

      if (string.IsNullOrWhiteSpace (trimmed))
      {
        return (wasFilterActive && searchTermChanged, null, false);
      }

      Func<TItem?, SearchMatch[]> filter;
      if (useRegex)
      {
        try
        {
          Regex regex = new(_currentSearchTerm, RegexOptions.Compiled | RegexOptions.IgnoreCase);
          // ReSharper disable once ConvertToLocalFunction
          filter = fileSystemItem =>
          {
            if (fileSystemItem == null)
            {
              return [];
            }

            string displayName = fileSystemItem.SearchableText;
            Match match = regex.Match (displayName);
            if (!match.Success)
            {
              return [];
            }

            return [new SearchMatch (match.Index, match.Length)];
          };
        }
        catch (RegexParseException)
        {
          return (false, null, false);
        }
      }
      else
      {
        string[] parts = _currentSearchTerm.Split (
            ' ',
            StringSplitOptions.RemoveEmptyEntries); // GetDistinctSearchTerms(_currentSearchTerm).ToArray();
        filter = fileSystemItem =>
        {
          if (fileSystemItem == null)
          {
            return [];
          }

          string displayName = fileSystemItem.SearchableText;
          int lastIndex = 0;
          SearchMatch[] matches = new SearchMatch[parts.Length];
          for (int i = 0; i < parts.Length; i++)
          {
            string part = parts[i];
            int currentIndex = displayName.IndexOf (part, lastIndex, StringComparison.OrdinalIgnoreCase);

            if (currentIndex < lastIndex || currentIndex < 0)
            {
              return [];
            }

            matches[i] = new SearchMatch (currentIndex, part.Length);
            lastIndex = currentIndex + 1;
          }

          return matches;
        };
      }

      return (true, filter, applyFilterToAlreadyFilteredList);
    }
  }
}