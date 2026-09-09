using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class ScreenshotSmokeTargetRunnerContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl22-screenshot-smoke-target-runner-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Smoke runner evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        string captureDirectory = Path.Combine(evidenceDirectory, "captures");
        Directory.CreateDirectory(captureDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string runnerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "ScreenshotSmokeTargetRunner.cs");
        string program = File.ReadAllText(programPath);
        string runner = File.ReadAllText(runnerPath);

        Check(
            "Program delegates target execution to the runner",
            program.Contains("ScreenshotSmokeTargetRunner.CaptureTargets(", StringComparison.Ordinal)
                && program.Contains("ScreenshotSmokeTargetRunner.ExpandSuites(", StringComparison.Ordinal)
                && program.Contains("ScreenshotSmokeTargetRunner.PrintTargetsAndSuites(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Program no longer owns the target execution and suite expansion methods",
            !program.Contains("private static int CaptureTargets(", StringComparison.Ordinal)
                && !program.Contains("private static IReadOnlyList<string> ExpandSuites(", StringComparison.Ordinal)
                && !program.Contains("private static string[] SplitNames(", StringComparison.Ordinal)
                && !program.Contains("private static void PrintTargetsAndSuites(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "CaptureResult is owned by the runner module",
            runner.Contains("internal readonly record struct CaptureResult", StringComparison.Ordinal)
                && !program.Contains("record struct CaptureResult", StringComparison.Ordinal),
            passed,
            failed);

        Dictionary<string, Func<string, CaptureResult>> targets = new Dictionary<string, Func<string, CaptureResult>>(StringComparer.OrdinalIgnoreCase)
        {
            ["ok"] = path =>
            {
                File.WriteAllText(path, "ok");
                return new CaptureResult(12, 8, 3.5D);
            },
            ["throws"] = _ => throw new InvalidOperationException("expected target failure")
        };
        TextWriter originalOutput = Console.Out;
        TextWriter originalError = Console.Error;
        StringWriter output = new StringWriter();
        StringWriter error = new StringWriter();
        int successCode;
        int mixedCode;
        try
        {
            Console.SetOut(output);
            Console.SetError(error);
            successCode = ScreenshotSmokeTargetRunner.CaptureTargets(
                captureDirectory,
                new[] { "ok" },
                targets);
            mixedCode = ScreenshotSmokeTargetRunner.CaptureTargets(
                captureDirectory,
                new[] { "missing", "throws" },
                targets);
        }
        finally
        {
            Console.SetOut(originalOutput);
            Console.SetError(originalError);
        }

        string outputText = output.ToString();
        string errorText = error.ToString();
        Check(
            "successful target keeps the capture result contract",
            successCode == 0
                && File.Exists(Path.Combine(captureDirectory, "ok.png"))
                && outputText.Contains("ok=OK", StringComparison.Ordinal)
                && outputText.Contains("size=12x8", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "unknown target is reported without invoking a capture",
            mixedCode == 1
                && outputText.Contains("missing=NG", StringComparison.Ordinal)
                && !File.Exists(Path.Combine(captureDirectory, "missing.png")),
            passed,
            failed);
        Check(
            "capture exceptions are reported and persisted",
            mixedCode == 1
                && outputText.Contains("throws=NG", StringComparison.Ordinal)
                && errorText.Contains("throws: expected target failure", StringComparison.Ordinal)
                && File.Exists(Path.Combine(captureDirectory, "throws.png.error.txt")),
            passed,
            failed);

        string[] split = ScreenshotSmokeTargetRunner.SplitNames(" alpha, ,B ");
        Check(
            "target names preserve trim and empty-entry behavior",
            split.SequenceEqual(new[] { "alpha", "B" }, StringComparer.Ordinal),
            passed,
            failed);

        Dictionary<string, string[]> suites = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["first"] = new[] { "ok", "shared" },
            ["second"] = new[] { "shared", "throws" }
        };
        IReadOnlyList<string> expanded = ScreenshotSmokeTargetRunner.ExpandSuites(
            new[] { "first", "second" },
            suites);
        Check(
            "suite expansion preserves order and removes duplicate targets",
            expanded.SequenceEqual(new[] { "ok", "shared", "throws" }, StringComparer.Ordinal),
            passed,
            failed);

        bool unknownSuiteRejected = false;
        try
        {
            ScreenshotSmokeTargetRunner.ExpandSuites(new[] { "unknown" }, suites);
        }
        catch (InvalidOperationException ex)
        {
            unknownSuiteRejected = ex.Message.Contains("Unknown smoke suite 'unknown'", StringComparison.Ordinal);
        }
        Check("unknown suites remain a command-line error", unknownSuiteRejected, passed, failed);

        StringWriter catalogOutput = new StringWriter();
        try
        {
            Console.SetOut(catalogOutput);
            ScreenshotSmokeTargetRunner.PrintTargetsAndSuites(
                new Dictionary<string, Func<string, CaptureResult>>(StringComparer.OrdinalIgnoreCase)
                {
                    ["z-target"] = _ => new CaptureResult(1, 1, 0D),
                    ["a-target"] = _ => new CaptureResult(1, 1, 0D)
                },
                new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["z-suite"] = new[] { "z-target" },
                    ["a-suite"] = new[] { "a-target" }
                });
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
        string catalogText = catalogOutput.ToString();
        Check(
            "catalog output remains sorted and discoverable",
            catalogText.IndexOf("  a-suite:", StringComparison.Ordinal) < catalogText.IndexOf("  z-suite:", StringComparison.Ordinal)
                && catalogText.IndexOf("  a-target", StringComparison.Ordinal) < catalogText.IndexOf("  z-target", StringComparison.Ordinal),
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
        string reportPath = Path.Combine(evidenceDirectory, "screenshot-smoke-target-runner-contract.txt");
        File.WriteAllLines(reportPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("SMOKE_TARGET_RUNNER_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("SMOKE_TARGET_RUNNER_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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
