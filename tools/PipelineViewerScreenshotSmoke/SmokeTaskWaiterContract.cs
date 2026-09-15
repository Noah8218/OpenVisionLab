using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal static class SmokeTaskWaiterContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl42-smoke-task-waiter-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Smoke task waiter contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string directRunnerPath = Path.Combine(repositoryRoot, "tools", "OpenVisionLab.DirectSmokeRunner", "OpenVisionLabDirectSmokeRunner.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "SmokeTaskWaiter.cs");
        string appProjectPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "OpenVisionLab.csproj");
        string program = File.ReadAllText(programPath);
        string directRunner = File.ReadAllText(directRunnerPath);
        string owner = File.ReadAllText(ownerPath);
        string appProject = File.ReadAllText(appProjectPath);
        string pipelineWaitRegion = ExtractRegion(
            program,
            "private static void WaitForTaskWithPump(",
            "private static CaptureResult CaptureThresholdOutputThenBlobOpen(");
        string directWaitRegion = ExtractRegion(
            directRunner,
            "private static void WaitForTaskWithPump(",
            "private static string PlaceWindowOnLeftmostMonitor(");

        Check(
            "Pipeline task waits delegate to the shared owner while preserving the null-optional wrapper",
            pipelineWaitRegion.Contains("SmokeTaskWaiter.Wait(", StringComparison.Ordinal)
                && pipelineWaitRegion.Contains("if (task == null)", StringComparison.Ordinal)
                && pipelineWaitRegion.Contains("return;", StringComparison.Ordinal)
                && pipelineWaitRegion.Contains("TimeSpan.FromMilliseconds(Math.Max(1000, timeoutMilliseconds))", StringComparison.Ordinal)
                && pipelineWaitRegion.Contains("TimeSpan.FromMilliseconds(10)", StringComparison.Ordinal)
                && pipelineWaitRegion.Contains("\" timed out.\"", StringComparison.Ordinal)
                && !pipelineWaitRegion.Contains("DateTime.UtcNow.AddMilliseconds", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Embedded Direct task waits delegate to the shared owner while preserving required null semantics",
            directWaitRegion.Contains("SmokeTaskWaiter.Wait(", StringComparison.Ordinal)
                && directWaitRegion.Contains("if (task == null)", StringComparison.Ordinal)
                && directWaitRegion.Contains("throw new ArgumentNullException(nameof(task))", StringComparison.Ordinal)
                && directWaitRegion.Contains("TimeSpan.FromSeconds(20)", StringComparison.Ordinal)
                && directWaitRegion.Contains("TimeSpan.FromMilliseconds(20)", StringComparison.Ordinal)
                && directWaitRegion.Contains("\" did not complete within 20 seconds.\"", StringComparison.Ordinal)
                && !directWaitRegion.Contains("Stopwatch.StartNew", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "The embedded build links the same WPF-free owner only when its smoke runner is enabled",
            appProject.Contains("tools\\PipelineViewerScreenshotSmoke\\SmokeTaskWaiter.cs", StringComparison.Ordinal)
                && appProject.Contains("OpenVisionLabEnableEmbeddedSmokeRunner", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Owner keeps task wait policy independent from WPF, Shell, and product modules",
            owner.Contains("Stopwatch.StartNew()", StringComparison.Ordinal)
                && owner.Contains("pump();", StringComparison.Ordinal)
                && owner.Contains("task.GetAwaiter().GetResult();", StringComparison.Ordinal)
                && !owner.Contains("System.Windows", StringComparison.Ordinal)
                && !owner.Contains("OpenVisionLab", StringComparison.Ordinal)
                && !owner.Contains("Program", StringComparison.Ordinal),
            passed,
            failed);

        int pumpCalls = 0;
        TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        Task completedTask = completion.Task;
        SmokeTaskWaiter.Wait(
            completedTask,
            "completed task",
            () =>
            {
                pumpCalls++;
                completion.TrySetResult(true);
            },
            TimeSpan.FromSeconds(2),
            TimeSpan.FromMilliseconds(5),
            " timed out.");
        Check(
            "A completed task returns and pumps while work is pending",
            completedTask.IsCompleted && pumpCalls > 0,
            passed,
            failed);

        pumpCalls = 0;
        bool timeoutPreserved = false;
        try
        {
            SmokeTaskWaiter.Wait(
                new TaskCompletionSource<bool>().Task,
                "timeout case",
                () => pumpCalls++,
                TimeSpan.FromMilliseconds(30),
                TimeSpan.FromMilliseconds(5),
                " did not complete within 20 seconds.");
        }
        catch (TimeoutException ex) when (ex.Message == "timeout case did not complete within 20 seconds.")
        {
            timeoutPreserved = true;
        }

        Check(
            "Timeouts use the caller-provided message and pump policy",
            timeoutPreserved && pumpCalls > 0,
            passed,
            failed);

        bool taskExceptionPropagated = false;
        Task failedTask = Task.FromException(new InvalidOperationException("task failure"));
        try
        {
            SmokeTaskWaiter.Wait(
                failedTask,
                "failed task",
                () => { },
                TimeSpan.FromSeconds(1),
                TimeSpan.Zero,
                " timed out.");
        }
        catch (InvalidOperationException ex) when (ex.Message == "task failure")
        {
            taskExceptionPropagated = true;
        }

        Check(
            "Completed task exceptions propagate without translation",
            taskExceptionPropagated,
            passed,
            failed);

        bool nullPumpRejected = false;
        try
        {
            SmokeTaskWaiter.Wait(
                Task.CompletedTask,
                "null pump",
                null!,
                TimeSpan.FromSeconds(1),
                TimeSpan.Zero,
                " timed out.");
        }
        catch (ArgumentNullException ex) when (ex.ParamName == "pump")
        {
            nullPumpRejected = true;
        }

        Check(
            "The shared owner validates its pump boundary",
            nullPumpRejected,
            passed,
            failed);

        string reportPath = Path.Combine(evidenceDirectory, "smoke-task-waiter-contract.txt");
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
            Console.Error.WriteLine("SMOKE_TASK_WAITER_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("SMOKE_TASK_WAITER_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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
            throw new InvalidOperationException("Smoke task waiter wrapper region was not found.");
        }

        return source.Substring(start, end - start);
    }
}
