using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Linq;

internal static class SmokeRecipeWorkspaceCleanup
{
    internal static void DeleteTransient(params string[] keepRecipeNames)
    {
        foreach (string recipeName in SelectTransientRecipeNames(
            RecipeWorkspaceService.GetRecipeNames(),
            keepRecipeNames))
        {
            RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
        }
    }

    internal static IReadOnlyList<string> SelectTransientRecipeNames(
        IEnumerable<string>? recipeNames,
        IEnumerable<string>? keepRecipeNames)
    {
        HashSet<string> keep = new(
            (keepRecipeNames ?? Array.Empty<string>())
                .Where(name => !string.IsNullOrWhiteSpace(name)),
            StringComparer.OrdinalIgnoreCase);
        keep.Add("Default");

        return (recipeNames ?? Array.Empty<string>())
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Where(name => !keep.Contains(name)
                && (name.StartsWith("Smoke_", StringComparison.OrdinalIgnoreCase)
                    || name.StartsWith("Recipe_", StringComparison.OrdinalIgnoreCase)))
            .ToArray();
    }
}
