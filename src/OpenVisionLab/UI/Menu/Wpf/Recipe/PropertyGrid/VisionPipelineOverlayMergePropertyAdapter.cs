using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Controls.WpfPropertyGrid;
using static OpenVisionLab.PropertyGridEditorFactory;

namespace OpenVisionLab
{
    internal static class VisionPipelineOverlayMergePropertyAdapter
    {
        public static bool TryCreateProperty(
            VisionPipelineStep step,
            string name,
            out object property)
        {
            property = null;
            if (!VisionPipelineOverlayMergeService.IsMergeTool(step?.ToolType))
            {
                return false;
            }

            property = new OverlayMergeProperty(step, name);
            return true;
        }

        public static bool TryCreateStep(
            object property,
            string inputLayer,
            string outputLayer,
            out VisionPipelineStep step)
        {
            if (property is OverlayMergeProperty overlayMerge)
            {
                step = overlayMerge.ToStep(inputLayer, outputLayer);
                return true;
            }

            step = null;
            return false;
        }

        public static bool IsProperty(object property)
        {
            return property is OverlayMergeProperty;
        }

        public static bool TryResetRenderingDefaults(object property)
        {
            if (!(property is OverlayMergeProperty overlayMerge))
            {
                return false;
            }

            overlayMerge.ResetRenderingDefaults();
            return true;
        }

        [CategoryOrder("Step", -1)]
        [CategoryOrder("Overlay sources", 0)]
        [CategoryOrder("Display only", 1)]
        [CategoryOrder("Output", 2)]
        [CategoryOrder("Acceptance", 20)]
        private sealed class OverlayMergeProperty :
            VisionPipelineStepPropertyMapper.IPipelineStepMetadata,
            INotifyPropertyChanged
        {
            private readonly Dictionary<string, string> baselineParameters;
            private readonly string toolType;
            private VisionPipelineOverlayRenderPreset renderPreset;
            private VisionPipelineOverlayLabelMode labelMode;
            private int lineWidth;
            private int pointSize;
            private bool labelBackground;
            private int labelMargin;

            public OverlayMergeProperty(VisionPipelineStep step, string name)
            {
                baselineParameters = new Dictionary<string, string>(
                    step?.Parameters ?? new Dictionary<string, string>(),
                    StringComparer.OrdinalIgnoreCase);
                toolType = string.IsNullOrWhiteSpace(step?.ToolType)
                    ? "OverlayMerge"
                    : step.ToolType;
                PipelineStepName = string.IsNullOrWhiteSpace(name)
                    ? "OverlayMerge"
                    : name;
                InputLayer = string.IsNullOrWhiteSpace(step?.InputLayer)
                    ? "Main"
                    : step.InputLayer;
                OutputLayer = string.IsNullOrWhiteSpace(step?.OutputLayer)
                    ? "OverlayMerge_Output"
                    : step.OutputLayer;
                SourceLayers = GetString(step?.Parameters, "SourceLayers", string.Empty);
                SourceSteps = GetString(step?.Parameters, "SourceSteps", string.Empty);
                BurnIn = GetBool(step?.Parameters, "BurnIn", true);
                AllowEmpty = GetBool(step?.Parameters, "AllowEmpty", false);
                MaxPoints = GetInt(step?.Parameters, "MaxPoints", 300);

                bool legacyDrawLabels = GetBool(step?.Parameters, "DrawLabels", false);
                renderPreset = GetEnum(
                    step?.Parameters,
                    VisionPipelineOverlayMergeService.RenderPresetParameter,
                    VisionPipelineOverlayRenderPreset.LegacyDefault);
                labelMode = GetEnum(
                    step?.Parameters,
                    VisionPipelineOverlayMergeService.LabelModeParameter,
                    legacyDrawLabels
                        ? VisionPipelineOverlayLabelMode.Name
                        : VisionPipelineOverlayLabelMode.None);
                lineWidth = GetInt(
                    step?.Parameters,
                    VisionPipelineOverlayMergeService.LineWidthParameter,
                    VisionPipelineOverlayMergeService.DefaultLineWidth);
                pointSize = GetInt(
                    step?.Parameters,
                    VisionPipelineOverlayMergeService.PointSizeParameter,
                    VisionPipelineOverlayMergeService.DefaultPointSize);
                labelBackground = GetBool(
                    step?.Parameters,
                    VisionPipelineOverlayMergeService.LabelBackgroundParameter,
                    false);
                labelMargin = GetInt(
                    step?.Parameters,
                    VisionPipelineOverlayMergeService.LabelMarginParameter,
                    VisionPipelineOverlayMergeService.DefaultLabelMargin);
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [PropertyOrder(-3)]
            [Category("Step")]
            [DisplayName("Step Name")]
            public string PipelineStepName { get; set; }

            [PropertyOrder(-2)]
            [Category("Step")]
            [DisplayName("Input Layer")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineLayerNameConverter))]
            public string InputLayer { get; set; } = "Main";

            [PropertyOrder(-1)]
            [Category("Step")]
            [DisplayName("Output Layer")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineLayerNameConverter))]
            public string OutputLayer { get; set; } = "OverlayMerge_Output";

            [Category("Step")]
            [DisplayName("Enabled")]
            public bool Enabled { get; set; } = true;

            [Category("Overlay sources")]
            [DisplayName("Source Layers")]
            [Description("Optional comma/semicolon-separated prior output layers. Leave empty only when every prior overlay belongs in this review image.")]
            public string SourceLayers { get; set; } = string.Empty;

            [Category("Overlay sources")]
            [DisplayName("Source Steps")]
            [Description("Optional comma/semicolon-separated prior Step names. This selection is saved with the recipe.")]
            public string SourceSteps { get; set; } = string.Empty;

            [Category("Display only")]
            [DisplayName("Rendering Preset")]
            [Description("Project-owned bounded palette. LegacyDefault preserves existing recipe colors.")]
            public VisionPipelineOverlayRenderPreset RenderPreset
            {
                get => renderPreset;
                set => SetField(ref renderPreset, value);
            }

            [Category("Display only")]
            [DisplayName("Label Mode")]
            [Description("NameWithCoordinates appends image X/Y coordinates. Labels do not affect metrics or acceptance.")]
            public VisionPipelineOverlayLabelMode LabelMode
            {
                get => labelMode;
                set => SetField(ref labelMode, value);
            }

            [Category("Display only")]
            [DisplayName("Line Width")]
            [Description("Display-only line width in image pixels (1..8).")]
            public int LineWidth
            {
                get => lineWidth;
                set => SetField(ref lineWidth, value);
            }

            [Category("Display only")]
            [DisplayName("Point Marker Size")]
            [Description("Display-only point/cross marker size in image pixels (1..12).")]
            public int PointSize
            {
                get => pointSize;
                set => SetField(ref pointSize, value);
            }

            [Category("Display only")]
            [DisplayName("Label Background")]
            [Description("Draw a black backing rectangle behind labels for readability.")]
            public bool LabelBackground
            {
                get => labelBackground;
                set => SetField(ref labelBackground, value);
            }

            [Category("Display only")]
            [DisplayName("Label Margin")]
            [Description("Background margin in image pixels (0..12).")]
            public int LabelMargin
            {
                get => labelMargin;
                set => SetField(ref labelMargin, value);
            }

            [Category("Display only")]
            [DisplayName("Maximum Points")]
            [Description("Maximum number of points drawn for a Points overlay. Zero hides point clouds without changing result metrics.")]
            public int MaxPoints { get; set; } = 300;

            [Category("Output")]
            [DisplayName("Burn Into Image")]
            [Description("Burn selected overlays into the output image. Preview/Run remains an explicit action.")]
            public bool BurnIn { get; set; } = true;

            [Category("Output")]
            [DisplayName("Allow Empty")]
            public bool AllowEmpty { get; set; }

            [Category("Acceptance")]
            [DisplayName("Use Acceptance")]
            public bool UseAcceptance { get; set; }

            [Category("Acceptance")]
            [DisplayName("Expected Success")]
            public bool ExpectedSuccess { get; set; } = true;

            [Category("Acceptance")]
            [DisplayName("Maximum Time (ms)")]
            public double MaxElapsedMilliseconds { get; set; }

            [Category("Acceptance")]
            [DisplayName("Required Message")]
            public string RequiredMessageText { get; set; } = string.Empty;

            [Category("Acceptance")]
            [DisplayName("Acceptance Metric")]
            [TypeConverter(typeof(VisionPipelineStepPropertyMapper.PipelineMetricNameConverter))]
            public string AcceptanceMetricName { get; set; } = string.Empty;

            [Category("Acceptance")]
            [DisplayName("Use Metric Minimum")]
            public bool UseAcceptanceMetricMinimum { get; set; }

            [Category("Acceptance")]
            [DisplayName("Metric Minimum")]
            public double AcceptanceMetricMinimum { get; set; }

            [Category("Acceptance")]
            [DisplayName("Use Metric Maximum")]
            public bool UseAcceptanceMetricMaximum { get; set; }

            [Category("Acceptance")]
            [DisplayName("Metric Maximum")]
            public double AcceptanceMetricMaximum { get; set; }

            public VisionPipelineStep ToStep(string inputLayer, string outputLayer)
            {
                VisionPipelineStep step = new VisionPipelineStep
                {
                    Name = string.IsNullOrWhiteSpace(PipelineStepName)
                        ? "OverlayMerge"
                        : PipelineStepName,
                    ToolType = toolType,
                    InputLayer = string.IsNullOrWhiteSpace(inputLayer)
                        ? InputLayer
                        : inputLayer,
                    OutputLayer = string.IsNullOrWhiteSpace(outputLayer)
                        ? OutputLayer
                        : outputLayer
                };

                foreach (KeyValuePair<string, string> item in baselineParameters)
                {
                    step.Parameters[item.Key] = item.Value;
                }

                Set(step, "SourceLayers", SourceLayers);
                Set(step, "SourceSteps", SourceSteps);
                Set(step, "BurnIn", BurnIn);
                Set(step, "AllowEmpty", AllowEmpty);
                Set(step, "MaxPoints", MaxPoints);
                Set(step, VisionPipelineOverlayMergeService.RenderPresetParameter, RenderPreset);
                Set(step, VisionPipelineOverlayMergeService.LabelModeParameter, LabelMode);
                Set(step, VisionPipelineOverlayMergeService.LineWidthParameter, LineWidth);
                Set(step, VisionPipelineOverlayMergeService.PointSizeParameter, PointSize);
                Set(step, VisionPipelineOverlayMergeService.LabelBackgroundParameter, LabelBackground);
                Set(step, VisionPipelineOverlayMergeService.LabelMarginParameter, LabelMargin);
                Set(step, "DrawLabels", LabelMode != VisionPipelineOverlayLabelMode.None);
                return step;
            }

            public void ResetRenderingDefaults()
            {
                RenderPreset = VisionPipelineOverlayRenderPreset.LegacyDefault;
                LabelMode = VisionPipelineOverlayLabelMode.None;
                LineWidth = VisionPipelineOverlayMergeService.DefaultLineWidth;
                PointSize = VisionPipelineOverlayMergeService.DefaultPointSize;
                LabelBackground = false;
                LabelMargin = VisionPipelineOverlayMergeService.DefaultLabelMargin;
            }

            private bool SetField<T>(
                ref T field,
                T value,
                [CallerMemberName] string propertyName = null)
            {
                if (Equals(field, value))
                {
                    return false;
                }

                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
        }

        private static string GetString(
            IDictionary<string, string> parameters,
            string key,
            string defaultValue)
        {
            return TryGetValue(parameters, key, out string value)
                ? value ?? string.Empty
                : defaultValue;
        }

        private static bool GetBool(
            IDictionary<string, string> parameters,
            string key,
            bool defaultValue)
        {
            return TryGetValue(parameters, key, out string value)
                && bool.TryParse(value, out bool result)
                    ? result
                    : defaultValue;
        }

        private static int GetInt(
            IDictionary<string, string> parameters,
            string key,
            int defaultValue)
        {
            return TryGetValue(parameters, key, out string value)
                && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
                    ? result
                    : defaultValue;
        }

        private static T GetEnum<T>(
            IDictionary<string, string> parameters,
            string key,
            T defaultValue)
            where T : struct
        {
            return TryGetValue(parameters, key, out string value)
                && Enum.TryParse(value, true, out T result)
                && Enum.IsDefined(typeof(T), result)
                    ? result
                    : defaultValue;
        }

        private static bool TryGetValue(
            IDictionary<string, string> parameters,
            string key,
            out string value)
        {
            value = null;
            if (parameters == null)
            {
                return false;
            }

            foreach (KeyValuePair<string, string> item in parameters)
            {
                if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    value = item.Value;
                    return true;
                }
            }

            return false;
        }

        private static void Set(VisionPipelineStep step, string key, object value)
        {
            step.Parameters[key] =
                Convert.ToString(value, CultureInfo.InvariantCulture)
                ?? string.Empty;
        }
    }
}
