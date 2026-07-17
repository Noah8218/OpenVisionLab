using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using CvPoint = OpenCvSharp.Point;

namespace OpenVisionLab
{
    internal static class VisionPipelineOverlayMergeService
    {
        private sealed class MergeOptions
        {
            public HashSet<string> SourceLayers { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public HashSet<string> SourceSteps { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public bool BurnIn { get; set; } = true;
            public bool DrawLabels { get; set; }
            public bool AllowEmpty { get; set; }
            public int MaxPoints { get; set; } = 300;
        }

        public static bool IsMergeTool(string toolType)
        {
            string normalized = VisionPipelineNormalizer.NormalizeToolType(toolType);
            return normalized == "overlaymerge"
                || normalized == "resultmerge"
                || normalized == "mergeresult";
        }

        public static VisionToolResult Execute(
            VisionPipelineStep step,
            Mat input,
            VisionPipelineRunResult runResult)
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            if (input == null || input.Empty())
            {
                stopwatch.Stop();
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InputImageInvalid,
                    "OverlayMerge input image is empty.",
                    stopwatch.Elapsed);
            }

            MergeOptions options = ResolveOptions(step?.Parameters);
            List<VisionPipelineStepResult> sourceResults = SelectSourceResults(runResult, options).ToList();
            List<VisionToolOverlay> overlays = sourceResults
                .SelectMany(result => result?.ToolResult?.Overlays ?? Enumerable.Empty<VisionToolOverlay>())
                .Where(overlay => overlay != null)
                .Select(CloneOverlay)
                .ToList();

            if (overlays.Count == 0 && !options.AllowEmpty)
            {
                stopwatch.Stop();
                string sourceText = BuildSourceDescription(options);
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InvalidParameter,
                    $"OverlayMerge found no previous overlays. {sourceText}",
                    stopwatch.Elapsed);
            }

            Mat resultImage = CreateDisplayImage(input);
            if (options.BurnIn)
            {
                DrawOverlays(resultImage, sourceResults, options);
            }

            stopwatch.Stop();
            Dictionary<string, double> metrics = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.ResultCount] = overlays.Count,
                [VisionPipelineKnownMetrics.MergeOverlayCount] = overlays.Count,
                [VisionPipelineKnownMetrics.MergeSourceCount] = sourceResults.Count(result => result?.ToolResult?.Overlays?.Count > 0)
            };

            return VisionToolResult.Passed(resultImage, stopwatch.Elapsed, metrics, overlays);
        }

        private static MergeOptions ResolveOptions(IDictionary<string, string> parameters)
        {
            return new MergeOptions
            {
                SourceLayers = SplitList(GetValue(parameters, "SourceLayers"))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase),
                SourceSteps = SplitList(GetValue(parameters, "SourceSteps"))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase),
                BurnIn = GetBool(parameters, "BurnIn", true),
                DrawLabels = GetBool(parameters, "DrawLabels", false),
                AllowEmpty = GetBool(parameters, "AllowEmpty", false),
                MaxPoints = Math.Max(0, GetInt(parameters, "MaxPoints", 300))
            };
        }

        private static IEnumerable<VisionPipelineStepResult> SelectSourceResults(
            VisionPipelineRunResult runResult,
            MergeOptions options)
        {
            IEnumerable<VisionPipelineStepResult> candidates = runResult?.StepResults
                ?? Enumerable.Empty<VisionPipelineStepResult>();

            foreach (VisionPipelineStepResult result in candidates)
            {
                VisionPipelineStep sourceStep = result?.Step;
                if (sourceStep == null
                    || result.Skipped
                    || result.ToolResult?.Success != true
                    || result.ToolResult.Overlays.Count == 0)
                {
                    continue;
                }

                bool layerSelected = options.SourceLayers.Count == 0
                    || options.SourceLayers.Contains(sourceStep.OutputLayer ?? string.Empty);
                bool stepSelected = options.SourceSteps.Count == 0
                    || options.SourceSteps.Contains(sourceStep.Name ?? string.Empty);
                if (layerSelected && stepSelected)
                {
                    yield return result;
                }
            }
        }

        private static Mat CreateDisplayImage(Mat source)
        {
            Mat result = new Mat();
            int channels = source.Channels();
            if (channels == 1)
            {
                Cv2.CvtColor(source, result, ColorConversionCodes.GRAY2BGR);
                return result;
            }

            if (channels == 4)
            {
                Cv2.CvtColor(source, result, ColorConversionCodes.BGRA2BGR);
                return result;
            }

            return source.Clone();
        }

        private static void DrawOverlays(Mat image, IReadOnlyList<VisionPipelineStepResult> sourceResults, MergeOptions options)
        {
            if (image == null || image.Empty())
            {
                return;
            }

            for (int sourceIndex = 0; sourceIndex < sourceResults.Count; sourceIndex++)
            {
                VisionPipelineStepResult sourceResult = sourceResults[sourceIndex];
                Scalar color = ResolveSourceColor(sourceIndex);
                foreach (VisionToolOverlay overlay in sourceResult?.ToolResult?.Overlays ?? Enumerable.Empty<VisionToolOverlay>())
                {
                    if (overlay == null)
                    {
                        continue;
                    }

                    DrawOverlay(image, overlay, color, options);
                }
            }
        }

        private static void DrawOverlay(Mat image, VisionToolOverlay overlay, Scalar color, MergeOptions options)
        {
            switch (overlay.Kind)
            {
                case VisionToolOverlayKind.Rectangle:
                    DrawRectangle(image, overlay, color, options.DrawLabels);
                    break;
                case VisionToolOverlayKind.Point:
                    DrawPoint(image, overlay.Center, color);
                    if (options.DrawLabels)
                    {
                        DrawLabel(image, overlay.Label, overlay.Center, color);
                    }
                    break;
                case VisionToolOverlayKind.Points:
                    DrawPoints(image, overlay.Points, color, options.MaxPoints);
                    break;
                case VisionToolOverlayKind.Line:
                    DrawLine(image, overlay, color, options.DrawLabels);
                    break;
            }
        }

        private static void DrawRectangle(Mat image, VisionToolOverlay overlay, Scalar color, bool drawLabel)
        {
            Rect rect = ToImageRect(overlay.Bounds, image.Width, image.Height);
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            Cv2.Rectangle(image, rect, color, 2);
            DrawPoint(image, overlay.Center, color);
            if (drawLabel)
            {
                DrawLabel(image, overlay.Label, new PointF(rect.X, rect.Y), color);
            }
        }

        private static void DrawLine(Mat image, VisionToolOverlay overlay, Scalar color, bool drawLabel)
        {
            CvPoint start = ToImagePoint(overlay.Start, image.Width, image.Height);
            CvPoint end = ToImagePoint(overlay.End, image.Width, image.Height);
            if (start == end)
            {
                return;
            }

            Cv2.Line(image, start, end, color, 2);
            PointF center = new PointF((start.X + end.X) / 2F, (start.Y + end.Y) / 2F);
            DrawPoint(image, center, color);
            if (drawLabel)
            {
                DrawLabel(image, overlay.Label, center, color);
            }
        }

        private static void DrawPoints(Mat image, IEnumerable<PointF> points, Scalar color, int maxPoints)
        {
            if (maxPoints <= 0)
            {
                return;
            }

            int count = 0;
            foreach (PointF point in points ?? Enumerable.Empty<PointF>())
            {
                if (count++ >= maxPoints)
                {
                    break;
                }

                Cv2.Circle(image, ToImagePoint(point, image.Width, image.Height), 2, color, -1);
            }
        }

        private static void DrawPoint(Mat image, PointF point, Scalar color)
        {
            CvPoint center = ToImagePoint(point, image.Width, image.Height);
            Cv2.Line(image, new CvPoint(center.X - 4, center.Y), new CvPoint(center.X + 4, center.Y), color, 1);
            Cv2.Line(image, new CvPoint(center.X, center.Y - 4), new CvPoint(center.X, center.Y + 4), color, 1);
        }

        private static void DrawLabel(Mat image, string label, PointF anchor, Scalar color)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return;
            }

            CvPoint point = ToImagePoint(anchor, image.Width, image.Height);
            int x = Math.Max(0, Math.Min(point.X, Math.Max(0, image.Width - 8)));
            int y = Math.Max(12, Math.Min(point.Y - 4, Math.Max(12, image.Height - 4)));
            Cv2.PutText(
                image,
                Truncate(label, 32),
                new CvPoint(x, y),
                HersheyFonts.HersheySimplex,
                0.35,
                color,
                1,
                LineTypes.AntiAlias);
        }

        private static Rect ToImageRect(RectangleF bounds, int imageWidth, int imageHeight)
        {
            int x1 = ClampToInt(bounds.Left, 0, imageWidth);
            int y1 = ClampToInt(bounds.Top, 0, imageHeight);
            int x2 = ClampToInt(bounds.Right, 0, imageWidth);
            int y2 = ClampToInt(bounds.Bottom, 0, imageHeight);
            return new Rect(x1, y1, Math.Max(0, x2 - x1), Math.Max(0, y2 - y1));
        }

        private static CvPoint ToImagePoint(PointF point, int imageWidth, int imageHeight)
        {
            return new CvPoint(
                ClampToInt(point.X, 0, Math.Max(0, imageWidth - 1)),
                ClampToInt(point.Y, 0, Math.Max(0, imageHeight - 1)));
        }

        private static int ClampToInt(float value, int min, int max)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                return min;
            }

            return Math.Max(min, Math.Min((int)Math.Round(value), max));
        }

        private static Scalar ResolveSourceColor(int sourceIndex)
        {
            switch (sourceIndex % 5)
            {
                case 1:
                    return new Scalar(255, 190, 0);
                case 2:
                    return new Scalar(0, 190, 255);
                case 3:
                    return new Scalar(255, 80, 180);
                case 4:
                    return new Scalar(70, 220, 255);
                default:
                    return new Scalar(0, 220, 90);
            }
        }

        private static VisionToolOverlay CloneOverlay(VisionToolOverlay source)
        {
            VisionToolOverlay clone = new VisionToolOverlay
            {
                Kind = source.Kind,
                Label = source.Label ?? string.Empty,
                Bounds = source.Bounds,
                Center = source.Center,
                Start = source.Start,
                End = source.End,
                Angle = source.Angle
            };
            clone.Points.AddRange(source.Points ?? Enumerable.Empty<PointF>());
            return clone;
        }

        private static string BuildSourceDescription(MergeOptions options)
        {
            List<string> parts = new List<string>();
            if (options.SourceLayers.Count > 0)
            {
                parts.Add("SourceLayers=" + string.Join(",", options.SourceLayers));
            }

            if (options.SourceSteps.Count > 0)
            {
                parts.Add("SourceSteps=" + string.Join(",", options.SourceSteps));
            }

            return parts.Count == 0
                ? "No SourceLayers/SourceSteps filter was set."
                : string.Join(" | ", parts);
        }

        private static IEnumerable<string> SplitList(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Enumerable.Empty<string>();
            }

            return value
                .Split(new[] { ';', ',', '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item));
        }

        private static string GetValue(IDictionary<string, string> parameters, string key)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            foreach (KeyValuePair<string, string> item in parameters)
            {
                if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return item.Value;
                }
            }

            return null;
        }

        private static bool GetBool(IDictionary<string, string> parameters, string key, bool defaultValue)
        {
            string value = GetValue(parameters, key);
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        private static int GetInt(IDictionary<string, string> parameters, string key, int defaultValue)
        {
            string value = GetValue(parameters, key);
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
                ? result
                : defaultValue;
        }

        private static string Truncate(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text) || maxLength <= 0)
            {
                return string.Empty;
            }

            return text.Length <= maxLength
                ? text
                : text.Substring(0, maxLength - 3) + "...";
        }
    }
}
