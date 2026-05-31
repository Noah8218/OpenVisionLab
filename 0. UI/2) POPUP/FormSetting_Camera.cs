using System;
using System.Windows.Forms;
using System.Reflection;
using RJCodeUI_M1.RJForms;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormSetting_Camera : RJChildForm
    {
        private CGlobal Global = CGlobal.Inst;

        private int originalExStyle = -1;
        private bool enableFormLevelDoubleBuffering = true;

        private int m_nSelectedIndex = 0;

        public FormSetting_Camera()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            InitEvent();
            InitCameraItem();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void InitCameraItem()
        {
            for (int i = 0; i < Global.Device.CAMERA_COUNT; i++)
            {
                cbCamera.Items.Add("Camera " + (i + 1));
            }

            cbCamera.SelectedIndex = 0;
        }

        private bool InitEvent()
        {
            try
            {
                this.KeyPreview = true;
                this.KeyDown += Form_KeyDown;
                Global.Recipe.EventChagedRecipe += OnChangedRecipe;

                tbCameraCount.Text = Global.Device.CAMERA_COUNT.ToString();

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }

        private void OnChangedRecipe(object sender, EventArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {

            });
        }

        private void cbCamera_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int nIndex = cbCamera.SelectedIndex;

                if (Global.Device.CAMERAS.Count > nIndex)
                {
                    propertyGrid_Cam.SelectedObject = Global.Device.CAMERAS[nIndex].Property;
                }
                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnSaveParameter_Click(object sender, EventArgs e)
        {
            try
            {
                if (CCommon.ShowdialogMessageBox("SAVE", "DO YOU WANT TO SAVE THE PARAMETER", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    int nIndex = cbCamera.SelectedIndex;
                    Global.Device.CAMERAS[nIndex].Property = (CPropertyCamera)propertyGrid_Cam.SelectedObject;

                    foreach (var Cam in Global.Device.CAMERAS)
                    {
                        //Cam.SetWidth((uint)Cam.Property.WIDTH);
                        //Cam.SetHeight((uint)Cam.Property.HEIGHT);
                        //Cam.SetGain(Cam.Property.GAIN);
                        //Cam.SetExposure(Cam.Property.EXPOSURETIME_US);
                        Cam.Property.SaveConfig(Global.Recipe.Name);
                    }
                    //for (int i = 0; i < Global.Device.CAMERAS.Count; i++) { Global.Device.CAMERAS[i].Property.SaveConfig(Global.Recipe.Name); }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowdialogMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void btnSetCamera_Click(object sender, EventArgs e)
        {
            try
            {
                if (CCommon.ShowdialogMessageBox("Edit", "Please Restart the application to view changes\nRestart now?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    Global.Device.CAMERA_COUNT = int.Parse(tbCameraCount.Text);
                    Global.Device.SaveConfig();
                    Global.Close();

                    Application.Restart();
                    Environment.Exit(0);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowdialogMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void tbCameraCount_onTextChanged(object sender, EventArgs e)
        {

        }

        private void rjLabel3_Click(object sender, EventArgs e)
        {

        }
    }
}

