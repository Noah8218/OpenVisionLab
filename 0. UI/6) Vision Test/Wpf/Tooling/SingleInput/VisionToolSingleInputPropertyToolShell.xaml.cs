using MahApps.Metro.IconPacks;
using System;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class VisionToolSingleInputPropertyToolShell : UserControl
    {
        private readonly DockedInspectorLayoutController layoutController;
        private readonly VisionToolLearnWindowController learnWindowController;

        public static readonly DependencyProperty TitleIconKindProperty =
            DependencyProperty.Register(
                nameof(TitleIconKind),
                typeof(PackIconMaterialKind),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata(PackIconMaterialKind.Tools));

        public static readonly DependencyProperty TemplateStatusVisibilityProperty =
            DependencyProperty.Register(
                nameof(TemplateStatusVisibility),
                typeof(Visibility),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty PropertyGridHostVisibilityProperty =
            DependencyProperty.Register(
                nameof(PropertyGridHostVisibility),
                typeof(Visibility),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty ToolContentVisibilityProperty =
            DependencyProperty.Register(
                nameof(ToolContentVisibility),
                typeof(Visibility),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty ParameterContentVisibilityProperty =
            DependencyProperty.Register(
                nameof(ParameterContentVisibility),
                typeof(Visibility),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty ResultReviewVisibilityProperty =
            DependencyProperty.Register(
                nameof(ResultReviewVisibility),
                typeof(Visibility),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty IsDockedInspectorModeProperty =
            DependencyProperty.Register(
                nameof(IsDockedInspectorMode),
                typeof(bool),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata(false, OnIsDockedInspectorModeChanged));

        public static readonly DependencyProperty ParameterContentProperty =
            DependencyProperty.Register(
                nameof(ParameterContent),
                typeof(object),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ToolContentProperty =
            DependencyProperty.Register(
                nameof(ToolContent),
                typeof(object),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata(null));

        public static readonly DependencyProperty LearnButtonVisibilityProperty =
            DependencyProperty.Register(
                nameof(LearnButtonVisibility),
                typeof(Visibility),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty LearnButtonTextProperty =
            DependencyProperty.Register(
                nameof(LearnButtonText),
                typeof(string),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata("Learn"));

        public static readonly DependencyProperty LearnTopicIndexProperty =
            DependencyProperty.Register(
                nameof(LearnTopicIndex),
                typeof(int),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata(0));

        public VisionToolSingleInputPropertyToolShell()
        {
            InitializeComponent();
            layoutController = new DockedInspectorLayoutController(this);
            learnWindowController = new VisionToolLearnWindowController(() => Window.GetWindow(this));
            ApplyDockedInspectorMode();
        }

        public event EventHandler DockedInspectorModeChanged;

        public PackIconMaterialKind TitleIconKind
        {
            get => (PackIconMaterialKind)GetValue(TitleIconKindProperty);
            set => SetValue(TitleIconKindProperty, value);
        }

        public Visibility TemplateStatusVisibility
        {
            get => (Visibility)GetValue(TemplateStatusVisibilityProperty);
            set => SetValue(TemplateStatusVisibilityProperty, value);
        }

        public Visibility PropertyGridHostVisibility
        {
            get => (Visibility)GetValue(PropertyGridHostVisibilityProperty);
            set => SetValue(PropertyGridHostVisibilityProperty, value);
        }

        public Visibility ToolContentVisibility
        {
            get => (Visibility)GetValue(ToolContentVisibilityProperty);
            set => SetValue(ToolContentVisibilityProperty, value);
        }

        public Visibility ParameterContentVisibility
        {
            get => (Visibility)GetValue(ParameterContentVisibilityProperty);
            set => SetValue(ParameterContentVisibilityProperty, value);
        }

        public Visibility ResultReviewVisibility
        {
            get => (Visibility)GetValue(ResultReviewVisibilityProperty);
            set => SetValue(ResultReviewVisibilityProperty, value);
        }

        public bool IsDockedInspectorMode
        {
            get => (bool)GetValue(IsDockedInspectorModeProperty);
            set => SetValue(IsDockedInspectorModeProperty, value);
        }

        public object ParameterContent
        {
            get => GetValue(ParameterContentProperty);
            set => SetValue(ParameterContentProperty, value);
        }

        public object ToolContent
        {
            get => GetValue(ToolContentProperty);
            set => SetValue(ToolContentProperty, value);
        }

        public Visibility LearnButtonVisibility
        {
            get => (Visibility)GetValue(LearnButtonVisibilityProperty);
            set => SetValue(LearnButtonVisibilityProperty, value);
        }

        public string LearnButtonText
        {
            get => (string)GetValue(LearnButtonTextProperty);
            set => SetValue(LearnButtonTextProperty, value);
        }

        public int LearnTopicIndex
        {
            get => (int)GetValue(LearnTopicIndexProperty);
            set => SetValue(LearnTopicIndexProperty, value);
        }

        public HeaderedContentControl InputLayerGroup => gbInputLayer;
        public HeaderedContentControl OutputLayerGroup => gbOutputLayer;
        public HeaderedContentControl ParameterGroup => gbParameters;
        public TextBlock TitleText => txtTitle;
        public TextBlock AddPipelineText => txtAddPipelineText;
        public TextBlock RunPreviewText => txtRunPreviewText;
        public TextBlock StatusText => txtStatus;
        public TextBlock SummaryText => txtSummary;
        public Border SummaryHost => bdSummary;
        public Border StatusHost => bdStatus;
        public TextBlock TemplateStatusText => txtTemplateStatus;
        public Control TemplateStatusIcon => icoTemplateStatus;
        public TextBlock ParameterHeaderText => txtParameterHeader;
        public Border PresetHost => bdPresetHost;
        public Border PresetGap => bdPresetGap;
        public TextBlock PresetTitleText => txtPresetTitle;
        public TextBlock PresetDetailText => txtPresetDetail;
        public Button PresetMenuButton => btnPresetMenu;
        public Button PresetBasicButton => btnPresetBasic;
        public Button PresetFastButton => btnPresetFast;
        public Button PresetPreciseButton => btnPresetPrecise;
        public Border ResultReviewHost => bdResultReview;
        public TextBlock ResultReviewText => txtResultReview;
        public TextBlock ResultGuidanceText => txtResultGuidance;
        public ScrollViewer ResultReviewScrollViewer => svResultReviewChips;
        public Panel ResultReviewChips => resultReviewChips;
        public Border InputPreviewFrame => bdInputPreview;
        public VisionToolInlinePreviewSlot InputPreview => imgInputPreview;
        public Border OutputPreviewFrame => bdOutputPreview;
        public VisionToolInlinePreviewSlot OutputPreview => imgOutputPreview;
        public ComboBox InputLayerComboBox => cbInputLayer;
        public ComboBox OutputLayerComboBox => cbOutputLayer;
        public Button CreateOutputLayerButton => btnCreateOutputLayer;
        public Button RunPreviewButton => btnRunPreview;
        public Button AddPipelineButton => btnAddPipeline;
        public Border PropertyGridHost => propertyGridHost;

        private static void OnIsDockedInspectorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VisionToolSingleInputPropertyToolShell shell)
            {
                shell.ApplyDockedInspectorMode();
                shell.DockedInspectorModeChanged?.Invoke(shell, EventArgs.Empty);
            }
        }

        private void ApplyDockedInspectorMode()
        {
            layoutController?.Apply();
        }

        private void LearnTopicButton_Click(object sender, RoutedEventArgs e)
        {
            learnWindowController.Open(LearnTopicIndex);
        }
    }
}
