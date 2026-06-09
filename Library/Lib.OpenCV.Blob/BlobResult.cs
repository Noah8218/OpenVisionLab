using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Lib.OpenCV.Blob
{
    public class BlobResult
    {
        public int Index { get; set; } = 0;
        public double Area { get; set; } = 0.0D;
        public double Angle { get; set; } = 0.0D;
        public Point2d Center { get; set; } = new Point2d();
        public Rectangle Bounding { get; set; } = new Rectangle();

        public bool UseDefect { get; set; } = true;

        public BlobResult(int index, double Area, Point2d Center, Rect rt, double dAngle = 0.0D)
        {
            Index = index;
            this.Area = Area;
            this.Center = new Point2d(Math.Round(Center.X, 2), Math.Round(Center.Y, 2));
            Bounding = new Rectangle(rt.X, rt.Y, rt.Width, rt.Height);
            Angle = Math.Round(dAngle, 2);
        }

        public BlobResult()
        {

        }
    }
}
