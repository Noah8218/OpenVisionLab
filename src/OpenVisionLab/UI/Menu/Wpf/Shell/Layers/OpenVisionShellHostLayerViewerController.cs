using OpenVisionLab.Core;
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
        private OpenVisionFloatingToolWindow toolPreviewWindow;
        private OpenVisionLayerViewerView toolPreviewViewer;

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

            OpenVisionLayerViewerView viewer = new OpenVisionLayerViewerView();
            viewer.Tag = layerTitle;
            using (ImageSpaceImageLease lease = displayManager.ImageSpace.AcquireImage(layerTitle))
            {
                Bitmap leasedImage = lease?.Image;
                viewer.SetLayer(layerTitle, leasedImage, BuildStatus(layerTitle, leasedImage));
            }

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

        public bool OpenToolPreview(
            string toolName,
            VisionToolPreviewImageRole role,
            string layerTitle)
        {
            if (!CanOpen(layerTitle))
            {
                return false;
            }

            string title = BuildToolPreviewTitle(toolName, role, layerTitle);
            if (toolPreviewWindow == null || toolPreviewViewer == null)
            {
                toolPreviewViewer = new OpenVisionLayerViewerView();
                toolPreviewWindow = new OpenVisionFloatingToolWindow(title, toolPreviewViewer)
                {
                    Width = 960,
                    Height = 720,
                    MinWidth = 520,
                    MinHeight = 380,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ResizeMode = ResizeMode.CanResize,
                    IsDockButtonVisible = false
                };
                toolPreviewWindow.SetTitleIcon(MahApps.Metro.IconPacks.PackIconMaterialKind.ImageMultipleOutline);
                toolPreviewWindow.Closed += ToolPreviewWindow_Closed;

                Window owner = ownerProvider();
                if (owner != null)
                {
                    toolPreviewWindow.Owner = owner;
                }
                else
                {
                    toolPreviewWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }

                windowRegistry.Add(toolPreviewWindow);
                UpdateToolPreview(title, layerTitle);
                toolPreviewWindow.Show();
            }
            else
            {
                UpdateToolPreview(title, layerTitle);
                if (toolPreviewWindow.WindowState == WindowState.Minimized)
                {
                    toolPreviewWindow.WindowState = WindowState.Normal;
                }

                toolPreviewWindow.Activate();
            }

            toolPreviewWindow.BringAboveOwnerAirspace();
            return true;
        }

        public bool RefreshToolPreview(
            string toolName,
            VisionToolPreviewImageRole role,
            string layerTitle)
        {
            if (toolPreviewWindow == null || toolPreviewViewer == null)
            {
                return false;
            }

            UpdateToolPreview(BuildToolPreviewTitle(toolName, role, layerTitle), layerTitle);
            return true;
        }

        public void CloseToolPreview()
        {
            toolPreviewWindow?.Close();
        }

        public void CloseAll()
        {
            windowRegistry.CloseAll();
        }

        public void RefreshOpenLayerViewers()
        {
            foreach (OpenVisionFloatingToolWindow window in windowRegistry.Windows)
            {
                if (window.HostedContent is not OpenVisionLayerViewerView viewer
                    || viewer.Tag is not string layerTitle)
                {
                    continue;
                }

                using ImageSpaceImageLease lease = displayManager.ImageSpace.AcquireImage(layerTitle);
                if (lease == null)
                {
                    window.Close();
                    continue;
                }

                Bitmap image = lease.Image;
                viewer.SetLayer(layerTitle, image, BuildStatus(layerTitle, image));
            }
        }

        private void UpdateToolPreview(string title, string layerTitle)
        {
            toolPreviewWindow.SetTitle(title);
            using ImageSpaceImageLease lease = displayManager.ImageSpace.AcquireImage(layerTitle);
            Bitmap image = lease?.Image;
            toolPreviewViewer.SetLayer(title, image, BuildStatus(layerTitle, image));
        }

        private void ToolPreviewWindow_Closed(object sender, EventArgs e)
        {
            if (sender is OpenVisionFloatingToolWindow window)
            {
                window.Closed -= ToolPreviewWindow_Closed;
            }

            toolPreviewWindow = null;
            toolPreviewViewer = null;
        }

        private static string BuildToolPreviewTitle(
            string toolName,
            VisionToolPreviewImageRole role,
            string layerTitle)
        {
            string localizedToolName = OpenVisionLanguageService.T("VisionMenu." + toolName);
            string roleText = role switch
            {
                VisionToolPreviewImageRole.InputA => OpenVisionLanguageService.T("ToolView.PreviewRole.InputA"),
                VisionToolPreviewImageRole.InputB => OpenVisionLanguageService.T("ToolView.PreviewRole.InputB"),
                VisionToolPreviewImageRole.Output => OpenVisionLanguageService.T("ToolView.PreviewRole.Output"),
                _ => OpenVisionLanguageService.T("ToolView.PreviewRole.Input")
            };
            return string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T("ToolView.PreviewViewerTitleFormat"),
                localizedToolName,
                roleText,
                layerTitle);
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
