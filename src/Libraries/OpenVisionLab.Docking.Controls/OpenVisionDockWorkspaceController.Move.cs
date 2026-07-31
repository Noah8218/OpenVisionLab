using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionDockWorkspaceController
    {
        private bool MoveToPane(string documentId, ICollection<string> documentIds, LayoutAnchorablePane requestedTargetPane)
        {
            if (string.IsNullOrWhiteSpace(documentId) || !HasRootPanel)
            {
                return false;
            }

            LayoutAnchorablePane targetPane = ResolveLivePane(requestedTargetPane) ?? GetPrimaryPane();
            LayoutAnchorable document = FindDocument(documentId, documentIds);
            if (targetPane == null || document == null)
            {
                return false;
            }

            if (document.Parent is LayoutAnchorablePane sourcePane && !ReferenceEquals(sourcePane, targetPane))
            {
                sourcePane.Children.Remove(document);
                SelectFirstDocumentInPane(sourcePane);
                targetPane.Children.Add(document);
                RemoveEmptyPanes();
            }

            SelectDocument(document);
            return true;
        }

        private bool MoveToPaneSide(
            string documentId,
            ICollection<string> documentIds,
            LayoutAnchorablePane requestedTargetPane,
            Orientation orientation,
            bool insertBefore)
        {
            if (string.IsNullOrWhiteSpace(documentId) || !HasRootPanel)
            {
                return false;
            }

            LayoutAnchorable document = FindDocument(documentId, documentIds);
            if (document?.Parent is not LayoutAnchorablePane sourcePane)
            {
                return false;
            }

            LayoutAnchorablePane targetPane = ResolveLivePane(requestedTargetPane) ?? GetPrimaryPane();
            if (targetPane == null)
            {
                return false;
            }

            if (ReferenceEquals(sourcePane, targetPane) && sourcePane.Children.Count <= 1)
            {
                SelectDocument(document);
                return true;
            }

            sourcePane.Children.Remove(document);
            SelectFirstDocumentInPane(sourcePane);
            LayoutAnchorablePane splitPane = CreatePaneWithDocument(document);
            if (!InsertPaneBesideTarget(splitPane, targetPane, orientation, insertBefore))
            {
                LayoutAnchorablePane fallbackPane = GetPrimaryPane();
                if (fallbackPane == null)
                {
                    return false;
                }

                fallbackPane.Children.Add(document);
                SelectDocument(document);
                return true;
            }

            SelectDocument(document);
            RemoveEmptyPanes();
            return true;
        }
    }
}
