using System;
using System.Collections.Generic;
using System.IO;

internal static class LineToolPersistenceBoundaryContract
{
    internal static int Run(string requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string viewPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "ToolViews",
            "LineToolWpfView.xaml.cs");
        string presenterPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Tooling",
            "Presentation",
            "LineToolPresenter.cs");
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

        string viewSource = File.ReadAllText(viewPath);
        string presenterSource = File.ReadAllText(presenterPath);
        string factorySource = File.ReadAllText(factoryPath);
        List<string> results = new();

        Check(
            "Line View no longer owns native property storage",
            !viewSource.Contains("OpenVisionNativeToolPropertySessionStore", StringComparison.Ordinal)
                && !viewSource.Contains("PersistLineProperties", StringComparison.Ordinal)
                && viewSource.Contains("presenter.PersistProperties", StringComparison.Ordinal),
            results);
        Check(
            "Line presenter owns the injected persistence boundary",
            presenterSource.Contains("private readonly Action persistProperties", StringComparison.Ordinal)
                && presenterSource.Contains("public void PersistProperties()", StringComparison.Ordinal)
                && presenterSource.Contains("persistProperties();", StringComparison.Ordinal),
            results);
        Check(
            "Line composition keeps the existing recipe storage contract",
            factorySource.Contains("new LineToolPresenter(", StringComparison.Ordinal)
                && factorySource.Contains("OpenVisionNativeToolPropertySessionStore.Save(\"Line(L)_1\"", StringComparison.Ordinal)
                && factorySource.Contains("OpenVisionNativeToolPropertySessionStore.Save(\"Line(R)_1\"", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "line-tool-persistence-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "LINE_TOOL_PERSISTENCE_BOUNDARY_CONTRACT="
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
