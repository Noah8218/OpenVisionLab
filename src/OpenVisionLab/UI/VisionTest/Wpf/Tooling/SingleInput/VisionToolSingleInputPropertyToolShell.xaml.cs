using MahApps.Metro.IconPacks;
using WpgPropertyGrid = System.Windows.Controls.WpfPropertyGrid.PropertyGrid;
using System;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class VisionToolSingleInputPropertyToolShell : UserControl
    {
        private readonly DockedInspectorLayoutController layoutController;
        private readonly VisionToolLearnWindowController learnWindowController;
        private readonly VisionToolParameterGuideView parameterGuideView;
        private readonly VisionToolParameterGuideSidecarController parameterGuideSidecarController;

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

        public static readonly DependencyProperty ParameterGuideVisibilityProperty =
            DependencyProperty.Register(
                nameof(ParameterGuideVisibility),
                typeof(Visibility),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata(Visibility.Collapsed, OnParameterGuideVisibilityChanged));

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

        public static readonly DependencyProperty LearnButtonAutomationIdProperty =
            DependencyProperty.Register(
                nameof(LearnButtonAutomationId),
                typeof(string),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata("VisionToolHeaderLearnButton"));

        public static readonly DependencyProperty LearnTopicIndexProperty =
            DependencyProperty.Register(
                nameof(LearnTopicIndex),
                typeof(int),
                typeof(VisionToolSingleInputPropertyToolShell),
                new PropertyMetadata(0));

        public VisionToolSingleInputPropertyToolShell()
        {
            InitializeComponent();
            parameterGuideView = new VisionToolParameterGuideView();
            parameterGuideSidecarController =
                new VisionToolParameterGuideSidecarController(this, parameterGuideView, () => IsDockedInspectorMode);
            layoutController = new DockedInspectorLayoutController(this);
            learnWindowController = new VisionToolLearnWindowController(() => Window.GetWindow(this));
            ApplyDockedInspectorMode();
            ApplyParameterGuideLocalization();
        }

        public event EventHandler DockedInspectorModeChanged;

        public event EventHandler LearnTopicRequested;

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

        public Visibility ParameterGuideVisibility
        {
            get => (Visibility)GetValue(ParameterGuideVisibilityProperty);
            set => SetValue(ParameterGuideVisibilityProperty, value);
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

        public string LearnButtonAutomationId
        {
            get => (string)GetValue(LearnButtonAutomationIdProperty);
            set => SetValue(LearnButtonAutomationIdProperty, value);
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
        public TextBlock NImageVerificationText => txtNImageVerificationText;
        public TextBlock RunPreviewText => txtRunPreviewText;
        public TextBlock StatusText => txtStatus;
        public TextBlock SummaryText => txtSummary;
        public ContentControl WorkbenchAcceptanceHost => workbenchAcceptanceHost;
        public ContentControl WorkbenchTemplateHost => workbenchTemplateHost;
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
        public Button NImageVerificationButton => btnNImageVerification;
        internal FrameworkElement WorkbenchStepActions => workbenchStepActions;
        internal Button WorkbenchDefaultsButton => btnWorkbenchDefaults;
        internal FrameworkElement WorkbenchRunShortcut => txtWorkbenchRunShortcut;
        public Border PropertyGridHost => propertyGridHost;
        public VisionToolParameterGuideView ParameterGuide => parameterGuideView;
        public void ToggleParameterGuide() => parameterGuideSidecarController.Toggle();

        internal void ApplyParameterGuideLocalization()
        {
            VisionToolChromePresenter.ApplyTooltip(btnParameterGuide, OpenVisionLanguageService.T("VisionTool.ParameterGuide.Header"));
            parameterGuideSidecarController.RefreshTitle();
        }

        private static void OnIsDockedInspectorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VisionToolSingleInputPropertyToolShell shell)
            {
                shell.ApplyDockedInspectorMode();
                shell.DockedInspectorModeChanged?.Invoke(shell, EventArgs.Empty);
            }
        }

        private static void OnParameterGuideVisibilityChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is VisionToolSingleInputPropertyToolShell shell)
            {
                shell.parameterGuideSidecarController?.SetAvailable(
                    (Visibility)e.NewValue == Visibility.Visible);
            }
        }

        private void ApplyDockedInspectorMode()
        {
            layoutController?.Apply();
            parameterGuideSidecarController?.OnLayoutModeChanged();
        }

        private void LearnTopicButton_Click(object sender, RoutedEventArgs e)
        {
            if (LearnTopicRequested != null)
            {
                LearnTopicRequested(this, EventArgs.Empty);
                return;
            }

            learnWindowController.Open(LearnTopicIndex);
        }

        private void ParameterGuideButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleParameterGuide();
        }

        private void WorkbenchActions_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) => layoutController?.Apply();

        private void WorkbenchBody_SizeChanged(object sender, SizeChangedEventArgs e) => layoutController?.UpdateBodyBounds();

        private void AddPipelineButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu is ContextMenu menu)
            {
                menu.PlacementTarget = button;
                menu.IsOpen = true;
            }
        }

        #region Docked Inspector Layout

        private sealed class DockedInspectorLayoutController
        {
            private const double DockedPreviewFrameHeight = 48D;
            private const double DockedPreviewCardMinHeight = 76D;
            private const double FloatingPreviewCardPadding = 8D;
            private const double DockedPreviewCardPadding = 4D;
            private const double FloatingParameterGroupMinHeight = 400D;
            private const double DockedParameterGroupMinHeight = 0D;
            private const double FloatingPropertyGridMinHeight = 370D;
            private const double DockedPropertyGridMinHeight = 0D;
            private const double FloatingResultReviewMinHeight = 48D;
            private const double FloatingResultReviewMaxHeight = 96D;
            private const double DockedResultReviewMinHeight = 34D;
            private const double DockedResultReviewMaxHeight = 64D;
            private const double DockedResultReviewChipsMaxHeight = 28D;
            private const double DockedSummaryMinHeight = 32D;
            private const double DockedStatusMinHeight = 28D;
            private const double ActionHeight = 36D;
            private const double ActionGap = 4D;

            private readonly VisionToolSingleInputPropertyToolShell shell;

            public DockedInspectorLayoutController(VisionToolSingleInputPropertyToolShell shell)
            {
                this.shell = shell ?? throw new ArgumentNullException(nameof(shell));
            }

            public void Apply()
            {
                bool docked = shell.IsDockedInspectorMode;
                bool workbench = shell.btnWorkbenchDefaults.Visibility == Visibility.Visible || shell.workbenchStepActions.Visibility == Visibility.Visible;
                bool inlineRoute = docked && workbench;
                bool naturalParameters = workbench && shell.PropertyGridHostVisibility != Visibility.Visible;
                shell.parameterBodyScrollViewer.VerticalScrollBarVisibility = workbench ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
                shell.parameterGroupRow.Height = naturalParameters ? GridLength.Auto : new GridLength(1D, GridUnitType.Star);

                shell.MinWidth = docked ? 0D : 900D;
                shell.MinHeight = docked ? 0D : 620D;
                shell.shellRoot.Margin = docked ? new Thickness(8) : new Thickness(14);
                shell.previewColumn.Width = docked ? new GridLength(1D, GridUnitType.Star) : new GridLength(390D);
                shell.flowColumn.Width = new GridLength(inlineRoute ? 24D : docked ? 8D : 16D);
                shell.parameterColumn.Width = new GridLength(1D, GridUnitType.Star);
                shell.propertyGridHost.MinWidth = workbench && docked ? 0D : 480D;
                shell.flowRail.Visibility = docked ? Visibility.Collapsed : Visibility.Visible;
                Thickness previewPadding = new Thickness(docked ? DockedPreviewCardPadding : FloatingPreviewCardPadding);
                shell.gbInputLayer.Padding = previewPadding;
                shell.gbOutputLayer.Padding = previewPadding;
                shell.bdTemplateStatus.Padding = workbench && shell.workbenchTemplateHost.Content != null ? new Thickness(10, 4, 10, 4)
                    : docked ? new Thickness(12, 5, 12, 5) : new Thickness(12, 8, 12, 8);

                shell.previewInputRow.Height = docked ? GridLength.Auto : new GridLength(1D, GridUnitType.Star);
                shell.previewSpacerRow.Height = docked ? new GridLength(2D) : new GridLength(10D);
                shell.previewOutputRow.Height = new GridLength(1D, GridUnitType.Star);

                shell.inputPreviewRow.Height = inlineRoute ? new GridLength(0D) : docked ? new GridLength(DockedPreviewFrameHeight) : new GridLength(1D, GridUnitType.Star);
                shell.inputPreviewSpacerRow.Height = new GridLength(inlineRoute ? 0D : docked ? 4D : 8D);
                shell.inputComboRow.Height = docked ? new GridLength(28D) : new GridLength(32D);
                shell.outputPreviewRow.Height = inlineRoute ? new GridLength(0D) : docked ? new GridLength(DockedPreviewFrameHeight) : new GridLength(1D, GridUnitType.Star);
                shell.outputPreviewSpacerRow.Height = new GridLength(inlineRoute ? 0D : docked ? 4D : 8D);
                shell.outputComboRow.Height = docked ? new GridLength(28D) : new GridLength(32D);
                shell.bdInputPreview.Visibility = shell.bdOutputPreview.Visibility = inlineRoute ? Visibility.Collapsed : Visibility.Visible;
                shell.workbenchRouteArrow.Visibility = inlineRoute ? Visibility.Visible : Visibility.Collapsed;
                shell.gbInputLayer.SetResourceReference(StyleProperty, inlineRoute ? "VisionTool.WorkbenchRouteStyle" : typeof(GroupBox));
                shell.gbOutputLayer.SetResourceReference(StyleProperty, inlineRoute ? "VisionTool.WorkbenchRouteStyle" : typeof(GroupBox));
                shell.gbInputLayer.Padding = shell.gbOutputLayer.Padding = inlineRoute ? new Thickness(0D) : previewPadding;
                shell.cbInputLayer.MinWidth = shell.cbOutputLayer.MinWidth = inlineRoute ? 0D : 160D;
                shell.cbInputLayer.SetResourceReference(StyleProperty, inlineRoute ? "VisionTool.WorkbenchRouteComboBoxStyle" : "VisionTool.LayerComboBoxStyle");
                shell.cbOutputLayer.SetResourceReference(StyleProperty, inlineRoute ? "VisionTool.WorkbenchRouteComboBoxStyle" : "VisionTool.LayerComboBoxStyle");
                shell.btnCreateOutputLayer.Visibility = inlineRoute ? Visibility.Collapsed : Visibility.Visible;
                shell.outputActionGapColumn.Width = new GridLength(inlineRoute ? 0D : 8D);
                shell.outputActionColumn.Width = new GridLength(inlineRoute ? 0D : 34D);

                Grid.SetRow(shell.gbInputLayer, 0);
                Grid.SetColumn(shell.gbInputLayer, 0);
                Grid.SetRowSpan(shell.gbInputLayer, 1);
                Grid.SetColumnSpan(shell.gbInputLayer, 1);

                Grid.SetRow(shell.gbOutputLayer, docked ? 0 : 2);
                Grid.SetColumn(shell.gbOutputLayer, docked ? 2 : 0);
                Grid.SetRowSpan(shell.gbOutputLayer, 1);
                Grid.SetColumnSpan(shell.gbOutputLayer, 1);

                Grid.SetRow(shell.parameterPanel, docked ? 2 : 0);
                Grid.SetColumn(shell.parameterPanel, docked ? 0 : 2);
                Grid.SetRowSpan(shell.parameterPanel, docked ? 1 : 3);
                Grid.SetColumnSpan(shell.parameterPanel, docked ? 3 : 1);

                shell.titleRow.Height = docked ? new GridLength(0D) : GridLength.Auto;
                shell.titleGapRow.Height = docked ? new GridLength(0D) : new GridLength(6D);
                shell.summaryGapRow.Height = workbench && shell.workbenchTemplateHost.Content != null ? new GridLength(0D)
                    : docked ? new GridLength(4D) : new GridLength(6D);
                shell.actionGapRow.Height = docked ? new GridLength(2D) : new GridLength(6D);

                shell.gbInputLayer.MinHeight = shell.gbOutputLayer.MinHeight = inlineRoute ? 28D : docked ? DockedPreviewCardMinHeight : 0D;
                shell.gbParameters.MinHeight = docked || workbench ? DockedParameterGroupMinHeight : FloatingParameterGroupMinHeight;
                double floatingWorkbenchPropertyGridMinHeight = 380D + shell.propertyGridHost.Padding.Top + shell.propertyGridHost.Padding.Bottom
                    + shell.propertyGridHost.BorderThickness.Top + shell.propertyGridHost.BorderThickness.Bottom;
                shell.propertyGridHost.MinHeight = workbench ? (docked ? 160D : floatingWorkbenchPropertyGridMinHeight)
                    : docked ? DockedPropertyGridMinHeight : FloatingPropertyGridMinHeight;
                shell.propertyGridScrollViewer.MinHeight = shell.propertyGridHost.MinHeight;
                shell.parameterGuideView.SetCompactMode(docked);
                ApplyPresetDensity(docked);
                ApplySummaryStatusDensity(docked);
                ApplyResultReviewDensity(docked);
                ApplyPropertyGridDensity(docked);
                ApplyToolContentDensity();
                shell.rowAddPipelineAction.Height = new GridLength(ActionHeight);
                shell.rowRunPreviewGap.Height = new GridLength(docked ? 0D : ActionGap);
                shell.rowRunPreviewAction.Height = new GridLength(docked ? 0D : ActionHeight);
                shell.runPreviewActionGapColumn.Width = docked ? new GridLength(ActionGap) : new GridLength(0D);
                shell.runPreviewActionColumn.Width = docked
                    ? new GridLength(1D, GridUnitType.Star)
                    : new GridLength(0D);
                Grid.SetRow(shell.btnRunPreview, docked ? 0 : 2);
                Grid.SetColumn(shell.btnRunPreview, docked ? 4 : 0);
                Grid.SetColumnSpan(shell.btnRunPreview, docked ? 1 : 5);
                bool testRow = shell.btnWorkbenchDefaults.Visibility == Visibility.Visible;
                Grid.SetColumn(shell.btnAddPipeline, testRow ? 4 : 0);
                Grid.SetColumnSpan(shell.btnAddPipeline, 1);
                shell.actionPanel.ColumnDefinitions[5].Width = new GridLength(testRow ? 6D : 0D);
                shell.actionPanel.ColumnDefinitions[6].Width = testRow ? GridLength.Auto : new GridLength(0D);
                shell.btnRunPreview.Height = testRow ? 34D : double.NaN;
                if (testRow)
                {
                    shell.actionPanel.ColumnDefinitions[0].Width = GridLength.Auto;
                    shell.actionPanel.ColumnDefinitions[1].Width = new GridLength(6D);
                    shell.actionPanel.ColumnDefinitions[2].Width = GridLength.Auto;
                    shell.actionPanel.ColumnDefinitions[3].Width = new GridLength(1D, GridUnitType.Star);
                    shell.actionPanel.ColumnDefinitions[4].Width = GridLength.Auto;
                    shell.rowAddPipelineAction.Height = new GridLength(34D);
                    shell.rowRunPreviewGap.Height = shell.rowRunPreviewAction.Height = new GridLength(0D);
                    Grid.SetRow(shell.btnRunPreview, 0);
                    Grid.SetColumn(shell.btnRunPreview, 6);
                    Grid.SetColumnSpan(shell.btnRunPreview, 1);
                }
                else
                {
                    shell.addPipelineActionColumn.Width = new GridLength(1D, GridUnitType.Star);
                    shell.addPipelineActionGapColumn.Width = new GridLength(4D);
                    shell.nImageActionColumn.Width = new GridLength(1D, GridUnitType.Star);
                }
                UpdateBodyBounds();
            }

            public void UpdateBodyBounds()
            {
                bool workbench = shell.btnWorkbenchDefaults.Visibility == Visibility.Visible || shell.workbenchStepActions.Visibility == Visibility.Visible;
                if (!workbench || shell.PropertyGridHostVisibility != Visibility.Visible)
                {
                    shell.parameterBodyGrid.ClearValue(HeightProperty);
                    shell.parameterBodyGrid.ClearValue(MinHeightProperty);
                    return;
                }
                if (shell.parameterBodyScrollViewer.ActualHeight <= 0D) { return; }
                double horizontalScrollBarHeight = shell.propertyGridScrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible ? SystemParameters.HorizontalScrollBarHeight : 0D;
                shell.propertyGridScrollViewer.MinHeight = shell.propertyGridHost.MinHeight + horizontalScrollBarHeight;
                double chromeHeight = Math.Max(0D, shell.gbParameters.ActualHeight - shell.propertyGridScrollViewer.ActualHeight) + horizontalScrollBarHeight;
                for (int row = 1; row < shell.parameterBodyGrid.RowDefinitions.Count; row++)
                    chromeHeight += shell.parameterBodyGrid.RowDefinitions[row].ActualHeight;
                shell.parameterBodyGrid.MinHeight = shell.propertyGridHost.MinHeight + chromeHeight;
                shell.parameterBodyGrid.Height = shell.parameterBodyScrollViewer.ActualHeight;
            }

            private void ApplyPresetDensity(bool docked)
            {
                bool workbench = shell.WorkbenchStepActions.DataContext is OpenVisionWorkbenchToolViewModel;
                shell.bdPresetHost.Padding = workbench ? new Thickness(0, 4, 0, 4) : docked ? new Thickness(7, 4, 7, 4) : new Thickness(10, 8, 10, 8);
                shell.txtPresetDetail.Visibility = workbench || docked ? Visibility.Collapsed : Visibility.Visible;
                shell.txtPresetDetail.FontSize = docked ? 10D : 11D;
                shell.btnPresetBasic.Height = workbench ? 32D : docked ? 24D : 28D;
                shell.btnPresetFast.Height = workbench ? 32D : docked ? 24D : 28D;
                shell.btnPresetPrecise.Height = workbench ? 32D : docked ? 24D : 28D;
            }

            private void ApplySummaryStatusDensity(bool docked)
            {
                shell.bdSummary.MinHeight = docked ? DockedSummaryMinHeight : 0D;
                shell.bdSummary.Padding = docked ? new Thickness(9, 5, 9, 5) : new Thickness(12, 8, 12, 8);
                shell.txtSummary.TextTrimming = TextTrimming.CharacterEllipsis;

                shell.bdStatus.MinHeight = docked ? DockedStatusMinHeight : 0D;
                shell.bdStatus.Margin = docked ? new Thickness(0, 4, 0, 0) : new Thickness(0, 6, 0, 0);
                shell.bdStatus.Padding = docked ? new Thickness(8, 3, 8, 3) : new Thickness(8, 4, 8, 4);
                shell.txtStatus.MinHeight = docked ? 14D : 18D;
                shell.txtStatus.TextTrimming = TextTrimming.None;
            }

            private void ApplyResultReviewDensity(bool docked)
            {
                shell.bdResultReview.MinHeight = docked ? DockedResultReviewMinHeight : FloatingResultReviewMinHeight;
                shell.bdResultReview.MaxHeight = docked ? DockedResultReviewMaxHeight : FloatingResultReviewMaxHeight;
                shell.svResultReviewChips.MaxHeight = docked ? DockedResultReviewChipsMaxHeight : double.PositiveInfinity;
                shell.svResultReviewChips.Margin = docked ? new Thickness(0, 3, 0, 0) : new Thickness(0, 4, 0, 0);
                shell.svResultReviewChips.Visibility = docked || shell.workbenchResultHost.Content != null ? Visibility.Collapsed : Visibility.Visible;
                shell.svResultReviewChips.VerticalScrollBarVisibility = docked
                    ? ScrollBarVisibility.Auto
                    : ScrollBarVisibility.Disabled;
                shell.txtResultGuidance.FontSize = docked ? 10D : 11D;
                shell.txtResultGuidance.MaxHeight = double.PositiveInfinity;
            }

            private void ApplyPropertyGridDensity(bool compactDensity)
            {
                if (shell.propertyGridHost.Child is WpgPropertyGrid grid)
                {
                    grid.IsCompactDensity = compactDensity;
                }
            }

            private void ApplyToolContentDensity()
            {
                if (shell.ToolContent is VisionToolVerificationGuideView guideView)
                {
                    guideView.IsCompactMode = true;
                }
            }
        }

        #endregion
    }
}
