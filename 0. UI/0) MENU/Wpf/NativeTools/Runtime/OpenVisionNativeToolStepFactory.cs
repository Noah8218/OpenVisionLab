using Lib.OpenCV.Pipeline;
using OpenVisionLab.Contracts;
using OpenVisionLab._1._Core;
using System;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeToolStepFactory
    {
        // Keeps pipeline step creation separate from preview window state handling.
        public static VisionPipelineStep CreateLineGaugeStep(LineToolWpfView view, string inputLayer, string outputLayer)
        {
            if (string.Equals(view.SelectedPurpose, nameof(LineToolPurpose.Measure), StringComparison.Ordinal))
            {
                return VisionPipelineStepBuilder.FromLineGaugePair(
                    "LineDistance",
                    "LineDistance",
                    view.CreateLineAProperty(),
                    view.CreateLineBProperty(),
                    inputLayer,
                    outputLayer,
                    view.SelectedPurpose);
            }

            if (string.Equals(view.SelectedPurpose, nameof(LineToolPurpose.Intersection), StringComparison.Ordinal))
            {
                return VisionPipelineStepBuilder.FromLineGaugePair(
                    "LineIntersection",
                    "LineIntersection",
                    view.CreateLineAProperty(),
                    view.CreateLineBProperty(),
                    inputLayer,
                    outputLayer,
                    view.SelectedPurpose);
            }

            VisionPipelineStep step = VisionPipelineStepBuilder.FromProperty(
                view.CreateProperty(),
                inputLayer,
                outputLayer);
            step.Parameters[LineToolWpfView.LinePurposeParameterName] = view.SelectedPurpose;
            step.Parameters["LineSetting"] = view.SelectedLineName;
            return step;
        }
    }
}
