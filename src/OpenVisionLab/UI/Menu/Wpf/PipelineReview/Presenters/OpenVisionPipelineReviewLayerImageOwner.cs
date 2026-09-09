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

        internal OpenVisionPipelineReviewLayerImageOwner(
            IDisplayManager displayManager,
            Func<string, Bitmap> acquireCachedOutputSnapshot)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
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
    }
}
