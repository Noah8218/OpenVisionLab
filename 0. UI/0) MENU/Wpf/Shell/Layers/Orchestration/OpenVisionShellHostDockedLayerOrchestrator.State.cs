using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed partial class OpenVisionShellHostDockedLayerOrchestrator
    {
        public IOpenVisionDockedLayerViewer FindViewer(string layerTitle)
        {
            return composition.FindContent<IOpenVisionDockedLayerViewer>(layerTitle);
        }

        public void ResetLayoutToPrimaryPane()
        {
            composition.ResetLayoutToPrimaryPane();
        }

        public OpenVisionDockedLayerWorkspaceState GetWorkspaceState()
        {
            return composition.GetWorkspaceState();
        }

        public IEnumerable<FrameworkElement> EnumerateGestureHeaders()
        {
            return composition.EnumerateGestureHeaders();
        }

        public void EnsureStateLoaded()
        {
            composition.EnsureStateLoaded();
        }

        public bool ApplyPersistedLayers()
        {
            return composition.ApplyPersistedDocuments();
        }

        public bool RestoreWorkspaceState()
        {
            // Persisted docking is restored only through explicit commands/tests.
            // Loading or processing images must not create comparison panels behind the operator's back.
            ApplyPersistedLayers();
            RefreshViews();
            bool layoutRestored = composition.RestoreLayoutState();
            RefreshLayout();

            OpenVisionDockedLayerWorkspaceState state = GetWorkspaceState();
            bool restored = layoutRestored
                && composition.DocumentCount > 0
                && state.Documents.Sum(document => document.TextureTileCount) >= composition.DocumentCount;
            if (restored)
            {
                NotifyWorkspaceStateChanged();
            }

            return restored;
        }

        public void SaveWorkspaceState(bool preservePendingPersistedState = false)
        {
            composition.SaveWorkspaceState(preservePendingPersistedState);
        }

        public void StopPendingSave()
        {
            composition.StopPendingWorkspaceStateSave();
        }

        public List<string> GetWorkspaceLayerTitles()
        {
            return composition.GetDocumentIds();
        }
    }
}
