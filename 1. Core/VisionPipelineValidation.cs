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
            "linedistance",
            "linedistancegauge",
            "lineintersection",
            "lineintersectiongauge",
            "matching",
            "templatematching",
            "edgebasedmatching",
            "edgebasedtemplatematching",
            "edgetemplatematching",
            "mean",
            "rotatescale",
            "rotateandscale",
            "feature",
            "featurematching",
            "sift",
            "arithmetic",
            "overlaymerge",
            "resultmerge",
            "mergeresult"
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
            List<VisionPipelineStep> enabledSteps = new List<VisionPipelineStep>();
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
                enabledSteps.Add(step);
                ValidateAcceptance(result, label, step);
                ValidateParameters(result, label, step);

                bool isMergeStep = VisionPipelineOverlayMergeService.IsMergeTool(step.ToolType);
                if (isMergeStep)
                {
                    ValidateOverlayMergeSources(result, label, step, availableLayers, stepNames);
                }

                if (!isMergeStep
                    && previousEnabledStep != null
                    && !string.IsNullOrWhiteSpace(previousEnabledStep.OutputLayer)
                    && !string.IsNullOrWhiteSpace(step.InputLayer)
                    && !VisionPipelineNormalizer.IsBranchInputAllowed(step)
                    && !string.Equals(step.InputLayer, previousEnabledStep.OutputLayer, StringComparison.OrdinalIgnoreCase))
                {
                    result.Warnings.Add(
                        $"{label} '{step.Name}': Review branch input. This step reads '{step.InputLayer}' while the previous step outputs '{previousEnabledStep.OutputLayer}'. Keep this only when the step should intentionally start from that layer.");
                }
                else if (!isMergeStep
                    && previousEnabledStep != null
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

                ValidateArithmeticInputLayerB(result, label, step, availableLayers);

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

            ValidateFinalReviewIntent(result, enabledSteps);

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
            ValidateGrayValueRange(result, label, step, "Threshold");
            ValidateGrayValueRange(result, label, step, "MaxValue");
            ValidateGrayValueRange(result, label, step, "RangeMin");
            ValidateGrayValueRange(result, label, step, "RangeMax");
            ValidateGrayValueRange(result, label, step, "CANNY_LOW");
            ValidateGrayValueRange(result, label, step, "CANNY_HIGH");
            ValidateMinMax(result, label, step, "CANNY_LOW", "CANNY_HIGH");            ValidateGrayValueRange(result, label, step, "CannyThresholdLow");
            ValidateGrayValueRange(result, label, step, "CannyThresholdHigh");
            ValidateMinMax(result, label, step, "CannyThresholdLow", "CannyThresholdHigh");
            ValidateMinMax(result, label, step, "FIND_ANGLE_MIN", "FIND_ANGLE_MAX");
            ValidateUnitInterval(result, label, step, "SCORE_MIN");
            ValidateUnitInterval(result, label, step, "GREEDINESS");
            ValidateUnitInterval(result, label, step, "HYBRID_VERIFY_IMAGE_WEIGHT");
            ValidatePositiveDouble(result, label, step, "MAGNIFIATION");
            ValidatePositiveDouble(result, label, step, "RANSAC_REPROJ_THRESHOLD");
            ValidatePositiveDouble(result, label, step, "COARSE_ANGLE_STEP");
            ValidatePositiveInt(result, label, step, "BlockSize", oddOnly: true);
            ValidatePositiveInt(result, label, step, "KernelWidth", oddOnly: false);
            ValidatePositiveInt(result, label, step, "KernelHeight", oddOnly: false);
            ValidatePositiveInt(result, label, step, "Iterations", oddOnly: false);
            ValidatePositiveInt(result, label, step, "MedianKernelSize", oddOnly: true);
            ValidatePositiveInt(result, label, step, "Diameter", oddOnly: false);
            ValidatePositiveInt(result, label, step, "SigmaColor", oddOnly: false);
            ValidatePositiveInt(result, label, step, "SigmaSpace", oddOnly: false);
            ValidatePositiveInt(result, label, step, "NUM_MATCH", oddOnly: false);
            ValidatePositiveInt(result, label, step, "SEARCH_STEP", oddOnly: false);
            ValidatePositiveInt(result, label, step, "MAX_TEMPLATE_POINTS", oddOnly: false);
            ValidateOddKernelInRange(result, label, step, "CANNY_APERTURE_SIZE", 3, 7);
            ValidateCannyApertureSize(result, label, step);
            ValidateDerivativePair(result, label, step, "SobelDegreeX", "SobelDegreeY");
            ValidateDerivativePair(result, label, step, "ScharrDegreeX", "ScharrDegreeY");
            ValidateOddKernelInRange(result, label, step, "SobelKernelSize", 1, 31);
            ValidatePositiveInt(result, label, step, "LaplacianKernelSize", oddOnly: true);
            ValidatePositiveDouble(result, label, step, "PIXELPERMM");
            ValidatePositiveDouble(result, label, step, "ScaleXPercent");
            ValidatePositiveDouble(result, label, step, "ScaleYPercent");
            ValidateArithmeticParameters(result, label, step);
        }

        private static void ValidateArithmeticInputLayerB(
            VisionPipelineValidationResult result,
            string label,
            VisionPipelineStep step,
            HashSet<string> availableLayers)
        {
            if (!VisionPipelineArithmeticStep.RequiresInputLayerB(step))
            {
                return;
            }

            string inputLayerB = VisionPipelineArithmeticStep.GetInputLayerB(step);
            if (string.IsNullOrWhiteSpace(inputLayerB))
            {
                result.Errors.Add($"{label} '{step.Name}': InputLayerB is required for Arithmetic operation '{ReadParameter(step, VisionPipelineArithmeticStep.ParameterOperation)}'.");
            }
            else if (!availableLayers.Contains(inputLayerB))
            {
                result.Errors.Add($"{label} '{step.Name}': input layer B '{inputLayerB}' does not exist before this step.");
            }
        }

        private static void ValidateArithmeticParameters(VisionPipelineValidationResult result, string label, VisionPipelineStep step)
        {
            if (!VisionPipelineArithmeticStep.IsArithmetic(step))
            {
                return;
            }

            string mode = ReadParameter(step, VisionPipelineArithmeticStep.ParameterMode, VisionPipelineArithmeticStep.ModeOperation);
            if (!string.Equals(mode, VisionPipelineArithmeticStep.ModeOperation, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(mode, VisionPipelineArithmeticStep.ModeOffset, StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add($"{label} '{step.Name}': ArithmeticMode expects Operation or Offset.");
            }

            string operation = ReadParameter(step, VisionPipelineArithmeticStep.ParameterOperation, "Bitwise_AND");
            string[] operations =
            {
                "Bitwise_AND",
                "Bitwise_OR",
                "Bitwise_XOR",
                "Bitwise_NOT",
                "ADD",
                "SUBTRACT",
                "MULTIPLY",
                "DIVIDE",
                "MAX",
                "MIN",
                "ABS",
                "ABSDIFF"
            };
            if (!operations.Any(item => string.Equals(item, operation, StringComparison.OrdinalIgnoreCase)))
            {
                result.Errors.Add($"{label} '{step.Name}': unsupported ArithmeticOperation '{operation}'.");
            }

            ValidateGrayValueRange(result, label, step, VisionPipelineArithmeticStep.ParameterGray);
            ValidateGrayValueRange(result, label, step, VisionPipelineArithmeticStep.ParameterB);
            ValidateGrayValueRange(result, label, step, VisionPipelineArithmeticStep.ParameterG);
            ValidateGrayValueRange(result, label, step, VisionPipelineArithmeticStep.ParameterR);
        }

        private static void ValidateOverlayMergeSources(
            VisionPipelineValidationResult result,
            string label,
            VisionPipelineStep step,
            HashSet<string> availableLayers,
            HashSet<string> knownStepNames)
        {
            List<string> sourceLayers = ReadListParameter(step, "SourceLayers").ToList();
            List<string> sourceSteps = ReadListParameter(step, "SourceSteps").ToList();
            if (sourceLayers.Count == 0 && sourceSteps.Count == 0)
            {
                result.Warnings.Add($"{label} '{step.Name}': OverlayMerge has no SourceLayers or SourceSteps filter. It will merge every previous overlay result, which may be broader than intended.");
            }

            foreach (string sourceLayer in sourceLayers)
            {
                if (!availableLayers.Contains(sourceLayer))
                {
                    result.Errors.Add($"{label} '{step.Name}': OverlayMerge SourceLayer '{sourceLayer}' does not exist before this step.");
                }
            }

            foreach (string sourceStep in sourceSteps)
            {
                if (!knownStepNames.Contains(sourceStep))
                {
                    result.Errors.Add($"{label} '{step.Name}': OverlayMerge SourceStep '{sourceStep}' does not exist before this step.");
                }
            }
        }

        private static void ValidateFinalReviewIntent(
            VisionPipelineValidationResult result,
            IReadOnlyList<VisionPipelineStep> enabledSteps)
        {
            if (enabledSteps == null || enabledSteps.Count == 0)
            {
                return;
            }

            List<VisionPipelineStep> reviewOutputSteps = enabledSteps
                .Where(step => IsOverlayReviewTool(step?.ToolType))
                .ToList();
            List<VisionPipelineStep> mergeSteps = enabledSteps
                .Where(step => VisionPipelineOverlayMergeService.IsMergeTool(step?.ToolType))
                .ToList();

            if (reviewOutputSteps.Count >= 2 && mergeSteps.Count == 0)
            {
                result.Warnings.Add("Pipeline review: multiple inspection result steps exist, but no OverlayMerge step is configured. Add a final OverlayMerge when the user should verify all detections in one review image.");
                return;
            }

            if (mergeSteps.Count == 0)
            {
                return;
            }

            VisionPipelineStep lastEnabledStep = enabledSteps[enabledSteps.Count - 1];
            if (!VisionPipelineOverlayMergeService.IsMergeTool(lastEnabledStep.ToolType))
            {
                result.Warnings.Add($"Pipeline review: OverlayMerge exists, but the final enabled step is '{lastEnabledStep.Name}'. Put the final OverlayMerge last when it is the user-facing review result.");
            }
        }

        private static bool IsOverlayReviewTool(string toolType)
        {
            string normalized = VisionPipelineNormalizer.NormalizeToolType(toolType);
            return normalized == "blob"
                || normalized == "contour"
                || normalized == "line"
                || normalized == "linegauge"
                || normalized == "matching"
                || normalized == "templatematching"
                || normalized == "edgebasedmatching"
                || normalized == "edgebasedtemplatematching"
                || normalized == "edgetemplatematching"
                || normalized == "feature"
                || normalized == "featurematching"
                || normalized == "sift";
        }

        private static void ValidateGrayValueRange(VisionPipelineValidationResult result, string label, VisionPipelineStep step, string key)
        {
            if (!TryGetDouble(step, key, out double value))
            {
                return;
            }

            if (value < 0 || value > 255)
            {
                result.Warnings.Add($"{label} '{step.Name}': {key} is usually expected to be in the 0..255 grayscale range. Current value is {value.ToString(CultureInfo.InvariantCulture)}.");
            }
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

        private static void ValidateCannyApertureSize(VisionPipelineValidationResult result, string label, VisionPipelineStep step)
        {
            if (!TryGetInt(step, "CannyApertureSize", out int value))
            {
                return;
            }

            if (value != 3 && value != 5 && value != 7)
            {
                result.Warnings.Add($"{label} '{step.Name}': CannyApertureSize should usually be 3, 5, or 7. The runtime will normalize unsupported values.");
            }
        }

        private static void ValidateDerivativePair(VisionPipelineValidationResult result, string label, VisionPipelineStep step, string xKey, string yKey)
        {
            if (!TryGetInt(step, xKey, out int x) || !TryGetInt(step, yKey, out int y))
            {
                return;
            }

            if (x == 0 && y == 0)
            {
                result.Errors.Add($"{label} '{step.Name}': {xKey} and {yKey} cannot both be 0.");
            }
        }

        private static void ValidateOddKernelInRange(VisionPipelineValidationResult result, string label, VisionPipelineStep step, string key, int minimum, int maximum)
        {
            if (!TryGetInt(step, key, out int value))
            {
                return;
            }

            if (value < minimum || value > maximum)
            {
                result.Warnings.Add($"{label} '{step.Name}': {key} should usually be between {minimum} and {maximum}. The runtime may clamp unsupported values.");
            }
            else if (value % 2 == 0)
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

        private static void ValidateUnitInterval(VisionPipelineValidationResult result, string label, VisionPipelineStep step, string key)
        {
            if (TryGetDouble(step, key, out double value) && (value < 0 || value > 1))
            {
                result.Errors.Add($"{label} '{step.Name}': {key} expects a 0..1 value, not a percentage.");
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

        private static string ReadParameter(VisionPipelineStep step, string key, string defaultValue = "")
        {
            if (step?.Parameters == null
                || string.IsNullOrWhiteSpace(key)
                || !step.Parameters.TryGetValue(key, out string text)
                || string.IsNullOrWhiteSpace(text))
            {
                return defaultValue ?? string.Empty;
            }

            return text;
        }

        private static IEnumerable<string> ReadListParameter(VisionPipelineStep step, string key)
        {
            if (step?.Parameters == null
                || !step.Parameters.TryGetValue(key, out string text)
                || string.IsNullOrWhiteSpace(text))
            {
                return Enumerable.Empty<string>();
            }

            return text
                .Split(new[] { ';', ',', '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item));
        }

    }
}
