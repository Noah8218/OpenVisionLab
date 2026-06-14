using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static Lib.Common.FormulaUtil;

namespace OpenVisionLab
{
    internal static class VisionPipelineAppToolFactory
    {
        public static IVisionTool Create(VisionPipelineStep step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            string toolType = NormalizeToolType(step.ToolType);

            switch (toolType)
            {
                case "blob":
                    return CreateBlobTool(step.Parameters);
                case "contour":
                    return CreateContourTool(step.Parameters);
                case "line":
                case "linegauge":
                    return CreateLineGaugeTool(step.Parameters);
                case "matching":
                case "templatematching":
                    return CreateMatchingTool(step.Parameters);
                case "mean":
                    return CreateMeanTool(step.Parameters);
                case "feature":
                case "featurematching":
                case "sift":
                    return CreateFeatureMatchingTool(step.Parameters);
                default:
                    return VisionPipelineToolFactory.Create(step);
            }
        }

        private static IVisionTool CreateBlobTool(IDictionary<string, string> parameters)
        {
            BlobProperty property = new BlobProperty(GetString(parameters, "Name", "PipelineBlob"))
            {
                MIN_AREA = GetInt(parameters, nameof(BlobProperty.MIN_AREA), 200),
                MAX_AREA = GetInt(parameters, nameof(BlobProperty.MAX_AREA), 1000000)
            };

            ApplyCommonOpenCvProperty(property, parameters);

            BlobTool tool = new BlobTool();
            tool.SetProperty(property);
            return tool;
        }

        private static IVisionTool CreateContourTool(IDictionary<string, string> parameters)
        {
            ContourProperty property = new ContourProperty(GetString(parameters, "Name", "PipelineContour"))
            {
                USE_APPROXPOLYDP = GetBool(parameters, nameof(ContourProperty.USE_APPROXPOLYDP), false),
                USE_DRAW_IMAGE = GetBool(parameters, nameof(ContourProperty.USE_DRAW_IMAGE), false),
                ApproximationModes = GetEnum(parameters, nameof(ContourProperty.ApproximationModes), ContourApproximationModes.ApproxSimple),
                DetectMode = GetEnum(parameters, nameof(ContourProperty.DetectMode), RetrievalModes.List),
                EPSILON = GetDouble(parameters, nameof(ContourProperty.EPSILON), 0.01),
                MIN_AREA = GetInt(parameters, nameof(ContourProperty.MIN_AREA), 200),
                MAX_AREA = GetInt(parameters, nameof(ContourProperty.MAX_AREA), 1000000),
                ClrGridHtml = GetString(parameters, nameof(ContourProperty.ClrGridHtml), "#000000"),
                DrawThickness = GetInt(parameters, nameof(ContourProperty.DrawThickness), 2)
            };

            ApplyCommonOpenCvProperty(property, parameters);

            ContourTool tool = new ContourTool();
            tool.SetProperty(property);
            return tool;
        }

        private static IVisionTool CreateLineGaugeTool(IDictionary<string, string> parameters)
        {
            LineGaugeProperty property = new LineGaugeProperty(GetString(parameters, "Name", "PipelineLineGauge"))
            {
                PRJ_PORALITY = GetEnum(parameters, nameof(LineGaugeProperty.PRJ_PORALITY), PROJECTION_POLARITY.BTOW),
                PRJ_DIR = GetEnum(parameters, nameof(LineGaugeProperty.PRJ_DIR), PROJECTION_DIR.X_LTOR),
                CONTRAST = GetDouble(parameters, nameof(LineGaugeProperty.CONTRAST), 30),
                THICKNESS = GetDouble(parameters, nameof(LineGaugeProperty.THICKNESS), 5),
                SAMPLING_STEP = GetDouble(parameters, nameof(LineGaugeProperty.SAMPLING_STEP), 10),
                VER_PRJ_DIR = GetEnum(parameters, nameof(LineGaugeProperty.VER_PRJ_DIR), PROJECTION_DIR.X_LTOR),
                POINT_RANGE = GetInt(parameters, nameof(LineGaugeProperty.POINT_RANGE), 10),
                USE_MANUAL_ANGLE = GetBool(parameters, nameof(LineGaugeProperty.USE_MANUAL_ANGLE), false),
                MANUAL_ANGLE_VALUE = GetDouble(parameters, nameof(LineGaugeProperty.MANUAL_ANGLE_VALUE), 0),
                USE_EXTEND_FIT_LINE = GetBool(parameters, nameof(LineGaugeProperty.USE_EXTEND_FIT_LINE), false),
                EXTEND_FIT_LINE_VALUE = GetInt(parameters, nameof(LineGaugeProperty.EXTEND_FIT_LINE_VALUE), 100),
                AVERAGE_Diff = GetDouble(parameters, nameof(LineGaugeProperty.AVERAGE_Diff), 100),
                USE_AVERAGE_FILTER = GetBool(parameters, nameof(LineGaugeProperty.USE_AVERAGE_FILTER), false),
                AVERAGE_FILTER_TYPE = GetEnum(parameters, nameof(LineGaugeProperty.AVERAGE_FILTER_TYPE), LineGaugeProperty.AVERAGE_FILTER_TYPES.Y),
                SHOW_VERTICAL_LINE = GetBool(parameters, nameof(LineGaugeProperty.SHOW_VERTICAL_LINE), true),
                SHOW_EDGE = GetBool(parameters, nameof(LineGaugeProperty.SHOW_EDGE), true),
                SHOW_CONTOUR = GetBool(parameters, nameof(LineGaugeProperty.SHOW_CONTOUR), true),
                SHOW_FITLINE = GetBool(parameters, nameof(LineGaugeProperty.SHOW_FITLINE), true)
            };

            ApplyCommonOpenCvProperty(property, parameters);

            LineGaugeTool tool = new LineGaugeTool();
            tool.SetProperty(property);
            return tool;
        }

        private static IVisionTool CreateMatchingTool(IDictionary<string, string> parameters)
        {
            MatchingProperty property = new MatchingProperty(GetString(parameters, "Name", "PipelineMatching"))
            {
                MATCH_MODE = GetEnum(parameters, nameof(MatchingProperty.MATCH_MODE), TemplateMatchModes.CCoeffNormed),
                SCORE_MIN = GetDouble(parameters, nameof(MatchingProperty.SCORE_MIN), 0.6),
                MAGNIFIATION = GetDouble(parameters, nameof(MatchingProperty.MAGNIFIATION), 1),
                NUM_MATCH = GetInt(parameters, nameof(MatchingProperty.NUM_MATCH), 3),
                USE_FIND_ANGLE = GetBool(parameters, nameof(MatchingProperty.USE_FIND_ANGLE), true),
                FIND_ANGLE = GetDouble(parameters, nameof(MatchingProperty.FIND_ANGLE), 0.1),
                FIND_ANGLE_MAX = GetInt(parameters, nameof(MatchingProperty.FIND_ANGLE_MAX), 10),
                FIND_ANGLE_MIN = GetInt(parameters, nameof(MatchingProperty.FIND_ANGLE_MIN), -10),
                PATTERN_PATH = GetString(parameters, nameof(MatchingProperty.PATTERN_PATH), string.Empty),
                USE_CANNY = GetBool(parameters, nameof(MatchingProperty.USE_CANNY), false),
                CANNY_HIGH = GetInt(parameters, nameof(MatchingProperty.CANNY_HIGH), 60),
                CANNY_LOW = GetInt(parameters, nameof(MatchingProperty.CANNY_LOW), 30),
                USE_PADDING_COLOR_WHITE = GetBool(parameters, nameof(MatchingProperty.USE_PADDING_COLOR_WHITE), false)
            };

            ApplyCommonOpenCvProperty(property, parameters);

            string templatePath = GetString(parameters, "TemplatePath", property.PATTERN_PATH);
            if (!string.IsNullOrWhiteSpace(templatePath) && File.Exists(templatePath))
            {
                property.ImageTemplate = Cv2.ImRead(templatePath);
            }

            MatchingTool tool = new MatchingTool();
            tool.SetProperty(property);
            if (!Lib.OpenCV.OpenCvHelper.IsImageEmpty(property.ImageTemplate))
            {
                tool.SetTemplateImage(property.ImageTemplate);
            }

            return tool;
        }

        private static IVisionTool CreateMeanTool(IDictionary<string, string> parameters)
        {
            MeanProperty property = new MeanProperty(GetString(parameters, "Name", "PipelineMean"))
            {
                MEAN_MAX = GetInt(parameters, nameof(MeanProperty.MEAN_MAX), 240),
                MEAN_MIN = GetInt(parameters, nameof(MeanProperty.MEAN_MIN), 100),
                MEAN_TYPES = GetEnum(parameters, nameof(MeanProperty.MEAN_TYPES), MeanType.Mean)
            };

            ApplyCommonOpenCvProperty(property, parameters);

            MeanTool tool = new MeanTool();
            tool.SetProperty(property);
            return tool;
        }

        private static IVisionTool CreateFeatureMatchingTool(IDictionary<string, string> parameters)
        {
            FeatureMatchingProperty property = new FeatureMatchingProperty(GetString(parameters, "Name", "PipelineFeatureMatching"))
            {
                SCORE_MIN = GetDouble(parameters, nameof(FeatureMatchingProperty.SCORE_MIN), 0.6),
                RANSAC_REPROJ_THRESHOLD = GetDouble(parameters, nameof(FeatureMatchingProperty.RANSAC_REPROJ_THRESHOLD), 3),
                PATTERN_PATH = GetString(parameters, nameof(FeatureMatchingProperty.PATTERN_PATH), string.Empty)
            };

            ApplyCommonOpenCvProperty(property, parameters);

            SiftTool tool = new SiftTool();
            tool.SetProperty(property);

            string templatePath = GetString(parameters, "TemplatePath", property.PATTERN_PATH);
            if (!string.IsNullOrWhiteSpace(templatePath) && File.Exists(templatePath))
            {
                property.ImageTemplate = Cv2.ImRead(templatePath);
                tool.SetTemplateImage(property.ImageTemplate);
            }

            return tool;
        }

        private static void ApplyCommonOpenCvProperty(OpenVisionLab.Vision._1._Tools.OpenCV.OpenCvPropertyBase property, IDictionary<string, string> parameters)
        {
            property.PIXELPERMM = GetDouble(parameters, nameof(property.PIXELPERMM), property.PIXELPERMM);
            property.USE_THRESHOLD = GetBool(parameters, nameof(property.USE_THRESHOLD), property.USE_THRESHOLD);
            property.USE_BITWISENOT = GetBool(parameters, nameof(property.USE_BITWISENOT), property.USE_BITWISENOT);
            property.THRESHOLD_TYPES = GetEnum(parameters, nameof(property.THRESHOLD_TYPES), property.THRESHOLD_TYPES);
            property.THRESHOLD = GetDouble(parameters, nameof(property.THRESHOLD), property.THRESHOLD);
            property.USE_ADAPTIVE_THRESHOLD = GetBool(parameters, nameof(property.USE_ADAPTIVE_THRESHOLD), property.USE_ADAPTIVE_THRESHOLD);
            property.ADAPTIVE_THRESHOLD = GetDouble(parameters, nameof(property.ADAPTIVE_THRESHOLD), property.ADAPTIVE_THRESHOLD);
            property.ADAPTIVE_THRESHOLD_TYPES = GetEnum(parameters, nameof(property.ADAPTIVE_THRESHOLD_TYPES), property.ADAPTIVE_THRESHOLD_TYPES);
            property.ADAPTIVE_THRESHOLD_ALGORITHM = GetEnum(parameters, nameof(property.ADAPTIVE_THRESHOLD_ALGORITHM), property.ADAPTIVE_THRESHOLD_ALGORITHM);
            property.BlockSize = GetInt(parameters, nameof(property.BlockSize), property.BlockSize);
            property.Weight = GetInt(parameters, nameof(property.Weight), property.Weight);
            property.USE_ROI = GetBool(parameters, nameof(property.USE_ROI), property.USE_ROI);
            property.USE_MULTI_ROI = GetBool(parameters, nameof(property.USE_MULTI_ROI), property.USE_MULTI_ROI);
            property.CvROI = GetRect(parameters, nameof(property.CvROI), property.CvROI);
            property.CvROIS = GetRectList(parameters, nameof(property.CvROIS), property.CvROIS);
            property.CvMASKS = GetRectList(parameters, nameof(property.CvMASKS), property.CvMASKS);
        }

        private static string NormalizeToolType(string toolType)
        {
            string value = (toolType ?? string.Empty).Trim();
            if (value.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 4);
            }

            return value.Replace(" ", string.Empty).Replace("_", string.Empty).ToLowerInvariant();
        }

        private static string GetValue(IDictionary<string, string> parameters, string key)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            foreach (KeyValuePair<string, string> item in parameters)
            {
                if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return item.Value;
                }
            }

            return null;
        }

        private static string GetString(IDictionary<string, string> parameters, string key, string defaultValue)
        {
            string value = GetValue(parameters, key);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private static int GetInt(IDictionary<string, string> parameters, string key, int defaultValue)
        {
            string value = GetValue(parameters, key);
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
                ? result
                : defaultValue;
        }

        private static double GetDouble(IDictionary<string, string> parameters, string key, double defaultValue)
        {
            string value = GetValue(parameters, key);
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result)
                ? result
                : defaultValue;
        }

        private static bool GetBool(IDictionary<string, string> parameters, string key, bool defaultValue)
        {
            string value = GetValue(parameters, key);
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        private static TEnum GetEnum<TEnum>(IDictionary<string, string> parameters, string key, TEnum defaultValue)
            where TEnum : struct
        {
            string value = GetValue(parameters, key);
            return Enum.TryParse(value, true, out TEnum result) ? result : defaultValue;
        }

        private static Rect GetRect(IDictionary<string, string> parameters, string key, Rect defaultValue)
        {
            string value = GetValue(parameters, key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            string[] parts = value.Split(',');
            if (parts.Length != 4)
            {
                return defaultValue;
            }

            return int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)
                && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)
                && int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int width)
                && int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int height)
                ? new Rect(x, y, width, height)
                : defaultValue;
        }

        private static List<Rect> GetRectList(IDictionary<string, string> parameters, string key, List<Rect> defaultValue)
        {
            string value = GetValue(parameters, key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue ?? new List<Rect>();
            }

            return value
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => GetRect(new Dictionary<string, string> { [key] = part }, key, default))
                .ToList();
        }
    }
}
