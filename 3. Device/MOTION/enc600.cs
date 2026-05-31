using System;
using System.Runtime.InteropServices;

using HANDLE = System.IntPtr;

namespace OpenVisionLab
{
	public class Param
	{
		public const Byte YES = 1;
		public const Byte NO = 0;
		public const Byte ON = 1;
		public const Byte OFF = 0;

		public const Byte CW = 0;
		public const Byte CCW = 1;
		public const Byte X1_axis = 1;
		public const Byte X2_axis = 2;
		public const Byte X3_axis = 3;
		public const Byte X4_axis = 4;
		public const Byte X5_axis = 5;
		public const Byte X6_axis = 6;
    
		public const Byte ENC_QUADRANT = 0x00;
		public const Byte ENC_CW_CCW = 0x01;
		public const Byte ENC_PULSE_DIR = 0x02;
		public const Byte ENC_HR_RESET = 0x40;
		public const Byte INDEX_RESET = 0x80;
	}
	
	public class Functions
	{
		[DllImport("enc600.dll", EntryPoint = "ENC6_INITIAL")]
		public static extern HANDLE ENC6_INITIAL();
		
		[DllImport("enc600.dll", EntryPoint = "ENC6_END")]
		public static extern HANDLE ENC6_END();
		
		[DllImport("enc600.dll", EntryPoint = "ENC6_REGISTRATION")]
		public static extern Byte ENC6_REGISTRATION(
													Byte cardNo,
													UInt32 address
													);
												
		[DllImport("enc600.dll", EntryPoint = "ENC6_INIT_CARD")]
		public static extern HANDLE	ENC6_INIT_CARD(
                                                    Byte cardNo,
													Byte x1_mode,
													Byte x2_mode,
													Byte x3_mode,
													Byte x4_mode,
													Byte x5_mode,
													Byte x6_mode
													);
												
		[DllImport("enc600.dll", EntryPoint = "ENC6_CONFIG")]
		public static extern HANDLE	ENC6_CONFIG(
                                                Byte cardNo,
												Byte x1_mode,
												Byte x2_mode,
												Byte x3_mode,
												Byte x4_mode,
												Byte x5_mode,
												Byte x6_mode
												);
												
		[DllImport("enc600.dll", EntryPoint = "ENC6_GET_ENCODER")]
		public static extern Int32	ENC6_GET_ENCODER(
													Byte cardNo,
													Byte axis
													);

		[DllImport("enc600.dll", EntryPoint = "ENC6_RESET_ENCODER")]
		public static extern HANDLE	ENC6_RESET_ENCODER(
													    Byte cardNo,
												        Byte axis
												        );												
												
		[DllImport("enc600.dll", EntryPoint = "ENC6_GET_INDEX")]
		public static extern Byte	ENC6_GET_INDEX(
												    Byte cardNo,
												    Byte axis
												    );
												
		[DllImport("enc600.dll", EntryPoint = "ENC6_DO")]
		public static extern Byte	ENC6_DO(
											Byte cardNo,
											Byte value
											);																																		
	}
		
}