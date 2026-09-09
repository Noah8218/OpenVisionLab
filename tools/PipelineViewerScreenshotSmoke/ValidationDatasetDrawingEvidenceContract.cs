using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class ValidationDatasetDrawingEvidenceContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl34-validation-dataset-drawing-evidence-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Validation dataset drawing evidence contract must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "ValidationDatasetDrawingEvidence.cs");
        string program = File.ReadAllText(programPath);
        string owner = File.ReadAllText(ownerPath);
        string targetMethod = ExtractRegion(
            program,
            "    private static CaptureResult CaptureShellHostRecipeLocalValidationDataset(",
            "    private static void AssertReviewQueueMetricExtremes(");

        Check(
            "Program delegates selected-run drawing verification to the concrete owner",
            targetMethod.Contains("ValidationDatasetDrawingEvidence.VerifyAndWrite(", StringComparison.Ordinal)
                && targetMethod.Contains("configuration.PipelineXml", StringComparison.Ordinal)
                && targetMethod.Contains("okPaths[0]", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Program no longer owns stored drawing resolution or viewer composition",
            !targetMethod.Contains("OpenVisionRecipeRunEvidence.TryCreate", StringComparison.Ordinal)
                && !targetMethod.Contains("OpenVisionRecipeRunEvidenceViewerView", StringComparison.Ordinal)
                && !targetMethod.Contains("OpenVisionFloatingToolWindow", StringComparison.Ordinal)
                && !targetMethod.Contains("drawing_evidence_contract.txt", StringComparison.Ordinal)
                && !program.Contains("AssertExecutedPinArrayGapFailurePreservesPriorRowDrawing(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Owner contains source verification, selector probing, artifact copy, and workspace guards",
            owner.Contains("OpenVisionRecipeRunEvidence.TryCreate", StringComparison.Ordinal)
                && owner.Contains("IsStoredSourceVerified", StringComparison.Ordinal)
                && owner.Contains("cmbStoredDrawing", StringComparison.Ordinal)
                && owner.Contains("OpenSelectedRecentBatchRunEvidenceCommand", StringComparison.Ordinal)
                && owner.Contains("Application.Current.Windows", StringComparison.Ordinal)
                && owner.Contains("File.Copy", StringComparison.Ordinal)
                && owner.Contains("LayerDocumentCount", StringComparison.Ordinal)
                && owner.Contains("drawing_evidence_contract.txt", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Owner retains executed-failure drawing coverage in the same evidence boundary",
            owner.Contains("AssertExecutedPinArrayGapFailurePreservesPriorRowDrawing", StringComparison.Ordinal)
                && owner.Contains("ExecutedRowDrawingBoundary", StringComparison.Ordinal)
                && owner.Contains("DefaultDrawing?.Index != 2", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Contract projection is a pure owner method",
            owner.Contains("BuildContractLines(", StringComparison.Ordinal)
                && owner.Contains("StoredSourceVerified=", StringComparison.Ordinal)
                && owner.Contains("ViewerReady=", StringComparison.Ordinal)
                && owner.Contains("WorkspaceUnchanged=", StringComparison.Ordinal),
            passed,
            failed);

        IReadOnlyList<string> lines = ValidationDatasetDrawingEvidence.BuildContractLines(
            "NG sample",
            drawingCount: 2,
            storedSourceVerified: true,
            viewerReady: true,
            workspaceUnchanged: true);
        Check(
            "Stored evidence identity and viewer/workspace invariants are projected together",
            lines.SequenceEqual(
                new[]
                {
                    "Sample=NG sample",
                    "DrawingCount=2",
                    "StoredSourceVerified=True",
                    "ViewerReady=True",
                    "WorkspaceUnchanged=True"
                },
                StringComparer.Ordinal),
            passed,
            failed);

        string outputPath = Path.Combine(evidenceDirectory, "drawing_evidence_contract.txt");
        File.WriteAllLines(outputPath, lines);
        Check(
            "Drawing evidence projection is persisted without changing its values",
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
        string reportPath = Path.Combine(evidenceDirectory, "validation-dataset-drawing-evidence-contract.txt");
        File.WriteAllLines(reportPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("VALIDATION_DATASET_DRAWING_EVIDENCE_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("VALIDATION_DATASET_DRAWING_EVIDENCE_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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
            throw new InvalidOperationException("Validation dataset drawing evidence target region was not found.");
        }

        return source.Substring(start, end - start);
    }
}
