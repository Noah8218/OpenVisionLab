using Lib.Common;
using MetroFramework.Forms;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public partial class FormInit : MetroForm
    {
        public EventHandler<EventArgs> EventExit;

        public ManualResetEvent IsShow = new ManualResetEvent(false);

        public Action Function { get; set; }

        public bool Close { get; set; } = false;


        public FormInit()
        {
            InitializeComponent();

            this.TopMost = true;
            this.TopLevel = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Shown += new EventHandler(Form_Loaded);
        }

        private void Form_Loaded(object sender, EventArgs e)
        {
            //var thread = new Thread(
            //    () =>
            //    {
            //        Function?.Invoke();
            //        this.BeginInvoke(
            //            (Action)(() =>
            //            {

            //                //this.Close();

            //            }));

            //    });

            //thread.Start();

        }

        private void FormInit_Shown(object sender, EventArgs e)
        {
           
        }

        private void FormInit_Load(object sender, EventArgs e)
        {
            //IntPtr ip = CUtil.CreateRoundRectRgn(0, 0, circularProgressBar5.Width, circularProgressBar5.Height, 150, 150);
            //CUtil.SetWindowRgn(circularProgressBar5.Handle, ip, true);

            //IntPtr ip2 = CUtil.CreateRoundRectRgn(0, 0, this.Width, this.Height, 150, 150);
            //CUtil.SetWindowRgn(this.Handle, ip2, true);

            lbVersion.Text = $"VERSION : {CVersion.VERSION} - {CVersion.DATETIME_UPDATED} ({CVersion.MANAGER})";
            CLOG.NORMAL( $"VERSION : {CVersion.VERSION} - {CVersion.DATETIME_UPDATED} ({CVersion.MANAGER})");

            m_swTackTimeMinute = new System.Diagnostics.Stopwatch();
            m_swTackTimeMinute.Start();

            m_swTackTimeSecond = new System.Diagnostics.Stopwatch();
            m_swTackTimeSecond.Start();

            //var calcTask = Task.Run(() =>
            //{
            //    int nMinutie = (int)m_swTackTime.ElapsedMilliseconds / 60000;
            //    int nSeconds = (int)m_swTackTime.ElapsedMilliseconds / 1000;
            //    lbTackTime.Text = $"{nMinutie}:{nSeconds}";
            //});
        }

        public void OnInitStart(object sender = null, EventArgs e = null)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnInitStart(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    //Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                //IsShow.Set();
                //this.Show();                
                //m_swTackTime = new System.Diagnostics.Stopwatch();
                //m_swTackTime.Start();
            }
        }

        public void OnInitEnd(object sender = null, EventArgs e = null)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        OnInitStart(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    //Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                this.Close();
            }
        }

        private System.Diagnostics.Stopwatch m_swTackTimeMinute = new System.Diagnostics.Stopwatch();
        private System.Diagnostics.Stopwatch m_swTackTimeSecond = new System.Diagnostics.Stopwatch();

        public void OnShowProgress(object sender = null, EventArgs e = null)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        OnShowProgress(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    //Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                //var calcTask = Task.Run(() =>
                //{
                //    this.Show();
                //    this.Refresh();
                //    m_swTackTime = new System.Diagnostics.Stopwatch();
                //    m_swTackTime.Start();
                //});                
            }
        }

        public void OnEndProgress(object sender = null, EventArgs e = null)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        OnEndProgress(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    //Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
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

        private void timerTackTime_Tick(object sender, EventArgs e)
        {
            //this.Refresh();
            //this.BeginInvoke(new MethodInvoker(() =>
            //{
            //    int nMinutie = (int)m_swTackTimeMinute.ElapsedMilliseconds / 60000;
            //    int nSeconds = (int)m_swTackTimeSecond.ElapsedMilliseconds / 1000;

            //    if (nSeconds >= 60) { m_swTackTimeSecond.Restart(); }
            //    lbTackTime.Text = $"{nMinutie}:{nSeconds}";
            //}));            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(Close)
            {
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    this.Close();
                }));
            }
        }
    }
}
