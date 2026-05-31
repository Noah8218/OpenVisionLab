using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RJCodeUI_M1.RJControls;
using log4net.Repository.Hierarchy;
using log4net;
using log4net.Core;
using Lib.Common;
using System.Web.UI.WebControls;

namespace OpenVisionLab
{
    public class LogData
    {
        public Color SelectionColor { get; set; } = new Color();
        public string Text { get; set; } = "";
    }

    public partial class Log4netView : UserControl
    {
        private bool UseAutoScroll = true;

        private const int MAX_LOG_LINES = 5000;
        private const int WM_VSCROLL = 0x115;
        private const int SB_BOTTOM = 7;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);        

        private CustomMemoryAppender appender;

        public CLOG.LOG ShowLogType { get; set; } = CLOG.LOG.NORMAL;

        public bool ShowAllLog = true;

        public Log4netView()
        {
            InitializeComponent();
            this.richTextBoxExLog.MouseUp += btn_MouseUp;
            ddmLog.OwnerIsMenuButton = true;

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            foreach (var item in Enum.GetValues(typeof(CLOG.LOG)))
            {
                LogDatas.Add((CLOG.LOG)item, new List<LogData>());
            }
        }

        public Dictionary<CLOG.LOG, List<LogData>> LogDatas = new Dictionary<CLOG.LOG, List<LogData>>();

        public List<LogData> logDataAll = new List<LogData>();
        //public List<LogData> logDataNormal = new List<LogData>();
        //public List<LogData> logDataAbnormal = new List<LogData>();

        private void timerDisplayLog_Tick(object sender, EventArgs e)
        {
            string text = appender.ReadBuffer();

            if (!string.IsNullOrEmpty(text))
            {
                if (richTextBoxExLog.Lines.Length > MAX_LOG_LINES)
                {
                    richTextBoxExLog.ReadOnly = false;
                    richTextBoxExLog.Select(0, richTextBoxExLog.GetFirstCharIndexFromLine(richTextBoxExLog.Lines.Length - MAX_LOG_LINES));
                    richTextBoxExLog.SelectedText = "";
                    richTextBoxExLog.ReadOnly = true;

                    richTextBoxExLog.Clear();
                    foreach(var log in LogDatas.Values)
                    {
                        log.Clear();
                    }
                }

                string[] Split = text.Split('\n');

                for (int i = 0; i < Split.Length; i++)
                {
                    if (Split[i] == "") { continue; }

                    AddLogData(Split[i]);

                    this.richTextBoxExLog.SelectionColor = CLOG.GetColor(Split[i]);
                    this.richTextBoxExLog.AppendText(Split[i]);
                    this.richTextBoxExLog.SelectionColor = this.richTextBoxExLog.ForeColor;
                    this.richTextBoxExLog.SelectionStart = this.richTextBoxExLog.TextLength;
                    if (UseAutoScroll) { SendMessage(richTextBoxExLog.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, IntPtr.Zero); }                    
                }      
            }

            richTextBoxExLog.Invalidate();
        }

        public static CLOG.LOG GetType(string type)
        {
            if (type.Contains("[NORMAL]")) { return CLOG.LOG.NORMAL; }
            else if (type.Contains("[IO]")) { return CLOG.LOG.IO; }
            else if (type.Contains("[ABNORMAL]")) { return CLOG.LOG.ABNORMAL; }
            else if (type.Contains("[ALARM]")) { return CLOG.LOG.ALARM; }
            else if (type.Contains("[COMM]")) { return CLOG.LOG.COMM; }
            else if (type.Contains("[MOTION]")) { return CLOG.LOG.MOTION; }
            else if (type.Contains("[INSP]")) { return CLOG.LOG.INSP; }
            else if (type.Contains("[INTERLOCK]")) { return CLOG.LOG.INTERLOCK; }
            else if (type.Contains("[SEQ]")) { return CLOG.LOG.SEQ; }
            else if (type.Contains("[DEVICE]")) { return CLOG.LOG.DEVICE; }
            else if (type.Contains("[Thread]")) { return CLOG.LOG.Thread; }

            return CLOG.LOG.NORMAL;
        }

        public void AddLogData(List<LogData> logDatas)
        {
            this.richTextBoxExLog.Clear();
            for (int i = 0; i < logDatas.Count; i++)
            {
                this.richTextBoxExLog.SelectionColor = logDatas[i].SelectionColor;
                this.richTextBoxExLog.AppendText(logDatas[i].Text);
                this.richTextBoxExLog.SelectionColor = this.richTextBoxExLog.ForeColor;
                this.richTextBoxExLog.SelectionStart = this.richTextBoxExLog.TextLength;
                if (UseAutoScroll) { SendMessage(richTextBoxExLog.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, IntPtr.Zero); }
            }
        }

        public void AddLogData(CLOG.LOG type)
        {
            this.richTextBoxExLog.Clear();

            LogDatas.TryGetValue(type, out List<LogData> logDatas);

            for (int i = 0; i < logDatas.Count; i++)
            {
                this.richTextBoxExLog.SelectionColor = logDatas[i].SelectionColor;
                this.richTextBoxExLog.AppendText(logDatas[i].Text);
                this.richTextBoxExLog.SelectionColor = this.richTextBoxExLog.ForeColor;
                this.richTextBoxExLog.SelectionStart = this.richTextBoxExLog.TextLength;
                if (UseAutoScroll) { SendMessage(richTextBoxExLog.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, IntPtr.Zero); }
            }
        }

        public  void AddLogData(string type)
        {
            logDataAll.Add(new LogData()
            {
                SelectionColor = CLOG.GetColor(type),
                Text = type
            });

            switch (GetType(type))
            {
                case CLOG.LOG.NORMAL:
                case CLOG.LOG.CONFIG:
                case CLOG.LOG.ABNORMAL:
                case CLOG.LOG.COMM:
                case CLOG.LOG.IO:
                case CLOG.LOG.INTERLOCK:
                case CLOG.LOG.SEQ:
                case CLOG.LOG.ALARM:
                case CLOG.LOG.DEVICE:
                case CLOG.LOG.INSP:
                case CLOG.LOG.MOTION:
                case CLOG.LOG.LOT:
                case CLOG.LOG.Thread:
                case CLOG.LOG.TEACHING:
                    LogDatas[GetType(type)].Add(new LogData()
                    {
                        SelectionColor = CLOG.GetColor(type),
                        Text = type
                    });
                    break;
            }
        }

        private void btn_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Open_DropdownMenu(ddmLog, sender, e);                
            }
            //this.richTextBoxExLog.SelectionStart = this.richTextBoxExLog.TextLength;
        }

        private void Open_DropdownMenu(RJCodeUI_M1.RJControls.RJDropdownMenu dropdownMenu, object sender, MouseEventArgs e)
        {
            Control control = (Control)sender;
            dropdownMenu.ItemClicked += new ToolStripItemClickedEventHandler(LogClicked);
            dropdownMenu.Show(control, e.X, e.Y);
        }

        private void LogClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Show Folder":
                    Process.Start(Application.StartupPath + "\\Log");
                    break;
                case "Auto Scroll":
                    UseAutoScroll = !UseAutoScroll;
                    break;
                default:
                    break;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            appender = new CustomMemoryAppender();
            //Get the logger repository hierarchy.  
            var logRepository = (Hierarchy)LogManager.GetRepository();

            //and add the appender to the root level  
            //of the logging hierarchy  
            logRepository.Root.AddAppender(appender);

            //configure the logging at the root.  
            logRepository.Root.Level = Level.All;

            //mark repository as configured and  
            //notify that is has changed.  
            logRepository.Configured = true;
            logRepository.RaiseConfigurationChanged(EventArgs.Empty);
        }        

        private void ddmLog_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}
