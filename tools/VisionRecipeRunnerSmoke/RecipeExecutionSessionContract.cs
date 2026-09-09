using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

internal static class RecipeExecutionSessionContract
{
    private const string PipelineName = "Session_Mean";

    internal static async Task<int> RunAsync(string requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory);
        string runtimeDirectory = Path.GetFullPath(AppContext.BaseDirectory);
        string? previousDataRoot = Environment.GetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable);
        List<string> observations = new();
        List<string> failures = new();
        List<string> createdCatalogs = new();
        string recipeName = "Smoke_ExecutionSession_" + Guid.NewGuid().ToString("N")[..12];

        try
        {
            Require(string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase)
                && IsWithin(evidenceDirectory, runtimeDirectory),
                "Run the copied test runtime inside the supplied D-drive evidence directory; repository catalogs must not be modified.");
            Directory.CreateDirectory(evidenceDirectory);
            string dataRoot = Path.Combine(evidenceDirectory, "data", recipeName);
            Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);
            Require(string.Equals(Path.GetFullPath(AppPathService.DataRootDirectory), Path.GetFullPath(dataRoot), StringComparison.OrdinalIgnoreCase),
                "AppPathService was initialized before the isolated test data-root override.");

            string fixtureDirectory = Path.Combine(evidenceDirectory, "fixtures", recipeName);
            Directory.CreateDirectory(fixtureDirectory);
            string goodImage = WriteImage(fixtureDirectory, "good.bmp", 200);
            string badImage = WriteImage(fixtureDirectory, "bad.bmp", 40);
            VisionPipelineStorage.Save(recipeName, CreatePipeline());
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, PipelineName);
            byte[] pipelineXml = File.ReadAllBytes(pipelinePath);
            string catalogDirectory = Path.Combine(runtimeDirectory, "docs", "samples");
            Directory.CreateDirectory(catalogDirectory);
            string publicCatalog = Path.Combine(catalogDirectory, "OpenVisionLab.PublicSampleCatalog.csv");
            string productCatalog = Path.Combine(catalogDirectory, "OpenVisionLab.ProductSampleCatalog.csv");
            Require(!File.Exists(publicCatalog) && !File.Exists(productCatalog),
                "The isolated runtime already contains sample catalogs; use a fresh copied runtime without catalog data.");

            WriteCatalog(publicCatalog, new[]
            {
                CatalogRow("Pair_Bad", badImage, pipelinePath, "ExpectedFailure", "40", "FixturePair", "Bad"),
                CatalogRow("Pair_Good", goodImage, pipelinePath, "Required", "200", "FixturePair", "Good")
            });
            createdCatalogs.Add(publicCatalog);
            string[] expectedCatalogOrder = { "A_01", "A_02", "A_03", "A_04", "B_01", "B_02", "B_03", "B_04", "None_01", "None_02", "None_03", "None_04" };
            WriteCatalog(productCatalog, expectedCatalogOrder.Reverse().Select(name => CatalogRow(
                name, goodImage, pipelinePath, "Required", "200",
                name.StartsWith("A_", StringComparison.Ordinal) ? " a " : name.StartsWith("B_", StringComparison.Ordinal) ? "B" : string.Empty,
                "Good")));
            createdCatalogs.Add(productCatalog);

            List<VisionPipelineSampleCatalogItem> publicSamples = VisionPipelineSampleCatalogItem.LoadRunnable(VisionPipelineSampleCatalogSourceKind.Public);
            List<VisionPipelineSampleCatalogItem> productSamples = VisionPipelineSampleCatalogItem.LoadRunnable(VisionPipelineSampleCatalogSourceKind.Product);
            Require(publicSamples.Count == 2 && productSamples.Count == 12
                && publicSamples.All(sample => string.Equals(sample.CatalogSourcePath, publicCatalog, StringComparison.OrdinalIgnoreCase))
                && productSamples.All(sample => string.Equals(sample.CatalogSourcePath, productCatalog, StringComparison.OrdinalIgnoreCase)),
                "Catalog resolution escaped the isolated two-row Public and twelve-row Product fixtures.");
            OpenVisionRecipeSampleOption goodOption = new(publicSamples.Single(sample => sample.PairRole == "Good"));
            OpenVisionRecipeValidationSetOption localOption = CreateLocalSet(goodImage, badImage);

            await RunCaseAsync("selected sample has context and produces no persisted report", async () =>
            {
                OpenVisionRecipeExecutionSessionViewModel session = new();
                int reportsBefore = CountReports(dataRoot);
                await ObserveRunAsync(session, recipeName, PipelineName,
                    () => session.RunSelectedSampleCheckAsync(recipeName, PipelineName, goodOption),
                    () => session.IsSampleCheckRunning && !session.IsValidationSuiteRunning, expectedSaves: 0);
                Require(session.LatestSampleRunSummary.HasResult && session.LatestSampleRunSummary.Succeeded
                    && session.LatestSampleRunSummary.IsForRecipePipeline(recipeName, PipelineName)
                    && session.LatestSampleRunSummary.SampleName == "Pair_Good", "The selected sample lost its successful recipe/pipeline context.");
                Require(CountReports(dataRoot) == reportsBefore, "The normal selected check unexpectedly persisted a report.");
            }, observations, failures);

            await RunCaseAsync("selected suite persists one linked report and batch", async () =>
            {
                OpenVisionRecipeExecutionSessionViewModel session = new();
                await ObserveRunAsync(session, recipeName, PipelineName,
                    () => session.RunSelectedSampleValidationSuiteAsync(recipeName, PipelineName, goodOption),
                    () => session.IsSampleCheckRunning && session.IsValidationSuiteRunning && !session.CanStop, expectedSaves: 1);
                VisionPipelineBatchRunSummary summary = LatestSummary(recipeName, "SelectedSample");
                Require(summary.SuiteName == "Selected:Pair_Good" && summary.Results.Count == 1
                    && session.LatestSampleRunSummary.Succeeded, "Selected suite metadata or final binding summary changed.");
                RequireReports(summary, 1);
            }, observations, failures);

            await RunCaseAsync("pair retains Good/Bad order and successful rejection", async () =>
            {
                OpenVisionRecipeExecutionSessionViewModel session = new();
                await ObserveRunAsync(session, recipeName, PipelineName,
                    () => session.RunSelectedSamplePairCheckAsync(recipeName, PipelineName, goodOption),
                    () => session.IsPairCheckRunning && !session.IsSampleCheckRunning, expectedSaves: 1);
                VisionPipelineBatchRunSummary summary = LatestSummary(recipeName, "GoodBadPair");
                Require(summary.SuiteName == "Pair:FixturePair"
                    && summary.Results.Select(result => result.SampleName).SequenceEqual(new[] { "Pair_Good", "Pair_Bad" })
                    && summary.Results.All(result => result.Success)
                    && session.LatestPairRunSummary.HasResult && session.LatestPairRunSummary.Succeeded
                    && session.LatestPairRunSummary.SampleResults.Count == 2,
                    "Pair ordering, expected-failure handling, or final binding summary changed.");
                RequireReports(summary, 2);
            }, observations, failures);

            await RunCaseAsync("bounded catalog sorts groups and reports progress at 10/12 and 12/12", async () =>
            {
                OpenVisionRecipeExecutionSessionViewModel session = new();
                List<string> progress = new();
                session.PropertyChanged += (_, change) =>
                {
                    if (change.PropertyName == nameof(session.LatestCatalogBenchmarkSummary))
                        progress.Add(session.LatestCatalogBenchmarkSummary.CompactText);
                };
                await ObserveRunAsync(session, recipeName, PipelineName,
                    () => session.RunCatalogBenchmarkAsync(recipeName, PipelineName),
                    () => session.IsCatalogBenchmarkRunning && !session.IsValidationSuiteRunning, expectedSaves: 1);
                VisionPipelineBatchRunSummary summary = LatestSummary(recipeName, "Catalog");
                Require(summary.SuiteName == "Catalog" && summary.Results.Select(result => result.SampleName).SequenceEqual(expectedCatalogOrder),
                    "Catalog sorting or bounded fixture membership changed.");
                Require(progress.Count == 4 && progress[1].Contains("10/12", StringComparison.Ordinal)
                    && progress[2].Contains("12/12", StringComparison.Ordinal)
                    && session.LatestCatalogBenchmarkSummary.HasResult && session.LatestCatalogBenchmarkSummary.Succeeded,
                    "Catalog progress cadence or final binding summary changed.");
                RequireReports(summary, 12);
            }, observations, failures);

            await RunCaseAsync("local set persists explicit outcomes and copied image metadata", async () =>
            {
                OpenVisionRecipeExecutionSessionViewModel session = new();
                await ObserveRunAsync(session, recipeName, PipelineName,
                    () => session.RunLocalValidationSetAsync(recipeName, PipelineName, localOption),
                    () => session.IsValidationSuiteRunning && session.IsLocalValidationSetRunning && session.CanStop, expectedSaves: 1);
                VisionPipelineBatchRunSummary summary = LatestSummary(recipeName, "LocalValidationSet");
                Require(summary.SuiteName == "LocalFixture" && summary.Results.Count == 3
                    && summary.Results.All(result => result.ExecutionState == VisionPipelineBatchOutcomeContract.CompletedState && result.JudgmentCorrect)
                    && summary.Results[1].ActualOutcome == VisionPipelineBatchOutcomeContract.NgOutcome
                    && summary.Results[1].ExpectedOutcome == VisionPipelineBatchOutcomeContract.NgOutcome
                    && summary.Results[1].VariantId == "DarkReject"
                    && summary.Results[1].ExpectedMetricName == "MeanValueAvg"
                    && summary.Results[1].ExpectedMetricMinimum == "40"
                    && summary.Results[1].Message.Contains("Note: dark fixture", StringComparison.Ordinal),
                    "Local-set outcome, metric, variant, or notes were not preserved.");
                RequireReports(summary, 3);
            }, observations, failures);

            await RunCaseAsync("local stop saves exactly the first completed image as partial", async () =>
            {
                OpenVisionRecipeExecutionSessionViewModel session = new();
                bool acceptedStop = false;
                session.PropertyChanged += (_, change) =>
                {
                    if (change.PropertyName == nameof(session.StatusText) && session.StatusText.Contains("(1/3)", StringComparison.Ordinal))
                        acceptedStop = session.RequestStop("Stop after the first completed image.");
                };
                await ObserveRunAsync(session, recipeName, PipelineName,
                    () => session.RunLocalValidationSetAsync(recipeName, PipelineName, localOption),
                    () => session.IsValidationSuiteRunning && session.IsLocalValidationSetRunning, expectedSaves: 1, expectedCommandChanges: 3);
                VisionPipelineBatchRunSummary summary = LatestSummary(recipeName, "LocalValidationSetPartial");
                Require(acceptedStop && summary.Results.Count == 1
                    && summary.Notes.Contains("not a full-set accuracy or timing baseline", StringComparison.OrdinalIgnoreCase),
                    "The stop boundary did not retain a one-image partial result with explicit limitations.");
                RequireReports(summary, 1);
            }, observations, failures);

            foreach (string workflow in new[] { "selected", "suite", "pair", "catalog", "local" })
            {
                await RunCaseAsync(workflow + " missing XML restores flags without a saved-batch notification", async () =>
                {
                    OpenVisionRecipeExecutionSessionViewModel session = new();
                    string missingPipeline = "Missing_" + workflow;
                    Func<Task> run = workflow switch
                    {
                        "selected" => () => session.RunSelectedSampleCheckAsync(recipeName, missingPipeline, goodOption),
                        "suite" => () => session.RunSelectedSampleValidationSuiteAsync(recipeName, missingPipeline, goodOption),
                        "pair" => () => session.RunSelectedSamplePairCheckAsync(recipeName, missingPipeline, goodOption),
                        "catalog" => () => session.RunCatalogBenchmarkAsync(recipeName, missingPipeline),
                        _ => () => session.RunLocalValidationSetAsync(recipeName, missingPipeline, localOption)
                    };
                    int reportCount = CountReports(dataRoot);
                    await ObserveRunAsync(session, recipeName, missingPipeline, run, () => !IsIdle(session), expectedSaves: 0);
                    Require(session.ExecutionStatusText.Contains("ERROR", StringComparison.OrdinalIgnoreCase)
                        && CountReports(dataRoot) == reportCount, "Missing XML did not fail without a new report.");
                    Require(workflow switch
                    {
                        "selected" or "suite" => session.LatestSampleRunSummary.HasResult && !session.LatestSampleRunSummary.Succeeded,
                        "pair" => session.LatestPairRunSummary.HasResult && !session.LatestPairRunSummary.Succeeded,
                        "catalog" => session.LatestCatalogBenchmarkSummary.HasResult && !session.LatestCatalogBenchmarkSummary.Succeeded,
                        _ => true
                    }, "The failure result was not projected to its binding summary.");
                }, observations, failures);
            }

            Require(File.ReadAllBytes(pipelinePath).SequenceEqual(pipelineXml), "Executing validation changed the saved Recipe XML.");
            observations.Add("Recipe XML remained byte-for-byte identical; all evidence and generated inputs stayed on D:");
        }
        catch (Exception exception)
        {
            failures.Add(exception.GetBaseException().Message);
        }
        finally
        {
            foreach (string path in createdCatalogs)
                File.Delete(path);
            Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, previousDataRoot);
        }

        if (string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase) && Directory.Exists(evidenceDirectory))
        {
            string reportPath = Path.Combine(evidenceDirectory, "recipe-execution-session-contract.txt");
            File.WriteAllLines(reportPath, new[] { "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"), "Runtime: " + runtimeDirectory, "Recipe: " + recipeName }
                .Concat(observations.Select(item => "PASS: " + item)).Concat(failures.Select(item => "FAIL: " + item)));
            Console.WriteLine(reportPath);
        }
        foreach (string failure in failures)
            Console.Error.WriteLine(failure);
        Console.WriteLine($"Recipe execution session contract: {observations.Count} passed, {failures.Count} failed.");
        return failures.Count == 0 ? 0 : 1;
    }

    private static async Task ObserveRunAsync(OpenVisionRecipeExecutionSessionViewModel session, string recipeName, string pipelineName,
        Func<Task> run, Func<bool> expectedRunningState, int expectedSaves, int expectedCommandChanges = 2)
    {
        int batchesBefore = VisionPipelineBatchRunSummaryStorage.List(recipeName, pipelineName).Count;
        int savedEvents = 0;
        bool savedEventHadRunningState = true;
        bool savedEventHadPersistedBatch = true;
        List<bool> commandStates = new();
        HashSet<string> changedProperties = new(StringComparer.Ordinal);
        EventHandler commandHandler = (_, _) => commandStates.Add(IsIdle(session));
        EventHandler savedHandler = (_, _) =>
        {
            savedEvents++;
            savedEventHadRunningState &= !IsIdle(session);
            savedEventHadPersistedBatch &= VisionPipelineBatchRunSummaryStorage.List(recipeName, pipelineName).Count == batchesBefore + savedEvents;
        };
        PropertyChangedEventHandler propertyHandler = (_, change) => changedProperties.Add(change.PropertyName ?? string.Empty);
        session.CommandStateChanged += commandHandler;
        session.BatchRunSaved += savedHandler;
        session.PropertyChanged += propertyHandler;
        bool enteredRunningState = false;
        EventHandler startHandler = (_, _) =>
        {
            if (commandStates.Count == 1)
                enteredRunningState = expectedRunningState();
        };
        session.CommandStateChanged += startHandler;
        try
        {
            await run();
            Require(enteredRunningState && IsIdle(session) && !session.CanStop && !session.StopRequested,
                "A workflow did not expose the expected starting state or restore its completion flags.");
            Require(savedEvents == expectedSaves
                && VisionPipelineBatchRunSummaryStorage.List(recipeName, pipelineName).Count == batchesBefore + expectedSaves,
                "Saved-batch notifications and persisted batch count disagreed.");
            Require(savedEventHadRunningState && savedEventHadPersistedBatch,
                "BatchRunSaved did not preserve the persisted-batch-before-idle boundary.");
            Require(commandStates.Count == expectedCommandChanges && !commandStates[0] && commandStates[^1],
                "Command refresh notifications did not retain the busy-to-idle boundaries.");
            Require(changedProperties.Contains(nameof(session.ExecutionStatusText)), "The shell status binding was not notified.");
        }
        finally
        {
            session.CommandStateChanged -= startHandler;
            session.CommandStateChanged -= commandHandler;
            session.BatchRunSaved -= savedHandler;
            session.PropertyChanged -= propertyHandler;
        }
    }

    private static async Task RunCaseAsync(string name, Func<Task> run, List<string> observations, List<string> failures)
    {
        try
        {
            await run();
            observations.Add(name);
        }
        catch (Exception exception)
        {
            failures.Add(name + ": " + exception.GetBaseException().Message);
        }
    }

    private static VisionPipeline CreatePipeline()
    {
        VisionPipeline pipeline = new() { Name = PipelineName };
        VisionPipelineStep step = new()
        {
            Name = "01 Mean", ToolType = "Mean", Enabled = true, InputLayer = "Main", OutputLayer = "Mean_Output",
            UseAcceptance = true, ExpectedSuccess = true, AcceptanceMetricName = "MeanValueAvg",
            UseAcceptanceMetricMinimum = true, AcceptanceMetricMinimum = 150,
            UseAcceptanceMetricMaximum = true, AcceptanceMetricMaximum = 255
        };
        step.Parameters["MEAN_TYPES"] = "Mean";
        step.Parameters["USE_THRESHOLD"] = "false";
        step.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
        step.Parameters["USE_BITWISENOT"] = "false";
        step.Parameters["USE_ROI"] = "false";
        step.Parameters["USE_MULTI_ROI"] = "false";
        pipeline.Steps.Add(step);
        return pipeline;
    }

    private static OpenVisionRecipeValidationSetOption CreateLocalSet(string goodImage, string badImage)
    {
        return new OpenVisionRecipeValidationSetOption(new OpenVisionRecipeValidationSet
        {
            Name = "LocalFixture", Notes = "Three-image session fixture",
            Images = new List<OpenVisionRecipeValidationSetImage>
            {
                new() { Path = goodImage, Expected = "OK", VariantId = "BrightReference" },
                new() { Path = badImage, Expected = "NG", VariantId = "DarkReject", ExpectedMetricName = "MeanValueAvg", ExpectedMetricMinimum = "40", ExpectedMetricMaximum = "40", Notes = "dark fixture" },
                new() { Path = goodImage, Expected = "OK", VariantId = "BrightRepeat" }
            }
        });
    }

    private static string WriteImage(string directory, string name, int gray)
    {
        string path = Path.Combine(directory, name);
        using Bitmap image = new(8, 8, PixelFormat.Format24bppRgb);
        using (Graphics graphics = Graphics.FromImage(image))
            graphics.Clear(Color.FromArgb(gray, gray, gray));
        image.Save(path, ImageFormat.Bmp);
        return path;
    }

    private static string CatalogRow(string name, string image, string pipeline, string mode, string mean, string group, string role)
    {
        return string.Join(",", new[] { name, image, "8", "8", pipeline, mode, "MeanValueAvg", mean, mean, group, role }
            .Select(value => "\"" + value.Replace("\"", "\"\"") + "\""));
    }

    private static void WriteCatalog(string path, IEnumerable<string> rows)
    {
        File.WriteAllLines(path, new[] { "SampleName,ImagePath,Width,Height,BaselinePipeline,ValidationMode,ExpectedMetricName,ExpectedMetricMinimum,ExpectedMetricMaximum,PairGroup,PairRole" }.Concat(rows), new UTF8Encoding(false));
    }

    private static VisionPipelineBatchRunSummary LatestSummary(string recipeName, string suiteKind)
    {
        return VisionPipelineBatchRunSummaryStorage.List(recipeName, PipelineName)
            .Select(info => VisionPipelineBatchRunSummaryStorage.Load(info.SummaryPath))
            .First(summary => summary.SuiteKind == suiteKind);
    }

    private static void RequireReports(VisionPipelineBatchRunSummary summary, int expectedCount)
    {
        Require(summary.Results.Count == expectedCount && summary.Results.All(result => File.Exists(result.RunReportPath))
            && summary.Results.Select(result => result.RunReportPath).Distinct(StringComparer.OrdinalIgnoreCase).Count() == expectedCount,
            "Saved results did not retain one distinct existing run report per sample.");
    }

    private static int CountReports(string dataRoot) => Directory.GetFiles(dataRoot, "report.xml", SearchOption.AllDirectories).Length;

    private static bool IsIdle(OpenVisionRecipeExecutionSessionViewModel session) => !session.IsSampleCheckRunning && !session.IsPairCheckRunning
        && !session.IsCatalogBenchmarkRunning && !session.IsValidationSuiteRunning && !session.IsLocalValidationSetRunning;

    private static bool IsWithin(string directory, string path) => path.StartsWith(
        directory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
