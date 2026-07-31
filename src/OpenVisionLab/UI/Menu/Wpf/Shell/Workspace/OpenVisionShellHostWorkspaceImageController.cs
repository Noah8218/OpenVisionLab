using OpenVisionLab.Core;
using OpenVisionLab.ImageSpace.Core;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostWorkspaceImageController
    {
        private readonly IDisplayManager displayManager;
        private readonly OpenVisionShellHostDocumentController documentController;
        private readonly Action setDirectRunPending;
        private readonly Action<string> refreshSelectedLayerDetail;
        private readonly Action refreshRows;
        private readonly Action refreshDirectRouteText;

        public OpenVisionShellHostWorkspaceImageController(
            IDisplayManager displayManager,
            OpenVisionShellHostDocumentController documentController,
            Action setDirectRunPending,
            Action<string> refreshSelectedLayerDetail,
            Action refreshRows,
            Action refreshDirectRouteText)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.documentController = documentController ?? throw new ArgumentNullException(nameof(documentController));
            this.setDirectRunPending = setDirectRunPending ?? throw new ArgumentNullException(nameof(setDirectRunPending));
            this.refreshSelectedLayerDetail = refreshSelectedLayerDetail ?? throw new ArgumentNullException(nameof(refreshSelectedLayerDetail));
            this.refreshRows = refreshRows ?? throw new ArgumentNullException(nameof(refreshRows));
            this.refreshDirectRouteText = refreshDirectRouteText ?? throw new ArgumentNullException(nameof(refreshDirectRouteText));
        }

        public bool LoadImage(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return false;
            }

            Bitmap image = new Bitmap(path);
            ApplyMainLayerImage(image, disposeAfterApply: true);
            return true;
        }

        public void ApplyMainLayerImage(Bitmap image, bool disposeAfterApply)
        {
            if (image == null)
            {
                return;
            }

            try
            {
                displayManager.CreateLayerDisplay(ImageSpaceFrame.FromBitmap(CloneBitmapForLayer(image)), "Main", false);
                displayManager.SelectedItem = "Main";
                displayManager.ActivateLayer("Main");
                OpenVisionNativeToolDocument activeDocument = documentController.ActiveNativeDocument;
                activeDocument?.InvalidatePreviewResultForInputChange();
                activeDocument?.RefreshLayerState();
                setDirectRunPending();
                refreshSelectedLayerDetail("Main");
                refreshRows();
                refreshDirectRouteText();
            }
            finally
            {
                if (disposeAfterApply)
                {
                    image.Dispose();
                }
            }
        }

        public static Bitmap CloneBitmapForLayer(Bitmap image)
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
                return new Bitmap(image);
            }
        }
    }
}
