using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Result;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

internal static class ValidationDatasetDrawingEvidence
{
    internal static void VerifyAndWrite(
        OpenVisionShellHostView shellHost,
        string recipeName,
        string pipelineXml,
        string sourceImagePath,
        string artifactDirectory,
        Action<int> pump)
    {
        int layerCountBefore = shellHost.LayerDocumentCount;
        int previewRunsBefore = shellHost.NativePreviewRunCount;
        string inputRouteBefore = shellHost.ActiveNativeRouteInputLayerNameForTest;
        string outputRouteBefore = shellHost.ActiveNativeRouteOutputLayerNameForTest;
        string activeLayerBefore = shellHost.ActiveHostLayerTitle;
        bool previewResultBefore = shellHost.HasNativePreviewResult;
        if (!OpenVisionRecipeRunEvidence.TryCreate(
                shellHost.RecipeCommands.SelectedRecentBatchSampleResultOption,
                out OpenVisionRecipeRunEvidence storedEvidence,
                out string evidenceReason))
        {
            throw new InvalidOperationException("Stored batch evidence could not be resolved: " + evidenceReason);
        }

        if (!storedEvidence.IsStoredSourceVerified)
        {
            throw new InvalidOperationException(
                "Stored batch evidence did not use a SHA-256-verified run-time source snapshot.");
        }

        bool expectsTwoPinArrayGapDrawings = SerializeHelper.TryLoadFromXmlText(
                pipelineXml,
                out VisionPipeline drawingEvidencePipeline,
                out _)
            && drawingEvidencePipeline.Steps.Count(step => step?.Enabled == true) == 2
            && drawingEvidencePipeline.Steps
                .Where(step => step?.Enabled == true)
                .All(step => string.Equals(step.ToolType, "PinArrayGap", StringComparison.OrdinalIgnoreCase));
        if (expectsTwoPinArrayGapDrawings
            && (storedEvidence.Drawings.Count != 2
                || storedEvidence.Drawings.Any(drawing =>
                    !string.Equals(drawing?.ToolType, "PinArrayGap", StringComparison.OrdinalIgnoreCase))))
        {
            throw new InvalidOperationException(
                "Stored PinArrayGap evidence must contain exactly two executed row drawings. "
                + "DrawingCount=" + storedEvidence.Drawings.Count.ToString(CultureInfo.InvariantCulture));
        }

        if (expectsTwoPinArrayGapDrawings)
        {
            AssertExecutedPinArrayGapFailurePreservesPriorRowDrawing(
                recipeName,
                pipelineXml,
                sourceImagePath);
        }

        string evidenceArtifactDirectory = Path.Combine(artifactDirectory, "selected_run_report");
        Directory.CreateDirectory(evidenceArtifactDirectory);
        string sourceReportDirectory = Path.GetDirectoryName(storedEvidence.DrawingImagePath) ?? string.Empty;
        foreach (string artifact in Directory.GetFiles(sourceReportDirectory))
        {
            File.Copy(artifact, Path.Combine(evidenceArtifactDirectory, Path.GetFileName(artifact)), overwrite: true);
        }

        bool viewerReady = false;
        using (OpenVisionRecipeRunEvidenceViewerView evidenceProbe = new OpenVisionRecipeRunEvidenceViewerView())
        {
            if (!evidenceProbe.TrySetEvidence(storedEvidence))
            {
                throw new InvalidOperationException(
                    "Stored batch evidence images could not be assigned to the viewer: "
                    + evidenceProbe.LoadError);
            }
            if (expectsTwoPinArrayGapDrawings)
            {
                ComboBox drawingSelector = evidenceProbe.FindName("cmbStoredDrawing") as ComboBox
                    ?? throw new InvalidOperationException("Stored drawing selector was not found.");
                drawingSelector.SelectedIndex = 0;
                drawingSelector.SelectedIndex = 1;
                if (evidenceProbe.DrawingCount != 2
                    || !evidenceProbe.HasDrawingImage
                    || !string.Equals(
                        evidenceProbe.SelectedStepText,
                        storedEvidence.Drawings[1].StepText,
                        StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Stored evidence viewer could not select the second PinArrayGap row drawing. "
                        + $"DrawingCount={evidenceProbe.DrawingCount}, Selected='{evidenceProbe.SelectedStepText}'.");
                }
            }

            viewerReady = evidenceProbe.HasSourceImage && evidenceProbe.HasDrawingImage;
        }

        if (!shellHost.RecipeCommands.OpenSelectedRecentBatchRunEvidenceCommand.CanExecute(null))
        {
            shellHost.RecipeCommands.OpenSelectedRecentBatchRunEvidenceCommand.Execute(null);
            throw new InvalidOperationException(
                "Stored batch evidence was not available for the selected sample. "
                + "SampleImage='" + shellHost.RecipeCommands.SelectedRecentBatchSampleResultOption?.SampleImagePath + "', "
                + "RunReport='" + shellHost.RecipeCommands.SelectedRecentBatchSampleResultOption?.RunReportPath + "', "
                + "ReportExists=" + File.Exists(shellHost.RecipeCommands.SelectedRecentBatchSampleResultOption?.RunReportPath)
                + ". "
                + "Reason='" + shellHost.RecipeCommands.StatusText + "'. "
                + shellHost.RecipeCommands.SelectedRecentBatchRunReviewText);
        }

        shellHost.RecipeCommands.OpenSelectedRecentBatchRunEvidenceCommand.Execute(null);
        pump(120);
        OpenVisionFloatingToolWindow? evidenceWindow = Application.Current.Windows
            .OfType<OpenVisionFloatingToolWindow>()
            .LastOrDefault(item => item.IsVisible);
        OpenVisionRecipeRunEvidenceViewerView? evidenceViewer = evidenceWindow?.HostedContent
            as OpenVisionRecipeRunEvidenceViewerView;
        bool workspaceUnchanged = evidenceWindow != null
            && evidenceViewer != null
            && evidenceViewer.HasSourceImage
            && evidenceViewer.HasDrawingImage
            && shellHost.LayerDocumentCount == layerCountBefore
            && shellHost.NativePreviewRunCount == previewRunsBefore
            && string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, inputRouteBefore, StringComparison.Ordinal)
            && string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, outputRouteBefore, StringComparison.Ordinal)
            && string.Equals(shellHost.ActiveHostLayerTitle, activeLayerBefore, StringComparison.Ordinal)
            && shellHost.HasNativePreviewResult == previewResultBefore;
        if (!workspaceUnchanged)
        {
            throw new InvalidOperationException(
                "Stored batch evidence viewer did not keep source/drawing images or changed workspace execution state. "
                + $"ViewerType={evidenceWindow?.HostedContent?.GetType().Name ?? "-"}, "
                + $"Source={evidenceViewer?.HasSourceImage}, Drawing={evidenceViewer?.HasDrawingImage}, "
                + $"Status='{shellHost.RecipeCommands.StatusText}', "
                + $"Layers={layerCountBefore}->{shellHost.LayerDocumentCount}, "
                + $"Preview={previewRunsBefore}->{shellHost.NativePreviewRunCount}, "
                + $"Input={inputRouteBefore}->{shellHost.ActiveNativeRouteInputLayerNameForTest}, "
                + $"Output={outputRouteBefore}->{shellHost.ActiveNativeRouteOutputLayerNameForTest}, "
                + $"Active={activeLayerBefore}->{shellHost.ActiveHostLayerTitle}, "
                + $"PreviewResult={previewResultBefore}->{shellHost.HasNativePreviewResult}");
        }

        File.WriteAllLines(
            Path.Combine(artifactDirectory, "drawing_evidence_contract.txt"),
            BuildContractLines(
                storedEvidence.SampleName,
                storedEvidence.Drawings.Count,
                storedEvidence.IsStoredSourceVerified,
                viewerReady,
                workspaceUnchanged));
    }

    internal static IReadOnlyList<string> BuildContractLines(
        string sampleName,
        int drawingCount,
        bool storedSourceVerified,
        bool viewerReady,
        bool workspaceUnchanged)
    {
        return new[]
        {
            "Sample=" + (sampleName ?? string.Empty),
            "DrawingCount=" + drawingCount.ToString(CultureInfo.InvariantCulture),
            "StoredSourceVerified=" + storedSourceVerified,
            "ViewerReady=" + viewerReady,
            "WorkspaceUnchanged=" + workspaceUnchanged
        };
    }

    private static void AssertExecutedPinArrayGapFailurePreservesPriorRowDrawing(
        string recipeName,
        string pipelineXml,
        string sourceImagePath)
    {
        if (!SerializeHelper.TryLoadFromXmlText(pipelineXml, out VisionPipeline pipeline, out string parseError)
            || pipeline == null
            || pipeline.Steps.Count(step => step?.Enabled == true) != 2)
        {
            throw new InvalidOperationException("Could not prepare the executed-row drawing boundary smoke: " + parseError);
        }

        using Mat source = Cv2.ImRead(sourceImagePath, ImreadModes.Unchanged);
        if (source.Empty())
        {
            throw new InvalidOperationException("Executed-row drawing boundary smoke source could not be loaded.");
        }

        List<VisionPipelineStep> steps = pipeline.Steps.Where(step => step?.Enabled == true).ToList();
        steps[0].Name = "Row";
        steps[1].Name = "Row Bottom";
        VisionRecipeStepRunSummary CreateSummary(VisionPipelineStep step, int index, bool success)
        {
            return new VisionRecipeStepRunSummary
            {
                Index = index,
                Name = step.Name,
                ToolType = step.ToolType,
                Enabled = true,
                Skipped = false,
                InputLayer = step.InputLayer,
                OutputLayer = step.OutputLayer,
                Status = success ? "OK" : "ERROR",
                ToolSuccess = success,
                Success = success,
                AcceptancePassed = success,
                AcceptanceMessage = success ? "Acceptance passed." : "Synthetic executed failure before geometry.",
                Message = success ? "Completed." : "Synthetic executed failure before geometry.",
                ErrorCode = success ? 0 : -1,
                ErrorName = success ? "None" : "SmokeExecutedFailure",
                Parameters = new Dictionary<string, string>(step.Parameters, StringComparer.OrdinalIgnoreCase)
            };
        }

        using VisionRecipeRunResult result = new VisionRecipeRunResult
        {
            PipelineName = pipeline.Name,
            Success = false,
            Message = "Synthetic second-row executed failure for persisted-drawing coverage.",
            ResultImage = source.Clone(),
            ResultImageWidth = source.Width,
            ResultImageHeight = source.Height,
            Steps = new List<VisionRecipeStepRunSummary>
            {
                CreateSummary(steps[0], 1, success: true),
                CreateSummary(steps[1], 2, success: false)
            }
        };
        DateTime startedAt = DateTime.UtcNow;
        string reportPath = VisionPipelineRunReportStorage.Save(
            recipeName,
            pipeline,
            result,
            startedAt,
            startedAt.AddMilliseconds(1),
            "ExecutedRowDrawingBoundary",
            source);
        VisionPipelineRunReport report = VisionPipelineRunReportStorage.Load(reportPath)
            ?? throw new InvalidOperationException("Executed-row drawing boundary report could not be loaded.");
        string reportDirectory = Path.GetDirectoryName(reportPath) ?? string.Empty;
        List<VisionPipelineStepRunReport> storedRows = report.Steps
            .Where(step => step?.Enabled == true
                && !step.Skipped
                && string.Equals(step.ToolType, "PinArrayGap", StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (storedRows.Count != 2
            || storedRows.Any(step => string.IsNullOrWhiteSpace(step.OverlayImageFile)
                || !File.Exists(Path.Combine(reportDirectory, step.OverlayImageFile))))
        {
            throw new InvalidOperationException(
                "An executed PinArrayGap failure did not preserve both row drawings. "
                + "Stored=" + storedRows.Count.ToString(CultureInfo.InvariantCulture));
        }

        OpenVisionRecipeBatchSampleResultOption selectionProbe = OpenVisionRecipeBatchSampleResultOption.Create(
            new VisionPipelineBatchSampleRunResult
            {
                SampleName = "Prefix-name default drawing probe",
                Status = "NG",
                Success = false,
                FailedStep = "02 Row Bottom [ERROR] - Synthetic executed failure before geometry.",
                SampleImagePath = sourceImagePath,
                ReportPath = sourceImagePath,
                PairRole = "NG",
                ExpectedText = "ExpectedActual:NG",
                RunReportPath = reportPath
            });
        if (!OpenVisionRecipeRunEvidence.TryCreate(
                selectionProbe,
                out OpenVisionRecipeRunEvidence selectionEvidence,
                out string selectionError)
            || !selectionEvidence.IsStoredSourceVerified
            || selectionEvidence.DefaultDrawing?.Index != 2)
        {
            throw new InvalidOperationException(
                "Stored evidence did not resolve the stable failed-Step index before a prefix name. "
                + selectionError);
        }
    }
}
