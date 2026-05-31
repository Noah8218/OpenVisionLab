using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Basler.Pylon;
using OpenCvSharp;

namespace KtemVisionSystem
{
    public class CCameraBasler
    {
        public CPropertyCamera Property { get; set; } = null;

        private string m_PropertyName = "";

        public Camera m_Camera;
        
        public EventHandler<CVGrabEventArgs> EventGrabEnd;
        public EventHandler<CVGrabEventArgs> EventInspectionStart;

        private PixelDataConverter pxConvert = new PixelDataConverter();

        //public Mat ImageLast = new Mat();
        public Bitmap ImageBLast = null;
      
        public bool ViewModeCross
        {
            get;
            set;
        }

        //acA-1300-60gm
        //1282 px x 1026 px
        public int ImageWidth = 1282;
        public int ImageHeight = 1026;
        public int Index { get; set; } = 0;

        private int m_nCamIndex;

        private string m_strSN = "";
        public string SN
        {
            get { return m_strSN; }
            set { m_strSN = value; }
        }

        private string m_strExposure = "0";
        public string Exposure
        {
            get { return m_strExposure; }
            set { m_strExposure = value; }
        }

        private string m_strGain = "0";
        public string Gain
        {
            get { return m_strGain; }
            set { m_strGain = value; }
        }

        public bool IsOpen
        {
            get
            {
                if (m_Camera == null)
                    return false;
                else
                    return m_Camera.IsOpen && m_Camera.IsConnected;
            }
        }

        public ManualResetEvent IsGrabDone = new ManualResetEvent(false);

        private ICameraInfo m_CameraInfo;
        public ICameraInfo CameraInfo
        {
            get { return m_CameraInfo; }
            set { m_CameraInfo = value; }
        }

        public CCameraBasler(int nCamIndex)
        {
            try
            {
                m_nCamIndex = nCamIndex;
#if !Dbug
                m_Camera = new Camera();
#endif
            }
            catch
            {

            }    
        }

        //public bool Init(string strEnterSN, int nIndex)
        //{
        //    try
        //    {
        //        m_PropertyName = $"CAMERA_{nIndex}";
        //        Property = new CPropertyCamera(m_PropertyName);
        //        Property.INDEX = nIndex;
        //        Property = Property.LoadConfig();                
        //        bool bFind = false;

        //        List<ICameraInfo> allCameras = CameraFinder.Enumerate();

        //        foreach (ICameraInfo cameraInfos in allCameras)
        //        {
        //            Property.SERIAL_NUMBER = cameraInfos.GetValueOrDefault("SerialNumber", "");
        //            //Property.NAME =cameraInfos[CameraInfoKey.UserDefinedName];
        //            Property.IP = cameraInfos[CameraInfoKey.DeviceIpAddress];
        //            string Mac = cameraInfos[CameraInfoKey.DeviceMacAddress];
        //            //cameraInfos[CameraInfoKey.w];
        //            if (Property.SERIAL_NUMBER == strEnterSN)
        //            {
        //                m_Camera = new Camera(cameraInfos);
        //                bFind = true;
        //                break;
        //            }
        //        }

        //        if (m_Camera != null)
        //        {
        //            if (m_Camera.IsOpen)
        //            {
        //                m_Camera.Close();
        //            }

        //            if(bFind)
        //            {
        //                m_Camera.Open(cTimeOutMs, TimeoutHandling.ThrowException);
        //                SetExposure(Property.EXPOSURETIME_US);
        //                SetGain(Property.GAIN);
        //            }                    
        //            m_Camera.StreamGrabber.ImageGrabbed += OnGrabEnd;
        //            GC.Collect();
        //        }
        //        else
        //        {
        //        }

        //        return true;
        //    }
        //    catch (Exception Desc)
        //    {
        //        // ILogger.Add(LOG_TYPE.AbNormal, "[FAILED] Camera Init Class ==> {0}   Func ==> {1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);

        //        string arguments = "netsh interface set interface \"Camera\" disable";
        //        ProcessStartInfo procStartInfo = new ProcessStartInfo("netsh", arguments);

        //        procStartInfo.RedirectStandardOutput = true;
        //        procStartInfo.UseShellExecute = false;
        //        procStartInfo.CreateNoWindow = true;

        //        Process.Start(procStartInfo);

        //        Thread.Sleep(3000);

        //        arguments = "netsh interface set interface \"Camera\" enable";
        //        ProcessStartInfo procEnableInfo = new ProcessStartInfo("netsh", arguments);

        //        procStartInfo.RedirectStandardOutput = true;
        //        procStartInfo.UseShellExecute = false;
        //        procStartInfo.CreateNoWindow = true;

        //        Process.Start(procEnableInfo);

        //        Thread.Sleep(3000);

        //        return false;
        //    }
        //}
        private int m_nWaitSec = 300;
        private void OnGrabEnd(Object sender, ImageGrabbedEventArgs e)
        {
            try
            {
                IGrabResult grabResult = e.GrabResult;

                // Check if the image can be displayed.
                if (grabResult.GrabSucceeded)
                {
                    Property.WIDTH = grabResult.Width;
                    Property.HEIGHT = grabResult.Height;
                    ImageWidth = grabResult.Width;
                    ImageHeight = grabResult.Height;

                    Stopwatch sw = Stopwatch.StartNew();
                    sw.Start();

                    //Common.Convert8BitRawImageToCognexImage

                    ImageBLast = new Bitmap(grabResult.Width, grabResult.Height, PixelFormat.Format8bppIndexed);
                    BitmapData bmpData = ImageBLast.LockBits(new Rectangle(0, 0, ImageBLast.Width, ImageBLast.Height), ImageLockMode.ReadWrite, ImageBLast.PixelFormat);
                    pxConvert.OutputPixelFormat = PixelType.Mono8;
                    IntPtr bmpIntpr = bmpData.Scan0;
                    pxConvert.Convert(bmpIntpr, bmpData.Stride * ImageBLast.Height, grabResult);
                    ImageBLast.UnlockBits(bmpData);

                    sw.Stop();

                    //bmpIntpr  = Common.BitmapToPtr(ImageBLast);

                    string str = sw.ElapsedMilliseconds.ToString();

                    IsGrabDone.Set();

                    if (EventGrabEnd != null)
                        //EventGrabEnd(this, new CVGrabEventArgs(ImageBLast, this.m_nCamIndex));
                        EventGrabEnd(this, new CVGrabEventArgs(ImageBLast, Property.INDEX));
                    if (m_Camera != null) { m_Camera.StreamGrabber.Stop(); }                    
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);                
            }
            finally
            {
                // Dispose the grab result if needed for returning it to the grab loop.
                e.DisposeGrabResultIfClone();
            }
        }
        const int cTimeOutMs = 2000;
     

        public bool Grab()
        {
            try
            {
                if (m_Camera == null) return false;
                if (m_Camera.IsOpen)
                {
                    SetExposure(Property.EXPOSURETIME_US);
                    SetGain(Property.GAIN);
                    IsGrabDone.Reset();                  
                    if(!m_Camera.StreamGrabber.IsGrabbing)
                    {
                        m_Camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                    }                    
                }
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

#region Thread

        public bool Live(bool bEnable)
        {
            try
            {
                if(bEnable)
                {
                    SetExposure(Property.EXPOSURETIME_US);
                    SetGain(Property.GAIN);
                    StartThreadLive();
                }
                else
                {                    
                    StopThreadLive();
                    ResetThreadLive();
                }
            }
            catch(Exception Desc)
            {
                return false;
            }

            return true;
        }
        private CThreadStatus m_ThreadStatusLive = new CThreadStatus();
        public CThreadStatus ThreadStatusLive
        {
            get { return m_ThreadStatusLive; }
        }

        public void StartThreadLive()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadLive));
            t.Start(m_ThreadStatusLive);
        }

        public void StopThreadLive()
        {
            if (!ThreadStatusLive.IsExit())
            {
                ThreadStatusLive.Stop(100);
            }
        }

        private void ResetThreadLive()
        {
            m_ThreadStatusLive.End();
        }

        private void ThreadLive(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("Live Thread");
            CLOG.NORMAL( "Live Thread");

            try
            {
                while (!ThreadStatus.IsExit())
                {
                    Thread.Sleep(10);
                    //CGlobal.Instance.iData.Grab_Mode = true;
                    Grab();
                }
            }
            catch (Exception Desc)
            {
                
            }

        }
#endregion

        public bool Continuous()
        {
            try
            {
                if (m_Camera.IsOpen)
                {
                    //m_bIsGrabDone = false;
                    m_Camera.StreamGrabber.Stop();
                    m_Camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
                    m_Camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                }


                // ILogger.Add(LOG_TYPE.Normal, "[SUCCESS] Camera Grab Class ==> {0}   Func ==> {1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                if (m_Camera != null)
                {
                    if (m_Camera.IsOpen)
                    {
                        m_Camera.StreamGrabber.Stop();
                    }
                }

                //  ILogger.Add(LOG_TYPE.Normal, "[SUCCESS] Camera Stop Class ==> {0}   Func ==> {1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                if (m_Camera != null)
                {
                    if (m_Camera.IsOpen)
                    {
                        Live(false);

                        m_Camera.StreamGrabber.Stop();
                        m_Camera.Close();
                        //m_Camera.Dispose();
                        m_Camera = null;
                    }

                    CLOG.NORMAL( "[SUCCESS] Camera Dispose Class ==> {0}   Func ==> {1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                    return true;
                }
                else
                {
                    CLOG.ABNORMAL( "[FAILED] Camera Dispose Class ==> {0}   Func ==> {1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                    return false;
                }

            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public bool SetExposure(double dValue)
        {
            try
            {
                if (!IsOpen) { return false; }
                if (m_Camera.Parameters.Contains(PLCamera.ExposureTimeAbs))
                {
                    double exposuremintime = m_Camera.Parameters[PLCamera.ExposureTimeAbs].GetMinimum();
                    double exposuremaxtime = m_Camera.Parameters[PLCamera.ExposureTimeAbs].GetMaximum();
                    if ((dValue > exposuremintime) && (dValue < exposuremaxtime))
                    {
                        try
                        {
                            m_Camera.Parameters[PLCamera.ExposureTimeAbs].SetValue(dValue);
                            Property.EXPOSURETIME_US = (int)dValue;
                        }
                        catch (Exception Desc)
                        {
                            CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                            return false;
                        }
                    }

                }
                else
                {
                    double exposuremintime = m_Camera.Parameters[PLCamera.ExposureTime].GetMinimum();
                    double exposuremaxtime = m_Camera.Parameters[PLCamera.ExposureTime].GetMaximum();
                    if ((dValue > exposuremintime) && (dValue < exposuremaxtime))
                    {
                        m_Camera.Parameters[PLCamera.ExposureTime].SetValue(dValue);
                        Property.EXPOSURETIME_US = (int)dValue;
                    }                    
                }
                //CLog.Info( "[SUCCESS] Camera Exposure Class ==> {0}   Func ==> {1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }            
        }

        //public bool SetExposure(double dValue)
        //{
        //    try
        //    {
        //        if (m_Camera != null)
        //        {
        //            if (m_Camera.IsOpen)
        //            {
        //                m_Camera.Parameters[PLCamera.ExposureTimeAbs].SetValue(dValue);
        //            }

        //            CLog.Info( "[SUCCESS] Camera Exposure Class ==> {0}   Func ==> {1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
        //            return true;
        //        }
        //        else
        //        {
        //            CLog.Error( "[FAILED] Camera Exposure Class ==> {0}   Func ==> {1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
        //            return false;
        //        }

        //    }
        //    catch (Exception Desc)
        //    {
        //        CLog.Error( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
        //        return false;
        //    }
        //}


        public bool SetGain(int nValue)
        {
            try
            {
                if (m_Camera != null)
                {
                    // 왜 게인은 안쓰는지?
                    if (m_Camera.IsOpen)
                    {
                        m_Camera.Parameters[PLCamera.GainRaw].SetValue(nValue);
                        Property.GAIN = (int)nValue;    
                    }

                    //CLog.Info( "[SUCCESS] Camera Exposure Class ==> {0}   Func ==> {1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                    return true;
                }
                else
                {
                    CLOG.ABNORMAL( "[FAILED] Camera Exposure Class ==> {0}   Func ==> {1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                    return false;
                }

            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }         
    }

    public class CVGrabEventArgs : EventArgs
    {
        //public CogImage8Grey ImageSource = new CogImage8Grey();

        public Bitmap m_Bitmap = new Bitmap(10,10);
        private Mat m_ImageGrab = new Mat();
        public IntPtr ImagePtr = new IntPtr();
        private int m_nCamIdx;

        public int CamIdx
        {
            get => m_nCamIdx;
            set => m_nCamIdx = value;
        }

        public Mat ImageGrab
        {
            get => m_ImageGrab;
            set => m_ImageGrab = value;
        }

        public CVGrabEventArgs(IntPtr ImageArgs, int nIndex)
        {
            ImagePtr = ImageArgs;
            m_nCamIdx = nIndex;            
        }

        public CVGrabEventArgs(Mat ImageParam, int nCamIdx)
        {
            m_ImageGrab = ImageParam.Clone();
            ImageParam.Dispose();
            ImageParam = null;

            m_nCamIdx = nCamIdx;
        }

        public CVGrabEventArgs(Bitmap ImageParam, int nCamIdx)
        {
            m_Bitmap = (Bitmap)ImageParam.Clone();
            ImageParam.Dispose();
            ImageParam = null;

            m_nCamIdx = nCamIdx;
        }

        //public CVGrabEventArgs(CogImage8Grey ImageSource, int nCamIdx)
        //{
        //    this.ImageSource = ImageSource; 
        //    m_nCamIdx = nCamIdx;
        //}
    }
}