using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

internal static class ScreenshotCaptureLifecycleContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl26-screenshot-capture-lifecycle-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Screenshot capture lifecycle contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "ScreenshotCaptureLifecycle.cs");
        string program = File.ReadAllText(programPath);
        string owner = File.ReadAllText(ownerPath);

        Check(
            "Program delegates all capture workflows to the lifecycle owner",
            program.Contains("ScreenshotCaptureLifecycle.CaptureWindowWithContent(", StringComparison.Ordinal)
                && program.Contains("ScreenshotCaptureLifecycle.CaptureStandaloneWindow(", StringComparison.Ordinal)
                && program.Contains("ScreenshotCaptureLifecycle.CaptureElement(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Lifecycle owner contains Window creation, selection, and deterministic cleanup",
            owner.Contains("Window window = new()", StringComparison.Ordinal)
                && owner.Contains("Application.Current.Windows", StringComparison.Ordinal)
                && owner.Contains("content is IDisposable", StringComparison.Ordinal)
                && owner.Contains("window.Close()", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Pump and OpenGL diagnostics cross the boundary as explicit callbacks",
            owner.Contains("Action<int> pump", StringComparison.Ordinal)
                && owner.Contains("Action<string, DependencyObject?[]> writeOpenGlDiagnostics", StringComparison.Ordinal)
                && !owner.Contains("WriteOpenGlDiagnostics(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Program wrappers no longer contain the moved Window lifecycle implementation",
            !ExtractRegion(
                program,
                "    private static CaptureResult CaptureWindowWithContent(",
                "    private static void AssertWorkspaceLoadImageVisibleInCapture(")
                .Contains("Window window = new()", StringComparison.Ordinal)
                && !ExtractRegion(
                    program,
                    "    private static CaptureResult CaptureWindowWithContent(",
                    "    private static void AssertWorkspaceLoadImageVisibleInCapture(")
                    .Contains("foreach (Window owned in Application.Current.Windows", StringComparison.Ordinal),
            passed,
            failed);

        Application application = Application.Current
            ?? new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        int pumped = 0;
        int diagnosticRootCount = 0;
        bool verified = false;
        bool captureVerified = false;
        DisposableBorder content = new DisposableBorder
        {
            Width = 120,
            Height = 80,
            Background = System.Windows.Media.Brushes.SteelBlue
        };
        string contentPath = Path.Combine(evidenceDirectory, "content-window.png");
        CaptureResult contentResult = ScreenshotCaptureLifecycle.CaptureWindowWithContent(
            content,
            contentPath,
            120,
            80,
            () => verified = true,
            captureFloatingToolWindow: false,
            verifyCapture: path => captureVerified = string.Equals(path, contentPath, StringComparison.Ordinal),
            initialPumpCount: 2,
            captureScreen: false,
            pump: count => pumped += count,
            writeOpenGlDiagnostics: (_, roots) => diagnosticRootCount = roots.Length);

        bool contentDimensions = false;
        using (Bitmap bitmap = new Bitmap(contentPath))
        {
            contentDimensions = bitmap.Width == 120 && bitmap.Height == 80;
        }

        Check("content capture invokes verification and keeps the requested dimensions", verified
            && captureVerified
            && contentResult.Width > 0
            && contentResult.Height > 0
            && contentDimensions, passed, failed);
        Check("content capture pumps before and after verification", pumped == 14, passed, failed);
        Check("content capture passes both capture roots to diagnostics", diagnosticRootCount == 2, passed, failed);
        Check("content capture disposes disposable content and closes its temporary window", content.IsDisposed
            && !application.Windows.OfType<Window>().Any(), passed, failed);

        bool standaloneVerified = false;
        int standaloneDiagnostics = 0;
        Window standalone = new Window
        {
            Content = new Border { Width = 90, Height = 60, Background = System.Windows.Media.Brushes.DarkSeaGreen }
        };
        string standalonePath = Path.Combine(evidenceDirectory, "standalone-window.png");
        CaptureResult standaloneResult = ScreenshotCaptureLifecycle.CaptureStandaloneWindow(
            standalone,
            standalonePath,
            90,
            60,
            () => standaloneVerified = true,
            _ => { },
            (_, roots) => standaloneDiagnostics = roots.Length);
        Check("standalone capture verifies and closes the supplied window", standaloneVerified
            && standaloneResult.Width > 0
            && standaloneResult.Height > 0
            && !standalone.IsVisible
            && standaloneDiagnostics == 2
            && File.Exists(standalonePath), passed, failed);

        Border element = new Border { Background = System.Windows.Media.Brushes.SlateGray };
        string elementPath = Path.Combine(evidenceDirectory, "element.png");
        CaptureResult elementResult = ScreenshotCaptureLifecycle.CaptureElement(element, elementPath, 48, 32);
        bool elementDimensions = false;
        using (Bitmap bitmap = new Bitmap(elementPath))
        {
            elementDimensions = bitmap.Width == 48 && bitmap.Height == 32;
        }
        Check("element capture keeps the direct result and PNG contract", elementResult.Width == 48
            && elementResult.Height == 32
            && elementDimensions, passed, failed);

        List<string> report = new List<string>
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "ChecksPassed: " + passed.Count,
            "ChecksFailed: " + failed.Count
        };
        report.AddRange(passed.Select(item => "PASS: " + item));
        report.AddRange(failed.Select(item => "FAIL: " + item));
        string reportPath = Path.Combine(evidenceDirectory, "screenshot-capture-lifecycle-contract.txt");
        File.WriteAllLines(reportPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("SCREENSHOT_CAPTURE_LIFECYCLE_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("SCREENSHOT_CAPTURE_LIFECYCLE_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
        return 0;
    }

    private sealed class DisposableBorder : Border, IDisposable
    {
        internal bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
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
            throw new InvalidOperationException("Screenshot capture lifecycle wrapper region was not found.");
        }

        return source.Substring(start, end - start);
    }
}
