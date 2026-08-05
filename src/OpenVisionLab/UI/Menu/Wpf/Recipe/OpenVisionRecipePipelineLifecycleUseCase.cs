using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            string targetName = CreateUniquePipelineName(recipeName, baseName);
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
            pipeline.Name = CreateUniquePipelineName(recipeName, basePipelineName);
            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
            return OpenVisionRecipePipelineLifecycleResult.Success(pipeline.Name, string.Empty);
        }

        private static string CreateUniquePipelineName(string recipeName, string requestedBaseName)
        {
            string baseName = SanitizePathSegment(string.IsNullOrWhiteSpace(requestedBaseName)
                ? VisionPipelineAppendService.DefaultPipelineName
                : requestedBaseName.Trim());
            string candidate = baseName;
            int index = 2;
            HashSet<string> existing = RecipeWorkspaceService.GetVisionPipelineNames(recipeName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            while (existing.Contains(candidate))
            {
                candidate = baseName + "_" + index.ToString(CultureInfo.InvariantCulture);
                index++;
            }

            return candidate;
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
