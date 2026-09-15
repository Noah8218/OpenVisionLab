using System;
using System.Collections.Generic;
using System.IO;

internal static class VisionToolParameterGuidePartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "parameter-guide-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string sourceRoot = Path.Combine(repositoryRoot, "src", "OpenVisionLab");
        string viewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "PropertyGuide", "VisionToolParameterGuideView.xaml.cs");
        string xamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "PropertyGuide", "VisionToolParameterGuideView.xaml");
        string contentPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "PropertyGuide", "VisionToolParameterGuideContent.cs");
        string presenterPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "PropertyGuide", "VisionToolParameterGuidePresenter.cs");
        string catalogPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "PropertyGuide", "VisionToolParameterGuideCatalog.cs");
        string sidecarPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "PropertyGuide", "VisionToolParameterGuideSidecarController.cs");
        string binderPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "PropertyGuide", "VisionToolCustomParameterGuideBinder.cs");
        string hostPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "PropertyGrid", "VisionToolPropertyGridHost.cs");
        string shellPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputPropertyToolShell.xaml.cs");
        string shellXamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputPropertyToolShell.xaml");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string contentSource = File.ReadAllText(contentPath);
        string presenterSource = File.ReadAllText(presenterPath);
        string catalogSource = File.ReadAllText(catalogPath);
        string sidecarSource = File.ReadAllText(sidecarPath);
        string binderSource = File.ReadAllText(binderPath);
        string hostSource = File.ReadAllText(hostPath);
        string shellSource = File.ReadAllText(shellPath);
        string shellXamlSource = File.ReadAllText(shellXamlPath);
        List<string> results = new();

        Check(
            "Parameter guide Partial remains the required XAML visual adapter",
            viewSource.Contains("public partial class VisionToolParameterGuideView : UserControl", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent();", StringComparison.Ordinal)
                && viewSource.Contains("public VisionToolParameterGuideView()", StringComparison.Ordinal)
                && xamlSource.Contains("<UserControl x:Class=\"OpenVisionLab.VisionToolParameterGuideView\"", StringComparison.Ordinal)
                && xamlSource.Contains("AutomationProperties.AutomationId=\"VisionToolParameterGuide\"", StringComparison.Ordinal),
            results);
        Check(
            "Parameter guide XAML preserves the existing namescope and automation contract",
            xamlSource.Contains("x:Name=\"guideExpander\"", StringComparison.Ordinal)
                && xamlSource.Contains("VisionToolParameterGuideExpander", StringComparison.Ordinal)
                && xamlSource.Contains("VisionToolParameterGuideCoverage", StringComparison.Ordinal)
                && xamlSource.Contains("VisionToolParameterGuideTitle", StringComparison.Ordinal)
                && xamlSource.Contains("VisionToolParameterGuideIdentity", StringComparison.Ordinal)
                && xamlSource.Contains("VisionToolParameterGuideSummary", StringComparison.Ordinal)
                && xamlSource.Contains("VisionToolParameterGuideRelated", StringComparison.Ordinal),
            results);
        Check(
            "View code-behind owns only content projection and explicit related-property callback",
            viewSource.Contains("private Action<string> focusRelatedProperty", StringComparison.Ordinal)
                && viewSource.Contains("internal event EventHandler ContentPresented", StringComparison.Ordinal)
                && viewSource.Contains("ShowPrompt()", StringComparison.Ordinal)
                && viewSource.Contains("ShowContent(", StringComparison.Ordinal)
                && viewSource.Contains("SetCompactMode(bool compact)", StringComparison.Ordinal)
                && !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenCvSharp", StringComparison.Ordinal)
                && !viewSource.Contains("DispatcherTimer", StringComparison.Ordinal),
            results);
        Check(
            "ShowPrompt resets presentation state without owning selection policy",
            viewSource.Contains("focusRelatedProperty = null", StringComparison.Ordinal)
                && viewSource.Contains("guideExpander.IsExpanded = false", StringComparison.Ordinal)
                && viewSource.Contains("relatedButtons.Children.Clear()", StringComparison.Ordinal)
                && viewSource.Contains("ApplyLabels();", StringComparison.Ordinal)
                && viewSource.Contains("AutomationProperties.SetName(this, txtHeader.Text)", StringComparison.Ordinal)
                && !viewSource.Contains("VisionToolParameterGuideCatalog.Resolve", StringComparison.Ordinal),
            results);
        Check(
            "ShowContent consumes a presentation model and publishes the existing event",
            viewSource.Contains("VisionToolParameterGuideContent content", StringComparison.Ordinal)
                && viewSource.Contains("content.RelatedPropertyNames", StringComparison.Ordinal)
                && viewSource.Contains("PopulateRelatedButtons(content.RelatedPropertyNames)", StringComparison.Ordinal)
                && viewSource.Contains("ContentPresented(this, EventArgs.Empty)", StringComparison.Ordinal)
                && contentSource.Contains("public string Summary", StringComparison.Ordinal)
                && contentSource.Contains("public IReadOnlyList<string> RelatedPropertyNames", StringComparison.Ordinal),
            results);
        Check(
            "Related buttons route only through the injected focus callback",
            viewSource.Contains("button.Click += RelatedButton_Click", StringComparison.Ordinal)
                && viewSource.Contains("focusRelatedProperty?.Invoke(propertyName)", StringComparison.Ordinal)
                && !viewSource.Contains("PropertyGrid", StringComparison.Ordinal)
                && !viewSource.Contains("FocusProperty", StringComparison.Ordinal),
            results);
        Check(
            "Parameter guide presenter owns selected object/property and catalog policy",
            presenterSource.Contains("private object selectedObject", StringComparison.Ordinal)
                && presenterSource.Contains("private string selectedPropertyName", StringComparison.Ordinal)
                && presenterSource.Contains("VisionToolParameterGuideCatalog.Resolve", StringComparison.Ordinal)
                && presenterSource.Contains("view.ShowPrompt()", StringComparison.Ordinal)
                && presenterSource.Contains("view.ShowContent(content, FocusRelatedProperty)", StringComparison.Ordinal)
                && presenterSource.Contains("focusProperty", StringComparison.Ordinal),
            results);
        Check(
            "Catalog remains the policy owner for definitions, applicability, localization and fallback",
            catalogSource.Contains("CommonDefinitions", StringComparison.Ordinal)
                && catalogSource.Contains("Definitions", StringComparison.Ordinal)
                && catalogSource.Contains("public static VisionToolParameterGuideContent Resolve", StringComparison.Ordinal)
                && catalogSource.Contains("ResolveApplicability", StringComparison.Ordinal)
                && catalogSource.Contains("ResolveFallbackImpact", StringComparison.Ordinal)
                && catalogSource.Contains("ResolveFallbackCheck", StringComparison.Ordinal),
            results);
        Check(
            "Sidecar controller owns Window creation, positioning, re-entry and release",
            sidecarSource.Contains("OpenVisionFloatingToolWindow", StringComparison.Ordinal)
                && sidecarSource.Contains("Window.GetWindow(shell)", StringComparison.Ordinal)
                && sidecarSource.Contains("PositionNextToShell", StringComparison.Ordinal)
                && sidecarSource.Contains("sidecar.Closing += Sidecar_Closing", StringComparison.Ordinal)
                && sidecarSource.Contains("sidecar.ClearHostedContent()", StringComparison.Ordinal)
                && sidecarSource.Contains("shell.Unloaded += Shell_Unloaded", StringComparison.Ordinal),
            results);
        Check(
            "Binder owns PropertyGrid event subscriptions, language refresh and disposal",
            binderSource.Contains("VisionToolParameterGuidePresenter", StringComparison.Ordinal)
                && binderSource.Contains("VisionToolLanguageChangeController.Attach(Refresh)", StringComparison.Ordinal)
                && binderSource.Contains("Keyboard.GotKeyboardFocusEvent", StringComparison.Ordinal)
                && binderSource.Contains("Mouse.PreviewMouseDownEvent", StringComparison.Ordinal)
                && binderSource.Contains("public void Dispose()", StringComparison.Ordinal)
                && binderSource.Contains("languageController.Dispose()", StringComparison.Ordinal),
            results);
        Check(
            "PropertyGrid host owns standard selection/value wiring and presenter lifetime",
            hostSource.Contains("VisionToolParameterGuidePresenter", StringComparison.Ordinal)
                && hostSource.Contains("Grid.SelectedPropertyChanged += OnSelectedPropertyChanged", StringComparison.Ordinal)
                && hostSource.Contains("parameterGuidePresenter?.SelectProperty", StringComparison.Ordinal)
                && hostSource.Contains("parameterGuideLanguageController?.Dispose()", StringComparison.Ordinal)
                && hostSource.Contains("VisionToolSingleInputPropertyToolShell shell = FindShell(host)", StringComparison.Ordinal),
            results);
        Check(
            "Single-input shell owns the guide instance, visibility and sidecar composition",
            shellSource.Contains("private readonly VisionToolParameterGuideView parameterGuideView", StringComparison.Ordinal)
                && shellSource.Contains("private readonly VisionToolParameterGuideSidecarController parameterGuideSidecarController", StringComparison.Ordinal)
                && shellSource.Contains("parameterGuideView = new VisionToolParameterGuideView()", StringComparison.Ordinal)
                && shellSource.Contains("new VisionToolParameterGuideSidecarController(this, parameterGuideView)", StringComparison.Ordinal)
                && shellSource.Contains("ParameterGuideVisibilityProperty", StringComparison.Ordinal)
                && shellSource.Contains("public VisionToolParameterGuideView ParameterGuide", StringComparison.Ordinal)
                && shellXamlSource.Contains("VisionToolParameterGuideButton", StringComparison.Ordinal),
            results);
        Check(
            "Parameter guide keeps content readable through scrolling and text wrapping",
            xamlSource.Contains("VerticalScrollBarVisibility=\"Auto\"", StringComparison.Ordinal)
                && xamlSource.Contains("HorizontalScrollBarVisibility=\"Disabled\"", StringComparison.Ordinal)
                && xamlSource.Contains("TextWrapping=\"Wrap\"", StringComparison.Ordinal)
                && xamlSource.Contains("MaxHeight=\"184\"", StringComparison.Ordinal)
                && viewSource.Contains("guideScroll.MaxHeight = compact ? 116D : 184D", StringComparison.Ordinal),
            results);
        Check(
            "Parameter guide Partial has no execution, persistence, algorithm or independent lifetime owner",
            !viewSource.Contains("RunPreview", StringComparison.Ordinal)
                && !viewSource.Contains("Persist", StringComparison.Ordinal)
                && !viewSource.Contains("CreateProperty", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionNativeTool", StringComparison.Ordinal)
                && !viewSource.Contains("IDisposable", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionFloatingToolWindow", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "parameter-guide-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "VISION_TOOL_PARAMETER_GUIDE_PARTIAL_BOUNDARY_CONTRACT="
            + (passed ? "PASS" : "FAIL")
            + "|checks="
            + results.Count);
        return passed ? 0 : 1;
    }

    private static void Check(string name, bool condition, ICollection<string> results)
    {
        results.Add((condition ? "PASS: " : "FAIL: ") + name);
    }

    private static string ResolveRepositoryRoot()
    {
        DirectoryInfo? current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "src", "OpenVisionLab", "OpenVisionLab.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("OpenVisionLab repository root was not found.");
    }
}
