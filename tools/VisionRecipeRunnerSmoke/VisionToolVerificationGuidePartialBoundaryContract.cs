using System;
using System.Collections.Generic;
using System.IO;

internal static class VisionToolVerificationGuidePartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "verification-guide-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string sourceRoot = Path.Combine(repositoryRoot, "src", "OpenVisionLab");
        string viewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "Review", "VisionToolVerificationGuideView.xaml.cs");
        string xamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "Review", "VisionToolVerificationGuideView.xaml");
        string areaPresenterPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "Review", "VisionToolAreaVerificationGuidePresenter.cs");
        string matchingPresenterPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "Review", "VisionToolMatchingVerificationGuidePresenter.cs");
        string shellPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputPropertyToolShell.xaml.cs");
        string shellXamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputPropertyToolShell.xaml");
        string blobViewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "ToolViews", "BlobToolWpfView.xaml.cs");
        string contourViewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "ToolViews", "ContourToolWpfView.xaml.cs");
        string edgeViewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "ToolViews", "EdgeBasedMatchingToolWpfView.xaml.cs");
        string matchingRuntimePath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Tooling", "SingleInput", "VisionToolSingleInputMatchingToolRuntime.cs");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string areaPresenterSource = File.ReadAllText(areaPresenterPath);
        string matchingPresenterSource = File.ReadAllText(matchingPresenterPath);
        string shellSource = File.ReadAllText(shellPath);
        string shellXamlSource = File.ReadAllText(shellXamlPath);
        string blobViewSource = File.ReadAllText(blobViewPath);
        string contourViewSource = File.ReadAllText(contourViewPath);
        string edgeViewSource = File.ReadAllText(edgeViewPath);
        string matchingRuntimeSource = File.ReadAllText(matchingRuntimePath);
        List<string> results = new();

        Check(
            "Verification guide Partial remains the required XAML visual adapter",
            viewSource.Contains("public partial class VisionToolVerificationGuideView : UserControl", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent();", StringComparison.Ordinal)
                && viewSource.Contains("public VisionToolVerificationGuideView()", StringComparison.Ordinal)
                && xamlSource.Contains("<UserControl x:Class=\"OpenVisionLab.VisionToolVerificationGuideView\"", StringComparison.Ordinal)
                && xamlSource.Contains("AutomationProperties.AutomationId=\"VisionToolVerificationGuideView\"", StringComparison.Ordinal),
            results);
        Check(
            "Verification guide exposes the existing dependency-property contract",
            viewSource.Contains("HeaderTextProperty", StringComparison.Ordinal)
                && viewSource.Contains("StateTextProperty", StringComparison.Ordinal)
                && viewSource.Contains("CriteriaTextProperty", StringComparison.Ordinal)
                && viewSource.Contains("NextActionTextProperty", StringComparison.Ordinal)
                && viewSource.Contains("StateBrushProperty", StringComparison.Ordinal)
                && viewSource.Contains("IsCompactModeProperty", StringComparison.Ordinal)
                && viewSource.Contains("DependencyProperty.Register", StringComparison.Ordinal),
            results);
        Check(
            "Verification guide XAML binds all state projections and preserves automation IDs",
            xamlSource.Contains("Text=\"{Binding HeaderText, ElementName=root}\"", StringComparison.Ordinal)
                && xamlSource.Contains("Text=\"{Binding StateText, ElementName=root}\"", StringComparison.Ordinal)
                && xamlSource.Contains("Text=\"{Binding CriteriaText, ElementName=root}\"", StringComparison.Ordinal)
                && xamlSource.Contains("Text=\"{Binding NextActionText, ElementName=root}\"", StringComparison.Ordinal)
                && xamlSource.Contains("Foreground=\"{Binding StateBrush, ElementName=root", StringComparison.Ordinal)
                && xamlSource.Contains("VisionToolVerificationGuideState", StringComparison.Ordinal)
                && xamlSource.Contains("VisionToolVerificationGuideCriteria", StringComparison.Ordinal)
                && xamlSource.Contains("VisionToolVerificationGuideNextAction", StringComparison.Ordinal),
            results);
        Check(
            "Verification guide Partial has no file, dialog, OpenCV, Recipe, or algorithm coupling",
            !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("File.", StringComparison.Ordinal)
                && !viewSource.Contains("Directory.", StringComparison.Ordinal)
                && !viewSource.Contains("ShowDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenCvSharp", StringComparison.Ordinal)
                && !viewSource.Contains("VisionToolProperty", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionNativeTool", StringComparison.Ordinal),
            results);
        Check(
            "Verification guide owns only compact visual density projection",
            viewSource.Contains("OnIsCompactModeChanged", StringComparison.Ordinal)
                && viewSource.Contains("ApplyDensity();", StringComparison.Ordinal)
                && viewSource.Contains("headerGrid.Visibility", StringComparison.Ordinal)
                && viewSource.Contains("guideChrome.Padding", StringComparison.Ordinal)
                && viewSource.Contains("rowNextAction.Height", StringComparison.Ordinal)
                && !viewSource.Contains("DispatcherTimer", StringComparison.Ordinal)
                && !viewSource.Contains("async ", StringComparison.Ordinal),
            results);
        Check(
            "Area verification presenter remains the criteria/result/state policy owner",
            areaPresenterSource.Contains("VisionToolVerificationGuideView guideView", StringComparison.Ordinal)
                && areaPresenterSource.Contains("ShowTeachingState", StringComparison.Ordinal)
                && areaPresenterSource.Contains("ShowResult", StringComparison.Ordinal)
                && areaPresenterSource.Contains("CreateCriteriaText", StringComparison.Ordinal)
                && areaPresenterSource.Contains("guideView.StateText", StringComparison.Ordinal)
                && areaPresenterSource.Contains("guideView.CriteriaText", StringComparison.Ordinal)
                && areaPresenterSource.Contains("guideView.StateBrush", StringComparison.Ordinal),
            results);
        Check(
            "Matching verification presenter remains the matching policy owner",
            matchingPresenterSource.Contains("VisionToolVerificationGuideView guideView", StringComparison.Ordinal)
                && matchingPresenterSource.Contains("ShowTeachingState", StringComparison.Ordinal)
                && matchingPresenterSource.Contains("ShowResult", StringComparison.Ordinal)
                && matchingPresenterSource.Contains("CreateCriteriaText", StringComparison.Ordinal)
                && matchingPresenterSource.Contains("guideView.StateText", StringComparison.Ordinal)
                && matchingPresenterSource.Contains("guideView.CriteriaText", StringComparison.Ordinal)
                && matchingPresenterSource.Contains("guideView.StateBrush", StringComparison.Ordinal),
            results);
        Check(
            "Common single-input shell owns guide content composition and compact-density routing",
            shellSource.Contains("ToolContentProperty", StringComparison.Ordinal)
                && shellSource.Contains("ToolContentVisibilityProperty", StringComparison.Ordinal)
                && shellSource.Contains("shell.ToolContent is VisionToolVerificationGuideView guideView", StringComparison.Ordinal)
                && shellSource.Contains("guideView.IsCompactMode = true", StringComparison.Ordinal)
                && shellXamlSource.Contains("Content=\"{Binding ToolContent, RelativeSource={RelativeSource AncestorType=local:VisionToolSingleInputPropertyToolShell}}\"", StringComparison.Ordinal)
                && shellXamlSource.Contains("Visibility=\"{Binding ToolContentVisibility, RelativeSource={RelativeSource AncestorType=local:VisionToolSingleInputPropertyToolShell}}\"", StringComparison.Ordinal),
            results);
        Check(
            "Verification guide is reused by area and matching tool composition",
            blobViewSource.Contains("VisionToolVerificationGuideView", StringComparison.Ordinal)
                && contourViewSource.Contains("VisionToolVerificationGuideView", StringComparison.Ordinal)
                && edgeViewSource.Contains("VisionToolVerificationGuideView", StringComparison.Ordinal)
                && matchingRuntimeSource.Contains("CreateVerificationGuideView", StringComparison.Ordinal)
                && matchingRuntimeSource.Contains("shell.ToolContent = verificationGuideView", StringComparison.Ordinal),
            results);
        Check(
            "Verification guide has no independent mutable business state or lifetime owner",
            !viewSource.Contains("private ", StringComparison.Ordinal)
                || (viewSource.Contains("private static void OnIsCompactModeChanged", StringComparison.Ordinal)
                    && !viewSource.Contains("private readonly", StringComparison.Ordinal)
                    && !viewSource.Contains("private bool", StringComparison.Ordinal)),
            results);
        Check(
            "Verification guide preserves visual state labels, trimming, and tooltips",
            xamlSource.Contains("ToolTip=\"{Binding StateText, ElementName=root}\"", StringComparison.Ordinal)
                && xamlSource.Contains("ToolTip=\"{Binding CriteriaText, ElementName=root}\"", StringComparison.Ordinal)
                && xamlSource.Contains("ToolTip=\"{Binding NextActionText, ElementName=root}\"", StringComparison.Ordinal)
                && xamlSource.Contains("TextTrimming=\"CharacterEllipsis\"", StringComparison.Ordinal)
                && xamlSource.Contains("VisionTool.AccentBrush", StringComparison.Ordinal),
            results);
        Check(
            "Verification guide is not a ViewModel or inspection execution owner",
            !viewSource.Contains("ViewModel", StringComparison.Ordinal)
                && !viewSource.Contains("RunPreview", StringComparison.Ordinal)
                && !viewSource.Contains("Persist", StringComparison.Ordinal)
                && !viewSource.Contains("CreateProperty", StringComparison.Ordinal)
                && !viewSource.Contains("Dispose", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "verification-guide-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "VISION_TOOL_VERIFICATION_GUIDE_PARTIAL_BOUNDARY_CONTRACT="
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
