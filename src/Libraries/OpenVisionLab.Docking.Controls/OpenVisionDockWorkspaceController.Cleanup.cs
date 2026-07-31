using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionDockWorkspaceController
    {
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
    }
}
