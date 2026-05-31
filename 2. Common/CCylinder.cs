using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;

namespace KtemVisionSystem
{
    public class CCylinder
    {
        public enum STATUS : int { IDLE = 0, FWD = 1, BKD = 2, ERROR = 3 };
        public STATUS Status = STATUS.IDLE;

        public long TIME_LAST = 0;

        public string NAME = "";
        public bool IsClose = false;

        public bool IsDoubleActingCylinder = false;
        public bool IsExistsCheckSensor = false;
        public bool IsUpDown = false;

        public CSignal SignalOutput = null;
        public CSignal SignalOutput_FWD = null;
        public CSignal SignalOutput_BKD = null;

        public CSignal SignalInput_FWD = null;
        public CSignal SignalInput_BKD = null;

        public Stopwatch m_swTimeOut = new Stopwatch();
        public long TIME_OUT = 30000;

        private const int DO_32_BOARD_06 = 6;
        private const int DO_32_BOARD_07 = 7;
        private const int DO_32_BOARD_08 = 8;
        private const int DO_32_BOARD_09 = 9;
        public CCylinder(string strName, CSignal signalOutput, CSignal signalInput_FWD, CSignal signalInput_BKD, long lTimeOut = 30000)
        {
            try
            {
                NAME = strName;
                IsDoubleActingCylinder = false;

                SignalOutput = signalOutput;

                SignalInput_FWD = signalInput_FWD;
                SignalInput_BKD = signalInput_BKD;

                
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public CCylinder(string strName, CSignal signalOutput_FWD, CSignal signalOutput_BKD, CSignal signalInput_FWD, CSignal signalInput_BKD, bool bIsUpdown = false, long lTimeOut = 30000)
        {
            try
            {
                NAME = strName;
                IsDoubleActingCylinder = true;

                SignalOutput_FWD = signalOutput_FWD;
                SignalOutput_BKD = signalOutput_BKD;

                SignalInput_FWD = signalInput_FWD;
                SignalInput_BKD = signalInput_BKD;

                IsUpDown = bIsUpdown;

                if (signalInput_FWD != null && signalInput_BKD != null)
                {
                    IsExistsCheckSensor = true;
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public bool Close()
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
            finally
            {
                IsClose = true;
            }

            return true;
        }

        public bool CheckPosition(bool bFWD)
        {
            try
            {
                if (bFWD)
                {
                    if (IsDoubleActingCylinder)
                    {
                        if (SignalOutput_FWD == null)
                        {
                            //CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_REGIST_NULL, "REGIST FWD IS NULL"));
                            return false;
                        }

                        if (SignalOutput_BKD == null)
                        {
                            ///CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_REGIST_NULL, "REGIST BKD IS NULL"));
                            return false;
                        }

                        if (bFWD)
                        {
                            if (SignalOutput_FWD.IsOn && !SignalOutput_BKD.IsOn) return true;
                            else return false;
                        }
                        else
                        {
                            if (!SignalOutput_FWD.IsOn && SignalOutput_BKD.IsOn) return true;
                            else return false;
                        }
                    }
                    else
                    {
                        if (SignalOutput == null)
                        {
                            //CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_REGIST_NULL, "REGIST OUTPUT IS NULL"));
                            return false;
                        }

                        if (bFWD)
                        {
                            if (SignalOutput.IsOn) return true;
                            else return false;
                        }
                        else
                        {
                            if (!SignalOutput.IsOn) return true;
                            else return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Status = STATUS.ERROR;
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        //2021-11-21 송현수
        //실린더 확인 -> 동작 -> 타임아웃 루틴
        public bool CheckWithAction(bool bFWD)
        {
            try
            {
                if (IsDoubleActingCylinder)
                {
                    if (SignalOutput_FWD == null)
                    {
                        //CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_REGIST_NULL, "REGIST FWD IS NULL"));
                        return false;
                    }

                    if (SignalOutput_BKD == null)
                    {
                        //CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_REGIST_NULL, "REGIST BKD IS NULL"));
                        return false;
                    }

                    if (bFWD)
                    {
                        if (SignalOutput_FWD.IsOn && !SignalOutput_BKD.IsOn) return true;
                        else
                        {
                            Status = STATUS.FWD;
                            On(SignalOutput_FWD);
                            Off(SignalOutput_BKD);

                            if (!m_bTimeOutChecking && CGlobal.Inst.System.Mode == CSystem.MODE.AUTO)
                            {
                                m_swTimeOut.Restart();
                                Task.Run(() => TaskCheck());
                            }
                            return false;
                        }
                    }
                    else
                    {
                        if (!SignalOutput_FWD.IsOn && SignalOutput_BKD.IsOn) return true;
                        else
                        {
                            Status = STATUS.BKD;
                            Off(SignalOutput_FWD);
                            On(SignalOutput_BKD);

                            if (!m_bTimeOutChecking && CGlobal.Inst.System.Mode == CSystem.MODE.AUTO)
                            {
                                m_swTimeOut.Restart();
                                Task.Run(() => TaskCheck());
                            }
                            return false;
                        }
                    }
                }
                else
                {
                    if (SignalOutput == null)
                    {
                        //CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_REGIST_NULL, "REGIST OUTPUT IS NULL"));
                        return false;
                    }

                    if (bFWD)
                    {
                        if (SignalOutput.IsOn) return true;
                        else
                        {
                            On(SignalOutput);
                            return false;
                        }
                    }
                    else
                    {
                        if (!SignalOutput.IsOn) return true;
                        else
                        {
                            Off(SignalOutput);
                            return false;
                        }
                    }
                }

        
            }
            catch (Exception ex)
            {
                Status = STATUS.ERROR;
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return false;
        }

        public bool Set(bool bFWD)
        {
            try
            {
                bool bFail = false;
                if(bFWD)
                {
                    if (IsDoubleActingCylinder)
                    {
                        if (SignalOutput_FWD != null) On(SignalOutput_FWD);
                        else bFail = true;

                        if (SignalOutput_BKD != null) Off(SignalOutput_BKD);
                        else bFail = true;
                    }
                    else
                    {
                        if (SignalOutput != null) On(SignalOutput);
                        else bFail = true;
                    }

                    if (bFail)
                    {
                        Status = STATUS.ERROR;
                        CLogger.Add(LOG.IO, "[ERROR] [{0}] CYLINDER REGIST", NAME);
                    }
                    else
                    {
                        Status = STATUS.FWD;
                        CLogger.Add(LOG.IO, "[SUCCESS] [{0}] CYLINDER FWD", NAME);
                    }
                }
                else
                {
                    if (IsDoubleActingCylinder)
                    {
                        if (SignalOutput_FWD != null) On(SignalOutput_BKD);
                        else bFail = true;

                        if (SignalOutput_BKD != null) Off(SignalOutput_FWD);
                        else bFail = true;
                    }
                    else
                    {
                        if (SignalOutput != null) Off(SignalOutput);
                        else bFail = true;
                    }

                    if (bFail)
                    {
                        Status = STATUS.ERROR;
                        CLogger.Add(LOG.IO, "[ERROR] [{0}] CYLINDER REGIST", NAME);
                    }
                    else
                    {
                        Status = STATUS.BKD;
                        CLogger.Add(LOG.IO, "[SUCCESS] [{0}] CYLINDER BKD", NAME);
                    }
                }

                if(!bFail)
                {
                    if(CGlobal.Inst.System.Mode == CSystem.MODE.AUTO)
                    {
                        m_swTimeOut.Restart();
                        Task.Run(() => TaskCheck());
                    }
                }
            }
            catch (Exception ex)
            {
                Status = STATUS.ERROR;
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        private bool m_bTimeOutChecking = false;
        public void TaskCheck()
        {
            m_bTimeOutChecking = true;
            while (!IsClose)
            {
                Thread.Sleep(1);

                if (Status == STATUS.ERROR) break;

                if(m_swTimeOut.ElapsedMilliseconds > TIME_OUT)
                {
                    //CAlarm.Add(new CNodeAlarm("TIMEOUT", $"{NAME} TIME-OUT"));
                    break;
                }

                if (SignalInput_FWD == null || SignalInput_BKD == null)
                {
                    Status = STATUS.ERROR;
                    CLogger.Add(LOG.IO, "[ERROR] [{0}] CYLINDER REGIST", NAME);
                    break;
                }

                TIME_LAST = m_swTimeOut.ElapsedMilliseconds;

                if (Status == STATUS.FWD)
                {
                    if (SignalInput_FWD.IsOn && !SignalInput_BKD.IsOn) break;
                }
                else if (Status == STATUS.BKD)
                {
                    if (!SignalInput_FWD.IsOn && SignalInput_BKD.IsOn) break;
                }
            }

            m_bTimeOutChecking = false;
            TIME_LAST = m_swTimeOut.ElapsedMilliseconds;

            m_swTimeOut.Reset();
        }

        public bool On(CSignal signal)
        {
            if (signal == null) return false;

            int nAddr = int.Parse(signal.Address);
            SetOutput(nAddr, 1);
            return true;
        }

        public bool Off(CSignal signal)
        {
            if (signal == null) return false;

            int nAddr = int.Parse(signal.Address);
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

                if (nAddress >= 0 && nAddress <= 31) { nCurrentAddress = nAddress; nCurrentModule = DO_32_BOARD_06; }
                else if (nAddress > 31 && nAddress <= 63) { nCurrentAddress = nAddress - 32; nCurrentModule = DO_32_BOARD_07; }
                else if (nAddress > 63 && nAddress <= 95) { nCurrentAddress = nAddress - 64; nCurrentModule = DO_32_BOARD_08; }
                else if (nAddress > 95 && nAddress <= 127) { nCurrentAddress = nAddress - 96; nCurrentModule = DO_32_BOARD_09; }

                CAXD.AxdInfoGetModule(nCurrentModule, ref nBoardNo, ref nModulePos, ref uModuleID);

                switch ((AXT_MODULE)uModuleID)
                {
                    case AXT_MODULE.AXT_SIO_RDO32RTEX:
                        CAXD.AxdoWriteOutportBit(nCurrentModule, nCurrentAddress, uValue);
                        break;

                    default:
                        return false;
                }
            }

            return true;
        }
        private bool SetByAjin(int nAddress, uint uValue)
        {
            int nModuleCount = 0;

            CAXD.AxdInfoGetModuleCount(ref nModuleCount);

            if (nModuleCount > 0)
            {
                int nBoardNo = 0;
                int nModulePos = 0;
                uint uModuleID = 0;

                int nBaseModuleNo = CDIO_AJIN.DO_MODULE_BASE_NO;
                int nSetModuleNo = nBaseModuleNo + (nAddress / 32);
                int nSetAddress = nAddress % 32;

                CAXD.AxdInfoGetModule(nSetModuleNo, ref nBoardNo, ref nModulePos, ref uModuleID);

                switch ((AXT_MODULE)uModuleID)
                {
                    case AXT_MODULE.AXT_SIO_RDO32RTEX:
                        CAXD.AxdoWriteOutportBit(nSetModuleNo, nSetAddress, uValue);
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }
    }
}
