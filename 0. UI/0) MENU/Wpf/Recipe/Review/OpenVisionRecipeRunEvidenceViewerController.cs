using MahApps.Metro.IconPacks;
using System;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeRunEvidenceViewerController
    {
        private readonly Func<Window> ownerProvider;
        private readonly OpenVisionLayerViewerWindowRegistry windowRegistry;

        public OpenVisionRecipeRunEvidenceViewerController(
            Func<Window> ownerProvider,
            OpenVisionLayerViewerWindowRegistry windowRegistry)
        {
            this.ownerProvider = ownerProvider ?? throw new ArgumentNullException(nameof(ownerProvider));
            this.windowRegistry = windowRegistry ?? throw new ArgumentNullException(nameof(windowRegistry));
        }

        public bool Open(OpenVisionRecipeRunEvidence evidence)
        {
            OpenVisionRecipeRunEvidenceViewerView viewer = new OpenVisionRecipeRunEvidenceViewerView();
            if (!viewer.TrySetEvidence(evidence))
            {
                viewer.Dispose();
                return false;
            }

            OpenVisionFloatingToolWindow window = new OpenVisionFloatingToolWindow(
                OpenVisionRecipeText.Local("배치 검출 도면", "Batch detection drawing"),
                viewer)
            {
                Width = 1420,
                Height = 760,
                MinWidth = 920,
                MinHeight = 520,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.CanResize,
                IsDockButtonVisible = false
            };
            window.SetTitleIcon(PackIconMaterialKind.ImageMultipleOutline);

            Window owner = ownerProvider();
            if (owner != null)
            {
                window.Owner = owner;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            windowRegistry.Add(window);
            window.Show();
            window.BringAboveOwnerAirspace();
            return true;
        }
    }
}
