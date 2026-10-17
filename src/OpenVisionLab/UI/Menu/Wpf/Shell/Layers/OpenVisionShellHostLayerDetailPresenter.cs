using OpenVisionLab.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostLayerDetailPresenter
    {
        private readonly IDisplayManager displayManager;

        public OpenVisionShellHostLayerDetailPresenter(IDisplayManager displayManager)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
        }

        public OpenVisionShellHostLayerDetailState BuildDetail(string layerTitle)
        {
            if (string.IsNullOrWhiteSpace(layerTitle))
            {
                return OpenVisionShellHostLayerDetailState.Empty();
            }

            Bitmap image = displayManager.GetLayerImage(layerTitle);
            if (image == null)
            {
                return new OpenVisionShellHostLayerDetailState(
                    layerTitle,
                    OpenVisionLanguageService.T("Shell.LayerDetailNoImage"),
                    OpenVisionLanguageService.T("Shell.LayerDetailPending"),
                    null, "Shell.LayerDetailPending");
            }

            string tackTime = string.IsNullOrWhiteSpace(displayManager.TackTime) ? "-" : displayManager.TackTime;
            string activeLayer = string.IsNullOrWhiteSpace(displayManager.FocusItem)
                ? displayManager.SelectedItem
                : displayManager.FocusItem;
            string stateTextKey = string.Equals(layerTitle, activeLayer, StringComparison.OrdinalIgnoreCase) ? "Shell.LayerStateDisplay" : null;
            string stateText = stateTextKey == null ? "OK" : OpenVisionLanguageService.T(stateTextKey);
            string metaText = string.Format(CultureInfo.CurrentCulture, "{0}x{1} / {2}", image.Width, image.Height, tackTime);

            return new OpenVisionShellHostLayerDetailState(layerTitle, metaText, stateText, image, stateTextKey);
        }

        public OpenVisionShellHostLayerActionState BuildActionState(string selectedLayer, ICollection<string> dockedLayerTitles)
        {
            return new OpenVisionShellHostLayerActionState(
                CanOpenLayerViewer(selectedLayer),
                CanDockLayer(selectedLayer, dockedLayerTitles),
                dockedLayerTitles?.Count > 0);
        }

        public bool CanOpenLayerViewer(string layerTitle)
        {
            return !string.IsNullOrWhiteSpace(layerTitle)
                && displayManager.FindIndex(layerTitle) >= 0
                && displayManager.GetLayerImage(layerTitle) != null;
        }

        public bool CanDockLayer(string layerTitle, ICollection<string> dockedLayerTitles)
        {
            return CanOpenLayerViewer(layerTitle)
                && !ContainsLayer(dockedLayerTitles, layerTitle);
        }

        private static bool ContainsLayer(ICollection<string> layerTitles, string layerTitle)
        {
            if (layerTitles == null || string.IsNullOrWhiteSpace(layerTitle))
            {
                return false;
            }

            foreach (string title in layerTitles)
            {
                if (string.Equals(title, layerTitle, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal sealed class OpenVisionShellHostLayerDetailState
    {
        public OpenVisionShellHostLayerDetailState(string layerTitle, string metaText, string routeText, Bitmap image, string routeTextKey = null)
        {
            LayerTitle = layerTitle;
            MetaText = metaText;
            RouteText = routeText;
            RouteTextKey = routeTextKey;
            Image = image;
        }

        public string LayerTitle { get; }

        public string MetaText { get; }

        public string RouteText { get; }

        public string RouteTextKey { get; }

        public Bitmap Image { get; }

        public bool HasImage => Image != null;

        public static OpenVisionShellHostLayerDetailState Empty()
        {
            return new OpenVisionShellHostLayerDetailState(null, null, null, null);
        }
    }

    internal sealed class OpenVisionShellHostLayerActionState
    {
        public OpenVisionShellHostLayerActionState(bool canOpen, bool canDock, bool canClearDocked)
        {
            CanOpen = canOpen;
            CanDock = canDock;
            CanClearDocked = canClearDocked;
        }

        public bool CanOpen { get; }

        public bool CanDock { get; }

        public bool CanClearDocked { get; }
    }
}
