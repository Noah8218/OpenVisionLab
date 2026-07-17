using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    // Formats lifecycle name guidance from Host-provided selection state; it does not execute lifecycle commands.
    internal static class OpenVisionRecipeLifecycleValidationPresenter
    {
        internal static string BuildRecipeEditValidationText(OpenVisionRecipeEditValidationRequest request)
        {
            request = request ?? new OpenVisionRecipeEditValidationRequest();
            IReadOnlyList<string> recipeNames = request.RecipeNames ?? Array.Empty<string>();
            string selected = request.SelectedRecipeName?.Trim() ?? string.Empty;
            string requested = request.RequestedRecipeName?.Trim() ?? string.Empty;
            bool hasSelectedRecipe = !string.IsNullOrWhiteSpace(selected)
                && recipeNames.Any(name => string.Equals(name, selected, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(requested))
            {
                return hasSelectedRecipe
                    ? OpenVisionRecipeText.Local("빈 이름은 새 레시피 생성 시 자동 이름을 사용합니다. XML 가져오기/내보내기는 선택된 레시피에 적용됩니다.", "Blank name uses an automatic name for create. XML import/export applies to the selected recipe.")
                    : OpenVisionRecipeText.Local("레시피를 선택하거나 새 이름을 입력하세요.", "Select a recipe or type a new name.");
            }

            if (!RecipeWorkspaceService.IsValidRecipeName(requested))
            {
                return OpenVisionRecipeText.Local("이름에 사용할 수 없는 문자가 있습니다.", "The name contains invalid characters.");
            }

            bool matchesSelected = string.Equals(selected, requested, StringComparison.OrdinalIgnoreCase);
            bool duplicateName = recipeNames.Any(name => string.Equals(name, requested, StringComparison.OrdinalIgnoreCase));

            if (!hasSelectedRecipe)
            {
                return OpenVisionRecipeText.Local("선택된 레시피가 없어 가져오기/내보내기/복제/이름 변경은 사용할 수 없습니다.", "No recipe is selected, so import/export/duplicate/rename are unavailable.");
            }

            if (matchesSelected)
            {
                return recipeNames.Count > 1
                    ? OpenVisionRecipeText.Local("현재 선택된 레시피입니다. 다른 이름을 입력하면 이름 변경이 활성화됩니다.", "This is the selected recipe. Type a different name to enable rename.")
                    : OpenVisionRecipeText.Local("현재 유일한 레시피입니다. 마지막 레시피는 삭제할 수 없습니다.", "This is the only recipe. The last recipe cannot be deleted.");
            }

            if (duplicateName)
            {
                return OpenVisionRecipeText.Local("이미 같은 이름의 레시피가 있습니다.", "A recipe with this name already exists.");
            }

            return OpenVisionRecipeText.Local("사용 가능한 이름입니다. 새로 만들기, 복제, 이름 변경에 사용할 수 있습니다.", "This name is available for create, duplicate, and rename.");
        }

        internal static string BuildPipelineEditValidationText(OpenVisionRecipePipelineEditValidationRequest request)
        {
            request = request ?? new OpenVisionRecipePipelineEditValidationRequest();
            IReadOnlyList<string> recipeNames = request.RecipeNames ?? Array.Empty<string>();
            IReadOnlyList<string> pipelineNames = request.PipelineNames ?? Array.Empty<string>();
            string selectedRecipe = request.SelectedRecipeName?.Trim() ?? string.Empty;
            string selectedPipeline = request.SelectedPipelineName?.Trim() ?? string.Empty;
            string requested = request.RequestedPipelineName?.Trim() ?? string.Empty;
            string normalized = request.NormalizedPipelineName?.Trim() ?? string.Empty;
            bool hasSelectedRecipe = !string.IsNullOrWhiteSpace(selectedRecipe)
                && recipeNames.Any(name => string.Equals(name, selectedRecipe, StringComparison.OrdinalIgnoreCase));

            if (!hasSelectedRecipe)
            {
                return OpenVisionRecipeText.Local("레시피를 먼저 선택하세요.", "Select a recipe first.");
            }

            if (!request.HasSelectedPipelineOption)
            {
                return OpenVisionRecipeText.Local("파이프라인을 선택하세요.", "Select a pipeline.");
            }

            if (string.IsNullOrWhiteSpace(requested))
            {
                return OpenVisionRecipeText.Local("파이프라인 이름은 비워둘 수 없습니다.", "Pipeline name cannot be blank.");
            }

            if (!RecipeWorkspaceService.IsValidRecipeName(normalized))
            {
                return OpenVisionRecipeText.Local("파이프라인 이름에 사용할 수 없는 문자가 있습니다.", "The pipeline name contains invalid characters.");
            }

            if (!string.Equals(requested, normalized, StringComparison.Ordinal))
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local("사용할 수 없는 문자는 '_'로 저장됩니다: {0}", "Invalid characters will be saved as '_': {0}"),
                    normalized);
            }

            bool matchesSelected = string.Equals(selectedPipeline, normalized, StringComparison.OrdinalIgnoreCase);
            bool duplicateName = pipelineNames.Any(name => string.Equals(name, normalized, StringComparison.OrdinalIgnoreCase));

            if (matchesSelected)
            {
                return pipelineNames.Count > 1
                    ? OpenVisionRecipeText.Local("현재 선택된 파이프라인입니다. 다른 이름을 입력하면 이름 변경이 활성화됩니다.", "This is the selected pipeline. Type a different name to enable rename.")
                    : OpenVisionRecipeText.Local("현재 유일한 파이프라인입니다. 마지막 파이프라인은 삭제할 수 없습니다.", "This is the only pipeline. The last pipeline cannot be deleted.");
            }

            if (duplicateName)
            {
                return OpenVisionRecipeText.Local("이미 같은 이름의 파이프라인이 있습니다.", "A pipeline with this name already exists.");
            }

            return OpenVisionRecipeText.Local("사용 가능한 파이프라인 이름입니다. 복제 또는 이름 변경에 사용할 수 있습니다.", "This pipeline name is available for duplicate or rename.");
        }
    }

    internal sealed class OpenVisionRecipeEditValidationRequest
    {
        internal string SelectedRecipeName { get; set; }

        internal string RequestedRecipeName { get; set; }

        internal IReadOnlyList<string> RecipeNames { get; set; }
    }

    internal sealed class OpenVisionRecipePipelineEditValidationRequest
    {
        internal string SelectedRecipeName { get; set; }

        internal IReadOnlyList<string> RecipeNames { get; set; }

        internal bool HasSelectedPipelineOption { get; set; }

        internal string SelectedPipelineName { get; set; }

        internal string RequestedPipelineName { get; set; }

        internal string NormalizedPipelineName { get; set; }

        internal IReadOnlyList<string> PipelineNames { get; set; }
    }
}
