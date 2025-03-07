namespace consoleTools
{
  public enum EraseType
  {
    /// <summary>
    ///   Erases from the current cursor position (inclusive) to the end of the line/display
    /// </summary>
    EraseFromCursorToEnd = 0,

    /// <summary>
    ///   Erases from the beginning of the line/display up to and including the current cursor position
    /// </summary>
    EraseFromLineBeginningToCursors = 1,

    /// <summary>
    ///   Erases the entire line/display
    /// </summary>
    EraseEntireLine = 2
  }
}