using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionDockWorkspaceController
    {
        public bool UpsertDocumentInPrimaryPane(
            string documentId,
            ICollection<string> documentIds,
            Func<object, object> contentUpdater)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                return false;
            }

            LayoutAnchorablePane targetPane = GetPrimaryPane();
            if (targetPane == null)
            {
                return false;
            }

            LayoutAnchorable document = FindDocument(documentId, documentIds);
            if (document == null)
            {
                document = CreateDocument(documentId);
                AddDocumentToPane(targetPane, document);
            }

            UpdateDocument(document, documentId, contentUpdater);
            return true;
        }

        public void CloseStaleDocuments(ICollection<string> documentIds)
        {
            List<LayoutAnchorable> staleDocuments = EnumerateDocuments(documentIds)
                .Where(document => !ContainsDocumentId(documentIds, document.ContentId))
                .ToList();
            CloseDocuments(staleDocuments);
        }

        public void CloseDocuments(ICollection<string> documentIds)
        {
            CloseDocuments(EnumerateDocuments(documentIds).ToList());
            RemoveEmptyPanes();
        }

        public bool TryCloseDocumentFromSender(object sender, out string contentId)
        {
            contentId = string.Empty;
            if (sender is not LayoutAnchorable document || string.IsNullOrWhiteSpace(document.ContentId))
            {
                return false;
            }

            contentId = document.ContentId;
            DisposeDocumentContent(document);
            return true;
        }

        public bool SelectDocument(string documentId, ICollection<string> documentIds)
        {
            LayoutAnchorable document = FindDocument(documentId, documentIds);
            if (document == null)
            {
                return false;
            }

            SelectDocument(document);
            return true;
        }

        public bool SelectLastDocument(ICollection<string> documentIds)
        {
            LayoutAnchorable document = EnumerateHostedDocuments(documentIds).LastOrDefault();
            if (document == null)
            {
                return false;
            }

            SelectDocument(document);
            return true;
        }

        public object FindDocumentContent(string documentId, ICollection<string> documentIds)
        {
            return FindDocument(documentId, documentIds)?.Content;
        }

        public IEnumerable<OpenVisionDockDocumentState> EnumerateDocumentStates(ICollection<string> documentIds)
        {
            return EnumerateHostedDocuments(documentIds)
                .Select(document => new OpenVisionDockDocumentState(
                    document.ContentId,
                    document.CanFloat,
                    document.Content));
        }

        private LayoutAnchorable CreateDocument(string documentId)
        {
            LayoutAnchorable document = new LayoutAnchorable();
            ConfigureDocument(document, documentId);
            return document;
        }

        private void UpdateDocument(
            LayoutAnchorable document,
            string documentId,
            Func<object, object> contentUpdater)
        {
            if (document == null)
            {
                return;
            }

            ConfigureDocument(document, documentId);
            if (contentUpdater != null)
            {
                object updatedContent = contentUpdater(document.Content);
                if (!ReferenceEquals(updatedContent, document.Content))
                {
                    document.Content = updatedContent;
                }
            }
        }

        private void ConfigureDocument(LayoutAnchorable document, string documentId)
        {
            document.Title = documentId;
            document.ContentId = documentId;
            document.CanClose = false;
            document.CanHide = false;
            document.CanAutoHide = false;
            document.CanFloat = false;
            document.Closed -= documentClosedHandler;
            document.Closed += documentClosedHandler;
        }

        private void CloseDocuments(IEnumerable<LayoutAnchorable> documents)
        {
            foreach (LayoutAnchorable document in documents ?? Enumerable.Empty<LayoutAnchorable>())
            {
                document.Closed -= documentClosedHandler;
                DisposeDocumentContent(document);
                document.CanClose = true;
                document.Close();
            }
        }

        private static void DisposeDocumentContent(LayoutAnchorable document)
        {
            if (document?.Content is IDisposable disposable)
            {
                disposable.Dispose();
                document.Content = null;
            }
        }

        private static void SelectDocument(LayoutAnchorable document)
        {
            if (document == null)
            {
                return;
            }

            document.IsSelected = true;
            document.IsActive = true;
        }

        private LayoutAnchorable FindDocument(string documentId, ICollection<string> documentIds)
        {
            return EnumerateHostedDocuments(documentIds)
                .FirstOrDefault(document => string.Equals(document.ContentId, documentId, StringComparison.OrdinalIgnoreCase));
        }
    }
}
