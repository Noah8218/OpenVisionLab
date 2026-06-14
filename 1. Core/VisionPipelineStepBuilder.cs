using Lib.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal static class VisionPipelineStepBuilder
    {
        public static VisionPipelineStep FromProperty(OpenCvPropertyBase property, string inputLayer, string outputLayer)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            VisionPipelineStep step = CreateStep(property.NAME, GetToolType(property), inputLayer, outputLayer);
            AddCommonOpenCvParameters(step.Parameters, property);

            if (property is BlobProperty blob)
            {
                Add(step.Parameters, nameof(BlobProperty.MIN_AREA), blob.MIN_AREA);
                Add(step.Parameters, nameof(BlobProperty.MAX_AREA), blob.MAX_AREA);
            }
            else if (property is ContourProperty contour)
            {
                Add(step.Parameters, nameof(ContourProperty.USE_APPROXPOLYDP), contour.USE_APPROXPOLYDP);
                Add(step.Parameters, nameof(ContourProperty.USE_DRAW_IMAGE), contour.USE_DRAW_IMAGE);
                Add(step.Parameters, nameof(ContourProperty.ApproximationModes), contour.ApproximationModes);
                Add(step.Parameters, nameof(ContourProperty.DetectMode), contour.DetectMode);
                Add(step.Parameters, nameof(ContourProperty.EPSILON), contour.EPSILON);
                Add(step.Parameters, nameof(ContourProperty.MIN_AREA), contour.MIN_AREA);
                Add(step.Parameters, nameof(ContourProperty.MAX_AREA), contour.MAX_AREA);
                Add(step.Parameters, nameof(ContourProperty.ClrGridHtml), contour.ClrGridHtml);
                Add(step.Parameters, nameof(ContourProperty.DrawThickness), contour.DrawThickness);
            }
            else if (property is LineGaugeProperty line)
            {
                Add(step.Parameters, nameof(LineGaugeProperty.PRJ_PORALITY), line.PRJ_PORALITY);
                Add(step.Parameters, nameof(LineGaugeProperty.PRJ_DIR), line.PRJ_DIR);
                Add(step.Parameters, nameof(LineGaugeProperty.CONTRAST), line.CONTRAST);
                Add(step.Parameters, nameof(LineGaugeProperty.THICKNESS), line.THICKNESS);
                Add(step.Parameters, nameof(LineGaugeProperty.SAMPLING_STEP), line.SAMPLING_STEP);
                Add(step.Parameters, nameof(LineGaugeProperty.VER_PRJ_DIR), line.VER_PRJ_DIR);
                Add(step.Parameters, nameof(LineGaugeProperty.POINT_RANGE), line.POINT_RANGE);
                Add(step.Parameters, nameof(LineGaugeProperty.USE_MANUAL_ANGLE), line.USE_MANUAL_ANGLE);
                Add(step.Parameters, nameof(LineGaugeProperty.MANUAL_ANGLE_VALUE), line.MANUAL_ANGLE_VALUE);
                Add(step.Parameters, nameof(LineGaugeProperty.USE_EXTEND_FIT_LINE), line.USE_EXTEND_FIT_LINE);
                Add(step.Parameters, nameof(LineGaugeProperty.EXTEND_FIT_LINE_VALUE), line.EXTEND_FIT_LINE_VALUE);
                Add(step.Parameters, nameof(LineGaugeProperty.AVERAGE_Diff), line.AVERAGE_Diff);
                Add(step.Parameters, nameof(LineGaugeProperty.USE_AVERAGE_FILTER), line.USE_AVERAGE_FILTER);
                Add(step.Parameters, nameof(LineGaugeProperty.AVERAGE_FILTER_TYPE), line.AVERAGE_FILTER_TYPE);
                Add(step.Parameters, nameof(LineGaugeProperty.SHOW_VERTICAL_LINE), line.SHOW_VERTICAL_LINE);
                Add(step.Parameters, nameof(LineGaugeProperty.SHOW_EDGE), line.SHOW_EDGE);
                Add(step.Parameters, nameof(LineGaugeProperty.SHOW_CONTOUR), line.SHOW_CONTOUR);
                Add(step.Parameters, nameof(LineGaugeProperty.SHOW_FITLINE), line.SHOW_FITLINE);
            }
            else if (property is MatchingProperty matching)
            {
                Add(step.Parameters, nameof(MatchingProperty.MATCH_MODE), matching.MATCH_MODE);
                Add(step.Parameters, nameof(MatchingProperty.SCORE_MIN), matching.SCORE_MIN);
                Add(step.Parameters, nameof(MatchingProperty.MAGNIFIATION), matching.MAGNIFIATION);
                Add(step.Parameters, nameof(MatchingProperty.NUM_MATCH), matching.NUM_MATCH);
                Add(step.Parameters, nameof(MatchingProperty.USE_FIND_ANGLE), matching.USE_FIND_ANGLE);
                Add(step.Parameters, nameof(MatchingProperty.FIND_ANGLE), matching.FIND_ANGLE);
                Add(step.Parameters, nameof(MatchingProperty.FIND_ANGLE_MAX), matching.FIND_ANGLE_MAX);
                Add(step.Parameters, nameof(MatchingProperty.FIND_ANGLE_MIN), matching.FIND_ANGLE_MIN);
                Add(step.Parameters, nameof(MatchingProperty.PATTERN_PATH), matching.PATTERN_PATH);
                Add(step.Parameters, "TemplatePath", matching.PATTERN_PATH);
                Add(step.Parameters, nameof(MatchingProperty.USE_CANNY), matching.USE_CANNY);
                Add(step.Parameters, nameof(MatchingProperty.CANNY_HIGH), matching.CANNY_HIGH);
                Add(step.Parameters, nameof(MatchingProperty.CANNY_LOW), matching.CANNY_LOW);
                Add(step.Parameters, nameof(MatchingProperty.USE_PADDING_COLOR_WHITE), matching.USE_PADDING_COLOR_WHITE);
            }
            else if (property is MeanProperty mean)
            {
                Add(step.Parameters, nameof(MeanProperty.MEAN_MAX), mean.MEAN_MAX);
                Add(step.Parameters, nameof(MeanProperty.MEAN_MIN), mean.MEAN_MIN);
                Add(step.Parameters, nameof(MeanProperty.MEAN_TYPES), mean.MEAN_TYPES);
            }
            else if (property is FeatureMatchingProperty feature)
            {
                Add(step.Parameters, nameof(FeatureMatchingProperty.SCORE_MIN), feature.SCORE_MIN);
                Add(step.Parameters, nameof(FeatureMatchingProperty.RANSAC_REPROJ_THRESHOLD), feature.RANSAC_REPROJ_THRESHOLD);
                Add(step.Parameters, nameof(FeatureMatchingProperty.PATTERN_PATH), feature.PATTERN_PATH);
                Add(step.Parameters, "TemplatePath", feature.PATTERN_PATH);
            }

            return step;
        }

        public static VisionPipelineStep FromThresholdProperty(ThresholdToolProperty property, string name, string inputLayer, string outputLayer)
        {
            if (property == null) { throw new ArgumentNullException(nameof(property)); }

            VisionPipelineStep step = CreateStep(name, "Threshold", inputLayer, outputLayer);
            Add(step.Parameters, nameof(ThresholdToolProperty.Mode), property.Mode);
            Add(step.Parameters, nameof(ThresholdToolProperty.Threshold), property.Threshold);
            Add(step.Parameters, nameof(ThresholdToolProperty.MaxValue), property.MaxValue);
            Add(step.Parameters, nameof(ThresholdToolProperty.ThresholdType), property.ThresholdType);
            Add(step.Parameters, nameof(ThresholdToolProperty.RangeMin), property.RangeMin);
            Add(step.Parameters, nameof(ThresholdToolProperty.RangeMax), property.RangeMax);
            Add(step.Parameters, nameof(ThresholdToolProperty.Invert), property.Invert);
            Add(step.Parameters, nameof(ThresholdToolProperty.AdaptiveType), property.AdaptiveType);
            Add(step.Parameters, nameof(ThresholdToolProperty.AdaptiveThresholdType), property.AdaptiveThresholdType);
            Add(step.Parameters, nameof(ThresholdToolProperty.BlockSize), property.BlockSize);
            Add(step.Parameters, nameof(ThresholdToolProperty.Weight), property.Weight);
            return step;
        }

        public static VisionPipelineStep FromMorphologyProperty(MorphologyToolProperty property, string name, string inputLayer, string outputLayer)
        {
            if (property == null) { throw new ArgumentNullException(nameof(property)); }

            VisionPipelineStep step = CreateStep(name, "Morphology", inputLayer, outputLayer);
            Add(step.Parameters, nameof(MorphologyToolProperty.Shape), property.Shape);
            Add(step.Parameters, nameof(MorphologyToolProperty.Operator), property.Operator);
            Add(step.Parameters, nameof(MorphologyToolProperty.KernelWidth), property.KernelWidth);
            Add(step.Parameters, nameof(MorphologyToolProperty.KernelHeight), property.KernelHeight);
            Add(step.Parameters, nameof(MorphologyToolProperty.Iterations), property.Iterations);
            return step;
        }

        public static VisionPipelineStep FromFilterProperty(FilterToolProperty property, string name, string inputLayer, string outputLayer)
        {
            if (property == null) { throw new ArgumentNullException(nameof(property)); }

            VisionPipelineStep step = CreateStep(name, "Filter", inputLayer, outputLayer);
            Add(step.Parameters, nameof(FilterToolProperty.FilterType), property.FilterType);
            Add(step.Parameters, nameof(FilterToolProperty.KernelWidth), property.KernelWidth);
            Add(step.Parameters, nameof(FilterToolProperty.KernelHeight), property.KernelHeight);
            Add(step.Parameters, nameof(FilterToolProperty.MedianKernelSize), property.MedianKernelSize);
            Add(step.Parameters, nameof(FilterToolProperty.Diameter), property.Diameter);
            Add(step.Parameters, nameof(FilterToolProperty.SigmaColor), property.SigmaColor);
            Add(step.Parameters, nameof(FilterToolProperty.SigmaSpace), property.SigmaSpace);
            Add(step.Parameters, nameof(FilterToolProperty.BorderType), property.BorderType);
            return step;
        }

        public static VisionPipelineStep FromEdgeDetectionProperty(EdgeDetectionToolProperty property, string name, string inputLayer, string outputLayer)
        {
            if (property == null) { throw new ArgumentNullException(nameof(property)); }

            VisionPipelineStep step = CreateStep(name, "EdgeDetection", inputLayer, outputLayer);
            Add(step.Parameters, nameof(EdgeDetectionToolProperty.EdgeType), property.EdgeType);
            Add(step.Parameters, nameof(EdgeDetectionToolProperty.CannyThresholdLow), property.CannyThresholdLow);
            Add(step.Parameters, nameof(EdgeDetectionToolProperty.CannyThresholdHigh), property.CannyThresholdHigh);
            Add(step.Parameters, nameof(EdgeDetectionToolProperty.CannyApertureSize), property.CannyApertureSize);
            Add(step.Parameters, nameof(EdgeDetectionToolProperty.UseL2Gradient), property.UseL2Gradient);
            Add(step.Parameters, nameof(EdgeDetectionToolProperty.SobelDegreeX), property.SobelDegreeX);
            Add(step.Parameters, nameof(EdgeDetectionToolProperty.SobelDegreeY), property.SobelDegreeY);
            Add(step.Parameters, nameof(EdgeDetectionToolProperty.SobelKernelSize), property.SobelKernelSize);
            Add(step.Parameters, nameof(EdgeDetectionToolProperty.ScharrDegreeX), property.ScharrDegreeX);
            Add(step.Parameters, nameof(EdgeDetectionToolProperty.ScharrDegreeY), property.ScharrDegreeY);
            Add(step.Parameters, nameof(EdgeDetectionToolProperty.LaplacianKernelSize), property.LaplacianKernelSize);
            return step;
        }

        private static VisionPipelineStep CreateStep(string name, string toolType, string inputLayer, string outputLayer)
        {
            return new VisionPipelineStep
            {
                Name = string.IsNullOrWhiteSpace(name) ? toolType : name,
                ToolType = toolType,
                InputLayer = string.IsNullOrWhiteSpace(inputLayer) ? "Main" : inputLayer,
                OutputLayer = string.IsNullOrWhiteSpace(outputLayer) ? $"{toolType}_Output" : outputLayer
            };
        }

        private static void AddCommonOpenCvParameters(IDictionary<string, string> parameters, OpenCvPropertyBase property)
        {
            Add(parameters, "Name", property.NAME);
            Add(parameters, nameof(property.PIXELPERMM), property.PIXELPERMM);
            Add(parameters, nameof(property.USE_THRESHOLD), property.USE_THRESHOLD);
            Add(parameters, nameof(property.USE_BITWISENOT), property.USE_BITWISENOT);
            Add(parameters, nameof(property.THRESHOLD_TYPES), property.THRESHOLD_TYPES);
            Add(parameters, nameof(property.THRESHOLD), property.THRESHOLD);
            Add(parameters, nameof(property.USE_ADAPTIVE_THRESHOLD), property.USE_ADAPTIVE_THRESHOLD);
            Add(parameters, nameof(property.ADAPTIVE_THRESHOLD), property.ADAPTIVE_THRESHOLD);
            Add(parameters, nameof(property.ADAPTIVE_THRESHOLD_TYPES), property.ADAPTIVE_THRESHOLD_TYPES);
            Add(parameters, nameof(property.ADAPTIVE_THRESHOLD_ALGORITHM), property.ADAPTIVE_THRESHOLD_ALGORITHM);
            Add(parameters, nameof(property.BlockSize), property.BlockSize);
            Add(parameters, nameof(property.Weight), property.Weight);
            Add(parameters, nameof(property.USE_ROI), property.USE_ROI);
            Add(parameters, nameof(property.USE_MULTI_ROI), property.USE_MULTI_ROI);
            Add(parameters, nameof(property.CvROI), RectToText(property.CvROI));
            Add(parameters, nameof(property.CvROIS), RectListToText(property.CvROIS));
            Add(parameters, nameof(property.CvMASKS), RectListToText(property.CvMASKS));
        }

        private static string GetToolType(OpenCvPropertyBase property)
        {
            if (property is BlobProperty) { return "Blob"; }
            if (property is ContourProperty) { return "Contour"; }
            if (property is LineGaugeProperty) { return "LineGauge"; }
            if (property is MatchingProperty) { return "Matching"; }
            if (property is MeanProperty) { return "Mean"; }
            if (property is FeatureMatchingProperty) { return "FeatureMatching"; }
            return property.GetType().Name;
        }

        private static string RectToText(Rect rect)
        {
            return string.Join(
                ",",
                rect.X.ToString(CultureInfo.InvariantCulture),
                rect.Y.ToString(CultureInfo.InvariantCulture),
                rect.Width.ToString(CultureInfo.InvariantCulture),
                rect.Height.ToString(CultureInfo.InvariantCulture));
        }

        private static string RectListToText(IEnumerable<Rect> rects)
        {
            if (rects == null)
            {
                return string.Empty;
            }

            return string.Join(";", rects.Select(RectToText));
        }

        private static void Add(IDictionary<string, string> parameters, string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key)) { return; }

            parameters[key] = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }
}
