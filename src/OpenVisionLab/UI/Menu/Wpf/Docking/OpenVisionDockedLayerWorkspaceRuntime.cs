using OpenVisionLab.Docking.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace OpenVisionLab
{
    internal sealed class OpenVisionDockedLayerWorkspaceRuntime : IOpenVisionDockedLayerWorkspace
    {
        private readonly OpenVisionLayerDockWorkspaceView workspaceView;
        private readonly OpenVisionShellHostDockedLayerOrchestrator orchestrator;
        private readonly Action<string> activateLayer;
        private readonly OpenVisionDockedLayerContentSource contentSource;
        private bool pointerLayerSelectionPending;

        internal OpenVisionDockedLayerWorkspaceRuntime(
            OpenVisionLayerDockWorkspaceView workspaceView,
            OpenVisionDockedLayerWorkspaceViewModel viewModel,
            OpenVisionShellHostDockedLayerOrchestrator orchestrator,
            Action<string> activateLayer,
            OpenVisionDockedLayerContentSource contentSource)
        {
            this.workspaceView = workspaceView ?? throw new ArgumentNullException(nameof(workspaceView));
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.orchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
            this.activateLayer = activateLayer ?? throw new ArgumentNullException(nameof(activateLayer));
            this.contentSource = contentSource ?? throw new ArgumentNullException(nameof(contentSource));
            this.orchestrator.WorkspaceStateChanged += OnOrchestratorWorkspaceStateChanged;
            this.workspaceView.ActiveDocumentChanged += OnActiveDocumentChanged;
            this.workspaceView.PreviewMouseDown += OnWorkspacePreviewMouseDown;
        }

        public event EventHandler WorkspaceStateChanged;

        public OpenVisionDockedLayerWorkspaceViewModel ViewModel { get; }

        public ICollection<string> LayerTitles => ViewModel.LayerTitles;

        public bool HasLayers => ViewModel.HasLayers;

        public IOpenVisionDockedLayerViewer ActiveLayerViewer => string.IsNullOrWhiteSpace(workspaceView.ActiveDocumentId) ? null : orchestrator.FindViewer(workspaceView.ActiveDocumentId);

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

        public bool ActivateLayerDocument(string layerTitle)
        {
            return orchestrator.ActivateLayer(layerTitle);
        }

        public void FitLayerImages()
        {
            foreach (string layerTitle in LayerTitles)
            {
                orchestrator.FindViewer(layerTitle)?.FitImageToView();
            }
        }

        public void SyncLayerDocuments(IReadOnlyList<string> layerTitles)
        {
            orchestrator.SyncLayers(contentSource.MainViewer?.DocumentIds ?? layerTitles);
        }

        public void AttachMainViewer(OpenVisionWorkbenchViewerViewModel viewer)
        {
            contentSource.MainViewer = viewer ?? throw new ArgumentNullException(nameof(viewer));
            workspaceView.IsPresetLayout = true;
            viewer.LayoutChanged += OnMainLayoutChanged;
            viewer.SourcesChanged += OnMainSourcesChanged;
            OnMainLayoutChanged(viewer, EventArgs.Empty);
        }

        private void OnMainLayoutChanged(object sender, EventArgs e)
        {
            OpenVisionWorkbenchViewerViewModel viewer = contentSource.MainViewer;
            if (viewer == null) { return; }
            orchestrator.SyncLayers(viewer.DocumentIds);
            if (viewer.Layout == 4) { orchestrator.ArrangeGrid(viewer.DocumentIds); }
            else { orchestrator.ArrangePanes(Orientation.Horizontal, viewer.DocumentIds); }
            orchestrator.ActivateLayer(viewer.ActivePaneId);
        }

        private void OnMainSourcesChanged(object sender, EventArgs e) => orchestrator.RefreshViews();

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

        public void ReleaseLayerViewerContents()
        {
            if (contentSource.MainViewer != null)
            {
                contentSource.MainViewer.LayoutChanged -= OnMainLayoutChanged;
                contentSource.MainViewer.SourcesChanged -= OnMainSourcesChanged;
                contentSource.MainViewer = null;
            }
            orchestrator.ClearDocuments();
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

        private void OnActiveDocumentChanged(object sender, EventArgs e)
        {
            WorkspaceStateChanged?.Invoke(this, EventArgs.Empty);
            if (!pointerLayerSelectionPending)
            {
                return;
            }

            pointerLayerSelectionPending = false;
            string layerTitle = workspaceView.ActiveDocumentId;
            if (contentSource.MainViewer?.FindPane(layerTitle) != null)
            {
                contentSource.MainViewer.Activate(layerTitle);
                return;
            }
            if (!string.IsNullOrWhiteSpace(layerTitle))
            {
                QueueLayerActivation(layerTitle);
            }
        }

        private void OnWorkspacePreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenVisionLayerViewerView mainView = FindVisualAncestor<OpenVisionLayerViewerView>(e?.OriginalSource as DependencyObject);
            if (mainView?.MainPane != null)
            {
                orchestrator.ActivateLayer(mainView.MainPane.Id);
                contentSource.MainViewer?.Activate(mainView.MainPane.Id);
                return;
            }
            if (FindVisualAncestor<ButtonBase>(e?.OriginalSource as DependencyObject) != null)
            {
                return;
            }

            pointerLayerSelectionPending = true;
            workspaceView.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (Action)(() => pointerLayerSelectionPending = false));
            OpenVisionLayerViewerView viewer = FindVisualAncestor<OpenVisionLayerViewerView>(
                e?.OriginalSource as DependencyObject);
            if (!string.IsNullOrWhiteSpace(viewer?.LayerTitle))
            {
                QueueLayerActivation(viewer.LayerTitle);
            }
        }

        private void QueueLayerActivation(string layerTitle)
        {
            if (contentSource.MainViewer != null)
            {
                string paneId = workspaceView.ActiveDocumentId;
                if (contentSource.MainViewer.FindPane(paneId) != null) { contentSource.MainViewer.Activate(paneId); }
                return;
            }
            workspaceView.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (Action)(() => activateLayer(layerTitle)));
        }

        private static T FindVisualAncestor<T>(DependencyObject element)
            where T : DependencyObject
        {
            while (element != null)
            {
                if (element is T match)
                {
                    return match;
                }

                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        }
    }
}
