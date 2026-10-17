using System;
using System.Drawing;

namespace OpenVisionLab
{
    internal interface IOpenVisionDockedLayerViewer
    {
        event EventHandler ZoomChanged;

        event EventHandler PointerStatusChanged;

        OpenVisionZoomableImageStatus PointerStatus { get; }

        bool HasImage { get; }

        double ZoomScale { get; }

        int TextureTileCount { get; }

        int ImagePixelWidth { get; }

        int ImagePixelHeight { get; }

        bool IsCompactSizeReady { get; }

        bool IsCompactChrome { get; }

        void SetCompactChrome(bool compact);

        void FitImageToView();

        void ZoomImage(double factor);

        void ShowOriginalSize();

        void SetLayer(string layerTitle, Bitmap image, string statusText);

        Bitmap CloneImageForTest();

        bool SaveImageToFileForTest(string path);
    }
}
