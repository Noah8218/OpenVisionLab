using OpenVisionLab;
using OpenVisionLab.Core;
using OpenVisionLab.Logging.Controls.Model;
using OpenVisionLab.Logging.Controls.View;
using OpenVisionLab.Logging.Controls.ViewModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using static OpenVisionLab.DEFINE;

internal static class UiPerformanceContract
{
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private static readonly List<object> cases = new();
    private static string evidence = "";

    internal static int Run(string outputDirectory, string inputDirectory, string phase = "all")
    {
        evidence = Path.GetFullPath(outputDirectory);
        if (!evidence.StartsWith(@"D:\OpenVisionLab-TestData\OpenVisionLab_Dev\", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Owned D: evidence required.");
        Directory.CreateDirectory(evidence);
        if (!new[] { "all", "images", "editor", "routes", "results", "refresh", "logs" }.Contains(phase)) throw new ArgumentException("Unknown UI performance phase: " + phase);
        try
        {
            Environment.SetEnvironmentVariable("OPENVISIONLAB_DATA_ROOT", Path.Combine(evidence, "data"));
            Environment.SetEnvironmentVariable("OPENVISIONLAB_DATA_ROOT_RESOLVED", null);
            AppPathService.Initialize();
            OpenVisionLanguageService.ConfigureDataDirectory(AppPathService.ConfigRootDirectory);
            OpenVisionLanguageService.Load();
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            var fixture = RecipeContextFixture.CreatePipeline("PerfPipeline", 2);
            foreach (var step in fixture.Steps) { step.ToolType = "Mean"; }
            VisionPipelineStorage.Save("PerfFixture", fixture);
            VisionPipelineStorage.SaveActivePipelineName("PerfFixture", "PerfPipeline");
            ApplicationRuntimeContext context = ApplicationRuntimeContext.CreateDefault();
            context.Global.Recipe.Name = "PerfFixture";
            OpenVisionShellHostWindow window = new(context);
            OpenVisionShellHostView host = window.ShellHostForSmoke;
            Action? verifyReleased = null;
            try
            {
                host.Workbench.IsAutoRunEnabled = false;
                Require(host.LoadMainImageFromFileForTest(Path.Combine(inputDirectory, "03-large4096.png")), "4096 input load");
                Pump();
                if (phase is "all" or "images") Images(host);
                if (phase is "all" or "editor") Editor(host, inputDirectory);
                if (phase is "all" or "routes") Routes(host);
                if (phase is "all" or "results") Results(host, inputDirectory);
                if (phase is "all" or "refresh") verifyReleased = RefreshPixels(host);
                if (phase is "all" or "logs") Logs(host);
            }
            finally { host.Dispose(); verifyReleased?.Invoke(); }
            Save("Pass", null);
            return 0;
        }
        catch (Exception exception) { Save("Fail", exception.ToString()); throw; }
    }

    private static void Images(OpenVisionShellHostView host)
    {
        var images = host.Workbench.Images;
        int runs = host.NativePreviewRunCount, layers = host.LayerDocumentCount;
        var current = images.CurrentImage;
        foreach (int count in new[] { 50, 200, 500 })
        {
            images.AddImages(Enumerable.Range(images.Count, count - images.Count).Select(index => new OpenVisionWorkbenchImageItem(Path.Combine(evidence, "row-" + index + ".png"), "row-" + index, current.Preview)));
            int itemChanges = 0, aggregate = 0;
            PropertyChangedEventHandler itemChanged = (_, e) => { if (e.PropertyName == nameof(OpenVisionWorkbenchImageItem.IsSelected)) itemChanges++; };
            PropertyChangedEventHandler aggregateChanged = (_, e) => { if (e.PropertyName == nameof(images.Count)) aggregate++; };
            foreach (var item in images.Images) item.PropertyChanged += itemChanged;
            images.PropertyChanged += aggregateChanged;
            try
            {
                Measure("C30-select-all-" + count, () => images.SelectAllCommand.Execute(null), () => new { Aggregate = aggregate, ItemChanges = itemChanges, images.SelectedCount });
                Require(aggregate == 1 && itemChanges == count && images.SelectedCount == count && images.ClearSelectionCommand.CanExecute(null), "bulk selection publishes once and keeps item notifications");
                aggregate = itemChanges = 0;
                images.SelectAllCommand.Execute(null);
                Require(aggregate == 1 && itemChanges == 0 && images.SelectedCount == count, "already selected no item changes");
                aggregate = itemChanges = 0;
                Measure("C32-clear-selection-" + count, () => images.ClearSelectionCommand.Execute(null), () => new { Aggregate = aggregate, ItemChanges = itemChanges, images.SelectedCount });
                Require(aggregate == 1 && itemChanges == count && images.SelectedCount == 0 && !images.ClearSelectionCommand.CanExecute(null), "bulk deselection");
                CheckBox selected = new();
                selected.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, new Binding(nameof(OpenVisionWorkbenchImageItem.IsSelected)) { Source = images.Images[0], Mode = BindingMode.TwoWay });
                aggregate = 0; selected.IsChecked = true;
                Require(images.SelectedCount == 1 && aggregate == 1, "single binding to item");
                images.Images[0].IsSelected = false;
                Require(selected.IsChecked == false && images.SelectedCount == 0, "item binding to control");
                BindingOperations.ClearAllBindings(selected);
                images.SelectImageCommand.Execute(Tuple.Create<object, bool, bool>(images.Images[^1], false, true));
                Require(images.SelectedCount == count, "shift range selection");
                aggregate = 0; images.LabelCommand.Execute("good");
                Require(aggregate == 1 && images.Images.All(x => x.Label == "good"), "bulk labels publish once");
                images.FilterCommand.Execute("good"); Require(images.VisibleImages.Count() == count, "good filter");
                images.FilterCommand.Execute("bad"); Require(!images.VisibleImages.Any(), "empty bad filter");
                images.FilterCommand.Execute("all"); images.ClearSelectionCommand.Execute(null);
            }
            finally
            {
                images.PropertyChanged -= aggregateChanged;
                foreach (var item in images.Images) item.PropertyChanged -= itemChanged;
            }
        }
        Require(ReferenceEquals(current, images.CurrentImage) && runs == host.NativePreviewRunCount && layers == host.LayerDocumentCount, "selection keeps input, run and layers");
    }

    private static void Editor(OpenVisionShellHostView host, string inputs)
    {
        var workbench = host.Workbench; var pipeline = workbench.Pipeline; var viewer = workbench.Viewer;
        pipeline.SelectStepCommand.Execute(0); Pump();
        var document = host.ActiveNativeDocumentForTest!;
        Require(document != null && workbench.Tool.IsStepEditing, "selected step opens editor");
        int sources = 0, rows = 0, tool = 0;
        EventHandler sourceChanged = (_, _) => sources++;
        PropertyChangedEventHandler rowChanged = (_, e) => { if (e.PropertyName == nameof(pipeline.Rows)) rows++; };
        PropertyChangedEventHandler toolChanged = (_, e) => { if (string.IsNullOrEmpty(e.PropertyName)) tool++; };
        viewer.SourcesChanged += sourceChanged; pipeline.PropertyChanged += rowChanged; workbench.Tool.PropertyChanged += toolChanged;
        int runs = host.NativePreviewRunCount, layers = host.LayerDocumentCount;
        try
        {
            long Session() => (long)typeof(OpenVisionWorkbenchToolViewModel).GetField("generation", Private)!.GetValue(workbench.Tool)!;
            long session = Session();
            byte[] definition = VisionPipelineExecutionPlan.SerializePipeline(pipeline.WorkingCopyForTest);
            bool dirty = pipeline.IsDirty;
            string context = pipeline.ContextKey, title = workbench.Tool.Title;
            workbench.IsRightCollapsed = true;
            Measure("C51-autoOFF-step-active-reselect", () => pipeline.SelectStepCommand.Execute(0), () => new { SourcesChanged = sources, PipelineRows = rows, ToolWildcard = tool, Session = Session(), Auto = workbench.IsAutoRunEnabled });
            Require(Session() == session && ReferenceEquals(document, host.ActiveNativeDocumentForTest) && sources < 23, "same editor preserves active token and reduces sources");
            Require(definition.SequenceEqual(VisionPipelineExecutionPlan.SerializePipeline(pipeline.WorkingCopyForTest)) && pipeline.IsDirty == dirty && pipeline.ContextKey == context && workbench.Tool.Title == title && !workbench.IsRightCollapsed, "reused editor preserves definition, dirty identity/title and reopens the panel");
            workbench.Tool.UseAcceptance = true;
            workbench.Tool.AcceptanceTimeLimitText = "incomplete-number";
            Require(workbench.Tool.HasAcceptanceError, "pending invalid editor value admitted by fixture");
            long pendingSession = Session(); pipeline.SelectStepCommand.Execute(0); Pump();
            Require(Session() == pendingSession && workbench.Tool.AcceptanceTimeLimitText == "incomplete-number" && workbench.Tool.HasAcceptanceError, "same active selection retains pending invalid editor value");
            workbench.Tool.AcceptanceTimeLimitText = "0"; workbench.Tool.UseAcceptance = false;
            session = Session();
            Require(host.CloseActiveWpfToolWindowForTest(), "close active editor");
            pipeline.SelectStepCommand.Execute(0); Pump();
            Require(workbench.Tool.IsStepEditing && Session() != session, "closed editor reopens with new session");
            pipeline.SelectStepCommand.Execute(1); Pump();
            Require(pipeline.SelectedStepIndex == 1 && workbench.Tool.CurrentStep.Name == pipeline.GetStep(1).Name, "other step identity");
            VisionPipelineStorage.Save("PerfOther", RecipeContextFixture.CreatePipeline("OtherPipeline", 2));
            VisionPipelineStorage.SaveActivePipelineName("PerfOther", "OtherPipeline");
            session = Session(); host.SwitchRecipeContextForTest("PerfOther"); pipeline.SelectStepCommand.Execute(0); Pump();
            Require(Session() != session && host.ActiveNativeRecipeContextNameForTest == "PerfOther" && workbench.Tool.CurrentStep.Name.StartsWith("OtherPipeline", StringComparison.Ordinal), "same index in another context rebinds");
            host.SwitchRecipeContextForTest("PerfFixture"); pipeline.SelectStepCommand.Execute(0); Pump();
            Require(host.ActiveNativeRecipeContextNameForTest == "PerfFixture", "original context recovers");
            Require(host.LoadMainImageFromFileForTest(Path.Combine(inputs, "01-small.png")), "small input switch"); Pump();
            foreach (VISION_MENU menu in new[] { VISION_MENU.Threshold, VISION_MENU.Filter, VISION_MENU.Morphology, VISION_MENU.Arithmetic, VISION_MENU.EdgeDetection, VISION_MENU.HSV, VISION_MENU.Histogram, VISION_MENU.Matching, VISION_MENU.EdgeBasedMatching, VISION_MENU.FeatureMatching, VISION_MENU.RotateAndScale, VISION_MENU.AffineTransform, VISION_MENU.Blob, VISION_MENU.Contour, VISION_MENU.Line, VISION_MENU.Mean })
            {
                OpenTool(host, menu); Pump();
                var cached = host.ActiveNativeDocumentForTest; var step = workbench.Tool.CurrentStep;
                Require(cached?.ToolName == (menu == VISION_MENU.RotateAndScale ? "RotateScale" : menu.ToString()) && workbench.Tool.IsNewTest, "controller fixture opens expected independent tool: " + menu);
                string input = step.InputLayer, output = step.OutputLayer;
                sources = rows = tool = 0;
                Measure("C60-" + menu + "-cached", () => OpenTool(host, menu), () => new { SourcesChanged = sources, PipelineRows = rows, ToolWildcard = tool });
                Require(ReferenceEquals(cached, host.ActiveNativeDocumentForTest) && workbench.Tool.CurrentStep.InputLayer == input && workbench.Tool.CurrentStep.OutputLayer == output && sources < 23, "cached tool keeps route and reduces publications: " + menu);
            }
            Require(runs == host.NativePreviewRunCount && layers == host.LayerDocumentCount, "editor navigation does not execute or add layers");
            sources = 0; pipeline.SetImageEditPending(true);
            Require(sources > 0, "pipeline state refresh remains synchronous"); pipeline.SetImageEditPending(false);
            int before = sources; workbench.Tool.NotifyExecutionState(Session()); Require(sources == before + 1, "standalone tool state refresh remains synchronous");
        }
        finally { viewer.SourcesChanged -= sourceChanged; pipeline.PropertyChanged -= rowChanged; workbench.Tool.PropertyChanged -= toolChanged; }
    }

    private static void Routes(OpenVisionShellHostView host)
    {
        using System.Drawing.Bitmap image = new(16, 16);
        Require(host.AddLayerImageForTest("Perf_Input_A", image) && host.AddLayerImageForTest("Perf_Input_B", image), "owned route input layers");
        int runs = host.NativePreviewRunCount, layers = host.LayerDocumentCount;
        foreach (VISION_MENU menu in new[] { VISION_MENU.Filter, VISION_MENU.Arithmetic })
        {
            OpenTool(host, menu); Pump();
            var document = host.ActiveNativeDocumentForTest!; var model = host.Workbench.Tool;
            if (document.View is ArithmeticToolWpfView arithmetic)
            {
                var settings = arithmetic.CaptureSettings(); settings.UseOffsetMode = false; settings.UseConstantInput = false;
                arithmetic.ApplyPersistedSettings(settings); document.RecordParameterChange(null); Pump();
            }
            var shell = (FrameworkElement)document.View.FindName("toolShell");
            var input = (ComboBox)shell.FindName(menu == VISION_MENU.Arithmetic ? "cbInputA" : "cbInputLayer");
            input.SelectedItem = input.Items.Cast<KeyValuePair<string, string>>().Single(x => x.Key == "Perf_Input_A"); Pump();
            Require(model.InputLayer == "Perf_Input_A" && document.RouteInputLayerName == "Perf_Input_A", "explicit input selection writes raw key: " + menu);
            if (menu == VISION_MENU.Arithmetic)
            {
                var inputB = (ComboBox)shell.FindName("cbInputB");
                inputB.SelectedItem = inputB.Items.Cast<KeyValuePair<string, string>>().Single(x => x.Key == "Perf_Input_B"); Pump();
                Require(model.InputLayer == "Perf_Input_A" && model.InputLayerB == "Perf_Input_B", "explicit B selection preserves A");
            }
            long token = (long)typeof(OpenVisionWorkbenchToolViewModel).GetField("generation", Private)!.GetValue(model)!;
            Require(model.SetRoute(token, "Perf_Unavailable", model.InputLayerB, model.OutputLayer), "raw unavailable input retained");
            document.RefreshLayerState(); Pump();
            Require(model.InputLayer == "Perf_Unavailable" && VisionToolLayerComboHelper.GetLayerText(input) == "Perf_Unavailable", "restoration preserves unavailable input without fallback write");
            input.SelectedItem = input.Items.Cast<KeyValuePair<string, string>>().Single(x => x.Key == "Main"); Pump();
            Require(model.InputLayer == "Main", "valid selection recovers from unavailable input");
            string output = model.OutputLayer;
            Require(!model.SetRoute(token, model.InputLayer, model.InputLayerB, "Main") && model.OutputLayer == output, "invalid output fails closed");
            document.RefreshLayerState(); Pump();
            Require(model.SetRoute(token, model.InputLayer, model.InputLayerB, "Perf_Owned_Output"), "explicit pending output accepted");
            document.RefreshLayerState(); Pump();
            Require(model.OutputLayer == "Perf_Owned_Output", "refresh preserves explicit pending output");
        }
        Require(host.NativePreviewRunCount == runs && host.LayerDocumentCount == layers, "route changes and refresh do not execute or create output layers");
        cases.Add(new { CaseId = "V00-route-refresh", Outcome = "Pass", SingleInput = "Selected", ArithmeticAB = "Independent", UnavailableInput = "Preserved", InvalidOutput = "Rejected", Recovery = "Main" });
    }

    private static void Results(OpenVisionShellHostView host, string inputs)
    {
        var pipeline = host.Workbench.Pipeline; var viewer = host.Workbench.Viewer;
        Require(host.LoadMainImageFromFileForTest(Path.Combine(inputs, "01-small.png")), "result fixture small input");
        pipeline.SelectStepCommand.Execute(0); Pump();
        Require(pipeline.RunCommand.CanExecute(null), "valid pipeline runnable");
        pipeline.RunCommand.Execute(null);
        Stopwatch timeout = Stopwatch.StartNew();
        while (pipeline.IsRunning && timeout.Elapsed < TimeSpan.FromSeconds(30)) { Pump(); Thread.Sleep(1); }
        Require(!pipeline.IsRunning && pipeline.GetResult(0)?.Executed == true, "explicit small-image run produces real result");
        var result = pipeline.GetResult(0);
        var changed = pipeline.GetStep(0); changed.Name += "_Edited"; pipeline.ReplaceStep(0, changed); Pump();
        Require(pipeline.IsResultPrevious(0) && ReferenceEquals(result, pipeline.GetResult(0)), "edit retains Previous result");
        pipeline.SelectStepCommand.Execute(0); Pump();
        Require(pipeline.IsResultPrevious(0) && host.Workbench.Tool.IsResultPrevious, "same-step reselect keeps Previous");
        int sources = 0; EventHandler refreshed = (_, _) => sources++; viewer.SourcesChanged += refreshed;
        try
        {
            Require(host.LoadMainImageFromFileForTest(Path.Combine(inputs, "03-large4096.png")), "changed input loads"); Pump();
            Require(sources > 0 && pipeline.GetResult(0) == null, "switching to another image publishes its own empty result without leaking the prior image result");
            var original = viewer.Panes[0]; original.Source = "layer:Main";
            int width = 0; viewer.ReadImage(original, image => width = image?.Width ?? 0);
            Require(width == 4096, "viewer reads replaced Main image through current lease");
            original.Source = "follow:in";
            var valid = pipeline.GetStep(0); var invalid = pipeline.GetStep(0); invalid.InputLayer = "missing_owned_perf_input"; pipeline.ReplaceStep(0, invalid); Pump();
            Require(!pipeline.RunCommand.CanExecute(null), "missing route fails closed without execution");
            pipeline.ReplaceStep(0, valid); Pump(); Require(pipeline.RunCommand.CanExecute(null), "valid route recovers");
            int before = sources; host.Workbench.Tool.NotifyExecutionState((long)typeof(OpenVisionWorkbenchToolViewModel).GetField("generation", Private)!.GetValue(host.Workbench.Tool)!);
            Require(sources == before + 1, "error/progress tool state publication remains synchronous");
            cases.Add(new { CaseId = "V01-result-previous-route-lease", Outcome = "Pass", InputWidth = width, SourcesChanged = sources, RealRun = "Executed", Previous = true, MissingRoute = "Blocked", Recovery = "Runnable" });
        }
        finally { viewer.SourcesChanged -= refreshed; }
    }

    private static Action RefreshPixels(OpenVisionShellHostView host)
    {
        var pipeline = host.Workbench.Pipeline; var tool = host.Workbench.Tool; var viewer = host.Workbench.Viewer;
        var step = pipeline.GetStep(0);
        step.ToolType = "Filter"; step.UseAcceptance = false; step.Parameters.Clear();
        step.Parameters["FilterType"] = "Blur"; step.Parameters["KernelWidth"] = "3"; step.Parameters["KernelHeight"] = "3";
        step.Parameters["BorderType"] = "Reflect101";
        Require(pipeline.ReplaceStep(0, step), "refresh fixture Filter definition"); pipeline.SelectStepCommand.Execute(0); Pump();
        var document = host.ActiveNativeDocumentForTest!;
        var shell = (VisionToolSingleInputPropertyToolShell)document.View.FindName("toolShell");
        var slot = shell.InputPreview;
        long generation = (long)typeof(OpenVisionWorkbenchToolViewModel).GetField("generation", Private)!.GetValue(tool)!;
        foreach (string guard in new[] { "isWorkbenchDocument", "captureParameters" })
        {
            var field = document.GetType().GetField(guard, Private)!; object? saved = field.GetValue(document);
            try
            {
                field.SetValue(document, guard == "isWorkbenchDocument" ? false : null);
                Require(!document.ActivateWorkbenchEditor(tool, null!), guard + " early return does not claim a completed refresh");
                Require((long)typeof(OpenVisionWorkbenchToolViewModel).GetField("generation", Private)!.GetValue(tool)! == generation, guard + " leaves active session intact");
            }
            finally { field.SetValue(document, saved); }
        }
        cases.Add(new { CaseId = "V05-activation-early-return", Outcome = "Pass" });
        string pathA = Path.Combine(evidence, "refresh-A.png"), pathB = Path.Combine(evidence, "refresh-B.png");
        string expectedA = WriteGray(pathA, 17, 9, 73), expectedB = WriteGray(pathB, 17, 9, 191);
        Require(host.LoadMainImageFromFileForTest(pathA), "supported source A load"); Pump();
        AssertPixels("load A", expectedA, 17, 9); Reselect("loaded A", expectedA); CloseReopen("loaded A", expectedA, 17, 9);
        Require(host.LoadMainImageFromFileForTest(pathB), "same-size source B load"); Pump();
        AssertPixels("same-size replacement B", expectedB, 17, 9); Reselect("loaded B", expectedB); CloseReopen("loaded B", expectedB, 17, 9);
        using (var branch = Gray(19, 11, 131)) Require(host.AddLayerImageForTest("Perf_Refresh_Branch", branch), "owned branch fixture");
        string branchPixels; using (var branch = Gray(19, 11, 131)) branchPixels = BitmapPixels(branch);
        var input = (ComboBox)shell.FindName("cbInputLayer");
        input.SelectedItem = input.Items.Cast<KeyValuePair<string, string>>().Single(item => item.Key == "Perf_Refresh_Branch"); Pump();
        Require(tool.InputLayer == "Perf_Refresh_Branch", "rendered route input reaches owner");
        AssertPixels("branch route", branchPixels, 19, 11); Reselect("branch route", branchPixels, allowRouteRebind: true); CloseReopen("branch", branchPixels, 19, 11);
        var missing = pipeline.GetStep(0); missing.InputLayer = "Perf_Refresh_Missing";
        Require(pipeline.ReplaceStep(0, missing), "supported raw missing route fixture"); Pump();
        Require(!slot.HasImage && !pipeline.RunCommand.CanExecute(null), "missing input clears preview and blocks run");
        Reselect("missing route", ""); CloseReopen("missing", "", 0, 0);
        input.SelectedItem = input.Items.Cast<KeyValuePair<string, string>>().Single(item => item.Key == "Main"); Pump();
        AssertPixels("route recovery", expectedB, 17, 9); Reselect("route recovered", expectedB, allowRouteRebind: true); CloseReopen("route recovered", expectedB, 17, 9);
        Require(pipeline.RunToSelectedCommand.CanExecute(null), "valid Filter explicit run allowed");
        pipeline.RunToSelectedCommand.Execute(null); Stopwatch timeout = Stopwatch.StartNew();
        while (pipeline.IsRunning && timeout.Elapsed < TimeSpan.FromSeconds(30)) { Pump(); Thread.Sleep(1); }
        Require(!pipeline.IsRunning && pipeline.GetResult(0)?.Success == true, "explicit Filter run completes: " + pipeline.StatusText + " / " + pipeline.RunDisabledReason + " / " + pipeline.GetResult(0)?.ErrorName + " / " + pipeline.GetResult(0)?.Message);
        var result = pipeline.GetResult(0); var output = viewer.Panes.Last(); output.Source = "follow:out";
        string outputPixels = ""; viewer.ReadImage(output, bitmap => outputPixels = bitmap == null ? "" : BitmapPixels(bitmap));
        Require(outputPixels == expectedB, "uniform gray Blur output preserves every expected pixel");
        Reselect("fresh result", expectedB); CloseReopen("fresh result", expectedB, 17, 9);
        Require(ReferenceEquals(result, pipeline.GetResult(0)) && !pipeline.IsResultPrevious(0), "reselection retains fresh result identity");
        object workspace = typeof(OpenVisionShellHostView).GetField("workspaceImageController", Private)!.GetValue(host)!;
        workspace.GetType().GetMethod("ClearWorkspaceInput")!.Invoke(workspace, null); Pump();
        Require(!slot.HasImage && slot.ImagePixelWidth == 0, "supported clear releases preview content"); Reselect("cleared input", ""); CloseReopen("cleared", "", 0, 0);
        Require(host.LoadMainImageFromFileForTest(pathA), "source reload after clear"); Pump(); AssertPixels("reload A", expectedA, 17, 9); CloseReopen("reload A", expectedA, 17, 9);
        Require(host.CloseActiveWpfToolWindowForTest(), "close before source replacement");
        Require(host.LoadMainImageFromFileForTest(pathB), "source replacement while editor closed"); Pump();
        pipeline.SelectStepCommand.Execute(0); Pump();
        Require(ReferenceEquals(document, host.ActiveNativeDocumentForTest), "reopen retains cached native document");
        AssertPixels("reopen B", expectedB, 17, 9); Reselect("reopened B", expectedB); CloseReopen("closed source replacement", expectedB, 17, 9);
        cases.Add(new { CaseId = "V02-refresh-pixels", Outcome = "Pass", SourceA = expectedA, SourceB = expectedB, Branch = branchPixels, ExplicitFilterOutput = outputPixels, PixelCoverage = "All pixels/size/route; same-size replacement, missing route recovery, fresh result, clear/reload, closed editor replacement/reopen" });
        return () =>
        {
            Require(typeof(VisionToolInlinePreviewSlot).GetField("pendingImage", Private)!.GetValue(slot) == null
                && ((Image)typeof(VisionToolInlinePreviewSlot).GetField("previewImage", Private)!.GetValue(slot)!).Source == null,
                "owning host Dispose releases pending bitmap and WPF source");
            cases.Add(new { CaseId = "V03-preview-dispose", Outcome = "Pass" });
        };

        void AssertPixels(string label, string expected, int width, int height)
        {
            Require(slot.ImagePixelWidth == width && slot.ImagePixelHeight == height && SlotPixels(slot) == expected, label + " inline dimensions/pixels");
            var pane = viewer.Panes[0]; pane.Source = "follow:in"; string actual = "";
            viewer.ReadImage(pane, bitmap => actual = bitmap == null ? "" : BitmapPixels(bitmap));
            Require(actual == expected, label + " main viewer current lease pixels");
        }
        void CloseReopen(string label, string pixels, int width, int height)
        {
            int runs = host.NativePreviewRunCount, layers = host.LayerDocumentCount;
            long revision = pipeline.InputRevision;
            var result = pipeline.GetResult(0); bool previous = pipeline.IsResultPrevious(0), dirty = pipeline.IsDirty;
            byte[] definition = VisionPipelineExecutionPlan.SerializePipeline(pipeline.WorkingCopyForTest);
            string sourcePath = RecipeWorkspaceService.GetVisionPipelinePath(host.RecipeCommands.SelectedRecipeName, pipeline.SelectedPipelineName);
            byte[] source = File.ReadAllBytes(sourcePath);
            Require(host.CloseActiveWpfToolWindowForTest(), label + " closes cached editor");
            int changed = 0, sources = 0;
            EventHandler imageChanged = (_, _) => changed++; EventHandler sourceChanged = (_, _) => sources++;
            slot.ImageChanged += imageChanged; viewer.SourcesChanged += sourceChanged;
            try
            {
                host.Workbench.IsRightCollapsed = true;
                pipeline.SelectStepCommand.Execute(0); Pump();
                Require(ReferenceEquals(document, host.ActiveNativeDocumentForTest) && tool.IsStepEditing && !host.Workbench.IsRightCollapsed, label + " reopens cached document and panel");
                AssertPixels(label + " closed reopen", pixels, width, height);
                Require(pipeline.SelectedStepIndex == 0 && pipeline.InputRevision == revision && host.NativePreviewRunCount == runs
                    && host.LayerDocumentCount == layers && pipeline.IsDirty == dirty && pipeline.IsResultPrevious(0) == previous
                    && ReferenceEquals(result, pipeline.GetResult(0)) && source.SequenceEqual(File.ReadAllBytes(sourcePath))
                    && definition.SequenceEqual(VisionPipelineExecutionPlan.SerializePipeline(pipeline.WorkingCopyForTest)), label + " reactivation preserves definition/input/result/dirty/run/layers");
                // Activation publishes the new editor and one final host source state. Inline input is copied at most once.
                Require(sources <= 2 && changed <= 1, label + " closed activation has no duplicate layer refresh: sources=" + sources + ", input=" + changed);
                cases.Add(new { CaseId = "V04-closed-" + label, Outcome = "Pass", SourcesChanged = sources, InputChanges = changed, NativeRuns = runs });
            }
            finally { slot.ImageChanged -= imageChanged; viewer.SourcesChanged -= sourceChanged; }
        }
        void Reselect(string label, string pixels, bool allowRouteRebind = false)
        {
            int changed = 0, publications = 0, runs = host.NativePreviewRunCount;
            var before = pipeline.GetResult(0); long revision = pipeline.InputRevision;
            EventHandler imageChanged = (_, _) => changed++; EventHandler sourcesChanged = (_, _) => publications++;
            slot.ImageChanged += imageChanged; viewer.SourcesChanged += sourcesChanged;
            try
            {
                host.Workbench.IsRightCollapsed = true;
                pipeline.SelectStepCommand.Execute(0); Pump();
                Require(!host.Workbench.IsRightCollapsed && ReferenceEquals(document, host.ActiveNativeDocumentForTest), label + " reopens same panel");
                Require(SlotPixels(slot) == pixels && pipeline.InputRevision == revision && host.NativePreviewRunCount == runs
                    && ReferenceEquals(before, pipeline.GetResult(0)), label + " reselect preserves pixels, input and result without execution");
                if (allowRouteRebind)
                {
                    // A changed route may require one guarded editor rebind; its next identical selection is a no-op.
                    changed = publications = 0; pipeline.SelectStepCommand.Execute(0); Pump();
                }
                Require(changed == 0 && publications == 0 && SlotPixels(slot) == pixels, label + " stable reselect does not recopy or republish");
            }
            finally { slot.ImageChanged -= imageChanged; viewer.SourcesChanged -= sourcesChanged; }
        }
    }

    private static System.Drawing.Bitmap Gray(int width, int height, int value)
    {
        var bitmap = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using var graphics = System.Drawing.Graphics.FromImage(bitmap); graphics.Clear(System.Drawing.Color.FromArgb(value, value, value)); return bitmap;
    }
    private static string WriteGray(string path, int width, int height, int value)
    {
        using var bitmap = Gray(width, height, value); bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png); return BitmapPixels(bitmap);
    }
    private static string BitmapPixels(System.Drawing.Bitmap bitmap)
    {
        byte[] bytes = new byte[8 + bitmap.Width * bitmap.Height * 4];
        BitConverter.GetBytes(bitmap.Width).CopyTo(bytes, 0); BitConverter.GetBytes(bitmap.Height).CopyTo(bytes, 4);
        for (int y = 0; y < bitmap.Height; y++) for (int x = 0; x < bitmap.Width; x++)
            BitConverter.GetBytes(bitmap.GetPixel(x, y).ToArgb()).CopyTo(bytes, 8 + (y * bitmap.Width + x) * 4);
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes));
    }
    private static string SlotPixels(VisionToolInlinePreviewSlot slot)
    {
        if (!slot.HasImage) return "";
        if (typeof(VisionToolInlinePreviewSlot).GetField("pendingImage", Private)!.GetValue(slot) is System.Drawing.Bitmap pending) return BitmapPixels(pending);
        var source = ((Image)typeof(VisionToolInlinePreviewSlot).GetField("previewImage", Private)!.GetValue(slot)!).Source as System.Windows.Media.Imaging.BitmapSource;
        Require(source != null, "inline preview owns pending or rendered pixels");
        var converted = new System.Windows.Media.Imaging.FormatConvertedBitmap(source, System.Windows.Media.PixelFormats.Bgra32, null, 0);
        byte[] pixels = new byte[8 + converted.PixelWidth * converted.PixelHeight * 4];
        BitConverter.GetBytes(converted.PixelWidth).CopyTo(pixels, 0); BitConverter.GetBytes(converted.PixelHeight).CopyTo(pixels, 4);
        byte[] rows = new byte[pixels.Length - 8]; converted.CopyPixels(rows, converted.PixelWidth * 4, 0); rows.CopyTo(pixels, 8);
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(pixels));
    }

    private static void Logs(OpenVisionShellHostView host)
    {
        LogPanelView view = new(); view.Resources.MergedDictionaries.Add(host.Resources);
        LogPanelViewModel model = (LogPanelViewModel)view.DataContext;
        ((DispatcherTimer)typeof(LogPanelViewModel).GetField("refreshTimer", Private)!.GetValue(model)!).Stop();
        try
        {
            view.Width = 1200; view.Height = 320; view.Measure(new Size(1200, 320)); view.Arrange(new Rect(0, 0, 1200, 320)); view.UpdateLayout();
            foreach (int count in new[] { 100, 3000 })
            {
                model.AutoScroll = false; model.Logs.Clear(); model.SearchText = "fixture-reset";
                for (int i = 0; i < count; i++) model.Logs.Add(new LogLine(DateTime.Now, "", i % 2 == 0 ? "System" : "Recipe", i % 3 == 0 ? "Error" : i % 3 == 1 ? "Warning" : "Info", "OwnedPerfFixture", "sample-" + i, (i % 5 == 0 ? "needle-" : "sample-") + i));
                model.ShowEntireStream = true; model.SelectedType = "Any"; model.SelectedLevel = "Any"; model.SearchText = ""; model.IsDetailedMode = true; Pump(); model.AutoScroll = true;
                LogCase("L01-search-" + count, () => model.SearchText = "needle", 1, count / 5, 1);
                LogCase("L02-same-search-" + count, () => model.SearchText = "needle", 0, count / 5, 0);
                LogCase("L03-clear-search-" + count, () => model.SearchText = "", 1, count, 1);
                model.AutoScroll = false; model.ShowEntireStream = false; model.SelectedType = "System"; model.SelectedLevel = "Warning"; model.SearchText = "needle"; model.IsDetailedMode = true; Pump(); model.AutoScroll = true;
                LogCase("L04-detail-to-summary-" + count, () => model.ToggleDetailCommand.Execute(null), 1, count, 1);
                model.AutoScroll = false; LogCase("L05-autoscroll-off-" + count, () => model.SearchText = "needle", 1, count / 5, 0);
                model.AutoScroll = true;
                LogCase("L06-quick-filter-" + count, () => LogPanelViewModel.ApplyQuickFilter("System", "Error", "needle", false), 1, model.Logs.Count(line => line.Category == "System" && line.Level == "Error" && line.RawText.Contains("needle")), 1);
                LogCase("L07-quick-filter-noop-" + count, () => LogPanelViewModel.ApplyQuickFilter("System", "Error", "needle", false), 0, model.FilteredLogs.Count, 0);
            }
            model.AutoScroll = false; model.ShowEntireStream = true; model.SearchText = "";
            MethodInfo tick = typeof(LogPanelViewModel).GetMethod("RefreshTimer_Tick", Private)!;
            tick.Invoke(model, new object?[] { null, EventArgs.Empty }); model.ResetCommand.Execute(null);
            model.SearchText = "owned-incoming-"; model.AutoScroll = true;
            for (int i = 0; i < 200; i++) OpenVisionLab.Logging.OVLog.Write(OpenVisionLab.Logging.LogCategory.System, "owned-incoming-" + i);
            tick.Invoke(model, new object?[] { null, EventArgs.Empty });
            Require(model.FilteredLogs.Count == 200 && Pending()?.Status == DispatcherOperationStatus.Pending, "runtime log stream burst retains all rows with one pending scroll"); Pump();
            OpenVisionLab.Logging.OVLog.Write(OpenVisionLab.Logging.LogCategory.System, "owned-incoming-next");
            tick.Invoke(model, new object?[] { null, EventArgs.Empty });
            Require(model.FilteredLogs.Count == 201 && Pending()?.Status == DispatcherOperationStatus.Pending, "incremental runtime log queues next scroll"); Pump();
            cases.Add(new { CaseId = "L08-runtime-stream", Outcome = "Pass", BurstRows = 200, IncrementalRows = 1 });
            model.AutoScroll = true; model.ShowEntireStream = true; model.SearchText = ""; Pump();
            model.FilteredLogs.Add(model.Logs[0]); Require(Pending() != null, "incoming row queues scroll");
            DispatcherOperation operation = Pending()!; model.FilteredLogs.Add(model.Logs[1]); Require(ReferenceEquals(operation, Pending()), "burst keeps one pending scroll");
            model.AutoScroll = false; Require(operation.Status == DispatcherOperationStatus.Aborted && Pending() == null, "AutoScroll OFF aborts pending"); Pump();
            model.AutoScroll = true; model.FilteredLogs.Add(model.Logs[0]); operation = Pending()!; model.FilteredLogs.Clear(); Require(operation.Status == DispatcherOperationStatus.Aborted && Pending() == null, "empty cancels pending"); Pump();
            model.FilteredLogs.Add(model.Logs[0]); operation = Pending()!;
            view.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));
            Require(operation.Status == DispatcherOperationStatus.Aborted && Pending() == null, "unload cancels pending"); Pump();
            cases.Add(new { CaseId = "L09-pending-lifetime", Outcome = "Pass", BurstMaximum = 1, AutoScrollOff = "Aborted", Empty = "Aborted", Unload = "Aborted" });
        }
        finally { model.Dispose(); }

        DispatcherOperation? Pending() => (DispatcherOperation?)typeof(LogPanelView).GetField("pendingScroll", Private)!.GetValue(view);
        void LogCase(string id, Action action, int resetsExpected, int rowsExpected, int postedExpected)
        {
            Pump(); int resets = 0, adds = 0, posted = 0, finished = 0, maxPending = 0;
            NotifyCollectionChangedEventHandler collection = (_, e) => { if (e.Action == NotifyCollectionChangedAction.Reset) resets++; if (e.Action == NotifyCollectionChangedAction.Add) adds += e.NewItems!.Count; };
            FieldInfo method = typeof(DispatcherOperation).GetField("_method", Private)!;
            HashSet<DispatcherOperation> scrolls = new();
            DispatcherHookEventHandler post = (_, e) => { var d = method.GetValue(e.Operation) as Delegate; if (d?.Method.DeclaringType?.FullName?.Contains("LogPanelView") == true && d.Method.Name.Contains("FilteredLogs_CollectionChanged")) { posted++; scrolls.Add(e.Operation); maxPending = Math.Max(maxPending, scrolls.Count(x => x.Status == DispatcherOperationStatus.Pending)); } };
            DispatcherHookEventHandler complete = (_, e) => { if (scrolls.Contains(e.Operation)) finished++; };
            model.FilteredLogs.CollectionChanged += collection; Dispatcher.CurrentDispatcher.Hooks.OperationPosted += post; Dispatcher.CurrentDispatcher.Hooks.OperationCompleted += complete;
            try
            {
                Measure(id, action, () => new { Resets = resets, AddedRows = adds, Rows = model.FilteredLogs.Count, PostedScrollCallbacks = posted, CompletedScrollCallbacks = finished, MaximumPending = maxPending });
                var expected = model.Logs.Where(line => string.IsNullOrWhiteSpace(model.SearchText) || line.RawText.Contains(model.SearchText, StringComparison.OrdinalIgnoreCase)).Where(line => model.ShowEntireStream || (model.SelectedType == "Any" || line.Category == model.SelectedType) && (model.SelectedLevel == "Any" || line.Level == model.SelectedLevel));
                Require(resets == resetsExpected && model.FilteredLogs.Count == rowsExpected && posted == postedExpected && finished == posted && maxPending <= 1 && model.FilteredLogs.SequenceEqual(expected), id + " final filters, collections, scroll publication");
            }
            finally { model.FilteredLogs.CollectionChanged -= collection; Dispatcher.CurrentDispatcher.Hooks.OperationPosted -= post; Dispatcher.CurrentDispatcher.Hooks.OperationCompleted -= complete; }
        }
    }

    private static void OpenTool(OpenVisionShellHostView host, VISION_MENU menu)
    {
        var controller = (OpenVisionShellHostToolWindowController)typeof(OpenVisionShellHostView).GetField("toolWindowController", Private)!.GetValue(host)!;
        var item = host.Workbench.Shell.NavigationGroups.SelectMany(group => group.Items).Single(item => item.Menu == menu);
        Require(controller.ShowSelectedTool(item), "production tool controller opens " + menu);
    }

    private static void Measure(string id, Action action, Func<object> state)
    {
        long bytes = GC.GetAllocatedBytesForCurrentThread(); Stopwatch watch = Stopwatch.StartNew(); action(); watch.Stop(); double sync = watch.Elapsed.TotalMilliseconds; long allocation = GC.GetAllocatedBytesForCurrentThread() - bytes;
        watch.Restart(); Pump(); watch.Stop();
        cases.Add(new { CaseId = id, SynchronousMs = sync, UiAllocatedBytes = allocation, DispatcherDrainMs = watch.Elapsed.TotalMilliseconds, State = state() }); Save("Running", null);
    }
    private static void Pump() { DispatcherFrame frame = new(); Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => frame.Continue = false)); Dispatcher.PushFrame(frame); }
    private static void Require(bool condition, string contract) { if (!condition) throw new InvalidOperationException(contract); }
    private static void Save(string status, string? error) => File.WriteAllText(Path.Combine(evidence, "measurements.json"), JsonSerializer.Serialize(new { Status = status, Error = error, Product = typeof(OpenVisionWorkbenchViewModel).Assembly.Location, BinarySha256 = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(typeof(OpenVisionWorkbenchViewModel).Assembly.Location))), Boundary = "Unshown in-process production Window/View/commands and real dispatcher only; no Window.Show or GUI input. Not actual EXE/pixel-present/native DPI or latency-percentile evidence.", Cases = cases }, new JsonSerializerOptions { WriteIndented = true }));
}
