using OpenVisionLab.Docking.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal interface IOpenVisionDockedLayerWorkspaceLayerCatalog
    {
        ICollection<string> LayerTitles { get; }

        bool HasLayers { get; }
    }

    internal interface IOpenVisionDockedLayerWorkspaceCommands : IOpenVisionDockedLayerWorkspaceLayerCatalog
    {
        event EventHandler WorkspaceStateChanged;

        bool DockLayerDocument(string layerTitle);

        bool ActivateLayerDocument(string layerTitle);

        void ClearDockedLayerDocuments();
    }

    internal interface IOpenVisionDockedLayerWorkspaceRefresh
    {
        void RefreshLayerViewers();
    }

    internal interface IOpenVisionDockedLayerWorkspaceSynchronization :
        IOpenVisionDockedLayerWorkspaceLayerCatalog,
        IOpenVisionDockedLayerWorkspaceRefresh
    {
        void SyncLayerDocuments(IReadOnlyList<string> layerTitles);
    }

    internal interface IOpenVisionDockedLayerWorkspaceSession
    {
        void EnsureWorkspaceStateLoaded();

        bool RestoreLayerWorkspaceState();

        void SaveLayerWorkspaceState(bool preservePendingPersistedState = false);

        void StopPendingWorkspaceSave();

        void ReleaseLayerViewerContents();

        void AttachDockLifecycle(IOpenVisionDockLifecycle lifecycle);
    }

    internal interface IOpenVisionDockedLayerWorkspaceDiagnostics
    {
        bool IsWorkspaceVisible { get; }

        bool HasGuideOverlay { get; }

        bool IsGuideOverlayVisible { get; }

        string ActiveGuideZoneName { get; }

        bool IsGuideOverlayHitTestSafe { get; }

        int GuideZoneCount { get; }

        bool DockLayerDocument(string layerTitle);

        void ClearDockedLayerDocuments();

        bool SplitLayerToNewPane(string layerTitle);

        bool MoveLayerToPrimaryPane(string layerTitle);

        bool MoveLayerToGuideZone(string layerTitle, DockingGuideZone zone);

        bool MoveLayerToGuideZone(string layerTitle, string zoneName);

        bool ArrangeLayerPanes(Orientation orientation, params string[] layerTitles);

        bool ArrangeLayerPanes(string orientationName, params string[] layerTitles);

        bool ArrangeLayerGrid(params string[] layerTitles);

        IOpenVisionDockedLayerViewer FindLayerViewer(string layerTitle);

        OpenVisionDockedLayerWorkspaceState GetLayerWorkspaceState();

        OpenVisionDockedLayerWorkspaceDiagnostics CreateDiagnostics();

        bool RestoreLayerWorkspaceState();

        void SaveLayerWorkspaceState(bool preservePendingPersistedState = false);

        void ShowGuideAtWorkspaceRatio(double xRatio, double yRatio);

        Point GetWorkspaceScreenPoint(Point point);

        OpenVisionDockingVisualSnapshot CreateDockingVisualSnapshot();

        bool ShowFirstDockedLayerTabDragGuide();

        void HideDockingGuide();
    }

    internal interface IOpenVisionDockedLayerWorkspace :
        IOpenVisionDockedLayerWorkspaceCommands,
        IOpenVisionDockedLayerWorkspaceSynchronization,
        IOpenVisionDockedLayerWorkspaceSession,
        IOpenVisionDockedLayerWorkspaceDiagnostics
    {
    }
}
