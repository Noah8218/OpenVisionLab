using Lib.OpenCV.Pipeline;
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
        public string MetricsText { get; set; } = string.Empty;
        public Dictionary<string, double> Metrics { get; set; } = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
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
                Message = ResolveMessage(stepResult),
                MetricsText = VisionPipelineKnownMetrics.FormatMetrics(toolResult?.Metrics),
                Metrics = VisionPipelineKnownMetrics.OrderMetrics(toolResult?.Metrics)
                    .ToDictionary(metric => metric.Key, metric => metric.Value, StringComparer.OrdinalIgnoreCase)
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
                string message = stepResult.ToolResult.Message ?? string.Empty;
                if (message.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "TIMEOUT";
                }

                if (message.IndexOf("cancel", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "CANCEL";
                }

                return "NG";
            }

            return stepResult.AcceptancePassed ? "OK" : "NG";
        }

        public static string ResolveMessage(VisionPipelineStepResult stepResult)
        {
            if (stepResult == null)
            {
                return string.Empty;
            }

            return string.IsNullOrWhiteSpace(stepResult.ToolResult?.Message)
                ? stepResult.AcceptanceMessage ?? string.Empty
                : stepResult.ToolResult.Message;
        }
    }
}
