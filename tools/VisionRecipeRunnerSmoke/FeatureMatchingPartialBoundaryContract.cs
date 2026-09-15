using System;
using System.Collections.Generic;
using System.IO;

internal static class FeatureMatchingPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "feature-matching-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string viewPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "ToolViews",
            "FeatureMatchingToolWpfView.xaml.cs");
        string xamlPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "ToolViews",
            "FeatureMatchingToolWpfView.xaml");
        string viewModelPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "ViewModels",
            "FeatureMatchingToolViewModel.cs");
        string runtimePath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Tooling",
            "SingleInput",
            "VisionToolSingleInputMatchingToolRuntime.cs");
        string controllerPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Tooling",
            "SingleInput",
            "VisionToolSingleInputMatchingToolController.cs");
        string propertyRuntimePath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Tooling",
            "PropertyGrid",
            "VisionToolMatchingPropertyRuntime.cs");
        string factoryPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "NativeTools",
            "Documents",
            "OpenVisionNativePropertyGridToolFactory.cs");
        string compositionPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Composition",
            "VisionToolCompositionService.cs");
        string baseViewPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Tooling",
            "SingleInput",
            "VisionToolSingleInputPropertyToolViewBase.cs");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string viewModelSource = File.ReadAllText(viewModelPath);
        string runtimeSource = File.ReadAllText(runtimePath);
        string controllerSource = File.ReadAllText(controllerPath);
        string propertyRuntimeSource = File.ReadAllText(propertyRuntimePath);
        string factorySource = File.ReadAllText(factoryPath);
        string compositionSource = File.ReadAllText(compositionPath);
        string baseViewSource = File.ReadAllText(baseViewPath);
        List<string> results = new();

        Check(
            "Feature Matching Partial remains a thin XAML/composition adapter",
            viewSource.Contains("public partial class FeatureMatchingToolWpfView", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolSingleInputMatchingToolController<FeatureMatchingProperty>", StringComparison.Ordinal)
                && viewSource.Contains("AttachPropertyToolController(toolController)", StringComparison.Ordinal)
                && xamlSource.Contains("x:Class=\"OpenVisionLab.FeatureMatchingToolWpfView\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"toolShell\"", StringComparison.Ordinal),
            results);
        Check(
            "Feature Matching Partial has no direct persistence, dialog, or algorithm construction",
            !viewSource.Contains("OpenVisionNativeToolSettingsStore", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("new FeatureMatchingTool", StringComparison.Ordinal)
                && !viewSource.Contains("ReloadTemplateImage", StringComparison.Ordinal),
            results);
        Check(
            "Feature Matching View preserves the existing binding and test facade contract",
            viewSource.Contains("public string ResultReviewTextForTest => toolController.ResultReviewText", StringComparison.Ordinal)
                && viewSource.Contains("public FeatureMatchingProperty CreateProperty()", StringComparison.Ordinal)
                && viewSource.Contains("public void SetTemplatePathForTest(string path)", StringComparison.Ordinal)
                && viewSource.Contains("public void SetResultReview(IEnumerable<MatchingResult> results)", StringComparison.Ordinal)
                && xamlSource.Contains("LearnButtonText=\"Learn Feature\"", StringComparison.Ordinal)
                && xamlSource.Contains("LearnTopicIndex=\"10\"", StringComparison.Ordinal)
                && xamlSource.Contains("TemplateStatusVisibility=\"Visible\"", StringComparison.Ordinal),
            results);
        Check(
            "Feature Matching ViewModel owns mutable property state and template policy",
            viewModelSource.Contains("private readonly FeatureMatchingProperty property", StringComparison.Ordinal)
                && viewModelSource.Contains("ConfigureDefaults()", StringComparison.Ordinal)
                && viewModelSource.Contains("property.ReloadTemplateImage()", StringComparison.Ordinal)
                && viewModelSource.Contains("public FeatureMatchingProperty CreateProperty()", StringComparison.Ordinal)
                && viewModelSource.Contains("property.SCORE_MIN = VisionToolPropertySummaryViewModel.ClampDouble", StringComparison.Ordinal),
            results);
        Check(
            "Shared matching runtime owns PropertyGrid, preview, review, and disposal state",
            runtimeSource.Contains("VisionToolSingleInputViewRuntime inputRuntime", StringComparison.Ordinal)
                && runtimeSource.Contains("VisionToolMatchingPropertyRuntime<TProperty> matchingRuntime", StringComparison.Ordinal)
                && runtimeSource.Contains("VisionToolPresetButtonPresenter<TProperty> presetPresenter", StringComparison.Ordinal)
                && runtimeSource.Contains("matchingRuntime.SetResultReview(title, results, tactTime)", StringComparison.Ordinal)
                && runtimeSource.Contains("presetPresenter.Dispose()", StringComparison.Ordinal)
                && runtimeSource.Contains("inputRuntime.Dispose()", StringComparison.Ordinal)
                && runtimeSource.Contains("matchingRuntime.Dispose()", StringComparison.Ordinal),
            results);
        Check(
            "Shared matching controller owns event and language lifetimes while the View delegates",
            controllerSource.Contains("VisionToolSingleInputToolEventHub eventHub", StringComparison.Ordinal)
                && controllerSource.Contains("VisionToolLanguageChangeController languageChangeController", StringComparison.Ordinal)
                && controllerSource.Contains("VisionToolSingleInputMatchingToolRuntime<TProperty> toolRuntime", StringComparison.Ordinal)
                && controllerSource.Contains("languageChangeController.Dispose()", StringComparison.Ordinal)
                && controllerSource.Contains("toolRuntime.Dispose()", StringComparison.Ordinal)
                && viewSource.Contains("toolController.CreateProperty()", StringComparison.Ordinal)
                && viewSource.Contains("toolController.SetResultReview(results, tactTime)", StringComparison.Ordinal),
            results);
        Check(
            "Matching PropertyRuntime keeps file/persistence callbacks behind the presenter contract",
            propertyRuntimeSource.Contains("presenter.ReloadTemplateIfPatternChanged(e)", StringComparison.Ordinal)
                && propertyRuntimeSource.Contains("presenter.PersistSelectedObject()", StringComparison.Ordinal)
                && propertyRuntimeSource.Contains("presenter.CreateProperty()", StringComparison.Ordinal)
                && !propertyRuntimeSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !propertyRuntimeSource.Contains("OpenFileDialog", StringComparison.Ordinal),
            results);
        Check(
            "Factory and composition establish the Feature Matching creation path",
            factorySource.Contains("public static OpenVisionNativeToolDocument CreateFeatureMatching", StringComparison.Ordinal)
                && factorySource.Contains("VisionToolCompositionService.CreateFeatureMatchingToolViewModel(item)", StringComparison.Ordinal)
                && factorySource.Contains("new FeatureMatchingToolWpfView(presenter)", StringComparison.Ordinal)
                && compositionSource.Contains("public static IFeatureMatchingToolViewModel CreateFeatureMatchingToolViewModel", StringComparison.Ordinal)
                && compositionSource.Contains("return new FeatureMatchingToolViewModel(property)", StringComparison.Ordinal),
            results);
        Check(
            "Base View owns controller release and the Feature Matching Partial does not duplicate disposal",
            baseViewSource.Contains("DisposeToolResources();", StringComparison.Ordinal)
                && baseViewSource.Contains("toolController?.Dispose();", StringComparison.Ordinal)
                && !viewSource.Contains("DisposeToolResources", StringComparison.Ordinal)
                && !viewSource.Contains("toolController.Dispose", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "feature-matching-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "FEATURE_MATCHING_PARTIAL_BOUNDARY_CONTRACT="
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
