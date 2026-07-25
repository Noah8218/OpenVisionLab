using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public class VisionToolSingleInputCustomToolViewBase : UserControl, ISingleInputVisionToolWpfView, IVisionToolPreviewImageCommands, IVisionToolViewLifetime
    {
        private VisionToolSingleInputCustomToolController toolController;
        private bool disposed;

        public event EventHandler SourceLayerChanged
        {
            add { ToolController.SourceLayerChanged += value; }
            remove { ToolController.SourceLayerChanged -= value; }
        }

        public event EventHandler DestinationLayerChanged
        {
            add { ToolController.DestinationLayerChanged += value; }
            remove { ToolController.DestinationLayerChanged -= value; }
        }

        public event EventHandler InputPreviewClicked
        {
            add { ToolController.InputPreviewClicked += value; }
            remove { ToolController.InputPreviewClicked -= value; }
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

        public string SelectedInputLayer => ToolController.SelectedInputLayer;

        public string SelectedOutputLayer => ToolController.SelectedOutputLayer;

        public string ResultReviewTextForTest => toolController?.ResultReviewText ?? string.Empty;

        internal VisionToolSingleInputCustomToolController ToolController
        {
            get
            {
                return toolController ?? throw new InvalidOperationException($"{GetType().Name} has not attached its tool controller.");
            }
        }

        protected bool HasToolController => toolController != null;

        protected void AttachToolController(
            string titleLocalizationKey,
            FrameworkElement parameterContent,
            Action refreshViewState = null,
            Action clearResultReview = null,
            Action applyToolLocalization = null)
        {
            if (toolController != null)
            {
                throw new InvalidOperationException($"{GetType().Name} already attached its tool controller.");
            }

            toolController = VisionToolSingleInputCustomToolController.Attach(
                this,
                titleLocalizationKey,
                parameterContent,
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

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayer, string selectedOutputLayer)
        {
            ToolController.SetLayerList(layerNames, selectedInputLayer, selectedOutputLayer);
        }

        public void SetInputPreview(Bitmap image)
        {
            ToolController.SetInputPreview(image);
        }

        public void SetOutputPreview(Bitmap image)
        {
            ToolController.SetOutputPreview(image);
        }

        public void SetStatus(string status)
        {
            ToolController.SetStatus(status);
        }

        public void SetAddPipelineVisible(bool visible)
        {
            ToolController.SetAddPipelineVisible(visible);
        }

        public void ClearResultReview()
        {
            toolController?.ClearResultReview();
        }

        protected void RequestRunPreview()
        {
            toolController?.RequestRunPreview();
        }

        internal void ShowToolResultReview(
            string summary,
            bool isSuccess,
            IEnumerable<VisionToolResultReviewItem> items,
            string guidance)
        {
            ToolController.ShowResultReview(summary, isSuccess, items, guidance);
        }

        protected virtual void DisposeToolResources()
        {
        }
    }
}
