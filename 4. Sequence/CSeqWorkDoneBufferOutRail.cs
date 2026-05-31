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
    public class CSeqWorkDoneBufferOutRail //버퍼에서 배출
    {
        private IGlobal Global = IGlobal.Instance;

        private Dictionary<int, CAXIS_AJIN> Axises = null;
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
            CHECK_QUEUE,
            MOVE_QUEUE_INDEX,
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

            ThreadStatus.Start("WORK DONE BUFFER OUT RAIL");
            CLogger.Add(LOG.NORMAL, $"THREAD START ==> {MethodBase.GetCurrentMethod().Name}");

            try
            {
                CMotionManager manager = Global.iDevice.MOTION_AJIN;
                Dictionary<int, CAXIS_AJIN> axises = Axises;
                CDIO_AJIN DIO = Global.iDevice.DIO_BD;

                int nIndex = -1;

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
                            if (!Global.SeqStreamUp.ThreadStatusSeq.IsExit())
                            {
                                ThreadStatus.End();
                                return;
                            }

                            if (!DIO.DI_WORK_DONE_INPUT_RAIL_INPUT_DETECT.IsOn
                                && !DIO.DI_WORK_DONE_INPUT_RAIL_RELAY_LIFT_POS_DETECT.IsOn
                                && !DIO.DI_WORK_DONE_INPUT_RAIL_RELAY_LIFT_CYL_UP.IsOn
                                && DIO.DI_WORK_DONE_INPUT_RAIL_RELAY_LIFT_CYL_DOWN.IsOn)
                            {
                            }
                            else
                            {
                                continue;
                            }

                            Thread.Sleep(500);
                            SetStep(STEP_SEQ.CHECK_QUEUE);
                            break;
                        case STEP_SEQ.CHECK_QUEUE:
                            if (Global.iData.SEQ_DATA.WorkDoneQueue.Count == 0)
                            {
                                ThreadStatus.End();
                                return;
                            }
                            else
                            {
                                Global.iData.SEQ_DATA.WorkDoneQueue.TryDequeue(out nIndex);                                

                                if (nIndex < 0)
                                {
                                    CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_WORK_DONE_BUFFER, "Please Check the Work Done Buffer"));
                                    //CAlarm.Add(new CNodeAlarm("WDB CHECK", "WDB CHECK"));
                                    return;
                                }
                            }

                            SetStep(STEP_SEQ.MOVE_QUEUE_INDEX);
                            break;
                        case STEP_SEQ.MOVE_QUEUE_INDEX:
                            if (Axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_Z].Status.InMotion) continue;
                         
                            double dTargetIndex = manager.POS_WORK_DONE_BUFFER_Z.POSITION - (nIndex * PITCH_Z);

                            if (!Axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_Z].InpositionEx(dTargetIndex))
                            {
							//InMotion 확인 추가
                                Axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_Z].Move(dTargetIndex, 50);
                                continue;
                            }

                            SetStep(STEP_SEQ.BUFFER_F_START);
                            break;
                        case STEP_SEQ.BUFFER_F_START:
                            if (DIO.DI_WORK_DONE_INPUT_RAIL_FEEDER_MOVING_CYL_BW_BUFFER.IsOn)
                            {
                                CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_WORK_DONE_BUFFER, "Please Check the Work Done Buffer Moving Cyl"));
                                continue;
                            }
                            axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_F].JogStart(-50);

                            Thread.Sleep(300);
                            DIO.On(DIO.DO_WORK_DONE_INPUT_RAIL_FEEDER_ON_OFF);

                            SetStep(STEP_SEQ.INPUT_CHECK_SENSOR);
                            break;

                        case STEP_SEQ.INPUT_CHECK_SENSOR:
                            if (!DIO.DI_WORK_DONE_INPUT_RAIL_RELAY_LIFT_POS_DETECT.IsOn) continue;
                            Thread.Sleep(1000);

                            axises[(int)DEFINE.MOTOR.WORK_DONE_BUFFER_F].EStop();
                            DIO.Off(DIO.DO_WORK_DONE_INPUT_RAIL_FEEDER_ON_OFF);

                            Global.iData.SEQ_DATA.WORK_DONE_BUFFER[nIndex] = false;

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
