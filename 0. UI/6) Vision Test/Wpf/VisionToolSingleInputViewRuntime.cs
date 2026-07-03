using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class VisionToolSingleInputViewRuntime : IDisposable
    {
        private readonly VisionToolSingleInputViewBinder binder;

        private VisionToolSingleInputViewRuntime(
            VisionToolSingleInputViewModel viewModel,
            VisionToolSingleInputViewBinder binder)
        {
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.binder = binder ?? throw new ArgumentNullException(nameof(binder));
        }

        public VisionToolSingleInputViewModel ViewModel { get; }

        public string SelectedInputLayer => ViewModel.SelectedInputLayer;

        public string SelectedOutputLayer => ViewModel.SelectedOutputLayer;

        public static VisionToolSingleInputViewRuntime Attach(
            ComboBox inputLayerComboBox,
            ComboBox outputLayerComboBox,
            Border inputPreviewFrame,
            VisionToolInlinePreviewSlot inputPreview,
            Border outputPreviewFrame,
            VisionToolInlinePreviewSlot outputPreview,
            Button createOutputLayerButton,
            Button runPreviewButton,
            Button addPipelineButton,
            Action sourceLayerChanged,
            Action destinationLayerChanged,
            Action inputPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action addPipelineRequested,
            Action<VisionToolPreviewImageRole> loadPreviewImageRequested,
            Action<VisionToolPreviewImageRole> savePreviewImageRequested,
            Action refreshViewState = null,
            Action clearResultReview = null)
        {
            VisionToolSingleInputViewModel viewModel = new VisionToolSingleInputViewModel(
                sourceLayerChanged,
                destinationLayerChanged,
                inputPreviewClicked,
                outputPreviewClicked,
                createOutputLayerRequested,
                runPreviewRequested,
                addPipelineRequested,
                loadPreviewImageRequested,
                savePreviewImageRequested,
                refreshViewState,
                clearResultReview);
            VisionToolSingleInputViewBinder binder = VisionToolSingleInputViewBinder.Attach(
                viewModel,
                inputLayerComboBox,
                outputLayerComboBox,
                inputPreviewFrame,
                inputPreview,
                outputPreviewFrame,
                outputPreview,
                createOutputLayerButton,
                runPreviewButton,
                addPipelineButton);
            return new VisionToolSingleInputViewRuntime(viewModel, binder);
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayer, string selectedOutputLayer)
        {
            binder.SetLayerList(layerNames, selectedInputLayer, selectedOutputLayer);
        }

        public void SetInputPreview(Bitmap image)
        {
            binder.SetInputPreview(image);
        }

        public void SetInputPreview(Bitmap image, Action afterRefresh)
        {
            binder.SetInputPreview(image, afterRefresh);
        }

        public void SetOutputPreview(Bitmap image)
        {
            binder.SetOutputPreview(image);
        }

        public void RequestRunPreview()
        {
            ViewModel.RequestRunPreview();
        }

        public void Dispose()
        {
            binder.Dispose();
        }
    }
}
