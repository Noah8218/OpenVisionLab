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

internal static class LearnBinaryLineSmoke
{
    private static readonly (string Name, int Topic, int Steps, string Setting, double[] Values)[] topics =
    {
        ("Morphology", 4, 25, "MorphologyModeIndexForTest", new double[] { 0, 1, 2, 3 }),
        ("Blob", 5, 3, "BlobMinAreaForTest", new double[] { 1, 3, 6 }),
        ("Contour", 6, 3, "ContourDrawModeIndexForTest", new double[] { 0, 1, 2 }),
        ("EdgeLine", 7, 3, "EdgeThresholdForTest", new double[] { 10, 80, 150 }),
        ("LineDistance", 8, 3, "LineDistanceRangeMaxForTest", new double[] { 0, 0.5, 2 })
    };

    internal static (int Width, int Height, double Milliseconds) Capture(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        Stopwatch elapsed = Stopwatch.StartNew();
        List<string> states = new();
        string directory = Path.GetDirectoryName(outputPath)!;
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
                Record("initial");
                Click(window, topic.Name, "Reset");
                Require(Stage() == 0, topic.Name + " Reset did not select stage 0.");
                for (int stage = 0; stage <= topic.Steps; stage++)
                {
                    if (stage > 0)
                        Click(window, topic.Name, "Step");
                    Require(Stage() == stage, topic.Name + " stage sequence changed.");
                    Record("step-" + stage);
                    if (stage == 0 || stage == 1 || stage == topic.Steps / 2 || stage == topic.Steps)
                    {
                        pump(2);
                        render(window, Path.Combine(directory, topic.Name + "-step-" + stage + ".png"), 1040, 700);
                    }
                }
                foreach (double value in topic.Values)
                {
                    PropertyInfo property = typeof(OpenVisionLearnWindow).GetProperty(topic.Setting)!;
                    property.SetValue(window, Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture));
                    pump(2);
                    Record("setting-" + value.ToString(CultureInfo.InvariantCulture));
                    render(window, Path.Combine(directory, topic.Name + "-setting-" + value.ToString(CultureInfo.InvariantCulture) + ".png"), 1040, 700);
                }
                Click(window, topic.Name, "Reset");
                Click(window, topic.Name, "Step");
                window.SelectTopic((OpenVisionLearnTopicIndex)13);
                window.SelectTopic((OpenVisionLearnTopicIndex)topic.Topic);
                pump(3);
                Require(Stage() == (topic.Name == "Morphology" ? 25 : 1), topic.Name + " topic-refresh rule changed.");
                Record("topic-return");
                Click(window, topic.Name, "Reset");
                Click(window, topic.Name, "Play");
                WaitUntil(() => Stage() > 0, pump);
                Click(window, topic.Name, "Play");
                int paused = Stage();
                Thread.Sleep(500);
                pump(2);
                Require(Stage() == paused, topic.Name + " Pause left timer active.");
                Click(window, topic.Name, "Reset");
                Click(window, topic.Name, "Play");
                window.SelectTopic((OpenVisionLearnTopicIndex)13);
                WaitUntil(() => Stage() > 0, pump);
                Require(Stage() > 0, topic.Name + " hidden-topic timer behavior changed.");
                window.SelectTopic((OpenVisionLearnTopicIndex)topic.Topic);
                Click(window, topic.Name, "Reset");
                Require(actions == 0, topic.Name + " controls invoked an external action.");
                render(window, outputPath, 1040, 700);
                Click(window, topic.Name, "Play");

                int Stage() => (int)Get(window, topic.Name + "AnimationStepForTest");
                void Record(string label)
                {
                    string prefix = char.ToLowerInvariant(topic.Name[0]) + topic.Name.Substring(1);
                    UniformGrid grid = Find<UniformGrid>(window, prefix + "OutputGrid");
                    string cells = string.Join(";", grid.Children.OfType<Border>().Select(cell =>
                        ((TextBlock)cell.Child).Text + "/" + cell.Background + "/" + cell.BorderBrush + "/" + cell.BorderThickness));
                    states.Add(topic.Name + "|" + label + "|" + Stage() + "|" + Get(window, topic.Setting)
                        + "|" + Get(window, topic.Name + "FormulaTextForTest")
                        + "|" + Get(window, topic.Name + "AnimationStatusTextForTest") + "|" + cells);
                }
            }
            finally
            {
                window.Close();
            }
            Thread.Sleep(500);
            pump(2);
            Require((int)Get(window, topic.Name + "AnimationStepForTest") == 0, topic.Name + " Close left timer active.");
        }
        File.WriteAllLines(Path.Combine(directory, "binary-line-states.txt"), states);
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".contract.txt"), new[]
        {
            "PASS: all five topic stage sequences, settings, topic refresh, Play/Pause, hidden playback and Close",
            "PASS: zero external Tool/sample/apply actions during lesson interaction",
            "Golden comparison data: binary-line-states.txt"
        });
        return (1040, 700, elapsed.Elapsed.TotalMilliseconds);
    }

    internal static (int Width, int Height, double Milliseconds) CaptureViews(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        Stopwatch elapsed = Stopwatch.StartNew();
        List<string> evidence = new();
        foreach (var topic in topics)
        {
            UserControl view = topic.Topic <= 6 ? new BinaryLearnView() : new LineLearnView();
            if (view is BinaryLearnView binary) binary.SelectTopic(topic.Topic);
            else ((LineLearnView)view).SelectTopic(topic.Topic);
            Window host = new() { Title = topic.Name + " Learn boundary", Width = 760, Height = 700, Content = view };
            host.Show();
            try
            {
                pump(8);
                Require(Stage() == topic.Steps, topic.Name + " standalone initial stage changed.");
                Click(view, topic.Name, "Reset");
                Click(view, topic.Name, "Play");
                PropertyInfo setting = view.GetType().GetProperty(topic.Setting, BindingFlags.Instance | BindingFlags.NonPublic)!;
                setting.SetValue(view, Convert.ChangeType(topic.Values.Last(), setting.PropertyType, CultureInfo.InvariantCulture));
                pump(1);
                if (topic.Name == "Blob")
                {
                    Require(Stage() == 0 && Equals(PlayButton().Content, "Pause"), "MIN_AREA changed stage or stopped playback.");
                    WaitUntil(() => Stage() > 0, pump);
                }
                else
                {
                    Require(Stage() == topic.Steps && Equals(PlayButton().Content, "Play"), topic.Name + " setting did not complete and pause.");
                    Thread.Sleep(500);
                    pump(2);
                    Require(Stage() == topic.Steps, topic.Name + " setting left the timer active.");
                }
                for (int cycle = 0; cycle < 2; cycle++)
                {
                    Click(view, topic.Name, "Reset");
                    Click(view, topic.Name, "Step");
                    Click(view, topic.Name, "Play");
                    host.Content = null;
                    pump(3);
                    Require(!view.IsLoaded, "Unload was not exercised.");
                    Thread.Sleep(500);
                    pump(3);
                    Require(Stage() == 1, topic.Name + " unloaded timer still advanced.");
                    host.Content = view;
                    pump(3);
                    Thread.Sleep(500);
                    pump(3);
                    Require(view.IsLoaded && Stage() == 1 && Equals(PlayButton().Content, "Play")
                        && Convert.ToDouble(Get(view, topic.Setting)) == topic.Values.Last(), topic.Name + " reload lost state or enabled autoplay.");
                    Click(view, topic.Name, "Reset");
                    Click(view, topic.Name, "Play");
                    WaitUntil(() => Stage() > 0, pump);
                    Require(Stage() == 1, topic.Name + " reload duplicated a Tick subscription.");
                    Click(view, topic.Name, "Reset");
                }

                (string Button, VISION_MENU Menu, string TitleFragment)[] links = topic.Name switch
                {
                    "Morphology" => new[] { ("btnMorphologyOpenTool", VISION_MENU.Morphology, "Kernel Width/Height") },
                    "Blob" => new[] { ("btnBlobOpenTool", VISION_MENU.Blob, "Min area / Max area") },
                    "Contour" => new[] { ("btnContourOpenTool", VISION_MENU.Contour, "Retrieval mode") },
                    "EdgeLine" => new[] { ("btnEdgeDetectionOpenTool", VISION_MENU.EdgeDetection, "Edge Detection"), ("btnEdgeLineOpenLineTool", VISION_MENU.Line, "Purpose, Line A/B") },
                    _ => new[] { ("btnLineDistanceOpenTool", VISION_MENU.Line, "Purpose > Measure") }
                };
                foreach (var link in links)
                {
                    Button button = Find<Button>(view, link.Button);
                    Require(!button.IsEnabled, "Tool link must start disabled without callback.");
                    string titleBefore = (string)Get(view, topic.Name + "ToolLocationTitleForTest");
                    InvalidOperationException expectedFailure = new("Expected Tool callback failure");
                    SetAction(_ => throw expectedFailure);
                    try
                    {
                        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        throw new InvalidOperationException("Tool callback failure was swallowed.");
                    }
                    catch (InvalidOperationException error) when (ReferenceEquals(error, expectedFailure)) { }
                    Require((string)Get(view, topic.Name + "ToolLocationTitleForTest") == titleBefore, "Tool failure changed the hint before callback completion.");
                    int actions = 0;
                    SetAction(menu => { Require(menu == link.Menu, "Tool menu changed."); actions++; });
                    Require(button.IsEnabled && actions == 0, "Installing Tool callback ran an action.");
                    button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    Require(actions == 1 && ((string)Get(view, topic.Name + "ToolLocationTitleForTest")).Contains(link.TitleFragment, StringComparison.Ordinal), "Explicit Tool link or location hint changed.");
                    SetAction(null);
                    Require(!button.IsEnabled && actions == 1, "Removing Tool callback ran an action or left the button enabled.");
                }
                pump(2);
                render(host, Path.Combine(Path.GetDirectoryName(outputPath)!, topic.Name + "-standalone.png"), 760, 700);
                render(host, outputPath, 760, 700);
                evidence.Add("PASS: " + topic.Name + " standalone initialization, setting/play rule, 2 unload/reload cycles, retained state, no autoplay/duplicate Tick, Tool links/exception ordering");
                evidence.Add("Rendered DPI: " + VisualTreeHelper.GetDpi(view).PixelsPerInchX);
                Click(view, topic.Name, "Play");

                Button PlayButton() => Find<Button>(view, "OpenVisionLearn" + topic.Name + "PlayButton");
                int Stage() => (int)Get(view, topic.Name + "AnimationStepForTest");
                void SetAction(Action<VISION_MENU>? action)
                {
                    if (view is BinaryLearnView binaryView) binaryView.SetOpenRelatedToolAction(action!);
                    else ((LineLearnView)view).SetOpenRelatedToolAction(action!);
                }
            }
            finally
            {
                host.Close();
            }
            Thread.Sleep(500);
            pump(2);
            Require((int)Get(view, topic.Name + "AnimationStepForTest") == 0, topic.Name + " standalone Close left the timer active.");
        }
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".contract.txt"), evidence);
        return (760, 700, elapsed.Elapsed.TotalMilliseconds);
    }

    private static object Get(object owner, string property) => owner.GetType().GetProperty(property,
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(owner)!;

    private static void Click(DependencyObject root, string topic, string action)
    {
        Button button = Find<Button>(root, "OpenVisionLearn" + topic + action + "Button");
        Require(button.IsEnabled, "Lesson button is disabled.");
        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
    }

    private static T Find<T>(DependencyObject root, string id) where T : FrameworkElement
    {
        return FindOrDefault<T>(root, id) ?? throw new InvalidOperationException("Missing control: " + id);
    }

    private static T? FindOrDefault<T>(DependencyObject root, string id) where T : FrameworkElement
    {
        if (root is T element && (element.Name == id || AutomationProperties.GetAutomationId(element) == id))
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
        while (!condition() && timer.ElapsedMilliseconds < 2000)
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
