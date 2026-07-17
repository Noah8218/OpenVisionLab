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
            parameterChangeController = new VisionToolParameterChangeController(() => suppressEvents, UpdateSummary, schedulePreview: previewScheduler.Schedule);
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
                clearResultReview: null,
                applyToolLocalization: ApplyLocalization);
            ToolController.BindSummary(new Binding("Summary"));

            ApplyLocalization();
            suppressEvents = false;
            parameterChangeController.RefreshProgrammatic(thresholdInteractionController.RefreshModePanels);
        }

        protected override void DisposeToolResources()
        {
            learnWindowController.Dispose();
            thresholdInteractionController.Detach();
            previewScheduler.Dispose();
        }

        private void ApplyLocalization()
        {
            ToolController.ApplyLocalization();
            textPresenter.ApplyLocalization();
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

        private void OpenThresholdGuide_Click(object sender, RoutedEventArgs e)
        {
            learnWindowController.Open();
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
