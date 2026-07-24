using Lib.OpenCV.Pipeline;
using System;
using System.Globalization;
using System.IO;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipeHybridRelativeRoiIntentSkill
    {
        public const string SupportedMeasurementDefinition = "Dark-band thickness in a locator-aligned ROI";
        public const string SupportedUnitMode = "px";
        public const string FrameName = "LocatorFrame";
        public const double DefaultScoreMinimum = 0.8D;
        public const double DefaultScoreMargin = 10D;
        public const double DefaultAngleMinimum = -5D;
        public const double DefaultAngleMaximum = 5D;
        public const double DefaultScaleRatioMinimum = 0.8D;
        public const double DefaultScaleRatioMaximum = 1.8D;
        public const double DefaultMinimumValidPixelRatio = 0.25D;

        internal readonly struct ReferencePose
        {
            internal ReferencePose(double x, double y, double angle, double scale, int imageWidth, int imageHeight)
            {
                X = x;
                Y = y;
                Angle = angle;
                Scale = scale;
                ImageWidth = imageWidth;
                ImageHeight = imageHeight;
            }

            internal double X { get; }

            internal double Y { get; }

            internal double Angle { get; }

            internal double Scale { get; }

            internal int ImageWidth { get; }

            internal int ImageHeight { get; }
        }

        internal static bool TryParseReferencePose(string text, out ReferencePose pose, out string message)
        {
            pose = default;
            string[] parts = (text ?? string.Empty).Split(',');
            if (parts.Length != 6
                || !TryParseFinite(parts[0], out double x)
                || !TryParseFinite(parts[1], out double y)
                || !TryParseFinite(parts[2], out double angle)
                || !TryParseFinite(parts[3], out double scale)
                || !int.TryParse(parts[4].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int imageWidth)
                || !int.TryParse(parts[5].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int imageHeight))
            {
                message = "Reference pose must be x,y,angle,scale,imageWidth,imageHeight.";
                return false;
            }

            if (scale <= 0D || imageWidth <= 0 || imageHeight <= 0)
            {
                message = "Reference scale and image dimensions must be positive.";
                return false;
            }

            if (x < 0D || x >= imageWidth || y < 0D || y >= imageHeight)
            {
                message = "Reference center must be inside the reference image.";
                return false;
            }

            pose = new ReferencePose(x, y, angle, scale, imageWidth, imageHeight);
            message = string.Empty;
            return true;
        }

        internal static bool TryValidateInputs(
            string locatorTemplatePath,
            string searchRoiText,
            string measurementRoiText,
            string referencePoseText,
            string scoreMinimumText,
            string scoreMarginText,
            string angleMinimumText,
            string angleMaximumText,
            string scaleRatioMinimumText,
            string scaleRatioMaximumText,
            string minimumValidPixelRatioText,
            out OpenVisionRecipePinGapIntentSkill.RoiSample searchRoi,
            out OpenVisionRecipePinGapIntentSkill.RoiSample measurementRoi,
            out ReferencePose referencePose,
            out double scoreMinimum,
            out double scoreMargin,
            out double angleMinimum,
            out double angleMaximum,
            out double scaleRatioMinimum,
            out double scaleRatioMaximum,
            out double minimumValidPixelRatio,
            out string message)
        {
            searchRoi = default;
            measurementRoi = default;
            referencePose = default;
            scoreMinimum = 0D;
            scoreMargin = 0D;
            angleMinimum = 0D;
            angleMaximum = 0D;
            scaleRatioMinimum = 0D;
            scaleRatioMaximum = 0D;
            minimumValidPixelRatio = 0D;

            string templatePath = (locatorTemplatePath ?? string.Empty).Trim();
            if (templatePath.Length == 0 || !File.Exists(templatePath))
            {
                message = "An existing cropped locator template is required.";
                return false;
            }

            if (!TryParseSingleRoi(searchRoiText, out searchRoi, out message)
                || !TryParseSingleRoi(measurementRoiText, out measurementRoi, out message)
                || !TryParseReferencePose(referencePoseText, out referencePose, out message))
            {
                return false;
            }

            if (!IsInsideReference(searchRoi, referencePose)
                || !IsInsideReference(measurementRoi, referencePose))
            {
                message = "Search and measurement ROIs must be fully inside the reference image.";
                return false;
            }

            if (!TryParseFinite(scoreMinimumText, out scoreMinimum) || scoreMinimum < 0D || scoreMinimum > 1D)
            {
                message = "SCORE_MIN must be between 0 and 1.";
                return false;
            }

            if (!TryParseFinite(scoreMarginText, out scoreMargin) || scoreMargin <= 0D || scoreMargin > 100D)
            {
                message = "ScoreMargin minimum must be greater than 0 and at most 100 percentage points.";
                return false;
            }

            if (!TryParseFinite(angleMinimumText, out angleMinimum)
                || !TryParseFinite(angleMaximumText, out angleMaximum)
                || angleMinimum > angleMaximum
                || referencePose.Angle < angleMinimum
                || referencePose.Angle > angleMaximum)
            {
                message = "Angle minimum/maximum must be ordered and contain the reference angle.";
                return false;
            }

            if (!TryParseFinite(scaleRatioMinimumText, out scaleRatioMinimum)
                || !TryParseFinite(scaleRatioMaximumText, out scaleRatioMaximum)
                || scaleRatioMinimum <= 0D
                || scaleRatioMinimum > 1D
                || scaleRatioMaximum < 1D
                || scaleRatioMinimum > scaleRatioMaximum)
            {
                message = "Scale ratio limits must satisfy 0 < minimum <= 1 <= maximum.";
                return false;
            }

            if (!TryParseFinite(minimumValidPixelRatioText, out minimumValidPixelRatio)
                || minimumValidPixelRatio <= 0D
                || minimumValidPixelRatio > 1D)
            {
                message = "Minimum valid-pixel ratio must be greater than 0 and at most 1.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        internal static VisionPipeline CreateMeasurementPipeline(
            string locatorTemplatePath,
            OpenVisionRecipePinGapIntentSkill.RoiSample searchRoi,
            OpenVisionRecipePinGapIntentSkill.RoiSample measurementRoi,
            ReferencePose referencePose,
            double scoreMinimum,
            double scoreMargin,
            double angleMinimum,
            double angleMaximum,
            double scaleRatioMinimum,
            double scaleRatioMaximum,
            double minimumValidPixelRatio)
        {
            VisionPipeline pipeline = new VisionPipeline { Name = "Hybrid_Locator_Relative_DarkBand_Gap" };
            VisionPipelineStep ambiguity = CreateMatchingStep(
                "01 Reject Missing Or Ambiguous Locator",
                "LocatorCandidates",
                locatorTemplatePath,
                searchRoi,
                scoreMinimum,
                2,
                referencePose,
                angleMinimum,
                angleMaximum,
                scaleRatioMinimum,
                scaleRatioMaximum);
            ambiguity.UseAcceptance = true;
            ambiguity.ExpectedSuccess = true;
            ambiguity.AcceptanceMetricName = VisionPipelineKnownMetrics.ScoreMargin;
            ambiguity.UseAcceptanceMetricMinimum = true;
            ambiguity.AcceptanceMetricMinimum = scoreMargin;
            pipeline.Steps.Add(ambiguity);

            VisionPipelineStep fixture = CreateMatchingStep(
                "02 Publish Reviewed Locator Pose",
                "FixtureMatch",
                locatorTemplatePath,
                searchRoi,
                scoreMinimum,
                1,
                referencePose,
                angleMinimum,
                angleMaximum,
                scaleRatioMinimum,
                scaleRatioMaximum);
            fixture.Parameters["ALLOW_BRANCH_INPUT"] = "true";
            fixture.Parameters[VisionPipelineFixtureFrameService.PublishParameter] = "true";
            fixture.Parameters[VisionPipelineFixtureFrameService.FrameNameParameter] = FrameName;
            AddDouble(fixture, VisionPipelineFixtureFrameService.ReferenceXParameter, referencePose.X);
            AddDouble(fixture, VisionPipelineFixtureFrameService.ReferenceYParameter, referencePose.Y);
            AddDouble(fixture, VisionPipelineFixtureFrameService.ReferenceAngleParameter, referencePose.Angle);
            AddDouble(fixture, VisionPipelineFixtureFrameService.ReferenceScaleParameter, referencePose.Scale);
            AddDouble(
                fixture,
                VisionPipelineFixtureFrameService.MaximumAngleDeltaParameter,
                Math.Max(Math.Abs(referencePose.Angle - angleMinimum), Math.Abs(angleMaximum - referencePose.Angle)));
            AddDouble(fixture, VisionPipelineFixtureFrameService.MinimumScaleRatioParameter, scaleRatioMinimum);
            AddDouble(fixture, VisionPipelineFixtureFrameService.MaximumScaleRatioParameter, scaleRatioMaximum);
            fixture.Parameters[VisionPipelineFixtureFrameService.ReferenceImageWidthParameter] = referencePose.ImageWidth.ToString(CultureInfo.InvariantCulture);
            fixture.Parameters[VisionPipelineFixtureFrameService.ReferenceImageHeightParameter] = referencePose.ImageHeight.ToString(CultureInfo.InvariantCulture);
            pipeline.Steps.Add(fixture);

            VisionPipelineStep normalize = new VisionPipelineStep
            {
                Name = "03 Normalize To Reference Coordinates",
                ToolType = "RotateScale",
                Enabled = true,
                InputLayer = "Main",
                OutputLayer = "DeviceAligned",
                UseAcceptance = false,
                ExpectedSuccess = true
            };
            normalize.Parameters["Name"] = "LocatorNormalizeImage";
            normalize.Parameters["Angle"] = "0";
            normalize.Parameters["ScaleXPercent"] = "100";
            normalize.Parameters["ScaleYPercent"] = "100";
            normalize.Parameters["Interpolation"] = "Linear";
            normalize.Parameters["BorderType"] = "Constant";
            normalize.Parameters[VisionPipelineFixtureFrameService.ConsumeParameter] = "true";
            normalize.Parameters[VisionPipelineFixtureFrameService.FrameNameParameter] = FrameName;
            normalize.Parameters[VisionPipelineFixtureFrameService.ApplyModeParameter] = VisionPipelineFixtureApplyMode.NormalizeImage.ToString();
            AddDouble(normalize, VisionPipelineFixtureFrameService.MinimumValidPixelRatioParameter, minimumValidPixelRatio);
            normalize.Parameters["ALLOW_BRANCH_INPUT"] = "true";
            pipeline.Steps.Add(normalize);

            VisionPipelineStep measurement = OpenVisionRecipeDarkBandGapIntentSkill
                .CreateMeasurementPipeline(measurementRoi)
                .Steps[0];
            measurement.Name = "04 Measure Locator-Aligned Dark Band Gap";
            measurement.InputLayer = "DeviceAligned";
            measurement.OutputLayer = "GapMeasured";
            measurement.Parameters["Name"] = "LocatorAlignedDarkBandGap";
            pipeline.Steps.Add(measurement);
            return pipeline;
        }

        private static VisionPipelineStep CreateMatchingStep(
            string name,
            string outputLayer,
            string locatorTemplatePath,
            OpenVisionRecipePinGapIntentSkill.RoiSample searchRoi,
            double scoreMinimum,
            int matchCount,
            ReferencePose referencePose,
            double angleMinimum,
            double angleMaximum,
            double scaleRatioMinimum,
            double scaleRatioMaximum)
        {
            VisionPipelineStep step = new VisionPipelineStep
            {
                Name = name,
                ToolType = "Matching",
                Enabled = true,
                InputLayer = "Main",
                OutputLayer = outputLayer,
                UseAcceptance = false,
                ExpectedSuccess = true
            };
            string templatePath = locatorTemplatePath.Trim();
            step.Parameters["Name"] = "RelativeRoiLocator";
            step.Parameters["TemplatePath"] = templatePath;
            step.Parameters["PATTERN_PATH"] = templatePath;
            step.Parameters["MATCH_MODE"] = "CCoeffNormed";
            AddDouble(step, "SCORE_MIN", scoreMinimum);
            step.Parameters["MAGNIFIATION"] = "1";
            step.Parameters["NUM_MATCH"] = matchCount.ToString(CultureInfo.InvariantCulture);
            step.Parameters["USE_FIND_ANGLE"] = "true";
            AddDouble(step, "FIND_ANGLE_MIN", angleMinimum);
            AddDouble(step, "FIND_ANGLE_MAX", angleMaximum);
            step.Parameters["FIND_ANGLE"] = "1";
            step.Parameters["USE_COARSE_TO_FINE_ANGLE_SEARCH"] = "false";
            step.Parameters["USE_FIND_SCALE"] = "true";
            AddDouble(step, "FIND_SCALE_MIN", referencePose.Scale * scaleRatioMinimum);
            AddDouble(step, "FIND_SCALE_MAX", referencePose.Scale * scaleRatioMaximum);
            step.Parameters["FIND_SCALE_STEP"] = "0.1";
            step.Parameters["USE_PYRAMID_POSITION_PROPOSAL"] = "false";
            step.Parameters["USE_CANNY"] = "false";
            step.Parameters["USE_THRESHOLD"] = "false";
            step.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
            step.Parameters["USE_ROI"] = "true";
            step.Parameters["USE_MULTI_ROI"] = "false";
            step.Parameters["CvROI"] = searchRoi.ToText();
            step.Parameters["USE_PADDING_COLOR_WHITE"] = "true";
            step.Parameters[VisionPipelineFixtureFrameService.PublishParameter] = "false";
            return step;
        }

        private static bool TryParseSingleRoi(
            string text,
            out OpenVisionRecipePinGapIntentSkill.RoiSample roi,
            out string message)
        {
            roi = default;
            if (!OpenVisionRecipePinGapIntentSkill.TryParseRoiSamples(
                    text,
                    out System.Collections.Generic.IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> rois,
                    out message))
            {
                return false;
            }

            if (rois.Count != 1)
            {
                message = "Exactly one x,y,w,h ROI is required.";
                return false;
            }

            roi = rois[0];
            return true;
        }

        private static bool IsInsideReference(
            OpenVisionRecipePinGapIntentSkill.RoiSample roi,
            ReferencePose pose)
        {
            return roi.X >= 0
                && roi.Y >= 0
                && roi.Width > 0
                && roi.Height > 0
                && roi.X < pose.ImageWidth
                && roi.Y < pose.ImageHeight
                && roi.Width <= pose.ImageWidth - roi.X
                && roi.Height <= pose.ImageHeight - roi.Y;
        }

        private static bool TryParseFinite(string text, out double value)
        {
            return double.TryParse(
                    (text ?? string.Empty).Trim(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out value)
                && !double.IsNaN(value)
                && !double.IsInfinity(value);
        }

        private static void AddDouble(VisionPipelineStep step, string key, double value)
        {
            step.Parameters[key] = value.ToString("0.######", CultureInfo.InvariantCulture);
        }
    }
}
