using OpenVisionLab.ImageCanvas;
using OpenVisionLab.Contracts;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Model;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using OpenVisionLab.ImageCanvas.ViewModels;
using OpenVisionLab.ImageCanvas.Views;
using System.Drawing;
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

            View = new RoiImageCanvasView
            {
                DataContext = canvasViewModel,
                ShowStatusBar = false,
                ShowToolBar = false
            };
        }

        public FrameworkElement View { get; }

        public int TextureTileCount => canvasViewModel.TextureTileCount;

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

            canvasViewModel.AddOverlay(
                string.Empty,
                groupType,
                rect,
                overlayId,
                ToCanvasOverlayKind(overlayKind),
                EnumItemType.Window);
        }

        public void DeleteOverlay(string overlayId)
        {
            canvasViewModel.DeleteOverlay(overlayId);
        }

        public void Refresh()
        {
            canvasViewModel.RefreshCanvas();
        }

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;

            if (View is RoiImageCanvasView canvasView)
            {
                canvasView.DataContext = null;
                canvasView.Dispose();
            }

            canvasViewModel.Dispose();
        }

        private static EnumInspWindowType ToCanvasOverlayKind(VisionToolPreviewOverlayKind overlayKind)
        {
            return overlayKind == VisionToolPreviewOverlayKind.Align
                ? EnumInspWindowType.Align
                : EnumInspWindowType.Unit;
        }
    }
}
