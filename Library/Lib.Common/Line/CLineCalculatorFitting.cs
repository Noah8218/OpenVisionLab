using Lib.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Line
{
    public class CLineCalculatorFitting
    {
        public double Slope { get; private set; }
        public double Intercept { get; private set; }

        private void Fit(IEnumerable<OpenCvSharp.Point2f> points)
        {
            int n = points.Count();
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            foreach (var point in points)
            {
                sumX += point.X;
                sumY += point.Y;
                sumXY += point.X * point.Y;
                sumX2 += point.X * point.X;
            }

            Slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            Intercept = (sumY - Slope * sumX) / n;
        }

        private void Fit(IEnumerable<OpenCvSharp.Point> points)
        {
            int n = points.Count();
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            foreach (var point in points)
            {
                sumX += point.X;
                sumY += point.Y;
                sumXY += point.X * point.Y;
                sumX2 += point.X * point.X;
            }

            Slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            Intercept = (sumY - Slope * sumX) / n;
        }

        private void Fit(IEnumerable<System.Drawing.Point> points)
        {
            int n = points.Count();
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            foreach (var point in points)
            {
                sumX += point.X;
                sumY += point.Y;
                sumXY += point.X * point.Y;
                sumX2 += point.X * point.X;
            }

            Slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            Intercept = (sumY - Slope * sumX) / n;
        }

        private void Fit(IEnumerable<System.Drawing.PointF> points)
        {
            int n = points.Count();
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            foreach (var point in points)
            {
                sumX += point.X;
                sumY += point.Y;
                sumXY += point.X * point.Y;
                sumX2 += point.X * point.X;
            }

            Slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            Intercept = (sumY - Slope * sumX) / n;
        }

        // Find the least squares linear fit.
        // Return the total error.
        public static double FindLinearLeastSquaresFit(IEnumerable<System.Drawing.PointF> points, out double m, out double b)
        {
            // Perform the calculation.
            // Find the values S1, Sx, Sy, Sxx, and Sxy.
            double S1 = points.Count();
            double Sx = 0;
            double Sy = 0;
            double Sxx = 0;
            double Sxy = 0;
            foreach (PointF pt in points)
            {
                Sx += pt.X;
                Sy += pt.Y;
                Sxx += pt.X * pt.X;
                Sxy += pt.X * pt.Y;
            }

            // Solve for m and b.
            m = (Sxy * S1 - Sx * Sy) / (Sxx * S1 - Sx * Sx);
            b = (Sxy * Sx - Sy * Sxx) / (Sx * Sx - S1 * Sxx);

            return Math.Sqrt(ErrorSquared(points, m, b));
        }

        // Return the error squared.
        public static double ErrorSquared(IEnumerable<System.Drawing.PointF> points, double m, double b)
        {
            double total = 0;
            foreach (PointF pt in points)
            {
                double dy = pt.Y - (m * pt.X + b);
                total += dy * dy;
            }
            return total;
        }

        public (System.Drawing.PointF, System.Drawing.PointF) LineFit(IEnumerable<System.Drawing.PointF> points)
        {
            // 직선에 맞추기            
            Fit(points);

            // 시작점과 끝점을 찾기 위해 X 좌표의 최소값과 최대값을 사용합니다.
            double minX = points.Min(p => p.X);
            double maxX = points.Max(p => p.X);

            // 시작점 (x, y) = (minX, lineFitting.Slope * minX + lineFitting.Intercept)
            System.Drawing.PointF startPoint = new System.Drawing.PointF((float)minX, (float)(Slope * minX + Intercept));

            // 끝점 (x, y) = (maxX, lineFitting.Slope * maxX + lineFitting.Intercept)
            System.Drawing.PointF endPoint = new System.Drawing.PointF((float)maxX, (float)(Slope * maxX + Intercept));

            return (startPoint, endPoint);
        }

        public (System.Drawing.PointF, System.Drawing.PointF) LineFit(IEnumerable<System.Drawing.Point> points)
        {
            // 직선에 맞추기            
            Fit(points);

            // 시작점과 끝점을 찾기 위해 X 좌표의 최소값과 최대값을 사용합니다.
            double minX = points.Min(p => p.X);
            double maxX = points.Max(p => p.X);

            // 시작점 (x, y) = (minX, lineFitting.Slope * minX + lineFitting.Intercept)
            System.Drawing.PointF startPoint = new System.Drawing.PointF((float)minX, (float)(Slope * minX + Intercept));

            // 끝점 (x, y) = (maxX, lineFitting.Slope * maxX + lineFitting.Intercept)
            System.Drawing.PointF endPoint = new System.Drawing.PointF((float)maxX, (float)(Slope * maxX + Intercept));

            return (startPoint, endPoint);
        }

        public (System.Drawing.Point, System.Drawing.Point) LinearLeastSquaresFit(List<OpenCvSharp.Point> points, System.Drawing.Size size)
        {
            float Xmin = 0;
            float Xmax = size.Width;
            float Ymin = 0;
            float Ymax = size.Height;

            // 직선에 맞추기            
            double BestM;
            double BestB;
            List<PointF> PointF = points.ConvertAll(new Converter<OpenCvSharp.Point, PointF>(CConverter.CVPointToPointF));
            FindLinearLeastSquaresFit(PointF, out BestM, out BestB);

            double y0 = BestM * Xmin + BestB;
            double y1 = BestM * Xmax + BestB;
            //e.Graphics.DrawLine(thin_pen,
            //    (float)Xmin, (float)y0, (float)Xmax, (float)y1);

            // 시작점 (x, y) = (minX, lineFitting.Slope * minX + lineFitting.Intercept)
            System.Drawing.Point startPoint = new System.Drawing.Point((int)Xmin, (int)(y0));

            // 끝점 (x, y) = (maxX, lineFitting.Slope * maxX + lineFitting.Intercept)
            System.Drawing.Point endPoint = new System.Drawing.Point((int)Xmax, (int)(y1));

            return (startPoint, endPoint);
        }

        public (System.Drawing.Point, System.Drawing.Point) LineFitX(IEnumerable<OpenCvSharp.Point> points)
        {
            // 직선에 맞추기            
            Fit(points);

            // 시작점과 끝점을 찾기 위해 X 좌표의 최소값과 최대값을 사용합니다.
            double minX = points.Min(p => p.X);
            double maxX = points.Max(p => p.X);

            // 시작점 (x, y) = (minX, lineFitting.Slope * minX + lineFitting.Intercept)
            System.Drawing.Point startPoint = new System.Drawing.Point((int)minX, (int)(Slope * minX + Intercept));

            // 끝점 (x, y) = (maxX, lineFitting.Slope * maxX + lineFitting.Intercept)
            System.Drawing.Point endPoint = new System.Drawing.Point((int)maxX, (int)(Slope * maxX + Intercept));

            return (startPoint, endPoint);
        }

        public (System.Drawing.Point, System.Drawing.Point) LineFitY(IEnumerable<OpenCvSharp.Point> points)
        {
            // 직선에 맞추기            
            Fit(points.Select(p => new System.Drawing.Point(p.Y, p.X)));

            // 시작점과 끝점을 찾기 위해 X 좌표의 최소값과 최대값을 사용합니다.
            double minY = points.Min(p => p.Y);
            double maxY = points.Max(p => p.Y);

            System.Drawing.Point startPointYX = new System.Drawing.Point((int)(Slope * minY + Intercept), (int)minY);
            System.Drawing.Point endPointYX = new System.Drawing.Point((int)(Slope * maxY + Intercept), (int)maxY);

            return (startPointYX, endPointYX);
        }
    }
}
