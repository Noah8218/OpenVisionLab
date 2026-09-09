using System;
using System.ComponentModel;
using System.Windows;

namespace OpenVisionLab
{
    public partial class VisionToolNImageVerificationWindow : Window
    {
        private readonly VisionToolNImageVerificationController controller;
        private readonly OpenVisionZoomableImageController sourceImageZoomController;
        private readonly OpenVisionZoomableImageController drawingImageZoomController;

        internal VisionToolNImageVerificationWindow(VisionToolNImageVerificationController controller)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            InitializeComponent();
            sourceImageZoomController = new OpenVisionZoomableImageController(
                sourceImageSurface,
                sourceImagePreview);
            drawingImageZoomController = new OpenVisionZoomableImageController(
                drawingImageSurface,
                drawingImagePreview);
            this.controller.PropertyChanged += OnControllerPropertyChanged;
            DataContext = this.controller;
            Closed += OnClosed;
        }

        internal void ZoomSourceImageForTest(double factor)
        {
            sourceImageZoomController.ZoomAtForTest(GetSurfaceCenter(sourceImageSurface), factor);
        }

        internal void PanSourceImageForTest(double deltaX, double deltaY)
        {
            sourceImageZoomController.PanByForTest(deltaX, deltaY);
        }

        internal void ResetSourceImageForTest()
        {
            sourceImageZoomController.Reset();
        }

        internal void ZoomDrawingImageForTest(double factor)
        {
            drawingImageZoomController.ZoomAtForTest(GetSurfaceCenter(drawingImageSurface), factor);
        }

        private void OnControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(VisionToolNImageVerificationController.SelectedSourceImage), StringComparison.Ordinal))
            {
                sourceImageZoomController.Reset();
            }
            else if (string.Equals(e.PropertyName, nameof(VisionToolNImageVerificationController.SelectedDrawingImage), StringComparison.Ordinal))
            {
                drawingImageZoomController.Reset();
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Closed -= OnClosed;
            controller.PropertyChanged -= OnControllerPropertyChanged;
            sourceImageZoomController.Dispose();
            drawingImageZoomController.Dispose();
            controller.Dispose();
        }

        private static Point GetSurfaceCenter(FrameworkElement surface)
        {
            return new Point(surface.ActualWidth / 2D, surface.ActualHeight / 2D);
        }
    }
}
