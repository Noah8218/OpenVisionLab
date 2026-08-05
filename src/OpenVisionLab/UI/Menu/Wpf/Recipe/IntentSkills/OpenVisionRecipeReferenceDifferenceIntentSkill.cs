using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipeReferenceDifferenceIntentSkill
    {
        internal static VisionPipeline CreatePipeline(
            IReadOnlyList<string> referencePaths,
            int differenceThreshold,
            int minimumDefectArea,
            int maximumDefectArea)
        {
            string[] normalizedPaths = (referencePaths ?? Array.Empty<string>())
                .Select(path => (path ?? string.Empty).Trim())
                .Where(path => path.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(4)
                .ToArray();

            VisionPipeline pipeline = new VisionPipeline { Name = "LLM_GoldenReferenceDefect_Skill" };
            VisionPipelineStep step = new VisionPipelineStep
            {
                Name = "01 Golden Reference Difference",
                ToolType = "ReferenceDifference",
                Enabled = true,
                InputLayer = "Main",
                OutputLayer = "Reference_Difference_Review",
                UseAcceptance = true,
                ExpectedSuccess = true,
                MaxElapsedMilliseconds = 3000,
                AcceptanceMetricName = VisionPipelineKnownMetrics.ResultCount,
                UseAcceptanceMetricMinimum = true,
                AcceptanceMetricMinimum = 0,
                UseAcceptanceMetricMaximum = true,
                AcceptanceMetricMaximum = 0
            };

            step.Parameters["Name"] = "Golden_Reference_Difference";
            for (int index = 0; index < normalizedPaths.Length; index++)
            {
                step.Parameters["ReferencePath" + (index + 1).ToString(CultureInfo.InvariantCulture)] = normalizedPaths[index];
            }

            step.Parameters["DifferenceThreshold"] = differenceThreshold.ToString(CultureInfo.InvariantCulture);
            step.Parameters["MinimumDefectArea"] = minimumDefectArea.ToString(CultureInfo.InvariantCulture);
            step.Parameters["MaximumDefectArea"] = maximumDefectArea.ToString(CultureInfo.InvariantCulture);
            step.Parameters["MorphologyKernel"] = "3";
            step.Parameters["IgnoreBorder"] = "8";
            step.Parameters["OrbFeatures"] = "1600";
            step.Parameters["MatchRatio"] = "0.75";
            step.Parameters["MinimumInliers"] = "12";
            step.Parameters["RansacThreshold"] = "3";
            pipeline.Steps.Add(step);
            return pipeline;
        }

        internal static bool TryCollectReferencePaths(
            string referencePath1,
            string referencePath2,
            string referencePath3,
            string referencePath4,
            out IReadOnlyList<string> paths)
        {
            string[] candidates = { referencePath1, referencePath2, referencePath3, referencePath4 };
            string[] normalized = candidates
                .Select(path => (path ?? string.Empty).Trim())
                .Where(path => path.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            paths = normalized;
            return normalized.Length >= 1
                && normalized.Length <= 4
                && normalized.All(File.Exists);
        }

        internal static bool TryParseThreshold(string text, out int value)
        {
            return OpenVisionRecipeBlobCountIntentSkill.TryParseByte(text, out value);
        }

        internal static bool TryParsePositiveArea(string text, out int value)
        {
            return OpenVisionRecipeBlobCountIntentSkill.TryParsePositiveInt(text, out value);
        }
    }
}
