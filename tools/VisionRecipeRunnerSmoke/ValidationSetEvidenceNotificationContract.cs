using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class ValidationSetEvidenceNotificationContract
{
    private static readonly string[] EvidencePropertyNames =
    {
        "ValidationSetExpectedText",
        "ValidationSetAcceptanceText",
        "ValidationSetCalibrationText",
        "ValidationSetNextActionText"
    };

    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl50-validation-set-evidence-notification-contract_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string? previousDataRoot = Environment.GetEnvironmentVariable(
            AppPathService.DataRootEnvironmentVariable);
        string recipeName = "Smoke_ValidationSetEvidence_" + Guid.NewGuid().ToString("N")[..10];

        try
        {
            Require(string.Equals(
                    Path.GetPathRoot(evidenceDirectory),
                    @"D:\",
                    StringComparison.OrdinalIgnoreCase),
                "Validation Set evidence notification evidence must be on D:.");

            string dataRoot = Path.Combine(evidenceDirectory, "data");
            Environment.SetEnvironmentVariable(
                AppPathService.DataRootEnvironmentVariable,
                dataRoot);
            OpenVisionRecipeValidationSetDocument document = new OpenVisionRecipeValidationSetDocument
            {
                Sets = new List<OpenVisionRecipeValidationSet>
                {
                    CreateSet("Set_A"),
                    CreateSet("Set_B")
                }
            };
            Require(
                OpenVisionRecipeValidationSetStorage.TrySave(
                    recipeName,
                    document,
                    out string saveError),
                "Could not prepare the validation-set fixture: " + saveError);

            OpenVisionShellHostRecipeCommandSurface surface =
                new OpenVisionShellHostRecipeCommandSurface(
                    () => recipeName,
                    _ => { },
                    () => { });
            List<string> notifications = new List<string>();
            surface.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName is string propertyName
                    && EvidencePropertyNames.Contains(propertyName, StringComparer.Ordinal))
                {
                    notifications.Add(propertyName);
                }
            };

            OpenVisionRecipeValidationSetOption target = surface.ValidationSetOptions
                .FirstOrDefault(option => string.Equals(option.Name, "Set_B", StringComparison.Ordinal))
                ?? throw new InvalidOperationException("The validation-set fixture was not projected into the Shell.");
            surface.SelectedValidationSetOption = target;

            foreach (string propertyName in EvidencePropertyNames)
            {
                int count = notifications.Count(name => string.Equals(name, propertyName, StringComparison.Ordinal));
                Require(
                    count == 1,
                    propertyName + " was raised " + count.ToString(CultureInfo.InvariantCulture) + " times for one set selection.");
            }

            passed.Add("one Validation Set selection raises each evidence binding exactly once");
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
            "validation-set-evidence-notification-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: OVL-50 Validation Set evidence PropertyChanged projection",
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
            "CONTRACT|validation-set-evidence-notification|passed="
            + passed.Count.ToString(CultureInfo.InvariantCulture)
            + "|failed="
            + failed.Count.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static OpenVisionRecipeValidationSet CreateSet(string name)
    {
        return new OpenVisionRecipeValidationSet
        {
            Name = name,
            Images = new List<OpenVisionRecipeValidationSetImage>()
        };
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
