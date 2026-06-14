using Lib.OpenCV.Pipeline;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal static class VisionPipelineAppendService
    {
        public const string DefaultPipelineName = "Pipeline";

        public static VisionPipelineStep AddStep(OpenCvPropertyBase property, string inputLayer, string outputLayer)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return AddStep(VisionPipelineStepBuilder.FromProperty(property, inputLayer, outputLayer));
        }

        public static VisionPipelineStep AddStep(VisionPipelineStep step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            string recipeName = PropertyGridEditorFactory.GetRecipeName();
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                recipeName = "Default";
            }

            VisionPipeline pipeline = VisionPipelineStorage.Load(recipeName, DefaultPipelineName);
            EnsureUniqueStepName(pipeline, step);
            pipeline.Steps.Add(step);
            VisionPipelineStorage.Save(recipeName, pipeline);
            return step;
        }

        private static void EnsureUniqueStepName(VisionPipeline pipeline, VisionPipelineStep step)
        {
            if (pipeline == null || step == null)
            {
                return;
            }

            string baseName = string.IsNullOrWhiteSpace(step.Name) ? step.ToolType : step.Name;
            bool exists = pipeline.Steps.Any(item => string.Equals(item.Name, baseName, StringComparison.OrdinalIgnoreCase));
            if (!exists)
            {
                step.Name = baseName;
                return;
            }

            int suffix = 2;
            string candidate;
            do
            {
                candidate = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", baseName, suffix++);
            }
            while (pipeline.Steps.Any(item => string.Equals(item.Name, candidate, StringComparison.OrdinalIgnoreCase)));

            step.Name = candidate;
        }
    }
}
