namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockDocumentLayoutEntry
    {
        public OpenVisionDockDocumentLayoutEntry(string layerTitle, int paneIndex, string layoutPath)
        {
            LayerTitle = layerTitle ?? string.Empty;
            PaneIndex = paneIndex;
            LayoutPath = layoutPath ?? string.Empty;
        }

        public string LayerTitle { get; }

        public int PaneIndex { get; }

        public string LayoutPath { get; }
    }
}
