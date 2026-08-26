using System;
using System.Drawing;

namespace OpenVisionLab.ImageSpace.Core
{
    /// <summary>
    /// Carries one Bitmap across the synchronous display boundary and becomes invalid when disposed.
    /// </summary>
    public sealed class ImageSpaceFrame : IDisposable
    {
        private readonly bool ownsImage;
        private Bitmap image;

        private ImageSpaceFrame(Bitmap image, bool ownsImage)
        {
            this.image = image ?? throw new ArgumentNullException(nameof(image));
            this.ownsImage = ownsImage;
        }

        public Bitmap Image => image ?? throw new ObjectDisposedException(nameof(ImageSpaceFrame));

        /// <summary>Wraps a caller-owned Bitmap without disposing that Bitmap.</summary>
        public static ImageSpaceFrame Borrow(Bitmap image)
        {
            return image == null ? null : new ImageSpaceFrame(image, false);
        }

        /// <summary>Transfers Bitmap ownership to the frame; disposing the frame disposes the Bitmap.</summary>
        public static ImageSpaceFrame TakeOwnership(Bitmap image)
        {
            return image == null ? null : new ImageSpaceFrame(image, true);
        }

        public void Dispose()
        {
            Bitmap currentImage = image;
            image = null;
            if (ownsImage)
            {
                currentImage?.Dispose();
            }
        }
    }
}
