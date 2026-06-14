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
    }

    internal static class VisionPipelineBatchRunSummaryStorage
    {
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
            IEnumerable<VisionPipelineBatchSampleRunResult> results)
        {
            List<VisionPipelineBatchSampleRunResult> resultList = (results ?? Enumerable.Empty<VisionPipelineBatchSampleRunResult>()).ToList();
            string batchName = CreateUniqueBatchName(recipeName, pipelineName, startedAt);
            string directory = RecipeWorkspaceService.GetVisionPipelineBatchRunDirectory(recipeName, pipelineName, batchName);

            VisionPipelineBatchRunSummary summary = new VisionPipelineBatchRunSummary
            {
                RecipeName = recipeName ?? string.Empty,
                PipelineName = pipelineName ?? string.Empty,
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
            yield return "SampleName\tStatus\tSuccess\tTotalMilliseconds\tFailedStep\tMessage\tReportPath";
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
                    Escape(result.ReportPath));
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
