using KtemVisionSystem._1._Core;
using KtemVisionSystem._2._Common;
using OpenCvSharp;
using RJCodeUI_M1.RJControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KtemVisionSystem
{
    public partial class FormMainButton : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;

        public bool ChangeSize { get; set; } = false;

        public FormMainButton()
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
            btnOperation.Click += OnClickOperation;
            btnGrab.Click += OnClickOperation;
            btnLive.Click += OnClickOperation;
            btnCross.Click += OnClickOperation;
            btnManualTest.Click += OnClickOperation;
        }

        private void OnClickOperation(object sender, EventArgs e)
        {
            try
            {
                string strIndex = ((RJCodeUI_M1.RJControls.RJButton)sender).Text;

                switch (strIndex)
                {
                    case "AUTO RUN\r\nSTART":
                    case "AUTO START":
                    case "START":
                        btnOperation.IconChar = FontAwesome.Sharp.IconChar.StopCircle;
                        btnOperation.Text = "STOP";
                        CUtil.SetButtonRed(btnOperation);
                        //lbModeStatus.Text = "MODE: AUTO";
                        btnLive.Enabled = false;
                        btnGrab.Enabled = false;
                        //pnSideMenu.Enabled = false;
                        Global.System.Mode = CSystem.MODE.AUTO;
                        break;
                    case "AUTO RUN\r\nSTOP":
                    case "AUTO STOP":
                    case "STOP":
                        btnOperation.IconChar = FontAwesome.Sharp.IconChar.Play;
                        btnOperation.Text = "START";
                        CUtil.SetButtonBlue(btnOperation);
                        //lbModeStatus.Text = "MODE: MANUAML";

                        btnGrab.Enabled = true;
                        btnLive.Enabled = true;
                        //pnSideMenu.Enabled = true;
                        Global.System.Mode = CSystem.MODE.READY;
                        break;
                    case DEFINE.Grab:
                        for (int i = 0; i < Global.Device.CAMERA_COUNT; i++)
                        {
                            if (!Global.Device.CAMERAS[i].IsOpen) continue;
                            Global.Device.CAMERAS[i].Grab(false);
                            //Thread.Sleep(100);
                        }
                        break;
                    case DEFINE.Live:
                        for (int i = 0; i < Global.Device.CAMERA_COUNT; i++)
                        {
                            if (!Global.Device.CAMERAS[i].IsOpen) continue;
                            Global.Device.CAMERAS[i].Live(true);
                        }
                        (sender as RJButton).Text = "LIVE STOP";
                        CUtil.SetButtonRed((sender as RJButton));
                        break;
                    case DEFINE.Live_Stop:
                        for (int i = 0; i < Global.Device.CAMERA_COUNT; i++)
                        {
                            if (!Global.Device.CAMERAS[i].IsOpen) continue;
                            Global.Device.CAMERAS[i].Live(false);
                        }
                        (sender as RJButton).Text = "LIVE";
                        CUtil.SetButtonBlue((sender as RJButton));
                        break;
                    case "CROSS":
                        for (int i = 0; i < Global.Device.CAMERA_COUNT; i++)
                        {
                            Global.Device.CAMERAS[i].ViewModeCross = !Global.Device.CAMERAS[i].ViewModeCross;
                            if (Global.Device.CAMERAS[i].ViewModeCross) { CUtil.SetButtonRed((sender as RJButton)); }
                            else { CUtil.SetButtonBlue((sender as RJButton)); }
                        }
                        for (int i = 0; i < Global.Device.CAMERA_COUNT; i++) { Global.Device.CAMERAS[i].ImageManager.ib.Refresh(); }

                        break;
                    case "단일 검사":

                        break;
                }
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
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
