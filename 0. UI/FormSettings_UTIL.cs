using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Diagnostics;


//IF 전용 Library
using OpenCvSharp;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;

using MetroFramework;
using MetroFramework.Forms;
using ADOX;
using Keys = System.Windows.Forms.Keys;

namespace KtemVisionSystem
{
    public partial class FormSettings_UTIL : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;
        private int m_nCameraIndex = 0;

        #region Event Register        
        public EventHandler<EventArgs> EventUpdateUi;
        #endregion
        public FormSettings_UTIL()
        {
            InitializeComponent();                        
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            InitEvent();
            propertyGrid_Parameter.SelectedObject = CImageManager.SaveImages.Property;
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

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        private void OnChangedRecipe(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnChangedRecipe(sender, e);
                    }));
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            else
            {
                try
                {
                    
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }                
            }
        }      
        private void btnSaveParameter_Click(object sender, EventArgs e)
        {
            try
            {
                if (CUtil.ShowdialogMessageBox("SAVE", "DO YOU WANT TO SAVE THE PARAMETER", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    Global.Data.SPEC.SaveConfig(Global.Recipe.Name);
                    CImageManager.SaveConfig(Global.Recipe.Name);
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowdialogMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
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
                            propertyGrid_Parameter.SelectedObject = CImageManager.SaveImages.Property;
                            break;
                        case "Delete UTIL":
                            propertyGrid_Parameter.SelectedObject = CImageManager.DelImages.Property;
                            break;
                        case "SPEC":
                            propertyGrid_Parameter.SelectedObject = CGlobal.Inst.Data.SPEC;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
 }

