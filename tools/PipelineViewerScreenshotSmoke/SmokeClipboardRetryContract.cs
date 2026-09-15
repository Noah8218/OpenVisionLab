using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

internal static class SmokeClipboardRetryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl41-smoke-clipboard-retry-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Smoke clipboard retry contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string directRunnerPath = Path.Combine(repositoryRoot, "tools", "OpenVisionLab.DirectSmokeRunner", "OpenVisionLabDirectSmokeRunner.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "SmokeClipboardRetry.cs");
        string appProjectPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "OpenVisionLab.csproj");
        string program = File.ReadAllText(programPath);
        string directRunner = File.ReadAllText(directRunnerPath);
        string owner = File.ReadAllText(ownerPath);
        string appProject = File.ReadAllText(appProjectPath);

        Check(
            "Pipeline clipboard wrappers delegate to the shared retry owner",
            program.Contains("SmokeClipboardRetry.Run(", StringComparison.Ordinal)
                && !program.Contains("RunClipboardActionWithRetry", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Embedded Direct clipboard wrappers delegate to the shared retry owner",
            directRunner.Contains("SmokeClipboardRetry.Run(", StringComparison.Ordinal)
                && !directRunner.Contains("RunClipboardActionWithRetry", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "The embedded build links the same owner only when its smoke runner is enabled",
            appProject.Contains("tools\\PipelineViewerScreenshotSmoke\\SmokeClipboardRetry.cs", StringComparison.Ordinal)
                && appProject.Contains("OpenVisionLabEnableEmbeddedSmokeRunner", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Owner keeps retry policy independent from WPF, Clipboard, and product modules",
            owner.Contains("0x800401D0", StringComparison.Ordinal)
                && owner.Contains("Thread.Sleep", StringComparison.Ordinal)
                && owner.Contains("pump();", StringComparison.Ordinal)
                && !owner.Contains("System.Windows", StringComparison.Ordinal)
                && !owner.Contains("OpenVisionLab", StringComparison.Ordinal)
                && !owner.Contains("Program", StringComparison.Ordinal),
            passed,
            failed);

        int attempts = 0;
        int pumpCalls = 0;
        string value = SmokeClipboardRetry.Run(
            () =>
            {
                attempts++;
                if (attempts < 3)
                {
                    throw new COMException("clipboard busy", unchecked((int)0x800401D0));
                }

                return "clipboard-ok";
            },
            () => pumpCalls++);
        Check(
            "Target clipboard COM failures retry through the supplied pump and return the result",
            value == "clipboard-ok" && attempts == 3 && pumpCalls == 2,
            passed,
            failed);

        bool unrelatedExceptionRethrown = false;
        try
        {
            SmokeClipboardRetry.Run<string>(
                () => throw new COMException("unrelated", unchecked((int)0x80004005)),
                () => pumpCalls++);
        }
        catch (COMException ex) when ((uint)ex.ErrorCode == 0x80004005)
        {
            unrelatedExceptionRethrown = true;
        }

        Check(
            "Unrelated COM failures are not retried or translated",
            unrelatedExceptionRethrown,
            passed,
            failed);

        bool nullActionRejected = false;
        try
        {
            SmokeClipboardRetry.Run<string>(null!, () => { });
        }
        catch (ArgumentNullException ex) when (ex.ParamName == "action")
        {
            nullActionRejected = true;
        }

        bool nullPumpRejected = false;
        try
        {
            SmokeClipboardRetry.Run(() => "unused", null!);
        }
        catch (ArgumentNullException ex) when (ex.ParamName == "pump")
        {
            nullPumpRejected = true;
        }

        Check(
            "Action and pump inputs are validated at the shared owner boundary",
            nullActionRejected && nullPumpRejected,
            passed,
            failed);

        string reportPath = Path.Combine(evidenceDirectory, "smoke-clipboard-retry-contract.txt");
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
            Console.Error.WriteLine("SMOKE_CLIPBOARD_RETRY_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("SMOKE_CLIPBOARD_RETRY_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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
