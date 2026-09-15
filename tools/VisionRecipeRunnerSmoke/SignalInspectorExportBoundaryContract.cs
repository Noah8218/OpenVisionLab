using System;
using System.Collections.Generic;
using System.IO;

internal static class SignalInspectorExportBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "signal-inspector-export-boundary-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string inspectorPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Tooling",
            "SignalInspection",
            "VisionToolSignalInspectorView.xaml.cs");
        string exporterPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Tooling",
            "SignalInspection",
            "VisionToolSignalEvidenceExporter.cs");
        string[] compositionPaths =
        {
            Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "ToolViews", "ThresholdToolWpfView.xaml.cs"),
            Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "ToolViews", "SimplePreprocessToolWpfView.xaml.cs"),
            Path.Combine(repositoryRoot, "src", "OpenVisionLab", "UI", "VisionTest", "Wpf", "ToolViews", "LineToolWpfView.xaml.cs")
        };

        string inspectorSource = File.ReadAllText(inspectorPath);
        string exporterSource = File.ReadAllText(exporterPath);
        string[] compositionSources = Array.ConvertAll(compositionPaths, File.ReadAllText);
        List<string> results = new();

        Check(
            "Inspector exposes an explicit exporter action seam",
            inspectorSource.Contains("private Action<VisionToolSignalEvidence, string> exportAction", StringComparison.Ordinal)
                && inspectorSource.Contains("SetExportAction(Action<VisionToolSignalEvidence, string> action)", StringComparison.Ordinal)
                && inspectorSource.Contains("exportAction(evidence, path);", StringComparison.Ordinal),
            results);
        Check(
            "Inspector keeps the SaveFileDialog presentation boundary",
            inspectorSource.Contains("SaveFileDialog dialog = new SaveFileDialog", StringComparison.Ordinal)
                && inspectorSource.Contains("ExportEvidence(dialog.FileName);", StringComparison.Ordinal),
            results);
        Check(
            "Inspector no longer owns file-export coupling",
            !inspectorSource.Contains("VisionToolSignalEvidenceExporter", StringComparison.Ordinal)
                && !inspectorSource.Contains("using System.IO;", StringComparison.Ordinal),
            results);
        Check(
            "Test export follows the same configured action path",
            inspectorSource.Contains("internal void ExportForTest(string path)", StringComparison.Ordinal)
                && inspectorSource.Contains("ExportEvidence(path);", StringComparison.Ordinal),
            results);
        Check(
            "Existing exporter remains the file-I/O owner",
            exporterSource.Contains("internal static class VisionToolSignalEvidenceExporter", StringComparison.Ordinal)
                && exporterSource.Contains("public static void ExportTsv(VisionToolSignalEvidence evidence, string path)", StringComparison.Ordinal)
                && exporterSource.Contains("Directory.CreateDirectory(directory);", StringComparison.Ordinal),
            results);
        Check(
            "All known Tool composition owners wire the existing exporter",
            Array.TrueForAll(
                compositionSources,
                source => source.Contains(
                    "signalInspector.SetExportAction(VisionToolSignalEvidenceExporter.ExportTsv);",
                    StringComparison.Ordinal)),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "signal-inspector-export-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "SIGNAL_INSPECTOR_EXPORT_BOUNDARY_CONTRACT="
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
