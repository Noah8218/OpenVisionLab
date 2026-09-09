using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class ValidationDatasetReviewQueueEvidenceContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl33-validation-dataset-review-queue-evidence-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Validation dataset review-queue evidence contract must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "ValidationDatasetReviewQueueEvidence.cs");
        string program = File.ReadAllText(programPath);
        string targetMethod = ExtractRegion(
            program,
            "    private static CaptureResult CaptureShellHostRecipeLocalValidationDataset(",
            "    private static void AssertReviewQueueMetricExtremes(");
        string owner = File.ReadAllText(ownerPath);

        Check(
            "Program delegates review-queue verification to the evidence owner",
            targetMethod.Contains("ValidationDatasetReviewQueueEvidence.VerifyAndWrite(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Program no longer owns the review-queue state and artifact block",
            !targetMethod.Contains("ShowRecentBatchReviewQueueOnly", StringComparison.Ordinal)
                && !targetMethod.Contains("review_queue_contract.txt", StringComparison.Ordinal)
                && !targetMethod.Contains("RecentBatchRunReviewQueueSummaryText", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Owner contains the review-queue filter, workspace guards, and panel evidence",
            owner.Contains("ShowRecentBatchReviewQueueOnly", StringComparison.Ordinal)
                && owner.Contains("RecentBatchRunReviewQueueSummaryText", StringComparison.Ordinal)
                && owner.Contains("BringIntoView", StringComparison.Ordinal)
                && owner.Contains("review_queue_contract.txt", StringComparison.Ordinal)
                && owner.Contains("File.Copy", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Contract output remains a named pure projection of persisted summary state",
            owner.Contains("BuildContractLines(", StringComparison.Ordinal)
                && owner.Contains("ReviewQueuePolicy", StringComparison.Ordinal)
                && owner.Contains("PreviewRunCountUnchanged", StringComparison.Ordinal)
                && owner.Contains("LayerCountUnchanged", StringComparison.Ordinal),
            passed,
            failed);

        VisionPipelineBatchRunSummary summary = new VisionPipelineBatchRunSummary
        {
            ReviewQueuePolicy = "v3|review-queue",
            ReviewQueueSha256 = new string('a', 64),
            Results = new List<VisionPipelineBatchSampleRunResult>
            {
                new VisionPipelineBatchSampleRunResult { SampleName = "ok" },
                new VisionPipelineBatchSampleRunResult { SampleName = "ng" }
            },
            ReviewQueue = new List<VisionPipelineBatchReviewQueueEntry>
            {
                new VisionPipelineBatchReviewQueueEntry
                {
                    SampleName = "ng",
                    Reasons = new List<string> { "PitchPx: 1.25" }
                }
            }
        };
        IReadOnlyList<string> lines = ValidationDatasetReviewQueueEvidence.BuildContractLines(
            summary,
            containsPitchMetric: true,
            previewRunsUnchanged: true,
            layerCountUnchanged: true);
        Check(
            "Persisted review-queue identity and workspace invariants are written together",
            lines.SequenceEqual(
                new[]
                {
                    "Policy=v3|review-queue",
                    "Sha256=" + new string('a', 64),
                    "Rows=1",
                    "Total=2",
                    "ContainsPitchMetric=True",
                    "PreviewRunCountUnchanged=True",
                    "LayerCountUnchanged=True"
                },
                StringComparer.Ordinal),
            passed,
            failed);

        string outputPath = Path.Combine(evidenceDirectory, "review_queue_contract.txt");
        File.WriteAllLines(outputPath, lines);
        Check(
            "Evidence projection is persisted without changing the summary values",
            File.Exists(outputPath)
                && File.ReadAllLines(outputPath).SequenceEqual(lines, StringComparer.Ordinal),
            passed,
            failed);

        List<string> report = new List<string>
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "ChecksPassed: " + passed.Count,
            "ChecksFailed: " + failed.Count
        };
        report.AddRange(passed.Select(item => "PASS: " + item));
        report.AddRange(failed.Select(item => "FAIL: " + item));
        string reportPath = Path.Combine(evidenceDirectory, "validation-dataset-review-queue-evidence-contract.txt");
        File.WriteAllLines(reportPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("VALIDATION_DATASET_REVIEW_QUEUE_EVIDENCE_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("VALIDATION_DATASET_REVIEW_QUEUE_EVIDENCE_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
        return 0;
    }

    private static void Check(string name, bool condition, ICollection<string> passed, ICollection<string> failed)
    {
        if (condition)
        {
            passed.Add(name);
        }
        else
        {
            failed.Add(name);
        }
    }

    private static string ResolveRepositoryRoot()
    {
        foreach (string start in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
        {
            DirectoryInfo? current = new DirectoryInfo(start);
            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, "src", "OpenVisionLab", "OpenVisionLab.csproj")))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }
        }

        throw new InvalidOperationException("OpenVisionLab repository root was not found.");
    }

    private static string ExtractRegion(string source, string startMarker, string endMarker)
    {
        int start = source.IndexOf(startMarker, StringComparison.Ordinal);
        int end = source.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
        if (start < 0 || end < 0)
        {
            throw new InvalidOperationException("Validation dataset review-queue target region was not found.");
        }

        return source.Substring(start, end - start);
    }
}
