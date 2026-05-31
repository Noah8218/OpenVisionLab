using Euresys;
using Euresys.MultiCam;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenCvSharp.UserInterface;
using System.Collections.Concurrent;
using System.Threading;

namespace KtemVisionSystem
{
    public class CGrabberEuresys
    {
        public CPropertyCamera Property = new CPropertyCamera("EGRABBER");
        public MC.CALLBACK multiCamCallback;
        public ManualResetEvent IsGrabDone = new ManualResetEvent(false);

        public Mat ImageGrab = new Mat();

        protected string m_strName;
        private bool isOpen;
        public bool IsOpen { get => isOpen; set => isOpen = value; }

        public bool m_bRun;

        public uint EuresysCh;
        public bool VisibleCross { get; set; } = false;

        public System.Drawing.Size m_ImageSize;
        public int m_nBufferPitch = 0;
        internal int m_nImageCount;

        public EventHandler<GrabEventArgs> EventGrabEnd;

        public CGrabberEuresys()
        {
            isOpen = false;
            m_bRun = false;
        }

        public bool Init()
        {
            try
            {
                // 나중에 레시피 이름 넣어서
                //Property.LoadConfig();

                string strFilePath = Application.StartupPath + "\\LA-CM-08K08A_L8192RG";
                int nImgSizeX, nImgSizeY, nbufferPitch;

                MC.OpenDriver();
                MC.SetParam(MC.CONFIGURATION, "ErrorLog", "error.log");
                MC.SetParam(MC.BOARD + 0, "BoardTopology", "MONO");

                MC.Create("CHANNEL", out EuresysCh);
                MC.SetParam(EuresysCh, "DriverIndex", 0);
                MC.SetParam(EuresysCh, "Connector", "M");

                MC.SetParam(EuresysCh, "CamFile", strFilePath);

                //MC.SetParam(EuresysCh, "Hactive_Px", 8192);
                //MC.SetParam(EuresysCh, "Vactive_Ln", 500);

                //MC.SetParam(EuresysCh, "ImageFlipX", "OFF");
                //MC.SetParam(EuresysCh, "ImageFlipY", "OFF");

                //MC.SetParam(EuresysCh, "AcquisitionMode", "WEB");
                MC.SetParam(EuresysCh, "TrigMode", "COMBINED");
                //MC.SetParam(EuresysCh, "NextTrigMode", "SAME");
                //MC.SetParam(EuresysCh, "SeqLength_Fr", MC.INDETERMINATE);

                //MC.SetParam(EuresysCh, "FrameRate_mHz", 100000);
                //MC.SetParam(EuresysCh, "ExposeOverlap", "ALLOW");
                MC.SetParam(EuresysCh, "Expose_us", 990);
                MC.SetParam(EuresysCh, "TapConfiguration", "FULL_8T8");

                //MC.SetParam(EuresysCh, "PageCaptureMode", "FIRST_LINE");
                //MC.SetParam(EuresysCh, "SynchronizedAcquisition", "OFF");
                //MC.SetParam(EuresysCh, "SynchronizedAcquisitionBus", "ISO");
                //MC.SetParam(EuresysCh, "SynchronizedPageTrigger", "LINETRIGGER");

                MC.GetParam(EuresysCh, "ImageSizeX", out nImgSizeX);
                MC.GetParam(EuresysCh, "ImageSizeY", out nImgSizeY);
                MC.GetParam(EuresysCh, "BufferPitch", out nbufferPitch);

                m_ImageSize.Width = nImgSizeX;
                m_ImageSize.Height = nImgSizeY;
                m_nBufferPitch = nbufferPitch;

                int[] sizes = new int[2] { (int)m_ImageSize.Height, (int)m_ImageSize.Width };

                // Register the callback function
                multiCamCallback = new MC.CALLBACK(MultiCamCallback);
                MC.RegisterCallback(EuresysCh, multiCamCallback, EuresysCh);

                // Enable the signals corresponding to the callback functions
                MC.SetParam(EuresysCh, MC.SignalEnable + MC.SIG_SURFACE_PROCESSING, "ON");
                MC.SetParam(EuresysCh, MC.SignalEnable + MC.SIG_ACQUISITION_FAILURE, "ON");

                string LineRataMode = "";
                string LineCaptureMode = "";
                string TrigMode = "";
                string TapConfiguration = "";
                MC.GetParam(EuresysCh, "LineRateMode", out LineRataMode);
                MC.GetParam(EuresysCh, "LineCaptureMode", out LineCaptureMode);
                MC.GetParam(EuresysCh, "TrigMode", out TrigMode);
                MC.GetParam(EuresysCh, "TapConfiguration", out TapConfiguration);
                Start();                
                isOpen = true;
            }
            catch (MultiCamException exc)
            {
                MessageBox.Show(exc.Message, "MultiCam Exception");
                return false;
            }

            return true;
        }

        public void Start()
        {
            string channelState;
            MC.GetParam(EuresysCh, "ChannelState", out channelState);
            if (channelState != "ACTIVE")
                MC.SetParam(EuresysCh, "ChannelState", "ACTIVE");
        }

        public void SetModeStrobe()
        {

            MC.SetParam(EuresysCh, "StrobeDur", 100);
            MC.SetParam(EuresysCh, "StrobePos", 0);
            MC.SetParam(EuresysCh, "StrobeMode", "AUTO");
            MC.SetParam(EuresysCh, "PreStrobe_us", 195);
            //MC.SetParam(EuresysCh, "PreStrobe_us", 50);
            //MC.SetParam(EuresysCh, "Expose_us", 100);            
        }

        public void Resume()
        {
            MC.SetParam(EuresysCh, "ChannelState", "IDLE");
            MC.SetParam(EuresysCh, "TrigMode", "IMMEDIATE");

            string channelState;
            MC.GetParam(EuresysCh, "ChannelState", out channelState);
            if (channelState != "ACTIVE")
                MC.SetParam(EuresysCh, "ChannelState", "ACTIVE");

            // Generate a soft trigger event
            MC.SetParam(EuresysCh, "ForceTrig", "TRIG");
        }

        public bool Grab()
        {
            try
            {
                IsGrabDone.Reset();

                //MC.SetParam(EuresysCh, "LineCaptureMode", "ALL");
                //MC.SetParam(EuresysCh, "LineRateMode", "PERIOD");
                MC.SetParam(EuresysCh, "Period_us", "1000");

                string channelState;
                MC.GetParam(EuresysCh, "ChannelState", out channelState);
                if (channelState != "ACTIVE")
                {                    
                    MC.SetParam(EuresysCh, "ChannelState", "ACTIVE");
                }                
                MC.SetParam(EuresysCh, "ForceTrig", "TRIG");
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool SetPeriodUS(string us)
        {
            try
            {                
                MC.SetParam(EuresysCh, "Period_us", us);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool Live(bool bLive)
        {
            try
            {
                if (bLive)
                {
                    SoftwareTrigger();
                    StartThreadLive();
                }
                else
                {
                    StopThreadLive();
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public CThreadStatus ThreadStatusLive { get; set; } = new CThreadStatus();

        private void StartThreadLive()
        {
            if (ThreadStatusLive.IsExit())
            {
                Thread t = new Thread(new ParameterizedThreadStart(ThreadLive));
                t.Start(ThreadStatusLive);
            }
        }

        public void StopThreadLive()
        {
            if (!ThreadStatusLive.IsExit())
            {
                ThreadStatusLive.Stop(100);
            }

            ThreadStatusLive.End();
        }

        private void ThreadLive(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("Start the Live");

            try
            {
                while (!ThreadStatus.IsExit())
                {
                    Thread.Sleep(100);
                    Grab();
                }

                ThreadStatus.End();
                return;
            }
            catch (Exception ex)
            {
                ThreadStatus.End();                
            }
        }

        public void Pause()
        {
            MC.SetParam(EuresysCh, "ChannelState", "IDLE");
            MC.SetParam(EuresysCh, "TrigMode", "SOFT");
            MC.SetParam(EuresysCh, "NextTrigMode", "SAME");
        }

        public bool Close()
        {
            StopThreadLive();
            Thread.Sleep(1000);

            if (!isOpen)
                return false;

            Stop();

            string channelState;
            MC.GetParam(EuresysCh, "ChannelState", out channelState);

            if (channelState != "IDLE") MC.SetParam(EuresysCh, "ChannelState", "IDLE");

            isOpen = false;

            MC.CloseDriver();

            return true;
        }

        public void Stop()
        {
            m_bRun = false;

            string channelState;
            MC.GetParam(EuresysCh, "ChannelState", out channelState);
            if (channelState != "IDLE") MC.SetParam(EuresysCh, "ChannelState", "IDLE");
        }
        public void HardwareTrigger(string sLineNo = "IIN1")
        {
            MC.SetParam(EuresysCh, "ChannelState", "IDLE");
            //MC.SetParam(EuresysCh, "ChannelState", "ORPHAN");
            MC.SetParam(EuresysCh, "TrigMode", "HARD");
            MC.SetParam(EuresysCh, "NextTrigMode", "SAME");
            MC.SetParam(EuresysCh, "TrigLine", sLineNo);
            MC.SetParam(EuresysCh, "TrigEdge", "GOHIGH");
            MC.SetParam(EuresysCh, "TrigFilter", "MEDIUM");
            //MC.SetParam(EuresysCh, "TrigFilter", "ON");
            MC.SetParam(EuresysCh, "TrigCtl", "ISO");
            MC.SetParam(EuresysCh, "SeqLength_Fr", MC.INDETERMINATE);

            string channelState;
            MC.GetParam(EuresysCh, "ChannelState", out channelState);
            if (channelState != "ACTIVE")
                MC.SetParam(EuresysCh, "ChannelState", "ACTIVE");

            CLOG.DEVICE( "UPDATED TRIGGER H/W");
        }

        public void SoftwareTrigger()
        {
            MC.SetParam(EuresysCh, "ChannelState", "IDLE");
            MC.SetParam(EuresysCh, "TrigMode", "SOFT");
            MC.SetParam(EuresysCh, "NextTrigMode", "SAME");

            CLOG.DEVICE( "UPDATED TRIGGER S/W");
        }

        public void MultiCamCallback(ref MC.SIGNALINFO signalInfo)
        {
            switch (signalInfo.Signal)
            {
                case MC.SIG_SURFACE_PROCESSING:
                    ProcessingCallback(signalInfo);
                    break;
            }
        }
        public int m_nRecvCount = 0;
        private void ProcessingCallback(MC.SIGNALINFO signalInfo)
        {
            try
            {
                uint currentChannel = (uint)signalInfo.Context;
                uint currentSurface = signalInfo.SignalInfo;

                IntPtr ptr;
                MC.GetParam(currentSurface, "SurfaceAddr", out ptr);

                int[] sizes = new int[2] { (int)m_ImageSize.Height, (int)m_ImageSize.Width };

                using (Mat ImageRecv = new Mat(sizes, MatType.CV_8U, ptr))
                {
                    ImageGrab = ImageRecv.Clone();
                    m_nRecvCount++;

                    if ( EventGrabEnd != null )
                    {
                        EventGrabEnd( this, new GrabEventArgs(ptr, 0 ) );
                    }
                }

                IsGrabDone.Set();
            }
            catch (MultiCamException exc)
            {
            }
            catch (Exception exc)
            {
            }
        }

        public bool SetExpose(int iChannel, int iExpose_us)
        {
            bool bRet = false;
            try
            {
                MC.SetParam(EuresysCh, "Expose_us", iExpose_us);
            }
            catch
            {

            }
            return bRet;
        }

        private PictureBoxIpl m_pbl = new PictureBoxIpl();

        

        public void SetDisplay(PictureBoxIpl pb)
        {
            m_pbl = pb;
        }
    }
}
