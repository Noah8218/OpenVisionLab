using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace KtemVisionSystem
{
    public class CDIO_AJIN
    {
        public static int DI_MODULE_BASE_NO = 0;
        public static int DO_MODULE_BASE_NO = 1;

        // Input 모듈 
        private const int DI_32_BOARD_00 = 0;
        // Output 모듈
        private const int DO_32_BOARD_01 = 1;

        private const int CH_BOARD_INPUT = 0;
        private const int CH_BOARD_OUTPUT = 1;

        public bool IsOpen { get; set; } = false;
        public bool UseInterrupt { get; set; } = false;
        public bool UseInterrupt_RigingEdge { get; set; } = false;
        public bool UseInterrupt_FallingEdge { get; set; } = false;

        public int BoardChannel { get; set; } = 0;

        //private CAXHS.AXT_INTERRUPT_PROC Callbackfunction = new CAXHS.AXT_INTERRUPT_PROC(InterruptCallback);
        private List<string> Moudules = new List<string>();

        private bool[] InputArray = new bool[32];
        private bool[] OutputArray = new bool[32];

        #region Interrupt
        private uint hInterrupt = 0;
        private Thread EventThread = null;
        private bool bThread = false;

        public readonly static uint INFINITE = 0xFFFFFFFF;
        public readonly static uint STATUS_WAIT_0 = 0x00000000;
        public readonly static uint WAIT_OBJECT_0 = ((STATUS_WAIT_0) + 0);
        [DllImport("kernel32", EntryPoint = "WaitForSingleObject", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint WaitForSingleObject(uint hHandle, uint dwMilliseconds);

        [DllImport("KERNEL32", EntryPoint = "SetEvent", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool SetEvent(long hEvent);
        #endregion

        public CDIO_AJIN()
        {
            m_ThreadRead = new CThread(ThreadRead, this);
        }

        #region IO 
        public CSignal DI_00_SWITCH_START = null;
        public CSignal DI_01_SWITCH_STOP = null;
        public CSignal DI_02_SWITCH_EMG = null; // EMG(스타트 스위치 옆) B접점
        public CSignal DI_03_SWITCH_EMG = null; // EMG(전원 스위치 옆) B접점
        public CSignal DI_04_SPARE = null;
        public CSignal DI_05_SPARE = null;
        public CSignal DI_06_SPARE = null;
        public CSignal DI_07_SPARE = null;
        public CSignal DI_08_SPARE = null;
        public CSignal DI_09_SPARE = null;
        public CSignal DI_10_SPARE = null;
        public CSignal DI_11_SPARE = null;
        public CSignal DI_12_PRODUCT_DETECT_SENSOR = null;
        public CSignal DI_13_TENSION_UPPER_SERVO_ALARAM = null;
        public CSignal DI_14_TENSION_MIDDLE_SERVO_ALARAM = null;
        public CSignal DI_15_TENSION_LOWER_SERVO_ALARAM = null;
        public CSignal DI_16_SPARE = null;
        public CSignal DI_17_SPARE = null;
        public CSignal DI_18_SPARE = null;
        public CSignal DI_19_SPARE = null;
        public CSignal DI_20_SPARE = null;
        public CSignal DI_21_SPARE = null;
        public CSignal DI_22_SPARE = null;
        public CSignal DI_23_SPARE = null;
        public CSignal DI_24_SPARE = null;
        public CSignal DI_25_SPARE = null;
        public CSignal DI_26_SPARE = null;
        public CSignal DI_27_SPARE = null;
        public CSignal DI_28_SPARE = null;
        public CSignal DI_29_SPARE = null;
        public CSignal DI_30_SPARE = null;
        public CSignal DI_31_SPARE = null;

        public CSignal DO_00_SWITCH_LAMP_START = null;
        public CSignal DO_01_SWITCH_LAMP_STOP = null;
        public CSignal DO_02_SPARE = null;
        public CSignal DO_03_SPARE = null;
        public CSignal DO_04_SPARE = null;
        public CSignal DO_05_SPARE = null;
        public CSignal DO_06_SPARE = null;
        public CSignal DO_07_SPARE = null;
        public CSignal DO_08_LAMP_RED = null;
        public CSignal DO_09_LAMP_YELLOW = null;
        public CSignal DO_10_LAMP_GREEN = null;
        public CSignal DO_11_BUZZER = null;
        public CSignal DO_12_SPARE = null;
        public CSignal DO_13_SPARE = null;
        public CSignal DO_14_SPARE = null;
        public CSignal DO_15_SPARE = null;
        public CSignal DO_16_TENSION_AUTO_UPPER_RELAY = null;
        public CSignal DO_17_TENSION_OUTPUT_UPPER_RELAY = null;
        public CSignal DO_18_TENSION_EMG_UPPER_RELAY = null;
        public CSignal DO_19_TENSION_AUTO_MIDDLE_RELAY = null;
        public CSignal DO_20_TENSION_OUTPUT_MIDDLE_RELAY = null;
        public CSignal DO_21_TENSION_EMG_MIDDLE_RELAY = null;
        public CSignal DO_22_TENSION_AUTO_LOWER_RELAY = null;
        public CSignal DO_23_TENSION_OUTPUT_LOWER_RELAY = null;
        public CSignal DO_24_TENSION_EMG_LOWER_RELAY = null;
        public CSignal DO_25_SPARE = null;
        public CSignal DO_26_SPARE = null;
        public CSignal DO_27_SPARE = null;
        public CSignal DO_28_SPARE = null;
        public CSignal DO_29_SPARE = null;
        public CSignal DO_30_SPARE = null;
        public CSignal DO_31_SPARE = null;
        #endregion


        public static List<CSignal> Inputs { get; set; } = new List<CSignal>();
        public static List<CSignal> Outputs { get; set; } = new List<CSignal>();

        public bool Init()
        {
            bool bRet = false;

            try
            {
              

                Inputs.Clear();

                Outputs.Clear();

                #region DI 모듈1

                //DI_00 ==> X
                //SECTION 추가 
                //SECTION 을 변수명에는 하고 추가 생성자 이름에는 안하고

                DI_00_SWITCH_START = new CSignal("SYSTEM", "DI_00_SWITCH_START", "0", false); //  A O K                
                DI_01_SWITCH_STOP = new CSignal("SYSTEM", "DI_01_SWITCH_STOP", "1", true); //  A O K                
                DI_02_SWITCH_EMG = new CSignal("SYSTEM", "DI_02_SWITCH_EMG", "2", false); //  B O K                
                DI_03_SWITCH_EMG = new CSignal("SYSTEM", "DI_03_SWITCH_EMG", "3", false); //  B O K                
                DI_04_SPARE = new CSignal("SYSTEM", "DI_04_SPARE", "4"); //  A O K                
                DI_05_SPARE = new CSignal("SYSTEM", "DI_05_SPARE", "5"); //  A O K                
                DI_06_SPARE = new CSignal("SYSTEM", "DI_06_SPARE", "6"); //  A O K                
                DI_07_SPARE = new CSignal("SYSTEM", "DI_07_SPARE", "7"); //  A O K
                DI_08_SPARE = new CSignal("SYSTEM", "DI_08_SPARE", "8"); //  A O K
                DI_09_SPARE = new CSignal("SYSTEM", "DI_09_SPARE", "9"); //  A O K
                DI_10_SPARE = new CSignal("SYSTEM", "DI_10_SPARE", "10"); //  A O K
                DI_11_SPARE = new CSignal("SYSTEM", "DI_11_SPARE", "11"); //  A O K
                DI_12_PRODUCT_DETECT_SENSOR = new CSignal("SYSTEM", "DI_12_PRODUCT_DETECT_SENSOR", "12", false); //  B O K
                DI_13_TENSION_UPPER_SERVO_ALARAM = new CSignal("SYSTEM", "DI_13_TENSION_UPPER_SERVO_ALARAM", "13"); //  A O K
                DI_14_TENSION_MIDDLE_SERVO_ALARAM = new CSignal("SYSTEM", "DI_14_TENSION_MIDDLE_SERVO_ALARAM", "14"); //  A O K
                DI_15_TENSION_LOWER_SERVO_ALARAM = new CSignal("SYSTEM", "DI_15_TENSION_LOWER_SERVO_ALARAM", "15"); //  A O K
                DI_16_SPARE = new CSignal("SYSTEM", "DI_16_SPARE", "16"); //  A O K
                DI_17_SPARE = new CSignal("SYSTEM", "DI_17_SPARE", "17"); //  A O K
                DI_18_SPARE = new CSignal("SYSTEM", "DI_18_SPARE", "18"); //  A O K
                DI_19_SPARE = new CSignal("SYSTEM", "DI_19_SPARE", "19"); //  A O K
                DI_20_SPARE = new CSignal("SYSTEM", "DI_20_SPARE", "20"); //  A O K
                DI_21_SPARE = new CSignal("SYSTEM", "DI_21_SPARE", "21"); //  A O K
                DI_22_SPARE = new CSignal("SYSTEM", "DI_22_SPARE", "22"); //  A O K
                DI_23_SPARE = new CSignal("SYSTEM", "DI_23_SPARE", "23"); //  A O K
                DI_24_SPARE = new CSignal("SYSTEM", "DI_24_SPARE", "24"); //  A O K
                DI_25_SPARE = new CSignal("SYSTEM", "DI_25_SPARE", "25"); //  A O K
                DI_26_SPARE = new CSignal("SYSTEM", "DI_26_SPARE", "26"); //  A O K
                DI_27_SPARE = new CSignal("SYSTEM", "DI_27_SPARE", "27"); //  A O K
                DI_28_SPARE = new CSignal("SYSTEM", "DI_28_SPARE", "28"); //  A O K
                DI_29_SPARE = new CSignal("SYSTEM", "DI_29_SPARE", "29"); //  A O K
                DI_30_SPARE = new CSignal("SYSTEM", "DI_30_SPARE", "30"); //  A O K
                DI_31_SPARE = new CSignal("SYSTEM", "DI_31_SPARE", "31"); //  A O K

                DO_00_SWITCH_LAMP_START = new CSignal("SYSTEM", "DO_00_SWITCH_LAMP_START", "0"); //  A O K                
                DO_01_SWITCH_LAMP_STOP = new CSignal("SYSTEM", "DO_01_SWITCH_LAMP_STOP", "1"); //  A O K                
                DO_02_SPARE = new CSignal("SYSTEM", "DO_02_SPARE", "2"); //  A O K                
                DO_03_SPARE = new CSignal("SYSTEM", "DO_03_SPARE", "3"); //  A O K                
                DO_04_SPARE = new CSignal("SYSTEM", "DO_04_SPARE", "4"); //  A O K                
                DO_05_SPARE = new CSignal("SYSTEM", "DO_05_SPARE", "5"); //  A O K                
                DO_06_SPARE = new CSignal("SYSTEM", "DO_06_SPARE", "6"); //  A O K                
                DO_07_SPARE = new CSignal("SYSTEM", "DO_07_SPARE", "7"); //  A O K
                DO_08_LAMP_RED = new CSignal("SYSTEM", "DO_08_LAMP_RED", "8"); //  A O K
                DO_09_LAMP_YELLOW = new CSignal("SYSTEM", "DO_09_LAMP_YELLOW", "9"); //  A O K
                DO_10_LAMP_GREEN = new CSignal("SYSTEM", "DO_10_LAMP_GREEN", "10"); //  A O K
                DO_11_BUZZER = new CSignal("SYSTEM", "DO_11_BUZZER", "11"); //  A O K
                DO_12_SPARE = new CSignal("SYSTEM", "DO_12_SPARE", "12"); //  A O K
                DO_13_SPARE = new CSignal("SYSTEM", "DO_13_SPARE", "13"); //  A O K
                DO_14_SPARE = new CSignal("SYSTEM", "DO_14_SPARE", "14"); //  A O K
                DO_15_SPARE = new CSignal("SYSTEM", "DO_15_SPARE", "15"); //  A O K
                DO_16_TENSION_AUTO_UPPER_RELAY = new CSignal("SYSTEM", "DO_16_TENSION_AUTO_UPPER_RELAY", "16"); //  A O K
                DO_17_TENSION_OUTPUT_UPPER_RELAY = new CSignal("SYSTEM", "DO_17_TENSION_OUTPUT_UPPER_RELAY", "17"); //  A O K
                DO_18_TENSION_EMG_UPPER_RELAY = new CSignal("SYSTEM", "DO_18_TENSION_EMG_UPPER_RELAY", "18"); //  A O K

                DO_19_TENSION_AUTO_MIDDLE_RELAY = new CSignal("SYSTEM", "DO_19_TENSION_AUTO_MIDDLE_RELAY", "19"); //  A O K
                DO_20_TENSION_OUTPUT_MIDDLE_RELAY = new CSignal("SYSTEM", "DO_20_TENSION_OUTPUT_MIDDLE_RELAY", "20"); //  A O K
                DO_21_TENSION_EMG_MIDDLE_RELAY = new CSignal("SYSTEM", "DO_21_TENSION_EMG_MIDDLE_RELAY", "21"); //  A O K

                DO_22_TENSION_AUTO_LOWER_RELAY = new CSignal("SYSTEM", "DO_22_TENSION_AUTO_LOWER_RELAY", "22"); //  A O K
                DO_23_TENSION_OUTPUT_LOWER_RELAY = new CSignal("SYSTEM", "DO_23_TENSION_OUTPUT_LOWER_RELAY", "23"); //  A O K
                DO_24_TENSION_EMG_LOWER_RELAY = new CSignal("SYSTEM", "DO_24_TENSION_EMG_LOWER_RELAY", "24"); //  A O K
                DO_25_SPARE = new CSignal("SYSTEM", "DO_25_SPARE", "25"); //  A O K
                DO_26_SPARE = new CSignal("SYSTEM", "DO_26_SPARE", "26"); //  A O K
                DO_27_SPARE = new CSignal("SYSTEM", "DO_27_SPARE", "27"); //  A O K
                DO_28_SPARE = new CSignal("SYSTEM", "DO_28_SPARE", "28"); //  A O K
                DO_29_SPARE = new CSignal("SYSTEM", "DO_29_SPARE", "29"); //  A O K
                DO_30_SPARE = new CSignal("SYSTEM", "DO_30_SPARE", "30"); //  A O K
                DO_31_SPARE = new CSignal("SYSTEM", "DO_31_SPARE", "31"); //  A O K

                /* 
              DO_00_SWITCH_LAMP_START = null;
              DO_01_SWITCH_LAMP_STOP = null;
              DO_02_SPARE = null;
              DO_03_SPARE = null;
              DO_04_SPARE = null;
              DO_05_SPARE = null;
              DO_06_SPARE = null;
              DO_07_SPARE = null;
              DO_08_LAMP_RED = null;
              DO_09_LAMP_YELLOW = null;
              DO_10_LAMP_GREEN = null;
              DO_11_BUZZER = null;
              DO_12_SPARE = null;
              DO_13_SPARE = null;
              DO_14_SPARE = null;
              DO_15_SPARE = null;
              DO_16_TENSION_UPPER_RELAY = null;
              DO_17_TENSION_UPPER_RELAY = null;
              DO_18_TENSION_UPPER_RELAY = null;
              DO_19_TENSION_MIDDLE_RELAY = null;
              DO_20_TENSION_MIDDLE_RELAY = null;
              DO_21_TENSION_MIDDLE_RELAY = null;
              DO_22_TENSION_LOWER_RELAY = null;
              DO_23_TENSION_LOWER_RELAY = null;
              DO_24_TENSION_LOWER_RELAY = null;
              DO_25_SPARE = null;
              DO_26_SPARE = null;
              DO_27_SPARE = null;
              DO_28_SPARE = null;
              DO_29_SPARE = null;
              DO_30_SPARE = null;
              DO_31_SPARE = null;
                */

                #endregion

                bRet = Open();

                if (bRet)
                {
                    m_ThreadRead.Start();
                }

                return bRet;
            }
            catch (Exception ex)
            {
                //CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                //for (int i = 0; i < Outputs.Count; i++)
                //{
                //    for (int j = 0; j < Outputs[i].Count; j++)
                //    {
                //        Off(Outputs[i][j]);
                //    }
                //}
                m_ThreadRead.Stop();
                CloseWindow();

                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        private bool Open()
        {
            try
            {
                uint uStatus = 0;

                if (CAXD.AxdInfoIsDIOModule(ref uStatus) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    if ((AXT_EXISTENCE)uStatus == AXT_EXISTENCE.STATUS_EXIST)
                    {
                        int nModuleCount = 0;

                        if (CAXD.AxdInfoGetModuleCount(ref nModuleCount) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        {
                            short i = 0;
                            int nBoardNo = 0;
                            int nModulePos = 0;
                            uint uModuleID = 0;
                            string strData = "";

                            Moudules.Clear();

                            //CAXD.AxdoSetNetworkErrorAct(0, )
                            //Module 2개 잡히는지
                            //INPUT OUTPUT 따로 잡히는지 확인
                            for (i = 0; i < nModuleCount; i++)
                            {
                                if (CAXD.AxdInfoGetModule(i, ref nBoardNo, ref nModulePos, ref uModuleID) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                                {
                                    switch ((AXT_MODULE)uModuleID)
                                    {
                                        case AXT_MODULE.AXT_SIO_DI32: strData = String.Format("[{0:D2}:{1:D2}] SIO-DI32", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DO32P: strData = String.Format("[{0:D2}:{1:D2}] SIO-DO32P", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DB32P: strData = String.Format("[{0:D2}:{1:D2}] SIO-DB32P", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DO32T: strData = String.Format("[{0:D2}:{1:D2}] SIO-DO32T", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DB32T: strData = String.Format("[{0:D2}:{1:D2}] SIO-DB32T", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDI32: strData = String.Format("[{0:D2}:{1:D2}] SIO_RDI32", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDO32: strData = String.Format("[{0:D2}:{1:D2}] SIO_RDO32", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDB128MLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDB128MLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RSIMPLEIOMLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RSIMPLEIOMLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDO16AMLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDO16AMLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDO16BMLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDO16BMLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDB96MLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDB96MLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDO32RTEX: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDO32RTEX", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDI32RTEX: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDI32RTEX", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDB32RTEX: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDB32RTEX", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DI32_P: strData = String.Format("[{0:D2}:{1:D2}] SIO-DI32_P", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DO32T_P: strData = String.Format("[{0:D2}:{1:D2}] SIO-DO32T_P", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDB32T: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDB32T", nBoardNo, i); break;
                                    }


                                    ////Logger.WriteLog(LOG.IO, $"Init the I/O Module ==> {strData}");

                                    Moudules.Add(strData);
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                IsOpen = true;
                return true;
            }
            catch (Exception ex)
            {
                //CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool On(CSignal signal)
        {
            if (!IsOpen) { return false; }
            if (signal == null) return false;

            //최선웅 선임 확인 필요
            if(signal.DELAY_BEFORE > 1) Thread.Sleep(signal.DELAY_BEFORE);
            //else Thread.Sleep(1);

           CLOG.IO($"OUTPUT ON : {signal.Name}");

            int nAddr = int.Parse(signal.Address);
            SetOutput(nAddr, 1);

            if (signal.DELAY_AFTER > 0) Thread.Sleep(signal.DELAY_AFTER);

            Thread.Sleep(1);
            return true;
        }

        public bool Off(CSignal signal)
        {
            if (!IsOpen) { return false; }
            if (signal == null) return false;

            int nAddr = int.Parse(signal.Address);

            //최선웅 선임 확인 필요
            if (signal.DELAY_BEFORE > 1) Thread.Sleep(signal.DELAY_BEFORE);
            //else Thread.Sleep(1);

           CLOG.IO($"OUTPUT OFF : {signal.Name}");

            SetOutput(nAddr, 0);

            if (signal.DELAY_AFTER > 0) Thread.Sleep(signal.DELAY_AFTER);
            return true;
        }

        public bool On(int nAddr)
        {
            SetOutput(nAddr, 1);
            return true;
        }

        public bool Off(int nAddr)
        {
            SetOutput(nAddr, 0);
            return true;
        }

        private bool SetOutput(int nAddress, uint uValue)
        {
            int nModuleCount = 0;

            CAXD.AxdInfoGetModuleCount(ref nModuleCount);

            if (nModuleCount > 0)
            {
                int nBoardNo = 0;
                int nModulePos = 0;
                uint uModuleID = 0;

                int nCurrentAddress = 0;
                int nCurrentModule = 0;

                //if (nAddress >= 0 && nAddress <= 31) { nCurrentAddress = nAddress; nCurrentModule = DO_32_BOARD_06; }
                //else if (nAddress > 31 && nAddress <= 63) { nCurrentAddress = nAddress - 32; nCurrentModule = DO_32_BOARD_07; }
                //else if (nAddress > 63 && nAddress <= 95) { nCurrentAddress = nAddress - 64; nCurrentModule = DO_32_BOARD_08; }
                //else if (nAddress > 95 && nAddress <= 127) { nCurrentAddress = nAddress - 96; nCurrentModule = DO_32_BOARD_09; }

                CAXD.AxdInfoGetModule(DO_MODULE_BASE_NO, ref nBoardNo, ref nModulePos, ref uModuleID);

                switch ((AXT_MODULE)uModuleID)
                {
                    case AXT_MODULE.AXT_SIO_DO32T:
                        CAXD.AxdoWriteOutportBit(DO_MODULE_BASE_NO, nAddress, uValue);
                        break;

                    default:
                        return false;
                }
            }

            return true;
        }

        private void CloseWindow()
        {
            if (EventThread != null)
            {
                bThread = false;
                SetEvent(hInterrupt);
                EventThread.Abort();
                EventThread = null;

            }

            CAXL.AxlClose();
        }

        #region Thread
        private CThread m_ThreadRead = null;// = new iThread(ThreadRead,this);

        private void ThreadRead(object ob)
        {
            Thread.Sleep(1);

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                short nIndex = 0;
                uint uDataHigh = 0;
                uint uDataLow = 0;
                uint uFlagHigh = 0;
                uint uFlagLow = 0;
                int nBoardNo = 0;
                int nModulePos = 0;
                uint uModuleID = 0;

                List<CSignal> Lists = new List<CSignal>();

                for (int i = 0; i < 2; i++)
                {
                    CAXD.AxdInfoGetModule(i, ref nBoardNo, ref nModulePos, ref uModuleID);
                    #region READ

                    switch ((AXT_MODULE)uModuleID)
                    {
                        case AXT_MODULE.AXT_SIO_DI32:
                            CAXD.AxdiReadInportDword(i, 0, ref uDataHigh);
                            

                            break;
                        case AXT_MODULE.AXT_SIO_DO32T:
                            CAXD.AxdoReadOutportDword(i, 0, ref uDataHigh);

                            break;
                    }

                    switch (i)
                    {
                        case DI_32_BOARD_00: { Lists = Inputs; break; }
                        case DO_32_BOARD_01: { Lists = Outputs; break; }
                    }


                    for (nIndex = 0; nIndex < 32; nIndex++)
                    {
                        // Verify the last bit value of data read
                        uFlagHigh = uDataHigh & 0x0001;

                        // Shift rightward by bit by bit
                        uDataHigh = uDataHigh >> 1;

                        if (Lists.Count > nIndex)
                        {
                            Lists[nIndex].Current = (int)uFlagHigh;
                        }
                    }

                    #endregion
                }

                stopwatch.Stop();
                string str = stopwatch.ElapsedMilliseconds.ToString();
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
        #endregion

        public bool SetUseInterrupt(bool bEnable)
        {
            try
            {
                UseInterrupt = bEnable;

                int nModuleCount = 0;

                CAXD.AxdInfoGetModuleCount(ref nModuleCount);

                if (nModuleCount > 0)
                {
                    int nBoardNo = 0;
                    int nModulePos = 0;
                    uint uModuleID = 0;

                    CAXD.AxdInfoGetModule(BoardChannel, ref nBoardNo, ref nModulePos, ref uModuleID);

                    switch ((AXT_MODULE)uModuleID)
                    {
                        case AXT_MODULE.AXT_SIO_DI32:
                        case AXT_MODULE.AXT_SIO_DB32P:
                        case AXT_MODULE.AXT_SIO_DB32T:
                            if (UseInterrupt)
                            {
                                CAXL.AxlInterruptEnable();
                                CAXD.AxdiInterruptSetModuleEnable(BoardChannel, (uint)AXT_USE.ENABLE);
                            }
                            else
                            {
                                IntPtr pEvent = (IntPtr)0;


                                CAXD.AxdiInterruptSetModuleEnable(BoardChannel, (uint)AXT_USE.DISABLE);
                                CAXL.AxlInterruptDisable();
                            }
                            break;

                        case AXT_MODULE.AXT_SIO_DO32P:
                        case AXT_MODULE.AXT_SIO_DO32T:
                        case AXT_MODULE.AXT_SIO_RDB128MLII:
                        case AXT_MODULE.AXT_SIO_RSIMPLEIOMLII:
                        case AXT_MODULE.AXT_SIO_RDO16AMLII:
                        case AXT_MODULE.AXT_SIO_RDO16BMLII:
                        case AXT_MODULE.AXT_SIO_DI32_P:
                        case AXT_MODULE.AXT_SIO_RDI32RTEX:
                        case AXT_MODULE.AXT_SIO_DO32T_P:
                        case AXT_MODULE.AXT_SIO_RDO32RTEX:
                        case AXT_MODULE.AXT_SIO_RDB32T:
                        case AXT_MODULE.AXT_SIO_RDB32RTEX:
                        case AXT_MODULE.AXT_SIO_RDB96MLII:
                            UseInterrupt = false;
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                //CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool SetUseRigingEdge(bool bEnable)
        {
            try
            {
                UseInterrupt_RigingEdge = bEnable;

                int nModuleCount = 0;

                CAXD.AxdInfoGetModuleCount(ref nModuleCount);

                if (nModuleCount > 0)
                {
                    int nBoardNo = 0;
                    int nModulePos = 0;
                    uint uModuleID = 0;

                    CAXD.AxdInfoGetModule(BoardChannel, ref nBoardNo, ref nModulePos, ref uModuleID);

                    switch ((AXT_MODULE)uModuleID)
                    {
                        case AXT_MODULE.AXT_SIO_DB32P:
                        case AXT_MODULE.AXT_SIO_DB32T:
                            if (UseInterrupt_RigingEdge)
                                CAXD.AxdiInterruptEdgeSetWord(BoardChannel, 0, (uint)AXT_DIO_EDGE.UP_EDGE, 0xFFFF);
                            else
                                CAXD.AxdiInterruptEdgeSetWord(BoardChannel, 0, (uint)AXT_DIO_EDGE.UP_EDGE, 0x0000);
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool SetUseFallingEdge(bool bEnable)
        {
            try
            {
                UseInterrupt_FallingEdge = bEnable;

                int nModuleCount = 0;

                CAXD.AxdInfoGetModuleCount(ref nModuleCount);

                if (nModuleCount > 0)
                {
                    int nBoardNo = 0;
                    int nModulePos = 0;
                    uint uModuleID = 0;

                    CAXD.AxdInfoGetModule(BoardChannel, ref nBoardNo, ref nModulePos, ref uModuleID);

                    switch ((AXT_MODULE)uModuleID)
                    {
                        case AXT_MODULE.AXT_SIO_DI32:
                            if (UseInterrupt_FallingEdge)
                            {
                                CAXD.AxdiInterruptEdgeSetWord(BoardChannel, 0, (uint)AXT_DIO_EDGE.DOWN_EDGE, 0xFFFF);
                                CAXD.AxdiInterruptEdgeSetWord(BoardChannel, 1, (uint)AXT_DIO_EDGE.DOWN_EDGE, 0xFFFF);
                            }
                            else
                            {
                                CAXD.AxdiInterruptEdgeSetWord(BoardChannel, 0, (uint)AXT_DIO_EDGE.DOWN_EDGE, 0x0000);
                                CAXD.AxdiInterruptEdgeSetWord(BoardChannel, 1, (uint)AXT_DIO_EDGE.DOWN_EDGE, 0x0000);
                            }
                            break;

                        case AXT_MODULE.AXT_SIO_DB32P:
                        case AXT_MODULE.AXT_SIO_DB32T:
                            if (UseInterrupt_FallingEdge)
                                CAXD.AxdiInterruptEdgeSetWord(BoardChannel, 0, (uint)AXT_DIO_EDGE.DOWN_EDGE, 0xFFFF);
                            else
                                CAXD.AxdiInterruptEdgeSetWord(BoardChannel, 0, (uint)AXT_DIO_EDGE.DOWN_EDGE, 0x0000);
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }
    }
}