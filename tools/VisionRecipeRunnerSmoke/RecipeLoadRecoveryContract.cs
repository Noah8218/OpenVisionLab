using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using OpenVisionLab;

internal static class RecipeLoadRecoveryContract
{
    public static int Run(string evidenceDirectory)
    {
        evidenceDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);
        string? previousDataRoot = Environment.GetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable);
        List<string> passed = new();
        List<string> failed = new();
        try
        {
            string dataRoot = Path.Combine(evidenceDirectory, "data");
            Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);
            Require(string.Equals(AppPathService.DataRootDirectory, dataRoot, StringComparison.OrdinalIgnoreCase),
                "The test must initialize an isolated data root before loading a recipe.");

            string stableName = "Smoke_LoadStable_" + Guid.NewGuid().ToString("N")[..12];
            string failingName = "Smoke_LoadFailing_" + Guid.NewGuid().ToString("N")[..12];
            VisionToolRepository tools = new();
            DataState data = new();
            DataState stableData = data;
            int dataSetterCalls = 0;
            int recipeChangedCalls = 0;
            RecipeState recipe = new();
            recipe.SetRuntime(() => data, value => { data = value; dataSetterCalls++; }, () => tools);
            recipe.EventChangedRecipe += (_, _) => recipeChangedCalls++;
            recipe.Name = stableName;
            Require(recipe.SaveTools(), "Initial stable recipe save failed.");

            VisionToolRepository failingTools = new();
            DataState failingData = new();
            RecipeState failingRecipe = new();
            failingRecipe.SetRuntime(() => failingData, value => failingData = value, () => failingTools);
            failingRecipe.Name = failingName;
            Require(failingRecipe.SaveTools(), "Initial failing recipe save failed.");

            string failingContourPath = RecipeWorkspaceService.GetVisionConfigPath(failingName, "Contour_1");
            File.Delete(failingContourPath);
            Directory.CreateDirectory(failingContourPath);

            dataSetterCalls = 0;
            recipeChangedCalls = 0;
            recipe.Name = failingName;

            Check(recipe.Name == stableName, "Failed Recipe load keeps the previous identity", passed, failed);
            Check(ReferenceEquals(data, stableData), "Failed Recipe load keeps the previous Data instance", passed, failed);
            Check(tools.Blobs.Count == 1 && tools.Contours.Count == 1 && tools.Features.Count == 1,
                "Failed Recipe load keeps the previous Tool collections", passed, failed);
            Check(tools.LastStorageError is IOException, "Failed Recipe load retains the storage exception", passed, failed);
            Check(dataSetterCalls == 0, "Failed Tool load does not replace Recipe data", passed, failed);
            Check(recipeChangedCalls == 0, "Failed Recipe load does not publish a change event", passed, failed);

            Directory.Delete(failingContourPath);
            recipe.Name = failingName;
            Check(recipe.Name == failingName && dataSetterCalls == 1 && recipeChangedCalls == 1,
                "Retry after repairing the workspace commits identity and Data", passed, failed);
            Check(tools.Contours.Count == 1 && tools.LastStorageError == null,
                "Retry after repairing the workspace commits all Tool collections", passed, failed);
        }
        catch (Exception exception)
        {
            failed.Add(exception.ToString());
        }
        finally
        {
            Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, previousDataRoot);
        }

        string reportPath = Path.Combine(evidenceDirectory, "recipe-load-recovery-contract.txt");
        File.WriteAllLines(reportPath, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        foreach (string item in failed)
            Console.WriteLine("FAIL|" + item);
        Console.WriteLine($"CONTRACT|recipe-load-recovery|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void Check(bool condition, string message, List<string> passed, List<string> failed)
    {
        (condition ? passed : failed).Add(message);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
