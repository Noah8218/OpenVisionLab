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

        internal string ValidateWorkbenchRecipe(string requestedName, string description, string pipelineName, bool saveAs)
        {
            string name = requestedName?.Trim() ?? string.Empty;
            if (name.Length == 0) { return "Workbench.Recipe.NameRequired"; }
            if (name.Length > 60 || !RecipeWorkspaceService.IsValidRecipeName(name)) { return "Workbench.Recipe.NameInvalid"; }
            if (ContainsRecipe(RecipeWorkspaceService.GetRecipeNames(), name)) { return "Workbench.Recipe.NameExists"; }
            if ((description?.Trim().Length ?? 0) > 120) { return "Workbench.Recipe.DescriptionInvalid"; }
            if (!saveAs && (string.IsNullOrWhiteSpace(pipelineName) || pipelineName.Trim().Length > 60 || !RecipeWorkspaceService.IsValidRecipeName(pipelineName.Trim())))
                return "Workbench.Recipe.PipelineInvalid";
            return string.Empty;
        }

        internal OpenVisionRecipeWorkspaceResult CreateWorkbenchRecipe(string requestedName, string description, VisionPipeline pipeline,
            string sourceRecipeName = null, Func<string, bool> saveRuntimeCopy = null)
        {
            string name = requestedName?.Trim() ?? string.Empty;
            string error = ValidateWorkbenchRecipe(name, description, pipeline?.Name, sourceRecipeName != null);
            if (!string.IsNullOrEmpty(error)) { return OpenVisionRecipeWorkspaceResult.Failure(error); }
            bool ownsTarget = false;
            try
            {
                ownsTarget = true;
                if (sourceRecipeName != null)
                {
                    if (!RecipeWorkspaceService.DuplicateVisionWorkspace(sourceRecipeName, name)) { ownsTarget = false; return OpenVisionRecipeWorkspaceResult.Failure("Recipe copy failed."); }
                    if (saveRuntimeCopy == null || !saveRuntimeCopy(name)) { throw new InvalidOperationException("Recipe runtime copy failed."); }
                }
                else
                {
                    RecipeWorkspaceService.EnsureVisionWorkspace(name);
                    if (saveRuntimeCopy == null) { VisionToolStorage.InitializeWorkspace(name); }
                    if (saveRuntimeCopy != null && !saveRuntimeCopy(name)) { throw new InvalidOperationException("Recipe runtime copy failed."); }
                }
                VisionPipelineStorage.Save(name, pipeline);
                VisionPipelineStorage.SaveActivePipelineName(name, pipeline.Name);
                DataState data = RecipeDataStorage.Load(name, new DataState());
                data.Description = description?.Trim() ?? string.Empty;
                RecipeRuntimeStorage.InitializeCreatedRevision(name, data);
                return OpenVisionRecipeWorkspaceResult.Success(name);
            }
            catch (Exception exception)
            {
                if (ownsTarget)
                {
                    try { RecipeWorkspaceService.DeleteVisionWorkspace(name); }
                    catch (Exception cleanup) { return OpenVisionRecipeWorkspaceResult.Failure(exception.GetBaseException().Message + Environment.NewLine + cleanup.GetBaseException().Message); }
                }
                return OpenVisionRecipeWorkspaceResult.Failure(exception.GetBaseException().Message);
            }
        }

        public OpenVisionRecipeWorkspaceResult Create(string requestedBaseName = null)
        {
            try
            {
                string recipeName = CreateUniqueRecipeName(requestedBaseName);
                RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
                VisionPipelineStorage.Load(recipeName, VisionPipelineAppendService.DefaultPipelineName);
                VisionPipelineStorage.SaveActivePipelineName(recipeName, VisionPipelineAppendService.DefaultPipelineName);
                return OpenVisionRecipeWorkspaceResult.Success(recipeName);
            }
            catch (Exception)
            {
                return OpenVisionRecipeWorkspaceResult.Failure();
            }
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
        private OpenVisionRecipeWorkspaceResult(bool succeeded, string recipeName, string error = null)
        {
            Succeeded = succeeded;
            RecipeName = recipeName ?? string.Empty;
            Error = error ?? string.Empty;
        }

        public bool Succeeded { get; }
        public string RecipeName { get; }
        public string Error { get; }

        public static OpenVisionRecipeWorkspaceResult Success(string recipeName)
        {
            return new OpenVisionRecipeWorkspaceResult(true, recipeName);
        }

        public static OpenVisionRecipeWorkspaceResult Failure(string error = null)
        {
            return new OpenVisionRecipeWorkspaceResult(false, string.Empty, error);
        }
    }
}
