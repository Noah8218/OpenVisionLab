using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineStepResultSummary
    {
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ToolType { get; set; } = string.Empty;
        public string InputLayer { get; set; } = string.Empty;
        public string OutputLayer { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool Success { get; set; }
        public bool Skipped { get; set; }
        public bool HasResultImage { get; set; }
        public int ResultImageWidth { get; set; }
        public int ResultImageHeight { get; set; }
        public int OverlayCount { get; set; }
        public int MetricCount { get; set; }
        public int ParameterCount { get; set; }
        public double ElapsedMilliseconds { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ErrorCode { get; set; }
        public string ErrorName { get; set; } = string.Empty;
        public string ResultStatus { get; set; } = string.Empty;
        public bool IsToolError { get; set; }
        public bool IsAcceptanceNg { get; set; }
        public string AcceptanceMessage { get; set; } = string.Empty;
        public string MetricsText { get; set; } = string.Empty;
        public string DiagnosticHint { get; set; } = string.Empty;
        public string SuggestedFix { get; set; } = string.Empty;
        public Dictionary<string, double> Metrics { get; set; } = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        public IReadOnlyList<VisionPipelineObjectResult> ObjectResults { get; set; } = Array.Empty<VisionPipelineObjectResult>();
        public int ObjectResultCount => ObjectResults?.Count ?? 0;
        public IReadOnlyList<VisionPipelineInstanceResult> InstanceResults { get; set; } = Array.Empty<VisionPipelineInstanceResult>();
        public int InstanceResultCount => InstanceResults?.Count ?? 0;
        public IReadOnlyList<VisionPipelineGeometryFeatureResult> GeometryFeatures { get; set; } = Array.Empty<VisionPipelineGeometryFeatureResult>();
        public int GeometryFeatureCount => GeometryFeatures?.Count ?? 0;
        public VisionPipelineCircleEvidence CircleEvidence { get; set; }
        public EdgeBasedMatchingDiagnosticEvidence EdgeBasedMatchingDiagnostics { get; set; }
        public string ResultImageSizeText => HasResultImage
            ? $"{ResultImageWidth} x {ResultImageHeight}"
            : string.Empty;
    }

    internal static class VisionPipelineResultSummaryService
    {
        public static List<VisionPipelineStepResultSummary> CreateStepSummaries(VisionPipelineRunResult runResult)
        {
            List<VisionPipelineStepResult> results = runResult?.StepResults ?? new List<VisionPipelineStepResult>();
            return results
                .Select((result, index) => CreateStepSummary(index + 1, result))
                .ToList();
        }

        public static VisionPipelineStepResultSummary CreateStepSummary(int index, VisionPipelineStepResult stepResult)
        {
            VisionPipelineStep step = stepResult?.Step;
            VisionToolResult toolResult = stepResult?.ToolResult;
            string resolvedMessage = ResolveMessage(stepResult);
            return new VisionPipelineStepResultSummary
            {
                Index = index,
                Name = step?.Name ?? string.Empty,
                ToolType = step?.ToolType ?? string.Empty,
                InputLayer = step?.InputLayer ?? string.Empty,
                OutputLayer = step?.OutputLayer ?? string.Empty,
                Status = ResolveStatus(stepResult),
                Success = IsPassed(stepResult),
                Skipped = stepResult?.Skipped == true,
                HasResultImage = toolResult?.ResultImage != null && !toolResult.ResultImage.Empty(),
                ResultImageWidth = toolResult?.ResultImage != null && !toolResult.ResultImage.Empty() ? toolResult.ResultImage.Width : 0,
                ResultImageHeight = toolResult?.ResultImage != null && !toolResult.ResultImage.Empty() ? toolResult.ResultImage.Height : 0,
                OverlayCount = toolResult?.Overlays?.Count ?? 0,
                MetricCount = toolResult?.Metrics?.Count ?? 0,
                ParameterCount = step?.Parameters?.Count ?? 0,
                ElapsedMilliseconds = toolResult?.Elapsed.TotalMilliseconds ?? 0d,
                Message = resolvedMessage,
                ErrorCode = toolResult?.ErrorCodeValue ?? 0,
                ErrorName = toolResult?.ErrorName ?? VisionToolErrorCode.None.ToString(),
                ResultStatus = toolResult?.ResultStatusName ?? string.Empty,
                IsToolError = toolResult != null && !toolResult.Success,
                IsAcceptanceNg = toolResult != null && toolResult.Success && stepResult?.AcceptancePassed == false,
                AcceptanceMessage = stepResult?.AcceptanceMessage ?? string.Empty,
                MetricsText = VisionPipelineKnownMetrics.FormatMetrics(toolResult?.Metrics),
                DiagnosticHint = VisionPipelineStepDiagnosticService.ResolveDiagnosticHint(stepResult, resolvedMessage),
                SuggestedFix = VisionPipelineStepDiagnosticService.ResolveSuggestedFix(stepResult, resolvedMessage),
                Metrics = VisionPipelineKnownMetrics.OrderMetrics(toolResult?.Metrics)
                    .ToDictionary(metric => metric.Key, metric => metric.Value, StringComparer.OrdinalIgnoreCase),
                ObjectResults = VisionPipelineObjectResultStore.Get(toolResult).ToList(),
                InstanceResults = VisionPipelineInstanceResultStore.Get(toolResult).Select(item => item.Clone()).ToList(),
                GeometryFeatures = VisionPipelineGeometryFeatureStore.Get(toolResult).Select(item => item.Clone()).ToList(),
                CircleEvidence = VisionPipelineCircleEvidenceStore.Get(toolResult),
                EdgeBasedMatchingDiagnostics = toolResult?.EdgeBasedMatchingDiagnostics?.Clone()
            };
        }

        public static VisionPipelineStepResult FindFirstFailedStep(VisionPipelineRunResult runResult)
        {
            return runResult?.StepResults.FirstOrDefault(result => !IsPassed(result));
        }

        public static bool IsPassed(VisionPipelineStepResult stepResult)
        {
            if (stepResult == null)
            {
                return false;
            }

            if (stepResult.Skipped)
            {
                return true;
            }

            return stepResult.ToolResult != null
                && stepResult.ToolResult.Success
                && stepResult.AcceptancePassed;
        }

        public static string ResolveStatus(VisionPipelineStepResult stepResult)
        {
            if (stepResult == null)
            {
                return "NG";
            }

            if (stepResult.Skipped)
            {
                return "SKIP";
            }

            if (stepResult.ToolResult == null)
            {
                return "NG";
            }

            if (!stepResult.ToolResult.Success)
            {
                switch (stepResult.ToolResult.ErrorCode)
                {
                    case VisionToolErrorCode.StepTimeout:
                        return "TIMEOUT";
                    case VisionToolErrorCode.StepCanceled:
                        return "CANCEL";
                    default:
                        return "ERROR";
                }
            }

            return stepResult.AcceptancePassed ? "OK" : "NG";
        }

        public static string ResolveMessage(VisionPipelineStepResult stepResult)
        {
            if (stepResult == null)
            {
                return string.Empty;
            }

            string toolMessage = stepResult.ToolResult?.Message ?? string.Empty;
            string acceptanceMessage = stepResult.AcceptanceMessage ?? string.Empty;
            string noDetectionMessage = ResolveNoDetectionMessage(stepResult);

            if (stepResult.ToolResult != null && !stepResult.ToolResult.Success)
            {
                return string.IsNullOrWhiteSpace(toolMessage) ? acceptanceMessage : toolMessage;
            }

            if (stepResult.ToolResult != null
                && stepResult.ToolResult.Success
                && stepResult.AcceptancePassed == false)
            {
                string friendlyMessage = ResolveAcceptanceFailureMessage(stepResult, acceptanceMessage);
                return string.IsNullOrWhiteSpace(friendlyMessage) ? toolMessage : friendlyMessage;
            }

            if (!string.IsNullOrWhiteSpace(toolMessage))
            {
                return toolMessage;
            }

            if (!string.IsNullOrWhiteSpace(noDetectionMessage))
            {
                return noDetectionMessage;
            }

            return acceptanceMessage;
        }

        private static string ResolveAcceptanceFailureMessage(VisionPipelineStepResult stepResult, string fallbackMessage)
        {
            VisionPipelineStep step = stepResult?.Step;
            VisionToolResult toolResult = stepResult?.ToolResult;
            if (step == null)
            {
                return fallbackMessage ?? string.Empty;
            }

            List<string> parts = new List<string>();
            bool actualSuccess = toolResult != null && toolResult.Success;
            if (actualSuccess != step.ExpectedSuccess)
            {
                parts.Add($"Expected success {step.ExpectedSuccess}, actual {actualSuccess}");
            }

            if (step.MaxElapsedMilliseconds > 0
                && toolResult != null
                && toolResult.Elapsed.TotalMilliseconds > step.MaxElapsedMilliseconds)
            {
                parts.Add($"Elapsed {toolResult.Elapsed.TotalMilliseconds:0.0} ms exceeds {step.MaxElapsedMilliseconds:0.0} ms");
            }

            if (!string.IsNullOrWhiteSpace(step.RequiredMessageText))
            {
                string toolMessage = toolResult?.Message ?? string.Empty;
                if (toolMessage.IndexOf(step.RequiredMessageText, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    parts.Add($"Message missing '{step.RequiredMessageText}'");
                }
            }

            if (!string.IsNullOrWhiteSpace(step.AcceptanceMetricName)
                && (step.UseAcceptanceMetricMinimum || step.UseAcceptanceMetricMaximum))
            {
                string displayName = VisionPipelineKnownMetrics.GetDisplayName(step.AcceptanceMetricName);
                if (toolResult == null || !toolResult.Metrics.TryGetValue(step.AcceptanceMetricName, out double metricValue))
                {
                    parts.Add($"{displayName} was not produced");
                }
                else
                {
                    if (step.UseAcceptanceMetricMinimum && metricValue < step.AcceptanceMetricMinimum)
                    {
                        parts.Add($"{displayName}: {metricValue:0.###} is below target {step.AcceptanceMetricMinimum:0.###}");
                    }

                    if (step.UseAcceptanceMetricMaximum && metricValue > step.AcceptanceMetricMaximum)
                    {
                        parts.Add($"{displayName}: {metricValue:0.###} is above target {step.AcceptanceMetricMaximum:0.###}");
                    }
                }
            }

            string noDetectionMessage = ResolveNoDetectionMessage(stepResult);
            if (!string.IsNullOrWhiteSpace(noDetectionMessage))
            {
                parts.Add(noDetectionMessage);
            }

            return parts.Count == 0
                ? fallbackMessage ?? string.Empty
                : string.Join("; ", parts.Distinct());
        }

        private static string ResolveNoDetectionMessage(VisionPipelineStepResult stepResult)
        {
            VisionPipelineStep step = stepResult?.Step;
            VisionToolResult toolResult = stepResult?.ToolResult;
            if (step == null || toolResult?.Metrics == null)
            {
                return string.Empty;
            }

            switch (NormalizeToolType(step.ToolType))
            {
                case "blob":
                    if (!HasZeroMetric(toolResult, VisionPipelineKnownMetrics.ResultCount))
                    {
                        return string.Empty;
                    }

                    return "No blob was detected. Check threshold polarity, morphology, ROI, area filters, and bounding width/height filters.";
                case "contour":
                    if (!HasZeroMetric(toolResult, VisionPipelineKnownMetrics.ResultCount))
                    {
                        return string.Empty;
                    }

                    return "No contour was detected. Check threshold polarity, morphology, ROI, area filters, bounding width/height filters, and retrieval mode.";
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                    if (toolResult.ErrorCode == VisionToolErrorCode.MatchingAmbiguous)
                    {
                        return string.IsNullOrWhiteSpace(toolResult.Message)
                            ? "Edge based template matching rejected multiple plausible unique-match candidates."
                            : toolResult.Message;
                    }

                    if (!HasZeroMetric(toolResult, VisionPipelineKnownMetrics.ResultCount))
                    {
                        return string.Empty;
                    }

                    return "No edge based template match was detected. Check template edge contrast, ROI, Canny thresholds, search step, and score threshold.";                case "feature":
                case "featurematching":
                case "sift":
                    if (!HasZeroMetric(toolResult, VisionPipelineKnownMetrics.ResultCount))
                    {
                        return string.Empty;
                    }

                    return "No feature match was detected. Check template features, ROI, preprocessing, SCORE_MIN, and RANSAC settings.";
                case "line":
                case "linegauge":
                case "linedistance":
                case "linedistancegauge":
                    if (!HasZeroMetric(toolResult, VisionPipelineKnownMetrics.ResultCount)
                        && HasPositiveMetric(toolResult, VisionPipelineKnownMetrics.EdgePointCount))
                    {
                        return string.Empty;
                    }

                    return "No line edge was detected. Check ROI, projection direction, polarity, contrast, sampling step, and preprocessing.";
                case "matching":
                case "templatematching":
                    if (!HasZeroMetric(toolResult, VisionPipelineKnownMetrics.ResultCount))
                    {
                        return string.Empty;
                    }

                    return "No template match was detected. Check template image, ROI, preprocessing, score threshold, and angle/scale search settings.";
                default:
                    return string.Empty;
            }
        }

        private static bool HasZeroMetric(VisionToolResult toolResult, string metricName)
        {
            return toolResult?.Metrics != null
                && toolResult.Metrics.TryGetValue(metricName, out double value)
                && Math.Abs(value) <= 0.000001;
        }

        private static bool HasPositiveMetric(VisionToolResult toolResult, string metricName)
        {
            return toolResult?.Metrics != null
                && toolResult.Metrics.TryGetValue(metricName, out double value)
                && value > 0.000001;
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
