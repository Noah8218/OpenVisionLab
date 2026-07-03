using MahApps.Metro.IconPacks;
using System;
using System.Windows;
using System.Windows.Controls;
using WpgPropertyGrid = System.Windows.Controls.WpfPropertyGrid.PropertyGrid;

namespace OpenVisionLab
{
    public partial class VisionToolSingleInputPropertyToolShell : UserControl
    {
        private const double DockedPreviewFrameHeight = 60D;
        private const double DockedPreviewCardMinHeight = 92D;
        private const double FloatingPreviewCardPadding = 8D;
        private const double DockedPreviewCardPadding = 6D;
        private const double FloatingParameterGroupMinHeight = 400D;
        private const double DockedParameterGroupMinHeight = 0D;
        private const double FloatingPropertyGridMinHeight = 370D;
        private const double DockedPropertyGridMinHeight = 0D;
        private const double FloatingResultReviewMinHeight = 48D;
        private const double DockedResultReviewMinHeight = 34D;
        private const double DockedResultReviewMaxHeight = 64D;
        private const double DockedResultReviewChipsMaxHeight = 28D;
        private const double DockedSummaryMinHeight = 32D;
        private const double DockedStatusMinHeight = 28D;
        private const double ActionHeight = 36D;
        private const double ActionGap = 4D;

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

        public VisionToolSingleInputPropertyToolShell()
        {
            InitializeComponent();
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
            bool docked = IsDockedInspectorMode;

            // Docked tools are inspectors beside the main workspace: keep previews compact and
            // give the PropertyGrid the full width so parameter editing remains comfortable.
            MinWidth = docked ? 0D : 900D;
            MinHeight = docked ? 0D : 620D;
            shellRoot.Margin = docked ? new Thickness(8) : new Thickness(14);
            previewColumn.Width = docked ? new GridLength(1D, GridUnitType.Star) : new GridLength(390D);
            flowColumn.Width = docked ? new GridLength(8D) : new GridLength(16D);
            parameterColumn.Width = docked ? new GridLength(1D, GridUnitType.Star) : new GridLength(1D, GridUnitType.Star);
            flowRail.Visibility = docked ? Visibility.Collapsed : Visibility.Visible;
            Thickness previewPadding = new Thickness(docked ? DockedPreviewCardPadding : FloatingPreviewCardPadding);
            gbInputLayer.Padding = previewPadding;
            gbOutputLayer.Padding = previewPadding;

            previewInputRow.Height = docked ? GridLength.Auto : new GridLength(1D, GridUnitType.Star);
            previewSpacerRow.Height = docked ? new GridLength(2D) : new GridLength(10D);
            previewOutputRow.Height = docked ? new GridLength(1D, GridUnitType.Star) : new GridLength(1D, GridUnitType.Star);

            inputPreviewRow.Height = docked ? new GridLength(DockedPreviewFrameHeight) : new GridLength(1D, GridUnitType.Star);
            inputPreviewSpacerRow.Height = docked ? new GridLength(4D) : new GridLength(8D);
            outputPreviewRow.Height = docked ? new GridLength(DockedPreviewFrameHeight) : new GridLength(1D, GridUnitType.Star);
            outputPreviewSpacerRow.Height = docked ? new GridLength(4D) : new GridLength(8D);
            bdInputPreview.Visibility = Visibility.Visible;
            bdOutputPreview.Visibility = Visibility.Visible;

            Grid.SetRow(gbInputLayer, 0);
            Grid.SetColumn(gbInputLayer, 0);
            Grid.SetRowSpan(gbInputLayer, 1);
            Grid.SetColumnSpan(gbInputLayer, 1);

            Grid.SetRow(gbOutputLayer, docked ? 0 : 2);
            Grid.SetColumn(gbOutputLayer, docked ? 2 : 0);
            Grid.SetRowSpan(gbOutputLayer, 1);
            Grid.SetColumnSpan(gbOutputLayer, 1);

            Grid.SetRow(parameterPanel, docked ? 2 : 0);
            Grid.SetColumn(parameterPanel, docked ? 0 : 2);
            Grid.SetRowSpan(parameterPanel, docked ? 1 : 3);
            Grid.SetColumnSpan(parameterPanel, docked ? 3 : 1);

            titleRow.Height = docked ? new GridLength(0D) : GridLength.Auto;
            titleGapRow.Height = docked ? new GridLength(0D) : new GridLength(6D);
            summaryGapRow.Height = docked ? new GridLength(4D) : new GridLength(6D);
            actionGapRow.Height = docked ? new GridLength(2D) : new GridLength(6D);

            gbInputLayer.MinHeight = docked ? DockedPreviewCardMinHeight : 0D;
            gbOutputLayer.MinHeight = docked ? DockedPreviewCardMinHeight : 0D;
            gbParameters.MinHeight = docked ? DockedParameterGroupMinHeight : FloatingParameterGroupMinHeight;
            propertyGridHost.MinHeight = docked ? DockedPropertyGridMinHeight : FloatingPropertyGridMinHeight;
            ApplyPresetDensity(docked);
            ApplySummaryStatusDensity(docked);
            ApplyResultReviewDensity(docked);
            ApplyPropertyGridDensity(docked);
            ApplyToolContentDensity(docked);
            rowAddPipelineAction.Height = new GridLength(ActionHeight);
            rowRunPreviewGap.Height = new GridLength(docked ? 2D : ActionGap);
            rowRunPreviewAction.Height = new GridLength(ActionHeight);
        }

        private void ApplyPresetDensity(bool docked)
        {
            bdPresetHost.Padding = docked ? new Thickness(7, 4, 7, 4) : new Thickness(10, 8, 10, 8);
            txtPresetDetail.Visibility = docked ? Visibility.Collapsed : Visibility.Visible;
            txtPresetDetail.FontSize = docked ? 10D : 11D;
            btnPresetBasic.Height = docked ? 24D : 28D;
            btnPresetFast.Height = docked ? 24D : 28D;
            btnPresetPrecise.Height = docked ? 24D : 28D;
        }

        private void ApplySummaryStatusDensity(bool docked)
        {
            bdSummary.MinHeight = docked ? DockedSummaryMinHeight : 0D;
            bdSummary.Padding = docked ? new Thickness(9, 5, 9, 5) : new Thickness(12, 8, 12, 8);
            txtSummary.TextTrimming = TextTrimming.CharacterEllipsis;

            bdStatus.MinHeight = docked ? DockedStatusMinHeight : 0D;
            bdStatus.Margin = docked ? new Thickness(0, 4, 0, 0) : new Thickness(0, 6, 0, 0);
            bdStatus.Padding = docked ? new Thickness(8, 3, 8, 3) : new Thickness(8, 4, 8, 4);
            txtStatus.MinHeight = docked ? 14D : 18D;
            txtStatus.TextTrimming = TextTrimming.CharacterEllipsis;
        }

        private void ApplyResultReviewDensity(bool docked)
        {
            bdResultReview.MinHeight = docked ? DockedResultReviewMinHeight : FloatingResultReviewMinHeight;
            bdResultReview.MaxHeight = docked ? DockedResultReviewMaxHeight : double.PositiveInfinity;
            svResultReviewChips.MaxHeight = docked ? DockedResultReviewChipsMaxHeight : double.PositiveInfinity;
            svResultReviewChips.Margin = docked ? new Thickness(0, 3, 0, 0) : new Thickness(0, 4, 0, 0);
            svResultReviewChips.Visibility = docked ? Visibility.Collapsed : Visibility.Visible;
            svResultReviewChips.VerticalScrollBarVisibility = docked
                ? ScrollBarVisibility.Auto
                : ScrollBarVisibility.Disabled;
            txtResultGuidance.FontSize = docked ? 10D : 11D;
            txtResultGuidance.MaxHeight = double.PositiveInfinity;
        }

        private void ApplyPropertyGridDensity(bool compactDensity)
        {
            if (propertyGridHost.Child is WpgPropertyGrid grid)
            {
                grid.IsCompactDensity = compactDensity;
            }
        }

        private void ApplyToolContentDensity(bool compactDensity)
        {
            if (ToolContent is VisionToolVerificationGuideView guideView)
            {
                guideView.IsCompactMode = true;
            }
        }
    }
}
