using OpenVisionLab.Core;
using OpenVisionLab.Docking.Controls;
using OpenVisionLab.ImageSpace.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OpenVisionLab
{
    internal sealed class OpenVisionDockedLayerContentSource : IOpenVisionDockDocumentContentSource
    {
        private readonly IDisplayManager displayManager;
        private readonly Func<List<string>> layerTitleProvider;
        private readonly Func<string> selectedLayerTitleProvider;
        private readonly Func<string, Bitmap, string> statusTextProvider;
        private readonly IOpenVisionDockedLayerViewerFactory viewerFactory;
        internal OpenVisionWorkbenchViewerViewModel MainViewer { get; set; }

        public OpenVisionDockedLayerContentSource(
            IDisplayManager displayManager,
            Func<List<string>> layerTitleProvider,
            Func<string> selectedLayerTitleProvider,
            Func<string, Bitmap, string> statusTextProvider,
            IOpenVisionDockedLayerViewerFactory viewerFactory)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.layerTitleProvider = layerTitleProvider ?? throw new ArgumentNullException(nameof(layerTitleProvider));
            this.selectedLayerTitleProvider = selectedLayerTitleProvider ?? throw new ArgumentNullException(nameof(selectedLayerTitleProvider));
            this.statusTextProvider = statusTextProvider ?? throw new ArgumentNullException(nameof(statusTextProvider));
            this.viewerFactory = viewerFactory ?? throw new ArgumentNullException(nameof(viewerFactory));
        }

        public string SelectedDocumentId => MainViewer?.ActivePaneId ?? selectedLayerTitleProvider() ?? string.Empty;

        public List<string> GetDocumentIds()
        {
            return MainViewer != null ? new List<string>(MainViewer.DocumentIds) : layerTitleProvider() ?? new List<string>();
        }

        public object UpdateDocumentContent(string documentId, object currentContent)
        {
            OpenVisionWorkbenchViewerPane pane = MainViewer?.FindPane(documentId);
            if (pane != null)
            {
                OpenVisionLayerViewerView paneView = currentContent as OpenVisionLayerViewerView ?? new OpenVisionLayerViewerView();
                paneView.SetCompactChrome(true);
                paneView.SetMainPane(pane);
                MainViewer.ReadImage(pane, image =>
                {
                    pane.UpdateImage(image);
                    paneView.UpdateMainPaneImage(image);
                });
                return paneView;
            }
            using ImageSpaceImageLease lease = displayManager.ImageSpace.AcquireImage(documentId);
            Bitmap image = lease?.Image;
            string statusText = BuildStatusText(documentId, image);
            IOpenVisionDockedLayerViewer viewer = currentContent as IOpenVisionDockedLayerViewer
                ?? viewerFactory.Create();

            viewer.SetCompactChrome(true);
            viewer.SetLayer(documentId, image, statusText);
            return viewer;
        }

        private string BuildStatusText(string layerTitle, Bitmap image)
        {
            return statusTextProvider(layerTitle, image);
        }
    }
}
