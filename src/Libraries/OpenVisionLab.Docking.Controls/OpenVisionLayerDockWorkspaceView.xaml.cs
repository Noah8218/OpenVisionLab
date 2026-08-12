using AvalonDock.Layout;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionLayerDockWorkspaceView : UserControl
    {
        private static readonly Thickness DefaultPaneGuideMargin = new Thickness(22D);

        public static readonly DependencyProperty IsWorkspaceDropEnabledProperty =
            DependencyProperty.Register(
                nameof(IsWorkspaceDropEnabled),
                typeof(bool),
                typeof(OpenVisionLayerDockWorkspaceView),
                new PropertyMetadata(false, OnIsWorkspaceDropEnabledChanged));

        public static readonly DependencyProperty IsGuideOverlayVisibleProperty =
            DependencyProperty.Register(
                nameof(IsGuideOverlayVisible),
                typeof(bool),
                typeof(OpenVisionLayerDockWorkspaceView),
                new PropertyMetadata(false, OnIsGuideOverlayVisibleChanged));

        public static readonly DependencyProperty ActiveGuideZoneProperty =
            DependencyProperty.Register(
                nameof(ActiveGuideZone),
                typeof(DockingGuideZone),
                typeof(OpenVisionLayerDockWorkspaceView),
                new PropertyMetadata(DockingGuideZone.Center, OnActiveGuideZoneChanged));

        public static readonly DependencyProperty PaneGuideMarginProperty =
            DependencyProperty.Register(
                nameof(PaneGuideMargin),
                typeof(Thickness),
                typeof(OpenVisionLayerDockWorkspaceView),
                new PropertyMetadata(DefaultPaneGuideMargin, OnPaneGuideMarginChanged));

        public OpenVisionLayerDockWorkspaceView()
        {
            InitializeComponent();
            HookDockingManagerEvents();
            ApplyIsWorkspaceDropEnabled();
            ApplyGuideOverlayVisible();
            ApplyActiveGuideZone();
            ApplyPaneGuideMargin();
        }

        public event EventHandler DockingLayoutChanged;

        public event EventHandler DockingContentDocked;

        public event EventHandler DockingContentFloated;

        public event EventHandler ActiveDocumentChanged;

        public bool IsWorkspaceDropEnabled
        {
            get => (bool)GetValue(IsWorkspaceDropEnabledProperty);
            set => SetValue(IsWorkspaceDropEnabledProperty, value);
        }

        public bool IsGuideOverlayVisible
        {
            get => (bool)GetValue(IsGuideOverlayVisibleProperty);
            set => SetValue(IsGuideOverlayVisibleProperty, value);
        }

        public DockingGuideZone ActiveGuideZone
        {
            get => (DockingGuideZone)GetValue(ActiveGuideZoneProperty);
            set => SetValue(ActiveGuideZoneProperty, value);
        }

        public Thickness PaneGuideMargin
        {
            get => (Thickness)GetValue(PaneGuideMarginProperty);
            set => SetValue(PaneGuideMarginProperty, value);
        }

        public OpenVisionDockWorkspaceHandle WorkspaceHandle =>
            OpenVisionDockWorkspaceHandle.FromNative(layerDockingManager, layerAnchorablePane);

        public string ActiveDocumentId
        {
            get
            {
                object activeContent = layerDockingManager?.ActiveContent;
                if (activeContent == null || layerDockingManager?.Layout == null)
                {
                    return string.Empty;
                }

                return layerDockingManager.Layout
                    .Descendents()
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(document => ReferenceEquals(document.Content, activeContent))
                    ?.ContentId
                    ?? string.Empty;
            }
        }

        public FrameworkElement PaneGuideOverlay => null;

        public Size WorkspaceSize => layerDockingManager == null
            ? Size.Empty
            : new Size(layerDockingManager.ActualWidth, layerDockingManager.ActualHeight);

        public Point PointFromWorkspaceRatio(double xRatio, double yRatio)
        {
            Size size = WorkspaceSize;
            return new Point(
                Math.Max(0D, Math.Min(1D, xRatio)) * size.Width,
                Math.Max(0D, Math.Min(1D, yRatio)) * size.Height);
        }

        public Point PointToScreenFromWorkspace(Point point)
        {
            return layerDockingManager == null
                ? new Point()
                : layerDockingManager.PointToScreen(point);
        }

        public Point TranslateToWorkspace(FrameworkElement element, Point point)
        {
            return element.TranslatePoint(point, layerDockingManager);
        }

        public void SetPaneGuideMargin(Thickness margin)
        {
            PaneGuideMargin = margin;
        }

        public void UpdateDockLayout()
        {
            layerDockingManager?.UpdateLayout();
            UpdateLayout();
        }

        private static void OnIsWorkspaceDropEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OpenVisionLayerDockWorkspaceView)d).ApplyIsWorkspaceDropEnabled();
        }

        private static void OnIsGuideOverlayVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OpenVisionLayerDockWorkspaceView)d).ApplyGuideOverlayVisible();
        }

        private static void OnActiveGuideZoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OpenVisionLayerDockWorkspaceView)d).ApplyActiveGuideZone();
        }

        private static void OnPaneGuideMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OpenVisionLayerDockWorkspaceView)d).ApplyPaneGuideMargin();
        }
    }
}
