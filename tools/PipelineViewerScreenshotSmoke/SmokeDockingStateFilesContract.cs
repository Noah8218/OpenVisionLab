using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenVisionLab;

internal static class SmokeDockingStateFilesContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl39-smoke-docking-state-files-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Smoke docking state files contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string directRunnerPath = Path.Combine(repositoryRoot, "tools", "OpenVisionLab.DirectSmokeRunner", "OpenVisionLabDirectSmokeRunner.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "SmokeDockingStateFiles.cs");
        string appProjectPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "OpenVisionLab.csproj");
        string program = File.ReadAllText(programPath);
        string directRunner = File.ReadAllText(directRunnerPath);
        string owner = File.ReadAllText(ownerPath);
        string appProject = File.ReadAllText(appProjectPath);

        Check(
            "Pipeline screenshot smoke delegates persisted docking backup to the owner",
            program.Contains("SmokeDockingStateFiles.RunWithBackup(", StringComparison.Ordinal)
                && !program.Contains("private static CaptureResult WithDockingStateFileBackup(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Embedded direct smoke delegates backup, clear, and copy operations to the owner",
            ExtractRegion(
                    directRunner,
                    "        private static void CopyCurrentDockingStateFile(",
                    "        private static void Pump(")
                .Contains("SmokeDockingStateFiles.", StringComparison.Ordinal)
                && !ExtractRegion(
                    directRunner,
                    "        private static void CopyCurrentDockingStateFile(",
                    "        private static void Pump(")
                    .Contains("LayerDocking.layers", StringComparison.Ordinal)
                && !ExtractRegion(
                    directRunner,
                    "        private static void CopyCurrentDockingStateFile(",
                    "        private static void Pump(")
                    .Contains("File.ReadAllBytes", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "The embedded build links the same owner only when its smoke runner is enabled",
            appProject.Contains("tools\\PipelineViewerScreenshotSmoke\\SmokeDockingStateFiles.cs", StringComparison.Ordinal)
                && appProject.Contains("OpenVisionLabEnableEmbeddedSmokeRunner", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Owner contains the two stable files and no WPF or Shell dependency",
            owner.Contains("LayerDocking.layers", StringComparison.Ordinal)
                && owner.Contains("LayerDocking.layout", StringComparison.Ordinal)
                && owner.Contains("RunWithBackupCore", StringComparison.Ordinal)
                && owner.Contains("Clear(string uiConfigDirectory)", StringComparison.Ordinal)
                && owner.Contains("CopyCurrent(", StringComparison.Ordinal)
                && !owner.Contains("Application.Current", StringComparison.Ordinal)
                && !owner.Contains("OpenVisionShellHost", StringComparison.Ordinal),
            passed,
            failed);

        string stateDirectory = Path.Combine(evidenceDirectory, "state");
        Directory.CreateDirectory(stateDirectory);
        string layersPath = Path.Combine(stateDirectory, "LayerDocking.layers");
        string layoutPath = Path.Combine(stateDirectory, "LayerDocking.layout");
        string extraPath = Path.Combine(stateDirectory, "unrelated.tmp");
        byte[] originalLayers = new byte[] { 1, 2, 3, 4 };
        byte[] originalLayout = new byte[] { 5, 6, 7 };
        File.WriteAllBytes(layersPath, originalLayers);
        File.WriteAllBytes(layoutPath, originalLayout);

        int returned = SmokeDockingStateFiles.RunWithBackup(stateDirectory, () =>
        {
            File.WriteAllBytes(layersPath, new byte[] { 9 });
            File.Delete(layoutPath);
            File.WriteAllBytes(extraPath, new byte[] { 8 });
            return 42;
        });
        Check(
            "Backup restores existing files and preserves the callback result",
            returned == 42
                && originalLayers.SequenceEqual(File.ReadAllBytes(layersPath))
                && originalLayout.SequenceEqual(File.ReadAllBytes(layoutPath)),
            passed,
            failed);
        File.Delete(extraPath);

        File.Delete(layoutPath);
        SmokeDockingStateFiles.RunWithBackup(stateDirectory, () =>
        {
            File.WriteAllBytes(layersPath, new byte[] { 10 });
            File.WriteAllBytes(layoutPath, new byte[] { 11 });
            return true;
        });
        Check(
            "Backup removes a file that did not exist before the callback",
            originalLayers.SequenceEqual(File.ReadAllBytes(layersPath)) && !File.Exists(layoutPath),
            passed,
            failed);

        File.WriteAllBytes(layoutPath, originalLayout);
        bool exceptionRethrown = false;
        try
        {
            SmokeDockingStateFiles.RunWithBackup(stateDirectory, () =>
            {
                File.WriteAllBytes(layersPath, new byte[] { 12 });
                throw new InvalidOperationException("contract failure");
            });
        }
        catch (InvalidOperationException ex) when (ex.Message == "contract failure")
        {
            exceptionRethrown = true;
        }
        Check(
            "Backup restores state before rethrowing a callback exception",
            exceptionRethrown
                && originalLayers.SequenceEqual(File.ReadAllBytes(layersPath))
                && originalLayout.SequenceEqual(File.ReadAllBytes(layoutPath)),
            passed,
            failed);

        SmokeDockingStateFiles.Clear(stateDirectory);
        Check(
            "Clear removes both persisted docking state files",
            !File.Exists(layersPath) && !File.Exists(layoutPath),
            passed,
            failed);

        File.WriteAllBytes(layersPath, originalLayers);
        string copyDirectory = Path.Combine(evidenceDirectory, "copy");
        Directory.CreateDirectory(copyDirectory);
        SmokeDockingStateFiles.CopyCurrent(stateDirectory, "LayerDocking.layers", copyDirectory, "copied.layers");
        Check(
            "CopyCurrent writes the selected state file to evidence",
            originalLayers.SequenceEqual(File.ReadAllBytes(Path.Combine(copyDirectory, "copied.layers"))),
            passed,
            failed);

        string reportPath = Path.Combine(evidenceDirectory, "smoke-docking-state-files-contract.txt");
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
            Console.Error.WriteLine("SMOKE_DOCKING_STATE_FILES_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("SMOKE_DOCKING_STATE_FILES_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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

    private static string ExtractRegion(string source, string startMarker, string endMarker)
    {
        int start = source.IndexOf(startMarker, StringComparison.Ordinal);
        int end = source.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
        if (start < 0 || end < 0)
        {
            throw new InvalidOperationException("Smoke docking state wrapper region was not found.");
        }

        return source.Substring(start, end - start);
    }
}
