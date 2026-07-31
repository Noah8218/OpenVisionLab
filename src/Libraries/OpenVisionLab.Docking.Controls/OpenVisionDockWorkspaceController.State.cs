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
        public IEnumerable<OpenVisionDockDocumentLayoutEntry> CapturePaneLayout(ICollection<string> documentIds)
        {
            if (!HasRootPanel || documentIds == null || documentIds.Count == 0)
            {
                return Enumerable.Empty<OpenVisionDockDocumentLayoutEntry>();
            }

            List<OpenVisionDockDocumentLayoutEntry> paneMap = new List<OpenVisionDockDocumentLayoutEntry>();
            List<LayoutAnchorablePane> panes = EnumeratePanes().ToList();
            for (int paneIndex = 0; paneIndex < panes.Count; paneIndex++)
            {
                string layoutPath = ResolvePaneLayoutPath(panes[paneIndex], paneIndex);
                foreach (LayoutAnchorable document in panes[paneIndex].Children.OfType<LayoutAnchorable>())
                {
                    if (!string.IsNullOrWhiteSpace(document.ContentId)
                        && documentIds.Contains(document.ContentId, StringComparer.OrdinalIgnoreCase))
                    {
                        paneMap.Add(new OpenVisionDockDocumentLayoutEntry(document.ContentId, paneIndex, layoutPath));
                    }
                }
            }

            return paneMap;
        }

        public bool RestorePaneLayout(ICollection<string> documentIds, IReadOnlyList<OpenVisionDockDocumentLayoutEntry> paneLayout)
        {
            if (!HasRootPanel || documentIds == null || documentIds.Count == 0 || paneLayout == null || paneLayout.Count == 0)
            {
                return false;
            }

            Dictionary<string, OpenVisionDockDocumentLayoutEntry> layoutByLayer = paneLayout
                .Where(entry => !string.IsNullOrWhiteSpace(entry.LayerTitle)
                    && documentIds.Contains(entry.LayerTitle, StringComparer.OrdinalIgnoreCase))
                .GroupBy(entry => entry.LayerTitle, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
            if (layoutByLayer.Count == 0)
            {
                return false;
            }

            Dictionary<string, LayoutAnchorable> documents = documentIds
                .Select(title => FindDocument(title, documentIds))
                .Where(document => document != null && !string.IsNullOrWhiteSpace(document.ContentId))
                .ToDictionary(document => document.ContentId, document => document, StringComparer.OrdinalIgnoreCase);
            if (documents.Count == 0)
            {
                return false;
            }

            if (TryRestorePaneLayoutWithMovePrimitives(documentIds, layoutByLayer))
            {
                return true;
            }

            DetachDocuments(documents.Values);
            ClearRootAndSetOrientation(ResolveRootOrientation(layoutByLayer.Values));
            Dictionary<string, LayoutAnchorablePane> paneByPath = new Dictionary<string, LayoutAnchorablePane>(StringComparer.OrdinalIgnoreCase);

            foreach (string title in documentIds)
            {
                if (!documents.TryGetValue(title, out LayoutAnchorable document))
                {
                    continue;
                }

                OpenVisionDockDocumentLayoutEntry entry = layoutByLayer.TryGetValue(title, out OpenVisionDockDocumentLayoutEntry restoredEntry)
                    ? restoredEntry
                    : new OpenVisionDockDocumentLayoutEntry(title, 0, CreateFlatLayoutPath(0));
                string layoutPath = string.IsNullOrWhiteSpace(entry.LayoutPath)
                    ? CreateFlatLayoutPath(entry.PaneIndex)
                    : entry.LayoutPath;
                LayoutAnchorablePane targetPane = GetOrCreatePaneForLayoutPath(
                    dockingManager.Layout.RootPanel,
                    paneByPath,
                    layoutPath,
                    Math.Max(0, entry.PaneIndex));
                if (targetPane != null && !targetPane.Children.Contains(document))
                {
                    targetPane.Children.Add(document);
                }
            }

            RemoveEmptyPanes();
            NormalizeRestoredPaneSizes();
            SelectFirstDocumentInEachPane();
            SetPrimaryPane(EnumeratePanes().FirstOrDefault());
            dockingManager.Layout.CollectGarbage();
            return true;
        }

        private bool TryRestorePaneLayoutWithMovePrimitives(
            ICollection<string> documentIds,
            IReadOnlyDictionary<string, OpenVisionDockDocumentLayoutEntry> layoutByLayer)
        {
            if (!HasRootPanel || documentIds == null || layoutByLayer == null || layoutByLayer.Count < 2)
            {
                return false;
            }

            Dictionary<string, List<LayoutPathSegment>> parsedPaths = layoutByLayer
                .ToDictionary(
                    pair => pair.Key,
                    pair => ParseLayoutPath(pair.Value.LayoutPath),
                    StringComparer.OrdinalIgnoreCase);
            if (parsedPaths.Values.Any(path => path.Count == 0))
            {
                return false;
            }

            LayoutAnchorablePane primary = GetPrimaryPane();
            if (primary == null)
            {
                return false;
            }

            foreach (string documentId in documentIds.Where(layoutByLayer.ContainsKey).ToList())
            {
                if (!MoveToPane(documentId, documentIds, primary))
                {
                    return false;
                }
            }

            Orientation rootOrientation = ResolveRootOrientation(layoutByLayer.Values);
            Orientation sideOrientation = rootOrientation == Orientation.Horizontal
                ? Orientation.Vertical
                : Orientation.Horizontal;

            List<KeyValuePair<string, List<LayoutPathSegment>>> orderedPaths = parsedPaths
                .OrderBy(pair => pair.Value[0].ChildIndex)
                .ThenBy(pair => pair.Value.Count > 1 ? pair.Value[1].ChildIndex : 0)
                .ToList();

            foreach (KeyValuePair<string, List<LayoutPathSegment>> pair in orderedPaths.Where(pair => pair.Value[0].ChildIndex > 0))
            {
                if (!MoveToOuterPane(pair.Key, documentIds, rootOrientation, insertBefore: false))
                {
                    return false;
                }
            }

            LayoutAnchorablePane targetPane = GetPrimaryPane();
            foreach (KeyValuePair<string, List<LayoutPathSegment>> pair in orderedPaths
                .Where(pair => pair.Value[0].ChildIndex == 0
                    && pair.Value.Count > 1
                    && pair.Value[1].Orientation == sideOrientation
                    && pair.Value[1].ChildIndex > 0))
            {
                if (!MoveToPaneSide(pair.Key, documentIds, targetPane, sideOrientation, insertBefore: false))
                {
                    return false;
                }
            }

            RemoveEmptyPanes();
            NormalizeRestoredPaneSizes();
            SelectFirstDocumentInEachPane();
            SetPrimaryPane(EnumeratePanes().FirstOrDefault());
            dockingManager.Layout.CollectGarbage();
            return ContentPaneCount >= layoutByLayer.Values
                .Select(entry => string.IsNullOrWhiteSpace(entry.LayoutPath)
                    ? CreateFlatLayoutPath(entry.PaneIndex)
                    : entry.LayoutPath)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();
        }

        private void NormalizeRestoredPaneSizes()
        {
            foreach (LayoutAnchorablePane pane in EnumeratePanes().Where(HasDocumentContent))
            {
                EnsurePaneMinimums(pane);
            }

            foreach (LayoutPanel panel in EnumerateLayoutElements().OfType<LayoutPanel>())
            {
                List<LayoutAnchorablePane> contentPanes = panel.Children
                    .OfType<LayoutAnchorablePane>()
                    .Where(HasDocumentContent)
                    .ToList();
                if (contentPanes.Count <= 1)
                {
                    continue;
                }

                foreach (LayoutAnchorablePane pane in contentPanes)
                {
                    if (panel.Orientation == Orientation.Horizontal)
                    {
                        pane.DockWidth = new GridLength(1D, GridUnitType.Star);
                    }
                    else
                    {
                        pane.DockHeight = new GridLength(1D, GridUnitType.Star);
                    }
                }
            }
        }

        private void SelectFirstDocumentInEachPane()
        {
            LayoutAnchorable lastSelectedDocument = null;
            foreach (LayoutAnchorablePane pane in EnumeratePanes())
            {
                if (!SelectFirstDocumentInPane(pane, out LayoutAnchorable firstDocument))
                {
                    continue;
                }

                lastSelectedDocument = firstDocument;
            }

            if (lastSelectedDocument != null)
            {
                lastSelectedDocument.IsActive = true;
            }
        }

        private static bool SelectFirstDocumentInPane(LayoutAnchorablePane pane)
        {
            return SelectFirstDocumentInPane(pane, out _);
        }

        private static bool SelectFirstDocumentInPane(LayoutAnchorablePane pane, out LayoutAnchorable firstDocument)
        {
            firstDocument = pane?.Children
                .OfType<LayoutAnchorable>()
                .FirstOrDefault();
            if (firstDocument == null)
            {
                return false;
            }

            firstDocument.IsSelected = true;
            firstDocument.IsActive = true;
            return true;
        }

        private static string ResolvePaneLayoutPath(LayoutAnchorablePane pane, int fallbackPaneIndex)
        {
            if (pane == null)
            {
                return CreateFlatLayoutPath(fallbackPaneIndex);
            }

            Stack<string> segments = new Stack<string>();
            ILayoutPanelElement current = pane;
            while (current is ILayoutElement currentElement && currentElement.Parent is LayoutPanel parentPanel)
            {
                int childIndex = parentPanel.Children.IndexOf(current);
                if (childIndex < 0)
                {
                    break;
                }

                segments.Push(FormatLayoutSegment(parentPanel.Orientation, childIndex));
                current = parentPanel;
            }

            return segments.Count == 0
                ? CreateFlatLayoutPath(fallbackPaneIndex)
                : string.Join("/", segments);
        }

        private static LayoutAnchorablePane GetOrCreatePaneForLayoutPath(
            LayoutPanel rootPanel,
            IDictionary<string, LayoutAnchorablePane> paneByPath,
            string layoutPath,
            int fallbackPaneIndex)
        {
            if (rootPanel == null)
            {
                return null;
            }

            string normalizedPath = string.IsNullOrWhiteSpace(layoutPath)
                ? CreateFlatLayoutPath(fallbackPaneIndex)
                : layoutPath.Trim();
            if (paneByPath.TryGetValue(normalizedPath, out LayoutAnchorablePane existingPane))
            {
                return existingPane;
            }

            List<LayoutPathSegment> segments = ParseLayoutPath(normalizedPath);
            if (segments.Count == 0)
            {
                segments.Add(new LayoutPathSegment(Orientation.Horizontal, Math.Max(0, fallbackPaneIndex)));
            }

            LayoutPanel currentPanel = rootPanel;
            for (int index = 0; index < segments.Count; index++)
            {
                LayoutPathSegment segment = segments[index];
                currentPanel.Orientation = segment.Orientation;
                EnsurePanelChildCount(currentPanel, segment.ChildIndex + 1);

                bool isLast = index == segments.Count - 1;
                ILayoutPanelElement child = currentPanel.Children[segment.ChildIndex];
                if (isLast)
                {
                    if (child is LayoutAnchorablePane pane)
                    {
                        paneByPath[normalizedPath] = pane;
                        return pane;
                    }

                    LayoutAnchorablePane replacementPane = new LayoutAnchorablePane();
                    ReplacePanelChild(currentPanel, segment.ChildIndex, replacementPane);
                    paneByPath[normalizedPath] = replacementPane;
                    return replacementPane;
                }

                Orientation nextOrientation = segments[index + 1].Orientation;
                if (child is LayoutPanel childPanel)
                {
                    childPanel.Orientation = nextOrientation;
                    currentPanel = childPanel;
                    continue;
                }

                LayoutPanel replacementPanel = new LayoutPanel
                {
                    Orientation = nextOrientation
                };
                ReplacePanelChild(currentPanel, segment.ChildIndex, replacementPanel);
                currentPanel = replacementPanel;
            }

            return null;
        }

        private static Orientation ResolveRootOrientation(IEnumerable<OpenVisionDockDocumentLayoutEntry> entries)
        {
            foreach (OpenVisionDockDocumentLayoutEntry entry in entries ?? Enumerable.Empty<OpenVisionDockDocumentLayoutEntry>())
            {
                List<LayoutPathSegment> segments = ParseLayoutPath(entry.LayoutPath);
                if (segments.Count > 0)
                {
                    return segments[0].Orientation;
                }
            }

            return Orientation.Horizontal;
        }

        private static List<LayoutPathSegment> ParseLayoutPath(string layoutPath)
        {
            List<LayoutPathSegment> segments = new List<LayoutPathSegment>();
            if (string.IsNullOrWhiteSpace(layoutPath))
            {
                return segments;
            }

            foreach (string rawSegment in layoutPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string segment = rawSegment.Trim();
                if (segment.Length < 2)
                {
                    continue;
                }

                Orientation orientation = char.ToUpperInvariant(segment[0]) == 'V'
                    ? Orientation.Vertical
                    : Orientation.Horizontal;
                if (!int.TryParse(segment.Substring(1), out int childIndex))
                {
                    childIndex = 0;
                }

                segments.Add(new LayoutPathSegment(orientation, Math.Max(0, childIndex)));
            }

            return segments;
        }

        private static void EnsurePanelChildCount(LayoutPanel panel, int count)
        {
            if (panel == null)
            {
                return;
            }

            while (panel.Children.Count < count)
            {
                panel.Children.Add(new LayoutAnchorablePane());
            }
        }

        private static void ReplacePanelChild(LayoutPanel panel, int childIndex, ILayoutPanelElement replacement)
        {
            if (panel == null || replacement == null || childIndex < 0 || childIndex >= panel.Children.Count)
            {
                return;
            }

            panel.Children.RemoveAt(childIndex);
            panel.Children.Insert(childIndex, replacement);
        }

        private static string CreateFlatLayoutPath(int paneIndex)
        {
            return FormatLayoutSegment(Orientation.Horizontal, Math.Max(0, paneIndex));
        }

        private static string FormatLayoutSegment(Orientation orientation, int childIndex)
        {
            return (orientation == Orientation.Vertical ? "V" : "H") + Math.Max(0, childIndex).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        private readonly struct LayoutPathSegment
        {
            public LayoutPathSegment(Orientation orientation, int childIndex)
            {
                Orientation = orientation;
                ChildIndex = childIndex;
            }

            public Orientation Orientation { get; }

            public int ChildIndex { get; }
        }
    }
}
