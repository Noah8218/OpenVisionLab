using System;
using System.Collections.Generic;
using System.IO;

internal static class BlobToolPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "blob-tool-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string sourceRoot = Path.Combine(repositoryRoot, "src", "OpenVisionLab");
        string viewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "ToolViews", "BlobToolWpfView.xaml.cs");
        string xamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "ToolViews", "BlobToolWpfView.xaml");
        string viewModelPath = Path.Combine(sourceRoot, "UI", "VisionTest", "ViewModels", "BlobToolViewModel.cs");
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
            "Blob Partial remains the required XAML/PropertyGrid adapter",
            viewSource.Contains("public partial class BlobToolWpfView : VisionToolSingleInputPropertyToolViewBase", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolPropertyGridPresenter<BlobProperty>", StringComparison.Ordinal)
                && viewSource.Contains("new VisionToolAreaVerificationGuidePresenter<BlobProperty, BlobResult>", StringComparison.Ordinal)
                && viewSource.Contains("new VisionToolThresholdTeachingPreviewController", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolSingleInputPropertyToolController<BlobProperty>.Attach", StringComparison.Ordinal)
                && viewSource.Contains("AttachPropertyToolController(toolController)", StringComparison.Ordinal)
                && xamlSource.Contains("x:Class=\"OpenVisionLab.BlobToolWpfView\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"toolShell\"", StringComparison.Ordinal)
                && xamlSource.Contains("LearnButtonText=\"Learn Blob\"", StringComparison.Ordinal)
                && xamlSource.Contains("LearnTopicIndex=\"5\"", StringComparison.Ordinal)
                && xamlSource.Contains("VisionToolVerificationGuideView", StringComparison.Ordinal),
            results);
        Check(
            "Blob Partial has no direct persistence, dialog, file, or algorithm coupling",
            !viewSource.Contains("OpenVisionNativeToolPropertySessionStore", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("OpenCvSharp", StringComparison.Ordinal)
                && !viewSource.Contains("new BlobTool", StringComparison.Ordinal)
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
            "Blob ViewModel and composition service own property state, normalization, summary, and ViewModel creation",
            viewModelSource.Contains("private readonly BlobProperty property", StringComparison.Ordinal)
                && viewModelSource.Contains("public BlobProperty CreateProperty()", StringComparison.Ordinal)
                && viewModelSource.Contains("NormalizeRanges()", StringComparison.Ordinal)
                && viewModelSource.Contains("public string Summary", StringComparison.Ordinal)
                && compositionSource.Contains("CreateBlobToolViewModel(BlobProperty property)", StringComparison.Ordinal)
                && compositionSource.Contains("new BlobToolViewModel(property)", StringComparison.Ordinal),
            results);
        Check(
            "Area verification and threshold-teaching owners keep inspection policy out of the View",
            guidePresenterSource.Contains("ShowTeachingState(TProperty property)", StringComparison.Ordinal)
                && guidePresenterSource.Contains("ShowResult(IEnumerable<TResult> results, TProperty property)", StringComparison.Ordinal)
                && guideCriteriaSource.Contains("CreateBlob(BlobProperty property)", StringComparison.Ordinal)
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
                && viewSource.Contains("public void SetResultReview(IEnumerable<BlobResult> results)", StringComparison.Ordinal)
                && !viewSource.Contains("toolController.Dispose()", StringComparison.Ordinal),
            results);
        Check(
            "Factory owns Blob property-grid composition and preview selection",
            factorySource.Contains("public static OpenVisionNativeToolDocument CreateBlob", StringComparison.Ordinal)
                && factorySource.Contains("VisionToolCompositionService.CreateBlobToolViewModel", StringComparison.Ordinal)
                && factorySource.Contains("presenter => new BlobToolWpfView(presenter)", StringComparison.Ordinal)
                && factorySource.Contains("OpenVisionNativeToolPreviewExecutor.ExecuteBlobPreview", StringComparison.Ordinal)
                && factorySource.Contains("VisionToolAreaVerificationCriteriaText.CreateBlob", StringComparison.Ordinal),
            results);
        Check(
            "Document builders own generic PropertyGrid and single-input document wiring",
            documentBuilderSource.Contains("public static OpenVisionNativeToolDocument Create<TView, TProperty>", StringComparison.Ordinal)
                && documentBuilderSource.Contains("CreatePropertyGridView", StringComparison.Ordinal)
                && singleInputBuilderSource.Contains("new OpenVisionNativeToolDocument", StringComparison.Ordinal),
            results);
        Check(
            "Preview executor and overlay own Blob algorithm and result image routing",
            previewExecutorSource.Contains("ExecuteBlobPreview(Mat source, BlobToolWpfView view)", StringComparison.Ordinal)
                && previewExecutorSource.Contains("new BlobTool()", StringComparison.Ordinal)
                && overlaySource.Contains("CreateBlobResultPreviewImage", StringComparison.Ordinal),
            results);
        Check(
            "Registry and pipeline builder retain Blob creation, warm-host, and step contracts",
            registrySource.Contains("LargeTool(VISION_MENU.Blob, OpenVisionNativePropertyGridToolFactory.CreateBlob, nameof(BlobToolWpfView), warmHostedLayout: true)", StringComparison.Ordinal)
                && registrySource.Contains("yield return VISION_MENU.Blob;", StringComparison.Ordinal)
                && pipelineSource.Contains("if (property is BlobProperty blob)", StringComparison.Ordinal)
                && pipelineSource.Contains("nameof(BlobProperty.MIN_AREA)", StringComparison.Ordinal)
                && pipelineSource.Contains("nameof(BlobProperty.MAX_HEIGHT)", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "blob-tool-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "BLOB_TOOL_PARTIAL_BOUNDARY_CONTRACT="
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
