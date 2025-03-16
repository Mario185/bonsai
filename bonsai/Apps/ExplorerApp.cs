using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using bonsai.CommandHandling;
using bonsai.FileSystemHandling;
using bonsai.Theme;
using clui.Controls;
using consoleTools;

namespace bonsai.Apps
{
  internal class ExplorerApp : AppBase, IBonsaiContext
  {
    private readonly string _initialDirectory;

    private class State
    {
      public string? SelectedItem { get; set; }
      public int? ListCurrentVisibleFromIndex { get; set; }
      public string? SearchText { get; set; }
    }

    private readonly FileSystemWorker _fileSystemWorker = new();
    private ExplorerAppUiBuilder? _uiBuilder;
    private readonly Stack<State> _state = new();

    private bool _includeSubDirectories;
    private bool _loadingFinished;
    private int _currentSpinnerPosition;
    private bool _regexSearchEnabled;

    public ExplorerApp(string initialDirectory)
    {
      _initialDirectory = initialDirectory;
    }

    public bool IsFilteringActive => _fileSystemWorker.IsFilterActive;

    protected override IBonsaiContext Context => this;

    protected override string? RunInternal()
    {
      using (_uiBuilder = new ExplorerAppUiBuilder())
      {
        _uiBuilder.CreateUi();
        _uiBuilder.EnableBufferSizeChangeWatching();
        DirectoryInfo directoryInfo = new(_initialDirectory);

        _fileSystemWorker.OnFileSystemInfoLoadingStateChanged += FileSystemWorker_OnFileSystemInfoLoadingStateChanged;
        _uiBuilder.FileSystemList.OnSelectionChanged += FileSystemList_OnSelectionChanged;
        _uiBuilder.SearchTextBox.OnTextChanged += SearchTextBox_OnTextChanged;

        _uiBuilder.SetFocus(_uiBuilder.SearchTextBox);

        var currentStateDirectory = directoryInfo;
        List<State> statesToPush = new List<State>();
        while (currentStateDirectory != null)
        {
          statesToPush.Insert(0, new State { SelectedItem = currentStateDirectory.FullName });
          currentStateDirectory = currentStateDirectory.Parent;
        }

        foreach (var s in statesToPush)
          _state.Push(s);

        _fileSystemWorker.ChangeDirectory(directoryInfo, false);
        try
        {
          _uiBuilder.RenderComplete();
          bool endLoop = false;
          while (!endLoop)
          {
            ConsoleKeyInfo key = ConsoleHandler.Read();

            if (_uiBuilder.SearchTextBox.HandleInput(key))
              continue;

            switch (Settings.Instance.GetInputActionType(key, KeyBindingContext.ExplorerApp))
            {
              case ActionType.Exit:
                endLoop = true;
                continue;

              case ActionType.OpenDirectory:
                OpenSelectedDirectory();
                continue;

              case ActionType.OpenParentDirectory:
                OpenParentDirectory();
                continue;

              case ActionType.ConfirmSelection:
                if (_uiBuilder.FileSystemList.FocusedItem == null)
                  break;

                var focusedItem = _uiBuilder.FileSystemList.FocusedItem;
                string? result = null;
                switch (focusedItem)
                {
                  case DirectoryItem:
                  case DriveItem:
                  case FileItem:
                    result = CommandHandler.GetCommandAndShowSelectionUiOnDemand(focusedItem.FullName);
                    break;
                  case ParentDirectoryItem:
                    var parent = _fileSystemWorker.CurrentDirectory?.Parent ?? _fileSystemWorker.CurrentDirectory;
                    if (parent == null)
                      break;

                    result = CommandHandler.GetCommandAndShowSelectionUiOnDemand(parent.FullName);
                    break;
                }

                if (!string.IsNullOrWhiteSpace(result))
                  return result;

                _uiBuilder.RenderComplete();
                continue;

              case ActionType.ListSelectPreviousItem:
                _uiBuilder.FileSystemList.SelectPreviousItem();
                continue;
              case ActionType.ListSelectNextItem:
                _uiBuilder.FileSystemList.SelectNextItem();
                continue;
              case ActionType.ListSelectOnePageUp:
                _uiBuilder.FileSystemList.SelectPageUp();
                continue;
              case ActionType.ListSelectOnePageDown:
                _uiBuilder.FileSystemList.SelectPageDown();
                continue;
              case ActionType.ListSelectFirstItem:
                _uiBuilder.FileSystemList.SelectFirstItem();
                continue;
              case ActionType.ListSelectLastItem:
                _uiBuilder.FileSystemList.SelectLastItem();
                continue;
              case ActionType.ToggleShowDetailsPanel:
                _uiBuilder.ToggleDetails();

                if (_uiBuilder.FileSystemList.FocusedItem != null)
                  UpdateDetails(_uiBuilder.FileSystemList.FocusedItem);
                continue;
              case ActionType.ToggleIncludeSubDirectories:
                if (_fileSystemWorker.CurrentDirectory != null)
                {
                  _includeSubDirectories = !_includeSubDirectories;
                  _fileSystemWorker.ChangeDirectory(_fileSystemWorker.CurrentDirectory, _includeSubDirectories);
                }

                continue;
              case ActionType.ToggleRegexSearch:
                _regexSearchEnabled = !_regexSearchEnabled;
                _uiBuilder.SetEnableRegExSearch(_regexSearchEnabled);
                _fileSystemWorker.ApplyFilter(_uiBuilder.SearchTextBox.Text, _regexSearchEnabled);
                continue;

              case ActionType.None:
              default:
                // do nothing, it is just a fallback
                break;
            }
          }
        }
        finally
        {
          _fileSystemWorker.CancelOperations();
        }

        return null;
      }
    }

    private void SearchTextBox_OnTextChanged(object sender, string text)
    {
      _fileSystemWorker.ApplyFilter(text, _regexSearchEnabled);
    }

    private void UpdateFileListBorderText()
    {
      string spinner = string.Empty;
      if (!_loadingFinished && ThemeManger.Instance.LoadingSpinnerChars != null)
      {
        char currentSpinnerChar = ThemeManger.Instance.LoadingSpinnerChars[_currentSpinnerPosition % ThemeManger.Instance.LoadingSpinnerChars.Length];
        _currentSpinnerPosition++;
        spinner = currentSpinnerChar + " ";
      }

      if (string.IsNullOrWhiteSpace(_uiBuilder.SearchTextBox.Text))
      {
        _uiBuilder.FileSystemListBorder.TextColor = _loadingFinished ? Color.Green : Color.Orange;
        _uiBuilder.FileSystemListBorder.Text = spinner + $"❮ {_fileSystemWorker.Data.Count} ❯";
      }
      else
      {
        _uiBuilder.FileSystemListBorder.TextColor = _loadingFinished ? Color.CornflowerBlue : Color.Orange;
        _uiBuilder.FileSystemListBorder.Text = spinner + $"❮ {_fileSystemWorker.CountFiltered} / {_fileSystemWorker.CountUnfiltered} ❯";
      }

      _uiBuilder.RenderPartial(_uiBuilder.FileSystemListBorder);
    }

    private void OpenParentDirectory()
    {
      string? searchedItem = null;
      if (_state.TryPop(out State? state))
      {
        _uiBuilder.SearchTextBox.SetText(state.SearchText ?? string.Empty);
        searchedItem = state.SelectedItem;
        if (state.ListCurrentVisibleFromIndex.HasValue)
          _uiBuilder.FileSystemList.TrySetCurrentVisibleFromIndex(state.ListCurrentVisibleFromIndex.Value);
      }
      else
        _uiBuilder.SearchTextBox.SetText(string.Empty);

      if (_fileSystemWorker.CurrentDirectory?.Parent != null)
        _fileSystemWorker.ChangeDirectory(_fileSystemWorker.CurrentDirectory.Parent, _includeSubDirectories, searchedItem);
      else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && _fileSystemWorker.CurrentDirectory != null)
        _fileSystemWorker.ChangeDirectory(null, _includeSubDirectories, searchedItem);


    }

    private void OpenSelectedDirectory()
    {
      FileSystemItem? focusedItem = _uiBuilder.FileSystemList.FocusedItem;

      if (focusedItem is ParentDirectoryItem)
      {
        OpenParentDirectory();
        return;
      }

      if (_fileSystemWorker.CurrentDirectory != null && focusedItem is DirectoryItem directoryItem)
      {
        DirectoryInfo newDirectory = new(directoryItem.FullName);
        DirectoryInfo? currentStackDirectory = newDirectory;
        List<DirectoryInfo> directoriesToPushIntoStack = new();
        while (currentStackDirectory.Parent!.FullName != _fileSystemWorker.CurrentDirectory.FullName)
        {
          directoriesToPushIntoStack.Insert(0, currentStackDirectory);
          currentStackDirectory = currentStackDirectory.Parent;
        }

        _state.Push(new State
        {
          SearchText = _uiBuilder.SearchTextBox.Text,
          SelectedItem = directoryItem.FullName,
          ListCurrentVisibleFromIndex = _uiBuilder.FileSystemList.CurrentVisibleFromIndex
        });

        foreach (DirectoryInfo dir in directoriesToPushIntoStack)
          _state.Push(new State { SelectedItem = dir.FullName });

        _uiBuilder.SearchTextBox.SetText(string.Empty);
        _fileSystemWorker.ChangeDirectory(newDirectory, _includeSubDirectories);
      }

      if (focusedItem is DriveItem driveItem)
      {
        _state.Push(new State
        {
          SearchText = _uiBuilder.SearchTextBox.Text,
          SelectedItem = driveItem.FullName,
          ListCurrentVisibleFromIndex = _uiBuilder.FileSystemList.CurrentVisibleFromIndex
        });
        _fileSystemWorker.ChangeDirectory(new DirectoryInfo(driveItem.FullName), _includeSubDirectories);

      }
    }

    private void FileSystemList_OnSelectionChanged(object sender, FileSystemItem selectedItem)
    {
      UpdateDetails(selectedItem);
    }

    private void UpdateDetails(FileSystemItem? selectedItem)
    {
      if (!_uiBuilder.AreDetailsVisible() || selectedItem == null)
        return;

      // we use fixed size array so the label will clear itself on rendering
      _uiBuilder.DetailsLabel.Lines = new FormattedLine[8];

      FileSystemInfo? fileSystemInfo;

      switch (selectedItem)
      {
        case DirectoryItem directoryItem:
          fileSystemInfo = new DirectoryInfo(directoryItem.FullName);
          break;
        case FileItem fileItem:
          fileSystemInfo = new FileInfo(fileItem.FullName);
          break;
        case DriveItem driveItem:
          _uiBuilder.DetailsLabel.Lines[0] = new FormattedLine("Name:", Color.LightSlateGray);
          _uiBuilder.DetailsLabel.Lines[1] = new FormattedLine(driveItem.Info.Name, ThemeManger.Instance.FolderColors.DefaultColor) { Indent = 1 };

          _uiBuilder.DetailsLabel.Lines[2] = new FormattedLine("Space:", Color.LightSlateGray);

          var total = driveItem.Info.TotalSize;
          var used = driveItem.Info.TotalSize - driveItem.Info.AvailableFreeSpace;

          _uiBuilder.DetailsLabel.Lines[3] = new FormattedLine($"{FormatFileSize(used)} / {FormatFileSize(total)} ({(int)((double)used / total * 100)}%)") { Indent = 1 };
          _uiBuilder.DetailsLabel.Lines[4] = new FormattedLine("Format:", Color.LightSlateGray);
          _uiBuilder.DetailsLabel.Lines[5] = new FormattedLine(driveItem.Info.DriveFormat) { Indent = 1 };
          _uiBuilder.DetailsLabel.Lines[6] = new FormattedLine("Type:", Color.LightSlateGray);
          _uiBuilder.DetailsLabel.Lines[7] = new FormattedLine(driveItem.Info.DriveType.ToString()) { Indent = 1 };

          _uiBuilder.RenderPartial(_uiBuilder.DetailsLabel);
          return;
          break;
        case ParentDirectoryItem:
          _uiBuilder.RenderPartial(_uiBuilder.DetailsLabel);
          return;
        default:
          throw new ArgumentOutOfRangeException(nameof(selectedItem));
      }

      _uiBuilder.DetailsLabel.Lines[0] = new FormattedLine("Name:", Color.LightSlateGray);
      _uiBuilder.DetailsLabel.Lines[2] = new FormattedLine("Created at:", Color.LightSlateGray);
      _uiBuilder.DetailsLabel.Lines[3] = new FormattedLine(fileSystemInfo.CreationTime.ToString("O")) { Indent = 1 };
      _uiBuilder.DetailsLabel.Lines[4] = new FormattedLine("Last write at:", Color.LightSlateGray);
      _uiBuilder.DetailsLabel.Lines[5] = new FormattedLine(fileSystemInfo.LastWriteTime.ToString("O")) { Indent = 1 };

      if (fileSystemInfo is DirectoryInfo directoryInfo)
        _uiBuilder.DetailsLabel.Lines[1] = new FormattedLine(directoryInfo.Name, ThemeManger.Instance.FolderColors.DefaultColor) { Indent = 1 };
      else if (fileSystemInfo is FileInfo fileInfo)
      {
        _uiBuilder.DetailsLabel.Lines[1] = new FormattedLine(fileSystemInfo.Name, ThemeManger.Instance.FileColors.DefaultColor) { Indent = 1 };
        _uiBuilder.DetailsLabel.Lines[6] = new FormattedLine("Size: ", Color.LightSlateGray);
        _uiBuilder.DetailsLabel.Lines[7] = new FormattedLine(FormatFileSize(fileInfo.Length)) { Indent = 1 };
      }

      _uiBuilder.RenderPartial(_uiBuilder.DetailsLabel);
    }

    private string FormatFileSize(long bytes)
    {
      int unit = 1024;
      if (bytes < unit)
        return $"{bytes} B ";

      int exp = (int)(Math.Log(bytes) / Math.Log(unit));
      return $"{bytes / Math.Pow(unit, exp):F1} {"KMGTPE"[exp - 1]}B";
    }

    private void FileSystemWorker_OnFileSystemInfoLoadingStateChanged(FileSystemWorker sender, FileSystemLoadingState state, int searchedItemAtIndex)
    {
      switch (state)
      {
        case FileSystemLoadingState.Started:
          _loadingFinished = false;
          _uiBuilder.FileSystemList.SetItemList(sender.Data);

          List<string> pathParts = new List<string>();
          DirectoryInfo? currentDirectory = sender.CurrentDirectory;

          while (currentDirectory != null)
          {
            pathParts.Insert(0, currentDirectory.Name);
            currentDirectory = currentDirectory.Parent;
          }

          if (sender.CurrentDirectory == null)
            _uiBuilder.CurrentDirectoryLabel.Text = "root";
          else
          {
            _uiBuilder.CurrentDirectoryLabel.Text = " " + string.Join(" ❯ ", pathParts);
          }

          var wasVisible = _uiBuilder.GitPanel.Visible;
          //if (sender.CurrentDirectory != null)
          //{
          //  var processInfo = new ProcessStartInfo("git", "rev-parse --is-inside-work-tree");
          //  processInfo.WorkingDirectory = sender.CurrentDirectory.FullName;
          //  processInfo.RedirectStandardOutput = true;
          //  processInfo.RedirectStandardError = true;
          //  processInfo.RedirectStandardInput = true;
          //  var x = Process.Start(processInfo); 
          //  var y = x.StandardOutput.ReadToEnd().Trim();
          //  x.WaitForExit();
          //  // current branch: git branch --show-current
          //  // changes git diff --shortstat
          //  // status with ahead: git status -sb
          //  _uiBuilder.GitPanel.Visible = string.Equals("true", y, StringComparison.OrdinalIgnoreCase);
          //}
          //else
          //{
          _uiBuilder.GitPanel.Visible = false;
          //}

          //if (wasVisible != _uiBuilder.GitPanel.Visible)
          //  _uiBuilder.RenderPartial(_uiBuilder.GitPanel.Parent!);

          break;
        case FileSystemLoadingState.Updated:
        case FileSystemLoadingState.Finished:

          _loadingFinished = state == FileSystemLoadingState.Finished;

          if (searchedItemAtIndex > -1)
            _uiBuilder.FileSystemList.SetFocusedIndex(searchedItemAtIndex);

          if (_uiBuilder.FileSystemList.FocusedItemIndex < 0)
            _uiBuilder.FileSystemList.SetFocusedIndex(0);

          UpdateFileListBorderText();

          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(state), state, null);
      }
    }


  }
}