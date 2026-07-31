using AvalonDock.Layout;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionDockWorkspaceController
    {
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
    }
}
