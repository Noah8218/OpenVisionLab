#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class SmokeMouseInputContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl44-smoke-mouse-input-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Smoke mouse input contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string directRunnerPath = Path.Combine(repositoryRoot, "tools", "OpenVisionLab.DirectSmokeRunner", "OpenVisionLabDirectSmokeRunner.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "SmokeMouseInput.cs");
        string appProjectPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "OpenVisionLab.csproj");
        string directRunner = File.ReadAllText(directRunnerPath);
        string owner = File.ReadAllText(ownerPath);
        string appProject = File.ReadAllText(appProjectPath);

        Check(
            "Direct scenarios use one concrete mouse input owner",
            directRunner.Contains("SmokeMouseInput.ReleaseLeftButton()", StringComparison.Ordinal)
                && directRunner.Contains("SmokeMouseInput.PressLeftButton()", StringComparison.Ordinal)
                && directRunner.Contains("SmokeMouseInput.SetCursorPosOrThrow", StringComparison.Ordinal)
                && directRunner.Contains("SmokeMouseInput.RoundToScreenPixel", StringComparison.Ordinal)
                && directRunner.Contains("SmokeMouseInput.DragViaPointOnBackgroundThread", StringComparison.Ordinal)
                && !directRunner.Contains("mouse_event(", StringComparison.Ordinal)
                && !directRunner.Contains("SetCursorPos(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "The owner keeps the existing Win32 button and cursor contracts",
            owner.Contains("MouseEventLeftDown = 0x0002", StringComparison.Ordinal)
                && owner.Contains("MouseEventLeftUp = 0x0004", StringComparison.Ordinal)
                && owner.Contains("SetCursorPos failed.", StringComparison.Ordinal)
                && owner.Contains("mouse_event(MouseEventLeftDown", StringComparison.Ordinal)
                && owner.Contains("mouse_event(MouseEventLeftUp", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Background drag lifetime and failure messages stay with the input owner",
            owner.Contains("inputThread.IsBackground = true", StringComparison.Ordinal)
                && owner.Contains("inputThread.Join();", StringComparison.Ordinal)
                && owner.Contains("DateTime.UtcNow.AddSeconds(8D)", StringComparison.Ordinal)
                && owner.Contains("Mouse drag input thread did not finish within the expected time.", StringComparison.Ordinal)
                && owner.Contains("Host tab mouse drag input thread did not finish within the expected time.", StringComparison.Ordinal)
                && owner.Contains("Mouse drag input failed.", StringComparison.Ordinal)
                && owner.Contains("Host tab mouse drag input failed.", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "The caller retains WPF pumping and coordinate/state orchestration",
            directRunner.Contains("SmokeMouseInput.DragViaPointOnBackgroundThread(", StringComparison.Ordinal)
                && directRunner.Contains("                () => Pump(1));", StringComparison.Ordinal)
                && directRunner.Contains("private static void ClickDockedLayerHeaderWithoutDrag(", StringComparison.Ordinal)
                && directRunner.Contains("private static void DragHostLayerTabToWorkspace(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Embedded composition links the owner only with the Direct smoke runner",
            appProject.Contains("tools\\PipelineViewerScreenshotSmoke\\SmokeMouseInput.cs", StringComparison.Ordinal)
                && appProject.Contains("OpenVisionLabEnableEmbeddedSmokeRunner", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "The owner has no product, Recipe, or runner dependency",
            !owner.Contains("OpenVisionLab", StringComparison.Ordinal)
                && !owner.Contains("Recipe", StringComparison.Ordinal)
                && !owner.Contains("Program", StringComparison.Ordinal)
                && owner.Contains("Action pump", StringComparison.Ordinal)
                && owner.Contains("System.Windows", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Drag interpolation and delays preserve the existing gesture contract",
            owner.Contains("const int steps = 34", StringComparison.Ordinal)
                && owner.Contains("segment == 1 ? 10 : 34", StringComparison.Ordinal)
                && owner.Contains("Thread.Sleep(80)", StringComparison.Ordinal)
                && owner.Contains("Thread.Sleep(120)", StringComparison.Ordinal)
                && owner.Contains("Thread.Sleep(180)", StringComparison.Ordinal)
                && owner.Contains("segment == 1 ? 24 : 18", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Screen-pixel rounding preserves midpoint-away-from-zero behavior",
            SmokeMouseInput.RoundToScreenPixel(1.4D) == 1
                && SmokeMouseInput.RoundToScreenPixel(1.5D) == 2
                && SmokeMouseInput.RoundToScreenPixel(-1.5D) == -2
                && SmokeMouseInput.RoundToScreenPixel(-1.4D) == -1,
            passed,
            failed);
        Check(
            "The owner validates the pump seam before creating a background thread",
            owner.Contains("if (pump == null)", StringComparison.Ordinal)
                && owner.Contains("throw new ArgumentNullException(nameof(pump));", StringComparison.Ordinal)
                && owner.Contains("pump();", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "No Direct low-level mouse declarations remain after extraction",
            !directRunner.Contains("private static extern bool SetCursorPos", StringComparison.Ordinal)
                && !directRunner.Contains("private static extern void mouse_event", StringComparison.Ordinal)
                && !directRunner.Contains("MouseEventLeftDown", StringComparison.Ordinal)
                && !directRunner.Contains("MouseEventLeftUp", StringComparison.Ordinal),
            passed,
            failed);

        string reportPath = Path.Combine(evidenceDirectory, "smoke-mouse-input-contract.txt");
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
            Console.Error.WriteLine("SMOKE_MOUSE_INPUT_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("SMOKE_MOUSE_INPUT_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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
