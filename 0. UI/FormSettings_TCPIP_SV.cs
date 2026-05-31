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

namespace KtemVisionSystem
{
    public partial class FormSettings_TCPIP_SV : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;
        private int m_nCameraIndex = 0;

        #region Event Register        
        public EventHandler<EventArgs> EventUpdateUi;
        #endregion
        public FormSettings_TCPIP_SV()
        {
            InitializeComponent();

            cbTriggerMode.SelectedIndex = 0;
        }

        private void FormSettings_TCPIP_SV_Load(object sender, EventArgs e)
        {
            InitEvent();
        }

        private bool InitEvent()
        {
            try
            {                
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
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
        }

        private void cbIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            //m_nCameraIndex = cbIndex.SelectedIndex;
            //if (m_nCameraIndex >= Global.CamManager.Cameras.Count)
            //{
            //    CUtil.ShowMessageBox("ALARM", "Camera Index Overflow");
            //    return;
            //}
        }
    }
 }

