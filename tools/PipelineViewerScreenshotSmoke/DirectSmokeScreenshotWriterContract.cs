using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

internal static class DirectSmokeScreenshotWriterContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "direct-smoke-screenshot-writer-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Direct smoke screenshot writer evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string directRunnerPath = Path.Combine(repositoryRoot, "tools", "OpenVisionLab.DirectSmokeRunner", "OpenVisionLabDirectSmokeRunner.cs");
        string writerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "ScreenshotPngWriter.cs");
        string appProjectPath = Path.Combine(repositoryRoot, "src", "OpenVisionLab", "OpenVisionLab.csproj");
        string directRunner = File.ReadAllText(directRunnerPath);
        string writer = File.ReadAllText(writerPath);
        string appProject = File.ReadAllText(appProjectPath);

        string windowWriterRegion = ExtractRegion(
            directRunner,
            "        private static void SaveWindowScreenshot(",
            "        private static void SaveWindowScreenScreenshot(");
        string screenWriterRegion = ExtractRegion(
            directRunner,
            "        private static void SaveWindowScreenScreenshot(",
            "        private static void BringWindowToFront(");
        string windowsWriterRegion = ExtractRegion(
            directRunner,
            "        private static void SaveWindowsScreenScreenshot(",
            "        private static void RunLayerInitialDockedWorkspace(");

        Check(
            "Direct window capture wrapper delegates DPI-aware PNG output",
            windowWriterRegion.Contains("ScreenshotPngWriter.WriteDpiAwareWindowPng(", StringComparison.Ordinal)
                && !windowWriterRegion.Contains("RenderTargetBitmap", StringComparison.Ordinal)
                && !windowWriterRegion.Contains("PngBitmapEncoder", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Direct screen capture wrapper delegates single-window PNG output",
            screenWriterRegion.Contains("ScreenshotPngWriter.WriteWindowScreenPng(", StringComparison.Ordinal)
                && !screenWriterRegion.Contains("CopyFromScreen", StringComparison.Ordinal)
                && !screenWriterRegion.Contains("ImageFormat.Png", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Direct multi-window capture wrapper delegates union PNG output",
            windowsWriterRegion.Contains("ScreenshotPngWriter.WriteWindowsScreenPng(", StringComparison.Ordinal)
                && !windowsWriterRegion.Contains("Rect.Union", StringComparison.Ordinal)
                && !windowsWriterRegion.Contains("CopyFromScreen", StringComparison.Ordinal)
                && !windowsWriterRegion.Contains("ImageFormat.Png", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "The existing PNG owner contains all Direct capture contracts",
            writer.Contains("WriteDpiAwareWindowPng(Window window", StringComparison.Ordinal)
                && writer.Contains("WriteWindowScreenPng(Window window", StringComparison.Ordinal)
                && writer.Contains("WriteWindowsScreenPng(IEnumerable<Window> windows", StringComparison.Ordinal)
                && writer.Contains("CopyFromScreen", StringComparison.Ordinal)
                && writer.Contains("RenderTargetBitmap", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "The embedded app links the existing PNG owner only for Direct smoke",
            appProject.Contains("tools\\PipelineViewerScreenshotSmoke\\ScreenshotPngWriter.cs", StringComparison.Ordinal)
                && appProject.Contains("OpenVisionLabEnableEmbeddedSmokeRunner", StringComparison.Ordinal),
            passed,
            failed);

        Application application = Application.Current
            ?? new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        Window window = new Window
        {
            Content = new Border { Width = 96, Height = 64, Background = System.Windows.Media.Brushes.SteelBlue },
            Width = 96,
            Height = 64,
            WindowStyle = WindowStyle.None,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false
        };
        string windowPath = Path.Combine(evidenceDirectory, "dpi-aware-window.png");
        try
        {
            window.Show();
            ScreenshotPngWriter.WriteDpiAwareWindowPng(window, windowPath);
        }
        finally
        {
            window.Close();
        }

        bool windowWritten = File.Exists(windowPath);
        bool windowHasPixels = false;
        if (windowWritten)
        {
            using Bitmap bitmap = new Bitmap(windowPath);
            windowHasPixels = bitmap.Width > 0 && bitmap.Height > 0;
        }
        Check(
            "DPI-aware writer renders a supplied WPF window to a PNG",
            windowWritten && windowHasPixels && !application.Windows.OfType<Window>().Any(),
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
        string reportPath = Path.Combine(evidenceDirectory, "direct-smoke-screenshot-writer-contract.txt");
        File.WriteAllLines(reportPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("DIRECT_SMOKE_SCREENSHOT_WRITER_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("DIRECT_SMOKE_SCREENSHOT_WRITER_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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
            throw new InvalidOperationException("Direct smoke screenshot writer wrapper region was not found.");
        }

        return source.Substring(start, end - start);
    }
}
