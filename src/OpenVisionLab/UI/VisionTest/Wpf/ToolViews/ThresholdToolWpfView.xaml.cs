using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Property;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using OpenVisionLab.Contracts;
using OpenVisionLab.Services;

namespace OpenVisionLab
{
    public partial class ThresholdToolWpfView : VisionToolSingleInputCustomToolViewBase, ISingleInputPropertyVisionToolWpfView<ThresholdToolProperty>
    {
        private readonly ThresholdToolPresenter presenter;

        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly VisionToolThresholdInteractionController thresholdInteractionController;
        private readonly ThresholdToolLearnWindowController learnWindowController;
        private readonly ThresholdToolTextPresenter textPresenter;
        private readonly VisionToolCustomParameterGuideBinder parameterGuideBinder;
        private readonly DispatcherTimer signalEvidenceCueTimer;
        private readonly ThresholdToolSuggestionController thresholdSuggestionController;
        private bool suppressEvents = true;

        internal ThresholdToolWpfView(ThresholdToolPresenter presenter)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            InitializeComponent();
            signalInspector.SetExportAction(VisionToolSignalEvidenceExporter.ExportTsv);
            previewScheduler = new VisionToolDebouncedPreviewScheduler(this, RequestRunPreview);
            parameterChangeController = new VisionToolParameterChangeController(
                () => suppressEvents,
                RefreshSummaryAndClearSignalEvidence,
                schedulePreview: previewScheduler.Schedule);
            thresholdInteractionController = new VisionToolThresholdInteractionController(
                presenter,
                parameterChangeController,
                () => suppressEvents,
                value => suppressEvents = value,
                rbBasic,
                rbRange,
                rbAdaptive,
                rbBasicBinary,
                rbBasicInvert,
                rbAdaptiveMean,
                rbAdaptiveGaussian,
                rbAdaptiveBinary,
                rbAdaptiveInvert,
                chkRangeInvert,
                sliderThreshold,
                sliderRangeMin,
                sliderRangeMax,
                sliderBlockSize,
                txtThreshold,
                txtMaxValue,
                txtRangeMin,
                txtRangeMax,
                txtAdaptiveMaxValue,
                txtWeight,
                txtBlockSize,
                panelBasic,
                panelRange,
                panelAdaptive);
            thresholdSuggestionController = new ThresholdToolSuggestionController(
                () => signalInspector.CurrentEvidence,
                CreateProperty,
                thresholdInteractionController,
                visible => thresholdSuggestionPanel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed,
                enabled => btnUseThresholdSuggestion.IsEnabled = enabled,
                enabled => btnUndoThresholdSuggestion.IsEnabled = enabled,
                status => thresholdSuggestionStatus.Text = status,
                marker => signalInspector.SetAdvisoryMarkers(marker));
            signalInspector.MarkerValueChangeRequested += SignalInspector_MarkerValueChangeRequested;
            signalEvidenceCueTimer = new DispatcherTimer(DispatcherPriority.Background, Dispatcher)
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            signalEvidenceCueTimer.Tick += SignalEvidenceCueTimer_Tick;
            learnWindowController = new ThresholdToolLearnWindowController(
                presenter,
                thresholdInteractionController,
                () => Window.GetWindow(this));
            toolShell.LearnTopicRequested += ToolShell_LearnTopicRequested;
            textPresenter = new ThresholdToolTextPresenter(
                gbThresholdParameters,
                gbMode,
                txtModeBasicTitle,
                txtModeBasicHint,
                txtModeRangeTitle,
                txtModeRangeHint,
                txtModeAdaptiveTitle,
                txtModeAdaptiveHint,
                lblBasicType,
                rbBasicBinary,
                rbBasicInvert,
                lblBasicMaxValue,
                lblBasicThreshold,
                lblRangeTitle,
                lblRangeMin,
                lblRangeMax,
                chkRangeInvert,
                lblAdaptiveMethod,
                rbAdaptiveMean,
                rbAdaptiveGaussian,
                lblAdaptiveType,
                rbAdaptiveBinary,
                rbAdaptiveInvert,
                lblAdaptiveMaxValue,
                lblAdaptiveWeight,
                lblBlockSize);
            AttachToolController(
                "VisionMenu.Threshold",
                parameterContentHost,
                refreshViewState: UpdateSummary,
                clearResultReview: ClearSignalEvidence,
                applyToolLocalization: ApplyLocalization);
            ToolController.BindSummary(new Binding("Summary"));
            parameterGuideBinder = VisionToolCustomParameterGuideBinder.Attach(
                toolShell,
                presenter.CreateProperty,
                new Dictionary<FrameworkElement, string>
                {
                    [rbBasic] = nameof(ThresholdToolProperty.Mode),
                    [rbRange] = nameof(ThresholdToolProperty.Mode),
                    [rbAdaptive] = nameof(ThresholdToolProperty.Mode),
                    [rbBasicBinary] = nameof(ThresholdToolProperty.ThresholdType),
                    [rbBasicInvert] = nameof(ThresholdToolProperty.ThresholdType),
                    [txtThreshold] = nameof(ThresholdToolProperty.Threshold),
                    [sliderThreshold] = nameof(ThresholdToolProperty.Threshold),
                    [txtMaxValue] = nameof(ThresholdToolProperty.MaxValue),
                    [txtRangeMin] = nameof(ThresholdToolProperty.RangeMin),
                    [sliderRangeMin] = nameof(ThresholdToolProperty.RangeMin),
                    [txtRangeMax] = nameof(ThresholdToolProperty.RangeMax),
                    [sliderRangeMax] = nameof(ThresholdToolProperty.RangeMax),
                    [chkRangeInvert] = nameof(ThresholdToolProperty.Invert),
                    [rbAdaptiveMean] = nameof(ThresholdToolProperty.AdaptiveType),
                    [rbAdaptiveGaussian] = nameof(ThresholdToolProperty.AdaptiveType),
                    [rbAdaptiveBinary] = nameof(ThresholdToolProperty.AdaptiveThresholdType),
                    [rbAdaptiveInvert] = nameof(ThresholdToolProperty.AdaptiveThresholdType),
                    [txtAdaptiveMaxValue] = nameof(ThresholdToolProperty.MaxValue),
                    [txtBlockSize] = nameof(ThresholdToolProperty.BlockSize),
                    [sliderBlockSize] = nameof(ThresholdToolProperty.BlockSize),
                    [txtWeight] = nameof(ThresholdToolProperty.Weight)
                });

            ApplyLocalization();
            suppressEvents = false;
            parameterChangeController.RefreshProgrammatic(thresholdInteractionController.RefreshModePanels);
        }

        protected override void DisposeToolResources()
        {
            signalEvidenceCueTimer.Stop();
            signalEvidenceCueTimer.Tick -= SignalEvidenceCueTimer_Tick;
            parameterGuideBinder.Dispose();
            toolShell.LearnTopicRequested -= ToolShell_LearnTopicRequested;
            learnWindowController.Dispose();
            signalInspector.MarkerValueChangeRequested -= SignalInspector_MarkerValueChangeRequested;
            thresholdInteractionController.Detach();
            previewScheduler.Dispose();
        }

        private void ApplyLocalization()
        {
            ToolController.ApplyLocalization();
            textPresenter.ApplyLocalization();
            signalInspector.ApplyLocalization();
            btnCloseSignalInspector.Content =
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "매개변수로 돌아가기"
                    : "Back to parameters";
            btnOpenSignalInspector.Content =
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "분포 다시 보기"
                    : "Review distribution";
            bool korean = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean;
            btnOpenSignalInspector.ToolTip = korean
                ? "\uD604\uC7AC Preview\uC758 \uBC1D\uAE30 \uBD84\uD3EC\uC640 \uAE30\uC900\uAC12\uC744 \uAC80\uD1A0\uD569\uB2C8\uB2E4."
                : "Review the current Preview brightness distribution and cutoff.";
            txtSignalEvidenceCue.Text = korean
                ? "\uBD84\uD3EC \uAC31\uC2E0\uB428"
                : "Distribution updated";
            thresholdSuggestionTitle.Text = korean
                ? "Threshold 티칭 제안"
                : "Threshold teaching suggestion";
            btnAnalyzeThresholdSuggestion.Content = korean ? "제안 분석" : "Analyze suggestion";
            btnUseThresholdSuggestion.Content = korean ? "T 사용" : "Use T";
            btnUndoThresholdSuggestion.Content = korean ? "이전 T 복원" : "Undo";
        }

        public ThresholdToolProperty CreateProperty()
        {
            thresholdInteractionController.FlushParameterBindings();
            return presenter.CreateProperty();
        }

        public void ConfigureBasicInvertForTest(bool invert)
        {
            thresholdInteractionController.ConfigureBasicInvertForTest(invert);
        }

        public void OpenThresholdGuideForTest()
        {
            learnWindowController.Open();
        }

        internal bool SignalInspectorHasEvidenceForTest => signalInspector.HasEvidence;

        internal string SignalInspectorEvidenceIdForTest => signalInspector.EvidenceId;

        internal string SignalInspectorSourceSha256ForTest => signalInspector.SourceSha256;

        internal int SignalInspectorSeriesCountForTest => signalInspector.SeriesCount;

        internal int SignalInspectorMarkerCountForTest => signalInspector.MarkerCount;

        internal int SignalInspectorAdvisoryMarkerCountForTest => signalInspector.AdvisoryMarkerCount;

        internal bool IsThresholdSuggestionPanelVisibleForTest =>
            thresholdSuggestionPanel.Visibility == Visibility.Visible;

        internal bool HasThresholdSuggestionForTest => thresholdSuggestionController.HasAcceptedSuggestion;

        internal int ThresholdSuggestionValueForTest => thresholdSuggestionController.SuggestedThreshold;

        internal string ThresholdSuggestionStatusForTest => thresholdSuggestionStatus.Text ?? string.Empty;

        internal string ThresholdSuggestionEvidenceIdForTest => thresholdSuggestionController.SuggestionEvidenceId;

        internal bool CanUseThresholdSuggestionForTest => btnUseThresholdSuggestion.IsEnabled;

        internal bool CanUndoThresholdSuggestionForTest => btnUndoThresholdSuggestion.IsEnabled;

        internal bool IsSignalInspectorOverlayVisibleForTest =>
            signalInspectorOverlay.Visibility == Visibility.Visible;

        internal bool IsSignalEvidenceCueVisibleForTest =>
            signalEvidenceCue.Visibility == Visibility.Visible;

        internal double GetSignalInspectorMarkerValueForTest(string markerId)
        {
            return signalInspector.GetMarkerValue(markerId);
        }

        internal void CommitSignalInspectorMarkerForTest(string markerId, double value)
        {
            signalInspector.CommitMarkerForTest(markerId, value);
        }

        internal void ExportSignalEvidenceForTest(string path)
        {
            signalInspector.ExportForTest(path);
        }

        internal void AnalyzeThresholdSuggestionForTest()
        {
            thresholdSuggestionController.Analyze();
        }

        internal void UseThresholdSuggestionForTest()
        {
            thresholdSuggestionController.Use();
        }

        internal void UndoThresholdSuggestionForTest()
        {
            thresholdSuggestionController.Undo();
        }

        internal void ShowSignalEvidence(VisionToolSignalEvidence evidence)
        {
            signalInspector.ShowEvidence(evidence);
            btnOpenSignalInspector.Visibility = Visibility.Visible;
            if (signalInspectorOverlay.Visibility != Visibility.Visible)
            {
                ShowSignalEvidenceCue();
            }
            thresholdSuggestionController.UpdateAvailability(evidence);
        }

        internal void ClearSignalEvidence()
        {
            signalInspector.ClearEvidence();
            thresholdSuggestionController.Clear();
            btnOpenSignalInspector.Visibility = Visibility.Hidden;
            signalInspectorOverlay.Visibility = Visibility.Collapsed;
            HideSignalEvidenceCue();
        }

        internal void CloseSignalInspectorForTest()
        {
            signalInspectorOverlay.Visibility = Visibility.Collapsed;
        }

        internal void OpenSignalInspectorForTest()
        {
            if (signalInspector.HasEvidence)
            {
                HideSignalEvidenceCue();
                signalInspectorOverlay.Visibility = Visibility.Visible;
            }
        }

        private void ToolShell_LearnTopicRequested(object sender, EventArgs e)
        {
            learnWindowController.Open();
        }

        private void CloseSignalInspector_Click(object sender, RoutedEventArgs e)
        {
            signalInspectorOverlay.Visibility = Visibility.Collapsed;
        }

        private void OpenSignalInspector_Click(object sender, RoutedEventArgs e)
        {
            OpenSignalInspectorForTest();
        }

        private void ShowSignalEvidenceCue()
        {
            signalEvidenceCueTimer.Stop();
            signalEvidenceCue.Visibility = Visibility.Visible;
            signalEvidenceCueTimer.Start();
        }

        private void HideSignalEvidenceCue()
        {
            signalEvidenceCueTimer.Stop();
            signalEvidenceCue.Visibility = Visibility.Collapsed;
        }

        private void SignalEvidenceCueTimer_Tick(object sender, EventArgs e)
        {
            HideSignalEvidenceCue();
        }

        private void AnalyzeThresholdSuggestion_Click(object sender, RoutedEventArgs e)
        {
            thresholdSuggestionController.Analyze();
        }

        private void UseThresholdSuggestion_Click(object sender, RoutedEventArgs e)
        {
            thresholdSuggestionController.Use();
        }

        private void UndoThresholdSuggestion_Click(object sender, RoutedEventArgs e)
        {
            thresholdSuggestionController.Undo();
        }

        private void SignalInspector_MarkerValueChangeRequested(
            object sender,
            VisionToolSignalMarkerValueChangedEventArgs e)
        {
            thresholdInteractionController.ApplySignalMarkerValue(e.MarkerId, e.Value);
        }

        private void RefreshSummaryAndClearSignalEvidence()
        {
            UpdateSummary();
            ClearSignalEvidence();
        }

        private void UpdateSummary()
        {
            if (!HasToolController)
            {
                return;
            }

            thresholdInteractionController?.FlushParameterBindings();
            ToolController.RefreshSummaryBinding();
        }

    }
}
