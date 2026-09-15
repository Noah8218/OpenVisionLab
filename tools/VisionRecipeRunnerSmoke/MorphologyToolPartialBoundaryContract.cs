using System;
using System.Collections.Generic;
using System.IO;

internal static class MorphologyToolPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "morphology-tool-partial-retention-20260914")
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
            "MorphologyToolWpfView.xaml.cs");
        string presenterPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Tooling",
            "PropertyGrid",
            "VisionToolParameterPresenters.cs");
        string viewModelPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "ViewModels",
            "MorphologyToolViewModel.cs");
        string factoryPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "NativeTools",
            "Documents",
            "OpenVisionNativeCustomToolFactory.cs");
        string baseViewPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Tooling",
            "SingleInput",
            "VisionToolSingleInputCustomToolViewBase.cs");
        string viewSource = File.ReadAllText(viewPath);
        string presenterSource = File.ReadAllText(presenterPath);
        string viewModelSource = File.ReadAllText(viewModelPath);
        string factorySource = File.ReadAllText(factoryPath);
        string baseViewSource = File.ReadAllText(baseViewPath);
        List<string> results = new();

        Check(
            "Morphology Partial keeps only presentation/composition collaborators",
            viewSource.Contains("MorphologyToolPresenter presenter", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolParameterChangeController", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolKernelSizeController", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolMorphologyInteractionController", StringComparison.Ordinal)
                && viewSource.Contains("MorphologyToolTextPresenter", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolCustomParameterGuideBinder", StringComparison.Ordinal),
            results);
        Check(
            "Morphology Partial has no direct persistence or file/UI-dialog coupling",
            !viewSource.Contains("OpenVisionNativeToolSettingsStore", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("new MorphologyTool(", StringComparison.Ordinal),
            results);
        Check(
            "Morphology Partial releases its owned controller/lifetime collaborators",
            viewSource.Contains("protected override void DisposeToolResources()", StringComparison.Ordinal)
                && viewSource.Contains("parameterGuideBinder.Dispose();", StringComparison.Ordinal)
                && viewSource.Contains("morphologyInteractionController.Detach();", StringComparison.Ordinal)
                && viewSource.Contains("kernelSizeController.Detach();", StringComparison.Ordinal)
                && viewSource.Contains("previewScheduler.Dispose();", StringComparison.Ordinal),
            results);
        Check(
            "Presenter owns the ViewModel-facing property contract",
            presenterSource.Contains("internal sealed class MorphologyToolPresenter", StringComparison.Ordinal)
                && presenterSource.Contains("private readonly IMorphologyToolViewModel viewModel", StringComparison.Ordinal)
                && presenterSource.Contains("public MorphologyToolProperty CreateProperty()", StringComparison.Ordinal),
            results);
        Check(
            "ViewModel owns mutable parameters and settings persistence",
            viewModelSource.Contains("private MorphTypes operation", StringComparison.Ordinal)
                && viewModelSource.Contains("private int kernelWidth", StringComparison.Ordinal)
                && viewModelSource.Contains("OpenVisionNativeToolSettingsStore.Save", StringComparison.Ordinal)
                && viewModelSource.Contains("public MorphologyToolProperty CreateProperty()", StringComparison.Ordinal),
            results);
        Check(
            "Factory and base View establish creation and release paths",
            factorySource.Contains("CreateMorphologyToolViewModel()", StringComparison.Ordinal)
                && factorySource.Contains("new MorphologyToolPresenter", StringComparison.Ordinal)
                && factorySource.Contains("new MorphologyToolWpfView(viewPresenter)", StringComparison.Ordinal)
                && baseViewSource.Contains("DisposeToolResources();", StringComparison.Ordinal)
                && baseViewSource.Contains("toolController?.Dispose();", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "morphology-tool-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "MORPHOLOGY_TOOL_PARTIAL_BOUNDARY_CONTRACT="
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
