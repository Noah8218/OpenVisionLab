using OpenVisionLab;
using OpenVisionLab.Core;
using OpenVisionLab.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Bitmap = System.Drawing.Bitmap;
using DrawingRectangle = System.Drawing.Rectangle;
using PropertyGrid = System.Windows.Controls.WpfPropertyGrid.PropertyGrid;

internal static class UiReadabilityContract
{
    internal static int RunSegmentedSelection(string? requestedEvidenceDirectory)
    {
        string directory = Path.GetFullPath(requestedEvidenceDirectory ?? @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev\segmented-selection");
        Require(string.Equals(Path.GetPathRoot(directory), @"D:\", StringComparison.OrdinalIgnoreCase), "Selection evidence must remain on D:.");
        Directory.CreateDirectory(directory);
        _ = Application.Current ?? new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        ResourceDictionary theme = new ResourceDictionary
        {
            Source = new Uri("/OpenVisionLab;component/UI/VisionTest/Wpf/Tooling/Presentation/VisionToolWpfTheme.xaml", UriKind.Relative)
        };
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        foreach (int height in new[] { 30, 46 })
        {
            Check("R-05 selected, unchecked and disabled geometry at height " + height, () =>
            {
                WrapPanel choices = new WrapPanel { Margin = new Thickness(12) };
                Border root = new Border { Background = (Brush)theme["VisionTool.PanelBrush"], Child = choices };
                RadioButton[] buttons = new[] { "Binary", "반전 / Inverse", "Intersection", "Operation" }.Select(caption => new RadioButton
                {
                    Content = caption,
                    Style = (Style)theme["VisionTool.SegmentedRadioButtonStyle"],
                    Height = height,
                    GroupName = "SelectionContract" + height
                }).ToArray();
                foreach (RadioButton button in buttons) choices.Children.Add(button);
                buttons[0].IsChecked = true;
                Layout(root, 560, 100);
                double[] widths = buttons.Select(button => button.ActualWidth).ToArray();
                ScreenshotPngWriter.WriteElementPng(root, Path.Combine(directory, "selected-" + height + ".png"), 560, 100);
                foreach (RadioButton button in buttons)
                {
                    Border? mark = button.Template.FindName("selectionIndicator", button) as Border;
                    Require(mark != null, "Selection has no non-color indicator.");
                    Require(mark.Visibility == (button.IsChecked == true ? Visibility.Visible : Visibility.Collapsed), "Selection indicator disagrees with IsChecked.");
                    if (button.IsChecked == true)
                    {
                        AssertInside(mark, button, button.ActualWidth, button.ActualHeight, "Selection indicator");
                        Require(mark.ActualHeight >= 2, "Selection indicator has no visible thickness.");
                        Require(Contrast(BrushColor(mark.Background), BrushColor((Brush)theme["VisionTool.SelectionBrush"])) >= 3, "Selection indicator contrast is below 3:1.");
                        TextBlock caption = Descendants(button).OfType<TextBlock>().Single();
                        Require(caption.TranslatePoint(new Point(0, caption.ActualHeight), button).Y <= mark.TranslatePoint(new Point(), button).Y,
                            "Selection indicator overlaps the caption.");
                    }
                }
                buttons[2].IsChecked = true;
                buttons[2].IsEnabled = false;
                Layout(root, 560, 100);
                ScreenshotPngWriter.WriteElementPng(root, Path.Combine(directory, "disabled-selected-" + height + ".png"), 560, 100);
                Require(((Border)buttons[0].Template.FindName("selectionIndicator", buttons[0])).Visibility == Visibility.Collapsed
                    && ((Border)buttons[2].Template.FindName("selectionIndicator", buttons[2])).Visibility == Visibility.Visible,
                    "Selection transfer or disabled state removed the required state cue.");
                Require(buttons.Select(button => button.ActualWidth).SequenceEqual(widths), "Changing selection changed control widths.");
                foreach (RadioButton button in buttons)
                {
                    TextBlock caption = Descendants(button).OfType<TextBlock>().Single();
                    FormattedText text = new(caption.Text, System.Globalization.CultureInfo.CurrentCulture, caption.FlowDirection,
                        new Typeface(caption.FontFamily, caption.FontStyle, caption.FontWeight, caption.FontStretch), caption.FontSize, caption.Foreground, 1);
                    Require(caption.ActualWidth + 1 >= text.WidthIncludingTrailingWhitespace && caption.ActualHeight + 1 >= text.Height,
                        "Selected-control caption is clipped: " + caption.Text);
                    AssertInside(button, root, 560, 100, "Segmented control");
                }
            }, passed, failed);
        }
        Require(string.Equals(Path.GetPathRoot(AppPathService.DataRootDirectory), @"D:\", StringComparison.OrdinalIgnoreCase), "Consumer settings must remain on D:.");
        OpenVisionLanguage previousLanguage = OpenVisionLanguageService.CurrentLanguage;
        try
        {
            foreach (OpenVisionLanguage language in new[] { OpenVisionLanguage.Korean, OpenVisionLanguage.English })
            {
                OpenVisionLanguageService.SetLanguage(language, false);
                foreach ((string name, Func<FrameworkElement> createView) in new (string, Func<FrameworkElement>)[]
                {
                    ("Threshold", () => new ThresholdToolWpfView(new ThresholdToolPresenter(new ThresholdToolViewModel()))),
                    ("Line", () => new LineToolWpfView(new LineToolPresenter(new LineToolViewModel(new LineGaugeProperty("A"), new LineGaugeProperty("B")), () => { }))),
                    ("Arithmetic", () => new ArithmeticToolWpfView())
                })
                {
                    Check($"R-05 {name}/{language} compiled consumer caption/selection", () =>
                    {
                        FrameworkElement view = createView();
                        try
                        {
                            Layout(view, 1180, 890);
                            RadioButton[] segments = Descendants(view).OfType<RadioButton>()
                                .Where(button => button.ActualWidth > 0 && button.Template.FindName("selectionIndicator", button) != null).ToArray();
                            Require(segments.Length >= 2 && segments.Any(button => button.IsChecked == true), "Consumer did not realize selected segmented controls.");
                            ScreenshotPngWriter.WriteElementPng(view, Path.Combine(directory, name + "-" + language + ".png"), 1180, 890);
                            foreach (RadioButton button in segments)
                            {
                                Border mark = (Border)button.Template.FindName("selectionIndicator", button);
                                Require(mark.Visibility == (button.IsChecked == true ? Visibility.Visible : Visibility.Collapsed), "Consumer selection and indicator diverged.");
                                foreach (TextBlock caption in Descendants(button).OfType<TextBlock>())
                                {
                                    FormattedText text = new(caption.Text, System.Globalization.CultureInfo.CurrentCulture, caption.FlowDirection,
                                        new Typeface(caption.FontFamily, caption.FontStyle, caption.FontWeight, caption.FontStretch), caption.FontSize, caption.Foreground, 1);
                                    Require(caption.ActualWidth + 1 >= text.WidthIncludingTrailingWhitespace && caption.ActualHeight + 1 >= text.Height,
                                        "Consumer caption is clipped: " + caption.Text);
                                    if (button.IsChecked == true)
                                        Require(caption.TranslatePoint(new Point(0, caption.ActualHeight), button).Y <= mark.TranslatePoint(new Point(), button).Y,
                                            "Consumer indicator overlaps its caption.");
                                }
                            }
                        }
                        finally { ((IVisionToolViewLifetime)view).DisposeView(); }
                    }, passed, failed);
                }
            }
        }
        finally { OpenVisionLanguageService.SetLanguage(previousLanguage, false); }
        string report = Path.Combine(directory, "segmented-selection-contract.txt");
        File.WriteAllLines(report, new[]
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "Assembly: " + typeof(VisionToolSingleInputPropertyToolShell).Assembly.ManifestModule.ModuleVersionId,
            "Boundary: compiled shared theme and Threshold/Line/Arithmetic consumers, in-process 96-DPI rendering; 30/46-DIP states and 1180x890 KO/EN views. No Window.Show, pointer, keyboard, docked host or workflow proof."
        }.Concat(passed.Select(item => "PASS: " + item)).Concat(failed.Select(item => "FAIL: " + item)));
        Console.WriteLine("SEGMENTED_SELECTION=" + (failed.Count == 0 ? "PASS" : "FAIL") + "|report=" + report);
        return failed.Count == 0 ? 0 : 1;
    }

    internal static int RunToolShellStatus(string? requestedEvidenceDirectory)
    {
        string directory = Path.GetFullPath(requestedEvidenceDirectory ?? @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev\tool-shell-status");
        Require(string.Equals(Path.GetPathRoot(directory), @"D:\", StringComparison.OrdinalIgnoreCase), "Tool Shell evidence must remain on D:.");
        Require(string.Equals(Path.GetPathRoot(AppPathService.DataRootDirectory), @"D:\", StringComparison.OrdinalIgnoreCase), "Consumer settings must remain on D:.");
        Directory.CreateDirectory(directory);
        _ = Application.Current ?? new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        foreach (bool docked in new[] { false, true })
        {
            Check("UI-21 single-input " + (docked ? "docked" : "floating") + " existing fixture",
                () => CheckShell(docked, directory, passed, failed), passed, failed);
            Check("UI-21 Arithmetic " + (docked ? "560x700 docked" : "920x620 floating") + " error readability/recovery",
                () => CheckDoubleInputShellStatus(docked, directory), passed, failed);
        }
        string report = Path.Combine(directory, "tool-shell-status-contract.txt");
        File.WriteAllLines(report, new[]
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "Assembly: " + typeof(VisionToolDoubleInputCustomToolShell).Assembly.Location,
            "MVID: " + typeof(VisionToolDoubleInputCustomToolShell).Assembly.ManifestModule.ModuleVersionId,
            "Boundary: public Arithmetic SetStatus, unchanged settings/routes and zero action callbacks; existing single-input status/guidance. In-process 96-DPI View rendering; no Window.Show, actual keyboard/mouse, native DPI or algorithm failure/recovery proof."
        }.Concat(passed.Select(item => "PASS: " + item)).Concat(failed.Select(item => "FAIL: " + item)));
        Console.WriteLine("TOOL_SHELL_STATUS=" + (failed.Count == 0 ? "PASS" : "FAIL") + "|report=" + report);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void CheckDoubleInputShellStatus(bool docked, string directory)
    {
        int width = docked ? 560 : 920;
        int height = docked ? 700 : 620;
        string name = docked ? "docked" : "floating";
        ArithmeticToolWpfView view = new ArithmeticToolWpfView();
        try
        {
            VisionToolDoubleInputCustomToolShell shell = (VisionToolDoubleInputCustomToolShell)view.FindName("toolShell");
            shell.IsDockedInspectorMode = docked;
            view.MinWidth = docked ? 0 : 920;
            view.MinHeight = docked ? 0 : 620;
            view.SetLayerList(new[] { "Input A / 1000x650", "Input B / 800x600", "Output / Arithmetic" }, "Input A / 1000x650", "Input B / 800x600", "Output / Arithmetic");
            view.SetOperationList(new[] { "Add", "Subtract", "AbsDiff" }, "AbsDiff");
            string settings = System.Text.Json.JsonSerializer.Serialize(view.CaptureSettings());
            string[] routes = new[] { view.SelectedInputLayerA, view.SelectedInputLayerB, view.SelectedOutputLayer };
            int actions = 0;
            view.RunPreviewRequested += (_, _) => actions++;
            view.RunOffsetRequested += (_, _) => actions++;
            view.AddPipelineRequested += (_, _) => actions++;
            view.CreateOutputLayerRequested += (_, _) => actions++;
            string longLine = "Preview failed: " + string.Concat(Enumerable.Repeat("입력 A와 B의 크기와 선택 레이어를 확인한 뒤 미리보기를 다시 실행하세요. ", 8)) + "LAST-ERROR-LINE";
            foreach ((string label, string message) in new[]
            {
                ("long-line", longLine),
                ("multiline", longLine + Environment.NewLine + string.Join(Environment.NewLine,
                    Enumerable.Range(1, 8).Select(index => "오류 상세 " + index + ": A=1000x650 / B=800x600 / 원본은 유지됩니다.")) + Environment.NewLine + "LAST-ERROR-LINE / 마지막 안내")
            })
            {
                view.SetStatus(message);
                Layout(view, width, height);
                Require(shell.StatusText.Text == message, "Public SetStatus lost required error content.");
                ScreenshotPngWriter.WriteElementPng(view, Path.Combine(directory, "arithmetic-" + name + "-" + label + "-start.png"), width, height);
                AssertTextScrolls(shell.StatusText, view, width, height);
                Require(System.Windows.Input.KeyboardNavigation.GetIsTabStop((ScrollViewer)shell.StatusHost.Child), "Error scrolling is skipped by Tab navigation.");
                Require(shell.StatusHost.ActualHeight <= 66, "Long error displaced the parameter/action area.");
                AssertInside(shell.RunPreviewButton, view, width, height, "Arithmetic Preview");
                AssertInside(shell.AddPipelineButton, view, width, height, "Arithmetic Add");
                Require(Contrast(BrushColor(shell.StatusText.Foreground), BrushColor(shell.StatusHost.Background)) >= 4.5, "Error text is unreadable on its semantic background.");
                ScreenshotPngWriter.WriteElementPng(view, Path.Combine(directory, "arithmetic-" + name + "-" + label + "-end.png"), width, height);
            }
            view.SetStatus("");
            Layout(view, width, height);
            Require(shell.StatusHost.Visibility == Visibility.Collapsed, "Clearing the status retained stale error space.");
            view.SetStatus(longLine);
            shell.IsDockedInspectorMode = !docked;
            Layout(view, 980, 700);
            AssertTextScrolls(shell.StatusText, view, 980, 700);
            shell.IsDockedInspectorMode = docked;
            Layout(view, width, height);
            AssertTextScrolls(shell.StatusText, view, width, height);
            Require(routes.SequenceEqual(new[] { view.SelectedInputLayerA, view.SelectedInputLayerB, view.SelectedOutputLayer })
                && settings == System.Text.Json.JsonSerializer.Serialize(view.CaptureSettings()) && actions == 0,
                "Error display/recovery changed settings, routes or executed an action.");
        }
        finally { view.DisposeView(); }
    }

    internal static int RunLayerNames(string? requestedEvidenceDirectory)
    {
        string directory = Path.GetFullPath(requestedEvidenceDirectory ?? @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev\double-input-layers");
        Require(string.Equals(Path.GetPathRoot(directory), @"D:\", StringComparison.OrdinalIgnoreCase), "Layer evidence must remain on D:.");
        Directory.CreateDirectory(directory);
        _ = Application.Current ?? new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        OpenVisionLanguage previousLanguage = OpenVisionLanguageService.CurrentLanguage;
        try
        {
            foreach (OpenVisionLanguage language in new[] { OpenVisionLanguage.Korean, OpenVisionLanguage.English })
            {
                OpenVisionLanguageService.SetLanguage(language, false);
                foreach (bool docked in new[] { false, true })
                foreach (bool unbroken in new[] { false, true })
                {
                    string name = language + "-" + (docked ? "docked" : "floating") + "-" + (unbroken ? "unbroken" : "mixed");
                    Check("Layer selected-name/route contract " + name, () => CheckLayerNames(directory, name, docked, unbroken), passed, failed);
                }
            }
            foreach (bool docked in new[] { false, true })
                Check("Shared single-input pending output " + (docked ? "docked" : "floating"),
                    () => CheckSingleInputPendingOutput(docked), passed, failed);
        }
        finally { OpenVisionLanguageService.SetLanguage(previousLanguage, false); }
        File.WriteAllLines(Path.Combine(directory, "layer-name-contract.txt"), new[]
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "Assembly: " + typeof(ArithmeticToolWpfView).Assembly.Location,
            "MVID: " + typeof(ArithmeticToolWpfView).Assembly.ManifestModule.ModuleVersionId,
            "ChecksPassed: " + passed.Count,
            "ChecksFailed: " + failed.Count,
            "Scope: unshown compiled Arithmetic/Threshold Views, 96-DPI layout/render, selected text, pending/restored/changed routes, input-B visibility, left scroll and released callbacks.",
            "Unverified by this contract: actual pointer/keyboard/popup, native DPI, application docking and algorithm execution."
        }.Concat(passed.Select(item => "PASS " + item)).Concat(failed.Select(item => "FAIL " + item)));
        Console.WriteLine("Layer names: " + passed.Count + " passed, " + failed.Count + " failed. " + directory);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void CheckLayerNames(string directory, string name, bool docked, bool unbroken)
    {
        ArithmeticToolWpfView view = new ArithmeticToolWpfView();
        try
        {
            VisionToolDoubleInputCustomToolShell shell = (VisionToolDoubleInputCustomToolShell)view.FindName("toolShell");
            shell.IsDockedInspectorMode = docked;
            view.MinWidth = docked ? 0 : 920;
            view.MinHeight = docked ? 0 : 620;
            int width = docked ? 560 : 980;
            int height = 700;
            string[] layers = unbroken
                ? new[] { "Input_A_" + new string('A', 180) + "_END_A", "Input_B_" + new string('B', 180) + "_END_B", "Output_" + new string('C', 180) + "_END_OUTPUT" }
                : new[] { "입력 A - 제품 윗면 검사 원본 / Sample_001_1000x650", "입력 B - 기준 영상 / Reference_20261003_1000x650", "출력 - 차영상 검사 결과 / Arithmetic_AbsDiff_1000x650" };
            view.SetLayerList(layers, layers[0], layers[1], layers[2]);
            view.SetOperationList(new[] { "Add", "Subtract", "AbsDiff" }, "AbsDiff");
            int actions = 0;
            int outputNotifications = 0;
            view.OutputLayerChanged += (_, _) => outputNotifications++;
            view.RunPreviewRequested += (_, _) => actions++;
            view.RunOffsetRequested += (_, _) => actions++;
            view.AddPipelineRequested += (_, _) => actions++;
            view.CreateOutputLayerRequested += (_, _) => actions++;
            string settings = System.Text.Json.JsonSerializer.Serialize(view.CaptureSettings());
            Layout(view, width, height);
            ScreenshotPngWriter.WriteElementPng(view, Path.Combine(directory, name + "-start.png"), width, height);
            ScrollViewer? scroll = shell.FindName("previewScrollViewer") as ScrollViewer;
            foreach ((ComboBox combo, string expected) in new[] { (shell.InputAComboBox, layers[0]), (shell.InputBComboBox, layers[1]) })
            {
                TextBlock? text = Descendants(combo).OfType<TextBlock>().SingleOrDefault(item => item.Text == expected);
                Require(text != null && text.Text == expected, "Selected input name has no complete rendered text.");
                AssertWrappedLayerText(text);
                Require(combo.ActualWidth <= shell.InputAPreviewFrame.ActualWidth + 1, "Input selector overflows the preview column.");
                Require(!combo.IsEditable && combo.IsTextSearchEnabled, "Existing input-selection contract changed.");
            }
            TextBlock? output = shell.FindName("txtOutputLayerName") as TextBlock;
            Require(output != null && output.Text == layers[2], "Output write target has no complete visible name.");
            AssertWrappedLayerText(output);
            Require(shell.OutputLayerComboBox.IsEditable && !shell.OutputLayerComboBox.IsReadOnly, "Pending output names are no longer editable.");
            Require(scroll != null && scroll.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled, "Layer cards have no bounded vertical scrolling.");
            scroll.ScrollToEnd();
            Layout(view, width, height);
            AssertInside(output, view, width, height, "Complete output identifier");
            AssertInside(shell.AddPipelineButton, view, width, height, "Add Pipeline");
            AssertInside(shell.RunPreviewButton, view, width, height, "Preview");
            ScreenshotPngWriter.WriteElementPng(view, Path.Combine(directory, name + "-end.png"), width, height);
            shell.InputAComboBox.SelectedItem = layers[1];
            Layout(view, width, height);
            Require(view.SelectedInputLayerA == layers[1] && view.SelectedInputLayerB == layers[1] && view.SelectedOutputLayer == layers[2], "User input selection changed another route.");
            view.SetLayerList(layers, layers[0], layers[1], layers[2]);
            shell.SetInputBPreviewVisible(false);
            Layout(view, width, height);
            Require(!shell.InputBGroup.IsVisible && ((RowDefinition)shell.FindName("rowInputB")).ActualHeight == 0, "Hidden input B retained a stale card row.");
            shell.SetInputBPreviewVisible(true);
            shell.IsDockedInspectorMode = !docked;
            view.MinWidth = 920;
            Layout(view, 980, height);
            shell.IsDockedInspectorMode = docked;
            view.MinWidth = docked ? 0 : 920;
            Layout(view, width, height);
            Require(layers.SequenceEqual(new[] { view.SelectedInputLayerA, view.SelectedInputLayerB, view.SelectedOutputLayer })
                && settings == System.Text.Json.JsonSerializer.Serialize(view.CaptureSettings()) && actions == 0,
                "Layer layout/visibility/restoration changed settings, routes or ran an action.");
            TextBox edit = (TextBox)shell.OutputLayerComboBox.Template.FindName("PART_EditableTextBox", shell.OutputLayerComboBox);
            string pendingOutput = "출력 - 새 이름 / Pending_" + name + "_END";
            edit.Text = pendingOutput;
            Layout(view, width, height);
            Require(view.SelectedOutputLayer == pendingOutput && shell.OutputLayerComboBox.Text == pendingOutput && output.Text == pendingOutput,
                "Typed output, visible identifier and ViewModel write target disagree.");
            Require(view.SelectedInputLayerA == layers[0] && view.SelectedInputLayerB == layers[1] && actions == 0 && outputNotifications == 1,
                "Editing an output name changed inputs, executed an action or duplicated the route notification.");
            view.SetLayerList(layers, layers[0], layers[1], layers[2]);
            Layout(view, width, height);
            Require(view.SelectedOutputLayer == layers[2] && output.Text == layers[2] && outputNotifications == 1,
                "Restoring the output model caused a notification or left stale visible text.");
            view.DisposeView();
            edit.Text = "AfterDispose";
            Require(outputNotifications == 1 && actions == 0, "Disposed layer input still has a live callback.");
        }
        finally { view.DisposeView(); }
    }

    private static void CheckSingleInputPendingOutput(bool docked)
    {
        ThresholdToolWpfView view = new ThresholdToolWpfView(new ThresholdToolPresenter(new ThresholdToolViewModel()));
        try
        {
            VisionToolSingleInputPropertyToolShell shell = (VisionToolSingleInputPropertyToolShell)view.FindName("toolShell");
            shell.IsDockedInspectorMode = docked;
            view.SetLayerList(new[] { "Main", "Result" }, "Main", "Result");
            int outputNotifications = 0;
            int actions = 0;
            view.DestinationLayerChanged += (_, _) => outputNotifications++;
            view.RunPreviewRequested += (_, _) => actions++;
            view.CreateOutputLayerRequested += (_, _) => actions++;
            view.AddPipelineRequested += (_, _) => actions++;
            Layout(view, 980, 700);
            ComboBox combo = (ComboBox)shell.FindName("cbOutputLayer");
            TextBox edit = (TextBox)combo.Template.FindName("PART_EditableTextBox", combo);
            edit.Text = "Pending_Output_END";
            Layout(view, 980, 700);
            Require(view.SelectedInputLayer == "Main" && view.SelectedOutputLayer == "Pending_Output_END" && combo.Text == "Pending_Output_END"
                && outputNotifications == 1 && actions == 0, "Single-input pending output did not reach the model exactly once without execution.");
            view.SetLayerList(new[] { "Main", "Result" }, "Main", "Result");
            Layout(view, 980, 700);
            Require(combo.Text == "Result" && view.SelectedOutputLayer == "Result" && outputNotifications == 1,
                "Single-input model restore notified or retained stale output text.");
            view.DisposeView();
            edit.Text = "AfterDispose";
            Require(outputNotifications == 1 && actions == 0, "Disposed single-input layer field still has a callback.");
        }
        finally { view.DisposeView(); }
    }

    private static void AssertWrappedLayerText(TextBlock text)
    {
        Require(text.TextWrapping == TextWrapping.Wrap && text.TextTrimming == TextTrimming.None, "A required layer identifier still trims.");
        Require(text.ActualWidth > 0, "Layer identifier has no render width.");
        FormattedText expected = new(text.Text, System.Globalization.CultureInfo.CurrentCulture, text.FlowDirection,
            new Typeface(text.FontFamily, text.FontStyle, text.FontWeight, text.FontStretch), text.FontSize, text.Foreground, 1);
        expected.MaxTextWidth = text.ActualWidth;
        Require(text.ActualHeight + 1 >= expected.Height, "Layer identifier is clipped vertically.");
        Require(Contrast(BrushColor(text.Foreground), BrushColor((Brush)text.FindResource("VisionTool.PanelBrush"))) >= 4.5, "Layer identifier has insufficient contrast.");
    }

    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-fix-20260922\readability-contract");
        Require(string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase),
            "Readability evidence must remain on D:.");
        Directory.CreateDirectory(evidenceDirectory);
        _ = Application.Current ?? new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        foreach (bool docked in new[] { true, false })
        {
            Check("UI-21 " + (docked ? "610x620 docked" : "900x620 floating") + " long-message fixture",
                () => CheckShell(docked, evidenceDirectory, passed, failed), passed, failed);
        }
        using Bitmap source = new Bitmap(96, 64);
        Check("UI-04/UI-09 ROI compiled View/caption/template contract",
            () => CheckPopup(new RoiEditorWindow(source, new DrawingRectangle(8, 8, 48, 32), "ROI"),
                "Roi", "RoiPrimaryButtonStyle", "RoiChromeButtonStyle", evidenceDirectory), passed, failed);
        Check("UI-04/UI-09 Template compiled View/caption/template contract",
            () => CheckPopup(new OpenGlTemplateEditorWindow(source, new DrawingRectangle(8, 8, 48, 32), "TRAIN"),
                "Template", "TemplatePrimaryButtonStyle", "TemplateChromeButtonStyle", evidenceDirectory), passed, failed);
        Check("UI-04/UI-09 Compare compiled View/caption/template contract",
            () => CheckPopup(new ImageCompareWindow(), "Compare", "ComparePrimaryButtonStyle", "CompareChromeButtonStyle", evidenceDirectory),
            passed, failed);
        Check("UI-14 Parameter Guide applicability uses readable semantic warning colors",
            () => CheckGuide(evidenceDirectory), passed, failed);
        Check("UI-23 Learn actual inactive selection remains readable in an unshown compiled View",
            () => CheckLearnInactiveSelection(evidenceDirectory), passed, failed);
        Check("UI-16 Canvas mode names, command bindings and accessible state round trip in KO/EN",
            () => CheckCanvasModes(evidenceDirectory), passed, failed);
        Check("VIS-L03 Layer/Recipe routes retain full layer identifiers at the minimum Learn width",
            () => CheckLearnLayerRoutes(evidenceDirectory), passed, failed);

        List<string> report = new List<string>
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "Assembly: " + typeof(VisionToolSingleInputPropertyToolShell).Assembly.Location,
            "ChecksPassed: " + passed.Count,
            "ChecksFailed: " + failed.Count,
            "Runtime scope: in-process compiled WPF View construction, Measure/Arrange, ApplyTemplate and 96-DPI element rendering; Window.Show is not called.",
            "Popup scope: actual primary-caption Foreground binding/inheritance and compiled hover/pressed/focus trigger values; foreground probe is not actual mouse hover/pressed evidence.",
            "Unverified: actual pointer-down, mouse recovery, keyboard focus activation, open popups, monitor placement, 125/150/175/200% DPI, and full product theme matrix.",
            "UI-23 Learn checks the actual inactive selected-item state with the corrected product template; no fake focus property is used. Focused selection remains unverified."
        };
        report.AddRange(passed.Select(item => "PASS: " + item));
        report.AddRange(failed.Select(item => "FAIL: " + item));
        string reportPath = Path.Combine(evidenceDirectory, "ui-readability-contract.txt");
        File.WriteAllLines(reportPath, report);
        Console.WriteLine("UI_READABILITY_CONTRACT=" + (failed.Count == 0 ? "PASS" : "FAIL") + "|report=" + reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void CheckShell(bool docked, string directory, ICollection<string> passed, ICollection<string> failed)
    {
        int width = docked ? 610 : 900;
        const int height = 620;
        string name = docked ? "docked" : "floating";
        VisionToolSingleInputPropertyToolShell shell = new VisionToolSingleInputPropertyToolShell
        {
            IsDockedInspectorMode = docked,
            TemplateStatusVisibility = Visibility.Collapsed,
            ToolContentVisibility = Visibility.Collapsed,
            ResultReviewVisibility = Visibility.Visible
        };
        PropertyGrid propertyGrid = new PropertyGrid { SelectedObject = new { Threshold = 180, MaxValue = 255 } };
        shell.PropertyGridHost.Child = propertyGrid;
        shell.TitleText.Text = "Threshold / parameter and result review";
        shell.SummaryText.Text = "Threshold 180 / Binary / MaxValue 255";
        shell.ResultReviewText.Text = "미리보기 결과를 확인하세요 / Review preview output";
        shell.StatusText.Text = string.Join(Environment.NewLine, Enumerable.Range(1, 8).Select(index =>
            $"오류 {index}: 이미지 파일과 입력 레이어를 확인한 뒤 다시 미리보기를 실행하세요. Preview error {index}: verify the source image and input layer."));
        shell.StatusText.Text += Environment.NewLine + "STATUS-LAST-LINE / 마지막 오류 내용";
        shell.ResultGuidanceText.Text = string.Join(Environment.NewLine, Enumerable.Range(1, 8).Select(index =>
            $"안내 {index}: 입력 이미지와 결과 오버레이를 비교하고 대상 영역의 명암 및 임계값을 확인하세요. Review source, threshold and result overlay."));
        shell.ResultGuidanceText.Text += Environment.NewLine + "GUIDANCE-LAST-LINE / 마지막 다음 작업";
        Layout(shell, width, height);
        ScreenshotPngWriter.WriteElementPng(shell, Path.Combine(directory, "shell-" + name + "-start.png"), width, height);
        Check("UI-21 " + name + " complete error text is vertically scrollable and its last line reachable",
            () => AssertTextScrolls(shell.StatusText, shell, width, height), passed, failed);
        Check("UI-21 " + name + " complete guidance is vertically scrollable and its last line reachable",
            () => AssertTextScrolls(shell.ResultGuidanceText, shell, width, height), passed, failed);
        Check("UI-21 " + name + " Run Preview and PropertyGrid retain reachable positive area",
            () =>
            {
                AssertInside(shell.RunPreviewButton, shell, width, height, "Run Preview");
                AssertInside(propertyGrid, shell, width, height, "PropertyGrid");
            }, passed, failed);
        ScreenshotPngWriter.WriteElementPng(shell, Path.Combine(directory, "shell-" + name + "-end.png"), width, height);
        propertyGrid.SelectedObject = null;
        shell.PropertyGridHost.Child = null;
    }

    private static void AssertTextScrolls(TextBlock text, FrameworkElement root, int width, int height)
    {
        Require(text.TextWrapping == TextWrapping.Wrap && text.TextTrimming == TextTrimming.None,
            "Required text still trims or does not wrap.");
        ScrollViewer? scroll = Ancestors(text).OfType<ScrollViewer>().FirstOrDefault();
        Require(scroll != null, "Required long text has no owning ScrollViewer.");
        AssertInside(scroll, root, width, height, text.Name + " scroll viewport");
        Require(scroll.VerticalScrollBarVisibility == ScrollBarVisibility.Auto
            && scroll.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled,
            "Long text is not configured for vertical wrapped scrolling.");
        Require(scroll.ViewportHeight > 0 && scroll.ExtentHeight > scroll.ViewportHeight && scroll.ScrollableHeight > 0,
            $"Long text has no usable scroll range: extent={scroll.ExtentHeight:0.##}, viewport={scroll.ViewportHeight:0.##}.");
        scroll.ScrollToEnd();
        root.UpdateLayout();
        PumpBindings();
        root.UpdateLayout();
        double bottom = text.TranslatePoint(new Point(0, text.ActualHeight), scroll).Y;
        Require(scroll.VerticalOffset > 0 && Math.Abs(scroll.VerticalOffset - scroll.ScrollableHeight) < 1,
            "ScrollToEnd did not reach the final offset.");
        Require(bottom > 0 && bottom <= scroll.ActualHeight + 2,
            $"The final text line is outside its scroll viewport: bottom={bottom:0.##}, height={scroll.ActualHeight:0.##}.");
        Require(text.ActualWidth <= scroll.ViewportWidth + 2, "Required text overflows horizontally.");
    }

    private static void CheckPopup(Window window, string name, string primaryKey, string chromeKey, string directory)
    {
        try
        {
            FrameworkElement root = window.Content as FrameworkElement
                ?? throw new InvalidOperationException("Popup compiled content is missing.");
            Layout(root, 1100, 780);
            Style primaryStyle = (Style)window.FindResource(primaryKey);
            Style chromeStyle = (Style)window.FindResource(chromeKey);
            Button primary = Descendants(root).OfType<Button>().Single(button => ReferenceEquals(button.Style, primaryStyle));
            primary.ApplyTemplate();
            List<TextBlock> captions = Descendants(primary).OfType<TextBlock>().Where(text => !string.IsNullOrWhiteSpace(text.Text)).ToList();
            Require(captions.Count > 0, name + " primary action caption was not realized.");
            Require(captions.All(text => BrushColor(text.Foreground) == BrushColor(primary.Foreground)),
                name + " primary caption does not match its initial Button foreground.");
            SolidColorBrush probe = new SolidColorBrush(Color.FromRgb(24, 43, 54));
            primary.SetCurrentValue(Control.ForegroundProperty, probe);
            PumpBindings();
            Require(captions.All(text => BrushColor(text.Foreground) == probe.Color),
                name + " primary caption ignores its Button foreground (for example a local fixed White value).");
            primary.ClearValue(Control.ForegroundProperty);
            PumpBindings();
            AssertActionTriggers(primary);
            Button[] chrome = Descendants(root).OfType<Button>().Where(button => ReferenceEquals(button.Style, chromeStyle)).ToArray();
            Require(chrome.Length > 0, name + " window chrome buttons were not realized.");
            foreach (Button button in chrome)
            {
                button.ApplyTemplate();
                Require(!string.IsNullOrWhiteSpace(AutomationProperties.GetName(button)), name + " chrome button has no accessible name.");
                AssertFocusCue(button);
            }
            ScreenshotPngWriter.WriteElementPng(root, Path.Combine(directory, "popup-" + name.ToLowerInvariant() + "-normal.png"), 1100, 780);
        }
        finally
        {
            (window as IDisposable)?.Dispose();
            window.Close();
        }
    }

    private static void AssertActionTriggers(Button button)
    {
        foreach (DependencyProperty state in new[] { UIElement.IsMouseOverProperty, System.Windows.Controls.Primitives.ButtonBase.IsPressedProperty })
        {
            Trigger trigger = FindTrigger(button, state);
            Setter? foreground = trigger.Setters.OfType<Setter>().FirstOrDefault(setter => setter.Property == Control.ForegroundProperty);
            Setter? background = trigger.Setters.OfType<Setter>().FirstOrDefault(setter => setter.Property == Border.BackgroundProperty);
            Require(background != null && background.TargetName == "buttonChrome",
                state.Name + " template state is missing the rendered button background.");
            Brush stateForeground = foreground == null ? button.Foreground : ResolveStateBrush(button, foreground.Value);
            Brush stateBackground = ResolveStateBrush(button, background.Value);
            Require(Contrast(BrushColor(stateForeground), BrushColor(stateBackground)) >= 4.5,
                state.Name + " template caption colors have contrast below 4.5:1.");
        }
        AssertFocusCue(button);
    }

    private static Brush ResolveStateBrush(Button button, object value)
    {
        if (value is Brush brush) return brush;
        if (value is DynamicResourceExtension resource && button.TryFindResource(resource.ResourceKey) is Brush resolved) return resolved;
        throw new InvalidOperationException("Compiled state brush could not be resolved: " + value);
    }

    private static void AssertFocusCue(Button button)
    {
        Border? cue = button.Template.FindName("focusCue", button) as Border;
        Require(cue != null && cue.BorderThickness.Left >= 2 && BrushColor(cue.BorderBrush).A > 0,
            "Action template lacks a visible keyboard-focus border.");
        Trigger focus = FindTrigger(button, UIElement.IsKeyboardFocusedProperty);
        Require(focus.Setters.OfType<Setter>().Any(setter => setter.TargetName == "focusCue"
            && setter.Property == UIElement.VisibilityProperty && Equals(setter.Value, Visibility.Visible)),
            "Keyboard focus does not reveal the action's focus border.");
    }

    private static void CheckGuide(string directory)
    {
        VisionToolParameterGuideView guide = new VisionToolParameterGuideView();
        guide.ShowContent(new VisionToolParameterGuideContent
        {
            Title = "Threshold", Identity = "Threshold / Mode", Coverage = "현재 도구",
            Applicability = "현재 모드에서는 비활성입니다. 적용되는 모드를 먼저 선택하세요.",
            Summary = "매개변수를 변경한 뒤 입력 이미지와 결과를 비교하세요.",
            CheckAfterPreview = "대상 영역의 밝기와 선택된 임계값을 확인하세요."
        }, _ => { });
        Layout(guide, 430, 330);
        TextBlock applicability = (TextBlock)guide.FindName("txtApplicability");
        Require(applicability.Visibility == Visibility.Visible && applicability.ActualHeight > 0, "Applicability warning was not laid out.");
        Require(ReferenceEquals(applicability.Foreground, guide.FindResource("VisionTool.StatusWarningBrush"))
            && ReferenceEquals(applicability.Background, guide.FindResource("VisionTool.PanelBrush")), "Applicability warning bypasses semantic brushes.");
        Require(Contrast(BrushColor(applicability.Foreground), BrushColor(applicability.Background)) >= 4.5,
            "Applicability warning contrast is below 4.5:1.");
        ScreenshotPngWriter.WriteElementPng(guide, Path.Combine(directory, "parameter-guide-applicability.png"), 430, 330);
    }

    private static Trigger FindTrigger(Button button, DependencyProperty property)
    {
        return button.Template.Triggers.OfType<Trigger>().SingleOrDefault(trigger => trigger.Property == property && Equals(trigger.Value, true))
            ?? throw new InvalidOperationException("Compiled template is missing " + property.Name + ".");
    }

    private static void CheckLearnInactiveSelection(string directory)
    {
        OpenVisionLearnWindow window = new OpenVisionLearnWindow();
        try
        {
            FrameworkElement root = (FrameworkElement)window.Content;
            Layout(root, 1040, 700);
            ListBox topics = (ListBox)window.FindName("topicList");
            ListBoxItem? selected = topics.ItemContainerGenerator.ContainerFromIndex(topics.SelectedIndex) as ListBoxItem;
            Require(selected != null && selected.IsSelected, "Learn selected topic container was not realized.");
            Require(!selected.IsKeyboardFocusWithin && !Selector.GetIsSelectionActive(selected),
                "Learn fixture does not represent actual inactive selection; no inactive-state claim is permitted.");
            TextBlock? caption = Descendants(selected).OfType<TextBlock>().FirstOrDefault(text => !string.IsNullOrWhiteSpace(text.Text));
            Require(caption != null, "Learn inactive selected-item caption was not realized.");
            RenderTargetBitmap pixels = new RenderTargetBitmap(1040, 700, 96, 96, PixelFormats.Pbgra32);
            pixels.Render(root);
            Point sample = selected.TranslatePoint(new Point(5, selected.ActualHeight / 2), root);
            byte[] pixel = new byte[4];
            pixels.CopyPixels(new Int32Rect((int)sample.X, (int)sample.Y, 1, 1), pixel, 4, 0);
            Color background = Color.FromArgb(pixel[3], pixel[2], pixel[1], pixel[0]);
            ScreenshotPngWriter.WriteElementPng(root, Path.Combine(directory, "learn-inactive-selection.png"), 1040, 700);
            Require(Contrast(BrushColor(caption.Foreground), background) >= 4.5,
                $"Learn inactive selected topic caption contrast is below 4.5:1: foreground={caption.Foreground}, background={background}.");
        }
        finally
        {
            window.Close();
        }
    }

    private static void CheckLearnLayerRoutes(string directory)
    {
        OpenVisionLearnWindow window = new(127, 255, false, 11);
        try
        {
            FrameworkElement root = (FrameworkElement)window.Content;
            Layout(root, 920, 620);
            LayerRecipeLearnView view = (LayerRecipeLearnView)window.FindName("layerRecipeLearnView");
            bool foundCombinedInput = false;
            foreach (string name in new[] { "layerRecipeLayerGrid", "layerRecipeFlowGrid" })
            {
                System.Windows.Controls.Primitives.UniformGrid grid = (System.Windows.Controls.Primitives.UniformGrid)view.FindName(name);
                foreach (Border cell in grid.Children.OfType<Border>())
                {
                    TextBlock caption = (TextBlock)cell.Child;
                    FormattedText text = new(caption.Text, System.Globalization.CultureInfo.CurrentCulture, caption.FlowDirection,
                        new Typeface(caption.FontFamily, caption.FontStyle, caption.FontWeight, caption.FontStretch), caption.FontSize, caption.Foreground, 1);
                    Require(cell.ActualWidth - cell.BorderThickness.Left - cell.BorderThickness.Right >= text.WidthIncludingTrailingWhitespace,
                        "Learn route identifier is clipped: " + caption.Text);
                    foundCombinedInput |= caption.Text == "Main + Pin_Gap";
                }
            }
            Require(foundCombinedInput, "Combined layer route fixture is missing.");
            UniformGrid routeGrid = (UniformGrid)view.FindName("layerRecipeFlowGrid");
            ScrollViewer routeScroll = Ancestors(routeGrid).OfType<ScrollViewer>().First();
            routeScroll.ScrollToVerticalOffset(routeGrid.TranslatePoint(new Point(0, 0), (FrameworkElement)routeScroll.Content).Y);
            Layout(root, 920, 620);
            TextBlock combinedInput = routeGrid.Children.OfType<Border>().Select(cell => (TextBlock)cell.Child).Single(caption => caption.Text == "Main + Pin_Gap");
            AssertInside(combinedInput, routeScroll, routeScroll.ActualWidth, routeScroll.ActualHeight, "Combined layer input");
            ScreenshotPngWriter.WriteElementPng(root, Path.Combine(directory, "learn-layer-routes-minimum.png"), 920, 620);
        }
        finally { window.Close(); }
    }

    private static void CheckCanvasModes(string directory)
    {
        using OpenVisionLab.ImageCanvas.ViewModels.RoiImageCanvasViewModel model = new("UiModeContract");
        using OpenVisionLab.ImageCanvas.Views.RoiImageCanvasView view = new() { DataContext = model };
        OpenVisionLanguage original = OpenVisionLanguageService.CurrentLanguage;
        try
        {
            Layout(view, 560, 320);
            Button roi = Descendants(view).OfType<Button>().Single(button => AutomationProperties.GetAutomationId(button) == "ImageCanvasRoiModeButton");
            Button measure = Descendants(view).OfType<Button>().Single(button => AutomationProperties.GetAutomationId(button) == "ImageCanvasMeasureModeButton");
            System.Windows.Shapes.Path roiMark = Descendants(roi).OfType<System.Windows.Shapes.Path>().Single(path => AutomationProperties.GetAutomationId(path) == "ImageCanvasRoiModeCheckedMark");
            System.Windows.Shapes.Path measureMark = Descendants(measure).OfType<System.Windows.Shapes.Path>().Single(path => AutomationProperties.GetAutomationId(path) == "ImageCanvasMeasureModeCheckedMark");
            void AssertVisibleModes()
            {
                Layout(view, 560, 320);
                Require(roiMark.Visibility == (model.IsTeachingMode ? Visibility.Visible : Visibility.Collapsed)
                    && measureMark.Visibility == (model.IsShowMeasure ? Visibility.Visible : Visibility.Collapsed), "Visible mode checks diverged.");
                foreach (var pair in new[] { (roiMark, roi), (measureMark, measure) })
                {
                    Require(pair.Item1.Data != null && pair.Item1.StrokeThickness >= 2, "Canvas mode check lacks visible geometry.");
                    Require(Contrast(BrushColor(pair.Item1.Stroke), Color.FromRgb(64, 64, 64)) >= 3, "Canvas mode check has insufficient contrast on its darkest active background.");
                    if (pair.Item1.Visibility == Visibility.Visible) { AssertInside(pair.Item1, pair.Item2, 35, 34, "Canvas mode check"); }
                }
            }
            foreach (OpenVisionLanguage language in new[] { OpenVisionLanguage.Korean, OpenVisionLanguage.English })
            {
                OpenVisionLanguageService.SetLanguage(language, false);
                model.IsTeachingMode = false;
                model.IsShowMeasure = false;
                PumpBindings();
                AssertVisibleModes();
                bool english = language == OpenVisionLanguage.English;
                string on = english ? "On" : "켜짐";
                string off = english ? "Off" : "꺼짐";
                Require(AutomationProperties.GetName(roi) == (english ? "ROI selection mode" : "ROI 선택 모드")
                    && AutomationProperties.GetName(measure) == (english ? "Measurement mode" : "측정 모드")
                    && AutomationProperties.GetItemStatus(roi) == off && AutomationProperties.GetItemStatus(measure) == off,
                    "Canvas language/off bindings did not follow the model.");
                Require(roi.Command?.CanExecute(roi.CommandParameter) == true, "ROI command is unavailable.");
                roi.Command.Execute(roi.CommandParameter);
                PumpBindings();
                AssertVisibleModes();
                ScreenshotPngWriter.WriteElementPng(view, Path.Combine(directory, "canvas-roi-" + language + ".png"), 560, 320);
                Require(model.IsTeachingMode && !model.IsShowMeasure && AutomationProperties.GetItemStatus(roi) == on,
                    "ROI command and accessible state diverged.");
                Require(measure.Command?.CanExecute(measure.CommandParameter) == true, "Measure command is unavailable.");
                measure.Command.Execute(measure.CommandParameter);
                PumpBindings();
                AssertVisibleModes();
                Require(!model.IsTeachingMode && model.IsShowMeasure && AutomationProperties.GetItemStatus(roi) == off
                    && AutomationProperties.GetItemStatus(measure) == on, "Measure command and accessible state diverged.");
                ScreenshotPngWriter.WriteElementPng(view, Path.Combine(directory, "canvas-measure-" + language + ".png"), 560, 320);
                measure.Command.Execute(measure.CommandParameter);
                PumpBindings();
                AssertVisibleModes();
                Require(!model.IsTeachingMode && !model.IsShowMeasure && AutomationProperties.GetItemStatus(measure) == off,
                    "Repeated Measure command did not restore the off state.");
            }
        }
        finally { OpenVisionLanguageService.SetLanguage(original, false); }
    }

    private static void AssertInside(FrameworkElement element, FrameworkElement root, double width, double height, string name)
    {
        Point origin = element.TranslatePoint(new Point(0, 0), root);
        Require(element.ActualWidth > 0 && element.ActualHeight > 0 && origin.X >= -1 && origin.Y >= -1
            && origin.X + element.ActualWidth <= width + 1 && origin.Y + element.ActualHeight <= height + 1,
            $"{name} is outside the {width}x{height} viewport: {origin.X:0.##},{origin.Y:0.##} / {element.ActualWidth:0.##}x{element.ActualHeight:0.##}.");
    }

    private static void Layout(FrameworkElement root, int width, int height)
    {
        root.ApplyTemplate();
        root.Measure(new Size(width, height));
        root.Arrange(new Rect(0, 0, width, height));
        root.UpdateLayout();
        PumpBindings();
        root.UpdateLayout();
    }

    private static void PumpBindings()
    {
        DispatcherFrame frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false));
        Dispatcher.PushFrame(frame);
    }

    private static IEnumerable<DependencyObject> Descendants(DependencyObject root)
    {
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(root, index);
            yield return child;
            foreach (DependencyObject descendant in Descendants(child)) { yield return descendant; }
        }
    }

    private static IEnumerable<DependencyObject> Ancestors(DependencyObject element)
    {
        DependencyObject? parent = VisualTreeHelper.GetParent(element);
        while (parent != null) { yield return parent; parent = VisualTreeHelper.GetParent(parent); }
    }

    private static Color BrushColor(Brush brush)
    {
        return brush is SolidColorBrush solid ? solid.Color : throw new InvalidOperationException("Expected a resolved solid color brush.");
    }

    private static double Contrast(Color foreground, Color background)
    {
        static double Channel(byte value) { double v = value / 255d; return v <= 0.04045 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4); }
        static double Luminance(Color color) => 0.2126 * Channel(color.R) + 0.7152 * Channel(color.G) + 0.0722 * Channel(color.B);
        Require(foreground.A == 255 && background.A == 255, "Contrast requires resolved opaque colors.");
        double first = Luminance(foreground);
        double second = Luminance(background);
        return (Math.Max(first, second) + 0.05) / (Math.Min(first, second) + 0.05);
    }

    private static void Check(string name, Action action, ICollection<string> passed, ICollection<string> failed)
    {
        try { action(); passed.Add(name); }
        catch (Exception exception) { failed.Add(name + ": " + exception.GetType().Name + ": " + exception.Message); }
    }

    private static void Require([DoesNotReturnIf(false)] bool condition, string message)
    {
        if (!condition) { throw new InvalidOperationException(message); }
    }
}
