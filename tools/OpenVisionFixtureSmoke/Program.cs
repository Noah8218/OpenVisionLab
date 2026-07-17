using Lib.Common;
using Lib.OpenCV.Pipeline;
using OpenCvSharp;
using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal static class Program
{
    private const int ShiftX = 70;
    private const int ShiftY = 40;
    private const string SavedRoi = "170,80,50,50";

    private static async Task<int> Main()
    {
        string outputDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            "artifacts",
            "fixture_translation_smoke_current");
        Directory.CreateDirectory(outputDirectory);

        string templatePath = Path.Combine(outputDirectory, "fixture_template.png");
        using Mat referenceImage = CreateImage(0, 0);
        using Mat shiftedImage = CreateImage(ShiftX, ShiftY);
        using (Mat template = new Mat(referenceImage, new Rect(40, 50, 40, 40)).Clone())
        {
            Cv2.ImWrite(templatePath, template);
        }

        Cv2.ImWrite(Path.Combine(outputDirectory, "reference.png"), referenceImage);
        Cv2.ImWrite(Path.Combine(outputDirectory, "shifted.png"), shiftedImage);

        VisionPipeline referencePipeline = CreatePipeline(templatePath, useFixture: true);
        VisionPipeline shiftedPipeline = CreatePipeline(templatePath, useFixture: true);
        VisionPipeline unfixturedPipeline = CreatePipeline(templatePath, useFixture: false);
        VisionPipeline missingFramePipeline = CreatePipeline(templatePath, useFixture: true);
        missingFramePipeline.Steps[1].Parameters["FIXTURE_FRAME_NAME"] = "MissingFrame";
        VisionPipeline multiRoiPipeline = CreatePipeline(templatePath, useFixture: true);
        multiRoiPipeline.Steps[1].Parameters["USE_MULTI_ROI"] = "true";
        VisionPipeline duplicateFramePipeline = CreateDuplicateFramePipeline(templatePath);
        string fixturePipelinePath = Path.Combine(outputDirectory, "fixture_translation.pipeline.xml");
        Assert(SerializeHelper.SaveXmlFile(fixturePipelinePath, shiftedPipeline), "Fixture pipeline XML save must succeed.");
        VisionRecipeRunner runner = new VisionRecipeRunner();

        using VisionRecipeRunResult referenceResult = await runner.RunAsync(referencePipeline, referenceImage);
        using VisionRecipeRunResult shiftedResult = await runner.RunAsync(fixturePipelinePath, shiftedImage);
        using VisionRecipeRunResult unfixturedResult = await runner.RunAsync(unfixturedPipeline, shiftedImage);
        using VisionRecipeRunResult missingFrameResult = await runner.RunAsync(missingFramePipeline, referenceImage);
        using VisionRecipeRunResult multiRoiResult = await runner.RunAsync(multiRoiPipeline, referenceImage);
        using VisionRecipeRunResult duplicateFrameResult = await runner.RunAsync(duplicateFramePipeline, referenceImage);

        string publicPipelinePath = Path.GetFullPath(Path.Combine(
            "docs",
            "samples",
            "public",
            "Public_Matching_FixturePad.pipeline.xml"));
        string publicGoodPath = Path.GetFullPath(Path.Combine(
            "docs",
            "samples",
            "public",
            "Fixture_Pad_Synthetic_Shifted_OK.png"));
        string publicBadPath = Path.GetFullPath(Path.Combine(
            "docs",
            "samples",
            "public",
            "Fixture_Pad_Synthetic_Shifted_Missing_NG.png"));
        Assert(File.Exists(publicPipelinePath), "Public Fixture pipeline must exist.");
        Assert(File.Exists(publicGoodPath), "Public Fixture Good image must exist.");
        Assert(File.Exists(publicBadPath), "Public Fixture Bad image must exist.");
        Assert(
            SerializeHelper.TryLoadFromXmlFile(publicPipelinePath, out VisionPipeline publicNoFixturePipeline)
                && publicNoFixturePipeline != null,
            "Public Fixture pipeline XML load must succeed.");
        publicNoFixturePipeline.Steps[0].Parameters["USE_AS_FIXTURE_FRAME"] = "false";
        publicNoFixturePipeline.Steps[1].Parameters["USE_FIXTURE_FRAME"] = "false";
        using Mat publicGoodImage = Cv2.ImRead(publicGoodPath, ImreadModes.Unchanged);
        using Mat publicBadImage = Cv2.ImRead(publicBadPath, ImreadModes.Unchanged);
        using VisionRecipeRunResult publicGoodResult = await runner.RunAsync(publicPipelinePath, publicGoodImage);
        using VisionRecipeRunResult publicBadResult = await runner.RunAsync(publicPipelinePath, publicBadImage);
        using VisionRecipeRunResult publicNoFixtureResult = await runner.RunAsync(publicNoFixturePipeline, publicGoodImage);

        Assert(referenceResult.Success, "Reference fixture run must pass.");
        Assert(shiftedResult.Success, "Shifted fixture run must pass.");
        Assert(!unfixturedResult.Success, "Shifted run without fixture must fail the downstream ROI inspection.");
        AssertFixtureConfigurationFailure(missingFrameResult, "Missing fixture frame must fail closed.");
        AssertFixtureConfigurationFailure(multiRoiResult, "Fixture multi-ROI must fail closed.");
        AssertFixtureConfigurationFailure(duplicateFrameResult, "Duplicate fixture frame must fail closed.");
        Assert(publicGoodResult.Success, "Public shifted Fixture Good sample must pass.");
        Assert(!publicBadResult.Success, "Public shifted Fixture Bad sample must fail at the pad inspection.");
        Assert(
            string.Equals(publicBadResult.FirstFailedErrorName, "BlobNoResult", StringComparison.Ordinal),
            "Public Fixture Bad sample must keep Matching successful and fail at BlobNoResult.");
        Assert(!publicNoFixtureResult.Success, "Public shifted Good sample without Fixture correction must fail.");
        Assert(
            SerializeHelper.TryLoadFromXmlFile(fixturePipelinePath, out VisionPipeline persistedPipeline)
                && persistedPipeline != null,
            "Fixture pipeline XML reload must succeed.");
        Assert(
            string.Equals(persistedPipeline.Steps[1].Parameters["CvROI"], SavedRoi, StringComparison.Ordinal),
            "Runtime fixture application must not rewrite the saved CvROI.");

        VisionRecipeStepRunSummary fixtureProducer = shiftedResult.Steps[0];
        VisionRecipeStepRunSummary fixtureConsumer = shiftedResult.Steps[1];
        AssertMetric(fixtureProducer, "FixtureOffsetX", ShiftX, 1.1);
        AssertMetric(fixtureProducer, "FixtureOffsetY", ShiftY, 1.1);
        AssertMetric(fixtureConsumer, "FixtureEffectiveRoiX", 170 + ShiftX, 0.1);
        AssertMetric(fixtureConsumer, "FixtureEffectiveRoiY", 80 + ShiftY, 0.1);
        AssertMetric(publicGoodResult.Steps[0], "FixtureOffsetX", 80, 0.1);
        AssertMetric(publicGoodResult.Steps[0], "FixtureOffsetY", 55, 0.1);
        AssertMetric(publicGoodResult.Steps[1], "FixtureEffectiveRoiX", 400, 0.1);
        AssertMetric(publicGoodResult.Steps[1], "FixtureEffectiveRoiY", 235, 0.1);
        Assert(
            string.Equals(publicNoFixturePipeline.Steps[1].Parameters["CvROI"], "320,180,60,50", StringComparison.Ordinal),
            "Public Fixture execution must not rewrite the saved CvROI.");

        if (shiftedResult.ResultImage != null && !shiftedResult.ResultImage.Empty())
        {
            Cv2.ImWrite(Path.Combine(outputDirectory, "shifted_fixture_result.png"), shiftedResult.ResultImage);
        }

        string report = string.Join(Environment.NewLine, new[]
        {
            "Result: PASS",
            "Scenario: Matching fixture translation",
            "ReferenceRun: " + referenceResult.SummaryText,
            "ShiftedFixtureRun: " + shiftedResult.SummaryText,
            "ShiftedWithoutFixtureRun: " + unfixturedResult.SummaryText,
            "MissingFrameRun: " + missingFrameResult.SummaryText,
            "MultiRoiRun: " + multiRoiResult.SummaryText,
            "DuplicateFrameRun: " + duplicateFrameResult.SummaryText,
            "PublicGoodRun: " + publicGoodResult.SummaryText,
            "PublicBadRun: " + publicBadResult.SummaryText,
            "PublicGoodWithoutFixtureRun: " + publicNoFixtureResult.SummaryText,
            "SavedCvROI: " + persistedPipeline.Steps[1].Parameters["CvROI"],
            "PublicSavedCvROI: " + publicNoFixturePipeline.Steps[1].Parameters["CvROI"],
            "FixtureOffsetX: " + GetMetric(fixtureProducer, "FixtureOffsetX").ToString("0.###", CultureInfo.InvariantCulture),
            "FixtureOffsetY: " + GetMetric(fixtureProducer, "FixtureOffsetY").ToString("0.###", CultureInfo.InvariantCulture),
            "EffectiveRoiX: " + GetMetric(fixtureConsumer, "FixtureEffectiveRoiX").ToString("0.###", CultureInfo.InvariantCulture),
            "EffectiveRoiY: " + GetMetric(fixtureConsumer, "FixtureEffectiveRoiY").ToString("0.###", CultureInfo.InvariantCulture)
        });
        File.WriteAllText(Path.Combine(outputDirectory, "report.txt"), report);
        Console.WriteLine(report);
        return 0;
    }

    private static Mat CreateImage(int offsetX, int offsetY)
    {
        Mat image = new Mat(new Size(320, 220), MatType.CV_8UC1, Scalar.Black);
        Point markerCenter = new Point(60 + offsetX, 70 + offsetY);
        Cv2.Rectangle(
            image,
            new Rect(markerCenter.X - 12, markerCenter.Y - 12, 24, 24),
            Scalar.White,
            2);
        Cv2.Line(
            image,
            new Point(markerCenter.X - 9, markerCenter.Y),
            new Point(markerCenter.X + 9, markerCenter.Y),
            Scalar.White,
            2);
        Cv2.Line(
            image,
            new Point(markerCenter.X, markerCenter.Y - 9),
            new Point(markerCenter.X, markerCenter.Y + 9),
            Scalar.White,
            2);
        Cv2.Circle(image, markerCenter, 3, Scalar.Black, -1);

        Cv2.Rectangle(
            image,
            new Rect(182 + offsetX, 92 + offsetY, 24, 24),
            Scalar.White,
            -1);
        return image;
    }

    private static VisionPipeline CreatePipeline(string templatePath, bool useFixture)
    {
        VisionPipeline pipeline = new VisionPipeline
        {
            Name = useFixture ? "FixtureTranslation" : "NoFixtureTranslation"
        };

        VisionPipelineStep matching = new VisionPipelineStep
        {
            Name = "01 Locate Reference",
            ToolType = "Matching",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "FixtureMatch",
            UseAcceptance = true,
            ExpectedSuccess = true,
            AcceptanceMetricName = "ResultCount",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 1,
            UseAcceptanceMetricMaximum = true,
            AcceptanceMetricMaximum = 1,
            MaxElapsedMilliseconds = 2000
        };
        Add(matching, "Name", "FixtureTranslationMatching");
        Add(matching, "TemplatePath", templatePath);
        Add(matching, "PATTERN_PATH", templatePath);
        Add(matching, "MATCH_MODE", "CCoeffNormed");
        Add(matching, "SCORE_MIN", "0.80");
        Add(matching, "MAGNIFIATION", "1");
        Add(matching, "NUM_MATCH", "1");
        Add(matching, "USE_FIND_ANGLE", "false");
        Add(matching, "USE_THRESHOLD", "false");
        Add(matching, "USE_ADAPTIVE_THRESHOLD", "false");
        Add(matching, "USE_ROI", "false");
        Add(matching, "USE_AS_FIXTURE_FRAME", useFixture ? "true" : "false");
        Add(matching, "FIXTURE_FRAME_NAME", "PartFrame");
        Add(matching, "FIXTURE_REFERENCE_X", "60");
        Add(matching, "FIXTURE_REFERENCE_Y", "70");
        Add(matching, "FIXTURE_REFERENCE_ANGLE", "0");
        Add(matching, "FIXTURE_MAX_ANGLE_DELTA", "1");
        pipeline.Steps.Add(matching);

        VisionPipelineStep blob = new VisionPipelineStep
        {
            Name = "02 Inspect Shifted Pad",
            ToolType = "Blob",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "PadBlob",
            UseAcceptance = true,
            ExpectedSuccess = true,
            AcceptanceMetricName = "ResultCount",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 1,
            UseAcceptanceMetricMaximum = true,
            AcceptanceMetricMaximum = 1,
            MaxElapsedMilliseconds = 1000
        };
        Add(blob, "Name", "FixtureTranslationBlob");
        Add(blob, "MIN_AREA", "300");
        Add(blob, "MAX_AREA", "900");
        Add(blob, "USE_THRESHOLD", "true");
        Add(blob, "THRESHOLD_TYPES", "Binary");
        Add(blob, "THRESHOLD", "128");
        Add(blob, "USE_ADAPTIVE_THRESHOLD", "false");
        Add(blob, "USE_BITWISENOT", "false");
        Add(blob, "USE_ROI", "true");
        Add(blob, "USE_MULTI_ROI", "false");
        Add(blob, "USE_MASKING", "false");
        Add(blob, "CvROI", SavedRoi);
        Add(blob, "ALLOW_BRANCH_INPUT", "true");
        Add(blob, "USE_FIXTURE_FRAME", useFixture ? "true" : "false");
        Add(blob, "FIXTURE_FRAME_NAME", "PartFrame");
        pipeline.Steps.Add(blob);

        VisionPipelineStep merge = new VisionPipelineStep
        {
            Name = "03 Merge Fixture Review",
            ToolType = "OverlayMerge",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "FixtureReview",
            UseAcceptance = true,
            ExpectedSuccess = true,
            AcceptanceMetricName = "MergeOverlayCount",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 2,
            MaxElapsedMilliseconds = 1000
        };
        Add(merge, "SourceLayers", "FixtureMatch;PadBlob");
        Add(merge, "BurnIn", "true");
        Add(merge, "DrawLabels", "true");
        Add(merge, "AllowEmpty", "false");
        pipeline.Steps.Add(merge);

        return pipeline;
    }

    private static VisionPipeline CreateDuplicateFramePipeline(string templatePath)
    {
        VisionPipeline pipeline = CreatePipeline(templatePath, useFixture: true);
        VisionPipelineStep duplicate = CreatePipeline(templatePath, useFixture: true).Steps[0];
        duplicate.Name = "02 Duplicate Reference";
        duplicate.OutputLayer = "DuplicateFixtureMatch";
        pipeline.Steps.Insert(1, duplicate);
        pipeline.Steps[2].Name = "03 Inspect Shifted Pad";
        pipeline.Steps[3].Name = "04 Merge Fixture Review";
        return pipeline;
    }

    private static void Add(VisionPipelineStep step, string key, string value)
    {
        step.Parameters[key] = value;
    }

    private static void AssertMetric(
        VisionRecipeStepRunSummary step,
        string metricName,
        double expected,
        double tolerance)
    {
        double actual = GetMetric(step, metricName);
        Assert(
            Math.Abs(actual - expected) <= tolerance,
            $"{metricName} expected {expected:0.###} +/- {tolerance:0.###}, actual {actual:0.###}.");
    }

    private static double GetMetric(VisionRecipeStepRunSummary step, string metricName)
    {
        if (step?.Metrics == null || !step.Metrics.TryGetValue(metricName, out double value))
        {
            string available = step?.Metrics == null
                ? "none"
                : string.Join(", ", step.Metrics.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase));
            throw new InvalidOperationException($"Metric '{metricName}' is missing. Available: {available}");
        }

        return value;
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static void AssertFixtureConfigurationFailure(VisionRecipeRunResult result, string message)
    {
        Assert(!result.Success, message);
        Assert(
            string.Equals(result.FirstFailedErrorName, "InvalidParameter", StringComparison.Ordinal)
                || string.Equals(result.FirstFailedErrorName, "InvalidRoi", StringComparison.Ordinal),
            message + " Error=" + result.FirstFailedErrorName);
    }
}
