using OpenVisionLab.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace OpenVisionLab
{
    public sealed class OpenVisionShellHostRecipeCommandSurface : ObservableObject
    {
        private readonly Func<string> currentRecipeProvider;
        private readonly Action<string> switchRecipe;
        private readonly Action refreshAfterSwitch;
        private IReadOnlyList<string> recipeOptions = Array.Empty<string>();
        private string selectedRecipeName = string.Empty;

        internal OpenVisionShellHostRecipeCommandSurface(
            Func<string> currentRecipeProvider,
            Action<string> switchRecipe,
            Action refreshAfterSwitch)
        {
            this.currentRecipeProvider = currentRecipeProvider ?? throw new ArgumentNullException(nameof(currentRecipeProvider));
            this.switchRecipe = switchRecipe ?? throw new ArgumentNullException(nameof(switchRecipe));
            this.refreshAfterSwitch = refreshAfterSwitch ?? throw new ArgumentNullException(nameof(refreshAfterSwitch));

            CreateRecipeCommand = new RelayCommand(CreateRecipe);
            RefreshOptions();
        }

        public IReadOnlyList<string> RecipeOptions
        {
            get => recipeOptions;
            private set => SetProperty(ref recipeOptions, value ?? Array.Empty<string>());
        }

        public string SelectedRecipeName
        {
            get => selectedRecipeName;
            set => SelectRecipe(value);
        }

        public ICommand CreateRecipeCommand { get; }

        public string NewRecipeButtonText => LocalText("새 레시피", "New recipe");

        public string RecipeSelectorToolTipText => LocalText("레시피 선택 / 전환", "Select or switch recipe");

        public void RefreshOptions()
        {
            string current = NormalizeRecipeName(currentRecipeProvider());
            IReadOnlyList<string> names = RecipeWorkspaceService.GetRecipeNames()
                .Append(current)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            RecipeOptions = names;
            SetSelectedRecipeName(current);
        }

        public void RefreshLocalization()
        {
            OnPropertyChanged(nameof(NewRecipeButtonText));
            OnPropertyChanged(nameof(RecipeSelectorToolTipText));
        }

        private void SelectRecipe(string recipeName)
        {
            string normalized = NormalizeRecipeName(recipeName);
            if (string.Equals(selectedRecipeName, normalized, StringComparison.Ordinal))
            {
                return;
            }

            RecipeWorkspaceService.EnsureVisionWorkspace(normalized);
            switchRecipe(normalized);
            RefreshOptions();
            refreshAfterSwitch();
        }

        private void CreateRecipe()
        {
            string recipeName = CreateUniqueRecipeName();
            RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, VisionPipelineAppendService.DefaultPipelineName);
            VisionPipelineStorage.Load(recipeName, VisionPipelineAppendService.DefaultPipelineName);

            switchRecipe(recipeName);
            RefreshOptions();
            refreshAfterSwitch();
        }

        private string CreateUniqueRecipeName()
        {
            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            string baseName = "Recipe_" + stamp;
            string candidate = baseName;
            int index = 2;
            HashSet<string> existing = RecipeWorkspaceService.GetRecipeNames().ToHashSet(StringComparer.OrdinalIgnoreCase);
            while (existing.Contains(candidate))
            {
                candidate = baseName + "_" + index.ToString(CultureInfo.InvariantCulture);
                index++;
            }

            return candidate;
        }

        private void SetSelectedRecipeName(string recipeName)
        {
            string normalized = NormalizeRecipeName(recipeName);
            if (string.Equals(selectedRecipeName, normalized, StringComparison.Ordinal))
            {
                return;
            }

            selectedRecipeName = normalized;
            OnPropertyChanged(nameof(SelectedRecipeName));
        }

        private static string NormalizeRecipeName(string recipeName)
        {
            return string.IsNullOrWhiteSpace(recipeName) ? "Default" : recipeName.Trim();
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean ? korean : english;
        }
    }
}
