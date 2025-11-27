using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using bonsai.FileSystemHandling;
using bonsai.Theme;
using clui;
using clui.Controls;
using clui.Extensions;

namespace bonsai.Apps
{
  internal class ExplorerAppUiBuilder : Frame
  {
    private Border _detailsBorder = null!;
    private Border _filePreviewBorder = null!;
    private Label _regexLabel = null!;
    private Panel _rootPanel = null!;

    public TextBox SearchTextBox { get; private set; } = null!;

    public Border FileSystemListBorder { get; private set; } = null!;

    public ScrollableList<FileSystemItem> FileSystemList { get; private set; } = null!;

    public Label CurrentDirectoryLabel { get; private set; } = null!;

    public MultiLIneLabel DetailsLabel { get; private set; } = null!;
    public MultiLIneLabel FilePreviewLabel { get; private set; } = null!;

    public Panel GitPanel { get; private set; } = null!;

    public void CreateUi ()
    {
      _rootPanel = new Panel (1.AsFraction(), 1.AsFraction());
      AddControls (_rootPanel);

      _rootPanel.BackgroundColor = ThemeManger.Instance.BackgroundColor;

      Panel appPanel = new(1.AsFraction(), 1.AsFraction());

      Panel topPanel = CreateTopPanel();
      Panel centerPanel = CreateCenterPanel();
      Panel searchPanel = CreateSearchPanel();

      Panel settingsPanel = CreateSettingsPanel();

      appPanel.AddControls (topPanel, searchPanel, centerPanel);

      //var instructions = CreateInstructionsPanel();

      _rootPanel.AddControls (appPanel, settingsPanel); //, instructions);
    }

    [SuppressMessage ("CodeQuality", "IDE0051:Remove unused private members", Justification = "CreateInstructionsPanel is for later use")]
    // ReSharper disable once UnusedMember.Local
    private static Panel CreateInstructionsPanel ()
    {
      Panel panel = new(1.AsFraction(), 1.AsFixed())
                    {
                        BackgroundColor = ThemeManger.Instance.TopBarBackgroundColor,
                        TextColor = ThemeManger.Instance.TopBarTextColor,

                        Flow = ChildControlFlow.Horizontal
                    };
      Label instructions = new(30.AsFixed(), 1.AsFixed())
                           {
                               Text = "Hier könnte ihre Anleitung stehen :D"
                           };

      Label shortCutLabel = new(2.AsFixed(), 1.AsFixed())
                            {
                                Text = "^I",
                                BackgroundColor = Color.FromArgb (63, 119, 185)
                            };

      Label includeSubDirectoriesInSearchLabel = new(25.AsFixed(), 1.AsFixed())
                                                 {
                                                     Text = " Include sub directories",
                                                     TextColor = Color.DarkGray
                                                 };

      Label excludeSubDirectoriesInSearchLabel = new(25.AsFixed(), 1.AsFixed())
                                                 {
                                                     Text = " Exclude sub directories",
                                                     TextColor = Color.DarkGray,
                                                     Visible = true
                                                 };

      panel.AddControls (instructions, shortCutLabel, includeSubDirectoriesInSearchLabel, excludeSubDirectoriesInSearchLabel);
      return panel;
    }

    private Panel CreateTopPanel ()
    {
      Panel topPanel = new(1.AsFraction(), 1.AsFixed())
                       {
                           Flow = ChildControlFlow.Horizontal
                       };

      var leftBorder = new Label(1.AsFixed(), 1.AsFixed())
      {
        TextColor = ThemeManger.Instance.TopBarBackgroundColor,
      };
      leftBorder.Text = "\ue0b6";

      var rightBorder = new Label(1.AsFixed(), 1.AsFixed())
      {
        TextColor = ThemeManger.Instance.TopBarBackgroundColor,
      };
      rightBorder.Text = "\ue0b4";

      CurrentDirectoryLabel = new Label (1.AsFraction(), 1.AsFixed())
                              {
                                  TruncateLeft = true,
                                  TextColor = ThemeManger.Instance.TopBarTextColor,
                                  BackgroundColor = ThemeManger.Instance.TopBarBackgroundColor
                              };

      topPanel.AddControls (leftBorder, CurrentDirectoryLabel, rightBorder);
      return topPanel;
    }

    private Panel CreateCenterPanel ()
    {
      Panel centerPanel = new(1.AsFraction(), 1.AsFraction())
                          {
                              Flow = ChildControlFlow.Horizontal
                          };

      FileSystemListBorder = new Border (5.AsFraction(), 1.AsFraction())
                             {
                                 TextPosition = BorderTextPosition.BottomLeft,
                                 BorderColor = ThemeManger.Instance.BorderColor
                             };

      FileSystemList = new ScrollableList<FileSystemItem> (
          ThemeManger.Instance.SelectionForegroundColor,
          ThemeManger.Instance.SelectionBackgroundColor,
          1.AsFraction(),
          1.AsFraction());
      FileSystemListBorder.AddControls (FileSystemList);

      _detailsBorder = new Border (2.AsFraction(), 1.AsFraction())
                       {
                           BorderColor = ThemeManger.Instance.BorderColor,
                           Visible = false
                          
                       };

      DetailsLabel = new MultiLIneLabel (1.AsFraction(), 1.AsFraction());


      _filePreviewBorder = new Border(5.AsFraction(), 1.AsFraction())
                        {
                          BorderColor = ThemeManger.Instance.BorderColor,
                          Visible = false
      };

      FilePreviewLabel = new MultiLIneLabel(1.AsFraction(), 1.AsFraction());
      FilePreviewLabel.DisableTruncating = true;
      FilePreviewLabel.Lines = [];
      _filePreviewBorder.AddControls(FilePreviewLabel);


      GitPanel = new Panel (1.AsFraction(), 1.AsFraction());
      Border seperator = new(
          1.AsFraction(),
          1.AsFixed(),
          bottomFiller: null,
          bottomLeftCorner: null,
          bottomRightCorner: null,
          topLeftCorner: '─',
          topRightCorner: '─');
      MultiLIneLabel multilineLabel2 = new(1.AsFraction(), 1.AsFraction())
                                       {
                                           StickToBottom = true,
                                           Lines =
                                           [
                                               new FormattedLine ("Hier noch GIT infos?"), new FormattedLine ("ABC"), new FormattedLine ("DEF"),
                                               new FormattedLine ("GHI"), new FormattedLine ("JKL"), new FormattedLine ("MNO"),
                                               new FormattedLine ("PQR")
                                           ]
                                       };

      GitPanel.AddControls (seperator, multilineLabel2);
      _detailsBorder.AddControls (DetailsLabel, GitPanel);

      centerPanel.AddControls (FileSystemListBorder, _detailsBorder, _filePreviewBorder);
      return centerPanel;
    }

    private Panel CreateSearchPanel ()
    {
      Panel searchPanel = new(1.AsFraction(), 1.AsFixed())
                          {
                              Flow = ChildControlFlow.Horizontal
                          };

      Label label = new(10.AsFixed(), 1.AsFixed())
                    {
                        Text = " Search ❯",
                        TextColor = ThemeManger.Instance.SearchLabelTextColor,
                        BackgroundColor = ThemeManger.Instance.SearchLabelBackgroundColor
                    };

      SearchTextBox = new TextBox (1.AsFraction(), 1.AsFixed());

      string text = Settings.Instance.GetInstructionForAction (KeyBindingContext.ExplorerApp, ActionType.ToggleRegexSearch, "regex");
      _regexLabel = new Label (text.Length.AsFixed(), 1.AsFixed())
                    {
                        TextColor = ThemeManger.Instance.OptionDisabledColor,
                        Text = text
                    };

      searchPanel.AddControls (label, SearchTextBox, _regexLabel);

      return searchPanel;
    }

    private static Panel CreateSettingsPanel ()
    {
      Panel settingsPanel = new(1.AsFraction(), 1.AsFraction())
                            {
                                Visible = false
                            };

      Border border = new(1.AsFraction(), 1.AsFraction())
                      {
                          BorderColor = ThemeManger.Instance.BorderColor,
                          Text = "bonsai settings"
                      };
      Label label = new(1.AsFraction(), 1.AsFraction())
                    {
                        Text = "Settings YEAAA"
                    };

      border.AddControls (label);
      settingsPanel.AddControls (border);
      return settingsPanel;
    }

    public void ToggleDetails ()
    {
      _detailsBorder.Visible = !_detailsBorder.Visible;
      if (_filePreviewBorder.Visible && _detailsBorder.Visible)
        _filePreviewBorder.Visible = false;

      RenderPartial (_detailsBorder.Parent!);
    }

    public void ToggleFilePreview()
    {
      _filePreviewBorder.Visible = !_filePreviewBorder.Visible;
      if (_filePreviewBorder.Visible && _detailsBorder.Visible)
        _detailsBorder.Visible = false;

      RenderPartial(_filePreviewBorder.Parent!);
    }

    public bool AreDetailsVisible ()
    {
      return _detailsBorder.Visible;
    }

    public bool IsFilePreviewVisible()
    {
      return _filePreviewBorder.Visible;
    }

    public void SetEnableRegExSearch (bool enabled)
    {
      _regexLabel.TextColor = enabled ? ThemeManger.Instance.OptionEnabledColor : ThemeManger.Instance.OptionDisabledColor;
      RenderPartial (_regexLabel);
    }
  }
}