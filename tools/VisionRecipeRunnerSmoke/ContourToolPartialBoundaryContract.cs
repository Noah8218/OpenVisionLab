using System;
using System.Collections.Generic;
using System.IO;

internal static class ContourToolPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "contour-tool-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string sourceRoot = Path.Combine(repositoryRoot, "src", "OpenVisionLab");
        string viewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "ToolViews", "ContourToolWpfView.xaml.cs");
        string xamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "ToolViews", "ContourToolWpfView.xaml");
        string viewModelPath = Path.Combine(sourceRoot, "UI", "VisionTest", "ViewModels", "ContourToolViewModel.cs");
        string compositionPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Composition", "VisionToolCompositionService.cs");
        string controllerPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputPropertyToolController.cs");
        string runtimePath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputPropertyToolRuntime.cs");
        string baseViewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputPropertyToolViewBase.cs");
        string presenterPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "PropertyGrid", "VisionToolPropertyGridPresenter.cs");
        string guidePresenterPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "Review", "VisionToolAreaVerificationGuidePresenter.cs");
        string guideCriteriaPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "Review", "VisionToolAreaVerificationCriteriaText.cs");
        string teachingPreviewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "Preview", "VisionToolThresholdTeachingPreviewController.cs");
        string factoryPath = Path.Combine(sourceRoot, "UI", "Menu", "Wpf", "NativeTools", "Documents", "OpenVisionNativePropertyGridToolFactory.cs");
        string documentBuilderPath = Path.Combine(sourceRoot, "UI", "Menu", "Wpf", "NativeTools", "Documents", "OpenVisionNativePropertyGridToolDocumentBuilder.cs");
        string singleInputBuilderPath = Path.Combine(sourceRoot, "UI", "Menu", "Wpf", "NativeTools", "Documents", "OpenVisionNativeSingleInputToolDocumentBuilder.cs");
        string previewExecutorPath = Path.Combine(sourceRoot, "UI", "Menu", "Wpf", "NativeTools", "Preview", "OpenVisionNativeToolPreviewExecutor.cs");
        string overlayPath = Path.Combine(sourceRoot, "UI", "Menu", "Wpf", "NativeTools", "Preview", "OpenVisionNativeToolPreviewOverlayRenderer.cs");
        string registryPath = Path.Combine(sourceRoot, "UI", "Menu", "Wpf", "NativeTools", "Runtime", "OpenVisionNativeToolRegistry.cs");
        string pipelinePath = Path.Combine(sourceRoot, "Core", "Pipeline", "Definition", "VisionPipelineStepBuilder.cs");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string viewModelSource = File.ReadAllText(viewModelPath);
        string compositionSource = File.ReadAllText(compositionPath);
        string controllerSource = File.ReadAllText(controllerPath);
        string runtimeSource = File.ReadAllText(runtimePath);
        string baseViewSource = File.ReadAllText(baseViewPath);
        string presenterSource = File.ReadAllText(presenterPath);
        string guidePresenterSource = File.ReadAllText(guidePresenterPath);
        string guideCriteriaSource = File.ReadAllText(guideCriteriaPath);
        string teachingPreviewSource = File.ReadAllText(teachingPreviewPath);
        string factorySource = File.ReadAllText(factoryPath);
        string documentBuilderSource = File.ReadAllText(documentBuilderPath);
        string singleInputBuilderSource = File.ReadAllText(singleInputBuilderPath);
        string previewExecutorSource = File.ReadAllText(previewExecutorPath);
        string overlaySource = File.ReadAllText(overlayPath);
        string registrySource = File.ReadAllText(registryPath);
        string pipelineSource = File.ReadAllText(pipelinePath);
        List<string> results = new();

        Check(
            "Contour Partial remains the required XAML/PropertyGrid adapter",
            viewSource.Contains("public partial class ContourToolWpfView : VisionToolSingleInputPropertyToolViewBase", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolPropertyGridPresenter<ContourProperty>", StringComparison.Ordinal)
                && viewSource.Contains("new VisionToolAreaVerificationGuidePresenter<ContourProperty, ContourResult>", StringComparison.Ordinal)
                && viewSource.Contains("new VisionToolThresholdTeachingPreviewController", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolSingleInputPropertyToolController<ContourProperty>.Attach", StringComparison.Ordinal)
                && viewSource.Contains("AttachPropertyToolController(toolController)", StringComparison.Ordinal)
                && xamlSource.Contains("x:Class=\"OpenVisionLab.ContourToolWpfView\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"toolShell\"", StringComparison.Ordinal)
                && xamlSource.Contains("LearnButtonText=\"Learn Contour\"", StringComparison.Ordinal)
                && xamlSource.Contains("LearnTopicIndex=\"6\"", StringComparison.Ordinal)
                && xamlSource.Contains("VisionToolVerificationGuideView", StringComparison.Ordinal),
            results);
        Check(
            "Contour Partial has no direct persistence, dialog, file, or algorithm coupling",
            !viewSource.Contains("OpenVisionNativeToolPropertySessionStore", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("OpenCvSharp", StringComparison.Ordinal)
                && !viewSource.Contains("new ContourTool", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionNativeToolPreviewExecutor", StringComparison.Ordinal)
                && !viewSource.Contains("VisionPipelineStepBuilder", StringComparison.Ordinal),
            results);
        Check(
            "Generic PropertyGrid controller owns binding, layer/preview events, auto-preview, review, and release",
            controllerSource.Contains("internal sealed class VisionToolSingleInputPropertyToolController<TProperty>", StringComparison.Ordinal)
                && controllerSource.Contains("Attach(", StringComparison.Ordinal)
                && controllerSource.Contains("VisionToolSingleInputPropertyToolRuntime<TProperty>.Attach", StringComparison.Ordinal)
                && controllerSource.Contains("CreateProperty()", StringComparison.Ordinal)
                && controllerSource.Contains("ShowAreaResultReview<TResult>", StringComparison.Ordinal)
                && controllerSource.Contains("languageChangeController.Dispose();", StringComparison.Ordinal)
                && controllerSource.Contains("toolRuntime.Dispose();", StringComparison.Ordinal)
                && viewSource.Contains("return toolController.CreateProperty();", StringComparison.Ordinal)
                && viewSource.Contains("toolController.ShowAreaResultReview(", StringComparison.Ordinal),
            results);
        Check(
            "Generic PropertyGrid runtime owns mutable edit policy, persistence callback, debounce, and preview state",
            runtimeSource.Contains("VisionToolPropertyChangeController", StringComparison.Ordinal)
                && runtimeSource.Contains("VisionToolPropertyGridHost.Attach", StringComparison.Ordinal)
                && runtimeSource.Contains("presenter.PersistSelectedObject();", StringComparison.Ordinal)
                && runtimeSource.Contains("VisionToolDebouncedPreviewScheduler", StringComparison.Ordinal)
                && runtimeSource.Contains("refreshVerificationGuide", StringComparison.Ordinal)
                && runtimeSource.Contains("RefreshVerificationGuide()", StringComparison.Ordinal)
                && runtimeSource.Contains("autoPreviewOnPropertyChanged", StringComparison.Ordinal)
                && runtimeSource.Contains("propertyGridController.Dispose();", StringComparison.Ordinal),
            results);
        Check(
            "Contour ViewModel and composition service own property state, normalization, summary, and ViewModel creation",
            viewModelSource.Contains("private readonly ContourProperty property", StringComparison.Ordinal)
                && viewModelSource.Contains("public ContourProperty CreateProperty()", StringComparison.Ordinal)
                && viewModelSource.Contains("Normalize()", StringComparison.Ordinal)
                && viewModelSource.Contains("public string Summary", StringComparison.Ordinal)
                && compositionSource.Contains("CreateContourToolViewModel(ContourProperty property)", StringComparison.Ordinal)
                && compositionSource.Contains("new ContourToolViewModel(property)", StringComparison.Ordinal),
            results);
        Check(
            "Area verification and threshold-teaching owners keep inspection policy out of the View",
            guidePresenterSource.Contains("ShowTeachingState(TProperty property)", StringComparison.Ordinal)
                && guidePresenterSource.Contains("ShowResult(IEnumerable<TResult> results, TProperty property)", StringComparison.Ordinal)
                && guideCriteriaSource.Contains("CreateContour(ContourProperty property)", StringComparison.Ordinal)
                && teachingPreviewSource.Contains("public void Request()", StringComparison.Ordinal)
                && teachingPreviewSource.Contains("public bool ConsumeRequest()", StringComparison.Ordinal)
                && viewSource.Contains("toolController.ShowAreaResultReview(", StringComparison.Ordinal)
                && viewSource.Contains("verificationGuidePresenter", StringComparison.Ordinal)
                && viewSource.Contains("thresholdTeachingPreviewController.ConsumeRequest()", StringComparison.Ordinal),
            results);
        Check(
            "PropertyGrid presenter and base View retain the binding/public/lifetime contract",
            presenterSource.Contains("internal sealed class VisionToolPropertyGridPresenter<TProperty>", StringComparison.Ordinal)
                && presenterSource.Contains("public TProperty CreateProperty()", StringComparison.Ordinal)
                && presenterSource.Contains("public void PersistSelectedObject()", StringComparison.Ordinal)
                && baseViewSource.Contains("public event EventHandler SourceLayerChanged", StringComparison.Ordinal)
                && baseViewSource.Contains("public virtual void DisposeView()", StringComparison.Ordinal)
                && baseViewSource.Contains("DisposeToolResources();", StringComparison.Ordinal)
                && baseViewSource.Contains("toolController?.Dispose();", StringComparison.Ordinal)
                && viewSource.Contains("public void SetResultReview(IEnumerable<ContourResult> results)", StringComparison.Ordinal)
                && !viewSource.Contains("toolController.Dispose()", StringComparison.Ordinal),
            results);
        Check(
            "Factory owns Contour property-grid composition and preview selection",
            factorySource.Contains("public static OpenVisionNativeToolDocument CreateContour", StringComparison.Ordinal)
                && factorySource.Contains("VisionToolCompositionService.CreateContourToolViewModel", StringComparison.Ordinal)
                && factorySource.Contains("presenter => new ContourToolWpfView(presenter)", StringComparison.Ordinal)
                && factorySource.Contains("OpenVisionNativeToolPreviewExecutor.ExecuteContourPreview", StringComparison.Ordinal)
                && factorySource.Contains("VisionToolAreaVerificationCriteriaText.CreateContour", StringComparison.Ordinal),
            results);
        Check(
            "Document builders own generic PropertyGrid and single-input document wiring",
            documentBuilderSource.Contains("public static OpenVisionNativeToolDocument Create<TView, TProperty>", StringComparison.Ordinal)
                && documentBuilderSource.Contains("CreatePropertyGridView", StringComparison.Ordinal)
                && singleInputBuilderSource.Contains("new OpenVisionNativeToolDocument", StringComparison.Ordinal),
            results);
        Check(
            "Preview executor and overlay own Contour algorithm and result image routing",
            previewExecutorSource.Contains("ExecuteContourPreview(Mat source, ContourToolWpfView view)", StringComparison.Ordinal)
                && previewExecutorSource.Contains("new ContourTool()", StringComparison.Ordinal)
                && overlaySource.Contains("CreateContourResultPreviewImage", StringComparison.Ordinal),
            results);
        Check(
            "Registry and pipeline builder retain Contour creation, warm-host, and step contracts",
            registrySource.Contains("LargeTool(VISION_MENU.Contour, OpenVisionNativePropertyGridToolFactory.CreateContour, nameof(ContourToolWpfView), warmHostedLayout: true)", StringComparison.Ordinal)
                && registrySource.Contains("yield return VISION_MENU.Contour;", StringComparison.Ordinal)
                && pipelineSource.Contains("else if (property is ContourProperty contour)", StringComparison.Ordinal)
                && pipelineSource.Contains("nameof(ContourProperty.MIN_AREA)", StringComparison.Ordinal)
                && pipelineSource.Contains("nameof(ContourProperty.DrawThickness)", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "contour-tool-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "CONTOUR_TOOL_PARTIAL_BOUNDARY_CONTRACT="
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
