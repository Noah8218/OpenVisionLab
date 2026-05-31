using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MetroFramework.Forms;

namespace KtemVisionSystem
{
    public partial class FormProgress : MetroForm
    {
        public EventHandler<EventArgs> EventExit;
        public EventHandler<StringEventArgs> EventCancel;
        private string m_strProcedure = "";

        public FormProgress(string strHead, string strDesc)
        {
            InitializeComponent();

            m_strProcedure = strHead;

            lbName.Text = strHead;
            lbDesc.Text = strDesc;
            lbTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void FormProgress_Load(object sender, EventArgs e)
        {
            IntPtr ip = CUtil.CreateRoundRectRgn(0, 0, circularProgressBar5.Width, circularProgressBar5.Height, 150, 150);
            CUtil.SetWindowRgn(circularProgressBar5.Handle, ip, true);

            IntPtr ip2 = CUtil.CreateRoundRectRgn(0, 0, this.Width, this.Height, 150, 150);
            CUtil.SetWindowRgn(this.Handle, ip2, true);
        }

        private System.Diagnostics.Stopwatch m_swTackTime = new System.Diagnostics.Stopwatch();
        public void OnShowProgress(object sender = null, EventArgs e = null)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnShowProgress(sender, e);
                    }));
                }
                catch (Exception ex)
                {
                    //Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            else
            {                
                this.Show();
                this.Refresh();

                m_swTackTime = new System.Diagnostics.Stopwatch();
                m_swTackTime.Start();
            }
        }

        public void OnEndProgress(object sender = null, EventArgs e = null)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnEndProgress(sender, e);
                    }));
                }
                catch (Exception ex)
                {
                    //Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            else
            {
                try
                {
                    this.Hide();
                }
                catch
                {
                    
                }                
            }
        }

        public void OntimerProgress(object sender = null, EventArgs e = null)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OntimerProgress(sender, e);
                    }));
                }
                catch (Exception ex)
                {
                    //Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            else
            {
                try
                {
                    int nMinutie = (int)m_swTackTime.ElapsedMilliseconds / 60000;
                    int nSeconds = (int)(m_swTackTime.ElapsedMilliseconds % 60000) / 1000;
                    lbTackTime.Text = $"{nMinutie}:{nSeconds}";
                }
                catch
                {

                }
            }
        }

        private void timerTackTime_Tick(object sender, EventArgs e)
        {
            OntimerProgress(this, null);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(EventCancel != null)
            {
                EventCancel(this, new StringEventArgs(m_strProcedure));
            }

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
