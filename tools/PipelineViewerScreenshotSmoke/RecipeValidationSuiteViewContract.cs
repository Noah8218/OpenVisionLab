using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using OpenVisionLab;

internal static class RecipeValidationSuiteViewContract
{
    private static readonly string[] BasicLifecycleAutomationIds =
    {
        "HostRecipeNameEditor",
        "HostRecipeManagerCommandStrip",
        "HostRecipeCreateNamedButton",
        "HostRecipeDuplicateButton",
        "HostRecipeRenameButton",
        "HostRecipeDeleteButton",
        "HostRecipeEditValidation"
    };

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
        CheckInstantiatedViews(passed, failed);

        string outputPath = Path.Combine(evidenceDirectory, "recipe_validation_suite_view_contract.txt");
        List<string> report = new List<string>
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "ChecksPassed: " + passed.Count,
            "ChecksFailed: " + failed.Count,
            "OwnerAutomationIdCount: " + ValidationSuiteAutomationIds.Length,
            "BasicLifecycleAutomationIdCount: " + BasicLifecycleAutomationIds.Length,
            "ViewAssembly: " + typeof(OpenVisionRecipeValidationSuiteView).Assembly.Location,
            "RuntimeBoundary: In-process construction and layout only; no desktop interaction, theme-state, or DPI sweep."
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

    private static void CheckInstantiatedViews(ICollection<string> passed, ICollection<string> failed)
    {
        try
        {
            OpenVisionRecipeValidationSuiteView validationView = new OpenVisionRecipeValidationSuiteView();
            OpenVisionRecipeBasicLifecycleView lifecycleView = new OpenVisionRecipeBasicLifecycleView();
            Check("Validation Suite constructor initializes compiled XAML content", validationView.Content is Grid, passed, failed);
            Check("Basic Lifecycle constructor initializes compiled XAML content", lifecycleView.Content is Grid, passed, failed);

            string[] validationIds = EnumerateAutomationIds(validationView).ToArray();
            string[] lifecycleIds = EnumerateAutomationIds(lifecycleView).ToArray();
            Check("Instantiated Validation Suite contains every owned AutomationId exactly once",
                ValidationSuiteAutomationIds.All(id => validationIds.Count(actual => actual == id) == 1), passed, failed);
            Check("Instantiated Basic Lifecycle contains every owned AutomationId exactly once",
                BasicLifecycleAutomationIds.All(id => lifecycleIds.Count(actual => actual == id) == 1), passed, failed);

            object inheritedContext = new object();
            Grid host = new Grid { DataContext = inheritedContext };
            host.Children.Add(validationView);
            host.Children.Add(lifecycleView);
            Check("Both instantiated Views inherit the parent DataContext without a local override",
                ReferenceEquals(validationView.DataContext, inheritedContext)
                    && ReferenceEquals(lifecycleView.DataContext, inheritedContext)
                    && validationView.ReadLocalValue(FrameworkElement.DataContextProperty) == DependencyProperty.UnsetValue
                    && lifecycleView.ReadLocalValue(FrameworkElement.DataContextProperty) == DependencyProperty.UnsetValue,
                passed, failed);

            bool headingSeparated = false;
            if (validationView.Content is Grid root
                && root.Children.OfType<TextBlock>().SingleOrDefault() is TextBlock heading
                && root.Children.OfType<Border>().SingleOrDefault() is Border panel)
            {
                heading.SetCurrentValue(TextBlock.TextProperty, "검증 세트 / Validation Suite");
                root.Measure(new Size(900, double.PositiveInfinity));
                root.Arrange(new Rect(0, 0, 900, root.DesiredSize.Height));
                double headingBottom = heading.TranslatePoint(new Point(0, heading.ActualHeight), root).Y;
                double panelTop = panel.TranslatePoint(new Point(0, 0), root).Y;
                headingSeparated = heading.ActualHeight > 0 && panelTop >= headingBottom;
            }

            Check("Arranged non-empty Validation Suite heading does not overlap its panel", headingSeparated, passed, failed);
        }
        catch (Exception exception)
        {
            failed.Add("Recipe View construction/layout threw " + exception.GetType().Name + ": " + exception.Message);
        }
    }

    private static IEnumerable<string> EnumerateAutomationIds(DependencyObject root)
    {
        string automationId = AutomationProperties.GetAutomationId(root);
        if (!string.IsNullOrEmpty(automationId))
        {
            yield return automationId;
        }

        foreach (DependencyObject child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
        {
            foreach (string childId in EnumerateAutomationIds(child))
            {
                yield return childId;
            }
        }
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
