using AvalonDock;
using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockWorkspaceController : IOpenVisionDockDocumentWorkspace
    {
        #region Core

        private const double MinimumComparisonPaneWidth = 280D;
        private const double MinimumComparisonPaneHeight = 180D;

        private readonly DockingManager dockingManager;
        private readonly EventHandler documentClosedHandler;
        private readonly Predicate<object> documentContentPredicate;
        private LayoutAnchorablePane primaryPane;

        public OpenVisionDockWorkspaceController(
            OpenVisionDockWorkspaceHandle workspaceHandle,
            EventHandler documentClosedHandler,
            Predicate<object> documentContentPredicate)
            : this(
                workspaceHandle?.NativeWorkspace as DockingManager,
                workspaceHandle?.NativePrimaryPane as LayoutAnchorablePane,
                documentClosedHandler,
                documentContentPredicate)
        {
        }

        private OpenVisionDockWorkspaceController(
            DockingManager dockingManager,
            LayoutAnchorablePane initialPane,
            EventHandler documentClosedHandler,
            Predicate<object> documentContentPredicate)
        {
            this.dockingManager = dockingManager;
            primaryPane = initialPane;
            this.documentClosedHandler = documentClosedHandler;
            this.documentContentPredicate = documentContentPredicate;
        }

        public bool HasRootPanel => dockingManager?.Layout?.RootPanel != null;

        public string RootOrientationName => dockingManager?.Layout?.RootPanel?.Orientation.ToString() ?? string.Empty;

        public int ContentPaneCount => EnumeratePanes()
            .Count(pane => pane.Children.Any(document => IsDocumentContent(document.Content)));

        public int NestedLayoutPanelCount => EnumerateLayoutElements()
            .OfType<LayoutPanel>()
            .Count(panel => !ReferenceEquals(panel, dockingManager?.Layout?.RootPanel));

        public IEnumerable<OpenVisionDockPaneHandle> EnumeratePaneHandles()
        {
            return EnumeratePanes().Select(OpenVisionDockPaneHandle.FromNative);
        }

        public OpenVisionDockPaneHandle GetPrimaryPaneHandle()
        {
            return OpenVisionDockPaneHandle.FromNative(GetPrimaryPane());
        }

        public bool EnsurePrimaryPane()
        {
            return GetPrimaryPane() != null;
        }

        public string ResolveSelectedDocumentContentId(ICollection<string> documentIds, string fallbackContentId)
        {
            LayoutAnchorablePane targetPane = GetPrimaryPane();
            return EnumerateHostedDocuments(documentIds)
                .FirstOrDefault(document => document.IsActive)?.ContentId
                ?? targetPane?.SelectedContent?.ContentId
                ?? EnumerateHostedDocuments(documentIds)
                    .FirstOrDefault(document => document.IsSelected)?.ContentId
                ?? fallbackContentId
                ?? string.Empty;
        }

        public void ResetLayoutToPrimaryPane()
        {
            if (!HasRootPanel)
            {
                return;
            }

            dockingManager.Layout.RootPanel.Children.Clear();
            primaryPane = new LayoutAnchorablePane();
            dockingManager.Layout.RootPanel.Children.Add(primaryPane);
        }

        #endregion

        #region Documents

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
            document.CanClose = true;
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

        #endregion

        #region Layout

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

        #endregion

        #region LayoutPrimitives

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

        #endregion

        #region Move

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

        #endregion

        #region Native

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

        #endregion

        #region Normalize

public bool NormalizeComparisonPaneSizes()
        {
            if (!HasRootPanel)
            {
                return false;
            }

            bool changed = false;
            List<LayoutAnchorablePane> contentPanes = EnumeratePanes()
                .Where(HasDocumentContent)
                .ToList();

            foreach (LayoutAnchorablePane pane in contentPanes)
            {
                changed |= EnsurePaneMinimums(pane);
            }

            foreach (LayoutPanel panel in EnumerateLayoutElements().OfType<LayoutPanel>())
            {
                List<LayoutAnchorablePane> panelContentPanes = panel.Children
                    .OfType<LayoutAnchorablePane>()
                    .Where(HasDocumentContent)
                    .ToList();

                if (panelContentPanes.Count <= 1)
                {
                    continue;
                }

                if (panel.Orientation == Orientation.Horizontal
                    && panelContentPanes.Any(IsPaneTooNarrowForComparison))
                {
                    foreach (LayoutAnchorablePane pane in panelContentPanes)
                    {
                        pane.DockWidth = new GridLength(1D, GridUnitType.Star);
                    }

                    changed = true;
                }

                if (panel.Orientation == Orientation.Vertical
                    && panelContentPanes.Any(IsPaneTooShortForComparison))
                {
                    foreach (LayoutAnchorablePane pane in panelContentPanes)
                    {
                        pane.DockHeight = new GridLength(1D, GridUnitType.Star);
                    }

                    changed = true;
                }
            }

            return changed;
        }

        private bool HasDocumentContent(LayoutAnchorablePane pane)
        {
            return pane?.Children
                .OfType<LayoutAnchorable>()
                .Any(document => IsDocumentContent(document.Content)) == true;
        }

        private static bool EnsurePaneMinimums(LayoutAnchorablePane pane)
        {
            bool changed = false;
            if (pane.DockMinWidth < MinimumComparisonPaneWidth)
            {
                pane.DockMinWidth = MinimumComparisonPaneWidth;
                changed = true;
            }

            if (pane.DockMinHeight < MinimumComparisonPaneHeight)
            {
                pane.DockMinHeight = MinimumComparisonPaneHeight;
                changed = true;
            }

            if (pane.DockWidth.IsAbsolute && pane.DockWidth.Value < MinimumComparisonPaneWidth)
            {
                pane.DockWidth = new GridLength(MinimumComparisonPaneWidth);
                changed = true;
            }

            if (pane.DockHeight.IsAbsolute && pane.DockHeight.Value < MinimumComparisonPaneHeight)
            {
                pane.DockHeight = new GridLength(MinimumComparisonPaneHeight);
                changed = true;
            }

            return changed;
        }

        private bool IsPaneTooNarrowForComparison(LayoutAnchorablePane pane)
        {
            if (pane.DockWidth.IsAbsolute && pane.DockWidth.Value < MinimumComparisonPaneWidth)
            {
                return true;
            }

            return pane.Children
                .OfType<LayoutAnchorable>()
                .Select(document => document.Content)
                .Any(IsContentTooNarrowForComparison);
        }

        private bool IsPaneTooShortForComparison(LayoutAnchorablePane pane)
        {
            if (pane.DockHeight.IsAbsolute && pane.DockHeight.Value < MinimumComparisonPaneHeight)
            {
                return true;
            }

            return pane.Children
                .OfType<LayoutAnchorable>()
                .Select(document => document.Content)
                .Any(IsContentTooShortForComparison);
        }

        private bool IsContentTooNarrowForComparison(object content)
        {
            return IsDocumentContent(content)
                && content is FrameworkElement element
                && element.ActualWidth > 0D
                && element.ActualWidth < MinimumComparisonPaneWidth;
        }

        private bool IsContentTooShortForComparison(object content)
        {
            return IsDocumentContent(content)
                && content is FrameworkElement element
                && element.ActualHeight > 0D
                && element.ActualHeight < MinimumComparisonPaneHeight;
        }

        #endregion

        #region State

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

        #endregion

        #region Cleanup

public void RemoveEmptyPanes()
        {
            if (!HasRootPanel)
            {
                return;
            }

            bool changed;
            do
            {
                changed = false;
                List<LayoutAnchorablePane> panes = EnumeratePanes().ToList();
                foreach (LayoutAnchorablePane pane in panes.Where(pane => pane.Children.Count == 0).ToList())
                {
                    if (panes.Count <= 1)
                    {
                        break;
                    }

                    if (RemovePanelChild(pane))
                    {
                        if (ReferenceEquals(primaryPane, pane))
                        {
                            primaryPane = null;
                        }

                        changed = true;
                    }
                }

                foreach (LayoutPanel panel in EnumerateLayoutElements()
                    .OfType<LayoutPanel>()
                    .Where(panel => !ReferenceEquals(panel, dockingManager.Layout.RootPanel) && panel.Children.Count == 0)
                    .ToList())
                {
                    changed |= RemovePanelChild(panel);
                }

                foreach (LayoutPanel panel in EnumerateLayoutElements()
                    .OfType<LayoutPanel>()
                    .Where(panel => !ReferenceEquals(panel, dockingManager.Layout.RootPanel) && panel.Children.Count == 1)
                    .ToList())
                {
                    changed |= CollapseSingleChildPanel(panel);
                }
            }
            while (changed);

            primaryPane = ResolveLivePane(primaryPane) ?? EnumeratePanes().FirstOrDefault();
        }

        private static bool CollapseSingleChildPanel(LayoutPanel panel)
        {
            if (panel?.Parent is not LayoutPanel parentPanel || panel.Children.Count != 1)
            {
                return false;
            }

            int panelIndex = parentPanel.Children.IndexOf(panel);
            if (panelIndex < 0)
            {
                return false;
            }

            ILayoutPanelElement onlyChild = panel.Children[0];
            panel.Children.RemoveAt(0);
            parentPanel.Children.RemoveAt(panelIndex);
            parentPanel.Children.Insert(panelIndex, onlyChild);
            return true;
        }

        #endregion
    }
}
