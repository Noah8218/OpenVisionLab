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
    public partial class FormViewer_2DID : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;

        private const int DGV_NO = 0;
        private const int DGV_ID = 1;
        private const int DGV_STATUS = 2;

        public EventHandler<EventArgs> EventUpdateStatus;
        public FormViewer_2DID()
        {
            InitializeComponent();
        }

        private void FormViewer_2DID_Load(object sender, EventArgs e)
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

        private void FormViewer_2DID_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        public void OnChangeStyle(object sender, EventArgs e)
        {
        }        

        public bool InitUI()
        {
            try
            {
                tbZigElevatorBufferCount.Text = Global.iData.PropertyBufferPitch.ZIG_ELEVATOR_BUFFER_COUNT.ToString();

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
                dgvMetalTray.Rows.Clear();
                dgvTopCover.Rows.Clear();

                for (int i = 0; i < IGlobal.Instance.iData.SEQ_DATA.BUFFER_METAL_TRAY_ID.Length; i++)
                {
                    string strStatus = Enum.GetName(typeof(CSeqData.TRAY_TYPE), Global.iData.SEQ_DATA.JIG_ELEVATOR_BUFFER[i]);

                    dgvMetalTray.Rows.Add((i + 1).ToString(), IGlobal.Instance.iData.SEQ_DATA.BUFFER_METAL_TRAY_ID[i], strStatus);
                    dgvTopCover.Rows.Add((i + 1).ToString(), IGlobal.Instance.iData.SEQ_DATA.BUFFER_TOP_COVER_ID[i], strStatus);
                }
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
                Global.iData.PropertyBufferPitch.SaveConfig(Global.System.Recipe.Name);
                Global.iData.PropertyBufferPitch.LoadConfig(Global.System.Recipe.Name);
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }            
        }
    }
}
