using System;
using System.Collections.Generic;
using System.Drawing;

namespace OpenVisionLab
{
    public interface IArithmeticVisionToolWpfView
    {
        event EventHandler InputALayerChanged;
        event EventHandler InputBLayerChanged;
        event EventHandler OutputLayerChanged;
        event EventHandler InputAPreviewClicked;
        event EventHandler InputBPreviewClicked;
        event EventHandler OutputPreviewClicked;
        event EventHandler CreateOutputLayerRequested;
        event EventHandler RunPreviewRequested;
        event EventHandler RunOffsetRequested;
        event EventHandler AddPipelineRequested;
        event EventHandler ParameterChanged;

        string SelectedInputLayerA { get; }
        string SelectedInputLayerB { get; }
        string SelectedOutputLayer { get; }
        string SelectedArithmeticType { get; }
        bool UseConstantInput { get; }
        bool UseColorConstant { get; }
        bool UseOffsetMode { get; }

        void SetOperationList(IEnumerable<string> operationNames, string selectedOperation);
        void SetLayerList(IEnumerable<string> layerNames, string selectedInputA, string selectedInputB, string selectedOutput);
        void SetInputAPreview(Bitmap image);
        void SetInputBPreview(Bitmap image);
        void SetOutputPreview(Bitmap image);
        void SetStatus(string status);
        int GetGrayValue(int fallback);
        int GetBValue(int fallback);
        int GetGValue(int fallback);
        int GetRValue(int fallback);
        int GetOffsetX(int fallback);
        int GetOffsetY(int fallback);
    }
}
