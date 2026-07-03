using Lib.OpenCV.Property;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using OpenVisionLab.Contracts;
using OpenVisionLab.Services;

namespace OpenVisionLab
{
    public partial class MorphologyToolWpfView : UserControl, ISingleInputPropertyVisionToolWpfView<MorphologyToolProperty>, IVisionToolPreviewImageCommands, IVisionToolViewLifetime
    {
        private readonly MorphologyToolPresenter presenter;

        private readonly VisionToolSingleInputCustomToolController toolController;
        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly VisionToolKernelSizeController kernelSizeController;
        private readonly VisionToolMorphologyInteractionController morphologyInteractionController;
        private bool suppressEvents = true;

        internal MorphologyToolWpfView(MorphologyToolPresenter presenter)
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
            morphologyInteractionController = new VisionToolMorphologyInteractionController(
                presenter,
                parameterChangeController,
                this,
                new[]
                {
                    btnMorphOperationErode,
                    btnMorphOperationDilate,
                    btnMorphOperationOpen,
                    btnMorphOperationClose,
                    btnMorphOperationTopHat,
                    btnMorphOperationBlackHat,
                    btnMorphOperationHitMiss,
                    btnMorphOperationGradient
                },
                new[]
                {
                    rdoShapeRect,
                    rdoShapeEllipse,
                    rdoShapeCross
                });
            toolController = VisionToolSingleInputCustomToolController.Attach(
                this,
                "VisionMenu.Morphology",
                parameterContentHost,
                refreshViewState: UpdateSummary,
                clearResultReview: null,
                applyToolLocalization: ApplyLocalization);
            ApplyLocalization();
            parameterChangeController.RefreshProgrammatic(morphologyInteractionController.RefreshOperationButtons);
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
            gbOperation.Header = OpenVisionLanguageService.T("Arithmetic.Operation");
            gbKernel.Header = OpenVisionLanguageService.T("PropertyGrid.Category.Kernel");
            lblKernelWidth.Text = OpenVisionLanguageService.T("PropertyGrid.Property.KernelWidth.DisplayName");
            lblKernelHeight.Text = OpenVisionLanguageService.T("PropertyGrid.Property.KernelHeight.DisplayName");
            lblShape.Text = OpenVisionLanguageService.T("PropertyGrid.Property.Shape.DisplayName");
            morphologyInteractionController.RefreshLabels();
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

        public MorphologyToolProperty CreateProperty()
        {
            FlushParameterBindings();
            return presenter.CreateProperty();
        }

        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            morphologyInteractionController?.HandleOperationClick(sender);
        }

        private void Shape_Checked(object sender, RoutedEventArgs e)
        {
            morphologyInteractionController?.HandleShapeChecked(sender);
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

            FlushParameterBindings();
            toolController.SetSummaryText(morphologyInteractionController.CreateSummary());
        }

        private void FlushParameterBindings()
        {
            VisionToolControlBinding.UpdateTextSources(txtWidth, txtHeight);
        }
    }
}
