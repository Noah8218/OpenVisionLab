using OpenVisionLab._1._Core;
using OpenVisionLab.ImageSpace.Core;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostLayerViewerController
    {
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionShellHostLayerDetailPresenter layerDetailPresenter;
        private readonly OpenVisionLayerViewerWindowRegistry windowRegistry;
        private readonly Func<Window> ownerProvider;

        public OpenVisionShellHostLayerViewerController(
            IDisplayManager displayManager,
            OpenVisionShellHostLayerDetailPresenter layerDetailPresenter,
            OpenVisionLayerViewerWindowRegistry windowRegistry,
            Func<Window> ownerProvider)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.layerDetailPresenter = layerDetailPresenter ?? throw new ArgumentNullException(nameof(layerDetailPresenter));
            this.windowRegistry = windowRegistry ?? throw new ArgumentNullException(nameof(windowRegistry));
            this.ownerProvider = ownerProvider ?? throw new ArgumentNullException(nameof(ownerProvider));
        }

        public bool CanOpen(string layerTitle)
        {
            return layerDetailPresenter.CanOpenLayerViewer(layerTitle);
        }

        public bool Open(string layerTitle)
        {
            if (!CanOpen(layerTitle))
            {
                return false;
            }

            Bitmap image = displayManager.GetLayerImage(layerTitle);
            OpenVisionLayerViewerView viewer = new OpenVisionLayerViewerView();
            viewer.SetLayer(layerTitle, image, BuildStatus(layerTitle, image));

            string title = string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T("Shell.LayerViewerTitleFormat"),
                layerTitle);
            OpenVisionFloatingToolWindow window = new OpenVisionFloatingToolWindow(title, viewer)
            {
                Width = 820,
                Height = 600,
                MinWidth = 520,
                MinHeight = 380,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.CanResize
            };
            window.SetTitleIcon(MahApps.Metro.IconPacks.PackIconMaterialKind.ImageMultipleOutline);

            Window owner = ownerProvider();
            if (owner != null)
            {
                window.Owner = owner;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            windowRegistry.Add(window);
            window.Show();
            window.BringAboveOwnerAirspace();
            return true;
        }

        public void CloseAll()
        {
            windowRegistry.CloseAll();
        }

        public string BuildStatus(string layerTitle, Bitmap image)
        {
            if (image == null)
            {
                return OpenVisionLanguageService.T("Shell.LayerDetailNoImage");
            }

            string tackTime = string.IsNullOrWhiteSpace(displayManager.TackTime) ? "-" : displayManager.TackTime;
            string activeLayer = string.IsNullOrWhiteSpace(displayManager.FocusItem)
                ? displayManager.SelectedItem
                : displayManager.FocusItem;
            string state = string.Equals(layerTitle, activeLayer, StringComparison.OrdinalIgnoreCase)
                ? OpenVisionLanguageService.T("Shell.LayerStateDisplay")
                : "OK";
            return string.Format(CultureInfo.CurrentCulture, "{0} / {1}x{2} / {3}", state, image.Width, image.Height, tackTime);
        }
    }
}
