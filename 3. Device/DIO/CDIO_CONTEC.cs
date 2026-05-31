//============================================================-
//	CDIOCS.CS
//	Class file for CONTEC Digital I/O device
//												CONTEC.Co.,Ltd.
//============================================================-

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using static OpenVisionLab.CSignal;
using OpenVisionLab;
using Vila.Extensions;
using Lib.Common;

public enum CdioConst
{
    //-------------------------------------------------
    //	Type definition
    //-------------------------------------------------
    DEVICE_TYPE_ISA = 0,    //	ISA or C bus
    DEVICE_TYPE_PC = 1, //	PCI bus
    DEVICE_TYPE_PCMCIA = 2, //	PCMCIA
    DEVICE_TYPE_USB = 3,    //	USB
    DEVICE_TYPE_FIT = 4,    //	FIT
    DEVICE_TYPE_CARDBUS = 5,    //	CardBus
    DEVICE_TYPE_NET = 20,   //	Ethernet, Wireless etc

    //-------------------------------------------------
    //	Parameters
    //-------------------------------------------------
    //	I/O(for Sample)
    DIO_MAX_ACCS_PORTS = 256,
    //	DioNotifyInt:Logic
    DIO_INT_NONE = 0,
    DIO_INT_RISE = 1,
    DIO_INT_FALL = 2,
    //	DioNotifyTrg:TrgKind
    DIO_TRG_RISE = 1,
    DIO_TRG_FALL = 2,
    //	Message
    DIOM_INTERRUPT = 0x1300,
    DIOM_TRIGGER = 0x1340,
    DIO_DMM_STOP = 0x1350,
    DIO_DMM_COUNT = 0x1360,
    //	Device Information
    IDIO_DEVICE_TYPE = 0,   //	device type.							Param1:short
    IDIO_NUMBER_OF_8255 = 1,    //	Number of 8255 chip.					Param1:int
    IDIO_IS_8255_BOARD = 2, //	Is 8255	board?							Param1:BOOL(True/False)
    IDIO_NUMBER_OF_DI_BIT = 3,  //	Number of digital input bit.			Param1:int
    IDIO_NUMBER_OF_DO_BIT = 4,  //	Number of digital outout bit.			Param1:int
    IDIO_NUMBER_OF_DI_PORT = 5, //	Number of digital input	port.			Param1:int
    IDIO_NUMBER_OF_DO_PORT = 6, //	Number of digital output port.			Param1:int
    IDIO_IS_POSITIVE_LOGIC = 7, //	Is positive logic?						Param1:BOOL(True/False)
    IDIO_IS_ECHO_BACK = 8,  //	Can echo back output port?				Param1:BOOL(True/False)
    IDIO_IS_DIRECTION = 9,  //	Can DioSetIoDirection function be used?	Param1:int(1:true, 0:false)
    IDIO_IS_FILTER = 10,    //	Can digital filter be used?				Param1:int(1:true, 0:false)
    IDIO_NUMBER_OF_INT_BIT = 11,    //	Number of interrupt bit.				Param1:short

    //	DM
    //	Direction
    PI_32 = 1,
    PO_32 = 2,
    PIO_1616 = 3,
    DIODM_DIR_IN = 0x1,
    DIODM_DIR_OUT = 0x2,

    //  Start
    DIODM_START_SOFT = 1,
    DIODM_START_EXT_RISE = 2,
    DIODM_START_EXT_FALL = 3,
    DIODM_START_PATTERN = 4,
    DIODM_START_EXTSIG_1 = 5,
    DIODM_START_EXTSIG_2 = 6,
    DIODM_START_EXTSIG_3 = 7,

    //  Clock
    DIODM_CLK_CLOCK = 1,
    DIODM_CLK_EXT_TRG = 2,
    DIODM_CLK_HANDSHAKE = 3,
    DIODM_CLK_EXTSIG_1 = 4,
    DIODM_CLK_EXTSIG_2 = 5,
    DIODM_CLK_EXTSIG_3 = 6,

    //  Internal Clock
    DIODM_TIM_UNIT_S = 1,
    DIODM_TIM_UNIT_MS = 2,
    DIODM_TIM_UNIT_US = 3,
    DIODM_TIM_UNIT_NS = 4,

    //  Stop
    DIODM_STOP_SOFT = 1,
    DIODM_STOP_EXT_RISE = 2,
    DIODM_STOP_EXT_FALL = 3,
    DIODM_STOP_NUM = 4,
    DIODM_STOP_EXTSIG_1 = 5,
    DIODM_STOP_EXTSIG_2 = 6,
    DIODM_STOP_EXTSIG_3 = 7,

    //	ExtSig
    DIODM_EXT_START_SOFT_IN = 1,
    DIODM_EXT_STOP_SOFT_IN = 2,
    DIODM_EXT_CLOCK_IN = 3,
    DIODM_EXT_EXT_TRG_IN = 4,
    DIODM_EXT_START_EXT_RISE_IN = 5,
    DIODM_EXT_START_EXT_FALL_IN = 6,
    DIODM_EXT_START_PATTERN_IN = 7,
    DIODM_EXT_STOP_EXT_RISE_IN = 8,
    DIODM_EXT_STOP_EXT_FALL_IN = 9,
    DIODM_EXT_CLOCK_ERROR_IN = 10,
    DIODM_EXT_HANDSHAKE_IN = 11,
    DIODM_EXT_TRNSNUM_IN = 12,

    DIODM_EXT_START_SOFT_OUT = 101,
    DIODM_EXT_STOP_SOFT_OUT = 102,
    DIODM_EXT_CLOCK_OUT = 103,
    DIODM_EXT_EXT_TRG_OUT = 104,
    DIODM_EXT_START_EXT_RISE_OUT = 105,
    DIODM_EXT_START_EXT_FALL_OUT = 106,
    DIODM_EXT_STOP_EXT_RISE_OUT = 107,
    DIODM_EXT_STOP_EXT_FALL_OUT = 108,
    DIODM_EXT_CLOCK_ERROR_OUT = 109,
    DIODM_EXT_HANDSHAKE_OUT = 110,
    DIODM_EXT_TRNSNUM_OUT = 111,

    //	Status
    DIODM_STATUS_BMSTOP = 0x1,
    DIODM_STATUS_PIOSTART = 0x2,
    DIODM_STATUS_PIOSTOP = 0x4,
    DIODM_STATUS_TRGIN = 0x8,
    DIODM_STATUS_OVERRUN = 0x10,

    //	Error
    DIODM_STATUS_FIFOEMPTY = 0x1,
    DIODM_STATUS_FIFOFULL = 0x2,
    DIODM_STATUS_SGOVERIN = 0x4,
    DIODM_STATUS_TRGERR = 0x8,
    DIODM_STATUS_CLKERR = 0x10,
    DIODM_STATUS_SLAVEHALT = 0x20,
    DIODM_STATUS_MASTERHALT = 0x40,

    //	Reset
    DIODM_RESET_FIFO_IN = 0x02,
    DIODM_RESET_FIFO_OUT = 0x04,

    //	Buffer Ring
    DIODM_WRITE_ONCE = 0,
    DIODM_WRITE_RING = 1,

    //  NET
    DIONET_MODE_DIRECT = 0,
    DIONET_MODE_AP = 1,

    //  Counter
    DIO_COUNT_EDGE_UP = 1,
    DIO_COUNT_EDGE_DOWN = 2,

    //-------------------------------------------------
    //	Error	codes
    //-------------------------------------------------
    //	Initialize	Error
    //	Common
    DIO_ERR_SUCCESS = 0,        //	NORMAL completed
    DIO_ERR_INI_RESOURCE = 1,       //	invalid resource reference specified
    DIO_ERR_INI_INTERRUPT = 2,      //	invalid interrupt routine registered
    DIO_ERR_INI_MEMORY = 3,     //	invalid memory allocationed
    DIO_ERR_INI_REGISTRY = 4,       //	invalid registry accesse

    DIO_ERR_SYS_RECOVERED_FROM_STANDBY = 7,     //	Execute DioResetDevice function because the device has recovered from standby mode.
    DIO_ERR_INI_NOT_FOUND_SYS_FILE = 8,     //	Because the Cdio.sys file is not found, it is not possible to initialize it.
    DIO_ERR_INI_DLL_FILE_VERSION = 9,       //	Because version information on the Cdio.dll file cannot be acquired, it is not possible to initialize it.
    DIO_ERR_INI_SYS_FILE_VERSION = 10,      //	Because version information on the Cdio.sys file cannot be acquired, it is not possible to initialize it.
    DIO_ERR_INI_NO_MATCH_DRV_VERSION = 11,      //	Because version information on Cdio.dll and Cdio.sys is different, it is not possible to initialize it.

    //	DLL	Error
    //	Common
    DIO_ERR_DLL_DEVICE_NAME = 10000,    //	invalid device name specified.
    DIO_ERR_DLL_INVALID_ID = 10001, //	invalid ID specified.
    DIO_ERR_DLL_CALL_DRIVER = 10002,    //	not call the driver.(Invalid device I/O controller)
    DIO_ERR_DLL_CREATE_FILE = 10003,    //	not create the file.(Invalid CreateFile)
    DIO_ERR_DLL_CLOSE_FILE = 10004, //	not close the file.(Invalid CloseFile)
    DIO_ERR_DLL_CREATE_THREAD = 10005,  //	not create the thread.(Invalid CreateThread)
    DIO_ERR_INFO_INVALID_DEVICE = 10050,    //	invalid device infomation specified .Please check the spell.
    DIO_ERR_INFO_NOT_FIND_DEVICE = 10051,   //	not find the available device
    DIO_ERR_INFO_INVALID_INFOTYPE = 10052,  //	specified device infomation type beyond the limit

    //	DIO
    DIO_ERR_DLL_BUFF_ADDRESS = 10100,   //	invalid data buffer address
    DIO_ERR_DLL_HWND = 10200,   //	window handle beyond the limit
    DIO_ERR_DLL_TRG_KIND = 10300,   //	trigger kind beyond the limit

    //	SYS	Error
    //	Common
    DIO_ERR_SYS_MEMORY = 20000, //	not secure memory
    DIO_ERR_SYS_NOT_SUPPORTED = 20001,  //	this board couldn't use this function
    DIO_ERR_SYS_BOARD_EXECUTING = 20002,    //	board is behaving, not execute
    DIO_ERR_SYS_USING_OTHER_PROCESS = 20003,    //	other process is using the device, not execute

    STATUS_SYS_USB_CRC = 20020, //	the last data packet received from end point exist CRC error
    STATUS_SYS_USB_BTSTUFF = 20021, //	the last data packet received from end point exist bit stuffing offense error
    STATUS_SYS_USB_DATA_TOGGLE_MISMATCH = 20022,    //	the last data packet received from end point exist toggle packet mismatch error
    STATUS_SYS_USB_STALL_PID = 20023,   //	end point return STALL packet identifier
    STATUS_SYS_USB_DEV_NOT_RESPONDING = 20024,  //	device don't respond to token(IN), don't support handshake
    STATUS_SYS_USB_PID_CHECK_FAILURE = 20025,
    STATUS_SYS_USB_UNEXPECTED_PID = 20026,  //	invalid packet identifier received
    STATUS_SYS_USB_DATA_OVERRUN = 20027,    //	end point return data quantity overrun
    STATUS_SYS_USB_DATA_UNDERRUN = 20028,   //	end point return data quantity underrun
    STATUS_SYS_USB_BUFFER_OVERRUN = 20029,  //	IN transmit specified buffer overrun
    STATUS_SYS_USB_BUFFER_UNDERRUN = 20030, //	OUT transmit specified buffer underrun
    STATUS_SYS_USB_ENDPOINT_HALTED = 20031, //	end point status is STALL, not transmit
    STATUS_SYS_USB_NOT_FOUND_DEVINFO = 20032,   //	not found device infomation
    STATUS_SYS_USB_ACCESS_DENIED = 20033,   //	Access denied
    STATUS_SYS_USB_INVALID_HANDLE = 20034,  //	Invalid handle

    //	DIO
    DIO_ERR_SYS_PORT_NO = 20100,    //	board No. beyond the limit
    DIO_ERR_SYS_PORT_NUM = 20101,   //	board number beyond the limit
    DIO_ERR_SYS_BIT_NO = 20102, //	bit No. beyond the limit
    DIO_ERR_SYS_BIT_NUM = 20103,    //	bit number beyond the limit
    DIO_ERR_SYS_BIT_DATA = 20104,   //	bit data beyond the limit of 0 to 1
    DIO_ERR_SYS_INT_BIT = 20200,    //	interrupt bit beyond the limit
    DIO_ERR_SYS_INT_LOGIC = 20201,  //	interrupt logic beyond the limit
    DIO_ERR_SYS_TIM = 20300,    //	timer value beyond the limit
    DIO_ERR_SYS_FILTER = 20400, //	filter number beyond the limit
    DIO_ERR_SYS_IODIRECTION = 20500,    //	Direction value is out of range

    //  DM
    DIO_ERR_SYS_SIGNAL = 21000, //	Usable signal is outside the setting range.
    DIO_ERR_SYS_START = 21001,  //	Usable start conditions are outside the setting range.
    DIO_ERR_SYS_CLOCK = 21002,  //	Clock conditions are outside the setting range.
    DIO_ERR_SYS_CLOCK_VAL = 21003,  //	Clock value is outside the setting range.
    DIO_ERR_SYS_CLOCK_UNIT = 21004, //	Clock value unit is outside the setting range.
    DIO_ERR_SYS_STOP = 21005,   //	Stop conditions are outside the setting range.
    DIO_ERR_SYS_STOP_NUM = 21006,   //	Stop number is outside the setting range.
    DIO_ERR_SYS_RESET = 21007,  //	Contents of reset are outside the setting range.
    DIO_ERR_SYS_LEN = 21008,    //	Data number is outside the setting range.
    DIO_ERR_SYS_RING = 21009,   //	Buffer repetition use setup is outside the setting range.
    DIO_ERR_SYS_COUNT = 21010,  //	Data transmission number is outside the setting range.
    DIO_ERR_DM_BUFFER = 21100,  //	Buffer was too large and has not secured.
    DIO_ERR_DM_LOCK_MEMORY = 21101, //	Memory has not been locked.
    DIO_ERR_DM_PARAM = 21102,   //	Parameter error
    DIO_ERR_DM_SEQUENCE = 21103,    //	Procedure error of execution

    // NET
    DIO_ERR_NET_BASE = 22000,   //	Access error
    DIO_ERR_NET_ACCESS = 22001, //	Access violation
    DIO_ERR_NET_AREA = 22002,   //	Area error
    DIO_ERR_NET_SIZE = 22003,   //	Access size error
    DIO_ERR_NET_PARAMETER = 22004,  //	Parameter error
    DIO_ERR_NET_LENGTH = 22005, //	Length error
    DIO_ERR_NET_RESOURCE = 22006,   //	Insufficient resources
    DIO_ERR_NET_TIMEOUT = 22016,    //	Communications timeout
    DIO_ERR_NET_HANDLE = 22017, //	Handle error
    DIO_ERR_NET_CLOSE = 22018,  //	Close error
    DIO_ERR_NET_TIMEOUT_WIO = 22064	//	Wireless communications timeout
}

namespace OpenVisionLab.Common
{
    unsafe public delegate void PINTCALLBACK(short Id, int wParam, int lParam, void* Param);
    unsafe public delegate void PTRGCALLBACK(short Id, int wParam, int lParam, void* Param);
    unsafe public delegate void PDMCOUNTCALLBACK(short Id, int wParam, int lParam, void* Param);
    unsafe public delegate void PDMSTOPCALLBACK(short Id, int wParam, int lParam, void* Param);

    public class CDIO_CONTEC
    {
        // Definition of common functions
        [DllImport("cdio.dll")] static extern int DioInit(string DeviceName, ref short Id);
        [DllImport("cdio.dll")] static extern int DioExit(short Id);
        [DllImport("cdio.dll")] static extern int DioResetDevice(short Id);
        [DllImport("cdio.dll")] static extern int DioGetErrorString(int ErrorCode, System.Text.StringBuilder ErrorString);

        // Digital filter functions
        [DllImport("cdio.dll")] static extern int DioSetDigitalFilter(short Id, short FilterValue);
        [DllImport("cdio.dll")] static extern int DioGetDigitalFilter(short Id, ref short FilterValue);

        // I/O Direction functions
        [DllImport("cdio.dll")] static extern int DioSetIoDirection(short Id, uint dwDir);
        [DllImport("cdio.dll")] static extern int DioGetIoDirection(short Id, ref uint dwDir);
        [DllImport("cdio.dll")] static extern int DioSetIoDirectionEx(short Id, uint dwDir);
        [DllImport("cdio.dll")] static extern int DioGetIoDirectionEx(short Id, ref uint dwDir);
        [DllImport("cdio.dll")] static extern int DioSet8255Mode(short Id, short ChipNo, short CtrlWord);
        [DllImport("cdio.dll")] static extern int DioGet8255Mode(short Id, short ChipNo, ref short CtrlWord);

        // Simple I/O functions
        [DllImport("cdio.dll")] static extern int DioInpByte(short Id, short PortNo, ref byte Data);
        [DllImport("cdio.dll")] static extern int DioInpBit(short Id, short BitNo, ref byte Data);
        [DllImport("cdio.dll")] static extern int DioInpByteSR(short Id, short PortNo, ref byte Data, ref ushort Timestanp, byte Mode);
        [DllImport("cdio.dll")] static extern int DioInpBitSR(short Id, short BitNo, ref byte Data, ref ushort Timestanp, byte Mode);
        [DllImport("cdio.dll")] static extern int DioOutByte(short Id, short PortNo, byte Data);
        [DllImport("cdio.dll")] static extern int DioOutBit(short Id, short BitNo, byte Data);
        [DllImport("cdio.dll")] static extern int DioEchoBackByte(short Id, short PortNo, ref byte Data);
        [DllImport("cdio.dll")] static extern int DioEchoBackBit(short Id, short BitNo, ref byte Data);

        // Multiple I/O functions
        [DllImport("cdio.dll")] static extern int DioInpMultiByte(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] PortNo, short PortNum, [MarshalAs(UnmanagedType.LPArray)] byte[] Data);
        [DllImport("cdio.dll")] static extern int DioInpMultiBit(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] BitNo, short BitNum, [MarshalAs(UnmanagedType.LPArray)] byte[] Data);
        [DllImport("cdio.dll")] static extern int DioInpMultiByteSR(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] PortNo, short PortNum, [MarshalAs(UnmanagedType.LPArray)] byte[] Data, ref ushort Timestanp, byte Mode);
        [DllImport("cdio.dll")] static extern int DioInpMultiBitSR(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] BitNo, short BitNum, [MarshalAs(UnmanagedType.LPArray)] byte[] Data, ref ushort Timestanp, byte Mode);
        [DllImport("cdio.dll")] static extern int DioOutMultiByte(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] PortNo, short PortNum, [MarshalAs(UnmanagedType.LPArray)] byte[] Data);
        [DllImport("cdio.dll")] static extern int DioOutMultiBit(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] BitNo, short BitNum, [MarshalAs(UnmanagedType.LPArray)] byte[] Data);
        [DllImport("cdio.dll")] static extern int DioEchoBackMultiByte(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] PortNo, short PortNum, [MarshalAs(UnmanagedType.LPArray)] byte[] Data);
        [DllImport("cdio.dll")] static extern int DioEchoBackMultiBit(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] BitNo, short BitNum, [MarshalAs(UnmanagedType.LPArray)] byte[] Data);

        // Interrupt functions
        [DllImport("cdio.dll")] static extern int DioNotifyInterrupt(short Id, short IntBit, short Logic, int hWnd);
        [DllImport("cdio.dll")] unsafe static extern int DioSetInterruptCallBackProc(short Id, IntPtr pIntCallBack, void* Param);

        // Trigger functions
        [DllImport("cdio.dll")] static extern int DioNotifyTrg(short Id, short TrgBit, short TrgKind, int Tim, int hWnd);
        [DllImport("cdio.dll")] static extern int DioStopNotifyTrg(short Id, short TrgBit);
        [DllImport("cdio.dll")] static extern int DioSetTrgCallBackProc(short Id, IntPtr CallBackProc, ref int Param);

        // Information functions
        [DllImport("cdio.dll")] static extern int DioGetDeviceInfo(string Device, short InfoType, ref int Param1, ref int Param2, ref int Param3);
        [DllImport("cdio.dll")] static extern int DioQueryDeviceName(short Index, System.Text.StringBuilder DeviceName, System.Text.StringBuilder Device);
        [DllImport("cdio.dll")] static extern int DioGetDeviceType(string Device, ref short DeviceType);
        [DllImport("cdio.dll")] static extern int DioGetMaxPorts(short Id, ref short InPortNum, ref short OutPortNum);
        [DllImport("cdio.dll")] static extern int DioGetMaxCountChannels(short Id, ref short ChannelNum);

        // Counter function        
        [DllImport("cdio.dll")] static extern int DioSetCountEdge(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] ChNo, short ChNum, [MarshalAs(UnmanagedType.LPArray)] short[] CountEdge);
        [DllImport("cdio.dll")] static extern int DioGetCountEdge(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] ChNo, short ChNum, [MarshalAs(UnmanagedType.LPArray)] short[] CountEdge);
        [DllImport("cdio.dll")] static extern int DioSetCountMatchValue(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] ChNo, short ChNum, [MarshalAs(UnmanagedType.LPArray)] short[] CompareRegNo, [MarshalAs(UnmanagedType.LPArray)] uint[] CompareCount);
        [DllImport("cdio.dll")] static extern int DioStartCount(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] ChNo, short ChNum);
        [DllImport("cdio.dll")] static extern int DioStopCount(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] ChNo, short ChNum);
        [DllImport("cdio.dll")] static extern int DioGetCountStatus(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] ChNo, short ChNum, [MarshalAs(UnmanagedType.LPArray)] uint[] CountStatus);
        [DllImport("cdio.dll")] static extern int DioCountPreset(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] ChNo, short ChNum, [MarshalAs(UnmanagedType.LPArray)] uint[] PresetCount);
        [DllImport("cdio.dll")] static extern int DioReadCount(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] ChNo, short ChNum, [MarshalAs(UnmanagedType.LPArray)] uint[] Count);
        [DllImport("cdio.dll")] static extern int DioReadCountSR(short Id, [MarshalAs(UnmanagedType.LPArray)] short[] ChNo, short ChNum, [MarshalAs(UnmanagedType.LPArray)] uint[] Count, ref ushort Timestanp, byte Mode);

        //dm functions
        [DllImport("cdio.DLL")] static extern int DioDmSetDirection(short Id, short Direction);
        [DllImport("cdio.DLL")] static extern int DioDmGetDirection(short Id, ref short Direction);
        [DllImport("cdio.DLL")] static extern int DioDmSetStandAlone(short Id);
        [DllImport("cdio.DLL")] static extern int DioDmSetMaster(short Id, short ExtSig1, short ExtSig2, short ExtSig3, short MasterHalt, short SlaveHalt);
        [DllImport("cdio.DLL")] static extern int DioDmSetSlave(short Id, short ExtSig1, short ExtSig2, short ExtSig3, short MasterHalt, short SlaveHalt);
        [DllImport("cdio.DLL")] static extern int DioDmSetStartTrigger(short Id, short Direction, short Start);
        [DllImport("cdio.DLL")] static extern int DioDmSetStartPattern(short Id, uint Pattern, uint Mask);
        [DllImport("cdio.DLL")] static extern int DioDmSetClockTrigger(short Id, short Direction, short Clock);
        [DllImport("cdio.DLL")] static extern int DioDmSetInternalClock(short Id, short Direction, uint Clock, short Unit);
        [DllImport("cdio.DLL")] static extern int DioDmSetStopTrigger(short Id, short Direction, short Stop);
        [DllImport("cdio.DLL")] static extern int DioDmSetStopNumber(short Id, short Direction, uint StopNumber);
        [DllImport("cdio.DLL")] static extern int DioDmFifoReset(short Id, short Reset);
        [DllImport("cdio.DLL")] static extern int DioDmSetBuffer(short Id, short Direction, IntPtr Buffer, uint Length, short IsRing);
        [DllImport("cdio.DLL")] static extern int DioDmSetTransferStartWait(short Id, short Time);
        [DllImport("cdio.DLL")] static extern int DioDmTransferStart(short Id, short Direction);
        [DllImport("cdio.DLL")] static extern int DioDmTransferStop(short Id, short Direction);
        [DllImport("cdio.DLL")] static extern int DioDmGetStatus(short Id, short Direction, ref uint Status, ref uint Err);
        [DllImport("cdio.DLL")] static extern int DioDmGetCount(short Id, short Direction, ref uint Count, ref uint Carry);
        [DllImport("cdio.DLL")] static extern int DioDmGetWritePointer(short Id, short Direction, ref uint WritePointer, ref uint Count, ref uint Carry);
        [DllImport("cdio.DLL")] static extern int DioDmSetStopEvent(short Id, short Direction, int hWnd);
        [DllImport("cdio.DLL")] unsafe static extern int DioDmSetStopCallBackProc(short Id, IntPtr CallBackProc, void* Param);
        [DllImport("cdio.DLL")] static extern int DioDmSetCountEvent(short Id, short Direction, uint Count, int hWnd);
        [DllImport("cdio.DLL")] unsafe static extern int DioDmSetCountCallBackProc(short Id, IntPtr CallBackProc, void* Param);

        // Demo Device I/O functions
        [DllImport("cdio.dll")] static extern int DioSetDemoByte(short Id, short PortNo, byte Data);
        [DllImport("cdio.dll")] static extern int DioSetDemoBit(short Id, short BitNo, byte Data);

        [DllImport("cdio.dll")] static extern int DioResetPatternEvent(short Id, [MarshalAs(UnmanagedType.LPArray)] byte[] Data);
        [DllImport("cdio.dll")] static extern int DioGetPatternEventStatus(short Id, ref short Status);

        //public IData iData
        //{
        //    get;
        //    set;
        //}
        //public bool IsOpen { get; set; } = false;

        public static List<CSignal> Inputs { get; set; } = new List<CSignal>();
        public static List<CSignal> Outputs { get; set; } = new List<CSignal>();

        private short m_shBoardId = 0;
        public short BoardId
        {
            get { return m_shBoardId; }
            set { m_shBoardId = value; }
        }

        private bool m_bIsAlive = false;
        public bool IsAlive
        {
            get { return m_bIsAlive; }
            set { m_bIsAlive = value; }
        }

        // Constructor
        public CDIO_CONTEC()
        {

        }


        #region IO
        public CSignal InputEndoer = null;
        public CSignal InputSensing = null;
        public CSignal InputSensing2 = null;

        public CSignal DI_00_Encoder_상승 = null;
        public CSignal DI_01_Encoder_하강 = null;
        public CSignal DI_02_Label1_Ready = null;
        public CSignal DI_03_Label1_Error = null;
        public CSignal DI_04_Label1_Return = null;
        public CSignal DI_05_Label2_Ready = null;
        public CSignal DI_06_Label2_Error = null;
        public CSignal DI_07_Label2_Return = null;
        public CSignal DI_08_Label3_Ready = null;
        public CSignal DI_09_Label3_Error = null;
        public CSignal DI_10_Label3_Return = null;
        public CSignal DI_11_Label4_Ready = null;
        public CSignal DI_12_Label4_Error = null;
        public CSignal DI_13_Label4_Return = null;
        public CSignal DI_14_Test15 = null;
        public CSignal DI_15_Test16 = null;


        public CSignal DO_00_Encoder_OnOff = null;
        public CSignal DO_01_Test2 = null;
        public CSignal DO_02_Label1_Start = null;
        public CSignal DO_03_Label1_Reset = null;
        public CSignal DO_04_Label2_Start = null;
        public CSignal DO_05_Label2_Reset = null;
        public CSignal DO_06_Label3_Start = null;
        public CSignal DO_07_Label3_Reset = null;
        public CSignal DO_08_Label4_Start = null;
        public CSignal DO_09_Label4_Reset = null;
        public CSignal DO_10_Test3 = null;
        public CSignal DO_11_Test4 = null;
        public CSignal DO_12_Test5 = null;
        public CSignal DO_13_Test6 = null;
        public CSignal DO_14_Test7 = null;
        public CSignal DO_15_Test8 = null;

        #endregion

        public bool InitIO()
        {
            try
            {
                Inputs.Clear();
                Outputs.Clear();
                DI_00_Encoder_상승 = new CSignal("SYSTEM", "DI_00_Encoder_상승", "0");
                DI_01_Encoder_하강 = new CSignal("SYSTEM", "DI_01_Encoder_하강", "1");
                DI_02_Label1_Ready = new CSignal("SYSTEM", "DI_02_Label1_Ready", "2");
                DI_03_Label1_Error = new CSignal("SYSTEM", "DI_03_Label1_Error", "3");
                DI_04_Label1_Return = new CSignal("SYSTEM", "DI_04_Label1_Return", "4");
                DI_05_Label2_Ready = new CSignal("SYSTEM", "DI_05_Label2_Ready", "5");
                DI_06_Label2_Error = new CSignal("SYSTEM", "DI_06_Label2_Error", "6");
                DI_07_Label2_Return = new CSignal("SYSTEM", "DI_07_Label2_Return", "7");
                DI_08_Label3_Ready = new CSignal("SYSTEM", "DI_08_Label3_Ready", "8");
                DI_09_Label3_Error = new CSignal("SYSTEM", "DI_09_Label3_Error", "9");
                DI_10_Label3_Return = new CSignal("SYSTEM", "DI_10_Label3_Return", "10");
                DI_11_Label4_Ready = new CSignal("SYSTEM", "DI_11_Label4_Ready", "11");
                DI_12_Label4_Error = new CSignal("SYSTEM", "DI_12_Label4_Error", "12");
                DI_13_Label4_Return = new CSignal("SYSTEM", "DI_13_Label4_Return", "13");
                DI_14_Test15 = new CSignal("SYSTEM", "DI_14_Test15", "14");
                DI_15_Test16 = new CSignal("SYSTEM", "DI_15_Test16", "15");


                DO_00_Encoder_OnOff = new CSignal("SYSTEM", "DO_00_Encoder_OnOff", "0");
                DO_01_Test2 = new CSignal("SYSTEM", "DO_01_Test2", "1");
                DO_02_Label1_Start = new CSignal("SYSTEM", "DO_02_Label1_Start", "2");
                DO_03_Label1_Reset = new CSignal("SYSTEM", "DO_03_Label1_Reset", "3");
                DO_04_Label2_Start = new CSignal("SYSTEM", "DO_04_Label2_Start", "4");
                DO_05_Label2_Reset = new CSignal("SYSTEM", "DO_05_Label2_Reset", "5");
                DO_06_Label3_Start = new CSignal("SYSTEM", "DO_06_Label3_Start", "6");
                DO_07_Label3_Reset = new CSignal("SYSTEM", "DO_07_Label3_Reset", "7");
                DO_08_Label4_Start = new CSignal("SYSTEM", "DO_08_Label4_Start", "8");
                DO_09_Label4_Reset = new CSignal("SYSTEM", "DO_09_Label4_Reset", "9");
                DO_10_Test3 = new CSignal("SYSTEM", "DO_10_Test3", "10");
                DO_11_Test4 = new CSignal("SYSTEM", "DO_11_Test4", "11");
                DO_12_Test5 = new CSignal("SYSTEM", "DO_12_Test5", "12");
                DO_13_Test6 = new CSignal("SYSTEM", "DO_13_Test6", "13");
                DO_14_Test7 = new CSignal("SYSTEM", "DO_14_Test7", "14");
                DO_15_Test8 = new CSignal("SYSTEM", "DO_15_Test8", "15");

                StartThreadRead();

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return false;
            }
        }


        #region Thread
        private CThreadStatus m_ThreadStatusRead = new CThreadStatus();
        public CThreadStatus ThreadStatusRead
        {
            get { return m_ThreadStatusRead; }
        }

        private void StartThreadRead()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadRead));
            t.Priority = ThreadPriority.Highest;
            t.Start(m_ThreadStatusRead);
        }

        public void StopThreadRead()
        {
            if (!ThreadStatusRead.IsExit())
            {
                ThreadStatusRead.Stop(100);
            }
        }

        int m_nAliveTime = 0;
        private void ThreadRead(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("Read Io");
            CLOG.NORMAL("Read the Io Signal");

            try
            {
                while (!ThreadStatus.IsExit())
                {
                    if ((Environment.TickCount - m_nAliveTime) > 1000)
                    {
                        m_nAliveTime = Environment.TickCount;
                    }

                    Thread.Sleep(1);

                    try
                    {
                        if (!IsAlive) { continue; }

                        // Input
                        for (int nIndex = 0; nIndex < Inputs.Count; nIndex++)
                        {
                            GetInputBit(Inputs[nIndex]);
                        }

                        // Output
                        for (int nIndex = 0; nIndex < Outputs.Count; nIndex++)
                        {
                            GetOutputBit(Outputs[nIndex]);
                        }

                    }
                    catch (Exception Desc)
                    {
                        CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
            }

        }
        #endregion

        /// <summary>
        /// IO InitialLize를 진행합니다. 
        /// </summary>
        /// <param name="DeviceName">장치관리자에 등록된 드라이버 ID</param>
        /// <param name="Id">이 ID로 각종 메서드 작동</param>
        /// <returns></returns>
        // Description of common functions
        public bool Init(string DeviceName)
        {

            short shId = 1;
            try
            {
                InitIO();

                int ret = DioInit(DeviceName, ref shId);
                m_shBoardId = shId;

                string strErr = "";
                if (ret != (int)CdioConst.DIO_ERR_SUCCESS)
                {
                    IsAlive = false;
                    GetErrorString(ret, out strErr);
                    CLOG.ABNORMAL($"[ERROR] Ret = {ret}, Reason = {strErr} ");
                    
                    return false;
                }
                else
                {
                    IsAlive = true;
                }                

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");                
                return false;
            }
        }

        /// <summary>
        /// 연결된 IO를 종료시킵니다.
        /// </summary>
        /// <param name="Id">INIT시 GET한 ID</param>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                Off(DO_00_Encoder_OnOff);
                StopThreadRead();

                if (IsAlive)
                {
                    int ret = DioExit(m_shBoardId);
                    string strErr = "";
                    if (ret != (int)CdioConst.DIO_ERR_SUCCESS)
                    {
                        GetErrorString(ret, out strErr);
                        CLOG.ABNORMAL($"[ERROR] Ret = {ret}, Reason = {strErr} ");
                        return false;
                    }
                    else
                    {
                        IsAlive = false;
                    }
                }
                else
                {
                    CLOG.ABNORMAL("[Faile] Not Open DIO");
                    return false;
                }                

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Exception ==> {Desc.Message}");
                return false;
            }

        }

        public void Off(CSignal signal)
        {
            OutBitOnOff(signal, false);
        }

        public void On(CSignal signal)
        {
            OutBitOnOff(signal, true);
        }

        /// <summary>
        /// Output Bit 1개를 On/Off시킵니다.
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="bIsOnOff"></param>
        /// <returns></returns>
        public bool OutBitOnOff(CSignal signal, bool bIsOnOff)
        {
            try
            {
                if (signal == null) { return false; }
                byte btData = 0;
                short n = short.Parse(signal.Address);
                if (bIsOnOff)
                {
                    btData = 1;
                }
                else
                {
                    btData = 0;
                }

                if (IsAlive)
                {
                    int ret = DioOutBit(m_shBoardId, n, btData);
                    string strErr = "";
                    if (ret != (int)CdioConst.DIO_ERR_SUCCESS)
                    {
                        GetErrorString(ret, out strErr);
                        CLOG.ABNORMAL($"[ERROR] Ret = {ret}, Reason = {strErr} ");
                        return false;
                    }

                    signal.Current = (int)(CSignal.SIGNAL_TYPE)btData;
                }
                else
                {
                    CLOG.ABNORMAL("[Faile] Not Open DIO");
                    return false;
                }

                //CLogger.Add(LOG.IO, "[{0}] {1} ==> {2}", signal.Name, btData.ToString() == "1" ? "0" : "OFF", "ON");
                //CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }
        }

        /// <summary>
        /// 현재 DIO보드에 적용된 Output비트 상태를 확인합니다. 
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public bool GetOutputBit(CSignal signal)
        {
            try
            {
                byte btData = 0;
                short n = short.Parse(signal.Address);
                if (IsAlive)
                {
                    int ret = DioEchoBackBit(m_shBoardId, n, ref btData);
                    string strErr = "";
                    if (ret != (int)CdioConst.DIO_ERR_SUCCESS)
                    {
                        GetErrorString(ret, out strErr);
                        CLOG.ABNORMAL($"[ERROR] Ret = {ret}, Reason = {strErr} ");
                        return false;
                    }

                    signal.Current = (int)(CSignal.SIGNAL_TYPE)btData;
                }
                else
                {
                    CLOG.ABNORMAL("[Faile] Not Open DIO");
                    return false;
                }

                //CLogger.Add(LOG.IO, "[{0}] {1} ==> {2}", signal.Name, btData.ToString() == "1" ? "0" : "OFF", "ON");
                //CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }

        }

        /// <summary>
        /// 현재 DIO보드에 적용된 Input비트 상태를 확인합니다. 
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public bool GetInputBit(CSignal signal)
        {
            try
            {
                if (IsAlive)
                {
                    byte btData = 0;
                    short n = short.Parse(signal.Address);
                    int ret = DioInpBit(m_shBoardId, n, ref btData);
                    string strErr = "";
                    if (ret != (int)CdioConst.DIO_ERR_SUCCESS)
                    {
                        GetErrorString(ret, out strErr);
                        CLOG.ABNORMAL($"[ERROR] Ret = {ret}, Reason = {strErr} ");
                        return false;
                    }

                    signal.Current = (int)(CSignal.SIGNAL_TYPE)btData;
                }
                else
                {
                    CLOG.ABNORMAL("[Faile] Not Open DIO");
                    return false;
                }
                //CLogger.Add(LOG.IO, "[{0}] {1} ==> {2}", signal.Name, btData.ToString() == "1" ? "0" : "OFF", "ON");
                //CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool GetIsOn(CSignal signal)
        {
            try
            {
                if (IsAlive)
                {
                    byte btData = 0;
                    short n = short.Parse(signal.Address);

                    int ret = DioInpBit(m_shBoardId, n, ref btData);
                    string strErr = "";
                    if (ret != (int)CdioConst.DIO_ERR_SUCCESS)
                    {
                        GetErrorString(ret, out strErr);
                        CLOG.ABNORMAL($"[ERROR] Ret = {ret}, Reason = {strErr} ");
                        return false;
                    }

                    signal.Current = (int)(CSignal.SIGNAL_TYPE)btData;
                    //확인
                    //if (signal.Current == SIGNAL_TYPE.ON) return true;
                    //else return false;

                }
                else
                {
                    CLOG.ABNORMAL("[Faile] Not Open DIO");
                    return false;
                }
                //CLogger.Add(LOG.IO, "[{0}] {1} ==> {2}", signal.Name, btData.ToString() == "1" ? "0" : "OFF", "ON");
                //CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }
        }

        /// <summary>
        /// 현재 메서드 실행시 오류가 발생하면 어떠한 오류가 발생한 것인지 상세하게 확인하기 위해 존재
        /// </summary>
        /// <param name="ErrorCode"></param>
        /// <param name="ErrorString"></param>
        /// <returns></returns>
        public bool GetErrorString(int ErrorCode, out string ErrorString)
        {
            ErrorString = new String('0', 1);
            try
            {
                System.Text.StringBuilder errorstring = new System.Text.StringBuilder(256);
                int ret = DioGetErrorString(ErrorCode, errorstring);
                if (ret == (int)CdioConst.DIO_ERR_SUCCESS)
                {
                    ErrorString = errorstring.ToString();
                }
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool ResetDeviceIO()
        {
            try
            {
                if (IsAlive)
                {
                    int ret = DioResetDevice(m_shBoardId);
                    string strErr = "";
                    if (ret != (int)CdioConst.DIO_ERR_SUCCESS)
                    {
                        GetErrorString(ret, out strErr);
                        CLOG.ABNORMAL($"[ERROR] Ret = {ret}, Reason = {strErr} ");
                        return false;
                    }
                }
                else
                {
                    CLOG.ABNORMAL("[Faile] Not Open DIO");
                    return false;
                }

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }

        }

        public bool InpBitSR(CSignal signal)
        {
            try
            {
                if (IsAlive)
                {
                    byte btData = 0;
                    ushort Timestanp = 0;
                    byte btMode = 0;
                    short n = short.Parse(signal.Address);
                    int ret = DioInpBitSR(m_shBoardId, n, ref btData, ref Timestanp, btMode);
                    string strErr = "";
                    if (ret != (int)CdioConst.DIO_ERR_SUCCESS)
                    {
                        GetErrorString(ret, out strErr);
                        CLOG.ABNORMAL($"[ERROR] Ret = {ret}, Reason = {strErr} ");
                        return false;
                    }

                    signal.Current = (int)(CSignal.SIGNAL_TYPE)btData;
                }
                else
                {
                    CLOG.ABNORMAL("[Faile] Not Open DIO");
                    return false;
                }
                //CLogger.Add(LOG.IO, "[{0}] {1} ==> {2}", signal.Name, btData.ToString() == "1" ? "0" : "OFF", "ON");
                //CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }
        }


        //public bool InpBitOnOff(ISignal signal, bool bIsOnOff)
        //{
        //    byte btData = 0;
        //    if (bIsOnOff)
        //    {
        //        btData = 1;
        //    }
        //    else
        //    {
        //        btData = 0;
        //    }

        //    try
        //    {
        //        if (IsAlive)
        //        {                    
        //            int ret = DioOutBit(m_shBoardId, signal.Address, btData);

        //            string strErr = "";
        //            if (ret != (int)CdioConst.DIO_ERR_SUCCESS)
        //            {
        //                GetErrorString(ret, out strErr);
        //                CLOG.NORMAL("[OK] Ret = {0}, Reason = {1} ", ret, strErr);
        //                return false;
        //            }

        //            signal.Current = btData;
        //        }
        //        else
        //        {
        //            CLOG.NORMAL("[Faile] Not Open DIO");
        //            return false;
        //        }
        //        CLogger.Add(LOG.IO, "[{0}] {1} ==> {2}", signal.Name, btData.ToString() == "1" ? "0" : "OFF", "ON");
        //        CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
        //        return true;
        //    }
        //    catch (Exception Desc)
        //    {
        //        CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
        //        return false;
        //    }
        //}


        #region 추후 개발
        // Digital filter functions
        public int SetDigitalFilter(short Id, short FilterValue)
        {
            int ret = DioSetDigitalFilter(Id, FilterValue);
            return ret;
        }

        public int GetDigitalFilter(short Id, out short FilterValue)
        {
            FilterValue = 0;
            int ret = DioGetDigitalFilter(Id, ref FilterValue);
            return ret;
        }

        // I/O Direction functions
        public int SetIoDirection(short Id, uint dwDir)
        {
            int ret = DioSetIoDirection(Id, dwDir);
            return ret;
        }

        public int GetIoDirection(short Id, out uint dwDir)
        {
            dwDir = 0;
            int ret = DioGetIoDirection(Id, ref dwDir);
            return ret;
        }

        public int Set8255Mode(short Id, short ChipNo, short CtrlWord)
        {
            int ret = DioSet8255Mode(Id, ChipNo, CtrlWord);
            return ret;
        }

        public int Get8255Mode(short Id, short ChipNo, out short CtrlWord)
        {
            CtrlWord = 0;
            int ret = DioGet8255Mode(Id, ChipNo, ref CtrlWord);
            return ret;
        }

        public int SetIoDirectionEx(short Id, uint dwDir)
        {
            int ret = DioSetIoDirectionEx(Id, dwDir);
            return ret;
        }

        public int GetIoDirectionEx(short Id, out uint dwDir)
        {
            dwDir = 0;
            int ret = DioGetIoDirectionEx(Id, ref dwDir);
            return ret;
        }

        // Simple I/O functions
        public int InpByte(short Id, short PortNo, out byte Data)
        {
            Data = 0;
            int ret = DioInpByte(Id, PortNo, ref Data);
            return ret;
        }

        public int InpByteSR(short Id, short PortNo, out byte Data, out ushort Timestanp, byte Mode)
        {
            Data = 0;
            Timestanp = 0;
            int ret = DioInpByteSR(Id, PortNo, ref Data, ref Timestanp, Mode);
            return ret;
        }


        public int OutByte(short Id, short PortNo, byte Data)
        {
            int ret = DioOutByte(Id, PortNo, Data);
            return ret;
        }

        public int EchoBackByte(short Id, short PortNo, out byte Data)
        {
            Data = 0;
            int ret = DioEchoBackByte(Id, PortNo, ref Data);
            return ret;
        }

        // Multiple I/O functions
        public int InpMultiByte(short Id, short[] PortNo, short PortNum, byte[] Data)
        {
            int ret = DioInpMultiByte(Id, PortNo, PortNum, Data);
            return ret;
        }

        public int InpMultiBit(short Id, short[] BitNo, short BitNum, byte[] Data)
        {
            int ret = DioInpMultiBit(Id, BitNo, BitNum, Data);
            return ret;
        }

        public int InpMultiByteSR(short Id, short[] PortNo, short PortNum, byte[] Data, out ushort Timestanp, byte Mode)
        {
            Timestanp = 0;
            int ret = DioInpMultiByteSR(Id, PortNo, PortNum, Data, ref Timestanp, Mode);
            return ret;
        }

        public int InpMultiBitSR(short Id, short[] BitNo, short BitNum, byte[] Data, out ushort Timestanp, byte Mode)
        {
            Timestanp = 0;
            int ret = DioInpMultiBitSR(Id, BitNo, BitNum, Data, ref Timestanp, Mode);
            return ret;
        }

        public int OutMultiByte(short Id, short[] PortNo, short PortNum, byte[] Data)
        {
            int ret = DioOutMultiByte(Id, PortNo, PortNum, Data);
            return ret;
        }

        public int OutMultiBit(short Id, short[] BitNo, short BitNum, byte[] Data)
        {
            int ret = DioOutMultiBit(Id, BitNo, BitNum, Data);
            return ret;
        }

        public int EchoBackMultiByte(short Id, short[] PortNo, short PortNum, byte[] Data)
        {
            int ret = DioEchoBackMultiByte(Id, PortNo, PortNum, Data);
            return ret;
        }

        public int EchoBackMultiBit(short Id, short[] BitNo, short BitNum, byte[] Data)
        {
            int ret = DioEchoBackMultiBit(Id, BitNo, BitNum, Data);
            return ret;
        }

        // Interrupt functions
        public int NotifyInterrupt(short Id, short IntBit, short Logic, int hWnd)
        {
            int ret = DioNotifyInterrupt(Id, IntBit, Logic, hWnd);
            return ret;
        }

        unsafe public int SetInterruptCallBackProc(short Id, IntPtr pIntCallBack, void* Param)
        {
            int ret = DioSetInterruptCallBackProc(Id, pIntCallBack, Param);
            return ret;
        }

        // Trigger functions
        public int NotifyTrg(short Id, short TrgBit, short TrgKind, int Tim, int hWnd)
        {
            int ret = DioNotifyTrg(Id, TrgBit, TrgKind, Tim, hWnd);
            return ret;
        }

        public int StopNotifyTrg(short Id, short TrgBit)
        {
            int ret = DioStopNotifyTrg(Id, TrgBit);
            return ret;
        }

        public int SetTrgCallBackProc(short Id, IntPtr CallBackProc, out int Param)
        {
            Param = 0;
            int ret = DioSetTrgCallBackProc(Id, CallBackProc, ref Param);
            return ret;
        }

        // Information functions
        public int GetDeviceInfo(string Device, short InfoType, out int Param1, out int Param2, out int Param3)
        {
            Param1 = 0;
            Param2 = 0;
            Param3 = 0;
            int ret = DioGetDeviceInfo(Device, InfoType, ref Param1, ref Param2, ref Param3);
            return ret;
        }

        public int QueryDeviceName(short Index, out string DeviceName, out string Device)
        {
            DeviceName = new String('0', 1);
            Device = new String('0', 1);
            System.Text.StringBuilder devicename = new System.Text.StringBuilder(256);
            System.Text.StringBuilder device = new System.Text.StringBuilder(256);
            int ret = DioQueryDeviceName(Index, devicename, device);
            if (ret == 0)
            {
                DeviceName = devicename.ToString();
                Device = device.ToString();
            }
            return ret;
        }

        public int GetDeviceType(string Device, out short DeviceType)
        {
            DeviceType = 0;
            int ret = DioGetDeviceType(Device, ref DeviceType);
            return ret;
        }

        public int GetMaxPorts(short Id, out short InPortNum, out short OutPortNum)
        {
            InPortNum = 0;
            OutPortNum = 0;
            int ret = DioGetMaxPorts(Id, ref InPortNum, ref OutPortNum);
            return ret;
        }

        public int GetMaxCountChannels(short Id, out short ChannelNum)
        {
            ChannelNum = 0;
            int ret = DioGetMaxCountChannels(Id, ref ChannelNum);
            return ret;
        }

        public int SetCountEdge(short Id, short[] ChNo, short ChNum, short[] CountEdge)
        {
            int ret = DioSetCountEdge(Id, ChNo, ChNum, CountEdge);
            return ret;
        }

        public int GetCountEdge(short Id, short[] ChNo, short ChNum, short[] CountEdge)
        {
            int ret = DioGetCountEdge(Id, ChNo, ChNum, CountEdge);
            return ret;
        }

        public int SetCountMatchValue(short Id, short[] ChNo, short ChNum, short[] CompareRegNo, uint[] CompareCount)
        {
            int ret = DioSetCountMatchValue(Id, ChNo, ChNum, CompareRegNo, CompareCount);
            return ret;
        }

        public int StartCount(short Id, short[] ChNo, short ChNum)
        {
            int ret = DioStartCount(Id, ChNo, ChNum);
            return ret;
        }

        public int StopCount(short Id, short[] ChNo, short ChNum)
        {
            int ret = DioStopCount(Id, ChNo, ChNum);
            return ret;
        }

        public int GetCountStatus(short Id, short[] ChNo, short ChNum, uint[] CountStatus)
        {
            int ret = DioGetCountStatus(Id, ChNo, ChNum, CountStatus);
            return ret;
        }

        public int CountPreset(short Id, short[] ChNo, short ChNum, uint[] PresetCount)
        {
            int ret = DioCountPreset(Id, ChNo, ChNum, PresetCount);
            return ret;
        }

        public int ReadCount(short Id, short[] ChNo, short ChNum, uint[] Count)
        {
            int ret = DioReadCount(Id, ChNo, ChNum, Count);
            return ret;
        }

        public int DmSetDirection(short Id, short Direction)
        {
            int ret = DioDmSetDirection(Id, Direction);
            return ret;
        }

        public int DmGetDirection(short Id, out short Direction)
        {
            Direction = 0;
            int ret = DioDmGetDirection(Id, ref Direction);
            return ret;
        }

        public int DmSetStandAlone(short Id)
        {
            int ret = DioDmSetStandAlone(Id);
            return ret;
        }

        public int DmSetMaster(short Id, short ExtSig1, short ExtSig2, short ExtSig3, short MasterHalt, short SlaveHalt)
        {
            int ret = DioDmSetMaster(Id, ExtSig1, ExtSig2, ExtSig3, MasterHalt, SlaveHalt);
            return ret;
        }

        public int DmSetSlave(short Id, short ExtSig1, short ExtSig2, short ExtSig3, short MasterHalt, short SlaveHalt)
        {
            int ret = DioDmSetSlave(Id, ExtSig1, ExtSig2, ExtSig3, MasterHalt, SlaveHalt);
            return ret;
        }

        public int DmSetStartTrigger(short Id, short Direction, short Start)
        {
            int ret = DioDmSetStartTrigger(Id, Direction, Start);
            return ret;
        }

        public int DmSetStartPattern(short Id, uint Pattern, uint Mask)
        {
            int ret = DioDmSetStartPattern(Id, Pattern, Mask);
            return ret;
        }

        public int DmSetClockTrigger(short Id, short Direction, short Clock)
        {
            int ret = DioDmSetClockTrigger(Id, Direction, Clock);
            return ret;
        }

        public int DmSetInternalClock(short Id, short Direction, uint Clock, short Unit)
        {
            int ret = DioDmSetInternalClock(Id, Direction, Clock, Unit);
            return ret;
        }

        public int DmSetStopTrigger(short Id, short Direction, short Stop)
        {
            int ret = DioDmSetStopTrigger(Id, Direction, Stop);
            return ret;
        }

        public int DmSetStopNumber(short Id, short Direction, uint StopNumber)
        {
            int ret = DioDmSetStopNumber(Id, Direction, StopNumber);
            return ret;
        }

        public int DmFifoReset(short Id, short Reset)
        {
            int ret = DioDmFifoReset(Id, Reset);
            return ret;
        }

        public int DmSetBuffer(short Id, short Direction, IntPtr Buffer, uint Length, short IsRing)
        {
            int ret = DioDmSetBuffer(Id, Direction, Buffer, Length, IsRing);
            return ret;
        }

        public int DmSetTransferStartWait(short Id, short Time)
        {
            int ret = DioDmSetTransferStartWait(Id, Time);
            return ret;
        }

        public int DmTransferStart(short Id, short Direction)
        {
            int ret = DioDmTransferStart(Id, Direction);
            return ret;
        }

        public int DmTransferStop(short Id, short Direction)
        {
            int ret = DioDmTransferStop(Id, Direction);
            return ret;
        }

        public int DmGetStatus(short Id, short Direction, out uint Status, out uint Err)
        {
            Status = 0;
            Err = 0;
            int ret = DioDmGetStatus(Id, Direction, ref Status, ref Err);
            return ret;
        }

        public int DmGetCount(short Id, short Direction, out uint Count, out uint Carry)
        {
            Count = 0;
            Carry = 0;
            int ret = DioDmGetCount(Id, Direction, ref Count, ref Carry);
            return ret;
        }

        public int DmGetWritePointer(short Id, short Direction, out uint WritePointer, out uint Count, out uint Carry)
        {
            WritePointer = 0;
            Count = 0;
            Carry = 0;
            int ret = DioDmGetWritePointer(Id, Direction, ref WritePointer, ref Count, ref Carry);
            return ret;
        }

        public int DmSetStopEvent(short Id, short Direction, int hWnd)
        {
            int ret = DioDmSetStopEvent(Id, Direction, hWnd);
            return ret;
        }

        unsafe public int DmSetStopCallBackProc(short Id, IntPtr CallBackProc, void* Param)
        {
            int ret = DioDmSetStopCallBackProc(Id, CallBackProc, Param);
            return ret;
        }

        public int DmSetCountEvent(short Id, short Direction, uint Count, int hWnd)
        {
            int ret = DioDmSetCountEvent(Id, Direction, Count, hWnd);
            return ret;
        }

        unsafe public int DmSetCountCallBackProc(short Id, IntPtr CallBackProc, void* Param)
        {
            int ret = DioDmSetCountCallBackProc(Id, CallBackProc, Param);
            return ret;
        }

        public int SetDemoByte(short Id, short PortNo, byte Data)
        {
            int ret = DioSetDemoByte(Id, PortNo, Data);
            return ret;
        }

        public int SetDemoBit(short Id, short BitNo, byte Data)
        {
            int ret = DioSetDemoBit(Id, BitNo, Data);
            return ret;
        }

        public int ResetPatternEvent(short Id, byte[] Data)
        {
            int ret = DioResetPatternEvent(Id, Data);
            return ret;
        }

        public int GetPatternEventStatus(short Id, out short Status)
        {
            Status = 0;
            int ret = DioGetPatternEventStatus(Id, ref Status);
            return ret;
        }
        #endregion


    }
}
