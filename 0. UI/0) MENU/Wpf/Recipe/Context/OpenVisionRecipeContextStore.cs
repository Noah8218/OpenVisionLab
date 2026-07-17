using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeContextStore
    {
        private readonly Func<string> recipeNameProvider;
        private readonly Func<string> activeLayerNameProvider;
        private OpenVisionRecipeContext current;

        public OpenVisionRecipeContextStore(Func<string> recipeNameProvider, Func<string> activeLayerNameProvider)
        {
            this.recipeNameProvider = recipeNameProvider ?? throw new ArgumentNullException(nameof(recipeNameProvider));
            this.activeLayerNameProvider = activeLayerNameProvider ?? throw new ArgumentNullException(nameof(activeLayerNameProvider));
            current = Resolve();
        }

        public event EventHandler ContextChanged;

        public OpenVisionRecipeContext Current => current;

        public string CurrentRecipeName => Current.Name;

        public OpenVisionRecipeContext Refresh()
        {
            OpenVisionRecipeContext next = Resolve();
            if (IsSameContext(current, next))
            {
                current = next;
                return current;
            }

            current = next;
            ContextChanged?.Invoke(this, EventArgs.Empty);
            return current;
        }

        private OpenVisionRecipeContext Resolve()
        {
            string recipeName = Normalize(recipeNameProvider(), "Default");
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            string activeLayerName = Normalize(activeLayerNameProvider(), "Main");
            string sourcePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, activePipelineName);

            return new OpenVisionRecipeContext(
                id: recipeName,
                name: recipeName,
                pipelineName: activePipelineName,
                sourcePath: sourcePath,
                isDirty: false,
                activeLayerName: activeLayerName,
                lastReviewState: string.Empty);
        }

        private static bool IsSameContext(OpenVisionRecipeContext current, OpenVisionRecipeContext next)
        {
            return string.Equals(current.Id, next.Id, StringComparison.Ordinal)
                && string.Equals(current.Name, next.Name, StringComparison.Ordinal)
                && string.Equals(current.PipelineName, next.PipelineName, StringComparison.Ordinal)
                && string.Equals(current.SourcePath, next.SourcePath, StringComparison.Ordinal)
                && current.IsDirty == next.IsDirty
                && string.Equals(current.ActiveLayerName, next.ActiveLayerName, StringComparison.Ordinal)
                && string.Equals(current.LastReviewState, next.LastReviewState, StringComparison.Ordinal);
        }

        private static string Normalize(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }
    }
}
