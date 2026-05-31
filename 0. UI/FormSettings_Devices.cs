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


//IF 전용 Library
using OpenCvSharp;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;

using MetroFramework;
using MetroFramework.Forms;
using MetroFramework.Controls;

namespace KtemVisionSystem
{
    public partial class FormSettings_Devices : MetroForm
    {
        CGlobal Global = CGlobal.Inst;

        #region Event Register        
        public EventHandler<EventArgs> EventUpdateUi;
        #endregion
        public FormSettings_Devices()
        {
            InitializeComponent();           
        }

        private void FormSettings_Device_Load(object sender, EventArgs e)
        {
            
        }

        private void FormSettings_Devices_Shown(object sender, EventArgs e)
        {
            try
            {
                string[] ComportList = CUtil.AvalibleComports();

                if (ComportList.Length > 0)
                {
                    cbPortName.Items.AddRange(ComportList);
                    cbPortName.SelectedIndex = 0;
                }

                if (cbBaudrate.Items.Count > 0) cbBaudrate.SelectedIndex = 0;

                //foreach (var Comport in cbPortName.Items)
                //{
                //    if (Global.Device.LIGHT.PortNm == Comport)
                //    {
                //        cbPortName.SelectedItem = Comport;
                //    }
                //}

                //foreach (var Baudrate in cbBaudrate.Items)
                //{
                //    if (Global.Device.LIGHT.BaudRate == int.Parse(Baudrate.ToString()))
                //    {
                //        cbBaudrate.SelectedItem = Baudrate.ToString();
                //    }
                //}
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void InitLightChannel()
        {
            //for (int i = 0; i < Global.Device.LIGHT.ChannelCount; i++) { cbLightCh.Items.Add(i + 1); }
            if (cbLightCh.Items.Count > 0) { cbLightCh.SelectedIndex = 0; }
        }

        private bool InitEvent()
        {
            try
            {                
                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
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
                    CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            else
            {
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    int nChannel = int.Parse(cbLightCh.Text);
            //    //Global.iDevice.LIGHTCON_JSL_200.On(nChannel);
            //}
            //catch (Exception ex)
            //{
            //    CLog.Error( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            //    CUtil.ShowMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
            //}            
        }

        private void btnSaveCameraParam_Click(object sender, EventArgs e)
        {
            try
            {
                if(CUtil.ShowdialogMessageBox("SAVE", "DO YOU WANT TO SAVE THE PARAMETER OF CAMERA", FormMessageBox.MESSAGEBOX_TYPE.Info))
                {
                    for (int i = 0; i < Global.Device.CAMERAS.Count; i++)
                    {
                        //Global.iDevice.CAMERAS[i].Live(false);
                        Global.Device.CAMERAS[i].SetExposure(Global.Device.CAMERAS[i].Property.EXPOSURETIME_US);
                        Global.Device.CAMERAS[i].SetGain(Global.Device.CAMERAS[i].Property.GAIN);
                        Global.Device.CAMERAS[i].Property.SaveConfig(Global.Recipe.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowdialogMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
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
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowdialogMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void trbLightValue_Scroll(object sender, ScrollEventArgs e)
        {
            //try
            //{
            //    int LightValue = trbLightValue.Value;
            //    lbLightValue.Text = LightValue.ToString();
            //    int Channel = int.Parse(cbLightCh.SelectedItem.ToString());
            //    Global.iDevice.LIGHT.SetValue(Channel, LightValue);
            //}
            //catch (Exception ex)
            //{
            //    CLog.Error( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            //}
        }

        PerformanceCounter Cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        PerformanceCounter Ram = new PerformanceCounter("Memory", "Available MBytes");
        private void timerStatus_Tick(object sender, EventArgs e)
        {
            try
            {
                //if (Global.Device.DIO_PLC.Binary.Connected) { lbConnectePLC.BackColor = Color.LimeGreen; }
                //else { lbConnectePLC.BackColor = Color.Red; }

                //if (Global.iDevice.LIGHT.IsOpen) { lbConnectionLight.BackColor = Color.LimeGreen; }
                //else { lbConnectionLight.BackColor = Color.Red; }

                try
                {
                    double dDrivePercentC = CUtil.DrivePercent("C:\\", out double dCDriveTotalSize, out double dCDriveUsedSize);
                    double dDrivePercentD = CUtil.DrivePercent("D:\\", out double dDDriveTotalSize, out double dDDriveUsedSize);

                    lbDriveC.Text = $"Drive (C:) : {dDrivePercentC.ToString("F1")}%  ({dCDriveUsedSize.ToString("F1")}/ {dCDriveTotalSize.ToString("F1")} GB)";
                    lbDriveD.Text = $"Drive (D:) : {dDrivePercentD.ToString("F1")}%  ({dDDriveUsedSize.ToString("F1")}/ {dDDriveTotalSize.ToString("F1")} GB)";

                    pgbDriveC.Value = (int)dDrivePercentC;
                    pgbDriveD.Value = (int)dDrivePercentD;

                    float CpuUse = Cpu.NextValue();
                    float RamUse = Ram.NextValue();

                    if ((int)CpuUse <= 50)
                        cpuUsageLb.ForeColor = Color.Green;
                    else if ((int)CpuUse > 50 && (int)CpuUse <= 75)
                        cpuUsageLb.ForeColor = Color.Orange;
                    else if ((int)CpuUse > 75)
                        cpuUsageLb.ForeColor = Color.Red;

                    cpuUsageLb.Text = Convert.ToString((int)CpuUse) + " %";
                    ramLb.Text = Convert.ToString(RamUse) + " MB";
                }
                catch (Exception Desc)
                {

                }
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void lbConnecte_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!Global.Device.DIO_PLC.Binary.Connected)
                //{
                //    Global.Device.DIO_PLC.Binary.Open();
                //}
                //else
                //{
                //    Global.Device.DIO_PLC.Binary.Close();
                //    Global.Device.DIO_PLC.Binary.Open();
                //}
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void metroTile4_Click(object sender, EventArgs e)
        {

        }

        private void btnCloseOptionParam_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnAutoRevision(object sender, EventArgs e)
        {

        }

        private void btnFindDirectory_Click(object sender, EventArgs e)
        {
            OpenFolderPath(out string strPath);

            try
            {
                if(strPath == "")
                {
                    return;
                }

                tbDirectoryBackup.Text = strPath;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        private bool OpenFolderPath(out string strdirPath)
        {
            strdirPath = "";
            try
            {
                using(FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();
                    
                    if(result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        strdirPath = fbd.SelectedPath;
                    }
                }
                CLOG.NORMAL( "[OK] {0}/{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;

            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        private void metroLabel7_Click(object sender, EventArgs e)
        {
            OpenFolderPath(out string strPath);
            try
            {
                if (strPath == "")
                {
                    return;
                }

                tbDirectoryBackup2.Text = strPath;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnOptionPara_Click(object sender, EventArgs e)
        {
        
        }

        private void btnTablePara_Click(object sender, EventArgs e)
        {
           
        }

        private void btnSaveComPara_Click(object sender, EventArgs e)
        {
            try
            {               
                //Global.Device.DIO_PLC.SetAddress(tbServerIP.Text, int.Parse(tbServerPORT.Text));
                //Global.Device.DIO_PLC.SaveConfig();


                //Global.System.Recipe.SaveConfig();

            }

            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void OnClickOption(object sender, EventArgs e)
        {
            
        }

        private void lbConnectePLC_Click(object sender, EventArgs e)
        {

        }

        private void btnON_Click(object sender, EventArgs e)
        {
            try
            {
                //Global.Device.LIGHT.AllOn();
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowdialogMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void btnOFF_Click(object sender, EventArgs e)
        {
            try
            {
                //Global.Device.LIGHT.AllOff();
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowdialogMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
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
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void metroTile25_Click(object sender, EventArgs e)
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
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                CUtil.ShowdialogMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }
        }

        private void trbLightValue_Scroll_1(object sender, ScrollEventArgs e)
        {
            try
            {
                int LightValue = trbLightValue.Value;
                lbLightValue.Text = LightValue.ToString();
                int Channel = int.Parse(cbLightCh.SelectedItem.ToString());
                //Global.Device.LIGHT.SetValue(Channel, LightValue);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
 }

