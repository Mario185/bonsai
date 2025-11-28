using System;
using consoleTools.VirtualTerminalSequences;

namespace consoleTools.SubWriter
{
  public class CursorWriter : SubWriterBase
  {
    private readonly Action<string> _writeAction;

    internal CursorWriter (ConsoleWriter consoleWriter)
        : base (consoleWriter)
    {
      _writeAction = s => consoleWriter.Write (s);
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.DisableBlinking" />
    /// </summary>
    public CursorWriter DisableBlinking ()
    {
      Writer.Write (CursorSequences.DisableBlinking);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.EnableBlinking" />
    /// </summary>
    public CursorWriter EnableBlinking ()
    {
      Writer.Write (CursorSequences.EnableBlinking);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.Hide" />
    /// </summary>
    public CursorWriter Hide ()
    {
      Writer.Write (CursorSequences.Hide);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveAbsoluteHorizontally(int)" />
    /// </summary>
    public CursorWriter MoveAbsoluteHorizontally (int n)
    {
      CursorSequences.MoveAbsoluteHorizontally (n, _writeAction);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveAbsoluteVertically(int)" />
    /// </summary>
    public CursorWriter MoveAbsoluteVertically (int n)
    {
      CursorSequences.MoveAbsoluteVertically (n, _writeAction);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveDown(int)" />
    /// </summary>
    public CursorWriter MoveDown (int rows = 1)
    {
      CursorSequences.MoveDown (rows, _writeAction);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveLeft(int)" />
    /// </summary>
    public CursorWriter MoveLeft (int columns = 1)
    {
      CursorSequences.MoveLeft (columns, _writeAction);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveRight(int)" />
    /// </summary>
    public CursorWriter MoveRight (int columns = 1)
    {
      CursorSequences.MoveRight (columns, _writeAction);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveTo(int, int)" />
    /// </summary>
    public CursorWriter MoveTo (int x, int y)
    {
      CursorSequences.MoveTo (x, y, _writeAction);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveUp(int)" />
    /// </summary>
    public CursorWriter MoveUp (int rows = 1)
    {
      CursorSequences.MoveUp (rows, _writeAction);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.ResetPosition" />
    /// </summary>
    public CursorWriter ResetPosition ()
    {
      Writer.Write (CursorSequences.ResetPosition);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.RestorePosition" />
    /// </summary>
    public CursorWriter RestorePosition ()
    {
      Writer.Write (CursorSequences.RestorePosition);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.SavePosition" />
    /// </summary>
    public CursorWriter SavePosition ()
    {
      Writer.Write (CursorSequences.SavePosition);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.NextLineBeginning" />
    /// </summary>
    public CursorWriter NextLineBeginning()
    {
      Writer.Write(CursorSequences.NextLineBeginning);
      return this;
    }

    /// <summary>
    ///   Moves the cursor to the beginning of the previous line
    /// </summary>
    public CursorWriter PreviousLineBeginning()
    {
      MoveUp().MoveAbsoluteHorizontally(0);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.SetShape(CursorShape)" />
    /// </summary>
    public CursorWriter SetShape (CursorShape shape)
    {
      CursorSequences.SetShape (shape, _writeAction);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.Show" />
    /// </summary>
    public CursorWriter Show ()
    {
      Writer.Write (CursorSequences.Show);
      return this;
    }
  }
}