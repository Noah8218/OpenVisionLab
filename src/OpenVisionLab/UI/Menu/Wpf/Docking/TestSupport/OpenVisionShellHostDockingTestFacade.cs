using OpenVisionLab.Docking.Controls;
using System;
using System.Windows;
using DrawingBitmap = System.Drawing.Bitmap;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostDockingTestFacade
    {
        private readonly IOpenVisionDockedLayerWorkspaceDiagnostics dockedLayerWorkspace;
        private readonly Action updateLayout;

        public OpenVisionShellHostDockingTestFacade(
            IOpenVisionDockedLayerWorkspaceDiagnostics dockedLayerWorkspace,
            Action updateLayout)
        {
            this.dockedLayerWorkspace = dockedLayerWorkspace ?? throw new ArgumentNullException(nameof(dockedLayerWorkspace));
            this.updateLayout = updateLayout ?? throw new ArgumentNullException(nameof(updateLayout));
        }

        public bool IsDockedWorkspaceVisible => dockedLayerWorkspace.IsWorkspaceVisible;

        public bool HasGuideOverlay => dockedLayerWorkspace.HasGuideOverlay;

        public bool IsGuideOverlayVisible => dockedLayerWorkspace.IsGuideOverlayVisible;

        public string ActiveGuideZone => dockedLayerWorkspace.ActiveGuideZoneName;

        public bool IsGuideOverlayHitTestSafe => dockedLayerWorkspace.IsGuideOverlayHitTestSafe;

        public int GuideZoneCount => dockedLayerWorkspace.GuideZoneCount;

        public int LayerCount => Diagnostics.LayerCount;

        public int TextureTileCount => Diagnostics.TextureTileCount;

        public int PaneCount => Diagnostics.PaneCount;

        public string RootOrientationName => Diagnostics.RootOrientationName;

        public int NestedLayoutPanelCount => Diagnostics.NestedLayoutPanelCount;

        public bool AreViewersCompactSizeReady => Diagnostics.AreViewersCompactSizeReady;

        public bool AreNativeFloatingDisabled => Diagnostics.AreNativeFloatingDisabled;

        public bool AreNativeFloatingEnabled => Diagnostics.AreNativeFloatingEnabled;

        public bool AreViewersCompact => Diagnostics.AreViewersCompact;

        public int TabHeaderCount => Diagnostics.TabHeaderCount;

        public bool AreTabHeadersGestureReady => Diagnostics.AreTabHeadersGestureReady;

        public bool AreTabHeadersReadable => Diagnostics.AreTabHeadersReadable;

        public bool AreTabHeaderGripsReady => Diagnostics.AreTabHeaderGripsReady;

        public string TabHeaderDiagnostics => Diagnostics.TabHeaderDiagnostics;

        public string Titles => Diagnostics.Titles;

        public OpenVisionDockingVisualSnapshot CreateDockingVisualSnapshot()
        {
            return dockedLayerWorkspace.CreateDockingVisualSnapshot();
        }

        public bool DockLayerDocument(string layerTitle)
        {
            return dockedLayerWorkspace.DockLayerDocument(layerTitle);
        }

        public bool SplitLayerToNewPane(string layerTitle)
        {
            return dockedLayerWorkspace.SplitLayerToNewPane(layerTitle);
        }

        public bool ArrangeLayerPanes(string orientationName, params string[] layerTitles)
        {
            return dockedLayerWorkspace.ArrangeLayerPanes(orientationName, layerTitles);
        }

        public bool ArrangeLayerGrid(params string[] layerTitles)
        {
            return dockedLayerWorkspace.ArrangeLayerGrid(layerTitles);
        }

        public bool MoveLayerToPrimaryPane(string layerTitle)
        {
            return dockedLayerWorkspace.MoveLayerToPrimaryPane(layerTitle);
        }

        public bool MoveLayerToGuideZone(string layerTitle, string zoneName)
        {
            return dockedLayerWorkspace.MoveLayerToGuideZone(layerTitle, zoneName);
        }

        public void ClearDockedLayerDocuments()
        {
            dockedLayerWorkspace.ClearDockedLayerDocuments();
        }

        public void ShowDockingGuide(double xRatio, double yRatio)
        {
            dockedLayerWorkspace.ShowGuideAtWorkspaceRatio(xRatio, yRatio);
            updateLayout();
        }

        public Point GetWorkspaceScreenPoint(double x, double y)
        {
            return dockedLayerWorkspace.GetWorkspaceScreenPoint(new Point(x, y));
        }

        public bool ShowFirstDockedLayerTabDragGuide()
        {
            updateLayout();
            if (!dockedLayerWorkspace.ShowFirstDockedLayerTabDragGuide())
            {
                return false;
            }

            updateLayout();
            return true;
        }

        public void HideDockingGuide()
        {
            dockedLayerWorkspace.HideDockingGuide();
            updateLayout();
        }

        public void SaveLayerWorkspaceState()
        {
            dockedLayerWorkspace.SaveLayerWorkspaceState();
        }

        public bool RestoreLayerWorkspaceState()
        {
            return dockedLayerWorkspace.RestoreLayerWorkspaceState();
        }

        public bool SaveDockedLayerImageToFile(string layerTitle, string path)
        {
            IOpenVisionDockedLayerViewer viewer = dockedLayerWorkspace.FindLayerViewer(layerTitle);
            return viewer?.SaveImageToFileForTest(path) ?? false;
        }

        public DrawingBitmap CloneDockedLayerImage(string layerTitle) =>
            dockedLayerWorkspace.FindLayerViewer(layerTitle)?.CloneImageForTest();

        public int GetLayerImagePixelWidth(string layerTitle) =>
            dockedLayerWorkspace.FindLayerViewer(layerTitle)?.ImagePixelWidth ?? 0;

        public int GetLayerImagePixelHeight(string layerTitle) =>
            dockedLayerWorkspace.FindLayerViewer(layerTitle)?.ImagePixelHeight ?? 0;

        public int GetLayerTextureTileCount(string layerTitle) =>
            dockedLayerWorkspace.FindLayerViewer(layerTitle)?.TextureTileCount ?? 0;

        private OpenVisionDockedLayerWorkspaceDiagnostics Diagnostics =>
            dockedLayerWorkspace.CreateDiagnostics() ?? OpenVisionDockedLayerWorkspaceDiagnostics.Empty;
    }
}
