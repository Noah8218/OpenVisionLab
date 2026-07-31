using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;
using OpenVisionLab.Mvvm;

namespace OpenVisionLab
{
    internal sealed class VisionToolDoubleInputViewBinder : IDisposable
    {
        private readonly VisionToolDoubleInputViewModel viewModel;
        private readonly Border inputAPreviewFrame;
        private readonly VisionToolInlinePreviewSlot inputAPreview;
        private readonly Border inputBPreviewFrame;
        private readonly VisionToolInlinePreviewSlot inputBPreview;
        private readonly Border outputPreviewFrame;
        private readonly VisionToolInlinePreviewSlot outputPreview;
        private readonly Button loadInputAImageButton;
        private readonly Button loadInputBImageButton;
        private readonly VisionToolLayerSelectionBehavior layerSelectionBehavior;
        private readonly VisionToolActionBehavior actionBehavior;
        private bool disposed;

        private VisionToolDoubleInputViewBinder(
            VisionToolDoubleInputViewModel viewModel,
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
            Func<bool> useOffsetMode)
        {
            this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.inputAPreviewFrame = inputAPreviewFrame ?? throw new ArgumentNullException(nameof(inputAPreviewFrame));
            this.inputAPreview = inputAPreview ?? throw new ArgumentNullException(nameof(inputAPreview));
            this.inputBPreviewFrame = inputBPreviewFrame ?? throw new ArgumentNullException(nameof(inputBPreviewFrame));
            this.inputBPreview = inputBPreview ?? throw new ArgumentNullException(nameof(inputBPreview));
            this.outputPreviewFrame = outputPreviewFrame ?? throw new ArgumentNullException(nameof(outputPreviewFrame));
            this.outputPreview = outputPreview ?? throw new ArgumentNullException(nameof(outputPreview));
            this.loadInputAImageButton = loadInputAImageButton;
            this.loadInputBImageButton = loadInputBImageButton;

            layerSelectionBehavior = VisionToolLayerSelectionBehavior.AttachDual(
                inputAComboBox,
                inputBComboBox,
                outputComboBox,
                () => viewModel.NotifyInputALayerChanged(VisionToolLayerComboHelper.GetLayerText(inputAComboBox)),
                () => viewModel.NotifyInputBLayerChanged(VisionToolLayerComboHelper.GetLayerText(inputBComboBox)),
                () => viewModel.NotifyOutputLayerChanged(VisionToolLayerComboHelper.GetLayerText(outputComboBox)));

            actionBehavior = VisionToolActionBehavior.AttachArithmetic(
                inputAPreviewFrame,
                inputAPreview,
                inputBPreviewFrame,
                inputBPreview,
                outputPreviewFrame,
                outputPreview,
                createOutputLayerButton,
                runPreviewButton,
                addPipelineButton,
                runOffsetButton,
                Execute(viewModel.InputAPreviewClickCommand),
                Execute(viewModel.InputBPreviewClickCommand),
                Execute(viewModel.OutputPreviewClickCommand),
                Execute(viewModel.CreateOutputLayerCommand),
                Execute(viewModel.RunPreviewCommand),
                Execute(viewModel.AddPipelineCommand),
                Execute(viewModel.RunOffsetCommand),
                useOffsetMode);

            VisionToolPreviewSlotBehavior.AttachArithmetic(
                inputAPreviewFrame,
                inputAPreview,
                inputBPreviewFrame,
                inputBPreview,
                outputPreviewFrame,
                outputPreview,
                this,
                OnLoadPreviewImageRequested,
                OnSavePreviewImageRequested);

            // Keep visible load buttons on the same role-based command path as the preview context menu.
            BindLoadButton(loadInputAImageButton, VisionToolPreviewImageRole.InputA);
            BindLoadButton(loadInputBImageButton, VisionToolPreviewImageRole.InputB);
        }

        public static VisionToolDoubleInputViewBinder Attach(
            VisionToolDoubleInputViewModel viewModel,
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
            Func<bool> useOffsetMode)
        {
            return new VisionToolDoubleInputViewBinder(
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
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayerA, string selectedInputLayerB, string selectedOutputLayer)
        {
            layerSelectionBehavior.ApplyDual(layerNames, selectedInputLayerA, selectedInputLayerB, selectedOutputLayer);
            viewModel.ApplyLayerSelection(selectedInputLayerA, selectedInputLayerB, selectedOutputLayer);
        }

        public void SetInputAPreview(Bitmap image)
        {
            VisionToolPreviewStatePresenter.SetImage(inputAPreview, inputAPreviewFrame, image);
        }

        public void SetInputBPreview(Bitmap image)
        {
            VisionToolPreviewStatePresenter.SetImage(inputBPreview, inputBPreviewFrame, image);
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
            VisionToolPreviewSlotBehavior.Detach(inputAPreviewFrame);
            VisionToolPreviewSlotBehavior.Detach(inputBPreviewFrame);
            VisionToolPreviewSlotBehavior.Detach(outputPreviewFrame);
            ClearButton(loadInputAImageButton);
            ClearButton(loadInputBImageButton);
            layerSelectionBehavior.Dispose();
            actionBehavior.Dispose();
            inputAPreview.DisposeView();
            inputBPreview.DisposeView();
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

        private void BindLoadButton(Button button, VisionToolPreviewImageRole role)
        {
            if (button != null)
            {
                button.Command = new RelayCommand(() => viewModel.RequestLoadPreviewImage(role));
            }
        }

        private static void ClearButton(Button button)
        {
            if (button != null)
            {
                button.Command = null;
            }
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
