using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal static class BlobContourCandidateBoundaryContract
{
    public static async Task<int> RunAsync(string evidenceDirectory)
    {
        string outputDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(outputDirectory);
        Environment.SetEnvironmentVariable(
            AppPathService.DataRootEnvironmentVariable,
            Path.Combine(outputDirectory, "runtime-data"));

        List<string> observations = new List<string>();
        List<string> failures = new List<string>();
        using Mat source = CreateSource();
        string sourcePath = Path.Combine(outputDirectory, "blob-contour-boundary-source.png");
        Require(Cv2.ImWrite(sourcePath, source), "Could not write the Blob/Contour boundary source.");

        foreach (string toolType in new[] { "Blob", "Contour" })
        {
            foreach (string boundary in new[] { "area", "dimension" })
            {
                try
                {
                    await VerifyBoundaryCaseAsync(
                        toolType,
                        boundary,
                        source,
                        outputDirectory,
                        observations).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    failures.Add($"{toolType}/{boundary}: {exception.GetBaseException().Message}");
                }
            }
        }

        string reportPath = Path.Combine(outputDirectory, "blob-contour-candidate-boundary-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-032 Blob/Contour candidate explanation and single-execution result parity",
                "Owner: existing Blob/Contour SDK candidates -> VisionPipelineObjectResultCaptureService -> metrics/overlays/Run History",
                "EvidenceDirectory: " + outputDirectory,
                "Source: " + sourcePath
            }
            .Concat(observations.Select(item => "PASS: " + item))
            .Concat(failures.Select(item => "FAIL: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Blob/Contour candidate boundary contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Blob/Contour candidate boundary contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static async Task VerifyBoundaryCaseAsync(
        string toolType,
        string boundary,
        Mat source,
        string evidenceDirectory,
        ICollection<string> observations)
    {
        const int targetWidth = 24;
        const int targetHeight = 32;
        int expectedArea = string.Equals(toolType, "Blob", StringComparison.Ordinal)
            ? targetWidth * targetHeight
            : (targetWidth - 1) * (targetHeight - 1);
        bool areaBoundary = string.Equals(boundary, "area", StringComparison.Ordinal);
        int minimumArea = areaBoundary ? expectedArea : 1;
        int maximumArea = areaBoundary ? expectedArea : 100000;
        int minimumWidth = areaBoundary ? 0 : targetWidth;
        int maximumWidth = areaBoundary ? 1000 : targetWidth;
        int minimumHeight = areaBoundary ? 0 : targetHeight;
        int maximumHeight = areaBoundary ? 1000 : targetHeight;
        VisionPipeline pipeline = CreatePipeline(
            toolType,
            minimumArea,
            maximumArea,
            minimumWidth,
            maximumWidth,
            minimumHeight,
            maximumHeight);
        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            pipeline,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        Require(validation.Success, $"{toolType}/{boundary} validation failed: {string.Join(" | ", validation.Errors)}");

        VisionRecipeRunner runner = new VisionRecipeRunner();
        string recipeName = "Smoke_2D032_" + toolType + "_" + boundary + "_" + Guid.NewGuid().ToString("N");
        DateTime startedAt = DateTime.UtcNow;
        try
        {
            using VisionRecipeRunResult run = await runner.RunAsync(pipeline, source).ConfigureAwait(false);
            VisionRecipeStepRunSummary? summary = run.Steps.SingleOrDefault();
            Require(run.Success && summary != null && summary.Success,
                $"{toolType}/{boundary} execution failed: {run.Message}");

            VisionRecipeStepRunSummary summaryValue = summary!;
            List<VisionPipelineObjectResult> rows = summaryValue.ObjectResults.ToList();
            List<VisionPipelineObjectResult> accepted = rows.Where(item => item.Accepted).ToList();
            List<VisionPipelineObjectResult> targetRows = rows
                .Where(item => item.BoundsWidth == targetWidth && item.BoundsHeight == targetHeight)
                .OrderBy(item => item.BoundsX)
                .ToList();
            Require(targetRows.Count == 4,
                $"{toolType}/{boundary} expected four same-area target rows, actual {targetRows.Count}.");
            Require(targetRows.All(item => item.Accepted && item.RejectReasonCode == "None"),
                $"{toolType}/{boundary} target rows were not all accepted with RejectReasonCode=None.");
            Require(targetRows.Select(item => item.Area).Distinct().Count() == 1
                && Math.Abs(targetRows[0].Area - expectedArea) <= 0.001D,
                $"{toolType}/{boundary} target area expected {expectedArea}, actual {FormatAreas(targetRows)}.");
            Require(targetRows.Select(item => item.RegionIndex).Distinct().OrderBy(item => item).SequenceEqual(new[] { 0, 1 }),
                $"{toolType}/{boundary} did not preserve RegionIndex 0/1 for target rows.");
            Require(targetRows.All(item => item.BoundsX < 200) == false
                && targetRows.Any(item => item.BoundsX < 200)
                && targetRows.Any(item => item.BoundsX >= 200),
                $"{toolType}/{boundary} target bounds did not remain in both source ROI halves.");

            Require(rows.Count == 12 && accepted.Count == 4,
                $"{toolType}/{boundary} expected 12 candidate rows and 4 accepted rows, actual rows={rows.Count}, accepted={accepted.Count}.");
            Require(rows.Select(item => item.CandidateId).Distinct(StringComparer.Ordinal).Count() == rows.Count,
                $"{toolType}/{boundary} candidate IDs were not unique.");
            Require(rows.All(item => item.NativeIndex >= 0
                && item.RegionIndex is 0 or 1
                && !string.IsNullOrWhiteSpace(item.GenerationStage)
                && string.Equals(item.CoordinateFrame, "SourceImage", StringComparison.Ordinal)
                && item.AppliedMinimumArea == minimumArea
                && item.AppliedMaximumArea == maximumArea
                && item.AppliedMinimumWidth == minimumWidth
                && item.AppliedMaximumWidth == maximumWidth
                && item.AppliedMinimumHeight == minimumHeight
                && item.AppliedMaximumHeight == maximumHeight),
                $"{toolType}/{boundary} candidate identity, frame, stage, or AppliedLimits were inconsistent.");
            Require(rows.Where(item => !item.Accepted).All(item => !string.IsNullOrWhiteSpace(item.RejectReasonCode)
                && !string.IsNullOrWhiteSpace(item.RejectReason)),
                $"{toolType}/{boundary} rejected rows did not retain both reject code and reason text.");

            if (areaBoundary)
            {
                Require(rows.Any(item => item.RejectReasonCode == "AreaBelowMinimum")
                    && rows.Any(item => item.RejectReasonCode == "AreaAboveMaximum"),
                    $"{toolType}/{boundary} did not retain both exact area-boundary reject reasons.");
            }
            else
            {
                foreach (string code in new[] { "WidthBelowMinimum", "WidthAboveMaximum", "HeightBelowMinimum", "HeightAboveMaximum" })
                {
                    Require(rows.Any(item => item.RejectReasonCode == code),
                        $"{toolType}/{boundary} did not retain reject reason code {code}.");
                }
            }

            double resultCount = summaryValue.Metrics.GetValueOrDefault(VisionPipelineKnownMetrics.ResultCount, -1D);
            Require(resultCount == accepted.Count, $"{toolType}/{boundary} ResultCount {resultCount} != accepted rows {accepted.Count}.");
            VerifyAcceptedMetric(summaryValue, VisionPipelineKnownMetrics.AreaMin, accepted.Min(item => item.Area), toolType, boundary);
            VerifyAcceptedMetric(summaryValue, VisionPipelineKnownMetrics.AreaMax, accepted.Max(item => item.Area), toolType, boundary);
            VerifyAcceptedMetric(summaryValue, VisionPipelineKnownMetrics.BoundsWidthMin, accepted.Min(item => item.BoundsWidth), toolType, boundary);
            VerifyAcceptedMetric(summaryValue, VisionPipelineKnownMetrics.BoundsWidthMax, accepted.Max(item => item.BoundsWidth), toolType, boundary);
            VerifyAcceptedMetric(summaryValue, VisionPipelineKnownMetrics.BoundsHeightMin, accepted.Min(item => item.BoundsHeight), toolType, boundary);
            VerifyAcceptedMetric(summaryValue, VisionPipelineKnownMetrics.BoundsHeightMax, accepted.Max(item => item.BoundsHeight), toolType, boundary);

            int acceptedOverlayCount = summaryValue.Overlays.Count(item =>
                string.Equals(item.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase)
                && string.Equals(item.Label, "Accepted object", StringComparison.Ordinal));
            Require(acceptedOverlayCount == accepted.Count,
                $"{toolType}/{boundary} accepted overlay count {acceptedOverlayCount} != accepted rows {accepted.Count}.");

            List<string> firstSignature = rows.Select(Signature).ToList();
            using VisionRecipeRunResult repeatRun = await runner.RunAsync(pipeline, source).ConfigureAwait(false);
            VisionRecipeStepRunSummary? repeatSummary = repeatRun.Steps.SingleOrDefault();
            Require(repeatRun.Success && repeatSummary != null,
                $"{toolType}/{boundary} repeat execution failed.");
            Require(firstSignature.SequenceEqual(repeatSummary!.ObjectResults.Select(Signature)),
                $"{toolType}/{boundary} candidate ordering/metadata changed across identical executions.");

            string caseEvidenceDirectory = Path.Combine(evidenceDirectory, toolType.ToLowerInvariant(), boundary);
            Directory.CreateDirectory(caseEvidenceDirectory);
            File.WriteAllLines(
                Path.Combine(caseEvidenceDirectory, "candidate_rows.tsv"),
                new[] { "Number\tCandidateId\tRegionIndex\tNativeIndex\tAccepted\tArea\tX\tY\tWidth\tHeight\tRejectReasonCode\tRejectReason\tAppliedMinArea\tAppliedMaxArea\tAppliedMinWidth\tAppliedMaxWidth\tAppliedMinHeight\tAppliedMaxHeight\tGenerationStage\tCoordinateFrame" }
                    .Concat(rows.Select(item => string.Join(
                        "\t",
                        item.Number.ToString(CultureInfo.InvariantCulture),
                        item.CandidateId,
                        item.RegionIndex.ToString(CultureInfo.InvariantCulture),
                        item.NativeIndex.ToString(CultureInfo.InvariantCulture),
                        item.Accepted.ToString(CultureInfo.InvariantCulture),
                        item.Area.ToString("0.###", CultureInfo.InvariantCulture),
                        item.BoundsX.ToString(CultureInfo.InvariantCulture),
                        item.BoundsY.ToString(CultureInfo.InvariantCulture),
                        item.BoundsWidth.ToString(CultureInfo.InvariantCulture),
                        item.BoundsHeight.ToString(CultureInfo.InvariantCulture),
                        item.RejectReasonCode,
                        item.RejectReason,
                        item.AppliedMinimumArea.ToString(CultureInfo.InvariantCulture),
                        item.AppliedMaximumArea.ToString(CultureInfo.InvariantCulture),
                        item.AppliedMinimumWidth.ToString(CultureInfo.InvariantCulture),
                        item.AppliedMaximumWidth.ToString(CultureInfo.InvariantCulture),
                        item.AppliedMinimumHeight.ToString(CultureInfo.InvariantCulture),
                        item.AppliedMaximumHeight.ToString(CultureInfo.InvariantCulture),
                        item.GenerationStage,
                        item.CoordinateFrame))));

            string reportPath = VisionPipelineRunReportStorage.Save(
                recipeName,
                pipeline,
                run,
                startedAt,
                DateTime.UtcNow,
                runLabel: toolType + "-" + boundary,
                sourceImage: source);
            VisionPipelineRunReport? report = VisionPipelineRunReportStorage.Load(reportPath);
            VisionPipelineStepRunReport? persistedStep = report?.Steps.SingleOrDefault();
            Require(report != null && persistedStep != null,
                $"{toolType}/{boundary} Run History report did not reload.");
            Require(persistedStep!.Objects.Count == rows.Count
                && persistedStep.OverlayCount == summaryValue.OverlayCount
                && persistedStep.Metrics.Any(item =>
                    string.Equals(item.Name, VisionPipelineKnownMetrics.ResultCount, StringComparison.OrdinalIgnoreCase)
                    && item.Value == accepted.Count),
                $"{toolType}/{boundary} Run History changed candidate/overlay/ResultCount evidence.");
            Require(persistedStep.Objects.Select(Signature).SequenceEqual(rows.Select(Signature)),
                $"{toolType}/{boundary} Run History candidate rows did not match the same execution snapshot.");
            observations.Add($"{toolType}/{boundary}: 12 rows, 4 same-area accepted candidates across ROI 0/1, exact limits/reasons, metrics, overlays, deterministic ordering, and Run History round-trip preserved.");
        }
        finally
        {
            RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
        }
    }

    private static void VerifyAcceptedMetric(
        VisionRecipeStepRunSummary summary,
        string metricName,
        double expected,
        string toolType,
        string boundary)
    {
        Require(summary.Metrics.TryGetValue(metricName, out double actual)
            && Math.Abs(actual - expected) <= 0.001D,
            $"{toolType}/{boundary} metric {metricName} expected {expected:0.###}, actual {FormatMetric(summary, metricName)}.");
    }

    private static VisionPipeline CreatePipeline(
        string toolType,
        int minimumArea,
        int maximumArea,
        int minimumWidth,
        int maximumWidth,
        int minimumHeight,
        int maximumHeight)
    {
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = toolType + " candidate boundary",
            ToolType = toolType,
            Enabled = true,
            InputLayer = VisionRecipeRunner.DefaultInputLayer,
            OutputLayer = toolType + "_Boundary_Result"
        };
        step.Parameters["USE_THRESHOLD"] = "true";
        step.Parameters["THRESHOLD_TYPES"] = "Binary";
        step.Parameters["THRESHOLD"] = "100";
        step.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
        step.Parameters["USE_BITWISENOT"] = "false";
        step.Parameters["USE_ROI"] = "false";
        step.Parameters["USE_MULTI_ROI"] = "true";
        step.Parameters["CvROIS"] = "0,0,200,150;200,0,200,150";
        step.Parameters["MIN_AREA"] = minimumArea.ToString(CultureInfo.InvariantCulture);
        step.Parameters["MAX_AREA"] = maximumArea.ToString(CultureInfo.InvariantCulture);
        step.Parameters["MIN_WIDTH"] = minimumWidth.ToString(CultureInfo.InvariantCulture);
        step.Parameters["MAX_WIDTH"] = maximumWidth.ToString(CultureInfo.InvariantCulture);
        step.Parameters["MIN_HEIGHT"] = minimumHeight.ToString(CultureInfo.InvariantCulture);
        step.Parameters["MAX_HEIGHT"] = maximumHeight.ToString(CultureInfo.InvariantCulture);
        if (string.Equals(toolType, "Contour", StringComparison.Ordinal))
        {
            step.Parameters["USE_DRAW_IMAGE"] = "true";
            step.Parameters["DetectMode"] = "External";
            step.Parameters["ApproximationModes"] = "ApproxSimple";
        }

        VisionPipeline pipeline = new VisionPipeline { Name = "2D-032 " + toolType + " " + minimumArea };
        pipeline.Steps.Add(step);
        return pipeline;
    }

    private static Mat CreateSource()
    {
        Mat source = new Mat(new OpenCvSharp.Size(400, 150), MatType.CV_8UC1, Scalar.Black);
        foreach (Rect rectangle in new[]
        {
            new Rect(20, 15, 24, 32),
            new Rect(60, 15, 24, 32),
            new Rect(100, 15, 8, 8),
            new Rect(120, 15, 40, 40),
            new Rect(20, 80, 12, 32),
            new Rect(50, 80, 36, 32),
            new Rect(100, 80, 24, 12),
            new Rect(130, 80, 24, 48),
            new Rect(220, 15, 24, 32),
            new Rect(260, 15, 24, 32),
            new Rect(300, 80, 12, 32),
            new Rect(340, 80, 36, 32)
        })
        {
            Cv2.Rectangle(source, rectangle, Scalar.White, -1);
        }

        return source;
    }

    private static string Signature(VisionPipelineObjectResult item)
    {
        return string.Join(
            "|",
            item.Number.ToString(CultureInfo.InvariantCulture),
            item.CandidateId,
            item.RegionIndex.ToString(CultureInfo.InvariantCulture),
            item.NativeIndex.ToString(CultureInfo.InvariantCulture),
            item.Accepted.ToString(CultureInfo.InvariantCulture),
            item.Area.ToString("R", CultureInfo.InvariantCulture),
            item.BoundsX.ToString(CultureInfo.InvariantCulture),
            item.BoundsY.ToString(CultureInfo.InvariantCulture),
            item.BoundsWidth.ToString(CultureInfo.InvariantCulture),
            item.BoundsHeight.ToString(CultureInfo.InvariantCulture),
            item.RejectReasonCode,
            item.RejectReason,
            item.AppliedMinimumArea.ToString(CultureInfo.InvariantCulture),
            item.AppliedMaximumArea.ToString(CultureInfo.InvariantCulture),
            item.AppliedMinimumWidth.ToString(CultureInfo.InvariantCulture),
            item.AppliedMaximumWidth.ToString(CultureInfo.InvariantCulture),
            item.AppliedMinimumHeight.ToString(CultureInfo.InvariantCulture),
            item.AppliedMaximumHeight.ToString(CultureInfo.InvariantCulture),
            item.GenerationStage,
            item.CoordinateFrame);
    }

    private static string Signature(VisionPipelineObjectRunReport item)
    {
        return string.Join(
            "|",
            item.Number.ToString(CultureInfo.InvariantCulture),
            item.CandidateId,
            item.RegionIndex.ToString(CultureInfo.InvariantCulture),
            item.NativeIndex.ToString(CultureInfo.InvariantCulture),
            item.Accepted.ToString(CultureInfo.InvariantCulture),
            item.Area.ToString("R", CultureInfo.InvariantCulture),
            item.BoundsX.ToString(CultureInfo.InvariantCulture),
            item.BoundsY.ToString(CultureInfo.InvariantCulture),
            item.BoundsWidth.ToString(CultureInfo.InvariantCulture),
            item.BoundsHeight.ToString(CultureInfo.InvariantCulture),
            item.RejectReasonCode,
            item.RejectReason,
            item.AppliedMinimumArea.ToString(CultureInfo.InvariantCulture),
            item.AppliedMaximumArea.ToString(CultureInfo.InvariantCulture),
            item.AppliedMinimumWidth.ToString(CultureInfo.InvariantCulture),
            item.AppliedMaximumWidth.ToString(CultureInfo.InvariantCulture),
            item.AppliedMinimumHeight.ToString(CultureInfo.InvariantCulture),
            item.AppliedMaximumHeight.ToString(CultureInfo.InvariantCulture),
            item.GenerationStage,
            item.CoordinateFrame);
    }

    private static string FormatAreas(IEnumerable<VisionPipelineObjectResult> rows)
    {
        return string.Join(",", rows.Select(item => item.Area.ToString("0.###", CultureInfo.InvariantCulture)));
    }

    private static string FormatMetric(VisionRecipeStepRunSummary summary, string metricName)
    {
        return summary.Metrics.TryGetValue(metricName, out double value)
            ? value.ToString("0.###", CultureInfo.InvariantCulture)
            : "missing";
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
