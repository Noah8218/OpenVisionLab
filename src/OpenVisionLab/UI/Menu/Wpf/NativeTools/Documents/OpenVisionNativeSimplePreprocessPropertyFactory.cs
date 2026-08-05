using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Property;
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
                EdgeType = view.Parameters.GetEnum("EdgeType", EdgeDetectionToolType.Canny),
                CannyThresholdLow = view.Parameters.GetInt("CannyThresholdLow", 100),
                CannyThresholdHigh = view.Parameters.GetInt("CannyThresholdHigh", 200),
                CannyApertureSize = view.Parameters.GetInt("CannyApertureSize", 3),
                UseL2Gradient = view.Parameters.GetBool("UseL2Gradient", true),
                SobelDegreeX = view.Parameters.GetInt("SobelDegreeX", 1),
                SobelDegreeY = view.Parameters.GetInt("SobelDegreeY", 0),
                SobelKernelSize = view.Parameters.GetInt("SobelKernelSize", 3),
                ScharrDegreeX = view.Parameters.GetInt("ScharrDegreeX", 1),
                ScharrDegreeY = view.Parameters.GetInt("ScharrDegreeY", 0),
                LaplacianKernelSize = view.Parameters.GetInt("LaplacianKernelSize", 3)
            };
        }

        public static RotateScaleToolProperty CreateRotateScaleProperty(SimplePreprocessToolWpfView view)
        {
            return new RotateScaleToolProperty
            {
                Angle = view.Parameters.GetDouble("Angle", 0),
                ScaleXPercent = view.Parameters.GetDouble("ScaleXPercent", 100),
                ScaleYPercent = view.Parameters.GetDouble("ScaleYPercent", 100),
                Interpolation = InterpolationFlags.Linear,
                BorderType = BorderTypes.Constant
            };
        }

        public static MeanProperty CreateMeanProperty(SimplePreprocessToolWpfView view)
        {
            MeanProperty property = new MeanProperty("Mean")
            {
                MEAN_TYPES = view.Parameters.GetEnum("MeanType", MeanType.Mean)
            };
            int min = view.Parameters.GetInt("MeanMin", property.MEAN_MIN);
            int max = view.Parameters.GetInt("MeanMax", property.MEAN_MAX);
            property.MEAN_MIN = Math.Min(min, max);
            property.MEAN_MAX = Math.Max(min, max);
            return property;
        }
    }
}
