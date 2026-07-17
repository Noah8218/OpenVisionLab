using OpenVisionLab._1._Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostLayerListPresenter
    {
        private readonly IDisplayManager displayManager;
        private readonly List<string> layerTitles = new List<string>();

        public OpenVisionShellHostLayerListPresenter(IDisplayManager displayManager)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
        }

        public ObservableCollection<OpenVisionShellHostLayerTabItem> Rows { get; } = new ObservableCollection<OpenVisionShellHostLayerTabItem>();

        public IReadOnlyList<string> LayerTitles => layerTitles;

        public int RowCount => Rows.Count;

        public bool IsSynchronizingSelection { get; private set; }

        public string ActiveLayerTitle =>
            string.IsNullOrWhiteSpace(displayManager.FocusItem)
                ? displayManager.SelectedItem
                : displayManager.FocusItem;

        public OpenVisionShellHostLayerListRefreshResult Refresh()
        {
            Rows.Clear();
            layerTitles.Clear();

            if (displayManager.LayerCount <= 0)
            {
                return new OpenVisionShellHostLayerListRefreshResult(-1, null);
            }

            string activeLayer = ActiveLayerTitle;
            string displayText = OpenVisionLanguageService.T("Shell.LayerStateDisplay");
            int activeIndex = -1;
            for (int index = 0; index < displayManager.LayerCount; index++)
            {
                string title = displayManager.GetLayerTitle(index);
                bool isActive = string.Equals(title, activeLayer, StringComparison.OrdinalIgnoreCase);
                if (isActive)
                {
                    activeIndex = index;
                }

                // Keep the visible row and backing title in one presenter so layer selection cannot drift by index.
                layerTitles.Add(title);
                string displayIndex = string.Format(CultureInfo.CurrentCulture, "{0:00}", index + 1);
                string state = isActive ? displayText : "OK";
                Rows.Add(new OpenVisionShellHostLayerTabItem(displayIndex, title, state, isActive));
            }

            int selectedIndex = activeIndex >= 0 ? activeIndex : displayManager.LayerCount - 1;
            string selectedTitle = TryGetLayerTitle(selectedIndex, out string titleAtIndex)
                ? titleAtIndex
                : null;
            return new OpenVisionShellHostLayerListRefreshResult(selectedIndex, selectedTitle);
        }

        public void ApplySelection(int selectedIndex, Action<int> setSelectedIndex)
        {
            IsSynchronizingSelection = true;
            try
            {
                setSelectedIndex?.Invoke(selectedIndex);
            }
            finally
            {
                IsSynchronizingSelection = false;
            }
        }

        public bool TryGetLayerTitle(int index, out string layerTitle)
        {
            if (index >= 0 && index < layerTitles.Count)
            {
                layerTitle = layerTitles[index];
                return true;
            }

            layerTitle = null;
            return false;
        }

        public string GetSelectedLayerTitle(int selectedIndex)
        {
            return TryGetLayerTitle(selectedIndex, out string layerTitle)
                ? layerTitle
                : ActiveLayerTitle;
        }
    }

    internal sealed class OpenVisionShellHostLayerListRefreshResult
    {
        public OpenVisionShellHostLayerListRefreshResult(int selectedIndex, string selectedLayerTitle)
        {
            SelectedIndex = selectedIndex;
            SelectedLayerTitle = selectedLayerTitle;
        }

        public int SelectedIndex { get; }

        public string SelectedLayerTitle { get; }
    }

    internal sealed class OpenVisionShellHostLayerTabItem
    {
        public OpenVisionShellHostLayerTabItem(string displayIndex, string title, string statusText, bool isActive)
        {
            DisplayIndex = string.IsNullOrWhiteSpace(displayIndex) ? "--" : displayIndex;
            Title = string.IsNullOrWhiteSpace(title) ? "-" : title;
            StatusText = string.IsNullOrWhiteSpace(statusText) ? "-" : statusText;
            IsActive = isActive;
        }

        public string DisplayIndex { get; }

        public string Title { get; }

        public string StatusText { get; }

        public bool IsActive { get; }

        public string DisplayText => string.Format(CultureInfo.CurrentCulture, "{0}  {1}  {2}", DisplayIndex, Title, StatusText);

        public string ToolTip => DisplayText;
    }
}
