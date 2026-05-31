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
    public class CSeqZigElevator
    {
        private IGlobal Global = IGlobal.Instance;

        private Dictionary<int, CAXIS_AJIN> Axises = null;

        public int MAX_COUNT = 5;

        public int METAL_TRAY_COUNT = 0;
        public int COVER_TRAY_COUNT = 0;

        public int BUFFER_INDEX = 0;

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
            INIT_INTERLOCK,
            MOVE_INDEX_POS_Z,
            MOVE_INDEX_POS_Z_INPOS,
            LOADING_CYL_FWD,
            LOADING_CYL_FWD_CHECK,
            LOADING_CYL_BKD,
            LOADING_CYL_BKD_CHECK,
            READ_POS_DETECT,
            READ_POS_DETECT_CHECK,
            ID_READ,
            ID_READ_CHECK,
            FEEDING_CONV_OUTPUT,
            READ_POS_DETECT_BACK,
            READ_POS_DETECT_BACK_CHECK,
            UNLOADING_CYL_BKD,
            UNLOADING_CYL_BKD_CHECK,
            UNLOADING_CYL_DOWN,
            UNLOADING_CYL_DOWN_CHECK,
            UNLOADING_CYL_FWD,
            UNLOADING_CYL_FWD_CHECK,
            UNLOADING_CYL_INIT,
            UNLOADING_CYL_INIT_CHECK,

            //JT Tray -> Zig


            COMPLETE            
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

                Global.System.AlarmWait.Set();
                Global.System.PauseWait.Set();

                while (!ThreadStatus.IsExit())
                {
                    Thread.Sleep(10);

                    if(!Global.System.USE_DRY_RUN)
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
                            //if(IGlobal.Instance.iData.SEQ_DATA.LOADING_FEEDING_REQ)
                            {
                                SetStep(STEP_SEQ.MOVE_INDEX_POS_Z);
                            }
                            //else
                            {
                                continue;
                            }
                            break;
                        case STEP_SEQ.MOVE_INDEX_POS_Z:
                            {
                                if(BUFFER_INDEX >= MAX_COUNT)
                                {   
                                    IGlobal.Instance.iData.SEQ_DATA.IS_COMPLETE_ID_MAPPING = true;
                                    ThreadStatus.End();
                                    return;
                                }

                                if (DIO.DI_METAL_TRAY_RAIL_LOADING_PUSHER_FW.IsOn)
                                {
                                    DIO.Off(DIO.DO_METAL_TRAY_RAIL_LOADING_PUSHER_FW);
                                    DIO.On(DIO.DO_METAL_TRAY_RAIL_LOADING_PUSHER_BW);
                                    continue;
                                }
                                if (DIO.DI_TOP_COVER_RAIL_LOADING_PUSHER_FW.IsOn)
                                {
                                    DIO.Off(DIO.DO_TOP_COVER_RAIL_LOADING_PUSHER_FW);
                                    DIO.On(DIO.DO_TOP_COVER_RAIL_LOADING_PUSHER_BW);
                                    continue;
                                }

                                if (DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_FW.IsOn)
                                {
                                    DIO.Off(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_FW);
                                    DIO.On(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_BW);
                                    continue;
                                }
                                if (DIO.DI_TOP_COVER_RAIL_UNLOADING_PUSHER_FW.IsOn)
                                {
                                    DIO.Off(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_FW);
                                    DIO.On(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_BW);
                                    continue;
                                }

                                if (!DIO.DI_METAL_TRAY_RAIL_LOADING_PUSHER_BW.IsOn
                                    || DIO.DI_METAL_TRAY_RAIL_LOADING_PUSHER_FW.IsOn) continue;

                                if (!DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_BW.IsOn
                                    || DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_FW.IsOn) continue;
                                
                                bool bInterlockZMove = false;

                                bInterlockZMove |= DIO.DI_METAL_TRAY_RAIL_Z_MOVE_INTERLOCK_DETECT_LOADING_SIDE.IsOn;
                                bInterlockZMove |= DIO.DI_METAL_TRAY_RAIL_Z_MOVE_INTERLOCK_DETECT_UNLOADING_SIDE.IsOn;
                                bInterlockZMove |= DIO.DI_TOP_COVER_RAIL_Z_MOVE_INTERLOCK_DETECT_LOADING_SIDE.IsOn;
                                bInterlockZMove |= DIO.DI_TOP_COVER_RAIL_Z_MOVE_INTERLOCK_DETECT_UNLOADING_SIDE.IsOn;

                                if (bInterlockZMove)
                                {
                                    Global.System.AddAlarm("INTERLOCK", "CHECK THE ZIG ELEVATOR FOR MOVE Z");
                                    continue;
                                }

                                double dIndexPos = manager.POS_ZIG_ELAVATOR_INIT_Z.POSITION + (BUFFER_INDEX * Global.iData.PropertyBufferPitch.ZIG_ELEVATOR_PITCH); 
                                axises[(int)DEFINE.MOTOR.JIG_ELEVATOR_Z].Move(dIndexPos, manager.POS_ZIG_ELAVATOR_INIT_Z.SPEED_SLOW);

                                Thread.Sleep(1000);
                            }
                            SetStep(STEP_SEQ.MOVE_INDEX_POS_Z_INPOS);
                            break;
                        case STEP_SEQ.MOVE_INDEX_POS_Z_INPOS:
                            {
                                double dIndexPos = manager.POS_ZIG_ELAVATOR_INIT_Z.POSITION + (BUFFER_INDEX * Global.iData.PropertyBufferPitch.ZIG_ELEVATOR_PITCH);
                                if (!axises[(int)DEFINE.MOTOR.JIG_ELEVATOR_Z].InpositionEx(dIndexPos)) continue;
                            }

                            SetStep(STEP_SEQ.LOADING_CYL_FWD);
                            break;
                        case STEP_SEQ.LOADING_CYL_FWD:
                            DIO.On(DIO.DO_METAL_TRAY_RAIL_LOADING_PUSHER_FW);
                            DIO.Off(DIO.DO_METAL_TRAY_RAIL_LOADING_PUSHER_BW);
                            DIO.On(DIO.DO_TOP_COVER_RAIL_LOADING_PUSHER_FW);
                            DIO.Off(DIO.DO_TOP_COVER_RAIL_LOADING_PUSHER_BW);

                            SetStep(STEP_SEQ.LOADING_CYL_FWD_CHECK);
                            break;
                        case STEP_SEQ.LOADING_CYL_FWD_CHECK:
                            if(DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_DOWN.IsOn
                                || DIO.DI_TOP_COVER_RAIL_UNLOADING_PUSHER_DOWN.IsOn)
                            {
                                DIO.On(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_UP);
                                DIO.Off(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_DOWN);

                                DIO.On(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_UP);
                                DIO.Off(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_DOWN);
                            }

                            if (DIO.DI_METAL_TRAY_RAIL_LOADING_PUSHER_BW.IsOn
                                    || !DIO.DI_METAL_TRAY_RAIL_LOADING_PUSHER_FW.IsOn) continue;

                            if (DIO.DI_TOP_COVER_RAIL_LOADING_PUSHER_BW.IsOn
                                    || !DIO.DI_TOP_COVER_RAIL_LOADING_PUSHER_FW.IsOn) continue;

                            SetStep(STEP_SEQ.LOADING_CYL_BKD);
                            break;
                        case STEP_SEQ.LOADING_CYL_BKD:
                            DIO.Off(DIO.DO_METAL_TRAY_RAIL_LOADING_PUSHER_FW);
                            DIO.On(DIO.DO_METAL_TRAY_RAIL_LOADING_PUSHER_BW);

                            DIO.Off(DIO.DO_TOP_COVER_RAIL_LOADING_PUSHER_FW);
                            DIO.On(DIO.DO_TOP_COVER_RAIL_LOADING_PUSHER_BW);

                            SetStep(STEP_SEQ.LOADING_CYL_BKD_CHECK);
                            break;
                        case STEP_SEQ.LOADING_CYL_BKD_CHECK:
                            if (!DIO.DI_METAL_TRAY_RAIL_LOADING_PUSHER_BW.IsOn
                                    || DIO.DI_METAL_TRAY_RAIL_LOADING_PUSHER_FW.IsOn) continue;

                            if (!DIO.DI_TOP_COVER_RAIL_LOADING_PUSHER_BW.IsOn
                                    || DIO.DI_TOP_COVER_RAIL_LOADING_PUSHER_FW.IsOn) continue;

                            SetStep(STEP_SEQ.READ_POS_DETECT);
                            break;
                        case STEP_SEQ.READ_POS_DETECT:
                            bool bNA = false;

                            if (DIO.DI_METAL_TRAY_RAIL_ID_READ_POS_DETECT.IsOn) IGlobal.Instance.iData.SEQ_DATA.BUFFER_METAL_TRAY_ID[BUFFER_INDEX] = "EXSIT";
                            else IGlobal.Instance.iData.SEQ_DATA.BUFFER_METAL_TRAY_ID[BUFFER_INDEX] = "N/A";

                            if (DIO.DI_TOP_COVER_RAIL_ID_READ_POS_DETECT.IsOn) IGlobal.Instance.iData.SEQ_DATA.BUFFER_TOP_COVER_ID[BUFFER_INDEX] = "EXSIT";
                            else IGlobal.Instance.iData.SEQ_DATA.BUFFER_TOP_COVER_ID[BUFFER_INDEX] = "N/A";

                            bNA = !(DIO.DI_METAL_TRAY_RAIL_ID_READ_POS_DETECT.IsOn || DIO.DI_TOP_COVER_RAIL_ID_READ_POS_DETECT.IsOn);

                            if(bNA)
                            {
                                SetStep(STEP_SEQ.MOVE_INDEX_POS_Z);
                            }
                            else
                            {
                                SetStep(STEP_SEQ.READ_POS_DETECT_CHECK);
                            }
                            break;
                        case STEP_SEQ.READ_POS_DETECT_CHECK:

                            SetStep(STEP_SEQ.ID_READ);
                            break;
                        case STEP_SEQ.ID_READ:
                            for (int i = 0; i < 3; i++)
                            {
                                Thread.Sleep(1000);

                                Global.iDevice.BCR.StartReadCoverBcr();
                                Global.iDevice.BCR.StartReadMetalBcr();

                                Thread.Sleep(1000);

                                if(Global.iDevice.BCR.MetalBCR != "\r\n" && Global.iDevice.BCR.CoverBCR != "\r\n")
                                {
                                    break;
                                }
                            }

                            IGlobal.Instance.iData.SEQ_DATA.BUFFER_METAL_TRAY_ID[BUFFER_INDEX] = Global.iDevice.BCR.MetalBCR;
                            IGlobal.Instance.iData.SEQ_DATA.BUFFER_TOP_COVER_ID[BUFFER_INDEX] = Global.iDevice.BCR.CoverBCR;

                            BUFFER_INDEX++;

                            //if (!DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_BW.IsOn && 
                            //    DIO.DI_TOP_COVER_RAIL_ID_READ_POS_DETECT.IsOn) continue;
                            //ID Read Strart
                            SetStep(STEP_SEQ.ID_READ_CHECK);
                            break;
                        case STEP_SEQ.ID_READ_CHECK:
                            Thread.Sleep(500);

                            SetStep(STEP_SEQ.FEEDING_CONV_OUTPUT);
                            break;
                        case STEP_SEQ.FEEDING_CONV_OUTPUT:
                            DIO.On(DIO.DO_METAL_TRAY_RAIL_FEEDDER_ON_OFF_OUTPUT_DIRECTION);
                            DIO.On(DIO.DO_TOP_COVER_RAIL_FEEDDER_ON_OFF_OUTPUT_DIRECTION);

                            if(DIO.DI_METAL_TRAY_RAIL_RELAY_LIFT_POS_DETECT.IsOn ||
                               DIO.DI_TOP_COVER_RAIL_RELAY_LIFT_POS_DETECT.IsOn)
                            {
                                Thread.Sleep(700);

                                DIO.Off(DIO.DO_METAL_TRAY_RAIL_FEEDDER_ON_OFF_OUTPUT_DIRECTION);
                                DIO.Off(DIO.DO_TOP_COVER_RAIL_FEEDDER_ON_OFF_OUTPUT_DIRECTION);

                            }

                            SetStep(STEP_SEQ.READ_POS_DETECT_BACK);
                            break;
                        case STEP_SEQ.READ_POS_DETECT_BACK:
                            if(!DIO.DI_METAL_TRAY_RAIL_ID_READ_POS_DETECT.IsOn 
                                && !DIO.DI_TOP_COVER_RAIL_ID_READ_POS_DETECT.IsOn)
                            {
                                SetStep(STEP_SEQ.READ_POS_DETECT_BACK_CHECK);
                            }
                            else
                            {
                                continue;
                            }
                            break;
                        case STEP_SEQ.READ_POS_DETECT_BACK_CHECK:

                            Thread.Sleep(3000);
                            DIO.Off(DIO.DO_METAL_TRAY_RAIL_FEEDDER_ON_OFF_OUTPUT_DIRECTION);
                            DIO.Off(DIO.DO_TOP_COVER_RAIL_FEEDDER_ON_OFF_OUTPUT_DIRECTION);

                            SetStep(STEP_SEQ.UNLOADING_CYL_BKD);
                            break;
                        case STEP_SEQ.UNLOADING_CYL_BKD:
                            DIO.Off(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_FW);
                            DIO.On(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_BW);
                            DIO.Off(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_FW);
                            DIO.On(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_BW);

                            SetStep(STEP_SEQ.UNLOADING_CYL_BKD_CHECK);
                            break;
                        case STEP_SEQ.UNLOADING_CYL_BKD_CHECK:
                            if (!DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_BW.IsOn
                                   || DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_FW.IsOn) continue;

                            if (!DIO.DI_TOP_COVER_RAIL_UNLOADING_PUSHER_BW.IsOn
                                    || DIO.DI_TOP_COVER_RAIL_UNLOADING_PUSHER_FW.IsOn) continue;

                            SetStep(STEP_SEQ.UNLOADING_CYL_DOWN);
                            break;
                        case STEP_SEQ.UNLOADING_CYL_DOWN:
                            //DIO.On(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_UP);
                            //DIO.Off(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_DOWN);
                            //DIO.On(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_UP);
                            //DIO.Off(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_DOWN);

                            DIO.Off(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_UP);
                            DIO.On(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_DOWN);
                            DIO.Off(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_UP);
                            DIO.On(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_DOWN);

                            SetStep(STEP_SEQ.UNLOADING_CYL_DOWN_CHECK);
                            break;
                        case STEP_SEQ.UNLOADING_CYL_DOWN_CHECK:
                            if (!DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_DOWN.IsOn
                                  || DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_UP.IsOn) continue;

                            if (!DIO.DI_TOP_COVER_RAIL_UNLOADING_PUSHER_DOWN.IsOn
                                  || DIO.DI_TOP_COVER_RAIL_UNLOADING_PUSHER_UP.IsOn) continue;

                            SetStep(STEP_SEQ.UNLOADING_CYL_FWD);
                            break;
                        case STEP_SEQ.UNLOADING_CYL_FWD:
                            DIO.On(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_FW);
                            DIO.Off(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_BW);
                            DIO.On(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_FW);
                            DIO.Off(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_BW);

                            SetStep(STEP_SEQ.UNLOADING_CYL_FWD_CHECK);
                            break;
                        case STEP_SEQ.UNLOADING_CYL_FWD_CHECK:
                            if (DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_BW.IsOn
                                 || !DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_FW.IsOn) continue;

                            if (DIO.DI_TOP_COVER_RAIL_UNLOADING_PUSHER_BW.IsOn
                                    || !DIO.DI_TOP_COVER_RAIL_UNLOADING_PUSHER_FW.IsOn) continue;

                            SetStep(STEP_SEQ.UNLOADING_CYL_INIT);
                            break;
                        case STEP_SEQ.UNLOADING_CYL_INIT:
                            DIO.Off(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_FW);
                            DIO.On(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_BW);
                            DIO.Off(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_FW);
                            DIO.On(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_BW);

                            DIO.On(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_UP);
                            DIO.Off(DIO.DO_METAL_TRAY_RAIL_UNLOADING_PUSHER_DOWN);
                            DIO.On(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_UP);
                            DIO.Off(DIO.DO_TOP_COVER_RAIL_UNLOADING_PUSHER_DOWN);

                            SetStep(STEP_SEQ.UNLOADING_CYL_INIT_CHECK);
                            break;
                        case STEP_SEQ.UNLOADING_CYL_INIT_CHECK:
                            if (!DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_BW.IsOn
                                 || DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_FW.IsOn) continue;

                            if (!DIO.DI_TOP_COVER_RAIL_UNLOADING_PUSHER_BW.IsOn
                                    || DIO.DI_TOP_COVER_RAIL_UNLOADING_PUSHER_FW.IsOn) continue;

                            if (DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_DOWN.IsOn
                                 || !DIO.DI_METAL_TRAY_RAIL_UNLOADING_PUSHER_UP.IsOn) continue;

                            if (DIO.DI_TOP_COVER_RAIL_UNLOADING_PUSHER_DOWN.IsOn
                                  || !DIO.DI_TOP_COVER_RAIL_UNLOADING_PUSHER_UP.IsOn) continue;

                            SetStep(STEP_SEQ.MOVE_INDEX_POS_Z);
                            break;
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
