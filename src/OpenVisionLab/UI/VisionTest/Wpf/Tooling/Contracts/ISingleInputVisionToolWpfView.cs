using System;
using System.Collections.Generic;
using System.Drawing;

namespace OpenVisionLab
{
    public interface ISingleInputVisionToolWpfView
    {
        event EventHandler SourceLayerChanged;
        event EventHandler DestinationLayerChanged;
        event EventHandler InputPreviewClicked;
        event EventHandler OutputPreviewClicked;
        event EventHandler CreateOutputLayerRequested;
        event EventHandler RunPreviewRequested;
        event EventHandler AddPipelineRequested;

        string SelectedInputLayer { get; }
        string SelectedOutputLayer { get; }

        void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayer, string selectedOutputLayer);
        void SetInputPreview(Bitmap image);
        void SetOutputPreview(Bitmap image);
    }

    public interface ISingleInputPropertyVisionToolWpfView<TProperty> : ISingleInputVisionToolWpfView
    {
        TProperty CreateProperty();
    }
}
