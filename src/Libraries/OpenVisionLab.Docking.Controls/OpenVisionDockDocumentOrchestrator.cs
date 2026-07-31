using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockDocumentOrchestrator
    {
        private readonly IOpenVisionDockDocumentWorkspace workspace;
        private readonly IOpenVisionDockDocumentState documentState;
        private readonly OpenVisionDockDocumentController documents;
        private readonly OpenVisionLayerDockingCommandController commandController;
        private readonly Func<string, bool> canOpenDocument;
        private readonly Action<OpenVisionDockDocumentRefreshResult> applyRefreshResult;
        private readonly Action refreshActions;
        private readonly Action refreshLayout;
        private readonly Action queueWorkspaceStateSave;

        public OpenVisionDockDocumentOrchestrator(
            IOpenVisionDockDocumentWorkspace workspace,
            IOpenVisionDockDocumentState documentState,
            OpenVisionDockDocumentController documents,
            Func<string, bool> canOpenDocument,
            Action<OpenVisionDockDocumentRefreshResult> applyRefreshResult,
            Action refreshActions,
            Action refreshLayout,
            Action queueWorkspaceStateSave)
        {
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            this.documentState = documentState ?? throw new ArgumentNullException(nameof(documentState));
            this.documents = documents ?? throw new ArgumentNullException(nameof(documents));
            this.canOpenDocument = canOpenDocument ?? throw new ArgumentNullException(nameof(canOpenDocument));
            this.applyRefreshResult = applyRefreshResult ?? throw new ArgumentNullException(nameof(applyRefreshResult));
            this.refreshActions = refreshActions ?? throw new ArgumentNullException(nameof(refreshActions));
            this.refreshLayout = refreshLayout ?? throw new ArgumentNullException(nameof(refreshLayout));
            this.queueWorkspaceStateSave = queueWorkspaceStateSave ?? throw new ArgumentNullException(nameof(queueWorkspaceStateSave));
            commandController = new OpenVisionLayerDockingCommandController(
                workspace,
                documents.GetDocumentIds,
                () => documentState.LayerTitles);
        }

        public bool IsRestoring { get; private set; }

        public void RefreshDocuments()
        {
            applyRefreshResult(documents.RefreshDocuments());
            refreshActions();
        }

        public bool DockDocument(string documentId)
        {
            if (!canOpenDocument(documentId))
            {
                return false;
            }

            if (!documentState.Contains(documentId))
            {
                documentState.Add(documentId);
            }

            RefreshDocuments();
            SaveWorkspaceState();
            return true;
        }

        public bool SelectDocument(string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId) || !documentState.Contains(documentId))
            {
                return false;
            }

            bool selected = workspace.SelectDocument(documentId, documents.GetDocumentIds());
            if (selected)
            {
                refreshActions();
            }

            return selected;
        }

        public void ClearDocuments()
        {
            documents.ClearDocuments();
            documentState.Clear();
            workspace.ResetLayoutToPrimaryPane();
            RefreshDocuments();
            refreshLayout();
            SaveWorkspaceState();
        }

        public void ClearDocumentContents()
        {
            documents.ClearDocuments();
        }

        public bool SplitToNewPane(string documentId)
        {
            bool split = commandController.SplitToNewPane(documentId);
            if (split)
            {
                refreshLayout();
            }

            return split;
        }

        public bool MoveToPrimaryPane(string documentId)
        {
            bool moved = commandController.MoveToPrimaryPane(documentId);
            if (moved)
            {
                refreshActions();
            }

            return moved;
        }

        public bool DockToGuideZone(string documentId, DockingGuideZone zone, OpenVisionDockPaneHandle targetPane)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                return false;
            }

            bool moved = commandController.DockToGuideZone(documentId, zone, targetPane);
            if (!moved)
            {
                return false;
            }

            RefreshDocuments();
            refreshLayout();
            queueWorkspaceStateSave();
            return true;
        }

        public bool ArrangePanes(Orientation orientation, params string[] documentIds)
        {
            List<string> titles = documentState.ResolveTargetTitles(documentIds);
            if (titles.Count == 0 || !EnsureDockedDocuments(titles))
            {
                return false;
            }

            IsRestoring = true;
            try
            {
                if (!workspace.ArrangePanes(titles, orientation))
                {
                    return false;
                }
            }
            finally
            {
                IsRestoring = false;
            }

            RefreshDocuments();
            refreshLayout();
            queueWorkspaceStateSave();
            return true;
        }

        public bool ArrangeGrid(params string[] documentIds)
        {
            List<string> titles = documentState.ResolveTargetTitles(documentIds);
            if (titles.Count < 2 || !EnsureDockedDocuments(titles))
            {
                return false;
            }

            IsRestoring = true;
            try
            {
                if (!workspace.ArrangeGrid(titles))
                {
                    return false;
                }
            }
            finally
            {
                IsRestoring = false;
            }

            RefreshDocuments();
            refreshLayout();
            queueWorkspaceStateSave();
            return true;
        }

        public void EnsureStateLoaded()
        {
            documents.EnsureStateLoaded();
        }

        public bool ApplyPersistedDocuments()
        {
            return documents.ApplyPersistedLayers(canOpenDocument);
        }

        public bool CanQueueWorkspaceStateSave(bool loaded)
        {
            return !IsRestoring && documents.CanQueueWorkspaceStateSave(loaded);
        }

        public bool CanNormalizeComparisonLayout()
        {
            return !IsRestoring && documents.CanNormalizeComparisonLayout();
        }

        public void SaveWorkspaceState(bool preservePendingPersistedState = false)
        {
            documents.SaveWorkspaceState(preservePendingPersistedState);
        }

        public bool RestoreLayoutState()
        {
            bool restored = documents.RestoreLayoutState();
            if (restored)
            {
                refreshLayout();
            }

            return restored;
        }

        private bool EnsureDockedDocuments(IEnumerable<string> documentIds)
        {
            foreach (string documentId in documentIds)
            {
                if (!documentState.Contains(documentId) && !DockDocument(documentId))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
