using OpenVisionLab;
using OpenVisionLab.Common;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class RecipeContextFixtureContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl28-recipe-context-fixture-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Recipe context fixture contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "RecipeContextFixture.cs");
        string program = File.ReadAllText(programPath);
        string owner = File.ReadAllText(ownerPath);

        Check(
            "Program delegates every recipe context fixture construction call",
            program.Contains("RecipeContextFixture.CreatePipeline(", StringComparison.Ordinal)
                && !program.Contains("CreateRecipeContextSmokePipeline(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Fixture owner contains only deterministic pipeline construction",
            owner.Contains("VisionPipeline pipeline = new()", StringComparison.Ordinal)
                && owner.Contains("ToolType = \"Threshold\"", StringComparison.Ordinal)
                && owner.Contains("InputLayer = index == 0 ? \"Main\"", StringComparison.Ordinal)
                && owner.Contains("OutputLayer = $\"{name}_Preview_{index + 1}\"", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Fixture owner has no smoke entry point or WPF dependency",
            !owner.Contains("Program", StringComparison.Ordinal)
                && !owner.Contains("System.Windows", StringComparison.Ordinal)
                && !owner.Contains("OpenVisionShellHost", StringComparison.Ordinal),
            passed,
            failed);

        VisionPipeline pipeline = RecipeContextFixture.CreatePipeline("Contract_Context", 3);
        bool linkedLayers = pipeline.Steps.Count == 3
            && pipeline.Steps.Select((step, index) =>
                string.Equals(step.Name, "Contract_Context_Step_" + (index + 1).ToString(), StringComparison.Ordinal)
                && string.Equals(step.ToolType, "Threshold", StringComparison.Ordinal)
                && string.Equals(
                    step.InputLayer,
                    index == 0 ? "Main" : "Contract_Context_Preview_" + index.ToString(),
                    StringComparison.Ordinal)
                && string.Equals(
                    step.OutputLayer,
                    "Contract_Context_Preview_" + (index + 1).ToString(),
                    StringComparison.Ordinal))
            .All(valid => valid);
        Check(
            "Fixture preserves pipeline name, step count, and linked layer sequence",
            string.Equals(pipeline.Name, "Contract_Context", StringComparison.Ordinal) && linkedLayers,
            passed,
            failed);

        string xmlPath = Path.Combine(evidenceDirectory, "recipe-context.xml");
        SerializeHelper.SaveXmlFile(xmlPath, pipeline);
        bool xmlRoundTrip = SerializeHelper.TryLoadFromXmlFile(xmlPath, out VisionPipeline? reloaded)
            && reloaded != null
            && string.Equals(reloaded.Name, pipeline.Name, StringComparison.Ordinal)
            && reloaded.Steps.Count == pipeline.Steps.Count
            && reloaded.Steps.Select((step, index) =>
                string.Equals(step.InputLayer, pipeline.Steps[index].InputLayer, StringComparison.Ordinal)
                && string.Equals(step.OutputLayer, pipeline.Steps[index].OutputLayer, StringComparison.Ordinal))
                .All(valid => valid);
        Check(
            "Fixture remains compatible with the existing Recipe XML serializer",
            xmlRoundTrip,
            passed,
            failed);

        Check(
            "Zero and negative step counts keep the existing empty-fixture behavior",
            RecipeContextFixture.CreatePipeline("Empty_Context", 0).Steps.Count == 0
                && RecipeContextFixture.CreatePipeline("Negative_Context", -1).Steps.Count == 0,
            passed,
            failed);

        List<string> report = new List<string>
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "ChecksPassed: " + passed.Count,
            "ChecksFailed: " + failed.Count
        };
        report.AddRange(passed.Select(item => "PASS: " + item));
        report.AddRange(failed.Select(item => "FAIL: " + item));
        string reportPath = Path.Combine(evidenceDirectory, "recipe-context-fixture-contract.txt");
        File.WriteAllLines(reportPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("RECIPE_CONTEXT_FIXTURE_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("RECIPE_CONTEXT_FIXTURE_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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
