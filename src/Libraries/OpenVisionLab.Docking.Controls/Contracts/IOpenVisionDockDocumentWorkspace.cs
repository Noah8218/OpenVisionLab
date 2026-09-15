using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace OpenVisionLab.Docking.Controls
{
    public interface IOpenVisionDockDocumentWorkspace : IOpenVisionDockPaneProvider, IOpenVisionLayerDockingCommandTarget
    {
        bool HasRootPanel { get; }

        string RootOrientationName { get; }

        int ContentPaneCount { get; }

        int NestedLayoutPanelCount { get; }

        bool EnsurePrimaryPane();

        string ResolveSelectedDocumentContentId(ICollection<string> documentIds, string fallbackContentId);

        void ResetLayoutToPrimaryPane();

        bool UpsertDocumentInPrimaryPane(string documentId, ICollection<string> documentIds, Func<object, object> contentUpdater);

        void CloseStaleDocuments(ICollection<string> documentIds);

        void CloseDocuments(ICollection<string> documentIds);

        bool TryCloseDocumentFromSender(object sender, out string contentId);

        bool ArrangePanes(ICollection<string> documentIds, Orientation orientation);

        bool ArrangeGrid(ICollection<string> documentIds);

        bool SelectDocument(string documentId, ICollection<string> documentIds);

        bool SelectLastDocument(ICollection<string> documentIds);

        bool NormalizeComparisonPaneSizes();

        object FindDocumentContent(string documentId, ICollection<string> documentIds);

        IEnumerable<OpenVisionDockDocumentState> EnumerateDocumentStates(ICollection<string> documentIds);

        IEnumerable<OpenVisionDockDocumentLayoutEntry> CapturePaneLayout(ICollection<string> documentIds);

        bool RestorePaneLayout(ICollection<string> documentIds, IReadOnlyList<OpenVisionDockDocumentLayoutEntry> paneLayout);

        void RemoveEmptyPanes();
    }
}
