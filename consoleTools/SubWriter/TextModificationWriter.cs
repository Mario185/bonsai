using consoleTools.VirtualTerminalSequences;

namespace consoleTools.SubWriter
{
  public class TextModificationWriter : SubWriterBase
  {
    internal TextModificationWriter(ConsoleWriter consoleWriter)
      : base(consoleWriter)
    {
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.DeleteCharacter(int)" />
    /// </summary>
    public TextModificationWriter DeleteCharacter(int number = 1)
    {
      Writer.Write(TextModificationSequences.DeleteCharacter(number));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.DeleteLines(int)" />
    /// </summary>
    public TextModificationWriter DeleteLines(int number = 1)
    {
      Writer.Write(TextModificationSequences.DeleteLines(number));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.EraseCharacter(int)" />
    /// </summary>
    public TextModificationWriter EraseCharacter(int number = 1)
    {
      Writer.Write(TextModificationSequences.EraseCharacter(number));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.EraseInDisplay(EraseType)" />
    /// </summary>
    public TextModificationWriter EraseInDisplay(EraseType eraseType)
    {
      Writer.Write(TextModificationSequences.EraseInDisplay(eraseType));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.EraseInLine(EraseType)" />
    /// </summary>
    public TextModificationWriter EraseInLine(EraseType eraseType)
    {
      Writer.Write(TextModificationSequences.EraseInLine(eraseType));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.InsertCharacter(int)" />
    /// </summary>
    public TextModificationWriter InsertCharacter(int number = 1)
    {
      Writer.Write(TextModificationSequences.InsertCharacter(number));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.InsertLine(int)" />
    /// </summary>
    public TextModificationWriter InsertLine(int number = 1)
    {
      Writer.Write(TextModificationSequences.InsertLine(number));
      return this;
    }
  }
}