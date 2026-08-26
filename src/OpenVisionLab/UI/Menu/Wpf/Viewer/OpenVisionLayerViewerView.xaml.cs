using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionLayerViewerView : UserControl, IOpenVisionDockedLayerViewer, IDisposable
    {
        private static readonly object liveInstancesSync = new();
        private static readonly List<WeakReference<OpenVisionLayerViewerView>> liveInstances = new();
        private readonly OpenVisionBitmapCanvasPresenter canvasPresenter;
        private readonly OpenVisionZoomableImageController fallbackImageZoomController;
        private DispatcherOperation pendingCanvasRefresh;
        private Bitmap ownedLayerImage;
        private bool isCompactChrome;
        private bool disposed;

        public OpenVisionLayerViewerView()
        {
            lock (liveInstancesSync)
            {
                liveInstances.Add(new WeakReference<OpenVisionLayerViewerView>(this));
            }

            InitializeComponent();
            canvasPresenter = new OpenVisionBitmapCanvasPresenter("OpenVisionLab_LayerViewer", "Layer");
            fallbackImageZoomController = new OpenVisionZoomableImageController(layerImageSurface, layerFallbackImage);
            layerCanvas.DataContext = canvasPresenter.CanvasViewModel;
            ApplyEmptyText();
            layerCanvas.Loaded += LayerCanvas_Loaded;
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
            // AvalonDock raises Unloaded during pane rearrangement; document/window close paths own disposal.
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

        public void FitImageToViewForTest()
        {
            canvasPresenter.FitImageToView();
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
            previousImage?.Dispose();

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
            canvasPresenter.Dispose();
            layerCanvas.DataContext = null;
            layerCanvas.Dispose();
            fallbackImageZoomController.Dispose();
            layerFallbackImage.Source = null;
            ReplaceOwnedLayerImage(null);
            DataContext = null;
            Content = null;
            GC.SuppressFinalize(this);
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
