using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using OpenVisionLab._3._Device.DB;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OpenVisionLab
{
    public partial class FormSummary : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;

        public bool ChangeSize { get; set; } = false;

        public FormSummary()
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

        private void Form_Load(object sender, EventArgs e)
        {
            List<CDefectSummary> de = new List<CDefectSummary>();            
            dgvSummary.DataSource = new CDefectSummary().GetAttachLabelList(de);

            Global.Thread.CSeqVision.EventSeqComplete += OnInspResult;
        }

        private void OnInspResult(object sender, EventArgs e)
        {
            if (!(e is InspResultArgs args)) { return; }
            this.UIThreadBeginInvoke(() =>
            {
                if (Global.System.Menu == CSystem.MENU.MAIN)
                {
                    List<CDefectSummary> de = new List<CDefectSummary>();
                    foreach (var lot in args.DefectSummaries)
                    {
                        de.Add(lot.Value);                        
                    }
                    dgvSummary.DataSource = new CDefectSummary().GetAttachLabelList(de);
                }
            });
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
    }
}
