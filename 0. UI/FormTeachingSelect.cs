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


namespace IntelligentFactory
{
    public partial class FormTeachingSelect : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;

        public FormTeachingSelect()
        {
            InitializeComponent();
        }

        private void FormTeachingSelect_Load(object sender, EventArgs e)
        {
            try
            {
                CUtil.UpdateLabelOnOff(btnOptionDryRun, Global.System.USE_DRY_RUN);
                CUtil.UpdateLabelOnOff(btnOptionUpper, Global.System.USE_SEQ_UPPER);
                CUtil.UpdateLabelOnOff(btnOptionID, Global.System.USE_ID_MAPPING);
                CUtil.UpdateLabelOnOff(btnOptionSafetyDoor, Global.System.USE_SAFETY_DOOR);

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            
        }

        private void btnVisionSettings_Click(object sender, EventArgs e)
        {
            //FormSettings_ELineGuage FrmSettingsELineGuage = new FormSettings_ELineGuage(new Euresys.Open_eVision_2_16.EImageBW8());
            //FrmSettingsELineGuage.Show();
        }

        private void btnCameraSettings_Click(object sender, EventArgs e)
        {
            FormSettings_Camera FrmSettingsCamera = new FormSettings_Camera();
            FrmSettingsCamera.Show();
        }

        private void btnLightSettings_Click(object sender, EventArgs e)
        {
            FormSettings_Illumination FrmSettingsIllumination = new FormSettings_Illumination();
            FrmSettingsIllumination.Show();
        }

        private void btnTest_CircleGuage_Click(object sender, EventArgs e)
        {
            //FormSettings_ECircleGuage FrmSettingsECircleGuage = new FormSettings_ECircleGuage(new Euresys.Open_eVision_2_16.EImageBW8());
            //FrmSettingsECircleGuage.Show();
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
                    case "TIME OUT":
                        FormSetting_TimeOut FrmSetting_TimeOut = new FormSetting_TimeOut();
                        FrmSetting_TimeOut.ShowDialog();
                        break;
                    case "TOWER LAMP":
                        FormSetting_TowerLamp FrmSetting_TowerLamp = new FormSetting_TowerLamp();
                        FrmSetting_TowerLamp.ShowDialog();
                        break;
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
                if (sender is MetroTile)
                {
                    string strIndex = (sender as MetroTile).Text;

                    switch (strIndex)
                    {
                        case "USE DRY RUN":
                            Global.System.USE_DRY_RUN = !Global.System.USE_DRY_RUN;
                            break;
                        case "USE UPPER":
                            Global.System.USE_SEQ_UPPER = !Global.System.USE_SEQ_UPPER;
                            break;
                        case "USE ID MAPPING":
                            Global.System.USE_ID_MAPPING = !Global.System.USE_ID_MAPPING;
                            break;
                        case "USE SAFETY DOOR":
                            Global.System.USE_SAFETY_DOOR = !Global.System.USE_SAFETY_DOOR;
                            break;
                        case "USE BCR VERIFY":
                            Global.System.USE_BCR_VERIFY = !Global.System.USE_BCR_VERIFY;
                            break;
                    }
                }

                CUtil.UpdateLabelOnOff(btnOptionDryRun, Global.System.USE_DRY_RUN);
                CUtil.UpdateLabelOnOff(btnOptionUpper, Global.System.USE_SEQ_UPPER);
                CUtil.UpdateLabelOnOff(btnOptionID, Global.System.USE_ID_MAPPING);
                CUtil.UpdateLabelOnOff(btnOptionSafetyDoor, Global.System.USE_SAFETY_DOOR);
                CUtil.UpdateLabelOnOff(btnOptionBcrVerify, Global.System.USE_BCR_VERIFY);

                Global.System.SaveConfig();

                Global.iDevice.CAMERA_LD.Property.SaveConfig();
                Global.iDevice.CAMERA_ULD.Property.SaveConfig();
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
    }
}


