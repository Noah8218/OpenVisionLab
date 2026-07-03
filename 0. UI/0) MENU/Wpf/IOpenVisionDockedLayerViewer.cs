using System.Drawing;

namespace OpenVisionLab
{
    internal interface IOpenVisionDockedLayerViewer
    {
        int TextureTileCount { get; }

        bool IsCompactSizeReady { get; }

        bool IsCompactChrome { get; }

        void SetCompactChrome(bool compact);

        void SetLayer(string layerTitle, Bitmap image, string statusText);

        bool SaveImageToFileForTest(string path);
    }
}
