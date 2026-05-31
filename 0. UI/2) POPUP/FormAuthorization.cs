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
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormAuthorization : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;

        public FormAuthorization()
        {
            InitializeComponent();
        }

        private void FormTeachingSelect_Load(object sender, EventArgs e)
        {
            try
            {                           
                switch( Global.System.Authorization )
                {
                    case DEFINE.AUTHORIZATION.OPERATOR: 
                        cbAuthoriztion.SelectedIndex = 0;
                        break;
                    case DEFINE.AUTHORIZATION.ENGINEER:
                        cbAuthoriztion.SelectedIndex = 1;
                        break;
                    case DEFINE.AUTHORIZATION.MASTER:
                        cbAuthoriztion.SelectedIndex = 2;
                        break;
                }

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
            
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string strIndex = cbAuthoriztion.Text;

                using(FormKeyboard Form = new FormKeyboard())
                {                  
                    if(strIndex == "OPERATOR")
                    {
                        Global.System.Authorization = DEFINE.AUTHORIZATION.OPERATOR;
                        this.Close();
                    }
                    else
                    {
                        CAccount account;

                        if (Form.ShowDialog() == DialogResult.OK)
                        {
                            string strPassword = Form.ReturnString.ToUpper();                            
                            switch (strIndex)
                            {
                                case "ENGINEER":
                                    Global.System.AccountManager.Accounts.TryGetValue("ENGINEER", out account);

                                    if (strPassword == account.PASSWORD.ToUpper())

                                    {
                                        Global.System.Authorization = DEFINE.AUTHORIZATION.ENGINEER;
                                        this.Close();
                                    }
                                    else
                                    {
                                        CCommon.ShowdialogMessageBox("NOTICE", "PASSWORD IS WRONG", FormMessageBox.MESSAGEBOX_TYPE.Info);
                                        lbNotice.Style = MetroFramework.MetroColorStyle.Red;
                                        lbNotice.Text = "Password is Wrong";
                                    }
                                    break;
                                case "MASTER":
                                    Global.System.AccountManager.Accounts.TryGetValue("MASTER", out account);

                                    if (strPassword == account.PASSWORD.ToUpper())
                                    {
                                        Global.System.Authorization = DEFINE.AUTHORIZATION.MASTER;
                                        this.Close();
                                    }
                                    else
                                    {
                                        CCommon.ShowdialogMessageBox("NOTICE", "PASSWORD IS WRONG", FormMessageBox.MESSAGEBOX_TYPE.Info);
                                        lbNotice.Style = MetroFramework.MetroColorStyle.Red;
                                        lbNotice.Text = "Password is Wrong";
                                    }
                                    break;
                            }
                        }                       
                    }                   
                }               
            }
            catch(Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnLoginClose_Click(object sender, EventArgs e)
        {
            using (FormKeyboard Form = new FormKeyboard())
            {                
                if (Form.ShowDialog() == DialogResult.OK)
                {

                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

