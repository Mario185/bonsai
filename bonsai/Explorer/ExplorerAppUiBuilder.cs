using System;
using System.Drawing;
using bonsai.Theme;
using clui;
using clui.Controls;
using clui.Extensions;

namespace bonsai.Explorer
{
  internal class ExplorerAppUiBuilder : Frame
  {
    private Border _detailsBorder = null!;
    private Panel _rootPanel = null!;
    private Label _regexLabel = null!;

    public ExplorerAppUiBuilder()
    : base()
    {
    }

    public TextBox SearchTextBox { get; private set; } = null!;

    public Border FileSystemListBorder { get; private set; } = null!;

    public ScrollableList<FileSystemItem> FileSystemList { get; private set; } = null!;

    public Label CurrentDirectoryLabel { get; private set; } = null!;

    public MultiLIneLabel DetailsLabel { get; private set; } = null!;

    public Panel GitPanel { get; private set; } = null!;



    public void CreateUi()
    {

      _rootPanel = new Panel(1.AsFraction(), 1.AsFraction());
      AddControls(_rootPanel);

      _rootPanel.BackgroundColor = ThemeManger.Instance.BackgroundColor;

      Panel appPanel = new(1.AsFraction(), 1.AsFraction());

      Panel topPanel = CreateTopPanel();
      Panel centerPanel = CreateCenterPanel();
      Panel searchPanel = CreateSearchPanel();

      Panel settingsPanel = CreateSettingsPanel();


      appPanel.AddControls(topPanel, searchPanel, centerPanel);

      //var instructions = CreateInstructionsPanel();

      _rootPanel.AddControls(appPanel, settingsPanel); //, instructions);
    }

    private Panel CreateInstructionsPanel()
    {
      var panel = new Panel(1.AsFraction(), 1.AsFixed());
      panel.BackgroundColor = ThemeManger.Instance.TopBarBackgroundColor;
      panel.TextColor = ThemeManger.Instance.TopBarTextColor;

      panel.Flow = ChildControlFlow.Horizontal;
      Label instructions = new(30.AsFixed(), 1.AsFixed());
      instructions.Text = "Hier könnte ihre Anleitung stehen :D";


      Label shortCutLabel = new(2.AsFixed(), 1.AsFixed());
      shortCutLabel.Text = "^I";
      shortCutLabel.BackgroundColor = Color.FromArgb(63, 119, 185);

      Label includeSubDirectoriesInSearchLabel = new(25.AsFixed(), 1.AsFixed());
      includeSubDirectoriesInSearchLabel.Text = " Include sub directories";
      includeSubDirectoriesInSearchLabel.TextColor = Color.DarkGray;

      Label excludeSubDirectoriesInSearchLabel = new(25.AsFixed(), 1.AsFixed());
      excludeSubDirectoriesInSearchLabel.Text = " Exclude sub directories";
      excludeSubDirectoriesInSearchLabel.TextColor = Color.DarkGray;
      excludeSubDirectoriesInSearchLabel.Visible = true;

      panel.AddControls(instructions, shortCutLabel, includeSubDirectoriesInSearchLabel, excludeSubDirectoriesInSearchLabel);
      return panel;
    }

    private Panel CreateTopPanel()
    {
      Panel topPanel = new(1.AsFraction(), 1.AsFixed());
      topPanel.Flow = ChildControlFlow.Horizontal;
      CurrentDirectoryLabel = new Label(1.AsFraction(), 1.AsFixed());
      CurrentDirectoryLabel.TruncateLeft = true;
      CurrentDirectoryLabel.TextColor = ThemeManger.Instance.TopBarTextColor;
      CurrentDirectoryLabel.BackgroundColor = ThemeManger.Instance.TopBarBackgroundColor;

      topPanel.AddControls(CurrentDirectoryLabel);
      return topPanel;
    }

    private Panel CreateCenterPanel()
    {
      Panel centerPanel = new(1.AsFraction(), 1.AsFraction());
      centerPanel.Flow = ChildControlFlow.Horizontal;

      FileSystemListBorder = new Border(5.AsFraction(), 1.AsFraction());
      FileSystemListBorder.TextPosition = BorderTextPosition.BottomLeft;
      FileSystemListBorder.BorderColor = ThemeManger.Instance.BorderColor;

      FileSystemList = new ScrollableList<FileSystemItem>(ThemeManger.Instance.SelectionForegroundColor, ThemeManger.Instance.SelectionBackgroundColor,
        1.AsFraction(), 1.AsFraction());
      FileSystemListBorder.AddControls(FileSystemList);

      _detailsBorder = new Border(2.AsFraction(), 1.AsFraction());
      _detailsBorder.BorderColor = ThemeManger.Instance.BorderColor;
      _detailsBorder.Visible = false;

      DetailsLabel = new MultiLIneLabel(1.AsFraction(), 1.AsFraction());

      GitPanel = new Panel(1.AsFraction(), 1.AsFraction());
      var seperator = new Border(1.AsFraction(), 1.AsFixed(), bottomFiller: null, bottomLeftCorner: null, bottomRightCorner: null, topLeftCorner: '─', topRightCorner: '─');
      var multilineLabel2 = new MultiLIneLabel(1.AsFraction(), 1.AsFraction());
      multilineLabel2.StickToBottom = true;
      multilineLabel2.Lines =
        [new FormattedLine("Hier noch GIT infos?"), new FormattedLine("ABC"), new FormattedLine("DEF"), new FormattedLine("GHI"), new FormattedLine("JKL"), new FormattedLine("MNO"), new FormattedLine("PQR")];

      GitPanel.AddControls(seperator, multilineLabel2);
      _detailsBorder.AddControls(DetailsLabel, GitPanel);

      centerPanel.AddControls(FileSystemListBorder, _detailsBorder);
      return centerPanel;
    }

    private Panel CreateSearchPanel()
    {
      Panel searchPanel = new(1.AsFraction(), 1.AsFixed());
      searchPanel.Flow = ChildControlFlow.Horizontal;

      Label label = new(10.AsFixed(), 1.AsFixed());
      label.Text = " Search ❯";
      label.TextColor = ThemeManger.Instance.SearchLabelTextColor;
      label.BackgroundColor = ThemeManger.Instance.SearchLabelBackgroundColor;

      SearchTextBox = new TextBox(1.AsFraction(), 1.AsFixed());

      var text = Settings.Instance.GetInstructionForAction(KeyBindingContext.ExplorerApp, ActionType.ToggleRegexSearch,"regex");
      _regexLabel = new(text.Length.AsFixed(), 1.AsFixed());
      _regexLabel.TextColor = ThemeManger.Instance.OptionDisabledColor;
      _regexLabel.Text = text;

      searchPanel.AddControls(label, SearchTextBox, _regexLabel);

      return searchPanel;
    }

    private Panel CreateSettingsPanel()
    {
      Panel settingsPanel = new(1.AsFraction(), 1.AsFraction());
      settingsPanel.Visible = false;

      Border border = new(1.AsFraction(), 1.AsFraction());
      border.BorderColor = ThemeManger.Instance.BorderColor;
      border.Text = "bonsai settings";
      Label label = new(1.AsFraction(), 1.AsFraction());
      label.Text = "Settings YEAAA";

      border.AddControls(label);
      settingsPanel.AddControls(border);
      return settingsPanel;
    }

    public void ToggleDetails()
    {
      _detailsBorder.Visible = !_detailsBorder.Visible;
      RenderPartial(_detailsBorder.Parent!);
    }

    public bool AreDetailsVisible()
    {
      return _detailsBorder.Visible;
    }

    public void SetEnableRegExSearch(bool enabled)
    {
      _regexLabel.TextColor = enabled ? ThemeManger.Instance.OptionEnabledColor : ThemeManger.Instance.OptionDisabledColor;
      RenderPartial(_regexLabel);
    }
  }
}