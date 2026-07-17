using OpenVisionLab.History;

namespace OpenVisionLab._1._Core
{
    internal sealed class DisplayLayerImageSnapshot
    {
        public int Index { get; set; }
        public string Title { get; set; }
        public BitmapHistorySnapshot Image { get; set; }
    }
}

