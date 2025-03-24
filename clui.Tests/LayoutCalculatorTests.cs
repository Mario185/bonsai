using Argon;
using clui.Controls;
using clui.Extensions;
using clui.Layout;

namespace clui.Tests
{

  public abstract class TestBase
  {
    protected VerifySettings VerifySettings { get; }

    protected TestBase()
    {
      VerifySettings = new VerifySettings();
      VerifySettings.AddExtraSettings(s => s.TypeNameHandling = TypeNameHandling.All);
      VerifySettings.DisableDiff();
      VerifySettings.UseDirectory(Path.Combine("verify_snapshots", GetType().Name));
    }
  }

  public class LayoutCalculatorTests : TestBase
  {
    [SetUp]
    public void Setup()
    {
      
    }

    [Test]
    public Task SimpleFraction()
    {
      LayoutCalculator calculator = new();
      
      Panel rootPanel = new(1.AsFraction(), 1.AsFraction());
      rootPanel.Position.X = 1;
      rootPanel.Position.Y = 1;

      calculator.CalculateRootControlLayout(rootPanel, 10, 10);
      VerifySettings.IgnoreMembers<ControlBase>(p => p.RootControl);
      return Verify(rootPanel, GetSettingsWithIgnoredMembers());
    }

    [Test]
    public Task SimpleFixedSize()
    {
      LayoutCalculator calculator = new();

      Panel rootPanel = new(5.AsFixed(), 5.AsFixed());
      rootPanel.Position.X = 1;
      rootPanel.Position.Y = 1;

      calculator.CalculateRootControlLayout(rootPanel, 10, 10);

      return Verify(rootPanel, GetSettingsWithIgnoredMembers());
    }

    [Test]
    public Task InvisibleControlDoesNotGetCalculated()
    {
      LayoutCalculator calculator = new();

      Panel rootPanel = new(5.AsFixed(), 5.AsFixed());
      rootPanel.Position.X = 1;
      rootPanel.Position.Y = 1;
      rootPanel.Visible = false;

      calculator.CalculateRootControlLayout(rootPanel, 10, 10);

      return Verify(rootPanel, GetSettingsWithIgnoredMembers());
    }

    [Test]
    [TestCase(ChildControlFlow.Vertical)]
    [TestCase(ChildControlFlow.Horizontal)]
    public Task ComplexLayout(ChildControlFlow rootFlow)
    {
      LayoutCalculator calculator = new();

      var rootPanel = new Panel(1.AsFraction(), 1.AsFraction());
      rootPanel.Flow = rootFlow;

      var row1 = new Panel(1.AsFraction(), 1.AsFraction());
      var row2 = new Panel(1.AsFraction(), 5.AsFixed());
      var row3 = new Panel(1.AsFraction(), 1.AsFraction());
      var row4 = new Panel(1.AsFixed(), 1.AsFixed());
      row4.Visible = false;
      rootPanel.AddControls(row1, row2, row3, row4);

      AddColumns(row1, 1.AsFraction(), 1.AsFraction());
      AddColumns(row2, 10.AsFixed(), 10.AsFixed());
      AddColumns(row3, 1.AsFraction(), 10.AsFixed());

      calculator.CalculateRootControlLayout(rootPanel, 100, 100);

      return Verify(rootPanel, GetSettingsWithIgnoredMembers());
    }

    private void AddColumns(Panel panel, LayoutSize width, LayoutSize height)
    {
      panel.Flow = ChildControlFlow.Horizontal;
      for (int i = 0; i < 3; i++)
      {
        var temp = new Panel(width, height);
        if (i == 1)
          temp.Visible = false;

        panel.AddControls(temp);
      }
    }

    private VerifySettings GetSettingsWithIgnoredMembers()
    {
      var newSettings = new VerifySettings(VerifySettings);
      newSettings.IgnoreMembers<ControlBase>(p => p.RootControl);
     
      return newSettings;
    }
  }
}
