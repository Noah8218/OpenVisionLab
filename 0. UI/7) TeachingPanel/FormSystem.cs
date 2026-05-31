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
    public partial class FormSystem : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;
        public FormSystem()
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

        private void InitCameraItem()
        {
            for (int i = 0; i < Global.Device.CAMERA_COUNT; i++) { cbCamera.Items.Add("Camera " + (i + 1)); }
        }

        private void InitLayListItem()
        {
            cbLayerList.Items.Clear();
            for (int i = 0; i < CDisplayManager.Displays.Count; i++) { cbLayerList.Items.Add(CDisplayManager.Displays[i].Text); }
            //cbLayerList.SelectedIndex = CDisplayManager.FindIndex(strSelecteItem);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            InitCameraItem();
            InitLayListItem();
            cbCamera.SelectedIndex = 0;
            cbLayerList.SelectedIndex = 0;

            tbExposure.Text = Global.Device.CAMERAS[0].Property.EXPOSURETIME_US.ToString();
            tbGain.Text = Global.Device.CAMERAS[0].Property.GAIN.ToString();

            toolTip1.SetToolTip(btnNewPanel, "Create New Layer");
        }

        private void tbPress_KeyPress(object sender, KeyPressEventArgs e) => CUtil.txtInterval_KeyPress(sender, e);
        private void btnNewPanel_Click(object sender, EventArgs e) => CDisplayManager.CreatePanel();

        private void tbGain_MouseLeave(object sender, EventArgs e)
        {
            try
            {

                int Gain = int.Parse(tbGain.Text);
                int Exposure = int.Parse(tbExposure.Text);
                Global.Device.CAMERAS[cbCamera.SelectedIndex].Property.GAIN = Gain;
                Global.Device.CAMERAS[cbCamera.SelectedIndex].Property.EXPOSURETIME_US = Exposure;
                Global.Device.CAMERAS[cbCamera.SelectedIndex].Property.SaveConfig(Global.Recipe.Name);

                Global.Device.CAMERAS[cbCamera.SelectedIndex].SetGain(Gain);
                Global.Device.CAMERAS[cbCamera.SelectedIndex].SetExposure(Exposure);
            }
            catch (Exception ex) { CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message); }
        }
        private void chkUseLayerImage_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chkUseLayerImage.Check) { CDisplayManager.ImageSrc = CConverter.ToMat((Bitmap)CDisplayManager.Displays[DEFINE.Main].ImageView.ib.Image).Clone(); }
                else
                {
                    CDisplayManager.SelecteItem = cbLayerList.SelectedItem.ToString();
                    CDisplayManager.ImageSrc = CConverter.ToMat((Bitmap)CDisplayManager.Displays[CDisplayManager.FindIndex(CDisplayManager.SelecteItem)].ImageView.ib.Image).Clone();
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void OnClickCameraOperation(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is RJCodeUI_M1.RJControls.RJButton)) return;

                string strIndex = (sender as RJCodeUI_M1.RJControls.RJButton).Text;

                switch (strIndex)
                {
                    case DEFINE.Grab:
                        if (!Global.Device.CAMERAS[cbCamera.SelectedIndex].IsOpen) return;
                        Global.Device.CAMERAS[cbCamera.SelectedIndex].Grab(false);
                        CDisplayManager.Displays[DEFINE.Main].Activate();
                        CDisplayManager.Displays[DEFINE.Main].ImageView.ib.ZoomToFit();
                        break;
                    case DEFINE.Live:
                        if (!Global.Device.CAMERAS[cbCamera.SelectedIndex].IsOpen) return;
                        (sender as RJCodeUI_M1.RJControls.RJButton).Text = "LIVE STOP";
                        Global.Device.CAMERAS[cbCamera.SelectedIndex].Live(true);
                        CDisplayManager.Displays[DEFINE.Main].Activate();
                        CDisplayManager.Displays[DEFINE.Main].ImageView.ib.ZoomToFit();
                        break;
                    case DEFINE.Live_Stop:
                        if (!Global.Device.CAMERAS[cbCamera.SelectedIndex].IsOpen) return;
                        (sender as RJCodeUI_M1.RJControls.RJButton).Text = "LIVE";
                        Global.Device.CAMERAS[cbCamera.SelectedIndex].Live(false);
                        break;
                }

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnSaveVisionParam_Click(object sender, EventArgs e)
        {
            try
            {
                if (CUtil.ShowdialogMessageBox("Save", "현재까지 설정된 값을 저장하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    int nIndex = cbCamera.SelectedIndex;
                    Global.Device.CAMERAS[nIndex].Property.SaveConfig(Global.Recipe.Name);
                    CGlobal.Inst.Data.SPEC.SaveConfig(Global.Recipe.Name);
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void cbCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CDisplayManager.CameraIndex = cbCamera.SelectedIndex;

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void cbLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chkUseLayerImage.Check)
            {
                if (cbLayerList.SelectedItem == null) { return; }
                CDisplayManager.SelecteItem = cbLayerList.SelectedItem.ToString();
                CDisplayManager.ImageSrc = CConverter.ToMat((Bitmap)CDisplayManager.Displays[CDisplayManager.FindIndex(CDisplayManager.SelecteItem)].ImageView.ib.Image).Clone();
                CDisplayManager.Displays[CDisplayManager.FindIndex(CDisplayManager.SelecteItem)].Activate();
            }
        }

        public int PanelCount = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PanelCount != CDisplayManager.Displays.Count)
                {
                    PanelCount = CDisplayManager.Displays.Count;
                    InitLayListItem();
                }

                if (CDisplayManager.Displays[CDisplayManager.FindIndex()].ImageView.ImageChanged)
                {
                    CDisplayManager.ImageSrc = CConverter.ToMat((Bitmap)CDisplayManager.Displays[CDisplayManager.FindIndex()].ImageView.ib.Image);
                    CDisplayManager.Displays[CDisplayManager.FindIndex()].ImageView.ImageChanged = false;
                }

                rjPanel3.Invalidate();
                rjPanel1.Invalidate();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}
