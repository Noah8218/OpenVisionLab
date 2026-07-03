using AvalonDock;
using System;
using System.Windows;
using System.Windows.Input;

namespace OpenVisionLab.Docking.Controls
{
    public sealed partial class OpenVisionLayerDockingGestureController
    {
        private const string DragDataFormat = "OpenVisionLab.DockedLayerTitle";

        private readonly DockingManager dockingManager;
        private readonly OpenVisionLayerDockingGuidePresenter guidePresenter;
        private readonly OpenVisionDockingGuideStateController guideStateController;
        private readonly Func<string, bool> canOpenLayer;
        private readonly Func<string, DockingGuideZone, OpenVisionDockPaneHandle, bool> dockToGuideZone;

        private Point dragStartPoint;
        private bool dragCandidate;
        private bool dragActive;
        private bool dropInProgress;
        private bool dropAccepted;
        private string dragLayerTitle = string.Empty;

        public OpenVisionLayerDockingGestureController(
            OpenVisionLayerDockWorkspaceView dockWorkspaceView,
            OpenVisionLayerDockingGuidePresenter guidePresenter,
            OpenVisionDockingGuideStateController guideStateController,
            Func<string, bool> canOpenLayer,
            Func<string, DockingGuideZone, OpenVisionDockPaneHandle, bool> dockToGuideZone)
        {
            dockingManager = (RequireDockWorkspaceView(dockWorkspaceView).WorkspaceHandle.NativeWorkspace as DockingManager)
                ?? throw new ArgumentException("Dock workspace view does not expose a native docking workspace.", nameof(dockWorkspaceView));
            this.guidePresenter = guidePresenter ?? throw new ArgumentNullException(nameof(guidePresenter));
            this.guideStateController = guideStateController ?? throw new ArgumentNullException(nameof(guideStateController));
            this.canOpenLayer = canOpenLayer ?? throw new ArgumentNullException(nameof(canOpenLayer));
            this.dockToGuideZone = dockToGuideZone ?? throw new ArgumentNullException(nameof(dockToGuideZone));
        }

        public OpenVisionDockPaneHandle TargetPane { get; private set; }

        public void HandlePreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                BeginDrag(e);
            }
        }

        public void HandlePreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!dragCandidate)
            {
                return;
            }

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                Reset();
                return;
            }

            Point currentPoint = e.GetPosition(dockingManager);
            if (Math.Abs(currentPoint.X - dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance
                && Math.Abs(currentPoint.Y - dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            dragActive = true;
            ShowGuideAt(currentPoint);
            StartDragDrop(e);
        }

        public void HandlePreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Reset();
            }
        }

        public void HandleMouseLeave(object sender, MouseEventArgs e)
        {
            if (!dropInProgress && e.LeftButton != MouseButtonState.Pressed)
            {
                Reset();
            }
        }

        public void HandleDragOver(object sender, DragEventArgs e)
        {
            string layerTitle = ReadDragTitle(e);
            if (string.IsNullOrWhiteSpace(layerTitle))
            {
                return;
            }

            ShowGuideAt(e.GetPosition(dockingManager));
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        public void HandleDrop(object sender, DragEventArgs e)
        {
            string layerTitle = ReadDragTitle(e);
            if (string.IsNullOrWhiteSpace(layerTitle))
            {
                return;
            }

            Point point = e.GetPosition(dockingManager);
            OpenVisionDockPaneHandle targetPane = guidePresenter.ResolveTargetPane(point);
            DockingGuideZone zone = guidePresenter.ResolveZone(point, targetPane);
            dropAccepted = dockToGuideZone(layerTitle, zone, targetPane);
            Reset();
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        public void HandleDragLeave(object sender, DragEventArgs e)
        {
            if (!dropInProgress)
            {
                Reset();
            }
        }

        public void RefreshDuringLayoutChange()
        {
            if (dragActive && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                ShowGuideAt(Mouse.GetPosition(dockingManager));
                return;
            }

            Reset();
        }

        public void ShowGuideAt(Point point)
        {
            guideStateController.SetOverlayVisible(true);
            TargetPane = guidePresenter.ResolveTargetPane(point);
            guidePresenter.PositionPaneGuideOverlay(TargetPane);
            guideStateController.SetActiveZone(guidePresenter.ResolveZone(point, TargetPane));
        }

        public bool BeginTestDragGuide(DependencyObject source, Point point)
        {
            string layerTitle = ResolveLayerTitle(source);
            if (string.IsNullOrWhiteSpace(layerTitle))
            {
                return false;
            }

            dragLayerTitle = layerTitle;
            dragCandidate = true;
            dragActive = true;
            dragStartPoint = point;
            ShowGuideAt(point);
            return true;
        }

        public void Reset()
        {
            dragCandidate = false;
            dragActive = false;
            dragLayerTitle = string.Empty;
            TargetPane = null;
            guideStateController.SetOverlayVisible(false);
        }

        private void BeginDrag(MouseButtonEventArgs e)
        {
            dragLayerTitle = ResolveLayerTitle(e?.OriginalSource as DependencyObject);
            dragCandidate = !string.IsNullOrWhiteSpace(dragLayerTitle);
            if (!dragCandidate)
            {
                guideStateController.SetOverlayVisible(false);
                return;
            }

            dragStartPoint = e.GetPosition(dockingManager);
        }

        private void StartDragDrop(MouseEventArgs e)
        {
            if (dropInProgress || string.IsNullOrWhiteSpace(dragLayerTitle))
            {
                return;
            }

            string layerTitle = dragLayerTitle;
            DataObject data = new DataObject();
            data.SetData(DragDataFormat, layerTitle);
            dropInProgress = true;
            dropAccepted = false;
            e.Handled = true;

            try
            {
                DragDrop.DoDragDrop(dockingManager, data, DragDropEffects.Move);
                if (!dropAccepted)
                {
                    TryDockAtCurrentPointer(layerTitle);
                }
            }
            finally
            {
                dropInProgress = false;
                Reset();
            }
        }

        private bool TryDockAtCurrentPointer(string layerTitle)
        {
            if (string.IsNullOrWhiteSpace(layerTitle)
                || dockingManager.ActualWidth <= 0D
                || dockingManager.ActualHeight <= 0D)
            {
                return false;
            }

            Point point = Mouse.GetPosition(dockingManager);
            if (point.X < 0D
                || point.Y < 0D
                || point.X > dockingManager.ActualWidth
                || point.Y > dockingManager.ActualHeight)
            {
                return false;
            }

            OpenVisionDockPaneHandle targetPane = guidePresenter.ResolveTargetPane(point);
            DockingGuideZone zone = guidePresenter.ResolveZone(point, targetPane);
            dropAccepted = dockToGuideZone(layerTitle, zone, targetPane);
            return dropAccepted;
        }

        private static string ReadDragTitle(DragEventArgs e)
        {
            return e?.Data.GetDataPresent(DragDataFormat) == true
                ? e.Data.GetData(DragDataFormat) as string
                : string.Empty;
        }

        private static OpenVisionLayerDockWorkspaceView RequireDockWorkspaceView(OpenVisionLayerDockWorkspaceView dockWorkspaceView)
        {
            return dockWorkspaceView ?? throw new ArgumentNullException(nameof(dockWorkspaceView));
        }
    }
}
