using System;
using System.Windows.Forms;
using System.Reflection;
using MetroFramework;
using MetroFramework.Forms;
using System.Collections.Generic;
//using ImageGlass;
using System.Threading;
using System.IO.Ports;

namespace KtemVisionSystem
{
    public partial class FormSettings_Illumination : MetroForm
    {
        CGlobal Global = CGlobal.Inst;

        #region Event Register        
        public EventHandler<EventArgs> EventUpdateUi;
        #endregion

        private List<RJCodeUI_M1.RJControls.RJButton> btnList = new List<RJCodeUI_M1.RJControls.RJButton>();

        private int SelectedIndex = 1;

        public FormSettings_Illumination()
        {
            InitializeComponent();        
        }

        private void FormSettings_Illumination_Load(object sender, EventArgs e)
        {
            InitEvent();
            AddBaudrate();
            AddComport();
            AddBtnList();

            tbChannelCount.Text = Global.Device.LIGHT.ChannelCount.ToString();
            cbComport.Text = Global.Device.LIGHT.PortName.ToString();
            cbBaudrate.Text = Global.Device.LIGHT.BaudRate.ToString();

            for (int i = 0; i < btnList.Count; i++)
            {
                btnList[i].Click += FormSettings_Illumination_Click;
                btnList[i].Enabled = false;
            }

            for (int i = 0; i < Global.Device.LIGHT.ChannelCount; i++)
            {
                btnList[i].Enabled = true;
            }
            //Global.Device.LIGHT.Event_OnOff += LIGHT_Event_OnOff;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void LIGHT_Event_OnOff(object sender, bool e)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            {
                tgbChannle.Checked = e;
                tgbChannle.Refresh();
            }));


        }

        private void FormSettings_Illumination_Click(object sender, EventArgs e)
        {
            RJCodeUI_M1.RJControls.RJButton btn = (sender as RJCodeUI_M1.RJControls.RJButton);
            SelectedIndex = int.Parse(btn.Text);

            lbSelectedChannel.Text = "선택 채널 : " + SelectedIndex.ToString();
            Global.Device.LIGHT.ReadOnOff(SelectedIndex);

            trbLight.Value = Global.Device.LIGHT.Values[SelectedIndex - 1];
        }

        private void AddBtnList()
        {
            try
            {
                btnList.Add(btnChannel1);
                btnList.Add(btnChannel2);
                btnList.Add(btnChannel3);
                btnList.Add(btnChannel4);
                btnList.Add(btnChannel5);
                btnList.Add(btnChannel6);
                btnList.Add(btnChannel7);
                btnList.Add(btnChannel8);
                btnList.Add(btnChannel9);
                btnList.Add(btnChannel10);
                btnList.Add(btnChannel11);
                btnList.Add(btnChannel12);
                btnList.Add(btnChannel13);
                btnList.Add(btnChannel14);
                btnList.Add(btnChannel15);
                btnList.Add(btnChannel16);
                btnList.Add(btnChannel17);
                btnList.Add(btnChannel18);
                btnList.Add(btnChannel19);
                btnList.Add(btnChannel20);
                btnList.Add(btnChannel21);
                btnList.Add(btnChannel22);
                btnList.Add(btnChannel23);
                btnList.Add(btnChannel24);
                btnList.Add(btnChannel25);
                btnList.Add(btnChannel26);
                btnList.Add(btnChannel27);
                btnList.Add(btnChannel28);
                btnList.Add(btnChannel29);
                btnList.Add(btnChannel30);
                btnList.Add(btnChannel31);
                btnList.Add(btnChannel32);
                btnList.Add(btnChannel33);
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void AddComport()
        {
            try
            {
                string[] ComportList = CUtil.AvalibleComports();

                if (ComportList.Length > 0)
                {
                    cbComport.Items.AddRange(ComportList);
                    cbComport.SelectedIndex = 0;
                }

                if(Global.Device.LIGHT.IsOpen)
                {
                    cbComport.Text = Global.Device.LIGHT.PortName.ToString();
                }
                
                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void AddBaudrate()
        {
            try
            {
                cbBaudrate.Items.Add("9600");
                cbBaudrate.Items.Add("19200");
                cbBaudrate.Items.Add("38400");
                cbBaudrate.Items.Add("51200");
                cbBaudrate.Items.Add("115400");

                cbBaudrate.SelectedIndex = 0;

                if (Global.Device.LIGHT.IsOpen)
                {
                    cbBaudrate.Text = Global.Device.LIGHT.BaudRate.ToString();
                }

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private bool InitEvent()
        {
            try
            {
                this.KeyPreview = true;
                this.KeyDown += Form_KeyDown;

                Global.Recipe.EventChagedRecipe += OnChangedRecipe;

                CLogger.Add(LOG.NORMAL, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        private void OnChangedRecipe(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnChangedRecipe(sender, e);
                    }));
                }
                catch (Exception ex)
                {
                    CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            else
            {
                          
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void trbValue_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void btnON_Click(object sender, EventArgs e)
        {
            try
            {
                //int nChannel = int.Parse(cbChannel.Text);
                //Global.Device.LIGHT.AllOn();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowdialogMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }            
        }

        private void btnOFF_Click(object sender, EventArgs e)
        {
            try
            {
                //int nChannel = int.Parse(cbChannel.Text);
                //Global.Device.LIGHT.AllOff();
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowdialogMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

        }

        private void lbConnected_Click(object sender, EventArgs e)
        {
            try
            {
                //Global.Device.LIGHT.PortName = cbPortName.Text;
                //Global.Device.LIGHT.BaudRate = int.Parse(cbBaudrate.Text);

                //if (!Global.Device.LIGHT.IsOpen)
                //{
                //    Global.Device.LIGHT.Connect();
                //}
                //else
                //{
                //    Global.Device.LIGHT.DisConnect();
                //    Global.Device.LIGHT.Connect();
                //}
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnSave_LightController_Click(object sender, EventArgs e)
        {
            try
            {
                if (CUtil.ShowdialogMessageBox("SAVE", "DO YOU WANT TO SAVE THE PARAMETER LIGHTCONTROLLER", FormMessageBox.MESSAGEBOX_TYPE.Info))
                {
                    //Global.Device.LIGHT.WriteInitFile();
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowdialogMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void trbLightValue_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                //int LightValue = trbLightValue.Value;
                //lbValue.Text = LightValue.ToString();
                //int Channel = int.Parse(cbLightCh.SelectedItem.ToString());
                //Global.Device.LIGHT.SetValue(Channel, LightValue);
            }
            catch(Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Exception ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnClose_LightController_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }

            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Exception ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConnection_Click(object sender, EventArgs e)
        {
            try
            {
                Global.Device.LIGHT.PortName = cbComport.Texts;
                Global.Device.LIGHT.BaudRate = int.Parse(cbBaudrate.Texts);

                if (!Global.Device.LIGHT.IsOpen)
                {
                    Global.Device.LIGHT.DisConnect();
                    Global.Device.LIGHT.Connect(Global.Recipe.Name);                    
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnDisConnection_Click(object sender, EventArgs e)
        {
            try
            {                
                if (Global.Device.LIGHT.IsOpen)
                {
                    Global.Device.LIGHT.DisConnect();
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }        

        private void timerConnected_Tick(object sender, EventArgs e)
        {
            if (Global.Device.LIGHT.IsOpen) { CUtil.UpdateLabelOnOff(lbStatusConnected, true); }
            else { CUtil.UpdateLabelOnOff(lbStatusConnected, false); }
        }

        private void btnChannel1_Click(object sender, EventArgs e)
        {

        }

        private void tgbChannle_CheckedChanged(object sender, EventArgs e)
        {
            //RJToggleButton pt = (RJToggleButton)sender;

            //if(pt.Checked)
            //{
            //    Global.Device.LIGHT.On(SelectedIndex);
            //}
            //else
            //{
            //    Global.Device.LIGHT.Off(SelectedIndex);
            //}
            
        }

        private void tgbChannle_Click(object sender, EventArgs e)
        {
            RJCodeUI_M1.RJControls.RJToggleButton pt = (RJCodeUI_M1.RJControls.RJToggleButton)sender;

            if (pt.Checked)
            {
                Global.Device.LIGHT.On(SelectedIndex);
            }
            else
            {
                Global.Device.LIGHT.Off(SelectedIndex);
            }

        }

        private void btnSaveParameter_Click(object sender, EventArgs e)
        {
            try
            {
                if (CUtil.ShowdialogMessageBox("SAVE", "DO YOU WANT TO SAVE THE PARAMETER LIGHTCONTROLLER", FormMessageBox.MESSAGEBOX_TYPE.Info))
                {
                    Global.Device.LIGHT.ChannelCount = int.Parse(tbChannelCount.Text);
                    Global.Device.LIGHT.PortName = cbComport.SelectedItem.ToString();
                    Global.Device.LIGHT.BaudRate = int.Parse(cbBaudrate.SelectedItem.ToString());
                    Global.Device.LIGHT.WriteInitFile(Global.Recipe.Name);

                    for (int i = 0; i < btnList.Count; i++) { btnList[i].Enabled = false; }
                    for (int i = 0; i < Global.Device.LIGHT.ChannelCount; i++) { btnList[i].Enabled = true; }                   
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowdialogMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void tgbAllChannle_CheckedChanged(object sender, EventArgs e)
        {
            RJCodeUI_M1.RJControls.RJToggleButton pt = (RJCodeUI_M1.RJControls.RJToggleButton)sender;

            if (pt.Checked)
            {
                Global.Device.LIGHT.AllOn();
            }
            else
            {
                Global.Device.LIGHT.AllOff();
            }            
        }

        private void tbChannelCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            CUtil.txtInterval_KeyPress(sender, e);
        }

        private void trbLight_Scroll(object sender, EventArgs e)
        {
            try
            {
                Thread.Sleep(30);

                int LightValue = trbLight.Value;
                Global.Device.LIGHT.Write(SelectedIndex, LightValue);

                Global.Device.LIGHT.Values[SelectedIndex - 1] = LightValue;
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1}   Exception ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
 }

