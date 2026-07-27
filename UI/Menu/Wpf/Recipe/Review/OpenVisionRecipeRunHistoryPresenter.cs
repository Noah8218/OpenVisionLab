using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    // Derives read-only Run History filters, baseline comparison, and performance evidence from supplied run state.
    internal static class OpenVisionRecipeRunHistoryPresenter
    {
        internal static OpenVisionRecipeRunHistorySelection BuildRecentRunSelection(
            IEnumerable<OpenVisionRecipeBatchRunOption> runs,
            string previousSummaryPath)
        {
            List<OpenVisionRecipeBatchRunOption> options = (runs ?? Array.Empty<OpenVisionRecipeBatchRunOption>())
                .Take(3)
                .ToList();
            if (options.Count == 0)
            {
                options.Add(OpenVisionRecipeBatchRunOption.CreateEmpty());
            }

            OpenVisionRecipeBatchRunOption selected = options.FirstOrDefault(option =>
                    string.Equals(option.SummaryPath, previousSummaryPath, StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault();
            return new OpenVisionRecipeRunHistorySelection(options, selected);
        }

        internal static OpenVisionRecipeRunHistorySelection BuildBaselineRunSelection(
            OpenVisionRecipeBatchRunOption current,
            IEnumerable<OpenVisionRecipeBatchRunOption> recentRuns,
            string previousBaselinePath)
        {
            List<OpenVisionRecipeBatchRunOption> options = (recentRuns ?? Array.Empty<OpenVisionRecipeBatchRunOption>())
                .Where(option => option != null
                    && !string.IsNullOrWhiteSpace(option.SummaryPath)
                    && !string.Equals(option.SummaryPath, current?.SummaryPath, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (options.Count == 0)
            {
                options.Add(OpenVisionRecipeBatchRunOption.CreateEmpty());
            }

            List<OpenVisionRecipeBatchRunOption> historyRuns = recentRuns?.ToList()
                ?? new List<OpenVisionRecipeBatchRunOption>();
            OpenVisionRecipeBatchRunOption autoBaseline = FindAutoBaselineRunOption(current, historyRuns);
            OpenVisionRecipeBatchRunOption selected = options.FirstOrDefault(option =>
                    !string.IsNullOrWhiteSpace(option.SummaryPath)
                    && string.Equals(option.SummaryPath, previousBaselinePath, StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault(option =>
                    !string.IsNullOrWhiteSpace(option.SummaryPath)
                    && string.Equals(option.SummaryPath, autoBaseline?.SummaryPath, StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault();
            return new OpenVisionRecipeRunHistorySelection(options, selected);
        }

        internal static OpenVisionRecipeBatchSampleResultOption SelectDefaultBatchSampleResult(
            OpenVisionRecipeBatchRunOption option,
            bool ngOnly = false,
            bool reviewQueueOnly = false)
        {
            IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> results =
                option?.SampleResults ?? Array.Empty<OpenVisionRecipeBatchSampleResultOption>();
            if (reviewQueueOnly)
            {
                return results.FirstOrDefault(result => result?.IsInReviewQueue == true);
            }

            if (ngOnly)
            {
                return results.FirstOrDefault(result => result != null && IsFilteredFailure(option, result));
            }

            return results
                .FirstOrDefault(result => result != null
                    && IsFilteredFailure(option, result)
                    && !string.IsNullOrWhiteSpace(result.FailedStep))
                ?? results.FirstOrDefault();
        }

        internal static OpenVisionRecipePairSampleRunSummary SelectDefaultPairSampleResult(
            OpenVisionRecipePairRunSummary summary)
        {
            return summary?.SampleResults?
                .FirstOrDefault(result => result != null && !result.Success)
                ?? summary?.SampleResults?.FirstOrDefault();
        }

        internal static IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> BuildFilteredSampleResults(
            OpenVisionRecipeBatchRunOption option,
            bool showNgOnly,
            bool showReviewQueueOnly)
        {
            IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> results =
                option?.SampleResults ?? Array.Empty<OpenVisionRecipeBatchSampleResultOption>();
            if (showReviewQueueOnly)
            {
                return results
                    .Where(result => result?.IsInReviewQueue == true)
                    .ToList();
            }

            if (!showNgOnly)
            {
                return results;
            }

            return results
                .Where(result => result != null && IsFilteredFailure(option, result))
                .ToList();
        }

        internal static string BuildReviewQueueSummaryText(OpenVisionRecipeBatchRunOption option)
        {
            int total = option?.SampleResults?.Count(result => result != null) ?? 0;
            if (option == null || string.IsNullOrWhiteSpace(option.SummaryPath))
            {
                return OpenVisionRecipeText.Local(
                    "저장된 실행을 선택하면 검토 큐를 표시합니다.",
                    "Select a saved run to show its review queue.");
            }

            if (!option.HasPersistedReviewQueue)
            {
                return OpenVisionRecipeText.Local(
                    "이 과거 실행에는 저장된 검토 큐가 없습니다. 새 Suite 실행부터 생성됩니다.",
                    "This older run has no saved review queue. New suite runs create one.");
            }

            string shortHash = option.ReviewQueueSha256.Length <= 12
                ? option.ReviewQueueSha256
                : option.ReviewQueueSha256.Substring(0, 12);
            string policyVersion = option.ReviewQueuePolicy.Split('|').FirstOrDefault() ?? "v1";
            return OpenVisionRecipeText.Local("검토 큐 ", "Review queue ")
                + option.ReviewQueueCount.ToString(CultureInfo.InvariantCulture)
                + "/"
                + total.ToString(CultureInfo.InvariantCulture)
                + " | " + policyVersion
                + " | SHA-256 " + shortHash;
        }

        internal static string BuildNgFilterSummaryText(
            OpenVisionRecipeBatchRunOption option,
            bool showNgOnly)
        {
            IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> results =
                option?.SampleResults ?? Array.Empty<OpenVisionRecipeBatchSampleResultOption>();
            int total = results.Count(result => result != null);
            if (total == 0 || string.IsNullOrWhiteSpace(option?.SummaryPath))
            {
                return OpenVisionRecipeText.Local(
                    "저장된 실행을 선택하면 NG 원인을 요약합니다.",
                    "Select a saved run to summarize NG causes.");
            }

            List<OpenVisionRecipeBatchSampleResultOption> failures = results
                .Where(result => result != null && IsFilteredFailure(option, result))
                .ToList();
            bool judgmentSuite = option?.IsJudgmentSuite == true;
            string prefix = showNgOnly
                ? judgmentSuite
                    ? OpenVisionRecipeText.Local("오판 필터: ", "Misclassification filter: ")
                    : OpenVisionRecipeText.Local("NG 필터: ", "NG filter: ")
                : OpenVisionRecipeText.Local("샘플: ", "Samples: ");
            string counts = showNgOnly
                ? failures.Count.ToString(CultureInfo.InvariantCulture) + "/" + total.ToString(CultureInfo.InvariantCulture)
                : total.ToString(CultureInfo.InvariantCulture)
                    + (judgmentSuite ? OpenVisionRecipeText.Local(" / 오판 ", " / misclassified ") : " / NG ")
                    + failures.Count.ToString(CultureInfo.InvariantCulture);

            if (failures.Count == 0)
            {
                return prefix + counts + " | " + (judgmentSuite
                    ? OpenVisionRecipeText.Local("오판 없음", "No misclassifications")
                    : OpenVisionRecipeText.Local("NG 없음", "No NG samples"));
            }

            if (judgmentSuite)
            {
                return prefix
                    + counts
                    + " | "
                    + OpenVisionRecipeText.Local("미검 ", "false accept ")
                    + failures.Count(result => result.IsFalseAccept).ToString(CultureInfo.InvariantCulture)
                    + " · "
                    + OpenVisionRecipeText.Local("과검 ", "false reject ")
                    + failures.Count(result => result.IsFalseReject).ToString(CultureInfo.InvariantCulture)
                    + " · "
                    + OpenVisionRecipeText.Local("실행 오류 ", "execution error ")
                    + failures.Count(result => !result.ExecutionCompleted).ToString(CultureInfo.InvariantCulture);
            }

            string causes = string.Join(
                " | ",
                failures
                    .GroupBy(result => string.IsNullOrWhiteSpace(result.FailedStep) ? "No failed step" : result.FailedStep.Trim())
                    .Select(group => group.Key + " x" + group.Count().ToString(CultureInfo.InvariantCulture))
                    .Take(3));
            return prefix + counts + " | " + OpenVisionRecipeText.Local("NG 원인: ", "NG causes: ") + causes;
        }

        private static bool IsFilteredFailure(
            OpenVisionRecipeBatchRunOption option,
            OpenVisionRecipeBatchSampleResultOption result)
        {
            return option?.IsJudgmentSuite == true
                ? result.HasExpectedOutcome && !result.JudgmentCorrect
                : !result.Success;
        }

        internal static OpenVisionRecipeBatchRunOption ResolveBaselineRunOption(
            OpenVisionRecipeBatchRunOption selectedBaseline,
            OpenVisionRecipeBatchRunOption current,
            IReadOnlyList<OpenVisionRecipeBatchRunOption> runs)
        {
            if (selectedBaseline != null
                && !string.IsNullOrWhiteSpace(selectedBaseline.SummaryPath)
                && !string.Equals(selectedBaseline.SummaryPath, current?.SummaryPath, StringComparison.OrdinalIgnoreCase))
            {
                return selectedBaseline;
            }

            return FindAutoBaselineRunOption(current, runs);
        }

        internal static OpenVisionRecipeBatchRunOption FindAutoBaselineRunOption(
            OpenVisionRecipeBatchRunOption current,
            IReadOnlyList<OpenVisionRecipeBatchRunOption> runs)
        {
            if (current == null || string.IsNullOrWhiteSpace(current.SummaryPath))
            {
                return null;
            }

            List<OpenVisionRecipeBatchRunOption> candidates = (runs ?? Array.Empty<OpenVisionRecipeBatchRunOption>())
                .Where(option => option != null && !string.IsNullOrWhiteSpace(option.SummaryPath))
                .ToList();
            int currentIndex = candidates.FindIndex(option =>
                string.Equals(option.SummaryPath, current.SummaryPath, StringComparison.OrdinalIgnoreCase));
            if (currentIndex < 0)
            {
                return null;
            }

            return candidates.Skip(currentIndex + 1).FirstOrDefault()
                ?? candidates.Take(currentIndex).LastOrDefault();
        }

        internal static IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> BuildComparisonRows(
            OpenVisionRecipeBatchRunOption currentOption,
            OpenVisionRecipeBatchRunOption baselineOption,
            VisionPipelineBatchRunSummary current,
            VisionPipelineBatchRunSummary baseline)
        {
            if (currentOption == null || string.IsNullOrWhiteSpace(currentOption.SummaryPath))
            {
                return new[] { OpenVisionRecipeBatchRunComparisonRow.CreateEmpty() };
            }

            if (baselineOption == null || string.IsNullOrWhiteSpace(baselineOption.SummaryPath))
            {
                return new[] { OpenVisionRecipeBatchRunComparisonRow.CreateNoBaseline(currentOption.DisplayText) };
            }

            if (current?.Results == null || baseline?.Results == null)
            {
                return new[] { OpenVisionRecipeBatchRunComparisonRow.CreateNoBaseline(currentOption.DisplayText) };
            }

            Dictionary<string, VisionPipelineBatchSampleRunResult> currentBySample = BuildResultMap(current.Results);
            Dictionary<string, VisionPipelineBatchSampleRunResult> baselineBySample = BuildResultMap(baseline.Results);
            List<string> sampleNames = currentBySample.Keys
                .Union(baselineBySample.Keys, StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (sampleNames.Count == 0)
            {
                return new[] { OpenVisionRecipeBatchRunComparisonRow.CreateEmpty() };
            }

            return sampleNames
                .Select(sampleName =>
                {
                    currentBySample.TryGetValue(sampleName, out VisionPipelineBatchSampleRunResult currentResult);
                    baselineBySample.TryGetValue(sampleName, out VisionPipelineBatchSampleRunResult baselineResult);
                    return OpenVisionRecipeBatchRunComparisonRow.Create(sampleName, baselineResult, currentResult);
                })
                .ToList();
        }

        internal static OpenVisionRecipeBatchRunComparisonRow SelectDefaultComparisonRow(
            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> rows)
        {
            return rows?.FirstOrDefault(row => row != null && row.IsRegression)
                ?? rows?.FirstOrDefault(row => row != null && row.IsStillFailing)
                ?? rows?.FirstOrDefault(row => row != null && row.IsRecovered)
                ?? rows?.FirstOrDefault();
        }

        internal static string BuildComparisonSummaryText(
            OpenVisionRecipeBatchRunOption current,
            OpenVisionRecipeBatchRunOption selectedBaseline,
            OpenVisionRecipeBatchRunOption resolvedBaseline,
            IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> rows)
        {
            rows = rows ?? Array.Empty<OpenVisionRecipeBatchRunComparisonRow>();
            string analyticsPrefix = string.IsNullOrWhiteSpace(current?.AnalyticsText)
                ? string.Empty
                : current.AnalyticsText + Environment.NewLine;
            string performanceComparison = BuildPerformanceComparisonText(current, resolvedBaseline);
            if (!string.IsNullOrWhiteSpace(performanceComparison))
            {
                analyticsPrefix += performanceComparison + Environment.NewLine;
            }

            int comparable = rows.Count(row => row != null && row.IsComparable);
            if (comparable == 0)
            {
                return analyticsPrefix
                    + OpenVisionRecipeText.Local("비교할 이전 benchmark 실행이 없습니다.", "No previous benchmark run is available for comparison.");
            }

            int regression = rows.Count(row => row != null && row.IsRegression);
            int recovered = rows.Count(row => row != null && row.IsRecovered);
            int stillNg = rows.Count(row => row != null && row.IsStillFailing);
            string baseline = selectedBaseline?.DisplayText;
            string prefix = string.IsNullOrWhiteSpace(baseline)
                ? string.Empty
                : OpenVisionRecipeText.Local("기준 ", "Baseline ") + baseline + " | ";
            return analyticsPrefix + prefix + "Compared "
                + comparable.ToString(CultureInfo.InvariantCulture)
                + " | Regression "
                + regression.ToString(CultureInfo.InvariantCulture)
                + " | Recovered "
                + recovered.ToString(CultureInfo.InvariantCulture)
                + " | Still NG "
                + stillNg.ToString(CultureInfo.InvariantCulture);
        }

        private static Dictionary<string, VisionPipelineBatchSampleRunResult> BuildResultMap(
            IEnumerable<VisionPipelineBatchSampleRunResult> results)
        {
            return (results ?? Enumerable.Empty<VisionPipelineBatchSampleRunResult>())
                .Where(result => result != null && !string.IsNullOrWhiteSpace(result.SampleName))
                .GroupBy(result => result.SampleName.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        }

        private static string BuildPerformanceComparisonText(
            OpenVisionRecipeBatchRunOption current,
            OpenVisionRecipeBatchRunOption baseline)
        {
            if (current == null
                || baseline == null
                || string.IsNullOrWhiteSpace(current.SummaryPath)
                || string.IsNullOrWhiteSpace(baseline.SummaryPath))
            {
                return string.Empty;
            }

            if (!HaveEquivalentSampleSets(current.RunSummary, baseline.RunSummary))
            {
                return OpenVisionRecipeText.Local(
                    "성능 비교 안 함: 기준과 현재 검증 샘플 세트가 다릅니다.",
                    "Performance comparison skipped: the baseline and current validation sample sets differ.");
            }

            VisionPipelineBatchRunSummaryStorage.BatchRunStatistics currentStatistics = current.Statistics;
            VisionPipelineBatchRunSummaryStorage.BatchRunStatistics baselineStatistics = baseline.Statistics;
            if (currentStatistics.TimingCount != currentStatistics.ResultCount
                || baselineStatistics.TimingCount != baselineStatistics.ResultCount)
            {
                return OpenVisionRecipeText.Local(
                    "성능 비교 안 함: 기준 또는 현재 실행의 시간 기록이 완전하지 않습니다.",
                    "Performance comparison skipped: baseline or current timing data is incomplete.");
            }

            double averageDelta = currentStatistics.AverageMilliseconds - baselineStatistics.AverageMilliseconds;
            double p95Delta = currentStatistics.P95Milliseconds - baselineStatistics.P95Milliseconds;
            return OpenVisionRecipeText.Local("성능 비교: 평균 ", "Performance comparison: avg ")
                + baselineStatistics.AverageMilliseconds.ToString("0.0", CultureInfo.CurrentCulture)
                + " -> "
                + currentStatistics.AverageMilliseconds.ToString("0.0", CultureInfo.CurrentCulture)
                + " ms ("
                + averageDelta.ToString("+0.0;-0.0;0.0", CultureInfo.CurrentCulture)
                + ") | p95 "
                + baselineStatistics.P95Milliseconds.ToString("0.0", CultureInfo.CurrentCulture)
                + " -> "
                + currentStatistics.P95Milliseconds.ToString("0.0", CultureInfo.CurrentCulture)
                + " ms ("
                + p95Delta.ToString("+0.0;-0.0;0.0", CultureInfo.CurrentCulture)
                + ")";
        }

        private static bool HaveEquivalentSampleSets(
            VisionPipelineBatchRunSummary current,
            VisionPipelineBatchRunSummary baseline)
        {
            if (current == null
                || baseline == null
                || string.IsNullOrWhiteSpace(current.SuiteKind)
                || string.IsNullOrWhiteSpace(baseline.SuiteKind)
                || string.IsNullOrWhiteSpace(current.SuiteName)
                || string.IsNullOrWhiteSpace(baseline.SuiteName)
                || !string.Equals(current.SuiteKind, baseline.SuiteKind, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(current.SuiteName, baseline.SuiteName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            List<string> currentSamples = BuildSampleIdentityList(current.Results);
            List<string> baselineSamples = BuildSampleIdentityList(baseline.Results);
            return currentSamples.Count > 0
                && currentSamples.SequenceEqual(baselineSamples, StringComparer.OrdinalIgnoreCase);
        }

        private static List<string> BuildSampleIdentityList(IEnumerable<VisionPipelineBatchSampleRunResult> results)
        {
            return (results ?? Enumerable.Empty<VisionPipelineBatchSampleRunResult>())
                .Where(result => result != null)
                .Select(result =>
                {
                    string path = string.IsNullOrWhiteSpace(result.SampleImagePath)
                        ? result.ReportPath
                        : result.SampleImagePath;
                    return string.IsNullOrWhiteSpace(path)
                        ? "name:" + (result.SampleName ?? string.Empty).Trim()
                        : "path:" + path.Trim().Replace('/', '\\');
                })
                .Where(identity => !string.Equals(identity, "name:", StringComparison.Ordinal))
                .OrderBy(identity => identity, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }

    internal sealed class OpenVisionRecipeRunHistorySelection
    {
        internal OpenVisionRecipeRunHistorySelection(
            IReadOnlyList<OpenVisionRecipeBatchRunOption> options,
            OpenVisionRecipeBatchRunOption selectedOption)
        {
            Options = options ?? Array.Empty<OpenVisionRecipeBatchRunOption>();
            SelectedOption = selectedOption;
        }

        internal IReadOnlyList<OpenVisionRecipeBatchRunOption> Options { get; }
        internal OpenVisionRecipeBatchRunOption SelectedOption { get; }
    }
}
