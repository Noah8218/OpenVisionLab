using System;
using System.Collections.Generic;
using System.IO;

internal static class VisionToolDoubleInputCustomToolShellPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "double-input-shell-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string sourceRoot = Path.Combine(repositoryRoot, "src", "OpenVisionLab");
        string shellPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "DoubleInput", "VisionToolDoubleInputCustomToolShell.xaml.cs");
        string xamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "DoubleInput", "VisionToolDoubleInputCustomToolShell.xaml");
        string runtimePath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "DoubleInput", "VisionToolDoubleInputCustomToolRuntime.cs");
        string controllerPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "DoubleInput", "VisionToolDoubleInputCustomToolController.cs");
        string baseViewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "DoubleInput", "VisionToolDoubleInputCustomToolViewBase.cs");
        string arithmeticViewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "ToolViews", "ArithmeticToolWpfView.xaml.cs");
        string arithmeticXamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "ToolViews", "ArithmeticToolWpfView.xaml");
        string dockingHelperPath = Path.Combine(sourceRoot, "UI", "Menu", "Wpf", "Docking", "OpenVisionToolDockModeHelper.cs");
        string learnControllerPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "VisionToolLearnWindowController.cs");

        string shellSource = File.ReadAllText(shellPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string runtimeSource = File.ReadAllText(runtimePath);
        string controllerSource = File.ReadAllText(controllerPath);
        string baseViewSource = File.ReadAllText(baseViewPath);
        string arithmeticViewSource = File.ReadAllText(arithmeticViewPath);
        string arithmeticXamlSource = File.ReadAllText(arithmeticXamlPath);
        string dockingHelperSource = File.ReadAllText(dockingHelperPath);
        string learnControllerSource = File.ReadAllText(learnControllerPath);
        List<string> results = new();

        Check(
            "Double-input shell remains the required XAML visual adapter",
            shellSource.Contains("public partial class VisionToolDoubleInputCustomToolShell : UserControl", StringComparison.Ordinal)
                && shellSource.Contains("InitializeComponent();", StringComparison.Ordinal)
                && shellSource.Contains("new DockedInspectorLayoutController(this)", StringComparison.Ordinal)
                && shellSource.Contains("new VisionToolLearnWindowController(() => Window.GetWindow(this))", StringComparison.Ordinal)
                && xamlSource.Contains("<UserControl x:Class=\"OpenVisionLab.VisionToolDoubleInputCustomToolShell\"", StringComparison.Ordinal)
                && xamlSource.Contains("AutomationProperties.AutomationId=\"VisionToolDoubleInputCustomToolShell\"", StringComparison.Ordinal),
            results);
        Check(
            "Shell preserves the dependency-property and namescope binding contract",
            shellSource.Contains("TitleIconKindProperty", StringComparison.Ordinal)
                && shellSource.Contains("ParameterContentProperty", StringComparison.Ordinal)
                && shellSource.Contains("IsDockedInspectorModeProperty", StringComparison.Ordinal)
                && shellSource.Contains("LearnButtonVisibilityProperty", StringComparison.Ordinal)
                && shellSource.Contains("LearnButtonTextProperty", StringComparison.Ordinal)
                && shellSource.Contains("LearnTopicIndexProperty", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"gbInputA\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"gbInputB\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"gbOutputLayer\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"parameterPanel\"", StringComparison.Ordinal),
            results);
        Check(
            "Shell exposes only the existing visual elements and layout toggles",
            shellSource.Contains("public HeaderedContentControl InputAGroup", StringComparison.Ordinal)
                && shellSource.Contains("public VisionToolInlinePreviewSlot InputAPreview", StringComparison.Ordinal)
                && shellSource.Contains("public ComboBox InputAComboBox", StringComparison.Ordinal)
                && shellSource.Contains("public Button RunPreviewButton", StringComparison.Ordinal)
                && shellSource.Contains("public void SetInputBPreviewVisible(bool visible)", StringComparison.Ordinal)
                && shellSource.Contains("public void SetOffsetActionsVisible(bool useOffsetMode)", StringComparison.Ordinal),
            results);
        Check(
            "Docked layout state stays inside the WPF adapter controller",
            shellSource.Contains("private sealed class DockedInspectorLayoutController", StringComparison.Ordinal)
                && shellSource.Contains("private readonly VisionToolDoubleInputCustomToolShell shell", StringComparison.Ordinal)
                && shellSource.Contains("shell.previewColumn.Width", StringComparison.Ordinal)
                && shellSource.Contains("shell.rowRunOffsetAction.Height", StringComparison.Ordinal)
                && shellSource.Contains("ApplyPreviewCardDocking", StringComparison.Ordinal)
                && !shellSource.Contains("VisionPipelineStepBuilder", StringComparison.Ordinal)
                && !shellSource.Contains("OpenVisionNativeTool", StringComparison.Ordinal),
            results);
        Check(
            "Arithmetic composition uses the shell through the existing base View",
            arithmeticXamlSource.Contains("VisionToolDoubleInputCustomToolShell x:Name=\"toolShell\"", StringComparison.Ordinal)
                && arithmeticViewSource.Contains("public partial class ArithmeticToolWpfView : VisionToolDoubleInputCustomToolViewBase", StringComparison.Ordinal)
                && arithmeticViewSource.Contains("AttachToolController(", StringComparison.Ordinal)
                && arithmeticViewSource.Contains("ToolController.SetInputBPreviewVisible", StringComparison.Ordinal)
                && arithmeticViewSource.Contains("ToolController.SetOffsetActionsVisible", StringComparison.Ordinal),
            results);
        Check(
            "Double-input runtime owns preview binding, command callbacks and shell text projection",
            runtimeSource.Contains("VisionToolDoubleInputViewRuntime.Attach", StringComparison.Ordinal)
                && runtimeSource.Contains("inputRuntime.Dispose();", StringComparison.Ordinal)
                && runtimeSource.Contains("VisionToolWpfStatusPresenter.Apply(shell.StatusText, status)", StringComparison.Ordinal)
                && runtimeSource.Contains("shell.ParameterContent = parameterContent", StringComparison.Ordinal)
                && runtimeSource.Contains("Action runPreviewRequested", StringComparison.Ordinal)
                && runtimeSource.Contains("Action<VisionToolPreviewImageRole> loadPreviewImageRequested", StringComparison.Ordinal),
            results);
        Check(
            "Double-input controller owns event forwarding and language/runtime release",
            controllerSource.Contains("VisionToolDoubleInputToolEventHub eventHub", StringComparison.Ordinal)
                && controllerSource.Contains("VisionToolLanguageChangeController.Attach(RefreshLocalization)", StringComparison.Ordinal)
                && controllerSource.Contains("languageChangeController.Dispose();", StringComparison.Ordinal)
                && controllerSource.Contains("toolRuntime.Dispose();", StringComparison.Ordinal)
                && controllerSource.Contains("eventHub.RaiseRunPreviewRequested", StringComparison.Ordinal),
            results);
        Check(
            "Base View remains the mutable lifetime owner",
            baseViewSource.Contains("AttachToolController(", StringComparison.Ordinal)
                && baseViewSource.Contains("public virtual void DisposeView()", StringComparison.Ordinal)
                && baseViewSource.Contains("DisposeToolResources();", StringComparison.Ordinal)
                && baseViewSource.Contains("toolController?.Dispose();", StringComparison.Ordinal)
                && !shellSource.Contains("IDisposable", StringComparison.Ordinal)
                && !shellSource.Contains("DisposeView", StringComparison.Ordinal),
            results);
        Check(
            "Dock mode caller changes only the existing shell presentation state",
            dockingHelperSource.Contains("if (element is VisionToolDoubleInputCustomToolShell doubleInputShell)", StringComparison.Ordinal)
                && dockingHelperSource.Contains("doubleInputShell.IsDockedInspectorMode = isDocked", StringComparison.Ordinal)
                && !dockingHelperSource.Contains("RunPreview", StringComparison.Ordinal)
                && !dockingHelperSource.Contains("CreateProperty", StringComparison.Ordinal),
            results);
        Check(
            "Learn action stays behind the existing window controller",
            shellSource.Contains("private void LearnTopicButton_Click", StringComparison.Ordinal)
                && shellSource.Contains("learnWindowController.Open(LearnTopicIndex)", StringComparison.Ordinal)
                && learnControllerSource.Contains("public void Open(int topicIndex)", StringComparison.Ordinal)
                && !shellSource.Contains("new OpenVisionLearnWindow", StringComparison.Ordinal)
                && !shellSource.Contains("ShowDialog", StringComparison.Ordinal),
            results);
        Check(
            "Shell has no persistence, file, algorithm, or independent workflow policy",
            !shellSource.Contains("System.IO", StringComparison.Ordinal)
                && !shellSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !shellSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !shellSource.Contains("OpenCvSharp", StringComparison.Ordinal)
                && !shellSource.Contains("VisionPipelineStepBuilder", StringComparison.Ordinal)
                && !shellSource.Contains("OpenVisionNativeToolSettingsStore", StringComparison.Ordinal)
                && !shellSource.Contains("new ArithmeticTool", StringComparison.Ordinal),
            results);
        Check(
            "XAML retains the three preview slots, parameter content and Learn command surface",
            xamlSource.Contains("VisionToolInputAPreviewFrame", StringComparison.Ordinal)
                && xamlSource.Contains("VisionToolInputBPreviewFrame", StringComparison.Ordinal)
                && xamlSource.Contains("VisionToolOutputPreviewFrame", StringComparison.Ordinal)
                && xamlSource.Contains("Content=\"{Binding ParameterContent", StringComparison.Ordinal)
                && xamlSource.Contains("Click=\"LearnTopicButton_Click\"", StringComparison.Ordinal)
                && xamlSource.Contains("AutomationProperties.AutomationId=\"VisionToolHeaderLearnButton\"", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "double-input-shell-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "VISION_TOOL_DOUBLE_INPUT_SHELL_PARTIAL_BOUNDARY_CONTRACT="
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
