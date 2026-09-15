using System;
using System.Collections.Generic;
using System.IO;

internal static class ArithmeticToolPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "arithmetic-tool-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string viewPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "ToolViews", "ArithmeticToolWpfView.xaml.cs");
        string xamlPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "ToolViews", "ArithmeticToolWpfView.xaml");
        string interactionPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Behaviors", "ArithmeticToolInteractionController.cs");
        string textPresenterPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Tooling", "Presentation", "ArithmeticToolTextPresenter.cs");
        string previewControllerPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Tooling", "Preview", "ArithmeticToolPreviewController.cs");
        string baseViewPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Tooling", "DoubleInput", "VisionToolDoubleInputCustomToolViewBase.cs");
        string controllerPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Tooling", "DoubleInput", "VisionToolDoubleInputCustomToolController.cs");
        string viewModelPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Tooling", "DoubleInput", "VisionToolDoubleInputViewModel.cs");
        string binderPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Tooling", "DoubleInput", "VisionToolDoubleInputViewBinder.cs");
        string factoryPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "Menu", "Wpf", "NativeTools", "Documents", "OpenVisionNativeArithmeticDocumentFactory.cs");
        string documentPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "Menu", "Wpf", "NativeTools", "Documents", "OpenVisionNativeToolDocument.cs");
        string registryPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "Menu", "Wpf", "NativeTools", "Runtime", "OpenVisionNativeToolRegistry.cs");
        string contractPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Tooling", "Contracts", "IArithmeticVisionToolWpfView.cs");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string interactionSource = File.ReadAllText(interactionPath);
        string textPresenterSource = File.ReadAllText(textPresenterPath);
        string previewControllerSource = File.ReadAllText(previewControllerPath);
        string baseViewSource = File.ReadAllText(baseViewPath);
        string controllerSource = File.ReadAllText(controllerPath);
        string viewModelSource = File.ReadAllText(viewModelPath);
        string binderSource = File.ReadAllText(binderPath);
        string factorySource = File.ReadAllText(factoryPath);
        string documentSource = File.ReadAllText(documentPath);
        string registrySource = File.ReadAllText(registryPath);
        string contractSource = File.ReadAllText(contractPath);
        List<string> results = new();

        Check(
            "Arithmetic Partial remains the required XAML/double-input adapter",
            viewSource.Contains("public partial class ArithmeticToolWpfView : VisionToolDoubleInputCustomToolViewBase, IArithmeticVisionToolWpfView", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent();", StringComparison.Ordinal)
                && viewSource.Contains("AttachToolController(", StringComparison.Ordinal)
                && viewSource.Contains("new ArithmeticToolInteractionController(", StringComparison.Ordinal)
                && viewSource.Contains("new ArithmeticToolTextPresenter(", StringComparison.Ordinal)
                && xamlSource.Contains("x:Class=\"OpenVisionLab.ArithmeticToolWpfView\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"toolShell\"", StringComparison.Ordinal)
                && xamlSource.Contains("LearnTopicIndex=\"14\"", StringComparison.Ordinal),
            results);
        Check(
            "Arithmetic Partial has no direct persistence, dialog, file, or algorithm coupling",
            !viewSource.Contains("OpenVisionNativeToolSettingsStore", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("OpenCvSharp", StringComparison.Ordinal)
                && !viewSource.Contains("new ArithmeticTool()", StringComparison.Ordinal)
                && !viewSource.Contains("VisionPipelineArithmeticStep", StringComparison.Ordinal),
            results);
        Check(
            "Existing interaction owner contains Arithmetic mutable editor state and event policy",
            interactionSource.Contains("internal sealed class ArithmeticToolInteractionController", StringComparison.Ordinal)
                && interactionSource.Contains("CaptureSettings()", StringComparison.Ordinal)
                && interactionSource.Contains("ApplySettings(ArithmeticToolSettings settings)", StringComparison.Ordinal)
                && interactionSource.Contains("AttachEvents()", StringComparison.Ordinal)
                && interactionSource.Contains("Detach()", StringComparison.Ordinal)
                && interactionSource.Contains("RefreshMode()", StringComparison.Ordinal)
                && viewSource.Contains("interactionController?.CaptureSettings()", StringComparison.Ordinal)
                && viewSource.Contains("interactionController?.ApplySettings(settings)", StringComparison.Ordinal),
            results);
        Check(
            "Existing text presenter owns localization and summary projection",
            textPresenterSource.Contains("internal sealed class ArithmeticToolTextPresenter", StringComparison.Ordinal)
                && textPresenterSource.Contains("ApplyLocalization()", StringComparison.Ordinal)
                && textPresenterSource.Contains("RefreshSummary()", StringComparison.Ordinal)
                && textPresenterSource.Contains("ArithmeticToolTextState", StringComparison.Ordinal)
                && viewSource.Contains("textPresenter.RefreshSummary", StringComparison.Ordinal)
                && viewSource.Contains("textPresenter?.ApplyLocalization()", StringComparison.Ordinal),
            results);
        Check(
            "Existing preview scheduler owns debounce and mode-specific run routing",
            previewControllerSource.Contains("internal sealed class ArithmeticToolPreviewController : IDisposable", StringComparison.Ordinal)
                && previewControllerSource.Contains("VisionToolDebouncedPreviewScheduler", StringComparison.Ordinal)
                && previewControllerSource.Contains("toolController.RequestRunOffset()", StringComparison.Ordinal)
                && previewControllerSource.Contains("toolController.RequestRunPreview()", StringComparison.Ordinal)
                && viewSource.Contains("previewController.ScheduleAutoPreview", StringComparison.Ordinal)
                && viewSource.Contains("previewController?.Dispose()", StringComparison.Ordinal),
            results);
        Check(
            "Shared double-input controller, ViewModel and binder own binding, commands, previews, and release",
            controllerSource.Contains("internal sealed class VisionToolDoubleInputCustomToolController : IDisposable", StringComparison.Ordinal)
                && controllerSource.Contains("public static VisionToolDoubleInputCustomToolController Attach", StringComparison.Ordinal)
                && controllerSource.Contains("VisionToolDoubleInputCustomToolRuntime.Attach", StringComparison.Ordinal)
                && controllerSource.Contains("languageChangeController.Dispose()", StringComparison.Ordinal)
                && viewModelSource.Contains("RunOffsetCommand", StringComparison.Ordinal)
                && viewModelSource.Contains("NotifyInputALayerChanged", StringComparison.Ordinal)
                && binderSource.Contains("VisionToolActionBehavior.AttachArithmetic", StringComparison.Ordinal)
                && binderSource.Contains("VisionToolLayerSelectionBehavior.AttachDual", StringComparison.Ordinal)
                && binderSource.Contains("inputAPreview.DisposeView()", StringComparison.Ordinal),
            results);
        Check(
            "Arithmetic factory and document own settings persistence, routing, and pipeline composition",
            factorySource.Contains("internal static OpenVisionNativeToolDocument Create(IDisplayManager displayManager)", StringComparison.Ordinal)
                && factorySource.Contains("OpenVisionNativeToolSettingsStore.Load", StringComparison.Ordinal)
                && factorySource.Contains("OpenVisionNativeToolSettingsStore.Save", StringComparison.Ordinal)
                && factorySource.Contains("new OpenVisionNativeToolDocument(", StringComparison.Ordinal)
                && documentSource.Contains("IArithmeticVisionToolWpfView arithmeticView", StringComparison.Ordinal)
                && documentSource.Contains("OpenVisionNativeToolEventBinder.BindArithmetic", StringComparison.Ordinal)
                && documentSource.Contains("CreateArithmeticStep", StringComparison.Ordinal),
            results);
        Check(
            "Registry and public/test contract retain the Arithmetic creation and facade path",
            registrySource.Contains("Tool(VISION_MENU.Arithmetic, OpenVisionNativeArithmeticDocumentFactory.Create, nameof(ArithmeticToolWpfView))", StringComparison.Ordinal)
                && contractSource.Contains("public interface IArithmeticVisionToolWpfView", StringComparison.Ordinal)
                && contractSource.Contains("event EventHandler RunOffsetRequested", StringComparison.Ordinal)
                && contractSource.Contains("void SetOperationList", StringComparison.Ordinal)
                && contractSource.Contains("int GetOffsetY", StringComparison.Ordinal)
                && viewSource.Contains("public event EventHandler ParameterChanged", StringComparison.Ordinal)
                && viewSource.Contains("public ArithmeticToolSettings CaptureSettings()", StringComparison.Ordinal),
            results);
        Check(
            "Base View remains the final lifetime owner and the Partial does not duplicate disposal",
            baseViewSource.Contains("DisposeToolResources();", StringComparison.Ordinal)
                && baseViewSource.Contains("toolController?.Dispose();", StringComparison.Ordinal)
                && viewSource.Contains("protected override void DisposeToolResources()", StringComparison.Ordinal)
                && viewSource.Contains("interactionController?.Detach();", StringComparison.Ordinal)
                && viewSource.Contains("previewController?.Dispose();", StringComparison.Ordinal)
                && !viewSource.Contains("toolController?.Dispose()", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "arithmetic-tool-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "ARITHMETIC_TOOL_PARTIAL_BOUNDARY_CONTRACT="
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
