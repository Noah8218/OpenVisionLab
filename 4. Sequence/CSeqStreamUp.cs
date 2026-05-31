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
    public class CSeqStreamUp //재투입
    {
        private IGlobal Global = IGlobal.Instance;

        private Dictionary<int, CAXIS_AJIN> Axises = null;

        //private int CURRENT_BUFFER_INDEX = 0;
        
        private int PITCH_Z = 30;


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
                    if (ThreadStatusSeq.IsExit())
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
            WAIT_UP,
            WAIT_UP_RESP,
            INPUT_CHECK_SENSOR,
            WORK_FEEDER_MOVING_CYL_FW,
            WAIT_RELAY_LIFT_CYL_CHECK,
            BUFFER_F_START,
            OUTPUT_CHECK_SENSOR,
            POS_DETECT,
            BYPASS,
            CONV_RUN,

            //?
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

            ThreadStatus.Start("STREAM UP");
            CLogger.Add(LOG.NORMAL, $"THREAD START ==> {MethodBase.GetCurrentMethod().Name}");

            try
            {
                CMotionManager manager = Global.iDevice.MOTION_AJIN;
                Dictionary<int, CAXIS_AJIN> axises = Axises;
                CDIO_AJIN DIO = Global.iDevice.DIO_BD;

                while (!ThreadStatus.IsExit())
                {
                    Thread.Sleep(10);

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
                            if (Axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_Z].Status.InMotion) continue;
                            if (!Global.SeqWorkDoneBufferOutRail.ThreadStatusSeq.IsExit()) continue;                            
                            SetStep(STEP_SEQ.WAIT_UP);
                            break;
                        case STEP_SEQ.WAIT_UP:
                            if (!DIO.DI_SYSTEM_IF_UP_STEARM_REQUEST.IsOn && !IGlobal.Instance.iData.SEQ_DATA.UP_REQ) continue;

                            DIO.On(DIO.DO_SYSTEM_IF_UP_STEARM_RESPONSE);

                            IGlobal.Instance.iData.SEQ_DATA.UP_REQ = false;
                            SetStep(STEP_SEQ.WAIT_UP_RESP);
                            break;
                        case STEP_SEQ.WAIT_UP_RESP:           

                            SetStep(STEP_SEQ.INPUT_CHECK_SENSOR);
                            break;
                        case STEP_SEQ.INPUT_CHECK_SENSOR:
                            if (!DIO.DI_WORK_DONE_INPUT_RAIL_INPUT_DETECT_BUFFER.IsOn) continue;

                            SetStep(STEP_SEQ.WORK_FEEDER_MOVING_CYL_FW);
                            break;
                        case STEP_SEQ.WORK_FEEDER_MOVING_CYL_FW:
                            if (!DIO.DI_WORK_DONE_INPUT_RAIL_FEEDER_MOVING_CYL_FW_BUFFER.IsOn)
                            {
                                DIO.On(DIO.DO_WORK_DONE_INPUT_RAIL_BUFFER_FEEDER_MOVING_CYL_FW);
                                DIO.Off(DIO.DO_WORK_DONE_INPUT_RAIL_BUFFER_FEEDER_MOVING_CYL_BW);
                                continue;
                            }

                            SetStep(STEP_SEQ.WAIT_RELAY_LIFT_CYL_CHECK);
                            break;
                        case STEP_SEQ.WAIT_RELAY_LIFT_CYL_CHECK:
                            if (DIO.DI_WORK_DONE_INPUT_RAIL_INPUT_DETECT.IsOn
                                || DIO.DI_WORK_DONE_INPUT_RAIL_RELAY_LIFT_POS_DETECT.IsOn)
                            {

                            }
                            else
                            {
                                if (DIO.DI_WORK_DONE_INPUT_RAIL_RELAY_LIFT_CYL_UP.IsOn
                                    && !DIO.DI_WORK_DONE_INPUT_RAIL_INPUT_DETECT.IsOn
                                    && !DIO.DI_WORK_DONE_INPUT_RAIL_RELAY_LIFT_POS_DETECT.IsOn)
                                {
                                    DIO.Off(DIO.DO_WORK_DONE_INPUT_RAIL_RELAY_LIFT_CYl_UP);
                                    DIO.On(DIO.DO_WORK_DONE_INPUT_RAIL_RELAY_LIFT_CYl_DOWN);
                                    continue;
                                }
                                Thread.Sleep(500);

                                if (DIO.DI_WORK_DONE_INPUT_RAIL_METAL_TRAY_LOCK_CYL_FW.IsOn)
                                {
                                    DIO.Off(DIO.DO_WORK_DONE_INPUT_RAIL_METAL_TRAY_LOCK_CYL_FW);
                                    DIO.On(DIO.DO_WORK_DONE_INPUT_RAIL_METAL_TRAY_LOCK_CYL_BW);
                                    continue;
                                }
                            }

                            SetStep(STEP_SEQ.BUFFER_F_START);
                            break;
                        case STEP_SEQ.BUFFER_F_START:
                            axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_F].JogStart(-50);
                            
                            Thread.Sleep(300);
                            DIO.On(DIO.DO_WORK_DONE_INPUT_RAIL_FEEDER_ON_OFF);

                            if(!DIO.DI_WORK_DONE_INPUT_RAIL_INPUT_DETECT.IsOn
                                && !DIO.DI_WORK_DONE_INPUT_RAIL_RELAY_LIFT_POS_DETECT.IsOn
                                && !DIO.DI_WORK_DONE_INPUT_RAIL_RELAY_LIFT_CYL_UP.IsOn
                                && DIO.DI_WORK_DONE_INPUT_RAIL_RELAY_LIFT_CYL_DOWN.IsOn)
                            {
                                SetStep(STEP_SEQ.BYPASS);
                            }
                            else
                            {
                                SetStep(STEP_SEQ.OUTPUT_CHECK_SENSOR);
                            }

                            break;
                        case STEP_SEQ.OUTPUT_CHECK_SENSOR:
                            if (!DIO.DI_WORK_DONE_INPUT_RAIL_OUTPUT_DETECT_BUFFER.IsOn) continue;

                            axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_F].EStop();
                            DIO.Off(DIO.DO_WORK_DONE_INPUT_RAIL_FEEDER_ON_OFF);

                            axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_F].JogStart(5);

                            SetStep(STEP_SEQ.POS_DETECT);
                            break;
                        case STEP_SEQ.POS_DETECT:
                            if (DIO.DI_WORK_DONE_INPUT_RAIL_OUTPUT_DETECT_BUFFER.IsOn) continue;

                            DIO.Off(DIO.DO_SYSTEM_IF_UP_STEARM_RESPONSE);

                            axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_F].EStop();
                            Global.iData.SEQ_DATA.SetIndex_WDB_EXISTS();

                            double dTargetIndex = manager.POS_WORK_DONE_BUFFER_Z.POSITION - (Global.iData.SEQ_DATA.CURRENT_WORK_DONE_BUFFER_INDEX * PITCH_Z);

                            DIO.On(DIO.DO_WORK_DONE_INPUT_RAIL_BUFFER_FEEDER_MOVING_CYL_BW);
                            DIO.Off(DIO.DO_WORK_DONE_INPUT_RAIL_BUFFER_FEEDER_MOVING_CYL_FW);

                            Thread.Sleep(1000);

                            if (!Axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_Z].InpositionEx(dTargetIndex))
                            {
                                Axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_Z].Move(dTargetIndex, 50);
                                Axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_Z].WaitEx(dTargetIndex);
                            }

                            DIO.Off(DIO.DO_WORK_DONE_INPUT_RAIL_BUFFER_FEEDER_MOVING_CYL_BW);
                            DIO.On(DIO.DO_WORK_DONE_INPUT_RAIL_BUFFER_FEEDER_MOVING_CYL_FW);

                            SetStep(STEP_SEQ.IDLE);
                            ThreadStatus.End();
                            return;
                        case STEP_SEQ.BYPASS:
                            if (!DIO.DI_WORK_DONE_INPUT_RAIL_RELAY_LIFT_POS_DETECT.IsOn) continue;
                            Thread.Sleep(1000);

                            axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_F].EStop();
                            DIO.Off(DIO.DO_WORK_DONE_INPUT_RAIL_FEEDER_ON_OFF);

                            Global.iData.SEQ_DATA.WAIT_WORK_DONE_BUFFER = true;
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
