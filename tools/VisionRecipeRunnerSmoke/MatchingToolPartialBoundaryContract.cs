using System;
using System.Collections.Generic;
using System.IO;

internal static class MatchingToolPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "matching-tool-partial-boundary-20260914")
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
            "MatchingToolWpfView.xaml.cs");
        string xamlPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "ToolViews",
            "MatchingToolWpfView.xaml");
        string adapterPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Recipe",
            "PropertyGrid",
            "VisionPipelineMatchingPropertyAdapter.cs");
        string documentPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "NativeTools",
            "Documents",
            "OpenVisionNativeToolDocument.cs");
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
        string viewModelPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "ViewModels",
            "MatchingToolViewModel.cs");
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
        string adapterSource = File.ReadAllText(adapterPath);
        string documentSource = File.ReadAllText(documentPath);
        string factorySource = File.ReadAllText(factoryPath);
        string compositionSource = File.ReadAllText(compositionPath);
        string viewModelSource = File.ReadAllText(viewModelPath);
        string runtimeSource = File.ReadAllText(runtimePath);
        string controllerSource = File.ReadAllText(controllerPath);
        string baseViewSource = File.ReadAllText(baseViewPath);
        List<string> results = new();

        Check(
            "Matching Tool Partial remains a thin XAML/controller composition adapter",
            viewSource.Contains("public partial class MatchingToolWpfView", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolSingleInputMatchingToolController<MatchingProperty>", StringComparison.Ordinal)
                && viewSource.Contains("AttachPropertyToolController(toolController)", StringComparison.Ordinal)
                && xamlSource.Contains("x:Class=\"OpenVisionLab.MatchingToolWpfView\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"toolShell\"", StringComparison.Ordinal),
            results);
        Check(
            "Sample property policy is delegated to the existing Matching adapter owner",
            viewSource.Contains("VisionPipelineMatchingPropertyAdapter.ResolveSampleTemplatePath(source)", StringComparison.Ordinal)
                && viewSource.Contains("VisionPipelineMatchingPropertyAdapter.ApplySampleProperty(source, target)", StringComparison.Ordinal)
                && !viewSource.Contains("VisionPipelineAppToolFactory.ResolveTemplatePath", StringComparison.Ordinal)
                && !viewSource.Contains("target.PIXELPERMM = source.PIXELPERMM", StringComparison.Ordinal)
                && !viewSource.Contains("new List<OpenCvSharp.Rect>", StringComparison.Ordinal),
            results);

        string[] samplePropertyFields =
        {
            "target.PIXELPERMM = source.PIXELPERMM",
            "target.USE_THRESHOLD = source.USE_THRESHOLD",
            "target.CvROIS = source.CvROIS",
            "target.CvMASKS = source.CvMASKS",
            "target.AUTO_PREVIEW = false",
            "target.MATCH_MODE = source.MATCH_MODE",
            "target.SCORE_MIN = source.SCORE_MIN",
            "target.NUM_MATCH = source.NUM_MATCH",
            "target.MAGNIFIATION = source.MAGNIFIATION",
            "target.USE_FIND_ANGLE = source.USE_FIND_ANGLE",
            "target.FIND_ANGLE_MIN = source.FIND_ANGLE_MIN",
            "target.FIND_ANGLE_MAX = source.FIND_ANGLE_MAX",
            "target.USE_COARSE_TO_FINE_ANGLE_SEARCH = source.USE_COARSE_TO_FINE_ANGLE_SEARCH",
            "target.USE_FIND_SCALE = source.USE_FIND_SCALE",
            "target.FIND_SCALE_MIN = source.FIND_SCALE_MIN",
            "target.FIND_SCALE_MAX = source.FIND_SCALE_MAX",
            "target.USE_PYRAMID_POSITION_PROPOSAL = source.USE_PYRAMID_POSITION_PROPOSAL",
            "target.PYRAMID_POSITION_TOP_N = source.PYRAMID_POSITION_TOP_N",
            "target.PYRAMID_POSITION_MIN_SCORE = source.PYRAMID_POSITION_MIN_SCORE",
            "target.USE_CANNY = source.USE_CANNY",
            "target.CANNY_LOW = source.CANNY_LOW",
            "target.CANNY_HIGH = source.CANNY_HIGH",
            "target.USE_PADDING_COLOR_WHITE = source.USE_PADDING_COLOR_WHITE"
        };
        bool allSampleFieldsOwned = true;
        foreach (string field in samplePropertyFields)
        {
            allSampleFieldsOwned &= adapterSource.Contains(field, StringComparison.Ordinal);
        }

        Check(
            "Matching adapter owns the complete sample copy and clone policy",
            adapterSource.Contains("internal static string ResolveSampleTemplatePath(MatchingProperty source)", StringComparison.Ordinal)
                && adapterSource.Contains("VisionPipelineAppToolFactory.ResolveTemplatePath(source?.PATTERN_PATH)", StringComparison.Ordinal)
                && adapterSource.Contains("new List<Rect>(source.CvROIS)", StringComparison.Ordinal)
                && adapterSource.Contains("new List<Rect>(source.CvMASKS)", StringComparison.Ordinal)
                && allSampleFieldsOwned,
            results);
        Check(
            "Matching adapter remains the existing pipeline property owner",
            adapterSource.Contains("public static bool TryCreateProperty(", StringComparison.Ordinal)
                && adapterSource.Contains("public static bool TryCreateStep(", StringComparison.Ordinal)
                && adapterSource.Contains("public static bool IsProperty(object property)", StringComparison.Ordinal)
                && adapterSource.Contains("private sealed class PipelineMatchingProperty", StringComparison.Ordinal),
            results);
        Check(
            "View preserves the public test facade and Matching shell contract",
            viewSource.Contains("public string ResultReviewTextForTest => toolController.ResultReviewText", StringComparison.Ordinal)
                && viewSource.Contains("public MatchingProperty CreateProperty()", StringComparison.Ordinal)
                && viewSource.Contains("public void SetTemplatePathForTest(string path)", StringComparison.Ordinal)
                && viewSource.Contains("public void ConfigurePropertyForTest(Action<MatchingProperty> configure)", StringComparison.Ordinal)
                && xamlSource.Contains("LearnButtonText=\"Learn Matching\"", StringComparison.Ordinal)
                && xamlSource.Contains("LearnTopicIndex=\"9\"", StringComparison.Ordinal)
                && xamlSource.Contains("TemplateStatusVisibility=\"Visible\"", StringComparison.Ordinal),
            results);
        int sampleMethodStart = viewSource.IndexOf(
            "internal void ApplySampleProperty(MatchingProperty source)",
            StringComparison.Ordinal);
        int sampleMethodEnd = viewSource.IndexOf(
            "public bool ApplyPresetForTest",
            sampleMethodStart,
            StringComparison.Ordinal);
        string sampleMethodSource = sampleMethodStart >= 0 && sampleMethodEnd > sampleMethodStart
            ? viewSource.Substring(sampleMethodStart, sampleMethodEnd - sampleMethodStart)
            : string.Empty;
        Check(
            "Sample application preserves template-path-before-property ordering",
            sampleMethodSource.IndexOf("SetTemplatePathForTest", StringComparison.Ordinal)
                < sampleMethodSource.IndexOf("ConfigurePropertyForTest", StringComparison.Ordinal)
                && adapterSource.IndexOf("target.AUTO_PREVIEW = false", StringComparison.Ordinal)
                    > adapterSource.IndexOf("target.CvMASKS", StringComparison.Ordinal),
            results);
        Check(
            "Document and factory establish the Matching sample call path",
            documentSource.Contains("VisionPipelineStepPropertyMapper.CreateProperty(step)", StringComparison.Ordinal)
                && documentSource.Contains("matchingView.ApplySampleProperty(matching)", StringComparison.Ordinal)
                && factorySource.Contains("public static OpenVisionNativeToolDocument CreateMatching", StringComparison.Ordinal)
                && factorySource.Contains("VisionToolCompositionService.CreateMatchingToolViewModel(item)", StringComparison.Ordinal)
                && factorySource.Contains("new MatchingToolWpfView(presenter)", StringComparison.Ordinal),
            results);
        Check(
            "ViewModel owns mutable Matching state and normalization",
            viewModelSource.Contains("private readonly MatchingProperty property", StringComparison.Ordinal)
                && viewModelSource.Contains("ConfigureDefaults()", StringComparison.Ordinal)
                && viewModelSource.Contains("property.ReloadTemplateImage()", StringComparison.Ordinal)
                && viewModelSource.Contains("public MatchingProperty CreateProperty()", StringComparison.Ordinal)
                && viewModelSource.Contains("private void Normalize()", StringComparison.Ordinal),
            results);
        Check(
            "Shared runtime and controller retain preview, event, and release ownership",
            runtimeSource.Contains("VisionToolSingleInputViewRuntime inputRuntime", StringComparison.Ordinal)
                && runtimeSource.Contains("VisionToolMatchingPropertyRuntime<TProperty> matchingRuntime", StringComparison.Ordinal)
                && runtimeSource.Contains("matchingRuntime.Dispose()", StringComparison.Ordinal)
                && controllerSource.Contains("VisionToolSingleInputToolEventHub eventHub", StringComparison.Ordinal)
                && controllerSource.Contains("VisionToolLanguageChangeController languageChangeController", StringComparison.Ordinal)
                && controllerSource.Contains("toolRuntime.Dispose()", StringComparison.Ordinal),
            results);
        Check(
            "Base View remains the lifetime owner and the Partial has no duplicate disposal",
            baseViewSource.Contains("DisposeToolResources();", StringComparison.Ordinal)
                && baseViewSource.Contains("toolController?.Dispose();", StringComparison.Ordinal)
                && !viewSource.Contains("DisposeToolResources", StringComparison.Ordinal)
                && !viewSource.Contains("toolController.Dispose", StringComparison.Ordinal),
            results);
        Check(
            "Matching Partial has no direct persistence, dialog, or file-system policy",
            !viewSource.Contains("OpenVisionNativeToolSettingsStore", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("new MatchingTool", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "matching-tool-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "MATCHING_TOOL_PARTIAL_BOUNDARY_CONTRACT="
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
