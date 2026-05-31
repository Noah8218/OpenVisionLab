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
using MetroFramework.Controls;


namespace KtemVisionSystem
{
    public partial class FormKeyboard : MetroForm
    {
        public string ReturnString = "";
        public FormKeyboard()
        {
            InitializeComponent();
        }

        private void FormKeyboard_Load(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = true;
                this.TopLevel = true;

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbPassword.Text = "";
        }

        private void btnBackSpace_Click(object sender, EventArgs e)
        {
            try
            {
                tbPassword.Text = tbPassword.Text.Remove(tbPassword.Text.Length - 1);
            }
            catch(Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            try
            {
                ReturnString = tbPassword.Text;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void OnClickKeyboard(object sender, MouseEventArgs e)
        {
            try
            {
                if (sender is MetroTile)
                {
                    string strKey = (sender as MetroTile).Text;
                    tbPassword.Text = tbPassword.Text + strKey;
                }
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }            
        }
    }
}

