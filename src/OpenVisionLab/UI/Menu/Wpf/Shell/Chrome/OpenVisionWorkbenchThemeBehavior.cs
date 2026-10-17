using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace OpenVisionLab
{
    // Presentation adapter for independently scoped Main dictionaries; no application-wide theme mutation.
    public static class OpenVisionWorkbenchThemeBehavior
    {
        public static readonly DependencyProperty ThemeNameProperty = DependencyProperty.RegisterAttached(
            "ThemeName", typeof(string), typeof(OpenVisionWorkbenchThemeBehavior), new PropertyMetadata(null, OnThemeChanged));
        private static readonly DependencyProperty PaletteProperty = DependencyProperty.RegisterAttached(
            "Palette", typeof(ResourceDictionary), typeof(OpenVisionWorkbenchThemeBehavior), new PropertyMetadata(null));

        public static string GetThemeName(DependencyObject element) => (string)element.GetValue(ThemeNameProperty);
        public static void SetThemeName(DependencyObject element, string value) => element.SetValue(ThemeNameProperty, value);

        private static void OnThemeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not FrameworkElement element || e.NewValue is not string theme || (theme != "Dark" && theme != "Light")) { return; }
            ResourceDictionary palette = new() { Source = new Uri($"/OpenVisionLab;component/UI/Wpf/OpenVisionWorkbench{theme}.xaml", UriKind.Relative) };
            ResourceDictionary previous = (ResourceDictionary)element.GetValue(PaletteProperty);
            if (element is VisionToolSingleInputPropertyToolShell or VisionToolDoubleInputCustomToolShell)
            {
                element.Resources.MergedDictionaries.Add(palette);
                if (previous != null) element.Resources.MergedDictionaries.Remove(previous);
                foreach (string name in new[] { "gbInputLayer", "gbInputA", "gbInputB", "gbOutputLayer", "parameterPanel", "actionPanel", "workbenchTemplateHost", "workbenchAcceptanceHost", "workbenchResultHost" })
                {
                    if (element.FindName(name) is not FrameworkElement chrome) continue;
                    chrome.Resources.MergedDictionaries.Add(palette);
                    if (previous != null) chrome.Resources.MergedDictionaries.Remove(previous);
                }
                if (element.FindName("shellRoot") is Panel root) root.Background = (Brush)palette["OpenVision.Workbench.PanelBrush"];
                if (element.FindName("parameterPanel") is Panel parameters) parameters.Background = (Brush)palette["OpenVision.Workbench.PanelBrush"];
                element.SetValue(PaletteProperty, palette);
                return;
            }
            element.Resources.MergedDictionaries.Add(palette);
            if (previous != null) { element.Resources.MergedDictionaries.Remove(previous); }
            element.SetValue(PaletteProperty, palette);
            if (element is OpenVisionLab.Logging.Controls.View.LogPanelView)
            {
                foreach ((string localKey, string semanticKey) in new (string, string)[]
                {
                    ("ShellBrush", "PanelBrush"),
                    ("PanelBrush", "RaisedBrush"),
                    ("ListBrush", "PanelBrush"),
                    ("RowBrush", "PanelAltBrush"),
                    ("LineBrush", "LineBrush"),
                    ("PrimaryTextBrush", "TextBrush"),
                    ("SecondaryTextBrush", "SecondaryTextBrush"),
                    ("MutedTextBrush", "MutedTextBrush"),
                    ("ActionBrush", "RaisedBrush"),
                    ("ActionBorderBrush", "FieldBorderBrush"),
                    ("InputBrush", "PanelBrush"),
                    ("BadgeBrush", "PanelAltBrush"),
                    ("ComboArrowBrush", "SecondaryTextBrush"),
                    ("ComboHoverBrush", "HoverBrush"),
                    ("FieldBorderBrush", "FieldBorderBrush"),
                    ("DisabledBackgroundBrush", "PanelAltBrush"),
                    ("DisabledBorderBrush", "FieldBorderBrush"),
                    ("ButtonHoverBrush", "HoverBrush"),
                    ("ButtonPressedBrush", "PressedBrush"),
                    ("FocusBorderBrush", "AccentBrush"),
                    ("ButtonDisabledBrush", "PanelAltBrush"),
                    ("BadgeBorderBrush", "LineBrush"),
                    ("RowBorderBrush", "LineBrush"),
                    ("SourceTextBrush", "DoneBrush"),
                    ("EmptyBackgroundBrush", "PanelAltBrush"),
                    ("EmptyBorderBrush", "AccentLineBrush"),
                    ("EmptyIconBackgroundBrush", "RaisedBrush"),
                    ("InfoToneBrush", "TextBrush"),
                    ("WarningToneBrush", "StaleBrush"),
                    ("ErrorToneBrush", "NgBrush"),
                    ("DebugToneBrush", "SecondaryTextBrush"),
                    ("OpenVision.ControlFocusBrush", "AccentBrush"),
                    ("OpenVision.ControlDisabledBrush", "PanelAltBrush"),
                    ("OpenVision.ControlDisabledTextBrush", "MutedTextBrush"),
                    ("OpenVision.ControlDisabledBorderBrush", "FieldBorderBrush"),
                    ("OpenVision.ControlErrorBrush", "NgBrush"),
                }) element.Resources[localKey] = palette["OpenVision.Workbench." + semanticKey];
                element.Resources[typeof(TextBox)] = element.FindResource("ShellHost.MainLogSearchStyle");
                element.Resources[typeof(CheckBox)] = element.FindResource("ShellHost.MainLogCheckBoxStyle");
                element.Resources["FilterLabelDisabledOpacity"] = 1D;
                if (element.FindName("logHeaderText") is TextBlock header && header.Parent is DockPanel bar && bar.Children.IndexOf(header) == 3)
                {
                    bar.Children.Remove(header);
                    DockPanel.SetDock(header, Dock.Left);
                    header.Margin = new Thickness(0, 0, 8, 0);
                    bar.Children.Insert(2, header);
                }
                if (element.FindName("emptyLogState") is Border empty && empty.Parent is Grid body)
                {
                    body.Children.Remove(empty);
                    ScrollViewer scroll = new() { Name = "emptyLogScroller", Content = empty, VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled, VerticalContentAlignment = VerticalAlignment.Center };
                    scroll.SetBinding(UIElement.VisibilityProperty, new Binding(nameof(empty.Visibility)) { Source = empty });
                    element.RegisterName(scroll.Name, scroll);
                    body.Children.Add(scroll);
                }
            }

        }
    }
}
