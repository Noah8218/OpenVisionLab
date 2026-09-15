using System;
using System.Collections.Generic;
using System.IO;

internal static class BinaryLearnPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "binary-learn-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string sourceRoot = Path.Combine(repositoryRoot, "src", "OpenVisionLab");
        string viewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "BinaryLearnView.xaml.cs");
        string xamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "BinaryLearnView.xaml");
        string presenterPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "BinaryLearnPresenter.cs");
        string learnWindowPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "OpenVisionLearnWindow.xaml.cs");
        string learnWindowXamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "OpenVisionLearnWindow.xaml");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string presenterSource = File.ReadAllText(presenterPath);
        string learnWindowSource = File.ReadAllText(learnWindowPath);
        string learnWindowXamlSource = File.ReadAllText(learnWindowXamlPath);
        List<string> results = new();

        Check(
            "Binary Learn Partial remains the required XAML/presentation adapter",
            viewSource.Contains("public sealed partial class BinaryLearnView : UserControl", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent();", StringComparison.Ordinal)
                && viewSource.Contains("private readonly BinaryLearnPresenter presenter = new();", StringComparison.Ordinal)
                && viewSource.Contains("private readonly DispatcherTimer morphologyAnimationTimer", StringComparison.Ordinal)
                && viewSource.Contains("private readonly DispatcherTimer blobAnimationTimer", StringComparison.Ordinal)
                && viewSource.Contains("private readonly DispatcherTimer contourAnimationTimer", StringComparison.Ordinal)
                && xamlSource.Contains("<UserControl x:Class=\"OpenVisionLab.BinaryLearnView\"", StringComparison.Ordinal)
                && xamlSource.Contains("Focusable=\"False\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"morphologyTopicPanel\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"blobTopicPanel\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"contourTopicPanel\"", StringComparison.Ordinal),
            results);
        Check(
            "Binary Learn presenter owns lesson state, simulation decisions, and fixed text",
            presenterSource.Contains("internal sealed class BinaryLearnPresenter", StringComparison.Ordinal)
                && presenterSource.Contains("OpenVisionLearnBinarySimulationModel.CalculateMorphology", StringComparison.Ordinal)
                && presenterSource.Contains("OpenVisionLearnBinarySimulationModel.LabelConnectedBlobs", StringComparison.Ordinal)
                && presenterSource.Contains("OpenVisionLearnBinarySimulationModel.FindContourPixels", StringComparison.Ordinal)
                && presenterSource.Contains("OpenVisionLearnBinarySimulationModel.FindBounds", StringComparison.Ordinal)
                && presenterSource.Contains("internal void AdvanceMorphologyAnimation()", StringComparison.Ordinal)
                && presenterSource.Contains("internal void AdvanceBlobAnimation()", StringComparison.Ordinal)
                && presenterSource.Contains("internal void AdvanceContourAnimation()", StringComparison.Ordinal)
                && presenterSource.Contains("MorphologyOpenedTitle", StringComparison.Ordinal)
                && presenterSource.Contains("ContourOpenedDetail", StringComparison.Ordinal),
            results);
        Check(
            "Binary Learn View has no direct file, dialog, OpenCV, or simulation coupling",
            !viewSource.Contains("System.IO", StringComparison.Ordinal)
                && !viewSource.Contains("File.", StringComparison.Ordinal)
                && !viewSource.Contains("Directory.", StringComparison.Ordinal)
                && !viewSource.Contains("SaveFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenFileDialog", StringComparison.Ordinal)
                && !viewSource.Contains("OpenCvSharp", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionLearnBinarySimulationModel", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionNativeTool", StringComparison.Ordinal),
            results);
        Check(
            "Binary Learn View owns only WPF animation and visual projection",
            viewSource.Contains("DispatcherTimer", StringComparison.Ordinal)
                && viewSource.Contains("morphologyAnimationTimer.Tick += MorphologyAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("blobAnimationTimer.Tick += BlobAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("contourAnimationTimer.Tick += ContourAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("morphologyAnimationTimer.Tick -= MorphologyAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("blobAnimationTimer.Tick -= BlobAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("contourAnimationTimer.Tick -= ContourAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("CreateBinaryCell", StringComparison.Ordinal)
                && viewSource.Contains("PaintMorphologyAnimationFrame", StringComparison.Ordinal)
                && viewSource.Contains("PaintBlobAnimationFrame", StringComparison.Ordinal)
                && viewSource.Contains("PaintContourAnimationFrame", StringComparison.Ordinal),
            results);
        Check(
            "Binary Learn animation lifetime is stopped and unsubscribed on unload",
            viewSource.Contains("private void OnLoaded(object sender, RoutedEventArgs e)", StringComparison.Ordinal)
                && viewSource.Contains("private void OnUnloaded(object sender, RoutedEventArgs e)", StringComparison.Ordinal)
                && viewSource.Contains("StopAnimations();", StringComparison.Ordinal)
                && viewSource.Contains("btnMorphologyPlay.Content = \"Play\";", StringComparison.Ordinal)
                && viewSource.Contains("btnBlobPlay.Content = \"Play\";", StringComparison.Ordinal)
                && viewSource.Contains("btnContourPlay.Content = \"Play\";", StringComparison.Ordinal),
            results);
        Check(
            "Binary Learn topic and related-tool routing remain explicit composition contracts",
            viewSource.Contains("internal void SelectTopic(int topicIndex)", StringComparison.Ordinal)
                && viewSource.Contains("internal void SetOpenRelatedToolAction(Action<VISION_MENU> action)", StringComparison.Ordinal)
                && viewSource.Contains("openRelatedToolAction?.Invoke(menu);", StringComparison.Ordinal)
                && learnWindowSource.Contains("binaryLearnView.SelectTopic(topicIndex);", StringComparison.Ordinal)
                && learnWindowSource.Contains("binaryLearnView.SetOpenRelatedToolAction(action);", StringComparison.Ordinal)
                && learnWindowXamlSource.Contains("x:Name=\"binaryLearnView\"", StringComparison.Ordinal),
            results);
        Check(
            "Binary Learn public/test facade preserves visual state access without moving mutable state",
            viewSource.Contains("internal int MorphologyAnimationStepForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal double BlobMinAreaForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal int ContourDrawModeIndexForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal void ResetMorphologyAnimationForTest()", StringComparison.Ordinal)
                && viewSource.Contains("internal void ResetBlobAnimationForTest()", StringComparison.Ordinal)
                && viewSource.Contains("internal void ResetContourAnimationForTest()", StringComparison.Ordinal)
                && viewSource.Contains("internal string ContourToolLocationDetailForTest", StringComparison.Ordinal),
            results);
        Check(
            "Binary Learn XAML preserves automation and related-tool interaction surface",
            xamlSource.Contains("OpenVisionLearnMorphologyPracticePanel", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnMorphologyOpenToolButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnBlobOpenToolButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnContourOpenToolButton", StringComparison.Ordinal)
                && xamlSource.Contains("Click=\"OpenRelatedToolButton_Click\"", StringComparison.Ordinal)
                && xamlSource.Contains("SelectionChanged=\"MorphologyModeCombo_SelectionChanged\"", StringComparison.Ordinal)
                && xamlSource.Contains("ValueChanged=\"BlobMinAreaSlider_ValueChanged\"", StringComparison.Ordinal)
                && xamlSource.Contains("SelectionChanged=\"ContourDrawModeCombo_SelectionChanged\"", StringComparison.Ordinal),
            results);
        Check(
            "Binary Learn View does not own persistence, tool creation, or algorithm lifetime",
            !viewSource.Contains("CreateBlob", StringComparison.Ordinal)
                && !viewSource.Contains("CreateContour", StringComparison.Ordinal)
                && !viewSource.Contains("new BlobTool", StringComparison.Ordinal)
                && !viewSource.Contains("new ContourTool", StringComparison.Ordinal)
                && !viewSource.Contains("VisionToolCompositionService", StringComparison.Ordinal)
                && !viewSource.Contains("OpenVisionNativePropertyGridToolFactory", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "binary-learn-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "BINARY_LEARN_PARTIAL_BOUNDARY_CONTRACT="
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
