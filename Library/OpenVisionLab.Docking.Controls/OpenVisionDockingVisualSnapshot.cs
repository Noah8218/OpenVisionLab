using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockingVisualSnapshot
    {
        public OpenVisionDockingVisualSnapshot(
            Size workspaceSize,
            IReadOnlyList<OpenVisionDockingVisualElementSnapshot> panes,
            IReadOnlyList<OpenVisionDockingVisualElementSnapshot> tabHeaders,
            IReadOnlyList<OpenVisionDockingVisualElementSnapshot> paneHeaders)
        {
            WorkspaceSize = workspaceSize;
            Panes = panes ?? new List<OpenVisionDockingVisualElementSnapshot>();
            TabHeaders = tabHeaders ?? new List<OpenVisionDockingVisualElementSnapshot>();
            PaneHeaders = paneHeaders ?? new List<OpenVisionDockingVisualElementSnapshot>();
        }

        public static OpenVisionDockingVisualSnapshot Empty { get; } =
            new OpenVisionDockingVisualSnapshot(
                Size.Empty,
                new List<OpenVisionDockingVisualElementSnapshot>(),
                new List<OpenVisionDockingVisualElementSnapshot>(),
                new List<OpenVisionDockingVisualElementSnapshot>());

        public Size WorkspaceSize { get; }

        public IReadOnlyList<OpenVisionDockingVisualElementSnapshot> Panes { get; }

        public IReadOnlyList<OpenVisionDockingVisualElementSnapshot> TabHeaders { get; }

        public IReadOnlyList<OpenVisionDockingVisualElementSnapshot> PaneHeaders { get; }

        public IEnumerable<OpenVisionDockingVisualElementSnapshot> Headers =>
            TabHeaders.Concat(PaneHeaders);

        public string ToReport()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(
                string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "Workspace: {0:0.0}x{1:0.0}",
                    WorkspaceSize.Width,
                    WorkspaceSize.Height));
            builder.AppendLine("Panes: " + Panes.Count.ToString(System.Globalization.CultureInfo.InvariantCulture));
            foreach (OpenVisionDockingVisualElementSnapshot pane in Panes)
            {
                builder.AppendLine("  " + pane.ToReportLine());
            }

            builder.AppendLine("TabHeaders: " + TabHeaders.Count.ToString(System.Globalization.CultureInfo.InvariantCulture));
            foreach (OpenVisionDockingVisualElementSnapshot header in TabHeaders)
            {
                builder.AppendLine("  " + header.ToReportLine());
            }

            builder.AppendLine("PaneHeaders: " + PaneHeaders.Count.ToString(System.Globalization.CultureInfo.InvariantCulture));
            foreach (OpenVisionDockingVisualElementSnapshot header in PaneHeaders)
            {
                builder.AppendLine("  " + header.ToReportLine());
            }

            return builder.ToString();
        }
    }
}
