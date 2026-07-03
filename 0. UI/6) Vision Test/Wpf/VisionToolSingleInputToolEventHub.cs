using System;

namespace OpenVisionLab
{
    // Keeps single-input tool event sender and preview-role payload creation consistent across tool controller types.
    internal sealed class VisionToolSingleInputToolEventHub
    {
        private readonly object sender;

        public VisionToolSingleInputToolEventHub(object sender)
        {
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public event EventHandler SourceLayerChanged = delegate { };
        public event EventHandler DestinationLayerChanged = delegate { };
        public event EventHandler InputPreviewClicked = delegate { };
        public event EventHandler OutputPreviewClicked = delegate { };
        public event EventHandler CreateOutputLayerRequested = delegate { };
        public event EventHandler RunPreviewRequested = delegate { };
        public event EventHandler AddPipelineRequested = delegate { };
        public event EventHandler<VisionToolPreviewImageCommandEventArgs> LoadPreviewImageRequested = delegate { };
        public event EventHandler<VisionToolPreviewImageCommandEventArgs> SavePreviewImageRequested = delegate { };

        public void RaiseSourceLayerChanged()
        {
            SourceLayerChanged(sender, EventArgs.Empty);
        }

        public void RaiseDestinationLayerChanged()
        {
            DestinationLayerChanged(sender, EventArgs.Empty);
        }

        public void RaiseInputPreviewClicked()
        {
            InputPreviewClicked(sender, EventArgs.Empty);
        }

        public void RaiseOutputPreviewClicked()
        {
            OutputPreviewClicked(sender, EventArgs.Empty);
        }

        public void RaiseCreateOutputLayerRequested()
        {
            CreateOutputLayerRequested(sender, EventArgs.Empty);
        }

        public void RaiseRunPreviewRequested()
        {
            RunPreviewRequested(sender, EventArgs.Empty);
        }

        public void RaiseAddPipelineRequested()
        {
            AddPipelineRequested(sender, EventArgs.Empty);
        }

        public void RaiseLoadPreviewImageRequested(VisionToolPreviewImageRole role)
        {
            LoadPreviewImageRequested(sender, new VisionToolPreviewImageCommandEventArgs(role));
        }

        public void RaiseSavePreviewImageRequested(VisionToolPreviewImageRole role)
        {
            SavePreviewImageRequested(sender, new VisionToolPreviewImageCommandEventArgs(role));
        }
    }
}
