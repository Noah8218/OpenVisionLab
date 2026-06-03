using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Lib.OpenCV.Result
{
    public class CResultMatching
    {
        public int Index { get; set; } = 0;
        public double Score { get; set; } = 0.0D;
        public double Angle { get; set; } = 0.0D;
        public OpenCvSharp.Point2f Center { get; set; } = new OpenCvSharp.Point2f();
        public RectangleF Bounding { get; set; } = new RectangleF();

        public CResultMatching(int nIndex, double dScore, OpenCvSharp.Point2f ptCenter, Rect2f rt, double dAngle = 0.0D)
        {
            Index = nIndex;
            Score = dScore;
            Center = new OpenCvSharp.Point2f(ptCenter.X, ptCenter.Y);
            Bounding = new RectangleF(rt.X, rt.Y, rt.Width, rt.Height);
            Angle = dAngle;
        }
    }
}
