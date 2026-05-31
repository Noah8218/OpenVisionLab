using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Drawing;

namespace IntelligentFactory
{
    public class CSeqMotion
    {
        private IGlobal Global = IGlobal.Instance;

        private Stopwatch m_swStepTime = new Stopwatch();
        private Stopwatch m_swInspTime = new Stopwatch();

        public Stopwatch InspTime
        {
            get => m_swInspTime;
        }

        private CDIO_AJIN DIO = null;

        private CAXIS_AJIN AxisX = null;
        private CAXIS_AJIN AxisY = null;
        private CAXIS_AJIN AxisZ = null;
        private CAXIS_AJIN AxisR = null;

        private CCameraHikVision CAMERA = null;

        public ManualResetEvent PauseWait = new ManualResetEvent(false);
        public ManualResetEvent AlarmWait = new ManualResetEvent(false);
        public ManualResetEvent InspWait = new ManualResetEvent(false);
        public ManualResetEvent ResetWait = new ManualResetEvent(false);

        public EventHandler<EventArgs> EventResetSeqEnd;

        public bool CanMoveToStation { get; set; } = false;
        public bool CanMoveToUnloading { get; set; } = false;

        public enum SEQ_MODE : int
        {
            NORMAL = 0,
            LOADING = 10,
            UNLOADING = 20,
            INSPECTION = 30,
            BYPASS = 40,
        }
        public SEQ_MODE SeqMode = SEQ_MODE.NORMAL;

        public enum SEQ_MAIN : int
        {
            IDLE = 0,
            INIT_RAIL = 10,
            INIT_RAIL_INPOS = 20,
            INIT_Z_SAFETY = 30,
            INIT_Z_SAFETY_INPOS = 40,
            INIT_XY_READY = 50,
            INIT_XY_READY_INPOS = 60,
            INTERLOCK_STRIP_EXIST = 70,
            INIT_Z_LOADING_READY = 80,
            INIT_Z_LOADING_READY_INPOS = 90,
            LOAD_WAIT_STRIP = 100,
            LOAD_WAIT_STRIP_CHECK = 110,
            LOAD_STATION_XY = 120,
            LOAD_STATION_XY_INPOS = 130,
            LOAD_STATION_R_ALIGN = 140,
            LOAD_STATION_R_ALIGN_INPOS = 150,
            LOAD_STATION_CYL_UP = 160,
            LOAD_STATION_CYL_UP_CHECK = 170,
            LOAD_STATION_VACUUM_ON = 180,
            LOAD_STATION_VACUUM_ON_CHECK = 190,
            LOAD_STATION_CLAMP_OPEN = 200,
            LOAD_STATION_CLAMP_OPEN_CHECK = 210,
            LOAD_STATION_XY_BACK = 220,
            LOAD_STATION_XY_BACK_INPOS = 230,
            VISION_Z_READY = 240,
            VISION_Z_READY_INPOS = 250,
            VISION_INIT = 260,
            VISION_INIT_CHECK = 270,
            VISION_INSP_START = 280,
            VISION_INSP_WAIT = 290,
            UNLOAD_Z_READY = 300,
            UNLOAD_Z_READY_INPOS = 310,
            UNLOAD_R_READY = 320,
            UNLOAD_R_READY_INPOS = 330,
            UNLOAD_XY_READY = 340,
            UNLOAD_XY_READY_INPOS = 350,
            UNLOAD_STATION_VACUUM_OFF = 360,
            UNLOAD_STATION_VACUUM_OFF_CHECK = 370,
            UNLOAD_STATION_CYL_DOWN = 380,
            UNLOAD_STATION_CYL_DOWN_CHECK = 390,
            UNLOAD_MAGZINE_READY = 391,
            UNLOAD_MAGZINE_READY_CHECK = 392,
            UNLOAD_Z_READY_PUSH = 400,
            UNLOAD_Z_READY_PUSH_INPOS = 410,
            UNLOAD_X_PUSH = 420,
            UNLOAD_X_PUSH_INPOS = 430,
            UNLOAD_Z_SAFETY = 440,
            UNLOAD_Z_SAFETY_INPOS = 450,
            UNLOAD_XY_LOAD_READY = 460,
            UNLOAD_XY_LOAD_READY_INPOS = 470,
            UNLOAD_PUSH_FWD = 480,
            UNLOAD_PUSH_FWD_CHECK = 490,
            UNLOAD_PUSH_BKD = 500,
            UNLOAD_PUSH_BKD_CHECK = 510
        }

        public SEQ_MAIN m_StepMain = SEQ_MAIN.IDLE;

        public enum SEQ_RESET : int
        {
            IDLE = 0,
            RAIL_FREE = 10,
            RAIL_FREE_INPOS = 11,
            CYL_FREE = 20,
            CYL_FREE_CHECK = 21,
            Z_FREE = 30,
            Z_FREE_INPOS = 31,
            COMPLETE = 40
        }

        public SEQ_RESET m_StepReset = SEQ_RESET.IDLE;

        public long VACUUM_START_TIME = 0;

        public CSeqMotion()
        {
            DIO = IGlobal.Instance.iDevice.DIO_BD;

            //AxisX = IGlobal.Instance.iDevice.MOTION_AJIN.AxisX;
            //AxisY = IGlobal.Instance.iDevice.MOTION_AJIN.AxisY;
            //AxisZ = IGlobal.Instance.iDevice.MOTION_AJIN.AxisZ;
            //AxisR = IGlobal.Instance.iDevice.MOTION_AJIN.AxisR;

            CAMERA = IGlobal.Instance.iDevice.CAMERA_LD;
        }

        public void SetStep(SEQ_MAIN step)
        {
            SEQ_MAIN stepPrev = m_StepMain;
            m_StepMain = step;

            if (m_StepMain != SEQ_MAIN.IDLE) m_swStepTime.Restart();

            CLogger.Add(LOG.SEQ, "Sequence [MAIN] {0} => {1}", stepPrev.ToString(), m_StepMain.ToString());
        }

        public void SetStepReset(SEQ_RESET step)
        {
            SEQ_RESET stepPrev = m_StepReset;
            m_StepReset = step;

            CLogger.Add(LOG.SEQ, "Sequence [RESET] {0} => {1}", stepPrev.ToString(), m_StepReset.ToString());
        }

        public void On(CSignal signal)
        {
            IGlobal.Instance.iDevice.DIO_BD.On(signal);
            CLogger.Add(LOG.IO, $"{signal.Name} ON");
        }

        public void Off(CSignal signal)
        {
            IGlobal.Instance.iDevice.DIO_BD.Off(signal);
            CLogger.Add(LOG.IO, $"{signal.Name} OFF");
        }


        public void ResetAlarm()
        {
            m_swStepTime.Restart();
            AlarmWait.Set();
        }

        #region Thread
        public void Run(SEQ_MODE mode)
        {
            SeqMode = mode;
            StartThreadSeqMain();
        }

        public CThreadStatus ThreadStatusSeqMain { get; set; } = new CThreadStatus();

        public void ResetThread()
        {
            ThreadStatusSeqMain.End();
            SetStep(SEQ_MAIN.IDLE);
        }

        public void StartThreadSeqMain()
        {
            bool bIsHomeComplete = AxisX.HomeComplete
                && AxisY.HomeComplete
                && AxisZ.HomeComplete
                && AxisR.HomeComplete;            

            if (!AxisX.Status.ServoOn ||
                !AxisY.Status.ServoOn ||
                !AxisZ.Status.ServoOn ||
                !AxisR.Status.ServoOn)
            {
                CUtil.ShowMessageBox("Servo OFF", "Please Check Servo");
                IGlobal.Instance.System.Mode = ISystem.MODE.ALARM;
                return;
            }

            if (!bIsHomeComplete)
            {
                IGlobal.Instance.System.Mode = ISystem.MODE.ALARM;
                CUtil.ShowMessageBox("ERROR", "Should be Home");
                return;
            }
            else
            {
                if (ThreadStatusSeqMain.IsExit())
                {
                    ThreadStatusSeqMain.End();
                    m_StepMain = SEQ_MAIN.IDLE;
                    Thread t = new Thread(new ParameterizedThreadStart(ThreadSeqMain));
                    t.Start(ThreadStatusSeqMain);
                }
                else
                {

                }
            }
        }

        public void StopThreadSeqMain()
        {
            if (!ThreadStatusSeqMain.IsExit())
            {
                ThreadStatusSeqMain.Stop(100);
            }

            ThreadStatusSeqMain.End();
        }

        private void ThreadSeqMain(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("SEQUENCE MAIN");

            try
            {
                PointF[,] PositionMap = null;

                PauseWait.Set();
                AlarmWait.Set();
                ResetWait.Set();

                SeqMode = SEQ_MODE.BYPASS;

                while (!ThreadStatus.IsExit())
                {
                    Thread.Sleep(1);

                    PauseWait.WaitOne();
                    AlarmWait.WaitOne();
                    ResetWait.WaitOne();

                    if( m_StepMain  != SEQ_MAIN.IDLE)
                    {
                        if ( m_swStepTime.ElapsedMilliseconds > IGlobal.Instance.iData.PropertyTimeOut.MAIN_TIME_OUT )
                        {
                            CAlarm.Add( new CNodeAlarm( ( ( int ) m_StepMain ).ToString( ), "MAIN STEP TIME-OUT" ) );
                            AlarmWait.Reset( );
                            continue;
                        }
                        
                    }

                    
                    //if (Global.System.Mode != ISystem.MODE.AUTO)
                    //{
                    //    PauseWait.Reset();
                    //    continue;
                    //}

                    switch (m_StepMain)
                    {
                        case SEQ_MAIN.IDLE:
                            {
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ALARM, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CAlarm.Add(new CNodeAlarm("999", "HANDLER SEQ STOPPED"));
            }
        }

        public CThreadStatus ThreadStatusSeqReset { get; set; } = new CThreadStatus();
        public void StartThreadSeqReset()
        {
            if (!AxisX.Status.ServoOn ||
                !AxisY.Status.ServoOn ||
                !AxisZ.Status.ServoOn ||
                !AxisR.Status.ServoOn)
            {
                CUtil.ShowMessageBox("Servo OFF", "Please Check Servo");
                IGlobal.Instance.System.Mode = ISystem.MODE.ALARM;
                return;
            }

            if (ThreadStatusSeqReset.IsExit())
            {
                ThreadStatusSeqReset.End();

                ResetWait.Reset();

                m_StepReset = SEQ_RESET.IDLE;
                Thread t = new Thread(new ParameterizedThreadStart(ThreadSeqReset));
                t.Start(ThreadStatusSeqReset);
            }
        }

        public void StopThreadSeqReset()
        {
            if (!ThreadStatusSeqReset.IsExit())
            {
                ThreadStatusSeqReset.Stop(100);
            }

            ThreadStatusSeqReset.End();
        }
        private void ThreadSeqReset(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("SEQUENCE RESET");

            try
            {
                int nTimeOutCheck = Environment.TickCount;

                PauseWait.Set();
                AlarmWait.Set();

                while (!ThreadStatus.IsExit())
                {
                    Thread.Sleep(1);

                    PauseWait.Set();
                    AlarmWait.Set();

                    switch (m_StepReset)
                    {
                        case SEQ_RESET.IDLE:
                            Global.System.PROCDURE = "RESET THE SEQUENCE";

#if !DEBUG
                            Global.iDevice.TOWER_LAMP.Set(false, false, true, true); 
#endif
                            SetStepReset(SEQ_RESET.COMPLETE);
                            break;
                        case SEQ_RESET.COMPLETE:

                            ResetWait.Set();

                            if (EventResetSeqEnd != null)
                            {
                                EventResetSeqEnd(this, new EventArgs());
                            }

                            SetStepReset(SEQ_RESET.IDLE);

                            ThreadStatus.End();
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ALARM, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CAlarm.Add(new CNodeAlarm("999", "HANDLER RESET SEQ STOPPED"));
            }
        }

        #endregion
    }
}
