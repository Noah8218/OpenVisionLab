using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVisionLab.Logging.Retention
{
	public static class LogRetentionPruner
	{
		/// <summary>
		/// Log\yyyy\MM\dd\ 구조에서 dd 폴더(하루치) 기준으로 N일 지난 로그 폴더를 삭제합니다.
		/// 예: Log\2026\01\08\...
		/// </summary>
		/// <param name="logRootDir">예: AppDomain.CurrentDomain.BaseDirectory + "Log"</param>
		/// <param name="retentionDays">보관일수 (예: 30이면 30일만 보관)</param>
		/// <param name="now">기준시간(테스트용). null이면 DateTime.Now</param>
		/// <returns>삭제한 날짜 폴더(yyyy\MM\dd)의 개수</returns>
		public static int DeleteExpiredDateFolders(string logRootDir, int retentionDays, DateTime? now = null)
		{
			if (string.IsNullOrWhiteSpace(logRootDir))
				throw new ArgumentException("logRootDir is null/empty", nameof(logRootDir));
			if (retentionDays < 1)
				throw new ArgumentOutOfRangeException(nameof(retentionDays), "retentionDays must be >= 1");

			var root = new DirectoryInfo(logRootDir);
			if (!root.Exists)
				return 0;

			DateTime 기준 = (now ?? DateTime.Now).Date;
			DateTime cutoff = 기준.AddDays(-retentionDays); 

			int deletedCount = 0;

			// 1) yyyy
			foreach (var yearDir in SafeGetDirectories(root))
			{
				if (!int.TryParse(yearDir.Name, out int y) || y < 2000 || y > 3000)
					continue;

				// 2) MM
				foreach (var monthDir in SafeGetDirectories(yearDir))
				{
					if (!int.TryParse(monthDir.Name, out int m) || m < 1 || m > 12)
						continue;

					// 3) dd
					foreach (var dayDir in SafeGetDirectories(monthDir))
					{
						if (!int.TryParse(dayDir.Name, out int d) || d < 1 || d > 31)
							continue;

						if (!TryMakeDate(y, m, d, out DateTime folderDate))
							continue;
						
						if (folderDate < cutoff)
						{
							if (TryDeleteDirectoryRecursive(dayDir.FullName))
								deletedCount++;
						}
					}
					
					TryDeleteIfEmpty(monthDir.FullName);
				}
				
				TryDeleteIfEmpty(yearDir.FullName);
			}

			return deletedCount;
		}

		private static bool TryMakeDate(int y, int m, int d, out DateTime date)
		{
			try
			{
				date = new DateTime(y, m, d);
				return true;
			}
			catch
			{
				date = default;
				return false;
			}
		}

		private static DirectoryInfo[] SafeGetDirectories(DirectoryInfo dir)
		{
			try { return dir.GetDirectories(); }
			catch { return Array.Empty<DirectoryInfo>(); }
		}

		private static bool TryDeleteDirectoryRecursive(string path)
		{
			try
			{				
				RemoveReadOnlyAttributes(path);

				Directory.Delete(path, recursive: true);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static void RemoveReadOnlyAttributes(string directoryPath)
		{
			try
			{
				foreach (var file in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories))
				{
					try
					{
						var attr = File.GetAttributes(file);
						if ((attr & FileAttributes.ReadOnly) != 0)
							File.SetAttributes(file, attr & ~FileAttributes.ReadOnly);
					}
					catch { }
				}
			}
			catch { }
		}

		private static void TryDeleteIfEmpty(string dir)
		{
			try
			{
				if (!Directory.Exists(dir)) return;

				bool hasAny = Directory.EnumerateFileSystemEntries(dir).Any();
				if (!hasAny)
					Directory.Delete(dir, recursive: false);
			}
			catch { /* ignore */ }
		}
	}
}
