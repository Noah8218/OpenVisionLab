using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeWorkspaceUseCase
    {
        public bool CanCreate(string requestedBaseName)
        {
            string requested = requestedBaseName?.Trim();
            return string.IsNullOrWhiteSpace(requested)
                || RecipeWorkspaceService.IsValidRecipeName(requested);
        }

        public bool CanDuplicate(
            string sourceName,
            string requestedBaseName,
            IReadOnlyCollection<string> recipeNames)
        {
            string selected = sourceName?.Trim();
            string requested = requestedBaseName?.Trim();
            return ContainsRecipe(recipeNames, selected)
                && (string.IsNullOrWhiteSpace(requested)
                    || RecipeWorkspaceService.IsValidRecipeName(requested));
        }

        public bool CanRename(
            string sourceName,
            string targetName,
            IReadOnlyCollection<string> recipeNames)
        {
            string oldName = sourceName?.Trim();
            string newName = targetName?.Trim();
            return ContainsRecipe(recipeNames, oldName)
                && RecipeWorkspaceService.IsValidRecipeName(newName)
                && !string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase)
                && !ContainsRecipe(recipeNames, newName);
        }

        public bool CanDelete(string recipeName, IReadOnlyCollection<string> recipeNames)
        {
            string selected = recipeName?.Trim();
            return recipeNames != null
                && recipeNames.Count > 1
                && ContainsRecipe(recipeNames, selected);
        }

        public OpenVisionRecipeWorkspaceResult Create(string requestedBaseName = null)
        {
            string recipeName = CreateUniqueRecipeName(requestedBaseName);
            RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
            VisionPipelineStorage.Load(recipeName, VisionPipelineAppendService.DefaultPipelineName);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, VisionPipelineAppendService.DefaultPipelineName);
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

        private static bool ContainsRecipe(
            IReadOnlyCollection<string> recipeNames,
            string recipeName)
        {
            return recipeNames != null
                && !string.IsNullOrWhiteSpace(recipeName)
                && recipeNames.Any(name => string.Equals(
                    name,
                    recipeName,
                    StringComparison.OrdinalIgnoreCase));
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
