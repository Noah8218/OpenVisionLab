using AvalonDock;
using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionDockWorkspaceController : IOpenVisionDockDocumentWorkspace
    {
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
    }
}
