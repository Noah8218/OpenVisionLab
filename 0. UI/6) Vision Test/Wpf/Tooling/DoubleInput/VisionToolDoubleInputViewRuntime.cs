using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class VisionToolDoubleInputViewRuntime : IDisposable
    {
        private readonly VisionToolDoubleInputViewBinder binder;

        private VisionToolDoubleInputViewRuntime(
            VisionToolDoubleInputViewModel viewModel,
            VisionToolDoubleInputViewBinder binder)
        {
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.binder = binder ?? throw new ArgumentNullException(nameof(binder));
        }

        public VisionToolDoubleInputViewModel ViewModel { get; }

        public string SelectedInputLayerA => ViewModel.SelectedInputLayerA;

        public string SelectedInputLayerB => ViewModel.SelectedInputLayerB;

        public string SelectedOutputLayer => ViewModel.SelectedOutputLayer;

        public static VisionToolDoubleInputViewRuntime Attach(
            ComboBox inputAComboBox,
            ComboBox inputBComboBox,
            ComboBox outputComboBox,
            Button loadInputAImageButton,
            Button loadInputBImageButton,
            Border inputAPreviewFrame,
            VisionToolInlinePreviewSlot inputAPreview,
            Border inputBPreviewFrame,
            VisionToolInlinePreviewSlot inputBPreview,
            Border outputPreviewFrame,
            VisionToolInlinePreviewSlot outputPreview,
            Button createOutputLayerButton,
            Button runPreviewButton,
            Button runOffsetButton,
            Button addPipelineButton,
            Action inputALayerChanged,
            Action inputBLayerChanged,
            Action outputLayerChanged,
            Action inputAPreviewClicked,
            Action inputBPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action runOffsetRequested,
            Action addPipelineRequested,
            Action<VisionToolPreviewImageRole> loadPreviewImageRequested,
            Action<VisionToolPreviewImageRole> savePreviewImageRequested,
            Func<bool> useOffsetMode,
            Action refreshViewState = null,
            Action clearResultReview = null)
        {
            VisionToolDoubleInputViewModel viewModel = new VisionToolDoubleInputViewModel(
                inputALayerChanged,
                inputBLayerChanged,
                outputLayerChanged,
                inputAPreviewClicked,
                inputBPreviewClicked,
                outputPreviewClicked,
                createOutputLayerRequested,
                runPreviewRequested,
                runOffsetRequested,
                addPipelineRequested,
                loadPreviewImageRequested,
                savePreviewImageRequested,
                refreshViewState,
                clearResultReview);
            VisionToolDoubleInputViewBinder binder = VisionToolDoubleInputViewBinder.Attach(
                viewModel,
                inputAComboBox,
                inputBComboBox,
                outputComboBox,
                loadInputAImageButton,
                loadInputBImageButton,
                inputAPreviewFrame,
                inputAPreview,
                inputBPreviewFrame,
                inputBPreview,
                outputPreviewFrame,
                outputPreview,
                createOutputLayerButton,
                runPreviewButton,
                runOffsetButton,
                addPipelineButton,
                useOffsetMode);
            return new VisionToolDoubleInputViewRuntime(viewModel, binder);
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayerA, string selectedInputLayerB, string selectedOutputLayer)
        {
            binder.SetLayerList(layerNames, selectedInputLayerA, selectedInputLayerB, selectedOutputLayer);
        }

        public void SetInputAPreview(Bitmap image)
        {
            binder.SetInputAPreview(image);
        }

        public void SetInputBPreview(Bitmap image)
        {
            binder.SetInputBPreview(image);
        }

        public void SetOutputPreview(Bitmap image)
        {
            binder.SetOutputPreview(image);
        }

        public void Dispose()
        {
            binder.Dispose();
        }
    }
}
