using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using Lib.Common;
using OpenCvSharp;
using RJCodeUI_M1.RJControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OpenVisionLab
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
            string strIndex = ((RJCodeUI_M1.RJControls.RJButton)sender).Text;

            switch (strIndex)
            {
                case "AUTO RUN\r\nSTART":
                case "AUTO START":
                case "START":
                    FormVision_LotOpen formVision_LotOpen = new FormVision_LotOpen();
                    formVision_LotOpen.ShowDialog();

                    btnOperation.IconChar = FontAwesome.Sharp.IconChar.StopCircle;
                    btnOperation.Text = "STOP";
                    CCommon.SetButtonRed(btnOperation);                    
                    btnLive.Enabled = false;
                    btnGrab.Enabled = false;         

                    Global.System.Mode = CSystem.MODE.AUTO;
                    break;
                case "AUTO RUN\r\nSTOP":
                case "AUTO STOP":
                case "STOP":
                    btnOperation.IconChar = FontAwesome.Sharp.IconChar.Play;
                    btnOperation.Text = "START";
                    CCommon.SetButtonBlue(btnOperation);
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
                        Global.Device.CAMERAS[i].Grab();
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
                    CCommon.SetButtonRed((sender as RJButton));
                    break;
                case DEFINE.Live_Stop:
                    for (int i = 0; i < Global.Device.CAMERA_COUNT; i++)
                    {
                        if (!Global.Device.CAMERAS[i].IsOpen) continue;
                        Global.Device.CAMERAS[i].Live(false);
                    }
                    (sender as RJButton).Text = "LIVE";
                    CCommon.SetButtonBlue((sender as RJButton));
                    break;
                case "CROSS":
                    for (int i = 0; i < Global.Device.CAMERA_COUNT; i++)
                    {
                        Global.Device.CAMERAS[i].ImageManager._ViewCross = !Global.Device.CAMERAS[i].ImageManager._ViewCross;
                        if (Global.Device.CAMERAS[i].ImageManager._ViewCross) { CCommon.SetButtonRed((sender as RJButton)); }
                        else { CCommon.SetButtonBlue((sender as RJButton)); }
                    }
                    for (int i = 0; i < Global.Device.CAMERA_COUNT; i++) { Global.Device.CAMERAS[i].ImageManager._Ib.Refresh(); }

                    break;
                case "단일 검사":
                    int count = 0;
                    string[] strExtensions = { "jpg", "png", "jpeg", "bmp" };

                    string strPath1 = Application.StartupPath + "\\Aging\\Cam1\\";
                    string strPath2 = Application.StartupPath + "\\Aging\\Cam2\\";

                    string[] strImages1 = Directory.GetFiles(strPath1, "*.*").Where(f => strExtensions.Contains(f.Split('.').Last().ToLower())).ToArray();
                    string[] strImages2 = Directory.GetFiles(strPath2, "*.*").Where(f => strExtensions.Contains(f.Split('.').Last().ToLower())).ToArray();

                    Random random = new Random();

                    Mat ImageSource = Cv2.ImRead(strImages1[random.Next(0, strImages1.Length - 1)]);

                    if (ImageSource.Channels() != 1)
                    {
                        Cv2.CvtColor(ImageSource, ImageSource, ColorConversionCodes.RGBA2GRAY);
                    }

                    Mat ImageSource2 = Cv2.ImRead(strImages2[random.Next(0, strImages2.Length - 1)]);

                    if (ImageSource2.Channels() != 1)
                    {
                        Cv2.CvtColor(ImageSource2, ImageSource2, ColorConversionCodes.RGBA2GRAY);
                    }

                    if (Global.Device.CAMERA_COUNT == 1)
                    {
                        Global.Data.GrabQueue.Enqueue(new CGrabBuffer(ImageSource, 0, Global.Data.Total_Encoder, true));
                    }
                    else
                    {
                        Global.Data.GrabQueue.Enqueue(new CGrabBuffer(ImageSource, 0, Global.Data.Total_Encoder, true));
                        Global.Data.GrabQueue.Enqueue(new CGrabBuffer(ImageSource2, 1, Global.Data.Total_Encoder, true));
                    }

                    count++;
                    break;
            }
            CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
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

        private void btnOperation_Click(object sender, EventArgs e)
        {

        }

        private void btnGrab_Click(object sender, EventArgs e)
        {

        }

        private void btnCross_Click(object sender, EventArgs e)
        {

        }

        private void btnLive_Click(object sender, EventArgs e)
        {

        }
  
        private void rjButton1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Global.Data.TestEncoder += 100;

            if (Global.Data.TestEncoder % 3000 == 0)
            {
                if (Global.Data.Total_Encoder == 0) { return; }
                this.UIThreadBeginInvoke(() =>
                {
                    if (Global.Data.Total_Encoder == 0) { return; }                    
                    string[] strExtensions = { "jpg", "png", "jpeg", "bmp" };

                    string strPath1 = Application.StartupPath + "\\Aging\\Cam1\\";
                    string strPath2 = Application.StartupPath + "\\Aging\\Cam2\\";

                    string[] strImages1 = Directory.GetFiles(strPath1, "*.*").Where(f => strExtensions.Contains(f.Split('.').Last().ToLower())).ToArray();
                    string[] strImages2 = Directory.GetFiles(strPath2, "*.*").Where(f => strExtensions.Contains(f.Split('.').Last().ToLower())).ToArray();

                    Random random = new Random();

                    Mat ImageSource = Cv2.ImRead(strImages1[random.Next(0, strImages1.Length - 1)]);

                    if (ImageSource.Channels() != 1)
                    {
                        Cv2.CvtColor(ImageSource, ImageSource, ColorConversionCodes.RGBA2GRAY);
                    }

                    Mat ImageSource2 = Cv2.ImRead(strImages2[random.Next(0, strImages2.Length - 1)]);

                    if (ImageSource2.Channels() != 1)
                    {
                        Cv2.CvtColor(ImageSource2, ImageSource2, ColorConversionCodes.RGBA2GRAY);
                    }

                    if (Global.Device.CAMERA_COUNT == 1)
                    {
                        Global.Data.GrabQueue.Enqueue(new CGrabBuffer(ImageSource, 0, Global.Data.Total_Encoder, true));
                    }
                    else
                    {
                        Global.Data.GrabQueue.Enqueue(new CGrabBuffer(ImageSource, 0, Global.Data.Total_Encoder, true));
                        Global.Data.GrabQueue.Enqueue(new CGrabBuffer(ImageSource2, 1, Global.Data.Total_Encoder, true));
                    }

                    //Global.Data.TestEncoder++;
                });
            }
        }
    }
}
