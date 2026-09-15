using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockDocumentSynchronizationController
    {
        private readonly IOpenVisionDockDocumentState documentState;
        private readonly OpenVisionDockDocumentOrchestrator documentOrchestrator;
        private readonly Func<string, bool> canOpenDocument;
        private readonly Action refreshLayout;

        public OpenVisionDockDocumentSynchronizationController(
            IOpenVisionDockDocumentState documentState,
            OpenVisionDockDocumentOrchestrator documentOrchestrator,
            Func<string, bool> canOpenDocument,
            Action refreshLayout)
        {
            this.documentState = documentState ?? throw new ArgumentNullException(nameof(documentState));
            this.documentOrchestrator = documentOrchestrator ?? throw new ArgumentNullException(nameof(documentOrchestrator));
            this.canOpenDocument = canOpenDocument ?? throw new ArgumentNullException(nameof(canOpenDocument));
            this.refreshLayout = refreshLayout ?? throw new ArgumentNullException(nameof(refreshLayout));
        }

        public bool SyncDocuments(IReadOnlyList<string> documentIds)
        {
            List<string> targetIds = (documentIds ?? Array.Empty<string>())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Where(canOpenDocument)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            bool changed = false;
            HashSet<string> targetSet = new HashSet<string>(targetIds, StringComparer.OrdinalIgnoreCase);
            foreach (string existingId in documentState.LayerTitles.ToList())
            {
                if (!targetSet.Contains(existingId))
                {
                    changed |= documentState.Remove(existingId);
                }
            }

            foreach (string targetId in targetIds)
            {
                if (!documentState.Contains(targetId))
                {
                    changed |= documentState.Add(targetId);
                }
            }

            documentOrchestrator.RefreshDocuments();
            if (!changed)
            {
                return false;
            }

            refreshLayout();
            documentOrchestrator.SaveWorkspaceState();
            return true;
        }
    }
}
