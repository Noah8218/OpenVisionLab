using Lib.OpenCV.Pipeline;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls.WpfPropertyGrid;
using static OpenVisionLab.PropertyGridEditorFactory;
using static OpenVisionLab.VisionPipelineStepPropertyMapper;

namespace OpenVisionLab
{
    internal static class VisionPipelineGeometryPropertyAdapter
    {
        public static bool TryCreateProperty(
            VisionPipelineStep step,
            string name,
            VisionPipelinePropertyContext context,
            out object property)
        {
            property = null;
            switch (NormalizeToolType(step?.ToolType))
            {
                case "geometrymeasure":
                case "geometricmeasurement":
                    property = AttachStepMetadata(
                        new GeometryMeasureProperty(step, name, context),
                        name,
                        step.InputLayer,
                        step.OutputLayer);
                    return true;
                case "linefixture":
                case "dualedgefixture":
                    property = AttachStepMetadata(
                        new LineFixtureProperty(step, name, context),
                        name,
                        step.InputLayer,
                        step.OutputLayer);
                    return true;
                case "circlegauge":
                    property = AttachStepMetadata(
                        new CircleGaugeProperty(step, name),
                        name,
                        step.InputLayer,
                        step.OutputLayer);
                    return true;
                default:
                    return false;
            }
        }

        public static bool TryCreateStep(
            object property,
            string inputLayer,
            string outputLayer,
            out VisionPipelineStep step)
        {
            if (property is GeometryMeasureProperty geometryMeasure)
            {
                step = geometryMeasure.ToStep(inputLayer, outputLayer);
                return true;
            }

            if (property is CircleGaugeProperty circleGauge)
            {
                step = circleGauge.ToStep(inputLayer, outputLayer);
                return true;
            }

            if (property is LineFixtureProperty lineFixture)
            {
                step = lineFixture.ToStep(inputLayer, outputLayer);
                return true;
            }

            step = null;
            return false;
        }

        public static string ResolveMetricToolType(object property)
        {
            if (property is GeometryMeasureProperty)
            {
                return "GeometryMeasure";
            }

            if (property is LineFixtureProperty)
            {
                return "LineFixture";
            }

            return property is CircleGaugeProperty ? "CircleGauge" : string.Empty;
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

        public sealed class GeometryFeatureConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(
                ITypeDescriptorContext context)
            {
                if (!(context?.Instance is GeometryMeasureProperty property))
                {
                    return new StandardValuesCollection(Array.Empty<string>());
                }

                bool sourceA = string.Equals(
                    context.PropertyDescriptor?.Name,
                    nameof(GeometryMeasureProperty.SourceA),
                    StringComparison.Ordinal);
                string[] values = property.Context
                    .GetCompatibleGeometryFeatureReferences(
                        property.MeasurementMode,
                        sourceA)
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                return new StandardValuesCollection(values);
            }
        }

        public sealed class LineFixtureFeatureConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(
                ITypeDescriptorContext context)
            {
                if (!(context?.Instance is LineFixtureProperty property))
                {
                    return new StandardValuesCollection(Array.Empty<string>());
                }

                string[] values = property.Context
                    .GetCompatibleGeometryFeatureReferences(
                        GeometryMeasurementMode.LineLineIntersection,
                        true)
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                return new StandardValuesCollection(values);
            }
        }

        private abstract class GeometryPropertyBase :
            VisionPipelineStepPropertyMapper.IPipelineStepMetadata
        {
            protected GeometryPropertyBase(VisionPipelineStep step, string name)
            {
                BaselineParameters = step?.Parameters == null
                    ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, string>(
                        step.Parameters,
                        StringComparer.OrdinalIgnoreCase);
                PipelineStepName = name;
            }

            protected Dictionary<string, string> BaselineParameters { get; }

            [Category("Step")]
            [DisplayName("Step Name")]
            [PropertyOrder(-3)]
            public string PipelineStepName { get; set; }

            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineLayerNameConverter))]
            [PropertyOrder(-2)]
            public string InputLayer { get; set; } = "Main";

            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineLayerNameConverter))]
            [PropertyOrder(-1)]
            public string OutputLayer { get; set; } = "Geometry_Output";

            [Category("Step")]
            [DisplayName("Enabled")]
            [PropertyOrder(0)]
            public bool Enabled { get; set; } = true;

            [Category("Acceptance")]
            [DisplayName("Use Acceptance")]
            [PropertyOrder(1)]
            public bool UseAcceptance { get; set; }

            [Category("Acceptance")]
            [DisplayName("Expected Success")]
            [PropertyOrder(2)]
            public bool ExpectedSuccess { get; set; } = true;

            [Category("Acceptance")]
            [DisplayName("Max Elapsed (ms)")]
            [PropertyOrder(3)]
            public double MaxElapsedMilliseconds { get; set; }

            [Category("Acceptance")]
            [DisplayName("Required Message")]
            [PropertyOrder(4)]
            public string RequiredMessageText { get; set; } = string.Empty;

            [Category("Acceptance")]
            [DisplayName("Acceptance Metric")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineMetricNameConverter))]
            [PropertyOrder(5)]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [Browsable(false)]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [Category("Acceptance")]
            [DisplayName("Metric range")]
            [PropertyEditor(typeof(WpgMetricRangeEditor))]
            [MetricRangeEditor(3, nameof(UseAcceptanceMetricMinimum), nameof(AcceptanceMetricMinimum), nameof(UseAcceptanceMetricMaximum), nameof(AcceptanceMetricMaximum))]
            [PropertyOrder(7)]
            public double AcceptanceMetricMinimum { get; set; }

            [Browsable(false)]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [Browsable(false)]
            public double AcceptanceMetricMaximum { get; set; }

            protected VisionPipelineStep CreateStep(
                string toolType,
                string inputLayer,
                string outputLayer)
            {
                VisionPipelineStep mapped = new VisionPipelineStep
                {
                    Name = string.IsNullOrWhiteSpace(PipelineStepName)
                        ? toolType
                        : PipelineStepName,
                    ToolType = toolType,
                    InputLayer = string.IsNullOrWhiteSpace(inputLayer)
                        ? "Main"
                        : inputLayer,
                    OutputLayer = string.IsNullOrWhiteSpace(outputLayer)
                        ? toolType + "_Output"
                        : outputLayer
                };
                foreach (KeyValuePair<string, string> item in BaselineParameters)
                {
                    mapped.Parameters[item.Key] = item.Value;
                }

                return mapped;
            }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Sources", 0)]
        [CategoryOrder("Geometry Gates", 1)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class GeometryMeasureProperty : GeometryPropertyBase
        {
            public GeometryMeasureProperty(
                VisionPipelineStep step,
                string name,
                VisionPipelinePropertyContext context)
                : base(step, name)
            {
                Context = context ?? VisionPipelinePropertyContext.Empty;
                OutputLayer = string.IsNullOrWhiteSpace(step?.OutputLayer)
                    ? "GeometryMeasure_Output"
                    : step.OutputLayer;
                MeasurementMode = GetEnum(
                    step?.Parameters,
                    VisionPipelineGeometryMeasureService.ModeParameter,
                    GeometryMeasurementMode.PointPointDistance);
                SourceA = JoinGeometryReference(
                    GetString(
                        step?.Parameters,
                        VisionPipelineGeometryMeasureService.SourceStepAParameter,
                        string.Empty),
                    GetString(
                        step?.Parameters,
                        VisionPipelineGeometryMeasureService.SourceFeatureAParameter,
                        string.Empty));
                SourceB = JoinGeometryReference(
                    GetString(
                        step?.Parameters,
                        VisionPipelineGeometryMeasureService.SourceStepBParameter,
                        string.Empty),
                    GetString(
                        step?.Parameters,
                        VisionPipelineGeometryMeasureService.SourceFeatureBParameter,
                        string.Empty));
                MaximumParallelAngleDeltaDeg = GetDouble(
                    step?.Parameters,
                    VisionPipelineGeometryMeasureService.MaximumParallelAngleDeltaParameter,
                    2D);
                MaximumExtensionAPx = GetDouble(
                    step?.Parameters,
                    VisionPipelineGeometryMeasureService.MaximumExtensionAParameter,
                    100D);
                MaximumExtensionBPx = GetDouble(
                    step?.Parameters,
                    VisionPipelineGeometryMeasureService.MaximumExtensionBParameter,
                    100D);
                RequireResultInImage = GetBool(
                    step?.Parameters,
                    VisionPipelineGeometryMeasureService.RequireResultInImageParameter,
                    true);
                UseResultRoi = GetBool(step?.Parameters, "USE_ROI", false);
                ResultRoi = GetRect(step?.Parameters, "CvROI", default);
            }

            [Browsable(false)]
            public VisionPipelinePropertyContext Context { get; }

            [Category("Sources")]
            [DisplayName("Measurement mode")]
            [PropertyOrder(0)]
            public GeometryMeasurementMode MeasurementMode { get; set; }

            [Category("Sources")]
            [DisplayName("Source A")]
            [Description("Compatible typed feature from an earlier enabled Step.")]
            [TypeConverter(typeof(GeometryFeatureConverter))]
            [PropertyOrder(1)]
            public string SourceA { get; set; } = string.Empty;

            [Category("Sources")]
            [DisplayName("Source B")]
            [Description("Compatible typed feature from an earlier enabled Step.")]
            [TypeConverter(typeof(GeometryFeatureConverter))]
            [PropertyOrder(2)]
            public string SourceB { get; set; } = string.Empty;

            [Category("Geometry Gates")]
            [DisplayName("Maximum parallel delta (deg)")]
            [PropertyOrder(0)]
            public double MaximumParallelAngleDeltaDeg { get; set; } = 2D;

            [Category("Geometry Gates")]
            [DisplayName("Maximum extension A (px)")]
            [PropertyOrder(1)]
            public double MaximumExtensionAPx { get; set; } = 100D;

            [Category("Geometry Gates")]
            [DisplayName("Maximum extension B (px)")]
            [PropertyOrder(2)]
            public double MaximumExtensionBPx { get; set; } = 100D;

            [Category("Geometry Gates")]
            [DisplayName("Require result in image")]
            [PropertyOrder(3)]
            public bool RequireResultInImage { get; set; } = true;

            [Category("Geometry Gates")]
            [DisplayName("Use result ROI")]
            [PropertyOrder(4)]
            public bool UseResultRoi { get; set; }

            [Category("Geometry Gates")]
            [DisplayName("Result ROI")]
            [PropertyOrder(5)]
            public Rect ResultRoi { get; set; }

            public VisionPipelineStep ToStep(string inputLayer, string outputLayer)
            {
                SplitGeometryReference(SourceA, out string stepA, out string featureA);
                SplitGeometryReference(SourceB, out string stepB, out string featureB);
                VisionPipelineStep mapped = CreateStep(
                    "GeometryMeasure",
                    inputLayer,
                    outputLayer);
                AddParameter(
                    mapped.Parameters,
                    VisionPipelineGeometryMeasureService.ModeParameter,
                    MeasurementMode);
                AddParameter(
                    mapped.Parameters,
                    VisionPipelineGeometryMeasureService.SourceStepAParameter,
                    stepA);
                AddParameter(
                    mapped.Parameters,
                    VisionPipelineGeometryMeasureService.SourceFeatureAParameter,
                    featureA);
                AddParameter(
                    mapped.Parameters,
                    VisionPipelineGeometryMeasureService.SourceStepBParameter,
                    stepB);
                AddParameter(
                    mapped.Parameters,
                    VisionPipelineGeometryMeasureService.SourceFeatureBParameter,
                    featureB);
                AddParameter(
                    mapped.Parameters,
                    VisionPipelineGeometryMeasureService.MaximumParallelAngleDeltaParameter,
                    MaximumParallelAngleDeltaDeg);
                AddParameter(
                    mapped.Parameters,
                    VisionPipelineGeometryMeasureService.MaximumExtensionAParameter,
                    MaximumExtensionAPx);
                AddParameter(
                    mapped.Parameters,
                    VisionPipelineGeometryMeasureService.MaximumExtensionBParameter,
                    MaximumExtensionBPx);
                AddParameter(
                    mapped.Parameters,
                    VisionPipelineGeometryMeasureService.RequireResultInImageParameter,
                    RequireResultInImage);
                AddParameter(mapped.Parameters, "USE_ROI", UseResultRoi);
                AddParameter(mapped.Parameters, "CvROI", FormatGeometryRect(ResultRoi));
                AddParameter(
                    mapped.Parameters,
                    VisionPipelineNormalizer.AllowBranchInputParameter,
                    true);
                return mapped;
            }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Datum Sources", 0)]
        [CategoryOrder("Fixture Reference", 1)]
        [CategoryOrder("Datum Gates", 2)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class LineFixtureProperty : GeometryPropertyBase
        {
            public LineFixtureProperty(
                VisionPipelineStep step,
                string name,
                VisionPipelinePropertyContext context)
                : base(step, name)
            {
                Context = context ?? VisionPipelinePropertyContext.Empty;
                OutputLayer = string.IsNullOrWhiteSpace(step?.OutputLayer)
                    ? "LineFixture_Output"
                    : step.OutputLayer;
                SourceA = JoinGeometryReference(
                    GetString(step?.Parameters, VisionPipelineLineFixtureService.SourceStepAParameter, string.Empty),
                    GetString(step?.Parameters, VisionPipelineLineFixtureService.SourceFeatureAParameter, string.Empty));
                SourceB = JoinGeometryReference(
                    GetString(step?.Parameters, VisionPipelineLineFixtureService.SourceStepBParameter, string.Empty),
                    GetString(step?.Parameters, VisionPipelineLineFixtureService.SourceFeatureBParameter, string.Empty));
                FrameName = GetString(
                    step?.Parameters,
                    VisionPipelineFixtureFrameService.FrameNameParameter,
                    "DatumFrame");
                ReferenceX = GetDouble(
                    step?.Parameters,
                    VisionPipelineFixtureFrameService.ReferenceXParameter,
                    0D);
                ReferenceY = GetDouble(
                    step?.Parameters,
                    VisionPipelineFixtureFrameService.ReferenceYParameter,
                    0D);
                ReferenceAngleDeg = GetDouble(
                    step?.Parameters,
                    VisionPipelineFixtureFrameService.ReferenceAngleParameter,
                    0D);
                ReferenceImageWidth = GetInt(
                    step?.Parameters,
                    VisionPipelineFixtureFrameService.ReferenceImageWidthParameter,
                    0);
                ReferenceImageHeight = GetInt(
                    step?.Parameters,
                    VisionPipelineFixtureFrameService.ReferenceImageHeightParameter,
                    0);
                MaximumAngleDeltaDeg = GetDouble(
                    step?.Parameters,
                    VisionPipelineFixtureFrameService.MaximumAngleDeltaParameter,
                    10D);
                MinimumSupportA = GetInt(
                    step?.Parameters,
                    VisionPipelineLineFixtureService.MinimumSupportAParameter,
                    3);
                MinimumSupportB = GetInt(
                    step?.Parameters,
                    VisionPipelineLineFixtureService.MinimumSupportBParameter,
                    3);
                MaximumFitResidualAPx = GetDouble(
                    step?.Parameters,
                    VisionPipelineLineFixtureService.MaximumFitResidualAParameter,
                    2D);
                MaximumFitResidualBPx = GetDouble(
                    step?.Parameters,
                    VisionPipelineLineFixtureService.MaximumFitResidualBParameter,
                    2D);
                MinimumIncludedAngleDeg = GetDouble(
                    step?.Parameters,
                    VisionPipelineLineFixtureService.MinimumIncludedAngleParameter,
                    60D);
                MaximumIncludedAngleDeg = GetDouble(
                    step?.Parameters,
                    VisionPipelineLineFixtureService.MaximumIncludedAngleParameter,
                    90D);
                MaximumExtensionAPx = GetDouble(
                    step?.Parameters,
                    VisionPipelineLineFixtureService.MaximumExtensionAParameter,
                    100D);
                MaximumExtensionBPx = GetDouble(
                    step?.Parameters,
                    VisionPipelineLineFixtureService.MaximumExtensionBParameter,
                    100D);
            }

            [Browsable(false)]
            public VisionPipelinePropertyContext Context { get; }

            [Category("Datum Sources")]
            [DisplayName("Datum A segment")]
            [Description("Typed Segment from an earlier accepted Line Step. Datum A defines the fixture X axis.")]
            [TypeConverter(typeof(LineFixtureFeatureConverter))]
            [PropertyOrder(0)]
            public string SourceA { get; set; } = string.Empty;

            [Category("Datum Sources")]
            [DisplayName("Datum B segment")]
            [Description("Distinct typed Segment from an earlier accepted Line Step.")]
            [TypeConverter(typeof(LineFixtureFeatureConverter))]
            [PropertyOrder(1)]
            public string SourceB { get; set; } = string.Empty;

            [Category("Fixture Reference")]
            [DisplayName("Frame name")]
            [PropertyOrder(0)]
            public string FrameName { get; set; } = "DatumFrame";

            [Category("Fixture Reference")]
            [DisplayName("Reference origin X")]
            [PropertyOrder(1)]
            public double ReferenceX { get; set; }

            [Category("Fixture Reference")]
            [DisplayName("Reference origin Y")]
            [PropertyOrder(2)]
            public double ReferenceY { get; set; }

            [Category("Fixture Reference")]
            [DisplayName("Reference X-axis angle (deg)")]
            [PropertyOrder(3)]
            public double ReferenceAngleDeg { get; set; }

            [Category("Fixture Reference")]
            [DisplayName("Reference image width")]
            [PropertyOrder(4)]
            public int ReferenceImageWidth { get; set; }

            [Category("Fixture Reference")]
            [DisplayName("Reference image height")]
            [PropertyOrder(5)]
            public int ReferenceImageHeight { get; set; }

            [Category("Fixture Reference")]
            [DisplayName("Maximum angle delta (deg)")]
            [PropertyOrder(6)]
            public double MaximumAngleDeltaDeg { get; set; } = 10D;

            [Category("Datum Gates")]
            [DisplayName("Minimum support A")]
            [PropertyOrder(0)]
            public int MinimumSupportA { get; set; } = 3;

            [Category("Datum Gates")]
            [DisplayName("Minimum support B")]
            [PropertyOrder(1)]
            public int MinimumSupportB { get; set; } = 3;

            [Category("Datum Gates")]
            [DisplayName("Maximum residual A (px)")]
            [PropertyOrder(2)]
            public double MaximumFitResidualAPx { get; set; } = 2D;

            [Category("Datum Gates")]
            [DisplayName("Maximum residual B (px)")]
            [PropertyOrder(3)]
            public double MaximumFitResidualBPx { get; set; } = 2D;

            [Category("Datum Gates")]
            [DisplayName("Minimum included angle (deg)")]
            [PropertyOrder(4)]
            public double MinimumIncludedAngleDeg { get; set; } = 60D;

            [Category("Datum Gates")]
            [DisplayName("Maximum included angle (deg)")]
            [PropertyOrder(5)]
            public double MaximumIncludedAngleDeg { get; set; } = 90D;

            [Category("Datum Gates")]
            [DisplayName("Maximum extension A (px)")]
            [PropertyOrder(6)]
            public double MaximumExtensionAPx { get; set; } = 100D;

            [Category("Datum Gates")]
            [DisplayName("Maximum extension B (px)")]
            [PropertyOrder(7)]
            public double MaximumExtensionBPx { get; set; } = 100D;

            public VisionPipelineStep ToStep(string inputLayer, string outputLayer)
            {
                SplitGeometryReference(SourceA, out string stepA, out string featureA);
                SplitGeometryReference(SourceB, out string stepB, out string featureB);
                VisionPipelineStep mapped = CreateStep("LineFixture", inputLayer, outputLayer);
                AddParameter(mapped.Parameters, VisionPipelineLineFixtureService.SourceStepAParameter, stepA);
                AddParameter(mapped.Parameters, VisionPipelineLineFixtureService.SourceFeatureAParameter, featureA);
                AddParameter(mapped.Parameters, VisionPipelineLineFixtureService.SourceStepBParameter, stepB);
                AddParameter(mapped.Parameters, VisionPipelineLineFixtureService.SourceFeatureBParameter, featureB);
                AddParameter(mapped.Parameters, VisionPipelineLineFixtureService.MinimumSupportAParameter, MinimumSupportA);
                AddParameter(mapped.Parameters, VisionPipelineLineFixtureService.MinimumSupportBParameter, MinimumSupportB);
                AddParameter(mapped.Parameters, VisionPipelineLineFixtureService.MaximumFitResidualAParameter, MaximumFitResidualAPx);
                AddParameter(mapped.Parameters, VisionPipelineLineFixtureService.MaximumFitResidualBParameter, MaximumFitResidualBPx);
                AddParameter(mapped.Parameters, VisionPipelineLineFixtureService.MinimumIncludedAngleParameter, MinimumIncludedAngleDeg);
                AddParameter(mapped.Parameters, VisionPipelineLineFixtureService.MaximumIncludedAngleParameter, MaximumIncludedAngleDeg);
                AddParameter(mapped.Parameters, VisionPipelineLineFixtureService.MaximumExtensionAParameter, MaximumExtensionAPx);
                AddParameter(mapped.Parameters, VisionPipelineLineFixtureService.MaximumExtensionBParameter, MaximumExtensionBPx);
                AddParameter(mapped.Parameters, VisionPipelineFixtureFrameService.PublishParameter, true);
                AddParameter(mapped.Parameters, VisionPipelineFixtureFrameService.FrameNameParameter, FrameName);
                AddParameter(mapped.Parameters, VisionPipelineFixtureFrameService.ReferenceXParameter, ReferenceX);
                AddParameter(mapped.Parameters, VisionPipelineFixtureFrameService.ReferenceYParameter, ReferenceY);
                AddParameter(mapped.Parameters, VisionPipelineFixtureFrameService.ReferenceAngleParameter, ReferenceAngleDeg);
                AddParameter(mapped.Parameters, VisionPipelineFixtureFrameService.ReferenceScaleParameter, 1D);
                AddParameter(mapped.Parameters, VisionPipelineFixtureFrameService.MaximumAngleDeltaParameter, MaximumAngleDeltaDeg);
                AddParameter(mapped.Parameters, VisionPipelineFixtureFrameService.MinimumScaleRatioParameter, 1D);
                AddParameter(mapped.Parameters, VisionPipelineFixtureFrameService.MaximumScaleRatioParameter, 1D);
                AddParameter(mapped.Parameters, VisionPipelineFixtureFrameService.ReferenceImageWidthParameter, ReferenceImageWidth);
                AddParameter(mapped.Parameters, VisionPipelineFixtureFrameService.ReferenceImageHeightParameter, ReferenceImageHeight);
                AddParameter(mapped.Parameters, VisionPipelineNormalizer.AllowBranchInputParameter, true);
                return mapped;
            }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Annular Sector", 0)]
        [CategoryOrder("Edge Fit", 1)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class CircleGaugeProperty : GeometryPropertyBase
        {
            public CircleGaugeProperty(VisionPipelineStep step, string name)
                : base(step, name)
            {
                OutputLayer = string.IsNullOrWhiteSpace(step?.OutputLayer)
                    ? "CircleGauge_Output"
                    : step.OutputLayer;
                UseRoi = GetBool(step?.Parameters, "USE_ROI", true);
                Roi = GetRect(step?.Parameters, "CvROI", default);
                CenterX = GetDouble(step?.Parameters, "CENTER_X", 0D);
                CenterY = GetDouble(step?.Parameters, "CENTER_Y", 0D);
                MinimumRadius = GetDouble(step?.Parameters, "RADIUS_MIN", 20D);
                MaximumRadius = GetDouble(step?.Parameters, "RADIUS_MAX", 60D);
                StartAngleDeg = GetDouble(step?.Parameters, "START_ANGLE_DEG", 0D);
                SweepAngleDeg = GetDouble(step?.Parameters, "SWEEP_ANGLE_DEG", 360D);
                ScanCount = GetInt(step?.Parameters, "SCAN_COUNT", 180);
                EdgePolarity = GetEnum(
                    step?.Parameters,
                    "EDGE_POLARITY",
                    CircleGaugeEdgePolarity.Either);
                MinimumContrast = GetDouble(step?.Parameters, "MIN_CONTRAST", 12D);
                MinimumSupportRatio = GetDouble(
                    step?.Parameters,
                    "MIN_SUPPORT_RATIO",
                    0.6D);
                MaximumFitResidualPx = GetDouble(
                    step?.Parameters,
                    "MAX_FIT_RESIDUAL_PX",
                    2D);
            }

            [Category("Annular Sector")]
            [DisplayName("Use ROI")]
            [PropertyOrder(0)]
            public bool UseRoi { get; set; }

            [Category("Annular Sector")]
            [DisplayName("ROI")]
            [PropertyOrder(1)]
            public Rect Roi { get; set; }

            [Category("Annular Sector")]
            [DisplayName("Center X")]
            [PropertyOrder(2)]
            public double CenterX { get; set; }

            [Category("Annular Sector")]
            [DisplayName("Center Y")]
            [PropertyOrder(3)]
            public double CenterY { get; set; }

            [Category("Annular Sector")]
            [DisplayName("Minimum radius (px)")]
            [PropertyOrder(4)]
            public double MinimumRadius { get; set; }

            [Category("Annular Sector")]
            [DisplayName("Maximum radius (px)")]
            [PropertyOrder(5)]
            public double MaximumRadius { get; set; }

            [Category("Annular Sector")]
            [DisplayName("Start angle (deg)")]
            [PropertyOrder(6)]
            public double StartAngleDeg { get; set; }

            [Category("Annular Sector")]
            [DisplayName("Sweep angle (deg)")]
            [PropertyOrder(7)]
            public double SweepAngleDeg { get; set; }

            [Category("Annular Sector")]
            [DisplayName("Radial scan count")]
            [PropertyOrder(8)]
            public int ScanCount { get; set; }

            [Category("Edge Fit")]
            [DisplayName("Edge polarity")]
            [PropertyOrder(0)]
            public CircleGaugeEdgePolarity EdgePolarity { get; set; }

            [Category("Edge Fit")]
            [DisplayName("Minimum contrast")]
            [PropertyOrder(1)]
            public double MinimumContrast { get; set; }

            [Category("Edge Fit")]
            [DisplayName("Minimum support ratio")]
            [PropertyOrder(2)]
            public double MinimumSupportRatio { get; set; }

            [Category("Edge Fit")]
            [DisplayName("Maximum fit residual (px)")]
            [PropertyOrder(3)]
            public double MaximumFitResidualPx { get; set; }

            public VisionPipelineStep ToStep(string inputLayer, string outputLayer)
            {
                VisionPipelineStep mapped = CreateStep(
                    "CircleGauge",
                    inputLayer,
                    outputLayer);
                AddParameter(mapped.Parameters, "USE_ROI", UseRoi);
                AddParameter(mapped.Parameters, "CvROI", FormatGeometryRect(Roi));
                AddParameter(mapped.Parameters, "CENTER_X", CenterX);
                AddParameter(mapped.Parameters, "CENTER_Y", CenterY);
                AddParameter(mapped.Parameters, "RADIUS_MIN", MinimumRadius);
                AddParameter(mapped.Parameters, "RADIUS_MAX", MaximumRadius);
                AddParameter(mapped.Parameters, "START_ANGLE_DEG", StartAngleDeg);
                AddParameter(mapped.Parameters, "SWEEP_ANGLE_DEG", SweepAngleDeg);
                AddParameter(mapped.Parameters, "SCAN_COUNT", ScanCount);
                AddParameter(mapped.Parameters, "EDGE_POLARITY", EdgePolarity);
                AddParameter(mapped.Parameters, "MIN_CONTRAST", MinimumContrast);
                AddParameter(
                    mapped.Parameters,
                    "MIN_SUPPORT_RATIO",
                    MinimumSupportRatio);
                AddParameter(
                    mapped.Parameters,
                    "MAX_FIT_RESIDUAL_PX",
                    MaximumFitResidualPx);
                return mapped;
            }
        }

        private static string JoinGeometryReference(string step, string feature)
        {
            return string.IsNullOrWhiteSpace(step)
                || string.IsNullOrWhiteSpace(feature)
                    ? string.Empty
                    : step.Trim() + "/" + feature.Trim();
        }

        private static void SplitGeometryReference(
            string reference,
            out string step,
            out string feature)
        {
            int slash = (reference ?? string.Empty).LastIndexOf('/');
            step = slash > 0
                ? reference.Substring(0, slash).Trim()
                : string.Empty;
            feature = slash > 0 && slash < reference.Length - 1
                ? reference.Substring(slash + 1).Trim()
                : string.Empty;
        }

        private static string FormatGeometryRect(Rect roi)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0},{1},{2},{3}",
                roi.X,
                roi.Y,
                roi.Width,
                roi.Height);
        }
    }
}
