using Lib.Common;
using System;
using System.Reflection;
using System.Threading;

namespace OpenVisionLab
{
    public class CSequence
    {
        public enum STEP_ROTATE : int { IDLE = 0, MOVE_ORTHOGONAL_START, START_ROTATE, START_OrthogonalY, COMPLETE }
        public System.Diagnostics.Stopwatch m_swStepTime = new System.Diagnostics.Stopwatch();

        public CSequence()
        {

        }

        public bool Init()
        {
            try
            {
            }
            catch (Exception Desc)
            {
                return false;
            }

            return true;
        }


        #region Thread
        public CThreadStatus ThreadStatusSeq = new CThreadStatus();

        public void StartThreadSeq()
        {
            try
            {
                if (ThreadStatusSeq.IsExit())
                {

                    Thread t = new Thread(new ParameterizedThreadStart(ThreadSeq));
                    t.Start(ThreadStatusSeq);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public void StopThreadSeq()
        {
            SetStep(STEP_ROTATE.IDLE);
            ThreadStatusSeq.Stop(100);
            ThreadStatusSeq.End();                        
        }

        public void SetStep(STEP_ROTATE step)
        {
            STEP_ROTATE stepPrev = (STEP_ROTATE)ThreadStatusSeq.STEP;
            ThreadStatusSeq.STEP = (int)step;

            if ((STEP_ROTATE)ThreadStatusSeq.STEP != STEP_ROTATE.IDLE) m_swStepTime.Restart();
            
            CLOG.SEQ( "SEQ {0} STEP {1} ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, stepPrev.ToString(), step.ToString());
        }

        private void ThreadSeq(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("Start 5G 안테나");
            CLOG.NORMAL( $"THREAD START ==> {MethodBase.GetCurrentMethod().Name}");

            try
            {
                while (!ThreadStatus.IsExit())
                {
                    Thread.Sleep(1);

                    switch ((STEP_ROTATE)ThreadStatus.STEP)
                    {
                        case STEP_ROTATE.IDLE:
                            SetStep(STEP_ROTATE.MOVE_ORTHOGONAL_START);
                            break;                       
                        case STEP_ROTATE.COMPLETE:
                            SetStep(STEP_ROTATE.IDLE);
                            ThreadStatus.End();
                            return;
                    }
                }

                ThreadStatus.End();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
            }
            finally
            {
                ThreadStatus.End();
            }
        }
        #endregion
    }
}
