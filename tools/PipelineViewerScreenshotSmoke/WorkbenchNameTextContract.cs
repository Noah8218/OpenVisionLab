using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

internal static class WorkbenchNameTextContract
{
    internal static int Run(string outputPath, string sourceRoot)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);
        XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";
        XElement titleSource = XDocument.Load(Path.Combine(sourceRoot, "src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml"))
            .Descendants().Single(item => (string?)item.Attribute(x + "Name") == "txtDockedToolTitle");
        XElement rowsSource = XDocument.Load(Path.Combine(sourceRoot, "src/OpenVisionLab/UI/Menu/Wpf/Windows/OpenVisionShellHostWindow.xaml"))
            .Descendants().Single(item => (string?)item.Attribute(x + "Name") == "managedLayerRows");
        Require(titleSource.Name.LocalName == nameof(OpenVisionWorkbenchNameText)
            && (string?)titleSource.Attribute("Text") == "{Binding Workbench.Tool.Title, RelativeSource={RelativeSource AncestorType=local:OpenVisionShellHostView}}"
            && (string?)titleSource.Attribute("AutomationProperties.AutomationId") == "WorkbenchToolTitle", "Main source binding/identity");
        XElement layerSource = rowsSource.Descendants().Single(item => (string?)item.Attribute("Text") == "{Binding Title}");
        Require(layerSource.Name.LocalName == nameof(OpenVisionWorkbenchNameText) && (string?)layerSource.Attribute("Grid.Column") == "0"
            && (string?)layerSource.Attribute("Margin") == "8,10", "Layer source binding/column/margin");

        FontFamily font = new(new Uri(Path.Combine(sourceRoot, "src/OpenVisionLab/UI/Wpf/Fonts/") + Path.DirectorySeparatorChar), "./#Pretendard");
        var cases = new List<object>();
        string[] samples = {
            "02 · 긴 단계 이름 - 입자 검출 개수와 면적 조건의 판정 사유를 확인하는 단계",
            "생산라인 검사 이미지 원본 이름 InspectionReference_20261015_Revision001",
            "02 · Long stage name  -  inspect particle count and area acceptance decision details",
            "InspectionResultCountAndAreaAcceptanceDecisionForVeryLongIdentifier_20261015_Revision001",
            "검사  이름\t공백과 영문 ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890 한글어절",
            "첫째 줄\r\n둘째 줄\n\n마지막 줄"
        };
        foreach (double width in new[] { 308D, 180D, 80D })
        foreach (string sample in samples)
        {
            OpenVisionWorkbenchNameText actual = new() { Text = sample, FontFamily = font, FontWeight = FontWeights.Bold, FontSize = 15, Foreground = Brushes.Black };
            actual.Measure(new Size(width, double.PositiveInfinity));
            actual.Arrange(new Rect(0, 0, width, actual.DesiredSize.Height));
            RenderTargetBitmap bitmap = new((int)Math.Ceiling(width), Math.Max(1, (int)Math.Ceiling(actual.ActualHeight)), 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(actual);
            GlyphRunDrawing[] glyphs = Glyphs(VisualTreeHelper.GetDrawing(actual)).ToArray();
            string[] lines = glyphs.GroupBy(item => Math.Round(item.GlyphRun.BaselineOrigin.Y, 3))
                .OrderBy(group => group.Key).Select(group => string.Concat(group.Select(item => new string(item.GlyphRun.Characters.ToArray())))).ToArray();
            Require(actual.Text == sample && RemoveWhitespace(string.Concat(lines)) == RemoveWhitespace(sample), "unchanged raw/glyph content");
            foreach (string word in sample.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            {
                FormattedText measured = new(word, actual.Language.GetSpecificCulture(), FlowDirection.LeftToRight,
                    new Typeface(font, FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 15, Brushes.Black, 1D);
                if (measured.Width <= width) { Require(lines.Any(line => line.Contains(word, StringComparison.Ordinal)), "fitting word stays on one line: " + word); }
            }
            Require(glyphs.All(item => item.GlyphRun.GlyphTypeface.FontUri.ToString().EndsWith("Pretendard-Bold.ttf", StringComparison.OrdinalIgnoreCase)), "original font/weight");
            Require(glyphs.All(item => item.Bounds.Right <= width + 1D && item.Bounds.Bottom <= actual.ActualHeight + 1D), "glyphs fit measured extent");
            if (sample == samples[3]) { Require(lines.Length > 1, "over-wide identifier wraps"); }
            if (width == 180D && sample == samples[0])
            {
                DrawingVisual background = new();
                using (DrawingContext context = background.RenderOpen()) { context.DrawRectangle(Brushes.White, null, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight)); }
                bitmap.Clear(); bitmap.Render(background); bitmap.Render(actual);
                PngBitmapEncoder encoder = new(); encoder.Frames.Add(BitmapFrame.Create(bitmap));
                using FileStream stream = File.Create(Path.ChangeExtension(outputPath, ".independent-render.png")); encoder.Save(stream);
            }
            cases.Add(new { width, sample, lines, actual.ActualHeight, GlyphCount = glyphs.Sum(item => item.GlyphRun.Characters.Count) });
        }

        Model model = new() { Title = samples[0] };
        TextBlock before = new() { FontFamily = font, FontSize = 15, FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap };
        OpenVisionWorkbenchNameText after = new() { FontFamily = font, FontSize = 15, FontWeight = FontWeights.Bold };
        BindingOperations.SetBinding(before, TextBlock.TextProperty, new Binding(nameof(Model.Title)) { Source = model });
        BindingOperations.SetBinding(after, TextBlock.TextProperty, new Binding(nameof(Model.Title)) { Source = model });
        AutomationProperties.SetAutomationId(before, "WorkbenchToolTitle"); AutomationProperties.SetAutomationId(after, "WorkbenchToolTitle");
        AutomationPeer oldPeer = UIElementAutomationPeer.CreatePeerForElement(before);
        AutomationPeer newPeer = UIElementAutomationPeer.CreatePeerForElement(after);
        var patterns = new List<object>();
        foreach (PatternInterface pattern in Enum.GetValues<PatternInterface>())
        {
            bool oldAvailable = oldPeer.GetPattern(pattern) != null;
            bool newAvailable = newPeer.GetPattern(pattern) != null;
            Require(oldAvailable == newAvailable, "same supported pattern: " + pattern);
            patterns.Add(new { Pattern = pattern.ToString(), Old = oldAvailable, New = newAvailable });
        }
        foreach (string next in new[] { samples[0], samples[1], samples[1], string.Empty, samples[2] })
        {
            model.Title = next;
            Require(before.Text == next && after.Text == next && BindingOperations.IsDataBound(after, TextBlock.TextProperty), "live original binding/update/clear");
            Require(oldPeer.GetName() == newPeer.GetName() && newPeer.GetName() == next, "same raw automation name");
        }
        Require(oldPeer.GetAutomationId() == newPeer.GetAutomationId() && oldPeer.GetClassName() == newPeer.GetClassName()
            && oldPeer.GetAutomationControlType() == newPeer.GetAutomationControlType()
            && oldPeer.GetLocalizedControlType() == newPeer.GetLocalizedControlType()
            && oldPeer.GetAccessKey() == newPeer.GetAccessKey() && oldPeer.GetAcceleratorKey() == newPeer.GetAcceleratorKey()
            && oldPeer.GetHelpText() == newPeer.GetHelpText() && oldPeer.GetItemType() == newPeer.GetItemType()
            && oldPeer.GetItemStatus() == newPeer.GetItemStatus() && oldPeer.GetOrientation() == newPeer.GetOrientation()
            && oldPeer.IsEnabled() == newPeer.IsEnabled() && oldPeer.IsPassword() == newPeer.IsPassword()
            && oldPeer.IsContentElement() == newPeer.IsContentElement() && oldPeer.IsControlElement() == newPeer.IsControlElement()
            && oldPeer.IsKeyboardFocusable() == newPeer.IsKeyboardFocusable()
            && (oldPeer.GetChildren()?.Count ?? 0) == (newPeer.GetChildren()?.Count ?? 0), "same peer identity/type/content/order/keyboard contract");
        AutomationProperties.SetName(before, "Explicit accessible name"); AutomationProperties.SetName(after, "Explicit accessible name");
        Require(oldPeer.GetName() == newPeer.GetName(), "explicit accessible name precedence");
        BindingOperations.ClearAllBindings(after); after.ClearValue(TextBlock.TextProperty);
        Require(after.Text == string.Empty && !BindingOperations.IsDataBound(after, TextBlock.TextProperty), "binding detach/reset");
        StackPanel inheritance = new();
        TextElement.SetFontFamily(inheritance, font); TextElement.SetFontSize(inheritance, 13D);
        TextBlock normalBefore = new() { Text = "Layer 이름" };
        OpenVisionWorkbenchNameText normalAfter = new() { Text = normalBefore.Text };
        inheritance.Children.Add(normalBefore); inheritance.Children.Add(normalAfter);
        normalAfter.SetResourceReference(Control.ForegroundProperty, "TestText");
        foreach (Brush color in new[] { Brushes.Black, Brushes.White })
        {
            inheritance.Resources["TestText"] = color;
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            inheritance.Measure(new Size(500D, double.PositiveInfinity));
            inheritance.Arrange(new Rect(0D, 0D, 500D, inheritance.DesiredSize.Height));
            RenderTargetBitmap render = new(500, 100, 96, 96, PixelFormats.Pbgra32); render.Render(inheritance);
            GlyphRunDrawing[] drawn = Glyphs(VisualTreeHelper.GetDrawing(normalAfter)).ToArray();
            Require(normalAfter.FontFamily.Source == normalBefore.FontFamily.Source && normalAfter.FontSize == normalBefore.FontSize
                && normalAfter.FontWeight == normalBefore.FontWeight && normalAfter.FontStyle == normalBefore.FontStyle
                && normalAfter.DesiredSize == normalBefore.DesiredSize && drawn.All(item => item.ForegroundBrush is SolidColorBrush brush && brush.Color == ((SolidColorBrush)color).Color),
                $"inherited normal Layer font/layout and dynamic theme brush: before={normalBefore.FontFamily.Source}/{normalBefore.FontSize}/{normalBefore.FontWeight}/{normalBefore.DesiredSize}; after={normalAfter.FontFamily.Source}/{normalAfter.FontSize}/{normalAfter.FontWeight}/{normalAfter.DesiredSize}; expected={color}; actual={normalAfter.Foreground}; drawn={string.Join(",", drawn.Select(item => item.ForegroundBrush))}");
        }
        foreach (string shortText in new[] { string.Empty, "Short title", "한글 제목" })
        {
            normalBefore.Text = shortText; normalAfter.Text = shortText;
            inheritance.Measure(new Size(500D, double.PositiveInfinity));
            Require(normalBefore.DesiredSize == normalAfter.DesiredSize, "short/empty intrinsic layout preserved");
        }
        CheckHitRegion(font, samples[0]);
        inheritance.Children.Clear();
        WeakReference detached = CreateDetachedReference(model);
        System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
        Require(!detached.IsAlive, "detached text control is not retained by subscriptions/static state");
        Require(Application.Current == null || Application.Current.Windows.Count == 0, "no desktop Window created");
        File.WriteAllText(outputPath, JsonSerializer.Serialize(new {
            Status = "PASS", Scope = "Independent WPF rendering/peer comparison and hidden owned HWND hit-test; no shown desktop Window or Computer Use",
            Cases = cases, Patterns = patterns, RawTextAndBinding = "PASS", PeerIdentityAndOrder = "PASS", BindingUpdateClearDetach = "PASS", InheritedNormalFontDynamicBrushShortEmpty = "PASS", DetachedControlCollection = "PASS", ConnectedInputAndVisualHitRegion = "PASS: glyph/line gap/line end/full-box center",
            Boundary = "Actual caller mouse/double-click, product theme/layout bounds, screen automation bounds, teardown and native DPI require product runtime verification. No claim of GUI or 125% PASS."
        }, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine("WorkbenchNameText=PASS | RenderCases=18 | RawBinding/Peer/Update/Clear/Detach/ConnectedHitRegion=PASS | ShownWindows=0");
        return 0;
    }

    private static IEnumerable<GlyphRunDrawing> Glyphs(Drawing? drawing)
    {
        if (drawing is GlyphRunDrawing glyph) { yield return glyph; }
        if (drawing is DrawingGroup group) { foreach (Drawing child in group.Children) { foreach (GlyphRunDrawing item in Glyphs(child)) { yield return item; } } }
    }

    private static string RemoveWhitespace(string text) => new(text.Where(character => !char.IsWhiteSpace(character)).ToArray());
    private static void Require(bool condition, string message) { if (!condition) { throw new InvalidOperationException(message); } }

    private static void CheckHitRegion(FontFamily font, string text)
    {
        const double width = 308D;
        const double height = 35.78666666666667D;
        TextBlock before = new() { Text = text, FontFamily = font, FontSize = 15D, FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap };
        OpenVisionWorkbenchNameText after = new() { Text = text, FontFamily = font, FontSize = 15D, FontWeight = FontWeights.Bold };
        Grid root = new() { Width = width, Height = height * 2D };
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(height) });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(height) });
        root.Children.Add(before); root.Children.Add(after); Grid.SetRow(after, 1);
        // A PresentationSource makes InputHitTest meaningful; disconnected controls return null.
        using HwndSource source = new(new HwndSourceParameters("Owned hidden name hit contract") { Width = 308, Height = 72, WindowStyle = unchecked((int)0x80000000) });
        source.RootVisual = root;
        root.Measure(new Size(width, height * 2D)); root.Arrange(new Rect(0D, 0D, width, height * 2D)); root.UpdateLayout();
        new RenderTargetBitmap(308, 72, 96D, 96D, PixelFormats.Pbgra32).Render(root);
        Require(before.IsVisible && after.IsVisible && Math.Abs(before.ActualHeight - height) < 0.001D && Math.Abs(after.ActualHeight - height) < 0.001D, "connected two-line hit fixture");
        foreach (Point point in new[] { new Point(154D, height / 2D), new Point(154D, 18D), new Point(10D, 8D), new Point(307D, 8D), new Point(307D, 26D), new Point(5D, 18D), new Point(1D, 35D), new Point(307D, 35D) })
        {
            Require(ReferenceEquals(before.InputHitTest(point), before) && ReferenceEquals(after.InputHitTest(point), after)
                && ReferenceEquals(VisualTreeHelper.HitTest(before, point)?.VisualHit, before) && ReferenceEquals(VisualTreeHelper.HitTest(after, point)?.VisualHit, after), "same full rectangular title InputHitTest/VisualHit: " + point);
        }
        source.RootVisual = null;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateDetachedReference(Model model)
    {
        OpenVisionWorkbenchNameText text = new();
        BindingOperations.SetBinding(text, TextBlock.TextProperty, new Binding(nameof(Model.Title)) { Source = model });
        text.Measure(new Size(180D, double.PositiveInfinity));
        BindingOperations.ClearAllBindings(text);
        return new WeakReference(text);
    }

    private sealed class Model : INotifyPropertyChanged
    {
        private string title = string.Empty;
        public event PropertyChangedEventHandler? PropertyChanged;
        public string Title { get => title; set { title = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title))); } }
    }
}
