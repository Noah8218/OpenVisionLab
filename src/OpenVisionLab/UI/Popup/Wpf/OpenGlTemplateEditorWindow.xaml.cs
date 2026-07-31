using MahApps.Metro.IconPacks;
using Lib.Common;
using OpenCvSharp;
using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Model;
using OpenVisionLab.ImageCanvas.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CvRect = OpenCvSharp.Rect;
using DrawingPixelFormat = System.Drawing.Imaging.PixelFormat;
using DrawingRectangle = System.Drawing.Rectangle;
using WpfPoint = System.Windows.Point;
using WpfRect = System.Windows.Rect;
using WpfWindow = System.Windows.Window;

namespace OpenVisionLab
{
    public sealed partial class OpenGlTemplateEditorWindow : WpfWindow, IPropertyGridTemplateImageEditView
    {
        private const double RoiHandleScreenSize = 10D;
        private const double RoiHandleHitSize = 8D;
        private const double RoiRotationHandleScreenDistance = 28D;
        private const double RoiRotationHandleScreenSize = 16D;
        private const double MinimumRoiSize = 1D;
        private readonly Bitmap sourceBitmap;
        private readonly DrawingRectangle initialRoi;
        private readonly RoiImageCanvasViewModel canvasViewModel;
        private readonly ScaleTransform wpfImageScale = new ScaleTransform(1D, 1D);
        private readonly TranslateTransform wpfImageTranslate = new TranslateTransform();
        private CvRect selectedRegion;
        private TemplateRoiDragOperation roiDragOperation = TemplateRoiDragOperation.None;
        private TemplateRoiHitHandle roiDragHandle = TemplateRoiHitHandle.None;
        private WpfPoint roiDragStartImagePoint;
        private WpfRect roiDragStartRect = WpfRect.Empty;
        private double roiDragStartRotationDegrees;
        private double roiDragStartPointerAngleDegrees;
        private WpfPoint panStartView;
        private Vector panStartOffset;
        private bool isPanning;
        private bool wpfViewerFitted;
        private bool viewerInitialized;
        private bool isUpdatingRotationText;
        private double templateRotationDegrees;
        private bool disposed;

        public OpenGlTemplateEditorWindow(Bitmap image, DrawingRectangle roi, string mode)
        {
            sourceBitmap = CloneSourceBitmap(image);
            initialRoi = ClampToImage(roi);
            selectedRegion = ToOpenCvRect(initialRoi);

            canvasViewModel = new RoiImageCanvasViewModel($"TemplateRegistration_{Guid.NewGuid():N}")
            {
                ReplaceExistingRoiOnDraw = true,
                ShowGroupNames = false,
                ShowRoiItemNames = false,
                UseGroupMoveMode = false,
                ShowGroupBounds = false
            };
            canvasViewModel.RoiAdded += CanvasViewModel_RoiChanged;
            canvasViewModel.RoiEditingCompleted += CanvasViewModel_RoiChanged;

            InitializeComponent();
            glCanvas.DataContext = canvasViewModel;
            InitializeWpfViewer();
            UpdateSelectionText();

            Loaded += OpenGlTemplateEditorWindow_Loaded;
            Closed += OpenGlTemplateEditorWindow_Closed;
        }

        public CvRect SelectedRegion => selectedRegion;

        public List<CvRect> SelectedRegions
        {
            get
            {
                return HasValidRegion(selectedRegion)
                    ? new List<CvRect> { selectedRegion }
                    : new List<CvRect>();
            }
        }

        public double TemplateRotationDegrees
        {
            get => templateRotationDegrees;
            set => SetTemplateRotationDegrees(value);
        }

        internal int RoiHandleVisualCountForTest => wpfRoiHandleLayer?.Children.Count ?? 0;

        internal double TemplateRotationDegreesForTest => templateRotationDegrees;

        internal bool SetTemplateRotationDegreesForTest(double rotationDegrees)
        {
            SetTemplateRotationDegrees(rotationDegrees);
            return Math.Abs(templateRotationDegrees - TemplateImageExtraction.NormalizeRotationDegrees(rotationDegrees)) < 0.001D;
        }

        internal bool MoveSelectedRegionForTest(int deltaX, int deltaY)
        {
            WpfRect movedRect = MoveRoiRect(ToWpfRect(selectedRegion), deltaX, deltaY);
            return ApplySelectedRegionForTest(movedRect);
        }

        internal bool ResizeSelectedRegionForTest(int deltaRight, int deltaBottom)
        {
            WpfRect rect = ToWpfRect(selectedRegion);
            if (rect.IsEmpty)
            {
                return false;
            }

            WpfPoint bottomRight = new WpfPoint(rect.Right + deltaRight, rect.Bottom + deltaBottom);
            return ApplySelectedRegionForTest(ResizeRoiRect(rect, TemplateRoiHitHandle.BottomRight, bottomRight));
        }

        bool IPropertyGridImageEditView.ShowDialog()
        {
            bool? result = base.ShowDialog();
            return result == true;
        }

        public void LoadPatternPreviewImage(string imagePath)
        {
            if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    using Bitmap patternBitmap = new Bitmap(imagePath);
                    SetPatternPreview(CreateBitmapSource(patternBitmap));
                    return;
                }
                catch
                {
                    SetPatternPreview(null);
                }
            }

            UpdatePatternPreview();
        }

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;

            Loaded -= OpenGlTemplateEditorWindow_Loaded;
            Closed -= OpenGlTemplateEditorWindow_Closed;
            canvasViewModel.RoiAdded -= CanvasViewModel_RoiChanged;
            canvasViewModel.RoiEditingCompleted -= CanvasViewModel_RoiChanged;
            glCanvas.DataContext = null;
            canvasViewModel.Dispose();
            sourceBitmap?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OpenGlTemplateEditorWindow_Closed(object sender, EventArgs e)
        {
            Dispose();
        }

        private void OpenGlTemplateEditorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ScheduleInitializeViewer();
        }

        private void ScheduleInitializeViewer()
        {
            Dispatcher.BeginInvoke(new Action(InitializeViewer), DispatcherPriority.Loaded);
        }

        private void InitializeViewer()
        {
            if (viewerInitialized) { return; }
            viewerInitialized = true;

            try
            {
                canvasViewModel.LoadImage(sourceBitmap, "Template registration");
                canvasViewModel.FitImageToView();
                FitWpfViewer();
                ApplySingleRoi(initialRoi);
                canvasViewModel.IsTeachingMode = true;
                UpdatePatternPreview();
                ScheduleDeferredCanvasRefresh();
                glCanvas.Focus();
                SetStatus("Ready");
            }
            catch (Exception ex)
            {
                SetStatus($"Viewer initialization failed: {ex.Message}");
            }
        }

        private void ScheduleDeferredCanvasRefresh()
        {
            ScheduleDeferredCanvasRefresh(120);
            ScheduleDeferredCanvasRefresh(420);
        }

        private void ScheduleDeferredCanvasRefresh(int delayMilliseconds)
        {
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background, Dispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(delayMilliseconds)
            };
            timer.Tick += (sender, _) =>
            {
                timer.Stop();
                RefreshOpenGlCanvas();
            };
            timer.Start();
        }

        private void RefreshOpenGlCanvas()
        {
            if (disposed || !viewerInitialized)
            {
                return;
            }

            try
            {
                glCanvas.UpdateLayout();
                canvasViewModel.LoadImage(sourceBitmap, "Template registration");
                canvasViewModel.FitImageToView();
                canvasViewModel.ImageViewer.Reshape();
                canvasViewModel.ImageViewer.RefreshGL();
            }
            catch
            {
            }
        }

        private void CanvasViewModel_RoiChanged(object sender, RoiChangedEventArgs e)
        {
            CvRect rect = ToOpenCvRect(e?.RoiRect);
            if (!HasValidRegion(rect))
            {
                selectedRegion = new CvRect();
                UpdateSelectionText();
                return;
            }

            selectedRegion = rect;
            UpdateSelectionText();
            UpdatePatternPreview();
        }

        private void FitView_Click(object sender, RoutedEventArgs e)
        {
            canvasViewModel.FitImageToView();
            FitWpfViewer();
        }

        private void CenterRoi_Click(object sender, RoutedEventArgs e)
        {
            ApplySingleRoi(CreateCenteredRoi());
        }

        private void FullRoi_Click(object sender, RoutedEventArgs e)
        {
            ApplySingleRoi(new DrawingRectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height));
        }

        private void RotateLeft_Click(object sender, RoutedEventArgs e)
        {
            SetTemplateRotationDegrees(templateRotationDegrees - 90D);
        }

        private void RotateRight_Click(object sender, RoutedEventArgs e)
        {
            SetTemplateRotationDegrees(templateRotationDegrees + 90D);
        }

        private void ResetRotation_Click(object sender, RoutedEventArgs e)
        {
            SetTemplateRotationDegrees(0D);
        }

        private void RotationText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingRotationText)
            {
                return;
            }

            if (double.TryParse(txtRotation.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedDegrees)
                || double.TryParse(txtRotation.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out parsedDegrees))
            {
                SetTemplateRotationDegrees(parsedDegrees, updateText: false);
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            AcceptSelection();
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

            if (e.Key == Key.Enter)
            {
                AcceptSelection();
                e.Handled = true;
            }
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
                ? PackIconMaterialKind.WindowRestore
                : PackIconMaterialKind.WindowMaximize;
        }

        private void AcceptSelection()
        {
            if (!HasValidRegion(selectedRegion))
            {
                SetStatus("Select a template area.");
                return;
            }

            DialogResult = true;
            Close();
        }

        private void ApplySingleRoi(DrawingRectangle roi)
        {
            DrawingRectangle clamped = ClampToImage(roi);
            canvasViewModel.RestoreWindowRoiSnapshot(Array.Empty<RoiSnapshotItem>());
            selectedRegion = ToOpenCvRect(clamped);
            canvasViewModel.AddInitialRoi(clamped);
            canvasViewModel.IsTeachingMode = true;
            UpdateWpfRoiRectangle();
            UpdateSelectionText();
            UpdatePatternPreview();
        }

        private void InitializeWpfViewer()
        {
            TransformGroup transform = new TransformGroup();
            transform.Children.Add(wpfImageScale);
            transform.Children.Add(wpfImageTranslate);
            wpfImageLayer.RenderTransform = transform;
            wpfImageLayer.Width = sourceBitmap.Width;
            wpfImageLayer.Height = sourceBitmap.Height;
            wpfSourceImage.Width = sourceBitmap.Width;
            wpfSourceImage.Height = sourceBitmap.Height;
            wpfSourceImage.Source = CreateBitmapSource(sourceBitmap);
            UpdateWpfRoiRectangle();
        }

        private void ViewerHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!wpfViewerFitted)
            {
                FitWpfViewer();
            }
        }

        private void FitWpfViewer()
        {
            if (viewerHost == null || sourceBitmap.Width <= 0 || sourceBitmap.Height <= 0)
            {
                return;
            }

            double width = Math.Max(1D, viewerHost.ActualWidth);
            double height = Math.Max(1D, viewerHost.ActualHeight);
            double scale = Math.Min(width / sourceBitmap.Width, height / sourceBitmap.Height);
            scale = Math.Max(0.01D, scale * 0.96D);

            wpfImageScale.ScaleX = scale;
            wpfImageScale.ScaleY = scale;
            wpfImageTranslate.X = (width - sourceBitmap.Width * scale) / 2D;
            wpfImageTranslate.Y = (height - sourceBitmap.Height * scale) / 2D;
            wpfViewerFitted = true;
            UpdateWpfRoiRectangle();
        }

        private void ViewerHost_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            WpfPoint viewPoint = e.GetPosition(viewerHost);
            WpfPoint imagePoint = ViewToImage(viewPoint);
            double factor = e.Delta > 0 ? 1.15D : 1D / 1.15D;
            double nextScale = Math.Max(0.05D, Math.Min(20D, wpfImageScale.ScaleX * factor));

            wpfImageScale.ScaleX = nextScale;
            wpfImageScale.ScaleY = nextScale;
            wpfImageTranslate.X = viewPoint.X - imagePoint.X * nextScale;
            wpfImageTranslate.Y = viewPoint.Y - imagePoint.Y * nextScale;
            UpdateWpfRoiRectangle();
            e.Handled = true;
        }

        private void ViewerHost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewerHost.Focus();
            WpfPoint viewPoint = e.GetPosition(viewerHost);
            WpfPoint imagePoint = ClampImagePoint(ViewToImage(viewPoint));
            TemplateRoiHitHandle hitHandle = HitTestRoi(viewPoint);
            roiDragStartImagePoint = imagePoint;
            roiDragStartRect = ToWpfRect(selectedRegion);
            roiDragHandle = hitHandle == TemplateRoiHitHandle.None ? TemplateRoiHitHandle.BottomRight : hitHandle;
            roiDragStartRotationDegrees = templateRotationDegrees;
            roiDragStartPointerAngleDegrees = GetPointerAngleDegrees(GetRoiCenter(roiDragStartRect), imagePoint);
            roiDragOperation = hitHandle switch
            {
                TemplateRoiHitHandle.Rotation => TemplateRoiDragOperation.Rotate,
                TemplateRoiHitHandle.Body => TemplateRoiDragOperation.Move,
                TemplateRoiHitHandle.None => TemplateRoiDragOperation.Create,
                _ => TemplateRoiDragOperation.Resize
            };
            if (roiDragOperation == TemplateRoiDragOperation.Create)
            {
                SetTemplateRotationDegrees(0D);
            }

            viewerHost.CaptureMouse();
            e.Handled = true;
        }

        private void ViewerHost_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (roiDragOperation != TemplateRoiDragOperation.None)
            {
                UpdateRoiFromDrag(ClampImagePoint(ViewToImage(e.GetPosition(viewerHost))));
                ResetRoiDragState();
                viewerHost.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        private void ViewerHost_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle)
            {
                return;
            }

            isPanning = true;
            panStartView = e.GetPosition(viewerHost);
            panStartOffset = new Vector(wpfImageTranslate.X, wpfImageTranslate.Y);
            viewerHost.CaptureMouse();
            e.Handled = true;
        }

        private void ViewerHost_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle || !isPanning)
            {
                return;
            }

            isPanning = false;
            viewerHost.ReleaseMouseCapture();
            e.Handled = true;
        }

        private void ViewerHost_MouseMove(object sender, MouseEventArgs e)
        {
            WpfPoint viewPoint = e.GetPosition(viewerHost);
            if (roiDragOperation != TemplateRoiDragOperation.None && e.LeftButton == MouseButtonState.Pressed)
            {
                UpdateRoiFromDrag(ClampImagePoint(ViewToImage(viewPoint)));
                e.Handled = true;
                return;
            }

            if (roiDragOperation == TemplateRoiDragOperation.None)
            {
                UpdateRoiCursor(viewPoint);
            }

            if (isPanning && e.MiddleButton == MouseButtonState.Pressed)
            {
                Vector delta = viewPoint - panStartView;
                wpfImageTranslate.X = panStartOffset.X + delta.X;
                wpfImageTranslate.Y = panStartOffset.Y + delta.Y;
                UpdateWpfRoiRectangle();
                e.Handled = true;
            }
        }

        private WpfPoint ViewToImage(WpfPoint viewPoint)
        {
            double scale = Math.Max(0.0001D, wpfImageScale.ScaleX);
            return new WpfPoint(
                (viewPoint.X - wpfImageTranslate.X) / scale,
                (viewPoint.Y - wpfImageTranslate.Y) / scale);
        }

        private WpfPoint ClampImagePoint(WpfPoint point)
        {
            return new WpfPoint(
                Math.Max(0D, Math.Min(sourceBitmap.Width, point.X)),
                Math.Max(0D, Math.Min(sourceBitmap.Height, point.Y)));
        }

        private void UpdateRoiFromDrag(WpfPoint currentImagePoint)
        {
            WpfRect updatedRect = roiDragOperation switch
            {
                TemplateRoiDragOperation.Create => CreateRoiRect(roiDragStartImagePoint, currentImagePoint),
                TemplateRoiDragOperation.Move => MoveRoiRect(roiDragStartRect, currentImagePoint.X - roiDragStartImagePoint.X, currentImagePoint.Y - roiDragStartImagePoint.Y),
                TemplateRoiDragOperation.Resize => ResizeRoiRect(roiDragStartRect, roiDragHandle, ImageToLocalRoiPoint(currentImagePoint, roiDragStartRect, roiDragStartRotationDegrees)),
                TemplateRoiDragOperation.Rotate => RotateRoiFromDrag(currentImagePoint),
                _ => roiDragStartRect
            };

            CvRect updatedRegion = ToOpenCvRect(ClampRoiRect(updatedRect));
            if (!HasValidRegion(updatedRegion))
            {
                return;
            }

            selectedRegion = updatedRegion;
            UpdateWpfRoiRectangle();
            UpdateSelectionText();
            UpdatePatternPreview();
        }

        private bool ApplySelectedRegionForTest(WpfRect rect)
        {
            CvRect updatedRegion = ToOpenCvRect(ClampRoiRect(rect));
            if (!HasValidRegion(updatedRegion))
            {
                return false;
            }

            selectedRegion = updatedRegion;
            UpdateWpfRoiRectangle();
            UpdateSelectionText();
            UpdatePatternPreview();
            return true;
        }

        private void UpdateWpfRoiRectangle()
        {
            if (wpfRoiRectangle == null)
            {
                return;
            }

            if (!HasValidRegion(selectedRegion))
            {
                wpfRoiRectangle.Visibility = Visibility.Collapsed;
                wpfRoiHandleLayer?.Children.Clear();
                return;
            }

            wpfRoiRectangle.Visibility = Visibility.Visible;
            Canvas.SetLeft(wpfRoiRectangle, selectedRegion.X);
            Canvas.SetTop(wpfRoiRectangle, selectedRegion.Y);
            wpfRoiRectangle.Width = selectedRegion.Width;
            wpfRoiRectangle.Height = selectedRegion.Height;
            wpfRoiRectangle.RenderTransformOrigin = new WpfPoint(0.5D, 0.5D);
            wpfRoiRectangle.RenderTransform = new RotateTransform(templateRotationDegrees);
            UpdateWpfRoiHandles();
        }

        private void UpdateWpfRoiHandles()
        {
            if (wpfRoiHandleLayer == null)
            {
                return;
            }

            wpfRoiHandleLayer.Children.Clear();
            if (!HasValidRegion(selectedRegion))
            {
                return;
            }

            double scale = Math.Max(0.0001D, wpfImageScale.ScaleX);
            double size = RoiHandleScreenSize / scale;
            WpfRect rect = ToWpfRect(selectedRegion);
            foreach (WpfPoint point in GetHandlePoints(rect, templateRotationDegrees))
            {
                System.Windows.Shapes.Rectangle handle = new System.Windows.Shapes.Rectangle
                {
                    Width = size,
                    Height = size,
                    RadiusX = Math.Min(2D, size / 3D),
                    RadiusY = Math.Min(2D, size / 3D),
                    Fill = System.Windows.Media.Brushes.White,
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(21, 124, 134)),
                    StrokeThickness = Math.Max(1D / scale, 0.5D),
                    IsHitTestVisible = false
                };
                Canvas.SetLeft(handle, point.X - size / 2D);
                Canvas.SetTop(handle, point.Y - size / 2D);
                wpfRoiHandleLayer.Children.Add(handle);
            }

            WpfPoint topCenter = RotatePoint(new WpfPoint(rect.Left + rect.Width / 2D, rect.Top), GetRoiCenter(rect), templateRotationDegrees);
            WpfPoint rotationHandlePoint = GetRotationHandlePoint(rect, scale);
            System.Windows.Shapes.Line rotationLine = new System.Windows.Shapes.Line
            {
                X1 = topCenter.X,
                Y1 = topCenter.Y,
                X2 = rotationHandlePoint.X,
                Y2 = rotationHandlePoint.Y,
                Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(21, 124, 134)),
                StrokeThickness = Math.Max(1D / scale, 0.5D),
                IsHitTestVisible = false
            };
            wpfRoiHandleLayer.Children.Add(rotationLine);

            double rotationHandleSize = RoiRotationHandleScreenSize / scale;
            System.Windows.Shapes.Ellipse rotationHandle = new System.Windows.Shapes.Ellipse
            {
                Width = rotationHandleSize,
                Height = rotationHandleSize,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 250, 230)),
                Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(21, 124, 134)),
                StrokeThickness = Math.Max(1.2D / scale, 0.6D),
                IsHitTestVisible = false
            };
            Canvas.SetLeft(rotationHandle, rotationHandlePoint.X - rotationHandleSize / 2D);
            Canvas.SetTop(rotationHandle, rotationHandlePoint.Y - rotationHandleSize / 2D);
            wpfRoiHandleLayer.Children.Add(rotationHandle);
        }

        private TemplateRoiHitHandle HitTestRoi(WpfPoint viewPoint)
        {
            if (!HasValidRegion(selectedRegion))
            {
                return TemplateRoiHitHandle.None;
            }

            WpfRect rect = ToWpfRect(selectedRegion);
            double scale = Math.Max(0.0001D, wpfImageScale.ScaleX);
            WpfPoint imagePoint = ViewToImage(viewPoint);
            double hitImage = RoiHandleHitSize / scale;
            TemplateRoiHitHandle handle = HitTestRoiHandle(rect, imagePoint, hitImage);
            if (handle != TemplateRoiHitHandle.None)
            {
                return handle;
            }

            WpfPoint localPoint = ImageToLocalRoiPoint(imagePoint, rect, templateRotationDegrees);
            WpfRect inflated = Inflate(rect, hitImage);
            if (!inflated.Contains(localPoint))
            {
                return TemplateRoiHitHandle.None;
            }

            return rect.Contains(localPoint) ? TemplateRoiHitHandle.Body : TemplateRoiHitHandle.None;
        }

        private TemplateRoiHitHandle HitTestRoiHandle(WpfRect rect, WpfPoint imagePoint, double hit)
        {
            WpfPoint rotationHandlePoint = GetRotationHandlePoint(rect, Math.Max(0.0001D, wpfImageScale.ScaleX));
            if (Distance(imagePoint, rotationHandlePoint) <= Math.Max(hit, RoiRotationHandleScreenSize / Math.Max(0.0001D, wpfImageScale.ScaleX)))
            {
                return TemplateRoiHitHandle.Rotation;
            }

            WpfPoint[] points = GetHandlePoints(rect, templateRotationDegrees);
            if (Distance(imagePoint, points[0]) <= hit) { return TemplateRoiHitHandle.TopLeft; }
            if (Distance(imagePoint, points[2]) <= hit) { return TemplateRoiHitHandle.TopRight; }
            if (Distance(imagePoint, points[6]) <= hit) { return TemplateRoiHitHandle.BottomLeft; }
            if (Distance(imagePoint, points[4]) <= hit) { return TemplateRoiHitHandle.BottomRight; }

            WpfPoint localPoint = ImageToLocalRoiPoint(imagePoint, rect, templateRotationDegrees);
            bool nearLeft = Math.Abs(localPoint.X - rect.Left) <= hit && localPoint.Y >= rect.Top && localPoint.Y <= rect.Bottom;
            bool nearRight = Math.Abs(localPoint.X - rect.Right) <= hit && localPoint.Y >= rect.Top && localPoint.Y <= rect.Bottom;
            bool nearTop = Math.Abs(localPoint.Y - rect.Top) <= hit && localPoint.X >= rect.Left && localPoint.X <= rect.Right;
            bool nearBottom = Math.Abs(localPoint.Y - rect.Bottom) <= hit && localPoint.X >= rect.Left && localPoint.X <= rect.Right;

            if (nearLeft) { return TemplateRoiHitHandle.Left; }
            if (nearRight) { return TemplateRoiHitHandle.Right; }
            if (nearTop) { return TemplateRoiHitHandle.Top; }
            if (nearBottom) { return TemplateRoiHitHandle.Bottom; }
            return TemplateRoiHitHandle.None;
        }

        private void UpdateRoiCursor(WpfPoint viewPoint)
        {
            TemplateRoiHitHandle handle = HitTestRoi(viewPoint);
            viewerHost.Cursor = handle switch
            {
                TemplateRoiHitHandle.Rotation => Cursors.Hand,
                TemplateRoiHitHandle.Body => Cursors.SizeAll,
                TemplateRoiHitHandle.Left or TemplateRoiHitHandle.Right => Cursors.SizeWE,
                TemplateRoiHitHandle.Top or TemplateRoiHitHandle.Bottom => Cursors.SizeNS,
                TemplateRoiHitHandle.TopLeft or TemplateRoiHitHandle.BottomRight => Cursors.SizeNWSE,
                TemplateRoiHitHandle.TopRight or TemplateRoiHitHandle.BottomLeft => Cursors.SizeNESW,
                _ => Cursors.Cross
            };
        }

        private WpfRect ImageToViewRect(CvRect rect)
        {
            double scale = Math.Max(0.0001D, wpfImageScale.ScaleX);
            return new WpfRect(
                rect.X * scale + wpfImageTranslate.X,
                rect.Y * scale + wpfImageTranslate.Y,
                rect.Width * scale,
                rect.Height * scale);
        }

        private WpfRect CreateRoiRect(WpfPoint start, WpfPoint end)
        {
            return ClampRoiRect(new WpfRect(
                Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y),
                Math.Abs(end.X - start.X),
                Math.Abs(end.Y - start.Y)));
        }

        private WpfRect MoveRoiRect(WpfRect rect, double deltaX, double deltaY)
        {
            if (rect.IsEmpty)
            {
                return WpfRect.Empty;
            }

            double width = Math.Min(rect.Width, sourceBitmap.Width);
            double height = Math.Min(rect.Height, sourceBitmap.Height);
            double x = Clamp(rect.X + deltaX, 0D, Math.Max(0D, sourceBitmap.Width - width));
            double y = Clamp(rect.Y + deltaY, 0D, Math.Max(0D, sourceBitmap.Height - height));
            return new WpfRect(x, y, width, height);
        }

        private WpfRect ResizeRoiRect(WpfRect rect, TemplateRoiHitHandle handle, WpfPoint point)
        {
            if (rect.IsEmpty)
            {
                return WpfRect.Empty;
            }

            double left = rect.Left;
            double top = rect.Top;
            double right = rect.Right;
            double bottom = rect.Bottom;

            switch (handle)
            {
                case TemplateRoiHitHandle.Left:
                case TemplateRoiHitHandle.TopLeft:
                case TemplateRoiHitHandle.BottomLeft:
                    left = point.X;
                    break;
                case TemplateRoiHitHandle.Right:
                case TemplateRoiHitHandle.TopRight:
                case TemplateRoiHitHandle.BottomRight:
                    right = point.X;
                    break;
            }

            switch (handle)
            {
                case TemplateRoiHitHandle.Top:
                case TemplateRoiHitHandle.TopLeft:
                case TemplateRoiHitHandle.TopRight:
                    top = point.Y;
                    break;
                case TemplateRoiHitHandle.Bottom:
                case TemplateRoiHitHandle.BottomLeft:
                case TemplateRoiHitHandle.BottomRight:
                    bottom = point.Y;
                    break;
            }

            return ClampRoiRect(new WpfRect(
                Math.Min(left, right),
                Math.Min(top, bottom),
                Math.Abs(right - left),
                Math.Abs(bottom - top)));
        }

        private WpfRect ClampRoiRect(WpfRect rect)
        {
            if (rect.IsEmpty)
            {
                return WpfRect.Empty;
            }

            double width = Clamp(Math.Max(MinimumRoiSize, rect.Width), MinimumRoiSize, Math.Max(MinimumRoiSize, sourceBitmap.Width));
            double height = Clamp(Math.Max(MinimumRoiSize, rect.Height), MinimumRoiSize, Math.Max(MinimumRoiSize, sourceBitmap.Height));
            double x = Clamp(rect.X, 0D, Math.Max(0D, sourceBitmap.Width - width));
            double y = Clamp(rect.Y, 0D, Math.Max(0D, sourceBitmap.Height - height));
            return new WpfRect(x, y, width, height);
        }

        private CvRect ToOpenCvRect(WpfRect rect)
        {
            if (rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0)
            {
                return new CvRect();
            }

            int x = Clamp((int)Math.Round(rect.X), 0, sourceBitmap.Width);
            int y = Clamp((int)Math.Round(rect.Y), 0, sourceBitmap.Height);
            int right = Clamp((int)Math.Round(rect.Right), 0, sourceBitmap.Width);
            int bottom = Clamp((int)Math.Round(rect.Bottom), 0, sourceBitmap.Height);
            int width = Math.Max(0, right - x);
            int height = Math.Max(0, bottom - y);
            return width > 0 && height > 0 ? new CvRect(x, y, width, height) : new CvRect();
        }

        private static WpfRect ToWpfRect(CvRect rect)
        {
            return rect.Width <= 0 || rect.Height <= 0
                ? WpfRect.Empty
                : new WpfRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        private static WpfPoint[] GetHandlePoints(WpfRect rect, double rotationDegrees)
        {
            WpfPoint center = GetRoiCenter(rect);
            WpfPoint[] points =
            {
                new WpfPoint(rect.Left, rect.Top),
                new WpfPoint(rect.Left + rect.Width / 2D, rect.Top),
                new WpfPoint(rect.Right, rect.Top),
                new WpfPoint(rect.Right, rect.Top + rect.Height / 2D),
                new WpfPoint(rect.Right, rect.Bottom),
                new WpfPoint(rect.Left + rect.Width / 2D, rect.Bottom),
                new WpfPoint(rect.Left, rect.Bottom),
                new WpfPoint(rect.Left, rect.Top + rect.Height / 2D)
            };

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = RotatePoint(points[i], center, rotationDegrees);
            }

            return points;
        }

        private static WpfPoint GetRoiCenter(WpfRect rect)
        {
            return rect.IsEmpty
                ? new WpfPoint()
                : new WpfPoint(rect.Left + rect.Width / 2D, rect.Top + rect.Height / 2D);
        }

        private static WpfPoint RotatePoint(WpfPoint point, WpfPoint center, double rotationDegrees)
        {
            if (Math.Abs(rotationDegrees) < 0.0001D)
            {
                return point;
            }

            double radians = rotationDegrees * Math.PI / 180D;
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);
            double dx = point.X - center.X;
            double dy = point.Y - center.Y;
            return new WpfPoint(
                center.X + (dx * cos) - (dy * sin),
                center.Y + (dx * sin) + (dy * cos));
        }

        private static WpfPoint ImageToLocalRoiPoint(WpfPoint imagePoint, WpfRect rect, double rotationDegrees)
        {
            return RotatePoint(imagePoint, GetRoiCenter(rect), -rotationDegrees);
        }

        private WpfPoint GetRotationHandlePoint(WpfRect rect, double scale)
        {
            double distance = RoiRotationHandleScreenDistance / Math.Max(0.0001D, scale);
            WpfPoint localPoint = new WpfPoint(rect.Left + rect.Width / 2D, rect.Top - distance);
            return RotatePoint(localPoint, GetRoiCenter(rect), templateRotationDegrees);
        }

        private static double GetPointerAngleDegrees(WpfPoint center, WpfPoint imagePoint)
        {
            return Math.Atan2(imagePoint.Y - center.Y, imagePoint.X - center.X) * 180D / Math.PI;
        }

        private WpfRect RotateRoiFromDrag(WpfPoint currentImagePoint)
        {
            WpfPoint center = GetRoiCenter(roiDragStartRect);
            double currentPointerAngle = GetPointerAngleDegrees(center, currentImagePoint);
            SetTemplateRotationDegrees(roiDragStartRotationDegrees + currentPointerAngle - roiDragStartPointerAngleDegrees);
            return roiDragStartRect;
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

        private void ResetRoiDragState()
        {
            roiDragOperation = TemplateRoiDragOperation.None;
            roiDragHandle = TemplateRoiHitHandle.None;
            roiDragStartRect = WpfRect.Empty;
            viewerHost.Cursor = Cursors.Arrow;
        }

        private void SetTemplateRotationDegrees(double rotationDegrees, bool updateText = true)
        {
            templateRotationDegrees = TemplateImageExtraction.NormalizeRotationDegrees(rotationDegrees);
            if (updateText && txtRotation != null)
            {
                isUpdatingRotationText = true;
                txtRotation.Text = templateRotationDegrees.ToString("0.###", CultureInfo.InvariantCulture);
                isUpdatingRotationText = false;
            }

            UpdateWpfRoiRectangle();
            UpdateSelectionText();
            UpdatePatternPreview();
        }

        private void UpdateSelectionText()
        {
            txtX.Text = selectedRegion.X.ToString(CultureInfo.InvariantCulture);
            txtY.Text = selectedRegion.Y.ToString(CultureInfo.InvariantCulture);
            txtWidth.Text = selectedRegion.Width.ToString(CultureInfo.InvariantCulture);
            txtHeight.Text = selectedRegion.Height.ToString(CultureInfo.InvariantCulture);
            if (txtRotation != null && !isUpdatingRotationText)
            {
                isUpdatingRotationText = true;
                txtRotation.Text = templateRotationDegrees.ToString("0.###", CultureInfo.InvariantCulture);
                isUpdatingRotationText = false;
            }

            string text = HasValidRegion(selectedRegion)
                ? $"X {selectedRegion.X} / Y {selectedRegion.Y} / W {selectedRegion.Width} / H {selectedRegion.Height} / R {templateRotationDegrees:0.###}"
                : "No template ROI";
            footerText.Text = text;
            statusText.Text = text;
        }

        private void SetStatus(string message)
        {
            statusText.Text = message;
            footerText.Text = HasValidRegion(selectedRegion)
                ? $"X {selectedRegion.X} / Y {selectedRegion.Y} / W {selectedRegion.Width} / H {selectedRegion.Height} / R {templateRotationDegrees:0.###}"
                : message;
        }

        private void UpdatePatternPreview()
        {
            if (!HasValidRegion(selectedRegion))
            {
                SetPatternPreview(null);
                return;
            }

            try
            {
                // Show exactly what will be saved: a rotated ROI is affined back to a
                // zero-degree template so matching can start from 0 degrees.
                using Mat sourceMat = BitmapImageConverter.ToMat(sourceBitmap);
                using Mat templateMat = TemplateImageExtraction.Extract(sourceMat, selectedRegion, templateRotationDegrees);
                if (templateMat.Empty())
                {
                    SetPatternPreview(null);
                    return;
                }

                using Bitmap templateBitmap = BitmapImageConverter.ToBitmap(templateMat);
                SetPatternPreview(CreateBitmapSource(templateBitmap));
            }
            catch
            {
                SetPatternPreview(null);
            }
        }

        private void SetPatternPreview(BitmapSource source)
        {
            patternPreviewImage.Source = source;
            patternPreviewEmptyText.Visibility = source == null ? Visibility.Visible : Visibility.Collapsed;
        }

        private CvRect ToOpenCvRect(CanvasRect<float> canvasRect)
        {
            if (canvasRect == null || canvasRect.IsEmpty()) { return new CvRect(); }

            int left = Clamp((int)Math.Round(canvasRect.Left), 0, sourceBitmap.Width);
            int right = Clamp((int)Math.Round(canvasRect.Right), 0, sourceBitmap.Width);
            int topFromImage = Clamp((int)Math.Round(sourceBitmap.Height - canvasRect.Top), 0, sourceBitmap.Height);
            int bottomFromImage = Clamp((int)Math.Round(sourceBitmap.Height - canvasRect.Bottom), 0, sourceBitmap.Height);

            int x = Math.Min(left, right);
            int y = Math.Min(topFromImage, bottomFromImage);
            int width = Math.Abs(right - left);
            int height = Math.Abs(bottomFromImage - topFromImage);
            return width > 0 && height > 0 ? new CvRect(x, y, width, height) : new CvRect();
        }

        private CvRect ToOpenCvRect(DrawingRectangle rect)
        {
            return rect.Width > 0 && rect.Height > 0
                ? new CvRect(rect.X, rect.Y, rect.Width, rect.Height)
                : new CvRect();
        }

        private DrawingRectangle ClampToImage(DrawingRectangle roi)
        {
            if (sourceBitmap.Width <= 0 || sourceBitmap.Height <= 0)
            {
                return new DrawingRectangle();
            }

            if (roi.Width <= 0 || roi.Height <= 0)
            {
                return CreateCenteredRoi();
            }

            int width = Clamp(roi.Width, 1, sourceBitmap.Width);
            int height = Clamp(roi.Height, 1, sourceBitmap.Height);
            int x = Clamp(roi.X, 0, Math.Max(0, sourceBitmap.Width - width));
            int y = Clamp(roi.Y, 0, Math.Max(0, sourceBitmap.Height - height));
            return new DrawingRectangle(x, y, width, height);
        }

        private DrawingRectangle CreateCenteredRoi()
        {
            int width = Math.Max(1, sourceBitmap.Width / 2);
            int height = Math.Max(1, sourceBitmap.Height / 2);
            return new DrawingRectangle(
                Math.Max(0, (sourceBitmap.Width - width) / 2),
                Math.Max(0, (sourceBitmap.Height - height) / 2),
                width,
                height);
        }

        private static Bitmap CloneSourceBitmap(Bitmap image)
        {
            if (image == null || image.Width <= 0 || image.Height <= 0)
            {
                return new Bitmap(10, 10, DrawingPixelFormat.Format24bppRgb);
            }

            try
            {
                return WpfBitmapSourceFactory.CloneCompatibleBitmap(image);
            }
            catch
            {
                return new Bitmap(10, 10, DrawingPixelFormat.Format24bppRgb);
            }
        }

        private static bool HasValidRegion(CvRect rect)
        {
            return rect.Width > 0 && rect.Height > 0;
        }

        private static BitmapSource CreateBitmapSource(Bitmap bitmap)
        {
            return WpfBitmapSourceFactory.Create(bitmap);
        }

        private static int Clamp(int value, int minimum, int maximum)
        {
            if (maximum < minimum) { return minimum; }
            if (value < minimum) { return minimum; }
            if (value > maximum) { return maximum; }
            return value;
        }

        private static double Clamp(double value, double minimum, double maximum)
        {
            if (maximum < minimum) { return minimum; }
            if (value < minimum) { return minimum; }
            if (value > maximum) { return maximum; }
            return value;
        }

        private enum TemplateRoiDragOperation
        {
            None,
            Create,
            Move,
            Resize,
            Rotate
        }

        private enum TemplateRoiHitHandle
        {
            None,
            Rotation,
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
    }
}
