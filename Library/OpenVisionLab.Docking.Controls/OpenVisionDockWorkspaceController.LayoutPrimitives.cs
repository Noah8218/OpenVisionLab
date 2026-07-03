using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionDockWorkspaceController
    {
        private void ClearRootAndSetOrientation(Orientation orientation)
        {
            if (!HasRootPanel)
            {
                return;
            }

            dockingManager.Layout.RootPanel.Children.Clear();
            dockingManager.Layout.RootPanel.Orientation = orientation;
            primaryPane = null;
        }

        private void AddPane(LayoutAnchorablePane pane)
        {
            if (pane == null || !HasRootPanel)
            {
                return;
            }

            dockingManager.Layout.RootPanel.Children.Add(pane);
            primaryPane ??= pane;
        }

        private void AddPanel(LayoutPanel panel)
        {
            if (panel == null || !HasRootPanel)
            {
                return;
            }

            dockingManager.Layout.RootPanel.Children.Add(panel);
        }

        private void AddPaneToPanel(LayoutPanel panel, LayoutAnchorablePane pane)
        {
            if (panel == null || pane == null)
            {
                return;
            }

            panel.Children.Add(pane);
            primaryPane ??= pane;
        }

        private static void AddDocumentToPane(LayoutAnchorablePane pane, LayoutAnchorable document)
        {
            if (pane == null || document == null)
            {
                return;
            }

            pane.Children.Add(document);
        }

        private static void MoveDocumentToPane(LayoutAnchorable document, LayoutAnchorablePane targetPane)
        {
            if (document == null || targetPane == null)
            {
                return;
            }

            if (document.Parent is LayoutAnchorablePane sourcePane && !ReferenceEquals(sourcePane, targetPane))
            {
                sourcePane.Children.Remove(document);
                targetPane.Children.Add(document);
            }
        }

        private static LayoutAnchorablePane CreatePaneWithDocument(LayoutAnchorable document)
        {
            LayoutAnchorablePane pane = new LayoutAnchorablePane();
            if (document != null)
            {
                pane.Children.Add(document);
            }

            return pane;
        }

        private LayoutAnchorablePane GetOrCreatePane(List<LayoutAnchorablePane> panes, int paneIndex)
        {
            if (!HasRootPanel || panes == null)
            {
                return null;
            }

            paneIndex = Math.Max(0, paneIndex);
            while (panes.Count <= paneIndex)
            {
                LayoutAnchorablePane pane = new LayoutAnchorablePane();
                dockingManager.Layout.RootPanel.Children.Add(pane);
                panes.Add(pane);
            }

            return panes[paneIndex];
        }

        private static void DetachDocuments(IEnumerable<LayoutAnchorable> documents)
        {
            foreach (LayoutAnchorable document in documents ?? Enumerable.Empty<LayoutAnchorable>())
            {
                if (document.Parent is LayoutAnchorablePane pane)
                {
                    pane.Children.Remove(document);
                }
            }
        }

        private List<LayoutAnchorable> ResolveDocuments(ICollection<string> documentIds)
        {
            return (documentIds ?? Array.Empty<string>())
                .Select(title => FindDocument(title, documentIds))
                .Where(document => document != null)
                .ToList();
        }

        private static ILayoutPanelElement CreateExistingLayoutGroup(
            List<ILayoutPanelElement> existingElements,
            Orientation orientation)
        {
            if (existingElements == null || existingElements.Count == 0)
            {
                return new LayoutAnchorablePane();
            }

            if (existingElements.Count == 1)
            {
                return existingElements[0];
            }

            LayoutPanel group = new LayoutPanel
            {
                Orientation = orientation
            };
            foreach (ILayoutPanelElement element in existingElements)
            {
                group.Children.Add(element);
            }

            return group;
        }

        private static bool InsertPaneBesideTarget(
            LayoutAnchorablePane pane,
            LayoutAnchorablePane targetPane,
            Orientation orientation,
            bool insertBefore)
        {
            if (pane == null || targetPane == null || ReferenceEquals(pane, targetPane))
            {
                return false;
            }

            pane.DockWidth = new GridLength(1D, GridUnitType.Star);
            pane.DockHeight = new GridLength(1D, GridUnitType.Star);

            ILayoutPanelElement targetElement = ResolveSideSplitTargetElement(targetPane, orientation);
            if (targetElement is LayoutAnchorablePane targetPaneElement)
            {
                targetPaneElement.DockWidth = new GridLength(1D, GridUnitType.Star);
                targetPaneElement.DockHeight = new GridLength(1D, GridUnitType.Star);
            }

            if (targetElement?.Parent is not LayoutPanel parentPanel)
            {
                return false;
            }

            int targetIndex = parentPanel.Children.IndexOf(targetElement);
            if (targetIndex < 0)
            {
                return false;
            }

            bool shouldInsertIntoExistingPanel = parentPanel.Orientation == orientation
                && (parentPanel.Parent is not LayoutRoot || parentPanel.Children.Count <= 1);
            if (shouldInsertIntoExistingPanel)
            {
                parentPanel.Children.Insert(insertBefore ? targetIndex : targetIndex + 1, pane);
                return true;
            }

            LayoutPanel wrapperPanel = new LayoutPanel
            {
                Orientation = orientation
            };
            parentPanel.Children.RemoveAt(targetIndex);
            parentPanel.Children.Insert(targetIndex, wrapperPanel);
            if (insertBefore)
            {
                wrapperPanel.Children.Add(pane);
                wrapperPanel.Children.Add(targetElement);
            }
            else
            {
                wrapperPanel.Children.Add(targetElement);
                wrapperPanel.Children.Add(pane);
            }

            return true;
        }

        private static ILayoutPanelElement ResolveSideSplitTargetElement(
            LayoutAnchorablePane targetPane,
            Orientation requestedOrientation)
        {
            if (targetPane?.Parent is not LayoutPanel parentPanel)
            {
                return targetPane;
            }

            if (parentPanel.Orientation == requestedOrientation
                || parentPanel.Children.Count <= 1
                || parentPanel.Parent is LayoutRoot)
            {
                return targetPane;
            }

            return parentPanel;
        }

        private static bool RemovePanelChild(ILayoutPanelElement element)
        {
            if (element?.Parent is not LayoutPanel parentPanel)
            {
                return false;
            }

            return parentPanel.Children.Remove(element);
        }
    }
}
