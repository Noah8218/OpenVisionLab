using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using Lib.Common;
using log4net.Repository.Hierarchy;
using Matrox.MatroxImagingLibrary;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OpenVisionLab
{
    public partial class FormLabel : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;

        public bool ChangeSize { get; set; } = false;

        public FormLabel()
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
            Global.Device.DIO_BD.DO_02_Label1_Start.EventUpdateSignal += OnUpdateLabel;
            Global.Device.DIO_BD.DO_04_Label2_Start.EventUpdateSignal += OnUpdateLabel;
            Global.Device.DIO_BD.DO_06_Label3_Start.EventUpdateSignal += OnUpdateLabel;
            Global.Device.DIO_BD.DO_08_Label4_Start.EventUpdateSignal += OnUpdateLabel;

            Global.Device.DIO_BD.DI_03_Label1_Error.EventUpdateSignal += OnUpdateLabel;
            Global.Device.DIO_BD.DI_06_Label2_Error.EventUpdateSignal += OnUpdateLabel;
            Global.Device.DIO_BD.DI_09_Label3_Error.EventUpdateSignal += OnUpdateLabel;
            Global.Device.DIO_BD.DI_12_Label4_Error.EventUpdateSignal += OnUpdateLabel;

            Global.Device.DIO_BD.DI_04_Label1_Return.EventUpdateSignal += OnUpdateLabel;
            Global.Device.DIO_BD.DI_07_Label2_Return.EventUpdateSignal += OnUpdateLabel;
            Global.Device.DIO_BD.DI_10_Label3_Return.EventUpdateSignal += OnUpdateLabel;
            Global.Device.DIO_BD.DI_13_Label4_Return.EventUpdateSignal += OnUpdateLabel;
        }

        private async void OnUpdateLabel(object sender, EventArgs e)
        {   
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    OnUpdateLabel(sender, e);
                }));
            }
            else
            {
                CSignal signal = (sender as CSignal);

                if (signal == null) return;
                if (signal.IsDisplay) signal.IsDisplay = false;

                if (signal.Current == CSignal.SIGNAL_ON)
                {
                    switch (signal.Name)
                    {
                        case "DO_02_Label1_Start":
                            CUtil_UI.UpdateButtonOnOff(btnLabelStart1, Global.Device.DIO_BD.DO_02_Label1_Start.IsOn);
                            break;
                        case "DO_04_Label2_Start":
                            CUtil_UI.UpdateButtonOnOff(btnLabelStart2, Global.Device.DIO_BD.DO_04_Label2_Start.IsOn);
                            break;
                        case "DO_06_Label3_Start":
                            CUtil_UI.UpdateButtonOnOff(btnLabelStart3, Global.Device.DIO_BD.DO_06_Label3_Start.IsOn);
                            break;
                        case "DO_08_Label4_Start":
                            CUtil_UI.UpdateButtonOnOff(btnLabelStart4, Global.Device.DIO_BD.DO_08_Label4_Start.IsOn);
                            break;
                        case "DI_04_Label1_Return":
                            CUtil_UI.UpdateButtonOnOff(btnLabelReturn1, Global.Device.DIO_BD.DI_04_Label1_Return.IsOn);
                            break;
                        case "DI_07_Label2_Return":
                            CUtil_UI.UpdateButtonOnOff(btnLabelReturn2, Global.Device.DIO_BD.DI_07_Label2_Return.IsOn);
                            break;
                        case "DI_10_Label3_Return":
                            CUtil_UI.UpdateButtonOnOff(btnLabelReturn3, Global.Device.DIO_BD.DI_10_Label3_Return.IsOn);
                            break;
                        case "DI_13_Label4_Return":
                            CUtil_UI.UpdateButtonOnOff(btnLabelReturn4, Global.Device.DIO_BD.DI_13_Label4_Return.IsOn);
                            break;
                        case "DI_03_Label1_Error":
                            CUtil_UI.UpdateButtonOnOff(btnLabelError1, Global.Device.DIO_BD.DI_03_Label1_Error.IsOn);
                            break;
                        case "DI_06_Label2_Error":
                            CUtil_UI.UpdateButtonOnOff(btnLabelError2, Global.Device.DIO_BD.DI_06_Label2_Error.IsOn);
                            break;
                        case "DI_09_Label3_Error":
                            CUtil_UI.UpdateButtonOnOff(btnLabelError3, Global.Device.DIO_BD.DI_09_Label3_Error.IsOn);
                            break;
                        case "DI_12_Label4_Error":
                            CUtil_UI.UpdateButtonOnOff(btnLabelError4, Global.Device.DIO_BD.DI_12_Label4_Error.IsOn);
                            break;

                    }

                    await Task.Delay(10);

                    // Capture는 Start 신호 + 리턴 신호가 들어 왔을 때 찍어야함
                    string saveImagePath = CCommon.GetPath_Screen() + $"{signal.Address}_{signal.Name}_{DateTime.Now.ToString("yyMMdd_HHmmss")}_.jpeg";
                    CUtil.CaptureScreen(saveImagePath);
                }
                else
                {
                    await Task.Delay(Global.Data.SETTING.Display_Labeler_OffTime);
                    switch (signal.Name)
                    {
                        case "DO_02_Label1_Start":
                            CUtil_UI.UpdateButtonOnOff(btnLabelStart1, Global.Device.DIO_BD.DO_02_Label1_Start.IsOn);
                            break;
                        case "DO_04_Label2_Start":
                            CUtil_UI.UpdateButtonOnOff(btnLabelStart2, Global.Device.DIO_BD.DO_04_Label2_Start.IsOn);
                            break;
                        case "DO_06_Label3_Start":
                            CUtil_UI.UpdateButtonOnOff(btnLabelStart3, Global.Device.DIO_BD.DO_06_Label3_Start.IsOn);
                            break;
                        case "DO_08_Label4_Start":
                            CUtil_UI.UpdateButtonOnOff(btnLabelStart4, Global.Device.DIO_BD.DO_08_Label4_Start.IsOn);
                            break;
                        case "DI_04_Label1_Return":
                            CUtil_UI.UpdateButtonOnOff(btnLabelReturn1, Global.Device.DIO_BD.DI_04_Label1_Return.IsOn);
                            break;
                        case "DI_07_Label2_Return":
                            CUtil_UI.UpdateButtonOnOff(btnLabelReturn2, Global.Device.DIO_BD.DI_07_Label2_Return.IsOn);
                            break;
                        case "DI_10_Label3_Return":
                            CUtil_UI.UpdateButtonOnOff(btnLabelReturn3, Global.Device.DIO_BD.DI_10_Label3_Return.IsOn);
                            break;
                        case "DI_13_Label4_Return":
                            CUtil_UI.UpdateButtonOnOff(btnLabelReturn4, Global.Device.DIO_BD.DI_13_Label4_Return.IsOn);
                            break;
                        case "DI_03_Label1_Error":
                            CUtil_UI.UpdateButtonOnOff(btnLabelError1, Global.Device.DIO_BD.DI_03_Label1_Error.IsOn);
                            break;
                        case "DI_06_Label2_Error":
                            CUtil_UI.UpdateButtonOnOff(btnLabelError2, Global.Device.DIO_BD.DI_06_Label2_Error.IsOn);
                            break;
                        case "DI_09_Label3_Error":
                            CUtil_UI.UpdateButtonOnOff(btnLabelError3, Global.Device.DIO_BD.DI_09_Label3_Error.IsOn);
                            break;
                        case "DI_12_Label4_Error":
                            CUtil_UI.UpdateButtonOnOff(btnLabelError4, Global.Device.DIO_BD.DI_12_Label4_Error.IsOn);
                            break;

                    }
                }
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

        private void timer1_Tick(object sender, EventArgs e)
        {

            CUtil_UI.UpdateLabelOnOff(lbUseLabeler1, Global.Data.SETTING.USE_Labeler_No1);
            CUtil_UI.UpdateLabelOnOff(lbUseLabeler2, Global.Data.SETTING.USE_Labeler_No2);
            CUtil_UI.UpdateLabelOnOff(lbUseLabeler3, Global.Data.SETTING.USE_Labeler_No3);
            CUtil_UI.UpdateLabelOnOff(lbUseLabeler4, Global.Data.SETTING.USE_Labeler_No4);

            lbPearseDis.Text = Global.Data.SETTING.EncoderPermm.ToString();
            lbTopEncoderOffset.Text = Global.Data.SETTING.Upper_Encoder_Offset.ToString();
            lbBottomEncoderOffset.Text = Global.Data.SETTING.Lower_Encoder_Offset.ToString();
            lbLabelOnTime.Text = Global.Data.SETTING.Labeler_OnTime.ToString();
            lbLabelOffTime.Text = Global.Data.SETTING.Labeler_OffTime.ToString();
            lbLabelMinGap.Text = Global.Data.SETTING.Labeler_Minimum_Spacing.ToString();

            CUtil_UI.UpdateButtonOnOff(btnLabelerReady1, Global.Device.DIO_BD.DI_02_Label1_Ready.IsOn);
            CUtil_UI.UpdateButtonOnOff(btnLabelerReady2, Global.Device.DIO_BD.DI_05_Label2_Ready.IsOn);
            CUtil_UI.UpdateButtonOnOff(btnLabelerReady3, Global.Device.DIO_BD.DI_08_Label3_Ready.IsOn);
            CUtil_UI.UpdateButtonOnOff(btnLabelerReady4, Global.Device.DIO_BD.DI_11_Label4_Ready.IsOn);

            CUtil_UI.UpdateButtonOnOff(btnLabelReset1, Global.Device.DIO_BD.DO_03_Label1_Reset.IsOn);
            CUtil_UI.UpdateButtonOnOff(btnLabelReset2, Global.Device.DIO_BD.DO_05_Label2_Reset.IsOn);
            CUtil_UI.UpdateButtonOnOff(btnLabelReset3, Global.Device.DIO_BD.DO_07_Label3_Reset.IsOn);
            CUtil_UI.UpdateButtonOnOff(btnLabelReset4, Global.Device.DIO_BD.DO_09_Label4_Reset.IsOn);
        }

        private void btnLabelOnOff1_Click(object sender, EventArgs e)
        {
            string strIndex = ((RJCodeUI_M1.RJControls.RJButton)sender).Name;

            switch(strIndex)
            {
                case "btnLabelOnOff1":
                    LabelOnOff(Global.Device.DIO_BD.DO_02_Label1_Start);
                    break;
                case "btnLabelOnOff2":
                    LabelOnOff(Global.Device.DIO_BD.DO_04_Label2_Start);
                    break;
                case "btnLabelOnOff3":
                    LabelOnOff(Global.Device.DIO_BD.DO_06_Label3_Start);
                    break;
                case "btnLabelOnOff4":
                    LabelOnOff(Global.Device.DIO_BD.DO_08_Label4_Start);
                    break;
            }            
        }
        
        private void LabelOnOff(CSignal DO)
        {
            Global.Device.DIO_BD.On(DO);
            Thread.Sleep(Global.Data.SETTING.Labeler_OnTime);
            Global.Device.DIO_BD.Off(DO);
            Thread.Sleep(Global.Data.SETTING.Labeler_OffTime);
        }

        private void LabelResetOnOff(CSignal DO)
        {
            Global.Device.DIO_BD.On(DO);
            Thread.Sleep(Global.Data.SETTING.Labeler_RestOnTime);
            Global.Device.DIO_BD.Off(DO);
            Thread.Sleep(Global.Data.SETTING.Labeler_OffTime);
        }

        private void btnLabelResetOnOff1_Click(object sender, EventArgs e)
        {
            string strIndex = ((RJCodeUI_M1.RJControls.RJButton)sender).Name;

            switch (strIndex)
            {
                case "btnLabelResetOnOff1":
                    LabelResetOnOff(Global.Device.DIO_BD.DO_03_Label1_Reset);
                    break;
                case "btnLabelResetOnOff2":
                    LabelResetOnOff(Global.Device.DIO_BD.DO_05_Label2_Reset);
                    break;
                case "btnLabelResetOnOff3":
                    LabelResetOnOff(Global.Device.DIO_BD.DO_07_Label3_Reset);
                    break;
                case "btnLabelResetOnOff4":
                    LabelResetOnOff(Global.Device.DIO_BD.DO_09_Label4_Reset);
                    break;
            }
        }
    }
}
