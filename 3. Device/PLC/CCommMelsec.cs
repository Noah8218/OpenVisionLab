#if MX_COMPONENT
using ActUtlTypeLib;
using ACTMULTILib;
using OpenVisionLab;
//using OpenVisionLab.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;

using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using System.Security.Policy;

namespace OpenVisionLab
{
    public class CCommMelsec
    {
#region Event Register        
        public EventHandler<EventArgs> EventChangedConnection;
        public EventHandler<EventArgs> EventReadEnd;
#endregion


        //private ActUtlType plc;
        public ActEasyIF m_actType = new ActEasyIF();

        private bool m_bIsOpen = false;
        public bool IsOpen
        {
            get { return m_bIsOpen; }
            set
            {
                m_bIsOpen = value;

                if (EventChangedConnection != null)
                {
                    EventChangedConnection(this, new EventArgs());
                }
            }
        }

#region IO
        public enum m_JudgementData
        {
            A_GRAGE = 1, // A Grade 
            B_GRAGE = 2, // B Grade 
            C_GRAGE = 3, // C Grade 
            D_GRAGE = 4, // D Grade 
            E_GRAGE = 5, // E Grade 
            OCV_NG = 11, // OCV NG 
            RIV_NG = 12, // IRV NG
            IR_NG = 13, // IR NG
            OCVRIV_NG = 13, // IR NG
            K_VALUE_NG = 14, // K-VALUE NG
            NO_CELL_NG = 15, // NO CELL NG
            BCR_NG = 16 // BCR NG
        }
        private m_JudgementData m_Judgement = m_JudgementData.A_GRAGE;
        public m_JudgementData Judgement
        {
            get { return m_Judgement; }
            set { m_Judgement = value; }
        }


#region  Inspection Welding
        List<ISignal> ListInput = new List<ISignal>();

#endregion


        //public CSignal InputReadingInspectionBCR = null;
        //public CSignal InputStartInspectionOCV = null;
        //public CSignal InputStartInspectionIR = null;
        //public CSignal InputWidthInspectionData = null;
        //public CSignal InputStartInspection = null;

        public CSignal DI_INSP_START = null;
        public CSignal DI_MODEL_NO = null;
        public CSignal DI_MODEL_UPDATE = null;



        // 매뉴얼 Move ACK
        //public CSignal InputManualMoveACK = null;
        //public CSignal AUTO = null;
        public CSignal DO_PC_READY = null;
        public CSignal DO_INSP_START_ACK = null;
        public CSignal DO_INSP_COMPLETE = null;
        public CSignal DO_INSP_RESULT = null;
        //public CSignal DO_INSP_RESULT_NG = null;
        public CSignal DO_MODEL_NO = null;
        //public CSignal OutputAlignT = null;

        //public CSignal InputEncX = null;
        //public CSignal InputEncY = null;
        //public CSignal InputEncZ = null;
        //public CSignal InputEncT = null;

        //public CSignal OutputManualMoveX = null;
        //public CSignal OutputManualMoveY = null;
        //public CSignal OutputManualMoveZ = null;
        //public CSignal OutputManualMoveT = null;
        //public CSignal OutputManualMoveStart = null;


        //public CSignal OutputAlignMove = null;
        //public CSignal OutputAlignResult = null;
        //public CSignal OutputInspectionResult = null;

        //public CSignal OutputMoveX  = null;
        //public CSignal OutputMoveY = null;        

        //public CSignal OutputAlignAlarm = null;

        // 3074

#endregion

        public CCommMelsec()
        {

        }

        public bool InitIO(int nLogicalNumber)
        {
            try
            {
                m_actType.ActLogicalStationNumber = nLogicalNumber;
                ListInput.Clear();

                //InputReadingInspectionBCR = new ISignal("InputReadingInspectionBCR", "3010", ISignal.DEV_TYPE.LW);
                //ListInput.Add(InputReadingInspectionBCR);
                //InputStartInspectionOCV = new ISignal("InputStartInspectionOCV", "3011", ISignal.DEV_TYPE.LW);
                //ListInput.Add(InputStartInspectionOCV);
                //InputStartInspectionIR = new ISignal("InputStartInspectionIR", "3012", ISignal.DEV_TYPE.LW);
                //ListInput.Add(InputStartInspectionIR);
                //InputWidthInspectionData = new ISignal("InputWidthInspectionData", "3014", ISignal.DEV_TYPE.LW, 2);
                //ListInput.Add(InputWidthInspectionData);

                // Input

                DI_INSP_START = new ISignal("DI_INSP_START", "401", ISignal.DEV_TYPE.LW);
                ListInput.Add(DI_INSP_START);

                DI_MODEL_NO = new ISignal("DI_MODEL_NO", "403", ISignal.DEV_TYPE.LW);
                ListInput.Add(DI_MODEL_NO);

                DI_MODEL_UPDATE = new ISignal("DI_MODEL_UPDATE", "405", ISignal.DEV_TYPE.LW); // Retry
                ListInput.Add(DI_MODEL_UPDATE);


                //InputManualMoveACK = new ISignal("InputManualMoveACK", "3073", ISignal.DEV_TYPE.LW);
                //ListInput.Add(InputManualMoveACK);

                //InputEncX = new ISignal("InputEncX", "3162", ISignal.DEV_TYPE.LW, 2, "mm", 0.01D);
                //ListInput.Add(InputEncX);
                //InputEncY = new ISignal("InputEncY", "3164", ISignal.DEV_TYPE.LW, 2, "mm", 0.01D);
                //ListInput.Add(InputEncY);
                //InputEncZ = new ISignal("InputEncZ", "3166", ISignal.DEV_TYPE.LW, 2, "mm", 0.01D);
                //ListInput.Add(InputEncZ);
                //InputEncT = new ISignal("InputEncT", "3168", ISignal.DEV_TYPE.LW, 1, "mm", 0.01D);
                //ListInput.Add(InputEncT);
                //InputStartInspection = new ISignal("InputStartInspection", "3080", ISignal.DEV_TYPE.LW, 2);
                //ListInput.Add(InputStartInspection);

                //OutputReadingResultBCR = new ISignal("OutputReadingResultBCR", "3021", ISignal.DEV_TYPE.LW);
                //OutputReadingResultOCV = new ISignal("OutputReadingResultOCV", "3022", ISignal.DEV_TYPE.LW);
                //OutputReadingResultIR = new ISignal("OutputReadingResultIR", "3023", ISignal.DEV_TYPE.LW);

                // Output
                DO_PC_READY = new ISignal("DO_PC_READY", "400", ISignal.DEV_TYPE.LW);

                //DO_INSP_START_ACK = new ISignal("DO_INSP_START_ACK", "3020", ISignal.DEV_TYPE.LW);

                DO_INSP_COMPLETE = new ISignal("DO_INSP_COMPLETE", "406", ISignal.DEV_TYPE.LW);

                DO_INSP_RESULT = new ISignal("DO_INSP_RESULT", "402", ISignal.DEV_TYPE.LW);

                //DO_INSP_RESULT_NG = new ISignal("DO_INSP_RESULT_NG", "402", ISignal.DEV_TYPE.LW);

                DO_MODEL_NO = new ISignal("OutputAlignZ", "404", ISignal.DEV_TYPE.LW);

                //OutputAlignT = new ISignal("OutputAlignT", "3064", ISignal.DEV_TYPE.LW);
                //OutputAlignMove = new ISignal("OutputAlignMove", "3070", ISignal.DEV_TYPE.LW);
                //OutputAlignResult = new ISignal("OutputAlignResult", "3071", ISignal.DEV_TYPE.LW);
                //OutputManualMoveStart = new ISignal("OutputManualMoveStart", "3072", ISignal.DEV_TYPE.LW);
                //OutputAlignAlarm = new ISignal("OutputAlignAlarm", "3074", ISignal.DEV_TYPE.LW);


                //OutputManualMoveX = new ISignal("OutputManualMoveX", "3172", ISignal.DEV_TYPE.LW, 2);
                //OutputManualMoveY = new ISignal("OutputManualMoveY", "3174", ISignal.DEV_TYPE.LW, 2);
                //OutputManualMoveZ = new ISignal("OutputManualMoveZ", "3176", ISignal.DEV_TYPE.LW, 2);
                //OutputManualMoveT = new ISignal("OutputManualMoveT", "3178", ISignal.DEV_TYPE.LW, 1);

                //OutputInspectionResult = new ISignal("OutputInspectionResult", "3090", ISignal.DEV_TYPE.LW);

                //OutputMoveX = new ISignal("OutputMoveX", "3182", ISignal.DEV_TYPE.LW, 2);
                //OutputMoveY = new ISignal("OutputMoveY", "3184", ISignal.DEV_TYPE.LW, 2);


                //Logger.WriteLog(LOG.Normal, $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (Exception Desc)
            {
                //CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public bool Init()
        {
            try
            {
                InitIO(0);

                if (!m_actType.Open().Equals(CGlobal.Instance.iSystem.PlcLogicalNo))
                {
                    return false;
                }

                //if (!PingTest())
                //{
                //    return false;
                //}


                StartThreadReadInput();
                // 로그
                return true;
            }
            catch (Exception Desc)
            {
                //CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                if (m_bIsOpen)
                {
                    m_actType.Close();
                    StopThreadReadInput();
                }

                //Logger.WriteLog(LOG.Normal, $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (Exception Desc)
            {
                //CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

        }

        public bool PingTest()
        {
            Ping pingSender = new Ping();
            int nTimeout = 1000;
            // IP를 따로 빼야함

            // CLog.Error( "[Fail] - {0}", "PLC Server Ping TEST");
            return false;
        }

        public bool OutputClear()
        {
            try
            {
                if (IsOpen)
                {
                    WriteB(DO_INSP_START_ACK.Name, DO_INSP_START_ACK.Address, 0);
                    WriteB(DO_INSP_COMPLETE.Name, DO_INSP_COMPLETE.Address, 0);
                    //WriteB(OutputLeftJuge.Name, OutputLeftJuge.Address, 0);
                    //WriteB(OutputLeftResultJuge.Name, OutputLeftResultJuge.Address, 0);
                    //WriteB(OutputRightComple.Name, OutputRightComple.Address, 0);
                    //WriteB(OutputRightJuge.Name, OutputRightJuge.Address, 0);
                }

                //  Logger.WriteLog(LOG.Normal, $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return true;
            }
            catch (Exception Desc)
            {
                // CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }
        public string ReadBCR(int nAddr, int nSize = 20)
        {
            try
            {
                Thread.Sleep(10);
                int nAddress = 0;
                nAddress = nAddr;

                short[] sarrReadData = new short[20];
                string strBarcode = "";

                int[] nArrayValue = new int[40];
                int nRet = m_actType.ReadDeviceBlock(string.Format("D{0}", nAddress), 20, out nArrayValue[0]);

                if (nRet == 0)
                {
                    for (int i = 0; i < nArrayValue.Length; i++)
                    {
                        byte[] bytes = BitConverter.GetBytes(nArrayValue[i]);
                        string str = Encoding.ASCII.GetString(bytes);

                        str = str.Replace("\0", "");
                        strBarcode += str;
                    }

                }

                Console.WriteLine(strBarcode);
                // Logger.WriteLog(LOG.Normal, $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return strBarcode;
            }
            catch (Exception Desc)
            {
                //   CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return "BCR Parsing Error";
            }
        }

        public string ReadBCR(CSignal signal, int nSize = 20)
        {
            try
            {
                Thread.Sleep(1);
                int nAddress = 0;
                nAddress = int.Parse(signal.Address);

                short[] sarrReadData = new short[20];
                string strBarcode = "";

                int[] nArrayValue = new int[40];
                int nRet = m_actType.ReadDeviceBlock(string.Format("D{0}", nAddress), 20, out nArrayValue[0]);

                if (nRet == 0)
                {
                    for (int i = 0; i < nArrayValue.Length; i++)
                    {
                        byte[] bytes = BitConverter.GetBytes(nArrayValue[i]);
                        string str = Encoding.ASCII.GetString(bytes);

                        str = str.Replace("\0", "");
                        strBarcode += str;
                    }

                }

                Console.WriteLine(strBarcode);
                // Logger.WriteLog(LOG.Normal, $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                return strBarcode;
            }
            catch (Exception Desc)
            {
                //  CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return "BCR Parsing Error";
            }
        }
        public short[] IntToShort(int[] nArray)
        {
            short[] shArray = new short[nArray.Length * 2];

            for (int i = 0; i < nArray.Length; i += 2)
            {
                shArray[i] = (short)nArray[i];
                shArray[i + 1] = (short)(nArray[i] >> 16);
            }

            return shArray;
        }

        public int WriteArray(string strStartAddress, int[] nArrayData, int nSize)
        {
            try
            {
                Stopwatch swWrite = new Stopwatch();

                int nAddress = int.Parse(strStartAddress);
                short[] sBuf = IntToShort(nArrayData);
                int ret = -1;

                string strAddr = "";
                for (int i = 0; i < sBuf.Length; i++)
                {
                    strAddr += (string.Format("D{0}", nAddress + i) + "\n");
                }
                swWrite.Start();
                ret = m_actType.WriteDeviceRandom2(strAddr, sBuf.Length, ref sBuf[0]);

                swWrite.Stop();

                return ret;
            }
            catch (Exception Desc)
            {
                //  CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return -1;
            }
        }

        public int WriteB(string strName, string strAddress, int nData, int nSize = 2)
        {
            try
            {
                int nAddress = int.Parse(strAddress);

                int ret = m_actType.WriteDeviceBlock(string.Format("D{0}", nAddress), nSize, nData);

                if (ret == 0)
                {
                    //   Logger.WriteLog(LOG.IO, "[{0}] ==> {1}", strName, nData);
                }
                else
                {
                    //  CLog.Error( "[{0}] ==> FAIL", strName, nData);
                }

                return ret;
            }
            catch (Exception Desc)
            {// CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return -1;
            }
        }

        public int WriteWord(CSignal signal, int nData, int nSize = 2)
        {
            try
            {
                int nAddress = int.Parse(signal.Address);

                int ret = m_actType.WriteDeviceBlock(string.Format("D{0}", nAddress), nSize, nData);

                if (ret == 0)
                {
                    // Logger.WriteLog(LOG.IO, "[{0}] ==> {1}", signal.Name, nData);
                }
                else
                {
                    // CLog.Error( "[{0}] ==> FAIL", signal.Name, nData);
                }

                return ret;
            }
            catch (Exception Desc)
            {
                //CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return -1;
            }
        }

        public int WriteD(string strAddress, int nData, int nSize = 2)
        {
            try
            {
                int nAddress = int.Parse(strAddress);
                int ret = m_actType.WriteDeviceBlock(string.Format("D{0}", nAddress), nSize, nData);

                return ret;
            }
            catch (Exception Desc)
            {
                //  CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return -1;
            }
        }

        public int ReadD(string strAddress, out int nData, int nSize = 1)
        {
            nData = -1;
            try
            {
                int nAddress = int.Parse(strAddress);
                int ret = m_actType.ReadDeviceBlock(string.Format("D{0}", nAddress), nSize, out nData);

                return ret;
            }
            catch (Exception Desc)
            {
                //  CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return -1;
            }
        }


        public int ReadDW(string strLowAddress, string strHighAddress, out double dValue, int nSize = 1)
        {
            try
            {
                dValue = -1;
                int nValueLow = 0;
                int nValueHigh = 0;

                int nLowValue = 0;
                int nHighValue = 0;

                int ret = m_actType.ReadDeviceBlock(strLowAddress, nSize, out nValueLow);
                nLowValue = (int)nValueLow;

                if (ret != 0)
                {
                    return ret;
                }

                ret = m_actType.ReadDeviceBlock(strHighAddress, nSize, out nValueHigh);
                nHighValue = nValueHigh << 16;

                if (ret != 0)
                {
                    return ret;
                }

                dValue = nLowValue + nHighValue;
                return ret;
            }
            catch (Exception Desc)
            {
                // CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                dValue = -1;
                return -1;
            }
        }

        public int ReadB(string strAddress, int nData, int nSize = 1)
        {
            try
            {
                int ret = m_actType.ReadDeviceBlock(strAddress, nSize, out nData);

                return ret;
            }
            catch (Exception Desc)
            {
                // CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return -1;
            }
        }

#region Thread             
        private CThreadStatus m_ThreadStatusReadInput = new CThreadStatus();
        public CThreadStatus ThreadStatusReadInput
        {
            get { return m_ThreadStatusReadInput; }
        }

        private void StartThreadReadInput()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadReadInput));
            t.Start(m_ThreadStatusReadInput);
        }

        public void StopThreadReadInput()
        {
            if (!ThreadStatusReadInput.IsExit())
            {
                ThreadStatusReadInput.Stop(100);
            }
        }


        bool bIsAlive = false;
        int m_nAliveTime = 0;

        private const int START_ADDRESS = 3000;
        private const int COUNT_ADDRESS = 3200;

        private void ThreadReadInput(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("Read Input");
            //Logger.WriteLog(LOG.Inspection, "Read the Input Signal");

            int nCurrSum = 0;
            Stopwatch sw = new Stopwatch();
            while (!ThreadStatus.IsExit())
            {
                Thread.Sleep(1);
                sw.Start();
                int nTimeOut = Environment.TickCount;
                try
                {
                    if (Math.Abs(nTimeOut - Environment.TickCount) > 5000)
                    {
                        m_bIsOpen = false;
                    }
                    else
                    {
                        nTimeOut = Environment.TickCount;
                    }


                    sw.Start();
                    nCurrSum = 0;

                    short[] shArrayValue = new short[COUNT_ADDRESS];
                    int ret = -1;

                    ret = m_actType.ReadDeviceBlock2(string.Format("D{0}", START_ADDRESS), COUNT_ADDRESS, out shArrayValue[0]);

                    if (ret == 0)
                    {
                        m_bIsOpen = true;

                        for (int i = 0; i < ListInput.Count; i++)
                        {
                            CSignal InputSignal = ListInput[i];
                            string strAddr = InputSignal.Address;

                            int nWordCount = InputSignal.WordCount;
                            int nValueW = 0;

                            int nAddr = int.Parse(strAddr);
                            nAddr = nAddr - START_ADDRESS;

                            int nData = 0;
                            if (nWordCount == 1)
                            {
                                nData = shArrayValue[nAddr];
                                //nRet = m_actType.ReadDeviceBlock(string.Format("D{0}", strAddr), 2, out nValueW);
                            }
                            else
                            {
                                m_actType.ReadDeviceBlock(string.Format("D{0}", InputSignal.Address), 2, out nData);

                                for (int j = 0; j < nWordCount; j++)
                                {
                                    int nValueTemp = shArrayValue[nAddr + j];

                                    if (j == 0)
                                    {

                                    }
                                    else
                                    {
                                        nValueTemp = nValueTemp << 16;
                                    }

                                    nValueW += nValueTemp;
                                }
                            }

                            if (ret == 0)
                            {
                                InputSignal.Current = nData;
                                nCurrSum += nValueW;
                            }
                            else
                            {
                                // CLog.Error( "[FAILED] Read Input {0}==>{1} Err Code ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ret);
                            }
                        }

                        if (EventReadEnd != null)
                        {
                            EventReadEnd(null, new EventArgs());
                        }

                        //Logger.WriteLog(LOG.IO, string.Format("IO Read Time {0} : ", sw.ElapsedMilliseconds));

                        sw.Restart();
                    }
                    else
                    {
                        IsOpen = false;
                        // 로그
                    }
                }
                catch (Exception Desc)
                {
                    //  CLog.Error( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
        }
#endregion

    }
}

#endif