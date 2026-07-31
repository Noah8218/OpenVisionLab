using System;
using System.Collections.Generic;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockWorkspaceCompositionOptions<TDocumentState, TWorkspaceState>
    {
        public OpenVisionDockWorkspaceCompositionOptions(
            OpenVisionLayerDockWorkspaceView workspaceView,
            IOpenVisionDockDocumentState documentState,
            IOpenVisionDockDocumentContentSource contentSource,
            Predicate<object> documentContentPredicate,
            Func<OpenVisionDockDocumentState, TDocumentState> createDocumentState,
            Func<IReadOnlyList<TDocumentState>, int, string, int, TWorkspaceState> createWorkspaceState,
            Func<string, bool> canOpenDocument,
            Func<bool> isLoadedProvider,
            Action<OpenVisionDockDocumentRefreshResult> applyRefreshResult,
            Action refreshActions)
        {
            WorkspaceView = workspaceView ?? throw new ArgumentNullException(nameof(workspaceView));
            DocumentState = documentState ?? throw new ArgumentNullException(nameof(documentState));
            ContentSource = contentSource ?? throw new ArgumentNullException(nameof(contentSource));
            DocumentContentPredicate = documentContentPredicate ?? throw new ArgumentNullException(nameof(documentContentPredicate));
            CreateDocumentState = createDocumentState ?? throw new ArgumentNullException(nameof(createDocumentState));
            CreateWorkspaceState = createWorkspaceState ?? throw new ArgumentNullException(nameof(createWorkspaceState));
            CanOpenDocument = canOpenDocument ?? throw new ArgumentNullException(nameof(canOpenDocument));
            IsLoadedProvider = isLoadedProvider ?? throw new ArgumentNullException(nameof(isLoadedProvider));
            ApplyRefreshResult = applyRefreshResult ?? throw new ArgumentNullException(nameof(applyRefreshResult));
            RefreshActions = refreshActions ?? throw new ArgumentNullException(nameof(refreshActions));
        }

        public OpenVisionLayerDockWorkspaceView WorkspaceView { get; }

        public IOpenVisionDockDocumentState DocumentState { get; }

        public IOpenVisionDockDocumentContentSource ContentSource { get; }

        public Predicate<object> DocumentContentPredicate { get; }

        public Func<OpenVisionDockDocumentState, TDocumentState> CreateDocumentState { get; }

        public Func<IReadOnlyList<TDocumentState>, int, string, int, TWorkspaceState> CreateWorkspaceState { get; }

        public Func<string, bool> CanOpenDocument { get; }

        public Func<bool> IsLoadedProvider { get; }

        public Action<OpenVisionDockDocumentRefreshResult> ApplyRefreshResult { get; }

        public Action RefreshActions { get; }
    }
}
