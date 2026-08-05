using OpenVisionLab.Core.Geometry2D;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Result;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineLineIntersectionTool : IVisionTool
    {
        private readonly LineGaugeProperty leftProperty;
        private readonly LineGaugeProperty rightProperty;

        public VisionPipelineLineIntersectionTool(string name, LineGaugeProperty leftProperty, LineGaugeProperty rightProperty)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "LineIntersection" : name;
            this.leftProperty = leftProperty ?? throw new ArgumentNullException(nameof(leftProperty));
            this.rightProperty = rightProperty ?? throw new ArgumentNullException(nameof(rightProperty));
        }

        public string Name { get; }

        public VisionToolResult Execute(Mat source)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            if (OpenCvHelper.IsImageEmpty(source))
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InputImageInvalid,
                    "LineIntersection input image is empty.",
                    stopwatch.Elapsed);
            }

            LineGaugeTool leftTool = new LineGaugeTool();
            LineGaugeTool rightTool = new LineGaugeTool();
            leftTool.SetProperty((LineGaugeProperty)leftProperty.DeepCopy());
            rightTool.SetProperty((LineGaugeProperty)rightProperty.DeepCopy());

            VisionToolResult leftResult = leftTool.Execute(source);
            if (!leftResult.Success)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    leftResult.ErrorCode == VisionToolErrorCode.None ? VisionToolErrorCode.LineGaugeEdgeNotFound : leftResult.ErrorCode,
                    "LineIntersection left edge failed. " + leftResult.Message,
                    stopwatch.Elapsed,
                    leftResult.Exception);
            }

            VisionToolResult rightResult = rightTool.Execute(source);
            if (!rightResult.Success)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    rightResult.ErrorCode == VisionToolErrorCode.None ? VisionToolErrorCode.LineGaugeEdgeNotFound : rightResult.ErrorCode,
                    "LineIntersection right edge failed. " + rightResult.Message,
                    stopwatch.Elapsed,
                    rightResult.Exception);
            }

            bool crosses = TryFindFitLineIntersection(leftTool, rightTool, out OpenCvSharp.Point intersection);
            Mat resultImage = CreateResultImage(source, leftTool, rightTool, crosses, intersection);
            Dictionary<string, double> metrics = CreateMetrics(source, resultImage, leftTool, rightTool, crosses, intersection);
            List<VisionToolOverlay> overlays = CreateOverlays(leftTool, rightTool, crosses, intersection);

            stopwatch.Stop();
            return VisionToolResult.Passed(resultImage, stopwatch.Elapsed, metrics, overlays);
        }

        private static Mat CreateResultImage(Mat source, LineGaugeTool leftTool, LineGaugeTool rightTool, bool crosses, OpenCvSharp.Point intersection)
        {
            Mat resultImage = CreateColorResultImage(source);
            DrawLineGauge(resultImage, leftTool, new Scalar(0, 220, 80), new Scalar(0, 140, 255));
            DrawLineGauge(resultImage, rightTool, new Scalar(255, 180, 0), new Scalar(255, 80, 180));

            if (crosses)
            {
                DrawExtendedFitLine(resultImage, leftTool, new Scalar(0, 0, 255));
                DrawExtendedFitLine(resultImage, rightTool, new Scalar(0, 0, 255));
                DrawIntersectionMarker(resultImage, intersection);
            }

            return resultImage;
        }

        private static Mat CreateColorResultImage(Mat source)
        {
            if (source.Channels() == 1)
            {
                Mat color = new Mat();
                Cv2.CvtColor(source, color, ColorConversionCodes.GRAY2BGR);
                return color;
            }

            if (source.Channels() == 4)
            {
                Mat color = new Mat();
                Cv2.CvtColor(source, color, ColorConversionCodes.BGRA2BGR);
                return color;
            }

            return source.Clone();
        }

        private static void DrawLineGauge(Mat image, LineGaugeTool tool, Scalar edgeColor, Scalar roiColor)
        {
            if (image == null || image.Empty() || tool == null)
            {
                return;
            }

            DrawGaugeRois(image, tool, roiColor);

            foreach (LineGaugeResult result in tool.resultList ?? Enumerable.Empty<LineGaugeResult>())
            {
                foreach (OpenCvSharp.Point point in result?.edgeList ?? Enumerable.Empty<OpenCvSharp.Point>())
                {
                    Cv2.Circle(image, point, 2, edgeColor, -1, LineTypes.AntiAlias);
                }

                if (result?.FitLine != null)
                {
                    Cv2.Line(image, result.FitLine.Start, result.FitLine.End, edgeColor, 2, LineTypes.AntiAlias);
                }
            }
        }

        private static void DrawGaugeRois(Mat image, LineGaugeTool tool, Scalar roiColor)
        {
            var property = tool?.property;
            if (property == null || !property.USE_ROI)
            {
                return;
            }

            IEnumerable<Rect> rois = property.USE_MULTI_ROI
                ? property.CvROIS ?? Enumerable.Empty<Rect>()
                : new[] { property.CvROI };

            foreach (Rect roi in rois)
            {
                Rect clamped = ClampRect(roi, image.Width, image.Height);
                if (clamped.Width > 0 && clamped.Height > 0)
                {
                    Cv2.Rectangle(image, clamped, roiColor, 1, LineTypes.AntiAlias);
                }
            }
        }

        private static Rect ClampRect(Rect rect, int width, int height)
        {
            int x = Math.Max(0, Math.Min(width - 1, rect.X));
            int y = Math.Max(0, Math.Min(height - 1, rect.Y));
            int right = Math.Max(x, Math.Min(width, rect.X + rect.Width));
            int bottom = Math.Max(y, Math.Min(height, rect.Y + rect.Height));
            return new Rect(x, y, right - x, bottom - y);
        }

        private static void DrawIntersectionMarker(Mat image, OpenCvSharp.Point intersection)
        {
            Scalar color = new Scalar(0, 255, 0);
            Cv2.Circle(image, intersection, 6, color, 2, LineTypes.AntiAlias);
            Cv2.Line(
                image,
                new OpenCvSharp.Point(intersection.X - 10, intersection.Y),
                new OpenCvSharp.Point(intersection.X + 10, intersection.Y),
                color,
                2,
                LineTypes.AntiAlias);
            Cv2.Line(
                image,
                new OpenCvSharp.Point(intersection.X, intersection.Y - 10),
                new OpenCvSharp.Point(intersection.X, intersection.Y + 10),
                color,
                2,
                LineTypes.AntiAlias);
        }

        private static Dictionary<string, double> CreateMetrics(
            Mat source,
            Mat resultImage,
            LineGaugeTool leftTool,
            LineGaugeTool rightTool,
            bool crosses,
            OpenCvSharp.Point intersection)
        {
            return new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.ResultCount] = crosses ? 1 : 0,
                [VisionPipelineKnownMetrics.EdgeCount] = (leftTool.resultList?.Sum(item => item.EdgeCount) ?? 0)
                    + (rightTool.resultList?.Sum(item => item.EdgeCount) ?? 0),
                [VisionPipelineKnownMetrics.EdgePointCount] = (leftTool.resultList?.Sum(item => item.EdgePointCount) ?? 0)
                    + (rightTool.resultList?.Sum(item => item.EdgePointCount) ?? 0),
                ["IntersectionCross"] = crosses ? 1 : 0,
                [VisionPipelineKnownMetrics.IntersectionX] = intersection.X,
                [VisionPipelineKnownMetrics.IntersectionY] = intersection.Y,
                [VisionPipelineKnownMetrics.SourceImageWidth] = source.Width,
                [VisionPipelineKnownMetrics.SourceImageHeight] = source.Height,
                [VisionPipelineKnownMetrics.SourceImageChannels] = source.Channels(),
                [VisionPipelineKnownMetrics.ResultImageWidth] = resultImage.Width,
                [VisionPipelineKnownMetrics.ResultImageHeight] = resultImage.Height,
                [VisionPipelineKnownMetrics.ResultImageChannels] = resultImage.Channels()
            };
        }

        private static List<VisionToolOverlay> CreateOverlays(
            LineGaugeTool leftTool,
            LineGaugeTool rightTool,
            bool crosses,
            OpenCvSharp.Point intersection)
        {
            List<VisionToolOverlay> overlays = new List<VisionToolOverlay>();
            AddFitLine(overlays, leftTool, "Line A");
            AddFitLine(overlays, rightTool, "Line B");
            if (crosses)
            {
                overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Point,
                    Label = "Intersection",
                    Center = new PointF(intersection.X, intersection.Y)
                });
            }

            return overlays;
        }

        private static bool TryFindFitLineIntersection(LineGaugeTool leftTool, LineGaugeTool rightTool, out OpenCvSharp.Point intersection)
        {
            intersection = new OpenCvSharp.Point();
            LineGaugeResult left = leftTool?.resultList?.FirstOrDefault(item => item?.FitLine != null);
            LineGaugeResult right = rightTool?.resultList?.FirstOrDefault(item => item?.FitLine != null);
            if (left?.FitLine == null || right?.FitLine == null)
            {
                return false;
            }

            OpenCvSharp.Point p = left.FitLine.Start;
            OpenCvSharp.Point p2 = left.FitLine.End;
            OpenCvSharp.Point q = right.FitLine.Start;
            OpenCvSharp.Point q2 = right.FitLine.End;
            double a1 = p2.Y - p.Y;
            double b1 = p.X - p2.X;
            double c1 = (a1 * p.X) + (b1 * p.Y);
            double a2 = q2.Y - q.Y;
            double b2 = q.X - q2.X;
            double c2 = (a2 * q.X) + (b2 * q.Y);
            double determinant = (a1 * b2) - (a2 * b1);
            if (Math.Abs(determinant) < 0.000001)
            {
                return false;
            }

            intersection = new OpenCvSharp.Point(
                (int)Math.Round(((b2 * c1) - (b1 * c2)) / determinant),
                (int)Math.Round(((a1 * c2) - (a2 * c1)) / determinant));
            return true;
        }

        private static void DrawExtendedFitLine(Mat image, LineGaugeTool tool, Scalar color)
        {
            LineGaugeResult result = tool?.resultList?.FirstOrDefault(item => item?.FitLine != null);
            if (result?.FitLine == null || image == null || image.Empty() || image.Width <= 1 || image.Height <= 1)
            {
                return;
            }

            if (!TryClipInfiniteLine(result.FitLine, image.Width, image.Height, out PointF start, out PointF end))
            {
                start = new PointF(result.FitLine.Start.X, result.FitLine.Start.Y);
                end = new PointF(result.FitLine.End.X, result.FitLine.End.Y);
            }

            Cv2.Line(image, ToPoint(start), ToPoint(end), color, 3, LineTypes.AntiAlias);
        }

        private static OpenCvSharp.Point ToPoint(PointF point)
        {
            return new OpenCvSharp.Point((int)Math.Round(point.X), (int)Math.Round(point.Y));
        }

        private static bool TryClipInfiniteLine(LineSegment2D line, int width, int height, out PointF start, out PointF end)
        {
            start = default;
            end = default;
            double x1 = line.Start.X;
            double y1 = line.Start.Y;
            double x2 = line.End.X;
            double y2 = line.End.Y;
            double dx = x2 - x1;
            double dy = y2 - y1;
            const double epsilon = 0.000001D;
            if (Math.Abs(dx) < epsilon && Math.Abs(dy) < epsilon)
            {
                return false;
            }

            List<PointF> points = new List<PointF>();
            AddClippedPoint(points, 0D, Math.Abs(dx) < epsilon ? y1 : y1 + ((0D - x1) * dy / dx), width, height, Math.Abs(dx) >= epsilon);
            AddClippedPoint(points, width - 1D, Math.Abs(dx) < epsilon ? y1 : y1 + (((width - 1D) - x1) * dy / dx), width, height, Math.Abs(dx) >= epsilon);
            AddClippedPoint(points, Math.Abs(dy) < epsilon ? x1 : x1 + ((0D - y1) * dx / dy), 0D, width, height, Math.Abs(dy) >= epsilon);
            AddClippedPoint(points, Math.Abs(dy) < epsilon ? x1 : x1 + (((height - 1D) - y1) * dx / dy), height - 1D, width, height, Math.Abs(dy) >= epsilon);

            List<PointF> distinct = points
                .GroupBy(point => $"{Math.Round(point.X, 2):0.00},{Math.Round(point.Y, 2):0.00}")
                .Select(group => group.First())
                .ToList();
            if (distinct.Count < 2)
            {
                return false;
            }

            start = distinct[0];
            PointF startPoint = start;
            end = distinct
                .Skip(1)
                .OrderByDescending(point => DistanceSquared(startPoint, point))
                .First();
            return true;
        }

        private static void AddClippedPoint(List<PointF> points, double x, double y, int width, int height, bool isValid)
        {
            if (!isValid || double.IsNaN(x) || double.IsNaN(y) || double.IsInfinity(x) || double.IsInfinity(y))
            {
                return;
            }

            if (x >= -0.5D && x <= width - 0.5D && y >= -0.5D && y <= height - 0.5D)
            {
                points.Add(new PointF(
                    (float)Math.Max(0D, Math.Min(width - 1D, x)),
                    (float)Math.Max(0D, Math.Min(height - 1D, y))));
            }
        }

        private static double DistanceSquared(PointF first, PointF second)
        {
            double dx = first.X - second.X;
            double dy = first.Y - second.Y;
            return (dx * dx) + (dy * dy);
        }

        private static void AddFitLine(List<VisionToolOverlay> overlays, LineGaugeTool tool, string label)
        {
            LineGaugeResult result = tool?.resultList?.FirstOrDefault(item => item?.FitLine != null);
            if (result?.FitLine == null)
            {
                return;
            }

            overlays.Add(new VisionToolOverlay
            {
                Kind = VisionToolOverlayKind.Line,
                Label = label,
                Start = new PointF(result.FitLine.Start.X, result.FitLine.Start.Y),
                End = new PointF(result.FitLine.End.X, result.FitLine.End.Y),
                Center = new PointF(
                    (result.FitLine.Start.X + result.FitLine.End.X) / 2f,
                    (result.FitLine.Start.Y + result.FitLine.End.Y) / 2f)
            });
        }
    }
}
