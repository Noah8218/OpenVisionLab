using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipePipelineLifecycleUseCase
    {
        public OpenVisionRecipePipelineLifecycleResult Activate(string recipeName, string pipelineName)
        {
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipelineName);
            return OpenVisionRecipePipelineLifecycleResult.Success(pipelineName, string.Empty);
        }

        public OpenVisionRecipePipelineLifecycleResult Duplicate(
            string recipeName,
            string sourcePipelineName,
            string requestedName)
        {
            string baseName = string.Equals(sourcePipelineName, requestedName, StringComparison.OrdinalIgnoreCase)
                ? sourcePipelineName + "_Copy"
                : requestedName;
            if (!TryCreateUniquePipelineName(
                    recipeName,
                    baseName,
                    out string targetName,
                    out string nameError))
            {
                return OpenVisionRecipePipelineLifecycleResult.Failure(nameError);
            }

            if (!VisionPipelineStorage.TryDuplicatePipeline(recipeName, sourcePipelineName, targetName, out string message))
            {
                return OpenVisionRecipePipelineLifecycleResult.Failure(message);
            }

            VisionPipelineStorage.SaveActivePipelineName(recipeName, targetName);
            return OpenVisionRecipePipelineLifecycleResult.Success(targetName, message);
        }

        public OpenVisionRecipePipelineLifecycleResult Rename(
            string recipeName,
            string sourcePipelineName,
            string targetPipelineName)
        {
            return VisionPipelineStorage.TryRenamePipeline(
                recipeName,
                sourcePipelineName,
                targetPipelineName,
                out string message)
                ? OpenVisionRecipePipelineLifecycleResult.Success(targetPipelineName, message)
                : OpenVisionRecipePipelineLifecycleResult.Failure(message);
        }

        public OpenVisionRecipePipelineLifecycleResult Delete(string recipeName, string pipelineName)
        {
            return VisionPipelineStorage.TryDeletePipeline(recipeName, pipelineName, out string fallbackPipelineName, out string message)
                ? OpenVisionRecipePipelineLifecycleResult.Success(fallbackPipelineName, message)
                : OpenVisionRecipePipelineLifecycleResult.Failure(message);
        }

        public OpenVisionRecipePipelineLifecycleResult DuplicateFromSample(
            string recipeName,
            string samplePipelinePath,
            string sampleName)
        {
            if (!VisionPipelineStorage.TryLoadFromFile(samplePipelinePath, out VisionPipeline pipeline, out string message))
            {
                return OpenVisionRecipePipelineLifecycleResult.Failure(message);
            }

            string basePipelineName = string.IsNullOrWhiteSpace(sampleName)
                ? pipeline.Name
                : "Sample_" + sampleName;
            if (!TryCreateUniquePipelineName(
                    recipeName,
                    basePipelineName,
                    out string uniqueName,
                    out string nameError))
            {
                return OpenVisionRecipePipelineLifecycleResult.Failure(nameError);
            }

            pipeline.Name = uniqueName;
            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
            return OpenVisionRecipePipelineLifecycleResult.Success(pipeline.Name, string.Empty);
        }

        private static bool TryCreateUniquePipelineName(
            string recipeName,
            string requestedBaseName,
            out string pipelineName,
            out string error)
        {
            if (!RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                    requestedBaseName,
                    VisionPipelineAppendService.DefaultPipelineName,
                    "Pipeline name",
                    out string baseName,
                    out error))
            {
                pipelineName = string.Empty;
                return false;
            }

            string candidate = baseName;
            int index = 2;
            HashSet<string> existing = RecipeWorkspaceService.GetVisionPipelineNames(recipeName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            while (existing.Contains(candidate))
            {
                candidate = baseName + "_" + index.ToString(CultureInfo.InvariantCulture);
                index++;
            }

            pipelineName = candidate;
            error = string.Empty;
            return true;
        }
    }

    internal sealed class OpenVisionRecipePipelineLifecycleResult
    {
        private OpenVisionRecipePipelineLifecycleResult(bool succeeded, string pipelineName, string detail)
        {
            Succeeded = succeeded;
            PipelineName = pipelineName ?? string.Empty;
            Detail = detail ?? string.Empty;
        }

        public bool Succeeded { get; }
        public string PipelineName { get; }
        public string Detail { get; }

        public static OpenVisionRecipePipelineLifecycleResult Success(string pipelineName, string detail)
        {
            return new OpenVisionRecipePipelineLifecycleResult(true, pipelineName, detail);
        }

        public static OpenVisionRecipePipelineLifecycleResult Failure(string detail)
        {
            return new OpenVisionRecipePipelineLifecycleResult(false, string.Empty, detail);
        }
    }
}
