using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public class VisionToolSingleInputPropertyToolViewBase : UserControl, ISingleInputVisionToolWpfView, IVisionToolPreviewImageCommands, IVisionToolViewLifetime
    {
        private IVisionToolSingleInputPropertyToolController toolController;
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

        private IVisionToolSingleInputPropertyToolController ToolController
        {
            get
            {
                return toolController ?? throw new InvalidOperationException($"{GetType().Name} has not attached its tool controller.");
            }
        }

        internal void AttachPropertyToolController(IVisionToolSingleInputPropertyToolController controller)
        {
            if (toolController != null)
            {
                throw new InvalidOperationException($"{GetType().Name} already attached its tool controller.");
            }

            toolController = controller ?? throw new ArgumentNullException(nameof(controller));
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

        public virtual void SetInputPreview(Bitmap image)
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

        protected virtual void DisposeToolResources()
        {
        }
    }

    internal interface IVisionToolSingleInputPropertyToolController : IDisposable
    {
        event EventHandler SourceLayerChanged;
        event EventHandler DestinationLayerChanged;
        event EventHandler InputPreviewClicked;
        event EventHandler OutputPreviewClicked;
        event EventHandler CreateOutputLayerRequested;
        event EventHandler RunPreviewRequested;
        event EventHandler AddPipelineRequested;
        event EventHandler<VisionToolPreviewImageCommandEventArgs> LoadPreviewImageRequested;
        event EventHandler<VisionToolPreviewImageCommandEventArgs> SavePreviewImageRequested;

        string SelectedInputLayer { get; }
        string SelectedOutputLayer { get; }

        void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayer, string selectedOutputLayer);
        void SetInputPreview(Bitmap image);
        void SetOutputPreview(Bitmap image);
        void SetStatus(string status);
    }
}
