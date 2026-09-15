using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class ValidationSetProjectionErrorContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl51-validation-set-projection-error-contract_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string? previousDataRoot = Environment.GetEnvironmentVariable(
            AppPathService.DataRootEnvironmentVariable);
        OpenVisionLanguage previousLanguage = OpenVisionLanguageService.CurrentLanguage;
        string recipeName = "Smoke_ValidationSetProjectionError_" + Guid.NewGuid().ToString("N")[..10];

        try
        {
            Require(string.Equals(
                    Path.GetPathRoot(evidenceDirectory),
                    @"D:\",
                    StringComparison.OrdinalIgnoreCase),
                "Validation Set projection error evidence must be on D:.");

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
                "Could not prepare the validation-set fixture: " + saveError);

            OpenVisionShellHostRecipeCommandSurface surface =
                new OpenVisionShellHostRecipeCommandSurface(
                    () => recipeName,
                    _ => { },
                    () => { });
            string storagePath = OpenVisionRecipeValidationSetStorage.GetPath(recipeName);
            File.WriteAllText(storagePath, "<OpenVisionValidationSets>");

            int summaryNotifications = 0;
            surface.PropertyChanged += (_, args) =>
            {
                if (string.Equals(
                    args.PropertyName,
                    nameof(OpenVisionShellHostRecipeCommandSurface.ValidationSetSelectionSummaryText),
                    StringComparison.Ordinal))
                {
                    summaryNotifications++;
                }
            };

            surface.RefreshOptions();

            Require(
                summaryNotifications == 1,
                "ValidationSetSelectionSummaryText was raised "
                + summaryNotifications.ToString(CultureInfo.InvariantCulture)
                + " times after a validation-set load failure.");
            Require(
                surface.ValidationSetOptions.Count == 0
                    && surface.ValidationSetSelectionSummaryText.Contains(
                        "validation-sets.xml could not be read",
                        StringComparison.Ordinal),
                "Validation-set error projection did not clear options and expose the read failure.");
            passed.Add("validation-set load failure refreshes the selection summary binding");
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
            "validation-set-projection-error-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: OVL-51 Validation Set projection error branch",
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
            "CONTRACT|validation-set-projection-error|passed="
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
