using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class VisionToolDoubleInputCustomToolShell : UserControl
    {
        private const double DockedPreviewCardHeight = 132D;
        private const double FloatingActionHeight = 40D;
        private const double DockedActionHeight = 36D;
        private const double FloatingActionGap = 8D;
        private const double DockedActionGap = 4D;
        private const double DockedSummaryMinHeight = 32D;
        private const double DockedStatusMinHeight = 28D;

        public static readonly DependencyProperty TitleIconKindProperty =
            DependencyProperty.Register(
                nameof(TitleIconKind),
                typeof(PackIconMaterialKind),
                typeof(VisionToolDoubleInputCustomToolShell),
                new PropertyMetadata(PackIconMaterialKind.CalculatorVariant));

        public static readonly DependencyProperty ParameterContentProperty =
            DependencyProperty.Register(
                nameof(ParameterContent),
                typeof(object),
                typeof(VisionToolDoubleInputCustomToolShell),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsDockedInspectorModeProperty =
            DependencyProperty.Register(
                nameof(IsDockedInspectorMode),
                typeof(bool),
                typeof(VisionToolDoubleInputCustomToolShell),
                new PropertyMetadata(false, OnIsDockedInspectorModeChanged));

        public VisionToolDoubleInputCustomToolShell()
        {
            InitializeComponent();
            ApplyDockedInspectorMode();
        }

        public PackIconMaterialKind TitleIconKind
        {
            get => (PackIconMaterialKind)GetValue(TitleIconKindProperty);
            set => SetValue(TitleIconKindProperty, value);
        }

        public object ParameterContent
        {
            get => GetValue(ParameterContentProperty);
            set => SetValue(ParameterContentProperty, value);
        }

        public bool IsDockedInspectorMode
        {
            get => (bool)GetValue(IsDockedInspectorModeProperty);
            set => SetValue(IsDockedInspectorModeProperty, value);
        }

        public HeaderedContentControl InputAGroup => gbInputA;
        public HeaderedContentControl InputBGroup => gbInputB;
        public HeaderedContentControl OutputLayerGroup => gbOutputLayer;
        public TextBlock TitleText => txtTitle;
        public TextBlock AddPipelineText => txtAddPipelineText;
        public TextBlock RunPreviewText => txtRunPreviewText;
        public TextBlock RunOffsetText => txtRunOffsetText;
        public TextBlock StatusText => txtStatus;
        public TextBlock SummaryText => txtSummary;
        public Border SummaryHost => bdSummary;
        public Border StatusHost => bdStatus;
        public Border InputAPreviewFrame => bdInputAPreview;
        public VisionToolInlinePreviewSlot InputAPreview => imgInputA;
        public Border InputBPreviewFrame => bdInputBPreview;
        public VisionToolInlinePreviewSlot InputBPreview => imgInputB;
        public Border OutputPreviewFrame => bdOutputPreview;
        public VisionToolInlinePreviewSlot OutputPreview => imgOutputPreview;
        public ComboBox InputAComboBox => cbInputA;
        public ComboBox InputBComboBox => cbInputB;
        public ComboBox OutputLayerComboBox => cbOutputLayer;
        public Button LoadInputAImageButton => btnLoadInputAImage;
        public Button LoadInputBImageButton => btnLoadInputBImage;
        public Button CreateOutputLayerButton => btnCreateOutputLayer;
        public Button RunPreviewButton => btnRunPreview;
        public Button RunOffsetButton => btnRunOffset;
        public Button AddPipelineButton => btnAddPipeline;

        public void SetInputBPreviewVisible(bool visible)
        {
            gbInputB.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            gbInputB.IsEnabled = visible;
            rowInputB.Height = visible
                ? IsDockedInspectorMode ? GridLength.Auto : new GridLength(1, GridUnitType.Star)
                : new GridLength(0);
            rowInputBGap.Height = visible ? new GridLength(IsDockedInspectorMode ? 6D : 8D) : new GridLength(0);
            ApplyPreviewCardDocking(gbInputB, IsDockedInspectorMode, visible);
        }

        public void SetOffsetActionsVisible(bool useOffsetMode)
        {
            // Keep Arithmetic's mode-specific action layout in the shared shell so every two-input tool gets the same button behavior.
            btnRunPreview.Visibility = useOffsetMode ? Visibility.Collapsed : Visibility.Visible;
            btnRunOffset.Visibility = useOffsetMode ? Visibility.Visible : Visibility.Collapsed;
            ApplyActionRows(useOffsetMode);
        }

        private static void OnIsDockedInspectorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VisionToolDoubleInputCustomToolShell shell)
            {
                shell.ApplyDockedInspectorMode();
            }
        }

        private void ApplyDockedInspectorMode()
        {
            bool docked = IsDockedInspectorMode;
            bool inputBVisible = gbInputB.Visibility == Visibility.Visible;

            MinWidth = docked ? 0D : 920D;
            MinHeight = docked ? 0D : 620D;
            shellRoot.Margin = docked ? new Thickness(8) : new Thickness(14);
            previewColumn.Width = docked ? new GridLength(180D) : new GridLength(390D);
            flowColumn.Width = docked ? new GridLength(0D) : new GridLength(16D);
            parameterColumn.Width = new GridLength(1D, GridUnitType.Star);
            flowRail.Visibility = docked ? Visibility.Collapsed : Visibility.Visible;

            rowInputA.Height = docked ? GridLength.Auto : new GridLength(1D, GridUnitType.Star);
            rowInputBGap.Height = inputBVisible ? new GridLength(docked ? 6D : 8D) : new GridLength(0D);
            rowInputB.Height = inputBVisible
                ? docked ? GridLength.Auto : new GridLength(1D, GridUnitType.Star)
                : new GridLength(0D);
            rowOutputGap.Height = docked ? new GridLength(6D) : new GridLength(8D);
            rowOutput.Height = docked ? new GridLength(1D, GridUnitType.Star) : new GridLength(1D, GridUnitType.Star);

            titleRow.Height = docked ? new GridLength(0D) : GridLength.Auto;
            titleGapRow.Height = docked ? new GridLength(0D) : new GridLength(14D);
            summaryGapRow.Height = docked ? new GridLength(8D) : new GridLength(12D);
            actionGapRow.Height = docked ? new GridLength(8D) : new GridLength(14D);

            ApplyPreviewCardDocking(gbInputA, docked, visible: true);
            ApplyPreviewCardDocking(gbInputB, docked, inputBVisible);
            ApplyPreviewCardDocking(gbOutputLayer, docked, visible: true);
            ApplySummaryStatusDensity(docked);
            ApplyActionRows(IsOffsetActionActive());
        }

        private void ApplySummaryStatusDensity(bool docked)
        {
            bdSummary.MinHeight = docked ? DockedSummaryMinHeight : 0D;
            bdSummary.Padding = docked ? new Thickness(9, 5, 9, 5) : new Thickness(12, 8, 12, 8);
            txtSummary.TextTrimming = TextTrimming.CharacterEllipsis;

            bdStatus.MinHeight = docked ? DockedStatusMinHeight : 0D;
            bdStatus.Margin = docked ? new Thickness(0, 4, 0, 0) : new Thickness(0, 8, 0, 0);
            bdStatus.Padding = docked ? new Thickness(8, 3, 8, 3) : new Thickness(8, 4, 8, 4);
            txtStatus.MinHeight = docked ? 14D : 18D;
            txtStatus.TextTrimming = TextTrimming.CharacterEllipsis;
        }

        private static void ApplyPreviewCardDocking(HeaderedContentControl group, bool docked, bool visible)
        {
            if (group == null)
            {
                return;
            }

            group.VerticalAlignment = docked ? VerticalAlignment.Top : VerticalAlignment.Stretch;
            group.Height = docked && visible ? DockedPreviewCardHeight : double.NaN;
            group.MinHeight = docked && visible ? DockedPreviewCardHeight : 0D;
        }

        private bool IsOffsetActionActive()
        {
            return btnRunOffset.Visibility == Visibility.Visible
                && rowRunOffsetAction.Height.Value > 0D;
        }

        private void ApplyActionRows(bool useOffsetMode)
        {
            double actionHeight = IsDockedInspectorMode ? DockedActionHeight : FloatingActionHeight;
            double actionGap = IsDockedInspectorMode ? DockedActionGap : FloatingActionGap;

            rowAddPipelineAction.Height = new GridLength(actionHeight);
            rowRunPreviewGap.Height = useOffsetMode ? new GridLength(0) : new GridLength(actionGap);
            rowRunPreviewAction.Height = useOffsetMode ? new GridLength(0) : new GridLength(actionHeight);
            rowRunOffsetGap.Height = useOffsetMode ? new GridLength(actionGap) : new GridLength(0);
            rowRunOffsetAction.Height = useOffsetMode ? new GridLength(actionHeight) : new GridLength(0);
        }
    }
}
