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

namespace KtemVisionSystem
{
    public partial class FormAuthorization_DoubleCheck : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;
        private bool m_bIsMaster1_Login = false;
        private string m_strMaster1_ID = "";
        private bool m_bIsMaster2_Login = false;

        public enum MESSAGEBOX_TYPE { OKCANCEL, OK, CANCEL, EXIT, btnLotOpen };

        public FormAuthorization_DoubleCheck()
        {
            InitializeComponent();

            this.KeyPreview = true;

            cbMaster1.Items.Clear();
            cbMaster2.Items.Clear();

            //for (int i = 0; i < Global.iData.AccountManager.Accounts.Count; i++)
            //{
            //    if(Global.iData.AccountManager.Accounts.ElementAt(i).Value.AUTHORIZATION == "MASTER")
            //    {
            //        cbMaster1.Items.Add(Global.iData.AccountManager.Accounts.ElementAt(i).Key);
            //        cbMaster2.Items.Add(Global.iData.AccountManager.Accounts.ElementAt(i).Key);
            //    }
            //}

            if (cbMaster1.Items.Count > 0) cbMaster1.SelectedIndex = 0;
            if (cbMaster2.Items.Count > 0) cbMaster2.SelectedIndex = 0;
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

        private void btnLoginMaster1_Click(object sender, EventArgs e)
        {
            try
            {
                string strAccount = cbMaster1.Text;
                //string strPassword = Global.iData.AccountManager.Accounts[strAccount].PASSWORD;


                using (FormKeyboard Form = new FormKeyboard())
                {
                    Form.ShowDialog();

                    string strPassword_Entered = Form.ReturnString.ToUpper();

                    //if(strPassword_Entered == strPassword)
                    //{
                    //    m_strMaster1_ID = strAccount;
                    //    m_bIsMaster1_Login = true;

                    //    lbMaster1.BackColor = DEFINE.COLOR_TEAL;
                    //    lbMaster1.ForeColor = Color.White;
                    //}
                    //else
                    //{
                    //    CUtil.ShowMessageBox("NOTICE", "PASSWORD IS WRONG");
                    //}
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnLoginMaster2_Click(object sender, EventArgs e)
        {
            try
            {
                if(!m_bIsMaster1_Login)
                {
                    CUtil.ShowdialogMessageBox("NOTICE", "MASTER #1 먼저 로그인 해주세요.");
                    return;
                }

                string strAccount = cbMaster2.Text;
                //string strPassword = Global.iData.AccountManager.Accounts[strAccount].PASSWORD;

                if(strAccount == m_strMaster1_ID)
                {
                    CUtil.ShowdialogMessageBox("NOTICE", "MASTER #1 계정 중복");
                    return;
                }

                using (FormKeyboard Form = new FormKeyboard())
                {
                    Form.ShowDialog();

                    string strPassword_Entered = Form.ReturnString.ToUpper();

                    //if (strPassword_Entered == strPassword)
                    //{
                    //    m_bIsMaster2_Login = true;

                    //    lbMaster2.BackColor = DEFINE.COLOR_TEAL;
                    //    lbMaster2.ForeColor = Color.White;

                    //    btnOK.Enabled = true;
                    //}
                    //else
                    //{
                    //    CUtil.ShowMessageBox("NOTICE", "PASSWORD IS WRONG");
                    //}
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCanel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
