using OpenVisionLab.Core;
using OpenVisionLab.Mvvm;
using OpenVisionLab.ImageSpace.Core;
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

        public LayerListRefreshResult Refresh()
        {
            Rows.Clear();
            layerTitles.Clear();

            if (displayManager.LayerCount <= 0)
            {
                return new LayerListRefreshResult(-1, null);
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
                using ImageSpaceImageLease lease = displayManager.ImageSpace.AcquireImage(title);
                string size = lease?.Image == null ? null : lease.Image.Width.ToString(CultureInfo.InvariantCulture) + "×" + lease.Image.Height.ToString(CultureInfo.InvariantCulture);
                Rows.Add(new OpenVisionShellHostLayerTabItem(displayIndex, title, state, isActive, size));
            }

            int selectedIndex = activeIndex >= 0 ? activeIndex : displayManager.LayerCount - 1;
            string selectedTitle = TryGetLayerTitle(selectedIndex, out string titleAtIndex)
                ? titleAtIndex
                : null;
            return new LayerListRefreshResult(selectedIndex, selectedTitle);
        }

        public void ApplyLocalization()
        {
            foreach (OpenVisionShellHostLayerTabItem row in Rows) { row.ApplyLocalization(); }
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

    internal sealed class LayerListRefreshResult
    {
        public LayerListRefreshResult(int selectedIndex, string selectedLayerTitle)
        {
            SelectedIndex = selectedIndex;
            SelectedLayerTitle = selectedLayerTitle;
        }

        public int SelectedIndex { get; }

        public string SelectedLayerTitle { get; }
    }

    internal sealed class OpenVisionShellHostLayerTabItem : ObservableObject
    {
        private string statusText;
        private readonly string sizeText;

        public OpenVisionShellHostLayerTabItem(string displayIndex, string title, string statusText, bool isActive, string sizeText = null)
        {
            DisplayIndex = string.IsNullOrWhiteSpace(displayIndex) ? "--" : displayIndex;
            Title = string.IsNullOrWhiteSpace(title) ? "-" : title;
            this.statusText = string.IsNullOrWhiteSpace(statusText) ? "-" : statusText;
            IsActive = isActive;
            this.sizeText = sizeText;
        }

        public string OriginText => OpenVisionLanguageService.T("Workbench.LayerManager.UnknownOrigin");

        public string SizeText => sizeText ?? OpenVisionLanguageService.T("Workbench.LayerManager.NotStored");

        public string DisplayIndex { get; }

        public string Title { get; }

        public string StatusText => statusText;

        public bool IsActive { get; }

        public string DisplayText => string.Format(CultureInfo.CurrentCulture, "{0}  {1}  {2}", DisplayIndex, Title, StatusText);

        public string ToolTip => DisplayText;

        public void ApplyLocalization()
        {
            OnPropertyChanged(nameof(OriginText)); OnPropertyChanged(nameof(SizeText));
            if (SetProperty(ref statusText, IsActive ? OpenVisionLanguageService.T("Shell.LayerStateDisplay") : "OK", nameof(StatusText)))
            {
                OnPropertyChanged(nameof(DisplayText));
                OnPropertyChanged(nameof(ToolTip));
            }
        }
    }
}
