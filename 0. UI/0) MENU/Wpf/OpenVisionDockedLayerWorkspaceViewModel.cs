using OpenVisionLab.Docking.Controls;
using OpenVisionLab.Mvvm;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal sealed class OpenVisionDockedLayerWorkspaceViewModel : ObservableObject
    {
        private readonly IOpenVisionDockDocumentState documentState;

        public OpenVisionDockedLayerWorkspaceViewModel(IOpenVisionDockDocumentState documentState)
        {
            this.documentState = documentState ?? throw new ArgumentNullException(nameof(documentState));
        }

        public ICollection<string> LayerTitles => documentState.LayerTitles;

        public bool HasLayers => documentState.HasLayers;

        public int LayerCount => documentState.Count;

        public string LayerTitleSummary => string.Join("|", documentState.LayerTitles);

        public void RefreshDocumentState()
        {
            OnPropertyChanged(nameof(LayerTitles));
            OnPropertyChanged(nameof(HasLayers));
            OnPropertyChanged(nameof(LayerCount));
            OnPropertyChanged(nameof(LayerTitleSummary));
        }
    }
}
