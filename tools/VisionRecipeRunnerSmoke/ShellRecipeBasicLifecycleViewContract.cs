using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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

            XNamespace wpf = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            XElement workbench = XDocument.Parse(shellView).Descendants(wpf + "Grid")
                .Single(element => (string?)element.Attribute("AutomationProperties.AutomationId") == "HostRecipeManagerWorkbenchGrid");
            XElement libraryColumn = workbench.Element(wpf + "Grid.ColumnDefinitions")!.Elements().First();
            Require(
                libraryColumn.Descendants(wpf + "DataTrigger").Any(trigger =>
                    ((string?)trigger.Attribute("Binding"))?.Contains("recipeAdvancedReviewToggle", StringComparison.Ordinal) == true
                    && (string?)trigger.Attribute("Value") == "True"
                    && trigger.Elements(wpf + "Setter").Any(setter => (string?)setter.Attribute("Property") == "Width" && (string?)setter.Attribute("Value") == "0"))
                && !shellView.Contains("<Grid x:Name=\"recipeBasicLifecyclePanel\">", StringComparison.Ordinal),
                "Shell library column no longer collapses during main/advanced work.");
            passed.Add("Shell owns the actual library-column visibility boundary; runtime main-entry smoke checks rendered hiding");

            string[] requiredBindings =
            {
                "RecipeCommands.RecipeManagerNameEditHintText",
                "RecipeCommands.CancelRecipeNameEditCommand",
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

            string initializationOnly = "usingSystem.Windows.Controls;namespaceOpenVisionLab{publicpartialclassOpenVisionRecipeBasicLifecycleView:UserControl{publicOpenVisionRecipeBasicLifecycleView(){InitializeComponent();}}}";
            Require(!File.Exists(lifecycleCodeBehindPath)
                || Regex.Replace(File.ReadAllText(lifecycleCodeBehindPath), @"\s", string.Empty) == initializationOnly,
                "Lifecycle code-behind contains behavior beyond compiled-XAML initialization.");
            passed.Add("View code-behind is absent or initialization-only; CRUD state remains in RecipeCommands");
            Require(lifecycleView.Contains("Key=\"Enter\" Command=\"{Binding RecipeCommands.RenameRecipeCommand}\"", StringComparison.Ordinal)
                && lifecycleView.Contains("Key=\"Escape\" Command=\"{Binding RecipeCommands.CancelRecipeNameEditCommand}\"", StringComparison.Ordinal)
                && shellView.Contains("mvvmBehaviors:InputCommandBehaviors.FocusOnKey=\"F2\"", StringComparison.Ordinal)
                && shellView.Contains("mvvmBehaviors:InputCommandBehaviors.FocusTarget=\"{Binding ElementName=recipeBasicLifecyclePanel}\"", StringComparison.Ordinal),
                "Manager keyboard focus/save/cancel bindings are missing.");
            passed.Add("F2 uses the presentation Behavior; Enter/Esc use existing state-owner commands");
            XNamespace behaviors = "clr-namespace:OpenVisionLab.Mvvm.Behaviors;assembly=OpenVisionLab.Mvvm";
            XElement pipelineList = XDocument.Parse(shellView).Descendants(wpf + "ListBox")
                .Single(element => (string?)element.Attribute("AutomationProperties.AutomationId") == "HostRecipeStoredInspectionSelector");
            Require((string?)pipelineList.Attribute(behaviors + "InputCommandBehaviors.FocusOnKey") == "F2"
                && (string?)pipelineList.Attribute(behaviors + "InputCommandBehaviors.FocusTarget") == "{Binding ElementName=recipeManagerPipelineNameEditor}"
                && (string?)pipelineList.Attribute("SelectedItem") == "{Binding RecipeCommands.RecipeManagerSelectedPipelineOption, Mode=TwoWay}"
                && pipelineList.Descendants(wpf + "Setter").Any(setter => (string?)setter.Attribute("Property") == "AutomationProperties.Name"
                    && (string?)setter.Attribute("Value") == "{Binding PipelineName}"),
                "Stored inspection list lost its contextual F2 target or accessible pipeline name.");
            passed.Add("Saved inspection list exposes pipeline names and routes F2 to its own name editor");

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
