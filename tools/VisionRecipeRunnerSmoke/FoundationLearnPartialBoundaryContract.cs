using System;
using System.Collections.Generic;
using System.IO;

internal static class FoundationLearnPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "foundation-learn-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string sourceRoot = Path.Combine(repositoryRoot, "src", "OpenVisionLab");
        string viewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "FoundationLearnView.xaml.cs");
        string xamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "FoundationLearnView.xaml");
        string presenterPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "FoundationLearnPresenter.cs");
        string learnWindowPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "OpenVisionLearnWindow.xaml.cs");
        string learnWindowXamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "OpenVisionLearnWindow.xaml");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string presenterSource = File.ReadAllText(presenterPath);
        string learnWindowSource = File.ReadAllText(learnWindowPath);
        string learnWindowXamlSource = File.ReadAllText(learnWindowXamlPath);
        List<string> results = new();

        Check(
            "Foundation Learn Partial remains the required XAML/presentation adapter",
            viewSource.Contains("public sealed partial class FoundationLearnView : UserControl", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent();", StringComparison.Ordinal)
                && viewSource.Contains("private readonly FoundationLearnPresenter presenter = new();", StringComparison.Ordinal)
                && viewSource.Contains("private readonly DispatcherTimer foundationAnimationTimer", StringComparison.Ordinal)
                && viewSource.Contains("private readonly DispatcherTimer matChannelAnimationTimer", StringComparison.Ordinal)
                && xamlSource.Contains("<UserControl x:Class=\"OpenVisionLab.FoundationLearnView\"", StringComparison.Ordinal)
                && xamlSource.Contains("Focusable=\"False\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"pixelTopicPanel\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"foundationMatGrid\"", StringComparison.Ordinal),
            results);
        Check(
            "Foundation Learn presenter owns lesson state, decisions, and fixed guidance",
            presenterSource.Contains("internal sealed class FoundationLearnPresenter", StringComparison.Ordinal)
                && presenterSource.Contains("FoundationAnimationStepCount", StringComparison.Ordinal)
                && presenterSource.Contains("MatChannelAnimationStepCount", StringComparison.Ordinal)
                && presenterSource.Contains("FoundationAnimationStatusText", StringComparison.Ordinal)
                && presenterSource.Contains("MatChannelAnimationStatusText", StringComparison.Ordinal)
                && presenterSource.Contains("GetFoundationCellRole", StringComparison.Ordinal)
                && presenterSource.Contains("UpdateToolLocation", StringComparison.Ordinal)
                && presenterSource.Contains("ResetFoundationAnimation", StringComparison.Ordinal)
                && presenterSource.Contains("AdvanceMatChannelAnimation", StringComparison.Ordinal),
            results);
        Check(
            "Foundation Learn View has no direct file, dialog, OpenCV, or tool-factory coupling",
            !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("File.", StringComparison.Ordinal)
                && !viewSource.Contains("Directory.", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenCvSharp", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionLearnFoundationSimulationModel", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionNativeTool", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionNativePropertyGridToolFactory", StringComparison.Ordinal),
            results);
        Check(
            "Foundation Learn View owns only WPF cell, brush, and animation projection",
            viewSource.Contains("BuildFoundationCells", StringComparison.Ordinal)
                && viewSource.Contains("UpdateFoundationGuide", StringComparison.Ordinal)
                && viewSource.Contains("UpdateMatChannelGuide", StringComparison.Ordinal)
                && viewSource.Contains("foundationAnimationTimer.Tick += FoundationAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("matChannelAnimationTimer.Tick += MatChannelAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("foundationAnimationTimer.Tick -= FoundationAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("matChannelAnimationTimer.Tick -= MatChannelAnimationTimer_Tick", StringComparison.Ordinal),
            results);
        Check(
            "Foundation Learn animation lifetime is stopped and unsubscribed on unload",
            viewSource.Contains("private void OnLoaded(object sender, RoutedEventArgs e)", StringComparison.Ordinal)
                && viewSource.Contains("private void OnUnloaded(object sender, RoutedEventArgs e)", StringComparison.Ordinal)
                && viewSource.Contains("StopAnimations();", StringComparison.Ordinal)
                && viewSource.Contains("btnFoundationPlay.Content = \"자동 재생\";", StringComparison.Ordinal)
                && viewSource.Contains("btnMatChannelPlay.Content = \"자동 재생\";", StringComparison.Ordinal),
            results);
        Check(
            "Foundation Learn topic and related-tool routing remain explicit composition contracts",
            viewSource.Contains("internal void SelectTopic(int topicIndex)", StringComparison.Ordinal)
                && viewSource.Contains("internal void SetOpenRelatedToolAction(Action<VISION_MENU> action)", StringComparison.Ordinal)
                && viewSource.Contains("openRelatedToolAction?.Invoke(menu);", StringComparison.Ordinal)
                && learnWindowSource.Contains("foundationLearnView.SelectTopic(topicIndex);", StringComparison.Ordinal)
                && learnWindowSource.Contains("foundationLearnView.SetOpenRelatedToolAction(action);", StringComparison.Ordinal)
                && learnWindowXamlSource.Contains("x:Name=\"foundationLearnView\"", StringComparison.Ordinal),
            results);
        Check(
            "Foundation Learn public/test facade preserves visual state access",
            viewSource.Contains("internal int FoundationAnimationStepForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal int MatChannelAnimationStepForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal bool IsFoundationRotatedRectVisibleForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal void ResetFoundationAnimationForTest()", StringComparison.Ordinal)
                && viewSource.Contains("internal void AdvanceMatChannelAnimationForTest()", StringComparison.Ordinal)
                && learnWindowSource.Contains("public int FoundationAnimationStepForTest", StringComparison.Ordinal)
                && learnWindowSource.Contains("public int MatChannelAnimationStepForTest", StringComparison.Ordinal),
            results);
        Check(
            "Foundation Learn XAML preserves automation and related-tool interaction surface",
            xamlSource.Contains("OpenVisionLearnFoundationOpenRoiToolButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnFoundationOpenKernelToolButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnFoundationOpenOutputSizeToolButton", StringComparison.Ordinal)
                && xamlSource.Contains("Click=\"OpenRelatedToolButton_Click\"", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnFoundationToolLocationTitle", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnFoundationToolLocationDetail", StringComparison.Ordinal),
            results);
        Check(
            "Foundation Learn View does not own persistence, tool creation, or algorithm lifetime",
            !viewSource.Contains("CreateBlob", StringComparison.Ordinal)
                && !viewSource.Contains("CreateFilter", StringComparison.Ordinal)
                && !viewSource.Contains("new BlobTool", StringComparison.Ordinal)
                && !viewSource.Contains("new FilterTool", StringComparison.Ordinal)
                && !viewSource.Contains("VisionToolCompositionService", StringComparison.Ordinal)
                && !viewSource.Contains("ShowDialog", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "foundation-learn-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "FOUNDATION_LEARN_PARTIAL_BOUNDARY_CONTRACT="
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
