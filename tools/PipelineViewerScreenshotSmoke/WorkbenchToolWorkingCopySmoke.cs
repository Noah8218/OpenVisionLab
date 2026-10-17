using OpenVisionLab;
using OpenVisionLab.Core;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Automation;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using WpgPropertyGrid = System.Windows.Controls.WpfPropertyGrid.PropertyGrid;
using static OpenVisionLab.DEFINE;

internal static class WorkbenchToolWorkingCopySmoke
{
    internal static void Run(OpenVisionShellHostView host, string outputPath, string templatePath, Action<int> pump, VISION_MENU[]? selectedMenus = null)
    {
        VISION_MENU[] menus = selectedMenus ?? new[]
        {
            VISION_MENU.Threshold, VISION_MENU.Filter, VISION_MENU.Morphology, VISION_MENU.Arithmetic,
            VISION_MENU.EdgeDetection, VISION_MENU.HSV, VISION_MENU.Histogram,
            VISION_MENU.Matching, VISION_MENU.EdgeBasedMatching, VISION_MENU.FeatureMatching,
            VISION_MENU.RotateAndScale, VISION_MENU.AffineTransform, VISION_MENU.Blob,
            VISION_MENU.Contour, VISION_MENU.Line, VISION_MENU.Mean
        };
        string recipeDirectory = RecipeWorkspaceService.GetRecipeWorkspaceDirectory(host.ActiveRecipeContextNameForTest);
        Dictionary<string, string> files = SnapshotFiles(recipeDirectory);
        OpenVisionWorkbenchPipelineViewModel pipeline = host.Workbench.Pipeline;
        List<string> checks = new();
        int layers = host.LayerDocumentCount;
        int addedCount = 0;
        foreach (VISION_MENU menu in menus)
        {
            host.SelectToolForTest(menu);
            pump(12);
            OpenVisionNativeToolDocument document = host.ActiveNativeDocumentForTest ?? throw new InvalidOperationException(menu + ": native document missing");
            Assert(host.Workbench.Tool.IsNewTest, menu + ": opening must select test mode");
            FrameworkElement originalView = document.View;
            object? originalProperty = GetSelectedProperty(originalView);
            int runs = document.PreviewRunCount;
            Assert(host.Workbench.Tool.Title == string.Format(CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T("Workbench.Tool.NewFormat"), OpenVisionShellCommandCatalog.DisplayName(menu)), menu + ": test title");
            AssertChrome(host, document, host.Workbench.Tool, false);
            Assert(!pipeline.IsDirty || pipeline.StepCount == addedCount, menu + ": opening must not edit pipeline");
            AssertFiles(files, recipeDirectory, menu + ": opening");

            if (menu is VISION_MENU.Matching or VISION_MENU.EdgeBasedMatching or VISION_MENU.FeatureMatching)
            {
                string beforeRejected = StepXml(pipeline.WorkingCopyForTest);
                Assert(document.AddPipelineStep() == null && StepXml(pipeline.WorkingCopyForTest) == beforeRejected,
                    menu + ": no-template add must reject without changing working copy");
                if (menu == VISION_MENU.Matching) { host.SetActiveMatchingTemplatePathForTest(templatePath); }
                else if (menu == VISION_MENU.EdgeBasedMatching) { host.SetActiveEdgeBasedMatchingTemplatePathForTest(templatePath); }
                else { host.SetActiveFeatureMatchingTemplatePathForTest(templatePath); }
            }

            ChangeParameter(document);
            document.RecordParameterChange(null);
            pump(3);
            VisionPipelineStep? testStep = VisionPipelineStepEditService.CopyStep(host.Workbench.Tool.CurrentStep);
            string parameters = StepXml(testStep);
            string? auxiliarySettings = originalView is SimplePreprocessToolWpfView simple ? ObjectXml(simple.Parameters.CaptureSettings()) : null;
            int afterChangeRuns = document.PreviewRunCount;
            host.SelectToolForTest(menu == VISION_MENU.Filter ? VISION_MENU.Threshold : VISION_MENU.Filter);
            pump(4);
            host.SelectToolForTest(menu);
            pump(14);
            Assert(ReferenceEquals(originalView, host.ActiveNativeDocumentForTest?.View), menu + ": view must be reused");
            Assert(ReferenceEquals(originalProperty, GetSelectedProperty(originalView)), menu + ": PropertyGrid object must be preserved");
            Assert(StepXml(host.Workbench.Tool.CurrentStep) == parameters, menu + ": test parameter/route restoration");
            if (GetSelectedProperty(originalView) is EdgeBasedMatchingProperty restoredEdge)
            {
                Assert(restoredEdge.SCORE_MIN == double.Parse(testStep!.Parameters["SCORE_MIN"], CultureInfo.InvariantCulture)
                    && restoredEdge.CANNY_LOW == int.Parse(testStep.Parameters["CANNY_LOW"], CultureInfo.InvariantCulture)
                    && restoredEdge.CANNY_HIGH == int.Parse(testStep.Parameters["CANNY_HIGH"], CultureInfo.InvariantCulture),
                    "EdgeBasedMatching: native score/Canny values must survive restoration; expected="
                    + testStep.Parameters["SCORE_MIN"] + "/" + testStep.Parameters["CANNY_LOW"] + "/" + testStep.Parameters["CANNY_HIGH"]
                    + "; actual=" + restoredEdge.SCORE_MIN + "/" + restoredEdge.CANNY_LOW + "/" + restoredEdge.CANNY_HIGH);
            }
            if (auxiliarySettings != null)
                Assert(ObjectXml(((SimplePreprocessToolWpfView)originalView).Parameters.CaptureSettings()) == auxiliarySettings, menu + ": all SimplePreprocess settings restored");
            Assert(document.PreviewRunCount == afterChangeRuns && host.LayerDocumentCount == layers, menu + ": restoration must not execute or create layers");
            AssertFiles(files, recipeDirectory, menu + ": parameter change and restore");

            if (menu is VISION_MENU.HSV or VISION_MENU.Histogram)
            {
                Assert(document.AddPipelineStep() == null && pipeline.StepCount == addedCount, menu + ": unsupported add rejected");
                checks.Add(menu + ": test/restoration/non-addable/no-write PASS");
                continue;
            }
            VisionPipelineStep? added = document.AddPipelineStep();
            pump(10);
            addedCount++;
            Assert(added != null && pipeline.StepCount == addedCount && pipeline.SelectedStepIndex == addedCount - 1,
                menu + ": add must insert/select one memory step");
            Assert(host.Workbench.Tool.IsStepEditing && host.Workbench.Tool.ToolName == document.ToolName, menu + ": add must enter step mode");
            VisionPipelineStep actualStep = pipeline.GetStep(addedCount - 1)!;
            File.WriteAllText(Path.ChangeExtension(outputPath, "." + menu + ".expected-step.txt"), ObjectXml(testStep!));
            File.WriteAllText(Path.ChangeExtension(outputPath, "." + menu + ".actual-step.txt"), ObjectXml(actualStep));
            Assert(actualStep.Parameters.OrderBy(item => item.Key).SequenceEqual(testStep!.Parameters.OrderBy(item => item.Key)),
                menu + ": added parameters preserved; differences=" + ParameterDifferences(testStep, actualStep));
            AssertChrome(host, document, host.Workbench.Tool, true);
            AssertFiles(files, recipeDirectory, menu + ": add must not save XML");
            ChangeParameter(document);
            document.RecordParameterChange(null);
            pump(3);
            Assert(pipeline.SelectedRow?.IsDirty == true && host.Workbench.Tool.BadgeText == OpenVisionLanguageService.T("Workbench.Tool.BadgeChanged"), menu + ": step edit must be dirty");
            string edited = StepXml(pipeline.GetStep(addedCount - 1));
            pipeline.UndoCommand.Execute(null);
            pump(5);
            pipeline.RedoCommand.Execute(null);
            pump(8);
            Assert(StepXml(pipeline.GetStep(addedCount - 1)) == edited, menu + ": parameter undo/redo round trip");
            AssertFiles(files, recipeDirectory, menu + ": step edit/undo/redo");
            checks.Add(menu + ": same view/property, test restore, memory add, step edit/undo/redo/no-write PASS; runs before change=" + runs);
            File.WriteAllText(Path.ChangeExtension(outputPath, ".working-copy-progress.txt"), string.Join(Environment.NewLine, checks));
        }
        int expectedAdds = menus.Count(menu => menu is not VISION_MENU.HSV and not VISION_MENU.Histogram);
        Assert(addedCount == expectedAdds && pipeline.StepCount == expectedAdds, "supported tools must add exactly one step each");
        File.WriteAllText(Path.ChangeExtension(outputPath, ".working-copy.txt"), string.Join(Environment.NewLine, checks));
    }

    internal static void RunMenuState(OpenVisionShellHostView host, string outputPath, string templatePath, Action<int> pump, VISION_MENU[]? selectedMenus = null)
    {
        VISION_MENU[] menus = selectedMenus ?? new[]
        {
            VISION_MENU.Matching, VISION_MENU.EdgeBasedMatching, VISION_MENU.FeatureMatching, VISION_MENU.AffineTransform,
            VISION_MENU.Filter, VISION_MENU.Threshold, VISION_MENU.RotateAndScale, VISION_MENU.Morphology,
            VISION_MENU.Arithmetic, VISION_MENU.EdgeDetection, VISION_MENU.HSV, VISION_MENU.Histogram, VISION_MENU.Blob,
            VISION_MENU.Contour, VISION_MENU.Line, VISION_MENU.Mean
        };
        string recipeDirectory = RecipeWorkspaceService.GetRecipeWorkspaceDirectory(host.ActiveRecipeContextNameForTest);
        Dictionary<string, string> files = SnapshotFiles(recipeDirectory);
        OpenVisionWorkbenchPipelineViewModel pipeline = host.Workbench.Pipeline;
        List<string> checks = new();
        int layers = host.LayerDocumentCount;
        foreach (VISION_MENU menu in menus)
        {
            host.SelectToolForTest(menu);
            pump(12);
            OpenVisionNativeToolDocument document = host.ActiveNativeDocumentForTest!;
            OpenVisionWorkbenchToolViewModel model = host.Workbench.Tool;
            FrameworkElement originalView = document.View;
            object? originalProperty = GetSelectedProperty(originalView);
            int runs = document.PreviewRunCount;
            string defaults = ParameterState(document);
            Assert(model.ParameterGuideCommand.CanExecute(null), menu + ": tool menu must expose the existing parameter guide");
            string pipelineBefore = StepXml(pipeline.WorkingCopyForTest);
            ChangeParameter(document);
            document.RecordParameterChange(null);
            Assert(ParameterState(document) != defaults, menu + ": fixture must differ from defaults");
            VisionPipelineStep draft = model.CurrentStep!;
            draft.UseAcceptance = true;
            draft.AcceptanceMetricName = VisionPipelineKnownMetrics.GetMetricNamesForTool(draft.ToolType).First();
            draft.MaxElapsedMilliseconds = 987;
            draft.Parameters["Smoke.Custom"] = "preserved";
            string input = draft.InputLayer;
            string output = draft.OutputLayer;
            Assert(model.RestoreDefaultsCommand.CanExecute(null), menu + ": test defaults command must be enabled");
            model.RestoreDefaultsCommand.Execute(null);
            pump(5);
            Assert(ParameterState(document) == defaults, menu + ": defaults must restore the original factory parameter state");
            Assert(draft.InputLayer == input && draft.OutputLayer == output && draft.UseAcceptance
                && draft.MaxElapsedMilliseconds == 987 && draft.Parameters["Smoke.Custom"] == "preserved", menu + ": defaults must retain routing, acceptance and extension values");
            Assert(ReferenceEquals(originalProperty, GetSelectedProperty(originalView)), menu + ": defaults must retain the PropertyGrid instance");
            Assert(!document.HasPreviewResult && StepXml(pipeline.WorkingCopyForTest) == pipelineBefore, menu + ": defaults must clear displayed results without changing the pipeline");

            if (menu is VISION_MENU.Matching or VISION_MENU.EdgeBasedMatching or VISION_MENU.FeatureMatching)
            {
                Assert(document.AddPipelineStep() == null && StepXml(pipeline.WorkingCopyForTest) == pipelineBefore, menu + ": reset template must reject add");
                if (menu == VISION_MENU.Matching) { host.SetActiveMatchingTemplatePathForTest(templatePath); }
                else if (menu == VISION_MENU.EdgeBasedMatching) { host.SetActiveEdgeBasedMatchingTemplatePathForTest(templatePath); }
                else { host.SetActiveFeatureMatchingTemplatePathForTest(templatePath); }
            }
            ChangeParameter(document);
            document.RecordParameterChange(null);
            pump(3);
            if (menu is not VISION_MENU.HSV and not VISION_MENU.Histogram)
            {
                Assert(document.AddPipelineStep() != null && model.IsStepEditing, menu + ": arrange editable step");
                pump(6);
                int index = pipeline.SelectedStepIndex;
                VisionPipelineStep source = pipeline.GetStep(index)!;
                string parameters = ParameterState(document);
                pipelineBefore = StepXml(pipeline.WorkingCopyForTest);
                var binding = (OpenVisionWorkbenchToolViewModel.EditorBinding)typeof(OpenVisionWorkbenchToolViewModel)
                    .GetField("editor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(model)!;
                Func<bool> running = binding.IsRunning;
                binding.IsRunning = () => true;
                try
                {
                    Assert(!model.NewTestCommand.CanExecute(null) && !model.DuplicateCommand.CanExecute(null)
                        && !model.DeleteCommand.CanExecute(null) && !model.RenameCommand.CanExecute(null), menu + ": native running callback must disable edits");
                    model.NewTestCommand.Execute(null);
                    model.DuplicateCommand.Execute(null);
                    Assert(model.IsStepEditing && StepXml(pipeline.WorkingCopyForTest) == pipelineBefore, menu + ": forced repeated command must still respect the running guard");
                }
                finally { binding.IsRunning = running; }
                Assert(!model.RestoreDefaultsCommand.CanExecute(null) && model.NewTestCommand.CanExecute(null), menu + ": step command mode");
                model.NewTestCommand.Execute(null);
                pump(8);
                Assert(model.IsNewTest && ReferenceEquals(originalView, host.ActiveNativeDocumentForTest!.View)
                    && ReferenceEquals(originalProperty, GetSelectedProperty(originalView)), menu + ": new test must reuse the native View and property object");
                VisionPipelineStep copy = VisionPipelineStepEditService.CopyStep(source);
                copy.Name = OpenVisionShellCommandCatalog.DisplayName(menu);
                copy.InputLayer = source.OutputLayer;
                copy.OutputLayer = model.CurrentStep!.OutputLayer;
                copy.Enabled = true;
                Assert(StepXml(model.CurrentStep) == StepXml(copy) && string.IsNullOrEmpty(pipeline.ValidateOutput(copy.OutputLayer)), menu + ": new test must copy parameters, acceptance, template and secondary route with a unique output");
                Assert(ParameterState(document) == parameters && StepXml(pipeline.WorkingCopyForTest) == pipelineBefore, menu + ": cloning must preserve native parameters and the source step");
                ChangeParameter(document);
                document.RecordParameterChange(null);
                pump(3);
                Assert(StepXml(pipeline.WorkingCopyForTest) == pipelineBefore, menu + ": editing a copied test must not edit its source");
                pipeline.SelectStepCommand.Execute(index);
                pump(7);
                string restored = ParameterState(document);
                File.WriteAllText(Path.ChangeExtension(outputPath, "." + menu + ".source-native.txt"), parameters);
                File.WriteAllText(Path.ChangeExtension(outputPath, "." + menu + ".restored-native.txt"), restored);
                File.WriteAllText(Path.ChangeExtension(outputPath, "." + menu + ".source-step.txt"), ObjectXml(source));
                Assert(model.IsStepEditing && restored == parameters, menu + ": returning to source must restore its values");
                pipeline.NewTestCommand.Execute(null);
                pump(7);
                Assert(model.IsNewTest && StepXml(model.CurrentStep) == StepXml(copy), menu + ": row-menu command must create the same test draft");
            }
            Assert(document.PreviewRunCount == runs && host.LayerDocumentCount == layers, menu + ": menu commands must not execute or create layers");
            AssertFiles(files, recipeDirectory, menu + ": menu commands must not write recipe files");
            checks.Add(menu + ": defaults, same native object, routes/acceptance/extensions, template guard, copied test/source isolation, no run/layer/write PASS");
            File.WriteAllText(Path.ChangeExtension(outputPath, ".menu-state.txt"), string.Join(Environment.NewLine, checks));
        }
    }

    internal static void RunAcceptance(OpenVisionShellHostView host, string outputPath, Action<int> pump, Action<string> capture)
    {
        OpenVisionWorkbenchPipelineViewModel pipeline = host.Workbench.Pipeline;
        string recipeDirectory = RecipeWorkspaceService.GetRecipeWorkspaceDirectory(host.ActiveRecipeContextNameForTest);
        Dictionary<string, string> files = SnapshotFiles(recipeDirectory);
        int layers = host.LayerDocumentCount;
        List<string> checks = new();
        VISION_MENU[] menus = { VISION_MENU.Blob, VISION_MENU.Contour, VISION_MENU.Line, VISION_MENU.Matching,
            VISION_MENU.EdgeBasedMatching, VISION_MENU.FeatureMatching, VISION_MENU.Mean, VISION_MENU.HSV,
            VISION_MENU.Threshold, VISION_MENU.Arithmetic };
        foreach (bool light in new[] { false, true })
        {
            host.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/OpenVisionLab;component/UI/Wpf/OpenVisionWorkbench" + (light ? "Light" : "Dark") + ".xaml") });
            foreach (OpenVisionLanguage language in new[] { OpenVisionLanguage.Korean, OpenVisionLanguage.English })
            {
                OpenVisionLanguageService.SetLanguage(language, false);
                foreach (VISION_MENU menu in menus)
                {
                    host.SelectToolForTest(menu);
                    pump(7);
                    OpenVisionNativeToolDocument document = host.ActiveNativeDocumentForTest!;
                    OpenVisionWorkbenchToolViewModel model = host.Workbench.Tool;
                    bool expected = menu is not VISION_MENU.Threshold and not VISION_MENU.Arithmetic;
                    Assert(model.HasAcceptance == expected, menu + ": acceptance surface eligibility");
                    if (!expected)
                    {
                        if (document.View.FindName("toolShell") is VisionToolSingleInputPropertyToolShell omitted)
                            Assert(!omitted.WorkbenchAcceptanceHost.IsVisible, menu + ": no empty acceptance section");
                        checks.Add($"{(light ? "Light" : "Dark")}/{language}/{menu}: no acceptance surface PASS");
                        continue;
                    }
                    VisionToolSingleInputPropertyToolShell shell = (VisionToolSingleInputPropertyToolShell)document.View.FindName("toolShell");
                    ContentControl surface = shell.WorkbenchAcceptanceHost;
                    ResourceDictionary palette = new() { Source = new Uri("pack://application:,,,/OpenVisionLab;component/UI/Wpf/OpenVisionWorkbench" + (light ? "Light" : "Dark") + ".xaml") };
                    surface.Resources.MergedDictionaries.Add(palette);
                    surface.BringIntoView();
                    pump(2);
                    T Control<T>(string id) where T : FrameworkElement => Program.FindVisualChildren<T>(surface).Single(item => AutomationProperties.GetAutomationId(item) == id);
                    ToggleButton enabled = Control<ToggleButton>("WorkbenchToolAcceptanceToggle");
                    ComboBox metrics = Control<ComboBox>("WorkbenchToolAcceptanceMetric");
                    CheckBox useMinimum = Control<CheckBox>("WorkbenchToolAcceptanceUseMinimum");
                    CheckBox useMaximum = Control<CheckBox>("WorkbenchToolAcceptanceUseMaximum");
                    TextBox minimum = Control<TextBox>("WorkbenchToolAcceptanceMinimum");
                    TextBox maximum = Control<TextBox>("WorkbenchToolAcceptanceMaximum");
                    TextBox time = Control<TextBox>("WorkbenchToolAcceptanceTime");
                    TextBlock error = Control<TextBlock>("WorkbenchToolAcceptanceError");
                    object? property = GetSelectedProperty(document.View);
                    string parameterSnapshot = ParameterState(document);
                    int runs = document.PreviewRunCount;
                    VisionPipelineStep draft = model.CurrentStep!;
                    double originalTime = draft.MaxElapsedMilliseconds;
                    Assert(double.Parse(time.Text, CultureInfo.CurrentCulture) == originalTime, menu + ": existing time is not replaced by proposal default");
                    draft.UseAcceptance = true;
                    draft.AcceptanceMetricName = model.AcceptanceMetrics.First().Key;
                    draft.UseAcceptanceMetricMinimum = draft.UseAcceptanceMetricMaximum = true;
                    draft.AcceptanceMetricMinimum = 13;
                    draft.AcceptanceMetricMaximum = 21;
                    draft.MaxElapsedMilliseconds = 321;
                    draft.ExpectedSuccess = false;
                    draft.RequiredMessageText = "owned acceptance fixture";
                    draft.Parameters["Smoke.AcceptanceOpaque"] = "retained";
                    model.NotifyExecutionState(GetGeneration(model));
                    pump(3);
                    Assert(surface.IsVisible && ReferenceEquals(enabled.DataContext, model) && enabled.IsChecked == true
                        && minimum.Text == "13" && maximum.Text == "21" && time.Text == "321", menu + ": rendered acceptance binding");
                    Assert(((IEnumerable<KeyValuePair<string, string>>)metrics.ItemsSource).SequenceEqual(model.AcceptanceMetrics)
                        && model.AcceptanceMetrics.All(metric => VisionPipelineKnownMetrics.GetMetricNamesForTool(draft.ToolType).Contains(metric.Key)), menu + ": only produced metrics");
                    enabled.SetCurrentValue(ToggleButton.IsCheckedProperty, false);
                    enabled.GetBindingExpression(ToggleButton.IsCheckedProperty)!.UpdateSource();
                    pump(3);
                    Assert(!model.UseAcceptance && !metrics.IsEnabled && !useMinimum.IsEnabled && !useMaximum.IsEnabled
                        && !minimum.IsEnabled && !maximum.IsEnabled && !time.IsEnabled, menu + ": off disables rendered fields");
                    Assert(((SolidColorBrush)time.Foreground).Color == ((SolidColorBrush)palette["OpenVision.Workbench.MutedTextBrush"]).Color
                        && ((SolidColorBrush)((Border)time.Template.FindName("fieldChrome", time)).Background).Color == ((SolidColorBrush)palette["OpenVision.Workbench.PanelAltBrush"]).Color,
                        menu + ": disabled numeric text and chrome retain the semantic theme");
                    model.AcceptanceMinimumText = "99";
                    Assert(draft.AcceptanceMetricMinimum == 13 && draft.AcceptanceMetricMaximum == 21 && draft.MaxElapsedMilliseconds == 321
                        && !draft.ExpectedSuccess && draft.RequiredMessageText == "owned acceptance fixture", menu + ": off retains all criteria and hidden metadata");
                    if (!light && language == OpenVisionLanguage.Korean && menu == VISION_MENU.Blob) { capture("dark-ko-Blob-off"); }
                    enabled.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
                    enabled.GetBindingExpression(ToggleButton.IsCheckedProperty)!.UpdateSource();
                    pump(3);
                    Assert(model.UseAcceptance && metrics.IsEnabled && minimum.IsEnabled && maximum.IsEnabled && time.IsEnabled,
                        menu + ": on restores editability");
                    useMinimum.SetCurrentValue(ToggleButton.IsCheckedProperty, false);
                    useMinimum.GetBindingExpression(ToggleButton.IsCheckedProperty)!.UpdateSource();
                    pump(2);
                    Assert(!model.UseAcceptanceMinimum && !minimum.IsEnabled && minimum.Text == "13", menu + ": minimum flag preserves its value");
                    useMinimum.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
                    useMinimum.GetBindingExpression(ToggleButton.IsCheckedProperty)!.UpdateSource();
                    pump(2);
                    if (model.AcceptanceMetrics.Count > 1)
                    {
                        string choice = model.AcceptanceMetrics[1].Key;
                        metrics.SetCurrentValue(ComboBox.SelectedValueProperty, choice);
                        metrics.GetBindingExpression(ComboBox.SelectedItemProperty)!.UpdateSource();
                        pump(2);
                        Assert(draft.AcceptanceMetricName == choice && (string)metrics.SelectedValue == choice, menu + ": metric binding writes working draft");
                    }
                    string acceptedMetric = draft.AcceptanceMetricName;
                    model.AcceptanceMetricName = "UnsupportedMetric";
                    Assert(draft.AcceptanceMetricName == acceptedMetric, menu + ": unproduced metric cannot be selected");
                    foreach (string invalid in new[] { "bad", "NaN", "Infinity", "30" })
                    {
                        minimum.SetCurrentValue(TextBox.TextProperty, invalid);
                        minimum.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
                        pump(2);
                        Assert(model.HasAcceptanceError && Validation.GetHasError(minimum) && error.IsVisible && error.Text == model.AcceptanceError
                            && !shell.RunPreviewButton.IsEnabled && !model.AddAfterCommand.CanExecute(null) && draft.AcceptanceMetricMinimum == 13,
                            menu + ": invalid field is visible, uncommitted and blocks native run/add: " + invalid);
                        Assert(((SolidColorBrush)((Border)minimum.Template.FindName("fieldChrome", minimum)).BorderBrush).Color
                            == ((SolidColorBrush)palette["OpenVision.Workbench.NgBrush"]).Color, menu + ": invalid field has a visible semantic error border");
                        string status = document.LastStatusText;
                        document.RunPreview();
                        model.AddAfterCommand.Execute(null);
                        pump(2);
                        Assert(!document.IsPreviewRunning && document.LastStatusText == status && pipeline.StepCount == 0,
                            menu + ": forced invalid actions are guarded");
                        if (!light && language == OpenVisionLanguage.Korean && menu == VISION_MENU.Blob && invalid == "30") { capture("dark-ko-Blob-invalid-range"); }
                        KeyBinding escape = minimum.InputBindings.OfType<KeyBinding>().Single();
                        Assert(escape.Key == System.Windows.Input.Key.Escape && ReferenceEquals(escape.Command, model.CancelAcceptanceEditCommand), menu + ": Escape is bound to the editor owner");
                        escape.Command.Execute(escape.CommandParameter);
                        pump(2);
                        Assert(!model.HasAcceptanceError && !Validation.GetHasError(minimum) && minimum.Text == "13" && shell.RunPreviewButton.IsEnabled,
                            menu + ": Escape restores the last committed field and actions");
                    }
                    time.SetCurrentValue(TextBox.TextProperty, "-1");
                    time.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
                    pump(2);
                    Assert(model.HasAcceptanceError && Validation.GetHasError(time) && draft.MaxElapsedMilliseconds == 321, menu + ": negative time remains uncommitted");
                    time.SetCurrentValue(TextBox.TextProperty, "750");
                    time.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
                    pump(2);
                    Assert(!model.HasAcceptanceError && draft.MaxElapsedMilliseconds == 750 && time.Text == "750", menu + ": valid correction recovers");
                    if (!light && language == OpenVisionLanguage.Korean && menu == VISION_MENU.Blob)
                    {
                        minimum.SetCurrentValue(TextBox.TextProperty, "30");
                        minimum.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
                        maximum.SetCurrentValue(TextBox.TextProperty, "40");
                        maximum.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
                        pump(3);
                        Assert(!model.HasAcceptanceError && draft.AcceptanceMetricMinimum == 30 && draft.AcceptanceMetricMaximum == 40,
                            "Acceptance: correcting the second bound commits both pending values");
                        minimum.SetCurrentValue(TextBox.TextProperty, "13");
                        minimum.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
                        maximum.SetCurrentValue(TextBox.TextProperty, "21");
                        maximum.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
                        pump(2);
                        Assert(enabled.Focus() && enabled.IsKeyboardFocused && ((Border)enabled.Template.FindName("switchFocus", enabled)).Visibility == Visibility.Visible,
                            "Acceptance: rendered switch has a keyboard focus cue");
                        capture("dark-ko-Blob-toggle-focus");
                        MouseButtonEventArgs openMetric = new(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
                        { RoutedEvent = UIElement.PreviewMouseLeftButtonDownEvent };
                        metrics.RaiseEvent(openMetric);
                        pump(3);
                        Popup popup = (Popup)metrics.Template.FindName("PART_Popup", metrics);
                        Assert(openMetric.Handled && metrics.IsDropDownOpen && popup.IsOpen && popup.Child is FrameworkElement,
                            "Acceptance: existing field-click behavior opens the metric popup");
                        FrameworkElement popupContent = (FrameworkElement)popup.Child;
                        Assert(Program.FindVisualChildren<ComboBoxItem>(popupContent).Count() == model.AcceptanceMetrics.Count,
                            "Acceptance: the opened popup contains each produced metric");
                        ScreenshotPngWriter.WriteElementPng(popupContent, Path.ChangeExtension(outputPath, ".dark-ko-Blob-metric-popup-isolated.png"),
                            (int)Math.Ceiling(popupContent.ActualWidth), (int)Math.Ceiling(popupContent.ActualHeight));
                        metrics.SetCurrentValue(ComboBox.IsDropDownOpenProperty, false);
                        pump(2);
                    }
                    surface.BringIntoView();
                    pump(2);
                    Rect bounds = surface.TransformToAncestor(shell).TransformBounds(new Rect(surface.RenderSize));
                    Assert(bounds.Width > 0 && bounds.Height >= 100 && bounds.Right <= shell.ActualWidth + 1 && bounds.Bottom <= shell.ActualHeight + 1,
                        menu + ": acceptance stays inside the native shell: " + bounds);
                    foreach (FrameworkElement field in new FrameworkElement[] { enabled, metrics, useMinimum, useMaximum, minimum, maximum, time })
                    {
                        Rect fieldBounds = field.TransformToAncestor(surface).TransformBounds(new Rect(field.RenderSize));
                        Assert(fieldBounds.Width > 0 && fieldBounds.Left >= -1 && fieldBounds.Right <= surface.ActualWidth + 1,
                            menu + ": acceptance control fits: " + AutomationProperties.GetAutomationId(field));
                    }
                    System.Windows.Shapes.Path arrow = (System.Windows.Shapes.Path)metrics.Template.FindName("comboDropGlyph", metrics);
                    string rendering = $"{(light ? "Light" : "Dark")}/{language}/{menu}: selected={metrics.SelectedValue}; item={metrics.SelectedItem}; expected={model.SelectedAcceptanceMetric}; draft={draft.AcceptanceMetricName}; text={metrics.Foreground}; background={metrics.Background}; arrow={arrow?.Fill}; arrow-size={arrow?.ActualWidth}x{arrow?.ActualHeight}";
                    File.AppendAllLines(Path.ChangeExtension(outputPath, ".acceptance-rendering.txt"), new[] { rendering });
                    Assert(metrics.SelectedValue is string selected && selected == draft.AcceptanceMetricName
                        && metrics.SelectedItem is KeyValuePair<string, string> item && item.Value == model.AcceptanceMetrics.Single(metric => metric.Key == selected).Value,
                        menu + ": metric remains visibly selected after edits and language changes: " + rendering);
                    Assert(arrow?.Fill is SolidColorBrush arrowBrush && arrowBrush.Color.A == 255 && arrow.ActualWidth > 0 && arrow.ActualHeight > 0
                        && Program.GetContrastRatio(arrowBrush.Color, ((SolidColorBrush)metrics.Background).Color) >= 3,
                        menu + ": rendered metric arrow contrast: " + rendering);
                    Assert(((SolidColorBrush)time.Foreground).Color == ((SolidColorBrush)palette["OpenVision.Workbench.TextBrush"]).Color
                        && ((SolidColorBrush)time.Background).Color == ((SolidColorBrush)palette["OpenVision.Workbench.PanelAltBrush"]).Color,
                        menu + ": the focused acceptance surface uses the selected theme");
                    Assert(Program.GetContrastRatio(((SolidColorBrush)time.Foreground).Color, ((SolidColorBrush)time.Background).Color) >= 4.5,
                        menu + ": acceptance numeric field contrast");
                    Assert(ReferenceEquals(property, GetSelectedProperty(document.View)) && ParameterState(document) == parameterSnapshot,
                        menu + ": acceptance must not replace or edit native parameter forms");
                    Assert(document.PreviewRunCount == runs && host.LayerDocumentCount == layers && pipeline.StepCount == 0,
                        menu + ": acceptance editing does not execute, create layers or add a step");
                    AssertFiles(files, recipeDirectory, menu + ": acceptance does not write disk");
                    if (menu is VISION_MENU.Blob or VISION_MENU.Mean or VISION_MENU.Line or VISION_MENU.HSV)
                    {
                        if (!light && language == OpenVisionLanguage.Korean || light && language == OpenVisionLanguage.English)
                            capture((light ? "light" : "dark") + "-" + language + "-" + menu + "-acceptance");
                    }
                    checks.Add($"{(light ? "Light" : "Dark")}/{language}/{menu}: fields/binding/metrics/off retention/invalid guard/Escape/recovery/form identity/bounds/no run-write PASS");
                    surface.Resources.MergedDictionaries.Remove(palette);
                }
            }
        }
        host.SelectToolForTest(VISION_MENU.Mean);
        pump(6);
        OpenVisionWorkbenchToolViewModel tool = host.Workbench.Tool;
        tool.CurrentStep!.ExpectedSuccess = true;
        tool.CurrentStep.RequiredMessageText = string.Empty;
        tool.CurrentStep.AcceptanceMetricName = "MeanValueAvg";
        tool.CurrentStep.Parameters["USE_ROI"] = "false";
        tool.NotifyExecutionState(GetGeneration(tool));
        VisionPipelineStep beforeAdd = VisionPipelineStepEditService.CopyStep(tool.CurrentStep)!;
        tool.AddAfterCommand.Execute(null);
        pump(6);
        Assert(tool.IsStepEditing && pipeline.StepCount == 1 && pipeline.GetStep(0)!.AcceptanceMetricMinimum == beforeAdd.AcceptanceMetricMinimum
            && pipeline.GetStep(0)!.AcceptanceMetricMaximum == beforeAdd.AcceptanceMetricMaximum && pipeline.GetStep(0)!.MaxElapsedMilliseconds == 750
            && pipeline.GetStep(0)!.Parameters["Smoke.AcceptanceOpaque"] == "retained", "Acceptance: add copies criteria and opaque parameters");
        OpenVisionNativeToolDocument mean = host.ActiveNativeDocumentForTest!;
        string inputPath = Path.ChangeExtension(outputPath, ".acceptance-input.png");
        using (System.Drawing.Bitmap input = new(32, 32))
        {
            using System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(input);
            graphics.Clear(System.Drawing.Color.FromArgb(128, 128, 128));
            input.Save(inputPath, System.Drawing.Imaging.ImageFormat.Png);
        }
        Assert(mean.LoadPreviewImageFromFileForTest(VisionToolPreviewImageRole.InputA, inputPath), "Acceptance: prepare owned input for run gate");
        pump(5);
        Assert(pipeline.ValidationSucceeded && pipeline.RunCommand.CanExecute(null), "Acceptance: valid starting pipeline can run");
        tool.AcceptanceTimeLimitText = "NaN";
        pump(3);
        Assert(tool.HasAcceptanceError && !pipeline.RunCommand.CanExecute(null) && !pipeline.RunToSelectedCommand.CanExecute(null)
            && !pipeline.TrySave(), "Acceptance: uncommitted step input blocks pipeline run/prefix/save");
        AssertFiles(files, recipeDirectory, "Acceptance: invalid save does not write");
        tool.CancelAcceptanceEditCommand.Execute(nameof(tool.AcceptanceTimeLimitText));
        pump(3);
        Assert(!tool.HasAcceptanceError && pipeline.RunCommand.CanExecute(null) && pipeline.TrySave(), "Acceptance: Escape allows explicit valid save");
        VisionPipelineStep saved = pipeline.GetStep(0)!;
        tool.AcceptanceMinimumText = "18";
        pump(3);
        Assert(pipeline.GetStep(0)!.AcceptanceMetricMinimum == 18 && tool.IsChanged, "Acceptance: stage edit immediately changes working copy");
        pipeline.UndoCommand.Execute(null);
        pump(4);
        Assert(pipeline.GetStep(0)!.AcceptanceMetricMinimum == saved.AcceptanceMetricMinimum && tool.AcceptanceMinimumText == "13", "Acceptance: Undo restores rendered criterion");
        pipeline.RedoCommand.Execute(null);
        pump(4);
        Assert(tool.AcceptanceMinimumText == "18", "Acceptance: Redo restores rendered criterion");
        tool.RevertCommand.Execute(null);
        pump(4);
        Assert(StepXml(pipeline.GetStep(0)) == StepXml(saved) && tool.AcceptanceMinimumText == "13", "Acceptance: saved step revert restores whole criteria");
        tool.NewTestCommand.Execute(null);
        pump(4);
        Assert(tool.IsNewTest && tool.CurrentStep!.AcceptanceMetricName == saved.AcceptanceMetricName
            && tool.CurrentStep.AcceptanceMetricMinimum == saved.AcceptanceMetricMinimum && tool.CurrentStep.MaxElapsedMilliseconds == saved.MaxElapsedMilliseconds,
            "Acceptance: same-tool test preserves saved criteria");
        capture("step-undo-revert-new-test");
        checks.Add("Step: add retention; valid pipeline gate; invalid input blocks run/prefix/save; Escape; explicit save; edit/Undo/Redo/saved revert; same-tool test PASS");
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".acceptance-checks.txt"), checks);
    }

    internal static void RunRouteCandidates(OpenVisionShellHostView host, string outputPath, Action<int> pump, Action<string> capture)
    {
        using System.Drawing.Bitmap image = new(16, 16);
        using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(image)) { graphics.Clear(System.Drawing.Color.Gray); }
        host.SetMainLayerImageForTest(image);
        Assert(host.AddLayerImageForTest("Operator_Input", image), "Candidate fixture must open an independent input image");
        Assert(host.AddLayerImageForTest("Candidate_Future", image), "Owned opened future output fixture");
        Assert(host.AddLayerImageForTest("Candidate_Disabled", image), "Owned opened disabled output fixture");
        Assert(host.AddLayerImageForTest("Candidate_First", image), "Owned already generated earlier output fixture");
        host.ActivateHostLayerForTest("Main");
        pump(8);
        OpenVisionWorkbenchPipelineViewModel pipeline = host.Workbench.Pipeline;
        string recipeDirectory = RecipeWorkspaceService.GetRecipeWorkspaceDirectory(host.ActiveRecipeContextNameForTest);
        Dictionary<string, string> files = SnapshotFiles(recipeDirectory);
        int layers = host.LayerDocumentCount;
        List<string> checks = new();
        foreach (OpenVisionLanguage language in new[] { OpenVisionLanguage.Korean, OpenVisionLanguage.English })
        {
            OpenVisionLanguageService.SetLanguage(language, false);
            (language == OpenVisionLanguage.Korean ? host.Workbench.SelectDarkThemeCommand : host.Workbench.SelectLightThemeCommand).Execute(null);
            pipeline.SelectStepCommand.Execute(2);
            pump(10);
            OpenVisionNativeToolDocument document = host.ActiveNativeDocumentForTest!;
            OpenVisionWorkbenchToolViewModel model = host.Workbench.Tool;
            FrameworkElement shell = (FrameworkElement)document.View.FindName("toolShell");
            ComboBox input = (ComboBox)shell.FindName("cbInputLayer");
            int runs = document.PreviewRunCount;
            string parameters = ParameterState(document);
            capture(language + "-step-initial");
            AssertCandidates(input, new[] { "Main", "Candidate_First", "Operator_Input" }, language + "/step");
            AssertChoice(input, "Main", language == OpenVisionLanguage.Korean ? "원본" : "Original");
            AssertChoice(input, "Candidate_First", "01 · Candidate_First");
            AssertRenderedChoice(input, "01 · Candidate_First");
            Assert(model.SetRoute(GetGeneration(model), "Candidate_Future", model.InputLayerB, model.OutputLayer), "Existing raw unavailable reference retained"); pump(6);
            File.AppendAllText(Path.ChangeExtension(outputPath, ".candidate-projection.jsonl"), System.Text.Json.JsonSerializer.Serialize(new { Case = "raw retained before normal adapter refresh", Raw = model.InputLayer, DocumentRaw = document.RouteInputLayerName, Items = input.Items.Cast<KeyValuePair<string, string>>().ToArray() }) + Environment.NewLine);
            document.RefreshLayerState(); pump(6);
            File.AppendAllText(Path.ChangeExtension(outputPath, ".candidate-projection.jsonl"), System.Text.Json.JsonSerializer.Serialize(new { Case = "raw retained after normal adapter refresh", Raw = model.InputLayer, DocumentRaw = document.RouteInputLayerName, Items = input.Items.Cast<KeyValuePair<string, string>>().ToArray() }) + Environment.NewLine);
            AssertChoice(input, "Candidate_Future", OpenVisionLanguageService.T("Workbench.Tool.Unavailable") + " Candidate_Future");
            Assert(model.InputLayer == "Candidate_Future", "Unavailable projection must preserve raw input");
            SelectInput(input, "Candidate_First");
            CaptureInputPopup(input, "step-popup");
            SelectInput(input, "Main");
            SelectInput(input, "Operator_Input");
            Assert(model.InputLayer == "Operator_Input" && document.RouteInputLayerName == "Operator_Input", "Step selection must write the raw workspace key");
            SelectInput(input, "Candidate_First");
            pipeline.ToggleRowEnabledCommand.Execute(0);
            pipeline.SelectStepCommand.Execute(2);
            pump(8);
            string unavailable = (language == OpenVisionLanguage.Korean ? "(사용할 수 없음)" : "(unavailable)") + " Candidate_First";
            Assert(model.InputLayer == "Candidate_First" && document.RouteInputLayerName == "Candidate_First", "Disabling a producer must retain its current reference");
            AssertChoice(input, "Candidate_First", unavailable);
            AssertRenderedChoice(input, unavailable);
            Assert(!pipeline.RunCommand.CanExecute(null), "Unavailable input must retain the existing execution gate");
            capture(language + "-step-unavailable");
            pipeline.UndoCommand.Execute(null);
            pump(8);
            AssertChoice(input, "Candidate_First", "01 · Candidate_First");
            AssertRenderedChoice(input, "01 · Candidate_First");
            Assert(parameters == ParameterState(document), "Candidate refresh must preserve the existing parameter form and values");
            Assert(document.PreviewRunCount == runs, "Step selection/producer toggle must not run the tool");

            foreach (VISION_MENU menu in new[] { VISION_MENU.Blob, VISION_MENU.Arithmetic })
            {
                host.SelectToolForTest(menu);
                pump(10);
                document = host.ActiveNativeDocumentForTest!;
                shell = (FrameworkElement)document.View.FindName("toolShell");
                if (document.View is ArithmeticToolWpfView arithmetic)
                {
                    ArithmeticToolSettings settings = arithmetic.CaptureSettings();
                    settings.UseOffsetMode = false;
                    settings.UseConstantInput = false;
                    arithmetic.ApplyPersistedSettings(settings);
                    document.RecordParameterChange(null);
                    System.Threading.Thread.Sleep(180);
                    pump(15);
                }
                input = (ComboBox)shell.FindName(menu == VISION_MENU.Arithmetic ? "cbInputA" : "cbInputLayer");
                ComboBox? inputB = shell.FindName("cbInputB") as ComboBox;
                runs = document.PreviewRunCount;
                parameters = ParameterState(document);
                string pipelineBefore = StepXml(pipeline.WorkingCopyForTest);
                string expectedInput = language == OpenVisionLanguage.Korean ? "Candidate_Current" : "Operator_Input";
                Assert(model.IsNewTest && model.InputLayer == expectedInput, menu + ": test input must be captured once and retained when reopened");
                AssertChoice(input, "Candidate_Current", "03 · Candidate_Current");
                AssertChoice(input, "Candidate_First", "01 · Candidate_First");
                Assert(!input.Items.Cast<KeyValuePair<string, string>>().Any(item => item.Key is "Candidate_Disabled" or "Candidate_Future"), menu + ": disabled and future outputs must be absent");
                CaptureInputPopup(input, menu + "-popup");
                SelectInput(input, "Main");
                SelectInput(input, "Operator_Input");
                Assert(model.InputLayer == "Operator_Input" && document.RouteInputLayerName == "Operator_Input", menu + ": test must retain raw input A");
                if (inputB != null)
                {
                    Assert(inputB.IsVisible, "Arithmetic image-operation fixture must show input B");
                    string inputA = model.InputLayer;
                    SelectInput(inputB, "Main");
                    Assert(model.InputLayer == inputA && model.InputLayerB == "Main", "B Main changes only B");
                    Assert(model.SetRoute(GetGeneration(model), inputA, string.Empty, model.OutputLayer), "existing owner accepts empty B"); pump(4);
                    AssertRouteAction(host, outputPath, language == OpenVisionLanguage.Korean ? "입력: " : "Input: ", "empty B owner call");
                    Assert(model.InputLayer == inputA && model.InputLayerB == string.Empty && document.RouteInputLayerBName == string.Empty, "empty B preserves A and raw empty");
                    SelectInput(inputB, "Candidate_First");
                    Assert(model.InputLayerB == "Candidate_First" && document.RouteInputLayerBName == "Candidate_First", "Input B must retain its own raw key");
                    AssertRenderedChoice(inputB, "01 · Candidate_First");
                    CaptureInputPopup(inputB, "Arithmetic-B-popup");
                }
                pipeline.ToggleRowEnabledCommand.Execute(0);
                pipeline.SelectStepCommand.Execute(2);
                host.SelectToolForTest(menu);
                pump(8);
                Assert(!input.Items.Cast<KeyValuePair<string, string>>().Any(item => item.Key == "Candidate_First") || inputB != null,
                    menu + ": test refresh must drop a disabled producer unless it is the current B");
                if (inputB != null)
                {
                    AssertChoice(inputB, "Candidate_First", unavailable);
                    AssertRenderedChoice(inputB, unavailable);
                }
                Assert(model.InputLayer == "Operator_Input", menu + ": producer changes must not reset the captured test input");
                pipeline.UndoCommand.Execute(null);
                pipeline.SelectStepCommand.Execute(2);
                host.SelectToolForTest(menu);
                pump(8);
                AssertChoice(input, "Candidate_First", "01 · Candidate_First");
                Assert(parameters == ParameterState(document), menu + ": route selection must retain tool parameters");
                FrameworkElement originalView = document.View;
                Assert(host.FloatDockedWpfToolWindowForTest(), menu + ": float must succeed");
                pump(8);
                Assert(ReferenceEquals(originalView, document.View) && model.InputLayer == "Operator_Input", menu + ": float must retain view and raw route");
                AssertRenderedChoice(input, "Operator_Input");
                Assert(host.DockActiveWpfToolWindowForTest(), menu + ": dock must succeed");
                pump(8);
                AssertRenderedChoice(input, "Operator_Input");
                capture(language + "-" + menu + "-selected");
                Assert(document.PreviewRunCount == runs && host.ActiveHostLayerTitle == "Main" && host.LayerDocumentCount == layers && pipeline.StepCount == 4,
                    menu + ": candidate projection/selection/float must not execute or change workspace layers");
                Assert(StepXml(pipeline.WorkingCopyForTest) == pipelineBefore, menu + ": test input selection must not change working pipeline");
                AssertFiles(files, recipeDirectory, menu + ": input selection must not save");
                if (menu == VISION_MENU.Arithmetic && language == OpenVisionLanguage.English)
                {
                    string a = model.InputLayer, bInput = model.InputLayerB, o = model.OutputLayer;
                    AssertRouteNoticeCount(host, outputPath, pump, "combined output > B > A", () => Assert(model.SetRoute(GetGeneration(model), "Main", string.Empty, "Combined_Owned"), "combined accepted"), "Output: Combined_Owned");
                    AssertRouteNoticeCount(host, outputPath, pump, "combined B > A", () => Assert(model.SetRoute(GetGeneration(model), "Operator_Input", "Candidate_First", model.OutputLayer), "combined inputs accepted"), "Input: Candidate_First");
                    AssertRouteNoticeCount(host, outputPath, pump, "A only", () => Assert(model.SetRoute(GetGeneration(model), "Main", model.InputLayerB, model.OutputLayer), "A accepted"), "Input: Original");
                    AssertRouteNoticeCount(host, outputPath, pump, "combined restore raw routes", () => Assert(model.SetRoute(GetGeneration(model), a, bInput, o), "raw routes restored"), "Output: " + o);
                    Assert(model.InputLayer == a && model.InputLayerB == bInput && model.OutputLayer == o && document.PreviewRunCount == runs, "combined owner calls restore routes without Run");
                    AssertFiles(files, recipeDirectory, "combined route owner must not save");
                }
                checks.Add(language + "/" + menu + ": raw A/B, popup labels, missing producer/recovery, test anchor, dock/float, no execution/layer/save PASS");
            }
        }
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".checks.txt"), checks);

        void SelectInput(ComboBox combo, string key)
        {
            string previous = VisionToolLayerComboHelper.GetLayerText(combo);
            combo.SelectedItem = combo.Items.Cast<KeyValuePair<string, string>>().Single(item => item.Key == key);
            pump(6);
            Assert(VisionToolLayerComboHelper.GetLayerText(combo) == key, "Input combo must retain raw key " + key);
            if (previous != key) AssertRouteAction(host, outputPath, (OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean ? "입력: " : "Input: ") + (key == "Main" ? languageText() : key), "raw input " + key);
            if (key == "Main" && previous != key)
            {
                OpenVisionLanguage initial = OpenVisionLanguageService.CurrentLanguage, other = initial == OpenVisionLanguage.Korean ? OpenVisionLanguage.English : OpenVisionLanguage.Korean;
                AssertNoRouteNotice(host, outputPath, pump, "locale is not fresh route", () => OpenVisionLanguageService.SetLanguage(other, false), false);
                AssertRouteAction(host, outputPath, other == OpenVisionLanguage.Korean ? "입력: 원본" : "Input: Original", "Main alias dynamic locale");
                OpenVisionLanguageService.SetLanguage(initial, false); pump(3);
                capture(initial + "-route-action-" + combo.Name + "-Main");
            }

        }
        void CaptureInputPopup(ComboBox combo, string suffix)
        {
            combo.IsDropDownOpen = true;
            pump(6);
            Popup popup = (Popup)combo.Template.FindName("PART_Popup", combo);
            string[] labels = Program.FindVisualChildren<TextBlock>(popup.Child).Where(text => text.IsVisible).Select(text => text.Text).ToArray();
            Assert(labels.Contains(languageText()) && labels.All(label => !label.StartsWith("[Main,")), "Popup must render localized display values");
            File.AppendAllLines(Path.ChangeExtension(outputPath, ".popups.txt"), new[] { suffix + ": " + string.Join(" | ", labels) });
            capture(OpenVisionLanguageService.CurrentLanguage + "-" + suffix);
            combo.IsDropDownOpen = false;
            pump(3);
        }
        string languageText() => OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean ? "원본" : "Original";
    }

    private static void AssertRouteAction(OpenVisionShellHostView host, string outputPath, string expected, string label)
    {
        TextBlock status = Program.FindVisualChildren<TextBlock>(host).Single(t => AutomationProperties.GetAutomationId(t) == "WorkbenchActionStatus");
        var icon = Program.FindVisualChildren<MahApps.Metro.IconPacks.PackIconMaterial>(host).Single(i => AutomationProperties.GetAutomationId(i) == "WorkbenchActionIcon");
        bool pass = status.Text == expected && host.Workbench.MainActionText == expected && host.Workbench.MainActionKind == string.Empty && icon.Kind.ToString() == "InformationOutline" && (string)status.ToolTip == expected;
        File.AppendAllText(Path.ChangeExtension(outputPath, ".route-action.jsonl"), System.Text.Json.JsonSerializer.Serialize(new { Case = label, Expected = expected, Actual = status.Text, Tooltip = status.ToolTip, Kind = host.Workbench.MainActionKind, Icon = icon.Kind.ToString(), Theme = host.Workbench.ThemeName, Locale = OpenVisionLanguageService.CurrentLanguage.ToString(), PASS = pass }) + Environment.NewLine);
        Assert(pass, label + ": bound W09 route notice must be " + expected + "; actual=" + status.Text);
    }

    private static void AssertRouteNoticeCount(OpenVisionShellHostView host, string outputPath, Action<int> pump, string label, Action action, string expected)
    {
        int notices = 0;
        System.ComponentModel.PropertyChangedEventHandler listener = (_, e) => { if (e.PropertyName == "RouteActionKey") notices++; };
        host.Workbench.Tool.PropertyChanged += listener;
        try { action(); pump(4); }
        finally { host.Workbench.Tool.PropertyChanged -= listener; }
        File.AppendAllText(Path.ChangeExtension(outputPath, ".route-action.jsonl"), System.Text.Json.JsonSerializer.Serialize(new { Case = label, DirectOwnerCall = true, Notices = notices, Expected = expected, PASS = notices == 1 }) + Environment.NewLine);
        Assert(notices == 1, label + ": exactly one successful notice");
        AssertRouteAction(host, outputPath, expected, label);
    }

    private static void AssertNoRouteNotice(OpenVisionShellHostView host, string outputPath, Action<int> pump, string label, Action action, bool preserveText = true)
    {
        int notices = 0; string before = host.Workbench.MainActionText;
        System.ComponentModel.PropertyChangedEventHandler listener = (_, e) => { if (e.PropertyName == "RouteActionKey") notices++; };
        host.Workbench.Tool.PropertyChanged += listener;
        try { action(); pump(4); }
        finally { host.Workbench.Tool.PropertyChanged -= listener; }
        bool pass = notices == 0 && (!preserveText || host.Workbench.MainActionText == before);
        File.AppendAllText(Path.ChangeExtension(outputPath, ".route-action.jsonl"), System.Text.Json.JsonSerializer.Serialize(new { Case = label, Notices = notices, Before = before, After = host.Workbench.MainActionText, PreserveText = preserveText, PASS = pass }) + Environment.NewLine);
        Assert(pass, label + ": must not emit a fresh route notice");
    }

    private static void AssertCandidates(ComboBox input, string[] expected, string reason) => Assert(
        input.Items.Cast<KeyValuePair<string, string>>().Select(item => item.Key).SequenceEqual(expected), reason + ": unexpected input candidate keys");
    private static void AssertChoice(ComboBox input, string key, string label) => Assert(
        input.Items.Cast<KeyValuePair<string, string>>().Single(item => item.Key == key).Value == label, "Candidate label must be " + label);
    private static void AssertRenderedChoice(ComboBox input, string label) => Assert(
        Program.FindVisualChildren<TextBlock>(input).Any(text => text.IsVisible && text.Text == label), "Selected input must render " + label);

    internal static void RunRouteEditing(OpenVisionShellHostView host, string outputPath, Action<int> pump, Action<string> capture)
    {
        OpenVisionWorkbenchPipelineViewModel pipeline = host.Workbench.Pipeline;
        string recipeDirectory = RecipeWorkspaceService.GetRecipeWorkspaceDirectory(host.ActiveRecipeContextNameForTest);
        Dictionary<string, string> files = SnapshotFiles(recipeDirectory);
        int layers = host.LayerDocumentCount;
        List<string> checks = new();
        foreach (OpenVisionLanguage language in new[] { OpenVisionLanguage.Korean, OpenVisionLanguage.English })
        {
            OpenVisionLanguageService.SetLanguage(language, false);
            (language == OpenVisionLanguage.Korean ? host.Workbench.SelectDarkThemeCommand : host.Workbench.SelectLightThemeCommand).Execute(null);
            foreach (VISION_MENU menu in new[] { VISION_MENU.Blob, VISION_MENU.Arithmetic, VISION_MENU.Threshold, VISION_MENU.Mean })
            {
                host.SelectToolForTest(menu);
                pump(10);
                OpenVisionNativeToolDocument document = host.ActiveNativeDocumentForTest!;
                OpenVisionWorkbenchToolViewModel model = host.Workbench.Tool;
                FrameworkElement shell = (FrameworkElement)document.View.FindName("toolShell");
                ComboBox output = shell is VisionToolSingleInputPropertyToolShell single
                    ? single.OutputLayerComboBox : ((VisionToolDoubleInputCustomToolShell)shell).OutputLayerComboBox;
                output.ApplyTemplate();
                TextBox edit = (TextBox)output.Template.FindName("PART_EditableTextBox", output);
                Assert(edit != null && edit.IsVisible && output.IsEditable, menu + ": actual editable route field must exist");
                int runs = document.PreviewRunCount;
                if (document.View is ArithmeticToolWpfView arithmetic)
                {
                    VisionToolDoubleInputCustomToolShell pair = (VisionToolDoubleInputCustomToolShell)shell;
                    bool previousAutoRun = host.Workbench.IsAutoRunEnabled;
                    host.Workbench.IsAutoRunEnabled = false;
                    Assert(model.SetRoute(GetGeneration(model), model.InputLayer, "Main", model.OutputLayer), "Arithmetic B fixture raw route accepted");
                    document.RefreshLayerState();
                    foreach (string mode in new[] { "Image", "Constant", "Image", "Offset", "Operation" })
                    {
                        ((RadioButton)arithmetic.FindName(mode == "Constant" ? "rdoContrast" : mode == "Offset" ? "rdoModeOffset" : mode == "Operation" ? "rdoModeOperation" : "rdoSourceImage")).IsChecked = true;
                        pump(8);
                        bool imageB = !arithmetic.UseOffsetMode && !arithmetic.UseConstantInput;
                        string header = OpenVisionLanguageService.T(imageB ? "Workbench.Tool.InputA" : "Workbench.Tool.Input");
                        string diagnostic = $"{language}/{mode}: offset={arithmetic.UseOffsetMode}; constant={arithmetic.UseConstantInput}; visible={pair.InputBGroup.IsVisible}; local={pair.InputBGroup.Visibility}; enabled={pair.InputBGroup.IsEnabled}; header={pair.InputAGroup.Header}; rawB={model.InputLayerB}; comboB={VisionToolLayerComboHelper.GetLayerText(pair.InputBComboBox)}";
                        File.AppendAllLines(Path.ChangeExtension(outputPath, ".arithmetic-route-mode.txt"), new[] { diagnostic });
                        capture(language + "-Arithmetic-route-" + mode);
                        Assert(pair.InputBGroup.IsVisible == imageB && (string)pair.InputAGroup.Header == header, "Arithmetic Main route mode must match contract: " + diagnostic);
                        Rect inputBounds = pair.InputAGroup.TransformToAncestor(pair).TransformBounds(new Rect(pair.InputAGroup.RenderSize));
                        Rect outputBounds = pair.OutputLayerGroup.TransformToAncestor(pair).TransformBounds(new Rect(pair.OutputLayerGroup.RenderSize));
                        Assert(imageB ? outputBounds.Top >= inputBounds.Bottom + 3 : Math.Abs(inputBounds.Top - outputBounds.Top) < 1 && inputBounds.Right + 3 <= outputBounds.Left,
                            "Arithmetic Main single Input/Output or A/B then Output geometry: " + diagnostic);
                        Assert(model.InputLayerB == "Main" && VisionToolLayerComboHelper.GetLayerText(pair.InputBComboBox) == "Main", "Hidden B raw route must survive mode round trip: " + diagnostic);
                        Assert(document.PreviewRunCount == runs && host.LayerDocumentCount == layers && pipeline.StepCount == 0, "Arithmetic route mode alone must not execute or create layers/steps");
                        AssertFiles(files, recipeDirectory, "Arithmetic mode must not save");
                    }
                    pair.IsEnabled = false;
                    pump(4);
                    Assert(pair.InputBGroup.IsVisible, "Image B route remains visible while its container is disabled; enabled state is not a mode predicate");
                    pair.ClearValue(UIElement.IsEnabledProperty);
                    VisionToolDebouncedPreviewScheduler.CancelPending(arithmetic);
                    host.Workbench.IsAutoRunEnabled = previousAutoRun;
                }
                string renamed = "RouteEdit_" + language + "_" + menu;
                foreach (string entered in new[] { renamed, "Main", string.Empty })
                {
                    string previous = model.CurrentStep!.OutputLayer;
                    edit.Focus();
                    edit.SelectAll();
                    edit.Text = entered;
                    ((Button)shell.FindName("btnWorkbenchDefaults")).Focus();
                    pump(8);
                    edit = (TextBox)output.Template.FindName("PART_EditableTextBox", output);
                    string expected = entered == renamed ? renamed : previous;
                    string observation = $"{language}/{menu}: entered={entered}; expected={expected}; editor={edit.Text}; combo={output.Text}; selected={output.SelectedItem}; working={model.CurrentStep!.OutputLayer}; document={document.RouteOutputLayerName}; status={model.StatusText}";
                    File.AppendAllLines(Path.ChangeExtension(outputPath, ".route-edit.txt"), new[] { observation });
                    capture(language + "-" + menu + "-" + (entered == renamed ? "valid" : entered == "Main" ? "main" : "empty"));
                    AssertRouteAction(host, outputPath, (language == OpenVisionLanguage.Korean ? "출력: " : "Output: ") + expected, language + "/" + menu + "/" + entered);
                    Assert(model.CurrentStep.OutputLayer == expected && edit.Text == expected && document.RouteOutputLayerName == expected,
                        menu + ": rendered output edit must synchronize or restore: " + observation);
                    Assert(document.PreviewRunCount == runs && host.LayerDocumentCount == layers && pipeline.StepCount == 0,
                        menu + ": route editing must not execute, create layers or insert steps");
                    AssertFiles(files, recipeDirectory, menu + ": route editing must not save");
                }
                string recovered = renamed + "_Recovery";
                edit.Focus();
                edit.Text = "Main";
                edit.Text = recovered;
                ((Button)shell.FindName("btnWorkbenchDefaults")).Focus();
                pump(8);
                edit = (TextBox)output.Template.FindName("PART_EditableTextBox", output);
                Assert(model.CurrentStep!.OutputLayer == recovered && edit.Text == recovered && document.RouteOutputLayerName == recovered,
                    menu + ": delayed rejection must preserve a newer valid edit");
                capture(language + "-" + menu + "-rapid-recovery");
                AssertRouteAction(host, outputPath, (language == OpenVisionLanguage.Korean ? "출력: " : "Output: ") + recovered, "valid recovery");
                long token = GetGeneration(model);
                AssertNoRouteNotice(host, outputPath, pump, "same route", () => Assert(model.SetRoute(token, model.InputLayer, model.InputLayerB, recovered), "same route accepted"));
                AssertNoRouteNotice(host, outputPath, pump, "stale token", () => Assert(!model.SetRoute(token - 1, "WrongInput", string.Empty, "WrongOutput"), "stale token rejected"));
                AssertNoRouteNotice(host, outputPath, pump, "owner Main rejected", () => Assert(!model.SetRoute(token, model.InputLayer, model.InputLayerB, "Main"), "Main rejected"));
                AssertNoRouteNotice(host, outputPath, pump, "owner empty rejected", () => Assert(!model.SetRoute(token, model.InputLayer, model.InputLayerB, string.Empty), "empty rejected"));


                edit.Focus();
                edit.Text = "Main";
                host.SelectToolForTest(menu == VISION_MENU.Blob ? VISION_MENU.Arithmetic : VISION_MENU.Blob);
                pump(8);
                OpenVisionNativeToolDocument other = host.ActiveNativeDocumentForTest!;
                Assert(host.Workbench.Tool.CurrentStep!.OutputLayer != "Main" && other.RouteOutputLayerName == host.Workbench.Tool.CurrentStep.OutputLayer,
                    menu + ": stale rejection callback must not change another tool session");
                host.SelectToolForTest(menu);
                pump(8);
                edit = (TextBox)output.Template.FindName("PART_EditableTextBox", output);
                Assert(model.CurrentStep!.OutputLayer == recovered && edit.Text == recovered && document.RouteOutputLayerName == recovered,
                    menu + ": reselect after pending rejection restores the same route");
                Assert(document.PreviewRunCount == runs && host.LayerDocumentCount == layers && pipeline.StepCount == 0,
                    menu + ": recovery and switching must not execute, create layers or insert steps");
                AssertFiles(files, recipeDirectory, menu + ": recovery and switching must not save");
                capture(language + "-" + menu + "-switch-restored");
                AssertNoRouteNotice(host, outputPath, pump, "restore defaults not route", () => model.RestoreDefaultsCommand.Execute(null), false);
                AssertNoRouteNotice(host, outputPath, pump, "reselect not route", () => host.SelectToolForTest(menu), false);
                checks.Add(language + "/" + menu + ": output editing/synchronization/invalid rollback/rapid recovery/session switch/no side effects PASS");
            }
        }
        ArithmeticToolWpfView legacy = new();
        OpenVisionFloatingToolWindow legacyWindow = new("Legacy Arithmetic", legacy) { Width = 1280, Height = 900, Owner = Window.GetWindow(host), ShowInTaskbar = false };
        int legacyRuns = 0;
        legacy.RunPreviewRequested += (_, _) => legacyRuns++;
        legacy.RunOffsetRequested += (_, _) => legacyRuns++;
        try
        {
            legacyWindow.Show();
            legacy.SetOperationList(new[] { "Bitwise_AND", "ABS" }, "Bitwise_AND");
            legacy.SetLayerList(new[] { "Main", "Legacy_B", "Legacy_Output" }, "Main", "Legacy_B", "Legacy_Output");
            VisionToolDoubleInputCustomToolShell legacyShell = (VisionToolDoubleInputCustomToolShell)legacy.FindName("toolShell");
            foreach (string mode in new[] { "Image", "Constant", "Offset", "Unary", "Image" })
            {
                legacy.ApplyPersistedSettings(new ArithmeticToolSettings { SelectedOperation = mode == "Unary" ? "ABS" : "Bitwise_AND", UseConstantInput = mode == "Constant", UseOffsetMode = mode == "Offset" });
                pump(8);
                bool previewB = mode == "Image";
                Assert(legacyShell.InputBGroup.IsVisible == previewB && !legacyShell.IsDockedInspectorMode,
                    "Independent Legacy Arithmetic retains preview-card visibility for " + mode);
                Assert(legacyShell.InputAPreviewFrame.IsVisible && legacyShell.OutputPreviewFrame.IsVisible && legacyShell.CreateOutputLayerButton.IsVisible,
                    "Independent Legacy preview/output creation controls remain visible for " + mode);
                Assert(VisionToolLayerComboHelper.GetLayerText(legacyShell.InputBComboBox) == "Legacy_B" && legacyRuns == 0, "Legacy mode changes preserve B and do not request execution");
                if (mode is "Image" or "Constant" or "Offset")
                {
                    System.Windows.Media.Imaging.RenderTargetBitmap image = new((int)legacyWindow.ActualWidth, (int)legacyWindow.ActualHeight, 96D, 96D, PixelFormats.Pbgra32);
                    image.Render(legacyWindow);
                    System.Windows.Media.Imaging.PngBitmapEncoder encoder = new();
                    encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(image));
                    using FileStream frame = File.Create(Path.ChangeExtension(outputPath, ".Legacy-" + mode + ".png"));
                    encoder.Save(frame);
                }
            }
            checks.Add("Independent Legacy Arithmetic: Image/Constant/Offset/Unary/Image preview-card and raw B/no execution PASS; in-process display only");
        }
        finally { legacyWindow.Close(); legacy.DisposeView(); }
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".checks.txt"), checks);
    }

    internal static void RunTemplates(OpenVisionShellHostView host, string outputPath, string templatePath, Action<int> pump, Action<string> capture)
    {
        OpenVisionWorkbenchToolViewModel model = host.Workbench.Tool;
        OpenVisionWorkbenchPipelineViewModel pipeline = host.Workbench.Pipeline;
        string recipeDirectory = RecipeWorkspaceService.GetRecipeWorkspaceDirectory(host.ActiveRecipeContextNameForTest);
        Dictionary<string, string> xmlFiles = SnapshotFiles(recipeDirectory).Where(item => Path.GetExtension(item.Key).Equals(".xml", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(item => item.Key, item => item.Value, StringComparer.OrdinalIgnoreCase);
        PropertyGridEditorRuntime runtime = (PropertyGridEditorRuntime)typeof(PropertyGridEditorFactory)
            .GetField("runtime", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.GetValue(null)!;
        IPropertyGridImageEditorService originalService = runtime.ImageEditorService;
        TemplateEditorProbe editorProbe = new(originalService, templatePath);
        List<string> checks = new();
        string corruptPath = Path.ChangeExtension(outputPath, ".invalid-template.png");
        File.WriteAllText(corruptPath, "Owned template file with no image bytes.");
        string longPath = Path.ChangeExtension(outputPath, ".template_with_a_long_but_plausible_original_file_name_012345678901234567890123456789.png");
        File.Copy(templatePath, longPath, true);
        PropertyGridEditorFactory.SetImageEditorService(editorProbe);
        host.Workbench.IsAutoRunEnabled = true;
        try
        {
            foreach (OpenVisionLanguage language in new[] { OpenVisionLanguage.Korean, OpenVisionLanguage.English })
            {
                OpenVisionLanguageService.SetLanguage(language, false);
                foreach (VISION_MENU menu in new[] { VISION_MENU.Matching, VISION_MENU.EdgeBasedMatching, VISION_MENU.FeatureMatching })
                {
                    host.SelectToolForTest(menu);
                    pump(8);
                    OpenVisionNativeToolDocument document = host.ActiveNativeDocumentForTest!;
                    model.RestoreDefaultsCommand.Execute(null);
                    pump(4);
                    Assert(document.LoadPreviewImageFromFileForTest(VisionToolPreviewImageRole.InputA, templatePath), menu + ": owned source loaded");
                    VisionToolSingleInputPropertyToolShell shell = (VisionToolSingleInputPropertyToolShell)document.View.FindName("toolShell");
                    object property = GetSelectedProperty(document.View)!;
                    switch (property)
                    {
                        case MatchingProperty value: value.SCORE_MIN = .81D; value.NUM_MATCH = 4; value.USE_FIND_ANGLE = true; value.FIND_ANGLE_MIN = -11; value.FIND_ANGLE_MAX = 17; break;
                        case EdgeBasedMatchingProperty value: value.SCORE_MIN = .82D; value.NUM_MATCH = 3; value.CANNY_LOW = 41; value.CANNY_HIGH = 113; break;
                        case FeatureMatchingProperty value: value.SCORE_MIN = .83D; value.RANSAC_REPROJ_THRESHOLD = 4.7D; break;
                    }
                    document.RecordParameterChange(null);
                    pump(4);
                    Button draw = Program.FindVisualChildren<Button>(shell.WorkbenchTemplateHost).Single(control => control.Name == "templateDrawButton");
                    Button file = Program.FindVisualChildren<Button>(shell.WorkbenchTemplateHost).Single(control => control.Name == "templateFileButton");
                    Image thumbnail = Program.FindVisualChildren<Image>(shell.WorkbenchTemplateHost).Single(control => control.Name == "templateThumbnail");
                    TextBlock info = Program.FindVisualChildren<TextBlock>(shell.WorkbenchTemplateHost).Single(control => control.Name == "templateInfo");
                    string route = document.RouteInputLayerName + "->" + document.RouteOutputLayerName;
                    int runs = document.PreviewRunCount;
                    int layers = host.LayerDocumentCount;
                    Assert(shell.WorkbenchTemplateHost.IsVisible && model.HasTemplateCard && !model.HasTemplate && info.Text == OpenVisionLanguageService.T("Workbench.Tool.TemplateMissing"), menu + ": empty card bound to current editor");
                    Assert(model.DrawTemplateText == OpenVisionLanguageService.T("Workbench.Tool.TemplateDraw") && draw.IsEnabled && file.IsEnabled,
                        menu + ": empty template uses exact draw/file captions");
                    Assert(((FrameworkElement)shell.FindName("legacyTemplateStatus")).Visibility == Visibility.Collapsed, menu + ": old status strip hidden");
                    Assert(!Program.FindVisualChildren<VisionToolVerificationGuideView>(shell).Any(guide => guide.IsVisible), menu + ": no duplicate old guide");
                    if (menu == VISION_MENU.EdgeBasedMatching)
                        Assert(Program.FindVisualChildren<AutoMPointTeachingPanel>(shell).Single().TitleText.Text == OpenVisionLanguageService.T("Workbench.Tool.AutoMPoint"), menu + ": existing suggestion panel uses exact caption");
                    capture(language + "-" + menu + "-empty");
                    VisionPipelineStep parameterBaseline = VisionPipelineStepEditService.CopyStep(model.CurrentStep!);
                    string parameters = ParametersWithoutTemplate(model.CurrentStep!);
                    string beforeCancel = StepXml(model.CurrentStep);
                    editorProbe.Accepted = false;
                    Schedule(document);
                    Execute(draw);
                    Wait(220);
                    Assert(StepXml(model.CurrentStep) == beforeCancel && editorProbe.LastView!.Disposed && editorProbe.LastSource!.IsDisposed,
                        menu + ": draw cancel preserves all data and releases editor/source");
                    Assert(editorProbe.Mode == "TRAIN" && editorProbe.LastView.LoadedPath == string.Empty, menu + ": existing image editor call path");
                    editorProbe.Accepted = true;
                    editorProbe.Region = new OpenCvSharp.Rect(2, 3, 16, 12);
                    Execute(draw);
                    pump(5);
                    string firstPath = model.CurrentStep!.Parameters["PATTERN_PATH"];
                    Assert(File.Exists(firstPath) && model.HasTemplate && model.DrawTemplateText == OpenVisionLanguageService.T("Workbench.Tool.TemplateRedraw"), menu + ": registered card ready/redraw");
                    Assert(ParametersWithoutTemplate(model.CurrentStep) == parameters && ReferenceEquals(property, GetSelectedProperty(document.View)), menu + ": draw keeps score/angle/Canny/RANSAC and same grid object; differences=" + ParameterDifferences(parameterBaseline, model.CurrentStep)
                        + "; sameObject=" + ReferenceEquals(property, GetSelectedProperty(document.View)));
                    Assert(thumbnail.Source is System.Windows.Media.Imaging.BitmapSource image && image.IsFrozen && image.PixelWidth == 16 && image.PixelHeight == 12,
                        menu + ": thumbnail comes from actual loaded 16x12 template");
                    Border thumbnailBorder = Program.FindVisualChildren<Border>(shell.WorkbenchTemplateHost).Single(border => border.Width == 72D && border.Height == 56D);
                    Assert(thumbnailBorder.ActualWidth == 72D && thumbnailBorder.ActualHeight == 56D && info.Text.Contains("16×12 px · ", StringComparison.Ordinal), menu + ": 72x56 border/metadata");
                    Assert(editorProbe.LastView!.Disposed && editorProbe.LastSource!.IsDisposed && editorProbe.LastSavedRotation == 0D, menu + ": accepted edit source/view disposed and rotation preserved");
                    editorProbe.Accepted = false;
                    Execute(draw);
                    Assert(model.CurrentStep.Parameters["PATTERN_PATH"] == firstPath && editorProbe.LastView!.LoadedPath == firstPath,
                        menu + ": redraw cancel loads previous template and keeps path");
                    document.SetTemplateSelectionForTest(fromFile => { Assert(fromFile, menu + ": file selection entry"); return corruptPath; });
                    Execute(file);
                    pump(3);
                    Assert(model.HasTemplate && model.CurrentStep.Parameters["PATTERN_PATH"] == firstPath && model.StatusText == OpenVisionLanguageService.T("Workbench.Status.Error"), menu + ": unreadable file keeps ready template and reports error");
                    TextBlock renderedError = Program.FindVisualChildren<TextBlock>(shell.WorkbenchTemplateHost).Single(text => text.Name == "templateError");
                    Assert(renderedError.IsVisible && renderedError.Text == OpenVisionLanguageService.T("Workbench.Status.Error"), menu + ": file error is rendered on template card");
                    capture(language + "-" + menu + "-corrupt-file-preserved");
                    document.SetTemplateSelectionForTest(_ => longPath);
                    Schedule(document);
                    Execute(file);
                    Wait(220);
                    Assert(model.CurrentStep.Parameters["PATTERN_PATH"] == longPath && ParametersWithoutTemplate(model.CurrentStep) == parameters && !model.HasStatus,
                        menu + ": valid file recovers without changing other values");
                    using (FileStream unlocked = new(longPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { Assert(unlocked.Length > 0, menu + ": thumbnail does not lock template file"); }
                    Assert(info.TextWrapping == TextWrapping.NoWrap && info.TextTrimming == TextTrimming.CharacterEllipsis && (string)info.ToolTip == longPath
                        && model.CurrentStep.Parameters["PATTERN_PATH"] == longPath && draw.ActualHeight >= 32D && file.ActualHeight >= 32D,
                        menu + ": long filename intentionally trims; complete path remains in existing editable PATTERN_PATH and supplemental tooltip");
                    Assert(ReferenceEquals(draw.Style, draw.FindResource("OpenVision.Workbench.ButtonStyle")), menu + ": ready redraw uses the existing secondary button style");
                    Assert(!renderedError.IsVisible, menu + ": valid file hides the prior error");
                    Assert(document.PreviewRunCount == runs && host.LayerDocumentCount == layers && route == document.RouteInputLayerName + "->" + document.RouteOutputLayerName,
                        menu + ": draw/cancel/file/recovery cancel AUTO and never execute or reroute");
                    capture(language + "-" + menu + "-ready-long-name");
                    document.SetTemplateSelectionForTest(null);
                    host.SelectToolForTest(VISION_MENU.Filter);
                    pump(4);
                    host.SelectToolForTest(menu);
                    pump(8);
                    Assert(model.CurrentStep!.Parameters["PATTERN_PATH"] == longPath && model.HasTemplate && ReferenceEquals(property, GetSelectedProperty(document.View)), menu + ": draft template restored without execution");
                    if (language == OpenVisionLanguage.Korean && menu == VISION_MENU.Matching)
                    {
                        document = host.ActiveNativeDocumentForTest!;
                        Assert(document.AddPipelineStep() != null, "Template: add ready template to memory pipeline");
                        pump(7);
                        document = host.ActiveNativeDocumentForTest!;
                        document.SetTemplateSelectionForTest(_ => firstPath);
                        Execute(Program.FindVisualChildren<Button>(shell.WorkbenchTemplateHost).Single(control => control.Name == "templateFileButton"));
                        pump(5);
                        Assert(pipeline.GetStep(0)!.Parameters["PATTERN_PATH"] == firstPath, "Template: selected step applies path to working copy");
                        pipeline.UndoCommand.Execute(null);
                        pump(6);
                        Assert(pipeline.GetStep(0)!.Parameters["PATTERN_PATH"] == longPath && model.HasTemplate, "Template: Undo restores template in working copy and rendered card");
                        document.SetTemplateSelectionForTest(null);
                        using System.Threading.ManualResetEventSlim release = new(false);
                        document.SetPreviewExecutionOverrideForTest(_ => { release.Wait(TimeSpan.FromSeconds(10)); throw new InvalidOperationException("Owned template busy probe."); });
                        try
                        {
                            document.RunPreview();
                            CommandManager.InvalidateRequerySuggested();
                            pump(10);
                            draw = Program.FindVisualChildren<Button>(shell.WorkbenchTemplateHost).Single(control => control.Name == "templateDrawButton"); file = Program.FindVisualChildren<Button>(shell.WorkbenchTemplateHost).Single(control => control.Name == "templateFileButton");
                            Assert(document.IsPreviewRunning && !draw.IsEnabled && !file.IsEnabled && !model.DrawTemplateCommand.CanExecute(null), "Template: real async running disables both template commands");
                            capture("Matching-template-busy");
                        }
                        finally
                        {
                            release.Set();
                            System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                            while (document.IsPreviewRunning && timer.Elapsed < TimeSpan.FromSeconds(5)) { pump(2); System.Threading.Thread.Sleep(5); }
                            document.SetPreviewExecutionOverrideForTest(null); pump(5);
                        }
                        Assert(!document.IsPreviewRunning && draw.IsEnabled && file.IsEnabled && document.PreviewRunCount == runs, "Template: failure restores commands without publishing a result");
                        model.NewTestCommand.Execute(null); pump(6); pipeline.UndoCommand.Execute(null); pump(6);
                    }
                    if (language == OpenVisionLanguage.English)
                    {
                        Assert(host.FloatDockedWpfToolWindowForTest(), "Template: float same matching view"); pump(8);
                        WpgPropertyGrid floatingGrid = Program.FindVisualChildren<WpgPropertyGrid>(shell.PropertyGridHost).Single(grid => grid.IsVisible);
                        Assert(floatingGrid.ActualWidth >= 600D && floatingGrid.ActualHeight >= 380D, menu + ": floating parameter viewport keeps the existing600x380 minimum");
                        checks.Add(language + "/" + menu + " floating: property viewport=" + floatingGrid.ActualWidth + "x" + floatingGrid.ActualHeight + "; PASS");
                        Assert(shell.WorkbenchTemplateHost.IsVisible && !Program.FindVisualChildren<VisionToolVerificationGuideView>(shell).Any(guide => guide.IsVisible), "Template: floating preserves card and hides old guide");
                        capture("English-" + menu + "-template-floating");
                        Assert(host.DockActiveWpfToolWindowForTest(), "Template: redock same matching view"); pump(8);
                    }
                    Dictionary<string, string> currentXml = SnapshotFiles(recipeDirectory).Where(item => Path.GetExtension(item.Key).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                        .ToDictionary(item => item.Key, item => item.Value, StringComparer.OrdinalIgnoreCase);
                    Assert(xmlFiles.Count == currentXml.Count && xmlFiles.All(item => currentXml.TryGetValue(item.Key, out string? hash) && hash == item.Value), menu + ": recipe XML unchanged; only explicitly taught template assets written");
                    checks.Add(language + "/" + menu + ": empty/ready; draw/redraw/cancel; corrupt→valid file; exact other values/object/route; disposed source/editor; draft restoration; no automatic execution or recipe XML write PASS");
                }
            }
        }
        finally { PropertyGridEditorFactory.SetImageEditorService(originalService); OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false); }
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".template-checks.txt"), checks);
        void Execute(Button button) { Assert(button.Command?.CanExecute(button.CommandParameter) == true, "Template: enabled command binding"); button.Command.Execute(button.CommandParameter); pump(5); }
        void Wait(int milliseconds) { System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew(); while (timer.ElapsedMilliseconds < milliseconds) { pump(2); System.Threading.Thread.Sleep(5); } }
        void Schedule(OpenVisionNativeToolDocument document)
        {
            VisionToolDebouncedPreviewScheduler scheduler = (VisionToolDebouncedPreviewScheduler)document.View.GetValue((DependencyProperty)typeof(VisionToolDebouncedPreviewScheduler)
                .GetField("SchedulerProperty", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.GetValue(null)!);
            scheduler.Schedule();
        }
    }

    private static string ParametersWithoutTemplate(VisionPipelineStep step)
    {
        VisionPipelineStep copy = VisionPipelineStepEditService.CopyStep(step);
        copy.Parameters.Remove("PATTERN_PATH");
        copy.Parameters.Remove("TemplatePath");
        return StepXml(copy);
    }

    private sealed class TemplateEditorProbe : IPropertyGridImageEditorService
    {
        private readonly IPropertyGridImageEditorService original;
        private readonly string sourcePath;
        internal bool Accepted { get; set; }
        internal OpenCvSharp.Rect Region { get; set; } = new(2, 3, 16, 12);
        internal OpenCvSharp.Mat? LastSource { get; private set; }
        internal TemplateEditViewProbe? LastView { get; private set; }
        internal string Mode { get; private set; } = string.Empty;
        internal double LastSavedRotation { get; private set; }
        internal TemplateEditorProbe(IPropertyGridImageEditorService original, string sourcePath) { this.original = original; this.sourcePath = sourcePath; }
        public IDisplayManager RuntimeContext { get => original.RuntimeContext; set => original.RuntimeContext = value; }
        public void SetRuntimeContext(Func<IDisplayManager> accessor) => original.SetRuntimeContext(accessor);
        public void SetRecipeNameContext(Func<string> accessor) => original.SetRecipeNameContext(accessor);
        public void SetSourceLayerContext(Func<string> accessor) => original.SetSourceLayerContext(accessor);
        public OpenCvSharp.Mat GetSourceImage() => LastSource = OpenCvSharp.Cv2.ImRead(sourcePath);
        public IPropertyGridImageEditView CreateImageEditView(OpenCvSharp.Mat source, System.Drawing.Rectangle roi, string mode)
        { Mode = mode; return LastView = new TemplateEditViewProbe(Accepted, Region); }
        public IPropertyGridImageEditView CreateImageEditView(OpenCvSharp.Mat source, List<OpenCvSharp.Rect> roi, string mode) => throw new NotSupportedException();
        public OpenCvSharp.Rect LoadTemplateRoi(OpenCvSharp.Mat source, string path) => original.LoadTemplateRoi(source, path);
        public double LoadTemplateRotationDegrees(string path) => original.LoadTemplateRotationDegrees(path);
        public string SaveTemplateImage(OpenCvSharp.Mat source, OpenCvSharp.Rect roi) => SaveTemplateImage(source, roi, 0D);
        public string SaveTemplateImage(OpenCvSharp.Mat source, OpenCvSharp.Rect roi, double rotation)
        { LastSavedRotation = rotation; return original.SaveTemplateImage(source, roi, rotation); }
    }

    private sealed class TemplateEditViewProbe : IPropertyGridTemplateImageEditView
    {
        private readonly bool accepted;
        public OpenCvSharp.Rect SelectedRegion { get; }
        public List<OpenCvSharp.Rect> SelectedRegions => new() { SelectedRegion };
        public double TemplateRotationDegrees { get; set; }
        internal string LoadedPath { get; private set; } = string.Empty;
        internal bool Disposed { get; private set; }
        internal TemplateEditViewProbe(bool accepted, OpenCvSharp.Rect roi) { this.accepted = accepted; SelectedRegion = roi; }
        public bool ShowDialog() => accepted;
        public void LoadPatternPreviewImage(string path) => LoadedPath = path;
        public void Dispose() => Disposed = true;
    }

    internal static void RunPresets(OpenVisionShellHostView host, string outputPath, string templatePath, Action<int> pump, Action<string> capture, VISION_MENU[]? selectedMenus = null)
    {
        OpenVisionWorkbenchToolViewModel model = host.Workbench.Tool;
        OpenVisionWorkbenchPipelineViewModel pipeline = host.Workbench.Pipeline;
        string recipeDirectory = RecipeWorkspaceService.GetRecipeWorkspaceDirectory(host.ActiveRecipeContextNameForTest);
        Dictionary<string, string> files = SnapshotFiles(recipeDirectory);
        List<string> checks = new();
        host.Workbench.IsAutoRunEnabled = true;
        host.SelectToolForTest(VISION_MENU.Blob);
        pump(8);
        string inputPath = Path.ChangeExtension(outputPath, ".preset-input.png");
        using (System.Drawing.Bitmap input = new(32, 32))
        {
            using System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(input);
            graphics.Clear(System.Drawing.Color.FromArgb(128, 128, 128));
            input.Save(inputPath, System.Drawing.Imaging.ImageFormat.Png);
        }
        Assert(host.ActiveNativeDocumentForTest!.LoadPreviewImageFromFileForTest(VisionToolPreviewImageRole.InputA, inputPath), "Preset: owned input loaded");
        foreach (OpenVisionLanguage language in new[] { OpenVisionLanguage.Korean, OpenVisionLanguage.English })
        {
            OpenVisionLanguageService.SetLanguage(language, false);
            foreach (VISION_MENU menu in selectedMenus ?? new[] { VISION_MENU.Blob, VISION_MENU.Contour, VISION_MENU.Line,
                VISION_MENU.Matching, VISION_MENU.EdgeBasedMatching, VISION_MENU.FeatureMatching })
            {
                host.SelectToolForTest(menu);
                pump(8);
                OpenVisionNativeToolDocument document = host.ActiveNativeDocumentForTest!;
                VisionToolSingleInputPropertyToolShell shell = (VisionToolSingleInputPropertyToolShell)document.View.FindName("toolShell");
                object property = GetSelectedProperty(document.View)!;
                capture(language + "-" + menu + "-before");
                Assert(shell.PresetHost.IsVisible && !shell.PresetMenuButton.IsVisible, menu + ": three preset buttons are visible in docked Workbench");
                Assert(shell.PresetTitleText.Text == OpenVisionLanguageService.T("Workbench.Tool.Presets") && !shell.PresetDetailText.IsVisible,
                    menu + ": Workbench preset title and no explanation line");
                string route = document.RouteInputLayerName + "->" + document.RouteOutputLayerName;
                int runs = document.PreviewRunCount;
                int layers = host.LayerDocumentCount;
                if (menu == VISION_MENU.Matching) { host.SetActiveMatchingTemplatePathForTest(templatePath); }
                else if (menu == VISION_MENU.EdgeBasedMatching) { host.SetActiveEdgeBasedMatchingTemplatePathForTest(templatePath); }
                else if (menu == VISION_MENU.FeatureMatching) { host.SetActiveFeatureMatchingTemplatePathForTest(templatePath); }
                VisionToolDebouncedPreviewScheduler.CancelPending(document.View);
                Button[] buttons = { shell.PresetBasicButton, shell.PresetFastButton, shell.PresetPreciseButton };
                string[] ids = { "Basic", "Fast", "Precise" };
                for (int index = 0; index < buttons.Length; index++)
                {
                    Button button = buttons[index];
                    Assert(button.IsVisible && button.IsEnabled && button.ActualHeight >= 32D,
                        menu + ": visible enabled preset hit target " + ids[index]);
                    Assert((string)button.Content == OpenVisionLanguageService.T("VisionTool.Preset." + ids[index])
                        && (string)button.ToolTip == OpenVisionLanguageService.T("Workbench.Tool.Preset" + ids[index]), menu + ": exact localized caption/short tooltip");
                    Rect bounds = button.TransformToAncestor(shell.PresetHost).TransformBounds(new Rect(button.RenderSize));
                    Assert(bounds.Left >= 0 && bounds.Right <= shell.PresetHost.ActualWidth + 1D && bounds.Bottom <= shell.PresetHost.ActualHeight + 1D,
                        menu + ": preset is inside its actual container");
                    VisionToolDebouncedPreviewScheduler scheduler = (VisionToolDebouncedPreviewScheduler)document.View.GetValue((DependencyProperty)typeof(VisionToolDebouncedPreviewScheduler)
                        .GetField("SchedulerProperty", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.GetValue(null)!);
                    scheduler.Schedule();
                    Assert(button.Focus() && button.IsKeyboardFocused, menu + ": preset keyboard focus");
                    button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    Wait(220);
                    string applied = string.Format(System.Globalization.CultureInfo.CurrentCulture, OpenVisionLanguageService.T("Workbench.Tool.PresetAppliedFormat"), OpenVisionLanguageService.T("VisionTool.Preset." + ids[index]));
                    TextBlock actionStatus = (TextBlock)host.FindName("workbenchActionStatus");
                    Assert(model.HasPresetApplied && model.PresetAppliedText == applied && actionStatus.IsVisible && actionStatus.Text == applied, menu + ": preset notification is rendered in the actual statusbar");
                    Rect statusBounds = actionStatus.TransformToAncestor(host).TransformBounds(new Rect(actionStatus.RenderSize));
                    Assert(statusBounds.Left >= 0D && statusBounds.Right <= host.ActualWidth + 1D && actionStatus.ActualWidth + .5D >= actionStatus.DesiredSize.Width, menu + ": applied status caption fits viewport");
                    Assert(document.PreviewRunCount == runs && host.LayerDocumentCount == layers, menu + ": preset cancels pending preview and creates no layer");
                    Assert(ReferenceEquals(property, GetSelectedProperty(document.View)) && route == document.RouteInputLayerName + "->" + document.RouteOutputLayerName,
                        menu + ": preset keeps the property object and route");
                    bool expectedValues = property switch
                    {
                        BlobProperty value => value.MIN_AREA == new[] { 200, 500, 80 }[index] && value.USE_ADAPTIVE_THRESHOLD == (index == 2),
                        ContourProperty value => value.MIN_AREA == new[] { 200, 500, 80 }[index] && value.THRESHOLD == new[] { 100D, 120D, 90D }[index],
                        LineGaugeProperty value => value.CONTRAST == new[] { 30D, 45D, 20D }[index] && value.SAMPLING_STEP == new[] { 10D, 16D, 4D }[index],
                        MatchingProperty value => value.SCORE_MIN == new[] { .6D, .7D, .6D }[index] && value.NUM_MATCH == new[] { 1, 1, 3 }[index] && !value.AUTO_PREVIEW,
                        EdgeBasedMatchingProperty value => value.SCORE_MIN == new[] { .75D, .8D, .7D }[index] && value.CANNY_LOW == new[] { 30, 35, 25 }[index],
                        FeatureMatchingProperty value => value.SCORE_MIN == new[] { .6D, .7D, .55D }[index] && value.RANSAC_REPROJ_THRESHOLD == new[] { 3D, 4D, 2D }[index],
                        _ => false
                    };
                    if (!expectedValues)
                    {
                        capture(language + "-" + menu + "-failed-" + ids[index]);
                        File.WriteAllText(Path.ChangeExtension(outputPath, "." + menu + "-failed-property.xml"), ObjectXml(property));
                    }
                    Assert(expectedValues, menu + ": preserved catalog values reach the actual property; preset=" + ids[index] + "; type=" + property.GetType().FullName
                        + (property is LineGaugeProperty line ? "; C=" + line.CONTRAST + "; Step=" + line.SAMPLING_STEP : string.Empty));
                    AssertFiles(files, recipeDirectory, menu + ": preset does not write recipe XML");
                }
                capture(language + "-" + menu + "-precise");
                OpenVisionLanguageService.SetLanguage(language == OpenVisionLanguage.Korean ? OpenVisionLanguage.English : OpenVisionLanguage.Korean, false);
                pump(6);
                Assert(((TextBlock)host.FindName("workbenchActionStatus")).Text == string.Format(System.Globalization.CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.T("Workbench.Tool.PresetAppliedFormat"), OpenVisionLanguageService.T("VisionTool.Preset.Precise")), menu + ": applied notification follows current language");
                OpenVisionLanguageService.SetLanguage(language, false);
                pump(6);
                Assert(document.PreviewRunCount == runs && route == document.RouteInputLayerName + "->" + document.RouteOutputLayerName, menu + ": notification localization has no side effect");
                if (language == OpenVisionLanguage.Korean && menu == VISION_MENU.Blob)
                {
                    void DrainPreview()
                    {
                        System.Diagnostics.Stopwatch elapsed = System.Diagnostics.Stopwatch.StartNew();
                        while (document.IsPreviewRunning && elapsed.Elapsed < TimeSpan.FromSeconds(5)) { pump(2); System.Threading.Thread.Sleep(5); }
                        pump(5);
                    }
                    using (System.Threading.ManualResetEventSlim releaseWorker = new(false))
                    {
                        document.SetPreviewExecutionOverrideForTest(_ =>
                        {
                            if (!releaseWorker.Wait(TimeSpan.FromSeconds(10))) { throw new InvalidOperationException("Owned preset worker was not released."); }
                            throw new InvalidOperationException("Owned preset failure probe.");
                        });
                        try
                        {
                            document.RunPreview();
                            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                            pump(10);
                            capture("Blob-presets-busy");
                            Assert(document.IsPreviewRunning && !shell.PresetHost.IsEnabled && buttons.All(button => !button.IsEnabled),
                                "Preset: actual asynchronous busy state disables all three presets");
                            string busyParameters = ObjectXml(property);
                            string busyNotification = model.PresetAppliedText;
                            foreach (Button button in buttons)
                            {
                                button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                                Border chrome = (Border)button.Template.FindName("buttonChrome", button);
                                Assert(button.IsVisible && button.Foreground is SolidColorBrush foreground && foreground.Color.A == 255
                                    && chrome.Background is SolidColorBrush background && background.Color.A == 255
                                    && chrome.BorderBrush is SolidColorBrush border && border.Color.A == 255,
                                    "Preset: disabled caption/background/border remain visible and opaque");
                                Assert(((SolidColorBrush)button.Foreground).Color == ((SolidColorBrush)button.FindResource("OpenVision.Workbench.MutedTextBrush")).Color
                                    && ((SolidColorBrush)chrome.Background).Color == ((SolidColorBrush)button.FindResource("OpenVision.Workbench.PanelAltBrush")).Color
                                    && ((SolidColorBrush)chrome.BorderBrush).Color == ((SolidColorBrush)button.FindResource("OpenVision.Workbench.StrongLineBrush")).Color,
                                    "Preset: disabled controls use existing semantic state colors");
                            }
                            Assert(ObjectXml(property) == busyParameters && model.PresetAppliedText == busyNotification,
                                "Preset: actual busy click entry preserves parameters and notification");
                            document.RunPreview();
                            Assert(document.PreviewRunCount == runs && host.LayerDocumentCount == layers,
                                "Preset: repeated run while busy does not execute or publish");
                        }
                        finally
                        {
                            releaseWorker.Set();
                            DrainPreview();
                            document.SetPreviewExecutionOverrideForTest(null);
                        }
                    }
                    Assert(!document.IsPreviewRunning && !document.HasPreviewResult && document.PreviewRunCount == runs
                        && document.LastStatusText.Contains("Owned preset failure probe.", StringComparison.Ordinal)
                        && buttons.All(button => button.IsEnabled), "Preset: failure restores all three controls without publishing a result");
                    capture("Blob-presets-failure-recovered");
                    shell.PresetBasicButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    Wait(220);
                    Assert(property is BlobProperty recovered && recovered.MIN_AREA == 200 && !recovered.USE_ADAPTIVE_THRESHOLD
                        && document.PreviewRunCount == runs && host.LayerDocumentCount == layers, "Preset: recovered basic preset changes values without execution");
                    document.RunPreview();
                    DrainPreview();
                    capture("Blob-presets-valid-recovery-probe");
                    File.WriteAllText(Path.ChangeExtension(outputPath, ".valid-recovery.txt"), "Running=" + document.IsPreviewRunning + "; HasResult=" + document.HasPreviewResult
                        + "; runs=" + document.PreviewRunCount + "; expected=" + (runs + 1) + "; enabled=" + string.Join(",", buttons.Select(button => button.IsEnabled))
                        + "; layers=" + host.LayerDocumentCount + "; route=" + route + "; status=" + document.LastStatusText + Environment.NewLine + ObjectXml(property));
                    Assert(!document.IsPreviewRunning && document.HasPreviewResult && document.PreviewRunCount == runs + 1
                        && buttons.All(button => button.IsEnabled), "Preset: valid explicit execution recovers after induced failure");
                    runs = document.PreviewRunCount;
                    layers = host.LayerDocumentCount;
                    capture("Blob-presets-valid-recovered");
                    shell.PresetPreciseButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    Wait(220);
                    Assert(((BlobProperty)property).MIN_AREA == 80 && document.PreviewRunCount == runs && host.LayerDocumentCount == layers,
                        "Preset: precise values restore after recovery without execution");
                    checks.Add("Blob async boundary: three disabled semantic presets; repeated run guarded; failure then valid explicit execution recovered PASS");
                }
                if (language == OpenVisionLanguage.Korean && menu == VISION_MENU.Blob)
                {
                    VisionPipelineStep added = document.AddPipelineStep() ?? throw new InvalidOperationException("Preset: Blob memory add failed");
                    pump(8);
                    shell.PresetFastButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    Wait(220);
                    Assert(model.IsStepEditing && model.IsChanged && model.CurrentStep!.Parameters["MIN_AREA"] == "500"
                        && document.PreviewRunCount == runs && host.LayerDocumentCount == layers, "Preset: selected step updates working copy only");
                    capture("Blob-step-fast");
                    pipeline.UndoCommand.Execute(null);
                    pump(8);
                    Assert(model.CurrentStep!.Parameters["MIN_AREA"] == "80" && document.PreviewRunCount == runs, "Preset: Undo restores values without execution");
                    model.NewTestCommand.Execute(null);
                    pump(6);
                    pipeline.UndoCommand.Execute(null);
                    pump(6);
                }
                if (menu == VISION_MENU.Line)
                {
                    LineToolWpfView view = (LineToolWpfView)document.View;
                    string lineABefore = ObjectXml(view.CreateLineAProperty());
                    string lineBBefore = ObjectXml(view.CreateLineBProperty());
                    string purposeBefore = view.SelectedPurpose;
                    string lineBefore = view.SelectedLineName;
                    Border header = (Border)view.FindName("purposeContentHost");
                    TextBlock purposeLabel = (TextBlock)view.FindName("txtPurposeLabel");
                    TextBlock lineLabel = (TextBlock)view.FindName("txtLineSelectorLabel");
                    Assert(lineLabel.Text == (language == OpenVisionLanguage.Korean ? "라인" : "Line"), "Line: exact localized selector label");
                    TextBlock note = (TextBlock)view.FindName("txtPurposeHint");
                    Button roi = (Button)view.FindName("btnEditSelectedRoi");
                    TextBlock roiCaption = (TextBlock)view.FindName("txtEditSelectedRoiCaption");
                    var controller = (VisionToolSingleInputSpecialPropertyToolController)typeof(LineToolWpfView)
                        .GetField("toolController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(view)!;
                    foreach (string purpose in new[] { "Edge", "Measure", "Intersection" })
                    {
                        RadioButton purposeButton = (RadioButton)view.FindName("rdoPurpose" + purpose);
                        string purposeKey = purpose == "Measure" ? "LineMeasure" : purpose == "Intersection" ? "LineIntersection" : "LineEdge";
                        Assert(purposeLabel.Text == OpenVisionLanguageService.T("Workbench.Tool.LinePurpose")
                            && (string)purposeButton.Content == OpenVisionLanguageService.T("Workbench.Tool." + purposeKey), "Line: exact localized purpose caption");
                        foreach (string line in new[] { "A", "B" })
                        {
                            controller.ResultReviewText.Text = "Owned retained result presentation probe";
                            purposeButton.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
                            purposeButton.Command.Execute(purposeButton.CommandParameter);
                            RadioButton lineButton = (RadioButton)view.FindName("rdoLine" + line);
                            lineButton.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
                            lineButton.Command.Execute(lineButton.CommandParameter);
                            pump(5);
                            string expectedRoi = string.Format(OpenVisionLanguageService.T("Workbench.Tool.LineDrawRoiFormat"), line);
                            string expectedNote = OpenVisionLanguageService.T("Workbench.Tool.LineNote" + purpose) + " · "
                                + string.Format(OpenVisionLanguageService.T("Workbench.Tool.LineEditingFormat"), line);
                            File.WriteAllText(Path.ChangeExtension(outputPath, ".Line-header-last.txt"), $"Purpose={view.SelectedPurpose}/{purpose}; Line={view.SelectedLineName}/{line}; purposeChecked={purposeButton.IsChecked}; lineChecked={lineButton.IsChecked}; noteVisible={note.IsVisible}; note={note.Text}/{expectedNote}; ROI={roiCaption.Text}/{expectedRoi}; Name={AutomationProperties.GetName(roi)}; enabled={roi.IsEnabled}; canExecute={roi.Command.CanExecute(roi.CommandParameter)}; roiWidth={roiCaption.ActualWidth}/{roiCaption.DesiredSize.Width}; noteHeight={note.ActualHeight}/{note.DesiredSize.Height}; PG={shell.PropertyGridHost.ActualWidth}x{shell.PropertyGridHost.ActualHeight}");
                            capture(language + "-Line-probe-" + purpose + "-" + line);
                            Assert(view.SelectedPurpose == purpose && view.SelectedLineName == "Line " + line && purposeButton.IsChecked == true
                                && lineButton.IsChecked == true && note.IsVisible && note.Text == expectedNote && roiCaption.Text == expectedRoi
                                && AutomationProperties.GetName(roi) == expectedRoi && roi.IsEnabled && roi.Command.CanExecute(roi.CommandParameter),
                                "Line: actual radio state, current purpose/line note and selected ROI command caption");
                            Assert(roiCaption.ActualWidth + .5D >= roiCaption.DesiredSize.Width && note.ActualHeight + note.Margin.Top + note.Margin.Bottom + .5D >= note.DesiredSize.Height
                                && shell.PropertyGridHost.ActualHeight >= 160D, "Line: full caption/note and unchanged minimum parameter height");
                            foreach (FrameworkElement control in new FrameworkElement[] { purposeLabel, purposeButton, lineButton, roi, note })
                            {
                                Rect bounds = control.TransformToAncestor(header).TransformBounds(new Rect(control.RenderSize));
                                Assert(bounds.Left >= 0D && bounds.Right <= header.ActualWidth + 1D && bounds.Bottom <= header.ActualHeight + 1D,
                                    "Line: header controls remain inside their actual container");
                            }
                            Assert(ObjectXml(view.CreateLineAProperty()) == lineABefore && ObjectXml(view.CreateLineBProperty()) == lineBBefore
                                && document.PreviewRunCount == runs && host.LayerDocumentCount == layers
                                && document.RouteInputLayerName + "->" + document.RouteOutputLayerName == route, "Line: header selection retains both parameter objects, runs, layers and routes");
                            capture(language + "-Line-header-" + purpose + "-" + line);
                        }
                    }
                    view.SetPurposeForTest(purposeBefore);
                    view.SetLineSettingForTest(lineBefore);
                    pump(4);
                    checks.Add(language + "/Line header: 3 purposes x A/B; retained-result selection; exact ROI/note/accessible name; bounds/PG160; parameters/routes/layers/runs unchanged PASS");
                }
                if (language == OpenVisionLanguage.English && menu == VISION_MENU.Line)
                {
                    string parameters = ParameterState(document);
                    Assert(host.FloatDockedWpfToolWindowForTest(), "Preset: float selected Line");
                    pump(8);
                    Assert(shell.PresetHost.IsVisible && !shell.PresetDetailText.IsVisible, "Preset: floating Workbench keeps three buttons and no explanation");
                    WpgPropertyGrid floatingGrid = Program.FindVisualChildren<WpgPropertyGrid>(shell.PropertyGridHost).Single(grid => grid.IsVisible);
                    Assert(floatingGrid.ActualWidth >= 600D && floatingGrid.ActualHeight >= 380D, "Line: floating parameter viewport keeps the existing600x380 minimum");
                    checks.Add(language + "/Line floating: property viewport=" + floatingGrid.ActualWidth + "x" + floatingGrid.ActualHeight + "; PASS");
                    Assert(host.DockActiveWpfToolWindowForTest(), "Preset: dock selected Line");
                    pump(8);
                    Assert(shell.PresetHost.IsVisible && !shell.PresetMenuButton.IsVisible && ParameterState(document) == parameters
                        && document.PreviewRunCount == runs, "Preset: dock restores values without execution");
                }
                checks.Add(language + "/" + menu + ": three localized presets; exact catalog values; keyboard focus; pending cancelled; same object/route; no SDK/layer/disk write PASS");
            }
        }
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        AssertFiles(files, recipeDirectory, "Preset: language/dock/Undo do not write XML");
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".preset-checks.txt"), checks);
        void Wait(int milliseconds)
        {
            System.Diagnostics.Stopwatch elapsed = System.Diagnostics.Stopwatch.StartNew();
            while (elapsed.ElapsedMilliseconds < milliseconds) { pump(2); System.Threading.Thread.Sleep(4); }
            pump(4);
        }
    }

    internal static void RunAutoRun(OpenVisionShellHostView host, string outputPath, Action<int> pump, Action<string> capture)
    {
        OpenVisionWorkbenchToolViewModel model = host.Workbench.Tool;
        OpenVisionWorkbenchPipelineViewModel pipeline = host.Workbench.Pipeline;
        string recipeDirectory = RecipeWorkspaceService.GetRecipeWorkspaceDirectory(host.ActiveRecipeContextNameForTest);
        Dictionary<string, string> files = SnapshotFiles(recipeDirectory);
        List<string> checks = new();
        Assert(new SystemStateConfig().WorkbenchAutoRunEnabled, "Auto: new user preference defaults on");
        using (StringReader missing = new("<SYSTEM />"))
            Assert(((SystemStateConfig)new XmlSerializer(typeof(SystemStateConfig)).Deserialize(missing)!).WorkbenchAutoRunEnabled, "Auto: omitted old XML field defaults on");
        host.Workbench.IsAutoRunEnabled = true;
        host.SelectToolForTest(VISION_MENU.Threshold);
        pump(8);
        OpenVisionNativeToolDocument threshold = host.ActiveNativeDocumentForTest!;
        string inputPath = Path.ChangeExtension(outputPath, ".auto-input.png");
        using (System.Drawing.Bitmap input = new(32, 32))
        {
            using System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(input);
            graphics.Clear(System.Drawing.Color.FromArgb(128, 128, 128));
            input.Save(inputPath, System.Drawing.Imaging.ImageFormat.Png);
        }
        Assert(threshold.LoadPreviewImageFromFileForTest(VisionToolPreviewImageRole.InputA, inputPath), "Auto: owned Main image loaded");
        ToggleButton toggle = AutoToggle(threshold);
        Assert(toggle.IsVisible && toggle.IsChecked == true && model.IsAutoRunEnabled && BindingOperations.IsDataBound(toggle, ToggleButton.IsCheckedProperty), "Auto: default-on rendered switch binding");
        toggle.SetCurrentValue(ToggleButton.IsCheckedProperty, false);
        pump(4);
        Assert(!model.IsAutoRunEnabled && !host.Workbench.IsAutoRunEnabled && !VisionToolDebouncedPreviewScheduler.GetAllowAutoPreview(threshold.View), "Auto: rendered switch writes user preference and scheduler gate");
        int runs = threshold.PreviewRunCount;
        EditThreshold(180);
        Wait(220);
        Assert(threshold.PreviewRunCount == runs && !model.HasResult, "Auto: off parameter edit does not execute");
        capture("Threshold-off-before-run");
        model.RunCommand.Execute(null);
        Drain(threshold);
        Assert(threshold.PreviewRunCount == runs + 1 && model.ResultSummaryText.Contains("180"), "Auto: explicit command still executes while off");
        AssertGray(threshold, 0, "off-explicit-output");
        toggle = AutoToggle(threshold);
        toggle.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
        Wait(220);
        Assert(threshold.PreviewRunCount == runs + 1, "Auto: enabling the switch does not execute");
        EditThreshold(170); EditThreshold(150); EditThreshold(100);
        Wait(250); Drain(threshold);
        Assert(threshold.PreviewRunCount == runs + 2 && model.ResultSummaryText.Contains("100") && !model.IsResultPrevious, "Auto: rapid real editor changes debounce to one SDK execution with the latest cutoff");
        AssertGray(threshold, 255, "on-latest-output");
        capture("Threshold-on-current");
        TextBox focusedInput = (TextBox)threshold.View.FindName("txtThreshold");
        focusedInput.Focus();
        pump(3);
        Assert(focusedInput.IsKeyboardFocused, "Auto: real editor retains keyboard focus");
        focusedInput.SelectAll();
        focusedInput.SelectedText = "97";
        Wait(250); Drain(threshold);
        Assert(threshold.PreviewRunCount == runs + 3 && model.ResultSummaryText.Contains("97") && !model.IsResultPrevious,
            "Auto: focused text event without manual source update/RecordParameterChange publishes the actual executed cutoff");
        Assert(model.IsNewTest && model.CurrentStep.Parameters["Threshold"] == "97", "Auto: focused text changes the working draft before result publish");
        AssertGray(threshold, 255, "on-focused-output");
        capture("Threshold-focused-current");
        runs++;
        EditThreshold(190);
        toggle.SetCurrentValue(ToggleButton.IsCheckedProperty, false);
        toggle.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
        Wait(220);
        Assert(threshold.PreviewRunCount == runs + 2 && model.IsResultPrevious && model.ResultSummaryText.Contains("97"), "Auto: off cancels a pending timer; re-enabling does not revive it");
        model.RunCommand.Execute(null);
        Drain(threshold);
        Assert(threshold.PreviewRunCount == runs + 3 && model.ResultSummaryText.Contains("190"), "Auto: explicit command cancels any pending duplicate");
        Wait(220);
        Assert(threshold.PreviewRunCount == runs + 3, "Auto: explicit commit did not schedule a second execution");
        toggle = AutoToggle(threshold);
        toggle.SetCurrentValue(ToggleButton.IsCheckedProperty, false);
        SystemState loaded = new();
        Assert(loaded.LoadConfig() && !loaded.WorkbenchLayoutPreferences.WorkbenchAutoRunEnabled, "Auto: actual SystemState saved preference reloads off");
        Assert(loaded.SaveConfig(), "Auto: ordinary SystemState save retains preference");
        SystemState reopened = new();
        Assert(reopened.LoadConfig() && !reopened.WorkbenchLayoutPreferences.WorkbenchAutoRunEnabled, "Auto: save/load round trip retains off");
        using (OpenVisionWorkbenchViewModel restored = new(reopened.WorkbenchLayoutPreferences, _ => true))
            Assert(!restored.IsAutoRunEnabled, "Auto: a new Workbench VM restores off");
        File.Copy(RecipeWorkspaceService.GetSystemConfigPath("SYSTEM"), Path.ChangeExtension(outputPath, ".saved-off.xml"), true);
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, false);
        Wait(150);
        Assert(threshold.PreviewRunCount == runs + 3 && AutoToggle(threshold).IsChecked == false
            && AutomationProperties.GetName(AutoToggle(threshold)) == "Run on change", "Auto: English caption and preference restoration do not execute");
        capture("Threshold-English-off");
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        Assert(threshold.AddPipelineStep() != null && model.IsStepEditing, "Auto: current test adds to the working step");
        pipeline.InsertStep(1, new VisionPipelineStep { Name = "Downstream mean", ToolType = "Mean", InputLayer = "Main", OutputLayer = "Downstream_Mean" });
        pipeline.SelectedStepIndex = 1;
        pump(8);
        OpenVisionNativeToolDocument mean = host.ActiveNativeDocumentForTest!;
        model.RunCommand.Execute(null);
        Drain(mean);
        VisionPipelineStepResultSummary downstream = pipeline.GetResult(1)!;
        Assert(downstream.Success && downstream.Executed, "Auto: downstream SDK result exists before upstream edit");
        int meanRuns = mean.PreviewRunCount;
        pipeline.SelectedStepIndex = 0;
        pump(8);
        threshold = host.ActiveNativeDocumentForTest!;
        runs = threshold.PreviewRunCount;
        AutoToggle(threshold).SetCurrentValue(ToggleButton.IsCheckedProperty, true);
        Wait(180);
        Assert(threshold.PreviewRunCount == runs && mean.PreviewRunCount == meanRuns, "Auto: stage selection and option restore do not execute");
        EditThreshold(80);
        Wait(250); Drain(threshold);
        Assert(threshold.PreviewRunCount == runs + 1 && mean.PreviewRunCount == meanRuns && ReferenceEquals(downstream, pipeline.GetResult(1))
            && pipeline.IsResultPrevious(1), "Auto: only the edited step executes; downstream keeps its real previous result");
        capture("Threshold-step-current-downstream-previous");
        focusedInput = (TextBox)threshold.View.FindName("txtThreshold");
        focusedInput.Focus();
        pump(3);
        focusedInput.SelectAll();
        focusedInput.SelectedText = "77";
        Wait(250); Drain(threshold);
        Assert(threshold.PreviewRunCount == runs + 2 && model.ResultSummaryText.Contains("77") && model.CurrentStep.Parameters["Threshold"] == "77"
            && !model.IsResultPrevious && mean.PreviewRunCount == meanRuns, "Auto: focused step text event commits and publishes the same executed cutoff without Tab");
        AssertGray(threshold, 255, "step-focused-output");
        capture("Threshold-step-focused-current");
        runs = threshold.PreviewRunCount;
        EditThreshold(220);
        Assert(threshold.LoadPreviewImageFromFileForTest(VisionToolPreviewImageRole.InputA, inputPath), "Auto: input reload while an edit is pending");
        Wait(250);
        Assert(threshold.PreviewRunCount == runs && mean.PreviewRunCount == meanRuns, "Auto: input reload cancels the pending edit preview and does not execute");
        EditThreshold(110);
        pipeline.SelectedStepIndex = 1;
        Wait(250);
        Assert(threshold.PreviewRunCount == runs && mean.PreviewRunCount == meanRuns, "Auto: switching away cancels a pending preview");
        pipeline.SelectedStepIndex = 0;
        Wait(250);
        threshold = host.ActiveNativeDocumentForTest!;
        Assert(threshold.PreviewRunCount == runs, "Auto: step restoration does not revive the pending preview");
        host.Workbench.IsAutoRunEnabled = false;
        foreach (VISION_MENU menu in new[] { VISION_MENU.Threshold, VISION_MENU.Filter, VISION_MENU.Morphology, VISION_MENU.Arithmetic,
            VISION_MENU.EdgeDetection, VISION_MENU.RotateAndScale, VISION_MENU.HSV, VISION_MENU.Histogram, VISION_MENU.Mean })
        {
            host.SelectToolForTest(menu);
            pump(6);
            OpenVisionNativeToolDocument document = host.ActiveNativeDocumentForTest!;
            int priorRuns = document.PreviewRunCount;
            ToggleButton actual = AutoToggle(document);
            Assert(model.SupportsAutoRun && actual.IsVisible && actual.IsChecked == false
                && !VisionToolDebouncedPreviewScheduler.GetAllowAutoPreview(document.View), menu + ": shared off preference is bound to the actual native View");
            VisionToolDebouncedPreviewScheduler scheduler = (VisionToolDebouncedPreviewScheduler)document.View.GetValue((DependencyProperty)typeof(VisionToolDebouncedPreviewScheduler)
                .GetField("SchedulerProperty", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.GetValue(null)!);
            scheduler.Schedule();
            Wait(160);
            Assert(document.PreviewRunCount == priorRuns, menu + ": common scheduler refuses while off");
            Rect toggleBounds = actual.TransformToAncestor((FrameworkElement)document.View.FindName("toolShell")).TransformBounds(new Rect(actual.RenderSize));
            Assert(toggleBounds.Width >= 30 && toggleBounds.Height >= 18 && actual.Focus() && actual.IsKeyboardFocused, menu + ": actual switch is visible and focusable");
            capture(menu + "-off-option");
            checks.Add(menu + ": shared off preference/gate/focus PASS; this case uses Schedule, not a parameter editor input.");
        }
        host.SelectToolForTest(VISION_MENU.Matching);
        pump(8);
        OpenVisionNativeToolDocument matching = host.ActiveNativeDocumentForTest!;
        MatchingProperty property = (MatchingProperty)GetSelectedProperty(matching.View)!;
        Assert(!property.AUTO_PREVIEW && !model.HasAutoRunOption, "Auto: Matching opt-in remains off by default");
        int matchingRuns = matching.PreviewRunCount;
        property.AUTO_PREVIEW = true;
        matching.RecordParameterChange(null);
        pump(8);
        Assert(model.HasAutoRunOption && AutoToggle(matching).IsVisible && AutoToggle(matching).IsChecked == false
            && matching.PreviewRunCount == matchingRuns, "Auto: Matching opt-in reveals shared off option without executing");
        property.AUTO_PREVIEW = false;
        matching.RecordParameterChange(null);
        pump(6);
        Assert(!model.HasAutoRunOption && matching.PreviewRunCount == matchingRuns, "Auto: Matching opt-out does not execute");
        AssertFiles(files, recipeDirectory, "Auto: switches/edits/execution do not save recipe XML");
        host.SelectToolForTest(VISION_MENU.Threshold);
        pump(8);
        capture("Threshold-final-off");
        checks.Add("Threshold actual editor off/on/latest debounce/pending cancel/explicit command SDK pixels; actual downstream previous result; SystemState save/load and new VM off; language/selection/input reload no run PASS.");
        checks.Add("Matching opt-in visibility/no-execution PASS; actual Matching parameter-event execution is not exercised here.");
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".auto-checks.txt"), checks);

        ToggleButton AutoToggle(OpenVisionNativeToolDocument document)
        {
            FrameworkElement shell = (FrameworkElement)document.View.FindName("toolShell");
            ContentControl result = (ContentControl)shell.FindName("workbenchResultHost");
            pump(3);
            return Program.FindVisualChildren<ToggleButton>(result).Single(item => AutomationProperties.GetAutomationId(item) == "WorkbenchAutoRun");
        }
        void EditThreshold(int value)
        {
            TextBox field = (TextBox)threshold.View.FindName("txtThreshold");
            field.Text = value.ToString(CultureInfo.InvariantCulture);
            field.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
            threshold.RecordParameterChange(null);
        }
        void Wait(int milliseconds)
        {
            System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
            while (timer.ElapsedMilliseconds < milliseconds) { pump(2); System.Threading.Thread.Sleep(2); }
            pump(4);
        }
        void Drain(OpenVisionNativeToolDocument document)
        {
            System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
            while (document.IsPreviewRunning && timer.Elapsed < TimeSpan.FromSeconds(8)) { pump(2); System.Threading.Thread.Sleep(5); }
            pump(8);
            Assert(!document.IsPreviewRunning, "Auto: owned SDK execution completes");
        }
        void AssertGray(OpenVisionNativeToolDocument document, int expected, string name)
        {
            string export = Path.ChangeExtension(outputPath, "." + name + ".png");
            Assert(document.SavePreviewImageToFileForTest(VisionToolPreviewImageRole.Output, export), "Auto: real output export");
            using System.Drawing.Bitmap bitmap = new(export);
            System.Drawing.Color actual = bitmap.GetPixel(0, 0);
            Assert(bitmap.Width == 32 && bitmap.Height == 32 && actual.R == expected && actual.G == expected && actual.B == expected, "Auto: real output pixels match executed cutoff");
        }
    }

    internal static void RunResultSummary(OpenVisionShellHostView host, string outputPath, Action<int> pump, Action<string> capture)
    {
        OpenVisionWorkbenchToolViewModel model = host.Workbench.Tool;
        OpenVisionWorkbenchPipelineViewModel pipeline = host.Workbench.Pipeline;
        string recipeDirectory = RecipeWorkspaceService.GetRecipeWorkspaceDirectory(host.ActiveRecipeContextNameForTest);
        Dictionary<string, string> files = SnapshotFiles(recipeDirectory);
        string inputPath = Path.ChangeExtension(outputPath, ".result-input.png");
        using (System.Drawing.Bitmap input = new(32, 32))
        {
            using System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(input);
            graphics.Clear(System.Drawing.Color.FromArgb(128, 128, 128));
            input.Save(inputPath, System.Drawing.Imaging.ImageFormat.Png);
        }
        host.SelectToolForTest(VISION_MENU.Threshold);
        pump(5);
        OpenVisionNativeToolDocument threshold = host.ActiveNativeDocumentForTest!;
        foreach (OpenVisionLanguage language in new[] { OpenVisionLanguage.Korean, OpenVisionLanguage.English })
        {
            OpenVisionLanguageService.SetLanguage(language, false);
            pump(5);
            string expected = language == OpenVisionLanguage.Korean ? "실행 전" : "Not run";
            Assert(!model.HasResult && model.ResultKind == "NotRun" && model.ResultStatusText == expected, "Result: never-run caption follows AppendixC " + language);
            AssertRendered(threshold, "NotRun", expected);
            capture("Threshold-" + language + "-before-run-status");
        }
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        Assert(threshold.LoadPreviewImageFromFileForTest(VisionToolPreviewImageRole.InputA, inputPath), "Result: owned Main image load");
        TextBox t = (TextBox)threshold.View.FindName("txtThreshold");
        t.Text = "180";
        t.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
        threshold.RecordParameterChange(null);
        CancelThresholdSchedule();
        pump(8);
        capture("Threshold-before-run");
        threshold.RunPreview();
        Drain(threshold);
        Assert(threshold.HasPreviewResult && threshold.PreviewRunCount == 1, "Result: real Threshold SDK execution succeeded: " + threshold.LastStatusText);
        capture("Threshold-real-result");
        host.SelectToolForTest(VISION_MENU.Arithmetic);
        pump(5);
        OpenVisionNativeToolDocument arithmetic = host.ActiveNativeDocumentForTest!;
        ArithmeticToolWpfView arithmeticView = (ArithmeticToolWpfView)arithmetic.View;
        ArithmeticToolSettings arithmeticSettings = arithmeticView.CaptureSettings();
        arithmeticSettings.UseOffsetMode = true;
        arithmeticSettings.OffsetX = arithmeticSettings.OffsetY = 0;
        arithmeticView.ApplyPersistedSettings(arithmeticSettings);
        arithmetic.RecordParameterChange(null);
        arithmetic.RunPreview();
        Drain(arithmetic);
        Assert(arithmetic.HasPreviewResult && arithmetic.PreviewRunCount == 1, "Result: real Arithmetic SDK execution succeeded: " + arithmetic.LastStatusText);
        capture("Arithmetic-real-result");
        Assert(typeof(OpenVisionWorkbenchToolViewModel).GetProperty("ResultKind") != null, "Result: Workbench summary binding has not been implemented in this baseline");
        Assert(Read<string>("ResultKind") == "Done" && Summary()?.HasResultImage == true, "Result: Arithmetic without acceptance is Done and copied before SDK Dispose");
        Assert(Read<string>("ResultSummaryText") == OpenVisionLanguageService.T("Arithmetic.OffsetMode") + " · X 0 · Y 0",
            "Result: Arithmetic summary uses the executed offset snapshot");
        AssertRendered(arithmetic, "Done");
        host.SelectToolForTest(VISION_MENU.Threshold);
        pump(5);
        Assert(ReferenceEquals(threshold, host.ActiveNativeDocumentForTest) && Read<string>("ResultKind") == "Done", "Result: returning to the same test retains its real result without rerun");
        VisionPipelineStepResultSummary first = Summary()!;
        Assert(first.Success && first.Executed && first.Metrics.Count > 0 && Read<IReadOnlyList<KeyValuePair<string, string>>>("ResultMetrics").Count <= 6,
            "Result: real managed metrics are present, capped at six, after SDK release");
        Assert(Read<string>("ResultSummaryText").Contains("180") && first.Parameters["Threshold"] == "180",
            "Result: Threshold summary includes the executed cutoff");
        Assert(OpenVisionPipelineReviewResultPresenter.WorkbenchMetricName("OwnedUnlistedMetric") == "OwnedUnlistedMetric",
            "Result: an unlisted metric keeps its code name");
        AssertRendered(threshold, "Done");
        threshold.RunPreview();
        Drain(threshold);
        int runs = threshold.PreviewRunCount;
        t.Text = "100";
        t.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
        threshold.RecordParameterChange(null);
        CancelThresholdSchedule();
        pump(5);
        Assert(Read<bool>("IsResultPrevious") && threshold.PreviewRunCount == runs && ReferenceEquals(first, Summary()) == false,
            "Result: setting edit preserves the latest summary as Previous without execution");
        Assert(Read<string>("ResultSummaryText").Contains("180") && Summary()!.Parameters["Threshold"] == "180",
            "Result: Previous retains the executed cutoff after editing to100");
        AssertRendered(threshold, "Previous");
        capture("Threshold-previous");
        VisionPipelineStepResultSummary previous = Summary()!;
        Assert(threshold.AddPipelineStep() != null && model.IsStepEditing && ReferenceEquals(Summary(), previous) && Read<bool>("IsResultPrevious"),
            "Result: add inherits the same result and its stale state without execution");
        Assert(threshold.PreviewRunCount == runs, "Result: adding does not rerun");
        threshold = host.ActiveNativeDocumentForTest!;
        threshold.RunPreview();
        Drain(threshold);
        Assert(!Read<bool>("IsResultPrevious") && Read<string>("ResultKind") == "Done", "Result: actual step rerun becomes current");
        capture("Threshold-step-current");
        Assert(Read<string>("ResultSummaryText").Contains("100"), "Result: explicit rerun updates the result cutoff");
        AssertGray(threshold, VisionToolPreviewImageRole.InputA, "Threshold-ko-input", 128);
        AssertGray(threshold, VisionToolPreviewImageRole.Output, "Threshold-ko-output", 255);
        int beforeLanguageRuns = threshold.PreviewRunCount;
        var beforeLanguage = ((ThresholdToolWpfView)threshold.View).CreateProperty();
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, false);
        pump(8);
        var afterLanguage = ((ThresholdToolWpfView)threshold.View).CreateProperty();
        Assert(ReferenceEquals(threshold, host.ActiveNativeDocumentForTest) && beforeLanguage.Threshold == afterLanguage.Threshold
            && beforeLanguage.ThresholdType == afterLanguage.ThresholdType && model.InputLayer == "Main",
            "Result: language keeps the same native editor, parameters and input");
        Assert(threshold.PreviewRunCount == beforeLanguageRuns, "Result: language change does not execute");
        Assert(Read<string>("ResultSummaryText") == "Binary · Threshold 100", "Result: English summary uses the same executed cutoff");
        AssertGray(threshold, VisionToolPreviewImageRole.InputA, "Threshold-English-input", 128);
        AssertGray(threshold, VisionToolPreviewImageRole.Output, "Threshold-English-retained-output", 255);
        System.Windows.Input.ICommand run = Read<System.Windows.Input.ICommand>("RunCommand");
        Assert(host.InputBindings.OfType<System.Windows.Input.KeyBinding>().Any(binding => binding.Key == System.Windows.Input.Key.F8
            && ReferenceEquals(binding.Command, run)), "Result: host F8 uses the active tool command");
        Assert(run.CanExecute(null), "Result: ready active-tool command enabled");
        run.Execute(null);
        run.Execute(null);
        Drain(threshold);
        Assert(threshold.PreviewRunCount == beforeLanguageRuns + 1, "Result: repeated active-tool command executes once while busy");
        AssertGray(threshold, VisionToolPreviewImageRole.Output, "Threshold-English-rerun-output", 255);
        capture("Threshold-English-step-current");
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        pump(5);
        ChangeParameter(threshold);
        threshold.RecordParameterChange(null);
        CancelThresholdSchedule();
        pipeline.UndoCommand.Execute(null);
        pump(5);
        Assert(Read<bool>("IsResultPrevious"), "Result: Undo does not clear stale results");
        VisionPipelineStepResultSummary thresholdResult = Summary()!;
        pipeline.InsertStep(0, new VisionPipelineStep { Name = "Result identity probe", ToolType = "Mean", InputLayer = "Main", OutputLayer = "Result_IdentityProbe" });
        pipeline.UndoCommand.Execute(null);
        pump(5);
        Assert(ReferenceEquals(Summary(), thresholdResult) && Read<bool>("IsResultPrevious"), "Result: insert/Undo restores retained results to the same step identity");
        AssertFiles(files, recipeDirectory, "Result: execute/edit/add/Undo do not save recipe files");

        pipeline.InsertStep(0, new VisionPipelineStep { Name = "Sparse saved threshold", ToolType = "Threshold", InputLayer = "Main", OutputLayer = "Sparse_Threshold" });
        pipeline.SelectStepCommand.Execute(0);
        pump(5);
        OpenVisionNativeToolDocument sparseThreshold = host.ActiveNativeDocumentForTest!;
        Assert(model.CurrentStep!.Parameters.Count == 0, "Result: saved sparse step keeps its missing-key representation before execution");
        model.RunCommand.Execute(null);
        Drain(sparseThreshold);
        Assert(Summary()!.Parameters["Threshold"] == "127" && Read<string>("ResultSummaryText").Contains("127")
            && model.CurrentStep.Parameters.Count == 0, "Result: executed native defaults appear in result metadata without rewriting the working step");
        AssertGray(sparseThreshold, VisionToolPreviewImageRole.Output, "Threshold-sparse-output", 255);
        capture("Threshold-sparse-saved-current");
        AssertFiles(files, recipeDirectory, "Result: native-default snapshot does not save recipe files");
        pipeline.UndoCommand.Execute(null);
        pump(5);

        host.SelectToolForTest(VISION_MENU.Blob);
        pump(5);
        OpenVisionNativeToolDocument blob = host.ActiveNativeDocumentForTest!;
        BlobProperty property = (BlobProperty)GetSelectedProperty(blob.View)!;
        property.USE_THRESHOLD = true;
        property.THRESHOLD = 64;
        property.THRESHOLD_TYPES = OpenCvSharp.ThresholdTypes.Binary;
        property.USE_BITWISENOT = property.USE_ADAPTIVE_THRESHOLD = false;
        property.USE_ROI = property.USE_MULTI_ROI = true;
        property.CvROI = new OpenCvSharp.Rect(0, 0, 32, 32);
        property.CvROIS = new List<OpenCvSharp.Rect> { property.CvROI };
        property.USE_MASKING = false;
        property.MIN_AREA = 1;
        property.MAX_AREA = property.MAX_WIDTH = property.MAX_HEIGHT = 1000000;
        property.MIN_WIDTH = property.MIN_HEIGHT = 0;
        blob.RecordParameterChange(null);
        model.SetRoute(GetGeneration(model), "Main", string.Empty, model.OutputLayer);
        model.UseAcceptance = true;
        model.AcceptanceMetricName = "ResultCount";
        model.UseAcceptanceMinimum = true;
        model.UseAcceptanceMaximum = false;
        model.AcceptanceMinimumText = "2";
        blob.RunPreview();
        Drain(blob);
        Assert(Summary()?.Metrics["ResultCount"] == 1 && Summary()?.IsAcceptanceNg == true && Read<string>("ResultKind") == "Ng",
            "Result: real one-region SDK result fails the independent minimum of two");
        AssertRendered(blob, "Ng");
        capture("Blob-acceptance-NG");
        model.AcceptanceMinimumText = "1";
        Assert(Read<bool>("IsResultPrevious"), "Result: acceptance edit marks the existing result Previous");
        blob.RunPreview();
        Drain(blob);
        Assert(Summary()?.Success == true && Read<string>("ResultKind") == "Ok", "Result: real rerun recovers to acceptance OK");
        AssertRendered(blob, "Ok");
        capture("Blob-acceptance-OK");
        model.SetRoute(GetGeneration(model), "Result_OwnedMissingInput", string.Empty, model.OutputLayer);
        blob.RunPreview();
        Drain(blob);
        Assert(Read<string>("ResultKind") == "Error" && Summary()?.IsToolError == true && !string.IsNullOrEmpty(Read<string>("ResultReason")),
            "Result: unavailable input reports an error and replaces the prior success");
        capture("Blob-input-error");
        model.SetRoute(GetGeneration(model), "Main", string.Empty, model.OutputLayer);
        blob.RunPreview();
        Drain(blob);
        Assert(Read<string>("ResultKind") == "Ok", "Result: valid-input retry recovers after failure");
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, false);
        pump(5);
        AssertRendered(blob, "Ok");
        capture("Blob-English-current");
        VisionPipelineStepResultSummary retainedResult = Summary()!;
        int beforeSkippedFixtureRuns = blob.PreviewRunCount;
        try
        {
            foreach (OpenVisionLanguage language in new[] { OpenVisionLanguage.Korean, OpenVisionLanguage.English })
            {
                OpenVisionLanguageService.SetLanguage(language, false);
                model.PublishResult(GetGeneration(model), new VisionPipelineStepResultSummary
                { ToolType = "Blob", Executed = false, Skipped = true, ExecutionState = VisionPipelineResultSummaryService.NotRunAfterFailureState });
                pump(5);
                string expected = language == OpenVisionLanguage.Korean ? "미실행" : "Not run";
                Assert(model.HasResult && model.ResultKind == "NotRun" && model.ResultStatusText == expected, "Result: after-failure skipped caption remains " + language);
                AssertRendered(blob, "NotRun", expected);
                capture("Blob-" + language + "-skipped-status-fixture");
            }
        }
        finally { model.PublishResult(GetGeneration(model), retainedResult); }
        Assert(blob.PreviewRunCount == beforeSkippedFixtureRuns, "Result: skipped-caption fixture never executes the tool");
        AssertFiles(files, recipeDirectory, "Result: NG/error/recovery/language change never save XML");
        File.WriteAllText(Path.ChangeExtension(outputPath, ".result-contract.txt"),
            "PASS: real Threshold/Arithmetic/Blob SDK -> managed result -> rendered ko/en; Done/NG/OK/Error/Previous; edit/Undo/add inheritance/retry; route/language/add/Undo never execute or save; Threshold edit fixture cancels its existing auto scheduler.\n"
            + "Boundary: in-process WPF Wide/Compact; actual desktop interaction/themes/DPI/held press/4-split unverified.\n"
            + "Threshold SDK metrics: " + string.Join("; ", first.Metrics.Select(metric => metric.Key + "=" + metric.Value.ToString(CultureInfo.InvariantCulture))));

        T Read<T>(string name) => (T)typeof(OpenVisionWorkbenchToolViewModel).GetProperty(name,
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)!.GetValue(model)!;
        VisionPipelineStepResultSummary? Summary() => Read<VisionPipelineStepResultSummary?>("CurrentResult");
        void Drain(OpenVisionNativeToolDocument document)
        {
            System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
            while (document.IsPreviewRunning && timer.Elapsed < TimeSpan.FromSeconds(8)) { pump(2); System.Threading.Thread.Sleep(5); }
            pump(8);
            Assert(!document.IsPreviewRunning, "Result: owned SDK execution completes");
        }
        void AssertGray(OpenVisionNativeToolDocument document, VisionToolPreviewImageRole role, string name, int expected)
        {
            string export = Path.ChangeExtension(outputPath, "." + name + ".png");
            Assert(document.SavePreviewImageToFileForTest(role, export), "Result: export actual " + name);
            using System.Drawing.Bitmap bitmap = new(export);
            System.Drawing.Color actual = bitmap.GetPixel(0, 0);
            Assert(bitmap.Width == 32 && bitmap.Height == 32 && actual.R == expected && actual.G == expected && actual.B == expected,
                "Result: " + name + " actual pixels " + actual + ", expected gray " + expected);
        }
        void CancelThresholdSchedule() => ((VisionToolDebouncedPreviewScheduler)typeof(ThresholdToolWpfView)
            .GetField("previewScheduler", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(threshold.View)!).Cancel();
        void AssertRendered(OpenVisionNativeToolDocument document, string kind, string? expectedStatus = null)
        {
            FrameworkElement shell = (FrameworkElement)document.View.FindName("toolShell");
            ContentControl result = (ContentControl)shell.FindName("workbenchResultHost");
            pump(3);
            Rect resultBounds = result.TransformToAncestor(shell).TransformBounds(new Rect(result.RenderSize));
            FrameworkElement footer = (FrameworkElement)shell.FindName("actionPanel");
            double footerTop = footer.TranslatePoint(new Point(), shell).Y;
            bool resultVisible = resultBounds.Top >= 0 && resultBounds.Bottom <= footerTop && resultBounds.Left >= 0 && resultBounds.Right <= shell.ActualWidth;
            if (shell is VisionToolSingleInputPropertyToolShell single && single.PropertyGridHostVisibility == Visibility.Visible)
            {
                FrameworkElement parameters = single.PropertyGridHost;
                FrameworkElement body = (FrameworkElement)shell.FindName("parameterBodyScrollViewer");
                Rect visibleParameters = parameters.TransformToAncestor(shell).TransformBounds(new Rect(parameters.RenderSize));
                visibleParameters.Intersect(body.TransformToAncestor(shell).TransformBounds(new Rect(body.RenderSize)));
                if (visibleParameters.Height < 160) { capture(document.ToolName + "-" + kind + "-parameter-viewport-NG"); }
                Assert(visibleParameters.Height >= 160, "Result: actual parameter viewport remains160px with the result visible: " + visibleParameters);
            }
            if (!resultVisible) { capture(document.ToolName + "-" + kind + "-result-viewport-NG"); }
            Assert(resultVisible, "Result: card is inside the visible tool body before the pinned footer: " + resultBounds + "; footer=" + footerTop);
            if (kind == "Ng") { Assert(!string.IsNullOrWhiteSpace(Read<string>("ResultSummaryText")) && Read<bool>("HasResultHint"), "Result: NG keeps actual metrics summary and existing next action"); }
            Assert(result.IsVisible && Program.FindVisualChildren<TextBlock>(result).Any(text => text.IsVisible
                && text.Text == (expectedStatus ?? OpenVisionLanguageService.T("Workbench.Status." + kind))), "Result: actual visible status matches expected " + kind);
        }
    }

    internal static void RunTestFooter(OpenVisionShellHostView host, string outputPath, Action<int> pump, Action<string> capture)
    {
        OpenVisionWorkbenchPipelineViewModel pipeline = host.Workbench.Pipeline;
        string recipeDirectory = RecipeWorkspaceService.GetRecipeWorkspaceDirectory(host.ActiveRecipeContextNameForTest);
        Dictionary<string, string> files = SnapshotFiles(recipeDirectory);
        int layers = host.LayerDocumentCount;
        List<string> checks = new();
        VISION_MENU[] menus = { VISION_MENU.Matching, VISION_MENU.EdgeBasedMatching, VISION_MENU.FeatureMatching,
            VISION_MENU.AffineTransform, VISION_MENU.Filter, VISION_MENU.Threshold, VISION_MENU.RotateAndScale,
            VISION_MENU.Morphology, VISION_MENU.Arithmetic, VISION_MENU.EdgeDetection, VISION_MENU.HSV,
            VISION_MENU.Histogram, VISION_MENU.Blob, VISION_MENU.Contour, VISION_MENU.Line, VISION_MENU.Mean };
        foreach (OpenVisionLanguage language in new[] { OpenVisionLanguage.Korean, OpenVisionLanguage.English })
        {
            OpenVisionLanguageService.SetLanguage(language, false);
            foreach (VISION_MENU menu in menus)
            {
                host.SelectToolForTest(menu);
                pump(8);
                OpenVisionNativeToolDocument document = host.ActiveNativeDocumentForTest!;
                OpenVisionWorkbenchToolViewModel model = host.Workbench.Tool;
                FrameworkElement shell = (FrameworkElement)document.View.FindName("toolShell");
                Button defaults = (Button)shell.FindName("btnWorkbenchDefaults");
                Button add = (Button)shell.FindName("btnAddPipeline");
                Button run = shell is VisionToolDoubleInputCustomToolShell pair && pair.RunOffsetButton.IsVisible
                    ? pair.RunOffsetButton : (Button)shell.FindName("btnRunPreview");
                TextBlock shortcut = (TextBlock)shell.FindName(run.Name == "btnRunOffset" ? "txtWorkbenchRunOffsetShortcut" : "txtWorkbenchRunShortcut");
                if (shell is VisionToolSingleInputPropertyToolShell route)
                {
                    Assert(route.IsDockedInspectorMode && !route.InputPreviewFrame.IsVisible && !route.OutputPreviewFrame.IsVisible
                        && !route.CreateOutputLayerButton.IsVisible, menu + ": inline route omits thumbnail/action cards");
                    Rect inputBounds = route.InputLayerGroup.TransformToAncestor(route).TransformBounds(new Rect(route.InputLayerGroup.RenderSize));
                    Rect outputBounds = route.OutputLayerGroup.TransformToAncestor(route).TransformBounds(new Rect(route.OutputLayerGroup.RenderSize));
                    Assert(inputBounds.Height <= 32 && outputBounds.Height <= 32 && Math.Abs(inputBounds.Top - outputBounds.Top) < 1
                        && inputBounds.Right <= outputBounds.Left && outputBounds.Right <= route.ActualWidth + 1,
                        menu + ": inline input/output stay on one row: " + inputBounds + "/" + outputBounds);
                    Assert((string)route.InputLayerGroup.Header == OpenVisionLanguageService.T("Workbench.Tool.Input")
                        && (string)route.OutputLayerGroup.Header == OpenVisionLanguageService.T("Workbench.Tool.Output"), menu + ": route captions follow the current language");
                    route.InputLayerComboBox.ApplyTemplate();
                    route.InputLayerComboBox.UpdateLayout();
                    if (model.InputLayer == "Main")
                    {
                        string renderedLayers = string.Join("/", Program.FindVisualChildren<TextBlock>(route.InputLayerComboBox).Select(text => $"{text.Text}:{text.IsVisible}; data={text.DataContext}; triggers={text.Style?.Triggers.Count}; local={text.ReadLocalValue(TextBlock.TextProperty)}; label={text.TryFindResource("VisionTool.WorkbenchOriginalText")}"));
                        string layerDiagnostic = $"{menu}: label={route.InputLayerGroup.TryFindResource("VisionTool.WorkbenchOriginalText")}; raw={VisionToolLayerComboHelper.GetLayerText(route.InputLayerComboBox)}; selected={route.InputLayerComboBox.SelectedItem}; editable={route.InputLayerComboBox.IsEditable}; template={route.InputLayerComboBox.ItemTemplate == route.InputLayerComboBox.TryFindResource("VisionTool.WorkbenchLayerNameTemplate")}; selectionTemplate={route.InputLayerComboBox.SelectionBoxItemTemplate == route.InputLayerComboBox.ItemTemplate}; rendered={renderedLayers}";
                        File.AppendAllLines(Path.ChangeExtension(outputPath, ".route-labels.txt"), new[] { layerDiagnostic });
                        if (menu == VISION_MENU.Matching) { capture(language + "-Matching-inline-route-label"); }
                        Assert(Program.FindVisualChildren<TextBlock>(route.InputLayerComboBox).Any(text => text.IsVisible
                            && text.Text == OpenVisionLanguageService.T("Workbench.Pane.Original"))
                            && VisionToolLayerComboHelper.GetLayerText(route.InputLayerComboBox) == "Main",
                            menu + ": original input label preserves the raw layer key: " + layerDiagnostic);
                    }
                    System.Windows.Shapes.Path routeGlyph = (System.Windows.Shapes.Path)route.InputLayerComboBox.Template.FindName("comboDropGlyph", route.InputLayerComboBox);
                    Assert(routeGlyph.Fill is SolidColorBrush routeBrush && routeBrush.Color.A == 255,
                        menu + ": inline input arrow remains opaque");
                    if (model.HasAcceptance)
                    {
                        ComboBox metric = Program.FindVisualChildren<ComboBox>(route.WorkbenchAcceptanceHost).Single(control => AutomationProperties.GetAutomationId(control) == "WorkbenchToolAcceptanceMetric");
                        string metricDiagnostic = $"{language}/{menu}: selected={metric.SelectedItem}; expected={model.SelectedAcceptanceMetric}; errors={string.Join("/", Validation.GetErrors(metric).Select(error => error.ErrorContent))}; rendered={string.Join("/", Program.FindVisualChildren<TextBlock>(metric).Select(text => text.Text))}";
                        File.AppendAllLines(Path.ChangeExtension(outputPath, ".route-metrics.txt"), new[] { metricDiagnostic });
                        bool metricReady = !Validation.GetHasError(metric) && metric.SelectedItem is KeyValuePair<string, string> selectedMetric
                            && selectedMetric.Key == model.AcceptanceMetricName
                            && Program.FindVisualChildren<TextBlock>(metric).Any(text => text.IsVisible && text.Text == selectedMetric.Value);
                        if (!metricReady) { capture(language + "-" + menu + "-route-metric-transition"); }
                        Assert(metricReady, menu + ": acceptance metric survives tool transition: " + metricDiagnostic);
                    }
                    if (route.PropertyGridHost.IsVisible)
                    {
                        if (menu == VISION_MENU.EdgeBasedMatching) { capture(language + "-EdgeBasedMatching-inline-route-viewport"); }
                        double top = route.PropertyGridHost.TranslatePoint(new Point(), route).Y;
                        double bottom = route.WorkbenchAcceptanceHost.IsVisible
                            ? route.WorkbenchAcceptanceHost.TranslatePoint(new Point(), route).Y : route.SummaryHost.TranslatePoint(new Point(), route).Y;
                        File.AppendAllLines(Path.ChangeExtension(outputPath, ".route-viewport.txt"), new[]
                        { $"{language}/{menu}: property-height={route.PropertyGridHost.ActualHeight:0.0}; available={bottom - top - 4:0.0}; shell-height={route.ActualHeight:0.0}" });
                        ScrollViewer body = (ScrollViewer)route.FindName("parameterBodyScrollViewer");
                        Rect visibleParameters = route.PropertyGridHost.TransformToAncestor(route).TransformBounds(new Rect(route.PropertyGridHost.RenderSize));
                        visibleParameters.Intersect(body.TransformToAncestor(route).TransformBounds(new Rect(body.RenderSize)));
                        Assert(visibleParameters.Height >= 160, menu + ": at least160px actually visible parameter viewport: " + visibleParameters);
                        Assert(route.PropertyGridHost.ActualHeight >= 160 && top + route.PropertyGridHost.ActualHeight <= bottom - 3,
                            menu + ": at least160px visible parameter viewport before acceptance: " + route.PropertyGridHost.ActualHeight);
                    }
                    if (menu is VISION_MENU.Blob or VISION_MENU.Line or VISION_MENU.Mean) { capture(language + "-" + menu + "-inline-route"); }
                }
                else if (shell is VisionToolDoubleInputCustomToolShell routePair)
                {
                    Rect inputA = routePair.InputAGroup.TransformToAncestor(routePair).TransformBounds(new Rect(routePair.InputAGroup.RenderSize));
                    Rect output = routePair.OutputLayerGroup.TransformToAncestor(routePair).TransformBounds(new Rect(routePair.OutputLayerGroup.RenderSize));
                    Assert(!routePair.InputAPreviewFrame.IsVisible && !routePair.OutputPreviewFrame.IsVisible
                        && !routePair.CreateOutputLayerButton.IsVisible && output.Top >= inputA.Bottom + 3,
                        "Arithmetic: inline inputs above full-width output");
                    Assert((string)routePair.InputAGroup.Header == OpenVisionLanguageService.T("Workbench.Tool.InputA")
                        && (string)routePair.InputBGroup.Header == OpenVisionLanguageService.T("Workbench.Tool.InputB")
                        && (string)routePair.OutputLayerGroup.Header == OpenVisionLanguageService.T("Workbench.Tool.Output"),
                        "Arithmetic: route captions follow the current language");
                    if (routePair.InputBGroup.IsVisible)
                    {
                        Rect inputB = routePair.InputBGroup.TransformToAncestor(routePair).TransformBounds(new Rect(routePair.InputBGroup.RenderSize));
                        Assert(Math.Abs(inputA.Top - inputB.Top) < 1 && inputA.Right + 3 <= inputB.Left
                            && routePair.LoadInputAImageButton.IsVisible && routePair.LoadInputBImageButton.IsVisible,
                            "Arithmetic: A/B side by side with explicit image-load controls");
                    }
                }
                Assert(defaults.IsVisible && defaults.IsEnabled && ReferenceEquals(defaults.Command, model.RestoreDefaultsCommand)
                    && ReferenceEquals(defaults.DataContext, model), menu + ": defaults binding and mode");
                Assert(add.IsVisible && run.IsVisible && shortcut.IsVisible && shortcut.Text == "F8", menu + ": test action/F8 visibility");
                Assert(add.IsEnabled == (menu is not VISION_MENU.HSV and not VISION_MENU.Histogram)
                    && (string)add.ToolTip == model.AddTooltip, menu + ": supported add/tooltip state");
                List<Button> buttons = new() { defaults };
                if (shell.FindName("btnNImageVerification") is Button nImage && nImage.IsVisible)
                {
                    Assert(shell.FindName("txtNImageVerificationText") is TextBlock label
                        && label.Text == OpenVisionLanguageService.T("Workbench.Tool.NTest"), menu + ": existing N-image entry caption");
                    buttons.Add(nImage);
                }
                buttons.Add(add);
                buttons.Add(run);
                Rect previous = Rect.Empty;
                foreach (Button button in buttons)
                {
                    Rect bounds = button.TransformToAncestor(shell).TransformBounds(new Rect(button.RenderSize));
                    Assert(bounds.Width > 0 && bounds.Height >= 30 && bounds.Left >= -1 && bounds.Right <= shell.ActualWidth + 1
                        && bounds.Bottom <= shell.ActualHeight + 1, menu + ": action inside shell: " + bounds);
                    if (!previous.IsEmpty)
                    {
                        Assert(previous.Right <= bounds.Left + 1 && Math.Abs(previous.Top - bounds.Top) < 2,
                            menu + ": left/right test actions must share a non-overlapping row");
                    }
                    previous = bounds;
                    foreach (TextBlock text in ((StackPanel)button.Content).Children.OfType<TextBlock>())
                    {
                        text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        double width = text.DesiredSize.Width - text.Margin.Left - text.Margin.Right;
                        Assert(width <= text.ActualWidth + 1, menu + ": footer caption must fit: " + text.Text);
                    }
                }
                int count = pipeline.StepCount;
                int runs = document.PreviewRunCount;
                if (add.IsEnabled)
                {
                    add.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    pump(3);
                    ContextMenu popup = add.ContextMenu;
                    Assert(popup.IsOpen && pipeline.StepCount == count && ReferenceEquals(popup.DataContext, model), menu + ": opening add menu must not insert");
                    MenuItem after = (MenuItem)popup.Items[0];
                    MenuItem before = (MenuItem)popup.Items[1];
                    MenuItem end = (MenuItem)popup.Items[2];
                    Assert((string)after.Header == model.AddAfterText && after.InputGestureText == "Ctrl+Enter"
                        && ReferenceEquals(after.Command, model.AddAfterCommand) && ReferenceEquals(before.Command, model.AddBeforeCommand)
                        && ReferenceEquals(end.Command, model.AddEndCommand), menu + ": bound three-position menu");
                    Assert(!before.IsEnabled && !model.AddBeforeCommand.CanExecute(null), menu + ": before requires a selection");
                    model.AddBeforeCommand.Execute(null);
                    Assert(pipeline.StepCount == count, menu + ": forced disabled before must not add at end");
                    popup.IsOpen = false;
                    pump(2);
                    if (menu is VISION_MENU.Matching or VISION_MENU.EdgeBasedMatching or VISION_MENU.FeatureMatching)
                    {
                        model.AddAfterCommand.Execute(null);
                        Assert(pipeline.StepCount == count && model.StatusText == OpenVisionLanguageService.T("Workbench.Tool.TemplateMissing"), menu + ": missing template rejects menu add");
                    }
                }
                defaults.Command.Execute(null);
                pump(3);
                Assert(document.PreviewRunCount == runs && host.LayerDocumentCount == layers && pipeline.StepCount == count,
                    menu + ": defaults/menu state must not execute, create layers or change pipeline");
                AssertFiles(files, recipeDirectory, menu + ": defaults/menu state must not save");
                checks.Add(language + "/" + menu + ": footer bounds/labels/F8/defaults/menu/guards/no run/layer/write PASS");
                if (menu is VISION_MENU.Threshold or VISION_MENU.Arithmetic or VISION_MENU.HSV or VISION_MENU.Histogram)
                {
                    capture(language + "-" + menu + "-test-footer");
                }
            }
        }

        host.SelectToolForTest(VISION_MENU.Mean);
        pump(6);
        OpenVisionNativeToolDocument mean = host.ActiveNativeDocumentForTest!;
        OpenVisionWorkbenchToolViewModel tool = host.Workbench.Tool;
        string inputPath = Path.ChangeExtension(outputPath, ".busy-input.png");
        using (System.Drawing.Bitmap input = new(32, 32))
        {
            using System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(input);
            graphics.Clear(System.Drawing.Color.FromArgb(128, 128, 128));
            input.Save(inputPath, System.Drawing.Imaging.ImageFormat.Png);
        }
        Assert(mean.LoadPreviewImageFromFileForTest(VisionToolPreviewImageRole.InputA, inputPath), "Footer: prepare owned busy input");
        pump(4);
        layers = host.LayerDocumentCount;
        FrameworkElement meanShell = (FrameworkElement)mean.View.FindName("toolShell");
        Button meanDefaults = (Button)meanShell.FindName("btnWorkbenchDefaults");
        Button meanAdd = (Button)meanShell.FindName("btnAddPipeline");
        Button meanRun = (Button)meanShell.FindName("btnRunPreview");
        Button meanNImage = (Button)meanShell.FindName("btnNImageVerification");
        int beforeProbeRuns = mean.PreviewRunCount;
        void DrainPreview()
        {
            System.Diagnostics.Stopwatch elapsed = System.Diagnostics.Stopwatch.StartNew();
            while (mean.IsPreviewRunning && elapsed.Elapsed < TimeSpan.FromSeconds(5)) { pump(2); System.Threading.Thread.Sleep(5); }
            pump(5);
        }
        using (System.Threading.ManualResetEventSlim releaseWorker = new(false))
        {
            mean.SetPreviewExecutionOverrideForTest(_ =>
            {
                if (!releaseWorker.Wait(TimeSpan.FromSeconds(10))) { throw new InvalidOperationException("Owned footer worker was not released."); }
                throw new InvalidOperationException("Owned footer failure probe.");
            });
            try
            {
                mean.RunPreview();
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                pump(10);
                capture("busy-toolbar-state");
                Assert(mean.IsPreviewRunning && !meanDefaults.IsEnabled && !meanAdd.IsEnabled && !meanRun.IsEnabled && !meanNImage.IsEnabled,
                    "Footer: actual asynchronous busy state must disable defaults/add/run/N-image buttons");
                Border runChrome = (Border)meanRun.Template.FindName("buttonChrome", meanRun);
                File.WriteAllText(Path.ChangeExtension(outputPath, ".busy-render.txt"),
                    "Run visibility=" + meanRun.Visibility + "; IsVisible=" + meanRun.IsVisible + "; size=" + meanRun.RenderSize
                    + "; foreground=" + meanRun.Foreground + "; chrome background=" + runChrome.Background + "; chrome border=" + runChrome.BorderBrush);
                Assert(meanRun.IsVisible && meanRun.ActualWidth >= 30 && meanRun.Foreground is SolidColorBrush foreground && foreground.Color.A == 255
                    && runChrome.Background is SolidColorBrush background && background.Color.A == 255
                    && runChrome.BorderBrush is SolidColorBrush border && border.Color.A == 255,
                    "Footer: disabled run caption/background/border must stay visible and opaque");
                Assert(((SolidColorBrush)meanRun.Foreground).Color == ((SolidColorBrush)meanRun.FindResource("OpenVision.Workbench.MutedTextBrush")).Color
                    && ((SolidColorBrush)runChrome.Background).Color == ((SolidColorBrush)meanRun.FindResource("OpenVision.Workbench.PanelAltBrush")).Color
                    && ((SolidColorBrush)runChrome.BorderBrush).Color == ((SolidColorBrush)meanRun.FindResource("OpenVision.Workbench.StrongLineBrush")).Color,
                    "Footer: disabled run must use the existing semantic foreground/background/border");
                Assert(!tool.AddAfterCommand.CanExecute(null) && !tool.AddBeforeCommand.CanExecute(null) && !tool.AddEndCommand.CanExecute(null), "Footer: actual asynchronous busy state disables insertion commands");
                tool.AddAfterCommand.Execute(null);
                tool.AddBeforeCommand.Execute(null);
                tool.AddEndCommand.Execute(null);
                mean.RunPreview();
                Assert(pipeline.StepCount == 0 && mean.PreviewRunCount == beforeProbeRuns && host.LayerDocumentCount == layers,
                    "Footer: forced busy add/repeated run must not insert, publish or create layers");
            }
            finally
            {
                releaseWorker.Set();
                DrainPreview();
                mean.SetPreviewExecutionOverrideForTest(null);
            }
        }
        Assert(!mean.IsPreviewRunning && !mean.HasPreviewResult && mean.PreviewRunCount == beforeProbeRuns
            && mean.LastStatusText.Contains("Owned footer failure probe.", StringComparison.Ordinal)
            && meanDefaults.IsEnabled && meanAdd.IsEnabled && meanRun.IsEnabled && meanNImage.IsEnabled,
            "Footer: asynchronous failure recovers rendered actions without a result");
        capture("failure-recovered-toolbar-state");
        checks.Add("Actual async failure: disabled rendered defaults/add/run/N-image, guarded insertion/repeated run, recovered actions PASS");
        using (System.Threading.ManualResetEventSlim releaseWorker = new(false))
        {
            mean.SetPreviewExecutionOverrideForTest(_ =>
            {
                if (!releaseWorker.Wait(TimeSpan.FromSeconds(10))) { throw new InvalidOperationException("Owned discarded footer worker was not released."); }
                throw new InvalidOperationException("Owned discarded footer probe.");
            });
            try
            {
                mean.RunPreview();
                pump(8);
                Assert(mean.IsPreviewRunning && !meanDefaults.IsEnabled && !meanAdd.IsEnabled && !meanRun.IsEnabled && !meanNImage.IsEnabled,
                    "Footer: second asynchronous start updates rendered actions");
                mean.InvalidatePreviewResultForInputChange();
                Assert(mean.LastStatusText.Contains("input changed", StringComparison.Ordinal), "Footer: discard marks the input-change cancellation");
            }
            finally
            {
                releaseWorker.Set();
                DrainPreview();
                mean.SetPreviewExecutionOverrideForTest(null);
            }
        }
        Assert(!mean.IsPreviewRunning && !mean.HasPreviewResult && mean.PreviewRunCount == beforeProbeRuns
            && mean.LastStatusText.Contains("input changed", StringComparison.Ordinal) && host.LayerDocumentCount == layers
            && meanDefaults.IsEnabled && meanAdd.IsEnabled && meanRun.IsEnabled && meanNImage.IsEnabled,
            "Footer: discard without result callback must recover actions and reject stale publication");
        capture("discard-recovered-toolbar-state");
        mean.RunPreview();
        DrainPreview();
        Assert(!mean.IsPreviewRunning && mean.HasPreviewResult && mean.PreviewRunCount == beforeProbeRuns + 1
            && meanDefaults.IsEnabled && meanAdd.IsEnabled && meanRun.IsEnabled && meanNImage.IsEnabled,
            "Footer: valid native Mean after failure/discard must publish once and keep actions usable");
        capture("valid-recovered-toolbar-state");
        checks.Add("Actual async discard: callback skipped, actions restored, stale output rejected; valid native Mean recovery PASS");
        layers = host.LayerDocumentCount;
        int initialRuns = mean.PreviewRunCount;
        for (int i = 0; i < 3; i++)
        {
            if (tool.IsStepEditing) { tool.NewTestCommand.Execute(null); pump(5); }
            Assert(tool.SetRoute(GetGeneration(tool), "Main", string.Empty, tool.OutputLayer), "Footer: arrange valid independent input");
            tool.AddAfterCommand.Execute(null);
            pump(5);
            Assert(pipeline.StepCount == i + 1 && tool.IsStepEditing, "Footer: seed three steps through add command");
        }
        foreach ((int position, int selected, int expected) in new[] { (1, 1, 1), (0, 1, 2), (2, 1, 5) })
        {
            pipeline.SelectStepCommand.Execute(selected);
            pump(5);
            tool.NewTestCommand.Execute(null);
            pump(5);
            Assert(tool.SetRoute(GetGeneration(tool), "Main", string.Empty, tool.OutputLayer), "Footer: select valid new-test route");
            VisionPipelineStep draft = tool.CurrentStep!;
            draft.UseAcceptance = true;
            draft.AcceptanceMetricName = VisionPipelineKnownMetrics.GetMetricNamesForTool(draft.ToolType).First();
            draft.MaxElapsedMilliseconds = 777;
            draft.Parameters["Smoke.FooterCopy"] = "preserved";
            string output = draft.OutputLayer;
            string[] oldOrder = pipeline.WorkingCopyForTest.Steps.Select(step => step.OutputLayer).ToArray();
            var binding = (OpenVisionWorkbenchToolViewModel.EditorBinding)typeof(OpenVisionWorkbenchToolViewModel)
                .GetField("editor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(tool)!;
            Func<bool> running = binding.IsRunning;
            binding.IsRunning = () => true;
            try
            {
                Assert(!tool.AddAfterCommand.CanExecute(null) && !tool.AddBeforeCommand.CanExecute(null) && !tool.AddEndCommand.CanExecute(null), "Footer: busy disables every insertion command");
                tool.AddAfterCommand.Execute(null);
                tool.AddBeforeCommand.Execute(null);
                tool.AddEndCommand.Execute(null);
                Assert(pipeline.StepCount == oldOrder.Length, "Footer: forced busy commands preserve pipeline");
            }
            finally { binding.IsRunning = running; }
            System.Windows.Input.ICommand command = position == 1 ? tool.AddBeforeCommand : position == 2 ? tool.AddEndCommand : tool.AddAfterCommand;
            command.Execute(null);
            pump(6);
            VisionPipelineStep added = pipeline.GetStep(expected)!;
            Assert(pipeline.SelectedStepIndex == expected && added.OutputLayer == output && added.InputLayer == "Main"
                && added.UseAcceptance && added.MaxElapsedMilliseconds == 777 && added.Parameters["Smoke.FooterCopy"] == "preserved", "Footer: insertion position and full working-copy transfer");
            Assert(pipeline.WorkingCopyForTest.Steps.Where(step => step.OutputLayer != output).Select(step => step.OutputLayer).SequenceEqual(oldOrder), "Footer: existing order preserved");
            checks.Add("Position=" + position + "; selected=" + selected + "; inserted=" + expected + "; copy/old order/busy PASS");
        }
        Assert(mean.PreviewRunCount == initialRuns && host.LayerDocumentCount == layers, "Footer: insertions must not run or create layers");
        AssertFiles(files, recipeDirectory, "Footer: insertion is working-copy only");
        File.WriteAllText(Path.ChangeExtension(outputPath, ".test-footer.txt"), string.Join(Environment.NewLine, checks));
    }

    internal static void RunSavedStepRevert(OpenVisionShellHostView host, string outputPath, string templatePath, Action<int> pump, Action<string> capture)
    {
        OpenVisionWorkbenchPipelineViewModel pipeline = host.Workbench.Pipeline;
        for (int index = 0; index < 3; index++)
        {
            host.SelectToolForTest(VISION_MENU.Threshold);
            pump(6);
            OpenVisionNativeToolDocument document = host.ActiveNativeDocumentForTest!;
            ChangeParameter(document);
            document.RecordParameterChange(null);
            host.Workbench.Tool.CurrentStep!.OutputLayer = "Revert_" + index;
            Assert(document.AddPipelineStep() != null, "Revert: arrange three distinct steps");
            pump(6);
            Assert(!host.Workbench.Tool.RevertCommand.CanExecute(null), "Revert: a new insertion has no saved baseline");
        }
        Assert(pipeline.TrySave(), "Revert: baseline save");
        string recipeDirectory = RecipeWorkspaceService.GetRecipeWorkspaceDirectory(host.RecipeCommands.SelectedRecipeName);
        Dictionary<string, string> files = Directory.EnumerateFiles(recipeDirectory, "*", SearchOption.AllDirectories)
            .ToDictionary(path => path, path => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))));
        int layers = host.LayerDocumentCount;
        pipeline.SelectStepCommand.Execute(1);
        pump(6);
        OpenVisionNativeToolDocument active = host.ActiveNativeDocumentForTest!;
        string nativeSaved = ParameterState(active);
        VisionPipelineStep saved = pipeline.GetStep(1)!;
        int runs = active.PreviewRunCount;
        ChangeParameter(active);
        active.RecordParameterChange(null);
        pipeline.RenameCommand.Execute(null);
        pipeline.RenameText = "Revert renamed";
        pipeline.CommitRenameCommand.Execute(null);
        pump(5);
        string edited = StepXml(pipeline.GetStep(1));
        capture("renamed-before-revert");
        OpenVisionWorkbenchToolViewModel model = host.Workbench.Tool;
        FrameworkElement shell = (FrameworkElement)active.View.FindName("toolShell");
        Button revertButton = (Button)shell.FindName("btnWorkbenchRevert");
        Button runToButton = (Button)shell.FindName("btnWorkbenchRunTo");
        Assert(revertButton.IsVisible && revertButton.IsEnabled && ReferenceEquals(revertButton.Command, model.RevertCommand)
            && runToButton.IsVisible && ReferenceEquals(runToButton.Command, model.RunToCommand), "Revert: rendered command bindings");
        List<string> buttonChecks = new();
        List<string> footerChecks = new();
        foreach (string theme in new[] { "Dark", "Light" })
        {
            ResourceDictionary palette = new() { Source = new Uri($"pack://application:,,,/OpenVisionLab;component/UI/Wpf/OpenVisionWorkbench{theme}.xaml") };
            host.Resources.MergedDictionaries.Add(palette);
            shell.Resources.MergedDictionaries.Add(palette);
            try
            {
                foreach (OpenVisionLanguage language in new[] { OpenVisionLanguage.Korean, OpenVisionLanguage.English })
                {
                    OpenVisionLanguageService.SetLanguage(language, false);
                    pump(5);
                    TextBlock revertText = ((StackPanel)revertButton.Content).Children.OfType<TextBlock>().Single();
                    TextBlock runToText = ((StackPanel)runToButton.Content).Children.OfType<TextBlock>().Single();
                    Assert(revertText.Text == (language == OpenVisionLanguage.Korean ? "되돌리기" : "Revert")
                        && runToText.Text == (language == OpenVisionLanguage.Korean ? "이 단계까지" : "Run to here"), "Revert: exact Appendix C captions");
                    foreach (Button button in new[] { revertButton, runToButton })
                    {
                        string foregroundKey = button.IsEnabled ? "OpenVision.Workbench.TextBrush" : "OpenVision.Workbench.MutedTextBrush";
                        Assert(((SolidColorBrush)button.Foreground).Color == ((SolidColorBrush)palette[foregroundKey]).Color
                            && ((SolidColorBrush)button.Background).Color == ((SolidColorBrush)palette["OpenVision.Workbench.PanelAltBrush"]).Color,
                            "Revert: the native shell must actually use the selected focused palette");
                        buttonChecks.Add($"{theme}/{language}/{button.Name}: " + AssertActionButton(button));
                    }
                    capture("renamed-" + theme + "-" + language);
                }
            }
            finally
            {
                shell.Resources.MergedDictionaries.Remove(palette);
                host.Resources.MergedDictionaries.Remove(palette);
            }
        }
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        pump(4);
        Assert(model.RevertCommand.CanExecute(null), "Revert: renamed saved step must keep its identity");
        var binding = (OpenVisionWorkbenchToolViewModel.EditorBinding)typeof(OpenVisionWorkbenchToolViewModel)
            .GetField("editor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(model)!;
        Func<bool> isRunning = binding.IsRunning;
        binding.IsRunning = () => true;
        try
        {
            Assert(!model.RevertCommand.CanExecute(null) && !model.RunToCommand.CanExecute(null), "Revert: native running disables both commands");
            model.RevertCommand.Execute(null);
            Assert(StepXml(pipeline.GetStep(1)) == edited, "Revert: forced command during a run must not mutate");
        }
        finally { binding.IsRunning = isRunning; }
        model.RevertCommand.Execute(null);
        pump(6);
        Assert(StepXml(pipeline.GetStep(1)) == StepXml(saved) && ParameterState(active) == nativeSaved,
            "Revert: restore full saved step and rendered native parameter state");
        Assert(!revertButton.IsEnabled && !model.RevertCommand.CanExecute(null), "Revert: unchanged saved step disables restoration");
        buttonChecks.Add("Restored/" + revertButton.Name + ": " + AssertActionButton(revertButton));
        capture("renamed-reverted");
        pipeline.UndoCommand.Execute(null);
        pump(5);
        Assert(StepXml(pipeline.GetStep(1)) == edited, "Revert: undo restoration");
        pipeline.RedoCommand.Execute(null);
        pump(5);
        Assert(StepXml(pipeline.GetStep(1)) == StepXml(saved), "Revert: redo restoration");
        pipeline.MoveSelectedTo(2);
        pump(6);
        Assert(pipeline.SelectedStepIndex == 2 && pipeline.SelectedRow?.IsDirty == false, "Revert: move retains saved-value identity");
        VisionPipelineStep changed = pipeline.GetStep(2)!;
        changed.Parameters["Smoke.Revert"] = "changed";
        pipeline.ReplaceStep(2, changed);
        Assert(pipeline.RevertSavedStep(2) && StepXml(pipeline.GetStep(2)) == StepXml(saved), "Revert: moved step restores its own saved values");
        pipeline.SelectStepCommand.Execute(0);
        host.QueuePendingRecipeEditDecisionForTest(OpenVisionRecipePendingEditDecision.ApplyAndContinue);
        pipeline.DeleteCommand.Execute(null);
        pump(6);
        pipeline.SelectStepCommand.Execute(1);
        pump(6);
        changed = pipeline.GetStep(1)!;
        changed.Parameters["Smoke.Revert"] = "changed after deletion";
        pipeline.ReplaceStep(1, changed);
        Assert(pipeline.RevertSavedStep(1) && StepXml(pipeline.GetStep(1)) == StepXml(saved), "Revert: deleting an earlier step retains identity");
        pipeline.DuplicateCommand.Execute(null);
        pump(6);
        string duplicated = StepXml(pipeline.WorkingCopyForTest);
        Assert(!pipeline.RevertSavedStep(pipeline.SelectedStepIndex) && StepXml(pipeline.WorkingCopyForTest) == duplicated,
            "Revert: an unsaved duplicate must never revert to the source step");
        pipeline.UndoCommand.Execute(null);
        pump(5);
        pipeline.RedoCommand.Execute(null);
        pump(5);
        Assert(!pipeline.RevertSavedStep(pipeline.SelectedStepIndex), "Revert: duplicate identity survives undo/redo");
        AssertFiles(files, recipeDirectory, "Revert: all restoration/history operations before explicit save");
        Assert(active.PreviewRunCount == runs && host.LayerDocumentCount == layers, "Revert: no execution or layer changes");
        changed = pipeline.GetStep(pipeline.SelectedStepIndex)!;
        changed.OutputLayer = "Revert_SavedDuplicate";
        pipeline.ReplaceStep(pipeline.SelectedStepIndex, changed);
        Assert(pipeline.TrySave(), "Revert: save new baseline including duplicate");
        VisionPipelineStep duplicateSaved = pipeline.GetStep(pipeline.SelectedStepIndex)!;
        changed = pipeline.GetStep(pipeline.SelectedStepIndex)!;
        changed.Parameters["Smoke.Revert"] = "after save";
        pipeline.ReplaceStep(pipeline.SelectedStepIndex, changed);
        Assert(pipeline.RevertSavedStep(pipeline.SelectedStepIndex) && StepXml(pipeline.GetStep(pipeline.SelectedStepIndex)) == StepXml(duplicateSaved),
            "Revert: explicit save makes the duplicate its own saved step");
        files = Directory.EnumerateFiles(recipeDirectory, "*", SearchOption.AllDirectories)
            .ToDictionary(path => path, path => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))));
        changed = pipeline.GetStep(pipeline.SelectedStepIndex)!;
        changed.Parameters["Smoke.Revert"] = "save failure";
        pipeline.ReplaceStep(pipeline.SelectedStepIndex, changed);
        host.FailNextRecipeStepSaveForTest();
        Assert(!pipeline.TrySave() && pipeline.RevertSavedStep(pipeline.SelectedStepIndex)
            && StepXml(pipeline.GetStep(pipeline.SelectedStepIndex)) == StepXml(duplicateSaved), "Revert: failed save must retain the last successful baseline");
        AssertFiles(files, recipeDirectory, "Revert: failed save and recovery");
        pump(6);
        Assert(pipeline.StatusText == OpenVisionLanguageService.T("Workbench.Step.Reverted"), "Revert: recovery replaces the failed-save status");
        foreach (VISION_MENU menu in new[] { VISION_MENU.Arithmetic, VISION_MENU.Line, VISION_MENU.Matching })
        {
            host.SelectToolForTest(menu);
            pump(10);
            OpenVisionNativeToolDocument document = host.ActiveNativeDocumentForTest!;
            if (menu == VISION_MENU.Matching) { host.SetActiveMatchingTemplatePathForTest(templatePath); }
            ChangeParameter(document);
            document.RecordParameterChange(null);
            host.Workbench.Tool.CurrentStep!.OutputLayer = "Revert_" + menu;
            host.Workbench.Tool.CurrentStep.UseAcceptance = true;
            host.Workbench.Tool.CurrentStep.MaxElapsedMilliseconds = 987;
            host.Workbench.Tool.CurrentStep.Parameters["Smoke.Stored"] = "saved extension";
            if (menu == VISION_MENU.Arithmetic) { host.Workbench.Tool.CurrentStep.Parameters["InputLayerB"] = "Main"; }
            Assert(document.AddPipelineStep() != null && pipeline.TrySave(), menu + ": saved-step fixture");
            pump(6);
            int index = pipeline.SelectedStepIndex;
            VisionPipelineStep original = pipeline.GetStep(index)!;
            string parameters = ParameterState(document);
            int beforeRuns = document.PreviewRunCount;
            int beforeLayers = host.LayerDocumentCount;
            files = Directory.EnumerateFiles(recipeDirectory, "*", SearchOption.AllDirectories)
                .ToDictionary(path => path, path => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))));
            ChangeParameter(document);
            document.RecordParameterChange(null);
            changed = pipeline.GetStep(index)!;
            changed.UseAcceptance = false;
            changed.MaxElapsedMilliseconds = 654;
            changed.Parameters["Smoke.Stored"] = "changed extension";
            pipeline.ReplaceStep(index, changed);
            pump(5);
            Assert(host.Workbench.Tool.RevertCommand.CanExecute(null), menu + ": changed saved step can revert");
            host.Workbench.Tool.RevertCommand.Execute(null);
            pump(8);
            Assert(StepXml(pipeline.GetStep(index)) == StepXml(original) && ParameterState(document) == parameters,
                menu + ": native editor/template/secondary input/acceptance/extension restoration");
            Assert(document.PreviewRunCount == beforeRuns && host.LayerDocumentCount == beforeLayers, menu + ": revert must not run or change layers");
            AssertFiles(files, recipeDirectory, menu + ": revert must not write recipe files");
            shell = (FrameworkElement)document.View.FindName("toolShell");
            foreach (string name in new[] { "btnWorkbenchRevert", "btnWorkbenchRunTo" })
                buttonChecks.Add(menu + "/" + name + ": " + AssertActionButton((Button)shell.FindName(name)));
            capture("restored-" + menu);
            if (document.View is ArithmeticToolWpfView arithmetic)
            {
                VisionToolDoubleInputCustomToolShell pair = (VisionToolDoubleInputCustomToolShell)shell;
                footerChecks.Add("Saved operation: " + AssertStepActionRow(pair, pair.RunPreviewButton));
                CheckArithmeticDockCycle("operation", pair.RunPreviewButton);
                VisionPipelineStep offset = VisionPipelineStepEditService.CopyStep(original);
                offset.Parameters[VisionPipelineArithmeticStep.ParameterMode] = VisionPipelineArithmeticStep.ModeOffset;
                pipeline.ReplaceStep(index, offset);
                pump(8);
                Assert(arithmetic.UseOffsetMode && pair.RunOffsetButton.IsVisible && !pair.RunPreviewButton.IsVisible
                    && pair.RunOffsetText.Text == host.Workbench.Tool.RunText, "Arithmetic: actual offset mode and Workbench run caption");
                footerChecks.Add("Changed offset: " + AssertStepActionRow(pair, pair.RunOffsetButton));
                capture("Arithmetic-offset-actions");
                CheckArithmeticDockCycle("offset", pair.RunOffsetButton);
                host.Workbench.Tool.NewTestCommand.Execute(null);
                pump(8);
                Assert(host.Workbench.Tool.IsNewTest && arithmetic.UseOffsetMode && pair.AddPipelineButton.IsVisible
                    && !pair.WorkbenchStepActions.IsVisible && pair.RunOffsetButton.IsVisible && pair.RunOffsetText.Text == host.Workbench.Tool.RunText,
                    "Arithmetic: offset test mode must restore the legacy add/run rows and its caption");
                capture("Arithmetic-offset-test-actions");
                CheckArithmeticDockCycle("offset-test", pair.RunOffsetButton);
                pipeline.SelectStepCommand.Execute(index);
                pump(8);
                footerChecks.Add("Reselected offset: " + AssertStepActionRow(pair, pair.RunOffsetButton));
                host.Workbench.Tool.RevertCommand.Execute(null);
                pump(8);
                Assert(!arithmetic.UseOffsetMode && pair.RunPreviewButton.IsVisible && !pair.RunOffsetButton.IsVisible
                    && StepXml(pipeline.GetStep(index)) == StepXml(original) && ParameterState(document) == parameters,
                    "Arithmetic: revert restores operation mode, parameters and one action row");
                footerChecks.Add("Reverted operation: " + AssertStepActionRow(pair, pair.RunPreviewButton));
                Assert(document.PreviewRunCount == beforeRuns && host.LayerDocumentCount == beforeLayers, "Arithmetic: action layout/mode restoration must not execute or change layers");
                AssertFiles(files, recipeDirectory, "Arithmetic: action layout/mode restoration must not save");
                capture("Arithmetic-operation-restored-actions");

                void CheckArithmeticDockCycle(string mode, Button runButton)
                {
                    string state = ParameterState(document);
                    string step = StepXml(pipeline.WorkingCopyForTest);
                    Assert(host.FloatDockedWpfToolWindowForTest(), "Arithmetic: float " + mode);
                    pump(12);
                    Window floating = Window.GetWindow(pair);
                    FrameworkElement root = (FrameworkElement)pair.FindName("shellRoot");
                    FrameworkElement footer = (FrameworkElement)pair.FindName("footerPanel");
                    FrameworkElement parameter = (FrameworkElement)pair.FindName("parameterPanel");
                    Rect footerBounds = footer.TransformToAncestor(root).TransformBounds(new Rect(footer.RenderSize));
                    Rect parameterBounds = parameter.TransformToAncestor(root).TransformBounds(new Rect(parameter.RenderSize));
                    Assert(floating != null && floating.IsVisible && !pair.IsDockedInspectorMode && runButton.IsVisible
                        && Grid.GetColumn(footer) == 2 && Grid.GetColumnSpan(footer) == 1
                        && Grid.GetRowSpan((FrameworkElement)pair.FindName("previewScrollViewer")) == 2,
                        "Arithmetic: floating footer must retain the parameter-column layout and preview height");
                    Assert(footerBounds.Top >= parameterBounds.Bottom - 1 && footerBounds.Bottom <= root.ActualHeight + 1,
                        "Arithmetic: floating parameters and footer must not overlap or escape the shell");
                    ScreenshotPngWriter.WriteElementPng(floating, Path.ChangeExtension(outputPath, ".Arithmetic-" + mode + "-floating.png"),
                        (int)Math.Ceiling(floating.ActualWidth), (int)Math.Ceiling(floating.ActualHeight));
                    Assert(host.DockActiveWpfToolWindowForTest(), "Arithmetic: dock " + mode);
                    pump(12);
                    Assert(pair.IsDockedInspectorMode && ReferenceEquals(document.View.FindName("toolShell"), pair)
                        && ParameterState(document) == state && StepXml(pipeline.WorkingCopyForTest) == step,
                        "Arithmetic: dock/float must preserve the native editor, parameter state and working pipeline");
                    Assert(document.PreviewRunCount == beforeRuns && host.LayerDocumentCount == beforeLayers,
                        "Arithmetic: dock/float must not execute or change layers");
                    AssertFiles(files, recipeDirectory, "Arithmetic: dock/float must not save");
                    footerChecks.Add("Dock cycle " + mode + ": floating=" + footerBounds + "; parameter=" + parameterBounds + "; PASS");
                    if (host.Workbench.Tool.IsStepEditing) { footerChecks.Add("Dock cycle " + mode + ": " + AssertStepActionRow(pair, runButton)); }
                }
            }
        }
        string currentPipelineName = host.RecipeCommands.SelectedPipelineOption!.PipelineName;
        VisionPipeline other = new() { Name = "Revert other context" };
        VisionPipelineStorage.Save(host.RecipeCommands.SelectedRecipeName, other);
        host.RecipeCommands.RefreshOptions();
        host.RecipeCommands.SelectedPipelineOption = host.RecipeCommands.PipelineOptions.Single(option => option.PipelineName == other.Name);
        pump(5);
        host.RecipeCommands.SelectedPipelineOption = host.RecipeCommands.PipelineOptions.Single(option => option.PipelineName == currentPipelineName);
        pump(8);
        pipeline.SelectStepCommand.Execute(pipeline.StepCount - 1);
        pump(7);
        VisionPipelineStep reopened = pipeline.GetStep(pipeline.SelectedStepIndex)!;
        Assert(!host.Workbench.Tool.RevertCommand.CanExecute(null), "Revert: reopened saved step starts unchanged");
        changed = pipeline.GetStep(pipeline.SelectedStepIndex)!;
        changed.Parameters["Smoke.Stored"] = "changed after reopen";
        pipeline.ReplaceStep(pipeline.SelectedStepIndex, changed);
        host.Workbench.Tool.RevertCommand.Execute(null);
        pump(6);
        Assert(StepXml(pipeline.GetStep(pipeline.SelectedStepIndex)) == StepXml(reopened), "Revert: reloaded context establishes its own saved identity");
        capture("final");
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".action-colors.txt"), buttonChecks);
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".footer-layout.txt"), footerChecks);
        File.WriteAllText(Path.ChangeExtension(outputPath, ".revert.txt"), "Rename/native restore; rendered ko/en commands; native running guard; move; preceding deletion; insertion/duplicate; undo/redo; new save baseline; failed save recovery; Arithmetic/Line/Matching template/secondary input/acceptance/extension; context reload; no implicit file/run/layer: PASS");
    }

    private static string AssertActionButton(Button button)
    {
        StackPanel content = (StackPanel)button.Content;
        Assert(content.DesiredSize.Width <= button.ActualWidth - button.Padding.Left - button.Padding.Right,
            "Revert: button content must fit in the rendered action slot");
        Assert(content.Children.OfType<TextBlock>().All(text => ReferenceEquals(text.Foreground, button.Foreground))
            && content.Children.OfType<MahApps.Metro.IconPacks.PackIconMaterial>().All(icon => ReferenceEquals(icon.Foreground, button.Foreground)),
            "Revert: rendered text and icon must follow the button state foreground");
        Assert(button.Foreground is SolidColorBrush && button.Background is SolidColorBrush, "Revert: resolved button brushes");
        SolidColorBrush foreground = (SolidColorBrush)button.Foreground;
        SolidColorBrush background = (SolidColorBrush)button.Background;
        double contrast = Program.GetContrastRatio(foreground.Color, background.Color);
        Assert(foreground.Color.A == 255 && background.Color.A == 255 && foreground.Opacity == 1 && background.Opacity == 1 && contrast >= 4.5,
            $"Revert: visible caption/icon; enabled={button.IsEnabled}, foreground={foreground.Color}, background={background.Color}, contrast={contrast:0.00}");
        return $"enabled={button.IsEnabled}; foreground={foreground.Color}; background={background.Color}; contrast={contrast:0.00}; fit/content bindings PASS";
    }

    private static string AssertStepActionRow(VisionToolDoubleInputCustomToolShell shell, Button runButton)
    {
        FrameworkElement root = (FrameworkElement)shell.FindName("shellRoot");
        Button revert = (Button)shell.FindName("btnWorkbenchRevert");
        Button runTo = (Button)shell.FindName("btnWorkbenchRunTo");
        Rect first = revert.TransformToAncestor(root).TransformBounds(new Rect(revert.RenderSize));
        Rect second = runTo.TransformToAncestor(root).TransformBounds(new Rect(runTo.RenderSize));
        Rect last = runButton.TransformToAncestor(root).TransformBounds(new Rect(runButton.RenderSize));
        Assert(revert.IsVisible && runTo.IsVisible && runButton.IsVisible && !shell.AddPipelineButton.IsVisible,
            "Arithmetic: step action mode visibility");
        Assert(Math.Abs(first.Top - second.Top) < 1 && Math.Abs(first.Top - last.Top) < 1 && Math.Abs(first.Height - last.Height) <= 2
            && first.Right <= second.Left && second.Right <= last.Left && first.Left < 1 && Math.Abs(last.Right - root.ActualWidth) < 1,
            $"Arithmetic: three step actions must share one full-width bottom row; revert={first}; runTo={second}; run={last}; root={root.ActualWidth}");
        Assert(last.Bottom <= root.ActualHeight + 1, "Arithmetic: the action row must remain inside its shell");
        return $"revert={first}; runTo={second}; run={last}; root={root.ActualWidth}x{root.ActualHeight}; PASS";
    }

    private static string ParameterState(OpenVisionNativeToolDocument document)
    {
        var capture = (Func<object>)typeof(OpenVisionNativeToolDocument)
            .GetField("captureParameters", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(document)!;
        object snapshot = capture();
        return snapshot is byte[] bytes ? Convert.ToBase64String(bytes) : snapshot is Tuple<byte[], byte[], string> pair
            ? Convert.ToBase64String(pair.Item1) + "/" + Convert.ToBase64String(pair.Item2) + "/" + pair.Item3 : ObjectXml(snapshot);
    }

    private static void ChangeParameter(OpenVisionNativeToolDocument document)
    {
        FrameworkElement view = document.View;
        if (GetSelectedProperty(view) is OpenCvPropertyBase property)
        {
            if (property is AffineTransformProperty affine)
            {
                affine.SourcePoint1X = affine.SourcePoint1X == 3 ? 4 : 3;
                return;
            }
            property.USE_ROI = !property.USE_ROI;
            if (property is EdgeBasedMatchingProperty edge)
            {
                edge.SCORE_MIN = edge.SCORE_MIN == 0.83 ? 0.84 : 0.83;
                edge.CANNY_LOW = 43;
                edge.CANNY_HIGH = 97;
                byte[] snapshot = VisionPipelineStepPropertyMapper.CaptureNativeParameters(edge);
                VisionPipelineStepPropertyMapper.RestoreNativeParameters(snapshot, edge);
                Assert(edge.CANNY_LOW == 43 && edge.CANNY_HIGH == 97, "EdgeBasedMatching: data-only snapshot restore must retain 43/97; actual="
                    + edge.CANNY_LOW + "/" + edge.CANNY_HIGH);
            }
            return;
        }
        if (view is SimplePreprocessToolWpfView simple)
        {
            SimplePreprocessToolSettings settings = simple.Parameters.CaptureSettings();
            ToolParameterValue parameter = settings.Parameters.First(item => double.TryParse(item.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out _));
            parameter.Value = (double.Parse(parameter.Value, CultureInfo.InvariantCulture) + 1).ToString(CultureInfo.InvariantCulture);
            simple.Parameters.ApplySettings(settings);
            return;
        }
        if (view is ArithmeticToolWpfView arithmetic)
        {
            ArithmeticToolSettings settings = arithmetic.CaptureSettings();
            settings.OffsetX = settings.OffsetX == -7 ? -8 : -7;
            arithmetic.ApplyPersistedSettings(settings);
            return;
        }
        string name = view is ThresholdToolWpfView ? "txtThreshold" : "txtWidth";
        TextBox field = (TextBox)view.FindName(name);
        field.Text = view is ThresholdToolWpfView ? field.Text == "171" ? "172" : "171" : field.Text == "5" ? "7" : "5";
        field.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
    }

    private static long GetGeneration(OpenVisionWorkbenchToolViewModel model) => (long)typeof(OpenVisionWorkbenchToolViewModel)
        .GetField("generation", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(model)!;

    private static object? GetSelectedProperty(FrameworkElement view) => view.FindName("toolShell") is VisionToolSingleInputPropertyToolShell shell
        && shell.PropertyGridHost.Child is WpgPropertyGrid grid ? grid.SelectedObject : null;

    private static void AssertChrome(OpenVisionShellHostView host, OpenVisionNativeToolDocument document, OpenVisionWorkbenchToolViewModel model, bool editing)
    {
        Assert(host.FindName("txtDockedToolTitle") is TextBlock outer && outer.IsVisible && outer.Text == model.Title
            && System.Windows.Data.BindingOperations.IsDataBound(outer, TextBlock.TextProperty) && host.DockedToolTitleForTest == model.Title,
            document.ToolName + ": visible outer title binding must survive controller updates");
        if (document.View.FindName("toolShell") is VisionToolSingleInputPropertyToolShell single)
        {
            Assert(single.TitleText.Text == model.Title && single.RunPreviewText.Text == model.RunText, document.ToolName + ": title/run bindings");
            Assert(single.AddPipelineText.Text == OpenVisionLanguageService.T("Workbench.Tool.AddToPipeline") && single.AddPipelineButton.Visibility == (editing ? Visibility.Collapsed : Visibility.Visible), document.ToolName + ": add caption/mode");
        }
        else if (document.View.FindName("toolShell") is VisionToolDoubleInputCustomToolShell pair)
        {
            Assert(pair.TitleText.Text == model.Title && pair.RunPreviewText.Text == model.RunText && pair.AddPipelineButton.Visibility == (editing ? Visibility.Collapsed : Visibility.Visible), "Arithmetic: chrome bindings");
        }
    }

    private static Dictionary<string, string> SnapshotFiles(string root) => Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
        .ToDictionary(Path.GetFullPath, file => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(file))), StringComparer.OrdinalIgnoreCase);
    private static void AssertFiles(Dictionary<string, string> baseline, string root, string reason)
    {
        Dictionary<string, string> current = SnapshotFiles(root);
        Assert(baseline.Count == current.Count && baseline.All(item => current.TryGetValue(item.Key, out string? hash) && hash == item.Value), reason + ": recipe files changed");
    }
    private static string StepXml(VisionPipelineStep? step)
    {
        VisionPipeline pipeline = new();
        if (step != null) { pipeline.Steps.Add(step); }
        return StepXml(pipeline);
    }
    private static string StepXml(VisionPipeline pipeline) => Convert.ToBase64String(VisionPipelineExecutionPlan.SerializePipeline(pipeline));
    private static string ObjectXml(object value)
    {
        using StringWriter writer = new(CultureInfo.InvariantCulture);
        new XmlSerializer(value.GetType()).Serialize(writer, value);
        return writer.ToString();
    }
    private static string ParameterDifferences(VisionPipelineStep expected, VisionPipelineStep actual) => string.Join("; ",
        expected.Parameters.Keys.Concat(actual.Parameters.Keys).Distinct().Where(key => expected.Parameters.GetValueOrDefault(key) != actual.Parameters.GetValueOrDefault(key))
            .Select(key => key + ":" + expected.Parameters.GetValueOrDefault(key) + "->" + actual.Parameters.GetValueOrDefault(key)));
    private static void Assert(bool condition, string message)
    {
        if (!condition) { throw new InvalidOperationException(message); }
    }
}
