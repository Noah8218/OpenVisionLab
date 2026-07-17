using OpenVisionLab.Contracts;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OpenVisionLab
{
    public class VisionToolInlinePreviewSlot : UserControl
    {
        private const double MinimumZoomScale = 1D;
        private const double MaximumZoomScale = 32D;

        private readonly Grid contentLayer;
        private readonly System.Windows.Controls.Image previewImage;
        private readonly Canvas overlayCanvas;
        private readonly ScaleTransform zoomTransform;
        private readonly TranslateTransform panTransform;
        private readonly List<PreviewOverlayRect> overlays = new List<PreviewOverlayRect>();
        private Bitmap pendingImage;
        private double zoomScale = MinimumZoomScale;
        private bool isPanning;
        private bool hasPanMoved;
        private System.Windows.Point panStartPoint;
        private double panStartX;
        private double panStartY;
        private bool isLoaded;
        private bool uploadScheduled;
        private bool disposed;

        public VisionToolInlinePreviewSlot()
        {
            Grid host = new Grid
            {
                Background = System.Windows.Media.Brushes.Black,
                ClipToBounds = true
            };
            contentLayer = new Grid
            {
                ClipToBounds = true,
                RenderTransformOrigin = new System.Windows.Point(0D, 0D)
            };
            TransformGroup transformGroup = new TransformGroup();
            zoomTransform = new ScaleTransform(MinimumZoomScale, MinimumZoomScale);
            panTransform = new TranslateTransform();
            transformGroup.Children.Add(zoomTransform);
            transformGroup.Children.Add(panTransform);
            contentLayer.RenderTransform = transformGroup;

            previewImage = new System.Windows.Controls.Image
            {
                Stretch = Stretch.Uniform,
                SnapsToDevicePixels = true
            };
            overlayCanvas = new Canvas
            {
                IsHitTestVisible = false
            };
            contentLayer.Children.Add(previewImage);
            contentLayer.Children.Add(overlayCanvas);
            host.Children.Add(contentLayer);

            Content = host;
            Focusable = true;
            ClipToBounds = true;
            Visibility = Visibility.Hidden;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            SizeChanged += OnSizeChanged;
            PreviewMouseWheel += OnPreviewMouseWheel;
            MouseDoubleClick += OnMouseDoubleClick;
            PreviewMouseUp += OnPreviewMouseUp;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseLeave += OnMouseLeave;
            LostMouseCapture += OnLostMouseCapture;
        }

        public event EventHandler ImageChanged = delegate { };

        public bool HasImage { get; private set; }

        public int ImagePixelWidth { get; private set; }

        public int ImagePixelHeight { get; private set; }

        public int TextureTileCount => HasImage ? 1 : 0;

        public int RoiOverlayCount => overlays.Count;

        public bool LastMouseUpWasPanGesture { get; private set; }

        public void SetImage(Bitmap image, string imageName = null)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => SetImage(image, imageName));
                return;
            }

            ClearPendingImage();

            if (image == null)
            {
                ClearImage();
                return;
            }

            bool resetView = !HasImage || ImagePixelWidth != image.Width || ImagePixelHeight != image.Height;
            ImagePixelWidth = image.Width;
            ImagePixelHeight = image.Height;
            pendingImage = (Bitmap)image.Clone();
            HasImage = true;
            Visibility = Visibility.Visible;
            if (resetView)
            {
                ResetViewTransform();
            }

            ImageChanged(this, EventArgs.Empty);

            SchedulePendingImageUpload();
        }

        public void ClearImage()
        {
            ClearRoiOverlays();
            ClearPendingImage();
            ImagePixelWidth = 0;
            ImagePixelHeight = 0;
            HasImage = false;
            previewImage.Source = null;
            ResetViewTransform();
            Visibility = Visibility.Hidden;
            ImageChanged(this, EventArgs.Empty);
        }

        public void FitImageToView()
        {
            ResetViewTransform();
        }

        public bool TryGetContentPointForTest(double xRatio, double yRatio, out System.Windows.Point contentPoint)
        {
            if (!Dispatcher.CheckAccess())
            {
                var result = Dispatcher.Invoke(() =>
                {
                    bool success = TryGetContentPointForTest(xRatio, yRatio, out System.Windows.Point nestedPoint);
                    return (Success: success, Point: nestedPoint);
                });
                contentPoint = result.Point;
                return result.Success;
            }

            contentPoint = default;
            if (ActualWidth <= 0D || ActualHeight <= 0D)
            {
                return false;
            }

            return TryTransformSlotToContent(CreateSurfacePointForTest(xRatio, yRatio), out contentPoint);
        }

        public void ZoomAtForTest(double xRatio, double yRatio, double factor)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => ZoomAtForTest(xRatio, yRatio, factor));
                return;
            }

            ApplyZoomAt(CreateSurfacePointForTest(xRatio, yRatio), factor);
        }

        public void PanByForTest(double surfaceDeltaX, double surfaceDeltaY)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => PanByForTest(surfaceDeltaX, surfaceDeltaY));
                return;
            }

            if (!HasImage)
            {
                return;
            }

            ApplyPanDelta(surfaceDeltaX, surfaceDeltaY);
        }

        public void SetLineRoiOverlays(
            OpenCvSharp.Rect lineA,
            OpenCvSharp.Rect lineB,
            bool isLineBSelected)
        {
            // This is a tool-owned ROI visualization for Line teaching, not a generic viewer ROI editor.
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => SetLineRoiOverlays(lineA, lineB, isLineBSelected));
                return;
            }

            ClearRoiOverlays();
            if (!HasImage || ImagePixelWidth <= 0 || ImagePixelHeight <= 0)
            {
                return;
            }

            AddLineRoiOverlay(lineA, "Line_A_ROI", VisionToolPreviewOverlayKind.Unit, !isLineBSelected);
            AddLineRoiOverlay(lineB, "Line_B_ROI", VisionToolPreviewOverlayKind.Align, isLineBSelected);
            RenderOverlays();
        }

        public void SetOpenCvRoiOverlays(OpenCvPropertyBase property)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => SetOpenCvRoiOverlays(property));
                return;
            }

            ClearRoiOverlays();
            if (!HasImage || ImagePixelWidth <= 0 || ImagePixelHeight <= 0 || property == null || !property.USE_ROI)
            {
                return;
            }

            if (property.USE_MULTI_ROI)
            {
                int index = 0;
                foreach (OpenCvSharp.Rect roi in property.CvROIS ?? new List<OpenCvSharp.Rect>())
                {
                    AddLineRoiOverlay(roi, "ROI_" + index.ToString(System.Globalization.CultureInfo.InvariantCulture), VisionToolPreviewOverlayKind.Unit, true);
                    index++;
                }
            }
            else
            {
                AddLineRoiOverlay(property.CvROI, "ROI", VisionToolPreviewOverlayKind.Unit, true);
            }

            RenderOverlays();
        }

        public void ClearRoiOverlays()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ClearRoiOverlays);
                return;
            }

            overlays.Clear();
            overlayCanvas.Children.Clear();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;
            SchedulePendingImageUpload();
            RenderOverlays();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RenderOverlays();
        }

        private void SchedulePendingImageUpload()
        {
            if (!isLoaded || pendingImage == null || uploadScheduled)
            {
                return;
            }

            uploadScheduled = true;
            Dispatcher.BeginInvoke(new Action(UploadPendingImage), DispatcherPriority.Loaded);
        }

        private void UploadPendingImage()
        {
            uploadScheduled = false;
            if (pendingImage == null)
            {
                return;
            }

            Bitmap image = pendingImage;
            pendingImage = null;
            try
            {
                LoadBitmap(image);
            }
            finally
            {
                image.Dispose();
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            isLoaded = false;
            // Tool panels are cached and reparented between floating hosts. WPF raises
            // Unloaded during that move, so keep subscriptions/images alive until Dispose().
        }

        public void DisposeView()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
            SizeChanged -= OnSizeChanged;
            PreviewMouseWheel -= OnPreviewMouseWheel;
            MouseDoubleClick -= OnMouseDoubleClick;
            PreviewMouseUp -= OnPreviewMouseUp;
            MouseDown -= OnMouseDown;
            MouseMove -= OnMouseMove;
            MouseUp -= OnMouseUp;
            MouseLeave -= OnMouseLeave;
            LostMouseCapture -= OnLostMouseCapture;
            ClearPendingImage();
            overlayCanvas.Children.Clear();
            previewImage.Source = null;
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!HasImage)
            {
                return;
            }

            ApplyZoomAt(e.GetPosition(this), e.Delta > 0 ? 1.2D : 1D / 1.2D);
            Focus();
            e.Handled = true;
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!HasImage)
            {
                return;
            }

            ResetViewTransform();
            e.Handled = true;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!HasImage || (e.ChangedButton != MouseButton.Left && e.ChangedButton != MouseButton.Middle))
            {
                return;
            }

            isPanning = true;
            hasPanMoved = false;
            LastMouseUpWasPanGesture = false;
            panStartPoint = e.GetPosition(this);
            panStartX = panTransform.X;
            panStartY = panTransform.Y;
            CaptureMouse();
            e.Handled = e.ChangedButton == MouseButton.Middle;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!isPanning)
            {
                return;
            }

            System.Windows.Point currentPoint = e.GetPosition(this);
            double surfaceDeltaX = currentPoint.X - panStartPoint.X;
            double surfaceDeltaY = currentPoint.Y - panStartPoint.Y;
            if (!hasPanMoved
                && Math.Abs(surfaceDeltaX) < SystemParameters.MinimumHorizontalDragDistance
                && Math.Abs(surfaceDeltaY) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            hasPanMoved = true;
            ApplyPanFromStart(surfaceDeltaX, surfaceDeltaY);
            e.Handled = true;
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            CompletePanGesture(e);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            CompletePanGesture(e);
        }

        private void CompletePanGesture(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Middle)
            {
                if (!isPanning)
                {
                    return;
                }

                bool shouldConsume = e.ChangedButton == MouseButton.Middle || hasPanMoved;
                LastMouseUpWasPanGesture = shouldConsume;
                EndPan();
                // A plain left click still belongs to the tool action layer: input/output preview
                // clicks activate that layer in the main workspace. Only consume real pan gestures.
                e.Handled = shouldConsume;
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            // When the mouse is captured, leaving the slot is still a valid pan operation.
            // Ending here would turn an out-of-bounds drag release into a false output-click.
            if (!IsMouseCaptured)
            {
                EndPan();
            }
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (!isPanning)
            {
                return;
            }

            LastMouseUpWasPanGesture = hasPanMoved || LastMouseUpWasPanGesture;
            EndPan(releaseCapture: false);
        }

        private void ApplyZoomAt(System.Windows.Point anchorPoint, double factor)
        {
            if (!TryTransformSlotToContent(anchorPoint, out System.Windows.Point contentAnchorPoint))
            {
                return;
            }

            double nextZoom = Math.Max(MinimumZoomScale, Math.Min(MaximumZoomScale, zoomScale * factor));
            if (Math.Abs(nextZoom - MinimumZoomScale) < 0.001D)
            {
                ResetViewTransform();
                return;
            }

            zoomScale = nextZoom;
            zoomTransform.ScaleX = nextZoom;
            zoomTransform.ScaleY = nextZoom;
            System.Windows.Point shiftedAnchorPoint = TransformContentToSlot(contentAnchorPoint);
            ApplyPanDelta(anchorPoint.X - shiftedAnchorPoint.X, anchorPoint.Y - shiftedAnchorPoint.Y);
        }

        private System.Windows.Point CreateSurfacePointForTest(double xRatio, double yRatio)
        {
            double x = Math.Max(0D, Math.Min(1D, xRatio)) * ActualWidth;
            double y = Math.Max(0D, Math.Min(1D, yRatio)) * ActualHeight;
            return new System.Windows.Point(x, y);
        }

        private void ResetViewTransform()
        {
            LastMouseUpWasPanGesture = false;
            zoomScale = MinimumZoomScale;
            zoomTransform.ScaleX = MinimumZoomScale;
            zoomTransform.ScaleY = MinimumZoomScale;
            panTransform.X = 0D;
            panTransform.Y = 0D;
            EndPan();
        }

        private void EndPan(bool releaseCapture = true)
        {
            if (!isPanning)
            {
                return;
            }

            isPanning = false;
            hasPanMoved = false;
            if (releaseCapture && IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
        }

        private bool TryTransformSlotToContent(System.Windows.Point slotPoint, out System.Windows.Point contentPoint)
        {
            contentPoint = default;
            try
            {
                GeneralTransform inverse = contentLayer.TransformToAncestor(this)?.Inverse;
                if (inverse == null)
                {
                    return false;
                }

                contentPoint = inverse.Transform(slotPoint);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private System.Windows.Point TransformContentToSlot(System.Windows.Point contentPoint)
        {
            try
            {
                return contentLayer.TransformToAncestor(this).Transform(contentPoint);
            }
            catch (InvalidOperationException)
            {
                return contentPoint;
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
            System.Windows.Point before = TransformContentToSlot(new System.Windows.Point(0D, 0D));
            double oldPan = panTransform.X;
            panTransform.X = oldPan + 1D;
            System.Windows.Point after = TransformContentToSlot(new System.Windows.Point(0D, 0D));
            panTransform.X = oldPan;
            return after.X - before.X;
        }

        private double GetSurfaceDeltaPerPanY()
        {
            System.Windows.Point before = TransformContentToSlot(new System.Windows.Point(0D, 0D));
            double oldPan = panTransform.Y;
            panTransform.Y = oldPan + 1D;
            System.Windows.Point after = TransformContentToSlot(new System.Windows.Point(0D, 0D));
            panTransform.Y = oldPan;
            return after.Y - before.Y;
        }

        private void LoadBitmap(Bitmap image)
        {
            if (image == null)
            {
                ClearImage();
                return;
            }

            previewImage.Source = CreateBitmapSource(image);
            HasImage = true;
            Visibility = Visibility.Visible;
            RenderOverlays();
            ImageChanged(this, EventArgs.Empty);
        }

        private void AddLineRoiOverlay(OpenCvSharp.Rect roi, string groupType, VisionToolPreviewOverlayKind overlayKind, bool isSelected)
        {
            int left = Math.Max(0, roi.X);
            int top = Math.Max(0, roi.Y);
            int right = Math.Min(ImagePixelWidth, roi.X + roi.Width);
            int bottom = Math.Min(ImagePixelHeight, roi.Y + roi.Height);
            if (right <= left || bottom <= top)
            {
                return;
            }

            overlays.Add(new PreviewOverlayRect(left, top, right - left, bottom - top, overlayKind, isSelected, groupType));
        }

        private void RenderOverlays()
        {
            overlayCanvas.Children.Clear();
            if (!HasImage || ImagePixelWidth <= 0 || ImagePixelHeight <= 0 || ActualWidth <= 0 || ActualHeight <= 0)
            {
                return;
            }

            Rect imageRect = GetDisplayedImageRect();
            double scaleX = imageRect.Width / ImagePixelWidth;
            double scaleY = imageRect.Height / ImagePixelHeight;
            foreach (PreviewOverlayRect overlay in overlays)
            {
                System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle
                {
                    Width = Math.Max(1D, overlay.Width * scaleX),
                    Height = Math.Max(1D, overlay.Height * scaleY),
                    Stroke = overlay.IsSelected ? System.Windows.Media.Brushes.Gold : System.Windows.Media.Brushes.DeepSkyBlue,
                    StrokeThickness = overlay.IsSelected ? 2.4D : 1.4D,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    IsHitTestVisible = false
                };
                Canvas.SetLeft(rectangle, imageRect.Left + overlay.Left * scaleX);
                Canvas.SetTop(rectangle, imageRect.Top + overlay.Top * scaleY);
                overlayCanvas.Children.Add(rectangle);
            }
        }

        private Rect GetDisplayedImageRect()
        {
            double availableWidth = ActualWidth;
            double availableHeight = ActualHeight;
            double imageAspect = ImagePixelWidth / (double)ImagePixelHeight;
            double slotAspect = availableWidth / availableHeight;
            if (slotAspect > imageAspect)
            {
                double width = availableHeight * imageAspect;
                return new Rect((availableWidth - width) / 2D, 0D, width, availableHeight);
            }

            double height = availableWidth / imageAspect;
            return new Rect(0D, (availableHeight - height) / 2D, availableWidth, height);
        }

        private static BitmapSource CreateBitmapSource(Bitmap image)
        {
            using MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Bmp);
            stream.Position = 0;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }

        private void ClearPendingImage()
        {
            pendingImage?.Dispose();
            pendingImage = null;
        }

        private sealed class PreviewOverlayRect
        {
            public PreviewOverlayRect(int left, int top, int width, int height, VisionToolPreviewOverlayKind kind, bool isSelected, string groupType)
            {
                Left = left;
                Top = top;
                Width = width;
                Height = height;
                Kind = kind;
                IsSelected = isSelected;
                GroupType = groupType;
            }

            public int Left { get; }

            public int Top { get; }

            public int Width { get; }

            public int Height { get; }

            public VisionToolPreviewOverlayKind Kind { get; }

            public bool IsSelected { get; }

            public string GroupType { get; }
        }
    }
}
