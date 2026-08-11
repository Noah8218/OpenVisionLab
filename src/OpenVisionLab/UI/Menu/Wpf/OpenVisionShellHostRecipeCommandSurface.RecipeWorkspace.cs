using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostRecipeCommandSurface
    {
        private void CreateRecipe()
        {
            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                LocalText("새 Recipe", "New Recipe")))
            {
                return;
            }

            CreateAndSwitchRecipe(recipeWorkspaceUseCase.Create());
        }

        private void CreateNamedRecipe()
        {
            string requestedName = EditRecipeName?.Trim();
            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                requestedName))
            {
                return;
            }

            CreateAndSwitchRecipe(recipeWorkspaceUseCase.Create(requestedName));
        }

        private bool CanCreateNamedRecipe()
        {
            string requestedName = EditRecipeName?.Trim();
            return string.IsNullOrWhiteSpace(requestedName)
                || RecipeWorkspaceService.IsValidRecipeName(requestedName);
        }

        private void DuplicateSelectedRecipe()
        {
            string sourceName = NormalizeRecipeName(selectedRecipeName);
            string requestedName = NormalizeRecipeName(EditRecipeName);
            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                requestedName))
            {
                return;
            }

            OpenVisionRecipeWorkspaceResult result = recipeWorkspaceUseCase.Duplicate(sourceName, requestedName);
            if (!result.Succeeded)
            {
                StatusText = LocalText("레시피 복제에 실패했습니다.", "Duplicate failed.");
                return;
            }

            switchRecipe(result.RecipeName);
            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("복제됨: {0}", "Duplicated: {0}"),
                result.RecipeName);
            RefreshOptions();
            refreshAfterSwitch();
        }

        private bool CanDuplicateSelectedRecipe()
        {
            string selected = NormalizeRecipeName(selectedRecipeName);
            string requested = EditRecipeName?.Trim();
            return !string.IsNullOrWhiteSpace(selected)
                && RecipeOptions.Any(name => string.Equals(name, selected, StringComparison.OrdinalIgnoreCase))
                && (string.IsNullOrWhiteSpace(requested)
                    || RecipeWorkspaceService.IsValidRecipeName(requested));
        }

        private void RenameSelectedRecipe()
        {
            string oldName = NormalizeRecipeName(selectedRecipeName);
            string newName = NormalizeRecipeName(EditRecipeName);
            if (!CanRenameSelectedRecipe())
            {
                StatusText = LocalText("이름을 변경할 수 없습니다.", "Cannot rename this recipe.");
                return;
            }

            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                newName))
            {
                return;
            }

            OpenVisionRecipeWorkspaceResult result = recipeWorkspaceUseCase.Rename(oldName, newName);
            if (!result.Succeeded)
            {
                StatusText = LocalText("이름 변경에 실패했습니다.", "Rename failed.");
                return;
            }

            switchRecipe(result.RecipeName);
            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("이름 변경됨: {0}", "Renamed: {0}"),
                result.RecipeName);
            RefreshOptions();
            refreshAfterSwitch();
        }

        private bool CanRenameSelectedRecipe()
        {
            string oldName = NormalizeRecipeName(selectedRecipeName);
            string newName = NormalizeRecipeName(EditRecipeName);
            return !string.IsNullOrWhiteSpace(oldName)
                && RecipeWorkspaceService.IsValidRecipeName(newName)
                && !string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase)
                && !RecipeOptions.Any(name => string.Equals(name, newName, StringComparison.OrdinalIgnoreCase));
        }

        private void DeleteSelectedRecipe()
        {
            string deletedName = NormalizeRecipeName(selectedRecipeName);
            if (!CanDeleteSelectedRecipe())
            {
                StatusText = LocalText("삭제할 수 없습니다.", "Cannot delete this recipe.");
                return;
            }

            if (!confirmDeleteRecipe(deletedName))
            {
                StatusText = LocalText("삭제가 취소되었습니다.", "Delete canceled.");
                return;
            }

            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                LocalText("Recipe 삭제: ", "Delete Recipe: ") + deletedName))
            {
                return;
            }

            string fallback = RecipeOptions
                .FirstOrDefault(name => !string.Equals(name, deletedName, StringComparison.OrdinalIgnoreCase));
            fallback = NormalizeRecipeName(fallback);
            OpenVisionRecipeWorkspaceResult result = recipeWorkspaceUseCase.Delete(deletedName, fallback);
            if (!result.Succeeded)
            {
                StatusText = LocalText("삭제에 실패했습니다.", "Delete failed.");
                return;
            }

            switchRecipe(result.RecipeName);
            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("삭제됨: {0}", "Deleted: {0}"),
                deletedName);
            RefreshOptions();
            refreshAfterSwitch();
        }

        private bool CanDeleteSelectedRecipe()
        {
            string selected = NormalizeRecipeName(selectedRecipeName);
            return !string.IsNullOrWhiteSpace(selected)
                && RecipeOptions.Count > 1
                && RecipeOptions.Any(name => string.Equals(name, selected, StringComparison.OrdinalIgnoreCase));
        }

        private void CreateAndSwitchRecipe(OpenVisionRecipeWorkspaceResult result)
        {
            if (!result.Succeeded)
            {
                return;
            }

            try
            {
                BeginRecipeSwitchingState(result.RecipeName);
                switchRecipe(result.RecipeName);
                StatusText = string.Format(
                    CultureInfo.CurrentCulture,
                    LocalText("생성됨: {0}", "Created: {0}"),
                    result.RecipeName);
                RefreshAfterRecipeSwitchIfNeeded(result.RecipeName);
            }
            finally
            {
                IsSwitchingRecipe = false;
            }
        }

        private void SaveSelectedRecipe()
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return;
            }

            if (selectedStepEditSession.IsDirty && !TryApplySelectedStepParameters())
            {
                return;
            }

            try
            {
                bool saved = saveRecipe();
                StatusText = saved
                    ? string.Format(
                        CultureInfo.CurrentCulture,
                        LocalText("레시피 저장 완료: {0}", "Recipe saved: {0}"),
                        recipeName)
                    : string.Format(
                        CultureInfo.CurrentCulture,
                        LocalText("레시피 저장 실패: {0}", "Recipe save failed: {0}"),
                        recipeName);
            }
            catch (Exception ex)
            {
                StatusText = string.Format(
                    CultureInfo.CurrentCulture,
                    LocalText("레시피 저장 실패: {0} / {1}", "Recipe save failed: {0} / {1}"),
                    recipeName,
                    ex.GetBaseException().Message);
            }
        }
    }
}
