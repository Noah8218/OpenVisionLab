using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class RecipeValidationSuiteViewContract
{
    private static readonly string[] ValidationSuiteAutomationIds =
    {
        "HostRecipeValidationSuitePanel",
        "HostRecipeValidationSuiteScopeCombo",
        "HostRecipeRunValidationSuiteButton",
        "HostRecipeStopValidationSuiteButton",
        "HostRecipeValidationSuiteStatus",
        "HostRecipeValidationSetEvidenceBoard",
        "HostRecipeValidationSetExpectedEvidence",
        "HostRecipeValidationSetAcceptanceEvidence",
        "HostRecipeValidationSetCalibrationEvidence",
        "HostRecipeValidationSetNextActionEvidence",
        "HostRecipeLocalValidationSetEditor",
        "HostRecipeValidationSetCombo",
        "HostRecipeNewValidationSetNameTextBox",
        "HostRecipeCreateValidationSetButton",
        "HostRecipeDeleteValidationSetButton",
        "HostRecipeValidationSetNotesTextBox",
        "HostRecipeAddValidationSetOkImagesButton",
        "HostRecipeAddValidationSetNgImagesButton",
        "HostRecipeRemoveValidationSetImageButton",
        "HostRecipeAddValidationSetOkFolderButton",
        "HostRecipeAddValidationSetNgFolderButton",
        "HostRecipeRepairValidationSetImagePathButton",
        "HostRecipeValidationVariantIdTextBox",
        "HostRecipeValidationMetricNameTextBox",
        "HostRecipeValidationMetricMinimumTextBox",
        "HostRecipeValidationMetricMaximumTextBox",
        "HostRecipeApplyValidationVariantButton",
        "HostRecipeResetValidationVariantButton",
        "HostRecipeValidationSetImageList",
        "HostRecipeValidationSuiteSummary"
    };

    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl35-recipe-validation-suite-view-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Recipe Validation Suite view contract must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        string shell = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "OpenVisionShellHostView.xaml"));
        string view = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Recipe",
            "Views",
            "OpenVisionRecipeValidationSuiteView.xaml"));
        string codeBehind = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Recipe",
            "Views",
            "OpenVisionRecipeValidationSuiteView.xaml.cs"));

        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        Check(
            "Shell composes the dedicated Validation Suite view",
            shell.Contains("<local:OpenVisionRecipeValidationSuiteView", StringComparison.Ordinal)
                && !shell.Contains("HostRecipeValidationSuitePanel", StringComparison.Ordinal)
                && shell.Contains("HostRecipeQualifiedSnapshotPanel", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Validation Suite automation surface is owned by the extracted view",
            ValidationSuiteAutomationIds.All(id => Count(view, id) == 1)
                && ValidationSuiteAutomationIds.All(id => !shell.Contains(id, StringComparison.Ordinal)),
            passed,
            failed);
        Check(
            "Extracted view retains RecipeCommands bindings and inherited shell DataContext",
            view.Contains("RecipeCommands.ValidationSuiteScopeOptions", StringComparison.Ordinal)
                && view.Contains("RecipeCommands.SelectedValidationSetOption", StringComparison.Ordinal)
                && view.Contains("RecipeCommands.ValidationSetImageRows", StringComparison.Ordinal)
                && view.Contains("<BooleanToVisibilityConverter x:Key=\"ShellHost.BooleanToVisibilityConverter\" />", StringComparison.Ordinal)
                && !view.Contains("DataContext=", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Extracted view keeps code-behind presentation-only",
            codeBehind.Contains("public partial class OpenVisionRecipeValidationSuiteView : UserControl", StringComparison.Ordinal)
                && codeBehind.Contains("InitializeComponent();", StringComparison.Ordinal)
                && codeBehind.Split('\n').Count(line => !string.IsNullOrWhiteSpace(line)) <= 12,
            passed,
            failed);

        string outputPath = Path.Combine(evidenceDirectory, "recipe_validation_suite_view_contract.txt");
        List<string> report = new List<string>
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "ChecksPassed: " + passed.Count,
            "ChecksFailed: " + failed.Count,
            "OwnerAutomationIdCount: " + ValidationSuiteAutomationIds.Length
        };
        report.AddRange(passed.Select(item => "PASS: " + item));
        report.AddRange(failed.Select(item => "FAIL: " + item));
        File.WriteAllLines(outputPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("RECIPE_VALIDATION_SUITE_VIEW_CONTRACT=FAIL|report=" + outputPath);
            return 1;
        }

        Console.WriteLine("RECIPE_VALIDATION_SUITE_VIEW_CONTRACT=PASS|checks=" + passed.Count + "|report=" + outputPath);
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

    private static int Count(string source, string value)
    {
        int count = 0;
        int start = 0;
        while ((start = source.IndexOf(value, start, StringComparison.Ordinal)) >= 0)
        {
            count++;
            start += value.Length;
        }

        return count;
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
