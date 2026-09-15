using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineExpectedFailureEvaluation
    {
        public bool IsValid { get; set; }
        public bool IsLegacy { get; set; }
        public bool IsQualityNg { get; set; }
        public bool IsControlledNoResult { get; set; }
        public string Classification { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public string ValidationStrength => IsLegacy ? "Legacy" : "Strict";
    }

    internal static class VisionPipelineExpectedFailureContract
    {
        internal const string QualityNgOutcome = "QualityNG";
        internal const string ControlledNoResultOutcome = "ControlledNoResult";

        private static readonly HashSet<string> ControlledNoResultErrors = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "BlobNoResult",
            "ContourNoResult",
            "FeatureNoKeypoints",
            "FeatureNotEnoughMatches",
            "FeatureHomographyFailed",
            "FeatureNoResult",
            "LineGaugeEdgeNotFound",
            "LineGaugeFitFailed",
            "MatchingNoResult"
        };

        public static bool HasExplicitDefinition(VisionPipelineSampleCatalogItem sample)
        {
            return sample != null
                && (!string.IsNullOrWhiteSpace(sample.ExpectedOutcome)
                    || !string.IsNullOrWhiteSpace(sample.ExpectedError)
                    || !string.IsNullOrWhiteSpace(sample.ExpectedFailedStep));
        }

        public static VisionPipelineExpectedFailureEvaluation Evaluate(
            VisionPipelineSampleCatalogItem sample,
            VisionRecipeRunResult result)
        {
            VisionPipelineExpectedFailureEvaluation evaluation = new VisionPipelineExpectedFailureEvaluation
            {
                IsLegacy = !HasExplicitDefinition(sample)
            };

            if (sample == null || !sample.ExpectsFailure)
            {
                evaluation.Message = "ExpectedFailure contract requires an ExpectedFailure catalog row.";
                return evaluation;
            }

            string expectedOutcome = NormalizeOutcome(sample.ExpectedOutcome);
            bool hasExpectedError = !string.IsNullOrWhiteSpace(sample.ExpectedError);
            bool hasExpectedStep = !string.IsNullOrWhiteSpace(sample.ExpectedFailedStep);
            if (string.IsNullOrWhiteSpace(expectedOutcome) && (hasExpectedError || hasExpectedStep))
            {
                evaluation.Message = "ExpectedOutcome is required when ExpectedError or ExpectedFailedStep is specified.";
                return evaluation;
            }

            if (!string.IsNullOrWhiteSpace(sample.ExpectedOutcome)
                && string.IsNullOrWhiteSpace(expectedOutcome))
            {
                evaluation.Message = $"Unsupported ExpectedOutcome '{sample.ExpectedOutcome}'. Use QualityNG or ControlledNoResult.";
                return evaluation;
            }

            if (result == null || result.Success)
            {
                evaluation.Message = "Expected failure did not produce a failed pipeline outcome.";
                return evaluation;
            }

            VisionRecipeStepRunSummary failedStep = result.FirstFailedStep;
            evaluation.IsQualityNg = IsQualityNg(failedStep);
            evaluation.IsControlledNoResult = IsControlledNoResult(failedStep);

            if (string.IsNullOrWhiteSpace(expectedOutcome))
            {
                expectedOutcome = evaluation.IsQualityNg
                    ? QualityNgOutcome
                    : evaluation.IsControlledNoResult
                        ? ControlledNoResultOutcome
                        : string.Empty;
            }

            if (string.Equals(expectedOutcome, QualityNgOutcome, StringComparison.OrdinalIgnoreCase))
            {
                if (!evaluation.IsQualityNg)
                {
                    evaluation.Message = "ExpectedFailure requires a tool-successful step whose acceptance evaluation returned quality NG.";
                    return evaluation;
                }

                if (hasExpectedError && !IsNoneError(sample.ExpectedError))
                {
                    evaluation.Message = "QualityNG cannot declare a tool error in ExpectedError.";
                    return evaluation;
                }
            }
            else if (string.Equals(expectedOutcome, ControlledNoResultOutcome, StringComparison.OrdinalIgnoreCase))
            {
                if (!evaluation.IsControlledNoResult)
                {
                    evaluation.Message = "ControlledNoResult requires a supported controlled no-result error.";
                    return evaluation;
                }

                if (hasExpectedError
                    && !string.Equals(sample.ExpectedError?.Trim(), failedStep?.ErrorName?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    evaluation.Message = $"ExpectedError '{sample.ExpectedError}' does not match '{failedStep?.ErrorName}'.";
                    return evaluation;
                }
            }
            else
            {
                evaluation.Message = $"Unsupported ExpectedOutcome '{expectedOutcome}'. Use QualityNG or ControlledNoResult.";
                return evaluation;
            }

            if (hasExpectedStep && !MatchesFailedStep(sample.ExpectedFailedStep, failedStep))
            {
                evaluation.Message = $"ExpectedFailedStep '{sample.ExpectedFailedStep}' does not match the first failed step.";
                return evaluation;
            }

            evaluation.IsValid = true;
            evaluation.Classification = expectedOutcome;
            evaluation.Message = evaluation.IsLegacy
                ? $"Legacy ExpectedFailure accepted as {expectedOutcome}; add ExpectedOutcome/ExpectedError/ExpectedFailedStep for strict validation."
                : $"Strict ExpectedFailure accepted as {expectedOutcome}.";
            return evaluation;
        }

        internal static bool IsControlledNoResult(VisionRecipeStepRunSummary step)
        {
            return step != null
                && step.Executed
                && !step.Skipped
                && !step.ToolSuccess
                && ControlledNoResultErrors.Contains(step.ErrorName?.Trim() ?? string.Empty);
        }

        private static bool IsQualityNg(VisionRecipeStepRunSummary step)
        {
            return step != null
                && step.Executed
                && !step.Skipped
                && step.ToolSuccess
                && step.AcceptanceEvaluated
                && !step.AcceptancePassed
                && step.ErrorCode == 0
                && string.Equals(step.Status?.Trim(), "NG", StringComparison.OrdinalIgnoreCase);
        }

        private static bool MatchesFailedStep(string expectedStep, VisionRecipeStepRunSummary failedStep)
        {
            if (failedStep == null || string.IsNullOrWhiteSpace(expectedStep))
            {
                return false;
            }

            string candidate = expectedStep.Trim();
            return string.Equals(candidate, failedStep.Index.ToString(), StringComparison.OrdinalIgnoreCase)
                || string.Equals(candidate, failedStep.Name?.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNoneError(string value)
        {
            return string.Equals(value?.Trim(), "None", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value?.Trim(), "0", StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeOutcome(string value)
        {
            string normalized = (value ?? string.Empty).Trim().Replace(" ", string.Empty).Replace("_", string.Empty).Replace("-", string.Empty);
            if (string.Equals(normalized, "QualityNG", StringComparison.OrdinalIgnoreCase)
                || string.Equals(normalized, "NG", StringComparison.OrdinalIgnoreCase))
            {
                return QualityNgOutcome;
            }

            if (string.Equals(normalized, "ControlledNoResult", StringComparison.OrdinalIgnoreCase)
                || string.Equals(normalized, "NoResult", StringComparison.OrdinalIgnoreCase))
            {
                return ControlledNoResultOutcome;
            }

            return string.Empty;
        }
    }
}
