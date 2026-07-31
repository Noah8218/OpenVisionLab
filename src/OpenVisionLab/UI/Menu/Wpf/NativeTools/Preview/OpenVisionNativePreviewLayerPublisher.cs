using OpenVisionLab.ImageSpace.Core;
using OpenVisionLab.Core;
using System;
using System.Drawing;
using System.Globalization;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativePreviewLayerPublisher
    {
        private readonly IDisplayManager displayManager;

        public OpenVisionNativePreviewLayerPublisher(IDisplayManager displayManager)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
        }

        public void PublishPreviewBitmap(string outputLayer, string activationLayer, Bitmap resultBitmap, TimeSpan elapsed)
        {
            if (resultBitmap == null)
            {
                return;
            }

            EnsureOutputLayer(outputLayer, resultBitmap.Width, resultBitmap.Height);
            int outputIndex = displayManager.FindIndex(outputLayer);
            // DisplayManager/ImageSpaceFrame keeps the Bitmap reference for the OpenGL viewer.
            // Preview bitmaps are often disposed by the caller after publishing, so publish an
            // owned clone to keep the central viewer and thumbnails in sync.
            displayManager.SetLayerImage(outputIndex, OpenVisionPreviewImageFileService.CloneBitmapPreservingPixelFormat(resultBitmap));
            displayManager.RefreshLayer(outputIndex);
            RestoreDisplayActivation(activationLayer);
            displayManager.TackTime = elapsed.TotalSeconds.ToString("0.000s", CultureInfo.InvariantCulture);
        }

        public void EnsureOutputLayer(string title, int width, int height)
        {
            if (displayManager.FindIndex(title) >= 0)
            {
                return;
            }

            Bitmap placeholder = CreatePlaceholderBitmap(width, height, title);
            displayManager.CreateLayerDisplay(ImageSpaceFrame.FromBitmap(placeholder), title, true);
        }

        public void RestoreDisplayActivation(string layerName)
        {
            if (string.IsNullOrWhiteSpace(layerName) || displayManager.FindIndex(layerName) < 0)
            {
                return;
            }

            // Output creation/publish makes DisplayManager focus the new layer internally.
            // Restore the route input so output layers never become implicit inputs.
            displayManager.SelectedItem = layerName;
            displayManager.ActivateLayer(layerName);
        }

        private static Bitmap CreatePlaceholderBitmap(int width, int height, string title)
        {
            Bitmap bitmap = new Bitmap(Math.Max(1, width), Math.Max(1, height));
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Black);
            using Brush brush = new SolidBrush(Color.FromArgb(72, 96, 104));
            using Font font = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, GraphicsUnit.Point);
            graphics.DrawString(title, font, brush, 12, 12);
            return bitmap;
        }
    }
}
