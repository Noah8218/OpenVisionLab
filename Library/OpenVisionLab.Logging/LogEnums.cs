using System;

namespace OpenVisionLab.Logging
{
	public enum LogLevel
	{
		Normal = 0, // 정상 동작상태 
		Abnormal, // 비정상 동작상태 
		Debug, // 테스트 필요시 남기는 로그
		MemoryPool, // Memory Pool 상태로그		
		Main, // Main Window 운영로그		
		Worker, // Memory Pool 상태로그		
		Grab, // Grab 상태로그		
		Vision, // Vision 상태로그
		Network, // Tcp, 시리얼 등 통신 상태 로그		
		DB, //DB 상태 로그		
		Inspect, // 검사 상태 로그			
		Config // 레시피 관련 파라미터 관련 로그(Load, Save, Result)		
	}

	public enum LogCategory
	{
		All,
		Info,
		Fatal,
		Warn,
		Error
	}


	[Flags]
	public enum MiniDumpType
	{
		MiniDumpNormal = 0x00000000,
		MiniDumpWithDataSegs = 0x00000001,
		MiniDumpWithFullMemory = 0x00000002,
	}
}
