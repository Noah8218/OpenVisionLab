using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockDocumentStateController : IOpenVisionDockDocumentState
    {
        private readonly List<string> documentIds = new List<string>();
        private readonly OpenVisionDockDocumentStateStore stateStore;

        public OpenVisionDockDocumentStateController(OpenVisionDockDocumentStateStore stateStore)
        {
            this.stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
        }

        public ICollection<string> LayerTitles => documentIds;

        public int Count => documentIds.Count;

        public bool HasLayers => documentIds.Count > 0;

        public void EnsureStateLoaded()
        {
            stateStore.EnsureLoaded();
        }

        public bool Contains(string documentId)
        {
            return !string.IsNullOrWhiteSpace(documentId)
                && documentIds.Contains(documentId, StringComparer.OrdinalIgnoreCase);
        }

        public bool Add(string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId) || Contains(documentId))
            {
                return false;
            }

            documentIds.Add(documentId);
            return true;
        }

        public bool Remove(string documentId)
        {
            return documentIds.RemoveAll(item => string.Equals(item, documentId, StringComparison.OrdinalIgnoreCase)) > 0;
        }

        public void Clear()
        {
            documentIds.Clear();
        }

        public List<string> ResolveTargetTitles(params string[] requestedDocumentIds)
        {
            IEnumerable<string> source = requestedDocumentIds == null || requestedDocumentIds.Length == 0
                ? documentIds
                : requestedDocumentIds.Where(id => !string.IsNullOrWhiteSpace(id));

            return source
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public bool ApplyPersistedLayers(Func<string, bool> canOpenDocument)
        {
            stateStore.EnsureLoaded();
            IReadOnlyList<string> persistedDocumentIds = stateStore.LoadedDocumentIds;
            if (persistedDocumentIds.Count == 0)
            {
                return false;
            }

            bool changed = false;
            foreach (string documentId in persistedDocumentIds)
            {
                if (Contains(documentId) || canOpenDocument?.Invoke(documentId) != true)
                {
                    continue;
                }

                // Persisted document ids are only state; actual dock documents are rebuilt by the workspace adapter.
                documentIds.Add(documentId);
                changed = true;
            }

            stateStore.ClearLoadedDocumentIds();
            return changed;
        }

        public void SaveLayerState(Action saveLayoutState, bool preservePendingPersistedState = false)
        {
            if (preservePendingPersistedState)
            {
                stateStore.EnsureLoaded();
                if (documentIds.Count == 0 && stateStore.HasLoadedDocumentIds)
                {
                    return;
                }
            }

            stateStore.SaveDocumentIds(documentIds);
            if (documentIds.Count > 0)
            {
                saveLayoutState?.Invoke();
            }
        }

        public IReadOnlyList<OpenVisionDockDocumentLayoutEntry> LoadPaneLayout(ICollection<string> activeDocumentIds)
        {
            return stateStore.LoadPaneLayout(activeDocumentIds);
        }

        public void SavePaneLayout(IEnumerable<OpenVisionDockDocumentLayoutEntry> paneLayout)
        {
            stateStore.SavePaneLayout(paneLayout);
        }
    }
}
