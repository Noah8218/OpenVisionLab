using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenVisionLab
{
    internal sealed class VisionToolSingleInputViewBinder : IDisposable
    {
        private readonly VisionToolSingleInputViewModel viewModel;
        private readonly ComboBox inputLayerComboBox;
        private readonly ComboBox outputLayerComboBox;
        private readonly Border inputPreviewFrame;
        private readonly VisionToolInlinePreviewSlot inputPreview;
        private readonly Border outputPreviewFrame;
        private readonly VisionToolInlinePreviewSlot outputPreview;
        private readonly VisionToolLayerSelectionBehavior layerSelectionBehavior;
        private readonly VisionToolActionBehavior actionBehavior;
        private bool disposed;

        private VisionToolSingleInputViewBinder(
            VisionToolSingleInputViewModel viewModel,
            ComboBox inputLayerComboBox,
            ComboBox outputLayerComboBox,
            Border inputPreviewFrame,
            VisionToolInlinePreviewSlot inputPreview,
            Border outputPreviewFrame,
            VisionToolInlinePreviewSlot outputPreview,
            Button createOutputLayerButton,
            Button runPreviewButton,
            Button addPipelineButton)
        {
            this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.inputLayerComboBox = inputLayerComboBox ?? throw new ArgumentNullException(nameof(inputLayerComboBox));
            this.outputLayerComboBox = outputLayerComboBox ?? throw new ArgumentNullException(nameof(outputLayerComboBox));
            this.inputPreviewFrame = inputPreviewFrame ?? throw new ArgumentNullException(nameof(inputPreviewFrame));
            this.inputPreview = inputPreview ?? throw new ArgumentNullException(nameof(inputPreview));
            this.outputPreviewFrame = outputPreviewFrame ?? throw new ArgumentNullException(nameof(outputPreviewFrame));
            this.outputPreview = outputPreview ?? throw new ArgumentNullException(nameof(outputPreview));

            layerSelectionBehavior = VisionToolLayerSelectionBehavior.AttachSingle(
                inputLayerComboBox,
                outputLayerComboBox,
                () => viewModel.NotifyInputLayerChanged(VisionToolLayerComboHelper.GetLayerText(inputLayerComboBox)),
                () => viewModel.NotifyOutputLayerChanged(VisionToolLayerComboHelper.GetLayerText(outputLayerComboBox)));

            actionBehavior = VisionToolActionBehavior.AttachSingle(
                inputPreviewFrame,
                inputPreview,
                outputPreviewFrame,
                outputPreview,
                createOutputLayerButton,
                runPreviewButton,
                addPipelineButton,
                Execute(viewModel.InputPreviewClickCommand),
                Execute(viewModel.OutputPreviewClickCommand),
                Execute(viewModel.CreateOutputLayerCommand),
                Execute(viewModel.RunPreviewCommand),
                Execute(viewModel.AddPipelineCommand));

            VisionToolPreviewSlotBehavior.AttachSingle(
                inputPreviewFrame,
                inputPreview,
                outputPreviewFrame,
                outputPreview,
                this,
                OnLoadPreviewImageRequested,
                OnSavePreviewImageRequested);
        }

        public static VisionToolSingleInputViewBinder Attach(
            VisionToolSingleInputViewModel viewModel,
            ComboBox inputLayerComboBox,
            ComboBox outputLayerComboBox,
            Border inputPreviewFrame,
            VisionToolInlinePreviewSlot inputPreview,
            Border outputPreviewFrame,
            VisionToolInlinePreviewSlot outputPreview,
            Button createOutputLayerButton,
            Button runPreviewButton,
            Button addPipelineButton)
        {
            return new VisionToolSingleInputViewBinder(
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
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayer, string selectedOutputLayer)
        {
            layerSelectionBehavior.ApplySingle(layerNames, selectedInputLayer, selectedOutputLayer);
            viewModel.ApplyLayerSelection(selectedInputLayer, selectedOutputLayer);
        }

        public void SetInputPreview(Bitmap image)
        {
            VisionToolPreviewStatePresenter.SetImage(inputPreview, inputPreviewFrame, image);
        }

        public void SetInputPreview(Bitmap image, Action afterRefresh)
        {
            VisionToolPreviewStatePresenter.SetImage(inputPreview, inputPreviewFrame, image, afterRefresh);
        }

        public void SetOutputPreview(Bitmap image)
        {
            VisionToolPreviewStatePresenter.SetImage(outputPreview, outputPreviewFrame, image);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            VisionToolPreviewSlotBehavior.Detach(inputPreviewFrame);
            VisionToolPreviewSlotBehavior.Detach(outputPreviewFrame);
            layerSelectionBehavior.Dispose();
            actionBehavior.Dispose();
            inputPreview.DisposeView();
            outputPreview.DisposeView();
        }

        private static Action Execute(ICommand command)
        {
            return () =>
            {
                if (command?.CanExecute(null) == true)
                {
                    command.Execute(null);
                }
            };
        }

        private void OnLoadPreviewImageRequested(object sender, VisionToolPreviewImageCommandEventArgs e)
        {
            viewModel.RequestLoadPreviewImage(e.Role);
        }

        private void OnSavePreviewImageRequested(object sender, VisionToolPreviewImageCommandEventArgs e)
        {
            viewModel.RequestSavePreviewImage(e.Role);
        }
    }
}
