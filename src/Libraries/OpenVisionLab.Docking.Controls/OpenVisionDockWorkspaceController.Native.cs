using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionDockWorkspaceController
    {
        private IEnumerable<LayoutAnchorablePane> EnumeratePanes()
        {
            return EnumerateLayoutElements()
                .OfType<LayoutAnchorablePane>();
        }

        private IEnumerable<LayoutAnchorable> EnumerateDocuments(ICollection<string> documentIds)
        {
            return EnumerateLayoutElements()
                .OfType<LayoutAnchorable>()
                .Where(document => IsDocumentContent(document.Content)
                    || ContainsDocumentId(documentIds, document.ContentId));
        }

        private IEnumerable<LayoutAnchorable> EnumerateHostedDocuments(ICollection<string> documentIds)
        {
            return EnumerateDocuments(documentIds)
                .Where(IsHostedDocument);
        }

        private LayoutAnchorablePane GetPrimaryPane()
        {
            if (primaryPane?.Root == dockingManager?.Layout)
            {
                return primaryPane;
            }

            LayoutAnchorablePane existingPane = EnumeratePanes().FirstOrDefault();
            if (existingPane != null)
            {
                primaryPane = existingPane;
                return existingPane;
            }

            if (!HasRootPanel)
            {
                return null;
            }

            primaryPane = new LayoutAnchorablePane();
            dockingManager.Layout.RootPanel.Children.Add(primaryPane);
            return primaryPane;
        }

        private void SetPrimaryPane(LayoutAnchorablePane pane)
        {
            primaryPane = pane;
        }

        private IEnumerable<ILayoutElement> EnumerateLayoutElements()
        {
            return EnumerateLayoutElements(dockingManager?.Layout);
        }

        private static IEnumerable<ILayoutElement> EnumerateLayoutElements(ILayoutElement element)
        {
            if (element == null)
            {
                yield break;
            }

            yield return element;
            if (element is ILayoutContainer container)
            {
                foreach (ILayoutElement child in container.Children.ToList())
                {
                    foreach (ILayoutElement descendant in EnumerateLayoutElements(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        private static bool ContainsDocumentId(ICollection<string> documentIds, string documentId)
        {
            return !string.IsNullOrWhiteSpace(documentId)
                && documentIds != null
                && documentIds.Contains(documentId, StringComparer.OrdinalIgnoreCase);
        }

        private LayoutAnchorablePane ResolveLivePane(LayoutAnchorablePane pane)
        {
            if (pane == null)
            {
                return null;
            }

            return EnumeratePanes().FirstOrDefault(current => ReferenceEquals(current, pane));
        }

        private LayoutAnchorablePane ResolveLivePane(OpenVisionDockPaneHandle paneHandle)
        {
            return ResolveLivePane(paneHandle?.NativePane as LayoutAnchorablePane);
        }

        private bool IsDocumentContent(object content)
        {
            return documentContentPredicate?.Invoke(content) == true;
        }

        private static bool IsHostedDocument(LayoutAnchorable document)
        {
            return document?.Parent is LayoutAnchorablePane;
        }
    }
}
