using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipeLlmTemplateDraftBuilder
    {
        internal static VisionPipeline Create(string template, string referenceImagePath, string pinGapRoiText)
        {
            string selectedTemplate = template ?? string.Empty;
            string pipelineName = "LLM_Starter_" + SanitizePathSegment(selectedTemplate.Replace("+", "And").Replace(" ", string.Empty));
            VisionPipeline pipeline = new VisionPipeline { Name = pipelineName };

            if (OpenVisionRecipeLlmIntent.IsPinArrayGapTemplate(selectedTemplate))
            {
                if (!OpenVisionRecipePinArrayGapIntentSkill.TryParseRowRois(
                        pinGapRoiText,
                        out IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> rowRois,
                        out _))
                {
                    return pipeline;
                }

                return OpenVisionRecipePinArrayGapIntentSkill.CreateMeasurementPipeline(
                    rowRois,
                    OpenVisionRecipePinArrayGapIntentSkill.DefaultDarkThreshold,
                    OpenVisionRecipePinArrayGapIntentSkill.DefaultMinimumDarkCoverageRatio,
                    OpenVisionRecipePinArrayGapIntentSkill.DefaultMinimumPinWidth,
                    OpenVisionRecipePinArrayGapIntentSkill.DefaultMaximumPinBreakWidth,
                    OpenVisionRecipePinArrayGapIntentSkill.DefaultMinimumGapWidth);
            }

            if (OpenVisionRecipeLlmIntent.IsDarkBandGapTemplate(selectedTemplate))
            {
                if (!OpenVisionRecipeDarkBandGapIntentSkill.TryParseCoarseRoi(
                        pinGapRoiText,
                        out OpenVisionRecipePinGapIntentSkill.RoiSample roi,
                        out _))
                {
                    return pipeline;
                }

                return OpenVisionRecipeDarkBandGapIntentSkill.CreateMeasurementPipeline(roi);
            }

            if (OpenVisionRecipeLlmIntent.IsLineDistanceTemplate(selectedTemplate))
            {
                IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> samples =
                    OpenVisionRecipePinGapIntentSkill.TryParseRoiSamples(pinGapRoiText, out IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> parsedSamples, out _)
                        ? parsedSamples
                        : OpenVisionRecipePinGapIntentSkill.DefaultRoiSamples;
                return OpenVisionRecipePinGapIntentSkill.CreatePipeline(samples, 0.40, 0.55, 0.06, 0.006);
            }

            if (OpenVisionRecipeLlmIntent.IsBlobTemplate(selectedTemplate))
            {
                VisionPipelineStep threshold = CreateDraftStep("Threshold_Precheck", "Threshold", "Main", "Threshold_Preview");
                threshold.Parameters["Threshold"] = "128";
                threshold.Parameters["MaxValue"] = "255";
                pipeline.Steps.Add(threshold);

                VisionPipelineStep blob = CreateDraftStep("Blob_Inspect", "Blob", "Threshold_Preview", "Blob_Result");
                blob.Parameters["MIN_AREA"] = "50";
                blob.Parameters["MAX_AREA"] = "999999";
                pipeline.Steps.Add(blob);
                return pipeline;
            }

            if (OpenVisionRecipeLlmIntent.IsContourTemplate(selectedTemplate))
            {
                VisionPipelineStep step = CreateDraftStep("Contour_Inspect", "Contour", "Main", "Contour_Result");
                step.Parameters["USE_THRESHOLD"] = "True";
                step.Parameters["THRESHOLD"] = "128";
                step.Parameters["MIN_AREA"] = "50";
                step.Parameters["MAX_AREA"] = "999999";
                step.Parameters["USE_DRAW_IMAGE"] = "True";
                step.UseAcceptance = true;
                step.ExpectedSuccess = true;
                step.AcceptanceMetricName = "ResultCount";
                pipeline.Steps.Add(step);
                return pipeline;
            }

            if (OpenVisionRecipeLlmIntent.IsEdgeBasedTemplate(selectedTemplate))
            {
                return OpenVisionRecipeEdgeBasedMatchingIntentSkill.CreatePipeline(referenceImagePath, 0.70, 1, 30, 90, 70);
            }

            if (OpenVisionRecipeLlmIntent.IsMeanTemplate(selectedTemplate))
            {
                VisionPipelineStep step = CreateDraftStep("Mean_Check", "Mean", "Main", "Mean_Result");
                step.Parameters["MEAN_MIN"] = "0";
                step.Parameters["MEAN_MAX"] = "255";
                pipeline.Steps.Add(step);
                return pipeline;
            }

            VisionPipelineStep matching = CreateDraftStep("Template_Match", "Matching", "Main", "Matching_Result");
            matching.Parameters["SCORE_MIN"] = "0.6";
            matching.Parameters["NUM_MATCH"] = "1";
            matching.Parameters["MAGNIFIATION"] = "1";
            matching.Parameters["USE_FIND_ANGLE"] = "True";
            matching.Parameters["FIND_ANGLE_MIN"] = "-10";
            matching.Parameters["FIND_ANGLE_MAX"] = "10";
            AddReferenceTemplateParameters(matching, referenceImagePath);
            pipeline.Steps.Add(matching);
            return pipeline;
        }

        private static void AddReferenceTemplateParameters(VisionPipelineStep step, string referenceImagePath)
        {
            if (step == null || string.IsNullOrWhiteSpace(referenceImagePath))
            {
                return;
            }

            step.Parameters["TemplatePath"] = referenceImagePath.Trim();
            step.Parameters["PATTERN_PATH"] = referenceImagePath.Trim();
        }

        private static VisionPipelineStep CreateDraftStep(string name, string toolType, string inputLayer, string outputLayer)
        {
            return new VisionPipelineStep
            {
                Name = name,
                ToolType = toolType,
                InputLayer = inputLayer,
                OutputLayer = outputLayer
            };
        }

        private static string SanitizePathSegment(string value)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = new string((value ?? string.Empty)
                .Select(ch => invalidChars.Contains(ch) ? '_' : ch)
                .ToArray());
            return string.IsNullOrWhiteSpace(sanitized) ? "Item" : sanitized;
        }
    }
}
