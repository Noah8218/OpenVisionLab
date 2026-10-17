using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Core;
using OpenVisionLab.ImageSpace.Core;
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
using System.Windows.Threading;

internal static class SimplePreprocessPreviewProductionContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string directory = Path.GetFullPath(requestedEvidenceDirectory
            ?? @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-fix-20260922\preview\simplepreprocess-production");
        Require(string.Equals(Path.GetPathRoot(directory), @"D:\", StringComparison.OrdinalIgnoreCase)
            && string.Equals(Path.GetPathRoot(AppPathService.DataRootDirectory), @"D:\", StringComparison.OrdinalIgnoreCase),
            "Evidence and isolated OPENVISIONLAB_DATA_ROOT must remain on D:.");
        Directory.CreateDirectory(directory);
        string recipe = "Smoke_SimplePreview_" + Guid.NewGuid().ToString("N").Substring(0, 12);
        string recipeDirectory = Path.GetFullPath(RecipeWorkspaceService.GetRecipeDirectoryPath(recipe));
        Require(string.Equals(recipeDirectory, Path.GetFullPath(Path.Combine(AppPathService.DataRootDirectory, "RECIPE", recipe)), StringComparison.OrdinalIgnoreCase)
            && !Directory.Exists(recipeDirectory) && !File.Exists(recipeDirectory), "The test recipe must be a new exact child of the isolated D: runtime.");
        PropertyGridEditorRuntime runtime = (PropertyGridEditorRuntime)typeof(PropertyGridEditorFactory)
            .GetField("runtime", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        Func<string> previousRecipeContext = runtime.RecipeNameAccessor;
        _ = Application.Current ?? new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        try
        {
            PropertyGridEditorFactory.SetRecipeNameContext(() => recipe);
            foreach (string tool in new[] { "EdgeDetection", "RotateScale", "Mean", "HSV", "Histogram" })
            {
                Check(tool + " actual document/factory execution", () => RunDocument(tool, directory), passed, failed);
                foreach (string completion in new[] { "publish", "cancel", "discard", "dispose" })
                {
                    Check(tool + " production capture/worker/Mat lifetime/frozen inputs/guarded " + completion,
                        () => RunBoundary(tool, completion, directory), passed, failed);
                }
            }
            foreach (string completion in new[] { "publish", "cancel", "discard", "dispose" })
            {
                Check("Mean real-native empty-input failure review/guarded " + completion + "/valid recovery",
                    () => RunBoundary("Mean", completion, directory, emptyNativeInput: true), passed, failed);
            }
        }
        finally
        {
            try
            {
                Check("Only this run's exact recipe workspace is removed", () =>
                {
                    Require(string.Equals(Path.GetFullPath(RecipeWorkspaceService.GetRecipeDirectoryPath(recipe)), recipeDirectory, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(Path.GetPathRoot(recipeDirectory), @"D:\", StringComparison.OrdinalIgnoreCase), "Recipe cleanup target changed.");
                    if (Directory.Exists(recipeDirectory))
                    {
                        Require((File.GetAttributes(recipeDirectory) & FileAttributes.ReparsePoint) == 0, "Recipe cleanup must not follow a redirected workspace.");
                        Require(RecipeWorkspaceService.DeleteVisionWorkspace(recipe), "Owned recipe cleanup failed.");
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
            "IsolatedRecipe: " + RecipeWorkspaceService.GetRecipeDirectoryPath(recipe),
            "ChecksPassed: " + passed.Count,
            "ChecksFailed: " + failed.Count,
            "Boundary: in-process WPF controls + real registered native delegates; no desktop/theme/DPI claim.",
            "Failure injection passes an owned empty Mat to the real Mean production delegate, never a stub result or test execution override."
        };
        report.AddRange(passed.ConvertAll(item => "PASS: " + item));
        report.AddRange(failed.ConvertAll(item => "FAIL: " + item));
        string reportPath = Path.Combine(directory, "simplepreprocess-preview-production-contract.txt");
        File.WriteAllLines(reportPath, report);
        Console.WriteLine("SIMPLE_PREPROCESS_PREVIEW_PRODUCTION_CONTRACT=" + (failed.Count == 0 ? "PASS" : "FAIL") + "|report=" + reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void RunDocument(string tool, string directory)
    {
        using DisplayManagerService display = CreateDisplay();
        using OpenVisionNativeToolDocument document = CreateDocument(tool, display);
        SimplePreprocessToolWpfView view = PrepareView(document, tool);
        document.RunPreview();
        document.RunPreview();
        WaitUntil(() => document.HasPreviewResult || document.LastStatusText.Contains(" NG /", StringComparison.Ordinal), pumpDispatcher: true);
        Require(document.HasPreviewResult && document.PreviewRunCount == 1, tool + " actual Preview failed or duplicated: " + document.LastStatusText);
        using Bitmap output = display.GetLayerImageSnapshot(document.RouteOutputLayerName);
        Require(output != null && display.SelectedItem == "Main", "Output missing or Preview changed the active input layer.");
        AssertOutput(tool, view, output);
        output.Save(Path.Combine(directory, tool + "-document.png"));
    }

    private static void RunBoundary(string tool, string completion, string directory, bool emptyNativeInput = false)
    {
        using DisplayManagerService display = CreateDisplay();
        using OpenVisionNativeToolDocument document = CreateDocument(tool, display);
        SimplePreprocessToolWpfView view = PrepareView(document, tool);
        Func<OpenVisionNativePreviewOperation> captureProduction = (Func<OpenVisionNativePreviewOperation>?)
            typeof(OpenVisionNativeToolDocument).GetField("capturePreview", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(document)
            ?? throw new InvalidOperationException(tool + " production factory omitted the capture seam.");
        ConcurrentQueue<Action> completions = new ConcurrentQueue<Action>();
        using OpenVisionNativePreviewExecutionBoundary boundary = new OpenVisionNativePreviewExecutionBoundary(
            new OpenVisionNativePreviewExecutionController(display, new OpenVisionNativePreviewLayerPublisher(display)), completions.Enqueue);
        using ManualResetEventSlim computeGate = new ManualResetEventSlim(false);
        int uiThread = Environment.CurrentManagedThreadId;
        int workerThread = 0;
        int captures = 0;
        int publications = 0;
        int statusCount = 0;
        Mat? observedSource = null;
        Mat? observedResult = null;
        OpenVisionNativePreviewExecutionResult? completed = null;
        string outputLayer = tool + "_" + completion + (emptyNativeInput ? "_failure" : string.Empty);
        Func<OpenVisionNativePreviewOperation> capture = () =>
        {
            Require(Environment.CurrentManagedThreadId == uiThread, "Parameter capture left UI thread.");
            captures++;
            OpenVisionNativePreviewOperation production = captureProduction();
            return new OpenVisionNativePreviewOperation(
                source =>
                {
                    workerThread = Environment.CurrentManagedThreadId;
                    observedSource = source;
                    Require(computeGate.Wait(TimeSpan.FromSeconds(10)), "Fixture did not release worker.");
                    VisionToolResult result;
                    if (emptyNativeInput)
                    {
                        using Mat empty = new Mat();
                        result = production.Execute(empty);
                    }
                    else
                    {
                        result = production.Execute(source);
                    }
                    observedResult = result?.ResultImage;
                    return result!;
                },
                () =>
                {
                    Require(Environment.CurrentManagedThreadId == uiThread, "Result publication left UI thread.");
                    publications++;
                    production.PublishEvidence?.Invoke();
                },
                production.PublishEvidenceOnFailure,
                production.Dispose);
        };
        try
        {
            Require(boundary.TryStartSingleInput("Main", outputLayer, "Main", tool != "HSV", null,
                result => { statusCount++; completed = result; }, null, capture), "Production execution was rejected.");
            Require(!boundary.TryStartSingleInput("Main", outputLayer, "Main", true, null,
                _ => throw new InvalidOperationException("Duplicate completion."), null, capture), "Duplicate execution was accepted.");
            ApplyParameters(view, tool, edited: true);
        }
        finally
        {
            computeGate.Set();
        }
        string reviewBefore = view.ResultReviewTextForTest;
        WaitUntil(() => !completions.IsEmpty);
        Require(workerThread != 0 && workerThread != uiThread && captures == 1, "Native work ran on UI or a duplicate captured UI state.");
        Require(observedSource?.IsDisposed == true && (emptyNativeInput || observedResult?.IsDisposed == true),
            "Native source/result survived computation.");
        Require(display.FindIndex(outputLayer) < 0 && view.ResultReviewTextForTest == reviewBefore && !view.SignalInspectorHasEvidenceForTest,
            "Worker prematurely published output/review/evidence.");
        if (completion == "cancel") Require(boundary.Cancel(), "Late cancellation rejected.");
        if (completion == "discard") Require(boundary.CancelAndDiscard(), "Input-change discard rejected.");
        if (completion == "dispose") boundary.Dispose();
        Require(completions.TryDequeue(out Action? publish), "UI completion missing.");
        publish!();
        Require(!boundary.IsRunning, "Active execution did not drain.");
        if (completion == "publish")
        {
            Require(statusCount == 1 && publications == 1 && completed != null && !completed.Canceled,
                "Current execution did not publish exactly once: " + completed?.Status);
            Require(completed != null, "Current execution result is missing.");
            if (emptyNativeInput)
            {
                Require(!completed.Success && display.FindIndex(outputLayer) < 0,
                    "Empty-input native failure became success or created output.");
                Require(view.ResultReviewTextForTest.StartsWith("Mean /", StringComparison.Ordinal)
                    && view.ResultReviewTextForTest.Contains("100-240", StringComparison.Ordinal),
                    "Mean failure review was lost or read later limits: " + view.ResultReviewTextForTest);
            }
            else
            {
                Require(completed.Success, "Production failed: " + completed.Status);
                using Bitmap output = display.GetLayerImageSnapshot(outputLayer);
                Require(output != null && display.SelectedItem == "Main", "Output missing or input layer activation changed.");
                AssertOutput(tool, view, output);
                output.Save(Path.Combine(directory, tool + "-captured.png"));
            }
        }
        else
        {
            Require(publications == 0 && display.FindIndex(outputLayer) < 0
                && view.ResultReviewTextForTest == reviewBefore && !view.SignalInspectorHasEvidenceForTest,
                "Canceled/discarded/closed operation published stale evidence, including failure review.");
            Require(completion == "cancel" ? statusCount == 1 && completed?.Canceled == true : statusCount == 0,
                "Cancellation/discard/close status contract changed.");
        }
        if (emptyNativeInput)
        {
            // The actual document uses its own still-live boundary: valid data must recover after the failed test operation.
            ApplyParameters(view, tool, edited: false);
            document.RunPreview();
            WaitUntil(() => document.HasPreviewResult || document.LastStatusText.Contains(" NG /", StringComparison.Ordinal), pumpDispatcher: true);
            Require(document.HasPreviewResult, "Valid Mean production execution failed to recover: " + document.LastStatusText);
        }
    }

    private static OpenVisionNativeToolDocument CreateDocument(string tool, IDisplayManager display)
    {
        return tool switch
        {
            "EdgeDetection" => OpenVisionNativeSimplePreprocessDocumentFactory.CreateEdgeDetectionDocument(display),
            "RotateScale" => OpenVisionNativeSimplePreprocessDocumentFactory.CreateRotateScaleDocument(display),
            "Mean" => OpenVisionNativeSimplePreprocessDocumentFactory.CreateMeanDocument(display),
            "HSV" => OpenVisionNativeSimplePreprocessDocumentFactory.CreateHsvDocument(display),
            "Histogram" => OpenVisionNativeSimplePreprocessDocumentFactory.CreateHistogramDocument(display),
            _ => throw new ArgumentOutOfRangeException(nameof(tool))
        };
    }

    private static SimplePreprocessToolWpfView PrepareView(OpenVisionNativeToolDocument document, string tool)
    {
        SimplePreprocessToolWpfView view = (SimplePreprocessToolWpfView)document.View;
        view.Measure(new System.Windows.Size(920, 660));
        view.Arrange(new System.Windows.Rect(0, 0, 920, 660));
        view.UpdateLayout();
        view.Dispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
        ApplyParameters(view, tool, edited: false);
        return view;
    }

    private static void ApplyParameters(SimplePreprocessToolWpfView view, string tool, bool edited)
    {
        (string Key, string Value)[] parameters = tool switch
        {
            "EdgeDetection" => new[] { ("EdgeType", edited ? "Sobel" : "Canny"), ("CannyThresholdLow", "100"), ("CannyThresholdHigh", "200"), ("CannyApertureSize", "3"), ("UseL2Gradient", "True") },
            "RotateScale" => new[] { ("Angle", edited ? "90" : "0"), ("ScaleXPercent", edited ? "200" : "100"), ("ScaleYPercent", edited ? "200" : "100") },
            "Mean" => new[] { ("MeanType", "Mean"), ("MeanMin", edited ? "0" : "100"), ("MeanMax", edited ? "0" : "240") },
            "HSV" => new[] { ("HueMin", "0"), ("HueMax", "179"), ("SatMin", "0"), ("SatMax", "255"), ("ValMin", edited ? "0" : "100"), ("ValMax", edited ? "255" : "150") },
            "Histogram" => new[] { ("HistogramType", "Normalize"), ("Alpha", edited ? "0" : "10"), ("Beta", edited ? "5" : "210") },
            _ => throw new ArgumentOutOfRangeException(nameof(tool))
        };
        SimplePreprocessToolSettings settings = new SimplePreprocessToolSettings();
        foreach ((string key, string value) in parameters) settings.Parameters.Add(new ToolParameterValue { Key = key, Value = value });
        view.Parameters.ApplySettings(settings);
    }

    private static void AssertOutput(string tool, SimplePreprocessToolWpfView view, Bitmap output)
    {
        Require(output.Width == 256 && output.Height == 32, tool + " output dimensions changed.");
        if (tool == "Mean")
        {
            Require(view.ResultReviewTextForTest.StartsWith("Mean /", StringComparison.Ordinal)
                && view.ResultReviewTextForTest.Contains("100-240", StringComparison.Ordinal), "Mean review lost captured range.");
            return;
        }
        for (int y = 0; y < output.Height; y++)
        {
            for (int x = 0; x < output.Width; x++)
            {
                Color pixel = output.GetPixel(x, y);
                int expected = tool switch
                {
                    "EdgeDetection" => 0, // Constant ramp gradient is below captured Canny thresholds 100/200.
                    "RotateScale" => x, // Captured zero rotation and 100% scaling preserve all source pixels.
                    "HSV" => x >= 100 && x <= 150 ? x : 0,
                    "Histogram" => (int)Math.Round(10D + x * 200D / 255D),
                    _ => throw new ArgumentOutOfRangeException(nameof(tool))
                };
                Require(pixel.R == expected && pixel.G == expected && pixel.B == expected,
                    tool + " captured output differs at " + x + "," + y + ": expected " + expected + ", actual " + pixel + ".");
            }
        }
        if (tool == "HSV")
        {
            Require(view.ResultReviewTextForTest.Contains("V 100-150", StringComparison.Ordinal), "HSV review read later V range.");
        }
        if (tool == "Histogram")
        {
            VisionToolSignalInspectorView inspector = (VisionToolSignalInspectorView)view.FindName("signalInspector");
            Require(view.SignalInspectorHasEvidenceForTest && inspector.CurrentEvidence.ParameterSummary == "Normalize / Alpha 10 / Beta 210"
                && inspector.CurrentEvidence.InputLayer == "Main", "Histogram evidence does not match captured parameters/input identity.");
        }
    }

    private static DisplayManagerService CreateDisplay()
    {
        DisplayManagerService display = new DisplayManagerService();
        Bitmap image = new Bitmap(256, 32);
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++) image.SetPixel(x, y, Color.FromArgb(x, x, x));
        }
        display.CreateLayerDisplay(ImageSpaceFrame.TakeOwnership(image), "Main", false);
        display.SelectedItem = "Main";
        display.ActivateLayer("Main");
        return display;
    }

    private static void WaitUntil(Func<bool> predicate, bool pumpDispatcher = false)
    {
        Stopwatch clock = Stopwatch.StartNew();
        while (!predicate() && clock.Elapsed < TimeSpan.FromSeconds(10))
        {
            if (pumpDispatcher)
            {
                DispatcherFrame frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false));
                Dispatcher.PushFrame(frame);
            }
            Thread.Sleep(1);
        }
        Require(predicate(), "Timed out waiting for production delegate completion.");
    }

    private static void Check(string name, Action action, ICollection<string> passed, ICollection<string> failed)
    {
        try { action(); passed.Add(name); }
        catch (Exception error) { failed.Add(name + ": " + error.GetType().Name + ": " + error.GetBaseException().Message); }
    }

    private static void Require([DoesNotReturnIf(false)] bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
