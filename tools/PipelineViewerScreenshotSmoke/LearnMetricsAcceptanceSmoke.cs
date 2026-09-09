using OpenVisionLab;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

internal static class LearnMetricsAcceptanceSmoke
{
    internal static (int Width, int Height, double Milliseconds) Capture(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        Stopwatch elapsed = Stopwatch.StartNew();
        OpenVisionLearnWindow window = new(127, 255, false, 13);
        int externalActions = 0;
        window.SetOpenRelatedToolAction(_ => externalActions++);
        window.SetOpenPracticeSamplesAction(_ => externalActions++);
        window.ApplyThresholdRequested += (_, _) => externalActions++;
        window.Show();
        try
        {
            pump(10);
            // Capture the settled host layout, independently of its Expander transition timing.
            Thread.Sleep(600);
            pump(3);
            Require(window.MetricsAcceptanceAnimationStepForTest == 3, "Initial stage changed.");
            Require(!window.IsPracticeWorkflowExpandedForTest && !window.IsMetricGateCheatSheetExpandedForTest,
                "Repeated guidance must start collapsed.");
            string[] formulas =
            {
                "Avg 0.45..0.60 | Range <= 0.10 | Max <= 0.65",
                "Samples=5 | 측정값 5개를 모두 확인합니다.",
                "DistanceMmAvg=0.56 -> OK",
                "Range=0.33 / Max=0.82 -> NG"
            };
            string[] samples = { "0.50", "0.51", "0.49", "0.82", "0.50" };
            Click(window, "Reset");
            for (int stage = 0; stage <= 3; stage++)
            {
                if (stage > 0)
                    Click(window, "Step");
                Require(window.MetricsAcceptanceAnimationStepForTest == stage
                    && window.MetricsAcceptanceFormulaTextForTest == formulas[stage]
                    && window.MetricsAcceptanceAnimationStatusTextForTest.StartsWith(stage + " / 3 -", StringComparison.Ordinal),
                    "Stage, formula or status changed.");
                UniformGrid grid = Find<UniformGrid>(window, "metricsAcceptanceSampleGrid");
                Require(grid.Children.Count == 5, "Sample count changed.");
                for (int index = 0; index < 5; index++)
                {
                    Border cell = (Border)grid.Children[index];
                    Require(((TextBlock)cell.Child).Text == (stage == 0 ? "-" : samples[index]), "Sample text changed.");
                    string brush = stage < 3 ? "Neutral" : index == 3 ? "Warning" : "Pass";
                    Require(((SolidColorBrush)cell.Background).Color ==
                        ((SolidColorBrush)window.FindResource("Learn.Animation." + brush + "Brush")).Color, "Sample highlight changed.");
                }
                pump(3);
                render(window, Path.Combine(Path.GetDirectoryName(outputPath)!, $"metrics-step-{stage}.png"), 1040, 700);
            }
            Expander sheet = Find<Expander>(window, "OpenVisionLearnMetricGateCheatSheetPanel");
            sheet.IsExpanded = true;
            pump(3);
            Thread.Sleep(600);
            pump(3);
            Require(window.IsMetricGateCheatSheetExpandedForTest, "Cheat-sheet facade did not reflect the control.");
            render(window, Path.Combine(Path.GetDirectoryName(outputPath)!, "metrics-cheat-sheet.png"), 1040, 700);
            sheet.IsExpanded = false;
            Click(window, "Step");
            Require(window.MetricsAcceptanceAnimationStepForTest == 1, "Step after completion must restart at 1.");
            Click(window, "Reset");
            window.SelectTopic((OpenVisionLearnTopicIndex)11);
            window.SelectTopic((OpenVisionLearnTopicIndex)13);
            pump(3);
            Require(window.MetricsAcceptanceAnimationStepForTest == 0, "Topic return must retain the current stage.");
            Button play = Find<Button>(window, "OpenVisionLearnMetricsAcceptancePlayButton");
            Click(window, "Play");
            WaitUntil(() => window.MetricsAcceptanceAnimationStepForTest > 0, pump);
            Click(window, "Play");
            int paused = window.MetricsAcceptanceAnimationStepForTest;
            Thread.Sleep(600);
            pump(3);
            Require(window.MetricsAcceptanceAnimationStepForTest == paused && Equals(play.Content, "Play"), "Pause advanced a frame.");
            Click(window, "Play");
            WaitUntil(() => window.MetricsAcceptanceAnimationStepForTest == 3, pump);
            Require(Equals(play.Content, "Play"), "Completion did not stop playback.");
            Click(window, "Play");
            Require(window.MetricsAcceptanceAnimationStepForTest == 0 && Equals(play.Content, "Pause"), "Play at completion did not reset.");
            Click(window, "Reset");
            Require(externalActions == 0, "Lesson invoked an external Tool/sample/apply action.");
            render(window, outputPath, 1040, 700);
            Click(window, "Play");
        }
        finally
        {
            window.Close();
        }
        Thread.Sleep(600);
        pump(3);
        Require(window.MetricsAcceptanceAnimationStepForTest == 0, "Close left the timer active.");
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".contract.txt"), new[]
        {
            "PASS: stages 0..3; exact formulas, sample values and highlight colors; collapsed/expanded guide",
            "PASS: Step wrap; topic return; real timer Play/Pause/completion/restart; Close",
            "PASS: external Tool/sample/apply action count 0",
            "Rendered DPI: " + VisualTreeHelper.GetDpi(window).PixelsPerInchX
        });
        return (1040, 700, elapsed.Elapsed.TotalMilliseconds);
    }

    internal static (int Width, int Height, double Milliseconds) CaptureViewBoundary(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        Stopwatch elapsed = Stopwatch.StartNew();
        MetricsAcceptanceLearnView view = new();
        Window host = new() { Title = "Metrics Acceptance Learn View boundary", Width = 760, Height = 700, Content = view };
        host.Show();
        try
        {
            pump(8);
            Require(view.AnimationStepForTest == 3 && view.FormulaTextForTest == "Range=0.33 / Max=0.82 -> NG",
                "Standalone View did not initialize its own lesson.");
            Find<Expander>(view, "OpenVisionLearnMetricGateCheatSheetPanel").IsExpanded = true;
            for (int cycle = 0; cycle < 2; cycle++)
            {
                view.ResetAnimation();
                view.AdvanceAnimation();
                view.AdvanceAnimation();
                view.ToggleAnimation();
                host.Content = null;
                pump(3);
                Require(!view.IsLoaded, "View unload was not exercised.");
                Thread.Sleep(600);
                pump(3);
                Require(view.AnimationStepForTest == 2, "Detached View left its timer active.");
                host.Content = view;
                pump(3);
                Require(view.IsLoaded, "View reload was not exercised.");
                Thread.Sleep(600);
                pump(3);
                Require(view.AnimationStepForTest == 2 && view.IsCheatSheetExpandedForTest
                    && Equals(Find<Button>(view, "OpenVisionLearnMetricsAcceptancePlayButton").Content, "Play"),
                    "Reload changed retained stage/guide or restarted playback.");
                view.ResetAnimation();
                view.ToggleAnimation();
                WaitUntil(() => view.AnimationStepForTest > 0, pump);
                Require(view.AnimationStepForTest == 1, "Reload duplicated the timer subscription.");
                view.ResetAnimation();
            }
            view.AdvanceAnimation();
            view.AdvanceAnimation();
            view.AdvanceAnimation();
            pump(3);
            render(host, outputPath, 760, 700);
            view.ResetAnimation();
            view.ToggleAnimation();
        }
        finally
        {
            host.Close();
        }
        Thread.Sleep(600);
        pump(3);
        Require(view.AnimationStepForTest == 0, "Independent host Close left the timer active.");
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".contract.txt"), new[]
        {
            "PASS: standalone lesson/resources; 2 unload/reload cycles; retained stage and expanded guide",
            "PASS: no autoplay/duplicate Tick; independent host Close",
            "Rendered DPI: " + VisualTreeHelper.GetDpi(view).PixelsPerInchX
        });
        return (760, 700, elapsed.Elapsed.TotalMilliseconds);
    }

    private static void Click(DependencyObject root, string action)
    {
        Button button = Find<Button>(root, "OpenVisionLearnMetricsAcceptance" + action + "Button");
        Require(button.IsEnabled, "Disabled lesson button: " + action);
        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
    }

    private static T Find<T>(DependencyObject root, string id) where T : FrameworkElement
    {
        return FindOrDefault<T>(root, id) ?? throw new InvalidOperationException("Missing lesson control: " + id);
    }

    private static T? FindOrDefault<T>(DependencyObject root, string id) where T : FrameworkElement
    {
        if (root is T element && (AutomationProperties.GetAutomationId(element) == id || element.Name == id))
            return element;
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
        {
            T? found = FindOrDefault<T>(VisualTreeHelper.GetChild(root, index), id);
            if (found != null)
                return found;
        }
        return null;
    }

    private static void WaitUntil(Func<bool> condition, Action<int> pump)
    {
        Stopwatch timer = Stopwatch.StartNew();
        while (!condition() && timer.ElapsedMilliseconds < 2300)
        {
            pump(1);
            Thread.Sleep(10);
        }
        Require(condition(), "Lesson timer did not advance.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
