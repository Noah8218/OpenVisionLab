using Lib.OpenCV;
using Lib.OpenCV.Property;
using OpenCvSharp;
using System;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeSimplePreprocessPropertyFactory
    {
        // PropertyGrid-backed tool models are created here to keep pipeline and preview paths on the same parameter mapping.
        public static EdgeDetectionToolProperty CreateEdgeDetectionProperty(SimplePreprocessToolWpfView view)
        {
            return new EdgeDetectionToolProperty
            {
                EdgeType = view.GetEnum("EdgeType", EdgeDetectionToolType.Canny),
                CannyThresholdLow = view.GetInt("CannyThresholdLow", 100),
                CannyThresholdHigh = view.GetInt("CannyThresholdHigh", 200),
                CannyApertureSize = view.GetInt("CannyApertureSize", 3),
                UseL2Gradient = view.GetBool("UseL2Gradient", true),
                SobelDegreeX = view.GetInt("SobelDegreeX", 1),
                SobelDegreeY = view.GetInt("SobelDegreeY", 0),
                SobelKernelSize = view.GetInt("SobelKernelSize", 3),
                ScharrDegreeX = view.GetInt("ScharrDegreeX", 1),
                ScharrDegreeY = view.GetInt("ScharrDegreeY", 0),
                LaplacianKernelSize = view.GetInt("LaplacianKernelSize", 3)
            };
        }

        public static RotateScaleToolProperty CreateRotateScaleProperty(SimplePreprocessToolWpfView view)
        {
            return new RotateScaleToolProperty
            {
                Angle = view.GetDouble("Angle", 0),
                ScaleXPercent = view.GetDouble("ScaleXPercent", 100),
                ScaleYPercent = view.GetDouble("ScaleYPercent", 100),
                Interpolation = InterpolationFlags.Linear,
                BorderType = BorderTypes.Constant
            };
        }

        public static MeanProperty CreateMeanProperty(SimplePreprocessToolWpfView view)
        {
            MeanProperty property = new MeanProperty("Mean")
            {
                MEAN_TYPES = view.GetEnum("MeanType", MeanType.Mean)
            };
            int min = view.GetInt("MeanMin", property.MEAN_MIN);
            int max = view.GetInt("MeanMax", property.MEAN_MAX);
            property.MEAN_MIN = Math.Min(min, max);
            property.MEAN_MAX = Math.Max(min, max);
            return property;
        }
    }
}
