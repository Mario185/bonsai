﻿using System;
using System.IO;
using System.Threading;
using bonsai.Utilities;

namespace bonsai.Logging
{
  public class Logger
  {
    private const LogSeverity c_minimumSeverity = LogSeverity.Warning;

    private static DateTime s_currentLogFileDate = DateTime.MinValue;
    private static AbsolutePath? s_currentLogFilePath;
    private static readonly Lock s_lock = new();
    private readonly Type _loggerType;

    protected Logger (Type loggerType)
    {
      _loggerType = loggerType;
    }

    public static Logger Create<TLoggerType> ()
    {
      UpdateLogFileNameOnDemand();
      return new Logger (typeof(TLoggerType));
    }

    private static void LogMessageToFile (Type loggerType, string message, LogSeverity severity)
    {
      using (s_lock.EnterScope())
      {
        UpdateLogFileNameOnDemand();
        using (StreamWriter? stream = File.AppendText (s_currentLogFilePath!))
        {
          stream.WriteLine (
              DateTime.Now.ToString ("O") + " - " + severity + " - " + Environment.CurrentManagedThreadId + " - " + loggerType.FullName + " - "
              + message);
        }
      }
    }

    private static void UpdateLogFileNameOnDemand ()
    {
      if (s_currentLogFileDate == DateTime.Today && s_currentLogFilePath != null)
      {
        return;
      }

      s_currentLogFileDate = DateTime.Today;
      s_currentLogFilePath = GetLogFileName();
    }

    private static AbsolutePath GetLogFileName ()
    {
      AbsolutePath? logFolder = KnownPaths.LogFolder;
      logFolder.EnsureDirectoryExists();

      return (logFolder / s_currentLogFileDate.ToString ("yyyyMMdd")) + ".txt";
    }

    public void Debug (string message)
    {
      LogMessage (message, LogSeverity.Debug);
    }

    public void Info (string message)
    {
      LogMessage (message, LogSeverity.Information);
    }

    public void Warn (string message)
    {
      LogMessage (message, LogSeverity.Warning);
    }

    public void Error (string message)
    {
      LogMessage (message, LogSeverity.Error);
    }

    public void Fatal (string message)
    {
      LogMessage (message, LogSeverity.Fatal);
    }

    private void LogMessage (string message, LogSeverity severity)
    {
      if (severity >= c_minimumSeverity)
      {
        LogMessageToFile (_loggerType, message, severity);
      }
    }
  }

  public enum LogSeverity
  {
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
    Fatal = 5
  }
}