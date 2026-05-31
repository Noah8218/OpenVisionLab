using KtemVisionSystem._1._Core;
using KtemVisionSystem._2._Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KtemVisionSystem
{
    public partial class FormMainSystem : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;
        public FormMainSystem()
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

        }

        private void timerDateTime_Tick(object sender, EventArgs e)
        {
            try
            {
                double dDrivePercentC = CUtil.DrivePercent("C:\\", out double dCDriveTotalSize, out double dCDriveUsedSize);
                double dDrivePercentD = CUtil.DrivePercent("D:\\", out double dDDriveTotalSize, out double dDDriveUsedSize);

                lbDriveC.Text = $"Drive (C:) : {dDrivePercentC.ToString("F1")}%  ({dCDriveUsedSize.ToString("F1")}/ {dCDriveTotalSize.ToString("F1")} GB)";
                lbDriveD.Text = $"Drive (D:) : {dDrivePercentD.ToString("F1")}%  ({dDDriveUsedSize.ToString("F1")}/ {dDDriveTotalSize.ToString("F1")} GB)";

                pgbDriveC.Value = (int)dDrivePercentC;
                pgbDriveD.Value = (int)dDrivePercentD;
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void timerConnection_Tick(object sender, EventArgs e)
        {
            try
            {
                lbMenu.Text = string.Format("Status : {0}",Global.System.Mode.ToString()); 

                if (Global.Device.CAMERAS.Count > 0)
                {

                    for (int i = 0; i < Global.Device.CAMERAS.Count; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                if (Global.Device.CAMERAS[i].IsOpen)
                                {
                                    btnStatusCAM1.BackColor = System.Drawing.Color.Green;
                                }
                                else
                                {
                                    btnStatusCAM1.BackColor = System.Drawing.Color.Red;
                                }
                                break;
                            case 1:
                                if (Global.Device.CAMERAS[i].IsOpen)
                                {
                                    btnStatusCAM2.BackColor = System.Drawing.Color.Green;
                                }
                                else
                                {
                                    btnStatusCAM2.BackColor = System.Drawing.Color.Red;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private bool ChangeSize = false;

        private void Form_VisibleChanged(object sender, EventArgs e)
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
