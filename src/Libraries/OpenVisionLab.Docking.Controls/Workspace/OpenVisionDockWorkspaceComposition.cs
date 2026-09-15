using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockWorkspaceComposition<TDocumentState, TWorkspaceState>
    {
        private bool gestureControllerAttached;

        private OpenVisionDockWorkspaceComposition(
            OpenVisionLayerDockWorkspaceView workspaceView,
            IOpenVisionDockDocumentState documentState,
            IOpenVisionDockDocumentWorkspace workspace,
            OpenVisionDockDocumentController documents,
            OpenVisionDockDocumentOrchestrator documentOrchestrator,
            OpenVisionDockDocumentSynchronizationController documentSynchronization,
            OpenVisionDockDocumentProjectionController<TDocumentState, TWorkspaceState> documentProjection,
            OpenVisionDockWorkspaceStateSaveScheduler stateSaveScheduler,
            OpenVisionDockWorkspaceLayoutController layoutController,
            OpenVisionDockWorkspaceLifecycleBinder lifecycleBinder,
            OpenVisionLayerDockingGestureController gestureController)
        {
            WorkspaceView = workspaceView ?? throw new ArgumentNullException(nameof(workspaceView));
            DocumentState = documentState ?? throw new ArgumentNullException(nameof(documentState));
            Workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            Documents = documents ?? throw new ArgumentNullException(nameof(documents));
            DocumentOrchestrator = documentOrchestrator ?? throw new ArgumentNullException(nameof(documentOrchestrator));
            DocumentSynchronization = documentSynchronization ?? throw new ArgumentNullException(nameof(documentSynchronization));
            DocumentProjection = documentProjection ?? throw new ArgumentNullException(nameof(documentProjection));
            StateSaveScheduler = stateSaveScheduler ?? throw new ArgumentNullException(nameof(stateSaveScheduler));
            LayoutController = layoutController ?? throw new ArgumentNullException(nameof(layoutController));
            LifecycleBinder = lifecycleBinder ?? throw new ArgumentNullException(nameof(lifecycleBinder));
            GestureController = gestureController ?? throw new ArgumentNullException(nameof(gestureController));
        }

        private OpenVisionLayerDockWorkspaceView WorkspaceView { get; }

        private IOpenVisionDockDocumentState DocumentState { get; }

        private IOpenVisionDockDocumentWorkspace Workspace { get; }

        private OpenVisionDockDocumentController Documents { get; }

        private OpenVisionDockDocumentOrchestrator DocumentOrchestrator { get; }

        private OpenVisionDockDocumentSynchronizationController DocumentSynchronization { get; }

        private OpenVisionDockDocumentProjectionController<TDocumentState, TWorkspaceState> DocumentProjection { get; }

        private OpenVisionDockWorkspaceStateSaveScheduler StateSaveScheduler { get; }

        private OpenVisionDockWorkspaceLayoutController LayoutController { get; }

        private OpenVisionDockWorkspaceLifecycleBinder LifecycleBinder { get; }

        private OpenVisionLayerDockingGestureController GestureController { get; }

        public int DocumentCount => DocumentState.Count;

        public event EventHandler WorkspaceStateChanged;

        public static OpenVisionDockWorkspaceComposition<TDocumentState, TWorkspaceState> Create(
            OpenVisionDockWorkspaceCompositionOptions<TDocumentState, TWorkspaceState> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            OpenVisionDockWorkspaceComposition<TDocumentState, TWorkspaceState> composition = null;
            EventHandler documentClosedHandler = (sender, args) => composition?.HandleDocumentClosed(sender);
            Action refreshLayout = () => composition?.RefreshLayout();
            Action queueWorkspaceStateSave = () => composition?.QueueWorkspaceStateSave();
            Action saveWorkspaceState = () => composition?.SaveWorkspaceState();

            IOpenVisionDockDocumentWorkspace workspace = new OpenVisionDockWorkspaceController(
                options.WorkspaceView.WorkspaceHandle,
                documentClosedHandler,
                options.DocumentContentPredicate);
            OpenVisionDockDocumentController documents = new OpenVisionDockDocumentController(
                workspace,
                options.DocumentState,
                options.ContentSource);
            OpenVisionDockDocumentProjectionController<TDocumentState, TWorkspaceState> documentProjection =
                new OpenVisionDockDocumentProjectionController<TDocumentState, TWorkspaceState>(
                    workspace,
                    documents,
                    options.CreateDocumentState,
                    options.CreateWorkspaceState);
            OpenVisionDockDocumentOrchestrator documentOrchestrator = new OpenVisionDockDocumentOrchestrator(
                workspace,
                options.DocumentState,
                documents,
                options.CanOpenDocument,
                options.ApplyRefreshResult,
                options.RefreshActions,
                refreshLayout,
                queueWorkspaceStateSave);
            OpenVisionDockDocumentSynchronizationController documentSynchronization = new OpenVisionDockDocumentSynchronizationController(
                options.DocumentState,
                documentOrchestrator,
                options.CanOpenDocument,
                refreshLayout);
            OpenVisionLayerDockingGuidePresenter guidePresenter = new OpenVisionLayerDockingGuidePresenter(
                options.WorkspaceView,
                workspace);
            OpenVisionDockingGuideStateController guideStateController = new OpenVisionDockingGuideStateController(
                options.WorkspaceView,
                guidePresenter);
            OpenVisionLayerDockingGestureController gestureController = new OpenVisionLayerDockingGestureController(
                options.WorkspaceView,
                guidePresenter,
                guideStateController,
                options.CanOpenDocument,
                documentOrchestrator.DockToGuideZone);
            OpenVisionDockWorkspaceStateSaveScheduler stateSaveScheduler = new OpenVisionDockWorkspaceStateSaveScheduler(
                TimeSpan.FromMilliseconds(600),
                () => documentOrchestrator.CanQueueWorkspaceStateSave(options.IsLoadedProvider()),
                saveWorkspaceState);
            OpenVisionDockWorkspaceLayoutController layoutController = new OpenVisionDockWorkspaceLayoutController(
                options.WorkspaceView,
                workspace,
                stateSaveScheduler,
                documentOrchestrator.CanNormalizeComparisonLayout);
            OpenVisionDockWorkspaceLifecycleBinder lifecycleBinder = new OpenVisionDockWorkspaceLifecycleBinder(
                options.WorkspaceView,
                layoutController.HandleLayoutChanged,
                layoutController.HandleDockingStateChanged);

            composition = new OpenVisionDockWorkspaceComposition<TDocumentState, TWorkspaceState>(
                options.WorkspaceView,
                options.DocumentState,
                workspace,
                documents,
                documentOrchestrator,
                documentSynchronization,
                documentProjection,
                stateSaveScheduler,
                layoutController,
                lifecycleBinder,
                gestureController);
            return composition;
        }

        public void AttachGestureController()
        {
            if (gestureControllerAttached)
            {
                return;
            }

            WorkspaceView.IsWorkspaceDropEnabled = true;
            WorkspaceView.PreviewMouseDown += GestureController.HandlePreviewMouseDown;
            WorkspaceView.PreviewMouseMove += GestureController.HandlePreviewMouseMove;
            WorkspaceView.PreviewMouseUp += GestureController.HandlePreviewMouseUp;
            WorkspaceView.MouseLeave += GestureController.HandleMouseLeave;
            WorkspaceView.DragOver += GestureController.HandleDragOver;
            WorkspaceView.Drop += GestureController.HandleDrop;
            WorkspaceView.DragLeave += GestureController.HandleDragLeave;
            gestureControllerAttached = true;
        }

        public void AttachLifecycle(IOpenVisionDockLifecycle lifecycle)
        {
            if (lifecycle == null)
            {
                throw new ArgumentNullException(nameof(lifecycle));
            }

            LifecycleBinder.Attach(lifecycle);
            StateSaveScheduler.Attach(lifecycle);
        }

        public void RefreshLayout()
        {
            LayoutController.RefreshLayout();
        }

        public bool DockDocument(string documentId)
        {
            return NotifyWorkspaceStateChangedIf(DocumentOrchestrator.DockDocument(documentId));
        }

        public bool SelectDocument(string documentId)
        {
            return NotifyWorkspaceStateChangedIf(DocumentOrchestrator.SelectDocument(documentId));
        }

        public bool SyncDocuments(IReadOnlyList<string> documentIds)
        {
            return NotifyWorkspaceStateChangedIf(DocumentSynchronization.SyncDocuments(documentIds));
        }

        public void ClearDocuments()
        {
            DocumentOrchestrator.ClearDocuments();
            NotifyWorkspaceStateChanged();
        }

        public void RefreshDocuments()
        {
            DocumentOrchestrator.RefreshDocuments();
        }

        public void ClearDocumentContents()
        {
            DocumentOrchestrator.ClearDocumentContents();
        }

        public bool SplitToNewPane(string documentId)
        {
            return NotifyWorkspaceStateChangedIf(DocumentOrchestrator.SplitToNewPane(documentId));
        }

        public bool MoveToPrimaryPane(string documentId)
        {
            return NotifyWorkspaceStateChangedIf(DocumentOrchestrator.MoveToPrimaryPane(documentId));
        }

        public bool DockToPrimaryGuideZone(string documentId, DockingGuideZone zone)
        {
            return DockToGuideZone(documentId, zone, Workspace.GetPrimaryPaneHandle());
        }

        public bool DockToGuideZone(string documentId, DockingGuideZone zone, OpenVisionDockPaneHandle targetPane)
        {
            return NotifyWorkspaceStateChangedIf(DocumentOrchestrator.DockToGuideZone(documentId, zone, targetPane));
        }

        public bool ArrangePanes(Orientation orientation, params string[] documentIds)
        {
            return NotifyWorkspaceStateChangedIf(DocumentOrchestrator.ArrangePanes(orientation, documentIds));
        }

        public bool ArrangeGrid(params string[] documentIds)
        {
            return NotifyWorkspaceStateChangedIf(DocumentOrchestrator.ArrangeGrid(documentIds));
        }

        public void EnsureStateLoaded()
        {
            DocumentOrchestrator.EnsureStateLoaded();
        }

        public bool ApplyPersistedDocuments()
        {
            return DocumentOrchestrator.ApplyPersistedDocuments();
        }

        public bool RestoreLayoutState()
        {
            return DocumentOrchestrator.RestoreLayoutState();
        }

        public void SaveWorkspaceState(bool preservePendingPersistedState = false)
        {
            DocumentOrchestrator.SaveWorkspaceState(preservePendingPersistedState);
        }

        private void HandleDocumentClosed(object sender)
        {
            if (!Documents.HandleDocumentClosed(sender))
            {
                return;
            }

            RefreshDocuments();
            SaveWorkspaceState();
            NotifyWorkspaceStateChanged();
        }

        public void ResetLayoutToPrimaryPane()
        {
            Workspace.ResetLayoutToPrimaryPane();
        }

        public TContent FindContent<TContent>(string documentId)
            where TContent : class
        {
            return DocumentProjection.FindContent<TContent>(documentId);
        }

        public TWorkspaceState GetWorkspaceState()
        {
            return DocumentProjection.GetWorkspaceState();
        }

        public List<string> GetDocumentIds()
        {
            return DocumentProjection.GetDocumentIds();
        }

        public IEnumerable<FrameworkElement> EnumerateGestureHeaders()
        {
            return WorkspaceView.EnumerateGestureHeaders();
        }

        public void ShowGuideAt(Point point)
        {
            GestureController.ShowGuideAt(point);
        }

        public bool BeginTestDragGuide(DependencyObject source, Point point)
        {
            return GestureController.BeginTestDragGuide(source, point);
        }

        public bool IsGestureSource(DependencyObject source)
        {
            return GestureController.IsGestureSource(source);
        }

        public void ResetGuide()
        {
            GestureController.Reset();
        }

        private void QueueWorkspaceStateSave()
        {
            StateSaveScheduler.Queue();
        }

        public void StopPendingWorkspaceStateSave()
        {
            StateSaveScheduler.Stop();
        }

        private bool NotifyWorkspaceStateChangedIf(bool changed)
        {
            if (changed)
            {
                NotifyWorkspaceStateChanged();
            }

            return changed;
        }

        private void NotifyWorkspaceStateChanged()
        {
            WorkspaceStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
