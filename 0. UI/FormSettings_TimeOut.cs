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
    public partial class FormSetting_TimeOut : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;

        public FormSetting_TimeOut()
        {
            InitializeComponent();
        }

        private void FormSetting_TimeOut_Load(object sender, EventArgs e)
        {
            InitUI();
        }

        private void FormSetting_TimeOut_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        public bool InitUI()
        {
            try
            {
                dgvTimeOut.Rows.Clear();

                //for (int i = 0; i < Global.iData.TIME_OUT.List.Count; i++)
                //{
                //    string strName = Global.iData.TIME_OUT.List.ElementAt(i).Value.NAME;
                //    string strTimeOutms = Global.iData.TIME_OUT.List.ElementAt(i).Value.TIME_OUT.ToString();
                //    string strAction = Global.iData.TIME_OUT.List.ElementAt(i).Value.ACTION_IS_ALARM ? "ALARM" : "CONTINUE";

                //    dgvTimeOut.Rows.Add(new string[] { strName, strTimeOutms, strAction });
                //}

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
            //Global.iData.TIME_OUT.Init();
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (CUtil.ShowMessageBox("SAVE", "DO YOU WANT TO SAVE THE PARAMETER OF TIME_OUT", FormMessageBox.MESSAGEBOX_TYPE.OKCANCEL))
                    {
                        //Global.iData.TIME_OUT.Save();
                        InitUI();
                       
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                    CUtil.ShowMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
                }

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                string strName = lbName.Text;
                int nTimeOutms = int.Parse(tbTimeOut.Text);

                //Global.iData.TIME_OUT.List[strName].TIME_OUT = nTimeOutms;

                InitUI();

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnActionContinue_Click(object sender, EventArgs e)
        {
            try
            {
                string strName = lbName.Text;
                //Global.iData.TIME_OUT.List[strName].ACTION_IS_ALARM = false;

                InitUI();

                //if (Global.iData.TIME_OUT.List[strName].ACTION_IS_ALARM)
                //{
                //    btnActionAlarm.BackColor = DEFINE.COLOR_TEAL;
                //    btnActionContinue.BackColor = Color.DimGray;
                //}
                //else
                //{
                //    btnActionAlarm.BackColor = Color.DimGray;
                //    btnActionContinue.BackColor = DEFINE.COLOR_TEAL;
                //}

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnActionAlarm_Click(object sender, EventArgs e)
        {
            try
            {
                string strName = lbName.Text;
                //Global.iData.TIME_OUT.List[strName].ACTION_IS_ALARM = true;

                InitUI();

                //if (Global.iData.TIME_OUT.List[strName].ACTION_IS_ALARM)
                //{
                //    btnActionAlarm.BackColor = DEFINE.COLOR_TEAL;
                //    btnActionContinue.BackColor = Color.DimGray;
                //}
                //else
                //{
                //    btnActionAlarm.BackColor = Color.DimGray;
                //    btnActionContinue.BackColor = DEFINE.COLOR_TEAL;
                //}

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void dgvTimeOut_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string strName = dgvTimeOut[0, e.RowIndex].Value.ToString();

                //if(Global.iData.TIME_OUT.List[strName].ACTION_IS_ALARM) 
                //{
                //    btnActionAlarm.BackColor = DEFINE.COLOR_TEAL;
                //    btnActionContinue.BackColor = Color.DimGray;
                //}
                //else
                //{
                //    btnActionAlarm.BackColor = Color.DimGray;
                //    btnActionContinue.BackColor = DEFINE.COLOR_TEAL;
                //}

                lbName.Text = strName;
                //tbTimeOut.Text = Global.iData.TIME_OUT.List[strName].TIME_OUT.ToString();

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}
