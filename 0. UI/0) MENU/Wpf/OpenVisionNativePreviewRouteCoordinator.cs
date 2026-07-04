using OpenVisionLab._1._Core;
using System;
using System.Globalization;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativePreviewRouteCoordinator
    {
        private readonly IDisplayManager displayManager;
        private readonly Func<OpenVisionNativeToolDocument> getActiveNativeDocument;
        private readonly Func<OpenVisionPipelineReviewDocument> getActivePipelineReviewDocument;
        private readonly Func<string> getFallbackRouteText;
        private readonly Action<string> setRouteText;
        private readonly Action refreshHostLayerRows;
        private readonly Action<string> refreshHostSelectedLayerDetail;
        private readonly Action<string> activateDockedLayerDocument;
        private string lastVisibleNativeOutputLayer;

        public OpenVisionNativePreviewRouteCoordinator(
            IDisplayManager displayManager,
            Func<OpenVisionNativeToolDocument> getActiveNativeDocument,
            Func<OpenVisionPipelineReviewDocument> getActivePipelineReviewDocument,
            Func<string> getFallbackRouteText,
            Action<string> setRouteText,
            Action refreshHostLayerRows,
            Action<string> refreshHostSelectedLayerDetail,
            Action<string> activateDockedLayerDocument)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.getActiveNativeDocument = getActiveNativeDocument ?? throw new ArgumentNullException(nameof(getActiveNativeDocument));
            this.getActivePipelineReviewDocument = getActivePipelineReviewDocument ?? throw new ArgumentNullException(nameof(getActivePipelineReviewDocument));
            this.getFallbackRouteText = getFallbackRouteText ?? throw new ArgumentNullException(nameof(getFallbackRouteText));
            this.setRouteText = setRouteText ?? throw new ArgumentNullException(nameof(setRouteText));
            this.refreshHostLayerRows = refreshHostLayerRows ?? throw new ArgumentNullException(nameof(refreshHostLayerRows));
            this.refreshHostSelectedLayerDetail = refreshHostSelectedLayerDetail ?? throw new ArgumentNullException(nameof(refreshHostSelectedLayerDetail));
            this.activateDockedLayerDocument = activateDockedLayerDocument ?? (_ => { });
        }

        public void RefreshRouteText()
        {
            setRouteText(BuildRouteText());
        }

        public void RefreshAfterLayerStateChanged(bool hasPreviewResult)
        {
            RefreshRouteText();
            refreshHostLayerRows();
            if (hasPreviewResult)
            {
                RefreshNativeOutputWorkspacePreview();
            }
        }

        private string BuildRouteText()
        {
            if (getActivePipelineReviewDocument() != null)
            {
                return OpenVisionLanguageService.T("Shell.RoutePipelineReview");
            }

            OpenVisionNativeToolDocument nativeDocument = getActiveNativeDocument();
            string inputLayer = nativeDocument?.RouteInputLayerName;
            string outputLayer = nativeDocument?.RouteOutputLayerName;
            if (string.IsNullOrWhiteSpace(inputLayer) && string.IsNullOrWhiteSpace(outputLayer))
            {
                return getFallbackRouteText() ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(inputLayer))
            {
                inputLayer = OpenVisionLanguageService.T("Shell.RouteInputFallback");
            }

            if (string.IsNullOrWhiteSpace(outputLayer))
            {
                outputLayer = OpenVisionLanguageService.T("Shell.RouteOutputFallback");
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T("Shell.RoutePairFormat"),
                inputLayer,
                outputLayer);
        }

        private void RefreshNativeOutputWorkspacePreview()
        {
            string outputLayer = getActiveNativeDocument()?.RouteOutputLayerName;
            if (string.IsNullOrWhiteSpace(outputLayer) || displayManager.GetLayerImage(outputLayer) == null)
            {
                return;
            }

            lastVisibleNativeOutputLayer = outputLayer;
            RefreshNativeOutputWorkspacePreview(outputLayer);
        }

        public void RefreshLastVisibleNativeOutputWorkspacePreview()
        {
            if (string.IsNullOrWhiteSpace(lastVisibleNativeOutputLayer)
                || displayManager.GetLayerImage(lastVisibleNativeOutputLayer) == null)
            {
                return;
            }

            RefreshNativeOutputWorkspacePreview(lastVisibleNativeOutputLayer);
        }

        private void RefreshNativeOutputWorkspacePreview(string outputLayer)
        {
            // Tool previews must be visible without stealing the input route from the user-selected layer.
            refreshHostSelectedLayerDetail(outputLayer);
            activateDockedLayerDocument(outputLayer);
        }
    }
}
