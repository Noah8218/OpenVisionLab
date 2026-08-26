using Microsoft.Win32;
using OpenVisionLab.Core;
using OpenVisionLab.ImageSpace.Core;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostLayerManagementController
    {
        private const string MainLayerTitle = "Main";
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionShellHostDocumentController documentController;
        private readonly Func<Window> ownerProvider;
        private readonly Action<string> refreshSelectedLayerDetail;
        private readonly Action refreshRows;
        private readonly Action refreshDockedLayerViews;
        private readonly Action refreshDirectRouteText;
        private readonly Action<string> rememberImagePath;

        public OpenVisionShellHostLayerManagementController(
            IDisplayManager displayManager,
            OpenVisionShellHostDocumentController documentController,
            Func<Window> ownerProvider,
            Action<string> refreshSelectedLayerDetail,
            Action refreshRows,
            Action refreshDockedLayerViews,
            Action refreshDirectRouteText,
            Action<string> rememberImagePath = null)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.documentController = documentController ?? throw new ArgumentNullException(nameof(documentController));
            this.ownerProvider = ownerProvider ?? throw new ArgumentNullException(nameof(ownerProvider));
            this.refreshSelectedLayerDetail = refreshSelectedLayerDetail ?? throw new ArgumentNullException(nameof(refreshSelectedLayerDetail));
            this.refreshRows = refreshRows ?? throw new ArgumentNullException(nameof(refreshRows));
            this.refreshDockedLayerViews = refreshDockedLayerViews ?? throw new ArgumentNullException(nameof(refreshDockedLayerViews));
            this.refreshDirectRouteText = refreshDirectRouteText ?? throw new ArgumentNullException(nameof(refreshDirectRouteText));
            this.rememberImagePath = rememberImagePath;
        }

        public string CreateLayer()
        {
            string layerTitle = CreateUniqueLayerTitle("Layer");
            using Bitmap placeholder = CreatePlaceholderLayerBitmap();
            displayManager.CreateLayerDisplay(
                ImageSpaceFrame.Borrow(placeholder),
                layerTitle,
                true);

            ActivateAndRefresh(layerTitle);
            return layerTitle;
        }

        public bool PromptAndLoadImageIntoLayer(string layerTitle)
        {
            if (!CanLoadImageIntoLayer(layerTitle))
            {
                return false;
            }

            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.T("Shell.LoadImageIntoLayer"),
                Filter = "Image files|*.bmp;*.png;*.jpg;*.jpeg;*.tif;*.tiff|Bitmap (*.bmp)|*.bmp|PNG (*.png)|*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|TIFF (*.tif;*.tiff)|*.tif;*.tiff|All files|*.*",
                Multiselect = false,
                InitialDirectory = OpenVisionPreviewImageFileService.ResolveOpenImageDirectory(null)
            };

            if (dialog.ShowDialog(ownerProvider()) != true
                || !LoadImageIntoLayer(layerTitle, dialog.FileName))
            {
                return false;
            }

            OpenVisionImageDirectoryResolver.RememberImagePath(dialog.FileName);
            rememberImagePath?.Invoke(dialog.FileName);
            return true;
        }

        public bool LoadImageIntoLayer(string layerTitle, string path)
        {
            if (!CanLoadImageIntoLayer(layerTitle) || string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return false;
            }

            using Bitmap image = new Bitmap(path);
            return SetLayerImage(layerTitle, image);
        }

        public bool SetLayerImage(string layerTitle, Bitmap image)
        {
            int index = FindLayerIndex(layerTitle);
            if (index < 0 || image == null)
            {
                return false;
            }

            displayManager.SetLayerImage(index, OpenVisionShellHostWorkspaceImageController.CloneBitmapForLayer(image));
            ActivateAndRefresh(layerTitle);
            return true;
        }

        public bool DeleteLayer(string layerTitle)
        {
            if (!CanDeleteLayer(layerTitle))
            {
                return false;
            }

            string fallbackLayer = ResolveFallbackLayerTitle(layerTitle);
            if (displayManager is DisplayManagerService service)
            {
                service.RemoveLayerDisplay(layerTitle);
            }
            else
            {
                return false;
            }

            ActivateAndRefresh(fallbackLayer);
            return true;
        }

        public bool RenameLayer(string oldLayerTitle, string newLayerTitle)
        {
            if (!CanRenameLayer(oldLayerTitle, newLayerTitle)
                || displayManager is not DisplayManagerService service)
            {
                return false;
            }

            string normalizedNewTitle = NormalizeLayerTitle(newLayerTitle);
            if (!service.RenameLayerDisplay(oldLayerTitle, normalizedNewTitle))
            {
                return false;
            }

            ActivateAndRefresh(normalizedNewTitle);
            return true;
        }

        public bool CanLoadImageIntoLayer(string layerTitle)
        {
            return FindLayerIndex(layerTitle) >= 0;
        }

        public bool CanDeleteLayer(string layerTitle)
        {
            return FindLayerIndex(layerTitle) >= 0
                && !string.Equals(layerTitle, MainLayerTitle, StringComparison.OrdinalIgnoreCase);
        }

        public bool CanRenameLayer(string oldLayerTitle, string newLayerTitle)
        {
            string normalizedNewTitle = NormalizeLayerTitle(newLayerTitle);
            return CanDeleteLayer(oldLayerTitle)
                && !string.IsNullOrWhiteSpace(normalizedNewTitle)
                && !string.Equals(oldLayerTitle, normalizedNewTitle, StringComparison.OrdinalIgnoreCase)
                && FindLayerIndex(normalizedNewTitle) < 0
                && IsValidLayerTitle(normalizedNewTitle);
        }

        private int FindLayerIndex(string layerTitle)
        {
            return string.IsNullOrWhiteSpace(layerTitle) ? -1 : displayManager.FindIndex(layerTitle);
        }

        private void ActivateAndRefresh(string layerTitle)
        {
            if (!string.IsNullOrWhiteSpace(layerTitle) && displayManager.FindIndex(layerTitle) >= 0)
            {
                displayManager.SelectedItem = layerTitle;
                displayManager.ActivateLayer(layerTitle);
            }

            documentController.ActiveNativeDocument?.RefreshLayerState();
            refreshRows();
            refreshSelectedLayerDetail(
                !string.IsNullOrWhiteSpace(layerTitle) && displayManager.FindIndex(layerTitle) >= 0
                    ? layerTitle
                    : ResolveFirstLayerTitle());
            refreshDirectRouteText();
            refreshDockedLayerViews();
        }

        private string ResolveFallbackLayerTitle(string removedLayerTitle)
        {
            if (displayManager.FindIndex(MainLayerTitle) >= 0
                && !string.Equals(removedLayerTitle, MainLayerTitle, StringComparison.OrdinalIgnoreCase))
            {
                return MainLayerTitle;
            }

            return ResolveFirstLayerTitle();
        }

        private string ResolveFirstLayerTitle()
        {
            return displayManager.LayerCount > 0 ? displayManager.GetLayerTitle(0) : string.Empty;
        }

        private string CreateUniqueLayerTitle(string prefix)
        {
            int next = Math.Max(1, displayManager.LayerCount + 1);
            string candidate;
            do
            {
                candidate = prefix + "_" + next.ToString("000", CultureInfo.InvariantCulture);
                next++;
            }
            while (displayManager.FindIndex(candidate) >= 0);

            return candidate;
        }

        private static string NormalizeLayerTitle(string layerTitle)
        {
            return string.IsNullOrWhiteSpace(layerTitle)
                ? string.Empty
                : layerTitle.Trim();
        }

        private static bool IsValidLayerTitle(string layerTitle)
        {
            if (string.IsNullOrWhiteSpace(layerTitle))
            {
                return false;
            }

            foreach (char c in layerTitle)
            {
                if (char.IsControl(c) || Array.IndexOf(Path.GetInvalidFileNameChars(), c) >= 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static Bitmap CreatePlaceholderLayerBitmap()
        {
            Bitmap image = new Bitmap(640, 420, PixelFormat.Format24bppRgb);
            using Graphics graphics = Graphics.FromImage(image);
            using SolidBrush background = new SolidBrush(Color.FromArgb(18, 26, 30));
            using Pen border = new Pen(Color.FromArgb(42, 88, 96), 2F);
            graphics.FillRectangle(background, 0, 0, image.Width, image.Height);
            graphics.DrawRectangle(border, 18, 18, image.Width - 36, image.Height - 36);
            return image;
        }
    }
}
