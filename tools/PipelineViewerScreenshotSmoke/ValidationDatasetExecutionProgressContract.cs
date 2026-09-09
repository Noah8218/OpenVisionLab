using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class ValidationDatasetExecutionProgressContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl32-validation-dataset-execution-progress-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Validation dataset execution progress contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "ValidationDatasetExecutionProgress.cs");
        string program = File.ReadAllText(programPath);
        string targetMethod = ExtractRegion(
            program,
            "    private static CaptureResult CaptureShellHostRecipeLocalValidationDataset(",
            "    private static void AssertReviewQueueMetricExtremes(");
        string owner = File.ReadAllText(ownerPath);

        Check(
            "Program delegates validation execution progress to the owner",
            targetMethod.Contains("ValidationDatasetExecutionProgress.Run(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Execution progress contract is discoverable from the command usage",
            program.Contains("--validation-dataset-execution-progress-contract", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Program no longer owns the progress write and polling loop",
            !targetMethod.Contains("DateTime nextProgressWrite = DateTime.MinValue", StringComparison.Ordinal)
                && !targetMethod.Contains("File.AppendAllText", StringComparison.Ordinal)
                && !targetMethod.Contains("while (DateTime.UtcNow < deadline)", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Owner keeps execution, UI pumping, status, and saved-run checks explicit",
            owner.Contains("Action execute", StringComparison.Ordinal)
                && owner.Contains("Action<int> pump", StringComparison.Ordinal)
                && owner.Contains("Func<string> statusText", StringComparison.Ordinal)
                && owner.Contains("Func<bool> hasSavedRun", StringComparison.Ordinal)
                && owner.Contains("File.AppendAllText", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Owner is independent of WPF and the Shell type",
            !owner.Contains("System.Windows", StringComparison.Ordinal)
                && !owner.Contains("OpenVisionShellHost", StringComparison.Ordinal),
            passed,
            failed);

        string successPath = Path.Combine(evidenceDirectory, "success-progress.txt");
        DateTime successClock = new DateTime(2026, 9, 9, 1, 2, 3, DateTimeKind.Utc);
        int successPumps = 0;
        int executeCount = 0;
        ValidationDatasetExecutionProgress.Run(
            successPath,
            registeredOkCount: 2,
            registeredNgCount: 1,
            canExecute: () => true,
            execute: () => executeCount++,
            statusText: () => "Saved validation run",
            hasSavedRun: () => true,
            pump: count =>
            {
                successPumps += count;
                successClock = successClock.AddSeconds(1);
            },
            utcNow: () => successClock,
            localNow: () => successClock,
            timeout: TimeSpan.FromMinutes(10),
            progressInterval: TimeSpan.FromSeconds(2));
        string[] successLines = File.ReadAllLines(successPath);
        Check(
            "Successful execution writes registration and one status checkpoint",
            executeCount == 1
                && successPumps == 20
                && successLines.Length == 2
                && successLines[0] == "Registered OK 2 / NG 1"
                && successLines[1].Contains("Saved validation run", StringComparison.Ordinal),
            passed,
            failed);

        string timeoutPath = Path.Combine(evidenceDirectory, "timeout-progress.txt");
        DateTime timeoutClock = new DateTime(2026, 9, 9, 2, 3, 4, DateTimeKind.Utc);
        int timeoutPumps = 0;
        ValidationDatasetExecutionProgress.Run(
            timeoutPath,
            registeredOkCount: 0,
            registeredNgCount: 0,
            canExecute: () => true,
            execute: () => { },
            statusText: () => "Still running",
            hasSavedRun: () => false,
            pump: count =>
            {
                timeoutPumps++;
                timeoutClock = timeoutClock.AddSeconds(1);
            },
            utcNow: () => timeoutClock,
            localNow: () => timeoutClock,
            timeout: TimeSpan.FromSeconds(3),
            progressInterval: TimeSpan.FromSeconds(2));
        string[] timeoutLines = File.ReadAllLines(timeoutPath);
        Check(
            "Unfinished execution exits at the deadline with retained progress",
            timeoutPumps > 0
                && timeoutPumps < 10
                && timeoutLines.Length >= 2
                && timeoutLines[^1].Contains("Still running", StringComparison.Ordinal),
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
        string reportPath = Path.Combine(evidenceDirectory, "validation-dataset-execution-progress-contract.txt");
        File.WriteAllLines(reportPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("VALIDATION_DATASET_EXECUTION_PROGRESS_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("VALIDATION_DATASET_EXECUTION_PROGRESS_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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
            throw new InvalidOperationException("Validation dataset execution progress method region was not found.");
        }

        return source.Substring(start, end - start);
    }
}
