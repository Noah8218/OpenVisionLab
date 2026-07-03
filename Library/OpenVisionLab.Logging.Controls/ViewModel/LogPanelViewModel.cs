using OpenVisionLab.Logging;
using OpenVisionLab.Logging.Controls.Infrastructure;
using OpenVisionLab.Logging.Controls.Model;
using OpenVisionLab.Logging.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace OpenVisionLab.Logging.Controls.ViewModel
{
    public sealed class LogPanelQuickFilterRequest
    {
        public string Type { get; set; } = "Any";
        public string Level { get; set; } = "Any";
        public string SearchText { get; set; } = string.Empty;
        public bool ShowEntireStream { get; set; } = true;
    }

    public sealed class LogPanelViewModel : BindableObject, IDisposable
    {
        private const int MaxLogsCount = 3000;
        private const string AnyFilter = "Any";
        private static readonly string[] VisibleLevelNames =
        {
            nameof(LogLevel.Info),
            nameof(LogLevel.Warning),
            nameof(LogLevel.Error)
        };
        private static event Action<LogPanelQuickFilterRequest> QuickFilterRequested;

        private readonly RuntimeLogStream logBufferReader;
        private readonly DispatcherTimer refreshTimer;
        private readonly DateTime sessionStartedAt = Process.GetCurrentProcess().StartTime.AddSeconds(-2);
        private string selectedLevel;
        private string selectedType;
        private bool showEntireStream = true;
        private bool autoScroll = true;
        private bool isDetailedMode;
        private bool isCompactLayout = true;
        private bool hasVisibleLogs;
        private string searchText = string.Empty;
        private string summaryText;
        private string latestSummaryText;

        public LogPanelViewModel()
        {
            logBufferReader = new RuntimeLogStream();
            Levels = new ObservableCollection<string>(new[] { AnyFilter }.Concat(VisibleLevelNames));
            Types = new ObservableCollection<string>(
                new[] { AnyFilter }.Concat(
                    Enum.GetNames(typeof(LogCategory))
                        .Where(name => !string.Equals(name, LogCategory.All.ToString(), StringComparison.OrdinalIgnoreCase))));
            SelectedLevel = AnyFilter;
            SelectedType = AnyFilter;
            summaryText = FormatCount(0);
            latestSummaryText = T("Log.NoRecentEvent");

            OpenDirectoryCommand = new UiCommand(OpenLogFolder);
            ResetCommand = new UiCommand(Reset);
            ToggleDetailCommand = new UiCommand(ToggleDetailMode);
            LoadLatestLogFile();

            refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
            QuickFilterRequested += OnQuickFilterRequested;
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
        }

        public BulkObservableCollection<LogLine> Logs { get; } = new BulkObservableCollection<LogLine>();

        public BulkObservableCollection<LogLine> FilteredLogs { get; } = new BulkObservableCollection<LogLine>();

        public ObservableCollection<string> Levels { get; }

        public ObservableCollection<string> Types { get; }

        public ICommand OpenDirectoryCommand { get; }

        public ICommand ResetCommand { get; }

        public ICommand ToggleDetailCommand { get; }

        public bool IsDetailedMode
        {
            get => isDetailedMode;
            set
            {
                if (SetProperty(ref isDetailedMode, value))
                {
                    OnPropertyChanged(nameof(ModeButtonText));
                    OnPropertyChanged(nameof(HeaderText));
                    OnPropertyChanged(nameof(ActiveFilterText));
                    UpdateSummaryText();
                }
            }
        }

        public string ModeButtonText => IsDetailedMode ? T("Log.SummaryMode") : T("Log.DetailMode");

        public string HeaderText => IsDetailedMode ? T("Log.HeaderDetail") : T("Log.HeaderSummary");

        public string ActiveFilterText => BuildActiveFilterText();

        public string ClearText => T("Log.Clear");

        public string FolderText => T("Log.Folder");

        public string AutoScrollText => T("Log.AutoScroll");

        public string AllLogsText => T("Log.AllLogs");

        public string LevelText => T("Log.Level");

        public string AreaText => T("Log.Area");

        public string EmptyText => T("Log.Empty");

        public string EmptyTitleText => T("Log.EmptyTitle", LocalText("실행 로그 대기", "Run log is waiting"));

        public string EmptyDetailText => T(
            "Log.EmptyDetail",
            LocalText(
                "이미지 로드, Preview, Run, 도구 검증 이벤트가 여기에 표시됩니다.",
                "Image load, Preview, Run, and tool verification events appear here."));

        public string EmptyActionHintText => T(
            "Log.EmptyActionHint",
            LocalText(
                "오류나 경고가 있으면 상세 모드에서 필터링할 수 있습니다.",
                "Use Details mode to filter warnings and errors."));

        public string ModeToolTip => T("Log.Tooltip.Mode");

        public string ClearToolTip => T("Log.Tooltip.Clear");

        public string FolderToolTip => T("Log.Tooltip.Folder");

        public string AutoScrollToolTip => T("Log.Tooltip.AutoScroll");

        public string SearchToolTip => T("Log.Tooltip.Search");

        public string AllLogsToolTip => T("Log.Tooltip.AllLogs");

        public string FilterComboToolTip => T("Log.Tooltip.FilterCombo");

        public string SelectedLevel
        {
            get => selectedLevel;
            set
            {
                if (SetProperty(ref selectedLevel, value))
                {
                    OnPropertyChanged(nameof(ActiveFilterText));
                    RebuildFilteredLogs();
                }
            }
        }

        public string SelectedType
        {
            get => selectedType;
            set
            {
                if (SetProperty(ref selectedType, value))
                {
                    OnPropertyChanged(nameof(ActiveFilterText));
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
                    OnPropertyChanged(nameof(IsFilterControlsEnabled));
                    OnPropertyChanged(nameof(ActiveFilterText));
                    RebuildFilteredLogs();
                }
            }
        }

        public bool IsFilterControlsEnabled => !ShowEntireStream;

        public bool AutoScroll
        {
            get => autoScroll;
            set => SetProperty(ref autoScroll, value);
        }

        public bool IsCompactLayout
        {
            get => isCompactLayout;
            set => SetProperty(ref isCompactLayout, value);
        }

        public bool HasVisibleLogs
        {
            get => hasVisibleLogs;
            private set => SetProperty(ref hasVisibleLogs, value);
        }

        public string SearchText
        {
            get => searchText;
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    OnPropertyChanged(nameof(ActiveFilterText));
                    RebuildFilteredLogs();
                }
            }
        }

        public string SummaryText
        {
            get => summaryText;
            private set => SetProperty(ref summaryText, value);
        }

        public string LatestSummaryText
        {
            get => latestSummaryText;
            private set => SetProperty(ref latestSummaryText, value);
        }

        public static void ApplyQuickFilter(string type, string level, string searchText, bool showEntireStream)
        {
            QuickFilterRequested?.Invoke(new LogPanelQuickFilterRequest
            {
                Type = string.IsNullOrWhiteSpace(type) ? AnyFilter : type,
                Level = string.IsNullOrWhiteSpace(level) ? AnyFilter : level,
                SearchText = searchText ?? string.Empty,
                ShowEntireStream = showEntireStream
            });
        }

        public void Dispose()
        {
            QuickFilterRequested -= OnQuickFilterRequested;
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            refreshTimer.Stop();
            refreshTimer.Tick -= RefreshTimer_Tick;
            logBufferReader.Dispose();
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(ModeButtonText));
            OnPropertyChanged(nameof(HeaderText));
            OnPropertyChanged(nameof(ActiveFilterText));
            OnPropertyChanged(nameof(ClearText));
            OnPropertyChanged(nameof(FolderText));
            OnPropertyChanged(nameof(AutoScrollText));
            OnPropertyChanged(nameof(AllLogsText));
            OnPropertyChanged(nameof(LevelText));
            OnPropertyChanged(nameof(AreaText));
            OnPropertyChanged(nameof(EmptyText));
            OnPropertyChanged(nameof(EmptyTitleText));
            OnPropertyChanged(nameof(EmptyDetailText));
            OnPropertyChanged(nameof(EmptyActionHintText));
            OnPropertyChanged(nameof(ModeToolTip));
            OnPropertyChanged(nameof(ClearToolTip));
            OnPropertyChanged(nameof(FolderToolTip));
            OnPropertyChanged(nameof(AutoScrollToolTip));
            OnPropertyChanged(nameof(SearchToolTip));
            OnPropertyChanged(nameof(AllLogsToolTip));
            OnPropertyChanged(nameof(FilterComboToolTip));
            UpdateSummaryText();
        }

        private void OnQuickFilterRequested(LogPanelQuickFilterRequest request)
        {
            if (request == null)
            {
                return;
            }

            if (refreshTimer.Dispatcher.CheckAccess())
            {
                ApplyQuickFilterCore(request);
                return;
            }

            refreshTimer.Dispatcher.BeginInvoke(new Action(() => ApplyQuickFilterCore(request)));
        }

        private void ApplyQuickFilterCore(LogPanelQuickFilterRequest request)
        {
            ShowEntireStream = request.ShowEntireStream;
            SelectedType = NormalizeFilter(Types, request.Type);
            SelectedLevel = NormalizeFilter(Levels, request.Level);
            SearchText = request.SearchText ?? string.Empty;
            IsDetailedMode = !request.ShowEntireStream
                || !string.IsNullOrWhiteSpace(request.SearchText);
            RebuildFilteredLogs();
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
            UpdateSummaryText();
        }

        private void RebuildFilteredLogs()
        {
            FilteredLogs.Clear();
            FilteredLogs.AddRange(Logs.Where(ShouldDisplayLog));
            UpdateSummaryText();
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
            UpdateSummaryText();
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

        private bool ShouldDisplayLog(LogLine log)
        {
            bool textMatches = string.IsNullOrWhiteSpace(SearchText)
                || log.RawText.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0;

            if (ShowEntireStream)
            {
                return textMatches;
            }

            bool categoryMatches = IsAnyFilterText(SelectedType)
                || string.Equals(SelectedType, log.Category, StringComparison.OrdinalIgnoreCase);
            bool levelMatches = IsAnyFilterText(SelectedLevel)
                || string.Equals(SelectedLevel, log.Level, StringComparison.OrdinalIgnoreCase);

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

        private static bool IsAnyFilterText(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                || string.Equals(value, AnyFilter, StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "Any", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "All", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, LogCategory.All.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeFilter(IEnumerable<string> options, string requested)
        {
            if (IsAnyFilterText(requested))
            {
                return AnyFilter;
            }

            return options?.FirstOrDefault(option => string.Equals(option, requested, StringComparison.OrdinalIgnoreCase))
                ?? AnyFilter;
        }

        private void Reset()
        {
            Logs.Clear();
            FilteredLogs.Clear();
            UpdateSummaryText();
        }

        private void ToggleDetailMode()
        {
            if (IsDetailedMode)
            {
                ShowEntireStream = true;
                SelectedType = AnyFilter;
                SelectedLevel = AnyFilter;
                SearchText = string.Empty;
                IsDetailedMode = false;
                RebuildFilteredLogs();
                return;
            }

            IsDetailedMode = true;
        }

        private void UpdateSummaryText()
        {
            HasVisibleLogs = FilteredLogs.Count > 0;
            SummaryText = ShowEntireStream && string.IsNullOrWhiteSpace(SearchText)
                ? FormatCount(Logs.Count)
                : string.Format(CultureInfo.CurrentCulture, T("Log.FilteredCountFormat"), FilteredLogs.Count, Logs.Count);
            LatestSummaryText = BuildLatestSummaryText();
            OnPropertyChanged(nameof(ActiveFilterText));
        }

        private string BuildLatestSummaryText()
        {
            LogLine latest = FilteredLogs.LastOrDefault();
            if (latest == null)
            {
                return T("Log.NoRecentEvent");
            }

            string category = latest.Category;
            string level = latest.Level;
            string source = latest.Source;
            string message = string.IsNullOrWhiteSpace(latest.Message) ? latest.RawText : latest.Message;
            string prefix = string.IsNullOrWhiteSpace(source) ? level : $"{level} · {source}";
            if (!IsAnyFilterText(category))
            {
                prefix = string.IsNullOrWhiteSpace(prefix) ? category : $"{category} · {prefix}";
            }

            string summary = string.IsNullOrWhiteSpace(prefix) ? message : $"{prefix} · {message}";
            return summary.Length > 120 ? summary.Substring(0, 120) + "..." : summary;
        }

        private string BuildActiveFilterText()
        {
            List<string> parts = new List<string>();
            if (ShowEntireStream)
            {
                parts.Add(T("Log.AllLogs"));
                parts.Add(T("Log.FiltersOff"));
            }
            else
            {
                parts.Add(T("Log.FilteredView"));
                if (!IsAnyFilterText(SelectedType))
                {
                    parts.Add(string.Format(CultureInfo.CurrentCulture, T("Log.AreaFormat"), SelectedType));
                }

                if (!IsAnyFilterText(SelectedLevel))
                {
                    parts.Add(string.Format(CultureInfo.CurrentCulture, T("Log.LevelFormat"), SelectedLevel));
                }

                if (parts.Count == 1)
                {
                    parts.Add(T("Log.NoFilter"));
                }
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string keyword = SearchText.Trim();
                if (keyword.Length > 24)
                {
                    keyword = keyword.Substring(0, 24) + "...";
                }

                parts.Add(string.Format(CultureInfo.CurrentCulture, T("Log.SearchFormat"), keyword));
            }

            return string.Join(" · ", parts);
        }

        private static string FormatCount(int count)
        {
            return string.Format(CultureInfo.CurrentCulture, T("Log.CountFormat"), count);
        }

        private static string T(string key)
        {
            return OpenVisionLanguageService.T(key);
        }

        private static string T(string key, string fallback)
        {
            string text = OpenVisionLanguageService.T(key);
            return string.Equals(text, key, StringComparison.OrdinalIgnoreCase)
                ? fallback ?? string.Empty
                : text;
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? english ?? string.Empty
                : korean ?? string.Empty;
        }
    }
}


