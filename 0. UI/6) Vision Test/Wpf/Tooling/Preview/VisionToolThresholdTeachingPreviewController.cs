using System;

namespace OpenVisionLab
{
    internal sealed class VisionToolThresholdTeachingPreviewController
    {
        private readonly Action clearResultReview;
        private bool requested;

        public VisionToolThresholdTeachingPreviewController(Action clearResultReview)
        {
            this.clearResultReview = clearResultReview ?? throw new ArgumentNullException(nameof(clearResultReview));
        }

        public bool ConsumeRequest()
        {
            bool current = requested;
            requested = false;
            return current;
        }

        public void Request()
        {
            clearResultReview();
            requested = true;
        }
    }
}
