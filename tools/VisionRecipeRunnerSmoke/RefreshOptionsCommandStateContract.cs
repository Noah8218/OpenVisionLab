using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class RefreshOptionsCommandStateContract
{
    private static readonly string[] CommandStateProperties =
    {
        nameof(OpenVisionShellHostRecipeCommandSurface.RecipeEditValidationText),
        nameof(OpenVisionShellHostRecipeCommandSurface.PipelineEditValidationText),
        nameof(OpenVisionShellHostRecipeCommandSurface.RecipeGuidedNextActionText),
        nameof(OpenVisionShellHostRecipeCommandSurface.RunValidationSuiteText),
        nameof(OpenVisionShellHostRecipeCommandSurface.StopValidationSuiteText),
        nameof(OpenVisionShellHostRecipeCommandSurface.IsLocalValidationSetRunning),
        nameof(OpenVisionShellHostRecipeCommandSurface.ValidationSuiteSummaryText),
        nameof(OpenVisionShellHostRecipeCommandSurface.CorrectedOutputRerunText),
        nameof(OpenVisionShellHostRecipeCommandSurface.CorrectedOutputRerunToolTipText),
        nameof(OpenVisionShellHostRecipeCommandSurface.QualifiedSnapshotPreflightText)
    };

    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl53-refresh-options-command-state-contract_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string? previousDataRoot = Environment.GetEnvironmentVariable(
            AppPathService.DataRootEnvironmentVariable);
        OpenVisionLanguage previousLanguage = OpenVisionLanguageService.CurrentLanguage;
        string recipeName = "Smoke_RefreshOptionsCommandState_" + Guid.NewGuid().ToString("N")[..10];

        try
        {
            Require(string.Equals(
                    Path.GetPathRoot(evidenceDirectory),
                    @"D:\",
                    StringComparison.OrdinalIgnoreCase),
                "RefreshOptions command-state evidence must be on D:.");

            Environment.SetEnvironmentVariable(
                AppPathService.DataRootEnvironmentVariable,
                Path.Combine(evidenceDirectory, "data"));
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, save: false);
            Require(
                OpenVisionRecipeValidationSetStorage.TrySave(
                    recipeName,
                    new OpenVisionRecipeValidationSetDocument
                    {
                        Sets = new List<OpenVisionRecipeValidationSet>
                        {
                            new OpenVisionRecipeValidationSet { Name = "Valid_Set" }
                        }
                    },
                    out string saveError),
                "Could not prepare the RefreshOptions fixture: " + saveError);

            OpenVisionShellHostRecipeCommandSurface surface =
                new OpenVisionShellHostRecipeCommandSurface(
                    () => recipeName,
                    _ => { },
                    () => { });
            Dictionary<string, int> notifications = new Dictionary<string, int>(StringComparer.Ordinal);
            surface.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName is string propertyName)
                {
                    notifications[propertyName] = notifications.TryGetValue(propertyName, out int count)
                        ? count + 1
                        : 1;
                }
            };

            surface.RefreshOptions();

            foreach (string propertyName in CommandStateProperties)
            {
                int count = GetCount(notifications, propertyName);
                Require(
                    count == 1,
                    propertyName
                    + " was projected "
                    + count.ToString(CultureInfo.InvariantCulture)
                    + " times by one RefreshOptions composite refresh.");
            }

            passed.Add("RefreshOptions composite path emits one notification per shared command-state property");
        }
        catch (Exception exception)
        {
            failed.Add(exception.GetBaseException().Message);
        }
        finally
        {
            OpenVisionLanguageService.SetLanguage(previousLanguage, save: false);
            Environment.SetEnvironmentVariable(
                AppPathService.DataRootEnvironmentVariable,
                previousDataRoot);
        }

        string outputPath = Path.Combine(
            evidenceDirectory,
            "refresh-options-command-state-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: OVL-53 RefreshOptions command-state projection ownership",
                "EvidenceDirectory: " + evidenceDirectory,
                "RecipeName: " + recipeName
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
            "CONTRACT|refresh-options-command-state|passed="
            + passed.Count.ToString(CultureInfo.InvariantCulture)
            + "|failed="
            + failed.Count.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static int GetCount(IReadOnlyDictionary<string, int> notifications, string propertyName)
    {
        return notifications.TryGetValue(propertyName, out int count) ? count : 0;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
