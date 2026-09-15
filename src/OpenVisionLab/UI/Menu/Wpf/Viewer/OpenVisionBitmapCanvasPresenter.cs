using OpenVisionLab.ImageCanvas.ViewModels;
using System;
using System.Drawing;
using System.IO;

namespace OpenVisionLab
{
    internal sealed class OpenVisionBitmapCanvasPresenter : IDisposable
    {
        private readonly RoiImageCanvasViewModel canvasViewModel;
        private readonly string fallbackImageName;
        private Bitmap currentImage;
        private string currentImageName;
        private bool fitImageOnNextRefresh;
        private bool disposed;

        public OpenVisionBitmapCanvasPresenter(string viewerName, string fallbackImageName)
        {
            canvasViewModel = new RoiImageCanvasViewModel(viewerName);
            this.fallbackImageName = string.IsNullOrWhiteSpace(fallbackImageName) ? "Image" : fallbackImageName;
            currentImageName = this.fallbackImageName;
        }

        public RoiImageCanvasViewModel CanvasViewModel => canvasViewModel;

        public bool HasImage => currentImage != null;

        public int ImagePixelWidth => currentImage?.Width ?? 0;

        public int ImagePixelHeight => currentImage?.Height ?? 0;

        public int TextureTileCount => canvasViewModel?.TextureTileCount ?? 0;

        public void SetBitmap(Bitmap image, string imageName)
        {
            if (disposed)
            {
                return;
            }

            string nextImageName = string.IsNullOrWhiteSpace(imageName) ? fallbackImageName : imageName;
            fitImageOnNextRefresh = currentImage == null
                || image == null
                || currentImage.Width != image.Width
                || currentImage.Height != image.Height
                || !string.Equals(currentImageName, nextImageName, StringComparison.Ordinal);

            currentImage = image;
            currentImageName = nextImageName;
            RefreshCanvas();
        }

        public void RefreshCanvas()
        {
            if (disposed)
            {
                return;
            }

            if (currentImage == null)
            {
                canvasViewModel.ClearImage();
                return;
            }

            // One presenter owns bitmap-to-OpenGL upload and save callback wiring for every shell preview canvas.
            canvasViewModel.ClearImage();
            canvasViewModel.LoadImage(currentImage, currentImageName, SaveCurrentImageFromCurrentBitmap);
            if (fitImageOnNextRefresh)
            {
                // Keep user zoom stable while a tool repeatedly updates the same layer image.
                canvasViewModel.FitImageToView();
                fitImageOnNextRefresh = false;
            }

            canvasViewModel.RefreshCanvas();
        }

        public void FitImageToView()
        {
            if (disposed)
            {
                return;
            }

            canvasViewModel.FitImageToView();
        }

        public bool SaveCurrentImage(string path)
        {
            return canvasViewModel?.SaveCurrentImage(path) ?? false;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            currentImage = null;
            canvasViewModel.Dispose();
        }

        private bool SaveCurrentImageFromCurrentBitmap(string path)
        {
            if (currentImage == null || string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using Bitmap savableImage = OpenVisionPreviewImageFileService.CreateSavableBitmap(currentImage);
                savableImage.Save(path, OpenVisionPreviewImageFileService.ResolveImageFormat(path));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
