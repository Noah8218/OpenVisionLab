using OpenVisionLab;
using System;
using System.IO;
using System.Text.Json;

namespace OpenVisionLab.Core.Integration;

/// <summary>
/// Consumer-side reader for the locator integration recipe published by
/// Machine Studio. The JSON carries reviewed parameters only; the template
/// bytes are resolved from the hash-checked Handoff artifact.
/// </summary>
internal static class TwoDIntegrationLocatorRecipeContract
{
    internal const string SchemaVersion = "locator-relative-blob-integration-recipe-v1";
    internal const string TemplateArtifactRole = "locator-template";
    internal const string TemplateArtifactId = "locator-template";
    internal const string EvidenceRole = "locator-relative-blob-evidence";
    internal const string EvidenceArtifactId = "locator-relative-blob-evidence";
    internal const string OverlayRole = "locator-relative-blob-overlay";
    internal const string OverlayArtifactId = "locator-relative-blob-overlay";

    internal static bool IsLocatorRecipe(string recipePath)
    {
        if (!string.Equals(
                Path.GetExtension(recipePath),
                ".json",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        using var document = JsonDocument.Parse(File.ReadAllText(Path.GetFullPath(recipePath)));
        return document.RootElement.TryGetProperty("schemaVersion", out var schema)
            && schema.ValueKind == JsonValueKind.String
            && string.Equals(schema.GetString(), SchemaVersion, StringComparison.Ordinal);
    }

    internal static OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan CreatePlan(
        string recipePath,
        string locatorTemplatePath)
    {
        var recipe = Read(recipePath);
        if (!File.Exists(locatorTemplatePath)
            || new FileInfo(locatorTemplatePath).Length <= 0)
        {
            throw new InvalidDataException(
                "The locator template artifact is missing or empty.");
        }

        if (!OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCreatePlan(
                locatorTemplatePath,
                recipe.SearchRoi,
                recipe.InspectionRoi,
                recipe.ReferencePose,
                recipe.ScoreMinimum,
                recipe.ScoreMargin,
                recipe.AngleMinimum,
                recipe.AngleMaximum,
                recipe.ScaleRatioMinimum,
                recipe.ScaleRatioMaximum,
                recipe.MinimumValidPixelRatio,
                recipe.Threshold,
                recipe.MinimumArea,
                recipe.MaximumArea,
                recipe.ExpectedCount,
                out var plan,
                out var message))
        {
            throw new InvalidDataException(
                $"The locator integration recipe is invalid: {message}");
        }

        return plan;
    }

    private static Recipe Read(string recipePath)
    {
        Recipe recipe;
        try
        {
            recipe = JsonSerializer.Deserialize<Recipe>(
                File.ReadAllText(Path.GetFullPath(recipePath)),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException(
                "The locator integration recipe is not valid JSON.",
                exception);
        }

        if (recipe is null
            || !string.Equals(recipe.SchemaVersion, SchemaVersion, StringComparison.Ordinal)
            || !string.Equals(recipe.TemplateArtifactId, TemplateArtifactId, StringComparison.Ordinal)
            || string.IsNullOrWhiteSpace(recipe.TemplatePath))
        {
            throw new InvalidDataException(
                "The locator integration recipe schema or template declaration is invalid.");
        }

        return recipe;
    }

    private sealed class Recipe
    {
        public string SchemaVersion { get; set; } = string.Empty;
        public string TemplateArtifactId { get; set; } = string.Empty;
        public string TemplatePath { get; set; } = string.Empty;
        public string SearchRoi { get; set; } = string.Empty;
        public string InspectionRoi { get; set; } = string.Empty;
        public string ReferencePose { get; set; } = string.Empty;
        public string ScoreMinimum { get; set; } = string.Empty;
        public string ScoreMargin { get; set; } = string.Empty;
        public string AngleMinimum { get; set; } = string.Empty;
        public string AngleMaximum { get; set; } = string.Empty;
        public string ScaleRatioMinimum { get; set; } = string.Empty;
        public string ScaleRatioMaximum { get; set; } = string.Empty;
        public string MinimumValidPixelRatio { get; set; } = string.Empty;
        public string Threshold { get; set; } = string.Empty;
        public string MinimumArea { get; set; } = string.Empty;
        public string MaximumArea { get; set; } = string.Empty;
        public string ExpectedCount { get; set; } = string.Empty;
    }
}
