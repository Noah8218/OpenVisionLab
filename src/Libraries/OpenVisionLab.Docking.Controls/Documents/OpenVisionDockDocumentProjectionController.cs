using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockDocumentProjectionController<TDocumentState, TWorkspaceState>
    {
        private readonly IOpenVisionDockDocumentWorkspace workspace;
        private readonly OpenVisionDockDocumentController documents;
        private readonly Func<OpenVisionDockDocumentState, TDocumentState> createDocumentState;
        private readonly Func<IReadOnlyList<TDocumentState>, int, string, int, TWorkspaceState> createWorkspaceState;

        public OpenVisionDockDocumentProjectionController(
            IOpenVisionDockDocumentWorkspace workspace,
            OpenVisionDockDocumentController documents,
            Func<OpenVisionDockDocumentState, TDocumentState> createDocumentState,
            Func<IReadOnlyList<TDocumentState>, int, string, int, TWorkspaceState> createWorkspaceState)
        {
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            this.documents = documents ?? throw new ArgumentNullException(nameof(documents));
            this.createDocumentState = createDocumentState ?? throw new ArgumentNullException(nameof(createDocumentState));
            this.createWorkspaceState = createWorkspaceState ?? throw new ArgumentNullException(nameof(createWorkspaceState));
        }

        public TContent FindContent<TContent>(string documentId)
            where TContent : class
        {
            return documents.FindDocumentContent(documentId) as TContent;
        }

        public TWorkspaceState GetWorkspaceState()
        {
            List<TDocumentState> documentStates = documents
                .EnumerateDocumentStates()
                .Select(createDocumentState)
                .ToList();

            return createWorkspaceState(
                documentStates,
                workspace.ContentPaneCount,
                workspace.RootOrientationName,
                workspace.NestedLayoutPanelCount);
        }

        public List<string> GetDocumentIds()
        {
            return documents.GetDocumentIds();
        }
    }
}
