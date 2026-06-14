using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineNormalizationChange
    {
        public int StepIndex { get; set; }
        public VisionPipelineStep Step { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    internal static class VisionPipelineNormalizer
    {
        public static IReadOnlyList<VisionPipelineNormalizationChange> NormalizeChainedInspectionPreprocessing(VisionPipeline pipeline)
        {
            List<VisionPipelineNormalizationChange> changes = new List<VisionPipelineNormalizationChange>();
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                return changes;
            }

            for (int i = 0; i < pipeline.Steps.Count; i++)
            {
                VisionPipelineNormalizationChange change = NormalizeChainedInspectionPreprocessing(pipeline, pipeline.Steps[i], i);
                if (change != null)
                {
                    changes.Add(change);
                }
            }

            return changes;
        }

        public static VisionPipelineNormalizationChange NormalizeChainedInspectionPreprocessing(
            VisionPipeline pipeline,
            VisionPipelineStep step,
            int stepIndex)
        {
            if (pipeline == null
                || step == null
                || !IsLinkedToPreviousEnabledOutput(pipeline, stepIndex, step.InputLayer))
            {
                return null;
            }

            string toolType = NormalizeToolType(step.ToolType);
            if (!IsOpenCvInspectionToolWithInternalThreshold(toolType))
            {
                return null;
            }

            bool changed = false;
            changed |= SetParameterIfDifferent(step, "USE_THRESHOLD", "false");
            changed |= SetParameterIfDifferent(step, "USE_ADAPTIVE_THRESHOLD", "false");
            changed |= SetParameterIfDifferent(step, "USE_BITWISENOT", "false");

            if (!changed)
            {
                return null;
            }

            return new VisionPipelineNormalizationChange
            {
                StepIndex = stepIndex,
                Step = step,
                Message = $"CHAIN AUTO | {step.Name} | Chained input uses a processed layer, so internal threshold/adaptive/invert preprocessing was disabled."
            };
        }

        public static bool IsLinkedToPreviousEnabledOutput(VisionPipeline pipeline, int stepIndex, string inputLayer)
        {
            return TryGetPreviousEnabledOutput(pipeline, stepIndex, out string previousOutput)
                && !string.IsNullOrWhiteSpace(inputLayer)
                && string.Equals(inputLayer.Trim(), previousOutput, StringComparison.OrdinalIgnoreCase);
        }

        public static bool TryGetPreviousEnabledOutput(VisionPipeline pipeline, int stepIndex, out string previousOutput)
        {
            previousOutput = string.Empty;
            if (pipeline?.Steps == null || stepIndex <= 0)
            {
                return false;
            }

            for (int i = stepIndex - 1; i >= 0; i--)
            {
                VisionPipelineStep previousStep = pipeline.Steps[i];
                if (previousStep == null || !previousStep.Enabled || string.IsNullOrWhiteSpace(previousStep.OutputLayer))
                {
                    continue;
                }

                previousOutput = previousStep.OutputLayer.Trim();
                return true;
            }

            return false;
        }

        public static bool HasInternalPreprocessingEnabled(VisionPipelineStep step)
        {
            return GetBool(step, "USE_THRESHOLD", defaultValue: true)
                || GetBool(step, "USE_ADAPTIVE_THRESHOLD", defaultValue: false)
                || GetBool(step, "USE_BITWISENOT", defaultValue: false);
        }

        public static bool IsOpenCvInspectionToolWithInternalThreshold(string normalizedToolType)
        {
            switch (normalizedToolType)
            {
                case "blob":
                case "contour":
                case "line":
                case "linegauge":
                case "mean":
                    return true;
                default:
                    return false;
            }
        }

        public static string NormalizeToolType(string toolType)
        {
            string value = (toolType ?? string.Empty).Trim();
            if (value.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 4);
            }

            return value.Replace(" ", string.Empty).Replace("_", string.Empty).ToLowerInvariant();
        }

        private static bool SetParameterIfDifferent(VisionPipelineStep step, string key, string value)
        {
            if (step == null || string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            if (step.Parameters.TryGetValue(key, out string currentValue)
                && string.Equals(currentValue, value, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            step.Parameters[key] = value;
            return true;
        }

        private static bool GetBool(VisionPipelineStep step, string key, bool defaultValue)
        {
            if (step == null || string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }

            return step.Parameters.TryGetValue(key, out string text)
                && bool.TryParse(text, out bool value)
                    ? value
                    : defaultValue;
        }
    }
}
