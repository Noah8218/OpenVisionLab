using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class SmokeRecipeWorkspaceCleanupContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl29-smoke-recipe-workspace-cleanup-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Smoke recipe workspace cleanup contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "SmokeRecipeWorkspaceCleanup.cs");
        string program = File.ReadAllText(programPath);
        string owner = File.ReadAllText(ownerPath);

        Check(
            "Program delegates every transient workspace cleanup call",
            program.Contains("SmokeRecipeWorkspaceCleanup.DeleteTransient(", StringComparison.Ordinal)
                && !program.Contains("CleanupTransientRecipeWorkspaces(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Cleanup owner keeps RecipeWorkspaceService as the deletion boundary",
            owner.Contains("RecipeWorkspaceService.GetRecipeNames()", StringComparison.Ordinal)
                && owner.Contains("RecipeWorkspaceService.DeleteVisionWorkspace(recipeName)", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Cleanup owner is independent of the smoke entry point and WPF",
            !owner.Contains("Program", StringComparison.Ordinal)
                && !owner.Contains("System.Windows", StringComparison.Ordinal),
            passed,
            failed);

        string[] recipeNames =
        {
            "Default",
            "Smoke_Keep",
            "Smoke_Delete",
            "Recipe_Delete",
            "recipe_keep",
            "SmokeWithoutSeparator",
            "RecipeWithoutSeparator",
            "UserRecipe",
            string.Empty,
            "  "
        };
        IReadOnlyList<string> selected = SmokeRecipeWorkspaceCleanup.SelectTransientRecipeNames(
            recipeNames,
            new[] { "smoke_keep", "Recipe_Keep" });
        Check(
            "Selection keeps Default and explicitly retained names",
            !selected.Contains("Default", StringComparer.OrdinalIgnoreCase)
                && !selected.Contains("Smoke_Keep", StringComparer.OrdinalIgnoreCase)
                && !selected.Contains("recipe_keep", StringComparer.OrdinalIgnoreCase),
            passed,
            failed);
        Check(
            "Selection removes only reserved Smoke_ and Recipe_ names",
            selected.SequenceEqual(new[] { "Smoke_Delete", "Recipe_Delete" }, StringComparer.Ordinal),
            passed,
            failed);
        Check(
            "Selection ignores null, whitespace, and unrelated workspace names",
            !selected.Any(name => string.IsNullOrWhiteSpace(name)
                || string.Equals(name, "UserRecipe", StringComparison.Ordinal)
                || string.Equals(name, "SmokeWithoutSeparator", StringComparison.Ordinal)
                || string.Equals(name, "RecipeWithoutSeparator", StringComparison.Ordinal)),
            passed,
            failed);

        List<string> report = new List<string>
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "ChecksPassed: " + passed.Count,
            "ChecksFailed: " + failed.Count
        };
        report.AddRange(passed.Select(item => "PASS: " + item));
        report.AddRange(failed.Select(item => "FAIL: " + item));
        string reportPath = Path.Combine(evidenceDirectory, "smoke-recipe-workspace-cleanup-contract.txt");
        File.WriteAllLines(reportPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("SMOKE_RECIPE_WORKSPACE_CLEANUP_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("SMOKE_RECIPE_WORKSPACE_CLEANUP_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
        return 0;
    }

    private static void Check(string name, bool condition, ICollection<string> passed, ICollection<string> failed)
    {
        if (condition)
        {
            passed.Add(name);
        }
        else
        {
            failed.Add(name);
        }
    }

    private static string ResolveRepositoryRoot()
    {
        foreach (string start in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
        {
            DirectoryInfo? current = new DirectoryInfo(start);
            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, "src", "OpenVisionLab", "OpenVisionLab.csproj")))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }
        }

        throw new InvalidOperationException("OpenVisionLab repository root was not found.");
    }
}
