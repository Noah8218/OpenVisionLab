using OpenVisionLab.ImageSpace.Core;
using System.Drawing;

namespace OpenVisionLab._1._Core
{
    internal sealed class DisplayImageSyncService
    {
        private readonly IImageSpace imageSpace;
        private readonly DisplayLayerStore layers;

        public DisplayImageSyncService(IImageSpace imageSpace, DisplayLayerStore layers)
        {
            this.imageSpace = imageSpace;
            this.layers = layers;
        }

        public void SetImage(int index, Bitmap image)
        {
            string title = layers.GetTitle(index);
            if (string.IsNullOrWhiteSpace(title))
            {
                return;
            }

            imageSpace.SetImage(index, title, CloneBitmap(image));
            imageSpace.MarkImageChanged(title, true);
        }

        public void AcceptImageChanged(string title, int index)
        {
            imageSpace.AcceptImageChanged(title);
        }

        private static Bitmap CloneBitmap(Bitmap image)
        {
            if (image == null)
            {
                return null;
            }

            try
            {
                return image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            }
            catch
            {
                return new Bitmap(image);
            }
        }
    }
}
