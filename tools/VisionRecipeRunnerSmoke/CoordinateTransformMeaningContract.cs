using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal static class CoordinateTransformMeaningContract
{
    private const double CoordinateTolerance = 1e-4D;

    public static async Task<int> RunAsync(string evidenceDirectory)
    {
        string outputDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(outputDirectory);
        Environment.SetEnvironmentVariable(
            AppPathService.DataRootEnvironmentVariable,
            Path.Combine(outputDirectory, "runtime-data"));

        List<string> observations = new List<string>();
        List<string> failures = new List<string>();
        VisionRecipeRunner runner = new VisionRecipeRunner();

        using Mat source = CreateSourceImage();
        Cv2.ImWrite(Path.Combine(outputDirectory, "coordinate_transform_source.png"), source);

        Point2f[] sourcePoints =
        {
            new Point2f(20F, 20F),
            new Point2f(100F, 20F),
            new Point2f(20F, 100F)
        };

        TransformCase[] cases =
        {
            new TransformCase(
                "identity",
                new Point2f(20F, 20F),
                new Point2f(100F, 20F),
                new Point2f(20F, 100F)),
            new TransformCase(
                "translation",
                new Point2f(35F, 25F),
                new Point2f(115F, 25F),
                new Point2f(35F, 105F)),
            new TransformCase(
                "rotation-90",
                new Point2f(100F, 20F),
                new Point2f(100F, 100F),
                new Point2f(20F, 20F)),
            new TransformCase(
                "horizontal-flip",
                new Point2f(140F, 20F),
                new Point2f(60F, 20F),
                new Point2f(140F, 100F))
        };

        foreach (TransformCase transformCase in cases)
        {
            try
            {
                await VerifyAffineCaseAsync(
                    transformCase,
                    sourcePoints,
                    source,
                    runner,
                    outputDirectory,
                    observations).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                failures.Add(transformCase.Name + ": " + exception.GetBaseException().Message);
            }
        }

        try
        {
            await VerifyAnisotropicPixelOnlyGateAsync(source, runner, observations).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            failures.Add("anisotropic-pixel-only: " + exception.GetBaseException().Message);
        }

        try
        {
            VerifyFixtureTranslationAndFailureGates(observations);
        }
        catch (Exception exception)
        {
            failures.Add("fixture-translation: " + exception.GetBaseException().Message);
        }

        try
        {
            VerifySingularAffineGate(sourcePoints, failures, observations);
        }
        catch (Exception exception)
        {
            failures.Add("singular-affine: " + exception.GetBaseException().Message);
        }

        string reportPath = Path.Combine(outputDirectory, "coordinate-transform-meaning-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-030 ROI and fixture/affine coordinate meaning",
                "Coordinate frame: pixel coordinates remain explicit; no scalar mm/px inference for anisotropic transforms.",
                "EvidenceDirectory: " + outputDirectory
            }
            .Concat(observations.Select(item => "PASS: " + item))
            .Concat(failures.Select(item => "FAIL: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Coordinate transform meaning contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Coordinate transform meaning contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static async Task VerifyAffineCaseAsync(
        TransformCase transformCase,
        Point2f[] sourcePoints,
        Mat source,
        VisionRecipeRunner runner,
        string evidenceDirectory,
        ICollection<string> observations)
    {
        Point2f[] destinationPoints = transformCase.DestinationPoints;
        AffineTransformProperty property = new AffineTransformProperty(transformCase.Name)
        {
            SourcePoint1X = sourcePoints[0].X,
            SourcePoint1Y = sourcePoints[0].Y,
            SourcePoint2X = sourcePoints[1].X,
            SourcePoint2Y = sourcePoints[1].Y,
            SourcePoint3X = sourcePoints[2].X,
            SourcePoint3Y = sourcePoints[2].Y,
            DestinationPoint1X = destinationPoints[0].X,
            DestinationPoint1Y = destinationPoints[0].Y,
            DestinationPoint2X = destinationPoints[1].X,
            DestinationPoint2Y = destinationPoints[1].Y,
            DestinationPoint3X = destinationPoints[2].X,
            DestinationPoint3Y = destinationPoints[2].Y,
            OutputWidth = source.Width,
            OutputHeight = source.Height,
            MinimumSourceTriangleArea = 1D,
            MinimumDestinationTriangleArea = 1D,
            MinimumValidPixelRatio = 0.1D
        };

        VisionPipelineStep step = VisionPipelineStepBuilder.FromAffineTransformProperty(
            property,
            transformCase.Name,
            VisionRecipeRunner.DefaultInputLayer,
            "Affine_" + transformCase.Name);
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-030 " + transformCase.Name };
        pipeline.Steps.Add(step);
        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            pipeline,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        Require(validation.Success, "validation failed: " + string.Join(" | ", validation.Errors));

        using VisionRecipeRunResult run = await runner.RunAsync(pipeline, source).ConfigureAwait(false);
        VisionRecipeStepRunSummary? summary = run.Steps.SingleOrDefault();
        Require(run.Success && summary != null, "runtime failed: " + run.Message);
        VisionRecipeStepRunSummary verifiedSummary = summary!;
        Require(verifiedSummary.ResultImageWidth == source.Width && verifiedSummary.ResultImageHeight == source.Height,
            "transformed output size changed unexpectedly.");

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
                AssertMetric(
                    verifiedSummary.Metrics,
                    matrixMetrics[row * 3 + column],
                    expectedMatrix.At<double>(row, column),
                    CoordinateTolerance,
                    transformCase.Name);
            }
        }

        Require(verifiedSummary.Metrics.TryGetValue(VisionPipelineKnownMetrics.AffineValidPixelRatio, out double validRatio)
            && double.IsFinite(validRatio)
            && validRatio > 0D,
            "AffineValidPixelRatio was not a positive finite metric.");

        List<VisionRecipeOverlaySummary> pointOverlays = verifiedSummary.Overlays
            .Where(item => string.Equals(item.Kind, "Point", StringComparison.OrdinalIgnoreCase))
            .ToList();
        Require(pointOverlays.Count == 3, "expected three transformed point overlays, got " + pointOverlays.Count + ".");
        foreach (Point2f expectedPoint in destinationPoints)
        {
            Require(pointOverlays.Any(item =>
                    Math.Abs(item.CenterX - expectedPoint.X) <= 0.01F
                    && Math.Abs(item.CenterY - expectedPoint.Y) <= 0.01F),
                $"transformed point ({expectedPoint.X},{expectedPoint.Y}) was not retained in the overlay summary.");
        }

        string pipelinePath = Path.Combine(evidenceDirectory, transformCase.Name + ".pipeline.xml");
        Require(VisionPipelineStorage.TrySaveToFile(pipelinePath, pipeline, out string saveMessage), saveMessage);
        Require(VisionPipelineStorage.TryLoadFromFile(pipelinePath, out VisionPipeline reopened, out string loadMessage), loadMessage);
        using VisionRecipeRunResult reopenedRun = await runner.RunAsync(reopened, source).ConfigureAwait(false);
        VisionRecipeStepRunSummary? reopenedSummary = reopenedRun.Steps.SingleOrDefault();
        Require(reopenedRun.Success && reopenedSummary != null, "save/reopen runtime failed: " + reopenedRun.Message);
        VisionRecipeStepRunSummary verifiedReopenedSummary = reopenedSummary!;
        CompareMatrixMetrics(verifiedSummary, verifiedReopenedSummary, matrixMetrics, transformCase.Name + " save/reopen");
        Require(OverlayCoordinatesEqual(verifiedSummary.Overlays, verifiedReopenedSummary.Overlays),
            "save/reopen changed transformed overlay coordinates.");

        string reportPath = VisionPipelineRunReportStorage.Save(
            "2D-030",
            reopened,
            reopenedRun,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMilliseconds(Math.Max(1D, reopenedRun.TotalMilliseconds)),
            transformCase.Name,
            source);
        VisionPipelineRunReport? report = VisionPipelineRunReportStorage.Load(reportPath);
        VisionPipelineStepRunReport? persistedStep = report?.Steps.SingleOrDefault();
        Require(report != null && persistedStep != null, "run report did not reload.");
        VisionPipelineStepRunReport verifiedPersistedStep = persistedStep!;
        Require(verifiedPersistedStep.OverlayCount == verifiedReopenedSummary.OverlayCount,
            "run report changed overlay count.");
        Require(verifiedPersistedStep.Metrics.Any(item =>
                string.Equals(item.Name, VisionPipelineKnownMetrics.AffineM13, StringComparison.OrdinalIgnoreCase)
                && Math.Abs(item.Value - verifiedReopenedSummary.Metrics[VisionPipelineKnownMetrics.AffineM13]) <= CoordinateTolerance),
            "run report did not preserve affine translation metric.");
        Require(!string.IsNullOrWhiteSpace(verifiedPersistedStep.OverlayImageFile)
            && File.Exists(Path.Combine(Path.GetDirectoryName(reportPath)!, verifiedPersistedStep.OverlayImageFile)),
            "run report did not export an overlay image.");

        string evidenceResultPath = Path.Combine(evidenceDirectory, transformCase.Name + "_result.png");
        if (reopenedRun.ResultImage != null && !reopenedRun.ResultImage.Empty())
        {
            Cv2.ImWrite(evidenceResultPath, reopenedRun.ResultImage);
        }

        observations.Add(
            $"Affine {transformCase.Name}: matrix, three point overlays, XML round-trip, report metrics, and overlay export preserved (validRatio={validRatio.ToString("0.###", CultureInfo.InvariantCulture)}).");
    }

    private static async Task VerifyAnisotropicPixelOnlyGateAsync(
        Mat source,
        VisionRecipeRunner runner,
        ICollection<string> observations)
    {
        AffineTransformProperty property = new AffineTransformProperty("anisotropic")
        {
            SourcePoint1X = 20D,
            SourcePoint1Y = 20D,
            SourcePoint2X = 100D,
            SourcePoint2Y = 20D,
            SourcePoint3X = 20D,
            SourcePoint3Y = 100D,
            DestinationPoint1X = 20D,
            DestinationPoint1Y = 20D,
            DestinationPoint2X = 140D,
            DestinationPoint2Y = 20D,
            DestinationPoint3X = 20D,
            DestinationPoint3Y = 80D,
            OutputWidth = source.Width,
            OutputHeight = source.Height,
            MinimumSourceTriangleArea = 1D,
            MinimumDestinationTriangleArea = 1D,
            MinimumValidPixelRatio = 0.1D
        };
        VisionPipelineStep step = VisionPipelineStepBuilder.FromAffineTransformProperty(
            property,
            "anisotropic",
            VisionRecipeRunner.DefaultInputLayer,
            "Affine_anisotropic");
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-030 anisotropic" };
        pipeline.Steps.Add(step);
        using VisionRecipeRunResult run = await runner.RunAsync(pipeline, source).ConfigureAwait(false);
        VisionRecipeStepRunSummary? summary = run.Steps.SingleOrDefault();
        Require(run.Success && summary != null, "anisotropic affine runtime failed: " + run.Message);
        VisionRecipeStepRunSummary verifiedSummary = summary!;
        Require(verifiedSummary.Metrics.TryGetValue(VisionPipelineKnownMetrics.AffineScaleX, out double scaleX)
            && verifiedSummary.Metrics.TryGetValue(VisionPipelineKnownMetrics.AffineScaleY, out double scaleY)
            && Math.Abs(scaleX - scaleY) > CoordinateTolerance,
            "anisotropic affine did not retain distinct pixel scale axes.");
        Require(!verifiedSummary.Metrics.Keys.Any(name =>
                name.Contains("Mm", StringComparison.OrdinalIgnoreCase)),
            "anisotropic affine fabricated a scalar millimeter metric without PIXELPERMM.");
        observations.Add("Anisotropic Affine retained distinct pixel scale axes and published no fabricated mm metric.");
    }

    private static void VerifyFixtureTranslationAndFailureGates(
        ICollection<string> observations)
    {
        VisionPipelineStep consumer = new VisionPipelineStep
        {
            Name = "Fixture ROI consumer",
            ToolType = "Threshold",
            InputLayer = VisionRecipeRunner.DefaultInputLayer,
            OutputLayer = "Fixture_Output",
            Enabled = true
        };
        consumer.Parameters[VisionPipelineFixtureFrameService.ConsumeParameter] = "true";
        consumer.Parameters[VisionPipelineFixtureFrameService.FrameNameParameter] = "Frame";
        consumer.Parameters["USE_ROI"] = "true";
        consumer.Parameters["CvROI"] = "10,11,20,15";

        VisionPipelineFixtureFrame identity = CreateFixtureFrame(20D, 30D, 20D, 30D, 0D);
        VisionPipelineFixtureApplication identityApplication = VisionPipelineFixtureFrameService.PrepareRuntimeStep(
            consumer,
            new Dictionary<string, VisionPipelineFixtureFrame>(StringComparer.OrdinalIgnoreCase)
            {
                [identity.Name] = identity
            });
        Require(identityApplication.Success && identityApplication.Applied,
            "identity fixture was not applied.");
        Require(identityApplication.EffectiveRoi == new Rect(10, 11, 20, 15),
            "identity fixture changed the saved ROI coordinates.");

        VisionPipelineFixtureFrame translated = CreateFixtureFrame(20D, 30D, 23D, 35D, 0D);
        VisionPipelineFixtureApplication translatedApplication = VisionPipelineFixtureFrameService.PrepareRuntimeStep(
            consumer,
            new Dictionary<string, VisionPipelineFixtureFrame>(StringComparer.OrdinalIgnoreCase)
            {
                [translated.Name] = translated
            });
        Require(translatedApplication.Success && translatedApplication.EffectiveRoi == new Rect(13, 16, 20, 15),
            "fixture translation did not move the effective ROI by the rounded offset.");
        using (VisionToolResult result = new VisionToolResult { Success = true })
        {
            VisionPipelineFixtureFrameService.AddApplicationMetrics(result, translatedApplication);
            Require(result.Metrics.TryGetValue(VisionPipelineKnownMetrics.FixtureOffsetX, out double offsetX)
                && offsetX == 3D
                && result.Metrics.TryGetValue(VisionPipelineKnownMetrics.FixtureOffsetY, out double offsetY)
                && offsetY == 5D
                && result.Metrics.TryGetValue(VisionPipelineKnownMetrics.FixtureEffectiveRoiX, out double roiX)
                && roiX == 13D
                && result.Metrics.TryGetValue(VisionPipelineKnownMetrics.FixtureEffectiveRoiY, out double roiY)
                && roiY == 16D,
                "fixture metrics did not preserve offset/effective ROI meaning.");
        }

        VisionPipelineFixtureFrame rotated = CreateFixtureFrame(20D, 30D, 23D, 35D, 90D);
        VisionPipelineFixtureApplication rotatedApplication = VisionPipelineFixtureFrameService.PrepareRuntimeStep(
            consumer,
            new Dictionary<string, VisionPipelineFixtureFrame>(StringComparer.OrdinalIgnoreCase)
            {
                [rotated.Name] = rotated
            });
        Require(!rotatedApplication.Success
            && rotatedApplication.Message.Contains("translation-only", StringComparison.OrdinalIgnoreCase),
            "translation-only fixture consumer accepted an unsupported rotation.");

        VisionPipelineFixtureApplication missingFrame = VisionPipelineFixtureFrameService.PrepareRuntimeStep(
            consumer,
            new Dictionary<string, VisionPipelineFixtureFrame>(StringComparer.OrdinalIgnoreCase));
        Require(!missingFrame.Success && missingFrame.Message.Contains("not available", StringComparison.OrdinalIgnoreCase),
            "fixture consumer did not fail closed when the frame was missing.");

        observations.Add("Fixture identity/translation preserved effective ROI and offset metrics; rotation and missing-frame gates failed closed.");
    }

    private static void VerifySingularAffineGate(
        Point2f[] sourcePoints,
        ICollection<string> failures,
        ICollection<string> observations)
    {
        AffineTransformProperty property = new AffineTransformProperty("Singular")
        {
            SourcePoint1X = sourcePoints[0].X,
            SourcePoint1Y = sourcePoints[0].Y,
            SourcePoint2X = sourcePoints[1].X,
            SourcePoint2Y = sourcePoints[1].Y,
            SourcePoint3X = sourcePoints[2].X,
            SourcePoint3Y = sourcePoints[2].Y,
            DestinationPoint1X = 20D,
            DestinationPoint1Y = 20D,
            DestinationPoint2X = 80D,
            DestinationPoint2Y = 20D,
            DestinationPoint3X = 140D,
            DestinationPoint3Y = 20D,
            OutputWidth = 160,
            OutputHeight = 120,
            MinimumSourceTriangleArea = 1D,
            MinimumDestinationTriangleArea = 0D,
            MinimumValidPixelRatio = 0.1D
        };
        VisionPipelineStep step = VisionPipelineStepBuilder.FromAffineTransformProperty(
            property,
            "Singular",
            VisionRecipeRunner.DefaultInputLayer,
            "Singular_Output");
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-030 singular" };
        pipeline.Steps.Add(step);
        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            pipeline,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        if (validation.Success
            || !validation.Errors.Any(error => error.Contains("destination point triangle area", StringComparison.OrdinalIgnoreCase)))
        {
            failures.Add("singular destination points were accepted by the shared validator.");
        }
        else
        {
            observations.Add("Singular destination triangle rejected by shared validation.");
        }
    }

    private static VisionPipelineFixtureFrame CreateFixtureFrame(
        double referenceX,
        double referenceY,
        double currentX,
        double currentY,
        double currentAngle)
    {
        return new VisionPipelineFixtureFrame
        {
            Name = "Frame",
            SourceLayer = VisionRecipeRunner.DefaultInputLayer,
            ReferenceX = referenceX,
            ReferenceY = referenceY,
            ReferenceAngle = 0D,
            ReferenceScale = 1D,
            CurrentX = currentX,
            CurrentY = currentY,
            CurrentAngle = currentAngle,
            CurrentScale = 1D,
            MaximumAngleDelta = 2D,
            MinimumScaleRatio = 0.5D,
            MaximumScaleRatio = 2D
        };
    }

    private static Mat CreateSourceImage()
    {
        Mat source = new Mat(new Size(160, 120), MatType.CV_8UC3, Scalar.Black);
        Cv2.Rectangle(source, new Rect(20, 20, 50, 40), new Scalar(255, 255, 255), -1);
        Cv2.Circle(source, new OpenCvSharp.Point(110, 70), 14, new Scalar(0, 180, 255), -1);
        return source;
    }

    private static void CompareMatrixMetrics(
        VisionRecipeStepRunSummary first,
        VisionRecipeStepRunSummary second,
        IReadOnlyList<string> metricNames,
        string label)
    {
        foreach (string metricName in metricNames)
        {
            Require(first.Metrics.TryGetValue(metricName, out double firstValue)
                && second.Metrics.TryGetValue(metricName, out double secondValue)
                && Math.Abs(firstValue - secondValue) <= CoordinateTolerance,
                label + " changed " + metricName + ".");
        }
    }

    private static bool OverlayCoordinatesEqual(
        IReadOnlyList<VisionRecipeOverlaySummary> first,
        IReadOnlyList<VisionRecipeOverlaySummary> second)
    {
        if (first.Count != second.Count)
        {
            return false;
        }

        for (int index = 0; index < first.Count; index++)
        {
            VisionRecipeOverlaySummary left = first[index];
            VisionRecipeOverlaySummary right = second[index];
            if (!string.Equals(left.Kind, right.Kind, StringComparison.OrdinalIgnoreCase)
                || Math.Abs(left.CenterX - right.CenterX) > 0.01F
                || Math.Abs(left.CenterY - right.CenterY) > 0.01F
                || Math.Abs(left.StartX - right.StartX) > 0.01F
                || Math.Abs(left.StartY - right.StartY) > 0.01F
                || Math.Abs(left.EndX - right.EndX) > 0.01F
                || Math.Abs(left.EndY - right.EndY) > 0.01F)
            {
                return false;
            }
        }

        return true;
    }

    private static void AssertMetric(
        IReadOnlyDictionary<string, double> metrics,
        string metricName,
        double expected,
        double tolerance,
        string label)
    {
        Require(metrics.TryGetValue(metricName, out double actual)
            && Math.Abs(actual - expected) <= tolerance,
            $"{label} {metricName} expected {expected.ToString("R", CultureInfo.InvariantCulture)} but was {(metrics.TryGetValue(metricName, out actual) ? actual : double.NaN).ToString("R", CultureInfo.InvariantCulture)}.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed class TransformCase
    {
        public TransformCase(string name, params Point2f[] destinationPoints)
        {
            Name = name;
            DestinationPoints = destinationPoints;
        }

        public string Name { get; }
        public Point2f[] DestinationPoints { get; }
    }
}
