using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Core;
using OpenVisionLab.ImageSpace.Core;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using OpenVisionLab.Vision2D.Tool;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using WpgPropertyGrid = System.Windows.Controls.WpfPropertyGrid.PropertyGrid;

internal static class MatchingLineAffinePreviewContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-fix-20260922\matching-line-affine");
        Require(string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase)
            && string.Equals(Path.GetPathRoot(AppPathService.DataRootDirectory), @"D:\", StringComparison.OrdinalIgnoreCase),
            "Contract evidence and isolated OPENVISIONLAB_DATA_ROOT must be on D:.");
        Directory.CreateDirectory(evidenceDirectory);
        _ = Application.Current ?? new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        List<string> results = new();
        foreach (string tool in new[] { "Matching", "EdgeBasedMatching", "FeatureMatching", "Line", "AffineTransform" })
        {
            Check(tool + " real factory/document explicit Preview", () => RunDocument(tool, evidenceDirectory), results);
            foreach (string completion in new[] { "publish", "cancel", "discard", "dispose" })
            {
                Check(tool + " captured property/template, worker affinity, release and guarded " + completion,
                    () => RunBoundary(tool, completion, evidenceDirectory), results);
            }
        }
        foreach (string purpose in new[] { "Measure", "Intersection" })
        {
            Check("Line " + purpose + " production algorithm and managed review", () => RunDocument("Line", evidenceDirectory, purpose), results);
        }
        foreach (string tool in new[] { "Matching", "Line", "AffineTransform" })
        {
            Check(tool + " failure review and same-document recovery", () => RunFailureRecovery(tool, evidenceDirectory), results);
        }
        Check("Line threshold teaching request is consumed before worker and does not publish detection review",
            () => RunBoundary("Line", "publish", evidenceDirectory, teachingPreview: true), results);
        bool passed = results.All(item => item.StartsWith("PASS: ", StringComparison.Ordinal));
        string report = Path.Combine(evidenceDirectory, "matching-line-affine-preview-contract.txt");
        File.WriteAllLines(report, new[]
        {
            "Status: " + (passed ? "PASS" : "FAIL"),
            "Assembly: " + typeof(OpenVisionNativeToolDocument).Assembly.Location,
            "Boundary: in-process real factory/View/capture/native algorithm; no desktop, theme or DPI claim.",
            "No execution override or substitute native algorithm. Generated templates remain in D: evidence; exact generated recipe workspaces are cleaned in finally."
        }.Concat(results));
        Console.WriteLine("MATCHING_LINE_AFFINE_PREVIEW_CONTRACT=" + (passed ? "PASS" : "FAIL") + "|report=" + report);
        return passed ? 0 : 1;
    }

    private static void RunDocument(string tool, string evidenceDirectory, string purpose = "Edge")
    {
        using DisplayManagerService display = CreateDisplay(tool, purpose);
        using OpenVisionNativeToolDocument document = CreateDocument(tool, display);
        Prepare(document, display, evidenceDirectory, purpose);
        document.RunPreview();
        document.RunPreview();
        WaitUntil(() => document.HasPreviewResult || document.LastStatusText.Contains(" NG /", StringComparison.Ordinal), true);
        Require(document.HasPreviewResult, tool + " failed: " + document.LastStatusText);
        Require(document.PreviewRunCount == 1 && display.SelectedItem == "Main", "Preview re-entry or active layer changed.");
        Require(!string.IsNullOrWhiteSpace(Review(document)), "Successful native Preview did not publish review.");
        using Bitmap output = display.GetLayerImageSnapshot(document.RouteOutputLayerName);
        Require(output != null && output.Width == 512 && output.Height == 384, "Native output is absent or has wrong bounds.");
        output.Save(Path.Combine(evidenceDirectory, tool + "-" + purpose + ".png"));
    }

    private static void RunBoundary(string tool, string completion, string evidenceDirectory, bool teachingPreview = false)
    {
        using DisplayManagerService display = CreateDisplay(tool, "Edge");
        using OpenVisionNativeToolDocument document = CreateDocument(tool, display);
        OpenCvPropertyBase property = Prepare(document, display, evidenceDirectory, "Edge");
        OpenCvSharp.Rect capturedRoi = new(1, 2, 3, 4);
        OpenCvSharp.Rect capturedMask = new(7, 8, 9, 10);
        property.CvROIS.Add(capturedRoi);
        property.CvMASKS.Add(capturedMask);
        string? expectedLineCriteria = document.View is LineToolWpfView capturedLine
            ? VisionToolVerificationText.FormatLineCriteria(
                VisionToolVerificationText.CreateLinePurposeText("Edge"), capturedLine.SelectedLineName,
                ((LineGaugeProperty)property).CONTRAST, ((LineGaugeProperty)property).SAMPLING_STEP,
                property.USE_ROI ? VisionToolVerificationText.RoiOn : VisionToolVerificationText.FullImage)
            : null;
        if (teachingPreview)
        {
            property.USE_THRESHOLD = true;
            property.THRESHOLD = 120;
            LineToolPreviewController teaching = (LineToolPreviewController?)
                document.View.GetType().GetField("previewController", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(document.View)
                ?? throw new InvalidOperationException("Line teaching controller missing.");
            FieldInfo request = typeof(LineToolPreviewController).GetField("thresholdTeachingPreviewRequested", BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("Line teaching request state missing.");
            request.SetValue(teaching, true);
        }
        Func<OpenVisionNativePreviewOperation> captureProduction = (Func<OpenVisionNativePreviewOperation>?)
            typeof(OpenVisionNativeToolDocument).GetField("capturePreview", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(document)
            ?? throw new InvalidOperationException("Factory has no capture seam: " + tool);
        ConcurrentQueue<Action> completions = new();
        using OpenVisionNativePreviewExecutionBoundary boundary = new(
            new OpenVisionNativePreviewExecutionController(display, new OpenVisionNativePreviewLayerPublisher(display)), completions.Enqueue);
        using ManualResetEventSlim allowWorker = new(false);
        int uiThread = Environment.CurrentManagedThreadId;
        int workerThread = 0, captures = 0, publishes = 0, releases = 0, applications = 0;
        Mat? input = null, output = null, ownedTemplate = null;
        OpenCvPropertyBase[] capturedProperties = Array.Empty<OpenCvPropertyBase>();
        OpenVisionNativePreviewExecutionResult? completed = null;
        string reviewBefore = Review(document);
        string outputLayer = tool + "_Boundary";
        Func<OpenVisionNativePreviewOperation> capture = () =>
        {
            Require(Environment.CurrentManagedThreadId == uiThread, "Capture was not on UI.");
            captures++;
            OpenVisionNativePreviewOperation production = captureProduction();
            capturedProperties = GetCapturedProperties(production).Where(item => item.NAME == property.NAME).ToArray();
            ownedTemplate = capturedProperties.Select(GetTemplate).FirstOrDefault(item => item != null);
            return new OpenVisionNativePreviewOperation(
                source =>
                {
                    workerThread = Environment.CurrentManagedThreadId;
                    input = source;
                    Require(allowWorker.Wait(TimeSpan.FromSeconds(10)), "Worker release timed out.");
                    VisionToolResult result = production.Execute(source);
                    output = result.ResultImage;
                    return result;
                },
                () =>
                {
                    Require(Environment.CurrentManagedThreadId == uiThread, "Review publish was not on UI.");
                    publishes++;
                    production.PublishEvidence?.Invoke();
                }, production.PublishEvidenceOnFailure,
                () => { production.Dispose(); releases++; });
        };
        Require(boundary.TryStartSingleInput("Main", outputLayer, "Main", true, null,
            result => { completed = result; applications++; }, null, capture), "Run rejected.");
        Require(!boundary.TryStartSingleInput("Main", outputLayer, "Main", true, null, _ => { }, null, capture), "Duplicate run accepted.");
        // Change mutable inputs after synchronous capture, before allowing the native worker to use them.
        property.CvROIS.Clear();
        property.CvMASKS.Add(new OpenCvSharp.Rect(0, 0, 512, 384));
        if (property is MatchingProperty matching) matching.SCORE_MIN = 0.9999;
        if (property is EdgeBasedMatchingProperty edge) edge.SCORE_MIN = 0.9999;
        if (property is AffineTransformProperty affine) affine.OutputWidth = 77;
        Mat? originalTemplate = GetTemplate(property);
        if (originalTemplate != null)
        {
            Require(ownedTemplate != null && !ReferenceEquals(ownedTemplate, originalTemplate), "Template was borrowed rather than cloned.");
            originalTemplate.SetTo(Scalar.All(0));
            originalTemplate.Dispose();
        }
        if (document.View is LineToolWpfView line)
        {
            Require(!line.ConsumeThresholdTeachingPreviewRequest(), "Teaching request survived UI capture.");
            property.THRESHOLD = 250;
            line.SetPurposeForTest("Intersection");
        }
        allowWorker.Set();
        WaitUntil(() => !completions.IsEmpty);
        Require(workerThread != 0 && workerThread != uiThread && captures == 1, "Worker affinity/capture re-entry contract failed.");
        Require(Review(document) == reviewBefore && display.FindIndex(outputLayer) < 0, "Worker published before guarded completion.");
        Require(input?.IsDisposed == true && output?.IsDisposed == true, "Native input/output Mat retained after computation.");
        if (completion == "cancel") Require(boundary.Cancel(), "Late cancel rejected.");
        else if (completion == "discard") Require(boundary.CancelAndDiscard(), "Discard rejected.");
        else if (completion == "dispose") boundary.Dispose();
        Require(completions.TryDequeue(out Action? apply), "Completion missing.");
        apply!();
        Require(!boundary.IsRunning && releases == 1 && (ownedTemplate == null || ownedTemplate.IsDisposed), "Captured resources/active slot not released.");
        Require(capturedProperties.Length > 0 && capturedProperties.All(item =>
            !ReferenceEquals(item.CvROIS, property.CvROIS) && !ReferenceEquals(item.CvMASKS, property.CvMASKS)
            && item.CvROIS.SequenceEqual(new[] { capturedRoi }) && item.CvMASKS.SequenceEqual(new[] { capturedMask })),
            tool + " captured ROI/mask lists did not remain independent after source-list mutation.");
        if (completion == "publish")
        {
            Require(completed?.Success == true && applications == 1 && publishes == 1, "Production operation failed: " + completed?.Status);
            using Bitmap published = display.GetLayerImageSnapshot(outputLayer);
            Require(published != null && published.Width == 512 && published.Height == 384 && display.SelectedItem == "Main", "Captured geometry or layer changed.");
            Require(teachingPreview ? Review(document) == reviewBefore : !string.IsNullOrWhiteSpace(Review(document)), "Review publication contract changed.");
            if (tool == "Matching" || tool == "EdgeBasedMatching")
            {
                VisionToolSingleInputPropertyToolShell shell = (VisionToolSingleInputPropertyToolShell)document.View.FindName("toolShell");
                string capturedScore = VisionToolVerificationText.T("VisionTool.Review.ScoreThresholdLabel", "Score >=")
                    + " " + 0.3.ToString("0.###", System.Globalization.CultureInfo.CurrentCulture);
                Require(shell.ResultGuidanceText.Text.Contains(capturedScore, StringComparison.Ordinal),
                    "Review lost captured criteria. Expected='" + capturedScore + "'; actual='" + shell.ResultGuidanceText.Text + "'.");
            }
            if (expectedLineCriteria != null && !teachingPreview)
            {
                VisionToolSingleInputPropertyToolShell shell = (VisionToolSingleInputPropertyToolShell)document.View.FindName("toolShell");
                string edgePrefix = VisionToolVerificationText.T("VisionTool.Review.LineEdgeSummaryFormat", "Edge / Count {0} / Edge {1} / Length {2:0.#}").Split('{')[0];
                Require(Review(document).StartsWith(edgePrefix, StringComparison.Ordinal)
                    && shell.ResultGuidanceText.Text.Contains(expectedLineCriteria, StringComparison.Ordinal),
                    "Line review used later purpose/criteria. Expected='" + expectedLineCriteria + "'; review='" + Review(document)
                    + "'; guidance='" + shell.ResultGuidanceText.Text + "'.");
            }
        }
        else
        {
            Require(publishes == 0 && display.FindIndex(outputLayer) < 0 && Review(document) == reviewBefore, "Canceled/stale/closed run published output or review.");
            Require(completion == "cancel" ? applications == 1 && completed?.Canceled == true : applications == 0, "Terminal callback semantics changed.");
        }
    }

    private static void RunFailureRecovery(string tool, string evidenceDirectory)
    {
        using DisplayManagerService display = CreateDisplay(tool, "Edge");
        using OpenVisionNativeToolDocument document = CreateDocument(tool, display);
        OpenCvPropertyBase property = Prepare(document, display, evidenceDirectory, "Edge");
        if (property is AffineTransformProperty affine) { affine.OutputWidth = int.MaxValue; affine.OutputHeight = int.MaxValue; }
        else if (document.View is MatchingToolWpfView matching) matching.SetTemplatePathForTest(Path.Combine(evidenceDirectory, "absent-template.png"));
        else { property.USE_THRESHOLD = true; property.THRESHOLD = 255; }
        document.RunPreview();
        WaitUntil(() => document.LastStatusText.Contains(" NG /", StringComparison.Ordinal), true);
        Require(!document.HasPreviewResult && !string.IsNullOrWhiteSpace(Review(document)), "Failure did not publish its existing review.");
        Prepare(document, display, evidenceDirectory, "Edge");
        document.RunPreview();
        WaitUntil(() => document.HasPreviewResult || document.LastStatusText.Contains(" NG /", StringComparison.Ordinal), true);
        Require(document.HasPreviewResult && !string.IsNullOrWhiteSpace(Review(document)), "Same-document recovery failed: " + document.LastStatusText);
    }

    private static OpenCvPropertyBase Prepare(OpenVisionNativeToolDocument document, DisplayManagerService display, string directory, string purpose)
    {
        document.View.Measure(new System.Windows.Size(900, 680));
        document.View.Arrange(new System.Windows.Rect(0, 0, 900, 680));
        document.View.UpdateLayout();
        document.View.Dispatcher.Invoke(() => { }, DispatcherPriority.DataBind);
        VisionToolSingleInputPropertyToolShell shell = (VisionToolSingleInputPropertyToolShell)document.View.FindName("toolShell");
        WpgPropertyGrid grid = (WpgPropertyGrid)shell.PropertyGridHost.Child;
        if (document.View is LineToolWpfView line)
        {
            foreach (string side in new[] { "Line A", "Line B" })
            {
                line.SetLineSettingForTest(side);
                LineGaugeProperty item = (LineGaugeProperty)grid.SelectedObject;
                ResetRegions(item);
                item.USE_ROI = true;
                item.CvROI = purpose == "Intersection"
                    ? (side == "Line A" ? new OpenCvSharp.Rect(42, 268, 190, 74) : new OpenCvSharp.Rect(306, 44, 86, 142))
                    : (side == "Line A" ? new OpenCvSharp.Rect(92, 64, 120, 272) : new OpenCvSharp.Rect(300, 64, 122, 272));
                item.CONTRAST = 20; item.THICKNESS = 2; item.SAMPLING_STEP = 4; item.POINT_RANGE = 4;
                item.USE_AVERAGE_FILTER = false;
                if (purpose == "Intersection")
                {
                    item.USE_THRESHOLD = true; item.THRESHOLD = 170;
                    line.ConfigureSelectedLineForTest(side == "Line A" ? "Y_TTOB" : "X_LTOR", "ALL", side == "Line A" ? "Y_BTOT" : "X_LTOR");
                }
                else line.ConfigureSelectedLineForTest(side == "Line A" ? "X_LTOR" : "X_RTOL", "ALL", "X_RTOL");
            }
            line.SetLineSettingForTest("Line A");
            line.SetPurposeForTest(purpose);
            line.ConsumeThresholdTeachingPreviewRequest();
            return (OpenCvPropertyBase)grid.SelectedObject;
        }
        OpenCvPropertyBase property = (OpenCvPropertyBase)grid.SelectedObject;
        ResetRegions(property);
        if (property is AffineTransformProperty affine) { affine.OutputWidth = 0; affine.OutputHeight = 0; }
        if (property is MatchingProperty matchingProperty) matchingProperty.AUTO_PREVIEW = false;
        if (GetTemplate(property) != null)
        {
            using Bitmap input = display.GetLayerImageSnapshot("Main");
            System.Drawing.Rectangle crop = property is FeatureMatchingProperty
                ? new System.Drawing.Rectangle(132, 82, 190, 142) : new System.Drawing.Rectangle(150, 100, 120, 96);
            using Bitmap template = input.Clone(crop, input.PixelFormat);
            string templatePath = Path.Combine(directory, document.ToolName + "-template-" + Guid.NewGuid().ToString("N") + ".png");
            template.Save(templatePath, System.Drawing.Imaging.ImageFormat.Png);
            if (document.View is MatchingToolWpfView matching) matching.SetTemplatePathForTest(templatePath);
            else if (document.View is EdgeBasedMatchingToolWpfView edge) edge.SetTemplatePathForTest(templatePath);
            else ((FeatureMatchingToolWpfView)document.View).SetTemplatePathForTest(templatePath);
        }
        // Template test hooks intentionally restore teaching defaults (Matching 0.55 / Edge 0.70).
        // Set the mutation-test criterion after registration so both worker and UI capture start at 0.3.
        if (property is MatchingProperty configuredMatching) configuredMatching.SCORE_MIN = 0.3;
        if (property is EdgeBasedMatchingProperty configuredEdge) configuredEdge.SCORE_MIN = 0.3;
        grid.RefreshSelectedObject();
        return property;
    }

    private static void ResetRegions(OpenCvPropertyBase property)
    {
        property.USE_THRESHOLD = false; property.USE_ADAPTIVE_THRESHOLD = false; property.USE_BITWISENOT = false;
        property.USE_ROI = false; property.USE_MULTI_ROI = false; property.USE_MASKING = false;
        property.CvROIS = new List<OpenCvSharp.Rect>(); property.CvMASKS = new List<OpenCvSharp.Rect>();
    }

    private static IEnumerable<OpenCvPropertyBase> GetCapturedProperties(OpenVisionNativePreviewOperation operation)
    {
        object? closure = operation.Execute.Target;
        return closure?.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(field => field.GetValue(closure)).OfType<OpenCvPropertyBase>() ?? Enumerable.Empty<OpenCvPropertyBase>();
    }

    private static Mat? GetTemplate(OpenCvPropertyBase property) => property switch
    {
        MatchingProperty matching => matching.ImageTemplate,
        EdgeBasedMatchingProperty edge => edge.ImageTemplate,
        FeatureMatchingProperty feature => feature.ImageTemplate,
        _ => null
    };

    private static string Review(OpenVisionNativeToolDocument document) => document.View switch
    {
        MatchingToolWpfView matching => matching.ResultReviewTextForTest,
        EdgeBasedMatchingToolWpfView edge => edge.ResultReviewTextForTest,
        FeatureMatchingToolWpfView feature => feature.ResultReviewTextForTest,
        LineToolWpfView line => line.ResultReviewTextForTest,
        AffineTransformToolWpfView affine => affine.ResultReviewTextForTest,
        _ => throw new InvalidOperationException("Unexpected View.")
    };

    private static OpenVisionNativeToolDocument CreateDocument(string tool, IDisplayManager display)
    {
        return tool switch
        {
            "Matching" => OpenVisionNativePropertyGridToolFactory.CreateMatching(display),
            "EdgeBasedMatching" => OpenVisionNativePropertyGridToolFactory.CreateEdgeBasedMatching(display),
            "FeatureMatching" => OpenVisionNativePropertyGridToolFactory.CreateFeatureMatching(display),
            "Line" => OpenVisionNativeCustomToolFactory.CreateLine(display),
            "AffineTransform" => OpenVisionNativePropertyGridToolFactory.CreateAffineTransform(display),
            _ => throw new ArgumentOutOfRangeException(nameof(tool))
        };
    }

    private static DisplayManagerService CreateDisplay(string tool, string purpose)
    {
        DisplayManagerService display = new();
        Bitmap input = tool == "FeatureMatching" ? SmokeFixtureResources.CreateFeatureMatchingSmokeBitmap()
            : tool == "Line" ? (purpose == "Intersection" ? SmokeFixtureResources.CreateLineIntersectionSmokeBitmap() : SmokeFixtureResources.CreateLineMeasureSmokeBitmap())
            : SmokeFixtureResources.CreateMatchingSmokeBitmap();
        display.CreateLayerDisplay(ImageSpaceFrame.TakeOwnership(input), "Main", false);
        display.SelectedItem = "Main";
        display.ActivateLayer("Main");
        return display;
    }

    private static void WaitUntil(Func<bool> predicate, bool pumpDispatcher = false)
    {
        Stopwatch timer = Stopwatch.StartNew();
        while (!predicate() && timer.Elapsed < TimeSpan.FromSeconds(20))
        {
            if (pumpDispatcher)
            {
                DispatcherFrame frame = new();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false));
                Dispatcher.PushFrame(frame);
            }
            Thread.Sleep(1);
        }
        Require(predicate(), "Timed out waiting for native Preview.");
    }

    private static void Check(string name, Action action, ICollection<string> results)
    {
        PropertyGridEditorRuntime runtime = (PropertyGridEditorRuntime)typeof(PropertyGridEditorFactory)
            .GetField("runtime", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        Func<string> previousRecipeContext = runtime.RecipeNameAccessor;
        string recipeName = "Smoke_CapturedPreview_" + Guid.NewGuid().ToString("N").Substring(0, 12);
        string recipePath = Path.GetFullPath(RecipeWorkspaceService.GetRecipeDirectoryPath(recipeName));
        bool ownsRecipe = false;
        try
        {
            Require(string.Equals(recipePath, Path.GetFullPath(Path.Combine(AppPathService.DataRootDirectory, "RECIPE", recipeName)), StringComparison.OrdinalIgnoreCase)
                && !Directory.Exists(recipePath) && !File.Exists(recipePath),
                "Refusing to use an existing or out-of-root recipe workspace: " + recipePath);
            ownsRecipe = true;
            PropertyGridEditorFactory.SetRecipeNameContext(() => recipeName);
            action();
            results.Add("PASS: " + name);
        }
        catch (Exception exception) { results.Add("FAIL: " + name + ": " + exception); }
        finally
        {
            PropertyGridEditorFactory.SetRecipeNameContext(previousRecipeContext);
            if (ownsRecipe)
            {
                try
                {
                    Require(string.Equals(Path.GetFullPath(RecipeWorkspaceService.GetRecipeDirectoryPath(recipeName)), recipePath, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(Path.GetPathRoot(recipePath), @"D:\", StringComparison.OrdinalIgnoreCase), "Recipe cleanup target changed.");
                    if (Directory.Exists(recipePath))
                    {
                        Require((File.GetAttributes(recipePath) & FileAttributes.ReparsePoint) == 0, "Recipe cleanup must not follow a redirected workspace.");
                        Require(RecipeWorkspaceService.DeleteVisionWorkspace(recipeName), "Exact-owned recipe cleanup failed: " + recipePath);
                    }
                    Require(!Directory.Exists(recipePath), "Exact-owned recipe workspace remains: " + recipePath);
                }
                catch (Exception exception) { results.Add("FAIL: " + name + " cleanup: " + exception); }
            }
        }
    }

    private static void Require([DoesNotReturnIf(false)] bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
