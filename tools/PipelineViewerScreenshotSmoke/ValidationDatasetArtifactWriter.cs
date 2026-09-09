using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

internal static class ValidationDatasetArtifactWriter
{
    internal static void WriteValidationDatasetCsv(
        string path,
        IReadOnlyList<VisionPipelineBatchSampleRunResult> results)
    {
        static string Escape(string? value)
        {
            string text = value ?? string.Empty;
            return "\"" + text.Replace("\"", "\"\"") + "\"";
        }

        List<string> lines = new List<string>
        {
            "SampleName,Expected,Actual,Judgment,PipelineAccepted,ResultCount,ScoreMax,AreaMin,AreaMax,AreaAvg,BoundsWidthMax,BoundsHeightMax,MeanValueAvg,DifferencePixelCount,DifferencePixelRatio,DifferenceMean,RegistrationInliers,RegistrationInlierRatio,RegistrationScore,ReferenceIndex,ValidPixelRatio,AcceptanceMessage,FailedStep,MetricText,ElapsedMilliseconds,ImagePath"
        };
        foreach (VisionPipelineBatchSampleRunResult result in results ?? Array.Empty<VisionPipelineBatchSampleRunResult>())
        {
            bool expectedOk = string.Equals(result?.PairRole, "OK", StringComparison.OrdinalIgnoreCase);
            bool actualOk = result?.Success == true;
            VisionPipelineRunReport? report = !string.IsNullOrWhiteSpace(result?.RunReportPath)
                ? VisionPipelineRunReportStorage.Load(result.RunReportPath)
                : null;
            VisionPipelineStepRunReport? finalStep = report?.Steps?
                .LastOrDefault(step => step != null && step.Enabled && !step.Skipped);
            double? resultCount = finalStep?.Metrics?
                .FirstOrDefault(metric => string.Equals(metric?.Name, "ResultCount", StringComparison.OrdinalIgnoreCase))
                ?.Value;
            double? scoreMax = finalStep?.Metrics?
                .FirstOrDefault(metric => string.Equals(metric?.Name, "ScoreMax", StringComparison.OrdinalIgnoreCase))
                ?.Value;
            double? areaMin = FindMetric(finalStep, "AreaMin");
            double? areaMax = FindMetric(finalStep, "AreaMax");
            double? areaAvg = FindMetric(finalStep, "AreaAvg");
            double? boundsWidthMax = FindMetric(finalStep, "BoundsWidthMax");
            double? boundsHeightMax = FindMetric(finalStep, "BoundsHeightMax");
            double? meanValueAvg = FindMetric(finalStep, "MeanValueAvg");
            double? differencePixelCount = FindMetric(finalStep, "DifferencePixelCount");
            double? differencePixelRatio = FindMetric(finalStep, "DifferencePixelRatio");
            double? differenceMean = FindMetric(finalStep, "DifferenceMean");
            double? registrationInliers = FindMetric(finalStep, "RegistrationInliers");
            double? registrationInlierRatio = FindMetric(finalStep, "RegistrationInlierRatio");
            double? registrationScore = FindMetric(finalStep, "RegistrationScore");
            double? referenceIndex = FindMetric(finalStep, "ReferenceIndex");
            double? validPixelRatio = FindMetric(finalStep, "ValidPixelRatio");
            string judgment = expectedOk
                ? actualOk ? "CorrectAccept" : "FalseReject"
                : actualOk ? "FalseAccept" : "CorrectReject";
            lines.Add(string.Join(",", new[]
            {
                Escape(result?.SampleName),
                Escape(expectedOk ? "OK" : "NG"),
                Escape(actualOk ? "OK" : "NG"),
                Escape(judgment),
                Escape(report?.Success == true ? "true" : "false"),
                resultCount?.ToString("0.###", CultureInfo.InvariantCulture) ?? string.Empty,
                scoreMax?.ToString("0.###", CultureInfo.InvariantCulture) ?? string.Empty,
                areaMin?.ToString("0.###", CultureInfo.InvariantCulture) ?? string.Empty,
                areaMax?.ToString("0.###", CultureInfo.InvariantCulture) ?? string.Empty,
                areaAvg?.ToString("0.###", CultureInfo.InvariantCulture) ?? string.Empty,
                boundsWidthMax?.ToString("0.###", CultureInfo.InvariantCulture) ?? string.Empty,
                boundsHeightMax?.ToString("0.###", CultureInfo.InvariantCulture) ?? string.Empty,
                meanValueAvg?.ToString("0.###", CultureInfo.InvariantCulture) ?? string.Empty,
                differencePixelCount?.ToString("0.###", CultureInfo.InvariantCulture) ?? string.Empty,
                differencePixelRatio?.ToString("0.######", CultureInfo.InvariantCulture) ?? string.Empty,
                differenceMean?.ToString("0.###", CultureInfo.InvariantCulture) ?? string.Empty,
                registrationInliers?.ToString("0.###", CultureInfo.InvariantCulture) ?? string.Empty,
                registrationInlierRatio?.ToString("0.######", CultureInfo.InvariantCulture) ?? string.Empty,
                registrationScore?.ToString("0.###", CultureInfo.InvariantCulture) ?? string.Empty,
                referenceIndex?.ToString("0", CultureInfo.InvariantCulture) ?? string.Empty,
                validPixelRatio?.ToString("0.######", CultureInfo.InvariantCulture) ?? string.Empty,
                Escape(finalStep?.AcceptanceMessage),
                Escape(result?.FailedStep),
                Escape(result?.MetricText),
                (result?.TotalMilliseconds ?? 0D).ToString("0.###", CultureInfo.InvariantCulture),
                Escape(result?.SampleImagePath)
            }));
        }

        File.WriteAllLines(path, lines);
    }

    internal static void WriteMisclassificationEvidence(
        string artifactDirectory,
        IReadOnlyList<VisionPipelineBatchSampleRunResult> results)
    {
        string evidenceRoot = Path.Combine(artifactDirectory, "misclassification_evidence");
        Directory.CreateDirectory(evidenceRoot);
        List<string> manifest = new List<string>
        {
            "EvidenceId,Expected,Actual,Judgment,SampleName,OriginalImage,DrawingImage,RunReport,FailedStep,Message"
        };
        int evidenceIndex = 0;
        foreach (VisionPipelineBatchSampleRunResult result in results ?? Array.Empty<VisionPipelineBatchSampleRunResult>())
        {
            bool expectedOk = string.Equals(result?.PairRole, "OK", StringComparison.OrdinalIgnoreCase);
            bool actualOk = result?.Success == true;
            if (result == null || expectedOk == actualOk)
            {
                continue;
            }

            evidenceIndex++;
            string evidenceId = evidenceIndex.ToString("000", CultureInfo.InvariantCulture)
                + "_"
                + (expectedOk ? "FalseReject" : "FalseAccept")
                + "_"
                + SanitizeArtifactFileName(Path.GetFileNameWithoutExtension(result.SampleName));
            string sampleDirectory = Path.Combine(evidenceRoot, evidenceId);
            Directory.CreateDirectory(sampleDirectory);

            string originalFile = CopyArtifactFile(result.SampleImagePath, sampleDirectory, "original");
            string runReportFile = CopyArtifactFile(result.RunReportPath, sampleDirectory, "run_report");
            VisionPipelineRunReport? report = string.IsNullOrWhiteSpace(result.RunReportPath)
                ? null
                : VisionPipelineRunReportStorage.Load(result.RunReportPath);
            string reportDirectory = string.IsNullOrWhiteSpace(result.RunReportPath)
                ? string.Empty
                : Path.GetDirectoryName(result.RunReportPath) ?? string.Empty;
            VisionPipelineStepRunReport? displayStep = report?.Steps?
                .Where(step => step != null)
                .Reverse()
                .FirstOrDefault(step => !string.IsNullOrWhiteSpace(ResolveReportArtifactPath(reportDirectory, step.OverlayImageFile))
                    || !string.IsNullOrWhiteSpace(ResolveReportArtifactPath(reportDirectory, step.ResultImageFile)));
            string drawingPath = displayStep == null
                ? string.Empty
                : FirstExistingArtifactPath(
                    ResolveReportArtifactPath(reportDirectory, displayStep.OverlayImageFile),
                    ResolveReportArtifactPath(reportDirectory, displayStep.ResultImageFile));
            string drawingFile = CopyArtifactFile(drawingPath, sampleDirectory, "drawing");

            manifest.Add(string.Join(",", new[]
            {
                EscapeCsv(evidenceId),
                EscapeCsv(expectedOk ? "OK" : "NG"),
                EscapeCsv(actualOk ? "OK" : "NG"),
                EscapeCsv(expectedOk ? "FalseReject" : "FalseAccept"),
                EscapeCsv(result.SampleName),
                EscapeCsv(originalFile),
                EscapeCsv(drawingFile),
                EscapeCsv(runReportFile),
                EscapeCsv(result.FailedStep),
                EscapeCsv(result.Message)
            }));
        }

        File.WriteAllLines(Path.Combine(evidenceRoot, "manifest.csv"), manifest);
        File.WriteAllText(
            Path.Combine(evidenceRoot, "README.md"),
            "# Misclassification drawing evidence" + Environment.NewLine + Environment.NewLine
            + "Each child folder contains the original sample, persisted detection drawing, and Run Report for one False Reject or False Accept." + Environment.NewLine
            + "The runner copies this evidence before it cleans its reserved Smoke recipe workspace." + Environment.NewLine
            + "Rows: " + evidenceIndex.ToString(CultureInfo.InvariantCulture) + Environment.NewLine);
    }

    internal static void WriteValidationDatasetArtifacts(
        string artifactDirectory,
        string pipelineXml,
        VisionPipelineBatchRunSummary summary,
        string datasetRoot,
        string templatePath,
        string pipelineName,
        string boundary)
    {
        File.WriteAllText(Path.Combine(artifactDirectory, "pipeline.xml"), pipelineXml);
        File.WriteAllText(
            Path.Combine(artifactDirectory, "batch_summary.json"),
            JsonSerializer.Serialize(summary, new JsonSerializerOptions { WriteIndented = true }));
        WriteValidationDatasetCsv(
            Path.Combine(artifactDirectory, "misclassification_table.csv"),
            summary.Results);
        WriteMisclassificationEvidence(artifactDirectory, summary.Results);
        File.WriteAllText(
            Path.Combine(artifactDirectory, "audit_summary.json"),
            JsonSerializer.Serialize(
                new
                {
                    DatasetRoot = datasetRoot,
                    TemplatePath = templatePath,
                    Pipeline = pipelineName,
                    Total = summary.Results.Count,
                    CorrectAccept = summary.Results.Count(result => IsDatasetJudgment(result, expectedOk: true, actualOk: true)),
                    FalseReject = summary.Results.Count(result => IsDatasetJudgment(result, expectedOk: true, actualOk: false)),
                    FalseAccept = summary.Results.Count(result => IsDatasetJudgment(result, expectedOk: false, actualOk: true)),
                    CorrectReject = summary.Results.Count(result => IsDatasetJudgment(result, expectedOk: false, actualOk: false)),
                    AverageMilliseconds = summary.Results.Average(result => result.TotalMilliseconds),
                    Boundary = boundary
                },
                new JsonSerializerOptions { WriteIndented = true }));
    }

    private static string CopyArtifactFile(string sourcePath, string destinationDirectory, string destinationStem)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
        {
            return string.Empty;
        }

        string extension = Path.GetExtension(sourcePath);
        string destinationFile = destinationStem + (string.IsNullOrWhiteSpace(extension) ? string.Empty : extension);
        File.Copy(sourcePath, Path.Combine(destinationDirectory, destinationFile), overwrite: true);
        return destinationFile;
    }

    private static string ResolveReportArtifactPath(string reportDirectory, string imageFile)
    {
        if (string.IsNullOrWhiteSpace(imageFile))
        {
            return string.Empty;
        }

        string path = Path.IsPathRooted(imageFile)
            ? imageFile
            : Path.Combine(reportDirectory, imageFile);
        return File.Exists(path) ? path : string.Empty;
    }

    private static string FirstExistingArtifactPath(params string[] paths)
    {
        return paths?.FirstOrDefault(path => !string.IsNullOrWhiteSpace(path) && File.Exists(path)) ?? string.Empty;
    }

    private static string SanitizeArtifactFileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Sample";
        }

        char[] invalid = Path.GetInvalidFileNameChars();
        string sanitized = new string(value.Select(character => invalid.Contains(character) ? '_' : character).ToArray());
        return string.IsNullOrWhiteSpace(sanitized) ? "Sample" : sanitized;
    }

    private static string EscapeCsv(string? value)
    {
        string text = value ?? string.Empty;
        return "\"" + text.Replace("\"", "\"\"") + "\"";
    }

    private static double? FindMetric(VisionPipelineStepRunReport? step, string name)
    {
        return step?.Metrics?
            .FirstOrDefault(metric => string.Equals(metric?.Name, name, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }

    private static bool IsDatasetJudgment(
        VisionPipelineBatchSampleRunResult result,
        bool expectedOk,
        bool actualOk)
    {
        return result != null
            && string.Equals(result.PairRole, expectedOk ? "OK" : "NG", StringComparison.OrdinalIgnoreCase)
            && result.Success == actualOk;
    }
}
