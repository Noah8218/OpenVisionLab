using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using MetroFramework.Forms;
using MetroFramework.Controls;
using System.Collections.Generic;

namespace KtemVisionSystem
{
    public partial class FormIO_BD : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;

        private const int DGV_NO = 0;
        private const int DGV_SECTION = 1;
        private const int DGV_NAME = 2;
        private const int DGV_STATUS = 3;
        private const int DGV_CONTACT = 4;

        private const int DGV_OUTPUT_DELAY_BEF = 4;
        private const int DGV_OUTPUT_DELAY_AFT = 5;

        public EventHandler<EventArgs> EventUpdateStatus;
        public FormIO_BD()
        {
            InitializeComponent();
        }

        private void FormIO_Load(object sender, EventArgs e)
        {
            InitIO();
            InitUI();

            timerIO.Enabled = true;

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
      
        private void FormIO_FormClosing(object sender, FormClosingEventArgs e)
        {
            Feeder_Off();

            Hide();
        }


        private void dgvDO_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex != DGV_STATUS) return;

                CSignal signal = null;

                bool bContinue = true;

                for (int nTotalCount = 0; nTotalCount < CDIO_AJIN.Outputs.Count; nTotalCount++)
                {
                    if (!bContinue) { break; }

                    List<CSignal> Outputs = CDIO_AJIN.Outputs;

                    string strAddr = dgvDO[DGV_NO, e.RowIndex].Value.ToString();

                    for(int j =0; j < Outputs.Count; j++)
                    {
                        if (strAddr == Outputs[j].Address)
                        {
                            signal = Outputs[j];
                            bContinue = false;

                            break;
                        }
                    }
                }

                if (signal != null)
                {
                    if (signal.Current == CSignal.SIGNAL_ON)
                    {
                        Global.Device.DIO_BD.Off(signal);
                    }
                    else if (signal.Current == CSignal.SIGNAL_OFF)
                    {
                        Global.Device.DIO_BD.On(signal);
                    }
                }
                else
                {
                  //  CLog.Error( "[FAILED] {0}==>{1}   Can't Find the Output", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                }
            }
            catch (Exception ex)
            {
                //CLog.Error( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

       

        public void OnChangeStyle(object sender, EventArgs e)
        {
        }        

        public bool InitUI()
        {
            List<CSignal> Inputs = CDIO_AJIN.Inputs;

            for (int i = 0; i < Inputs.Count; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    dgvDI.Rows[i].Cells[j].Style.ForeColor = Color.Black;
                    dgvDI.Rows[i].Cells[j].Style.BackColor = Color.White;

                    dgvDI.Rows[i].Cells[j].Style.SelectionForeColor = Color.Black;
                    dgvDI.Rows[i].Cells[j].Style.SelectionBackColor = Color.White;
                }
            }

            List<CSignal> Outputs = CDIO_AJIN.Outputs;

            for (int i = 0; i < Outputs.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    dgvDO.Rows[i].Cells[j].Style.ForeColor = Color.Black;
                    dgvDO.Rows[i].Cells[j].Style.BackColor = Color.White;

                    dgvDO.Rows[i].Cells[j].Style.SelectionForeColor = Color.Black;
                    dgvDO.Rows[i].Cells[j].Style.SelectionBackColor = Color.White;
                }
            }

            return true;
        }

        public bool InitIO()
        {
            try
            {
                List<CSignal> Inputs = CDIO_AJIN.Inputs;

                for (int i = 0; i < Inputs.Count; i++)
                {
                    AddRowIO(Inputs[i], "Input");
                }

                List<CSignal> Outputs = CDIO_AJIN.Outputs;

                for (int i = 0; i < Outputs.Count; i++)
                {
                    AddRowIO(Outputs[i], "Output");
                }

                // Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
               // CLog.Error( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        public bool AddRowIO(CSignal signal, string strName)
        {
            try
            {
                if (signal == null)
                {
                    return false;
                }
                if (strName == "Input")
                {
                    dgvDI.Rows.Add(signal.Address, signal.Section, signal.Name, signal.Current == CSignal.SIGNAL_ON ? "ON" : "OFF", signal.IS_CONTACT_A? "A":"B");
                }
                else if (strName == "Output")
                {
                    dgvDO.Rows.Add(signal.Address, signal.Section, signal.Name, signal.Current == CSignal.SIGNAL_OFF ? "ON" : "OFF");
                }

                return true;
            }
            catch (Exception ex)
            {
                //CLog.Error( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message); 
                return false;
            }
        }

        private void timerIO_Tick(object sender, EventArgs e)
        {
            List<CSignal> Inputs = CDIO_AJIN.Inputs;

            for (int i = 0; i < Inputs.Count; i++)
            {                
                dgvDI.Rows[i].Cells[DGV_CONTACT].Value = Inputs[i].IS_CONTACT_A ? "A" : "B";

                if (Inputs[i].IsOn)
                {
                    dgvDI.Rows[i].Cells[DGV_STATUS].Value = "ON";
                    dgvDI.Rows[i].Cells[DGV_STATUS].Style.ForeColor = Color.Black;
                    dgvDI.Rows[i].Cells[DGV_STATUS].Style.BackColor = Color.Aquamarine;

                    dgvDI.Rows[i].Cells[DGV_STATUS].Style.SelectionForeColor = Color.Black;
                    dgvDI.Rows[i].Cells[DGV_STATUS].Style.SelectionBackColor = Color.Aquamarine;
                }
                else
                {
                    dgvDI.Rows[i].Cells[DGV_STATUS].Value = "OFF";
                    dgvDI.Rows[i].Cells[DGV_STATUS].Style.ForeColor = Color.Black;
                    dgvDI.Rows[i].Cells[DGV_STATUS].Style.BackColor = Color.White;

                    dgvDI.Rows[i].Cells[DGV_STATUS].Style.SelectionForeColor = Color.Black;
                    dgvDI.Rows[i].Cells[DGV_STATUS].Style.SelectionBackColor = Color.White;

                    dgvDI.Rows[i].DefaultCellStyle.BackColor = Color.White;
                }
            }

            List<CSignal> Outputs = CDIO_AJIN.Outputs;

            for (int i = 0; i < Outputs.Count; i++)
            {                
                if (Outputs[i].Current == CSignal.SIGNAL_ON)
                {
                    dgvDO.Rows[i].Cells[DGV_STATUS].Value = "ON";
                    dgvDO.Rows[i].Cells[DGV_STATUS].Style.BackColor = Color.Aquamarine;
                    dgvDO.Rows[i].Cells[DGV_STATUS].Style.ForeColor = Color.Black;

                    dgvDO.Rows[i].Cells[DGV_STATUS].Style.SelectionBackColor = Color.Aquamarine;
                    dgvDO.Rows[i].Cells[DGV_STATUS].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    dgvDO.Rows[i].Cells[DGV_STATUS].Value = "OFF";
                    dgvDO.Rows[i].Cells[DGV_STATUS].Style.ForeColor = Color.Black;
                    dgvDO.Rows[i].Cells[DGV_STATUS].Style.BackColor = Color.White;

                    dgvDO.Rows[i].Cells[DGV_STATUS].Style.SelectionForeColor = Color.Black;
                    dgvDO.Rows[i].Cells[DGV_STATUS].Style.SelectionBackColor = Color.White;
                }

                dgvDO.Rows[i].Cells[DGV_OUTPUT_DELAY_BEF].Value = Outputs[i].DELAY_BEFORE.ToString();
                dgvDO.Rows[i].Cells[DGV_OUTPUT_DELAY_AFT].Value = Outputs[i].DELAY_AFTER.ToString();
            }
        }
        private void Feeder_Off()
        {
            
        }
        private void dgvDI_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dgvDI_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (Global.System.Authorization != DEFINE.AUTHORIZATION.MASTER) return;
                if (e.ColumnIndex != DGV_CONTACT) return;

                CSignal signal = null;                
                List<CSignal> Inputs = CDIO_AJIN.Inputs;

                string strAddr = dgvDI[DGV_NO, e.RowIndex].Value.ToString();

                for (int j = 0; j < Inputs.Count; j++)
                {
                    if (strAddr == Inputs[j].Address)
                    {
                        signal = Inputs[j];
                        break;
                    }
                }

                if (CUtil.ShowMessageBox("SAVE", $"DO YOU WANT TO CHANGE THE CONTACT?\n [{signal.Name}]", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    if (signal != null)
                    {
                        signal.IS_CONTACT_A = !signal.IS_CONTACT_A;
                        signal.SaveConfig();
                    }
                }

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void dgvDO_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (Global.System.Authorization != DEFINE.AUTHORIZATION.MASTER) return;
                if (e.ColumnIndex != DGV_OUTPUT_DELAY_BEF && e.ColumnIndex != DGV_OUTPUT_DELAY_AFT) return;

                CSignal signal = null;                
                List<CSignal> Outputs = CDIO_AJIN.Outputs;

                string strAddr = dgvDO[DGV_NO, e.RowIndex].Value.ToString();

                for (int j = 0; j < Outputs.Count; j++)
                {
                    if (strAddr == Outputs[j].Address)
                    {
                        signal = Outputs[j];      
                        break;
                    }
                }

                //if (signal != null)
                //{
                //    FormSetting_Delay Frm = new FormSetting_Delay(signal);
                //    Frm.ShowDialog();
                //}

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}
