using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class ValidationDatasetSmokeConfigurationContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl31-validation-dataset-configuration-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Validation dataset configuration contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "ValidationDatasetSmokeConfiguration.cs");
        string program = File.ReadAllText(programPath);
        string owner = File.ReadAllText(ownerPath);

        Check(
            "Program delegates validation dataset preparation to the configuration owner",
            program.Contains("ValidationDatasetSmokeConfiguration.LoadFromEnvironment(", StringComparison.Ordinal)
                && !program.Contains("OPENVISIONLAB_VALIDATION_DATASET_ROOT", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Program no longer owns dataset folders, baseline defaults, or XML preparation",
            !program.Contains("Directory.Exists(datasetRoot)", StringComparison.Ordinal)
                && !program.Contains("Public_Matching_DiePad.pipeline.xml", StringComparison.Ordinal)
                && !program.Contains("DiePad500_Matching_Baseline", StringComparison.Ordinal)
                && !program.Contains("Validation Matching template was not found", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Configuration owner contains the complete validation input state",
            owner.Contains("DatasetRoot", StringComparison.Ordinal)
                && owner.Contains("OkFolder", StringComparison.Ordinal)
                && owner.Contains("NgFolder", StringComparison.Ordinal)
                && owner.Contains("PipelineXml", StringComparison.Ordinal)
                && owner.Contains("MaximumPerRole", StringComparison.Ordinal)
                && owner.Contains("LoadFromEnvironment", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Configuration owner is independent of the smoke entry point and WPF",
            !owner.Contains("Program", StringComparison.Ordinal)
                && !owner.Contains("System.Windows", StringComparison.Ordinal)
                && !owner.Contains("OpenVisionShellHost", StringComparison.Ordinal),
            passed,
            failed);

        string fixtureRoot = Path.Combine(evidenceDirectory, "fixtures");
        string customDatasetRoot = Path.Combine(fixtureRoot, "custom-dataset");
        string customOkFolder = Path.Combine(customDatasetRoot, "all_images", "OK");
        string customNgFolder = Path.Combine(customDatasetRoot, "all_images", "NG");
        Directory.CreateDirectory(customOkFolder);
        Directory.CreateDirectory(customNgFolder);
        string customPipelinePath = Path.Combine(fixtureRoot, "custom.pipeline.xml");
        const string customPipelineXml = "<VisionPipeline><Name>Custom_Pipeline</Name></VisionPipeline>";
        File.WriteAllText(customPipelinePath, customPipelineXml);

        ValidationDatasetSmokeConfiguration custom = ValidationDatasetSmokeConfiguration.Create(
            repositoryRoot,
            customDatasetRoot,
            customPipelinePath,
            " Custom_Pipeline ",
            " ",
            " ",
            null,
            "-4");
        Check(
            "Caller-supplied pipeline keeps XML and uses the nested all_images folders",
            !custom.UsesDefaultMatchingBaseline
                && string.Equals(custom.OkFolder, customOkFolder, StringComparison.Ordinal)
                && string.Equals(custom.NgFolder, customNgFolder, StringComparison.Ordinal)
                && string.Equals(custom.PipelineName, "Custom_Pipeline", StringComparison.Ordinal)
                && string.Equals(custom.PipelineXml, customPipelineXml, StringComparison.Ordinal)
                && string.Equals(custom.SuiteName, "Custom_Pipeline", StringComparison.Ordinal)
                && custom.Boundary.Contains("Caller-supplied validation pipeline", StringComparison.Ordinal)
                && custom.MaximumPerRole == 1,
            passed,
            failed);

        string baselineRepositoryRoot = Path.Combine(fixtureRoot, "baseline-repository");
        string baselineDatasetRoot = Path.Combine(fixtureRoot, "baseline-dataset");
        Directory.CreateDirectory(Path.Combine(baselineRepositoryRoot, "bin", "Debug", "EasyMatch"));
        Directory.CreateDirectory(Path.Combine(baselineRepositoryRoot, "docs", "samples", "public"));
        Directory.CreateDirectory(Path.Combine(baselineDatasetRoot, "OK"));
        Directory.CreateDirectory(Path.Combine(baselineDatasetRoot, "NG"));
        string baselineTemplatePath = Path.Combine(
            baselineRepositoryRoot,
            "bin",
            "Debug",
            "EasyMatch",
            "Die Pad Model 1.bmp");
        File.WriteAllBytes(baselineTemplatePath, new byte[] { 1 });
        string baselinePipelinePath = Path.Combine(
            baselineRepositoryRoot,
            "docs",
            "samples",
            "public",
            "Public_Matching_DiePad.pipeline.xml");
        File.WriteAllText(
            baselinePipelinePath,
            "<VisionPipeline><Name>Public_Matching_DiePad</Name><Value>Public_Matching_DiePad</Value>"
            + "<Template>docs\\samples\\public\\templates\\Matching_DiePad_Synthetic_Template.png</Template></VisionPipeline>");

        ValidationDatasetSmokeConfiguration baseline = ValidationDatasetSmokeConfiguration.Create(
            baselineRepositoryRoot,
            baselineDatasetRoot,
            null,
            null,
            null,
            null,
            null,
            null);
        Check(
            "Default Matching baseline keeps its template, pipeline, and suite contract",
            baseline.UsesDefaultMatchingBaseline
                && string.Equals(baseline.PipelineName, "DiePad500_Matching_Baseline", StringComparison.Ordinal)
                && string.Equals(baseline.SuiteName, "Die Pad 500 Matching baseline", StringComparison.Ordinal)
                && baseline.Boundary.Contains("Matching-only baseline", StringComparison.Ordinal)
                && baseline.PipelineXml.Contains("<Name>DiePad500_Matching_Baseline</Name>", StringComparison.Ordinal)
                && baseline.PipelineXml.Contains("<Value>DiePad500_Matching_Baseline</Value>", StringComparison.Ordinal)
                && baseline.PipelineXml.Contains(baselineTemplatePath, StringComparison.Ordinal)
                && string.Equals(baseline.PipelinePath, baselinePipelinePath, StringComparison.Ordinal),
            passed,
            failed);

        bool rejectedMissingFolders = false;
        try
        {
            ValidationDatasetSmokeConfiguration.Create(
                repositoryRoot,
                Path.Combine(fixtureRoot, "missing-dataset"),
                customPipelinePath,
                null,
                null,
                null,
                null,
                null);
        }
        catch (DirectoryNotFoundException)
        {
            rejectedMissingFolders = true;
        }

        Check(
            "Missing dataset roots fail closed before Recipe or Shell execution",
            rejectedMissingFolders,
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
        string reportPath = Path.Combine(evidenceDirectory, "validation-dataset-configuration-contract.txt");
        File.WriteAllLines(reportPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("VALIDATION_DATASET_CONFIGURATION_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("VALIDATION_DATASET_CONFIGURATION_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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
