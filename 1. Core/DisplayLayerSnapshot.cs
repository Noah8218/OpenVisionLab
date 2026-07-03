using OpenVisionLab.History;
using System.Drawing;

namespace OpenVisionLab._1._Core
{
    internal sealed class DisplayLayerSnapshot
    {
        public bool Exists { get; set; }
        public int Index { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool UseClose { get; set; } = true;
        public Rectangle Roi { get; set; } = Rectangle.Empty;
        public Rectangle TrainRoi { get; set; } = Rectangle.Empty;
        public BitmapHistorySnapshot Image { get; set; }

        public static DisplayLayerSnapshot Missing(int index, string title)
        {
            return new DisplayLayerSnapshot
            {
                Exists = false,
                Index = index,
                Title = title ?? string.Empty
            };
        }
    }
}
