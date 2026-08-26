using System.Drawing;

namespace OpenVisionLab
{
    internal interface IOpenVisionDockedLayerViewer
    {
        int TextureTileCount { get; }

        int ImagePixelWidth { get; }

        int ImagePixelHeight { get; }

        bool IsCompactSizeReady { get; }

        bool IsCompactChrome { get; }

        void SetCompactChrome(bool compact);

        void SetLayer(string layerTitle, Bitmap image, string statusText);

        Bitmap CloneImageForTest();

        bool SaveImageToFileForTest(string path);
    }
}
