using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Controls.WpfPropertyGrid.KnownTypes;

namespace OpenVisionLab
{
    public partial class FormProperty : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;

        private System.Windows.Forms.Integration.ElementHost host = null;
        private System.Windows.Controls.WpfPropertyGrid.PropertyGrid wpg = null;

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
            rdoContourPara.Checked = true;
            CDisplayManager.EventUpdateParameter += OnUpdateParameter;

            host = new System.Windows.Forms.Integration.ElementHost
            {
                Dock = DockStyle.Fill
            };
            wpg = new System.Windows.Controls.WpfPropertyGrid.PropertyGrid
            {
                Layout = new System.Windows.Controls.WpfPropertyGrid.Design.CategorizedLayout()
            };
            host.Child = wpg;
            pnParameter.Controls.Add(host);

            wpg.PropertyValueChanged += CWpgEvent.Wpg_PropertyValueChanged;
            wpg.SelectedObjectsChanged += CWpgEvent.Wpg_SelectedObjectsChanged;
        }

        private void OnUpdateParameter(object sender, EventArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {
                try
                {
                    var checkedButton = splitContainer1.Panel1.Controls.OfType<RJCodeUI_M1.RJControls.RJRadioButton>()
                                      .FirstOrDefault(r => r.Checked);
                    if (checkedButton != null) { OnPara_CheckedChanged(checkedButton, null); }

                }
                catch (Exception Desc)
                {
                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                }
            });
        }

        private void OnPara_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                RJCodeUI_M1.RJControls.RJRadioButton rdoButton = (RJCodeUI_M1.RJControls.RJRadioButton)sender;

                if (wpg == null) { return; }
                if (rdoButton.Checked)
                {
                    switch (rdoButton.Text)
                    {
                        case "Edge(L)":
                            wpg.SelectedObject = CVisionTools.Lines_L[CDisplayManager.CameraIndex];
                            break;
                        case "Edge(R)":
                            wpg.SelectedObject = CVisionTools.Lines_R[CDisplayManager.CameraIndex];
                            break;
                        case "Edge(Top)":
                            wpg.SelectedObject = CVisionTools.Lines_TOP[CDisplayManager.CameraIndex];
                            break;
                        case "Contour":
                            wpg.SelectedObject = CVisionTools.Contours[CDisplayManager.CameraIndex];
                            break;
                        case "Blob":
                            wpg.SelectedObject = CVisionTools.Blobs[CDisplayManager.CameraIndex];
                            break;
                        case "Vision ":
                            wpg.SelectedObject = CVisionTools.PropertyVision;
                            break;
                        case "Feature":
                            wpg.SelectedObject = CVisionTools.Features[CDisplayManager.CameraIndex];
                            break;
                    }                    
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnSaveVisionParam_Click(object sender, EventArgs e)
        {
            try
            {
                if (CCommon.ShowdialogMessageBox("Save", "현재까지 설정된 값을 저장하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    for (int i = 0; i < Global.Device.CAMERA_COUNT; i++)
                    {
                        CVisionTools.SaveTools(Global.Recipe.Name);
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
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
