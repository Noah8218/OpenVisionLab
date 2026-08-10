using OpenVisionLab.Core;
using OpenVisionLab.ImageSpace.Core;
using System;
using System.Drawing;
using System.Linq;
using WindowsPoint = System.Windows.Point;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostLayerTestFacade
    {
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionShellHostStatePresenter statePresenter;
        private readonly OpenVisionShellHostLayerListPresenter layerListPresenter;
        private readonly OpenVisionShellHostLayerViewerController layerViewerController;
        private readonly OpenVisionShellHostLayerActivationController layerActivationController;
        private readonly OpenVisionShellHostTestAdapter testAdapter;
        private readonly OpenVisionShellHostRefreshCoordinator refreshCoordinator;
        private readonly OpenVisionShellHostChromeController chromeController;
        private readonly OpenVisionShellHostWorkspacePreviewController workspacePreviewController;
        private readonly OpenVisionShellHostWorkspaceImageController workspaceImageController;
        private readonly OpenVisionShellHostCommandController commandController;
        private readonly OpenVisionZoomableImageController workspaceFallbackZoomController;
        private readonly OpenVisionShellHostLayerTestFacadeBindings bindings;

        public OpenVisionShellHostLayerTestFacade(
            IDisplayManager displayManager,
            OpenVisionShellHostStatePresenter statePresenter,
            OpenVisionShellHostLayerListPresenter layerListPresenter,
            OpenVisionShellHostLayerViewerController layerViewerController,
            OpenVisionShellHostLayerActivationController layerActivationController,
            OpenVisionShellHostTestAdapter testAdapter,
            OpenVisionShellHostRefreshCoordinator refreshCoordinator,
            OpenVisionShellHostChromeController chromeController,
            OpenVisionShellHostWorkspacePreviewController workspacePreviewController,
            OpenVisionShellHostWorkspaceImageController workspaceImageController,
            OpenVisionShellHostCommandController commandController,
            OpenVisionZoomableImageController workspaceFallbackZoomController,
            OpenVisionShellHostLayerTestFacadeBindings bindings)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.statePresenter = statePresenter ?? throw new ArgumentNullException(nameof(statePresenter));
            this.layerListPresenter = layerListPresenter ?? throw new ArgumentNullException(nameof(layerListPresenter));
            this.layerViewerController = layerViewerController ?? throw new ArgumentNullException(nameof(layerViewerController));
            this.layerActivationController = layerActivationController ?? throw new ArgumentNullException(nameof(layerActivationController));
            this.testAdapter = testAdapter ?? throw new ArgumentNullException(nameof(testAdapter));
            this.refreshCoordinator = refreshCoordinator ?? throw new ArgumentNullException(nameof(refreshCoordinator));
            this.chromeController = chromeController ?? throw new ArgumentNullException(nameof(chromeController));
            this.workspacePreviewController = workspacePreviewController ?? throw new ArgumentNullException(nameof(workspacePreviewController));
            this.workspaceImageController = workspaceImageController ?? throw new ArgumentNullException(nameof(workspaceImageController));
            this.commandController = commandController ?? throw new ArgumentNullException(nameof(commandController));
            this.workspaceFallbackZoomController = workspaceFallbackZoomController ?? throw new ArgumentNullException(nameof(workspaceFallbackZoomController));
            this.bindings = bindings ?? throw new ArgumentNullException(nameof(bindings));
        }

        public int LayerDocumentCount => statePresenter.LayerDocumentCount;

        public bool HasMainLayer => statePresenter.HasMainLayer;

        public int HostLayerRowCount => statePresenter.HostLayerRowCount;

        public string ActiveHostLayerTitle => statePresenter.ActiveHostLayerTitle;

        public string SelectedHostLayerTitle => bindings.SelectedHostLayerTitleText();

        public string SelectedHostLayerMeta => bindings.SelectedHostLayerMetaText();

        public bool HasSelectedHostLayerPreview => bindings.HasSelectedHostLayerPreview();

        public bool HasWorkspaceLayerPreview => statePresenter.HasWorkspaceLayerPreview;

        public bool IsSingleWorkspaceVisible => bindings.IsSingleWorkspaceVisible();

        public bool IsWorkspaceLayerDropEnabled => bindings.IsWorkspaceLayerDropEnabled();

        public bool HasWorkspaceDropOverlay => bindings.HasWorkspaceDropOverlay();

        public bool IsWorkspaceDropOverlayVisible => bindings.IsWorkspaceDropOverlayVisible();

        public bool IsWorkspaceDropOverlayHitTestSafe => bindings.IsWorkspaceDropOverlayHitTestSafe();

        public int WorkspaceTextureTileCount => statePresenter.WorkspaceTextureTileCount;

        public bool IsWorkspaceEmptyPromptVisible => bindings.IsWorkspaceEmptyPromptVisible();

        public string WorkspaceCoordinatesText => bindings.WorkspaceCoordinatesText();

        public string WorkspacePixelText => bindings.WorkspacePixelText();

        public string WorkspaceEmptyTitle => bindings.WorkspaceEmptyTitleText();

        public string WorkspaceEmptyDetail => bindings.WorkspaceEmptyDetailText();

        public string WorkspaceLayerTitle => bindings.WorkspaceLayerTitleText();

        public string WorkspaceLayerMeta => bindings.WorkspaceLayerMetaText();

        public string WorkspaceLoadImageMenuText => bindings.WorkspaceLoadImageMenuText();

        public string WorkspaceLoadImageButtonText => bindings.WorkspaceLoadImageButtonText();

        public bool HasWorkspaceLoadImageMenu => bindings.HasWorkspaceLoadImageMenu();

        public bool IsWorkspaceLoadImageIntoLayerMenuVisible => bindings.IsWorkspaceLoadImageIntoLayerMenuVisible();

        public int OpenLayerViewerWindowCount => statePresenter.OpenLayerViewerWindowCount;

        public string OpenLayerViewerWindowTitles => statePresenter.OpenLayerViewerWindowTitles;

        public string HostLayerTabTexts => string.Join("|", layerListPresenter.Rows.Select(row => row.DisplayText));

        public bool AreHostLayerTabsReadable => testAdapter.AreHostLayerTabsReadable();

        public bool OpenLayerViewer(string layerTitle)
        {
            return layerViewerController.Open(layerTitle);
        }

        public bool HasLayer(string layerTitle)
        {
            return displayManager.FindIndex(layerTitle) >= 0;
        }

        public Bitmap GetLayerImageClone(string layerTitle)
        {
            Bitmap image = displayManager.GetLayerImage(layerTitle);
            return OpenVisionShellHostWorkspaceImageController.CloneBitmapForLayer(image);
        }

        public bool ActivateHostLayer(string layerTitle)
        {
            return layerViewerController.CanOpen(layerTitle)
                && layerActivationController.Activate(layerTitle);
        }

        public bool SelectHostLayerRow(string layerTitle)
        {
            return testAdapter.SelectHostLayerRow(layerTitle);
        }

        public bool RightClickHostLayerRow(string layerTitle)
        {
            return testAdapter.RightClickHostLayerRow(layerTitle);
        }

        public bool AddLayerImage(string layerTitle, Bitmap image)
        {
            if (string.IsNullOrWhiteSpace(layerTitle) || image == null)
            {
                return false;
            }

            displayManager.CreateLayerDisplay(
                ImageSpaceFrame.FromBitmap(OpenVisionShellHostWorkspaceImageController.CloneBitmapForLayer(image)),
                layerTitle,
                true);
            displayManager.SelectedItem = layerTitle;
            displayManager.ActivateLayer(layerTitle);
            refreshCoordinator.RefreshHostSelectedLayerDetail(layerTitle);
            refreshCoordinator.RefreshHostLayerRows();
            chromeController.RefreshDirectRouteText();
            refreshCoordinator.RefreshDockedLayerViews();
            return layerViewerController.CanOpen(layerTitle);
        }

        public bool SaveWorkspaceImageToFile(string path)
        {
            return workspacePreviewController.SaveCurrentImage(path);
        }

        public void SetMainLayerImage(Bitmap image)
        {
            if (image == null)
            {
                return;
            }

            workspaceImageController.ApplyMainLayerImage(image, false);
        }

        public bool LoadMainImageFromFile(string path)
        {
            bool loaded = workspaceImageController.LoadImage(path);
            if (loaded)
            {
                commandController.RecordWorkspaceImagePath(path);
                bindings.WorkspaceImageReady();
            }

            return loaded;
        }

        public bool UpdateWorkspacePointerAtCenter()
        {
            if (!TryGetWorkspaceImageSurfaceSize(out double width, out double height))
            {
                return false;
            }

            return workspaceFallbackZoomController.UpdatePointerStatusForTest(new WindowsPoint(width / 2D, height / 2D));
        }

        public string GetWorkspacePointerCoordinate(double xRatio, double yRatio)
        {
            if (!TryGetWorkspaceImageSurfaceSize(out _, out _))
            {
                return string.Empty;
            }

            WindowsPoint point = CreateWorkspaceSurfacePoint(xRatio, yRatio);
            return workspaceFallbackZoomController.TryGetPointerStatusForTest(point, out OpenVisionZoomableImageStatus status)
                ? status.FormatCoordinates()
                : string.Empty;
        }

        public void ZoomWorkspaceAt(double xRatio, double yRatio, double factor)
        {
            if (!TryGetWorkspaceImageSurfaceSize(out _, out _))
            {
                return;
            }

            workspaceFallbackZoomController.ZoomAtForTest(CreateWorkspaceSurfacePoint(xRatio, yRatio), factor);
        }

        public void PanWorkspaceBy(double surfaceDeltaX, double surfaceDeltaY)
        {
            workspaceFallbackZoomController.PanByForTest(surfaceDeltaX, surfaceDeltaY);
        }

        private bool TryGetWorkspaceImageSurfaceSize(out double width, out double height)
        {
            width = bindings.WorkspaceImageSurfaceWidth();
            height = bindings.WorkspaceImageSurfaceHeight();
            return width > 0D && height > 0D;
        }

        private WindowsPoint CreateWorkspaceSurfacePoint(double xRatio, double yRatio)
        {
            double x = Math.Max(0D, Math.Min(1D, xRatio)) * bindings.WorkspaceImageSurfaceWidth();
            double y = Math.Max(0D, Math.Min(1D, yRatio)) * bindings.WorkspaceImageSurfaceHeight();
            return new WindowsPoint(x, y);
        }
    }

    internal sealed class OpenVisionShellHostLayerTestFacadeBindings
    {
        public Func<string> SelectedHostLayerTitleText { get; set; } = EmptyText;

        public Func<string> SelectedHostLayerMetaText { get; set; } = EmptyText;

        public Func<bool> HasSelectedHostLayerPreview { get; set; } = False;

        public Func<bool> IsSingleWorkspaceVisible { get; set; } = False;

        public Func<bool> IsWorkspaceLayerDropEnabled { get; set; } = False;

        public Func<bool> HasWorkspaceDropOverlay { get; set; } = False;

        public Func<bool> IsWorkspaceDropOverlayVisible { get; set; } = False;

        public Func<bool> IsWorkspaceDropOverlayHitTestSafe { get; set; } = False;

        public Func<bool> IsWorkspaceEmptyPromptVisible { get; set; } = False;

        public Func<string> WorkspaceCoordinatesText { get; set; } = EmptyText;

        public Func<string> WorkspacePixelText { get; set; } = EmptyText;

        public Func<string> WorkspaceEmptyTitleText { get; set; } = EmptyText;

        public Func<string> WorkspaceEmptyDetailText { get; set; } = EmptyText;

        public Func<string> WorkspaceLayerTitleText { get; set; } = EmptyText;

        public Func<string> WorkspaceLayerMetaText { get; set; } = EmptyText;

        public Func<string> WorkspaceLoadImageMenuText { get; set; } = EmptyText;

        public Func<string> WorkspaceLoadImageButtonText { get; set; } = EmptyText;

        public Func<bool> HasWorkspaceLoadImageMenu { get; set; } = False;

        public Func<bool> IsWorkspaceLoadImageIntoLayerMenuVisible { get; set; } = False;

        public Action WorkspaceImageReady { get; set; } = NoOp;

        public Func<double> WorkspaceImageSurfaceWidth { get; set; } = Zero;

        public Func<double> WorkspaceImageSurfaceHeight { get; set; } = Zero;

        private static string EmptyText()
        {
            return string.Empty;
        }

        private static bool False()
        {
            return false;
        }

        private static double Zero()
        {
            return 0D;
        }

        private static void NoOp()
        {
        }
    }
}
