using System;
using System.Collections.Generic;
using System.IO;

internal static class MetricsAcceptanceLearnPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "metrics-acceptance-learn-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string sourceRoot = Path.Combine(repositoryRoot, "src", "OpenVisionLab");
        string viewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "MetricsAcceptanceLearnView.xaml.cs");
        string xamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "MetricsAcceptanceLearnView.xaml");
        string presenterPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "MetricsAcceptanceLearnPresenter.cs");
        string learnWindowPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "OpenVisionLearnWindow.xaml.cs");
        string learnWindowXamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "OpenVisionLearnWindow.xaml");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string presenterSource = File.ReadAllText(presenterPath);
        string learnWindowSource = File.ReadAllText(learnWindowPath);
        string learnWindowXamlSource = File.ReadAllText(learnWindowXamlPath);
        List<string> results = new();

        Check(
            "Metrics Acceptance Partial remains the required XAML/presentation adapter",
            viewSource.Contains("public sealed partial class MetricsAcceptanceLearnView : UserControl", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent();", StringComparison.Ordinal)
                && viewSource.Contains("private readonly MetricsAcceptanceLearnPresenter presenter = new();", StringComparison.Ordinal)
                && viewSource.Contains("private readonly DispatcherTimer animationTimer", StringComparison.Ordinal)
                && viewSource.Contains("RefreshFrame();", StringComparison.Ordinal)
                && xamlSource.Contains("<UserControl x:Class=\"OpenVisionLab.MetricsAcceptanceLearnView\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"metricsAcceptanceTopicPanel\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"metricsAcceptanceSampleGrid\"", StringComparison.Ordinal),
            results);
        Check(
            "Metrics Acceptance presenter owns samples, statistics, gates, stages, and explanations",
            presenterSource.Contains("internal sealed class MetricsAcceptanceLearnPresenter", StringComparison.Ordinal)
                && presenterSource.Contains("internal IReadOnlyList<double> Samples", StringComparison.Ordinal)
                && presenterSource.Contains("internal double Average", StringComparison.Ordinal)
                && presenterSource.Contains("internal double Range", StringComparison.Ordinal)
                && presenterSource.Contains("internal double Maximum", StringComparison.Ordinal)
                && presenterSource.Contains("internal int AnimationStep", StringComparison.Ordinal)
                && presenterSource.Contains("internal string FormulaText", StringComparison.Ordinal)
                && presenterSource.Contains("internal string AnimationStatusText", StringComparison.Ordinal)
                && presenterSource.Contains("internal bool IsOutlier", StringComparison.Ordinal),
            results);
        Check(
            "Metrics Acceptance presenter stays WPF-free and owns no external lifetime",
            !presenterSource.Contains("System.Windows", StringComparison.Ordinal)
                && !presenterSource.Contains("DispatcherTimer", StringComparison.Ordinal)
                && !presenterSource.Contains("UserControl", StringComparison.Ordinal)
                && !presenterSource.Contains("OpenVisionRecipe", StringComparison.Ordinal)
                && !presenterSource.Contains("OpenCvSharp", StringComparison.Ordinal),
            results);
        Check(
            "Metrics Acceptance View has no direct file, dialog, OpenCV, or tool-factory coupling",
            !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("File.", StringComparison.Ordinal)
                && !viewSource.Contains("Directory.", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenCvSharp", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionRecipe", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionNativePropertyGridToolFactory", StringComparison.Ordinal),
            results);
        Check(
            "Metrics Acceptance View owns only WPF cell, brush, text, button, and timer projection",
            viewSource.Contains("CreateSmallValueCell", StringComparison.Ordinal)
                && viewSource.Contains("sampleCells[index].Background", StringComparison.Ordinal)
                && viewSource.Contains("sampleTexts[index].Text", StringComparison.Ordinal)
                && viewSource.Contains("txtMetricsAcceptanceFormula.Text = presenter.FormulaText", StringComparison.Ordinal)
                && viewSource.Contains("txtMetricsAcceptanceAnimationStatus.Text = presenter.AnimationStatusText", StringComparison.Ordinal)
                && viewSource.Contains("animationTimer.Tick += AnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("animationTimer.Tick -= AnimationTimer_Tick", StringComparison.Ordinal),
            results);
        Check(
            "Metrics Acceptance animation lifetime stops and unsubscribes on unload",
            viewSource.Contains("private void OnLoaded(object sender, RoutedEventArgs e)", StringComparison.Ordinal)
                && viewSource.Contains("private void OnUnloaded(object sender, RoutedEventArgs e)", StringComparison.Ordinal)
                && viewSource.Contains("StopAnimation();", StringComparison.Ordinal)
                && viewSource.Contains("animationTimer.Stop();", StringComparison.Ordinal)
                && viewSource.Contains("btnMetricsAcceptancePlay.Content = \"Play\";", StringComparison.Ordinal)
                && viewSource.Contains("animationTimer.Tick -= AnimationTimer_Tick", StringComparison.Ordinal),
            results);
        Check(
            "Metrics Acceptance stage state flows through the presenter",
            viewSource.Contains("presenter.ResetAnimation();", StringComparison.Ordinal)
                && viewSource.Contains("presenter.AdvanceAnimation();", StringComparison.Ordinal)
                && viewSource.Contains("presenter.IsAnimationComplete", StringComparison.Ordinal)
                && viewSource.Contains("presenter.ShowsSampleDecision", StringComparison.Ordinal)
                && viewSource.Contains("presenter.IsOutlier(index)", StringComparison.Ordinal)
                && !viewSource.Contains("AverageMin", StringComparison.Ordinal)
                && !viewSource.Contains("ValueMax", StringComparison.Ordinal),
            results);
        Check(
            "Metrics Acceptance XAML preserves the sample, gate, and animation interaction surface",
            xamlSource.Contains("OpenVisionLearnMetricGateCheatSheetPanel", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnMetricsAcceptancePlayButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnMetricsAcceptanceStepButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnMetricsAcceptanceResetButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnMetricsAcceptanceAnimationStatus", StringComparison.Ordinal)
                && xamlSource.Contains("Click=\"MetricsAcceptancePlayButton_Click\"", StringComparison.Ordinal)
                && xamlSource.Contains("Click=\"MetricsAcceptanceStepButton_Click\"", StringComparison.Ordinal)
                && xamlSource.Contains("Click=\"MetricsAcceptanceResetButton_Click\"", StringComparison.Ordinal),
            results);
        Check(
            "Metrics Acceptance test facade preserves stage, formula, status, and cheat-sheet access",
            viewSource.Contains("internal int AnimationStepForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal string FormulaTextForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal string AnimationStatusTextForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal bool IsCheatSheetExpandedForTest", StringComparison.Ordinal)
                && learnWindowSource.Contains("public int MetricsAcceptanceAnimationStepForTest", StringComparison.Ordinal)
                && learnWindowSource.Contains("public string MetricsAcceptanceFormulaTextForTest", StringComparison.Ordinal)
                && learnWindowSource.Contains("public void ResetMetricsAcceptanceAnimationForTest()", StringComparison.Ordinal)
                && learnWindowSource.Contains("public void ToggleMetricsAcceptanceAnimationForTest()", StringComparison.Ordinal),
            results);
        Check(
            "Learn Window owns Metrics Acceptance visibility, refresh, composition, and close lifetime",
            learnWindowXamlSource.Contains("x:Name=\"metricsAcceptanceLearnView\"", StringComparison.Ordinal)
                && learnWindowSource.Contains("metricsAcceptanceLearnView.RefreshFrame();", StringComparison.Ordinal)
                && learnWindowSource.Contains("metricsAcceptanceLearnView.Visibility = presentation.ShowMetricsAcceptanceTopic", StringComparison.Ordinal)
                && learnWindowSource.Contains("metricsAcceptanceLearnView.StopAnimation();", StringComparison.Ordinal)
                && learnWindowSource.Contains("metricsAcceptanceLearnView.AnimationStepForTest", StringComparison.Ordinal),
            results);
        Check(
            "Metrics Acceptance View does not own persistence, Tool creation, or algorithm lifetime",
            !viewSource.Contains("Save", StringComparison.Ordinal)
                && !viewSource.Contains("CreateThreshold", StringComparison.Ordinal)
                && !viewSource.Contains("new ThresholdTool", StringComparison.Ordinal)
                && !viewSource.Contains("ShowDialog", StringComparison.Ordinal)
                && !viewSource.Contains("VisionToolCompositionService", StringComparison.Ordinal),
            results);
        Check(
            "Metrics Acceptance has an existing focused presenter and WPF test path",
            learnWindowSource.Contains("metricsAcceptanceLearnView.RefreshFrame();", StringComparison.Ordinal)
                && File.Exists(Path.Combine(repositoryRoot, "tools", "VisionRecipeRunnerSmoke", "LearnMetricsAcceptanceContract.cs"))
                && File.Exists(Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "LearnMetricsAcceptanceSmoke.cs")),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "metrics-acceptance-learn-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "METRICS_ACCEPTANCE_LEARN_PARTIAL_BOUNDARY_CONTRACT="
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
