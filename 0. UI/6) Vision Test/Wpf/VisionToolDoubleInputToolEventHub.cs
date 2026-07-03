using System;

namespace OpenVisionLab
{
    // Keeps double-input tool event sender and preview-role payload creation consistent.
    internal sealed class VisionToolDoubleInputToolEventHub
    {
        private readonly object sender;

        public VisionToolDoubleInputToolEventHub(object sender)
        {
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public event EventHandler InputALayerChanged = delegate { };
        public event EventHandler InputBLayerChanged = delegate { };
        public event EventHandler OutputLayerChanged = delegate { };
        public event EventHandler InputAPreviewClicked = delegate { };
        public event EventHandler InputBPreviewClicked = delegate { };
        public event EventHandler OutputPreviewClicked = delegate { };
        public event EventHandler CreateOutputLayerRequested = delegate { };
        public event EventHandler RunPreviewRequested = delegate { };
        public event EventHandler RunOffsetRequested = delegate { };
        public event EventHandler AddPipelineRequested = delegate { };
        public event EventHandler<VisionToolPreviewImageCommandEventArgs> LoadPreviewImageRequested = delegate { };
        public event EventHandler<VisionToolPreviewImageCommandEventArgs> SavePreviewImageRequested = delegate { };

        public void RaiseInputALayerChanged()
        {
            InputALayerChanged(sender, EventArgs.Empty);
        }

        public void RaiseInputBLayerChanged()
        {
            InputBLayerChanged(sender, EventArgs.Empty);
        }

        public void RaiseOutputLayerChanged()
        {
            OutputLayerChanged(sender, EventArgs.Empty);
        }

        public void RaiseInputAPreviewClicked()
        {
            InputAPreviewClicked(sender, EventArgs.Empty);
        }

        public void RaiseInputBPreviewClicked()
        {
            InputBPreviewClicked(sender, EventArgs.Empty);
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

        public void RaiseRunOffsetRequested()
        {
            RunOffsetRequested(sender, EventArgs.Empty);
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
