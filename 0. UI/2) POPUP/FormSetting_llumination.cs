using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using RJCodeUI_M1.RJForms;
using System.Threading;
using Lib.Common;
using System.IO.Ports;

namespace OpenVisionLab
{
    public partial class FormSetting_llumination : RJChildForm
    {
        private CGlobal Global = CGlobal.Inst;

        #region Event Register        
        public EventHandler<EventArgs> EventUpdateUi;
        #endregion

        private List<RJCodeUI_M1.RJControls.RJButton> btnList = new List<RJCodeUI_M1.RJControls.RJButton>();

        private int SelectedIndex = 1;

        public FormSetting_llumination()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            InitEvent();
            AddBaudrate();
            AddComport();
            AddBtnList();
            AddLIGHT();
            tbLightCount.Text = CGlobal.Inst.Device.LIGHT_COUNT.ToString();
            
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private bool InitEvent()
        {
            try
            {
                this.KeyPreview = true;
                this.KeyDown += Form_KeyDown;
                Global.Recipe.EventChagedRecipe += OnChangedRecipe;
                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }

        private void OnChangedRecipe(object sender, EventArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {

            });
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
            Global.Device.LIGHTS[nIndex].ReadOnOff(SelectedIndex);
            
            trbLight.Value = Global.Device.LIGHTS[nIndex].Values[SelectedIndex - 1];
            tgbChannle.Checked = Global.Device.LIGHTS[nIndex].OnOffCheck[SelectedIndex - 1];
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
                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void AddComport()
        {
            try
            {                
                string[] ComportList = SerialPort.GetPortNames();

                if (ComportList.Length > 0)
                {
                    cbComport.Items.AddRange(ComportList);
                    cbComport.SelectedIndex = 0;
                }

                if (Global.Device.LIGHTS[nIndex].IsOpen)
                {
                    cbComport.SelectedIndex = cbComport.Items.IndexOf(Global.Device.LIGHTS[nIndex].PortName.ToString());
                }

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void AddLIGHT()
        {
            try
            {
                for (int i = 0; i < CGlobal.Inst.Device.LIGHT_COUNT; i++) { cbLightIndex.Items.Add((i + 1) + "-LIGHT"); }
                cbLightIndex.SelectedIndex = 0;

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
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
                cbBaudrate.Items.Add("115200");

                cbBaudrate.SelectedIndex = 0;

                if (Global.Device.LIGHTS[nIndex].IsOpen)
                {
                    cbBaudrate.SelectedIndex = cbBaudrate.Items.IndexOf(Global.Device.LIGHTS[nIndex].BaudRate.ToString());
                }

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnON_Click(object sender, EventArgs e)
        {
            try
            {
                //int nChannel = int.Parse(cbChannel.Text);
                //Global.Device.LIGHT.AllOn();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowdialogMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void btnOFF_Click(object sender, EventArgs e)
        {
            try
            {
                //int nChannel = int.Parse(cbChannel.Text);
                //Global.Device.LIGHT.AllOff();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowdialogMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
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
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnSave_LightController_Click(object sender, EventArgs e)
        {
            try
            {
                if (CCommon.ShowdialogMessageBox("SAVE", "DO YOU WANT TO SAVE THE PARAMETER LIGHTCONTROLLER", FormMessageBox.MESSAGEBOX_TYPE.Info))
                {
                    //Global.Device.LIGHT.WriteInitFile();
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowdialogMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
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
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Exception ==> {Desc.Message}");
            }
        }

        private void btnConnection_Click(object sender, EventArgs e)
        {
            try
            {
                Global.Device.LIGHTS[nIndex].PortName = cbComport.Texts;
                Global.Device.LIGHTS[nIndex].BaudRate = int.Parse(cbBaudrate.Texts);

                if (!Global.Device.LIGHTS[nIndex].IsOpen)
                {
                    Global.Device.LIGHTS[nIndex].DisConnect();
                    Global.Device.LIGHTS[nIndex].Connect(Global.Recipe.Name, nIndex);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnDisConnection_Click(object sender, EventArgs e)
        {
            try
            {                
                Global.Device.LIGHTS[nIndex].DisConnect();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void timerConnected_Tick(object sender, EventArgs e)
        {
            if (Global.Device.LIGHTS.Count == 0) { return; }
            if (Global.Device.LIGHTS[nIndex].IsOpen) { CUtil_UI.UpdateLabelOnOff(lbStatusConnected, true); }
            else { CUtil_UI.UpdateLabelOnOff(lbStatusConnected, false); }
        }


        private void tgbChannle_Click(object sender, EventArgs e)
        {
            RJCodeUI_M1.RJControls.RJToggleButton pt = (RJCodeUI_M1.RJControls.RJToggleButton)sender;

            if (pt.Checked) { Global.Device.LIGHTS[nIndex].On(SelectedIndex); }
            else { Global.Device.LIGHTS[nIndex].Off(SelectedIndex); }           
        }

        private void btnSaveParameter_Click(object sender, EventArgs e)
        {
            try
            {
                if (CCommon.ShowdialogMessageBox("SAVE", "DO YOU WANT TO SAVE THE PARAMETER LIGHTCONTROLLER", FormMessageBox.MESSAGEBOX_TYPE.Info))
                {
                    Global.Device.LIGHTS[nIndex].ChannelCount = int.Parse(tbChannelCount.Text);
                    Global.Device.LIGHTS[nIndex].PortName = cbComport.SelectedItem.ToString();                    
                    Global.Device.LIGHTS[nIndex].BaudRate = int.Parse(cbBaudrate.SelectedItem.ToString());
                    Global.Device.LIGHTS[nIndex].WriteInitFile(Global.Recipe.Name);
                    Global.Device.LIGHTS[nIndex].ReadInitFile(Global.Recipe.Name);

                    for (int i = 0; i < btnList.Count; i++) { btnList[i].Enabled = false; }
                    for (int i = 0; i < Global.Device.LIGHTS[nIndex].ChannelCount; i++) { btnList[i].Enabled = true; }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowdialogMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void tgbAllChannle_CheckedChanged(object sender, EventArgs e)
        {
            RJCodeUI_M1.RJControls.RJToggleButton pt = (RJCodeUI_M1.RJControls.RJToggleButton)sender;

            if (pt.Checked)
            {
                Global.Device.LIGHTS[nIndex].AllOn();
                tgbChannle.Checked = true;
            }
            else
            {
                Global.Device.LIGHTS[nIndex].AllOff();
                tgbChannle.Checked = false;
            }
        }

        private void tbChannelCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            Lib.Common.CUtil_UI.txtInterval_KeyPress(sender, e);
        }

        private void trbLight_Scroll(object sender, EventArgs e)
        {
            try
            {
                Thread.Sleep(30);

                int LightValue = trbLight.Value;                

                if (tgbAllChangeVal.Checked)
                {
                    for (int i = 0; i < Global.Device.LIGHTS[nIndex].Values.Length; i++)
                    {
                        Global.Device.LIGHTS[nIndex].Write(i+1, LightValue);
                        Global.Device.LIGHTS[nIndex].Values[i] = LightValue;
                        Thread.Sleep(30);
                    }
                    Global.Device.LIGHTS[nIndex].AllOn();
                }
                else
                {
                    Global.Device.LIGHTS[nIndex].Write(SelectedIndex, LightValue);
                    Global.Device.LIGHTS[nIndex].Values[SelectedIndex - 1] = LightValue;
                    Thread.Sleep(30);   
                    Global.Device.LIGHTS[nIndex].On(SelectedIndex);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Exception ==> {Desc.Message}");
            }
        }

        private void btnChannel1_Click_1(object sender, EventArgs e)
        {

        }

        private int nIndex = 0;

        private void cbLightIndex_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            nIndex = cbLightIndex.SelectedIndex;

            tbChannelCount.Text = Global.Device.LIGHTS[nIndex].ChannelCount.ToString();

            cbComport.SelectedIndex = cbComport.Items.IndexOf(Global.Device.LIGHTS[nIndex].PortName.ToString());
            cbBaudrate.SelectedIndex = cbBaudrate.Items.IndexOf(Global.Device.LIGHTS[nIndex].BaudRate.ToString());

            tgbChannle.Checked = Global.Device.LIGHTS[nIndex].OnOffCheck[SelectedIndex - 1];
            trbLight.Value = Global.Device.LIGHTS[nIndex].Values[SelectedIndex - 1];

            for (int i = 0; i < btnList.Count; i++)
            {
                btnList[i].Click += FormSettings_Illumination_Click;
                btnList[i].Enabled = false;
            }

            for (int i = 0; i < Global.Device.LIGHTS[nIndex].ChannelCount; i++)
            {
                btnList[i].Enabled = true;
            }
            FormSettings_Illumination_Click(btnList[0], null);
        }

        private void btnSetLight_Click(object sender, EventArgs e)
        {
            try
            {
                if (CCommon.ShowdialogMessageBox("Edit", "Please Restart the application to view changes\nRestart now?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    Global.Device.LIGHT_COUNT = int.Parse(tbLightCount.Text);
                    Global.Device.SaveConfig();
                    Global.Close();

                    Application.Restart();
                    Environment.Exit(0);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowdialogMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void tgbAllChangeVal_CheckedChanged(object sender, EventArgs e)
        {
            RJCodeUI_M1.RJControls.RJToggleButton pt = (RJCodeUI_M1.RJControls.RJToggleButton)sender;

            if (pt.Checked)
            {
                
            }
            else
            {
               
            }
        }

        private void tgbChannle_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

