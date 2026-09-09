using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    internal enum OpenVisionPipelineReviewResultStatusKind
    {
        NotRun,
        Draining,
        AlreadyRunning,
        NoSteps,
        ValidationErrors,
        Started,
        Superseded,
        Failed,
        Completed,
        ReferenceChanged,
        RunRequired
    }

    // Owns Pipeline Review run-level result/status text without owning document or view state.
    internal sealed class OpenVisionPipelineReviewResultStatusProjectionOwner
    {
        internal OpenVisionPipelineReviewResultStatusProjection Project(
            OpenVisionPipelineReviewResultStatusKind kind,
            int stepResultCount = 0,
            string errorMessage = null)
        {
            switch (kind)
            {
                case OpenVisionPipelineReviewResultStatusKind.Draining:
                    return StateOnly("PipelineReview.Execution.Draining", "종료 대기");
                case OpenVisionPipelineReviewResultStatusKind.AlreadyRunning:
                    return StateOnly("PipelineReview.Execution.AlreadyRunning", "Already running");
                case OpenVisionPipelineReviewResultStatusKind.NoSteps:
                    return StateOnly("PipelineReview.Execution.NoSteps", "No steps");
                case OpenVisionPipelineReviewResultStatusKind.ValidationErrors:
                    return Result(
                        "PipelineReview.Execution.ValidationErrors",
                        "Validation errors",
                        "PipelineReview.ValidationError",
                        "Validation error",
                        "PipelineReview.FixValidationErrors",
                        "Fix validation errors before running review.");
                case OpenVisionPipelineReviewResultStatusKind.Started:
                    return Result(
                        "PipelineReview.Execution.Started",
                        "Started",
                        "PipelineReview.RunningSummary",
                        "Running",
                        "PipelineReview.RunningDetail",
                        "Pipeline review execution in progress.");
                case OpenVisionPipelineReviewResultStatusKind.Superseded:
                    return StateOnly("PipelineReview.Execution.NotRun", "Not run");
                case OpenVisionPipelineReviewResultStatusKind.Failed:
                    string failureMessage = errorMessage ?? string.Empty;
                    return Result(
                        "PipelineReview.Execution.FailedFormat",
                        "Failed: {0}",
                        "PipelineReview.RunFailed",
                        "Run failed",
                        string.Empty,
                        failureMessage,
                        failureMessage);
                case OpenVisionPipelineReviewResultStatusKind.Completed:
                    return StateOnly(
                        "PipelineReview.Execution.CompletedFormat",
                        "Completed / {0} step results",
                        stepResultCount);
                case OpenVisionPipelineReviewResultStatusKind.ReferenceChanged:
                    return Result(
                        "PipelineReview.Execution.ReferenceChanged",
                        "Reference changed / run review required",
                        "PipelineReview.FixtureTeach.RunRequired",
                        "Reference saved",
                        "PipelineReview.FixtureTeach.RunRequiredDetail",
                        "The reference changed. Consumer ROI and routing were preserved; click Run Review to refresh every result.");
                case OpenVisionPipelineReviewResultStatusKind.RunRequired:
                    return Result(
                        "PipelineReview.Execution.NotRun",
                        "Not run",
                        "PipelineReview.RunRequired",
                        "Run review required",
                        "PipelineReview.RunRequiredDetail",
                        "Click Run Review to refresh step results.");
                default:
                    return StateOnly("PipelineReview.Execution.NotRun", "Not run");
            }
        }

        internal string ProjectProgress(
            IReadOnlyList<VisionPipelineStep> steps,
            Func<VisionPipelineStep, VisionPipelineStepResultSummary> resolveSummary,
            bool isRunning,
            bool isStopping)
        {
            if (steps == null || steps.Count == 0)
            {
                return T("PipelineReview.Progress.NoSteps", "No steps");
            }

            int okCount = 0;
            int ngCount = 0;
            int skippedCount = 0;
            foreach (VisionPipelineStep step in steps)
            {
                if (step?.Enabled == false)
                {
                    skippedCount++;
                    continue;
                }

                VisionPipelineStepResultSummary summary = resolveSummary?.Invoke(step);
                if (summary == null)
                {
                    continue;
                }

                if (summary.Success && !summary.IsAcceptanceNg)
                {
                    okCount++;
                }
                else
                {
                    ngCount++;
                }
            }

            int reviewableCount = 0;
            foreach (VisionPipelineStep step in steps)
            {
                if (step?.Enabled != false)
                {
                    reviewableCount++;
                }
            }

            int waitCount = Math.Max(0, reviewableCount - okCount - ngCount);
            if (okCount == 0 && ngCount == 0 && waitCount == reviewableCount && !isRunning)
            {
                return T("PipelineReview.Progress.NotRun", "Not run");
            }

            string progress = TF(
                "PipelineReview.Progress.CountsFormat",
                "OK {0} / NG {1} / WAIT {2}",
                okCount,
                ngCount,
                waitCount);
            if (skippedCount > 0)
            {
                progress = string.Format(
                    CultureInfo.CurrentCulture,
                    "{0} / {1}",
                    progress,
                    TF("PipelineReview.Progress.OffFormat", "OFF {0}", skippedCount));
            }

            return isStopping
                ? string.Format(
                    CultureInfo.CurrentCulture,
                    "{0} / {1}",
                    T("PipelineReview.Progress.Draining", "종료 대기"),
                    progress)
                : isRunning
                ? string.Format(
                    CultureInfo.CurrentCulture,
                    "{0} / {1}",
                    T("PipelineReview.Progress.Running", "Running..."),
                    progress)
                : progress;
        }

        internal string ProjectRunningProgressText()
        {
            return T("PipelineReview.Progress.Running", "Running...");
        }

        private static OpenVisionPipelineReviewResultStatusProjection StateOnly(
            string key,
            string fallback,
            params object[] args)
        {
            return new OpenVisionPipelineReviewResultStatusProjection(
                Format(key, fallback, args),
                string.Empty,
                string.Empty);
        }

        private static OpenVisionPipelineReviewResultStatusProjection Result(
            string stateKey,
            string stateFallback,
            string summaryKey,
            string summaryFallback,
            string detailKey,
            string detailFallback,
            params object[] args)
        {
            return new OpenVisionPipelineReviewResultStatusProjection(
                Format(stateKey, stateFallback, args),
                T(summaryKey, summaryFallback),
                T(detailKey, detailFallback));
        }

        private static string Format(
            string key,
            string fallback,
            params object[] args)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T(key, fallback),
                args ?? Array.Empty<object>());
        }

        private static string T(string key, string fallback)
        {
            string value = OpenVisionLanguageService.T(key);
            if (!string.IsNullOrWhiteSpace(value)
                && !string.Equals(value, key, StringComparison.Ordinal))
            {
                return value;
            }

            return fallback ?? string.Empty;
        }

        private static string TF(
            string key,
            string fallback,
            params object[] args)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T(key, fallback),
                args ?? Array.Empty<object>());
        }
    }

    internal sealed class OpenVisionPipelineReviewResultStatusProjection
    {
        internal OpenVisionPipelineReviewResultStatusProjection(
            string executionStateText,
            string resultSummaryText,
            string resultDetailText)
        {
            ExecutionStateText = executionStateText ?? string.Empty;
            ResultSummaryText = resultSummaryText ?? string.Empty;
            ResultDetailText = resultDetailText ?? string.Empty;
        }

        internal string ExecutionStateText { get; }

        internal string ResultSummaryText { get; }

        internal string ResultDetailText { get; }
    }
}
