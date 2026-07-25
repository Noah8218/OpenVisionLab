using Lib.OpenCV;
using Lib.OpenCV.Property;
using OpenCvSharp;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    [XmlRoot("ThresholdToolSettings")]
    public sealed class ThresholdToolSettings
    {
        public ThresholdToolMode Mode { get; set; } = ThresholdToolMode.Threshold;
        public double Threshold { get; set; } = 127;
        public double MaxValue { get; set; } = 255;
        public bool BasicInvert { get; set; }
        public int RangeMin { get; set; } = 1;
        public int RangeMax { get; set; } = 255;
        public bool RangeInvert { get; set; }
        public bool AdaptiveGaussian { get; set; }
        public bool AdaptiveInvert { get; set; }
        public double AdaptiveMaxValue { get; set; } = 255;
        public int BlockSize { get; set; } = 25;
        public int Weight { get; set; } = 5;
    }

    [XmlRoot("FilterToolSettings")]
    public sealed class FilterToolSettings
    {
        public FilterToolType FilterType { get; set; } = FilterToolType.Blur;
        public int KernelWidth { get; set; } = 3;
        public int KernelHeight { get; set; } = 3;
        public int MedianKernelSize { get; set; } = 3;
        public int Diameter { get; set; } = 3;
        public int SigmaColor { get; set; } = 3;
        public int SigmaSpace { get; set; } = 3;
        public BorderTypes BorderType { get; set; } = BorderTypes.Reflect101;
    }

    [XmlRoot("MorphologyToolSettings")]
    public sealed class MorphologyToolSettings
    {
        public MorphTypes Operator { get; set; } = MorphTypes.Erode;
        public MorphShapes Shape { get; set; } = MorphShapes.Rect;
        public int KernelWidth { get; set; } = 3;
        public int KernelHeight { get; set; } = 3;
        public int Iterations { get; set; } = 1;
    }

    [XmlRoot("SimplePreprocessToolSettings")]
    public sealed class SimplePreprocessToolSettings
    {
        public List<ToolParameterValue> Parameters { get; set; } = new List<ToolParameterValue>();
    }

    [XmlRoot("ArithmeticToolSettings")]
    public sealed class ArithmeticToolSettings
    {
        public string SelectedOperation { get; set; } = "Bitwise_AND";
        public bool UseConstantInput { get; set; }
        public bool UseColorConstant { get; set; }
        public bool UseOffsetMode { get; set; }
        public int Gray { get; set; } = 1;
        public int B { get; set; } = 1;
        public int G { get; set; } = 1;
        public int R { get; set; } = 1;
        public int OffsetX { get; set; } = 1;
        public int OffsetY { get; set; } = 1;
    }

    public sealed class ToolParameterValue
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
