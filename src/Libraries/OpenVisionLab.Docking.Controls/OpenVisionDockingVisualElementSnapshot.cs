using System.Windows;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockingVisualElementSnapshot
    {
        public OpenVisionDockingVisualElementSnapshot(
            string kind,
            string title,
            Rect bounds,
            int paneIndex)
        {
            Kind = kind ?? string.Empty;
            Title = title ?? string.Empty;
            Bounds = bounds;
            PaneIndex = paneIndex;
        }

        public string Kind { get; }

        public string Title { get; }

        public Rect Bounds { get; }

        public int PaneIndex { get; }

        public string ToReportLine()
        {
            return string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "{0}:{1}:pane={2}:x={3:0.0},y={4:0.0},w={5:0.0},h={6:0.0}",
                Kind,
                Title,
                PaneIndex,
                Bounds.X,
                Bounds.Y,
                Bounds.Width,
                Bounds.Height);
        }
    }
}
