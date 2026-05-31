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
    public partial class FormSetting_Delay : MetroForm
    {
        private CGlobal Global = CGlobal.Inst;
        private CSignal Signal = null;

        public FormSetting_Delay(CSignal signal)
        {
            InitializeComponent();

            this.Signal = signal;
        }

        private void FormSetting_Delay_Load(object sender, EventArgs e)
        {
            InitUI();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void FormSetting_Delay_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        public bool InitUI()
        {
            try
            {
                if(Signal != null)
                {
                    this.Text = "DELAY - " + Signal.Name;

                    tbDelayBefore.Text = Signal.DELAY_BEFORE.ToString();
                    tbDelayAfter.Text = Signal.DELAY_AFTER.ToString();
                }

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
                
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (CUtil.ShowdialogMessageBox("SAVE", "DO YOU WANT TO SAVE THE PARAMETER OF DELAY", FormMessageBox.MESSAGEBOX_TYPE.Info))
                    {
                        Signal.DELAY_BEFORE = int.Parse(tbDelayBefore.Text);
                        Signal.DELAY_AFTER = int.Parse(tbDelayAfter.Text);

                        Signal.SaveConfig();

                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                    CUtil.ShowdialogMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
                }

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}
