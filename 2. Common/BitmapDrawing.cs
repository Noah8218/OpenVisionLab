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
    public static class BitmapDrawing
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

        public static void DrawLineGauge(Graphics g, LineGaugeTool edgeTool)
        {
            for (int i = 0; i < edgeTool.resultList.Count; i++)
            {
                if (edgeTool.property.SHOW_CONTOUR) { DrawLines(g, edgeTool.resultList[i].edgeList); }
                if (edgeTool.property.SHOW_EDGE) { DrawEdge(g, edgeTool.resultList[i].edgeList); }
                if (edgeTool.property.SHOW_FITLINE) { DrawFitLine(g, edgeTool.resultList[i].FitLine); }
                if (edgeTool.property.SHOW_VERTICAL_LINE) { DrawVerticalLines(g, Lib.Line.VerticalLineCalculator.GetVerticalLines(edgeTool.resultList[i].edgeList, edgeTool.size.Width, edgeTool.size.Height, edgeTool.property.POINT_RANGE, edgeTool.property.VER_PRJ_DIR)); }
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
        public static void DrawFitLinesInstersectionLines(Graphics g, LineGaugeTool edgeToolL, LineGaugeTool edgeToolR, OpenCvSharp.Point intersectionPoint)
        {
                        DrawLineGauge(g, edgeToolL);
            DrawLineGauge(g, edgeToolR);
            DrawPoint(g, CommonConverter.CVPointToPointF(intersectionPoint));
        
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
        public static void DrawInstersectionLines(Graphics g, LineGaugeTool edgeToolL, LineGaugeTool edgeToolR, List<LineGaugeVerticalLines> intersectionLines)
        {                        
                        DrawLineGauge(g, edgeToolL);
            DrawLineGauge(g, edgeToolR);

            for (int i = 0; i < intersectionLines.Count; i++)
            {
                DrawIntersectionPoint(g, intersectionLines[i].cLines, intersectionLines[i].intersectionLengths);                    
            }
        
        }

        public static void DrawBlob(Graphics g, List<BlobResult> result, bool findBlackBlob)
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
            //    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CommonConverter.CVRectToRect(CVROI));
            //    g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)CVROI.X - 20, (int)CVROI.Y - 20);
            //}
        }

        public static void DrawLines(Graphics g, List<OpenCvSharp.Point> Edges)
        {
            if (Edges.Count == 0) 
            {

                return; 
            }
            List<PointF> PointF = Edges.ConvertAll(new Converter<OpenCvSharp.Point, PointF>(CommonConverter.CVPointToPointF));
            g.DrawLines(new System.Drawing.Pen(System.Drawing.Color.Red, 3), PointF.ToArray());
        }

        public static void DrawFitLine(Graphics g, LineSegment2D fitLine)
        {            
            System.Drawing.Point center = new System.Drawing.Point((fitLine.Start.X + fitLine.End.X) / 2, (fitLine.Start.Y + fitLine.End.Y) / 2);

            double T = FormulaUtil.CalculateAngle360(CommonConverter.CVPointToPointF(fitLine.Start), CommonConverter.CVPointToPointF(fitLine.End));
            g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Red, 5), CommonConverter.CVPointToPointF(fitLine.Start), CommonConverter.CVPointToPointF(fitLine.End));
            g.DrawString(T.ToString("F1") + "°", new Font("Arial", 25, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), center.X - 15, center.Y);
        }

        public static void DrawEdge(Graphics g, List<OpenCvSharp.Point> Edges)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                DrawDot(g, new System.Drawing.Pen(System.Drawing.Color.Green, 1), CommonConverter.CVPointToPoint(Edges[i]), 1);
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

        public static void DrawIntersectionPoint(
            Graphics g,
            List<LineSegment2D> intersection_Lines,
            List<double> intersectionLengths)
        {
            if (g == null || intersection_Lines == null || intersectionLengths == null)
                return;

            int count = Math.Min(intersection_Lines.Count, intersectionLengths.Count);

            using (Pen pen = new Pen(Color.Green, 3))
            using (Font font = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Pixel))
            using (SolidBrush textBrush = new SolidBrush(Color.Lime))
            using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(160, Color.Black)))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                for (int h = 0; h < count; h++)
                {
                    var line = intersection_Lines[h];
                    double distance = intersectionLengths[h];

                    PointF start = CommonConverter.CVPointToPointF(line.Start);
                    PointF end = CommonConverter.CVPointToPointF(line.End);

                    // 1. 선 먼저
                    g.DrawLine(pen, start, end);

                    // 2. 포인트 먼저
                    DrawPoint(g, start);
                    DrawPoint(g, end);

                    // 3. 글자는 마지막에
                    string text = $"{distance:F2} mm";

                    PointF mid = new PointF(
                        (start.X + end.X) / 2f,
                        (start.Y + end.Y) / 2f);

                    SizeF textSize = g.MeasureString(text, font);

                    RectangleF textRect = new RectangleF(
                        mid.X - textSize.Width / 2f,
                        mid.Y - textSize.Height - 8,
                        textSize.Width,
                        textSize.Height);

                    // 배경을 살짝 깔면 영상 위에서도 잘 보입니다.
                    g.FillRectangle(bgBrush, textRect);
                    g.DrawString(text, font, textBrush, textRect);
                }
            }
        }

        public static void DrawVerticalLines(Graphics g, List<LineSegment2D> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Red, 1), CommonConverter.CVPointToPoint(lines[i].Start), CommonConverter.CVPointToPoint(lines[i].End));
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
