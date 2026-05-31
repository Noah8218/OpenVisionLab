using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using MetroFramework.Forms;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using RJCodeUI_M1.RJControls;
using OpenVisionLab._2._Common;
using RJCodeUI_M1.RJForms;
using static System.Windows.Controls.WpfPropertyGrid.KnownTypes;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormSetting_UTIL : RJChildForm
    {
        private CGlobal Global = CGlobal.Inst;

        private int originalExStyle = -1;
        private bool enableFormLevelDoubleBuffering = true;

        private int m_nSelectedIndex = 0;

        private System.Windows.Forms.Integration.ElementHost host = null;
        private System.Windows.Controls.WpfPropertyGrid.PropertyGrid wpg = null;

        public FormSetting_UTIL()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            InitEvent();

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

            wpg.SelectedObject = CImageManager.SaveImages.Property;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private bool InitEvent()
        {
            try
            {
                this.KeyPreview = true;
                this.KeyDown += Form_KeyDown;
                Global.Recipe.EventChagedRecipe += OnChangedRecipe;
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

        private void btnSaveParameter_Click(object sender, EventArgs e)
        {
            try
            {
                if (CCommon.ShowdialogMessageBox("SAVE", "DO YOU WANT TO SAVE THE PARAMETER", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {                
                    Global.Data.SETTING.SaveConfig(Global.Recipe.Name);
                    Global.Data.SPEC.SaveConfig(Global.Recipe.Name);
                    CImageManager.SaveConfig(Global.Recipe.Name);
                    Global.Device.DIO_PLC.Property.SaveConfig(Global.Recipe.Name);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowdialogMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void OnRdoPara_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                RJCodeUI_M1.RJControls.RJRadioButton rdoButton = (RJCodeUI_M1.RJControls.RJRadioButton)sender;

                if (rdoButton.Checked)
                {
                    switch (rdoButton.Text)
                    {
                        case "Save UTIL":
                            wpg.SelectedObject = CImageManager.SaveImages.Property;
                            break;
                        case "Delete UTIL":
                            wpg.SelectedObject = CImageManager.DelImages.Property;
                            break;
                        case "SPEC":
                            wpg.SelectedObject = CGlobal.Inst.Data.SPEC;
                            break;
                        case "SETTING":
                            wpg.SelectedObject = CGlobal.Inst.Data.SETTING;
                            break;
                        case "PLC":
                            wpg.SelectedObject = CGlobal.Inst.Device.DIO_PLC.Property;
                            break;
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

    }
}

