using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Lib.OpenCV.Result
{
    public class CResultContour
    {
        public double Area { get; set; } = 0.0D;
        public double Angle { get; set; } = 0.0D;
        public OpenCvSharp.Point2d Center { get; set; } = new OpenCvSharp.Point2d();
        public Rectangle Bounding { get; set; } = new Rectangle();

        public OpenCvSharp.Point[] Contours { get; set; } = new OpenCvSharp.Point[4];

        public int Index { get; set; } = 0;

        public CResultContour(int index, double Area, OpenCvSharp.Point2d Center, Rect rt, OpenCvSharp.Point[] Contours, double dAngle = 0.0D)
        {
            this.Index = index;
            this.Area = Area;
            this.Center = new OpenCvSharp.Point2d(Center.X, Center.Y);
            Bounding = new Rectangle(rt.X, rt.Y, rt.Width, rt.Height);
            this.Contours = Contours;
            Angle = dAngle;
        }
    }
}
