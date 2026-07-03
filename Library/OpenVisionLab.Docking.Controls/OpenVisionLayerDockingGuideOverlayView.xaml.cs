using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenVisionLab.Docking.Controls
{
    public partial class OpenVisionLayerDockingGuideOverlayView : UserControl
    {
        public OpenVisionLayerDockingGuideOverlayView()
        {
            InitializeComponent();
        }

        public FrameworkElement PaneGuideOverlay => paneDockingGuideOverlay;

        public int GuideZoneCount => EnumerateVisualDescendants<FrameworkElement>(this)
            .Count(element => string.Equals(
                AutomationProperties.GetAutomationId(element),
                "DockingGuideZone",
                StringComparison.Ordinal));

        public void ApplyActiveZone(DockingGuideZone activeZone)
        {
            bool isGlobalZone = IsGlobalZone(activeZone);
            SetVisible(globalDockingGuideOverlay, isGlobalZone);
            SetVisible(paneDockingGuideOverlay, !isGlobalZone);
            SetVisible(dockGuideGlobalLeftZone, activeZone == DockingGuideZone.GlobalLeft);
            SetVisible(dockGuideGlobalRightZone, activeZone == DockingGuideZone.GlobalRight);
            SetVisible(dockGuideGlobalTopZone, activeZone == DockingGuideZone.GlobalTop);
            SetVisible(dockGuideGlobalBottomZone, activeZone == DockingGuideZone.GlobalBottom);
            SetVisible(dockGuideGlobalLeftRegion, activeZone == DockingGuideZone.GlobalLeft);
            SetVisible(dockGuideGlobalRightRegion, activeZone == DockingGuideZone.GlobalRight);
            SetVisible(dockGuideGlobalTopRegion, activeZone == DockingGuideZone.GlobalTop);
            SetVisible(dockGuideGlobalBottomRegion, activeZone == DockingGuideZone.GlobalBottom);

            ApplyDockingGuideZoneState(dockGuideGlobalLeftZone, activeZone == DockingGuideZone.GlobalLeft);
            ApplyDockingGuideZoneState(dockGuideGlobalRightZone, activeZone == DockingGuideZone.GlobalRight);
            ApplyDockingGuideZoneState(dockGuideGlobalTopZone, activeZone == DockingGuideZone.GlobalTop);
            ApplyDockingGuideZoneState(dockGuideGlobalBottomZone, activeZone == DockingGuideZone.GlobalBottom);
            ApplyDockingGuideZoneState(dockGuideLeftZone, activeZone == DockingGuideZone.Left);
            ApplyDockingGuideZoneState(dockGuideRightZone, activeZone == DockingGuideZone.Right);
            ApplyDockingGuideZoneState(dockGuideTopZone, activeZone == DockingGuideZone.Top);
            ApplyDockingGuideZoneState(dockGuideBottomZone, activeZone == DockingGuideZone.Bottom);
            ApplyDockingGuideZoneState(dockGuideCenterZone, activeZone == DockingGuideZone.Center);

            SetVisible(dockGuideLeftRegion, !isGlobalZone && activeZone == DockingGuideZone.Left);
            SetVisible(dockGuideRightRegion, !isGlobalZone && activeZone == DockingGuideZone.Right);
            SetVisible(dockGuideTopRegion, !isGlobalZone && activeZone == DockingGuideZone.Top);
            SetVisible(dockGuideBottomRegion, !isGlobalZone && activeZone == DockingGuideZone.Bottom);
            SetVisible(dockGuideCenterRegion, !isGlobalZone && activeZone == DockingGuideZone.Center);
        }

        public void SetPaneGuideMargin(Thickness margin)
        {
            paneDockingGuideOverlay.Margin = margin;
        }

        private static bool IsGlobalZone(DockingGuideZone activeZone)
        {
            return activeZone == DockingGuideZone.GlobalLeft
                || activeZone == DockingGuideZone.GlobalRight
                || activeZone == DockingGuideZone.GlobalTop
                || activeZone == DockingGuideZone.GlobalBottom;
        }

        private static void SetVisible(UIElement element, bool visible)
        {
            if (element != null)
            {
                element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private static void ApplyDockingGuideZoneState(Border zone, bool active)
        {
            if (zone == null)
            {
                return;
            }

            zone.Background = new SolidColorBrush(active
                ? Color.FromArgb(0xAA, 0x15, 0x7C, 0x86)
                : Color.FromArgb(0xCC, 0x10, 0x24, 0x2A));
            zone.BorderBrush = new SolidColorBrush(active
                ? Color.FromRgb(0xDF, 0xFB, 0xFF)
                : Color.FromRgb(0x6A, 0xAF, 0xB7));
            zone.BorderThickness = active ? new Thickness(2) : new Thickness(1);
            zone.Opacity = active ? 1D : 0.78D;
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
