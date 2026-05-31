using MetroFramework.Controls;
using MetroFramework.Forms;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static KtemVisionSystem.CAXIS_AJIN;
using static KtemVisionSystem.CVision;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using ComboBox = System.Windows.Forms.ComboBox;

namespace KtemVisionSystem
{
    public partial class FormTeachingMotion : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;        

        #region Event Register        
        public EventHandler<EventArgs> EventUpdateUi;
        #endregion

        public CPropertyMotion pos = null;
        public CAXIS_AJIN Axis = null;

        public CStatusMotion Status = null;
        public CStatusMotionHome Home = null;

        public FormTeachingMotion()
        {
            InitializeComponent();
        }
        
        private void FormTeachingVision_Load(object sender, EventArgs e)
        {
            timerView.Enabled = true;
            this.KeyPreview = true;
            InitEvent();
            InitUI();
            InitControl();
        }

        private void InitControl()
        {
            try
            {
                foreach (DataGridViewColumn i in gridPosition.Columns)
                {
                    i.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                foreach (DataGridViewColumn c in gridPosition.Columns)
                {
                    c.DefaultCellStyle.Font = new Font("Arial", 15F, GraphicsUnit.Pixel);
                }
                ShowVisionPositionDgv();

                tbRewinderPitch.Texts = Global.Recipe.ReWinderPitch.ToString();
                tbLappingPitch.Texts = Global.Recipe.LappingPitch.ToString();
                tbProdudctSpeed.Texts = Global.Recipe.ProductSpeed.ToString();
                tbRadio.Texts = Global.Recipe.Radio.ToString();
                tbAngle.Texts = Global.Recipe.LappingAngle.ToString();                
            }
            catch (Exception Desc)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        public bool ShowVisionPositionDgv()
        {
            try
            {
                if (Global.Data.PositionManager.Positions.Count < 0)
                {
                    //Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, iData.PositionManager.ListPositionVisionStage.Count);
                    return false;
                }

                gridPosition.EnableHeadersVisualStyles = false;

                gridPosition.Rows.Clear();

                int nStageIndex = 0;

                for (int i = 0; i < Global.Data.PositionManager.Positions.Count; i++)
                {
                    if (nStageIndex == 8)
                    {
                        nStageIndex = 0;
                    }
                    nStageIndex++;

                    switch (Global.Data.PositionManager.Positions[i].Axis)
                    {
                        case "Y":                            
                            string strDistancebymm = (Global.Data.PositionManager.Positions[i].Position / 1000.0D).ToString() + "mm";
                            gridPosition.Rows.Add(Global.Data.PositionManager.Positions[i].Axis,
                        Global.Data.PositionManager.Positions[i].Name, Global.Data.PositionManager.Positions[i].Position, strDistancebymm);
                            break;
                    }
                }

                this.gridPosition.EnableHeadersVisualStyles = false; // Windows XP 비주얼 스타일 적용시 추가함!
                this.gridPosition.ColumnHeadersDefaultCellStyle.BackColor = DEFINE.BACK_COLOR;
                this.gridPosition.RowHeadersDefaultCellStyle.BackColor = DEFINE.BACK_COLOR;
                this.gridPosition.RowHeadersDefaultCellStyle.SelectionBackColor = DEFINE.BACK_COLOR;
                this.gridPosition.ColumnHeadersDefaultCellStyle.SelectionBackColor = DEFINE.BACK_COLOR;
                this.gridPosition.DefaultCellStyle.SelectionBackColor = DEFINE.MOUSEHOVER_COLOR;
                //this.gridPosition.RowHeadersDefaultCellStyle
                //this.gridPosition.DefaultCellStyle.SelectionForeColor = DEFINE.MOUSEHOVER_COLOR;
                //gridPosition.Columns["Axis"].DefaultCellStyle.BackColor = DEFINE.BACK_COLOR;

                gridPosition.Refresh();
                return true;
            }
            catch (Exception Desc)
            {
                //Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        private void gridPosition_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (gridPosition.CurrentCell == null)
                {
                    return;
                }
                int nRowIndex = gridPosition.CurrentCell.RowIndex;
                var oldValue = gridPosition[e.ColumnIndex, e.RowIndex].Value;

                if (oldValue == null)
                {
                    return;
                }
                try
                {
                    switch (e.ColumnIndex)
                    {
                        case 0:
                            Global.Data.PositionManager.Positions[nRowIndex].Axis = oldValue.ToString();
                            break;
                        case 1:
                            Global.Data.PositionManager.Positions[nRowIndex].Name = oldValue.ToString();
                            break;
                        case 2:
                            Global.Data.PositionManager.Positions[nRowIndex].Position = long.Parse(oldValue.ToString());
                            break;
                    }

                    gridPosition.Rows[nRowIndex].Frozen = true;
                    gridPosition.EnableHeadersVisualStyles = false;

                    gridPosition.Rows.Clear();

                    for (int i = 0; i < Global.Data.PositionManager.Positions.Count; i++)
                    {
                        switch (Global.Data.PositionManager.Positions[i].Axis)
                        {
                            case "Y":
                                string strDistancebymm = (Global.Data.PositionManager.Positions[i].Position / 1000.0D).ToString() + "mm";
                                gridPosition.Rows.Add(Global.Data.PositionManager.Positions[i].Axis,
                            Global.Data.PositionManager.Positions[i].Name, Global.Data.PositionManager.Positions[i].Position, strDistancebymm);
                                break;
                        }
                    }
                }
                catch
                {
                    switch (e.ColumnIndex)
                    {
                        case 0:
                            gridPosition[e.ColumnIndex, e.RowIndex].Value = Global.Data.PositionManager.Positions[nRowIndex].Axis;
                            break;
                        case 1:
                            gridPosition[e.ColumnIndex, e.RowIndex].Value = Global.Data.PositionManager.Positions[nRowIndex].Name;
                            break;
                        case 2:
                            gridPosition[e.ColumnIndex, e.RowIndex].Value = Global.Data.PositionManager.Positions[nRowIndex].Position;
                            break;

                    }
                    gridPosition.Rows[nRowIndex].Frozen = true;
                }
            }
            catch (Exception Desc)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }

        }


        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            Keys key = keyData & ~(Keys.Shift | Keys.Control);

            switch (key)
            {
                case Keys.Escape:
                    return true;
                case Keys.Q:

                    return true;
                case Keys.W:

                    return true;
                case Keys.E:

                    return true;
                case Keys.R:

                    return true;
                case Keys.T:

                    return true;
                case Keys.Y:

                    return true;
                case Keys.U:

                    return true;
                case Keys.I:

                    return true;
                case Keys.O:

                    return true;
                // Train
                case Keys.F1:
                    return true;
                // ROI
                case Keys.F2:

                    return true;
                // Modify
                case Keys.F3:

                    return true;
                // RUN
                case Keys.F5:

                    return true;
                case Keys.F7:
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }    
        
        private void FormTeachingMotion_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private bool InitEvent()
        {
            try
            {                
                Global.SeqVision.EventInspResult += OnInspResult;
                Global.Recipe.EventChagedRecipe += OnChangedRecipe;
                //ucPosition1.EventShowSetting += OnShowSettingForm;
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
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
                    InitUI();
                    InitControl();

                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
        }

        private void OnInspResult(object sender, InspResultArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        OnInspResult(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                
            }
        }

        private void OnShowSettingForm(object sender, CPropertyMotion e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        OnShowSettingForm(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {

            }
        }

        private bool InitUI()
        {
            try
            {
                CMotionManager manager = Global.Device.MOTION_AJIN;

                manager.LoadPositions(Global.Recipe.Name);

                //Work Picker #1
                ucPosition1.Init(manager.POS_LOADER_LAPPING_R, AxisMode.Rotate);
                ucPosition2.Init(manager.POS_MAIN_R, AxisMode.Rotate);
                ucPosition3.Init(manager.POS_BACK_AND_FORTH_Y, AxisMode.Position);
                ucPosition4.Init(manager.POS_LAPPING_R, AxisMode.Rotate);

                cbPositionMenu.Items.Clear();

                cbPositionMenu.Items.Add(manager.POS_LOADER_LAPPING_R.DESC);
                cbPositionMenu.Items.Add(manager.POS_MAIN_R.DESC);
                cbPositionMenu.Items.Add(manager.POS_BACK_AND_FORTH_Y.DESC);
                cbPositionMenu.Items.Add(manager.POS_LAPPING_R.DESC);

                cbPositionMenu.SelectedIndex = 0;                
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
     

        private void cbPositionMenu_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                btnStartHome.Enabled = false;

                ComboBox menu = (sender as ComboBox);
                switch (menu.SelectedItem.ToString())
                {
                    case DEFINE.Axis_0:
                        pos = Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R;
                        Axis = Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR;

                        lbInterlockRPM.Text = "RPM";
                        break;
                    case DEFINE.Axis_1:
                        pos = Global.Device.MOTION_AJIN.POS_MAIN_R;
                        Axis = Global.Device.MOTION_AJIN.AxisWork_MainR;
                        break;
                    case DEFINE.Axis_2:
                        pos = Global.Device.MOTION_AJIN.POS_BACK_AND_FORTH_Y;
                        Axis = Global.Device.MOTION_AJIN.AxisWork_BackAndForthY;

                        btnStartHome.Enabled = true;
                        break;
                    case DEFINE.Axis_3:
                        pos = Global.Device.MOTION_AJIN.POS_LAPPING_R;
                        Axis = Global.Device.MOTION_AJIN.AxisWork_LappingR;
                        break;
                }

                switch (menu.SelectedItem.ToString())
                {
                    case DEFINE.Axis_0:
                    case DEFINE.Axis_1:
                    case DEFINE.Axis_3:
                        lbJogSpeed.Text = "RPM";
                        lbInterlockRPM.Text = "RPM";
                        tbInterlockMaxSpeed.Texts = pos.RPM_SPEED_MAX.ToString();
                        tbInterlockMinSpeed.Texts = pos.RPM_SPEED_MIN.ToString();
                        break;
                    case DEFINE.Axis_2:
                        lbJogSpeed.Text = "SPEED (mm/s)";
                        lbInterlockRPM.Text = "SPEED (mm/s)";
                        tbInterlockMaxSpeed.Texts = pos.POSITION_SPEED_MAX.ToString();
                        tbInterlockMinSpeed.Texts = pos.POSITION_SPEED_MIN.ToString();
                        break;
                }

                this.Text = pos.NAME;
                Status = Axis.Status;
                Home = Axis.Home;
            
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
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

        private void timerView_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) { return; }                
                if (!Global.Device.MOTION_AJIN.IsOpen) { return; }

                //CAXM.AxmStatusSetReadServoLoadRatio(Axis.AxisNo, 0);
                double dRadio = 0;
                CAXM.AxmStatusReadServoLoadRatio(Axis.AxisNo, ref dRadio);

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

                lbActualPulse.Text = (Status.ActualPos).ToString("F3");
                //lbActualDistance.Text = (Status.ActualPos).ToString("F3") + "mm";
                lbCommandPulse.Text = Status.CommandPos.ToString("F3");
                //lbCommandDistance.Text = (Status.CommandPos).ToString("F3") + "mm";
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnStartHome_Click(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) CUtil.ShowMessageBox("ALARM", "SELECT THE AXIS", FormMessageBox.MESSAGEBOX_TYPE.Stop);
                else
                {
                    if (CUtil.ShowdialogMessageBox("CHECK", "DO YOU WANT TO SET THE HOME?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
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
                int JogSpeed = int.Parse(tbJogSpeed.Texts);

                double Accel = double.Parse(tbJogSpeedAccel.Texts);
                double Decel = double.Parse(tbJogSpeedDecel.Texts);
                double Interlock_MIN = double.Parse(tbInterlockMinSpeed.Texts);
                double Interlock_MAX = double.Parse(tbInterlockMaxSpeed.Texts);
                //double Position_Min = double.Parse(tbPositionMinSpeed.Texts);
                //double Position_Max = double.Parse(tbPositionMaxSpeed.Texts);

                //double CycleRPM = (double)RPM / 60;

                //double ResultRPM = (double)CycleRPM * 360000;

                string strIndex = "";

                if (sender is Button) strIndex = ((Button)sender).Name;

                switch (strIndex)
                {
                    case "btnJogPlus":                        
                        switch (cbPositionMenu.SelectedItem.ToString())
                        {
                            case DEFINE.Axis_0:
                            case DEFINE.Axis_1:
                            case DEFINE.Axis_3:
                                //Axis.SetUnitPerFulse(Axis.AxisNo, 1, (int)ResultRPM);
                                Axis.JogStartRPM(JogPosition.Plus, JogSpeed, Accel, Decel, Interlock_MAX, Interlock_MIN);
                                break;
                            case DEFINE.Axis_2:
                                Axis.JogStart(JogPosition.Plus, Accel, Decel, Interlock_MAX, Interlock_MIN, JogSpeed);
                                break;                                                           
                        }
                        
                        break;
                    case "btnJogMinus":
                        switch (cbPositionMenu.SelectedItem.ToString())
                        {
                            case DEFINE.Axis_0:
                            case DEFINE.Axis_1:
                            case DEFINE.Axis_3:
                                //Axis.SetUnitPerFulse(Axis.AxisNo, 1, (int)ResultRPM);
                                Axis.JogStartRPM(JogPosition.Minus, JogSpeed, Accel, Decel, Interlock_MAX, Interlock_MIN);
                                break;
                            case DEFINE.Axis_2:
                                Axis.JogStart(JogPosition.Minus, Accel, Decel, Interlock_MAX, Interlock_MIN, JogSpeed);
                                break;
                        }                        
                        break;
                }


            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowMessageBox("EXCEPTTION", "값을 제대로 입력하여 주십시오.", FormMessageBox.MESSAGEBOX_TYPE.Waring);
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

        private void btnMove_Click(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) return;
                if (Axis.Status.InMotion) return;

                if(Axis.AxisNo == 2)
                {
                    if (!Axis.HomeComplete)
                    {
                        CUtil.ShowdialogMessageBox("ALARM", "SET THE HOME FIRST", FormMessageBox.MESSAGEBOX_TYPE.Stop);
                        return;
                    }
                }           
                double dStepSpeed = double.Parse(tbStepSpeed.Texts);
                double dStepmm = double.Parse(tbStepmm.Texts);
                double dActualPos = Axis.Status.ActualPos;
                double dTargetPos = dActualPos + dStepmm;

                double Interlock_MIN = double.Parse(tbInterlockMinSpeed.Texts);
                double Interlock_MAX = double.Parse(tbInterlockMaxSpeed.Texts);

                if (dStepSpeed >= Interlock_MAX || dStepSpeed <= Interlock_MIN)
                {
                    CUtil.ShowdialogMessageBox("Speed Interlock!!", "현재 속도로 진행할 수 없습니다. 인터락 속도를 확인해 주십시오.", FormMessageBox.MESSAGEBOX_TYPE.Waring);
                    return;
                }

                Axis.SetMotionEndVelocity(Axis.AxisNo, dStepSpeed);
                Axis.Move(dTargetPos, dStepSpeed);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) return;
                if (Axis.Status.InMotion) return;

                if (Axis.AxisNo == 2)
                {
                    if (!Axis.HomeComplete)
                    {
                        CUtil.ShowMessageBox("ALARM", "SET THE HOME FIRST", FormMessageBox.MESSAGEBOX_TYPE.Stop);
                        return;
                    }
                }
          
                double dPulsePermm = Axis.PulsePermm;

                double dMoveSpeed = double.Parse(tbAbsoluteSpeed.Texts);
                double dTargetmm = double.Parse(tbAbsoluteTarget.Texts);

                double Interlock_MIN = double.Parse(tbInterlockMinSpeed.Texts);
                double Interlock_MAX = double.Parse(tbInterlockMaxSpeed.Texts);


                if (dMoveSpeed >= Interlock_MAX || dMoveSpeed <= Interlock_MIN)
                {
                    CUtil.ShowdialogMessageBox("Speed Interlock!!", "현재 속도로 진행할 수 없습니다. 인터락 속도를 확인해 주십시오.", FormMessageBox.MESSAGEBOX_TYPE.Waring);
                    return;
                }

                Axis.SetMotionEndVelocity(Axis.AxisNo, dMoveSpeed);
                Axis.Move(dTargetmm, dMoveSpeed);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void Move_Pos_Click(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) return;
                if (Axis.Status.InMotion) return;

                if (Axis.AxisNo == 2)
                {
                    if (!Axis.HomeComplete)
                    {
                        CUtil.ShowMessageBox("ALARM", "SET THE HOME FIRST", FormMessageBox.MESSAGEBOX_TYPE.Stop);
                        return;
                    }
                }

                double dPulsePermm = Axis.PulsePermm;

                double dMoveSpeed = double.Parse(tbAbsoluteSpeed.Texts);
                double dTargetmm = double.Parse(tbAbsoluteTarget.Texts);

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
                FormMessageBox SaveForm = new FormMessageBox("Save", "현재까지 설정된 값을 저장하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit);
                if (SaveForm.ShowDialog() == DialogResult.OK)
                {
                    switch (cbPositionMenu.SelectedItem.ToString())
                    {
                        case DEFINE.Axis_0:
                        case DEFINE.Axis_1:
                        case DEFINE.Axis_3:
                            pos.RPM_SPEED_MAX = double.Parse(tbInterlockMaxSpeed.Texts);
                            pos.RPM_SPEED_MIN = double.Parse(tbInterlockMinSpeed.Texts);
                            break;
                        case DEFINE.Axis_2:
                            pos.POSITION_SPEED_MAX = double.Parse(tbInterlockMaxSpeed.Texts);
                            pos.POSITION_SPEED_MIN = double.Parse(tbInterlockMinSpeed.Texts);
                            break;
                    }

                    pos.SaveConfig(Global.Recipe.Name);
                    Global.Recipe.LappingPitch = double.Parse(tbLappingPitch.Texts);
                    Global.Recipe.LappingAngle = double.Parse(tbAngle.Texts);
                    Global.Recipe.ReWinderPitch = double.Parse(tbRewinderPitch.Texts);
                    Global.Recipe.SaveConfig();
                }



                this.DialogResult = DialogResult.OK;
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnJogMinus_Click(object sender, EventArgs e)
        {

        }

        private void btnJogPlus_Click(object sender, EventArgs e)
        {

        }

        private void btnAxisAllStop_Click(object sender, EventArgs e)
        {
            try
            {                
                Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.JogStop();
                Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.EStop();
                Global.Device.MOTION_AJIN.AxisWork_MainR.JogStop();
                Global.Device.MOTION_AJIN.AxisWork_MainR.EStop();
                Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.JogStop();
                Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.EStop();
                Global.Device.MOTION_AJIN.AxisWork_LappingR.JogStop();
                Global.Device.MOTION_AJIN.AxisWork_LappingR.EStop();                

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnClearPos_Click(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) return;

                Axis.ClearActPos(Axis.AxisNo);
                Axis.ClearComPos(Axis.AxisNo);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnClearAllPos_Click(object sender, EventArgs e)
        {
            Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.ClearActPos(0);
            Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.ClearComPos(0);
            Global.Device.MOTION_AJIN.AxisWork_MainR.ClearActPos(1);
            Global.Device.MOTION_AJIN.AxisWork_MainR.ClearComPos(1);
            Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.ClearActPos(2);
            Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.ClearComPos(2);
            Global.Device.MOTION_AJIN.AxisWork_LappingR.ClearActPos(3);
            Global.Device.MOTION_AJIN.AxisWork_LappingR.ClearComPos(3);
        }

        private const int COL_AXIS = 0;
        private const int COL_NAME = 1;
        private const int COL_POS = 2;
        private int PositionClickRow = -1;

        private void btnGetCurrentPosition_Click(object sender, EventArgs e)
        {
            try
            {
                int nRowIndex = gridPosition.CurrentRow.Index;

                string strAxis = gridPosition[COL_AXIS, nRowIndex].Value.ToString();
                string strName = gridPosition[COL_NAME, nRowIndex].Value.ToString();
                string strPos = gridPosition[COL_POS, nRowIndex].Value.ToString();

                double dPos = 0;

                if (CUtil.ShowdialogMessageBox("GET POS", string.Format("DO YOU WANT TO GET POSITION OF CURRENT {0}?", strName), FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    switch (strAxis)
                    {
                        case "Y":
                            dPos = Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.Status.ActualPos;
                            break;
                    }

                    Global.Data.PositionManager.Positions[nRowIndex].Position = (long)dPos;

                    gridPosition.Rows.Clear();

                    for (int i = 0; i < Global.Data.PositionManager.Positions.Count; i++)
                    {
                        switch (Global.Data.PositionManager.Positions[i].Axis)
                        {
                            case "Y":
                                string strDistancebymm = (Global.Data.PositionManager.Positions[i].Position / 1000.0D).ToString() + "mm";
                                gridPosition.Rows.Add(Global.Data.PositionManager.Positions[i].Axis,
                                Global.Data.PositionManager.Positions[i].Name, Global.Data.PositionManager.Positions[i].Position, strDistancebymm);
                                break;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void btnSavePosition_Click(object sender, EventArgs e)
        {
            try
            {
                if (CUtil.ShowdialogMessageBox("Save Position", string.Format("Do you want to Save Position?"), FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    Global.Data.PositionManager.SaveConfig();
                }
            }
            catch (Exception Desc)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void btnMovePosition_Click(object sender, EventArgs e)
        {
            try
            {
                if (Axis == null) return;
                if (Axis.Status.InMotion) return;
                if (PositionClickRow == -1) { return; }

                if (Axis.AxisNo == 2)
                {
                    if (!Axis.HomeComplete)
                    {
                        CUtil.ShowMessageBox("ALARM", "SET THE HOME FIRST", FormMessageBox.MESSAGEBOX_TYPE.Stop);
                        return;
                    }

                    string strPos = gridPosition[COL_POS, PositionClickRow].Value.ToString();

                    double dTargetmm = double.Parse(strPos);

                    Axis.SetMotionEndVelocity(Axis.AxisNo, 100000);
                    Axis.Move(dTargetmm, 100000);
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void gridPosition_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            PositionClickRow = gridPosition.CurrentRow.Index;
        }

        private void btnAlarmClear_Click(object sender, EventArgs e)
        {
            try
            {
                Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.AlarmReset();                
                Global.Device.MOTION_AJIN.AxisWork_MainR.AlarmReset();                
                Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.AlarmReset();                
                Global.Device.MOTION_AJIN.AxisWork_LappingR.AlarmReset();                

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {

        }

        private void tbPress_KeyPress(object sender, KeyPressEventArgs e)
        {
            CUtil.txtInterval_KeyPress(sender, e);
        }

        private void btnSpeedExcute_Click(object sender, EventArgs e)
        {
            try
            {
                for(int i = 0; i < Global.Device.MOTION_AJIN.Axises.Count; i++)
                {
                    if(Global.Device.MOTION_AJIN.Axises.ElementAt(i).Value.Status.InMotion)
                    {
                        CUtil.ShowdialogMessageBox("ALARM", "현재 모션축이 움직이고 있습니다.", FormMessageBox.MESSAGEBOX_TYPE.Stop);
                        return;
                   }
                }

                FormMessageBox SaveForm = new FormMessageBox("Save", "현재까지 설정된 값을 저장하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit);
                if (SaveForm.ShowDialog() == DialogResult.OK)
                {
                    double ProductPitch = double.Parse(tbLappingPitch.Texts);
                    double ProductSpeed = double.Parse(tbProdudctSpeed.Texts) / 10;                    
                    double Radio = double.Parse(tbRadio.Texts);

                    // 1mm 간격으로 생산시 적용
                    double DefineRPM = ((double)95 / (double)ProductPitch);

                    // 1. 일단 주축하고 리와인더 축은 감가속 비율만큼 변환                
                    Global.Device.MOTION_AJIN.POS_MAIN_R.SPEED_RPM = ProductSpeed;
                    Global.Device.MOTION_AJIN.POS_MAIN_R.ACCEL_TIME = Math.Round(ProductSpeed / Radio, 2);
                    Global.Device.MOTION_AJIN.POS_MAIN_R.DECEL_TIME = Math.Round(ProductSpeed / Radio, 2);

                    Global.Device.MOTION_AJIN.POS_LAPPING_R.SPEED_RPM = ProductSpeed * 3;
                    Global.Device.MOTION_AJIN.POS_LAPPING_R.ACCEL_TIME = Math.Round((ProductSpeed * 3) / Radio, 2);
                    Global.Device.MOTION_AJIN.POS_LAPPING_R.DECEL_TIME = Math.Round((ProductSpeed * 3) / Radio, 2);

                    // 2. 랩핑 축을 비율 + 생산속도만큼 계산해서 넣어주기
                    Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R.SPEED_RPM = Math.Round(ProductSpeed * DefineRPM, 2);
                    Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R.ACCEL_TIME = Math.Round((ProductSpeed * DefineRPM) / Radio, 2);
                    Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R.DECEL_TIME = Math.Round((ProductSpeed * DefineRPM) / Radio, 2);

                    Global.Device.MOTION_AJIN.POS_MAIN_R.SaveConfig(Global.Recipe.Name);
                    Global.Device.MOTION_AJIN.POS_LAPPING_R.SaveConfig(Global.Recipe.Name);
                    Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R.SaveConfig(Global.Recipe.Name); Global.Recipe.LappingPitch = double.Parse(tbLappingPitch.Texts);

                    InitUI();
                    Global.Recipe.LappingPitch = double.Parse(tbLappingPitch.Texts);
                    Global.Recipe.LappingAngle = double.Parse(tbAngle.Texts);
                    Global.Recipe.ReWinderPitch = double.Parse(tbRewinderPitch.Texts);
                    Global.Recipe.Radio = Radio;
                    Global.Recipe.ProductSpeed = double.Parse(tbProdudctSpeed.Texts);

                    //Global.Recipe.BaseMetalThicknessMM = double.Parse(tbBaseMetalmm.Texts);
                    //Global.Recipe.TapeThicknessMM = double.Parse(tbTapeThicknessMM.Texts);
                    Global.Recipe.SaveConfig();

                    if(Global.System.EventChangedUi != null)
                    {
                        Global.System.EventChangedUi(null, null);
                    }
                }
                               

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}