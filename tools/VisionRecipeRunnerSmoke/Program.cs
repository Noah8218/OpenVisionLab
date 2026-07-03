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

    int thickness = Math.Max(2, Math.Min(sourceImage.Width, sourceImage.Height) / 260);
    int labelThickness = Math.Max(1, thickness - 1);
    int colorIndex = 0;
    foreach (VisionRecipeStepRunSummary step in runResult.Steps.Where(step => step.Overlays.Count > 0))
    {
        Scalar color = colors[colorIndex++ % colors.Length];
        DrawStepRoiOverlay(preview, step, color, thickness, labelThickness);
        foreach (VisionRecipeOverlaySummary overlay in step.Overlays)
        {
            if (string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase))
            {
                DrawRectangleOverlay(preview, overlay, color, thickness, labelThickness, $"{step.Index:00}");
            }
            else if (string.Equals(overlay.Kind, "Line", StringComparison.OrdinalIgnoreCase))
            {
                DrawLineOverlay(preview, overlay, color, thickness, labelThickness, $"{step.Index:00}");
            }
            else if (string.Equals(overlay.Kind, "Point", StringComparison.OrdinalIgnoreCase))
            {
                DrawPointOverlay(preview, overlay.CenterX, overlay.CenterY, color, thickness, $"{step.Index:00}");
            }
            else if (string.Equals(overlay.Kind, "Points", StringComparison.OrdinalIgnoreCase))
            {
                DrawPointsOverlay(preview, overlay, color, thickness);
            }
        }
    }

    Cv2.ImWrite(outputPath, preview);
}

static void DrawStepRoiOverlay(
    Mat preview,
    VisionRecipeStepRunSummary step,
    Scalar color,
    int thickness,
    int labelThickness)
{
    if (step?.Parameters == null
        || !TryGetBoolParameter(step.Parameters, "USE_ROI")
        || !TryGetRectParameter(step.Parameters, "CvROI", out Rect roi)
        || roi.Width <= 0
        || roi.Height <= 0)
    {
        return;
    }

    Rect bounds = ClampRect(preview, roi);
    if (bounds.Width <= 0
        || bounds.Height <= 0
        || (bounds.X <= 0 && bounds.Y <= 0 && bounds.Width >= preview.Width - 1 && bounds.Height >= preview.Height - 1))
    {
        return;
    }

    Cv2.Rectangle(preview, bounds, color, Math.Max(1, thickness - 1), LineTypes.AntiAlias);
    DrawLabel(preview, $"{step.Index:00} ROI", new OpenCvSharp.Point(bounds.X + 3, bounds.Y + 14), color, labelThickness);
}

static bool TryGetBoolParameter(IReadOnlyDictionary<string, string> parameters, string key)
{
    if (parameters == null || !parameters.TryGetValue(key, out string? value))
    {
        return false;
    }

    return bool.TryParse(value, out bool result) && result;
}

static bool TryGetRectParameter(IReadOnlyDictionary<string, string> parameters, string key, out Rect roi)
{
    roi = default;
    if (parameters == null || !parameters.TryGetValue(key, out string? value) || string.IsNullOrWhiteSpace(value))
    {
        return false;
    }

    string[] tokens = value.Split(',');
    if (tokens.Length != 4)
    {
        return false;
    }

    if (!int.TryParse(tokens[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)
        || !int.TryParse(tokens[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)
        || !int.TryParse(tokens[2].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int width)
        || !int.TryParse(tokens[3].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int height))
    {
        return false;
    }

    roi = new Rect(x, y, width, height);
    return true;
}

static Rect ClampRect(Mat preview, Rect rect)
{
    int x = Math.Clamp(rect.X, 0, Math.Max(0, preview.Width - 1));
    int y = Math.Clamp(rect.Y, 0, Math.Max(0, preview.Height - 1));
    int right = Math.Clamp(rect.X + rect.Width, x + 1, preview.Width);
    int bottom = Math.Clamp(rect.Y + rect.Height, y + 1, preview.Height);
    return new Rect(x, y, right - x, bottom - y);
}

static void DrawRectangleOverlay(
    Mat preview,
    VisionRecipeOverlaySummary overlay,
    Scalar color,
    int thickness,
    int labelThickness,
    string label)
{
    if (overlay.BoundsWidth <= 0 || overlay.BoundsHeight <= 0)
    {
        return;
    }

    int x = Math.Clamp((int)Math.Round(overlay.BoundsX), 0, Math.Max(0, preview.Width - 1));
    int y = Math.Clamp((int)Math.Round(overlay.BoundsY), 0, Math.Max(0, preview.Height - 1));
    int right = Math.Clamp((int)Math.Round(overlay.BoundsX + overlay.BoundsWidth), x + 1, preview.Width);
    int bottom = Math.Clamp((int)Math.Round(overlay.BoundsY + overlay.BoundsHeight), y + 1, preview.Height);
    Rect bounds = new Rect(x, y, right - x, bottom - y);

    Cv2.Rectangle(preview, bounds, color, thickness, LineTypes.AntiAlias);
    DrawCross(preview, ClampPoint(preview, overlay.CenterX, overlay.CenterY), color, thickness);
    DrawLabel(preview, label, new OpenCvSharp.Point(bounds.X, Math.Max(14, bounds.Y - 5)), color, labelThickness);
}

static void DrawLineOverlay(
    Mat preview,
    VisionRecipeOverlaySummary overlay,
    Scalar color,
    int thickness,
    int labelThickness,
    string label)
{
    OpenCvSharp.Point start = ClampPoint(preview, overlay.StartX, overlay.StartY);
    OpenCvSharp.Point end = ClampPoint(preview, overlay.EndX, overlay.EndY);
    if (start == end)
    {
        if (Math.Abs(overlay.CenterX) < 0.001 && Math.Abs(overlay.CenterY) < 0.001)
        {
            return;
        }

        double angleRadians = overlay.Angle * Math.PI / 180.0;
        if (double.IsNaN(angleRadians) || double.IsInfinity(angleRadians))
        {
            angleRadians = 0.0;
        }

        double halfLength = Math.Max(24.0, Math.Min(preview.Width, preview.Height) * 0.18);
        OpenCvSharp.Point centerPoint = ClampPoint(preview, overlay.CenterX, overlay.CenterY);
        start = ClampPoint(
            preview,
            (float)(centerPoint.X - Math.Cos(angleRadians) * halfLength),
            (float)(centerPoint.Y - Math.Sin(angleRadians) * halfLength));
        end = ClampPoint(
            preview,
            (float)(centerPoint.X + Math.Cos(angleRadians) * halfLength),
            (float)(centerPoint.Y + Math.Sin(angleRadians) * halfLength));
    }

    Cv2.Line(preview, start, end, color, thickness, LineTypes.AntiAlias);
    Cv2.Circle(preview, start, Math.Max(3, thickness + 1), color, -1, LineTypes.AntiAlias);
    Cv2.Circle(preview, end, Math.Max(3, thickness + 1), color, -1, LineTypes.AntiAlias);
    OpenCvSharp.Point center = new OpenCvSharp.Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
    DrawCross(preview, center, color, thickness);
    DrawLabel(preview, label, new OpenCvSharp.Point(center.X + 5, center.Y - 5), color, labelThickness);
}

static void DrawPointOverlay(Mat preview, float centerX, float centerY, Scalar color, int thickness, string label)
{
    OpenCvSharp.Point center = ClampPoint(preview, centerX, centerY);
    int radius = Math.Max(5, thickness * 3);
    Cv2.Circle(preview, center, radius, color, thickness, LineTypes.AntiAlias);
    DrawCross(preview, center, color, thickness);
    DrawLabel(preview, label, new OpenCvSharp.Point(center.X + radius + 2, center.Y), color, Math.Max(1, thickness - 1));
}

static void DrawPointsOverlay(Mat preview, VisionRecipeOverlaySummary overlay, Scalar color, int thickness)
{
    if (overlay.Points == null || overlay.Points.Count == 0)
    {
        if (Math.Abs(overlay.CenterX) > 0.001 || Math.Abs(overlay.CenterY) > 0.001)
        {
            DrawPointOverlay(preview, overlay.CenterX, overlay.CenterY, color, thickness, string.Empty);
        }

        return;
    }

    int radius = Math.Max(2, thickness + 1);
    int count = 0;
    foreach (VisionRecipeOverlayPointSummary point in overlay.Points)
    {
        if (count >= 500)
        {
            break;
        }

        OpenCvSharp.Point clamped = ClampPoint(preview, point.X, point.Y);
        Cv2.Circle(preview, clamped, radius, color, -1, LineTypes.AntiAlias);
        count++;
    }
}

static void DrawCross(Mat preview, OpenCvSharp.Point center, Scalar color, int thickness)
{
    int radius = Math.Max(5, thickness * 4);
    Cv2.Line(preview, new OpenCvSharp.Point(center.X - radius, center.Y), new OpenCvSharp.Point(center.X + radius, center.Y), color, thickness, LineTypes.AntiAlias);
    Cv2.Line(preview, new OpenCvSharp.Point(center.X, center.Y - radius), new OpenCvSharp.Point(center.X, center.Y + radius), color, thickness, LineTypes.AntiAlias);
}

static void DrawLabel(Mat preview, string label, OpenCvSharp.Point anchor, Scalar color, int thickness)
{
    if (string.IsNullOrWhiteSpace(label))
    {
        return;
    }

    OpenCvSharp.Point point = new OpenCvSharp.Point(
        Math.Clamp(anchor.X, 0, Math.Max(0, preview.Width - 1)),
        Math.Clamp(anchor.Y, 12, Math.Max(12, preview.Height - 1)));

    Cv2.PutText(
        preview,
        label,
        point,
        HersheyFonts.HersheySimplex,
        0.46,
        color,
        thickness,
        LineTypes.AntiAlias);
}

static OpenCvSharp.Point ClampPoint(Mat preview, float x, float y)
{
    return new OpenCvSharp.Point(
        Math.Clamp((int)Math.Round(x), 0, Math.Max(0, preview.Width - 1)),
        Math.Clamp((int)Math.Round(y), 0, Math.Max(0, preview.Height - 1)));
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
Console.WriteLine($"SchemaVersion={result.SchemaVersion}");
Console.WriteLine($"Success={result.Success}");
Console.WriteLine($"Outcome={result.OutcomeText}");
Console.WriteLine($"Message={result.Message}");
Console.WriteLine($"Summary={result.SummaryText}");
Console.WriteLine($"ActionSummary={result.ActionSummaryText}");
Console.WriteLine($"StepSummary={result.StepSummaryText}");
Console.WriteLine($"Normalization={result.NormalizationText}");
Console.WriteLine($"FinalLayer={result.FinalLayer}");
Console.WriteLine($"FinalStep={result.FinalStepName}");
Console.WriteLine($"FinalTool={result.FinalToolType}");
Console.WriteLine($"ResultImage={result.ResultImageSizeText}");
Console.WriteLine($"HasFinalResultImage={result.HasFinalResultImage}");
Console.WriteLine($"FinalMetricCount={result.FinalMetricCount}");
Console.WriteLine($"FinalOverlayCount={result.FinalOverlayCount}");
Console.WriteLine($"FinalMetrics={result.FinalMetricsText}");
Console.WriteLine($"TotalStepTime={result.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)} ms");
Console.WriteLine($"StepCount={result.StepCount}");
Console.WriteLine($"PassedStepCount={result.PassedStepCount}");
Console.WriteLine($"FailedStepCount={result.FailedStepCount}");
Console.WriteLine($"SkippedStepCount={result.SkippedStepCount}");
Console.WriteLine(result.HasFailedStep
    ? $"FirstFailedStep={result.FirstFailedStepIndex}|{result.FirstFailedStepName}|{result.FirstFailedErrorCode}:{result.FirstFailedErrorName}|{result.FirstFailedResultStatus}"
    : "FirstFailedStep=None");
Console.WriteLine($"FirstFailedSummary={result.FirstFailedSummaryText}");
Console.WriteLine($"FirstFailedDiagnostic={(result.HasFailedStep ? result.FirstFailedDiagnosticHint : string.Empty)}");
Console.WriteLine($"FirstFailedSuggestedFix={(result.HasFailedStep ? result.FirstFailedSuggestedFix : string.Empty)}");

foreach (VisionRecipeStepRunSummary step in result.Steps)
{
    Console.WriteLine(
        $"{step.Index} | {step.ToolType} | {step.Status} | {step.OutputLayer} | {step.ElapsedMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)} ms | Image={step.ResultImageSizeText} | Metrics={step.MetricCount} | Overlays={step.OverlayCount} | Error={step.ErrorCode}:{step.ErrorName}");

    if (!string.IsNullOrWhiteSpace(step.MetricsText))
    {
        Console.WriteLine($"  {step.MetricsText}");
    }

    if (!string.IsNullOrWhiteSpace(step.DiagnosticHint))
    {
        Console.WriteLine($"  Diagnostic={step.DiagnosticHint}");
    }

    if (!string.IsNullOrWhiteSpace(step.SuggestedFix))
    {
        Console.WriteLine($"  SuggestedFix={step.SuggestedFix}");
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
            if (string.Equals(overlay.Kind, "Line", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(
                    "    Line Start=({0:0.#},{1:0.#}) End=({2:0.#},{3:0.#}) Angle={4:0.###}",
                    overlay.StartX,
                    overlay.StartY,
                    overlay.EndX,
                    overlay.EndY,
                    overlay.Angle);
            }
            else if (string.Equals(overlay.Kind, "Points", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"    Points={overlay.Points.Count}");
            }
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
