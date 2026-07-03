using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    public sealed class OpenVisionPipelineReviewGuideState
    {
        public OpenVisionPipelineReviewGuideState(
            string stageText,
            string currentStepText,
            string nextActionText,
            string resultDecisionText,
            string detailText = null,
            string pairReviewText = null,
            string checklistText = null)
        {
            StageText = stageText ?? string.Empty;
            CurrentStepText = currentStepText ?? string.Empty;
            NextActionText = nextActionText ?? string.Empty;
            ResultDecisionText = resultDecisionText ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            PairReviewText = pairReviewText ?? string.Empty;
            ChecklistText = checklistText ?? string.Empty;
        }

        public string StageText { get; }

        public string CurrentStepText { get; }

        public string NextActionText { get; }

        public string ResultDecisionText { get; }

        public string DetailText { get; }

        public string PairReviewText { get; }

        public string ChecklistText { get; }
    }

    internal static class OpenVisionPipelineReviewGuidePresenter
    {
        public static OpenVisionPipelineReviewGuideState CreateEmpty(string pipelineName)
        {
            string name = SafeText(pipelineName, T("Pipeline.Title", "Pipeline"));
            return new OpenVisionPipelineReviewGuideState(
                T("PipelineReview.Guide.EmptyStage", "No steps"),
                TF("PipelineReview.Guide.EmptyCurrentFormat", "{0} has no review steps", name),
                T("PipelineReview.Guide.EmptyNext", "Add a tool result to the pipeline"),
                T("PipelineReview.Guide.EmptyDecision", "No result to judge"),
                T("PipelineReview.Guide.EmptyDetail", "Add a tool result from a Tool View, then run review."),
                string.Empty,
                ResolveChecklistText());
        }

        public static OpenVisionPipelineReviewGuideState CreateRunning(
            int displayIndex,
            int stepCount,
            VisionPipelineStep step)
        {
            return new OpenVisionPipelineReviewGuideState(
                FormatStage(displayIndex, stepCount),
                FormatCurrentStep(displayIndex, step),
                T("PipelineReview.Guide.RunningNext", "Review is running"),
                T("PipelineReview.Guide.RunningDecision", "Waiting for step result"),
                T("PipelineReview.Guide.RunningDetail", "The selected step is waiting for the review result."),
                string.Empty,
                ResolveChecklistText());
        }

        public static OpenVisionPipelineReviewGuideState CreateSelected(
            int displayIndex,
            int stepCount,
            VisionPipelineStep step,
            string statusText,
            bool hasInputImage,
            bool hasOutputImage,
            VisionPipelineStepResultSummary summary,
            VisionPipelineValidationResult validationResult,
            string expectedInputLayer = null,
            bool isBranch = false,
            OpenVisionWorkspaceSamplePairDecisionGuide samplePairGuide = null)
        {
            return new OpenVisionPipelineReviewGuideState(
                FormatStage(displayIndex, stepCount),
                FormatCurrentStep(displayIndex, step),
                ResolveNextActionText(displayIndex, stepCount, step, hasInputImage, hasOutputImage, summary, validationResult),
                ResolveResultDecisionText(statusText, summary, validationResult),
                ResolveDetailText(displayIndex, stepCount, step, hasInputImage, hasOutputImage, summary, validationResult, expectedInputLayer, isBranch),
                ResolvePairReviewText(samplePairGuide),
                ResolveChecklistText(samplePairGuide));
        }

        public static OpenVisionPipelineReviewGuideState CreateValidationError(
            int displayIndex,
            int stepCount,
            VisionPipelineStep step)
        {
            return new OpenVisionPipelineReviewGuideState(
                FormatStage(displayIndex, stepCount),
                FormatCurrentStep(displayIndex, step),
                T("PipelineReview.Guide.ValidationNext", "Fix validation errors before review"),
                T("PipelineReview.Guide.ValidationDecision", "Pipeline cannot be judged yet"),
                T("PipelineReview.Guide.ValidationDetail", "Open the validation detail and fix the route or parameters first."),
                string.Empty,
                ResolveChecklistText());
        }

        private static string ResolveChecklistText(OpenVisionWorkspaceSamplePairDecisionGuide samplePairGuide = null)
        {
            if (samplePairGuide?.HasGuide == true)
            {
                string checklistText = SafeText(samplePairGuide.ChecklistText, string.Empty);
                if (!string.IsNullOrWhiteSpace(checklistText))
                {
                    return checklistText;
                }

                string nextActionText = SafeText(samplePairGuide.NextActionText, string.Empty);
                if (!string.IsNullOrWhiteSpace(nextActionText))
                {
                    return nextActionText;
                }

                string metricText = SafeText(samplePairGuide.MetricText, string.Empty);
                if (!string.IsNullOrWhiteSpace(metricText))
                {
                    return metricText;
                }
            }

            return T(
                "PipelineReview.Guide.ChecklistText",
                "Review habit: run Good first -> run Bad in the same PairGroup with the same pipeline -> compare output image, overlay, metrics, and log.");
        }

        private static string ResolvePairReviewText(OpenVisionWorkspaceSamplePairDecisionGuide samplePairGuide)
        {
            return samplePairGuide?.HasGuide == true
                ? SafeText(samplePairGuide.PairReviewText, string.Empty)
                : string.Empty;
        }

        private static string FormatStage(int displayIndex, int stepCount)
        {
            int normalizedIndex = Math.Max(0, displayIndex);
            int normalizedCount = Math.Max(0, stepCount);
            return normalizedCount <= 0
                ? T("PipelineReview.Guide.EmptyStage", "No steps")
                : TF("PipelineReview.Guide.StageFormat", "Step {0}/{1}", normalizedIndex, normalizedCount);
        }

        private static string FormatCurrentStep(int displayIndex, VisionPipelineStep step)
        {
            if (step == null)
            {
                return "-";
            }

            string name = SafeText(step.Name, SafeText(step.ToolType, "Tool"));
            string route = TF(
                "PipelineReview.Guide.RouteFormat",
                "{0} -> {1}",
                SafeText(step.InputLayer, "-"),
                SafeText(step.OutputLayer, "-"));
            return string.Format(CultureInfo.CurrentCulture, "{0:00} {1} / {2}", Math.Max(0, displayIndex), name, route);
        }

        private static string ResolveNextActionText(
            int displayIndex,
            int stepCount,
            VisionPipelineStep step,
            bool hasInputImage,
            bool hasOutputImage,
            VisionPipelineStepResultSummary summary,
            VisionPipelineValidationResult validationResult)
        {
            if (validationResult?.Errors.Count > 0)
            {
                return T("PipelineReview.Guide.ValidationNext", "Fix validation errors before review");
            }

            if (step?.Enabled == false)
            {
                return T("PipelineReview.Guide.DisabledNext", "Enable this step before judging it");
            }

            if (!hasInputImage)
            {
                return T("PipelineReview.Guide.InputMissingNext", "Connect or load the input layer");
            }

            if (summary == null)
            {
                return hasOutputImage
                    ? T("PipelineReview.Guide.ReadyNext", "Run Review to refresh the measured result")
                    : T("PipelineReview.Guide.BeforeRunNext", "Run Review to create the step output");
            }

            if (!summary.Success || summary.IsAcceptanceNg)
            {
                return T("PipelineReview.Guide.NgNext", "Adjust the tool parameters or route, then run review again");
            }

            return displayIndex >= stepCount
                ? T("PipelineReview.Guide.OkFinalNext", "Compare output, metrics, and the Good/Bad pair before accepting the pipeline")
                : T("PipelineReview.Guide.OkNext", "Check the output image, then continue to the next step");
        }

        private static string ResolveResultDecisionText(
            string statusText,
            VisionPipelineStepResultSummary summary,
            VisionPipelineValidationResult validationResult)
        {
            if (validationResult?.Errors.Count > 0)
            {
                return T("PipelineReview.Guide.ValidationDecision", "Pipeline cannot be judged yet");
            }

            if (summary == null)
            {
                return T("PipelineReview.Guide.NoRunDecision", "Not judged");
            }

            string status = SafeText(statusText, summary.Success ? "OK" : "NG");
            if (summary.Success && !summary.IsAcceptanceNg)
            {
                return TF("PipelineReview.Guide.OkDecisionFormat", "{0}: output accepted", status);
            }

            return TF("PipelineReview.Guide.NgDecisionFormat", "{0}: review required", status);
        }

        private static string ResolveDetailText(
            int displayIndex,
            int stepCount,
            VisionPipelineStep step,
            bool hasInputImage,
            bool hasOutputImage,
            VisionPipelineStepResultSummary summary,
            VisionPipelineValidationResult validationResult,
            string expectedInputLayer,
            bool isBranch)
        {
            List<string> parts = new List<string>();
            if (validationResult?.Errors.Count > 0)
            {
                return T("PipelineReview.Guide.ValidationDetail", "Open the validation detail and fix the route or parameters first.");
            }

            if (step?.Enabled == false)
            {
                return T("PipelineReview.Guide.DisabledDetail", "This step is skipped during review because it is disabled.");
            }

            if (isBranch)
            {
                parts.Add(TF(
                    "PipelineReview.Guide.BranchDetailFormat",
                    "Branch: this step reads {0} instead of previous output {1}.",
                    SafeText(step?.InputLayer, "-"),
                    SafeText(expectedInputLayer, "-")));
            }

            if (!hasInputImage)
            {
                parts.Add(T("PipelineReview.Guide.InputMissingDetail", "The selected input layer has no image, so this step cannot be judged yet."));
                return string.Join(" / ", parts);
            }

            if (summary == null)
            {
                parts.Add(hasOutputImage
                    ? T("PipelineReview.Guide.ReadyDetail", "An output image exists, but Run Review refreshes the measured result.")
                    : T("PipelineReview.Guide.BeforeRunDetail", "Run Review creates the output image and measured result for this step."));
                return string.Join(" / ", parts);
            }

            if (!summary.Success || summary.IsAcceptanceNg)
            {
                string reason = ResolveNgReasonText(step, summary);
                parts.Add(TF("PipelineReview.Guide.FailedDetailFormat", "NG reason: {0}", Truncate(reason, 120)));
                string fix = ResolveNgFixText(step, summary);
                if (!string.IsNullOrWhiteSpace(fix)
                    && !string.Equals(reason, fix, StringComparison.Ordinal))
                {
                    parts.Add(T("PipelineReview.Guide.FixDetailPrefix", "Check first") + ": " + Truncate(fix, 160));
                }

                return string.Join(" / ", parts);
            }

            if (displayIndex < stepCount)
            {
                parts.Add(TF("PipelineReview.Guide.OkContinueDetailFormat", "This step passed. Move to Step {0}/{1} and confirm the next output.", displayIndex + 1, stepCount));
            }
            else
            {
                parts.Add(T("PipelineReview.Guide.OkFinalDetail", "Final step passed. Compare the output image, metrics, and matching Good/Bad pair before accepting the pipeline."));
            }

            return string.Join(" / ", parts);
        }

        private static string ResolveNgReasonText(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            string acceptanceMetricReason = FormatAcceptanceMetricNgReason(step, summary);
            if (!string.IsNullOrWhiteSpace(acceptanceMetricReason))
            {
                return acceptanceMetricReason;
            }

            return FirstText(
                summary?.SuggestedFix,
                summary?.DiagnosticHint,
                summary?.AcceptanceMessage,
                summary?.Message,
                T("PipelineReview.Guide.NgDetail", "Review the selected step's result and adjust the route or parameters."));
        }

        private static string ResolveNgFixText(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            string toolType = SafeText(step?.ToolType, SafeText(step?.Name, string.Empty));
            if (toolType.IndexOf("Threshold", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return T("PipelineReview.Guide.ThresholdFix", "Check input layer, ROI, threshold mode, value/range, adaptive block size, and weight.");
            }

            if (toolType.IndexOf("Blob", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return T("PipelineReview.Guide.BlobFix", "Check input layer chaining, threshold polarity, morphology cleanup, ROI, area limits, and connectivity.");
            }

            if (toolType.IndexOf("Contour", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return T("PipelineReview.Guide.ContourFix", "Check input layer chaining, threshold polarity, morphology cleanup, ROI, and area limits.");
            }

            if (toolType.IndexOf("Line", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return T("PipelineReview.Guide.LineFix", "Check ROI placement, edge layer selection, polarity/contrast, sampling interval, projection direction, and Pixel/mm.");
            }

            if (toolType.IndexOf("Matching", StringComparison.OrdinalIgnoreCase) >= 0
                || toolType.IndexOf("Feature", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return T("PipelineReview.Guide.MatchingFix", "Check template image, input layer, ROI, score threshold, angle search, and scale search.");
            }

            return FirstText(
                summary?.AcceptanceMessage,
                T("PipelineReview.Guide.GenericFix", "Check the input layer, route, ROI, and parameters before changing acceptance limits."));
        }

        internal static string FormatAcceptanceMetricNgReason(VisionPipelineStep step, VisionPipelineStepResultSummary summary)
        {
            if (step == null
                || summary?.IsAcceptanceNg != true
                || summary.Metrics == null
                || string.IsNullOrWhiteSpace(step.AcceptanceMetricName)
                || !summary.Metrics.TryGetValue(step.AcceptanceMetricName, out double value))
            {
                return string.Empty;
            }

            string target = FormatAcceptanceTargetText(step);
            if (string.IsNullOrWhiteSpace(target))
            {
                return string.Empty;
            }

            return TF(
                "PipelineReview.Guide.AcceptanceMetricNgDetailFormat",
                "{0}: measured {1}, target {2}.",
                ResolveMetricDisplayName(step.AcceptanceMetricName),
                FormatMetricValue(value),
                target);
        }

        private static string FormatAcceptanceTargetText(VisionPipelineStep step)
        {
            if (step == null)
            {
                return string.Empty;
            }

            bool hasMinimum = step.UseAcceptanceMetricMinimum;
            bool hasMaximum = step.UseAcceptanceMetricMaximum;
            if (hasMinimum && hasMaximum)
            {
                if (Math.Abs(step.AcceptanceMetricMinimum - step.AcceptanceMetricMaximum) < 0.000001)
                {
                    return "= " + FormatMetricValue(step.AcceptanceMetricMinimum);
                }

                return FormatMetricValue(step.AcceptanceMetricMinimum) + " - " + FormatMetricValue(step.AcceptanceMetricMaximum);
            }

            if (hasMinimum)
            {
                return ">= " + FormatMetricValue(step.AcceptanceMetricMinimum);
            }

            return hasMaximum
                ? "<= " + FormatMetricValue(step.AcceptanceMetricMaximum)
                : string.Empty;
        }

        private static string ResolveMetricDisplayName(string metricName)
        {
            string name = SafeText(metricName, string.Empty);
            return string.IsNullOrWhiteSpace(name)
                ? "-"
                : T("PipelineReview.Metric." + name, VisionPipelineKnownMetrics.GetDisplayName(name));
        }

        private static string FormatMetricValue(double value)
        {
            return value.ToString("0.###", CultureInfo.CurrentCulture);
        }

        private static string FirstText(params string[] values)
        {
            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return string.Empty;
        }

        private static string Truncate(string value, int maxLength)
        {
            string text = SafeText(value, string.Empty);
            return text.Length <= maxLength ? text : text.Substring(0, Math.Max(0, maxLength - 3)) + "...";
        }

        private static string SafeText(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }

        private static string TF(string key, string fallbackFormat, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, T(key, fallbackFormat), args);
        }
    }
}
