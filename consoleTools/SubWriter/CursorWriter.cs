using consoleTools.VirtualTerminalSequences;

namespace consoleTools.SubWriter
{
  public class CursorWriter : SubWriterBase
  {
    internal CursorWriter(ConsoleWriter consoleWriter)
      : base(consoleWriter)
    {
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.DisableBlinking" />
    /// </summary>
    public CursorWriter DisableBlinking()
    {
      Writer.Write(CursorSequences.DisableBlinking);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.EnableBlinking" />
    /// </summary>
    public CursorWriter EnableBlinking()
    {
      Writer.Write(CursorSequences.EnableBlinking);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.Hide" />
    /// </summary>
    public CursorWriter Hide()
    {
      Writer.Write(CursorSequences.Hide);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveAbsoluteHorizontally(int)" />
    /// </summary>
    public CursorWriter MoveAbsoluteHorizontally(int n)
    {
      Writer.Write(CursorSequences.MoveAbsoluteHorizontally(n));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveAbsoluteVertically(int)" />
    /// </summary>
    public CursorWriter MoveAbsoluteVertically(int n)
    {
      Writer.Write(CursorSequences.MoveAbsoluteVertically(n));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveDown(int)" />
    /// </summary>
    public CursorWriter MoveDown(int rows = 1)
    {
      Writer.Write(CursorSequences.MoveDown(rows));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveLeft(int)" />
    /// </summary>
    public CursorWriter MoveLeft(int columns = 1)
    {
      Writer.Write(CursorSequences.MoveLeft(columns));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveRight(int)" />
    /// </summary>
    public CursorWriter MoveRight(int columns = 1)
    {
      Writer.Write(CursorSequences.MoveRight(columns));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveTo(int, int)" />
    /// </summary>
    public CursorWriter MoveTo(int x, int y)
    {
      Writer.Write(CursorSequences.MoveTo(x, y));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.MoveUp(int)" />
    /// </summary>
    public CursorWriter MoveUp(int rows = 1)
    {
      Writer.Write(CursorSequences.MoveUp(rows));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.ResetPosition" />
    /// </summary>
    public CursorWriter ResetPosition()
    {
      Writer.Write(CursorSequences.ResetPosition);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.RestorePosition" />
    /// </summary>
    public CursorWriter RestorePosition()
    {
      Writer.Write(CursorSequences.RestorePosition);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.ReverseIndex" />
    /// </summary>
    public CursorWriter ReverseIndex()
    {
      Writer.Write(CursorSequences.ReverseIndex);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.SavePosition" />
    /// </summary>
    public CursorWriter SavePosition()
    {
      Writer.Write(CursorSequences.SavePosition);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.SetShape(CursorShape)" />
    /// </summary>
    public CursorWriter SetShape(CursorShape shape)
    {
      Writer.Write(CursorSequences.SetShape(shape));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="CursorSequences.Show" />
    /// </summary>
    public CursorWriter Show()
    {
      Writer.Write(CursorSequences.Show);
      return this;
    }
  }
}