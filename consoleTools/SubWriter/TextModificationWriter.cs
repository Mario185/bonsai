using consoleTools.VirtualTerminalSequences;
using System;

namespace consoleTools.SubWriter
{
  public class TextModificationWriter : SubWriterBase
  {
    private readonly Action<string> _writeAction;
    internal TextModificationWriter(ConsoleWriter consoleWriter)
      : base(consoleWriter)
    {
      _writeAction = s => consoleWriter.Write(s);
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.DeleteCharacter(int)" />
    /// </summary>
    public TextModificationWriter DeleteCharacter(int number = 1)
    {
      TextModificationSequences.DeleteCharacter(number, _writeAction);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.DeleteLines(int)" />
    /// </summary>
    public TextModificationWriter DeleteLines(int number = 1)
    {
      TextModificationSequences.DeleteLines(number, _writeAction);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.EraseCharacter(int)" />
    /// </summary>
    public TextModificationWriter EraseCharacter(int number = 1)
    {
      TextModificationSequences.EraseCharacter(number, _writeAction);
      //Writer.Write(TextModificationSequences.EraseCharacter(number));
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.EraseInDisplay(EraseType)" />
    /// </summary>
    public TextModificationWriter EraseInDisplay(EraseType eraseType)
    {
      TextModificationSequences.EraseInDisplay(eraseType, _writeAction);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.EraseInLine(EraseType)" />
    /// </summary>
    public TextModificationWriter EraseInLine(EraseType eraseType)
    {
      TextModificationSequences.EraseInLine(eraseType, _writeAction);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.InsertCharacter(int)" />
    /// </summary>
    public TextModificationWriter InsertCharacter(int number = 1)
    {
      TextModificationSequences.InsertCharacter(number, _writeAction);
      return this;
    }

    /// <summary>
    ///   <inheritdoc cref="TextModificationSequences.InsertLine(int)" />
    /// </summary>
    public TextModificationWriter InsertLine(int number = 1)
    {
      TextModificationSequences.InsertLine(number, _writeAction);
      return this;
    }
  }
}