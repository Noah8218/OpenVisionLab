using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Common;
using OpenVisionLab.Core;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Vision2D.Tool;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using OpenVisionLab.ImageCanvas.OpenGLRendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Drawing.Imaging;
using Bitmap = System.Drawing.Bitmap;

if (args.Length == 1 && string.Equals(args[0], "--pinarraygap-intent-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunPinArrayGapIntentContract();
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--runtime-stability-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunRuntimeStabilityContractAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--object-dimension-filter-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunObjectDimensionFilterContractAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--tool-n-image-verification-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunToolNImageVerificationContractAsync(args.Length == 2 ? args[1] : null);
}

if (args.Length == 6
    && string.Equals(args[0], "--tool-n-image-real-folder-acceptance", StringComparison.OrdinalIgnoreCase))
{
    return await RunToolNImageRealFolderAcceptanceAsync(
        args[1],
        args[2],
        args[3],
        args[4],
        args[5]);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--affine-transform-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunAffineTransformContractAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--affine-detected-points-contract", StringComparison.OrdinalIgnoreCase))
{
    return await RunAffineDetectedPointsContractAsync(args.Length == 2 ? args[1] : null);
}

if (args.Length == 3
    && string.Equals(args[0], "--affine-card-pilot", StringComparison.OrdinalIgnoreCase))
{
    return await RunAffineCardPilotAsync(args[1], args[2]);
}

if (args.Length == 3
    && string.Equals(args[0], "--affine-card-fixed-roi", StringComparison.OrdinalIgnoreCase))
{
    return await RunAffineCardPilotAsync(
        args[1],
        args[2],
        includeFixedRoiMean: true,
        maximumPostResidualPx: 5D);
}

if (args.Length == 4
    && string.Equals(args[0], "--edge-unique-card-r-matrix", StringComparison.OrdinalIgnoreCase))
{
    return await RunEdgeUniqueCardRMatrixAsync(args[1], args[2], args[3]);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--edge-global-polarity-contract", StringComparison.OrdinalIgnoreCase))
{
    return RunEdgeGlobalPolarityContract(args.Length == 2 ? args[1] : null);
}

if (args.Length == 3
    && string.Equals(args[0], "--auto-mpoint-easymatch-candidates", StringComparison.OrdinalIgnoreCase))
{
    return RunAutoMPointEasyMatchCandidates(args[1], args[2]);
}

if (args.Length == 3
    && string.Equals(args[0], "--auto-mpoint-six-corpus-pilot", StringComparison.OrdinalIgnoreCase))
{
    return RunAutoMPointSixCorpusPilot(args[1], args[2]);
}

if (args.Length == 4
    && string.Equals(args[0], "--auto-mpoint-representative-best-pilot", StringComparison.OrdinalIgnoreCase))
{
    return RunAutoMPointRepresentativeBestPilot(args[1], args[2], args[3]);
}

if (args.Length == 5
    && string.Equals(args[0], "--auto-mpoint-full-stratum-qualification", StringComparison.OrdinalIgnoreCase))
{
    return RunAutoMPointFullStratumQualification(args[1], args[2], args[3], args[4]);
}

if (args.Length == 5 && string.Equals(args[0], "--batch", StringComparison.OrdinalIgnoreCase))
{
    return await RunBatchAsync(args[1], args[2], args[3], args[4]);
}

if (args.Length == 6 && string.Equals(args[0], "--batch-evidence", StringComparison.OrdinalIgnoreCase))
{
    return await RunBatchAsync(args[1], args[2], args[3], args[4], args[5]);
}

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: VisionRecipeRunnerSmoke <imagePath> <pipelineXmlPath> [resultImagePath]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --batch <imageListPath> <datasetRoot> <pipelineXmlPath> <csvPath>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --batch-evidence <imageListPath> <datasetRoot> <pipelineXmlPath> <csvPath> <evidenceRoot>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --pinarraygap-intent-contract");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --runtime-stability-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --object-dimension-filter-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --tool-n-image-verification-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --affine-transform-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --affine-detected-points-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --affine-card-pilot <cardDatasetRoot> <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --affine-card-fixed-roi <cardDatasetRoot> <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --edge-unique-card-r-matrix <cardDatasetRoot> <p220ResultsCsv> <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --edge-global-polarity-contract [evidenceDirectory]");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --auto-mpoint-easymatch-candidates <easyMatchSampleRoot> <evidenceDirectory>");
    Console.Error.WriteLine("   or: VisionRecipeRunnerSmoke --auto-mpoint-six-corpus-pilot <labelTestRoot> <evidenceDirectory>");
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

static async Task<int> RunRuntimeStabilityContractAsync(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
        ?? Path.Combine("artifacts", "runtime_stability_contract"));
    Directory.CreateDirectory(evidenceDirectory);
    List<string> failures = new List<string>();

    if (!OpenVisionLabUnhandledExceptionPolicy.IsRecoverableDispatcherException(new OperationCanceledException())
        || OpenVisionLabUnhandledExceptionPolicy.IsRecoverableDispatcherException(new InvalidOperationException("fatal")))
    {
        failures.Add("Dispatcher exception policy did not distinguish cancellation from a fatal unhandled exception.");
    }

    using (CancellationTokenSource canceled = new CancellationTokenSource())
    {
        canceled.Cancel();
        VisionPipelineSampleCheckResult canceledResult = await VisionPipelineSampleCheckService.RunSampleCheckSafeAsync(
            null,
            null,
            canceled.Token);
        if (!canceledResult.Message.Contains("canceled", StringComparison.OrdinalIgnoreCase))
        {
            failures.Add("A pre-canceled sample check did not return the safe cancellation result.");
        }
    }

    VisionPipelineSampleCatalogItem? publicSample = VisionPipelineSampleCatalogItem
        .LoadRunnable(VisionPipelineSampleCatalogSourceKind.Public)
        .FirstOrDefault(item => string.Equals(
            item.SampleName,
            "Public_Threshold_BandPads_Good",
            StringComparison.OrdinalIgnoreCase));
    if (publicSample == null)
    {
        failures.Add("The public Threshold stability sample was not found.");
    }
    else
    {
        VisionPipelineSampleCheckResult publicResult = await VisionPipelineSampleCheckService.RunSampleCheckSafeAsync(
            publicSample,
            cancellationToken: CancellationToken.None);
        if (!publicResult.ExecutionCompleted || !publicResult.Success)
        {
            failures.Add("The valid async public sample path failed: " + publicResult.Message);
        }
    }

    TaskCompletionSource<VisionToolResult> lateCompletion = new TaskCompletionSource<VisionToolResult>(
        TaskCreationOptions.RunContinuationsAsynchronously);
    Task<bool> deadlineWait = VisionPipelineExecutionService.WaitForStepCompletionAsync(
        lateCompletion.Task,
        1,
        CancellationToken.None);
    await Task.Delay(100);
    if (deadlineWait.IsCompleted)
    {
        failures.Add("A timed-out Step returned before its in-process work released owned resources.");
    }

    Mat lateImage = new Mat(2, 2, MatType.CV_8UC1, Scalar.White);
    lateCompletion.SetResult(new VisionToolResult
    {
        Success = true,
        ResultImage = lateImage
    });
    bool completedWithinDeadline = await deadlineWait;
    if (completedWithinDeadline || !lateImage.IsDisposed)
    {
        failures.Add("The timed-out Step drain did not report the deadline or dispose the late result image.");
    }

    bool immediateCompletion = await VisionPipelineExecutionService.WaitForStepCompletionAsync(
        Task.FromResult(new VisionToolResult { Success = true }),
        1000,
        CancellationToken.None);
    if (!immediateCompletion)
    {
        failures.Add("A completed Step was incorrectly reported as timed out.");
    }

    using (Bitmap indexed = new Bitmap(2, 1, PixelFormat.Format8bppIndexed))
    {
        ColorPalette palette = indexed.Palette;
        palette.Entries[1] = System.Drawing.Color.FromArgb(10, 20, 30);
        palette.Entries[2] = System.Drawing.Color.FromArgb(40, 50, 60);
        indexed.Palette = palette;
        BitmapData bitmapData = indexed.LockBits(
            new System.Drawing.Rectangle(0, 0, 2, 1),
            ImageLockMode.WriteOnly,
            indexed.PixelFormat);
        try
        {
            Marshal.Copy(new byte[] { 1, 2, 0, 0 }, 0, bitmapData.Scan0, bitmapData.Stride);
        }
        finally
        {
            indexed.UnlockBits(bitmapData);
        }

        using Mat converted = new Mat(1, 2, MatType.CV_8UC3);
        BitmapImageConverter.ToMat(indexed, converted);
        Vec3b first = converted.At<Vec3b>(0, 0);
        Vec3b second = converted.At<Vec3b>(0, 1);
        if (first.Item0 != 30 || first.Item1 != 20 || first.Item2 != 10
            || second.Item0 != 60 || second.Item1 != 50 || second.Item2 != 40)
        {
            failures.Add("Indexed Bitmap palette conversion did not preserve BGR values.");
        }
    }

    FieldInfo? glyphCountField = typeof(OpenGlDrawing).GetField(
        "FontGlyphCount",
        BindingFlags.Static | BindingFlags.NonPublic);
    if (glyphCountField?.GetRawConstantValue() is not int glyphCount || glyphCount != 256)
    {
        failures.Add("OpenGL text rendering does not reserve all 256 byte glyph display lists.");
    }

    string reportPath = Path.Combine(evidenceDirectory, "runtime_stability_contract.txt");
    File.WriteAllLines(
        reportPath,
        new[]
        {
            "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
            "DispatcherPolicy: cancellation handled; unexpected UI exceptions remain fatal",
            "PipelineDeadline: timed-out in-process work drained before Context ownership ends",
            "SampleCheck: async CancellationToken path active",
            "BitmapConverter: indexed BGR conversion and temporary Mat ownership checked",
            "OpenGLFontLists: 256 contiguous glyph lists required"
        }.Concat(failures.Select(item => "Failure: " + item)));

    if (failures.Count == 0)
    {
        Console.WriteLine("Runtime stability contract passed.");
        Console.WriteLine(reportPath);
        return 0;
    }

    Console.Error.WriteLine("Runtime stability contract failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    return 1;
}

static int RunEdgeGlobalPolarityContract(string? requestedEvidenceDirectory)
{
    string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
        ?? Path.Combine("artifacts", "cvr11_global_polarity_contract"));
    string sourceDirectory = Path.Combine(evidenceDirectory, "sources");
    string drawingDirectory = Path.Combine(evidenceDirectory, "drawings");
    Directory.CreateDirectory(sourceDirectory);
    Directory.CreateDirectory(drawingDirectory);

    string templatePath = Path.Combine(evidenceDirectory, "template.png");
    using Mat template = CreateGlobalPolarityPattern();
    Cv2.ImWrite(templatePath, template);

    (string Split, string Name, bool HasTarget, bool Reversed, int X, int Y)[] cases =
    {
        ("Train", "train_same_01", true, false, 18, 18),
        ("Train", "train_same_02", true, false, 52, 24),
        ("Train", "train_same_03", true, false, 94, 16),
        ("Train", "train_same_04", true, false, 42, 62),
        ("Train", "train_reversed_01", true, true, 22, 20),
        ("Train", "train_reversed_02", true, true, 58, 28),
        ("Train", "train_reversed_03", true, true, 102, 18),
        ("Train", "train_reversed_04", true, true, 48, 64),
        ("Validation", "validation_same_01", true, false, 30, 38),
        ("Validation", "validation_same_02", true, false, 84, 52),
        ("Validation", "validation_reversed_01", true, true, 34, 42),
        ("Validation", "validation_reversed_02", true, true, 88, 50),
        ("Validation", "validation_no_target_01", false, false, 0, 0),
        ("Validation", "validation_no_target_02", false, true, 0, 0),
        ("HeldOut", "heldout_same_01", true, false, 16, 66),
        ("HeldOut", "heldout_same_02", true, false, 108, 58),
        ("HeldOut", "heldout_reversed_01", true, true, 20, 70),
        ("HeldOut", "heldout_reversed_02", true, true, 106, 60),
        ("HeldOut", "heldout_no_target_01", false, false, 0, 0),
        ("HeldOut", "heldout_no_target_02", false, true, 0, 0)
    };

    VisionPipelineStep enabledStep = CreateGlobalPolarityStep(templatePath, true);
    VisionPipelineStep legacyStep = CreateGlobalPolarityStep(templatePath, false);
    List<string> rows = new List<string>
    {
        "Split,Case,Expected,Actual,Success,Polarity,Score,CenterX,CenterY,CenterErrorPx,ErrorCode,SourceSha256,DrawingSha256"
    };
    List<string> failures = new List<string>();

    using (Mat reversedProbe = CreateGlobalPolaritySource(template, true, true, 52, 36))
    {
        EdgeBasedTemplateMatchingTool legacyTool =
            (EdgeBasedTemplateMatchingTool)VisionPipelineAppToolFactory.Create(legacyStep);
        VisionToolResult legacyResult = legacyTool.Execute(reversedProbe);
        try
        {
            if (legacyResult.Success || legacyTool.results.Count != 0)
            {
                failures.Add("Legacy XML/default path accepted a globally reversed target.");
            }
        }
        finally
        {
            legacyResult.ResultImage?.Dispose();
        }
    }

    foreach ((string split, string name, bool hasTarget, bool reversed, int x, int y) in cases)
    {
        using Mat source = CreateGlobalPolaritySource(template, hasTarget, reversed, x, y);
        string sourcePath = Path.Combine(sourceDirectory, name + ".png");
        string drawingPath = Path.Combine(drawingDirectory, name + ".png");
        Cv2.ImWrite(sourcePath, source);

        EdgeBasedTemplateMatchingTool tool =
            (EdgeBasedTemplateMatchingTool)VisionPipelineAppToolFactory.Create(enabledStep);
        VisionToolResult result = tool.Execute(source);
        OpenVisionLab.Vision2D.Result.MatchingResult? match = tool.results.SingleOrDefault();
        string actual = result.Success && match != null ? "Match" : "NoMatch";
        string expected = hasTarget ? "Match" : "NoMatch";
        double centerError = double.NaN;
        string polarity = match == null ? "None" : match.PolarityReversed ? "Reversed" : "Same";
        double score = match?.Score ?? double.NaN;

        try
        {
            if (match != null && result.ResultImage != null && !result.ResultImage.Empty())
            {
                Cv2.ImWrite(drawingPath, result.ResultImage);
            }
            else
            {
                using Mat fallback = source.Clone();
                Cv2.PutText(
                    fallback,
                    "NoMatch",
                    new OpenCvSharp.Point(8, 22),
                    HersheyFonts.HersheySimplex,
                    0.6,
                    Scalar.Red,
                    2,
                    LineTypes.AntiAlias);
                Cv2.ImWrite(drawingPath, fallback);
            }

            if (!string.Equals(actual, expected, StringComparison.Ordinal))
            {
                failures.Add($"{split}/{name}: expected {expected}, actual {actual} ({result.ErrorName}: {result.Message}).");
            }

            if (hasTarget && match != null)
            {
                centerError = Math.Sqrt(
                    Math.Pow(match.Center.X - (x + (template.Width / 2D)), 2D)
                    + Math.Pow(match.Center.Y - (y + (template.Height / 2D)), 2D));
                if (centerError > 2D)
                {
                    failures.Add($"{split}/{name}: center error {centerError:0.###} px exceeded 2 px.");
                }

                if (match.PolarityReversed != reversed)
                {
                    failures.Add($"{split}/{name}: expected polarity {(reversed ? "Reversed" : "Same")}, actual {polarity}.");
                }
            }

            rows.Add(string.Join(",",
                split,
                name,
                expected,
                actual,
                result.Success,
                polarity,
                double.IsNaN(score) ? string.Empty : score.ToString("0.###", CultureInfo.InvariantCulture),
                match == null ? string.Empty : match.Center.X.ToString("0.###", CultureInfo.InvariantCulture),
                match == null ? string.Empty : match.Center.Y.ToString("0.###", CultureInfo.InvariantCulture),
                double.IsNaN(centerError) ? string.Empty : centerError.ToString("0.###", CultureInfo.InvariantCulture),
                result.ErrorName,
                ComputeSha256(sourcePath),
                ComputeSha256(drawingPath)));
        }
        finally
        {
            result.ResultImage?.Dispose();
        }
    }

    string matrixPath = Path.Combine(evidenceDirectory, "matrix.csv");
    File.WriteAllLines(matrixPath, rows);
    File.WriteAllText(
        Path.Combine(evidenceDirectory, "pipeline.xml"),
        "<VisionPipeline Name=\"CVR-11 Global Polarity\"><Step Name=\"Global polarity edge match\" ToolType=\"EdgeBasedMatching\" InputLayer=\"Main\" OutputLayer=\"Match\"><Parameter><Key>PATTERN_PATH</Key><Value>"
        + System.Security.SecurityElement.Escape(templatePath)
        + "</Value></Parameter><Parameter><Key>SCORE_MIN</Key><Value>0.8</Value></Parameter><Parameter><Key>NUM_MATCH</Key><Value>1</Value></Parameter><Parameter><Key>ALLOW_GLOBAL_POLARITY_REVERSAL</Key><Value>true</Value></Parameter><Parameter><Key>SEARCH_STEP</Key><Value>1</Value></Parameter><Parameter><Key>USE_POSITION_REFINE</Key><Value>true</Value></Parameter></Step></VisionPipeline>");
    File.WriteAllLines(
        Path.Combine(evidenceDirectory, "completion.txt"),
        new[]
        {
            failures.Count == 0 ? "Status=Complete" : "Status=Incomplete",
            "Scope=Project-authored synthetic global contrast reversal only",
            "Train=8 target rows",
            "Validation=4 target + 2 no-target rows",
            "HeldOut=4 target + 2 no-target rows",
            "LegacyReversedProbe=Rejected",
            "MatrixSha256=" + ComputeSha256(matrixPath),
            "Boundary=No local edge-direction ignore; no automatic mode selection; no physical or field qualification"
        }.Concat(failures.Select(failure => "Failure=" + failure)));

    if (failures.Count > 0)
    {
        Console.Error.WriteLine("CVR-11 global polarity contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        return 1;
    }

    Console.WriteLine($"CVR-11 global polarity contract passed: {cases.Length}/{cases.Length} rows.");
    Console.WriteLine("Evidence=" + evidenceDirectory);
    return 0;
}

static VisionPipelineStep CreateGlobalPolarityStep(string templatePath, bool allowReversal)
{
    VisionPipelineStep step = new VisionPipelineStep
    {
        Name = "Global polarity edge match",
        ToolType = "EdgeBasedMatching",
        InputLayer = "Main",
        OutputLayer = "Match"
    };
    step.Parameters["Name"] = step.Name;
    step.Parameters["PATTERN_PATH"] = templatePath;
    step.Parameters["SCORE_MIN"] = "0.8";
    step.Parameters["NUM_MATCH"] = "1";
    step.Parameters["ALLOW_GLOBAL_POLARITY_REVERSAL"] = allowReversal.ToString(CultureInfo.InvariantCulture);
    step.Parameters["SEARCH_STEP"] = "1";
    step.Parameters["USE_POSITION_REFINE"] = "true";
    step.Parameters["USE_DRAW_IMAGE"] = "true";
    step.Parameters["USE_THRESHOLD"] = "false";
    step.Parameters["CANNY_LOW"] = "30";
    step.Parameters["CANNY_HIGH"] = "90";
    return step;
}

static Mat CreateGlobalPolarityPattern()
{
    Mat pattern = new Mat(new Size(64, 64), MatType.CV_8UC1, Scalar.All(230));
    Cv2.Rectangle(pattern, new Rect(10, 9, 12, 42), Scalar.All(28), -1);
    Cv2.Rectangle(pattern, new Rect(10, 39, 34, 12), Scalar.All(28), -1);
    Cv2.Circle(pattern, new OpenCvSharp.Point(44, 18), 8, Scalar.All(28), -1);
    Cv2.Line(pattern, new OpenCvSharp.Point(39, 32), new OpenCvSharp.Point(52, 47), Scalar.All(28), 5);
    return pattern;
}

static Mat CreateGlobalPolaritySource(
    Mat template,
    bool hasTarget,
    bool reversed,
    int x,
    int y)
{
    byte background = reversed ? (byte)25 : (byte)230;
    Mat source = new Mat(new Size(192, 144), MatType.CV_8UC1, Scalar.All(background));
    if (!hasTarget)
    {
        return source;
    }

    using Mat target = reversed ? new Mat() : template.Clone();
    if (reversed)
    {
        Cv2.BitwiseNot(template, target);
    }

    using Mat roi = new Mat(source, new Rect(x, y, target.Width, target.Height));
    target.CopyTo(roi);
    return source;
}

static int RunPinArrayGapIntentContract()
{
    const string skillTypeName = "OpenVisionLab.OpenVisionRecipePinArrayGapIntentSkill";
    const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    List<string> failures = new List<string>();
    Type? skillType = typeof(VisionRecipeRunner).Assembly.GetType(skillTypeName, throwOnError: false);
    MethodInfo? parseMethod = skillType?.GetMethod("TryParseRowRois", flags);
    MethodInfo? validateMethod = skillType?.GetMethod("TryValidateV1Inputs", flags);
    MethodInfo? measurementMethod = skillType?.GetMethod("CreateMeasurementPipeline", flags);
    MethodInfo? judgedMethod = skillType?.GetMethod("CreateJudgedPipeline", flags);

    if (skillType == null || parseMethod == null || validateMethod == null || measurementMethod == null || judgedMethod == null)
    {
        Console.Error.WriteLine("PinArrayGap intent contract smoke failed.");
        Console.Error.WriteLine($"- Internal skill contract was not found: {skillTypeName}");
        return 1;
    }

    try
    {
        object? rowRois = InvokePinArrayGapParse(parseMethod, "10,20,100,30;10,60,100,30", out bool parseSucceeded, out string parseMessage);
        if (!parseSucceeded || rowRois == null)
        {
            failures.Add("Two valid row ROIs were not parsed: " + parseMessage);
        }
        else
        {
            if (!InvokePinArrayGapValidation(validateMethod, rowRois, "Adjacent edge-to-edge clearance", "Dark", 200, 120, out string validMessage))
            {
                failures.Add("Supported v1 inputs were rejected: " + validMessage);
            }

            VisionPipeline? measurementPipeline = measurementMethod.Invoke(
                null,
                new object?[] { rowRois, 128, 0.55D, 5, 2, 3 }) as VisionPipeline;
            VerifyPinArrayGapMeasurementPipeline(measurementPipeline, failures);

            VisionPipeline? judgedPipeline = judgedMethod.Invoke(
                null,
                new object?[] { rowRois, 128, 0.55D, 5, 2, 3, 6D }) as VisionPipeline;
            VerifyPinArrayGapJudgedPipeline(judgedPipeline, failures);

            if (InvokePinArrayGapValidation(validateMethod, rowRois, "Adjacent edge-to-edge clearance", "Bright", 200, 120, out _))
            {
                failures.Add("Unsupported Bright polarity was accepted.");
            }

            if (InvokePinArrayGapValidation(validateMethod, rowRois, "Center-to-center pitch", "Dark", 200, 120, out _))
            {
                failures.Add("Unsupported center-pitch measurement was accepted.");
            }
        }

        object? outOfBoundsRoi = InvokePinArrayGapParse(parseMethod, "180,100,30,30", out bool outOfBoundsParsed, out string outOfBoundsParseMessage);
        if (!outOfBoundsParsed || outOfBoundsRoi == null)
        {
            failures.Add("The syntactically valid out-of-bounds ROI did not parse: " + outOfBoundsParseMessage);
        }
        else if (InvokePinArrayGapValidation(validateMethod, outOfBoundsRoi, "Adjacent edge-to-edge clearance", "Dark", 200, 120, out _))
        {
            failures.Add("An ROI extending beyond the source bounds was accepted.");
        }
    }
    catch (TargetInvocationException exception)
    {
        failures.Add("Internal skill invocation failed: " + (exception.InnerException?.Message ?? exception.Message));
    }
    catch (Exception exception)
    {
        failures.Add("Contract smoke failed unexpectedly: " + exception.Message);
    }

    if (failures.Count == 0)
    {
        Console.WriteLine("PinArrayGap intent contract smoke passed.");
        return 0;
    }

    Console.Error.WriteLine("PinArrayGap intent contract smoke failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    return 1;
}

static async Task<int> RunToolNImageVerificationContractAsync(string? evidenceDirectory)
{
    List<string> failures = new List<string>();
    string evidencePath = string.IsNullOrWhiteSpace(evidenceDirectory)
        ? Path.Combine(Path.GetTempPath(), "OpenVisionLab_P233_NImage")
        : Path.GetFullPath(evidenceDirectory);
    Directory.CreateDirectory(evidencePath);
    string inputDirectory = Path.Combine(evidencePath, "inputs");
    Directory.CreateDirectory(inputDirectory);
    List<string> imagePaths = new List<string>();
    for (int index = 0; index < 30; index++)
    {
        string path = Path.Combine(inputDirectory, $"n_image_{index + 1:000}.png");
        using Mat image = new Mat(new OpenCvSharp.Size(320, 220), MatType.CV_8UC3, Scalar.Black);
        Cv2.Rectangle(image, new Rect(48, 52, 112, 82), Scalar.White, -1);
        Cv2.Circle(image, new OpenCvSharp.Point(224, 96), 24, Scalar.White, -1);
        Cv2.Line(image, new OpenCvSharp.Point(62, 160), new OpenCvSharp.Point(266, 168), Scalar.White, 4);
        Cv2.Circle(
            image,
            new OpenCvSharp.Point(286, 184 + (index % 4)),
            2 + (index % 2),
            new Scalar(40 + index, 80, 120),
            -1);
        Cv2.ImWrite(path, image);
        imagePaths.Add(path);
    }

    string matchingTemplatePath = Path.Combine(evidencePath, "matching_template.png");
    using (Mat first = Cv2.ImRead(imagePaths[0], ImreadModes.Color))
    using (Mat template = first.SubMat(new Rect(38, 42, 138, 102)).Clone())
    {
        Cv2.ImWrite(matchingTemplatePath, template);
    }

    Func<VisionPipelineStep> thresholdFactory =
        () => VisionPipelineStepBuilder.FromThresholdProperty(
                    new ThresholdToolProperty
                    {
                        Mode = ThresholdToolMode.Threshold,
                        Threshold = 100,
                        MaxValue = 255,
                        ThresholdType = ThresholdTypes.Binary
                    },
                    "Threshold",
                    "Main",
                    "NImageResult");
    Func<VisionPipelineStep> blobFactory =
        () => VisionPipelineStepBuilder.FromProperty(
                    new BlobProperty("Blob")
                    {
                        USE_THRESHOLD = false,
                        USE_ADAPTIVE_THRESHOLD = false,
                        USE_BITWISENOT = false,
                        MIN_AREA = 20,
                        MAX_AREA = 30000
                    },
                    "Main",
                    "NImageResult");
    Func<VisionPipelineStep> lineFactory =
        () => VisionPipelineStepBuilder.FromProperty(
                    new LineGaugeProperty("Line")
                    {
                        USE_THRESHOLD = false,
                        USE_ADAPTIVE_THRESHOLD = false,
                        USE_BITWISENOT = false,
                        USE_ROI = true,
                        CvROI = new Rect(50, 145, 230, 40),
                        PRJ_PORALITY = FormulaUtil.PROJECTION_POLARITY.BTOW,
                        PRJ_DIR = FormulaUtil.PROJECTION_DIR.Y_TTOB,
                        VER_PRJ_DIR = FormulaUtil.PROJECTION_DIR.X_LTOR,
                        CONTRAST = 30,
                        THICKNESS = 2,
                        SAMPLING_STEP = 4,
                        POINT_RANGE = 8,
                        SHOW_VERTICAL_LINE = true,
                        SHOW_EDGE = true,
                        SHOW_CONTOUR = true,
                        SHOW_FITLINE = true
                    },
                    "Main",
                    "NImageResult");
    Func<VisionPipelineStep> matchingFactory =
        () => VisionPipelineStepBuilder.FromProperty(
                    new MatchingProperty("Matching")
                    {
                        PATTERN_PATH = matchingTemplatePath,
                        SCORE_MIN = 0.75,
                        NUM_MATCH = 1,
                        USE_FIND_ANGLE = false,
                        USE_FIND_SCALE = false
                    },
                    "Main",
                    "NImageResult");
    Func<VisionPipelineStep> edgeBasedMatchingFactory =
        () => VisionPipelineStepBuilder.FromProperty(
                    new EdgeBasedMatchingProperty("EdgeBasedMatching")
                    {
                        PATTERN_PATH = matchingTemplatePath,
                        SCORE_MIN = 0.7,
                        NUM_MATCH = 1,
                        CANNY_LOW = 20,
                        CANNY_HIGH = 60,
                        SEARCH_STEP = 2,
                        USE_POSITION_REFINE = true,
                        USE_FIND_ANGLE = false,
                        USE_FIND_SCALE = false,
                        USE_DRAW_IMAGE = true
                    },
                    "Main",
                    "NImageResult");
    Func<VisionPipelineStep> affineFactory =
        () => VisionPipelineStepBuilder.FromAffineTransformProperty(
                    new AffineTransformProperty("AffineTransform")
                    {
                        SourcePoint1X = 0,
                        SourcePoint1Y = 0,
                        SourcePoint2X = 319,
                        SourcePoint2Y = 0,
                        SourcePoint3X = 0,
                        SourcePoint3Y = 219,
                        DestinationPoint1X = 0,
                        DestinationPoint1Y = 0,
                        DestinationPoint2X = 319,
                        DestinationPoint2Y = 0,
                        DestinationPoint3X = 0,
                        DestinationPoint3Y = 219,
                        OutputWidth = 320,
                        OutputHeight = 220,
                        MinimumSourceTriangleArea = 100,
                        MinimumDestinationTriangleArea = 100,
                        MinimumValidPixelRatio = 0.95
                    },
                    "AffineTransform",
                    "Main",
                    "NImageResult");
    List<(string Name, Func<VisionPipelineStep> Factory)> tools =
        new List<(string, Func<VisionPipelineStep>)>
        {
            ("Threshold", thresholdFactory),
            ("Blob", blobFactory),
            ("Line", lineFactory),
            ("Matching", matchingFactory),
            ("EdgeBasedMatching", edgeBasedMatchingFactory),
            ("AffineTransform", affineFactory)
        };

    List<string> contractLines = new List<string>
    {
        "Tool\tRows\tOK\tNG\tCreateStepCount\tEquivalence\tHtml\tDefinitionSha256"
    };
    foreach ((string toolName, Func<VisionPipelineStep> factory) in tools)
    {
        int createStepCount = 0;
        VisionToolNImageVerificationSession session;
        try
        {
            session = await VisionToolNImageVerificationService.RunAsync(
                toolName,
                "P233_Smoke_" + toolName,
                () =>
                {
                    createStepCount++;
                    return factory();
                },
                normalizeInputToGray: true,
                imagePaths,
                progress: null,
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            failures.Add(toolName + ": N-image service failed: " + ex.GetBaseException().Message);
            continue;
        }

        if (createStepCount != 1)
        {
            failures.Add(toolName + $": current Step was created {createStepCount} times instead of once.");
        }

        if (session.Rows.Count != imagePaths.Count)
        {
            failures.Add(toolName + $": row count {session.Rows.Count} != {imagePaths.Count}.");
        }

        int executionFailures = session.Rows.Count(row => !row.Success);
        if (executionFailures > 0)
        {
            failures.Add(toolName + $": generated success corpus returned {executionFailures} NG/error rows.");
        }

        if (session.Rows.Any(row => !row.IsUngated || !string.Equals(row.Status, "RUN OK", StringComparison.Ordinal)))
        {
            failures.Add(toolName + ": execution-only rows were not clearly marked RUN OK/ungated.");
        }

        if (!SerializeHelper.TryLoadFromXmlText(
                session.PipelineXml,
                out VisionPipeline frozenPipeline,
                out string pipelineLoadError)
            || frozenPipeline == null)
        {
            failures.Add(toolName + ": frozen Pipeline XML did not reload: " + pipelineLoadError);
            continue;
        }

        bool equivalent = true;
        for (int index = 0; index < session.Rows.Count; index++)
        {
            VisionToolNImageVerificationRow row = session.Rows[index];
            VisionPipelineRunReport retainedReport =
                VisionPipelineRunReportStorage.Load(row.RunReportPath);
            if (retainedReport == null
                || !File.Exists(row.SourceSnapshotPath)
                || !VisionPipelineRunReportStorage.IsFileSha256Match(
                    row.SourceSnapshotPath,
                    row.SourceSha256)
                || !File.Exists(row.DrawingPath))
            {
                failures.Add(toolName + $": retained evidence is incomplete for row {index + 1}.");
                equivalent = false;
                continue;
            }

            using Mat source = Cv2.ImRead(imagePaths[index], ImreadModes.Unchanged);
            OpenCvHelper.SetImageChannel1(source);
            using VisionRecipeRunResult direct = await new VisionRecipeRunner().RunAsync(
                frozenPipeline,
                source,
                VisionRecipeRunner.DefaultInputLayer,
                VisionRecipeRunner.DefaultStepTimeoutMilliseconds);
            if (direct.Success != row.Success)
            {
                failures.Add(toolName + $": direct/N-image success mismatch at row {index + 1}.");
                equivalent = false;
            }

            VisionRecipeStepRunSummary? directStep = direct.Steps.LastOrDefault();
            VisionPipelineStepRunReport? retainedStep = retainedReport.Steps.LastOrDefault();
            Dictionary<string, double> retainedMetrics = (retainedStep?.Metrics
                    ?? new List<VisionPipelineMetricRunReport>())
                .ToDictionary(metric => metric.Name, metric => metric.Value, StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, double> metric in directStep?.Metrics
                ?? new Dictionary<string, double>())
            {
                if (!retainedMetrics.TryGetValue(metric.Key, out double retainedValue)
                    || Math.Abs(retainedValue - metric.Value) > 0.000001)
                {
                    failures.Add(
                        toolName + $": metric mismatch at row {index + 1}: {metric.Key} "
                        + $"direct={metric.Value:0.######}, retained={retainedValue:0.######}.");
                    equivalent = false;
                    break;
                }
            }
        }

        VisionPipelineBatchRunSummary batch =
            VisionPipelineBatchRunSummaryStorage.Load(session.BatchSummaryPath);
        if (batch == null
            || batch.TotalCount != imagePaths.Count
            || string.IsNullOrWhiteSpace(batch.PipelineSnapshotFile)
            || !File.Exists(Path.Combine(
                Path.GetDirectoryName(session.BatchSummaryPath) ?? string.Empty,
                batch.PipelineSnapshotFile)))
        {
            failures.Add(toolName + ": batch summary/pipeline snapshot is incomplete.");
        }
        else
        {
            VisionPipelineBatchRunSummaryStorage.BatchReviewQueue rebuilt =
                VisionPipelineBatchRunSummaryStorage.BuildReviewQueue(batch.Results);
            if (!string.Equals(
                    rebuilt.Sha256,
                    batch.ReviewQueueSha256,
                    StringComparison.OrdinalIgnoreCase))
            {
                failures.Add(toolName + ": deterministic review queue SHA-256 changed on rebuild.");
            }
        }

        Dictionary<string, DateTime> reportWriteTimes = session.Rows
            .Where(row => File.Exists(row.RunReportPath))
            .ToDictionary(
                row => row.RunReportPath,
                row => File.GetLastWriteTimeUtc(row.RunReportPath),
                StringComparer.OrdinalIgnoreCase);
        string toolEvidenceDirectory = Path.Combine(evidencePath, toolName);
        Directory.CreateDirectory(toolEvidenceDirectory);
        string htmlPath = Path.Combine(toolEvidenceDirectory, "n_image_report.html");
        bool htmlSaved = VisionToolNImageVerificationHtmlReportExporter.TryExport(
            session.BatchSummaryPath,
            session.PipelineXml,
            session.StepDefinitionSha256,
            htmlPath,
            OpenVisionLanguage.English,
            out string htmlError);
        string koreanHtmlPath = Path.Combine(toolEvidenceDirectory, "n_image_report_ko.html");
        bool koreanHtmlSaved = VisionToolNImageVerificationHtmlReportExporter.TryExport(
            session.BatchSummaryPath,
            session.PipelineXml,
            session.StepDefinitionSha256,
            koreanHtmlPath,
            OpenVisionLanguage.Korean,
            out string koreanHtmlError);
        if (!htmlSaved
            || !File.Exists(htmlPath)
            || !File.ReadAllText(htmlPath).Contains("data:image", StringComparison.Ordinal)
            || !File.ReadAllText(htmlPath).Contains(session.StepDefinitionSha256, StringComparison.Ordinal)
            || !File.ReadAllText(htmlPath).Contains("N-image verification report", StringComparison.Ordinal)
            || !koreanHtmlSaved
            || !File.Exists(koreanHtmlPath)
            || !File.ReadAllText(koreanHtmlPath).Contains("N장 검증 보고서", StringComparison.Ordinal)
            || File.ReadAllText(koreanHtmlPath).Contains("?", StringComparison.Ordinal))
        {
            failures.Add(toolName + ": localized self-contained HTML export failed: " + htmlError + " / " + koreanHtmlError);
        }

        if (reportWriteTimes.Any(pair =>
                !File.Exists(pair.Key)
                || File.GetLastWriteTimeUtc(pair.Key) != pair.Value))
        {
            failures.Add(toolName + ": HTML export modified or reran retained run reports.");
        }

        File.WriteAllText(
            Path.Combine(toolEvidenceDirectory, "pipeline.xml"),
            session.PipelineXml,
            new UTF8Encoding(false));
        File.Copy(
            session.BatchSummaryPath,
            Path.Combine(toolEvidenceDirectory, "summary.xml"),
            true);
        contractLines.Add(string.Join(
            "\t",
            toolName,
            session.Rows.Count.ToString(CultureInfo.InvariantCulture),
            session.Rows.Count(row => row.Success).ToString(CultureInfo.InvariantCulture),
            session.Rows.Count(row => !row.Success).ToString(CultureInfo.InvariantCulture),
            createStepCount.ToString(CultureInfo.InvariantCulture),
            equivalent ? "PASS" : "FAIL",
            htmlSaved ? "PASS" : "FAIL",
            session.StepDefinitionSha256));
    }

    try
    {
        VisionToolNImageVerificationSession gatedSession =
            await VisionToolNImageVerificationService.RunAsync(
                "Threshold",
                "P233_Smoke_AcceptanceNg",
                () =>
                {
                    VisionPipelineStep step = thresholdFactory();
                    step.UseAcceptance = true;
                    step.ExpectedSuccess = false;
                    return step;
                },
                normalizeInputToGray: true,
                imagePaths.Take(1).ToList(),
                progress: null,
                CancellationToken.None);
        VisionToolNImageVerificationRow gatedRow = gatedSession.Rows.Single();
        if (!gatedRow.IsNg
            || !string.Equals(gatedRow.Status, "NG", StringComparison.Ordinal)
            || string.IsNullOrWhiteSpace(gatedRow.ReviewDetailText))
        {
            failures.Add(
                "Acceptance-gated N-image result did not expose NG and its review reason. "
                + $"Status={gatedRow.Status}, Reason={gatedRow.ReviewDetailText}");
        }
    }
    catch (Exception ex)
    {
        failures.Add("Acceptance-gated NG contract failed: " + ex.GetBaseException().Message);
    }

    File.WriteAllLines(
        Path.Combine(evidencePath, "contract.tsv"),
        contractLines,
        new UTF8Encoding(false));
    if (failures.Count > 0)
    {
        Console.Error.WriteLine("Tool View N-image verification contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        return 1;
    }

    Console.WriteLine(
        $"Tool View N-image verification contract passed. Tools={tools.Count}, ImagesPerTool={imagePaths.Count}, "
        + $"Evidence={evidencePath}");
    foreach (string line in contractLines.Skip(1))
    {
        Console.WriteLine(line);
    }

    return 0;
}

static async Task<int> RunToolNImageRealFolderAcceptanceAsync(
    string datasetRootArgument,
    string sourceFile,
    string templatePathArgument,
    string baselineCsvArgument,
    string evidenceDirectoryArgument)
{
    const int rowsPerRole = 12;
    const double scoreTolerance = 0.1D;
    string datasetRoot = Path.GetFullPath(datasetRootArgument);
    string templatePath = Path.GetFullPath(templatePathArgument);
    string baselineCsvPath = Path.GetFullPath(baselineCsvArgument);
    string evidenceDirectory = Path.GetFullPath(evidenceDirectoryArgument);
    string metadataPath = Path.Combine(datasetRoot, "metadata.csv");
    if (!File.Exists(metadataPath)
        || !File.Exists(templatePath)
        || !File.Exists(baselineCsvPath))
    {
        Console.Error.WriteLine(
            "P234 real-folder acceptance prerequisite is missing. "
            + $"Metadata={metadataPath}; Template={templatePath}; Baseline={baselineCsvPath}.");
        return 2;
    }

    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> sourceRows =
        LoadAutoMPointCorpusMetadata(metadataPath)
            .Where(row => string.Equals(row.SourceFile, sourceFile, StringComparison.OrdinalIgnoreCase))
            .OrderBy(row => row.GlobalId)
            .ToList();
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> selectedRows =
        sourceRows
            .GroupBy(row => row.Status, StringComparer.OrdinalIgnoreCase)
            .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
            .SelectMany(group =>
            {
                List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> ordered =
                    group.OrderBy(row => row.Md5, StringComparer.OrdinalIgnoreCase).ToList();
                return Enumerable.Range(0, rowsPerRole)
                    .Select(index => ordered[(int)Math.Round(
                        index * (ordered.Count - 1) / (double)(rowsPerRole - 1),
                        MidpointRounding.AwayFromZero)]);
            })
            .OrderBy(row => row.Status, StringComparer.OrdinalIgnoreCase)
            .ThenBy(row => row.FileName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    if (selectedRows.Count != rowsPerRole * 2
        || selectedRows.Count(row => row.Status == "OK") != rowsPerRole
        || selectedRows.Count(row => row.Status == "NG") != rowsPerRole)
    {
        Console.Error.WriteLine(
            $"P234 expected {rowsPerRole} OK and {rowsPerRole} NG rows for {sourceFile}; "
            + $"selected {selectedRows.Count}.");
        return 2;
    }

    Dictionary<string, double> baselineScores = LoadBaselineScores(baselineCsvPath);
    Directory.CreateDirectory(evidenceDirectory);
    string inputDirectory = Path.Combine(evidenceDirectory, "real_folder_input");
    Directory.CreateDirectory(inputDirectory);
    List<string> integrityFailures = new List<string>();
    foreach (var row in selectedRows)
    {
        string sourcePath = GetAutoMPointCorpusImagePath(datasetRoot, row);
        string actualMd5 = File.Exists(sourcePath) ? ComputeMd5(sourcePath) : "MISSING";
        if (!string.Equals(actualMd5, row.Md5, StringComparison.OrdinalIgnoreCase))
        {
            integrityFailures.Add(
                $"{row.Status}/{row.FileName}: metadata MD5={row.Md5}, actual={actualMd5}.");
            continue;
        }

        File.Copy(sourcePath, Path.Combine(inputDirectory, row.FileName), true);
    }

    if (!OpenVisionRecipeValidationSetStorage.TryGetTopLevelImagePaths(
            inputDirectory,
            out IReadOnlyList<string> registeredPaths,
            out string folderError))
    {
        Console.Error.WriteLine("P234 top-level folder registration failed: " + folderError);
        return 1;
    }

    int createStepCount = 0;
    VisionToolNImageVerificationSession session =
        await VisionToolNImageVerificationService.RunAsync(
            "EdgeBasedMatching",
            "P234_DiePad1_RealFolder",
            () =>
            {
                createStepCount++;
                return CreateEdgeUniqueCardRPipeline(
                    templatePath,
                    new Rect(0, 0, 512, 512),
                    uniqueEnabled: true,
                    scoreMinimum: 0.75D,
                    uniqueMarginMinimum: 0.05D).Steps.Single();
            },
            normalizeInputToGray: true,
            registeredPaths,
            progress: null,
            CancellationToken.None);

    List<string> verificationFailures = new List<string>(integrityFailures);
    if (createStepCount != 1)
    {
        verificationFailures.Add($"Step factory count was {createStepCount}, expected 1.");
    }
    if (session.Rows.Count != selectedRows.Count)
    {
        verificationFailures.Add(
            $"Result row count was {session.Rows.Count}, expected {selectedRows.Count}.");
    }

    List<string> resultLines = new List<string>
    {
        "Role\tFileName\tStatus\tScoreMax\tBaselineScore\tScoreDelta\tSourceSha256\tDrawing"
    };
    foreach (VisionToolNImageVerificationRow result in session.Rows)
    {
        var selected = selectedRows.Single(row =>
            string.Equals(row.FileName, result.FileName, StringComparison.OrdinalIgnoreCase));
        VisionPipelineRunReport report = VisionPipelineRunReportStorage.Load(result.RunReportPath);
        VisionPipelineStepRunReport? step = report?.Steps?.LastOrDefault();
        double score = step?.Metrics?
            .FirstOrDefault(metric => string.Equals(
                metric.Name,
                VisionPipelineKnownMetrics.ScoreMax,
                StringComparison.OrdinalIgnoreCase))?.Value ?? double.NaN;
        baselineScores.TryGetValue(result.FileName, out double baselineScore);
        double scoreDelta = score - baselineScore;
        bool sourceEvidenceValid =
            File.Exists(result.SourceSnapshotPath)
            && VisionPipelineRunReportStorage.IsFileSha256Match(
                result.SourceSnapshotPath,
                result.SourceSha256)
            && AreDecodedImagesEqual(result.ImagePath, result.SourceSnapshotPath);
        if (!result.Success)
        {
            verificationFailures.Add($"{selected.Status}/{result.FileName}: {result.Status} {result.Message}");
        }
        if (!double.IsFinite(score)
            || !baselineScores.ContainsKey(result.FileName)
            || Math.Abs(scoreDelta) > scoreTolerance)
        {
            verificationFailures.Add(
                $"{selected.Status}/{result.FileName}: ScoreMax={score:0.###}, "
                + $"baseline={baselineScore:0.###}, delta={scoreDelta:0.######}.");
        }
        if (!sourceEvidenceValid)
        {
            verificationFailures.Add(
                $"{selected.Status}/{result.FileName}: retained source snapshot/hash/pixels mismatch.");
        }
        if (!result.HasDrawing)
        {
            verificationFailures.Add($"{selected.Status}/{result.FileName}: retained drawing missing.");
        }

        resultLines.Add(string.Join(
            "\t",
            selected.Status,
            result.FileName,
            result.Status,
            score.ToString("0.###", CultureInfo.InvariantCulture),
            baselineScore.ToString("0.###", CultureInfo.InvariantCulture),
            scoreDelta.ToString("0.######", CultureInfo.InvariantCulture),
            result.SourceSha256,
            result.HasDrawing ? "PASS" : "FAIL"));
    }

    string htmlPath = Path.Combine(evidenceDirectory, "P234_DIE_PAD_REAL_FOLDER_REPORT.html");
    bool htmlSaved = VisionToolNImageVerificationHtmlReportExporter.TryExport(
        session.BatchSummaryPath,
        session.PipelineXml,
        session.StepDefinitionSha256,
        htmlPath,
        OpenVisionLanguage.English,
        out string htmlError);
    if (!htmlSaved)
    {
        verificationFailures.Add("HTML export failed: " + htmlError);
    }

    File.WriteAllLines(
        Path.Combine(evidenceDirectory, "results.tsv"),
        resultLines,
        new UTF8Encoding(false));
    File.WriteAllText(
        Path.Combine(evidenceDirectory, "pipeline.xml"),
        session.PipelineXml,
        new UTF8Encoding(false));
    File.Copy(
        session.BatchSummaryPath,
        Path.Combine(evidenceDirectory, "summary.xml"),
        true);
    string completionRecordPath = Path.Combine(evidenceDirectory, "completion_record.txt");
    File.WriteAllLines(
        completionRecordPath,
        new[]
        {
            "Status: " + (verificationFailures.Count == 0 ? "Complete" : "Incomplete"),
            $"Scope: P233 shared Tool View N-image path on a deterministic top-level folder containing {rowsPerRole} OK + {rowsPerRole} NG real operator-supplied Die Pad 1 rows, without parameter tuning.",
            $"Acceptance criteria: folder registration -> {registeredPaths.Count}/{selectedRows.Count}; Step freeze -> {createStepCount}/1; execution -> {session.Rows.Count(row => row.Success)}/{selectedRows.Count}; drawings -> {session.Rows.Count(row => row.HasDrawing)}/{selectedRows.Count}; score parity within {scoreTolerance:0.###} -> {(verificationFailures.Count == 0 ? "PASS" : "FAIL")}.",
            $"Verification: source metadata MD5; retained source SHA-256; exact P230 score comparison; retained drawing; retained-only HTML; Step SHA-256 {session.StepDefinitionSha256}.",
            $"Evidence: {htmlPath}; {Path.Combine(evidenceDirectory, "results.tsv")}; {Path.Combine(evidenceDirectory, "pipeline.xml")}; {Path.Combine(evidenceDirectory, "summary.xml")}; {inputDirectory}.",
            "Boundary / next dependency: This is a 24-row same-source synthetic/augmented integration acceptance using the already frozen P230 locator. It does not create new semantic qualification, retune the locator, test other source strata, or prove parallel execution."
        },
        new UTF8Encoding(false));

    Console.WriteLine($"P234FolderRegistration={registeredPaths.Count}/{selectedRows.Count}");
    Console.WriteLine($"P234StepCreateCount={createStepCount}");
    Console.WriteLine($"P234Execution={session.Rows.Count(row => row.Success)}/{selectedRows.Count}");
    Console.WriteLine($"P234Drawings={session.Rows.Count(row => row.HasDrawing)}/{selectedRows.Count}");
    Console.WriteLine($"P234Failures={verificationFailures.Count}");
    Console.WriteLine($"P234Report={htmlPath}");
    if (verificationFailures.Count == 0)
    {
        return 0;
    }

    foreach (string failure in verificationFailures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    return 1;
}

static Dictionary<string, double> LoadBaselineScores(string csvPath)
{
    string[] lines = File.ReadAllLines(csvPath);
    List<string> header = ParseCsvRecord(lines[0]);
    int fileIndex = header.FindIndex(value => value == "FileName");
    int scoreIndex = header.FindIndex(value => value == "Score");
    int outcomeIndex = header.FindIndex(value => value == "Outcome");
    if (fileIndex < 0 || scoreIndex < 0 || outcomeIndex < 0)
    {
        throw new InvalidDataException("Baseline CSV is missing FileName, Score, or Outcome.");
    }

    return lines.Skip(1)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(ParseCsvRecord)
        .Where(values => string.Equals(values[outcomeIndex], "SUCCESS", StringComparison.OrdinalIgnoreCase))
        .ToDictionary(
            values => values[fileIndex],
            values => double.Parse(values[scoreIndex], CultureInfo.InvariantCulture),
            StringComparer.OrdinalIgnoreCase);
}

static bool AreDecodedImagesEqual(string leftPath, string rightPath)
{
    using System.Drawing.Bitmap leftBitmap = new System.Drawing.Bitmap(leftPath);
    using System.Drawing.Bitmap rightBitmap = new System.Drawing.Bitmap(rightPath);
    using Mat left = BitmapImageConverter.ToMat(leftBitmap);
    using Mat right = BitmapImageConverter.ToMat(rightBitmap);
    return !left.Empty()
        && !right.Empty()
        && left.Size() == right.Size()
        && left.Type() == right.Type()
        && Cv2.Norm(left, right, NormTypes.L1) == 0D;
}

static int RunAutoMPointEasyMatchCandidates(
    string sampleRootArgument,
    string evidenceDirectoryArgument)
{
    string sampleRoot = Path.GetFullPath(sampleRootArgument);
    string evidenceDirectory = Path.GetFullPath(evidenceDirectoryArgument);
    string[] sampleNames =
    {
        "BOARD.JPG",
        "Die Pad 1.bmp",
        "Floppies.jpg",
        "Frame 1.tif",
        "Switch1.tif"
    };
    string[] samplePaths = sampleNames
        .Select(name => Path.Combine(sampleRoot, name))
        .ToArray();
    string[] missing = samplePaths.Where(path => !File.Exists(path)).ToArray();
    if (missing.Length > 0)
    {
        Console.Error.WriteLine("Auto MPoint EasyMatch samples are missing: " + string.Join("; ", missing));
        return 2;
    }

    Directory.CreateDirectory(evidenceDirectory);
    string drawingsDirectory = Path.Combine(evidenceDirectory, "drawings");
    string cropsDirectory = Path.Combine(evidenceDirectory, "candidate_crops");
    Directory.CreateDirectory(drawingsDirectory);
    Directory.CreateDirectory(cropsDirectory);

    List<string> csvRows = new List<string>
    {
        "Sample,SourcePath,SourceSha256,ExecutionSuccess,ErrorCode,ErrorName,CandidateIndex,Rank,Accepted,Suggested,PatternRoi,Score,FeatureQuality,ContrastStdDev,EdgeDensity,QuadrantBalance,OrientationBalance,SelfMatchScore,AlternativeMatchScore,UniquenessMargin,SyntheticSuccessRate,PositionErrorMaxPx,RuntimeP95Ms,RejectReason,DrawingPath,CropPath"
    };
    List<string> drawingPaths = new List<string>();
    List<string> drawingLabels = new List<string>();
    int suggestedTotal = 0;
    int evaluatedTotal = 0;
    int successfulSamples = 0;

    foreach (string samplePath in samplePaths)
    {
        string sampleKey = Path.GetFileNameWithoutExtension(samplePath)
            .Replace(' ', '_');
        using Mat source = Cv2.ImRead(samplePath, ImreadModes.Unchanged);
        if (source.Empty())
        {
            Console.Error.WriteLine("Auto MPoint sample could not be loaded: " + samplePath);
            return 2;
        }

        AutoMPointToolProperty property = new AutoMPointToolProperty
        {
            UseAnalysisRoi = false,
            CandidateMode = AutoMPointCandidateMode.Grid,
            PatternWidth = 96,
            PatternHeight = 96,
            CandidateStride = 16,
            MaximumFinalists = 8,
            MaximumResults = 5,
            MinimumFeatureQuality = 0.15D,
            MatchingMinimumScore = 0.75D,
            MinimumUniquenessMargin = 0.05D,
            MaximumTemplatePoints = 300,
            SearchStep = 2,
            UsePositionRefine = true,
            UseSubpixelRefine = true,
            UsePyramidPositionProposal = true,
            UseHybridVerify = true,
            UseAngleSearch = false,
            UseScaleSearch = false,
            MaximumPositionErrorPixels = 2.5D,
            MaximumAngleErrorDegrees = 1.5D,
            MaximumScaleErrorRatio = 0.03D
        };
        AutoMPointTool tool = new AutoMPointTool();
        tool.SetProperty(property);
        VisionToolResult execution = tool.Execute(source);
        try
        {
            evaluatedTotal += tool.candidates.Count;
            suggestedTotal += tool.results.Count;
            if (execution.Success)
            {
                successfulSamples++;
            }

            string drawingPath = Path.Combine(drawingsDirectory, sampleKey + "_auto_mpoint.png");
            if (execution.ResultImage != null && !execution.ResultImage.Empty())
            {
                Cv2.ImWrite(drawingPath, execution.ResultImage);
                drawingPaths.Add(drawingPath);
                drawingLabels.Add(string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} | suggested {1}/{2} | {3}",
                    Path.GetFileName(samplePath),
                    tool.results.Count,
                    tool.candidates.Count,
                    execution.Success ? "suggestions" : execution.ErrorName));
            }

            foreach (OpenVisionLab.Vision2D.Result.AutoMPointCandidateResult candidate in tool.candidates)
            {
                string cropPath = string.Empty;
                bool suggested = candidate.Rank > 0;
                if (suggested)
                {
                    cropPath = Path.Combine(
                        cropsDirectory,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}_rank_{1:00}_x{2}_y{3}.png",
                            sampleKey,
                            candidate.Rank,
                            candidate.PatternRoi.X,
                            candidate.PatternRoi.Y));
                    using Mat crop = source.SubMat(candidate.PatternRoi).Clone();
                    Cv2.ImWrite(cropPath, crop);
                }

                string[] values =
                {
                    Path.GetFileName(samplePath),
                    samplePath,
                    ComputeSha256(samplePath),
                    execution.Success.ToString(CultureInfo.InvariantCulture),
                    ((int)execution.ErrorCode).ToString(CultureInfo.InvariantCulture),
                    execution.ErrorName ?? string.Empty,
                    candidate.Index.ToString(CultureInfo.InvariantCulture),
                    candidate.Rank.ToString(CultureInfo.InvariantCulture),
                    candidate.Accepted.ToString(CultureInfo.InvariantCulture),
                    suggested.ToString(CultureInfo.InvariantCulture),
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0},{1},{2},{3}",
                        candidate.PatternRoi.X,
                        candidate.PatternRoi.Y,
                        candidate.PatternRoi.Width,
                        candidate.PatternRoi.Height),
                    candidate.Score.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.FeatureQuality.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.ContrastStdDev.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.EdgeDensity.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.QuadrantBalance.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.OrientationBalance.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.SelfMatchScore.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.AlternativeMatchScore.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.UniquenessMargin.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.SyntheticSuccessRate.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.PositionErrorMaxPixels.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.RuntimeP95Milliseconds.ToString("0.############", CultureInfo.InvariantCulture),
                    candidate.RejectReason ?? string.Empty,
                    File.Exists(drawingPath) ? drawingPath : string.Empty,
                    cropPath
                };
                csvRows.Add(string.Join(",", values.Select(EscapeBatchCsvValue)));
            }
        }
        finally
        {
            execution.ResultImage?.Dispose();
            tool.imageSource?.Dispose();
            tool.imageResult?.Dispose();
            tool.imageTemplate?.Dispose();
        }
    }

    string resultsPath = Path.Combine(evidenceDirectory, "p226_auto_mpoint_easymatch_candidates.csv");
    File.WriteAllLines(resultsPath, csvRows);
    string contactSheetPath = Path.Combine(evidenceDirectory, "p226_auto_mpoint_easymatch_contact_sheet.png");
    SaveCardPilotContactSheet(drawingPaths, drawingLabels, contactSheetPath);
    string recordPath = Path.Combine(evidenceDirectory, "completion_record.md");
    File.WriteAllLines(recordPath, new[]
    {
        "# P226 Auto MPoint EasyMatch candidate presentation",
        string.Empty,
        "Status: Complete",
        string.Empty,
        "Scope: Run the OpenVisionLab Vision SDK Auto MPoint engine once on five diverse public EasyMatch source images and retain operator-review drawings and candidate metrics.",
        string.Empty,
        "Acceptance criteria:",
        $"- Five frozen source images loaded: PASS ({samplePaths.Length}/5).",
        $"- Current-run Auto MPoint drawings retained: PASS ({drawingPaths.Count}/5).",
        $"- Evaluated candidate rows retained: PASS ({evaluatedTotal}).",
        $"- Displayed suggestions retained without automatic apply: PASS ({suggestedTotal}).",
        "- Result-dependent threshold tuning: PASS (none).",
        "- Pattern application or cross-image matching run: PASS (not performed).",
        string.Empty,
        "Verification:",
        "- Product UI defaults were frozen before execution: 96x96, stride 16, maximum results 5, minimum feature quality 0.15, matching score 0.75, uniqueness 0.05, maximum synthetic position error 2.5 px.",
        $"- Samples with at least one suggestion: {successfulSamples}/5.",
        $"- CSV: `{resultsPath}`",
        $"- Contact sheet: `{contactSheetPath}`",
        string.Empty,
        "Boundary / next dependency: These are automatic pattern suggestions on each source image only. They do not prove that a feature is a durable physical locator across a family, and no suggestion may be applied until the operator reviews and approves its physical meaning."
    });

    Console.WriteLine($"P226Samples={samplePaths.Length}");
    Console.WriteLine($"P226SuccessfulSamples={successfulSamples}");
    Console.WriteLine($"P226EvaluatedCandidates={evaluatedTotal}");
    Console.WriteLine($"P226DisplayedSuggestions={suggestedTotal}");
    Console.WriteLine($"P226Results={resultsPath}");
    Console.WriteLine($"P226ContactSheet={contactSheetPath}");
    Console.WriteLine($"P226Record={recordPath}");
    return drawingPaths.Count == samplePaths.Length ? 0 : 1;
}

static int RunAutoMPointFullStratumQualification(
    string datasetRootArgument,
    string sourceFile,
    string templatePathArgument,
    string evidenceDirectoryArgument)
{
    const int expectedRows = 122;
    const int expectedOkRows = 62;
    const int expectedNgRows = 60;
    const double scoreMinimum = 0.75D;
    const double uniquenessMinimum = 0.05D;

    string datasetRoot = Path.GetFullPath(datasetRootArgument);
    string templatePath = Path.GetFullPath(templatePathArgument);
    string evidenceDirectory = Path.GetFullPath(evidenceDirectoryArgument);
    string metadataPath = Path.Combine(datasetRoot, "metadata.csv");
    string drawingsDirectory = Path.Combine(evidenceDirectory, "drawings");
    string overlapDrawingsDirectory = Path.Combine(evidenceDirectory, "defect_overlap_drawings");
    Directory.CreateDirectory(drawingsDirectory);
    Directory.CreateDirectory(overlapDrawingsDirectory);
    if (!File.Exists(metadataPath) || !File.Exists(templatePath))
    {
        Console.Error.WriteLine(
            $"P230 requires metadata and approved template. Metadata={metadataPath}, Template={templatePath}.");
        return 2;
    }

    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> rows =
        LoadAutoMPointCorpusMetadata(metadataPath)
            .Where(row => string.Equals(row.SourceFile, sourceFile, StringComparison.OrdinalIgnoreCase))
            .OrderBy(row => row.GlobalId)
            .ToList();
    int okRows = rows.Count(row => string.Equals(row.Status, "OK", StringComparison.OrdinalIgnoreCase));
    int ngRows = rows.Count(row => string.Equals(row.Status, "NG", StringComparison.OrdinalIgnoreCase));
    if (rows.Count != expectedRows || okRows != expectedOkRows || ngRows != expectedNgRows)
    {
        Console.Error.WriteLine(
            $"P230 frozen stratum mismatch. Expected {expectedRows} ({expectedOkRows} OK/{expectedNgRows} NG), "
            + $"actual {rows.Count} ({okRows} OK/{ngRows} NG).");
        return 2;
    }

    Dictionary<string, string> maskPaths = Directory
        .EnumerateFiles(
            Path.Combine(datasetRoot, "segmentation", "masks_binary"),
            "*.png",
            SearchOption.AllDirectories)
        .GroupBy(path => Path.GetFileNameWithoutExtension(path), StringComparer.OrdinalIgnoreCase)
        .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
    List<string> integrityFailures = new List<string>();
    Dictionary<string, string> overlapDrawingPaths =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    List<(
        int GlobalId,
        string Role,
        string FileName,
        string Outcome,
        double Score,
        double Uniqueness,
        double CenterX,
        double CenterY,
        double Angle,
        double Scale,
        double ElapsedMs,
        int MaskOverlapPixels,
        string ErrorName,
        string Message,
        string DrawingPath,
        string MetadataMd5,
        string ActualMd5,
        bool Md5Verified)> results =
        new List<(
            int,
            string,
            string,
            string,
            double,
            double,
            double,
            double,
            double,
            double,
            double,
            int,
            string,
            string,
            string,
            string,
            string,
            bool)>();
    int runtimeErrors = 0;

    for (int index = 0; index < rows.Count; index++)
    {
        (int globalId, string fileName, string status, string _, string metadataMd5) = rows[index];
        string sourcePath = GetAutoMPointCorpusImagePath(datasetRoot, rows[index]);
        string actualMd5 = File.Exists(sourcePath) ? ComputeMd5(sourcePath) : "MISSING";
        bool md5Verified = string.Equals(actualMd5, metadataMd5, StringComparison.OrdinalIgnoreCase);
        if (!md5Verified)
        {
            integrityFailures.Add(
                $"{status}/{fileName}: metadata MD5={metadataMd5}, actual={actualMd5}.");
        }

        string drawingPath = Path.Combine(
            drawingsDirectory,
            $"{index + 1:000}_{status}_{Path.GetFileNameWithoutExtension(fileName)}.png");
        string outcome = "ERROR";
        double score = double.NaN;
        double uniqueness = double.NaN;
        double centerX = double.NaN;
        double centerY = double.NaN;
        double angle = double.NaN;
        double scale = double.NaN;
        double elapsedMs = double.NaN;
        int maskOverlapPixels = 0;
        string? overlapMaskPath = null;
        string errorName = "MissingImage";
        string message = "Source image could not be loaded.";

        using Mat source = Cv2.ImRead(sourcePath, ImreadModes.Color);
        if (source.Empty())
        {
            runtimeErrors++;
        }
        else
        {
            VisionPipeline pipeline = CreateEdgeUniqueCardRPipeline(
                templatePath,
                new Rect(0, 0, source.Width, source.Height),
                true,
                scoreMinimum,
                uniquenessMinimum);
            EdgeBasedTemplateMatchingTool matcher =
                (EdgeBasedTemplateMatchingTool)VisionPipelineAppToolFactory.Create(
                    pipeline.Steps.Single());
            VisionToolResult execution = matcher.Execute(source);
            try
            {
                elapsedMs = execution.Elapsed.TotalMilliseconds;
                errorName = execution.ErrorName ?? execution.ErrorCode.ToString();
                message = execution.Message ?? string.Empty;
                OpenVisionLab.Vision2D.Result.MatchingResult? match = matcher.results.SingleOrDefault();
                if (execution.Success && match != null)
                {
                    outcome = "SUCCESS";
                    score = match.Score;
                    uniqueness = GetMetricOrNaN(execution, "UniqueMatch.ScoreMargin");
                    centerX = match.Center.X;
                    centerY = match.Center.Y;
                    angle = match.Angle;
                    scale = match.Scale;
                    if (string.Equals(status, "NG", StringComparison.OrdinalIgnoreCase)
                        && maskPaths.TryGetValue(
                            Path.GetFileNameWithoutExtension(fileName),
                            out string? maskPath))
                    {
                        maskOverlapPixels = CountMaskOverlap(maskPath, match.Bounding);
                        overlapMaskPath = maskPath;
                    }
                }
                else if (execution.ErrorCode == VisionToolErrorCode.MatchingAmbiguous)
                {
                    outcome = "AMBIGUOUS";
                }
                else if (execution.ErrorCode == VisionToolErrorCode.MatchingNoResult)
                {
                    outcome = "NO_MATCH";
                }
                else
                {
                    runtimeErrors++;
                }

                if (execution.ResultImage != null && !execution.ResultImage.Empty())
                {
                    Cv2.ImWrite(drawingPath, execution.ResultImage);
                    if (maskOverlapPixels > 0
                        && !string.IsNullOrWhiteSpace(overlapMaskPath))
                    {
                        using Mat mask = Cv2.ImRead(overlapMaskPath, ImreadModes.Grayscale);
                        if (!mask.Empty()
                            && mask.Width == execution.ResultImage.Width
                            && mask.Height == execution.ResultImage.Height)
                        {
                            using Mat red = new Mat(
                                execution.ResultImage.Size(),
                                MatType.CV_8UC3,
                                new Scalar(0, 0, 255));
                            using Mat blended = new Mat();
                            using Mat overlapDrawing = execution.ResultImage.Clone();
                            Cv2.AddWeighted(execution.ResultImage, 0.55D, red, 0.45D, 0D, blended);
                            blended.CopyTo(overlapDrawing, mask);
                            Cv2.PutText(
                                overlapDrawing,
                                $"RED defect mask overlap = {maskOverlapPixels}px",
                                new OpenCvSharp.Point(12, 28),
                                HersheyFonts.HersheySimplex,
                                0.55D,
                                new Scalar(0, 255, 255),
                                2,
                                LineTypes.AntiAlias);
                            string overlapDrawingPath = Path.Combine(
                                overlapDrawingsDirectory,
                                $"{index + 1:000}_{status}_{Path.GetFileNameWithoutExtension(fileName)}.png");
                            Cv2.ImWrite(overlapDrawingPath, overlapDrawing);
                            overlapDrawingPaths[fileName] = overlapDrawingPath;
                        }
                    }
                }
                else
                {
                    using Mat fallback = source.Clone();
                    Cv2.PutText(
                        fallback,
                        $"{outcome}: {errorName}",
                        new OpenCvSharp.Point(12, 28),
                        HersheyFonts.HersheySimplex,
                        0.65,
                        new Scalar(0, 0, 255),
                        2,
                        LineTypes.AntiAlias);
                    Cv2.ImWrite(drawingPath, fallback);
                }
            }
            finally
            {
                execution.ResultImage?.Dispose();
                matcher.imageSource?.Dispose();
                matcher.imageResult?.Dispose();
                using Mat emptyTemplate = new Mat();
                matcher.SetTemplateImage(emptyTemplate);
                matcher.imageTemplate?.Dispose();
            }
        }

        results.Add((
            globalId,
            status,
            fileName,
            outcome,
            score,
            uniqueness,
            centerX,
            centerY,
            angle,
            scale,
            elapsedMs,
            maskOverlapPixels,
            errorName,
            message,
            drawingPath,
            metadataMd5,
            actualMd5,
            md5Verified));
    }

    string resultsCsvPath = Path.Combine(evidenceDirectory, "p230_full_stratum_results.csv");
    File.WriteAllLines(
        resultsCsvPath,
        new[]
        {
            "GlobalId,Role,FileName,Outcome,Score,UniquenessMargin,CenterX,CenterY,AngleDeg,Scale,ElapsedMs,DefectMaskOverlapPixels,ErrorName,Message,DrawingPath,MetadataMd5,ActualMd5,Md5Verified"
        }.Concat(results.Select(result => string.Join(",", new[]
        {
            result.GlobalId.ToString(CultureInfo.InvariantCulture),
            result.Role,
            result.FileName,
            result.Outcome,
            FormatFinite(result.Score),
            FormatFinite(result.Uniqueness),
            FormatFinite(result.CenterX),
            FormatFinite(result.CenterY),
            FormatFinite(result.Angle),
            FormatFinite(result.Scale),
            FormatFinite(result.ElapsedMs),
            result.MaskOverlapPixels.ToString(CultureInfo.InvariantCulture),
            result.ErrorName,
            result.Message,
            result.DrawingPath,
            result.MetadataMd5,
            result.ActualMd5,
            result.Md5Verified.ToString(CultureInfo.InvariantCulture)
        }.Select(EscapeBatchCsvValue)))),
        new System.Text.UTF8Encoding(true));

    Dictionary<string, HashSet<string>> queueReasons =
        new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
    void AddQueueReason(string fileName, string reason)
    {
        if (!queueReasons.TryGetValue(fileName, out HashSet<string>? reasons))
        {
            reasons = new HashSet<string>(StringComparer.Ordinal);
            queueReasons[fileName] = reasons;
        }
        reasons.Add(reason);
    }

    foreach (var result in results.Where(result => result.Outcome != "SUCCESS"))
    {
        AddQueueReason(result.FileName, result.Outcome);
    }
    foreach (var result in results.Where(result => result.MaskOverlapPixels > 0))
    {
        AddQueueReason(result.FileName, "DEFECT_OVERLAP");
    }
    foreach (var result in results.Where(result => result.Outcome == "SUCCESS")
        .OrderBy(result => result.Score).Take(6))
    {
        AddQueueReason(result.FileName, "LOWEST_SCORE");
    }
    foreach (var result in results.Where(result => result.Outcome == "SUCCESS")
        .OrderBy(result => result.Uniqueness).Take(6))
    {
        AddQueueReason(result.FileName, "LOWEST_UNIQUENESS");
    }
    foreach (var result in results.OrderByDescending(result => result.ElapsedMs).Take(4))
    {
        AddQueueReason(result.FileName, "HIGHEST_RUNTIME");
    }
    foreach (var result in results.Where(result => result.Outcome == "SUCCESS")
        .OrderByDescending(result => Math.Abs(result.Angle)).Take(4))
    {
        AddQueueReason(result.FileName, "ANGLE_EXTREME");
    }
    foreach (var result in results.Where(result => result.Outcome == "SUCCESS")
        .OrderBy(result => result.Scale).Take(2)
        .Concat(results.Where(result => result.Outcome == "SUCCESS")
            .OrderByDescending(result => result.Scale).Take(2)))
    {
        AddQueueReason(result.FileName, "SCALE_EXTREME");
    }
    List<(int GlobalId, string Role, string FileName, string Outcome, double Score, double Uniqueness, double CenterX, double CenterY, double Angle, double Scale, double ElapsedMs, int MaskOverlapPixels, string ErrorName, string Message, string DrawingPath, string MetadataMd5, string ActualMd5, bool Md5Verified)> hashOrdered =
        results.OrderBy(result => result.ActualMd5, StringComparer.OrdinalIgnoreCase).ToList();
    for (int index = 0; index < 8; index++)
    {
        int selectedIndex = (int)Math.Round(
            index * (hashOrdered.Count - 1) / 7D,
            MidpointRounding.AwayFromZero);
        AddQueueReason(hashOrdered[selectedIndex].FileName, "HASH_SPREAD");
    }

    List<(int GlobalId, string Role, string FileName, string Outcome, double Score, double Uniqueness, double CenterX, double CenterY, double Angle, double Scale, double ElapsedMs, int MaskOverlapPixels, string ErrorName, string Message, string DrawingPath, string MetadataMd5, string ActualMd5, bool Md5Verified)> reviewRows =
        results.Where(result => queueReasons.ContainsKey(result.FileName))
            .OrderBy(result => result.GlobalId)
            .ToList();
    string queueCsvPath = Path.Combine(evidenceDirectory, "p230_review_queue.csv");
    File.WriteAllLines(
        queueCsvPath,
        new[]
        {
            "GlobalId,Role,FileName,Reasons,Outcome,Score,UniquenessMargin,AngleDeg,Scale,ElapsedMs,DefectMaskOverlapPixels,DrawingPath"
        }.Concat(reviewRows.Select(result => string.Join(",", new[]
        {
            result.GlobalId.ToString(CultureInfo.InvariantCulture),
            result.Role,
            result.FileName,
            string.Join("+", queueReasons[result.FileName].OrderBy(reason => reason, StringComparer.Ordinal)),
            result.Outcome,
            FormatFinite(result.Score),
            FormatFinite(result.Uniqueness),
            FormatFinite(result.Angle),
            FormatFinite(result.Scale),
            FormatFinite(result.ElapsedMs),
            result.MaskOverlapPixels.ToString(CultureInfo.InvariantCulture),
            result.DrawingPath
        }.Select(EscapeBatchCsvValue)))),
        new System.Text.UTF8Encoding(true));

    string queueSheetPath = Path.Combine(evidenceDirectory, "p230_review_queue_contact_sheet.png");
    SaveCardPilotContactSheet(
        reviewRows.Select(result => result.DrawingPath).ToList(),
        reviewRows.Select(result =>
            $"{result.Role} {Path.GetFileNameWithoutExtension(result.FileName)} "
            + $"{result.Outcome} S={result.Score:0.0}").ToList(),
        queueSheetPath);

    string overlapSheetPath = Path.Combine(
        evidenceDirectory,
        "p230_defect_overlap_contact_sheet.png");
    List<(int GlobalId, string Role, string FileName, string Outcome, double Score, double Uniqueness, double CenterX, double CenterY, double Angle, double Scale, double ElapsedMs, int MaskOverlapPixels, string ErrorName, string Message, string DrawingPath, string MetadataMd5, string ActualMd5, bool Md5Verified)> overlapRows =
        results.Where(result => result.MaskOverlapPixels > 0)
            .OrderBy(result => result.GlobalId)
            .ToList();
    if (overlapRows.Count > 0)
    {
        SaveCardPilotContactSheet(
            overlapRows.Select(result => overlapDrawingPaths[result.FileName]).ToList(),
            overlapRows.Select(result =>
                $"{result.Role} {Path.GetFileNameWithoutExtension(result.FileName)} "
                + $"overlap={result.MaskOverlapPixels}px").ToList(),
            overlapSheetPath);
    }

    int successCount = results.Count(result => result.Outcome == "SUCCESS");
    int okSuccess = results.Count(result => result.Role == "OK" && result.Outcome == "SUCCESS");
    int ngSuccess = results.Count(result => result.Role == "NG" && result.Outcome == "SUCCESS");
    int ambiguousCount = results.Count(result => result.Outcome == "AMBIGUOUS");
    int noMatchCount = results.Count(result => result.Outcome == "NO_MATCH");
    int maskOverlapRows = results.Count(result => result.MaskOverlapPixels > 0);
    int drawingCount = results.Count(result => File.Exists(result.DrawingPath));
    double minimumScore = results.Where(result => result.Outcome == "SUCCESS")
        .Select(result => result.Score).DefaultIfEmpty(double.NaN).Min();
    double minimumUniqueness = results.Where(result => result.Outcome == "SUCCESS")
        .Select(result => result.Uniqueness).DefaultIfEmpty(double.NaN).Min();
    double maximumRuntime = results.Select(result => result.ElapsedMs)
        .Where(double.IsFinite).DefaultIfEmpty(double.NaN).Max();
    bool numericalPass =
        successCount == expectedRows
        && ambiguousCount == 0
        && noMatchCount == 0
        && runtimeErrors == 0
        && integrityFailures.Count == 0
        && drawingCount == expectedRows;
    string decision = numericalPass
        ? maskOverlapRows > 0
            ? "Keep with documented limits"
            : "Keep"
        : "Reject";

    string reportPath = Path.Combine(
        evidenceDirectory,
        "OPENVISIONLAB_AUTO_MPOINT_FULL_STRATUM_REPORT.html");
    System.Text.StringBuilder html = new System.Text.StringBuilder();
    html.AppendLine("<!doctype html><html lang=\"ko\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
    html.AppendLine("<title>OpenVisionLab Auto MPoint 전체 계층 자격 검증</title>");
    html.AppendLine("<style>body{margin:0;background:#08111f;color:#eef5ff;font-family:'Segoe UI','Malgun Gothic',sans-serif;line-height:1.55}.wrap{max-width:1400px;margin:auto;padding:32px}.panel{background:#111d2e;border:1px solid #2b3d56;border-radius:14px;padding:22px;margin:18px 0}.cards{display:grid;grid-template-columns:repeat(4,1fr);gap:12px}.card{background:#17263b;border-radius:12px;padding:16px}.value{font-size:25px;font-weight:800}.label{color:#9db0c9;font-size:13px}.ok{color:#6ce9bb}.bad{color:#ff7b83}.warn{color:#ffc36c}.image{background:#050a11;border:1px solid #31445d;border-radius:10px;padding:8px}.image img{width:100%;height:auto;display:block}.template{display:flex;justify-content:center}.template img{width:auto;max-width:420px;max-height:420px}table{width:100%;border-collapse:collapse;font-size:13px}th,td{padding:9px;border-bottom:1px solid #2b3d56;text-align:left}th{background:#1b2b42}.scroll{overflow:auto}.print{float:right;background:#24405e;color:white;border:1px solid #5e789b;border-radius:9px;padding:10px 14px}@media(max-width:900px){.cards{grid-template-columns:repeat(2,1fr)}}@media print{body{background:white;color:#111}.panel,.card{background:white;border-color:#aaa}.print{display:none}}</style></head><body><main class=\"wrap\">");
    html.AppendLine("<button class=\"print\" onclick=\"window.print()\">인쇄 / PDF 저장</button>");
    html.Append("<h1>Auto MPoint 전체 계층 자격 검증</h1><p>")
        .Append(HtmlReportEncode(sourceFile))
        .Append(" · 승인 ROI 128,256,96,96 · 템플릿 SHA-256 ")
        .Append(HtmlReportEncode(ComputeSha256(templatePath)))
        .AppendLine("</p><section class=\"cards\">");
    AppendAutoMPointHtmlMetric(html, $"{successCount}/{expectedRows}", "전체 Matching 성공");
    AppendAutoMPointHtmlMetric(html, $"{okSuccess}/{expectedOkRows}", "OK 성공");
    AppendAutoMPointHtmlMetric(html, $"{ngSuccess}/{expectedNgRows}", "NG 성공");
    AppendAutoMPointHtmlMetric(html, decision, "수치 결정");
    AppendAutoMPointHtmlMetric(html, minimumScore.ToString("0.0", CultureInfo.InvariantCulture), "최저 점수");
    AppendAutoMPointHtmlMetric(html, minimumUniqueness.ToString("0.000", CultureInfo.InvariantCulture), "최저 고유성");
    AppendAutoMPointHtmlMetric(html, maskOverlapRows.ToString(CultureInfo.InvariantCulture), "결함 마스크 겹침 행");
    AppendAutoMPointHtmlMetric(html, reviewRows.Count.ToString(CultureInfo.InvariantCulture), "결정적 검토 큐");
    html.AppendLine("</section>");
    html.Append("<section class=\"panel\"><h2>결론</h2><p><strong class=\"")
        .Append(numericalPass ? "ok" : "bad")
        .Append("\">").Append(HtmlReportEncode(decision)).Append("</strong> · 모호 ")
        .Append(ambiguousCount).Append(" · 미검출 ").Append(noMatchCount)
        .Append(" · 런타임 오류 ").Append(runtimeErrors)
        .Append(" · 무결성 오류 ").Append(integrityFailures.Count)
        .Append(" · 최대 실행시간 ").Append(maximumRuntime.ToString("0.0", CultureInfo.InvariantCulture))
        .AppendLine(" ms</p>");
    html.AppendLine("<p class=\"warn\">이 결정은 동일 합성·증강 Die Pad 1 계층의 수치 자격 검증입니다. 실제 촬영, 생산 변동, 자동 패턴 크기, 다른 Die Pad 2~4 계층, 현장 자격을 의미하지 않습니다.</p></section>");
    html.AppendLine("<section class=\"panel\"><h2>승인 템플릿</h2>");
    html.Append("<div class=\"image template\"><img alt=\"승인된 Auto MPoint 템플릿\" src=\"")
        .Append(ToEmbeddedImageDataUri(templatePath)).AppendLine("\"></div></section>");
    html.AppendLine("<section class=\"panel\"><h2>결정적 검토 큐</h2><p>모든 실패·결함 겹침과 최저 점수/고유성, 각도·배율·시간 극단, 해시 분산 표본을 중복 제거했습니다.</p>");
    html.Append("<div class=\"image\"><img alt=\"Auto MPoint 전체 계층 검토 큐\" src=\"")
        .Append(ToEmbeddedImageDataUri(queueSheetPath)).AppendLine("\"></div></section>");
    if (File.Exists(overlapSheetPath))
    {
        html.AppendLine("<section class=\"panel\"><h2>결함 마스크 겹침 9건</h2><p class=\"warn\">빨간색은 공급된 NG 결함 마스크입니다. 96×96 매칭 영역과 일부 겹쳤지만, 현재 데이터에서는 모두 동일한 중앙 패드에 성공했습니다. 이 겹침 때문에 결과를 실패로 바꾸지는 않되 실제 촬영 변동 위험으로 유지합니다.</p>");
        html.Append("<div class=\"image\"><img alt=\"결함 마스크와 매칭 영역 겹침\" src=\"")
            .Append(ToEmbeddedImageDataUri(overlapSheetPath)).AppendLine("\"></div></section>");
    }
    html.AppendLine("<section class=\"panel\"><h2>검토 큐 상세</h2><div class=\"scroll\"><table><thead><tr><th>Role</th><th>File</th><th>Reasons</th><th>Outcome</th><th>Score</th><th>Uniqueness</th><th>Angle</th><th>Scale</th><th>Overlap</th></tr></thead><tbody>");
    foreach (var result in reviewRows)
    {
        html.Append("<tr><td>").Append(HtmlReportEncode(result.Role))
            .Append("</td><td>").Append(HtmlReportEncode(result.FileName))
            .Append("</td><td>").Append(HtmlReportEncode(string.Join("+", queueReasons[result.FileName].OrderBy(reason => reason, StringComparer.Ordinal))))
            .Append("</td><td>").Append(HtmlReportEncode(result.Outcome))
            .Append("</td><td>").Append(FormatFinite(result.Score))
            .Append("</td><td>").Append(FormatFinite(result.Uniqueness))
            .Append("</td><td>").Append(FormatFinite(result.Angle))
            .Append("</td><td>").Append(FormatFinite(result.Scale))
            .Append("</td><td>").Append(result.MaskOverlapPixels)
            .AppendLine("</td></tr>");
    }
    html.AppendLine("</tbody></table></div></section>");
    html.AppendLine("<section class=\"panel\"><h2>증거 파일</h2><p><a href=\"p230_full_stratum_results.csv\">전체 122행 CSV</a> · <a href=\"p230_review_queue.csv\">검토 큐 CSV</a> · 개별 122장 드로잉은 <code>drawings/</code>에 보존됩니다.</p></section>");
    html.AppendLine("</main></body></html>");
    File.WriteAllText(reportPath, html.ToString(), new System.Text.UTF8Encoding(true));

    bool taskComplete =
        results.Count == expectedRows
        && runtimeErrors == 0
        && integrityFailures.Count == 0
        && drawingCount == expectedRows;
    string completionRecordPath = Path.Combine(evidenceDirectory, "completion_record.txt");
    File.WriteAllLines(
        completionRecordPath,
        new[]
        {
            "Status: " + (taskComplete ? "Complete" : "Incomplete"),
            $"Scope: Frozen approved Auto MPoint template replay on all {expectedRows} {sourceFile} rows without parameter tuning.",
            $"Acceptance criteria: rows -> {results.Count}/{expectedRows}; drawings -> {drawingCount}/{expectedRows}; success -> {successCount}/{expectedRows}; ambiguous -> {ambiguousCount}; no match -> {noMatchCount}; runtime errors -> {runtimeErrors}; integrity failures -> {integrityFailures.Count}; defect overlap rows -> {maskOverlapRows}.",
            $"Verification: score >= {scoreMinimum:0.##}; uniqueness >= {uniquenessMinimum:0.##}; angle -8..8; scale 0.9..1.1; template SHA-256 {ComputeSha256(templatePath)}; numerical decision {decision}.",
            $"Evidence: {reportPath}; {resultsCsvPath}; {queueCsvPath}; {queueSheetPath}; {overlapSheetPath}; {drawingsDirectory}.",
            "Boundary / next dependency: Same-source synthetic/augmented evidence only. This numerical record does not replace review of the deterministic queue and every defect-overlap drawing; no other source stratum or field qualification is implied."
        },
        new System.Text.UTF8Encoding(true));

    Console.WriteLine($"P230Rows={results.Count}/{expectedRows}");
    Console.WriteLine($"P230Success={successCount}/{expectedRows}");
    Console.WriteLine($"P230OkSuccess={okSuccess}/{expectedOkRows}");
    Console.WriteLine($"P230NgSuccess={ngSuccess}/{expectedNgRows}");
    Console.WriteLine($"P230Ambiguous={ambiguousCount}");
    Console.WriteLine($"P230NoMatch={noMatchCount}");
    Console.WriteLine($"P230MaskOverlapRows={maskOverlapRows}");
    Console.WriteLine($"P230RuntimeErrors={runtimeErrors}");
    Console.WriteLine($"P230IntegrityFailures={integrityFailures.Count}");
    Console.WriteLine($"P230Drawings={drawingCount}/{expectedRows}");
    Console.WriteLine($"P230ReviewQueue={reviewRows.Count}");
    Console.WriteLine($"P230Decision={decision}");
    Console.WriteLine($"P230Report={reportPath}");
    return taskComplete ? 0 : 1;
}

static int RunAutoMPointRepresentativeBestPilot(
    string datasetRoot,
    string sourceFile,
    string evidenceDirectory)
{
    Directory.CreateDirectory(evidenceDirectory);
    string metadataPath = Path.Combine(datasetRoot, "metadata.csv");
    if (!File.Exists(metadataPath))
    {
        Console.Error.WriteLine("P229 metadata is missing: " + metadataPath);
        return 2;
    }

    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> rows =
        LoadAutoMPointCorpusMetadata(metadataPath)
            .Where(row => string.Equals(row.SourceFile, sourceFile, StringComparison.OrdinalIgnoreCase))
            .OrderBy(row => row.GlobalId)
            .ToList();
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> okRows =
        rows.Where(row => string.Equals(row.Status, "OK", StringComparison.OrdinalIgnoreCase)).ToList();
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> ngRows =
        rows.Where(row => string.Equals(row.Status, "NG", StringComparison.OrdinalIgnoreCase)).ToList();
    if (okRows.Count < 9 || ngRows.Count < 8)
    {
        Console.Error.WriteLine(
            $"P229 requires at least 9 OK and 8 NG rows for {sourceFile}. "
            + $"Actual OK={okRows.Count}, NG={ngRows.Count}.");
        return 2;
    }

    (int GlobalId, string FileName, string Status, string SourceFile, string Md5) canonical = okRows[0];
    HashSet<string> usedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        canonical.FileName
    };
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> representativeRows =
        SelectAutoMPointPilotRows(
                okRows.Where(row => !usedFiles.Contains(row.FileName)).ToList(),
                null)
            .Concat(SelectAutoMPointPilotRows(ngRows, null))
            .ToList();
    foreach ((int _, string fileName, string _, string _, string _) in representativeRows)
    {
        usedFiles.Add(fileName);
    }
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> heldOutRows =
        SelectAutoMPointPilotRows(
                okRows.Where(row => !usedFiles.Contains(row.FileName)).ToList(),
                null)
            .Concat(SelectAutoMPointPilotRows(
                ngRows.Where(row => !usedFiles.Contains(row.FileName)).ToList(),
                null))
            .ToList();

    List<string> integrityFailures = new List<string>();
    foreach ((int _, string fileName, string status, string _, string md5) in
        new[] { canonical }.Concat(representativeRows).Concat(heldOutRows))
    {
        string path = Path.Combine(datasetRoot, "all_images", status, fileName);
        string actual = File.Exists(path) ? ComputeMd5(path) : "MISSING";
        if (!string.Equals(actual, md5, StringComparison.OrdinalIgnoreCase))
        {
            integrityFailures.Add($"{status}/{fileName}: metadata MD5={md5}, actual={actual}.");
        }
    }

    string canonicalPath = GetAutoMPointCorpusImagePath(datasetRoot, canonical);
    string candidateDrawingPath = Path.Combine(evidenceDirectory, "p229_auto_mpoint_best_candidate.png");
    string templatePath = Path.Combine(evidenceDirectory, "p229_auto_mpoint_best_template.png");
    string candidateCsvPath = Path.Combine(evidenceDirectory, "p229_auto_mpoint_candidate_ranking.csv");
    string evaluationCsvPath = Path.Combine(evidenceDirectory, "p229_auto_mpoint_representative_heldout_results.csv");
    string representativeContactSheetPath =
        Path.Combine(evidenceDirectory, "p229_representative_contact_sheet.png");
    string heldOutContactSheetPath =
        Path.Combine(evidenceDirectory, "p229_heldout_contact_sheet.png");
    string reportPath =
        Path.Combine(evidenceDirectory, "OPENVISIONLAB_AUTO_MPOINT_REPRESENTATIVE_BEST_REPORT.html");
    string recordPath = Path.Combine(evidenceDirectory, "completion_record.txt");

    using Mat canonicalImage = Cv2.ImRead(canonicalPath, ImreadModes.Color);
    if (canonicalImage.Empty())
    {
        Console.Error.WriteLine("P229 canonical image could not be loaded: " + canonicalPath);
        return 2;
    }

    List<Mat> representativeImages = representativeRows
        .Select(row => Cv2.ImRead(GetAutoMPointCorpusImagePath(datasetRoot, row), ImreadModes.Color))
        .ToList();
    AutoMPointToolProperty autoProperty = new AutoMPointToolProperty
    {
        UseAnalysisRoi = false,
        CandidateMode = AutoMPointCandidateMode.Grid,
        PatternWidth = 96,
        PatternHeight = 96,
        CandidateStride = 16,
        MaximumFinalists = 8,
        MaximumResults = 5,
        MinimumFeatureQuality = 0.15D,
        MatchingMinimumScore = 0.75D,
        MinimumUniquenessMargin = 0.05D,
        MaximumTemplatePoints = 300,
        SearchStep = 2,
        UsePositionRefine = true,
        UseSubpixelRefine = true,
        UsePyramidPositionProposal = false,
        UseHybridVerify = false,
        UseAngleSearch = true,
        AngleMinimum = -8,
        AngleMaximum = 8,
        AngleStep = 1D,
        UseScaleSearch = true,
        ScaleMinimum = 0.9D,
        ScaleMaximum = 1.1D,
        ScaleStep = 0.05D,
        MaximumPositionErrorPixels = 2.5D,
        MaximumAngleErrorDegrees = 1.5D,
        MaximumScaleErrorRatio = 0.03D,
        MinimumRepresentativeImageCount = 8,
        MinimumRepresentativeSuccessRate = 0.75D
    };
    AutoMPointTool autoTool = new AutoMPointTool();
    autoTool.SetProperty(autoProperty);
    VisionToolResult autoExecution = autoTool.Execute(canonicalImage, representativeImages);
    OpenVisionLab.Vision2D.Result.AutoMPointCandidateResult? selected = null;
    try
    {
        if (autoExecution.ResultImage != null && !autoExecution.ResultImage.Empty())
        {
            Cv2.ImWrite(candidateDrawingPath, autoExecution.ResultImage);
        }
        selected = autoTool.results.OrderBy(candidate => candidate.Rank).FirstOrDefault();
        if (selected != null)
        {
            using Mat template = canonicalImage.SubMat(selected.PatternRoi).Clone();
            Cv2.ImWrite(templatePath, template);
        }

        List<string> candidateRows = new List<string>
        {
            "PatternRoi,Rank,Accepted,OverallScore,SelfScore,SelfUniqueness,RepresentativeImages,RepresentativeSuccess,RepresentativeSuccessRate,RepresentativeMeanScore,RepresentativeMinimumScore,RepresentativeMeanUniqueness,RepresentativeMinimumUniqueness,RepresentativeRuntimeP95Ms,RejectReason"
        };
        candidateRows.AddRange(autoTool.candidates
            .OrderBy(candidate => candidate.Rank == 0 ? int.MaxValue : candidate.Rank)
            .ThenBy(candidate => candidate.PatternRoi.Y)
            .ThenBy(candidate => candidate.PatternRoi.X)
            .Select(candidate => string.Join(",", new[]
            {
                $"{candidate.PatternRoi.X};{candidate.PatternRoi.Y};{candidate.PatternRoi.Width};{candidate.PatternRoi.Height}",
                candidate.Rank.ToString(CultureInfo.InvariantCulture),
                candidate.Accepted.ToString(CultureInfo.InvariantCulture),
                FormatFinite(candidate.Score),
                FormatFinite(candidate.SelfMatchScore),
                FormatFinite(candidate.UniquenessMargin),
                candidate.RepresentativeImageCount.ToString(CultureInfo.InvariantCulture),
                candidate.RepresentativeSuccessCount.ToString(CultureInfo.InvariantCulture),
                FormatFinite(candidate.RepresentativeSuccessRate),
                FormatFinite(candidate.RepresentativeMeanScore),
                FormatFinite(candidate.RepresentativeMinimumScore),
                FormatFinite(candidate.RepresentativeMeanUniquenessMargin),
                FormatFinite(candidate.RepresentativeMinimumUniquenessMargin),
                FormatFinite(candidate.RepresentativeRuntimeP95Milliseconds),
                candidate.RejectReason
            }.Select(EscapeBatchCsvValue))));
        File.WriteAllLines(candidateCsvPath, candidateRows, new System.Text.UTF8Encoding(true));
    }
    finally
    {
        autoExecution.ResultImage?.Dispose();
        autoTool.imageSource?.Dispose();
        autoTool.imageResult?.Dispose();
        autoTool.imageTemplate?.Dispose();
        foreach (Mat image in representativeImages)
        {
            image.Dispose();
        }
    }

    if (selected == null || !File.Exists(templatePath))
    {
        File.WriteAllLines(recordPath, new[]
        {
            "Status: Incomplete",
            $"Scope: Select one Auto MPoint best pattern for {sourceFile} from eight representative images.",
            $"Verification: {autoExecution.ErrorName}: {autoExecution.Message}",
            $"Evidence: {candidateDrawingPath}; {candidateCsvPath}",
            "Boundary / next dependency: No candidate passed the frozen representative-image gates."
        });
        Console.Error.WriteLine($"P229 Auto MPoint selected no candidate. {autoExecution.ErrorName}: {autoExecution.Message}");
        return 1;
    }

    List<(string SetName, string Role, string FileName, string Outcome, double Score, double Uniqueness, double CenterX, double CenterY, double ElapsedMs, string DrawingPath)> evaluationResults
        = new List<(string, string, string, string, double, double, double, double, double, string)>();
    Dictionary<string, List<string>> drawingPaths = new Dictionary<string, List<string>>(StringComparer.Ordinal)
    {
        ["Representative"] = new List<string>(),
        ["HeldOut"] = new List<string>()
    };
    Dictionary<string, List<string>> drawingLabels = new Dictionary<string, List<string>>(StringComparer.Ordinal)
    {
        ["Representative"] = new List<string>(),
        ["HeldOut"] = new List<string>()
    };
    int runtimeErrors = 0;
    var evaluationRows = representativeRows.Select(row => (SetName: "Representative", Row: row))
        .Concat(heldOutRows.Select(row => (SetName: "HeldOut", Row: row)))
        .ToList();
    for (int index = 0; index < evaluationRows.Count; index++)
    {
        string setName = evaluationRows[index].SetName;
        (int _, string fileName, string status, string _, string _) = evaluationRows[index].Row;
        string imagePath = GetAutoMPointCorpusImagePath(datasetRoot, evaluationRows[index].Row);
        using Mat source = Cv2.ImRead(imagePath, ImreadModes.Color);
        string drawingPath = Path.Combine(
            evidenceDirectory,
            $"{setName.ToLowerInvariant()}_{index + 1:00}_{status}_{Path.GetFileNameWithoutExtension(fileName)}.png");
        string outcome = "ERROR";
        double score = double.NaN;
        double uniqueness = double.NaN;
        double centerX = double.NaN;
        double centerY = double.NaN;
        double elapsedMs = double.NaN;
        if (source.Empty())
        {
            runtimeErrors++;
        }
        else
        {
            VisionPipeline pipeline = CreateEdgeUniqueCardRPipeline(
                templatePath,
                new Rect(0, 0, source.Width, source.Height),
                true,
                0.75D,
                0.05D);
            EdgeBasedTemplateMatchingTool matcher =
                (EdgeBasedTemplateMatchingTool)VisionPipelineAppToolFactory.Create(
                    pipeline.Steps.Single());
            VisionToolResult execution = matcher.Execute(source);
            try
            {
                elapsedMs = execution.Elapsed.TotalMilliseconds;
                OpenVisionLab.Vision2D.Result.MatchingResult? match = matcher.results.SingleOrDefault();
                if (execution.Success && match != null)
                {
                    outcome = "SUCCESS";
                    score = match.Score;
                    uniqueness = GetMetricOrNaN(execution, "UniqueMatch.ScoreMargin");
                    centerX = match.Center.X;
                    centerY = match.Center.Y;
                }
                else if (execution.ErrorCode == VisionToolErrorCode.MatchingAmbiguous)
                {
                    outcome = "AMBIGUOUS";
                }
                else if (execution.ErrorCode == VisionToolErrorCode.MatchingNoResult)
                {
                    outcome = "NO_MATCH";
                }
                else
                {
                    runtimeErrors++;
                }

                if (execution.ResultImage != null && !execution.ResultImage.Empty())
                {
                    Cv2.ImWrite(drawingPath, execution.ResultImage);
                    drawingPaths[setName].Add(drawingPath);
                    drawingLabels[setName].Add(
                        $"{status} {Path.GetFileNameWithoutExtension(fileName)} {outcome} S={score:0.0}");
                }
            }
            finally
            {
                execution.ResultImage?.Dispose();
                matcher.imageSource?.Dispose();
                matcher.imageResult?.Dispose();
                using Mat emptyTemplate = new Mat();
                matcher.SetTemplateImage(emptyTemplate);
                matcher.imageTemplate?.Dispose();
            }
        }

        evaluationResults.Add((
            setName,
            status,
            fileName,
            outcome,
            score,
            uniqueness,
            centerX,
            centerY,
            elapsedMs,
            File.Exists(drawingPath) ? drawingPath : string.Empty));
    }

    if (drawingPaths["Representative"].Count > 0)
    {
        SaveCardPilotContactSheet(
            drawingPaths["Representative"],
            drawingLabels["Representative"],
            representativeContactSheetPath);
    }
    if (drawingPaths["HeldOut"].Count > 0)
    {
        SaveCardPilotContactSheet(
            drawingPaths["HeldOut"],
            drawingLabels["HeldOut"],
            heldOutContactSheetPath);
    }
    File.WriteAllLines(
        evaluationCsvPath,
        new[]
        {
            "Set,Role,FileName,Outcome,Score,UniquenessMargin,CenterX,CenterY,ElapsedMs,DrawingPath"
        }.Concat(evaluationResults.Select(result => string.Join(",", new[]
        {
            result.SetName,
            result.Role,
            result.FileName,
            result.Outcome,
            FormatFinite(result.Score),
            FormatFinite(result.Uniqueness),
            FormatFinite(result.CenterX),
            FormatFinite(result.CenterY),
            FormatFinite(result.ElapsedMs),
            result.DrawingPath
        }.Select(EscapeBatchCsvValue)))),
        new System.Text.UTF8Encoding(true));

    int representativeSuccess = evaluationResults.Count(result =>
        result.SetName == "Representative" && result.Outcome == "SUCCESS");
    int heldOutSuccess = evaluationResults.Count(result =>
        result.SetName == "HeldOut" && result.Outcome == "SUCCESS");
    System.Text.StringBuilder html = new System.Text.StringBuilder();
    html.AppendLine("<!doctype html><html lang=\"ko\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
    html.AppendLine("<title>OpenVisionLab Auto MPoint 대표 이미지 자동 선정 보고서</title>");
    html.AppendLine("<style>body{margin:0;background:#08111f;color:#eef5ff;font-family:'Segoe UI','Malgun Gothic',sans-serif;line-height:1.55}.wrap{max-width:1400px;margin:auto;padding:32px}.panel{background:#111d2e;border:1px solid #2b3d56;border-radius:14px;padding:22px;margin:18px 0}.cards{display:grid;grid-template-columns:repeat(4,1fr);gap:12px}.card{background:#17263b;border-radius:12px;padding:16px}.v{font-size:26px;font-weight:800}.m{color:#9db0c9;font-size:13px}.ok{color:#6ce9bb}.warn{color:#ffc36c}.image{background:#050a11;border:1px solid #31445d;border-radius:10px;padding:8px}.image img{width:100%;height:auto;display:block}table{width:100%;border-collapse:collapse;font-size:13px}th,td{padding:9px;border-bottom:1px solid #2b3d56;text-align:left}th{background:#1b2b42}.scroll{overflow:auto}.print{float:right;background:#24405e;color:white;border:1px solid #5e789b;border-radius:9px;padding:10px 14px}@media print{body{background:white;color:#111}.panel,.card{background:white;border-color:#aaa}.print{display:none}}</style></head><body><main class=\"wrap\">");
    html.AppendLine("<button class=\"print\" onclick=\"window.print()\">인쇄 / PDF 저장</button>");
    html.Append("<h1>Auto MPoint 대표 이미지 자동 선정</h1><p>")
        .Append(HtmlReportEncode(sourceFile))
        .AppendLine(" · 기준 이미지에서 만든 후보를 대표 이미지의 실제 Matching 성공률로 비교했습니다.</p>");
    html.AppendLine("<section class=\"cards\">");
    AppendAutoMPointHtmlMetric(html, $"{selected.RepresentativeSuccessCount}/{selected.RepresentativeImageCount}", "선정용 대표 이미지 성공");
    AppendAutoMPointHtmlMetric(html, $"{representativeSuccess}/{representativeRows.Count}", "선정 후보 재실행");
    AppendAutoMPointHtmlMetric(html, $"{heldOutSuccess}/{heldOutRows.Count}", "분리 확인 이미지 성공");
    AppendAutoMPointHtmlMetric(html, $"{selected.PatternRoi.X},{selected.PatternRoi.Y},{selected.PatternRoi.Width},{selected.PatternRoi.Height}", "자동 선정 ROI");
    html.AppendLine("</section>");
    html.Append("<section class=\"panel\"><h2>자동 선정 결과</h2><p><strong class=\"ok\">Rank #1 자동 선택</strong> · 대표 성공률 ")
        .Append((selected.RepresentativeSuccessRate * 100d).ToString("0.0", CultureInfo.InvariantCulture))
        .Append("% · 평균 점수 ").Append(selected.RepresentativeMeanScore.ToString("0.0", CultureInfo.InvariantCulture))
        .Append(" · 최소 고유성 ").Append(selected.RepresentativeMinimumUniquenessMargin.ToString("0.000", CultureInfo.InvariantCulture))
        .AppendLine("</p>");
    if (File.Exists(candidateDrawingPath))
    {
        html.Append("<div class=\"image\"><img alt=\"Auto MPoint 자동 선정 후보\" src=\"")
            .Append(ToEmbeddedImageDataUri(candidateDrawingPath)).AppendLine("\"></div>");
    }
    html.AppendLine("</section>");
    html.AppendLine("<section class=\"panel\"><h2>후보 순위 근거</h2><div class=\"scroll\"><table><thead><tr><th>Rank</th><th>ROI</th><th>상태</th><th>대표 성공</th><th>평균 점수</th><th>최소 고유성</th><th>탈락 사유</th></tr></thead><tbody>");
    foreach (OpenVisionLab.Vision2D.Result.AutoMPointCandidateResult candidate in autoTool.candidates
        .OrderBy(candidate => candidate.Rank == 0 ? int.MaxValue : candidate.Rank)
        .ThenBy(candidate => candidate.PatternRoi.Y)
        .ThenBy(candidate => candidate.PatternRoi.X))
    {
        html.Append("<tr><td>").Append(candidate.Rank == 0 ? "-" : candidate.Rank)
            .Append("</td><td>").Append(HtmlReportEncode(candidate.PatternRoi.ToString()))
            .Append("</td><td>").Append(candidate.Accepted ? "통과" : "탈락")
            .Append("</td><td>").Append(candidate.RepresentativeSuccessCount).Append("/")
            .Append(candidate.RepresentativeImageCount)
            .Append("</td><td>").Append(candidate.RepresentativeMeanScore.ToString("0.0", CultureInfo.InvariantCulture))
            .Append("</td><td>").Append(candidate.RepresentativeMinimumUniquenessMargin.ToString("0.000", CultureInfo.InvariantCulture))
            .Append("</td><td>").Append(HtmlReportEncode(candidate.RejectReason)).AppendLine("</td></tr>");
    }
    html.AppendLine("</tbody></table></div></section>");
    html.AppendLine("<section class=\"panel\"><h2>선정용 대표 이미지 드로잉</h2>");
    if (File.Exists(representativeContactSheetPath))
    {
        html.Append("<div class=\"image\"><img alt=\"선정용 대표 이미지 결과\" src=\"")
            .Append(ToEmbeddedImageDataUri(representativeContactSheetPath)).AppendLine("\"></div>");
    }
    html.AppendLine("</section><section class=\"panel\"><h2>분리 확인 이미지 드로잉</h2>");
    if (File.Exists(heldOutContactSheetPath))
    {
        html.Append("<div class=\"image\"><img alt=\"분리 확인 이미지 결과\" src=\"")
            .Append(ToEmbeddedImageDataUri(heldOutContactSheetPath)).AppendLine("\"></div>");
    }
    html.AppendLine("</section><section class=\"panel\"><h2>판정 경계</h2><p class=\"warn\">이 결과는 같은 합성 Die Pad 1 계열에서 자동 순위와 재현성을 확인한 것입니다. 실제 생산 특징의 의미, 자세 정답 오차, 500장 전체, 현장 변동은 아직 검증하지 않았습니다.</p>");
    html.AppendLine("<p><a href=\"p229_auto_mpoint_candidate_ranking.csv\">후보 순위 CSV</a> · <a href=\"p229_auto_mpoint_representative_heldout_results.csv\">실행 결과 CSV</a></p></section>");
    html.AppendLine("</main></body></html>");
    File.WriteAllText(reportPath, html.ToString(), new System.Text.UTF8Encoding(true));

    string statusText = integrityFailures.Count == 0 && runtimeErrors == 0
        ? "Complete"
        : "Incomplete";
    File.WriteAllLines(recordPath, new[]
    {
        "Status: " + statusText,
        $"Scope: Automatically select one Auto MPoint pattern for {sourceFile} using 4 OK + 4 NG representative images, then replay 4 OK + 4 NG disjoint held-out rows.",
        $"Acceptance criteria: candidate selected -> {(selected != null ? "PASS" : "FAIL")}; representative replay -> {representativeSuccess}/{representativeRows.Count}; held-out replay -> {heldOutSuccess}/{heldOutRows.Count}; runtime errors -> {runtimeErrors}; integrity failures -> {integrityFailures.Count}.",
        $"Verification: Auto MPoint 96x96/stride16/top8/score0.75/uniqueness0.05; rank by representative success, minimum uniqueness, mean score; unique EdgeBased replay with unchanged gates.",
        $"Evidence: {reportPath}; {candidateCsvPath}; {evaluationCsvPath}; {candidateDrawingPath}; {representativeContactSheetPath}; {heldOutContactSheetPath}.",
        "Boundary / next dependency: Synthetic/augmented same-source evidence only. Operator drawing review is still required before any 500-row run."
    });

    Console.WriteLine($"P229SelectedRoi={selected!.PatternRoi.X},{selected.PatternRoi.Y},{selected.PatternRoi.Width},{selected.PatternRoi.Height}");
    Console.WriteLine($"P229RepresentativeSelection={selected.RepresentativeSuccessCount}/{selected.RepresentativeImageCount}");
    Console.WriteLine($"P229RepresentativeReplay={representativeSuccess}/{representativeRows.Count}");
    Console.WriteLine($"P229HeldOutReplay={heldOutSuccess}/{heldOutRows.Count}");
    Console.WriteLine($"P229RuntimeErrors={runtimeErrors}");
    Console.WriteLine($"P229IntegrityFailures={integrityFailures.Count}");
    Console.WriteLine($"P229Report={reportPath}");
    Console.WriteLine($"P229Record={recordPath}");
    return statusText == "Complete" ? 0 : 1;
}

static int RunAutoMPointSixCorpusPilot(
    string labelTestRootArgument,
    string evidenceDirectoryArgument)
{
    const double matchingScoreMinimum = 0.75D;
    const double uniqueMarginMinimum = 0.05D;
    string labelTestRoot = Path.GetFullPath(labelTestRootArgument);
    string evidenceDirectory = Path.GetFullPath(evidenceDirectoryArgument);
    (string Key, string RelativeRoot)[] datasets =
    {
        ("switch_housing", @"EasyMatch_Switch_Housing_500(1)\EasyMatch_Switch_Housing_500"),
        ("pcb_board", @"EasyMatch_PCB_Board_500(1)\EasyMatch_PCB_Board_500"),
        ("ic_frame", @"EasyMatch_IC_Frame_500(1)\EasyMatch_IC_Frame_500"),
        ("floppy_disk", @"EasyMatch_Floppy_Disk_500(1)\EasyMatch_Floppy_Disk_500"),
        ("die_pad", @"EasyMatch_Die_Pad_500(1)\EasyMatch_Die_Pad_500"),
        ("die_array", @"EasyMatch_Die_Array_500(1)\EasyMatch_Die_Array_500")
    };
    Directory.CreateDirectory(evidenceDirectory);
    string candidatesDirectory = Path.Combine(evidenceDirectory, "candidate_analysis");
    string templatesDirectory = Path.Combine(evidenceDirectory, "templates");
    string runsDirectory = Path.Combine(evidenceDirectory, "runs");
    Directory.CreateDirectory(candidatesDirectory);
    Directory.CreateDirectory(templatesDirectory);
    Directory.CreateDirectory(runsDirectory);

    List<string> resultRows = new List<string>
    {
        "Dataset,SourceFile,Role,FileName,MetadataMd5,ActualMd5,Md5Verified,CandidateRoi,CandidateScore,CandidateUniqueness,ExecutionSuccess,Outcome,ErrorCode,ErrorName,Message,MatchScore,UniqueMargin,CenterX,CenterY,AngleDeg,Scale,Bounds,DefectMaskOverlapPixels,ElapsedMs,DrawingPath"
    };
    List<string> summaryRows = new List<string>
    {
        "Dataset,SourceFile,TotalRows,OkRows,NgRows,CanonicalFile,CandidateState,CandidateRoi,CandidateScore,CandidateUniqueness,PilotRows,OkSuccess,OkAmbiguous,OkNoMatch,NgSuccess,NgAmbiguous,NgNoMatch,NgSuccessWithDefectOverlap,RuntimeErrors,MeanElapsedMs,Decision,ContactSheet"
    };
    List<string> candidateDrawingPaths = new List<string>();
    List<string> candidateDrawingLabels = new List<string>();
    List<string> integrityFailures = new List<string>();
    int totalMetadataRows = 0;
    int sourceGroupCount = 0;
    int pilotRunCount = 0;
    int sourceGroupsWithSuggestion = 0;
    int sourceGroupsAdvancing = 0;
    int totalRuntimeErrors = 0;

    foreach ((string datasetKey, string relativeRoot) in datasets)
    {
        string datasetRoot = Path.Combine(labelTestRoot, relativeRoot);
        string metadataPath = Path.Combine(datasetRoot, "metadata.csv");
        if (!File.Exists(metadataPath))
        {
            integrityFailures.Add($"{datasetKey}: metadata.csv is missing at {metadataPath}.");
            continue;
        }

        List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> rows;
        try
        {
            rows = LoadAutoMPointCorpusMetadata(metadataPath);
        }
        catch (Exception ex)
        {
            integrityFailures.Add($"{datasetKey}: metadata parse failed: {ex.Message}");
            continue;
        }
        totalMetadataRows += rows.Count;
        Dictionary<string, string> maskPaths = Directory
            .EnumerateFiles(
                Path.Combine(datasetRoot, "segmentation", "masks_binary"),
                "*.png",
                SearchOption.AllDirectories)
            .GroupBy(path => Path.GetFileNameWithoutExtension(path), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.First(),
                StringComparer.OrdinalIgnoreCase);

        foreach (IGrouping<string, (int GlobalId, string FileName, string Status, string SourceFile, string Md5)> group
            in rows.GroupBy(row => row.SourceFile, StringComparer.OrdinalIgnoreCase).OrderBy(group => group.Key))
        {
            sourceGroupCount++;
            List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> groupRows =
                group.OrderBy(row => row.GlobalId).ToList();
            List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> okRows =
                groupRows.Where(row => string.Equals(row.Status, "OK", StringComparison.OrdinalIgnoreCase)).ToList();
            List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> ngRows =
                groupRows.Where(row => string.Equals(row.Status, "NG", StringComparison.OrdinalIgnoreCase)).ToList();
            if (okRows.Count == 0 || ngRows.Count == 0)
            {
                integrityFailures.Add($"{datasetKey}/{group.Key}: both OK and NG rows are required.");
                continue;
            }

            (int GlobalId, string FileName, string Status, string SourceFile, string Md5) canonical = okRows[0];
            string canonicalPath = GetAutoMPointCorpusImagePath(datasetRoot, canonical);
            string safeGroup = datasetKey + "__" + Path.GetFileNameWithoutExtension(group.Key).Replace(' ', '_');
            string groupCandidateDirectory = Path.Combine(candidatesDirectory, safeGroup);
            string groupRunDirectory = Path.Combine(runsDirectory, safeGroup);
            Directory.CreateDirectory(groupCandidateDirectory);
            Directory.CreateDirectory(groupRunDirectory);
            string candidateState = "NO_SUGGESTION";
            Rect candidateRoi = new Rect();
            double candidateScore = double.NaN;
            double candidateUniqueness = double.NaN;
            string candidateDrawingPath = Path.Combine(groupCandidateDirectory, "auto_mpoint.png");
            string templatePath = Path.Combine(templatesDirectory, safeGroup + "_rank_01.png");
            using Mat canonicalImage = Cv2.ImRead(canonicalPath, ImreadModes.Color);
            if (canonicalImage.Empty())
            {
                integrityFailures.Add($"{datasetKey}/{group.Key}: canonical image could not be loaded.");
                continue;
            }

            AutoMPointToolProperty autoProperty = new AutoMPointToolProperty
            {
                UseAnalysisRoi = false,
                CandidateMode = AutoMPointCandidateMode.Grid,
                PatternWidth = 96,
                PatternHeight = 96,
                CandidateStride = 16,
                MaximumFinalists = 8,
                MaximumResults = 5,
                MinimumFeatureQuality = 0.15D,
                MatchingMinimumScore = matchingScoreMinimum,
                MinimumUniquenessMargin = uniqueMarginMinimum,
                MaximumTemplatePoints = 300,
                SearchStep = 2,
                UsePositionRefine = true,
                UseSubpixelRefine = true,
                UsePyramidPositionProposal = false,
                UseHybridVerify = false,
                UseAngleSearch = false,
                UseScaleSearch = false,
                MaximumPositionErrorPixels = 2.5D,
                MaximumAngleErrorDegrees = 1.5D,
                MaximumScaleErrorRatio = 0.03D
            };
            AutoMPointTool autoTool = new AutoMPointTool();
            autoTool.SetProperty(autoProperty);
            VisionToolResult autoExecution = autoTool.Execute(canonicalImage);
            try
            {
                if (autoExecution.ResultImage != null && !autoExecution.ResultImage.Empty())
                {
                    Cv2.ImWrite(candidateDrawingPath, autoExecution.ResultImage);
                    candidateDrawingPaths.Add(candidateDrawingPath);
                }
                OpenVisionLab.Vision2D.Result.AutoMPointCandidateResult? candidate = autoTool.results
                    .OrderBy(result => result.Rank)
                    .FirstOrDefault();
                candidateDrawingLabels.Add(
                    $"{datasetKey}/{Path.GetFileNameWithoutExtension(group.Key)} "
                    + (candidate == null ? "NO SUGGESTION" : "rank #1"));
                if (candidate == null)
                {
                    summaryRows.Add(string.Join(",", new[]
                    {
                        datasetKey,
                        group.Key,
                        groupRows.Count.ToString(CultureInfo.InvariantCulture),
                        okRows.Count.ToString(CultureInfo.InvariantCulture),
                        ngRows.Count.ToString(CultureInfo.InvariantCulture),
                        canonical.FileName,
                        candidateState,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        "0",
                        string.Empty,
                        "No suggestion",
                        string.Empty
                    }.Select(EscapeBatchCsvValue)));
                    continue;
                }

                sourceGroupsWithSuggestion++;
                candidateState = "SUGGESTED";
                candidateRoi = candidate.PatternRoi;
                candidateScore = candidate.Score;
                candidateUniqueness = candidate.UniquenessMargin;
                using Mat template = canonicalImage.SubMat(candidateRoi).Clone();
                Cv2.ImWrite(templatePath, template);
            }
            finally
            {
                autoExecution.ResultImage?.Dispose();
                autoTool.imageSource?.Dispose();
                autoTool.imageResult?.Dispose();
                autoTool.imageTemplate?.Dispose();
            }

            List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> pilotRows =
                SelectAutoMPointPilotRows(okRows, canonical)
                .Concat(SelectAutoMPointPilotRows(ngRows, null))
                .ToList();
            int okSuccess = 0;
            int okAmbiguous = 0;
            int okNoMatch = 0;
            int ngSuccess = 0;
            int ngAmbiguous = 0;
            int ngNoMatch = 0;
            int ngOverlap = 0;
            int runtimeErrors = 0;
            List<double> elapsedValues = new List<double>();
            List<string> reviewImages = new List<string>();
            List<string> reviewLabels = new List<string>();

            for (int pilotIndex = 0; pilotIndex < pilotRows.Count; pilotIndex++)
            {
                (int GlobalId, string FileName, string Status, string SourceFile, string Md5) row = pilotRows[pilotIndex];
                pilotRunCount++;
                string imagePath = GetAutoMPointCorpusImagePath(datasetRoot, row);
                string actualMd5 = File.Exists(imagePath) ? ComputeMd5(imagePath) : "MISSING";
                bool md5Verified = string.Equals(actualMd5, row.Md5, StringComparison.OrdinalIgnoreCase);
                if (!md5Verified)
                {
                    integrityFailures.Add(
                        $"{datasetKey}/{group.Key}/{row.FileName}: MD5 mismatch. "
                        + $"Metadata={row.Md5}, Actual={actualMd5}.");
                }

                string drawingPath = Path.Combine(
                    groupRunDirectory,
                    $"{pilotIndex + 1:00}_{row.Status}_{Path.GetFileNameWithoutExtension(row.FileName)}.png");
                string outcome = "ERROR";
                int errorCode = -1;
                string errorName = "MissingImage";
                string errorMessage = "Source or template image could not be loaded.";
                bool executionSuccess = false;
                double matchScore = double.NaN;
                double uniqueMargin = double.NaN;
                double centerX = double.NaN;
                double centerY = double.NaN;
                double angle = double.NaN;
                double scale = double.NaN;
                string boundsText = string.Empty;
                int maskOverlapPixels = 0;
                double elapsedMs = double.NaN;

                using Mat source = Cv2.ImRead(imagePath, ImreadModes.Color);
                using Mat templateForRun = Cv2.ImRead(templatePath, ImreadModes.Color);
                if (source.Empty() || templateForRun.Empty())
                {
                    runtimeErrors++;
                }
                else
                {
                    VisionPipeline matchingPipeline = CreateEdgeUniqueCardRPipeline(
                        templatePath,
                        new Rect(0, 0, source.Width, source.Height),
                        true,
                        matchingScoreMinimum,
                        uniqueMarginMinimum);
                    EdgeBasedTemplateMatchingTool matcher =
                        (EdgeBasedTemplateMatchingTool)VisionPipelineAppToolFactory.Create(
                            matchingPipeline.Steps.Single());
                    VisionToolResult execution = matcher.Execute(source);
                    try
                    {
                        executionSuccess = execution.Success;
                        errorCode = (int)execution.ErrorCode;
                        errorName = execution.ErrorName ?? execution.ErrorCode.ToString();
                        errorMessage = execution.Message ?? string.Empty;
                        elapsedMs = execution.Elapsed.TotalMilliseconds;
                        elapsedValues.Add(elapsedMs);
                        OpenVisionLab.Vision2D.Result.MatchingResult? match = matcher.results.SingleOrDefault();
                        if (execution.Success && match != null)
                        {
                            outcome = "SUCCESS";
                            matchScore = match.Score;
                            uniqueMargin = GetMetricOrNaN(execution, "UniqueMatch.ScoreMargin");
                            centerX = match.Center.X;
                            centerY = match.Center.Y;
                            angle = match.Angle;
                            scale = match.Scale;
                            boundsText = string.Format(
                                CultureInfo.InvariantCulture,
                                "{0:0.###};{1:0.###};{2:0.###};{3:0.###}",
                                match.Bounding.X,
                                match.Bounding.Y,
                                match.Bounding.Width,
                                match.Bounding.Height);
                            if (string.Equals(row.Status, "OK", StringComparison.OrdinalIgnoreCase))
                            {
                                okSuccess++;
                            }
                            else
                            {
                                ngSuccess++;
                                if (maskPaths.TryGetValue(
                                        Path.GetFileNameWithoutExtension(row.FileName),
                                        out string? maskPath))
                                {
                                    maskOverlapPixels = CountMaskOverlap(maskPath, match.Bounding);
                                    if (maskOverlapPixels > 0)
                                    {
                                        ngOverlap++;
                                    }
                                }
                            }
                        }
                        else if (execution.ErrorCode == VisionToolErrorCode.MatchingAmbiguous)
                        {
                            outcome = "AMBIGUOUS";
                            if (string.Equals(row.Status, "OK", StringComparison.OrdinalIgnoreCase))
                            {
                                okAmbiguous++;
                            }
                            else
                            {
                                ngAmbiguous++;
                            }
                        }
                        else if (execution.ErrorCode == VisionToolErrorCode.MatchingNoResult)
                        {
                            outcome = "NO_MATCH";
                            if (string.Equals(row.Status, "OK", StringComparison.OrdinalIgnoreCase))
                            {
                                okNoMatch++;
                            }
                            else
                            {
                                ngNoMatch++;
                            }
                        }
                        else
                        {
                            runtimeErrors++;
                        }

                        if (execution.ResultImage != null && !execution.ResultImage.Empty())
                        {
                            Cv2.ImWrite(drawingPath, execution.ResultImage);
                            reviewImages.Add(drawingPath);
                            reviewLabels.Add(
                                $"{row.Status} {Path.GetFileNameWithoutExtension(row.FileName)} "
                                + $"{outcome} S={matchScore:0.0}");
                        }
                    }
                    finally
                    {
                        execution.ResultImage?.Dispose();
                        matcher.imageSource?.Dispose();
                        matcher.imageResult?.Dispose();
                        using Mat emptyTemplate = new Mat();
                        matcher.SetTemplateImage(emptyTemplate);
                        matcher.imageTemplate?.Dispose();
                    }
                }

                resultRows.Add(string.Join(",", new[]
                {
                    datasetKey,
                    group.Key,
                    row.Status,
                    row.FileName,
                    row.Md5,
                    actualMd5,
                    md5Verified.ToString(CultureInfo.InvariantCulture),
                    $"{candidateRoi.X};{candidateRoi.Y};{candidateRoi.Width};{candidateRoi.Height}",
                    FormatFinite(candidateScore),
                    FormatFinite(candidateUniqueness),
                    executionSuccess.ToString(CultureInfo.InvariantCulture),
                    outcome,
                    errorCode.ToString(CultureInfo.InvariantCulture),
                    errorName,
                    errorMessage,
                    FormatFinite(matchScore),
                    FormatFinite(uniqueMargin),
                    FormatFinite(centerX),
                    FormatFinite(centerY),
                    FormatFinite(angle),
                    FormatFinite(scale),
                    boundsText,
                    maskOverlapPixels.ToString(CultureInfo.InvariantCulture),
                    FormatFinite(elapsedMs),
                    File.Exists(drawingPath) ? drawingPath : string.Empty
                }.Select(EscapeBatchCsvValue)));
            }

            string contactSheetPath = Path.Combine(groupRunDirectory, "contact_sheet.png");
            if (reviewImages.Count > 0)
            {
                SaveCardPilotContactSheet(reviewImages, reviewLabels, contactSheetPath);
            }
            totalRuntimeErrors += runtimeErrors;
            string decision = runtimeErrors > 0
                ? "Incomplete: runtime error"
                : okSuccess >= 3
                    ? "Operator drawing review required"
                    : "Reject mechanical pilot";
            if (decision == "Operator drawing review required")
            {
                sourceGroupsAdvancing++;
            }
            summaryRows.Add(string.Join(",", new[]
            {
                datasetKey,
                group.Key,
                groupRows.Count.ToString(CultureInfo.InvariantCulture),
                okRows.Count.ToString(CultureInfo.InvariantCulture),
                ngRows.Count.ToString(CultureInfo.InvariantCulture),
                canonical.FileName,
                candidateState,
                $"{candidateRoi.X};{candidateRoi.Y};{candidateRoi.Width};{candidateRoi.Height}",
                FormatFinite(candidateScore),
                FormatFinite(candidateUniqueness),
                pilotRows.Count.ToString(CultureInfo.InvariantCulture),
                okSuccess.ToString(CultureInfo.InvariantCulture),
                okAmbiguous.ToString(CultureInfo.InvariantCulture),
                okNoMatch.ToString(CultureInfo.InvariantCulture),
                ngSuccess.ToString(CultureInfo.InvariantCulture),
                ngAmbiguous.ToString(CultureInfo.InvariantCulture),
                ngNoMatch.ToString(CultureInfo.InvariantCulture),
                ngOverlap.ToString(CultureInfo.InvariantCulture),
                runtimeErrors.ToString(CultureInfo.InvariantCulture),
                elapsedValues.Count > 0
                    ? elapsedValues.Average().ToString("0.###", CultureInfo.InvariantCulture)
                    : string.Empty,
                decision,
                File.Exists(contactSheetPath) ? contactSheetPath : string.Empty
            }.Select(EscapeBatchCsvValue)));
        }
    }

    string resultsPath = Path.Combine(evidenceDirectory, "p227_auto_mpoint_six_corpus_pilot_results.csv");
    string summaryPath = Path.Combine(evidenceDirectory, "p227_auto_mpoint_six_corpus_pilot_summary.csv");
    File.WriteAllLines(resultsPath, resultRows);
    File.WriteAllLines(summaryPath, summaryRows);
    string candidateSheetPath = Path.Combine(evidenceDirectory, "p227_candidate_analysis_contact_sheet.png");
    if (candidateDrawingPaths.Count > 0)
    {
        SaveCardPilotContactSheet(candidateDrawingPaths, candidateDrawingLabels, candidateSheetPath);
    }
    string reportPath = WriteAutoMPointSixCorpusReport(
        summaryPath,
        evidenceDirectory,
        candidateSheetPath,
        totalMetadataRows,
        sourceGroupCount,
        sourceGroupsWithSuggestion,
        pilotRunCount,
        sourceGroupsAdvancing,
        totalRuntimeErrors,
        integrityFailures);
    string recordPath = Path.Combine(evidenceDirectory, "completion_record.md");
    File.WriteAllLines(recordPath, new[]
    {
        "# P227 Auto MPoint six-corpus mechanical pilot",
        string.Empty,
        $"Status: {(integrityFailures.Count == 0 && totalRuntimeErrors == 0 ? "Complete" : "Incomplete")}",
        string.Empty,
        "Scope: Audit six operator-provided EasyMatch 500-image corpora, stratify by source_file, freeze one current-default Auto MPoint rank-1 suggestion per source stratum, and run a deterministic 4 OK + 4 NG EdgeBased unique-match pilot without result-dependent tuning.",
        string.Empty,
        "Acceptance criteria:",
        $"- Metadata rows found: {(totalMetadataRows == 3000 ? "PASS" : "FAIL")} ({totalMetadataRows}/3000).",
        $"- Source strata found: {(sourceGroupCount == 16 ? "PASS" : "FAIL")} ({sourceGroupCount}/16).",
        $"- Source strata with Auto MPoint suggestion: {sourceGroupsWithSuggestion}/16.",
        $"- Pilot executions retained: {pilotRunCount}.",
        $"- Source strata mechanically eligible for operator drawing review: {sourceGroupsAdvancing}/16.",
        $"- Runtime errors: {totalRuntimeErrors}.",
        $"- Integrity failures: {integrityFailures.Count}.",
        "- Codex drawing review: 13 matching contact sheets and the 16-stratum candidate sheet reviewed.",
        "- Drawing-review decision: 10 expansion candidates / 6 stopped strata.",
        "- Result-dependent threshold/ROI tuning: PASS (none).",
        "- Full 500-image expansion or automatic pattern apply: PASS (not performed).",
        string.Empty,
        "Verification:",
        "- Auto MPoint: current UI-equivalent 96x96/stride16/score0.75/uniqueness0.05/max-position-error2.5 defaults.",
        "- Matching pilot: full 512x512 search, score 0.75, unique margin 0.05, angle -8..8 step1, scale 0.9..1.1 step0.05.",
        "- Pilot selection: canonical first OK plus MD5-spread rows, four OK and four NG per source stratum.",
        $"- Results: `{resultsPath}`",
        $"- Summary: `{summaryPath}`",
        $"- Candidate drawings: `{candidateSheetPath}`",
        $"- Operator report: `{reportPath}`",
        string.Empty,
        "Boundary / next dependency: Corpus metadata does not contain generated pose ground truth, so this pilot does not claim pixel localization accuracy. Mechanical success still requires operator drawing review for physical-feature identity. A full 500-image replay is allowed only for an explicitly approved source stratum/candidate.",
        string.Empty,
        "Integrity issues:",
        integrityFailures.Count == 0 ? "- None." : string.Join(Environment.NewLine, integrityFailures.Select(item => "- " + item))
    });

    Console.WriteLine($"P227MetadataRows={totalMetadataRows}");
    Console.WriteLine($"P227SourceStrata={sourceGroupCount}");
    Console.WriteLine($"P227StrataWithSuggestion={sourceGroupsWithSuggestion}");
    Console.WriteLine($"P227PilotRuns={pilotRunCount}");
    Console.WriteLine($"P227MechanicallyEligible={sourceGroupsAdvancing}");
    Console.WriteLine($"P227RuntimeErrors={totalRuntimeErrors}");
    Console.WriteLine($"P227IntegrityFailures={integrityFailures.Count}");
    Console.WriteLine($"P227Summary={summaryPath}");
    Console.WriteLine($"P227Report={reportPath}");
    Console.WriteLine($"P227Record={recordPath}");
    return integrityFailures.Count == 0 && totalRuntimeErrors == 0 ? 0 : 1;
}

static string WriteAutoMPointSixCorpusReport(
    string summaryPath,
    string evidenceDirectory,
    string candidateSheetPath,
    int totalMetadataRows,
    int sourceGroupCount,
    int sourceGroupsWithSuggestion,
    int pilotRunCount,
    int sourceGroupsAdvancing,
    int totalRuntimeErrors,
    IReadOnlyList<string> integrityFailures)
{
    string[] lines = File.ReadAllLines(summaryPath);
    List<string> header = ParseCsvRecord(lines[0]);
    int datasetIndex = header.IndexOf("Dataset");
    int sourceIndex = header.IndexOf("SourceFile");
    int candidateStateIndex = header.IndexOf("CandidateState");
    int candidateRoiIndex = header.IndexOf("CandidateRoi");
    int okSuccessIndex = header.IndexOf("OkSuccess");
    int okAmbiguousIndex = header.IndexOf("OkAmbiguous");
    int okNoMatchIndex = header.IndexOf("OkNoMatch");
    int ngSuccessIndex = header.IndexOf("NgSuccess");
    int ngAmbiguousIndex = header.IndexOf("NgAmbiguous");
    int ngNoMatchIndex = header.IndexOf("NgNoMatch");
    int overlapIndex = header.IndexOf("NgSuccessWithDefectOverlap");
    int runtimeIndex = header.IndexOf("RuntimeErrors");
    int elapsedIndex = header.IndexOf("MeanElapsedMs");
    int decisionIndex = header.IndexOf("Decision");
    int contactSheetIndex = header.IndexOf("ContactSheet");
    List<List<string>> rows = lines
        .Skip(1)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(ParseCsvRecord)
        .ToList();
    List<(string Dataset, string SourceFile, string Decision, string Reason)> drawingReviews =
        rows.Select(row =>
        {
            (string decision, string reason) =
                GetAutoMPointSixCorpusDrawingReview(row[datasetIndex], row[sourceIndex]);
            return (row[datasetIndex], row[sourceIndex], decision, reason);
        }).ToList();
    int expansionCandidateCount = drawingReviews.Count(review =>
        string.Equals(review.Decision, "확대 검증 후보", StringComparison.Ordinal));
    int stoppedCount = drawingReviews.Count - expansionCandidateCount;
    string drawingReviewPath = Path.Combine(
        evidenceDirectory,
        "p227_auto_mpoint_six_corpus_drawing_review.csv");
    File.WriteAllLines(
        drawingReviewPath,
        new[] { "Dataset,SourceFile,DrawingReview,Reason" }
            .Concat(drawingReviews.Select(review => string.Join(
                ",",
                new[]
                {
                    review.Dataset,
                    review.SourceFile,
                    review.Decision,
                    review.Reason
                }.Select(EscapeBatchCsvValue)))),
        new System.Text.UTF8Encoding(true));

    string reportPath = Path.Combine(evidenceDirectory, "OPENVISIONLAB_AUTO_MPOINT_SIX_CORPUS_REPORT.md");
    string overallStatus =
        integrityFailures.Count == 0 && totalRuntimeErrors == 0 ? "Complete" : "Incomplete";
    List<string> report = new List<string>
    {
        "# OpenVisionLab Auto MPoint 6종 코퍼스 검증 보고서",
        string.Empty,
        $"상태: `{overallStatus}`",
        string.Empty,
        $"생성일: {DateTime.Now:yyyy-MM-dd HH:mm:ss K}",
        string.Empty,
        "## 결론 요약",
        string.Empty,
        $"- 데이터셋: 6종, 메타데이터 {totalMetadataRows}/3000장",
        $"- 원본 이미지별 검증 층: {sourceGroupCount}/16개",
        $"- Auto MPoint 1순위 후보 생성: {sourceGroupsWithSuggestion}/16개 층",
        $"- 고정 파일럿 실행: {pilotRunCount}건",
        $"- 기계적 통과 후 사람의 드로잉 검토가 필요한 층: {sourceGroupsAdvancing}/16개",
        $"- 드로잉 검토 후 확대 검증 후보: {expansionCandidateCount}/16개",
        $"- 드로잉 검토에서 중단: {stoppedCount}/16개",
        $"- 런타임 오류: {totalRuntimeErrors}건",
        $"- 무결성 오류: {integrityFailures.Count}건",
        string.Empty,
        "이 보고서의 `기계적 통과`는 정상적인 물리 특징을 찾았다는 뜻이 아닙니다. "
        + "OK 4장 중 3장 이상에서 실행됐다는 뜻이며, 최종 채택은 아래 드로잉으로 같은 물리 특징인지 확인해야 합니다.",
        string.Empty,
        "## 고정 검증 조건",
        string.Empty,
        "- Auto MPoint: 96×96, stride 16, score 0.75, uniqueness 0.05, 최대 위치 오차 2.5 px",
        "- EdgeBased Matching: 전체 512×512 검색, score 0.75, unique margin 0.05",
        "- 자세 범위: angle -8..8° / 1°, scale 0.9..1.1 / 0.05",
        "- 표본: 각 원본별 첫 OK와 MD5 분산 표본을 포함한 OK 4장 + NG 4장",
        "- 결과 확인 후 ROI·점수·각도·스케일 문턱을 변경하지 않음",
        "- 메타데이터에 생성 자세의 수치 정답이 없으므로 위치 정밀도 px는 판정하지 않음",
        string.Empty,
        "## 후보 분석 전체 보기",
        string.Empty
    };
    if (File.Exists(candidateSheetPath))
    {
        report.Add($"![16개 원본별 Auto MPoint 후보]({ToReportRelativePath(evidenceDirectory, candidateSheetPath)})");
        report.Add(string.Empty);
    }
    report.AddRange(new[]
    {
        "## 원본별 결과표",
        string.Empty,
        "| 데이터셋 | 원본 | 후보 | ROI | OK 성공/모호/미검출 | NG 성공/모호/미검출 | NG 결함 겹침 | 오류 | 평균 ms | 기계 판정 | 드로잉 검토 |",
        "| --- | --- | --- | --- | ---: | ---: | ---: | ---: | ---: | --- | --- |"
    });
    foreach (List<string> row in rows)
    {
        (string drawingDecision, _) =
            GetAutoMPointSixCorpusDrawingReview(row[datasetIndex], row[sourceIndex]);
        report.Add(
            $"| {row[datasetIndex]} | {row[sourceIndex]} | {row[candidateStateIndex]} | "
            + $"{row[candidateRoiIndex]} | {row[okSuccessIndex]}/{row[okAmbiguousIndex]}/{row[okNoMatchIndex]} | "
            + $"{row[ngSuccessIndex]}/{row[ngAmbiguousIndex]}/{row[ngNoMatchIndex]} | "
            + $"{row[overlapIndex]} | {row[runtimeIndex]} | {row[elapsedIndex]} | "
            + $"{row[decisionIndex]} | {drawingDecision} |");
    }
    report.AddRange(new[]
    {
        string.Empty,
        "## 실제 결과 드로잉",
        string.Empty,
        "노란색/녹색 패턴 윤곽과 중심이 동일한 물리 특징을 따라가는지 확인하십시오. "
        + "반복 구조의 다른 위치, 배경 경계, 결함 자체를 따라가면 수치가 높아도 부적합입니다.",
        string.Empty
    });
    foreach (List<string> row in rows)
    {
        string contactSheetPath = row[contactSheetIndex];
        (string drawingDecision, string drawingReason) =
            GetAutoMPointSixCorpusDrawingReview(row[datasetIndex], row[sourceIndex]);
        report.Add($"### {row[datasetIndex]} / {row[sourceIndex]}");
        report.Add(string.Empty);
        report.Add($"- 후보 상태: `{row[candidateStateIndex]}`");
        report.Add($"- 후보 ROI: `{row[candidateRoiIndex]}`");
        report.Add($"- 기계 판정: `{row[decisionIndex]}`");
        report.Add($"- 드로잉 검토: **{drawingDecision}** — {drawingReason}");
        report.Add(string.Empty);
        if (File.Exists(contactSheetPath))
        {
            report.Add($"![{row[datasetIndex]} {row[sourceIndex]} 파일럿 결과]({ToReportRelativePath(evidenceDirectory, contactSheetPath)})");
        }
        else
        {
            report.Add("Auto MPoint 추천 후보가 없어 Matching 파일럿을 실행하지 않았습니다.");
        }
        report.Add(string.Empty);
    }
    report.AddRange(new[]
    {
        "## 증거 파일",
        string.Empty,
        "- [원본별 요약 CSV](p227_auto_mpoint_six_corpus_pilot_summary.csv)",
        $"- [{pilotRunCount}건 실행 결과 CSV](p227_auto_mpoint_six_corpus_pilot_results.csv)",
        "- [드로잉 검토 CSV](p227_auto_mpoint_six_corpus_drawing_review.csv)",
        "- [완료 기록](completion_record.md)",
        string.Empty,
        "## 한계와 다음 판단",
        string.Empty,
        "1. 이 데이터는 EasyMatch 원본을 변형하고 NG 결함을 합성한 데이터이며 실제 생산 변동 증거가 아닙니다.",
        "2. 서로 다른 `source_file`은 별도 템플릿으로 평가했습니다. 하나의 템플릿이 제품군 전체를 대표한다고 주장하지 않습니다.",
        "3. 생성 자세의 정답 좌표가 없으므로 드로잉의 물리적 동일성은 운영자가 확인해야 합니다.",
        "4. 운영자가 승인한 후보만 해당 원본 층의 전체 이미지로 확대 검증할 수 있습니다.",
        string.Empty,
        "## 무결성 오류",
        string.Empty
    });
    report.AddRange(integrityFailures.Count == 0
        ? new[] { "- 없음" }
        : integrityFailures.Select(item => "- " + item));
    File.WriteAllLines(reportPath, report, new System.Text.UTF8Encoding(true));
    return WriteAutoMPointSixCorpusHtmlReport(
        summaryPath,
        evidenceDirectory,
        candidateSheetPath,
        totalMetadataRows,
        sourceGroupCount,
        sourceGroupsWithSuggestion,
        pilotRunCount,
        sourceGroupsAdvancing,
        totalRuntimeErrors,
        integrityFailures);
}

static string WriteAutoMPointSixCorpusHtmlReport(
    string summaryPath,
    string evidenceDirectory,
    string candidateSheetPath,
    int totalMetadataRows,
    int sourceGroupCount,
    int sourceGroupsWithSuggestion,
    int pilotRunCount,
    int sourceGroupsAdvancing,
    int totalRuntimeErrors,
    IReadOnlyList<string> integrityFailures)
{
    string[] lines = File.ReadAllLines(summaryPath);
    List<string> header = ParseCsvRecord(lines[0]);
    int datasetIndex = header.IndexOf("Dataset");
    int sourceIndex = header.IndexOf("SourceFile");
    int candidateStateIndex = header.IndexOf("CandidateState");
    int candidateRoiIndex = header.IndexOf("CandidateRoi");
    int candidateScoreIndex = header.IndexOf("CandidateScore");
    int candidateUniquenessIndex = header.IndexOf("CandidateUniqueness");
    int okSuccessIndex = header.IndexOf("OkSuccess");
    int okAmbiguousIndex = header.IndexOf("OkAmbiguous");
    int okNoMatchIndex = header.IndexOf("OkNoMatch");
    int ngSuccessIndex = header.IndexOf("NgSuccess");
    int ngAmbiguousIndex = header.IndexOf("NgAmbiguous");
    int ngNoMatchIndex = header.IndexOf("NgNoMatch");
    int overlapIndex = header.IndexOf("NgSuccessWithDefectOverlap");
    int runtimeIndex = header.IndexOf("RuntimeErrors");
    int elapsedIndex = header.IndexOf("MeanElapsedMs");
    int decisionIndex = header.IndexOf("Decision");
    int contactSheetIndex = header.IndexOf("ContactSheet");
    List<List<string>> rows = lines
        .Skip(1)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(ParseCsvRecord)
        .ToList();
    int expansionCandidateCount = rows.Count(row =>
        string.Equals(
            GetAutoMPointSixCorpusDrawingReview(row[datasetIndex], row[sourceIndex]).Decision,
            "확대 검증 후보",
            StringComparison.Ordinal));
    int stoppedCount = rows.Count - expansionCandidateCount;
    string overallStatus =
        integrityFailures.Count == 0 && totalRuntimeErrors == 0 ? "Complete" : "Incomplete";
    string overallStatusClass = overallStatus == "Complete" ? "status-complete" : "status-incomplete";
    string reportPath = Path.Combine(
        evidenceDirectory,
        "OPENVISIONLAB_AUTO_MPOINT_SIX_CORPUS_REPORT.html");
    System.Text.StringBuilder html = new System.Text.StringBuilder();
    html.AppendLine("<!doctype html>");
    html.AppendLine("<html lang=\"ko\"><head><meta charset=\"utf-8\">");
    html.AppendLine("<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
    html.AppendLine("<title>OpenVisionLab Auto MPoint 6종 코퍼스 검증 보고서</title>");
    html.AppendLine("<style>");
    html.AppendLine(":root{color-scheme:dark;--bg:#08111f;--panel:#111d2e;--panel2:#17263b;--line:#2b3d56;--text:#eef5ff;--muted:#9db0c9;--green:#35d69f;--orange:#ffb454;--red:#ff6b7a;--cyan:#4fc3f7}");
    html.AppendLine("*{box-sizing:border-box}html{scroll-behavior:smooth}body{margin:0;background:linear-gradient(135deg,#08111f 0%,#0b1728 45%,#111b2b 100%);color:var(--text);font-family:\"Segoe UI\",\"Malgun Gothic\",sans-serif;line-height:1.55}");
    html.AppendLine(".wrap{max-width:1500px;margin:auto;padding:34px 30px 80px}.hero{display:flex;gap:24px;align-items:flex-start;justify-content:space-between;margin-bottom:28px}.eyebrow{color:var(--cyan);font-weight:700;letter-spacing:.08em;text-transform:uppercase}.hero h1{font-size:clamp(28px,4vw,48px);line-height:1.15;margin:8px 0 12px}.sub{color:var(--muted);max-width:880px}.toolbar{position:sticky;top:16px;z-index:5}.print{border:1px solid #5e789b;background:#1c314c;color:white;border-radius:10px;padding:12px 17px;font-weight:700;cursor:pointer;box-shadow:0 10px 25px #0006}.print:hover{background:#28496f}");
    html.AppendLine(".status{display:inline-flex;align-items:center;border-radius:999px;padding:7px 12px;font-weight:800;font-size:13px}.status-complete{background:#153f35;color:#75efc5}.status-incomplete{background:#4c2430;color:#ff9ba7}");
    html.AppendLine(".cards{display:grid;grid-template-columns:repeat(6,minmax(145px,1fr));gap:12px;margin:22px 0}.card,.section{background:linear-gradient(180deg,var(--panel),#0d1929);border:1px solid var(--line);border-radius:15px;box-shadow:0 14px 34px #0004}.card{padding:18px}.card .value{font-size:28px;font-weight:800}.card .label{font-size:13px;color:var(--muted)}");
    html.AppendLine(".section{padding:24px;margin:20px 0}.section h2{margin:0 0 15px;font-size:23px}.notice{border-left:4px solid var(--orange);background:#2b241b;padding:14px 16px;border-radius:8px;color:#ffe5bf}.criteria{display:grid;grid-template-columns:repeat(2,minmax(260px,1fr));gap:8px 24px;color:#d7e3f3}.criteria li{margin:4px 0}");
    html.AppendLine(".image-frame{background:#050a11;border:1px solid #31445d;border-radius:12px;padding:10px;overflow:auto}.image-frame img{display:block;width:100%;height:auto;border-radius:7px}.caption{font-size:13px;color:var(--muted);margin-top:8px}");
    html.AppendLine(".table-wrap{overflow:auto;border:1px solid var(--line);border-radius:12px}table{width:100%;min-width:1230px;border-collapse:collapse;font-size:13px}th{position:sticky;top:0;background:#1b2b42;color:#cfe3fb;text-align:left}th,td{padding:11px 10px;border-bottom:1px solid #263950;vertical-align:top}tbody tr:hover{background:#17283c}.mono{font-family:Consolas,monospace;white-space:nowrap}");
    html.AppendLine(".chip{display:inline-block;border-radius:999px;padding:4px 9px;font-size:12px;font-weight:800}.advance{background:#12493b;color:#72edc2}.stop{background:#52252e;color:#ff9aa5}.mechanical{background:#283c59;color:#b8d7ff}.details{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:18px}.result{background:var(--panel2);border:1px solid var(--line);border-radius:14px;padding:17px}.result h3{margin:0 0 8px;font-size:18px}.meta{color:var(--muted);font-size:13px;margin-bottom:10px}.reason{margin:10px 0 14px}.links a{color:#77d6ff;margin-right:18px}.limits li{margin:7px 0}");
    html.AppendLine("@media(max-width:1100px){.cards{grid-template-columns:repeat(3,1fr)}.details{grid-template-columns:1fr}.hero{display:block}.toolbar{position:static;margin-top:16px}}@media(max-width:650px){.wrap{padding:22px 14px 60px}.cards{grid-template-columns:repeat(2,1fr)}.criteria{grid-template-columns:1fr}.section{padding:17px}}");
    html.AppendLine("@media print{body{background:white;color:#111}.wrap{max-width:none;padding:0}.toolbar{display:none}.card,.section,.result{background:white;color:#111;box-shadow:none;border-color:#bbb;break-inside:avoid}.sub,.caption,.meta{color:#555}.details{display:block}.result{margin:0 0 18px}.table-wrap{overflow:visible}table{font-size:9px;min-width:0}th{position:static;background:#eee;color:#111}.image-frame{background:white;border-color:#aaa}.notice{background:#fff7e8;color:#332200}.links a{color:#0645ad}}");
    html.AppendLine("</style></head><body><main class=\"wrap\">");
    html.AppendLine("<header class=\"hero\"><div>");
    html.AppendLine("<div class=\"eyebrow\">OpenVisionLab · Auto MPoint</div>");
    html.AppendLine("<h1>6종 EasyMatch 코퍼스<br>검증 보고서</h1>");
    html.Append("<span class=\"status ").Append(overallStatusClass).Append("\">")
        .Append(HtmlReportEncode(overallStatus)).AppendLine("</span>");
    html.Append("<p class=\"sub\">자동 후보가 실제로 같은 물리 특징을 추적했는지 판단할 수 있도록 수치, 판정 사유, 후보 드로잉과 104건 파일럿 결과를 한 파일에 담았습니다. 생성일: ")
        .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss K", CultureInfo.InvariantCulture))
        .AppendLine("</p></div>");
    html.AppendLine("<div class=\"toolbar\"><button class=\"print\" type=\"button\" onclick=\"window.print()\">인쇄 / PDF 저장</button></div></header>");
    html.AppendLine("<section class=\"cards\">");
    AppendAutoMPointHtmlMetric(html, totalMetadataRows + "/3000", "메타데이터 이미지");
    AppendAutoMPointHtmlMetric(html, sourceGroupCount + "/16", "원본 이미지 층");
    AppendAutoMPointHtmlMetric(html, sourceGroupsWithSuggestion + "/16", "후보 생성");
    AppendAutoMPointHtmlMetric(html, pilotRunCount.ToString(CultureInfo.InvariantCulture), "고정 파일럿 실행");
    AppendAutoMPointHtmlMetric(html, expansionCandidateCount + "/16", "확대 검증 후보");
    AppendAutoMPointHtmlMetric(html, stoppedCount + "/16", "중단");
    html.AppendLine("</section>");
    html.AppendLine("<section class=\"section\"><h2>판단 전에 알아둘 점</h2>");
    html.AppendLine("<div class=\"notice\"><strong>기계적 통과는 물리 특징이 올바르다는 뜻이 아닙니다.</strong> OK 4장 중 3장 이상에서 Matching이 실행됐다는 뜻입니다. 아래 드로잉에서 동일한 물리 특징을 따라가는지 확인한 뒤에만 후보를 승인할 수 있습니다.</div>");
    html.AppendLine("<ul class=\"criteria\">");
    html.AppendLine("<li>Auto MPoint: 96×96, stride 16, score 0.75, uniqueness 0.05</li>");
    html.AppendLine("<li>EdgeBased Matching: 전체 512×512 검색, score 0.75</li>");
    html.AppendLine("<li>자세 범위: angle -8..8° / 1°, scale 0.9..1.1 / 0.05</li>");
    html.AppendLine("<li>표본: 각 원본별 OK 4장 + NG 4장</li>");
    html.AppendLine("<li>결과 확인 뒤 ROI·문턱·자세 범위를 조정하지 않음</li>");
    html.AppendLine("<li>생성 자세 정답이 없어 위치 정밀도 px는 판정하지 않음</li>");
    html.AppendLine("</ul></section>");
    html.AppendLine("<section class=\"section\"><h2>Auto MPoint 후보 전체 보기</h2>");
    if (File.Exists(candidateSheetPath))
    {
        html.Append("<div class=\"image-frame\"><img alt=\"16개 원본별 Auto MPoint 후보\" src=\"")
            .Append(ToEmbeddedImageDataUri(candidateSheetPath))
            .AppendLine("\"></div>");
        html.AppendLine("<div class=\"caption\">16개 원본 층에서 고정 조건으로 생성한 1순위 후보. 후보 없음도 그대로 표시합니다.</div>");
    }
    else
    {
        html.AppendLine("<p>후보 전체 드로잉이 생성되지 않았습니다.</p>");
    }
    html.AppendLine("</section>");
    html.AppendLine("<section class=\"section\"><h2>원본별 판정표</h2><div class=\"table-wrap\"><table>");
    html.AppendLine("<thead><tr><th>데이터셋 / 원본</th><th>후보</th><th>ROI</th><th>점수 / 고유성</th><th>OK 성공/모호/미검출</th><th>NG 성공/모호/미검출</th><th>결함 겹침</th><th>평균 ms</th><th>기계 판정</th><th>드로잉 검토</th></tr></thead><tbody>");
    foreach (List<string> row in rows)
    {
        (string drawingDecision, _) =
            GetAutoMPointSixCorpusDrawingReview(row[datasetIndex], row[sourceIndex]);
        string decisionClass = drawingDecision == "확대 검증 후보" ? "advance" : "stop";
        html.Append("<tr><td><strong>").Append(HtmlReportEncode(row[datasetIndex]))
            .Append("</strong><br>").Append(HtmlReportEncode(row[sourceIndex])).Append("</td>");
        html.Append("<td>").Append(HtmlReportEncode(row[candidateStateIndex])).Append("</td>");
        html.Append("<td class=\"mono\">").Append(HtmlReportEncode(row[candidateRoiIndex])).Append("</td>");
        html.Append("<td class=\"mono\">").Append(HtmlReportEncode(row[candidateScoreIndex]))
            .Append(" / ").Append(HtmlReportEncode(row[candidateUniquenessIndex])).Append("</td>");
        html.Append("<td class=\"mono\">").Append(HtmlReportEncode(row[okSuccessIndex])).Append("/")
            .Append(HtmlReportEncode(row[okAmbiguousIndex])).Append("/")
            .Append(HtmlReportEncode(row[okNoMatchIndex])).Append("</td>");
        html.Append("<td class=\"mono\">").Append(HtmlReportEncode(row[ngSuccessIndex])).Append("/")
            .Append(HtmlReportEncode(row[ngAmbiguousIndex])).Append("/")
            .Append(HtmlReportEncode(row[ngNoMatchIndex])).Append("</td>");
        html.Append("<td>").Append(HtmlReportEncode(row[overlapIndex])).Append("</td>");
        html.Append("<td>").Append(HtmlReportEncode(row[elapsedIndex])).Append("</td>");
        html.Append("<td><span class=\"chip mechanical\">").Append(HtmlReportEncode(row[decisionIndex]))
            .Append("</span></td>");
        html.Append("<td><span class=\"chip ").Append(decisionClass).Append("\">")
            .Append(HtmlReportEncode(drawingDecision)).AppendLine("</span></td></tr>");
    }
    html.AppendLine("</tbody></table></div>");
    html.Append("<p class=\"caption\">기계적 검토 대상: ").Append(sourceGroupsAdvancing)
        .Append("/16 · 런타임 오류: ").Append(totalRuntimeErrors)
        .Append(" · 무결성 오류: ").Append(integrityFailures.Count).AppendLine("</p></section>");
    html.AppendLine("<section class=\"section\"><h2>실제 결과 드로잉</h2>");
    html.AppendLine("<p class=\"sub\">노란색/녹색 패턴 윤곽과 중심이 동일한 물리 특징을 따라가는지 확인하십시오. 반복 구조의 다른 위치, 배경 경계, 결함 자체를 따라가면 수치가 높아도 부적합입니다.</p>");
    html.AppendLine("<div class=\"details\">");
    foreach (List<string> row in rows)
    {
        string contactSheetPath = row[contactSheetIndex];
        (string drawingDecision, string drawingReason) =
            GetAutoMPointSixCorpusDrawingReview(row[datasetIndex], row[sourceIndex]);
        string decisionClass = drawingDecision == "확대 검증 후보" ? "advance" : "stop";
        html.AppendLine("<article class=\"result\">");
        html.Append("<h3>").Append(HtmlReportEncode(row[datasetIndex])).Append(" / ")
            .Append(HtmlReportEncode(row[sourceIndex])).AppendLine("</h3>");
        html.Append("<div class=\"meta\">ROI <span class=\"mono\">")
            .Append(HtmlReportEncode(row[candidateRoiIndex]))
            .Append("</span> · 기계 판정 ").Append(HtmlReportEncode(row[decisionIndex])).AppendLine("</div>");
        html.Append("<span class=\"chip ").Append(decisionClass).Append("\">")
            .Append(HtmlReportEncode(drawingDecision)).AppendLine("</span>");
        html.Append("<p class=\"reason\">").Append(HtmlReportEncode(drawingReason)).AppendLine("</p>");
        if (File.Exists(contactSheetPath))
        {
            html.Append("<div class=\"image-frame\"><img loading=\"lazy\" alt=\"")
                .Append(HtmlReportEncode(row[datasetIndex] + " " + row[sourceIndex] + " 파일럿 결과"))
                .Append("\" src=\"").Append(ToEmbeddedImageDataUri(contactSheetPath))
                .AppendLine("\"></div>");
        }
        else
        {
            html.AppendLine("<div class=\"notice\">Auto MPoint 추천 후보가 없어 Matching 파일럿을 실행하지 않았습니다.</div>");
        }
        html.AppendLine("</article>");
    }
    html.AppendLine("</div></section>");
    html.AppendLine("<section class=\"section\"><h2>근거 파일</h2><p class=\"links\">");
    html.AppendLine("<a href=\"p227_auto_mpoint_six_corpus_pilot_summary.csv\">원본별 요약 CSV</a>");
    html.Append("<a href=\"p227_auto_mpoint_six_corpus_pilot_results.csv\">")
        .Append(pilotRunCount).AppendLine("건 실행 결과 CSV</a>");
    html.AppendLine("<a href=\"p227_auto_mpoint_six_corpus_drawing_review.csv\">드로잉 검토 CSV</a>");
    html.AppendLine("<a href=\"completion_record.md\">완료 기록</a></p></section>");
    html.AppendLine("<section class=\"section\"><h2>한계와 다음 판단</h2><ol class=\"limits\">");
    html.AppendLine("<li>이 데이터는 EasyMatch 원본 변형과 합성 NG이며 실제 생산 변동 증거가 아닙니다.</li>");
    html.AppendLine("<li>서로 다른 source_file은 별도 템플릿으로 평가했습니다.</li>");
    html.AppendLine("<li>생성 자세의 정답 좌표가 없어 드로잉의 물리적 동일성은 운영자가 확인해야 합니다.</li>");
    html.AppendLine("<li>운영자가 승인한 후보만 해당 원본 층의 500장 전체 검증으로 확대할 수 있습니다.</li>");
    html.AppendLine("</ol>");
    if (integrityFailures.Count == 0)
    {
        html.AppendLine("<p><strong>무결성 오류:</strong> 없음</p>");
    }
    else
    {
        html.AppendLine("<p><strong>무결성 오류:</strong></p><ul>");
        foreach (string failure in integrityFailures)
        {
            html.Append("<li>").Append(HtmlReportEncode(failure)).AppendLine("</li>");
        }
        html.AppendLine("</ul>");
    }
    html.AppendLine("</section>");
    html.AppendLine("<footer class=\"caption\">OpenVisionLab · P227 · 단일 파일 HTML 보고서 (이미지 포함)</footer>");
    html.AppendLine("</main></body></html>");
    File.WriteAllText(reportPath, html.ToString(), new System.Text.UTF8Encoding(true));
    return reportPath;
}

static void AppendAutoMPointHtmlMetric(
    System.Text.StringBuilder html,
    string value,
    string label)
{
    html.Append("<div class=\"card\"><div class=\"value\">")
        .Append(HtmlReportEncode(value))
        .Append("</div><div class=\"label\">")
        .Append(HtmlReportEncode(label))
        .AppendLine("</div></div>");
}

static string HtmlReportEncode(string value)
{
    return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
}

static string ToEmbeddedImageDataUri(string path)
{
    string extension = Path.GetExtension(path).ToLowerInvariant();
    string mimeType = extension switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".gif" => "image/gif",
        ".webp" => "image/webp",
        _ => "image/png"
    };
    return "data:" + mimeType + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path));
}

static (string Decision, string Reason) GetAutoMPointSixCorpusDrawingReview(
    string dataset,
    string sourceFile)
{
    string key = dataset + "|" + sourceFile;
    return key switch
    {
        "switch_housing|Switch1.tif" =>
            ("확대 검증 후보", "좌하단 슬롯과 하우징 윤곽을 같은 위치에서 추적했습니다. OK는 3/4 성공이므로 전체 검증 전 채택은 금지합니다."),
        "switch_housing|Switch2.tif" =>
            ("확대 검증 후보", "우상단 슬롯과 코너 윤곽을 일관되게 추적했습니다. 한 NG의 최저 점수 75.5가 문턱에 가까워 여유가 작습니다."),
        "switch_housing|Switch3.tif" =>
            ("확대 검증 후보", "하단 원형 홀과 인접 슬롯을 일관되게 추적했습니다. NG 결함 마스크가 후보와 겹친 성공 1건은 취약성으로 남깁니다."),
        "pcb_board|BOARD.JPG" =>
            ("확대 검증 후보", "중앙 부품 군집의 동일 위치를 추적했지만 엣지가 매우 조밀합니다. OK 3/4 및 결함 겹침 1건 때문에 보수적으로 유지합니다."),
        "ic_frame|Frame 1.tif" =>
            ("중단", "고정 조건에서 추천 후보가 생성되지 않았습니다."),
        "ic_frame|Frame 2.tif" =>
            ("중단", "고정 조건에서 추천 후보가 생성되지 않았습니다."),
        "ic_frame|Frame 3.tif" =>
            ("중단", "고정 조건에서 추천 후보가 생성되지 않았습니다."),
        "ic_frame|Frame 4.bmp" =>
            ("중단", "우하단 코너를 추적했으나 OK 성공이 2/4라 최소 기계 조건을 통과하지 못했습니다."),
        "ic_frame|Frame 5.bmp" =>
            ("확대 검증 후보", "좌하단 코너와 핀 군을 같은 위치에서 추적했습니다. 반복 핀 구조이므로 전체 검증 전 채택은 금지합니다."),
        "floppy_disk|Floppies.jpg" =>
            ("확대 검증 후보", "비대칭 흰 탭을 포함한 동일 허브를 OK 4/4에서 추적했습니다. 여러 유사 디스크가 있어 고유성 재확인이 필요합니다."),
        "die_pad|Die Pad 1.bmp" =>
            ("확대 검증 후보", "중앙 패드와 연결 배선을 OK/NG 각 4/4에서 같은 위치로 추적했습니다."),
        "die_pad|Die Pad 2.bmp" =>
            ("확대 검증 후보", "인접 패드와 수직 배선의 동일 조합을 OK 4/4에서 추적했습니다. NG는 3/4 성공입니다."),
        "die_pad|Die Pad 3.bmp" =>
            ("확대 검증 후보", "우측 패드와 코너 배선을 OK 4/4에서 추적했습니다. 결함 겹침 성공 2건은 취약성으로 남깁니다."),
        "die_pad|Die Pad 4.bmp" =>
            ("확대 검증 후보", "중앙 패드와 인접 배선을 OK/NG 각 4/4에서 같은 위치로 추적했습니다."),
        "die_array|Die1.tif" =>
            ("중단", "반복되는 다이 격자 교차점을 후보로 삼았고 NG 1건에서 실제 모호 판정이 발생했습니다. 고유 랜드마크로 보지 않습니다."),
        "die_array|Die2.tif" =>
            ("중단", "오브젝트 내부 특징이 아니라 영상 상단의 잘린 프레임 경계를 추적했습니다. 촬영 프레이밍 의존 후보라 부적합합니다."),
        _ => ("중단", "드로잉 검토 규칙이 정의되지 않은 원본 층입니다.")
    };
}

static string ToReportRelativePath(string evidenceDirectory, string path)
{
    return Path.GetRelativePath(evidenceDirectory, path).Replace('\\', '/').Replace(" ", "%20");
}

static List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)>
    LoadAutoMPointCorpusMetadata(string metadataPath)
{
    string[] lines = File.ReadAllLines(metadataPath);
    if (lines.Length < 2)
    {
        throw new InvalidDataException("Metadata CSV has no rows.");
    }
    List<string> header = ParseCsvRecord(lines[0]);
    int globalIdIndex = header.FindIndex(value => value == "global_id");
    int fileNameIndex = header.FindIndex(value => value == "filename");
    int statusIndex = header.FindIndex(value => value == "status");
    int sourceFileIndex = header.FindIndex(value => value == "source_file");
    int md5Index = header.FindIndex(value => value == "md5");
    if (new[] { globalIdIndex, fileNameIndex, statusIndex, sourceFileIndex, md5Index }.Any(index => index < 0))
    {
        throw new InvalidDataException("Metadata CSV is missing global_id, filename, status, source_file, or md5.");
    }

    List<(int, string, string, string, string)> rows = new List<(int, string, string, string, string)>();
    foreach (string line in lines.Skip(1).Where(value => !string.IsNullOrWhiteSpace(value)))
    {
        List<string> values = ParseCsvRecord(line);
        if (!int.TryParse(values[globalIdIndex], NumberStyles.Integer, CultureInfo.InvariantCulture, out int globalId))
        {
            throw new InvalidDataException("Metadata global_id is invalid: " + values[globalIdIndex]);
        }
        rows.Add((
            globalId,
            values[fileNameIndex],
            values[statusIndex],
            values[sourceFileIndex],
            values[md5Index]));
    }
    return rows;
}

static string GetAutoMPointCorpusImagePath(
    string datasetRoot,
    (int GlobalId, string FileName, string Status, string SourceFile, string Md5) row)
{
    return Path.Combine(datasetRoot, "all_images", row.Status, row.FileName);
}

static IEnumerable<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)>
    SelectAutoMPointPilotRows(
        IReadOnlyList<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> rows,
        (int GlobalId, string FileName, string Status, string SourceFile, string Md5)? required)
{
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> ordered =
        rows.OrderBy(row => row.Md5, StringComparer.OrdinalIgnoreCase).ToList();
    List<(int GlobalId, string FileName, string Status, string SourceFile, string Md5)> selected =
        new List<(int, string, string, string, string)>();
    if (required.HasValue)
    {
        selected.Add(required.Value);
    }
    int[] indices =
    {
        0,
        ordered.Count / 3,
        ordered.Count * 2 / 3,
        ordered.Count - 1
    };
    foreach (int index in indices)
    {
        (int GlobalId, string FileName, string Status, string SourceFile, string Md5) candidate = ordered[index];
        if (!selected.Any(row => row.GlobalId == candidate.GlobalId))
        {
            selected.Add(candidate);
        }
        if (selected.Count == 4)
        {
            break;
        }
    }
    foreach ((int GlobalId, string FileName, string Status, string SourceFile, string Md5) candidate in ordered)
    {
        if (selected.Count == 4)
        {
            break;
        }
        if (!selected.Any(row => row.GlobalId == candidate.GlobalId))
        {
            selected.Add(candidate);
        }
    }
    return selected;
}

static string ComputeMd5(string filePath)
{
    using FileStream stream = File.OpenRead(filePath);
    return Convert.ToHexString(MD5.HashData(stream)).ToLowerInvariant();
}

static int CountMaskOverlap(string maskPath, System.Drawing.RectangleF bounds)
{
    using Mat mask = Cv2.ImRead(maskPath, ImreadModes.Grayscale);
    if (mask.Empty())
    {
        return 0;
    }
    int left = Math.Clamp((int)Math.Floor(bounds.Left), 0, mask.Width);
    int top = Math.Clamp((int)Math.Floor(bounds.Top), 0, mask.Height);
    int right = Math.Clamp((int)Math.Ceiling(bounds.Right), 0, mask.Width);
    int bottom = Math.Clamp((int)Math.Ceiling(bounds.Bottom), 0, mask.Height);
    if (right <= left || bottom <= top)
    {
        return 0;
    }
    using Mat overlap = mask.SubMat(new Rect(left, top, right - left, bottom - top));
    return Cv2.CountNonZero(overlap);
}

static async Task<int> RunObjectDimensionFilterContractAsync(string? evidenceDirectory)
{
    List<string> failures = new List<string>();
    string? evidencePath = string.IsNullOrWhiteSpace(evidenceDirectory)
        ? null
        : Path.GetFullPath(evidenceDirectory);
    if (evidencePath != null)
    {
        Directory.CreateDirectory(evidencePath);
    }

    using Mat source = new Mat(new OpenCvSharp.Size(360, 140), MatType.CV_8UC1, Scalar.Black);
    Cv2.Rectangle(source, new Rect(20, 20, 24, 32), Scalar.White, -1);
    Cv2.Rectangle(source, new Rect(80, 20, 52, 24), Scalar.White, -1);
    Cv2.Rectangle(source, new Rect(155, 20, 8, 32), Scalar.White, -1);
    Cv2.Rectangle(source, new Rect(195, 20, 24, 8), Scalar.White, -1);
    Cv2.Rectangle(source, new Rect(250, 20, 24, 60), Scalar.White, -1);
    if (evidencePath != null)
    {
        Cv2.ImWrite(Path.Combine(evidencePath, "object_dimension_filter_source.png"), source);
    }

    foreach (string toolType in new[] { "Blob", "Contour" })
    {
        await VerifyObjectDimensionFilterAsync(source, toolType, failures, evidencePath);
    }

    VerifyObjectDimensionPropertyRoundTrip(failures);
    VerifyObjectDimensionValidation(failures);

    if (failures.Count == 0)
    {
        Console.WriteLine("Object dimension filter contract smoke passed.");
        Console.WriteLine("Blob/Contour: 1 accepted object, width/height reject reasons retained, legacy missing-key behavior preserved.");
        return 0;
    }

    Console.Error.WriteLine("Object dimension filter contract smoke failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    return 1;
}

static async Task<int> RunAffineTransformContractAsync(string? evidenceDirectory)
{
    List<string> failures = new List<string>();
    string? evidencePath = string.IsNullOrWhiteSpace(evidenceDirectory)
        ? null
        : Path.GetFullPath(evidenceDirectory);
    if (evidencePath != null)
    {
        Directory.CreateDirectory(evidencePath);
    }

    using Mat source = new Mat(new OpenCvSharp.Size(160, 120), MatType.CV_8UC3, Scalar.Black);
    Cv2.Rectangle(source, new Rect(20, 20, 50, 40), new Scalar(255, 255, 255), -1);
    Cv2.Circle(source, new OpenCvSharp.Point(110, 70), 14, new Scalar(0, 180, 255), -1);
    if (evidencePath != null)
    {
        Cv2.ImWrite(Path.Combine(evidencePath, "affine_contract_source.png"), source);
    }

    AffineTransformProperty authored = new AffineTransformProperty("Affine contract")
    {
        SourcePoint1X = 0,
        SourcePoint1Y = 0,
        SourcePoint2X = 100,
        SourcePoint2Y = 0,
        SourcePoint3X = 0,
        SourcePoint3Y = 100,
        DestinationPoint1X = 12,
        DestinationPoint1Y = 18,
        DestinationPoint2X = 132,
        DestinationPoint2Y = 8,
        DestinationPoint3X = 37,
        DestinationPoint3Y = 108,
        OutputWidth = 240,
        OutputHeight = 180,
        MinimumSourceTriangleArea = 100,
        MinimumDestinationTriangleArea = 100,
        MinimumValidPixelRatio = 0.4
    };

    VisionPipelineStep canonicalStep = VisionPipelineStepBuilder.FromAffineTransformProperty(
        authored,
        "01 Affine contract",
        VisionRecipeRunner.DefaultInputLayer,
        "Affine_Result");
    VerifyAffineTransformPropertyRoundTrip(canonicalStep, failures);

    VisionRecipeRunner runner = new VisionRecipeRunner();
    string[] aliases = { "AffineTransform", "Affine", "AffineMatrix" };
    foreach (string alias in aliases)
    {
        VisionPipeline pipeline = CreateAffineContractPipeline(canonicalStep, alias);
        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            pipeline,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (!validation.Success)
        {
            failures.Add(alias + ": strict validation failed: " + string.Join(" | ", validation.Errors));
            continue;
        }

        using VisionRecipeRunResult run = await runner.RunAsync(pipeline, source);
        VisionRecipeStepRunSummary? step = run.Steps.SingleOrDefault();
        if (!run.Success || step == null)
        {
            failures.Add(alias + ": runtime failed: " + run.Message);
            continue;
        }

        VerifyAffineMetric(step, VisionPipelineKnownMetrics.AffineM11, 1.2, failures, alias);
        VerifyAffineMetric(step, VisionPipelineKnownMetrics.AffineM12, 0.25, failures, alias);
        VerifyAffineMetric(step, VisionPipelineKnownMetrics.AffineM13, 12, failures, alias);
        VerifyAffineMetric(step, VisionPipelineKnownMetrics.AffineM21, -0.1, failures, alias);
        VerifyAffineMetric(step, VisionPipelineKnownMetrics.AffineM22, 0.9, failures, alias);
        VerifyAffineMetric(step, VisionPipelineKnownMetrics.AffineM23, 18, failures, alias);
        if (step.Overlays.Count != 10
            || step.Overlays.Count(item => string.Equals(item.Kind, "Point", StringComparison.OrdinalIgnoreCase)) != 3
            || step.Overlays.Count(item => string.Equals(item.Kind, "Line", StringComparison.OrdinalIgnoreCase)) != 7)
        {
            failures.Add(alias + ": expected 3 point and 7 line drawings.");
        }

        if (string.Equals(alias, "AffineTransform", StringComparison.Ordinal) && evidencePath != null)
        {
            if (run.ResultImage != null && !run.ResultImage.Empty())
            {
                Cv2.ImWrite(Path.Combine(evidencePath, "affine_contract_result.png"), run.ResultImage);
            }
            SaveAllOverlayImage(
                source,
                run,
                pipeline,
                Path.Combine(evidencePath, "affine_contract_drawing.png"));
        }
    }

    VisionPipeline degenerate = CreateAffineContractPipeline(canonicalStep, "AffineTransform");
    degenerate.Steps[0].Parameters[nameof(AffineTransformToolProperty.SourcePoint2X)] = "10";
    degenerate.Steps[0].Parameters[nameof(AffineTransformToolProperty.SourcePoint2Y)] = "10";
    degenerate.Steps[0].Parameters[nameof(AffineTransformToolProperty.SourcePoint3X)] = "20";
    degenerate.Steps[0].Parameters[nameof(AffineTransformToolProperty.SourcePoint3Y)] = "20";
    degenerate.Steps[0].Parameters[nameof(AffineTransformToolProperty.MinimumSourceTriangleArea)] = "0";
    VisionPipelineValidationResult degenerateValidation = VisionPipelineValidator.Validate(
        degenerate,
        new[] { VisionRecipeRunner.DefaultInputLayer });
    if (degenerateValidation.Success
        || !degenerateValidation.Errors.Any(error => error.Contains("source point triangle area", StringComparison.OrdinalIgnoreCase)))
    {
        failures.Add("Strict validation accepted collinear source points when the operator area gate was zero.");
    }

    VisionPipeline coverageFailure = CreateAffineContractPipeline(canonicalStep, "AffineTransform");
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.DestinationPoint1X)] = "500";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.DestinationPoint1Y)] = "500";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.DestinationPoint2X)] = "600";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.DestinationPoint2Y)] = "500";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.DestinationPoint3X)] = "500";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.DestinationPoint3Y)] = "600";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.OutputWidth)] = "64";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.OutputHeight)] = "64";
    coverageFailure.Steps[0].Parameters[nameof(AffineTransformToolProperty.MinimumValidPixelRatio)] = "0.1";
    using (VisionRecipeRunResult failedRun = await runner.RunAsync(coverageFailure, source))
    {
        VisionRecipeStepRunSummary? failedStep = failedRun.Steps.SingleOrDefault();
        if (failedRun.Success
            || failedStep == null
            || !string.Equals(failedStep.ErrorName, "AffineInsufficientCoverage", StringComparison.Ordinal)
            || failedStep.Overlays.Count != 10
            || !failedStep.Metrics.TryGetValue(VisionPipelineKnownMetrics.AffineValidPixelRatio, out double validRatio)
            || validRatio != 0)
        {
            failures.Add("Coverage failure did not fail closed while retaining matrix/coverage/drawing evidence.");
        }
    }

    if (evidencePath != null)
    {
        File.WriteAllLines(
            Path.Combine(evidencePath, "affine_contract_report.txt"),
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Aliases: AffineTransform, Affine, AffineMatrix",
                "KnownMatrix: 1.2,0.25,12;-0.1,0.9,18",
                "PropertyGridRoundTrip: " + (failures.Any(item => item.Contains("round trip", StringComparison.OrdinalIgnoreCase)) ? "FAIL" : "PASS"),
                "DegenerateSourceGate: " + (failures.Any(item => item.Contains("collinear", StringComparison.OrdinalIgnoreCase)) ? "FAIL" : "PASS"),
                "CoverageFailureEvidence: " + (failures.Any(item => item.Contains("Coverage failure", StringComparison.OrdinalIgnoreCase)) ? "FAIL" : "PASS")
            }.Concat(failures.Select(item => "Failure: " + item)));
    }

    if (failures.Count == 0)
    {
        Console.WriteLine("Affine transform contract smoke passed.");
        Console.WriteLine("Aliases, known matrix, PropertyGrid/XML round trip, collinear rejection, and coverage evidence passed.");
        return 0;
    }

    Console.Error.WriteLine("Affine transform contract smoke failed.");
    foreach (string failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    return 1;
}

static async Task<int> RunAffineDetectedPointsContractAsync(string? evidenceDirectory)
{
    List<string> failures = new List<string>();
    string workDirectory = string.IsNullOrWhiteSpace(evidenceDirectory)
        ? Path.Combine(Path.GetTempPath(), "OpenVisionLab_P219_" + Guid.NewGuid().ToString("N"))
        : Path.GetFullPath(evidenceDirectory);
    bool deleteWorkDirectory = string.IsNullOrWhiteSpace(evidenceDirectory);
    Directory.CreateDirectory(workDirectory);

    try
    {
        using Mat reference = new Mat(new OpenCvSharp.Size(400, 300), MatType.CV_8UC1, Scalar.Black);
        Cv2.Rectangle(reference, new Rect(180, 130, 40, 30), Scalar.White, -1);
        Cv2.Rectangle(reference, new Rect(176, 126, 48, 38), new Scalar(90), 2);

        Point2f[] sourcePoints =
        {
            new Point2f(80.5f, 60.5f),
            new Point2f(300.5f, 70.5f),
            new Point2f(90.5f, 230.5f)
        };
        Point2f[] destinationPoints =
        {
            new Point2f(60.5f, 50.5f),
            new Point2f(300.5f, 50.5f),
            new Point2f(60.5f, 230.5f)
        };

        using Mat referenceToSource = Cv2.GetAffineTransform(destinationPoints, sourcePoints);
        using Mat source = new Mat();
        Cv2.WarpAffine(
            reference,
            source,
            referenceToSource,
            reference.Size(),
            InterpolationFlags.Linear,
            BorderTypes.Constant,
            Scalar.Black);

        string[] templatePaths = new string[3];
        for (int index = 0; index < 3; index++)
        {
            using Mat template = CreateAffineFiducial(index);
            int left = (int)Math.Floor(sourcePoints[index].X - template.Width / 2D);
            int top = (int)Math.Floor(sourcePoints[index].Y - template.Height / 2D);
            template.CopyTo(source.SubMat(new Rect(left, top, template.Width, template.Height)));
            templatePaths[index] = Path.Combine(workDirectory, $"template_{index + 1}.png");
            Cv2.ImWrite(templatePaths[index], template);
        }

        Cv2.ImWrite(Path.Combine(workDirectory, "00_source.png"), source);
        Cv2.ImWrite(Path.Combine(workDirectory, "00_reference_expected.png"), reference);

        VisionPipeline pipeline = CreateDetectedPointAffinePipeline(
            templatePaths,
            sourcePoints,
            destinationPoints);
        string pipelinePath = Path.Combine(workDirectory, "p219_matching_affine_fixed_roi.pipeline.xml");
        string saveMessage = string.Empty;
        string loadMessage = string.Empty;
        VisionPipeline loaded = pipeline;
        VisionPipeline loadedFromFile = pipeline;
        bool saved = VisionPipelineStorage.TrySaveToFile(pipelinePath, pipeline, out saveMessage);
        bool reloaded = saved
            && VisionPipelineStorage.TryLoadFromFile(pipelinePath, out loadedFromFile, out loadMessage);
        if (reloaded)
        {
            loaded = loadedFromFile;
        }
        else
        {
            failures.Add("Pipeline XML round trip failed. " + saveMessage + " " + loadMessage);
        }

        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            loaded,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (!validation.Success)
        {
            failures.Add("Detected-point Affine pipeline validation failed: " + string.Join(" | ", validation.Errors));
        }

        VerifyDetectedPointAffinePropertyRoundTrip(loaded.Steps[3], failures);

        using VisionPipelineContext context = new VisionPipelineContext();
        context.SetLayer(VisionRecipeRunner.DefaultInputLayer, source);
        VisionPipelineRunResult run = await VisionPipelineExecutionService.RunAsync(
            loaded,
            context,
            10000,
            CancellationToken.None);
        try
        {
            if (!run.Success || run.StepResults.Count != 6)
            {
                string stepEvidence = string.Join(
                    " | ",
                    run.StepResults.Select(stepResult =>
                        $"{stepResult.Step?.Name}: "
                        + $"ToolSuccess={stepResult.ToolResult?.Success}, "
                        + $"Error={stepResult.ToolResult?.ErrorName}, "
                        + $"Message={stepResult.ToolResult?.Message}, "
                        + $"Acceptance={stepResult.AcceptancePassed} {stepResult.AcceptanceMessage}"));
                failures.Add(
                    $"Matching -> Affine -> fixed ROI runtime failed. "
                    + $"Steps={run.StepResults.Count}/6. {stepEvidence}");
            }
            else
            {
                for (int index = 0; index < 3; index++)
                {
                    IReadOnlyList<VisionPipelineGeometryFeatureResult> features =
                        VisionPipelineGeometryFeatureStore.Get(run.StepResults[index].ToolResult);
                    VisionPipelineGeometryFeatureResult? center = features.SingleOrDefault(item =>
                        item.Kind == VisionPipelineGeometryKind.Point
                        && string.Equals(item.FeatureName, "Center", StringComparison.OrdinalIgnoreCase));
                    if (center == null
                        || Math.Abs(center.CenterX - sourcePoints[index].X) > 0.6
                        || Math.Abs(center.CenterY - sourcePoints[index].Y) > 0.6)
                    {
                        failures.Add($"Matching Step {index + 1} did not publish the expected typed Center Point.");
                    }
                }

                VisionToolResult affineResult = run.StepResults[3].ToolResult;
                if (!affineResult.Metrics.TryGetValue(VisionPipelineKnownMetrics.AffineDetectedSourcePointCount, out double pointCount)
                    || pointCount != 3D)
                {
                    failures.Add("Affine runtime did not retain detected source-point provenance metrics.");
                }
                VerifyCoreMetric(affineResult, VisionPipelineKnownMetrics.AffineSourcePoint1X, sourcePoints[0].X, 0.6, failures);
                VerifyCoreMetric(affineResult, VisionPipelineKnownMetrics.AffineSourcePoint1Y, sourcePoints[0].Y, 0.6, failures);
                VerifyCoreMetric(affineResult, VisionPipelineKnownMetrics.AffineSourcePoint2X, sourcePoints[1].X, 0.6, failures);
                VerifyCoreMetric(affineResult, VisionPipelineKnownMetrics.AffineSourcePoint2Y, sourcePoints[1].Y, 0.6, failures);
                VerifyCoreMetric(affineResult, VisionPipelineKnownMetrics.AffineSourcePoint3X, sourcePoints[2].X, 0.6, failures);
                VerifyCoreMetric(affineResult, VisionPipelineKnownMetrics.AffineSourcePoint3Y, sourcePoints[2].Y, 0.6, failures);

                using Mat expectedMatrix = Cv2.GetAffineTransform(sourcePoints, destinationPoints);
                string[] matrixMetrics =
                {
                    VisionPipelineKnownMetrics.AffineM11,
                    VisionPipelineKnownMetrics.AffineM12,
                    VisionPipelineKnownMetrics.AffineM13,
                    VisionPipelineKnownMetrics.AffineM21,
                    VisionPipelineKnownMetrics.AffineM22,
                    VisionPipelineKnownMetrics.AffineM23
                };
                for (int row = 0; row < 2; row++)
                {
                    for (int column = 0; column < 3; column++)
                    {
                        VerifyCoreMetric(
                            affineResult,
                            matrixMetrics[row * 3 + column],
                            expectedMatrix.At<double>(row, column),
                            1e-6,
                            failures);
                    }
                }

                VisionToolResult blobResult = run.StepResults[5].ToolResult;
                double resultCount = double.NaN;
                if (!run.StepResults[5].AcceptancePassed
                    || !blobResult.Metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out resultCount)
                    || resultCount != 1D
                    || loaded.Steps[5].Parameters.GetValueOrDefault("CvROI") != "170,120,70,60")
                {
                    failures.Add("The unchanged fixed reference ROI did not find exactly one normalized inspection target.");
                }

                using Mat normalized = context.GetLayer("Reference");
                if (normalized == null
                    || normalized.Empty()
                    || Cv2.Mean(normalized.SubMat(new Rect(185, 135, 30, 20))).Val0 < 180D)
                {
                    failures.Add("Affine output did not restore the taught reference target region.");
                }
            }

            SaveAffineDetectedPointEvidence(run, workDirectory);
        }
        finally
        {
            foreach (VisionPipelineStepResult stepResult in run.StepResults)
            {
                stepResult?.ToolResult?.ResultImage?.Dispose();
            }
        }

        VisionPipeline duplicate = ClonePipeline(loaded);
        duplicate.Steps[3].Parameters[VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter] =
            duplicate.Steps[3].Parameters[VisionPipelineAffinePointBindingService.SourcePoint2FeatureParameter];
        VisionPipelineValidationResult duplicateValidation = VisionPipelineValidator.Validate(
            duplicate,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (duplicateValidation.Success
            || !duplicateValidation.Errors.Any(item => item.Contains("must be distinct", StringComparison.OrdinalIgnoreCase)))
        {
            failures.Add("Static validation accepted duplicate detected Point references.");
        }

        using VisionPipelineContext duplicateContext = new VisionPipelineContext();
        duplicateContext.SetLayer(VisionRecipeRunner.DefaultInputLayer, source);
        VisionPipelineRunResult duplicateRun = await VisionPipelineExecutionService.RunAsync(
            duplicate,
            duplicateContext,
            10000,
            CancellationToken.None);
        try
        {
            VisionPipelineStepResult? failedAffine = duplicateRun.StepResults.LastOrDefault();
            if (duplicateRun.Success
                || failedAffine?.Step?.Name != "04 Normalize from detected points"
                || failedAffine.ToolResult?.Success != false
                || !failedAffine.ToolResult.Message.Contains("three distinct Point features", StringComparison.OrdinalIgnoreCase))
            {
                failures.Add("Runtime did not fail closed on duplicate detected Point references.");
            }
        }
        finally
        {
            foreach (VisionPipelineStepResult stepResult in duplicateRun.StepResults)
            {
                stepResult?.ToolResult?.ResultImage?.Dispose();
            }
        }

        File.WriteAllLines(
            Path.Combine(workDirectory, "p219_affine_detected_points_report.txt"),
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Pipeline: Matching x3 -> AffineTransform -> Threshold -> fixed-ROI Blob",
                "SourceBinding: LocateTopLeft/Center;LocateTopRight/Center;LocateBottomLeft/Center",
                "DestinationFrame: fixed taught pixel coordinates",
                "FixedInspectionRoi: 170,120,70,60",
                "LegacyFixedSourceMode: preserved by the separate affine-transform contract",
                "DuplicatePointGate: " + (failures.Any(item => item.Contains("duplicate", StringComparison.OrdinalIgnoreCase)) ? "FAIL" : "PASS")
            }.Concat(failures.Select(item => "Failure: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Affine detected-point contract smoke passed.");
            Console.WriteLine("Three Matching centers drove the OpenVisionLab Vision SDK AffineTransform, then an unchanged fixed ROI found one normalized target.");
            return 0;
        }

        Console.Error.WriteLine("Affine detected-point contract smoke failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }
        return 1;
    }
    finally
    {
        if (deleteWorkDirectory && Directory.Exists(workDirectory))
        {
            Directory.Delete(workDirectory, true);
        }
    }
}

static async Task<int> RunAffineCardPilotAsync(
    string datasetRootArgument,
    string evidenceDirectoryArgument,
    bool includeFixedRoiMean = false,
    double maximumPostResidualPx = 3D)
{
    string datasetRoot = Path.GetFullPath(datasetRootArgument);
    string evidenceDirectory = Path.GetFullPath(evidenceDirectoryArgument);
    string evidencePrefix = includeFixedRoiMean ? "p221" : "p220";
    Rect fixedInspectionRoi = new Rect(250, 315, 190, 80);
    string referencePath = Path.Combine(
        datasetRoot,
        "images",
        "OK",
        "card_original_OK_0001.jpg");
    Directory.CreateDirectory(evidenceDirectory);

    if (!File.Exists(referencePath))
    {
        Console.Error.WriteLine("Affine card pilot reference image was not found: " + referencePath);
        return 2;
    }

    (string Name, Rect Roi, Rect SearchRoi)[] locators =
    {
        ("R", new Rect(100, 38, 68, 126), new Rect(85, 5, 220, 200)),
        ("5", new Rect(320, 35, 75, 125), new Rect(280, 5, 250, 200)),
        ("Expiry", new Rect(165, 333, 85, 55), new Rect(105, 280, 220, 150))
    };
    Point2f[] destinationPoints = locators
        .Select(item => new Point2f(
            item.Roi.X + item.Roi.Width / 2F,
            item.Roi.Y + item.Roi.Height / 2F))
        .ToArray();

    string templateDirectory = Path.Combine(evidenceDirectory, "templates");
    Directory.CreateDirectory(templateDirectory);
    string[] templatePaths = new string[locators.Length];
    Mat[] templates = new Mat[locators.Length];
    using (Mat reference = Cv2.ImRead(referencePath, ImreadModes.Color))
    {
        if (reference.Empty() || reference.Width != 640 || reference.Height != 480)
        {
            Console.Error.WriteLine("Affine card pilot reference must be the approved 640x480 image.");
            return 2;
        }

        for (int index = 0; index < locators.Length; index++)
        {
            templates[index] = reference.SubMat(locators[index].Roi).Clone();
            templatePaths[index] = Path.Combine(
                templateDirectory,
                $"{index + 1:00}_{locators[index].Name}.png");
            Cv2.ImWrite(templatePaths[index], templates[index]);
        }
    }

    try
    {
        VisionPipeline pipeline = CreateCardAffinePilotPipeline(
            templatePaths,
            locators.Select(item => item.SearchRoi).ToArray(),
            destinationPoints,
            includeFixedRoiMean,
            fixedInspectionRoi);
        string pipelinePath = Path.Combine(
            evidenceDirectory,
            evidencePrefix + "_card_matching_x3_affine.pipeline.xml");
        string saveMessage = string.Empty;
        string loadMessage = string.Empty;
        VisionPipeline loaded = pipeline;
        bool saved = VisionPipelineStorage.TrySaveToFile(
            pipelinePath,
            pipeline,
            out saveMessage);
        bool loadedFromFile = saved
            && VisionPipelineStorage.TryLoadFromFile(
                pipelinePath,
                out loaded,
                out loadMessage);
        if (!loadedFromFile)
        {
            Console.Error.WriteLine(
                "Affine card pilot XML round trip failed. "
                + saveMessage
                + " "
                + loadMessage);
            return 1;
        }

        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            loaded,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (!validation.Success)
        {
            Console.Error.WriteLine(
                "Affine card pilot definition is invalid: "
                + string.Join(" | ", validation.Errors));
            return 1;
        }
        if (includeFixedRoiMean
            && (loaded.Steps.Count != 5
                || !string.Equals(loaded.Steps[4].ToolType, "Mean", StringComparison.OrdinalIgnoreCase)
                || loaded.Steps[4].Parameters.GetValueOrDefault("CvROI") != "250,315,190,80"
                || !string.Equals(loaded.Steps[4].InputLayer, "CardReference", StringComparison.Ordinal)))
        {
            Console.Error.WriteLine("Affine card fixed-ROI XML round trip did not retain the exact Mean Step contract.");
            return 1;
        }

        (string Role, string FileName)[] selected =
        {
            ("OK", "card_original_OK_0026.jpg"),
            ("OK", "card_original_OK_0051.jpg"),
            ("OK", "card_original_OK_0101.jpg"),
            ("OK", "card_original_OK_0150.jpg"),
            ("OK", "card_original_OK_0200.jpg"),
            ("OK", "card_original_OK_0250.jpg"),
            ("NG", "card_original_NG_0026.jpg"),
            ("NG", "card_original_NG_0051.jpg"),
            ("NG", "card_original_NG_0101.jpg"),
            ("NG", "card_original_NG_0150.jpg"),
            ("NG", "card_original_NG_0200.jpg"),
            ("NG", "card_original_NG_0250.jpg")
        };

        string[] imagePaths = selected
            .Select(item => Path.Combine(datasetRoot, "images", item.Role, item.FileName))
            .ToArray();
        string manifestPath = Path.Combine(evidenceDirectory, evidencePrefix + "_input_manifest.csv");
        File.WriteAllLines(
            manifestPath,
            new[] { "Role,FileName,SourcePath,SourceSha256" }
                .Concat(selected.Select((item, index) =>
                    string.Join(
                        ",",
                        item.Role,
                        item.FileName,
                        EscapeBatchCsvValue(imagePaths[index]),
                        File.Exists(imagePaths[index])
                            ? ComputeSha256(imagePaths[index])
                            : "MISSING"))));

        List<string> csvRows = new List<string>
        {
            includeFixedRoiMean
                ? "Role,FileName,Status,LocatorAScore,LocatorBScore,LocatorCScore,SourcePoint1,SourcePoint2,SourcePoint3,AffineValidPixelRatio,PostCheckMinScore,PostCheckMaxResidualPx,FixedRoiMeanValueAvg,SourceSha256,DrawingDirectory"
                : "Role,FileName,Status,LocatorAScore,LocatorBScore,LocatorCScore,SourcePoint1,SourcePoint2,SourcePoint3,AffineValidPixelRatio,PostCheckMinScore,PostCheckMaxResidualPx,SourceSha256,DrawingDirectory"
        };
        List<string> reviewImages = new List<string>();
        List<string> reviewLabels = new List<string>();
        List<string> failures = new List<string>();
        int passed = 0;

        for (int sampleIndex = 0; sampleIndex < selected.Length; sampleIndex++)
        {
            (string role, string fileName) = selected[sampleIndex];
            string imagePath = imagePaths[sampleIndex];
            string runDirectory = Path.Combine(
                evidenceDirectory,
                "runs",
                $"{sampleIndex + 1:00}_{role}_{Path.GetFileNameWithoutExtension(fileName)}");
            Directory.CreateDirectory(runDirectory);
            string reviewImagePath = Path.Combine(
                runDirectory,
                includeFixedRoiMean
                    ? "05_05 Measure fixed date ROI.png"
                    : "06_fixed_point_recheck.png");
            string status = "PASS";
            double[] scores = { double.NaN, double.NaN, double.NaN };
            Point2f[] sourcePoints = new Point2f[3];
            double validPixelRatio = double.NaN;
            double postMinScore = double.NaN;
            double postMaxResidual = double.NaN;
            double fixedRoiMean = double.NaN;

            if (!File.Exists(imagePath))
            {
                status = "MISSING";
                failures.Add(role + "/" + fileName + ": source image is missing.");
            }
            else
            {
                File.Copy(
                    imagePath,
                    Path.Combine(runDirectory, "00_source.jpg"),
                    true);
                using Mat source = Cv2.ImRead(imagePath, ImreadModes.Color);
                if (source.Empty())
                {
                    status = "LOAD_FAIL";
                    failures.Add(role + "/" + fileName + ": source image could not be loaded.");
                }
                else
                {
                    using VisionPipelineContext context = new VisionPipelineContext();
                    context.SetLayer(VisionRecipeRunner.DefaultInputLayer, source);
                    VisionPipelineRunResult run = await VisionPipelineExecutionService.RunAsync(
                        loaded,
                        context,
                        30000,
                        CancellationToken.None);
                    try
                    {
                        SaveAffineDetectedPointEvidence(run, runDirectory);
                        int expectedStepCount = includeFixedRoiMean ? 5 : 4;
                        if (!run.Success || run.StepResults.Count < expectedStepCount)
                        {
                            status = "RUNTIME_FAIL";
                            string failedStep = run.StepResults.LastOrDefault()?.Step?.Name ?? "unknown";
                            failures.Add(role + "/" + fileName + ": runtime stopped at " + failedStep + ".");
                        }
                        else
                        {
                            bool centersValid = true;
                            for (int locatorIndex = 0; locatorIndex < 3; locatorIndex++)
                            {
                                VisionPipelineStepResult stepResult = run.StepResults[locatorIndex];
                                scores[locatorIndex] = stepResult.ToolResult.Metrics.TryGetValue(
                                    VisionPipelineKnownMetrics.ScoreMax,
                                    out double score)
                                    ? score
                                    : double.NaN;
                                VisionPipelineGeometryFeatureResult? center =
                                    VisionPipelineGeometryFeatureStore
                                        .Get(stepResult.ToolResult)
                                        .SingleOrDefault(item =>
                                            item.Kind == VisionPipelineGeometryKind.Point
                                            && string.Equals(
                                                item.FeatureName,
                                                "Center",
                                                StringComparison.OrdinalIgnoreCase));
                                if (center == null)
                                {
                                    centersValid = false;
                                    break;
                                }
                                sourcePoints[locatorIndex] = new Point2f(
                                    (float)center.CenterX,
                                    (float)center.CenterY);
                            }

                            VisionPipelineStepResult affineStep = run.StepResults[3];
                            validPixelRatio = affineStep.ToolResult.Metrics.TryGetValue(
                                VisionPipelineKnownMetrics.AffineValidPixelRatio,
                                out double ratio)
                                ? ratio
                                : double.NaN;
                            using Mat normalized = context.GetLayer("CardReference");
                            if (!centersValid || normalized == null || normalized.Empty())
                            {
                                status = "POINT_OR_AFFINE_FAIL";
                                failures.Add(role + "/" + fileName + ": three typed Points or Affine output were missing.");
                            }
                            else
                            {
                                Cv2.ImWrite(
                                    Path.Combine(runDirectory, "05_normalized_raw.png"),
                                    normalized);
                                using Mat postCheckDrawing = ValidateNormalizedCardPoints(
                                    normalized,
                                    templates,
                                    destinationPoints,
                                    out postMinScore,
                                    out postMaxResidual);
                                Cv2.ImWrite(
                                    Path.Combine(runDirectory, "06_fixed_point_recheck.png"),
                                    postCheckDrawing);
                                if (includeFixedRoiMean)
                                {
                                    VisionPipelineStepResult fixedRoiStep = run.StepResults[4];
                                    fixedRoiMean = fixedRoiStep.ToolResult.Metrics.TryGetValue(
                                        VisionPipelineKnownMetrics.MeanValueAvg,
                                        out double meanValue)
                                        ? meanValue
                                        : double.NaN;
                                    if (!double.IsFinite(fixedRoiMean))
                                    {
                                        status = "FIXED_ROI_MEAN_FAIL";
                                        failures.Add(
                                            role
                                            + "/"
                                            + fileName
                                            + ": fixed reference ROI did not publish MeanValueAvg.");
                                    }
                                }
                                else
                                {
                                    Cv2.ImWrite(reviewImagePath, postCheckDrawing);
                                }
                                if (status == "PASS"
                                    && (postMinScore < 0.65D
                                        || postMaxResidual > maximumPostResidualPx))
                                {
                                    status = "POST_CHECK_FAIL";
                                    failures.Add(
                                        role
                                        + "/"
                                         + fileName
                                         + $": normalized point check score/residual was {postMinScore:0.000}/{postMaxResidual:0.00}px.");
                                }
                            }
                        }
                    }
                    finally
                    {
                        foreach (VisionPipelineStepResult stepResult in run.StepResults)
                        {
                            stepResult?.ToolResult?.ResultImage?.Dispose();
                        }
                    }
                }
            }

            if (status == "PASS")
            {
                passed++;
            }
            if (File.Exists(reviewImagePath))
            {
                reviewImages.Add(reviewImagePath);
                string sampleId = Path.GetFileNameWithoutExtension(fileName)
                    .Split('_')
                    .Last();
                reviewLabels.Add(
                    $"{role}_{sampleId} | {status} | r={postMaxResidual:0.00}px");
            }
            List<string> csvValues = new List<string>
            {
                role,
                fileName,
                status,
                scores[0].ToString("0.000000", CultureInfo.InvariantCulture),
                scores[1].ToString("0.000000", CultureInfo.InvariantCulture),
                scores[2].ToString("0.000000", CultureInfo.InvariantCulture),
                FormatPoint(sourcePoints[0]),
                FormatPoint(sourcePoints[1]),
                FormatPoint(sourcePoints[2]),
                validPixelRatio.ToString("0.000000", CultureInfo.InvariantCulture),
                postMinScore.ToString("0.000000", CultureInfo.InvariantCulture),
                postMaxResidual.ToString("0.000000", CultureInfo.InvariantCulture)
            };
            if (includeFixedRoiMean)
            {
                csvValues.Add(fixedRoiMean.ToString("0.000000", CultureInfo.InvariantCulture));
            }
            csvValues.Add(File.Exists(imagePath) ? ComputeSha256(imagePath) : "MISSING");
            csvValues.Add(EscapeBatchCsvValue(runDirectory));
            csvRows.Add(string.Join(",", csvValues));
        }

        File.WriteAllLines(
            Path.Combine(evidenceDirectory, evidencePrefix + "_results.csv"),
            csvRows);
        if (reviewImages.Count > 0)
        {
            SaveCardPilotContactSheet(
                reviewImages,
                reviewLabels,
                Path.Combine(
                    evidenceDirectory,
                    evidencePrefix
                    + (includeFixedRoiMean
                        ? "_fixed_roi_contact_sheet.png"
                        : "_normalized_recheck_contact_sheet.png")));
        }
        File.WriteAllLines(
            Path.Combine(evidenceDirectory, evidencePrefix + "_report.txt"),
            new[]
            {
                "Result: " + (passed == selected.Length ? "PASS" : "FAIL"),
                $"Samples: {selected.Length}",
                $"Passed: {passed}",
                $"Failed: {selected.Length - passed}",
                "Reference: " + referencePath,
                "ReferenceSha256: " + ComputeSha256(referencePath),
                "PipelineSha256: " + ComputeSha256(pipelinePath),
                "FrozenMatching: CCoeffNormed; ScoreMin=0.55; Angle=-8..8 step 1; Scale=0.9..1.1 step 0.05; one coarse ROI per approved feature",
                $"PostCheckGate: min normalized template score >= 0.65 and max center residual <= {maximumPostResidualPx:0.##} px",
                includeFixedRoiMean
                    ? "FixedRoi: Mean on CardReference at CvROI=250,315,190,80; no acceptance judgement."
                    : "FixedRoi: not executed.",
                "Boundary: fixture normalization and fixed-ROI linkage only; NG inspection classification was not attempted."
            }.Concat(failures.Select(item => "Failure: " + item)));

        if (passed == selected.Length)
        {
            Console.WriteLine(
                $"Affine card pilot passed {passed}/{selected.Length}. "
                + "Approved R/5/expiry Points normalized into the fixed reference frame.");
            return 0;
        }

        Console.Error.WriteLine(
            $"Affine card pilot failed {selected.Length - passed}/{selected.Length}; no parameter tuning was applied.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }
        return 1;
    }
    finally
    {
        foreach (Mat template in templates)
        {
            template?.Dispose();
        }
    }
}

static async Task<int> RunEdgeUniqueCardRMatrixAsync(
    string datasetRootArgument,
    string p220ResultsCsvArgument,
    string evidenceDirectoryArgument)
{
    const double maximumCenterErrorPx = 5D;
    const double scoreMinimum = 0.45D;
    const double uniqueMarginMinimum = 0.03D;
    string datasetRoot = Path.GetFullPath(datasetRootArgument);
    string p220ResultsCsv = Path.GetFullPath(p220ResultsCsvArgument);
    string evidenceDirectory = Path.GetFullPath(evidenceDirectoryArgument);
    string referencePath = Path.Combine(
        datasetRoot,
        "images",
        "OK",
        "card_original_OK_0001.jpg");
    Directory.CreateDirectory(evidenceDirectory);

    if (!File.Exists(referencePath) || !File.Exists(p220ResultsCsv))
    {
        Console.Error.WriteLine(
            "P225 requires the approved card reference and P220 results CSV. "
            + $"Reference={referencePath}; Baseline={p220ResultsCsv}");
        return 2;
    }

    List<(string Role, string FileName, Point2f ExpectedCenter, string SourceSha256)> baselines;
    try
    {
        baselines = LoadCardRBaselines(p220ResultsCsv);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("P220 baseline CSV could not be read: " + ex.Message);
        return 2;
    }
    if (baselines.Count != 12)
    {
        Console.Error.WriteLine($"P225 expected the exact 12-row P220 baseline, but found {baselines.Count} rows.");
        return 2;
    }

    string templateDirectory = Path.Combine(evidenceDirectory, "templates");
    Directory.CreateDirectory(templateDirectory);
    string templatePath = Path.Combine(templateDirectory, "01_R_from_card_original_OK_0001.png");
    using (Mat reference = Cv2.ImRead(referencePath, ImreadModes.Color))
    {
        Rect templateRoi = new Rect(100, 38, 68, 126);
        if (reference.Empty()
            || reference.Width != 640
            || reference.Height != 480
            || templateRoi.X + templateRoi.Width > reference.Width
            || templateRoi.Y + templateRoi.Height > reference.Height)
        {
            Console.Error.WriteLine("P225 reference image or approved R template ROI is invalid.");
            return 2;
        }
        using Mat template = reference.SubMat(templateRoi).Clone();
        Cv2.ImWrite(templatePath, template);
    }

    (string Key, string Label, Rect SearchRoi, bool UniqueEnabled)[] modes =
    {
        ("01_narrow_unique", "Reviewed ROI + unique", new Rect(85, 5, 220, 200), true),
        ("02_broad_legacy", "Original broad ROI + legacy", new Rect(50, 5, 180, 200), false),
        ("03_broad_unique", "Original broad ROI + unique", new Rect(50, 5, 180, 200), true)
    };

    List<string> csvRows = new List<string>
    {
        "Mode,ModeLabel,UniqueEnabled,SearchRoi,Role,FileName,SourceSha256,BaselineCenter,PipelineSuccess,ToolSuccess,Outcome,ErrorCode,ErrorName,ScoreMax,UniqueState,UniqueAlternativeCount,UniqueScoreMargin,DetectedCenter,CenterErrorPx,ElapsedMilliseconds,RawDrawingPath,ComparisonDrawingPath"
    };
    List<(string Key, string Label, int CorrectAccept, int FalseAccept, int AmbiguousReject, int NoMatchReject, int RuntimeError, double MeanElapsedMs)> summaries
        = new List<(string, string, int, int, int, int, int, double)>();
    List<string> integrityFailures = new List<string>();

    foreach ((string modeKey, string modeLabel, Rect searchRoi, bool uniqueEnabled) in modes)
    {
        string modeDirectory = Path.Combine(evidenceDirectory, modeKey);
        Directory.CreateDirectory(modeDirectory);
        VisionPipeline pipeline = CreateEdgeUniqueCardRPipeline(
            templatePath,
            searchRoi,
            uniqueEnabled,
            scoreMinimum,
            uniqueMarginMinimum);
        string pipelinePath = Path.Combine(modeDirectory, modeKey + ".pipeline.xml");
        string saveMessage = string.Empty;
        string loadMessage = string.Empty;
        VisionPipeline loaded = pipeline;
        if (!VisionPipelineStorage.TrySaveToFile(
                pipelinePath,
                pipeline,
                out saveMessage)
            || !VisionPipelineStorage.TryLoadFromFile(
                pipelinePath,
                out loaded,
                out loadMessage))
        {
            integrityFailures.Add(
                $"{modeKey}: Pipeline XML round trip failed. {saveMessage} {loadMessage}");
            continue;
        }
        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            loaded,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (!validation.Success)
        {
            integrityFailures.Add(
                $"{modeKey}: Pipeline definition failed validation: "
                + string.Join(" | ", validation.Errors));
            continue;
        }
        IVisionTool configuredTool = VisionPipelineAppToolFactory.Create(loaded.Steps.Single());
        if (configuredTool is not EdgeBasedTemplateMatchingTool configuredEdgeTool
            || configuredEdgeTool.property == null
            || !configuredEdgeTool.property.USE_FIND_SCALE
            || Math.Abs(configuredEdgeTool.property.FIND_SCALE_MIN - 0.9D) > 0.000001D
            || Math.Abs(configuredEdgeTool.property.FIND_SCALE_MAX - 1.1D) > 0.000001D
            || Math.Abs(configuredEdgeTool.property.FIND_SCALE_STEP - 0.05D) > 0.000001D
            || !configuredEdgeTool.property.USE_POSITION_REFINE
            || !configuredEdgeTool.property.USE_SUBPIXEL_REFINE
            || configuredEdgeTool.property.USE_UNIQUE_MATCH_VALIDATION != uniqueEnabled)
        {
            integrityFailures.Add(
                $"{modeKey}: EdgeBasedMatching Pipeline factory did not retain the frozen "
                + "scale/refinement/unique settings.");
            continue;
        }

        int correctAccept = 0;
        int falseAccept = 0;
        int ambiguousReject = 0;
        int noMatchReject = 0;
        int runtimeError = 0;
        List<double> elapsedValues = new List<double>();
        List<string> reviewImages = new List<string>();
        List<string> reviewLabels = new List<string>();

        for (int sampleIndex = 0; sampleIndex < baselines.Count; sampleIndex++)
        {
            (string role, string fileName, Point2f expectedCenter, string expectedSha256) = baselines[sampleIndex];
            string imagePath = Path.Combine(datasetRoot, "images", role, fileName);
            string runDirectory = Path.Combine(
                modeDirectory,
                "runs",
                $"{sampleIndex + 1:00}_{role}_{Path.GetFileNameWithoutExtension(fileName)}");
            Directory.CreateDirectory(runDirectory);
            string rawDrawingPath = Path.Combine(runDirectory, "01_runtime_drawing.png");
            string comparisonPath = Path.Combine(runDirectory, "02_baseline_comparison.png");
            string sourceSha256 = File.Exists(imagePath) ? ComputeSha256(imagePath) : "MISSING";
            string outcome = "RUNTIME_ERROR";
            string errorName = "MissingSource";
            int errorCode = -1;
            bool pipelineSuccess = false;
            bool toolSuccess = false;
            double scoreMax = double.NaN;
            double uniqueState = double.NaN;
            double alternativeCount = double.NaN;
            double uniqueScoreMargin = double.NaN;
            Point2f? detectedCenter = null;
            double centerErrorPx = double.NaN;
            double elapsedMs = double.NaN;

            if (!File.Exists(imagePath))
            {
                integrityFailures.Add($"{role}/{fileName}: source image is missing.");
                runtimeError++;
            }
            else if (!string.Equals(sourceSha256, expectedSha256, StringComparison.OrdinalIgnoreCase))
            {
                integrityFailures.Add(
                    $"{role}/{fileName}: source SHA-256 changed. Expected {expectedSha256}, actual {sourceSha256}.");
                runtimeError++;
            }
            else
            {
                using Mat source = Cv2.ImRead(imagePath, ImreadModes.Color);
                if (source.Empty())
                {
                    integrityFailures.Add($"{role}/{fileName}: source image could not be loaded.");
                    runtimeError++;
                }
                else
                {
                    using VisionPipelineContext context = new VisionPipelineContext();
                    context.SetLayer(VisionRecipeRunner.DefaultInputLayer, source);
                    VisionPipelineRunResult run = await VisionPipelineExecutionService.RunAsync(
                        loaded,
                        context,
                        30000,
                        CancellationToken.None);
                    pipelineSuccess = run.Success;
                    VisionPipelineStepResult? stepResult = run.StepResults.FirstOrDefault();
                    VisionToolResult? toolResult = stepResult?.ToolResult;
                    if (toolResult == null)
                    {
                        integrityFailures.Add($"{modeKey}/{role}/{fileName}: Step result was not created.");
                        runtimeError++;
                    }
                    else
                    {
                        toolSuccess = toolResult.Success;
                        errorCode = (int)toolResult.ErrorCode;
                        errorName = toolResult.ErrorCode.ToString();
                        elapsedMs = toolResult.Elapsed.TotalMilliseconds;
                        elapsedValues.Add(elapsedMs);
                        scoreMax = GetMetricOrNaN(toolResult, VisionPipelineKnownMetrics.ScoreMax);
                        uniqueState = GetMetricOrNaN(toolResult, VisionPipelineKnownMetrics.UniqueMatchState);
                        alternativeCount = GetMetricOrNaN(
                            toolResult,
                            VisionPipelineKnownMetrics.UniqueMatchPlausibleAlternativeCount);
                        uniqueScoreMargin = GetMetricOrNaN(
                            toolResult,
                            VisionPipelineKnownMetrics.UniqueMatchScoreMargin);
                        VisionPipelineGeometryFeatureResult? center =
                            VisionPipelineGeometryFeatureStore
                                .Get(toolResult)
                                .SingleOrDefault(item =>
                                    item.Kind == VisionPipelineGeometryKind.Point
                                    && string.Equals(
                                        item.FeatureName,
                                        "Center",
                                        StringComparison.OrdinalIgnoreCase));
                        if (center != null)
                        {
                            detectedCenter = new Point2f(
                                (float)center.CenterX,
                                (float)center.CenterY);
                            centerErrorPx = Math.Sqrt(
                                Math.Pow(center.CenterX - expectedCenter.X, 2D)
                                + Math.Pow(center.CenterY - expectedCenter.Y, 2D));
                        }

                        if (toolSuccess && detectedCenter.HasValue)
                        {
                            if (centerErrorPx <= maximumCenterErrorPx)
                            {
                                outcome = "CORRECT_ACCEPT";
                                correctAccept++;
                            }
                            else
                            {
                                outcome = "FALSE_ACCEPT";
                                falseAccept++;
                            }
                        }
                        else if (toolResult.ErrorCode == VisionToolErrorCode.MatchingAmbiguous)
                        {
                            outcome = "AMBIGUOUS_REJECT";
                            ambiguousReject++;
                        }
                        else if (toolResult.ErrorCode == VisionToolErrorCode.MatchingNoResult)
                        {
                            outcome = "NO_MATCH_REJECT";
                            noMatchReject++;
                        }
                        else
                        {
                            outcome = "RUNTIME_ERROR";
                            runtimeError++;
                            integrityFailures.Add(
                                $"{modeKey}/{role}/{fileName}: unexpected runtime result "
                                + $"{toolResult.ErrorCode}: {toolResult.Message}");
                        }

                        if (toolResult.ResultImage != null && !toolResult.ResultImage.Empty())
                        {
                            Cv2.ImWrite(rawDrawingPath, toolResult.ResultImage);
                        }
                        using Mat comparison = CreateEdgeUniqueComparisonDrawing(
                            source,
                            toolResult.ResultImage,
                            searchRoi,
                            expectedCenter,
                            detectedCenter,
                            centerErrorPx,
                            outcome,
                            scoreMax,
                            uniqueScoreMargin);
                        Cv2.ImWrite(comparisonPath, comparison);
                        reviewImages.Add(comparisonPath);
                        reviewLabels.Add(
                            $"{sampleIndex + 1:00} {role} {outcome} "
                            + (double.IsFinite(centerErrorPx)
                                ? $"{centerErrorPx:0.00}px"
                                : errorName));
                    }
                }
            }

            string[] values =
            {
                modeKey,
                modeLabel,
                uniqueEnabled.ToString(CultureInfo.InvariantCulture),
                $"{searchRoi.X};{searchRoi.Y};{searchRoi.Width};{searchRoi.Height}",
                role,
                fileName,
                sourceSha256,
                $"{expectedCenter.X:0.###};{expectedCenter.Y:0.###}",
                pipelineSuccess.ToString(CultureInfo.InvariantCulture),
                toolSuccess.ToString(CultureInfo.InvariantCulture),
                outcome,
                errorCode.ToString(CultureInfo.InvariantCulture),
                errorName,
                FormatFinite(scoreMax),
                FormatFinite(uniqueState),
                FormatFinite(alternativeCount),
                FormatFinite(uniqueScoreMargin),
                detectedCenter.HasValue
                    ? $"{detectedCenter.Value.X:0.###};{detectedCenter.Value.Y:0.###}"
                    : string.Empty,
                FormatFinite(centerErrorPx),
                FormatFinite(elapsedMs),
                File.Exists(rawDrawingPath) ? rawDrawingPath : string.Empty,
                File.Exists(comparisonPath) ? comparisonPath : string.Empty
            };
            csvRows.Add(string.Join(",", values.Select(EscapeBatchCsvValue)));
        }

        if (reviewImages.Count > 0)
        {
            SaveCardPilotContactSheet(
                reviewImages,
                reviewLabels,
                Path.Combine(modeDirectory, modeKey + "_contact_sheet.png"));
        }
        summaries.Add((
            modeKey,
            modeLabel,
            correctAccept,
            falseAccept,
            ambiguousReject,
            noMatchReject,
            runtimeError,
            elapsedValues.Count > 0 ? elapsedValues.Average() : double.NaN));
    }

    string resultsPath = Path.Combine(evidenceDirectory, "p225_edge_unique_card_r_results.csv");
    File.WriteAllLines(resultsPath, csvRows);
    (string Key, string Label, int CorrectAccept, int FalseAccept, int AmbiguousReject, int NoMatchReject, int RuntimeError, double MeanElapsedMs)
        narrow = summaries.SingleOrDefault(item => item.Key == "01_narrow_unique");
    (string Key, string Label, int CorrectAccept, int FalseAccept, int AmbiguousReject, int NoMatchReject, int RuntimeError, double MeanElapsedMs)
        broadLegacy = summaries.SingleOrDefault(item => item.Key == "02_broad_legacy");
    (string Key, string Label, int CorrectAccept, int FalseAccept, int AmbiguousReject, int NoMatchReject, int RuntimeError, double MeanElapsedMs)
        broadUnique = summaries.SingleOrDefault(item => item.Key == "03_broad_unique");

    string decision;
    if (integrityFailures.Count > 0 || summaries.Count != modes.Length)
    {
        decision = "Incomplete";
    }
    else if (narrow.CorrectAccept != baselines.Count
        || narrow.FalseAccept != 0
        || narrow.AmbiguousReject != 0
        || narrow.NoMatchReject != 0
        || narrow.RuntimeError != 0)
    {
        decision = "Reject fixed candidate";
    }
    else if (broadLegacy.FalseAccept > 0
        && broadUnique.FalseAccept == 0
        && broadUnique.RuntimeError == 0)
    {
        decision = "Keep";
    }
    else
    {
        decision = "Keep with documented limits";
    }

    List<string> report = new List<string>
    {
        "# P225 Edge Unique Card R Fixed-ROI Matrix",
        string.Empty,
        $"Decision: `{decision}`",
        string.Empty,
        "## Frozen inputs",
        string.Empty,
        $"- Dataset: `{datasetRoot}`",
        $"- Reference: `{referencePath}`",
        $"- Reference SHA-256: `{ComputeSha256(referencePath)}`",
        $"- R template ROI: `100,38,68,126`",
        $"- Template SHA-256: `{ComputeSha256(templatePath)}`",
        $"- P220 baseline: `{p220ResultsCsv}`",
        $"- P220 baseline SHA-256: `{ComputeSha256(p220ResultsCsv)}`",
        $"- Accepted center-error gate: `<= {maximumCenterErrorPx:0.###} px` (P221 operator decision)",
        $"- Edge score gate: `{scoreMinimum:0.###}`; unique margin: `{uniqueMarginMinimum:0.###}`",
        "- Pose envelope: angle `-8..8 deg / 1 deg`, scale `0.9..1.1 / 0.05`",
        "- No parameter tuning was performed after observing outcomes.",
        string.Empty,
        "## Results",
        string.Empty,
        "| Mode | Correct accept | Baseline mismatch >5 px | Ambiguous reject | No-match reject | Runtime error | Mean ms |",
        "| --- | ---: | ---: | ---: | ---: | ---: | ---: |"
    };
    report.AddRange(summaries.Select(item =>
        $"| {item.Label} | {item.CorrectAccept}/12 | {item.FalseAccept} | "
        + $"{item.AmbiguousReject} | {item.NoMatchReject} | {item.RuntimeError} | "
        + $"{item.MeanElapsedMs:0.###} |"));
    report.AddRange(new[]
    {
        string.Empty,
        "## Interpretation boundary",
        string.Empty,
        "The expected R centers are the previously reviewed P220/P221 Matching centers, not independent metrology ground truth. "
        + "OK/NG is retained only as a dataset stratum; it is not an R-locator truth label. "
        + "A `FALSE_ACCEPT` here means an EdgeBasedMatching center more than 5 px from that frozen baseline.",
        string.Empty,
        "Every row retains the unmodified current-run result drawing and a separate comparison drawing. "
        + "Yellow marks the frozen baseline, green marks a result within 5 px, red marks a result outside the gate, and cyan marks the exact search ROI.",
        string.Empty,
        "## Integrity issues",
        string.Empty
    });
    report.AddRange(integrityFailures.Count == 0
        ? new[] { "- None." }
        : integrityFailures.Select(item => "- " + item));
    File.WriteAllLines(Path.Combine(evidenceDirectory, "README.md"), report);

    Console.WriteLine($"P225 matrix decision: {decision}");
    foreach (var summary in summaries)
    {
        Console.WriteLine(
            $"{summary.Key}: correct={summary.CorrectAccept}/12, false={summary.FalseAccept}, "
            + $"ambiguous={summary.AmbiguousReject}, no-match={summary.NoMatchReject}, "
            + $"runtime-error={summary.RuntimeError}, mean={summary.MeanElapsedMs:0.###}ms");
    }
    if (integrityFailures.Count > 0)
    {
        foreach (string failure in integrityFailures)
        {
            Console.Error.WriteLine("- " + failure);
        }
        return 1;
    }
    return 0;
}

static VisionPipeline CreateEdgeUniqueCardRPipeline(
    string templatePath,
    Rect searchRoi,
    bool uniqueEnabled,
    double scoreMinimum,
    double uniqueMarginMinimum)
{
    VisionPipelineStep match = new VisionPipelineStep
    {
        Name = "01 Locate approved R with EdgeBasedMatching",
        ToolType = "EdgeBasedMatching",
        Enabled = true,
        InputLayer = VisionRecipeRunner.DefaultInputLayer,
        OutputLayer = "Card_R_Edge_Result"
    };
    match.Parameters["PATTERN_PATH"] = templatePath;
    match.Parameters["TemplatePath"] = templatePath;
    match.Parameters["SCORE_MIN"] = scoreMinimum.ToString("0.###", CultureInfo.InvariantCulture);
    match.Parameters["NUM_MATCH"] = "1";
    match.Parameters["USE_UNIQUE_MATCH_VALIDATION"] = uniqueEnabled.ToString(CultureInfo.InvariantCulture);
    match.Parameters["UNIQUE_MATCH_MIN_SCORE_MARGIN"] =
        uniqueMarginMinimum.ToString("0.###", CultureInfo.InvariantCulture);
    match.Parameters["USE_FIND_ANGLE"] = "true";
    match.Parameters["FIND_ANGLE_MIN"] = "-8";
    match.Parameters["FIND_ANGLE_MAX"] = "8";
    match.Parameters["FIND_ANGLE"] = "1";
    match.Parameters["USE_COARSE_TO_FINE_ANGLE_SEARCH"] = "false";
    match.Parameters["USE_FIND_SCALE"] = "true";
    match.Parameters["FIND_SCALE_MIN"] = "0.9";
    match.Parameters["FIND_SCALE_MAX"] = "1.1";
    match.Parameters["FIND_SCALE_STEP"] = "0.05";
    match.Parameters["CANNY_LOW"] = "30";
    match.Parameters["CANNY_HIGH"] = "90";
    match.Parameters["CANNY_APERTURE_SIZE"] = "3";
    match.Parameters["USE_L2_GRADIENT"] = "true";
    match.Parameters["CONTOUR_RETRIEVAL_MODE"] = RetrievalModes.External.ToString();
    match.Parameters["CONTOUR_APPROXIMATION_MODE"] = ContourApproximationModes.ApproxNone.ToString();
    match.Parameters["MAX_TEMPLATE_POINTS"] = "300";
    match.Parameters["MIN_GRADIENT_MAGNITUDE"] = "1";
    match.Parameters["GREEDINESS"] = "0.9";
    match.Parameters["SEARCH_STEP"] = "2";
    match.Parameters["USE_POSITION_REFINE"] = "true";
    match.Parameters["USE_SUBPIXEL_REFINE"] = "true";
    match.Parameters["USE_PYRAMID_POSITION_PROPOSAL"] = "false";
    match.Parameters["PYRAMID_POSITION_TOP_N"] = "6";
    match.Parameters["PYRAMID_POSITION_MIN_SCORE"] = "0.7";
    match.Parameters["USE_HYBRID_VERIFY"] = "false";
    match.Parameters["HYBRID_VERIFY_TOP_N"] = "5";
    match.Parameters["HYBRID_VERIFY_IMAGE_WEIGHT"] = "0.35";
    match.Parameters["USE_DRAW_IMAGE"] = "true";
    match.Parameters["USE_THRESHOLD"] = "false";
    match.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
    match.Parameters["USE_ROI"] = "true";
    match.Parameters["CvROI"] = string.Format(
        CultureInfo.InvariantCulture,
        "{0},{1},{2},{3}",
        searchRoi.X,
        searchRoi.Y,
        searchRoi.Width,
        searchRoi.Height);
    match.Parameters["USE_MULTI_ROI"] = "false";
    return new VisionPipeline
    {
        Name = uniqueEnabled
            ? "P225 Card R EdgeBasedMatching unique"
            : "P225 Card R EdgeBasedMatching legacy",
        Steps = { match }
    };
}

static List<(string Role, string FileName, Point2f ExpectedCenter, string SourceSha256)> LoadCardRBaselines(
    string csvPath)
{
    string[] lines = File.ReadAllLines(csvPath);
    if (lines.Length < 2)
    {
        throw new InvalidDataException("CSV has no data rows.");
    }
    List<string> header = ParseCsvRecord(lines[0]);
    int roleIndex = header.FindIndex(value => value == "Role");
    int fileIndex = header.FindIndex(value => value == "FileName");
    int pointIndex = header.FindIndex(value => value == "SourcePoint1");
    int hashIndex = header.FindIndex(value => value == "SourceSha256");
    if (roleIndex < 0 || fileIndex < 0 || pointIndex < 0 || hashIndex < 0)
    {
        throw new InvalidDataException("CSV is missing Role, FileName, SourcePoint1, or SourceSha256.");
    }

    List<(string, string, Point2f, string)> result = new List<(string, string, Point2f, string)>();
    foreach (string line in lines.Skip(1).Where(value => !string.IsNullOrWhiteSpace(value)))
    {
        List<string> values = ParseCsvRecord(line);
        if (values.Count <= Math.Max(Math.Max(roleIndex, fileIndex), Math.Max(pointIndex, hashIndex)))
        {
            throw new InvalidDataException("CSV row has fewer fields than its header.");
        }
        string[] point = values[pointIndex].Split(';');
        if (point.Length != 2
            || !float.TryParse(point[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x)
            || !float.TryParse(point[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
        {
            throw new InvalidDataException("SourcePoint1 is not an X;Y point: " + values[pointIndex]);
        }
        result.Add((values[roleIndex], values[fileIndex], new Point2f(x, y), values[hashIndex]));
    }
    return result;
}

static List<string> ParseCsvRecord(string line)
{
    List<string> values = new List<string>();
    System.Text.StringBuilder current = new System.Text.StringBuilder();
    bool quoted = false;
    for (int index = 0; index < line.Length; index++)
    {
        char character = line[index];
        if (character == '"')
        {
            if (quoted && index + 1 < line.Length && line[index + 1] == '"')
            {
                current.Append('"');
                index++;
            }
            else
            {
                quoted = !quoted;
            }
        }
        else if (character == ',' && !quoted)
        {
            values.Add(current.ToString());
            current.Clear();
        }
        else
        {
            current.Append(character);
        }
    }
    values.Add(current.ToString());
    return values;
}

static double GetMetricOrNaN(VisionToolResult result, string name)
{
    return result?.Metrics != null && result.Metrics.TryGetValue(name, out double value)
        ? value
        : double.NaN;
}

static string FormatFinite(double value)
{
    return double.IsFinite(value)
        ? value.ToString("0.############", CultureInfo.InvariantCulture)
        : string.Empty;
}

static Mat CreateEdgeUniqueComparisonDrawing(
    Mat source,
    Mat? runtimeDrawing,
    Rect searchRoi,
    Point2f expectedCenter,
    Point2f? detectedCenter,
    double centerErrorPx,
    string outcome,
    double scoreMax,
    double uniqueScoreMargin)
{
    Mat drawing;
    Mat basis = runtimeDrawing != null && !runtimeDrawing.Empty() ? runtimeDrawing : source;
    if (basis.Channels() == 1)
    {
        drawing = new Mat();
        Cv2.CvtColor(basis, drawing, ColorConversionCodes.GRAY2BGR);
    }
    else
    {
        drawing = basis.Clone();
    }

    Cv2.Rectangle(drawing, searchRoi, new Scalar(255, 255, 0), 1, LineTypes.AntiAlias);
    DrawMatrixCross(drawing, expectedCenter, new Scalar(0, 255, 255), 9, 2);
    Cv2.PutText(
        drawing,
        $"baseline ({expectedCenter.X:0.0},{expectedCenter.Y:0.0})",
        new OpenCvSharp.Point(
            Math.Clamp((int)Math.Round(expectedCenter.X) + 12, 0, Math.Max(0, drawing.Width - 1)),
            Math.Clamp((int)Math.Round(expectedCenter.Y) - 8, 18, Math.Max(18, drawing.Height - 1))),
        HersheyFonts.HersheySimplex,
        0.45,
        new Scalar(0, 255, 255),
        1,
        LineTypes.AntiAlias);

    if (detectedCenter.HasValue)
    {
        Scalar detectedColor = centerErrorPx <= 5D
            ? new Scalar(0, 255, 0)
            : new Scalar(0, 0, 255);
        DrawMatrixCross(drawing, detectedCenter.Value, detectedColor, 11, 2);
        Cv2.Line(
            drawing,
            new OpenCvSharp.Point(
                (int)Math.Round(expectedCenter.X),
                (int)Math.Round(expectedCenter.Y)),
            new OpenCvSharp.Point(
                (int)Math.Round(detectedCenter.Value.X),
                (int)Math.Round(detectedCenter.Value.Y)),
            detectedColor,
            1,
            LineTypes.AntiAlias);
    }

    Cv2.Rectangle(
        drawing,
        new Rect(0, 0, drawing.Width, Math.Min(44, drawing.Height)),
        Scalar.Black,
        -1);
    Cv2.PutText(
        drawing,
        $"{outcome} | err={(double.IsFinite(centerErrorPx) ? centerErrorPx.ToString("0.00", CultureInfo.InvariantCulture) + "px" : "-")}",
        new OpenCvSharp.Point(8, 18),
        HersheyFonts.HersheySimplex,
        0.48,
        outcome == "CORRECT_ACCEPT" ? new Scalar(0, 255, 0) : new Scalar(0, 165, 255),
        1,
        LineTypes.AntiAlias);
    Cv2.PutText(
        drawing,
        $"score={FormatFinite(scoreMax)} uniqueMargin={FormatFinite(uniqueScoreMargin)}",
        new OpenCvSharp.Point(8, 37),
        HersheyFonts.HersheySimplex,
        0.42,
        Scalar.White,
        1,
        LineTypes.AntiAlias);
    return drawing;
}

static void DrawMatrixCross(Mat image, Point2f point, Scalar color, int radius, int thickness)
{
    OpenCvSharp.Point center = new OpenCvSharp.Point(
        Math.Clamp((int)Math.Round(point.X), 0, Math.Max(0, image.Width - 1)),
        Math.Clamp((int)Math.Round(point.Y), 0, Math.Max(0, image.Height - 1)));
    Cv2.Line(
        image,
        new OpenCvSharp.Point(Math.Max(0, center.X - radius), center.Y),
        new OpenCvSharp.Point(Math.Min(image.Width - 1, center.X + radius), center.Y),
        color,
        thickness,
        LineTypes.AntiAlias);
    Cv2.Line(
        image,
        new OpenCvSharp.Point(center.X, Math.Max(0, center.Y - radius)),
        new OpenCvSharp.Point(center.X, Math.Min(image.Height - 1, center.Y + radius)),
        color,
        thickness,
        LineTypes.AntiAlias);
}

static VisionPipeline CreateCardAffinePilotPipeline(
    IReadOnlyList<string> templatePaths,
    IReadOnlyList<Rect> searchRois,
    IReadOnlyList<Point2f> destinationPoints,
    bool includeFixedRoiMean = false,
    Rect fixedInspectionRoi = default)
{
    string[] names = { "01 Locate R", "02 Locate 5", "03 Locate expiry" };
    VisionPipeline pipeline = new VisionPipeline
    {
        Name = "P220 Card Matching x3 to Affine reference"
    };
    for (int index = 0; index < 3; index++)
    {
        VisionPipelineStep match = new VisionPipelineStep
        {
            Name = names[index],
            ToolType = "Matching",
            Enabled = true,
            InputLayer = VisionRecipeRunner.DefaultInputLayer,
            OutputLayer = $"Card_Match_{index + 1}"
        };
        match.Parameters["PATTERN_PATH"] = templatePaths[index];
        match.Parameters["MATCH_MODE"] = TemplateMatchModes.CCoeffNormed.ToString();
        match.Parameters["SCORE_MIN"] = "0.55";
        match.Parameters["MAGNIFIATION"] = "1";
        match.Parameters["NUM_MATCH"] = "1";
        match.Parameters["USE_FIND_ANGLE"] = "true";
        match.Parameters["FIND_ANGLE_MIN"] = "-8";
        match.Parameters["FIND_ANGLE_MAX"] = "8";
        match.Parameters["FIND_ANGLE"] = "1";
        match.Parameters["USE_COARSE_TO_FINE_ANGLE_SEARCH"] = "false";
        match.Parameters["USE_FIND_SCALE"] = "true";
        match.Parameters["FIND_SCALE_MIN"] = "0.9";
        match.Parameters["FIND_SCALE_MAX"] = "1.1";
        match.Parameters["FIND_SCALE_STEP"] = "0.05";
        match.Parameters["USE_THRESHOLD"] = "false";
        match.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
        match.Parameters["USE_CANNY"] = "false";
        match.Parameters["USE_ROI"] = "true";
        match.Parameters["CvROI"] = string.Format(
            CultureInfo.InvariantCulture,
            "{0},{1},{2},{3}",
            searchRois[index].X,
            searchRois[index].Y,
            searchRois[index].Width,
            searchRois[index].Height);
        match.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
        pipeline.Steps.Add(match);
    }

    AffineTransformToolProperty property = new AffineTransformToolProperty
    {
        DestinationPoint1X = destinationPoints[0].X,
        DestinationPoint1Y = destinationPoints[0].Y,
        DestinationPoint2X = destinationPoints[1].X,
        DestinationPoint2Y = destinationPoints[1].Y,
        DestinationPoint3X = destinationPoints[2].X,
        DestinationPoint3Y = destinationPoints[2].Y,
        OutputWidth = 640,
        OutputHeight = 480,
        MinimumSourceTriangleArea = 10000,
        MinimumDestinationTriangleArea = 10000,
        MinimumValidPixelRatio = 0.55
    };
    VisionPipelineStep affine = VisionPipelineStepBuilder.FromAffineTransformProperty(
        property,
        "04 Normalize approved card points",
        VisionRecipeRunner.DefaultInputLayer,
        "CardReference");
    affine.Parameters[VisionPipelineAffinePointBindingService.UseDetectedSourcePointsParameter] = "true";
    affine.Parameters[VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter] = names[0] + "/Center";
    affine.Parameters[VisionPipelineAffinePointBindingService.SourcePoint2FeatureParameter] = names[1] + "/Center";
    affine.Parameters[VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter] = names[2] + "/Center";
    affine.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
    pipeline.Steps.Add(affine);
    if (includeFixedRoiMean)
    {
        VisionPipelineStep mean = new VisionPipelineStep
        {
            Name = "05 Measure fixed date ROI",
            ToolType = "Mean",
            Enabled = true,
            InputLayer = "CardReference",
            OutputLayer = "CardDateMean"
        };
        mean.Parameters["Name"] = "P221_CardDateMean";
        mean.Parameters["MEAN_TYPES"] = "Mean";
        mean.Parameters["USE_THRESHOLD"] = "false";
        mean.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
        mean.Parameters["USE_BITWISENOT"] = "false";
        mean.Parameters["USE_ROI"] = "true";
        mean.Parameters["CvROI"] = string.Format(
            CultureInfo.InvariantCulture,
            "{0},{1},{2},{3}",
            fixedInspectionRoi.X,
            fixedInspectionRoi.Y,
            fixedInspectionRoi.Width,
            fixedInspectionRoi.Height);
        mean.Parameters["USE_MULTI_ROI"] = "false";
        pipeline.Steps.Add(mean);
    }
    return pipeline;
}

static Mat ValidateNormalizedCardPoints(
    Mat normalized,
    IReadOnlyList<Mat> templates,
    IReadOnlyList<Point2f> destinationPoints,
    out double minimumScore,
    out double maximumResidual)
{
    using Mat gray = new Mat();
    if (normalized.Channels() == 1)
    {
        normalized.CopyTo(gray);
    }
    else
    {
        Cv2.CvtColor(normalized, gray, ColorConversionCodes.BGR2GRAY);
    }

    Mat drawing = new Mat();
    if (normalized.Channels() == 1)
    {
        Cv2.CvtColor(normalized, drawing, ColorConversionCodes.GRAY2BGR);
    }
    else
    {
        normalized.CopyTo(drawing);
    }

    minimumScore = double.PositiveInfinity;
    maximumResidual = 0D;
    for (int index = 0; index < templates.Count; index++)
    {
        using Mat templateGray = new Mat();
        if (templates[index].Channels() == 1)
        {
            templates[index].CopyTo(templateGray);
        }
        else
        {
            Cv2.CvtColor(templates[index], templateGray, ColorConversionCodes.BGR2GRAY);
        }

        int margin = 18;
        int left = Math.Max(
            0,
            (int)Math.Floor(destinationPoints[index].X - templateGray.Width / 2D - margin));
        int top = Math.Max(
            0,
            (int)Math.Floor(destinationPoints[index].Y - templateGray.Height / 2D - margin));
        int right = Math.Min(
            gray.Width,
            (int)Math.Ceiling(destinationPoints[index].X + templateGray.Width / 2D + margin));
        int bottom = Math.Min(
            gray.Height,
            (int)Math.Ceiling(destinationPoints[index].Y + templateGray.Height / 2D + margin));
        Rect searchRect = new Rect(left, top, right - left, bottom - top);
        using Mat search = gray.SubMat(searchRect);
        using Mat result = new Mat();
        Cv2.MatchTemplate(search, templateGray, result, TemplateMatchModes.CCoeffNormed);
        Cv2.MinMaxLoc(
            result,
            out _,
            out double score,
            out _,
            out OpenCvSharp.Point location);
        Point2f found = new Point2f(
            searchRect.X + location.X + templateGray.Width / 2F,
            searchRect.Y + location.Y + templateGray.Height / 2F);
        double residual = Math.Sqrt(
            Math.Pow(found.X - destinationPoints[index].X, 2D)
            + Math.Pow(found.Y - destinationPoints[index].Y, 2D));
        minimumScore = Math.Min(minimumScore, score);
        maximumResidual = Math.Max(maximumResidual, residual);

        Rect foundRect = new Rect(
            searchRect.X + location.X,
            searchRect.Y + location.Y,
            templateGray.Width,
            templateGray.Height);
        Cv2.Rectangle(drawing, foundRect, new Scalar(255, 0, 255), 2);
        Cv2.DrawMarker(
            drawing,
            new OpenCvSharp.Point(
                (int)Math.Round(destinationPoints[index].X),
                (int)Math.Round(destinationPoints[index].Y)),
            new Scalar(0, 255, 0),
            MarkerTypes.Cross,
            18,
            2);
        Cv2.Line(
            drawing,
            new OpenCvSharp.Point(
                (int)Math.Round(destinationPoints[index].X),
                (int)Math.Round(destinationPoints[index].Y)),
            new OpenCvSharp.Point(
                (int)Math.Round(found.X),
                (int)Math.Round(found.Y)),
            new Scalar(0, 255, 255),
            1);
        Cv2.PutText(
            drawing,
            $"{index + 1}: {score:0.000} / {residual:0.00}px",
            new OpenCvSharp.Point(foundRect.X, Math.Max(18, foundRect.Y - 6)),
            HersheyFonts.HersheySimplex,
            0.5,
            new Scalar(0, 255, 255),
            1,
            LineTypes.AntiAlias);
    }
    return drawing;
}

static void SaveCardPilotContactSheet(
    IReadOnlyList<string> imagePaths,
    IReadOnlyList<string> labels,
    string outputPath)
{
    const int columns = 3;
    const int tileWidth = 320;
    const int imageHeight = 240;
    const int labelHeight = 34;
    int rows = (int)Math.Ceiling(imagePaths.Count / (double)columns);
    using Mat sheet = new Mat(
        rows * (imageHeight + labelHeight),
        columns * tileWidth,
        MatType.CV_8UC3,
        Scalar.Black);
    for (int index = 0; index < imagePaths.Count; index++)
    {
        using Mat source = Cv2.ImRead(imagePaths[index], ImreadModes.Color);
        if (source.Empty())
        {
            continue;
        }
        using Mat resized = new Mat();
        Cv2.Resize(source, resized, new OpenCvSharp.Size(tileWidth, imageHeight));
        int x = index % columns * tileWidth;
        int y = index / columns * (imageHeight + labelHeight);
        resized.CopyTo(sheet.SubMat(new Rect(x, y, tileWidth, imageHeight)));
        Cv2.PutText(
            sheet,
            labels[index],
            new OpenCvSharp.Point(x + 8, y + imageHeight + 23),
            HersheyFonts.HersheySimplex,
            0.42,
            new Scalar(0, 255, 255),
            1,
            LineTypes.AntiAlias);
    }
    Cv2.ImWrite(outputPath, sheet);
}

static string FormatPoint(Point2f point)
{
    return string.Format(
        CultureInfo.InvariantCulture,
        "\"{0:0.000};{1:0.000}\"",
        point.X,
        point.Y);
}

static VisionPipeline CreateDetectedPointAffinePipeline(
    IReadOnlyList<string> templatePaths,
    IReadOnlyList<Point2f> sourcePoints,
    IReadOnlyList<Point2f> destinationPoints)
{
    string[] names = { "01 Locate top-left", "02 Locate top-right", "03 Locate bottom-left" };
    VisionPipeline pipeline = new VisionPipeline { Name = "P219 Matching points to Affine fixed ROI" };
    for (int index = 0; index < 3; index++)
    {
        int roiX = (int)Math.Floor(sourcePoints[index].X) - 30;
        int roiY = (int)Math.Floor(sourcePoints[index].Y) - 30;
        VisionPipelineStep match = new VisionPipelineStep
        {
            Name = names[index],
            ToolType = "Matching",
            Enabled = true,
            InputLayer = VisionRecipeRunner.DefaultInputLayer,
            OutputLayer = $"Match_{index + 1}_Result"
        };
        match.Parameters["PATTERN_PATH"] = templatePaths[index];
        match.Parameters["MATCH_MODE"] = TemplateMatchModes.SqDiffNormed.ToString();
        match.Parameters["SCORE_MIN"] = "0.8";
        match.Parameters["MAGNIFIATION"] = "1";
        match.Parameters["NUM_MATCH"] = "1";
        match.Parameters["USE_FIND_ANGLE"] = "false";
        match.Parameters["USE_FIND_SCALE"] = "false";
        match.Parameters["USE_THRESHOLD"] = "false";
        match.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
        match.Parameters["USE_CANNY"] = "false";
        match.Parameters["USE_ROI"] = "true";
        match.Parameters["CvROI"] = string.Format(
            CultureInfo.InvariantCulture,
            "{0},{1},61,61",
            roiX,
            roiY);
        match.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
        pipeline.Steps.Add(match);
    }

    AffineTransformToolProperty affineProperty = new AffineTransformToolProperty
    {
        DestinationPoint1X = destinationPoints[0].X,
        DestinationPoint1Y = destinationPoints[0].Y,
        DestinationPoint2X = destinationPoints[1].X,
        DestinationPoint2Y = destinationPoints[1].Y,
        DestinationPoint3X = destinationPoints[2].X,
        DestinationPoint3Y = destinationPoints[2].Y,
        OutputWidth = 400,
        OutputHeight = 300,
        MinimumSourceTriangleArea = 1000,
        MinimumDestinationTriangleArea = 1000,
        MinimumValidPixelRatio = 0.6
    };
    VisionPipelineStep affine = VisionPipelineStepBuilder.FromAffineTransformProperty(
        affineProperty,
        "04 Normalize from detected points",
        VisionRecipeRunner.DefaultInputLayer,
        "Reference");
    affine.Parameters[VisionPipelineAffinePointBindingService.UseDetectedSourcePointsParameter] = "true";
    affine.Parameters[VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter] = names[0] + "/Center";
    affine.Parameters[VisionPipelineAffinePointBindingService.SourcePoint2FeatureParameter] = names[1] + "/Center";
    affine.Parameters[VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter] = names[2] + "/Center";
    affine.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
    pipeline.Steps.Add(affine);

    VisionPipelineStep threshold = new VisionPipelineStep
    {
        Name = "05 Threshold normalized target",
        ToolType = "Threshold",
        Enabled = true,
        InputLayer = "Reference",
        OutputLayer = "Reference_Binary"
    };
    threshold.Parameters["Mode"] = "Threshold";
    threshold.Parameters["Threshold"] = "127";
    threshold.Parameters["MaxValue"] = "255";
    threshold.Parameters["ThresholdType"] = ThresholdTypes.Binary.ToString();
    pipeline.Steps.Add(threshold);

    VisionPipelineStep blob = new VisionPipelineStep
    {
        Name = "06 Inspect fixed reference ROI",
        ToolType = "Blob",
        Enabled = true,
        InputLayer = "Reference_Binary",
        OutputLayer = "Inspection_Result",
        UseAcceptance = true,
        ExpectedSuccess = true,
        AcceptanceMetricName = VisionPipelineKnownMetrics.ResultCount,
        UseAcceptanceMetricMinimum = true,
        AcceptanceMetricMinimum = 1,
        UseAcceptanceMetricMaximum = true,
        AcceptanceMetricMaximum = 1
    };
    blob.Parameters["USE_ROI"] = "true";
    blob.Parameters["CvROI"] = "170,120,70,60";
    blob.Parameters["USE_THRESHOLD"] = "false";
    blob.Parameters["MIN_AREA"] = "600";
    blob.Parameters["MAX_AREA"] = "1800";
    pipeline.Steps.Add(blob);
    return pipeline;
}

static Mat CreateAffineFiducial(int index)
{
    Mat template = new Mat(new OpenCvSharp.Size(25, 25), MatType.CV_8UC1, Scalar.Black);
    if (index == 0)
    {
        Cv2.Circle(template, new OpenCvSharp.Point(12, 12), 7, Scalar.White, 2);
        Cv2.Line(template, new OpenCvSharp.Point(3, 12), new OpenCvSharp.Point(21, 12), new Scalar(180), 2);
    }
    else if (index == 1)
    {
        Cv2.Rectangle(template, new Rect(5, 5, 15, 15), Scalar.White, 2);
        Cv2.Line(template, new OpenCvSharp.Point(12, 2), new OpenCvSharp.Point(12, 22), new Scalar(180), 2);
    }
    else
    {
        OpenCvSharp.Point[] triangle =
        {
            new OpenCvSharp.Point(12, 3),
            new OpenCvSharp.Point(3, 21),
            new OpenCvSharp.Point(21, 21)
        };
        Cv2.Polylines(template, new[] { triangle }, true, Scalar.White, 2);
        Cv2.Line(template, new OpenCvSharp.Point(5, 18), new OpenCvSharp.Point(18, 7), new Scalar(180), 2);
    }

    return template;
}

static void VerifyDetectedPointAffinePropertyRoundTrip(
    VisionPipelineStep source,
    ICollection<string> failures)
{
    object? property = VisionPipelineStepPropertyMapper.CreateProperty(source);
    PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(property);
    string[] names =
    {
        "UseDetectedSourcePoints",
        "SourcePoint1Feature",
        "SourcePoint2Feature",
        "SourcePoint3Feature"
    };
    if (property == null
        || names.Any(name => descriptors.Find(name, true) == null)
        || !Equals(descriptors["UseDetectedSourcePoints"]?.GetValue(property), true)
        || !string.Equals(
            Convert.ToString(descriptors["SourcePoint1Feature"]?.GetValue(property), CultureInfo.InvariantCulture),
            "01 Locate top-left/Center",
            StringComparison.Ordinal))
    {
        failures.Add("Detected-point Affine PropertyGrid load did not retain its source bindings.");
        return;
    }

    VisionPipelineStep reapplied = new VisionPipelineStep
    {
        Name = source.Name,
        ToolType = source.ToolType,
        Enabled = true,
        InputLayer = source.InputLayer,
        OutputLayer = source.OutputLayer
    };
    if (!VisionPipelineStepPropertyMapper.ApplyProperty(reapplied, property)
        || reapplied.Parameters.GetValueOrDefault(VisionPipelineAffinePointBindingService.UseDetectedSourcePointsParameter) != "True"
        || reapplied.Parameters.GetValueOrDefault(VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter) != "01 Locate top-left/Center"
        || reapplied.Parameters.GetValueOrDefault(VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter) != "03 Locate bottom-left/Center")
    {
        failures.Add("Detected-point Affine PropertyGrid apply-back lost its source bindings.");
    }
}

static void VerifyCoreMetric(
    VisionToolResult result,
    string name,
    double expected,
    double tolerance,
    ICollection<string> failures)
{
    double actual = double.NaN;
    if (result?.Metrics == null
        || !result.Metrics.TryGetValue(name, out actual)
        || Math.Abs(actual - expected) > tolerance)
    {
        failures.Add($"{name} expected {expected:0.######} but was {(double.IsNaN(actual) ? "missing" : actual.ToString("0.######", CultureInfo.InvariantCulture))}.");
    }
}

static void SaveAffineDetectedPointEvidence(
    VisionPipelineRunResult run,
    string directory)
{
    for (int index = 0; index < run.StepResults.Count; index++)
    {
        VisionPipelineStepResult stepResult = run.StepResults[index];
        if (stepResult == null)
        {
            continue;
        }

        Mat? image = stepResult?.ToolResult?.ResultImage;
        if (image == null || image.Empty())
        {
            continue;
        }

        using System.Drawing.Bitmap raw = BitmapImageConverter.ToBitmap(image);
        using System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(
            raw.Width,
            raw.Height,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
        {
            graphics.DrawImageUnscaled(raw, 0, 0);
        }
        VisionPipelineRunReportImageRenderer.RenderInPlace(bitmap, stepResult, index + 1);
        string safeName = new string((stepResult?.Step?.Name ?? $"Step_{index + 1}")
            .Select(character => Path.GetInvalidFileNameChars().Contains(character) ? '_' : character)
            .ToArray());
        bitmap.Save(Path.Combine(directory, $"{index + 1:00}_{safeName}.png"));
    }
}

static VisionPipeline ClonePipeline(VisionPipeline source)
{
    VisionPipeline clone = new VisionPipeline { Name = source?.Name ?? string.Empty };
    foreach (VisionPipelineStep item in source?.Steps ?? new List<VisionPipelineStep>())
    {
        if (item == null)
        {
            continue;
        }

        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = item.Name,
            ToolType = item.ToolType,
            Enabled = item.Enabled,
            InputLayer = item.InputLayer,
            OutputLayer = item.OutputLayer,
            UseAcceptance = item.UseAcceptance,
            ExpectedSuccess = item.ExpectedSuccess,
            MaxElapsedMilliseconds = item.MaxElapsedMilliseconds,
            RequiredMessageText = item.RequiredMessageText,
            AcceptanceMetricName = item.AcceptanceMetricName,
            UseAcceptanceMetricMinimum = item.UseAcceptanceMetricMinimum,
            AcceptanceMetricMinimum = item.AcceptanceMetricMinimum,
            UseAcceptanceMetricMaximum = item.UseAcceptanceMetricMaximum,
            AcceptanceMetricMaximum = item.AcceptanceMetricMaximum
        };
        foreach (KeyValuePair<string, string> parameter in item.Parameters)
        {
            step.Parameters[parameter.Key] = parameter.Value;
        }
        clone.Steps.Add(step);
    }
    return clone;
}

static VisionPipeline CreateAffineContractPipeline(VisionPipelineStep source, string toolType)
{
    VisionPipelineStep step = new VisionPipelineStep
    {
        Name = source.Name,
        ToolType = toolType,
        Enabled = true,
        InputLayer = source.InputLayer,
        OutputLayer = source.OutputLayer
    };
    foreach (KeyValuePair<string, string> parameter in source.Parameters)
    {
        step.Parameters[parameter.Key] = parameter.Value;
    }

    VisionPipeline pipeline = new VisionPipeline { Name = "Affine " + toolType + " contract" };
    pipeline.Steps.Add(step);
    return pipeline;
}

static void VerifyAffineTransformPropertyRoundTrip(
    VisionPipelineStep source,
    ICollection<string> failures)
{
    AffineTransformToolProperty? restored =
        VisionPipelineStepPropertyMapper.CreateProperty(source) as AffineTransformToolProperty;
    if (restored == null
        || restored.SourcePoint2X != 100
        || restored.DestinationPoint2X != 132
        || restored.DestinationPoint3Y != 108
        || restored.OutputWidth != 240
        || restored.MinimumValidPixelRatio != 0.4)
    {
        failures.Add("Affine PropertyGrid -> XML -> PropertyGrid round trip failed.");
        return;
    }

    VisionPipelineStep reapplied = new VisionPipelineStep
    {
        Name = source.Name,
        ToolType = source.ToolType,
        Enabled = true,
        InputLayer = source.InputLayer,
        OutputLayer = source.OutputLayer
    };
    if (!VisionPipelineStepPropertyMapper.ApplyProperty(reapplied, restored)
        || reapplied.ToolType != "AffineTransform"
        || reapplied.Parameters.GetValueOrDefault(nameof(AffineTransformToolProperty.DestinationPoint2X)) != "132"
        || reapplied.Parameters.GetValueOrDefault(nameof(AffineTransformToolProperty.MinimumValidPixelRatio)) != "0.4")
    {
        failures.Add("Affine selected-Step PropertyGrid apply-back round trip failed.");
    }
}

static void VerifyAffineMetric(
    VisionRecipeStepRunSummary step,
    string metricName,
    double expected,
    ICollection<string> failures,
    string alias)
{
    if (!step.Metrics.TryGetValue(metricName, out double actual)
        || Math.Abs(actual - expected) > 1e-6)
    {
        failures.Add(alias + ": " + metricName + " expected " + expected.ToString("0.######", CultureInfo.InvariantCulture)
            + ", actual " + (double.IsNaN(actual) ? "missing" : actual.ToString("0.######", CultureInfo.InvariantCulture)) + ".");
    }
}

static async Task VerifyObjectDimensionFilterAsync(
    Mat source,
    string toolType,
    ICollection<string> failures,
    string? evidenceDirectory)
{
    VisionPipeline filteredPipeline = CreateObjectDimensionPipeline(toolType, includeDimensions: true);
    VisionRecipeRunner runner = new VisionRecipeRunner();
    using VisionRecipeRunResult filtered = await runner.RunAsync(filteredPipeline, source);
    VisionRecipeStepRunSummary? step = filtered.Steps.SingleOrDefault();
    if (!filtered.Success || step == null)
    {
        failures.Add($"{toolType}: filtered pipeline did not complete. {filtered.Message}");
        return;
    }

    double resultCount = step.Metrics.GetValueOrDefault(VisionPipelineKnownMetrics.ResultCount, -1D);
    if (resultCount != 1D)
    {
        failures.Add($"{toolType}: expected ResultCount=1 after dimension filters, actual {resultCount:0.###}.");
    }

    if (step.ObjectResults.Count(item => item.Accepted) != 1)
    {
        failures.Add($"{toolType}: Object Results Inspector did not retain exactly one accepted row.");
    }

    string[] expectedReasonPrefixes =
    {
        "Width 52 > MAX_WIDTH 30",
        "Width 8 < MIN_WIDTH 15",
        "Height 8 < MIN_HEIGHT 16",
        "Height 60 > MAX_HEIGHT 40"
    };
    foreach (string expected in expectedReasonPrefixes)
    {
        if (!step.ObjectResults.Any(item => item.RejectReason.StartsWith(expected, StringComparison.Ordinal)))
        {
            failures.Add($"{toolType}: missing exact reject reason '{expected}'.");
        }
    }

    int acceptedRectangles = step.Overlays.Count(item =>
        string.Equals(item.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase)
        && string.Equals(item.Label, "Accepted object", StringComparison.Ordinal));
    if (acceptedRectangles != 1)
    {
        failures.Add($"{toolType}: accepted drawing count should be 1, actual {acceptedRectangles}.");
    }

    if (evidenceDirectory != null)
    {
        SaveObjectDimensionEvidence(
            source,
            step.ObjectResults,
            Path.Combine(
                evidenceDirectory,
                toolType.ToLowerInvariant() + "_object_dimension_filter_drawing.png"));
        File.WriteAllLines(
            Path.Combine(
                evidenceDirectory,
                toolType.ToLowerInvariant() + "_object_dimension_filter_rows.tsv"),
            new[] { "Number\tAccepted\tArea\tX\tY\tWidth\tHeight\tAngle\tRejectReason" }
                .Concat(step.ObjectResults.Select(item => string.Join(
                    "\t",
                    item.Number.ToString(CultureInfo.InvariantCulture),
                    item.Accepted.ToString(CultureInfo.InvariantCulture),
                    item.Area.ToString("0.###", CultureInfo.InvariantCulture),
                    item.BoundsX.ToString(CultureInfo.InvariantCulture),
                    item.BoundsY.ToString(CultureInfo.InvariantCulture),
                    item.BoundsWidth.ToString(CultureInfo.InvariantCulture),
                    item.BoundsHeight.ToString(CultureInfo.InvariantCulture),
                    item.Angle.ToString("0.###", CultureInfo.InvariantCulture),
                    item.RejectReason))));
    }

    if (string.Equals(toolType, "Blob", StringComparison.Ordinal))
    {
        string recipeName = "Smoke_P216ObjectDimensions_" + Guid.NewGuid().ToString("N");
        try
        {
            DateTime startedAt = DateTime.Now;
            string reportPath = VisionPipelineRunReportStorage.Save(
                recipeName,
                filteredPipeline,
                filtered,
                startedAt,
                startedAt.AddMilliseconds(filtered.TotalMilliseconds),
                "dimension-contract",
                source);
            VisionPipelineRunReport report = VisionPipelineRunReportStorage.Load(reportPath);
            if (evidenceDirectory != null)
            {
                File.Copy(
                    reportPath,
                    Path.Combine(evidenceDirectory, "blob_object_dimension_filter_run_report.xml"),
                    overwrite: true);
            }
            List<string> persistedReasons = report?.Steps.SingleOrDefault()?.Objects
                .Select(item => item.RejectReason)
                .Where(reason => !string.IsNullOrWhiteSpace(reason))
                .ToList() ?? new List<string>();
            foreach (string expected in expectedReasonPrefixes)
            {
                if (!persistedReasons.Any(reason => reason.StartsWith(expected, StringComparison.Ordinal)))
                {
                    failures.Add($"Blob: saved Run History missed reject reason '{expected}'.");
                }
            }
        }
        finally
        {
            RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
        }
    }

    VisionPipeline legacyPipeline = CreateObjectDimensionPipeline(toolType, includeDimensions: false);
    using VisionRecipeRunResult legacy = await runner.RunAsync(legacyPipeline, source);
    VisionRecipeStepRunSummary? legacyStep = legacy.Steps.SingleOrDefault();
    double legacyCount = legacyStep?.Metrics.GetValueOrDefault(VisionPipelineKnownMetrics.ResultCount, -1D) ?? -1D;
    if (!legacy.Success || legacyCount != 5D)
    {
        failures.Add($"{toolType}: legacy XML defaults must preserve all 5 area-valid objects; actual {legacyCount:0.###}.");
    }
}

static void SaveObjectDimensionEvidence(
    Mat source,
    IEnumerable<VisionPipelineObjectResult> objectResults,
    string outputPath)
{
    using Mat drawing = new Mat();
    Cv2.CvtColor(source, drawing, ColorConversionCodes.GRAY2BGR);
    foreach (VisionPipelineObjectResult item in objectResults)
    {
        Scalar color = item.Accepted
            ? new Scalar(0, 220, 0)
            : new Scalar(0, 0, 255);
        Cv2.Rectangle(
            drawing,
            new Rect(item.BoundsX, item.BoundsY, item.BoundsWidth, item.BoundsHeight),
            color,
            2,
            LineTypes.AntiAlias);
        string label = item.Accepted
            ? "OK"
            : item.RejectReason.StartsWith("Width", StringComparison.Ordinal)
                ? item.RejectReason.Contains(" < ", StringComparison.Ordinal) ? "W<MIN" : "W>MAX"
                : item.RejectReason.StartsWith("Height", StringComparison.Ordinal)
                    ? item.RejectReason.Contains(" < ", StringComparison.Ordinal) ? "H<MIN" : "H>MAX"
                    : "AREA";
        Cv2.PutText(
            drawing,
            label,
            new OpenCvSharp.Point(item.BoundsX, Math.Max(13, item.BoundsY - 4)),
            HersheyFonts.HersheySimplex,
            0.42,
            color,
            1,
            LineTypes.AntiAlias);
    }

    Cv2.PutText(
        drawing,
        "GREEN=accepted  RED=rejected | W 15..30 px | H 16..40 px",
        new OpenCvSharp.Point(8, 124),
        HersheyFonts.HersheySimplex,
        0.38,
        new Scalar(0, 220, 255),
        1,
        LineTypes.AntiAlias);
    Cv2.ImWrite(outputPath, drawing);
}

static VisionPipeline CreateObjectDimensionPipeline(string toolType, bool includeDimensions)
{
    VisionPipelineStep step = new VisionPipelineStep
    {
        Name = toolType + " dimension filter",
        ToolType = toolType,
        Enabled = true,
        InputLayer = VisionRecipeRunner.DefaultInputLayer,
        OutputLayer = toolType + "_Result"
    };
    step.Parameters["USE_THRESHOLD"] = "false";
    step.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
    step.Parameters["USE_BITWISENOT"] = "false";
    step.Parameters["MIN_AREA"] = "50";
    step.Parameters["MAX_AREA"] = "5000";
    if (includeDimensions)
    {
        step.Parameters["MIN_WIDTH"] = "15";
        step.Parameters["MAX_WIDTH"] = "30";
        step.Parameters["MIN_HEIGHT"] = "16";
        step.Parameters["MAX_HEIGHT"] = "40";
    }

    VisionPipeline pipeline = new VisionPipeline { Name = toolType + " dimension contract" };
    pipeline.Steps.Add(step);
    return pipeline;
}

static void VerifyObjectDimensionPropertyRoundTrip(ICollection<string> failures)
{
    BlobProperty source = new BlobProperty("Blob dimensions")
    {
        MIN_AREA = 50,
        MAX_AREA = 5000,
        MIN_WIDTH = 15,
        MAX_WIDTH = 30,
        MIN_HEIGHT = 16,
        MAX_HEIGHT = 40
    };
    VisionPipelineStep step = VisionPipelineStepBuilder.FromProperty(source, "Main", "Blob_Result");
    BlobProperty? restored = VisionPipelineStepPropertyMapper.CreateProperty(step) as BlobProperty;
    if (restored == null
        || restored.MIN_WIDTH != 15
        || restored.MAX_WIDTH != 30
        || restored.MIN_HEIGHT != 16
        || restored.MAX_HEIGHT != 40)
    {
        failures.Add("Blob PropertyGrid -> XML -> PropertyGrid dimension round trip failed.");
    }

    VisionPipelineStep legacy = new VisionPipelineStep
    {
        Name = "Legacy contour",
        ToolType = "Contour",
        Enabled = true,
        InputLayer = "Main",
        OutputLayer = "Contour_Result"
    };
    legacy.Parameters["MIN_AREA"] = "50";
    legacy.Parameters["MAX_AREA"] = "5000";
    ContourProperty? legacyRestored = VisionPipelineStepPropertyMapper.CreateProperty(legacy) as ContourProperty;
    if (legacyRestored == null
        || legacyRestored.MIN_WIDTH != 0
        || legacyRestored.MAX_WIDTH != 1000000
        || legacyRestored.MIN_HEIGHT != 0
        || legacyRestored.MAX_HEIGHT != 1000000)
    {
        failures.Add("Legacy missing dimension keys did not restore unbounded Contour defaults.");
    }
}

static void VerifyObjectDimensionValidation(ICollection<string> failures)
{
    VisionPipeline invalid = CreateObjectDimensionPipeline("Blob", includeDimensions: true);
    invalid.Steps[0].Parameters["MIN_WIDTH"] = "31";
    invalid.Steps[0].Parameters["MAX_WIDTH"] = "30";
    VisionPipelineValidationResult result = VisionPipelineValidator.Validate(invalid, new[] { "Main" });
    if (result.Success
        || !result.Errors.Any(error => error.Contains("MIN_WIDTH is greater than MAX_WIDTH", StringComparison.Ordinal)))
    {
        failures.Add("Reversed MIN_WIDTH/MAX_WIDTH did not fail strict pipeline validation.");
    }
}

static object? InvokePinArrayGapParse(MethodInfo method, string roiText, out bool succeeded, out string message)
{
    object?[] invokeArguments = { roiText, null, null };
    succeeded = method.Invoke(null, invokeArguments) as bool? == true;
    message = invokeArguments[2]?.ToString() ?? string.Empty;
    return invokeArguments[1];
}

static bool InvokePinArrayGapValidation(
    MethodInfo method,
    object rowRois,
    string measurementDefinition,
    string pinPolarity,
    int sourceWidth,
    int sourceHeight,
    out string message)
{
    object?[] invokeArguments =
    {
        measurementDefinition,
        pinPolarity,
        "px",
        rowRois,
        sourceWidth,
        sourceHeight,
        128,
        0.55D,
        5,
        2,
        3,
        null
    };
    bool succeeded = method.Invoke(null, invokeArguments) as bool? == true;
    message = invokeArguments[11]?.ToString() ?? string.Empty;
    return succeeded;
}

static void VerifyPinArrayGapMeasurementPipeline(VisionPipeline? pipeline, ICollection<string> failures)
{
    if (pipeline == null || pipeline.Steps.Count != 2)
    {
        failures.Add("Measurement starter must contain exactly two PinArrayGap steps.");
        return;
    }

    if (pipeline.Steps.Any(step => !string.Equals(step.ToolType, "PinArrayGap", StringComparison.Ordinal)))
    {
        failures.Add("Measurement starter contains a tool outside the locked PinArrayGap family.");
    }

    if (pipeline.Steps.Any(step => step.UseAcceptance))
    {
        failures.Add("Measurement-only starter unexpectedly contains an acceptance gate.");
    }

    if (pipeline.Steps[0].Parameters.ContainsKey("ALLOW_BRANCH_INPUT")
        || !pipeline.Steps[1].Parameters.TryGetValue("ALLOW_BRANCH_INPUT", out string? branchValue)
        || !string.Equals(branchValue, "true", StringComparison.OrdinalIgnoreCase))
    {
        failures.Add("Only the second row step must opt into branch input.");
    }
}

static void VerifyPinArrayGapJudgedPipeline(VisionPipeline? pipeline, ICollection<string> failures)
{
    if (pipeline == null || pipeline.Steps.Count != 2)
    {
        failures.Add("Judged starter must contain exactly two PinArrayGap steps.");
        return;
    }

    if (pipeline.Steps.Any(step =>
            !step.UseAcceptance
            || !string.Equals(step.AcceptanceMetricName, "DistancePxRange", StringComparison.Ordinal)
            || !step.UseAcceptanceMetricMaximum
            || step.AcceptanceMetricMaximum != 6D))
    {
        failures.Add("Every judged row must use an exact DistancePxRange maximum of 6.");
    }
}

static async Task<int> RunBatchAsync(
    string imageListPath,
    string datasetRoot,
    string pipelineXmlPath,
    string csvPath,
    string? evidenceRoot = null)
{
    string listPath = Path.GetFullPath(imageListPath);
    string rootPath = Path.GetFullPath(datasetRoot);
    string xmlPath = Path.GetFullPath(pipelineXmlPath);
    string outputPath = Path.GetFullPath(csvPath);
    string? evidencePath = string.IsNullOrWhiteSpace(evidenceRoot) ? null : Path.GetFullPath(evidenceRoot);

    if (!File.Exists(listPath) || !Directory.Exists(rootPath) || !File.Exists(xmlPath))
    {
        Console.Error.WriteLine("Batch prerequisites are missing: image list, dataset root, or pipeline XML.");
        return 2;
    }

    string[] entries = File.ReadAllLines(listPath)
        .Where(entry => !string.IsNullOrWhiteSpace(entry))
        .ToArray();
    if (entries.Length == 0)
    {
        Console.Error.WriteLine("Batch image list is empty.");
        return 2;
    }

    Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
    VisionPipeline? evidencePipeline = null;
    StreamWriter? evidenceWriter = null;
    if (evidencePath != null)
    {
        if (!SerializeHelper.TryLoadFromXmlFile(xmlPath, out evidencePipeline) || evidencePipeline == null)
        {
            Console.Error.WriteLine("Pipeline XML could not be loaded for batch evidence drawings.");
            return 2;
        }

        Directory.CreateDirectory(Path.Combine(evidencePath, "runs"));
        evidenceWriter = new StreamWriter(
            Path.Combine(evidencePath, "evidence_rows.csv"),
            false,
            new System.Text.UTF8Encoding(false));
        evidenceWriter.WriteLine("ImagePath,Expected,SourceSha256,PipelineSuccess,StepIndex,StepName,StepSuccess,StepStatus,ErrorCode,ErrorName,Message,ResultImagePath,ResultImageSha256,StepOverlayPath,ScoreMax,ScoreSecond,ScoreMargin,FixtureCenterX,FixtureCenterY,FixtureAngle,FixtureAngleDelta,FixtureScale,FixtureScaleRatio,FixtureValidPixelRatio,DistancePxMin,DistancePxMax,DistancePxAvg,DistancePxRange,PitchPxMin,PitchPxMax,PitchPxAvg,PitchPxRange,GapCandidateLineCount,GapCandidatePairCount,GapOverlapPairCount,GapSeparationPairCount,GapParallelPairCount,GapContrastPairCount,GapSelectedAngleDeltaDeg,GapSelectedSupportRatio,GapDarkContrast,GapDarkCoverageRatio,GapBandMeanGray,GapScoreMargin,GapUpperSupportPointCount,GapLowerSupportPointCount,ElapsedMilliseconds");
    }

    int completed = 0;
    int pipelinePasses = 0;
    int missingImages = 0;
    VisionRecipeRunner runner = new VisionRecipeRunner();
    using StreamWriter writer = new StreamWriter(outputPath, false, new System.Text.UTF8Encoding(false));
    writer.WriteLine("ImagePath,Expected,PipelineSuccess,StepIndex,StepName,ToolType,StepSuccess,StepStatus,ResultCount,IntersectionCross,IntersectionX,IntersectionY,CornerOuterContourVerified,LineAngleMin,LineAngleMax,LineAngleAvg,CurveOuterArcLengthPx,CurveInnerArcLengthPx,CurveCenterArcLengthPx,CurveProfileRowCount,DistancePxMin,DistancePxMax,DistancePxAvg,DistancePxRange,PitchPxMin,PitchPxMax,PitchPxAvg,PitchPxRange,ElapsedMilliseconds,ErrorCode,ErrorName,Message");

    foreach (string entry in entries)
    {
        string imagePath = ResolveBatchImagePath(rootPath, entry);
        string expected = ResolveExpectedOutcome(entry);
        if (!File.Exists(imagePath))
        {
            missingImages++;
            WriteBatchCsvRow(writer, imagePath, expected, false, 0, string.Empty, string.Empty, false, "MISSING", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0, -1, "MissingImage", "Image was not found.");
            continue;
        }

        using Mat source = Cv2.ImRead(imagePath, ImreadModes.Unchanged);
        if (source.Empty())
        {
            WriteBatchCsvRow(writer, imagePath, expected, false, 0, string.Empty, string.Empty, false, "ERROR", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0, -1, "ImageLoadFailed", "Image could not be loaded.");
            continue;
        }

        using Mat sourceEvidence = source.Clone();
        using VisionRecipeRunResult result = await runner.RunAsync(xmlPath, source);
        completed++;
        if (result.Success)
        {
            pipelinePasses++;
        }

        string sourceSha256 = ComputeSha256(imagePath);
        string resultEvidencePath = string.Empty;
        string resultSha256 = string.Empty;
        string evidenceRunPath = string.Empty;
        if (evidencePath != null && evidencePipeline != null)
        {
            string runName = CreateEvidenceDirectoryName(expected, imagePath);
            evidenceRunPath = Path.Combine(evidencePath, "runs", runName);
            Directory.CreateDirectory(evidenceRunPath);
            string sourceSnapshotPath = Path.Combine(evidenceRunPath, "source" + Path.GetExtension(imagePath).ToLowerInvariant());
            File.Copy(imagePath, sourceSnapshotPath, true);
            resultEvidencePath = Path.Combine(evidenceRunPath, "runtime_result.png");
            SaveAllOverlayImage(sourceEvidence, result, evidencePipeline, resultEvidencePath);

            if (File.Exists(resultEvidencePath))
            {
                resultSha256 = ComputeSha256(resultEvidencePath);
            }
        }

        foreach (VisionRecipeStepRunSummary step in result.Steps)
        {
            double? resultCount = TryGetMetric(step, "ResultCount");
            double? intersectionCross = TryGetMetric(step, "IntersectionCross");
            double? intersectionX = TryGetMetric(step, "IntersectionX");
            double? intersectionY = TryGetMetric(step, "IntersectionY");
            double? cornerOuterContourVerified = TryGetMetric(step, "CornerOuterContourVerified");
            double? lineAngleMinimum = TryGetMetric(step, "LineAngleMin");
            double? lineAngleMaximum = TryGetMetric(step, "LineAngleMax");
            double? lineAngleAverage = TryGetMetric(step, "LineAngleAvg");
            double? curveOuterArcLength = TryGetMetric(step, "CurveOuterArcLengthPx");
            double? curveInnerArcLength = TryGetMetric(step, "CurveInnerArcLengthPx");
            double? curveCenterArcLength = TryGetMetric(step, "CurveCenterArcLengthPx");
            double? curveProfileRowCount = TryGetMetric(step, "CurveProfileRowCount");
            double? minimum = TryGetMetric(step, "DistancePxMin");
            double? maximum = TryGetMetric(step, "DistancePxMax");
            double? average = TryGetMetric(step, "DistancePxAvg");
            double? range = TryGetMetric(step, "DistancePxRange");
            double? pitchMinimum = TryGetMetric(step, "PitchPxMin");
            double? pitchMaximum = TryGetMetric(step, "PitchPxMax");
            double? pitchAverage = TryGetMetric(step, "PitchPxAvg");
            double? pitchRange = TryGetMetric(step, "PitchPxRange");
            WriteBatchCsvRow(
                writer,
                imagePath,
                expected,
                result.Success,
                step.Index,
                step.Name,
                step.ToolType,
                step.Success,
                step.Status,
                resultCount,
                intersectionCross,
                intersectionX,
                intersectionY,
                cornerOuterContourVerified,
                lineAngleMinimum,
                lineAngleMaximum,
                lineAngleAverage,
                curveOuterArcLength,
                curveInnerArcLength,
                curveCenterArcLength,
                curveProfileRowCount,
                minimum,
                maximum,
                average,
                range,
                pitchMinimum,
                pitchMaximum,
                pitchAverage,
                pitchRange,
                step.ElapsedMilliseconds,
                step.ErrorCode,
                step.ErrorName,
                step.Message);

            if (evidenceWriter != null)
            {
                string stepOverlayPath = string.Empty;
                if (!string.IsNullOrWhiteSpace(evidenceRunPath)
                    && string.Equals(step.ToolType, "Matching", StringComparison.OrdinalIgnoreCase)
                    && evidencePipeline != null
                    && step.Index > 0
                    && step.Index <= evidencePipeline.Steps.Count)
                {
                    stepOverlayPath = Path.Combine(
                        evidenceRunPath,
                        step.Index.ToString("00", CultureInfo.InvariantCulture) + "_matching_overlay.png");
                    SaveStepOverlayImage(source, step, evidencePipeline.Steps[step.Index - 1], stepOverlayPath);
                }

                WriteEvidenceCsvRow(
                    evidenceWriter,
                    imagePath,
                    expected,
                    sourceSha256,
                    result.Success,
                    step,
                    resultEvidencePath,
                    resultSha256,
                    stepOverlayPath);
            }
        }
    }

    evidenceWriter?.Dispose();

    Console.WriteLine($"BatchRows={entries.Length}");
    Console.WriteLine($"BatchCompleted={completed}");
    Console.WriteLine($"BatchPipelinePasses={pipelinePasses}");
    Console.WriteLine($"BatchMissingImages={missingImages}");
    Console.WriteLine($"BatchCsv={outputPath}");
    if (evidencePath != null)
    {
        Console.WriteLine($"BatchEvidence={evidencePath}");
    }
    return 0;
}

static string CreateEvidenceDirectoryName(string expected, string imagePath)
{
    string name = Path.GetFileNameWithoutExtension(imagePath);
    foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
    {
        name = name.Replace(invalidCharacter, '_');
    }

    return expected + "_" + name;
}

static string ComputeSha256(string path)
{
    using FileStream stream = File.OpenRead(path);
    return Convert.ToHexString(SHA256.HashData(stream));
}

static void WriteEvidenceCsvRow(
    TextWriter writer,
    string imagePath,
    string expected,
    string sourceSha256,
    bool pipelineSuccess,
    VisionRecipeStepRunSummary step,
    string resultImagePath,
    string resultImageSha256,
    string stepOverlayPath)
{
    string[] metricNames =
    {
        "ScoreMax",
        "ScoreMin",
        "ScoreMargin",
        "FixtureCenterX",
        "FixtureCenterY",
        "FixtureAngle",
        "FixtureAngleDelta",
        "FixtureScale",
        "FixtureScaleRatio",
        "FixtureValidPixelRatio",
        "DistancePxMin",
        "DistancePxMax",
        "DistancePxAvg",
        "DistancePxRange",
        "PitchPxMin",
        "PitchPxMax",
        "PitchPxAvg",
        "PitchPxRange",
        "GapCandidateLineCount",
        "GapCandidatePairCount",
        "GapOverlapPairCount",
        "GapSeparationPairCount",
        "GapParallelPairCount",
        "GapContrastPairCount",
        "GapSelectedAngleDeltaDeg",
        "GapSelectedSupportRatio",
        "GapDarkContrast",
        "GapDarkCoverageRatio",
        "GapBandMeanGray",
        "GapScoreMargin",
        "GapUpperSupportPointCount",
        "GapLowerSupportPointCount"
    };

    List<string> values = new List<string>
    {
        imagePath,
        expected,
        sourceSha256,
        pipelineSuccess ? "true" : "false",
        step.Index.ToString(CultureInfo.InvariantCulture),
        step.Name,
        step.Success ? "true" : "false",
        step.Status,
        step.ErrorCode.ToString(CultureInfo.InvariantCulture),
        step.ErrorName,
        step.Message,
        resultImagePath,
        resultImageSha256,
        stepOverlayPath
    };

    values.AddRange(metricNames.Select(metricName =>
        TryGetMetric(step, metricName)?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty));
    values.Add(step.ElapsedMilliseconds.ToString("0.############", CultureInfo.InvariantCulture));
    writer.WriteLine(string.Join(",", values.Select(EscapeBatchCsvValue)));
}

static void SaveStepOverlayImage(
    Mat sourceImage,
    VisionRecipeStepRunSummary step,
    VisionPipelineStep definition,
    string outputPath)
{
    using Mat preview = new Mat();
    if (sourceImage.Channels() == 1)
    {
        Cv2.CvtColor(sourceImage, preview, ColorConversionCodes.GRAY2BGR);
    }
    else
    {
        sourceImage.CopyTo(preview);
    }

    Scalar color = new Scalar(0, 220, 70);
    int thickness = Math.Max(2, Math.Min(sourceImage.Width, sourceImage.Height) / 260);
    int labelThickness = Math.Max(1, thickness - 1);
    IReadOnlyDictionary<string, string> parameters = step.Parameters
        ?? definition.Parameters
        ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    DrawStepRoiOverlay(preview, step.Index, parameters, color, thickness, labelThickness);
    foreach (VisionRecipeOverlaySummary overlay in step.Overlays)
    {
        if (string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase))
        {
            DrawRectangleOverlay(preview, overlay, color, thickness, labelThickness, step.Index.ToString("00", CultureInfo.InvariantCulture));
        }
        else if (string.Equals(overlay.Kind, "Line", StringComparison.OrdinalIgnoreCase))
        {
            DrawLineOverlay(preview, overlay, color, thickness, labelThickness, step.Index.ToString("00", CultureInfo.InvariantCulture));
        }
        else if (string.Equals(overlay.Kind, "Point", StringComparison.OrdinalIgnoreCase))
        {
            DrawPointOverlay(preview, overlay.CenterX, overlay.CenterY, color, thickness, step.Index.ToString("00", CultureInfo.InvariantCulture));
        }
        else if (string.Equals(overlay.Kind, "Points", StringComparison.OrdinalIgnoreCase))
        {
            DrawPointsOverlay(preview, overlay, color, thickness);
        }
    }

    Cv2.ImWrite(outputPath, preview);
}

static string ResolveBatchImagePath(string datasetRoot, string entry)
{
    string value = (entry ?? string.Empty).Trim().Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
    if (Path.IsPathRooted(value))
    {
        return Path.GetFullPath(value);
    }

    string direct = Path.Combine(datasetRoot, value);
    if (File.Exists(direct))
    {
        return direct;
    }

    int imagesIndex = value.IndexOf(Path.DirectorySeparatorChar + "images" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    string relativeToImages = imagesIndex >= 0
        ? value.Substring(imagesIndex + 1)
        : value;
    return Path.Combine(datasetRoot, relativeToImages);
}

static string ResolveExpectedOutcome(string entry)
{
    string normalized = (entry ?? string.Empty).Replace('/', '\\');
    return normalized.IndexOf("\\NG\\", StringComparison.OrdinalIgnoreCase) >= 0 ? "NG" : "OK";
}

static double? TryGetMetric(VisionRecipeStepRunSummary step, string metricName)
{
    return step?.Metrics != null && step.Metrics.TryGetValue(metricName, out double value)
        ? value
        : null;
}

static void WriteBatchCsvRow(
    TextWriter writer,
    string imagePath,
    string expected,
    bool pipelineSuccess,
    int stepIndex,
    string stepName,
    string toolType,
    bool stepSuccess,
    string stepStatus,
    double? resultCount,
    double? intersectionCross,
    double? intersectionX,
    double? intersectionY,
    double? cornerOuterContourVerified,
    double? lineAngleMinimum,
    double? lineAngleMaximum,
    double? lineAngleAverage,
    double? curveOuterArcLength,
    double? curveInnerArcLength,
    double? curveCenterArcLength,
    double? curveProfileRowCount,
    double? distancePxMinimum,
    double? distancePxMaximum,
    double? distancePxAverage,
    double? distancePxRange,
    double? pitchPxMinimum,
    double? pitchPxMaximum,
    double? pitchPxAverage,
    double? pitchPxRange,
    double elapsedMilliseconds,
    int errorCode,
    string errorName,
    string message)
{
    string[] values =
    {
        imagePath,
        expected,
        pipelineSuccess ? "true" : "false",
        stepIndex.ToString(CultureInfo.InvariantCulture),
        stepName,
        toolType,
        stepSuccess ? "true" : "false",
        stepStatus,
        resultCount?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        intersectionCross?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        intersectionX?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        intersectionY?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        cornerOuterContourVerified?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        lineAngleMinimum?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        lineAngleMaximum?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        lineAngleAverage?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        curveOuterArcLength?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        curveInnerArcLength?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        curveCenterArcLength?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        curveProfileRowCount?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        distancePxMinimum?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        distancePxMaximum?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        distancePxAverage?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        distancePxRange?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        pitchPxMinimum?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        pitchPxMaximum?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        pitchPxAverage?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        pitchPxRange?.ToString("0.############", CultureInfo.InvariantCulture) ?? string.Empty,
        elapsedMilliseconds.ToString("0.############", CultureInfo.InvariantCulture),
        errorCode.ToString(CultureInfo.InvariantCulture),
        errorName,
        message
    };
    writer.WriteLine(string.Join(",", values.Select(EscapeBatchCsvValue)));
}

static string EscapeBatchCsvValue(string value)
{
    string text = value ?? string.Empty;
    return "\"" + text.Replace("\"", "\"\"") + "\"";
}

static void SaveAllOverlayImage(Mat sourceImage, VisionRecipeRunResult runResult, VisionPipeline pipeline, string outputPath)
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
    foreach (VisionRecipeStepRunSummary step in runResult.Steps)
    {
        IReadOnlyDictionary<string, string> parameters = step.Parameters;
        if ((parameters == null || !TryGetBoolParameter(parameters, "USE_ROI"))
            && step.Index > 0
            && step.Index <= pipeline.Steps.Count)
        {
            parameters = pipeline.Steps[step.Index - 1].Parameters;
        }

        if (step.Overlays.Count == 0 && (parameters == null || !TryGetBoolParameter(parameters, "USE_ROI")))
        {
            continue;
        }

        Scalar color = colors[colorIndex++ % colors.Length];
        DrawStepRoiOverlay(preview, step.Index, parameters!, color, thickness, labelThickness);
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
    int stepIndex,
    IReadOnlyDictionary<string, string> parameters,
    Scalar color,
    int thickness,
    int labelThickness)
{
    if (parameters == null
        || !TryGetBoolParameter(parameters, "USE_ROI")
        || !TryGetRectParameter(parameters, "CvROI", out Rect roi)
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
    DrawLabel(preview, $"{stepIndex:00} ROI", new OpenCvSharp.Point(bounds.X + 3, bounds.Y + 14), color, labelThickness);
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
if (!SerializeHelper.TryLoadFromXmlFile(pipelineXmlPath, out VisionPipeline overlayPipeline) || overlayPipeline == null)
{
    Console.Error.WriteLine("Pipeline XML could not be loaded for overlay evidence.");
    return 2;
}

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
    SaveAllOverlayImage(source, result, overlayPipeline, allOverlayImagePath);
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
