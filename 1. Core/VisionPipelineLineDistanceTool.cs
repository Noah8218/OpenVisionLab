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

        public VisionPipelineLineDistanceTool(string name, LineGaugeProperty leftProperty, LineGaugeProperty rightProperty)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "LineDistance" : name;
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
                    "LineDistance input image is empty.",
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

                result.AddRange(VerticalLineCalculator.GetIntersectionLines(scanLines, rightResult.edgeList));
            }

            return result;
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
            }

            return metrics;
        }

        private static List<VisionToolOverlay> CreateOverlays(
            List<LineSegment2D> distanceLines,
            LineGaugeTool leftTool,
            LineGaugeTool rightTool)
        {
            List<VisionToolOverlay> overlays = new List<VisionToolOverlay>();

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
