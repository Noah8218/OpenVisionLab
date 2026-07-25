using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostLayerInteractionController
    {
        private readonly ListBox layerRowsList;
        private readonly OpenVisionShellHostLayerListPresenter layerListPresenter;
        private readonly string layerDragDataFormat;
        private readonly Func<string, bool> canDockLayer;
        private readonly Func<string, bool> dockLayer;
        private readonly Action<bool, bool> setWorkspaceDropOverlay;
        private Point dragStartPoint;

        public OpenVisionShellHostLayerInteractionController(
            ListBox layerRowsList,
            OpenVisionShellHostLayerListPresenter layerListPresenter,
            string layerDragDataFormat,
            Func<string, bool> canDockLayer,
            Func<string, bool> dockLayer,
            Action<bool, bool> setWorkspaceDropOverlay)
        {
            this.layerRowsList = layerRowsList ?? throw new ArgumentNullException(nameof(layerRowsList));
            this.layerListPresenter = layerListPresenter ?? throw new ArgumentNullException(nameof(layerListPresenter));
            this.layerDragDataFormat = layerDragDataFormat ?? throw new ArgumentNullException(nameof(layerDragDataFormat));
            this.canDockLayer = canDockLayer ?? throw new ArgumentNullException(nameof(canDockLayer));
            this.dockLayer = dockLayer ?? throw new ArgumentNullException(nameof(dockLayer));
            this.setWorkspaceDropOverlay = setWorkspaceDropOverlay ?? throw new ArgumentNullException(nameof(setWorkspaceDropOverlay));
        }

        public void HandleLayerTabPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            dragStartPoint = e.GetPosition(layerRowsList);
        }

        public void HandleLayerTabPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e == null || e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            Point currentPoint = e.GetPosition(layerRowsList);
            if (Math.Abs(currentPoint.X - dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance
                && Math.Abs(currentPoint.Y - dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            ListBoxItem item = FindVisualAncestor<ListBoxItem>(e.OriginalSource as DependencyObject);
            if (item == null)
            {
                return;
            }

            int index = layerRowsList.ItemContainerGenerator.IndexFromContainer(item);
            if (!layerListPresenter.TryGetLayerTitle(index, out string layerTitle)
                || !canDockLayer(layerTitle))
            {
                return;
            }

            DataObject data = new DataObject();
            data.SetData(layerDragDataFormat, layerTitle);
            DragDrop.DoDragDrop(layerRowsList, data, DragDropEffects.Copy);
        }

        public void HandleWorkspacePreviewDragOver(object sender, DragEventArgs e)
        {
            if (!TryReadLayerTitle(e, out string layerTitle))
            {
                return;
            }

            bool canDrop = canDockLayer(layerTitle);
            e.Effects = canDrop ? DragDropEffects.Copy : DragDropEffects.None;
            setWorkspaceDropOverlay(true, canDrop);
            e.Handled = true;
        }

        public void HandleWorkspacePreviewDragLeave(object sender, DragEventArgs e)
        {
            if (!TryReadLayerTitle(e, out _))
            {
                return;
            }

            setWorkspaceDropOverlay(false, false);
            if (e != null)
            {
                e.Handled = true;
            }
        }

        public void HandleWorkspacePreviewDrop(object sender, DragEventArgs e)
        {
            if (!TryReadLayerTitle(e, out string layerTitle))
            {
                return;
            }

            bool canDrop = canDockLayer(layerTitle);
            setWorkspaceDropOverlay(false, false);
            if (canDrop)
            {
                dockLayer(layerTitle);
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private bool TryReadLayerTitle(DragEventArgs e, out string layerTitle)
        {
            layerTitle = e?.Data.GetDataPresent(layerDragDataFormat) == true
                ? e.Data.GetData(layerDragDataFormat) as string
                : null;
            return !string.IsNullOrWhiteSpace(layerTitle);
        }

        private static T FindVisualAncestor<T>(DependencyObject element)
            where T : DependencyObject
        {
            while (element != null)
            {
                if (element is T match)
                {
                    return match;
                }

                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        }
    }
}
