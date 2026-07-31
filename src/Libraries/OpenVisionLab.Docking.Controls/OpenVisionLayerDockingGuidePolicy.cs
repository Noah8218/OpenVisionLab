using System;
using System.Windows;

namespace OpenVisionLab
{
    public enum DockingGuideZone
    {
        Center,
        Left,
        Right,
        Top,
        Bottom,
        GlobalLeft,
        GlobalRight,
        GlobalTop,
        GlobalBottom
    }

    public static class OpenVisionLayerDockingGuidePolicy
    {
        private const double GlobalDockGuideEdgeRatio = 0.085D;
        private const double PaneDockGuideEdgeRatio = 0.24D;
        private const double PaneDockGuideCompassWidth = 244D;
        private const double PaneDockGuideCompassHeight = 178D;
        private const double PaneDockGuideCompassExpandRatio = 0.38D;
        private const double PaneDockGuideCompassSideRatio = 0.32D;

        public static DockingGuideZone ResolveZone(Point workspacePoint, Size workspaceSize, Rect targetPaneBounds)
        {
            Rect bounds = IsUsable(targetPaneBounds)
                ? targetPaneBounds
                : new Rect(new Point(0D, 0D), workspaceSize);

            DockingGuideZone compassZone = ResolvePaneCompassZone(workspacePoint, bounds);
            if (compassZone != DockingGuideZone.Center)
            {
                return compassZone;
            }

            DockingGuideZone globalZone = ResolveGlobalZone(workspacePoint, workspaceSize);
            if (globalZone != DockingGuideZone.Center)
            {
                return globalZone;
            }

            return ResolvePaneZone(workspacePoint, bounds);
        }

        private static DockingGuideZone ResolveGlobalZone(Point point, Size workspaceSize)
        {
            if (workspaceSize.Width <= 0D || workspaceSize.Height <= 0D)
            {
                return DockingGuideZone.Center;
            }

            double xRatio = point.X / workspaceSize.Width;
            double yRatio = point.Y / workspaceSize.Height;
            if (xRatio <= GlobalDockGuideEdgeRatio)
            {
                return DockingGuideZone.GlobalLeft;
            }

            if (xRatio >= 1D - GlobalDockGuideEdgeRatio)
            {
                return DockingGuideZone.GlobalRight;
            }

            if (yRatio <= GlobalDockGuideEdgeRatio)
            {
                return DockingGuideZone.GlobalTop;
            }

            if (yRatio >= 1D - GlobalDockGuideEdgeRatio)
            {
                return DockingGuideZone.GlobalBottom;
            }

            return DockingGuideZone.Center;
        }

        private static DockingGuideZone ResolvePaneZone(Point point, Rect bounds)
        {
            if (!IsUsable(bounds))
            {
                return DockingGuideZone.Center;
            }

            double xRatio = Math.Max(0D, Math.Min(1D, (point.X - bounds.Left) / bounds.Width));
            double yRatio = Math.Max(0D, Math.Min(1D, (point.Y - bounds.Top) / bounds.Height));
            if (xRatio < PaneDockGuideEdgeRatio)
            {
                return DockingGuideZone.Left;
            }

            if (xRatio > 1D - PaneDockGuideEdgeRatio)
            {
                return DockingGuideZone.Right;
            }

            if (yRatio < PaneDockGuideEdgeRatio)
            {
                return DockingGuideZone.Top;
            }

            if (yRatio > 1D - PaneDockGuideEdgeRatio)
            {
                return DockingGuideZone.Bottom;
            }

            return DockingGuideZone.Center;
        }

        private static DockingGuideZone ResolvePaneCompassZone(Point point, Rect bounds)
        {
            if (!IsUsable(bounds) || !bounds.Contains(point))
            {
                return DockingGuideZone.Center;
            }

            double width = Math.Min(
                bounds.Width,
                Math.Max(PaneDockGuideCompassWidth, bounds.Width * PaneDockGuideCompassExpandRatio));
            double height = Math.Min(
                bounds.Height,
                Math.Max(PaneDockGuideCompassHeight, bounds.Height * PaneDockGuideCompassExpandRatio));
            Rect compassBounds = new Rect(
                bounds.Left + ((bounds.Width - width) * 0.5D),
                bounds.Top + ((bounds.Height - height) * 0.5D),
                width,
                height);

            if (!compassBounds.Contains(point))
            {
                return DockingGuideZone.Center;
            }

            double xRatio = Math.Max(0D, Math.Min(1D, (point.X - compassBounds.Left) / compassBounds.Width));
            double yRatio = Math.Max(0D, Math.Min(1D, (point.Y - compassBounds.Top) / compassBounds.Height));
            if (xRatio < PaneDockGuideCompassSideRatio)
            {
                return DockingGuideZone.Left;
            }

            if (xRatio > 1D - PaneDockGuideCompassSideRatio)
            {
                return DockingGuideZone.Right;
            }

            if (yRatio < PaneDockGuideCompassSideRatio)
            {
                return DockingGuideZone.Top;
            }

            if (yRatio > 1D - PaneDockGuideCompassSideRatio)
            {
                return DockingGuideZone.Bottom;
            }

            return DockingGuideZone.Center;
        }

        private static bool IsUsable(Rect rect)
        {
            return !rect.IsEmpty && rect.Width > 0D && rect.Height > 0D;
        }
    }
}
