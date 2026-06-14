using OpenCvSharp;
using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: VisionRecipeRunnerSmoke <imagePath> <pipelineXmlPath> [resultImagePath]");
    return 2;
}

string imagePath = Path.GetFullPath(args[0]);
string pipelineXmlPath = Path.GetFullPath(args[1]);
string? resultImagePath = args.Length >= 3 && !args[2].StartsWith("--", StringComparison.Ordinal)
    ? Path.GetFullPath(args[2])
    : null;
string? allOverlayImagePath = GetOptionValue(args, "--all-overlay-image");
bool printOverlays = args.Any(arg =>
    string.Equals(arg, "--overlays", StringComparison.OrdinalIgnoreCase)
    || string.Equals(arg, "--overlay-bounds", StringComparison.OrdinalIgnoreCase));

static string? GetOptionValue(string[] args, string optionName)
{
    for (int i = 0; i < args.Length - 1; i++)
    {
        if (string.Equals(args[i], optionName, StringComparison.OrdinalIgnoreCase))
        {
            return Path.GetFullPath(args[i + 1]);
        }
    }

    return null;
}

static void SaveAllOverlayImage(Mat sourceImage, VisionRecipeRunResult runResult, string outputPath)
{
    if (sourceImage == null || sourceImage.Empty() || runResult == null)
    {
        return;
    }

    Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
    using Mat preview = new Mat();
    if (sourceImage.Channels() == 1)
    {
        Cv2.CvtColor(sourceImage, preview, ColorConversionCodes.GRAY2BGR);
    }
    else
    {
        sourceImage.CopyTo(preview);
    }

    Scalar[] colors =
    {
        new Scalar(0, 220, 70),
        new Scalar(255, 130, 0),
        new Scalar(0, 190, 255),
        new Scalar(220, 80, 255)
    };

    int colorIndex = 0;
    foreach (VisionRecipeStepRunSummary step in runResult.Steps.Where(step => step.Overlays.Count > 0))
    {
        Scalar color = colors[colorIndex++ % colors.Length];
        foreach (VisionRecipeOverlaySummary overlay in step.Overlays)
        {
            if (!string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase)
                || overlay.BoundsWidth <= 0
                || overlay.BoundsHeight <= 0)
            {
                continue;
            }

            int x = Math.Clamp((int)Math.Round(overlay.BoundsX), 0, Math.Max(0, preview.Width - 1));
            int y = Math.Clamp((int)Math.Round(overlay.BoundsY), 0, Math.Max(0, preview.Height - 1));
            int right = Math.Clamp((int)Math.Round(overlay.BoundsX + overlay.BoundsWidth), x + 1, preview.Width);
            int bottom = Math.Clamp((int)Math.Round(overlay.BoundsY + overlay.BoundsHeight), y + 1, preview.Height);
            Rect bounds = new Rect(x, y, right - x, bottom - y);

            Cv2.Rectangle(preview, bounds, color, 2);
            string label = $"{step.Index:00}";
            Cv2.PutText(
                preview,
                label,
                new OpenCvSharp.Point(bounds.X, Math.Max(12, bounds.Y - 4)),
                HersheyFonts.HersheySimplex,
                0.42,
                color,
                1,
                LineTypes.AntiAlias);
        }
    }

    Cv2.ImWrite(outputPath, preview);
}

if (!File.Exists(imagePath))
{
    Console.Error.WriteLine($"Image was not found: {imagePath}");
    return 2;
}

if (!File.Exists(pipelineXmlPath))
{
    Console.Error.WriteLine($"Pipeline XML was not found: {pipelineXmlPath}");
    return 2;
}

using Mat source = Cv2.ImRead(imagePath, ImreadModes.Unchanged);
if (source.Empty())
{
    Console.Error.WriteLine($"Image could not be loaded: {imagePath}");
    return 2;
}

VisionRecipeRunner runner = new VisionRecipeRunner();
using VisionRecipeRunResult result = await runner.RunAsync(pipelineXmlPath, source);

Console.WriteLine($"Pipeline={result.PipelineName}");
Console.WriteLine($"Success={result.Success}");
Console.WriteLine($"Message={result.Message}");
Console.WriteLine($"FinalLayer={result.FinalLayer}");
Console.WriteLine($"FinalStep={result.FinalStepName}");
Console.WriteLine($"FinalTool={result.FinalToolType}");
Console.WriteLine($"ResultImage={result.ResultImageSizeText}");
Console.WriteLine($"TotalStepTime={result.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)} ms");

foreach (VisionRecipeStepRunSummary step in result.Steps)
{
    Console.WriteLine(
        $"{step.Index} | {step.ToolType} | {step.Status} | {step.OutputLayer} | {step.ElapsedMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)} ms | Image={step.ResultImageSizeText} | Metrics={step.MetricCount} | Overlays={step.OverlayCount}");

    if (!string.IsNullOrWhiteSpace(step.MetricsText))
    {
        Console.WriteLine($"  {step.MetricsText}");
    }

    if (printOverlays && step.Overlays.Count > 0)
    {
        for (int overlayIndex = 0; overlayIndex < step.Overlays.Count; overlayIndex++)
        {
            VisionRecipeOverlaySummary overlay = step.Overlays[overlayIndex];
            Console.WriteLine(
                "  Overlay {0:000}: {1} Bounds=({2:0.#},{3:0.#},{4:0.#},{5:0.#}) Center=({6:0.#},{7:0.#}) Label={8}",
                overlayIndex + 1,
                overlay.Kind,
                overlay.BoundsX,
                overlay.BoundsY,
                overlay.BoundsWidth,
                overlay.BoundsHeight,
                overlay.CenterX,
                overlay.CenterY,
                overlay.Label);
        }
    }
}

if (!string.IsNullOrWhiteSpace(resultImagePath) && result.ResultImage != null && !result.ResultImage.Empty())
{
    Directory.CreateDirectory(Path.GetDirectoryName(resultImagePath) ?? ".");
    Cv2.ImWrite(resultImagePath, result.ResultImage);
    Console.WriteLine($"Saved={resultImagePath}");
}

if (!string.IsNullOrWhiteSpace(allOverlayImagePath))
{
    SaveAllOverlayImage(source, result, allOverlayImagePath);
    Console.WriteLine($"SavedAllOverlays={allOverlayImagePath}");
}

List<string> failures = new List<string>();
if (!result.Success)
{
    failures.Add($"Runner returned NG: {result.Message}");
}

if (result.Steps.Count == 0)
{
    failures.Add("Runner returned no step summaries.");
}

if (string.IsNullOrWhiteSpace(result.FinalLayer))
{
    failures.Add("Runner did not resolve a final output layer.");
}

if (result.ResultImage == null || result.ResultImage.Empty())
{
    failures.Add("Runner returned no final result image.");
}

VisionRecipeStepRunSummary? finalStep = result.Steps.LastOrDefault(step => !step.Skipped);
if (finalStep == null)
{
    failures.Add("Runner returned no enabled step summary.");
}
else
{
    if (finalStep.MetricCount == 0 && finalStep.OverlayCount == 0)
    {
        failures.Add($"Final step '{finalStep.Name}' has neither metrics nor overlays.");
    }

    if (!finalStep.AcceptancePassed)
    {
        failures.Add($"Final step '{finalStep.Name}' acceptance failed: {finalStep.AcceptanceMessage}");
    }
}

if (failures.Count == 0)
{
    Console.WriteLine("Runner smoke passed.");
    return 0;
}

Console.Error.WriteLine("Runner smoke failed.");
foreach (string failure in failures)
{
    Console.Error.WriteLine($"- {failure}");
}

return 1;
