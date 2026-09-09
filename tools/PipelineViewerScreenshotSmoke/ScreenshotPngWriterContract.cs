using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

internal static class ScreenshotPngWriterContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl25-screenshot-png-writer-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Screenshot PNG writer contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "ScreenshotPngWriter.cs");
        string program = File.ReadAllText(programPath);
        string owner = File.ReadAllText(ownerPath);

        Check(
            "Program delegates all PNG output paths to the writer",
            program.Contains("ScreenshotPngWriter.WriteScreenPng(", StringComparison.Ordinal)
                && program.Contains("ScreenshotPngWriter.WriteElementPng(", StringComparison.Ordinal)
                && program.Contains("ScreenshotPngWriter.WriteVisibleElementPng(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Program no longer declares the moved PNG writer methods",
            !program.Contains("private static void WriteScreenPng(", StringComparison.Ordinal)
                && !program.Contains("private static void WriteElementPng(", StringComparison.Ordinal)
                && !program.Contains("private static void WriteVisibleElementPng(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Writer owns WPF rendering and screen capture",
            owner.Contains("RenderTargetBitmap", StringComparison.Ordinal)
                && owner.Contains("PngBitmapEncoder", StringComparison.Ordinal)
                && owner.Contains("CopyFromScreen", StringComparison.Ordinal)
                && owner.Contains("ImageFormat.Png", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Writer has no dependency on the smoke entry point or product modules",
            !owner.Contains("Program", StringComparison.Ordinal)
                && !owner.Contains("OpenVisionLab", StringComparison.Ordinal),
            passed,
            failed);

        string elementPath = Path.Combine(evidenceDirectory, "element.png");
        string visiblePath = Path.Combine(evidenceDirectory, "visible.png");
        Border element = new Border
        {
            Width = 48,
            Height = 32,
            Background = System.Windows.Media.Brushes.SteelBlue
        };
        ScreenshotPngWriter.WriteElementPng(element, elementPath, 48, 32);
        ScreenshotPngWriter.WriteVisibleElementPng(element, visiblePath);

        bool elementSize = false;
        bool visibleSize = false;
        using (Bitmap elementBitmap = new Bitmap(elementPath))
        {
            elementSize = elementBitmap.Width == 48 && elementBitmap.Height == 32;
        }

        using (Bitmap visibleBitmap = new Bitmap(visiblePath))
        {
            visibleSize = visibleBitmap.Width == 48 && visibleBitmap.Height == 32;
        }

        Check("element rendering keeps the requested PNG dimensions", elementSize, passed, failed);
        Check("visible element rendering keeps the arranged PNG dimensions", visibleSize, passed, failed);
        Check(
            "screen capture remains represented by the same writer owner",
            owner.Contains("window.PointToScreen", StringComparison.Ordinal)
                && owner.Contains("graphics.CopyFromScreen", StringComparison.Ordinal),
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
        string reportPath = Path.Combine(evidenceDirectory, "screenshot-png-writer-contract.txt");
        File.WriteAllLines(reportPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("SCREENSHOT_PNG_WRITER_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("SCREENSHOT_PNG_WRITER_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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
