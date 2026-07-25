using Lib.OpenCV;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using static Lib.Common.FormulaUtil;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    internal static partial class VisionPipelineStepPropertyMapper
    {
        private static object CreateBasicImageProperty(VisionPipelineStep step, string name, string toolType)
        {
            switch (toolType)
            {
                case "threshold":
                    return AttachStepMetadata(new PipelineThresholdToolProperty
                    {
                        Mode = GetEnum(step.Parameters, nameof(ThresholdToolProperty.Mode), ThresholdToolMode.Threshold),
                        Threshold = GetDouble(step.Parameters, nameof(ThresholdToolProperty.Threshold), 127),
                        MaxValue = GetDouble(step.Parameters, nameof(ThresholdToolProperty.MaxValue), 255),
                        ThresholdType = GetEnum(step.Parameters, nameof(ThresholdToolProperty.ThresholdType), ThresholdTypes.Binary),
                        RangeMin = GetInt(step.Parameters, nameof(ThresholdToolProperty.RangeMin), 1),
                        RangeMax = GetInt(step.Parameters, nameof(ThresholdToolProperty.RangeMax), 255),
                        Invert = GetBool(step.Parameters, nameof(ThresholdToolProperty.Invert), false),
                        AdaptiveType = GetEnum(step.Parameters, nameof(ThresholdToolProperty.AdaptiveType), AdaptiveThresholdTypes.MeanC),
                        AdaptiveThresholdType = GetEnum(step.Parameters, nameof(ThresholdToolProperty.AdaptiveThresholdType), ThresholdTypes.Binary),
                        BlockSize = GetInt(step.Parameters, nameof(ThresholdToolProperty.BlockSize), 25),
                        Weight = GetInt(step.Parameters, nameof(ThresholdToolProperty.Weight), 5)
                    }, name, step.InputLayer, step.OutputLayer);
                case "morphology":
                    return AttachStepMetadata(new PipelineMorphologyToolProperty
                    {
                        Shape = GetEnum(step.Parameters, nameof(MorphologyToolProperty.Shape), MorphShapes.Rect),
                        Operator = GetEnum(step.Parameters, nameof(MorphologyToolProperty.Operator), MorphTypes.Erode),
                        KernelWidth = GetInt(step.Parameters, nameof(MorphologyToolProperty.KernelWidth), 3),
                        KernelHeight = GetInt(step.Parameters, nameof(MorphologyToolProperty.KernelHeight), 3),
                        Iterations = GetInt(step.Parameters, nameof(MorphologyToolProperty.Iterations), 1)
                    }, name, step.InputLayer, step.OutputLayer);
                case "filter":
                    return AttachStepMetadata(new PipelineFilterToolProperty
                    {
                        FilterType = GetEnum(step.Parameters, nameof(FilterToolProperty.FilterType), FilterToolType.Blur),
                        KernelWidth = GetInt(step.Parameters, nameof(FilterToolProperty.KernelWidth), 3),
                        KernelHeight = GetInt(step.Parameters, nameof(FilterToolProperty.KernelHeight), 3),
                        MedianKernelSize = GetInt(step.Parameters, nameof(FilterToolProperty.MedianKernelSize), 3),
                        Diameter = GetInt(step.Parameters, nameof(FilterToolProperty.Diameter), 3),
                        SigmaColor = GetInt(step.Parameters, nameof(FilterToolProperty.SigmaColor), 3),
                        SigmaSpace = GetInt(step.Parameters, nameof(FilterToolProperty.SigmaSpace), 3),
                        BorderType = GetEnum(step.Parameters, nameof(FilterToolProperty.BorderType), BorderTypes.Reflect101)
                    }, name, step.InputLayer, step.OutputLayer);
                case "edgedetection":
                case "edge":
                    return AttachStepMetadata(new PipelineEdgeDetectionToolProperty
                    {
                        EdgeType = GetEnum(step.Parameters, nameof(EdgeDetectionToolProperty.EdgeType), EdgeDetectionToolType.Canny),
                        CannyThresholdLow = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.CannyThresholdLow), 100),
                        CannyThresholdHigh = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.CannyThresholdHigh), 200),
                        CannyApertureSize = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.CannyApertureSize), 3),
                        UseL2Gradient = GetBool(step.Parameters, nameof(EdgeDetectionToolProperty.UseL2Gradient), true),
                        SobelDegreeX = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.SobelDegreeX), 0),
                        SobelDegreeY = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.SobelDegreeY), 0),
                        SobelKernelSize = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.SobelKernelSize), 1),
                        ScharrDegreeX = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.ScharrDegreeX), 0),
                        ScharrDegreeY = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.ScharrDegreeY), 0),
                        LaplacianKernelSize = GetInt(step.Parameters, nameof(EdgeDetectionToolProperty.LaplacianKernelSize), 1)
                    }, name, step.InputLayer, step.OutputLayer);
                default:
                    return null;
            }
        }

        private static bool TryApplyBasicImageProperty(
            object property,
            string fallbackName,
            string inputLayer,
            string outputLayer,
            out VisionPipelineStep mapped)
        {
            mapped = null;
            if (property is ThresholdToolProperty threshold)
            {
                mapped = VisionPipelineStepBuilder.FromThresholdProperty(threshold, GetPropertyName(property, fallbackName), inputLayer, outputLayer);
            }
            else if (property is MorphologyToolProperty morphology)
            {
                mapped = VisionPipelineStepBuilder.FromMorphologyProperty(morphology, GetPropertyName(property, fallbackName), inputLayer, outputLayer);
            }
            else if (property is FilterToolProperty filter)
            {
                mapped = VisionPipelineStepBuilder.FromFilterProperty(filter, GetPropertyName(property, fallbackName), inputLayer, outputLayer);
            }
            else if (property is EdgeDetectionToolProperty edgeDetection)
            {
                mapped = VisionPipelineStepBuilder.FromEdgeDetectionProperty(edgeDetection, GetPropertyName(property, fallbackName), inputLayer, outputLayer);
            }

            return mapped != null;
        }
    }
}
