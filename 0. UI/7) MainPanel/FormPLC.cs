using OpenVisionLab.Common;
using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormPLC : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;

        private const int DGV_NO = 0;        
        private const int DGV_NAME = 1;
        private const int DGV_STATUS = 2;
        private const int DGV_CONTACT = 3;
        private const int DGV_SET = 4;

        private const int DGV_OUTPUT_DELAY_BEF = 4;
        private const int DGV_OUTPUT_DELAY_AFT = 5;

        public EventHandler<EventArgs> EventUpdateStatus;

        public bool ChangeSize { get; set; } = false;

        public FormPLC()
        {           
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }
     
        // If the content won't display nicely, hide it
        private void ResizeEvent(object sender, EventArgs e)
        {
            this.Visible = this.Width > this.MinimumSize.Width && this.Height > this.MinimumSize.Height;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            InitAddress();
            InitUI();

            timerIO.Enabled = true;

            Global.System.EventUpdateStyle += OnChangeStyle;

            this.KeyPreview = true;

            for (int i = 0; i < dgvDI.Columns.Count - 1; i++)
            {
                dgvDI.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dgvDI.Columns[dgvDI.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            for (int i = 0; i < dgvDO.Columns.Count - 1; i++)
            {
                dgvDO.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dgvDO.Columns[dgvDO.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void FormLayerDisplay_VisibleChanged(object sender, EventArgs e)
        {
            if (!ChangeSize)
            {
                if (DockHandler.FloatPane == null) { return; }
                DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(DockHandler.FloatPane.FloatWindow.Bounds.X, DockHandler.FloatPane.FloatWindow.Bounds.Y, 800, 400);
                this.Refresh();
                ChangeSize = true;
            }
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
                if (e.ColumnIndex != DGV_SET) return;

                CSignal signal = null;

                bool bContinue = true;

                for (int nTotalCount = 0; nTotalCount < CDIO_PLC.Outputs.Count; nTotalCount++)
                {
                    if (!bContinue) { break; }

                    List<CSignal> Outputs = CDIO_PLC.Outputs;

                    string strAddr = dgvDO[DGV_NO, e.RowIndex].Value.ToString();
                    

                    for (int j = 0; j < Outputs.Count; j++)
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
                    int Value = int.Parse(dgvDO[DGV_CONTACT, e.RowIndex].Value.ToString());

                    Global.Device.DIO_PLC.WriteWord(signal, Value);
                }
                else
                {
                    //  CLog.Error( "[FAILED] {0}==>{1}   Can't Find the Output", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                }
            }
            catch (Exception Desc)
            {
                //CLog.Error( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }



        public void OnChangeStyle(object sender, EventArgs e)
        {
        }

        public bool InitUI()
        {
            List<CSignal> Inputs = CDIO_PLC.Inputs;

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

            List<CSignal> Outputs = CDIO_PLC.Outputs;

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

        public bool InitAddress()
        {
            try
            {
                List<CSignal> Inputs = CDIO_PLC.Inputs;

                for (int i = 0; i < Inputs.Count; i++)
                {
                    AddRowIO(Inputs[i], "Input");
                }

                List<CSignal> Outputs = CDIO_PLC.Outputs;

                for (int i = 0; i < Outputs.Count; i++)
                {
                    AddRowIO(Outputs[i], "Output");
                }
                
            }
            catch (Exception Desc)
            {
                // CLog.Error( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }

        public bool AddRowIO(CSignal signal, string strName)
        {
            try
            {
                switch(strName)
                {
                    case "Input":
                        dgvDI.Rows.Add(signal.Address, signal.Name, signal.Current);
                        break;
                    case "Output":
                        dgvDO.Rows.Add(signal.Address, signal.Name, signal.Current);
                        break;
                    default: return false;
                }

                for (int i = 0; i < dgvDO.Rows.Count; i++)
                {
                    dgvDO.Rows[i].Cells[DGV_NO].ReadOnly = true;
                    dgvDO.Rows[i].Cells[DGV_NAME].ReadOnly = true;
                    dgvDO.Rows[i].Cells[DGV_STATUS].ReadOnly = true;
                }

                return true;
            }
            catch (Exception Desc)
            {
                //CLog.Error( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}"); 
                return false;
            }
        }

        private void timerIO_Tick(object sender, EventArgs e)
        {
            List<CSignal> Inputs = CDIO_PLC.Inputs;
            
            for (int i = 0; i < Inputs.Count; i++)
            {
                if (dgvDI.Rows.Count == 0) { return; }

                dgvDI.Rows[i].Cells[DGV_STATUS].Value = Inputs[i].Current;
                dgvDI.Rows[i].Cells[DGV_STATUS].Style.ForeColor = Color.Black;
                dgvDI.Rows[i].Cells[DGV_STATUS].Style.BackColor = Color.White;

                dgvDI.Rows[i].Cells[DGV_STATUS].Style.SelectionForeColor = Color.Black;
                dgvDI.Rows[i].Cells[DGV_STATUS].Style.SelectionBackColor = Color.White;

                dgvDI.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }

            List<CSignal> Outputs = CDIO_PLC.Outputs;

            for (int i = 0; i < Outputs.Count; i++)
            {
                dgvDO.Rows[i].Cells[DGV_STATUS].Value = Outputs[i].Current.ToString(); 
                dgvDO.Rows[i].Cells[DGV_SET].Value = "Set";
                dgvDO.Rows[i].Cells[DGV_SET].Style.BackColor = Color.White;
                dgvDO.Rows[i].Cells[DGV_SET].Style.ForeColor = Color.Black;

                dgvDO.Rows[i].Cells[DGV_SET].Style.SelectionBackColor = Color.Aquamarine;
                dgvDO.Rows[i].Cells[DGV_SET].Style.SelectionForeColor = Color.Black;
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
                List<CSignal> Inputs = CDIO_PLC.Inputs;

                string strAddr = dgvDI[DGV_NO, e.RowIndex].Value.ToString();

                for (int j = 0; j < Inputs.Count; j++)
                {
                    if (strAddr == Inputs[j].Address)
                    {
                        signal = Inputs[j];
                        break;
                    }
                }

                if (CCommon.ShowMessageBox("SAVE", $"DO YOU WANT TO CHANGE THE CONTACT?\n [{signal.Name}]", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    if (signal != null)
                    {
                        signal.IS_CONTACT_A = !signal.IS_CONTACT_A;
                        signal.SaveConfig();
                    }
                }

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void dgvDO_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (Global.System.Authorization != DEFINE.AUTHORIZATION.MASTER) return;
                if (e.ColumnIndex != DGV_OUTPUT_DELAY_BEF && e.ColumnIndex != DGV_OUTPUT_DELAY_AFT) return;

                CSignal signal = null;
                List<CSignal> Outputs = CDIO_PLC.Outputs;

                string strAddr = dgvDO[DGV_NO, e.RowIndex].Value.ToString();

                for (int j = 0; j < Outputs.Count; j++)
                {
                    if (strAddr == Outputs[j].Address)
                    {
                        signal = Outputs[j];
                        break;
                    }
                }

                if (signal != null)
                {
                    //FormSetting_Delay Frm = new FormSetting_Delay(signal);
                    //Frm.ShowDialog();
                }

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void timerIO_Tick_1(object sender, EventArgs e)
        {

        }
    }
}
