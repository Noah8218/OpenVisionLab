using Lib.Common;
using Lib.Line;
using Lib.OpenCV;
using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineLineDistanceTool : IVisionTool
    {
        private readonly LineGaugeProperty leftProperty;
        private readonly LineGaugeProperty rightProperty;
        private readonly Dictionary<string, string> parameters;

        public VisionPipelineLineDistanceTool(
            string name,
            LineGaugeProperty leftProperty,
            LineGaugeProperty rightProperty,
            IDictionary<string, string> parameters = null)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "LineDistance" : name;
            this.leftProperty = leftProperty ?? throw new ArgumentNullException(nameof(leftProperty));
            this.rightProperty = rightProperty ?? throw new ArgumentNullException(nameof(rightProperty));
            this.parameters = new Dictionary<string, string>(parameters ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
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
                    "LineDistance input image is empty.",
                    stopwatch.Elapsed);
            }

            if (parameters.TryGetValue(VisionPipelineGapEdgePairTool.UseParameter, out string useGapText)
                && bool.TryParse(useGapText, out bool useGap)
                && useGap)
            {
                return new VisionPipelineGapEdgePairTool(Name, leftProperty, parameters).Execute(source);
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
                    "LineDistance left edge failed. " + leftResult.Message,
                    stopwatch.Elapsed,
                    leftResult.Exception);
            }

            VisionToolResult rightResult = rightTool.Execute(source);
            if (!rightResult.Success)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    rightResult.ErrorCode == VisionToolErrorCode.None ? VisionToolErrorCode.LineGaugeEdgeNotFound : rightResult.ErrorCode,
                    "LineDistance right edge failed. " + rightResult.Message,
                    stopwatch.Elapsed,
                    rightResult.Exception);
            }

            List<LineSegment2D> distanceLines = CreateDistanceLines(leftTool, rightTool);
            if (distanceLines.Count == 0)
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.LineGaugeEdgeNotFound,
                    "LineDistance found edge points but could not create distance lines. Check ROI, projection direction, polarity, POINT_RANGE, and VER_PRJ_DIR.",
                    stopwatch.Elapsed);
            }

            Mat resultImage = CreateResultImage(source);
            Dictionary<string, double> metrics = CreateMetrics(distanceLines, leftTool, rightTool, resultImage);
            List<VisionToolOverlay> overlays = CreateOverlays(distanceLines, leftTool, rightTool);
            DrawResult(resultImage, distanceLines, metrics);

            stopwatch.Stop();
            return VisionToolResult.Passed(resultImage, stopwatch.Elapsed, metrics, overlays);
        }

        private static List<LineSegment2D> CreateDistanceLines(LineGaugeTool leftTool, LineGaugeTool rightTool)
        {
            List<LineSegment2D> result = new List<LineSegment2D>();
            int count = Math.Min(leftTool.resultList?.Count ?? 0, rightTool.resultList?.Count ?? 0);
            bool useFittedEdges = leftTool.property.USE_EXTEND_FIT_LINE
                && rightTool.property.USE_EXTEND_FIT_LINE;

            for (int i = 0; i < count; i++)
            {
                LineGaugeResult leftResult = leftTool.resultList[i];
                LineGaugeResult rightResult = rightTool.resultList[i];
                if (leftResult?.edgeList == null || rightResult?.edgeList == null)
                {
                    continue;
                }

                List<LineSegment2D> scanLines = leftTool.property.USE_MANUAL_ANGLE
                    ? VerticalLineCalculator.GetVerticalLinesManual(
                        leftResult.edgeList,
                        leftTool.size.Width,
                        leftTool.size.Height,
                        leftTool.property.MANUAL_ANGLE_VALUE,
                        leftTool.property.VER_PRJ_DIR)
                    : VerticalLineCalculator.GetVerticalLines(
                        leftResult.edgeList,
                        leftTool.size.Width,
                        leftTool.size.Height,
                        leftTool.property.POINT_RANGE,
                        leftTool.property.VER_PRJ_DIR);

                if (useFittedEdges)
                {
                    if (!IsUsableFitLine(leftResult.FitLine)
                        || !IsUsableFitLine(rightResult.FitLine))
                    {
                        continue;
                    }

                    foreach (LineSegment2D scanLine in scanLines)
                    {
                        if (!TryIntersectInfiniteLines(scanLine, leftResult.FitLine, out OpenCvSharp.Point left)
                            || !TryIntersectInfiniteLines(scanLine, rightResult.FitLine, out OpenCvSharp.Point right))
                        {
                            continue;
                        }

                        LineSegment2D fittedDistance = new LineSegment2D(left, right);
                        if (IsInsideImage(left, leftTool.size)
                            && IsInsideImage(right, leftTool.size)
                            && IsInsideRoi(left, leftTool.property.CvROI)
                            && IsInsideRoi(right, rightTool.property.CvROI)
                            && fittedDistance.Distance() > 0)
                        {
                            result.Add(fittedDistance);
                        }
                    }
                }
                else
                {
                    result.AddRange(VerticalLineCalculator.GetIntersectionLines(scanLines, rightResult.edgeList));
                }
            }

            return result;
        }

        private static bool IsUsableFitLine(LineSegment2D line)
        {
            return line != null && line.Distance() > 0;
        }

        private static bool IsInsideImage(OpenCvSharp.Point point, OpenCvSharp.Size size)
        {
            return point.X >= 0 && point.X < size.Width
                && point.Y >= 0 && point.Y < size.Height;
        }

        private static bool IsInsideRoi(OpenCvSharp.Point point, Rect roi)
        {
            return roi.Width <= 0 || roi.Height <= 0
                || (point.X >= roi.X && point.X < roi.Right
                    && point.Y >= roi.Y && point.Y < roi.Bottom);
        }

        private static bool TryIntersectInfiniteLines(
            LineSegment2D first,
            LineSegment2D second,
            out OpenCvSharp.Point intersection)
        {
            intersection = default;
            double x1 = first.Start.X;
            double y1 = first.Start.Y;
            double x2 = first.End.X;
            double y2 = first.End.Y;
            double x3 = second.Start.X;
            double y3 = second.Start.Y;
            double x4 = second.End.X;
            double y4 = second.End.Y;
            double denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (Math.Abs(denominator) <= 0.000001d)
            {
                return false;
            }

            double firstCross = x1 * y2 - y1 * x2;
            double secondCross = x3 * y4 - y3 * x4;
            double x = (firstCross * (x3 - x4) - (x1 - x2) * secondCross) / denominator;
            double y = (firstCross * (y3 - y4) - (y1 - y2) * secondCross) / denominator;
            if (double.IsNaN(x) || double.IsInfinity(x) || double.IsNaN(y) || double.IsInfinity(y))
            {
                return false;
            }

            intersection = new OpenCvSharp.Point(
                (int)Math.Round(x, MidpointRounding.AwayFromZero),
                (int)Math.Round(y, MidpointRounding.AwayFromZero));
            return true;
        }

        private static Mat CreateResultImage(Mat source)
        {
            if (source.Channels() == 1)
            {
                Mat color = new Mat();
                Cv2.CvtColor(source, color, ColorConversionCodes.GRAY2BGR);
                return color;
            }

            return source.Clone();
        }

        private double PixelPerMm => leftProperty.PIXELPERMM > 0 ? leftProperty.PIXELPERMM : rightProperty.PIXELPERMM;

        private Dictionary<string, double> CreateMetrics(
            List<LineSegment2D> distanceLines,
            LineGaugeTool leftTool,
            LineGaugeTool rightTool,
            Mat resultImage)
        {
            List<double> distances = distanceLines.Select(line => line.Distance()).Where(value => value > 0).ToList();
            double pixelPerMm = PixelPerMm;

            Dictionary<string, double> metrics = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.ResultCount] = distanceLines.Count,
                [VisionPipelineKnownMetrics.DistanceCount] = distanceLines.Count,
                [VisionPipelineKnownMetrics.DistancePxMin] = distances.Min(),
                [VisionPipelineKnownMetrics.DistancePxMax] = distances.Max(),
                [VisionPipelineKnownMetrics.DistancePxAvg] = distances.Average(),
                [VisionPipelineKnownMetrics.DistancePxRange] = distances.Max() - distances.Min(),
                [VisionPipelineKnownMetrics.EdgeCount] = (leftTool.resultList?.Sum(item => item.EdgeCount) ?? 0)
                    + (rightTool.resultList?.Sum(item => item.EdgeCount) ?? 0),
                [VisionPipelineKnownMetrics.EdgePointCount] = (leftTool.resultList?.Sum(item => item.EdgePointCount) ?? 0)
                    + (rightTool.resultList?.Sum(item => item.EdgePointCount) ?? 0),
                [VisionPipelineKnownMetrics.SourceImageWidth] = leftTool.size.Width,
                [VisionPipelineKnownMetrics.SourceImageHeight] = leftTool.size.Height,
                [VisionPipelineKnownMetrics.SourceImageChannels] = leftTool.imageSource?.Channels() ?? 0,
                [VisionPipelineKnownMetrics.ResultImageWidth] = resultImage.Width,
                [VisionPipelineKnownMetrics.ResultImageHeight] = resultImage.Height,
                [VisionPipelineKnownMetrics.ResultImageChannels] = resultImage.Channels()
            };

            if (pixelPerMm > 0)
            {
                metrics[VisionPipelineKnownMetrics.DistanceMmMin] = metrics[VisionPipelineKnownMetrics.DistancePxMin] * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.DistanceMmMax] = metrics[VisionPipelineKnownMetrics.DistancePxMax] * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.DistanceMmAvg] = metrics[VisionPipelineKnownMetrics.DistancePxAvg] * pixelPerMm;
                metrics[VisionPipelineKnownMetrics.DistanceMmRange] = metrics[VisionPipelineKnownMetrics.DistancePxRange] * pixelPerMm;
            }

            return metrics;
        }

        private static List<VisionToolOverlay> CreateOverlays(
            List<LineSegment2D> distanceLines,
            LineGaugeTool leftTool,
            LineGaugeTool rightTool)
        {
            List<VisionToolOverlay> overlays = new List<VisionToolOverlay>();

            Rect leftRoi = leftTool?.property?.CvROI ?? default;
            Rect rightRoi = rightTool?.property?.CvROI ?? default;
            bool hasLeftRoi = HasArea(leftRoi);
            bool hasRightRoi = HasArea(rightRoi);
            if (hasLeftRoi && hasRightRoi && leftRoi == rightRoi)
            {
                AddRoiOverlay(overlays, leftRoi, "Measurement ROI");
            }
            else
            {
                if (hasLeftRoi)
                {
                    AddRoiOverlay(overlays, leftRoi, "Line A ROI");
                }

                if (hasRightRoi)
                {
                    AddRoiOverlay(overlays, rightRoi, "Line B ROI");
                }
            }

            if (leftTool.property.USE_EXTEND_FIT_LINE
                && rightTool.property.USE_EXTEND_FIT_LINE)
            {
                AddFitLineOverlays(overlays, leftTool, "Line A fitted edge");
                AddFitLineOverlays(overlays, rightTool, "Line B fitted edge");
            }

            int index = 1;
            foreach (LineSegment2D line in distanceLines)
            {
                overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Line,
                    Label = $"D{index++}",
                    Start = ToPointF(line.Start),
                    End = ToPointF(line.End),
                    Center = new PointF((line.Start.X + line.End.X) / 2f, (line.Start.Y + line.End.Y) / 2f)
                });
            }

            AddEdgePoints(overlays, leftTool, "Left edges");
            AddEdgePoints(overlays, rightTool, "Right edges");
            return overlays;
        }

        private static bool HasArea(Rect roi)
        {
            return roi.Width > 0 && roi.Height > 0;
        }

        private static void AddRoiOverlay(List<VisionToolOverlay> overlays, Rect roi, string label)
        {
            overlays.Add(new VisionToolOverlay
            {
                Kind = VisionToolOverlayKind.Rectangle,
                Label = label,
                Bounds = new RectangleF(roi.X, roi.Y, roi.Width, roi.Height)
            });
        }

        private static void AddFitLineOverlays(
            List<VisionToolOverlay> overlays,
            LineGaugeTool tool,
            string label)
        {
            foreach (LineGaugeResult result in tool?.resultList ?? new List<LineGaugeResult>())
            {
                if (!IsUsableFitLine(result?.FitLine))
                {
                    continue;
                }

                overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Line,
                    Label = label,
                    Start = ToPointF(result.FitLine.Start),
                    End = ToPointF(result.FitLine.End),
                    Center = new PointF(
                        (result.FitLine.Start.X + result.FitLine.End.X) / 2f,
                        (result.FitLine.Start.Y + result.FitLine.End.Y) / 2f)
                });
            }
        }

        private static void AddEdgePoints(List<VisionToolOverlay> overlays, LineGaugeTool tool, string label)
        {
            List<PointF> points = (tool.resultList ?? new List<LineGaugeResult>())
                .Where(result => result?.edgeList != null)
                .SelectMany(result => result.edgeList)
                .Select(ToPointF)
                .ToList();

            if (points.Count == 0)
            {
                return;
            }

            VisionToolOverlay overlay = new VisionToolOverlay
            {
                Kind = VisionToolOverlayKind.Points,
                Label = label
            };
            overlay.Points.AddRange(points);
            overlays.Add(overlay);
        }

        private void DrawResult(Mat resultImage, List<LineSegment2D> distanceLines, Dictionary<string, double> metrics)
        {
            Scalar distanceColor = new Scalar(0, 0, 255);
            Scalar labelColor = new Scalar(0, 255, 0);
            double pixelPerMm = PixelPerMm;

            for (int i = 0; i < distanceLines.Count; i++)
            {
                LineSegment2D line = distanceLines[i];
                Cv2.Line(resultImage, line.Start, line.End, distanceColor, 1, LineTypes.AntiAlias);

                if (i % 2 != 0 && distanceLines.Count > 16)
                {
                    continue;
                }

                double distance = line.Distance();
                string text = pixelPerMm > 0
                    ? $"{distance * pixelPerMm:0.###} mm"
                    : $"{distance:0.#} px";
                OpenCvSharp.Point labelPoint = new OpenCvSharp.Point(
                    Math.Min(line.Start.X, line.End.X) + 4,
                    Math.Min(line.Start.Y, line.End.Y) - 2);
                Cv2.PutText(resultImage, text, labelPoint, HersheyFonts.HersheySimplex, 0.35, labelColor, 1, LineTypes.AntiAlias);
            }
        }

        private static PointF ToPointF(OpenCvSharp.Point point)
        {
            return new PointF(point.X, point.Y);
        }
    }
}
