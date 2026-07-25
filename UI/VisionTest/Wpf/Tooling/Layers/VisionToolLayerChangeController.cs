using System;

namespace OpenVisionLab
{
    internal sealed class VisionToolLayerChangeController
    {
        private readonly Action refreshViewState;
        private readonly Action clearResultReview;

        public VisionToolLayerChangeController(Action refreshViewState = null, Action clearResultReview = null)
        {
            this.refreshViewState = refreshViewState;
            this.clearResultReview = clearResultReview;
        }

        public void NotifyInput(Action layerChanged)
        {
            NotifyLayerChanged(layerChanged);
        }

        public void NotifyInputB(Action layerChanged)
        {
            NotifyLayerChanged(layerChanged);
        }

        public void NotifyOutput(Action layerChanged)
        {
            NotifyLayerChanged(layerChanged);
        }

        private void NotifyLayerChanged(Action layerChanged)
        {
            // Layer changes must invalidate stale results before routing the new selection to the host document.
            clearResultReview?.Invoke();
            layerChanged?.Invoke();
            refreshViewState?.Invoke();
        }
    }
}