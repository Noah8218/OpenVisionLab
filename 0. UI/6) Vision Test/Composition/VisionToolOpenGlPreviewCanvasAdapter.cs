using OpenVisionLab.ImageCanvas;
using OpenVisionLab.Contracts;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Model;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using OpenVisionLab.ImageCanvas.ViewModels;
using OpenVisionLab.ImageCanvas.Views;
using System.Drawing;
using System.Linq;
using System.Windows;

namespace OpenVisionLab.Composition
{
    internal sealed class VisionToolOpenGlPreviewCanvasAdapter : IVisionToolOpenGlPreviewCanvas
    {
        // This adapter owns the ImageCanvas DataContext; tool views only talk to the preview canvas contract.
        private readonly RoiImageCanvasViewModel canvasViewModel;
        private bool disposed;

        public VisionToolOpenGlPreviewCanvasAdapter(string textureName)
        {
            canvasViewModel = new RoiImageCanvasViewModel(textureName)
            {
                ShowGroupNames = false,
                ShowRoiItemNames = false
            };
            ConfigureHostedImageViewer();

            View = new RoiImageCanvasView
            {
                DataContext = canvasViewModel,
                ShowStatusBar = false,
                ShowToolBar = false
            };
        }

        public FrameworkElement View { get; }

        public int TextureTileCount => canvasViewModel.ImageViewer.TextureAreas.Values.Sum(items => items?.Count ?? 0);

        public void LoadImage(Bitmap image, string textureName)
        {
            canvasViewModel.LoadImage(image, textureName);
        }

        public void ClearImage()
        {
            canvasViewModel.ClearImage();
        }

        public void FitImageToView()
        {
            canvasViewModel.FitImageToView();
        }

        public void AddRectangleOverlay(
            string overlayId,
            string groupType,
            int imageHeight,
            int left,
            int top,
            int right,
            int bottom,
            bool isSelected,
            VisionToolPreviewOverlayKind overlayKind)
        {
            int canvasTop = imageHeight - top;
            int canvasBottom = imageHeight - bottom;
            CanvasRect<float> rect = new CanvasRect<float>(left, canvasTop, right, canvasBottom)
            {
                LineWidth = isSelected ? 2.6f : 1.4f
            };

            canvasViewModel.ImageViewer.AddOverlay(
                string.Empty,
                groupType,
                rect,
                overlayId,
                ToCanvasOverlayKind(overlayKind),
                EnumItemType.Window);
        }

        public void DeleteOverlay(string overlayId)
        {
            canvasViewModel.ImageViewer.DeleteOverlay(overlayId, string.Empty);
        }

        public void Refresh()
        {
            canvasViewModel.ImageViewer.RefreshGL();
        }

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;

            if (View is RoiImageCanvasView canvasView)
            {
                canvasView.DataContext = null;
            }

            canvasViewModel.Dispose();
        }

        private void ConfigureHostedImageViewer()
        {
            // Tool previews live beside WPF comboboxes; keep the native OpenGL HWND inside its WPF slot.
            canvasViewModel.ImageViewer.AutoSize = false;
            canvasViewModel.ImageViewer.MinimumSize = System.Drawing.Size.Empty;
            canvasViewModel.ImageViewer.Dock = System.Windows.Forms.DockStyle.Fill;
        }

        private static EnumInspWindowType ToCanvasOverlayKind(VisionToolPreviewOverlayKind overlayKind)
        {
            return overlayKind == VisionToolPreviewOverlayKind.Align
                ? EnumInspWindowType.Align
                : EnumInspWindowType.Unit;
        }
    }
}
