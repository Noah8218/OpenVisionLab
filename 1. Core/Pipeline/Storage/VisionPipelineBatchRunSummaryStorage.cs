using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    public sealed class VisionPipelineBatchRunSummary
    {
        public string RecipeName { get; set; } = string.Empty;
        public string PipelineName { get; set; } = string.Empty;
        public string SuiteName { get; set; } = string.Empty;
        public string SuiteKind { get; set; } = string.Empty;
        public string PipelineSnapshotFile { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string StartedAt { get; set; } = string.Empty;
        public string FinishedAt { get; set; } = string.Empty;
        public double TotalMilliseconds { get; set; }
        public int TotalCount { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
        public List<VisionPipelineBatchSampleRunResult> Results { get; set; } = new List<VisionPipelineBatchSampleRunResult>();
    }

    public sealed class VisionPipelineBatchSampleRunResult
    {
        public string SampleName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool Success { get; set; }
        public double TotalMilliseconds { get; set; }
        public string FailedStep { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string ReportPath { get; set; } = string.Empty;
        public string SampleImagePath { get; set; } = string.Empty;
        public string PairGroup { get; set; } = string.Empty;
        public string PairRole { get; set; } = string.Empty;
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
            string notes = "")
        {
            List<VisionPipelineBatchSampleRunResult> resultList = (results ?? Enumerable.Empty<VisionPipelineBatchSampleRunResult>()).ToList();
            string batchName = CreateUniqueBatchName(recipeName, pipelineName, startedAt);
            string directory = RecipeWorkspaceService.GetVisionPipelineBatchRunDirectory(recipeName, pipelineName, batchName);

            VisionPipelineBatchRunSummary summary = new VisionPipelineBatchRunSummary
            {
                RecipeName = recipeName ?? string.Empty,
                PipelineName = pipelineName ?? string.Empty,
                SuiteName = string.IsNullOrWhiteSpace(suiteName) ? "Validation Suite" : suiteName.Trim(),
                SuiteKind = string.IsNullOrWhiteSpace(suiteKind) ? "Batch" : suiteKind.Trim(),
                Notes = notes?.Trim() ?? string.Empty,
                StartedAt = startedAt.ToString("o"),
                FinishedAt = finishedAt.ToString("o"),
                TotalMilliseconds = (finishedAt - startedAt).TotalMilliseconds,
                TotalCount = resultList.Count,
                PassCount = resultList.Count(result => result.Success),
                FailCount = resultList.Count(result => !result.Success),
                Results = resultList
            };

            string xmlPath = Path.Combine(directory, "summary.xml");
            SerializeHelper.SaveXmlFile(xmlPath, summary);
            File.WriteAllLines(Path.Combine(directory, "summary.tsv"), CreateTsvLines(summary));
            return xmlPath;
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
                string summaryPath = Path.Combine(directory, "summary.xml");
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
            while (Directory.Exists(Path.Combine(rootDirectory, candidate)))
            {
                candidate = $"{baseName}_{suffix++}";
            }

            return candidate;
        }

        private static IEnumerable<string> CreateTsvLines(VisionPipelineBatchRunSummary summary)
        {
            yield return "SampleName\tStatus\tSuccess\tTotalMilliseconds\tFailedStep\tMessage\tReportPath\tSampleImagePath\tPairGroup\tPairRole\tExpectedText\tMetricText\tMetricReviewText\tFinalLayer\tOverlayCount\tActionSummary\tRunReportPath";
            foreach (VisionPipelineBatchSampleRunResult result in summary.Results)
            {
                yield return string.Join(
                    "\t",
                    Escape(result.SampleName),
                    Escape(result.Status),
                    result.Success,
                    result.TotalMilliseconds.ToString("0.0"),
                    Escape(result.FailedStep),
                    Escape(result.Message),
                    Escape(result.ReportPath),
                    Escape(result.SampleImagePath),
                    Escape(result.PairGroup),
                    Escape(result.PairRole),
                    Escape(result.ExpectedText),
                    Escape(result.MetricText),
                    Escape(result.MetricReviewText),
                    Escape(result.FinalLayer),
                    Escape(result.OverlayCount),
                    Escape(result.ActionSummary),
                    Escape(result.RunReportPath));
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
