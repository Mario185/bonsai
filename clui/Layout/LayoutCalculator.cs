using System;
using System.Linq;
using clui.Controls;

namespace clui.Layout
{
  public class LayoutCalculator
  {
    internal LayoutCalculator ()
    {
    }

    public void CalculateChildrenLayout (ControlBase control)
    {
      switch (control.Flow)
      {
        case ChildControlFlow.Vertical:
          CalculateChildrenVerticalFlow (control);
          break;
        case ChildControlFlow.Horizontal:
          CalculateChildrenHorizontalFlow (control);

          break;
        default:
          throw new ArgumentOutOfRangeException ($"{control.Flow} currently not supported.");
      }
    }

    public void CalculateRootControlLayout (ControlBase rootControl, int windowWidth, int windowHeight)
    {
      CalculateRootControlSize (rootControl, windowWidth, windowHeight);
    }

    private void CalculateChildrenHorizontalFlow (ControlBase container)
    {
      ControlBase[] fixedSizeControls = container.Controls.Where (c => c.Visible && c.Width is FixedSize).ToArray();
      int sumFractions = container.Controls.Where (c => c.Visible && c.Width is FractionSize).Sum (c => c.Width.Value);

      int containerRemaining = container.CalculatedWidth!.Value - (container.Padding.Left + container.Padding.Right);
      int containerInnerHeight = container.CalculatedHeight!.Value - (container.Padding.Top + container.Padding.Bottom);

      foreach (ControlBase control in fixedSizeControls)
      {
        control.CalculatedWidth = Math.Min (containerRemaining, control.Width.Value);
        control.CalculatedHeight = control.Height is FractionSize ? containerInnerHeight : control.Height.Value;
        containerRemaining -= control.CalculatedWidth.Value;
      }

      int containerRemainingForFraction = containerRemaining;

      int positionOffset = 0;
      foreach (ControlBase control in container.Controls)
      {
        if (!control.Visible)
        {
          continue;
        }

        if (!fixedSizeControls.Contains (control))
        {
          if (control.Width is FractionSize)
          {
            control.CalculatedWidth = CalculateForFraction (control.Width.Value, containerRemainingForFraction, containerRemaining, sumFractions);
          }
          else
          {
            control.CalculatedWidth = Math.Min (containerRemaining, control.Width.Value);
          }

          control.CalculatedHeight = control.Height is FractionSize ? containerInnerHeight : control.Height.Value;

          containerRemaining -= control.CalculatedWidth.Value;
        }

        CalculatePositionFlowHorizontal (control, container, positionOffset);
        positionOffset += control.CalculatedWidth!.Value;

        control.OnLayoutCalculated();

        CalculateChildrenLayout (control);
      }
    }

    private void CalculateChildrenVerticalFlow (ControlBase container)
    {
      ControlBase[] fixedSizeControls = container.Controls.Where (c => c.Visible && c.Height is FixedSize).ToArray();
      int sumFractions = container.Controls.Where (c => c.Visible && c.Height is FractionSize).Sum (c => c.Height.Value);

      int containerRemaining = container.CalculatedHeight!.Value - (container.Padding.Top + container.Padding.Bottom);
      int containerInnerWidth = container.CalculatedWidth!.Value - (container.Padding.Left + container.Padding.Right);

      foreach (ControlBase control in fixedSizeControls)
      {
        control.CalculatedWidth = control.Width is FractionSize ? containerInnerWidth : control.Width.Value;
        control.CalculatedHeight = Math.Min (containerRemaining, control.Height.Value);
        containerRemaining -= control.CalculatedHeight.Value;
      }

      int containerRemainingForFraction = containerRemaining;

      int positionOffset = 0;
      foreach (ControlBase control in container.Controls)
      {
        if (!control.Visible)
        {
          continue;
        }

        if (!fixedSizeControls.Contains (control))
        {
          if (control.Height is FractionSize)
          {
            control.CalculatedHeight = CalculateForFraction (control.Height.Value, containerRemainingForFraction, containerRemaining, sumFractions);
          }
          else
          {
            control.CalculatedHeight = Math.Min (containerRemaining, control.Height.Value);
          }

          control.CalculatedWidth = control.Width is FractionSize ? containerInnerWidth : control.Width.Value;

          containerRemaining -= control.CalculatedHeight.Value;
        }

        CalculatePositionFlowVertical (control, container, positionOffset);
        positionOffset += control.CalculatedHeight!.Value;

        control.OnLayoutCalculated();

        CalculateChildrenLayout (control);
      }
    }

    private static int CalculateForFraction (
        int desired,
        int remainingForFraction,
        int totalRemaining,
        int sumFractions)
    {
      double percentage = sumFractions < 0 ? 1.0 : desired / (double)sumFractions;
      int targetValue = (int)((remainingForFraction * percentage) + 0.5);

      int value = Math.Min (totalRemaining, targetValue);
      return value;
    }

    private static void CalculatePositionFlowHorizontal (ControlBase controlToPosition, ControlBase container, int positionOffset)
    {
      controlToPosition.Position.X = container.Position.X + container.Padding.Left + positionOffset;
      controlToPosition.Position.Y = container.Position.Y + container.Padding.Top;

      //return new Position(container.Position.X + container.Padding.Left + positionOffset, container.Position.Y + container.Padding.Top);
    }

    private static void CalculatePositionFlowVertical (ControlBase controlToPosition, ControlBase container, int positionOffset)
    {
      controlToPosition.Position.X = container.Position.X + container.Padding.Left;
      controlToPosition.Position.Y = container.Position.Y + container.Padding.Top + positionOffset;

      // return new Position(container.Position.X + container.Padding.Left, container.Position.Y + container.Padding.Top + positionOffset);
    }

    private void CalculateRootControlSize (ControlBase rootControl, int windowWidth, int windowHeight)
    {
      if (!rootControl.Visible)
      {
        return;
      }

      int maxWidth = windowWidth - (rootControl.Position.X - 1);
      int maxHeight = windowHeight - (rootControl.Position.Y - 1);

      int width = rootControl.Width is FractionSize ? maxWidth : Math.Min (rootControl.Width.Value, maxWidth);
      int height = rootControl.Height is FractionSize ? maxHeight : Math.Min (rootControl.Height.Value, maxHeight);

      rootControl.CalculatedWidth = Math.Max (width, 0);
      rootControl.CalculatedHeight = Math.Max (height, 0);

      rootControl.OnLayoutCalculated();

      switch (rootControl.Flow)
      {
        case ChildControlFlow.Vertical:
          CalculateChildrenVerticalFlow (rootControl);
          break;
        case ChildControlFlow.Horizontal:
          CalculateChildrenHorizontalFlow (rootControl);
          break;
        default:
          throw new ArgumentOutOfRangeException ($"{rootControl.Flow} currently not supported.");
      }
    }
  }
}