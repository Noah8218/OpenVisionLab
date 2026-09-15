using OpenVisionLab.Core;
using System;
using System.Drawing;

namespace OpenVisionLab
{
    // Owns short-lived, lease-backed layer snapshots for Pipeline Review consumers.
    internal sealed class OpenVisionPipelineReviewLayerImageOwner
    {
        private readonly IDisplayManager displayManager;
        private readonly Func<string, Bitmap> acquireCachedOutputSnapshot;
        private readonly Func<int, string, Bitmap> acquireCachedStepOutputSnapshot;

        internal OpenVisionPipelineReviewLayerImageOwner(
            IDisplayManager displayManager,
            Func<string, Bitmap> acquireCachedOutputSnapshot)
            : this(displayManager, null, acquireCachedOutputSnapshot)
        {
        }

        internal OpenVisionPipelineReviewLayerImageOwner(
            IDisplayManager displayManager,
            Func<int, string, Bitmap> acquireCachedStepOutputSnapshot,
            Func<string, Bitmap> acquireCachedOutputSnapshot)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.acquireCachedStepOutputSnapshot = acquireCachedStepOutputSnapshot;
            this.acquireCachedOutputSnapshot = acquireCachedOutputSnapshot;
        }

        internal Bitmap AcquirePreview(string layerName)
        {
            if (string.IsNullOrWhiteSpace(layerName))
            {
                return null;
            }

            Bitmap displaySnapshot = displayManager.GetLayerImageSnapshot(layerName);
            if (displaySnapshot != null)
            {
                if (!DisplayManagerImageExtensions.IsPlaceholderBitmap(displaySnapshot))
                {
                    return displaySnapshot;
                }

                displaySnapshot.Dispose();
            }

            return CloneCachedOutput(layerName);
        }

        internal Bitmap AcquireOutputPreview(string layerName)
        {
            Bitmap cachedSnapshot = CloneCachedOutput(layerName);
            return cachedSnapshot ?? AcquirePreview(layerName);
        }

        internal Bitmap AcquireOutputPreview(int stepIndex, string layerName)
        {
            Bitmap cachedSnapshot = CloneCachedOutput(stepIndex, layerName);
            return cachedSnapshot ?? AcquireOutputPreview(layerName);
        }

        internal bool HasPreview(string layerName)
        {
            using Bitmap snapshot = AcquirePreview(layerName);
            return snapshot != null;
        }

        private Bitmap CloneCachedOutput(string layerName)
        {
            using Bitmap cachedOutput = acquireCachedOutputSnapshot?.Invoke(layerName);
            if (cachedOutput == null)
            {
                return null;
            }

            try
            {
                return new Bitmap(cachedOutput);
            }
            catch
            {
                return null;
            }
        }

        private Bitmap CloneCachedOutput(int stepIndex, string layerName)
        {
            using Bitmap cachedOutput = acquireCachedStepOutputSnapshot?.Invoke(stepIndex, layerName);
            if (cachedOutput == null)
            {
                return null;
            }

            try
            {
                return new Bitmap(cachedOutput);
            }
            catch
            {
                return null;
            }
        }
    }
}
