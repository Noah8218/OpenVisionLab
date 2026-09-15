using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

internal static class ValidationSetSuccessProjectionContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl52-validation-set-success-projection-contract_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string? previousDataRoot = Environment.GetEnvironmentVariable(
            AppPathService.DataRootEnvironmentVariable);
        OpenVisionLanguage previousLanguage = OpenVisionLanguageService.CurrentLanguage;
        string recipeName = "Smoke_ValidationSetSuccessProjection_" + Guid.NewGuid().ToString("N")[..10];

        try
        {
            Require(string.Equals(
                    Path.GetPathRoot(evidenceDirectory),
                    @"D:\",
                    StringComparison.OrdinalIgnoreCase),
                "Validation Set success projection evidence must be on D:.");

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

            MethodInfo refreshMethod = typeof(OpenVisionShellHostRecipeCommandSurface).GetMethod(
                    "RefreshValidationSetOptions",
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    binder: null,
                    new[] { typeof(string), typeof(bool) },
                    modifiers: null)
                ?? throw new InvalidOperationException(
                    "RefreshValidationSetOptions method could not be located.");
            refreshMethod.Invoke(surface, new object?[] { null, true });

            Require(
                GetCount(notifications, nameof(OpenVisionShellHostRecipeCommandSurface.ValidationSetSelectionSummaryText)) == 1,
                "ValidationSetSelectionSummaryText was not projected exactly once.");
            Require(
                GetCount(notifications, nameof(OpenVisionShellHostRecipeCommandSurface.ValidationSuiteSummaryText)) == 1,
                "ValidationSuiteSummaryText was projected "
                + GetCount(notifications, nameof(OpenVisionShellHostRecipeCommandSurface.ValidationSuiteSummaryText)).ToString(CultureInfo.InvariantCulture)
                + " times by one successful refresh.");
            Require(
                GetCount(notifications, "ValidationSetExpectedText") == 1
                    && GetCount(notifications, "ValidationSetAcceptanceText") == 1
                    && GetCount(notifications, "ValidationSetCalibrationText") == 1
                    && GetCount(notifications, "ValidationSetNextActionText") == 1,
                "Validation Set evidence was not projected exactly once by one successful refresh.");
            passed.Add("successful Validation Set refresh emits one summary and one evidence projection");
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
            "validation-set-success-projection-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: OVL-52 Validation Set success projection order",
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
            "CONTRACT|validation-set-success-projection|passed="
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
