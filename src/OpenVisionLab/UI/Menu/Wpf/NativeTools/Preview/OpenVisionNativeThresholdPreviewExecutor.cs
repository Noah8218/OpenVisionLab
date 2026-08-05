using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using System;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeThresholdPreviewExecutor
    {
        public static VisionToolResult Execute(Mat source, ThresholdToolWpfView view)
        {
            view.ClearSignalEvidence();
            ThresholdToolProperty property = view.CreateProperty();
            ThresholdTool tool = new ThresholdTool();
            tool.SetProperty(property);
            VisionToolResult result = tool.Execute(source);
            if (result != null
                && result.Success
                && result.ResultImage != null
                && !result.ResultImage.Empty()
                && property.Mode != OpenVisionLab.Vision2D.ThresholdToolMode.Adaptive)
            {
                view.ShowSignalEvidence(OpenVisionNativeThresholdSignalEvidenceFactory.Create(
                    source,
                    result.ResultImage,
                    property,
                    view.SelectedInputLayer));
            }

            return result;
        }
    }
}
