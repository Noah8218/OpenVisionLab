using System;

namespace OpenVisionLab
{
    public sealed class OpenVisionShellHostCommandSurfaces
    {
        public OpenVisionShellHostCommandSurfaces(
            OpenVisionShellHostLayerCommandSurface layerCommands,
            OpenVisionShellHostWorkspaceCommandSurface workspaceCommands)
        {
            LayerCommands = layerCommands ?? throw new ArgumentNullException(nameof(layerCommands));
            WorkspaceCommands = workspaceCommands ?? throw new ArgumentNullException(nameof(workspaceCommands));
        }

        public OpenVisionShellHostLayerCommandSurface LayerCommands { get; }

        public OpenVisionShellHostWorkspaceCommandSurface WorkspaceCommands { get; }
    }
}
