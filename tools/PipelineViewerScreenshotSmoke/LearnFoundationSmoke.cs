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
using System.Windows.Shapes;
using static OpenVisionLab.DEFINE;

internal static class LearnFoundationSmoke
{
    internal static (int Width, int Height, double Milliseconds) Capture(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        Stopwatch elapsed = Stopwatch.StartNew();
        string directory = System.IO.Path.GetDirectoryName(outputPath)!;
        List<string> states = new();
        OpenVisionLearnWindow window = new(127, 255, false, 0) { Width = 1040, Height = 980 };
        List<VISION_MENU> tools = new();
        int unexpectedActions = 0;
        window.SetOpenRelatedToolAction(tools.Add);
        window.SetOpenPracticeSamplesAction(_ => unexpectedActions++);
        window.ApplyThresholdRequested += (_, _) => unexpectedActions++;
        window.Show();
        try
        {
            pump(8);
            Thread.Sleep(650);
            pump(3);
            Border panel = Find<Border>(window, "pixelTopicPanel");
            Require(window.FoundationAnimationStepForTest == 5 && window.MatChannelAnimationStepForTest == 4, "Initial frame changed.");
            Record("initial");
            foreach (var lesson in new[] { (Name: "Foundation", Steps: 5, Anchor: "OpenVisionLearnFoundationGeometryPanel"),
                (Name: "MatChannel", Steps: 4, Anchor: "OpenVisionLearnMatChannelPanel") })
            {
                Find<FrameworkElement>(window, lesson.Anchor).BringIntoView();
                pump(3);
                Click(window, "OpenVisionLearn" + lesson.Name + "ResetButton");
                for (int stage = 0; stage <= lesson.Steps; stage++)
                {
                    if (stage > 0) Click(window, "OpenVisionLearn" + lesson.Name + "StepButton");
                    Require(Stage() == stage, lesson.Name + " stage progression changed.");
                    Record(lesson.Name + "-" + stage);
                    pump(2);
                    render(window, System.IO.Path.Combine(directory, lesson.Name + "-step-" + stage + ".png"), 1040, 980);
                }
                Click(window, "OpenVisionLearn" + lesson.Name + "StepButton");
                Require(Stage() == 1, "Completed lesson did not restart at step one.");
                window.SelectTopic((OpenVisionLearnTopicIndex)13);
                window.SelectTopic((OpenVisionLearnTopicIndex)0);
                Require(Stage() == 1, "Topic return reset the stage.");
                Record(lesson.Name + "-return");
                Click(window, "OpenVisionLearn" + lesson.Name + "ResetButton");
                Click(window, "OpenVisionLearn" + lesson.Name + "PlayButton");
                WaitUntil(() => Stage() > 0, pump);
                Click(window, "OpenVisionLearn" + lesson.Name + "PlayButton");
                int paused = Stage();
                Thread.Sleep(700);
                pump(2);
                Require(Stage() == paused, "Pause left a timer active.");
                Click(window, "OpenVisionLearn" + lesson.Name + "ResetButton");
                Click(window, "OpenVisionLearn" + lesson.Name + "PlayButton");
                window.SelectTopic((OpenVisionLearnTopicIndex)13);
                WaitUntil(() => Stage() > 0, pump);
                window.SelectTopic((OpenVisionLearnTopicIndex)0);
                Click(window, "OpenVisionLearn" + lesson.Name + "ResetButton");
                int Stage() => (int)Get(window, lesson.Name + "AnimationStepForTest");
            }
            Require(tools.Count == 0 && unexpectedActions == 0, "Lesson navigation/playback invoked an external action.");
            Require(window.CanOpenRelatedToolsForTest, "Explicit Tool callback did not enable the buttons.");
            foreach (var action in new[] { (Id: "Roi", Menu: VISION_MENU.Blob), (Id: "Kernel", Menu: VISION_MENU.Filter), (Id: "OutputSize", Menu: VISION_MENU.RotateAndScale) })
            {
                int count = tools.Count;
                Click(window, "OpenVisionLearnFoundationOpen" + action.Id + "ToolButton");
                Require(tools.Count == count + 1 && tools[^1] == action.Menu, "Tool callback was lost or duplicated.");
                Record("tool-" + action.Id);
            }
            string previousHint = window.FoundationToolLocationTitleForTest;
            window.SetOpenRelatedToolAction(_ => throw new InvalidOperationException("foundation-probe"));
            try { Click(window, "OpenVisionLearnFoundationOpenRoiToolButton"); throw new Exception("Callback exception was swallowed."); }
            catch (InvalidOperationException exception) when (exception.Message == "foundation-probe") { }
            Require(window.FoundationToolLocationTitleForTest == previousHint, "Failed callback changed the hint.");
            window.SetOpenRelatedToolAction(null!);
            Require(!window.CanOpenRelatedToolsForTest, "Null callback left buttons enabled.");
            window.SetOpenRelatedToolAction(tools.Add);
            window.SelectTopic((OpenVisionLearnTopicIndex)2);
            pump(3);
            Click(window, "OpenVisionLearnThresholdOpenToolButton");
            Require(tools[^1] == VISION_MENU.Threshold && window.FoundationToolLocationTitleForTest.Contains("Parameter panel", StringComparison.Ordinal), "Threshold-to-Foundation hint relay changed.");
            window.SelectTopic((OpenVisionLearnTopicIndex)0);
            Record("threshold-hint");
            window.BringFoundationToolLocationIntoViewForTest();
            pump(3);
            render(window, outputPath, 1040, 980);
            states.Add("DPI=" + VisualTreeHelper.GetDpi(window).PixelsPerInchX.ToString(CultureInfo.InvariantCulture));
            File.WriteAllLines(System.IO.Path.ChangeExtension(outputPath, ".states.txt"), states);
            window.ResetFoundationAnimationForTest();
            window.ResetMatChannelAnimationForTest();
            window.ToggleFoundationAnimationForTest();
            window.ToggleMatChannelAnimationForTest();

            void Record(string label) => states.Add(label + "|" + window.FoundationAnimationStepForTest + "|" + window.MatChannelAnimationStepForTest
                + "|" + window.FoundationSelectedCellCountForTest + "|" + Describe(panel));
        }
        finally { window.Close(); }
        Thread.Sleep(700);
        pump(2);
        Require(window.FoundationAnimationStepForTest == 0 && window.MatChannelAnimationStepForTest == 0, "Close left a lesson timer active.");
        File.WriteAllText(System.IO.Path.ChangeExtension(outputPath, ".contract.txt"), "PASS: both stage sequences, ROI/frame state, topic return, play/pause, hidden playback, Tool callbacks/errors/disable, Threshold hint relay, no implicit actions and Close.");
        return (1040, 980, elapsed.Elapsed.TotalMilliseconds);
    }

    internal static (int Width, int Height, double Milliseconds) CaptureView(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        Stopwatch elapsed = Stopwatch.StartNew();
        string directory = System.IO.Path.GetDirectoryName(outputPath)!;
        List<string> evidence = new();
        List<VISION_MENU> actions = new();
        FoundationLearnView view = new();
        Window host = new() { Title = "Foundation Learn View boundary", Width = 760, Height = 850, Content = view };
        view.SelectTopic(0);
        view.SetOpenRelatedToolAction(actions.Add);
        Require(actions.Count == 0, "Installing the Tool callback executed an action.");
        host.Show();
        try
        {
            pump(6);
            Require(view.IsLoaded, "Standalone View was not loaded.");
            Require(view.FoundationAnimationStepForTest == 5 && view.MatChannelAnimationStepForTest == 4, "Standalone initial stages changed.");
            foreach (var lesson in new[] { (Name: "Foundation", Steps: 5, Anchor: "OpenVisionLearnFoundationGeometryPanel"),
                (Name: "MatChannel", Steps: 4, Anchor: "OpenVisionLearnMatChannelPanel") })
            {
                string prefix = "OpenVisionLearn" + lesson.Name;
                Find<FrameworkElement>(view, lesson.Anchor).BringIntoView();
                pump(3);
                Click(view, prefix + "ResetButton");
                for (int stage = 0; stage <= lesson.Steps; stage++)
                {
                    if (stage > 0) Click(view, prefix + "StepButton");
                    Require(Stage() == stage, lesson.Name + " standalone stage progression changed.");
                    TextBlock status = Find<TextBlock>(view, prefix + "AnimationStatus");
                    Require(status.Text.StartsWith(stage + " / " + lesson.Steps + " - ", StringComparison.Ordinal), "Rendered status disagrees with the stage.");
                    if (lesson.Name == "Foundation")
                    {
                        Require(view.FoundationSelectedCellCountForTest == (stage is 3 or 4 ? 12 : 0), "Selected ROI count changed.");
                        Require(view.IsFoundationPointVisibleForTest == (stage >= 1 && stage <= 4), "Point visibility changed.");
                        Require(view.IsFoundationRectVisibleForTest == (stage >= 2 && stage <= 4), "Rect visibility changed.");
                        Require(view.IsFoundationRotatedRectVisibleForTest == (stage == 5)
                            && view.IsFoundationRotatedBoundsVisibleForTest == (stage == 5)
                            && view.IsFoundationRotatedCenterVisibleForTest == (stage == 5)
                            && view.FoundationRotatedRectAngleForTest == 25D, "RotatedRect frame changed.");
                    }
                    else
                    {
                        Require(view.MatChannelSplitOpacityForTest == (stage >= 2 ? 1D : 0.28D)
                            && view.MatChannelGrayOpacityForTest == (stage >= 3 ? 1D : 0.28D)
                            && view.MatChannelTypeGuideOpacityForTest == (stage >= 4 ? 1D : 0.28D), "Rendered channel/type opacity changed.");
                        Require(view.MatChannelBgrShapeTextForTest == "행 x 열 x 3" && view.MatChannelGrayShapeTextForTest == "행 x 열 x 1", "Rendered Mat shape labels changed.");
                    }
                    evidence.Add(lesson.Name + " stage " + stage + ": " + status.Text);
                }

                for (int cycle = 0; cycle < 2; cycle++)
                {
                    Click(view, prefix + "ResetButton");
                    Click(view, prefix + "StepButton");
                    Click(view, prefix + "PlayButton");
                    host.Content = null;
                    pump(3);
                    Require(!view.IsLoaded, "Standalone unload was not exercised.");
                    Thread.Sleep(700);
                    pump(2);
                    Require(Stage() == 1, lesson.Name + " advanced while detached.");
                    host.Content = view;
                    pump(3);
                    Thread.Sleep(700);
                    pump(2);
                    Require(view.IsLoaded && Stage() == 1 && Equals(Find<Button>(view, prefix + "PlayButton").Content, "자동 재생"), "Rehosting changed the stage or enabled autoplay.");
                    Click(view, prefix + "ResetButton");
                    Click(view, prefix + "PlayButton");
                    WaitUntil(() => Stage() > 0, pump);
                    Click(view, prefix + "PlayButton");
                    Require(Stage() == 1, lesson.Name + " rehosting duplicated a timer callback.");
                    Click(view, prefix + "ResetButton");
                }

                Click(view, prefix + "PlayButton");
                view.SelectTopic(13);
                Require(view.Visibility == Visibility.Collapsed && view.IsLoaded, "Topic hiding unexpectedly unloaded the View.");
                WaitUntil(() => Stage() > 0, pump);
                view.SelectTopic(0);
                Click(view, prefix + "PlayButton");
                Require(Stage() == 1, "Hidden topic did not retain its first playing stage.");
                Click(view, prefix + "ResetButton");
                for (int stage = 0; stage < lesson.Steps; stage++) Click(view, prefix + "StepButton");
                Find<FrameworkElement>(view, lesson.Anchor).BringIntoView();
                pump(3);
                render(host, System.IO.Path.Combine(directory, lesson.Name + "-standalone.png"), 760, 850);
                evidence.Add("PASS: " + lesson.Name + " two unload/reload cycles, stage retention, no autoplay, first tick exactly one, hidden-topic playback");
                int Stage() => (int)Get(view, lesson.Name + "AnimationStepForTest");
            }

            Require(actions.Count == 0, "Lesson stages, rehosting or playback executed a Tool.");
            foreach (var link in new[] { (Id: "Roi", Menu: VISION_MENU.Blob, Hint: "열림: Blob"),
                (Id: "Kernel", Menu: VISION_MENU.Filter, Hint: "열림: Filter"),
                (Id: "OutputSize", Menu: VISION_MENU.RotateAndScale, Hint: "열림: Rotate / Scale") })
            {
                Button button = Find<Button>(view, "OpenVisionLearnFoundationOpen" + link.Id + "ToolButton");
                string priorTitle = view.FoundationToolLocationTitleForTest;
                string priorDetail = view.FoundationToolLocationDetailForTest;
                InvalidOperationException expected = new("Expected Foundation Tool failure");
                view.SetOpenRelatedToolAction(_ => throw expected);
                try { Click(view, "OpenVisionLearnFoundationOpen" + link.Id + "ToolButton"); throw new InvalidOperationException("Tool callback exception was swallowed."); }
                catch (InvalidOperationException exception) when (ReferenceEquals(exception, expected)) { }
                Require(view.FoundationToolLocationTitleForTest == priorTitle && view.FoundationToolLocationDetailForTest == priorDetail, "Failed callback changed the Tool hint.");
                int previousCount = actions.Count;
                view.SetOpenRelatedToolAction(actions.Add);
                Require(actions.Count == previousCount, "Callback installation executed a Tool.");
                Click(view, "OpenVisionLearnFoundationOpen" + link.Id + "ToolButton");
                Require(actions.Count == previousCount + 1 && actions[^1] == link.Menu
                    && view.FoundationToolLocationTitleForTest.StartsWith(link.Hint, StringComparison.Ordinal), "Explicit Tool callback or location hint changed.");
                view.SetOpenRelatedToolAction(null!);
                Require(!view.CanOpenRelatedToolsForTest && !button.IsEnabled && actions.Count == previousCount + 1, "Null callback left a Tool enabled or executed an action.");
                foreach (string id in new[] { "Roi", "Kernel", "OutputSize" })
                    Require(!Find<Button>(view, "OpenVisionLearnFoundationOpen" + id + "ToolButton").IsEnabled, "A sibling Tool button remained enabled.");
            }

            string retainedTitle = view.FoundationToolLocationTitleForTest;
            string retainedDetail = view.FoundationToolLocationDetailForTest;
            host.Content = null;
            pump(3);
            host.Content = view;
            pump(3);
            Require(view.FoundationToolLocationTitleForTest == retainedTitle && view.FoundationToolLocationDetailForTest == retainedDetail
                && !view.CanOpenRelatedToolsForTest && actions.Count == 3, "Rehosting changed the Tool hint/disabled state or executed an action.");

            Button focusButton = Find<Button>(view, "OpenVisionLearnFoundationPlayButton");
            focusButton.BringIntoView();
            pump(3);
            host.Activate();
            Require(focusButton.Focus(), "Play button rejected keyboard focus.");
            pump(2);
            Require(focusButton.IsKeyboardFocused, "Keyboard focus did not reach the Play button.");
            render(host, System.IO.Path.Combine(directory, "Foundation-keyboard-focus.png"), 760, 850);
            view.BringFoundationToolLocationIntoViewForTest();
            pump(3);
            render(host, outputPath, 760, 850);
            DpiScale dpi = VisualTreeHelper.GetDpi(view);
            evidence.Add("Rendered view DPI: " + dpi.PixelsPerInchX.ToString(CultureInfo.InvariantCulture) + " x " + dpi.PixelsPerInchY.ToString(CultureInfo.InvariantCulture));
            evidence.Add("PASS: all three Tool callbacks, thrown callback leaves both hint fields unchanged, null disables all buttons, hint survives rehosting, keyboard focus");
            evidence.Add("Not run: actual pointer hover/down/pressed, physical keyboard navigation, other themes/layouts or additional DPI settings.");
            view.ResetFoundationAnimationForTest();
            view.ResetMatChannelAnimationForTest();
            view.ToggleFoundationAnimationForTest();
            view.ToggleMatChannelAnimationForTest();
        }
        finally { host.Close(); }
        pump(3);
        Require(!view.IsLoaded, "Host Close did not unload its View.");
        Thread.Sleep(700);
        pump(2);
        Require(view.FoundationAnimationStepForTest == 0 && view.MatChannelAnimationStepForTest == 0, "Host Close left a Foundation/MatChannel timer active.");
        Require(actions.Count == 3, "Host Close executed an external action.");
        evidence.Add("PASS: host Close stops both playing timers without changing their reset stages or executing external actions");
        File.WriteAllLines(System.IO.Path.ChangeExtension(outputPath, ".contract.txt"), evidence);
        return (760, 850, elapsed.Elapsed.TotalMilliseconds);
    }

    private static string Describe(DependencyObject root) => string.Join(";", Descendants<FrameworkElement>(root).Select(element =>
        element.GetType().Name + ":" + element.Name + ":" + element.Visibility + ":" + element.Opacity.ToString(CultureInfo.InvariantCulture) + ":" + (element switch
        {
            TextBlock text => text.Text + "/" + text.Foreground,
            Border border => border.Background + "/" + border.BorderBrush + "/" + border.BorderThickness,
            Shape shape => shape.Fill + "/" + shape.Stroke + "/" + shape.RenderTransform,
            Button button => button.Content + "/" + button.IsEnabled,
            _ => string.Empty
        })));
    private static object Get(object owner, string name) => owner.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(owner)!;
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
        while (!condition() && elapsed.ElapsedMilliseconds < 2200) { pump(1); Thread.Sleep(10); }
        Require(condition(), "Timer did not advance.");
    }
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
