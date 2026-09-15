using System;
using System.Collections.Generic;
using System.IO;

internal static class LearnWindowDocumentBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "learn-window-document-boundary-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string learnWindowPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Learn",
            "OpenVisionLearnWindow.xaml.cs");
        string shellControllerPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Shell",
            "Commands",
            "OpenVisionShellHostLearnWindowController.cs");
        string toolControllerPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Learn",
            "VisionToolLearnWindowController.cs");
        string thresholdControllerPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "VisionTest",
            "Wpf",
            "Learn",
            "ThresholdToolLearnWindowController.cs");

        string learnWindowSource = File.ReadAllText(learnWindowPath);
        string shellControllerSource = File.ReadAllText(shellControllerPath);
        string toolControllerSource = File.ReadAllText(toolControllerPath);
        string thresholdControllerSource = File.ReadAllText(thresholdControllerPath);
        List<string> results = new();

        Check(
            "Learn Window exposes an explicit document-action seam",
            learnWindowSource.Contains("private Action<string> openLearnDocumentAction", StringComparison.Ordinal)
                && learnWindowSource.Contains("SetOpenLearnDocumentAction(Action<string> action)", StringComparison.Ordinal)
                && learnWindowSource.Contains("openLearnDocumentAction?.Invoke(ResolveSelectedTopicDocumentFileName(topicList.SelectedIndex))", StringComparison.Ordinal)
                && learnWindowSource.Contains("openLearnDocumentAction?.Invoke(\"LEARN_OPENCVSHARP_FOUNDATIONS.md\")", StringComparison.Ordinal),
            results);
        Check(
            "Learn Window no longer owns the concrete document service call",
            !learnWindowSource.Contains("OpenVisionWorkspaceLearnDocumentService", StringComparison.Ordinal)
                && !learnWindowSource.Contains("OpenLearnDocumentFile", StringComparison.Ordinal),
            results);
        Check(
            "Shell Learn composition wires the existing document owner",
            shellControllerSource.Contains("SetOpenLearnDocumentAction(OpenVisionWorkspaceLearnDocumentService.OpenLearnDocumentFile)", StringComparison.Ordinal),
            results);
        Check(
            "Tool Learn composition wires the existing document owner",
            toolControllerSource.Contains("SetOpenLearnDocumentAction(OpenVisionWorkspaceLearnDocumentService.OpenLearnDocumentFile)", StringComparison.Ordinal)
                && thresholdControllerSource.Contains("SetOpenLearnDocumentAction(OpenVisionWorkspaceLearnDocumentService.OpenLearnDocumentFile)", StringComparison.Ordinal),
            results);
        Check(
            "Topic filename resolution remains in the existing catalog path",
            learnWindowSource.Contains("OpenVisionLearnTopicCatalog.Resolve(index).Document", StringComparison.Ordinal)
                && learnWindowSource.Contains("OpenVisionLearnTopicCatalog.Resolve(index).PracticePathId", StringComparison.Ordinal),
            results);
        Check(
            "Learn Window still preserves its existing public action and event contracts",
            learnWindowSource.Contains("SetOpenPracticeSamplesAction(Action<string> action)", StringComparison.Ordinal)
                && learnWindowSource.Contains("SetOpenRelatedToolAction(Action<VISION_MENU> action)", StringComparison.Ordinal)
                && learnWindowSource.Contains("ApplyThresholdRequested", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "learn-window-document-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "LEARN_WINDOW_DOCUMENT_BOUNDARY_CONTRACT="
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
