using Microsoft.Win32;
using OpenVisionLab._1._Core;
using OpenVisionLab.ImageSpace.Core;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativePreviewImageCommandController
    {
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionNativePreviewLayerPublisher previewLayerPublisher;
        private readonly FrameworkElement ownerElement;
        private readonly Func<VisionToolPreviewImageRole, string> resolveLayerForRole;
        private readonly Func<VisionToolPreviewImageRole, string> prepareLayerForLoadRole;
        private readonly Func<string> resolvePrimaryInputLayer;
        private readonly Action clearPreviewResult;
        private readonly Action refreshLayerState;
        private readonly Action<string> setStatus;
        private static string lastImageDirectory;

        public OpenVisionNativePreviewImageCommandController(
            IDisplayManager displayManager,
            OpenVisionNativePreviewLayerPublisher previewLayerPublisher,
            FrameworkElement ownerElement,
            Func<VisionToolPreviewImageRole, string> resolveLayerForRole,
            Func<VisionToolPreviewImageRole, string> prepareLayerForLoadRole,
            Func<string> resolvePrimaryInputLayer,
            Action clearPreviewResult,
            Action refreshLayerState,
            Action<string> setStatus)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.previewLayerPublisher = previewLayerPublisher ?? throw new ArgumentNullException(nameof(previewLayerPublisher));
            this.ownerElement = ownerElement ?? throw new ArgumentNullException(nameof(ownerElement));
            this.resolveLayerForRole = resolveLayerForRole ?? throw new ArgumentNullException(nameof(resolveLayerForRole));
            this.prepareLayerForLoadRole = prepareLayerForLoadRole ?? this.resolveLayerForRole;
            this.resolvePrimaryInputLayer = resolvePrimaryInputLayer ?? throw new ArgumentNullException(nameof(resolvePrimaryInputLayer));
            this.clearPreviewResult = clearPreviewResult ?? throw new ArgumentNullException(nameof(clearPreviewResult));
            this.refreshLayerState = refreshLayerState ?? throw new ArgumentNullException(nameof(refreshLayerState));
            this.setStatus = setStatus ?? throw new ArgumentNullException(nameof(setStatus));
        }

        public void LoadWithDialog(VisionToolPreviewImageRole role)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.T("ToolView.LoadImage"),
                Filter = "Image files|*.bmp;*.png;*.jpg;*.jpeg;*.tif;*.tiff|Bitmap (*.bmp)|*.bmp|PNG (*.png)|*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|TIFF (*.tif;*.tiff)|*.tif;*.tiff|All files|*.*",
                Multiselect = false,
                InitialDirectory = OpenVisionPreviewImageFileService.ResolveOpenImageDirectory(lastImageDirectory)
            };

            if (dialog.ShowDialog(Window.GetWindow(ownerElement)) != true)
            {
                return;
            }

            LoadFromFile(role, dialog.FileName);
        }

        public void SaveWithDialog(VisionToolPreviewImageRole role)
        {
            string layerName = ResolveLayerName(role, fallbackToMain: false);
            Bitmap image = displayManager.GetLayerImage(layerName);
            if (image == null)
            {
                setStatus("Image save NG / image missing / " + layerName);
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = OpenVisionLanguageService.T("ToolView.SaveImage"),
                Filter = "PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|TIFF (*.tif)|*.tif",
                FileName = OpenVisionPreviewImageFileService.CreateDefaultImageFileName(layerName),
                InitialDirectory = OpenVisionPreviewImageFileService.ResolveOpenImageDirectory(lastImageDirectory),
                AddExtension = true,
                DefaultExt = ".png"
            };

            if (dialog.ShowDialog(Window.GetWindow(ownerElement)) != true)
            {
                return;
            }

            SaveToFile(role, dialog.FileName, layerName);
        }

        public bool LoadFromFile(VisionToolPreviewImageRole role, string path, string layerName = null)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                setStatus("Image load NG / file missing");
                return false;
            }

            try
            {
                using Bitmap loaded = new Bitmap(path);
                layerName = string.IsNullOrWhiteSpace(layerName) ? ResolveLayerNameForLoad(role, fallbackToMain: true) : layerName;
                Bitmap image = OpenVisionPreviewImageFileService.CloneBitmapPreservingPixelFormat(loaded);
                int width = image.Width;
                int height = image.Height;
                displayManager.CreateLayerDisplay(ImageSpaceFrame.FromBitmap(OpenVisionPreviewImageFileService.CloneBitmapPreservingPixelFormat(image)), layerName, false);
                image.Dispose();

                // Loading an output preview must not steal the selected input layer from the tool.
                string activationLayer = role == VisionToolPreviewImageRole.Output
                    ? resolvePrimaryInputLayer()
                    : layerName;
                previewLayerPublisher.RestoreDisplayActivation(activationLayer);
                lastImageDirectory = Path.GetDirectoryName(path);
                clearPreviewResult();
                refreshLayerState();
                setStatus(string.Format(CultureInfo.CurrentCulture, "Image loaded / {0} / {1}x{2}", layerName, width, height));
                return true;
            }
            catch (Exception ex)
            {
                setStatus("Image load NG / " + ex.GetBaseException().Message);
                return false;
            }
        }

        public bool SaveToFile(VisionToolPreviewImageRole role, string path, string layerName = null)
        {
            layerName = string.IsNullOrWhiteSpace(layerName) ? ResolveLayerName(role, fallbackToMain: false) : layerName;
            Bitmap image = displayManager.GetLayerImage(layerName);
            if (image == null)
            {
                setStatus("Image save NG / image missing / " + layerName);
                return false;
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                setStatus("Image save NG / path missing");
                return false;
            }

            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using Bitmap savableImage = OpenVisionPreviewImageFileService.CreateSavableBitmap(image);
                savableImage.Save(path, OpenVisionPreviewImageFileService.ResolveImageFormat(path));
                lastImageDirectory = Path.GetDirectoryName(path);
                setStatus("Image saved / " + layerName + " / " + path);
                return true;
            }
            catch (Exception ex)
            {
                setStatus("Image save NG / " + ex.GetBaseException().Message);
                return false;
            }
        }

        private string ResolveLayerName(VisionToolPreviewImageRole role, bool fallbackToMain)
        {
            string layerName = resolveLayerForRole(role);
            if (fallbackToMain && string.IsNullOrWhiteSpace(layerName))
            {
                return "Main";
            }

            return layerName;
        }

        private string ResolveLayerNameForLoad(VisionToolPreviewImageRole role, bool fallbackToMain)
        {
            string layerName = prepareLayerForLoadRole(role);
            if (fallbackToMain && string.IsNullOrWhiteSpace(layerName))
            {
                return "Main";
            }

            return layerName;
        }
    }
}
