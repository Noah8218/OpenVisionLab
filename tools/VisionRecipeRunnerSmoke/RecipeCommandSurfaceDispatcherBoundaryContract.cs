using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal static class RecipeCommandSurfaceDispatcherBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "recipe-command-surface-dispatch-boundary_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);
        string? previousDataRoot = Environment.GetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        int yieldCount = 0;
        int flushCount = 0;
        string currentRecipe = "Default";

        try
        {
            Require(
                string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase),
                "Dispatcher boundary evidence must be on D:.");
            Environment.SetEnvironmentVariable(
                AppPathService.DataRootEnvironmentVariable,
                Path.Combine(evidenceDirectory, "data"));

            OpenVisionShellHostRecipeCommandSurface surface =
                new OpenVisionShellHostRecipeCommandSurface(
                    () => currentRecipe,
                    recipeName => currentRecipe = recipeName,
                    () => { },
                    yieldToUi: () =>
                    {
                        yieldCount++;
                        return Task.CompletedTask;
                    },
                    flushUi: () => flushCount++);

            surface.SelectedRecipeName = "DispatcherSelection";
            Require(
                string.Equals(currentRecipe, "DispatcherSelection", StringComparison.Ordinal),
                "Recipe selection did not reach the existing switch callback.");
            Require(yieldCount == 1, "Recipe selection did not invoke the injected UI yield callback exactly once.");
            Require(flushCount == 1, "Recipe selection did not invoke the injected UI flush callback exactly once.");
            Require(
                surface.StatusText.Contains("Selected", StringComparison.OrdinalIgnoreCase)
                    || surface.StatusText.Contains("선택", StringComparison.Ordinal),
                "Recipe selection did not preserve the existing selected status projection.");
            passed.Add("recipe selection routes scheduling through Shell callbacks and preserves status");

            surface.EditRecipeName = "DispatcherCreate_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            Require(
                surface.CreateNamedRecipeCommand.CanExecute(null),
                "Create named recipe command was not executable for a valid unique name.");
            surface.CreateNamedRecipeCommand.Execute(null);
            Require(yieldCount == 2, "Recipe creation did not invoke the injected UI yield callback exactly once.");
            Require(flushCount == 2, "Recipe creation did not invoke the injected UI flush callback exactly once.");
            Require(
                surface.StatusText.Contains("Created", StringComparison.OrdinalIgnoreCase)
                    || surface.StatusText.Contains("생성", StringComparison.Ordinal),
                "Recipe creation did not preserve the existing created status projection.");
            passed.Add("recipe creation routes scheduling through Shell callbacks and preserves status");
        }
        catch (Exception exception)
        {
            failed.Add(exception.GetBaseException().Message);
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                AppPathService.DataRootEnvironmentVariable,
                previousDataRoot);
        }

        string outputPath = Path.Combine(
            evidenceDirectory,
            "recipe-command-surface-dispatch-boundary-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: PL-0025 RecipeCommandSurface Dispatcher boundary",
                "EvidenceDirectory: " + evidenceDirectory
            }
            .Concat(passed.Select(item => "PASS: " + item))
            .Concat(failed.Select(item => "FAIL: " + item)));

        foreach (string item in passed)
        {
            Console.WriteLine("PASS|" + item);
        }

        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine(
            "CONTRACT|recipe-command-surface-dispatch-boundary|passed="
            + passed.Count.ToString(CultureInfo.InvariantCulture)
            + "|failed="
            + failed.Count.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}

