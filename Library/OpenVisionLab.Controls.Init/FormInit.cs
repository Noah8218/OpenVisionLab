using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public partial class FormInit : Form
    {
        public string VersionText { get; set; } = "";

        public Action<string> VersionLogAction { get; set; }

        public FormInit()
        {
            InitializeComponent();

            this.TopMost = true;
            this.TopLevel = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Layout += FormInit_Layout;
        }

        private void FormInit_Layout(object sender, EventArgs e)
        {
            NormalizeProgressCircleLayout();
        }

        private void NormalizeProgressCircleLayout()
        {
            if (progressLayout == null || progressCanvas == null || circularProgressBar5 == null || lbTackTime == null)
            {
                return;
            }

            int available = Math.Min(progressLayout.ClientSize.Width, progressLayout.ClientSize.Height);
            int size = Math.Max(240, Math.Min(300, available - 8));
            Size squareSize = new Size(size, size);

            if (progressCanvas.Size != squareSize)
            {
                progressCanvas.Size = squareSize;
            }

            circularProgressBar5.Location = Point.Empty;
            circularProgressBar5.Size = squareSize;
            lbTackTime.SetBounds(0, (int)(size * 0.57), size, 40);
        }

        private void FormInit_Shown(object sender, EventArgs e)
        {
            NormalizeProgressCircleLayout();
        }

        private void FormInit_Load(object sender, EventArgs e)
        {
            //IntPtr ip = AppUtil.CreateRoundRectRgn(0, 0, circularProgressBar5.Width, circularProgressBar5.Height, 150, 150);
            //AppUtil.SetWindowRgn(circularProgressBar5.Handle, ip, true);

            //IntPtr ip2 = AppUtil.CreateRoundRectRgn(0, 0, this.Width, this.Height, 150, 150);
            //AppUtil.SetWindowRgn(this.Handle, ip2, true);

            lbVersion.Text = VersionText;
            VersionLogAction?.Invoke(VersionText);
            NormalizeProgressCircleLayout();

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
                catch
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
            if (IsDisposed || Disposing)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        CloseSafely();
                    }));
                }
                catch
                {
                    //Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                CloseSafely();
            }
        }

        private void CloseSafely()
        {
            if (IsDisposed || Disposing)
            {
                return;
            }

            try
            {
                timerTackTime?.Stop();
                TopMost = false;
                Hide();
                Close();
            }
            catch
            {
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
                catch
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
                catch
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

    }
}
