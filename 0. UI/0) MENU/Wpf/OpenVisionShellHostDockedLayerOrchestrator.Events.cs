using OpenVisionLab.Docking.Controls;
using System;

namespace OpenVisionLab
{
    internal sealed partial class OpenVisionShellHostDockedLayerOrchestrator
    {
        public void AttachLifecycle(IOpenVisionDockLifecycle lifecycle)
        {
            if (lifecycle == null)
            {
                throw new ArgumentNullException(nameof(lifecycle));
            }

            composition.AttachLifecycle(lifecycle);
        }
    }
}
