using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.Generic;

namespace OpenVisionLab.Logging.Model
{
	public class RuntimeLogSink : AppenderSkeleton
	{
		public const int DefaultMaxBufferedEntries = 4096;
		public const int DefaultMaxBufferedCharacters = 1024 * 1024;
		public const int DefaultMaxReadEntries = 250;

		private readonly Queue<string> _logQueue = new Queue<string>();
		private readonly object _syncRoot = new object();
		private string _lastRenderedMessage;
		private long _lastTimestampTicks;
		private int _bufferedCharacters;
		private long _droppedLogCount;

		public RuntimeLogSink()
		{
		}

		protected override void Append(LoggingEvent loggingEvent)
		{
			if (loggingEvent == null)
			{
				return;
			}

			string renderedMessage = loggingEvent.RenderedMessage ?? string.Empty;
			if (ShouldSkipDuplicate(renderedMessage, loggingEvent.TimeStamp))
			{
				return;
			}

			string logMessage = string.Format("[{0}]{1}", loggingEvent.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff"), renderedMessage);
			lock (_syncRoot)
			{
				if (logMessage.Length > DefaultMaxBufferedCharacters)
				{
					_droppedLogCount++;
					return;
				}

				while (_logQueue.Count >= DefaultMaxBufferedEntries
					|| _bufferedCharacters + logMessage.Length > DefaultMaxBufferedCharacters)
				{
					if (_logQueue.Count == 0)
					{
						break;
					}

					string removedLog = _logQueue.Dequeue();
					_bufferedCharacters -= removedLog.Length;
					_droppedLogCount++;
				}

				_logQueue.Enqueue(logMessage);
				_bufferedCharacters += logMessage.Length;
			}
		}

		private bool ShouldSkipDuplicate(string renderedMessage, System.DateTime timestamp)
		{
			lock (_syncRoot)
			{
				long timestampTicks = timestamp.Ticks;
				bool isDuplicate = renderedMessage == _lastRenderedMessage
					&& timestampTicks - _lastTimestampTicks < System.TimeSpan.FromMilliseconds(200).Ticks;

				_lastRenderedMessage = renderedMessage;
				_lastTimestampTicks = timestampTicks;
				return isDuplicate;
			}
		}

		public long DroppedLogCount
		{
			get
			{
				lock (_syncRoot)
				{
					return _droppedLogCount;
				}
			}
		}

		public IReadOnlyList<string> ReadEntries(int maxEntries = DefaultMaxReadEntries)
		{
			if (maxEntries <= 0)
			{
				return Array.Empty<string>();
			}

			lock (_syncRoot)
			{
				int count = Math.Min(maxEntries, _logQueue.Count);
				List<string> entries = new List<string>(count);
				for (int i = 0; i < count; i++)
				{
					string log = _logQueue.Dequeue();
					_bufferedCharacters -= log.Length;
					entries.Add(log);
				}

				return entries;
			}
		}

		public string ReadBuffer()
		{
			IReadOnlyList<string> entries = ReadEntries();
			if (entries.Count == 0)
			{
				return string.Empty;
			}

			return string.Join(Environment.NewLine, entries) + Environment.NewLine;
		}
	}
}
