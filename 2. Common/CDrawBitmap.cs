using OpenVisionLab._2._Common;
using Lib.Common;
using Lib.Line;
using Lib.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    public static class CDrawBitmap
    {
        public static Bitmap GetBitmapFormat24bppRgb(Bitmap bitmap)
        {
            return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        }

        public static void ToHighQuality(this Graphics graphics)
        {
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        public static void ToLowQuality(this Graphics graphics)
        {
            graphics.InterpolationMode = InterpolationMode.Low;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        }

        public static void DrawCVLineGuage(Graphics g, CVLineGuage edgeTool)
        {
            for (int i = 0; i < edgeTool.resultList.Count; i++)
            {
                if (edgeTool.property.SHOW_CONTOUR) { DrawLines(g, edgeTool.resultList[i].edgeList); }
                if (edgeTool.property.SHOW_EDGE) { DrawEdge(g, edgeTool.resultList[i].edgeList); }
                if (edgeTool.property.SHOW_FITLINE) { DrawFitLine(g, edgeTool.resultList[i].FitLine); }
                if (edgeTool.property.SHOW_VERTICAL_LINE) { DrawVerticalLines(g, Lib.Line.CLineVertical.GetVerticalLines(edgeTool.resultList[i].edgeList, edgeTool.size.Width, edgeTool.size.Height, edgeTool.property.POINT_RANGE, edgeTool.property.VER_PRJ_DIR)); }
            }
        }


        /// <summary>
        /// edgeTool_L, edgeTool_R 에서 엣지를 검출한 것을 Fit Line을 만들어서 교차여부를 구하고 Draw
        /// </summary>
        /// <param name="g"></param>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="TotalEdge_L"></param>
        /// <param name="TotalEdge_R"></param>
        /// <param name="size"></param>
        /// <param name="USE_MULTI_INSPECTION"></param>
        /// <param name="UseSpec"></param>
        /// <returns></returns>
        public static void DrawFitLinesInstersectionLines(Graphics g, CVLineGuage edgeToolL, CVLineGuage edgeToolR, OpenCvSharp.Point intersectionPoint)
        {
            try
            {
                DrawCVLineGuage(g, edgeToolL);
                DrawCVLineGuage(g, edgeToolR);
                DrawPoint(g, CConverter.CVPointToPointF(intersectionPoint));
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        /// <summary>
        /// edgeTool_L에서 엣지를 검출한 것을 수직선을 만들어서 edgeTool_R의 엣지 교차여부를 구하고 Draw
        /// </summary>
        /// <param name="g"></param>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="TotalEdge_L"></param>
        /// <param name="TotalEdge_R"></param>
        /// <param name="size"></param>
        /// <param name="USE_MULTI_INSPECTION"></param>
        /// <param name="UseSpec"></param>
        /// <returns></returns>
        public static void DrawInstersectionLines(Graphics g, CVLineGuage edgeToolL, CVLineGuage edgeToolR, List<CVLineGuage_VerticalLines> intersectionLines)
        {                        
            try
            {
                DrawCVLineGuage(g, edgeToolL);
                DrawCVLineGuage(g, edgeToolR);

                for (int i = 0; i < intersectionLines.Count; i++)
                {
                    DrawIntersectionPoint(g, intersectionLines[i].cLines, intersectionLines[i].intersectionLengths);                    
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public static void DrawBlob(Graphics g, List<CResultBlob> result, bool findBlackBlob)
        {
            int count = 1;
            foreach (var item in result)
            {
                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                if (findBlackBlob) { g.DrawString("B-" + (count).ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 10, (int)item.Bounding.Y - 10); }
                else { g.DrawString("W-" + (count).ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 10, (int)item.Bounding.Y - 10); }

                g.DrawString(item.Area.ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X, (int)item.Center.Y);
                count++;
            }

            //if (result.Count == 0)
            //{
            //    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(CVROI));
            //    g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)CVROI.X - 20, (int)CVROI.Y - 20);
            //}
        }

        public static void DrawLines(Graphics g, List<OpenCvSharp.Point> Edges)
        {
            if (Edges.Count == 0) 
            {
                CLOG.ABNORMAL("Not Exists Edge, Check Parameter");
                return; 
            }
            List<PointF> PointF = Edges.ConvertAll(new Converter<OpenCvSharp.Point, PointF>(CConverter.CVPointToPointF));
            g.DrawLines(new System.Drawing.Pen(System.Drawing.Color.Red, 3), PointF.ToArray());
        }

        public static void DrawFitLine(Graphics g, CLine fitLine)
        {            
            System.Drawing.Point center = new System.Drawing.Point((fitLine.Start.X + fitLine.End.X) / 2, (fitLine.Start.Y + fitLine.End.Y) / 2);

            double T = CFormula.CalculateAngle360(CConverter.CVPointToPointF(fitLine.Start), CConverter.CVPointToPointF(fitLine.End));
            g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Red, 5), CConverter.CVPointToPointF(fitLine.Start), CConverter.CVPointToPointF(fitLine.End));
            g.DrawString(T.ToString("F1") + "°", new Font("Arial", 25, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), center.X - 15, center.Y);
        }

        public static void DrawEdge(Graphics g, List<OpenCvSharp.Point> Edges)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                DrawDot(g, new System.Drawing.Pen(System.Drawing.Color.Green, 1), CConverter.CVPointToPoint(Edges[i]), 1);
            }
        }

        public static void DrawDot(Graphics graphics, System.Drawing.Pen pen, PointF point, float radius)
        {
            graphics.DrawEllipse
            (
                pen,
                point.X - radius / 2,
                point.Y - radius / 2,
                radius,
                radius
            );
        }

        public static void DrawIntersectionPoint(Graphics g, List<CLine> intersection_Lines, List<double> intersectionLengths)
        {
            using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Green, 3))
            {
                for (int h = 0; h < intersection_Lines.Count; h++)
                {
                    var line = intersection_Lines[h];        
                    double Distance = intersectionLengths[h];

                    g.DrawString(Distance.ToString("F2") + "mm", new Font("Arial", 8, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), line.Start.X - 15, line.Start.Y);
                    g.DrawString(Distance.ToString("F2") + "mm", new Font("Arial", 8, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), line.End.X + 5, line.End.Y);
                    g.DrawLine(pen, CConverter.CVPointToPointF(line.Start), CConverter.CVPointToPointF(line.End));

                    DrawPoint(g, CConverter.CVPointToPointF(line.Start));
                    DrawPoint(g, CConverter.CVPointToPointF(line.End));
                }
            }
        }

        public static void DrawVerticalLines(Graphics g, List<CLine> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Red, 1), CConverter.CVPointToPoint(lines[i].Start), CConverter.CVPointToPoint(lines[i].End));
            }
        }

        public static void DrawPoint(Graphics graphics, PointF point)
        {
            RectangleF rectangle = new RectangleF(point.X - 3, point.Y - 3, 6, 6);

            graphics.FillEllipse(Brushes.White, rectangle);

            graphics.DrawEllipse(Pens.Black, rectangle);
        }

        public static void DrawPoint(Graphics graphics, Point2d point)
        {
            RectangleF rectangle = new RectangleF((float)point.X - 3, (float)point.Y - 3, 6, 6);

            graphics.FillEllipse(Brushes.White, rectangle);

            graphics.DrawEllipse(Pens.Black, rectangle);
        }
    }
}
