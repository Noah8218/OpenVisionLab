using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class VisionPipelinePropertyContext
    {
        public static VisionPipelinePropertyContext Empty { get; } = new VisionPipelinePropertyContext(null, 0);

        private readonly VisionPipeline pipeline;
        private readonly int currentStepIndex;

        public VisionPipelinePropertyContext(VisionPipeline pipeline, int currentStepIndex)
        {
            this.pipeline = pipeline;
            this.currentStepIndex = Math.Max(0, currentStepIndex);
        }

        public IEnumerable<string> GetCompatibleGeometryFeatureReferences(
            GeometryMeasurementMode mode,
            bool sourceA)
        {
            return VisionPipelineStepPropertyMapper.GetCompatibleGeometryFeatureReferences(
                pipeline,
                currentStepIndex,
                mode,
                sourceA);
        }

        public IEnumerable<string> GetCompatiblePointFeatureReferences()
        {
            return VisionPipelineStepPropertyMapper.GetCompatiblePointFeatureReferences(
                pipeline,
                currentStepIndex);
        }

        public IEnumerable<string> GetCompatibleMultiMatchSourceSteps()
        {
            VisionPipelineStep consumer = pipeline?.Steps != null
                && currentStepIndex >= 0
                && currentStepIndex < pipeline.Steps.Count
                    ? pipeline.Steps[currentStepIndex]
                    : null;
            return (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Take(Math.Max(0, currentStepIndex))
                .Where(candidate => candidate?.Enabled == true
                    && consumer != null
                    && string.Equals(
                        candidate.InputLayer,
                        consumer.InputLayer,
                        StringComparison.OrdinalIgnoreCase))
                .Where(candidate =>
                {
                    string type = VisionPipelineNormalizer.NormalizeToolType(
                        candidate.ToolType);
                    return type == "matching"
                        || type == "templatematching"
                        || type == "edgebasedmatching"
                        || type == "edgebasedtemplatematching"
                        || type == "edgetemplatematching";
                })
                .Where(candidate => candidate.Parameters != null
                    && candidate.Parameters.TryGetValue("NUM_MATCH", out string value)
                    && int.TryParse(value, out int count)
                    && count >= 2)
                .Select(candidate => candidate.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }
}
