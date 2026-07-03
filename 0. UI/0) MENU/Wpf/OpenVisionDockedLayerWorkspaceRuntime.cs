using OpenVisionLab.Docking.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class OpenVisionDockedLayerWorkspaceRuntime : IOpenVisionDockedLayerWorkspace
    {
        private readonly OpenVisionLayerDockWorkspaceView workspaceView;
        private readonly OpenVisionShellHostDockedLayerOrchestrator orchestrator;

        internal OpenVisionDockedLayerWorkspaceRuntime(
            OpenVisionLayerDockWorkspaceView workspaceView,
            OpenVisionDockedLayerWorkspaceViewModel viewModel,
            OpenVisionShellHostDockedLayerOrchestrator orchestrator)
        {
            this.workspaceView = workspaceView ?? throw new ArgumentNullException(nameof(workspaceView));
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.orchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
            this.orchestrator.WorkspaceStateChanged += OnOrchestratorWorkspaceStateChanged;
        }

        public event EventHandler WorkspaceStateChanged;

        public OpenVisionDockedLayerWorkspaceViewModel ViewModel { get; }

        public ICollection<string> LayerTitles => ViewModel.LayerTitles;

        public bool HasLayers => ViewModel.HasLayers;

        public bool IsWorkspaceVisible => workspaceView.Visibility == Visibility.Visible;

        public bool HasGuideOverlay => workspaceView != null;

        public bool IsGuideOverlayVisible => workspaceView.IsGuideOverlayVisible;

        public string ActiveGuideZoneName => workspaceView.ActiveGuideZone.ToString();

        public bool IsGuideOverlayHitTestSafe => workspaceView.IsGuideOverlayHitTestSafe;

        public int GuideZoneCount => workspaceView.GuideZoneCount;

        public bool DockLayerDocument(string layerTitle)
        {
            return orchestrator.DockLayer(layerTitle);
        }

        public void SyncLayerDocuments(IReadOnlyList<string> layerTitles)
        {
            orchestrator.SyncLayers(layerTitles);
        }

        public void ClearDockedLayerDocuments()
        {
            orchestrator.ClearLayers();
        }

        public void RefreshLayerViewers()
        {
            orchestrator.RefreshViews();
        }

        public bool SplitLayerToNewPane(string layerTitle)
        {
            return orchestrator.SplitToNewPane(layerTitle);
        }

        public bool MoveLayerToPrimaryPane(string layerTitle)
        {
            return orchestrator.MoveToPrimaryPane(layerTitle);
        }

        public bool MoveLayerToGuideZone(string layerTitle, DockingGuideZone zone)
        {
            return orchestrator.DockLayerToGuideZone(layerTitle, zone);
        }

        public bool MoveLayerToGuideZone(string layerTitle, string zoneName)
        {
            return MoveLayerToGuideZone(layerTitle, OpenVisionDockingGuideZoneParser.ParseOrCenter(zoneName));
        }

        public bool ArrangeLayerPanes(Orientation orientation, params string[] layerTitles)
        {
            return orchestrator.ArrangePanes(orientation, layerTitles);
        }

        public bool ArrangeLayerPanes(string orientationName, params string[] layerTitles)
        {
            Orientation orientation = string.Equals(orientationName, "Vertical", StringComparison.OrdinalIgnoreCase)
                ? Orientation.Vertical
                : Orientation.Horizontal;
            return ArrangeLayerPanes(orientation, layerTitles);
        }

        public bool ArrangeLayerGrid(params string[] layerTitles)
        {
            return orchestrator.ArrangeGrid(layerTitles);
        }

        public IOpenVisionDockedLayerViewer FindLayerViewer(string layerTitle)
        {
            return orchestrator.FindViewer(layerTitle);
        }

        public OpenVisionDockedLayerWorkspaceState GetLayerWorkspaceState()
        {
            return orchestrator.GetWorkspaceState();
        }

        public OpenVisionDockedLayerWorkspaceDiagnostics CreateDiagnostics()
        {
            OpenVisionDockedLayerWorkspaceState state = GetLayerWorkspaceState()
                ?? OpenVisionDockedLayerWorkspaceState.Empty;
            return new OpenVisionDockedLayerWorkspaceDiagnostics(
                state,
                workspaceView.CreateHeaderDiagnostics(state.Documents.Count));
        }

        public void ShowGuideAtWorkspaceRatio(double xRatio, double yRatio)
        {
            orchestrator.ShowGuideAt(workspaceView.PointFromWorkspaceRatio(xRatio, yRatio));
        }

        public Point GetWorkspaceScreenPoint(Point point)
        {
            return workspaceView.PointToScreenFromWorkspace(point);
        }

        public OpenVisionDockingVisualSnapshot CreateDockingVisualSnapshot()
        {
            return workspaceView.CreateVisualSnapshot();
        }

        public bool ShowFirstDockedLayerTabDragGuide()
        {
            workspaceView.UpdateDockLayout();
            FrameworkElement tabHeader = workspaceView.FindDockedLayerTabHeader();
            if (tabHeader == null || !orchestrator.IsGestureSource(tabHeader))
            {
                return false;
            }

            Point point = workspaceView.TranslateToWorkspace(
                tabHeader,
                new Point(Math.Max(1D, tabHeader.ActualWidth * 0.5D), Math.Max(1D, tabHeader.ActualHeight * 0.5D)));
            return orchestrator.BeginTestDragGuide(tabHeader, point);
        }

        public void EnsureWorkspaceStateLoaded()
        {
            orchestrator.EnsureStateLoaded();
        }

        public bool RestoreLayerWorkspaceState()
        {
            return orchestrator.RestoreWorkspaceState();
        }

        public void SaveLayerWorkspaceState(bool preservePendingPersistedState = false)
        {
            orchestrator.SaveWorkspaceState(preservePendingPersistedState);
        }

        public void StopPendingWorkspaceSave()
        {
            orchestrator.StopPendingSave();
        }

        public void AttachDockLifecycle(IOpenVisionDockLifecycle lifecycle)
        {
            orchestrator.AttachLifecycle(lifecycle);
        }

        public void HideDockingGuide()
        {
            orchestrator.ResetGuide();
        }

        private void OnOrchestratorWorkspaceStateChanged(object sender, EventArgs e)
        {
            ViewModel.RefreshDocumentState();
            WorkspaceStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
