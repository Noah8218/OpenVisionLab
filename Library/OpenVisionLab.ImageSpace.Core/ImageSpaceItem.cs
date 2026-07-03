using System.Drawing;

namespace OpenVisionLab.ImageSpace.Core
{
    internal sealed class ImageSpaceItem
    {
        public string Title { get; set; } = string.Empty;
        public Bitmap Image { get; set; }
        public Rectangle Roi { get; set; } = Rectangle.Empty;
        public Rectangle TrainRoi { get; set; } = Rectangle.Empty;
        public bool ImageChanged { get; set; }
    }
}
