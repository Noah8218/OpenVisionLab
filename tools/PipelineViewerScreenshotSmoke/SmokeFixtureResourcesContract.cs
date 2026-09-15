using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

internal static class SmokeFixtureResourcesContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl38-smoke-fixture-resources-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Smoke fixture resources contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "SmokeFixtureResources.cs");
        string program = File.ReadAllText(programPath);
        string owner = File.ReadAllText(ownerPath);

        Check(
            "Program imports the smoke fixture resource owner",
            program.Contains("using static SmokeFixtureResources;", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Program no longer declares the moved fixture helpers",
            !program.Contains("private static Bitmap CreateLineMeasureSmokeBitmap(", StringComparison.Ordinal)
                && !program.Contains("private static Bitmap CreateLargeSmokeBitmap(", StringComparison.Ordinal)
                && !program.Contains("private static string CreateMatchingTemplateFile(", StringComparison.Ordinal)
                && !program.Contains("private static Bitmap CreateRoiSmokeBitmap(", StringComparison.Ordinal)
                && !program.Contains("private static void TryDeleteFile(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Owner contains only fixture resource dependencies",
            !owner.Contains("OpenVisionShellHost", StringComparison.Ordinal)
                && !owner.Contains("Application.Current", StringComparison.Ordinal)
                && !owner.Contains("ScreenshotPngWriter", StringComparison.Ordinal)
                && owner.Contains("CreateLargeSmokeBitmap(", StringComparison.Ordinal)
                && owner.Contains("CreateMatchingTemplateFile(", StringComparison.Ordinal)
                && owner.Contains("TryDeleteFile(", StringComparison.Ordinal),
            passed,
            failed);

        using (Bitmap workspace = SmokeFixtureResources.CreateWorkspaceSeedSmokeBitmap())
        using (Bitmap docking = SmokeFixtureResources.CreateDockingPanelSmokeBitmap(1))
        using (Bitmap matching = SmokeFixtureResources.CreateMatchingSmokeBitmap())
        using (Bitmap large = SmokeFixtureResources.CreateLargeSmokeBitmap(64, 48, 3))
        using (Bitmap roi = SmokeFixtureResources.CreateRoiSmokeBitmap())
        {
            Check(
                "workspace, docking, matching, large, and ROI fixtures keep their expected dimensions",
                workspace.Size.Width == 512 && workspace.Size.Height == 384
                    && docking.Size.Width == 512 && docking.Size.Height == 384
                    && matching.Size.Width == 512 && matching.Size.Height == 384
                    && large.Size.Width == 64 && large.Size.Height == 48
                    && roi.Size.Width == 512 && roi.Size.Height == 384,
                passed,
                failed);

            string baselineHash = SmokeFixtureResources.ComputeStreamingBitmapSha256(large);
            using Bitmap changed = SmokeFixtureResources.CreateLargeSmokeBitmap(64, 48, 4);
            string changedHash = SmokeFixtureResources.ComputeStreamingBitmapSha256(changed);
            Check(
                "large fixtures produce stable and variation-sensitive hashes",
                baselineHash == SmokeFixtureResources.ComputeStreamingBitmapSha256(large)
                    && !string.Equals(baselineHash, changedHash, StringComparison.Ordinal),
                passed,
                failed);
        }

        string matchingTemplatePath;
        using (Bitmap matchingSource = SmokeFixtureResources.CreateMatchingSmokeBitmap())
        {
            matchingTemplatePath = SmokeFixtureResources.CreateMatchingTemplateFile(matchingSource);
        }
        try
        {
            bool matchingTemplateValid;
            using (Bitmap template = new Bitmap(matchingTemplatePath))
            {
                matchingTemplateValid = template.Width == 120 && template.Height == 96;
            }

            Check("matching template resource is created with its contract crop", matchingTemplateValid, passed, failed);
        }
        finally
        {
            SmokeFixtureResources.TryDeleteFile(matchingTemplatePath);
        }

        using (Bitmap feature = SmokeFixtureResources.CreateFeatureMatchingSmokeBitmap())
        {
            string featureTemplatePath = SmokeFixtureResources.CreateFeatureMatchingTemplateFile(feature);
            try
            {
                bool featureTemplateValid;
                using (Bitmap template = new Bitmap(featureTemplatePath))
                {
                    featureTemplateValid = template.Width == 190 && template.Height == 142;
                }

                Check("feature matching template resource is created with its contract crop", featureTemplateValid, passed, failed);
            }
            finally
            {
                SmokeFixtureResources.TryDeleteFile(featureTemplatePath);
            }
        }

        string workspacePath = SmokeFixtureResources.CreateWorkspaceLoadSmokeImageFile();
        try
        {
            Check(
                "workspace load fixture writes a readable image file",
                File.Exists(workspacePath) && new FileInfo(workspacePath).Length > 0,
                passed,
                failed);
        }
        finally
        {
            SmokeFixtureResources.TryDeleteFile(workspacePath);
        }

        using (Bitmap representative = SmokeFixtureResources.CreateMatchingSmokeBitmap())
        {
            List<string> paths = SmokeFixtureResources.CreateAutoMPointRepresentativeFiles(representative, 3);
            try
            {
                Check(
                    "AutoMPoint representative resources keep requested count",
                    paths.Count == 3 && paths.All(File.Exists),
                    passed,
                    failed);
            }
            finally
            {
                foreach (string path in paths)
                {
                    SmokeFixtureResources.TryDeleteFile(path);
                }
            }
        }

        string reportPath = Path.Combine(evidenceDirectory, "smoke-fixture-resources-contract.txt");
        List<string> report = new List<string>
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "ChecksPassed: " + passed.Count,
            "ChecksFailed: " + failed.Count
        };
        report.AddRange(passed.Select(item => "PASS: " + item));
        report.AddRange(failed.Select(item => "FAIL: " + item));
        File.WriteAllLines(reportPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("SMOKE_FIXTURE_RESOURCES_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("SMOKE_FIXTURE_RESOURCES_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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
