using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal static class VisionPipelineMetricEnrichmentService
    {
        private const string PixelPerMmParameterName = "PIXELPERMM";

        public static void Enrich(VisionToolResult result, VisionPipelineStep step = null)
        {
            if (result == null)
            {
                return;
            }

            double pixelPerMm = ResolvePixelPerMm(step);
            AddRectangleOverlayMetrics(result.Metrics, result.Overlays, pixelPerMm);
            AddLineOverlayMetrics(result.Metrics, result.Overlays, pixelPerMm);
            AddExplicitMillimeterMetrics(result.Metrics, pixelPerMm);
            AddMatchingCandidateMetrics(result.Metrics, step);
        }

        public static Dictionary<string, double> CreateEnrichedMetrics(
            IDictionary<string, double> sourceMetrics,
            IEnumerable<VisionToolOverlay> overlays,
            VisionPipelineStep step = null)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>(
                sourceMetrics ?? new Dictionary<string, double>(),
                System.StringComparer.OrdinalIgnoreCase);

            double pixelPerMm = ResolvePixelPerMm(step);
            AddRectangleOverlayMetrics(metrics, overlays, pixelPerMm);
            AddLineOverlayMetrics(metrics, overlays, pixelPerMm);
            AddExplicitMillimeterMetrics(metrics, pixelPerMm);
            AddMatchingCandidateMetrics(metrics, step);

            return VisionPipelineKnownMetrics.OrderMetrics(metrics)
                .ToDictionary(metric => metric.Key, metric => metric.Value, System.StringComparer.OrdinalIgnoreCase);
        }

        private static void AddRectangleOverlayMetrics(
            IDictionary<string, double> metrics,
            IEnumerable<VisionToolOverlay> overlays,
            double pixelPerMm)
        {
            if (metrics == null)
            {
                return;
            }

            List<VisionToolOverlay> rectangles = (overlays ?? Enumerable.Empty<VisionToolOverlay>())
                .Where(overlay => overlay != null && overlay.Kind == VisionToolOverlayKind.Rectangle)
                .Where(overlay => overlay.Bounds.Width > 0 && overlay.Bounds.Height > 0)
                .ToList();

            if (rectangles.Count == 0)
            {
                return;
            }

            metrics[VisionPipelineKnownMetrics.BoundsWidthMin] = rectangles.Min(overlay => overlay.Bounds.Width);
            metrics[VisionPipelineKnownMetrics.BoundsWidthMax] = rectangles.Max(overlay => overlay.Bounds.Width);
            metrics[VisionPipelineKnownMetrics.BoundsWidthAvg] = rectangles.Average(overlay => overlay.Bounds.Width);
            metrics[VisionPipelineKnownMetrics.BoundsHeightMin] = rectangles.Min(overlay => overlay.Bounds.Height);
            metrics[VisionPipelineKnownMetrics.BoundsHeightMax] = rectangles.Max(overlay => overlay.Bounds.Height);
            metrics[VisionPipelineKnownMetrics.BoundsHeightAvg] = rectangles.Average(overlay => overlay.Bounds.Height);

            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.BoundsWidthMin, VisionPipelineKnownMetrics.BoundsWidthMmMin, pixelPerMm);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.BoundsWidthMax, VisionPipelineKnownMetrics.BoundsWidthMmMax, pixelPerMm);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.BoundsWidthAvg, VisionPipelineKnownMetrics.BoundsWidthMmAvg, pixelPerMm);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.BoundsHeightMin, VisionPipelineKnownMetrics.BoundsHeightMmMin, pixelPerMm);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.BoundsHeightMax, VisionPipelineKnownMetrics.BoundsHeightMmMax, pixelPerMm);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.BoundsHeightAvg, VisionPipelineKnownMetrics.BoundsHeightMmAvg, pixelPerMm);
        }

        private static void AddLineOverlayMetrics(
            IDictionary<string, double> metrics,
            IEnumerable<VisionToolOverlay> overlays,
            double pixelPerMm)
        {
            if (metrics == null)
            {
                return;
            }

            List<(double Length, double Angle)> lines = (overlays ?? Enumerable.Empty<VisionToolOverlay>())
                .Where(overlay => overlay != null && overlay.Kind == VisionToolOverlayKind.Line)
                .Select(overlay => CreateLineMetric(overlay.Start, overlay.End))
                .Where(metric => metric.Length > 0)
                .ToList();

            if (lines.Count == 0)
            {
                return;
            }

            metrics[VisionPipelineKnownMetrics.LineLengthMin] = lines.Min(line => line.Length);
            metrics[VisionPipelineKnownMetrics.LineLengthMax] = lines.Max(line => line.Length);
            metrics[VisionPipelineKnownMetrics.LineLengthAvg] = lines.Average(line => line.Length);
            metrics[VisionPipelineKnownMetrics.LineAngleMin] = lines.Min(line => line.Angle);
            metrics[VisionPipelineKnownMetrics.LineAngleMax] = lines.Max(line => line.Angle);
            metrics[VisionPipelineKnownMetrics.LineAngleAvg] = lines.Average(line => line.Angle);

            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.LineLengthMin, VisionPipelineKnownMetrics.LineLengthMmMin, pixelPerMm);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.LineLengthMax, VisionPipelineKnownMetrics.LineLengthMmMax, pixelPerMm);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.LineLengthAvg, VisionPipelineKnownMetrics.LineLengthMmAvg, pixelPerMm);
        }

        private static (double Length, double Angle) CreateLineMetric(PointF start, PointF end)
        {
            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            double angle = Math.Atan2(dy, dx) * 180d / Math.PI;
            return (length, angle);
        }

        private static void AddExplicitMillimeterMetrics(
            IDictionary<string, double> metrics,
            double millimetersPerPixel)
        {
            if (metrics == null || !IsFinite(millimetersPerPixel) || millimetersPerPixel <= 0D)
            {
                return;
            }

            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.GeometryDistancePx, VisionPipelineKnownMetrics.GeometryDistanceMm, millimetersPerPixel);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.GeometrySignedClearancePx, VisionPipelineKnownMetrics.GeometrySignedClearanceMm, millimetersPerPixel);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.CircleRadiusPx, VisionPipelineKnownMetrics.CircleRadiusMm, millimetersPerPixel);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.CircleDiameterPx, VisionPipelineKnownMetrics.CircleDiameterMm, millimetersPerPixel);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.DistancePxMin, VisionPipelineKnownMetrics.DistanceMmMin, millimetersPerPixel);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.DistancePxMax, VisionPipelineKnownMetrics.DistanceMmMax, millimetersPerPixel);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.DistancePxAvg, VisionPipelineKnownMetrics.DistanceMmAvg, millimetersPerPixel);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.DistancePxRange, VisionPipelineKnownMetrics.DistanceMmRange, millimetersPerPixel);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.CurveOuterArcLengthPx, VisionPipelineKnownMetrics.CurveOuterArcLengthMm, millimetersPerPixel);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.CurveInnerArcLengthPx, VisionPipelineKnownMetrics.CurveInnerArcLengthMm, millimetersPerPixel);
            AddConvertedMetric(metrics, VisionPipelineKnownMetrics.CurveCenterArcLengthPx, VisionPipelineKnownMetrics.CurveCenterArcLengthMm, millimetersPerPixel);
        }

        internal static void AddConvertedMetric(
            IDictionary<string, double> metrics,
            string pixelMetric,
            string millimeterMetric,
            double millimetersPerPixel)
        {
            if (metrics != null
                && metrics.TryGetValue(pixelMetric, out double value)
                && TryConvertPixelToMillimeters(value, millimetersPerPixel, out double converted))
            {
                metrics[millimeterMetric] = converted;
            }
        }

        internal static bool TryConvertPixelToMillimeters(
            double pixelValue,
            double millimetersPerPixel,
            out double millimeters)
        {
            millimeters = 0D;
            if (!IsFinite(pixelValue)
                || !IsFinite(millimetersPerPixel)
                || millimetersPerPixel <= 0D)
            {
                return false;
            }

            double converted = pixelValue * millimetersPerPixel;
            if (!IsFinite(converted))
            {
                return false;
            }

            millimeters = converted;
            return true;
        }

        private static void AddMatchingCandidateMetrics(
            IDictionary<string, double> metrics,
            VisionPipelineStep step)
        {
            if (metrics == null || step?.Parameters == null)
            {
                return;
            }

            string toolType = VisionPipelineNormalizer.NormalizeToolType(step.ToolType);
            if ((toolType != "matching" && toolType != "templatematching")
                || !step.Parameters.TryGetValue("NUM_MATCH", out string countText)
                || !int.TryParse(countText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int requestedCount)
                || requestedCount != 2
                || !metrics.TryGetValue(VisionPipelineKnownMetrics.ScoreMax, out double bestScore))
            {
                return;
            }

            double secondScore = metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out double resultCount)
                && resultCount >= 2d
                && metrics.TryGetValue(VisionPipelineKnownMetrics.ScoreMin, out double scoreMin)
                    ? scoreMin
                    : 0d;
            metrics[VisionPipelineKnownMetrics.ScoreMargin] = Math.Max(0d, bestScore - secondScore);
        }

        private static double ResolvePixelPerMm(VisionPipelineStep step)
        {
            if (step?.Parameters == null
                || !step.Parameters.TryGetValue(PixelPerMmParameterName, out string value)
                || string.IsNullOrWhiteSpace(value))
            {
                return 0d;
            }

            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed)
                && IsFinite(parsed)
                && parsed > 0
                    ? parsed
                    : 0d;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
