using OpenVisionLab;
using System;
using System.Collections.Generic;
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

internal static class LearnGeometrySmoke
{
    internal static (int Width, int Height, double Milliseconds) Capture(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        DateTime started = DateTime.UtcNow;
        string directory = Path.GetDirectoryName(outputPath)!;
        List<string> records = new();
        List<VISION_MENU> actions = new();
        OpenVisionLearnWindow window = new(127, 255, false, 15) { Width = 1040, Height = 900 };
        window.SetOpenRelatedToolAction(actions.Add);
        window.Show();
        try
        {
            pump(10);
            window.GeometryAngleForTest = 25;
            window.GeometryScaleForTest = 80;
            pump(5);
            Require(window.SelectedTopicIndexForTest == 15, "Geometry topic was not selected.");
            Require(window.GeometryAngleForTest == 25 && window.GeometryScaleForTest == 80, "Geometry slider values changed.");
            Require(window.GeometryFormulaTextForTest.Contains("OutputSize~614x461", StringComparison.Ordinal), "Geometry formula changed.");
            Record("configured");
            window.ResetGeometryAnimationForTest();
            for (int stage = 0; stage <= 3; stage++)
            {
                if (stage > 0) window.AdvanceGeometryAnimationForTest();
                Require(window.GeometryAnimationStepForTest == stage, "Geometry stage progression changed.");
                Require(window.GeometryAnimationStatusTextForTest.StartsWith(stage + " / 3 - ", StringComparison.Ordinal), "Geometry status stage changed.");
                Record("stage-" + stage);
                render(window, Path.Combine(directory, "Geometry-stage-" + stage + ".png"), 1040, 900);
            }
            window.AdvanceGeometryAnimationForTest();
            Require(window.GeometryAnimationStepForTest == 1, "Completed Geometry animation did not restart at Rotate.");
            window.SelectTopic((OpenVisionLearnTopicIndex)13);
            window.SelectTopic((OpenVisionLearnTopicIndex)15);
            Require(window.GeometryAnimationStepForTest == 1, "Topic return changed the Geometry stage.");
            Record("topic-return");

            window.ResetGeometryAnimationForTest();
            window.ToggleGeometryAnimationForTest();
            WaitUntil(() => window.GeometryAnimationStepForTest > 0, pump);
            window.ToggleGeometryAnimationForTest();
            int paused = window.GeometryAnimationStepForTest;
            Thread.Sleep(700);
            pump(2);
            Require(window.GeometryAnimationStepForTest == paused, "Geometry pause left the timer active.");

            string oldTitle = window.GeometryToolLocationTitleForTest;
            string oldDetail = window.GeometryToolLocationDetailForTest;
            window.SetOpenRelatedToolAction(_ => throw new InvalidOperationException("geometry-probe"));
            try { Click(window, "OpenVisionLearnGeometryOpenToolButton"); throw new InvalidOperationException("Geometry callback exception was swallowed."); }
            catch (InvalidOperationException error) when (error.Message == "geometry-probe") { }
            Require(window.GeometryToolLocationTitleForTest == oldTitle && window.GeometryToolLocationDetailForTest == oldDetail, "Failed Geometry Tool callback changed its hint.");
            window.SetOpenRelatedToolAction(actions.Add);
            Click(window, "OpenVisionLearnGeometryOpenToolButton");
            Require(actions.Last() == VISION_MENU.RotateAndScale && window.GeometryToolLocationTitleForTest.Contains("Input/Output Layer", StringComparison.Ordinal), "RotateScale callback or hint changed.");
            Click(window, "OpenVisionLearnGeometryOpenAffineToolButton");
            Require(actions.Last() == VISION_MENU.AffineTransform && window.GeometryToolLocationDetailForTest.Contains("destination triangle", StringComparison.Ordinal), "Affine callback or hint changed.");
            Record("tools");
            window.ResetGeometryAnimationForTest();
            window.ToggleGeometryAnimationForTest();
            window.SelectTopic((OpenVisionLearnTopicIndex)13);
            WaitUntil(() => window.GeometryAnimationStepForTest > 0, pump);
            window.SelectTopic((OpenVisionLearnTopicIndex)15);
            Require(window.GeometryAnimationStepForTest > 0, "Hidden Geometry topic did not retain playback.");
            Record("hidden-playback");
            Find<FrameworkElement>(window, "OpenVisionLearnGeometryToolLocationPanel").BringIntoView();
            pump(3);
            render(window, outputPath, 1040, 900);
            File.WriteAllLines(Path.ChangeExtension(outputPath, ".states.txt"), records);
        }
        finally { window.Close(); }

        Thread.Sleep(700);
        pump(2);
        Require(window.GeometryAnimationStepForTest > 0, "Window close unexpectedly changed the compatibility stage.");
        File.WriteAllText(Path.ChangeExtension(outputPath, ".contract.txt"), "PASS: Geometry stage, sliders, formula, reset/restart, topic return, pause, hidden playback, Tool callbacks/errors, explicit Preview/Run text and close.");
        return (1040, 900, (DateTime.UtcNow - started).TotalMilliseconds);

        void Record(string label) => records.Add(label + "|" + window.GeometryAnimationStepForTest + "|"
            + window.GeometryAngleForTest.ToString(CultureInfo.InvariantCulture) + "|"
            + window.GeometryScaleForTest.ToString(CultureInfo.InvariantCulture) + "|"
            + window.GeometryFormulaTextForTest + "|" + window.GeometryAnimationStatusTextForTest + "|"
            + window.GeometryRenderedAngleForTest.ToString(CultureInfo.InvariantCulture) + "|"
            + window.GeometryRenderedScaleForTest.ToString(CultureInfo.InvariantCulture) + "|"
            + window.GeometryToolLocationTitleForTest + "|" + window.GeometryToolLocationDetailForTest);
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
        DateTime end = DateTime.UtcNow.AddSeconds(2);
        while (!condition() && DateTime.UtcNow < end) { pump(1); Thread.Sleep(10); }
        Require(condition(), "Geometry timer did not advance.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
