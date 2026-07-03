using Lib.OpenCV;
using Lib.OpenCV.Property;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using OpenVisionLab.Contracts;
using OpenVisionLab.Services;

namespace OpenVisionLab
{
    public partial class ThresholdToolWpfView : UserControl, ISingleInputPropertyVisionToolWpfView<ThresholdToolProperty>, IVisionToolPreviewImageCommands, IVisionToolViewLifetime
    {
        private readonly ThresholdToolPresenter presenter;

        private readonly VisionToolSingleInputCustomToolController toolController;
        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly VisionToolThresholdInteractionController thresholdInteractionController;
        private bool suppressEvents = true;

        internal ThresholdToolWpfView(ThresholdToolPresenter presenter)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            InitializeComponent();
            previewScheduler = new VisionToolDebouncedPreviewScheduler(this, () => toolController?.RequestRunPreview());
            parameterChangeController = new VisionToolParameterChangeController(() => suppressEvents, UpdateSummary, schedulePreview: ScheduleAutoPreview);
            thresholdInteractionController = new VisionToolThresholdInteractionController(
                presenter,
                parameterChangeController,
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
            toolController = VisionToolSingleInputCustomToolController.Attach(
                this,
                "VisionMenu.Threshold",
                parameterContentHost,
                refreshViewState: UpdateSummary,
                clearResultReview: null,
                applyToolLocalization: ApplyLocalization);
            toolController.BindSummary(new Binding("Summary"));

            ApplyLocalization();
            suppressEvents = false;
            parameterChangeController.RefreshProgrammatic(thresholdInteractionController.RefreshModePanels);
        }

        public event EventHandler SourceLayerChanged
        {
            add { toolController.SourceLayerChanged += value; }
            remove { toolController.SourceLayerChanged -= value; }
        }

        public event EventHandler DestinationLayerChanged
        {
            add { toolController.DestinationLayerChanged += value; }
            remove { toolController.DestinationLayerChanged -= value; }
        }

        public event EventHandler InputPreviewClicked
        {
            add { toolController.InputPreviewClicked += value; }
            remove { toolController.InputPreviewClicked -= value; }
        }

        public event EventHandler OutputPreviewClicked
        {
            add { toolController.OutputPreviewClicked += value; }
            remove { toolController.OutputPreviewClicked -= value; }
        }

        public event EventHandler CreateOutputLayerRequested
        {
            add { toolController.CreateOutputLayerRequested += value; }
            remove { toolController.CreateOutputLayerRequested -= value; }
        }

        public event EventHandler RunPreviewRequested
        {
            add { toolController.RunPreviewRequested += value; }
            remove { toolController.RunPreviewRequested -= value; }
        }

        public event EventHandler AddPipelineRequested
        {
            add { toolController.AddPipelineRequested += value; }
            remove { toolController.AddPipelineRequested -= value; }
        }

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> LoadPreviewImageRequested
        {
            add { toolController.LoadPreviewImageRequested += value; }
            remove { toolController.LoadPreviewImageRequested -= value; }
        }

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> SavePreviewImageRequested
        {
            add { toolController.SavePreviewImageRequested += value; }
            remove { toolController.SavePreviewImageRequested -= value; }
        }

        public string SelectedInputLayer => toolController.SelectedInputLayer;
        public string SelectedOutputLayer => toolController.SelectedOutputLayer;

        public void DisposeView()
        {
            toolController.Dispose();
            thresholdInteractionController.Detach();
            previewScheduler.Dispose();
        }

        private void ApplyLocalization()
        {
            toolController.ApplyLocalization();
            gbThresholdParameters.Header = OpenVisionLanguageService.T("Pipeline.ResultRow.Parameters");
            gbMode.Header = OpenVisionLanguageService.T("Threshold.Mode");

            txtModeBasicTitle.Text = OpenVisionLanguageService.T("Threshold.ModeBasic");
            txtModeBasicHint.Text = OpenVisionLanguageService.T("Threshold.BasicHint");
            txtModeRangeTitle.Text = OpenVisionLanguageService.T("Threshold.ModeRange");
            txtModeRangeHint.Text = OpenVisionLanguageService.T("Threshold.RangeHint");
            txtModeAdaptiveTitle.Text = OpenVisionLanguageService.T("Threshold.ModeAdaptive");
            txtModeAdaptiveHint.Text = OpenVisionLanguageService.T("Threshold.AdaptiveHint");

            lblBasicType.Text = OpenVisionLanguageService.T("Threshold.ResultType");
            rbBasicBinary.Content = OpenVisionLanguageService.T("Threshold.Binary");
            rbBasicInvert.Content = OpenVisionLanguageService.T("Threshold.BinaryInv");
            lblBasicMaxValue.Text = OpenVisionLanguageService.T("Threshold.MaxValue");
            lblBasicThreshold.Text = OpenVisionLanguageService.T("PropertyGrid.Property.Threshold.DisplayName");
            lblRangeTitle.Text = OpenVisionLanguageService.T("Threshold.RangeTitle");
            lblRangeMin.Text = OpenVisionLanguageService.T("Threshold.RangeMin");
            lblRangeMax.Text = OpenVisionLanguageService.T("Threshold.RangeMax");
            chkRangeInvert.Content = OpenVisionLanguageService.T("Threshold.Invert");
            lblAdaptiveMethod.Text = OpenVisionLanguageService.T("Threshold.Method");
            rbAdaptiveMean.Content = OpenVisionLanguageService.T("Threshold.MeanC");
            rbAdaptiveGaussian.Content = OpenVisionLanguageService.T("Threshold.GaussianC");
            lblAdaptiveType.Text = OpenVisionLanguageService.T("Threshold.ResultType");
            rbAdaptiveBinary.Content = OpenVisionLanguageService.T("Threshold.Binary");
            rbAdaptiveInvert.Content = OpenVisionLanguageService.T("Threshold.BinaryInv");
            lblAdaptiveMaxValue.Text = OpenVisionLanguageService.T("Threshold.MaxValue");
            lblAdaptiveWeight.Text = OpenVisionLanguageService.T("Threshold.Weight");
            lblBlockSize.Text = OpenVisionLanguageService.T("Threshold.BlockSize");
        }
        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayer, string selectedOutputLayer)
        {
            toolController.SetLayerList(layerNames, selectedInputLayer, selectedOutputLayer);
        }

        public void SetInputPreview(Bitmap image)
        {
            toolController.SetInputPreview(image);
        }

        public void SetOutputPreview(Bitmap image)
        {
            toolController.SetOutputPreview(image);
        }

        public void SetStatus(string status)
        {
            toolController.SetStatus(status);
        }

        public ThresholdToolProperty CreateProperty()
        {
            thresholdInteractionController.FlushParameterBindings();
            return presenter.CreateProperty();
        }

        public void ConfigureBasicInvertForTest(bool invert)
        {
            bool previousSuppressEvents = suppressEvents;
            suppressEvents = true;
            try
            {
                presenter.Mode = ThresholdToolMode.Threshold;
                presenter.BasicInvert = invert;
                rbBasic.IsChecked = true;
                rbBasicBinary.IsChecked = !invert;
                rbBasicInvert.IsChecked = invert;
            }
            finally
            {
                suppressEvents = previousSuppressEvents;
            }

            // CreateProperty flushes control bindings, so keep the visible controls and ViewModel in the same state.
            parameterChangeController.RefreshProgrammatic(thresholdInteractionController.RefreshModePanels);
        }

        private void ScheduleAutoPreview()
        {
            if (suppressEvents || !IsLoaded)
            {
                return;
            }

            previewScheduler.Schedule();
        }
        private void UpdateSummary()
        {
            if (toolController == null)
            {
                return;
            }

            thresholdInteractionController?.FlushParameterBindings();
            toolController.RefreshSummaryBinding();
        }

    }
}
