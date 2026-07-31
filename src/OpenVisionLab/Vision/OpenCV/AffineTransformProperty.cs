using Lib.OpenCV.Property;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    [CategoryOrder("Source Points", 0)]
    [CategoryOrder("Destination Points", 1)]
    [CategoryOrder("Output", 2)]
    [CategoryOrder("Sampling", 3)]
    [CategoryOrder("Validation Gates", 4)]
    [System.Xml.Serialization.XmlRoot("CPropertyAffineTransform")]
    public class AffineTransformProperty : OpenCvPropertyBase, IAffineTransformToolProperty, IOpenCvConfigurableProperty<AffineTransformProperty>
    {
        public AffineTransformProperty(string name)
            : base(name)
        {
            USE_THRESHOLD = false;
            USE_ROI = false;
            PIXELPERMM = 0d;
        }

        public AffineTransformProperty()
            : this("AffineTransform")
        {
        }

        [PropertyOrder(0), Category("Source Points"), DisplayName("Source point 1 X")]
        [Description("First source pixel X coordinate. Keep the three source points non-collinear.")]
        [NumberRange(-100000, 100000, 1, 3)]
        public double SourcePoint1X { get; set; }

        [PropertyOrder(1), Category("Source Points"), DisplayName("Source point 1 Y")]
        [NumberRange(-100000, 100000, 1, 3)]
        public double SourcePoint1Y { get; set; }

        [PropertyOrder(2), Category("Source Points"), DisplayName("Source point 2 X")]
        [NumberRange(-100000, 100000, 1, 3)]
        public double SourcePoint2X { get; set; } = 100d;

        [PropertyOrder(3), Category("Source Points"), DisplayName("Source point 2 Y")]
        [NumberRange(-100000, 100000, 1, 3)]
        public double SourcePoint2Y { get; set; }

        [PropertyOrder(4), Category("Source Points"), DisplayName("Source point 3 X")]
        [NumberRange(-100000, 100000, 1, 3)]
        public double SourcePoint3X { get; set; }

        [PropertyOrder(5), Category("Source Points"), DisplayName("Source point 3 Y")]
        [NumberRange(-100000, 100000, 1, 3)]
        public double SourcePoint3Y { get; set; } = 100d;

        [PropertyOrder(0), Category("Destination Points"), DisplayName("Destination point 1 X")]
        [Description("Output pixel X coordinate corresponding to source point 1.")]
        [NumberRange(-100000, 100000, 1, 3)]
        public double DestinationPoint1X { get; set; }

        [PropertyOrder(1), Category("Destination Points"), DisplayName("Destination point 1 Y")]
        [NumberRange(-100000, 100000, 1, 3)]
        public double DestinationPoint1Y { get; set; }

        [PropertyOrder(2), Category("Destination Points"), DisplayName("Destination point 2 X")]
        [NumberRange(-100000, 100000, 1, 3)]
        public double DestinationPoint2X { get; set; } = 100d;

        [PropertyOrder(3), Category("Destination Points"), DisplayName("Destination point 2 Y")]
        [NumberRange(-100000, 100000, 1, 3)]
        public double DestinationPoint2Y { get; set; }

        [PropertyOrder(4), Category("Destination Points"), DisplayName("Destination point 3 X")]
        [NumberRange(-100000, 100000, 1, 3)]
        public double DestinationPoint3X { get; set; }

        [PropertyOrder(5), Category("Destination Points"), DisplayName("Destination point 3 Y")]
        [NumberRange(-100000, 100000, 1, 3)]
        public double DestinationPoint3Y { get; set; } = 100d;

        [PropertyOrder(0), Category("Output"), DisplayName("Output width")]
        [Description("Output width in pixels. Use 0 to keep the input width.")]
        [NumberRange(0, 32768, 1, 0)]
        public int OutputWidth { get; set; }

        [PropertyOrder(1), Category("Output"), DisplayName("Output height")]
        [Description("Output height in pixels. Use 0 to keep the input height.")]
        [NumberRange(0, 32768, 1, 0)]
        public int OutputHeight { get; set; }

        [PropertyOrder(0), Category("Sampling"), DisplayName("Interpolation")]
        public InterpolationFlags Interpolation { get; set; } = InterpolationFlags.Linear;

        [PropertyOrder(1), Category("Sampling"), DisplayName("Border type")]
        public BorderTypes BorderType { get; set; } = BorderTypes.Constant;

        [PropertyOrder(2), Category("Sampling"), DisplayName("Border value")]
        [Description("Constant border gray/color channel value used when Border Type is Constant.")]
        [NumberRange(0, 255, 1, 3)]
        public double BorderValue { get; set; }

        [PropertyOrder(0), Category("Validation Gates"), DisplayName("Minimum source triangle area")]
        [Description("Reject source teaching whose three points form a smaller pixel area.")]
        [NumberRange(0, 1000000000, 1, 3)]
        public double MinimumSourceTriangleArea { get; set; } = 1d;

        [PropertyOrder(1), Category("Validation Gates"), DisplayName("Minimum destination triangle area")]
        [Description("Reject destination teaching whose three points form a smaller pixel area.")]
        [NumberRange(0, 1000000000, 1, 3)]
        public double MinimumDestinationTriangleArea { get; set; } = 1d;

        [PropertyOrder(2), Category("Validation Gates"), DisplayName("Minimum valid pixel ratio")]
        [Description("Reject a transform when the output canvas contains too little transformed source coverage (0..1).")]
        [NumberRange(0, 1, 0.01, 4)]
        public double MinimumValidPixelRatio { get; set; }

        public AffineTransformProperty DeepCopy() => (AffineTransformProperty)MemberwiseClone();

        public AffineTransformProperty LoadConfig(string recipeName) => LoadConfigFile<AffineTransformProperty>(recipeName);

        public AffineTransformProperty LoadTestConfig(string path) => LoadTestConfigFile<AffineTransformProperty>(path);
    }
}
