using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Core;
using OpenVisionLab.ImageSpace.Core;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WpgPropertyGrid = System.Windows.Controls.WpfPropertyGrid.PropertyGrid;

internal static class NativePreviewProductionDelegateContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-fix-20260922\preview\production-contract");
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Native Preview contract evidence must remain on D:.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        Require(string.Equals(Path.GetPathRoot(AppPathService.DataRootDirectory), @"D:\", StringComparison.OrdinalIgnoreCase),
            "Set OPENVISIONLAB_DATA_ROOT to an isolated D: runtime before launching this contract.");
        string recipeName = "Smoke_NativePreviewProduction_" + Guid.NewGuid().ToString("N").Substring(0, 12);
        string recipeDirectory = Path.GetFullPath(RecipeWorkspaceService.GetRecipeDirectoryPath(recipeName));
        Require(string.Equals(recipeDirectory, Path.GetFullPath(Path.Combine(AppPathService.DataRootDirectory, "RECIPE", recipeName)), StringComparison.OrdinalIgnoreCase)
            && !Directory.Exists(recipeDirectory) && !File.Exists(recipeDirectory), "The test recipe must be a new exact child of the isolated D: runtime.");
        PropertyGridEditorRuntime runtime = (PropertyGridEditorRuntime)typeof(PropertyGridEditorFactory)
            .GetField("runtime", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        Func<string> previousRecipeContext = runtime.RecipeNameAccessor;
        _ = Application.Current ?? new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        try
        {
            PropertyGridEditorFactory.SetRecipeNameContext(() => recipeName);
            Check("Threshold factory/document uses the production delegate and publishes edited T=180",
                () => RunDocument(OpenVisionNativeCustomToolFactory.CreateThreshold, evidenceDirectory), passed, failed);
            Check("Filter factory/document captures its control-bound property before worker execution",
                () => RunDocument(OpenVisionNativeCustomToolFactory.CreateFilter, evidenceDirectory), passed, failed);
            Check("Morphology factory/document captures its control-bound property before worker execution",
                () => RunDocument(OpenVisionNativeCustomToolFactory.CreateMorphology, evidenceDirectory), passed, failed);
            Check("Filter production capture, worker affinity, Mat release and UI publication",
                () => RunBoundary("publish", evidenceDirectory, OpenVisionNativeCustomToolFactory.CreateFilter), passed, failed);
            Check("Morphology production capture, worker affinity, Mat release and UI publication",
                () => RunBoundary("publish", evidenceDirectory, OpenVisionNativeCustomToolFactory.CreateMorphology), passed, failed);
            foreach (string completion in new[] { "publish", "cancel", "discard", "dispose" })
            {
                Check("Production Threshold snapshot, worker affinity, lifetime and guarded " + completion,
                    () => RunBoundary(completion, evidenceDirectory), passed, failed);
            }
            Check("Capture failure releases the active slot and real Threshold execution recovers",
                CheckCaptureFailureRecovery, passed, failed);
            Check("Captured resources and slots release even when cleanup throws during startup, dispatch or completion",
                CheckOperationFailureLifetime, passed, failed);
            foreach ((string name, Func<IDisplayManager, bool, OpenVisionNativeToolDocument> factory) in new[]
            {
                ("Blob", (Func<IDisplayManager, bool, OpenVisionNativeToolDocument>)OpenVisionNativePropertyGridToolFactory.CreateBlob),
                ("Contour", (Func<IDisplayManager, bool, OpenVisionNativeToolDocument>)OpenVisionNativePropertyGridToolFactory.CreateContour)
            })
            {
                Check(name + " production factory/document detects and publishes the managed result review",
                    () => RunDocument(factory, evidenceDirectory), passed, failed);
                foreach (string completion in new[] { "publish", "cancel", "discard", "dispose" })
                {
                    Check(name + " UI capture, worker affinity, ROI snapshot, frozen result criteria, Mat release and guarded " + completion,
                        () => RunBoundary(completion, evidenceDirectory, factory), passed, failed);
                }
                Check(name + " production teaching flag is captured/consumed on UI and does not publish detection review",
                    () => RunBoundary("publish", evidenceDirectory, factory, teachingPreview: true), passed, failed);
            }
        }
        finally
        {
            try
            {
                Check("Only this run's exact recipe workspace is removed", () =>
                {
                    Require(string.Equals(Path.GetFullPath(RecipeWorkspaceService.GetRecipeDirectoryPath(recipeName)), recipeDirectory, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(Path.GetPathRoot(recipeDirectory), @"D:\", StringComparison.OrdinalIgnoreCase), "Recipe cleanup target changed.");
                    if (Directory.Exists(recipeDirectory))
                    {
                        Require((File.GetAttributes(recipeDirectory) & FileAttributes.ReparsePoint) == 0, "Recipe cleanup must not follow a redirected workspace.");
                        Require(RecipeWorkspaceService.DeleteVisionWorkspace(recipeName), "Owned recipe cleanup failed.");
                    }
                    Require(!Directory.Exists(recipeDirectory), "Owned recipe workspace remains after cleanup.");
                }, passed, failed);
            }
            finally
            {
                PropertyGridEditorFactory.SetRecipeNameContext(previousRecipeContext);
            }
        }

        List<string> report = new List<string>
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "Assembly: " + typeof(OpenVisionNativeToolDocument).Assembly.Location,
            "IsolatedRecipe: " + RecipeWorkspaceService.GetRecipeDirectoryPath(recipeName),
            "ChecksPassed: " + passed.Count,
            "ChecksFailed: " + failed.Count,
            "Boundary: In-process WPF controls and real native production delegates; no desktop interaction, theme or DPI claim.",
            "No SetPreviewExecutionOverrideForTest or replacement native algorithm is used."
        };
        report.AddRange(passed.ConvertAll(item => "PASS: " + item));
        report.AddRange(failed.ConvertAll(item => "FAIL: " + item));
        string reportPath = Path.Combine(evidenceDirectory, "native-preview-production-delegate-contract.txt");
        File.WriteAllLines(reportPath, report);
        Console.WriteLine("NATIVE_PREVIEW_PRODUCTION_DELEGATE_CONTRACT=" + (failed.Count == 0 ? "PASS" : "FAIL") + "|report=" + reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void RunDocument(Func<IDisplayManager, bool, OpenVisionNativeToolDocument> createDocument, string evidenceDirectory)
    {
        using DisplayManagerService display = CreateDisplay();
        using OpenVisionNativeToolDocument document = createDocument(display, false);
        ThresholdToolWpfView? threshold = document.View as ThresholdToolWpfView;
        if (threshold != null)
        {
            SetThreshold(threshold, "180");
        }
        OpenCvPropertyBase? areaProperty = PrepareAreaView(document);
        document.RunPreview();
        document.RunPreview();
        WaitUntil(() => document.HasPreviewResult || document.LastStatusText.Contains(" NG /", StringComparison.Ordinal), pumpDispatcher: true);
        Require(document.HasPreviewResult, document.ToolName + " Preview failed: " + document.LastStatusText);
        Require(document.PreviewRunCount == 1, "Repeated Preview started more than one active operation.");
        Require(display.SelectedItem == "Main", "Preview changed the active input layer.");
        using Bitmap output = display.GetLayerImageSnapshot(document.RouteOutputLayerName);
        Require(output != null && output.Width == 256 && output.Height == 8, "Published native output missing.");
        if (threshold != null)
        {
            AssertThresholdPixels(output, 180);
            Require(threshold.SignalInspectorHasEvidenceForTest, "Successful production Preview did not publish signal evidence.");
            Require(threshold.GetSignalInspectorMarkerValueForTest("Threshold") == 180, "Signal marker does not match the edited threshold.");
            output.Save(Path.Combine(evidenceDirectory, "threshold-T180.png"));
        }
        if (areaProperty != null)
        {
            AssertAreaReview(document);
        }
    }

    private static void RunBoundary(string completion, string evidenceDirectory, Func<IDisplayManager, bool, OpenVisionNativeToolDocument>? createDocument = null, bool teachingPreview = false)
    {
        using DisplayManagerService display = CreateDisplay();
        using OpenVisionNativeToolDocument document = (createDocument ?? OpenVisionNativeCustomToolFactory.CreateThreshold)(display, false);
        ThresholdToolWpfView? view = document.View as ThresholdToolWpfView;
        if (view != null)
        {
            SetThreshold(view, "64");
        }
        OpenCvPropertyBase? areaProperty = PrepareAreaView(document);
        VisionToolSingleInputPropertyToolShell? areaShell = areaProperty == null
            ? null : (VisionToolSingleInputPropertyToolShell)document.View.FindName("toolShell");
        if (teachingPreview)
        {
            // Seed the existing auto-preview controller state, not a replacement execution delegate.
            VisionToolThresholdTeachingPreviewController teaching = (VisionToolThresholdTeachingPreviewController?)
                document.View.GetType().GetField("thresholdTeachingPreviewController", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(document.View)
                ?? throw new InvalidOperationException("Area teaching controller is missing.");
            teaching.Request();
        }
        string? reviewBefore = areaShell?.ResultReviewText.Text;
        string? guidanceBefore = areaShell?.ResultGuidanceText.Text;
        string? capturedCriteria = areaProperty is BlobProperty blobCriteria
            ? VisionToolAreaVerificationCriteriaText.CreateBlob(blobCriteria)
            : areaProperty is ContourProperty contourCriteria
                ? VisionToolAreaVerificationCriteriaText.CreateContour(contourCriteria) : null;
        // Observe the exact operation wired by the production factory, without replacing its native delegate.
        Func<OpenVisionNativePreviewOperation> captureProduction = (Func<OpenVisionNativePreviewOperation>?)
            typeof(OpenVisionNativeToolDocument).GetField("capturePreview", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(document)
            ?? throw new InvalidOperationException(document.ToolName + " factory did not wire a UI capture operation.");
        ConcurrentQueue<Action> uiCompletions = new ConcurrentQueue<Action>();
        using OpenVisionNativePreviewExecutionBoundary boundary = new OpenVisionNativePreviewExecutionBoundary(
            new OpenVisionNativePreviewExecutionController(display, new OpenVisionNativePreviewLayerPublisher(display)),
            uiCompletions.Enqueue);
        int uiThread = Environment.CurrentManagedThreadId;
        int workerThread = 0;
        int applyCount = 0;
        int evidencePublishCount = 0;
        int captureCount = 0;
        int releaseCount = 0;
        Mat? observedSource = null;
        Mat? observedResult = null;
        OpenVisionNativePreviewExecutionResult? completed = null;
        string outputLayer = document.ToolName + "_" + completion;
        Func<OpenVisionNativePreviewOperation> capture = () =>
        {
            Require(Environment.CurrentManagedThreadId == uiThread, "Capture left the UI thread.");
            captureCount++;
            OpenVisionNativePreviewOperation production = captureProduction();
            return new OpenVisionNativePreviewOperation(
                source =>
                {
                    workerThread = Environment.CurrentManagedThreadId;
                    observedSource = source;
                    VisionToolResult result = production.Execute(source);
                    observedResult = result.ResultImage;
                    return result;
                },
                () =>
                {
                    Require(Environment.CurrentManagedThreadId == uiThread, "Evidence publication left the UI thread.");
                    evidencePublishCount++;
                    production.PublishEvidence?.Invoke();
                }, production.PublishEvidenceOnFailure, () => { production.Dispose(); releaseCount++; });
        };
        Require(boundary.TryStartSingleInput("Main", outputLayer, "Main", true, null,
            result => { applyCount++; completed = result; }, null, capture), "Production run was not accepted.");
        Require(!boundary.TryStartSingleInput("Main", outputLayer, "Main", true, null,
            _ => throw new InvalidOperationException("Duplicate completed."), null, capture), "Duplicate run was accepted.");
        if (view != null)
        {
            SetThreshold(view, "220");
        }
        if (areaProperty != null)
        {
            areaProperty.THRESHOLD = 220;
            areaProperty.CvROIS.Clear();
            areaProperty.CvMASKS.Add(new OpenCvSharp.Rect(0, 0, 256, 8));
            if (areaProperty is BlobProperty editedBlob)
            {
                editedBlob.MIN_AREA = 900000;
                editedBlob.MIN_WIDTH = 192;
                editedBlob.MIN_HEIGHT = 9;
            }
            else if (areaProperty is ContourProperty editedContour)
            {
                editedContour.MIN_AREA = 900000;
                editedContour.MIN_WIDTH = 192;
                editedContour.MIN_HEIGHT = 9;
            }
            bool pendingTeaching = document.View is BlobToolWpfView blob
                ? blob.ConsumeThresholdTeachingPreviewRequest()
                : ((ContourToolWpfView)document.View).ConsumeThresholdTeachingPreviewRequest();
            Require(!pendingTeaching, "UI capture did not consume the teaching request before worker execution.");
        }
        WaitUntil(() => !uiCompletions.IsEmpty);
        Require(workerThread != 0 && workerThread != uiThread, "Native computation ran on the UI thread.");
        Require(captureCount == 1, "Rejected duplicate captured mutable UI state.");
        Require(view?.SignalInspectorHasEvidenceForTest != true && display.FindIndex(outputLayer) < 0,
            "Worker published UI evidence or an output layer before the UI completion.");
        Require(areaShell?.ResultReviewText.Text == reviewBefore, "Worker changed the area result review before UI completion.");
        Require(areaShell?.ResultGuidanceText.Text == guidanceBefore, "Worker changed result criteria before UI completion.");
        Require(observedSource?.IsDisposed == true && observedResult?.IsDisposed == true,
            "Native input/result Mat was retained past computation.");

        if (completion == "cancel")
        {
            Require(boundary.Cancel(), "Late cancellation was rejected.");
        }
        else if (completion == "discard")
        {
            Require(boundary.CancelAndDiscard(), "Input-change discard was rejected.");
        }
        else if (completion == "dispose")
        {
            boundary.Dispose();
        }
        Require(uiCompletions.TryDequeue(out Action? apply), "UI completion was lost.");
        apply!();
        Require(!boundary.IsRunning, "Active operation did not drain.");
        Require(releaseCount == 1, "Captured operation resources were not released exactly once.");
        if (completion == "publish")
        {
            Require(completed?.Success == true && applyCount == 1 && evidencePublishCount == 1,
                "Successful output/evidence were not published exactly once: " + completed?.Status);
            using Bitmap output = display.GetLayerImageSnapshot(outputLayer);
            Require(output != null && output.Width == 256 && output.Height == 8, "Native output missing after UI publication.");
            Require(display.SelectedItem == "Main", "UI publication changed the input layer.");
            if (view != null)
            {
                Require(view.GetSignalInspectorMarkerValueForTest("Threshold") == 64,
                    "Worker read the later T=220 instead of captured T=64.");
                AssertThresholdPixels(output, 64);
                output.Save(Path.Combine(evidenceDirectory, "threshold-captured-T64.png"));
            }
            if (areaProperty != null)
            {
                if (teachingPreview)
                {
                    AssertThresholdPixels(output, 64);
                    Require(areaShell!.ResultReviewText.Text == reviewBefore, "Threshold teaching unexpectedly published detection results.");
                    Require(areaShell.ResultGuidanceText.Text == guidanceBefore, "Threshold teaching unexpectedly published detection criteria.");
                }
                else
                {
                    AssertAreaReview(document);
                    VisionToolVerificationGuideView guide = (VisionToolVerificationGuideView)areaShell!.ToolContent;
                    Require(capturedCriteria != null && areaShell.ResultGuidanceText.Text.Contains(capturedCriteria, StringComparison.Ordinal)
                        && guide.CriteriaText.Contains(capturedCriteria, StringComparison.Ordinal),
                        document.ToolName + " result/guide criteria did not retain executed T=64, area 1-1000000, W/H 0-* after later edits: "
                        + areaShell.ResultGuidanceText.Text + " | " + guide.CriteriaText);
                }
                output.Save(Path.Combine(evidenceDirectory, document.ToolName + (teachingPreview ? "-teaching-T64.png" : "-captured-T64.png")));
            }
        }
        else
        {
            Require(evidencePublishCount == 0 && view?.SignalInspectorHasEvidenceForTest != true && display.FindIndex(outputLayer) < 0,
                "Canceled/discarded/closed operation published stale output or evidence.");
            Require(completion == "cancel" ? applyCount == 1 && completed?.Canceled == true : applyCount == 0,
                "Cancel/discard/close status callback contract changed.");
            Require(areaShell?.ResultReviewText.Text == reviewBefore, "Canceled/discarded/closed area result review was published.");
            Require(areaShell?.ResultGuidanceText.Text == guidanceBefore, "Canceled/discarded/closed result criteria were published.");
        }
    }

    private static OpenCvPropertyBase? PrepareAreaView(OpenVisionNativeToolDocument document)
    {
        if (document.View is not BlobToolWpfView && document.View is not ContourToolWpfView)
        {
            return null;
        }
        VisionToolSingleInputPropertyToolShell shell = (VisionToolSingleInputPropertyToolShell)document.View.FindName("toolShell");
        WpgPropertyGrid grid = (WpgPropertyGrid)shell.PropertyGridHost.Child;
        OpenCvPropertyBase property = (OpenCvPropertyBase)grid.SelectedObject;
        property.USE_THRESHOLD = true;
        property.THRESHOLD = 64;
        property.THRESHOLD_TYPES = ThresholdTypes.Binary;
        property.USE_BITWISENOT = false;
        property.USE_ADAPTIVE_THRESHOLD = false;
        property.USE_ROI = true;
        property.USE_MULTI_ROI = true;
        property.CvROI = new OpenCvSharp.Rect(0, 0, 256, 8);
        property.CvROIS = new List<OpenCvSharp.Rect> { property.CvROI };
        property.USE_MASKING = true;
        property.CvMASKS = new List<OpenCvSharp.Rect>();
        if (property is BlobProperty blob)
        {
            blob.MIN_AREA = 1;
            blob.MAX_AREA = 1000000;
            blob.MIN_WIDTH = blob.MIN_HEIGHT = 0;
            blob.MAX_WIDTH = blob.MAX_HEIGHT = 1000000;
        }
        else if (property is ContourProperty contour)
        {
            contour.MIN_AREA = 1;
            contour.MAX_AREA = 1000000;
            contour.MIN_WIDTH = contour.MIN_HEIGHT = 0;
            contour.MAX_WIDTH = contour.MAX_HEIGHT = 1000000;
            contour.USE_APPROXPOLYDP = false;
            contour.DetectMode = RetrievalModes.External;
        }
        grid.RefreshSelectedObject();
        document.View.Measure(new System.Windows.Size(900, 620));
        document.View.Arrange(new System.Windows.Rect(0, 0, 900, 620));
        document.View.UpdateLayout();
        document.View.Dispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
        Require(property.THRESHOLD == 64 && property.CvROIS.Count == 1, "Area fixture did not retain valid T=64/full-image ROI.");
        return property;
    }

    private static void AssertAreaReview(OpenVisionNativeToolDocument document)
    {
        VisionToolSingleInputPropertyToolShell shell = (VisionToolSingleInputPropertyToolShell)document.View.FindName("toolShell");
        // Ramp pixels x=65..255 produce one 191x8 object; contour area excludes its one-pixel boundary.
        double expectedArea = document.View is BlobToolWpfView ? 191 * 8 : 190 * 7;
        // SDK f4f0c0 ContourTool uses integer bounding-box center, not Blob's pixel centroid.
        double expectedCenterY = document.View is BlobToolWpfView ? 3.5 : 8 / 2;
        string expected = VisionToolVerificationText.FormatAreaSummary(document.ToolName, 1, expectedArea, 160, expectedCenterY, 191, 8);
        Require(shell.ResultReviewText.Text == expected,
            document.ToolName + " managed result review differs from captured T=64 ROI: expected='" + expected + "', actual='" + shell.ResultReviewText.Text + "'.");
    }

    private static void CheckCaptureFailureRecovery()
    {
        using DisplayManagerService display = CreateDisplay();
        using OpenVisionNativeToolDocument document = OpenVisionNativeCustomToolFactory.CreateThreshold(display);
        ThresholdToolWpfView view = (ThresholdToolWpfView)document.View;
        ConcurrentQueue<Action> uiCompletions = new ConcurrentQueue<Action>();
        using OpenVisionNativePreviewExecutionBoundary boundary = new OpenVisionNativePreviewExecutionBoundary(
            new OpenVisionNativePreviewExecutionController(display, new OpenVisionNativePreviewLayerPublisher(display)), uiCompletions.Enqueue);
        OpenVisionNativePreviewExecutionResult? completed = null;
        Require(boundary.TryStartSingleInput("Main", "CaptureFailure", "Main", true, null,
            result => completed = result, null, () => throw new InvalidOperationException("controlled capture failure")),
            "Capture failure was not reported as an accepted failed attempt.");
        Require(!boundary.IsRunning && completed?.Success == false
            && completed.Status.Contains("controlled capture failure", StringComparison.Ordinal),
            "Capture failure leaked the active slot or hid its error.");
        Require(display.FindIndex("CaptureFailure") < 0 && uiCompletions.IsEmpty, "Capture failure started native computation.");
        SetThreshold(view, "180");
        Require(boundary.TryStartSingleInput("Main", "Recovered", "Main", true, null,
            result => completed = result, null, () => OpenVisionNativeThresholdPreviewExecutor.Capture(view)),
            "Real production recovery run was rejected.");
        WaitUntil(() => !uiCompletions.IsEmpty);
        Require(uiCompletions.TryDequeue(out Action? apply), "Recovery completion missing.");
        apply!();
        Require(completed?.Success == true && !boundary.IsRunning && view.SignalInspectorHasEvidenceForTest,
            "Real production run did not recover after capture failure: " + completed?.Status);
    }

    private static void CheckOperationFailureLifetime()
    {
        using DisplayManagerService display = CreateDisplay();
        foreach (string completion in new[] { "startup", "dispatch", "publish", "cancel", "discard", "dispose" })
        {
            foreach (bool releaseThrows in new[] { false, true })
            {
                int releases = 0;
                int callbacks = 0;
                OpenVisionNativePreviewExecutionResult? completed = null;
                OpenVisionNativePreviewOperation? captured = null;
                ConcurrentQueue<Action> uiCompletions = new();
                using OpenVisionNativePreviewExecutionBoundary boundary = new(
                    new OpenVisionNativePreviewExecutionController(display, new OpenVisionNativePreviewLayerPublisher(display)),
                    action =>
                    {
                        if (completion == "dispatch") throw new InvalidOperationException("controlled dispatch failure");
                        uiCompletions.Enqueue(action);
                    });
                using Mat ownedTemplate = new(2, 2, MatType.CV_8UC1, Scalar.All(127));
                string outputLayer = "FailureLifetime_" + completion + "_" + releaseThrows;
                Require(boundary.TryStartSingleInput("Main", outputLayer, "Main", true, null,
                    result => { completed = result; callbacks++; },
                    completion == "startup" ? () => throw new InvalidOperationException("controlled start failure") : null,
                    () => captured = new OpenVisionNativePreviewOperation(
                        source => VisionToolResult.Passed(source.Clone(), TimeSpan.Zero),
                        release: () =>
                        {
                            ownedTemplate.Dispose();
                            Interlocked.Increment(ref releases);
                            if (releaseThrows) throw new InvalidOperationException("controlled release failure");
                        })), "Failure lifetime run rejected.");
                if (completion != "startup" && completion != "dispatch")
                {
                    WaitUntil(() => !uiCompletions.IsEmpty);
                    if (completion == "cancel") Require(boundary.Cancel(), "Cleanup cancel rejected.");
                    else if (completion == "discard") Require(boundary.CancelAndDiscard(), "Cleanup discard rejected.");
                    else if (completion == "dispose") boundary.Dispose();
                    Require(uiCompletions.TryDequeue(out Action? apply), "Cleanup completion missing.");
                    apply!();
                }
                WaitUntil(() => !boundary.IsRunning);
                Require(Volatile.Read(ref releases) == 1 && ownedTemplate.IsDisposed,
                    completion + " retained captured resources after cleanup (throws=" + releaseThrows + ").");
                if (completion == "publish")
                {
                    Require(callbacks == 1 && completed?.Success == !releaseThrows, "Cleanup must produce one final success/failure callback.");
                }
                else
                {
                    Require(display.FindIndex(outputLayer) < 0, "Failed/canceled/discarded/closed run published an output.");
                    Require(callbacks == (completion == "startup" || completion == "cancel" ? 1 : 0), "Failure cleanup callback count changed.");
                }
                if (releaseThrows && callbacks == 1)
                {
                    Require(completed?.Success == false && completed.Status.Contains("controlled release failure", StringComparison.Ordinal),
                        "UI failure omitted the cleanup exception.");
                }
                if (completion == "startup")
                {
                    Require(completed?.Status.Contains("controlled start failure", StringComparison.Ordinal) == true, "Startup exception was replaced by cleanup failure.");
                }
                captured!.Dispose();
                Require(releases == 1, "Operation disposal is not idempotent after release failure.");
            }
        }

        using Bitmap sourceBitmap = new Bitmap(2, 2);
        using OpenVisionNativePreviewInputSnapshot snapshot = new(sourceBitmap, "Output", "Main", true);
        using OpenVisionNativePreviewComputation computation = OpenVisionNativePreviewComputation.Passed(new Bitmap(2, 2), TimeSpan.Zero);
        using OpenVisionNativePreviewOperation failingRelease = new(_ => null, release: () => throw new InvalidOperationException("controlled release failure"));
        Exception? cleanupFailure = (Exception?)typeof(OpenVisionNativePreviewExecutionBoundary)
            .GetMethod("ReleaseResources", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, new object[] { failingRelease, computation, snapshot });
        bool sourceDisposed = false;
        try { _ = sourceBitmap.Width; }
        catch (Exception ex) when (ex is ArgumentException or ObjectDisposedException) { sourceDisposed = true; }
        Require(cleanupFailure?.Message == "controlled release failure" && computation.ResultBitmap == null && sourceDisposed,
            "A throwing operation release prevented result/source bitmap disposal.");
    }

    private static DisplayManagerService CreateDisplay()
    {
        DisplayManagerService display = new DisplayManagerService();
        Bitmap input = new Bitmap(256, 8);
        for (int y = 0; y < input.Height; y++)
        {
            for (int x = 0; x < input.Width; x++)
            {
                input.SetPixel(x, y, Color.FromArgb(x, x, x));
            }
        }
        display.CreateLayerDisplay(ImageSpaceFrame.TakeOwnership(input), "Main", false);
        display.SelectedItem = "Main";
        display.ActivateLayer("Main");
        return display;
    }

    private static void SetThreshold(ThresholdToolWpfView view, string value)
    {
        // A factory-created, unshown View still needs its initial DataContext bindings transferred.
        view.Measure(new System.Windows.Size(900, 620));
        view.Arrange(new System.Windows.Rect(0, 0, 900, 620));
        view.UpdateLayout();
        view.Dispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
        TextBox editor = (TextBox)view.FindName("txtThreshold");
        Require(editor != null, "Threshold input is missing from compiled XAML.");
        view.ConfigureBasicInvertForTest(false);
        editor.SetCurrentValue(TextBox.TextProperty, value);
        editor.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        view.Dispatcher.Invoke(() => { }, DispatcherPriority.DataBind);
        double actual = view.CreateProperty().Threshold;
        Require(actual == double.Parse(value, System.Globalization.CultureInfo.InvariantCulture),
            $"Threshold editor did not update its parameter: expected={value}, actual={actual}, text='{editor.Text}', binding={editor.GetBindingExpression(TextBox.TextProperty)?.Status}, context={editor.DataContext?.GetType().Name}.");
    }

    private static void AssertThresholdPixels(Bitmap output, int threshold)
    {
        Require(output != null && output.Width == 256 && output.Height == 8, "Threshold result dimensions differ.");
        for (int y = 0; y < output.Height; y++)
        {
            for (int x = 0; x < output.Width; x++)
            {
                Color pixel = output.GetPixel(x, y);
                int expected = x > threshold ? 255 : 0;
                Require(pixel.R == expected && pixel.G == expected && pixel.B == expected,
                    "Threshold pixel differs at " + x + "," + y + "; expected " + expected + ".");
            }
        }
    }

    private static void WaitUntil(Func<bool> predicate, bool pumpDispatcher = false)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (!predicate() && stopwatch.Elapsed < TimeSpan.FromSeconds(10))
        {
            if (pumpDispatcher)
            {
                DispatcherFrame frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false));
                Dispatcher.PushFrame(frame);
            }
            Thread.Sleep(1);
        }
        Require(predicate(), "Timed out waiting for production Preview completion.");
    }

    private static void Check(string name, Action action, ICollection<string> passed, ICollection<string> failed)
    {
        try
        {
            action();
            passed.Add(name);
        }
        catch (Exception exception)
        {
            failed.Add(name + ": " + exception.GetType().Name + ": " + exception.Message);
        }
    }

    private static void Require([DoesNotReturnIf(false)] bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
