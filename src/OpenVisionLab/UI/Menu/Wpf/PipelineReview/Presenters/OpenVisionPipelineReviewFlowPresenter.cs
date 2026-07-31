using Lib.OpenCV.Pipeline;
using OpenVisionLab.Pipeline.Controls;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal sealed class OpenVisionPipelineReviewFlowProjection
    {
        public string ExpectedInputLayer { get; init; }
        public string FlowSummaryText { get; init; }
        public string StatusText { get; init; }
        public PipelineFlowStepStatus Status { get; init; }
        public bool IsBranch { get; init; }
        public bool InputWillBeProduced { get; init; }
        public bool IsInputMissing { get; init; }
    }

    internal static class OpenVisionPipelineReviewFlowPresenter
    {
        public static IReadOnlyList<PipelineFlowStepItem> CreateItems(
            IReadOnlyList<VisionPipelineStep> steps,
            Func<string, bool> hasLayerImage,
            Func<VisionPipelineStep, VisionPipelineStepResultSummary> resolveSummary)
        {
            List<PipelineFlowStepItem> items = new List<PipelineFlowStepItem>();
            for (int index = 0; index < (steps?.Count ?? 0); index++)
            {
                VisionPipelineStep step = steps[index];
                if (step == null)
                {
                    continue;
                }

                bool hasInputImage = hasLayerImage?.Invoke(step.InputLayer) == true;
                bool hasOutputImage = hasLayerImage?.Invoke(step.OutputLayer) == true;
                OpenVisionPipelineReviewFlowProjection projection = CreateStepProjection(
                    steps,
                    index,
                    hasInputImage,
                    hasOutputImage,
                    resolveSummary?.Invoke(step));
                items.Add(new PipelineFlowStepItem
                {
                    Index = index,
                    Name = step.Name,
                    ToolType = step.ToolType,
                    InputLayer = step.InputLayer,
                    OutputLayer = step.OutputLayer,
                    ExpectedInputLayer = projection.ExpectedInputLayer,
                    FlowStateText = projection.FlowSummaryText,
                    IsBranch = projection.IsBranch,
                    IsEnabled = step.Enabled,
                    HasInputImage = hasInputImage,
                    IsInputMissing = projection.IsInputMissing,
                    HasOutputImage = hasOutputImage,
                    Status = projection.Status,
                    StatusText = projection.StatusText
                });
            }

            return items;
        }

        public static OpenVisionPipelineReviewFlowProjection CreateStepProjection(
            IReadOnlyList<VisionPipelineStep> steps,
            int index,
            bool hasInputImage,
            bool hasOutputImage,
            VisionPipelineStepResultSummary summary)
        {
            VisionPipelineStep step = steps != null && index >= 0 && index < steps.Count
                ? steps[index]
                : null;
            string expectedInputLayer = ResolveExpectedInputLayer(steps, index);
            bool isBranch = IsBranch(step, expectedInputLayer);
            bool inputWillBeProduced = HasEnabledProducerBefore(steps, index, step?.InputLayer);
            bool isInputMissing = step?.Enabled == true && !hasInputImage && !inputWillBeProduced;
            return new OpenVisionPipelineReviewFlowProjection
            {
                ExpectedInputLayer = expectedInputLayer,
                FlowSummaryText = ResolveFlowSummary(step, isBranch, expectedInputLayer, isInputMissing),
                StatusText = ResolveStatusText(step, hasOutputImage, summary, isInputMissing),
                Status = ResolveStatus(step, hasOutputImage, summary, isInputMissing),
                IsBranch = isBranch,
                InputWillBeProduced = inputWillBeProduced,
                IsInputMissing = isInputMissing
            };
        }

        private static string ResolveExpectedInputLayer(
            IReadOnlyList<VisionPipelineStep> steps,
            int index)
        {
            string previousEnabledOutput = null;
            for (int candidateIndex = 0;
                candidateIndex < index && candidateIndex < (steps?.Count ?? 0);
                candidateIndex++)
            {
                VisionPipelineStep previous = steps[candidateIndex];
                if (previous?.Enabled == true && !string.IsNullOrWhiteSpace(previous.OutputLayer))
                {
                    previousEnabledOutput = previous.OutputLayer.Trim();
                }
            }

            return previousEnabledOutput;
        }

        private static bool IsBranch(VisionPipelineStep step, string expectedInputLayer)
        {
            return step?.Enabled == true
                && !string.IsNullOrWhiteSpace(expectedInputLayer)
                && !string.Equals(
                    SafeText(step.InputLayer, string.Empty),
                    expectedInputLayer,
                    StringComparison.OrdinalIgnoreCase);
        }

        private static PipelineFlowStepStatus ResolveStatus(
            VisionPipelineStep step,
            bool hasOutputImage,
            VisionPipelineStepResultSummary summary,
            bool isInputMissing)
        {
            if (step != null && !step.Enabled)
            {
                return PipelineFlowStepStatus.Skipped;
            }

            if (summary != null)
            {
                return summary.Success && !summary.IsAcceptanceNg
                    ? PipelineFlowStepStatus.Passed
                    : PipelineFlowStepStatus.Failed;
            }

            if (isInputMissing)
            {
                return PipelineFlowStepStatus.MissingInput;
            }

            return hasOutputImage ? PipelineFlowStepStatus.Loaded : PipelineFlowStepStatus.Waiting;
        }

        private static string ResolveStatusText(
            VisionPipelineStep step,
            bool hasOutputImage,
            VisionPipelineStepResultSummary summary,
            bool isInputMissing)
        {
            if (step != null && !step.Enabled)
            {
                return "OFF";
            }

            if (summary != null)
            {
                return SafeText(summary.Status, "DONE");
            }

            if (isInputMissing)
            {
                return T("PipelineReview.Status.InputMissing", "Input missing");
            }

            return hasOutputImage ? "READY" : "WAIT";
        }

        private static bool HasEnabledProducerBefore(
            IReadOnlyList<VisionPipelineStep> steps,
            int stepIndex,
            string inputLayer)
        {
            if (steps == null || stepIndex <= 0 || string.IsNullOrWhiteSpace(inputLayer))
            {
                return false;
            }

            string normalizedInput = inputLayer.Trim();
            for (int index = 0; index < stepIndex && index < steps.Count; index++)
            {
                VisionPipelineStep candidate = steps[index];
                if (candidate?.Enabled == true
                    && string.Equals(
                        candidate.OutputLayer?.Trim(),
                        normalizedInput,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string ResolveFlowSummary(
            VisionPipelineStep step,
            bool isBranch,
            string expectedInputLayer,
            bool isInputMissing)
        {
            if (step == null)
            {
                return "-";
            }

            if (!step.Enabled)
            {
                return T("PipelineReview.Flow.DisabledStep", "Disabled step");
            }

            string inputLayer = SafeText(
                step.InputLayer,
                T("PipelineReview.Flow.UnknownInput", "Input?"));
            if (isInputMissing)
            {
                return TF(
                    "PipelineReview.Flow.MissingInputFormat",
                    "Missing input: {0}",
                    inputLayer);
            }

            if (string.IsNullOrWhiteSpace(expectedInputLayer))
            {
                return TF(
                    "PipelineReview.Flow.SourceImageFormat",
                    "Source image: {0}",
                    inputLayer);
            }

            if (isBranch)
            {
                return TF(
                    "PipelineReview.Flow.BranchInputFormat",
                    "Branch input: {0} instead of previous output {1}",
                    inputLayer,
                    expectedInputLayer);
            }

            return TF(
                "PipelineReview.Flow.PreviousOutputFormat",
                "Previous output: {0}",
                expectedInputLayer);
        }

        private static string SafeText(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value)
                || string.Equals(value, key, StringComparison.Ordinal)
                    ? fallbackText ?? string.Empty
                    : value;
        }

        private static string TF(string key, string fallbackFormat, params object[] args)
        {
            return string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                T(key, fallbackFormat),
                args);
        }
    }
}
