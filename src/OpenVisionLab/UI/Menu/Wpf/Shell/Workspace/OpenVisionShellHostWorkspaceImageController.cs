using OpenVisionLab.Core;
using OpenVisionLab.ImageSpace.Core;
using OpenVisionLab.Logging;
using OpenVisionLab.Logging.Model;
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
        private readonly Action refreshRows;
        private readonly Action refreshCommandCanExecute;
        private readonly Action refreshDirectRouteText;

        public OpenVisionShellHostWorkspaceImageController(
            IDisplayManager displayManager,
            OpenVisionShellHostDocumentController documentController,
            Action setDirectRunPending,
            Action refreshRows,
            Action refreshCommandCanExecute,
            Action refreshDirectRouteText)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.documentController = documentController ?? throw new ArgumentNullException(nameof(documentController));
            this.setDirectRunPending = setDirectRunPending ?? throw new ArgumentNullException(nameof(setDirectRunPending));
            this.refreshRows = refreshRows ?? throw new ArgumentNullException(nameof(refreshRows));
            this.refreshCommandCanExecute = refreshCommandCanExecute ?? throw new ArgumentNullException(nameof(refreshCommandCanExecute));
            this.refreshDirectRouteText = refreshDirectRouteText ?? throw new ArgumentNullException(nameof(refreshDirectRouteText));
        }

        public bool LoadImage(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return false;
            }

            Bitmap image;
            try
            {
                image = new Bitmap(path);
            }
            catch (Exception ex)
            {
                OVLog.Write(
                    LogCategory.Main,
                    LogLevel.Error,
                    "Workspace image load failed.",
                    path,
                    ex);
                return false;
            }

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
                displayManager.CreateLayerDisplay(ImageSpaceFrame.Borrow(image), "Main", false);
                displayManager.SelectedItem = "Main";
                displayManager.ActivateLayer("Main");
                OpenVisionNativeToolDocument activeDocument = documentController.ActiveNativeDocument;
                activeDocument?.InvalidatePreviewResultForInputChange();
                activeDocument?.RefreshLayerState();
                documentController.RefreshPipelineReviewInputLayerState();
                setDirectRunPending();
                refreshRows();
                refreshCommandCanExecute();
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
