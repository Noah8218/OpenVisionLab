using Lib.OpenCV.Pipeline;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal static class VisionPipelineAppendService
    {
        private const string DefaultRecipeName = "Default";
        public const string DefaultPipelineName = "Pipeline";

        public static VisionPipelineStep AddStep(OpenCvPropertyBase property, string inputLayer, string outputLayer)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return AddStep(
                VisionPipelineStepBuilder.FromProperty(property, inputLayer, outputLayer),
                DefaultRecipeName,
                DefaultPipelineName);
        }

        [Obsolete("Use AddStep(step, recipeContext) and pass explicit OpenVisionRecipeContext from caller.")]
        public static VisionPipelineStep AddStep(VisionPipelineStep step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            return AddStep(step, DefaultRecipeName, DefaultPipelineName);
        }

        public static VisionPipelineStep AddStep(VisionPipelineStep step, OpenVisionRecipeContext recipeContext)
        {
            if (recipeContext == null)
            {
                throw new ArgumentNullException(nameof(recipeContext));
            }

            return AddStep(step, recipeContext.Name, recipeContext.PipelineName);
        }

        public static VisionPipelineStep AddStep(VisionPipelineStep step, string recipeName, string pipelineName)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            string targetRecipeName = Normalize(recipeName, "Default");
            string targetPipelineName = Normalize(pipelineName, DefaultPipelineName);
            VisionPipeline pipeline = VisionPipelineStorage.Load(targetRecipeName, targetPipelineName);
            if (pipeline == null)
            {
                pipeline = new VisionPipeline { Name = targetPipelineName };
            }

            if (string.IsNullOrWhiteSpace(pipeline.Name))
            {
                pipeline.Name = targetPipelineName;
            }

            EnsureUniqueStepName(pipeline, step);
            pipeline.Steps.Add(step);
            VisionPipelineStorage.Save(targetRecipeName, pipeline);
            return step;
        }

        private static string Normalize(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
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
