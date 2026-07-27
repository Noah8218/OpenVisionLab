using System;

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
}
