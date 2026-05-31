using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MetroFramework.Forms;
using MetroFramework.Controls;

namespace KtemVisionSystem
{
    public partial class FormMessageBox_MemoryAlarm : MetroForm
    {
        private bool m_bIsRecovery = false;
        public enum MESSAGEBOX_TYPE { OKCANCEL, OK, CANCEL, EXIT, btnLotOpen };

        public FormMessageBox_MemoryAlarm(string strPosition)
        {
            InitializeComponent();

            this.KeyPreview = true;

            //if(CGlobal.Instance.iData.SEQ_DATA.ABNORMAL_STOP)
            //{
            //    btnIsRecovery.Visible = true;
            //}
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(m_bIsRecovery)
            {
                //CGlobal.Instance.iData.SEQ_DATA.REQ_RECOVERY = true;
            }
            //CGlobal.Instance.iData.SEQ_DATA.ABNORMAL_STOP = false;
            this.DialogResult = DialogResult.OK;        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void FormMessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                // CLogger.WriteLog(LOG.NORMAL, "[OK] {0}==>{1}   Action ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);

                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void btnIsRecovery_Click(object sender, EventArgs e)
        {
            m_bIsRecovery = !m_bIsRecovery;

            if(m_bIsRecovery)
            {
                btnIsRecovery.BackColor = DEFINE.COLOR_TEAL;
            }
            else
            {
                btnIsRecovery.BackColor = Color.DimGray;
            }
        }
    }
}
