using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Xml.Serialization;
using Vila.Extensions;
using OpenVisionLab.Common;
using OpenCvSharp;
using System.Collections;
using System.Threading;
using System.Security.Cryptography;
using System.Threading.Tasks;
using OpenVisionLab;
using OpenVisionLab._3._Device.DB;
using Lib.Common;

namespace OpenVisionLab
{
    public class CDevice
    {
        public int CAMERA_COUNT { get; set; } = 4;
        public int LIGHT_COUNT { get; set; } = 1;

        private bool CloseProgram = false;
        
        [XmlIgnore]  public List<CGrabberMatrox> CAMERAS = new List<CGrabberMatrox>();
        [XmlIgnore] public CDIO_CONTEC DIO_BD { get; set; } = new CDIO_CONTEC();
        [XmlIgnore] public List<CLightControler_KVP400> LIGHTS { get; set; } = new List<CLightControler_KVP400>();
        [XmlIgnore] public CMOTION_AJIN_ENC600 ENC600 { get; set; } = new CMOTION_AJIN_ENC600();
        [XmlIgnore] public CDIO_PLC DIO_PLC { get; set; } = new CDIO_PLC();
        [XmlIgnore] public CMariaSQL DB { get; set; } = new CMariaSQL();
        public CDevice() { }
        ~CDevice() { }      
        
        public bool Init()
        {
            try
            {                
                CUtil.InitDirectory("CONFIG");
                CUtil.InitDirectory("CONFIG\\DEVICE");

                try
                {
                    //DB.OpenConnection();
                }
                catch (Exception Desc)
                {
                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                }

                try
                {
                   //if (!DIO_BD.Init("DIO000")) { CCommon.ShowMessageBox("ALARM", "Failed the Init I/O", FormMessageBox.MESSAGEBOX_TYPE.Waring); }
                }
                catch (Exception Desc)
                {
                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                }

                try
                {
#if MX_COMPONENT
                if (!DIO_PLC.Init()) { CCommon.ShowMessageBox("ALARM", "Failed the Init ComMelsec"); } 
#endif
                   //if (!DIO_PLC.Init(CGlobal.Inst.Recipe.Name)) { CCommon.ShowMessageBox("ALARM", "Failed the Init PLC", FormMessageBox.MESSAGEBOX_TYPE.Waring); }
                }
                catch (Exception Desc)
                {
                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                }

                try
                {                 
                    //for(int i= 0; i < LIGHT_COUNT; i++)
                    //{
                    //    LIGHTS.Add(new CLightControler_KVP400());
                    //    if (!LIGHTS[i].Connect(CGlobal.Inst.Recipe.Name, i)) { CCommon.ShowMessageBox("ALARM", "Failed the Init LIGHT", FormMessageBox.MESSAGEBOX_TYPE.Waring); }
                    //}                    
                }
                catch (Exception Desc)
                {
                    CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                }

                try
                {
                    //if (!ENC600.Init()) { CCommon.ShowMessageBox("ALARM", "Failed the Init Encoder", FormMessageBox.MESSAGEBOX_TYPE.Waring); }
                }
                catch (Exception Desc)
                {
                    CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                }

                try
                {
                    //for (int i = 0; i < CAMERA_COUNT; i++)
                    //{
                    //    CGrabberMatrox camera = new CGrabberMatrox(CGlobal.Inst.Recipe.Name, i);
                    //    camera.Init();                        
                    //    CAMERAS.Add(camera);                        
                    //}
                }
                catch (Exception Desc)
                {
                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                }

              //  StartThreadDeviceConnected();

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool LoadDevices(string RecipeName)
        {
            try
            {                
                for (int i = 0; i < CAMERAS.Count; i++)
                {
                    CAMERAS[i].Property.LoadConfig(RecipeName);
                }
                for (int i = 0; i < LIGHTS.Count; i++)
                {
                    LIGHTS[i].ReadInitFile(RecipeName);
                }
                
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }

        }

        public bool Close()
        {
            try
            {
                CUtil.InitDirectory("CONFIG");
                CUtil.InitDirectory("CONFIG\\DEVICE");

                CloseProgram = true;
                StopThreadDeviceConnected();
                for (int i = 0; i < CAMERAS.Count; i++) { CAMERAS[i].Close(); }
                for (int i = 0; i < LIGHTS.Count; i++) { LIGHTS[i].DisConnect(); }
                DIO_BD.Close();                
                DIO_PLC.Close();
                DIO_PLC.StopThreadRead();
                ENC600.StopThreadGetEncoder();
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                return false;
            }

        }

        private CThreadStatus ThreadStatusDeviceConnected { get; set; } = new CThreadStatus();
        private object m_ob = new object();

        public void StartThreadDeviceConnected()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadDeviceConnted));
            t.Start(ThreadStatusDeviceConnected);
        }

        public void ResetThreadDeviceConnected()
        {
            ThreadStatusDeviceConnected.End();
        }

        public void StopThreadDeviceConnected()
        {
            ThreadStatusDeviceConnected.Stop(100);
        }

        private async void ThreadDeviceConnted(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("Device Connected Thread Start");
            CLOG.NORMAL("Device Connected Thread Start");

            try
            {
                while (!ThreadStatus.IsExit())
                {
                    await Task.Delay(5000);                    
                    for (int i = 0; i < CAMERAS.Count; i++)
                    {
                        if (CloseProgram)
                        {
                            ThreadStatus.End();
                            continue;
                        }
                        //if (!CAMERAS[i].ConnectionCheck())
                        //{
                        //    ThreadPool.QueueUserWorkItem(new WaitCallback(OpenDeviceCam), i);
                        //}
                    }

                    for (int i = 0; i < LIGHTS.Count; i++)
                    {
                        if (CloseProgram)
                        {
                            ThreadStatus.End();
                            continue;
                        }
                        if (!LIGHTS[i].IsOpen)
                        {
                            ThreadPool.QueueUserWorkItem(new WaitCallback(OpenDeviceLight), i);
                        }
                    }
                }
                ThreadStatus.End();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                ThreadStatus.End();
            }
        }

        public void OpenDeviceCam(Object obj)
        {
            int Cam = (int)obj;

            //CAMERAS[Cam].Init();
        }

        public void OpenDeviceLight(Object obj)
        {
            int Light = (int)obj;

            LIGHTS[Light].DisConnect();
            LIGHTS[Light].Connect(CGlobal.Inst.Recipe.Name, Light);
        }


        public CDevice LoadConfig()
        {
            string strPath = Application.StartupPath + "\\CONFIG\\DEVICE\\DEVICE.xml";
            CDevice newData = null;

            CUtil.InitDirectory("CONFIG");
            CUtil.InitDirectory("CONFIG\\DEVICE");

            if (File.Exists(strPath))
            {
                newData = SerializeHelper.FromXmlFile<CDevice>(strPath);
                if (newData != null)
                    return newData;
            }
            this.SaveConfig();
            return newData = LoadConfig();
        }

        public void SaveConfig()
        {
            string strPath = Application.StartupPath + "\\CONFIG\\DEVICE\\DEVICE.xml";
            SerializeHelper.ToXmlFile(strPath, this);
        }
    }
}
