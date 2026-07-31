using log4net.Appender;
using log4net.Core;
using System.Collections.Concurrent;
using System.Text;

namespace OpenVisionLab.Logging.Model
{
	public class RuntimeLogSink : AppenderSkeleton
	{
		private readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
		private readonly object _syncRoot = new object();
		private string _lastRenderedMessage;
		private long _lastTimestampTicks;

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
			_logQueue.Enqueue(logMessage);
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

		public string ReadBuffer()
		{
			StringBuilder sb = new StringBuilder();
			while (_logQueue.TryDequeue(out string log))
			{
				sb.AppendLine(log);
			}
			return sb.ToString();
		}
	}
}
