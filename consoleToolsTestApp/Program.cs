using System.Globalization;
using clui.New;
using consoleTools;
using consoleToolsTestApp;

using (var consoleWriter = new ConsoleWriter())
{
  int headerlen = WriteHeader("Styles", consoleWriter);
  
  consoleWriter.Write("Plain").Cursor.NextLineBeginning();
  
  consoleWriter.Style.Bold().Writer.Write("Bold").Style.ResetStyles().Cursor.NextLineBeginning();
  
  consoleWriter.Style.Dim().Writer.Write("Dim").Style.ResetStyles().Cursor.NextLineBeginning();
  
  consoleWriter.Style.Underline().Writer.Write("Underline").Style.ResetStyles().Cursor.NextLineBeginning();
  
  consoleWriter.Style.Blink().Writer.Write("Blinking").Style.ResetStyles().Cursor.NextLineBeginning();
  
  consoleWriter.Style.Reverse().Writer.Write("Reverse").Style.ResetStyles().Cursor.NextLineBeginning();
  
  consoleWriter.Write("Hidden > ").Style.Hidden().Writer.Write("Hidden").Style.ResetStyles().Writer.Write(" <").Cursor.NextLineBeginning();
  
  WriteEnd(headerlen, consoleWriter);
  
  consoleWriter.Write("1111").Cursor.NextLineBeginning().Writer.Write("2222").Cursor.NextLineBeginning();
  
  consoleWriter.Cursor.PreviousLineBeginning().Writer.Write("aaa").Cursor.NextLineBeginning();

}

var appliation = new CluiApplication();



MainWindow w = new MainWindow();




static int WriteHeader(string header, ConsoleWriter writer)
{
  var output = $"────────── {header} ───────────";
  writer.Write(output).Cursor.NextLineBeginning();
  return output.Length;
}

static void WriteEnd(int length, ConsoleWriter writer)
{
  writer.Write(new string('─', length)).Cursor.NextLineBeginning().NextLineBeginning();
}