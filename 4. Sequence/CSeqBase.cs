using Lib.Common;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace OpenVisionLab
{
    public abstract class CSeqBase
    {
        public EventHandler<EventArgs> EventSeqComplete;
        public string Name { get; set; } = "";
        public string SeqIndex { get; set; } = "IDLE";        
        public int TimeMS { get; set; } = 1;                        
        private DateTime SeqChangedTime { get; set; } = DateTime.Now;        

        public CThreadStatus ThreadStatus = new CThreadStatus();

        public void StartThread(string strThreadName = "")
        {
            if (strThreadName != "") Name = strThreadName;

            CLOG.SEQ($"Thread Start ==> {Name}");

            if (ThreadStatus.IsExit())
            {
                SeqIndex = "IDLE";
                Thread t = new Thread(new ParameterizedThreadStart(ThreadMain));
                t.Start(ThreadStatus);
            }
        }

        public virtual void StopThread()
        {
            if (!ThreadStatus.IsExit())
            {
                CLOG.SEQ($"Thread Stop ==> {Name}");

                SeqIndex = "IDLE";

                ThreadStatus.Stop(100);
                ThreadStatus.End();
            }
        }

        public void SetStep(string strSeqIndex)
        {
            //CLogger.Add(LOG.SEQ, "SEQ {0} STEP {1} ==> {2}", ThreadName, SeqIndex, strSeqIndex);

            SeqChangedTime = DateTime.Now;

            SeqIndex = strSeqIndex;
        }

        public bool IsTimeOver(int nTimeOver_ms)
        {
            TimeSpan t = DateTime.Now - SeqChangedTime;
            if (t.TotalMilliseconds > nTimeOver_ms) return true;
            else return false;
        }

        private void ThreadMain(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start($"{Name} Start");

            try
            {
                while (!ThreadStatus.IsExit())
                {         
                    try
                    {
                        Run();
                    }
                    catch (Exception Desc)
                    {                        
                        CLOG.ABNORMAL($"[EXCEPTION] Thread : {Name} Seq : {SeqIndex} Detail : {MethodBase.GetCurrentMethod().ReflectedType.Name}/{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                    }

                    Thread.Sleep(TimeMS);
                }

                ThreadStatus.End();
            }
            catch (Exception Desc)
            {                
                CLOG.ABNORMAL($"[EXCEPTION] Thread : {Name} Seq : {SeqIndex} Detail : {MethodBase.GetCurrentMethod().ReflectedType.Name}/{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");                
                ThreadStatus.End();
            }
        }

        public virtual void Run()
        {

        }

        public virtual void IDLE()
        {
            SeqIndex = "IDLE";
        }
    }
}
