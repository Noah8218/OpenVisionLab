using log4net;
using log4net.Appender;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using OpenVisionLab.Logging.Retention;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace OpenVisionLab.Logging
{
	public static class OVLog
	{
		[DllImport("Dbghelp.dll")]
		private static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, IntPtr hFile, int dumpType, ref MINIDUMP_EXCEPTION_INFORMATION exceptionParam, IntPtr userStreamParam, IntPtr callbackParam);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll")]
		private static extern uint GetCurrentProcessId();

		[DllImport("kernel32.dll")]
		private static extern uint GetCurrentThreadId();

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		public struct MINIDUMP_EXCEPTION_INFORMATION
		{
			public uint ThreadId;
			public IntPtr ExceptionPointers;
			[MarshalAs(UnmanagedType.Bool)]
			public bool ClientPointers;
		}

		private const int MiniDumpNormal = 0x00000000;
		private const int MiniDumpWithFullMemory = 0x00000002;
		private static readonly ILog allLog = LogManager.GetLogger("OpenVisionLab.All");
		private static readonly ILog warningLog = LogManager.GetLogger("OpenVisionLab.Warning");
		private static readonly ILog errorLog = LogManager.GetLogger("OpenVisionLab.Error");
		private static readonly ILog debugLog = LogManager.GetLogger("OpenVisionLab.Debug");
		private static LogRetentionService logRetention;

		static OVLog()
		{
			string assemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			string configPath = Path.Combine(assemblyDirectory ?? AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
			if (File.Exists(configPath))
			{
				ConfigureFromFile(configPath, GetFallbackLogDirectory());
			}

			log4net.Util.LogLog.InternalDebugging = false;
		}

		[DllImport("DbgHelp.dll", SetLastError = true)]
		public static extern bool MiniDumpWriteDump(
			IntPtr hProcess,
			uint processId,
			SafeHandle hFile,
			MiniDumpType dumpType,
			ref MINIDUMP_EXCEPTION_INFORMATION exceptionParam,
			IntPtr userStreamParam,
			IntPtr callbackParam);

		public static string GetLogDirectory()
		{
			Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
			RollingFileAppender appender = hierarchy.GetAppenders().FirstOrDefault(a => a.Name == "AllFile") as RollingFileAppender;
			return appender == null ? null : Path.GetDirectoryName(appender.File);
		}

		public static void ApplyRetentionPolicy(string logDirectory, int retentionDays = 90)
		{
			if (string.IsNullOrWhiteSpace(logDirectory))
			{
				return;
			}

			string logRoot = logDirectory.EndsWith("\\") ? logDirectory : logDirectory + "\\";
			logRetention = new LogRetentionService(logRoot, retentionDays);
		}

		public static void ApplyFilePolicy(string logDirectory, int maxBackupFileCount, int maximumFileSizeInMB)
		{
			if (string.IsNullOrWhiteSpace(logDirectory))
			{
				return;
			}

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

		public static void CreateMiniDump(bool useDetailDump = false)
		{
			string logDirPath = GetLogDirectory();
			if (string.IsNullOrWhiteSpace(logDirPath))
			{
				logDirPath = GetFallbackLogDirectory();
			}

			Directory.CreateDirectory(logDirPath);
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
			using (FileStream file = new FileStream(dumpFile, FileMode.Create))
			{
				MiniDumpWriteDump(GetCurrentProcess(), GetCurrentProcessId(), file.SafeFileHandle.DangerousGetHandle(), dumpType, ref info, IntPtr.Zero, IntPtr.Zero);
			}
		}

		private static void ConfigureFromFile(string configPath, string logDirectory)
		{
			Directory.CreateDirectory(logDirectory);
			XmlDocument document = new XmlDocument
			{
				XmlResolver = null
			};
			document.Load(configPath);
			foreach (XmlNode node in document.SelectNodes("//appender/file"))
			{
				XmlElement fileElement = node as XmlElement;
				if (fileElement == null)
				{
					continue;
				}

				string configuredValue = fileElement.GetAttribute("value") ?? string.Empty;
				string fileName = configuredValue.EndsWith(
					".log",
					StringComparison.OrdinalIgnoreCase)
					? Path.GetFileName(configuredValue)
					: string.Empty;
				fileElement.SetAttribute(
					"value",
					string.IsNullOrWhiteSpace(fileName)
						? logDirectory + Path.DirectorySeparatorChar
						: Path.Combine(logDirectory, fileName));
			}

			XmlElement log4netElement =
				document.SelectSingleNode("//log4net") as XmlElement;
			if (log4netElement == null)
			{
				throw new InvalidDataException(
					"log4net.config does not contain a log4net element.");
			}

			log4net.Config.XmlConfigurator.Configure(
				LogManager.GetRepository(),
				log4netElement);
		}

		private static string GetFallbackLogDirectory()
		{
			string configured = Environment.GetEnvironmentVariable(
				"OPENVISIONLAB_LOG_ROOT");
			if (!string.IsNullOrWhiteSpace(configured)
				&& Path.IsPathRooted(configured))
			{
				return Path.GetFullPath(configured);
			}

			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
		}

		public static void Write(params object[] values)
		{
			Write(LogCategory.Main, LogLevel.Info, values);
		}

		public static void Write(LogCategory category, params object[] values)
		{
			Write(category, LogLevel.Info, values);
		}

		public static void Write(LogLevel level, params object[] values)
		{
			Write(LogCategory.Main, level, values);
		}

		public static void Write(LogCategory category, LogLevel level, params object[] values)
		{
			string message = BuildMessage(category, level, values);
			WriteToLogger(allLog, level, message);

			switch (level)
			{
				case LogLevel.Warning:
					WriteToLogger(warningLog, level, message);
					break;
				case LogLevel.Error:
					WriteToLogger(errorLog, level, message);
					break;
				case LogLevel.Debug:
					WriteToLogger(debugLog, level, message);
					break;
			}
		}

		private static string BuildMessage(LogCategory category, LogLevel level, params object[] values)
		{
			System.Reflection.MethodBase method = new StackFrame(4, false).GetMethod();
			string className = method?.ReflectedType?.Name ?? "Unknown";
			string methodName = method?.Name ?? "Unknown";

			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("[{0}][{1}][{2}.{3}] ", category, level, className, methodName);
			if (values != null)
			{
				for (int i = 0; i < values.Length; i++)
				{
					if (i > 0)
					{
						builder.Append(' ');
					}

					builder.Append(values[i]);
				}
			}

			return builder.ToString();
		}

		private static void WriteToLogger(ILog logger, LogLevel level, string message)
		{
			if (logger == null)
			{
				return;
			}

			switch (level)
			{
				case LogLevel.Error:
					logger.Error(message);
					break;
				case LogLevel.Warning:
					logger.Warn(message);
					break;
				case LogLevel.Debug:
					logger.Debug(message);
					break;
				default:
					logger.Info(message);
					break;
			}
		}
	}
}

