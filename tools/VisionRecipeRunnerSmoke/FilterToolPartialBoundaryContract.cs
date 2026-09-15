using System;
using System.Collections.Generic;
using System.IO;

internal static class FilterToolPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "filter-tool-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string sourceRoot = Path.Combine(repositoryRoot, "src", "OpenVisionLab");
        string viewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "ToolViews", "FilterToolWpfView.xaml.cs");
        string xamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "ToolViews", "FilterToolWpfView.xaml");
        string interactionPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Behaviors", "VisionToolFilterInteractionController.cs");
        string kernelControllerPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Behaviors", "VisionToolKernelSizeController.cs");
        string parameterChangePath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "PropertyGrid", "VisionToolParameterChangeController.cs");
        string textPresenterPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "Presentation", "FilterToolTextPresenter.cs");
        string guideBinderPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "PropertyGuide", "VisionToolCustomParameterGuideBinder.cs");
        string presenterPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "PropertyGrid", "VisionToolParameterPresenters.cs");
        string viewModelPath = Path.Combine(sourceRoot, "UI", "VisionTest", "ViewModels", "FilterToolViewModel.cs");
        string compositionPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Composition", "VisionToolCompositionService.cs");
        string baseViewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputCustomToolViewBase.cs");
        string controllerPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputCustomToolController.cs");
        string runtimePath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputCustomToolRuntime.cs");
        string factoryPath = Path.Combine(sourceRoot, "UI", "Menu", "Wpf", "NativeTools", "Documents", "OpenVisionNativeCustomToolFactory.cs");
        string documentBuilderPath = Path.Combine(sourceRoot, "UI", "Menu", "Wpf", "NativeTools", "Documents", "OpenVisionNativeCustomToolDocumentBuilder.cs");
        string registryPath = Path.Combine(sourceRoot, "UI", "Menu", "Wpf", "NativeTools", "Runtime", "OpenVisionNativeToolRegistry.cs");
        string contractPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "Contracts", "ISingleInputVisionToolWpfView.cs");
        string pipelinePath = Path.Combine(sourceRoot, "Core", "Pipeline", "Definition", "VisionPipelineStepBuilder.cs");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string interactionSource = File.ReadAllText(interactionPath);
        string kernelControllerSource = File.ReadAllText(kernelControllerPath);
        string parameterChangeSource = File.ReadAllText(parameterChangePath);
        string textPresenterSource = File.ReadAllText(textPresenterPath);
        string guideBinderSource = File.ReadAllText(guideBinderPath);
        string presenterSource = File.ReadAllText(presenterPath);
        string viewModelSource = File.ReadAllText(viewModelPath);
        string compositionSource = File.ReadAllText(compositionPath);
        string baseViewSource = File.ReadAllText(baseViewPath);
        string controllerSource = File.ReadAllText(controllerPath);
        string runtimeSource = File.ReadAllText(runtimePath);
        string factorySource = File.ReadAllText(factoryPath);
        string documentBuilderSource = File.ReadAllText(documentBuilderPath);
        string registrySource = File.ReadAllText(registryPath);
        string contractSource = File.ReadAllText(contractPath);
        string pipelineSource = File.ReadAllText(pipelinePath);
        List<string> results = new();

        Check(
            "Filter Partial remains the required XAML/custom-tool adapter",
            viewSource.Contains("public partial class FilterToolWpfView : VisionToolSingleInputCustomToolViewBase, ISingleInputPropertyVisionToolWpfView<FilterToolProperty>", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent();", StringComparison.Ordinal)
                && viewSource.Contains("AttachToolController(", StringComparison.Ordinal)
                && viewSource.Contains("new VisionToolFilterInteractionController(", StringComparison.Ordinal)
                && viewSource.Contains("new VisionToolKernelSizeController(", StringComparison.Ordinal)
                && viewSource.Contains("new FilterToolTextPresenter(", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolCustomParameterGuideBinder.Attach(", StringComparison.Ordinal)
                && xamlSource.Contains("x:Class=\"OpenVisionLab.FilterToolWpfView\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"toolShell\"", StringComparison.Ordinal)
                && xamlSource.Contains("LearnButtonText=\"Learn Filter\"", StringComparison.Ordinal)
                && xamlSource.Contains("LearnTopicIndex=\"3\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"parameterContentHost\"", StringComparison.Ordinal),
            results);
        Check(
            "Filter Partial has no direct persistence, dialog, file, or algorithm coupling",
            !viewSource.Contains("OpenVisionNativeToolSettingsStore", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("OpenCvSharp", StringComparison.Ordinal)
                && !viewSource.Contains("new FilterTool()", StringComparison.Ordinal)
                && !viewSource.Contains("FromFilterProperty", StringComparison.Ordinal),
            results);
        Check(
            "Filter interaction controller owns selection events, binding flush, and mode visibility",
            interactionSource.Contains("internal sealed class VisionToolFilterInteractionController", StringComparison.Ordinal)
                && interactionSource.Contains("SelectionChanged +=", StringComparison.Ordinal)
                && interactionSource.Contains("SelectionChanged -=", StringComparison.Ordinal)
                && interactionSource.Contains("InitializeOptions()", StringComparison.Ordinal)
                && interactionSource.Contains("RefreshModePanels()", StringComparison.Ordinal)
                && interactionSource.Contains("FlushParameterBindings()", StringComparison.Ordinal)
                && viewSource.Contains("filterInteractionController.InitializeOptions();", StringComparison.Ordinal)
                && viewSource.Contains("filterInteractionController.Detach();", StringComparison.Ordinal),
            results);
        Check(
            "Existing kernel and parameter-change owners keep input policy and preview scheduling out of the View",
            kernelControllerSource.Contains("TextChanged +=", StringComparison.Ordinal)
                && kernelControllerSource.Contains("Checked +=", StringComparison.Ordinal)
                && kernelControllerSource.Contains("Unchecked +=", StringComparison.Ordinal)
                && kernelControllerSource.Contains("Click +=", StringComparison.Ordinal)
                && kernelControllerSource.Contains("ApplyPreset(int size)", StringComparison.Ordinal)
                && kernelControllerSource.Contains("Detach()", StringComparison.Ordinal)
                && parameterChangeSource.Contains("TryHandle", StringComparison.Ordinal)
                && parameterChangeSource.Contains("RefreshProgrammatic", StringComparison.Ordinal)
                && parameterChangeSource.Contains("schedulePreview", StringComparison.Ordinal)
                && viewSource.Contains("previewScheduler.Schedule", StringComparison.Ordinal),
            results);
        Check(
            "Text presenter and guide binder own localization and parameter-help projection",
            textPresenterSource.Contains("internal sealed class FilterToolTextPresenter", StringComparison.Ordinal)
                && textPresenterSource.Contains("ApplyLocalization()", StringComparison.Ordinal)
                && textPresenterSource.Contains("PropertyGrid.Property.FilterType.DisplayName", StringComparison.Ordinal)
                && guideBinderSource.Contains("VisionToolCustomParameterGuideBinder", StringComparison.Ordinal)
                && guideBinderSource.Contains("Keyboard.GotKeyboardFocusEvent", StringComparison.Ordinal)
                && guideBinderSource.Contains("Dispose()", StringComparison.Ordinal)
                && viewSource.Contains("textPresenter.ApplyLocalization();", StringComparison.Ordinal)
                && viewSource.Contains("parameterGuideBinder.Dispose();", StringComparison.Ordinal),
            results);
        Check(
            "Presenter and ViewModel own the binding facade, mutable parameters, normalization, summary, and settings",
            presenterSource.Contains("internal sealed class FilterToolPresenter", StringComparison.Ordinal)
                && presenterSource.Contains("private readonly IFilterToolViewModel viewModel", StringComparison.Ordinal)
                && presenterSource.Contains("public FilterToolProperty CreateProperty()", StringComparison.Ordinal)
                && presenterSource.Contains("SetKernelPreset(int size)", StringComparison.Ordinal)
                && presenterSource.Contains("SyncKernelHeightToWidth()", StringComparison.Ordinal)
                && viewModelSource.Contains("private FilterToolType filterType", StringComparison.Ordinal)
                && viewModelSource.Contains("public FilterToolProperty CreateProperty()", StringComparison.Ordinal)
                && viewModelSource.Contains("NormalizeOddKernelSize", StringComparison.Ordinal)
                && viewModelSource.Contains("OpenVisionNativeToolSettingsStore.Save", StringComparison.Ordinal)
                && viewSource.Contains("return presenter.CreateProperty();", StringComparison.Ordinal),
            results);
        Check(
            "Shared custom-tool controller/runtime/base own layer, preview, summary, language, and release",
            controllerSource.Contains("internal sealed class VisionToolSingleInputCustomToolController : IDisposable", StringComparison.Ordinal)
                && controllerSource.Contains("VisionToolSingleInputCustomToolRuntime.Attach", StringComparison.Ordinal)
                && controllerSource.Contains("languageChangeController.Dispose();", StringComparison.Ordinal)
                && runtimeSource.Contains("VisionToolSingleInputViewRuntime.Attach", StringComparison.Ordinal)
                && runtimeSource.Contains("BindSummary(BindingBase binding)", StringComparison.Ordinal)
                && runtimeSource.Contains("ShowResultReview(", StringComparison.Ordinal)
                && baseViewSource.Contains("DisposeToolResources();", StringComparison.Ordinal)
                && baseViewSource.Contains("toolController?.Dispose();", StringComparison.Ordinal)
                && !viewSource.Contains("toolController?.Dispose()", StringComparison.Ordinal),
            results);
        Check(
            "Composition, factory, document builder, and pipeline builder own creation, persistence, preview, and routing",
            compositionSource.Contains("CreateFilterToolViewModel()", StringComparison.Ordinal)
                && compositionSource.Contains("OpenVisionNativeToolSettingsStore.Load", StringComparison.Ordinal)
                && factorySource.Contains("public static OpenVisionNativeToolDocument CreateFilter", StringComparison.Ordinal)
                && factorySource.Contains("new FilterToolPresenter", StringComparison.Ordinal)
                && factorySource.Contains("new FilterToolWpfView(viewPresenter)", StringComparison.Ordinal)
                && factorySource.Contains("new FilterTool()", StringComparison.Ordinal)
                && factorySource.Contains("VisionPipelineStepBuilder.FromFilterProperty", StringComparison.Ordinal)
                && documentBuilderSource.Contains("ExecutePropertyTool", StringComparison.Ordinal)
                && documentBuilderSource.Contains("OpenVisionNativeSingleInputToolDocumentBuilder.Create", StringComparison.Ordinal)
                && pipelineSource.Contains("FromFilterProperty", StringComparison.Ordinal),
            results);
        Check(
            "Registry and public/test contract retain the Filter creation and facade path",
            registrySource.Contains("Tool(VISION_MENU.Filter, OpenVisionNativeCustomToolFactory.CreateFilter, nameof(FilterToolWpfView))", StringComparison.Ordinal)
                && contractSource.Contains("ISingleInputPropertyVisionToolWpfView<TProperty>", StringComparison.Ordinal)
                && contractSource.Contains("TProperty CreateProperty();", StringComparison.Ordinal)
                && viewSource.Contains("public FilterToolProperty CreateProperty()", StringComparison.Ordinal)
                && baseViewSource.Contains("ResultReviewTextForTest", StringComparison.Ordinal)
                && xamlSource.Contains("SelectedItem=\"{Binding FilterType", StringComparison.Ordinal)
                && xamlSource.Contains("Text=\"{Binding KernelWidth", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "filter-tool-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "FILTER_TOOL_PARTIAL_BOUNDARY_CONTRACT="
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
