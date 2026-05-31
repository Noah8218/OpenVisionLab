using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;

namespace KtemVisionSystem
{
    public class Line
    {
        private Point m_ptStart = new Point();
        public Point Start
        {
            get { return m_ptStart; }
            set { m_ptStart = value; }
        }

        private Point m_ptEnd = new Point();
        public Point End
        {
            get { return m_ptEnd; }
            set { m_ptEnd = value; }
        }

        public double Distance()
        {
            return m_ptStart.DistanceTo(m_ptEnd);
        }

        public Line(Point ptStart, Point ptEnd)
        {
            m_ptStart = ptStart;
            m_ptEnd = ptEnd;
        }

        public Line()
        {

        }
    }
    public static class IMath
    {
        /* 큰 순서로 정렬
         *                         //for (int j = 0; j < pp.Length-1; j++)
                        //{
                        //    for (int k = j + 1; k < pp.Length; k++)
                        //    {
                        //        if (pp[j].Y < pp[k].Y)
                        //        {
                        //            ptTemp = pp[k];
                        //            pp[k] = pp[j];
                        //            pp[j] = ptTemp;
                        //        }
                        //    }
                        //}
         */

        public static double GetDistanceLineToPoint(Point ptTarget, Point lineStartPoint, Point lineEndPoint, out Point closestPoint)
        {
            closestPoint = new Point();

            double dx = lineEndPoint.X - lineStartPoint.X;
            double dy = lineEndPoint.Y - lineStartPoint.Y;

            if ((dx == 0) && (dy == 0))
            {
                dx = ptTarget.X - lineStartPoint.X;
                dy = ptTarget.Y - lineEndPoint.Y;

                return Math.Sqrt(dx * dx + dy * dy);
            }

            double t = ((ptTarget.X - lineStartPoint.X) * dx + (ptTarget.Y - lineStartPoint.Y) * dy) / (dx * dx + dy * dy);

            if (t < 0)
            {
                dx = ptTarget.X - lineStartPoint.X;
                dy = ptTarget.Y - lineStartPoint.Y;
            }
            else if (t > 1)
            {
                dx = ptTarget.X - lineEndPoint.X; dy = ptTarget.Y - lineEndPoint.Y;
            }
            else
            {
                closestPoint = new Point(lineStartPoint.X + t * dx, lineStartPoint.Y + t * dy);
                dx = ptTarget.X - closestPoint.X; dy = ptTarget.Y - closestPoint.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

#if EURESYS
        public static double GetShortestDistance(Euresys.Open_eVision_2_16.EPoint point, Euresys.Open_eVision_2_16.EPoint listStartPoint, Euresys.Open_eVision_2_16.EPoint lineEndPoint, out Euresys.Open_eVision_2_16.EPoint closestPoint) 
        { 
            float dx = lineEndPoint.X - listStartPoint.X; 
            float dy = lineEndPoint.Y - listStartPoint.Y; 
            
            if ((dx == 0) && (dy == 0)) 
            {
                closestPoint = listStartPoint; dx = point.X - listStartPoint.X; 
                dy = point.Y - listStartPoint.Y; 
                
                return Math.Sqrt(dx * dx + dy * dy); 
            }
            
            float t = ((point.X - listStartPoint.X) * dx + (point.Y - listStartPoint.Y) * dy) / (dx * dx + dy * dy); 
            if (t < 0) 
            {
                closestPoint = new Euresys.Open_eVision_2_16.EPoint(listStartPoint.X, listStartPoint.Y);
                dx = point.X - listStartPoint.X; dy = point.Y - listStartPoint.Y; 
            } 
            else if (t > 1) 
            {
                closestPoint = new Euresys.Open_eVision_2_16.EPoint(lineEndPoint.X, lineEndPoint.Y); dx = point.X - lineEndPoint.X; dy = point.Y - lineEndPoint.Y; 
            } 
            else
            { 
                closestPoint = new Euresys.Open_eVision_2_16.EPoint(listStartPoint.X + t * dx, listStartPoint.Y + t * dy); dx = point.X - closestPoint.X; dy = point.Y - closestPoint.Y;
            }
            
            return Math.Sqrt(dx * dx + dy * dy);
        }


        public static double GetDistanceLineToPoint(Euresys.Open_eVision_2_16.EPoint ptTarget, Euresys.Open_eVision_2_16.ELine Line)
        {
            Point2f lineStartPoint = new Point2f(Line.Org.X, Line.Org.Y);
            Point2f lineEndPoint = new Point2f(Line.End.X, Line.End.Y);

            double dx = lineEndPoint.X - lineStartPoint.X;
            double dy = lineEndPoint.Y - lineStartPoint.Y;

            if ((dx == 0) && (dy == 0))
            {
                dx = ptTarget.X - lineStartPoint.X;
                dy = ptTarget.Y - lineEndPoint.Y;

                return Math.Sqrt(dx * dx + dy * dy);
            }

            double t = ((ptTarget.X - lineStartPoint.X) * dx + (ptTarget.Y - lineStartPoint.Y) * dy) / (dx * dx + dy * dy);

            if (t < 0)
            {
                dx = ptTarget.X - lineStartPoint.X;
                dy = ptTarget.Y - lineStartPoint.Y;
            }
            else if (t > 1)
            {
                dx = ptTarget.X - lineEndPoint.X; dy = ptTarget.Y - lineEndPoint.Y;
            }
            else
            {
                Point3d closestPoint = new Point3d(lineStartPoint.X + t * dx, lineStartPoint.Y + t * dy, 0);
                dx = ptTarget.X - closestPoint.X; dy = ptTarget.Y - closestPoint.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }
#endif

        public static double Angle(Point ptfrom, Point ptto)
        {
            return Math.Atan2(ptto.Y - ptfrom.Y, ptto.X - ptfrom.X) * 180.0D / Math.PI;
        }

        public static double Angle(System.Drawing.Point ptfrom, System.Drawing.Point ptto)
        {
            return Math.Atan2(ptto.Y - ptfrom.Y, ptto.X - ptfrom.X) * 180.0D / Math.PI;
        }

        public static double RoiAngle(CLine BaseLien, Point ptCenter)
        {
            double d1 = 0;
            double d2 = 0;
            if (BaseLien.Start.X > ptCenter.X)
            {
                d1 = Math.Atan((BaseLien.Start.Y - ptCenter.Y) / (BaseLien.Start.X - ptCenter.X));
                d2 = Math.Atan((BaseLien.End.Y - ptCenter.Y) / (BaseLien.End.X - ptCenter.X));
            }
            else
            {
                d1 = Math.Atan((ptCenter.Y - BaseLien.Start.Y) / (ptCenter.X - BaseLien.Start.X));
                d2 = Math.Atan((ptCenter.Y - BaseLien.End.Y) / (ptCenter.X - BaseLien.End.X));
            }


            double dAngle =  Math.Abs((d2 - d1) * 180 / Math.PI);
            return dAngle;
        }
        //public static float DistanceToPoint(Point a, Point b)
        //{
        //    return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        //}

        //public static Point RotatePoint(float angle, Point pt)
        //{
        //    var a = angle * System.Math.PI / 180.0;
        //    double cosa = Math.Cos(a);
        //    double sina = Math.Sin(a);
        //    var pointX = (pt.X * cosa) - (pt.Y * sina);
        //    var pointY = (pt.X * sina) + (pt.Y * cosa);
        //    return new Point((int)pointX, (int)pointY);
        //}
    }
    public class CLine
    {
        private Point2d m_ptStart = new Point2d();
        public Point2d Start
        {
            get { return m_ptStart; }
            set { m_ptStart = value; }
        }

        private Point2d m_ptEnd = new Point2d();
        public Point2d End
        {
            get { return m_ptEnd; }
            set { m_ptEnd = value; }
        }

        public double Distance()
        {
            return m_ptStart.DistanceTo(m_ptEnd);
        }

        public CLine(Point ptStart, Point ptEnd)
        {
            m_ptStart = ptStart;
            m_ptEnd = ptEnd;
        }

        public CLine(Point2d ptStart, Point2d ptEnd)
        {
            m_ptStart = ptStart;
            m_ptEnd = ptEnd;
        }

        public CLine(double dStartX, double dStartY, double dEndX, double dEndY)
        {
            m_ptStart = new Point2d(dStartX, dStartY);
            m_ptEnd = new Point2d(dEndX, dEndY); 
        }

        public CLine()
        {

        }
    }

    public class Polygon
    {
        private Point[] m_points;
        public Point[] pts
        {
            get { return m_points; }
        }

        public Polygon(Point[] pointsofPolygon)
        {
            m_points = pointsofPolygon;
        }

        public Point Center()
        {            
            Point[] parrCt = ReturnCenterpointofPolygon();

            if (parrCt.Length > 0)
            {
                return parrCt[parrCt.Length - 1];
            }
            else
            {
                return new Point();
            }
        }

        public List<Point> PointsList()
        {
            List<Point> listPoints = new List<Point>();

            for(int i=0; i<m_points.Length; i++)
            {
                listPoints.Add(m_points[i]);
            }

            return listPoints;
        }

        public Rect BoundingRect()
        {
            Rect rt = Cv2.BoundingRect(m_points);
            return rt;
        }

        public Point Centerpoint(Point x1, Point x2)
        {
            return new Point((x1.X + x2.X) / 2, (x1.Y + x2.Y) / 2);
        }

        public Point GetNdivedPoint(Point x1, Point x2, int i)
        {
            Point vector = new Point(x2.X - x1.X, x2.Y - x1.Y);
            return new Point(x1.X + (int)(vector.X / i), x1.Y + (int)(vector.Y / i));
        }

        public Point[] ReturnCenterpointofPolygon()
        {
            //Point temp;
            Point[] temp = new Point[m_points.Length - 1];
            int i = 2;

            temp[0] = Centerpoint(m_points[0], m_points[1]);

            while (i < m_points.Length) 
            {
                temp[i - 1] = GetNdivedPoint(temp[i - 2], m_points[i], ++i); //담엔 1/n
            }
            return temp;
        }
    }
}
