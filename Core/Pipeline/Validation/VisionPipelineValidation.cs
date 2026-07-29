using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
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
            "pinarraygap",
            "adjacentpingap",
            "curvebandprofile",
            "darkbandcurve",
            "outercornerintersection",
            "brightobjectcorner",
            "lineintersection",
            "lineintersectiongauge",
            "circlegauge",
            "geometrymeasure",
            "geometricmeasurement",
            "linefixture",
            "dualedgefixture",
            "multimatchmean",
            "multifixturemean",
            "matching",
            "templatematching",
            "edgebasedmatching",
            "edgebasedtemplatematching",
            "edgetemplatematching",
            "mean",
            "hsv",
            "hsvmask",
            "colorhsv",
            "colormask",
            "rotatescale",
            "rotateandscale",
            "affine",
            "affinematrix",
            "affinetransform",
            "feature",
            "featurematching",
            "sift",
            "arithmetic",
            "referencedifference",
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
                ValidateGeometryMeasureSources(result, label, step, enabledSteps.Take(enabledSteps.Count - 1).ToList());
                ValidateLineFixtureSources(result, label, step, enabledSteps.Take(enabledSteps.Count - 1).ToList());
                ValidateMultiMatchMeanSource(result, label, step, enabledSteps.Take(enabledSteps.Count - 1).ToList());

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

            VisionPipelineFixtureFrameService.ValidatePipelineDefinition(
                pipeline,
                result.Errors,
                result.Warnings);
            VisionPipelineAffinePointBindingService.ValidatePipelineDefinition(
                pipeline,
                result.Errors);
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
            ValidateMinMax(result, label, step, "MIN_WIDTH", "MAX_WIDTH");
            ValidateMinMax(result, label, step, "MIN_HEIGHT", "MAX_HEIGHT");
            ValidateNonNegativeDouble(result, label, step, "MIN_WIDTH");
            ValidateNonNegativeDouble(result, label, step, "MAX_WIDTH");
            ValidateNonNegativeDouble(result, label, step, "MIN_HEIGHT");
            ValidateNonNegativeDouble(result, label, step, "MAX_HEIGHT");
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
            // Hue is circular in OpenCV's 0..179 HSV scale; HueMin may exceed HueMax to wrap through red.
            ValidateMinMax(result, label, step, "SaturationMin", "SaturationMax");
            ValidateMinMax(result, label, step, "ValueMin", "ValueMax");
            ValidateBoundedInt(result, label, step, "HueMin", 0, 179);
            ValidateBoundedInt(result, label, step, "HueMax", 0, 179);
            ValidateBoundedInt(result, label, step, "SaturationMin", 0, 255);
            ValidateBoundedInt(result, label, step, "SaturationMax", 0, 255);
            ValidateBoundedInt(result, label, step, "ValueMin", 0, 255);
            ValidateBoundedInt(result, label, step, "ValueMax", 0, 255);
            ValidateMinMax(result, label, step, "FIND_ANGLE_MIN", "FIND_ANGLE_MAX");
            ValidateMinMax(result, label, step, "FIND_SCALE_MIN", "FIND_SCALE_MAX");
            ValidateUnitInterval(result, label, step, "SCORE_MIN");
            ValidateUnitInterval(result, label, step, "GREEDINESS");
            ValidateUnitInterval(result, label, step, "HYBRID_VERIFY_IMAGE_WEIGHT");
            ValidateUnitInterval(result, label, step, nameof(EdgeBasedMatchingProperty.UNIQUE_MATCH_MIN_SCORE_MARGIN));
            ValidateBooleanWhenPresent(result, label, step, nameof(EdgeBasedMatchingProperty.ALLOW_GLOBAL_POLARITY_REVERSAL));
            ValidatePositiveDouble(result, label, step, "MAGNIFIATION");
            ValidatePositiveDouble(result, label, step, "RANSAC_REPROJ_THRESHOLD");
            ValidatePositiveDouble(result, label, step, "COARSE_ANGLE_STEP");
            ValidatePositiveDouble(result, label, step, "FIND_SCALE_MIN");
            ValidatePositiveDouble(result, label, step, "FIND_SCALE_MAX");
            ValidatePositiveDouble(result, label, step, "FIND_SCALE_STEP");
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
            ValidateNonNegativeDouble(result, label, step, "PIXELPERMM");
            ValidateGrayValueRange(result, label, step, "DarkThreshold");
            ValidateGrayValueRange(result, label, step, "ForegroundThreshold");
            ValidateUnitInterval(result, label, step, "MinDarkCoverageRatio");
            ValidatePositiveInt(result, label, step, "MinPinWidth", oddOnly: false);
            ValidateNonNegativeDouble(result, label, step, "MaxPinBreakWidth");
            ValidatePositiveInt(result, label, step, "MinGapWidth", oddOnly: false);
            ValidatePositiveInt(result, label, step, "MinComponentArea", oddOnly: false);
            ValidatePositiveInt(result, label, step, "MinComponentHeight", oddOnly: false);
            ValidateUnitInterval(result, label, step, "MinComponentHeightRatio");
            ValidateUnitInterval(result, label, step, "EdgeFitEndPercent");
            ValidateMetricCalibration(result, label, step);
            ValidatePositiveDouble(result, label, step, "ScaleXPercent");
            ValidatePositiveDouble(result, label, step, "ScaleYPercent");
            ValidateArithmeticParameters(result, label, step);
            ValidateReferenceDifferenceParameters(result, label, step);
            ValidateGapEdgePairParameters(result, label, step);
            ValidateGeometryParameters(result, label, step);
            ValidateAffineParameters(result, label, step);
            ValidateUniqueEdgeMatchContract(result, label, step);
        }

        private static void ValidateAffineParameters(
            VisionPipelineValidationResult result,
            string label,
            VisionPipelineStep step)
        {
            string toolType = VisionPipelineNormalizer.NormalizeToolType(step?.ToolType);
            if (toolType != "affine" && toolType != "affinematrix" && toolType != "affinetransform")
            {
                return;
            }

            ValidateBoundedInt(result, label, step, nameof(AffineTransformToolProperty.OutputWidth), 0, 32768);
            ValidateBoundedInt(result, label, step, nameof(AffineTransformToolProperty.OutputHeight), 0, 32768);
            ValidateNonNegativeDouble(result, label, step, nameof(AffineTransformToolProperty.MinimumSourceTriangleArea));
            ValidateNonNegativeDouble(result, label, step, nameof(AffineTransformToolProperty.MinimumDestinationTriangleArea));
            ValidateUnitInterval(result, label, step, nameof(AffineTransformToolProperty.MinimumValidPixelRatio));

            double sourceArea = TriangleArea(
                GetDoubleOrDefault(step, nameof(AffineTransformToolProperty.SourcePoint1X), 0d),
                GetDoubleOrDefault(step, nameof(AffineTransformToolProperty.SourcePoint1Y), 0d),
                GetDoubleOrDefault(step, nameof(AffineTransformToolProperty.SourcePoint2X), 100d),
                GetDoubleOrDefault(step, nameof(AffineTransformToolProperty.SourcePoint2Y), 0d),
                GetDoubleOrDefault(step, nameof(AffineTransformToolProperty.SourcePoint3X), 0d),
                GetDoubleOrDefault(step, nameof(AffineTransformToolProperty.SourcePoint3Y), 100d));
            double destinationArea = TriangleArea(
                GetDoubleOrDefault(step, nameof(AffineTransformToolProperty.DestinationPoint1X), 0d),
                GetDoubleOrDefault(step, nameof(AffineTransformToolProperty.DestinationPoint1Y), 0d),
                GetDoubleOrDefault(step, nameof(AffineTransformToolProperty.DestinationPoint2X), 100d),
                GetDoubleOrDefault(step, nameof(AffineTransformToolProperty.DestinationPoint2Y), 0d),
                GetDoubleOrDefault(step, nameof(AffineTransformToolProperty.DestinationPoint3X), 0d),
                GetDoubleOrDefault(step, nameof(AffineTransformToolProperty.DestinationPoint3Y), 100d));
            double minimumSourceArea = GetDoubleOrDefault(
                step,
                nameof(AffineTransformToolProperty.MinimumSourceTriangleArea),
                1d);
            double minimumDestinationArea = GetDoubleOrDefault(
                step,
                nameof(AffineTransformToolProperty.MinimumDestinationTriangleArea),
                1d);

            if (!VisionPipelineAffinePointBindingService.IsDetectedPointConsumer(step)
                && (sourceArea <= 1e-9d || sourceArea < minimumSourceArea))
            {
                result.Errors.Add(
                    $"{label} '{step.Name}': source point triangle area {sourceArea:0.######} is below MinimumSourceTriangleArea {minimumSourceArea:0.######}.");
            }

            if (destinationArea <= 1e-9d || destinationArea < minimumDestinationArea)
            {
                result.Errors.Add(
                    $"{label} '{step.Name}': destination point triangle area {destinationArea:0.######} is below MinimumDestinationTriangleArea {minimumDestinationArea:0.######}.");
            }
        }

        private static void ValidateGeometryParameters(
            VisionPipelineValidationResult result,
            string label,
            VisionPipelineStep step)
        {
            string toolType = VisionPipelineNormalizer.NormalizeToolType(step?.ToolType);
            if (toolType == "geometrymeasure" || toolType == "geometricmeasurement")
            {
                ValidateNonNegativeDouble(result, label, step, VisionPipelineGeometryMeasureService.MaximumParallelAngleDeltaParameter);
                ValidateNonNegativeDouble(result, label, step, VisionPipelineGeometryMeasureService.MaximumExtensionAParameter);
                ValidateNonNegativeDouble(result, label, step, VisionPipelineGeometryMeasureService.MaximumExtensionBParameter);
            }

            if (toolType == "linefixture" || toolType == "dualedgefixture")
            {
                ValidatePositiveInt(result, label, step, VisionPipelineLineFixtureService.MinimumSupportAParameter, oddOnly: false);
                ValidatePositiveInt(result, label, step, VisionPipelineLineFixtureService.MinimumSupportBParameter, oddOnly: false);
                ValidateNonNegativeDouble(result, label, step, VisionPipelineLineFixtureService.MaximumFitResidualAParameter);
                ValidateNonNegativeDouble(result, label, step, VisionPipelineLineFixtureService.MaximumFitResidualBParameter);
                ValidatePositiveDouble(result, label, step, VisionPipelineLineFixtureService.MinimumIncludedAngleParameter);
                ValidatePositiveDouble(result, label, step, VisionPipelineLineFixtureService.MaximumIncludedAngleParameter);
                ValidateMinMax(result, label, step, VisionPipelineLineFixtureService.MinimumIncludedAngleParameter, VisionPipelineLineFixtureService.MaximumIncludedAngleParameter);
                ValidateNonNegativeDouble(result, label, step, VisionPipelineLineFixtureService.MaximumExtensionAParameter);
                ValidateNonNegativeDouble(result, label, step, VisionPipelineLineFixtureService.MaximumExtensionBParameter);

                double maximumIncludedAngle = GetDoubleOrDefault(
                    step,
                    VisionPipelineLineFixtureService.MaximumIncludedAngleParameter,
                    90d);
                if (maximumIncludedAngle > 90d)
                {
                    result.Errors.Add($"{label} '{step.Name}': {VisionPipelineLineFixtureService.MaximumIncludedAngleParameter} must be 90 or less.");
                }
            }

            if (toolType == "multimatchmean" || toolType == "multifixturemean")
            {
                ValidatePositiveInt(result, label, step, VisionPipelineMultiMatchMeanService.MinimumInstancesParameter, oddOnly: false);
                ValidatePositiveInt(result, label, step, VisionPipelineMultiMatchMeanService.MaximumInstancesParameter, oddOnly: false);
                ValidatePositiveInt(result, label, step, VisionPipelineMultiMatchMeanService.MinimumPassCountParameter, oddOnly: false);
                ValidateNonNegativeDouble(result, label, step, VisionPipelineMultiMatchMeanService.RowToleranceParameter);
                ValidateUnitInterval(result, label, step, VisionPipelineMultiMatchMeanService.MaximumOverlapParameter);
                ValidateNonNegativeDouble(result, label, step, VisionPipelineMultiMatchMeanService.MinimumMeanParameter);
                ValidateNonNegativeDouble(result, label, step, VisionPipelineMultiMatchMeanService.MaximumMeanParameter);
                ValidateMinMax(result, label, step, VisionPipelineMultiMatchMeanService.MinimumMeanParameter, VisionPipelineMultiMatchMeanService.MaximumMeanParameter);
                ValidateNonNegativeDouble(result, label, step, VisionPipelineMultiMatchMeanService.MaximumAngleDeltaParameter);
                ValidatePositiveDouble(result, label, step, VisionPipelineMultiMatchMeanService.MinimumScaleRatioParameter);
                ValidatePositiveDouble(result, label, step, VisionPipelineMultiMatchMeanService.MaximumScaleRatioParameter);
                ValidateMinMax(result, label, step, VisionPipelineMultiMatchMeanService.MinimumScaleRatioParameter, VisionPipelineMultiMatchMeanService.MaximumScaleRatioParameter);
                ValidateUnitInterval(result, label, step, VisionPipelineMultiMatchMeanService.MinimumValidPixelRatioParameter);

                int minimumInstances = GetIntOrDefault(
                    step,
                    VisionPipelineMultiMatchMeanService.MinimumInstancesParameter,
                    1);
                int maximumInstances = GetIntOrDefault(
                    step,
                    VisionPipelineMultiMatchMeanService.MaximumInstancesParameter,
                    8);
                int minimumPassCount = GetIntOrDefault(
                    step,
                    VisionPipelineMultiMatchMeanService.MinimumPassCountParameter,
                    1);
                if (maximumInstances < minimumInstances || maximumInstances > 64)
                {
                    result.Errors.Add($"{label} '{step.Name}': instance limits must satisfy MIN_INSTANCES <= MAX_INSTANCES <= 64.");
                }
                if (minimumPassCount > maximumInstances)
                {
                    result.Errors.Add($"{label} '{step.Name}': MIN_PASS_COUNT cannot exceed MAX_INSTANCES.");
                }
                if (GetDoubleOrDefault(
                        step,
                        VisionPipelineMultiMatchMeanService.MaximumMeanParameter,
                        255D) > 255D)
                {
                    result.Errors.Add($"{label} '{step.Name}': MAX_MEAN must be 255 or less.");
                }
                if (!step.UseAcceptance
                    || !string.Equals(
                        step.AcceptanceMetricName,
                        VisionPipelineMultiMatchMeanService.InstanceAggregatePassedMetric,
                        StringComparison.OrdinalIgnoreCase)
                    || !step.UseAcceptanceMetricMinimum
                    || step.AcceptanceMetricMinimum != 1D
                    || !step.UseAcceptanceMetricMaximum
                    || step.AcceptanceMetricMaximum != 1D)
                {
                    result.Errors.Add(
                        $"{label} '{step.Name}': MultiMatchMean requires acceptance metric InstanceAggregatePassed with exact range 1..1.");
                }
            }

            if (toolType == "circlegauge")
            {
                if (!bool.TryParse(ReadParameter(step, "USE_ROI"), out bool useRoi) || !useRoi)
                {
                    result.Errors.Add($"{label} '{step.Name}': CircleGauge requires USE_ROI=true and one reviewed CvROI.");
                }
                if (string.IsNullOrWhiteSpace(ReadParameter(step, "CvROI")))
                {
                    result.Errors.Add($"{label} '{step.Name}': CircleGauge requires a non-empty CvROI.");
                }
                ValidatePositiveDouble(result, label, step, "RADIUS_MIN");
                ValidatePositiveDouble(result, label, step, "RADIUS_MAX");
                ValidateMinMax(result, label, step, "RADIUS_MIN", "RADIUS_MAX");
                ValidatePositiveDouble(result, label, step, "SWEEP_ANGLE_DEG");
                ValidatePositiveInt(result, label, step, "SCAN_COUNT", oddOnly: false);
                ValidateNonNegativeDouble(result, label, step, "MIN_CONTRAST");
                ValidateUnitInterval(result, label, step, "MIN_SUPPORT_RATIO");
                ValidateNonNegativeDouble(result, label, step, "MAX_FIT_RESIDUAL_PX");
            }
        }

        private static void ValidateGeometryMeasureSources(
            VisionPipelineValidationResult result,
            string label,
            VisionPipelineStep step,
            IReadOnlyList<VisionPipelineStep> earlierEnabledSteps)
        {
            if (!VisionPipelineGeometryMeasureService.IsGeometryMeasure(step?.ToolType))
            {
                return;
            }

            if (!Enum.TryParse(ReadParameter(step, VisionPipelineGeometryMeasureService.ModeParameter), true, out GeometryMeasurementMode mode))
            {
                result.Errors.Add($"{label} '{step.Name}': MeasurementMode must be one of {string.Join(", ", Enum.GetNames(typeof(GeometryMeasurementMode)))}.");
                return;
            }

            ValidateGeometrySource(result, label, step, earlierEnabledSteps, "A", mode);
            ValidateGeometrySource(result, label, step, earlierEnabledSteps, "B", mode);
        }

        private static void ValidateLineFixtureSources(
            VisionPipelineValidationResult result,
            string label,
            VisionPipelineStep step,
            IReadOnlyList<VisionPipelineStep> earlierEnabledSteps)
        {
            if (!VisionPipelineLineFixtureService.IsLineFixture(step?.ToolType))
            {
                return;
            }

            string sourceStepA = ReadParameter(step, VisionPipelineLineFixtureService.SourceStepAParameter);
            string sourceFeatureA = ReadParameter(step, VisionPipelineLineFixtureService.SourceFeatureAParameter);
            string sourceStepB = ReadParameter(step, VisionPipelineLineFixtureService.SourceStepBParameter);
            string sourceFeatureB = ReadParameter(step, VisionPipelineLineFixtureService.SourceFeatureBParameter);
            if (!string.IsNullOrWhiteSpace(sourceStepA)
                && !string.IsNullOrWhiteSpace(sourceFeatureA)
                && string.Equals(sourceStepA, sourceStepB, StringComparison.OrdinalIgnoreCase)
                && string.Equals(sourceFeatureA, sourceFeatureB, StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add($"{label} '{step.Name}': datum A and datum B must reference distinct Segment results.");
            }

            ValidateGeometrySource(
                result,
                label,
                step,
                earlierEnabledSteps,
                "A",
                GeometryMeasurementMode.LineLineIntersection);
            ValidateGeometrySource(
                result,
                label,
                step,
                earlierEnabledSteps,
                "B",
                GeometryMeasurementMode.LineLineIntersection);
            ValidateLineFixtureSourceTool(
                result,
                label,
                step,
                earlierEnabledSteps,
                sourceStepA);
            ValidateLineFixtureSourceTool(
                result,
                label,
                step,
                earlierEnabledSteps,
                sourceStepB);
        }

        private static void ValidateLineFixtureSourceTool(
            VisionPipelineValidationResult result,
            string label,
            VisionPipelineStep consumer,
            IReadOnlyList<VisionPipelineStep> earlierEnabledSteps,
            string sourceStepName)
        {
            if (string.IsNullOrWhiteSpace(sourceStepName))
            {
                return;
            }

            List<VisionPipelineStep> matches = (earlierEnabledSteps
                    ?? Array.Empty<VisionPipelineStep>())
                .Where(item => string.Equals(
                    item?.Name,
                    sourceStepName,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (matches.Count != 1)
            {
                return;
            }

            string sourceToolType = VisionPipelineNormalizer.NormalizeToolType(
                matches[0].ToolType);
            if (sourceToolType != "line" && sourceToolType != "linegauge")
            {
                result.Errors.Add(
                    $"{label} '{consumer.Name}': LineFixture source '{sourceStepName}' must use Line or LineGauge.");
            }
        }

        private static void ValidateMultiMatchMeanSource(
            VisionPipelineValidationResult result,
            string label,
            VisionPipelineStep consumer,
            IReadOnlyList<VisionPipelineStep> earlierEnabledSteps)
        {
            if (!VisionPipelineMultiMatchMeanService.IsMultiMatchMean(
                    consumer?.ToolType))
            {
                return;
            }

            string sourceStepName = ReadParameter(
                consumer,
                VisionPipelineMultiMatchMeanService.SourceStepParameter);
            if (string.IsNullOrWhiteSpace(sourceStepName))
            {
                result.Errors.Add(
                    $"{label} '{consumer.Name}': {VisionPipelineMultiMatchMeanService.SourceStepParameter} is required.");
                return;
            }

            List<VisionPipelineStep> matches = (earlierEnabledSteps
                    ?? Array.Empty<VisionPipelineStep>())
                .Where(item => string.Equals(
                    item?.Name,
                    sourceStepName,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (matches.Count != 1)
            {
                result.Errors.Add(matches.Count == 0
                    ? $"{label} '{consumer.Name}': multi-match source '{sourceStepName}' must be an earlier enabled Step."
                    : $"{label} '{consumer.Name}': multi-match source '{sourceStepName}' is ambiguous.");
                return;
            }

            VisionPipelineStep producer = matches[0];
            string type = VisionPipelineNormalizer.NormalizeToolType(
                producer.ToolType);
            if (type != "matching"
                && type != "templatematching"
                && type != "edgebasedmatching"
                && type != "edgebasedtemplatematching"
                && type != "edgetemplatematching")
            {
                result.Errors.Add(
                    $"{label} '{consumer.Name}': source '{sourceStepName}' must use Matching or EdgeBasedMatching.");
            }

            if (!string.Equals(
                    producer.InputLayer,
                    consumer.InputLayer,
                    StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add(
                    $"{label} '{consumer.Name}': source '{sourceStepName}' must use the same input coordinate layer '{consumer.InputLayer}'.");
            }

            if (!int.TryParse(
                    ReadParameter(producer, "NUM_MATCH"),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int requestedMatches)
                || requestedMatches < 2)
            {
                result.Errors.Add(
                    $"{label} '{consumer.Name}': source '{sourceStepName}' must request NUM_MATCH >= 2.");
            }

            if (string.IsNullOrWhiteSpace(ReadParameter(
                    consumer,
                    VisionPipelineMultiMatchMeanService.RelativeRoiParameter)))
            {
                result.Errors.Add(
                    $"{label} '{consumer.Name}': {VisionPipelineMultiMatchMeanService.RelativeRoiParameter} is required.");
            }
        }

        private static void ValidateGeometrySource(
            VisionPipelineValidationResult result,
            string label,
            VisionPipelineStep consumer,
            IReadOnlyList<VisionPipelineStep> earlierEnabledSteps,
            string role,
            GeometryMeasurementMode mode)
        {
            string sourceStepKey = role == "A" ? VisionPipelineGeometryMeasureService.SourceStepAParameter : VisionPipelineGeometryMeasureService.SourceStepBParameter;
            string sourceFeatureKey = role == "A" ? VisionPipelineGeometryMeasureService.SourceFeatureAParameter : VisionPipelineGeometryMeasureService.SourceFeatureBParameter;
            string sourceStepName = ReadParameter(consumer, sourceStepKey);
            string sourceFeatureName = ReadParameter(consumer, sourceFeatureKey);
            if (string.IsNullOrWhiteSpace(sourceStepName) || string.IsNullOrWhiteSpace(sourceFeatureName))
            {
                result.Errors.Add($"{label} '{consumer.Name}': {sourceStepKey} and {sourceFeatureKey} are required.");
                return;
            }

            List<VisionPipelineStep> matches = (earlierEnabledSteps ?? Array.Empty<VisionPipelineStep>())
                .Where(item => string.Equals(item?.Name, sourceStepName, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (matches.Count != 1)
            {
                result.Errors.Add(matches.Count == 0
                    ? $"{label} '{consumer.Name}': geometry source '{sourceStepName}' must be an earlier enabled Step."
                    : $"{label} '{consumer.Name}': geometry source '{sourceStepName}' is ambiguous ({matches.Count} earlier Steps).");
                return;
            }

            VisionPipelineStep producer = matches[0];
            if (!string.Equals(producer.InputLayer, consumer.InputLayer, StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add($"{label} '{consumer.Name}': source '{sourceStepName}' uses coordinate layer '{producer.InputLayer}', but the consumer input is '{consumer.InputLayer}'.");
            }

            if (!TryGetProducedGeometryKind(producer, sourceFeatureName, out VisionPipelineGeometryKind actualKind))
            {
                result.Errors.Add($"{label} '{consumer.Name}': source feature '{sourceStepName}/{sourceFeatureName}' is not a supported typed output of '{producer.ToolType}'.");
                return;
            }

            VisionPipelineGeometryKind requiredKind = RequiredKind(mode, role);
            if (actualKind != requiredKind)
            {
                result.Errors.Add($"{label} '{consumer.Name}': source {role} requires {requiredKind} for {mode}, but '{sourceStepName}/{sourceFeatureName}' is {actualKind}.");
            }
        }

        private static VisionPipelineGeometryKind RequiredKind(GeometryMeasurementMode mode, string role)
        {
            if (mode == GeometryMeasurementMode.PointPointDistance) return VisionPipelineGeometryKind.Point;
            if (mode == GeometryMeasurementMode.PointLineDistance) return role == "A" ? VisionPipelineGeometryKind.Point : VisionPipelineGeometryKind.Segment;
            if (mode == GeometryMeasurementMode.CircleSegmentClearance) return role == "A" ? VisionPipelineGeometryKind.Circle : VisionPipelineGeometryKind.Segment;
            return VisionPipelineGeometryKind.Segment;
        }

        private static bool TryGetProducedGeometryKind(
            VisionPipelineStep producer,
            string featureName,
            out VisionPipelineGeometryKind kind)
        {
            kind = VisionPipelineGeometryKind.Point;
            string type = VisionPipelineNormalizer.NormalizeToolType(producer?.ToolType);
            if (type == "line" || type == "linegauge")
            {
                if (string.Equals(featureName, "Segment", StringComparison.OrdinalIgnoreCase)) { kind = VisionPipelineGeometryKind.Segment; return true; }
                if (new[] { "Start", "End", "Midpoint" }.Any(name => string.Equals(featureName, name, StringComparison.OrdinalIgnoreCase))) { kind = VisionPipelineGeometryKind.Point; return true; }
                return false;
            }
            if (type == "circlegauge")
            {
                if (string.Equals(featureName, "Circle", StringComparison.OrdinalIgnoreCase)) { kind = VisionPipelineGeometryKind.Circle; return true; }
                if (string.Equals(featureName, "Center", StringComparison.OrdinalIgnoreCase)) { kind = VisionPipelineGeometryKind.Point; return true; }
                return false;
            }
            if ((type == "linefixture" || type == "dualedgefixture")
                && string.Equals(featureName, "Origin", StringComparison.OrdinalIgnoreCase))
            {
                kind = VisionPipelineGeometryKind.Point;
                return true;
            }
            if (type == "geometrymeasure" || type == "geometricmeasurement")
            {
                if (!Enum.TryParse(ReadParameter(producer, VisionPipelineGeometryMeasureService.ModeParameter), true, out GeometryMeasurementMode mode)) return false;
                if (mode == GeometryMeasurementMode.LineLineIntersection && string.Equals(featureName, "Intersection", StringComparison.OrdinalIgnoreCase)) { kind = VisionPipelineGeometryKind.Point; return true; }
                if (mode != GeometryMeasurementMode.LineLineAngle && new[] { "MeasureStart", "MeasureEnd" }.Any(name => string.Equals(featureName, name, StringComparison.OrdinalIgnoreCase))) { kind = VisionPipelineGeometryKind.Point; return true; }
            }
            return false;
        }

        private static void ValidateGapEdgePairParameters(
            VisionPipelineValidationResult result,
            string label,
            VisionPipelineStep step)
        {
            string toolType = VisionPipelineNormalizer.NormalizeToolType(step?.ToolType);
            if (toolType != "linedistance" && toolType != "linedistancegauge")
            {
                return;
            }

            if (!bool.TryParse(ReadParameter(step, VisionPipelineGapEdgePairTool.UseParameter), out bool enabled) || !enabled)
            {
                return;
            }

            if (!bool.TryParse(ReadParameter(step, "USE_ROI"), out bool useRoi) || !useRoi)
            {
                result.Errors.Add($"{label} '{step.Name}': Gap edge-pair mode requires USE_ROI=true and one reviewed CvROI.");
            }

            ValidateMinMax(result, label, step, VisionPipelineGapEdgePairTool.MinimumGapParameter, VisionPipelineGapEdgePairTool.MaximumGapParameter);
            ValidatePositiveDouble(result, label, step, VisionPipelineGapEdgePairTool.MinimumGapParameter);
            ValidatePositiveDouble(result, label, step, VisionPipelineGapEdgePairTool.MaximumGapParameter);
            ValidatePositiveDouble(result, label, step, VisionPipelineGapEdgePairTool.MaximumAngleParameter);
            ValidatePositiveDouble(result, label, step, VisionPipelineGapEdgePairTool.MaximumParallelDeltaParameter);
            ValidateUnitInterval(result, label, step, VisionPipelineGapEdgePairTool.MinimumSupportRatioParameter);
            ValidateNonNegativeDouble(result, label, step, VisionPipelineGapEdgePairTool.MinimumDarkContrastParameter);
            ValidateUnitInterval(result, label, step, VisionPipelineGapEdgePairTool.MinimumDarkCoverageParameter);
            ValidateNonNegativeDouble(result, label, step, VisionPipelineGapEdgePairTool.MinimumScoreMarginParameter);
        }

        private static void ValidateReferenceDifferenceParameters(
            VisionPipelineValidationResult result,
            string label,
            VisionPipelineStep step)
        {
            if (!string.Equals(
                    VisionPipelineNormalizer.NormalizeToolType(step?.ToolType),
                    "referencedifference",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            bool hasReferencePath = !string.IsNullOrWhiteSpace(ReadParameter(step, "ReferencePaths"))
                || Enumerable.Range(1, 4).Any(index => !string.IsNullOrWhiteSpace(
                    ReadParameter(step, "ReferencePath" + index.ToString(CultureInfo.InvariantCulture))));
            if (!hasReferencePath)
            {
                result.Errors.Add($"{label} '{step.Name}': at least ReferencePath1 is required for ReferenceDifference.");
            }

            ValidateGrayValueRange(result, label, step, "DifferenceThreshold");
            ValidateMinMax(result, label, step, "MinimumDefectArea", "MaximumDefectArea");
            ValidatePositiveInt(result, label, step, "MinimumDefectArea", oddOnly: false);
            ValidatePositiveInt(result, label, step, "MaximumDefectArea", oddOnly: false);
            ValidatePositiveInt(result, label, step, "MorphologyKernel", oddOnly: true);
            ValidatePositiveInt(result, label, step, "OrbFeatures", oddOnly: false);
            ValidatePositiveInt(result, label, step, "MinimumInliers", oddOnly: false);
            ValidateUnitInterval(result, label, step, "MatchRatio");
            ValidatePositiveDouble(result, label, step, "RansacThreshold");
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

            ValidateBoundedInt(
                result,
                label,
                step,
                VisionPipelineOverlayMergeService.LineWidthParameter,
                VisionPipelineOverlayMergeService.MinimumLineWidth,
                VisionPipelineOverlayMergeService.MaximumLineWidth);
            ValidateBoundedInt(
                result,
                label,
                step,
                VisionPipelineOverlayMergeService.PointSizeParameter,
                VisionPipelineOverlayMergeService.MinimumPointSize,
                VisionPipelineOverlayMergeService.MaximumPointSize);
            ValidateBoundedInt(
                result,
                label,
                step,
                VisionPipelineOverlayMergeService.LabelMarginParameter,
                VisionPipelineOverlayMergeService.MinimumLabelMargin,
                VisionPipelineOverlayMergeService.MaximumLabelMargin);
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
                || normalized == "pinarraygap"
                || normalized == "adjacentpingap"
                || normalized == "curvebandprofile"
                || normalized == "darkbandcurve"
                || normalized == "outercornerintersection"
                || normalized == "brightobjectcorner"
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

        private static void ValidateBoundedInt(VisionPipelineValidationResult result, string label, VisionPipelineStep step, string key, int minimum, int maximum)
        {
            if (!TryGetInt(step, key, out int value))
            {
                return;
            }

            if (value < minimum || value > maximum)
            {
                result.Errors.Add($"{label} '{step.Name}': {key} expects a value between {minimum} and {maximum}.");
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

        private static void ValidateNonNegativeDouble(VisionPipelineValidationResult result, string label, VisionPipelineStep step, string key)
        {
            if (TryGetDouble(step, key, out double value) && value < 0)
            {
                result.Errors.Add($"{label} '{step.Name}': {key} cannot be negative.");
            }
        }

        private static void ValidateMetricCalibration(VisionPipelineValidationResult result, string label, VisionPipelineStep step)
        {
            if (TryGetDouble(step, "PIXELPERMM", out double mmPerPixel)
                && mmPerPixel <= 0
                && (step.AcceptanceMetricName ?? string.Empty).IndexOf("Mm", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                result.Errors.Add($"{label} '{step.Name}': PIXELPERMM must be greater than 0 when an mm acceptance metric is used.");
            }
        }

        private static void ValidateUniqueEdgeMatchContract(
            VisionPipelineValidationResult result,
            string label,
            VisionPipelineStep step)
        {
            string toolType = VisionPipelineNormalizer.NormalizeToolType(step?.ToolType);
            bool isEdgeMatcher = toolType == "edgebasedmatching"
                || toolType == "edgebasedtemplatematching"
                || toolType == "edgetemplatematching";
            if (!isEdgeMatcher
                || !TryGetBool(step, nameof(EdgeBasedMatchingProperty.USE_UNIQUE_MATCH_VALIDATION), out bool enabled)
                || !enabled)
            {
                return;
            }

            if (TryGetInt(step, nameof(EdgeBasedMatchingProperty.NUM_MATCH), out int matchCount)
                && matchCount != 1)
            {
                result.Errors.Add(
                    $"{label} '{step.Name}': {nameof(EdgeBasedMatchingProperty.USE_UNIQUE_MATCH_VALIDATION)} requires NUM_MATCH=1.");
            }

            if (TryGetBool(step, "USE_MULTI_ROI", out bool useMultiRoi)
                && useMultiRoi)
            {
                result.Errors.Add(
                    $"{label} '{step.Name}': {nameof(EdgeBasedMatchingProperty.USE_UNIQUE_MATCH_VALIDATION)} requires USE_MULTI_ROI=false.");
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

        private static bool TryGetBool(VisionPipelineStep step, string key, out bool value)
        {
            value = false;
            return step?.Parameters != null
                && step.Parameters.TryGetValue(key, out string text)
                && bool.TryParse(text, out value);
        }

        private static void ValidateBooleanWhenPresent(
            VisionPipelineValidationResult result,
            string label,
            VisionPipelineStep step,
            string key)
        {
            if (step?.Parameters != null
                && step.Parameters.TryGetValue(key, out string text)
                && !bool.TryParse(text, out _))
            {
                result.Errors.Add($"{label} '{step.Name}': {key} must be true or false.");
            }
        }

        private static double GetDoubleOrDefault(VisionPipelineStep step, string key, double defaultValue)
        {
            return TryGetDouble(step, key, out double value) ? value : defaultValue;
        }

        private static int GetIntOrDefault(
            VisionPipelineStep step,
            string key,
            int defaultValue)
        {
            return TryGetInt(step, key, out int value) ? value : defaultValue;
        }

        private static double TriangleArea(
            double x1,
            double y1,
            double x2,
            double y2,
            double x3,
            double y3)
        {
            return Math.Abs(((x2 - x1) * (y3 - y1)) - ((y2 - y1) * (x3 - x1))) * 0.5d;
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
