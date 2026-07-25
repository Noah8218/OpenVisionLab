using Lib.OpenCV.Pipeline;
using System;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeWorkspaceUseCase
    {
        public OpenVisionRecipeWorkspaceResult Create(string requestedBaseName = null)
        {
            string recipeName = CreateUniqueRecipeName(requestedBaseName);
            RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, VisionPipelineAppendService.DefaultPipelineName);
            VisionPipelineStorage.Load(recipeName, VisionPipelineAppendService.DefaultPipelineName);
            return OpenVisionRecipeWorkspaceResult.Success(recipeName);
        }

        public OpenVisionRecipeWorkspaceResult Duplicate(string sourceName, string requestedBaseName)
        {
            string baseName = string.Equals(sourceName, requestedBaseName, StringComparison.OrdinalIgnoreCase)
                ? sourceName + "_Copy"
                : requestedBaseName;
            string targetName = CreateUniqueRecipeName(baseName);
            return RecipeWorkspaceService.DuplicateVisionWorkspace(sourceName, targetName)
                ? OpenVisionRecipeWorkspaceResult.Success(targetName)
                : OpenVisionRecipeWorkspaceResult.Failure();
        }

        public OpenVisionRecipeWorkspaceResult Rename(string sourceName, string targetName)
        {
            return RecipeWorkspaceService.RenameVisionWorkspace(sourceName, targetName)
                ? OpenVisionRecipeWorkspaceResult.Success(targetName)
                : OpenVisionRecipeWorkspaceResult.Failure();
        }

        public OpenVisionRecipeWorkspaceResult Delete(string recipeName, string fallbackRecipeName)
        {
            if (!RecipeWorkspaceService.DeleteVisionWorkspace(recipeName))
            {
                return OpenVisionRecipeWorkspaceResult.Failure();
            }

            RecipeWorkspaceService.EnsureVisionWorkspace(fallbackRecipeName);
            return OpenVisionRecipeWorkspaceResult.Success(fallbackRecipeName);
        }

        private static string CreateUniqueRecipeName(string requestedBaseName)
        {
            string baseName = string.IsNullOrWhiteSpace(requestedBaseName)
                ? "Recipe_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)
                : requestedBaseName.Trim();
            string candidate = baseName;
            int index = 2;
            string[] names = RecipeWorkspaceService.GetRecipeNames();
            while (names.Any(name => string.Equals(name, candidate, StringComparison.OrdinalIgnoreCase)))
            {
                candidate = baseName + "_" + index.ToString(CultureInfo.InvariantCulture);
                index++;
            }

            return candidate;
        }
    }

    internal sealed class OpenVisionRecipeWorkspaceResult
    {
        private OpenVisionRecipeWorkspaceResult(bool succeeded, string recipeName)
        {
            Succeeded = succeeded;
            RecipeName = recipeName ?? string.Empty;
        }

        public bool Succeeded { get; }
        public string RecipeName { get; }

        public static OpenVisionRecipeWorkspaceResult Success(string recipeName)
        {
            return new OpenVisionRecipeWorkspaceResult(true, recipeName);
        }

        public static OpenVisionRecipeWorkspaceResult Failure()
        {
            return new OpenVisionRecipeWorkspaceResult(false, string.Empty);
        }
    }
}
