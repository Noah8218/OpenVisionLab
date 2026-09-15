using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal static class MatchingBoundaryContract
{
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
        string templatePath = Path.Combine(outputDirectory, "matching_template.png");

        using Mat template = CreateTemplate();
        Cv2.ImWrite(templatePath, template);

        using Mat noResultSource = CreateSource(template, Array.Empty<(int X, int Y, bool Mutate)>());
        using Mat multipleSource = CreateSource(
            template,
            new[]
            {
                (24, 30, false),
                (116, 30, false)
            });
        using Mat scoreSource = CreateSource(
            template,
            new[]
            {
                (72, 30, true)
            });
        Cv2.ImWrite(Path.Combine(outputDirectory, "matching_no_result_source.png"), noResultSource);
        Cv2.ImWrite(Path.Combine(outputDirectory, "matching_multiple_source.png"), multipleSource);
        Cv2.ImWrite(Path.Combine(outputDirectory, "matching_score_source.png"), scoreSource);

        try
        {
            await VerifyNoResultAsync(runner, templatePath, noResultSource, observations).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            failures.Add("no-result: " + exception.GetBaseException().Message);
        }

        try
        {
            await VerifyMultipleResultAsync(
                runner,
                templatePath,
                multipleSource,
                outputDirectory,
                observations).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            failures.Add("multiple-result: " + exception.GetBaseException().Message);
        }

        try
        {
            await VerifyScoreBoundaryAsync(
                runner,
                templatePath,
                scoreSource,
                outputDirectory,
                observations).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            failures.Add("score-boundary: " + exception.GetBaseException().Message);
        }

        string reportPath = Path.Combine(outputDirectory, "matching-boundary-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-031 Matching no-result, multiple-result, and score boundary",
                "Owner: existing Matching tool -> VisionPipelineExecutionService -> VisionRecipeRunner -> acceptance/overlay/Run History",
                "EvidenceDirectory: " + outputDirectory
            }
            .Concat(observations.Select(item => "PASS: " + item))
            .Concat(failures.Select(item => "FAIL: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Matching boundary contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Matching boundary contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static async Task VerifyNoResultAsync(
        VisionRecipeRunner runner,
        string templatePath,
        Mat source,
        ICollection<string> observations)
    {
        VisionPipeline pipeline = CreateMatchingPipeline(templatePath, 0.8D, 1, "No result");
        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            pipeline,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        Require(validation.Success, "no-result validation failed: " + string.Join(" | ", validation.Errors));

        using VisionRecipeRunResult run = await runner.RunAsync(pipeline, source).ConfigureAwait(false);
        VisionRecipeStepRunSummary? summary = run.Steps.SingleOrDefault();
        Require(summary != null,
            "blank source did not produce a step summary. Run success=" + run.Success + ".");
        VisionRecipeStepRunSummary summaryValue = summary!;
        string blankCount = summaryValue.Metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out double blankResultCount)
            ? blankResultCount.ToString("R", CultureInfo.InvariantCulture)
            : "missing";
        Require(!run.Success,
            "blank source did not fail closed. Run success=" + run.Success
            + ", step success=" + summaryValue.Success
            + ", error=" + summaryValue.ErrorName
            + ", count=" + blankCount
            + ", overlays=" + summaryValue.OverlayCount
            + ", metrics=" + FormatMetrics(summaryValue.Metrics)
            + ", message=" + summaryValue.Message);
        Require(string.Equals(summaryValue.ErrorName, "MatchingNoResult", StringComparison.Ordinal),
            "blank source error was " + summaryValue.ErrorName + " instead of MatchingNoResult.");
        Require(summaryValue.OverlayCount == 0
            && (!summaryValue.Metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out double count) || count == 0D),
            "no-result published a positive result count or overlay.");
        observations.Add("Blank source failed closed as MatchingNoResult without a positive ResultCount or overlay.");
    }

    private static async Task VerifyMultipleResultAsync(
        VisionRecipeRunner runner,
        string templatePath,
        Mat source,
        string evidenceDirectory,
        ICollection<string> observations)
    {
        VisionPipeline pipeline = CreateMatchingPipeline(templatePath, 0.8D, 2, "Multiple result");
        pipeline.Steps[0].UseAcceptance = true;
        pipeline.Steps[0].AcceptanceMetricName = VisionPipelineKnownMetrics.ResultCount;
        pipeline.Steps[0].UseAcceptanceMetricMinimum = true;
        pipeline.Steps[0].AcceptanceMetricMinimum = 2D;
        pipeline.Steps[0].UseAcceptanceMetricMaximum = true;
        pipeline.Steps[0].AcceptanceMetricMaximum = 2D;

        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            pipeline,
            new[] { VisionRecipeRunner.DefaultInputLayer });
        Require(validation.Success, "multiple-result validation failed: " + string.Join(" | ", validation.Errors));

        using VisionRecipeRunResult run = await runner.RunAsync(pipeline, source).ConfigureAwait(false);
        VisionRecipeStepRunSummary? summary = run.Steps.SingleOrDefault();
        Require(summary != null,
            "multiple-target run did not produce a step summary. Run success=" + run.Success + ".");
        VisionRecipeStepRunSummary summaryValue = summary!;
        string multipleCount = summaryValue.Metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out double multipleResultCount)
            ? multipleResultCount.ToString("R", CultureInfo.InvariantCulture)
            : "missing";
        Require(run.Success,
            "multiple-target run failed: " + run.Message
            + ", step success=" + summaryValue.Success
            + ", error=" + summaryValue.ErrorName
            + ", count=" + multipleCount
            + ", overlays=" + summaryValue.OverlayCount
            + ", metrics=" + FormatMetrics(summaryValue.Metrics));
        Require(summaryValue.Success && summaryValue.AcceptancePassed,
            "multiple-target acceptance did not pass: " + summaryValue.AcceptanceMessage);
        Require(summaryValue.Metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out double count)
            && count == 2D,
            "NUM_MATCH=2 did not publish ResultCount=2.");
        double scoreMin = double.NaN;
        double scoreMax = double.NaN;
        Require(summaryValue.Metrics.TryGetValue(VisionPipelineKnownMetrics.ScoreMin, out scoreMin)
            && summaryValue.Metrics.TryGetValue(VisionPipelineKnownMetrics.ScoreMax, out scoreMax)
            && double.IsFinite(scoreMin)
            && double.IsFinite(scoreMax)
            && scoreMax >= scoreMin
            && scoreMin >= 0D,
            "multiple-target run did not publish ordered finite score bounds.");

        List<VisionRecipeOverlaySummary> rectangles = summaryValue.Overlays
            .Where(item => string.Equals(item.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase))
            .ToList();
        Require(rectangles.Count >= 2, "multiple-target run did not retain two rectangle overlays.");
        Require(rectangles.Any(item => Math.Abs(item.CenterX - 40D) <= 3D)
            && rectangles.Any(item => Math.Abs(item.CenterX - 132D) <= 3D),
            "matching overlays did not retain both target centers.");

        string pipelinePath = Path.Combine(evidenceDirectory, "matching_multiple.pipeline.xml");
        Require(VisionPipelineStorage.TrySaveToFile(pipelinePath, pipeline, out string saveMessage), saveMessage);
        Require(VisionPipelineStorage.TryLoadFromFile(pipelinePath, out VisionPipeline reopened, out string loadMessage), loadMessage);
        using VisionRecipeRunResult reopenedRun = await runner.RunAsync(reopened, source).ConfigureAwait(false);
        VisionRecipeStepRunSummary? reopenedSummary = reopenedRun.Steps.SingleOrDefault();
        Require(reopenedRun.Success && reopenedSummary != null && reopenedSummary.Success,
            "multiple-target save/reopen run failed: " + reopenedRun.Message);
        Require(reopenedSummary!.Metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out double reopenedCount)
            && reopenedCount == 2D,
            "save/reopen changed NUM_MATCH=2 ResultCount.");
        Require(reopenedSummary.Overlays.Count == summaryValue.Overlays.Count,
            "save/reopen changed multiple-result overlay count.");

        string reportPath = VisionPipelineRunReportStorage.Save(
            "2D-031",
            reopened,
            reopenedRun,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMilliseconds(Math.Max(1D, reopenedRun.TotalMilliseconds)),
            "multiple-result",
            source);
        VisionPipelineRunReport? report = VisionPipelineRunReportStorage.Load(reportPath);
        VisionPipelineStepRunReport? persistedStep = report?.Steps.SingleOrDefault();
        Require(report != null && persistedStep != null, "multiple-result report did not reload.");
        Require(persistedStep!.OverlayCount == reopenedSummary.OverlayCount
            && persistedStep.Metrics.Any(item =>
                string.Equals(item.Name, VisionPipelineKnownMetrics.ResultCount, StringComparison.OrdinalIgnoreCase)
                && item.Value == 2D),
            "Run History did not retain multiple-result count/overlay evidence.");
        Require(!string.IsNullOrWhiteSpace(persistedStep.OverlayImageFile)
            && File.Exists(Path.Combine(Path.GetDirectoryName(reportPath)!, persistedStep.OverlayImageFile)),
            "Run History did not export the multiple-result overlay image.");
        observations.Add($"NUM_MATCH=2 retained ResultCount=2, ordered scores ({scoreMin.ToString("0.###", CultureInfo.InvariantCulture)}..{scoreMax.ToString("0.###", CultureInfo.InvariantCulture)}), two overlays, acceptance, XML reopen, and Run History export.");
    }

    private static async Task VerifyScoreBoundaryAsync(
        VisionRecipeRunner runner,
        string templatePath,
        Mat source,
        string evidenceDirectory,
        ICollection<string> observations)
    {
        VisionPipeline baselinePipeline = CreateMatchingPipeline(templatePath, 0D, 1, "Score baseline");
        using VisionRecipeRunResult baselineRun = await runner.RunAsync(baselinePipeline, source).ConfigureAwait(false);
        VisionRecipeStepRunSummary? baselineSummary = baselineRun.Steps.SingleOrDefault();
        Require(baselineRun.Success && baselineSummary != null,
            "score baseline failed: " + baselineRun.Message);
        Require(baselineSummary!.Metrics.TryGetValue(VisionPipelineKnownMetrics.ScoreMax, out double score)
            && double.IsFinite(score)
            && score > 0D
            && score < 99.999999D,
            "score boundary fixture did not produce a usable finite score below 100: "
            + (baselineSummary.Metrics.TryGetValue(VisionPipelineKnownMetrics.ScoreMax, out double observed) ? observed.ToString("R", CultureInfo.InvariantCulture) : "missing"));

        double normalizedScore = score / 100D;
        double lowThreshold = Math.Max(0D, normalizedScore - Math.Max(0.01D, normalizedScore * 0.05D));
        double highThreshold = Math.Min(1D, normalizedScore + Math.Max(0.01D, normalizedScore * 0.05D));
        Require(lowThreshold < normalizedScore && highThreshold > normalizedScore,
            "could not create thresholds around the observed score.");

        VisionPipeline lowPipeline = CreateMatchingPipeline(templatePath, lowThreshold, 1, "Score accepted");
        using VisionRecipeRunResult lowRun = await runner.RunAsync(lowPipeline, source).ConfigureAwait(false);
        VisionRecipeStepRunSummary? lowSummary = lowRun.Steps.SingleOrDefault();
        Require(lowRun.Success && lowSummary != null && lowSummary.Success
            && lowSummary.Metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out double lowCount)
            && lowCount == 1D,
            "threshold below observed score did not retain the candidate.");

        VisionPipeline highPipeline = CreateMatchingPipeline(templatePath, highThreshold, 1, "Score rejected");
        using VisionRecipeRunResult highRun = await runner.RunAsync(highPipeline, source).ConfigureAwait(false);
        VisionRecipeStepRunSummary? highSummary = highRun.Steps.SingleOrDefault();
        Require(!highRun.Success && highSummary != null
            && string.Equals(highSummary.ErrorName, "MatchingNoResult", StringComparison.Ordinal)
            && (!highSummary.Metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out double highCount) || highCount == 0D),
            "threshold above observed score did not fail closed as MatchingNoResult.");

        string pipelinePath = Path.Combine(evidenceDirectory, "matching_score_boundary.pipeline.xml");
        Require(VisionPipelineStorage.TrySaveToFile(pipelinePath, lowPipeline, out string saveMessage), saveMessage);
        Require(VisionPipelineStorage.TryLoadFromFile(pipelinePath, out VisionPipeline reopened, out string loadMessage), loadMessage);
        using VisionRecipeRunResult reopenedRun = await runner.RunAsync(reopened, source).ConfigureAwait(false);
        VisionRecipeStepRunSummary? reopenedSummary = reopenedRun.Steps.SingleOrDefault();
        Require(reopenedRun.Success && reopenedSummary != null
            && reopenedSummary.Metrics.TryGetValue(VisionPipelineKnownMetrics.ScoreMax, out double reopenedScore)
            && Math.Abs(reopenedScore - score) <= 1D,
            "save/reopen changed the observed score boundary.");
        observations.Add($"Observed score {score.ToString("0.######", CultureInfo.InvariantCulture)}% was accepted at SCORE_MIN={lowThreshold.ToString("0.######", CultureInfo.InvariantCulture)} and rejected at SCORE_MIN={highThreshold.ToString("0.######", CultureInfo.InvariantCulture)} as MatchingNoResult; XML reopen retained the score.");
    }

    private static VisionPipeline CreateMatchingPipeline(
        string templatePath,
        double scoreMinimum,
        int matchCount,
        string name)
    {
        MatchingProperty property = new MatchingProperty(name)
        {
            PATTERN_PATH = templatePath,
            MATCH_MODE = TemplateMatchModes.SqDiffNormed,
            SCORE_MIN = scoreMinimum,
            NUM_MATCH = matchCount,
            USE_FIND_ANGLE = true,
            FIND_ANGLE_MIN = 0,
            FIND_ANGLE_MAX = 0,
            FIND_ANGLE = 1D,
            USE_FIND_SCALE = false,
            USE_CANNY = false,
            USE_THRESHOLD = false,
            USE_ADAPTIVE_THRESHOLD = false,
            USE_BITWISENOT = false,
            USE_ROI = true,
            MAGNIFIATION = 1D
        };
        VisionPipelineStep step = VisionPipelineStepBuilder.FromProperty(
            property,
            VisionRecipeRunner.DefaultInputLayer,
            "Matching_Result");
        step.Parameters["TemplatePath"] = templatePath;
        step.Parameters["Name"] = name;
        step.Parameters["CvROI"] = "0,0,180,100";
        VisionPipeline pipeline = new VisionPipeline { Name = "2D-031 " + name };
        pipeline.Steps.Add(step);
        return pipeline;
    }

    private static Mat CreateTemplate()
    {
        Mat template = new Mat(new Size(32, 32), MatType.CV_8UC1, Scalar.All(30));
        Cv2.Rectangle(template, new Rect(3, 3, 26, 5), Scalar.All(220), -1);
        Cv2.Rectangle(template, new Rect(3, 24, 26, 5), Scalar.All(180), -1);
        Cv2.Circle(template, new OpenCvSharp.Point(10, 16), 5, Scalar.All(245), -1);
        Cv2.Line(template, new OpenCvSharp.Point(18, 9), new OpenCvSharp.Point(27, 22), Scalar.All(110), 3);
        Cv2.Line(template, new OpenCvSharp.Point(27, 9), new OpenCvSharp.Point(18, 22), Scalar.All(70), 2);
        return template;
    }

    private static Mat CreateSource(
        Mat template,
        IEnumerable<(int X, int Y, bool Mutate)> targets)
    {
        using Mat graySource = new Mat(new Size(180, 100), MatType.CV_8UC1);
        for (int y = 0; y < graySource.Height; y++)
        {
            for (int x = 0; x < graySource.Width; x++)
            {
                graySource.Set(y, x, (byte)(20 + ((x * 17 + y * 29 + x * y) % 70)));
            }
        }
        foreach ((int x, int y, bool mutate) in targets)
        {
            using Mat target = template.Clone();
            if (mutate)
            {
                Cv2.Rectangle(target, new Rect(2, 2, 4, 4), Scalar.All(100), -1);
                Cv2.Circle(target, new OpenCvSharp.Point(23, 23), 2, Scalar.All(210), -1);
            }

            using Mat roi = new Mat(graySource, new Rect(x, y, target.Width, target.Height));
            target.CopyTo(roi);
        }

        Mat source = new Mat();
        Cv2.CvtColor(graySource, source, ColorConversionCodes.GRAY2BGR);
        return source;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static string FormatMetrics(IReadOnlyDictionary<string, double> metrics)
    {
        return string.Join(
            ",",
            metrics.Select(item => item.Key + "=" + item.Value.ToString("R", CultureInfo.InvariantCulture)));
    }
}
