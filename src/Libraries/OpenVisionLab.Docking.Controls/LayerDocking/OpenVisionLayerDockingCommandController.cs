using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionLayerDockingCommandController
    {
        private readonly IOpenVisionLayerDockingCommandTarget commandTarget;
        private readonly Func<ICollection<string>> workspaceLayerTitleProvider;
        private readonly Func<ICollection<string>> dockedLayerTitleProvider;

        public OpenVisionLayerDockingCommandController(
            IOpenVisionLayerDockingCommandTarget commandTarget,
            Func<ICollection<string>> workspaceLayerTitleProvider,
            Func<ICollection<string>> dockedLayerTitleProvider)
        {
            this.commandTarget = commandTarget ?? throw new ArgumentNullException(nameof(commandTarget));
            this.workspaceLayerTitleProvider = workspaceLayerTitleProvider ?? throw new ArgumentNullException(nameof(workspaceLayerTitleProvider));
            this.dockedLayerTitleProvider = dockedLayerTitleProvider ?? throw new ArgumentNullException(nameof(dockedLayerTitleProvider));
        }

        public bool SplitToNewPane(string layerTitle)
        {
            return commandTarget.SplitToNewPane(layerTitle, GetDockedLayerTitles());
        }

        public bool MoveToPrimaryPane(string layerTitle)
        {
            return commandTarget.MoveToPrimaryPane(layerTitle, GetDockedLayerTitles());
        }

        public bool DockToGuideZone(string layerTitle, DockingGuideZone zone, OpenVisionDockPaneHandle targetPane)
        {
            if (string.IsNullOrWhiteSpace(layerTitle))
            {
                return false;
            }

            return zone switch
            {
                DockingGuideZone.GlobalLeft => commandTarget.MoveToOuterPane(
                    layerTitle,
                    GetWorkspaceLayerTitles(),
                    Orientation.Horizontal,
                    insertBefore: true),
                DockingGuideZone.GlobalRight => commandTarget.MoveToOuterPane(
                    layerTitle,
                    GetWorkspaceLayerTitles(),
                    Orientation.Horizontal,
                    insertBefore: false),
                DockingGuideZone.GlobalTop => commandTarget.MoveToOuterPane(
                    layerTitle,
                    GetWorkspaceLayerTitles(),
                    Orientation.Vertical,
                    insertBefore: true),
                DockingGuideZone.GlobalBottom => commandTarget.MoveToOuterPane(
                    layerTitle,
                    GetWorkspaceLayerTitles(),
                    Orientation.Vertical,
                    insertBefore: false),
                DockingGuideZone.Left => commandTarget.MoveToPaneSide(
                    layerTitle,
                    GetWorkspaceLayerTitles(),
                    targetPane,
                    Orientation.Horizontal,
                    insertBefore: true),
                DockingGuideZone.Right => commandTarget.MoveToPaneSide(
                    layerTitle,
                    GetWorkspaceLayerTitles(),
                    targetPane,
                    Orientation.Horizontal,
                    insertBefore: false),
                DockingGuideZone.Top => commandTarget.MoveToPaneSide(
                    layerTitle,
                    GetWorkspaceLayerTitles(),
                    targetPane,
                    Orientation.Vertical,
                    insertBefore: true),
                DockingGuideZone.Bottom => commandTarget.MoveToPaneSide(
                    layerTitle,
                    GetWorkspaceLayerTitles(),
                    targetPane,
                    Orientation.Vertical,
                    insertBefore: false),
                _ => commandTarget.MoveToPane(layerTitle, GetWorkspaceLayerTitles(), targetPane)
            };
        }

        private ICollection<string> GetWorkspaceLayerTitles()
        {
            return workspaceLayerTitleProvider() ?? Array.Empty<string>();
        }

        private ICollection<string> GetDockedLayerTitles()
        {
            return dockedLayerTitleProvider() ?? Array.Empty<string>();
        }
    }
}
