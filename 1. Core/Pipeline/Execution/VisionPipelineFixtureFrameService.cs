using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineFixtureFrame
    {
        public string Name { get; set; } = string.Empty;
        public string SourceLayer { get; set; } = string.Empty;
        public double ReferenceX { get; set; }
        public double ReferenceY { get; set; }
        public double ReferenceAngle { get; set; }
        public double CurrentX { get; set; }
        public double CurrentY { get; set; }
        public double CurrentAngle { get; set; }
        public double MaximumAngleDelta { get; set; }
        public double OffsetX => CurrentX - ReferenceX;
        public double OffsetY => CurrentY - ReferenceY;
        public double AngleDelta => VisionPipelineFixtureFrameService.NormalizeAngle(CurrentAngle - ReferenceAngle);
    }

    internal sealed class VisionPipelineFixtureApplication
    {
        public bool Success { get; set; }
        public bool Applied { get; set; }
        public string Message { get; set; } = string.Empty;
        public VisionToolErrorCode ErrorCode { get; set; } = VisionToolErrorCode.None;
        public VisionPipelineStep RuntimeStep { get; set; }
        public VisionPipelineFixtureFrame Frame { get; set; }
        public Rect EffectiveRoi { get; set; }
    }

    internal static class VisionPipelineFixtureFrameService
    {
        public const string PublishParameter = "USE_AS_FIXTURE_FRAME";
        public const string ConsumeParameter = "USE_FIXTURE_FRAME";
        public const string FrameNameParameter = "FIXTURE_FRAME_NAME";
        public const string ReferenceXParameter = "FIXTURE_REFERENCE_X";
        public const string ReferenceYParameter = "FIXTURE_REFERENCE_Y";
        public const string ReferenceAngleParameter = "FIXTURE_REFERENCE_ANGLE";
        public const string MaximumAngleDeltaParameter = "FIXTURE_MAX_ANGLE_DELTA";

        private const double DefaultMaximumAngleDelta = 2d;

        public static bool IsProducer(VisionPipelineStep step)
        {
            return GetBool(step?.Parameters, PublishParameter, false);
        }

        public static bool IsConsumer(VisionPipelineStep step)
        {
            return GetBool(step?.Parameters, ConsumeParameter, false);
        }

        public static VisionPipelineFixtureApplication PrepareRuntimeStep(
            VisionPipelineStep step,
            IReadOnlyDictionary<string, VisionPipelineFixtureFrame> frames)
        {
            if (!IsConsumer(step))
            {
                return new VisionPipelineFixtureApplication
                {
                    Success = true,
                    RuntimeStep = step
                };
            }

            string frameName = GetString(step.Parameters, FrameNameParameter);
            if (string.IsNullOrWhiteSpace(frameName)
                || frames == null
                || !frames.TryGetValue(frameName, out VisionPipelineFixtureFrame frame))
            {
                return Failure(
                    VisionToolErrorCode.InvalidParameter,
                    $"Fixture frame '{frameName}' is not available before step '{step?.Name ?? "Step"}'.");
            }

            if (!string.Equals(step.InputLayer, frame.SourceLayer, StringComparison.OrdinalIgnoreCase))
            {
                return Failure(
                    VisionToolErrorCode.InvalidParameter,
                    $"Step '{step.Name}' reads layer '{step.InputLayer}', but fixture frame '{frame.Name}' was located on '{frame.SourceLayer}'. Translation-only fixture consumers must use the same source layer.");
            }

            if (GetBool(step.Parameters, "USE_MULTI_ROI", false)
                || GetBool(step.Parameters, "USE_MASKING", false))
            {
                return Failure(
                    VisionToolErrorCode.InvalidParameter,
                    $"Step '{step.Name}' uses multi-ROI or masking. Translation-only fixture v1 supports one CvROI only.");
            }

            if (Math.Abs(frame.AngleDelta) > frame.MaximumAngleDelta)
            {
                return Failure(
                    VisionToolErrorCode.InvalidParameter,
                    $"Fixture frame '{frame.Name}' angle delta {frame.AngleDelta:0.###} deg exceeds the translation-only limit {frame.MaximumAngleDelta:0.###} deg.");
            }

            if (!GetBool(step.Parameters, "USE_ROI", false)
                || !TryParseRect(GetString(step.Parameters, "CvROI"), out Rect sourceRoi))
            {
                return Failure(
                    VisionToolErrorCode.InvalidRoi,
                    $"Step '{step.Name}' must enable one CvROI before using fixture frame '{frame.Name}'.");
            }

            if (!IsFinite(frame.OffsetX)
                || !IsFinite(frame.OffsetY)
                || frame.OffsetX < int.MinValue
                || frame.OffsetX > int.MaxValue
                || frame.OffsetY < int.MinValue
                || frame.OffsetY > int.MaxValue)
            {
                return Failure(
                    VisionToolErrorCode.InvalidParameter,
                    $"Fixture frame '{frame.Name}' produced an invalid translation offset.");
            }

            int offsetX = RoundToInt(frame.OffsetX);
            int offsetY = RoundToInt(frame.OffsetY);
            Rect effectiveRoi = new Rect(
                sourceRoi.X + offsetX,
                sourceRoi.Y + offsetY,
                sourceRoi.Width,
                sourceRoi.Height);
            VisionPipelineStep runtimeStep = CloneStep(step);
            runtimeStep.Parameters["CvROI"] = FormatRect(effectiveRoi);

            return new VisionPipelineFixtureApplication
            {
                Success = true,
                Applied = true,
                RuntimeStep = runtimeStep,
                Frame = frame,
                EffectiveRoi = effectiveRoi,
                Message = $"Fixture '{frame.Name}' moved ROI by ({offsetX},{offsetY}) px."
            };
        }

        public static bool TryCreatePublishedFrame(
            VisionPipelineStep step,
            VisionToolResult result,
            out VisionPipelineFixtureFrame frame,
            out string message)
        {
            frame = null;
            message = string.Empty;
            if (!IsProducer(step))
            {
                return true;
            }

            string toolType = VisionPipelineNormalizer.NormalizeToolType(step.ToolType);
            if (toolType != "matching" && toolType != "templatematching")
            {
                message = $"Fixture producer '{step.Name}' must use Matching in the translation-only workflow.";
                return false;
            }

            if (GetInt(step.Parameters, "NUM_MATCH", 0) != 1)
            {
                message = $"Fixture producer '{step.Name}' requires NUM_MATCH=1 so the reference pose is unambiguous.";
                return false;
            }

            if (!TryGetRequiredDouble(step, ReferenceXParameter, out double referenceX, out message)
                || !TryGetRequiredDouble(step, ReferenceYParameter, out double referenceY, out message)
                || !TryGetRequiredDouble(step, ReferenceAngleParameter, out double referenceAngle, out message))
            {
                return false;
            }

            string maximumAngleText = GetString(step.Parameters, MaximumAngleDeltaParameter);
            double maximumAngleDelta = DefaultMaximumAngleDelta;
            if ((!string.IsNullOrWhiteSpace(maximumAngleText)
                    && !double.TryParse(maximumAngleText, NumberStyles.Float, CultureInfo.InvariantCulture, out maximumAngleDelta))
                || !IsFinite(maximumAngleDelta)
                || maximumAngleDelta < 0d)
            {
                message = $"Fixture producer '{step.Name}' requires {MaximumAngleDeltaParameter} >= 0.";
                return false;
            }

            VisionToolOverlay overlay = (result?.Overlays ?? new List<VisionToolOverlay>())
                .FirstOrDefault(item => item != null
                    && item.Kind == VisionToolOverlayKind.Rectangle
                    && item.Bounds.Width > 0
                    && item.Bounds.Height > 0)
                ?? (result?.Overlays ?? new List<VisionToolOverlay>())
                    .FirstOrDefault(item => item != null && item.Bounds.Width > 0 && item.Bounds.Height > 0);
            if (overlay == null)
            {
                message = $"Fixture producer '{step.Name}' returned no rectangle pose overlay.";
                return false;
            }

            double currentX = overlay.Center.X;
            double currentY = overlay.Center.Y;
            if ((!IsFinite(currentX) || !IsFinite(currentY))
                || (Math.Abs(currentX) < double.Epsilon
                    && Math.Abs(currentY) < double.Epsilon
                    && (Math.Abs(overlay.Bounds.X) > double.Epsilon || Math.Abs(overlay.Bounds.Y) > double.Epsilon)))
            {
                currentX = overlay.Bounds.X + overlay.Bounds.Width / 2d;
                currentY = overlay.Bounds.Y + overlay.Bounds.Height / 2d;
            }

            if (!IsFinite(currentX) || !IsFinite(currentY) || !IsFinite(overlay.Angle))
            {
                message = $"Fixture producer '{step.Name}' returned an invalid pose.";
                return false;
            }

            frame = new VisionPipelineFixtureFrame
            {
                Name = GetString(step.Parameters, FrameNameParameter),
                SourceLayer = step.InputLayer ?? string.Empty,
                ReferenceX = referenceX,
                ReferenceY = referenceY,
                ReferenceAngle = referenceAngle,
                CurrentX = currentX,
                CurrentY = currentY,
                CurrentAngle = overlay.Angle,
                MaximumAngleDelta = maximumAngleDelta
            };

            if (string.IsNullOrWhiteSpace(frame.Name))
            {
                message = $"Fixture producer '{step.Name}' requires {FrameNameParameter}.";
                frame = null;
                return false;
            }

            if (Math.Abs(frame.AngleDelta) > frame.MaximumAngleDelta)
            {
                message = $"Fixture frame '{frame.Name}' angle delta {frame.AngleDelta:0.###} deg exceeds the translation-only limit {frame.MaximumAngleDelta:0.###} deg.";
                frame = null;
                return false;
            }

            AddFrameMetrics(result.Metrics, frame);
            return true;
        }

        public static void AddApplicationMetrics(VisionToolResult result, VisionPipelineFixtureApplication application)
        {
            if (result?.Metrics == null || application?.Applied != true || application.Frame == null)
            {
                return;
            }

            AddFrameMetrics(result.Metrics, application.Frame);
            result.Metrics[VisionPipelineKnownMetrics.FixtureEffectiveRoiX] = application.EffectiveRoi.X;
            result.Metrics[VisionPipelineKnownMetrics.FixtureEffectiveRoiY] = application.EffectiveRoi.Y;
        }

        public static void ValidatePipelineDefinition(
            VisionPipeline pipeline,
            ICollection<string> errors,
            ICollection<string> warnings)
        {
            if (pipeline?.Steps == null)
            {
                return;
            }

            Dictionary<string, VisionPipelineStep> producers = new Dictionary<string, VisionPipelineStep>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < pipeline.Steps.Count; i++)
            {
                VisionPipelineStep step = pipeline.Steps[i];
                if (step == null || !step.Enabled)
                {
                    continue;
                }

                string label = $"Step {i + 1} '{step.Name}'";
                bool producer = IsProducer(step);
                bool consumer = IsConsumer(step);
                string frameName = GetString(step.Parameters, FrameNameParameter);

                if (producer)
                {
                    string toolType = VisionPipelineNormalizer.NormalizeToolType(step.ToolType);
                    if (toolType != "matching" && toolType != "templatematching")
                    {
                        errors?.Add($"{label}: {PublishParameter} currently supports Matching only.");
                    }

                    if (string.IsNullOrWhiteSpace(frameName))
                    {
                        errors?.Add($"{label}: {FrameNameParameter} is required for a fixture producer.");
                    }
                    else if (producers.ContainsKey(frameName))
                    {
                        errors?.Add($"{label}: fixture frame '{frameName}' is already published by an earlier step.");
                    }
                    else
                    {
                        producers[frameName] = step;
                    }

                    if (GetInt(step.Parameters, "NUM_MATCH", 0) != 1)
                    {
                        errors?.Add($"{label}: fixture producers require NUM_MATCH=1.");
                    }

                    ValidateRequiredDouble(step, label, ReferenceXParameter, errors);
                    ValidateRequiredDouble(step, label, ReferenceYParameter, errors);
                    ValidateRequiredDouble(step, label, ReferenceAngleParameter, errors);
                    double maximumAngle = GetDouble(step.Parameters, MaximumAngleDeltaParameter, DefaultMaximumAngleDelta);
                    if (!IsFinite(maximumAngle) || maximumAngle < 0d)
                    {
                        errors?.Add($"{label}: {MaximumAngleDeltaParameter} must be zero or greater.");
                    }
                }

                if (!consumer)
                {
                    continue;
                }

                if (producer)
                {
                    errors?.Add($"{label}: one step cannot publish and consume a fixture frame in the translation-only workflow.");
                }

                if (string.IsNullOrWhiteSpace(frameName) || !producers.TryGetValue(frameName, out VisionPipelineStep fixtureStep))
                {
                    errors?.Add($"{label}: fixture frame '{frameName}' must be published by an earlier enabled Matching step.");
                    continue;
                }

                if (!GetBool(step.Parameters, "USE_ROI", false)
                    || !TryParseRect(GetString(step.Parameters, "CvROI"), out _))
                {
                    errors?.Add($"{label}: one enabled CvROI is required when {ConsumeParameter}=true.");
                }

                if (GetBool(step.Parameters, "USE_MULTI_ROI", false)
                    || GetBool(step.Parameters, "USE_MASKING", false))
                {
                    errors?.Add($"{label}: translation-only fixture v1 supports one CvROI and does not transform multi-ROI or masks.");
                }

                if (!string.Equals(step.InputLayer, fixtureStep.InputLayer, StringComparison.OrdinalIgnoreCase))
                {
                    errors?.Add($"{label}: input layer '{step.InputLayer}' must match fixture source layer '{fixtureStep.InputLayer}'.");
                }

                if (!GetBool(step.Parameters, VisionPipelineNormalizer.AllowBranchInputParameter, false))
                {
                    warnings?.Add($"{label}: set {VisionPipelineNormalizer.AllowBranchInputParameter}=true to make the intentional fixture branch explicit.");
                }
            }
        }

        internal static double NormalizeAngle(double angle)
        {
            while (angle > 180d)
            {
                angle -= 360d;
            }

            while (angle <= -180d)
            {
                angle += 360d;
            }

            return angle;
        }

        private static void AddFrameMetrics(IDictionary<string, double> metrics, VisionPipelineFixtureFrame frame)
        {
            if (metrics == null || frame == null)
            {
                return;
            }

            metrics[VisionPipelineKnownMetrics.FixtureCenterX] = frame.CurrentX;
            metrics[VisionPipelineKnownMetrics.FixtureCenterY] = frame.CurrentY;
            metrics[VisionPipelineKnownMetrics.FixtureAngle] = frame.CurrentAngle;
            metrics[VisionPipelineKnownMetrics.FixtureOffsetX] = frame.OffsetX;
            metrics[VisionPipelineKnownMetrics.FixtureOffsetY] = frame.OffsetY;
            metrics[VisionPipelineKnownMetrics.FixtureAngleDelta] = frame.AngleDelta;
        }

        private static VisionPipelineFixtureApplication Failure(VisionToolErrorCode errorCode, string message)
        {
            return new VisionPipelineFixtureApplication
            {
                Success = false,
                ErrorCode = errorCode,
                Message = message ?? string.Empty
            };
        }

        private static VisionPipelineStep CloneStep(VisionPipelineStep source)
        {
            VisionPipelineStep clone = new VisionPipelineStep
            {
                Name = source.Name,
                ToolType = source.ToolType,
                Enabled = source.Enabled,
                InputLayer = source.InputLayer,
                OutputLayer = source.OutputLayer,
                UseAcceptance = source.UseAcceptance,
                ExpectedSuccess = source.ExpectedSuccess,
                MaxElapsedMilliseconds = source.MaxElapsedMilliseconds,
                RequiredMessageText = source.RequiredMessageText,
                AcceptanceMetricName = source.AcceptanceMetricName,
                UseAcceptanceMetricMinimum = source.UseAcceptanceMetricMinimum,
                AcceptanceMetricMinimum = source.AcceptanceMetricMinimum,
                UseAcceptanceMetricMaximum = source.UseAcceptanceMetricMaximum,
                AcceptanceMetricMaximum = source.AcceptanceMetricMaximum
            };

            foreach (KeyValuePair<string, string> parameter in source.Parameters)
            {
                clone.Parameters[parameter.Key] = parameter.Value;
            }

            return clone;
        }

        private static bool TryGetRequiredDouble(
            VisionPipelineStep step,
            string key,
            out double value,
            out string message)
        {
            string text = GetString(step?.Parameters, key);
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && IsFinite(value))
            {
                message = string.Empty;
                return true;
            }

            message = $"Fixture producer '{step?.Name ?? "Step"}' requires numeric {key}.";
            return false;
        }

        private static void ValidateRequiredDouble(
            VisionPipelineStep step,
            string label,
            string key,
            ICollection<string> errors)
        {
            if (!TryGetRequiredDouble(step, key, out _, out _))
            {
                errors?.Add($"{label}: numeric {key} is required.");
            }
        }

        private static bool TryParseRect(string value, out Rect rect)
        {
            rect = default;
            string[] parts = (value ?? string.Empty).Split(',');
            if (parts.Length != 4
                || !int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)
                || !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)
                || !int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int width)
                || !int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int height))
            {
                return false;
            }

            rect = new Rect(x, y, width, height);
            return width > 0 && height > 0;
        }

        private static string FormatRect(Rect rect)
        {
            return string.Join(",", new[]
            {
                rect.X.ToString(CultureInfo.InvariantCulture),
                rect.Y.ToString(CultureInfo.InvariantCulture),
                rect.Width.ToString(CultureInfo.InvariantCulture),
                rect.Height.ToString(CultureInfo.InvariantCulture)
            });
        }

        private static int RoundToInt(double value)
        {
            return checked((int)Math.Round(value, MidpointRounding.AwayFromZero));
        }

        private static bool GetBool(IDictionary<string, string> parameters, string key, bool defaultValue)
        {
            string text = GetString(parameters, key);
            return bool.TryParse(text, out bool value) ? value : defaultValue;
        }

        private static int GetInt(IDictionary<string, string> parameters, string key, int defaultValue)
        {
            string text = GetString(parameters, key);
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                ? value
                : defaultValue;
        }

        private static double GetDouble(IDictionary<string, string> parameters, string key, double defaultValue)
        {
            string text = GetString(parameters, key);
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                ? value
                : defaultValue;
        }

        private static string GetString(IDictionary<string, string> parameters, string key)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                if (string.Equals(parameter.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return parameter.Value?.Trim() ?? string.Empty;
                }
            }

            return string.Empty;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
