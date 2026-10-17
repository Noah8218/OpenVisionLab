extern alias WpfPropertyGridOriginal;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OpenVisionLab;
using OpenVisionLab.PropertyGrid;

namespace System.Windows.Controls.WpfPropertyGrid
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class CategoryOrderAttribute : Attribute
    {
        public CategoryOrderAttribute(string categoryName, int order)
        {
            CategoryName = categoryName;
            Order = order;
        }

        public string CategoryName { get; }
        public int Order { get; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class PropertyOrderAttribute : Attribute
    {
        public PropertyOrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class PropertyEditorAttribute : Attribute
    {
        public PropertyEditorAttribute(Type editorType)
        {
            EditorType = editorType;
        }

        public Type EditorType { get; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class PropertyGridCompatibilityReadOnlyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class NumberRangeAttribute : Attribute
    {
        public NumberRangeAttribute(double minimum, double maximum, double tick)
            : this(minimum, maximum, tick, 0)
        {
        }

        public NumberRangeAttribute(double minimum, double maximum, double tick, double precision)
        {
            Minimum = minimum;
            Maximum = maximum;
            Tick = tick;
            Precision = precision;
        }

        public double Minimum { get; }
        public double Maximum { get; }
        public double Tick { get; }
        public double Precision { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ThresholdEditorAttribute : Attribute
    {
        public ThresholdEditorAttribute(double minimum, double maximum, double tick)
            : this(minimum, maximum, tick, 0, null)
        {
        }

        public ThresholdEditorAttribute(double minimum, double maximum, double tick, double precision)
            : this(minimum, maximum, tick, precision, null)
        {
        }

        public ThresholdEditorAttribute(double minimum, double maximum, double tick, double precision, string invertPropertyName)
        {
            Minimum = minimum;
            Maximum = maximum;
            Tick = tick;
            Precision = precision;
            InvertPropertyName = invertPropertyName;
        }

        public double Minimum { get; }
        public double Maximum { get; }
        public double Tick { get; }
        public double Precision { get; }
        public string InvertPropertyName { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class RangeEditorAttribute : Attribute
    {
        private const string NoInvertPropertyName = "__OpenVisionRangeEditorNoInvert__";

        public RangeEditorAttribute(double minimum, double maximum, double tick, string minPropertyName, string maxPropertyName)
            : this(minimum, maximum, tick, 0, minPropertyName, maxPropertyName, NoInvertPropertyName)
        {
        }

        public RangeEditorAttribute(double minimum, double maximum, double tick, double precision, string minPropertyName, string maxPropertyName)
            : this(minimum, maximum, tick, precision, minPropertyName, maxPropertyName, NoInvertPropertyName)
        {
        }

        public RangeEditorAttribute(double minimum, double maximum, double tick, double precision, string minPropertyName, string maxPropertyName, string invertPropertyName)
        {
            Minimum = minimum;
            Maximum = maximum;
            Tick = tick;
            Precision = precision;
            MinPropertyName = minPropertyName;
            MaxPropertyName = maxPropertyName;
            InvertPropertyName = string.IsNullOrEmpty(invertPropertyName)
                ? NoInvertPropertyName
                : invertPropertyName;
        }

        public double Minimum { get; }
        public double Maximum { get; }
        public double Tick { get; }
        public double Precision { get; }
        public string MinPropertyName { get; }
        public string MaxPropertyName { get; }
        public string InvertPropertyName { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class MetricRangeEditorAttribute : Attribute
    {
        public MetricRangeEditorAttribute(string useMinPropertyName, string minPropertyName, string useMaxPropertyName, string maxPropertyName)
            : this(3, useMinPropertyName, minPropertyName, useMaxPropertyName, maxPropertyName)
        {
        }

        public MetricRangeEditorAttribute(double precision, string useMinPropertyName, string minPropertyName, string useMaxPropertyName, string maxPropertyName)
        {
            Precision = precision;
            UseMinPropertyName = useMinPropertyName;
            MinPropertyName = minPropertyName;
            UseMaxPropertyName = useMaxPropertyName;
            MaxPropertyName = maxPropertyName;
        }

        public double Precision { get; }
        public string UseMinPropertyName { get; }
        public string MinPropertyName { get; }
        public string UseMaxPropertyName { get; }
        public string MaxPropertyName { get; }
    }

    public enum PropertyGridThemeVariant
    {
        Default,
        Dark
    }

    public sealed class PropertyGridSelectedPropertyChangedEventArgs : EventArgs
    {
        public PropertyGridSelectedPropertyChangedEventArgs(string propertyName)
        {
            PropertyName = propertyName ?? string.Empty;
        }

        public string PropertyName { get; }
    }

    public class PropertyGrid : UserControl, IPropertyGridView
    {
        private readonly WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyGrid innerPropertyGrid;
        private readonly PropertyGridPropertyValueChangeSubscription propertyValueChangeSubscription;
        private readonly Border searchEmptyOverlay;
        private readonly TextBlock searchEmptyMessage;
        private readonly HashSet<string> registeredPropertyEditors = new HashSet<string>();
        private readonly ConditionalWeakTable<DependencyObject, Dictionary<DependencyProperty, object>> darkThemeOriginalValues = new ConditionalWeakTable<DependencyObject, Dictionary<DependencyProperty, object>>();
        private readonly ConditionalWeakTable<Control, Style> appliedDarkStyles = new ConditionalWeakTable<Control, Style>();
        private bool suppressSelectedObjectsChanged;
        private bool suppressPropertyValueChanged;
        private bool suppressRangeEditorChanges;
        private static readonly object originalResourceLock = new object();
        private static bool originalResourcesRegistered;
        private static readonly object browsabilityLock = new object();
        private static readonly HashSet<Type> registeredBrowsableProviderTypes = new HashSet<Type>();
        private static readonly Dictionary<Type, HashSet<string>> hiddenPropertiesByType = new Dictionary<Type, HashSet<string>>();
        private static readonly ConditionalWeakTable<object, ProgressivePropertyViewportState> progressivePropertyViewports = new ConditionalWeakTable<object, ProgressivePropertyViewportState>();
        private static readonly ConditionalWeakTable<object, PropertyGridNavigationState> navigationStates = new ConditionalWeakTable<object, PropertyGridNavigationState>();
        private Func<object, string, bool> childParameterPredicate;
        private static readonly DependencyProperty DialogPropertyValueProperty =
            DependencyProperty.RegisterAttached(
                "DialogPropertyValue",
                typeof(object),
                typeof(PropertyGrid),
                new PropertyMetadata(null));
        private static readonly Lazy<ControlTemplate> SharedComboBoxTemplate =
            new Lazy<ControlTemplate>(() => CreateComboBoxTemplateCore(darkTheme: false));
        private static readonly Lazy<ControlTemplate> SharedDarkComboBoxTemplate =
            new Lazy<ControlTemplate>(() => CreateComboBoxTemplateCore(darkTheme: true));
        private static readonly Lazy<Style> SharedComboBoxItemStyle =
            new Lazy<Style>(() => CreateComboBoxItemStyleCore(compactDensity: false, darkTheme: false));
        private static readonly ControlTemplate DarkSliderFilledTrackTemplate = CreateDarkSliderTrackButtonTemplate(BrushFromRgb(46, 196, 182));
        private static readonly ControlTemplate DarkSliderEmptyTrackTemplate = CreateDarkSliderTrackButtonTemplate(BrushFromRgb(102, 113, 127));
        private static readonly ControlTemplate DarkSliderThumbTemplate = CreateDarkSliderThumbTemplate();
        private static readonly ControlTemplate DarkCheckBoxTemplate = CreateDarkCheckBoxTemplate();
        private static readonly ControlTemplate DarkDialogButtonTemplate = CreateDarkDialogButtonTemplate();
        public static readonly DependencyProperty IsCompactDensityProperty =
            DependencyProperty.Register(
                nameof(IsCompactDensity),
                typeof(bool),
                typeof(PropertyGrid),
                new PropertyMetadata(false, OnIsCompactDensityChanged));
        public static readonly DependencyProperty ThemeVariantProperty =
            DependencyProperty.Register(
                nameof(ThemeVariant),
                typeof(PropertyGridThemeVariant),
                typeof(PropertyGrid),
                new PropertyMetadata(PropertyGridThemeVariant.Default, OnThemeVariantChanged));
        private bool languageChangedSubscribed;
        private bool normalizeScheduled;
        private ScrollViewer horizontalViewport;
        private double editorColumnMinimum = 160D;
        private int normalizationSession;
        private readonly ConditionalWeakTable<TextBlock, object[]> originalNameLayout = new ConditionalWeakTable<TextBlock, object[]>();
        private bool searchFeedbackUpdateScheduled;
        private bool metadataCacheClearPending;
        private string selectedPropertyName = string.Empty;
        private TextBox focusedRangeEditorTextBox;
        private string focusedRangeEditorOriginalText;

        public PropertyGrid()
        {
            EnsureOriginalWpfResources();
            innerPropertyGrid = new WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyGrid();
            innerPropertyGrid.SetBinding(Control.FontFamilyProperty, new Binding(nameof(FontFamily)) { Source = this });
            propertyValueChangeSubscription = new PropertyGridPropertyValueChangeSubscription(
                (property, oldValue, newValue) =>
                {
                    if (!object.Equals(oldValue, newValue)) RaisePropertyValueChanged(property, oldValue, newValue);
                });
            ApplyBridgeDensity();

            Grid contentHost = new Grid();
            contentHost.Children.Add(innerPropertyGrid);
            searchEmptyMessage = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.SemiBold,
                Foreground = BrushFromRgb(66, 91, 112),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            searchEmptyOverlay = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(16, 72, 16, 0),
                Padding = new Thickness(14, 10, 14, 10),
                Background = BrushFromRgb(245, 250, 252),
                BorderBrush = BrushFromRgb(190, 211, 225),
                BorderThickness = new Thickness(1),
                Child = searchEmptyMessage,
                IsHitTestVisible = false,
                Visibility = Visibility.Collapsed
            };
            Panel.SetZIndex(searchEmptyOverlay, 100);
            contentHost.Children.Add(searchEmptyOverlay);
            Content = contentHost;
            ApplySearchFeedbackTheme();

            innerPropertyGrid.PropertyValueChanged += InnerPropertyGrid_PropertyValueChanged;
            innerPropertyGrid.SelectedObjectsChanged += (sender, e) =>
            {
                if (!suppressSelectedObjectsChanged)
                {
                    SelectedObjectsChanged?.Invoke(this, EventArgs.Empty);
                }
            };
            innerPropertyGrid.AddHandler(
                Mouse.PreviewMouseDownEvent,
                new MouseButtonEventHandler(InnerPropertyGrid_PreviewMouseLeftButtonDown),
                true);
            innerPropertyGrid.AddHandler(
                UIElement.GotKeyboardFocusEvent,
                new KeyboardFocusChangedEventHandler(InnerPropertyGrid_GotKeyboardFocus),
                true);

            innerPropertyGrid.PreviewKeyDown += InnerPropertyGrid_PreviewKeyDown;

            SubscribeLanguageChanged();
            Loaded += (sender, e) =>
            {
                SubscribeLanguageChanged();
                normalizationSession++;
                normalizeScheduled = false;
                if (childParameterPredicate != null)
                {
                    for (DependencyObject parent = GetParent(this); parent != null; parent = GetParent(parent))
                    {
                        if (parent is not ScrollViewer viewport || viewport.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled) continue;
                        if (horizontalViewport != null) horizontalViewport.SizeChanged -= HorizontalViewport_SizeChanged;
                        horizontalViewport = viewport;
                        horizontalViewport.SizeChanged += HorizontalViewport_SizeChanged;
                        break;
                    }
                }
                NormalizeInnerEditorControls();
            };
            Unloaded += (sender, e) =>
            {
                UnsubscribeLanguageChanged();
                normalizationSession++;
                normalizeScheduled = false;
                if (horizontalViewport != null) horizontalViewport.SizeChanged -= HorizontalViewport_SizeChanged;
                horizontalViewport = null;
                RestoreToolFormLayout();
            };
            SizeChanged += (sender, e) => { if (IsLoaded && childParameterPredicate != null && horizontalViewport == null && e.WidthChanged) ScheduleNormalizeInnerEditorControls(); };
        }

        public bool IsCompactDensity
        {
            get { return (bool)GetValue(IsCompactDensityProperty); }
            set { SetValue(IsCompactDensityProperty, value); }
        }

        public PropertyGridThemeVariant ThemeVariant
        {
            get { return (PropertyGridThemeVariant)GetValue(ThemeVariantProperty); }
            set { SetValue(ThemeVariantProperty, value); }
        }

        private static void OnIsCompactDensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyGrid grid)
            {
                grid.ApplyBridgeDensity();
            }
        }

        private static void OnThemeVariantChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyGrid grid)
            {
                grid.ApplyBridgeDensity();
                grid.ApplySearchFeedbackTheme();
            }
        }

        private void ApplyBridgeDensity()
        {
            if (innerPropertyGrid == null)
            {
                return;
            }

            // Docked inspectors need higher information density, while floating tools
            // keep the larger editor spacing that operators already verified.
            bool compactDensity = IsCompactDensity;
            bool darkTheme = ThemeVariant == PropertyGridThemeVariant.Dark;
            ApplyBridgeVisualStyle(innerPropertyGrid.Resources, compactDensity, darkTheme);
            ApplyBridgeSurfaceStyle(innerPropertyGrid, compactDensity, darkTheme);
            innerPropertyGrid.ItemsBackground = darkTheme ? BrushFromRgb(20, 23, 29) : BrushFromRgb(255, 255, 255);
            innerPropertyGrid.ItemsForeground = darkTheme ? BrushFromRgb(232, 235, 241) : BrushFromRgb(39, 54, 74);
            innerPropertyGrid.InvalidateMeasure();
            innerPropertyGrid.InvalidateArrange();
            ScheduleNormalizeInnerEditorControls();
        }

        private void ApplySearchFeedbackTheme()
        {
            if (searchEmptyOverlay == null || searchEmptyMessage == null)
            {
                return;
            }

            bool darkTheme = ThemeVariant == PropertyGridThemeVariant.Dark;
            searchEmptyOverlay.Background = darkTheme ? BrushFromRgb(27, 31, 39) : BrushFromRgb(245, 250, 252);
            searchEmptyOverlay.BorderBrush = darkTheme ? BrushFromRgb(102, 113, 127) : BrushFromRgb(190, 211, 225);
            searchEmptyMessage.Foreground = darkTheme ? BrushFromRgb(174, 182, 195) : BrushFromRgb(66, 91, 112);
        }

        private static void EnsureOriginalWpfResources()
        {
            lock (originalResourceLock)
            {
                Application application = Application.Current;
                if (application == null)
                {
                    application = new Application
                    {
                        ShutdownMode = ShutdownMode.OnExplicitShutdown
                    };
                }

                if (originalResourcesRegistered)
                {
                    return;
                }

                TryMergeOriginalResourceDictionary(
                    application.Resources,
                    "/System.Windows.Controls.WpfPropertyGrid;component/Themes/Generic.xaml");
                // Keep only the vendor WPG resources global; bridge visuals stay instance-scoped
                // so tool-layer ComboBoxes keep the normal VisionTool template and selection behavior.
                originalResourcesRegistered = true;
            }
        }

        private static void TryMergeOriginalResourceDictionary(ResourceDictionary resources, string source)
        {
            if (resources == null || string.IsNullOrWhiteSpace(source))
            {
                return;
            }

            foreach (ResourceDictionary dictionary in resources.MergedDictionaries)
            {
                if (dictionary.Source != null
                    && string.Equals(dictionary.Source.OriginalString, source, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            try
            {
                resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri(source, UriKind.Relative)
                });
            }
            catch
            {
                // The fallback category editor below prevents internal type names
                // from leaking even if the original resource dictionary is absent.
            }
        }

        private static void ApplyBridgeVisualStyle(ResourceDictionary resources, bool compactDensity, bool darkTheme)
        {
            if (resources == null)
            {
                return;
            }

            resources[typeof(TextBox)] = CreateTextBoxStyle(compactDensity, darkTheme);
            resources[typeof(ComboBox)] = CreateComboBoxStyle(compactDensity, darkTheme);
            resources[typeof(ComboBoxItem)] = CreateComboBoxItemStyle(compactDensity, darkTheme);
            resources[typeof(CheckBox)] = CreateCheckBoxStyle(compactDensity, darkTheme);
            resources[typeof(Slider)] = CreateSliderStyle(compactDensity, darkTheme);
        }

        private static void ApplyBridgeContainerStyles(ResourceDictionary resources)
        {
        }

        private static Style CreatePropertyContainerStyle(Type containerType, Style basedOn, bool compactDensity)
        {
            Style style = basedOn == null
                ? new Style(containerType)
                : new Style(containerType, basedOn);

            SolidColorBrush panelBrush = BrushFromRgb(255, 255, 255);
            SolidColorBrush surfaceBrush = BrushFromRgb(240, 244, 248);
            SolidColorBrush nameBrush = BrushFromRgb(237, 243, 248);
            SolidColorBrush hoverBrush = BrushFromRgb(243, 248, 253);
            SolidColorBrush lineBrush = BrushFromRgb(221, 230, 239);
            SolidColorBrush accentBrush = BrushFromRgb(47, 111, 171);

            FrameworkElementFactory rowBorder = new FrameworkElementFactory(typeof(Border), "RowBorder");
            rowBorder.SetValue(FrameworkElement.MinHeightProperty, compactDensity ? 30D : 34D);
            rowBorder.SetValue(Border.BackgroundProperty, panelBrush);
            rowBorder.SetValue(Border.BorderBrushProperty, lineBrush);
            rowBorder.SetValue(Border.BorderThicknessProperty, new Thickness(0, 0, 0, 1));
            rowBorder.SetValue(UIElement.SnapsToDevicePixelsProperty, true);

            FrameworkElementFactory rowPanel = new FrameworkElementFactory(typeof(DockPanel));
            rowPanel.SetValue(DockPanel.LastChildFillProperty, true);

            FrameworkElementFactory nameCell = new FrameworkElementFactory(typeof(Border), "NameCell");
            nameCell.SetValue(DockPanel.DockProperty, Dock.Left);
            nameCell.SetValue(FrameworkElement.WidthProperty, 158D);
            nameCell.SetValue(Border.BackgroundProperty, nameBrush);
            nameCell.SetValue(Border.BorderBrushProperty, lineBrush);
            nameCell.SetValue(Border.BorderThicknessProperty, new Thickness(0, 0, 1, 0));

            FrameworkElementFactory namePanel = new FrameworkElementFactory(typeof(DockPanel));
            namePanel.SetValue(DockPanel.LastChildFillProperty, true);

            FrameworkElementFactory accent = new FrameworkElementFactory(typeof(Border), "RowAccent");
            accent.SetValue(DockPanel.DockProperty, Dock.Left);
            accent.SetValue(FrameworkElement.WidthProperty, 3D);
            accent.SetValue(Border.BackgroundProperty, Brushes.Transparent);

            FrameworkElementFactory nameText = new FrameworkElementFactory(
                typeof(WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.Design.PropertyNameTextBlock));
            nameText.SetValue(FrameworkElement.MarginProperty, compactDensity ? new Thickness(6, 0, 8, 0) : new Thickness(8, 0, 10, 0));
            nameText.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            nameText.SetValue(Control.FontSizeProperty, 12D);
            nameText.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
            nameText.SetValue(TextBlock.ForegroundProperty, BrushFromRgb(79, 97, 116));
            nameText.SetBinding(TextBlock.TextProperty, new Binding("DisplayName")
            {
                Mode = BindingMode.OneTime
            });

            namePanel.AppendChild(accent);
            namePanel.AppendChild(nameText);
            nameCell.AppendChild(namePanel);

            FrameworkElementFactory editorCell = new FrameworkElementFactory(typeof(Border), "EditorCell");
            editorCell.SetValue(Border.BackgroundProperty, panelBrush);
            editorCell.SetValue(Border.PaddingProperty, compactDensity ? new Thickness(6, 2, 6, 2) : new Thickness(8, 4, 8, 4));
            editorCell.SetValue(FrameworkElement.MinWidthProperty, 120D);
            editorCell.AppendChild(new FrameworkElementFactory(
                typeof(WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.Design.PropertyEditorContentPresenter)));

            rowPanel.AppendChild(nameCell);
            rowPanel.AppendChild(editorCell);
            rowBorder.AppendChild(rowPanel);

            ControlTemplate template = new ControlTemplate(containerType)
            {
                VisualTree = rowBorder
            };

            Trigger hoverTrigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            hoverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, hoverBrush, "RowBorder"));
            hoverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, hoverBrush, "EditorCell"));
            hoverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, surfaceBrush, "NameCell"));
            hoverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, accentBrush, "RowAccent"));
            template.Triggers.Add(hoverTrigger);

            style.Setters.Add(new Setter(Control.TemplateProperty, template));
            return style;
        }

        private static void ApplyBridgeSurfaceStyle(Control control, bool compactDensity, bool darkTheme)
        {
            if (control == null)
            {
                return;
            }

            control.Background = darkTheme ? BrushFromRgb(22, 25, 32) : BrushFromRgb(238, 244, 250);
            control.Foreground = darkTheme ? BrushFromRgb(232, 235, 241) : BrushFromRgb(39, 54, 74);
            control.BorderBrush = darkTheme ? BrushFromRgb(38, 43, 52) : BrushFromRgb(194, 210, 226);
            control.BorderThickness = new Thickness(1);
            control.Padding = new Thickness(compactDensity ? 1D : 2D);
        }

        private static Style CreateTextBoxStyle(bool compactDensity, bool darkTheme)
        {
            Style style = new Style(typeof(TextBox));
            style.Setters.Add(new Setter(Control.FontSizeProperty, 12D));
            style.Setters.Add(new Setter(Control.ForegroundProperty, darkTheme ? BrushFromRgb(232, 235, 241) : BrushFromRgb(22, 64, 103)));
            style.Setters.Add(new Setter(Control.BackgroundProperty, darkTheme ? BrushFromRgb(20, 23, 29) : BrushFromRgb(250, 252, 253)));
            style.Setters.Add(new Setter(Control.BorderBrushProperty, darkTheme ? BrushFromRgb(88, 101, 119) : BrushFromRgb(175, 197, 221)));
            style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(1)));
            style.Setters.Add(new Setter(Control.PaddingProperty, compactDensity ? new Thickness(6, 2, 6, 2) : new Thickness(8, 3, 8, 3)));
            style.Setters.Add(new Setter(FrameworkElement.MarginProperty, compactDensity ? new Thickness(0, 1, 0, 1) : new Thickness(0, 2, 0, 2)));
            return style;
        }


        private void ApplyDarkControlStateStyle(Control control)
        {
            if (appliedDarkStyles.TryGetValue(control, out Style applied) && ReferenceEquals(control.Style, applied)) return;
            Dictionary<DependencyProperty, object> originals = darkThemeOriginalValues.GetOrCreateValue(control);
            BindingBase binding = BindingOperations.GetBindingBase(control, FrameworkElement.StyleProperty);
            originals[FrameworkElement.StyleProperty] = binding != null ? (object)binding
                : control.ReadLocalValue(FrameworkElement.StyleProperty) == DependencyProperty.UnsetValue ? DependencyProperty.UnsetValue : control.Style;
            Style style = control is TextBox ? CreateDarkTextBoxStyle(control.Style) : CreateDarkControlStateStyle(control);
            control.SetCurrentValue(FrameworkElement.StyleProperty, style);
            appliedDarkStyles.Remove(control);
            appliedDarkStyles.Add(control, style);
        }

        private static Style CreateDarkControlStateStyle(Control control)
        {
            Style style = new Style(control.Style?.TargetType ?? control.GetType(), control.Style);
            style.Setters.Add(new Setter(Control.ForegroundProperty, BrushFromRgb(232, 235, 241)));
            style.Setters.Add(new Setter(UIElement.OpacityProperty, 1D));
            if (control is ToggleButton && control.Name == "ExpanderToggleButton")
            {
                FrameworkElementFactory cue = new FrameworkElementFactory(typeof(Border));
                cue.SetValue(Border.BorderBrushProperty, BrushFromRgb(46, 196, 182));
                cue.SetValue(Border.BorderThicknessProperty, new Thickness(1));
                cue.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
                cue.SetValue(UIElement.IsHitTestVisibleProperty, false);
                Style focusVisual = new Style(typeof(Control));
                focusVisual.Setters.Add(new Setter(Control.TemplateProperty, new ControlTemplate(typeof(Control)) { VisualTree = cue }));
                style.Setters.Add(new Setter(Control.FocusVisualStyleProperty, focusVisual));
            }
            Trigger disabled = new Trigger { Property = UIElement.IsEnabledProperty, Value = false };
            disabled.Setters.Add(new Setter(Control.ForegroundProperty, BrushFromRgb(133, 142, 160)));
            disabled.Setters.Add(new Setter(UIElement.OpacityProperty, 1D));
            style.Triggers.Add(disabled);
            return style;
        }

        private static Style CreateDarkTextBoxStyle(Style basedOn)
        {
            Style style = new Style(basedOn?.TargetType ?? typeof(TextBox), basedOn);
            style.Setters.Add(new Setter(Control.ForegroundProperty, BrushFromRgb(232, 235, 241)));
            style.Setters.Add(new Setter(Control.BackgroundProperty, BrushFromRgb(14, 17, 21)));
            style.Setters.Add(new Setter(Control.BorderBrushProperty, BrushFromRgb(88, 101, 119)));
            Trigger focus = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
            focus.Setters.Add(new Setter(Control.BorderBrushProperty, BrushFromRgb(46, 196, 182)));
            style.Triggers.Add(focus);
            Trigger readOnly = new Trigger { Property = TextBox.IsReadOnlyProperty, Value = true };
            readOnly.Setters.Add(new Setter(Control.BackgroundProperty, BrushFromRgb(26, 30, 37)));
            readOnly.Setters.Add(new Setter(Control.ForegroundProperty, BrushFromRgb(133, 142, 160)));
            style.Triggers.Add(readOnly);
            Trigger disabled = new Trigger { Property = UIElement.IsEnabledProperty, Value = false };
            disabled.Setters.Add(new Setter(Control.ForegroundProperty, BrushFromRgb(133, 142, 160)));
            disabled.Setters.Add(new Setter(Control.BackgroundProperty, BrushFromRgb(26, 30, 37)));
            disabled.Setters.Add(new Setter(Control.BorderBrushProperty, BrushFromRgb(88, 101, 119)));
            disabled.Setters.Add(new Setter(UIElement.OpacityProperty, 1D));
            style.Triggers.Add(disabled);
            Trigger error = new Trigger { Property = Validation.HasErrorProperty, Value = true };
            error.Setters.Add(new Setter(Control.BorderBrushProperty, BrushFromRgb(255, 108, 122)));
            style.Triggers.Add(error);
            return style;
        }

        private static Style CreateComboBoxStyle(bool compactDensity, bool darkTheme)
        {
            Style style = new Style(typeof(ComboBox));
            style.Setters.Add(new Setter(Control.FontSizeProperty, 12D));
            style.Setters.Add(new Setter(Control.ForegroundProperty, darkTheme ? BrushFromRgb(232, 235, 241) : BrushFromRgb(22, 64, 103)));
            style.Setters.Add(new Setter(Control.BackgroundProperty, darkTheme ? BrushFromRgb(20, 23, 29) : BrushFromRgb(250, 252, 253)));
            style.Setters.Add(new Setter(Control.BorderBrushProperty, darkTheme ? BrushFromRgb(102, 113, 127) : BrushFromRgb(175, 197, 221)));
            style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(1)));
            style.Setters.Add(new Setter(Control.PaddingProperty, compactDensity ? new Thickness(6, 2, 6, 2) : new Thickness(8, 3, 8, 3)));
            style.Setters.Add(new Setter(FrameworkElement.MarginProperty, compactDensity ? new Thickness(0, 1, 0, 1) : new Thickness(0, 2, 0, 2)));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, compactDensity ? 28D : 30D));
            style.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
            style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
            style.Setters.Add(new Setter(ComboBox.MaxDropDownHeightProperty, 260D));
            style.Setters.Add(new Setter(ItemsControl.ItemContainerStyleProperty, CreateComboBoxItemStyle(compactDensity, darkTheme)));
            style.Setters.Add(new Setter(Control.TemplateProperty, CreateComboBoxTemplate(darkTheme)));
            return style;
        }

        private static ControlTemplate CreateComboBoxTemplate(bool darkTheme)
        {
            return darkTheme ? SharedDarkComboBoxTemplate.Value : SharedComboBoxTemplate.Value;
        }

        private static ControlTemplate CreateComboBoxTemplateCore(bool darkTheme)
        {
            SolidColorBrush backgroundBrush = darkTheme ? BrushFromRgb(20, 23, 29) : BrushFromRgb(250, 252, 253);
            SolidColorBrush hoverBrush = darkTheme ? BrushFromRgb(42, 48, 59) : BrushFromRgb(242, 248, 252);
            SolidColorBrush borderBrush = darkTheme ? BrushFromRgb(102, 113, 127) : BrushFromRgb(175, 197, 221);
            SolidColorBrush focusBrush = darkTheme ? BrushFromRgb(46, 196, 182) : BrushFromRgb(47, 111, 171);
            SolidColorBrush textBrush = darkTheme ? BrushFromRgb(232, 235, 241) : BrushFromRgb(22, 64, 103);
            SolidColorBrush popupBrush = darkTheme ? BrushFromRgb(27, 31, 39) : BrushFromRgb(255, 255, 255);

            FrameworkElementFactory root = new FrameworkElementFactory(typeof(Grid));
            root.SetValue(UIElement.SnapsToDevicePixelsProperty, true);
            root.SetValue(UIElement.ClipToBoundsProperty, true);

            FrameworkElementFactory chrome = new FrameworkElementFactory(typeof(Border), "ComboChrome");
            chrome.SetValue(Border.BackgroundProperty, backgroundBrush);
            chrome.SetValue(Border.BorderBrushProperty, borderBrush);
            chrome.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            chrome.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
            chrome.SetValue(UIElement.ClipToBoundsProperty, true);

            FrameworkElementFactory contentGrid = new FrameworkElementFactory(typeof(Grid));
            contentGrid.SetValue(UIElement.ClipToBoundsProperty, true);
            FrameworkElementFactory textColumn = new FrameworkElementFactory(typeof(ColumnDefinition));
            textColumn.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
            FrameworkElementFactory arrowColumn = new FrameworkElementFactory(typeof(ColumnDefinition));
            arrowColumn.SetValue(ColumnDefinition.WidthProperty, new GridLength(28D));
            contentGrid.AppendChild(textColumn);
            contentGrid.AppendChild(arrowColumn);

            FrameworkElementFactory selectionText = new FrameworkElementFactory(typeof(TextBlock), "PART_SelectionText");
            selectionText.SetValue(FrameworkElement.MarginProperty, new Thickness(10, 0, 4, 0));
            selectionText.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            selectionText.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            selectionText.SetValue(UIElement.IsHitTestVisibleProperty, false);
            if (darkTheme) selectionText.SetBinding(TextBlock.ForegroundProperty, new Binding("Foreground") { RelativeSource = RelativeSource.TemplatedParent });
            else selectionText.SetValue(TextBlock.ForegroundProperty, textBrush);
            selectionText.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
            selectionText.SetValue(TextBlock.TextWrappingProperty, TextWrapping.NoWrap);
            selectionText.SetBinding(TextBlock.TextProperty, new Binding("SelectionBoxItem") { RelativeSource = RelativeSource.TemplatedParent });
            contentGrid.AppendChild(selectionText);

            FrameworkElementFactory arrow = new FrameworkElementFactory(typeof(System.Windows.Shapes.Path));
            arrow.SetValue(Grid.ColumnProperty, 1);
            arrow.SetValue(FrameworkElement.WidthProperty, 8D);
            arrow.SetValue(FrameworkElement.HeightProperty, 5D);
            arrow.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            arrow.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            if (darkTheme) arrow.SetBinding(System.Windows.Shapes.Path.FillProperty, new Binding("Foreground") { RelativeSource = RelativeSource.TemplatedParent });
            else arrow.SetValue(System.Windows.Shapes.Path.FillProperty, textBrush);
            arrow.SetValue(System.Windows.Shapes.Path.DataProperty, Geometry.Parse("M 0 0 L 4 5 L 8 0 Z"));
            contentGrid.AppendChild(arrow);

            chrome.AppendChild(contentGrid);
            root.AppendChild(chrome);

            FrameworkElementFactory popup = new FrameworkElementFactory(typeof(Popup), "PART_Popup");
            popup.SetValue(Popup.AllowsTransparencyProperty, true);
            popup.SetValue(Popup.FocusableProperty, false);
            popup.SetValue(Popup.PlacementProperty, PlacementMode.Bottom);
            popup.SetBinding(Popup.PlacementTargetProperty, new Binding
            {
                RelativeSource = RelativeSource.TemplatedParent
            });
            popup.SetBinding(Popup.IsOpenProperty, new Binding("IsDropDownOpen")
            {
                RelativeSource = RelativeSource.TemplatedParent,
                Mode = BindingMode.TwoWay
            });
            popup.SetBinding(FrameworkElement.MinWidthProperty, new Binding("ActualWidth")
            {
                RelativeSource = RelativeSource.TemplatedParent
            });

            FrameworkElementFactory popupBorder = new FrameworkElementFactory(typeof(Border));
            popupBorder.SetValue(Border.BackgroundProperty, popupBrush);
            popupBorder.SetValue(Border.BorderBrushProperty, focusBrush);
            popupBorder.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            popupBorder.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
            popupBorder.SetValue(FrameworkElement.MaxHeightProperty, 260D);

            FrameworkElementFactory scrollViewer = new FrameworkElementFactory(typeof(ScrollViewer));
            scrollViewer.SetValue(ScrollViewer.CanContentScrollProperty, true);
            scrollViewer.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
            scrollViewer.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);

            FrameworkElementFactory itemsPresenter = new FrameworkElementFactory(typeof(ItemsPresenter));
            scrollViewer.AppendChild(itemsPresenter);
            popupBorder.AppendChild(scrollViewer);
            popup.AppendChild(popupBorder);
            root.AppendChild(popup);

            ControlTemplate template = new ControlTemplate(typeof(ComboBox))
            {
                VisualTree = root
            };

            Trigger hoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
            hoverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, hoverBrush, "ComboChrome"));
            hoverTrigger.Setters.Add(new Setter(Border.BorderBrushProperty, focusBrush, "ComboChrome"));
            template.Triggers.Add(hoverTrigger);

            Trigger focusTrigger = new Trigger { Property = UIElement.IsKeyboardFocusWithinProperty, Value = true };
            focusTrigger.Setters.Add(new Setter(Border.BorderBrushProperty, focusBrush, "ComboChrome"));
            template.Triggers.Add(focusTrigger);

            Trigger openTrigger = new Trigger { Property = ComboBox.IsDropDownOpenProperty, Value = true };
            openTrigger.Setters.Add(new Setter(Border.BorderBrushProperty, focusBrush, "ComboChrome"));
            template.Triggers.Add(openTrigger);

            Trigger disabledTrigger = new Trigger { Property = UIElement.IsEnabledProperty, Value = false };
            if (darkTheme)
            {
                disabledTrigger.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(26, 30, 37), "ComboChrome"));
                disabledTrigger.Setters.Add(new Setter(Border.BorderBrushProperty, BrushFromRgb(102, 113, 127), "ComboChrome"));
                disabledTrigger.Setters.Add(new Setter(Control.ForegroundProperty, BrushFromRgb(133, 142, 160)));
            }
            else disabledTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.55D, "ComboChrome"));
            template.Triggers.Add(disabledTrigger);

            return template;
        }

        private static Style CreateComboBoxItemStyle(bool compactDensity, bool darkTheme)
        {
            return darkTheme || compactDensity
                ? CreateComboBoxItemStyleCore(compactDensity, darkTheme)
                : SharedComboBoxItemStyle.Value;
        }

        private static Style CreateComboBoxItemStyleCore(bool compactDensity, bool darkTheme)
        {
            Style style = new Style(typeof(ComboBoxItem));
            style.Setters.Add(new Setter(Control.FontSizeProperty, 12D));
            style.Setters.Add(new Setter(Control.ForegroundProperty, darkTheme ? BrushFromRgb(232, 235, 241) : BrushFromRgb(22, 64, 103)));
            style.Setters.Add(new Setter(Control.BackgroundProperty, darkTheme ? BrushFromRgb(27, 31, 39) : BrushFromRgb(250, 252, 253)));
            style.Setters.Add(new Setter(Control.PaddingProperty, compactDensity ? new Thickness(8, 4, 8, 4) : new Thickness(9, 6, 9, 6)));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, compactDensity ? 28D : 32D));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, compactDensity ? 28D : 32D));
            style.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
            style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
            if (darkTheme) style.Setters.Add(new Setter(Control.TemplateProperty, CreateDarkComboBoxItemTemplate()));
            return style;
        }

        private static ControlTemplate CreateDarkComboBoxItemTemplate()
        {
            FrameworkElementFactory chrome = new FrameworkElementFactory(typeof(Border), "ItemChrome");
            chrome.SetBinding(Border.BackgroundProperty, new Binding("Background") { RelativeSource = RelativeSource.TemplatedParent });
            chrome.SetValue(Border.BorderBrushProperty, Brushes.Transparent);
            chrome.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            FrameworkElementFactory content = new FrameworkElementFactory(typeof(ContentPresenter));
            content.SetValue(ContentPresenter.ContentSourceProperty, "Content");
            content.SetValue(ContentPresenter.RecognizesAccessKeyProperty, true);
            content.SetBinding(FrameworkElement.MarginProperty, new Binding("Padding") { RelativeSource = RelativeSource.TemplatedParent });
            content.SetBinding(FrameworkElement.HorizontalAlignmentProperty, new Binding("HorizontalContentAlignment") { RelativeSource = RelativeSource.TemplatedParent });
            content.SetBinding(FrameworkElement.VerticalAlignmentProperty, new Binding("VerticalContentAlignment") { RelativeSource = RelativeSource.TemplatedParent });
            chrome.AppendChild(content);
            ControlTemplate template = new ControlTemplate(typeof(ComboBoxItem)) { VisualTree = chrome };
            foreach (DependencyProperty state in new[] { ComboBoxItem.IsHighlightedProperty, Selector.IsSelectedProperty })
            {
                Trigger highlight = new Trigger { Property = state, Value = true };
                highlight.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(30, 54, 59), "ItemChrome"));
                highlight.Setters.Add(new Setter(Border.BorderBrushProperty, BrushFromRgb(46, 196, 182), "ItemChrome"));
                template.Triggers.Add(highlight);
            }
            Trigger disabled = new Trigger { Property = UIElement.IsEnabledProperty, Value = false };
            disabled.Setters.Add(new Setter(Control.ForegroundProperty, BrushFromRgb(133, 142, 160)));
            disabled.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(26, 30, 37), "ItemChrome"));
            template.Triggers.Add(disabled);
            return template;
        }

        private static Style CreateCheckBoxStyle(bool compactDensity, bool darkTheme)
        {
            Style style = new Style(typeof(CheckBox));
            style.Setters.Add(new Setter(Control.FontSizeProperty, 12D));
            style.Setters.Add(new Setter(Control.ForegroundProperty, darkTheme ? BrushFromRgb(232, 235, 241) : BrushFromRgb(22, 64, 103)));
            style.Setters.Add(new Setter(FrameworkElement.MarginProperty, compactDensity ? new Thickness(0, 2, 0, 2) : new Thickness(0, 4, 0, 4)));
            return style;
        }

        private static Style CreateSliderStyle(bool compactDensity, bool darkTheme)
        {
            Style style = new Style(typeof(Slider));
            style.Setters.Add(new Setter(Control.ForegroundProperty, darkTheme ? BrushFromRgb(46, 196, 182) : BrushFromRgb(47, 111, 171)));
            style.Setters.Add(new Setter(Control.BackgroundProperty, darkTheme ? BrushFromRgb(38, 43, 52) : BrushFromRgb(218, 230, 241)));
            style.Setters.Add(new Setter(FrameworkElement.MarginProperty, compactDensity ? new Thickness(0, 3, 6, 3) : new Thickness(0, 6, 8, 6)));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, compactDensity ? 28D : 34D));
            return style;
        }


        private static ControlTemplate CreateDarkCheckBoxTemplate()
        {
            FrameworkElementFactory panel = new FrameworkElementFactory(typeof(DockPanel));
            FrameworkElementFactory chrome = new FrameworkElementFactory(typeof(Border), "CheckChrome");
            chrome.SetValue(DockPanel.DockProperty, Dock.Left);
            chrome.SetValue(FrameworkElement.WidthProperty, 13D);
            chrome.SetValue(FrameworkElement.HeightProperty, 13D);
            chrome.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            chrome.SetValue(Border.CornerRadiusProperty, new CornerRadius(2));
            chrome.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            chrome.SetValue(Border.BackgroundProperty, BrushFromRgb(14, 17, 21));
            chrome.SetValue(Border.BorderBrushProperty, BrushFromRgb(102, 113, 127));
            FrameworkElementFactory mark = new FrameworkElementFactory(typeof(System.Windows.Shapes.Path), "CheckMark");
            mark.SetValue(FrameworkElement.WidthProperty, 8D);
            mark.SetValue(FrameworkElement.HeightProperty, 6D);
            mark.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            mark.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            mark.SetValue(System.Windows.Shapes.Path.DataProperty, Geometry.Parse("M 0 3 L 3 6 L 8 0"));
            mark.SetValue(System.Windows.Shapes.Path.StrokeProperty, BrushFromRgb(10, 26, 24));
            mark.SetValue(System.Windows.Shapes.Path.StrokeThicknessProperty, 1.6D);
            mark.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);
            chrome.AppendChild(mark);
            panel.AppendChild(chrome);
            FrameworkElementFactory content = new FrameworkElementFactory(typeof(ContentPresenter), "CheckContent");
            content.SetValue(ContentPresenter.ContentSourceProperty, "Content");
            content.SetValue(FrameworkElement.MarginProperty, new Thickness(4, 0, 0, 0));
            content.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            panel.AppendChild(content);
            ControlTemplate template = new ControlTemplate(typeof(CheckBox)) { VisualTree = panel };
            Trigger noContent = new Trigger { Property = ContentControl.HasContentProperty, Value = false };
            noContent.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed, "CheckContent"));
            template.Triggers.Add(noContent);
            Trigger hover = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
            hover.Setters.Add(new Setter(Border.BorderBrushProperty, BrushFromRgb(46, 196, 182), "CheckChrome"));
            template.Triggers.Add(hover);
            Trigger focus = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
            focus.Setters.Add(new Setter(Border.BorderBrushProperty, BrushFromRgb(46, 196, 182), "CheckChrome"));
            template.Triggers.Add(focus);
            Trigger check = new Trigger { Property = ToggleButton.IsCheckedProperty, Value = true };
            check.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(46, 196, 182), "CheckChrome"));
            check.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Visible, "CheckMark"));
            template.Triggers.Add(check);
            Trigger indeterminate = new Trigger { Property = ToggleButton.IsCheckedProperty, Value = null };
            indeterminate.Setters.Add(new Setter(System.Windows.Shapes.Path.DataProperty, Geometry.Parse("M 0 3 L 8 3"), "CheckMark"));
            indeterminate.Setters.Add(new Setter(System.Windows.Shapes.Path.StrokeProperty, BrushFromRgb(232, 235, 241), "CheckMark"));
            indeterminate.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Visible, "CheckMark"));
            template.Triggers.Add(indeterminate);
            Trigger disabled = new Trigger { Property = UIElement.IsEnabledProperty, Value = false };
            disabled.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(26, 30, 37), "CheckChrome"));
            disabled.Setters.Add(new Setter(Border.BorderBrushProperty, BrushFromRgb(102, 113, 127), "CheckChrome"));
            disabled.Setters.Add(new Setter(System.Windows.Shapes.Path.StrokeProperty, BrushFromRgb(133, 142, 160), "CheckMark"));
            template.Triggers.Add(disabled);
            return template;
        }

        private static ControlTemplate CreateDarkDialogButtonTemplate()
        {
            FrameworkElementFactory chrome = new FrameworkElementFactory(typeof(Border), "DialogChrome");
            chrome.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
            chrome.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            chrome.SetBinding(Border.BackgroundProperty, new Binding("Background") { RelativeSource = RelativeSource.TemplatedParent });
            chrome.SetBinding(Border.BorderBrushProperty, new Binding("BorderBrush") { RelativeSource = RelativeSource.TemplatedParent });
            FrameworkElementFactory content = new FrameworkElementFactory(typeof(ContentPresenter));
            content.SetValue(ContentPresenter.ContentSourceProperty, "Content");
            content.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            content.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            chrome.AppendChild(content);
            ControlTemplate template = new ControlTemplate(typeof(Button)) { VisualTree = chrome };
            Trigger hover = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
            hover.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(37, 42, 52), "DialogChrome"));
            hover.Setters.Add(new Setter(Border.BorderBrushProperty, BrushFromRgb(46, 196, 182), "DialogChrome"));
            template.Triggers.Add(hover);
            Trigger pressed = new Trigger { Property = ButtonBase.IsPressedProperty, Value = true };
            pressed.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(42, 48, 59), "DialogChrome"));
            pressed.Setters.Add(new Setter(Border.BorderBrushProperty, BrushFromRgb(46, 196, 182), "DialogChrome"));
            template.Triggers.Add(pressed);
            Trigger focus = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
            focus.Setters.Add(new Setter(Border.BorderBrushProperty, BrushFromRgb(46, 196, 182), "DialogChrome"));
            template.Triggers.Add(focus);
            Trigger disabled = new Trigger { Property = UIElement.IsEnabledProperty, Value = false };
            disabled.Setters.Add(new Setter(Control.ForegroundProperty, BrushFromRgb(133, 142, 160)));
            disabled.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(26, 30, 37), "DialogChrome"));
            template.Triggers.Add(disabled);
            return template;
        }

        private static ControlTemplate CreateDarkSliderTrackButtonTemplate(Brush background)
        {
            FrameworkElementFactory line = new FrameworkElementFactory(typeof(Border), "TrackLine");
            line.SetValue(FrameworkElement.HeightProperty, 4D);
            line.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            line.SetValue(Border.CornerRadiusProperty, new CornerRadius(2));
            line.SetValue(Border.BackgroundProperty, background);
            ControlTemplate template = new ControlTemplate(typeof(RepeatButton)) { VisualTree = line };
            Trigger hover = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
            hover.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(34, 168, 155), "TrackLine"));
            template.Triggers.Add(hover);
            Trigger pressed = new Trigger { Property = ButtonBase.IsPressedProperty, Value = true };
            pressed.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(46, 196, 182), "TrackLine"));
            template.Triggers.Add(pressed);
            Trigger disabled = new Trigger { Property = UIElement.IsEnabledProperty, Value = false };
            disabled.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(102, 113, 127), "TrackLine"));
            template.Triggers.Add(disabled);
            return template;
        }

        private static ControlTemplate CreateDarkSliderThumbTemplate()
        {
            FrameworkElementFactory chrome = new FrameworkElementFactory(typeof(Border), "ThumbChrome");
            chrome.SetValue(FrameworkElement.WidthProperty, 12D);
            chrome.SetValue(FrameworkElement.HeightProperty, 18D);
            chrome.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
            chrome.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            chrome.SetValue(Border.BackgroundProperty, BrushFromRgb(46, 196, 182));
            chrome.SetValue(Border.BorderBrushProperty, BrushFromRgb(102, 113, 127));
            ControlTemplate template = new ControlTemplate(typeof(Thumb)) { VisualTree = chrome };
            Trigger hover = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
            hover.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(34, 168, 155), "ThumbChrome"));
            template.Triggers.Add(hover);
            Trigger dragging = new Trigger { Property = Thumb.IsDraggingProperty, Value = true };
            dragging.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(46, 196, 182), "ThumbChrome"));
            template.Triggers.Add(dragging);
            Trigger disabled = new Trigger { Property = UIElement.IsEnabledProperty, Value = false };
            disabled.Setters.Add(new Setter(Border.BackgroundProperty, BrushFromRgb(26, 30, 37), "ThumbChrome"));
            disabled.Setters.Add(new Setter(Border.BorderBrushProperty, BrushFromRgb(102, 113, 127), "ThumbChrome"));
            template.Triggers.Add(disabled);
            return template;
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
            where T : DependencyObject
        {
            if (parent == null)
            {
                yield break;
            }

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T match)
                {
                    yield return match;
                }

                foreach (T descendant in FindVisualChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            for (DependencyObject parent = GetParent(child); parent != null; parent = GetParent(parent))
            {
                if (parent is T match)
                {
                    return match;
                }
            }

            return null;
        }

        private static SolidColorBrush BrushFromRgb(byte red, byte green, byte blue)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(red, green, blue));
            brush.Freeze();
            return brush;
        }

        public event EventHandler<PropertyGridPropertyValueChangedEventArgs> PropertyValueChanged;
        public event EventHandler SelectedObjectsChanged;
        public event EventHandler<PropertyGridSelectedPropertyChangedEventArgs> SelectedPropertyChanged;

        public string SelectedPropertyName => selectedPropertyName;

        public object SelectedObject
        {
            get { return innerPropertyGrid.SelectedObject; }
            set
            {
                SaveNavigationState();
                suppressSelectedObjectsChanged = true;
                bool previousSuppressPropertyValueChanged = suppressPropertyValueChanged;
                suppressPropertyValueChanged = true;
                try
                {
                    propertyValueChangeSubscription.Detach();
                    EnsurePropertyGridProvider(value?.GetType());
                    RegisterPropertyEditors(value);
                    RegisterComparers(value);
                    // Do not clear WPG's global metadata cache on every tool open.
                    // Visibility/language changes mark it dirty explicitly below.
                    AssignSelectedObject(value);
                    ApplyHiddenPropertyBrowsableState(value);
                    propertyValueChangeSubscription.Attach(innerPropertyGrid.Properties);
                    ScheduleNormalizeInnerEditorControls();
                    ScheduleRestoreNavigationState(value);
                    ScheduleSearchFeedbackUpdate();
                }
                finally
                {
                    suppressPropertyValueChanged = previousSuppressPropertyValueChanged;
                    suppressSelectedObjectsChanged = false;
                }
            }
        }

        public bool HasCategories => innerPropertyGrid.HasCategories;

        public PropertyItemCollection Properties => new PropertyItemCollection(this, innerPropertyGrid.Properties);

        IPropertyGridPropertyCollection IPropertyGridView.Properties => Properties;

        public void ApplyDisplayOptions(PropertyGridDisplayOptions options)
        {
            options = options ?? new PropertyGridDisplayOptions();
            if (childParameterPredicate != null && options.ChildParameterPredicate == null) RestoreToolFormLayout();
            childParameterPredicate = options.ChildParameterPredicate;

            TrySetInnerProperty("PropertyNameColumnWidth", new GridLength(Math.Max(80, options.PropertyNameColumnWidth)));
            editorColumnMinimum = Math.Max(80, options.EditorColumnMinWidth);
            TrySetInnerProperty("EditorColumnMinWidth", editorColumnMinimum);
            TrySetInnerProperty("PropertyFilterVisibility", options.ShowSearchBox ? Visibility.Visible : Visibility.Collapsed);

            innerPropertyGrid.InvalidateMeasure();
            innerPropertyGrid.InvalidateArrange();
            ScheduleNormalizeInnerEditorControls();
        }

        public void SaveNavigationState()
        {
            object selectedObject = SelectedObject;
            if (selectedObject == null || innerPropertyGrid == null)
            {
                return;
            }

            ScrollViewer scrollViewer = FindPrimaryPropertyScrollViewer();
            TextBox searchTextBox = FindPropertySearchTextBox();
            if (scrollViewer == null && searchTextBox == null)
            {
                return;
            }

            lock (browsabilityLock)
            {
                navigationStates.Remove(selectedObject);
                navigationStates.Add(
                    selectedObject,
                    new PropertyGridNavigationState(
                        scrollViewer?.VerticalOffset ?? 0D,
                        searchTextBox?.Text ?? string.Empty));
            }
        }

        public double VerticalScrollOffsetForTest
        {
            get
            {
                ScrollViewer scrollViewer = FindPrimaryPropertyScrollViewer();
                return scrollViewer?.VerticalOffset ?? 0D;
            }
        }

        public string SearchTextForTest
        {
            get
            {
                TextBox searchTextBox = FindPropertySearchTextBox();
                return searchTextBox?.Text ?? string.Empty;
            }
        }

        public bool IsSearchEmptyMessageVisibleForTest => searchEmptyOverlay?.Visibility == Visibility.Visible;

        public bool TryCommitPendingEdit()
        {
            if (Dispatcher != null && !Dispatcher.CheckAccess()) { return Dispatcher.Invoke(TryCommitPendingEdit); }
            if (innerPropertyGrid == null) { return true; }
            TextBox[] editors = FindVisualChildren<TextBox>(innerPropertyGrid).Where(item => IsCommitCandidate(item) && !IsPropertyGridSearchTextBox(item)).ToArray();
            // Validate before any source write: the vendor converter can otherwise retain an old value silently.
            foreach (TextBox editor in editors)
            {
                BindingExpression binding = editor.GetBindingExpression(TextBox.TextProperty);
                PropertyDescriptor descriptor = GetPropertyDescriptor(ReadObjectProperty(editor.DataContext, "ParentProperty"))
                    ?? (binding?.ResolvedSource == null ? null : TypeDescriptor.GetProperties(binding.ResolvedSource)[binding.ResolvedSourcePropertyName ?? string.Empty]);
                TypeCode type = Type.GetTypeCode(Nullable.GetUnderlyingType(descriptor?.PropertyType ?? typeof(object)) ?? descriptor?.PropertyType ?? typeof(object));
                if (type is not (TypeCode.Byte or TypeCode.Decimal or TypeCode.Double or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.SByte or TypeCode.Single or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64)) { continue; }
                try { descriptor.Converter.ConvertFromString(null, CultureInfo.CurrentCulture, editor.Text); }
                catch (Exception error) when (error is FormatException or NotSupportedException or ArgumentException or OverflowException)
                {
                    if (binding != null) { Validation.MarkInvalid(binding, new ValidationError(new ExceptionValidationRule(), binding, error.GetBaseException().Message, error)); }
                    return false;
                }
            }
            CommitPendingEdit();
            return editors.All(editor => !Validation.GetHasError(editor));
        }

        public bool CommitPendingEdit()
        {
            if (innerPropertyGrid == null)
            {
                return false;
            }

            if (Dispatcher != null && !Dispatcher.CheckAccess())
            {
                return Dispatcher.Invoke(CommitPendingEdit);
            }

            bool committed = false;
            foreach (TextBox textBox in FindVisualChildren<TextBox>(innerPropertyGrid).Where(item => IsCommitCandidate(item) && !IsPropertyGridSearchTextBox(item)))
            {
                committed |= TryUpdateBindingSource(textBox, TextBox.TextProperty);
            }

            foreach (ComboBox comboBox in FindVisualChildren<ComboBox>(innerPropertyGrid).Where(IsCommitCandidate))
            {
                committed |= TryUpdateBindingSource(comboBox, Selector.SelectedItemProperty);
                committed |= TryUpdateBindingSource(comboBox, Selector.SelectedValueProperty);
                committed |= TryUpdateBindingSource(comboBox, ComboBox.TextProperty);
            }

            foreach (RangeBase rangeBase in FindVisualChildren<RangeBase>(innerPropertyGrid).Where(IsCommitCandidate))
            {
                committed |= TryUpdateBindingSource(rangeBase, RangeBase.ValueProperty);
            }

            return committed;
        }

        public bool FocusProperty(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName) || innerPropertyGrid == null)
            {
                return false;
            }

            object innerItem = FindInnerPropertyItem(propertyName);
            if (innerItem == null)
            {
                return false;
            }

            foreach (FrameworkElement element in FindVisualChildren<FrameworkElement>(innerPropertyGrid))
            {
                string resolvedName = ResolveSelectedPropertyName(element.DataContext);
                if (!string.Equals(resolvedName, propertyName, StringComparison.Ordinal))
                {
                    continue;
                }

                element.BringIntoView();
                element.Focus();
                Keyboard.Focus(element);
                RaiseSelectedPropertyChanged(propertyName);
                return true;
            }

            return false;
        }

        private void InnerPropertyGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RaiseSelectedPropertyChangedFromSource(e?.OriginalSource as DependencyObject);
        }

        private void InnerPropertyGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled || e.Key != Key.Tab || Keyboard.FocusedElement is not UIElement focused) { return; }
            // The vendor Tab lookup assumes its own row template. Keep the instance editor
            // template and use WPF traversal; an outer validation handler may block the key first.
            e.Handled = true;
            FocusNavigationDirection direction = (Keyboard.Modifiers & ModifierKeys.Shift) != 0
                ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
            if (!focused.MoveFocus(new TraversalRequest(direction))) { MoveFocus(new TraversalRequest(direction)); }
        }

        private void InnerPropertyGrid_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            RaiseSelectedPropertyChangedFromSource(e?.NewFocus as DependencyObject);
        }

        private void RaiseSelectedPropertyChangedFromSource(DependencyObject source)
        {
            DependencyObject current = source;
            while (current != null)
            {
                if (current is FrameworkElement frameworkElement)
                {
                    string propertyName = ResolveSelectedPropertyName(frameworkElement.DataContext);
                    if (!string.IsNullOrWhiteSpace(propertyName))
                    {
                        RaiseSelectedPropertyChanged(propertyName);
                        return;
                    }
                }

                DependencyObject visualParent = VisualTreeHelper.GetParent(current);
                current = visualParent ?? LogicalTreeHelper.GetParent(current);
            }
        }

        private string ResolveSelectedPropertyName(object candidate)
        {
            if (candidate == null)
            {
                return string.Empty;
            }

            if (IsOriginalPropertyItem(candidate))
            {
                return new PropertyItem(this, candidate).Name ?? string.Empty;
            }

            if (IsOriginalPropertyItemValue(candidate))
            {
                PropertyInfo parentProperty = candidate.GetType().GetProperty("ParentProperty");
                object parent = parentProperty?.GetValue(candidate, null);
                return IsOriginalPropertyItem(parent)
                    ? new PropertyItem(this, parent).Name ?? string.Empty
                    : string.Empty;
            }

            return string.Empty;
        }

        private void RaiseSelectedPropertyChanged(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)
                || string.Equals(selectedPropertyName, propertyName, StringComparison.Ordinal))
            {
                return;
            }

            selectedPropertyName = propertyName;
            if (ThemeVariant == PropertyGridThemeVariant.Dark) ScheduleNormalizeInnerEditorControls();
            SelectedPropertyChanged?.Invoke(
                this,
                new PropertyGridSelectedPropertyChangedEventArgs(propertyName));
        }

        private static bool IsCommitCandidate(FrameworkElement element)
        {
            return element != null && element.IsVisible && element.IsEnabled;
        }

        private static bool TryUpdateBindingSource(FrameworkElement element, DependencyProperty property)
        {
            try
            {
                BindingExpression binding = element?.GetBindingExpression(property);
                if (binding == null)
                {
                    return false;
                }

                // Tool commands must use the text the operator can see now. RangeEditor
                // TextBoxes still commit only here/Enter/focus-loss so partial typing is not snapped back.
                binding.UpdateSource();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ScheduleNormalizeInnerEditorControls()
        {
            if (normalizeScheduled)
            {
                return;
            }

            normalizeScheduled = true;
            int session = normalizationSession;
            Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    if (session != normalizationSession || (childParameterPredicate != null && normalizationSession > 0 && !IsLoaded)) return;
                    normalizeScheduled = false;
                    NormalizeInnerEditorControls();
                }),
                DispatcherPriority.Loaded);
        }

        private void ScheduleSearchFeedbackUpdate()
        {
            if (Dispatcher == null)
            {
                UpdateSearchFeedback();
                return;
            }

            if (searchFeedbackUpdateScheduled)
            {
                return;
            }

            searchFeedbackUpdateScheduled = true;
            Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    searchFeedbackUpdateScheduled = false;
                    UpdateSearchFeedback();
                }),
                DispatcherPriority.Background);
        }

        private void ScheduleRestoreNavigationState(object selectedObject)
        {
            if (selectedObject == null || Dispatcher == null)
            {
                return;
            }

            Dispatcher.BeginInvoke(
                (Action)(() => RestoreNavigationState(selectedObject, attempt: 0)),
                DispatcherPriority.ContextIdle);
        }

        private void RestoreNavigationState(object selectedObject, int attempt)
        {
            if (selectedObject == null || !ReferenceEquals(SelectedObject, selectedObject))
            {
                return;
            }

            PropertyGridNavigationState state;
            lock (browsabilityLock)
            {
                if (!navigationStates.TryGetValue(selectedObject, out state))
                {
                    return;
                }
            }

            innerPropertyGrid.ApplyTemplate();
            innerPropertyGrid.UpdateLayout();
            bool searchReady = RestoreSearchText(state.SearchText);
            ScrollViewer scrollViewer = FindPrimaryPropertyScrollViewer();
            bool scrollNeeded = state.VerticalOffset > 0D;
            if (!searchReady || (scrollNeeded && (scrollViewer == null || scrollViewer.ScrollableHeight <= 0D)))
            {
                if (attempt < 2)
                {
                    Dispatcher.BeginInvoke(
                        (Action)(() => RestoreNavigationState(selectedObject, attempt + 1)),
                        DispatcherPriority.ApplicationIdle);
                }

                return;
            }

            if (!scrollNeeded || scrollViewer == null)
            {
                return;
            }

            double targetOffset = Math.Max(0D, Math.Min(state.VerticalOffset, scrollViewer.ScrollableHeight));
            scrollViewer.ScrollToVerticalOffset(targetOffset);
            scrollViewer.UpdateLayout();
        }

        private bool RestoreSearchText(string searchText)
        {
            TextBox searchTextBox = FindPropertySearchTextBox();
            if (searchTextBox == null)
            {
                return string.IsNullOrEmpty(searchText);
            }

            string targetText = searchText ?? string.Empty;
            if (!string.Equals(searchTextBox.Text ?? string.Empty, targetText, StringComparison.Ordinal))
            {
                searchTextBox.Text = targetText;
                searchTextBox.CaretIndex = searchTextBox.Text.Length;
                TryUpdateBindingSource(searchTextBox, TextBox.TextProperty);
                searchTextBox.UpdateLayout();
            }

            ScheduleSearchFeedbackUpdate();
            return true;
        }

        private ScrollViewer FindPrimaryPropertyScrollViewer()
        {
            if (innerPropertyGrid == null)
            {
                return null;
            }

            return FindVisualChildren<ScrollViewer>(innerPropertyGrid)
                .Where(item => item.IsVisible)
                .OrderByDescending(item => item.ScrollableHeight)
                .ThenByDescending(item => item.ViewportHeight)
                .FirstOrDefault();
        }

        private TextBox FindPropertySearchTextBox()
        {
            if (innerPropertyGrid == null)
            {
                return null;
            }

            return FindVisualChildren<TextBox>(innerPropertyGrid)
                .FirstOrDefault(IsPropertyGridSearchTextBox);
        }

        private void NormalizeInnerEditorControls()
        {
            if (innerPropertyGrid == null)
            {
                return;
            }

            // Regenerated vendor rows must exist before their theme values are applied.
            innerPropertyGrid.ApplyTemplate();
            innerPropertyGrid.UpdateLayout();

            bool compactDensity = IsCompactDensity;
            bool darkTheme = ThemeVariant == PropertyGridThemeVariant.Dark;
            Style comboItemStyle = CreateComboBoxItemStyle(compactDensity, darkTheme);
            double requiredRangeWidth = 0D;
            List<FrameworkElement> editorElements = new List<FrameworkElement>();
            foreach (FrameworkElement element in FindVisualChildren<FrameworkElement>(innerPropertyGrid))
            {
                editorElements.Add(element);
            }

            foreach (FrameworkElement element in editorElements)
            {
                if (!darkTheme) RestoreDefaultThemeValues(element);
                if (element is TextBox textBox && IsPropertyGridSearchTextBox(textBox))
                {
                    NormalizeSearchTextBox(textBox, compactDensity);
                    continue;
                }

                if (darkTheme && element is TextBox editorTextBox)
                {
                    ApplyDarkEditorStyle(editorTextBox);
                }

                if (element is Expander complexEditor && complexEditor.Header is TextBlock complexHeader
                    && (complexHeader.Text == "Details" || complexHeader.Text == "상세"))
                {
                    complexHeader.Text = OpenVisionLanguageService.T("VisionTest.Details");
                    complexEditor.Expanded -= ComplexEditor_Expanded;
                    if (darkTheme) complexEditor.Expanded += ComplexEditor_Expanded;
                    if (darkTheme)
                    {
                        ApplyDarkControlStateStyle(complexEditor);
                        ToggleButton headerToggle = FindVisualChildren<ToggleButton>(complexEditor).FirstOrDefault(item => item.Name == "ExpanderToggleButton");
                        if (headerToggle != null)
                        {
                            ApplyDarkControlStateStyle(headerToggle);
                            SetDarkThemeBinding(headerToggle, Control.ForegroundProperty, new Binding(nameof(Control.Foreground)) { Source = complexEditor });
                            SetDarkThemeBinding(complexHeader, TextBlock.ForegroundProperty, new Binding(nameof(Control.Foreground)) { Source = complexEditor });
                            foreach (TextBlock icon in FindVisualChildren<TextBlock>(headerToggle).Where(item => item.FontFamily.Source.Contains("#FluentSystemIcons")))
                                SetDarkThemeBinding(icon, TextBlock.ForegroundProperty, new Binding(nameof(Control.Foreground)) { Source = complexEditor });
                        }
                    }
                }

                if (darkTheme && element is Control sliderEditor && element.GetType().Name == "SliderEditorBase")
                    ApplyDarkControlStateStyle(sliderEditor);

                if (darkTheme && element is CheckBox checkBox)
                {
                    SetDarkThemeValue(checkBox, Control.TemplateProperty, DarkCheckBoxTemplate);
                }

                if (element is ComboBox comboBox)
                {
                    if (darkTheme) SetDarkThemeValue(comboBox, Control.ForegroundProperty, BrushFromRgb(232, 235, 241));
                    // The WPG enum editor can retain editable chrome from the legacy bridge;
                    // keep it as a pure selector so the selected text is rendered once.
                    comboBox.IsEditable = false;
                    comboBox.StaysOpenOnEdit = false;
                    comboBox.Template = CreateComboBoxTemplate(darkTheme);
                    comboBox.MinHeight = Math.Max(comboBox.MinHeight, compactDensity ? 28D : 30D);
                    comboBox.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    comboBox.VerticalContentAlignment = VerticalAlignment.Center;
                    comboBox.MaxDropDownHeight = Math.Max(comboBox.MaxDropDownHeight, 260D);
                    comboBox.ItemContainerStyle = comboItemStyle;
                    comboBox.ApplyTemplate();
                    if (darkTheme)
                    {
                        ApplyDarkControlStateStyle(comboBox);
                        SetDarkThemeValue(comboBox, Control.ForegroundProperty, BrushFromRgb(232, 235, 241));
                    }

                    comboBox.PreviewMouseLeftButtonDown -= ComboBox_PreviewMouseLeftButtonDown;
                    comboBox.PreviewMouseLeftButtonDown += ComboBox_PreviewMouseLeftButtonDown;
                }

                if (darkTheme && element is Border searchIconBorder
                    && string.Equals(searchIconBorder.Name, "PART_SearchIconBorder", StringComparison.Ordinal))
                {
                    SetDarkThemeValue(searchIconBorder, Border.BackgroundProperty, BrushFromRgb(26, 30, 37));
                    SetDarkThemeValue(searchIconBorder, Border.BorderBrushProperty, BrushFromRgb(102, 113, 127));
                }

                if (darkTheme
                    && (element.GetType().Name == "ThresholdEditorBase"
                        || element.GetType().Name == "MetricRangeEditorBase"))
                {
                    if (element is Panel editorPanel)
                    {
                        SetDarkThemeValue(editorPanel, Panel.BackgroundProperty, BrushFromRgb(20, 23, 29));
                    }
                    else if (element is Control editorControl)
                    {
                        SetDarkThemeValue(editorControl, Control.BackgroundProperty, BrushFromRgb(20, 23, 29));
                        SetDarkThemeValue(editorControl, Control.ForegroundProperty, BrushFromRgb(232, 235, 241));
                    }

                    foreach (TextBlock label in FindVisualChildren<TextBlock>(element))
                    {
                        SetDarkThemeValue(label, TextBlock.ForegroundProperty, BrushFromRgb(174, 182, 195));
                    }
                }

                if (darkTheme && element is Border scrollBarBorder
                    && string.Equals(scrollBarBorder.Name, "PART_Border", StringComparison.Ordinal)
                    && FindVisualParent<ScrollBar>(scrollBarBorder) != null)
                {
                    SetDarkThemeValue(scrollBarBorder, Border.BackgroundProperty, BrushFromRgb(26, 30, 37));
                    SetDarkThemeValue(scrollBarBorder, Border.BorderBrushProperty, BrushFromRgb(38, 43, 52));
                }

                if (darkTheme && element is RepeatButton sliderTrackButton
                    && FindVisualParent<Track>(sliderTrackButton) is Track sliderTrack
                    && FindVisualParent<Slider>(sliderTrack) != null)
                {
                    bool filled = ReferenceEquals(sliderTrack.DecreaseRepeatButton, sliderTrackButton);
                    SetDarkThemeValue(sliderTrackButton, Control.BackgroundProperty, filled ? BrushFromRgb(46, 196, 182) : BrushFromRgb(102, 113, 127));
                    SetDarkThemeValue(sliderTrackButton, Control.TemplateProperty, filled ? DarkSliderFilledTrackTemplate : DarkSliderEmptyTrackTemplate);
                }

                if (darkTheme && element is Thumb sliderThumb
                    && FindVisualParent<Slider>(sliderThumb) != null)
                {
                    SetDarkThemeValue(sliderThumb, Control.BackgroundProperty, BrushFromRgb(46, 196, 182));
                    SetDarkThemeValue(sliderThumb, Control.BorderBrushProperty, BrushFromRgb(102, 113, 127));
                    SetDarkThemeValue(sliderThumb, Control.TemplateProperty, DarkSliderThumbTemplate);
                }

                NormalizeThemeSurface(element, darkTheme);
                requiredRangeWidth = Math.Max(requiredRangeWidth, NormalizeRangeEditorLayout(element));
                NormalizeChildParameterRow(element);

                if (element is Slider slider)
                {
                    if (darkTheme)
                    {
                        SetDarkThemeValue(slider, Control.ForegroundProperty, BrushFromRgb(46, 196, 182));
                        SetDarkThemeValue(slider, Control.BackgroundProperty, BrushFromRgb(38, 43, 52));
                        SetDarkThemeValue(slider, UIElement.OpacityProperty, 1D);
                    }

                    if (FindRangeEditorElement(slider) != null)
                    {
                        slider.MinHeight = Math.Max(slider.MinHeight, 20D);
                        slider.Margin = new Thickness(slider.Margin.Left, 0D, slider.Margin.Right, 0D);
                        continue;
                    }

                    slider.MinHeight = Math.Max(slider.MinHeight, compactDensity ? 28D : 34D);
                    slider.Margin = new Thickness(
                        slider.Margin.Left,
                        Math.Max(slider.Margin.Top, compactDensity ? 3D : 6D),
                        slider.Margin.Right,
                        Math.Max(slider.Margin.Bottom, compactDensity ? 3D : 6D));
                }

                if (element is Button button && IsPropertyGridDialogButton(button))
                {
                    if (darkTheme)
                    {
                        ApplyDarkControlStateStyle(button);
                        SetDarkThemeValue(button, Control.ForegroundProperty, BrushFromRgb(232, 235, 241));
                        SetDarkThemeValue(button, Control.BackgroundProperty, BrushFromRgb(26, 30, 37));
                        SetDarkThemeValue(button, Control.BorderBrushProperty, BrushFromRgb(102, 113, 127));
                        SetDarkThemeValue(button, Control.TemplateProperty, DarkDialogButtonTemplate);
                    }

                    object dialogPropertyValue = ResolveDialogPropertyValue(button);
                    if (CanInvokeDialogEditorDirect(dialogPropertyValue))
                    {
                        SetDialogPropertyValue(button, dialogPropertyValue);
                        button.Command = null;
                        button.CommandParameter = null;
                        button.CommandTarget = null;
                        button.IsEnabled = true;
                    }

                    if (HasEllipsisContent(button))
                    {
                        button.Width = double.IsNaN(button.Width) ? 32D : Math.Max(button.Width, 32D);
                        button.MinWidth = double.IsNaN(button.MinWidth) ? 32D : Math.Max(button.MinWidth, 32D);
                        button.MinHeight = Math.Max(button.MinHeight, 28D);
                        button.Padding = new Thickness(0);
                        button.HorizontalContentAlignment = HorizontalAlignment.Center;
                        button.VerticalContentAlignment = VerticalAlignment.Center;
                        button.Margin = new Thickness(
                            Math.Max(button.Margin.Left, 4D),
                            button.Margin.Top,
                            Math.Max(button.Margin.Right, 12D),
                            button.Margin.Bottom);
                        Panel.SetZIndex(button, 10);
                    }

                    button.Cursor = Cursors.Hand;
                    button.PreviewMouseLeftButtonDown -= DialogButton_PreviewMouseLeftButtonDown;
                    button.PreviewMouseLeftButtonDown += DialogButton_PreviewMouseLeftButtonDown;
                    button.Click -= DialogButton_Click;
                    button.Click += DialogButton_Click;
                }
            }

            innerPropertyGrid.MinWidth = requiredRangeWidth;
            ScheduleSearchFeedbackUpdate();
        }


        private void ComplexEditor_Expanded(object sender, RoutedEventArgs e) => ScheduleNormalizeInnerEditorControls();

        private void SetDarkThemeValue(DependencyObject element, DependencyProperty property, object value)
        {
            Dictionary<DependencyProperty, object> originals = darkThemeOriginalValues.GetOrCreateValue(element);
            if (!originals.ContainsKey(property))
                originals[property] = element.ReadLocalValue(property) == DependencyProperty.UnsetValue ? DependencyProperty.UnsetValue : element.GetValue(property);
            element.SetCurrentValue(property, value);
        }

        private void SetDarkThemeBinding(DependencyObject element, DependencyProperty property, BindingBase binding)
        {
            Dictionary<DependencyProperty, object> originals = darkThemeOriginalValues.GetOrCreateValue(element);
            if (!originals.ContainsKey(property))
            {
                BindingBase originalBinding = BindingOperations.GetBindingBase(element, property);
                originals[property] = originalBinding != null ? (object)originalBinding
                    : element.ReadLocalValue(property) == DependencyProperty.UnsetValue ? DependencyProperty.UnsetValue : element.GetValue(property);
            }
            BindingOperations.SetBinding(element, property, binding);
        }

        private void RestoreDefaultThemeValues(DependencyObject element)
        {
            if (!darkThemeOriginalValues.TryGetValue(element, out Dictionary<DependencyProperty, object> originals)) return;
            foreach (KeyValuePair<DependencyProperty, object> original in originals)
            {
                if (original.Value is BindingBase binding) BindingOperations.SetBinding(element, original.Key, binding);
                else if (original.Value == DependencyProperty.UnsetValue) element.ClearValue(original.Key);
                else element.SetCurrentValue(original.Key, original.Value);
            }
            darkThemeOriginalValues.Remove(element);
            if (element is Control control) appliedDarkStyles.Remove(control);
        }

        private void NormalizeThemeSurface(FrameworkElement element, bool darkTheme)
        {
            if (darkTheme && element is Border gridChrome && ReferenceEquals(gridChrome.TemplatedParent, innerPropertyGrid))
            {
                SetDarkThemeBinding(gridChrome, Border.BorderBrushProperty, new Binding(nameof(Control.BorderBrush)) { Source = innerPropertyGrid });
                return;
            }

            if (!darkTheme || !(element is Border rowBorder)
                || !string.Equals(rowBorder.Name, "RowBorder", StringComparison.Ordinal))
            {
                return;
            }

            bool selected = string.Equals(ResolveSelectedPropertyName(rowBorder.DataContext), selectedPropertyName, StringComparison.Ordinal);
            rowBorder.Background = BrushFromRgb(20, 23, 29);
            SetDarkThemeValue(rowBorder, Border.BorderBrushProperty, BrushFromRgb(38, 43, 52));
            foreach (Border border in FindVisualChildren<Border>(rowBorder))
            {
                if (FindVisualParent<Slider>(border) != null || FindVisualParent<TextBox>(border) != null
                    || FindVisualParent<ComboBox>(border) != null || FindVisualParent<CheckBox>(border) != null
                    || FindVisualParent<Button>(border) != null)
                {
                    continue;
                }

                if (string.Equals(border.Name, "NameCell", StringComparison.Ordinal))
                {
                    SetDarkThemeBinding(border, Border.BackgroundProperty, new Binding { Source = selected ? BrushFromRgb(30, 54, 59) : BrushFromRgb(26, 30, 37) });
                    SetDarkThemeValue(border, Border.BorderBrushProperty, BrushFromRgb(38, 43, 52));
                }
                else if (Grid.GetColumn(border) == 1)
                {
                    SetDarkThemeValue(border, Border.BackgroundProperty, BrushFromRgb(20, 23, 29));
                }
                else if (!string.Equals(border.Name, "RowAccent", StringComparison.Ordinal))
                {
                    SetDarkThemeValue(border, Border.BackgroundProperty, BrushFromRgb(20, 23, 29));
                }
            }

            foreach (TextBlock textBlock in FindVisualChildren<TextBlock>(rowBorder))
            {
                string typeName = textBlock.GetType().FullName ?? string.Empty;
                if (typeName.EndsWith(".Design.PropertyNameTextBlock", StringComparison.Ordinal))
                {
                    SetDarkThemeBinding(textBlock, TextBlock.ForegroundProperty, new Binding { Source = selected ? BrushFromRgb(232, 235, 241) : BrushFromRgb(174, 182, 195) });
                }
            }
        }

        private void ApplyDarkEditorStyle(TextBox textBox)
        {
            ApplyDarkControlStateStyle(textBox);
            SetDarkThemeValue(textBox, Control.ForegroundProperty, BrushFromRgb(232, 235, 241));
            SetDarkThemeValue(textBox, Control.BackgroundProperty, BrushFromRgb(14, 17, 21));
            SetDarkThemeValue(textBox, Control.BorderBrushProperty, BrushFromRgb(88, 101, 119));
            SetDarkThemeValue(textBox, TextBox.CaretBrushProperty, BrushFromRgb(232, 235, 241));
            SetDarkThemeValue(textBox, TextBox.SelectionBrushProperty, BrushFromRgb(46, 196, 182));
            textBox.ApplyTemplate();
            foreach (Border chrome in FindVisualChildren<Border>(textBox))
            {
                if (!string.Equals(chrome.Name, "border", StringComparison.Ordinal)) continue;
                SetDarkThemeBinding(chrome, Border.BackgroundProperty, new Binding("Background") { Source = textBox });
                SetDarkThemeBinding(chrome, Border.BorderBrushProperty, new Binding("BorderBrush") { Source = textBox });
                SetDarkThemeBinding(chrome, UIElement.OpacityProperty, new Binding { Source = 1D });
            }
        }

        private void NormalizeSearchTextBox(TextBox searchTextBox, bool compactDensity)
        {
            // Keep the vendor WPG filter path, but make the search field predictable for
            // long docked tools: instant search, localized hint, and Escape-to-clear.
            searchTextBox.MinHeight = Math.Max(searchTextBox.MinHeight, compactDensity ? 28D : 30D);
            searchTextBox.Margin = compactDensity ? new Thickness(0, 1, 0, 1) : new Thickness(0, 2, 0, 2);
            searchTextBox.ToolTip = SearchToolTipText();
            TrySetSearchTextBoxProperty(searchTextBox, "SearchMode", "Instant");
            TrySetSearchTextBoxProperty(searchTextBox, "SearchEventTimeDelay", 80);
            TrySetSearchTextBoxProperty(searchTextBox, "LabelText", SearchLabelText());
            if (ThemeVariant == PropertyGridThemeVariant.Dark)
            {
                ApplyDarkEditorStyle(searchTextBox);
            }

            searchTextBox.KeyDown -= SearchTextBox_KeyDown;
            searchTextBox.KeyDown += SearchTextBox_KeyDown;
            searchTextBox.TextChanged -= SearchTextBox_TextChanged;
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape || !(sender is TextBox searchTextBox))
            {
                return;
            }

            if (string.IsNullOrEmpty(searchTextBox.Text))
            {
                return;
            }

            searchTextBox.Clear();
            TryUpdateBindingSource(searchTextBox, TextBox.TextProperty);
            SaveNavigationState();
            e.Handled = true;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox searchTextBox && IsPropertyGridSearchTextBox(searchTextBox))
            {
                SaveNavigationState();
                ScheduleSearchFeedbackUpdate();
            }
        }

        private void UpdateSearchFeedback()
        {
            if (searchEmptyOverlay == null || searchEmptyMessage == null)
            {
                return;
            }

            TextBox searchTextBox = FindPropertySearchTextBox();
            string searchText = searchTextBox?.Text ?? string.Empty;
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyFilter filter = new WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyFilter(searchText);
            foreach (WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem property in innerPropertyGrid.Properties) property.ApplyFilter(filter);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                searchEmptyOverlay.Visibility = Visibility.Collapsed;
                return;
            }

            bool hasSearchMatch = HasPropertySearchMatch(searchText);
            searchEmptyMessage.Text = FormatSearchNoResultsText(searchText);
            searchEmptyOverlay.Visibility = hasSearchMatch ? Visibility.Collapsed : Visibility.Visible;
        }

        private bool HasPropertySearchMatch(string searchText)
        {
            object selectedObject = SelectedObject;
            if (selectedObject == null || string.IsNullOrWhiteSpace(searchText))
            {
                return true;
            }

            Type selectedType = selectedObject.GetType();
            foreach (WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem property in innerPropertyGrid.Properties)
            {
                if (property.PropertyDescriptor?.IsBrowsable != true
                    || IsPropertyHidden(selectedType, property.Name)
                    || IsRangeCompanionPropertyName(property.Name))
                {
                    continue;
                }

                if (property.MatchesFilter) return true;
            }

            return false;
        }

        private static string SearchLabelText()
        {
            string text = OpenVisionLanguageService.T("Localization.Search");
            return string.IsNullOrWhiteSpace(text) || string.Equals(text, "Localization.Search", StringComparison.Ordinal)
                ? "Search"
                : text;
        }

        private static string SearchToolTipText()
        {
            string text = OpenVisionLanguageService.T("PropertyGrid.Search.ToolTip");
            return string.IsNullOrWhiteSpace(text) || string.Equals(text, "PropertyGrid.Search.ToolTip", StringComparison.Ordinal)
                ? "Filter properties by name. Press Esc to clear the search."
                : text;
        }

        private static string FormatSearchNoResultsText(string searchText)
        {
            string format = OpenVisionLanguageService.T("PropertyGrid.Search.NoResults");
            if (string.IsNullOrWhiteSpace(format) || string.Equals(format, "PropertyGrid.Search.NoResults", StringComparison.Ordinal))
            {
                format = "No match for \u2018{0}\u2019 (Esc: clear)";
            }

            try
            {
                return string.Format(CultureInfo.CurrentCulture, format, searchText ?? string.Empty);
            }
            catch (FormatException)
            {
                return format;
            }
        }

        private static bool IsPropertyGridSearchTextBox(DependencyObject element)
        {
            string typeName = element?.GetType().FullName ?? string.Empty;
            return typeName.EndsWith(".Controls.SearchTextBox", StringComparison.Ordinal);
        }

        private static void TrySetSearchTextBoxProperty(TextBox searchTextBox, string propertyName, object value)
        {
            if (searchTextBox == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return;
            }

            PropertyInfo property = searchTextBox.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property == null || !property.CanWrite)
            {
                return;
            }

            try
            {
                object convertedValue = ConvertSearchTextBoxPropertyValue(property.PropertyType, value);
                if (convertedValue != null || !property.PropertyType.IsValueType)
                {
                    property.SetValue(searchTextBox, convertedValue, null);
                }
            }
            catch
            {
            }
        }

        private static object ConvertSearchTextBoxPropertyValue(Type targetType, object value)
        {
            if (targetType == null)
            {
                return null;
            }

            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, Convert.ToString(value, CultureInfo.InvariantCulture), true);
            }

            if (targetType == typeof(TimeSpan))
            {
                double milliseconds = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                return TimeSpan.FromMilliseconds(milliseconds);
            }

            if (targetType == typeof(string))
            {
                return Convert.ToString(value, CultureInfo.CurrentCulture);
            }

            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }

        private static bool IsPropertyGridDialogButton(Button button)
        {
            if (button == null)
            {
                return false;
            }

            return HasEllipsisContent(button)
                || IsPropertyGridPropertyValue(button.DataContext)
                || IsPropertyGridPropertyValue(button.CommandParameter);
        }

        private static bool HasEllipsisContent(Button button)
        {
            string text = ExtractElementText(button?.Content);
            return string.Equals(text, "...", StringComparison.Ordinal)
                || (text.Length == 1 && text[0] == '\u2026');
        }

        private static string ExtractElementText(object content)
        {
            if (content == null)
            {
                return string.Empty;
            }

            if (content is string text)
            {
                return text.Trim();
            }

            if (content is TextBlock textBlock)
            {
                return (textBlock.Text ?? string.Empty).Trim();
            }

            if (content is AccessText accessText)
            {
                return (accessText.Text ?? string.Empty).Trim();
            }

            if (content is ContentControl contentControl)
            {
                return ExtractElementText(contentControl.Content);
            }

            if (content is Panel panel)
            {
                foreach (UIElement child in panel.Children)
                {
                    string childText = ExtractElementText(child);
                    if (!string.IsNullOrWhiteSpace(childText))
                    {
                        return childText.Trim();
                    }
                }
            }

            if (content is DependencyObject dependencyObject)
            {
                foreach (TextBlock childTextBlock in FindVisualChildren<TextBlock>(dependencyObject))
                {
                    if (!string.IsNullOrWhiteSpace(childTextBlock.Text))
                    {
                        return childTextBlock.Text.Trim();
                    }
                }

                foreach (AccessText childAccessText in FindVisualChildren<AccessText>(dependencyObject))
                {
                    if (!string.IsNullOrWhiteSpace(childAccessText.Text))
                    {
                        return childAccessText.Text.Trim();
                    }
                }
            }

            return Convert.ToString(content)?.Trim() ?? string.Empty;
        }

        private static bool IsPropertyGridPropertyValue(object value)
        {
            string typeName = value?.GetType().FullName ?? string.Empty;
            return typeName.EndsWith(".PropertyItemValue", StringComparison.Ordinal)
                || typeName.EndsWith(".PropertyItem", StringComparison.Ordinal);
        }


        private void HorizontalViewport_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged) ScheduleNormalizeInnerEditorControls();
        }

        private void RestoreToolFormLayout()
        {
            if (childParameterPredicate == null) return;
            foreach (Border row in FindVisualChildren<Border>(innerPropertyGrid).Where(item => item.Name == "RowBorder")) NormalizePropertyRowLayout(row, restore: true);
            innerPropertyGrid.ClearValue(MinWidthProperty);
        }

        private void NormalizePropertyRowLayout(Border rowBorder, bool restore = false)
        {
            if (childParameterPredicate == null || rowBorder.Child is not Grid row || row.ColumnDefinitions.Count != 2) return;
            TextBlock name = row.Children.OfType<TextBlock>().FirstOrDefault(item => (item.GetType().FullName ?? string.Empty).EndsWith(".Design.PropertyNameTextBlock", StringComparison.Ordinal));
            Border editor = row.Children.OfType<Border>().FirstOrDefault(item => item.Name != "NameCell" && item.Name != "RowAccent");
            if (name == null || editor == null) return;
            if (restore)
            {
                if (originalNameLayout.TryGetValue(name, out object[] values))
                {
                    if (values[0] == DependencyProperty.UnsetValue) name.ClearValue(TextBlock.TextWrappingProperty); else name.SetValue(TextBlock.TextWrappingProperty, values[0]);
                    if (values[1] == DependencyProperty.UnsetValue) name.ClearValue(TextBlock.TextTrimmingProperty); else name.SetValue(TextBlock.TextTrimmingProperty, values[1]);
                    originalNameLayout.Remove(name);
                }
            }
            else
            {
                originalNameLayout.GetValue(name, item => new object[] { item.ReadLocalValue(TextBlock.TextWrappingProperty), item.ReadLocalValue(TextBlock.TextTrimmingProperty) });
                name.SetCurrentValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
                name.SetCurrentValue(TextBlock.TextTrimmingProperty, TextTrimming.None);
            }
            double available = restore ? double.PositiveInfinity : horizontalViewport?.ViewportWidth ?? ActualWidth;
            if (!restore && (available <= 0D || row.ActualWidth <= 0D)) return;
            FrameworkElement content = horizontalViewport?.Content as FrameworkElement;
            double chrome = Math.Max(0D, (content?.ActualWidth ?? ActualWidth) - row.ActualWidth);
            double nameWidth = row.ColumnDefinitions[0].Width.IsAbsolute ? row.ColumnDefinitions[0].Width.Value : row.ColumnDefinitions[0].ActualWidth;
            Grid range = FindVisualChildren<Grid>(editor).FirstOrDefault(item => item.GetType().FullName == "System.Windows.Controls.WpfPropertyGrid.Controls.RangeEditorBase");
            bool stacked = Grid.GetRow(editor) == 1 && Grid.GetColumnSpan(editor) == 2 && row.RowDefinitions.Count == 2;
            bool narrow = range != null && available + 0.5D < nameWidth + Math.Max(390D, range.MinWidth) + editor.Padding.Left + editor.Padding.Right + chrome;
            if (row.RowDefinitions.Count != 0 && !stacked) return;
            if (narrow && !stacked)
            {
                row.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                row.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            foreach (UIElement child in row.Children)
            {
                bool isEditor = ReferenceEquals(child, editor);
                Grid.SetRow(child, narrow && isEditor ? 1 : 0);
                Grid.SetColumn(child, narrow ? 0 : isEditor ? 1 : 0);
                Grid.SetColumnSpan(child, narrow ? 2 : 1);
            }
            if (!narrow && stacked) row.RowDefinitions.Clear();
            row.ColumnDefinitions[1].MinWidth = narrow ? 0D : Math.Min(editorColumnMinimum, Math.Max(120D, available - nameWidth - chrome));
            row.UpdateLayout();
        }

        private void NormalizeChildParameterRow(FrameworkElement element)
        {
            if (!(element is Border rowBorder)
                || !string.Equals(rowBorder.Name, "RowBorder", StringComparison.Ordinal))
            {
                return;
            }

            NormalizePropertyRowLayout(rowBorder);
            string propertyName = ResolvePropertyName(rowBorder.DataContext);
            ApplyCompatibilityReadOnlyPresentation(rowBorder, propertyName);
            if (IsRangeCompanionPropertyName(propertyName))
            {
                // Keep the companion descriptor alive for the WPG RangeEditor, but remove the
                // duplicate row from the operator surface. Removing it from TypeDescriptor breaks
                // Max endpoint edits such as Matching FIND_ANGLE_MAX.
                rowBorder.Visibility = Visibility.Collapsed;
                rowBorder.IsHitTestVisible = false;
                return;
            }

            if (!IsChildParameterProperty(propertyName))
            {
                return;
            }

            // Conditional rows are children of a switch/selector above them. Give them a quiet inset treatment
            // so operators can read the generated PropertyGrid hierarchy without changing the model-driven flow.
            bool darkTheme = ThemeVariant == PropertyGridThemeVariant.Dark;
            rowBorder.Background = darkTheme ? BrushFromRgb(27, 31, 39) : BrushFromRgb(250, 253, 255);
            rowBorder.BorderBrush = darkTheme ? BrushFromRgb(38, 43, 52) : BrushFromRgb(210, 226, 239);

            foreach (Border border in FindVisualChildren<Border>(rowBorder))
            {
                if (string.Equals(border.Name, "RowAccent", StringComparison.Ordinal))
                {
                    border.Width = 4D;
                    border.Background = darkTheme ? BrushFromRgb(46, 196, 182) : BrushFromRgb(93, 154, 205);
                }
                else if (string.Equals(border.Name, "NameCell", StringComparison.Ordinal))
                {
                    if (!darkTheme) border.Background = BrushFromRgb(229, 239, 247);
                }
            }

            foreach (TextBlock textBlock in FindVisualChildren<TextBlock>(rowBorder))
            {
                string typeName = textBlock.GetType().FullName ?? string.Empty;
                if (!typeName.EndsWith(".Design.PropertyNameTextBlock", StringComparison.Ordinal))
                {
                    continue;
                }

                textBlock.Margin = new Thickness(
                    Math.Max(textBlock.Margin.Left, 22D),
                    textBlock.Margin.Top,
                    textBlock.Margin.Right,
                    textBlock.Margin.Bottom);
                if (!darkTheme) textBlock.Foreground = BrushFromRgb(66, 91, 112);
            }
        }

        private bool IsChildParameterProperty(string propertyName)
        {
            object selectedObject = SelectedObject;
            return selectedObject != null
                && !string.IsNullOrWhiteSpace(propertyName)
                && childParameterPredicate?.Invoke(selectedObject, propertyName) == true;
        }

        private void ApplyCompatibilityReadOnlyPresentation(Border rowBorder, string propertyName)
        {
            if (rowBorder == null
                || string.IsNullOrWhiteSpace(propertyName)
                || SelectedObject == null)
            {
                return;
            }

            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(SelectedObject)[propertyName];
            if (!(descriptor?.Attributes[typeof(PropertyGridCompatibilityReadOnlyAttribute)]
                is PropertyGridCompatibilityReadOnlyAttribute))
            {
                return;
            }

            rowBorder.ToolTip = descriptor.Description;
            foreach (Control editor in FindVisualChildren<Control>(rowBorder))
            {
                editor.IsEnabled = false;
                editor.IsTabStop = false;
            }

            bool darkTheme = ThemeVariant == PropertyGridThemeVariant.Dark;
            foreach (Border border in FindVisualChildren<Border>(rowBorder))
            {
                if (Grid.GetColumn(border) == 1)
                {
                    border.Background = darkTheme
                        ? BrushFromRgb(25, 42, 47)
                        : BrushFromRgb(244, 247, 249);
                }
            }
        }

        private bool IsRangeCompanionPropertyName(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName) || SelectedObject == null)
            {
                return false;
            }

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(SelectedObject))
            {
                RangeEditorAttribute rangeEditor = descriptor?.Attributes[typeof(RangeEditorAttribute)] as RangeEditorAttribute;
                if (rangeEditor == null)
                {
                    continue;
                }

                if (string.Equals(rangeEditor.MaxPropertyName, propertyName, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(rangeEditor.MinPropertyName, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string ResolvePropertyName(object dataContext)
        {
            string name = ReadObjectProperty(dataContext, "Name") as string;
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            PropertyDescriptor descriptor = ReadObjectProperty(dataContext, "PropertyDescriptor") as PropertyDescriptor;
            return descriptor?.Name ?? string.Empty;
        }
        private double NormalizeRangeEditorLayout(FrameworkElement element)
        {
            if (!(element is Grid grid)
                || grid.ColumnDefinitions.Count < 5
                || !string.Equals(
                    element.GetType().FullName,
                    "System.Windows.Controls.WpfPropertyGrid.Controls.RangeEditorBase",
                    StringComparison.Ordinal))
            {
                return 0D;
            }

            // Range editors carry label, lower bound, slider, upper bound, and value columns.
            // Keep those columns explicit so long values such as 1000000 do not clip into the slider.
            grid.MinWidth = Math.Max(grid.MinWidth, 390D);
            grid.IsVisibleChanged -= RangeEditor_IsVisibleChanged;
            grid.IsVisibleChanged += RangeEditor_IsVisibleChanged;
            grid.MinHeight = Math.Max(grid.MinHeight, 72D);
            bool darkTheme = ThemeVariant == PropertyGridThemeVariant.Dark;
            if (darkTheme)
            {
                SetDarkThemeValue(grid, Panel.BackgroundProperty, BrushFromRgb(20, 23, 29));
            }
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            grid.Margin = new Thickness(
                grid.Margin.Left,
                Math.Max(grid.Margin.Top, 1D),
                grid.Margin.Right,
                Math.Max(grid.Margin.Bottom, 4D));
            grid.ColumnDefinitions[0].Width = new GridLength(38D);
            grid.ColumnDefinitions[1].Width = new GridLength(52D);
            grid.ColumnDefinitions[2].MinWidth = 130D;
            grid.ColumnDefinitions[3].Width = new GridLength(86D);
            grid.ColumnDefinitions[4].Width = new GridLength(84D);
            grid.RemoveHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(RangeEditorTextBox_TextChanged));
            grid.RemoveHandler(UIElement.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(RangeEditorTextBox_LostKeyboardFocus));
            grid.AddHandler(UIElement.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(RangeEditorTextBox_LostKeyboardFocus), true);
            grid.RemoveHandler(Keyboard.KeyDownEvent, new KeyEventHandler(RangeEditorTextBox_KeyDown));
            grid.AddHandler(Keyboard.KeyDownEvent, new KeyEventHandler(RangeEditorTextBox_KeyDown), true);
            grid.RemoveHandler(RangeBase.ValueChangedEvent, new RoutedPropertyChangedEventHandler<double>(RangeEditorSlider_ValueChanged));
            grid.AddHandler(RangeBase.ValueChangedEvent, new RoutedPropertyChangedEventHandler<double>(RangeEditorSlider_ValueChanged), true);
            BindRangeEditorEndpoints(grid);

            foreach (TextBlock textBlock in FindVisualChildren<TextBlock>(grid))
            {
                if (ReferenceEquals(VisualTreeHelper.GetParent(textBlock), grid) && Grid.GetColumn(textBlock) == 0 && Grid.GetRow(textBlock) is 0 or 1)
                    textBlock.SetCurrentValue(TextBlock.TextProperty, OpenVisionLanguageService.T(Grid.GetRow(textBlock) == 0 ? "PropertyGrid.Range.Lower" : "PropertyGrid.Range.Upper"));
                if (darkTheme)
                {
                    SetDarkThemeValue(textBlock, TextBlock.ForegroundProperty, BrushFromRgb(174, 182, 195));
                }
                textBlock.TextTrimming = TextTrimming.None;
                textBlock.TextWrapping = TextWrapping.NoWrap;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                if ((textBlock.Text ?? string.Empty).Length >= 6)
                {
                    textBlock.MinWidth = Math.Max(textBlock.MinWidth, 72D);
                }
            }

            foreach (TextBox textBox in FindVisualChildren<TextBox>(grid))
            {
                if (darkTheme)
                {
                    ApplyDarkEditorStyle(textBox);
                }
                textBox.MinWidth = Math.Max(textBox.MinWidth, 78D);
                textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                textBox.GotKeyboardFocus -= RangeEditorTextBox_GotKeyboardFocus;
                textBox.GotKeyboardFocus += RangeEditorTextBox_GotKeyboardFocus;
                textBox.LostKeyboardFocus -= RangeEditorTextBox_LostKeyboardFocus;
                textBox.LostKeyboardFocus += RangeEditorTextBox_LostKeyboardFocus;
                textBox.KeyDown -= RangeEditorTextBox_KeyDown;
                textBox.KeyDown += RangeEditorTextBox_KeyDown;
                textBox.TextChanged -= RangeEditorTextBox_TextChanged;
            }

            // The value column must include the existing endpoint margins as well as its numeric minimum.
            double valueColumnWidth = FindRangeEditorValueTextBoxes(grid).Select(textBox => textBox.MinWidth + textBox.Margin.Left + textBox.Margin.Right).DefaultIfEmpty(84D).Max();
            grid.ColumnDefinitions[4].Width = new GridLength(Math.Max(84D, valueColumnWidth));
            grid.MinWidth = Math.Max(grid.MinWidth, grid.ColumnDefinitions.Take(5).Sum(column => column.Width.IsAbsolute ? column.Width.Value : column.MinWidth));
            FrameworkElement presenter = FindVisualParent<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.Design.PropertyEditorContentPresenter>(grid);
            // Carry the current row's width through the vendor presenter, name column and chrome.
            // Visibility changes also schedule normalization, including a collapsed host's reveal.

            foreach (Slider slider in FindVisualChildren<Slider>(grid))
            {
                if (darkTheme)
                {
                    SetDarkThemeValue(slider, Control.ForegroundProperty, BrushFromRgb(46, 196, 182));
                    SetDarkThemeValue(slider, Control.BackgroundProperty, BrushFromRgb(38, 43, 52));
                }
                slider.MinHeight = Math.Max(slider.MinHeight, 20D);
                slider.Margin = new Thickness(slider.Margin.Left, 0D, slider.Margin.Right, 0D);
                slider.ValueChanged -= RangeEditorSlider_ValueChanged;
                slider.ValueChanged += RangeEditorSlider_ValueChanged;
            }

            foreach (CheckBox checkBox in FindVisualChildren<CheckBox>(grid))
            {
                // OpenVision range properties use Min/Max pairs only; threshold inversion is handled by WpgThresholdEditor.
                if (ExtractElementText(checkBox).IndexOf("Invert", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    checkBox.Visibility = Visibility.Collapsed;
                    checkBox.IsHitTestVisible = false;
                }
            }
            Border rowBorder = presenter == null ? null : FindVisualParent<Border>(presenter);
            while (rowBorder != null && rowBorder.Name != "RowBorder") rowBorder = FindVisualParent<Border>(rowBorder);
            if (rowBorder != null) NormalizePropertyRowLayout(rowBorder);
            return grid.IsVisible && presenter != null && innerPropertyGrid.ActualWidth > 0D
                ? grid.MinWidth + grid.Margin.Left + grid.Margin.Right + Math.Max(0D, innerPropertyGrid.ActualWidth - presenter.ActualWidth)
                : 0D;
        }

        private void RangeEditor_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) => ScheduleNormalizeInnerEditorControls();

        private void RangeEditorTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            focusedRangeEditorTextBox = e.OriginalSource as TextBox;
            focusedRangeEditorOriginalText = focusedRangeEditorTextBox?.Text;
        }

        private void RangeEditorTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!ReferenceEquals(e.OriginalSource, focusedRangeEditorTextBox))
            {
                return;
            }

            TextBox editor = focusedRangeEditorTextBox;
            bool changed = !string.Equals(focusedRangeEditorOriginalText, editor.Text, StringComparison.Ordinal)
                && TryParseRangeEditorNumber(editor.Text, out _);
            focusedRangeEditorTextBox = null;
            focusedRangeEditorOriginalText = null;
            if (changed)
            {
                ScheduleRangeEditorPropertyChanged(editor);
            }
        }

        private void BindRangeEditorEndpoints(FrameworkElement rangeEditor)
        {
            object propertyValue = ReadObjectProperty(rangeEditor, "PropertyValue");
            object parentProperty = ReadObjectProperty(propertyValue, "ParentProperty");
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem propertyItem =
                parentProperty as WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem;
            PropertyDescriptor descriptor = GetPropertyDescriptor(propertyItem);
            RangeEditorAttribute rangeEditorAttribute = descriptor?.Attributes[typeof(RangeEditorAttribute)] as RangeEditorAttribute;
            object selectedObject = SelectedObject;
            if (rangeEditorAttribute == null || selectedObject == null)
            {
                return;
            }

            Type selectedType = selectedObject.GetType();
            PropertyInfo minProperty = selectedType.GetProperty(rangeEditorAttribute.MinPropertyName, BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo maxProperty = selectedType.GetProperty(rangeEditorAttribute.MaxPropertyName, BindingFlags.Instance | BindingFlags.Public);
            if (minProperty == null || maxProperty == null || !minProperty.CanWrite || !maxProperty.CanWrite)
            {
                return;
            }

            // WPG's stock RangeEditor stores Min/Max in private endpoint controls. Once the
            // companion Max row is hidden by the bridge, bind those endpoints directly to the
            // model so the operator can still adjust both values from the single range row.
            bool previous = suppressRangeEditorChanges;
            FieldInfo stockUpdating = rangeEditor.GetType().GetField("_updating", BindingFlags.Instance | BindingFlags.NonPublic);
            bool previousStockUpdating = stockUpdating?.GetValue(rangeEditor) is bool updating && updating;
            suppressRangeEditorChanges = true;
            try
            {
                // The stock callback refreshes the hidden companion from the range limit.
                // Initial binding must update the controls without writing that default back.
                stockUpdating?.SetValue(rangeEditor, true);
                BindRangeEditorEndpoint(ReadPrivateField<Slider>(rangeEditor, "_minSlider"), RangeBase.ValueProperty, selectedObject, rangeEditorAttribute.MinPropertyName);
                BindRangeEditorEndpoint(ReadPrivateField<Slider>(rangeEditor, "_maxSlider"), RangeBase.ValueProperty, selectedObject, rangeEditorAttribute.MaxPropertyName);
                BindRangeEditorEndpoint(ReadPrivateField<TextBox>(rangeEditor, "_minBox"), TextBox.TextProperty, selectedObject, rangeEditorAttribute.MinPropertyName);
                BindRangeEditorEndpoint(ReadPrivateField<TextBox>(rangeEditor, "_maxBox"), TextBox.TextProperty, selectedObject, rangeEditorAttribute.MaxPropertyName);
            }
            finally
            {
                stockUpdating?.SetValue(rangeEditor, previousStockUpdating);
                suppressRangeEditorChanges = previous;
            }
        }

        private static void BindRangeEditorEndpoint(FrameworkElement element, DependencyProperty property, object source, string path)
        {
            if (element == null || source == null || string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            UpdateSourceTrigger updateTrigger = property == TextBox.TextProperty ? UpdateSourceTrigger.LostFocus : UpdateSourceTrigger.PropertyChanged;
            Binding current = BindingOperations.GetBinding(element, property);
            // Repeated theme/layout normalization must preserve an uncommitted endpoint edit.
            if (current != null && ReferenceEquals(current.Source, source) && string.Equals(current.Path?.Path, path, StringComparison.Ordinal)
                && current.Mode == BindingMode.TwoWay && current.UpdateSourceTrigger == updateTrigger && current.Converter == null)
                return;

            BindingOperations.SetBinding(
                element,
                property,
                new Binding(path)
                {
                    Source = source,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = updateTrigger
                });
        }

        private void RangeEditorTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ReferenceEquals(e.OriginalSource, focusedRangeEditorTextBox)
                && !string.Equals(focusedRangeEditorOriginalText, focusedRangeEditorTextBox.Text, StringComparison.Ordinal)
                && TryParseRangeEditorNumber(focusedRangeEditorTextBox.Text, out _))
            {
                ScheduleRangeEditorPropertyChanged(focusedRangeEditorTextBox);
                focusedRangeEditorOriginalText = focusedRangeEditorTextBox.Text;
            }
        }

        private void RangeEditorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScheduleRangeEditorPropertyChanged(e.OriginalSource as DependencyObject ?? sender as DependencyObject);
        }

        private void RangeEditorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!Equals(e.OldValue, e.NewValue))
            {
                ScheduleRangeEditorPropertyChanged(e.OriginalSource as DependencyObject ?? sender as DependencyObject);
            }
        }

        private void ScheduleRangeEditorPropertyChanged(DependencyObject source)
        {
            if (suppressRangeEditorChanges) { return; }
            FrameworkElement rangeEditor = FindRangeEditorElement(source);
            if (rangeEditor == null)
            {
                return;
            }

            if (rangeEditor.Dispatcher == null || rangeEditor.Dispatcher.CheckAccess())
            {
                RaiseRangeEditorPropertyChanged(rangeEditor, source);
                return;
            }

            rangeEditor.Dispatcher.BeginInvoke(
                new Action(() => RaiseRangeEditorPropertyChanged(rangeEditor, source)),
                DispatcherPriority.Send);
        }

        private void RaiseRangeEditorPropertyChanged(FrameworkElement rangeEditor, DependencyObject source)
        {
            TextBox minBox = ReadPrivateField<TextBox>(rangeEditor, "_minBox");
            // Reload can blur an editor from the previous SelectedObject after the new one is active.
            if (minBox == null || SelectedObject == null
                || !ReferenceEquals(BindingOperations.GetBinding(minBox, TextBox.TextProperty)?.Source, SelectedObject))
            {
                return;
            }

            object propertyValue = ReadObjectProperty(rangeEditor, "PropertyValue");
            object parentProperty = ReadObjectProperty(propertyValue, "ParentProperty");
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem propertyItem =
                parentProperty as WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem;
            if (propertyItem == null)
            {
                return;
            }

            CommitRangeEditorValues(rangeEditor, source, propertyItem);
            object newValue = PropertyGridPropertyValueChangeSubscription.ReadPropertyItemValue(propertyItem);
            RaisePropertyValueChanged(propertyItem, null, newValue);
        }

        private void CommitRangeEditorValues(
            FrameworkElement rangeEditor,
            DependencyObject source,
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem propertyItem)
        {
            PropertyDescriptor descriptor = GetPropertyDescriptor(propertyItem);
            RangeEditorAttribute rangeEditorAttribute = descriptor?.Attributes[typeof(RangeEditorAttribute)] as RangeEditorAttribute;
            object selectedObject = SelectedObject;
            if (rangeEditorAttribute == null || selectedObject == null)
            {
                return;
            }

            PropertyInfo minProperty = selectedObject.GetType().GetProperty(rangeEditorAttribute.MinPropertyName, BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo maxProperty = selectedObject.GetType().GetProperty(rangeEditorAttribute.MaxPropertyName, BindingFlags.Instance | BindingFlags.Public);
            if (minProperty == null || maxProperty == null || !minProperty.CanWrite || !maxProperty.CanWrite)
            {
                return;
            }

            double minValue = ReadNumericProperty(selectedObject, minProperty);
            double maxValue = ReadNumericProperty(selectedObject, maxProperty);
            List<TextBox> textBoxes = FindRangeEditorValueTextBoxes(rangeEditor);
            List<Slider> sliders = FindRangeEditorValueSliders(rangeEditor);

            if (source is TextBox sourceTextBox)
            {
                int index = textBoxes.IndexOf(sourceTextBox);
                if (index == 0 && TryParseRangeEditorNumber(sourceTextBox.Text, out double value))
                {
                    minValue = value;
                    SyncRangeEditorSlider(sliders, 0, minValue);
                }
                else if (index == 1 && TryParseRangeEditorNumber(sourceTextBox.Text, out value))
                {
                    maxValue = value;
                    SyncRangeEditorSlider(sliders, 1, maxValue);
                }
                else
                {
                    TryReadRangeEditorValue(textBoxes, sliders, 0, ref minValue);
                    TryReadRangeEditorValue(textBoxes, sliders, 1, ref maxValue);
                }
            }
            else if (source is Slider sourceSlider)
            {
                int index = sliders.IndexOf(sourceSlider);
                if (index == 0)
                {
                    minValue = sourceSlider.Value;
                }
                else if (index == 1)
                {
                    maxValue = sourceSlider.Value;
                }
                else
                {
                    TryReadRangeEditorValue(textBoxes, sliders, 0, ref minValue);
                    TryReadRangeEditorValue(textBoxes, sliders, 1, ref maxValue);
                }
            }
            else
            {
                TryReadRangeEditorValue(textBoxes, sliders, 0, ref minValue);
                TryReadRangeEditorValue(textBoxes, sliders, 1, ref maxValue);
            }

            // The original WPG RangeEditor can miss the hidden companion Max property after
            // OpenVision collapses duplicate rows. Commit both endpoints explicitly so Max edits
            // such as Matching FIND_ANGLE_MAX remain operator-editable.
            SetRangeEditorDouble(rangeEditor, rangeEditorAttribute.MinPropertyName, minValue);
            SetRangeEditorDouble(rangeEditor, rangeEditorAttribute.MaxPropertyName, maxValue);
            SetNumericProperty(selectedObject, minProperty, minValue);
            SetNumericProperty(selectedObject, maxProperty, maxValue);
            RefreshRangeEditorFromProperties(rangeEditor);
        }

        private static void SetRangeEditorDouble(FrameworkElement rangeEditor, string propertyName, double value)
        {
            if (rangeEditor == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return;
            }

            try
            {
                MethodInfo method = rangeEditor.GetType().GetMethod("SetDouble", BindingFlags.Instance | BindingFlags.NonPublic);
                method?.Invoke(rangeEditor, new object[] { propertyName, value });
            }
            catch
            {
            }
        }

        private static void RefreshRangeEditorFromProperties(FrameworkElement rangeEditor)
        {
            if (rangeEditor == null)
            {
                return;
            }

            try
            {
                MethodInfo method = rangeEditor.GetType().GetMethod("RefreshFromProperties", BindingFlags.Instance | BindingFlags.NonPublic);
                method?.Invoke(rangeEditor, null);
            }
            catch
            {
            }
        }

        private static void SyncRangeEditorSlider(List<Slider> sliders, int index, double value)
        {
            if (sliders.Count <= index)
            {
                return;
            }

            Slider slider = sliders[index];
            double clamped = Math.Min(slider.Maximum, Math.Max(slider.Minimum, value));
            if (Math.Abs(slider.Value - clamped) > double.Epsilon)
            {
                slider.SetCurrentValue(RangeBase.ValueProperty, clamped);
                slider.GetBindingExpression(RangeBase.ValueProperty)?.UpdateSource();
            }
        }

        private static void TryReadRangeEditorValue(List<TextBox> textBoxes, List<Slider> sliders, int index, ref double value)
        {
            if (textBoxes.Count > index && TryParseRangeEditorNumber(textBoxes[index].Text, out double parsed))
            {
                value = parsed;
                return;
            }

            if (sliders.Count > index)
            {
                value = sliders[index].Value;
            }
        }

        private static List<TextBox> FindRangeEditorValueTextBoxes(FrameworkElement rangeEditor)
        {
            TextBox minBox = ReadPrivateField<TextBox>(rangeEditor, "_minBox");
            TextBox maxBox = ReadPrivateField<TextBox>(rangeEditor, "_maxBox");
            if (minBox != null && maxBox != null)
            {
                return new List<TextBox> { minBox, maxBox };
            }

            List<TextBox> allTextBoxes = FindVisualChildren<TextBox>(rangeEditor)
                .Where(item => item.IsVisible)
                .OrderBy(Grid.GetRow)
                .ThenBy(Grid.GetColumn)
                .ToList();
            Grid grid = rangeEditor as Grid;
            if (grid == null || grid.ColumnDefinitions.Count == 0)
            {
                return allTextBoxes;
            }

            int valueColumn = grid.ColumnDefinitions.Count - 1;
            List<TextBox> valueTextBoxes = allTextBoxes
                .Where(item => GetGridColumnEnd(item) >= valueColumn)
                .ToList();
            return valueTextBoxes.Count >= 2 ? valueTextBoxes : allTextBoxes;
        }

        private static List<Slider> FindRangeEditorValueSliders(FrameworkElement rangeEditor)
        {
            Slider minSlider = ReadPrivateField<Slider>(rangeEditor, "_minSlider");
            Slider maxSlider = ReadPrivateField<Slider>(rangeEditor, "_maxSlider");
            if (minSlider != null && maxSlider != null)
            {
                return new List<Slider> { minSlider, maxSlider };
            }

            List<Slider> allSliders = FindVisualChildren<Slider>(rangeEditor)
                .Where(item => item.IsVisible)
                .OrderBy(Grid.GetRow)
                .ThenBy(Grid.GetColumn)
                .ToList();
            List<Slider> valueSliders = allSliders
                .Where(item => Grid.GetColumn(item) <= 2 && GetGridColumnEnd(item) >= 2)
                .ToList();
            return valueSliders.Count >= 2 ? valueSliders : allSliders;
        }

        private static T ReadPrivateField<T>(object instance, string fieldName)
            where T : class
        {
            if (instance == null || string.IsNullOrWhiteSpace(fieldName))
            {
                return null;
            }

            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return field?.GetValue(instance) as T;
        }

        private static int GetGridColumnEnd(FrameworkElement element)
        {
            return Grid.GetColumn(element) + Math.Max(1, Grid.GetColumnSpan(element)) - 1;
        }

        private static bool TryParseRangeEditorNumber(string text, out double value)
        {
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
            {
                return true;
            }

            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private static double ReadNumericProperty(object instance, PropertyInfo property)
        {
            object value = property.GetValue(instance, null);
            if (value == null)
            {
                return 0D;
            }

            return Convert.ToDouble(value, CultureInfo.CurrentCulture);
        }

        private static void SetNumericProperty(object instance, PropertyInfo property, double value)
        {
            object converted = Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType, CultureInfo.CurrentCulture);
            object currentValue = property.GetValue(instance, null);
            if (Equals(currentValue, converted))
            {
                return;
            }

            property.SetValue(instance, converted, null);
        }

        private static FrameworkElement FindRangeEditorElement(DependencyObject source)
        {
            DependencyObject current = source;
            while (current != null)
            {
                FrameworkElement frameworkElement = current as FrameworkElement;
                if (frameworkElement != null
                    && string.Equals(
                        frameworkElement.GetType().FullName,
                        "System.Windows.Controls.WpfPropertyGrid.Controls.RangeEditorBase",
                        StringComparison.Ordinal))
                {
                    return frameworkElement;
                }

                current = GetParent(current);
            }

            return null;
        }
        private static void ComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null || !comboBox.IsEnabled || comboBox.IsDropDownOpen)
            {
                return;
            }

            comboBox.Focus();
            comboBox.IsDropDownOpen = true;
            e.Handled = true;
        }

        private static void DialogButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ExecuteDialogButtonCommand(sender as Button, e);
        }

        private static void DialogButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteDialogButtonCommand(sender as Button, e);
        }

        private static void ExecuteDialogButtonCommand(Button button, RoutedEventArgs e)
        {
            if (button == null || e == null || e.Handled)
            {
                return;
            }

            object parameter = ResolveDialogPropertyValue(button);
            if (TryInvokeDialogEditorDirect(parameter, button))
            {
                e.Handled = true;
                return;
            }

            ICommand command = button.Command;
            if (command == null)
            {
                return;
            }

            IInputElement target = button.CommandTarget ?? button;
            RoutedCommand routedCommand = command as RoutedCommand;
            if (routedCommand != null)
            {
                if (!routedCommand.CanExecute(parameter, target))
                {
                    return;
                }

                routedCommand.Execute(parameter, target);
                e.Handled = true;
                return;
            }

            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
                e.Handled = true;
            }
        }

        private static object GetDialogPropertyValue(DependencyObject element)
        {
            return element?.GetValue(DialogPropertyValueProperty);
        }

        private static void SetDialogPropertyValue(DependencyObject element, object value)
        {
            element?.SetValue(DialogPropertyValueProperty, value);
        }

        private static object ResolveDialogPropertyValue(Button button)
        {
            if (button == null)
            {
                return null;
            }

            object attachedValue = GetDialogPropertyValue(button);
            object resolvedAttached = ResolvePropertyItemValue(attachedValue);
            if (resolvedAttached != null)
            {
                return resolvedAttached;
            }

            object resolvedCommandParameter = ResolvePropertyItemValue(button.CommandParameter);
            if (resolvedCommandParameter != null)
            {
                return resolvedCommandParameter;
            }

            object resolvedDataContext = ResolvePropertyItemValue(button.DataContext);
            if (resolvedDataContext != null)
            {
                return resolvedDataContext;
            }

            DependencyObject current = button;
            while (current != null)
            {
                FrameworkElement frameworkElement = current as FrameworkElement;
                if (frameworkElement != null)
                {
                    object resolved = ResolvePropertyItemValue(frameworkElement.DataContext);
                    if (resolved != null)
                    {
                        return resolved;
                    }
                }

                current = GetParent(current);
            }

            return null;
        }

        private static object ResolvePropertyItemValue(object candidate)
        {
            if (candidate == null)
            {
                return null;
            }

            if (IsOriginalPropertyItemValue(candidate))
            {
                return candidate;
            }

            if (IsOriginalPropertyItem(candidate))
            {
                return ReadObjectProperty(candidate, "PropertyValue");
            }

            object parentProperty = ReadObjectProperty(candidate, "ParentProperty");
            if (parentProperty != null && IsOriginalPropertyItem(parentProperty))
            {
                return candidate;
            }

            object propertyValue = ReadObjectProperty(candidate, "PropertyValue");
            return IsOriginalPropertyItemValue(propertyValue) ? propertyValue : null;
        }

        private static bool TryInvokeDialogEditorDirect(object propertyValue, IInputElement commandSource)
        {
            MethodInfo showDialog = GetDialogEditorShowDialogMethod(propertyValue);
            if (showDialog != null)
            {
                object parentProperty = ReadObjectProperty(propertyValue, "ParentProperty");
                object editor = ReadObjectProperty(parentProperty, "Editor");
                try
                {
                    showDialog.Invoke(editor, new object[] { propertyValue, commandSource });
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return TryInvokeBridgeDialogEditor(propertyValue, commandSource);
        }

        private static bool TryInvokeBridgeDialogEditor(object propertyValue, IInputElement commandSource)
        {
            Type editorType = GetBridgeDialogEditorType(propertyValue);
            if (editorType == null)
            {
                return false;
            }

            try
            {
                Editor editor = Activator.CreateInstance(editorType) as Editor;
                if (editor == null)
                {
                    return false;
                }

                editor.ShowDialog(new PropertyItemValue(propertyValue), commandSource);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool CanInvokeDialogEditorDirect(object propertyValue)
        {
            return GetDialogEditorShowDialogMethod(propertyValue) != null
                || GetBridgeDialogEditorType(propertyValue) != null;
        }

        private static MethodInfo GetDialogEditorShowDialogMethod(object propertyValue)
        {
            if (!IsOriginalPropertyItemValue(propertyValue) || IsReadOnlyPropertyValue(propertyValue))
            {
                return null;
            }

            object parentProperty = ReadObjectProperty(propertyValue, "ParentProperty");
            object editor = ReadObjectProperty(parentProperty, "Editor");
            if (editor == null)
            {
                return null;
            }

            return editor.GetType().GetMethod(
                "ShowDialog",
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new[] { propertyValue.GetType(), typeof(IInputElement) },
                null);
        }

        private static Type GetBridgeDialogEditorType(object propertyValue)
        {
            if (!IsOriginalPropertyItemValue(propertyValue) || IsReadOnlyPropertyValue(propertyValue))
            {
                return null;
            }

            object parentProperty = ReadObjectProperty(propertyValue, "ParentProperty");
            PropertyDescriptor descriptor = ReadObjectProperty(parentProperty, "PropertyDescriptor") as PropertyDescriptor;
            PropertyEditorAttribute attribute = descriptor?.Attributes[typeof(PropertyEditorAttribute)] as PropertyEditorAttribute;
            return attribute?.EditorType;
        }

        private static bool IsReadOnlyPropertyValue(object propertyValue)
        {
            if (propertyValue == null)
            {
                return true;
            }

            if (ReadBoolProperty(propertyValue, "IsReadOnly"))
            {
                return true;
            }

            object parentProperty = ReadObjectProperty(propertyValue, "ParentProperty");
            return ReadBoolProperty(parentProperty, "IsReadOnly");
        }

        private static object ReadObjectProperty(object instance, string propertyName)
        {
            if (instance == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            try
            {
                PropertyInfo property = instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                return property?.GetValue(instance, null);
            }
            catch
            {
                return null;
            }
        }

        private static bool ReadBoolProperty(object instance, string propertyName)
        {
            object value = ReadObjectProperty(instance, propertyName);
            return value is bool typedValue && typedValue;
        }

        private static bool IsOriginalPropertyItemValue(object value)
        {
            string typeName = value?.GetType().FullName ?? string.Empty;
            return typeName.EndsWith(".PropertyItemValue", StringComparison.Ordinal)
                && typeName.Contains("WpfPropertyGrid", StringComparison.Ordinal);
        }

        private static bool IsOriginalPropertyItem(object value)
        {
            string typeName = value?.GetType().FullName ?? string.Empty;
            return typeName.EndsWith(".PropertyItem", StringComparison.Ordinal)
                && typeName.Contains("WpfPropertyGrid", StringComparison.Ordinal);
        }

        private static DependencyObject GetParent(DependencyObject current)
        {
            if (current == null)
            {
                return null;
            }

            try
            {
                DependencyObject visualParent = VisualTreeHelper.GetParent(current);
                if (visualParent != null)
                {
                    return visualParent;
                }
            }
            catch
            {
            }

            return LogicalTreeHelper.GetParent(current);
        }

        public object Layout
        {
            get { return innerPropertyGrid.Layout; }
            set { innerPropertyGrid.Layout = OriginalValue.Unwrap(value) as Control; }
        }

        private void TrySetInnerProperty(string propertyName, object value)
        {
            PropertyInfo property = innerPropertyGrid.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property == null || !property.CanWrite)
            {
                return;
            }

            try
            {
                property.SetValue(innerPropertyGrid, value, null);
            }
            catch
            {
                // Older property-grid DLLs do not expose every display option.
            }
        }

        private void InnerPropertyGrid_PropertyValueChanged(
            object sender,
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyValueChangedEventArgs e)
        {
            // Template initialization can convert the selected color name back to the same value.
            if (!object.Equals(e.OldValue, PropertyGridPropertyValueChangeSubscription.ReadPropertyItemValue(e.Property))) RaisePropertyValueChanged(e.Property, null, null);
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            object selectedObject = SelectedObject;
            if (selectedObject == null)
            {
                return;
            }

            TypeDescriptor.Refresh(selectedObject.GetType());
            TypeDescriptor.Refresh(selectedObject);
            metadataCacheClearPending = true;
            RefreshSelectedObject(selectedObject);
        }

        private void SubscribeLanguageChanged()
        {
            if (languageChangedSubscribed)
            {
                return;
            }

            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
            languageChangedSubscribed = true;
        }

        private void UnsubscribeLanguageChanged()
        {
            if (!languageChangedSubscribed)
            {
                return;
            }

            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            languageChangedSubscribed = false;
        }

        public void SetPropertyBrowsable(string propertyName, bool isBrowsable)
        {
            SetPropertyBrowsable(propertyName, isBrowsable, false);
        }

        public void RefreshSelectedObject()
        {
            object selectedObject = SelectedObject;
            if (selectedObject != null)
            {
                RefreshSelectedObject(selectedObject);
            }
        }

        public void BeginInitialPropertyViewport(object selectedObject, int visiblePropertyCount)
        {
            if (selectedObject == null || visiblePropertyCount <= 0)
            {
                return;
            }

            Type selectedType = selectedObject.GetType();
            EnsurePropertyGridProvider(selectedType);
            lock (browsabilityLock)
            {
                progressivePropertyViewports.Remove(selectedObject);
                progressivePropertyViewports.Add(selectedObject, new ProgressivePropertyViewportState(visiblePropertyCount));
            }

            TypeDescriptor.Refresh(selectedType);
            TypeDescriptor.Refresh(selectedObject);
        }

        public void CompleteInitialPropertyViewport(object selectedObject)
        {
            if (selectedObject == null)
            {
                return;
            }

            bool removed;
            lock (browsabilityLock)
            {
                removed = progressivePropertyViewports.Remove(selectedObject);
            }

            if (!removed)
            {
                return;
            }

            ClearOriginalMetadataRepositoryCache();
            metadataCacheClearPending = false;
            TypeDescriptor.Refresh(selectedObject.GetType());
            TypeDescriptor.Refresh(selectedObject);
            RefreshSelectedObject(selectedObject);
        }
        internal void SetPropertyBrowsable(string propertyName, bool isBrowsable, bool refreshGrid)
        {
            object selectedObject = SelectedObject;
            if (selectedObject == null || string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            Type selectedType = selectedObject.GetType();
            EnsurePropertyGridProvider(selectedType);
            bool declaredBrowsable = IsDeclaredPropertyBrowsable(selectedType, propertyName);
            bool changed;
            lock (browsabilityLock)
            {
                if (!hiddenPropertiesByType.TryGetValue(selectedType, out HashSet<string> hiddenProperties))
                {
                    hiddenProperties = new HashSet<string>();
                    hiddenPropertiesByType[selectedType] = hiddenProperties;
                }

                changed = isBrowsable
                    ? hiddenProperties.Remove(propertyName)
                    : hiddenProperties.Add(propertyName);
            }

            if (changed && refreshGrid)
            {
                ClearOriginalMetadataRepositoryCache();
                metadataCacheClearPending = false;
                TypeDescriptor.Refresh(selectedType);
                TypeDescriptor.Refresh(selectedObject);
                RefreshSelectedObject(selectedObject);
            }
            else if (changed)
            {
                metadataCacheClearPending = true;
            }

            bool previousSuppressPropertyValueChanged = suppressPropertyValueChanged;
            suppressPropertyValueChanged = true;
            try
            {
                // Visibility rules reshape the generated WPG rows only. Treating IsBrowsable
                // writes as value changes can accidentally schedule vision previews from option toggles.
                SetInnerPropertyBrowsable(propertyName, isBrowsable && declaredBrowsable);
            }
            finally
            {
                suppressPropertyValueChanged = previousSuppressPropertyValueChanged;
            }
        }

        private void RefreshSelectedObject(object selectedObject)
        {
            SaveNavigationState();
            suppressSelectedObjectsChanged = true;
            bool previousSuppressPropertyValueChanged = suppressPropertyValueChanged;
            suppressPropertyValueChanged = true;
            try
            {
                propertyValueChangeSubscription.Detach();
                AssignSelectedObject(null);
                if (metadataCacheClearPending)
                {
                    ClearOriginalMetadataRepositoryCache();
                    metadataCacheClearPending = false;
                }

                RegisterComparers(selectedObject);
                AssignSelectedObject(selectedObject);
                ApplyHiddenPropertyBrowsableState(selectedObject);
                propertyValueChangeSubscription.Attach(innerPropertyGrid.Properties);
                ScheduleNormalizeInnerEditorControls();
                ScheduleRestoreNavigationState(selectedObject);
            }
            finally
            {
                suppressPropertyValueChanged = previousSuppressPropertyValueChanged;
                suppressSelectedObjectsChanged = false;
            }
        }

        private void AssignSelectedObject(object selectedObject)
        {
            try
            {
                innerPropertyGrid.SelectedObject = selectedObject;
            }
            catch (ArgumentException ex) when (IsDuplicateMetadataKeyException(ex) && selectedObject != null)
            {
                // WPG keeps metadata globally; clear and retry when stale attached-property keys collide.
                ClearOriginalMetadataRepositoryCache();
                TypeDescriptor.Refresh(selectedObject.GetType());
                TypeDescriptor.Refresh(selectedObject);
                innerPropertyGrid.SelectedObject = selectedObject;
            }
        }

        private static bool IsDuplicateMetadataKeyException(ArgumentException ex)
        {
            string message = ex?.Message ?? string.Empty;
            return message.IndexOf("same key", StringComparison.OrdinalIgnoreCase) >= 0
                || message.IndexOf("same key has already been added", StringComparison.OrdinalIgnoreCase) >= 0
                || message.IndexOf("같은 키", StringComparison.OrdinalIgnoreCase) >= 0;
        }
        private void ApplyHiddenPropertyBrowsableState(object selectedObject)
        {
            Type selectedType = selectedObject?.GetType();
            if (selectedType == null)
            {
                return;
            }

            string[] hiddenProperties;
            lock (browsabilityLock)
            {
                if (!hiddenPropertiesByType.TryGetValue(selectedType, out HashSet<string> properties)
                    || properties.Count == 0)
                {
                    return;
                }

                hiddenProperties = new string[properties.Count];
                properties.CopyTo(hiddenProperties);
            }

            foreach (string propertyName in hiddenProperties)
            {
                SetInnerPropertyBrowsable(propertyName, false);
            }
        }

        private void RegisterComparers(object selectedObject)
        {
            Type selectedType = selectedObject?.GetType();
            innerPropertyGrid.PropertyComparer = new BridgePropertyComparer(selectedType);
            innerPropertyGrid.CategoryComparer = new BridgeCategoryComparer(selectedType);
        }

        private static void ClearOriginalMetadataRepositoryCache()
        {
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.MetadataRepository.Clear();
        }

        internal bool IsPropertyBrowsable(string propertyName)
        {
            object selectedObject = SelectedObject;
            if (selectedObject == null || string.IsNullOrEmpty(propertyName))
            {
                return true;
            }

            lock (browsabilityLock)
            {
                return !hiddenPropertiesByType.TryGetValue(selectedObject.GetType(), out HashSet<string> hiddenProperties)
                    || !hiddenProperties.Contains(propertyName);
            }
        }

        internal static void RegisterHiddenPropertiesForType(Type selectedType, IEnumerable<string> propertyNames)
        {
            if (selectedType == null || propertyNames == null)
            {
                return;
            }

            lock (browsabilityLock)
            {
                if (!hiddenPropertiesByType.TryGetValue(selectedType, out HashSet<string> hiddenProperties))
                {
                    hiddenProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    hiddenPropertiesByType[selectedType] = hiddenProperties;
                }

                foreach (string propertyName in propertyNames)
                {
                    if (!string.IsNullOrWhiteSpace(propertyName))
                    {
                        hiddenProperties.Add(propertyName);
                    }
                }
            }

        }

        internal static bool TryGetProgressivePropertyViewport(object instance, out int visiblePropertyCount)
        {
            visiblePropertyCount = 0;
            if (instance == null)
            {
                return false;
            }

            lock (browsabilityLock)
            {
                if (!progressivePropertyViewports.TryGetValue(instance, out ProgressivePropertyViewportState state))
                {
                    return false;
                }

                visiblePropertyCount = state.VisiblePropertyCount;
                return visiblePropertyCount > 0;
            }
        }

        private sealed class ProgressivePropertyViewportState
        {
            public ProgressivePropertyViewportState(int visiblePropertyCount)
            {
                VisiblePropertyCount = visiblePropertyCount;
            }

            public int VisiblePropertyCount { get; }
        }

        private sealed class PropertyGridNavigationState
        {
            public PropertyGridNavigationState(double verticalOffset, string searchText)
            {
                VerticalOffset = verticalOffset;
                SearchText = searchText ?? string.Empty;
            }

            public double VerticalOffset { get; }
            public string SearchText { get; }
        }
        private void SetInnerPropertyBrowsable(string propertyName, bool isBrowsable)
        {
            object innerItem = FindInnerPropertyItem(propertyName);
            PropertyInfo property = innerItem?.GetType().GetProperty("IsBrowsable");
            if (property != null && property.CanWrite)
            {
                object currentValue = property.GetValue(innerItem, null);
                if (currentValue is bool current && current == isBrowsable)
                {
                    return;
                }

                property.SetValue(innerItem, isBrowsable, null);
            }
        }

        private object FindInnerPropertyItem(string propertyName)
        {
            object innerCollection = innerPropertyGrid?.Properties;
            if (innerCollection == null || string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            try
            {
                MethodInfo method = innerCollection.GetType().GetMethod("get_Item", new[] { typeof(string) });
                object item = method?.Invoke(innerCollection, new object[] { propertyName });
                if (item != null)
                {
                    return item;
                }
            }
            catch
            {
            }

            if (innerCollection is IEnumerable items)
            {
                foreach (object item in items)
                {
                    PropertyDescriptor descriptor = GetPropertyDescriptor(item);
                    string name = descriptor?.Name
                        ?? GetInnerValue<string>(item, "Name")
                        ?? GetInnerValue<string>(item, "DisplayName");
                    if (string.Equals(name, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        private static PropertyDescriptor GetPropertyDescriptor(object propertyObject)
        {
            PropertyInfo property = propertyObject?.GetType().GetProperty("PropertyDescriptor");
            return property?.GetValue(propertyObject, null) as PropertyDescriptor;
        }

        private static T GetInnerValue<T>(object source, string propertyName)
        {
            PropertyInfo property = source?.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return default(T);
            }

            object value = property.GetValue(source, null);
            return value is T typedValue ? typedValue : default(T);
        }

        private static bool IsDeclaredPropertyBrowsable(Type selectedType, string propertyName)
        {
            PropertyInfo property = selectedType?.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            BrowsableAttribute attribute = property?.GetCustomAttribute<BrowsableAttribute>(true);
            return attribute?.Browsable ?? true;
        }

        internal object GetPropertyValue(string propertyName)
        {
            PropertyInfo property = SelectedObject?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            return property?.GetValue(SelectedObject, null);
        }

        internal void SetPropertyValue(string propertyName, object value)
        {
            PropertyInfo property = SelectedObject?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property != null
                && property.CanWrite
                && property.GetCustomAttribute<PropertyGridCompatibilityReadOnlyAttribute>(true) == null)
            {
                object oldValue = property.GetValue(SelectedObject, null);
                property.SetValue(SelectedObject, value, null);
                RefreshSelectedObject(SelectedObject);
                RaisePropertyValueChanged(new PropertyItem(this, propertyName), oldValue, value);
            }
        }

        internal bool IsPropertyReadOnly(string propertyName)
        {
            PropertyInfo property = SelectedObject?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            return property == null
                || !property.CanWrite
                || property.GetCustomAttribute<PropertyGridCompatibilityReadOnlyAttribute>(true) != null;
        }

        internal bool HasClrProperty(string propertyName)
        {
            return SelectedObject?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public) != null;
        }

        internal string ResolveClrPropertyName(string candidate)
        {
            if (string.IsNullOrEmpty(candidate))
            {
                return string.Empty;
            }

            Type selectedType = SelectedObject?.GetType();
            if (selectedType == null)
            {
                return candidate;
            }

            foreach (PropertyInfo property in selectedType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (string.Equals(property.Name, candidate, StringComparison.OrdinalIgnoreCase))
                {
                    return property.Name;
                }

                DisplayNameAttribute displayName = property.GetCustomAttribute<DisplayNameAttribute>(true);
                if (displayName != null && string.Equals(displayName.DisplayName, candidate, StringComparison.CurrentCultureIgnoreCase))
                {
                    return property.Name;
                }
            }

            return candidate;
        }

        private static void EnsurePropertyGridProvider(Type type)
        {
            if (type == null)
            {
                return;
            }

            lock (browsabilityLock)
            {
                if (registeredBrowsableProviderTypes.Contains(type))
                {
                    return;
                }

                TypeDescriptor.AddProviderTransparent(
                    new DynamicPropertyGridTypeDescriptionProvider(TypeDescriptor.GetProvider(type)),
                    type);
                registeredBrowsableProviderTypes.Add(type);
            }
        }

        internal static bool IsPropertyHidden(Type type, string propertyName)
        {
            lock (browsabilityLock)
            {
                return hiddenPropertiesByType.TryGetValue(type, out HashSet<string> hiddenProperties)
                    && hiddenProperties.Contains(propertyName);
            }
        }

        private void RegisterPropertyEditors(object selectedObject)
        {
            if (selectedObject == null)
            {
                return;
            }

            Type selectedType = selectedObject.GetType();
            foreach (PropertyInfo property in selectedType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                PropertyEditorAttribute attribute = property.GetCustomAttribute<PropertyEditorAttribute>(true);
                if (attribute == null || attribute.EditorType == null)
                {
                    continue;
                }

                string key = selectedType.AssemblyQualifiedName + "|" + property.Name + "|" + attribute.EditorType.AssemblyQualifiedName;
                if (!registeredPropertyEditors.Add(key))
                {
                    continue;
                }

                object editorObject = Activator.CreateInstance(attribute.EditorType);
                Editor bridgeEditor = editorObject as Editor;
                if (bridgeEditor != null)
                {
                    innerPropertyGrid.Editors.Add(new OriginalPropertyEditorAdapter(selectedType, property.Name, bridgeEditor));
                    continue;
                }

                WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.Editor originalEditor =
                    editorObject as WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.Editor;
                if (originalEditor != null)
                {
                    innerPropertyGrid.Editors.Add(originalEditor);
                }
            }
        }

        private void RaisePropertyValueChanged(WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem property, object oldValue, object newValue)
        {
            RaisePropertyValueChanged(new PropertyItem(this, property), oldValue, newValue);
        }

        private void RaisePropertyValueChanged(IPropertyGridProperty property, object oldValue, object newValue)
        {
            if (suppressPropertyValueChanged || suppressRangeEditorChanges)
            {
                return;
            }

            PropertyValueChanged?.Invoke(
                this,
                new OpenVisionLab.PropertyGrid.PropertyGridPropertyValueChangedEventArgs(property, SelectedObject, oldValue, newValue));
        }
    }

    public class PropertyValueChangedEventArgs : PropertyGridPropertyValueChangedEventArgs
    {
        public PropertyValueChangedEventArgs(PropertyItem property)
            : base(property)
        {
            Property = property;
        }

        public PropertyValueChangedEventArgs(PropertyItem property, object targetObject, object oldValue, object newValue)
            : base(property, targetObject, oldValue, newValue)
        {
            Property = property;
        }

        public new PropertyItem Property { get; }
    }

    public class PropertyItemCollection : IPropertyGridPropertyCollection
    {
        private readonly PropertyGrid owner;
        private readonly object innerCollection;

        internal PropertyItemCollection(PropertyGrid owner, object innerCollection)
        {
            this.owner = owner;
            this.innerCollection = innerCollection;
        }

        public PropertyItem this[string propertyName]
        {
            get
            {
                object innerItem = GetInnerItem(propertyName);
                if (innerItem != null)
                {
                    return new PropertyItem(owner, innerItem);
                }

                return owner != null && owner.HasClrProperty(propertyName) ? new PropertyItem(owner, propertyName) : null;
            }
        }

        private object GetInnerItem(string propertyName)
        {
            if (innerCollection == null || string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            try
            {
                MethodInfo method = innerCollection.GetType().GetMethod("get_Item", new[] { typeof(string) });
                return method?.Invoke(innerCollection, new object[] { propertyName });
            }
            catch
            {
                foreach (object propertyObject in (IEnumerable)innerCollection)
                {
                    PropertyDescriptor descriptor = GetPropertyDescriptor(propertyObject);
                    string name = descriptor?.Name ?? GetValue<string>(propertyObject, "Name") ?? GetValue<string>(propertyObject, "DisplayName");
                    if (string.Equals(name, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return propertyObject;
                    }
                }
            }

            return null;
        }

        private static PropertyDescriptor GetPropertyDescriptor(object propertyObject)
        {
            PropertyInfo property = propertyObject?.GetType().GetProperty("PropertyDescriptor");
            return property?.GetValue(propertyObject, null) as PropertyDescriptor;
        }

        private static T GetValue<T>(object source, string propertyName)
        {
            PropertyInfo property = source?.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return default(T);
            }

            object value = property.GetValue(source, null);
            return value is T typedValue ? typedValue : default(T);
        }

        IPropertyGridProperty IPropertyGridPropertyCollection.this[string propertyName] => this[propertyName];
    }

    public class PropertyItem : IPropertyGridProperty
    {
        private readonly PropertyGrid owner;
        private readonly object innerItem;
        private readonly string propertyName;

        internal PropertyItem(object innerItem)
            : this(null, innerItem)
        {
        }

        internal PropertyItem(PropertyGrid owner, object innerItem)
        {
            this.owner = owner;
            this.innerItem = innerItem;
        }

        internal PropertyItem(PropertyGrid owner, string propertyName)
        {
            this.owner = owner;
            this.propertyName = propertyName;
        }

        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(propertyName))
                {
                    return propertyName;
                }

                PropertyDescriptor descriptor = GetPropertyDescriptor();
                if (descriptor != null)
                {
                    return descriptor.Name;
                }

                string name = GetValue<string>("Name");
                if (!string.IsNullOrEmpty(name))
                {
                    return owner?.ResolveClrPropertyName(name) ?? name;
                }

                string displayName = GetValue<string>("DisplayName");
                return owner?.ResolveClrPropertyName(displayName) ?? displayName;
            }
        }

        public bool IsBrowsable
        {
            get
            {
                if (innerItem != null)
                {
                    return GetValue<bool>("IsBrowsable") && (owner == null || owner.IsPropertyBrowsable(Name));
                }

                return owner == null || owner.IsPropertyBrowsable(Name);
            }
            set
            {
                bool innerUpdated = TrySetInnerBrowsable(value);
                if (!innerUpdated && owner != null)
                {
                    owner.SetPropertyBrowsable(Name, value, true);
                }

                if (owner == null && !innerUpdated)
                {
                    SetPropertyValue("IsBrowsable", value);
                }
            }
        }

        public bool IsReadOnly => !string.IsNullOrEmpty(propertyName) ? owner?.IsPropertyReadOnly(propertyName) ?? true : GetValue<bool>("IsReadOnly");

        public void SetValue(object value)
        {
            PropertyDescriptor descriptor = GetPropertyDescriptor();
            if ((!string.IsNullOrEmpty(propertyName)
                    && owner?.IsPropertyReadOnly(propertyName) == true)
                || descriptor?.Attributes[typeof(PropertyGridCompatibilityReadOnlyAttribute)]
                    is PropertyGridCompatibilityReadOnlyAttribute)
            {
                return;
            }

            if (!string.IsNullOrEmpty(propertyName))
            {
                owner?.SetPropertyValue(propertyName, value);
                return;
            }

            MethodInfo method = innerItem.GetType().GetMethod("SetValue", new[] { typeof(object) });
            if (method != null)
            {
                method.Invoke(innerItem, new[] { value });
                return;
            }

            SetPropertyValue("Value", value);
        }

        private T GetValue<T>(string propertyName)
        {
            if (innerItem == null)
            {
                return default(T);
            }

            PropertyInfo property = innerItem.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return default(T);
            }

            object value = property.GetValue(innerItem, null);
            return value is T typedValue ? typedValue : default(T);
        }

        private void SetPropertyValue(string propertyName, object value)
        {
            if (innerItem == null)
            {
                return;
            }

            PropertyInfo property = innerItem.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(innerItem, value, null);
            }
        }

        private bool TrySetInnerBrowsable(bool value)
        {
            if (innerItem == null)
            {
                return false;
            }

            PropertyInfo property = innerItem.GetType().GetProperty("IsBrowsable");
            if (property == null || !property.CanWrite)
            {
                return false;
            }

            property.SetValue(innerItem, value, null);
            return true;
        }

        private PropertyDescriptor GetPropertyDescriptor()
        {
            if (innerItem == null)
            {
                return null;
            }

            PropertyInfo property = innerItem.GetType().GetProperty("PropertyDescriptor");
            return property?.GetValue(innerItem, null) as PropertyDescriptor;
        }
    }

    public class PropertyItemValue
    {
        private readonly object innerValue;

        public PropertyItemValue()
        {
        }

        internal PropertyItemValue(object innerValue)
        {
            this.innerValue = innerValue;
        }

        public object Value
        {
            get { return GetValue<object>("Value"); }
            set { SetPropertyValue("Value", value); }
        }

        public string StringValue
        {
            get { return GetValue<string>("StringValue"); }
            set { SetPropertyValue("StringValue", value); }
        }

        public PropertyItem ParentProperty
        {
            get
            {
                object parentProperty = GetValue<object>("ParentProperty");
                return parentProperty == null ? null : new PropertyItem(parentProperty);
            }
            set { SetPropertyValue("ParentProperty", value); }
        }

        private T GetValue<T>(string propertyName)
        {
            if (innerValue == null)
            {
                return default(T);
            }

            PropertyInfo property = innerValue.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return default(T);
            }

            object value = property.GetValue(innerValue, null);
            return value is T typedValue ? typedValue : default(T);
        }

        private void SetPropertyValue(string propertyName, object value)
        {
            if (innerValue == null)
            {
                return;
            }

            PropertyInfo property = innerValue.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(innerValue, value, null);
            }
        }
    }

    public class Editor
    {
        public object InlineTemplate { get; set; }
        public object ExtendedTemplate { get; set; }
        public object DialogTemplate { get; set; }

        public virtual void ShowDialog(PropertyItemValue propertyValue, IInputElement commandSource)
        {
        }
    }

    public class PropertyEditor : Editor
    {
    }

    internal sealed class OriginalPropertyEditorAdapter
        : WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyEditor
    {
        private readonly Editor bridgeEditor;

        public OriginalPropertyEditorAdapter(Type declaringType, string propertyName, Editor bridgeEditor)
            : base(declaringType, propertyName)
        {
            this.bridgeEditor = bridgeEditor;
            InlineTemplate = OriginalValue.Unwrap(bridgeEditor.InlineTemplate);
            ExtendedTemplate = OriginalValue.Unwrap(bridgeEditor.ExtendedTemplate);
            DialogTemplate = OriginalValue.Unwrap(bridgeEditor.DialogTemplate);
        }

        public override void ShowDialog(
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItemValue propertyValue,
            IInputElement commandSource)
        {
            bridgeEditor.ShowDialog(new PropertyItemValue(propertyValue), commandSource);
        }
    }

    public static class EditorKeys
    {
        public static object SliderEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.SliderEditorKey;
        public static object ThresholdEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.ThresholdEditorKey;
        public static object RangeEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.RangeEditorKey;
        public static object MetricRangeEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.MetricRangeEditorKey;
        public static object DoubleEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.DoubleEditorKey;
        public static object BrushEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.BrushEditorKey;
        public static object FilePathPickerEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.FilePathPickerEditorKey;
        public static object ComplexPropertyEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.ComplexPropertyEditorKey;
    }

    public static class KnownTypes
    {
        public static class Collections
        {
        }

        public static class Attributes
        {
        }

        public static class Wpf
        {
        }

        public static class Wpg
        {
        }
    }

    internal interface IOriginalValue
    {
        object OriginalValue { get; }
    }

    internal static class OriginalValue
    {
        public static object Unwrap(object value)
        {
            return value is IOriginalValue originalValue ? originalValue.OriginalValue : value;
        }
    }
}

namespace System.Windows.Controls.WpfPropertyGrid.Design
{
    public sealed class CategorizedLayout : System.Windows.Controls.WpfPropertyGrid.IOriginalValue
    {
        private readonly object originalValue = new WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.Design.CategorizedLayout();

        object System.Windows.Controls.WpfPropertyGrid.IOriginalValue.OriginalValue => originalValue;
    }

    public sealed class AlphabeticalLayout : System.Windows.Controls.WpfPropertyGrid.IOriginalValue
    {
        private readonly object originalValue = new WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.Design.AlphabeticalLayout();

        object System.Windows.Controls.WpfPropertyGrid.IOriginalValue.OriginalValue => originalValue;
    }
}

namespace System.Windows.Controls.WpfPropertyGrid.Controls
{
    public enum SearchMode
    {
        Contains,
        StartsWith
    }

    public class DoubleEditor
    {
    }
}
