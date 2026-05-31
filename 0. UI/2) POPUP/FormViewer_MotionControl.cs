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
    public partial class FormViewer_MotionControl : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;

        private const int DGV_NO = 0;
        private const int DGV_ID = 1;
        private const int DGV_STATUS = 2;

        public CPropertyMotion pos = null;
        public CAXIS_AJIN Axis = null;

        public CStatusMotion Status = null;
        public CStatusMotionHome Home = null;
        public FormViewer_MotionControl(CPropertyMotion position)
        {
            InitializeComponent();

            pos = position;

            if(pos != null)
            {
                this.Text = pos.NAME;

                try
                {
                    Axis = Global.iDevice.MOTION_AJIN.Axises[pos.AXIS_NO];
                    Status = Axis.Status;
                    Home = Axis.Home;
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }        
            }
        }

        private void FormViewer_MotionControl_Load(object sender, EventArgs e)
        {
            InitUI();

            timerView.Enabled = true;

            Global.System.EventUpdateStyle += OnChangeStyle;

            this.KeyPreview = true;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void FormViewer_LotOpen_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        public void OnChangeStyle(object sender, EventArgs e)
        {
        }        

        public bool InitUI()
        {
            try
            {
                metroTextBox3.Text = pos.POSITION.ToString();
                metroTextBox2.Text = pos.SPEED_FAST.ToString();
                metroTextBox1.Text = pos.SPEED_SLOW.ToString();
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
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
                if (Axis == null) return;

                CStatusMotion Status = Axis.Status;
                CStatusMotionHome Home = Axis.Home;
                if (Status == null) { return; }
                if (Home == null) { return; }

                if (Status.GPIO_IN1) { CUtil.UpdateLabelOnOff(lbStatusGPIO1, true); }
                else { CUtil.UpdateLabelOnOff(lbStatusGPIO1, false); }

                if (Status.ServoOn) { CUtil.UpdateLabelOnOff(lbStatusServoOn, true); }
                else { CUtil.UpdateLabelOnOff(lbStatusServoOn, false); }

                if (Status.PlusLimit) { CUtil.UpdateLabelOnOff(lbStatusLimitPlus, true); }
                else { CUtil.UpdateLabelOnOff(lbStatusLimitPlus, false); }

                if (Status.MinusLimit) { CUtil.UpdateLabelOnOff(lbStatusLimitMinus, true); }
                else { CUtil.UpdateLabelOnOff(lbStatusLimitMinus, false); }

                if (Status.ServoAlarm) { CUtil.UpdateLabelOnOff(lbStatusServoAlarm, true); }
                else { CUtil.UpdateLabelOnOff(lbStatusServoAlarm, false); }

                if (Status.Inposition) { CUtil.UpdateLabelOnOff(lbStatusInPosition, true); }
                else { CUtil.UpdateLabelOnOff(lbStatusInPosition, false); }

                if (Status.InMotion) { CUtil.UpdateLabelOnOff(lbStatusMotioning, true); }
                else { CUtil.UpdateLabelOnOff(lbStatusMotioning, false); }

                if (Status.HomeSensor) { CUtil.UpdateLabelOnOff(lbStatusHome, true); }
                else { CUtil.UpdateLabelOnOff(lbStatusHome, false); }

                if (Axis.HomeComplete) { CUtil.UpdateLabelOnOff(lbStatusHomeComplete, true); }
                else { CUtil.UpdateLabelOnOff(lbStatusHomeComplete, false); }

                if (Axis.Status.ServoOn) btnMotorServoOn.Text = "SERVO OFF";
                else btnMotorServoOn.Text = "SERVO ON";

                lbActualPulse.Text = (Status.ActualPos).ToString();
                //lbActualDistance.Text = (Status.ActualPos).ToString("F3") + "mm";
                lbCommandPulse.Text = Status.CommandPos.ToString();
                //lbCommandDistance.Text = (Status.CommandPos).ToString("F3") + "mm";
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSaveZigElevatorBufferCount_Click(object sender, EventArgs e)
        {
            try
            {   
                Global.iData.PropertyBufferPitch.SaveConfig(Global.System.Recipe.Name);
                Global.iData.PropertyBufferPitch.LoadConfig(Global.System.Recipe.Name);
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }            
        }


        private void btnStartHome_Click(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) CUtil.ShowMessageBox("ALARM", "SELECT THE AXIS");
                else
                {
                    if (CUtil.ShowMessageBox("CHECK", "DO YOU WANT TO SET THE HOME?", FormMessageBox.MESSAGEBOX_TYPE.OKCANCEL))
                    {
                        Axis.StartThreadHome();
                    }
                }
            }
            
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnMotorServoOn_Click(object sender, EventArgs e)
        {
            try
            {
                Axis.ServoOnOff(!Status.ServoOn);

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}   Action ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, "SERVO CONTROL");

            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnJogPlus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (Axis == null) return;
                int nJogSpeed = int.Parse(tbJogSpeed.Text);

                string strIndex = "";

                if (sender is Button) strIndex = ((Button)sender).Name;

                switch (strIndex)
                {
                    case "btnJogPlus":
                        Axis.JogStart(nJogSpeed);
                        break;
                    case "btnJogMinus":
                        Axis.JogStart(nJogSpeed * -1);
                        break;
                }


            }
            catch (Exception ex)
            {

            }
        }

        private void btnJogPlus_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (Axis == null) return;
                Axis.JogStop();
            }
            catch (Exception ex)
            {

            }

        }

        private void btnAxisStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) return;

                Axis.JogStop();
                Axis.EStop();

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void metroTile4_Click(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) return;
                if (Axis.Status.InMotion) return;

                if (!Axis.HomeComplete)
                {
                    CUtil.ShowMessageBox("ALARM", "SET THE HOME FIRST");
                    return;
                }

                double dPulsePermm = Axis.PulsePermm;

                double dStepSpeed = double.Parse(tbStepSpeed.Text);
                double dStepmm = double.Parse(tbStepmm.Text);
                double dActualPos = Axis.Status.ActualPos;
                double dTargetPos = dActualPos + dStepmm;

                Axis.SetMotionEndVelocity(Axis.AxisNo, dStepSpeed);
                Axis.Move(dTargetPos, dStepSpeed);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void metroTile5_Click(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) return;
                if (Axis.Status.InMotion) return;

                if (!Axis.HomeComplete)
                {
                    CUtil.ShowMessageBox("ALARM", "SET THE HOME FIRST");
                    return;
                }

                double dPulsePermm = Axis.PulsePermm;

                double dMoveSpeed = double.Parse(tbAbsoluteSpeed.Text);
                double dTargetmm = double.Parse(tbAbsoluteTarget.Text);

                Axis.SetMotionEndVelocity(Axis.AxisNo, dMoveSpeed);
                Axis.Move(dTargetmm, dMoveSpeed);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                pos.POSITION = double.Parse(metroTextBox3.Text);
                pos.SPEED_FAST = double.Parse(metroTextBox2.Text);
                pos.SPEED_SLOW = double.Parse(metroTextBox1.Text);

                pos.SaveConfig(Global.System.Recipe.Name);

                this.DialogResult = DialogResult.OK;
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
    
        }

        private void bntMove_Pos(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) return;
                if (Axis.Status.InMotion) return;

                if (!Axis.HomeComplete)
                {
                    CUtil.ShowMessageBox("ALARM", "SET THE HOME FIRST");
                    return;
                }

                double dPulsePermm = Axis.PulsePermm;

                double dMoveSpeed = double.Parse(tbAbsoluteSpeed.Text);
                double dTargetmm = double.Parse(tbAbsoluteTarget.Text);

                Axis.SetMotionEndVelocity(Axis.AxisNo, dMoveSpeed);
                Axis.Move(dTargetmm, dMoveSpeed);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}
