using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using MetroFramework.Forms;
using MetroFramework.Controls;
using System.Collections.Generic;
using KtemVisionSystem;

namespace IntelligentFactory
{
    public partial class FormViewer_LotOpen : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;

        private const int DGV_NO = 0;
        private const int DGV_ID = 1;
        private const int DGV_STATUS = 2;

        public EventHandler<EventArgs> EventUpdateStatus;
        public FormViewer_LotOpen()
        {
            InitializeComponent();
        }

        private void FormViewer_LotOpen_Load(object sender, EventArgs e)
        {
            InitUI();

            timerView.Enabled = true;

            Global.System.EventUpdateStyle += OnChangeStyle;

            this.KeyPreview = true;


            //cbTapeMenu.Items.Add(DEFINE.TapeType.동);
            //cbTapeMenu.Items.Add(DEFINE.TapeType.은);
            //cbTapeMenu.Items.Add(DEFINE.TapeType.금);

            //tbLotWorker.Texts = Global.Recipe.Data.LOT.WORKER; 
            //tbLotID.Texts = Global.Recipe.Data.LOT.LOT_NO;

            //tbBaseMaterialDis.Texts = Global.Recipe.Data.LOT.BASE_METAL_DISTANCE_M.ToString();
            //tbTapeDis.Texts = Global.Recipe.Data.LOT.TAPE_DISTANCE_M.ToString();
            //tbTakeUpDis.Texts = Global.Recipe.Data.LOT.TAKEUP_DISTANCE_M.ToString();
            //tbTakeUpStartDelDis.Texts = Global.Recipe.Data.LOT.TAKEUP_START_DEL_DISTANCE_M.ToString();

            //tbTapeThicknessMM.Texts = Global.Recipe.Data.LOT.TAPE_T_MM.ToString();
            //tbBaseMetalmm.Texts = Global.Recipe.Data.LOT.BASEMETAL_T_MM.ToString();
            //cbTapeMenu.Texts = Global.Recipe.Data.LOT.TAPE_TYPE.ToString();

            //lbLotOpenTime.Text = Global.Recipe.Data.LOT.OPEN_TIME;
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
               // tbLotTrayRows.Text = Global.iData.PropertyTrayLoading.ROWS.ToString();
               // tbLotTrayCols.Text = Global.iData.PropertyTrayLoading.COLUMNS.ToString();

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
                //if(Global.iDevice.DIO_BD.DI_WORK_LOADING_RAIL_LOADED_TRAY_DETECT.IsOn)
                //{
                //    lbStatusLoaderEmpty.BackColor = DEFINE.COLOR_TEAL;
                //    lbStatusLoaderEmpty.Text = "EXIST";
                //}
                //else
                //{
                //    lbStatusLoaderEmpty.BackColor = DEFINE.COLOR_RED;
                //    lbStatusLoaderEmpty.Text = "EMPTY";
                //}
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSaveZigElevatorBufferCount_Click(object sender, EventArgs e)
        {
            try
            {   
                //Global.iData.PropertyBufferPitch.SaveConfig(Global.System.Recipe.Name);
                //Global.iData.PropertyBufferPitch.LoadConfig(Global.System.Recipe.Name);
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }            
        }

        private void btnLotLastLoad_Click(object sender, EventArgs e)
        {
            try
            {
                //tbLotWorker.Text = Global.iData.PropertyLot.WORKER;
                //tbLotNo.Text = Global.iData.PropertyLot.LOT_NO;
                //tbLotMetalTrayID.Text = Global.iData.PropertyLot.METAL_TRAY_ID;
                //tbLotTopCoverID.Text = Global.iData.PropertyLot.TOP_COVER_ID;

                //tbLotDeviceCount.Text = Global.iData.PropertyLot.DEVICE_COUNT.ToString();
                //tbLotTrayRows.Text = Global.iData.PropertyLot.ROWS.ToString();
                //tbLotTrayCols.Text = Global.iData.PropertyLot.COLUMNS.ToString();

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnLotClear_Click(object sender, EventArgs e)
        {
            try
            {
                //tbLotWorker.Text = "";
                //tbLotID.Text = "";
                //tbLotMetalTrayID.Text = "";
                //tbLotTopCoverID.Text = "";

                //tbLotDeviceCount.Text = "";
                //tbLotTrayRows.Text = "";
                //tbLotTrayCols.Text = "";

                //lbLotOpenTime.Text = "";

                //IGlobal.Instance.iData.SEQ_DATA.IS_LOT_END = true;
                //IGlobal.Instance.iData.SEQ_DATA.IS_COMPLETE_LOT_OPEN = false;

                pnLotOpen.Enabled = true;

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnLotOpen_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!Global.iDevice.DIO_BD.DI_WORK_LOADING_RAIL_LOADED_TRAY_DETECT.IsOn)
                //{
                //    CUtil.ShowMessageBox("ALARM", "BUFFER IS EMPTY");
                //    return;
                //}

                //string strLotNo = tbLotNo.Text;
                //if(Global.iData.PropertyLot.LOT_NO != strLotNo)
                //{
                //    if(Global.iData.SEQ_DATA.WorkDoneQueue.Count > 0)
                //    {
                //        CUtil.ShowMessageBox("ALARM", "PLEASE REJECT THE WORK DONE BUFFER");
                //        return;
                //    }
                //}

                //if (CUtil.ShowMessageBox("LOT_OPEN", "DO YOU WANT TO LOT OPEN?", FormMessageBox.MESSAGEBOX_TYPE.btnLotOpen))
                //{
                //    Global.iData.SEQ_DATA.IS_WORK_OUT_UNLOADING = false;
                //    Global.iData.SEQ_DATA.IS_WORK_OUT_LOADING = false;

                //    Global.iData.SEQ_DATA.REQ_JEDEC_TO_RAIL = false;
                //    Global.iData.SEQ_DATA.REQ_JEDEC_TO_BUFFER = false;

                //    Global.iData.SEQ_DATA.REQ_RECOVERY = false;

                //    Global.iData.SEQ_DATA.REQ_START_LOADING_VISION = false;
                //    Global.iData.SEQ_DATA.REQ_START_UNLOADING_VISION = false;

                //    Global.iData.SEQ_DATA.REQ_UNLOADING_START = false;
                //    Global.iData.SEQ_DATA.INSPECTED_DEVICE_COUNT = 0;

                //    Global.SeqWorkPicker.SetStep(CSeqWorkPicker.STEP_SEQ.IDLE);

                //    Global.iData.PropertyLot.OPEN_TIME = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                //    Global.iData.PropertyLot.WORKER = tbLotWorker.Text;
                //    Global.iData.PropertyLot.LOT_NO = (tbLotNo.Text).Replace(" ", "");

                //    Global.iData.PropertyLot.METAL_TRAY_ID = (tbLotMetalTrayID.Text).Replace(" ", "");
                //    Global.iData.PropertyLot.TOP_COVER_ID = (tbLotTopCoverID.Text).Replace(" ","");

                //    Global.iData.PropertyLot.DEVICE_COUNT = int.Parse(tbLotDeviceCount.Text);
                //    Global.iData.PropertyLot.ROWS = int.Parse(tbLotTrayRows.Text);
                //    Global.iData.PropertyLot.COLUMNS = int.Parse(tbLotTrayCols.Text);

                //    Global.iData.PropertyLot.PARTIAL_COUNT = Global.iData.PropertyLot.DEVICE_COUNT % (Global.iData.PropertyLot.ROWS * Global.iData.PropertyLot.COLUMNS);

                //    lbLotOpenTime.Text = Global.iData.PropertyLot.OPEN_TIME;

                //    int nTrayCount = Global.iData.PropertyLot.DEVICE_COUNT / (Global.iData.PropertyLot.ROWS * Global.iData.PropertyLot.COLUMNS);
                //    nTrayCount = Global.iData.PropertyLot.DEVICE_COUNT % (Global.iData.PropertyLot.ROWS * Global.iData.PropertyLot.COLUMNS) > 0 ? nTrayCount + 1 : nTrayCount;
                //    tbLotTrayCount.Text = nTrayCount.ToString();

                //    IGlobal.Instance.iData.SEQ_DATA.BUFFER_LOT_OPEN_TRAY_COUNT = nTrayCount;
                //    IGlobal.Instance.iData.SEQ_DATA.BUFFER_LOADING_TRAY_COUNT = nTrayCount;
                //    IGlobal.Instance.iData.SEQ_DATA.BUFFER_UNLOADING_TRAY_COUNT = 0;

                //    //IGlobal.Instance.iData.SEQ_DATA.BUFFER_LOADING_TRAY_COUNT = 5;
                //    ////IGlobal.Instance.iData.SEQ_DATA.BUFFER_UNLOADING_TRAY_COUNT = 7;

                //    //IGlobal.Instance.iData.SEQ_DATA.JIG_ELEVATOR_BUFFER[0] = CSeqData.TRAY_TYPE.JT;
                //    //IGlobal.Instance.iData.SEQ_DATA.JIG_ELEVATOR_BUFFER[1] = CSeqData.TRAY_TYPE.JT;
                //    //IGlobal.Instance.iData.SEQ_DATA.JIG_ELEVATOR_BUFFER[2] = CSeqData.TRAY_TYPE.JT;
                //    //IGlobal.Instance.iData.SEQ_DATA.JIG_ELEVATOR_BUFFER[3] = CSeqData.TRAY_TYPE.JT;
                //    //IGlobal.Instance.iData.SEQ_DATA.JIG_ELEVATOR_BUFFER[4] = CSeqData.TRAY_TYPE.JT;
                //    //IGlobal.Instance.iData.SEQ_DATA.JIG_ELEVATOR_BUFFER[5] = CSeqData.TRAY_TYPE.JT;
                //    //IGlobal.Instance.iData.SEQ_DATA.JIG_ELEVATOR_BUFFER[6] = CSeqData.TRAY_TYPE.JT;

                //    CLogger.Add(LOG.LOT, "");
                //    CLogger.Add(LOG.LOT, "==================== [LOT OPEN] ====================");
                //    CLogger.Add(LOG.LOT, $"WORKER : {Global.iData.PropertyLot.WORKER}");
                //    CLogger.Add(LOG.LOT, $"LOT_NO : {Global.iData.PropertyLot.LOT_NO}");
                //    CLogger.Add(LOG.LOT, $"METAL_TRAY_ID : {Global.iData.PropertyLot.METAL_TRAY_ID}");
                //    CLogger.Add(LOG.LOT, $"TOP_COVER_ID : {Global.iData.PropertyLot.METAL_TRAY_ID}");

                //    CLogger.Add(LOG.LOT, $"DEVICE_COUNT : {Global.iData.PropertyLot.DEVICE_COUNT}");
                //    CLogger.Add(LOG.LOT, $"TRAY_COUNT : {nTrayCount}");
                //    CLogger.Add(LOG.LOT, $"ROWS : {Global.iData.PropertyLot.ROWS}");
                //    CLogger.Add(LOG.LOT, $"COLUMNS : {Global.iData.PropertyLot.COLUMNS}");
                //    CLogger.Add(LOG.LOT, $"DATETIME : {Global.iData.PropertyLot.OPEN_TIME}");
                //    CLogger.Add(LOG.LOT, "==================== [--------] ====================");
                //    CLogger.Add(LOG.LOT, "");

                //    IGlobal.Instance.iData.SEQ_DATA.IS_COMPLETE_LOT_OPEN = true;

                //    pnLotOpen.Enabled = false;
                //}
                CUtil.ShowMessageBox("COMPLETE", "LOT OPEND");
                this.Close();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void keyPress_Device_Count(object sender, KeyPressEventArgs e)
        {
            if(!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void btnLotOpen_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (CUtil.ShowdialogMessageBox("LOT_OPEN", "DO YOU WANT TO LOT OPEN?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    
                    Global.Recipe.Data.LOT.OPEN_TIME = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                    //Global.Recipe.Data.LOT.WORKER = tbLotWorker.Texts;
                    //Global.Recipe.Data.LOT.LOT_NO = (tbLotID.Texts).Replace(" ", "");

                    //Global.Recipe.Data.LOT.BASE_METAL_DISTANCE_M = double.Parse(tbBaseMaterialDis.Texts);
                    //Global.Recipe.Data.LOT.TAPE_DISTANCE_M = double.Parse(tbTapeDis.Texts);
                    //Global.Recipe.Data.LOT.TAKEUP_DISTANCE_M = double.Parse(tbTakeUpDis.Texts);
                    //Global.Recipe.Data.LOT.TAKEUP_START_DEL_DISTANCE_M = double.Parse(tbTakeUpStartDelDis.Texts);

                    //Global.Recipe.Data.LOT.TAPE_T_MM = double.Parse(tbTapeThicknessMM.Texts);
                    //Global.Recipe.Data.LOT.BASEMETAL_T_MM = double.Parse(tbBaseMetalmm.Texts);
                    //Global.Recipe.Data.LOT.TAPE_TYPE = CUtil.ParseEnum<DEFINE.TapeType>(cbTapeMenu.Texts);

                    //lbLotOpenTime.Text = Global.Recipe.Data.LOT.OPEN_TIME;

                    
                    CLogger.Add(LOG.NORMAL, "");
                    CLogger.Add(LOG.NORMAL, "==================== [LOT OPEN] ====================");
                    CLogger.Add(LOG.NORMAL, $"WORKER : {Global.Recipe.Data.LOT.WORKER}");
                    CLogger.Add(LOG.NORMAL, $"LOT_NO : {Global.Recipe.Data.LOT.LOT_NO}");
                    CLogger.Add(LOG.NORMAL, $"BASE_METAL_DISTANCE : {Global.Recipe.Data.LOT.BASE_METAL_DISTANCE_M}");
                    CLogger.Add(LOG.NORMAL, $"TAPE_DISTANCE : {Global.Recipe.Data.LOT.TAPE_DISTANCE_M}");

                    CLogger.Add(LOG.NORMAL, $"TAKEUP_DISTANCE : {Global.Recipe.Data.LOT.TAKEUP_DISTANCE_M}");
                    CLogger.Add(LOG.NORMAL, $"TAKEUP_START_DEL_DISTTANCE : {Global.Recipe.Data.LOT.TAKEUP_START_DEL_DISTANCE_M}");
                    CLogger.Add(LOG.NORMAL, $"DATETIME : {Global.Recipe.Data.LOT.OPEN_TIME}");
                    CLogger.Add(LOG.NORMAL, "==================== [--------] ====================");
                    CLogger.Add(LOG.NORMAL, "");

                    CGlobal.Inst.Recipe.Data.LOT.IS_COMPLETE_LOT_OPEN = true;

                    pnLotOpen.Enabled = false;

                    CGlobal.Inst.Recipe.Data.LOT.SaveConfig(Global.Recipe.Name);

                    //Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.ClearActPos(0);
                    //Global.Device.MOTION_AJIN.AxisWork_Loader_LappingR.ClearComPos(0);
                    //Global.Device.MOTION_AJIN.AxisWork_MainR.ClearActPos(1);
                    //Global.Device.MOTION_AJIN.AxisWork_MainR.ClearComPos(1);
                    //Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.ClearActPos(2);
                    //Global.Device.MOTION_AJIN.AxisWork_BackAndForthY.ClearComPos(2);
                    //Global.Device.MOTION_AJIN.AxisWork_LappingR.ClearActPos(3);
                    //Global.Device.MOTION_AJIN.AxisWork_LappingR.ClearComPos(3);

                    //Global.Recipe.Data.FirstForword = true;
                    //Global.Recipe.Data.FirstBackWord = false;
                    //Global.Device.MOTION_AJIN.POS_BACK_AND_FORTH_Y.PreviousPos = 0;
                    //Global.Device.MOTION_AJIN.POS_BACK_AND_FORTH_Y.SaveConfig(Global.Recipe.Name);

                    Global.Recipe.Data.LOT.THICKNESS_DELAY_TIME.Restart();
                }
                CUtil.ShowMessageBox("COMPLETE", "LOT OPEND");
                this.Close();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}
