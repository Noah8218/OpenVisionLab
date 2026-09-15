using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Core;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal static class LineLengthMeanDegenerateContract
{
    public static async Task<int> RunAsync(string evidenceDirectory)
    {
        string outputDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(outputDirectory);
        Environment.SetEnvironmentVariable(
            AppPathService.DataRootEnvironmentVariable,
            Path.Combine(outputDirectory, "runtime-data"));

        string repositoryRoot = FindRepositoryRoot();
        List<string> observations = new List<string>();
        List<string> failures = new List<string>();

        try
        {
            await VerifyNormalLineAsync(repositoryRoot, outputDirectory, observations).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            failures.Add("normal line: " + exception.GetBaseException().Message);
        }

        try
        {
            await VerifyPolarityInversionAsync(repositoryRoot, outputDirectory, observations).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            failures.Add("polarity inversion: " + exception.GetBaseException().Message);
        }

        try
        {
            await VerifyNormalMeanAsync(repositoryRoot, outputDirectory, observations).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            failures.Add("normal mean: " + exception.GetBaseException().Message);
        }

        try
        {
            await VerifyUniformMeanAcceptanceAsync(repositoryRoot, outputDirectory, observations).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            failures.Add("uniform mean: " + exception.GetBaseException().Message);
        }

        try
        {
            VerifyEmptyMeanInput(outputDirectory, observations);
        }
        catch (Exception exception)
        {
            failures.Add("empty mean input: " + exception.GetBaseException().Message);
        }

        try
        {
            VerifyEmptyRoiValidation(outputDirectory, observations);
        }
        catch (Exception exception)
        {
            failures.Add("empty ROI: " + exception.GetBaseException().Message);
        }

        try
        {
            await VerifyUniformLineAndSameLineAsync(outputDirectory, observations).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            failures.Add("line degenerates: " + exception.GetBaseException().Message);
        }

        string reportPath = Path.Combine(outputDirectory, "line-length-mean-degenerate-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-033 Line/Length/Mean degenerate input and finite result",
                "Owners: LineGauge -> VisionPipelineLineDistanceTool; MeanTool -> VisionPipelineMetricEnrichmentService/acceptance",
                "EvidenceDirectory: " + outputDirectory
            }
            .Concat(observations.Select(item => "PASS: " + item))
            .Concat(failures.Select(item => "FAIL: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Line/Length/Mean degenerate contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Line/Length/Mean degenerate contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static async Task VerifyNormalLineAsync(
        string repositoryRoot,
        string evidenceDirectory,
        ICollection<string> observations)
    {
        string imagePath = Path.Combine(repositoryRoot, "docs", "samples", "public", "Line_Pins_Synthetic_OK.png");
        string pipelinePath = Path.Combine(repositoryRoot, "docs", "samples", "public", "Public_Line_Pins_Distance.pipeline.xml");
        Require(File.Exists(imagePath) && File.Exists(pipelinePath), "public Line fixture or Pipeline XML is missing.");
        Require(SerializeHelper.TryLoadFromXmlFile(pipelinePath, out VisionPipeline pipeline, out Exception loadError),
            "could not load the Line Pipeline XML: " + loadError?.Message);

        using Mat source = Cv2.ImRead(imagePath, ImreadModes.AnyColor | ImreadModes.AnyDepth);
        Require(source != null && !source.Empty(), "could not load the Line fixture image.");
        using VisionRecipeRunResult run = await new VisionRecipeRunner().RunAsync(pipeline, source).ConfigureAwait(false);
        List<VisionRecipeStepRunSummary> summaries = run.Steps
            .Where(item => item.ToolType.Equals("LineDistance", StringComparison.OrdinalIgnoreCase))
            .ToList();
        Require(summaries.Count == 2, "normal LineDistance fixture did not expose both measurement steps.");
        Require(run.Success && summaries.All(item => item.ToolSuccess && item.Success),
            "normal LineDistance did not complete: " + string.Join(" | ", summaries.Select(item => item.Message)));
        foreach (VisionRecipeStepRunSummary summary in summaries)
        {
            RequireFinite(summary, VisionPipelineKnownMetrics.DistancePxMin, VisionPipelineKnownMetrics.DistancePxMax, VisionPipelineKnownMetrics.DistancePxAvg, VisionPipelineKnownMetrics.DistancePxRange);
            RequireFinite(summary, VisionPipelineKnownMetrics.DistanceMmMin, VisionPipelineKnownMetrics.DistanceMmMax, VisionPipelineKnownMetrics.DistanceMmAvg, VisionPipelineKnownMetrics.DistanceMmRange);
            Require(summary.Overlays.Any(item => item.Kind.Equals("Line", StringComparison.OrdinalIgnoreCase)),
                "normal LineDistance did not retain a distance overlay.");
        }
        VisionRecipeStepRunSummary firstSummary = summaries[0];
        File.Copy(imagePath, Path.Combine(evidenceDirectory, "normal-line-source.png"), true);
        File.Copy(pipelinePath, Path.Combine(evidenceDirectory, "normal-line.pipeline.xml"), true);
        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "normal-line-result.txt"),
            new[]
            {
                "Result: PASS",
                "ToolSuccess: " + firstSummary.ToolSuccess.ToString(CultureInfo.InvariantCulture),
                "DistancePxAvg: " + firstSummary.Metrics[VisionPipelineKnownMetrics.DistancePxAvg].ToString("R", CultureInfo.InvariantCulture),
                "DistanceMmAvg: " + firstSummary.Metrics[VisionPipelineKnownMetrics.DistanceMmAvg].ToString("R", CultureInfo.InvariantCulture),
                "OverlayCount: " + firstSummary.Overlays.Count.ToString(CultureInfo.InvariantCulture)
            });
        observations.Add("normal LineDistance published finite DistancePx/DistanceMm metrics and retained distance overlays.");
    }

    private static async Task VerifyPolarityInversionAsync(
        string repositoryRoot,
        string evidenceDirectory,
        ICollection<string> observations)
    {
        string imagePath = Path.Combine(repositoryRoot, "docs", "samples", "public", "Line_Pins_Synthetic_OK.png");
        string pipelinePath = Path.Combine(repositoryRoot, "docs", "samples", "public", "Public_Line_Pins_Distance.pipeline.xml");
        Require(SerializeHelper.TryLoadFromXmlFile(pipelinePath, out VisionPipeline pipeline, out Exception loadError),
            "could not load the polarity Pipeline XML: " + loadError?.Message);
        foreach (VisionPipelineStep step in pipeline.Steps.Where(item => item.ToolType.Equals("LineDistance", StringComparison.OrdinalIgnoreCase)))
        {
            step.Parameters["PRJ_PORALITY"] = "BTOW";
            step.Parameters["LeftPRJ_PORALITY"] = "BTOW";
            step.Parameters["RightPRJ_PORALITY"] = "BTOW";
        }

        using Mat source = Cv2.ImRead(imagePath, ImreadModes.AnyColor | ImreadModes.AnyDepth);
        Require(source != null && !source.Empty(), "could not load the polarity Line fixture image.");
        using Mat inverted = new Mat();
        Cv2.BitwiseNot(source, inverted);
        using VisionRecipeRunResult run = await new VisionRecipeRunner().RunAsync(pipeline, inverted).ConfigureAwait(false);
        List<VisionRecipeStepRunSummary> summaries = run.Steps
            .Where(item => item.ToolType.Equals("LineDistance", StringComparison.OrdinalIgnoreCase))
            .ToList();
        Require(summaries.Count == 2, "inverted-polarity fixture did not expose both measurement steps.");
        Require(run.Success && summaries.All(item => item.ToolSuccess && item.Success),
            "inverted-polarity LineDistance did not complete: " + string.Join(" | ", summaries.Select(item => item.Message)));
        foreach (VisionRecipeStepRunSummary summary in summaries)
        {
            RequireFinite(summary, VisionPipelineKnownMetrics.DistancePxAvg, VisionPipelineKnownMetrics.DistanceMmAvg);
        }
        Cv2.ImWrite(Path.Combine(evidenceDirectory, "inverted-polarity-source.png"), inverted);
        observations.Add("inverted source with the opposite LineGauge polarity retained finite pixel/mm distance metrics.");
    }

    private static async Task VerifyNormalMeanAsync(
        string repositoryRoot,
        string evidenceDirectory,
        ICollection<string> observations)
    {
        string imagePath = Path.Combine(repositoryRoot, "docs", "samples", "public", "Mean_Brightness_Synthetic_OK.png");
        string pipelinePath = Path.Combine(repositoryRoot, "docs", "samples", "public", "Public_Mean_BrightnessDrift.pipeline.xml");
        Require(SerializeHelper.TryLoadFromXmlFile(pipelinePath, out VisionPipeline pipeline, out Exception loadError),
            "could not load the Mean Pipeline XML: " + loadError?.Message);
        using Mat source = Cv2.ImRead(imagePath, ImreadModes.AnyColor | ImreadModes.AnyDepth);
        Require(source != null && !source.Empty(), "could not load the Mean fixture image.");
        using VisionRecipeRunResult run = await new VisionRecipeRunner().RunAsync(pipeline, source).ConfigureAwait(false);
        VisionRecipeStepRunSummary summary = run.Steps.Single();
        Require(run.Success && summary.ToolSuccess && summary.Success,
            "normal Mean did not complete: " + summary.Message);
        RequireFinite(summary, VisionPipelineKnownMetrics.MeanValueMin, VisionPipelineKnownMetrics.MeanValueMax, VisionPipelineKnownMetrics.MeanValueAvg);
        File.Copy(imagePath, Path.Combine(evidenceDirectory, "normal-mean-source.png"), true);
        observations.Add("normal Mean published finite MeanValueMin/Max/Avg metrics with the configured acceptance band.");
    }

    private static async Task VerifyUniformMeanAcceptanceAsync(
        string repositoryRoot,
        string evidenceDirectory,
        ICollection<string> observations)
    {
        string pipelinePath = Path.Combine(repositoryRoot, "docs", "samples", "public", "Public_Mean_BrightnessDrift.pipeline.xml");
        Require(SerializeHelper.TryLoadFromXmlFile(pipelinePath, out VisionPipeline pipeline, out Exception loadError),
            "could not load the uniform Mean Pipeline XML: " + loadError?.Message);
        using Mat source = new Mat(new Size(160, 120), MatType.CV_8UC1, Scalar.All(0));
        using VisionRecipeRunResult run = await new VisionRecipeRunner().RunAsync(pipeline, source).ConfigureAwait(false);
        VisionRecipeStepRunSummary summary = run.Steps.Single();
        Require(summary.ToolSuccess, "uniform Mean did not produce a tool result: " + summary.Message);
        RequireFinite(summary, VisionPipelineKnownMetrics.MeanValueAvg);
        Require(summary.AcceptanceEvaluated && !summary.AcceptancePassed,
            "uniform dark Mean was not rejected by the configured acceptance band.");
        Cv2.ImWrite(Path.Combine(evidenceDirectory, "uniform-dark-mean-source.png"), source);
        observations.Add("uniform dark Mean remained finite but was rejected by the configured acceptance band instead of being accepted as a normal brightness result.");
    }

    private static void VerifyEmptyMeanInput(string evidenceDirectory, ICollection<string> observations)
    {
        VisionPipelineStep step = VisionPipelineStepBuilder.FromProperty(
            new MeanProperty("EmptyMean")
            {
                MEAN_MIN = 0,
                MEAN_MAX = 255,
                MEAN_TYPES = MeanType.Mean
            },
            "Main",
            "MeanResult");
        IVisionTool tool = VisionPipelineAppToolFactory.Create(step);
        using (tool as IDisposable)
        using (Mat empty = new Mat())
        using (VisionToolResult result = tool.Execute(empty))
        {
            Require(result != null && !result.Success, "empty Mean input unexpectedly succeeded.");
            Require(!string.IsNullOrWhiteSpace(result.Message), "empty Mean input did not retain an explicit failure reason.");
            File.WriteAllLines(
                Path.Combine(evidenceDirectory, "empty-mean-input.txt"),
                new[] { "Result: PASS", "Success: false", "Message: " + result.Message });
        }

        observations.Add("empty Mean input failed closed with an explicit reason and no accepted metric.");
    }

    private static void VerifyEmptyRoiValidation(string evidenceDirectory, ICollection<string> observations)
    {
        MeanProperty property = new MeanProperty("EmptyRoiMean")
        {
            MEAN_MIN = 0,
            MEAN_MAX = 255,
            USE_ROI = true,
            CvROI = new Rect(12, 12, 0, 20)
        };
        VisionPipeline pipeline = new VisionPipeline
        {
            Name = "2D033_EmptyRoi"
        };
        pipeline.Steps.Add(VisionPipelineStepBuilder.FromProperty(property, "Main", "MeanResult"));
        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, new[] { "Main" });
        Require(!validation.Success && validation.Errors.Any(error => error.Contains("ROI", StringComparison.OrdinalIgnoreCase)),
            "empty Mean ROI was not rejected by pipeline validation.");
        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "empty-mean-roi-validation.txt"),
            new[] { "Result: PASS", "ValidationSuccess: false" }.Concat(validation.Errors));
        observations.Add("empty Mean ROI failed at the existing validation boundary with an explicit ROI error.");
    }

    private static async Task VerifyUniformLineAndSameLineAsync(
        string evidenceDirectory,
        ICollection<string> observations)
    {
        LineGaugeProperty left = CreateLineProperty("Left", FormulaUtil.PROJECTION_DIR.X_LTOR);
        LineGaugeProperty right = CreateLineProperty("Right", FormulaUtil.PROJECTION_DIR.X_LTOR);
        VisionPipeline pipeline = new VisionPipeline
        {
            Name = "2D033_LineDegenerate"
        };
        pipeline.Steps.Add(VisionPipelineStepBuilder.FromLineGaugePair(
            "SameLineDistance",
            "LineDistance",
            left,
            right,
            VisionRecipeRunner.DefaultInputLayer,
            "SameLineResult",
            "Measure"));

        using Mat uniform = new Mat(new Size(240, 140), MatType.CV_8UC1, Scalar.All(80));
        using VisionRecipeRunResult uniformRun = await new VisionRecipeRunner().RunAsync(pipeline, uniform).ConfigureAwait(false);
        VisionRecipeStepRunSummary uniformSummary = uniformRun.Steps.Single();
        Require(!uniformRun.Success && !uniformSummary.ToolSuccess,
            "uniform LineDistance unexpectedly succeeded.");
        Require(!string.IsNullOrWhiteSpace(uniformSummary.Message), "uniform LineDistance did not retain an explicit failure reason.");

        using Mat oneEdge = new Mat(new Size(240, 140), MatType.CV_8UC1, Scalar.All(0));
        Cv2.Rectangle(oneEdge, new Rect(80, 20, 120, 100), Scalar.All(220), -1);
        using VisionRecipeRunResult sameLineRun = await new VisionRecipeRunner().RunAsync(pipeline, oneEdge).ConfigureAwait(false);
        VisionRecipeStepRunSummary sameLineSummary = sameLineRun.Steps.Single();
        Require(!sameLineRun.Success && !sameLineSummary.ToolSuccess,
            "same-line LineDistance unexpectedly succeeded.");
        Require(sameLineSummary.Message.Contains("positive finite", StringComparison.OrdinalIgnoreCase)
            || sameLineSummary.Message.Contains("coincident", StringComparison.OrdinalIgnoreCase)
            || sameLineSummary.Message.Contains("parallel", StringComparison.OrdinalIgnoreCase),
            "same-line LineDistance did not retain a geometry-specific failure reason: " + sameLineSummary.Message);
        Require(!sameLineSummary.Metrics.Values.Any(value => double.IsNaN(value) || double.IsInfinity(value)),
            "same-line LineDistance published a non-finite metric.");
        Cv2.ImWrite(Path.Combine(evidenceDirectory, "same-line-source.png"), oneEdge);
        File.WriteAllLines(
            Path.Combine(evidenceDirectory, "line-degenerate-results.txt"),
            new[]
            {
                "Result: PASS",
                "UniformMessage: " + uniformSummary.Message,
                "SameLineMessage: " + sameLineSummary.Message
            });
        observations.Add("uniform/no-edge and coincident same-line LineDistance cases failed closed with explicit reasons and no non-finite metrics.");
    }

    private static LineGaugeProperty CreateLineProperty(string name, FormulaUtil.PROJECTION_DIR direction)
    {
        return new LineGaugeProperty(name)
        {
            USE_THRESHOLD = false,
            USE_ADAPTIVE_THRESHOLD = false,
            USE_BITWISENOT = false,
            USE_ROI = true,
            CvROI = new Rect(40, 20, 160, 100),
            PRJ_PORALITY = FormulaUtil.PROJECTION_POLARITY.BTOW,
            PRJ_DIR = direction,
            VER_PRJ_DIR = FormulaUtil.PROJECTION_DIR.Y_TTOB,
            CONTRAST = 20,
            THICKNESS = 2,
            SAMPLING_STEP = 4,
            POINT_RANGE = 8,
            USE_EXTEND_FIT_LINE = false,
            PIXELPERMM = 0
        };
    }

    private static void RequireFinite(VisionRecipeStepRunSummary summary, params string[] metricNames)
    {
        foreach (string metricName in metricNames)
        {
            Require(summary.Metrics.TryGetValue(metricName, out double value) && double.IsFinite(value),
                $"metric {metricName} was missing or non-finite.");
        }
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "OpenVisionLab.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        return Directory.GetCurrentDirectory();
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
