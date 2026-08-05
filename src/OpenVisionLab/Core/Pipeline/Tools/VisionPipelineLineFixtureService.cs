using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal static class VisionPipelineLineFixtureService
    {
        public const string SourceStepAParameter = "SourceStepA";
        public const string SourceFeatureAParameter = "SourceFeatureA";
        public const string SourceStepBParameter = "SourceStepB";
        public const string SourceFeatureBParameter = "SourceFeatureB";
        public const string MinimumSupportAParameter = "MIN_SUPPORT_A";
        public const string MinimumSupportBParameter = "MIN_SUPPORT_B";
        public const string MaximumFitResidualAParameter = "MAX_FIT_RESIDUAL_A_PX";
        public const string MaximumFitResidualBParameter = "MAX_FIT_RESIDUAL_B_PX";
        public const string MinimumIncludedAngleParameter = "MIN_INCLUDED_ANGLE_DEG";
        public const string MaximumIncludedAngleParameter = "MAX_INCLUDED_ANGLE_DEG";
        public const string MaximumExtensionAParameter = "MAX_EXTENSION_A_PX";
        public const string MaximumExtensionBParameter = "MAX_EXTENSION_B_PX";

        private const int DefaultMinimumSupport = 3;
        private const double DefaultMaximumFitResidual = 2D;
        private const double DefaultMinimumIncludedAngle = 60D;
        private const double DefaultMaximumIncludedAngle = 90D;
        private const double DefaultMaximumExtension = 100D;

        public static bool IsLineFixture(string toolType)
        {
            string normalized = Normalize(toolType);
            return normalized == "linefixture" || normalized == "dualedgefixture";
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
                    "LineFixture input image is empty.",
                    stopwatch.Elapsed);
            }

            Mat resultImage = CreateColor(input);
            Dictionary<string, double> metrics = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                [VisionPipelineKnownMetrics.ResultCount] = 0D
            };
            List<VisionToolOverlay> overlays = new List<VisionToolOverlay>();

            if (!TryResolveSegment(
                    step,
                    input,
                    runResult,
                    SourceStepAParameter,
                    SourceFeatureAParameter,
                    out VisionPipelineGeometryFeatureResult lineA,
                    out string sourceAError))
            {
                return Fail(resultImage, stopwatch, sourceAError, metrics, overlays);
            }

            if (!TryResolveSegment(
                    step,
                    input,
                    runResult,
                    SourceStepBParameter,
                    SourceFeatureBParameter,
                    out VisionPipelineGeometryFeatureResult lineB,
                    out string sourceBError))
            {
                DrawSource(resultImage, overlays, lineA, "Datum A", new Scalar(0, 255, 255));
                return Fail(resultImage, stopwatch, sourceBError, metrics, overlays);
            }

            DrawSource(resultImage, overlays, lineA, "Datum A", new Scalar(0, 255, 255));
            DrawSource(resultImage, overlays, lineB, "Datum B", new Scalar(255, 255, 0));
            AddSourceMetrics(metrics, lineA, lineB);

            int minimumSupportA = GetInt(
                step.Parameters,
                MinimumSupportAParameter,
                DefaultMinimumSupport);
            int minimumSupportB = GetInt(
                step.Parameters,
                MinimumSupportBParameter,
                DefaultMinimumSupport);
            double maximumResidualA = GetDouble(
                step.Parameters,
                MaximumFitResidualAParameter,
                DefaultMaximumFitResidual);
            double maximumResidualB = GetDouble(
                step.Parameters,
                MaximumFitResidualBParameter,
                DefaultMaximumFitResidual);
            double minimumIncludedAngle = GetDouble(
                step.Parameters,
                MinimumIncludedAngleParameter,
                DefaultMinimumIncludedAngle);
            double maximumIncludedAngle = GetDouble(
                step.Parameters,
                MaximumIncludedAngleParameter,
                DefaultMaximumIncludedAngle);
            double maximumExtensionA = GetDouble(
                step.Parameters,
                MaximumExtensionAParameter,
                DefaultMaximumExtension);
            double maximumExtensionB = GetDouble(
                step.Parameters,
                MaximumExtensionBParameter,
                DefaultMaximumExtension);

            if (minimumSupportA <= 0 || minimumSupportB <= 0)
            {
                return Fail(
                    resultImage,
                    stopwatch,
                    "LineFixture minimum support must be greater than zero.",
                    metrics,
                    overlays);
            }

            if (!IsFinite(maximumResidualA)
                || !IsFinite(maximumResidualB)
                || maximumResidualA < 0D
                || maximumResidualB < 0D)
            {
                return Fail(
                    resultImage,
                    stopwatch,
                    "LineFixture maximum fit residual must be finite and zero or greater.",
                    metrics,
                    overlays);
            }

            if (!IsFinite(minimumIncludedAngle)
                || !IsFinite(maximumIncludedAngle)
                || minimumIncludedAngle <= 0D
                || maximumIncludedAngle > 90D
                || minimumIncludedAngle > maximumIncludedAngle)
            {
                return Fail(
                    resultImage,
                    stopwatch,
                    "LineFixture included-angle range must satisfy 0 < minimum <= maximum <= 90 degrees.",
                    metrics,
                    overlays);
            }

            if (!IsFinite(maximumExtensionA)
                || !IsFinite(maximumExtensionB)
                || maximumExtensionA < 0D
                || maximumExtensionB < 0D)
            {
                return Fail(
                    resultImage,
                    stopwatch,
                    "LineFixture maximum extension must be finite and zero or greater.",
                    metrics,
                    overlays);
            }

            if (lineA.SupportCount < minimumSupportA || lineB.SupportCount < minimumSupportB)
            {
                return Fail(
                    resultImage,
                    stopwatch,
                    $"LineFixture support is below the configured minimum. A={lineA.SupportCount}/{minimumSupportA}, B={lineB.SupportCount}/{minimumSupportB}.",
                    metrics,
                    overlays);
            }

            if (lineA.FitResidualPx > maximumResidualA || lineB.FitResidualPx > maximumResidualB)
            {
                return Fail(
                    resultImage,
                    stopwatch,
                    $"LineFixture residual is above the configured maximum. A={lineA.FitResidualPx:0.###}/{maximumResidualA:0.###}px, B={lineB.FitResidualPx:0.###}/{maximumResidualB:0.###}px.",
                    metrics,
                    overlays);
            }

            Point2d a1 = new Point2d(lineA.X1, lineA.Y1);
            Point2d a2 = new Point2d(lineA.X2, lineA.Y2);
            Point2d b1 = new Point2d(lineB.X1, lineB.Y1);
            Point2d b2 = new Point2d(lineB.X2, lineB.Y2);
            double includedAngle = UndirectedAngleDelta(a1, a2, b1, b2);
            metrics[VisionPipelineKnownMetrics.FixtureIncludedAngleDeg] = includedAngle;
            if (includedAngle < minimumIncludedAngle || includedAngle > maximumIncludedAngle)
            {
                return Fail(
                    resultImage,
                    stopwatch,
                    $"LineFixture included angle {includedAngle:0.###} deg is outside {minimumIncludedAngle:0.###}..{maximumIncludedAngle:0.###} deg.",
                    metrics,
                    overlays);
            }

            if (!TryIntersectInfinite(a1, a2, b1, b2, out Point2d origin))
            {
                return Fail(
                    resultImage,
                    stopwatch,
                    "LineFixture requires two non-parallel datum segments.",
                    metrics,
                    overlays);
            }

            double extensionA = ExtensionDistance(origin, a1, a2);
            double extensionB = ExtensionDistance(origin, b1, b2);
            metrics[VisionPipelineKnownMetrics.GeometryExtensionAPx] = extensionA;
            metrics[VisionPipelineKnownMetrics.GeometryExtensionBPx] = extensionB;
            if (extensionA > maximumExtensionA || extensionB > maximumExtensionB)
            {
                return Fail(
                    resultImage,
                    stopwatch,
                    $"LineFixture intersection extension is out of bounds. A={extensionA:0.###}/{maximumExtensionA:0.###}px, B={extensionB:0.###}/{maximumExtensionB:0.###}px.",
                    metrics,
                    overlays);
            }

            if (!IsInside(origin, input.Width, input.Height))
            {
                return Fail(
                    resultImage,
                    stopwatch,
                    $"LineFixture intersection ({origin.X:0.###}, {origin.Y:0.###}) is outside the current image.",
                    metrics,
                    overlays);
            }

            if (!TryGetRequiredDouble(
                    step.Parameters,
                    VisionPipelineFixtureFrameService.ReferenceXParameter,
                    out double referenceX)
                || !TryGetRequiredDouble(
                    step.Parameters,
                    VisionPipelineFixtureFrameService.ReferenceYParameter,
                    out double referenceY)
                || !TryGetRequiredDouble(
                    step.Parameters,
                    VisionPipelineFixtureFrameService.ReferenceAngleParameter,
                    out double referenceAngle))
            {
                return Fail(
                    resultImage,
                    stopwatch,
                    "LineFixture requires a finite taught fixture reference X, Y, and angle.",
                    metrics,
                    overlays);
            }

            // Segment coordinates use the image Y-down convention, while the existing
            // fixture/NormalizeImage contract uses OpenCV's positive counter-clockwise angle.
            double rawImageAngleA = Math.Atan2(a2.Y - a1.Y, a2.X - a1.X) * 180D / Math.PI;
            double currentAngle = OrientUndirectedAngle(-rawImageAngleA, referenceAngle);
            double angleDelta = VisionPipelineFixtureFrameService.NormalizeAngle(
                currentAngle - referenceAngle);
            double offsetX = origin.X - referenceX;
            double offsetY = origin.Y - referenceY;

            metrics[VisionPipelineKnownMetrics.ResultCount] = 1D;
            metrics[VisionPipelineKnownMetrics.FixtureCenterX] = origin.X;
            metrics[VisionPipelineKnownMetrics.FixtureCenterY] = origin.Y;
            metrics[VisionPipelineKnownMetrics.FixtureAngle] = currentAngle;
            metrics[VisionPipelineKnownMetrics.FixtureScale] = 1D;
            metrics[VisionPipelineKnownMetrics.FixtureOffsetX] = offsetX;
            metrics[VisionPipelineKnownMetrics.FixtureOffsetY] = offsetY;
            metrics[VisionPipelineKnownMetrics.FixtureAngleDelta] = angleDelta;
            metrics[VisionPipelineKnownMetrics.FixtureScaleRatio] = 1D;

            int referenceWidth = GetInt(
                step.Parameters,
                VisionPipelineFixtureFrameService.ReferenceImageWidthParameter,
                0);
            int referenceHeight = GetInt(
                step.Parameters,
                VisionPipelineFixtureFrameService.ReferenceImageHeightParameter,
                0);
            if (referenceWidth > 0 && referenceHeight > 0)
            {
                metrics[VisionPipelineKnownMetrics.FixtureReferenceImageWidth] = referenceWidth;
                metrics[VisionPipelineKnownMetrics.FixtureReferenceImageHeight] = referenceHeight;
            }

            DrawFixtureAxes(resultImage, overlays, origin, currentAngle, includedAngle);
            string message =
                $"PASS LineFixture origin=({origin.X:0.###},{origin.Y:0.###}) angle={currentAngle:0.###} deg included={includedAngle:0.###} deg";
            DrawStatus(resultImage, message, true);
            stopwatch.Stop();

            VisionToolResult result = VisionToolResult.Passed(
                resultImage,
                stopwatch.Elapsed,
                metrics,
                overlays);
            result.Message = message;
            VisionPipelineGeometryFeatureStore.Set(
                result,
                new[]
                {
                    new VisionPipelineGeometryFeatureResult
                    {
                        SourceStep = step.Name ?? string.Empty,
                        FeatureName = "Origin",
                        Kind = VisionPipelineGeometryKind.Point,
                        CoordinateLayer = step.InputLayer ?? string.Empty,
                        ImageWidth = input.Width,
                        ImageHeight = input.Height,
                        X1 = origin.X,
                        Y1 = origin.Y,
                        CenterX = origin.X,
                        CenterY = origin.Y,
                        SupportCount = lineA.SupportCount + lineB.SupportCount,
                        SupportRatio = Math.Min(lineA.SupportRatio, lineB.SupportRatio),
                        FitResidualPx = Math.Max(lineA.FitResidualPx, lineB.FitResidualPx)
                    }
                });
            return result;
        }

        private static bool TryResolveSegment(
            VisionPipelineStep consumer,
            Mat input,
            VisionPipelineRunResult runResult,
            string stepKey,
            string featureKey,
            out VisionPipelineGeometryFeatureResult feature,
            out string error)
        {
            feature = null;
            string sourceStep = GetString(consumer?.Parameters, stepKey);
            string featureName = GetString(consumer?.Parameters, featureKey);
            if (string.IsNullOrWhiteSpace(sourceStep) || string.IsNullOrWhiteSpace(featureName))
            {
                error = $"LineFixture requires {stepKey} and {featureKey}.";
                return false;
            }

            List<VisionPipelineStepResult> matches = (runResult?.StepResults
                    ?? new List<VisionPipelineStepResult>())
                .Where(item => item?.Step?.Enabled == true
                    && string.Equals(
                        item.Step.Name,
                        sourceStep,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (matches.Count != 1)
            {
                error = matches.Count == 0
                    ? $"LineFixture source Step '{sourceStep}' is not an earlier enabled Step in this run."
                    : $"LineFixture source Step '{sourceStep}' is ambiguous ({matches.Count} earlier Steps).";
                return false;
            }

            VisionPipelineStepResult source = matches[0];
            string sourceToolType = VisionPipelineNormalizer.NormalizeToolType(
                source.Step.ToolType);
            if (sourceToolType != "line" && sourceToolType != "linegauge")
            {
                error =
                    $"LineFixture source Step '{sourceStep}' must use Line or LineGauge, but uses '{source.Step.ToolType}'.";
                return false;
            }

            if (source.ToolResult?.Success != true || !source.AcceptancePassed)
            {
                error =
                    $"LineFixture source Step '{sourceStep}' did not pass its execution and acceptance gates.";
                return false;
            }

            List<VisionPipelineGeometryFeatureResult> featureMatches =
                VisionPipelineGeometryFeatureStore.Get(source.ToolResult)
                    .Where(item => string.Equals(
                        item.FeatureName,
                        featureName,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            if (featureMatches.Count != 1)
            {
                error = featureMatches.Count == 0
                    ? $"LineFixture feature '{sourceStep}/{featureName}' was not produced in this run."
                    : $"LineFixture feature '{sourceStep}/{featureName}' is ambiguous.";
                return false;
            }

            feature = featureMatches[0];
            if (feature.Kind != VisionPipelineGeometryKind.Segment)
            {
                error =
                    $"LineFixture feature '{sourceStep}/{featureName}' must be Segment, but is {feature.Kind}.";
                feature = null;
                return false;
            }

            if (!string.Equals(
                    feature.CoordinateLayer,
                    consumer.InputLayer,
                    StringComparison.OrdinalIgnoreCase)
                || feature.ImageWidth != input.Width
                || feature.ImageHeight != input.Height)
            {
                error =
                    $"LineFixture feature '{sourceStep}/{featureName}' uses coordinate frame '{feature.CoordinateLayer}' {feature.ImageWidth}x{feature.ImageHeight}, but the consumer input is '{consumer.InputLayer}' {input.Width}x{input.Height}.";
                feature = null;
                return false;
            }

            if (!IsInside(
                    new Point2d(feature.X1, feature.Y1),
                    input.Width,
                    input.Height)
                || !IsInside(
                    new Point2d(feature.X2, feature.Y2),
                    input.Width,
                    input.Height)
                || Distance(
                    new Point2d(feature.X1, feature.Y1),
                    new Point2d(feature.X2, feature.Y2)) <= 1e-9D)
            {
                error =
                    $"LineFixture feature '{sourceStep}/{featureName}' has invalid segment coordinates.";
                feature = null;
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static void AddSourceMetrics(
            IDictionary<string, double> metrics,
            VisionPipelineGeometryFeatureResult lineA,
            VisionPipelineGeometryFeatureResult lineB)
        {
            metrics[VisionPipelineKnownMetrics.FixtureLineASupportCount] = lineA.SupportCount;
            metrics[VisionPipelineKnownMetrics.FixtureLineBSupportCount] = lineB.SupportCount;
            metrics[VisionPipelineKnownMetrics.FixtureLineAFitResidualPx] = lineA.FitResidualPx;
            metrics[VisionPipelineKnownMetrics.FixtureLineBFitResidualPx] = lineB.FitResidualPx;
        }

        private static void DrawSource(
            Mat image,
            ICollection<VisionToolOverlay> overlays,
            VisionPipelineGeometryFeatureResult feature,
            string label,
            Scalar color)
        {
            if (feature == null)
            {
                return;
            }

            OpenCvSharp.Point start = ToPoint(new Point2d(feature.X1, feature.Y1));
            OpenCvSharp.Point end = ToPoint(new Point2d(feature.X2, feature.Y2));
            Cv2.Line(image, start, end, color, 2, LineTypes.AntiAlias);
            Cv2.PutText(
                image,
                label,
                ToPoint(new Point2d(feature.CenterX, feature.CenterY)),
                HersheyFonts.HersheySimplex,
                0.5D,
                color,
                1,
                LineTypes.AntiAlias);
            overlays.Add(new VisionToolOverlay
            {
                Kind = VisionToolOverlayKind.Line,
                Label = label,
                Start = new PointF((float)feature.X1, (float)feature.Y1),
                End = new PointF((float)feature.X2, (float)feature.Y2),
                Center = new PointF((float)feature.CenterX, (float)feature.CenterY)
            });
        }

        private static void DrawFixtureAxes(
            Mat image,
            ICollection<VisionToolOverlay> overlays,
            Point2d origin,
            double angleDeg,
            double includedAngle)
        {
            double axisLength = Math.Max(
                30D,
                Math.Min(image.Width, image.Height) * 0.14D);
            double radians = -angleDeg * Math.PI / 180D;
            Point2d xEnd = new Point2d(
                origin.X + Math.Cos(radians) * axisLength,
                origin.Y + Math.Sin(radians) * axisLength);
            double yRadians = radians + Math.PI / 2D;
            Point2d yEnd = new Point2d(
                origin.X + Math.Cos(yRadians) * axisLength,
                origin.Y + Math.Sin(yRadians) * axisLength);

            Cv2.DrawMarker(
                image,
                ToPoint(origin),
                new Scalar(0, 255, 0),
                MarkerTypes.Cross,
                21,
                3);
            Cv2.ArrowedLine(
                image,
                ToPoint(origin),
                ToPoint(xEnd),
                new Scalar(255, 0, 255),
                3,
                LineTypes.AntiAlias,
                0,
                0.18D);
            Cv2.ArrowedLine(
                image,
                ToPoint(origin),
                ToPoint(yEnd),
                new Scalar(0, 128, 255),
                3,
                LineTypes.AntiAlias,
                0,
                0.18D);
            Cv2.PutText(
                image,
                $"Fixture {angleDeg:0.##} deg / pair {includedAngle:0.##} deg",
                ToPoint(new Point2d(origin.X + 8D, origin.Y + 24D)),
                HersheyFonts.HersheySimplex,
                0.5D,
                new Scalar(0, 255, 0),
                1,
                LineTypes.AntiAlias);

            overlays.Add(new VisionToolOverlay
            {
                Kind = VisionToolOverlayKind.Point,
                Label = "O",
                Center = new PointF((float)origin.X, (float)origin.Y),
                Angle = angleDeg
            });
            overlays.Add(new VisionToolOverlay
            {
                Kind = VisionToolOverlayKind.Line,
                Label = "X",
                Start = new PointF((float)origin.X, (float)origin.Y),
                End = new PointF((float)xEnd.X, (float)xEnd.Y),
                Center = new PointF(
                    (float)((origin.X + xEnd.X) / 2D),
                    (float)((origin.Y + xEnd.Y) / 2D)),
                Angle = angleDeg
            });
            overlays.Add(new VisionToolOverlay
            {
                Kind = VisionToolOverlayKind.Line,
                Label = "Y",
                Start = new PointF((float)origin.X, (float)origin.Y),
                End = new PointF((float)yEnd.X, (float)yEnd.Y),
                Center = new PointF(
                    (float)((origin.X + yEnd.X) / 2D),
                    (float)((origin.Y + yEnd.Y) / 2D)),
                Angle = VisionPipelineFixtureFrameService.NormalizeAngle(angleDeg + 90D)
            });
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
            foreach (KeyValuePair<string, double> metric in metrics
                ?? new Dictionary<string, double>())
            {
                result.Metrics[metric.Key] = metric.Value;
            }
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

        private static void DrawStatus(Mat image, string text, bool pass)
        {
            if (image == null || image.Empty())
            {
                return;
            }

            string bounded = text?.Length > 120
                ? text.Substring(0, 120)
                : text ?? string.Empty;
            const double preferred = 0.55D;
            OpenCvSharp.Size size = Cv2.GetTextSize(
                bounded,
                HersheyFonts.HersheySimplex,
                preferred,
                2,
                out _);
            double scale = size.Width <= Math.Max(1, image.Width - 24)
                ? preferred
                : Math.Max(
                    0.28D,
                    preferred * (image.Width - 24D) / Math.Max(1D, size.Width));
            Cv2.PutText(
                image,
                bounded,
                new OpenCvSharp.Point(
                    12,
                    Math.Max(20, Math.Min(image.Height - 10, 28))),
                HersheyFonts.HersheySimplex,
                scale,
                pass ? new Scalar(0, 220, 0) : new Scalar(0, 0, 255),
                2,
                LineTypes.AntiAlias);
        }

        private static bool TryIntersectInfinite(
            Point2d a1,
            Point2d a2,
            Point2d b1,
            Point2d b2,
            out Point2d point)
        {
            double dax = a2.X - a1.X;
            double day = a2.Y - a1.Y;
            double dbx = b2.X - b1.X;
            double dby = b2.Y - b1.Y;
            double denominator = dax * dby - day * dbx;
            if (Math.Abs(denominator) <= 1e-9D)
            {
                point = default;
                return false;
            }

            double t =
                ((b1.X - a1.X) * dby - (b1.Y - a1.Y) * dbx)
                / denominator;
            point = new Point2d(a1.X + t * dax, a1.Y + t * day);
            return IsFinite(point.X) && IsFinite(point.Y);
        }

        private static double ExtensionDistance(Point2d point, Point2d a, Point2d b)
        {
            double dx = b.X - a.X;
            double dy = b.Y - a.Y;
            double denominator = dx * dx + dy * dy;
            if (denominator <= 1e-12D)
            {
                return Distance(point, a);
            }

            double t =
                ((point.X - a.X) * dx + (point.Y - a.Y) * dy)
                / denominator;
            return t >= 0D && t <= 1D
                ? 0D
                : Math.Min(Distance(point, a), Distance(point, b));
        }

        private static double UndirectedAngleDelta(
            Point2d a1,
            Point2d a2,
            Point2d b1,
            Point2d b2)
        {
            double angleA =
                Math.Atan2(a2.Y - a1.Y, a2.X - a1.X) * 180D / Math.PI;
            double angleB =
                Math.Atan2(b2.Y - b1.Y, b2.X - b1.X) * 180D / Math.PI;
            double delta = Math.Abs(angleA - angleB) % 180D;
            return delta > 90D ? 180D - delta : delta;
        }

        private static double OrientUndirectedAngle(
            double rawAngle,
            double referenceAngle)
        {
            double first = VisionPipelineFixtureFrameService.NormalizeAngle(rawAngle);
            double second = VisionPipelineFixtureFrameService.NormalizeAngle(rawAngle + 180D);
            double firstDelta = Math.Abs(
                VisionPipelineFixtureFrameService.NormalizeAngle(first - referenceAngle));
            double secondDelta = Math.Abs(
                VisionPipelineFixtureFrameService.NormalizeAngle(second - referenceAngle));
            return secondDelta < firstDelta ? second : first;
        }

        private static bool IsInside(Point2d point, int width, int height)
        {
            return IsFinite(point.X)
                && IsFinite(point.Y)
                && point.X >= 0D
                && point.Y >= 0D
                && point.X < width
                && point.Y < height;
        }

        private static double Distance(Point2d a, Point2d b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static OpenCvSharp.Point ToPoint(Point2d point)
        {
            return new OpenCvSharp.Point(
                (int)Math.Round(point.X),
                (int)Math.Round(point.Y));
        }

        private static bool TryGetRequiredDouble(
            IDictionary<string, string> parameters,
            string key,
            out double value)
        {
            return double.TryParse(
                    GetString(parameters, key),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out value)
                && IsFinite(value);
        }

        private static int GetInt(
            IDictionary<string, string> parameters,
            string key,
            int fallback)
        {
            return int.TryParse(
                GetString(parameters, key),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int value)
                    ? value
                    : fallback;
        }

        private static double GetDouble(
            IDictionary<string, string> parameters,
            string key,
            double fallback)
        {
            return double.TryParse(
                GetString(parameters, key),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double value)
                && IsFinite(value)
                    ? value
                    : fallback;
        }

        private static string GetString(
            IDictionary<string, string> parameters,
            string key)
        {
            return parameters != null
                && parameters.TryGetValue(key, out string value)
                    ? value?.Trim() ?? string.Empty
                    : string.Empty;
        }

        private static string Normalize(string value)
        {
            string normalized = (value ?? string.Empty).Trim();
            if (normalized.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(0, normalized.Length - 4);
            }

            return normalized
                .Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .ToLowerInvariant();
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
