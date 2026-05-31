using CodeMeter;
using ImageGlass;
using IntelligentFactory;
using MetroFramework.Controls;
using MetroFramework.Forms;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Vila.Win32;
using WeifenLuo.WinFormsUI.Docking;
using static KtemVisionSystem.CAXIS_AJIN;
using static KtemVisionSystem.CNodeAlarm;
using static KtemVisionSystem.CVision;

namespace KtemVisionSystem
{
    public partial class FormSideMotion : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;

        public CPropertyMotion pos = null;
        public CAXIS_AJIN Axis = null;

        public CStatusMotion Status = null;
        public CStatusMotionHome Home = null;

        #region Event Register        
        public EventHandler<EventArgs> EventUpdateUi;
        #endregion

        public FormSideMotion()
        {
            InitializeComponent();
        }

        private void FormTeachingVision_Load(object sender, EventArgs e)
        {
            InitEvent();
            InitProperty();

            Global.Device.CAMERAS[0].EventGrabEnd += OnGrabEnd;

            Global.Device.DIO_BD.DI_00_SWITCH_START.EventUpdateSignal += OnSwitchStart;
            Global.Device.DIO_BD.DI_01_SWITCH_STOP.EventUpdateSignal += OnSwitchStart;

            Global.Device.DIO_BD.On(Global.Device.DIO_BD.DO_08_LAMP_RED);
            //Global.Device.DIO_BD.On(Global.Device.DIO_BD.DO_01_SWITCH_LAMP_STOP);

            tgbVisionUse.Checked = Global.Recipe.UseVision;
            tgbThicknessAlaramUse.Checked = Global.Recipe.UseThicknessAlaram;
            tgbSaveImage.Checked = Global.ImageMgr.SaveImages.Property.UseSaveOK;
            tgbDeleteImage.Checked = Global.ImageMgr.DelImages.Property.UseDeleteImage;
        }

        private void OnSwitchStop(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnSwitchStop(sender, e);
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
                    if (sender is CSignal)
                    {
                        CSignal signal = sender as CSignal;

                        if (signal.IsDisplay)
                        {
                            signal.IsDisplay = false;
                            if (signal.IsOn)
                            {
                                OnClickOperation(btnOperation, null);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
        }

        private void OnSwitchStart(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnSwitchStart(sender, e);
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
                    if (sender is CSignal)
                    {
                        CSignal signal = sender as CSignal;

                        if (signal.IsDisplay)
                        {
                            try
                            {
                                int JogSpeed = int.Parse(tbJogSpeed.Texts);

                                double Accel = double.Parse(tbJogSpeedAccel.Texts);
                                double Decel = double.Parse(tbJogSpeedDecel.Texts);
                                double Interlock_MIN = Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R.RPM_SPEED_MIN;
                                double Interlock_MAX = Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R.RPM_SPEED_MAX;
                                string strIndex = "";

                                //if (sender is Button) strIndex = ((Button)sender).Name;

                                signal.IsDisplay = false;

                                if (Global.System.Mode == CSystem.MODE.AUTO) { return; }

                                if (signal.IsOn)
                                {
                                    switch (signal.Name)
                                    {
                                        case "DI_00_SWITCH_START":
                                            Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.JogStartRPM(JogPosition.Plus, JogSpeed, Accel, Decel, Interlock_MAX, Interlock_MIN);
                                            Global.Device.DIO_BD.On(Global.Device.DIO_BD.DO_00_SWITCH_LAMP_START);
                                            break;
                                        case "DI_01_SWITCH_STOP":
                                            Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.JogStartRPM(JogPosition.Minus, JogSpeed, Accel, Decel, Interlock_MAX, Interlock_MIN);
                                            Global.Device.DIO_BD.On(Global.Device.DIO_BD.DO_01_SWITCH_LAMP_STOP);                                            
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (signal.Name)
                                    {
                                        case "DI_00_SWITCH_START":
                                            Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.JogStop();
                                            Global.Device.DIO_BD.Off(Global.Device.DIO_BD.DO_00_SWITCH_LAMP_START);
                                            Global.Device.DIO_BD.Off(Global.Device.DIO_BD.DO_01_SWITCH_LAMP_STOP);
                                            break;
                                        case "DI_01_SWITCH_STOP":
                                            Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.JogStop();
                                            Global.Device.DIO_BD.Off(Global.Device.DIO_BD.DO_00_SWITCH_LAMP_START);
                                            Global.Device.DIO_BD.Off(Global.Device.DIO_BD.DO_01_SWITCH_LAMP_STOP);
                                            //Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.JogStartRPM(JogPosition.Minus, JogSpeed, Accel, Decel, Interlock_MAX, Interlock_MIN);
                                            //Global.Device.DIO_BD.On(Global.Device.DIO_BD.DO_01_SWITCH_LAMP_STOP);
                                            break;
                                    }

                                }


                            }
                            catch (Exception ex)
                            {
                                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                                CUtil.ShowMessageBox("EXCEPTTION", "값을 제대로 입력하여 주십시오.", FormMessageBox.MESSAGEBOX_TYPE.Waring);
                            }                     
                        }
                    }
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
        }



        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            Keys key = keyData & ~(Keys.Shift | Keys.Control);

            switch (key)
            {
                case Keys.Escape:
                    //if (CUtil.ShowMessageBox("Notice", "창을 닫으시겠습니까?"))
                    //{
                    //    this.DialogResult = DialogResult.Cancel;
                    //    this.Close();
                    //}
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
                EventUpdateUi += OnUpdateUi;
                EventUpdateUi(null, null);

                tbLotMinTapeDis.Texts = Global.Data.Lot.LOT_MIN_TAPE_DISTANCE_M.ToString();
                tbThicknessDelay.Texts = Global.Data.Lot.THICKNESS_DELAY_MINIT.ToString();

                Global.System.EventChangedUi += OnChangedUI;
                Global.System.EventChangedMode += OnChangedMode;
                Global.Recipe.EventChagedRecipe += OnChangedRecipe;
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
            return true;
        }

        private void OnChangedMode(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnChangedMode(sender, e);
                    }));
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            else
            {
                switch (Global.System.Mode)
                {
                    case CSystem.MODE.AUTO:
                        btnOperation.Image = Properties.Resources.stop_button_150;
                        btnOperation.Text = "STOP";
                        btnOperation.BackColor = DEFINE.MOUSEHOVER_COLOR;
                        //lbModeStatus.Text = "MODE: AUTO";
                        //pnSideMenu.Enabled = false;

                        btnGrab.Enabled = false;
                        btnLive.Enabled = false;
                        //btnStartHome.Enabled = false;

                        btnLotOpen.Enabled = false;
                        btnLotEnd.Enabled = false;
                        btnLotClear.Enabled = false;


                        Global.Sequence.StartThreadSeq();
                        StartLamp();
                        break;
                    case CSystem.MODE.READY:
                    case CSystem.MODE.ALARM:
                        btnOperation.Image = Properties.Resources.play_150;
                        btnOperation.Text = "START";
                        btnOperation.BackColor = DEFINE.COLOR_SECOND;
                        //lbModeStatus.Text = "MODE: MANUAML";
                        //pnSideMenu.Enabled = true;
                        //
                        btnGrab.Enabled = true;
                        btnLive.Enabled = true;
                        //btnStartHome.Enabled = true;

                        btnLotOpen.Enabled = true;
                        btnLotEnd.Enabled = true;
                        btnLotClear.Enabled = true;

                        Global.Sequence.StopThreadSeq();
                        Global.orthogonalY.StopThreadOrthogonalY();
                        StopLamp();
                        break;
                                           
                }
            }
        }

        private void OnChangedUI(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnChangedUI(sender, e);
                    }));
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            else
            {
                ucPosition1.Update();
                ucPosition2.Update();
                ucPosition3.Update();
                ucPosition4.Update();
            }
        }

        private void StartLamp()
        {
            try
            {
                Global.Device.DIO_BD.On(Global.Device.DIO_BD.DO_10_LAMP_GREEN);
                Global.Device.DIO_BD.On(Global.Device.DIO_BD.DO_00_SWITCH_LAMP_START);

                Global.Device.DIO_BD.Off(Global.Device.DIO_BD.DO_01_SWITCH_LAMP_STOP);
                Global.Device.DIO_BD.Off(Global.Device.DIO_BD.DO_08_LAMP_RED);
                Global.Device.DIO_BD.Off(Global.Device.DIO_BD.DO_09_LAMP_YELLOW);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void StopLamp()
        {
            try
            {
                Global.Device.DIO_BD.On(Global.Device.DIO_BD.DO_01_SWITCH_LAMP_STOP);
                Global.Device.DIO_BD.On(Global.Device.DIO_BD.DO_08_LAMP_RED);

                Global.Device.DIO_BD.Off(Global.Device.DIO_BD.DO_00_SWITCH_LAMP_START);
                Global.Device.DIO_BD.Off(Global.Device.DIO_BD.DO_09_LAMP_YELLOW);
                Global.Device.DIO_BD.Off(Global.Device.DIO_BD.DO_10_LAMP_GREEN);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private bool InitProperty()
        {
            try
            {
                //propertyGrid_Parameter.SelectedObject = Global.Data.PropertyVisionInsp;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void OnUpdateUi(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnUpdateUi(sender, e);
                    }));
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            else
            {
                InitUI();
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
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
                    tbLotMinTapeDis.Texts = Global.Data.Lot.LOT_MIN_TAPE_DISTANCE_M.ToString();
                    tbThicknessDelay.Texts = Global.Data.Lot.THICKNESS_DELAY_MINIT.ToString();

                    tgbVisionUse.Checked = Global.Recipe.UseVision;
                    tgbThicknessAlaramUse.Checked = Global.Recipe.UseThicknessAlaram;
                    if(Global.ImageMgr.SaveImages.Property == null)
                    {
                        Global.ImageMgr.SaveImages.Property = new PropertySaves();
                        Global.ImageMgr.SaveImages.Property.LoadConfig(Global.Recipe.Name);
                    }

                    if (Global.ImageMgr.DelImages.Property == null)
                    {
                        Global.ImageMgr.DelImages.Property = new PropertyDelete();
                        Global.ImageMgr.DelImages.Property.LoadConfig(Global.Recipe.Name);
                    }
                    tgbSaveImage.Checked = Global.ImageMgr.SaveImages.Property.UseSaveOK;
                    tgbDeleteImage.Checked = Global.ImageMgr.DelImages.Property.UseDeleteImage;
                    InitProperty();
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
        }

        private void cbPositionMenu_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //btnStartHome.Enabled = false;

                ComboBox menu = (sender as ComboBox);
                switch (menu.SelectedItem.ToString())
                {
                    case DEFINE.Axis_0:
                        pos = Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R;
                        Axis = Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR;
                        break;
                    case DEFINE.Axis_1:
                        pos = Global.Device.MOTION_AJIN.POS_MAIN_R;
                        Axis = Global.Device.MOTION_AJIN.AxisWork_MainR;
                        break;
                    case DEFINE.Axis_2:
                        pos = Global.Device.MOTION_AJIN.POS_BACK_AND_FORTH_Y;
                        Axis = Global.Device.MOTION_AJIN.AxisWork_BackAndForthY;

                        //btnStartHome.Enabled = true;
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
                        break;
                    case DEFINE.Axis_2:
                        lbJogSpeed.Text = "SPEED (mm/s)";
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

        private void OnGrabEnd(object sender, GrabEventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnGrabEnd(sender, e);
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
                    if (Global.System.m_strSelectedMenu == "MAIN" || Global.System.m_strSelectedMenu == "Main" ||
                        Global.System.m_strSelectedMenu == "DEVICE" || Global.System.m_strSelectedMenu == "MODEL")
                    {                        
                        //if (Global.System.Mode == CSystem.MODE.AUTO)
                        //{
                        //    Global.Data.GrabQueue.Enqueue(new CGrabBuffer(e.ImageGrab, 0));
                        //}

                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
        }

        private bool CheckStatusForAuto()
        {
            try
            {
                if (!Global.Data.Lot.IS_COMPLETE_LOT_OPEN) CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_LOT_OPEN_FAIL, "Lot 오픈을 해주세요.", "LOT OPEN", OnAlaramClear));

                if (!Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.HomeComplete) { CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_SYSTEM_HOME, "트래버스 홈을 잡아주세요.", "HOME", OnAlaramClear)); }

                if (!Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.Status.ServoOn) CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_SYSTEM_SERVO_POWER_AXIS_0, "헤드 SERVO OFF상태입니다.", "SERVO", OnAlaramClear));
                if (!Global.Device.MOTION_AJIN.AxisWork_MainR.Status.ServoOn) CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_SYSTEM_SERVO_POWER_AXIS_1, "캡스탄 SERVO OFF상태입니다.", "SERVO", OnAlaramClear));
                if (!Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.Status.ServoOn) CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_SYSTEM_SERVO_POWER_AXIS_2, "트래버스 SERVO OFF상태입니다.", "SERVO", OnAlaramClear));
                if (!Global.Device.MOTION_AJIN.AxisWork_LappingR.Status.ServoOn) CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_SYSTEM_SERVO_POWER_AXIS_3, "테이크업 SERVO OFF상태입니다.", "SERVO", OnAlaramClear));

                if (Global.Device.CAMERAS.Count > 0) { if (!Global.Device.CAMERAS[0].IsOpen) CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_DEVICE_CONNECTION, "카메라 연결을 확인해 주세요.", "시스템", OnAlaramClear)); }                               
                if (!Global.Device.DIO_BD.IsOpen) CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_DEVICE_CONNECTION, "I/O 보드 연결을 확인해 주세요.", "시스템", OnAlaramClear));
                if (!Global.Device.MOTION_AJIN.IsOpen) CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_DEVICE_CONNECTION, "모션보드 연결을 확인해 주세요.", "시스템", OnAlaramClear));
                if (!Global.Device.LIGHT.IsOpen) CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_DEVICE_CONNECTION, "조명 컨트롤러 연결을 확인해 주세요.", "시스템", OnAlaramClear));                

                if (CAlarm.Exists) return false;
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        private void OnAlaramClear(object sender, AlaramEventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnAlaramClear(sender, e);
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
                    switch(e.cNodeAlarm.Code)
                    {
                        case DEFINE.ALARM_SYSTEM_HOME:
                            if (CUtil.ShowdialogMessageBox("CHECK", "DO YOU WANT TO SET THE HOME?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                            {
                                Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.StartThreadHome();
                            }
                            break;
                        case DEFINE.ALARM_LOT_OPEN_FAIL:
                            FormViewer_LotOpen FrmViewer_LotOpen = new FormViewer_LotOpen();

                            if (CUtil.OpenCheckForm(FrmViewer_LotOpen))
                            {
                                FrmViewer_LotOpen.ShowDialog();
                            }
                            break;
                        case DEFINE.ALARM_SYSTEM_SERVO_POWER_AXIS_0:
                            Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.ServoOnOff(true);
                            break;
                        case DEFINE.ALARM_SYSTEM_SERVO_POWER_AXIS_1:
                            Global.Device.MOTION_AJIN.AxisWork_MainR.ServoOnOff(true);
                            break;
                        case DEFINE.ALARM_SYSTEM_SERVO_POWER_AXIS_2:
                            Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.ServoOnOff(true);
                            break;
                        case DEFINE.ALARM_SYSTEM_SERVO_POWER_AXIS_3:
                            Global.Device.MOTION_AJIN.AxisWork_LappingR.ServoOnOff(true);
                            break;
                    }

                    if (Global.System.m_strSelectedMenu == "MAIN" || Global.System.m_strSelectedMenu == "Main" ||
                        Global.System.m_strSelectedMenu == "DEVICE" || Global.System.m_strSelectedMenu == "MODEL")
                    {
                        //if (Global.System.Mode == CSystem.MODE.AUTO)
                        //{
                        //    Global.Data.GrabQueue.Enqueue(new CGrabBuffer(e.ImageGrab, 0));
                        //}

                        //GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
        }


        private void OnClickOperation(object sender, EventArgs e)
        {
            try
            {
                string strIndex = ((RJButton)sender).Text;

                switch (strIndex)
                {
                    case "AUTO RUN\r\nSTART":
                    case "AUTO START":
                    case "START":
                        if (!CheckStatusForAuto())
                        {
                            Global.System.Mode = CSystem.MODE.READY;                            
                            return;
                        }

                        if (CUtil.ShowdialogMessageBox("RUN", "생산을 시작하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                        {
                            Global.System.Mode = CSystem.MODE.AUTO;
                            btnLotOpen.Enabled = false;
                            btnLotEnd.Enabled = false;
                            btnLotClear.Enabled = false;
                              
                        }                       

                        break;
                    case "AUTO RUN\r\nSTOP":
                    case "AUTO STOP":
                    case "STOP":                        
                        Global.System.Mode = CSystem.MODE.READY;
                        btnLotOpen.Enabled = true;
                        btnLotEnd.Enabled = true;
                        btnLotClear.Enabled = true;
                        break;
                }
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
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

         private void btnJogPlus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (Axis == null) return;                
                int JogSpeed = int.Parse(tbJogSpeed.Texts);

                double Accel = double.Parse(tbJogSpeedAccel.Texts);
                double Decel = double.Parse(tbJogSpeedDecel.Texts);
                double Interlock_MIN = 0;
                double Interlock_MAX = 0;
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
                                Interlock_MIN = pos.RPM_SPEED_MIN;
                                Interlock_MAX = pos.RPM_SPEED_MAX;
                                //Axis.SetUnitPerFulse(Axis.AxisNo, 1, (int)ResultRPM);
                                Axis.JogStartRPM(JogPosition.Plus, JogSpeed, Accel, Decel, Interlock_MAX, Interlock_MIN);
                                break;                            
                            case DEFINE.Axis_2:
                                Interlock_MIN = pos.POSITION_SPEED_MIN;
                                Interlock_MAX = pos.POSITION_SPEED_MAX;
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
                                Interlock_MIN = pos.RPM_SPEED_MIN;
                                Interlock_MAX = pos.RPM_SPEED_MAX;
                                Axis.JogStartRPM(JogPosition.Minus, JogSpeed, Accel, Decel, Interlock_MAX, Interlock_MIN);
                                break;
                            case DEFINE.Axis_2:
                                Interlock_MIN = pos.POSITION_SPEED_MIN;
                                Interlock_MAX = pos.POSITION_SPEED_MAX;
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

        private void OnClickCameraOperation(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is RJButton)) return;

                string strIndex = (sender as RJButton).Text;

                switch (strIndex)
                {
                    case DEFINE.Grab:
                        if (!Global.Device.CAMERAS[0].IsOpen) return;
                        Global.Device.CAMERAS[0].Grab(false);
                        break;
                    case DEFINE.Live:
                        if (!Global.Device.CAMERAS[0].IsOpen) return;
                        (sender as RJButton).Text = "LIVE STOP";
                        Global.Device.CAMERAS[0].Live(true);
                        break;
                    case DEFINE.Live_Stop:
                        if (!Global.Device.CAMERAS[0].IsOpen) return;
                        (sender as RJButton).Text = "LIVE";
                        Global.Device.CAMERAS[0].Live(false);
                        break;
                    case DEFINE.Image_Load:
                        try
                        {
                            //cogDisplay_Source.InteractiveGraphics.Clear();

                            OpenFileDialog ofd = new OpenFileDialog();
                            ofd.Title = "Image Load";
                            ofd.Filter = "Image File (*.png, *.jpg, *.gif, *.bmp) | *.png; *.jpg; *.gif; *.bmp; | 모든 파일 (*.*) | *.*";

                            if (ofd.ShowDialog() == DialogResult.OK)
                            {
                                string strFilePath = ofd.FileName;

                                if (File.Exists(strFilePath))
                                {

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                }

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
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

        private void btnScreenCapture_Click(object sender, EventArgs e)
        {
            try
            {
                int w = Screen.PrimaryScreen.Bounds.Width;
                int h = Screen.PrimaryScreen.Bounds.Height;

                System.Drawing.Size s = new System.Drawing.Size(w, h);
                Bitmap b = new Bitmap(w, h);
                Graphics g = Graphics.FromImage(b);

                g.CopyFromScreen(0, 0, 0, 0, s);

                string strSavePath = $"{Application.StartupPath}\\CAPTURE\\{this.Text}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.jpeg";

                b.Save(strSavePath);

                CLogger.Add(LOG.NORMAL, "저장 경로 : {0}", strSavePath);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnScreenCapture_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    Open_DropdownMenu(ddmCapture, sender);

                    //ContextMenuStrip m = new ContextMenuStrip();
                    //{
                    //    var menuItem = new ToolStripMenuItem("Open Capture Folder");                        
                    //    m.Items.Add(menuItem);
                    //}

                    //m.ItemClicked += new ToolStripItemClickedEventHandler(CaptureClicked);
                    //m.Show(btnScreenCapture, e.X, e.Y);
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void CaptureClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                switch (e.ClickedItem.Text)
                {
                    case "Show Folder":
                        Process.Start(Application.StartupPath + "\\CAPTURE");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void Open_DropdownMenu(RJDropdownMenu dropdownMenu, object sender)
        {
            Control control = (Control)sender;
            dropdownMenu.VisibleChanged += new EventHandler((sender2, ev)
              => DropdownMenu_VisibleChanged(sender2, ev, control));
            dropdownMenu.ItemClicked += new ToolStripItemClickedEventHandler(CaptureClicked);
            dropdownMenu.Show(control, 0, control.Height);
        }

        private void DropdownMenu_VisibleChanged(object sender, EventArgs e, Control ctrl)
        {
            RJDropdownMenu dropdownMenu = (RJDropdownMenu)sender;
            if (!DesignMode)
            {
                if (dropdownMenu.Visible)
                    ctrl.BackColor = DEFINE.MOUSEHOVER_COLOR;
                else ctrl.BackColor = System.Drawing.Color.FromArgb(51, 51, 76);
            }
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Global.Device.LIGHT.IsOpen) { btnStatusLIGHT.BackColor = System.Drawing.Color.Green; }
                else { btnStatusLIGHT.BackColor = System.Drawing.Color.Red; }

                if (Global.Device.MOTION_AJIN.IsOpen) { btnStatusMotion.BackColor = System.Drawing.Color.Green; }
                else { btnStatusMotion.BackColor = System.Drawing.Color.Red; }

                if (Global.Device.DIO_BD.IsOpen) { btnStatusIO.BackColor = System.Drawing.Color.Green; }
                else { btnStatusIO.BackColor = System.Drawing.Color.Red; }

                if (Global.Device.CAMERAS[0].IsOpen) { btnStatusCAM.BackColor = System.Drawing.Color.Green; }
                else { btnStatusCAM.BackColor = System.Drawing.Color.Red; }

                if (CGlobal.Inst.Device.MOTION_AJIN.Axises.Count == 0) { return; }

                // 테이프
                //double TapeTotal_mm = (double)(0.0166666666666667 * CGlobal.Inst.Device.MOTION_AJIN.Axises[0].ActualPos);
                double TapeTotal_mm = (double)(0.0166666666666667 * CGlobal.Inst.Device.MOTION_AJIN.Axises[0].Status.ActualPos);                
                double TapeTotal_m = (Math.Pow((Math.Pow(CGlobal.Inst.Recipe.LappingPitch, 2) + Math.Pow(((CGlobal.Inst.Data.Lot.BASEMETAL_T_MM+ CGlobal.Inst.Data.Lot.TAPE_T_MM) * 3.14), 2)), 0.5) * TapeTotal_mm) / 1000;

                // 모재
                // mm단위임
                //double BaseMetalTotal_mm = (double)(1.676666666666667 * Global.Device.MOTION_AJIN.Axises[1].ActualPos);
                double BaseMetalTotal_mm = (double)(1.676666666666667 * Global.Device.MOTION_AJIN.Axises[1].Status.ActualPos);
                double BaseMetal_m = BaseMetalTotal_mm / 1000;
                // m단위로 변환

                lbLotID.Text = Global.Data.Lot.LOT_NO;
                lbStartTime.Text = Global.Data.Lot.OPEN_TIME;
                lbTakeUpStartDelDis.Text = Global.Data.Lot.TAKEUP_START_DEL_DISTANCE_M.ToString("F3")+"M";                
                lbTapeThicknessMM.Text = Global.Data.Lot.TAPE_T_MM.ToString();
                lbBaseMetalmm.Text = Global.Data.Lot.BASEMETAL_T_MM.ToString();
                lbTapeType.Text = Global.Data.Lot.TAPE_TYPE.ToString();

                // 초기 생산시 감싸지지 않은 부분만큼은 생산한거로 보지 않는다.
                // m단위                
                double TakeUp_Level = 0.0;
                double TakeUp_Usage = 0.0;
                //double BaseMetal_Level = 0.0;
                //double BaseMetal_Usage = 0.0;
                //double Tape_Level = 0.0;
                //double Tape_Usage = 0.0;

                TakeUp_Usage = BaseMetal_m - Global.Data.Lot.TAKEUP_START_DEL_DISTANCE_M;

                TakeUp_Level = Global.Data.Lot.TAKEUP_DISTANCE_M - TakeUp_Usage;



                lbTakeUpUsage.Text = TakeUp_Usage.ToString("F3") + "M";
                lbBaseMetalUsage.Text = BaseMetal_m.ToString("F3") + "M";
                lbTapeUsage.Text = TapeTotal_m.ToString("F3") + "M";

                lbTakeUpLevel.Text = TakeUp_Level.ToString("F3") + "M";                                
                lbBaseMetalLevel.Text = (Global.Data.Lot.BASE_METAL_DISTANCE_M - BaseMetal_m).ToString("F3") + "M";                
                lbTapeLevel.Text = (Global.Data.Lot.TAPE_DISTANCE_M - TapeTotal_m).ToString("F3") + "M";

                // 알람
                if (Global.Data.Lot.THICKNESS_DELAY_TIME.Elapsed.TotalSeconds < Global.Data.Lot.THICKNESS_DELAY_MINIT) { lbThicknessDelayTime.Text = Global.Data.Lot.THICKNESS_DELAY_TIME.Elapsed.ToString(); }
                else { Global.Data.Lot.USE_THICKNESS_ALARM = true; }

                if (Global.System.Mode == CSystem.MODE.AUTO)
                {
                    if (Global.Data.Lot.LOT_MIN_TAPE_DISTANCE_M > (Global.Data.Lot.TAPE_DISTANCE_M - TapeTotal_m)) { CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_테이프부족, "테이프 부족 알람", "테이프를 교체하시기 바랍니다.")); }                    
                    if (Global.Data.Lot.BASE_METAL_DISTANCE_M - BaseMetal_m < 0) { CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_모재부족, "모재 부족 알람", "모재를 교체하시기 바랍니다.")); }                    
                    if (TakeUp_Level < 0)
                    {
                        OnClickOperation(btnOperation, null);
                        CUtil.ShowdialogMessageBox("LOT END", "테이크업를 교체하시기 바랍니다.", FormMessageBox.MESSAGEBOX_TYPE.Info);                        
                        //CAlarm.Add(new CNodeAlarm(DEFINE.ALARM_테이크업교체, "테이크업 교체 알람", "테이크업 교체하시기 바랍니다.")); 
                    }                    
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void tgbSaveImage_CheckedChanged(object sender, EventArgs e)
        {
            RJToggleButton pt = (RJToggleButton)sender;

            if (pt.Checked)
            {
                Global.ImageMgr.SaveImages.Property.UseSaveOK = true;
                Global.ImageMgr.SaveImages.Property.UseSaveNG = true;
            }
            else
            {
                Global.ImageMgr.SaveImages.Property.UseSaveOK = false;
                Global.ImageMgr.SaveImages.Property.UseSaveNG = false;
            }

            Global.ImageMgr.SaveImages.Property.SaveConfig(Global.Recipe.Name);
        }

        private void tgbAllChannle_CheckedChanged(object sender, EventArgs e)
        {
            RJToggleButton pt = (RJToggleButton)sender;

            if (pt.Checked) { Global.Recipe.UseVision = true; }
            else { Global.Recipe.UseVision = false; }

            Global.ImageMgr.SaveImages.Property.SaveConfig(Global.Recipe.Name);
        }

        private void tgbDeleteImage_CheckedChanged(object sender, EventArgs e)
        {
            RJToggleButton pt = (RJToggleButton)sender;

            if (pt.Checked) { Global.ImageMgr.DelImages.Property.UseDeleteImage = true; }
            else { Global.ImageMgr.DelImages.Property.UseDeleteImage = false; }           

            Global.ImageMgr.DelImages.Property.SaveConfig(Global.Recipe.Name);
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void btnLotOpen_Click(object sender, EventArgs e)
        {
            try
            {

                FormViewer_LotOpen FrmViewer_LotOpen = new FormViewer_LotOpen();

                if (CUtil.OpenCheckForm(FrmViewer_LotOpen))
                {
                    FrmViewer_LotOpen.Show();
                }

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void tbPress_KeyPress(object sender, KeyPressEventArgs e)
        {
            CUtil.txtInterval_KeyPress(sender, e);
        }

        private void btnLotEnd_Click(object sender, EventArgs e)
        {
            FormMessageBox SaveForm = new FormMessageBox("LOT END", "생산을 종료하겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit);
            if (SaveForm.ShowDialog() == DialogResult.OK)
            {
                Global.Data.Lot.LotEnd();
                Global.Data.Lot.IS_COMPLETE_LOT_OPEN = false;
                Global.Data.Lot.USE_THICKNESS_ALARM = false;
                Global.Data.Lot.THICKNESS_DELAY_TIME.Reset();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Global.Data.Lot.LOT_MIN_TAPE_DISTANCE_M = double.Parse(tbLotMinTapeDis.Texts);
                Global.Data.Lot.THICKNESS_DELAY_MINIT = double.Parse(tbThicknessDelay.Texts);
                Global.Data.Lot.SaveConfig(Global.Recipe.Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void timerAlarm_Tick(object sender, EventArgs e)
        {

        }

        private void tgbThicknessAlaramUse_CheckedChanged(object sender, EventArgs e)
        {
            RJToggleButton pt = (RJToggleButton)sender;

            if (pt.Checked) { Global.Recipe.UseThicknessAlaram = true; }
            else { Global.Recipe.UseThicknessAlaram = false; }

            Global.Recipe.SaveConfig();
        }

        private void rjToggleButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void timerLotSave_Tick(object sender, EventArgs e)
        {
            if(CGlobal.Inst.Data.Lot.IS_COMPLETE_LOT_OPEN)
            {
                // 거리값 가지고 있어야 함
                Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R.ACTUAL_POS =  Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.ActualPos;
                Global.Device.MOTION_AJIN.POS_MAIN_R.ACTUAL_POS = Global.Device.MOTION_AJIN.AxisWork_MainR.ActualPos;
                Global.Device.MOTION_AJIN.POS_BACK_AND_FORTH_Y.ACTUAL_POS = Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.ActualPos;
                Global.Device.MOTION_AJIN.POS_LAPPING_R.ACTUAL_POS = Global.Device.MOTION_AJIN.AxisWork_LappingR.ActualPos;

                Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R.ACTUAL_POS_TEMP = Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.ActualPos_Temp;
                Global.Device.MOTION_AJIN.POS_MAIN_R.ACTUAL_POS_TEMP = Global.Device.MOTION_AJIN.AxisWork_MainR.ActualPos_Temp;
                Global.Device.MOTION_AJIN.POS_BACK_AND_FORTH_Y.ACTUAL_POS_TEMP = Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.ActualPos_Temp;
                Global.Device.MOTION_AJIN.POS_LAPPING_R.ACTUAL_POS_TEMP = Global.Device.MOTION_AJIN.AxisWork_LappingR.ActualPos_Temp;


                Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R.ACTUAL_POS_Pre = Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.ActualPos_Pre;
                Global.Device.MOTION_AJIN.POS_MAIN_R.ACTUAL_POS_Pre = Global.Device.MOTION_AJIN.AxisWork_MainR.ActualPos_Pre;
                Global.Device.MOTION_AJIN.POS_BACK_AND_FORTH_Y.ACTUAL_POS_Pre = Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.ActualPos_Pre;
                Global.Device.MOTION_AJIN.POS_LAPPING_R.ACTUAL_POS_Pre = Global.Device.MOTION_AJIN.AxisWork_LappingR.ActualPos_Pre;

                Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R.COMMAND_POS = Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.Status.CommandPos;
                Global.Device.MOTION_AJIN.POS_MAIN_R.COMMAND_POS = Global.Device.MOTION_AJIN.AxisWork_MainR.Status.CommandPos;
                Global.Device.MOTION_AJIN.POS_BACK_AND_FORTH_Y.COMMAND_POS = Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.Status.CommandPos;
                Global.Device.MOTION_AJIN.POS_LAPPING_R.COMMAND_POS = Global.Device.MOTION_AJIN.AxisWork_LappingR.Status.CommandPos;

                Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R.COMMAND_POS_TEMP = Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.CommanPos_Temp;
                Global.Device.MOTION_AJIN.POS_MAIN_R.COMMAND_POS_TEMP = Global.Device.MOTION_AJIN.AxisWork_MainR.CommanPos_Temp;
                Global.Device.MOTION_AJIN.POS_BACK_AND_FORTH_Y.COMMAND_POS_TEMP = Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.CommanPos_Temp;
                Global.Device.MOTION_AJIN.POS_LAPPING_R.COMMAND_POS_TEMP = Global.Device.MOTION_AJIN.AxisWork_LappingR.CommanPos_Temp;

                Global.Device.MOTION_AJIN.POS_LOADER_LAPPING_R.COMMAND_POS_Pre = Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.CommanPos_Pre;
                Global.Device.MOTION_AJIN.POS_MAIN_R.COMMAND_POS_Pre = Global.Device.MOTION_AJIN.AxisWork_MainR.CommanPos_Pre;
                Global.Device.MOTION_AJIN.POS_BACK_AND_FORTH_Y.COMMAND_POS_Pre = Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.CommanPos_Pre;
                Global.Device.MOTION_AJIN.POS_LAPPING_R.COMMAND_POS_Pre = Global.Device.MOTION_AJIN.AxisWork_LappingR.CommanPos_Pre;

                for (int i = 0; i < Global.Device.MOTION_AJIN.Positions.Count; i++)
                {
                    Global.Device.MOTION_AJIN.Positions.ElementAt(i).Value.SaveConfig(Global.Recipe.Name);
                }

                SaveCSV_Running(Global.Data.Lot.LOT_NO, lbBaseMetalUsage.Text, lbTapeUsage.Text, lbTakeUpUsage.Text, Global.Data.Lot.TAPE_T_MM.ToString(), Global.Data.Lot.TAPE_TYPE);
            }
        }

        public void SaveCSV_Running(string LOTID, string UseBaseLine, string UseTape, string UseTakeUp, string Thickness, DEFINE.TapeType Type)
        {
            try
            {
                CUtil.InitDirectory("CSV");
                string strPath = "";

                strPath = CUtil.SaveLotIDPath() + $"{LOTID}_{DateTime.Now.ToString("yyyy-MM-dd")}_{Thickness}_{Type.ToString()}.csv";
                if (!File.Exists(strPath))
                {
                    //string strHeader = "No,Head No,CELL ID,Max,Min,DateTime";
                    string strHeader = "No,LOT ID,모재 사용량,테이프 사용량,테이크업 사용량,모재 두께,테이프 종류";

                    strHeader += "DateTime";

                    File.AppendAllText(strPath, strHeader + "\r\n", Encoding.UTF8);
                }

                StringBuilder sb = new StringBuilder();

                // 현재 csv ROW 개수를 가져옴
                int nLineCount = File.ReadAllLines(strPath).Length;
                sb.Append(string.Format("{0},", nLineCount));
                sb.Append(string.Format("{0},", LOTID));
                sb.Append(string.Format("{0},", UseBaseLine));
                sb.Append(string.Format("{0},", UseTape));
                sb.Append(string.Format("{0},", UseTakeUp));
                sb.Append(string.Format("{0},", Thickness));
                sb.Append(string.Format("{0},", Type.ToString()));
                sb.Append(string.Format("{0},", DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss")));
                File.AppendAllText(strPath, sb.ToString() + "\r\n", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnLotClear_Click(object sender, EventArgs e)
        {
            try
            {
                FormMessageBox SaveForm = new FormMessageBox("LOT 초기화", "생산 데이터를 초기화 하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit);
                if (SaveForm.ShowDialog() == DialogResult.OK)
                {
                    Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.ClearActPos(0);
                    Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.ActualPos_Temp = 0;
                    Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.CommanPos_Temp = 0;
                    Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.ActualPos_Pre = 0;
                    Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.CommanPos_Pre = 0;
                    Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.ClearComPos(0);
                    Global.Device.MOTION_AJIN.AxisWork_MainR.ActualPos_Temp = 0;
                    Global.Device.MOTION_AJIN.AxisWork_MainR.CommanPos_Temp = 0;
                    Global.Device.MOTION_AJIN.AxisWork_MainR.ActualPos_Pre = 0;
                    Global.Device.MOTION_AJIN.AxisWork_MainR.CommanPos_Pre = 0;
                    Global.Device.MOTION_AJIN.AxisWork_MainR.ClearActPos(1);
                    Global.Device.MOTION_AJIN.AxisWork_MainR.ClearComPos(1);
                    Global.Device.MOTION_AJIN.AxisWork_LappingR.ActualPos_Temp = 0;
                    Global.Device.MOTION_AJIN.AxisWork_LappingR.CommanPos_Temp = 0;
                    Global.Device.MOTION_AJIN.AxisWork_LappingR.ActualPos_Pre = 0;
                    Global.Device.MOTION_AJIN.AxisWork_LappingR.CommanPos_Pre = 0;
                    Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.ClearActPos(2);
                    Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.ClearComPos(2);
                    Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.ActualPos_Pre = 0;
                    Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.CommanPos_Pre = 0;
                    Global.Device.MOTION_AJIN.AxisWork_LappingR.ClearActPos(3);
                    Global.Device.MOTION_AJIN.AxisWork_LappingR.ClearComPos(3);

                    for (int i = 0; i < Global.Device.MOTION_AJIN.Positions.Count; i++)
                    {
                        Global.Device.MOTION_AJIN.Positions.ElementAt(i).Value.ACTUAL_POS = 0;
                        Global.Device.MOTION_AJIN.Positions.ElementAt(i).Value.COMMAND_POS = 0;
                        Global.Device.MOTION_AJIN.Positions.ElementAt(i).Value.ACTUAL_POS_TEMP = 0;
                        Global.Device.MOTION_AJIN.Positions.ElementAt(i).Value.COMMAND_POS_TEMP = 0;
                        Global.Device.MOTION_AJIN.Positions.ElementAt(i).Value.SaveConfig(Global.Recipe.Name);
                    }

                    Global.Data.FirstForword = true;
                    Global.Data.FirstBackWord = false;
                    Global.Device.MOTION_AJIN.POS_BACK_AND_FORTH_Y.PreviousPos = 0;
                    Global.Device.MOTION_AJIN.POS_BACK_AND_FORTH_Y.SaveConfig(Global.Recipe.Name);

                    Global.Data.Lot.THICKNESS_DELAY_TIME.Reset();
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }           
        }
    }
}