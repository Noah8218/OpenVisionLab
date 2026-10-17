using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using OpenVisionLab;
using static OpenVisionLab.DEFINE;

internal static class ShellResponsiveLayoutContract
{
    private static readonly string[] ToolbarIds =
    {
        "HostLearnButton", "HostGuideButton", "HostTcpIntegrationButton",
        "HostRecipeSelector", "HostRecipeSaveButton", "HostRecipeManagerButton",
        "HostLayerNameCombo", "HostTopLayerNameEditor", "HostTopRenameLayerButton",
        "HostTopCreateLayerButton", "HostTopLoadImageIntoLayerButton", "HostTopDeleteLayerButton"
    };

    private static readonly string[] SampleActionIds =
    {
        "WorkspaceSampleCounterpartButton", "WorkspaceSamplePipelineButton", "WorkspaceSampleFirstStepButton"
    };

    internal static int Run(
        string? requestedEvidenceDirectory,
        Func<string, bool, OpenVisionShellHostView> createShellHost,
        Action<int> pump,
        Action<string, DependencyObject?[]> writeOpenGlDiagnostics)
    {
        string directory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(@"D:\OpenVisionLab-TestData\OpenVisionLab_Dev", "shell-layout-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(directory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Shell layout evidence must be written on D:.");
        }

        Directory.CreateDirectory(directory);
        List<string> report = new()
        {
            "Boundary: Current-source in-process WPF windows/layout, not actual EXE user interaction or a DPI/theme sweep.",
            "ViewAssembly: " + typeof(OpenVisionShellHostView).Assembly.Location,
            "ViewModuleVersionId: " + typeof(OpenVisionShellHostView).Module.ModuleVersionId,
            "1278x716 is the content budget of the 1280x760 shell window at scale 1 (border 2 + title bar 42); it does not simulate OS DPI.",
            "Long sample metadata is a presentation-only fixture; no sample catalog or recipe identity is overwritten."
        };
        int failed = 0;
        OpenVisionLanguage previousLanguage = OpenVisionLanguageService.CurrentLanguage;
        try
        {
            foreach ((OpenVisionLanguage language, int width, int height) in new[]
            {
                (OpenVisionLanguage.Korean, 1278, 716),
                (OpenVisionLanguage.English, 1278, 716),
                (OpenVisionLanguage.Korean, 1280, 760),
                (OpenVisionLanguage.English, 1280, 760),
                (OpenVisionLanguage.English, 1600, 900),
                (OpenVisionLanguage.English, 1920, 1032)
            })
            {
                RunCase($"shell-{language}-{width}x{height}", () =>
                    CaptureShellCase(directory, language, width, height, createShellHost, pump, writeOpenGlDiagnostics, report));
            }

            RunCase("review-multiline-and-recovery", () => CaptureReviewCase(directory, pump, writeOpenGlDiagnostics, report));
        }
        finally
        {
            OpenVisionLanguageService.SetLanguage(previousLanguage, false);
        }

        report.Insert(0, "Status: " + (failed == 0 ? "PASS" : "FAIL"));
        string reportPath = Path.Combine(directory, "shell-responsive-layout-contract.txt");
        File.WriteAllLines(reportPath, report);
        Console.WriteLine($"SHELL_RESPONSIVE_LAYOUT_CONTRACT={(failed == 0 ? "PASS" : "FAIL")}|failed={failed}|report={reportPath}");
        return failed == 0 ? 0 : 1;

        void RunCase(string name, Action verify)
        {
            try
            {
                verify();
                report.Add("PASS: " + name);
            }
            catch (Exception exception)
            {
                failed++;
                report.Add("FAIL: " + name + " | " + exception);
            }
        }
    }

    private static void CaptureShellCase(
        string directory, OpenVisionLanguage language, int width, int height,
        Func<string, bool, OpenVisionShellHostView> createShellHost,
        Action<int> pump, Action<string, DependencyObject?[]> writeOpenGlDiagnostics, ICollection<string> report)
    {
        string recipeName = "Smoke_ShellLayout_" + Guid.NewGuid().ToString("N").Substring(0, 12);
        OpenVisionLanguageService.SetLanguage(language, false);
        OpenVisionShellHostView shell = createShellHost(recipeName, false);
        try
        {
            ScreenshotCaptureLifecycle.CaptureWindowWithContent(
                shell, Path.Combine(directory, $"shell-{language}-{width}x{height}.png"), width, height,
                () =>
                {
                    Require(shell.OpenWorkspaceSampleForTest("Public_Matching_DiePad_Good"), "Required public sample could not open.");
                    shell.SelectToolForTest(VISION_MENU.Threshold);
                    pump(24);
                    Require(shell.DockActiveWpfToolWindowForTest(), "Threshold could not dock through the existing Tool command path.");
                    shell.SetDockedToolInspectorWidthForTest(610D);
                    pump(12);
                    Require(shell.IsDockedToolInspectorVisibleForTest, "Docked inspector is not visible.");

                    int previewRuns = shell.NativePreviewRunCount;
                    string inputRoute = shell.ActiveNativeRouteInputLayerNameForTest;
                    string outputRoute = shell.ActiveNativeRouteOutputLayerNameForTest;
                    TextBlock meta = FindNamed<TextBlock>(shell, "txtWorkspaceSampleWorkflowMeta");
                    meta.SetCurrentValue(TextBlock.TextProperty,
                        "Public_Matching_DiePad_Good_" + string.Concat(Enumerable.Repeat("Long_product_variant_", 7)) + " / 12 steps");
                    FindNamed<TextBox>(shell, "txtHostTopLayerNameEditor").SetCurrentValue(TextBox.TextProperty, "Main");
                    shell.UpdateLayout();
                    pump(4);

                    List<FrameworkElement> required = ToolbarIds.Concat(SampleActionIds)
                        .Select(id => Descendants(shell).OfType<FrameworkElement>().Single(element => AutomationProperties.GetAutomationId(element) == id))
                        .ToList();
                    foreach (FrameworkElement element in required)
                    {
                        AssertInsideAncestors(element, shell);
                        if (element.IsEnabled && element is ButtonBase)
                        {
                            DependencyObject? hit = shell.InputHitTest(element.TranslatePoint(new Point(element.ActualWidth / 2D, element.ActualHeight / 2D), shell)) as DependencyObject;
                            while (hit != null && !ReferenceEquals(hit, element))
                            {
                                hit = VisualTreeHelper.GetParent(hit);
                            }
                            Require(ReferenceEquals(hit, element), AutomationProperties.GetAutomationId(element) + " center hits another control.");
                        }
                    }
                    AssertInsideAncestors(meta, shell);

                    for (int index = 0; index < required.Count; index++)
                    {
                        Rect bounds = required[index].TransformToAncestor(shell).TransformBounds(new Rect(required[index].RenderSize));
                        foreach (FrameworkElement other in required.Skip(index + 1))
                        {
                            Rect overlap = Rect.Intersect(bounds, other.TransformToAncestor(shell).TransformBounds(new Rect(other.RenderSize)));
                            Require(overlap.IsEmpty || overlap.Width <= 1D || overlap.Height <= 1D, "Required controls overlap: "
                                + AutomationProperties.GetAutomationId(required[index]) + "/" + AutomationProperties.GetAutomationId(other));
                        }
                    }

                    ComboBox recipes = FindNamed<ComboBox>(shell, "cbHostRecipe");
                    ComboBox layers = FindNamed<ComboBox>(shell, "cbHostLayer");
                    Require(BindingOperations.IsDataBound(recipes, Selector.SelectedItemProperty)
                        && BindingOperations.IsDataBound(layers, Selector.SelectedItemProperty), "Recipe/layer selection bindings were lost.");
                    FrameworkElement imageViewport = FindNamed<FrameworkElement>(shell,
                        shell.IsDockedWorkspaceVisibleForTest ? "dockedWorkspaceView" : "singleWorkspaceView");
                    Require(imageViewport.IsVisible && imageViewport.ActualHeight > 0D, "Wrapping consumed the image viewport.");
                    Require(shell.NativePreviewRunCount == previewRuns
                        && shell.ActiveNativeRouteInputLayerNameForTest == inputRoute
                        && shell.ActiveNativeRouteOutputLayerNameForTest == outputRoute, "Layout fixture changed Preview execution or routing.");
                    report.Add($"BOUNDS: {language} {width}x{height}; {required.Count} controls; inspector={shell.DockedToolInspectorWidthForTest:0.##}; longMetaHeight={meta.ActualHeight:0.##}");
                },
                captureFloatingToolWindow: false, verifyCapture: null, initialPumpCount: 20,
                captureScreen: false, pump, writeOpenGlDiagnostics);
        }
        finally
        {
            RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
        }
    }

    private static void CaptureReviewCase(
        string directory, Action<int> pump, Action<string, DependencyObject?[]> writeOpenGlDiagnostics, ICollection<string> report)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, false);
        OpenVisionPipelineReviewView view = new();
        int runRequests = 0;
        view.RunReviewRequested += (_, _) => runRequests++;
        ScreenshotCaptureLifecycle.CaptureWindowWithContent(
            view, Path.Combine(directory, "review-valid-recovery.png"), 1280, 760,
            () =>
            {
                FindNamed<ToggleButton>(view, "btnReviewDetailsToggle").IsChecked = true;
                ScrollViewer scroll = FindNamed<ScrollViewer>(view, "reviewResultDetailsScrollViewer");
                double longHeight = 0D;
                foreach (int lines in new[] { 1, 2, 9 })
                {
                    string details = string.Join(Environment.NewLine, Enumerable.Range(1, lines).Select(index =>
                        $"Error {index}: D:\\Inspection\\Long_product_variant_with_reference_coordinates\\sample_{index:D2}.png - correct the ROI and retry. [END {index}]"));
                    view.ViewModel.SetValidation("ERROR", details);
                    view.ViewModel.SetResultSummary("Run failed", details);
                    view.UpdateLayout();
                    pump(4);
                    foreach (string name in new[] { "txtValidationDetail", "txtResultDetail" })
                    {
                        TextBlock text = FindNamed<TextBlock>(view, name);
                        Require(BindingOperations.IsDataBound(text, TextBlock.TextProperty) && text.Text == details, name + " did not receive the ViewModel value.");
                        TextBlock measure = new()
                        {
                            Text = details, TextWrapping = TextWrapping.Wrap, FontFamily = text.FontFamily,
                            FontSize = text.FontSize, FontStyle = text.FontStyle, FontWeight = text.FontWeight,
                            FontStretch = text.FontStretch, Padding = text.Padding
                        };
                        measure.Measure(new Size(text.ActualWidth, double.PositiveInfinity));
                        Require(text.ActualHeight + 1D >= measure.DesiredSize.Height, name + " clips measured multiline text.");
                        AssertInsideAncestors(text, (FrameworkElement)text.Parent);
                        ScrollContentPresenter viewport = Descendants(scroll).OfType<ScrollContentPresenter>().First();
                        foreach (double y in new[] { 0D, Math.Max(0D, text.ActualHeight - 1D) })
                        {
                            text.BringIntoView(new Rect(0D, y, Math.Min(1D, text.ActualWidth), 1D));
                            pump(4);
                            Point point = text.TranslatePoint(new Point(0D, y), viewport);
                            Require(point.Y >= -1D && point.Y <= viewport.ActualHeight + 1D, name + " first/last line cannot be scrolled into view.");
                        }
                        report.Add($"TEXT: {name}; lines={lines}; measured={measure.DesiredSize.Height:0.##}; actual={text.ActualHeight:0.##}; scroll={scroll.ScrollableHeight:0.##}");
                        if (lines == 9 && name == "txtValidationDetail") longHeight = text.ActualHeight;
                    }
                    ScreenshotPngWriter.WriteElementPng(view, Path.Combine(directory, $"review-{lines}-lines.png"), 1280, 760);
                }

                view.ViewModel.SetValidation("OK", "Pipeline structure looks valid.");
                view.ViewModel.SetResultSummary("No run result", "-");
                scroll.ScrollToTop();
                view.UpdateLayout();
                pump(4);
                Require(FindNamed<TextBlock>(view, "txtValidationDetail").ActualHeight < longHeight, "Recovery kept the multiline error height.");
                Require(runRequests == 0, "Displaying or scrolling validation text requested Run.");
            },
            captureFloatingToolWindow: false, verifyCapture: null, initialPumpCount: 20,
            captureScreen: false, pump, writeOpenGlDiagnostics);
    }

    private static T FindNamed<T>(FrameworkElement root, string name) where T : FrameworkElement =>
        root.FindName(name) as T ?? throw new InvalidOperationException("Missing named control: " + name);

    private static IEnumerable<DependencyObject> Descendants(DependencyObject root)
    {
        yield return root;
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
        {
            foreach (DependencyObject child in Descendants(VisualTreeHelper.GetChild(root, index))) yield return child;
        }
    }

    private static void AssertInsideAncestors(FrameworkElement element, FrameworkElement root)
    {
        Require(element.IsVisible && element.ActualWidth > 0D && element.ActualHeight > 0D, element.Name + " is not rendered.");
        DependencyObject? parent = VisualTreeHelper.GetParent(element);
        while (parent != null)
        {
            if (parent is FrameworkElement container)
            {
                Rect bounds = element.TransformToAncestor(container).TransformBounds(new Rect(element.RenderSize));
                Require(bounds.Left >= -1D && bounds.Top >= -1D
                    && bounds.Right <= container.ActualWidth + 1D && bounds.Bottom <= container.ActualHeight + 1D,
                    $"{AutomationProperties.GetAutomationId(element)}/{element.Name} exceeds {container.GetType().Name}/{container.Name}: {bounds} vs {container.RenderSize}.");
            }
            if (ReferenceEquals(parent, root)) return;
            parent = VisualTreeHelper.GetParent(parent);
        }
        throw new InvalidOperationException("Layout root is not an ancestor of " + element.Name);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
