using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    public enum GeometryMeasurementMode
    {
        PointPointDistance,
        PointLineDistance,
        SegmentSegmentDistance,
        LineLineDistance,
        LineLineAngle,
        LineLineIntersection,
        CircleSegmentClearance
    }

    internal static class VisionPipelineGeometryMeasureService
    {
        public const string ModeParameter = "MeasurementMode";
        public const string SourceStepAParameter = "SourceStepA";
        public const string SourceFeatureAParameter = "SourceFeatureA";
        public const string SourceStepBParameter = "SourceStepB";
        public const string SourceFeatureBParameter = "SourceFeatureB";
        public const string MaximumParallelAngleDeltaParameter = "MAX_PARALLEL_ANGLE_DELTA_DEG";
        public const string MaximumExtensionAParameter = "MAX_EXTENSION_A_PX";
        public const string MaximumExtensionBParameter = "MAX_EXTENSION_B_PX";
        public const string RequireResultInImageParameter = "REQUIRE_RESULT_IN_IMAGE";

        public static bool IsGeometryMeasure(string toolType)
        {
            string normalized = Normalize(toolType);
            return normalized == "geometrymeasure" || normalized == "geometricmeasurement";
        }

        public static VisionToolResult Execute(
            VisionPipelineStep step,
            Mat input,
            VisionPipelineRunResult runResult)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (step == null || input == null || input.Empty())
            {
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InputImageInvalid,
                    "GeometryMeasure input image is empty.",
                    stopwatch.Elapsed);
            }

            Mat resultImage = CreateColor(input);
            Dictionary<string, double> metrics = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.ResultCount] = 0D
            };
            List<VisionToolOverlay> overlays = new List<VisionToolOverlay>();

            if (!TryGetEnum(step.Parameters, ModeParameter, out GeometryMeasurementMode mode))
            {
                return Fail(resultImage, stopwatch, $"GeometryMeasure '{ModeParameter}' is missing or invalid.", metrics, overlays);
            }

            bool sourceAResolved = TryResolve(step, input, runResult, SourceStepAParameter, SourceFeatureAParameter, out VisionPipelineGeometryFeatureResult a, out string sourceAError);
            bool sourceBResolved = TryResolve(step, input, runResult, SourceStepBParameter, SourceFeatureBParameter, out VisionPipelineGeometryFeatureResult b, out string sourceBError);
            if (!sourceAResolved || !sourceBResolved)
            {
                string reason = string.IsNullOrWhiteSpace(sourceAError) ? sourceBError : sourceAError;
                return Fail(resultImage, stopwatch, reason, metrics, overlays);
            }

            DrawFeature(resultImage, a, new Scalar(0, 255, 255), "A");
            DrawFeature(resultImage, b, new Scalar(255, 255, 0), "B");
            AddFeatureOverlay(overlays, a, "Source A");
            AddFeatureOverlay(overlays, b, "Source B");

            if (!TryMeasure(step, input, mode, a, b, out Measurement measurement, out string error))
            {
                return Fail(resultImage, stopwatch, error, metrics, overlays);
            }

            if (measurement.HasConstruction)
            {
                Cv2.Line(resultImage, ToPoint(measurement.Start), ToPoint(measurement.End), new Scalar(255, 0, 255), 2, LineTypes.AntiAlias);
                Cv2.DrawMarker(resultImage, ToPoint(measurement.Start), new Scalar(0, 255, 0), MarkerTypes.Cross, 15, 2);
                Cv2.DrawMarker(resultImage, ToPoint(measurement.End), new Scalar(0, 255, 0), MarkerTypes.Cross, 15, 2);
                overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Line,
                    Label = "Geometry construction",
                    Start = ToPointF(measurement.Start),
                    End = ToPointF(measurement.End),
                    Center = ToPointF(new Point2d(
                        (measurement.Start.X + measurement.End.X) / 2D,
                        (measurement.Start.Y + measurement.End.Y) / 2D))
                });
            }

            if (measurement.Intersection.HasValue)
            {
                Cv2.DrawMarker(resultImage, ToPoint(measurement.Intersection.Value), new Scalar(0, 255, 0), MarkerTypes.Cross, 21, 3);
                overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Point,
                    Label = "Intersection",
                    Center = ToPointF(measurement.Intersection.Value)
                });
            }

            foreach (KeyValuePair<string, double> metric in measurement.Metrics)
            {
                metrics[metric.Key] = metric.Value;
            }
            metrics[VisionPipelineKnownMetrics.ResultCount] = 1D;

            string primary = measurement.PrimaryMetricName;
            double primaryValue = metrics.TryGetValue(primary, out double value) ? value : 0D;
            string unit = primary.IndexOf("Angle", StringComparison.OrdinalIgnoreCase) >= 0 ? "deg" : "px";
            string text;
            if (mode == GeometryMeasurementMode.LineLineIntersection)
            {
                text = $"PASS Intersection X={metrics[VisionPipelineKnownMetrics.IntersectionX]:0.###} Y={metrics[VisionPipelineKnownMetrics.IntersectionY]:0.###} extA={metrics[VisionPipelineKnownMetrics.GeometryExtensionAPx]:0.###} extB={metrics[VisionPipelineKnownMetrics.GeometryExtensionBPx]:0.###}px";
            }
            else
            {
                text = $"PASS {mode}: {primaryValue.ToString("0.###", CultureInfo.InvariantCulture)} {unit}";
            }
            DrawStatus(resultImage, text, true);

            VisionToolResult result = VisionToolResult.Passed(resultImage, stopwatch.Elapsed, metrics, overlays);
            result.Message = text;
            VisionPipelineGeometryFeatureStore.Set(result, CreateOutputs(step, input, measurement));
            return result;
        }

        private static bool TryResolve(
            VisionPipelineStep consumer,
            Mat input,
            VisionPipelineRunResult runResult,
            string stepKey,
            string featureKey,
            out VisionPipelineGeometryFeatureResult feature,
            out string error)
        {
            feature = null;
            string sourceStep = GetString(consumer.Parameters, stepKey);
            string featureName = GetString(consumer.Parameters, featureKey);
            if (string.IsNullOrWhiteSpace(sourceStep) || string.IsNullOrWhiteSpace(featureName))
            {
                error = $"GeometryMeasure requires {stepKey} and {featureKey}.";
                return false;
            }

            List<VisionPipelineStepResult> matches = (runResult?.StepResults ?? new List<VisionPipelineStepResult>())
                .Where(item => item?.Step?.Enabled == true
                    && string.Equals(item.Step.Name, sourceStep, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (matches.Count != 1)
            {
                error = matches.Count == 0
                    ? $"Geometry source Step '{sourceStep}' is not an earlier enabled Step in this run."
                    : $"Geometry source Step '{sourceStep}' is ambiguous ({matches.Count} earlier Steps).";
                return false;
            }

            VisionPipelineStepResult source = matches[0];
            if (source.ToolResult?.Success != true || !source.AcceptancePassed)
            {
                error = $"Geometry source Step '{sourceStep}' did not pass its execution and acceptance gates.";
                return false;
            }

            List<VisionPipelineGeometryFeatureResult> featureMatches = VisionPipelineGeometryFeatureStore.Get(source.ToolResult)
                .Where(item => string.Equals(item.FeatureName, featureName, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (featureMatches.Count != 1)
            {
                error = featureMatches.Count == 0
                    ? $"Geometry feature '{sourceStep}/{featureName}' was not produced in this run."
                    : $"Geometry feature '{sourceStep}/{featureName}' is ambiguous.";
                return false;
            }

            feature = featureMatches[0];
            if (!string.Equals(feature.CoordinateLayer, consumer.InputLayer, StringComparison.OrdinalIgnoreCase)
                || feature.ImageWidth != input.Width
                || feature.ImageHeight != input.Height)
            {
                error = $"Geometry feature '{sourceStep}/{featureName}' uses coordinate frame '{feature.CoordinateLayer}' {feature.ImageWidth}x{feature.ImageHeight}, but GeometryMeasure input is '{consumer.InputLayer}' {input.Width}x{input.Height}.";
                feature = null;
                return false;
            }

            if (!TryValidateFeatureGeometry(feature, input.Width, input.Height, out string geometryError))
            {
                error = $"Geometry feature '{sourceStep}/{featureName}' is invalid: {geometryError}";
                feature = null;
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool TryValidateFeatureGeometry(
            VisionPipelineGeometryFeatureResult feature,
            int imageWidth,
            int imageHeight,
            out string error)
        {
            bool IsInside(double x, double y) =>
                IsFinite(x) && IsFinite(y)
                && x >= 0D && y >= 0D
                && x < imageWidth && y < imageHeight;

            if (feature == null)
            {
                error = "the feature row is missing.";
                return false;
            }

            switch (feature.Kind)
            {
                case VisionPipelineGeometryKind.Point:
                    if (!IsInside(feature.CenterX, feature.CenterY))
                    {
                        error = "point coordinates must be finite and inside the recorded image.";
                        return false;
                    }
                    break;
                case VisionPipelineGeometryKind.Segment:
                    if (!IsInside(feature.X1, feature.Y1) || !IsInside(feature.X2, feature.Y2))
                    {
                        error = "segment endpoints must be finite and inside the recorded image.";
                        return false;
                    }
                    if (Distance(new Point2d(feature.X1, feature.Y1), new Point2d(feature.X2, feature.Y2)) <= 1e-9)
                    {
                        error = "segment length must be greater than zero.";
                        return false;
                    }
                    break;
                case VisionPipelineGeometryKind.Circle:
                    if (!IsInside(feature.CenterX, feature.CenterY)
                        || !IsFinite(feature.RadiusPx)
                        || feature.RadiusPx <= 0D)
                    {
                        error = "circle center must be finite and inside the recorded image, and radius must be positive.";
                        return false;
                    }
                    break;
                default:
                    error = $"unsupported feature kind '{feature.Kind}'.";
                    return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool TryMeasure(
            VisionPipelineStep step,
            Mat input,
            GeometryMeasurementMode mode,
            VisionPipelineGeometryFeatureResult a,
            VisionPipelineGeometryFeatureResult b,
            out Measurement measurement,
            out string error)
        {
            measurement = new Measurement();
            error = string.Empty;
            Point2d aPoint = PointOf(a);
            Point2d bPoint = PointOf(b);
            Point2d aStart = new Point2d(a.X1, a.Y1);
            Point2d aEnd = new Point2d(a.X2, a.Y2);
            Point2d bStart = new Point2d(b.X1, b.Y1);
            Point2d bEnd = new Point2d(b.X2, b.Y2);

            switch (mode)
            {
                case GeometryMeasurementMode.PointPointDistance:
                    if (!RequireKinds(a, b, VisionPipelineGeometryKind.Point, VisionPipelineGeometryKind.Point, out error)) return false;
                    measurement.SetDistance(aPoint, bPoint, Distance(aPoint, bPoint));
                    break;
                case GeometryMeasurementMode.PointLineDistance:
                    if (!RequireKinds(a, b, VisionPipelineGeometryKind.Point, VisionPipelineGeometryKind.Segment, out error)) return false;
                    Point2d pointProjection = ProjectInfinite(aPoint, bStart, bEnd);
                    measurement.SetDistance(aPoint, pointProjection, Distance(aPoint, pointProjection));
                    break;
                case GeometryMeasurementMode.SegmentSegmentDistance:
                    if (!RequireKinds(a, b, VisionPipelineGeometryKind.Segment, VisionPipelineGeometryKind.Segment, out error)) return false;
                    ClosestSegmentPoints(aStart, aEnd, bStart, bEnd, out Point2d closestA, out Point2d closestB);
                    measurement.SetDistance(closestA, closestB, Distance(closestA, closestB));
                    break;
                case GeometryMeasurementMode.LineLineDistance:
                    if (!RequireKinds(a, b, VisionPipelineGeometryKind.Segment, VisionPipelineGeometryKind.Segment, out error)) return false;
                    double parallelDelta = UndirectedAngleDelta(aStart, aEnd, bStart, bEnd);
                    double maximumDelta = GetDouble(step.Parameters, MaximumParallelAngleDeltaParameter, 2D);
                    if (parallelDelta > maximumDelta)
                    {
                        error = $"LineLineDistance requires near-parallel segments. Angle delta {parallelDelta:0.###} deg exceeds {maximumDelta:0.###} deg.";
                        return false;
                    }
                    Point2d aMid = Midpoint(aStart, aEnd);
                    Point2d lineProjection = ProjectInfinite(aMid, bStart, bEnd);
                    measurement.SetDistance(aMid, lineProjection, Distance(aMid, lineProjection));
                    measurement.Metrics[VisionPipelineKnownMetrics.GeometryParallelDeltaDeg] = parallelDelta;
                    break;
                case GeometryMeasurementMode.LineLineAngle:
                    if (!RequireKinds(a, b, VisionPipelineGeometryKind.Segment, VisionPipelineGeometryKind.Segment, out error)) return false;
                    measurement.PrimaryMetricName = VisionPipelineKnownMetrics.GeometryAngleDeg;
                    measurement.Metrics[measurement.PrimaryMetricName] = UndirectedAngleDelta(aStart, aEnd, bStart, bEnd);
                    measurement.HasConstruction = false;
                    break;
                case GeometryMeasurementMode.LineLineIntersection:
                    if (!RequireKinds(a, b, VisionPipelineGeometryKind.Segment, VisionPipelineGeometryKind.Segment, out error)) return false;
                    if (!TryIntersectInfinite(aStart, aEnd, bStart, bEnd, out Point2d intersection))
                    {
                        error = "LineLineIntersection requires non-parallel source segments.";
                        return false;
                    }
                    double extensionA = ExtensionDistance(intersection, aStart, aEnd);
                    double extensionB = ExtensionDistance(intersection, bStart, bEnd);
                    double maximumExtensionA = GetDouble(step.Parameters, MaximumExtensionAParameter, 100D);
                    double maximumExtensionB = GetDouble(step.Parameters, MaximumExtensionBParameter, 100D);
                    if (extensionA > maximumExtensionA || extensionB > maximumExtensionB)
                    {
                        error = $"LineLineIntersection extension is out of bounds. A={extensionA:0.###}/{maximumExtensionA:0.###}px, B={extensionB:0.###}/{maximumExtensionB:0.###}px.";
                        return false;
                    }
                    if (GetBool(step.Parameters, RequireResultInImageParameter, true)
                        && (intersection.X < 0D || intersection.Y < 0D || intersection.X >= input.Width || intersection.Y >= input.Height))
                    {
                        error = $"LineLineIntersection ({intersection.X:0.###}, {intersection.Y:0.###}) is outside the current image.";
                        return false;
                    }
                    if (GetBool(step.Parameters, "USE_ROI", false))
                    {
                        if (!TryGetRect(step.Parameters, "CvROI", out Rect resultRoi))
                        {
                            error = "LineLineIntersection requires a valid CvROI when USE_ROI=true.";
                            return false;
                        }
                        if (!resultRoi.Contains(ToPoint(intersection)))
                        {
                            error = $"LineLineIntersection ({intersection.X:0.###}, {intersection.Y:0.###}) is outside the reviewed result ROI.";
                            return false;
                        }
                    }
                    measurement.Intersection = intersection;
                    measurement.PrimaryMetricName = VisionPipelineKnownMetrics.IntersectionX;
                    measurement.Metrics[VisionPipelineKnownMetrics.IntersectionX] = intersection.X;
                    measurement.Metrics[VisionPipelineKnownMetrics.IntersectionY] = intersection.Y;
                    measurement.Metrics[VisionPipelineKnownMetrics.GeometryExtensionAPx] = extensionA;
                    measurement.Metrics[VisionPipelineKnownMetrics.GeometryExtensionBPx] = extensionB;
                    measurement.HasConstruction = false;
                    break;
                case GeometryMeasurementMode.CircleSegmentClearance:
                    if (!RequireKinds(a, b, VisionPipelineGeometryKind.Circle, VisionPipelineGeometryKind.Segment, out error)) return false;
                    Point2d nearest = ProjectFinite(aPoint, bStart, bEnd);
                    double centerDistance = Distance(aPoint, nearest);
                    if (centerDistance <= 1e-9)
                    {
                        error = "CircleSegmentClearance is undefined because the circle center lies on the segment.";
                        return false;
                    }
                    Point2d radial = new Point2d(
                        aPoint.X + (nearest.X - aPoint.X) * a.RadiusPx / centerDistance,
                        aPoint.Y + (nearest.Y - aPoint.Y) * a.RadiusPx / centerDistance);
                    measurement.Start = radial;
                    measurement.End = nearest;
                    measurement.HasConstruction = true;
                    measurement.PrimaryMetricName = VisionPipelineKnownMetrics.GeometrySignedClearancePx;
                    measurement.Metrics[measurement.PrimaryMetricName] = centerDistance - a.RadiusPx;
                    break;
                default:
                    error = $"Unsupported GeometryMeasure mode '{mode}'.";
                    return false;
            }

            return measurement.Metrics.Values.All(IsFinite);
        }

        private static IEnumerable<VisionPipelineGeometryFeatureResult> CreateOutputs(
            VisionPipelineStep step,
            Mat input,
            Measurement measurement)
        {
            List<VisionPipelineGeometryFeatureResult> outputs = new List<VisionPipelineGeometryFeatureResult>();
            if (measurement.Intersection.HasValue)
            {
                Point2d point = measurement.Intersection.Value;
                outputs.Add(CreatePoint(step, input, "Intersection", point));
            }
            if (measurement.HasConstruction)
            {
                outputs.Add(CreatePoint(step, input, "MeasureStart", measurement.Start));
                outputs.Add(CreatePoint(step, input, "MeasureEnd", measurement.End));
            }
            return outputs;
        }

        private static VisionPipelineGeometryFeatureResult CreatePoint(VisionPipelineStep step, Mat input, string name, Point2d point)
        {
            return new VisionPipelineGeometryFeatureResult
            {
                SourceStep = step.Name ?? string.Empty,
                FeatureName = name,
                Kind = VisionPipelineGeometryKind.Point,
                CoordinateLayer = step.InputLayer ?? string.Empty,
                ImageWidth = input.Width,
                ImageHeight = input.Height,
                X1 = point.X,
                Y1 = point.Y,
                CenterX = point.X,
                CenterY = point.Y,
                SupportCount = 1,
                SupportRatio = 1D
            };
        }

        private static bool RequireKinds(
            VisionPipelineGeometryFeatureResult a,
            VisionPipelineGeometryFeatureResult b,
            VisionPipelineGeometryKind requiredA,
            VisionPipelineGeometryKind requiredB,
            out string error)
        {
            if (a.Kind == requiredA && b.Kind == requiredB)
            {
                error = string.Empty;
                return true;
            }
            error = $"GeometryMeasure kind mismatch. A requires {requiredA} but received {a.Kind}; B requires {requiredB} but received {b.Kind}.";
            return false;
        }

        private static VisionToolResult Fail(
            Mat image,
            Stopwatch stopwatch,
            string message,
            IDictionary<string, double> metrics,
            IEnumerable<VisionToolOverlay> overlays)
        {
            DrawStatus(image, "REJECT: " + message, false);
            stopwatch.Stop();
            VisionToolResult result = new VisionToolResult
            {
                Success = false,
                Message = message ?? string.Empty,
                ResultImage = image,
                Elapsed = stopwatch.Elapsed,
                ErrorCode = VisionToolErrorCode.InvalidParameter,
                ResultStatus = VisionToolResultStatus.InvalidParameter
            };
            foreach (KeyValuePair<string, double> metric in metrics ?? new Dictionary<string, double>()) result.Metrics[metric.Key] = metric.Value;
            result.Overlays.AddRange(overlays ?? Enumerable.Empty<VisionToolOverlay>());
            return result;
        }

        private static Mat CreateColor(Mat source)
        {
            if (source.Channels() == 1)
            {
                Mat color = new Mat();
                Cv2.CvtColor(source, color, ColorConversionCodes.GRAY2BGR);
                return color;
            }
            return source.Clone();
        }

        private static void DrawFeature(Mat image, VisionPipelineGeometryFeatureResult feature, Scalar color, string label)
        {
            switch (feature.Kind)
            {
                case VisionPipelineGeometryKind.Point:
                    Cv2.DrawMarker(image, ToPoint(PointOf(feature)), color, MarkerTypes.Cross, 15, 2);
                    break;
                case VisionPipelineGeometryKind.Segment:
                    Cv2.Line(image, ToPoint(new Point2d(feature.X1, feature.Y1)), ToPoint(new Point2d(feature.X2, feature.Y2)), color, 2, LineTypes.AntiAlias);
                    break;
                case VisionPipelineGeometryKind.Circle:
                    Cv2.Circle(image, ToPoint(PointOf(feature)), (int)Math.Round(feature.RadiusPx), color, 2, LineTypes.AntiAlias);
                    break;
            }
            Cv2.PutText(image, label, ToPoint(PointOf(feature)), HersheyFonts.HersheySimplex, 0.5, color, 1, LineTypes.AntiAlias);
        }

        private static void AddFeatureOverlay(List<VisionToolOverlay> overlays, VisionPipelineGeometryFeatureResult feature, string label)
        {
            if (feature.Kind == VisionPipelineGeometryKind.Segment)
            {
                overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Line,
                    Label = label,
                    Start = new PointF((float)feature.X1, (float)feature.Y1),
                    End = new PointF((float)feature.X2, (float)feature.Y2),
                    Center = new PointF((float)feature.CenterX, (float)feature.CenterY)
                });
            }
            else
            {
                overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Point,
                    Label = label,
                    Center = new PointF((float)feature.CenterX, (float)feature.CenterY)
                });
            }
        }

        private static void DrawStatus(Mat image, string text, bool pass)
        {
            if (image == null || image.Empty()) return;
            string bounded = text?.Length > 120 ? text.Substring(0, 120) : text ?? string.Empty;
            double scale = FitStatusScale(bounded, image.Width);
            Cv2.PutText(image, bounded, new OpenCvSharp.Point(12, Math.Max(20, Math.Min(image.Height - 10, 28))), HersheyFonts.HersheySimplex, scale, pass ? new Scalar(0, 220, 0) : new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
        }

        private static double FitStatusScale(string text, int imageWidth)
        {
            const double preferred = 0.55D;
            OpenCvSharp.Size size = Cv2.GetTextSize(text ?? string.Empty, HersheyFonts.HersheySimplex, preferred, 2, out _);
            return size.Width <= Math.Max(1, imageWidth - 24)
                ? preferred
                : Math.Max(0.28D, preferred * (imageWidth - 24D) / Math.Max(1D, size.Width));
        }

        private static Point2d PointOf(VisionPipelineGeometryFeatureResult feature) => new Point2d(feature.CenterX, feature.CenterY);
        private static Point2d Midpoint(Point2d a, Point2d b) => new Point2d((a.X + b.X) / 2D, (a.Y + b.Y) / 2D);
        private static double Distance(Point2d a, Point2d b) => Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

        private static Point2d ProjectInfinite(Point2d p, Point2d a, Point2d b)
        {
            double dx = b.X - a.X;
            double dy = b.Y - a.Y;
            double denominator = dx * dx + dy * dy;
            if (denominator <= 1e-12) return a;
            double t = ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / denominator;
            return new Point2d(a.X + t * dx, a.Y + t * dy);
        }

        private static Point2d ProjectFinite(Point2d p, Point2d a, Point2d b)
        {
            double dx = b.X - a.X;
            double dy = b.Y - a.Y;
            double denominator = dx * dx + dy * dy;
            if (denominator <= 1e-12) return a;
            double t = Math.Max(0D, Math.Min(1D, ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / denominator));
            return new Point2d(a.X + t * dx, a.Y + t * dy);
        }

        private static void ClosestSegmentPoints(Point2d a1, Point2d a2, Point2d b1, Point2d b2, out Point2d pa, out Point2d pb)
        {
            if (TryIntersectSegments(a1, a2, b1, b2, out Point2d intersection))
            {
                pa = pb = intersection;
                return;
            }
            var pairs = new[]
            {
                Tuple.Create(a1, ProjectFinite(a1, b1, b2)),
                Tuple.Create(a2, ProjectFinite(a2, b1, b2)),
                Tuple.Create(ProjectFinite(b1, a1, a2), b1),
                Tuple.Create(ProjectFinite(b2, a1, a2), b2)
            };
            Tuple<Point2d, Point2d> best = pairs.OrderBy(pair => Distance(pair.Item1, pair.Item2)).First();
            pa = best.Item1;
            pb = best.Item2;
        }

        private static bool TryIntersectSegments(Point2d a1, Point2d a2, Point2d b1, Point2d b2, out Point2d point)
        {
            if (!TryIntersectInfinite(a1, a2, b1, b2, out point)) return false;
            return ContainsProjection(point, a1, a2) && ContainsProjection(point, b1, b2);
        }

        private static bool TryIntersectInfinite(Point2d a1, Point2d a2, Point2d b1, Point2d b2, out Point2d point)
        {
            double dax = a2.X - a1.X;
            double day = a2.Y - a1.Y;
            double dbx = b2.X - b1.X;
            double dby = b2.Y - b1.Y;
            double denominator = dax * dby - day * dbx;
            if (Math.Abs(denominator) <= 1e-9)
            {
                point = default;
                return false;
            }
            double t = ((b1.X - a1.X) * dby - (b1.Y - a1.Y) * dbx) / denominator;
            point = new Point2d(a1.X + t * dax, a1.Y + t * day);
            return IsFinite(point.X) && IsFinite(point.Y);
        }

        private static bool ContainsProjection(Point2d p, Point2d a, Point2d b)
        {
            double dx = b.X - a.X;
            double dy = b.Y - a.Y;
            double denominator = dx * dx + dy * dy;
            if (denominator <= 1e-12) return Distance(p, a) <= 1e-6;
            double t = ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / denominator;
            return t >= -1e-9 && t <= 1D + 1e-9;
        }

        private static double ExtensionDistance(Point2d p, Point2d a, Point2d b)
        {
            return ContainsProjection(p, a, b) ? 0D : Math.Min(Distance(p, a), Distance(p, b));
        }

        private static double UndirectedAngleDelta(Point2d a1, Point2d a2, Point2d b1, Point2d b2)
        {
            double angleA = Math.Atan2(a2.Y - a1.Y, a2.X - a1.X) * 180D / Math.PI;
            double angleB = Math.Atan2(b2.Y - b1.Y, b2.X - b1.X) * 180D / Math.PI;
            double delta = Math.Abs(angleA - angleB) % 180D;
            return delta > 90D ? 180D - delta : delta;
        }

        private static bool TryGetRect(IDictionary<string, string> parameters, string key, out Rect roi)
        {
            roi = default;
            string[] parts = GetString(parameters, key).Split(',');
            if (parts.Length != 4) return false;
            int[] values = new int[4];
            for (int i = 0; i < 4; i++)
            {
                if (!int.TryParse(parts[i].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out values[i])) return false;
            }
            roi = new Rect(values[0], values[1], values[2], values[3]);
            return roi.Width > 0 && roi.Height > 0;
        }

        private static bool TryGetEnum<T>(IDictionary<string, string> parameters, string key, out T value) where T : struct
        {
            return Enum.TryParse(GetString(parameters, key), true, out value);
        }
        private static string GetString(IDictionary<string, string> parameters, string key) => parameters != null && parameters.TryGetValue(key, out string value) ? value?.Trim() ?? string.Empty : string.Empty;
        private static double GetDouble(IDictionary<string, string> parameters, string key, double fallback) => double.TryParse(GetString(parameters, key), NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ? value : fallback;
        private static bool GetBool(IDictionary<string, string> parameters, string key, bool fallback) => bool.TryParse(GetString(parameters, key), out bool value) ? value : fallback;
        private static string Normalize(string value) => (value ?? string.Empty).Replace("_", string.Empty).Replace(" ", string.Empty).Trim().ToLowerInvariant();
        private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
        private static OpenCvSharp.Point ToPoint(Point2d value) => new OpenCvSharp.Point((int)Math.Round(value.X), (int)Math.Round(value.Y));
        private static PointF ToPointF(Point2d value) => new PointF((float)value.X, (float)value.Y);

        private sealed class Measurement
        {
            public string PrimaryMetricName { get; set; } = VisionPipelineKnownMetrics.GeometryDistancePx;
            public Dictionary<string, double> Metrics { get; } = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            public Point2d Start { get; set; }
            public Point2d End { get; set; }
            public bool HasConstruction { get; set; }
            public Point2d? Intersection { get; set; }

            public void SetDistance(Point2d start, Point2d end, double value)
            {
                Start = start;
                End = end;
                HasConstruction = true;
                PrimaryMetricName = VisionPipelineKnownMetrics.GeometryDistancePx;
                Metrics[PrimaryMetricName] = value;
            }
        }
    }
}
