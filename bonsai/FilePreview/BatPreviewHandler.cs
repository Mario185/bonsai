using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace bonsai.FilePreview
{
  public class BatPreviewHandler
  {
    private static readonly Lazy<bool> s_isAvailable = new(CheckIsAvailable);

    public static bool IsAvailable => s_isAvailable.Value;

    private static bool CheckIsAvailable()
    {
      ProcessStartInfo pi = new("bat", "--version")
      {
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
      };

      using (var process = new Process())
      {
        process.StartInfo = pi;
        try
        {
          process.Start();

          process.BeginOutputReadLine();
          process.BeginErrorReadLine();

          process.WaitForExit();
          return process.ExitCode == 0;
        }
        catch
        {
          return false;
        }
      }
    }

    public List<string> LoadPreview(string filePath, int lineStartPosition, int availableWidth, int availableHeight)
    {
      var outputErr = new StringBuilder();
      ProcessStartInfo pi = new("bat", $"--terminal-width {availableWidth} --line-range {lineStartPosition}:{lineStartPosition + availableHeight + 1} --style numbers --tabs 2 -f  \"{filePath}\"");
      pi.RedirectStandardOutput = true;
      pi.RedirectStandardError = true;
      pi.UseShellExecute = false;
      pi.CreateNoWindow = true;
      pi.StandardOutputEncoding = Encoding.UTF8;

      var lines = new List<string>();
      bool hasErrorWhileReading = false;
      string complete = "";
      using (var process = new Process())
      {
        process.StartInfo = pi;
        // Capture both stdout and stderr
        process.OutputDataReceived += (sender, e) =>
        {
          complete += e.Data + Environment.NewLine;
          if (!hasErrorWhileReading && e.Data != null && e.Data.StartsWith('\u001b'))
          {
            var secondEscape = e.Data.Length > 1 ? e.Data.IndexOf('\u001b', 1) : -1;
            if (secondEscape > -1 && e.Data[secondEscape - 1] != ' ')
            {
              lines.Add(e.Data);
            }

            if (e.Data.Contains("[bat warning]"))
            {
              lines.Clear();
              lines.Add("Can not display file.");
              hasErrorWhileReading = true;
            }
          }
        };
        process.ErrorDataReceived += (sender, e) =>
        {
          if (e.Data != null)
            outputErr.AppendLine(e.Data);
        };

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();
      }
      return lines;
    }
  }
}