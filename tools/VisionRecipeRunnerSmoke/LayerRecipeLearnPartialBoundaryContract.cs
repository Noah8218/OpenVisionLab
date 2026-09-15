using System;
using System.Collections.Generic;
using System.IO;

internal static class LayerRecipeLearnPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "layer-recipe-learn-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string sourceRoot = Path.Combine(repositoryRoot, "src", "OpenVisionLab");
        string viewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "LayerRecipeLearnView.xaml.cs");
        string xamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "LayerRecipeLearnView.xaml");
        string presenterPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "LayerRecipeLearnPresenter.cs");
        string learnWindowPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "OpenVisionLearnWindow.xaml.cs");
        string learnWindowXamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "OpenVisionLearnWindow.xaml");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string presenterSource = File.ReadAllText(presenterPath);
        string learnWindowSource = File.ReadAllText(learnWindowPath);
        string learnWindowXamlSource = File.ReadAllText(learnWindowXamlPath);
        List<string> results = new();

        Check(
            "Layer/Recipe Learn Partial remains the required XAML/presentation adapter",
            viewSource.Contains("public sealed partial class LayerRecipeLearnView : UserControl", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent();", StringComparison.Ordinal)
                && viewSource.Contains("private readonly LayerRecipeLearnPresenter presenter = new();", StringComparison.Ordinal)
                && viewSource.Contains("private readonly DispatcherTimer layerRecipeAnimationTimer", StringComparison.Ordinal)
                && viewSource.Contains("BuildLayerRecipeCells();", StringComparison.Ordinal)
                && xamlSource.Contains("<UserControl x:Class=\"OpenVisionLab.LayerRecipeLearnView\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"layerRecipeTopicPanel\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"layerRecipeLayerGrid\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"layerRecipeFlowGrid\"", StringComparison.Ordinal),
            results);
        Check(
            "Layer/Recipe Learn presenter owns fixed routes, lesson state, and explanations",
            presenterSource.Contains("internal sealed class LayerRecipeLearnPresenter", StringComparison.Ordinal)
                && presenterSource.Contains("IReadOnlyList<string> Layers", StringComparison.Ordinal)
                && presenterSource.Contains("IReadOnlyList<(string Input, string Tool, string Output)> Steps", StringComparison.Ordinal)
                && presenterSource.Contains("internal int SelectedStep", StringComparison.Ordinal)
                && presenterSource.Contains("internal int AnimationStep", StringComparison.Ordinal)
                && presenterSource.Contains("internal string FormulaText", StringComparison.Ordinal)
                && presenterSource.Contains("internal string MeaningText", StringComparison.Ordinal)
                && presenterSource.Contains("internal string AnimationStatusText", StringComparison.Ordinal)
                && presenterSource.Contains("internal bool IsRouteLayer", StringComparison.Ordinal),
            results);
        Check(
            "Layer/Recipe Learn presenter stays WPF-free and does not execute a real Recipe",
            !presenterSource.Contains("System.Windows", StringComparison.Ordinal)
                && !presenterSource.Contains("DispatcherTimer", StringComparison.Ordinal)
                && !presenterSource.Contains("UserControl", StringComparison.Ordinal)
                && !presenterSource.Contains("OpenVisionRecipe", StringComparison.Ordinal)
                && !presenterSource.Contains("OpenCvSharp", StringComparison.Ordinal),
            results);
        Check(
            "Layer/Recipe Learn View has no direct file, dialog, OpenCV, or tool-factory coupling",
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
            "Layer/Recipe Learn View owns only WPF cell, brush, text, and timer projection",
            viewSource.Contains("BuildLayerRecipeCells", StringComparison.Ordinal)
                && viewSource.Contains("AddLayerRecipeFlowCell", StringComparison.Ordinal)
                && viewSource.Contains("UpdateLayerRecipeGuide", StringComparison.Ordinal)
                && viewSource.Contains("layerRecipeLayerCells[i].Background", StringComparison.Ordinal)
                && viewSource.Contains("layerRecipeFlowCells[i].Background", StringComparison.Ordinal)
                && viewSource.Contains("layerRecipeAnimationTimer.Tick += LayerRecipeAnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("layerRecipeAnimationTimer.Tick -= LayerRecipeAnimationTimer_Tick", StringComparison.Ordinal),
            results);
        Check(
            "Layer/Recipe Learn animation lifetime is stopped and unsubscribed on unload",
            viewSource.Contains("private void OnLoaded(object sender, RoutedEventArgs e)", StringComparison.Ordinal)
                && viewSource.Contains("private void OnUnloaded(object sender, RoutedEventArgs e)", StringComparison.Ordinal)
                && viewSource.Contains("layerRecipeAnimationTimer.Stop();", StringComparison.Ordinal)
                && viewSource.Contains("btnLayerRecipePlay.Content = \"Play\";", StringComparison.Ordinal)
                && viewSource.Contains("layerRecipeAnimationTimer.Tick -= LayerRecipeAnimationTimer_Tick", StringComparison.Ordinal),
            results);
        Check(
            "Layer/Recipe selection and animation state flow through the presenter",
            viewSource.Contains("presenter.SelectStep(layerRecipeStepSlider.Value);", StringComparison.Ordinal)
                && viewSource.Contains("presenter.AdvanceAnimation();", StringComparison.Ordinal)
                && viewSource.Contains("presenter.ResetAnimation();", StringComparison.Ordinal)
                && viewSource.Contains("layerRecipeStepSlider.Value = presenter.AnimationStep;", StringComparison.Ordinal)
                && viewSource.Contains("presenter.IsSelectedFlowCell(i)", StringComparison.Ordinal)
                && viewSource.Contains("presenter.IsRouteLayer(i)", StringComparison.Ordinal),
            results);
        Check(
            "Layer/Recipe playback keeps manual selection and repeated-load guards local to the View",
            viewSource.Contains("private bool isLayerRecipeAnimationAdvancing", StringComparison.Ordinal)
                && viewSource.Contains("if (!isLayerRecipeAnimationAdvancing)", StringComparison.Ordinal)
                && viewSource.Contains("layerRecipeAnimationTimer.Stop();", StringComparison.Ordinal)
                && viewSource.Contains("if (presenter.IsAnimationComplete)", StringComparison.Ordinal)
                && viewSource.Contains("layerRecipeAnimationTimer.Start();", StringComparison.Ordinal),
            results);
        Check(
            "Layer/Recipe Learn test facade preserves selection, formula, stage, and reset access",
            viewSource.Contains("internal double LayerRecipeSelectedStepForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal string LayerRecipeFormulaTextForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal int LayerRecipeAnimationStepForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal string LayerRecipeAnimationStatusTextForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal void ResetLayerRecipeAnimationForTest()", StringComparison.Ordinal)
                && viewSource.Contains("internal void AdvanceLayerRecipeAnimationForTest()", StringComparison.Ordinal)
                && viewSource.Contains("internal void ToggleLayerRecipeAnimationForTest()", StringComparison.Ordinal)
                && learnWindowSource.Contains("public double LayerRecipeSelectedStepForTest", StringComparison.Ordinal)
                && learnWindowSource.Contains("public void ResetLayerRecipeAnimationForTest()", StringComparison.Ordinal),
            results);
        Check(
            "Layer/Recipe Learn XAML preserves automation and interaction surface",
            xamlSource.Contains("OpenVisionLearnLayerRoutingSafetyPanel", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnLayerRouteReviewLoopPanel", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnLayerRecipeStepSlider", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnLayerRecipePlayButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnLayerRecipeStepButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnLayerRecipeResetButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnLayerRecipeAnimationStatus", StringComparison.Ordinal)
                && xamlSource.Contains("ValueChanged=\"LayerRecipeStepSlider_ValueChanged\"", StringComparison.Ordinal)
                && xamlSource.Contains("Click=\"LayerRecipePlayButton_Click\"", StringComparison.Ordinal),
            results);
        Check(
            "Learn Window owns Layer/Recipe visibility, composition, and close lifetime",
            learnWindowXamlSource.Contains("x:Name=\"layerRecipeLearnView\"", StringComparison.Ordinal)
                && learnWindowSource.Contains("layerRecipeLearnView.Visibility = presentation.ShowLayerRecipeTopic", StringComparison.Ordinal)
                && learnWindowSource.Contains("layerRecipeLearnView.RefreshSelection();", StringComparison.Ordinal)
                && learnWindowSource.Contains("layerRecipeLearnView.StopAnimation();", StringComparison.Ordinal)
                && learnWindowSource.Contains("layerRecipeLearnView.LayerRecipeAnimationStepForTest", StringComparison.Ordinal),
            results);
        Check(
            "Layer/Recipe Learn View does not own persistence, Tool creation, or algorithm lifetime",
            !viewSource.Contains("Save", StringComparison.Ordinal)
                && !viewSource.Contains("CreateThreshold", StringComparison.Ordinal)
                && !viewSource.Contains("new ThresholdTool", StringComparison.Ordinal)
                && !viewSource.Contains("new LineDistanceTool", StringComparison.Ordinal)
                && !viewSource.Contains("ShowDialog", StringComparison.Ordinal)
                && !viewSource.Contains("VisionToolCompositionService", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "layer-recipe-learn-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "LAYER_RECIPE_LEARN_PARTIAL_BOUNDARY_CONTRACT="
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
