using Lib.Line;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OpenVisionLab
{
    public enum VisionPipelineGeometryKind
    {
        Point,
        Segment,
        Circle
    }

    public sealed class VisionPipelineGeometryFeatureResult
    {
        public string SourceStep { get; set; } = string.Empty;
        public string FeatureName { get; set; } = string.Empty;
        public VisionPipelineGeometryKind Kind { get; set; }
        public string CoordinateLayer { get; set; } = string.Empty;
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double RadiusPx { get; set; }
        public int SupportCount { get; set; }
        public double SupportRatio { get; set; }
        public double CoverageDeg { get; set; }
        public double FitResidualPx { get; set; }

        public string Identity => $"{SourceStep}/{FeatureName}";
        public string GeometryText => Kind == VisionPipelineGeometryKind.Point
            ? $"({CenterX:0.###}, {CenterY:0.###})"
            : Kind == VisionPipelineGeometryKind.Segment
                ? $"({X1:0.###}, {Y1:0.###}) -> ({X2:0.###}, {Y2:0.###})"
                : $"C({CenterX:0.###}, {CenterY:0.###}) R={RadiusPx:0.###}";
        public string QualityText => $"support {SupportCount} / {SupportRatio:0.###}, residual {FitResidualPx:0.###}px";

        public VisionPipelineGeometryFeatureResult Clone()
        {
            return (VisionPipelineGeometryFeatureResult)MemberwiseClone();
        }
    }

    internal static class VisionPipelineGeometryFeatureCatalog
    {
        public static IEnumerable<(string FeatureName, VisionPipelineGeometryKind Kind)> GetDeclaredFeatures(
            VisionPipelineStep step)
        {
            string type = VisionPipelineNormalizer.NormalizeToolType(step?.ToolType);
            if (type == "line" || type == "linegauge")
            {
                yield return ("Segment", VisionPipelineGeometryKind.Segment);
                yield return ("Start", VisionPipelineGeometryKind.Point);
                yield return ("End", VisionPipelineGeometryKind.Point);
                yield return ("Midpoint", VisionPipelineGeometryKind.Point);
            }
            else if (type == "matching"
                || type == "templatematching"
                || type == "edgebasedmatching"
                || type == "edgebasedtemplatematching"
                || type == "edgetemplatematching")
            {
                yield return ("Center", VisionPipelineGeometryKind.Point);
            }
            else if (type == "circlegauge")
            {
                yield return ("Circle", VisionPipelineGeometryKind.Circle);
                yield return ("Center", VisionPipelineGeometryKind.Point);
            }
            else if ((type == "geometrymeasure" || type == "geometricmeasurement")
                && Enum.TryParse(
                    GetString(step?.Parameters, VisionPipelineGeometryMeasureService.ModeParameter),
                    true,
                    out GeometryMeasurementMode mode))
            {
                if (mode == GeometryMeasurementMode.LineLineIntersection)
                {
                    yield return ("Intersection", VisionPipelineGeometryKind.Point);
                }
                else if (mode != GeometryMeasurementMode.LineLineAngle)
                {
                    yield return ("MeasureStart", VisionPipelineGeometryKind.Point);
                    yield return ("MeasureEnd", VisionPipelineGeometryKind.Point);
                }
            }
        }

        private static string GetString(IDictionary<string, string> parameters, string key)
        {
            return parameters != null && parameters.TryGetValue(key, out string value)
                ? value?.Trim() ?? string.Empty
                : string.Empty;
        }
    }

    internal static class VisionPipelineGeometryFeatureStore
    {
        private sealed class Holder
        {
            public IReadOnlyList<VisionPipelineGeometryFeatureResult> Items { get; set; }
                = Array.Empty<VisionPipelineGeometryFeatureResult>();
        }

        private static readonly ConditionalWeakTable<VisionToolResult, Holder> results
            = new ConditionalWeakTable<VisionToolResult, Holder>();

        public static void Set(VisionToolResult result, IEnumerable<VisionPipelineGeometryFeatureResult> items)
        {
            if (result == null)
            {
                return;
            }

            IReadOnlyList<VisionPipelineGeometryFeatureResult> stable = (items
                    ?? Enumerable.Empty<VisionPipelineGeometryFeatureResult>())
                .Where(item => item != null)
                .Select(item => item.Clone())
                .OrderBy(item => item.SourceStep, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.FeatureName, StringComparer.OrdinalIgnoreCase)
                .ToList();
            results.Remove(result);
            results.Add(result, new Holder { Items = stable });
        }

        public static IReadOnlyList<VisionPipelineGeometryFeatureResult> Get(VisionToolResult result)
        {
            return result != null && results.TryGetValue(result, out Holder holder)
                ? holder.Items
                : Array.Empty<VisionPipelineGeometryFeatureResult>();
        }

        public static bool TryGet(
            VisionToolResult result,
            string sourceStep,
            string featureName,
            out VisionPipelineGeometryFeatureResult feature)
        {
            feature = Get(result).SingleOrDefault(item =>
                string.Equals(item.SourceStep, sourceStep, StringComparison.OrdinalIgnoreCase)
                && string.Equals(item.FeatureName, featureName, StringComparison.OrdinalIgnoreCase));
            return feature != null;
        }
    }

    internal static class VisionPipelineGeometryFeatureCaptureService
    {
        public static void Capture(
            VisionPipelineStep step,
            Mat input,
            IVisionTool executedTool,
            VisionToolResult toolResult)
        {
            if (step == null || input == null || input.Empty() || toolResult?.Success != true)
            {
                return;
            }

            if (executedTool is MatchingTool matchingTool)
            {
                CaptureMatchingCenter(step, input, matchingTool.results, toolResult);
                return;
            }

            if (executedTool is EdgeBasedTemplateMatchingTool edgeBasedMatchingTool)
            {
                CaptureMatchingCenter(step, input, edgeBasedMatchingTool.results, toolResult);
                return;
            }

            if (!(executedTool is LineGaugeTool lineTool))
            {
                return;
            }

            List<LineGaugeResult> usable = (lineTool.resultList ?? new List<LineGaugeResult>())
                .Where(item => IsUsable(item?.FitLine))
                .ToList();
            if (usable.Count != 1)
            {
                return;
            }

            LineGaugeResult lineResult = usable[0];
            LineSegment2D line = lineResult.FitLine;
            double x1 = line.Start.X;
            double y1 = line.Start.Y;
            double x2 = line.End.X;
            double y2 = line.End.Y;
            double centerX = (x1 + x2) / 2D;
            double centerY = (y1 + y2) / 2D;
            int supportCount = lineResult.edgeList?.Count ?? 0;
            double residual = CalculateResidual(lineResult.edgeList, x1, y1, x2, y2);
            string sourceStep = step.Name ?? string.Empty;
            string coordinateLayer = step.InputLayer ?? string.Empty;

            VisionPipelineGeometryFeatureResult Create(string name, VisionPipelineGeometryKind kind)
            {
                return new VisionPipelineGeometryFeatureResult
                {
                    SourceStep = sourceStep,
                    FeatureName = name,
                    Kind = kind,
                    CoordinateLayer = coordinateLayer,
                    ImageWidth = input.Width,
                    ImageHeight = input.Height,
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    CenterX = centerX,
                    CenterY = centerY,
                    SupportCount = supportCount,
                    SupportRatio = 1D,
                    FitResidualPx = residual
                };
            }

            VisionPipelineGeometryFeatureResult segment = Create("Segment", VisionPipelineGeometryKind.Segment);
            VisionPipelineGeometryFeatureResult start = Create("Start", VisionPipelineGeometryKind.Point);
            start.X1 = start.CenterX = x1;
            start.Y1 = start.CenterY = y1;
            VisionPipelineGeometryFeatureResult end = Create("End", VisionPipelineGeometryKind.Point);
            end.X1 = end.CenterX = x2;
            end.Y1 = end.CenterY = y2;
            VisionPipelineGeometryFeatureResult midpoint = Create("Midpoint", VisionPipelineGeometryKind.Point);
            midpoint.X1 = midpoint.CenterX = centerX;
            midpoint.Y1 = midpoint.CenterY = centerY;

            VisionPipelineGeometryFeatureStore.Set(toolResult, new[] { segment, start, end, midpoint });
        }

        private static void CaptureMatchingCenter(
            VisionPipelineStep step,
            Mat input,
            IEnumerable<MatchingResult> matchingResults,
            VisionToolResult toolResult)
        {
            List<MatchingResult> usable = (matchingResults ?? Enumerable.Empty<MatchingResult>())
                .Where(item => item != null
                    && IsFinite(item.Center.X)
                    && IsFinite(item.Center.Y)
                    && item.Center.X >= 0
                    && item.Center.Y >= 0
                    && item.Center.X < input.Width
                    && item.Center.Y < input.Height)
                .ToList();
            if (usable.Count != 1)
            {
                return;
            }

            MatchingResult match = usable[0];
            double normalizedScore = Math.Max(0D, Math.Min(1D, match.Score / 100D));
            VisionPipelineGeometryFeatureStore.Set(toolResult, new[]
            {
                new VisionPipelineGeometryFeatureResult
                {
                    SourceStep = step.Name ?? string.Empty,
                    FeatureName = "Center",
                    Kind = VisionPipelineGeometryKind.Point,
                    CoordinateLayer = step.InputLayer ?? string.Empty,
                    ImageWidth = input.Width,
                    ImageHeight = input.Height,
                    X1 = match.Center.X,
                    Y1 = match.Center.Y,
                    CenterX = match.Center.X,
                    CenterY = match.Center.Y,
                    SupportCount = 1,
                    SupportRatio = normalizedScore,
                    FitResidualPx = 0D
                }
            });
        }

        private static bool IsUsable(LineSegment2D line)
        {
            if (line == null)
            {
                return false;
            }

            double length = line.Distance();
            return IsFinite(line.Start.X)
                && IsFinite(line.Start.Y)
                && IsFinite(line.End.X)
                && IsFinite(line.End.Y)
                && IsFinite(length)
                && length > 0D;
        }

        private static double CalculateResidual(
            IEnumerable<OpenCvSharp.Point> points,
            double x1,
            double y1,
            double x2,
            double y2)
        {
            List<OpenCvSharp.Point> values = (points ?? Enumerable.Empty<OpenCvSharp.Point>()).ToList();
            double dx = x2 - x1;
            double dy = y2 - y1;
            double denominator = Math.Sqrt(dx * dx + dy * dy);
            if (values.Count == 0 || denominator <= 0D)
            {
                return 0D;
            }

            double squared = values.Sum(point =>
            {
                double distance = Math.Abs(dy * point.X - dx * point.Y + x2 * y1 - y2 * x1) / denominator;
                return distance * distance;
            });
            return Math.Sqrt(squared / values.Count);
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
