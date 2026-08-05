using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Blob;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Property;
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

            if (property is AffineTransformProperty affineTransform)
            {
                return FromAffineTransformProperty(
                    affineTransform,
                    string.IsNullOrWhiteSpace(affineTransform.NAME) ? "AffineTransform" : affineTransform.NAME,
                    inputLayer,
                    outputLayer);
            }

            VisionPipelineStep step = CreateStep(property.NAME, GetToolType(property), inputLayer, outputLayer);
            AddCommonOpenCvParameters(step.Parameters, property);

            if (property is BlobProperty blob)
            {
                Add(step.Parameters, nameof(BlobProperty.MIN_AREA), blob.MIN_AREA);
                Add(step.Parameters, nameof(BlobProperty.MAX_AREA), blob.MAX_AREA);
                Add(step.Parameters, nameof(BlobProperty.MIN_WIDTH), blob.MIN_WIDTH);
                Add(step.Parameters, nameof(BlobProperty.MAX_WIDTH), blob.MAX_WIDTH);
                Add(step.Parameters, nameof(BlobProperty.MIN_HEIGHT), blob.MIN_HEIGHT);
                Add(step.Parameters, nameof(BlobProperty.MAX_HEIGHT), blob.MAX_HEIGHT);
            }
            else if (property is ContourProperty contour)
            {
                Add(step.Parameters, nameof(ContourProperty.USE_APPROXPOLYDP), contour.USE_APPROXPOLYDP);
                Add(step.Parameters, nameof(ContourProperty.USE_DRAW_IMAGE), contour.USE_DRAW_IMAGE);
                Add(step.Parameters, nameof(ContourProperty.DrawMode), contour.DrawMode);
                Add(step.Parameters, nameof(ContourProperty.ApproximationModes), contour.ApproximationModes);
                Add(step.Parameters, nameof(ContourProperty.DetectMode), contour.DetectMode);
                Add(step.Parameters, nameof(ContourProperty.EPSILON), contour.EPSILON);
                Add(step.Parameters, nameof(ContourProperty.MIN_AREA), contour.MIN_AREA);
                Add(step.Parameters, nameof(ContourProperty.MAX_AREA), contour.MAX_AREA);
                Add(step.Parameters, nameof(ContourProperty.MIN_WIDTH), contour.MIN_WIDTH);
                Add(step.Parameters, nameof(ContourProperty.MAX_WIDTH), contour.MAX_WIDTH);
                Add(step.Parameters, nameof(ContourProperty.MIN_HEIGHT), contour.MIN_HEIGHT);
                Add(step.Parameters, nameof(ContourProperty.MAX_HEIGHT), contour.MAX_HEIGHT);
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
                Add(step.Parameters, nameof(MatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH), matching.USE_COARSE_TO_FINE_ANGLE_SEARCH);
                Add(step.Parameters, nameof(MatchingProperty.COARSE_ANGLE_STEP), matching.COARSE_ANGLE_STEP);
                Add(step.Parameters, nameof(MatchingProperty.COARSE_ANGLE_TOP_K), matching.COARSE_ANGLE_TOP_K);
                Add(step.Parameters, nameof(MatchingProperty.USE_FIND_SCALE), matching.USE_FIND_SCALE);
                Add(step.Parameters, nameof(MatchingProperty.FIND_SCALE_MIN), matching.FIND_SCALE_MIN);
                Add(step.Parameters, nameof(MatchingProperty.FIND_SCALE_MAX), matching.FIND_SCALE_MAX);
                Add(step.Parameters, nameof(MatchingProperty.FIND_SCALE_STEP), matching.FIND_SCALE_STEP);
                Add(step.Parameters, nameof(MatchingProperty.PATTERN_PATH), matching.PATTERN_PATH);
                Add(step.Parameters, "TemplatePath", matching.PATTERN_PATH);
                Add(step.Parameters, nameof(MatchingProperty.USE_CANNY), matching.USE_CANNY);
                Add(step.Parameters, nameof(MatchingProperty.CANNY_HIGH), matching.CANNY_HIGH);
                Add(step.Parameters, nameof(MatchingProperty.CANNY_LOW), matching.CANNY_LOW);
                Add(step.Parameters, nameof(MatchingProperty.USE_PADDING_COLOR_WHITE), matching.USE_PADDING_COLOR_WHITE);
            }
            else if (property is EdgeBasedMatchingProperty edgeMatching)
            {
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.SCORE_MIN), edgeMatching.SCORE_MIN);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.NUM_MATCH), edgeMatching.NUM_MATCH);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_UNIQUE_MATCH_VALIDATION), edgeMatching.USE_UNIQUE_MATCH_VALIDATION);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.UNIQUE_MATCH_MIN_SCORE_MARGIN), edgeMatching.UNIQUE_MATCH_MIN_SCORE_MARGIN);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.ALLOW_GLOBAL_POLARITY_REVERSAL), edgeMatching.ALLOW_GLOBAL_POLARITY_REVERSAL);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.PATTERN_PATH), edgeMatching.PATTERN_PATH);
                Add(step.Parameters, "TemplatePath", edgeMatching.PATTERN_PATH);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_FIND_ANGLE), edgeMatching.USE_FIND_ANGLE);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_ANGLE), edgeMatching.FIND_ANGLE);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_ANGLE_MAX), edgeMatching.FIND_ANGLE_MAX);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_ANGLE_MIN), edgeMatching.FIND_ANGLE_MIN);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH), edgeMatching.USE_COARSE_TO_FINE_ANGLE_SEARCH);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.COARSE_ANGLE_STEP), edgeMatching.COARSE_ANGLE_STEP);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.COARSE_ANGLE_TOP_K), edgeMatching.COARSE_ANGLE_TOP_K);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_FIND_SCALE), edgeMatching.USE_FIND_SCALE);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_SCALE_MIN), edgeMatching.FIND_SCALE_MIN);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_SCALE_MAX), edgeMatching.FIND_SCALE_MAX);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.FIND_SCALE_STEP), edgeMatching.FIND_SCALE_STEP);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.CANNY_LOW), edgeMatching.CANNY_LOW);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.CANNY_HIGH), edgeMatching.CANNY_HIGH);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.CANNY_APERTURE_SIZE), edgeMatching.CANNY_APERTURE_SIZE);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_L2_GRADIENT), edgeMatching.USE_L2_GRADIENT);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.CONTOUR_RETRIEVAL_MODE), edgeMatching.CONTOUR_RETRIEVAL_MODE);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.CONTOUR_APPROXIMATION_MODE), edgeMatching.CONTOUR_APPROXIMATION_MODE);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.GREEDINESS), edgeMatching.GREEDINESS);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.SEARCH_STEP), edgeMatching.SEARCH_STEP);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_POSITION_REFINE), edgeMatching.USE_POSITION_REFINE);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_SUBPIXEL_REFINE), edgeMatching.USE_SUBPIXEL_REFINE);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_PYRAMID_POSITION_PROPOSAL), edgeMatching.USE_PYRAMID_POSITION_PROPOSAL);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.PYRAMID_POSITION_TOP_N), edgeMatching.PYRAMID_POSITION_TOP_N);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.PYRAMID_POSITION_MIN_SCORE), edgeMatching.PYRAMID_POSITION_MIN_SCORE);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_HYBRID_VERIFY), edgeMatching.USE_HYBRID_VERIFY);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.HYBRID_VERIFY_TOP_N), edgeMatching.HYBRID_VERIFY_TOP_N);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.HYBRID_VERIFY_IMAGE_WEIGHT), edgeMatching.HYBRID_VERIFY_IMAGE_WEIGHT);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.MAX_TEMPLATE_POINTS), edgeMatching.MAX_TEMPLATE_POINTS);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.MIN_GRADIENT_MAGNITUDE), edgeMatching.MIN_GRADIENT_MAGNITUDE);
                Add(step.Parameters, nameof(EdgeBasedMatchingProperty.USE_DRAW_IMAGE), edgeMatching.USE_DRAW_IMAGE);
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

        public static VisionPipelineStep FromLineGaugePair(
            string name,
            string toolType,
            LineGaugeProperty left,
            LineGaugeProperty right,
            string inputLayer,
            string outputLayer,
            string purpose)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            VisionPipelineStep step = CreateStep(name, string.IsNullOrWhiteSpace(toolType) ? "LineDistance" : toolType, inputLayer, outputLayer);
            Add(step.Parameters, "LinePurpose", string.IsNullOrWhiteSpace(purpose) ? toolType : purpose);
            AddPrefixedLineGaugeParameters(step.Parameters, "Left", left);
            AddPrefixedLineGaugeParameters(step.Parameters, "Right", right);
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

        public static VisionPipelineStep FromRotateScaleProperty(RotateScaleToolProperty property, string name, string inputLayer, string outputLayer)
        {
            if (property == null) { throw new ArgumentNullException(nameof(property)); }

            VisionPipelineStep step = CreateStep(name, "RotateScale", inputLayer, outputLayer);
            Add(step.Parameters, nameof(RotateScaleToolProperty.Angle), property.Angle);
            Add(step.Parameters, nameof(RotateScaleToolProperty.ScaleXPercent), property.ScaleXPercent);
            Add(step.Parameters, nameof(RotateScaleToolProperty.ScaleYPercent), property.ScaleYPercent);
            Add(step.Parameters, nameof(RotateScaleToolProperty.Interpolation), property.Interpolation);
            Add(step.Parameters, nameof(RotateScaleToolProperty.BorderType), property.BorderType);
            return step;
        }

        public static VisionPipelineStep FromAffineTransformProperty(
            IAffineTransformToolProperty property,
            string name,
            string inputLayer,
            string outputLayer)
        {
            if (property == null) { throw new ArgumentNullException(nameof(property)); }

            VisionPipelineStep step = CreateStep(name, "AffineTransform", inputLayer, outputLayer);
            Add(step.Parameters, nameof(property.SourcePoint1X), property.SourcePoint1X);
            Add(step.Parameters, nameof(property.SourcePoint1Y), property.SourcePoint1Y);
            Add(step.Parameters, nameof(property.SourcePoint2X), property.SourcePoint2X);
            Add(step.Parameters, nameof(property.SourcePoint2Y), property.SourcePoint2Y);
            Add(step.Parameters, nameof(property.SourcePoint3X), property.SourcePoint3X);
            Add(step.Parameters, nameof(property.SourcePoint3Y), property.SourcePoint3Y);
            Add(step.Parameters, nameof(property.DestinationPoint1X), property.DestinationPoint1X);
            Add(step.Parameters, nameof(property.DestinationPoint1Y), property.DestinationPoint1Y);
            Add(step.Parameters, nameof(property.DestinationPoint2X), property.DestinationPoint2X);
            Add(step.Parameters, nameof(property.DestinationPoint2Y), property.DestinationPoint2Y);
            Add(step.Parameters, nameof(property.DestinationPoint3X), property.DestinationPoint3X);
            Add(step.Parameters, nameof(property.DestinationPoint3Y), property.DestinationPoint3Y);
            Add(step.Parameters, nameof(property.OutputWidth), property.OutputWidth);
            Add(step.Parameters, nameof(property.OutputHeight), property.OutputHeight);
            Add(step.Parameters, nameof(property.Interpolation), property.Interpolation);
            Add(step.Parameters, nameof(property.BorderType), property.BorderType);
            Add(step.Parameters, nameof(property.BorderValue), property.BorderValue);
            Add(step.Parameters, nameof(property.MinimumSourceTriangleArea), property.MinimumSourceTriangleArea);
            Add(step.Parameters, nameof(property.MinimumDestinationTriangleArea), property.MinimumDestinationTriangleArea);
            Add(step.Parameters, nameof(property.MinimumValidPixelRatio), property.MinimumValidPixelRatio);
            return step;
        }

        public static VisionPipelineStep FromArithmetic(
            string name,
            string operation,
            string inputLayerA,
            string inputLayerB,
            string outputLayer,
            bool useConstantInput,
            bool useColorConstant,
            int gray,
            int b,
            int g,
            int r,
            int offsetX,
            int offsetY,
            string mode = VisionPipelineArithmeticStep.ModeOperation)
        {
            VisionPipelineStep step = CreateStep(name, VisionPipelineArithmeticStep.ToolType, inputLayerA, outputLayer);
            Add(step.Parameters, VisionPipelineArithmeticStep.ParameterMode, string.IsNullOrWhiteSpace(mode) ? VisionPipelineArithmeticStep.ModeOperation : mode);
            Add(step.Parameters, VisionPipelineArithmeticStep.ParameterOperation, string.IsNullOrWhiteSpace(operation) ? "Bitwise_AND" : operation);
            Add(step.Parameters, VisionPipelineArithmeticStep.ParameterInputLayerB, inputLayerB ?? string.Empty);
            Add(step.Parameters, VisionPipelineArithmeticStep.ParameterUseConstantInput, useConstantInput);
            Add(step.Parameters, VisionPipelineArithmeticStep.ParameterUseColorConstant, useColorConstant);
            Add(step.Parameters, VisionPipelineArithmeticStep.ParameterGray, gray);
            Add(step.Parameters, VisionPipelineArithmeticStep.ParameterB, b);
            Add(step.Parameters, VisionPipelineArithmeticStep.ParameterG, g);
            Add(step.Parameters, VisionPipelineArithmeticStep.ParameterR, r);
            Add(step.Parameters, VisionPipelineArithmeticStep.ParameterOffsetX, offsetX);
            Add(step.Parameters, VisionPipelineArithmeticStep.ParameterOffsetY, offsetY);
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
            Add(parameters, nameof(property.USE_MASKING), property.USE_MASKING || property.CvMASKS?.Count > 0);
            Add(parameters, nameof(property.CvROI), RectToText(property.CvROI));
            Add(parameters, nameof(property.CvROIS), RectListToText(property.CvROIS));
            Add(parameters, nameof(property.CvMASKS), RectListToText(property.CvMASKS));
        }

        private static void AddPrefixedCommonOpenCvParameters(IDictionary<string, string> parameters, string prefix, OpenCvPropertyBase property)
        {
            Add(parameters, prefix + "Name", property.NAME);
            Add(parameters, prefix + nameof(property.PIXELPERMM), property.PIXELPERMM);
            Add(parameters, prefix + nameof(property.USE_THRESHOLD), property.USE_THRESHOLD);
            Add(parameters, prefix + nameof(property.USE_BITWISENOT), property.USE_BITWISENOT);
            Add(parameters, prefix + nameof(property.THRESHOLD_TYPES), property.THRESHOLD_TYPES);
            Add(parameters, prefix + nameof(property.THRESHOLD), property.THRESHOLD);
            Add(parameters, prefix + nameof(property.USE_ADAPTIVE_THRESHOLD), property.USE_ADAPTIVE_THRESHOLD);
            Add(parameters, prefix + nameof(property.ADAPTIVE_THRESHOLD), property.ADAPTIVE_THRESHOLD);
            Add(parameters, prefix + nameof(property.ADAPTIVE_THRESHOLD_TYPES), property.ADAPTIVE_THRESHOLD_TYPES);
            Add(parameters, prefix + nameof(property.ADAPTIVE_THRESHOLD_ALGORITHM), property.ADAPTIVE_THRESHOLD_ALGORITHM);
            Add(parameters, prefix + nameof(property.BlockSize), property.BlockSize);
            Add(parameters, prefix + nameof(property.Weight), property.Weight);
            Add(parameters, prefix + nameof(property.USE_ROI), property.USE_ROI);
            Add(parameters, prefix + nameof(property.USE_MULTI_ROI), property.USE_MULTI_ROI);
            Add(parameters, prefix + nameof(property.USE_MASKING), property.USE_MASKING || property.CvMASKS?.Count > 0);
            Add(parameters, prefix + nameof(property.CvROI), RectToText(property.CvROI));
            Add(parameters, prefix + nameof(property.CvROIS), RectListToText(property.CvROIS));
            Add(parameters, prefix + nameof(property.CvMASKS), RectListToText(property.CvMASKS));
        }

        private static void AddPrefixedLineGaugeParameters(IDictionary<string, string> parameters, string prefix, LineGaugeProperty line)
        {
            AddPrefixedCommonOpenCvParameters(parameters, prefix, line);
            Add(parameters, prefix + nameof(LineGaugeProperty.PRJ_PORALITY), line.PRJ_PORALITY);
            Add(parameters, prefix + nameof(LineGaugeProperty.PRJ_DIR), line.PRJ_DIR);
            Add(parameters, prefix + nameof(LineGaugeProperty.CONTRAST), line.CONTRAST);
            Add(parameters, prefix + nameof(LineGaugeProperty.THICKNESS), line.THICKNESS);
            Add(parameters, prefix + nameof(LineGaugeProperty.SAMPLING_STEP), line.SAMPLING_STEP);
            Add(parameters, prefix + nameof(LineGaugeProperty.VER_PRJ_DIR), line.VER_PRJ_DIR);
            Add(parameters, prefix + nameof(LineGaugeProperty.POINT_RANGE), line.POINT_RANGE);
            Add(parameters, prefix + nameof(LineGaugeProperty.USE_MANUAL_ANGLE), line.USE_MANUAL_ANGLE);
            Add(parameters, prefix + nameof(LineGaugeProperty.MANUAL_ANGLE_VALUE), line.MANUAL_ANGLE_VALUE);
            Add(parameters, prefix + nameof(LineGaugeProperty.USE_EXTEND_FIT_LINE), line.USE_EXTEND_FIT_LINE);
            Add(parameters, prefix + nameof(LineGaugeProperty.EXTEND_FIT_LINE_VALUE), line.EXTEND_FIT_LINE_VALUE);
            Add(parameters, prefix + nameof(LineGaugeProperty.AVERAGE_Diff), line.AVERAGE_Diff);
            Add(parameters, prefix + nameof(LineGaugeProperty.USE_AVERAGE_FILTER), line.USE_AVERAGE_FILTER);
            Add(parameters, prefix + nameof(LineGaugeProperty.AVERAGE_FILTER_TYPE), line.AVERAGE_FILTER_TYPE);
            Add(parameters, prefix + nameof(LineGaugeProperty.SHOW_VERTICAL_LINE), line.SHOW_VERTICAL_LINE);
            Add(parameters, prefix + nameof(LineGaugeProperty.SHOW_EDGE), line.SHOW_EDGE);
            Add(parameters, prefix + nameof(LineGaugeProperty.SHOW_CONTOUR), line.SHOW_CONTOUR);
            Add(parameters, prefix + nameof(LineGaugeProperty.SHOW_FITLINE), line.SHOW_FITLINE);
        }

        private static string GetToolType(OpenCvPropertyBase property)
        {
            if (property is BlobProperty) { return "Blob"; }
            if (property is ContourProperty) { return "Contour"; }
            if (property is LineGaugeProperty) { return "LineGauge"; }
            if (property is MatchingProperty) { return "Matching"; }
            if (property is EdgeBasedMatchingProperty) { return "EdgeBasedMatching"; }
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
