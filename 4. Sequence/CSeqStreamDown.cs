using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentFactory
{
    public class CSeqStreamDown //배출
    {
        private IGlobal Global = IGlobal.Instance;

        private Dictionary<int, CAXIS_AJIN> Axises = null;

        public bool Init()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        #region Thread
        public CThreadStatus ThreadStatusSeq = new CThreadStatus();

        public void StartThreadSeq()
        {
            try
            {
                Axises = Global.iDevice.MOTION_AJIN.Axises;
                
                if (Axises != null)
                {
                    if(ThreadStatusSeq.IsExit())
                    {
                        SetStep(STEP_SEQ.IDLE);
                        Thread t = new Thread(new ParameterizedThreadStart(ThreadSeq));
                        t.Start(ThreadStatusSeq);
                    }
                }
                else
                {
                    
                }                
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void StopThreadSeq()
        {
            if (!ThreadStatusSeq.IsExit())
            {
                ThreadStatusSeq.Stop(100);
                ThreadStatusSeq.End();
            }
        }

        public enum STEP_SEQ : int 
        {
            IDLE = 0,
            WAIT_READY_DOWN,
            WAIT_CAN_DOWN,
            DOWN_STREAM,
            DOWN_STREAM_CHECK
        }
        public System.Diagnostics.Stopwatch m_swStepTime = new System.Diagnostics.Stopwatch();

        public void SetStep(STEP_SEQ step)
        {
            STEP_SEQ stepPrev = (STEP_SEQ)ThreadStatusSeq.STEP;
            ThreadStatusSeq.STEP = (int)step;

            if ((STEP_SEQ)ThreadStatusSeq.STEP != STEP_SEQ.IDLE) m_swStepTime.Restart();

            CLogger.Add(LOG.SEQ, "SEQ {0} STEP {1} ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, stepPrev.ToString(), step.ToString());
        }

        private void ThreadSeq(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("Scan Tray");
            CLogger.Add(LOG.NORMAL, $"THREAD START ==> {MethodBase.GetCurrentMethod().Name}");
           
            try
            {
                CMotionManager manager = Global.iDevice.MOTION_AJIN;
                Dictionary<int, CAXIS_AJIN> axises = Axises;
                CDIO_AJIN DIO = Global.iDevice.DIO_BD;

                while (!ThreadStatus.IsExit())
                {
                    Thread.Sleep(100);

                    if (Global.System.REQ_SYSTEM_CLOSE) return;

                    if (!Global.System.USE_DRY_RUN)
                    {
                        bool bIsAlarm = Global.System.AlarmWait.WaitOne(1000);

                        if (!bIsAlarm)
                        {
                            CLogger.Add(LOG.ALARM, "CHECK THE ALARM");
                            continue;
                        }

                        bool bIsPause = Global.System.PauseWait.WaitOne(10000);

                        if (!bIsPause)
                        {
                            CLogger.Add(LOG.ALARM, "CHECK THE PAUSE");
                            continue;
                        }
                    }

                    switch ((STEP_SEQ)ThreadStatus.STEP)
                    {
                        case STEP_SEQ.IDLE:
                            Thread.Sleep(300);
                            
                            SetStep(STEP_SEQ.WAIT_READY_DOWN);
                            break;
                        case STEP_SEQ.WAIT_READY_DOWN:
                            if (IGlobal.Instance.iData.SEQ_DATA.WAIT_READY_DOWN)
                            {
                                DIO.On(DIO.DO_SYSTEM_IF_DOWN_STEARM_REQUEST);
                                IGlobal.Instance.iData.SEQ_DATA.WAIT_READY_DOWN = false;
                                SetStep(STEP_SEQ.WAIT_CAN_DOWN);
                            }
                            else
                            {
                                continue;
                            }
                            break;
                        case STEP_SEQ.WAIT_CAN_DOWN:
                            {
                                if (!DIO.DI_SYSTEM_IF_DOWN_STEARM_RESPONSE.IsOn/* && !IGlobal.Instance.iData.SEQ_DATA.DOWN_RESP*/) continue;
                                DIO.On(DIO.DO_WORK_LOADING_RAIL_FEEDER_ON_OFF_OUTPUT);
                                IGlobal.Instance.iData.SEQ_DATA.DOWN_RESP = false;

                                DIO.Off(DIO.DO_SYSTEM_IF_DOWN_STEARM_REQUEST);

                                if(!DIO.DI_WORK_LOADING_RAIL_OUTPUT_STOPPER_DOWN.IsOn)
                                {
                                    DIO.On(DIO.DO_WORK_LOADING_RAIL_OUTPUT_POS_STOPPER_DOWN);
                                    DIO.Off(DIO.DO_WORK_LOADING_RAIL_OUTPUT_POS_STOPPER_UP);
                                    continue;
                                }                                

                                DIO.On(DIO.DO_WORK_LOADING_RAIL_FEEDER_ON_OFF_OUTPUT);
                            }
                            SetStep(STEP_SEQ.DOWN_STREAM);
                            break;
                        case STEP_SEQ.DOWN_STREAM:
                            {
                                if (DIO.DI_WORK_LOADING_RAIL_OUTPUT_STOPPER_POS_DETECT.IsOn) continue;
                                if (DIO.DI_WORK_LOADING_RAIL_OUTPUT_DETECT.IsOn) continue;

                                Thread.Sleep(5000);
                                DIO.Off(DIO.DO_WORK_LOADING_RAIL_FEEDER_ON_OFF_OUTPUT);

                                DIO.Off(DIO.DO_WORK_LOADING_RAIL_OUTPUT_POS_STOPPER_DOWN);
                                DIO.On(DIO.DO_WORK_LOADING_RAIL_OUTPUT_POS_STOPPER_UP);
                            }
                            SetStep(STEP_SEQ.DOWN_STREAM_CHECK);
                            break;
                        case STEP_SEQ.DOWN_STREAM_CHECK:
                            //Global.iData.SEQ_DATA.REQ_FEED_LOADING_TRAY = true;
                            //Global.SeqLoadingBuffer.StartThreadSeq();
                            SetStep(STEP_SEQ.IDLE);

                            ThreadStatus.End();
                            return;
                    }
                }

                ThreadStatus.End();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            finally
            {
                ThreadStatus.End();
            }
        }
        #endregion
    }
}
