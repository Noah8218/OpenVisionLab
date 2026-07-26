using Lib.OpenCV;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid;
using static Lib.Common.FormulaUtil;
using static OpenVisionLab.PropertyGridEditorFactory;
using static OpenVisionLab.VisionPipelineStepPropertyMapper;

namespace OpenVisionLab
{
    internal static class VisionPipelineBasicImagePropertyAdapter
    {
        public static bool TryCreateProperty(
            VisionPipelineStep step,
            string name,
            out object property)
        {
            property = null;
            switch (NormalizeToolType(step?.ToolType))
            {
                case "threshold":
                    property = AttachStepMetadata(new PipelineThresholdToolProperty
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
                    return true;
                case "morphology":
                    property = AttachStepMetadata(new PipelineMorphologyToolProperty
                    {
                        Shape = GetEnum(step.Parameters, nameof(MorphologyToolProperty.Shape), MorphShapes.Rect),
                        Operator = GetEnum(step.Parameters, nameof(MorphologyToolProperty.Operator), MorphTypes.Erode),
                        KernelWidth = GetInt(step.Parameters, nameof(MorphologyToolProperty.KernelWidth), 3),
                        KernelHeight = GetInt(step.Parameters, nameof(MorphologyToolProperty.KernelHeight), 3),
                        Iterations = GetInt(step.Parameters, nameof(MorphologyToolProperty.Iterations), 1)
                    }, name, step.InputLayer, step.OutputLayer);
                    return true;
                case "filter":
                    property = AttachStepMetadata(new PipelineFilterToolProperty
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
                    return true;
                case "edgedetection":
                case "edge":
                    property = AttachStepMetadata(new PipelineEdgeDetectionToolProperty
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
                    return true;
                default:
                    return false;
            }
        }

        public static bool TryCreateStep(
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

        public static string ResolveMetricToolType(object property)
        {
            if (property is PipelineThresholdToolProperty)
            {
                return "Threshold";
            }

            if (property is PipelineMorphologyToolProperty)
            {
                return "Morphology";
            }

            if (property is PipelineFilterToolProperty)
            {
                return "Filter";
            }

            return property is PipelineEdgeDetectionToolProperty
                ? "EdgeDetection"
                : string.Empty;
        }

        private static T AttachStepMetadata<T>(
            T property,
            string name,
            string inputLayer,
            string outputLayer)
            where T : VisionPipelineStepPropertyMapper.IPipelineStepMetadata
        {
            property.PipelineStepName = string.IsNullOrWhiteSpace(name)
                ? property.PipelineStepName
                : name;
            property.InputLayer = string.IsNullOrWhiteSpace(inputLayer) ? "Main" : inputLayer;
            property.OutputLayer = string.IsNullOrWhiteSpace(outputLayer)
                ? "Pipeline_Output"
                : outputLayer;
            return property;
        }

        private static string GetPropertyName(object property, string fallback)
        {
            if (property is VisionPipelineStepPropertyMapper.IPipelineStepMetadata metadata)
            {
                return string.IsNullOrWhiteSpace(metadata.PipelineStepName)
                    ? fallback
                    : metadata.PipelineStepName;
            }

            return fallback;
        }

        private static string NormalizeToolType(string toolType)
        {
            string value = (toolType ?? string.Empty).Trim();
            if (value.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 4);
            }

            return value.Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .ToLowerInvariant();
        }
        [CategoryOrder("Step", -1)]
        [CategoryOrder("Acceptance", 20)]
        [CategoryOrder("Threshold", 0)]
        [CategoryOrder("Range", 1)]
        [CategoryOrder("Adaptive Threshold", 2)]
        private sealed class PipelineThresholdToolProperty : ThresholdToolProperty, IPipelineStepMetadata
        {
            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string NAME { get; set; } = "Threshold";

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [Description("Layer image used as this step input. Linked steps normally use the previous step output.")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [Description("Layer name that stores this step result. Use a unique name when the result must be reviewed later.")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Threshold")]
            [DisplayName("Mode")]
            [Description("Threshold uses one gray value, Range uses lower and upper gray limits, Adaptive calculates a local threshold.")]
            public new ThresholdToolMode Mode
            {
                get => base.Mode;
                set => base.Mode = value;
            }

            [PropertyOrder(1)]
            [PropertyEditor(typeof(WpgThresholdEditor))]
            [ThresholdEditor(0, 255, 1, 0, nameof(Invert))]
            [NumberRange(0, 255, 1, 0)]
            [Category("Threshold")]
            [DisplayName("Threshold")]
            [Description("Single threshold value. Pixels are classified by this gray level and the selected threshold type.")]
            public new double Threshold
            {
                get => base.Threshold;
                set => base.Threshold = value;
            }

            [PropertyOrder(2)]
            [PropertyEditor(typeof(WpgDoubleEditor))]
            [NumberRange(0, 255, 1, 0)]
            [Category("Threshold")]
            [DisplayName("Max value")]
            public new double MaxValue
            {
                get => base.MaxValue;
                set => base.MaxValue = value;
            }

            [PropertyOrder(3)]
            [Category("Threshold")]
            [DisplayName("Threshold type")]
            public new ThresholdTypes ThresholdType
            {
                get => base.ThresholdType;
                set => base.ThresholdType = value;
            }

            [PropertyOrder(0)]
            [PropertyEditor(typeof(WpgRangeEditor))]
            [RangeEditor(0, 255, 1, 0, nameof(RangeMin), nameof(RangeMax), nameof(Invert))]
            [Category("Range")]
            [DisplayName("Range min")]
            [Description("Combined range threshold. Adjust Min and Max together; Invert selects pixels outside the range.")]
            public new int RangeMin
            {
                get => base.RangeMin;
                set => base.RangeMin = value;
            }

            [PropertyOrder(1)]
            [Browsable(false)]
            [Category("Range")]
            [DisplayName("Range max")]
            public new int RangeMax
            {
                get => base.RangeMax;
                set => base.RangeMax = value;
            }

            [PropertyOrder(2)]
            [Browsable(false)]
            [Category("Range")]
            [DisplayName("Invert")]
            public new bool Invert
            {
                get => base.Invert;
                set => base.Invert = value;
            }

            [PropertyOrder(0)]
            [Category("Adaptive Threshold")]
            [DisplayName("Algorithm")]
            [Description("Adaptive threshold algorithm. MeanC is stable for broad lighting changes; GaussianC gives more local weighting.")]
            public new AdaptiveThresholdTypes AdaptiveType
            {
                get => base.AdaptiveType;
                set => base.AdaptiveType = value;
            }

            [PropertyOrder(1)]
            [Category("Adaptive Threshold")]
            [DisplayName("Threshold type")]
            public new ThresholdTypes AdaptiveThresholdType
            {
                get => base.AdaptiveThresholdType;
                set => base.AdaptiveThresholdType = value;
            }

            [PropertyOrder(2)]
            [Category("Adaptive Threshold")]
            [DisplayName("Block size")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(3, 255, 2, 0)]
            [Description("Adaptive window size. Use odd values; larger windows are smoother and slower.")]
            public new int BlockSize
            {
                get => base.BlockSize;
                set => base.BlockSize = value;
            }

            [PropertyOrder(3)]
            [Category("Adaptive Threshold")]
            [DisplayName("Weight")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(-50, 50, 1, 0)]
            [Description("Adaptive correction value. Positive values make the result stricter.")]
            public new int Weight
            {
                get => base.Weight;
                set => base.Weight = value;
            }

            [PropertyOrder(1)]
            [Category("Acceptance")]
            [DisplayName("Use Acceptance")]
            public bool UseAcceptance { get; set; }

            [PropertyOrder(2)]
            [Category("Acceptance")]
            [DisplayName("Expected Success")]
            public bool ExpectedSuccess { get; set; } = true;

            [PropertyOrder(3)]
            [Category("Acceptance")]
            [DisplayName("Max Elapsed (ms)")]
            public double MaxElapsedMilliseconds { get; set; }

            [PropertyOrder(4)]
            [Category("Acceptance")]
            [DisplayName("Required Message")]
            public string RequiredMessageText { get; set; } = string.Empty;

            [PropertyOrder(5)]
            [Category("Acceptance")]
            [DisplayName("Acceptance Metric")]
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Acceptance", 20)]
        [CategoryOrder("Morphology", 0)]
        private sealed class PipelineMorphologyToolProperty : MorphologyToolProperty, IPipelineStepMetadata
        {
            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string NAME { get; set; } = "Morphology";

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Morphology")]
            [DisplayName("Shape")]
            public new MorphShapes Shape
            {
                get => base.Shape;
                set => base.Shape = value;
            }

            [PropertyOrder(1)]
            [Category("Morphology")]
            [DisplayName("Operator")]
            public new MorphTypes Operator
            {
                get => base.Operator;
                set => base.Operator = value;
            }

            [PropertyOrder(2)]
            [Category("Morphology")]
            [DisplayName("Kernel width")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 99, 1, 0)]
            [Description("Morphology kernel width. Increase to connect nearby pixels or remove wider noise.")]
            public new int KernelWidth
            {
                get => base.KernelWidth;
                set => base.KernelWidth = value;
            }

            [PropertyOrder(3)]
            [Category("Morphology")]
            [DisplayName("Kernel height")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 99, 1, 0)]
            [Description("Morphology kernel height. Increase to connect vertical gaps or remove taller noise.")]
            public new int KernelHeight
            {
                get => base.KernelHeight;
                set => base.KernelHeight = value;
            }

            [PropertyOrder(4)]
            [Category("Morphology")]
            [DisplayName("Iterations")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 20, 1, 0)]
            [Description("Number of repeated morphology operations. Keep this low unless the preview proves it is needed.")]
            public new int Iterations
            {
                get => base.Iterations;
                set => base.Iterations = value;
            }

            [PropertyOrder(1)]
            [Category("Acceptance")]
            [DisplayName("Use Acceptance")]
            public bool UseAcceptance { get; set; }

            [PropertyOrder(2)]
            [Category("Acceptance")]
            [DisplayName("Expected Success")]
            public bool ExpectedSuccess { get; set; } = true;

            [PropertyOrder(3)]
            [Category("Acceptance")]
            [DisplayName("Max Elapsed (ms)")]
            public double MaxElapsedMilliseconds { get; set; }

            [PropertyOrder(4)]
            [Category("Acceptance")]
            [DisplayName("Required Message")]
            public string RequiredMessageText { get; set; } = string.Empty;

            [PropertyOrder(5)]
            [Category("Acceptance")]
            [DisplayName("Acceptance Metric")]
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Acceptance", 20)]
        [CategoryOrder("Filter", 0)]
        [CategoryOrder("Kernel", 1)]
        [CategoryOrder("Bilateral", 2)]
        private sealed class PipelineFilterToolProperty : FilterToolProperty, IPipelineStepMetadata
        {
            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string NAME { get; set; } = "Filter";

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Filter")]
            [DisplayName("Filter type")]
            public new FilterToolType FilterType
            {
                get => base.FilterType;
                set => base.FilterType = value;
            }

            [PropertyOrder(1)]
            [Category("Filter")]
            [DisplayName("Border type")]
            public new BorderTypes BorderType
            {
                get => base.BorderType;
                set => base.BorderType = value;
            }

            [PropertyOrder(0)]
            [Category("Kernel")]
            [DisplayName("Kernel width")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 99, 1, 0)]
            [Description("Filter kernel width used by blur-like filters.")]
            public new int KernelWidth
            {
                get => base.KernelWidth;
                set => base.KernelWidth = value;
            }

            [PropertyOrder(1)]
            [Category("Kernel")]
            [DisplayName("Kernel height")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 99, 1, 0)]
            [Description("Filter kernel height used by blur-like filters.")]
            public new int KernelHeight
            {
                get => base.KernelHeight;
                set => base.KernelHeight = value;
            }

            [PropertyOrder(2)]
            [Category("Kernel")]
            [DisplayName("Median kernel size")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(3, 99, 2, 0)]
            [Description("Median blur kernel size. Use odd values such as 3, 5, 7.")]
            public new int MedianKernelSize
            {
                get => base.MedianKernelSize;
                set => base.MedianKernelSize = value;
            }

            [PropertyOrder(0)]
            [Category("Bilateral")]
            [DisplayName("Diameter")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 99, 1, 0)]
            [Description("Bilateral filter neighborhood diameter.")]
            public new int Diameter
            {
                get => base.Diameter;
                set => base.Diameter = value;
            }

            [PropertyOrder(1)]
            [Category("Bilateral")]
            [DisplayName("Sigma color")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 255, 1, 0)]
            [Description("Bilateral color sigma. Larger values smooth across stronger intensity differences.")]
            public new int SigmaColor
            {
                get => base.SigmaColor;
                set => base.SigmaColor = value;
            }

            [PropertyOrder(2)]
            [Category("Bilateral")]
            [DisplayName("Sigma space")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 255, 1, 0)]
            [Description("Bilateral spatial sigma. Larger values use a wider spatial neighborhood.")]
            public new int SigmaSpace
            {
                get => base.SigmaSpace;
                set => base.SigmaSpace = value;
            }

            [PropertyOrder(1)]
            [Category("Acceptance")]
            [DisplayName("Use Acceptance")]
            public bool UseAcceptance { get; set; }

            [PropertyOrder(2)]
            [Category("Acceptance")]
            [DisplayName("Expected Success")]
            public bool ExpectedSuccess { get; set; } = true;

            [PropertyOrder(3)]
            [Category("Acceptance")]
            [DisplayName("Max Elapsed (ms)")]
            public double MaxElapsedMilliseconds { get; set; }

            [PropertyOrder(4)]
            [Category("Acceptance")]
            [DisplayName("Required Message")]
            public string RequiredMessageText { get; set; } = string.Empty;

            [PropertyOrder(5)]
            [Category("Acceptance")]
            [DisplayName("Acceptance Metric")]
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Acceptance", 20)]
        [CategoryOrder("Edge", 0)]
        [CategoryOrder("Canny", 1)]
        [CategoryOrder("Sobel", 2)]
        [CategoryOrder("Scharr", 3)]
        [CategoryOrder("Laplacian", 4)]
        private sealed class PipelineEdgeDetectionToolProperty : EdgeDetectionToolProperty, IPipelineStepMetadata
        {
            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string NAME { get; set; } = "EdgeDetection";

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "Pipeline_Output";

            [PropertyOrder(0)]
            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [PropertyOrder(0)]
            [Category("Edge")]
            [DisplayName("Edge type")]
            public new EdgeDetectionToolType EdgeType
            {
                get => base.EdgeType;
                set => base.EdgeType = value;
            }

            [PropertyOrder(0)]
            [Category("Canny")]
            [DisplayName("Low threshold")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 255, 1, 0)]
            public new int CannyThresholdLow
            {
                get => base.CannyThresholdLow;
                set => base.CannyThresholdLow = value;
            }

            [PropertyOrder(1)]
            [Category("Canny")]
            [DisplayName("High threshold")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 255, 1, 0)]
            public new int CannyThresholdHigh
            {
                get => base.CannyThresholdHigh;
                set => base.CannyThresholdHigh = value;
            }

            [PropertyOrder(2)]
            [Category("Canny")]
            [DisplayName("Aperture size")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(3, 7, 2, 0)]
            public new int CannyApertureSize
            {
                get => base.CannyApertureSize;
                set => base.CannyApertureSize = value;
            }

            [PropertyOrder(3)]
            [Category("Canny")]
            [DisplayName("Use L2 gradient")]
            public new bool UseL2Gradient
            {
                get => base.UseL2Gradient;
                set => base.UseL2Gradient = value;
            }

            [PropertyOrder(0)]
            [Category("Sobel")]
            [DisplayName("Degree X")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 2, 1, 0)]
            public new int SobelDegreeX
            {
                get => base.SobelDegreeX;
                set => base.SobelDegreeX = value;
            }

            [PropertyOrder(1)]
            [Category("Sobel")]
            [DisplayName("Degree Y")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 2, 1, 0)]
            public new int SobelDegreeY
            {
                get => base.SobelDegreeY;
                set => base.SobelDegreeY = value;
            }

            [PropertyOrder(2)]
            [Category("Sobel")]
            [DisplayName("Kernel size")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 31, 2, 0)]
            public new int SobelKernelSize
            {
                get => base.SobelKernelSize;
                set => base.SobelKernelSize = value;
            }

            [PropertyOrder(0)]
            [Category("Scharr")]
            [DisplayName("Degree X")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 1, 1, 0)]
            public new int ScharrDegreeX
            {
                get => base.ScharrDegreeX;
                set => base.ScharrDegreeX = value;
            }

            [PropertyOrder(1)]
            [Category("Scharr")]
            [DisplayName("Degree Y")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(0, 1, 1, 0)]
            public new int ScharrDegreeY
            {
                get => base.ScharrDegreeY;
                set => base.ScharrDegreeY = value;
            }

            [PropertyOrder(0)]
            [Category("Laplacian")]
            [DisplayName("Kernel size")]
            [PropertyEditor(typeof(WpgSliderEditor))]
            [NumberRange(1, 31, 2, 0)]
            public new int LaplacianKernelSize
            {
                get => base.LaplacianKernelSize;
                set => base.LaplacianKernelSize = value;
            }

            [PropertyOrder(1)]
            [Category("Acceptance")]
            [DisplayName("Use Acceptance")]
            public bool UseAcceptance { get; set; }

            [PropertyOrder(2)]
            [Category("Acceptance")]
            [DisplayName("Expected Success")]
            public bool ExpectedSuccess { get; set; } = true;

            [PropertyOrder(3)]
            [Category("Acceptance")]
            [DisplayName("Max Elapsed (ms)")]
            public double MaxElapsedMilliseconds { get; set; }

            [PropertyOrder(4)]
            [Category("Acceptance")]
            [DisplayName("Required Message")]
            public string RequiredMessageText { get; set; } = string.Empty;

            [PropertyOrder(5)]
            [Category("Acceptance")]
            [DisplayName("Acceptance Metric")]
            [TypeConverter(typeof(PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [PropertyOrder(6)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Min")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [PropertyOrder(7)]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [Category("Acceptance")]
            [DisplayName("Metric range")]
            public double AcceptanceMetricMinimum { get; set; }

            [PropertyOrder(8)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Use Metric Max")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [PropertyOrder(9)]
            [Browsable(false)]
            [Category("Acceptance")]
            [DisplayName("Metric Max")]
            public double AcceptanceMetricMaximum { get; set; }
        }
    }
}
