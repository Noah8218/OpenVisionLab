using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal static class VisionPipelineBatchOutcomeContract
    {
        internal const int CurrentVersion = 1;
        internal const string CompletedState = "Completed";
        internal const string ErrorState = "Error";
        internal const string OkOutcome = "OK";
        internal const string NgOutcome = "NG";

        internal static void Apply(
            VisionPipelineBatchSampleRunResult result,
            bool executionCompleted,
            bool actualSuccess,
            bool hasJudgment,
            bool expectedSuccess,
            bool judgmentCorrect)
        {
            if (result == null)
            {
                return;
            }

            result.OutcomeSchemaVersion = CurrentVersion;
            result.ExecutionState = executionCompleted ? CompletedState : ErrorState;
            result.ActualOutcome = executionCompleted
                ? ToOutcome(actualSuccess)
                : string.Empty;
            result.HasJudgment = hasJudgment;
            result.ExpectedOutcome = hasJudgment
                ? ToOutcome(expectedSuccess)
                : string.Empty;
            result.JudgmentCorrect = hasJudgment && executionCompleted && judgmentCorrect;
        }

        internal static bool HasExplicitOutcome(VisionPipelineBatchSampleRunResult result)
        {
            return result?.OutcomeSchemaVersion == CurrentVersion;
        }

        internal static bool HasUnsupportedOutcome(VisionPipelineBatchSampleRunResult result)
        {
            return result?.OutcomeSchemaVersion > CurrentVersion;
        }

        internal static bool IsExecutionCompleted(VisionPipelineBatchSampleRunResult result)
        {
            if (result == null || HasUnsupportedOutcome(result))
            {
                return false;
            }

            return HasExplicitOutcome(result)
                ? string.Equals(result.ExecutionState, CompletedState, StringComparison.Ordinal)
                : true;
        }

        internal static bool TryResolveActualSuccess(
            VisionPipelineBatchSampleRunResult result,
            out bool actualSuccess)
        {
            if (HasUnsupportedOutcome(result))
            {
                actualSuccess = false;
                return false;
            }

            if (HasExplicitOutcome(result))
            {
                if (!IsExecutionCompleted(result))
                {
                    actualSuccess = false;
                    return false;
                }

                if (TryParseOutcome(result.ActualOutcome, out actualSuccess))
                {
                    return true;
                }

                actualSuccess = false;
                return false;
            }

            if (result != null)
            {
                actualSuccess = result.Success;
                return true;
            }

            actualSuccess = false;
            return false;
        }

        internal static bool TryResolveExpectedSuccess(
            VisionPipelineBatchSampleRunResult result,
            out bool expectedSuccess)
        {
            if (HasUnsupportedOutcome(result))
            {
                expectedSuccess = false;
                return false;
            }

            if (HasExplicitOutcome(result))
            {
                if (!result.HasJudgment)
                {
                    expectedSuccess = false;
                    return false;
                }

                return TryParseOutcome(result.ExpectedOutcome, out expectedSuccess);
            }

            string expected = result?.ExpectedText?.Trim() ?? string.Empty;
            if (!expected.StartsWith("ExpectedActual:", StringComparison.OrdinalIgnoreCase))
            {
                expectedSuccess = false;
                return false;
            }

            string role = result?.PairRole?.Trim();
            if (string.Equals(role, OkOutcome, StringComparison.OrdinalIgnoreCase))
            {
                expectedSuccess = true;
                return true;
            }

            if (string.Equals(role, NgOutcome, StringComparison.OrdinalIgnoreCase))
            {
                expectedSuccess = false;
                return true;
            }

            if (expected.EndsWith(OkOutcome, StringComparison.OrdinalIgnoreCase))
            {
                expectedSuccess = true;
                return true;
            }

            if (expected.EndsWith(NgOutcome, StringComparison.OrdinalIgnoreCase))
            {
                expectedSuccess = false;
                return true;
            }

            expectedSuccess = false;
            return false;
        }

        internal static bool ResolveJudgmentCorrect(VisionPipelineBatchSampleRunResult result)
        {
            if (HasExplicitOutcome(result))
            {
                return result.HasJudgment
                    && IsExecutionCompleted(result)
                    && result.JudgmentCorrect;
            }

            return TryResolveExpectedSuccess(result, out bool expectedSuccess)
                && TryResolveActualSuccess(result, out bool actualSuccess)
                && expectedSuccess == actualSuccess;
        }

        internal static string ResolveMisclassificationReason(
            VisionPipelineBatchSampleRunResult result)
        {
            if (!IsExecutionCompleted(result)
                || !TryResolveExpectedSuccess(result, out bool expectedSuccess)
                || !TryResolveActualSuccess(result, out bool actualSuccess)
                || ResolveJudgmentCorrect(result))
            {
                return string.Empty;
            }

            if (!expectedSuccess && actualSuccess)
            {
                return "false-accept";
            }

            if (expectedSuccess && !actualSuccess)
            {
                return "false-reject";
            }

            return string.Empty;
        }

        internal static VisionPipelineBatchConfusionMatrix BuildConfusionMatrix(
            IEnumerable<VisionPipelineBatchSampleRunResult> results,
            int inputSampleCount)
        {
            int observedCount = 0;
            int truePositiveCount = 0;
            int trueNegativeCount = 0;
            int falsePositiveCount = 0;
            int falseNegativeCount = 0;
            int executionErrorCount = 0;
            int unknownLabelCount = 0;

            foreach (VisionPipelineBatchSampleRunResult result in results ?? Array.Empty<VisionPipelineBatchSampleRunResult>())
            {
                observedCount++;
                if (result == null || !IsExecutionCompleted(result))
                {
                    executionErrorCount++;
                    continue;
                }

                if (!TryResolveExpectedSuccess(result, out bool expectedSuccess))
                {
                    unknownLabelCount++;
                    continue;
                }

                if (!TryResolveActualSuccess(result, out bool actualSuccess))
                {
                    executionErrorCount++;
                    continue;
                }

                if (expectedSuccess && actualSuccess)
                {
                    truePositiveCount++;
                }
                else if (!expectedSuccess && !actualSuccess)
                {
                    trueNegativeCount++;
                }
                else if (!expectedSuccess)
                {
                    falsePositiveCount++;
                }
                else
                {
                    falseNegativeCount++;
                }
            }

            int normalizedInputCount = Math.Max(inputSampleCount, observedCount);
            return new VisionPipelineBatchConfusionMatrix(
                normalizedInputCount,
                truePositiveCount,
                trueNegativeCount,
                falsePositiveCount,
                falseNegativeCount,
                executionErrorCount,
                normalizedInputCount - observedCount,
                unknownLabelCount);
        }

        internal static string ToOutcome(bool success)
        {
            return success ? OkOutcome : NgOutcome;
        }

        private static bool TryParseOutcome(string value, out bool success)
        {
            if (string.Equals(value?.Trim(), OkOutcome, StringComparison.OrdinalIgnoreCase))
            {
                success = true;
                return true;
            }

            if (string.Equals(value?.Trim(), NgOutcome, StringComparison.OrdinalIgnoreCase))
            {
                success = false;
                return true;
            }

            success = false;
            return false;
        }
    }

    internal sealed class VisionPipelineBatchConfusionMatrix
    {
        internal VisionPipelineBatchConfusionMatrix(
            int inputSampleCount,
            int truePositiveCount,
            int trueNegativeCount,
            int falsePositiveCount,
            int falseNegativeCount,
            int executionErrorCount,
            int notRunCount,
            int unknownLabelCount)
        {
            InputSampleCount = Math.Max(0, inputSampleCount);
            TruePositiveCount = Math.Max(0, truePositiveCount);
            TrueNegativeCount = Math.Max(0, trueNegativeCount);
            FalsePositiveCount = Math.Max(0, falsePositiveCount);
            FalseNegativeCount = Math.Max(0, falseNegativeCount);
            ExecutionErrorCount = Math.Max(0, executionErrorCount);
            NotRunCount = Math.Max(0, notRunCount);
            UnknownLabelCount = Math.Max(0, unknownLabelCount);
        }

        internal int InputSampleCount { get; }

        internal int TruePositiveCount { get; }

        internal int TrueNegativeCount { get; }

        internal int FalsePositiveCount { get; }

        internal int FalseNegativeCount { get; }

        internal int ExecutionErrorCount { get; }

        internal int NotRunCount { get; }

        internal int UnknownLabelCount { get; }

        internal int EvaluatedCount => TruePositiveCount
            + TrueNegativeCount
            + FalsePositiveCount
            + FalseNegativeCount;

        internal int AccountedCount => EvaluatedCount
            + ExecutionErrorCount
            + NotRunCount
            + UnknownLabelCount;

        internal bool IsBalanced => AccountedCount == InputSampleCount;

        internal static string FormatRate(int numerator, int denominator)
        {
            return denominator <= 0
                ? "N/A"
                : (100D * numerator / denominator).ToString("0.0", System.Globalization.CultureInfo.CurrentCulture) + "%";
        }

        internal string AccuracyText => FormatRate(
            TruePositiveCount + TrueNegativeCount,
            EvaluatedCount);

        internal string FalseAcceptRateText => FormatRate(
            FalsePositiveCount,
            FalsePositiveCount + TrueNegativeCount);

        internal string FalseRejectRateText => FormatRate(
            FalseNegativeCount,
            FalseNegativeCount + TruePositiveCount);
    }
}
