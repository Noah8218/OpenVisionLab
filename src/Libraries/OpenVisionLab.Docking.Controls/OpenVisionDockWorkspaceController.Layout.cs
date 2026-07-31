using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionDockWorkspaceController
    {
        public bool SplitToNewPane(string documentId, ICollection<string> documentIds)
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

            if (sourcePane.Children.Count <= 1)
            {
                SelectDocument(document);
                return true;
            }

            LayoutAnchorablePane targetPane = new LayoutAnchorablePane();
            sourcePane.Children.Remove(document);
            SelectFirstDocumentInPane(sourcePane);
            targetPane.Children.Add(document);
            dockingManager.Layout.RootPanel.Children.Add(targetPane);
            SelectDocument(document);
            RemoveEmptyPanes();
            return true;
        }

        public bool MoveToPrimaryPane(string documentId, ICollection<string> documentIds)
        {
            return MoveToPane(documentId, documentIds, GetPrimaryPane());
        }

        public bool MoveToPane(string documentId, ICollection<string> documentIds, OpenVisionDockPaneHandle requestedTargetPane)
        {
            return MoveToPane(documentId, documentIds, ResolveLivePane(requestedTargetPane));
        }

        public bool MoveToPaneSide(
            string documentId,
            ICollection<string> documentIds,
            OpenVisionDockPaneHandle requestedTargetPane,
            Orientation orientation,
            bool insertBefore)
        {
            return MoveToPaneSide(documentId, documentIds, ResolveLivePane(requestedTargetPane), orientation, insertBefore);
        }

        public bool MoveToOuterPane(
            string documentId,
            ICollection<string> documentIds,
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

            sourcePane.Children.Remove(document);
            SelectFirstDocumentInPane(sourcePane);
            RemoveEmptyPanes();

            LayoutPanel rootPanel = dockingManager.Layout.RootPanel;
            LayoutAnchorablePane targetPane = CreatePaneWithDocument(document);
            if (rootPanel.Children.Count == 0)
            {
                rootPanel.Orientation = orientation;
                rootPanel.Children.Add(targetPane);
                primaryPane = targetPane;
                SelectDocument(document);
                return true;
            }

            if (rootPanel.Orientation == orientation)
            {
                if (insertBefore)
                {
                    rootPanel.Children.Insert(0, targetPane);
                }
                else
                {
                    rootPanel.Children.Add(targetPane);
                }
            }
            else
            {
                Orientation previousOrientation = rootPanel.Orientation;
                List<ILayoutPanelElement> existingElements = rootPanel.Children.ToList();
                rootPanel.Children.Clear();
                rootPanel.Orientation = orientation;

                ILayoutPanelElement existingGroup = CreateExistingLayoutGroup(existingElements, previousOrientation);
                if (insertBefore)
                {
                    rootPanel.Children.Add(targetPane);
                    rootPanel.Children.Add(existingGroup);
                }
                else
                {
                    rootPanel.Children.Add(existingGroup);
                    rootPanel.Children.Add(targetPane);
                }
            }

            primaryPane = EnumeratePanes().FirstOrDefault();
            SelectDocument(document);
            RemoveEmptyPanes();
            return true;
        }

        public bool ArrangePanes(ICollection<string> documentIds, Orientation orientation)
        {
            if (!HasRootPanel || documentIds == null || documentIds.Count == 0)
            {
                return false;
            }

            List<LayoutAnchorable> documents = ResolveDocuments(documentIds);
            if (documents.Count != documentIds.Count)
            {
                return false;
            }

            DetachDocuments(documents);
            ClearRootAndSetOrientation(orientation);

            foreach (LayoutAnchorable document in documents)
            {
                AddPane(CreatePaneWithDocument(document));
            }

            SelectDocument(documents.LastOrDefault());
            return true;
        }

        public bool ArrangeGrid(ICollection<string> documentIds)
        {
            if (!HasRootPanel || documentIds == null || documentIds.Count < 2)
            {
                return false;
            }

            List<LayoutAnchorable> documents = ResolveDocuments(documentIds);
            if (documents.Count != documentIds.Count)
            {
                return false;
            }

            DetachDocuments(documents);
            ClearRootAndSetOrientation(Orientation.Vertical);

            for (int index = 0; index < documents.Count; index += 2)
            {
                LayoutPanel rowPanel = new LayoutPanel
                {
                    Orientation = Orientation.Horizontal
                };

                for (int column = 0; column < 2 && index + column < documents.Count; column++)
                {
                    AddPaneToPanel(rowPanel, CreatePaneWithDocument(documents[index + column]));
                }

                AddPanel(rowPanel);
            }

            SetPrimaryPane(EnumeratePanes().FirstOrDefault());
            SelectDocument(documents.LastOrDefault());
            return true;
        }

    }
}
