using System;
using System.Collections.Generic;
using System.Drawing;
using Lib.OpenCV.Blob;
using Matrox.MatroxImagingLibrary;
using OpenCvSharp;

namespace OpenVisionLab
{
    public class MessageEventArgs : EventArgs
    {
        public enum MESSAGEBOX_TYPE { OKCANCEL, OK };

        private string m_strHead = "";
        public string Head
        {
            get => m_strHead;
            set => m_strHead = value;
        }

        private string m_strMessage = "";
        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public MessageEventArgs(string strMessage, string strHead)
        {
            m_strMessage = strMessage;
            m_strHead = strHead;
        }

        public MessageEventArgs()
        {

        }
    }

    public class StringEventArgs : EventArgs
    {
        private string m_strMessage = "";
        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public StringEventArgs(string strMessage)
        {
            m_strMessage = strMessage;
        }

        public StringEventArgs()
        {

        }
    }


    public class InspResultArgs : EventArgs
    {
        public int Index = 0;
        public Bitmap imageResult { get; set; } = new Bitmap(10, 10);
        public Bitmap imageOri { get; set; } = new Bitmap(10, 10);
        public string resultData = "";
        public DEFINE.RESULT result = DEFINE.RESULT.NA;
        public double tackTime = 0;
        
        public List<CResultBlob> black_Result = new List<CResultBlob>();
        public List<CResultBlob> white_Result = new List<CResultBlob>();
        public List<CResultBlob> totalResults = new List<CResultBlob>();

        public List<CResultBlob> pin_Results = new List<CResultBlob>();
        public List<double> avgMM = new List<double>();
        public InspResultArgs() { }

        public InspResultArgs(Bitmap imageResult, double TackTime, DEFINE.RESULT result)
        {
            this.imageResult = (Bitmap)imageResult.Clone();
            imageResult.Dispose();
            imageResult = null;

            this.result = result;
            this.tackTime = TackTime;
        }

        public InspResultArgs(Bitmap imageResult, int Index, double TackTime, DEFINE.RESULT result)
        {
            this.imageResult = (Bitmap)imageResult.Clone();
            imageResult.Dispose();
            imageResult = null;

            this.result = result;
            this.Index = Index;
            this.tackTime = TackTime;
        }
    }

    public class AlignResultArgs : EventArgs
    {
        public OpenCvSharp.Point IntersectionL;
        public OpenCvSharp.Point IntersectionR;

        public double Angle = 0;

        public string MasterCenter = "";

        public AlignResultArgs(OpenCvSharp.Point IntersectionL, OpenCvSharp.Point IntersectionR, double Angle)
        {
            this.IntersectionL = IntersectionL;
            this.IntersectionR = IntersectionR;
            this.Angle = Angle;

            int nCx = (int)((IntersectionL.X + IntersectionR.X) / 2.0F);
            int nCy = (int)((IntersectionL.Y + IntersectionR.Y) / 2.0F);

            this.MasterCenter = string.Format("(X:{0} Y:{1})", nCx, nCy);
        }
    }

    public class CVRectEventArgs : EventArgs
    {
        private Rect m_rt = new Rect();
        public Rect rt
        {
            get { return m_rt; }
            set { m_rt = value; }
        }



        public CVRectEventArgs(Rect rt)
        {
            m_rt = rt;
        }
    }

    public class RectEventArgs : EventArgs
    {
        public string Mode = "";
        public System.Drawing.Rectangle Rect = new System.Drawing.Rectangle();

        public RectEventArgs(System.Drawing.Rectangle rt, string strMode)
        {
            Rect = rt;
            Mode = strMode;
        }
    }

    //public class CognexGrabEventArgs : EventArgs
    //{
    //    public Cognex.VisionPro.CogImage8Grey ImageGrab = new Cognex.VisionPro.CogImage8Grey();

    //    public int m_Index;

    //    public CognexGrabEventArgs(Cognex.VisionPro.CogImage8Grey ImageArgs, int nIndex)
    //    {
    //        ImageGrab = new Cognex.VisionPro.CogImage8Grey(ImageArgs);
    //        //(Cognex.VisionPro.CogImage8Grey)ImageArgs.CopyBase(Cognex.VisionPro.CogImageCopyModeConstants.CopyPixels);
    //        m_Index = nIndex;
    //    }
    //}

    public class GrabEventArgs : EventArgs
    {
        public Mat ImageGrabColor = new Mat();
        public Mat ImageGrab = null;
        public IntPtr ImagePtr = new IntPtr();
        public MIL_ID m_Buffer;
        public int m_Index;
        public double getEncoder = 0;

        public GrabEventArgs(IntPtr ImageArgs, int nIndex)
        {
            ImagePtr = ImageArgs;
            m_Index = nIndex;
        }
        public GrabEventArgs(Mat ImageArgs, int nIndex, double getEncoder)
        {
            ImageGrab = ImageArgs;
            m_Index = nIndex;
            this.getEncoder = getEncoder;   
        }

        public GrabEventArgs(Mat ImageArgs, Mat Image, int nIndex)
        {
            ImageGrabColor = Image.Clone();
            ImageGrab = ImageArgs.Clone();
            m_Index = nIndex;
        }

        public GrabEventArgs(MIL_ID Buffer, int nIndex)
        {
            m_Buffer = Buffer;
            m_Index = nIndex;
        }
    }

    public class DockDisplayEventArgs : EventArgs
    {
        public Bitmap Image = new Bitmap(10, 10);
        public int Index;
        public string TackTime = "";

        public DockDisplayEventArgs(Bitmap bitmap, int nIndex, string tackTime)
        {
            Image = bitmap;
            Index = nIndex;
            TackTime = tackTime;
        }
    }

    //public class LogEventArgs : EventArgs
    //{
    //    private ILog m_iLog;
    //    public ILog Log
    //    {
    //        get { return m_iLog; }
    //        set { m_iLog = value; }
    //    }

    //    public LogEventArgs(ILog iLog)
    //    {
    //        Log = iLog;
    //    }
    //}
}
