using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenVisionLab.Logging.Retention
{
	public sealed class LogRetentionService : IDisposable
	{
		private readonly string _logRootDir;
		private readonly int _retentionDays;
		private readonly Timer _timer;

		private static readonly TimeSpan OneDay = TimeSpan.FromDays(1);

		public LogRetentionService(string logRootDir, int retentionDays)
		{
			_logRootDir = logRootDir ?? throw new ArgumentNullException(nameof(logRootDir));
			_retentionDays = retentionDays;
			
			RunCleanupSafe();
			
			TimeSpan dueTime = GetInitialDueTime();

			_timer = new Timer(_ =>
			{
				RunCleanupSafe();
			}, null, dueTime, OneDay);
		}

		private static TimeSpan GetInitialDueTime()
		{
			DateTime now = DateTime.Now;
			DateTime next = now.Date.AddDays(1).AddMinutes(1); 
			return next - now;
		}

		private void RunCleanupSafe()
		{			
			Task.Run(() =>
			{
				try
				{
					int deleted = LogRetentionPruner.DeleteExpiredDateFolders(
						_logRootDir,
						_retentionDays
					);
				}
				catch
				{
					
				}
			});
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}
	}
}
