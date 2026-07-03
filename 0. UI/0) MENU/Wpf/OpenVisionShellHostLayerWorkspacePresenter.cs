using OpenVisionLab.Docking.Controls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostLayerWorkspacePresenter
    {
        private readonly OpenVisionShellHostWorkspacePreviewController workspacePreviewController;
        private readonly OpenVisionZoomableImageController fallbackZoomController;
        private readonly TextBox selectedLayerTitleText;
        private readonly TextBlock selectedLayerMetaText;
        private readonly TextBlock selectedLayerRouteText;
        private readonly Image selectedLayerPreviewImage;
        private readonly UIElement singleWorkspaceView;
        private readonly UIElement dockedWorkspaceView;
        private readonly UIElement workspaceCanvas;
        private readonly UIElement workspaceEmptyOverlay;
        private readonly UIElement workspaceLayerInfoOverlay;
        private readonly TextBox topLayerNameEditor;
        private readonly TextBlock workspaceLayerTitleText;
        private readonly TextBlock workspaceLayerMetaText;
        private readonly TextBlock workspaceStatusText;
        private readonly TextBlock workspaceCoordinatesText;
        private readonly TextBlock workspacePixelText;
        private readonly Image workspaceFallbackImage;
        private readonly Button openSelectedLayerWindowButton;
        private readonly Button dockSelectedLayerButton;
        private readonly Button clearDockedLayersButton;

        public OpenVisionShellHostLayerWorkspacePresenter(
            OpenVisionShellHostWorkspacePreviewController workspacePreviewController,
            OpenVisionZoomableImageController fallbackZoomController,
            TextBox selectedLayerTitleText,
            TextBlock selectedLayerMetaText,
            TextBlock selectedLayerRouteText,
            Image selectedLayerPreviewImage,
            UIElement singleWorkspaceView,
            UIElement dockedWorkspaceView,
            UIElement workspaceCanvas,
            UIElement workspaceEmptyOverlay,
            UIElement workspaceLayerInfoOverlay,
            TextBox topLayerNameEditor,
            TextBlock workspaceLayerTitleText,
            TextBlock workspaceLayerMetaText,
            TextBlock workspaceStatusText,
            TextBlock workspaceCoordinatesText,
            TextBlock workspacePixelText,
            Image workspaceFallbackImage,
            Button openSelectedLayerWindowButton,
            Button dockSelectedLayerButton,
            Button clearDockedLayersButton)
        {
            this.workspacePreviewController = workspacePreviewController ?? throw new ArgumentNullException(nameof(workspacePreviewController));
            this.fallbackZoomController = fallbackZoomController ?? throw new ArgumentNullException(nameof(fallbackZoomController));
            this.selectedLayerTitleText = selectedLayerTitleText;
            this.selectedLayerMetaText = selectedLayerMetaText;
            this.selectedLayerRouteText = selectedLayerRouteText;
            this.selectedLayerPreviewImage = selectedLayerPreviewImage;
            this.singleWorkspaceView = singleWorkspaceView;
            this.dockedWorkspaceView = dockedWorkspaceView;
            this.workspaceCanvas = workspaceCanvas;
            this.workspaceEmptyOverlay = workspaceEmptyOverlay;
            this.workspaceLayerInfoOverlay = workspaceLayerInfoOverlay;
            this.topLayerNameEditor = topLayerNameEditor;
            this.workspaceLayerTitleText = workspaceLayerTitleText;
            this.workspaceLayerMetaText = workspaceLayerMetaText;
            this.workspaceStatusText = workspaceStatusText;
            this.workspaceCoordinatesText = workspaceCoordinatesText;
            this.workspacePixelText = workspacePixelText;
            this.workspaceFallbackImage = workspaceFallbackImage;
            this.openSelectedLayerWindowButton = openSelectedLayerWindowButton;
            this.dockSelectedLayerButton = dockSelectedLayerButton;
            this.clearDockedLayersButton = clearDockedLayersButton;
        }

        public void ApplySelectedLayerDetail(OpenVisionShellHostLayerDetailState detail)
        {
            if (detail == null)
            {
                detail = OpenVisionShellHostLayerDetailState.Empty();
            }

            if (selectedLayerTitleText != null)
            {
                selectedLayerTitleText.Text = DisplayText(detail.LayerTitle);
            }

            if (selectedLayerMetaText != null)
            {
                selectedLayerMetaText.Text = DisplayText(detail.MetaText);
            }

            if (selectedLayerRouteText != null)
            {
                selectedLayerRouteText.Text = DisplayText(detail.RouteText);
            }

            if (selectedLayerPreviewImage != null)
            {
                selectedLayerPreviewImage.Source = detail.HasImage
                    ? OpenVisionBitmapImagePreviewFactory.Create(detail.Image)
                    : null;
            }
        }

        public void ApplyWorkspace(OpenVisionShellHostLayerDetailState detail)
        {
            if (detail == null)
            {
                detail = OpenVisionShellHostLayerDetailState.Empty();
            }

            ShowWorkspaceForImageState(detail.HasImage);
            workspacePreviewController.SetLayer(detail);
            ApplyWorkspaceFallbackImage(detail);

            if (workspaceCanvas != null)
            {
                workspaceCanvas.Visibility = Visibility.Collapsed;
            }

            if (workspaceEmptyOverlay != null)
            {
                workspaceEmptyOverlay.Visibility = detail.HasImage ? Visibility.Collapsed : Visibility.Visible;
            }

            if (workspaceLayerInfoOverlay != null)
            {
                workspaceLayerInfoOverlay.Visibility = detail.HasImage ? Visibility.Visible : Visibility.Collapsed;
            }

            if (workspaceLayerTitleText != null)
            {
                workspaceLayerTitleText.Text = DisplayText(detail.LayerTitle);
            }

            if (topLayerNameEditor != null && !topLayerNameEditor.IsKeyboardFocusWithin)
            {
                topLayerNameEditor.Text = detail.LayerTitle ?? string.Empty;
            }

            if (workspaceLayerMetaText != null)
            {
                workspaceLayerMetaText.Text = DisplayText(detail.MetaText);
            }

            if (workspaceStatusText != null)
            {
                workspaceStatusText.Text = string.IsNullOrWhiteSpace(detail.RouteText)
                    ? OpenVisionLanguageService.T("Shell.LayerDetailPending")
                    : detail.RouteText;
            }

            ApplyWorkspacePointerStatus(null);
        }

        public void ApplyActionState(OpenVisionShellHostLayerActionState actionState)
        {
            if (actionState == null)
            {
                return;
            }

            if (openSelectedLayerWindowButton != null)
            {
                openSelectedLayerWindowButton.IsEnabled = actionState.CanOpen;
                openSelectedLayerWindowButton.ToolTip = OpenVisionLanguageService.T("Shell.OpenLayerWindow");
            }

            if (dockSelectedLayerButton != null)
            {
                dockSelectedLayerButton.IsEnabled = actionState.CanDock;
                dockSelectedLayerButton.ToolTip = OpenVisionLanguageService.T("Shell.DockLayer");
            }

            if (clearDockedLayersButton != null)
            {
                clearDockedLayersButton.IsEnabled = actionState.CanClearDocked;
                clearDockedLayersButton.ToolTip = OpenVisionLanguageService.T("Shell.ClearDockedLayers");
            }
        }

        public void ApplyDockedLayerRefreshResult(OpenVisionDockDocumentRefreshResult result)
        {
            if (result == null)
            {
                return;
            }

            ShowWorkspaceForImageState(result.HasDocuments);

            if (workspaceStatusText == null)
            {
                return;
            }

            if (!result.HasDocuments)
            {
                workspaceStatusText.Text = string.IsNullOrWhiteSpace(selectedLayerRouteText?.Text)
                    ? OpenVisionLanguageService.T("Shell.LayerDetailPending")
                    : selectedLayerRouteText.Text;
                return;
            }

            workspaceStatusText.Text = string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T("Shell.DockedLayerStatusFormat"),
                result.DocumentCount);
        }

        public void ApplyWorkspacePointerStatus(OpenVisionZoomableImageStatus status)
        {
            if (workspaceCoordinatesText == null || workspacePixelText == null)
            {
                return;
            }

            if (status?.HasPixel == true)
            {
                workspaceCoordinatesText.Text = status.FormatCoordinates();
                workspacePixelText.Text = status.FormatPixel();
                return;
            }

            workspaceCoordinatesText.Text = "X:- Y:-";
            workspacePixelText.Text = "GV - | RGB -";
        }

        private void ApplyWorkspaceFallbackImage(OpenVisionShellHostLayerDetailState detail)
        {
            // Keep fallback data available for diagnostics, but do not cover the interactive OpenGL viewer.
            if (workspaceFallbackImage == null)
            {
                return;
            }

            if (detail?.HasImage == true)
            {
                workspaceFallbackImage.Source = OpenVisionBitmapImagePreviewFactory.Create(detail.Image);
                workspaceFallbackImage.Visibility = Visibility.Visible;
                fallbackZoomController.SetStatusBitmap(detail.Image);
                fallbackZoomController.Reset();
                return;
            }

            workspaceFallbackImage.Source = null;
            workspaceFallbackImage.Visibility = Visibility.Collapsed;
            fallbackZoomController.SetStatusBitmap(null);
            fallbackZoomController.Reset();
        }

        private void ShowWorkspaceForImageState(bool hasImageOrDocument)
        {
            if (hasImageOrDocument)
            {
                ShowDockedWorkspace();
                return;
            }

            ShowSingleWorkspace();
        }

        private void ShowSingleWorkspace()
        {
            if (singleWorkspaceView != null)
            {
                singleWorkspaceView.Visibility = Visibility.Visible;
            }

            if (dockedWorkspaceView != null)
            {
                dockedWorkspaceView.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowDockedWorkspace()
        {
            if (singleWorkspaceView != null)
            {
                singleWorkspaceView.Visibility = Visibility.Collapsed;
            }

            if (dockedWorkspaceView != null)
            {
                dockedWorkspaceView.Visibility = Visibility.Visible;
            }
        }

        private static string DisplayText(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? "-" : text;
        }
    }
}
