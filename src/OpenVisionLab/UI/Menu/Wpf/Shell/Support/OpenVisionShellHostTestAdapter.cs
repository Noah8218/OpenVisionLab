using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostTestAdapter
    {
        private readonly ListBox hostLayerRowsList;
        private readonly OpenVisionShellHostLayerListPresenter layerListPresenter;
        private readonly Action refreshHostLayerRows;

        public OpenVisionShellHostTestAdapter(
            ListBox hostLayerRowsList,
            OpenVisionShellHostLayerListPresenter layerListPresenter,
            Action refreshHostLayerRows)
        {
            this.hostLayerRowsList = hostLayerRowsList;
            this.layerListPresenter = layerListPresenter ?? throw new ArgumentNullException(nameof(layerListPresenter));
            this.refreshHostLayerRows = refreshHostLayerRows ?? throw new ArgumentNullException(nameof(refreshHostLayerRows));
        }

        public bool AreHostLayerTabsReadable()
        {
            if (layerListPresenter.RowCount <= 0)
            {
                return true;
            }

            if (hostLayerRowsList == null
                || hostLayerRowsList.Visibility != Visibility.Visible
                || hostLayerRowsList.ActualHeight <= 0D)
            {
                return AreLayerRowsReadableFromModel();
            }

            hostLayerRowsList?.UpdateLayout();
            List<ListBoxItem> tabItems = EnumerateVisualDescendants<ListBoxItem>(hostLayerRowsList)
                .Where(item => item.DataContext is OpenVisionShellHostLayerTabItem)
                .ToList();

            return tabItems.Count >= layerListPresenter.RowCount
                && tabItems.All(item => item.ActualWidth >= 120D && item.ActualHeight >= 24D)
                && layerListPresenter.Rows.All(row =>
                    !string.IsNullOrWhiteSpace(row.DisplayIndex)
                    && !string.IsNullOrWhiteSpace(row.Title)
                    && !string.IsNullOrWhiteSpace(row.StatusText));
        }

        public bool SelectHostLayerRow(string layerTitle)
        {
            if (string.IsNullOrWhiteSpace(layerTitle))
            {
                return false;
            }

            refreshHostLayerRows();
            for (int index = 0; index < layerListPresenter.RowCount; index++)
            {
                if (!layerListPresenter.TryGetLayerTitle(index, out string currentTitle)
                    || !string.Equals(currentTitle, layerTitle, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (hostLayerRowsList != null)
                {
                    hostLayerRowsList.SelectedIndex = index;
                    hostLayerRowsList.UpdateLayout();
                }

                return true;
            }

            return false;
        }

        public bool RightClickHostLayerRow(string layerTitle)
        {
            if (string.IsNullOrWhiteSpace(layerTitle) || hostLayerRowsList == null)
            {
                return false;
            }

            refreshHostLayerRows();
            hostLayerRowsList.UpdateLayout();
            for (int index = 0; index < layerListPresenter.RowCount; index++)
            {
                if (!layerListPresenter.TryGetLayerTitle(index, out string currentTitle)
                    || !string.Equals(currentTitle, layerTitle, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                hostLayerRowsList.SelectedIndex = index;
                hostLayerRowsList.UpdateLayout();
                return true;
            }

            return false;
        }

        private bool AreLayerRowsReadableFromModel()
        {
            return layerListPresenter.Rows.All(row =>
                !string.IsNullOrWhiteSpace(row.DisplayIndex)
                && !string.IsNullOrWhiteSpace(row.Title)
                && !string.IsNullOrWhiteSpace(row.StatusText));
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
