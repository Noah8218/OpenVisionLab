using System;
using System.Collections.Generic;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockDocumentController
    {
        private readonly IOpenVisionDockDocumentWorkspace workspace;
        private readonly IOpenVisionDockDocumentState documentState;
        private readonly IOpenVisionDockDocumentContentSource contentSource;

        public OpenVisionDockDocumentController(
            IOpenVisionDockDocumentWorkspace workspace,
            IOpenVisionDockDocumentState documentState,
            IOpenVisionDockDocumentContentSource contentSource)
        {
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            this.documentState = documentState ?? throw new ArgumentNullException(nameof(documentState));
            this.contentSource = contentSource ?? throw new ArgumentNullException(nameof(contentSource));
        }

        public bool IsSynchronizing { get; private set; }

        public bool IsRestoring { get; private set; }

        public OpenVisionDockDocumentRefreshResult RefreshDocuments()
        {
            List<string> documentIds = GetDocumentIds();
            if (documentIds.Count == 0)
            {
                ClearDocuments();
                return new OpenVisionDockDocumentRefreshResult(false, 0);
            }

            // Documents mirror the supplied model. Pane splits/tabs are changed only by explicit docking commands.
            if (!workspace.EnsurePrimaryPane())
            {
                return new OpenVisionDockDocumentRefreshResult(true, documentIds.Count);
            }

            string selectedContentId = workspace.ResolveSelectedDocumentContentId(documentIds, contentSource.SelectedDocumentId);

            IsSynchronizing = true;
            try
            {
                workspace.CloseStaleDocuments(documentIds);
            }
            finally
            {
                IsSynchronizing = false;
            }

            foreach (string documentId in documentIds)
            {
                workspace.UpsertDocumentInPrimaryPane(
                    documentId,
                    documentIds,
                    content => contentSource.UpdateDocumentContent(documentId, content));
            }

            if (!workspace.SelectDocument(selectedContentId, documentIds))
            {
                workspace.SelectLastDocument(documentIds);
            }

            return new OpenVisionDockDocumentRefreshResult(true, documentIds.Count);
        }

        public void ClearDocuments()
        {
            IsSynchronizing = true;
            try
            {
                workspace.CloseDocuments(GetDocumentIds());
            }
            finally
            {
                IsSynchronizing = false;
            }
        }

        public bool HandleDocumentClosed(object sender)
        {
            if (IsSynchronizing)
            {
                return false;
            }

            if (!workspace.TryCloseDocumentFromSender(sender, out string contentId))
            {
                return false;
            }

            documentState.Remove(contentId);
            return true;
        }

        public bool CanQueueWorkspaceStateSave(bool loaded)
        {
            return !IsRestoring
                && !IsSynchronizing
                && loaded
                && GetDocumentIds().Count > 0;
        }

        public bool CanNormalizeComparisonLayout()
        {
            return !IsRestoring
                && !IsSynchronizing
                && GetDocumentIds().Count > 0;
        }

        public void EnsureStateLoaded()
        {
            documentState.EnsureStateLoaded();
        }

        public bool ApplyPersistedLayers(Func<string, bool> canOpenLayer)
        {
            return documentState.ApplyPersistedLayers(canOpenLayer);
        }

        public void SaveWorkspaceState(bool preservePendingPersistedState)
        {
            if (documentState.HasLayers)
            {
                documentState.SaveLayerState(SaveLayoutState, preservePendingPersistedState);
                return;
            }

            SaveLayoutState();
        }

        public bool RestoreLayoutState()
        {
            List<string> documentIds = GetDocumentIds();
            if (!workspace.HasRootPanel || documentIds.Count == 0)
            {
                return false;
            }

            IReadOnlyList<OpenVisionDockDocumentLayoutEntry> paneLayout = documentState.LoadPaneLayout(documentIds);
            if (paneLayout.Count == 0)
            {
                return false;
            }

            IsRestoring = true;
            try
            {
                return workspace.RestorePaneLayout(documentIds, paneLayout);
            }
            catch
            {
                return false;
            }
            finally
            {
                IsRestoring = false;
            }
        }

        public object FindDocumentContent(string documentId)
        {
            return workspace.FindDocumentContent(documentId, GetDocumentIds());
        }

        public IEnumerable<OpenVisionDockDocumentState> EnumerateDocumentStates()
        {
            return workspace.EnumerateDocumentStates(GetDocumentIds());
        }

        public List<string> GetDocumentIds()
        {
            return documentState.LayerTitles?.ToList() ?? new List<string>();
        }

        private void SaveLayoutState()
        {
            List<string> documentIds = GetDocumentIds();
            if (!workspace.HasRootPanel || documentIds.Count == 0)
            {
                return;
            }

            documentState.SavePaneLayout(workspace.CapturePaneLayout(documentIds));
        }
    }
}
