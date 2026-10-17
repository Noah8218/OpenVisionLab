using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        private readonly bool usePixelZoomRange;
        private readonly ScaleTransform zoomTransform = new ScaleTransform(MinimumZoomScale, MinimumZoomScale);
        private readonly TranslateTransform panTransform = new TranslateTransform();
        private DrawingBitmap statusBitmap;
        private double zoomScale = MinimumZoomScale;
        private double lastPixelZoomScale;
        private bool isFitImage = true;
        private bool isPanning;
        private Point panStartPoint;
        private double panStartX;
        private double panStartY;
        private bool disposed;

        public OpenVisionZoomableImageController(
            FrameworkElement eventSurface,
            Image image,
            Action<OpenVisionZoomableImageStatus> statusChanged = null,
            bool usePixelZoomRange = false)
        {
            this.eventSurface = eventSurface ?? throw new ArgumentNullException(nameof(eventSurface));
            this.image = image ?? throw new ArgumentNullException(nameof(image));
            this.statusChanged = statusChanged;
            this.usePixelZoomRange = usePixelZoomRange;

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(zoomTransform);
            transformGroup.Children.Add(panTransform);
            image.RenderTransform = transformGroup;
            image.RenderTransformOrigin = new Point(0D, 0D);
            image.IsHitTestVisible = true;

            eventSurface.MouseWheel += OnMouseWheel;
            eventSurface.MouseDown += OnMouseDown;
            eventSurface.MouseMove += OnMouseMove;
            eventSurface.MouseUp += OnMouseUp;
            eventSurface.MouseLeave += OnMouseLeave;
            image.SizeChanged += OnImageSizeChanged;
        }

        public event EventHandler ZoomChanged;
        public event EventHandler ViewportChanged;
        internal OpenVisionImageViewport? CaptureViewport()
        {
            if (disposed || statusBitmap == null || PixelZoomScale <= 0D || !TryTransformSurfaceToImage(new Point(eventSurface.ActualWidth / 2D, eventSurface.ActualHeight / 2D), out Point center)) { return null; }
            Rect rect = GetDisplayedImageRect();
            return new OpenVisionImageViewport(PixelZoomScale, (center.X - rect.Left) * statusBitmap.Width / rect.Width, (center.Y - rect.Top) * statusBitmap.Height / rect.Height, isFitImage);
        }
        internal bool TryMapImagePoint(Point sourcePoint, out Point surfacePoint)
        {
            surfacePoint = default;
            if (disposed || statusBitmap == null || image.ActualWidth <= 0D || image.ActualHeight <= 0D || !double.IsFinite(sourcePoint.X) || !double.IsFinite(sourcePoint.Y)) { return false; }
            Rect rect = GetDisplayedImageRect();
            surfacePoint = TransformImageToSurface(new Point(rect.Left + sourcePoint.X * rect.Width / statusBitmap.Width, rect.Top + sourcePoint.Y * rect.Height / statusBitmap.Height));
            return double.IsFinite(surfacePoint.X) && double.IsFinite(surfacePoint.Y);
        }
        internal bool ApplyViewport(OpenVisionImageViewport viewport)
        {
            double fit = GetFitPixelScale(image.RenderSize);
            if (disposed || statusBitmap == null || fit <= 0D || !viewport.IsValid) { return false; }
            if (viewport.Fit) { Reset(); return true; }
            isFitImage = false;
            zoomScale = Math.Clamp(viewport.Scale, 0.05D, 40D) / fit;
            zoomTransform.ScaleX = zoomTransform.ScaleY = zoomScale;
            panTransform.X = panTransform.Y = 0D;
            Rect rect = GetDisplayedImageRect();
            Point center = TransformImageToSurface(new Point(rect.Left + viewport.CenterX * rect.Width / statusBitmap.Width, rect.Top + viewport.CenterY * rect.Height / statusBitmap.Height));
            ApplyPanDelta(eventSurface.ActualWidth / 2D - center.X, eventSurface.ActualHeight / 2D - center.Y);
            lastPixelZoomScale = PixelZoomScale;
            ZoomChanged?.Invoke(this, EventArgs.Empty);
            ViewportChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        // Displayed WPF DIP per source pixel, matching the proposal's CSS logical-pixel scale.
        public double PixelZoomScale => GetFitPixelScale(image.RenderSize) * zoomScale;

        public void ZoomImage(double factor)
        {
            ApplyZoomAt(new Point(eventSurface.ActualWidth / 2D, eventSurface.ActualHeight / 2D), factor);
        }

        public void ShowOriginalSize()
        {
            double currentScale = PixelZoomScale;
            if (currentScale > 0D) { ZoomImage(1D / currentScale); }
        }

        public void Reset()
        {
            if (disposed) { return; }
            isFitImage = true;
            zoomScale = MinimumZoomScale;
            zoomTransform.ScaleX = MinimumZoomScale;
            zoomTransform.ScaleY = MinimumZoomScale;
            panTransform.X = 0D;
            panTransform.Y = 0D;
            EndPan();
            PublishEmptyStatus();
            lastPixelZoomScale = PixelZoomScale;
            ZoomChanged?.Invoke(this, EventArgs.Empty);
            ViewportChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetStatusBitmap(DrawingBitmap bitmap)
        {
            statusBitmap = bitmap;
            PublishEmptyStatus();
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
            eventSurface.MouseWheel -= OnMouseWheel;
            eventSurface.MouseDown -= OnMouseDown;
            eventSurface.MouseMove -= OnMouseMove;
            eventSurface.MouseUp -= OnMouseUp;
            eventSurface.MouseLeave -= OnMouseLeave;
            image.SizeChanged -= OnImageSizeChanged;
            ZoomChanged = null;
            ViewportChanged = null;
            statusBitmap = null;
            image.RenderTransform = null;
        }

        private double GetFitPixelScale(Size size)
        {
            if (image.Source is not BitmapSource bitmap || image.Visibility != Visibility.Visible || size.Width <= 0D || size.Height <= 0D)
            {
                return 0D;
            }
            return Math.Min(size.Width / bitmap.PixelWidth, size.Height / bitmap.PixelHeight);
        }

        private void OnImageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (disposed) { return; }
            double fitScale = GetFitPixelScale(e.NewSize);
            if (usePixelZoomRange && !isFitImage && lastPixelZoomScale > 0D && fitScale > 0D)
            {
                // Retain the absolute scale and the image point at the centered viewport.
                zoomScale = lastPixelZoomScale / fitScale;
                zoomTransform.ScaleX = zoomScale;
                zoomTransform.ScaleY = zoomScale;
                panTransform.X += (e.NewSize.Width - e.PreviousSize.Width) / 2D;
                panTransform.Y += (e.NewSize.Height - e.PreviousSize.Height) / 2D;
            }
            lastPixelZoomScale = PixelZoomScale;
            ZoomChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
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
            for (DependencyObject source = e.OriginalSource as DependencyObject; source != null && source != eventSurface;
                source = source is Visual ? VisualTreeHelper.GetParent(source) : LogicalTreeHelper.GetParent(source))
            {
                if (source is ScrollViewer) { return; }
            }

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
            if (disposed || !(factor > 0D) || double.IsInfinity(factor) || image.Source == null || image.Visibility != Visibility.Visible
                || !TryTransformSurfaceToImage(anchorPoint, out Point imageAnchorPoint))
            {
                return;
            }

            double fitScale = GetFitPixelScale(image.RenderSize);
            if (usePixelZoomRange && fitScale <= 0D) { return; }
            double minimum = usePixelZoomRange ? 0.05D / fitScale : MinimumZoomScale;
            double maximum = usePixelZoomRange ? 40D / fitScale : MaximumZoomScale;
            double nextZoom = Math.Max(minimum, Math.Min(maximum, zoomScale * factor));
            if (!usePixelZoomRange && Math.Abs(nextZoom - MinimumZoomScale) < 0.001D)
            {
                Reset();
                return;
            }

            isFitImage = false;
            zoomScale = nextZoom;
            zoomTransform.ScaleX = nextZoom;
            zoomTransform.ScaleY = nextZoom;
            Point shiftedAnchorPoint = TransformImageToSurface(imageAnchorPoint);
            ApplyPanDelta(anchorPoint.X - shiftedAnchorPoint.X, anchorPoint.Y - shiftedAnchorPoint.Y);
            lastPixelZoomScale = PixelZoomScale;
            ZoomChanged?.Invoke(this, EventArgs.Empty);
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
            if (statusChanged == null) { return false; }
            if (!TryGetPixelStatus(surfacePoint, out OpenVisionZoomableImageStatus status))
            {
                PublishEmptyStatus();
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
            // Normalize in the pointer's surface coordinates to avoid inverse-transform
            // cancellation at exact pixel boundaries under the viewer's scale/translation.
            GeneralTransform imageToSurface = image.TransformToAncestor(eventSurface);
            if (imageToSurface is Transform affine && affine.Value.M12 == 0D && affine.Value.M21 == 0D
                && affine.Value.M11 > 0D && affine.Value.M22 > 0D)
            {
                imageRect = imageToSurface.TransformBounds(imageRect);
                imagePoint = surfacePoint;
            }
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
            ViewportChanged?.Invoke(this, EventArgs.Empty);
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

    public readonly record struct OpenVisionImageViewport(double Scale, double CenterX, double CenterY, bool Fit)
    {
        public bool IsValid => double.IsFinite(Scale) && Scale > 0D && double.IsFinite(CenterX) && double.IsFinite(CenterY);
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
