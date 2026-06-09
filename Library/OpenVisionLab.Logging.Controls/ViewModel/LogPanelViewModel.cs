using OpenVisionLab.Logging;
using OpenVisionLab.Logging.Controls.Infrastructure;
using OpenVisionLab.Logging.Controls.Model;
using OpenVisionLab.Logging.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace OpenVisionLab.Logging.Controls.ViewModel
{
    public sealed class LogPanelViewModel : BindableObject, IDisposable
    {
        private const int MaxLogsCount = 3000;
        private const string AnyFilter = "Any";

        private readonly RuntimeLogStream logBufferReader;
        private readonly DispatcherTimer refreshTimer;
        private readonly DateTime sessionStartedAt = Process.GetCurrentProcess().StartTime.AddSeconds(-2);
        private string selectedSignal;
        private string selectedArea;
        private bool showEntireStream = true;
        private bool autoScroll = true;
        private string searchText = string.Empty;

        public LogPanelViewModel()
        {
            logBufferReader = new RuntimeLogStream();
            Signals = new ObservableCollection<string>(new[] { AnyFilter }.Concat(Enum.GetNames(typeof(LogLevel))));
            Areas = new ObservableCollection<string>(Enum.GetNames(typeof(LogCategory)).Select(ConvertAreaName));
            SelectedSignal = AnyFilter;
            SelectedArea = AnyFilter;

            OpenDirectoryCommand = new UiCommand(OpenLogFolder);
            ResetCommand = new UiCommand(Reset);
            LoadLatestLogFile();

            refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        public BulkObservableCollection<LogLine> Logs { get; } = new BulkObservableCollection<LogLine>();

        public BulkObservableCollection<LogLine> FilteredLogs { get; } = new BulkObservableCollection<LogLine>();

        public ObservableCollection<string> Signals { get; }

        public ObservableCollection<string> Areas { get; }

        public ICommand OpenDirectoryCommand { get; }

        public ICommand ResetCommand { get; }

        public string SelectedSignal
        {
            get => selectedSignal;
            set
            {
                if (SetProperty(ref selectedSignal, value))
                {
                    RebuildFilteredLogs();
                }
            }
        }

        public string SelectedArea
        {
            get => selectedArea;
            set
            {
                if (SetProperty(ref selectedArea, value))
                {
                    RebuildFilteredLogs();
                }
            }
        }

        public bool ShowEntireStream
        {
            get => showEntireStream;
            set
            {
                if (SetProperty(ref showEntireStream, value))
                {
                    RebuildFilteredLogs();
                }
            }
        }

        public bool AutoScroll
        {
            get => autoScroll;
            set => SetProperty(ref autoScroll, value);
        }

        public string SearchText
        {
            get => searchText;
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    RebuildFilteredLogs();
                }
            }
        }

        public void Dispose()
        {
            refreshTimer.Stop();
            refreshTimer.Tick -= RefreshTimer_Tick;
            logBufferReader.Dispose();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            string[] newLogs = logBufferReader.GetLogs();
            if (newLogs.Length == 0)
            {
                return;
            }

            List<LogLine> newEntries = newLogs
                .Where(log => !string.IsNullOrWhiteSpace(log))
                .Select(LogLine.Parse)
                .ToList();

            if (newEntries.Count == 0)
            {
                return;
            }

            AddLogs(Logs, newEntries);

            List<LogLine> filtered = newEntries
                .Where(ShouldDisplayLog)
                .ToList();

            AddLogs(FilteredLogs, filtered);
        }

        private void RebuildFilteredLogs()
        {
            FilteredLogs.Clear();
            FilteredLogs.AddRange(Logs.Where(ShouldDisplayLog));
        }

        private void LoadLatestLogFile()
        {
            string latestLogFile = GetLatestLogFile();
            if (string.IsNullOrWhiteSpace(latestLogFile))
            {
                return;
            }

            List<string> lines;
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
            }
            catch
            {
                return;
            }

            List<LogLine> entries = ParseSessionEntries(lines);

            AddLogs(Logs, entries);
            AddLogs(FilteredLogs, entries.Where(ShouldDisplayLog).ToList());
        }

        private List<LogLine> ParseSessionEntries(List<string> lines)
        {
            List<LogLine> entries = new List<LogLine>();
            bool includeContinuation = false;

            foreach (string line in lines.Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                LogLine entry = LogLine.Parse(line);
                if (entry.Timestamp != DateTime.MinValue)
                {
                    includeContinuation = entry.Timestamp >= sessionStartedAt;
                    if (includeContinuation)
                    {
                        entries.Add(entry);
                    }

                    continue;
                }

                if (includeContinuation)
                {
                    entries.Add(entry);
                }
            }

            int excessCount = entries.Count - MaxLogsCount;
            return excessCount > 0
                ? entries.Skip(excessCount).ToList()
                : entries;
        }

        private static string GetLatestLogFile()
        {
            string logDirectory = CLog.GetLogDirectory();
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

        private bool ShouldDisplayLog(LogLine log)
        {
            if (ShowEntireStream)
            {
                return true;
            }

            bool categoryMatches = string.Equals(SelectedArea, AnyFilter, StringComparison.OrdinalIgnoreCase)
                || string.Equals(SelectedArea, ConvertAreaName(log.Category), StringComparison.OrdinalIgnoreCase);

            bool levelMatches = string.Equals(SelectedSignal, AnyFilter, StringComparison.OrdinalIgnoreCase)
                || string.Equals(SelectedSignal, log.Level, StringComparison.OrdinalIgnoreCase);

            bool textMatches = string.IsNullOrWhiteSpace(SearchText)
                || log.RawText.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0;

            return categoryMatches && levelMatches && textMatches;
        }

        private static void AddLogs(BulkObservableCollection<LogLine> target, List<LogLine> logs)
        {
            if (logs.Count == 0)
            {
                return;
            }

            int excessCount = target.Count + logs.Count - MaxLogsCount;
            while (excessCount > 0 && target.Count > 0)
            {
                target.RemoveAt(0);
                excessCount--;
            }

            if (excessCount > 0)
            {
                logs = logs.Skip(excessCount).ToList();
            }

            target.AddRange(logs);
        }

        private static void OpenLogFolder()
        {
            string logDirectory = CLog.GetLogDirectory();
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

        private static string ConvertAreaName(string category)
        {
            return string.Equals(category, LogCategory.All.ToString(), StringComparison.OrdinalIgnoreCase)
                ? AnyFilter
                : category;
        }

        private void Reset()
        {
            Logs.Clear();
            FilteredLogs.Clear();
        }
    }
}
