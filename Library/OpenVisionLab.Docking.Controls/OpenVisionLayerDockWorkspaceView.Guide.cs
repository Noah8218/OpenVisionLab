using AvalonDock.Controls;
using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionLayerDockWorkspaceView
    {
        public bool IsGuideOverlayHitTestSafe => true;

        public int GuideZoneCount => dockingGuideOverlay?.GuideZoneCount ?? 0;

        public void ResetPaneGuideMargin()
        {
            PaneGuideMargin = DefaultPaneGuideMargin;
        }

        public OpenVisionDockPaneHandle ResolveTargetPane(
            Point point,
            IEnumerable<OpenVisionDockPaneHandle> paneHandles,
            OpenVisionDockPaneHandle fallbackPane)
        {
            if (layerDockingManager == null)
            {
                return fallbackPane ?? OpenVisionDockPaneHandle.Empty;
            }

            List<LayoutAnchorablePane> panes = (paneHandles ?? Enumerable.Empty<OpenVisionDockPaneHandle>())
                .Select(handle => handle?.NativePane)
                .OfType<LayoutAnchorablePane>()
                .ToList();
            if (panes.Count == 0)
            {
                return fallbackPane ?? OpenVisionDockPaneHandle.Empty;
            }

            LayoutAnchorablePane nearestPane = null;
            double nearestDistance = double.MaxValue;
            foreach (LayoutAnchorablePaneControl paneControl in EnumerateVisualDescendants<LayoutAnchorablePaneControl>(layerDockingManager))
            {
                if (!paneControl.IsVisible
                    || paneControl.Model is not LayoutAnchorablePane pane
                    || !panes.Any(current => ReferenceEquals(current, pane)))
                {
                    continue;
                }

                Rect bounds = GetPaneControlBounds(paneControl);
                if (bounds.IsEmpty)
                {
                    continue;
                }

                if (bounds.Contains(point))
                {
                    return OpenVisionDockPaneHandle.FromNative(pane);
                }

                double centerX = bounds.Left + (bounds.Width * 0.5D);
                double centerY = bounds.Top + (bounds.Height * 0.5D);
                double distance = Math.Pow(point.X - centerX, 2D) + Math.Pow(point.Y - centerY, 2D);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPane = pane;
                }
            }

            return nearestPane == null
                ? fallbackPane ?? OpenVisionDockPaneHandle.Empty
                : OpenVisionDockPaneHandle.FromNative(nearestPane);
        }

        public DockingGuideZone ResolveGuideZone(Point point, OpenVisionDockPaneHandle targetPane)
        {
            if (layerDockingManager == null || layerDockingManager.ActualWidth <= 0D || layerDockingManager.ActualHeight <= 0D)
            {
                return DockingGuideZone.Center;
            }

            return OpenVisionLayerDockingGuidePolicy.ResolveZone(
                point,
                new Size(layerDockingManager.ActualWidth, layerDockingManager.ActualHeight),
                GetPaneBounds(targetPane));
        }

        public void PositionPaneGuideOverlay(OpenVisionDockPaneHandle targetPane)
        {
            Rect bounds = GetPaneBounds(targetPane);
            if (bounds.IsEmpty || bounds.Width <= 0D || bounds.Height <= 0D)
            {
                ResetPaneGuideMargin();
                return;
            }

            double right = Math.Max(0D, layerDockingManager.ActualWidth - bounds.Right);
            double bottom = Math.Max(0D, layerDockingManager.ActualHeight - bounds.Bottom);
            PaneGuideMargin = new Thickness(
                Math.Max(0D, bounds.Left),
                Math.Max(0D, bounds.Top),
                right,
                bottom);
        }

        private void ApplyIsWorkspaceDropEnabled()
        {
            if (layerDockingManager != null)
            {
                layerDockingManager.AllowDrop = IsWorkspaceDropEnabled;
            }
        }

        private void ApplyGuideOverlayVisible()
        {
            if (dockingGuideOverlay != null)
            {
                dockingGuideOverlay.Visibility = IsGuideOverlayVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ApplyActiveGuideZone()
        {
            dockingGuideOverlay?.ApplyActiveZone(ActiveGuideZone);
        }

        private void ApplyPaneGuideMargin()
        {
            dockingGuideOverlay?.SetPaneGuideMargin(PaneGuideMargin);
        }

        private Rect GetPaneBounds(OpenVisionDockPaneHandle paneHandle)
        {
            LayoutAnchorablePane pane = paneHandle?.NativePane as LayoutAnchorablePane;
            if (pane == null || layerDockingManager == null)
            {
                return Rect.Empty;
            }

            foreach (LayoutAnchorablePaneControl paneControl in EnumerateVisualDescendants<LayoutAnchorablePaneControl>(layerDockingManager))
            {
                if (paneControl.Model is LayoutAnchorablePane currentPane && ReferenceEquals(currentPane, pane))
                {
                    return GetPaneControlBounds(paneControl);
                }
            }

            return Rect.Empty;
        }

        private Rect GetPaneControlBounds(FrameworkElement element)
        {
            if (element == null || layerDockingManager == null || element.ActualWidth <= 0D || element.ActualHeight <= 0D)
            {
                return Rect.Empty;
            }

            try
            {
                Point topLeft = element.TranslatePoint(new Point(0D, 0D), layerDockingManager);
                return new Rect(topLeft, new Size(element.ActualWidth, element.ActualHeight));
            }
            catch (InvalidOperationException)
            {
                return Rect.Empty;
            }
        }
    }
}
