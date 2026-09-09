using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

internal static class ScreenshotBitmapAssertionsContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl24-screenshot-bitmap-assertions-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Screenshot bitmap contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string ownerPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "ScreenshotBitmapAssertions.cs");
        string program = File.ReadAllText(programPath);
        string owner = File.ReadAllText(ownerPath);
        string[] methodNames =
        {
            "AssertBitmapPresent",
            "SaveDiagnosticBitmap",
            "AssertBitmapVisiblyDifferent",
            "AssertBitmapPreviewOverlayDifferentFromMain",
            "AssertBitmapRetainsSourceBackground",
            "AssertBitmapBinaryLike",
            "AssertBitmapMostlyGrayscale",
            "AssertBitmapContainsColorNear"
        };

        Check(
            "Program delegates every bitmap evidence operation to the owner",
            methodNames.All(name => program.Contains("ScreenshotBitmapAssertions." + name + "(", StringComparison.Ordinal)),
            passed,
            failed);
        Check(
            "Program no longer declares the moved bitmap evidence methods",
            methodNames.All(name => !program.Contains("private static void " + name + "(", StringComparison.Ordinal)),
            passed,
            failed);
        Check(
            "Bitmap assertion owner is WPF-free and keeps PNG evidence output",
            !owner.Contains("System.Windows", StringComparison.Ordinal)
                && owner.Contains("ImageFormat.Png", StringComparison.Ordinal)
                && owner.Contains("GetPixel", StringComparison.Ordinal),
            passed,
            failed);

        string diagnosticOutputPath = Path.Combine(evidenceDirectory, "bitmap-capture.png");
        bool missingRejected = false;
        try
        {
            ScreenshotBitmapAssertions.AssertBitmapPresent(null!, "missing bitmap");
        }
        catch (InvalidOperationException ex)
        {
            missingRejected = ex.Message.Contains("missing bitmap", StringComparison.Ordinal);
        }

        using Bitmap source = new Bitmap(128, 96);
        using Bitmap changed = new Bitmap(128, 96);
        using Bitmap overlay = new Bitmap(128, 96);
        using Bitmap binary = new Bitmap(128, 96);
        using Bitmap grayscale = new Bitmap(128, 96);
        using Bitmap color = new Bitmap(128, 96);
        using (Graphics sourceGraphics = Graphics.FromImage(source))
        using (Graphics changedGraphics = Graphics.FromImage(changed))
        using (Graphics overlayGraphics = Graphics.FromImage(overlay))
        using (Graphics binaryGraphics = Graphics.FromImage(binary))
        using (Graphics grayscaleGraphics = Graphics.FromImage(grayscale))
        using (Graphics colorGraphics = Graphics.FromImage(color))
        {
            sourceGraphics.Clear(Color.FromArgb(10, 10, 10));
            changedGraphics.Clear(Color.FromArgb(240, 240, 240));
            overlayGraphics.Clear(Color.FromArgb(255, 0, 0));
            binaryGraphics.Clear(Color.Black);
            binaryGraphics.FillRectangle(Brushes.White, 64, 0, 64, 96);
            grayscaleGraphics.Clear(Color.FromArgb(120, 120, 120));
            colorGraphics.Clear(Color.Red);
        }

        bool differenceAccepted = true;
        try
        {
            ScreenshotBitmapAssertions.AssertBitmapVisiblyDifferent(source, changed, "changed bitmap");
        }
        catch
        {
            differenceAccepted = false;
        }

        bool identicalRejected = false;
        try
        {
            ScreenshotBitmapAssertions.AssertBitmapVisiblyDifferent(source, source, "identical bitmap");
        }
        catch (InvalidOperationException ex)
        {
            identicalRejected = ex.Message.Contains("identical bitmap", StringComparison.Ordinal);
        }

        bool overlayAccepted = true;
        bool backgroundAccepted = true;
        bool binaryAccepted = true;
        bool grayscaleAccepted = true;
        bool colorAccepted = true;
        try
        {
            ScreenshotBitmapAssertions.AssertBitmapPreviewOverlayDifferentFromMain(source, overlay, "overlay bitmap");
            ScreenshotBitmapAssertions.AssertBitmapRetainsSourceBackground(source, source, "background bitmap");
            ScreenshotBitmapAssertions.AssertBitmapBinaryLike(binary, "binary bitmap");
            ScreenshotBitmapAssertions.AssertBitmapMostlyGrayscale(grayscale, "grayscale bitmap");
            ScreenshotBitmapAssertions.AssertBitmapContainsColorNear(color, Color.Red, 1, "color bitmap");
        }
        catch (InvalidOperationException ex)
        {
            overlayAccepted = ex.Message.Contains("overlay bitmap", StringComparison.Ordinal);
            backgroundAccepted = ex.Message.Contains("background bitmap", StringComparison.Ordinal);
            binaryAccepted = ex.Message.Contains("binary bitmap", StringComparison.Ordinal);
            grayscaleAccepted = ex.Message.Contains("grayscale bitmap", StringComparison.Ordinal);
            colorAccepted = ex.Message.Contains("color bitmap", StringComparison.Ordinal);
        }

        ScreenshotBitmapAssertions.SaveDiagnosticBitmap(diagnosticOutputPath, "source.png", source);
        Check("invalid bitmap reports the supplied context", missingRejected, passed, failed);
        Check("visibly different images remain accepted", differenceAccepted, passed, failed);
        Check("unchanged images remain rejected by the difference assertion", identicalRejected, passed, failed);
        Check("overlay, background, binary, grayscale, and color assertions keep their contracts", overlayAccepted
            && backgroundAccepted
            && binaryAccepted
            && grayscaleAccepted
            && colorAccepted, passed, failed);
        Check(
            "diagnostic bitmap writer keeps the evidence directory contract",
            File.Exists(Path.Combine(evidenceDirectory, "bitmap-capture.diagnostics", "source.png")),
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
        string reportPath = Path.Combine(evidenceDirectory, "screenshot-bitmap-assertions-contract.txt");
        File.WriteAllLines(reportPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("SCREENSHOT_BITMAP_ASSERTIONS_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("SCREENSHOT_BITMAP_ASSERTIONS_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
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