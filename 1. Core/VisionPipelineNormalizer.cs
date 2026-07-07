using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public const string AllowBranchInputParameter = "ALLOW_BRANCH_INPUT";

        public static IReadOnlyList<VisionPipelineNormalizationChange> NormalizeForRun(VisionPipeline pipeline)
        {
            IReadOnlyList<VisionPipelineNormalizationChange> linkChanges = NormalizeLikelySequentialLinks(pipeline);
            IReadOnlyList<VisionPipelineNormalizationChange> preprocessingChanges = NormalizeChainedInspectionPreprocessing(pipeline);
            return linkChanges
                .Concat(preprocessingChanges)
                .OrderBy(change => change.StepIndex)
                .ToList();
        }

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

        public static IReadOnlyList<VisionPipelineNormalizationChange> NormalizeLikelySequentialLinks(VisionPipeline pipeline)
        {
            List<VisionPipelineNormalizationChange> changes = new List<VisionPipelineNormalizationChange>();
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                return changes;
            }

            for (int i = 1; i < pipeline.Steps.Count; i++)
            {
                VisionPipelineNormalizationChange change = NormalizeLikelySequentialLink(pipeline, pipeline.Steps[i], i);
                if (change != null)
                {
                    changes.Add(change);
                }
            }

            return changes;
        }

        public static VisionPipelineNormalizationChange NormalizeLikelySequentialLink(
            VisionPipeline pipeline,
            VisionPipelineStep step,
            int stepIndex)
        {
            if (pipeline == null
                || step == null
                || !step.Enabled
                || string.IsNullOrWhiteSpace(step.InputLayer)
                || IsBranchInputAllowed(step)
                || !IsPrimarySourceLayer(step.InputLayer)
                || !TryGetPreviousEnabledStep(pipeline, stepIndex, out VisionPipelineStep previousStep)
                || string.IsNullOrWhiteSpace(previousStep?.OutputLayer))
            {
                return null;
            }

            string currentTool = NormalizeToolType(step.ToolType);
            string previousTool = NormalizeToolType(previousStep.ToolType);
            if (!IsPreprocessingTool(previousTool) || !IsInspectionTool(currentTool))
            {
                return null;
            }

            string previousOutput = previousStep.OutputLayer.Trim();
            if (string.Equals(step.InputLayer.Trim(), previousOutput, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            string oldInput = step.InputLayer.Trim();
            step.InputLayer = previousOutput;
            return new VisionPipelineNormalizationChange
            {
                StepIndex = stepIndex,
                Step = step,
                Message = $"CHAIN LINK | {step.Name} | Input {oldInput} -> {previousOutput}. Previous preprocessing output is used as this inspection input."
            };
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
            if (!TryGetPreviousEnabledStep(pipeline, stepIndex, out VisionPipelineStep previousStep))
            {
                return false;
            }

            previousOutput = previousStep.OutputLayer.Trim();
            return true;
        }

        public static bool TryGetPreviousEnabledStep(VisionPipeline pipeline, int stepIndex, out VisionPipelineStep previousStep)
        {
            previousStep = null;
            if (pipeline?.Steps == null || stepIndex <= 0)
            {
                return false;
            }

            for (int i = stepIndex - 1; i >= 0; i--)
            {
                VisionPipelineStep candidate = pipeline.Steps[i];
                if (candidate == null || !candidate.Enabled || string.IsNullOrWhiteSpace(candidate.OutputLayer))
                {
                    continue;
                }

                previousStep = candidate;
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

        public static bool IsBranchInputAllowed(VisionPipelineStep step)
        {
            return GetBool(step, AllowBranchInputParameter, defaultValue: false)
                || GetBool(step, "AllowBranchInput", defaultValue: false);
        }

        public static bool IsOpenCvInspectionToolWithInternalThreshold(string normalizedToolType)
        {
            switch (normalizedToolType)
            {
                case "blob":
                case "contour":
                case "line":
                case "linegauge":
                case "linedistance":
                case "linedistancegauge":
                case "lineintersection":
                case "lineintersectiongauge":
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                case "mean":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsPreprocessingTool(string normalizedToolType)
        {
            switch (normalizedToolType)
            {
                case "threshold":
                case "morphology":
                case "filter":
                case "edgedetection":
                case "edge":
                case "hsv":
                case "hsvmask":
                case "colorhsv":
                case "colormask":
                case "arithmetic":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInspectionTool(string normalizedToolType)
        {
            switch (normalizedToolType)
            {
                case "blob":
                case "contour":
                case "line":
                case "linegauge":
                case "linedistance":
                case "linedistancegauge":
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                case "mean":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsPrimarySourceLayer(string inputLayer)
        {
            return string.Equals(inputLayer?.Trim(), "Main", StringComparison.OrdinalIgnoreCase);
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
