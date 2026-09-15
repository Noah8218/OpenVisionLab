using System;
using System.Collections.Generic;
using System.IO;

internal static class GeometryLearnPartialBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(
            string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
                ? Path.Combine(
                    "D:\\OpenVisionLab-TestData",
                    "OpenVisionLab_Dev",
                    "geometry-learn-partial-retention-20260914")
                : requestedEvidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);

        string sourceRoot = Path.Combine(repositoryRoot, "src", "OpenVisionLab");
        string viewPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "GeometryLearnView.xaml.cs");
        string xamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "GeometryLearnView.xaml");
        string presenterPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "GeometryLearnPresenter.cs");
        string learnWindowPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "OpenVisionLearnWindow.xaml.cs");
        string learnWindowXamlPath = Path.Combine(sourceRoot, "UI", "VisionTest", "Wpf", "Learn", "OpenVisionLearnWindow.xaml");

        string viewSource = File.ReadAllText(viewPath);
        string xamlSource = File.ReadAllText(xamlPath);
        string presenterSource = File.ReadAllText(presenterPath);
        string learnWindowSource = File.ReadAllText(learnWindowPath);
        string learnWindowXamlSource = File.ReadAllText(learnWindowXamlPath);
        List<string> results = new();

        Check(
            "Geometry Learn Partial remains the required XAML/presentation adapter",
            viewSource.Contains("public sealed partial class GeometryLearnView : UserControl", StringComparison.Ordinal)
                && viewSource.Contains("InitializeComponent();", StringComparison.Ordinal)
                && viewSource.Contains("private readonly GeometryLearnPresenter presenter = new();", StringComparison.Ordinal)
                && viewSource.Contains("private readonly DispatcherTimer animationTimer", StringComparison.Ordinal)
                && viewSource.Contains("UpdateGeometryGuide();", StringComparison.Ordinal)
                && xamlSource.Contains("<UserControl x:Class=\"OpenVisionLab.GeometryLearnView\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"geometryTopicPanel\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"geometrySourceBox\"", StringComparison.Ordinal)
                && xamlSource.Contains("x:Name=\"geometryTargetBox\"", StringComparison.Ordinal),
            results);
        Check(
            "Geometry Learn presenter owns transform state, stages, roles, and fixed guidance",
            presenterSource.Contains("internal sealed class GeometryLearnPresenter", StringComparison.Ordinal)
                && presenterSource.Contains("internal double Angle", StringComparison.Ordinal)
                && presenterSource.Contains("internal double Scale", StringComparison.Ordinal)
                && presenterSource.Contains("internal int AnimationStep", StringComparison.Ordinal)
                && presenterSource.Contains("internal bool IsRotationApplied", StringComparison.Ordinal)
                && presenterSource.Contains("internal bool IsScaleApplied", StringComparison.Ordinal)
                && presenterSource.Contains("internal GeometryLearnRole SourceRole", StringComparison.Ordinal)
                && presenterSource.Contains("internal GeometryLearnRole TargetRole", StringComparison.Ordinal)
                && presenterSource.Contains("internal string FormulaText", StringComparison.Ordinal)
                && presenterSource.Contains("internal string AnimationStatusText", StringComparison.Ordinal),
            results);
        Check(
            "Geometry Learn presenter stays WPF-free and owns no external lifetime",
            !presenterSource.Contains("System.Windows", StringComparison.Ordinal)
                && !presenterSource.Contains("DispatcherTimer", StringComparison.Ordinal)
                && !presenterSource.Contains("UserControl", StringComparison.Ordinal)
                && !presenterSource.Contains("OpenVisionRecipe", StringComparison.Ordinal)
                && !presenterSource.Contains("OpenCvSharp", StringComparison.Ordinal),
            results);
        Check(
            "Geometry Learn View has no direct file, dialog, OpenCV, or tool-factory coupling",
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
            "Geometry Learn View owns only WPF controls, transforms, brushes, text, and timer projection",
            viewSource.Contains("UpdateGeometryGuide", StringComparison.Ordinal)
                && viewSource.Contains("ResolveBrush", StringComparison.Ordinal)
                && viewSource.Contains("geometrySourceBox.Stroke", StringComparison.Ordinal)
                && viewSource.Contains("geometryTargetBox.Background", StringComparison.Ordinal)
                && viewSource.Contains("geometryRotateTransform.Angle", StringComparison.Ordinal)
                && viewSource.Contains("geometryScaleTransform.ScaleX", StringComparison.Ordinal)
                && viewSource.Contains("animationTimer.Tick += AnimationTimer_Tick", StringComparison.Ordinal)
                && viewSource.Contains("animationTimer.Tick -= AnimationTimer_Tick", StringComparison.Ordinal),
            results);
        Check(
            "Geometry Learn animation lifetime is stopped and unsubscribed on unload",
            viewSource.Contains("private void OnLoaded(object sender, RoutedEventArgs e)", StringComparison.Ordinal)
                && viewSource.Contains("private void OnUnloaded(object sender, RoutedEventArgs e)", StringComparison.Ordinal)
                && viewSource.Contains("StopAnimations();", StringComparison.Ordinal)
                && viewSource.Contains("animationTimer.Stop();", StringComparison.Ordinal)
                && viewSource.Contains("btnGeometryPlay.Content = \"Play\";", StringComparison.Ordinal)
                && viewSource.Contains("animationTimer.Tick -= AnimationTimer_Tick", StringComparison.Ordinal),
            results);
        Check(
            "Geometry settings and animation state flow through the presenter",
            viewSource.Contains("presenter.UpdateSettings(geometryAngleSlider.Value, geometryScaleSlider.Value);", StringComparison.Ordinal)
                && viewSource.Contains("presenter.ResetAnimation();", StringComparison.Ordinal)
                && viewSource.Contains("presenter.AdvanceAnimation();", StringComparison.Ordinal)
                && viewSource.Contains("presenter.IsAnimationComplete", StringComparison.Ordinal)
                && viewSource.Contains("presenter.Angle", StringComparison.Ordinal)
                && viewSource.Contains("presenter.Scale", StringComparison.Ordinal)
                && viewSource.Contains("presenter.SourceRole", StringComparison.Ordinal)
                && viewSource.Contains("presenter.TargetRole", StringComparison.Ordinal),
            results);
        Check(
            "Geometry related-tool routing remains an explicit callback and presenter policy boundary",
            viewSource.Contains("internal void SelectTopic(int topicIndex)", StringComparison.Ordinal)
                && viewSource.Contains("internal void SetOpenRelatedToolAction(Action<VISION_MENU> action)", StringComparison.Ordinal)
                && viewSource.Contains("openRelatedToolAction?.Invoke(menu);", StringComparison.Ordinal)
                && viewSource.Contains("presenter.UpdateToolLocation(menu);", StringComparison.Ordinal)
                && learnWindowSource.Contains("geometryLearnView.SelectTopic(topicIndex);", StringComparison.Ordinal)
                && learnWindowSource.Contains("geometryLearnView.SetOpenRelatedToolAction(action);", StringComparison.Ordinal)
                && learnWindowXamlSource.Contains("x:Name=\"geometryLearnView\"", StringComparison.Ordinal),
            results);
        Check(
            "Geometry Learn test facade preserves settings, rendered transforms, stage, and reset access",
            viewSource.Contains("internal double GeometryAngleForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal double GeometryScaleForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal string GeometryFormulaTextForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal int GeometryAnimationStepForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal double GeometryRenderedAngleForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal double GeometryRenderedScaleForTest", StringComparison.Ordinal)
                && viewSource.Contains("internal void ResetGeometryAnimationForTest()", StringComparison.Ordinal)
                && viewSource.Contains("internal void AdvanceGeometryAnimationForTest()", StringComparison.Ordinal)
                && viewSource.Contains("internal void ToggleGeometryAnimationForTest()", StringComparison.Ordinal)
                && learnWindowSource.Contains("public double GeometryAngleForTest", StringComparison.Ordinal)
                && learnWindowSource.Contains("public void ResetGeometryAnimationForTest()", StringComparison.Ordinal),
            results);
        Check(
            "Geometry Learn XAML preserves automation, tool actions, sliders, and animation controls",
            xamlSource.Contains("OpenVisionLearnGeometryOpenToolButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnGeometryOpenAffineToolButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnGeometryToolLocationPanel", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnGeometryToolLocationTitle", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnGeometryToolLocationDetail", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnGeometryAngleSlider", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnGeometryScaleSlider", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnGeometryPlayButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnGeometryStepButton", StringComparison.Ordinal)
                && xamlSource.Contains("OpenVisionLearnGeometryResetButton", StringComparison.Ordinal)
                && xamlSource.Contains("ValueChanged=\"GeometrySlider_ValueChanged\"", StringComparison.Ordinal)
                && xamlSource.Contains("Click=\"GeometryPlayButton_Click\"", StringComparison.Ordinal),
            results);
        Check(
            "Learn Window owns Geometry visibility, refresh, public facade, and close lifetime",
            learnWindowSource.Contains("geometryLearnView.SelectTopic(topicIndex);", StringComparison.Ordinal)
                && learnWindowSource.Contains("geometryLearnView.UpdateGeometryGuide();", StringComparison.Ordinal)
                && learnWindowSource.Contains("geometryLearnView.StopAnimations();", StringComparison.Ordinal)
                && learnWindowSource.Contains("geometryLearnView.GeometryAnimationStepForTest", StringComparison.Ordinal)
                && learnWindowSource.Contains("public bool CanOpenGeometryToolForTest", StringComparison.Ordinal),
            results);
        Check(
            "Geometry Learn View does not own persistence, Tool creation, or algorithm lifetime",
            !viewSource.Contains("Save", StringComparison.Ordinal)
                && !viewSource.Contains("CreateRotate", StringComparison.Ordinal)
                && !viewSource.Contains("CreateAffine", StringComparison.Ordinal)
                && !viewSource.Contains("new Rotate", StringComparison.Ordinal)
                && !viewSource.Contains("new Affine", StringComparison.Ordinal)
                && !viewSource.Contains("ShowDialog", StringComparison.Ordinal)
                && !viewSource.Contains("VisionToolCompositionService", StringComparison.Ordinal),
            results);

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "geometry-learn-partial-boundary-contract.txt"),
            results);
        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "GEOMETRY_LEARN_PARTIAL_BOUNDARY_CONTRACT="
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
