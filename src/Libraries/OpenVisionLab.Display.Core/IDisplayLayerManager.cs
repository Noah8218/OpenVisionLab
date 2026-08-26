using OpenVisionLab.ImageSpace.Core;
using System.Collections.Generic;

namespace OpenVisionLab.Core
{
    public interface IDisplayLayerManager
    {
        int LayerCount { get; }

        IReadOnlyList<DisplayLayerInfo> GetLayerInfos();
        string GetLayerTitle(int index);
        /// <summary>Consumes and disposes a non-null frame before returning.</summary>
        void CreatePanel(ImageSpaceFrame frame = null);
        int FindIndex(string title);
        int FindIndex();
        /// <summary>Consumes and disposes the frame before returning.</summary>
        void CreateLayerDisplay(ImageSpaceFrame frame, string title, bool useClose = true);
        void RefreshLayer(int index);
        void ActivateLayer(string title);
        void ActivateLayer(int index);
        void ZoomLayerToFit(string title);
        void ZoomLayerToFit(int index);
    }
}
