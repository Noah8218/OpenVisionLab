using System;
using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OpenVisionLab
{
    public partial class VisionToolDoubleInputCustomToolShell : UserControl
    {
        private readonly DockedInspectorLayoutController layoutController;
        private readonly VisionToolLearnWindowController learnWindowController;
        private readonly VisionToolParameterGuideView parameterGuideView;
        private readonly VisionToolParameterGuideSidecarController parameterGuideSidecarController;

        public static readonly DependencyProperty ParameterGuideVisibilityProperty =
            DependencyProperty.Register(nameof(ParameterGuideVisibility), typeof(Visibility), typeof(VisionToolDoubleInputCustomToolShell),
                new PropertyMetadata(Visibility.Collapsed, OnParameterGuideVisibilityChanged));

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
            parameterGuideView = new VisionToolParameterGuideView();
            parameterGuideSidecarController = new VisionToolParameterGuideSidecarController(this, parameterGuideView, () => IsDockedInspectorMode);
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

        public Visibility ParameterGuideVisibility
        {
            get => (Visibility)GetValue(ParameterGuideVisibilityProperty);
            set => SetValue(ParameterGuideVisibilityProperty, value);
        }

        public VisionToolParameterGuideView ParameterGuide => parameterGuideView;
        public void ToggleParameterGuide() => parameterGuideSidecarController.Toggle();

        internal void ApplyParameterGuideLocalization()
        {
            VisionToolChromePresenter.ApplyTooltip(btnParameterGuide, OpenVisionLanguageService.T("VisionTool.ParameterGuide.Header"));
            parameterGuideSidecarController.RefreshTitle();
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
        internal FrameworkElement WorkbenchStepActions => workbenchStepActions;
        internal Button WorkbenchDefaultsButton => btnWorkbenchDefaults;
        internal FrameworkElement WorkbenchRunShortcut => txtWorkbenchRunShortcut;
        internal FrameworkElement WorkbenchRunOffsetShortcut => txtWorkbenchRunOffsetShortcut;

        public void SetInputBPreviewVisible(bool visible)
        {
            layoutController?.SetInputBPreviewVisible(visible);
        }

        internal void SetWorkbenchInputBRouteVisible(bool visible) => layoutController.SetWorkbenchInputBRouteVisible(visible);

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
            parameterGuideSidecarController?.OnLayoutModeChanged();
        }

        private static void OnParameterGuideVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VisionToolDoubleInputCustomToolShell shell)
                shell.parameterGuideSidecarController?.SetAvailable((Visibility)e.NewValue == Visibility.Visible);
        }

        private void ParameterGuideButton_Click(object sender, RoutedEventArgs e) => ToggleParameterGuide();

        private void WorkbenchStepActions_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) => layoutController?.Apply();

        private void AddPipelineButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu is ContextMenu menu)
            {
                menu.PlacementTarget = button;
                menu.IsOpen = true;
            }
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
            private bool inputBPreviewVisible = true;
            private bool? workbenchInputBRouteVisible;

            public DockedInspectorLayoutController(VisionToolDoubleInputCustomToolShell shell)
            {
                this.shell = shell ?? throw new ArgumentNullException(nameof(shell));
            }

            public void Apply()
            {
                bool docked = shell.IsDockedInspectorMode;
                bool inlineRoute = docked && (shell.btnWorkbenchDefaults.Visibility == Visibility.Visible || shell.workbenchStepActions.Visibility == Visibility.Visible);
                bool inputBVisible = inlineRoute ? workbenchInputBRouteVisible ?? inputBPreviewVisible : inputBPreviewVisible;
                shell.gbInputB.Visibility = inputBVisible ? Visibility.Visible : Visibility.Collapsed;

                shell.MinWidth = docked ? 0D : 920D;
                shell.MinHeight = docked ? 0D : 620D;
                shell.shellRoot.Margin = docked ? new Thickness(8) : new Thickness(14);
                shell.previewColumn.Width = inlineRoute ? new GridLength(0D) : docked ? new GridLength(180D) : new GridLength(390D);
                shell.flowColumn.Width = docked ? new GridLength(0D) : new GridLength(16D);
                shell.parameterColumn.Width = new GridLength(1D, GridUnitType.Star);
                shell.routeRow.Height = inlineRoute ? GridLength.Auto : new GridLength(0D);
                shell.routeGapRow.Height = new GridLength(inlineRoute ? 8D : 0D);
                Grid.SetRow(shell.previewScrollViewer, inlineRoute ? 0 : 2);
                Grid.SetRowSpan(shell.previewScrollViewer, inlineRoute || docked ? 1 : 2);
                Grid.SetColumn(shell.previewScrollViewer, 0);
                Grid.SetColumnSpan(shell.previewScrollViewer, inlineRoute ? 3 : 1);
                Grid.SetRow(shell.parameterPanel, 2);
                Grid.SetRow(shell.footerPanel, 3);
                Grid.SetRow(shell.flowRail, 2);
                Grid.SetColumn(shell.footerPanel, docked ? 0 : 2);
                Grid.SetColumnSpan(shell.footerPanel, docked ? 3 : 1);
                shell.flowRail.Visibility = docked ? Visibility.Collapsed : Visibility.Visible;
                shell.previewScrollViewer.VerticalScrollBarVisibility = docked && !inlineRoute ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
                if (inlineRoute) { shell.routeGrid.MinHeight = 0D; }
                else { shell.routeGrid.SetBinding(MinHeightProperty, new Binding(nameof(ActualHeight)) { Source = shell.previewScrollViewer }); }
                shell.routeInputGapColumn.Width = new GridLength(inlineRoute ? 8D : 0D);
                shell.routeInputBColumn.Width = inlineRoute ? new GridLength(1D, GridUnitType.Star) : new GridLength(0D);
                Grid.SetRow(shell.gbInputB, inlineRoute ? 0 : 2);
                Grid.SetColumn(shell.gbInputB, inlineRoute ? 2 : 0);
                Grid.SetRow(shell.gbOutputLayer, inlineRoute && !inputBVisible ? 0 : 4);
                Grid.SetColumn(shell.gbOutputLayer, inlineRoute && !inputBVisible ? 2 : 0);
                Grid.SetColumnSpan(shell.gbOutputLayer, inlineRoute && inputBVisible ? 3 : 1);
                shell.bdInputAPreview.Visibility = shell.bdInputBPreview.Visibility = shell.bdOutputPreview.Visibility = inlineRoute ? Visibility.Collapsed : Visibility.Visible;
                shell.inputAPreviewRow.Height = shell.inputBPreviewRow.Height = shell.outputPreviewRow.Height = inlineRoute ? new GridLength(0D) : new GridLength(1D, GridUnitType.Star);
                shell.inputAPreviewGapRow.Height = shell.inputBPreviewGapRow.Height = shell.outputPreviewGapRow.Height = new GridLength(inlineRoute ? 0D : 8D);
                shell.cbInputA.Height = shell.cbInputB.Height = inlineRoute ? 28D : double.NaN;
                shell.cbInputA.MinHeight = shell.cbInputB.MinHeight = inlineRoute ? 28D : 32D;
                shell.cbInputA.SetResourceReference(StyleProperty, inlineRoute ? "VisionTool.WorkbenchRouteComboBoxStyle" : "VisionTool.InputLayerComboBoxStyle");
                shell.cbInputB.SetResourceReference(StyleProperty, inlineRoute ? "VisionTool.WorkbenchRouteComboBoxStyle" : "VisionTool.InputLayerComboBoxStyle");
                shell.cbOutputLayer.SetResourceReference(StyleProperty, inlineRoute ? "VisionTool.WorkbenchRouteComboBoxStyle" : "VisionTool.OutputLayerComboBoxStyle");
                shell.txtOutputLayerName.Visibility = inlineRoute ? Visibility.Collapsed : Visibility.Visible;
                shell.btnCreateOutputLayer.Visibility = inlineRoute ? Visibility.Collapsed : Visibility.Visible;
                shell.inputALoadGapColumn.Width = shell.inputBLoadGapColumn.Width = new GridLength(inlineRoute ? 4D : 0D);
                shell.inputALoadColumn.Width = shell.inputBLoadColumn.Width = new GridLength(inlineRoute ? 28D : 0D);
                foreach (Button loadButton in new[] { shell.btnLoadInputAImage, shell.btnLoadInputBImage })
                {
                    Grid.SetRow(loadButton, inlineRoute ? 2 : 0);
                    Grid.SetColumn(loadButton, inlineRoute ? 2 : 0);
                    loadButton.Width = loadButton.Height = inlineRoute ? 28D : 32D;
                    loadButton.Margin = inlineRoute ? new Thickness(0D) : new Thickness(4D);
                    loadButton.VerticalAlignment = inlineRoute ? VerticalAlignment.Center : VerticalAlignment.Top;
                }
                shell.bdInputAPreview.Height = shell.bdInputBPreview.Height = shell.bdOutputPreview.Height = docked ? 64D : double.NaN;

                shell.rowInputA.Height = docked ? GridLength.Auto : new GridLength(1D, GridUnitType.Star);
                shell.rowInputBGap.Height = new GridLength(inputBVisible && !inlineRoute ? docked ? 6D : 8D : 0D);
                shell.rowInputB.Height = inlineRoute || !inputBVisible ? new GridLength(0D)
                    : docked ? GridLength.Auto : new GridLength(1D, GridUnitType.Star);
                shell.rowOutputGap.Height = new GridLength(inlineRoute && !inputBVisible ? 0D : docked ? 6D : 8D);
                shell.rowOutput.Height = inlineRoute ? inputBVisible ? GridLength.Auto : new GridLength(0D) : new GridLength(1D, GridUnitType.Star);

                shell.titleRow.Height = docked ? new GridLength(0D) : GridLength.Auto;
                shell.titleGapRow.Height = docked ? new GridLength(0D) : new GridLength(14D);
                shell.summaryGapRow.Height = docked ? new GridLength(8D) : new GridLength(12D);
                shell.actionGapRow.Height = docked ? new GridLength(8D) : new GridLength(14D);

                foreach (HeaderedContentControl group in new[] { shell.gbInputA, shell.gbInputB, shell.gbOutputLayer })
                {
                    group.SetResourceReference(StyleProperty, inlineRoute ? "VisionTool.WorkbenchRouteStyle" : typeof(GroupBox));
                    group.Padding = inlineRoute ? new Thickness(0D) : new Thickness(8D);
                    ApplyPreviewCardDocking(group, docked && !inlineRoute, group.Visibility == Visibility.Visible);
                }
                ApplySummaryStatusDensity(docked);
                ApplyActionRows(IsOffsetActionActive());
            }

            public void SetInputBPreviewVisible(bool visible)
            {
                inputBPreviewVisible = visible;
                shell.gbInputB.IsEnabled = visible;
                shell.rowInputB.Height = visible
                    ? shell.IsDockedInspectorMode ? GridLength.Auto : new GridLength(1, GridUnitType.Star)
                    : new GridLength(0);
                shell.rowInputBGap.Height = visible ? new GridLength(shell.IsDockedInspectorMode ? 6D : 8D) : new GridLength(0);
                Apply();
            }

            public void SetWorkbenchInputBRouteVisible(bool visible)
            {
                workbenchInputBRouteVisible = visible;
                Apply();
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
                shell.txtStatus.TextTrimming = TextTrimming.None;
            }

            private static void ApplyPreviewCardDocking(HeaderedContentControl group, bool docked, bool visible)
            {
                if (group == null)
                {
                    return;
                }

                group.VerticalAlignment = docked ? VerticalAlignment.Top : VerticalAlignment.Stretch;
                group.Height = double.NaN;
                group.MinHeight = docked && visible ? DockedPreviewCardHeight : 0D;
            }

            private bool IsOffsetActionActive()
            {
                return shell.btnRunOffset.Visibility == Visibility.Visible
                    && (shell.rowRunOffsetAction.Height.Value > 0D || Grid.GetRow(shell.btnRunOffset) == 0);
            }

            private void ApplyActionRows(bool useOffsetMode)
            {
                double actionHeight = shell.IsDockedInspectorMode ? DockedActionHeight : FloatingActionHeight;
                double actionGap = shell.IsDockedInspectorMode ? DockedActionGap : FloatingActionGap;
                bool stepRow = shell.IsDockedInspectorMode && shell.workbenchStepActions.Visibility == Visibility.Visible;
                bool testRow = shell.btnWorkbenchDefaults.Visibility == Visibility.Visible;
                bool oneRow = stepRow || testRow;
                shell.btnRunPreview.Height = shell.btnRunOffset.Height = oneRow ? 34D : double.NaN;

                shell.actionPanel.ColumnDefinitions[0].Width = testRow ? GridLength.Auto : new GridLength(1D, GridUnitType.Star);
                shell.actionPanel.ColumnDefinitions[1].Width = new GridLength(testRow ? 6D : 4D);
                shell.actionPanel.ColumnDefinitions[2].Width = testRow ? GridLength.Auto : new GridLength(1D, GridUnitType.Star);
                shell.actionPanel.ColumnDefinitions[3].Width = testRow ? new GridLength(1D, GridUnitType.Star) : new GridLength(4D);
                shell.actionPanel.ColumnDefinitions[4].Width = testRow ? GridLength.Auto : new GridLength(1D, GridUnitType.Star);
                shell.actionPanel.ColumnDefinitions[5].Width = new GridLength(testRow ? 6D : 0D);
                shell.actionPanel.ColumnDefinitions[6].Width = testRow ? GridLength.Auto : new GridLength(0D);
                Grid.SetColumn(shell.btnAddPipeline, testRow ? 4 : 0);
                Grid.SetColumnSpan(shell.btnAddPipeline, testRow ? 1 : 5);

                shell.rowAddPipelineAction.Height = new GridLength(testRow ? 34D : actionHeight);
                shell.rowRunPreviewGap.Height = oneRow || useOffsetMode ? new GridLength(0) : new GridLength(actionGap);
                shell.rowRunPreviewAction.Height = oneRow || useOffsetMode ? new GridLength(0) : new GridLength(actionHeight);
                shell.rowRunOffsetGap.Height = !oneRow && useOffsetMode ? new GridLength(actionGap) : new GridLength(0);
                shell.rowRunOffsetAction.Height = !oneRow && useOffsetMode ? new GridLength(actionHeight) : new GridLength(0);
                Grid.SetColumnSpan(shell.workbenchStepActions, stepRow ? 3 : 5);
                Grid.SetRow(shell.btnRunPreview, oneRow ? 0 : 2);
                Grid.SetColumn(shell.btnRunPreview, testRow ? 6 : stepRow ? 4 : 0);
                Grid.SetColumnSpan(shell.btnRunPreview, oneRow ? 1 : 5);
                Grid.SetRow(shell.btnRunOffset, oneRow ? 0 : 4);
                Grid.SetColumn(shell.btnRunOffset, testRow ? 6 : stepRow ? 4 : 0);
                Grid.SetColumnSpan(shell.btnRunOffset, oneRow ? 1 : 5);
            }
        }

        #endregion
    }
}
