using System;
using System.Collections.Generic;
using System.IO;

internal static class EdgeBasedMatchingPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "edge-based-mpoint-partial-retention-20260914")
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
            "EdgeBasedMatchingToolWpfView.xaml.cs");
        string panelPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "ToolViews",
            "AutoMPointTeachingPanel.xaml.cs");
        string panelXamlPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "ToolViews",
            "AutoMPointTeachingPanel.xaml");
        string controllerPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Tooling",
            "Review",
            "AutoMPointTeachingController.cs");
        string exporterPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Tooling",
            "Review",
            "AutoMPointHtmlReportExporter.cs");
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
        string panelSource = File.ReadAllText(panelPath);
        string panelXamlSource = File.ReadAllText(panelXamlPath);
        string controllerSource = File.ReadAllText(controllerPath);
        string exporterSource = File.ReadAllText(exporterPath);
        string factorySource = File.ReadAllText(factoryPath);
        string compositionSource = File.ReadAllText(compositionPath);
        string baseViewSource = File.ReadAllText(baseViewPath);
        List<string> results = new();

        Check(
            "Edge Based Matching Partial remains a XAML/composition adapter",
            viewSource.Contains("public partial class EdgeBasedMatchingToolWpfView", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent", StringComparison.Ordinal)
                && viewSource.Contains("AutoMPointTeachingPanel autoMPointPanel", StringComparison.Ordinal)
                && viewSource.Contains("VisionToolVerificationGuideView verificationGuide", StringComparison.Ordinal)
                && viewSource.Contains("new AutoMPointTeachingController(autoMPointPanel, toolController)", StringComparison.Ordinal)
                && viewSource.Contains("Grid.SetRow(verificationGuide, 0)", StringComparison.Ordinal)
                && viewSource.Contains("Grid.SetRow(autoMPointPanel, 1)", StringComparison.Ordinal),
            results);
        Check(
            "Edge Based Matching Partial has no direct Auto MPoint persistence or algorithm coupling",
            !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("new AutoMPointTool(", StringComparison.Ordinal)
                && !viewSource.Contains("AutoMPointHtmlReportExporter", StringComparison.Ordinal)
                && !viewSource.Contains("SaveTemplateImageForTeaching", StringComparison.Ordinal),
            results);
        Check(
            "Auto MPoint Panel Partial remains presentation and localization only",
            panelSource.Contains("public partial class AutoMPointTeachingPanel : UserControl, IDisposable", StringComparison.Ordinal)
                && panelSource.Contains("VisionToolLanguageChangeController", StringComparison.Ordinal)
                && panelSource.Contains("ApplyLocalization", StringComparison.Ordinal)
                && !panelSource.Contains("AutoMPointTool", StringComparison.Ordinal)
                && !panelSource.Contains("AutoMPointCandidateResult", StringComparison.Ordinal)
                && !panelSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !panelSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !panelSource.Contains("System.IO", StringComparison.Ordinal),
            results);
        Check(
            "Auto MPoint Panel binding and automation contract remains explicit",
            panelXamlSource.Contains("AutoMPointAnalyzeCandidates", StringComparison.Ordinal)
                && panelXamlSource.Contains("AutoMPointCandidateList", StringComparison.Ordinal)
                && panelXamlSource.Contains("AutoMPointExportReport", StringComparison.Ordinal)
                && panelXamlSource.Contains("AutoMPointUsePattern", StringComparison.Ordinal)
                && panelSource.Contains("internal Button AnalyzeButton", StringComparison.Ordinal)
                && panelSource.Contains("internal ListBox CandidateList", StringComparison.Ordinal),
            results);
        Check(
            "Auto MPoint Controller owns mutable teaching state, dialog workflow, and event lifetime",
            controllerSource.Contains("List<string> representativeImagePaths", StringComparison.Ordinal)
                && controllerSource.Contains("Bitmap sourceBitmap", StringComparison.Ordinal)
                && controllerSource.Contains("analyzedSourceRevision", StringComparison.Ordinal)
                && controllerSource.Contains("appliedTemplatePath", StringComparison.Ordinal)
                && controllerSource.Contains("panel.AnalyzeButton.Click += AnalyzeCandidates", StringComparison.Ordinal)
                && controllerSource.Contains("panel.AnalyzeButton.Click -= AnalyzeCandidates", StringComparison.Ordinal)
                && controllerSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && controllerSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && controllerSource.Contains("AutoMPointHtmlReportExporter.TryExport", StringComparison.Ordinal)
                && controllerSource.Contains("new AutoMPointTool()", StringComparison.Ordinal),
            results);
        Check(
            "Edge Based Matching test/public facades delegate to existing owners",
            viewSource.Contains("autoMPointController.ExportSelectedReport(reportPath)", StringComparison.Ordinal)
                && viewSource.Contains("autoMPointController.SetRepresentativeImages(paths)", StringComparison.Ordinal)
                && viewSource.Contains("toolController.CreateProperty()", StringComparison.Ordinal)
                && viewSource.Contains("autoMPointController.SetInputPreview(image)", StringComparison.Ordinal),
            results);
        Check(
            "Factory, composition, and base View establish creation and release paths",
            factorySource.Contains("CreateEdgeBasedMatchingToolViewModel(item)", StringComparison.Ordinal)
                && factorySource.Contains("new EdgeBasedMatchingToolWpfView(presenter)", StringComparison.Ordinal)
                && compositionSource.Contains("CreateEdgeBasedMatchingToolViewModel", StringComparison.Ordinal)
                && baseViewSource.Contains("DisposeToolResources();", StringComparison.Ordinal)
                && baseViewSource.Contains("toolController?.Dispose();", StringComparison.Ordinal),
            results);
        Check(
            "Report export remains in the existing concrete report owner",
            exporterSource.Contains("internal static class AutoMPointHtmlReportExporter", StringComparison.Ordinal)
                && exporterSource.Contains("internal static bool TryExport(", StringComparison.Ordinal)
                && exporterSource.Contains("File.WriteAllText(temporaryPath", StringComparison.Ordinal)
                && exporterSource.Contains("File.Move(temporaryPath, fullReportPath, true)", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "edge-based-mpoint-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "EDGE_BASED_MATCHING_PARTIAL_BOUNDARY_CONTRACT="
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
