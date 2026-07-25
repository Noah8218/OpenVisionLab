using OpenVisionLab.History;

namespace OpenVisionLab.Core
{
    internal sealed class DisplayLayerImageSnapshot
    {
        public int Index { get; set; }
        public string Title { get; set; }
        public BitmapHistorySnapshot Image { get; set; }
    }
}

