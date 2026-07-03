using System;

namespace OpenVisionLab.Logging
{
	public enum LogLevel
	{
		Info = 0,
		Warning,
		Error,
		Debug
	}

	public enum LogCategory
	{
		All,
		System,
		Main,
		Vision,
		Pipeline,
		UI
	}


	[Flags]
	public enum MiniDumpType
	{
		MiniDumpNormal = 0x00000000,
		MiniDumpWithDataSegs = 0x00000001,
		MiniDumpWithFullMemory = 0x00000002,
	}
}
