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
    public partial class FormSettings_Developer : MetroForm
    {
        CGlobal Global = CGlobal.Inst;

        private CPropertyMotion PropertyMotion = null;
        private CPropertyMotion PropertyLoad = new CPropertyMotion("");

        #region Event Register        
        public EventHandler<EventArgs> EventUpdateUi;
        #endregion
        public FormSettings_Developer()
        {
            InitializeComponent();

            try
            {
               

            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);                
            }
        }

        private void FormSettings_Developer_Load(object sender, EventArgs e)
        {
        }
       
        private void btnPositionLoad_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Application.StartupPath;
                ofd.Filter = "Position Files(*.xml)|*.xml";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string strFilePath = ofd.FileName;
                    
                    PropertyLoad.ManualLoadConfig(strFilePath);

                    string strPositionName = Path.GetFileName(strFilePath);
                    lbName.Text = strPositionName;

                    if (PropertyLoad != null)
                    {
                        PropertyLoad.NAME = strPositionName;
                        PropertyLoad.POSITION_NAME = strPositionName;
                        propertygrid_Parameter.SelectedObject = PropertyLoad;
                    }

                    lbPositionPath.Text = strFilePath;

                    CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }            
        }

        private void btnPositionSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (PropertyLoad != null)
                {
                    PropertyLoad.ManualSaveConfig(lbPositionPath.Text);
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
 }

