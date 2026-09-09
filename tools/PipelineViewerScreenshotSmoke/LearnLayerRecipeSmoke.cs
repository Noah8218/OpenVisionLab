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

internal static class LearnLayerRecipeSmoke
{
    internal static (int Width, int Height, double Milliseconds) Capture(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        Stopwatch elapsed = Stopwatch.StartNew();
        OpenVisionLearnWindow window = new(127, 255, false, 11);
        int externalActions = 0;
        window.SetOpenRelatedToolAction(_ => externalActions++);
        window.SetOpenPracticeSamplesAction(_ => externalActions++);
        window.ApplyThresholdRequested += (_, _) => externalActions++;
        window.Show();
        try
        {
            pump(10);
            // The host opens the practice Expander with an animation; capture only its settled layout.
            Thread.Sleep(600);
            pump(4);
            Slider slider = Find<Slider>(window, "OpenVisionLearnLayerRecipeStepSlider");
            Button play = Find<Button>(window, "OpenVisionLearnLayerRecipePlayButton");
            TextBlock status = Find<TextBlock>(window, "OpenVisionLearnLayerRecipeAnimationStatus");
            Require(slider.Value == 2 && window.LayerRecipeAnimationStepForTest == 2,
                "Initial lesson selection changed.");
            string retainedFormula = window.LayerRecipeFormulaTextForTest;
            Click(window, "Reset");
            Require(slider.Value == 2 && window.LayerRecipeAnimationStepForTest == 0
                && window.LayerRecipeFormulaTextForTest == retainedFormula,
                "Reset must clear the frame while retaining the selected slider/formula.");

            string[] routes =
            {
                "Step 1: Input=Main -> Tool=Threshold -> Output=Pin_Binary",
                "Step 2: Input=Pin_Binary -> Tool=LineDistance -> Output=Pin_Gap",
                "Step 3: Input=Main + Pin_Gap -> Tool=Overlay -> Output=Pin_Review",
                "Step 4: Input=Pin_Gap -> Tool=Accept -> Output=Inspection"
            };
            for (int step = 0; step <= 4; step++)
            {
                if (step > 0)
                    Click(window, "Step");
                Require(window.LayerRecipeAnimationStepForTest == step
                    && status.Text.StartsWith(step + " / 4", StringComparison.Ordinal), "Step/status mismatch.");
                if (step > 0)
                    Require(slider.Value == step && window.LayerRecipeFormulaTextForTest == routes[step - 1],
                        "Animation did not synchronize the selected slider and exact route.");
                status.BringIntoView();
                pump(3);
                render(window, Path.Combine(Path.GetDirectoryName(outputPath)!, $"layer-recipe-step-{step}.png"), 1040, 700);
            }
            Click(window, "Step");
            Require(window.LayerRecipeAnimationStepForTest == 1, "Step after completion must restart at 1.");

            slider.Value = 4;
            Click(window, "Play");
            Require(window.LayerRecipeAnimationStepForTest == 0 && Equals(play.Content, "Pause"),
                "Play at completion did not restart the animation.");
            WaitUntil(() => window.LayerRecipeAnimationStepForTest >= 2, pump);
            Require(Equals(play.Content, "Pause"), "Programmatic slider updates stopped playback.");
            Click(window, "Play");
            int paused = window.LayerRecipeAnimationStepForTest;
            Thread.Sleep(600);
            pump(3);
            Require(window.LayerRecipeAnimationStepForTest == paused && Equals(play.Content, "Play"), "Pause advanced a frame.");

            Click(window, "Reset");
            Click(window, "Play");
            slider.Value = 3;
            Thread.Sleep(600);
            pump(3);
            Require(window.LayerRecipeAnimationStepForTest == 3 && Equals(play.Content, "Play"),
                "Manual slider selection must stop playback.");
            Click(window, "Reset");
            window.SelectTopic((OpenVisionLearnTopicIndex)9);
            window.SelectTopic((OpenVisionLearnTopicIndex)11);
            pump(3);
            Require(window.LayerRecipeAnimationStepForTest == 3 && slider.Value == 3,
                "Returning to the topic must refresh the frame from the retained slider.");
            Require(externalActions == 0, "Lesson controls invoked an external action.");
            render(window, outputPath, 1040, 700);
            Click(window, "Reset");
            Click(window, "Play");
        }
        finally
        {
            window.Close();
        }

        Thread.Sleep(600);
        pump(3);
        Require(window.LayerRecipeAnimationStepForTest == 0, "Window Close left the lesson timer running.");
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".contract.txt"), new[]
        {
            "PASS: initial selection; reset retains slider/formula; frames 0..4; exact routes; restart",
            "PASS: timer slider synchronization; Play/Pause; manual slider stops playback; topic return; Close",
            "PASS: Tool/sample/apply callback count 0"
        });
        return (1040, 700, elapsed.Elapsed.TotalMilliseconds);
    }

    internal static (int Width, int Height, double Milliseconds) CaptureViewBoundary(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        Stopwatch elapsed = Stopwatch.StartNew();
        LayerRecipeLearnView view = new();
        Window host = new() { Title = "Layer Recipe Learn View boundary", Width = 760, Height = 700, Content = view };
        host.Show();
        try
        {
            pump(8);
            Require(view.LayerRecipeSelectedStepForTest == 2 && view.LayerRecipeAnimationStepForTest == 2,
                "An independently hosted View did not initialize its own lesson.");
            for (int cycle = 0; cycle < 2; cycle++)
            {
                view.LayerRecipeSelectedStepForTest = 3;
                view.ResetLayerRecipeAnimationForTest();
                view.ToggleLayerRecipeAnimationForTest();
                host.Content = null;
                pump(3);
                Require(!view.IsLoaded, "View unload was not exercised.");
                Thread.Sleep(600);
                pump(3);
                Require(view.LayerRecipeAnimationStepForTest == 0, "Detached View left its timer active.");
                host.Content = view;
                pump(3);
                Require(view.IsLoaded, "View reload was not exercised.");
                Thread.Sleep(600);
                pump(3);
                Require(view.LayerRecipeAnimationStepForTest == 0 && view.LayerRecipeSelectedStepForTest == 3
                    && Equals(Find<Button>(view, "OpenVisionLearnLayerRecipePlayButton").Content, "Play"),
                    "Reload changed selection or restarted playback.");
                view.ToggleLayerRecipeAnimationForTest();
                WaitUntil(() => view.LayerRecipeAnimationStepForTest > 0, pump);
                Require(view.LayerRecipeAnimationStepForTest == 1, "Reload duplicated the timer subscription.");
                view.ResetLayerRecipeAnimationForTest();
            }
            view.LayerRecipeSelectedStepForTest = 4;
            Find<TextBlock>(view, "OpenVisionLearnLayerRecipeAnimationStatus").BringIntoView();
            pump(3);
            render(host, outputPath, 760, 700);
            File.WriteAllLines(Path.ChangeExtension(outputPath, ".contract.txt"), new[]
            {
                "PASS: standalone resources and lesson initialization",
                "PASS: 2 unload/reload cycles; retained selection; no autoplay or duplicate Tick",
                "Rendered DPI: " + VisualTreeHelper.GetDpi(view).PixelsPerInchX
            });
            view.ResetLayerRecipeAnimationForTest();
            view.ToggleLayerRecipeAnimationForTest();
        }
        finally
        {
            host.Close();
        }
        Thread.Sleep(600);
        pump(3);
        Require(view.LayerRecipeAnimationStepForTest == 0, "Independent host Close left the timer active.");
        File.AppendAllText(Path.ChangeExtension(outputPath, ".contract.txt"), "PASS: independent host Close\n");
        return (760, 700, elapsed.Elapsed.TotalMilliseconds);
    }

    private static void Click(DependencyObject root, string action)
    {
        Button button = Find<Button>(root, "OpenVisionLearnLayerRecipe" + action + "Button");
        Require(button.IsEnabled, "Disabled lesson button: " + action);
        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
    }

    private static T Find<T>(DependencyObject root, string id) where T : FrameworkElement
    {
        return FindOrDefault<T>(root, id) ?? throw new InvalidOperationException("Missing lesson control: " + id);
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
        while (!condition() && timer.ElapsedMilliseconds < 1800)
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
