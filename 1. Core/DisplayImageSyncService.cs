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

			display.SetImage(image);
			imageSpace.SetImage(index, display.Text, display.GetCurrentImage());
        }

        public void AcceptImageChanged(string title, int index)
        {
            FormLayerDisplay display = layers.GetOrNull(index);
            if (display != null)
            {
                display.AcceptImageChanged();
            }

            imageSpace.AcceptImageChanged(title);
        }

        public void SyncFromDisplay(int index)
        {
            FormLayerDisplay display = layers.GetOrNull(index);
            if (display == null) return;

            Bitmap image = display.GetCurrentImage();
            imageSpace.SetImage(index, display.Text, image);
            imageSpace.SetRoi(index, display.Roi);
            imageSpace.SetTrainRoi(index, display.TrainROI);
            imageSpace.MarkImageChanged(display.Text, display.ImageChanged);
        }
    }
}
