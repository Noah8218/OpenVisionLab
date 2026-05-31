using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    public class Point4D
    {
        double x;
        double y;
        double z;
        double t;

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double T { get; set; }
    }

    public static class CAlignCalculration
    {

        public enum CameraAlignDirection
        {
            R_0,
            R_90,
            R_180,
            R_270
        }

        static public Point4D CalculateRotate(Point cam1ImagePixel, Point cam2ImagePixel
                        , Point cam1TeachingPixel, Point cam2TeachingPixel,
                        double realMarkCenterMM
                        , double camPixelToMM, CameraAlignDirection camRotAngleDirection, bool camIsTop, bool shuttleIsClockwise = true, string saveRootPath = null)
        {

            var (physCam1Mark, physCam2Mark, physCam1Teaching, physCam2Teaching) =
                GetPhysicalPixel(cam1ImagePixel, cam2ImagePixel, cam1TeachingPixel, cam2TeachingPixel,
                    camRotAngleDirection, camIsTop);

            double tRadMark = 0;
            double tRadTeaching = 0;
            double deltaRad = 0;


            if (camRotAngleDirection == CameraAlignDirection.R_0)
            {
                tRadMark = Math.Asin((physCam2Mark.Y - physCam1Mark.Y) * camPixelToMM / realMarkCenterMM);
                tRadTeaching = Math.Asin((physCam2Teaching.Y - physCam1Teaching.Y) * camPixelToMM / realMarkCenterMM);
            }
            else if (camRotAngleDirection == CameraAlignDirection.R_180)
            {
                tRadMark = Math.Asin((physCam2Mark.Y - physCam1Mark.Y) * camPixelToMM / realMarkCenterMM);
                tRadTeaching = Math.Asin((physCam2Teaching.Y - physCam1Teaching.Y) * camPixelToMM / realMarkCenterMM);

            }
            else if (camRotAngleDirection == CameraAlignDirection.R_90)
            {
                tRadMark = Math.Asin((physCam1Mark.X - physCam2Mark.X) * camPixelToMM / realMarkCenterMM);
                tRadTeaching = Math.Asin((physCam1Teaching.X - physCam2Teaching.X) * camPixelToMM / realMarkCenterMM);

            }
            else if (camRotAngleDirection == CameraAlignDirection.R_270)
            {
                tRadMark = Math.Asin((physCam1Mark.X - physCam2Mark.X) * camPixelToMM / realMarkCenterMM);
                tRadTeaching = Math.Asin((physCam1Teaching.X - physCam2Teaching.X) * camPixelToMM / realMarkCenterMM);
            }

            double deltaT = (tRadMark - tRadTeaching) * 180.0 / Math.PI;

            if (shuttleIsClockwise)
            {
                deltaT *= -1.0;
            }

            return new Point4D() { T = deltaT };
        }

        static public Point4D CalculateXYOffset(Point cam1ImagePixel, Point cam2ImagePixel
            , Point cam1TeachingPixel, Point cam2TeachingPixel,
            double realMarkCenterMM
            , double camPixelToMM, CameraAlignDirection camRotAngleDirection, bool camIsTop, bool shuttleIsClockwise = true, string saveRootPath = null)
        {
            var (physCam1Mark, physCam2Mark, physCam1Teaching, physCam2Teaching) =
                GetPhysicalPixel(cam1ImagePixel, cam2ImagePixel, cam1TeachingPixel, cam2TeachingPixel,
                    camRotAngleDirection, camIsTop);

            Point deltaP = new Point(((physCam1Mark.X - physCam1Teaching.X) + (physCam2Mark.X - physCam2Teaching.X)) / 2.0
                , ((physCam1Mark.Y - physCam1Teaching.Y) + (physCam2Mark.Y - physCam2Teaching.Y)) / 2.0);


            if (camIsTop == false)
            {
                deltaP.Y *= -1;
            }

            return new Point4D() { X = deltaP.X * camPixelToMM, Y = deltaP.Y * camPixelToMM };
        }

        static (Point cam1PhysMark, Point cam2PhysMark, Point cam1PhysTeaching, Point cam2PhysTeaching)
          GetPhysicalPixel(Point cam1ImagePixel, Point cam2ImagePixel
          , Point cam1TeachingPixel, Point cam2TeachingPixel,
           CameraAlignDirection camRotAngleDirection, bool camIsTop)
        {
            Point cam1PhysMark = GetPhysicalImagePixel(camRotAngleDirection,
                camIsTop, cam1ImagePixel);
            Point cam2PhysMark = GetPhysicalImagePixel(camRotAngleDirection,
                camIsTop, cam2ImagePixel);
            Point cam1PhysTeaching = GetPhysicalImagePixel(camRotAngleDirection,
                camIsTop, cam1TeachingPixel);
            Point cam2PhysTeaching = GetPhysicalImagePixel(camRotAngleDirection,
                camIsTop, cam2TeachingPixel);

            return (cam1PhysMark, cam2PhysMark, cam1PhysTeaching, cam2PhysTeaching);

        }

        public static Point GetPhysicalImagePixel(CameraAlignDirection camDirection, bool isTopView, Point point)
        {
            Point phyPixel = new Point(point.X, point.Y);

            if (!isTopView)
            {
                phyPixel = GetXAxisSymmetry(phyPixel);
            }

            if (camDirection == CameraAlignDirection.R_0)
            {
                phyPixel = new Point(point.X, -1 * point.Y);
            }
            else if (camDirection == CameraAlignDirection.R_90)
            {
                phyPixel = new Point(point.Y, point.X);
            }
            else if (camDirection == CameraAlignDirection.R_180)
            {
                phyPixel = new Point(-1 * point.X, point.Y);
            }
            else if (camDirection == CameraAlignDirection.R_270)
            {
                phyPixel = new Point(-1 * point.Y, -1 * point.X);
            }

            return phyPixel;
        }
        public static Point GetXAxisSymmetry(Point src)
        {
            return new Point(src.X * -1.0, src.Y);
        }
    }
}
