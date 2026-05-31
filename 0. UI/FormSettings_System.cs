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
using System.IO;

using OpenCvSharp;

using MetroFramework.Forms;
using MetroFramework.Controls;


namespace KtemVisionSystem
{
    public partial class FormSettings_System : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;

        public FormSettings_System()
        {
            InitializeComponent();
        }

        private void FormTeachingSelect_Load(object sender, EventArgs e)
        {
            try
            {
                //CUtil.UpdateLabelOnOff(btnOptionDryRun, Global.System.USE_DRY_RUN);
                //CUtil.UpdateLabelOnOff(btnOptionSafetyDoor, Global.System.USE_SAFETY_DOOR);

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            
        }




        private void OnClickTools(object sender, EventArgs e)
        {
            try
            {
                string strIndex = (sender as MetroFramework.Controls.MetroButton).Text;

                //switch(strIndex)
                //{
                //    case "Line Guage":
                //        FormSettings_ELineGuage FrmSettings_eLineGuage = new FormSettings_ELineGuage(new Euresys.Open_eVision_2_16.EImageBW8());
                //        FrmSettings_eLineGuage.Show();
                //        break;
                //    case "Circle Guage":
                //        FormSettings_ECircleGuage FrmSettings_eCircleGuage = new FormSettings_ECircleGuage(new Euresys.Open_eVision_2_16.EImageBW8());
                //        FrmSettings_eCircleGuage.Show();
                //        break;
                //    case "Filter":
                //        FormSettings_EFilter FrmSettings_eFilter = new FormSettings_EFilter(new Euresys.Open_eVision_2_16.EImageBW8());
                //        FrmSettings_eFilter.Show();
                //        break;
                //    case "ROI":
                //        FormSettings_EROI FrmSettings_eROI = new FormSettings_EROI(new Euresys.Open_eVision_2_16.EImageBW8());
                //        FrmSettings_eROI.Show();
                //        break;
                //    case "Template Matching":
                //        FormSettings_EMatcher FrmSettings_eMatcher = new FormSettings_EMatcher(new Euresys.Open_eVision_2_16.EImageBW8());
                //        FrmSettings_eMatcher.Show();
                //        break;
                //    case "DataMatrix":
                //        FormSettings_EDataMatrix FrmSettings_eDataMatrix = new FormSettings_EDataMatrix(new Euresys.Open_eVision_2_16.EImageBW8());
                //        FrmSettings_eDataMatrix.Show();
                //        break;
                //}

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

        }

        private void OnClickSettings(object sender, MouseEventArgs e)
        {
            try
            {
                string strIndex = (sender as MetroTile).Text;

                switch (strIndex)
                {
                    //case "TIME OUT":
                    //    FormSetting_TimeOut FrmSetting_TimeOut = new FormSetting_TimeOut();
                    //    FrmSetting_TimeOut.ShowDialog();
                    //    break;
                    //case "TOWER LAMP":
                    //    FormSetting_TowerLamp FrmSetting_TowerLamp = new FormSetting_TowerLamp();
                    //    FrmSetting_TowerLamp.ShowDialog();
                    //    break;
                }

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void OnClickOption(object sender, EventArgs e)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnDevice_Click(object sender, EventArgs e)
        {
            FormSettings_Devices FrmDevice = new FormSettings_Devices();
            FrmDevice.ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Global.System.SaveConfig();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}


