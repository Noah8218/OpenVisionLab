using OpenVisionLab.Docking.Controls;
using System;

namespace OpenVisionLab
{
    internal sealed partial class OpenVisionShellHostDockedLayerOrchestrator
    {
        private readonly OpenVisionDockWorkspaceComposition<OpenVisionDockedLayerDocumentState, OpenVisionDockedLayerWorkspaceState> composition;

        public event EventHandler WorkspaceStateChanged;

        public OpenVisionShellHostDockedLayerOrchestrator(
            OpenVisionDockWorkspaceComposition<OpenVisionDockedLayerDocumentState, OpenVisionDockedLayerWorkspaceState> composition)
        {
            this.composition = composition ?? throw new ArgumentNullException(nameof(composition));
            this.composition.WorkspaceStateChanged += OnCompositionWorkspaceStateChanged;
            this.composition.AttachGestureController();
        }

        private void NotifyWorkspaceStateChanged()
        {
            WorkspaceStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCompositionWorkspaceStateChanged(object sender, EventArgs e)
        {
            NotifyWorkspaceStateChanged();
        }
    }
}
