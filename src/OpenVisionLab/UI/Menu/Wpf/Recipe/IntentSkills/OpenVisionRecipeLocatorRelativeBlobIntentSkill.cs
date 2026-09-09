using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace OpenVisionLab
{
    // Evidence-constrained v1 skill: a reviewed locator pose normalizes the image,
    // then a fixed reference-coordinate ROI is inspected by Threshold + Blob.
    // The skill never accepts coordinates or tolerances invented by an LLM.
    internal static class OpenVisionRecipeLocatorRelativeBlobIntentSkill
    {
        internal const string SkillId = "locator-relative-blob-v1";
        internal const string PlanSchemaVersion = "locator-relative-blob-plan-v1";
        internal const string CoordinateFrame = "LocatorFrame";
        internal const string SupportedUnitMode = "px";
        internal const double DefaultScoreMinimum = OpenVisionRecipeHybridRelativeRoiIntentSkill.DefaultScoreMinimum;
        internal const double DefaultScoreMargin = OpenVisionRecipeHybridRelativeRoiIntentSkill.DefaultScoreMargin;
        internal const double DefaultAngleMinimum = OpenVisionRecipeHybridRelativeRoiIntentSkill.DefaultAngleMinimum;
        internal const double DefaultAngleMaximum = OpenVisionRecipeHybridRelativeRoiIntentSkill.DefaultAngleMaximum;
        internal const double DefaultScaleRatioMinimum = OpenVisionRecipeHybridRelativeRoiIntentSkill.DefaultScaleRatioMinimum;
        internal const double DefaultScaleRatioMaximum = OpenVisionRecipeHybridRelativeRoiIntentSkill.DefaultScaleRatioMaximum;
        internal const double DefaultMinimumValidPixelRatio = OpenVisionRecipeHybridRelativeRoiIntentSkill.DefaultMinimumValidPixelRatio;
        internal const int DefaultThreshold = 170;
        internal const int DefaultMinimumArea = 700;
        internal const int DefaultMaximumArea = 1300;

        internal sealed class Plan
        {
            internal Plan(
                string locatorTemplatePath,
                OpenVisionRecipePinGapIntentSkill.RoiSample searchRoi,
                OpenVisionRecipePinGapIntentSkill.RoiSample inspectionRoi,
                OpenVisionRecipeHybridRelativeRoiIntentSkill.ReferencePose referencePose,
                double scoreMinimum,
                double scoreMargin,
                double angleMinimum,
                double angleMaximum,
                double scaleRatioMinimum,
                double scaleRatioMaximum,
                double minimumValidPixelRatio,
                int threshold,
                int minimumArea,
                int maximumArea,
                int? expectedCount)
            {
                LocatorTemplatePath = locatorTemplatePath ?? string.Empty;
                SearchRoi = searchRoi;
                InspectionRoi = inspectionRoi;
                ReferencePose = referencePose;
                ScoreMinimum = scoreMinimum;
                ScoreMargin = scoreMargin;
                AngleMinimum = angleMinimum;
                AngleMaximum = angleMaximum;
                ScaleRatioMinimum = scaleRatioMinimum;
                ScaleRatioMaximum = scaleRatioMaximum;
                MinimumValidPixelRatio = minimumValidPixelRatio;
                Threshold = threshold;
                MinimumArea = minimumArea;
                MaximumArea = maximumArea;
                ExpectedCount = expectedCount;
            }

            internal string LocatorTemplatePath { get; }
            internal OpenVisionRecipePinGapIntentSkill.RoiSample SearchRoi { get; }
            internal OpenVisionRecipePinGapIntentSkill.RoiSample InspectionRoi { get; }
            internal OpenVisionRecipeHybridRelativeRoiIntentSkill.ReferencePose ReferencePose { get; }
            internal double ScoreMinimum { get; }
            internal double ScoreMargin { get; }
            internal double AngleMinimum { get; }
            internal double AngleMaximum { get; }
            internal double ScaleRatioMinimum { get; }
            internal double ScaleRatioMaximum { get; }
            internal double MinimumValidPixelRatio { get; }
            internal int Threshold { get; }
            internal int MinimumArea { get; }
            internal int MaximumArea { get; }
            internal int? ExpectedCount { get; }
            internal bool IsMeasurementOnly => !ExpectedCount.HasValue;
        }

        internal static bool TryCreatePlan(
            string locatorTemplatePath,
            string searchRoiText,
            string inspectionRoiText,
            string referencePoseText,
            string scoreMinimumText,
            string scoreMarginText,
            string angleMinimumText,
            string angleMaximumText,
            string scaleRatioMinimumText,
            string scaleRatioMaximumText,
            string minimumValidPixelRatioText,
            string thresholdText,
            string minimumAreaText,
            string maximumAreaText,
            string expectedCountText,
            out Plan plan,
            out string message)
        {
            plan = null;
            if (!OpenVisionRecipeHybridRelativeRoiIntentSkill.TryValidateInputs(
                    locatorTemplatePath,
                    searchRoiText,
                    inspectionRoiText,
                    referencePoseText,
                    scoreMinimumText,
                    scoreMarginText,
                    angleMinimumText,
                    angleMaximumText,
                    scaleRatioMinimumText,
                    scaleRatioMaximumText,
                    minimumValidPixelRatioText,
                    out OpenVisionRecipePinGapIntentSkill.RoiSample searchRoi,
                    out OpenVisionRecipePinGapIntentSkill.RoiSample inspectionRoi,
                    out OpenVisionRecipeHybridRelativeRoiIntentSkill.ReferencePose referencePose,
                    out double scoreMinimum,
                    out double scoreMargin,
                    out double angleMinimum,
                    out double angleMaximum,
                    out double scaleRatioMinimum,
                    out double scaleRatioMaximum,
                    out double minimumValidPixelRatio,
                    out message))
            {
                return false;
            }

            if (!OpenVisionRecipeBlobCountIntentSkill.TryParseByte(thresholdText, out int threshold))
            {
                message = "Threshold must be an integer from 0 to 255.";
                return false;
            }

            if (!OpenVisionRecipeBlobCountIntentSkill.TryParsePositiveInt(minimumAreaText, out int minimumArea)
                || !OpenVisionRecipeBlobCountIntentSkill.TryParsePositiveInt(maximumAreaText, out int maximumArea)
                || minimumArea > maximumArea)
            {
                message = "Blob area limits must be positive and minimum area must not exceed maximum area.";
                return false;
            }

            int? expectedCount = null;
            if (!string.IsNullOrWhiteSpace(expectedCountText))
            {
                if (!OpenVisionRecipeBlobCountIntentSkill.TryParseNonNegativeInt(expectedCountText, out int parsedCount))
                {
                    message = "Expected ResultCount must be blank for measurement-only or a non-negative integer.";
                    return false;
                }

                expectedCount = parsedCount;
            }

            plan = new Plan(
                locatorTemplatePath.Trim(),
                searchRoi,
                inspectionRoi,
                referencePose,
                scoreMinimum,
                scoreMargin,
                angleMinimum,
                angleMaximum,
                scaleRatioMinimum,
                scaleRatioMaximum,
                minimumValidPixelRatio,
                threshold,
                minimumArea,
                maximumArea,
                expectedCount);
            message = string.Empty;
            return true;
        }

        internal static VisionPipeline CreateMeasurementPipeline(Plan plan)
        {
            if (plan == null)
            {
                throw new ArgumentNullException(nameof(plan));
            }

            VisionPipeline pipeline = OpenVisionRecipeHybridRelativeRoiIntentSkill.CreateMeasurementPipeline(
                plan.LocatorTemplatePath,
                plan.SearchRoi,
                plan.InspectionRoi,
                plan.ReferencePose,
                plan.ScoreMinimum,
                plan.ScoreMargin,
                plan.AngleMinimum,
                plan.AngleMaximum,
                plan.ScaleRatioMinimum,
                plan.ScaleRatioMaximum,
                plan.MinimumValidPixelRatio);
            pipeline.Name = "LLM_Locator_Relative_Blob_v1";

            if (pipeline.Steps == null || pipeline.Steps.Count != 4)
            {
                throw new InvalidOperationException("The locator-relative Blob prefix must contain exactly four steps.");
            }

            pipeline.Steps.RemoveAt(3);

            VisionPipelineStep threshold = new VisionPipelineStep
            {
                Name = "04 Threshold Reference Image",
                ToolType = "Threshold",
                Enabled = true,
                InputLayer = "DeviceAligned",
                OutputLayer = "AlignedInspectionBinary",
                UseAcceptance = false,
                ExpectedSuccess = true,
                MaxElapsedMilliseconds = 1000
            };
            threshold.Parameters["Mode"] = "Threshold";
            threshold.Parameters["Threshold"] = plan.Threshold.ToString(CultureInfo.InvariantCulture);
            threshold.Parameters["MaxValue"] = "255";
            threshold.Parameters["ThresholdType"] = "Binary";
            pipeline.Steps.Add(threshold);

            VisionPipelineStep blob = new VisionPipelineStep
            {
                Name = "05 Inspect Locator-Relative Blob",
                ToolType = "Blob",
                Enabled = true,
                InputLayer = "AlignedInspectionBinary",
                OutputLayer = "LocatorRelativeBlob",
                UseAcceptance = false,
                ExpectedSuccess = true,
                MaxElapsedMilliseconds = 1000
            };
            blob.Parameters["Name"] = "Locator_Relative_Blob";
            blob.Parameters["PIXELPERMM"] = "1";
            blob.Parameters["USE_THRESHOLD"] = "false";
            blob.Parameters["THRESHOLD_TYPES"] = "Binary";
            blob.Parameters["THRESHOLD"] = plan.Threshold.ToString(CultureInfo.InvariantCulture);
            blob.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
            blob.Parameters["USE_BITWISENOT"] = "false";
            blob.Parameters["USE_ROI"] = "true";
            blob.Parameters["USE_MULTI_ROI"] = "false";
            blob.Parameters["USE_MASKING"] = "false";
            blob.Parameters["CvROI"] = plan.InspectionRoi.ToText();
            blob.Parameters["MIN_AREA"] = plan.MinimumArea.ToString(CultureInfo.InvariantCulture);
            blob.Parameters["MAX_AREA"] = plan.MaximumArea.ToString(CultureInfo.InvariantCulture);
            if (plan.ExpectedCount.HasValue)
            {
                blob.UseAcceptance = true;
                blob.AcceptanceMetricName = VisionPipelineKnownMetrics.ResultCount;
                blob.UseAcceptanceMetricMinimum = true;
                blob.AcceptanceMetricMinimum = plan.ExpectedCount.Value;
                blob.UseAcceptanceMetricMaximum = true;
                blob.AcceptanceMetricMaximum = plan.ExpectedCount.Value;
            }

            pipeline.Steps.Add(blob);
            return pipeline;
        }

        internal static bool TryValidatePipeline(
            VisionPipeline pipeline,
            Plan plan,
            out string message)
        {
            message = string.Empty;
            if (pipeline == null || plan == null)
            {
                message = "A non-null pipeline and reviewed plan are required.";
                return false;
            }

            VisionPipeline expected;
            try
            {
                expected = CreateMeasurementPipeline(plan);
            }
            catch (Exception ex)
            {
                message = ex.GetBaseException().Message;
                return false;
            }

            if (pipeline.Steps == null || pipeline.Steps.Count != expected.Steps.Count)
            {
                message = "The locator-relative Blob contract requires exactly five enabled steps in the locked order.";
                return false;
            }

            for (int index = 0; index < expected.Steps.Count; index++)
            {
                if (!MatchesLockedStep(pipeline.Steps[index], expected.Steps[index]))
                {
                    message = "Step " + (index + 1).ToString(CultureInfo.InvariantCulture)
                        + " does not match the reviewed locator, normalization, threshold, or Blob contract.";
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(pipeline.Name))
            {
                message = "The compiled pipeline must have a name.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        internal static bool TryCompile(
            OpenVisionRecipeLocatorRelativeBlobEvidencePacket packet,
            Plan plan,
            out VisionPipeline pipeline,
            out string message)
        {
            pipeline = null;
            message = string.Empty;
            if (packet == null || !packet.TryValidate(out message))
            {
                return false;
            }

            if (plan == null
                || !AreSamePath(packet.LocatorTemplatePath, plan.LocatorTemplatePath)
                || packet.SourceImageWidth != plan.ReferencePose.ImageWidth
                || packet.SourceImageHeight != plan.ReferencePose.ImageHeight)
            {
                message = "Evidence packet provenance does not match the reviewed plan dimensions or locator template path.";
                return false;
            }

            if (!packet.TryGetSelectedCandidate(out OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate candidate, out message)
                || !string.Equals(candidate.CoordinateFrame, CoordinateFrame, StringComparison.Ordinal))
            {
                message = "The selected candidate must be an accepted candidate in the LocatorFrame coordinate frame.";
                return false;
            }

            pipeline = CreateMeasurementPipeline(plan);
            message = "Compiled from reviewed evidence candidate '" + candidate.CandidateId + "'; no LLM-supplied coordinates were applied.";
            return true;
        }

        private static bool AreSamePath(string left, string right)
        {
            try
            {
                return string.Equals(
                    Path.GetFullPath(left ?? string.Empty).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                    Path.GetFullPath(right ?? string.Empty).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static bool MatchesLockedStep(VisionPipelineStep actual, VisionPipelineStep expected)
        {
            if (actual == null
                || expected == null
                || !string.Equals(actual.Name, expected.Name, StringComparison.Ordinal)
                || !string.Equals(actual.ToolType, expected.ToolType, StringComparison.OrdinalIgnoreCase)
                || actual.Enabled != expected.Enabled
                || !string.Equals(actual.InputLayer, expected.InputLayer, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(actual.OutputLayer, expected.OutputLayer, StringComparison.OrdinalIgnoreCase)
                || actual.UseAcceptance != expected.UseAcceptance
                || actual.ExpectedSuccess != expected.ExpectedSuccess
                || actual.MaxElapsedMilliseconds != expected.MaxElapsedMilliseconds
                || !string.Equals(actual.AcceptanceMetricName ?? string.Empty, expected.AcceptanceMetricName ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                || actual.UseAcceptanceMetricMinimum != expected.UseAcceptanceMetricMinimum
                || actual.UseAcceptanceMetricMaximum != expected.UseAcceptanceMetricMaximum
                || Math.Abs(actual.AcceptanceMetricMinimum - expected.AcceptanceMetricMinimum) > 0.000000001D
                || Math.Abs(actual.AcceptanceMetricMaximum - expected.AcceptanceMetricMaximum) > 0.000000001D
                || actual.Parameters == null
                || expected.Parameters == null
                || actual.Parameters.Count != expected.Parameters.Count)
            {
                return false;
            }

            foreach (KeyValuePair<string, string> pair in expected.Parameters)
            {
                if (!actual.Parameters.TryGetValue(pair.Key, out string actualValue)
                    || !MatchesParameter(actualValue, pair.Value))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool MatchesParameter(string actual, string expected)
        {
            string left = (actual ?? string.Empty).Trim();
            string right = (expected ?? string.Empty).Trim();
            if (bool.TryParse(left, out bool leftBoolean) && bool.TryParse(right, out bool rightBoolean))
            {
                return leftBoolean == rightBoolean;
            }

            if (double.TryParse(left, NumberStyles.Float, CultureInfo.InvariantCulture, out double leftNumber)
                && double.TryParse(right, NumberStyles.Float, CultureInfo.InvariantCulture, out double rightNumber)
                && !double.IsNaN(leftNumber)
                && !double.IsInfinity(leftNumber)
                && !double.IsNaN(rightNumber)
                && !double.IsInfinity(rightNumber))
            {
                return Math.Abs(leftNumber - rightNumber) < 0.000000001D;
            }

            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }
    }
}
