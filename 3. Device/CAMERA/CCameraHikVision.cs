//#if HIKVISION
using MvCamCtrl.NET;
using OpenCvSharp;
using System;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Diagnostics;
using static MvCamCtrl.NET.MyCamera;
using System.Windows.Forms;
using static OpenVisionLab.CMvcGraph;
using System.Security.Cryptography;
using Lib.Common;
//using OpenVisionLab.Library;

namespace OpenVisionLab
{
    public class CCameraHikVision
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        static bool IsMonoPixelFormat(MyCamera.MvGvspPixelType enType)
        {
            switch (enType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;
                default:
                    return false;
            }
        }

        static bool IsColorPixelFormat(MyCamera.MvGvspPixelType enType)
        {
            switch (enType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                    return true;
                default:
                    return false;
            }
        }

        public CPropertyCamera Property { get; set; } = new CPropertyCamera("CAMERA");
        private MyCamera m_MyCamera = new MyCamera();
        public MyCamera.MV_FRAME_OUT_INFO_EX m_stFrameInfo;
        MyCamera.MV_CC_DEVICE_INFO_LIST m_stDeviceList;
        Thread m_hReceiveThread = null;


        public CCameraHikVision(int nIndex)
        {
            Property.NAME = $"CAMERA_{nIndex}";
            Property.INDEX = nIndex;

            DeviceListAcq();
        }

        bool m_bGrabbing = false;
        private static Object BufForDriverLock = new Object();
        private UInt32 m_nBufSizeForDriver = 0;
        private IntPtr m_BufForDriver;

        public Mat ImageGrab { get; set; } = new Mat();        

        public CViewer ImageManager { get; set; } = new CViewer();
        
        #region Event Register        
        public event EventHandler<GrabEventArgs> EventGrabEnd;
        public event EventHandler<EventArgs> EventChangedConnection;
        public EventHandler<StringEventArgs> EventLicense;
        public ManualResetEvent IsGrabDone = new ManualResetEvent(false);

        #endregion
        private bool m_bIsOpen = false;
        public bool IsOpen
        {
            get
            {
                return m_bIsOpen;
                //if (m_MyCamera != null)
                //{
                //    return m_MyCamera.MV_CC_IsDeviceConnected_NET();
                //}
                //else
                //{
                //    return false;
                //}
            }
            set { m_bIsOpen = value; }
        }
        public bool IsLive { get; set; } = false;
        private string m_strGateWay = "1";
        public string GateWay
        {
            get { return m_strGateWay; }
            set { m_strGateWay = value; }
        }
        private uint m_uTriggerMode = (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON;
        public uint TriggerMode
        {
            get { return m_uTriggerMode; }
            set { m_uTriggerMode = value; }

        }
        private float m_fLightBalance = 0.0f;
        public float LightBalance
        {
            get { return m_fLightBalance; }
            set { m_fLightBalance = value; }
        }
        public void Close()
        {
            try
            {
                Live(false);
                StopAcquisition();
                Disconnect();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public bool Init(string RecipeName, int nIndex = 0)
        {
            try
            {
                Property = new CPropertyCamera($"CAMERA_{nIndex}");
                Property.INDEX = nIndex;
                Property = Property.LoadConfig(RecipeName);
                Close();

                if (Connect() == false) { return false; }

                GC.Collect();
                Thread.Sleep(1);
                if (IsOpen == false)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Connect();
                        Thread.Sleep(10);
                        if (IsOpen)
                        {
                            break;
                        }
                    }

                    if (!IsOpen)
                    {
                        return false;
                    }
                }
                else
                {
                    //SetExposure(1000);
                    SetExposure(Property.EXPOSURETIME_US);
                    SetGain(Property.GAIN);
                    Thread.Sleep(200);
                    StartAcquisition();
                    CLOG.DEVICE( $"CAMERA_{nIndex} CONNECTED");
                    //if (m_MODE != CPropertyCamera.TRIGGER_MODE_TYPE.ON_HW) SetTriggerMode(CPropertyCamera.TRIGGER_MODE_TYPE.ON_HW);
                }

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }
        private void UpdateParameter()
        {
            try
            {
                System.GC.Collect();
                SetExposure(Property.EXPOSURETIME_US);
                SetGain(Property.GAIN);
                SetTriggerMode(Property.TRIGGER_MODE);

                return;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
        public void DeviceListAcq()
        {
            try
            {
                System.GC.Collect();
                m_stDeviceList.nDeviceNum = 7;
                int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_stDeviceList);
                if ((int)MyCamera.MV_OK != nRet)
                {
                    return;
                }

                return;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
        public bool Connect()
        {
            try
            {
                //return false;
                //IsOpen = true;

                IsOpen = false;
                m_MyCamera = new MyCamera();
                m_stDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();

                int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_stDeviceList);
                if (nRet != 0 || m_stDeviceList.nDeviceNum == 0)
                {
                    IsOpen = false;
                    //CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                    return false;
                }
                MyCamera.MV_CC_DEVICE_INFO device = new MyCamera.MV_CC_DEVICE_INFO();
                device.nTLayerType = MyCamera.MV_GIGE_DEVICE;

                if(m_stDeviceList.nDeviceNum <= Property.INDEX)
                {
                    IsOpen = false;
                    //CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                    return false;
                }

                device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[Property.INDEX], typeof(MyCamera.MV_CC_DEVICE_INFO));
                MyCamera.MV_GIGE_DEVICE_INFO Info = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                for (int i = 0; i < CGlobal.Inst.Device.CAMERA_COUNT; i++)
                {
                    device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                    Info = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                    Property.SERIAL_NUMBER = Info.chSerialNumber;
                    string DefinedName = Info.chUserDefinedName;

                    bool FindCam = false;

                    int CAM_INDEX = 0;
                    switch (DefinedName)
                    {
                        case "CAM1":
                            CAM_INDEX = 0;
                            break;
                        case "CAM2":
                            CAM_INDEX = 1;
                            break;
                        case "CAM3":
                            CAM_INDEX = 2;
                            break;
                        case "CAM4":
                            CAM_INDEX = 3;
                            break;
                        case "CAM5":
                            CAM_INDEX = 4;
                            break;
                        case "CAM6":
                            CAM_INDEX = 5;
                            break;
                        case "CAM7":
                            CAM_INDEX = 6;
                            break;
                    }

                    if (CAM_INDEX == Property.INDEX)
                    {
                        break;
                    }
                }

                if (EventLicense != null)
                {
                    string strSpecinfo = Info.chManufacturerSpecificInfo.Substring(8, Info.chManufacturerSpecificInfo.Length - 9);
                    strSpecinfo = strSpecinfo.Replace(" ", "");
                    EventLicense(null, new StringEventArgs(strSpecinfo));
                }
                nRet = m_MyCamera.MV_CC_CreateDevice_NET(ref device);
                if (MyCamera.MV_OK != nRet)
                {
                    return false;
                }
                nRet = m_MyCamera.MV_CC_OpenDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    m_MyCamera.MV_CC_DestroyDevice_NET();
                    return false;
                }

                MyCamera.MVCC_INTVALUE stIntVal = new MyCamera.MVCC_INTVALUE();
                nRet = m_MyCamera.MV_CC_GetIntValue_NET("Width", ref stIntVal);
                if (MyCamera.MV_OK != nRet)
                {
                    Console.WriteLine("Get width failed:{0:x8}", nRet);
                    return false;
                }

                Property.WIDTH = (int)stIntVal.nCurValue;

                stIntVal = new MyCamera.MVCC_INTVALUE();
                nRet = m_MyCamera.MV_CC_GetIntValue_NET("Height", ref stIntVal);
                if (MyCamera.MV_OK != nRet)
                {
                    Console.WriteLine("Get width failed:{0:x8}", nRet);
                    return false;
                }

                Property.HEIGHT = (int)stIntVal.nCurValue;

                //2022-06-07 BGR8변환 소스적용
                //uint enValue = (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed;
                //nRet = m_MyCamera.MV_CC_SetEnumValue_NET("PixelFormat", enValue);
                //여기까지
                ////2022-06-07 BGR8변환 소스적용
                //uint enValue = (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8;
                //nRet = m_MyCamera.MV_CC_SetEnumValue_NET("PixelFormat", enValue);
                ////여기까지
                ///
                                        //카메라의 Pixel Format을 BGR 8로 변경.
                //BGR 8
                //Enum Entry Name: BGR8Packed
                //Enum Entry Value: 0x02180015
                //uint nTemp = Convert.ToInt32(sTemp, 16);

                //uint enValue = (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
                //nRet = m_MyCamera.MV_CC_SetEnumValue_NET("PixelFormat", enValue);
                //if (MyCamera.MV_OK != nRet)
                //{
                //    Console.WriteLine("Set PixelFormat failed:{0:x8}", nRet);
                //    return false;
                //}

                //카메라의 Pixel Format을 확인
                MyCamera.MVCC_ENUMVALUE enumTemp = new MyCamera.MVCC_ENUMVALUE();
                nRet = m_MyCamera.MV_CC_GetEnumValue_NET("PixelFormat", ref enumTemp);
                if (MyCamera.MV_OK != nRet)
                {
                    Console.WriteLine("Set PixelFormat failed:{0:x8}", nRet);
                    return false;
                }

                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    int nPacketSize = m_MyCamera.MV_CC_GetOptimalPacketSize_NET();
                    if (nPacketSize > 0)
                    {
                        nRet = m_MyCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                        if (nRet != MyCamera.MV_OK)
                        {

                        }
                    }
                }
                SetTriggerMode(CPropertyCamera.TRIGGER_MODE_TYPE.ON_HW);
                //SetTriggerMode(CPropertyCamera.TRIGGER_MODE_TYPE.ON_SW);                
                //SetTriggerMode(CPropertyCamera.TRIGGER_MODE_TYPE.OFF);
                //SetTriggerMode(CPropertyCamera.TRIGGER_MODE_TYPE.ON_SW);
                IsOpen = true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                Disconnect();

                return false;
            }

            return true;
        }
        public bool ConnectionCheck()
        {
            try
            {
                //int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_stDeviceList);
                //if (nRet != 0 || m_stDeviceList.nDeviceNum == 0)
                //{
                //    CLog.Error( "[FAILED] {0}/{1}   Can't Find the Camrea", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                //    return false;
                //}
                
                MVCC_ENUMVALUE fValue = new MVCC_ENUMVALUE();

                int nRet = m_MyCamera.MV_CC_GetEnumValue_NET("ExposureAuto", ref fValue);
                               
                if (MyCamera.MV_OK != nRet)
                {
                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}/{ MethodBase.GetCurrentMethod().Name}   Can't Find the Camrea");
                    IsOpen = false;
                    return false;
                }

                //MyCamera.MVCC_INTVALUE stIntVal = new MyCamera.MVCC_INTVALUE();
                //int nRet = m_MyCamera.MV_CC_GetIntValue_NET("Width", ref stIntVal);
                //if (MyCamera.MV_OK != nRet)
                //{
                //    CLog.Error( "[FAILED] {0}/{1}   Can't Find the Camrea", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                //    IsOpen = false;
                //    return false;
                //}

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }
        public bool Disconnect()
        {
            try
            {
                if (m_MyCamera != null)
                {
                    m_MyCamera.MV_CC_StopGrabbing_NET();
                    m_MyCamera.MV_CC_CloseDevice_NET();
                    m_MyCamera.MV_CC_DestroyDevice_NET();
                }

                //m_stDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
                //m_stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
                //m_MyCamera = null;

                if (m_bGrabbing == true)
                {
                    m_bGrabbing = false;
                    m_hReceiveThread = null;
                }

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        private CPropertyCamera.TRIGGER_MODE_TYPE m_MODE = CPropertyCamera.TRIGGER_MODE_TYPE.ON_SW;

        public Stopwatch GrabTime = new Stopwatch();

        public bool Grab(bool bHw = true)
        {
            try
            {
                IsGrabDone.Reset();
                if (bHw)
                {
                    if (m_MODE != CPropertyCamera.TRIGGER_MODE_TYPE.ON_HW) SetTriggerMode(CPropertyCamera.TRIGGER_MODE_TYPE.ON_HW);
                }
                else
                {
                    if (m_MODE != CPropertyCamera.TRIGGER_MODE_TYPE.ON_SW) SetTriggerMode(CPropertyCamera.TRIGGER_MODE_TYPE.ON_SW);
                }

                int nRet = m_MyCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");
                nRet = m_MyCamera.MV_CC_SetCommandValue_NET("EncoderCounterReset");
                if (MyCamera.MV_OK != nRet)
                {
                    IsOpen = false;
                    CLOG.ABNORMAL( "Cam{0} - Disconnted",Property.INDEX);
                }
                //GrabTime = Stopwatch.StartNew();

                //CLog.Info( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                Disconnect();
                return false;
            }
            return true;
        }

        public void ClearEncoder()
        {        
            int nRet = m_MyCamera.MV_CC_SetCommandValue_NET("EncoderCounterReset");
            if (MyCamera.MV_OK != nRet)
            {
                IsOpen = false;
               // CLOG.ABNORMAL("Cam{0} - Disconnted", Property.INDEX);
            }
        }

        public int GetEncoder()
        {
            MyCamera.MVCC_INTVALUE stIntVal = new MyCamera.MVCC_INTVALUE();
            int nRet = m_MyCamera.MV_CC_GetIntValue_NET("EncoderCounter", ref stIntVal);
            if (MyCamera.MV_OK != nRet)
            {
                IsOpen = false;
               // CLOG.ABNORMAL("Cam{0} - Disconnted", Property.INDEX);
            }

            return (int)stIntVal.nCurValue;
        }
        
        public bool Live(bool bEnable)
        {
            try
            {
                IsLive = bEnable;
                if (IsLive == true)
                    SetTriggerMode(CPropertyCamera.TRIGGER_MODE_TYPE.OFF);
                else
                    SetTriggerMode(CPropertyCamera.TRIGGER_MODE_TYPE.ON_SW);

                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                Disconnect();
                return false;
            }

            return true;
        }
        
        public void SetCamPara()
        {
            SetExposure(Property.EXPOSURETIME_US);
            SetGain(Property.GAIN);
        }

        public void SetExposure(float fValue)
        {
            try
            {
                m_MyCamera.MV_CC_SetEnumValue_NET("ExposureAuto", (uint)fValue);
                int nRet = m_MyCamera.MV_CC_SetFloatValue_NET("ExposureTime", fValue);
                if (nRet != MyCamera.MV_OK)
                {

                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
        public void SetGain(float fValue)
        {
            try
            {
                //m_MyCamera.MV_CC_SetEnumValue_NET("GainAuto", (uint)Gain);
                int nRet = m_MyCamera.MV_CC_SetFloatValue_NET("Gain", fValue);
                if (nRet != MyCamera.MV_OK)
                {

                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public void SetWidth(uint Value)
        {
            try
            {
                int nRet = m_MyCamera.MV_CC_SetIntValue_NET("Width", Value);
                //int nRet = m_MyCamera.MV_CC_SetIntValue_NET("Width", Value);
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
            
        }

        public void SetHeight(uint Value)
        {
            try
            {
                int nRet = m_MyCamera.MV_CC_SetIntValue_NET("Height", Value);
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }            
        }

        public void SetIpConfig(string strIp, string strGateWay)
        {
            try
            {
                string[] strArrGatWay = strIp.Split('.');

                string strResultGateWay = strArrGatWay[0] + "." + strArrGatWay[1] + "." + strArrGatWay[2] + "." + strGateWay;

                // ch:IP转换 | en:IP conversion
                IPAddress clsIpAddr;
                if (false == IPAddress.TryParse(strIp, out clsIpAddr))
                {
                    return;
                }

                long nIp = IPAddress.NetworkToHostOrder(clsIpAddr.Address);

                // ch:掩码转换 | en:Mask conversion
                IPAddress clsSubMask;
                if (false == IPAddress.TryParse("255.255.255.0", out clsSubMask))
                {
                    return;
                }
                long nSubMask = IPAddress.NetworkToHostOrder(clsSubMask.Address);

                // ch:网关转换 | en:Gateway conversion

                IPAddress clsDefaultWay;
                if (false == IPAddress.TryParse(strGateWay, out clsDefaultWay))
                {
                    return;
                }
                long nDefaultWay = IPAddress.NetworkToHostOrder(clsDefaultWay.Address);

                if (m_stDeviceList.pDeviceInfo[0] == IntPtr.Zero)
                {
                    return;
                }

                MyCamera.MV_CC_DEVICE_INFO device =
                (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[0],
                                                      typeof(MyCamera.MV_CC_DEVICE_INFO));

                int nRet = m_MyCamera.MV_CC_CreateDevice_NET(ref device);
                if (MyCamera.MV_OK != nRet)
                {
                    return;
                }

                nRet = m_MyCamera.MV_GIGE_ForceIpEx_NET((uint)(nIp >> 32), (uint)(nSubMask >> 32), (uint)(nDefaultWay >> 32));
                if (MyCamera.MV_OK != nRet)
                {
                    return;
                }
                GC.Collect();

            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
        public void StartAcquisition()
        {
            try
            {
                m_bGrabbing = true;
                m_hReceiveThread = new Thread(ThreadReceiveBuffer);
                m_hReceiveThread.Start();
                m_stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
                m_stFrameInfo.nFrameLen = 0;
                m_stFrameInfo.enPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;
                int nRet = m_MyCamera.MV_CC_StartGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    m_bGrabbing = false;
                    m_hReceiveThread.Join();
                    //Logger.WriteLog(LOG.ERR, "[FAILED] {0}/{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                    return;
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
        public void StopAcquisition()
        {
            try
            {
                m_bGrabbing = false;
                if (m_hReceiveThread == null) { return; }
                bool bRet = m_hReceiveThread.Join(1000);
                if (bRet == true)
                {
                    string str = string.Empty;
                }
                else
                {
                    string str = string.Empty;
                }
                int nRet = m_MyCamera.MV_CC_StopGrabbing_NET();
                if (nRet != MyCamera.MV_OK)
                {
                    //Logger.WriteLog(LOG.ERR, "[FAILED] {0}/{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public static Bitmap ByteToImage(byte[] blob)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                mStream.Write(blob, 0, blob.Length);
                mStream.Seek(0, SeekOrigin.Begin);

                Bitmap bm = new Bitmap(mStream);
                return bm;
            }
        }

        UInt32 nSaveImageNeedSize = 0;
        public void ThreadReceiveBuffer()
        {
            try
            {
                IntPtr pBufForConvert = IntPtr.Zero;
                MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
                int nRet = m_MyCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    //Logger.WriteLog(LOG.ERR, "[FAILED] {0}/{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                    return;
                }
                UInt32 nPayloadSize = stParam.nCurValue;
                if (nPayloadSize > m_nBufSizeForDriver)
                {
                    if (m_BufForDriver != IntPtr.Zero)
                    {
                        Marshal.Release(m_BufForDriver);
                    }
                    m_nBufSizeForDriver = nPayloadSize;
                    m_BufForDriver = Marshal.AllocHGlobal((Int32)m_nBufSizeForDriver);
                }
                if (m_BufForDriver == IntPtr.Zero) { return; }
                MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();

                while (m_bGrabbing)
                {
                    Thread.Sleep(1);

                    nRet = m_MyCamera.MV_CC_GetOneFrameTimeout_NET(m_BufForDriver, nPayloadSize, ref stFrameInfo, 300);
                    if (nRet == MyCamera.MV_OK)
                    {
                        m_stFrameInfo = stFrameInfo;
                    }

                    if (nRet == MyCamera.MV_OK)
                    {
                        if (RemoveCustomPixelFormats(stFrameInfo.enPixelType))
                        {
                            continue;
                        }

                        // MVS 버그인거같은데 소프트웨어 모드에서 하드웨어(펄스모드)로 변경시에 GRAB이 한번 더 들어옴
                        if(changeHWRetrun && m_MODE == CPropertyCamera.TRIGGER_MODE_TYPE.ON_SW)
                        {
                            changeHWRetrun = false;
                            continue; 
                        }
                        
                        Property.WIDTH = stFrameInfo.nWidth;
                        Property.HEIGHT = stFrameInfo.nHeight;

                        int[] sz = new int[2] { Property.HEIGHT, Property.WIDTH };

                        try
                        {
                           // CLOG.NORMAL($"Grab Time : {GrabTime.ElapsedMilliseconds}ms");

                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            ImageGrab = new Mat(sz, MatType.CV_8UC1, m_BufForDriver);

                          //  CLOG.NORMAL($"이미지 변환 Time : {GrabTime.ElapsedMilliseconds}ms");

                            if (Property.USE_FLIP)
                            {
                                Cv2.Flip(ImageGrab, ImageGrab, Property.FLIP);
                            }

                            if (Property.USE_ROTATE)
                            {
                                Cv2.Rotate(ImageGrab, ImageGrab, Property.ROTATE);
                            }

                            if (ImageGrab.Channels() != 1) Cv2.CvtColor(ImageGrab, ImageGrab, ColorConversionCodes.RGB2GRAY);                         
                            IsGrabDone.Set();

                            // CLOG.NORMAL($"이미지 채널 변경 Time : {GrabTime.ElapsedMilliseconds}ms");
                            
                            if ((CGlobal.Inst.Data.IsSaveImage))
                            {
                                Random random = new Random();
                                string FilePath = Application.StartupPath + $"\\Sceen\\{Property.INDEX}_{DateTime.Now.ToString("HHmmsss")}_{random.Next(-10000, 10000)}.bmp";
                                CImageManager.SaveImages.Queues.Enqueue(new Images(Lib.Common.CImageConverter.ToBitmap(ImageGrab.Clone()), FilePath, ImageFormat.Bmp));
                            }

                            //CGlobal.Inst.SeqVision.RunInspection(new CGrabBuffer(ImageGrab, Property.INDEX));

                            int getEncoder = CGlobal.Inst.Device.ENC600.GetEncoder();

                            if (EventGrabEnd != null)
                            {
                                //EventGrabEnd(this, new GrabEventArgs(m_ImageGrab, m_nIndex));
                                EventGrabEnd(this, new GrabEventArgs(ImageGrab, Property.INDEX, getEncoder));
                            }

                           // CLOG.NORMAL($"이미지 헨들러 이벤트 발생 Time : {GrabTime.ElapsedMilliseconds}ms");

                            string d = sw.ElapsedMilliseconds.ToString();

                            GrabTime.Stop();
                            CLOG.NORMAL($"Grab Success : {Property.INDEX+1}-Cam");

                            //MyCamera.MVCC_FLOATVALUE stFloatVal = new MyCamera.MVCC_FLOATVALUE();
                            //nRet = m_MyCamera.MV_CC_GetFloatValue_NET("AcquisitionFrameRate", ref stFloatVal);
                            //if (MyCamera.MV_OK != nRet)
                            //{
                            //    Console.WriteLine("Get AcquisitionFrameRate failed:{0:x8}", nRet);
                            //    //return nRet;
                            //}
                            //Console.WriteLine("Current AcquisitionFrameRate:{0:f}Fps", stFloatVal.fCurValue);
                            //CLOG.NORMAL($"Cam{Property.INDEX} - Encoder{GetEncoder()}");
                            if (!IsLive)
                            {
                                if (m_MODE != CPropertyCamera.TRIGGER_MODE_TYPE.ON_HW)
                                {                                    
                                    SetTriggerMode(CPropertyCamera.TRIGGER_MODE_TYPE.ON_HW);
                                }
                                    
                            }
                        }
                        catch (Exception Desc)
                        {
                            CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                        }

                    }
                }

                //while (m_bGrabbing)
                //{
                //    Thread.Sleep(10);
                //    MyCamera.MV_FRAME_OUT stFrameOut = new MyCamera.MV_FRAME_OUT();
                //    lock (BufForDriverLock)
                //    {
                //        //nRet = m_MyCamera.MV_CC_GetOneFrameTimeout_NET(m_BufForDriver, nPayloadSize, ref stFrameInfo, 100);
                //        //if (nRet == MyCamera.MV_OK)
                //        //{
                //        //    m_stFrameInfo = stFrameInfo;
                //        //}
                //        nRet = m_MyCamera.MV_CC_GetImageBuffer_NET(ref stFrameOut, 1000);
                //    }

                //    // 获取一帧图像
                //    if (MyCamera.MV_OK == nRet)
                //    {
                //        Console.WriteLine("Get One Frame:" + "Width[" + Convert.ToString(stFrameOut.stFrameInfo.nWidth) + "] , Height["
                //            + Convert.ToString(stFrameOut.stFrameInfo.nHeight) + "] , FrameNum[" + Convert.ToString(stFrameOut.stFrameInfo.nFrameNum) + "]");

                //        MyCamera.MvGvspPixelType enType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;
                //        uint nChannelNum = 0;
                //        if (IsColorPixelFormat(stFrameOut.stFrameInfo.enPixelType))
                //        {
                //            enType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
                //            nChannelNum = 3;
                //        }
                //        else if (IsMonoPixelFormat(stFrameOut.stFrameInfo.enPixelType))
                //        {
                //            enType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
                //            nChannelNum = 1;
                //        }
                //        else
                //        {
                //            Console.WriteLine("Don't need to convert!");
                //        }

                //        if (enType != MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined)
                //        {
                //            if (pBufForConvert == IntPtr.Zero)
                //            {
                //                pBufForConvert = Marshal.AllocHGlobal((Int32)stFrameOut.stFrameInfo.nFrameLen);
                //                //pBufForConvert = Marshal.AllocHGlobal((int)(stFrameOut.stFrameInfo.nWidth * stFrameOut.stFrameInfo.nHeight * nChannelNum));
                //            }

                //            CopyMemory(pBufForConvert, stFrameOut.pBufAddr, stFrameOut.stFrameInfo.nFrameLen);

                //            MyCamera.MV_SAVE_IMG_TO_FILE_PARAM stSaveFileParam = new MyCamera.MV_SAVE_IMG_TO_FILE_PARAM();

                //            lock (BufForDriverLock)
                //            {
                //                if (stFrameOut.stFrameInfo.nFrameLen == 0)
                //                {
                //                    //ShowErrorMsg("Save Bmp Fail!", 0);
                //                    return;
                //                }

                //                stSaveFileParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Bmp;
                //                stSaveFileParam.enPixelType = stFrameOut.stFrameInfo.enPixelType;
                //                stSaveFileParam.pData = pBufForConvert;
                //                stSaveFileParam.nDataLen = stFrameOut.stFrameInfo.nFrameLen;
                //                stSaveFileParam.nHeight = stFrameOut.stFrameInfo.nHeight;
                //                stSaveFileParam.nWidth = stFrameOut.stFrameInfo.nWidth;
                //                stSaveFileParam.iMethodValue = 2;
                //                stSaveFileParam.pImagePath = "Image_w" + stSaveFileParam.nWidth.ToString() + "_h" + stSaveFileParam.nHeight.ToString() + "_fn" + m_stFrameInfo.nFrameNum.ToString() + ".bmp";
                //                nRet = m_MyCamera.MV_CC_SaveImageToFile_NET(ref stSaveFileParam);
                //                if (MyCamera.MV_OK != nRet)
                //                {
                //                    //ShowErrorMsg("Save Bmp Fail!", nRet);
                //                    return;
                //                }

                //                using (FileStream fsln = new FileStream(stSaveFileParam.pImagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                //                {

                //                    try
                //                    {
                //                        Image source = Image.FromStream(fsln);

                //                        //source.Save(stSaveFileParam.pImagePath);

                //                        source.RotateFlip(RotateFlipType.Rotate180FlipY);

                //                        Bitmap bmpTemp = new Bitmap(source);

                //                        Bitmap bit8 = Convert24bppTo8bpp((Bitmap)source);
                //                        bit8.Save("bmp_Mono.bmp");

                //                        ImageGrab = Lib.Common.CImageConverter.ToMat(bit8);

                //                        ImageGrabColor = Lib.Common.CImageConverter.ToMat(bmpTemp);
                //                        //Cv2.Flip(ImageGrab, ImageGrab, FlipMode.Y);

                //                        IsGrabDone.Set();

                //                        if (EventGrabEnd != null) { EventGrabEnd(this, new GrabEventArgs(ImageGrab, ImageGrabColor, 0)); }
                //                        //bmpTemp.Dispose();

                //                        //File.Delete(stSaveFileParam.pImagePath);
                //                    }
                //                    catch
                //                    {

                //                    }

                //                }
                //            }
                //        }
                //        m_MyCamera.MV_CC_FreeImageBuffer_NET(ref stFrameOut);
                //    }
                //    else
                //    {
                //        Console.WriteLine("Get Image failed:{0:x8}", nRet);
                //    }
                //}
                return;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        static public Bitmap Convert24bppTo8bpp(Bitmap in24bppImage)
        {
            if (in24bppImage != null)
            {
                //   
                Rectangle rect = new Rectangle(0, 0, in24bppImage.Width, in24bppImage.Height);
                BitmapData bmpData = in24bppImage.LockBits(rect, ImageLockMode.ReadOnly, in24bppImage.PixelFormat);

                int width = bmpData.Width;
                int height = bmpData.Height;
                int stride = bmpData.Stride;
                int offset = stride - width * 3;
                IntPtr ptr = bmpData.Scan0;
                int scanBytes = stride * height;

                int posScan = 0, posDst = 0;
                byte[] rgbValues = new byte[scanBytes];
                Marshal.Copy(ptr, rgbValues, 0, scanBytes);

                byte[] grayValues = new byte[width * height];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        double temp = rgbValues[posScan++] * 0.11 +
                            rgbValues[posScan++] * 0.59 +
                            rgbValues[posScan++] * 0.3;
                        grayValues[posDst++] = (byte)temp;
                    }

                    // length = stride - width * bytePerPixel  
                    posScan += offset;
                }

                Marshal.Copy(rgbValues, 0, ptr, scanBytes);
                in24bppImage.UnlockBits(bmpData);

                // 8  
                Bitmap retBitmap = BuiltGrayBitmap(grayValues, width, height);
                return retBitmap;
            }
            else
            {
                return null;
            }
        }

        static private Bitmap BuiltGrayBitmap(byte[] rawValues, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                 ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            int offset = bmpData.Stride - bmpData.Width;
            IntPtr ptr = bmpData.Scan0;
            int scanBytes = bmpData.Stride * bmpData.Height;
            byte[] grayValues = new byte[scanBytes];


            int posSrc = 0, posScan = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    grayValues[posScan++] = rawValues[posSrc++];
                }

                posScan += offset;
            }


            Marshal.Copy(grayValues, 0, ptr, scanBytes);
            bitmap.UnlockBits(bmpData);  //   


            ColorPalette palette;

            // Format8bppIndexedPalette  
            using (Bitmap bmp = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed))
            {
                palette = bmp.Palette;
            }
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
            }
            bitmap.Palette = palette;

            return bitmap;
        }


        private Boolean IsColorData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR411_8_CBYYCRYY:
                    return true;

                default:
                    return false;
            }
        }
        bool changeHWRetrun = false;
        public bool SetTriggerMode(CPropertyCamera.TRIGGER_MODE_TYPE type)
        {
            try
            {
                m_MODE = type;
                int nRet = -1;

                //MyCamera.MVCC_ENUMVALUE enValue = new MyCamera.MVCC_ENUMVALUE();
                //m_MyCamera.MV_CC_GetEnumValue_NET("TriggerMode", ref enValue);

                nRet = m_MyCamera.MV_CC_SetEnumValue_NET("EncoderSourceA", (uint)128); // NA
                nRet = m_MyCamera.MV_CC_SetEnumValue_NET("EncoderSourceB", (uint)128); // NA
                nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)0); // A상, B상 EncoderModuleOut

                switch (type)
                {
                    case CPropertyCamera.TRIGGER_MODE_TYPE.OFF: // LIVE
                        nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSelector", (uint)6); // FrameBurstStart
                        nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                        nRet = m_MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
                        break;
                    case CPropertyCamera.TRIGGER_MODE_TYPE.ON_SW: // SOFTWARE Trigger                        
                        nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSelector", (uint)6); // FrameBurstStart
                        nRet = m_MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);                        
                        nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
                        nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);                                                
                        //nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                        //nRet = m_MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);                        
                        //nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);                        
                        //nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0);
                        break;
                    case CPropertyCamera.TRIGGER_MODE_TYPE.ON_HW:// HARDWARE Trigger
                        nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSelector", (uint)9); // LINE 스타트 LineStart
                        nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);                                                
                        nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)6); // A상, B상 EncoderModuleOut
                        nRet = m_MyCamera.MV_CC_SetEnumValue_NET("EncoderSourceA", (uint)0); // A상
                        nRet = m_MyCamera.MV_CC_SetEnumValue_NET("EncoderSourceB", (uint)1); // B상
                        changeHWRetrun = true;
                        //nRet = m_MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
                        break;
                }

                if (nRet != 0)
                {
                    CLOG.DEVICE( $"CAMERA_{Property.INDEX} TRIGGER MODE SET ERROR");
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }

            return true;
        }
        private bool RemoveCustomPixelFormats(MyCamera.MvGvspPixelType enPixelFormat)
        {
            Int32 nResult = ((int)enPixelFormat) & (unchecked((Int32)0x80000000));
            if (0x80000000 == nResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void OnDisconnected()
        {
            try
            {
                if (EventChangedConnection != null)
                {
                    EventChangedConnection(this, null);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return;
            }
        }
    }
}

//#endif