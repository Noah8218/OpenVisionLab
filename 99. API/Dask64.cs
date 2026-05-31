using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    public delegate void CallbackDelegate();

    public class DASK
    {
        //private const String NativeDll = "PCI-Dask.dll";
        private const String NativeDll = "PCI-Dask64.dll";

        //ADLink PCI Card Type
        public const ushort PCI_6208V = 1;
        public const ushort PCI_6208A = 2;
        public const ushort PCI_6308V = 3;
        public const ushort PCI_6308A = 4;
        public const ushort PCI_7200 = 5;
        public const ushort PCI_7230 = 6;
        public const ushort PCI_7233 = 7;
        public const ushort PCI_7234 = 8;
        public const ushort PCI_7248 = 9;
        public const ushort PCI_7249 = 10;
        public const ushort PCI_7250 = 11;
        public const ushort PCI_7252 = 12;
        public const ushort PCI_7296 = 13;
        public const ushort PCI_7300A_RevA = 14;
        public const ushort PCI_7300A_RevB = 15;
        public const ushort PCI_7432 = 16;
        public const ushort PCI_7433 = 17;
        public const ushort PCI_7434 = 18;
        public const ushort PCI_8554 = 19;
        public const ushort PCI_9111DG = 20;
        public const ushort PCI_9111HR = 21;
        public const ushort PCI_9112 = 22;
        public const ushort PCI_9113 = 23;
        public const ushort PCI_9114DG = 24;
        public const ushort PCI_9114HG = 25;
        public const ushort PCI_9118DG = 26;
        public const ushort PCI_9118HG = 27;
        public const ushort PCI_9118HR = 28;
        public const ushort PCI_9810 = 29;
        public const ushort PCI_9812 = 30;
        public const ushort PCI_7396 = 31;
        public const ushort PCI_9116 = 32;
        public const ushort PCI_7256 = 33;
        public const ushort PCI_7258 = 34;
        public const ushort PCI_7260 = 35;
        public const ushort PCI_7452 = 36;
        public const ushort PCI_7442 = 37;

        public const ushort MAX_CARD = 32;

        //Error Number
        public const short NoError = 0;
        public const short ErrorUnknownCardType = -1;
        public const short ErrorInvalidCardNumber = -2;
        public const short ErrorTooManyCardRegistered = -3;
        public const short ErrorCardNotRegistered = -4;
        public const short ErrorFuncNotSupport = -5;
        public const short ErrorInvalidIoChannel = -6;
        public const short ErrorInvalidAdRange = -7;
        public const short ErrorContIoNotAllowed = -8;
        public const short ErrorDiffRangeNotSupport = -9;
        public const short ErrorLastChannelNotZero = -10;
        public const short ErrorChannelNotDescending = -11;
        public const short ErrorChannelNotAscending = -12;
        public const short ErrorOpenDriverFailed = -13;
        public const short ErrorOpenEventFailed = -14;
        public const short ErrorTransferCountTooLarge = -15;
        public const short ErrorNotDoubleBufferMode = -16;
        public const short ErrorInvalidSampleRate = -17;
        public const short ErrorInvalidCounterMode = -18;
        public const short ErrorInvalidCounter = -19;
        public const short ErrorInvalidCounterState = -20;
        public const short ErrorInvalidBinBcdParam = -21;
        public const short ErrorBadCardType = -22;
        public const short ErrorInvalidDaRefVoltage = -23;
        public const short ErrorAdTimeOut = -24;
        public const short ErrorNoAsyncAI = -25;
        public const short ErrorNoAsyncAO = -26;
        public const short ErrorNoAsyncDI = -27;
        public const short ErrorNoAsyncDO = -28;
        public const short ErrorNotInputPort = -29;
        public const short ErrorNotOutputPort = -30;
        public const short ErrorInvalidDioPort = -31;
        public const short ErrorInvalidDioLine = -32;
        public const short ErrorContIoActive = -33;
        public const short ErrorDblBufModeNotAllowed = -34;
        public const short ErrorConfigFailed = -35;
        public const short ErrorInvalidPortDirection = -36;
        public const short ErrorBeginThreadError = -37;
        public const short ErrorInvalidPortWidth = -38;
        public const short ErrorInvalidCtrSource = -39;
        public const short ErrorOpenFile = -40;
        public const short ErrorAllocateMemory = -41;
        public const short ErrorDaVoltageOutOfRange = -42;
        public const short ErrorDaExtRefNotAllowed = -43;
        public const short ErrorDIODataWidthError = -44;
        public const short ErrorTaskCodeError = -45;
        public const short ErrortriggercountError = -46;
        public const short ErrorInvalidTriggerMode = -47;
        public const short ErrorInvalidTriggerType = -48;
        public const short ErrorInvalidCounterValue = -50;
        public const short ErrorInvalidEventHandle = -60;
        public const short ErrorNoMessageAvailable = -61;
        public const short ErrorEventMessgaeNotAdded = -62;
        //Error number for driver API 
        public const short ErrorConfigIoctl = -201;
        public const short ErrorAsyncSetIoctl = -202;
        public const short ErrorDBSetIoctl = -203;
        public const short ErrorDBHalfReadyIoctl = -204;
        public const short ErrorContOPIoctl = -205;
        public const short ErrorContStatusIoctl = -206;
        public const short ErrorPIOIoctl = -207;
        public const short ErrorDIntSetIoctl = -208;
        public const short ErrorWaitEvtIoctl = -209;
        public const short ErrorOpenEvtIoctl = -210;
        public const short ErrorCOSIntSetIoctl = -211;
        public const short ErrorMemMapIoctl = -212;
        public const short ErrorMemUMapSetIoctl = -213;
        public const short ErrorCTRIoctl = -214;
        public const short ErrorGetResIoctl = -215;

        //Synchronous Mode
        public const ushort SYNCH_OP = 1;
        public const ushort ASYNCH_OP = 2;

        //AD Range
        public const ushort AD_B_10_V = 1;
        public const ushort AD_B_5_V = 2;
        public const ushort AD_B_2_5_V = 3;
        public const ushort AD_B_1_25_V = 4;
        public const ushort AD_B_0_625_V = 5;
        public const ushort AD_B_0_3125_V = 6;
        public const ushort AD_B_0_5_V = 7;
        public const ushort AD_B_0_05_V = 8;
        public const ushort AD_B_0_005_V = 9;
        public const ushort AD_B_1_V = 10;
        public const ushort AD_B_0_1_V = 11;
        public const ushort AD_B_0_01_V = 12;
        public const ushort AD_B_0_001_V = 13;
        public const ushort AD_U_20_V = 14;
        public const ushort AD_U_10_V = 15;
        public const ushort AD_U_5_V = 16;
        public const ushort AD_U_2_5_V = 17;
        public const ushort AD_U_1_25_V = 18;
        public const ushort AD_U_1_V = 19;
        public const ushort AD_U_0_1_V = 20;
        public const ushort AD_U_0_01_V = 21;
        public const ushort AD_U_0_001_V = 22;

        //Clock Mode
        public const ushort TRIG_SOFTWARE = 0;
        public const ushort TRIG_INT_PACER = 1;
        public const ushort TRIG_EXT_STROBE = 2;
        public const ushort TRIG_HANDSHAKE = 3;
        public const ushort TRIG_CLK_10MHZ = 4;  //PCI-7300A
        public const ushort TRIG_CLK_20MHZ = 5;  //PCI-7300A
        public const ushort TRIG_DO_CLK_TIMER_ACK = 6;  //PCI-7300A Rev. B
        public const ushort TRIG_DO_CLK_10M_ACK = 7;  //PCI-7300A Rev. B
        public const ushort TRIG_DO_CLK_20M_ACK = 8;  //PCI-7300A Rev. B
        //Virtual Sampling Rate for using external clock as the clock source
        public const double CLKSRC_EXT_SampRate = 10000;

        //-------- Constants for PCI-6208A/PCI-6308A/PCI-6308V -------------------
        //Output Mode
        public const ushort P6208_CURRENT_0_20MA = 0;
        public const ushort P6208_CURRENT_4_20MA = 3;
        public const ushort P6208_CURRENT_5_25MA = 1;
        public const ushort P6308_CURRENT_0_20MA = 0;
        public const ushort P6308_CURRENT_4_20MA = 3;
        public const ushort P6308_CURRENT_5_25MA = 1;
        //AO Setting
        public const ushort P6308V_AO_CH0_3 = 0;
        public const ushort P6308V_AO_CH4_7 = 1;
        public const ushort P6308V_AO_UNIPOLAR = 0;
        public const ushort P6308V_AO_BIPOLAR = 1;
        //-------- Constants for PCI-7200 --------------------
        //InputMode
        public const ushort DI_WAITING = 0x02;
        public const ushort DI_NOWAITING = 0x00;

        public const ushort DI_TRIG_RISING = 0x04;
        public const ushort DI_TRIG_FALLING = 0x00;

        public const ushort IREQ_RISING = 0x08;
        public const ushort IREQ_FALLING = 0x00;

        //Output Mode
        public const ushort OREQ_ENABLE = 0x10;
        public const ushort OREQ_DISABLE = 0x00;

        public const ushort OTRIG_HIGH = 0x20;
        public const ushort OTRIG_LOW = 0x00;

        //-------- Constants for PCI-7248/7296/7396/7442 ---------------
        //DIO Port Direction
        public const ushort INPUT_PORT = 1;
        public const ushort OUTPUT_PORT = 2;
        //DIO Line Direction
        public const ushort INPUT_LINE = 1;
        public const ushort OUTPUT_LINE = 2;

        //Channel & Port
        public const ushort Channel_P1A = 0;
        public const ushort Channel_P1B = 1;
        public const ushort Channel_P1C = 2;
        public const ushort Channel_P1CL = 3;
        public const ushort Channel_P1CH = 4;
        public const ushort Channel_P1AE = 10;
        public const ushort Channel_P1BE = 11;
        public const ushort Channel_P1CE = 12;
        public const ushort Channel_P2A = 5;
        public const ushort Channel_P2B = 6;
        public const ushort Channel_P2C = 7;
        public const ushort Channel_P2CL = 8;
        public const ushort Channel_P2CH = 9;
        public const ushort Channel_P2AE = 15;
        public const ushort Channel_P2BE = 16;
        public const ushort Channel_P2CE = 17;
        public const ushort Channel_P3A = 10;
        public const ushort Channel_P3B = 11;
        public const ushort Channel_P3C = 12;
        public const ushort Channel_P3CL = 13;
        public const ushort Channel_P3CH = 14;
        public const ushort Channel_P4A = 15;
        public const ushort Channel_P4B = 16;
        public const ushort Channel_P4C = 17;
        public const ushort Channel_P4CL = 18;
        public const ushort Channel_P4CH = 19;
        public const ushort Channel_P5A = 20;
        public const ushort Channel_P5B = 21;
        public const ushort Channel_P5C = 22;
        public const ushort Channel_P5CL = 23;
        public const ushort Channel_P5CH = 24;
        public const ushort Channel_P6A = 25;
        public const ushort Channel_P6B = 26;
        public const ushort Channel_P6C = 27;
        public const ushort Channel_P6CL = 28;
        public const ushort Channel_P6CH = 29;
        //the following are used for PCI7396
        public const ushort Channel_P1 = 30;
        public const ushort Channel_P2 = 31;
        public const ushort Channel_P3 = 32;
        public const ushort Channel_P4 = 33;
        public const ushort Channel_P1E = 34; //only used by DIO_PortConfig function
        public const ushort Channel_P2E = 35; //only used by DIO_PortConfig function
        public const ushort Channel_P3E = 36; //only used by DIO_PortConfig function
        public const ushort Channel_P4E = 37; //only used by DIO_PortConfig function
        //7442
        public const ushort P7442_CH0 = 0;
        public const ushort P7442_CH1 = 1;
        public const ushort P7442_TTL0 = 2;
        public const ushort P7442_TTL1 = 3;
        //-------- Constants for PCI-7300A -------------------
        //Wait Status
        public const ushort P7300_WAIT_NO = 0;
        public const ushort P7300_WAIT_TRG = 1;
        public const ushort P7300_WAIT_FIFO = 2;
        public const ushort P7300_WAIT_BOTH = 3;

        //Terminator control
        public const ushort P7300_TERM_OFF = 0;
        public const ushort P7300_TERM_ON = 1;

        //DI control signals polarity for PCI-7300A Rev. B
        public const ushort P7300_DIREQ_POS = 0x00000000;
        public const ushort P7300_DIREQ_NEG = 0x00000001;
        public const ushort P7300_DIACK_POS = 0x00000000;
        public const ushort P7300_DIACK_NEG = 0x00000002;
        public const ushort P7300_DITRIG_POS = 0x00000000;
        public const ushort P7300_DITRIG_NEG = 0x00000004;

        //DO control signals polarity for PCI-7300A Rev. B
        public const ushort P7300_DOREQ_POS = 0x00000000;
        public const ushort P7300_DOREQ_NEG = 0x00000008;
        public const ushort P7300_DOACK_POS = 0x00000000;
        public const ushort P7300_DOACK_NEG = 0x00000010;
        public const ushort P7300_DOTRIG_POS = 0x00000000;
        public const ushort P7300_DOTRIG_NEG = 0x00000020;
        //-------- Constants for PCI-7432/7433/7434 ---------------
        public const ushort PORT_DI_LOW = 0;
        public const ushort PORT_DI_HIGH = 1;
        public const ushort PORT_DO_LOW = 0;
        public const ushort PORT_DO_HIGH = 1;
        public const ushort P7432R_DO_LED = 1;
        public const ushort P7433R_DO_LED = 0;
        public const ushort P7434R_DO_LED = 2;
        public const ushort P7432R_DI_SLOT = 1;
        public const ushort P7433R_DI_SLOT = 2;
        public const ushort P7434R_DI_SLOT = 0;
        //-- Dual-Interrupt Source control for PCI-7248/96 & 7432/33 & 7230 & 8554 & 7396 &7256 &7260 ---
        public const short INT1_DISABLE = -1;    //INT1 Disabled
        public const short INT1_COS = 0;    //INT1 COS : only available for PCI-7396, PCI-7256, PCI-7260
        public const short INT1_FP1C0 = 1;    //INT1 by Falling edge of P1C0 : only available for PCI7248/96/7396
        public const short INT1_RP1C0_FP1C3 = 2;    //INT1 by P1C0 Rising or P1C3 Falling : only available for PCI7248/96/7396
        public const short INT1_EVENT_COUNTER = 3;    //INT1 by Event Counter down to zero : only available for PCI7248/96/7396
        public const short INT1_EXT_SIGNAL = 1;    //INT1 by external signal : only available for PCI7432/7433/7230/8554
        public const short INT1_COUT12 = 1;    //INT1 COUT12 : only available for PCI8554
        public const short INT1_CH0 = 1;    //INT1 CH0 : only available for PCI7256, PCI7260
        public const short INT1_COS0 = 1;    //INT1 COS0 : only available for PCI-7452/PCI-7442
        public const short INT1_COS1 = 2;    //INT1 COS1 : only available for PCI-7452/PCI-7442
        public const short INT1_COS2 = 4;    //INT1 COS2 : only available for PCI-7452/PCI-7442
        public const short INT1_COS3 = 8;    //INT1 COS3 : only available for PCI-7452/PCI-7442
        public const short INT2_DISABLE = -1;    //INT2 Disabled
        public const short INT2_COS = 0;    //INT2 COS : only available for PCI-7396
        public const short INT2_FP2C0 = 1;    //INT2 by Falling edge of P2C0 : only available for PCI7248/96/7396
        public const short INT2_RP2C0_FP2C3 = 2;    //INT2 by P2C0 Rising or P2C3 Falling : only available for PCI7248/96/7396
        public const short INT2_TIMER_COUNTER = 3;    //INT2 by Timer Counter down to zero : only available for PCI7248/96/7396
        public const short INT2_EXT_SIGNAL = 1;    //INT2 by external signal : only available for PCI7432/7433/7230/8554
        public const short INT2_CH1 = 2;	  //INT2 CH1 : only available for PCI7256, PCI7260
        public const short INT2_WDT = 4;	  //INT2 by WDT

        public const ushort ManualResetIEvt = 0x4000;//interrupt event is manually reset by user
        public const ushort WDT_OVRFLOW_SAFETYOUT = 0x8000;// enable safteyout while WDT overflow
        //-------- Constants for PCI-8554 --------------------
        //Clock Source of Cunter N
        public const ushort ECKN = 0;
        public const ushort COUTN_1 = 1;
        public const ushort CK1 = 2;
        public const ushort COUT10 = 3;

        //Clock Source of CK1
        public const ushort CK1_C8M = 0;
        public const ushort CK1_COUT11 = 1;

        //Debounce Clock
        public const ushort DBCLK_COUT11 = 0;
        public const ushort DBCLK_2MHZ = 1;

        //-------- Constants for PCI-9111 --------------------
        //Dual Interrupt Mode
        public const ushort P9111_INT1_EOC = 0;       //Ending of AD conversion
        public const ushort P9111_INT1_FIFO_HF = 1;       //FIFO Half Full
        public const ushort P9111_INT2_PACER = 0;       //Every Timer tick
        public const ushort P9111_INT2_EXT_TRG = 1;       //ExtTrig High->Low

        //Channel Count
        public const ushort P9111_CHANNEL_DO = 0;
        public const ushort P9111_CHANNEL_EDO = 1;
        public const ushort P9111_CHANNEL_DI = 0;
        public const ushort P9111_CHANNEL_EDI = 1;

        //EDO function
        public const ushort P9111_EDO_INPUT = 1;   //EDO port set as Input port
        public const ushort P9111_EDO_OUT_EDO = 2;   //EDO port set as Output port
        public const ushort P9111_EDO_OUT_CHN = 3;   //EDO port set as channel number ouput port

        //Trigger Mode
        public const ushort P9111_TRGMOD_SOFT = 0x00;   //Software Trigger Mode
        public const ushort P9111_TRGMOD_PRE = 0x01;   //Pre-Trigger Mode
        public const ushort P9111_TRGMOD_POST = 0x02;   //Post Trigger Mode

        //AO Setting
        public const ushort P9111_AO_UNIPOLAR = 0;
        public const ushort P9111_AO_BIPOLAR = 1;

        //-------- Constants for PCI-9118 --------------------
        public const ushort P9118_AI_BiPolar = 0x00;
        public const ushort P9118_AI_UniPolar = 0x01;

        public const ushort P9118_AI_SingEnded = 0x00;
        public const ushort P9118_AI_Differential = 0x02;

        public const ushort P9118_AI_ExtG = 0x04;

        public const ushort P9118_AI_ExtTrig = 0x08;

        public const ushort P9118_AI_DtrgNegative = 0x00;
        public const ushort P9118_AI_DtrgPositive = 0x10;

        public const ushort P9118_AI_EtrgNegative = 0x00;
        public const ushort P9118_AI_EtrgPositive = 0x20;

        public const ushort P9118_AI_BurstModeEn = 0x40;
        public const ushort P9118_AI_SampleHold = 0x80;
        public const ushort P9118_AI_PostTrgEn = 0x100;
        public const ushort P9118_AI_AboutTrgEn = 0x200;

        //-------- Constants for PCI-9116 --------------------
        public const ushort P9116_AI_LocalGND = 0x00;
        public const ushort P9116_AI_UserCMMD = 0x01;
        public const ushort P9116_AI_SingEnded = 0x00;
        public const ushort P9116_AI_Differential = 0x02;
        public const ushort P9116_AI_BiPolar = 0x00;
        public const ushort P9116_AI_UniPolar = 0x04;

        public const ushort P9116_TRGMOD_SOFT = 0x00;   //Software Trigger Mode
        public const ushort P9116_TRGMOD_POST = 0x10;   //Post Trigger Mode
        public const ushort P9116_TRGMOD_DELAY = 0x20;   //Delay Trigger Mode
        public const ushort P9116_TRGMOD_PRE = 0x30;   //Pre-Trigger Mode
        public const ushort P9116_TRGMOD_MIDL = 0x40;   //Middle Trigger Mode
        public const ushort P9116_AI_TrgPositive = 0x00;
        public const ushort P9116_AI_TrgNegative = 0x80;
        public const ushort P9116_AI_ExtTimeBase = 0x100;
        public const ushort P9116_AI_IntTimeBase = 0x000;
        public const ushort P9116_AI_DlyInSamples = 0x200;
        public const ushort P9116_AI_DlyInTimebase = 0x000;
        public const ushort P9116_AI_ReTrigEn = 0x400;
        public const ushort P9116_AI_MCounterEn = 0x800;
        public const ushort P9116_AI_SoftPolling = 0x0000;
        public const ushort P9116_AI_INT = 0x1000;
        public const ushort P9116_AI_DMA = 0x2000;

        //-------- Constants for PCI-9812 --------------------
        //Trigger Mode
        public const ushort P9812_TRGMOD_SOFT = 0x00;   //Software Trigger Mode
        public const ushort P9812_TRGMOD_POST = 0x01;   //Post Trigger Mode
        public const ushort P9812_TRGMOD_PRE = 0x02;   //Pre-Trigger Mode
        public const ushort P9812_TRGMOD_DELAY = 0x03;   //Delay Trigger Mode
        public const ushort P9812_TRGMOD_MIDL = 0x04;   //Middle Trigger Mode

        public const ushort P9812_AIEvent_Manual = 0x08;   //Middle Trigger Mode

        //Trigger Source
        public const ushort P9812_TRGSRC_CH0 = 0x00;   //trigger source --CH0
        public const ushort P9812_TRGSRC_CH1 = 0x08;   //trigger source --CH1
        public const ushort P9812_TRGSRC_CH2 = 0x10;   //trigger source --CH2
        public const ushort P9812_TRGSRC_CH3 = 0x18;   //trigger source --CH3
        public const ushort P9812_TRGSRC_EXT_DIG = 0x20;   //External Digital Trigger

        //Trigger Polarity
        public const ushort P9812_TRGSLP_POS = 0x00;   //Positive slope trigger
        public const ushort P9812_TRGSLP_NEG = 0x40;   //Negative slope trigger

        //Frequency Selection
        public const ushort P9812_AD2_GT_PCI = 0x80;   //Freq. of A/D clock > PCI clock freq.
        public const ushort P9812_AD2_LT_PCI = 0x00;   //Freq. of A/D clock < PCI clock freq.

        //Clock Source
        public const ushort P9812_CLKSRC_INT = 0x000;   //Internal clock
        public const ushort P9812_CLKSRC_EXT_SIN = 0x100;  //External SIN wave clock
        public const ushort P9812_CLKSRC_EXT_DIG = 0x200;  //External Square wave clock

        //EMG shdn ctrl code
        public const ushort EMGSHDN_OFF = 0;     //off
        public const ushort EMGSHDN_ON = 1; //on
        public const ushort EMGSHDN_RECOVERY = 2; //recovery

        //Hot Reset Hold ctrl code
        public const ushort HRH_OFF = 0; //off
        public const ushort HRH_ON = 1; //on

        //-------- Timer/Counter -----------------------------
        //Counter Mode (8254)
        public const ushort TOGGLE_OUTPUT = 0;     //Toggle output from low to high on terminal count
        public const ushort PROG_ONE_SHOT = 1;     //Programmable one-shot
        public const ushort RATE_GENERATOR = 2;     //Rate generator
        public const ushort SQ_WAVE_RATE_GENERATOR = 3;     //Square wave rate generator
        public const ushort SOFT_TRIG = 4;     //Software-triggered strobe
        public const ushort HARD_TRIG = 5;     //Hardware-triggered strobe

        //General Purpose Timer/Counter
        //Counter Mode
        public const ushort General_Counter = 0x00; //general counter
        public const ushort Pulse_Generation = 0x01; //pulse generation
        //GPTC clock source
        public const ushort GPTC_CLKSRC_EXT = 0x08;
        public const ushort GPTC_CLKSRC_INT = 0x00;
        public const ushort GPTC_GATESRC_EXT = 0x10;
        public const ushort GPTC_GATESRC_INT = 0x00;
        public const ushort GPTC_UPDOWN_SELECT_EXT = 0x20;
        public const ushort GPTC_UPDOWN_SELECT_SOFT = 0x00;
        public const ushort GPTC_UP_CTR = 0x40;
        public const ushort GPTC_DOWN_CTR = 0x00;
        public const ushort GPTC_ENABLE = 0x80;
        public const ushort GPTC_DISABLE = 0x00;

        //Watchdog Timer
        //Counter action
        public const ushort WDT_DISARM = 0;
        public const ushort WDT_ARM = 1;
        public const ushort WDT_RESTART = 2;

        //Pattern ID
        public const ushort INIT_PTN = 0;
        public const ushort EMGSHDN_PTN = 1;

        //16-bit binary or 4-decade BCD counter
        public const ushort BIN = 0;
        public const ushort BCD = 1;

        //Pattern ID for 7442
        public const ushort INIT_PTN_CH0 = 0;
        public const ushort INIT_PTN_CH1 = 1;
        public const ushort SAFTOUT_PTN_CH0 = 4;
        public const ushort SAFTOUT_PTN_CH1 = 5;

        //DAQ Event type for the event message  
        public const ushort AIEnd = 0;
        public const ushort DIEnd = 0;
        public const ushort DOEnd = 0;
        public const ushort DBEvent = 1;

        public const ushort RegBySlot = 0x8000;

        /*------------------------------------------------------------------
        ** PCIS-DASK Function prototype
        ------------------------------------------------------------------*/
        [DllImport(NativeDll)]
        public static extern short Register_Card(ushort CardType, ushort card_num);
        [DllImport(NativeDll)]
        public static extern short Release_Card(ushort CardNumber);
        [DllImport(NativeDll)]
        public static extern short GetActualRate(ushort CardNumber, double fSampleRate, out double fActualRate);
        [DllImport(NativeDll)]
        public static extern short EMGShutDownControl(ushort CardNumber, byte ctrl);
        [DllImport(NativeDll)]
        public static extern short EMGShutDownStatus(ushort CardNumber, out byte sts);
        [DllImport(NativeDll)]
        public static extern short HotResetHoldControl(ushort wCardNumber, byte enable);
        [DllImport(NativeDll)]
        public static extern short HotResetHoldStatus(ushort wCardNumber, out byte sts);
        [DllImport(NativeDll)]
        public static extern short GetInitPattern(ushort CardNumber, byte patID, out uint pattern);
        [DllImport(NativeDll)]
        public static extern short SetInitPattern(ushort wCardNumber, byte patID, uint pattern);
        [DllImport(NativeDll)]
        public static extern short IdentifyLED_Control(ushort CardNumber, byte ctrl);
        /*---------------------------------------------------------------------------*/
        [DllImport(NativeDll)]
        public static extern short AI_9111_Config(ushort CardNumber, ushort TrigSource, ushort TrgMode, ushort TraceCnt);
        [DllImport(NativeDll)]
        public static extern short AI_9112_Config(ushort CardNumber, ushort TrigSource);
        [DllImport(NativeDll)]
        public static extern short AI_9113_Config(ushort CardNumber, ushort TrigSource);
        [DllImport(NativeDll)]
        public static extern short AI_9114_Config(ushort CardNumber, ushort TrigSource);
        [DllImport(NativeDll)]
        public static extern short AI_9116_Config(ushort CardNumber, ushort ConfigCtrl, ushort TrigCtrl, ushort PostCnt, ushort MCnt, ushort ReTrgCnt);
        [DllImport(NativeDll)]
        public static extern short AI_9118_Config(ushort CardNumber, ushort ModeCtrl, ushort FunCtrl, ushort BurstCnt, ushort PostCnt);
        [DllImport(NativeDll)]
        public static extern short AI_9812_Config(ushort CardNumber, ushort TrgMode, ushort TrgSrc, ushort TrgPol, ushort ClkSel, ushort TrgLevel, ushort PostCnt);
        [DllImport(NativeDll)]
        public static extern short AI_9812_SetDiv(ushort wCardNumber, uint PacerVal);
        [DllImport(NativeDll)]
        public static extern short AI_9114_PreTrigConfig(ushort CardNumber, ushort PreTrgEn, ushort TraceCnt);
        [DllImport(NativeDll)]
        public static extern short AI_9116_CounterInterval(ushort wCardNumber, uint ScanIntrv, uint SampIntrv);
        [DllImport(NativeDll)]
        public static extern short AI_InitialMemoryAllocated(ushort CardNumber, out uint MemSize);
        [DllImport(NativeDll)]
        public static extern short AI_ReadChannel(ushort CardNumber, ushort Channel, ushort AdRange, out ushort Value);
        [DllImport(NativeDll)]
        public static extern short AI_VReadChannel(ushort CardNumber, ushort Channel, ushort AdRange, out double voltage);
        [DllImport(NativeDll)]
        public static extern short AI_VoltScale(ushort CardNumber, ushort AdRange, ushort reading, out double voltage);
        [DllImport(NativeDll)]
        public static extern short AI_ContReadChannel(ushort CardNumber, ushort Channel, ushort AdRange,
                   ushort[] Buffer, uint ReadCount, double SampleRate, ushort SyncMode);
        [DllImport(NativeDll)]
        public static extern short AI_ContReadMultiChannels(ushort CardNumber, ushort NumChans, ushort[] Chans,
                   ushort[] AdRanges, ushort[] Buffer, uint ReadCount,
                   double SampleRate, ushort SyncMode);
        [DllImport(NativeDll)]
        public static extern short AI_ContScanChannels(ushort CardNumber, ushort Channel, ushort AdRange,
                   ushort[] Buffer, uint ReadCount, double SampleRate, ushort SyncMode);
        [DllImport(NativeDll)]
        public static extern short AI_ContReadChannelToFile(ushort CardNumber, ushort Channel, ushort AdRange,
                   string FileName, uint ReadCount, double SampleRate, ushort SyncMode);
        [DllImport(NativeDll)]
        public static extern short AI_ContReadMultiChannelsToFile(ushort CardNumber, ushort NumChans, ushort[] Chans,
                   ushort[] AdRanges, string[] FileName, uint ReadCount,
                   double SampleRate, ushort SyncMode);
        [DllImport(NativeDll)]
        public static extern short AI_ContScanChannelsToFile(ushort CardNumber, ushort Channel, ushort AdRange,
                   string FileName, uint ReadCount, double SampleRate, ushort SyncMode);
        [DllImport(NativeDll)]
        public static extern short AI_ContStatus(ushort CardNumber, out ushort Status);
        [DllImport(NativeDll)]
        public static extern short AI_ContVScale(ushort wCardNumber, ushort adRange, ushort[] readingArray, double[] voltageArray, int count);
        [DllImport(NativeDll)]
        public static extern short AI_AsyncCheck(ushort CardNumber, out byte Stopped, out uint AccessCnt);
        [DllImport(NativeDll)]
        public static extern short AI_AsyncClear(ushort CardNumber, out uint AccessCnt);
        [DllImport(NativeDll)]
        public static extern short AI_AsyncDblBufferHalfReady(ushort CardNumber, out byte HalfReady, out byte StopFlag);
        [DllImport(NativeDll)]
        public static extern short AI_AsyncDblBufferMode(ushort CardNumber, bool Enable);
        [DllImport(NativeDll)]
        public static extern short AI_AsyncDblBufferTransfer(ushort CardNumber, ushort[] Buffer);
        [DllImport(NativeDll)]
        public static extern short AI_AsyncDblBufferOverrun(ushort CardNumber, ushort op, out ushort overrunFlag);
        [DllImport(NativeDll)]
        public static extern short AI_EventCallBack(ushort CardNumber, ushort mode, ushort EventType, MulticastDelegate callbackAddr);
        /*---------------------------------------------------------------------------*/
        [DllImport(NativeDll)]
        public static extern short AO_6208A_Config(ushort CardNumber, ushort V2AMode);
        [DllImport(NativeDll)]
        public static extern short AO_6308A_Config(ushort CardNumber, ushort V2AMode);
        [DllImport(NativeDll)]
        public static extern short AO_6308V_Config(ushort wCardNumber, ushort Channel, ushort wOutputPolarity, double refVoltage);
        [DllImport(NativeDll)]
        public static extern short AO_9111_Config(ushort CardNumber, ushort OutputPolarity);
        [DllImport(NativeDll)]
        public static extern short AO_9112_Config(ushort CardNumber, ushort Channel, double refVoltage);
        [DllImport(NativeDll)]
        public static extern short AO_WriteChannel(ushort CardNumber, ushort Channel, short Value);
        [DllImport(NativeDll)]
        public static extern short AO_VWriteChannel(ushort CardNumber, ushort Channel, double Voltage);
        [DllImport(NativeDll)]
        public static extern short AO_VoltScale(ushort CardNumber, ushort Channel, double Voltage, out short binValue);
        [DllImport(NativeDll)]
        public static extern short AO_SimuWriteChannel(ushort wCardNumber, ushort wGroup, short[] pwBuffer);
        [DllImport(NativeDll)]
        public static extern short AO_SimuVWriteChannel(ushort wCardNumber, ushort wGroup, double[] VBuffer);
        /*---------------------------------------------------------------------------*/
        [DllImport(NativeDll)]
        public static extern short DI_7200_Config(ushort CardNumber, ushort TrigSource, ushort ExtTrigEn, ushort TrigPol, ushort I_REQ_Pol);
        [DllImport(NativeDll)]
        public static extern short DI_7300A_Config(ushort CardNumber, ushort PortWidth, ushort TrigSource, ushort WaitStatus, ushort Terminator, ushort I_REQ_Pol, bool clear_fifo, bool disable_di);
        [DllImport(NativeDll)]
        public static extern short DI_7300B_Config(ushort CardNumber, ushort PortWidth, ushort TrigSource, ushort WaitStatus, ushort Terminator, ushort I_Cntrl_Pol, bool clear_fifo, bool disable_di);
        [DllImport(NativeDll)]
        public static extern short DI_InitialMemoryAllocated(ushort CardNumber, out uint DmaSize);
        [DllImport(NativeDll)]
        public static extern short DI_ReadLine(ushort CardNumber, ushort Port, ushort Line, out ushort State);
        [DllImport(NativeDll)]
        public static extern short DI_ReadPort(ushort CardNumber, ushort Port, out uint Value);
        [DllImport(NativeDll)]
        public static extern short DI_ContReadPort(ushort CardNumber, ushort Port, byte[] Buffer,
                   uint ReadCount, double SampleRate, ushort SyncMode);
        [DllImport(NativeDll)]
        public static extern short DI_ContReadPort(ushort CardNumber, ushort Port, ushort[] Buffer,
                   uint ReadCount, double SampleRate, ushort SyncMode);
        [DllImport(NativeDll)]
        public static extern short DI_ContReadPort(ushort CardNumber, ushort Port, uint[] Buffer,
                   uint ReadCount, double SampleRate, ushort SyncMode);
        [DllImport(NativeDll)]
        public static extern short DI_ContReadPortToFile(ushort CardNumber, ushort Port, string FileName,
                                      uint ReadCount, double SampleRate, ushort SyncMode);
        [DllImport(NativeDll)]
        public static extern short DI_ContStatus(ushort CardNumber, out ushort Status);
        [DllImport(NativeDll)]
        public static extern short DI_AsyncCheck(ushort CardNumber, out byte Stopped, out uint AccessCnt);
        [DllImport(NativeDll)]
        public static extern short DI_AsyncClear(ushort CardNumber, out uint AccessCnt);
        [DllImport(NativeDll)]
        public static extern short DI_AsyncDblBufferHalfReady(ushort CardNumber, out byte HalfReady);
        [DllImport(NativeDll)]
        public static extern short DI_AsyncDblBufferMode(ushort CardNumber, bool Enable);
        [DllImport(NativeDll)]
        public static extern short DI_AsyncDblBufferTransfer(ushort CardNumber, byte[] Buffer);
        [DllImport(NativeDll)]
        public static extern short DI_AsyncDblBufferTransfer(ushort CardNumber, short[] Buffer);
        [DllImport(NativeDll)]
        public static extern short DI_AsyncDblBufferTransfer(ushort CardNumber, uint[] Buffer);
        [DllImport(NativeDll)]
        public static extern short DI_ContMultiBufferSetup(ushort wCardNumber, byte[] pwBuffer, uint dwReadCount, out ushort BufferId);
        [DllImport(NativeDll)]
        public static extern short DI_ContMultiBufferSetup(ushort wCardNumber, short[] pwBuffer, uint dwReadCount, out ushort BufferId);
        [DllImport(NativeDll)]
        public static extern short DI_ContMultiBufferSetup(ushort wCardNumber, uint[] pwBuffer, uint dwReadCount, out ushort BufferId);
        [DllImport(NativeDll)]
        public static extern short DI_ContMultiBufferStart(ushort wCardNumber, ushort wPort, double fSampleRate);
        [DllImport(NativeDll)]
        public static extern short DI_AsyncMultiBufferNextReady(ushort CardNumber, out byte bNextReady, out ushort wBufferId);
        [DllImport(NativeDll)]
        public static extern short DI_AsyncDblBufferOverrun(ushort CardNumber, ushort op, out ushort overrunFlag);
        [DllImport(NativeDll)]
        public static extern short DI_EventCallBack(ushort CardNumber, short mode, short EventType, MulticastDelegate callbackAddr);
        /*---------------------------------------------------------------------------*/
        [DllImport(NativeDll)]
        public static extern short DO_7200_Config(ushort CardNumber, ushort TrigSource, ushort OutReqEn, ushort OutTrigSig);
        [DllImport(NativeDll)]
        public static extern short DO_7300A_Config(ushort CardNumber, ushort PortWidth, ushort TrigSource, ushort WaitStatus, ushort Terminator, ushort O_REQ_Pol);
        [DllImport(NativeDll)]
        public static extern short DO_7300B_Config(ushort CardNumber, ushort PortWidth, ushort TrigSource, ushort WaitStatus, ushort Terminator, ushort O_Cntrl_Pol, uint FifoThreshold);
        [DllImport(NativeDll)]
        public static extern short DO_InitialMemoryAllocated(ushort CardNumber, out uint MemSize);
        [DllImport(NativeDll)]
        public static extern short DO_WriteLine(ushort CardNumber, ushort Port, ushort Line, ushort Value);
        [DllImport(NativeDll)]
        public static extern short DO_WritePort(ushort CardNumber, byte Port, uint Value);
        [DllImport(NativeDll)]
        public static extern short DO_WritePort(ushort CardNumber, ushort Port, uint Value);
        [DllImport(NativeDll)]
        public static extern short DO_WritePort(ushort CardNumber, uint Port, uint Value);
        [DllImport(NativeDll)]
        public static extern short DO_WriteExtTrigLine(ushort CardNumber, ushort Value);
        [DllImport(NativeDll)]
        public static extern short DO_ReadLine(ushort CardNumber, ushort Port, ushort Line, out ushort Value);
        [DllImport(NativeDll)]
        public static extern short DO_ReadPort(ushort CardNumber, ushort Port, out uint Value);
        [DllImport(NativeDll)]
        public static extern short DO_ContWritePort(ushort CardNumber, ushort Port, byte[] Buffer,
                   uint WriteCount, ushort Iterations, double SampleRate, ushort SyncMode);
        [DllImport(NativeDll)]
        public static extern short DO_ContWritePort(ushort CardNumber, ushort Port, ushort[] Buffer,
                   uint WriteCount, ushort Iterations, double SampleRate, ushort SyncMode);
        [DllImport(NativeDll)]
        public static extern short DO_ContWritePort(ushort CardNumber, ushort Port, uint[] Buffer,
                   uint WriteCount, ushort Iterations, double SampleRate, ushort SyncMode);
        [DllImport(NativeDll)]
        public static extern short DO_PGStart(ushort CardNumber, byte[] Buffer, uint WriteCount, double SampleRate);
        [DllImport(NativeDll)]
        public static extern short DO_PGStart(ushort CardNumber, short[] Buffer, uint WriteCount, double SampleRate);
        [DllImport(NativeDll)]
        public static extern short DO_PGStart(ushort CardNumber, uint[] Buffer, uint WriteCount, double SampleRate);
        [DllImport(NativeDll)]
        public static extern short DO_PGStop(ushort CardNumber);
        [DllImport(NativeDll)]
        public static extern short DO_ContStatus(ushort CardNumber, out ushort Status);
        [DllImport(NativeDll)]
        public static extern short DO_AsyncCheck(ushort CardNumber, out byte Stopped, out uint AccessCnt);
        [DllImport(NativeDll)]
        public static extern short DO_AsyncClear(ushort CardNumber, out uint AccessCnt);
        [DllImport(NativeDll)]
        public static extern short EDO_9111_Config(ushort CardNumber, ushort EDO_Fun);
        [DllImport(NativeDll)]
        public static extern short DO_ContMultiBufferSetup(ushort CardNumber, byte[] pwBuffer, uint dwWriteCount, out ushort BufferId);
        [DllImport(NativeDll)]
        public static extern short DO_ContMultiBufferSetup(ushort CardNumber, short[] pwBuffer, uint dwWriteCount, out ushort BufferId);
        [DllImport(NativeDll)]
        public static extern short DO_ContMultiBufferSetup(ushort CardNumber, uint[] pwBuffer, uint dwWriteCount, out ushort BufferId);
        [DllImport(NativeDll)]
        public static extern short DO_AsyncMultiBufferNextReady(ushort CardNumber, out byte bNextReady, out ushort wBufferId);
        [DllImport(NativeDll)]
        public static extern short DO_ContMultiBufferStart(ushort wCardNumber, ushort wPort, double fSampleRate);
        [DllImport(NativeDll)]
        public static extern short DO_EventCallBack(ushort CardNumber, short mode, short EventType, MulticastDelegate callbackAddr);
        /*---------------------------------------------------------------------------*/
        [DllImport(NativeDll)]
        public static extern short DIO_PortConfig(ushort CardNumber, ushort Port, ushort Direction);
        [DllImport(NativeDll)]
        public static extern short DIO_LinesConfig(ushort wCardNumber, ushort wPort, ushort wLinesdirmap);
        [DllImport(NativeDll)]
        public static extern short DIO_LineConfig(ushort wCardNumber, ushort wPort, ushort wLine, ushort wDirection);
        [DllImport(NativeDll)]
        public static extern short DIO_SetDualInterrupt(ushort CardNumber, short Int1Mode, short Int2Mode, long hEvent);
        [DllImport(NativeDll)]
        public static extern short DIO_SetCOSInterrupt(ushort CardNumber, ushort Port, ushort ctlA, ushort ctlB, ushort ctlC);
        [DllImport(NativeDll)]
        public static extern short DIO_INT1_EventMessage(ushort CardNumber, short Int1Mode, long windowHandle, long message, MulticastDelegate callbackAddr);
        [DllImport(NativeDll)]
        public static extern short DIO_INT2_EventMessage(ushort CardNumber, short Int2Mode, long windowHandle, long message, MulticastDelegate callbackAddr);
        [DllImport(NativeDll)]
        public static extern short DIO_7300SetInterrupt(ushort CardNumber, short AuxDIEn, short T2En, long hEvent);
        [DllImport(NativeDll)]
        public static extern short DIO_AUXDI_EventMessage(ushort CardNumber, short AuxDIEn, long windowHandle, uint message, MulticastDelegate callbackAddr);
        [DllImport(NativeDll)]
        public static extern short DIO_T2_EventMessage(ushort CardNumber, short T2En, long windowHandle, uint message, MulticastDelegate callbackAddr);
        [DllImport(NativeDll)]
        public static extern short DIO_GetCOSLatchData(ushort wCardNumber, out ushort CosLData);
        [DllImport(NativeDll)]
        public static extern short DIO_SetCOSInterrupt32(ushort wCardNumber, byte Port, uint ctl, out long hEvent, bool bManualReset);
        [DllImport(NativeDll)]
        public static extern short DIO_GetCOSLatchDataInt32(ushort wCardNumber, byte Port, out uint CosLData);
        /*---------------------------------------------------------------------------*/
        [DllImport(NativeDll)]
        public static extern short CTR_Setup(ushort CardNumber, ushort Ctr, ushort Mode, uint Count, ushort BinBcd);
        [DllImport(NativeDll)]
        public static extern short CTR_Clear(ushort CardNumber, ushort Ctr, ushort State);
        [DllImport(NativeDll)]
        public static extern short CTR_Read(ushort CardNumber, ushort Ctr, out uint Value);
        [DllImport(NativeDll)]
        public static extern short CTR_Update(ushort CardNumber, ushort Ctr, uint Count);
        [DllImport(NativeDll)]
        public static extern short CTR_8554_ClkSrc_Config(ushort CardNumber, ushort Ctr, ushort ClockSource);
        [DllImport(NativeDll)]
        public static extern short CTR_8554_CK1_Config(ushort CardNumber, ushort ClockSource);
        [DllImport(NativeDll)]
        public static extern short CTR_8554_Debounce_Config(ushort CardNumber, ushort DebounceClock);
        [DllImport(NativeDll)]
        public static extern short GCTR_Setup(ushort wCardNumber, ushort wGCtr, ushort wGCtrCtrl, uint dwCount);
        [DllImport(NativeDll)]
        public static extern short GCTR_Clear(ushort wCardNumber, ushort wGCtr);
        [DllImport(NativeDll)]
        public static extern short GCTR_Read(ushort wCardNumber, ushort wGCtr, out uint pValue);
        [DllImport(NativeDll)]
        public static extern short WDT_Setup(ushort CardNumber, ushort wCtr, float ovflowSec, out float actualSec, out long hEvent);
        [DllImport(NativeDll)]
        public static extern short WDT_Control(ushort wCardNumber, ushort wCtr, ushort action);
        [DllImport(NativeDll)]
        public static extern short WDT_Status(ushort wCardNumber, ushort Ctr, out uint pValue);
        [DllImport(NativeDll)]
        public static extern short WDT_Reload(ushort wCardNumber, float ovflowSec, out float actualSec);
        /*---------------------------------------------------------------------------*/
        [DllImport(NativeDll)]
        public static extern short AI_GetEvent(ushort wCardNumber, out long hEvent);
        [DllImport(NativeDll)]
        public static extern short AO_GetEvent(ushort wCardNumber, out long hEvent);
        [DllImport(NativeDll)]
        public static extern short DI_GetEvent(ushort wCardNumber, out long hEvent);
        [DllImport(NativeDll)]
        public static extern short DO_GetEvent(ushort wCardNumber, out long hEvent);
        /*---------------------------------------------------------------------------*/
        [DllImport(NativeDll)]
        public static extern short AI_GetView(ushort wCardNumber, uint[] pView);
        [DllImport(NativeDll)]
        public static extern short DI_GetView(ushort wCardNumber, uint[] pView);
        [DllImport(NativeDll)]
        public static extern short DO_GetView(ushort wCardNumber, uint[] pView);
        /*---------------------------------------------------------------------------*/
        [DllImport(NativeDll)]
        public static extern short GetCardType(ushort wCardNumber, out ushort cardType);
        [DllImport(NativeDll)]
        public static extern short GetCardIndexFromID(ushort wCardNumber, out ushort cardType, out ushort cardIndex);
        [DllImport(NativeDll)]
        public static extern short GetBaseAddr(ushort wCardNumber, uint[] BaseAddr, uint[] BaseAddr2);
        [DllImport(NativeDll)]
        public static extern short GetLCRAddr(ushort wCardNumber, uint[] LcrAddr);
    }
}
