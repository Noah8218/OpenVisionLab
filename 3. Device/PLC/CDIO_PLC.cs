using Lib.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static OpenVisionLab.CSignal;

namespace OpenVisionLab
{
    public class CDIO_PLC
    {
        public MCProtocol.Mitsubishi.McProtocolTcp Binary = null;

        public CPropertyPlcSetting Property = new CPropertyPlcSetting();

        public bool bClear = false;

        public bool IsOpen
        {
            get
            {
                return Binary.Connected;
            }
        }

        public CDIO_PLC() { }
        #region IO 
        public CSignal DI_PLC_ALIVE_RECV = null;
        public CSignal DI_PLC_RUNNIG = null;
        public CSignal DI_PLC_STOP = null;
        public CSignal DI_PLC_ENCODER_ON = null;
        public CSignal DI_PLC_DRIVE_SPEED = null;

        public CSignal DI_PLC_INSPECTION_MODE = null;
        public CSignal DI_PLC_INSPECTION_INTERLOCK = null;
        public CSignal DI_PLC_BYPASS_MODE = null;
        public CSignal DI_PLC_END_CUT = null;

        public CSignal DO_ALIVE_SEND = null;
        public CSignal DO_PC_AUTO = null;
        public CSignal DO_PC_MANUAL = null;        
        public CSignal DO_SOL_ON_REQUEST = null;
        public CSignal DO_SOL_OFF_REQUEST = null;


        public static List<CSignal> Inputs { get; set; } = new List<CSignal>();
        public static List<CSignal> Outputs { get; set; } = new List<CSignal>();

        public bool Init(string RecipeName)
        {
            try
            {
                // Address 범위 지정 필요합니다.
                #region INPUT
                
                //////////////////////////////////////////////////////////////////////////////////////////Input 예시 
                Inputs.Clear();
                Outputs.Clear();

                DI_PLC_ALIVE_RECV = new CSignal(nameof(DI_PLC_ALIVE_RECV), "W100", DEV_TYPE.LW, 1);
                DI_PLC_RUNNIG = new CSignal(nameof(DI_PLC_RUNNIG), "W101", DEV_TYPE.LW, 1);
                DI_PLC_STOP = new CSignal(nameof(DI_PLC_STOP), "W102", DEV_TYPE.LW, 1);                
                DI_PLC_INSPECTION_MODE = new CSignal(nameof(DI_PLC_INSPECTION_MODE), "W103", DEV_TYPE.LW, 1);
                DI_PLC_INSPECTION_INTERLOCK = new CSignal(nameof(DI_PLC_INSPECTION_INTERLOCK), "W104", DEV_TYPE.LW, 1);
                DI_PLC_BYPASS_MODE = new CSignal(nameof(DI_PLC_BYPASS_MODE), "W105", DEV_TYPE.LW, 1);
                DI_PLC_END_CUT = new CSignal(nameof(DI_PLC_END_CUT), "W107", DEV_TYPE.LW, 1);
                DI_PLC_ENCODER_ON = new CSignal(nameof(DI_PLC_ENCODER_ON), "W108", DEV_TYPE.LW, 1);
                DI_PLC_DRIVE_SPEED = new CSignal(nameof(DI_PLC_DRIVE_SPEED), "W110", DEV_TYPE.LW, 1);

                if (CGlobal.Inst.Data.SETTING.PC_MODE == 1)
                {
                    OUTPUT_START_DEVICE_ADDRESS = 200;
                    OUTPUT_End_DEVICE_ADDRESS = 250;
                    DO_ALIVE_SEND = new CSignal(nameof(DO_ALIVE_SEND), "W200", DEV_TYPE.LW, 1);
                    DO_PC_AUTO = new CSignal(nameof(DO_PC_AUTO), "W201", DEV_TYPE.LW, 1);
                    DO_PC_MANUAL = new CSignal(nameof(DO_PC_MANUAL), "W202", DEV_TYPE.LW, 1);                    
                }
                else
                {
                    OUTPUT_START_DEVICE_ADDRESS = 300;
                    OUTPUT_End_DEVICE_ADDRESS = 350;
                    DO_ALIVE_SEND = new CSignal(nameof(DO_ALIVE_SEND), "W300", DEV_TYPE.LW, 1);
                    DO_PC_AUTO = new CSignal(nameof(DO_PC_AUTO), "W301", DEV_TYPE.LW, 1);
                    DO_PC_MANUAL = new CSignal(nameof(DO_PC_MANUAL), "W302", DEV_TYPE.LW, 1);                    
                }

                DO_SOL_ON_REQUEST = new CSignal(nameof(DO_SOL_ON_REQUEST), "W208", DEV_TYPE.LW, 1);
                DO_SOL_OFF_REQUEST = new CSignal(nameof(DO_SOL_OFF_REQUEST), "W209", DEV_TYPE.LW, 1);

                #endregion
                #region OUTPUT

                #endregion
                Property = Property.LoadConfig(RecipeName);
                Binary = new MCProtocol.Mitsubishi.McProtocolTcp(Property.HostIP, Property.HostPort, MCProtocol.Mitsubishi.McFrame.MC3E);

                if (PingTest())
                {
                    Binary.Open();
                    StartThreadRead();
                }
                else { return false; }               
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return false;
            }
        }

        public bool PingTest()
        {
            Ping pingSender = new Ping();
            int nTimeout = 1000;
            // IP를 따로 빼야함

            // plc 서버 ip 필요
            // 하드코딩된 ip 전부 레시피로 빼기
            // 적용
            PingReply reply = pingSender.Send(Property.HostIP, nTimeout);

            if (reply.Status == IPStatus.Success)
            {
                //Logger.WriteLog(LOG.Normal, "[OK] - {0}", "PLC Server Ping TEST");
                return true;
            }
            // IP를 따로 빼야함

            //Logger.WriteLog(LOG.AbNormal, "[Fail] - {0}", "PLC Server Ping TEST");
            return false;
        }

        public bool Close()
        {
            try
            {
                if (Binary != null)
                {
                    Binary.Close();
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return false;
            }

            return true;
        }
        private CThreadStatus m_ThreadStatusRead = new CThreadStatus();
        public CThreadStatus ThreadStatusRead
        {
            get { return m_ThreadStatusRead; }
        }
        private Stopwatch m_Timeout = new Stopwatch();
        public Stopwatch PlcConnectionTimeout
        {
            get => m_Timeout;
            set
            {
                m_Timeout = value;
            }
        }


        public void StartThreadRead()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadRead));
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
        private int INPUT_START_DEVICE_ADDRESS = 100;
        private int INPUT_End_DEVICE_ADDRESS = 150;

        private int OUTPUT_START_DEVICE_ADDRESS = 200;
        private int OUTPUT_End_DEVICE_ADDRESS = 350;

        /// <summary>
        /// 해당 주소값으로부터 값을 읽어옵니다 input / output주소값이 나눠져 있어서 나눠서 읽고 어드레스별로 값을 써줍니다.
        /// </summary>
        /// <param name="ob"></param>
        private void ThreadRead(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("Read Io");
            CLOG.NORMAL("Read the Io Signal");

            Thread.Sleep(30);
            try
            {
                while (!ThreadStatus.IsExit())
                {
                    if ((Environment.TickCount - m_nAliveTime) > 2500) { m_nAliveTime = Environment.TickCount; }

                    Thread.Sleep(1);

                    try
                    {
                        if (Binary.Connected)
                        {

                            #region Input
                            int nLengthInput = INPUT_End_DEVICE_ADDRESS - INPUT_START_DEVICE_ADDRESS;
                            int[] nArrReadDeviceDataInput = new int[nLengthInput];
                            // PLC 테스트 : Read 10~50ms
                            // 1 ~ 950개까지 읽을 수 있음--> 한번 읽을때마다 30~50ms
                            // W4800 ~ 읽기 시작
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            Binary.ReadDeviceBlock("W100", nLengthInput, nArrReadDeviceDataInput);

                            sw.Stop();
                            string str = sw.ElapsedMilliseconds.ToString();
                            for (int i = 0; i < Inputs.Count; i++)
                            {
                                try
                                {
                                    CSignal InputSignal = Inputs[i];

                                    if(InputSignal.Name == "DI_PLC_DRIVE_SPEED")
                                    {

                                    }

                                    // 1000자리인지, 100자리인지에 따라 달라짐
                                    string Address = InputSignal.Address.Substring(1);
                                    string High = "";
                                    string Low = "";

                                    if (Address.Length == 3)
                                    {
                                        High = Address.Substring(0, Address.Length - 2);
                                        Low = Address.Substring(Address.Length - 2, Address.Length - 1);
                                    }
                                    else if(Address.Length == 4)    
                                    {
                                        High = Address.Substring(0, Address.Length - 2);
                                        Low = Address.Substring(2, 2);
                                    }
                                    
                                    string LowDec = Convert.ToInt32(Low, 16).ToString();

                                    string Result = "";
                                    if (LowDec.Length == 1 && LowDec != "0")
                                    {
                                        LowDec = LowDec.PadLeft(2, '0');
                                        Result = (High + LowDec);
                                    }
                                    else
                                    {
                                        LowDec = LowDec.PadRight(2, '0');
                                        Result = (High + LowDec);
                                    }

                                    int nSignalArray = Math.Abs(INPUT_START_DEVICE_ADDRESS - int.Parse(Result));

                                    //string strAddr = InputSignal.Address;
                                    string strAddr = "W" + Result;

                                    int nWordCount = InputSignal.WordCount;
                                    int nValueW = 0;
                                    int[] nDataArr = new int[nWordCount];
                                    if (nWordCount == 1) { InputSignal.Current = nArrReadDeviceDataInput[nSignalArray]; }
                                    else
                                    {
                                        nDataArr[0] = nArrReadDeviceDataInput[nSignalArray];
                                        nDataArr[1] = nArrReadDeviceDataInput[nSignalArray + 1];
                                        List<short> listValue = new List<short>();
                                        int nSum = 0;
                                        int shBuffTemp = 0;
                                        int nBuffTemp = 0;

                                        for (int k = 0; k < nDataArr.Length; k++)
                                        {
                                            shBuffTemp = nDataArr[k];
                                            ushort ushBuffTemp = (ushort)shBuffTemp;
                                            nBuffTemp = (int)ushBuffTemp;

                                            if (k == 0) { }
                                            else { nBuffTemp = ((int)nBuffTemp << (16 * k)); }
                                            nSum += nBuffTemp;
                                        }

                                        nValueW = nSum;
                                        InputSignal.Current = nValueW;
                                    }
                                }
                                catch
                                {

                                }

                            }
                            #endregion

                            int nLengthOutput = OUTPUT_End_DEVICE_ADDRESS - OUTPUT_START_DEVICE_ADDRESS;
                            int[] nArrReadDeviceDataOutput = new int[nLengthOutput];
                            // PLC 테스트 : Read 10~50ms
                            // 1 ~ 950개까지 읽을 수 있음--> 한번 읽을때마다 30~50ms                            
                            Binary.ReadDeviceBlock("W200", nLengthOutput, nArrReadDeviceDataOutput);

                            #region Output 
                            for (int i = 0; i < Outputs.Count; i++)
                            {
                                try
                                {
                                    CSignal OutputSignal = Outputs[i];
                                    // 1000자리인지, 100자리인지에 따라 달라짐
                                    string Address = OutputSignal.Address.Substring(1);
                                    string High = "";
                                    string Low = "";

                                    if (Address.Length == 3)
                                    {
                                        High = Address.Substring(0, Address.Length - 2);
                                        Low = Address.Substring(Address.Length - 2, Address.Length - 1);
                                    }
                                    else if (Address.Length == 4)
                                    {
                                        High = Address.Substring(0, Address.Length - 2);
                                        Low = Address.Substring(2, 2);
                                    }

                                    string LowDec = Convert.ToInt32(Low, 16).ToString();
                                    string Result = "";
                                    if (LowDec.Length == 1 && LowDec != "0")  
                                    {
                                        LowDec = LowDec.PadLeft(2, '0');
                                        Result = (High + LowDec);
                                    }
                                    else
                                    {
                                        LowDec = LowDec.PadRight(2, '0');
                                        Result = (High + LowDec);
                                    }

                                    int nSignalArray = 0;
                                    nSignalArray = Math.Abs(OUTPUT_START_DEVICE_ADDRESS - int.Parse(Result));

                                    //string strAddr = OutputSignal.Address;
                                    string strAddr = "W" + Result;
                                    int nWordCount = OutputSignal.WordCount;
                                    int nValueW = 0;
                                    int[] nDataArr = new int[nWordCount];
                                    if (nWordCount == 1) { OutputSignal.Current = nArrReadDeviceDataOutput[nSignalArray]; }
                                    else
                                    {
                                        nDataArr[0] = nArrReadDeviceDataOutput[nSignalArray];
                                        nDataArr[1] = nArrReadDeviceDataOutput[nSignalArray + 1];
                                        List<short> listValue = new List<short>();
                                        int nSum = 0;
                                        int shBuffTemp = 0;
                                        int nBuffTemp = 0;

                                        for (int k = 0; k < nDataArr.Length; k++)
                                        {
                                            shBuffTemp = nDataArr[k];
                                            ushort ushBuffTemp = (ushort)shBuffTemp;
                                            nBuffTemp = (int)ushBuffTemp;

                                            if (k == 0) { }
                                            else { nBuffTemp = ((int)nBuffTemp << (16 * k)); }
                                            nSum += nBuffTemp;
                                        }

                                        nValueW = nSum;
                                        OutputSignal.Current = nValueW;
                                    }

                                }
                                catch
                                {

                                }

                            }
                            #endregion
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
                ThreadStatus.End();
            }

        }
        #endregion
        /// <summary>
        ///  주소값에 1을 씁니다.
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public bool On(CSignal signal, bool WriteLog = true)
        {
            try
            {
                if (Binary.Connected)
                {
                    if (signal != null)
                    {
                        Task<int> nCode = Binary.SetDevice(signal.Address, CSignal.SIGNAL_ON);

                        if (nCode.Result != 0)
                        {
                            string strError = string.Format("PLC Comm Error / Code : {0}", nCode.Result);
                            CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {strError}");
                        }

                        if(WriteLog)
                        {
                            string s = "";
                            if (signal.Current == CSignal.SIGNAL_ON) { s = "ON"; }
                            else { s = "OFF"; }
                            CLOG.IO($"[{signal.Name}] {s}");
                        }                        
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }
        /// <summary>
        /// 주소값에 0을 씁니다.
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public bool Off(CSignal signal, bool WriteLog = true)
        {
            try
            {
                if (Binary.Connected)
                {
                    if (signal != null)
                    {
                        Task<int> nCode = Binary.SetDevice(signal.Address, CSignal.SIGNAL_OFF);

                        if (nCode.Result != 0)
                        {
                            string strError = string.Format("PLC Comm Error / Code : {0}", nCode.Result);
                            CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Ex ==> {strError}");                            
                        }
                        if(WriteLog)
                        {
                            string s = "";
                            if (signal.Current == CSignal.SIGNAL_ON) { s = "ON"; }
                            else { s = "OFF"; }
                            CLOG.IO($"[{signal.Name}] {s}");
                        }                        
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }
        /// <summary>
        /// 단일 워드값을 씁니다.
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="nData"></param>
        /// <returns></returns>
        public bool WriteWord(CSignal signal, int nData)
        {
            try
            {
                if (Binary.Connected)
                {
                    if (signal != null)
                    {
                        Binary.SetDevice(signal.Address, nData);
                        //CLOG.IO( "Set Device [{0}] {1} ==> {2}", signal.Name, signal.Current.ToString(), nData);
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }
        public bool WriteWord(string Address, int nData)
        {
            try
            {
                if (Binary.Connected) { Binary.SetDevice(Address, nData); }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }
        public string GetRecipe(CSignal signal, int nLength, out int[] nArrReadDeviceData2)
        {
            string strBarcode = string.Empty;
            nArrReadDeviceData2 = new int[nLength];
            try
            {
                int[] nArrReadDeviceData = new int[nLength];

                //var nCode = Binary.ReadDeviceBlock(signal.Address, nLength, nArrReadDeviceData);
                Binary.ReadDeviceBlock(signal.Address, nLength, nArrReadDeviceData);

                nArrReadDeviceData2 = nArrReadDeviceData;

                for (int i = 0; i < nArrReadDeviceData.Length; i++)
                {
                    byte[] bytes = BitConverter.GetBytes(nArrReadDeviceData[i]);
                    string str = Encoding.ASCII.GetString(bytes);

                    str = str.Replace("\0", "");
                    strBarcode += str;
                }

                return strBarcode;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
            }

            return strBarcode;
        }
        /// <summary>
        /// 단일 워드값을 읽어옵니다.
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="nData"></param>
        /// <returns></returns>
        public bool ReadWord(CSignal signal, out int nData)
        {
            nData = -1;
            try
            {
                if (Binary.Connected)
                {
                    if (signal != null)
                    {
                        int[] oData = new int[1];

                        Binary.ReadDeviceBlock(signal.Address, 1, oData);

                        nData = oData[0];

                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }
        /// <summary>
        /// 랭스값만큼 값을 읽고 아스키로 값을 변환하여 리턴합니다. 어드레스 타입은 W입니다.
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="nLength"></param>
        /// <returns></returns>
        /// 
        //public int[] nArrReadDeviceDataAll = new int[100];
        public int[] nArrReadDeviceDataAll = new int[250];
        public int nStartWriteDevice = 0;
        public string GetBCR(CSignal signal, int nLength, out string[] barcodes)
        {
            barcodes = new string[26];
            string strBarcode = string.Empty;
            try
            {
                int[] nArrReadDeviceData = new int[nLength];

                int n1 = Environment.TickCount;

                if (Binary.Connected)
                {
                    var nCode = Binary.ReadDeviceBlock(signal.Address, nLength, nArrReadDeviceData);
                }

                int n2 = Environment.TickCount - n1;
                //if (nCode != 0) return strBarcode;

                int nCount = 0;
                int nCount2 = 0;
                for (int i = 0; i < nArrReadDeviceData.Length; i++)
                {
                    byte[] bytes = BitConverter.GetBytes(nArrReadDeviceData[i]);
                    string str = Encoding.ASCII.GetString(bytes);

                    str = str.Replace("\0", "");
                    strBarcode += str;
                    nCount++;
                    if (nCount == 16)
                    {
                        barcodes[nCount2] = (string)strBarcode.Clone();
                        strBarcode = string.Empty;
                        nCount = 0;
                        nCount2++;
                    }
                }

                return strBarcode;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
            }

            return strBarcode;
        }
        public string GetModel(CSignal signal, int nLength, out string barcodes)
        {
            barcodes = "";
            string strBarcode = string.Empty;
            try
            {
                int[] nArrReadDeviceData = new int[nLength];

                int n1 = Environment.TickCount;

                if (Binary.Connected)
                {
                    var nCode = Binary.ReadDeviceBlock(signal.Address, nLength, nArrReadDeviceData);
                }

            
                for (int i = 0; i < nArrReadDeviceData.Length; i++)
                {
                    byte[] bytes = BitConverter.GetBytes(nArrReadDeviceData[i]);
                    string str = Encoding.ASCII.GetString(bytes);

                    str = str.Replace("\0", "");
                    strBarcode += str;
                }

                return strBarcode;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
            }

            return strBarcode;
        }


        public bool WriteImagePath(CSignal signal, string strData)
        {
            try
            {
                if (Binary.Connected)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(strData);

                    int nImageLength = (bytes.Length / 2) + 1;

                    int[] nData = new int[nImageLength];

                    int nDoubleIndex = 0;
                    for (int i = 0; i < nData.Length; i++)
                    {
                        try
                        {
                            if (nDoubleIndex < bytes.Length)
                            {
                                nData[i] += (byte)bytes[nDoubleIndex];
                                if (nDoubleIndex + 1 < bytes.Length)
                                {
                                    nData[i] += (byte)bytes[nDoubleIndex + 1] << 8;
                                }
                                nDoubleIndex += 2;
                            }
                        }
                        catch (Exception Desc)
                        {
                            CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                        }

                    }
                    Task<int> nCode = Binary.WriteDeviceBlock(signal.Address, nData.Length, nData);

                    if (nCode.Result != 0)
                    {
                        string strError = string.Format("PLC Comm Error / Code : {0}", nCode.Result);
                        CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Ex ==> {strError}");
                    }
                }
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
            }

            return false;
        }
        public void WriteDeviceClear(CSignal signal, int nLength)
        {
            if (Binary.Connected)
            {
                int[] nData = new int[nLength];

                // 주의 900이상의 랭스는 넣지말아야함 짜놓은 구조상 그 이상은 에러가 발생할것임
                Task<int> nCode = Binary.WriteDeviceBlock(signal.Address, nData.Length, nData);

                if (nCode.Result != 0)
                {
                    string strError = string.Format("PLC Comm Error / Code : {0}", nCode.Result);
                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Ex ==> {strError}");
                }
            }
        }
        public bool WriteBCRALL()
        {
            try
            {
                //Binary.SetBitDevice(signal.Address, bytes.Length, bytes);
                Task<int> nCode = Binary.WriteDeviceBlock("9600", nArrReadDeviceDataAll.Length, nArrReadDeviceDataAll);

                if (nCode.Result != 0)
                {
                    string strError = string.Format("PLC Comm Error / Code : {0}", nCode.Result);
                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Ex ==> {strError}");
                }

                // 10개값 초기화
                nArrReadDeviceDataAll = new int[250];
                //nArrReadDeviceDataAll = new int[100];
                nStartWriteDevice = 0;

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
            }

            return false;
        }
        public bool WriteBCR(CSignal signal, string strData)
        {
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(strData);
                int[] nData = new int[bytes.Length];

                for (int i = 0; i < nData.Length; i++)
                {
                    nData[i] = (byte)bytes[i];
                }

                string str = Encoding.ASCII.GetString(bytes);

                str = str.Replace("\0", "");

                string strAddress = "D" + signal.Address;

                //Binary.SetBitDevice(signal.Address, bytes.Length, bytes);
                Task<int> nCode = Binary.WriteDeviceBlock(strAddress, nData.Length, nData);

                if (nCode.Result != 0)
                {
                    string strError = string.Format("PLC Comm Error / Code : {0}", nCode.Result);
                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Ex ==> {strError}");
                }

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
            }

            return false;
        }
        public bool WriteDeviceBlock(CSignal signal, int[] nData)
        {
            try
            {
                if (Binary.Connected)
                {
                    Task<int> nCode = Binary.WriteDeviceBlock(signal.Address, nData.Length, nData);

                    string dataArr = "";
                    foreach(int val in nData)
                    {
                        dataArr += val.ToString("X4") + " ";
                    }
                    CLOG.IO( $"[PLC] signal Addres={signal.Address} nData Length={nData.Length}, nData= {dataArr} ");

                    
                    if (nCode.Result != 0) return false;
                }
                else { return false; }

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }
        }
        public bool AllOff()
        {
            try
            {
                if (Binary.Connected)
                {
                    //Off(OutputHeaveyAlarm);
                    //Off(OutputInspectionComplete);
                    ////Off(OutputInspectionJudgement);
                    //Off(OutputInspectionStart);
                    //Off(OutputOperationMode);
                    //Off(OutputWarningAlarm);
                    //Off(OutputHeaveyAlarm);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }
        //#endregion   
        public void SetAddress(string strIp, int nPort)
        {
            Property.HostIP = strIp;
            Property.HostPort = nPort;
            //ILogger.Add(LOG_TYPE.SYSTEM, $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
        }
        public void GetAddress(out string strIp, out int nPort)
        {
            strIp = Property.HostIP;
            nPort = Property.HostPort;
        }      
    }
}
