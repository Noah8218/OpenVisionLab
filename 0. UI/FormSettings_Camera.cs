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
    public partial class FormSettings_Camera : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;
        private int m_nCameraIndex = 0;

        #region Event Register        
        public EventHandler<EventArgs> EventUpdateUi;
        #endregion
        public FormSettings_Camera()
        {
            InitializeComponent();                        
        }

        private void InitCameraItem()
        {
            for (int i = 0; i < Global.Device.CAMERA_COUNT; i++)
            {
                cbCamera.Items.Add("Camera " + (i + 1));
            }
           
            cbCamera.SelectedIndex = 0;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
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

        private bool InitEvent()
        {
            try
            {
                this.KeyPreview = true;
                this.KeyDown += Form_KeyDown;
                Global.Recipe.EventChagedRecipe += OnChangedRecipe;

                tbCameraCount.Text = Global.Device.CAMERA_COUNT.ToString();

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

        private void cbCamera_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int nIndex = cbCamera.SelectedIndex;

                if(Global.Device.CAMERAS.Count > nIndex)
                {
                    propertyGrid_Cam.SelectedObject = Global.Device.CAMERAS[nIndex].Property;
                }                
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnSaveParameter_Click(object sender, EventArgs e)
        {
            try
            {
                if (CUtil.ShowdialogMessageBox("SAVE", "DO YOU WANT TO SAVE THE PARAMETER", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    foreach(var Cam in Global.Device.CAMERAS)
                    {
                        //Cam.SetWidth((uint)Cam.Property.WIDTH);
                        //Cam.SetHeight((uint)Cam.Property.HEIGHT);
                        Cam.SetGain(Cam.Property.GAIN);
                        Cam.SetExposure(Cam.Property.EXPOSURETIME_US);
                        Cam.Property.SaveConfig(Global.Recipe.Name);
                    }                    
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowdialogMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void btnSetCamera_Click(object sender, EventArgs e)
        {
            try
            {
                if (CUtil.ShowdialogMessageBox("Edit", "Please Restart the application to view changes\nRestart now?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    Global.Device.CAMERA_COUNT = int.Parse(tbCameraCount.Text);
                    Global.Device.SaveConfig();
                    Global.Close();

                    Application.Restart();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowdialogMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }
    }
 }

