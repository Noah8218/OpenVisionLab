using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class ShellRecipeBasicLifecycleViewContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl19_shell_recipe_basic_lifecycle_view_contract_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        Directory.CreateDirectory(evidenceDirectory);

        string shellView = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "OpenVisionShellHostView.xaml"));
        string lifecycleView = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Recipe",
            "Views",
            "OpenVisionRecipeBasicLifecycleView.xaml"));
        string lifecycleCodeBehindPath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Recipe",
            "Views",
            "OpenVisionRecipeBasicLifecycleView.xaml.cs");
        string recipeCommands = File.ReadAllText(Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Recipe",
            "CommandSurface",
            "RecipeCommandSurface.cs"));

        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        try
        {
            Require(
                shellView.Contains(
                    "<local:OpenVisionRecipeBasicLifecycleView",
                    StringComparison.Ordinal)
                && shellView.Contains(
                    "x:Name=\"recipeBasicLifecyclePanel\"",
                    StringComparison.Ordinal),
                "Shell does not compose the named Recipe basic lifecycle view.");
            passed.Add("Shell composition uses the named Recipe basic lifecycle view");

            Require(
                shellView.Contains(
                    "<local:OpenVisionRecipeBasicLifecycleView.Style>",
                    StringComparison.Ordinal)
                && shellView.Contains(
                    "<DataTrigger Binding=\"{Binding IsChecked, ElementName=recipeAdvancedReviewToggle}\" Value=\"True\">",
                    StringComparison.Ordinal)
                && shellView.Contains(
                    "<Setter Property=\"Visibility\" Value=\"Collapsed\" />",
                    StringComparison.Ordinal)
                && !shellView.Contains(
                    "<Grid x:Name=\"recipeBasicLifecyclePanel\">",
                    StringComparison.Ordinal),
                "Advanced-review visibility or the former inline Grid boundary changed.");
            passed.Add("advanced-review visibility stays in the Shell namescope");

            string[] requiredBindings =
            {
                "RecipeCommands.EditRecipeNameLabelText",
                "RecipeCommands.EditRecipeName",
                "RecipeCommands.CreateNamedRecipeCommand",
                "RecipeCommands.DuplicateRecipeCommand",
                "RecipeCommands.RenameRecipeCommand",
                "RecipeCommands.DeleteRecipeCommand",
                "RecipeCommands.RecipeEditValidationText",
                "RecipeCommands.StatusText"
            };
            foreach (string binding in requiredBindings)
            {
                Require(
                    lifecycleView.Contains(binding, StringComparison.Ordinal),
                    "Lifecycle view lost binding: " + binding);
            }
            passed.Add("existing RecipeCommands state and CRUD bindings remain in the new view");

            string[] requiredAutomationIds =
            {
                "HostRecipeNameEditor",
                "HostRecipeManagerCommandStrip",
                "HostRecipeCreateNamedButton",
                "HostRecipeDuplicateButton",
                "HostRecipeRenameButton",
                "HostRecipeDeleteButton",
                "HostRecipeEditValidation"
            };
            foreach (string automationId in requiredAutomationIds)
            {
                Require(
                    lifecycleView.Contains(automationId, StringComparison.Ordinal),
                    "Lifecycle view lost AutomationId: " + automationId);
            }
            passed.Add("existing Recipe Manager automation ids remain in the new view");

            Require(
                !lifecycleView.Contains("ElementName=recipeAdvancedReviewToggle", StringComparison.Ordinal)
                && lifecycleView.Contains("HorizontalContentAlignment=\"Stretch\"", StringComparison.Ordinal)
                && lifecycleView.Contains("VerticalContentAlignment=\"Stretch\"", StringComparison.Ordinal),
                "Lifecycle view retained Shell-only toggle coupling or lost stretch layout.");
            passed.Add("new view owns presentation layout without Shell toggle coupling");

            Require(
                !File.Exists(lifecycleCodeBehindPath),
                "Lifecycle view retains a manual code-behind file even though its XAML has no view-specific behavior.");
            passed.Add("view uses compiled XAML without a redundant manual code-behind file");

            Require(
                recipeCommands.Contains("CreateNamedRecipeCommand", StringComparison.Ordinal)
                && recipeCommands.Contains("DuplicateRecipeCommand", StringComparison.Ordinal)
                && recipeCommands.Contains("RenameRecipeCommand", StringComparison.Ordinal)
                && recipeCommands.Contains("DeleteRecipeCommand", StringComparison.Ordinal),
                "Recipe command surface no longer owns the existing CRUD commands.");
            passed.Add("RecipeCommands remains the state and command owner");
        }
        catch (Exception exception)
        {
            failed.Add(exception.GetBaseException().Message);
        }

        string outputPath = Path.Combine(evidenceDirectory, "shell-recipe-basic-lifecycle-view-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: OVL-19 Shell Recipe basic lifecycle view boundary",
                "RepositoryRoot: " + repositoryRoot,
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
            "CONTRACT|shell-recipe-basic-lifecycle-view|passed="
            + passed.Count
            + "|failed="
            + failed.Count);
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static string ResolveRepositoryRoot()
    {
        foreach (string start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            DirectoryInfo? current = new DirectoryInfo(Path.GetFullPath(start));
            while (current != null)
            {
                string shellViewPath = Path.Combine(
                    current.FullName,
                    "src",
                    "OpenVisionLab",
                    "UI",
                    "Menu",
                    "Wpf",
                    "OpenVisionShellHostView.xaml");
                if (File.Exists(shellViewPath))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }
        }

        throw new DirectoryNotFoundException("OpenVisionLab repository root could not be located.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
