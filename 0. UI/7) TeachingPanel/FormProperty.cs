using KtemVisionSystem._1._Core;
using KtemVisionSystem._2._Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace KtemVisionSystem
{
    public partial class FormProperty : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;
        public FormProperty()
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
            rdoLeftEdgePara.Checked = true;
        }

        private void OnPara_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                RJCodeUI_M1.RJControls.RJRadioButton rdoButton = (RJCodeUI_M1.RJControls.RJRadioButton)sender;

                if (rdoButton.Checked)
                {
                    switch (rdoButton.Text)
                    {
                        case "Edge(L)":
                            propertyGrid_Parameter.SelectedObject = CVisionTools.Lines_L[CDisplayManager.CameraIndex];
                            break;
                        case "Edge(R)":
                            propertyGrid_Parameter.SelectedObject = CVisionTools.Lines_R[CDisplayManager.CameraIndex];
                            break;
                        case "Contour":
                            propertyGrid_Parameter.SelectedObject = CVisionTools.Contours[CDisplayManager.CameraIndex];
                            break;
                        case "Blob":
                            propertyGrid_Parameter.SelectedObject = CVisionTools.Blobs[CDisplayManager.CameraIndex];
                            break;
                    }
                }
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
                    for (int i = 0; i < Global.Device.CAMERA_COUNT; i++)
                    {
                        CVisionTools.Blobs[i].SaveConfig(Global.Recipe.Name);
                        CVisionTools.Lines_L[i].SaveConfig(Global.Recipe.Name);
                        CVisionTools.Lines_R[i].SaveConfig(Global.Recipe.Name);
                        CVisionTools.Contours[i].SaveConfig(Global.Recipe.Name);
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
