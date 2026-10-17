using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using System;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeThresholdPreviewExecutor
    {
        public static OpenVisionNativePreviewOperation Capture(ThresholdToolWpfView view)
        {
            view.Dispatcher.VerifyAccess();
            view.ClearSignalEvidence();
            ThresholdToolProperty property = view.CreateProperty();
            string inputLayer = view.SelectedInputLayer;
            VisionToolSignalEvidence evidence = null;
            return new OpenVisionNativePreviewOperation(
                source => Execute(source, property, inputLayer, out evidence),
                () =>
                {
                    if (evidence != null)
                    {
                        view.ShowSignalEvidence(evidence);
                    }
                });
        }

        private static VisionToolResult Execute(Mat source, ThresholdToolProperty property, string inputLayer, out VisionToolSignalEvidence evidence)
        {
            evidence = null;
            using ThresholdTool tool = new ThresholdTool();
            tool.SetProperty(property);
            VisionToolResult result = tool.Execute(source);
            try
            {
                if (result != null
                    && result.Success
                    && result.ResultImage != null
                    && !result.ResultImage.Empty()
                    && property.Mode != OpenVisionLab.Vision2D.ThresholdToolMode.Adaptive)
                {
                    evidence = OpenVisionNativeThresholdSignalEvidenceFactory.Create(source, result.ResultImage, property, inputLayer);
                }
                return result;
            }
            catch
            {
                result?.Dispose();
                throw;
            }
        }
    }
}
