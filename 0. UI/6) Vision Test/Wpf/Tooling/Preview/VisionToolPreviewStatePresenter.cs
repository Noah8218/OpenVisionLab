using System;
using System.Drawing;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal static class VisionToolPreviewStatePresenter
    {
        public static void SetImage(
            VisionToolInlinePreviewSlot previewSlot,
            Border previewFrame,
            Bitmap image,
            Action afterRefresh = null)
        {
            if (previewSlot == null)
            {
                return;
            }

            // Preview changes must update the OpenGL slot first, then refresh the WPF overlay state around it.
            previewSlot.SetImage(image);
            VisionToolPreviewSlotBehavior.Refresh(previewFrame);
            afterRefresh?.Invoke();
        }
    }
}