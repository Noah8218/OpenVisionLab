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
        private readonly ThresholdToolTextPresenter textPresenter;
        private OpenVisionLearnWindow learnWindow;
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
            if (learnWindow != null)
            {
                learnWindow.Close();
                learnWindow = null;
            }

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
            OpenThresholdGuide_Click(this, new RoutedEventArgs());
        }

        private void OpenThresholdGuide_Click(object sender, RoutedEventArgs e)
        {
            thresholdInteractionController.FlushParameterBindings();
            ThresholdToolProperty property = presenter.CreateProperty();
            if (learnWindow != null)
            {
                learnWindow.Activate();
                return;
            }

            learnWindow = new OpenVisionLearnWindow(
                property.Threshold,
                property.MaxValue,
                property.ThresholdType == OpenCvSharp.ThresholdTypes.BinaryInv)
            {
                Owner = Window.GetWindow(this)
            };
            learnWindow.ApplyThresholdRequested += LearnWindow_ApplyThresholdRequested;
            learnWindow.Closed += LearnWindow_Closed;
            learnWindow.Show();
        }

        private void LearnWindow_ApplyThresholdRequested(object sender, OpenVisionLearnThresholdApplyEventArgs e)
        {
            thresholdInteractionController.ApplyBasicThresholdFromGuide(e.Threshold, e.Invert);
        }

        private void LearnWindow_Closed(object sender, EventArgs e)
        {
            if (learnWindow != null)
            {
                learnWindow.ApplyThresholdRequested -= LearnWindow_ApplyThresholdRequested;
                learnWindow.Closed -= LearnWindow_Closed;
                learnWindow = null;
            }
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
