using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal static class VisionPipelineStepDiagnosticService
    {
        public static string ResolveDiagnosticHint(VisionPipelineStepResult stepResult, string resolvedMessage)
        {
            VisionPipelineStep step = stepResult?.Step;
            VisionToolResult toolResult = stepResult?.ToolResult;
            if (stepResult == null)
            {
                return "Step result was not created. Check pipeline execution service and step construction.";
            }

            if (stepResult.Skipped)
            {
                return "Step is disabled and was skipped.";
            }

            if (toolResult == null)
            {
                return "Tool returned no result object. Check ToolType, tool factory mapping, and property conversion.";
            }

            if (!toolResult.Success)
            {
                return ResolveToolFailureHint(step, toolResult);
            }

            if (!stepResult.AcceptancePassed)
            {
                return ResolveAcceptanceHint(step, toolResult, resolvedMessage);
            }

            return string.Empty;
        }

        public static string ResolveSuggestedFix(VisionPipelineStepResult stepResult, string resolvedMessage)
        {
            VisionPipelineStep step = stepResult?.Step;
            VisionToolResult toolResult = stepResult?.ToolResult;
            if (stepResult == null)
            {
                return "Run the pipeline again and inspect whether the step exists in the parsed XML.";
            }

            if (stepResult.Skipped)
            {
                return "Enable the step if it is required for the recipe.";
            }

            if (toolResult == null)
            {
                return "Use a supported ToolType and verify that the tool can be created before running.";
            }

            if (!toolResult.Success)
            {
                return ResolveToolFailureFix(step, toolResult);
            }

            if (!stepResult.AcceptancePassed)
            {
                return ResolveAcceptanceFix(step, toolResult, resolvedMessage);
            }

            return string.Empty;
        }

        private static string ResolveToolFailureHint(VisionPipelineStep step, VisionToolResult toolResult)
        {
            switch (toolResult.ErrorCode)
            {
                case VisionToolErrorCode.InputLayerMissing:
                    return $"Input layer '{step?.InputLayer ?? "-"}' was not available when this step started.";
                case VisionToolErrorCode.InputImageInvalid:
                    return "Input image is missing or empty.";
                case VisionToolErrorCode.InvalidRoi:
                case VisionToolErrorCode.ContourRoiInvalid:
                case VisionToolErrorCode.BlobRoiInvalid:
                case VisionToolErrorCode.MatchingRoiInvalid:
                case VisionToolErrorCode.LineGaugeRoiInvalid:
                case VisionToolErrorCode.MeanRoiInvalid:
                case VisionToolErrorCode.FeatureRoiInvalid:
                    return "ROI is invalid, empty, or outside the input image.";
                case VisionToolErrorCode.ToolPropertyMissing:
                case VisionToolErrorCode.ToolFactoryFailed:
                    return "Tool configuration could not be created from the step parameters.";
                case VisionToolErrorCode.TemplateImageMissing:
                case VisionToolErrorCode.TemplateImageInvalid:
                case VisionToolErrorCode.MatchingTemplateMissing:
                case VisionToolErrorCode.MatchingTemplateInvalid:
                case VisionToolErrorCode.FeatureTemplateMissing:
                case VisionToolErrorCode.FeatureTemplateInvalid:
                    return "Template image is missing, empty, or incompatible with the input image.";
                case VisionToolErrorCode.StepTimeout:
                    return "Step exceeded the configured timeout.";
                case VisionToolErrorCode.StepCanceled:
                    return "Step execution was canceled.";
                case VisionToolErrorCode.ThresholdInvalidRange:
                    return "Threshold Range mode has an invalid min/max range.";
                case VisionToolErrorCode.ThresholdInvalidMaxValue:
                    return "Threshold max value is outside the valid binary output range.";
                case VisionToolErrorCode.ThresholdInvalidAdaptiveBlockSize:
                    return "Adaptive threshold block size is invalid.";
                case VisionToolErrorCode.MorphologyInvalidKernel:
                    return "Morphology kernel size is invalid.";
                case VisionToolErrorCode.MorphologyInvalidIterations:
                    return "Morphology iteration count is invalid.";
                case VisionToolErrorCode.FilterInvalidKernel:
                    return "Filter kernel size is invalid for the selected filter.";
                case VisionToolErrorCode.FilterInvalidSigma:
                    return "Filter sigma value is invalid.";
                case VisionToolErrorCode.EdgeDetectionInvalidThreshold:
                    return "Edge threshold values are invalid.";
                case VisionToolErrorCode.EdgeDetectionInvalidKernel:
                    return "Edge detection kernel size is invalid.";
                case VisionToolErrorCode.EdgeDetectionInvalidDerivative:
                    return "Sobel derivative settings are invalid.";
                case VisionToolErrorCode.ContourInvalidAreaRange:
                    return "Contour area range is invalid.";
                case VisionToolErrorCode.ContourInvalidAdaptiveBlockSize:
                    return "Contour adaptive threshold block size is invalid.";
                case VisionToolErrorCode.BlobInvalidAreaRange:
                    return "Blob area range is invalid.";
                case VisionToolErrorCode.BlobInvalidAdaptiveBlockSize:
                    return "Blob adaptive threshold block size is invalid.";
                case VisionToolErrorCode.MatchingInvalidScale:
                    return "Matching scale search range is invalid.";
                case VisionToolErrorCode.MatchingInvalidAngleStep:
                    return "Matching angle search step is invalid.";
                case VisionToolErrorCode.MatchingInvalidAdaptiveBlockSize:
                    return "Matching adaptive threshold block size is invalid.";
                case VisionToolErrorCode.LineGaugeInvalidSampling:
                    return "LineGauge sampling interval is invalid.";
                case VisionToolErrorCode.LineGaugeInvalidAdaptiveBlockSize:
                    return "LineGauge adaptive threshold block size is invalid.";
                case VisionToolErrorCode.MeanInvalidAdaptiveBlockSize:
                    return "Mean adaptive threshold block size is invalid.";
                case VisionToolErrorCode.FeatureInvalidAdaptiveBlockSize:
                    return "Feature matching adaptive threshold block size is invalid.";
                case VisionToolErrorCode.RotateScaleInvalidScale:
                    return "Rotate/Scale scale value is invalid.";
                case VisionToolErrorCode.ContourNoResult:
                case VisionToolErrorCode.BlobNoResult:
                case VisionToolErrorCode.MatchingNoResult:
                case VisionToolErrorCode.LineGaugeEdgeNotFound:
                case VisionToolErrorCode.LineGaugeFitFailed:
                case VisionToolErrorCode.FeatureNoKeypoints:
                case VisionToolErrorCode.FeatureNotEnoughMatches:
                case VisionToolErrorCode.FeatureHomographyFailed:
                case VisionToolErrorCode.FeatureNoResult:
                    return ResolveNoResultHint(step, toolResult);
                default:
                    return string.IsNullOrWhiteSpace(toolResult.Message)
                        ? "Tool execution failed before producing an accepted result."
                        : toolResult.Message;
            }
        }

        private static string ResolveToolFailureFix(VisionPipelineStep step, VisionToolResult toolResult)
        {
            switch (toolResult.ErrorCode)
            {
                case VisionToolErrorCode.InputLayerMissing:
                    return "Set InputLayer to Main or to the exact OutputLayer of a previous enabled step.";
                case VisionToolErrorCode.InputImageInvalid:
                    return "Load a source image or publish the expected input layer before running this recipe.";
                case VisionToolErrorCode.InvalidRoi:
                case VisionToolErrorCode.ContourRoiInvalid:
                case VisionToolErrorCode.BlobRoiInvalid:
                case VisionToolErrorCode.MatchingRoiInvalid:
                case VisionToolErrorCode.LineGaugeRoiInvalid:
                case VisionToolErrorCode.MeanRoiInvalid:
                case VisionToolErrorCode.FeatureRoiInvalid:
                    return "Clamp ROI to the image bounds, or disable ROI for the first preview pass.";
                case VisionToolErrorCode.ToolPropertyMissing:
                case VisionToolErrorCode.ToolFactoryFailed:
                    return "Use a supported ToolType and parameter names from the OpenVisionLab pipeline schema.";
                case VisionToolErrorCode.TemplateImageMissing:
                case VisionToolErrorCode.TemplateImageInvalid:
                case VisionToolErrorCode.MatchingTemplateMissing:
                case VisionToolErrorCode.MatchingTemplateInvalid:
                    return "Attach a valid template image and verify template size, preprocessing, ROI, and score threshold.";
                case VisionToolErrorCode.FeatureTemplateMissing:
                case VisionToolErrorCode.FeatureTemplateInvalid:
                    return "Attach a feature-rich template image and lower the first-pass feature score gate if needed.";
                case VisionToolErrorCode.StepTimeout:
                    return "Reduce ROI/search range or increase MaxElapsedMilliseconds after confirming the recipe is correct.";
                case VisionToolErrorCode.StepCanceled:
                    return "Run the step again after cancellation is cleared, or check the caller cancellation token.";
                case VisionToolErrorCode.ThresholdInvalidRange:
                    return "Set RangeMin less than or equal to RangeMax, or switch to normal Threshold mode.";
                case VisionToolErrorCode.ThresholdInvalidMaxValue:
                    return "Set MaxValue to 1..255 for binary threshold output.";
                case VisionToolErrorCode.ThresholdInvalidAdaptiveBlockSize:
                    return "Set adaptive BlockSize to an odd value greater than 1, such as 3, 5, 11, or 25.";
                case VisionToolErrorCode.MorphologyInvalidKernel:
                    return "Set KernelWidth and KernelHeight to positive odd values and verify the selected operator.";
                case VisionToolErrorCode.MorphologyInvalidIterations:
                    return "Set Iterations to 1 or greater.";
                case VisionToolErrorCode.FilterInvalidKernel:
                    return "Set the filter kernel to a supported odd size for the selected filter.";
                case VisionToolErrorCode.FilterInvalidSigma:
                    return "Set Sigma to 0 or a positive value appropriate for the blur strength.";
                case VisionToolErrorCode.EdgeDetectionInvalidThreshold:
                    return "Set edge thresholds so the low/high values are valid for the selected edge mode.";
                case VisionToolErrorCode.EdgeDetectionInvalidKernel:
                    return "Set the edge kernel to a supported odd size, usually 3, 5, or 7.";
                case VisionToolErrorCode.EdgeDetectionInvalidDerivative:
                    return "For Sobel, set at least one derivative order to a non-zero value.";
                case VisionToolErrorCode.ContourInvalidAreaRange:
                    return "Set MIN_AREA less than or equal to MAX_AREA and keep both values within the expected object size.";
                case VisionToolErrorCode.ContourInvalidAdaptiveBlockSize:
                    return "Set Contour adaptive BlockSize to an odd value greater than 1.";
                case VisionToolErrorCode.BlobInvalidAreaRange:
                    return "Set Blob MIN_AREA less than or equal to MAX_AREA and match the expected object size.";
                case VisionToolErrorCode.BlobInvalidAdaptiveBlockSize:
                    return "Set Blob adaptive BlockSize to an odd value greater than 1.";
                case VisionToolErrorCode.MatchingInvalidScale:
                    return "Set matching scale min/max/step to a positive ordered range.";
                case VisionToolErrorCode.MatchingInvalidAngleStep:
                    return "Set angle search step to a positive value and keep min/max angle ordered.";
                case VisionToolErrorCode.MatchingInvalidAdaptiveBlockSize:
                    return "Set Matching adaptive BlockSize to an odd value greater than 1.";
                case VisionToolErrorCode.LineGaugeInvalidSampling:
                    return "Set LineGauge sampling interval and edge count settings to positive values.";
                case VisionToolErrorCode.LineGaugeInvalidAdaptiveBlockSize:
                    return "Set LineGauge adaptive BlockSize to an odd value greater than 1.";
                case VisionToolErrorCode.MeanInvalidAdaptiveBlockSize:
                    return "Set Mean adaptive BlockSize to an odd value greater than 1.";
                case VisionToolErrorCode.FeatureInvalidAdaptiveBlockSize:
                    return "Set Feature adaptive BlockSize to an odd value greater than 1.";
                case VisionToolErrorCode.RotateScaleInvalidScale:
                    return "Set scale to a positive value. Use 1.0 for original size.";
                case VisionToolErrorCode.ContourNoResult:
                case VisionToolErrorCode.BlobNoResult:
                case VisionToolErrorCode.MatchingNoResult:
                case VisionToolErrorCode.LineGaugeEdgeNotFound:
                case VisionToolErrorCode.LineGaugeFitFailed:
                case VisionToolErrorCode.FeatureNoKeypoints:
                case VisionToolErrorCode.FeatureNotEnoughMatches:
                case VisionToolErrorCode.FeatureHomographyFailed:
                case VisionToolErrorCode.FeatureNoResult:
                    return ResolveNoResultFix(step, toolResult);
                default:
                    return ResolveToolSpecificFix(step);
            }
        }

        private static string ResolveAcceptanceHint(VisionPipelineStep step, VisionToolResult toolResult, string resolvedMessage)
        {
            if (step == null)
            {
                return "Tool executed, but the acceptance rule failed.";
            }

            if (step.MaxElapsedMilliseconds > 0
                && toolResult != null
                && toolResult.Elapsed.TotalMilliseconds > step.MaxElapsedMilliseconds)
            {
                return $"Tool executed, but elapsed time {toolResult.Elapsed.TotalMilliseconds:0.0} ms exceeded MaxElapsedMilliseconds {step.MaxElapsedMilliseconds:0.0} ms.";
            }

            if (!string.IsNullOrWhiteSpace(step.AcceptanceMetricName)
                && (step.UseAcceptanceMetricMinimum || step.UseAcceptanceMetricMaximum))
            {
                string metric = VisionPipelineKnownMetrics.GetDisplayName(step.AcceptanceMetricName);
                string valueText = FormatMetricValue(toolResult, step.AcceptanceMetricName);
                string rangeText = FormatAcceptanceMetricRange(step);
                return $"Tool executed, but acceptance metric '{metric}' value {valueText} is outside target {rangeText}.";
            }

            return string.IsNullOrWhiteSpace(resolvedMessage)
                ? "Tool executed, but ExpectedSuccess, message, elapsed, or acceptance rule failed."
                : resolvedMessage;
        }

        private static string ResolveAcceptanceFix(VisionPipelineStep step, VisionToolResult toolResult, string resolvedMessage)
        {
            if (step == null)
            {
                return "Review the failed acceptance rule and compare it with produced metrics.";
            }

            if (step.MaxElapsedMilliseconds > 0
                && toolResult != null
                && toolResult.Elapsed.TotalMilliseconds > step.MaxElapsedMilliseconds)
            {
                return "Reduce ROI/search range, optimize preprocessing, or increase MaxElapsedMilliseconds after confirming the result is correct.";
            }

            if (!string.IsNullOrWhiteSpace(step.AcceptanceMetricName))
            {
                string metric = VisionPipelineKnownMetrics.GetDisplayName(step.AcceptanceMetricName);
                string valueText = FormatMetricValue(toolResult, step.AcceptanceMetricName);
                string rangeText = FormatAcceptanceMetricRange(step);
                string tuningGuidance = ResolveAcceptanceMetricTuningGuidance(step, step.AcceptanceMetricName);
                return $"Review '{metric}' value {valueText} against target {rangeText}. {tuningGuidance} Only change acceptance min/max after confirming the measured result is valid.";
            }

            return "Adjust ExpectedSuccess, RequiredMessageText, MaxElapsedMilliseconds, or use a metric-based acceptance rule.";
        }

        private static string ResolveNoResultHint(VisionPipelineStep step, VisionToolResult toolResult)
        {
            switch (NormalizeToolType(step?.ToolType))
            {
                case "contour":
                    return $"Contour produced no accepted objects from input layer '{ResolveInputLayerText(step)}'.{FormatDiagnosticMetricSuffix(step, toolResult)}";
                case "blob":
                    return $"Blob labeling produced no accepted objects from input layer '{ResolveInputLayerText(step)}'.{FormatDiagnosticMetricSuffix(step, toolResult)}";
                case "line":
                case "linegauge":
                case "linedistance":
                case "linedistancegauge":
                    return $"LineGauge did not find enough edge points or could not fit a line from input layer '{ResolveInputLayerText(step)}'.{FormatDiagnosticMetricSuffix(step, toolResult)}";
                case "matching":
                case "templatematching":
                    return $"Template matching did not find a match above the score threshold from input layer '{ResolveInputLayerText(step)}'.{FormatDiagnosticMetricSuffix(step, toolResult)}";
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                    return $"Edge based template matching did not find a match above the score threshold from input layer '{ResolveInputLayerText(step)}'.{FormatDiagnosticMetricSuffix(step, toolResult)}";
                case "feature":
                case "featurematching":
                case "sift":
                    return $"Feature matching did not find enough stable keypoints/matches from input layer '{ResolveInputLayerText(step)}'.{FormatDiagnosticMetricSuffix(step, toolResult)}";
                default:
                    return string.IsNullOrWhiteSpace(toolResult?.Message)
                        ? "Tool produced no accepted result."
                        : toolResult.Message;
            }
        }

        private static string ResolveNoResultFix(VisionPipelineStep step, VisionToolResult toolResult)
        {
            switch (NormalizeToolType(step?.ToolType))
            {
                case "contour":
                    return "Check whether InputLayer should be the previous preprocessing output, then tune threshold polarity, morphology, ROI, MIN_AREA, MAX_AREA, and DetectMode.";
                case "blob":
                    return "Check whether InputLayer should be the previous preprocessing output, then tune threshold polarity, morphology, ROI, MIN_AREA, MAX_AREA, and blob labeling options.";
                case "line":
                case "linegauge":
                case "linedistance":
                case "linedistancegauge":
                    return "Check whether InputLayer is the edge/preprocessed layer expected by LineGauge, then tune ROI, projection direction, polarity, contrast, sampling interval, and threshold.";
                case "matching":
                case "templatematching":
                    return "Check template image and whether InputLayer is the intended source/preprocessed layer, then tune ROI, score threshold, angle search, and scale search.";
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                    return "Check template edge contrast and whether InputLayer is the intended source/preprocessed layer, then tune ROI, Canny thresholds, score threshold, search step, and max template points.";
                case "feature":
                case "featurematching":
                case "sift":
                    return "Check template features and whether InputLayer is the intended source/preprocessed layer, then tune ROI, SCORE_MIN, match count, and homography/RANSAC settings.";
                default:
                    return ResolveToolSpecificFix(step);
            }
        }

        private static string ResolveToolSpecificFix(VisionPipelineStep step)
        {
            switch (NormalizeToolType(step?.ToolType))
            {
                case "threshold":
                    return "Adjust threshold mode, value/range, adaptive block size, adaptive weight, and output polarity.";
                case "morphology":
                    return "Adjust operator, kernel shape/size, iterations, and confirm the input layer is binary when expected.";
                case "filter":
                    return "Adjust filter type, kernel size, and sigma while checking that the output still preserves target contrast.";
                case "edgedetection":
                    return "Adjust edge thresholds, kernel, derivative order, and confirm the input has enough contrast.";
                case "rotatescale":
                    return "Adjust scale and angle while confirming the output layer remains the expected size/content.";
                case "mean":
                    return "Check ROI, threshold options, and acceptance metric range.";
                default:
                    return "Inspect the failed step parameters and preserve the successful previous layer chain.";
            }
        }

        private static string ResolveAcceptanceMetricTuningGuidance(VisionPipelineStep step, string metricName)
        {
            string normalizedTool = NormalizeToolType(step?.ToolType);
            string normalizedMetric = (metricName ?? string.Empty).Trim().ToLowerInvariant();
            switch (normalizedTool)
            {
                case "contour":
                    return "For contour count/size metrics, first check InputLayer chaining, threshold polarity/value, morphology cleanup, ROI, MIN_AREA, and MAX_AREA.";
                case "blob":
                    return "For blob count/size metrics, first check InputLayer chaining, threshold polarity/value, morphology cleanup, ROI, MIN_AREA, MAX_AREA, and labeling connectivity.";
                case "line":
                case "linegauge":
                case "linedistance":
                case "linedistancegauge":
                    return "For line metrics, first check ROI placement, edge layer selection, polarity/contrast, sampling interval, projection direction, and Pixel/mm calibration.";
                case "matching":
                case "templatematching":
                    return normalizedMetric.Contains("score")
                        ? "For matching score metrics, first check template crop, ROI, preprocessing mode, SCORE_MIN, angle search, and scale search."
                        : "For matching count/geometry metrics, first check template crop, ROI, preprocessing mode, score gate, angle search, and scale search.";
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                    return normalizedMetric.Contains("score")
                        ? "For edge matching score metrics, first check template edge quality, ROI, Canny thresholds, SCORE_MIN, search step, and max template points."
                        : "For edge matching count/geometry metrics, first check template edge quality, ROI, score gate, search step, and candidate suppression.";
                case "feature":
                case "featurematching":
                case "sift":
                    return normalizedMetric.Contains("score")
                        ? "For feature score metrics, first check template feature richness, ROI, SCORE_MIN, match count, homography, and RANSAC settings."
                        : "For feature count/geometry metrics, first check template feature richness, ROI, match count, homography, and RANSAC settings.";
                case "mean":
                    return "For mean/brightness metrics, first check ROI, lighting/reference image, threshold options, and whether the metric should be measured on Main or a processed layer.";
                case "threshold":
                    return "For threshold image metrics, first check mode, threshold value/range, adaptive block size, adaptive weight, and output polarity.";
                case "edgedetection":
                case "edge":
                    return "For edge metrics, first check input contrast, low/high thresholds, derivative mode, kernel size, and preprocessing filter.";
                default:
                    return "Tune the inspection parameters first while preserving the intended InputLayer/OutputLayer chain.";
            }
        }

        private static string ResolveInputLayerText(VisionPipelineStep step)
        {
            return string.IsNullOrWhiteSpace(step?.InputLayer)
                ? "-"
                : step.InputLayer.Trim();
        }

        private static string FormatMetricValue(VisionToolResult toolResult, string metricName)
        {
            if (toolResult?.Metrics == null || string.IsNullOrWhiteSpace(metricName))
            {
                return "missing";
            }

            return toolResult.Metrics.TryGetValue(metricName, out double value)
                ? value.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)
                : "missing";
        }

        private static string FormatDiagnosticMetricSuffix(VisionPipelineStep step, VisionToolResult toolResult)
        {
            if (toolResult?.Metrics == null || toolResult.Metrics.Count == 0)
            {
                return string.Empty;
            }

            string[] metricNames = GetDiagnosticMetricNames(step);
            List<string> parts = new List<string>();
            foreach (string metricName in metricNames)
            {
                if (toolResult.Metrics.TryGetValue(metricName, out double value))
                {
                    parts.Add(string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "{0}={1:0.###}",
                        VisionPipelineKnownMetrics.GetDisplayName(metricName),
                        value));
                }
            }

            if (parts.Count == 0)
            {
                return string.Empty;
            }

            return " Metrics: " + string.Join(", ", parts) + ".";
        }

        private static string[] GetDiagnosticMetricNames(VisionPipelineStep step)
        {
            switch (NormalizeToolType(step?.ToolType))
            {
                case "contour":
                case "blob":
                    return new[]
                    {
                        VisionPipelineKnownMetrics.ResultCount,
                        VisionPipelineKnownMetrics.AreaMin,
                        VisionPipelineKnownMetrics.AreaMax,
                        VisionPipelineKnownMetrics.BoundsWidthMax,
                        VisionPipelineKnownMetrics.BoundsHeightMax
                    };
                case "line":
                case "linegauge":
                case "linedistance":
                case "linedistancegauge":
                    return new[]
                    {
                        VisionPipelineKnownMetrics.EdgeCount,
                        VisionPipelineKnownMetrics.EdgePointCount,
                        VisionPipelineKnownMetrics.DistanceMmAvg,
                        VisionPipelineKnownMetrics.DistancePxAvg,
                        VisionPipelineKnownMetrics.LineLengthMax
                    };
                case "matching":
                case "templatematching":
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                case "feature":
                case "featurematching":
                case "sift":
                    return new[]
                    {
                        VisionPipelineKnownMetrics.ResultCount,
                        VisionPipelineKnownMetrics.ScoreMax,
                        VisionPipelineKnownMetrics.ScoreAvg,
                        VisionPipelineKnownMetrics.BoundsWidthMax,
                        VisionPipelineKnownMetrics.BoundsHeightMax
                    };
                default:
                    return new[] { VisionPipelineKnownMetrics.ResultCount };
            }
        }

        private static string FormatAcceptanceMetricRange(VisionPipelineStep step)
        {
            if (step == null)
            {
                return "-";
            }

            if (step.UseAcceptanceMetricMinimum && step.UseAcceptanceMetricMaximum)
            {
                if (Math.Abs(step.AcceptanceMetricMinimum - step.AcceptanceMetricMaximum) < 0.000001)
                {
                    return step.AcceptanceMetricMinimum.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
                }

                return string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "{0:0.###}..{1:0.###}",
                    step.AcceptanceMetricMinimum,
                    step.AcceptanceMetricMaximum);
            }

            if (step.UseAcceptanceMetricMinimum)
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture, ">= {0:0.###}", step.AcceptanceMetricMinimum);
            }

            if (step.UseAcceptanceMetricMaximum)
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture, "<= {0:0.###}", step.AcceptanceMetricMaximum);
            }

            return "-";
        }

        private static string NormalizeToolType(string toolType)
        {
            string value = (toolType ?? string.Empty).Trim();
            if (value.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 4);
            }

            return value.Replace(" ", string.Empty).Replace("_", string.Empty).ToLowerInvariant();
        }
    }
}
