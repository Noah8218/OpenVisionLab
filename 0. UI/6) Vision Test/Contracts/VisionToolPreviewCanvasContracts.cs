using System;
using System.Drawing;
using System.Windows;

namespace OpenVisionLab.Contracts
{
    internal enum VisionToolPreviewOverlayKind
    {
        Unit,
        Align
    }

    internal interface IVisionToolOpenGlPreviewCanvas : IDisposable
    {
        FrameworkElement View { get; }

        int TextureTileCount { get; }

        void LoadImage(Bitmap image, string textureName);

        void ClearImage();

        void FitImageToView();

        void AddRectangleOverlay(
            string overlayId,
            string groupType,
            int imageHeight,
            int left,
            int top,
            int right,
            int bottom,
            bool isSelected,
            VisionToolPreviewOverlayKind overlayKind);

        void DeleteOverlay(string overlayId);

        void Refresh();
    }
}
