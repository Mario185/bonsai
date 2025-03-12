using System;
using clui.Controls.Interfaces;
using clui.Layout;
using consoleTools;

namespace clui.Controls
{
  public class TextBox : ControlBase, ICanHandleInput, IHaveVisibleCursor, ICanHaveFocus
  {
    public event TextChangedHandler? OnTextChanged;

    public delegate void TextChangedHandler (object sender, string text);

    private int _currentVisibleFromIndex;
    private int _visibleTextLength;
    private string _text = string.Empty;

    public TextBox(LayoutSize width, LayoutSize height)
    : base(width, height)
    {
      
    }
 
    public string Text
    {
      get => _text;
      private set
      {
        bool changed = _text != value;
        _text = value;
        if (changed)
          OnTextChanged?.Invoke (this, _text);
      }
    }

    public int CursorPosition { get; private set; }

    public bool HandleInput (ConsoleKeyInfo key)
    {
      if (!HasFocus || !ShouldRenderControl())
        return false;

      bool keyHandled = false;
      if (key.Modifiers == ConsoleModifiers.None)
      {
        switch (key.Key)
        {
          case ConsoleKey.LeftArrow:
            CursorPosition--;
            keyHandled = true;
            break;

          case ConsoleKey.RightArrow:
            CursorPosition++;
            keyHandled = true;
            break;

          case ConsoleKey.Home:
            CursorPosition = 0;
            keyHandled = true;
            break;

          case ConsoleKey.End:
            CursorPosition = Text.Length;
            keyHandled = true;
            break;

          case ConsoleKey.Backspace:
            if (CursorPosition > 0)
            {
              Text = Text.Remove (CursorPosition - 1, 1);
              CursorPosition--;
            }

            keyHandled = true;
            break;

          case ConsoleKey.Delete:
            if (CursorPosition < Text.Length)
              Text = Text.Remove (CursorPosition, 1);
            keyHandled = true;
            break;
        }
      }

      if (!keyHandled && key.Modifiers != ConsoleModifiers.Control && !char.IsControl (key.KeyChar))
      {
        Text = Text.Insert (Math.Clamp (CursorPosition, 0, Text.Length), key.KeyChar.ToString());
        CursorPosition++;
        keyHandled = true;
      }

      if (!keyHandled)
        return false;

      CursorPosition = Math.Clamp (CursorPosition, 0, Text.Length);

      UpdateCurrentVisibleIndex();

      RootControl.AssociatedFrame.RenderPartial (this);
      return true;
    }

    private readonly Position _cursorPosition = new Position(0, 0);
    public Position GetCursorPosition ()
    {
      _cursorPosition.X = Position.X + CursorPosition - _currentVisibleFromIndex;
      _cursorPosition.Y = Position.Y;
      return _cursorPosition;
      //return new Position (Position.X + CursorPosition - _currentVisibleFromIndex, Position.Y);
    }

    public void SetText (string text)
    {
      Text = text;
      CursorPosition = Text.Length;
      UpdateCurrentVisibleIndex();

      if (CalculatedWidth != null && CalculatedHeight != null)
        RootControl.AssociatedFrame.RenderPartial (this);
    }

    public override void OnLayoutCalculated ()
    {
      base.OnLayoutCalculated();
      _visibleTextLength = CalculatedWidth!.Value! - 2;
      UpdateCurrentVisibleIndex();
    }

    internal override void Render (ConsoleWriter consoleWriter)
    {
      consoleWriter.Style.ForegroundColor (GetEffectiveTextColor()).BackgroundColor (GetEffectiveBackgroundColor());

      consoleWriter.Cursor.MoveTo (Position.X, Position.Y)
          .Text.EraseCharacter (CalculatedWidth!.Value)
          .Writer.WriteTruncated (Text, _currentVisibleFromIndex, Math.Min (Text.Length - _currentVisibleFromIndex, _visibleTextLength));

      consoleWriter.Style.ResetStyles()
          //.Cursor.MoveTo(Position.X + CursorPosition - _currentVisibleFromIndex, Position.Y)
          .Writer.Flush();
    }

    private void UpdateCurrentVisibleIndex ()
    {
      int currentTo = Math.Min (Text.Length, _currentVisibleFromIndex + _visibleTextLength);
      int newVisibleFrom = _currentVisibleFromIndex;

      // move left
      if (CursorPosition < _currentVisibleFromIndex)
        newVisibleFrom = CursorPosition;

      if (CursorPosition > _visibleTextLength && CursorPosition > currentTo)
        newVisibleFrom = CursorPosition - _visibleTextLength;

      // move right
      if (CursorPosition >= Text.Length)
        newVisibleFrom = Text.Length - _visibleTextLength;

      _currentVisibleFromIndex = Math.Max (0, newVisibleFrom);
    }

    public void OnGotFocus()
    {
      HasFocus = true;
    }

    public void OnLostFocus()
    {
      HasFocus = false;
    }

    public bool HasFocus { get; private set; }
  }
}