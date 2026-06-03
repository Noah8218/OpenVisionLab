using Lib.Line;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace Lib.Common
{
    public static class CFormula
    {
        public enum PROJECTION_POLARITY : uint { BTOW = 0, WTOB = 1, ALL = 2 };
        public enum PROJECTION_DIR : uint { X_LTOR = 0, X_RTOL, Y_TTOB, Y_BTOT};
        public enum Direction { LeftToRight, RightToLeft, ToptoBottom, BottomToTop }

        public static bool FindIntersection(CLine BaseLine/*수직선*/, CLine BaseTarget/*상판이나 하판*/, out OpenCvSharp.Point ptIntersection)
        {
            try
            {
                ptIntersection = new OpenCvSharp.Point();

                OpenCvSharp.Point p1 = BaseLine.Start; // BaseLine.ptStart부분 필요
                OpenCvSharp.Point p2 = BaseLine.End;
                OpenCvSharp.Point p3 = BaseTarget.Start;
                OpenCvSharp.Point p4 = BaseTarget.End;

                double dFactor = (p1.X - p2.X) * (p3.Y - p4.Y) - (p1.Y - p2.Y) * (p3.X - p4.X);
                if (dFactor == 0)
                {
                    return false;
                }

                double dPre = (p1.X * p2.Y - p1.Y * p2.X);
                double dPost = (p3.X * p4.Y - p3.Y * p4.X);

                double dX = (dPre * (p3.X - p4.X) - (p1.X - p2.X) * dPost) / dFactor;
                double dY = (dPre * (p3.Y - p4.Y) - (p1.Y - p2.Y) * dPost) / dFactor;

                ptIntersection = new OpenCvSharp.Point(dX, dY);
                return true;
            }
            catch (Exception Desc)
            {
                ptIntersection = new OpenCvSharp.Point();                
                return false;
            }
        }

        /// <summary>
        /// 세 점을을 이용한 각도 구하기
        /// </summary>
        /// <param name="ptBase"></param>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static double threePointAngle(OpenCvSharp.Point ptBase/*ROI와 수직점이 교차한 포인트*/, OpenCvSharp.Point pt1/*하판점*/, OpenCvSharp.Point pt2/*ROI점*/)
        {
            OpenCvSharp.Point ptVectorVerticalToBottom = new OpenCvSharp.Point();
            OpenCvSharp.Point ptVectorVerticalToRoi = new OpenCvSharp.Point();
            // B(PtBase) -> A(하판) 방향으로 가는 벡터1 생성
            ptVectorVerticalToBottom.X = pt1.X - ptBase.X;
            ptVectorVerticalToBottom.Y = pt1.Y - ptBase.Y;
            // B(PtBase) -> C(ROI) 방향으로 가는 벡터2 생성
            ptVectorVerticalToRoi.X = pt2.X - ptBase.X;
            ptVectorVerticalToRoi.Y = pt2.Y - ptBase.Y;

            // 벡터1과 벡터2의 내적
            int nMoleculatr = (ptVectorVerticalToBottom.X * ptVectorVerticalToRoi.X) + (ptVectorVerticalToBottom.Y * ptVectorVerticalToRoi.Y);

            // 벡터1과 벡터2의 Scalr값
            double dDistanceVerticalToBottom = Math.Sqrt(Math.Pow(ptVectorVerticalToBottom.X, 2) + Math.Pow(ptVectorVerticalToBottom.Y, 2));
            double dDistanceVerticalToRoi = Math.Sqrt(Math.Pow(ptVectorVerticalToRoi.X, 2) + Math.Pow(ptVectorVerticalToRoi.Y, 2));

            double nDenominator = dDistanceVerticalToBottom * dDistanceVerticalToRoi;
            // 각도 구하기 (내적 / Sclar값)
            double dAngle = Math.Acos(nMoleculatr / nDenominator) * (180 / Math.PI);

            return dAngle;
        }

        /// <summary>
        /// 중심점 기준 원근 변환 실행
        /// </summary>
        /// <param name="src"></param>
        /// <param name="squares"></param>
        /// <returns></returns>
        public static Mat PerspectiveTransform(Mat src, OpenCvSharp.Point[] squares)
        {
            Mat dst = new Mat();
            Moments moments = Cv2.Moments(squares);
            double cX = moments.M10 / moments.M00;
            double cY = moments.M01 / moments.M00;

            Point2f[] src_pts = new Point2f[4];
            for (int i = 0; i < squares.Length; i++)
            {
                if (cX > squares[i].X && cY > squares[i].Y) src_pts[0] = squares[i];
                if (cX > squares[i].X && cY < squares[i].Y) src_pts[1] = squares[i];
                if (cX < squares[i].X && cY > squares[i].Y) src_pts[2] = squares[i];
                if (cX < squares[i].X && cY < squares[i].Y) src_pts[3] = squares[i];
            }

            Point2f[] dst_pts = new Point2f[4];
            dst_pts[0] = new Point2f(0, 0);
            dst_pts[1] = new Point2f(0, src.Height);
            dst_pts[2] = new Point2f(src.Width, 0);
            dst_pts[3] = new Point2f(src.Width, src.Height);


            Mat matrix = Cv2.GetPerspectiveTransform(src_pts, dst_pts);
            Cv2.WarpPerspective(src, dst, matrix, new OpenCvSharp.Size(src.Width, src.Height));
            return dst;
        }

        /// <summary>
        /// Area(Width * Height) Return
        /// </summary>
        /// <param name="sz"></param>
        /// <returns></returns>
        public static int AreaofRect(OpenCvSharp.Size sz) => sz.Width * sz.Height;
        public static double Angle(OpenCvSharp.Point ptfrom, OpenCvSharp.Point ptto) => Math.Atan2(ptto.Y - ptfrom.Y, ptto.X - ptfrom.X) * 180.0D / Math.PI;    
        public static double Angle(System.Drawing.Point ptfrom, System.Drawing.Point ptto) => Math.Atan2(ptto.Y - ptfrom.Y, ptto.X - ptfrom.X) * 180.0D / Math.PI;
        public static double Angle(PointF ptfrom, PointF ptto) => Math.Atan2(ptto.Y - ptfrom.Y, ptto.X - ptfrom.X) * 180.0D / Math.PI;
        public static double CalculateAngle360(OpenCvSharp.Point ptfrom, OpenCvSharp.Point ptto)
        {
            double angle = Angle(ptfrom, ptto);
            if (angle < 0) { angle += 360; }
            return angle;
        }

        public static double CalculateAngle360(System.Drawing.Point ptfrom, System.Drawing.Point ptto)
        {
            double angle = Angle(ptfrom, ptto);
            if (angle < 0) { angle += 360; }
            return angle;
        }

        public static double CalculateAngle360(PointF ptFrom, PointF ptTo)
        {
            double angle = Angle(ptFrom, ptTo);
            if (angle < 0) { angle += 360; }
            return angle;
        }

        public static double RoiAngle(CLine BaseLien, System.Drawing.Point ptCenter)
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


            double dAngle = Math.Abs((d2 - d1) * 180 / Math.PI);
            return dAngle;
        }

        public static double DegreeToRadian(double Angle) { return Math.PI * Angle / 180.0; }

        public static double RadianToDegree(double Angle) { return Angle * (180.0 / Math.PI); }

        #region 내적 구하기 - GetDotProduct(x1, y1, x2, y2, x3, y3)

        /// <summary>
        /// 내적 구하기
        /// </summary>
        /// <param name="x1">X 1</param>
        /// <param name="y1">Y 1</param>
        /// <param name="x2">X 2</param>
        /// <param name="y2">Y 2</param>
        /// <param name="x3">X 3</param>
        /// <param name="y3">Y 3</param>
        /// <returns>내적</returns>
        public static float GetDotProduct(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            float dx12 = x1 - x2;
            float dy12 = y1 - y2;
            float dx32 = x3 - x2;
            float dy32 = y3 - y2;

            return (dx12 * dx32 + dy12 * dy32);
        }

        #endregion
        #region 외적 구하기 - GetCrossProduct(x1, y1, x2, y2, x3, y3)

        /// <summary>
        /// 외적 구하기
        /// </summary>
        /// <param name="x1">X 1</param>
        /// <param name="y1">Y 1</param>
        /// <param name="x2">X 2</param>
        /// <param name="y2">Y 2</param>
        /// <param name="x3">X 3</param>
        /// <param name="y3">Y 3</param>
        /// <returns>외적</returns>
        public static float GetCrossProduct(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            float dx12 = x1 - x2;
            float dy12 = y1 - y2;
            float dx32 = x3 - x2;
            float dy32 = y3 - y2;

            return (dx12 * dy32 - dy12 * dx32);
        }

        #endregion
        #region 각도 구하기 - GetAngle(x1, y1, x2, y2, x3, y3)

        /// <summary>
        /// 각도 구하기
        /// </summary>
        /// <param name="x1">X 1</param>
        /// <param name="y1">Y 1</param>
        /// <param name="x2">X 2</param>
        /// <param name="y2">Y 2</param>
        /// <param name="x3">X 3</param>
        /// <param name="y3">Y 3</param>
        /// <returns>각도</returns>
        public static float GetAngle(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            float dotProduct = GetDotProduct(x1, y1, x2, y2, x3, y3);

            float crossProduct = GetCrossProduct(x1, y1, x2, y2, x3, y3);

            return (float)Math.Atan2(crossProduct, dotProduct);
        }

        #endregion
        #region 다각형 내 포인트 여부 구하기 - IsPointInPolygon(x, y, polygonPointList)

        /// <summary>
        /// 다각형 내 포인트 여부 구하기
        /// </summary>
        /// <param name="x">X 좌표</param>
        /// <param name="y">Y 좌표</param>
        /// <param name="polygonPointList">다각형 포인트 리스트</param>
        /// <returns>다각형 내 포인트 여부</returns>
        public static bool IsPointInPolygon(float x, float y, PointF[] polygonPointList)
        {
            int pointCount = polygonPointList.Length - 1;

            float totalAngle = GetAngle
            (
                polygonPointList[pointCount].X,
                polygonPointList[pointCount].Y,
                x,
                y,
                polygonPointList[0].X,
                polygonPointList[0].Y
            );

            for (int i = 0; i < pointCount; i++)
            {
                totalAngle += GetAngle
                (
                    polygonPointList[i].X,
                    polygonPointList[i].Y,
                    x,
                    y,
                    polygonPointList[i + 1].X,
                    polygonPointList[i + 1].Y
                );
            }

            return (Math.Abs(totalAngle) > 0.000001);
        }

        #endregion
        #region 교차 포인트 찾기 - FindIntersectionPoint(point1, point2, point3, point4, lineIntersect, segmentIntersect, intersectionPoint, closePoint1, closePoint2, t1, t2)

        /// <summary>
        /// 교차 포인트 찾기
        /// </summary>
        /// <param name="point1">포인트 1</param>
        /// <param name="point2">포인트 2</param>
        /// <param name="point3">포인트 3</param>
        /// <param name="point4">포인트 41</param>
        /// <param name="lineIntersect">직선 교차 여부</param>
        /// <param name="segmentIntersect">세그먼트 교차 여부</param>
        /// <param name="intersectionPoint">교차 포인트</param>
        /// <param name="closePoint1">근접 포인트 1</param>
        /// <param name="closePoint2">근접 포인트 2</param>
        /// <param name="t1">T1</param>
        /// <param name="t2">T2</param>
        public static void FindIntersectionPoint(PointF point1, PointF point2, PointF point3, PointF point4, out bool lineIntersect, out bool segmentIntersect, out PointF intersectionPoint, out PointF closePoint1, out PointF closePoint2, out float t1, out float t2)
        {
            float dx12 = point2.X - point1.X;
            float dy12 = point2.Y - point1.Y;
            float dx34 = point4.X - point3.X;
            float dy34 = point4.Y - point3.Y;

            float denominator = (dy12 * dx34 - dx12 * dy34);

            t1 = ((point1.X - point3.X) * dy34 + (point3.Y - point1.Y) * dx34) / denominator;

            if (float.IsInfinity(t1))
            {
                lineIntersect = false;
                segmentIntersect = false;

                intersectionPoint = new PointF(float.NaN, float.NaN);

                closePoint1 = new PointF(float.NaN, float.NaN);
                closePoint2 = new PointF(float.NaN, float.NaN);

                t2 = float.PositiveInfinity;

                return;
            }

            lineIntersect = true;

            t2 = ((point3.X - point1.X) * dy12 + (point1.Y - point3.Y) * dx12) / -denominator;

            intersectionPoint = new PointF(point1.X + dx12 * t1, point1.Y + dy12 * t1);

            segmentIntersect = ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));

            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            closePoint1 = new PointF(point1.X + dx12 * t1, point1.Y + dy12 * t1);
            closePoint2 = new PointF(point3.X + dx34 * t2, point3.Y + dy34 * t2);
        }

        #endregion
        #region 클리핑 포인트 배열 구하기 - GetClippingPointArray(startOutsidePolygon, point1, point2, polygonPointList)

        /// <summary>
        /// 클리핑 포인트 배열 구하기
        /// </summary>
        /// <param name="startOutsidePolygon">다각형 외부 시작 여부</param>
        /// <param name="point1">포인트 1</param>
        /// <param name="point2">포인트 2</param>
        /// <param name="polygonPointList">다각형 포인트 리스트</param>
        /// <returns>포인트 배열</returns>
        public static PointF[] GetClippingPointArray(out bool startOutsidePolygon, PointF point1, PointF point2, List<PointF> polygonPointList)
        {
            List<PointF> intersectionPointList = new List<PointF>();

            List<float> valueList = new List<float>();

            intersectionPointList.Add(point1);

            valueList.Add(0f);

            startOutsidePolygon = !IsPointInPolygon(point1.X, point1.Y, polygonPointList.ToArray());

            for (int i = 0; i < polygonPointList.Count; i++)
            {
                int j = (i + 1) % polygonPointList.Count;

                bool lineIntersect;
                bool segmentIntersect;

                PointF intersectionPoint;
                PointF closePoint1;
                PointF closePoint2;
                float value1;
                float value2;

                FindIntersectionPoint
                (
                    point1,
                    point2,
                    polygonPointList[i],
                    polygonPointList[j],
                    out lineIntersect,
                    out segmentIntersect,
                    out intersectionPoint,
                    out closePoint1,
                    out closePoint2,
                    out value1,
                    out value2
                );

                if (segmentIntersect)
                {
                    intersectionPointList.Add(intersectionPoint);

                    valueList.Add(value1);

                    break;
                }
            }

            //intersectionPointList.Add(point2);

            //valueList.Add(1f);

            PointF[] intersectionPointArray = intersectionPointList.ToArray();

            float[] valueArray = valueList.ToArray();

            Array.Sort(valueArray, intersectionPointArray);

            return intersectionPointArray;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="pt3"></param>
        /// <param name="pt4"></param>
        /// <returns></returns>
        public static bool CCW(OpenCvSharp.Point pt1, OpenCvSharp.Point pt2, OpenCvSharp.Point pt3, OpenCvSharp.Point pt4)
        {
            double dFactor = 0;

            double d1 = (pt2.X - pt1.X) * (pt3.Y - pt1.Y) - (pt2.Y - pt1.Y) * (pt3.X - pt1.X);
            double d2 = (pt2.X - pt1.X) * (pt4.Y - pt1.Y) - (pt2.Y - pt1.Y) * (pt4.X - pt1.X);

            dFactor = d1 * d2;

            if (dFactor < 0) { return true; }

            else { return false; }
        }

        /// <summary>
        /// 4개의 포인트로 서로 교차 여부를 확인 합니다.
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="pt3"></param>
        /// <param name="pt4"></param>
        /// <returns></returns>
        public static bool CrossCheck(OpenCvSharp.Point pt1, OpenCvSharp.Point pt2, OpenCvSharp.Point pt3, OpenCvSharp.Point pt4)
        {
            bool bIsDivideLine1 = CCW(pt1, pt2, pt3, pt4);
            bool bIsDivideLine2 = CCW(pt3, pt4, pt1, pt2);

            if (bIsDivideLine1 && bIsDivideLine2) { return true; }
            else { return false; }
        }

        /// <summary>
        /// 라인 길이를 일정 길이만큼 늘립니다.
        /// </summary>
        /// <param name="FitLine"></param>
        /// <param name="T"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static (System.Drawing.Point, System.Drawing.Point) ExtendLength(CLine FitLine, double T, int length)
        {
            System.Drawing.Point start = new System.Drawing.Point();
            System.Drawing.Point end = new System.Drawing.Point();

            /*
             * [산수 메모] 한점과 기울기를 알때 일정거리만큼 떨어진 점의 좌표
             한점(x,y)
             기울기 m
             일정거리 t
             새점.x = x + (t * Cos(m));
             새점.y = y + (t * Sin(m));  
            */
            start.X = (int)(FitLine.Start.X - (length * Math.Cos(DegreeToRadian(T))));
            start.Y = (int)(FitLine.Start.Y - (length * Math.Sin(DegreeToRadian(T))));

            end.X = (int)(FitLine.End.X + (length * Math.Cos(DegreeToRadian(T))));
            end.Y = (int)(FitLine.End.Y + (length * Math.Sin(DegreeToRadian(T))));

            return (start, end);
        }
    }
}
