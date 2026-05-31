using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using Lib.Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OpenVisionLab
{
    public partial class FormLog : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;

        public bool ChangeSize { get; set; } = false;

        public FormLog()
        {           
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }
     
        // If the content won't display nicely, hide it
        private void ResizeEvent(object sender, EventArgs e)
        {
            this.Visible = this.Width > this.MinimumSize.Width && this.Height > this.MinimumSize.Height;
        }
        private Log4netView logview = new Log4netView();
        private void Form_Load(object sender, EventArgs e)
        {            
            logview.Dock = DockStyle.Fill;
            pnLog.Controls.Add(logview);

            foreach (var item in Enum.GetValues(typeof(CLOG.LOG)))
            {
                cbLogItems.Items.Add(item);
            }
            cbLogItems.SelectedIndex = 0;
        }

        private void FormLayerDisplay_VisibleChanged(object sender, EventArgs e)
        {
            if (!ChangeSize)
            {
                if (DockHandler.FloatPane == null) { return; }
                DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(DockHandler.FloatPane.FloatWindow.Bounds.X, DockHandler.FloatPane.FloatWindow.Bounds.Y, 800, 400);
                this.Refresh();
                ChangeSize = true;
            }
        }

        private void cbLogItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLogItems.SelectedItem.ToString() == "") { return; }
            var type = (CLOG.LOG)Enum.Parse(typeof(CLOG.LOG), cbLogItems.SelectedItem.ToString());

            switch(type)
            {
                case CLOG.LOG.NORMAL:
                    logview.ShowAllLog = true;
                    logview.ShowLogType = type;
                    logview.AddLogData(logview.logDataAll);
                    break;
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
                    logview.ShowAllLog = false;
                    logview.ShowLogType = type;
                    logview.AddLogData(type);
                    break;
            }
        }
    }
}
