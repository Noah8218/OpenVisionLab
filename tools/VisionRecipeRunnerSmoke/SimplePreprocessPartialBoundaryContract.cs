using System;
using System.Collections.Generic;
using System.IO;

internal static class SimplePreprocessPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "simple-preprocess-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string viewPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "ToolViews", "SimplePreprocessToolWpfView.xaml.cs");
        string xamlPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "ToolViews", "SimplePreprocessToolWpfView.xaml");
        string parameterControllerPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Behaviors", "SimplePreprocessParameterController.cs");
        string textPresenterPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Tooling", "Presentation", "SimplePreprocessTextPresenter.cs");
        string documentFactoryPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "Menu", "Wpf", "NativeTools", "Documents", "OpenVisionNativeSimplePreprocessDocumentFactory.cs");
        string propertyFactoryPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "Menu", "Wpf", "NativeTools", "Documents", "OpenVisionNativeSimplePreprocessPropertyFactory.cs");
        string previewExecutorPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "Menu", "Wpf", "NativeTools", "Preview", "OpenVisionNativeSimplePreprocessPreviewExecutor.cs");
        string registryPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "Menu", "Wpf", "NativeTools", "Runtime", "OpenVisionNativeToolRegistry.cs");
        string baseViewPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputCustomToolViewBase.cs");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string parameterControllerSource = File.ReadAllText(parameterControllerPath);
        string textPresenterSource = File.ReadAllText(textPresenterPath);
        string documentFactorySource = File.ReadAllText(documentFactoryPath);
        string propertyFactorySource = File.ReadAllText(propertyFactoryPath);
        string previewExecutorSource = File.ReadAllText(previewExecutorPath);
        string registrySource = File.ReadAllText(registryPath);
        string baseViewSource = File.ReadAllText(baseViewPath);
        List<string> results = new();

        Check(
            "Simple Preprocess Partial remains a required XAML/composition adapter",
            viewSource.Contains("public partial class SimplePreprocessToolWpfView", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent();", StringComparison.Ordinal)
                && viewSource.Contains("AttachToolController(", StringComparison.Ordinal)
                && viewSource.Contains("SimplePreprocessParameterController parameterController", StringComparison.Ordinal)
                && viewSource.Contains("SimplePreprocessTextPresenter textPresenter", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolDebouncedPreviewScheduler previewScheduler", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolSignalEvidenceExporter.ExportTsv", StringComparison.Ordinal),
            results);
        Check(
            "Simple Preprocess Partial has no direct persistence, dialog, file, or algorithm policy",
            !viewSource.Contains("OpenVisionNativeToolSettingsStore", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("OpenCvSharp", StringComparison.Ordinal)
                && !viewSource.Contains("new EdgeDetectionTool", StringComparison.Ordinal)
                && !viewSource.Contains("new RotateScaleTool", StringComparison.Ordinal)
                && !viewSource.Contains("new MeanTool", StringComparison.Ordinal),
            results);
        Check(
            "Existing parameter, change, preview, guide, and signal owners remain explicit",
            viewSource.Contains("new VisionToolParameterChangeController(", StringComparison.Ordinal)
                && viewSource.Contains("previewScheduler.Schedule", StringComparison.Ordinal)
                && viewSource.Contains("new SimplePreprocessParameterController(", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolCustomParameterGuideBinder.Attach(", StringComparison.Ordinal)
                && viewSource.Contains("parameterGuideBinder?.Dispose();", StringComparison.Ordinal)
                && viewSource.Contains("previewScheduler.Dispose();", StringComparison.Ordinal)
                && viewSource.Contains("signalInspector.ShowEvidence(evidence);", StringComparison.Ordinal)
                && viewSource.Contains("signalInspector.ClearEvidence();", StringComparison.Ordinal),
            results);
        Check(
            "Parameter Controller owns editor state and settings projection",
            parameterControllerSource.Contains("Dictionary<string, ComboBox>", StringComparison.Ordinal)
                && parameterControllerSource.Contains("Parameter_Changed", StringComparison.Ordinal)
                && parameterControllerSource.Contains("CaptureSettings()", StringComparison.Ordinal)
                && parameterControllerSource.Contains("ApplySettings(SimplePreprocessToolSettings settings)", StringComparison.Ordinal)
                && parameterControllerSource.Contains("CreateParameterGuideBindings", StringComparison.Ordinal)
                && !parameterControllerSource.Contains("OpenVisionNativeToolSettingsStore", StringComparison.Ordinal),
            results);
        Check(
            "Text and result presentation remain in concrete presenters/executors",
            textPresenterSource.Contains("internal sealed class SimplePreprocessTextPresenter", StringComparison.Ordinal)
                && textPresenterSource.Contains("ApplyLocalization", StringComparison.Ordinal)
                && previewExecutorSource.Contains("new EdgeDetectionTool()", StringComparison.Ordinal)
                && previewExecutorSource.Contains("new RotateScaleTool()", StringComparison.Ordinal)
                && previewExecutorSource.Contains("new MeanTool()", StringComparison.Ordinal)
                && previewExecutorSource.Contains("ShowResultReview", StringComparison.Ordinal)
                && previewExecutorSource.Contains("ShowSignalEvidence", StringComparison.Ordinal),
            results);
        Check(
            "Document and property factories own persistence, creation, and property contracts",
            documentFactorySource.Contains("OpenVisionNativeToolSettingsStore.Load", StringComparison.Ordinal)
                && documentFactorySource.Contains("OpenVisionNativeToolSettingsStore.Save", StringComparison.Ordinal)
                && documentFactorySource.Contains("OpenVisionNativeSingleInputToolDocumentBuilder.Create", StringComparison.Ordinal)
                && propertyFactorySource.Contains("CreateEdgeDetectionProperty(SimplePreprocessToolWpfView view)", StringComparison.Ordinal)
                && propertyFactorySource.Contains("CreateRotateScaleProperty(SimplePreprocessToolWpfView view)", StringComparison.Ordinal)
                && propertyFactorySource.Contains("CreateMeanProperty(SimplePreprocessToolWpfView view)", StringComparison.Ordinal),
            results);
        Check(
            "XAML binding and public/test facade remain stable",
            xamlSource.Contains("x:Class=\"OpenVisionLab.SimplePreprocessToolWpfView\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"toolShell\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"parameterContentHost\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"parameterPanel\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"signalInspector\"", StringComparison.Ordinal)
                && viewSource.Contains("internal SimplePreprocessParameterController Parameters", StringComparison.Ordinal)
                && viewSource.Contains("SignalInspectorEvidenceIdForTest", StringComparison.Ordinal)
                && viewSource.Contains("ExportSignalEvidenceForTest", StringComparison.Ordinal),
            results);
        Check(
            "Registry and base View preserve creation and release ownership",
            registrySource.Contains("OpenVisionNativeSimplePreprocessDocumentFactory.CreateEdgeDetectionDocument", StringComparison.Ordinal)
                && registrySource.Contains("OpenVisionNativeSimplePreprocessDocumentFactory.CreateRotateScaleDocument", StringComparison.Ordinal)
                && registrySource.Contains("OpenVisionNativeSimplePreprocessDocumentFactory.CreateHsvDocument", StringComparison.Ordinal)
                && registrySource.Contains("OpenVisionNativeSimplePreprocessDocumentFactory.CreateMeanDocument", StringComparison.Ordinal)
                && registrySource.Contains("OpenVisionNativeSimplePreprocessDocumentFactory.CreateHistogramDocument", StringComparison.Ordinal)
                && baseViewSource.Contains("DisposeToolResources();", StringComparison.Ordinal)
                && baseViewSource.Contains("toolController?.Dispose();", StringComparison.Ordinal),
            results);
        Check(
            "Simple Preprocess View does not duplicate mutable or lifetime owners",
            !viewSource.Contains("OpenVisionNativeSimplePreprocessPreviewExecutor", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionNativeSimplePreprocessPropertyFactory", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionNativeToolSettingsStore", StringComparison.Ordinal)
                && viewSource.Contains("protected override void DisposeToolResources()", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "simple-preprocess-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "SIMPLE_PREPROCESS_PARTIAL_BOUNDARY_CONTRACT="
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
