using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    // Loads the selected pipeline Step and projects it into the existing PropertyGrid edit model.
    internal sealed class OpenVisionRecipeStepEditLoader
    {
        internal OpenVisionRecipeStepEditLoadResult Load(
            string recipeName,
            string requestedPipelineName,
            OpenVisionRecipePipelineStepPreview preview)
        {
            string pipelineName = string.IsNullOrWhiteSpace(requestedPipelineName)
                ? VisionPipelineStorage.LoadActivePipelineName(
                    recipeName,
                    VisionPipelineAppendService.DefaultPipelineName)
                : requestedPipelineName;

            if (preview == null)
            {
                return OpenVisionRecipeStepEditLoadResult.Failure(
                    recipeName,
                    pipelineName,
                    OpenVisionRecipeText.Local("선택된 Step이 없습니다.", "No step is selected."));
            }

            string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
            if (!VisionPipelineStorage.TryLoadFromFile(path, out VisionPipeline pipeline, out string message))
            {
                return OpenVisionRecipeStepEditLoadResult.Failure(recipeName, pipelineName, message);
            }

            VisionPipelineStep step = ResolveStep(pipeline, preview);
            if (step == null)
            {
                return OpenVisionRecipeStepEditLoadResult.Failure(
                    recipeName,
                    pipelineName,
                    OpenVisionRecipeText.Local(
                        "선택 Step을 XML에서 다시 찾지 못했습니다.",
                        "Could not find the selected step in XML."));
            }

            int selectedStepIndex = Math.Max(0, (pipeline?.Steps ?? new List<VisionPipelineStep>()).IndexOf(step));
            object property = VisionPipelineStepPropertyMapper.CreateProperty(
                step,
                new VisionPipelinePropertyContext(pipeline, selectedStepIndex));
            if (property == null)
            {
                return OpenVisionRecipeStepEditLoadResult.Failure(
                    recipeName,
                    pipelineName,
                    OpenVisionRecipeText.Local("지원하지 않는 Step 도구입니다: ", "Unsupported step tool: ") + step.ToolType);
            }

            return OpenVisionRecipeStepEditLoadResult.Success(
                recipeName,
                pipelineName,
                pipeline,
                step,
                property);
        }

        private static VisionPipelineStep ResolveStep(
            VisionPipeline pipeline,
            OpenVisionRecipePipelineStepPreview preview)
        {
            int index = preview.Index - 1;
            if (pipeline?.Steps != null && index >= 0 && index < pipeline.Steps.Count)
            {
                return pipeline.Steps[index];
            }

            return pipeline?.Steps?.FirstOrDefault(candidate =>
                string.Equals(candidate.Name, preview.Name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(candidate.ToolType, preview.ToolType, StringComparison.OrdinalIgnoreCase)
                && string.Equals(candidate.OutputLayer, preview.OutputLayer, StringComparison.OrdinalIgnoreCase));
        }
    }

    internal sealed class OpenVisionRecipeStepEditLoadResult
    {
        private OpenVisionRecipeStepEditLoadResult(
            bool succeeded,
            string recipeName,
            string pipelineName,
            VisionPipeline pipeline,
            VisionPipelineStep step,
            object editObject,
            string message)
        {
            Succeeded = succeeded;
            RecipeName = recipeName ?? string.Empty;
            PipelineName = pipelineName ?? string.Empty;
            Pipeline = pipeline;
            Step = step;
            EditObject = editObject;
            Message = message ?? string.Empty;
        }

        internal bool Succeeded { get; }

        internal string RecipeName { get; }

        internal string PipelineName { get; }

        internal VisionPipeline Pipeline { get; }

        internal VisionPipelineStep Step { get; }

        internal object EditObject { get; }

        internal string Message { get; }

        internal static OpenVisionRecipeStepEditLoadResult Success(
            string recipeName,
            string pipelineName,
            VisionPipeline pipeline,
            VisionPipelineStep step,
            object editObject)
        {
            return new OpenVisionRecipeStepEditLoadResult(
                true,
                recipeName,
                pipelineName,
                pipeline,
                step,
                editObject,
                string.Empty);
        }

        internal static OpenVisionRecipeStepEditLoadResult Failure(
            string recipeName,
            string pipelineName,
            string message)
        {
            return new OpenVisionRecipeStepEditLoadResult(
                false,
                recipeName,
                pipelineName,
                null,
                null,
                null,
                message);
        }
    }
}
