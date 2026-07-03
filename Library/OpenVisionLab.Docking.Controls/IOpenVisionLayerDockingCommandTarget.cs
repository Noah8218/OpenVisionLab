using System.Collections.Generic;
using System.Windows.Controls;

namespace OpenVisionLab.Docking.Controls
{
    public interface IOpenVisionLayerDockingCommandTarget
    {
        bool SplitToNewPane(string layerTitle, ICollection<string> layerTitles);

        bool MoveToPrimaryPane(string layerTitle, ICollection<string> layerTitles);

        bool MoveToOuterPane(string layerTitle, ICollection<string> layerTitles, Orientation orientation, bool insertBefore);

        bool MoveToPaneSide(
            string layerTitle,
            ICollection<string> layerTitles,
            OpenVisionDockPaneHandle requestedTargetPane,
            Orientation orientation,
            bool insertBefore);

        bool MoveToPane(string layerTitle, ICollection<string> layerTitles, OpenVisionDockPaneHandle requestedTargetPane);
    }
}
