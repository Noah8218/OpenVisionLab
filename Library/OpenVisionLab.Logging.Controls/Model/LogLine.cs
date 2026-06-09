using System;
using System.Text.RegularExpressions;

namespace OpenVisionLab.Logging.Controls.Model
{
    public sealed class LogLine
    {
        private static readonly Regex LogPattern = new Regex(
            @"^\[(?<time>[^\]]*)\]\s*\[(?<category>[^\]]*)\]\[(?<level>[^\]]*)\](?:\[(?<source>[^\]]*)\])?\s*(?<message>.*)$",
            RegexOptions.Compiled);

        public LogLine(
            DateTime timestamp,
            string timestampText,
            string category,
            string level,
            string source,
            string message,
            string rawText)
        {
            Timestamp = timestamp;
            TimestampText = timestampText ?? string.Empty;
            Category = category ?? string.Empty;
            Level = level ?? string.Empty;
            Source = source ?? string.Empty;
            Message = message ?? string.Empty;
            RawText = rawText ?? string.Empty;
        }

        public DateTime Timestamp { get; }

        public string TimestampText { get; }

        public string DisplayTime => Timestamp == DateTime.MinValue
            ? string.Empty
            : Timestamp.ToString("HH:mm:ss.fff");

        public string Category { get; }

        public string Level { get; }

        public string Source { get; }

        public string Message { get; }

        public string RawText { get; }

        public static LogLine Parse(string rawText)
        {
            string text = rawText ?? string.Empty;
            Match match = LogPattern.Match(text);
            if (!match.Success)
            {
                return new LogLine(DateTime.MinValue, string.Empty, string.Empty, string.Empty, string.Empty, text, text);
            }

            string timestampText = match.Groups["time"].Value;
            DateTime.TryParse(timestampText, out DateTime timestamp);

            return new LogLine(
                timestamp,
                timestampText,
                match.Groups["category"].Value,
                match.Groups["level"].Value,
                match.Groups["source"].Value,
                match.Groups["message"].Value,
                text);
        }
    }
}
