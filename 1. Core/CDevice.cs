using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Xml.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lib.Common;

namespace OpenVisionLab
{
    public class CDevice
    {
        public int CAMERA_COUNT { get; set; } = 4;
        public int LIGHT_COUNT { get; set; } = 1;

        private bool CloseProgram = false;
        
        [XmlIgnore]  public List<CGrabberMatrox> CAMERAS = new List<CGrabberMatrox>();
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
                }
                ThreadStatus.End();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                ThreadStatus.End();
            }
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
