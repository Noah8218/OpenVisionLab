using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Bitmap = System.Drawing.Bitmap;
using CvRect = OpenCvSharp.Rect;
using DrawingRectangle = System.Drawing.Rectangle;
using WpfPoint = System.Windows.Point;
using WpfRect = System.Windows.Rect;
using WpfWindow = System.Windows.Window;

namespace OpenVisionLab
{
    public sealed partial class RoiEditorWindow : WpfWindow, IPropertyGridImageEditView
    {
        private const double HandleSize = 9;
        private const double MinimumRoiSize = 1;
        private const double ViewportPadding = 14;
        private const double MinimumZoom = 0.25;
        private const double MaximumZoom = 16;
        private const double ZoomStep = 1.25;
        private readonly Bitmap sourceBitmap;
        private readonly int sourceWidth;
        private readonly int sourceHeight;
        private readonly RoiEditorViewModel viewModel;
        private CvRect selectedRegionSnapshot = new CvRect();
        private List<CvRect> selectedRegionsSnapshot = new List<CvRect>();
        private DragOperation dragOperation = DragOperation.None;
        private RoiHitHandle dragHandle = RoiHitHandle.None;
        private int activeRegionIndex = -1;
        private WpfPoint dragStartImagePoint;
        private WpfRect dragStartRect = WpfRect.Empty;
        private WpfPoint panStartDisplayPoint;
        private Vector panStartOffset;
        private Vector panOffset;
        private double zoomLevel = 1;
        private bool isPanning;
        private bool hasSelectionSnapshot;
        private bool disposed;

        public RoiEditorWindow(Bitmap image, DrawingRectangle roi, string mode)
        {
            sourceBitmap = WpfBitmapSourceFactory.CloneCompatibleBitmap(image);
            sourceWidth = sourceBitmap.Width;
            sourceHeight = sourceBitmap.Height;
            viewModel = new RoiEditorViewModel(mode, sourceWidth, sourceHeight);

            InitializeComponent();
            DataContext = viewModel;
            viewModel.SourceImage = CreateBitmapSource(sourceBitmap);

            WpfRect initialRect = ToImageRect(roi);
            viewModel.SetSingleRegion(ClampRect(initialRect));
            HookLifecycle();
        }

        public RoiEditorWindow(Bitmap image, List<CvRect> rois, string mode)
        {
            sourceBitmap = WpfBitmapSourceFactory.CloneCompatibleBitmap(image);
            sourceWidth = sourceBitmap.Width;
            sourceHeight = sourceBitmap.Height;
            viewModel = new RoiEditorViewModel(mode, sourceWidth, sourceHeight);

            InitializeComponent();
            DataContext = viewModel;
            viewModel.SourceImage = CreateBitmapSource(sourceBitmap);

            viewModel.SetRegions((rois ?? new List<CvRect>()).Select(ToImageRect).Select(ClampRect));
            HookLifecycle();
        }

        public CvRect SelectedRegion
        {
            get
            {
                EnsureSelectionSnapshot();
                return selectedRegionSnapshot;
            }
        }

        public List<CvRect> SelectedRegions
        {
            get
            {
                EnsureSelectionSnapshot();
                return selectedRegionsSnapshot.ToList();
            }
        }

        internal CvRect CurrentSelectedRegionForTest => ToOpenCvRect(viewModel.SelectedRegion?.ImageRect ?? WpfRect.Empty);

        internal double ZoomLevelForTest => zoomLevel;

        internal bool IsLeftHandleInsideViewportForTest
        {
            get
            {
                WpfRect rect = viewModel.SelectedRegion == null
                    ? WpfRect.Empty
                    : ImageToDisplayRect(viewModel.SelectedRegion.ImageRect);
                return !rect.IsEmpty && rect.Left >= HandleSize / 2.0 && rect.Right <= viewportCanvas.ActualWidth - HandleSize / 2.0;
            }
        }

        internal bool ResizeSelectedLeftEdgeByDisplayPixelsForTest(double displayPixels)
        {
            if (viewModel.SelectedRegion == null || displayPixels <= 0)
            {
                return false;
            }

            WpfRect startRect = viewModel.SelectedRegion.ImageRect;
            WpfRect displayRect = ImageToDisplayRect(startRect);
            WpfPoint handlePoint = new WpfPoint(displayRect.Left, displayRect.Top + displayRect.Height / 2.0);
            if (HitTestHandle(displayRect, handlePoint) != RoiHitHandle.Left)
            {
                return false;
            }

            WpfPoint imagePoint = DisplayToClampedImagePoint(new WpfPoint(handlePoint.X + displayPixels, handlePoint.Y));
            int index = viewModel.IndexOf(viewModel.SelectedRegion);
            viewModel.ReplaceRegion(index, ResizeRect(startRect, RoiHitHandle.Left, imagePoint));
            RenderRegions();
            UpdatePatternPreview();
            return viewModel.SelectedRegion.ImageRect.X > startRect.X
                && viewModel.SelectedRegion.ImageRect.Width < startRect.Width;
        }

        internal void ZoomAndPanForTest(double zoom, double deltaX, double deltaY)
        {
            SetZoomAt(new WpfPoint(viewportCanvas.ActualWidth / 2.0, viewportCanvas.ActualHeight / 2.0), zoom);
            panOffset += new Vector(deltaX, deltaY);
            ClampPanOffset();
            RenderRegions();
        }

        bool IPropertyGridImageEditView.ShowDialog()
        {
            bool? result = base.ShowDialog();
            return result == true;
        }

        public void LoadPatternPreviewImage(string imagePath)
        {
            if (!viewModel.IsTrainingMode) { return; }

            if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    using Bitmap patternBitmap = new Bitmap(imagePath);
                    viewModel.PatternPreviewImage = CreateBitmapSource(patternBitmap);
                    return;
                }
                catch
                {
                    viewModel.PatternPreviewImage = null;
                }
            }

            UpdatePatternPreview();
        }

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;

            Loaded -= OnLoaded;
            Closed -= OnClosed;
            DataContext = null;
            sourceBitmap?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void HookLifecycle()
        {
            Loaded += OnLoaded;
            Closed += OnClosed;
            viewModel.UpdateSelectionSummary();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateViewportLayout();
            RenderRegions();
            UpdatePatternPreview();
            viewportCanvas.Focus();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Dispose();
        }

        private void ViewportCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClampPanOffset();
            UpdateViewportLayout();
            RenderRegions();
        }

        private void ViewportCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            viewportCanvas.Focus();

            if (e.ChangedButton == MouseButton.Middle)
            {
                isPanning = true;
                panStartDisplayPoint = e.GetPosition(viewportCanvas);
                panStartOffset = panOffset;
                viewportCanvas.CaptureMouse();
                Cursor = Cursors.Hand;
                e.Handled = true;
                return;
            }

            if (e.ChangedButton != MouseButton.Left) { return; }

            WpfPoint displayPoint = e.GetPosition(viewportCanvas);
            RoiHit hit = HitTest(displayPoint);
            WpfPoint imagePoint;
            if (hit.Index >= 0)
            {
                imagePoint = DisplayToClampedImagePoint(displayPoint);
            }
            else if (!TryDisplayToImagePoint(displayPoint, out imagePoint))
            {
                return;
            }

            if (hit.Index >= 0)
            {
                SelectRegion(hit.Index);
                activeRegionIndex = hit.Index;
                dragStartRect = viewModel.Regions[hit.Index].ImageRect;
                dragStartImagePoint = imagePoint;
                dragHandle = hit.Handle;
                dragOperation = hit.Handle == RoiHitHandle.Body ? DragOperation.Move : DragOperation.Resize;
            }
            else
            {
                if (!viewModel.IsMultiRoiMode)
                {
                    viewModel.ClearRegions();
                }

                RoiEditorRegionViewModel region = viewModel.AddRegion(new WpfRect(imagePoint, new Size(MinimumRoiSize, MinimumRoiSize)));
                activeRegionIndex = viewModel.IndexOf(region);
                dragStartImagePoint = imagePoint;
                dragStartRect = region.ImageRect;
                dragHandle = RoiHitHandle.BottomRight;
                dragOperation = DragOperation.Create;
            }

            overlayCanvas.CaptureMouse();
            RenderRegions();
            e.Handled = true;
        }

        private void ViewportCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            WpfPoint displayPoint = e.GetPosition(viewportCanvas);
            UpdatePixelStatus(displayPoint);

            if (isPanning && e.MiddleButton == MouseButtonState.Pressed)
            {
                Vector delta = displayPoint - panStartDisplayPoint;
                panOffset = panStartOffset + delta;
                ClampPanOffset();
                RenderRegions();
                e.Handled = true;
                return;
            }

            if (dragOperation == DragOperation.None || activeRegionIndex < 0)
            {
                UpdateCursor(displayPoint);
                return;
            }

            WpfPoint imagePoint = DisplayToClampedImagePoint(displayPoint);

            WpfRect updatedRect = dragOperation switch
            {
                DragOperation.Create => CreateRect(dragStartImagePoint, imagePoint),
                DragOperation.Move => MoveRect(dragStartRect, imagePoint.X - dragStartImagePoint.X, imagePoint.Y - dragStartImagePoint.Y),
                DragOperation.Resize => ResizeRect(dragStartRect, dragHandle, imagePoint),
                _ => dragStartRect
            };

            viewModel.ReplaceRegion(activeRegionIndex, ClampRect(updatedRect));
            viewModel.SelectedRegion = viewModel.Regions[activeRegionIndex];
            RenderRegions();
            UpdatePatternPreview();
            e.Handled = true;
        }

        private void ViewportCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && isPanning)
            {
                isPanning = false;
                viewportCanvas.ReleaseMouseCapture();
                Cursor = Cursors.Arrow;
                e.Handled = true;
                return;
            }

            if (e.ChangedButton != MouseButton.Left) { return; }

            if (dragOperation != DragOperation.None && activeRegionIndex >= 0 && activeRegionIndex < viewModel.Regions.Count)
            {
                WpfRect rect = ClampRect(viewModel.Regions[activeRegionIndex].ImageRect);
                if (rect.Width <= 0 || rect.Height <= 0)
                {
                    viewModel.Regions.RemoveAt(activeRegionIndex);
                }
                else
                {
                    viewModel.ReplaceRegion(activeRegionIndex, rect);
                }
            }

            ResetDragState();
            overlayCanvas.ReleaseMouseCapture();
            RenderRegions();
            UpdatePatternPreview();
            e.Handled = true;
        }

        private void ViewportCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            if (dragOperation == DragOperation.None && !isPanning)
            {
                viewModel.StatusText = "Ready";
                Cursor = Cursors.Arrow;
            }
        }

        private void ViewportCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            SetZoomAt(e.GetPosition(viewportCanvas), zoomLevel * (e.Delta > 0 ? ZoomStep : 1.0 / ZoomStep));
            e.Handled = true;
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            SetZoomAt(GetViewportCenter(), zoomLevel / ZoomStep);
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            SetZoomAt(GetViewportCenter(), zoomLevel * ZoomStep);
        }

        private void FitView_Click(object sender, RoutedEventArgs e)
        {
            zoomLevel = 1;
            panOffset = new Vector();
            UpdateZoomText();
            RenderRegions();
            viewportCanvas.Focus();
        }

        private void RoiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (roiList.SelectedItem is RoiEditorRegionViewModel region)
            {
                viewModel.SelectedRegion = region;
                RenderRegions();
            }
        }

        private void CoordinateField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommitCoordinateFields();
                viewportCanvas.Focus();
                e.Handled = true;
            }
        }

        private void Fit_Click(object sender, RoutedEventArgs e)
        {
            ApplySingleOrAddRegion(new WpfRect(0, 0, sourceBitmap.Width, sourceBitmap.Height));
        }

        private void Center_Click(object sender, RoutedEventArgs e)
        {
            double width = Math.Max(1, sourceBitmap.Width * 0.5);
            double height = Math.Max(1, sourceBitmap.Height * 0.5);

            if (viewModel.SelectedRegion != null && viewModel.SelectedRegion.ImageRect.Width > 0 && viewModel.SelectedRegion.ImageRect.Height > 0)
            {
                width = viewModel.SelectedRegion.ImageRect.Width;
                height = viewModel.SelectedRegion.ImageRect.Height;
            }

            ApplySingleOrAddRegion(new WpfRect(
                Math.Max(0, (sourceBitmap.Width - width) / 2.0),
                Math.Max(0, (sourceBitmap.Height - height) / 2.0),
                Math.Min(sourceBitmap.Width, width),
                Math.Min(sourceBitmap.Height, height)));
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ClearRegions();
            RenderRegions();
            UpdatePatternPreview();
        }

        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveSelectedRegion();
            RenderRegions();
            UpdatePatternPreview();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            AcceptAndClose();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter && !IsTextEditing())
            {
                AcceptAndClose();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Delete)
            {
                viewModel.RemoveSelectedRegion();
                RenderRegions();
                UpdatePatternPreview();
                e.Handled = true;
            }
        }

        private void AcceptAndClose()
        {
            CommitCoordinateFields();
            CaptureSelectionSnapshot();
            DialogResult = true;
            Close();
        }

        private void EnsureSelectionSnapshot()
        {
            if (hasSelectionSnapshot)
            {
                return;
            }

            if (!disposed)
            {
                CommitCoordinateFields();
            }

            CaptureSelectionSnapshot();
        }

        private void CaptureSelectionSnapshot()
        {
            selectedRegionSnapshot = ToOpenCvRect(viewModel.SelectedRegion?.ImageRect ?? WpfRect.Empty);
            selectedRegionsSnapshot = viewModel.Regions
                .Select(region => ToOpenCvRect(region.ImageRect))
                .Where(rect => rect.Width > 0 && rect.Height > 0)
                .ToList();
            hasSelectionSnapshot = true;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ToggleMaximize();
                return;
            }

            DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        private void ToggleMaximize()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            maximizeIcon.Kind = WindowState == WindowState.Maximized
                ? MahApps.Metro.IconPacks.PackIconMaterialKind.WindowRestore
                : MahApps.Metro.IconPacks.PackIconMaterialKind.WindowMaximize;
        }

        private void CommitCoordinateFields()
        {
            if (viewModel.SelectedRegion == null) { return; }
            if (!TryParseInt(txtX.Text, out int x)
                || !TryParseInt(txtY.Text, out int y)
                || !TryParseInt(txtWidth.Text, out int width)
                || !TryParseInt(txtHeight.Text, out int height))
            {
                viewModel.UpdateSelectionSummary();
                return;
            }

            WpfRect rect = ClampRect(new WpfRect(x, y, width, height));
            int index = viewModel.IndexOf(viewModel.SelectedRegion);
            viewModel.ReplaceRegion(index, rect);
            viewModel.UpdateSelectionSummary();
            RenderRegions();
            UpdatePatternPreview();
        }

        private void ApplySingleOrAddRegion(WpfRect rect)
        {
            rect = ClampRect(rect);
            if (viewModel.IsMultiRoiMode)
            {
                if (viewModel.SelectedRegion == null)
                {
                    viewModel.AddRegion(rect);
                }
                else
                {
                    viewModel.ReplaceRegion(viewModel.IndexOf(viewModel.SelectedRegion), rect);
                }
            }
            else
            {
                viewModel.SetSingleRegion(rect);
            }

            RenderRegions();
            UpdatePatternPreview();
        }

        private void SelectRegion(int index)
        {
            if (index < 0 || index >= viewModel.Regions.Count) { return; }
            viewModel.SelectedRegion = viewModel.Regions[index];
            if (viewModel.IsMultiRoiMode)
            {
                roiList.SelectedItem = viewModel.SelectedRegion;
            }
        }

        private void UpdateViewportLayout()
        {
            WpfRect displayRect = GetImageDisplayRect();
            overlayCanvas.Width = Math.Max(1, viewportCanvas.ActualWidth);
            overlayCanvas.Height = Math.Max(1, viewportCanvas.ActualHeight);
            Canvas.SetLeft(sourceImage, displayRect.Left);
            Canvas.SetTop(sourceImage, displayRect.Top);
            sourceImage.Width = displayRect.Width;
            sourceImage.Height = displayRect.Height;
            UpdateZoomText();
        }

        private void RenderRegions()
        {
            overlayCanvas.Children.Clear();
            UpdateViewportLayout();

            for (int i = 0; i < viewModel.Regions.Count; i++)
            {
                RoiEditorRegionViewModel region = viewModel.Regions[i];
                WpfRect displayRect = ImageToDisplayRect(region.ImageRect);
                if (displayRect.Width <= 0 || displayRect.Height <= 0) { continue; }

                bool isSelected = ReferenceEquals(region, viewModel.SelectedRegion);
                System.Windows.Shapes.Rectangle roiShape = new System.Windows.Shapes.Rectangle
                {
                    Width = displayRect.Width,
                    Height = displayRect.Height,
                    Fill = new SolidColorBrush(Color.FromArgb(isSelected ? (byte)44 : (byte)24, 21, 124, 134)),
                    Stroke = new SolidColorBrush(isSelected ? Color.FromRgb(138, 215, 218) : Color.FromRgb(255, 214, 102)),
                    StrokeThickness = isSelected ? 2 : 1.5,
                    IsHitTestVisible = false
                };
                Canvas.SetLeft(roiShape, displayRect.Left);
                Canvas.SetTop(roiShape, displayRect.Top);
                overlayCanvas.Children.Add(roiShape);

                TextBlock label = new TextBlock
                {
                    Text = viewModel.IsMultiRoiMode ? (i + 1).ToString() : "ROI",
                    Foreground = Brushes.White,
                    Background = new SolidColorBrush(Color.FromArgb(185, 18, 26, 30)),
                    Padding = new Thickness(5, 1, 5, 2),
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 11,
                    IsHitTestVisible = false
                };
                Canvas.SetLeft(label, displayRect.Left + 4);
                Canvas.SetTop(label, Math.Max(0, displayRect.Top + 4));
                overlayCanvas.Children.Add(label);

                if (isSelected)
                {
                    AddResizeHandles(displayRect);
                }
            }
        }

        private void AddResizeHandles(WpfRect displayRect)
        {
            foreach (WpfPoint point in GetHandlePoints(displayRect))
            {
                System.Windows.Shapes.Rectangle handle = new System.Windows.Shapes.Rectangle
                {
                    Width = HandleSize,
                    Height = HandleSize,
                    RadiusX = 2,
                    RadiusY = 2,
                    Fill = Brushes.White,
                    Stroke = new SolidColorBrush(Color.FromRgb(21, 124, 134)),
                    StrokeThickness = 1,
                    IsHitTestVisible = false
                };
                Canvas.SetLeft(handle, point.X - HandleSize / 2.0);
                Canvas.SetTop(handle, point.Y - HandleSize / 2.0);
                overlayCanvas.Children.Add(handle);
            }
        }

        private RoiHit HitTest(WpfPoint displayPoint)
        {
            for (int index = viewModel.Regions.Count - 1; index >= 0; index--)
            {
                WpfRect displayRect = ImageToDisplayRect(viewModel.Regions[index].ImageRect);
                if (!Inflate(displayRect, HandleSize).Contains(displayPoint)) { continue; }

                RoiHitHandle handle = HitTestHandle(displayRect, displayPoint);
                if (handle != RoiHitHandle.None)
                {
                    return new RoiHit(index, handle);
                }

                if (displayRect.Contains(displayPoint))
                {
                    return new RoiHit(index, RoiHitHandle.Body);
                }
            }

            return new RoiHit(-1, RoiHitHandle.None);
        }

        private RoiHitHandle HitTestHandle(WpfRect displayRect, WpfPoint point)
        {
            double hit = Math.Max(HandleSize, 8);
            if (Distance(point, new WpfPoint(displayRect.Left, displayRect.Top)) <= hit) { return RoiHitHandle.TopLeft; }
            if (Distance(point, new WpfPoint(displayRect.Right, displayRect.Top)) <= hit) { return RoiHitHandle.TopRight; }
            if (Distance(point, new WpfPoint(displayRect.Left, displayRect.Bottom)) <= hit) { return RoiHitHandle.BottomLeft; }
            if (Distance(point, new WpfPoint(displayRect.Right, displayRect.Bottom)) <= hit) { return RoiHitHandle.BottomRight; }

            bool nearLeft = Math.Abs(point.X - displayRect.Left) <= hit && point.Y >= displayRect.Top && point.Y <= displayRect.Bottom;
            bool nearRight = Math.Abs(point.X - displayRect.Right) <= hit && point.Y >= displayRect.Top && point.Y <= displayRect.Bottom;
            bool nearTop = Math.Abs(point.Y - displayRect.Top) <= hit && point.X >= displayRect.Left && point.X <= displayRect.Right;
            bool nearBottom = Math.Abs(point.Y - displayRect.Bottom) <= hit && point.X >= displayRect.Left && point.X <= displayRect.Right;

            if (nearLeft) { return RoiHitHandle.Left; }
            if (nearRight) { return RoiHitHandle.Right; }
            if (nearTop) { return RoiHitHandle.Top; }
            if (nearBottom) { return RoiHitHandle.Bottom; }
            return RoiHitHandle.None;
        }

        private void UpdateCursor(WpfPoint displayPoint)
        {
            RoiHit hit = HitTest(displayPoint);
            Cursor = hit.Handle switch
            {
                RoiHitHandle.Body => Cursors.SizeAll,
                RoiHitHandle.Left or RoiHitHandle.Right => Cursors.SizeWE,
                RoiHitHandle.Top or RoiHitHandle.Bottom => Cursors.SizeNS,
                RoiHitHandle.TopLeft or RoiHitHandle.BottomRight => Cursors.SizeNWSE,
                RoiHitHandle.TopRight or RoiHitHandle.BottomLeft => Cursors.SizeNESW,
                _ => Cursors.Cross
            };
        }

        private void UpdatePixelStatus(WpfPoint displayPoint)
        {
            if (!TryDisplayToImagePoint(displayPoint, out WpfPoint imagePoint))
            {
                viewModel.StatusText = "Ready";
                return;
            }

            int x = Clamp((int)Math.Round(imagePoint.X), 0, Math.Max(0, sourceBitmap.Width - 1));
            int y = Clamp((int)Math.Round(imagePoint.Y), 0, Math.Max(0, sourceBitmap.Height - 1));
            try
            {
                System.Drawing.Color color = sourceBitmap.GetPixel(x, y);
                int gray = (int)Math.Round((color.R + color.G + color.B) / 3.0);
                viewModel.StatusText = $"X {x}  Y {y}  GV {gray}  RGB {color.R},{color.G},{color.B}";
            }
            catch
            {
                viewModel.StatusText = $"X {x}  Y {y}";
            }
        }

        private void UpdatePatternPreview()
        {
            if (!viewModel.IsTrainingMode || viewModel.SelectedRegion == null)
            {
                return;
            }

                CvRect cvRect = ToOpenCvRect(viewModel.SelectedRegion.ImageRect);
            if (cvRect.Width <= 0 || cvRect.Height <= 0)
            {
                viewModel.PatternPreviewImage = null;
                return;
            }

            try
            {
                DrawingRectangle crop = new DrawingRectangle(cvRect.X, cvRect.Y, cvRect.Width, cvRect.Height);
                using Bitmap cropped = WpfBitmapSourceFactory.CloneRegion(sourceBitmap, crop);
                viewModel.PatternPreviewImage = CreateBitmapSource(cropped);
            }
            catch
            {
                viewModel.PatternPreviewImage = null;
            }
        }

        private WpfRect GetImageDisplayRect()
        {
            double canvasWidth = Math.Max(1, viewportCanvas.ActualWidth);
            double canvasHeight = Math.Max(1, viewportCanvas.ActualHeight);
            double imageWidth = Math.Max(1, sourceBitmap.Width);
            double imageHeight = Math.Max(1, sourceBitmap.Height);
            double availableWidth = Math.Max(1, canvasWidth - ViewportPadding * 2.0);
            double availableHeight = Math.Max(1, canvasHeight - ViewportPadding * 2.0);
            double scale = Math.Min(availableWidth / imageWidth, availableHeight / imageHeight) * zoomLevel;
            double width = imageWidth * scale;
            double height = imageHeight * scale;
            return new WpfRect(
                (canvasWidth - width) / 2.0 + panOffset.X,
                (canvasHeight - height) / 2.0 + panOffset.Y,
                width,
                height);
        }

        private WpfRect ImageToDisplayRect(WpfRect imageRect)
        {
            if (imageRect.IsEmpty) { return WpfRect.Empty; }

            WpfRect display = GetImageDisplayRect();
            double scale = display.Width / Math.Max(1, sourceBitmap.Width);
            return new WpfRect(
                display.Left + imageRect.X * scale,
                display.Top + imageRect.Y * scale,
                imageRect.Width * scale,
                imageRect.Height * scale);
        }

        private bool TryDisplayToImagePoint(WpfPoint displayPoint, out WpfPoint imagePoint)
        {
            WpfRect display = GetImageDisplayRect();
            if (!display.Contains(displayPoint))
            {
                imagePoint = default;
                return false;
            }

            double scale = display.Width / Math.Max(1, sourceBitmap.Width);
            imagePoint = new WpfPoint(
                Clamp((displayPoint.X - display.Left) / scale, 0, Math.Max(0, sourceBitmap.Width)),
                Clamp((displayPoint.Y - display.Top) / scale, 0, Math.Max(0, sourceBitmap.Height)));
            return true;
        }

        private WpfPoint DisplayToClampedImagePoint(WpfPoint displayPoint)
        {
            WpfRect display = GetImageDisplayRect();
            double scale = Math.Max(0.0001, display.Width / Math.Max(1, sourceBitmap.Width));
            return new WpfPoint(
                Clamp((displayPoint.X - display.Left) / scale, 0, Math.Max(0, sourceBitmap.Width)),
                Clamp((displayPoint.Y - display.Top) / scale, 0, Math.Max(0, sourceBitmap.Height)));
        }

        private void SetZoomAt(WpfPoint anchor, double requestedZoom)
        {
            WpfRect before = GetImageDisplayRect();
            double beforeScale = Math.Max(0.0001, before.Width / Math.Max(1, sourceBitmap.Width));
            WpfPoint imagePoint = before.Contains(anchor)
                ? new WpfPoint((anchor.X - before.Left) / beforeScale, (anchor.Y - before.Top) / beforeScale)
                : new WpfPoint(sourceBitmap.Width / 2.0, sourceBitmap.Height / 2.0);

            double nextZoom = Clamp(requestedZoom, MinimumZoom, MaximumZoom);
            if (Math.Abs(nextZoom - zoomLevel) < 0.0001)
            {
                return;
            }

            zoomLevel = nextZoom;
            WpfRect after = GetImageDisplayRect();
            double afterScale = Math.Max(0.0001, after.Width / Math.Max(1, sourceBitmap.Width));
            panOffset += new Vector(
                anchor.X - (after.Left + imagePoint.X * afterScale),
                anchor.Y - (after.Top + imagePoint.Y * afterScale));
            ClampPanOffset();
            UpdateZoomText();
            RenderRegions();
        }

        private void ClampPanOffset()
        {
            double canvasWidth = Math.Max(1, viewportCanvas.ActualWidth);
            double canvasHeight = Math.Max(1, viewportCanvas.ActualHeight);
            double imageWidth = Math.Max(1, sourceBitmap.Width);
            double imageHeight = Math.Max(1, sourceBitmap.Height);
            double fitScale = Math.Min(
                Math.Max(1, canvasWidth - ViewportPadding * 2.0) / imageWidth,
                Math.Max(1, canvasHeight - ViewportPadding * 2.0) / imageHeight);
            double width = imageWidth * fitScale * zoomLevel;
            double height = imageHeight * fitScale * zoomLevel;
            double maxX = Math.Max(0, (width - (canvasWidth - ViewportPadding * 2.0)) / 2.0);
            double maxY = Math.Max(0, (height - (canvasHeight - ViewportPadding * 2.0)) / 2.0);
            panOffset = new Vector(
                Clamp(panOffset.X, -maxX, maxX),
                Clamp(panOffset.Y, -maxY, maxY));
        }

        private WpfPoint GetViewportCenter()
        {
            return new WpfPoint(viewportCanvas.ActualWidth / 2.0, viewportCanvas.ActualHeight / 2.0);
        }

        private void UpdateZoomText()
        {
            if (zoomValueText != null)
            {
                zoomValueText.Text = $"{Math.Round(zoomLevel * 100):0}%";
            }
        }

        private WpfRect CreateRect(WpfPoint start, WpfPoint end)
        {
            return ClampRect(new WpfRect(
                Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y),
                Math.Abs(end.X - start.X),
                Math.Abs(end.Y - start.Y)));
        }

        private WpfRect MoveRect(WpfRect rect, double deltaX, double deltaY)
        {
            double width = Math.Min(rect.Width, sourceBitmap.Width);
            double height = Math.Min(rect.Height, sourceBitmap.Height);
            double x = Clamp(rect.X + deltaX, 0, Math.Max(0, sourceBitmap.Width - width));
            double y = Clamp(rect.Y + deltaY, 0, Math.Max(0, sourceBitmap.Height - height));
            return new WpfRect(x, y, width, height);
        }

        private WpfRect ResizeRect(WpfRect rect, RoiHitHandle handle, WpfPoint point)
        {
            double left = rect.Left;
            double top = rect.Top;
            double right = rect.Right;
            double bottom = rect.Bottom;

            switch (handle)
            {
                case RoiHitHandle.Left:
                case RoiHitHandle.TopLeft:
                case RoiHitHandle.BottomLeft:
                    left = point.X;
                    break;
                case RoiHitHandle.Right:
                case RoiHitHandle.TopRight:
                case RoiHitHandle.BottomRight:
                    right = point.X;
                    break;
            }

            switch (handle)
            {
                case RoiHitHandle.Top:
                case RoiHitHandle.TopLeft:
                case RoiHitHandle.TopRight:
                    top = point.Y;
                    break;
                case RoiHitHandle.Bottom:
                case RoiHitHandle.BottomLeft:
                case RoiHitHandle.BottomRight:
                    bottom = point.Y;
                    break;
            }

            return ClampRect(new WpfRect(
                Math.Min(left, right),
                Math.Min(top, bottom),
                Math.Abs(right - left),
                Math.Abs(bottom - top)));
        }

        private WpfRect ClampRect(WpfRect rect)
        {
            if (rect.IsEmpty) { return WpfRect.Empty; }

            double width = Clamp(Math.Max(MinimumRoiSize, rect.Width), MinimumRoiSize, Math.Max(MinimumRoiSize, sourceWidth));
            double height = Clamp(Math.Max(MinimumRoiSize, rect.Height), MinimumRoiSize, Math.Max(MinimumRoiSize, sourceHeight));
            double x = Clamp(rect.X, 0, Math.Max(0, sourceWidth - width));
            double y = Clamp(rect.Y, 0, Math.Max(0, sourceHeight - height));
            return new WpfRect(x, y, width, height);
        }

        private CvRect ToOpenCvRect(WpfRect rect)
        {
            if (rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0) { return new CvRect(); }

            int x = Clamp((int)Math.Round(rect.X), 0, sourceWidth);
            int y = Clamp((int)Math.Round(rect.Y), 0, sourceHeight);
            int right = Clamp((int)Math.Round(rect.Right), 0, sourceWidth);
            int bottom = Clamp((int)Math.Round(rect.Bottom), 0, sourceHeight);
            int width = Math.Max(0, right - x);
            int height = Math.Max(0, bottom - y);
            return width > 0 && height > 0 ? new CvRect(x, y, width, height) : new CvRect();
        }

        private WpfRect ToImageRect(CvRect rect)
        {
            return rect.Width <= 0 || rect.Height <= 0
                ? WpfRect.Empty
                : new WpfRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        private WpfRect ToImageRect(DrawingRectangle rect)
        {
            return rect.Width <= 0 || rect.Height <= 0
                ? WpfRect.Empty
                : new WpfRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        private static BitmapSource CreateBitmapSource(Bitmap bitmap)
        {
            return WpfBitmapSourceFactory.Create(bitmap);
        }

        private static WpfPoint[] GetHandlePoints(WpfRect rect)
        {
            return new[]
            {
                new WpfPoint(rect.Left, rect.Top),
                new WpfPoint(rect.Left + rect.Width / 2.0, rect.Top),
                new WpfPoint(rect.Right, rect.Top),
                new WpfPoint(rect.Right, rect.Top + rect.Height / 2.0),
                new WpfPoint(rect.Right, rect.Bottom),
                new WpfPoint(rect.Left + rect.Width / 2.0, rect.Bottom),
                new WpfPoint(rect.Left, rect.Bottom),
                new WpfPoint(rect.Left, rect.Top + rect.Height / 2.0)
            };
        }

        private static WpfRect Inflate(WpfRect rect, double value)
        {
            rect.Inflate(value, value);
            return rect;
        }

        private static double Distance(WpfPoint a, WpfPoint b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min) { return min; }
            if (value > max) { return max; }
            return value;
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min) { return min; }
            if (value > max) { return max; }
            return value;
        }

        private static bool TryParseInt(string text, out int value)
        {
            return int.TryParse((text ?? string.Empty).Trim(), out value);
        }

        private static bool IsTextEditing()
        {
            return Keyboard.FocusedElement is TextBox;
        }

        private void ResetDragState()
        {
            dragOperation = DragOperation.None;
            dragHandle = RoiHitHandle.None;
            activeRegionIndex = -1;
            dragStartRect = WpfRect.Empty;
            Cursor = Cursors.Arrow;
        }

        private enum DragOperation
        {
            None,
            Create,
            Move,
            Resize
        }

        private enum RoiHitHandle
        {
            None,
            Body,
            Left,
            Right,
            Top,
            Bottom,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        private readonly struct RoiHit
        {
            public RoiHit(int index, RoiHitHandle handle)
            {
                Index = index;
                Handle = handle;
            }

            public int Index { get; }
            public RoiHitHandle Handle { get; }
        }
    }
}
