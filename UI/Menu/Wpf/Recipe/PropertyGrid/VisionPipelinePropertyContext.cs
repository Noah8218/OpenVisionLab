using Lib.OpenCV.Pipeline;
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
    }
}
