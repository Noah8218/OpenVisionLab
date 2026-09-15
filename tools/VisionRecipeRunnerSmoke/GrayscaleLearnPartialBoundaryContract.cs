using System;
using System.Collections.Generic;
using System.IO;

internal static class GrayscaleLearnPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "grayscale-learn-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string sourceRoot = Path.Combine(repositoryRoot, "src", "OpenVisionLab");
        string viewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "GrayscaleLearnView.xaml.cs");
        string xamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "GrayscaleLearnView.xaml");
        string presenterPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "GrayscaleLearnPresenter.cs");
        string learnWindowPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "OpenVisionLearnWindow.xaml.cs");
        string learnWindowXamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "OpenVisionLearnWindow.xaml");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string presenterSource = File.ReadAllText(presenterPath);
        string learnWindowSource = File.ReadAllText(learnWindowPath);
        string learnWindowXamlSource = File.ReadAllText(learnWindowXamlPath);
        List<string> results = new();

        Check(
            "Grayscale Learn Partial remains the required XAML/presentation adapter",
            viewSource.Contains("public sealed partial class GrayscaleLearnView : UserControl", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent();", StringComparison.Ordinal)
                && viewSource.Contains("private readonly GrayscaleLearnPresenter presenter = new();", StringComparison.Ordinal)
                && viewSource.Contains("private readonly DispatcherTimer animationTimer", StringComparison.Ordinal)
                && viewSource.Contains("private readonly DispatcherTimer arithmeticAnimationTimer", StringComparison.Ordinal)
                && viewSource.Contains("private readonly DispatcherTimer brightnessAnimationTimer", StringComparison.Ordinal)
                && viewSource.Contains("private readonly DispatcherTimer filterAnimationTimer", StringComparison.Ordinal)
                && viewSource.Contains("internal event EventHandler<OpenVisionLearnThresholdApplyEventArgs> ApplyThresholdRequested", StringComparison.Ordinal)
                && xamlSource.Contains("<UserControl x:Class=\"OpenVisionLab.GrayscaleLearnView\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"brightnessTopicPanel\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"arithmeticTopicPanel\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"filterTopicPanel\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"thresholdTabs\"", StringComparison.Ordinal),
            results);
        Check(
            "Grayscale Learn presenter owns lesson state, simulation decisions, and fixed text",
            presenterSource.Contains("internal sealed class GrayscaleLearnPresenter", StringComparison.Ordinal)
                && presenterSource.Contains("OpenVisionLearnBasicGrayscaleSimulationModel.EvaluateThreshold", StringComparison.Ordinal)
                && presenterSource.Contains("OpenVisionLearnBasicGrayscaleSimulationModel.EvaluateBrightness", StringComparison.Ordinal)
                && presenterSource.Contains("OpenVisionLearnBasicGrayscaleSimulationModel.EvaluateArithmetic", StringComparison.Ordinal)
                && presenterSource.Contains("OpenVisionLearnBasicGrayscaleSimulationModel.EvaluateFilter", StringComparison.Ordinal)
                && presenterSource.Contains("internal void UpdateThreshold", StringComparison.Ordinal)
                && presenterSource.Contains("internal void UpdateBrightness", StringComparison.Ordinal)
                && presenterSource.Contains("internal void UpdateArithmetic", StringComparison.Ordinal)
                && presenterSource.Contains("internal void UpdateFilter", StringComparison.Ordinal)
                && presenterSource.Contains("GetThresholdOutputCell", StringComparison.Ordinal)
                && presenterSource.Contains("FilteringOpenedTitle", StringComparison.Ordinal),
            results);
        Check(
            "Grayscale Learn View has no direct file, dialog, OpenCV, or tool-factory coupling",
            !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("File.", StringComparison.Ordinal)
                && !viewSource.Contains("Directory.", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenCvSharp", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionLearnBasicGrayscaleSimulationModel", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionNativeTool", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionNativePropertyGridToolFactory", StringComparison.Ordinal),
            results);
        Check(
            "Grayscale Learn View owns only WPF cells, markers, text, and timer projection",
            viewSource.Contains("BuildSampleCells", StringComparison.Ordinal)
                && viewSource.Contains("BuildBrightnessCells", StringComparison.Ordinal)
                && viewSource.Contains("BuildArithmeticCells", StringComparison.Ordinal)
                && viewSource.Contains("BuildFilterCells", StringComparison.Ordinal)
                && viewSource.Contains("UpdateGuide()", StringComparison.Ordinal)
                && viewSource.Contains("UpdateBrightnessGuide()", StringComparison.Ordinal)
                && viewSource.Contains("UpdateArithmeticGuide()", StringComparison.Ordinal)
                && viewSource.Contains("UpdateFilterGuide()", StringComparison.Ordinal)
                && viewSource.Contains("animationTimer.Tick += AnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("arithmeticAnimationTimer.Tick += ArithmeticAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("brightnessAnimationTimer.Tick += BrightnessAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("filterAnimationTimer.Tick += FilterAnimationTimer_Tick", StringComparison.Ordinal),
            results);
        Check(
            "Grayscale Learn animation lifetime is stopped and unsubscribed on unload",
            viewSource.Contains("private void OnLoaded(object sender, RoutedEventArgs e)", StringComparison.Ordinal)
                && viewSource.Contains("private void OnUnloaded(object sender, RoutedEventArgs e)", StringComparison.Ordinal)
                && viewSource.Contains("StopAnimations();", StringComparison.Ordinal)
                && viewSource.Contains("animationTimer.Tick -= AnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("arithmeticAnimationTimer.Tick -= ArithmeticAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("brightnessAnimationTimer.Tick -= BrightnessAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("filterAnimationTimer.Tick -= FilterAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("btnAnimate.Content = \"Play\";", StringComparison.Ordinal),
            results);
        Check(
            "Grayscale Learn topic and related-tool routing remain explicit composition contracts",
            viewSource.Contains("internal void SelectTopic(int topicIndex)", StringComparison.Ordinal)
                && viewSource.Contains("internal void SetOpenRelatedToolAction(Action<VISION_MENU> action)", StringComparison.Ordinal)
                && viewSource.Contains("openRelatedToolAction?.Invoke(menu);", StringComparison.Ordinal)
                && learnWindowSource.Contains("grayscaleLearnView.SelectTopic(topicIndex);", StringComparison.Ordinal)
                && learnWindowSource.Contains("grayscaleLearnView.SetOpenRelatedToolAction(action);", StringComparison.Ordinal)
                && learnWindowXamlSource.Contains("x:Name=\"grayscaleLearnView\"", StringComparison.Ordinal),
            results);
        Check(
            "Threshold Apply and Close remain explicit result events",
            viewSource.Contains("internal event EventHandler<OpenVisionLearnThresholdApplyEventArgs> ApplyThresholdRequested", StringComparison.Ordinal)
                && viewSource.Contains("internal event EventHandler CloseRequested", StringComparison.Ordinal)
                && viewSource.Contains("internal event EventHandler ThresholdToolOpened", StringComparison.Ordinal)
                && viewSource.Contains("ApplyThresholdRequested?.Invoke(this, new OpenVisionLearnThresholdApplyEventArgs", StringComparison.Ordinal)
                && viewSource.Contains("CloseRequested?.Invoke(this, EventArgs.Empty);", StringComparison.Ordinal)
                && viewSource.Contains("ThresholdToolOpened?.Invoke(this, EventArgs.Empty);", StringComparison.Ordinal)
                && learnWindowSource.Contains("grayscaleLearnView.ApplyThresholdRequested += OnGrayscaleThresholdApplied;", StringComparison.Ordinal)
                && learnWindowSource.Contains("grayscaleLearnView.CloseRequested += OnGrayscaleCloseRequested;", StringComparison.Ordinal)
                && learnWindowSource.Contains("grayscaleLearnView.ThresholdToolOpened += OnGrayscaleThresholdToolOpened;", StringComparison.Ordinal),
            results);
        Check(
            "Grayscale Learn test facade preserves input, stage, text, and reset access",
            viewSource.Contains("internal double ThresholdValueForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal bool IsInvertedForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal double BrightnessOffsetForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal int BrightnessAnimationStepForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal int ArithmeticAnimationStepForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal int FilterAnimationStepForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal void ResetBrightnessAnimationForTest()", StringComparison.Ordinal)
                && viewSource.Contains("internal void ResetArithmeticAnimationForTest()", StringComparison.Ordinal)
                && viewSource.Contains("internal void ResetFilterAnimationForTest()", StringComparison.Ordinal)
                && learnWindowSource.Contains("public void ApplyForTest() => grayscaleLearnView.ApplyForTest();", StringComparison.Ordinal),
            results);
        Check(
            "Grayscale Learn XAML preserves automation and interaction surface",
            xamlSource.Contains("OpenVisionLearnBrightnessOpenMeanToolButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnBrightnessOpenHistogramToolButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnArithmeticOpenToolButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnFilteringOpenToolButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnThresholdOpenToolButton", StringComparison.Ordinal)
                && xamlSource.Contains("Click=\"OpenRelatedToolButton_Click\"", StringComparison.Ordinal)
                && xamlSource.Contains("ValueChanged=\"ThresholdSlider_ValueChanged\"", StringComparison.Ordinal)
                && xamlSource.Contains("SelectionChanged=\"ArithmeticModeCombo_SelectionChanged\"", StringComparison.Ordinal)
                && xamlSource.Contains("SelectionChanged=\"FilterModeCombo_SelectionChanged\"", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnThresholdApplyButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnCloseButton", StringComparison.Ordinal),
            results);
        Check(
            "Grayscale Learn mutable lesson state flows through the existing presenter",
            viewSource.Contains("presenter.UpdateThreshold(thresholdSlider.Value, chkInvert.IsChecked == true);", StringComparison.Ordinal)
                && viewSource.Contains("presenter.UpdateBrightness(brightnessOffsetSlider.Value);", StringComparison.Ordinal)
                && viewSource.Contains("presenter.UpdateArithmetic(GetSelectedArithmeticMode());", StringComparison.Ordinal)
                && viewSource.Contains("presenter.UpdateFilter(GetSelectedFilterMode());", StringComparison.Ordinal)
                && viewSource.Contains("presenter.NextThresholdAnimationValue(thresholdSlider.Value)", StringComparison.Ordinal)
                && !viewSource.Contains("new OpenVisionLearnBasicGrayscaleSimulationModel", StringComparison.Ordinal),
            results);
        Check(
            "Grayscale Learn View does not own persistence, Tool creation, or algorithm lifetime",
            !viewSource.Contains("CreateThreshold", StringComparison.Ordinal)
                && !viewSource.Contains("CreateFilter", StringComparison.Ordinal)
                && !viewSource.Contains("CreateArithmetic", StringComparison.Ordinal)
                && !viewSource.Contains("new ThresholdTool", StringComparison.Ordinal)
                && !viewSource.Contains("new FilterTool", StringComparison.Ordinal)
                && !viewSource.Contains("new ArithmeticTool", StringComparison.Ordinal)
                && !viewSource.Contains("VisionToolCompositionService", StringComparison.Ordinal)
                && !viewSource.Contains("ShowDialog", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "grayscale-learn-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "GRAYSCALE_LEARN_PARTIAL_BOUNDARY_CONTRACT="
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
