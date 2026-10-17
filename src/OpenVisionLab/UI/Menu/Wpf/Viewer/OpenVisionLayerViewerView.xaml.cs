using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionLayerViewerView : UserControl, IOpenVisionDockedLayerViewer, INotifyPropertyChanged, IDisposable
    {
        private static readonly object liveInstancesSync = new();
        private static readonly List<WeakReference<OpenVisionLayerViewerView>> liveInstances = new();
        private readonly OpenVisionBitmapCanvasPresenter canvasPresenter;
        private readonly OpenVisionZoomableImageController fallbackImageZoomController;
        private DispatcherOperation pendingCanvasRefresh;
        private DispatcherOperation pendingOverlayRefresh;
        private Bitmap ownedLayerImage;
        private bool isCompactChrome;
        private OpenVisionWorkbenchViewerPane mainPane;
        private Bitmap lastMainSourceImage;
        private string lastMainSource;
        private bool disposed;
        private bool updatingMainImage;
        private bool applyingMainViewport;
        private bool pendingMainViewport;
        private IReadOnlyList<OpenVisionMainPaneOverlay> displayOverlays = Array.Empty<OpenVisionMainPaneOverlay>();
        private OpenVisionZoomableImageStatus pointerStatus = OpenVisionZoomableImageStatus.Empty;

        public OpenVisionLayerViewerView()
        {
            lock (liveInstancesSync)
            {
                liveInstances.Add(new WeakReference<OpenVisionLayerViewerView>(this));
            }

            InitializeComponent();
            canvasPresenter = new OpenVisionBitmapCanvasPresenter("OpenVisionLab_LayerViewer", "Layer");
            fallbackImageZoomController = new OpenVisionZoomableImageController(layerImageSurface, layerFallbackImage, ApplyPointerStatus, usePixelZoomRange: true);
            layerCanvas.DataContext = canvasPresenter.CanvasViewModel;
            mainPaneFooter.DataContext = this;
            fallbackImageZoomController.ZoomChanged += OnZoomChanged;
            fallbackImageZoomController.ViewportChanged += OnViewportChanged;
            layerImageSurface.SizeChanged += OnImageSurfaceSizeChanged;
            ApplyEmptyText();
            layerCanvas.Loaded += LayerCanvas_Loaded;
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
            // AvalonDock raises Unloaded during pane rearrangement; document/window close paths own disposal.
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler PointerStatusChanged;

        OpenVisionZoomableImageStatus IOpenVisionDockedLayerViewer.PointerStatus => pointerStatus;

        public event EventHandler ZoomChanged
        {
            add => fallbackImageZoomController.ZoomChanged += value;
            remove => fallbackImageZoomController.ZoomChanged -= value;
        }

        public double ZoomScale => fallbackImageZoomController.PixelZoomScale;
        public string PointerText => pointerStatus.HasPixel ? $"X {pointerStatus.ImageX}  Y {pointerStatus.ImageY}  GV {pointerStatus.GrayValue}" : "X —  Y —  GV —";
        public string ZoomText => HasImage ? Math.Round(ZoomScale * 100D, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture) + "%" : string.Empty;
        public static readonly DependencyProperty NavigationChangedCommandProperty = DependencyProperty.Register(nameof(NavigationChangedCommand), typeof(ICommand), typeof(OpenVisionLayerViewerView));
        public ICommand NavigationChangedCommand { get => (ICommand)GetValue(NavigationChangedCommandProperty); set => SetValue(NavigationChangedCommandProperty, value); }
        internal OpenVisionImageViewport? ViewportForTest => fallbackImageZoomController.CaptureViewport();
        internal void PanImageByForTest(double x, double y) => fallbackImageZoomController.PanByForTest(x, y);
        internal OpenVisionWorkbenchViewerPane MainPane => mainPane;
        internal void SetMainPane(OpenVisionWorkbenchViewerPane pane)
        {
            if (mainPane == null) { Resources.MergedDictionaries.Clear(); }
            mainPane = pane;
            MinWidth = 0;
            mainPaneHeader.DataContext = mainPaneEmpty.DataContext = pane;
            layerViewerFrame.DataContext = pane;
            layerViewerFrame.ClearValue(Border.BorderBrushProperty);
            layerViewerFrame.Style = (Style)Resources["MainPane.FrameStyle"];
            SetBinding(NavigationChangedCommandProperty, new Binding(nameof(pane.NavigationChangedCommand)) { Source = pane });
            layerViewerFrame.SetResourceReference(Border.BackgroundProperty, "OpenVision.Workbench.CanvasBrush");
            imageFrame.SetResourceReference(Border.BackgroundProperty, "OpenVision.Workbench.CanvasBrush");
            imageFrame.BorderThickness = new Thickness(0);
            imageFrame.Margin = new Thickness(0);
            mainPaneHeader.Visibility = mainPaneFooter.Visibility = Visibility.Visible;
            statusRow.Height = new GridLength(0);
            viewerRootGrid.Margin = new Thickness(0);
            pane.FitAction = FitImageToView;
        }
        internal void UpdateMainPaneImage(Bitmap image)
        {
            if (!ReferenceEquals(lastMainSourceImage, image) || lastMainSource != mainPane.Source)
            {
                updatingMainImage = true;
                try { SetLayer(mainPane.Title, image, string.Empty); } finally { updatingMainImage = false; }
                lastMainSourceImage = image; lastMainSource = mainPane.Source;
            }
            else { txtLayerTitle.Text = mainPane.Title; }
            emptyOverlay.Visibility = Visibility.Collapsed;
            mainPaneEmpty.Visibility = image == null ? Visibility.Visible : Visibility.Collapsed;
            ApplyMainViewport();
            SetDisplayOverlays(mainPane.Overlays);
        }
        internal void SetDisplayOverlays(IReadOnlyList<OpenVisionMainPaneOverlay> overlays)
        {
            displayOverlays = overlays ?? Array.Empty<OpenVisionMainPaneOverlay>();
            RenderDisplayOverlays();
        }
        internal int DisplayOverlayVisualCountForTest => mainDisplayOverlays.Children.Count;
        private void RenderDisplayOverlays()
        {
            if (disposed || pendingOverlayRefresh != null) { return; }
            pendingOverlayRefresh = Dispatcher.BeginInvoke(new Action(() => { pendingOverlayRefresh = null; DrawDisplayOverlays(); }), DispatcherPriority.Render);
        }
        private void DrawDisplayOverlays()
        {
            mainDisplayOverlays.Children.Clear();
            if (disposed || !HasImage) { return; }
            foreach (OpenVisionMainPaneOverlay overlay in displayOverlays)
            {
                VisionToolOverlay shape = overlay.Shape;
                string brush = "OpenVision.Workbench." + (overlay.IsSelected ? "CanvasSelectionBrush" : overlay.IsRejected ? "CanvasRejectedBrush" : overlay.IsRoi ? "CanvasRoiBrush" : overlay.Failed ? "CanvasNgBrush" : "CanvasOkBrush");
                if (!fallbackImageZoomController.TryMapImagePoint(new System.Windows.Point(shape.Kind == VisionToolOverlayKind.Rectangle ? shape.Bounds.Left : shape.Kind == VisionToolOverlayKind.Line ? shape.Start.X : shape.Center.X,
                    shape.Kind == VisionToolOverlayKind.Rectangle ? shape.Bounds.Top : shape.Kind == VisionToolOverlayKind.Line ? shape.Start.Y : shape.Center.Y), out System.Windows.Point anchor)) { continue; }
                if (shape.Kind == VisionToolOverlayKind.Rectangle && shape.Bounds.Width > 0 && shape.Bounds.Height > 0
                    && fallbackImageZoomController.TryMapImagePoint(new System.Windows.Point(shape.Bounds.Right, shape.Bounds.Bottom), out System.Windows.Point end))
                {
                    System.Windows.Shapes.Rectangle box = new() { Width = Math.Abs(end.X - anchor.X), Height = Math.Abs(end.Y - anchor.Y), StrokeThickness = overlay.IsSelected ? 3D : 1.5D };
                    if (overlay.IsRejected) { box.StrokeDashArray = new System.Windows.Media.DoubleCollection { 3D, 3D }; }
                    if (overlay.IsRoi) { box.StrokeDashArray = new System.Windows.Media.DoubleCollection { 4D, 3D }; }
                    box.SetResourceReference(System.Windows.Shapes.Shape.StrokeProperty, brush);
                    Canvas.SetLeft(box, Math.Min(anchor.X, end.X)); Canvas.SetTop(box, Math.Min(anchor.Y, end.Y)); mainDisplayOverlays.Children.Add(box);
                    if (overlay.IsSelected
                        && fallbackImageZoomController.TryMapImagePoint(new System.Windows.Point(shape.Bounds.Left - 5D, shape.Bounds.Top - 5D), out System.Windows.Point frameStart)
                        && fallbackImageZoomController.TryMapImagePoint(new System.Windows.Point(shape.Bounds.Right + 5D, shape.Bounds.Bottom + 5D), out System.Windows.Point frameEnd))
                    {
                        System.Windows.Shapes.Rectangle frame = new() { Width = Math.Abs(frameEnd.X - frameStart.X), Height = Math.Abs(frameEnd.Y - frameStart.Y), StrokeThickness = 1D };
                        frame.SetResourceReference(System.Windows.Shapes.Shape.StrokeProperty, brush);
                        Canvas.SetLeft(frame, Math.Min(frameStart.X, frameEnd.X)); Canvas.SetTop(frame, Math.Min(frameStart.Y, frameEnd.Y)); mainDisplayOverlays.Children.Add(frame);
                    }
                }
                else if (shape.Kind == VisionToolOverlayKind.Line && fallbackImageZoomController.TryMapImagePoint(new System.Windows.Point(shape.End.X, shape.End.Y), out System.Windows.Point endLine)) { AddOverlayLine(anchor, endLine, brush); }
                else if (shape.Kind is VisionToolOverlayKind.Point or VisionToolOverlayKind.Points)
                {
                    IEnumerable<PointF> points = shape.Kind == VisionToolOverlayKind.Points ? shape.Points : new[] { shape.Center };
                    foreach (PointF point in points)
                    {
                        if (!fallbackImageZoomController.TryMapImagePoint(new System.Windows.Point(point.X, point.Y), out System.Windows.Point center)) { continue; }
                        AddOverlayLine(new System.Windows.Point(center.X - 4D, center.Y), new System.Windows.Point(center.X + 4D, center.Y), brush);
                        AddOverlayLine(new System.Windows.Point(center.X, center.Y - 4D), new System.Windows.Point(center.X, center.Y + 4D), brush);
                    }
                }
                if (!string.IsNullOrWhiteSpace(shape.Label))
                {
                    TextBlock label = new() { Text = shape.Label, FontSize = 11D, FontWeight = FontWeights.SemiBold };
                    label.SetResourceReference(TextBlock.ForegroundProperty, brush);
                    Border caption = new() { Child = label, Padding = new Thickness(3D, 1D, 3D, 1D), CornerRadius = new CornerRadius(2D) };
                    caption.SetResourceReference(Border.BackgroundProperty, "OpenVision.Workbench.CanvasBrush");
                    Canvas.SetLeft(caption, anchor.X + 3D); Canvas.SetTop(caption, Math.Max(0D, anchor.Y - 19D)); mainDisplayOverlays.Children.Add(caption);
                }
            }
        }
        private void AddOverlayLine(System.Windows.Point start, System.Windows.Point end, string brush)
        {
            System.Windows.Shapes.Line line = new() { X1 = start.X, Y1 = start.Y, X2 = end.X, Y2 = end.Y, StrokeThickness = 1.5D };
            line.SetResourceReference(System.Windows.Shapes.Shape.StrokeProperty, brush); mainDisplayOverlays.Children.Add(line);
        }

        private void OnZoomChanged(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ZoomText)));
            if (pendingMainViewport) { ApplyMainViewport(); }
            RenderDisplayOverlays();
        }
        private void ApplyMainViewport()
        {
            if (updatingMainImage || applyingMainViewport || mainPane?.Viewport is not OpenVisionImageViewport viewport) { return; }
            applyingMainViewport = true;
            try { pendingMainViewport = !fallbackImageZoomController.ApplyViewport(viewport); } finally { applyingMainViewport = false; }
        }
        private void OnImageSurfaceSizeChanged(object sender, SizeChangedEventArgs e) => RenderDisplayOverlays();

        private void OnViewportChanged(object sender, EventArgs e)
        {
            RenderDisplayOverlays();
            if (updatingMainImage || applyingMainViewport || fallbackImageZoomController.CaptureViewport() is not OpenVisionImageViewport viewport) { return; }
            ICommand command = NavigationChangedCommand;
            if (command?.CanExecute(viewport) == true) { command.Execute(viewport); }
        }

        public string LayerTitle => txtLayerTitle?.Text ?? string.Empty;

        public bool HasImage => canvasPresenter.HasImage;

        public bool IsCompactChrome => isCompactChrome;

        public bool IsCompactSizeReady => MinWidth <= 180
            && MinHeight <= 140
            && layerCanvas?.ShowStatusBar == false
            && layerCanvas?.ShowToolBar == false;

        public int ImagePixelWidth => canvasPresenter.ImagePixelWidth;

        public int ImagePixelHeight => canvasPresenter.ImagePixelHeight;

        public int TextureTileCount => canvasPresenter.TextureTileCount;

        internal static int LiveInstanceCountForTest
        {
            get
            {
                lock (liveInstancesSync)
                {
                    liveInstances.RemoveAll(reference => !reference.TryGetTarget(out _));
                    return liveInstances.Count;
                }
            }
        }

        internal static string LiveInstanceStatesForTest
        {
            get
            {
                lock (liveInstancesSync)
                {
                    liveInstances.RemoveAll(reference => !reference.TryGetTarget(out _));
                    List<string> states = new();
                    foreach (WeakReference<OpenVisionLayerViewerView> reference in liveInstances)
                    {
                        if (reference.TryGetTarget(out OpenVisionLayerViewerView view))
                        {
                            states.Add(
                                (view.disposed ? "Disposed:" : "Active:")
                                + view.LayerTitle
                                + (view.Tag is string ? ":Tagged" : ":Untagged"));
                        }
                    }

                    return string.Join("|", states);
                }
            }
        }

        public Bitmap CloneImageForTest()
        {
            return CloneLayerBitmap(ownedLayerImage);
        }

        public bool SaveImageToFileForTest(string path)
        {
            return canvasPresenter.SaveCurrentImage(path);
        }

        public void FitImageToView()
        {
            if (disposed || !canvasPresenter.HasImage) { return; }
            fallbackImageZoomController.Reset();
            canvasPresenter.FitImageToView();
        }

        public void FitImageToViewForTest() => FitImageToView();

        public void ZoomImage(double factor)
        {
            if (!disposed) { fallbackImageZoomController.ZoomImage(factor); }
        }

        public void ShowOriginalSize()
        {
            if (!disposed) { fallbackImageZoomController.ShowOriginalSize(); }
        }

        public void SetCompactChrome(bool compact)
        {
            isCompactChrome = compact;
            MinWidth = compact ? 160 : 420;
            MinHeight = compact ? 120 : 300;

            if (viewerRootGrid != null)
            {
                viewerRootGrid.Margin = compact ? new Thickness(3) : new Thickness(10);
            }

            if (layerHeader != null)
            {
                layerHeader.Visibility = compact ? Visibility.Collapsed : Visibility.Visible;
            }

            if (statusFooter != null)
            {
                statusFooter.Visibility = compact ? Visibility.Collapsed : Visibility.Visible;
            }

            if (statusRow != null)
            {
                statusRow.Height = compact ? new GridLength(0) : new GridLength(30);
            }

            if (imageFrame != null)
            {
                imageFrame.Margin = compact ? new Thickness(0) : new Thickness(0, 8, 0, 6);
            }

            if (layerCanvas != null)
            {
                layerCanvas.ShowStatusBar = !compact;
                layerCanvas.ShowToolBar = !compact;
            }
        }

        public void SetLayer(string layerTitle, Bitmap image, string statusText)
        {
            string canvasImageName = string.IsNullOrWhiteSpace(layerTitle) ? "Layer" : layerTitle;
            Bitmap displayImage = CloneLayerBitmap(image);
            Bitmap previousImage = ownedLayerImage;
            ownedLayerImage = displayImage;

            txtLayerTitle.Text = string.IsNullOrWhiteSpace(layerTitle) ? "-" : layerTitle;
            txtStatus.Text = string.IsNullOrWhiteSpace(statusText) ? "-" : statusText;
            canvasPresenter.SetBitmap(ownedLayerImage, canvasImageName);
            fallbackImageZoomController.SetStatusBitmap(ownedLayerImage);
            previousImage?.Dispose();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImagePixelWidth)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImagePixelHeight)));

            if (ownedLayerImage == null)
            {
                txtLayerMeta.Text = OpenVisionLanguageService.T("Shell.LayerDetailNoImage");
                layerCanvas.Visibility = Visibility.Collapsed;
                layerFallbackImage.Source = null;
                layerFallbackImage.Visibility = Visibility.Collapsed;
                fallbackImageZoomController.Reset();
                emptyOverlay.Visibility = Visibility.Visible;
                return;
            }

            txtLayerMeta.Text = string.Format(CultureInfo.CurrentCulture, "{0}x{1}", ownedLayerImage.Width, ownedLayerImage.Height);
            layerCanvas.Visibility = Visibility.Collapsed;
            // Remote/capture paths can show the OpenGL HWND as blank; this visible fallback is navigation-only.
            layerFallbackImage.Source = OpenVisionBitmapImagePreviewFactory.Create(ownedLayerImage);
            layerFallbackImage.Visibility = Visibility.Visible;
            fallbackImageZoomController.Reset();
            emptyOverlay.Visibility = Visibility.Collapsed;
        }

        private void LayerCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            pendingCanvasRefresh = Dispatcher.BeginInvoke(new Action(() =>
            {
                pendingCanvasRefresh = null;
                if (!disposed)
                {
                    canvasPresenter.RefreshCanvas();
                }
            }), DispatcherPriority.ContextIdle);
        }

        private void OnLanguageChanged(object sender, System.EventArgs e)
        {
            ApplyEmptyText();
        }

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;

            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            layerCanvas.Loaded -= LayerCanvas_Loaded;
            if (pendingCanvasRefresh?.Status == DispatcherOperationStatus.Pending)
            {
                pendingCanvasRefresh.Abort();
            }
            pendingCanvasRefresh = null;
            fallbackImageZoomController.ZoomChanged -= OnZoomChanged;
            fallbackImageZoomController.ViewportChanged -= OnViewportChanged;
            layerImageSurface.SizeChanged -= OnImageSurfaceSizeChanged;
            ClearValue(NavigationChangedCommandProperty);
            pendingOverlayRefresh?.Abort(); pendingOverlayRefresh = null;
            displayOverlays = Array.Empty<OpenVisionMainPaneOverlay>();
            mainDisplayOverlays.Children.Clear();
            if (mainPane != null) { mainPane.FitAction = null; }
            mainPane = null; lastMainSourceImage = null;
            layerCanvas.Dispose();
            canvasPresenter.Dispose();
            fallbackImageZoomController.Dispose();
            PointerStatusChanged = null;
            layerFallbackImage.Source = null;
            ReplaceOwnedLayerImage(null);
            DataContext = null;
            Content = null;
            GC.SuppressFinalize(this);
        }

        private void ApplyPointerStatus(OpenVisionZoomableImageStatus status)
        {
            pointerStatus = status ?? OpenVisionZoomableImageStatus.Empty;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PointerText)));
            PointerStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ApplyEmptyText()
        {
            if (txtEmptyTitle != null)
            {
                txtEmptyTitle.Text = OpenVisionLanguageService.T("Shell.LayerDetailNoImage");
            }
        }

        private void ReplaceOwnedLayerImage(Bitmap image)
        {
            if (ReferenceEquals(ownedLayerImage, image))
            {
                return;
            }

            ownedLayerImage?.Dispose();
            ownedLayerImage = image;
        }

        private static Bitmap CloneLayerBitmap(Bitmap image)
        {
            if (image == null)
            {
                return null;
            }

            try
            {
                return image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            }
            catch
            {
                try
                {
                    return new Bitmap(image);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
