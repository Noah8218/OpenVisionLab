using OpenVisionLab.Logging.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;

namespace OpenVisionLab.Logging.Controls.Infrastructure
{
    internal sealed class LogPanelFileAccess
    {
        public bool TryReadLatestLogLines(out List<string> lines)
        {
            lines = null;

            string latestLogFile;
            try
            {
                latestLogFile = GetLatestLogFile();
            }
            catch (Exception exception) when (IsRecoverableFileSystemException(exception))
            {
                Trace.TraceWarning("Log panel could not find the latest log file: {0}", exception);
                return false;
            }

            if (string.IsNullOrWhiteSpace(latestLogFile))
            {
                return false;
            }

            try
            {
                using (FileStream stream = new FileStream(latestLogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(stream))
                {
                    lines = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        lines.Add(reader.ReadLine());
                    }
                }

                return true;
            }
            catch (IOException exception)
            {
                Trace.TraceWarning("Log panel could not read the latest log file '{0}': {1}", latestLogFile, exception);
            }
            catch (UnauthorizedAccessException exception)
            {
                Trace.TraceWarning("Log panel could not read the latest log file '{0}': {1}", latestLogFile, exception);
            }
            catch (NotSupportedException exception)
            {
                Trace.TraceWarning("Log panel could not read the latest log file '{0}': {1}", latestLogFile, exception);
            }
            catch (ArgumentException exception)
            {
                Trace.TraceWarning("Log panel could not read the latest log file '{0}': {1}", latestLogFile, exception);
            }
            catch (SecurityException exception)
            {
                Trace.TraceWarning("Log panel could not read the latest log file '{0}': {1}", latestLogFile, exception);
            }

            lines = null;
            return false;
        }

        public void OpenLogFolder()
        {
            string logDirectory = OVLog.GetLogDirectory();
            if (string.IsNullOrWhiteSpace(logDirectory) || !Directory.Exists(logDirectory))
            {
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = logDirectory,
                UseShellExecute = true
            });
        }

        private static string GetLatestLogFile()
        {
            string logDirectory = OVLog.GetLogDirectory();
            if (string.IsNullOrWhiteSpace(logDirectory) || !Directory.Exists(logDirectory))
            {
                return null;
            }

            return Directory.EnumerateFiles(logDirectory, "*ALL.log", SearchOption.AllDirectories)
                .Select(path => new FileInfo(path))
                .OrderByDescending(file => file.LastWriteTime)
                .Select(file => file.FullName)
                .FirstOrDefault();
        }

        private static bool IsRecoverableFileSystemException(Exception exception)
        {
            return exception is IOException
                || exception is UnauthorizedAccessException
                || exception is NotSupportedException
                || exception is ArgumentException
                || exception is SecurityException;
        }
    }
}
