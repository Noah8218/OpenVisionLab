using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MetroFramework.Forms;

namespace KtemVisionSystem
{
    public partial class FormAlarm_Select : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;

        public EventHandler<EventArgs> EventExit;
        public EventHandler<StringEventArgs> EventSkip;
        public EventHandler<StringEventArgs> EventSolved;
        public EventHandler<StringEventArgs> EventReject;
        public EventHandler<StringEventArgs> EventRetry;

        public DEFINE.FORM_RESULT RESULT = DEFINE.FORM_RESULT.IDLE;


        public string m_strCode = "";

        public FormAlarm_Select(string strCode, string strDesc)
        {
            InitializeComponent();
            this.TopMost = true;

            lbAlarmCode.Text = strCode;
            lbAlarmDesc.Text = strDesc;
            lbAlarmTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            m_strCode = strCode;
        }

        private void FormAlarm_Select_Load(object sender, EventArgs e)
        {
            try
            {
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
        public void Set(string strCode, string strDesc)
        {
            
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            try
            {
                RESULT = DEFINE.FORM_RESULT.SKIP;
                if (EventSkip != null) EventSkip(this, new StringEventArgs("SKIP"));

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);

                this.Close();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }    
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            try
            {
                RESULT = DEFINE.FORM_RESULT.REJECT;
                if (EventReject != null) EventReject(this, new StringEventArgs("REJECT"));

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);

                this.Close();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            try
            {
                RESULT = DEFINE.FORM_RESULT.RETRY;
                if (EventRetry != null) EventRetry(this, new StringEventArgs(m_strCode));

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);

                this.Close();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnBuzzerOff_Click(object sender, EventArgs e)
        {
            try
            {

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}
