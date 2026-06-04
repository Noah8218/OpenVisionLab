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
            FormLayerDisplay display = layers.GetOrNull(index);
            if (display == null) return;

            display.ibSource.Image = image;
            imageSpace.SetImage(index, display.Text, image);
        }

        public void AcceptImageChanged(string title, int index)
        {
            FormLayerDisplay display = layers.GetOrNull(index);
            if (display != null)
            {
                display.viewer._ImageChanged = false;
            }

            imageSpace.AcceptImageChanged(title);
        }

        public void SyncFromDisplay(int index)
        {
            FormLayerDisplay display = layers.GetOrNull(index);
            if (display == null) return;

            Bitmap image = display.viewer?._Ib?.Image as Bitmap;
            imageSpace.SetImage(index, display.Text, image);
            imageSpace.SetRoi(index, display.viewer?.Roi ?? Rectangle.Empty);
            imageSpace.SetTrainRoi(index, display.viewer?.TrainROI ?? Rectangle.Empty);
            imageSpace.MarkImageChanged(display.Text, display.viewer?._ImageChanged ?? false);
        }
    }
}
