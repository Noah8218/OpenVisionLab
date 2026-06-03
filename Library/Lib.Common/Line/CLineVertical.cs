using Lib.Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using static Lib.Common.CFormula;

namespace Lib.Line
{
    public static class CLineVertical
    {
        public static void GetLineCoef(OpenCvSharp.Point ptStart, OpenCvSharp.Point ptEnd, OpenCvSharp.Point ptBase, OpenCvSharp.Point ptImageSize, out List<OpenCvSharp.Point> listPtVert)
        {
            try
            {
                listPtVert = new List<OpenCvSharp.Point>();
                int nImageWidth = ptImageSize.X;
                int nImageHeight = ptImageSize.Y;
                int nStartBaseX = ptBase.X;
                int nStartBaseY = ptBase.Y;

                // 포인트 1, 2의 기울기 구함
                int nVertY = 0;
                double dLineAngle = (double)(ptEnd.Y - ptStart.Y) / (double)(ptEnd.X - ptStart.X);

                // 직선 A와 직선 B가 수직이면
                // 직선 A 기울기 * 직선 B 기울기 == -1
                // 그러기 때문에 아래와 같은 공식이 됨
                double dVerticalAngle = -(1.0 / dLineAngle);

                if (ptStart.X - ptEnd.X != 0)
                {
                    if (ptStart.Y - ptEnd.Y != 0)
                    {
                        OpenCvSharp.Point ptVerticalX = new OpenCvSharp.Point();
                        for (int nIndex = 0; nIndex < nImageWidth; nIndex++)
                        {
                            nVertY = ((int)(dVerticalAngle * (nIndex - ptBase.X)) + ptBase.Y);
                            ptVerticalX.X = nIndex;
                            ptVerticalX.Y = nVertY;
                            listPtVert.Add(ptVerticalX);
                        }
                        //// 수직 라인 구하기               
                    }
                    else
                    {
                        OpenCvSharp.Point ptVerticalY = new OpenCvSharp.Point();
                        for (int nIndex = 0; nIndex < nImageHeight; nIndex++)
                        {
                            ptVerticalY.X = ptBase.X;
                            ptVerticalY.Y = nIndex;
                            listPtVert.Add(ptVerticalY);
                        }
                        // Y축에 평행

                    }
                }
                else
                {
                    OpenCvSharp.Point ptVerticalX = new OpenCvSharp.Point();
                    for (int nIndex = 0; nIndex < nImageWidth; nIndex++)
                    {
                        ptVerticalX.X = nIndex;
                        ptVerticalX.Y = ptBase.Y;
                        listPtVert.Add(ptVerticalX);
                    }
                    // X축에 평행
                }
            }
            catch (Exception Desc)
            {
                listPtVert = new List<OpenCvSharp.Point>();
                return;
            }
        }

        public static void GetLineCoef(OpenCvSharp.Point ptStart/*하판 1Point*/, OpenCvSharp.Point ptEnd/*하판 2Point*/, Rect rcRoi, PROJECTION_DIR direction, double dAngle, out List<OpenCvSharp.Point> listPtVertLine)
        {
            int nStartRoiX = rcRoi.X;
            int nStartRoiY = rcRoi.Y;
            int nWidthRoiX = rcRoi.X + rcRoi.Width;
            int nEndRoiY = rcRoi.Y + rcRoi.Height;

            // 포인트 1, 2의 기울기 구함
            int nVertX = 0;
            int nVertY = 0;
            listPtVertLine = new List<OpenCvSharp.Point>();
            OpenCvSharp.Point ptVertLineY = new OpenCvSharp.Point();
            double dLineAngle = dAngle;//(ptEnd.Y - ptStart.Y) / (ptEnd.X - ptStart.X);
                                       //(ptEnd.Y - ptStart.Y) / (ptEnd.X - ptStart.X);
                                       // 직선 A와 직선 B가 수직이면
                                       // 직선 A 기울기 * 직선 B 기울기 == -1
                                       // 그러기 때문에 아래와 같은 공식이 됨
            double dVerticalAngle = -(1.0 / dLineAngle);
            // 수직 라인 구하기
            switch (direction)
            {
                case PROJECTION_DIR.X_LTOR:
                    {
                        for (int nCount = ptStart.X; nCount > nStartRoiX; nCount--)
                        {
                            nVertY = ((int)(dVerticalAngle * (nCount - ptStart.X)) + ptStart.Y);
                            ptVertLineY.X = nCount;
                            ptVertLineY.Y = nVertY;
                            listPtVertLine.Add(ptVertLineY);
                        }
                        // 수직선 구하기 -> 직선의 방정식

                    }
                    break;
                case PROJECTION_DIR.X_RTOL:
                    {
                        for (int nCount = ptStart.X; nCount < nWidthRoiX; nCount++)
                        {
                            nVertY = ((int)(dVerticalAngle * (nCount - ptStart.X)) + ptStart.Y);
                            ptVertLineY.X = nCount;
                            ptVertLineY.Y = nVertY;
                            listPtVertLine.Add(ptVertLineY);
                        }
                    }
                    break;
                case PROJECTION_DIR.Y_TTOB:
                    {
                        for (int nCount = ptStart.Y; nCount > nStartRoiY; nCount--)
                        {
                            nVertX = ((int)((nCount - ptStart.Y) / dVerticalAngle) + ptStart.X);
                            ptVertLineY.X = nVertX;
                            ptVertLineY.Y = nCount;
                            listPtVertLine.Add(ptVertLineY);
                        }
                    }
                    break;
                case PROJECTION_DIR.Y_BTOT:
                    {
                        for (int nCount = ptStart.Y; nCount < nEndRoiY; nCount++)
                        {
                            nVertX = ((int)((nCount - ptStart.Y) / dVerticalAngle) + ptStart.X);
                            ptVertLineY.X = nVertX;
                            ptVertLineY.Y = nCount;
                            listPtVertLine.Add(ptVertLineY);
                        }
                    }
                    break;
            }
        }


        private static List<OpenCvSharp.Point> GetRangePoint(int Cnt, int Range, List<OpenCvSharp.Point> Edge)
        {
            List<OpenCvSharp.Point> Point = new List<OpenCvSharp.Point>();
            // 레인지만큼 포인트를 가져다가 수직선을 만듬
            if (Cnt + Range > Edge.Count)
            {
                if (Edge.Count < Range)
                {
                    int Diff = Edge.Count - (Cnt + Range);
                    if (Diff > 0) { Point = Edge.GetRange(Cnt - Diff, Range); }
                    else { Point = Edge.GetRange(0, Edge.Count); }
                }
                else
                {
                    int Diff = Math.Abs(Edge.Count - (Cnt + Range));
                    Point = Edge.GetRange(Cnt - Diff, Range);
                }
            }
            else { Point = Edge.GetRange(Cnt, Range); }

            return Point;
        }

        /// <summary>
        /// 지정한 엣지 개수만큼 각도를 구해서 선을 생성합니다.
        /// </summary>
        /// <param name="Edge">총 엣지 리스트</param>
        /// <param name="ImageW">이미지의 Width(생성한 선이 Width만큼 길어짐)</param>
        /// <param name="ImageH">이미지의 Height(생성한 선이 Height만큼 길어짐)</param>
        /// <param name="POINT_RANGE">POINT_RANGE마다 각도를 산출</param>
        /// <param name=""></param>
        /// <returns></returns>
        public static List<CLine> GetVerticalLines(List<OpenCvSharp.Point> Edge, int ImageW, int ImageH, int POINT_RANGE, PROJECTION_DIR VER_PRJ_DIR)
        {
            // 수직선 라인들(직선의 방정식으로 만들어진)
            List<CLine> ver_lines = new List<CLine>();
            // 검출된 엣지의 숫자만큼 반복
            for (int Cnt = 0; Cnt < Edge.Count; Cnt++)
            {
                OpenCvSharp.Point Start; OpenCvSharp.Point End;
                List<OpenCvSharp.Point> VerLine = new List<OpenCvSharp.Point>();
                if (Cnt == Edge.Count - 1)
                {
                    Start = Edge[Cnt];
                    End = Edge[(Cnt)];
                }
                else
                {
                    Start = Edge[Cnt];
                    End = Edge[(Cnt + 1)];
                }

                OpenCvSharp.Line2D Line = Cv2.FitLine(GetRangePoint(Cnt, POINT_RANGE, Edge), DistanceTypes.L2, 0, 0.01, 0.01);
                double T = Math.Tan(Line.GetVectorRadian());
                GetLineCoef(Start, End, new OpenCvSharp.Rect(0, 0, ImageW, ImageH), VER_PRJ_DIR, T, out VerLine);

                System.Drawing.Point ptStart = new System.Drawing.Point(VerLine[0].X, VerLine[0].Y);
                System.Drawing.Point ptEnd = new System.Drawing.Point(VerLine[VerLine.Count - 1].X, VerLine[VerLine.Count - 1].Y);
                ver_lines.Add(new CLine(CConverter.PointToCVPoint(ptStart), CConverter.PointToCVPoint(ptEnd)));
            }
            return ver_lines;
        }

        /// <summary>
        /// 지정한 각도로 선을 생성합니다.
        /// </summary>
        /// <param name="Edge">총 엣지 리스트</param>
        /// <param name="ImageW">이미지의 Width(생성한 선이 Width만큼 길어짐)</param>
        /// <param name="ImageH">이미지의 Height(생성한 선이 Height만큼 길어짐)</param>
        /// <param name="MANUAL_ANGLE_VALUE">지정한 각도로 선을 생성</param>
        /// <param name=""></param>
        /// <returns></returns>
        public static List<CLine> GetVerticalLinesManual(List<OpenCvSharp.Point> Edge, int ImageW, int ImageH, double MANUAL_ANGLE_VALUE, PROJECTION_DIR VER_PRJ_DIR)
        {
            // 수직선 라인들(직선의 방정식으로 만들어진)
            List<CLine> ver_lines = new List<CLine>();
            // 검출된 엣지의 숫자만큼 반복
            for (int Cnt = 0; Cnt < Edge.Count; Cnt++)
            {
                OpenCvSharp.Point Start; OpenCvSharp.Point End;
                List<OpenCvSharp.Point> VerLine = new List<OpenCvSharp.Point>();
                if (Cnt == Edge.Count - 1)
                {
                    Start = Edge[Cnt];
                    End = Edge[(Cnt)];
                }
                else
                {
                    Start = Edge[Cnt];
                    End = Edge[(Cnt + 1)];
                }

                double result = Math.Tan(MANUAL_ANGLE_VALUE * (Math.PI / 180));
                CLineVertical.GetLineCoef(Start, End, new OpenCvSharp.Rect(0, 0, ImageW, ImageH), VER_PRJ_DIR, result, out VerLine);

                System.Drawing.Point ptStart = new System.Drawing.Point(VerLine[0].X, VerLine[0].Y);
                System.Drawing.Point ptEnd = new System.Drawing.Point(VerLine[VerLine.Count - 1].X, VerLine[VerLine.Count - 1].Y);
                ver_lines.Add(new CLine(CConverter.PointToCVPoint(ptStart), CConverter.PointToCVPoint(ptEnd)));
            }
            return ver_lines;
        }

        public static List<CLine> GetIntersectionLines(List<CLine> ver_Lines, List<OpenCvSharp.Point> Edges)
        {
            List<CLine> intersectionLines = new List<CLine>();
            if (Edges.Count == 0)
            {
                CLOG.ABNORMAL($"Not Exists Edge, Check Parameter");
                Debug.WriteLine($"Not Exists Edge, Check Parameter");
                return intersectionLines;
            }

            List<PointF> Points_R = Edges.ConvertAll(new Converter<OpenCvSharp.Point, PointF>(CConverter.CVPointToPointF));

            for (int i = 0; i < ver_Lines.Count; i++)
            {
                CLine verLine = ver_Lines[i];

                bool draw = true;

                PointF start = new PointF(ver_Lines[i].Start.X, ver_Lines[i].Start.Y);
                PointF end = new PointF(ver_Lines[i].End.X, ver_Lines[i].End.Y);


                PointF[] intersectionPointArray = GetClippingPointArray
                (
                    out draw,
                    start,
                    end,
                    Points_R
                );
                if (intersectionPointArray.Length > 1) { intersectionLines.Add(new CLine(intersectionPointArray[0], intersectionPointArray[0 + 1])); }
            }

            return intersectionLines;
        }

        public static OpenCvSharp.Point GetIntersectionLines(CLine Lines_L, CLine Lines_R)
        {
            OpenCvSharp.Point intersection = new OpenCvSharp.Point();
            bool bInterSection = CrossCheck(Lines_L.Start, Lines_L.End, Lines_R.Start, Lines_R.End);
            if (bInterSection)
            {
                FindIntersection(Lines_L, Lines_R, out intersection);
            }

            return intersection;
        }
    }
}
