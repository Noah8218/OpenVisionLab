using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using MetroFramework.Forms;
using MetroFramework.Controls;
using System.Collections.Generic;

namespace IntelligentFactory
{
    public partial class FormSetting_TowerLamp : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;
        private CPropertyTowerLamp PropertyTowerLamp = null;

        public FormSetting_TowerLamp()
        {
            InitializeComponent();            
        }

        private void FormSetting_TowerLamp_Load(object sender, EventArgs e)
        {
            InitUI();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void FormSetting_Delay_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        public bool InitUI()
        {
            try
            {

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return true;
        }

        public bool UpdateTowerLamp()
        {
            try
            {
                if (PropertyTowerLamp == null) return false;

                CUtil.UpdateLabelOnOff(btnRedOn, false);
                CUtil.UpdateLabelOnOff(btnRedOff, false);
                CUtil.UpdateLabelOnOff(btnRedBlink200ms, false);
                CUtil.UpdateLabelOnOff(btnRedBlink500ms, false);

                CUtil.UpdateLabelOnOff(btnYellowOn, false);
                CUtil.UpdateLabelOnOff(btnYellowOff, false);
                CUtil.UpdateLabelOnOff(btnYellowBlink200ms, false);
                CUtil.UpdateLabelOnOff(btnYellowBlink500ms, false);

                CUtil.UpdateLabelOnOff(btnGreenOn, false);
                CUtil.UpdateLabelOnOff(btnGreenOff, false);
                CUtil.UpdateLabelOnOff(btnGreenBlink200ms, false);
                CUtil.UpdateLabelOnOff(btnGreenBlink500ms, false);

                CUtil.UpdateLabelOnOff(btnBuzzerOn, false);
                CUtil.UpdateLabelOnOff(btnBuzzerOff, false);
                CUtil.UpdateLabelOnOff(btnBuzzerBlink200ms, false);
                CUtil.UpdateLabelOnOff(btnBuzzerBlink500ms, false);

                switch (PropertyTowerLamp.RED)
                {
                    case CPropertyTowerLamp.ACTION_TYPE.ON: CUtil.UpdateLabelOnOff(btnRedOn, true); break;
                    case CPropertyTowerLamp.ACTION_TYPE.OFF: CUtil.UpdateLabelOnOff(btnRedOff, true); break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_200MS: CUtil.UpdateLabelOnOff(btnRedBlink200ms, true); break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_500MS: CUtil.UpdateLabelOnOff(btnRedBlink500ms, true); break;
                }

                switch (PropertyTowerLamp.YELLOW)
                {
                    case CPropertyTowerLamp.ACTION_TYPE.ON: CUtil.UpdateLabelOnOff(btnYellowOn, true); break;
                    case CPropertyTowerLamp.ACTION_TYPE.OFF: CUtil.UpdateLabelOnOff(btnYellowOff, true); break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_200MS: CUtil.UpdateLabelOnOff(btnYellowBlink200ms, true); break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_500MS: CUtil.UpdateLabelOnOff(btnYellowBlink500ms, true); break;
                }

                switch (PropertyTowerLamp.GREEN)
                {
                    case CPropertyTowerLamp.ACTION_TYPE.ON: CUtil.UpdateLabelOnOff(btnGreenOn, true); break;
                    case CPropertyTowerLamp.ACTION_TYPE.OFF: CUtil.UpdateLabelOnOff(btnGreenOff, true); break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_200MS: CUtil.UpdateLabelOnOff(btnGreenBlink200ms, true); break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_500MS: CUtil.UpdateLabelOnOff(btnGreenBlink500ms, true); break;
                }

                switch (PropertyTowerLamp.BUZZER)
                {
                    case CPropertyTowerLamp.ACTION_TYPE.ON: CUtil.UpdateLabelOnOff(btnBuzzerOn, true); break;
                    case CPropertyTowerLamp.ACTION_TYPE.OFF: CUtil.UpdateLabelOnOff(btnBuzzerOff, true); break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_200MS: CUtil.UpdateLabelOnOff(btnBuzzerBlink200ms, true); break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_500MS: CUtil.UpdateLabelOnOff(btnBuzzerBlink500ms, true); break;
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return true;
        }



        private void timerView_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PropertyTowerLamp == null) return;

                

            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (CUtil.ShowMessageBox("SAVE", "DO YOU WANT TO SAVE THE PARAMETER OF DELAY", FormMessageBox.MESSAGEBOX_TYPE.OKCANCEL))
                    {
                        //Global.iDevice.TOWER_LAMP.PropertyInitialize.SaveConfig();
                        //Global.iDevice.TOWER_LAMP.PropertyLotEnd.SaveConfig();
                        //Global.iDevice.TOWER_LAMP.PropertyAuto.SaveConfig();
                        //Global.iDevice.TOWER_LAMP.PropertyStop.SaveConfig();
                        //Global.iDevice.TOWER_LAMP.PropertyAlarm.SaveConfig();
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                    CUtil.ShowMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
                }

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void OnClickRed(object sender, EventArgs e)
        {
            try
            {
                string strIndex = (sender as MetroTile).Text;

                if (PropertyTowerLamp == null) return;

                switch (strIndex)
                {
                    case "ON": PropertyTowerLamp.RED = CPropertyTowerLamp.ACTION_TYPE.ON; break; 
                    case "OFF": PropertyTowerLamp.RED = CPropertyTowerLamp.ACTION_TYPE.OFF; break;
                    case "200 ms": PropertyTowerLamp.RED = CPropertyTowerLamp.ACTION_TYPE.BLINK_200MS; break;
                    case "500 ms": PropertyTowerLamp.RED = CPropertyTowerLamp.ACTION_TYPE.BLINK_500MS; break;
                }

                UpdateTowerLamp();
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void OnClickYellow(object sender, EventArgs e)
        {
            try
            {
                string strIndex = (sender as MetroTile).Text;

                if (PropertyTowerLamp == null) return;

                switch (strIndex)
                {
                    case "ON": PropertyTowerLamp.YELLOW = CPropertyTowerLamp.ACTION_TYPE.ON; break;
                    case "OFF": PropertyTowerLamp.YELLOW = CPropertyTowerLamp.ACTION_TYPE.OFF; break;
                    case "200 ms": PropertyTowerLamp.YELLOW = CPropertyTowerLamp.ACTION_TYPE.BLINK_200MS; break;
                    case "500 ms": PropertyTowerLamp.YELLOW = CPropertyTowerLamp.ACTION_TYPE.BLINK_500MS; break;
                }

                UpdateTowerLamp();
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void OnClickGreen(object sender, EventArgs e)
        {
            try
            {
                string strIndex = (sender as MetroTile).Text;

                if (PropertyTowerLamp == null) return;

                switch (strIndex)
                {
                    case "ON": PropertyTowerLamp.GREEN = CPropertyTowerLamp.ACTION_TYPE.ON; break;
                    case "OFF": PropertyTowerLamp.GREEN = CPropertyTowerLamp.ACTION_TYPE.OFF; break;
                    case "200 ms": PropertyTowerLamp.GREEN = CPropertyTowerLamp.ACTION_TYPE.BLINK_200MS; break;
                    case "500 ms": PropertyTowerLamp.GREEN = CPropertyTowerLamp.ACTION_TYPE.BLINK_500MS; break;
                }

                UpdateTowerLamp();
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void OnClickBuzzer(object sender, EventArgs e)
        {
            try
            {
                string strIndex = (sender as MetroTile).Text;

                if (PropertyTowerLamp == null) return;

                switch (strIndex)
                {
                    case "ON": PropertyTowerLamp.BUZZER = CPropertyTowerLamp.ACTION_TYPE.ON; break;
                    case "OFF": PropertyTowerLamp.BUZZER = CPropertyTowerLamp.ACTION_TYPE.OFF; break;
                    case "200 ms": PropertyTowerLamp.BUZZER = CPropertyTowerLamp.ACTION_TYPE.BLINK_200MS; break;
                    case "500 ms": PropertyTowerLamp.BUZZER = CPropertyTowerLamp.ACTION_TYPE.BLINK_500MS; break;
                }

                UpdateTowerLamp();
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void OnClickSituation(object sender, EventArgs e)
        {
            try
            {
                string strIndex = (sender as Label).Text;

                //switch (strIndex)
                //{
                //    case "INITIALIZE": PropertyTowerLamp = Global.iDevice.TOWER_LAMP.PropertyInitialize; break;
                //    case "LOT END": PropertyTowerLamp = Global.iDevice.TOWER_LAMP.PropertyLotEnd; break;
                //    case "AUTO": PropertyTowerLamp = Global.iDevice.TOWER_LAMP.PropertyAuto; break;
                //    case "STOP": PropertyTowerLamp = Global.iDevice.TOWER_LAMP.PropertyStop; break;
                //    case "ALARM": PropertyTowerLamp = Global.iDevice.TOWER_LAMP.PropertyAlarm; break;
                //}

                UpdateTowerLamp();
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void timerR_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PropertyTowerLamp == null) return;

                switch (PropertyTowerLamp.RED)
                {
                    case CPropertyTowerLamp.ACTION_TYPE.ON: lbStatusRed.BackColor = Color.Red; break;
                    case CPropertyTowerLamp.ACTION_TYPE.OFF: lbStatusRed.BackColor = Color.FromArgb(64, 0, 0); break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_200MS:
                        {
                            timerR.Interval = 200;
                            if (lbStatusRed.BackColor == Color.FromArgb(64, 0, 0)) lbStatusRed.BackColor = Color.Red;
                            else lbStatusRed.BackColor = Color.FromArgb(64, 0, 0);
                        }
                        break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_500MS:
                        {
                            timerR.Interval = 500;
                            if (lbStatusRed.BackColor == Color.FromArgb(64, 0, 0)) lbStatusRed.BackColor = Color.Red;
                            else lbStatusRed.BackColor = Color.FromArgb(64, 0, 0);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void timerG_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PropertyTowerLamp == null) return;

                switch (PropertyTowerLamp.GREEN)
                {
                    case CPropertyTowerLamp.ACTION_TYPE.ON: lbStatusGreen.BackColor = Color.Lime; break;
                    case CPropertyTowerLamp.ACTION_TYPE.OFF: lbStatusGreen.BackColor = Color.FromArgb(0, 64, 0); break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_200MS:
                        {
                            timerG.Interval = 200;
                            if (lbStatusGreen.BackColor == Color.FromArgb(0, 64, 0)) lbStatusGreen.BackColor = Color.Lime;
                            else lbStatusGreen.BackColor = Color.FromArgb(0, 64, 0);
                        }
                        break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_500MS:
                        {
                            timerG.Interval = 500;
                            if (lbStatusGreen.BackColor == Color.FromArgb(0, 64, 0)) lbStatusGreen.BackColor = Color.Lime;
                            else lbStatusGreen.BackColor = Color.FromArgb(0, 64, 0);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void timerY_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PropertyTowerLamp == null) return;

                switch (PropertyTowerLamp.YELLOW)
                {
                    case CPropertyTowerLamp.ACTION_TYPE.ON: lbStatusYellow.BackColor = Color.Yellow; break;
                    case CPropertyTowerLamp.ACTION_TYPE.OFF: lbStatusYellow.BackColor = Color.FromArgb(64, 64, 0); break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_200MS:
                        {
                            timerY.Interval = 200;
                            if (lbStatusYellow.BackColor == Color.FromArgb(64, 64, 0)) lbStatusYellow.BackColor = Color.Yellow;
                            else lbStatusYellow.BackColor = Color.FromArgb(64, 64, 0);
                        }
                        break;
                    case CPropertyTowerLamp.ACTION_TYPE.BLINK_500MS:
                        {
                            timerY.Interval = 500;
                            if (lbStatusYellow.BackColor == Color.FromArgb(64, 64, 0)) lbStatusYellow.BackColor = Color.Yellow;
                            else lbStatusYellow.BackColor = Color.FromArgb(64, 64, 0);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}
