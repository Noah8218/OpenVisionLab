using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Common;
using OpenVisionLab.Core;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal static class PreviewRunEquivalenceContract
{
    private const double CoordinateTolerance = 1e-3;
    private const double FloatingMetricTolerance = 1e-5;

    public static async Task<int> RunAsync(string evidenceDirectory)
    {
        evidenceDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string repositoryRoot = FindRepositoryRoot();
        List<string> passed = new();
        List<string> failed = new();
        string localeObservation;
        CaseDefinition[] cases =
        {
            new CaseDefinition("Mean", "Mean_Brightness_Synthetic_OK.png", "Public_Mean_BrightnessDrift.pipeline.xml", 0),
            new CaseDefinition("Blob", "Blob_Particles_Synthetic_OK.png", "Public_Blob_Particles.pipeline.xml", 1),
            new CaseDefinition("Contour", "Contour_Shapes_Synthetic_OK.png", "Public_Contour_Shapes.pipeline.xml", 1),
            new CaseDefinition("Matching", "Matching_DiePad_Synthetic_OK.png", "Public_Matching_DiePad.pipeline.xml", 0),
            new CaseDefinition("Line", "Line_Pins_Synthetic_OK.png", "Public_Line_Pins_Distance.pipeline.xml", 1)
        };

        foreach (CaseDefinition definition in cases)
        {
            try
            {
                await RunCaseAsync(repositoryRoot, evidenceDirectory, definition).ConfigureAwait(false);
                passed.Add(definition.Name);
            }
            catch (Exception exception)
            {
                failed.Add($"{definition.Name}: {exception.GetBaseException().Message}");
            }
        }

        try
        {
            await RunLocaleConsistencyAsync(repositoryRoot, evidenceDirectory, cases[0]).ConfigureAwait(false);
            localeObservation = "Locale: PASS (Mean Run decision and metrics were stable under en-US and ko-KR)";
        }
        catch (Exception exception)
        {
            localeObservation = "Locale: FAIL (" + exception.GetBaseException().Message + ")";
            failed.Add("Locale: " + exception.GetBaseException().Message);
        }

        string reportPath = Path.Combine(evidenceDirectory, "preview-run-reopen-equivalence.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failed.Count == 0 ? "PASS" : "FAIL"),
                "Contract: same input semantics, Preview execution, headless Run, and save/reopen result equivalence",
                "Tolerance: integer metrics exact; floating metrics <= 1e-5; coordinates <= 1e-3; decisions exact",
                "Preview boundary: OpenVisionNativePreviewExecutionController.ComputeSingleInput",
                "Runner boundary: VisionRecipeRunner.RunAsync -> VisionPipelineExecutionService",
                "Cases: " + string.Join(", ", cases.Select(item => item.Name)),
                "Passed: " + (passed.Count == 0 ? "None" : string.Join(", ", passed)),
                "Failed: " + (failed.Count == 0 ? "None" : string.Join(" | ", failed)),
                localeObservation,
                "Failure contracts: pipeline-prevalidation (missing input/invalid acceptance) and pipeline-acceptance-finite (non-finite/missing metric gates)"
            });

        foreach (string item in passed)
        {
            Console.WriteLine("PASS|" + item);
        }

        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static async Task RunLocaleConsistencyAsync(
        string repositoryRoot,
        string evidenceDirectory,
        CaseDefinition definition)
    {
        string imagePath = Path.Combine(repositoryRoot, "docs", "samples", "public", definition.ImageFileName);
        string pipelinePath = Path.Combine(repositoryRoot, "docs", "samples", "public", definition.PipelineFileName);
        using Mat source = Cv2.ImRead(imagePath, ImreadModes.AnyColor | ImreadModes.AnyDepth);
        Require(source != null && !source.Empty(), "Locale check source image was empty.");
        OpenCvHelper.SetImageChannel1(source);

        Dictionary<string, double> baselineMetrics = null;
        bool baselineToolSuccess = false;
        bool baselineAcceptancePassed = false;
        string[] cultureNames = { "en-US", "ko-KR" };
        List<string> observations = new();
        CultureInfo previousCulture = CultureInfo.CurrentCulture;
        CultureInfo previousUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            foreach (string cultureName in cultureNames)
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                Require(SerializeHelper.TryLoadFromXmlFile(pipelinePath, out VisionPipeline pipeline, out Exception loadError),
                    "Locale check could not load Pipeline XML: " + loadError?.Message);
                using VisionRecipeRunResult run = await new VisionRecipeRunner().RunAsync(
                    pipelinePath,
                    source.Clone(),
                    VisionRecipeRunner.DefaultInputLayer,
                    VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                    CancellationToken.None).ConfigureAwait(false);
                Require(run.Steps != null && run.Steps.Count > definition.StepIndex,
                    "Locale check did not produce the target Step.");
                VisionRecipeStepRunSummary summary = run.Steps[definition.StepIndex];
                if (baselineMetrics == null)
                {
                    baselineMetrics = new Dictionary<string, double>(summary.Metrics, StringComparer.OrdinalIgnoreCase);
                    baselineToolSuccess = summary.ToolSuccess;
                    baselineAcceptancePassed = summary.AcceptancePassed;
                }
                else
                {
                    Require(summary.ToolSuccess == baselineToolSuccess
                        && summary.AcceptancePassed == baselineAcceptancePassed,
                        "Locale check changed the Run decision.");
                    foreach (KeyValuePair<string, double> metric in baselineMetrics)
                    {
                        Require(summary.Metrics.TryGetValue(metric.Key, out double actual)
                            && Math.Abs(metric.Value - actual) <= FloatingMetricTolerance,
                            "Locale check changed metric '" + metric.Key + "'.");
                    }
                }

                observations.Add(
                    cultureName + ": ToolSuccess=" + summary.ToolSuccess.ToString(CultureInfo.InvariantCulture)
                    + "; Acceptance=" + summary.AcceptancePassed.ToString(CultureInfo.InvariantCulture)
                    + "; MetricCount=" + summary.Metrics.Count.ToString(CultureInfo.InvariantCulture));
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUiCulture;
        }

        string localeDirectory = Path.Combine(evidenceDirectory, "locale");
        Directory.CreateDirectory(localeDirectory);
        File.WriteAllLines(
            Path.Combine(localeDirectory, "locale-consistency.txt"),
            new[]
            {
                "Result: PASS",
                "Case: " + definition.Name,
                "ExecutionInputNormalization: OpenCvHelper.SetImageChannel1",
                "Cultures: en-US, ko-KR"
            }.Concat(observations));
    }

    private static async Task RunCaseAsync(
        string repositoryRoot,
        string evidenceDirectory,
        CaseDefinition definition)
    {
        string imagePath = Path.Combine(repositoryRoot, "docs", "samples", "public", definition.ImageFileName);
        string pipelinePath = Path.Combine(repositoryRoot, "docs", "samples", "public", definition.PipelineFileName);
        Require(File.Exists(imagePath), $"Missing public image: {imagePath}");
        Require(File.Exists(pipelinePath), $"Missing public Pipeline XML: {pipelinePath}");
        Require(SerializeHelper.TryLoadFromXmlFile(pipelinePath, out VisionPipeline pipeline, out Exception loadError),
            $"Could not load Pipeline XML: {loadError?.Message}");
        Require(pipeline?.Steps != null && definition.StepIndex >= 0 && definition.StepIndex < pipeline.Steps.Count,
            $"Target Step index {definition.StepIndex} is outside the Pipeline.");

        VisionPipelineStep targetStep = pipeline.Steps[definition.StepIndex];
        string caseDirectory = Path.Combine(evidenceDirectory, definition.Name.ToLowerInvariant());
        Directory.CreateDirectory(caseDirectory);
        string roundTripPath = Path.Combine(caseDirectory, "saved-reopened.pipeline.xml");
        Require(SerializeHelper.SaveXmlFile(roundTripPath, pipeline),
            $"Could not save the round-trip Pipeline XML: {roundTripPath}");

        using Mat source = Cv2.ImRead(imagePath, ImreadModes.AnyColor | ImreadModes.AnyDepth);
        Require(source != null && !source.Empty(), $"Could not load source image: {imagePath}");
        string sourceSha256 = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(imagePath)));
        using Mat executionSource = source.Clone();
        OpenCvHelper.SetImageChannel1(executionSource);

        using Mat targetInput = await CaptureTargetInputAsync(pipeline, executionSource, targetStep).ConfigureAwait(false);
        Require(targetInput != null && !targetInput.Empty(),
            $"The headless execution did not expose input layer '{targetStep.InputLayer}' for '{targetStep.Name}'.");

        DirectCapture direct = RunPreview(targetStep, targetInput);
        using VisionRecipeRunResult runner = await new VisionRecipeRunner().RunAsync(
            pipelinePath,
            executionSource.Clone(),
            VisionRecipeRunner.DefaultInputLayer,
            VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
            CancellationToken.None).ConfigureAwait(false);
        using VisionRecipeRunResult reopened = await new VisionRecipeRunner().RunAsync(
            roundTripPath,
            executionSource.Clone(),
            VisionRecipeRunner.DefaultInputLayer,
            VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
            CancellationToken.None).ConfigureAwait(false);

        Require(runner.Steps != null && runner.Steps.Count > definition.StepIndex,
            $"Runner did not produce target Step {definition.StepIndex}.");
        Require(reopened.Steps != null && reopened.Steps.Count > definition.StepIndex,
            $"Reopened Runner did not produce target Step {definition.StepIndex}.");

        VisionRecipeStepRunSummary runSummary = runner.Steps[definition.StepIndex];
        VisionRecipeStepRunSummary reopenedSummary = reopened.Steps[definition.StepIndex];
        List<string> mismatches = new();
        CompareInputSemantics(targetInput, direct.PreviewInput, mismatches);
        ComparePreviewToRun(direct, runSummary, mismatches);
        CompareRunToReopened(runSummary, reopenedSummary, mismatches);
        Require(mismatches.Count == 0,
            $"{definition.Name} equivalence mismatch: {string.Join("; ", mismatches)}");

        File.WriteAllLines(
            Path.Combine(caseDirectory, "result.txt"),
            new[]
            {
                "Result: PASS",
                "Case: " + definition.Name,
                "Source: " + imagePath,
                "SourceSha256: " + sourceSha256,
                "Pipeline: " + pipelinePath,
                "RoundTripPipeline: " + roundTripPath,
                "Step: " + targetStep.Name,
                "ToolType: " + targetStep.ToolType,
                "InputLayer: " + targetStep.InputLayer,
                "ExecutionInputNormalization: OpenCvHelper.SetImageChannel1",
                "PreviewInputMaxAbsDiffAfterNormalization: 0",
                "PreviewToolSuccess: " + direct.ToolSuccess.ToString(CultureInfo.InvariantCulture),
                "PreviewAcceptance: " + direct.AcceptancePassed.ToString(CultureInfo.InvariantCulture),
                "RunToolSuccess: " + runSummary.ToolSuccess.ToString(CultureInfo.InvariantCulture),
                "RunAcceptance: " + runSummary.AcceptancePassed.ToString(CultureInfo.InvariantCulture),
                "RunMetricCount: " + runSummary.Metrics.Count.ToString(CultureInfo.InvariantCulture),
                "RunOverlayCount: " + runSummary.Overlays.Count.ToString(CultureInfo.InvariantCulture),
                "RunObjectCount: " + runSummary.ObjectResults.Count.ToString(CultureInfo.InvariantCulture),
                "ReopenedResult: PASS"
            });
    }

    private static DirectCapture RunPreview(VisionPipelineStep step, Mat targetInput)
    {
        DirectCapture capture = null;
        using DisplayManagerService displayManager = new DisplayManagerService();
        OpenVisionNativePreviewExecutionController controller =
            new OpenVisionNativePreviewExecutionController(
                displayManager,
                new OpenVisionNativePreviewLayerPublisher(displayManager));
        using OpenVisionNativePreviewInputSnapshot snapshot =
            new OpenVisionNativePreviewInputSnapshot(
                BitmapImageConverter.ToBitmap(targetInput),
                step.OutputLayer,
                step.InputLayer,
                normalizeSingleChannelInput: true);
        using OpenVisionNativePreviewComputation computation = controller.ComputeSingleInput(
            snapshot,
            previewInput =>
            {
                capture = new DirectCapture
                {
                    PreviewInput = previewInput.Clone()
                };
                IVisionTool tool = VisionPipelineAppToolFactory.Create(step);
                using (tool as IDisposable)
                {
                    VisionToolResult result = tool.Execute(previewInput);
                    if (result == null)
                    {
                        throw new InvalidOperationException("Preview tool returned no result.");
                    }

                    VisionPipelineObjectResultCaptureService.Capture(step, previewInput, tool, result);
                    VisionPipelineMatchResultCaptureService.Capture(step, previewInput, tool, result);
                    VisionPipelineGeometryFeatureCaptureService.Capture(step, previewInput, tool, result);
                    VisionPipelineAcceptanceResult acceptance = VisionPipelineExecutionService.EvaluateAcceptance(step, result);
                    capture.ToolSuccess = result.Success;
                    capture.AcceptancePassed = acceptance.Passed;
                    capture.Metrics = new Dictionary<string, double>(result.Metrics, StringComparer.OrdinalIgnoreCase);
                    capture.Overlays = result.Overlays?.Select(OverlayValue.From).ToList() ?? new List<OverlayValue>();
                    capture.Objects = VisionPipelineObjectResultStore.Get(result).Select(ObjectValue.From).ToList();
                    capture.Geometry = VisionPipelineGeometryFeatureStore.Get(result).Select(GeometryValue.From).ToList();
                    // The headless tool factory intentionally returns no display bitmap for measurement-only tools.
                    // The real native preview executor supplies its visualization; this contract only needs a
                    // deterministic non-empty transport image so the shared Preview controller can complete.
                    if (result.ResultImage == null || result.ResultImage.Empty())
                    {
                        result.ResultImage = previewInput.Clone();
                    }
                    return result;
                }
            },
            CancellationToken.None);

        Require(capture != null, "Preview delegate did not execute.");
        Require(computation.Success, "Preview boundary failed: " + computation.Status);
        capture.ResultWidth = computation.ResultBitmap?.Width ?? 0;
        capture.ResultHeight = computation.ResultBitmap?.Height ?? 0;
        return capture;
    }

    private static async Task<Mat> CaptureTargetInputAsync(
        VisionPipeline pipeline,
        Mat source,
        VisionPipelineStep targetStep)
    {
        using VisionPipelineContext context = new VisionPipelineContext();
        context.SetLayer(VisionRecipeRunner.DefaultInputLayer, source.Clone());
        Mat targetInput = null;
        using VisionPipelineRunResult run = await VisionPipelineExecutionService.RunAsync(
            pipeline,
            context,
            VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
            CancellationToken.None,
            update =>
            {
                if (update?.Step == null
                    || !string.Equals(update.Step.Name, targetStep.Name, StringComparison.Ordinal)
                    || !string.Equals(update.Step.ToolType, targetStep.ToolType, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                Mat layer = context.GetLayer(update.Step.InputLayer);
                targetInput?.Dispose();
                targetInput = layer?.Clone();
            }).ConfigureAwait(false);

        return targetInput;
    }

    private static void CompareInputSemantics(Mat expectedInput, Mat previewInput, List<string> mismatches)
    {
        using Mat expected = expectedInput.Clone();
        OpenCvHelper.SetImageChannel1(expected);
        if (previewInput == null || previewInput.Empty())
        {
            mismatches.Add("Preview input was empty");
            return;
        }

        if (expected.Size() != previewInput.Size() || expected.Type() != previewInput.Type())
        {
            mismatches.Add($"input shape/type expected={expected.Size()}/{expected.Type()} actual={previewInput.Size()}/{previewInput.Type()}");
            return;
        }

        double maxAbsDiff = Cv2.Norm(expected, previewInput, NormTypes.INF);
        if (maxAbsDiff > 0D)
        {
            mismatches.Add($"input max abs diff={maxAbsDiff.ToString("R", CultureInfo.InvariantCulture)}");
        }
    }

    private static void ComparePreviewToRun(
        DirectCapture preview,
        VisionRecipeStepRunSummary run,
        List<string> mismatches)
    {
        if (preview.ToolSuccess != run.ToolSuccess)
        {
            mismatches.Add($"tool success preview={preview.ToolSuccess} run={run.ToolSuccess}");
        }

        if (preview.AcceptancePassed != run.AcceptancePassed)
        {
            mismatches.Add($"acceptance preview={preview.AcceptancePassed} run={run.AcceptancePassed}");
        }

        foreach (KeyValuePair<string, double> metric in preview.Metrics)
        {
            if (!run.Metrics.TryGetValue(metric.Key, out double actual))
            {
                mismatches.Add($"missing metric '{metric.Key}' in Run");
                continue;
            }

            double tolerance = IsIntegerMetric(metric.Key) ? 0D : FloatingMetricTolerance;
            if (Math.Abs(metric.Value - actual) > tolerance)
            {
                mismatches.Add($"metric {metric.Key} preview={metric.Value.ToString("R", CultureInfo.InvariantCulture)} run={actual.ToString("R", CultureInfo.InvariantCulture)} tol={tolerance.ToString("R", CultureInfo.InvariantCulture)}");
            }
        }

        if (preview.Overlays.Count != run.Overlays.Count)
        {
            mismatches.Add($"overlay count preview={preview.Overlays.Count} run={run.Overlays.Count}");
        }
        else
        {
            for (int index = 0; index < preview.Overlays.Count; index++)
            {
                preview.Overlays[index].Compare(run.Overlays[index], $"overlay[{index}]", mismatches);
            }
        }

        if (preview.Objects.Count != run.ObjectResults.Count)
        {
            mismatches.Add($"object count preview={preview.Objects.Count} run={run.ObjectResults.Count}");
        }
        else
        {
            for (int index = 0; index < preview.Objects.Count; index++)
            {
                preview.Objects[index].Compare(run.ObjectResults[index], $"object[{index}]", mismatches);
            }
        }

        if (preview.Geometry.Count != run.GeometryFeatures.Count)
        {
            mismatches.Add($"geometry count preview={preview.Geometry.Count} run={run.GeometryFeatures.Count}");
        }
        else
        {
            for (int index = 0; index < preview.Geometry.Count; index++)
            {
                preview.Geometry[index].Compare(run.GeometryFeatures[index], $"geometry[{index}]", mismatches);
            }
        }

        if (preview.ResultWidth != run.ResultImageWidth || preview.ResultHeight != run.ResultImageHeight)
        {
            mismatches.Add($"result size preview={preview.ResultWidth}x{preview.ResultHeight} run={run.ResultImageWidth}x{run.ResultImageHeight}");
        }
    }

    private static void CompareRunToReopened(
        VisionRecipeStepRunSummary first,
        VisionRecipeStepRunSummary reopened,
        List<string> mismatches)
    {
        if (first.ToolSuccess != reopened.ToolSuccess || first.AcceptancePassed != reopened.AcceptancePassed)
        {
            mismatches.Add("save/reopen decision changed");
        }

        foreach (KeyValuePair<string, double> metric in first.Metrics)
        {
            if (!reopened.Metrics.TryGetValue(metric.Key, out double actual))
            {
                mismatches.Add($"save/reopen missing metric '{metric.Key}'");
                continue;
            }

            double tolerance = IsIntegerMetric(metric.Key) ? 0D : FloatingMetricTolerance;
            if (Math.Abs(metric.Value - actual) > tolerance)
            {
                mismatches.Add($"save/reopen metric '{metric.Key}' changed");
            }
        }

        if (first.Overlays.Count != reopened.Overlays.Count
            || first.ObjectResults.Count != reopened.ObjectResults.Count
            || first.GeometryFeatures.Count != reopened.GeometryFeatures.Count
            || first.ResultImageWidth != reopened.ResultImageWidth
            || first.ResultImageHeight != reopened.ResultImageHeight)
        {
            mismatches.Add("save/reopen coordinate or result shape changed");
        }
    }

    private static bool IsIntegerMetric(string name)
    {
        return string.Equals(name, VisionPipelineKnownMetrics.ResultCount, StringComparison.OrdinalIgnoreCase)
            || name.EndsWith("Count", StringComparison.OrdinalIgnoreCase);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "OpenVisionLab.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the OpenVisionLab repository root.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed class CaseDefinition
    {
        public CaseDefinition(string name, string imageFileName, string pipelineFileName, int stepIndex)
        {
            Name = name;
            ImageFileName = imageFileName;
            PipelineFileName = pipelineFileName;
            StepIndex = stepIndex;
        }

        public string Name { get; }
        public string ImageFileName { get; }
        public string PipelineFileName { get; }
        public int StepIndex { get; }
    }

    private sealed class DirectCapture
    {
        public Mat PreviewInput { get; set; }
        public bool ToolSuccess { get; set; }
        public bool AcceptancePassed { get; set; }
        public int ResultWidth { get; set; }
        public int ResultHeight { get; set; }
        public Dictionary<string, double> Metrics { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public List<OverlayValue> Overlays { get; set; } = new();
        public List<ObjectValue> Objects { get; set; } = new();
        public List<GeometryValue> Geometry { get; set; } = new();
    }

    private sealed class OverlayValue
    {
        public string Kind { get; private set; } = string.Empty;
        public string Label { get; private set; } = string.Empty;
        public double BoundsX { get; private set; }
        public double BoundsY { get; private set; }
        public double BoundsWidth { get; private set; }
        public double BoundsHeight { get; private set; }
        public double CenterX { get; private set; }
        public double CenterY { get; private set; }
        public double StartX { get; private set; }
        public double StartY { get; private set; }
        public double EndX { get; private set; }
        public double EndY { get; private set; }
        public double Angle { get; private set; }
        public int PointCount { get; private set; }

        public static OverlayValue From(VisionToolOverlay overlay)
        {
            return new OverlayValue
            {
                Kind = overlay?.Kind.ToString() ?? string.Empty,
                Label = overlay?.Label ?? string.Empty,
                BoundsX = overlay?.Bounds.X ?? 0D,
                BoundsY = overlay?.Bounds.Y ?? 0D,
                BoundsWidth = overlay?.Bounds.Width ?? 0D,
                BoundsHeight = overlay?.Bounds.Height ?? 0D,
                CenterX = overlay?.Center.X ?? 0D,
                CenterY = overlay?.Center.Y ?? 0D,
                StartX = overlay?.Start.X ?? 0D,
                StartY = overlay?.Start.Y ?? 0D,
                EndX = overlay?.End.X ?? 0D,
                EndY = overlay?.End.Y ?? 0D,
                Angle = overlay?.Angle ?? 0D,
                PointCount = overlay?.Points?.Count ?? 0
            };
        }

        public void Compare(VisionRecipeOverlaySummary actual, string prefix, List<string> mismatches)
        {
            if (!string.Equals(Kind, actual.Kind, StringComparison.Ordinal)
                || !string.Equals(Label, actual.Label, StringComparison.Ordinal)
                || PointCount != actual.PointCount
                || !Close(BoundsX, actual.BoundsX)
                || !Close(BoundsY, actual.BoundsY)
                || !Close(BoundsWidth, actual.BoundsWidth)
                || !Close(BoundsHeight, actual.BoundsHeight)
                || !Close(CenterX, actual.CenterX)
                || !Close(CenterY, actual.CenterY)
                || !Close(StartX, actual.StartX)
                || !Close(StartY, actual.StartY)
                || !Close(EndX, actual.EndX)
                || !Close(EndY, actual.EndY)
                || !Close(Angle, actual.Angle))
            {
                mismatches.Add(prefix + " coordinate/identity changed");
            }
        }
    }

    private sealed class ObjectValue
    {
        public int Number { get; private set; }
        public string CandidateId { get; private set; } = string.Empty;
        public bool Accepted { get; private set; }
        public double Area { get; private set; }
        public double CenterX { get; private set; }
        public double CenterY { get; private set; }
        public int BoundsX { get; private set; }
        public int BoundsY { get; private set; }
        public int BoundsWidth { get; private set; }
        public int BoundsHeight { get; private set; }

        public static ObjectValue From(VisionPipelineObjectResult item)
        {
            return new ObjectValue
            {
                Number = item.Number,
                CandidateId = item.CandidateId ?? string.Empty,
                Accepted = item.Accepted,
                Area = item.Area,
                CenterX = item.CenterX,
                CenterY = item.CenterY,
                BoundsX = item.BoundsX,
                BoundsY = item.BoundsY,
                BoundsWidth = item.BoundsWidth,
                BoundsHeight = item.BoundsHeight
            };
        }

        public void Compare(VisionPipelineObjectResult actual, string prefix, List<string> mismatches)
        {
            if (Number != actual.Number
                || !string.Equals(CandidateId, actual.CandidateId, StringComparison.Ordinal)
                || Accepted != actual.Accepted
                || !Close(Area, actual.Area)
                || !Close(CenterX, actual.CenterX)
                || !Close(CenterY, actual.CenterY)
                || BoundsX != actual.BoundsX
                || BoundsY != actual.BoundsY
                || BoundsWidth != actual.BoundsWidth
                || BoundsHeight != actual.BoundsHeight)
            {
                mismatches.Add(prefix + " object identity/coordinate changed");
            }
        }
    }

    private sealed class GeometryValue
    {
        public string Identity { get; private set; } = string.Empty;
        public double X1 { get; private set; }
        public double Y1 { get; private set; }
        public double X2 { get; private set; }
        public double Y2 { get; private set; }
        public double CenterX { get; private set; }
        public double CenterY { get; private set; }
        public double RadiusPx { get; private set; }

        public static GeometryValue From(VisionPipelineGeometryFeatureResult item)
        {
            return new GeometryValue
            {
                Identity = item.Identity,
                X1 = item.X1,
                Y1 = item.Y1,
                X2 = item.X2,
                Y2 = item.Y2,
                CenterX = item.CenterX,
                CenterY = item.CenterY,
                RadiusPx = item.RadiusPx
            };
        }

        public void Compare(VisionPipelineGeometryFeatureResult actual, string prefix, List<string> mismatches)
        {
            if (!string.Equals(Identity, actual.Identity, StringComparison.Ordinal)
                || !Close(X1, actual.X1)
                || !Close(Y1, actual.Y1)
                || !Close(X2, actual.X2)
                || !Close(Y2, actual.Y2)
                || !Close(CenterX, actual.CenterX)
                || !Close(CenterY, actual.CenterY)
                || !Close(RadiusPx, actual.RadiusPx))
            {
                mismatches.Add(prefix + " geometry identity/coordinate changed");
            }
        }
    }

    private static bool Close(double expected, double actual)
    {
        return Math.Abs(expected - actual) <= CoordinateTolerance;
    }
}
