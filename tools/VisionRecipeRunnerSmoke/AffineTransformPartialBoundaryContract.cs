using System;
using System.Collections.Generic;
using System.IO;

internal static class AffineTransformPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "affine-transform-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string viewPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "ToolViews", "AffineTransformToolWpfView.xaml.cs");
        string xamlPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "ToolViews", "AffineTransformToolWpfView.xaml");
        string viewModelPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "ViewModels", "AffineTransformToolViewModel.cs");
        string controllerPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputPropertyToolController.cs");
        string resultPresenterPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Tooling", "Review", "AffineTransformResultReviewPresenter.cs");
        string factoryPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "Menu", "Wpf", "NativeTools", "Documents", "OpenVisionNativePropertyGridToolFactory.cs");
        string compositionPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Composition", "VisionToolCompositionService.cs");
        string previewPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "Menu", "Wpf", "NativeTools", "Preview", "OpenVisionNativeToolPreviewExecutor.cs");
        string registryPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "Menu", "Wpf", "NativeTools", "Runtime", "OpenVisionNativeToolRegistry.cs");
        string baseViewPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputPropertyToolViewBase.cs");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string viewModelSource = File.ReadAllText(viewModelPath);
        string controllerSource = File.ReadAllText(controllerPath);
        string resultPresenterSource = File.ReadAllText(resultPresenterPath);
        string factorySource = File.ReadAllText(factoryPath);
        string compositionSource = File.ReadAllText(compositionPath);
        string previewSource = File.ReadAllText(previewPath);
        string registrySource = File.ReadAllText(registryPath);
        string baseViewSource = File.ReadAllText(baseViewPath);
        List<string> results = new();

        Check(
            "Affine Transform Partial remains a required XAML/controller adapter",
            viewSource.Contains("public partial class AffineTransformToolWpfView", StringComparison.Ordinal)
                && viewSource.Contains("OpenVisionToolOpenProfiler.Measure(\"AffineTransformInitializeComponent\", InitializeComponent)", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolSingleInputPropertyToolController<AffineTransformProperty>.Attach(", StringComparison.Ordinal)
                && viewSource.Contains("AttachPropertyToolController(toolController);", StringComparison.Ordinal)
                && xamlSource.Contains("x:Class=\"OpenVisionLab.AffineTransformToolWpfView\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"toolShell\"", StringComparison.Ordinal),
            results);
        Check(
            "Affine Transform Partial has no direct persistence, dialog, file, or algorithm coupling",
            !viewSource.Contains("OpenVisionNativeToolPropertySessionStore", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("OpenCvSharp", StringComparison.Ordinal)
                && !viewSource.Contains("new AffineTransformTool", StringComparison.Ordinal),
            results);
        Check(
            "ViewModel owns mutable Affine property state and summary contract",
            viewModelSource.Contains("private readonly AffineTransformProperty property", StringComparison.Ordinal)
                && viewModelSource.Contains("property.DeepCopy()", StringComparison.Ordinal)
                && viewModelSource.Contains("public string Summary", StringComparison.Ordinal)
                && viewSource.Contains("CreateProperty() => toolController.CreateProperty()", StringComparison.Ordinal),
            results);
        Check(
            "Shared property controller owns binding, preview, events, and test facade",
            controllerSource.Contains("internal sealed class VisionToolSingleInputPropertyToolController", StringComparison.Ordinal)
                && controllerSource.Contains("public static VisionToolSingleInputPropertyToolController<TProperty> Attach", StringComparison.Ordinal)
                && controllerSource.Contains("CreateProperty()", StringComparison.Ordinal)
                && controllerSource.Contains("ShowResultReview", StringComparison.Ordinal)
                && controllerSource.Contains("ConfigurePropertyForTest", StringComparison.Ordinal)
                && controllerSource.Contains("Dispose()", StringComparison.Ordinal),
            results);
        Check(
            "Result review policy remains in the existing Affine presenter",
            resultPresenterSource.Contains("internal static class AffineTransformResultReviewPresenter", StringComparison.Ordinal)
                && resultPresenterSource.Contains("AffineValidPixelRatio", StringComparison.Ordinal)
                && resultPresenterSource.Contains("showResultReview(summary, isSuccess, items, guidance)", StringComparison.Ordinal)
                && viewSource.Contains("AffineTransformResultReviewPresenter.Show(", StringComparison.Ordinal)
                && viewSource.Contains("toolController.ShowResultReview(summary, isSuccess, items, guidance)", StringComparison.Ordinal),
            results);
        Check(
            "Factory and composition own property load, ViewModel creation, persistence, and document composition",
            factorySource.Contains("CreateAffineTransform(IDisplayManager displayManager)", StringComparison.Ordinal)
                && factorySource.Contains("OpenVisionNativeToolPropertySessionStore.GetOrLoad", StringComparison.Ordinal)
                && factorySource.Contains("VisionToolCompositionService.CreateAffineTransformToolViewModel(item)", StringComparison.Ordinal)
                && factorySource.Contains("new AffineTransformToolWpfView(presenter)", StringComparison.Ordinal)
                && factorySource.Contains("persistSelectedObject: () => OpenVisionNativeToolPropertySessionStore.Save(toolName, property)", StringComparison.Ordinal)
                && compositionSource.Contains("CreateAffineTransformToolViewModel", StringComparison.Ordinal)
                && compositionSource.Contains("return new AffineTransformToolViewModel(property)", StringComparison.Ordinal),
            results);
        Check(
            "Preview execution and overlay drawing remain outside the View Partial",
            previewSource.Contains("ExecuteAffineTransformPreview(Mat source, AffineTransformToolWpfView view)", StringComparison.Ordinal)
                && previewSource.Contains("new AffineTransformTool()", StringComparison.Ordinal)
                && previewSource.Contains("OpenVisionNativeToolPreviewOverlayRenderer.CreateAffineTransformPreviewImage", StringComparison.Ordinal)
                && previewSource.Contains("view.SetResultReview(result)", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionNativeToolPreviewExecutor", StringComparison.Ordinal),
            results);
        Check(
            "Registry and base View preserve creation and release ownership",
            registrySource.Contains("LargeTool(VISION_MENU.AffineTransform, OpenVisionNativePropertyGridToolFactory.CreateAffineTransform, nameof(AffineTransformToolWpfView)", StringComparison.Ordinal)
                && baseViewSource.Contains("DisposeToolResources();", StringComparison.Ordinal)
                && baseViewSource.Contains("toolController?.Dispose();", StringComparison.Ordinal)
                && !viewSource.Contains("DisposeToolResources", StringComparison.Ordinal),
            results);
        Check(
            "Existing public/test facade and Learn binding remain stable",
            viewSource.Contains("public string ResultReviewTextForTest", StringComparison.Ordinal)
                && viewSource.Contains("public void SetResultReview(VisionToolResult result)", StringComparison.Ordinal)
                && viewSource.Contains("public void ConfigurePropertyForTest(Action<AffineTransformProperty> configure)", StringComparison.Ordinal)
                && xamlSource.Contains("LearnButtonText=\"Learn Affine Transform\"", StringComparison.Ordinal)
                && xamlSource.Contains("LearnTopicIndex=\"15\"", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "affine-transform-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "AFFINE_TRANSFORM_PARTIAL_BOUNDARY_CONTRACT="
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
