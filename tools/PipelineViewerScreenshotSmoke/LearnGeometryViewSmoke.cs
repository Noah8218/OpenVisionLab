using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using static OpenVisionLab.DEFINE;

internal static class LearnGeometryViewSmoke
{
    internal static (int Width, int Height, double Milliseconds) Capture(string outputPath,
        Action<int> pump, Action<FrameworkElement, string, int, int> render)
    {
        DateTime started = DateTime.UtcNow;
        string directory = Path.GetDirectoryName(outputPath)!;
        List<string> evidence = new();
        List<VISION_MENU> actions = new();
        GeometryLearnView view = new();
        Window host = new() { Title = "Geometry Learn View boundary", Width = 760, Height = 850, Content = view };
        view.SelectTopic(15);
        view.SetOpenRelatedToolAction(actions.Add);
        Require(actions.Count == 0, "Installing the Geometry Tool callback executed an action.");
        host.Show();
        try
        {
            pump(6);
            Require(view.IsLoaded, "Standalone Geometry View was not loaded.");
            view.GeometryAngleForTest = 25;
            view.GeometryScaleForTest = 80;
            pump(3);
            Require(view.GeometryAngleForTest == 25 && view.GeometryScaleForTest == 80, "Standalone Geometry sliders changed.");
            Require(view.GeometryFormulaTextForTest.Contains("OutputSize~614x461", StringComparison.Ordinal), "Standalone Geometry formula changed.");

            view.ResetGeometryAnimationForTest();
            for (int stage = 0; stage <= 3; stage++)
            {
                if (stage > 0) view.AdvanceGeometryAnimationForTest();
                Require(view.GeometryAnimationStepForTest == stage, "Standalone Geometry stage progression changed.");
                Require(view.GeometryAnimationStatusTextForTest.StartsWith(stage + " / 3 - ", StringComparison.Ordinal), "Standalone Geometry status stage changed.");
                Require(view.GeometryRenderedAngleForTest == (stage >= 1 ? 25D : 0D), "Standalone Geometry rotation rendering changed.");
                Require(view.GeometryRenderedScaleForTest == (stage >= 2 ? 0.8D : 1D), "Standalone Geometry scale rendering changed.");
                evidence.Add("stage " + stage + ": " + view.GeometryAnimationStatusTextForTest);
            }

            view.AdvanceGeometryAnimationForTest();
            Require(view.GeometryAnimationStepForTest == 1, "Standalone completed Geometry animation did not restart at Rotate.");
            view.ResetGeometryAnimationForTest();
            for (int cycle = 0; cycle < 2; cycle++)
            {
                view.AdvanceGeometryAnimationForTest();
                view.ToggleGeometryAnimationForTest();
                host.Content = null;
                pump(3);
                Require(!view.IsLoaded, "Standalone Geometry unload was not exercised.");
                Thread.Sleep(700);
                pump(2);
                Require(view.GeometryAnimationStepForTest == 1, "Standalone Geometry advanced while detached.");
                host.Content = view;
                pump(3);
                Thread.Sleep(700);
                pump(2);
                Require(view.IsLoaded && view.GeometryAnimationStepForTest == 1, "Standalone Geometry rehosting changed the stage.");
                Require(Equals(Find<Button>(view, "OpenVisionLearnGeometryPlayButton").Content, "Play"), "Standalone Geometry rehosting enabled autoplay.");
                view.ResetGeometryAnimationForTest();
                view.ToggleGeometryAnimationForTest();
                WaitUntil(() => view.GeometryAnimationStepForTest > 0, pump);
                view.ToggleGeometryAnimationForTest();
                int paused = view.GeometryAnimationStepForTest;
                Thread.Sleep(700);
                pump(2);
                Require(view.GeometryAnimationStepForTest == paused, "Standalone Geometry rehosting duplicated a timer callback.");
                view.ResetGeometryAnimationForTest();
            }
            evidence.Add("PASS: two unload/reload cycles, stage retention, no autoplay, first timer tick exactly once");

            view.ResetGeometryAnimationForTest();
            view.ToggleGeometryAnimationForTest();
            view.SelectTopic(13);
            Require(view.Visibility == Visibility.Collapsed && view.IsLoaded, "Geometry topic hiding unexpectedly unloaded the View.");
            WaitUntil(() => view.GeometryAnimationStepForTest > 0, pump);
            view.SelectTopic(15);
            view.ToggleGeometryAnimationForTest();
            Require(view.GeometryAnimationStepForTest == 1, "Hidden Geometry topic did not retain playback.");
            view.ResetGeometryAnimationForTest();

            Button rotateButton = Find<Button>(view, "OpenVisionLearnGeometryOpenToolButton");
            Button affineButton = Find<Button>(view, "OpenVisionLearnGeometryOpenAffineToolButton");
            Require(rotateButton.IsEnabled && affineButton.IsEnabled, "Geometry Tool callback did not enable both buttons.");
            int actionCount = actions.Count;
            Click(view, "OpenVisionLearnGeometryOpenToolButton");
            Require(actions.Count == actionCount + 1 && actions[^1] == VISION_MENU.RotateAndScale
                && view.GeometryToolLocationTitleForTest.Contains("Input/Output Layer", StringComparison.Ordinal), "RotateScale callback or hint changed.");
            Click(view, "OpenVisionLearnGeometryOpenAffineToolButton");
            Require(actions.Count == actionCount + 2 && actions[^1] == VISION_MENU.AffineTransform
                && view.GeometryToolLocationDetailForTest.Contains("destination triangle", StringComparison.Ordinal), "Affine callback or hint changed.");
            string previousTitle = view.GeometryToolLocationTitleForTest;
            string previousDetail = view.GeometryToolLocationDetailForTest;
            InvalidOperationException expected = new("geometry-view-probe");
            view.SetOpenRelatedToolAction(_ => throw expected);
            try { Click(view, "OpenVisionLearnGeometryOpenToolButton"); throw new InvalidOperationException("Geometry callback exception was swallowed."); }
            catch (InvalidOperationException exception) when (ReferenceEquals(exception, expected)) { }
            Require(view.GeometryToolLocationTitleForTest == previousTitle && view.GeometryToolLocationDetailForTest == previousDetail, "Failed Geometry callback changed its hint.");
            view.SetOpenRelatedToolAction(null!);
            Require(!rotateButton.IsEnabled && !affineButton.IsEnabled, "Null Geometry callback left a Tool enabled.");
            view.SetOpenRelatedToolAction(actions.Add);
            Require(actions.Count == actionCount + 2, "Reinstalling the Geometry callback executed an action.");

            Button playButton = Find<Button>(view, "OpenVisionLearnGeometryPlayButton");
            playButton.BringIntoView();
            pump(3);
            host.Activate();
            Require(playButton.Focus() && playButton.IsKeyboardFocused, "Geometry Play button rejected keyboard focus.");
            Find<FrameworkElement>(view, "OpenVisionLearnGeometryToolLocationPanel").BringIntoView();
            pump(3);
            render(host, outputPath, 760, 850);
            DpiScale dpi = VisualTreeHelper.GetDpi(view);
            evidence.Add("Rendered view DPI: " + dpi.PixelsPerInchX.ToString(CultureInfo.InvariantCulture) + " x " + dpi.PixelsPerInchY.ToString(CultureInfo.InvariantCulture));
            evidence.Add("PASS: RotateScale/Affine callbacks, thrown callback preserves hints, null disables both buttons, hidden playback and keyboard focus");
            evidence.Add("Not run: actual pointer hover/down/pressed, physical keyboard navigation, other themes/layouts or additional DPI settings.");
            view.ResetGeometryAnimationForTest();
            view.ToggleGeometryAnimationForTest();
        }
        finally { host.Close(); }

        pump(3);
        Require(!view.IsLoaded, "Host Close did not unload the Geometry View.");
        Thread.Sleep(700);
        pump(2);
        Require(view.GeometryAnimationStepForTest == 0, "Host Close left the Geometry timer active.");
        Require(actions.Count == 2, "Host Close executed an external Geometry action.");
        evidence.Add("PASS: host Close stops the Geometry timer without executing an external action");
        File.WriteAllLines(Path.ChangeExtension(outputPath, ".contract.txt"), evidence);
        return (760, 850, (DateTime.UtcNow - started).TotalMilliseconds);
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
