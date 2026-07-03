using System.Drawing;

namespace OpenVisionLab.ImageSpace.Core
{
    public sealed class ImageSpaceFrame
    {
        public ImageSpaceFrame(Bitmap image)
        {
            Image = image;
        }

        public Bitmap Image { get; }

        public static ImageSpaceFrame FromBitmap(Bitmap image)
        {
            return image == null ? null : new ImageSpaceFrame(image);
        }
    }
}
