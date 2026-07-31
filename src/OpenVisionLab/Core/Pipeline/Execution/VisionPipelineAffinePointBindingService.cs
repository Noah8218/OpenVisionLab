using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineAffinePointBindingApplication
    {
        public bool Success { get; set; }
        public bool Applied { get; set; }
        public VisionToolErrorCode ErrorCode { get; set; } = VisionToolErrorCode.None;
        public string Message { get; set; } = string.Empty;
        public VisionPipelineStep RuntimeStep { get; set; }
        public IReadOnlyList<VisionPipelineGeometryFeatureResult> SourcePoints { get; set; }
            = Array.Empty<VisionPipelineGeometryFeatureResult>();
    }

    internal static class VisionPipelineAffinePointBindingService
    {
        public const string UseDetectedSourcePointsParameter = "USE_DETECTED_SOURCE_POINTS";
        public const string SourcePoint1FeatureParameter = "SOURCE_POINT_1_FEATURE";
        public const string SourcePoint2FeatureParameter = "SOURCE_POINT_2_FEATURE";
        public const string SourcePoint3FeatureParameter = "SOURCE_POINT_3_FEATURE";

        public static bool IsDetectedPointConsumer(VisionPipelineStep step)
        {
            string type = VisionPipelineNormalizer.NormalizeToolType(step?.ToolType);
            return (type == "affine" || type == "affinematrix" || type == "affinetransform")
                && GetBool(step?.Parameters, UseDetectedSourcePointsParameter, false);
        }

        public static VisionPipelineAffinePointBindingApplication PrepareRuntimeStep(
            VisionPipelineStep step,
            Mat input,
            VisionPipelineRunResult runResult)
        {
            if (!IsDetectedPointConsumer(step))
            {
                return new VisionPipelineAffinePointBindingApplication
                {
                    Success = true,
                    RuntimeStep = step
                };
            }

            if (input == null || input.Empty())
            {
                return Failure(VisionToolErrorCode.InputImageInvalid, "Affine detected-point input image is empty.");
            }

            string[] references =
            {
                GetString(step.Parameters, SourcePoint1FeatureParameter),
                GetString(step.Parameters, SourcePoint2FeatureParameter),
                GetString(step.Parameters, SourcePoint3FeatureParameter)
            };
            if (references.Any(string.IsNullOrWhiteSpace))
            {
                return Failure(
                    VisionToolErrorCode.InvalidParameter,
                    $"Affine detected-point mode requires {SourcePoint1FeatureParameter}, {SourcePoint2FeatureParameter}, and {SourcePoint3FeatureParameter}.");
            }

            if (references.Distinct(StringComparer.OrdinalIgnoreCase).Count() != 3)
            {
                return Failure(VisionToolErrorCode.InvalidParameter, "Affine detected-point mode requires three distinct Point features.");
            }

            List<VisionPipelineGeometryFeatureResult> points = new List<VisionPipelineGeometryFeatureResult>();
            foreach (string reference in references)
            {
                if (!TryResolvePoint(step, input, runResult, reference, out VisionPipelineGeometryFeatureResult point, out string error))
                {
                    return Failure(VisionToolErrorCode.InvalidParameter, error);
                }

                points.Add(point);
            }

            VisionPipelineStep runtimeStep = CloneStep(step);
            SetDouble(runtimeStep.Parameters, nameof(AffineTransformToolProperty.SourcePoint1X), points[0].CenterX);
            SetDouble(runtimeStep.Parameters, nameof(AffineTransformToolProperty.SourcePoint1Y), points[0].CenterY);
            SetDouble(runtimeStep.Parameters, nameof(AffineTransformToolProperty.SourcePoint2X), points[1].CenterX);
            SetDouble(runtimeStep.Parameters, nameof(AffineTransformToolProperty.SourcePoint2Y), points[1].CenterY);
            SetDouble(runtimeStep.Parameters, nameof(AffineTransformToolProperty.SourcePoint3X), points[2].CenterX);
            SetDouble(runtimeStep.Parameters, nameof(AffineTransformToolProperty.SourcePoint3Y), points[2].CenterY);

            return new VisionPipelineAffinePointBindingApplication
            {
                Success = true,
                Applied = true,
                RuntimeStep = runtimeStep,
                SourcePoints = points,
                Message = "Affine source points resolved from three earlier accepted Point features."
            };
        }

        public static void AddApplicationMetrics(
            VisionToolResult result,
            VisionPipelineAffinePointBindingApplication application)
        {
            if (result?.Metrics == null || application?.Applied != true || application.SourcePoints.Count != 3)
            {
                return;
            }

            result.Metrics[VisionPipelineKnownMetrics.AffineDetectedSourcePointCount] = 3D;
            result.Metrics[VisionPipelineKnownMetrics.AffineSourcePoint1X] = application.SourcePoints[0].CenterX;
            result.Metrics[VisionPipelineKnownMetrics.AffineSourcePoint1Y] = application.SourcePoints[0].CenterY;
            result.Metrics[VisionPipelineKnownMetrics.AffineSourcePoint2X] = application.SourcePoints[1].CenterX;
            result.Metrics[VisionPipelineKnownMetrics.AffineSourcePoint2Y] = application.SourcePoints[1].CenterY;
            result.Metrics[VisionPipelineKnownMetrics.AffineSourcePoint3X] = application.SourcePoints[2].CenterX;
            result.Metrics[VisionPipelineKnownMetrics.AffineSourcePoint3Y] = application.SourcePoints[2].CenterY;
        }

        public static void ValidatePipelineDefinition(
            VisionPipeline pipeline,
            ICollection<string> errors)
        {
            if (pipeline?.Steps == null)
            {
                return;
            }

            for (int index = 0; index < pipeline.Steps.Count; index++)
            {
                VisionPipelineStep consumer = pipeline.Steps[index];
                if (consumer?.Enabled != true || !IsDetectedPointConsumer(consumer))
                {
                    continue;
                }

                string label = $"Step {index + 1} '{consumer.Name}'";
                string[] references =
                {
                    GetString(consumer.Parameters, SourcePoint1FeatureParameter),
                    GetString(consumer.Parameters, SourcePoint2FeatureParameter),
                    GetString(consumer.Parameters, SourcePoint3FeatureParameter)
                };
                if (references.Any(string.IsNullOrWhiteSpace))
                {
                    errors?.Add($"{label}: detected-point Affine requires three source Point feature references.");
                    continue;
                }

                if (references.Distinct(StringComparer.OrdinalIgnoreCase).Count() != 3)
                {
                    errors?.Add($"{label}: detected-point Affine source Point references must be distinct.");
                }

                foreach (string reference in references)
                {
                    if (!TrySplitReference(reference, out string sourceStep, out string featureName))
                    {
                        errors?.Add($"{label}: invalid Point feature reference '{reference}'. Use StepName/FeatureName.");
                        continue;
                    }

                    List<VisionPipelineStep> matches = pipeline.Steps
                        .Take(index)
                        .Where(candidate => candidate?.Enabled == true
                            && string.Equals(candidate.Name, sourceStep, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    if (matches.Count != 1)
                    {
                        errors?.Add(matches.Count == 0
                            ? $"{label}: source Step '{sourceStep}' is not an earlier enabled Step."
                            : $"{label}: source Step '{sourceStep}' is ambiguous.");
                        continue;
                    }

                    VisionPipelineStep producer = matches[0];
                    bool declaredPoint = VisionPipelineGeometryFeatureCatalog.GetDeclaredFeatures(producer)
                        .Any(item => item.Kind == VisionPipelineGeometryKind.Point
                            && string.Equals(item.FeatureName, featureName, StringComparison.OrdinalIgnoreCase));
                    if (!declaredPoint)
                    {
                        errors?.Add($"{label}: '{reference}' is not a declared Point feature.");
                    }

                    if (!string.Equals(producer.InputLayer, consumer.InputLayer, StringComparison.OrdinalIgnoreCase))
                    {
                        errors?.Add(
                            $"{label}: source '{reference}' uses coordinate layer '{producer.InputLayer}', but Affine input is '{consumer.InputLayer}'.");
                    }
                }
            }
        }

        private static bool TryResolvePoint(
            VisionPipelineStep consumer,
            Mat input,
            VisionPipelineRunResult runResult,
            string reference,
            out VisionPipelineGeometryFeatureResult feature,
            out string error)
        {
            feature = null;
            if (!TrySplitReference(reference, out string sourceStep, out string featureName))
            {
                error = $"Affine Point feature reference '{reference}' is invalid. Use StepName/FeatureName.";
                return false;
            }

            List<VisionPipelineStepResult> matches = (runResult?.StepResults ?? new List<VisionPipelineStepResult>())
                .Where(item => item?.Step?.Enabled == true
                    && string.Equals(item.Step.Name, sourceStep, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (matches.Count != 1)
            {
                error = matches.Count == 0
                    ? $"Affine source Step '{sourceStep}' is not an earlier enabled Step in this run."
                    : $"Affine source Step '{sourceStep}' is ambiguous ({matches.Count} earlier Steps).";
                return false;
            }

            VisionPipelineStepResult source = matches[0];
            if (source.ToolResult?.Success != true || !source.AcceptancePassed)
            {
                error = $"Affine source Step '{sourceStep}' did not pass its execution and acceptance gates.";
                return false;
            }

            List<VisionPipelineGeometryFeatureResult> featureMatches = VisionPipelineGeometryFeatureStore.Get(source.ToolResult)
                .Where(item => string.Equals(item.FeatureName, featureName, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (featureMatches.Count != 1)
            {
                error = featureMatches.Count == 0
                    ? $"Affine Point feature '{reference}' was not produced in this run."
                    : $"Affine Point feature '{reference}' is ambiguous.";
                return false;
            }

            feature = featureMatches[0];
            if (feature.Kind != VisionPipelineGeometryKind.Point)
            {
                error = $"Affine source '{reference}' must be a Point but is {feature.Kind}.";
                feature = null;
                return false;
            }

            if (!string.Equals(feature.CoordinateLayer, consumer.InputLayer, StringComparison.OrdinalIgnoreCase)
                || feature.ImageWidth != input.Width
                || feature.ImageHeight != input.Height)
            {
                error = $"Affine Point '{reference}' uses coordinate frame '{feature.CoordinateLayer}' {feature.ImageWidth}x{feature.ImageHeight}, but Affine input is '{consumer.InputLayer}' {input.Width}x{input.Height}.";
                feature = null;
                return false;
            }

            if (!IsFinite(feature.CenterX)
                || !IsFinite(feature.CenterY)
                || feature.CenterX < 0D
                || feature.CenterY < 0D
                || feature.CenterX >= input.Width
                || feature.CenterY >= input.Height)
            {
                error = $"Affine Point '{reference}' must be finite and inside the current input image.";
                feature = null;
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static VisionPipelineAffinePointBindingApplication Failure(
            VisionToolErrorCode errorCode,
            string message)
        {
            return new VisionPipelineAffinePointBindingApplication
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

        private static bool TrySplitReference(string reference, out string step, out string feature)
        {
            int slash = (reference ?? string.Empty).LastIndexOf('/');
            step = slash > 0 ? reference.Substring(0, slash).Trim() : string.Empty;
            feature = slash > 0 && slash < reference.Length - 1
                ? reference.Substring(slash + 1).Trim()
                : string.Empty;
            return !string.IsNullOrWhiteSpace(step) && !string.IsNullOrWhiteSpace(feature);
        }

        private static string GetString(IDictionary<string, string> parameters, string key)
        {
            return parameters != null && parameters.TryGetValue(key, out string value)
                ? value?.Trim() ?? string.Empty
                : string.Empty;
        }

        private static bool GetBool(IDictionary<string, string> parameters, string key, bool fallback)
        {
            return bool.TryParse(GetString(parameters, key), out bool value) ? value : fallback;
        }

        private static void SetDouble(IDictionary<string, string> parameters, string key, double value)
        {
            parameters[key] = value.ToString("R", CultureInfo.InvariantCulture);
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
