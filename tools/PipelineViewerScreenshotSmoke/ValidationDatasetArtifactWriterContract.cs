using OpenVisionLab;
using OpenVisionLab.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class ValidationDatasetArtifactWriterContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl27-validation-dataset-artifact-writer-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Validation dataset artifact contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "ValidationDatasetArtifactWriter.cs");
        string program = File.ReadAllText(programPath);
        string owner = File.ReadAllText(ownerPath);

        Check(
            "Program delegates validation dataset artifact output to the writer",
            program.Contains("ValidationDatasetArtifactWriter.WriteValidationDatasetArtifacts(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Program no longer declares the moved dataset report methods",
            !program.Contains("private static void WriteValidationDatasetCsv(", StringComparison.Ordinal)
                && !program.Contains("private static void WriteMisclassificationEvidence(", StringComparison.Ordinal)
                && !program.Contains("private static string CopyArtifactFile(", StringComparison.Ordinal)
                && !program.Contains("private static string ResolveReportArtifactPath(", StringComparison.Ordinal)
                && !program.Contains("private static string FirstExistingArtifactPath(", StringComparison.Ordinal)
                && !program.Contains("private static string SanitizeArtifactFileName(", StringComparison.Ordinal)
                && !program.Contains("private static string EscapeCsv(", StringComparison.Ordinal)
                && !program.Contains("private static double? FindMetric(", StringComparison.Ordinal)
                && !program.Contains("private static bool IsDatasetJudgment(", StringComparison.Ordinal)
                && !program.Contains("Path.Combine(artifactDirectory, \"audit_summary.json\")", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Writer owns report loading, artifact copying, and CSV/evidence output",
            owner.Contains("VisionPipelineRunReportStorage.Load", StringComparison.Ordinal)
                && owner.Contains("File.Copy", StringComparison.Ordinal)
                && owner.Contains("File.WriteAllLines", StringComparison.Ordinal)
                && owner.Contains("misclassification_evidence", StringComparison.Ordinal)
                && owner.Contains("EscapeCsv", StringComparison.Ordinal)
                && owner.Contains("JsonSerializer.Serialize", StringComparison.Ordinal)
                && owner.Contains("audit_summary.json", StringComparison.Ordinal)
                && owner.Contains("IsDatasetJudgment", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Writer has no dependency on the smoke entry point or WPF view types",
            !owner.Contains("Program", StringComparison.Ordinal)
                && !owner.Contains("System.Windows", StringComparison.Ordinal)
                && !owner.Contains("OpenVisionShellHost", StringComparison.Ordinal),
            passed,
            failed);

        string runDirectory = Path.Combine(evidenceDirectory, "run");
        string outputDirectory = Path.Combine(evidenceDirectory, "dataset");
        Directory.CreateDirectory(runDirectory);
        Directory.CreateDirectory(outputDirectory);
        string samplePath = Path.Combine(runDirectory, "sample.png");
        string overlayPath = Path.Combine(runDirectory, "overlay.png");
        string reportPath = Path.Combine(runDirectory, "report.xml");
        File.WriteAllBytes(samplePath, new byte[] { 1, 2, 3 });
        File.WriteAllBytes(overlayPath, new byte[] { 4, 5, 6 });
        SerializeHelper.SaveXmlFile(
            reportPath,
            new VisionPipelineRunReport
            {
                Success = false,
                Steps = new List<VisionPipelineStepRunReport>
                {
                    new VisionPipelineStepRunReport
                    {
                        Index = 1,
                        Enabled = true,
                        Skipped = false,
                        AcceptanceMessage = "Synthetic acceptance message",
                        OverlayImageFile = "overlay.png",
                        Metrics = new List<VisionPipelineMetricRunReport>
                        {
                            new VisionPipelineMetricRunReport { Name = "ResultCount", Value = 3D },
                            new VisionPipelineMetricRunReport { Name = "ScoreMax", Value = 0.75D },
                            new VisionPipelineMetricRunReport { Name = "AreaMin", Value = 12D }
                        }
                    }
                }
            });

        VisionPipelineBatchSampleRunResult result = new VisionPipelineBatchSampleRunResult
        {
            SampleName = "fixture:sample",
            PairRole = "OK",
            Success = false,
            TotalMilliseconds = 12.5D,
            FailedStep = "01 Fixture [ERROR]",
            Message = "Synthetic false reject",
            MetricText = "ScoreMax=0.75",
            SampleImagePath = samplePath,
            RunReportPath = reportPath
        };
        string csvPath = Path.Combine(outputDirectory, "misclassification_table.csv");
        ValidationDatasetArtifactWriter.WriteValidationDatasetCsv(csvPath, new[] { result });
        ValidationDatasetArtifactWriter.WriteMisclassificationEvidence(outputDirectory, new[] { result });

        string csv = File.ReadAllText(csvPath);
        string evidenceRoot = Path.Combine(outputDirectory, "misclassification_evidence");
        string evidenceDirectoryPath = Directory.GetDirectories(evidenceRoot).Single();
        string manifest = File.ReadAllText(Path.Combine(evidenceRoot, "manifest.csv"));
        string readme = File.ReadAllText(Path.Combine(evidenceRoot, "README.md"));
        Check(
            "CSV output preserves judgment, report metrics, and source path",
            csv.Contains("FalseReject", StringComparison.Ordinal)
                && csv.Contains("3", StringComparison.Ordinal)
                && csv.Contains("0.75", StringComparison.Ordinal)
                && csv.Contains(samplePath, StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "misclassification evidence copies original, drawing, and run report artifacts",
            File.Exists(Path.Combine(evidenceDirectoryPath, "original.png"))
                && File.Exists(Path.Combine(evidenceDirectoryPath, "drawing.png"))
                && File.Exists(Path.Combine(evidenceDirectoryPath, "run_report.xml")),
            passed,
            failed);
        Check(
            "evidence manifest and README retain the false reject identity",
            manifest.Contains("FalseReject", StringComparison.Ordinal)
                && manifest.Contains("fixture:sample", StringComparison.Ordinal)
                && readme.Contains("Rows: 1", StringComparison.Ordinal),
            passed,
            failed);

        VisionPipelineBatchRunSummary batchSummary = new VisionPipelineBatchRunSummary
        {
            RecipeName = "Contract_Recipe",
            PipelineName = "Contract_Pipeline",
            Results = new List<VisionPipelineBatchSampleRunResult>
            {
                new VisionPipelineBatchSampleRunResult { SampleName = "correct-accept", PairRole = "OK", Success = true, TotalMilliseconds = 1D },
                new VisionPipelineBatchSampleRunResult { SampleName = "false-reject", PairRole = "OK", Success = false, TotalMilliseconds = 2D },
                new VisionPipelineBatchSampleRunResult { SampleName = "false-accept", PairRole = "NG", Success = true, TotalMilliseconds = 3D },
                new VisionPipelineBatchSampleRunResult { SampleName = "correct-reject", PairRole = "NG", Success = false, TotalMilliseconds = 4D }
            }
        };
        const string pipelineXml = "<VisionPipeline><Name>Contract_Pipeline</Name></VisionPipeline>";
        ValidationDatasetArtifactWriter.WriteValidationDatasetArtifacts(
            outputDirectory,
            pipelineXml,
            batchSummary,
            "contract-dataset",
            "contract-template.png",
            "Contract_Pipeline",
            "contract-boundary");
        string persistedPipelineXml = File.ReadAllText(Path.Combine(outputDirectory, "pipeline.xml"));
        string persistedBatchSummary = File.ReadAllText(Path.Combine(outputDirectory, "batch_summary.json"));
        string persistedAuditSummary = File.ReadAllText(Path.Combine(outputDirectory, "audit_summary.json"));
        Check(
            "summary writer persists the pipeline XML and batch summary",
            string.Equals(persistedPipelineXml, pipelineXml, StringComparison.Ordinal)
                && persistedBatchSummary.Contains("\"RecipeName\": \"Contract_Recipe\"", StringComparison.Ordinal)
                && persistedBatchSummary.Contains("correct-accept", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "audit summary preserves all four expected/actual judgments and boundary",
            persistedAuditSummary.Contains("\"Total\": 4", StringComparison.Ordinal)
                && persistedAuditSummary.Contains("\"CorrectAccept\": 1", StringComparison.Ordinal)
                && persistedAuditSummary.Contains("\"FalseReject\": 1", StringComparison.Ordinal)
                && persistedAuditSummary.Contains("\"FalseAccept\": 1", StringComparison.Ordinal)
                && persistedAuditSummary.Contains("\"CorrectReject\": 1", StringComparison.Ordinal)
                && persistedAuditSummary.Contains("\"Boundary\": \"contract-boundary\"", StringComparison.Ordinal),
            passed,
            failed);

        List<string> reportLines = new List<string>
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "ChecksPassed: " + passed.Count,
            "ChecksFailed: " + failed.Count
        };
        reportLines.AddRange(passed.Select(item => "PASS: " + item));
        reportLines.AddRange(failed.Select(item => "FAIL: " + item));
        string contractReportPath = Path.Combine(evidenceDirectory, "validation-dataset-artifact-writer-contract.txt");
        File.WriteAllLines(contractReportPath, reportLines);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("VALIDATION_DATASET_ARTIFACT_WRITER_CONTRACT=FAIL|report=" + contractReportPath);
            return 1;
        }

        Console.WriteLine("VALIDATION_DATASET_ARTIFACT_WRITER_CONTRACT=PASS|checks=" + passed.Count + "|report=" + contractReportPath);
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
}
