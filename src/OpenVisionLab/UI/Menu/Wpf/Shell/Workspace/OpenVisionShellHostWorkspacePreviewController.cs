using System;
using System.Drawing;
using OpenVisionLab.Core;
using OpenVisionLab.ImageSpace.Core;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostWorkspacePreviewController : IDisposable
    {
        private readonly OpenVisionBitmapCanvasPresenter canvasPresenter =
            new OpenVisionBitmapCanvasPresenter("OpenVisionLab_Workspace", "Workspace");
        private readonly IDisplayManager displayManager;
        private ImageSpaceImageLease currentImageLease;
        private bool disposed;

        public OpenVisionShellHostWorkspacePreviewController(IDisplayManager displayManager)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
        }

        public object CanvasViewModel => canvasPresenter.CanvasViewModel;

        public bool HasImage => canvasPresenter.HasImage;

        public int TextureTileCount => canvasPresenter.TextureTileCount;

        public void SetLayer(OpenVisionShellHostLayerDetailState detail, Action<Bitmap> rebindBorrower)
        {
            ImageSpaceImageLease nextLease = string.IsNullOrWhiteSpace(detail?.LayerTitle)
                ? null
                : displayManager.ImageSpace.AcquireImage(detail.LayerTitle);
            ImageSpaceImageLease previousLease = currentImageLease;
            currentImageLease = nextLease;
            Bitmap image = nextLease?.Image;
            try
            {
                canvasPresenter.SetBitmap(image, detail?.LayerTitle);
                rebindBorrower?.Invoke(image);
            }
            finally
            {
                previousLease?.Dispose();
            }
        }

        public void RefreshCanvas()
        {
            canvasPresenter.RefreshCanvas();
        }

        public bool SaveCurrentImage(string path)
        {
            return canvasPresenter.SaveCurrentImage(path);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            canvasPresenter.Dispose();
            currentImageLease?.Dispose();
            currentImageLease = null;
        }
    }
}
