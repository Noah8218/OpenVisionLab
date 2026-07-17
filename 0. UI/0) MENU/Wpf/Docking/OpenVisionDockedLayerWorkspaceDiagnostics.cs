using OpenVisionLab.Docking.Controls;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionDockedLayerWorkspaceDiagnostics
    {
        public static OpenVisionDockedLayerWorkspaceDiagnostics Empty { get; } =
            new OpenVisionDockedLayerWorkspaceDiagnostics(
                OpenVisionDockedLayerWorkspaceState.Empty,
                OpenVisionDockingHeaderDiagnostics.Empty);

        public OpenVisionDockedLayerWorkspaceDiagnostics(
            OpenVisionDockedLayerWorkspaceState workspaceState,
            OpenVisionDockingHeaderDiagnostics headerDiagnostics)
        {
            WorkspaceState = workspaceState ?? OpenVisionDockedLayerWorkspaceState.Empty;
            HeaderDiagnostics = headerDiagnostics ?? OpenVisionDockingHeaderDiagnostics.Empty;
        }

        public OpenVisionDockedLayerWorkspaceState WorkspaceState { get; }

        public OpenVisionDockingHeaderDiagnostics HeaderDiagnostics { get; }

        public int LayerCount => Documents.Count;

        public int TextureTileCount => Documents.Sum(document => document.TextureTileCount);

        public int PaneCount => WorkspaceState.PaneCount;

        public string RootOrientationName => WorkspaceState.RootOrientationName;

        public int NestedLayoutPanelCount => WorkspaceState.NestedLayoutPanelCount;

        public bool AreViewersCompactSizeReady =>
            Documents.Count > 0 && Documents.All(document => document.IsCompactSizeReady);

        public bool AreNativeFloatingDisabled =>
            Documents.Count > 0 && Documents.All(document => !document.CanFloat);

        public bool AreNativeFloatingEnabled =>
            Documents.Count > 0 && Documents.All(document => document.CanFloat);

        public bool AreViewersCompact =>
            Documents.Count > 0 && Documents.All(document => document.IsCompactChrome);

        public int TabHeaderCount => HeaderDiagnostics.HeaderCount;

        public bool AreTabHeadersGestureReady => HeaderDiagnostics.AreGestureReady;

        public bool AreTabHeadersReadable => HeaderDiagnostics.AreReadable;

        public bool AreTabHeaderGripsReady => HeaderDiagnostics.AreGripsReady;

        public string TabHeaderDiagnostics => HeaderDiagnostics.DiagnosticsText;

        public string Titles => string.Join("|", Documents
            .Select(document => document.ContentId)
            .Where(title => !string.IsNullOrWhiteSpace(title)));

        private IReadOnlyList<OpenVisionDockedLayerDocumentState> Documents => WorkspaceState.Documents;
    }
}
