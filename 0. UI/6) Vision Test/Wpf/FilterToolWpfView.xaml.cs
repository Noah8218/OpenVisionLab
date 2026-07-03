using Lib.OpenCV.Property;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using OpenVisionLab.Contracts;

namespace OpenVisionLab
{
    public partial class FilterToolWpfView : UserControl, ISingleInputPropertyVisionToolWpfView<FilterToolProperty>, IVisionToolPreviewImageCommands, IVisionToolViewLifetime
    {
        private readonly FilterToolPresenter presenter;

        private readonly VisionToolSingleInputCustomToolController toolController;
        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly VisionToolKernelSizeController kernelSizeController;
        private readonly VisionToolFilterInteractionController filterInteractionController;
        private bool suppressEvents = true;

        internal FilterToolWpfView(FilterToolPresenter presenter)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            InitializeComponent();
            previewScheduler = new VisionToolDebouncedPreviewScheduler(this, () => toolController?.RequestRunPreview());
            parameterChangeController = new VisionToolParameterChangeController(() => suppressEvents, UpdateSummary, schedulePreview: previewScheduler.Schedule);
            kernelSizeController = new VisionToolKernelSizeController(
                parameterChangeController,
                txtWidth,
                chkLockSize,
                presenter.SetKernelPreset,
                presenter.SyncKernelHeightToWidth,
                value => suppressEvents = value);
            filterInteractionController = new VisionToolFilterInteractionController(
                presenter,
                parameterChangeController,
                cbFilterType,
                cbBorderType,
                panelWidth,
                panelHeight,
                panelKernelPresets,
                chkLockSize,
                panelMedian,
                panelDiameter,
                panelSigmaColor,
                panelSigmaSpace,
                txtWidth,
                txtHeight,
                txtMedianKernel,
                txtDiameter,
                txtSigmaColor,
                txtSigmaSpace);
            toolController = VisionToolSingleInputCustomToolController.Attach(
                this,
                "VisionMenu.Filter",
                parameterContentHost,
                refreshViewState: UpdateSummary,
                clearResultReview: null,
                applyToolLocalization: ApplyLocalization);
            toolController.BindSummary(new Binding("Summary"));
            ApplyLocalization();
            filterInteractionController.InitializeOptions();
            parameterChangeController.RefreshProgrammatic(filterInteractionController.RefreshModePanels);
            suppressEvents = false;
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
            previewScheduler.Dispose();
        }

        private void ApplyLocalization()
        {
            toolController.ApplyLocalization();
            gbFilterOptions.Header = OpenVisionLanguageService.T("Arithmetic.Operation");
            gbKernel.Header = OpenVisionLanguageService.T("PropertyGrid.Category.Kernel");
            lblFilterType.Text = OpenVisionLanguageService.T("PropertyGrid.Property.FilterType.DisplayName");
            lblBorderType.Text = OpenVisionLanguageService.T("PropertyGrid.Property.BorderType.DisplayName");
            lblKernelWidth.Text = OpenVisionLanguageService.T("PropertyGrid.Property.KernelWidth.DisplayName");
            lblKernelHeight.Text = OpenVisionLanguageService.T("PropertyGrid.Property.KernelHeight.DisplayName");
            lblMedianKernel.Text = OpenVisionLanguageService.T("PropertyGrid.Property.MedianKernelSize.DisplayName");
            lblDiameter.Text = OpenVisionLanguageService.T("PropertyGrid.Property.Diameter.DisplayName");
            lblSigmaColor.Text = OpenVisionLanguageService.T("PropertyGrid.Property.SigmaColor.DisplayName");
            lblSigmaSpace.Text = OpenVisionLanguageService.T("PropertyGrid.Property.SigmaSpace.DisplayName");
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

        public FilterToolProperty CreateProperty()
        {
            filterInteractionController.FlushParameterBindings();
            return presenter.CreateProperty();
        }



        private void FilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterInteractionController?.HandleFilterTypeChanged();
        }

        private void BorderType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterInteractionController?.HandleParameterSelectionChanged();
        }


        private void Parameter_TextChanged(object sender, TextChangedEventArgs e)
        {
            kernelSizeController?.HandleTextChanged(sender);
        }

        private void LockSize_Changed(object sender, RoutedEventArgs e)
        {
            kernelSizeController?.HandleLockChanged();
        }

        private void KernelPreset_Click(object sender, RoutedEventArgs e)
        {
            kernelSizeController?.HandlePresetClick(sender);
        }
        private void UpdateSummary()
        {
            if (toolController == null)
            {
                return;
            }

            filterInteractionController?.FlushParameterBindings();
            toolController.RefreshSummaryBinding();
        }

    }
}
