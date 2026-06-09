using log4net;
using log4net.Appender;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using OpenVisionLab.Logging.Retention;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenVisionLab.Logging
{
	public static class CLog
	{
		[DllImport("Dbghelp.dll")]
		static extern bool MiniDumpWriteDump(IntPtr hProcess, uint ProcessId, IntPtr hFile, int DumpType, ref MINIDUMP_EXCEPTION_INFORMATION ExceptionParam, IntPtr UserStreamParam, IntPtr CallbackParam);

		[DllImport("kernel32.dll")]
		static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll")]
		static extern uint GetCurrentProcessId();

		[DllImport("kernel32.dll")]
		static extern uint GetCurrentThreadId();

		[StructLayout(LayoutKind.Sequential, Pack = 4)]  // Win32 구조체와 일치하도록 4-byte packing을 사용합니다.
		public struct MINIDUMP_EXCEPTION_INFORMATION
		{
			public uint ThreadId;
			public IntPtr ExceptionPointers;
			[MarshalAs(UnmanagedType.Bool)]
			public bool ClientPointers;
		}

		private const int MiniDumpNormal = 0x00000000; // 최소한의 스택 정보만 남기는 플래그
		private const int MiniDumpWithFullMemory = 0x00000002; // 모든 스택 정보와, 스레드, 메모리 상태 정보를 남기는 플래그		
		
		private static LogRetentionService _logRetention;

		static CLog()
		{
			string assemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			string configPath = Path.Combine(assemblyDirectory ?? AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
			if (File.Exists(configPath))
			{
				log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(configPath));
			}

			log4net.Util.LogLog.InternalDebugging = false;
		}

		[DllImport("DbgHelp.dll", SetLastError = true)]
		public static extern bool MiniDumpWriteDump
		(
			IntPtr hProcess,
			uint ProcessId,
			SafeHandle hFile,
			MiniDumpType DumpType,
			ref MINIDUMP_EXCEPTION_INFORMATION ExceptionParam,
			IntPtr UserStreamParam,
			IntPtr CallbackParam
		);

		// 전체 로그
		private static readonly ILog _log = LogManager.GetLogger(nameof(_log));
		private static readonly ILog _logAbnormal = LogManager.GetLogger(nameof(_logAbnormal));
		private static readonly ILog _logDebug = LogManager.GetLogger(nameof(_logDebug));
		private static readonly ILog _logMemoryPool = LogManager.GetLogger(nameof(_logMemoryPool));
		private static readonly ILog _logMain = LogManager.GetLogger(nameof(_logMain));
		private static readonly ILog _logWorker = LogManager.GetLogger(nameof(_logWorker));
		private static readonly ILog _logGrab = LogManager.GetLogger(nameof(_logGrab));
		private static readonly ILog _logVision = LogManager.GetLogger(nameof(_logVision));
		private static readonly ILog _logNetwork = LogManager.GetLogger(nameof(_logNetwork));
		private static readonly ILog _logDB = LogManager.GetLogger(nameof(_logDB));
		private static readonly ILog _logInspect = LogManager.GetLogger(nameof(_logInspect));
		private static readonly ILog _logConfig = LogManager.GetLogger(nameof(_logConfig));


		private static string Builder(LogCategory logType, LogLevel logLevel, params object[] ob)
		{
			StringBuilder sb = new StringBuilder();
			// 호출한 메서드와 클래스 이름을 얻기
			// 2023.09.27 클래스명과 메서드명을 얻기 위해 StackTrace를 사용하는건 적절한 방법이지만
			// 람다안에서 돌리거나 하면 정확한 네이밍을 얻기가 힘듭니다.
			// 다만 함수명은 제대로 나옵니다.
			var stackTrace = new System.Diagnostics.StackTrace();
			var method = stackTrace.GetFrame(4).GetMethod();
			string className = method.ReflectedType.Name;
			string methodName = method.Name;

			sb.Append(string.Format($"[{logType}][{logLevel}][{className}.{methodName}] "));
			for (int i = 0; i < ob.Length; i++)
			{
				sb.Append(ob[i]);
				sb.Append(" ");
			}
			return sb.ToString();
		}

		public static string GetLogDirectory()
		{
			var hierarchy = (Hierarchy)LogManager.GetRepository();

			var appender = hierarchy.GetAppenders().FirstOrDefault(a => a.Name == "file") as RollingFileAppender;

			if (appender != null)
			{
				var logFileDirectory = Path.GetDirectoryName(appender.File);
				return logFileDirectory;
			}
			else
			{
				return null;
			}
		}

		public static void ApplyRetentionPolicy(string logDirectory, int retentionDays = 90)
		{
			string logRoot = logDirectory.EndsWith("\\") ? logDirectory : logDirectory + "\\";

			_logRetention = new LogRetentionService(
				logRootDir: logRoot,
				retentionDays
			);
		}

		public static void ApplyFilePolicy(string logDirectory, int maxBackupFileCount, int maximumFileSizeInMB)
		{
			ILoggerRepository repository = LogManager.GetRepository();
			foreach (IAppender appender in repository.GetAppenders())
			{
				if (appender is RollingFileAppender rollingFileAppender)
				{
					rollingFileAppender.File = logDirectory.EndsWith("\\") ? logDirectory : logDirectory + "\\";
					rollingFileAppender.MaxSizeRollBackups = maxBackupFileCount;
					rollingFileAppender.MaximumFileSize = $"{maximumFileSizeInMB}MB";
					rollingFileAppender.Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
					rollingFileAppender.ActivateOptions();
				}
			}
		}

		public static void Read()
		{
			ILoggerRepository repository = LogManager.GetRepository();
			foreach (IAppender appender in repository.GetAppenders())
			{
				// RollingFileAppender를 찾습니다.
				if (appender is RollingFileAppender rollingFileAppender)
				{
					//Console.WriteLine($"{rollingFileAppender.File }");
				}
			}
		}

		public static void CreateMiniDump(bool useDetailDump = false)
		{
			string logDirPath = GetLogDirectory();
			string exeName = AppDomain.CurrentDomain.FriendlyName;
			string dateTime = DateTime.Now.ToString("[yyyy-MM-dd][HH-mm-ss-fff]");

			MINIDUMP_EXCEPTION_INFORMATION info = new MINIDUMP_EXCEPTION_INFORMATION
			{
				ClientPointers = true,
				ExceptionPointers = IntPtr.Zero,
				ThreadId = GetCurrentThreadId()
			};

			string dumpFile = useDetailDump
				? Path.Combine(logDirPath, $"[{exeName}]{dateTime}.dmp")
				: Path.Combine(logDirPath, $"[{exeName}_mini]{dateTime}.dmp");

			int dumpType = useDetailDump ? MiniDumpWithFullMemory : MiniDumpNormal;

			using (var file = new FileStream(dumpFile, FileMode.Create))
			{
				MiniDumpWriteDump(GetCurrentProcess(), GetCurrentProcessId(), file.SafeFileHandle.DangerousGetHandle(), dumpType, ref info, IntPtr.Zero, IntPtr.Zero);
			}
		}


		// 기본값
		public static void Write(params object[] ob)
		{
			WriteLogging(LogCategory.Info, LogLevel.Normal, ob);
		}

		// LogType만 지정하는 버전
		public static void Write(LogCategory logType, params object[] ob)
		{
			WriteLogging(logType, LogLevel.Normal, ob);
		}

		// LogLevel만 지정하는 버전
		public static void Write(LogLevel logLevel, params object[] ob)
		{
			WriteLogging(LogCategory.Info, logLevel, ob);
		}

		// LogType, LogLevel 둘다 지정하는 버전
		public static void Write(LogCategory logType, LogLevel logLevel, params object[] ob)
		{
			WriteLogging(logType, logLevel, ob);
		}

		private static void WriteLogging(LogCategory logType, LogLevel logLevel, params object[] ob)
		{
			switch (logLevel)
			{
				case LogLevel.Normal:
					CLog.Normal(logType, ob);
					break;
				case LogLevel.Abnormal:
					CLog.Abnormal(logType, ob);
					break;
				case LogLevel.Debug:
					CLog.Debug(logType, ob);
					break;
				case LogLevel.MemoryPool:
					CLog.MemoryPool(logType, ob);
					break;

				case LogLevel.Main:
					CLog.Main(logType, ob);
					break;

				case LogLevel.Worker:
					CLog.Worker(logType, ob);
					break;

				case LogLevel.Grab:
					CLog.Grab(logType, ob);
					break;

				case LogLevel.Vision:
					CLog.Vision(logType, ob);
					break;

				case LogLevel.Network:
					CLog.Network(logType, ob);
					break;

				case LogLevel.DB:
					CLog.DB(logType, ob);
					break;

				case LogLevel.Inspect:
					CLog.Inspect(logType, ob);
					break;

				case LogLevel.Config:
					CLog.Config(logType, ob);
					break;

			}
		}

		private static void Normal(LogCategory logType, params object[] ob)
		{
			if (_log == null) { return; }
			_log.Info(Builder(logType, LogLevel.Normal, ob));
		}

		private static void Abnormal(LogCategory logType, params object[] ob)
		{
			if (_log == null) { return; }
			if (_logAbnormal == null) { return; }
			_log.Error(Builder(logType, LogLevel.Abnormal, ob));
			_logAbnormal.Error(Builder(logType, LogLevel.Abnormal, ob));
		}

		private static void Debug(LogCategory logType, params object[] ob)
		{
			if (_log == null) { return; }
			if (_logDebug == null) { return; }
			_log.Info(Builder(logType, LogLevel.Debug, ob));
			_logDebug.Info(Builder(logType, LogLevel.Debug, ob));
		}

		private static void MemoryPool(LogCategory logType, params object[] ob)
		{
			if (_log == null) { return; }
			if (_logMemoryPool == null) { return; }
			_log.Info(Builder(logType, LogLevel.MemoryPool, ob));
			_logMemoryPool.Info(Builder(logType, LogLevel.MemoryPool, ob));
		}

		private static void Main(LogCategory logType, params object[] ob)
		{
			if (_log == null) { return; }
			if (_logMain == null) { return; }
			_log.Info(Builder(logType, LogLevel.Main, ob));
			_logMain.Info(Builder(logType, LogLevel.Main, ob));
		}
		private static void Worker(LogCategory logType, params object[] ob)
		{
			if (_log == null) { return; }
			if (_logWorker == null) { return; }
			_log.Info(Builder(logType, LogLevel.Worker, ob));
			_logWorker.Info(Builder(logType, LogLevel.Worker, ob));
		}

		private static void Grab(LogCategory logType, params object[] ob)
		{
			if (_log == null) { return; }
			if (_logGrab == null) { return; }
			_log.Info(Builder(logType, LogLevel.Grab, ob));
			_logGrab.Info(Builder(logType, LogLevel.Grab, ob));
		}

		private static void Vision(LogCategory logType, params object[] ob)
		{
			if (_log == null) { return; }
			if (_logVision == null) { return; }
			_log.Info(Builder(logType, LogLevel.Vision, ob));
			_logVision.Info(Builder(logType, LogLevel.Vision, ob));
		}

		private static void Network(LogCategory logType, params object[] ob)
		{
			if (_log == null) { return; }
			if (_logNetwork == null) { return; }
			_log.Info(Builder(logType, LogLevel.Network, ob));
			_logNetwork.Info(Builder(logType, LogLevel.Network, ob));
		}

		private static void DB(LogCategory logType, params object[] ob)
		{
			if (_log == null) { return; }
			if (_logDB == null) { return; }
			_log.Info(Builder(logType, LogLevel.DB, ob));
			_logDB.Info(Builder(logType, LogLevel.DB, ob));
		}

		private static void Inspect(LogCategory logType, params object[] ob)
		{
			if (_log == null) { return; }
			if (_logInspect == null) { return; }
			_log.Info(Builder(logType, LogLevel.Inspect, ob));
			_logInspect.Info(Builder(logType, LogLevel.Inspect, ob));
		}

		private static void Config(LogCategory logType, params object[] ob)
		{
			if (_log == null) { return; }
			if (_logConfig == null) { return; }
			_log.Info(Builder(logType, LogLevel.Config, ob));
			_logConfig.Info(Builder(logType, LogLevel.Config, ob));
		}
	}
}
