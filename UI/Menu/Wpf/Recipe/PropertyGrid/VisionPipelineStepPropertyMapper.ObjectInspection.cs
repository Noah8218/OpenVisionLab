using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls.WpfPropertyGrid;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    internal static partial class VisionPipelineStepPropertyMapper
    {
        private static object CreateObjectInspectionProperty(
            VisionPipelineStep step,
            string name,
            string toolType)
        {
            switch (toolType)
            {
                case "blob":
                    return AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineBlobProperty(name)
                    {
                        MIN_AREA = GetInt(step.Parameters, nameof(BlobProperty.MIN_AREA), 200),
                        MAX_AREA = GetInt(step.Parameters, nameof(BlobProperty.MAX_AREA), 1000000),
                        MIN_WIDTH = GetInt(step.Parameters, nameof(BlobProperty.MIN_WIDTH), 0),
                        MAX_WIDTH = GetInt(step.Parameters, nameof(BlobProperty.MAX_WIDTH), 1000000),
                        MIN_HEIGHT = GetInt(step.Parameters, nameof(BlobProperty.MIN_HEIGHT), 0),
                        MAX_HEIGHT = GetInt(step.Parameters, nameof(BlobProperty.MAX_HEIGHT), 1000000),
                        USE_FIXTURE_FRAME = GetBool(step.Parameters, VisionPipelineFixtureFrameService.ConsumeParameter, false),
                        FIXTURE_FRAME_NAME = GetString(step.Parameters, VisionPipelineFixtureFrameService.FrameNameParameter, string.Empty),
                        ALLOW_BRANCH_INPUT = GetBool(step.Parameters, VisionPipelineNormalizer.AllowBranchInputParameter, false)
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
                case "contour":
                    return AttachStepMetadata(ApplyCommonOpenCvProperty(new PipelineContourProperty(name)
                    {
                        USE_APPROXPOLYDP = GetBool(step.Parameters, nameof(ContourProperty.USE_APPROXPOLYDP), false),
                        USE_DRAW_IMAGE = GetBool(step.Parameters, nameof(ContourProperty.USE_DRAW_IMAGE), false),
                        DrawMode = GetEnum(step.Parameters, nameof(ContourProperty.DrawMode), ContourDrawMode.Outline),
                        ApproximationModes = GetEnum(step.Parameters, nameof(ContourProperty.ApproximationModes), ContourApproximationModes.ApproxSimple),
                        DetectMode = GetEnum(step.Parameters, nameof(ContourProperty.DetectMode), RetrievalModes.External),
                        EPSILON = GetDouble(step.Parameters, nameof(ContourProperty.EPSILON), 0.01),
                        MIN_AREA = GetInt(step.Parameters, nameof(ContourProperty.MIN_AREA), 200),
                        MAX_AREA = GetInt(step.Parameters, nameof(ContourProperty.MAX_AREA), 1000000),
                        MIN_WIDTH = GetInt(step.Parameters, nameof(ContourProperty.MIN_WIDTH), 0),
                        MAX_WIDTH = GetInt(step.Parameters, nameof(ContourProperty.MAX_WIDTH), 1000000),
                        MIN_HEIGHT = GetInt(step.Parameters, nameof(ContourProperty.MIN_HEIGHT), 0),
                        MAX_HEIGHT = GetInt(step.Parameters, nameof(ContourProperty.MAX_HEIGHT), 1000000),
                        DrawThickness = GetInt(step.Parameters, nameof(ContourProperty.DrawThickness), 2),
                        ClrGridHtml = GetString(step.Parameters, nameof(ContourProperty.ClrGridHtml), "#ff0000")
                    }, step.Parameters), name, step.InputLayer, step.OutputLayer);
                default:
                    return null;
            }
        }

        private static void ApplyObjectInspectionParameters(
            object property,
            IDictionary<string, string> parameters)
        {
            if (property is PipelineBlobProperty blobFixture)
            {
                blobFixture.ApplyFixtureParameters(parameters);
            }
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Blob Parameter", 0)]
        [CategoryOrder("Fixture", 5)]
        [CategoryOrder("ROI", 10)]
        [CategoryOrder("Threshold", 20)]
        [CategoryOrder("Parameter", 30)]
        [CategoryOrder("Acceptance", 40)]
        private sealed class PipelineBlobProperty : BlobProperty, IPipelineStepMetadata
        {
            public PipelineBlobProperty(string name)
                : base(name)
            {
            }

            [Browsable(false)]
            public string PipelineStepName
            {
                get => NAME;
                set => NAME = value;
            }

            [PropertyOrder(0)]
            [Category("Fixture")]
            [DisplayName("Use Fixture Frame")]
            [Description("Move this step's single CvROI by a previously published Matching fixture frame. Preview/Run remains explicit.")]
            public bool USE_FIXTURE_FRAME { get; set; }

            [PropertyOrder(1)]
            [Category("Fixture")]
            [DisplayName("Fixture Frame Name")]
            [Description("Name of the earlier Matching fixture frame. The producer and consumer names must match.")]
            public string FIXTURE_FRAME_NAME { get; set; } = string.Empty;

            [PropertyOrder(2)]
            [Category("Fixture")]
            [DisplayName("Allow Branch Input")]
            [Description("Explicitly allow this step to read the same source layer as the Matching fixture producer.")]
            public bool ALLOW_BRANCH_INPUT { get; set; }

            public void ApplyFixtureParameters(IDictionary<string, string> parameters)
            {
                if (parameters == null)
                {
                    return;
                }

                if (USE_FIXTURE_FRAME)
                {
                    parameters[VisionPipelineFixtureFrameService.ConsumeParameter] = Convert.ToString(true, CultureInfo.InvariantCulture);
                    parameters[VisionPipelineFixtureFrameService.FrameNameParameter] = FIXTURE_FRAME_NAME?.Trim() ?? string.Empty;
                }

                if (ALLOW_BRANCH_INPUT)
                {
                    parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = Convert.ToString(true, CultureInfo.InvariantCulture);
                }
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
        [CategoryOrder("Contour", 0)]
        [CategoryOrder("ROI", 10)]
        [CategoryOrder("Threshold", 20)]
        [CategoryOrder("Parameter", 30)]
        [CategoryOrder("Acceptance", 40)]
        private sealed class PipelineContourProperty : ContourProperty, IPipelineStepMetadata
        {
            public PipelineContourProperty(string name)
                : base(name)
            {
            }

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
