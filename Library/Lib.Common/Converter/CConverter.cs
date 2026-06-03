using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Drawing.Imaging;

namespace Lib.Common
{
    public static class CConverter
    {              
        public static string RoiToString(OpenCvSharp.Rect ROI) => string.Format("{0},{1},{2},{3}", ROI.X, ROI.Y, ROI.Width, ROI.Height);
        public static string RoiToString(Rectangle ROI) => string.Format("{0},{1},{2},{3}", ROI.X, ROI.Y, ROI.Width, ROI.Height);
        public static System.Drawing.Point CVPointToPoint(OpenCvSharp.Point pt) => new System.Drawing.Point(pt.X, pt.Y);
        public static PointF CVPointToPointF(OpenCvSharp.Point pt) => new PointF((float)pt.X, (float)pt.Y);
        public static System.Drawing.PointF PointToPointF(System.Drawing.Point pt) => new PointF(pt.X, pt.Y);
        public static System.Drawing.Point PointFToPoint(System.Drawing.PointF pt) => new System.Drawing.Point((int)pt.X, (int)pt.Y);
        public static OpenCvSharp.Point PointToCVPoint(System.Drawing.Point pt) => new OpenCvSharp.Point(pt.X, pt.Y);
        public static byte IntToByte(int nValue) => Convert.ToByte(nValue.ToString());
        public static string PointToString(System.Drawing.Point pt) => string.Format("{0},{1}", pt.X, pt.Y);
        public static string PointFToString(PointF pt) => string.Format("{0},{1}", pt.X, pt.Y);
        public static string CVPointToString(OpenCvSharp.Point pt) => string.Format("{0},{1}", pt.X, pt.Y);
        public static OpenCvSharp.Point CeterFromRect(OpenCvSharp.Rect rt) => new OpenCvSharp.Point(rt.X + rt.Width / 2, rt.Y + rt.Height / 2);
        public static System.Drawing.Point CeterFromRectangle(Rectangle rt) => new System.Drawing.Point(rt.X + rt.Width / 2, rt.Y + rt.Height / 2);
        public static OpenCvSharp.Point RectOfCenter(Rect rt) => new OpenCvSharp.Point(rt.X + rt.Width / 2, rt.Y + rt.Height / 2);
        public static OpenCvSharp.Point RectangleOfCenter(OpenCvSharp.Rect rt) => new OpenCvSharp.Point(rt.X + rt.Width / 2, rt.Y + rt.Height / 2);
        public static System.Drawing.Point RectangleOfCenter(Rectangle rt) => new System.Drawing.Point(rt.X + rt.Width / 2, rt.Y + rt.Height / 2);
        public static PointF Center(this Rectangle rect) => new PointF((float)rect.Left + (float)rect.Width / 2f, (float)rect.Top + (float)rect.Height / 2f);    
        public static PointF Center(this RectangleF rect) => new PointF(rect.Left + rect.Width / 2f, rect.Top + rect.Height / 2f);  
        public static string ColorToString(Color cr) => string.Format("{0},{1},{2}", cr.R, cr.G, cr.B);
        public static Rectangle RectToRectangle(Rect rect) => new Rectangle() { X = rect.X, Y = rect.Y, Width = rect.Width, Height = rect.Height };    
        public static Rect RectangleToRect(Rectangle rectangle) => new Rect() { X = rectangle.X, Y = rectangle.Y, Width = rectangle.Width, Height = rectangle.Height };    
        public static Rect2f RectangleToRect(RectangleF rectangle) => new Rect2f() { X = rectangle.X, Y = rectangle.Y, Width = rectangle.Width, Height = rectangle.Height };    
        public static string ShortToBinaryString(short shValue) => Convert.ToString(shValue, 2).PadLeft(8, '0');
        public static string IntToBinaryString(int nValue, int nZeroCount) => Convert.ToString(nValue, 2).PadLeft(nZeroCount, '0');
        public static string RectToString(Rectangle rt) => $"{rt.X},{rt.Y},{rt.Width},{rt.Height}";
        public static OpenCvSharp.Rect RectToCVRect(System.Drawing.Rectangle rt) => new OpenCvSharp.Rect(rt.X, rt.Y, rt.Width, rt.Height);
        public static System.Drawing.Rectangle CVRectToRect(OpenCvSharp.Rect rt) => new System.Drawing.Rectangle(rt.X, rt.Y, rt.Width, rt.Height);

        public static OpenCvSharp.Point CenterofRect(Rect rt) => new OpenCvSharp.Point(rt.X + rt.Width / 2, rt.Y + rt.Height / 2);
    
        public static Rectangle StringToRectangle(string strROI)
        {
            Rectangle ROI = new Rectangle();
            string[] strSplit = strROI.Split(',');

            if (strSplit.Length == 4)
            {
                int nX = int.Parse(strSplit[0]);
                int nY = int.Parse(strSplit[1]);
                int nW = int.Parse(strSplit[2]);
                int nH = int.Parse(strSplit[3]);

                ROI = new Rectangle(nX, nY, nW, nH);
            }

            return ROI;
        }

        public static Rect StringToRect(string strROI)
        {
            Rect ROI = new Rect();
            string[] strSplit = strROI.Split(',');

            if (strSplit.Length == 4)
            {
                int nX = int.Parse(strSplit[0]);
                int nY = int.Parse(strSplit[1]);
                int nW = int.Parse(strSplit[2]);
                int nH = int.Parse(strSplit[3]);

                ROI = new Rect(nX, nY, nW, nH);
            }

            return ROI;
        }

        public static System.Drawing.Point StringToPoint(string strPoint)
        {
            string[] strPointSplit = strPoint.Split(',');
            System.Drawing.Point pt = new System.Drawing.Point(0, 0);

            if (strPointSplit.Length == 2)
            {
                string strX = strPointSplit[0].Trim();
                string strY = strPointSplit[1].Trim();

                int nX = int.Parse(strX);
                int nY = int.Parse(strY);

                pt = new System.Drawing.Point(nX, nY);
            }

            return pt;
        }

        public static System.Drawing.PointF StringToPointF(string strPoint)
        {
            string[] strPointSplit = strPoint.Split(',');
            System.Drawing.PointF pt = new PointF(0, 0);

            if (strPointSplit.Length == 2)
            {
                string strX = strPointSplit[0].Trim();
                string strY = strPointSplit[1].Trim();

                float fX = float.Parse(strX);
                float fY = float.Parse(strY);

                pt = new PointF(fX, fY);
            }

            return pt;
        }

        public static OpenCvSharp.Point StringToCVPoint(string strPoint)
        {
            string[] strPointSplit = strPoint.Split(',');
            OpenCvSharp.Point pt = new OpenCvSharp.Point(0, 0);

            if (strPointSplit.Length == 2)
            {
                string strX = strPointSplit[0].Trim();
                string strY = strPointSplit[1].Trim();

                int nX = int.Parse(strX);
                int nY = int.Parse(strY);

                pt = new OpenCvSharp.Point(nX, nY);
            }

            return pt;
        }

        public static OpenCvSharp.Rect StringToCVRect(string strRect)
        {
            string[] sRT = strRect.Split(',');
            if (sRT.Length == 4)
            {
                int nX = int.Parse(sRT[0]);
                int nY = int.Parse(sRT[1]);
                int nW = int.Parse(sRT[2]);
                int nH = int.Parse(sRT[3]);

                return new OpenCvSharp.Rect(nX, nY, nW, nH);
            }


            return new OpenCvSharp.Rect();
        }

        public static System.Drawing.Rectangle ScreenRectToLogicalRect(System.Drawing.Rectangle rt, float fScaleFactorX, float fScaleFactorY)
        {
            rt.X = (int)(rt.X / fScaleFactorX);
            rt.Y = (int)(rt.Y / fScaleFactorY);
            rt.Width = (int)(rt.Width / fScaleFactorX);
            rt.Height = (int)(rt.Height / fScaleFactorY);

            return rt;
        }

        public static System.Drawing.Rectangle LogicalRectToScreenRect(System.Drawing.Rectangle rt, float fScaleFactorX, float fScaleFactorY)
        {
            System.Drawing.Rectangle rtScreen = new System.Drawing.Rectangle();
            rtScreen.X = (int)(rt.X * fScaleFactorX);
            rtScreen.Y = (int)(rt.Y * fScaleFactorY);
            rtScreen.Width = (int)(rt.Width * fScaleFactorX);
            rtScreen.Height = (int)(rt.Height * fScaleFactorY);

            return rtScreen;
        }

        public static OpenCvSharp.Rect ScreenCVRectToLogicalCVRect(OpenCvSharp.Rect rt, float fScaleFactorX, float fScaleFactorY)
        {
            rt.X = (int)(rt.X * fScaleFactorX);
            rt.Y = (int)(rt.Y * fScaleFactorY);
            rt.Width = (int)(rt.Width * fScaleFactorX);
            rt.Height = (int)(rt.Height * fScaleFactorY);

            return rt;
        }

        public static OpenCvSharp.Point ScreenCVPointToLogicalCVPoint(OpenCvSharp.Point pt, float fScaleFactorX, float fScaleFactorY)
        {
            OpenCvSharp.Point ptScreen = new OpenCvSharp.Point();

            ptScreen.X = (int)(pt.X * fScaleFactorX);
            ptScreen.Y = (int)(pt.Y * fScaleFactorY);

            return ptScreen;
        }

        public static System.Drawing.Point ScreenPointToLogicalPoint(System.Drawing.Point pt, float fScaleFactorX, float fScaleFactorY)
        {
            System.Drawing.Point ptScreen = new System.Drawing.Point();

            ptScreen.X = (int)(pt.X * fScaleFactorX);
            ptScreen.Y = (int)(pt.Y * fScaleFactorY);

            return ptScreen;
        }
    }
}
