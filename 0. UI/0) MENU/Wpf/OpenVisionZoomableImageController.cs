using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingColor = System.Drawing.Color;

namespace OpenVisionLab
{
    // Navigation-only image surface for WPF fallback viewers.
    // Keep ROI editing out of this shared viewer; tool-specific ROI belongs in the owning tool.
    // If measurement is added here later, report pixel distance only because layer resolution is unknown.
    internal sealed class OpenVisionZoomableImageController : IDisposable
    {
        private const double MinimumZoomScale = 1D;
        private const double MaximumZoomScale = 32D;

        private readonly FrameworkElement eventSurface;
        private readonly Image image;
        private readonly Action<OpenVisionZoomableImageStatus> statusChanged;
        private readonly ScaleTransform zoomTransform = new ScaleTransform(MinimumZoomScale, MinimumZoomScale);
        private readonly TranslateTransform panTransform = new TranslateTransform();
        private DrawingBitmap statusBitmap;
        private double zoomScale = MinimumZoomScale;
        private bool isPanning;
        private Point panStartPoint;
        private double panStartX;
        private double panStartY;
        private bool disposed;

        public OpenVisionZoomableImageController(
            FrameworkElement eventSurface,
            Image image,
            Action<OpenVisionZoomableImageStatus> statusChanged = null)
        {
            this.eventSurface = eventSurface ?? throw new ArgumentNullException(nameof(eventSurface));
            this.image = image ?? throw new ArgumentNullException(nameof(image));
            this.statusChanged = statusChanged;

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(zoomTransform);
            transformGroup.Children.Add(panTransform);
            image.RenderTransform = transformGroup;
            image.RenderTransformOrigin = new Point(0D, 0D);
            image.IsHitTestVisible = true;

            eventSurface.PreviewMouseWheel += OnPreviewMouseWheel;
            eventSurface.MouseDown += OnMouseDown;
            eventSurface.MouseMove += OnMouseMove;
            eventSurface.MouseUp += OnMouseUp;
            eventSurface.MouseLeave += OnMouseLeave;
        }

        public void Reset()
        {
            zoomScale = MinimumZoomScale;
            zoomTransform.ScaleX = MinimumZoomScale;
            zoomTransform.ScaleY = MinimumZoomScale;
            panTransform.X = 0D;
            panTransform.Y = 0D;
            EndPan();
        }

        public void SetStatusBitmap(DrawingBitmap bitmap)
        {
            statusBitmap = bitmap;
            if (bitmap == null)
            {
                PublishEmptyStatus();
            }
        }

        public bool UpdatePointerStatusForTest(Point surfacePoint)
        {
            return PublishPointerStatus(surfacePoint);
        }

        public bool TryGetPointerStatusForTest(Point surfacePoint, out OpenVisionZoomableImageStatus status)
        {
            return TryGetPixelStatus(surfacePoint, out status);
        }

        public void ZoomAtForTest(Point surfacePoint, double factor)
        {
            ApplyZoomAt(surfacePoint, factor);
        }

        public void PanByForTest(double surfaceDeltaX, double surfaceDeltaY)
        {
            if (image.Source == null || image.Visibility != Visibility.Visible)
            {
                return;
            }

            ApplyPanDelta(surfaceDeltaX, surfaceDeltaY);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            eventSurface.PreviewMouseWheel -= OnPreviewMouseWheel;
            eventSurface.MouseDown -= OnMouseDown;
            eventSurface.MouseMove -= OnMouseMove;
            eventSurface.MouseUp -= OnMouseUp;
            eventSurface.MouseLeave -= OnMouseLeave;
            statusBitmap = null;
            image.RenderTransform = null;
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (image.Source == null || image.Visibility != Visibility.Visible)
            {
                return;
            }

            ApplyZoomAt(e.GetPosition(eventSurface), e.Delta > 0 ? 1.2D : 1D / 1.2D);
            e.Handled = true;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (image.Source != null
                && image.Visibility == Visibility.Visible
                && e.ChangedButton == MouseButton.Left
                && e.ClickCount >= 2)
            {
                Reset();
                e.Handled = true;
                return;
            }

            if (image.Source == null
                || image.Visibility != Visibility.Visible
                || (e.ChangedButton != MouseButton.Left && e.ChangedButton != MouseButton.Middle))
            {
                return;
            }

            isPanning = true;
            panStartPoint = e.GetPosition(eventSurface);
            panStartX = panTransform.X;
            panStartY = panTransform.Y;
            eventSurface.CaptureMouse();
            e.Handled = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!isPanning)
            {
                PublishPointerStatus(e.GetPosition(eventSurface));
                return;
            }

            Point currentPoint = e.GetPosition(eventSurface);
            ApplyPanFromStart(currentPoint.X - panStartPoint.X, currentPoint.Y - panStartPoint.Y);
            PublishPointerStatus(currentPoint);
            e.Handled = true;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Middle)
            {
                EndPan();
                e.Handled = true;
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            EndPan();
            PublishEmptyStatus();
        }

        private void ApplyZoomAt(Point anchorPoint, double factor)
        {
            if (!TryTransformSurfaceToImage(anchorPoint, out Point imageAnchorPoint))
            {
                return;
            }

            double nextZoom = Math.Max(MinimumZoomScale, Math.Min(MaximumZoomScale, zoomScale * factor));
            if (Math.Abs(nextZoom - MinimumZoomScale) < 0.001D)
            {
                Reset();
                return;
            }

            zoomScale = nextZoom;
            zoomTransform.ScaleX = nextZoom;
            zoomTransform.ScaleY = nextZoom;
            Point shiftedAnchorPoint = TransformImageToSurface(imageAnchorPoint);
            ApplyPanDelta(anchorPoint.X - shiftedAnchorPoint.X, anchorPoint.Y - shiftedAnchorPoint.Y);
        }

        private void EndPan()
        {
            if (!isPanning)
            {
                return;
            }

            isPanning = false;
            if (eventSurface.IsMouseCaptured)
            {
                eventSurface.ReleaseMouseCapture();
            }
        }

        private bool PublishPointerStatus(Point surfacePoint)
        {
            if (statusChanged == null || !TryGetPixelStatus(surfacePoint, out OpenVisionZoomableImageStatus status))
            {
                return false;
            }

            statusChanged(status);
            return true;
        }

        private void PublishEmptyStatus()
        {
            statusChanged?.Invoke(OpenVisionZoomableImageStatus.Empty);
        }

        private bool TryGetPixelStatus(Point surfacePoint, out OpenVisionZoomableImageStatus status)
        {
            status = OpenVisionZoomableImageStatus.Empty;
            if (statusBitmap == null
                || image.Source == null
                || image.Visibility != Visibility.Visible
                || image.ActualWidth <= 0D
                || image.ActualHeight <= 0D)
            {
                return false;
            }

            Point imagePoint;
            if (!TryTransformSurfaceToImage(surfacePoint, out imagePoint))
            {
                return false;
            }

            Rect imageRect = GetDisplayedImageRect();
            if (!imageRect.Contains(imagePoint) || imageRect.Width <= 0D || imageRect.Height <= 0D)
            {
                return false;
            }

            int x = ClampToPixel((imagePoint.X - imageRect.Left) / imageRect.Width, statusBitmap.Width);
            int y = ClampToPixel((imagePoint.Y - imageRect.Top) / imageRect.Height, statusBitmap.Height);
            DrawingColor color = statusBitmap.GetPixel(x, y);
            int gray = (int)Math.Round(color.R * 0.299D + color.G * 0.587D + color.B * 0.114D);
            status = new OpenVisionZoomableImageStatus(true, x, y, gray, color.R, color.G, color.B);
            return true;
        }

        private Rect GetDisplayedImageRect()
        {
            double imageAspect = statusBitmap.Width / (double)statusBitmap.Height;
            double controlAspect = image.ActualWidth / image.ActualHeight;
            if (controlAspect > imageAspect)
            {
                double displayedWidth = image.ActualHeight * imageAspect;
                return new Rect((image.ActualWidth - displayedWidth) / 2D, 0D, displayedWidth, image.ActualHeight);
            }

            double displayedHeight = image.ActualWidth / imageAspect;
            return new Rect(0D, (image.ActualHeight - displayedHeight) / 2D, image.ActualWidth, displayedHeight);
        }

        private static int ClampToPixel(double ratio, int length)
        {
            if (length <= 1)
            {
                return 0;
            }

            return Math.Max(0, Math.Min(length - 1, (int)Math.Floor(ratio * length)));
        }

        private bool TryTransformSurfaceToImage(Point surfacePoint, out Point imagePoint)
        {
            imagePoint = default;
            try
            {
                GeneralTransform inverse = image.TransformToAncestor(eventSurface)?.Inverse;
                if (inverse == null)
                {
                    return false;
                }

                imagePoint = inverse.Transform(surfacePoint);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private Point TransformImageToSurface(Point imagePoint)
        {
            try
            {
                return image.TransformToAncestor(eventSurface).Transform(imagePoint);
            }
            catch (InvalidOperationException)
            {
                return imagePoint;
            }
        }

        private void ApplyPanFromStart(double surfaceDeltaX, double surfaceDeltaY)
        {
            panTransform.X = panStartX;
            panTransform.Y = panStartY;
            ApplyPanDelta(surfaceDeltaX, surfaceDeltaY);
        }

        private void ApplyPanDelta(double surfaceDeltaX, double surfaceDeltaY)
        {
            double surfacePerPanX = GetSurfaceDeltaPerPanX();
            double surfacePerPanY = GetSurfaceDeltaPerPanY();
            panTransform.X += Math.Abs(surfacePerPanX) > 0.0001D ? surfaceDeltaX / surfacePerPanX : surfaceDeltaX;
            panTransform.Y += Math.Abs(surfacePerPanY) > 0.0001D ? surfaceDeltaY / surfacePerPanY : surfaceDeltaY;
        }

        private double GetSurfaceDeltaPerPanX()
        {
            Point before = TransformImageToSurface(new Point(0D, 0D));
            double oldPan = panTransform.X;
            panTransform.X = oldPan + 1D;
            Point after = TransformImageToSurface(new Point(0D, 0D));
            panTransform.X = oldPan;
            return after.X - before.X;
        }

        private double GetSurfaceDeltaPerPanY()
        {
            Point before = TransformImageToSurface(new Point(0D, 0D));
            double oldPan = panTransform.Y;
            panTransform.Y = oldPan + 1D;
            Point after = TransformImageToSurface(new Point(0D, 0D));
            panTransform.Y = oldPan;
            return after.Y - before.Y;
        }
    }

    internal sealed class OpenVisionZoomableImageStatus
    {
        public static readonly OpenVisionZoomableImageStatus Empty = new OpenVisionZoomableImageStatus(false, 0, 0, 0, 0, 0, 0);

        public OpenVisionZoomableImageStatus(bool hasPixel, int imageX, int imageY, int grayValue, int red, int green, int blue)
        {
            HasPixel = hasPixel;
            ImageX = imageX;
            ImageY = imageY;
            GrayValue = grayValue;
            Red = red;
            Green = green;
            Blue = blue;
        }

        public bool HasPixel { get; }

        public int ImageX { get; }

        public int ImageY { get; }

        public int GrayValue { get; }

        public int Red { get; }

        public int Green { get; }

        public int Blue { get; }

        public string FormatCoordinates()
        {
            return string.Format(CultureInfo.InvariantCulture, "X:{0} Y:{1}", ImageX, ImageY);
        }

        public string FormatPixel()
        {
            return string.Format(CultureInfo.InvariantCulture, "GV {0} | RGB {1},{2},{3}", GrayValue, Red, Green, Blue);
        }
    }
}
