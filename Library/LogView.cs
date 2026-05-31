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

namespace KtemVisionSystem
{
    public partial class LogView : UserControl
    {
        private const int MAX_LOG_LINES = 5000;
        private const int WM_VSCROLL = 0x115;
        private const int SB_BOTTOM = 7;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private List<LogItem> m_lstLog = new List<LogItem>();

        public LogView()
        {
            InitializeComponent();
            this.richTextBoxExLog.MouseUp += btn_MouseUp;
            ddmLog.OwnerIsMenuButton = true;
        }

        Semaphore m_SemLockLog = new Semaphore(1, 1);

        private void timerDisplayLog_Tick(object sender, EventArgs e)
        {
            //m_SemLockLog.WaitOne();
            //timerDisplayLog.Enabled = false;

            if (richTextBoxExLog.Lines.Length > MAX_LOG_LINES)
            {
                richTextBoxExLog.ReadOnly = false;
                richTextBoxExLog.Select(0, richTextBoxExLog.GetFirstCharIndexFromLine(richTextBoxExLog.Lines.Length - MAX_LOG_LINES));
                richTextBoxExLog.SelectedText = "";
                richTextBoxExLog.ReadOnly = true;

                richTextBoxExLog.Clear();
            }

            foreach (LogItem item in m_lstLog)
            {
                if (item.IsDisplay)
                {
                    this.richTextBoxExLog.SelectionStart = this.richTextBoxExLog.TextLength;
                    this.richTextBoxExLog.ScrollToCaret();
                    this.richTextBoxExLog.SelectionLength = 0;
                    this.richTextBoxExLog.SelectionColor = CLogger.GetColor(item.Type);
                    this.richTextBoxExLog.AppendText(item.StrLog + "\r\n");
                    this.richTextBoxExLog.SelectionColor = this.richTextBoxExLog.ForeColor;
                    item.IsDisplay = false;

                    this.richTextBoxExLog.SelectionStart = this.richTextBoxExLog.TextLength;
                    int nRet = SendMessage(richTextBoxExLog.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, IntPtr.Zero);
                }
            }

            richTextBoxExLog.Invalidate();
            //m_SemLockLog.Release();    
        }

        private void btn_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Open_DropdownMenu(ddmLog, sender);                
            }
            this.richTextBoxExLog.SelectionStart = this.richTextBoxExLog.TextLength;
        }

        private void Open_DropdownMenu(RJDropdownMenu dropdownMenu, object sender)
        {
            Control control = (Control)sender;
            dropdownMenu.ItemClicked += new ToolStripItemClickedEventHandler(LogClicked);
            dropdownMenu.Show(control, control.Width/ 2, control.Height/ 2);
        }

        private void LogClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Show Folder":
                    Process.Start(Application.StartupPath + "\\Log");
                    break;
                default:
                    break;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {            
            CLogger.AddEvent(DisplayLog);   
        }
       
        private void DisplayLog(LogItem item)
        {
            //m_SemLockLog.WaitOne();
            //bool bClear = false;
            //bool bLock = false;
            //try
            //{
            //    Monitor.Enter(m_lstLog, ref bLock);
            //    m_lstLog.Add(item);
            //    while (m_lstLog.Count > MAX_LOG_LINES)
            //    {
            //        bClear = true;
            //        m_lstLog.RemoveAt(0);                    
            //    }
            //}
            //finally
            //{
            //    if (bLock)
            //    {
            //        Monitor.Exit(m_lstLog);
            //        GC.Collect();
            //    }
            //}

            m_lstLog.Add(item);
            while (m_lstLog.Count > MAX_LOG_LINES)
            {
                //bClear = true;
                m_lstLog.RemoveAt(0);
            }

            //this.richTextBoxExLog.SelectionStart = this.richTextBoxExLog.TextLength;
            //this.richTextBoxExLog.ScrollToCaret();
            //if (this.InvokeRequired)
            //{
            //    try
            //    {
            //        this.BeginInvoke(new MethodInvoker(() =>
            //        {                       
            //            this.timerDisplayLog.Enabled = true;
            //        }));
            //    }
            //    catch(Exception ex)
            //    {
            //        Debug.WriteLine(ex.Message);
            //    }
            //}
            //else
            //{
            //    this.timerDisplayLog.Enabled = true;
            //}
            //m_SemLockLog.Release();
        }

        private void ddmLog_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}
