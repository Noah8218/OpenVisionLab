using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using OpenVisionLab.Vision2D.Pipeline;

namespace OpenVisionLab
{
    public sealed class VisionPipelineBatchRunSummary
    {
        public int SchemaVersion { get; set; }
        public string RecipeName { get; set; } = string.Empty;
        public string PipelineName { get; set; } = string.Empty;
        public string SuiteName { get; set; } = string.Empty;
        public string SuiteKind { get; set; } = string.Empty;
        public string PipelineSnapshotFile { get; set; } = string.Empty;
        public VisionPipelineExecutionProvenance ExecutionProvenance { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string StartedAt { get; set; } = string.Empty;
        public string FinishedAt { get; set; } = string.Empty;
        public double TotalMilliseconds { get; set; }
        public int TotalCount { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
        public int JudgmentCount { get; set; }
        public int JudgmentCorrectCount { get; set; }
        public int FalseAcceptCount { get; set; }
        public int FalseRejectCount { get; set; }
        public int ExecutionErrorCount { get; set; }
        public int LegacyAmbiguousCount { get; set; }
        public List<VisionPipelineBatchSampleRunResult> Results { get; set; } = new List<VisionPipelineBatchSampleRunResult>();
        public string ReviewQueuePolicy { get; set; } = string.Empty;
        public string ReviewQueueSha256 { get; set; } = string.Empty;
        public List<VisionPipelineBatchReviewQueueEntry> ReviewQueue { get; set; } = new List<VisionPipelineBatchReviewQueueEntry>();
    }

    public sealed class VisionPipelineBatchReviewQueueEntry
    {
        public int ResultIndex { get; set; }
        public string SampleName { get; set; } = string.Empty;
        public string SampleImagePath { get; set; } = string.Empty;
        public string RunReportPath { get; set; } = string.Empty;
        public string SourceSha256 { get; set; } = string.Empty;
        public string VariantId { get; set; } = string.Empty;
        public List<string> Reasons { get; set; } = new List<string>();
    }

    public sealed class VisionPipelineBatchSampleRunResult
    {
        public string SampleName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int OutcomeSchemaVersion { get; set; }
        public string ExecutionState { get; set; } = string.Empty;
        public bool HasJudgment { get; set; }
        public string ExpectedOutcome { get; set; } = string.Empty;
        public string ActualOutcome { get; set; } = string.Empty;
        public bool JudgmentCorrect { get; set; }
        // Legacy aggregate pass/fail. New judgment consumers must use the explicit
        // outcome fields above instead of inferring their meaning from Success.
        public bool Success { get; set; }
        public double TotalMilliseconds { get; set; }
        public string FailedStep { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string ReportPath { get; set; } = string.Empty;
        public string SampleImagePath { get; set; } = string.Empty;
        public string PairGroup { get; set; } = string.Empty;
        public string PairRole { get; set; } = string.Empty;
        public string VariantId { get; set; } = string.Empty;
        public string ExpectedMetricName { get; set; } = string.Empty;
        public string ExpectedMetricMinimum { get; set; } = string.Empty;
        public string ExpectedMetricMaximum { get; set; } = string.Empty;
        public string ExpectedText { get; set; } = string.Empty;
        public string MetricText { get; set; } = string.Empty;
        public string MetricReviewText { get; set; } = string.Empty;
        public string FinalLayer { get; set; } = string.Empty;
        public string OverlayCount { get; set; } = string.Empty;
        public string ActionSummary { get; set; } = string.Empty;
        public string RunReportPath { get; set; } = string.Empty;
    }

    internal static class VisionPipelineBatchRunSummaryStorage
    {
        internal const int CurrentSchemaVersion = 2;
        internal const string ReviewQueuePolicyV2 =
            "v3|all-execution-errors|all-misclassifications|all-evidence-gaps|metric-min-max|hash-audit-3-per-variant-role";
        internal const string LegacyReviewQueuePolicyV2 =
            "v2|all-execution-errors|all-misclassifications|all-evidence-gaps|metric-min-max|hash-audit-3-per-stratum";

        internal sealed class BatchReviewQueue
        {
            public string Policy { get; set; } = ReviewQueuePolicyV2;
            public string Sha256 { get; set; } = string.Empty;
            public List<VisionPipelineBatchReviewQueueEntry> Entries { get; set; } = new List<VisionPipelineBatchReviewQueueEntry>();
        }

        public sealed class BatchRunStatistics
        {
            public int ResultCount { get; set; }
            public int TimingCount { get; set; }
            public int FailureCount { get; set; }
            public double FailureRatePercent { get; set; }
            public double AverageMilliseconds { get; set; }
            public double MedianMilliseconds { get; set; }
            public double P95Milliseconds { get; set; }
            public double MaximumMilliseconds { get; set; }
        }

        public enum StepTimingAvailability
        {
            Available,
            NoResults,
            MissingReportPath,
            MissingReportFile,
            InvalidReport,
            ReportIdentityMismatch,
            StepDefinitionMismatch,
            NoEnabledSteps,
            NoStepTimings
        }

        public sealed class BatchStepTimingStatistics
        {
            public int Index { get; set; }
            public string Name { get; set; } = string.Empty;
            public string ToolType { get; set; } = string.Empty;
            public int ReportCount { get; set; }
            public int TimingCount { get; set; }
            public double AverageMilliseconds { get; set; }
            public double P95Milliseconds { get; set; }
            public double MaximumMilliseconds { get; set; }
        }

        public sealed class BatchStepTimingAnalysis
        {
            public StepTimingAvailability Availability { get; set; }
            public int SampleCount { get; set; }
            public int ReportCount { get; set; }
            public string Detail { get; set; } = string.Empty;
            public List<BatchStepTimingStatistics> Steps { get; set; } = new List<BatchStepTimingStatistics>();

            public bool IsAvailable => Availability == StepTimingAvailability.Available;
        }

        public sealed class BatchRunSummaryInfo
        {
            public string Name { get; set; } = string.Empty;
            public string DirectoryPath { get; set; } = string.Empty;
            public string SummaryPath { get; set; } = string.Empty;
            public DateTime StartedAt { get; set; }
            public DateTime FinishedAt { get; set; }
            public int TotalCount { get; set; }
            public int PassCount { get; set; }
            public int FailCount { get; set; }

            public override string ToString()
            {
                return $"{StartedAt:yyyy-MM-dd HH:mm:ss.fff} [{PassCount}/{TotalCount}]";
            }
        }

        public static string Save(
            string recipeName,
            string pipelineName,
            DateTime startedAt,
            DateTime finishedAt,
            IEnumerable<VisionPipelineBatchSampleRunResult> results,
            string suiteName = "Validation Suite",
            string suiteKind = "Batch",
            string notes = "",
            VisionPipeline pipelineSnapshot = null,
            VisionPipelineExecutionProvenance executionProvenance = null)
        {
            List<VisionPipelineBatchSampleRunResult> resultList = (results ?? Enumerable.Empty<VisionPipelineBatchSampleRunResult>()).ToList();
            string batchName = CreateUniqueBatchName(recipeName, pipelineName, startedAt);
            string directory = RecipeWorkspaceService.GetVisionPipelineBatchRunDirectory(recipeName, pipelineName, batchName);

            VisionPipelineBatchRunSummary summary = new VisionPipelineBatchRunSummary
            {
                SchemaVersion = CurrentSchemaVersion,
                RecipeName = recipeName ?? string.Empty,
                PipelineName = pipelineName ?? string.Empty,
                SuiteName = string.IsNullOrWhiteSpace(suiteName) ? "Validation Suite" : suiteName.Trim(),
                SuiteKind = string.IsNullOrWhiteSpace(suiteKind) ? "Batch" : suiteKind.Trim(),
                PipelineSnapshotFile = pipelineSnapshot == null ? string.Empty : "pipeline.xml",
                ExecutionProvenance = pipelineSnapshot == null
                    ? null
                    : VisionPipelineExecutionPlan.CopyForStorage(
                        executionProvenance ?? VisionPipelineExecutionPlan.CreateIdentityOnly(pipelineSnapshot),
                        "pipeline.xml",
                        string.Empty),
                Notes = notes?.Trim() ?? string.Empty,
                StartedAt = startedAt.ToString("o"),
                FinishedAt = finishedAt.ToString("o"),
                TotalMilliseconds = (finishedAt - startedAt).TotalMilliseconds,
                TotalCount = resultList.Count,
                PassCount = resultList.Count(result => result.Success),
                FailCount = resultList.Count(result => !result.Success),
                JudgmentCount = resultList.Count(result =>
                    VisionPipelineBatchOutcomeContract.TryResolveExpectedSuccess(result, out _)),
                JudgmentCorrectCount = resultList.Count(result =>
                    VisionPipelineBatchOutcomeContract.TryResolveExpectedSuccess(result, out _)
                    && VisionPipelineBatchOutcomeContract.ResolveJudgmentCorrect(result)),
                FalseAcceptCount = resultList.Count(result =>
                    string.Equals(
                        VisionPipelineBatchOutcomeContract.ResolveMisclassificationReason(result),
                        "false-accept",
                        StringComparison.Ordinal)),
                FalseRejectCount = resultList.Count(result =>
                    string.Equals(
                        VisionPipelineBatchOutcomeContract.ResolveMisclassificationReason(result),
                        "false-reject",
                        StringComparison.Ordinal)),
                ExecutionErrorCount = resultList.Count(result =>
                    result?.OutcomeSchemaVersion > 0
                    && !VisionPipelineBatchOutcomeContract.IsExecutionCompleted(result)),
                LegacyAmbiguousCount = resultList.Count(result =>
                    !VisionPipelineBatchOutcomeContract.HasExplicitOutcome(result)),
                Results = resultList
            };
            BatchReviewQueue reviewQueue = BuildReviewQueue(resultList);
            summary.ReviewQueuePolicy = reviewQueue.Policy;
            summary.ReviewQueueSha256 = reviewQueue.Sha256;
            summary.ReviewQueue = reviewQueue.Entries;

            if (pipelineSnapshot != null)
            {
                SerializeHelper.SaveXmlFile(
                    RecipeWorkspaceService.GetContainedStoragePath(
                        directory,
                        summary.PipelineSnapshotFile,
                        "Batch pipeline snapshot path"),
                    pipelineSnapshot);
            }

            string xmlPath = RecipeWorkspaceService.GetContainedStoragePath(
                directory,
                "summary.xml",
                "Batch summary path");
            SerializeHelper.SaveXmlFile(xmlPath, summary);
            File.WriteAllLines(
                RecipeWorkspaceService.GetContainedStoragePath(
                    directory,
                    "summary.tsv",
                    "Batch summary table path"),
                CreateTsvLines(summary));
            return xmlPath;
        }

        internal static BatchReviewQueue BuildReviewQueue(
            IEnumerable<VisionPipelineBatchSampleRunResult> results)
        {
            return BuildReviewQueue(results, ReviewQueuePolicyV2);
        }

        internal static BatchReviewQueue BuildReviewQueue(
            IEnumerable<VisionPipelineBatchSampleRunResult> results,
            string persistedPolicy)
        {
            bool useLegacyStrata = string.Equals(
                persistedPolicy,
                LegacyReviewQueuePolicyV2,
                StringComparison.Ordinal);
            List<VisionPipelineBatchSampleRunResult> resultList = (results
                    ?? Enumerable.Empty<VisionPipelineBatchSampleRunResult>())
                .Where(result => result != null)
                .ToList();
            Dictionary<int, ReviewEvidence> evidenceByIndex = resultList
                .Select((result, index) => LoadReviewEvidence(index, result))
                .ToDictionary(evidence => evidence.ResultIndex);
            Dictionary<int, HashSet<string>> reasonsByIndex = new Dictionary<int, HashSet<string>>();

            foreach (ReviewEvidence evidence in evidenceByIndex.Values)
            {
                if (evidence.Result?.OutcomeSchemaVersion > 0)
                {
                    if (!VisionPipelineBatchOutcomeContract.IsExecutionCompleted(evidence.Result))
                    {
                        AddReviewReason(reasonsByIndex, evidence.ResultIndex, "execution-error");
                    }
                }
                else if (!evidence.Result.Success)
                {
                    AddReviewReason(reasonsByIndex, evidence.ResultIndex, "runtime-failure");
                }

                string misclassification =
                    VisionPipelineBatchOutcomeContract.ResolveMisclassificationReason(evidence.Result);
                if (!string.IsNullOrWhiteSpace(misclassification))
                {
                    AddReviewReason(reasonsByIndex, evidence.ResultIndex, misclassification);
                }

                if (!evidence.HasCompleteEvidence)
                {
                    AddReviewReason(reasonsByIndex, evidence.ResultIndex, "evidence-gap");
                }
            }

            IEnumerable<string> metricNames = evidenceByIndex.Values
                .SelectMany(evidence => evidence.Metrics.Keys)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(name => name, StringComparer.Ordinal);
            foreach (string metricName in metricNames)
            {
                List<ReviewEvidence> available = evidenceByIndex.Values
                    .Where(evidence => evidence.Metrics.TryGetValue(metricName, out double value) && IsFinite(value))
                    .OrderBy(evidence => evidence.Metrics[metricName])
                    .ThenBy(evidence => evidence.AuditKey, StringComparer.Ordinal)
                    .ToList();
                if (available.Count < 2
                    || available[0].Metrics[metricName] == available[available.Count - 1].Metrics[metricName])
                {
                    continue;
                }

                AddReviewReason(reasonsByIndex, available[0].ResultIndex, "metric-min:" + metricName);
                AddReviewReason(reasonsByIndex, available[available.Count - 1].ResultIndex, "metric-max:" + metricName);
            }

            foreach (IGrouping<string, ReviewEvidence> stratum in evidenceByIndex.Values
                .GroupBy(
                    evidence => ResolveReviewStratum(evidence.Result, useLegacyStrata),
                    StringComparer.OrdinalIgnoreCase)
                .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase))
            {
                foreach (ReviewEvidence evidence in stratum
                    .OrderBy(item => item.AuditKey, StringComparer.Ordinal)
                    .ThenBy(item => item.ResultIndex)
                    .Take(3))
                {
                    AddReviewReason(reasonsByIndex, evidence.ResultIndex, "hash-audit:" + stratum.Key);
                }
            }

            BatchReviewQueue queue = new BatchReviewQueue();
            queue.Policy = useLegacyStrata ? LegacyReviewQueuePolicyV2 : ReviewQueuePolicyV2;
            foreach (KeyValuePair<int, HashSet<string>> pair in reasonsByIndex.OrderBy(pair => pair.Key))
            {
                ReviewEvidence evidence = evidenceByIndex[pair.Key];
                queue.Entries.Add(new VisionPipelineBatchReviewQueueEntry
                {
                    ResultIndex = pair.Key,
                    SampleName = evidence.Result.SampleName ?? string.Empty,
                    SampleImagePath = evidence.Result.SampleImagePath ?? string.Empty,
                    RunReportPath = evidence.Result.RunReportPath ?? string.Empty,
                    SourceSha256 = evidence.SourceSha256,
                    VariantId = NormalizeVariantId(evidence.Result.VariantId),
                    Reasons = pair.Value.OrderBy(reason => reason, StringComparer.Ordinal).ToList()
                });
            }

            string canonical = queue.Policy + "\n" + string.Join(
                "\n",
                queue.Entries.Select(entry =>
                    entry.ResultIndex.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    + "|" + entry.SampleName
                    + "|" + entry.SampleImagePath
                    + "|" + entry.SourceSha256
                    + "|" + entry.VariantId
                    + "|" + string.Join(";", entry.Reasons)));
            queue.Sha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
            return queue;
        }

        private static ReviewEvidence LoadReviewEvidence(
            int resultIndex,
            VisionPipelineBatchSampleRunResult result)
        {
            VisionPipelineRunReport report = null;
            if (!string.IsNullOrWhiteSpace(result.RunReportPath) && File.Exists(result.RunReportPath))
            {
                try
                {
                    report = VisionPipelineRunReportStorage.Load(result.RunReportPath);
                }
                catch
                {
                    report = null;
                }
            }

            Dictionary<string, double> metrics = new Dictionary<string, double>(StringComparer.Ordinal);
            if (report?.Steps != null)
            {
                foreach (VisionPipelineStepRunReport step in report.Steps.Where(step => step != null))
                {
                    foreach (VisionPipelineMetricRunReport metric in step.Metrics?.Where(metric => metric != null)
                        ?? Enumerable.Empty<VisionPipelineMetricRunReport>())
                    {
                        if (string.IsNullOrWhiteSpace(metric.Name) || !IsFinite(metric.Value))
                        {
                            continue;
                        }

                        metrics[step.Index.ToString(System.Globalization.CultureInfo.InvariantCulture)
                            + ":" + metric.Name.Trim()] = metric.Value;
                    }
                }
            }

            string sourceSha256 = report?.SourceImageSha256?.Trim() ?? string.Empty;
            string reportDirectory = Path.GetDirectoryName(result.RunReportPath ?? string.Empty) ?? string.Empty;
            string storedSourcePath = ResolveReportArtifactPath(reportDirectory, report?.SourceImageFile);
            bool hasVerifiedSource = !string.IsNullOrWhiteSpace(storedSourcePath)
                && VisionPipelineRunReportStorage.IsFileSha256Match(storedSourcePath, sourceSha256);
            bool hasDrawing = report?.Steps?.Any(step => step != null
                && (!string.IsNullOrWhiteSpace(ResolveReportArtifactPath(reportDirectory, step.OverlayImageFile))
                    || !string.IsNullOrWhiteSpace(ResolveReportArtifactPath(reportDirectory, step.ResultImageFile)))) == true;
            bool hasCompleteEvidence = report != null && hasVerifiedSource && hasDrawing;
            string auditIdentity = string.IsNullOrWhiteSpace(sourceSha256)
                ? (result.SampleImagePath ?? string.Empty) + "|" + (result.SampleName ?? string.Empty) + "|" + resultIndex
                : sourceSha256;
            string auditKey = string.IsNullOrWhiteSpace(sourceSha256)
                ? Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(auditIdentity)))
                : sourceSha256.ToUpperInvariant();
            return new ReviewEvidence(resultIndex, result, hasCompleteEvidence, sourceSha256, auditKey, metrics);
        }

        private static string ResolveReportArtifactPath(string reportDirectory, string storedPath)
        {
            if (string.IsNullOrWhiteSpace(storedPath))
            {
                return string.Empty;
            }

            try
            {
                string candidate;
                if (Path.IsPathRooted(storedPath))
                {
                    // Absolute legacy evidence paths remain readable for compatibility;
                    // all newly written report artifacts are relative and contained.
                    candidate = Path.GetFullPath(storedPath);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(reportDirectory))
                    {
                        return string.Empty;
                    }

                    candidate = RecipeWorkspaceService.GetContainedStoragePath(
                        reportDirectory,
                        storedPath,
                        "Batch report artifact path");
                }

                return File.Exists(candidate) ? candidate : string.Empty;
            }
            catch (Exception exception) when (
                exception is ArgumentException
                || exception is IOException
                || exception is NotSupportedException)
            {
                return string.Empty;
            }
        }

        private static void AddReviewReason(
            IDictionary<int, HashSet<string>> reasonsByIndex,
            int resultIndex,
            string reason)
        {
            if (!reasonsByIndex.TryGetValue(resultIndex, out HashSet<string> reasons))
            {
                reasons = new HashSet<string>(StringComparer.Ordinal);
                reasonsByIndex[resultIndex] = reasons;
            }

            reasons.Add(reason);
        }

        private static string ResolveReviewStratum(
            VisionPipelineBatchSampleRunResult result,
            bool useLegacyStrata)
        {
            string role = string.IsNullOrWhiteSpace(result?.PairRole)
                ? "ALL"
                : result.PairRole.Trim().ToUpperInvariant();
            if (useLegacyStrata)
            {
                return role;
            }

            return "VARIANT:" + NormalizeVariantId(result?.VariantId) + "|ROLE:" + role;
        }

        private static string NormalizeVariantId(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "Default" : value.Trim();
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private sealed class ReviewEvidence
        {
            internal ReviewEvidence(
                int resultIndex,
                VisionPipelineBatchSampleRunResult result,
                bool hasCompleteEvidence,
                string sourceSha256,
                string auditKey,
                Dictionary<string, double> metrics)
            {
                ResultIndex = resultIndex;
                Result = result;
                HasCompleteEvidence = hasCompleteEvidence;
                SourceSha256 = sourceSha256 ?? string.Empty;
                AuditKey = auditKey ?? string.Empty;
                Metrics = metrics ?? new Dictionary<string, double>(StringComparer.Ordinal);
            }

            internal int ResultIndex { get; }
            internal VisionPipelineBatchSampleRunResult Result { get; }
            internal bool HasCompleteEvidence { get; }
            internal string SourceSha256 { get; }
            internal string AuditKey { get; }
            internal Dictionary<string, double> Metrics { get; }
        }

        public static List<BatchRunSummaryInfo> List(string recipeName, string pipelineName)
        {
            string rootDirectory = RecipeWorkspaceService.GetVisionPipelineBatchRunRootDirectory(recipeName, pipelineName);
            if (!Directory.Exists(rootDirectory))
            {
                return new List<BatchRunSummaryInfo>();
            }

            List<BatchRunSummaryInfo> summaries = new List<BatchRunSummaryInfo>();
            foreach (string directory in Directory.EnumerateDirectories(rootDirectory))
            {
                string summaryPath = RecipeWorkspaceService.GetContainedStoragePath(
                    directory,
                    "summary.xml",
                    "Batch summary path");
                if (!File.Exists(summaryPath))
                {
                    continue;
                }

                VisionPipelineBatchRunSummary summary = Load(summaryPath);
                if (summary == null)
                {
                    continue;
                }

                DateTime.TryParse(summary.StartedAt, out DateTime startedAt);
                DateTime.TryParse(summary.FinishedAt, out DateTime finishedAt);
                summaries.Add(new BatchRunSummaryInfo
                {
                    Name = Path.GetFileName(directory),
                    DirectoryPath = directory,
                    SummaryPath = summaryPath,
                    StartedAt = startedAt == default ? File.GetCreationTime(directory) : startedAt,
                    FinishedAt = finishedAt,
                    TotalCount = summary.TotalCount,
                    PassCount = summary.PassCount,
                    FailCount = summary.FailCount
                });
            }

            return summaries
                .OrderByDescending(summary => summary.StartedAt)
                .ToList();
        }

        public static VisionPipelineBatchRunSummary Load(string summaryPath)
        {
            return SerializeHelper.TryLoadFromXmlFile(summaryPath, out VisionPipelineBatchRunSummary summary)
                ? summary
                : null;
        }

        public static BatchRunStatistics CalculateStatistics(
            IEnumerable<VisionPipelineBatchSampleRunResult> results)
        {
            List<VisionPipelineBatchSampleRunResult> resultList = (results
                    ?? Enumerable.Empty<VisionPipelineBatchSampleRunResult>())
                .Where(result => result != null)
                .ToList();
            List<double> timings = resultList
                .Select(result => result.TotalMilliseconds)
                .Where(value => value > 0D && !double.IsNaN(value) && !double.IsInfinity(value))
                .OrderBy(value => value)
                .ToList();
            int failureCount = resultList.Count(result => !result.Success);

            if (timings.Count == 0)
            {
                return new BatchRunStatistics
                {
                    ResultCount = resultList.Count,
                    FailureCount = failureCount,
                    FailureRatePercent = resultList.Count == 0
                        ? 0D
                        : failureCount * 100D / resultList.Count
                };
            }

            int middle = timings.Count / 2;
            double median = timings.Count % 2 == 0
                ? (timings[middle - 1] + timings[middle]) / 2D
                : timings[middle];
            int p95Index = Math.Max(0, (int)Math.Ceiling(timings.Count * 0.95D) - 1);

            return new BatchRunStatistics
            {
                ResultCount = resultList.Count,
                TimingCount = timings.Count,
                FailureCount = failureCount,
                FailureRatePercent = failureCount * 100D / resultList.Count,
                AverageMilliseconds = timings.Average(),
                MedianMilliseconds = median,
                P95Milliseconds = timings[p95Index],
                MaximumMilliseconds = timings[timings.Count - 1]
            };
        }

        public static BatchStepTimingAnalysis CalculateStepTimingAnalysis(
            VisionPipelineBatchRunSummary summary)
        {
            List<VisionPipelineBatchSampleRunResult> results = summary?.Results?
                .Where(result => result != null)
                .ToList() ?? new List<VisionPipelineBatchSampleRunResult>();
            BatchStepTimingAnalysis analysis = new BatchStepTimingAnalysis
            {
                Availability = StepTimingAvailability.NoResults,
                SampleCount = results.Count
            };
            if (results.Count == 0)
            {
                return analysis;
            }

            List<List<VisionPipelineStepRunReport>> reportSteps = new List<List<VisionPipelineStepRunReport>>();
            foreach (VisionPipelineBatchSampleRunResult result in results)
            {
                if (string.IsNullOrWhiteSpace(result.RunReportPath))
                {
                    return Unavailable(analysis, StepTimingAvailability.MissingReportPath, result.SampleName);
                }

                if (!File.Exists(result.RunReportPath))
                {
                    return Unavailable(analysis, StepTimingAvailability.MissingReportFile, result.SampleName);
                }

                VisionPipelineRunReport report = VisionPipelineRunReportStorage.Load(result.RunReportPath);
                if (report == null)
                {
                    return Unavailable(analysis, StepTimingAvailability.InvalidReport, result.SampleName);
                }

                analysis.ReportCount++;
                if (!string.Equals(summary.RecipeName ?? string.Empty, report.RecipeName ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(summary.PipelineName ?? string.Empty, report.PipelineName ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                {
                    return Unavailable(analysis, StepTimingAvailability.ReportIdentityMismatch, result.SampleName);
                }

                List<VisionPipelineStepRunReport> steps = (report.Steps ?? new List<VisionPipelineStepRunReport>())
                    .Where(step => step != null)
                    .OrderBy(step => step.Index)
                    .ToList();
                if (steps.Count == 0 || steps.Select(step => step.Index).Distinct().Count() != steps.Count)
                {
                    return Unavailable(analysis, StepTimingAvailability.StepDefinitionMismatch, result.SampleName);
                }

                if (reportSteps.Count > 0 && !HaveEquivalentStepDefinitions(reportSteps[0], steps))
                {
                    return Unavailable(analysis, StepTimingAvailability.StepDefinitionMismatch, result.SampleName);
                }

                reportSteps.Add(steps);
            }

            List<VisionPipelineStepRunReport> referenceSteps = reportSteps[0]
                .Where(step => step.Enabled)
                .ToList();
            if (referenceSteps.Count == 0)
            {
                return Unavailable(analysis, StepTimingAvailability.NoEnabledSteps, string.Empty);
            }

            for (int stepPosition = 0; stepPosition < reportSteps[0].Count; stepPosition++)
            {
                VisionPipelineStepRunReport reference = reportSteps[0][stepPosition];
                if (!reference.Enabled)
                {
                    continue;
                }

                List<double> timings = reportSteps
                    .Select(steps => steps[stepPosition].ElapsedMilliseconds)
                    .Where(IsPositiveFinite)
                    .OrderBy(value => value)
                    .ToList();
                BatchStepTimingStatistics stepStatistics = new BatchStepTimingStatistics
                {
                    Index = reference.Index,
                    Name = reference.Name ?? string.Empty,
                    ToolType = reference.ToolType ?? string.Empty,
                    ReportCount = reportSteps.Count,
                    TimingCount = timings.Count
                };
                if (timings.Count > 0)
                {
                    int p95Index = Math.Max(0, (int)Math.Ceiling(timings.Count * 0.95D) - 1);
                    stepStatistics.AverageMilliseconds = timings.Average();
                    stepStatistics.P95Milliseconds = timings[p95Index];
                    stepStatistics.MaximumMilliseconds = timings[timings.Count - 1];
                }

                analysis.Steps.Add(stepStatistics);
            }

            if (!analysis.Steps.Any(step => step.TimingCount > 0))
            {
                return Unavailable(analysis, StepTimingAvailability.NoStepTimings, string.Empty);
            }

            analysis.Availability = StepTimingAvailability.Available;
            analysis.Steps = analysis.Steps
                .OrderByDescending(step => step.P95Milliseconds)
                .ThenByDescending(step => step.MaximumMilliseconds)
                .ThenBy(step => step.Index)
                .ToList();
            return analysis;
        }

        private static BatchStepTimingAnalysis Unavailable(
            BatchStepTimingAnalysis analysis,
            StepTimingAvailability availability,
            string detail)
        {
            analysis.Availability = availability;
            analysis.Detail = detail?.Trim() ?? string.Empty;
            analysis.Steps.Clear();
            return analysis;
        }

        private static bool HaveEquivalentStepDefinitions(
            IReadOnlyList<VisionPipelineStepRunReport> expected,
            IReadOnlyList<VisionPipelineStepRunReport> actual)
        {
            if (expected == null || actual == null || expected.Count != actual.Count)
            {
                return false;
            }

            for (int index = 0; index < expected.Count; index++)
            {
                VisionPipelineStepRunReport left = expected[index];
                VisionPipelineStepRunReport right = actual[index];
                if (left.Index != right.Index
                    || left.Enabled != right.Enabled
                    || !string.Equals(left.Name ?? string.Empty, right.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(left.ToolType ?? string.Empty, right.ToolType ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(left.InputLayer ?? string.Empty, right.InputLayer ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(left.OutputLayer ?? string.Empty, right.OutputLayer ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsPositiveFinite(double value)
        {
            return value > 0D && !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static string CreateUniqueBatchName(string recipeName, string pipelineName, DateTime startedAt)
        {
            string baseName = startedAt.ToString("yyyyMMdd_HHmmssfff");
            string rootDirectory = RecipeWorkspaceService.GetVisionPipelineBatchRunRootDirectory(recipeName, pipelineName);
            string candidate = baseName;
            int suffix = 2;
            while (Directory.Exists(
                RecipeWorkspaceService.GetContainedStoragePath(
                    rootDirectory,
                    candidate,
                    "Batch run directory")))
            {
                candidate = $"{baseName}_{suffix++}";
            }

            return candidate;
        }

        private static IEnumerable<string> CreateTsvLines(VisionPipelineBatchRunSummary summary)
        {
            yield return "SampleName\tStatus\tSuccess\tOutcomeSchemaVersion\tExecutionState\tHasJudgment\tExpectedOutcome\tActualOutcome\tJudgmentCorrect\tTotalMilliseconds\tFailedStep\tMessage\tReportPath\tSampleImagePath\tPairGroup\tPairRole\tVariantId\tExpectedMetricName\tExpectedMetricMinimum\tExpectedMetricMaximum\tExpectedText\tMetricText\tMetricReviewText\tFinalLayer\tOverlayCount\tActionSummary\tRunReportPath\tReviewReasons";
            Dictionary<int, VisionPipelineBatchReviewQueueEntry> queueByIndex = (summary.ReviewQueue
                    ?? new List<VisionPipelineBatchReviewQueueEntry>())
                .Where(entry => entry != null)
                .GroupBy(entry => entry.ResultIndex)
                .ToDictionary(group => group.Key, group => group.First());
            for (int index = 0; index < summary.Results.Count; index++)
            {
                VisionPipelineBatchSampleRunResult result = summary.Results[index];
                queueByIndex.TryGetValue(index, out VisionPipelineBatchReviewQueueEntry queueEntry);
                yield return string.Join(
                    "\t",
                    Escape(result.SampleName),
                    Escape(result.Status),
                    result.Success,
                    result.OutcomeSchemaVersion,
                    Escape(result.ExecutionState),
                    result.HasJudgment,
                    Escape(result.ExpectedOutcome),
                    Escape(result.ActualOutcome),
                    result.JudgmentCorrect,
                    result.TotalMilliseconds.ToString("0.0"),
                    Escape(result.FailedStep),
                    Escape(result.Message),
                    Escape(result.ReportPath),
                    Escape(result.SampleImagePath),
                    Escape(result.PairGroup),
                    Escape(result.PairRole),
                    Escape(result.VariantId),
                    Escape(result.ExpectedMetricName),
                    Escape(result.ExpectedMetricMinimum),
                    Escape(result.ExpectedMetricMaximum),
                    Escape(result.ExpectedText),
                    Escape(result.MetricText),
                    Escape(result.MetricReviewText),
                    Escape(result.FinalLayer),
                    Escape(result.OverlayCount),
                    Escape(result.ActionSummary),
                    Escape(result.RunReportPath),
                    Escape(string.Join(";", queueEntry?.Reasons ?? new List<string>())));
            }
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("\t", "\\t")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
        }
    }
}
