using bonsai.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace bonsai.Explorer
{
  internal enum FileSystemLoadingState
  {
    Started,
    Updated,
    Finished
  }

  internal class FileSystemWorker
  {
    public delegate void FileSystemLoadingStateChangedHandler(FileSystemWorker sender, FileSystemLoadingState state, int searchedItemAtIndex);

    public event FileSystemLoadingStateChangedHandler? OnFileSystemInfoLoadingStateChanged;

    private readonly EnumerationOptions _enumerationOptions = new() { IgnoreInaccessible = true, ReturnSpecialDirectories = false, AttributesToSkip = FileAttributes.None };
    private readonly EnumerationOptions _enumerationOptionsWithRecurse = new() { IgnoreInaccessible = true, RecurseSubdirectories = true, ReturnSpecialDirectories = false, AttributesToSkip = FileAttributes.None };
    private readonly List<FileSystemItem> _filteredList = new();
    private readonly List<FileSystemItem> _unfilteredList = new();
    private readonly Lock _filterChangeLock = new();
    private readonly ManualResetEventSlim _waitForDeferredApplyFilter = new(true);
    private readonly ManualResetEventSlim _waitForDirectoryLoadingThreadFinished = new(true);
    private readonly SearchFilterProvider<FileSystemItem> _searchFilterProvider = new();
    private bool _filterChanged;
    private CancellationTokenSource? _applyFilterCancellationTokenSource;
    private CancellationTokenSource? _directoryLoadingCancellationTokenSource;
    private Func<FileSystemItem?, SearchMatch[]>? _searchFilter;
    private long _lastSearchTermChange;

    private long? _lastFileSystemInfoLoadingStateChangedTriggered;

    public DirectoryInfo? CurrentDirectory { get; private set; }
    public int CountUnfiltered => _unfilteredList.Count;
    public int CountFiltered => _filteredList.Count;

    public bool IncludeSubDirectories { get; private set; }

    public bool IsFilterActive => _searchFilterProvider.IsFilterActive;

    public List<FileSystemItem> Data => _searchFilterProvider.IsFilterActive ? _filteredList : _unfilteredList;

    public void ChangeDirectory(DirectoryInfo? directory, bool includeSubDirectories, string? searchedItem = null)
    {
      _directoryLoadingCancellationTokenSource?.Cancel();

      _waitForDirectoryLoadingThreadFinished.Wait();
      _waitForDirectoryLoadingThreadFinished.Reset();

      _directoryLoadingCancellationTokenSource = new CancellationTokenSource();
      CurrentDirectory = directory;
      IncludeSubDirectories = includeSubDirectories;

      Thread thread = new(() => LoadDirectory(CurrentDirectory, includeSubDirectories ? _enumerationOptionsWithRecurse : _enumerationOptions,
        _directoryLoadingCancellationTokenSource.Token, searchedItem));

      thread.Start();
    }

    public void ApplyFilter(string searchTerm, bool useRegex)
    {
      string trimmed = searchTerm.Trim();
      (bool filterChanged, Func<FileSystemItem?, SearchMatch[]>? searchFilter, bool canApplyFilterToFilteredList) = _searchFilterProvider.GetFilter(trimmed, useRegex);

      if (!filterChanged)
        return;

      _lastSearchTermChange = Stopwatch.GetTimestamp();
      _applyFilterCancellationTokenSource?.Cancel();
      _applyFilterCancellationTokenSource = new CancellationTokenSource();
      _waitForDeferredApplyFilter.Wait();

      Thread thread = new(() => ApplyFilterDeferred(_applyFilterCancellationTokenSource.Token, searchFilter, canApplyFilterToFilteredList));
      thread.Start();
    }

    private void ApplyFilterDeferred(CancellationToken token, Func<FileSystemItem?, SearchMatch[]>? newFilter, bool canApplyFilterToFilteredList)
    {
      while (Stopwatch.GetElapsedTime(_lastSearchTermChange).TotalMilliseconds < 200)
      {
        if (token.IsCancellationRequested)
          return;
      }

      try
      {
        _waitForDeferredApplyFilter.Reset();

        if (token.IsCancellationRequested)
          return;

        _searchFilter = newFilter;

        // check if directory loading is finished
        if (_waitForDirectoryLoadingThreadFinished.IsSet)
        {
          if (!_searchFilterProvider.IsFilterActive || _searchFilter == null)
          {
            TriggerOnFileSystemInfoLoadingStateChanged(FileSystemLoadingState.Started, -1);
            TriggerOnFileSystemInfoLoadingStateChanged(FileSystemLoadingState.Finished, -1);
            return;
          }

          List<FileSystemItem> listToFilter = _unfilteredList;
          if (canApplyFilterToFilteredList && _filteredList.Count > 0)
            listToFilter = _filteredList.ToList();

          _filteredList.Clear();

          TriggerOnFileSystemInfoLoadingStateChanged(FileSystemLoadingState.Started, -1);
          for (int index = 0; index < listToFilter.Count; index++)
          {
            FileSystemItem item = listToFilter[index];
            if (token.IsCancellationRequested)
              break;

            ApplyFilterToItemAndAddToList(item, _searchFilter, _filteredList);
            TriggerOnFileSystemInfoLoadingStateChanged(FileSystemLoadingState.Updated, -1);
          }

          TriggerOnFileSystemInfoLoadingStateChanged(FileSystemLoadingState.Finished, -1);
        }
        else
        {
          // directory loading is still in progress, so just notify the loading thread
          // that the filter has changed
          using (_filterChangeLock.EnterScope())
            _filterChanged = true;
        }
      }
      finally
      {
        _waitForDeferredApplyFilter.Set();
      }
    }

    public void CancelOperations()
    {
      _directoryLoadingCancellationTokenSource?.Cancel();
      _applyFilterCancellationTokenSource?.Cancel();
      _waitForDirectoryLoadingThreadFinished.Wait();
      _waitForDeferredApplyFilter.Wait();
    }

    private void LoadDirectory(DirectoryInfo? directoryInfo, EnumerationOptions enumerationOptions, CancellationToken cancellationToken, string? searchedItem)
    {
      try
      {
        _unfilteredList.Clear();
        _filteredList.Clear();

        TriggerOnFileSystemInfoLoadingStateChanged(FileSystemLoadingState.Started, -1);

        int currentDirectoryFullNameLength = CurrentDirectory?.FullName.Length ?? 0;
        Func<bool> isFilterActiveFunc = () => IsFilterActive;

        if (directoryInfo == null)
          CreateAndAddFileSystemItems(DriveInfo.GetDrives(), cancellationToken, searchedItem, info => new DriveItem(info, currentDirectoryFullNameLength, isFilterActiveFunc));
        else
        {
          if (Settings.Instance.ShowParentDirectoryInList)
          {
            if (directoryInfo.Parent != null)
            {
              var parent = new ParentDirectoryItem(currentDirectoryFullNameLength, isFilterActiveFunc);
              _unfilteredList.Add(parent);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
              var parent = new ParentDirectoryItem(currentDirectoryFullNameLength, isFilterActiveFunc);
              _unfilteredList.Add(parent);
            }
          }

          if (!cancellationToken.IsCancellationRequested)
            CreateAndAddFileSystemItems(directoryInfo.EnumerateDirectories("*", enumerationOptions), cancellationToken, searchedItem, info => new DirectoryItem(info, currentDirectoryFullNameLength, isFilterActiveFunc));

          if (!cancellationToken.IsCancellationRequested)
            CreateAndAddFileSystemItems(directoryInfo.EnumerateFiles("*", enumerationOptions), cancellationToken, searchedItem, info => new FileItem(info, currentDirectoryFullNameLength, isFilterActiveFunc));

        }
        TriggerOnFileSystemInfoLoadingStateChanged(FileSystemLoadingState.Finished, -1);
      }
      finally
      {
        _waitForDirectoryLoadingThreadFinished.Set();
      }
    }

    private void CreateAndAddFileSystemItems<T>(IEnumerable<T> source, CancellationToken cancellationToken, string? searchedItem, Func<T, FileSystemItem> itemFactory)
    {
      int searchedItemAtIndex = -1;

      foreach (T fileSystemInfo in source)
      {
        if (cancellationToken.IsCancellationRequested)
          break;

        FileSystemItem listItem = itemFactory(fileSystemInfo);

        _unfilteredList.Add(listItem);

        ApplyFilterToItemAndAddToList(listItem, _searchFilter, _filteredList);

        using (_filterChangeLock.EnterScope())
        {
          if (_filterChanged && _searchFilter != null)
          {
            _filteredList.Clear();

            foreach (var item in _unfilteredList)
            {
              ApplyFilterToItemAndAddToList(item, _searchFilter, _filteredList);
              TriggerOnFileSystemInfoLoadingStateChanged(FileSystemLoadingState.Started, -1);
            }

            _filterChanged = false;
          }
        }

        if (searchedItem != null && listItem.FullName == searchedItem)
          searchedItemAtIndex = _unfilteredList.Count - 1;

        TriggerOnFileSystemInfoLoadingStateChanged(FileSystemLoadingState.Updated, searchedItemAtIndex);
        searchedItemAtIndex = -1;
      }
    }

    private void ApplyFilterToItemAndAddToList(FileSystemItem item, Func<FileSystemItem?, SearchMatch[]>? filter, List<FileSystemItem> targetList)
    {
      item.ClearSearchMatches();
      if (filter == null)
        return;

      var matches = filter(item);

      if (matches.Length == 0)
        return;

      item.SetSearchMatches(matches);

      targetList.Add(item);
    }

    private void TriggerOnFileSystemInfoLoadingStateChanged(FileSystemLoadingState state, int searchedItemAtIndex)
    {
      if (state != FileSystemLoadingState.Updated || !_lastFileSystemInfoLoadingStateChangedTriggered.HasValue ||
          Stopwatch.GetElapsedTime(_lastFileSystemInfoLoadingStateChangedTriggered.Value).TotalMilliseconds > 100 || searchedItemAtIndex > -1)
      {
        OnFileSystemInfoLoadingStateChanged?.Invoke(this, state, searchedItemAtIndex);
        _lastFileSystemInfoLoadingStateChangedTriggered = Stopwatch.GetTimestamp();
      }
    }
  }
}