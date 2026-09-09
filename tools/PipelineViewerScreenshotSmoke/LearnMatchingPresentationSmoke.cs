using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

internal static class LearnMatchingPresentationSmoke
{
    internal static (int Width, int Height, double Milliseconds) Capture(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        Stopwatch elapsed = Stopwatch.StartNew();
        List<string> observations = new();
        OpenVisionLearnWindow window = new(127, 255, false, 9);
        int toolRequests = 0;
        int visit = 0;
        window.Show();
        try
        {
            pump(12);
            Require(!window.CanOpenMatchingToolForTest && !window.CanOpenFeatureMatchingToolForTest,
                "Standalone Learn enabled a Tool command.");
            window.SetOpenRelatedToolAction(_ => toolRequests++);

            foreach (int topic in new[] { 9, 12, 10, 9 })
            {
                window.SelectTopic((OpenVisionLearnTopicIndex)topic);
                pump(4);
                bool feature = topic == 10;
                string prefix = feature ? "OpenVisionLearnFeatureMatching" : "OpenVisionLearnMatching";
                Slider slider = Find<Slider>(window, feature ? "OpenVisionLearnFeatureMatchMinSlider" : "OpenVisionLearnMatchingThresholdSlider");
                slider.Value = feature ? 6 : 0.85;
                pump(4);
                Require(feature ? window.FeatureMatchingFormulaTextForTest.Contains("Required=6", StringComparison.Ordinal)
                    : window.MatchingFormulaTextForTest.Contains(topic == 12 ? "EdgeScoreMax=1.00" : "BestScore=1.00", StringComparison.Ordinal),
                    "A topic or slider change did not update the rendered formula.");

                Click(window, prefix + "ResetButton");
                for (int step = 0; step <= 3; step++)
                {
                    if (step > 0)
                        Click(window, prefix + "StepButton");
                    pump(2);
                    int actualStep = feature ? window.FeatureMatchingAnimationStepForTest : window.MatchingAnimationStepForTest;
                    string status = feature ? window.FeatureMatchingAnimationStatusTextForTest : window.MatchingAnimationStatusTextForTest;
                    Require(actualStep == step && status.StartsWith(step + " / 3 -", StringComparison.Ordinal),
                        "The rendered animation step/status disagreed after a button click.");
                    Find<TextBlock>(window, prefix + "AnimationStatus").BringIntoView();
                    pump(3);
                    string framePath = Path.Combine(Path.GetDirectoryName(outputPath)!, $"matching-visit-{visit}-topic-{topic}-step-{step}.png");
                    render(window, framePath, 1040, 700);
                }

                Require(feature ? window.FeatureMatchingAnimationStatusTextForTest.Contains("NG", StringComparison.Ordinal)
                    : window.MatchingAnimationStatusTextForTest.Contains("OK", StringComparison.Ordinal),
                    "The final visible judgment did not match the teaching inputs.");
                Click(window, prefix + "StepButton");
                Require((feature ? window.FeatureMatchingAnimationStepForTest : window.MatchingAnimationStepForTest) == 1,
                    "Step after completion did not restart at step 1.");

                slider.Value = feature ? 4 : 1.0;
                pump(3);
                Require((feature ? window.FeatureMatchingAnimationStepForTest : window.MatchingAnimationStepForTest) == 3,
                    "Changing a teaching value did not return to the result frame.");
                Require((feature ? window.FeatureMatchingAnimationStatusTextForTest : window.MatchingAnimationStatusTextForTest).Contains("OK", StringComparison.Ordinal),
                    "The updated teaching value did not render its OK judgment.");

                Click(window, prefix + "ResetButton");
                Click(window, prefix + "PlayButton");
                Require(Equals(Find<Button>(window, prefix + "PlayButton").Content, "Pause"), "Play did not expose Pause state.");
                WaitUntil(() => (feature ? window.FeatureMatchingAnimationStepForTest : window.MatchingAnimationStepForTest) > 0, pump);
                Click(window, prefix + "PlayButton");
                int pausedStep = feature ? window.FeatureMatchingAnimationStepForTest : window.MatchingAnimationStepForTest;
                Thread.Sleep(550);
                pump(4);
                Require((feature ? window.FeatureMatchingAnimationStepForTest : window.MatchingAnimationStepForTest) == pausedStep,
                    "A paused animation advanced.");
                observations.Add($"Topic {topic}: slider/formula, reset, steps 0..3, restart, result refresh, play/pause PASS");
                visit++;
            }

            Require(toolRequests == 0, "Teaching interactions invoked a Tool command.");
            render(window, outputPath, 1040, 700);
            window.ResetMatchingAnimationForTest();
            window.ResetFeatureMatchingAnimationForTest();
            window.ToggleMatchingAnimationForTest();
            window.ToggleFeatureMatchingAnimationForTest();
        }
        finally
        {
            window.Close();
        }

        Thread.Sleep(550);
        pump(4);
        Require(window.MatchingAnimationStepForTest == 0 && window.FeatureMatchingAnimationStepForTest == 0,
            "Closing Learn left a Matching-family timer running.");
        observations.Add("Close: both animations stopped; Tool callback count 0");
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".contract.txt"), observations);
        return (1040, 700, elapsed.Elapsed.TotalMilliseconds);
    }

    internal static (int Width, int Height, double Milliseconds) CaptureViewBoundary(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        Stopwatch elapsed = Stopwatch.StartNew();
        MatchingLearnView view = new();
        List<DEFINE.VISION_MENU> requests = new();
        List<string> observations = new();
        Window host = new() { Title = "Matching Learn View boundary", Width = 760, Height = 700, Content = view };
        view.SelectTopic(9);
        host.Show();
        try
        {
            pump(10);
            Require(!view.CanOpenMatchingToolForTest && !view.CanOpenFeatureMatchingToolForTest,
                "An independently hosted View enabled a Tool without an explicit callback.");
            view.SetOpenRelatedToolAction(requests.Add);
            foreach (var entry in new[]
            {
                (Topic: 9, Menu: DEFINE.VISION_MENU.Matching, Id: "OpenVisionLearnMatchingOpenToolButton"),
                (Topic: 12, Menu: DEFINE.VISION_MENU.EdgeBasedMatching, Id: "OpenVisionLearnEdgeBasedMatchingOpenToolButton"),
                (Topic: 10, Menu: DEFINE.VISION_MENU.FeatureMatching, Id: "OpenVisionLearnFeatureMatchingOpenToolButton")
            })
            {
                int before = requests.Count;
                view.SelectTopic(entry.Topic);
                pump(3);
                Require(requests.Count == before, "Topic selection implicitly opened a Tool.");
                Click(view, entry.Id);
                Require(requests.Count == before + 1 && requests[^1] == entry.Menu,
                    "The extracted Tool link did not invoke its exact callback once.");
                string title = entry.Topic == 10 ? view.FeatureMatchingToolLocationTitleForTest : view.MatchingToolLocationTitleForTest;
                Require(title.StartsWith("열림: " + entry.Menu + " |", StringComparison.Ordinal),
                    "The explicit Tool result did not update its own topic guidance.");
            }
            observations.Add("Standalone resources/namescope and all three explicit Tool callbacks PASS");

            view.SelectTopic(9);
            view.MatchingThresholdForTest = 0.75;
            view.FeatureGoodMatchMinForTest = 6;
            for (int cycle = 0; cycle < 2; cycle++)
            {
                view.ResetMatchingAnimationForTest();
                view.ResetFeatureMatchingAnimationForTest();
                view.ToggleMatchingAnimationForTest();
                view.ToggleFeatureMatchingAnimationForTest();
                host.Content = null;
                pump(4);
                Require(!view.IsLoaded, "Removing the View did not exercise Unloaded.");
                Thread.Sleep(600);
                pump(3);
                Require(view.MatchingAnimationStepForTest == 0 && view.FeatureMatchingAnimationStepForTest == 0,
                    "An unloaded View left an animation timer advancing.");

                host.Content = view;
                pump(4);
                Require(view.IsLoaded, "Rehosting the View did not exercise Loaded.");
                Thread.Sleep(550);
                pump(3);
                Require(view.MatchingAnimationStepForTest == 0 && view.FeatureMatchingAnimationStepForTest == 0,
                    "Rehosting automatically resumed playback.");
                Require(view.MatchingThresholdForTest == 0.75 && view.FeatureGoodMatchMinForTest == 6,
                    "Rehosting lost topic teaching inputs.");
                Require(Equals(Find<Button>(view, "OpenVisionLearnMatchingPlayButton").Content, "Play")
                    && Equals(Find<Button>(view, "OpenVisionLearnFeatureMatchingPlayButton").Content, "Play"),
                    "Rehosting left stale Pause button content.");

                view.ToggleMatchingAnimationForTest();
                WaitUntil(() => view.MatchingAnimationStepForTest > 0, pump);
                Require(view.MatchingAnimationStepForTest == 1, "Reload duplicated the Matching timer subscription.");
                view.StopAnimations();
                view.ToggleFeatureMatchingAnimationForTest();
                WaitUntil(() => view.FeatureMatchingAnimationStepForTest > 0, pump);
                Require(view.FeatureMatchingAnimationStepForTest == 1, "Reload duplicated the FeatureMatching timer subscription.");
                view.StopAnimations();
            }
            observations.Add("Two unload/reload cycles: timers stop, inputs persist, no autoplay or duplicate Tick PASS");
            Require(requests.Count == 3, "Lifecycle or teaching operations invoked an extra Tool callback.");
            view.SelectTopic(12);
            Find<TextBlock>(view, "OpenVisionLearnMatchingAnimationStatus").BringIntoView();
            pump(3);
            render(host, outputPath, 760, 700);
            observations.Add("Rendered DPI: " + VisualTreeHelper.GetDpi(view).PixelsPerInchX);
            view.ResetMatchingAnimationForTest();
            view.ResetFeatureMatchingAnimationForTest();
            view.ToggleMatchingAnimationForTest();
            view.ToggleFeatureMatchingAnimationForTest();
        }
        finally
        {
            host.Close();
        }

        Thread.Sleep(600);
        pump(4);
        Require(view.MatchingAnimationStepForTest == 0 && view.FeatureMatchingAnimationStepForTest == 0,
            "Closing an independent host left timers active.");
        observations.Add("Independent host Close stops both timers PASS");
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".contract.txt"), observations);
        return (760, 700, elapsed.Elapsed.TotalMilliseconds);
    }

    private static void Click(DependencyObject root, string id)
    {
        Button button = Find<Button>(root, id);
        Require(button.IsEnabled, "Expected enabled teaching button: " + id);
        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
    }

    private static T Find<T>(DependencyObject root, string id) where T : FrameworkElement
    {
        return FindOrDefault<T>(root, id) ?? throw new InvalidOperationException("Missing teaching control: " + id);
    }

    private static T? FindOrDefault<T>(DependencyObject root, string id) where T : FrameworkElement
    {
        if (root is T element && AutomationProperties.GetAutomationId(element) == id)
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
        while (!condition() && timer.ElapsedMilliseconds < 1500)
        {
            pump(1);
            Thread.Sleep(10);
        }
        Require(condition(), "Play did not advance the animation timer.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
