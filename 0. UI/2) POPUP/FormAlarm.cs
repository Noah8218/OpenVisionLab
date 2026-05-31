using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lib.Common;
using MetroFramework.Forms;

namespace OpenVisionLab
{
    public partial class FormAlarm : MetroForm
    {
        public EventHandler<EventArgs> EventAlaramClear;
        //public EventHandler<StringEventArgs> EventSkip;
        //public EventHandler<StringEventArgs> EventSolved;
        //public EventHandler<StringEventArgs> EventResetSeq;
        //public EventHandler<StringEventArgs> EventRetry;

        public enum BUTTON_TYPE : uint { DEFAULT, RETRY, RETRYREJECT};
        public BUTTON_TYPE m_btnType = BUTTON_TYPE.DEFAULT;

        public enum BUTTON_RESULT : uint { NONE, SKIP, RESET, RETRY, REJECT };
        public BUTTON_RESULT m_ebtnResult = BUTTON_RESULT.NONE;

        public string m_strCode = "";
        public string m_strDesc = "";
        public string m_strPos = "";

        public FormAlarm(string strCode, string strDesc, string strPos, BUTTON_TYPE btnType = BUTTON_TYPE.DEFAULT)
        {
            InitializeComponent();

            this.m_strCode = strCode;
            this.m_strDesc = strDesc;
            this.m_strPos = strPos;
            this.m_btnType = btnType;

            this.TopMost = true;

            lbAlarmCode.Text = m_strCode;
            lbAlarmDesc.Text = m_strDesc;
            lbAlarmPos.Text = m_strPos;
            lbAlarmTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void FormAlarm_Load(object sender, EventArgs e)
        {
            try
            {
                switch (m_btnType)
                {
                    case BUTTON_TYPE.DEFAULT:
                        btnRetry.Visible = false;
                        btnReject.Visible = false;
                        break;
                    case BUTTON_TYPE.RETRY:
                        btnRetry.Visible = true;
                        btnReject.Visible = false;
                        break;
                    case BUTTON_TYPE.RETRYREJECT:
                        btnRetry.Visible = true;
                        btnReject.Visible = true;
                        break;
                }

                btnReject.Visible = true;

                gridAlarm.Rows.Clear();
                foreach (var item in CAlarm.m_dicNodes)
                {
                    string strCode = item.Value.Code;
                    string strDesc = item.Value.Desc;
                    string strPos = item.Value.Str;

                    gridAlarm.Rows.Add(new string[] {strCode, strDesc,strPos });
                }

                //CGlobal.Inst.Device.DIO_BD.On(CGlobal.Inst.Device.DIO_BD.DO_11_BUZZER);
                //CGlobal.Inst.Device.DIO_BD.DI_SYSTEM_SWITCH_RESET.EventUpdateSignal += OnInputReset;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
            }
        }

        private void OnInputReset(object sender, EventArgs e)
        {
            this.UIThreadInvoke(() =>
            {
                try
                {
                    if (sender is CSignal)
                    {
                        CSignal signal = sender as CSignal;

                        signal.IsDisplay = false;

                        if (signal.IsOn && CGlobal.Inst.System.Mode != CSystem.MODE.AUTO)
                        {
                            btnReset_Click(null, null);
                        }
                    }
                }
                catch (Exception Desc)
                {

                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                }
            });
        }

        public void Set(string strCode, string strDesc,string strPos)
        {
            lbAlarmCode.Text = strCode;
            lbAlarmDesc.Text = strDesc;
            lbAlarmPos.Text = strPos;

            lbAlarmTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            m_swTackTime = new System.Diagnostics.Stopwatch();
            m_swTackTime.Start();

            m_strCode = strCode;

            //if (strCode.Contains(DEFINE.ALARM_CODE_METAL_TRAY_LOADING_PUSHER)
            //    || strCode.Contains(DEFINE.ALARM_CODE_TOP_COVER_LOADING_PUSHER))
            //{
            //    btnRetry.Visible = true;
            //}
            //else
            //{
            //    btnRetry.Visible = false;
            //}

            btnRetry.Visible = true;
        }

        private System.Diagnostics.Stopwatch m_swTackTime = new System.Diagnostics.Stopwatch();
        public void OnShowProgress(object sender = null, EventArgs e = null)
        {
            this.UIThreadInvoke(() =>
            {
                if (lbAlarmCode.Text != "[1] 000")
                {
                    this.Show();

                    // if (m_strCode.Contains(DEFINE.ALARM_CODE_METAL_TRAY_LOADING_PUSHER)
                    //|| m_strCode.Contains(DEFINE.ALARM_CODE_TOP_COVER_LOADING_PUSHER)
                    //|| m_strCode.Contains(DEFINE.ALARM_CODE_DEVICE_LOST)
                    //)
                    // {
                    //     btnRetry.Visible = true;
                    // }
                    // else
                    // {
                    //     btnRetry.Visible = false;
                    // }
                    btnRetry.Visible = true;
                }
            });
        }

        public void OnEndProgress(object sender = null, EventArgs e = null)
        {
            this.UIThreadInvoke(() =>
            {
                this.Hide();
            });           
        }

        private void timerTackTime_Tick(object sender, EventArgs e)
        {
            int nMinutie = (int)m_swTackTime.ElapsedMilliseconds / 60000;
            int nSeconds = (int)(m_swTackTime.ElapsedMilliseconds % 60000) / 1000;
            lbTackTime.Text = $"{nMinutie}:{nSeconds}";
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {   
            try
            {
                m_ebtnResult = BUTTON_RESULT.SKIP;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            try
            {
                m_ebtnResult = BUTTON_RESULT.SKIP;
                this.DialogResult = DialogResult.OK;

                string strCode = "";
                string strDesc = "";
                string strPos = "";

                (strCode, strDesc, strPos) = CAlarm.GetLastAlarm();

                if (CAlarm.m_dicNodes.TryGetValue(strCode, out CNodeAlarm cNodeAlarm))
                {
                    CAlarm.m_dicNodes.Remove(strCode);
                }

                //CAlarm.Clear();
                CGlobal.Inst.System.AlarmWait.Set();
                
                if (CAlarm.m_dicNodes.Count == 0)
                {
                    CGlobal.Inst.System.Mode = CSystem.MODE.READY;
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            try
            {
                m_ebtnResult = BUTTON_RESULT.RETRY;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
        private void btnBuzzerOff_Click(object sender, EventArgs e)
        {
            try
            {
                //CMotionManager manager = CGlobal.Inst.Device.MOTION_AJIN;
                //CDIO_AJIN DIO = CGlobal.Inst.Device.DIO_BD;

                //if (DIO.DO_11_BUZZER.IsOn)
                //{
                //    DIO.Off(DIO.DO_11_BUZZER);
                //}

            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            m_ebtnResult = BUTTON_RESULT.RESET;
            this.DialogResult = DialogResult.OK;

            string strCode = "";
            string strDesc = "";
            string strPos = "";

            (strCode, strDesc, strPos) = CAlarm.GetLastAlarm();

            CAlarm.m_dicNodes.TryGetValue(strCode, out CNodeAlarm cNodeAlarm);
            CGlobal.Inst.System.AlarmWait.Set();            

            if (CAlarm.m_dicNodes.Count == 0)
            {
                CGlobal.Inst.System.Mode = CSystem.MODE.READY;
            }            

            CAlarm.m_dicNodes.Remove(strCode);            

            if (cNodeAlarm.EventAlaramClear != null) { cNodeAlarm.EventAlaramClear(null, new AlaramEventArgs(cNodeAlarm)); }
        }
    }
}
