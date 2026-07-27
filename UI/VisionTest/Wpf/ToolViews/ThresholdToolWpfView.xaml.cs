using Lib.OpenCV.Property;
using System;
using System.Windows;
using System.Windows.Data;
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
        private bool suppressEvents = true;

        internal ThresholdToolWpfView(ThresholdToolPresenter presenter)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            InitializeComponent();
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
            signalInspector.MarkerValueChangeRequested += SignalInspector_MarkerValueChangeRequested;
            learnWindowController = new ThresholdToolLearnWindowController(
                presenter,
                thresholdInteractionController,
                () => Window.GetWindow(this));
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

            ApplyLocalization();
            suppressEvents = false;
            parameterChangeController.RefreshProgrammatic(thresholdInteractionController.RefreshModePanels);
        }

        protected override void DisposeToolResources()
        {
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

        internal bool IsSignalInspectorOverlayVisibleForTest =>
            signalInspectorOverlay.Visibility == Visibility.Visible;

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

        internal void ShowSignalEvidence(VisionToolSignalEvidence evidence)
        {
            signalInspector.ShowEvidence(evidence);
            btnOpenSignalInspector.Visibility = Visibility.Visible;
            signalInspectorOverlay.Visibility = Visibility.Visible;
        }

        internal void ClearSignalEvidence()
        {
            signalInspector.ClearEvidence();
            btnOpenSignalInspector.Visibility = Visibility.Collapsed;
            signalInspectorOverlay.Visibility = Visibility.Collapsed;
        }

        internal void CloseSignalInspectorForTest()
        {
            signalInspectorOverlay.Visibility = Visibility.Collapsed;
        }

        internal void OpenSignalInspectorForTest()
        {
            if (signalInspector.HasEvidence)
            {
                signalInspectorOverlay.Visibility = Visibility.Visible;
            }
        }

        private void OpenThresholdGuide_Click(object sender, RoutedEventArgs e)
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
