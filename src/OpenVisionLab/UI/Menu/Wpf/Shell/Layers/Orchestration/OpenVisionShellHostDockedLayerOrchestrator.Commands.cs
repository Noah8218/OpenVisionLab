using OpenVisionLab.Docking.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed partial class OpenVisionShellHostDockedLayerOrchestrator
    {
        public bool DockLayer(string layerTitle)
        {
            return composition.DockDocument(layerTitle);
        }

        public bool ActivateLayer(string layerTitle)
        {
            return composition.SelectDocument(layerTitle);
        }

        public void SyncLayers(IReadOnlyList<string> layerTitles)
        {
            composition.SyncDocuments(layerTitles);
        }

        public void ClearLayers()
        {
            composition.ClearDocuments();
        }

        public void RefreshViews()
        {
            composition.RefreshDocuments();
        }

        public void ClearDocuments()
        {
            composition.ClearDocumentContents();
        }

        public bool SplitToNewPane(string layerTitle)
        {
            return composition.SplitToNewPane(layerTitle);
        }

        public bool MoveToPrimaryPane(string layerTitle)
        {
            return composition.MoveToPrimaryPane(layerTitle);
        }

        public bool DockLayerToGuideZone(string layerTitle, DockingGuideZone zone)
        {
            if (string.IsNullOrWhiteSpace(layerTitle))
            {
                return false;
            }

            return composition.DockToPrimaryGuideZone(layerTitle, zone);
        }

        public bool DockLayerToGuideZone(string layerTitle, DockingGuideZone zone, OpenVisionDockPaneHandle targetPane)
        {
            if (string.IsNullOrWhiteSpace(layerTitle))
            {
                return false;
            }

            return composition.DockToGuideZone(layerTitle, zone, targetPane);
        }

        public bool ArrangePanes(Orientation orientation, params string[] layerTitles)
        {
            return composition.ArrangePanes(orientation, layerTitles);
        }

        public bool ArrangeGrid(params string[] layerTitles)
        {
            return composition.ArrangeGrid(layerTitles);
        }

        public void RefreshLayout()
        {
            composition.RefreshLayout();
        }
    }
}
