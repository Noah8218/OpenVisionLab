using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace KtemVisionSystem
{
    public class CMOTION_AJIN_HOME
    {
        CMotionManager MOTION_AJIN = null;

        public CMOTION_AJIN_HOME(CMotionManager cMotionManager)
        {
            MOTION_AJIN = cMotionManager;
        }

        private CThreadStatus m_ThreadStatusAllHome = new CThreadStatus();
        public CThreadStatus ThreadStatusAllHome
        {
            get { return m_ThreadStatusAllHome; }
        }
        //FormMessageBox m_FrmHome = new FormMessageBox();

        public HOME_SEQ m_Step = HOME_SEQ.IDLE;
        public enum HOME_SEQ : int
        {
            IDLE = 0,
            CHECK_INTERLOCK = 1,
            HOME_START_BUFFER_INIT = 10,
            HOME_START_LOT_TRANSFER_Z = 20,
            HOME_START_LOT_TRANSFER_Z_CHECK = 21,
            HOME_START_LOT_TRANSFER_XY = 30,
            HOME_START_LOT_TRANSFER_XY_CHECK = 31,
            HOME_START_UPPER_Z = 40,
            HOME_START_WORK_LOADING_BUUFER_LIFT_Z = 50,
            HOME_START_VISION_X = 60,
            HOME_START_VISION_X_CHECK = 61,
            HOME_START_VISION_Y = 70,
            HOME_START_VISION_Y_CHECK = 71,
            HOME_START_WORK_PICKER_Z = 80,
            HOME_START_WORK_PICKER_Z_CHECK = 81,
            HOME_START_WORK_PICKER_Y = 90,
            HOME_START_WORK_PICKER_Y_CHECK = 91,
            HOME_START_UPPER_Z_CHECK = 100,
            HOME_START_UPPER_Z_AVOID = 110,
            HOME_START_UPPER_Y = 120,
            HOME_START_UPPER_Y_INPOS = 130,
            HOME_START_UPPER_X = 140,
            HOME_START_UPPER_X_INPOS = 150,
            HOME_START_UPPER_R = 160,
            HOME_START_UPPER_R_INPOS = 160,
            HOME_START_TOTAL_CHECK = 1000,
            HOME_COMPLETE = 2000
        }

        //;FormProgress FrmProgress = new FormProgress("HOME", "START THE HOMMING");

        public void StartThreadAllHome()
        {   
            //if (CGlobal.Instance.SeqLoadingBuffer != null) CGlobal.Instance.SeqLoadingBuffer.StopThreadSeq();
            //if (CGlobal.Instance.SeqLoadingVision != null) CGlobal.Instance.SeqLoadingVision.StopThreadSeq();
            //if (CGlobal.Instance.SeqLotTransfer != null) CGlobal.Instance.SeqLotTransfer.StopThreadSeq();
            //if (CGlobal.Instance.SeqScanTray != null) CGlobal.Instance.SeqScanTray.StopThreadSeq();
            //if (CGlobal.Instance.SeqUnloadingVision != null) CGlobal.Instance.SeqUnloadingVision.StopThreadSeq();
            //if (CGlobal.Instance.SeqWorkPicker != null) CGlobal.Instance.SeqWorkPicker.StopThreadSeq();
            //if (CGlobal.Instance.SeqZigElevator != null) CGlobal.Instance.SeqZigElevator.StopThreadSeq();
            //if (CGlobal.Instance.SeqUpper != null) CGlobal.Instance.SeqUpper.StopThreadSeq();

            //FrmProgress = new FormProgress("HOME", "START THE HOMMING");
            //FrmProgress.EventCancel += OnCancelHome;

            //CUtil.TaskStart(new Action(() => FrmProgress.OnShowProgress()));

            m_Step = HOME_SEQ.IDLE;

            Thread.Sleep(1000);

            if (m_ThreadStatusAllHome.IsExit())
            {
                Thread t = new Thread(new ParameterizedThreadStart(ThreadAllHome));
                t.Start(m_ThreadStatusAllHome);
            }
        }

        public void StopThreadAllHome()
        {
            if (!ThreadStatusAllHome.IsExit())
            {
                ThreadStatusAllHome.Stop(100);
            }

            MOTION_AJIN.AxisWork_Loader_LappingR.EStop();
            MOTION_AJIN.AxisWork_MainR.EStop();
            MOTION_AJIN.AxisWork_BackAndForthY.EStop();
            MOTION_AJIN.AxisWork_LappingR.EStop();      
        }

        public void AllStop()
        {
            try
            {
                MOTION_AJIN.AxisWork_Loader_LappingR.EStop();
                MOTION_AJIN.AxisWork_MainR.EStop();
                MOTION_AJIN.AxisWork_BackAndForthY.EStop();
                MOTION_AJIN.AxisWork_LappingR.EStop();          
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void OnCancelHome(object sender = null, StringEventArgs e = null)
        {
            if (e == null) return;

            switch(e.Message)
            {
                case "HOME":
                    StopThreadAllHome();
                    break;
            }
        }

        public void SetStep(HOME_SEQ step)
        {
            HOME_SEQ stepPrev = m_Step;
            m_Step = step;

            CLOG.NORMAL( "Home Sequence {0} => {1}", stepPrev.ToString(), m_Step.ToString());
        }

        //int m_nAliveTime = 0;
        private void ThreadAllHome(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("AllHome Start");
            CLOG.NORMAL( "AllHome Start");

            CGlobal.Inst.System.PROCDURE = "START THE ALL HOME";

            try
            {
                int nStartTime = Environment.TickCount;
                int TIME_OUT = 120000;

                while (!ThreadStatus.IsExit())
                {
                    Thread.Sleep(100);

                    //if (!CGlobal.Inst.System.AlarmWait.WaitOne(60000))
                    //{
                    //    CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_RESET_TIME_OUT, "ALARM_RESET_TIME_OUT"));
                    //    continue;
                    //}

                    switch (m_Step)
                    {
                        case HOME_SEQ.IDLE:
                          
                            SetStep(HOME_SEQ.CHECK_INTERLOCK);
                            break;
                        case HOME_SEQ.CHECK_INTERLOCK:
                            //MOTION_AJIN.AxisUpperX.StartThreadHome();
                            //MOTION_AJIN.AxisUpperY.StartThreadHome();
                            //MOTION_AJIN.AxisUpperR.StartThreadHome();

                            SetStep(HOME_SEQ.HOME_START_BUFFER_INIT);
                            break;                      
                        case HOME_SEQ.HOME_COMPLETE:

                            ThreadStatus.End();
                            break;
                    }
                }

                ThreadStatus.End();
            }
            catch (Exception ex)
            {
                //m_FrmHome.EventPopUpEnd(null, null);
                ThreadStatus.End();
                CLOG.ABNORMAL( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}
