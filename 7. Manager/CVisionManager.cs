using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

using OpenCvSharp;
using OpenCvSharp.UserInterface;

namespace KtemVisionSystem
{
    public class IVisionManager
    {
        public CGrabberEuresys m_Grabber;

        //private IResultFormat m_cResultMap;
        //public IResultFormat cResultMap
        //{
        //    set => cResultMap = value;
        //    get => cResultMap;
        //}

    public bool m_bClose = false;
        public bool IsOpen
        {
            get => m_Grabber.IsOpen;
        }

        public bool m_bIsImageSave = false;

        #region EVENT REGISTER
        public EventHandler<EventArgs>  EventImageSaveNG;
        #endregion

        public IVisionManager()
        {
            m_Grabber = new CGrabberEuresys();
            InitGrabber();
        }

        public bool InitGrabber()
        {
            try
            {
                m_Grabber.Init();                
                m_Grabber.Start();                
                //m_Grabber.SetExpose(0, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("[Worker Open] : {0}", ex.Message));
                return false;
            }

            return true;
        }

        public void Close()
        {
            m_bClose = true;
            m_Grabber.Close();
        }

        public void Grab()
        {
            m_Grabber.Grab();
        }

        public void SetDisplay(PictureBoxIpl pb)
        {
            m_Grabber.SetDisplay(pb);            
        }

        public void SetExposureTime(int nExposureTimeus)
        {
            m_Grabber.SetExpose(0, nExposureTimeus);
        }


        public Mat GetGrabImage()
        {
            return m_Grabber.ImageGrab.Clone();
        }
    }

    public class LogResultNG
    {
        public Mat m_ImageLog = new Mat();
        public string m_strDateTime = "";
        public int m_nCamIndex = 0;
        public ulong m_nInspIndex = 0;

        public LogResultNG(int nCamIndex, ulong nInspIndex, Mat ImageLog)
        {
            m_nCamIndex = nCamIndex;
            m_nInspIndex = nInspIndex;
            m_ImageLog = ImageLog;
            m_strDateTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss fff");
        }
    }
}
