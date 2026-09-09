using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using static OpenVisionLab.DEFINE;

internal static class LearnGrayscaleSmoke
{
    private static readonly (string Name, int Topic, string Setting, double[] Values)[] topics =
    {
        ("Brightness", 1, "BrightnessOffsetForTest", new double[] { -80, 0, 80 }),
        ("Filter", 3, "FilterModeIndexForTest", new double[] { 0, 1, 2 }),
        ("Arithmetic", 14, "ArithmeticModeIndexForTest", new double[] { 0, 1, 2, 3, 4 })
    };

    internal static (int Width, int Height, double Milliseconds) Capture(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        Stopwatch elapsed = Stopwatch.StartNew();
        string directory = Path.GetDirectoryName(outputPath)!;
        List<string> states = new();
        foreach (var topic in topics)
        {
            OpenVisionLearnWindow window = new(127, 255, false, topic.Topic);
            int actions = 0;
            window.SetOpenRelatedToolAction(_ => actions++);
            window.SetOpenPracticeSamplesAction(_ => actions++);
            window.ApplyThresholdRequested += (_, _) => actions++;
            window.Show();
            try
            {
                pump(8);
                Thread.Sleep(600);
                pump(3);
                Border panel = Find<Border>(window, char.ToLowerInvariant(topic.Name[0]) + topic.Name.Substring(1) + "TopicPanel");
                Descendants<ScrollViewer>(panel).First().ScrollToEnd();
                pump(3);
                Record("initial");
                Click(window, "OpenVisionLearn" + topic.Name + "ResetButton");
                for (int step = 0; step <= 3; step++)
                {
                    if (step > 0) Click(window, "OpenVisionLearn" + topic.Name + "StepButton");
                    Require(Stage() == step, topic.Name + " step sequence changed.");
                    Record("step-" + step);
                    pump(2);
                    render(window, Path.Combine(directory, topic.Name + "-step-" + step + ".png"), 1040, 700);
                }
                foreach (double value in topic.Values)
                {
                    Set(window, topic.Setting, value);
                    pump(2);
                    Require(Stage() == 3, topic.Name + " setting did not reveal final frame.");
                    Record("setting-" + value.ToString(CultureInfo.InvariantCulture));
                    render(window, Path.Combine(directory, topic.Name + "-setting-" + value.ToString(CultureInfo.InvariantCulture) + ".png"), 1040, 700);
                }
                Click(window, "OpenVisionLearn" + topic.Name + "ResetButton");
                Click(window, "OpenVisionLearn" + topic.Name + "StepButton");
                window.SelectTopic((OpenVisionLearnTopicIndex)13);
                window.SelectTopic((OpenVisionLearnTopicIndex)topic.Topic);
                pump(2);
                Require(Stage() == 1, topic.Name + " topic refresh changed the stage.");
                Record("topic-return");
                Click(window, "OpenVisionLearn" + topic.Name + "ResetButton");
                Click(window, "OpenVisionLearn" + topic.Name + "PlayButton");
                WaitUntil(() => Stage() > 0, pump);
                Click(window, "OpenVisionLearn" + topic.Name + "PlayButton");
                int paused = Stage();
                Thread.Sleep(550);
                pump(2);
                Require(Stage() == paused, topic.Name + " pause left timer active.");
                Click(window, "OpenVisionLearn" + topic.Name + "ResetButton");
                Click(window, "OpenVisionLearn" + topic.Name + "PlayButton");
                window.SelectTopic((OpenVisionLearnTopicIndex)13);
                WaitUntil(() => Stage() > 0, pump);
                Set(window, topic.Setting, topic.Values[0]);
                Require(Stage() == 3, topic.Name + " hidden setting did not complete and pause.");
                window.SelectTopic((OpenVisionLearnTopicIndex)topic.Topic);
                Click(window, "OpenVisionLearn" + topic.Name + "ResetButton");
                Require(actions == 0, "Lesson interaction unexpectedly ran an external action.");
                Click(window, "OpenVisionLearn" + topic.Name + "PlayButton");

                int Stage() => (int)Get(window, topic.Name + "AnimationStepForTest");
                void Record(string label) => states.Add(topic.Name + "|" + label + "|" + Stage() + "|" + Get(window, topic.Setting)
                    + "|" + Get(window, topic.Name + "FormulaTextForTest") + "|" + Get(window, topic.Name + "AnimationStatusTextForTest") + "|" + Describe(panel));
            }
            finally { window.Close(); }
            Thread.Sleep(550);
            pump(2);
            Require((int)Get(window, topic.Name + "AnimationStepForTest") == 0, topic.Name + " Close left timer active.");
        }

        foreach (var initial in new[] { (Threshold: 127D, Max: 255D, Invert: false), (Threshold: 255D, Max: 64D, Invert: true), (Threshold: double.NaN, Max: double.PositiveInfinity, Invert: false) })
        {
            OpenVisionLearnWindow window = new(initial.Threshold, initial.Max, initial.Invert, 2);
            int actions = 0;
            window.SetOpenRelatedToolAction(_ => actions++);
            window.Show();
            try
            {
                pump(8);
                Find<TabControl>(window, "OpenVisionLearnThresholdTabs").SelectedIndex = 1;
                Thread.Sleep(600);
                pump(3);
                string max = double.IsInfinity(initial.Max) ? "invalid" : initial.Max.ToString(CultureInfo.InvariantCulture);
                Record("initial");
                foreach (bool invert in new[] { false, true })
                {
                    window.IsInvertedForTest = invert;
                    foreach (int threshold in new[] { 0, 127, 255 })
                    {
                        window.ThresholdValueForTest = threshold;
                        pump(2);
                        string label = (invert ? "inverse" : "binary") + "-" + threshold;
                        Record(label);
                        render(window, Path.Combine(directory, "Threshold-" + max + "-" + label + ".png"), 1040, 700);
                    }
                }
                window.ThresholdValueForTest = 127;
                Click(window, "OpenVisionLearnThresholdAnimationButton");
                WaitUntil(() => window.ThresholdValueForTest != 127, pump);
                window.ThresholdValueForTest = 40;
                window.IsInvertedForTest = false;
                window.SelectTopic((OpenVisionLearnTopicIndex)13);
                WaitUntil(() => window.ThresholdValueForTest != 40, pump);
                Click(window, "OpenVisionLearnThresholdAnimationButton");
                double paused = window.ThresholdValueForTest;
                Thread.Sleep(180);
                pump(2);
                Require(window.ThresholdValueForTest == paused, "Threshold pause failed.");
                window.SelectTopic((OpenVisionLearnTopicIndex)2);
                Require(actions == 0, "Threshold controls ran a Tool.");
                window.ThresholdValueForTest = 160;
                window.IsInvertedForTest = true;
                int applies = 0;
                window.ApplyThresholdRequested += (sender, args) =>
                {
                    Require(ReferenceEquals(sender, window) && args.Threshold == 160 && args.Invert, "Apply sender/payload changed.");
                    applies++;
                };
                Click(window, "OpenVisionLearnThresholdApplyButton");
                Require(applies == 1 && window.IsVisible, "Apply did not fire exactly once or closed the Window.");
                InvalidOperationException expected = new("Expected apply failure");
                window.ApplyThresholdRequested += (_, _) => throw expected;
                try { window.ApplyForTest(); throw new InvalidOperationException("Apply exception was swallowed."); }
                catch (InvalidOperationException error) when (ReferenceEquals(error, expected)) { }
                Find<TabControl>(window, "OpenVisionLearnThresholdTabs").SelectedIndex = 0;
                pump(2);
                Click(window, "OpenVisionLearnThresholdOpenToolButton");
                Require(actions == 1 && window.FoundationToolLocationTitleForTest == "열림: Threshold | 찾을 위치: Parameter panel", "Threshold Tool/Foundation hint path changed.");
                render(window, outputPath, 1040, 700);
                window.ThresholdValueForTest = 127;
                Click(window, "OpenVisionLearnThresholdAnimationButton");
                void Record(string label) => states.Add("Threshold|" + max + "|" + label + "|" + window.ThresholdValueForTest + "|"
                    + window.IsInvertedForTest + "|" + window.FormulaTextForTest + "|" + Find<TextBlock>(window, "txtMaxValue").Text
                    + "|" + ((TranslateTransform)Find<Border>(window, "thresholdMarker").RenderTransform).X.ToString("R", CultureInfo.InvariantCulture)
                    + "|" + Describe(Find<UniformGrid>(window, "resultGrid")));
            }
            finally { window.Close(); }
            Thread.Sleep(180);
            pump(2);
            Require(window.ThresholdValueForTest == 127, "Threshold Close left timer active.");
        }
        File.WriteAllLines(Path.Combine(directory, "grayscale-states.txt"), states);
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".contract.txt"), new[] { "PASS: four topic states/settings/timer/hidden/Close contracts", "PASS: explicit Apply sender/payload/exception, no implicit actions, Threshold Tool/Foundation hint" });
        return (1040, 700, elapsed.Elapsed.TotalMilliseconds);
    }

    internal static (int Width, int Height, double Milliseconds) CaptureView(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        Stopwatch elapsed = Stopwatch.StartNew();
        GrayscaleLearnView view = new();
        Window host = new() { Title = "Grayscale Learn View boundary", Width = 760, Height = 700, Content = view };
        List<string> evidence = new();
        host.Show();
        try
        {
            pump(6);
            foreach (var topic in topics)
            {
                view.SelectTopic(topic.Topic);
                Set(view, topic.Setting, topic.Values.Last());
                pump(3);
                for (int cycle = 0; cycle < 2; cycle++)
                {
                    Click(view, "OpenVisionLearn" + topic.Name + "ResetButton");
                    Click(view, "OpenVisionLearn" + topic.Name + "StepButton");
                    Click(view, "OpenVisionLearn" + topic.Name + "PlayButton");
                    host.Content = null;
                    pump(3);
                    Require(!view.IsLoaded, "View unload was not exercised.");
                    Thread.Sleep(550);
                    pump(2);
                    Require(Stage() == 1, "Detached View timer advanced.");
                    host.Content = view;
                    pump(3);
                    Thread.Sleep(550);
                    pump(2);
                    Require(Stage() == 1 && Convert.ToDouble(Get(view, topic.Setting)) == topic.Values.Last()
                        && Equals(Find<Button>(view, "OpenVisionLearn" + topic.Name + "PlayButton").Content, "Play"), "Reload changed state or autoplay.");
                    Click(view, "OpenVisionLearn" + topic.Name + "ResetButton");
                    Click(view, "OpenVisionLearn" + topic.Name + "PlayButton");
                    WaitUntil(() => Stage() > 0, pump);
                    Require(Stage() == 1, "Reload duplicated timer callbacks.");
                    Click(view, "OpenVisionLearn" + topic.Name + "ResetButton");
                }
                for (int step = 0; step < 3; step++) Click(view, "OpenVisionLearn" + topic.Name + "StepButton");
                pump(2);
                render(host, Path.Combine(Path.GetDirectoryName(outputPath)!, topic.Name + "-standalone.png"), 760, 700);
                evidence.Add("PASS: " + topic.Name + " two unload/reload cycles, retained stage/settings, no autoplay/duplicate callback");
                int Stage() => (int)Get(view, topic.Name + "AnimationStepForTest");
            }
            view.SelectTopic(2);
            Find<TabControl>(view, "OpenVisionLearnThresholdTabs").SelectedIndex = 1;
            pump(3);
            for (int cycle = 0; cycle < 2; cycle++)
            {
                view.ThresholdValueForTest = 127;
                view.IsInvertedForTest = true;
                Click(view, "OpenVisionLearnThresholdAnimationButton");
                host.Content = null;
                pump(3);
                double detached = view.ThresholdValueForTest;
                Thread.Sleep(180);
                pump(2);
                Require(!view.IsLoaded && view.ThresholdValueForTest == detached, "Detached Threshold timer advanced.");
                host.Content = view;
                pump(3);
                Thread.Sleep(180);
                pump(2);
                Require(view.ThresholdValueForTest == detached && view.IsInvertedForTest
                    && Equals(Find<Button>(view, "OpenVisionLearnThresholdAnimationButton").Content, "Play"), "Threshold reload lost state or enabled autoplay.");
                view.ThresholdValueForTest = 127;
                Click(view, "OpenVisionLearnThresholdAnimationButton");
                WaitUntil(() => view.ThresholdValueForTest != 127, pump);
                Click(view, "OpenVisionLearnThresholdAnimationButton");
                Require(Math.Abs(view.ThresholdValueForTest - 127) == 5, "Threshold reload duplicated a callback.");
            }
            int opened = 0;
            view.ThresholdToolOpened += (_, _) => opened++;
            foreach (var link in new[]
            {
                (Topic: 1, Button: "btnBrightnessOpenMeanTool", Menu: VISION_MENU.Mean, Hint: "BrightnessToolLocationTitleForTest"),
                (Topic: 1, Button: "btnBrightnessOpenHistogramTool", Menu: VISION_MENU.Histogram, Hint: "BrightnessToolLocationTitleForTest"),
                (Topic: 3, Button: "btnFilteringOpenTool", Menu: VISION_MENU.Filter, Hint: "FilteringToolLocationTitleForTest"),
                (Topic: 14, Button: "btnArithmeticOpenTool", Menu: VISION_MENU.Arithmetic, Hint: "ArithmeticToolLocationTitleForTest"),
                (Topic: 2, Button: "btnThresholdOpenTool", Menu: VISION_MENU.Threshold, Hint: "")
            })
            {
                view.SelectTopic(link.Topic);
                if (link.Topic == 2) Find<TabControl>(view, "OpenVisionLearnThresholdTabs").SelectedIndex = 0;
                pump(2);
                Button button = Find<Button>(view, link.Button);
                Require(!button.IsEnabled, "Tool button started enabled without a callback.");
                string before = link.Hint.Length == 0 ? "" : (string)Get(view, link.Hint);
                InvalidOperationException expected = new("Expected Tool callback failure");
                view.SetOpenRelatedToolAction(_ => throw expected);
                try { button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); throw new InvalidOperationException("Tool failure swallowed."); }
                catch (InvalidOperationException error) when (ReferenceEquals(error, expected)) { }
                Require(opened == 0 && (link.Hint.Length == 0 || (string)Get(view, link.Hint) == before), "Failure updated Tool hint/event.");
                int actions = 0;
                view.SetOpenRelatedToolAction(menu => { Require(menu == link.Menu, "Tool menu changed."); actions++; });
                Require(actions == 0, "Callback installation executed a Tool.");
                button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                Require(actions == 1 && (link.Hint.Length == 0 ? opened == 1 : ((string)Get(view, link.Hint)).StartsWith("열림: " + link.Menu, StringComparison.Ordinal)), "Explicit Tool result changed.");
                view.SetOpenRelatedToolAction(null!);
                Require(!button.IsEnabled && actions == 1, "Removing callback ran a Tool or left button enabled.");
            }
            Find<TabControl>(view, "OpenVisionLearnThresholdTabs").SelectedIndex = 1;
            view.ThresholdValueForTest = 160;
            int applies = 0;
            view.ApplyThresholdRequested += (sender, args) => { Require(ReferenceEquals(sender, view) && args.Threshold == 160 && args.Invert, "Standalone Apply payload/sender changed."); applies++; };
            Click(view, "OpenVisionLearnThresholdApplyButton");
            Require(applies == 1 && host.IsVisible, "Apply closed host or dispatched twice.");
            pump(3);
            render(host, outputPath, 760, 700);
            view.ThresholdValueForTest = 127;
            Click(view, "OpenVisionLearnThresholdAnimationButton");
            view.CloseRequested += (_, _) => host.Close();
            Click(view, "OpenVisionLearnCloseButton");
            pump(3);
            Require(!host.IsVisible, "Close request did not reach host.");
        }
        finally { if (host.IsVisible) host.Close(); }
        double closed = view.ThresholdValueForTest;
        Thread.Sleep(180);
        pump(2);
        Require(view.ThresholdValueForTest == closed, "Host Close left Threshold timer active.");
        evidence.Add("PASS: Threshold lifetime; all five Tool callbacks/exception ordering; explicit Apply and Close");
        evidence.Add("Rendered DPI: " + VisualTreeHelper.GetDpi(view).PixelsPerInchX);
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".contract.txt"), evidence);
        return (760, 700, elapsed.Elapsed.TotalMilliseconds);
    }

    private static string Describe(DependencyObject root) => string.Join(";", Descendants<Border>(root)
        .Where(cell => cell.Child is TextBlock)
        .Select(cell => ((TextBlock)cell.Child).Text + "/" + cell.Background + "/" + cell.BorderBrush + "/" + cell.BorderThickness));

    private static object Get(object owner, string name) => owner.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(owner)!;
    private static void Set(object owner, string name, double value)
    {
        PropertyInfo property = owner.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
        property.SetValue(owner, Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture));
    }
    private static void Click(DependencyObject root, string id)
    {
        Button button = Find<Button>(root, id);
        Require(button.IsEnabled, "Disabled button: " + id);
        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
    }
    private static T Find<T>(DependencyObject root, string id) where T : FrameworkElement => Descendants<T>(root)
        .FirstOrDefault(control => control.Name == id || AutomationProperties.GetAutomationId(control) == id) ?? throw new InvalidOperationException("Missing control: " + id);
    private static IEnumerable<T> Descendants<T>(DependencyObject root) where T : DependencyObject
    {
        if (root is T element) yield return element;
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
            foreach (T child in Descendants<T>(VisualTreeHelper.GetChild(root, index))) yield return child;
    }
    private static void WaitUntil(Func<bool> condition, Action<int> pump)
    {
        Stopwatch elapsed = Stopwatch.StartNew();
        while (!condition() && elapsed.ElapsedMilliseconds < 2000) { pump(1); Thread.Sleep(10); }
        Require(condition(), "Timer did not advance.");
    }
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
