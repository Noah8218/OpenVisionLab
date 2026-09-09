using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostRecipeCommandSurface
    {
        private async void CreateRecipe()
        {
            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                LocalText("새 Recipe", "New Recipe")))
            {
                return;
            }

            await CreateAndSwitchRecipeAsync(recipeWorkspaceUseCase.Create());
        }

        private async void CreateNamedRecipe()
        {
            string requestedName = EditRecipeName?.Trim();
            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Recipe,
                requestedName))
            {
                return;
            }

            await CreateAndSwitchRecipeAsync(recipeWorkspaceUseCase.Create(requestedName));
        }

        private bool CanCreateNamedRecipe()
        {
            string requestedName = EditRecipeName?.Trim();
            return recipeWorkspaceUseCase.CanCreate(requestedName);
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
            OpenVisionRecipeWorkspaceLifecycleProjection projection =
                workspaceLifecycleProjectionOwner.Project(
                    OpenVisionRecipeWorkspaceLifecycleOperation.Duplicate,
                    result);
            if (!result.Succeeded)
            {
                StatusText = projection.StatusText;
                return;
            }

            switchRecipe(result.RecipeName);
            StatusText = projection.StatusText;
            RefreshOptions();
            refreshAfterSwitch();
        }

        private bool CanDuplicateSelectedRecipe()
        {
            string selected = NormalizeRecipeName(selectedRecipeName);
            string requested = EditRecipeName?.Trim();
            return recipeWorkspaceUseCase.CanDuplicate(selected, requested, RecipeOptions);
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
            OpenVisionRecipeWorkspaceLifecycleProjection projection =
                workspaceLifecycleProjectionOwner.Project(
                    OpenVisionRecipeWorkspaceLifecycleOperation.Rename,
                    result);
            if (!result.Succeeded)
            {
                StatusText = projection.StatusText;
                return;
            }

            switchRecipe(result.RecipeName);
            StatusText = projection.StatusText;
            RefreshOptions();
            refreshAfterSwitch();
        }

        private bool CanRenameSelectedRecipe()
        {
            string oldName = NormalizeRecipeName(selectedRecipeName);
            string newName = NormalizeRecipeName(EditRecipeName);
            return recipeWorkspaceUseCase.CanRename(oldName, newName, RecipeOptions);
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
            OpenVisionRecipeWorkspaceLifecycleProjection projection =
                workspaceLifecycleProjectionOwner.Project(
                    OpenVisionRecipeWorkspaceLifecycleOperation.Delete,
                    result,
                    deletedName);
            if (!result.Succeeded)
            {
                StatusText = projection.StatusText;
                return;
            }

            switchRecipe(result.RecipeName);
            StatusText = projection.StatusText;
            RefreshOptions();
            refreshAfterSwitch();
        }

        private bool CanDeleteSelectedRecipe()
        {
            string selected = NormalizeRecipeName(selectedRecipeName);
            return recipeWorkspaceUseCase.CanDelete(selected, RecipeOptions);
        }

        private async Task CreateAndSwitchRecipeAsync(OpenVisionRecipeWorkspaceResult result)
        {
            OpenVisionRecipeWorkspaceLifecycleProjection projection =
                workspaceLifecycleProjectionOwner.Project(
                    OpenVisionRecipeWorkspaceLifecycleOperation.Create,
                    result);
            if (!projection.Succeeded)
            {
                return;
            }

            try
            {
                BeginRecipeSwitchingState(result.RecipeName);
                await System.Windows.Threading.Dispatcher.Yield(
                    System.Windows.Threading.DispatcherPriority.Background);
                switchRecipe(result.RecipeName);
                RefreshAfterRecipeSwitchIfNeeded(result.RecipeName);
                await waitForRecipeSwitchCompletion();
                StatusText = projection.StatusText;
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
