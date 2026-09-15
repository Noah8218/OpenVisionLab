using System;
using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class VisionToolDoubleInputCustomToolShell : UserControl
    {
        private readonly DockedInspectorLayoutController layoutController;
        private readonly VisionToolLearnWindowController learnWindowController;

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

        public static readonly DependencyProperty LearnButtonVisibilityProperty =
            DependencyProperty.Register(
                nameof(LearnButtonVisibility),
                typeof(Visibility),
                typeof(VisionToolDoubleInputCustomToolShell),
                new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty LearnButtonTextProperty =
            DependencyProperty.Register(
                nameof(LearnButtonText),
                typeof(string),
                typeof(VisionToolDoubleInputCustomToolShell),
                new PropertyMetadata("Learn"));

        public static readonly DependencyProperty LearnTopicIndexProperty =
            DependencyProperty.Register(
                nameof(LearnTopicIndex),
                typeof(int),
                typeof(VisionToolDoubleInputCustomToolShell),
                new PropertyMetadata(0));

        public VisionToolDoubleInputCustomToolShell()
        {
            InitializeComponent();
            layoutController = new DockedInspectorLayoutController(this);
            learnWindowController = new VisionToolLearnWindowController(() => Window.GetWindow(this));
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
            layoutController?.SetInputBPreviewVisible(visible);
        }

        public void SetOffsetActionsVisible(bool useOffsetMode)
        {
            layoutController?.SetOffsetActionsVisible(useOffsetMode);
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
            layoutController?.Apply();
        }

        private void LearnTopicButton_Click(object sender, RoutedEventArgs e)
        {
            learnWindowController.Open(LearnTopicIndex);
        }

        #region Docked Inspector Layout

        private sealed class DockedInspectorLayoutController
        {
            private const double DockedPreviewCardHeight = 132D;
            private const double FloatingActionHeight = 40D;
            private const double DockedActionHeight = 36D;
            private const double FloatingActionGap = 8D;
            private const double DockedActionGap = 4D;
            private const double DockedSummaryMinHeight = 32D;
            private const double DockedStatusMinHeight = 28D;

            private readonly VisionToolDoubleInputCustomToolShell shell;

            public DockedInspectorLayoutController(VisionToolDoubleInputCustomToolShell shell)
            {
                this.shell = shell ?? throw new ArgumentNullException(nameof(shell));
            }

            public void Apply()
            {
                bool docked = shell.IsDockedInspectorMode;
                bool inputBVisible = shell.gbInputB.Visibility == Visibility.Visible;

                shell.MinWidth = docked ? 0D : 920D;
                shell.MinHeight = docked ? 0D : 620D;
                shell.shellRoot.Margin = docked ? new Thickness(8) : new Thickness(14);
                shell.previewColumn.Width = docked ? new GridLength(180D) : new GridLength(390D);
                shell.flowColumn.Width = docked ? new GridLength(0D) : new GridLength(16D);
                shell.parameterColumn.Width = new GridLength(1D, GridUnitType.Star);
                shell.flowRail.Visibility = docked ? Visibility.Collapsed : Visibility.Visible;

                shell.rowInputA.Height = docked ? GridLength.Auto : new GridLength(1D, GridUnitType.Star);
                shell.rowInputBGap.Height = inputBVisible ? new GridLength(docked ? 6D : 8D) : new GridLength(0D);
                shell.rowInputB.Height = inputBVisible
                    ? docked ? GridLength.Auto : new GridLength(1D, GridUnitType.Star)
                    : new GridLength(0D);
                shell.rowOutputGap.Height = docked ? new GridLength(6D) : new GridLength(8D);
                shell.rowOutput.Height = new GridLength(1D, GridUnitType.Star);

                shell.titleRow.Height = docked ? new GridLength(0D) : GridLength.Auto;
                shell.titleGapRow.Height = docked ? new GridLength(0D) : new GridLength(14D);
                shell.summaryGapRow.Height = docked ? new GridLength(8D) : new GridLength(12D);
                shell.actionGapRow.Height = docked ? new GridLength(8D) : new GridLength(14D);

                ApplyPreviewCardDocking(shell.gbInputA, docked, visible: true);
                ApplyPreviewCardDocking(shell.gbInputB, docked, inputBVisible);
                ApplyPreviewCardDocking(shell.gbOutputLayer, docked, visible: true);
                ApplySummaryStatusDensity(docked);
                ApplyActionRows(IsOffsetActionActive());
            }

            public void SetInputBPreviewVisible(bool visible)
            {
                shell.gbInputB.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
                shell.gbInputB.IsEnabled = visible;
                shell.rowInputB.Height = visible
                    ? shell.IsDockedInspectorMode ? GridLength.Auto : new GridLength(1, GridUnitType.Star)
                    : new GridLength(0);
                shell.rowInputBGap.Height = visible ? new GridLength(shell.IsDockedInspectorMode ? 6D : 8D) : new GridLength(0);
                ApplyPreviewCardDocking(shell.gbInputB, shell.IsDockedInspectorMode, visible);
            }

            public void SetOffsetActionsVisible(bool useOffsetMode)
            {
                shell.btnRunPreview.Visibility = useOffsetMode ? Visibility.Collapsed : Visibility.Visible;
                shell.btnRunOffset.Visibility = useOffsetMode ? Visibility.Visible : Visibility.Collapsed;
                ApplyActionRows(useOffsetMode);
            }

            private void ApplySummaryStatusDensity(bool docked)
            {
                shell.bdSummary.MinHeight = docked ? DockedSummaryMinHeight : 0D;
                shell.bdSummary.Padding = docked ? new Thickness(9, 5, 9, 5) : new Thickness(12, 8, 12, 8);
                shell.txtSummary.TextTrimming = TextTrimming.CharacterEllipsis;

                shell.bdStatus.MinHeight = docked ? DockedStatusMinHeight : 0D;
                shell.bdStatus.Margin = docked ? new Thickness(0, 4, 0, 0) : new Thickness(0, 8, 0, 0);
                shell.bdStatus.Padding = docked ? new Thickness(8, 3, 8, 3) : new Thickness(8, 4, 8, 4);
                shell.txtStatus.MinHeight = docked ? 14D : 18D;
                shell.txtStatus.TextTrimming = TextTrimming.CharacterEllipsis;
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
                return shell.btnRunOffset.Visibility == Visibility.Visible
                    && shell.rowRunOffsetAction.Height.Value > 0D;
            }

            private void ApplyActionRows(bool useOffsetMode)
            {
                double actionHeight = shell.IsDockedInspectorMode ? DockedActionHeight : FloatingActionHeight;
                double actionGap = shell.IsDockedInspectorMode ? DockedActionGap : FloatingActionGap;

                shell.rowAddPipelineAction.Height = new GridLength(actionHeight);
                shell.rowRunPreviewGap.Height = useOffsetMode ? new GridLength(0) : new GridLength(actionGap);
                shell.rowRunPreviewAction.Height = useOffsetMode ? new GridLength(0) : new GridLength(actionHeight);
                shell.rowRunOffsetGap.Height = useOffsetMode ? new GridLength(actionGap) : new GridLength(0);
                shell.rowRunOffsetAction.Height = useOffsetMode ? new GridLength(actionHeight) : new GridLength(0);
            }
        }

        #endregion
    }
}
