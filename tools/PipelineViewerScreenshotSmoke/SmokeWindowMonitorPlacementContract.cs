using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class SmokeWindowMonitorPlacementContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl43-smoke-window-monitor-placement-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Smoke window monitor placement contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string directRunnerPath = Path.Combine(repositoryRoot, "tools", "OpenVisionLab.DirectSmokeRunner", "OpenVisionLabDirectSmokeRunner.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "SmokeWindowMonitorPlacement.cs");
        string appProjectPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "OpenVisionLab.csproj");
        string directRunner = File.ReadAllText(directRunnerPath);
        string owner = File.ReadAllText(ownerPath);
        string appProject = File.ReadAllText(appProjectPath);
        string directPlacementRegion = ExtractRegion(
            directRunner,
            "private static string PlaceWindowOnLeftmostMonitor(",
            "private static string GetClipboardTextWithRetry(");

        Check(
            "Direct scenarios delegate monitor placement to one concrete owner",
            directPlacementRegion.Contains("SmokeWindowMonitorPlacement.PlaceOnLeftmostMonitor(window)", StringComparison.Ordinal)
                && !directPlacementRegion.Contains("EnumDisplayMonitors", StringComparison.Ordinal)
                && !directPlacementRegion.Contains("SetWindowPos", StringComparison.Ordinal)
                && (directRunner.Split("PlaceWindowOnLeftmostMonitor", StringSplitOptions.None).Length - 1) >= 20,
            passed,
            failed);
        Check(
            "The owner preserves monitor selection, placement, and intersection failures",
            owner.Contains("EnumDisplayMonitors", StringComparison.Ordinal)
                && owner.Contains("OrderBy(info => info.Monitor.Left)", StringComparison.Ordinal)
                && owner.Contains("ThenBy(info => info.Monitor.Top)", StringComparison.Ordinal)
                && owner.Contains("The EXE window rectangle was unavailable before monitor placement.", StringComparison.Ordinal)
                && owner.Contains("The EXE window could not be placed on the leftmost monitor.", StringComparison.Ordinal)
                && owner.Contains("The EXE window did not intersect the selected leftmost monitor.", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Embedded composition links the monitor owner only with the Direct smoke runner",
            appProject.Contains("tools\\PipelineViewerScreenshotSmoke\\SmokeWindowMonitorPlacement.cs", StringComparison.Ordinal)
                && appProject.Contains("OpenVisionLabEnableEmbeddedSmokeRunner", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "The owner has no product, Recipe, or runner dependency",
            !owner.Contains("OpenVisionLab", StringComparison.Ordinal)
                && !owner.Contains("Recipe", StringComparison.Ordinal)
                && !owner.Contains("Program", StringComparison.Ordinal)
                && owner.Contains("CalculateCenteredPosition(", StringComparison.Ordinal)
                && owner.Contains("Intersects(", StringComparison.Ordinal),
            passed,
            failed);

        (int centeredLeft, int centeredTop) = SmokeWindowMonitorPlacement.CalculateCenteredPosition(
            0,
            0,
            1920,
            1080,
            800,
            600);
        Check(
            "Centered placement keeps a normal window inside the work area",
            centeredLeft == 560 && centeredTop == 240,
            passed,
            failed);

        (int negativeLeft, int negativeTop) = SmokeWindowMonitorPlacement.CalculateCenteredPosition(
            -1920,
            0,
            0,
            1080,
            960,
            540);
        Check(
            "Centered placement preserves negative monitor coordinates",
            negativeLeft == -1440 && negativeTop == 270,
            passed,
            failed);

        (int oversizedLeft, int oversizedTop) = SmokeWindowMonitorPlacement.CalculateCenteredPosition(
            0,
            0,
            100,
            100,
            200,
            200);
        Check(
            "Oversized windows clamp to the work-area origin",
            oversizedLeft == 0 && oversizedTop == 0,
            passed,
            failed);

        SmokeWindowMonitorPlacement.NativeRect monitor = new SmokeWindowMonitorPlacement.NativeRect
        {
            Left = 0,
            Top = 0,
            Right = 1920,
            Bottom = 1080
        };
        SmokeWindowMonitorPlacement.NativeRect overlappingWindow = new SmokeWindowMonitorPlacement.NativeRect
        {
            Left = 560,
            Top = 240,
            Right = 1360,
            Bottom = 840
        };
        SmokeWindowMonitorPlacement.NativeRect outsideWindow = new SmokeWindowMonitorPlacement.NativeRect
        {
            Left = 1920,
            Top = 0,
            Right = 2720,
            Bottom = 600
        };
        Check(
            "Intersection verification distinguishes visible and disjoint windows",
            SmokeWindowMonitorPlacement.Intersects(overlappingWindow, monitor)
                && !SmokeWindowMonitorPlacement.Intersects(outsideWindow, monitor),
            passed,
            failed);

        Check(
            "Native rectangle evidence keeps the existing width/height text format",
            overlappingWindow.ToString() == "560,240,800,600",
            passed,
            failed);

        string reportPath = Path.Combine(evidenceDirectory, "smoke-window-monitor-placement-contract.txt");
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
            Console.Error.WriteLine("SMOKE_WINDOW_MONITOR_PLACEMENT_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("SMOKE_WINDOW_MONITOR_PLACEMENT_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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
            throw new InvalidOperationException("Smoke window monitor placement wrapper region was not found.");
        }

        return source.Substring(start, end - start);
    }
}
