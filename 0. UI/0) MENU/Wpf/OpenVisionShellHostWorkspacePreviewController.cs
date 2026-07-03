using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostWorkspacePreviewController : IDisposable
    {
        private readonly OpenVisionBitmapCanvasPresenter canvasPresenter =
            new OpenVisionBitmapCanvasPresenter("OpenVisionLab_Workspace", "Workspace");
        private bool disposed;

        public object CanvasViewModel => canvasPresenter.CanvasViewModel;

        public bool HasImage => canvasPresenter.HasImage;

        public int TextureTileCount => canvasPresenter.TextureTileCount;

        public void SetLayer(OpenVisionShellHostLayerDetailState detail)
        {
            canvasPresenter.SetBitmap(detail?.Image, detail?.LayerTitle);
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
        }
    }
}
