namespace OpenVisionLab._1._Core
{
    public sealed class DisplayLayerInfo
    {
        public DisplayLayerInfo(int index, string title)
        {
            Index = index;
            Title = title;
        }

        public int Index { get; }
        public string Title { get; }
    }
}
