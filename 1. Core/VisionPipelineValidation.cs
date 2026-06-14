using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineValidationResult
    {
        public List<string> Errors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();
        public bool Success => Errors.Count == 0;

        public string FormatErrors()
        {
            return string.Join(Environment.NewLine, Errors);
        }

        public string FormatWarnings()
        {
            return string.Join(Environment.NewLine, Warnings);
        }
    }

    internal static class VisionPipelineValidator
    {
        private static readonly HashSet<string> SupportedToolTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "threshold",
            "morphology",
            "filter",
            "edgedetection",
            "edge",
            "blob",
            "contour",
            "line",
            "linegauge",
            "matching",
            "templatematching",
            "mean",
            "feature",
            "featurematching",
            "sift"
        };

        public static VisionPipelineValidationResult Validate(VisionPipeline pipeline, IEnumerable<string> sourceLayers)
        {
            VisionPipelineValidationResult result = new VisionPipelineValidationResult();
            if (pipeline == null)
            {
                result.Errors.Add("Pipeline is null.");
                return result;
            }

            if (pipeline.Steps.Count == 0)
            {
                result.Errors.Add("Pipeline has no steps.");
                return result;
            }

            HashSet<string> availableLayers = new HashSet<string>(
                (sourceLayers ?? Enumerable.Empty<string>()).Where(layer => !string.IsNullOrWhiteSpace(layer)),
                StringComparer.OrdinalIgnoreCase);

            HashSet<string> stepNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> outputLayers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            bool hasEnabledStep = false;
            VisionPipelineStep previousEnabledStep = null;

            for (int i = 0; i < pipeline.Steps.Count; i++)
            {
                VisionPipelineStep step = pipeline.Steps[i];
                string label = $"Step {i + 1}";

                if (step == null)
                {
                    result.Errors.Add($"{label}: step is null.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(step.Name))
                {
                    result.Errors.Add($"{label}: Name is required.");
                }
                else if (!stepNames.Add(step.Name))
                {
                    result.Warnings.Add($"{label}: duplicate step name '{step.Name}'.");
                }

                if (string.IsNullOrWhiteSpace(step.ToolType))
                {
                    result.Errors.Add($"{label}: ToolType is required.");
                }
                else if (!SupportedToolTypes.Contains(VisionPipelineNormalizer.NormalizeToolType(step.ToolType)))
                {
                    result.Errors.Add($"{label} '{step.Name}': unsupported ToolType '{step.ToolType}'.");
                }

                if (!step.Enabled)
                {
                    result.Warnings.Add($"{label} '{step.Name}': step is disabled and will be skipped.");
                    continue;
                }

                hasEnabledStep = true;
                ValidateAcceptance(result, label, step);
                ValidateParameters(result, label, step);

                if (previousEnabledStep != null
                    && !string.IsNullOrWhiteSpace(previousEnabledStep.OutputLayer)
                    && !string.IsNullOrWhiteSpace(step.InputLayer)
                    && !string.Equals(step.InputLayer, previousEnabledStep.OutputLayer, StringComparison.OrdinalIgnoreCase))
                {
                    result.Warnings.Add(
                        $"{label} '{step.Name}': Review branch input. This step reads '{step.InputLayer}' while the previous step outputs '{previousEnabledStep.OutputLayer}'. Keep this only when the step should intentionally start from that layer.");
                }
                else if (previousEnabledStep != null
                    && !string.IsNullOrWhiteSpace(previousEnabledStep.OutputLayer)
                    && !string.IsNullOrWhiteSpace(step.InputLayer)
                    && string.Equals(step.InputLayer, previousEnabledStep.OutputLayer, StringComparison.OrdinalIgnoreCase)
                    && VisionPipelineNormalizer.IsOpenCvInspectionToolWithInternalThreshold(VisionPipelineNormalizer.NormalizeToolType(step.ToolType))
                    && VisionPipelineNormalizer.HasInternalPreprocessingEnabled(step))
                {
                    result.Warnings.Add(
                        $"{label} '{step.Name}': Review duplicated preprocessing. Input already comes from previous processed output '{step.InputLayer}', but this tool still has internal Threshold/Adaptive/Invert options enabled. Usually turn those off when a separate preprocessing step exists.");
                }

                if (string.IsNullOrWhiteSpace(step.InputLayer))
                {
                    result.Errors.Add($"{label} '{step.Name}': InputLayer is required.");
                }
                else if (!availableLayers.Contains(step.InputLayer))
                {
                    result.Errors.Add($"{label} '{step.Name}': input layer '{step.InputLayer}' does not exist before this step.");
                }

                if (string.IsNullOrWhiteSpace(step.OutputLayer))
                {
                    result.Errors.Add($"{label} '{step.Name}': OutputLayer is required.");
                }
                else
                {
                    if (!outputLayers.Add(step.OutputLayer))
                    {
                        result.Warnings.Add($"{label} '{step.Name}': Review output layer. '{step.OutputLayer}' is written by more than one step, so later steps can overwrite earlier results.");
                    }

                    if (string.Equals(step.InputLayer, step.OutputLayer, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Warnings.Add($"{label} '{step.Name}': Review output layer. Input and output are both '{step.OutputLayer}'. Prefer a separate output layer so the source image is preserved.");
                    }

                    availableLayers.Add(step.OutputLayer);
                }

                previousEnabledStep = step;
            }

            if (!hasEnabledStep)
            {
                result.Errors.Add("Pipeline has no enabled steps.");
            }

            return result;
        }

        private static void ValidateAcceptance(VisionPipelineValidationResult result, string label, VisionPipelineStep step)
        {
            if (step.MaxElapsedMilliseconds < 0)
            {
                result.Errors.Add($"{label} '{step.Name}': MaxElapsedMilliseconds cannot be negative.");
            }

            if (!step.UseAcceptance)
            {
                return;
            }

            bool hasCriteria = step.MaxElapsedMilliseconds > 0
                || !string.IsNullOrWhiteSpace(step.RequiredMessageText)
                || !string.IsNullOrWhiteSpace(step.AcceptanceMetricName)
                || !step.ExpectedSuccess;
            if (!hasCriteria)
            {
                result.Warnings.Add($"{label} '{step.Name}': Acceptance is enabled, but no pass/fail criteria are configured.");
            }

            if (step.UseAcceptanceMetricMinimum
                && step.UseAcceptanceMetricMaximum
                && step.AcceptanceMetricMinimum > step.AcceptanceMetricMaximum)
            {
                result.Errors.Add($"{label} '{step.Name}': acceptance metric minimum is greater than maximum.");
            }

            if (!string.IsNullOrWhiteSpace(step.AcceptanceMetricName)
                && !VisionPipelineKnownMetrics.IsKnownMetric(step.AcceptanceMetricName))
            {
                result.Warnings.Add($"{label} '{step.Name}': Acceptance metric '{step.AcceptanceMetricName}' is not known. Check the metric name before using it for OK/NG judgment.");
            }
            else if (!string.IsNullOrWhiteSpace(step.AcceptanceMetricName)
                && !VisionPipelineKnownMetrics.IsMetricRecommendedForTool(step.ToolType, step.AcceptanceMetricName))
            {
                string recommended = VisionPipelineKnownMetrics.FormatMetricListForTool(step.ToolType);
                result.Warnings.Add($"{label} '{step.Name}': Acceptance metric '{step.AcceptanceMetricName}' is unusual for {step.ToolType}. Recommended metrics: {recommended}.");
            }
        }

        private static void ValidateParameters(VisionPipelineValidationResult result, string label, VisionPipelineStep step)
        {
            foreach (KeyValuePair<string, string> parameter in step.Parameters)
            {
                if (!VisionPipelineStepParameterSchema.TryValidateValue(parameter.Key, parameter.Value, out string message))
                {
                    result.Errors.Add($"{label} '{step.Name}': {message}");
                }
            }

            ValidateMinMax(result, label, step, "MIN_AREA", "MAX_AREA");
            ValidateMinMax(result, label, step, "RangeMin", "RangeMax");
            ValidateMinMax(result, label, step, "MEAN_MIN", "MEAN_MAX");
            ValidatePositiveInt(result, label, step, "BlockSize", oddOnly: true);
            ValidatePositiveInt(result, label, step, "KernelWidth", oddOnly: false);
            ValidatePositiveInt(result, label, step, "KernelHeight", oddOnly: false);
            ValidatePositiveInt(result, label, step, "Iterations", oddOnly: false);
            ValidatePositiveDouble(result, label, step, "PIXELPERMM");
        }

        private static void ValidateMinMax(VisionPipelineValidationResult result, string label, VisionPipelineStep step, string minKey, string maxKey)
        {
            if (!TryGetDouble(step, minKey, out double minimum) || !TryGetDouble(step, maxKey, out double maximum))
            {
                return;
            }

            if (minimum > maximum)
            {
                result.Errors.Add($"{label} '{step.Name}': {minKey} is greater than {maxKey}.");
            }
        }

        private static void ValidatePositiveInt(VisionPipelineValidationResult result, string label, VisionPipelineStep step, string key, bool oddOnly)
        {
            if (!TryGetInt(step, key, out int value))
            {
                return;
            }

            if (value <= 0)
            {
                result.Errors.Add($"{label} '{step.Name}': {key} must be greater than 0.");
            }
            else if (oddOnly && value % 2 == 0)
            {
                result.Warnings.Add($"{label} '{step.Name}': {key} should usually be odd for this OpenCV operation.");
            }
        }

        private static void ValidatePositiveDouble(VisionPipelineValidationResult result, string label, VisionPipelineStep step, string key)
        {
            if (TryGetDouble(step, key, out double value) && value <= 0)
            {
                result.Errors.Add($"{label} '{step.Name}': {key} must be greater than 0.");
            }
        }

        private static bool TryGetInt(VisionPipelineStep step, string key, out int value)
        {
            value = 0;
            return step.Parameters.TryGetValue(key, out string text)
                && int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        private static bool TryGetDouble(VisionPipelineStep step, string key, out double value)
        {
            value = 0;
            return step.Parameters.TryGetValue(key, out string text)
                && double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

    }
}
