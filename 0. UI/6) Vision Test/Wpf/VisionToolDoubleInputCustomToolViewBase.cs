using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public class VisionToolDoubleInputCustomToolViewBase : UserControl, IVisionToolPreviewImageCommands, IVisionToolViewLifetime
    {
        private VisionToolDoubleInputCustomToolController toolController;
        private bool disposed;

        public event EventHandler InputALayerChanged
        {
            add { ToolController.InputALayerChanged += value; }
            remove { ToolController.InputALayerChanged -= value; }
        }

        public event EventHandler InputBLayerChanged
        {
            add { ToolController.InputBLayerChanged += value; }
            remove { ToolController.InputBLayerChanged -= value; }
        }

        public event EventHandler OutputLayerChanged
        {
            add { ToolController.OutputLayerChanged += value; }
            remove { ToolController.OutputLayerChanged -= value; }
        }

        public event EventHandler InputAPreviewClicked
        {
            add { ToolController.InputAPreviewClicked += value; }
            remove { ToolController.InputAPreviewClicked -= value; }
        }

        public event EventHandler InputBPreviewClicked
        {
            add { ToolController.InputBPreviewClicked += value; }
            remove { ToolController.InputBPreviewClicked -= value; }
        }

        public event EventHandler OutputPreviewClicked
        {
            add { ToolController.OutputPreviewClicked += value; }
            remove { ToolController.OutputPreviewClicked -= value; }
        }

        public event EventHandler CreateOutputLayerRequested
        {
            add { ToolController.CreateOutputLayerRequested += value; }
            remove { ToolController.CreateOutputLayerRequested -= value; }
        }

        public event EventHandler RunPreviewRequested
        {
            add { ToolController.RunPreviewRequested += value; }
            remove { ToolController.RunPreviewRequested -= value; }
        }

        public event EventHandler RunOffsetRequested
        {
            add { ToolController.RunOffsetRequested += value; }
            remove { ToolController.RunOffsetRequested -= value; }
        }

        public event EventHandler AddPipelineRequested
        {
            add { ToolController.AddPipelineRequested += value; }
            remove { ToolController.AddPipelineRequested -= value; }
        }

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> LoadPreviewImageRequested
        {
            add { ToolController.LoadPreviewImageRequested += value; }
            remove { ToolController.LoadPreviewImageRequested -= value; }
        }

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> SavePreviewImageRequested
        {
            add { ToolController.SavePreviewImageRequested += value; }
            remove { ToolController.SavePreviewImageRequested -= value; }
        }

        public string SelectedInputLayerA => ToolController.SelectedInputLayerA;

        public string SelectedInputLayerB => ToolController.SelectedInputLayerB;

        public string SelectedOutputLayer => ToolController.SelectedOutputLayer;

        internal VisionToolDoubleInputCustomToolController ToolController
        {
            get
            {
                return toolController ?? throw new InvalidOperationException($"{GetType().Name} has not attached its tool controller.");
            }
        }

        protected void AttachToolController(
            string titleLocalizationKey,
            FrameworkElement parameterContent,
            Func<bool> useOffsetMode,
            Action refreshViewState = null,
            Action clearResultReview = null,
            Action applyToolLocalization = null)
        {
            if (toolController != null)
            {
                throw new InvalidOperationException($"{GetType().Name} already attached its tool controller.");
            }

            toolController = VisionToolDoubleInputCustomToolController.Attach(
                this,
                titleLocalizationKey,
                parameterContent,
                useOffsetMode,
                refreshViewState,
                clearResultReview,
                applyToolLocalization);
        }

        public virtual void DisposeView()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            DisposeToolResources();
            toolController?.Dispose();
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputA, string selectedInputB, string selectedOutput)
        {
            ToolController.SetLayerList(layerNames, selectedInputA, selectedInputB, selectedOutput);
        }

        public void SetInputAPreview(Bitmap image)
        {
            ToolController.SetInputAPreview(image);
        }

        public void SetInputBPreview(Bitmap image)
        {
            ToolController.SetInputBPreview(image);
        }

        public void SetOutputPreview(Bitmap image)
        {
            ToolController.SetOutputPreview(image);
        }

        public void SetStatus(string status)
        {
            ToolController.SetStatus(status);
        }

        protected virtual void DisposeToolResources()
        {
        }
    }
}
