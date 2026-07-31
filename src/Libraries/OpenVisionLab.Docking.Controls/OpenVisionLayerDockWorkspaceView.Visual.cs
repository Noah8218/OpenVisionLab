using AvalonDock.Controls;
using AvalonDock.Layout;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionLayerDockWorkspaceView
    {
        public OpenVisionDockingVisualSnapshot CreateVisualSnapshot()
        {
            if (layerDockingManager == null)
            {
                return OpenVisionDockingVisualSnapshot.Empty;
            }

            UpdateDockLayout();
            List<OpenVisionDockingVisualElementSnapshot> panes = EnumerateVisualDescendants<LayoutAnchorablePaneControl>(layerDockingManager)
                .Where(element => element.IsVisible)
                .Select((element, index) => new OpenVisionDockingVisualElementSnapshot(
                    "Pane",
                    ResolveElementTitle(element),
                    GetPaneControlBounds(element),
                    index))
                .Where(snapshot => IsUsableBounds(snapshot.Bounds))
                .OrderBy(snapshot => snapshot.Bounds.Top)
                .ThenBy(snapshot => snapshot.Bounds.Left)
                .Select((snapshot, index) => new OpenVisionDockingVisualElementSnapshot(
                    snapshot.Kind,
                    snapshot.Title,
                    snapshot.Bounds,
                    index))
                .ToList();

            List<OpenVisionDockingVisualElementSnapshot> tabHeaders = EnumerateVisualDescendants<LayoutAnchorableTabItem>(layerDockingManager)
                .Where(element => element.IsVisible)
                .Select(element => CreateElementSnapshot("TabHeader", element, panes))
                .Where(snapshot => IsUsableBounds(snapshot.Bounds))
                .ToList();

            List<OpenVisionDockingVisualElementSnapshot> paneHeaders = EnumerateVisualDescendants<FrameworkElement>(layerDockingManager)
                .Where(element => element.IsVisible)
                .Where(element => string.Equals(
                    AutomationProperties.GetAutomationId(element),
                    "DockedLayerPaneHeader",
                    StringComparison.Ordinal))
                .Select(element => CreateElementSnapshot("PaneHeader", element, panes))
                .Where(snapshot => IsUsableBounds(snapshot.Bounds))
                .ToList();

            return new OpenVisionDockingVisualSnapshot(
                WorkspaceSize,
                panes,
                tabHeaders,
                paneHeaders);
        }

        public IEnumerable<FrameworkElement> EnumerateGestureHeaders()
        {
            return EnumerateVisualDescendants<FrameworkElement>(layerDockingManager)
                .Where(element =>
                {
                    string typeName = element.GetType().Name;
                    return element.IsVisible
                        && (string.Equals(typeName, nameof(LayoutAnchorableTabItem), StringComparison.Ordinal)
                            || string.Equals(typeName, nameof(LayoutDocumentTabItem), StringComparison.Ordinal)
                            || string.Equals(typeName, nameof(AnchorablePaneTitle), StringComparison.Ordinal)
                            || string.Equals(typeName, "ToggleAnchorablePaneTitle", StringComparison.Ordinal)
                            || string.Equals(
                                AutomationProperties.GetAutomationId(element),
                                "DockedLayerPaneHeader",
                                StringComparison.Ordinal));
                });
        }

        public OpenVisionDockingHeaderDiagnostics CreateHeaderDiagnostics(int documentCount)
        {
            UpdateDockLayout();
            List<FrameworkElement> headers = EnumerateGestureHeaders().ToList();
            int requiredHeaderCount = Math.Min(2, Math.Max(0, documentCount));
            bool hasRequiredHeaders = headers.Count >= requiredHeaderCount;

            bool areGestureReady = hasRequiredHeaders
                && headers.All(header =>
                    header.Cursor == Cursors.SizeAll
                    && header.ActualWidth >= 72D
                    && header.ActualHeight >= 28D
                    && header.ToolTip != null);
            bool areReadable = hasRequiredHeaders
                && headers.All(header =>
                    header.ActualWidth >= 112D
                    && header.ActualHeight >= 28D
                    && HasReadableDockingHeaderTitle(header));
            bool areGripsReady = hasRequiredHeaders
                && headers.All(HasDockingHeaderGrip);
            string diagnosticsText = string.Join(" || ", headers
                .Select((header, index) => FormatDockingHeaderDiagnostic(index, header)));

            return new OpenVisionDockingHeaderDiagnostics(
                headers.Count,
                areGestureReady,
                areReadable,
                areGripsReady,
                diagnosticsText);
        }

        public FrameworkElement FindDockedLayerTabHeader()
        {
            FrameworkElement tabHeader = EnumerateVisualDescendants<FrameworkElement>(layerDockingManager)
                .FirstOrDefault(element =>
                    element.IsVisible
                    && string.Equals(
                AutomationProperties.GetAutomationId(element),
                        "DockedLayerTabHeader",
                        StringComparison.Ordinal));
            return tabHeader
                ?? EnumerateVisualDescendants<FrameworkElement>(layerDockingManager)
                    .FirstOrDefault(element =>
                        element.IsVisible
                        && string.Equals(
                            AutomationProperties.GetAutomationId(element),
                            "DockedLayerPaneHeader",
                            StringComparison.Ordinal))
                ?? EnumerateVisualDescendants<AnchorablePaneTitle>(layerDockingManager)
                    .FirstOrDefault(element => element.IsVisible);
        }

        private OpenVisionDockingVisualElementSnapshot CreateElementSnapshot(
            string kind,
            FrameworkElement element,
            IReadOnlyList<OpenVisionDockingVisualElementSnapshot> panes)
        {
            Rect bounds = GetPaneControlBounds(element);
            return new OpenVisionDockingVisualElementSnapshot(
                kind,
                ResolveElementTitle(element),
                bounds,
                ResolveContainingPaneIndex(bounds, panes));
        }

        private static bool IsUsableBounds(Rect bounds)
        {
            return !bounds.IsEmpty
                && !double.IsNaN(bounds.X)
                && !double.IsNaN(bounds.Y)
                && bounds.Width > 0D
                && bounds.Height > 0D;
        }

        private static int ResolveContainingPaneIndex(
            Rect bounds,
            IReadOnlyList<OpenVisionDockingVisualElementSnapshot> panes)
        {
            if (!IsUsableBounds(bounds) || panes == null || panes.Count == 0)
            {
                return -1;
            }

            Point center = new Point(bounds.Left + (bounds.Width * 0.5D), bounds.Top + (bounds.Height * 0.5D));
            for (int index = 0; index < panes.Count; index++)
            {
                if (panes[index].Bounds.Contains(center))
                {
                    return index;
                }
            }

            int bestIndex = -1;
            double bestArea = 0D;
            for (int index = 0; index < panes.Count; index++)
            {
                Rect intersection = Rect.Intersect(bounds, panes[index].Bounds);
                if (intersection.IsEmpty)
                {
                    continue;
                }

                double area = intersection.Width * intersection.Height;
                if (area > bestArea)
                {
                    bestArea = area;
                    bestIndex = index;
                }
            }

            return bestIndex;
        }

        private static string ResolveElementTitle(FrameworkElement element)
        {
            string modelTitle = ResolveModelTitle(ResolveElementModel(element));
            if (!string.IsNullOrWhiteSpace(modelTitle))
            {
                return modelTitle;
            }

            string automationName = AutomationProperties.GetName(element);
            return string.IsNullOrWhiteSpace(automationName)
                ? element?.GetType().Name ?? string.Empty
                : automationName;
        }

        private static object ResolveElementModel(FrameworkElement element)
        {
            if (element is LayoutAnchorableTabItem tabItem)
            {
                return tabItem.Model;
            }

            if (element is LayoutAnchorablePaneControl paneControl)
            {
                return paneControl.Model;
            }

            if (element is AnchorablePaneTitle paneTitle)
            {
                return paneTitle.Model;
            }

            return element?.DataContext;
        }

        private static string ResolveModelTitle(object model)
        {
            if (model is LayoutContent layoutContent)
            {
                if (!string.IsNullOrWhiteSpace(layoutContent.Title))
                {
                    return layoutContent.Title;
                }

                if (!string.IsNullOrWhiteSpace(layoutContent.ContentId))
                {
                    return layoutContent.ContentId;
                }
            }

            return model?.ToString() ?? string.Empty;
        }

        private static bool HasReadableDockingHeaderTitle(FrameworkElement header)
        {
            return EnumerateVisualDescendants<TextBlock>(header)
                .Any(textBlock =>
                    textBlock.IsVisible
                    && textBlock.ActualWidth >= 8D
                    && !string.IsNullOrWhiteSpace(textBlock.Text)
                    && !string.Equals(textBlock.Text, "-", StringComparison.Ordinal));
        }

        private static bool HasDockingHeaderGrip(FrameworkElement header)
        {
            return EnumerateVisualDescendants<PackIconMaterial>(header)
                .Any(icon => icon.Kind == PackIconMaterialKind.DragHorizontalVariant
                    || icon.Kind == PackIconMaterialKind.DragVariant
                    || icon.Kind == PackIconMaterialKind.CursorMove);
        }

        private static string FormatDockingHeaderDiagnostic(int index, FrameworkElement header)
        {
            string title = string.Join("/", EnumerateVisualDescendants<TextBlock>(header)
                .Where(textBlock => textBlock.IsVisible && !string.IsNullOrWhiteSpace(textBlock.Text))
                .Select(textBlock => textBlock.Text.Trim()));
            return string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "#{0}:{1},Visible={2},W={3:0.0},H={4:0.0},Cursor={5},ToolTip={6},Grip={7},Title={8}",
                index,
                header?.GetType().Name ?? string.Empty,
                header?.IsVisible == true,
                header?.ActualWidth ?? 0D,
                header?.ActualHeight ?? 0D,
                header?.Cursor,
                header?.ToolTip != null,
                HasDockingHeaderGrip(header),
                string.IsNullOrWhiteSpace(title) ? "<empty>" : title);
        }

        private static IEnumerable<T> EnumerateVisualDescendants<T>(DependencyObject element)
            where T : DependencyObject
        {
            if (element == null)
            {
                yield break;
            }

            int childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is T match)
                {
                    yield return match;
                }

                foreach (T descendant in EnumerateVisualDescendants<T>(child))
                {
                    yield return descendant;
                }
            }
        }
    }
}
