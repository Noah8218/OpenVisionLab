using ADOX;
using Lib.Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public class CMOTION_AJIN_ENC600
    {
        private byte m_CurCardNo = 0;
        private byte m_CurAxisNo = 1;
        private byte m_CurrMode = 0;

        private string strErrMsg;

        public bool isOpen = false;

        public double GetEncoderValue = 0;

        private CThreadStatus ThreadStatusdGetEncoder { get; set; } = new CThreadStatus();

        public void StartThreadGetEncoder()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadGetEncoder));
            t.Start(ThreadStatusdGetEncoder);
        }

        public void StopThreadGetEncoder()
        {
            ThreadStatusdGetEncoder.Stop(10);
        }

        private void ThreadGetEncoder(Object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("Get Encoder Thread Start");
            CLOG.NORMAL("Get Encoder Thread Start");            
            try
            {
                while (!ThreadStatus.IsExit())
                {
                    Thread.Sleep(0);

                    GetEncoderValue = GetEncoder();
                }
                //Logger.WriteLog(LOG.Normal, $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                ThreadStatus.End();
            }
        }

        public CMOTION_AJIN_ENC600() { }    
        public bool Init()
        {
            try
            {                
                //Step1 : Open Encoder Card
                Functions.ENC6_INITIAL();

                Byte MaxCardNum = 16;

                //Address can be set as 0~15 by DIP-switch
                uint AddressNo = 0;

                //CardNo set as 0XFF(Invalid Card)
                m_CurCardNo = 0xFF;

                for (Byte CardNo = 0; CardNo < MaxCardNum; CardNo++)
                {
                    if (Functions.ENC6_REGISTRATION(CardNo, AddressNo) == Param.YES)
                    {
                        m_CurCardNo = CardNo;                        
                        break;
                    }
                }

                if (m_CurCardNo == 0xFF)
                {
                    strErrMsg = string.Format("PISO_ENC600/300 doesn't exist!");
                    CLOG.ABNORMAL($"{strErrMsg}");
                    isOpen = false;
                    //MessageBox.Show(strErrMsg);
                }
                else { isOpen = true; }
                
                Start();

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return false;
            }
        }

        public bool Start()
        {
            try
            {
                Functions.ENC6_INIT_CARD(m_CurCardNo, m_CurrMode,
                          m_CurrMode,
                          m_CurrMode,
                          m_CurrMode,
                          m_CurrMode,
                          m_CurrMode);

                StartThreadGetEncoder();
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return false;
            }
        }

        public int GetEncoder()
        {
            try
            {
                if (!isOpen) { return -1; }
                Int32 Counter  = Functions.ENC6_GET_ENCODER(m_CurCardNo, m_CurAxisNo);
                return Counter;
            }
             catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return -1000000;
            }
        }

        public bool Reset()
        {
            try
            {
                if (!isOpen) { return false; }
                Functions.ENC6_RESET_ENCODER(m_CurCardNo, m_CurAxisNo);
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return false;
            }            
        }
    }
}
