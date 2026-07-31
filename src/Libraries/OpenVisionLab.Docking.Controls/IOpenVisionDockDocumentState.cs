using System;
using System.Collections.Generic;

namespace OpenVisionLab.Docking.Controls
{
    public interface IOpenVisionDockDocumentState
    {
        ICollection<string> LayerTitles { get; }

        int Count { get; }

        bool HasLayers { get; }

        bool Contains(string layerTitle);

        bool Add(string layerTitle);

        bool Remove(string layerTitle);

        void Clear();

        void EnsureStateLoaded();

        List<string> ResolveTargetTitles(params string[] requestedLayerTitles);

        bool ApplyPersistedLayers(Func<string, bool> canOpenLayer);

        void SaveLayerState(Action saveLayoutState, bool preservePendingPersistedState = false);

        IReadOnlyList<OpenVisionDockDocumentLayoutEntry> LoadPaneLayout(ICollection<string> activeLayerTitles);

        void SavePaneLayout(IEnumerable<OpenVisionDockDocumentLayoutEntry> paneLayout);
    }
}
