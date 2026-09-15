using OpenCvSharp;
using OpenVisionLab;
using OpenVisionLab.Common;
using OpenVisionLab.Core;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

internal static class VideoSizeMemoryBaselineContract
{
    private const int WarmupRuns = 5;
    private const int MeasuredRuns = 30;

    public static async Task<int> RunAsync(string evidenceDirectory, bool includeTwentyThousand)
    {
        string outputDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(outputDirectory);
        Environment.SetEnvironmentVariable(
            AppPathService.DataRootEnvironmentVariable,
            Path.Combine(outputDirectory, "runtime-data"));

        string repositoryRoot = FindRepositoryRoot();
        string sdkAssemblyPath = typeof(VisionRecipeRunner).Assembly.Location;
        string sdkAssemblySha256 = File.Exists(sdkAssemblyPath) ? ComputeSha256(sdkAssemblyPath) : string.Empty;
        string gitCommit = ResolveGitCommit(repositoryRoot);
        List<string> observations = new List<string>();
        List<string> failures = new List<string>();
        List<BaselineRow> rows = new List<BaselineRow>();
        List<ScenarioAggregate> aggregates = new List<ScenarioAggregate>();
        string perRunCsvPath = Path.Combine(outputDirectory, "video-size-memory-baseline-runs.csv");

        using (StreamWriter writer = new StreamWriter(perRunCsvPath, false, new UTF8Encoding(false)))
        {
            writer.WriteLine(BaselineRow.Header);
            foreach (SizeSpec size in CreateSizeSpecs(includeTwentyThousand))
            {
                foreach (bool twoLayer in new[] { false, true })
                {
                    try
                    {
                        ScenarioAggregate aggregate = await MeasureScenarioAsync(
                            repositoryRoot,
                            outputDirectory,
                            size,
                            twoLayer,
                            sdkAssemblyPath,
                            sdkAssemblySha256,
                            gitCommit,
                            writer,
                            rows).ConfigureAwait(false);
                        aggregates.Add(aggregate);
                        observations.Add(
                            $"{size.Width}x{size.Height} {(twoLayer ? "two-layer" : "single-layer")} "
                            + $"passed {MeasuredRuns} measured runs after {WarmupRuns} warm-ups; "
                            + $"calculation P50/P95={aggregate.CalculationP50Milliseconds:0.###}/{aggregate.CalculationP95Milliseconds:0.###} ms.");
                    }
                    catch (Exception exception)
                    {
                        failures.Add(
                            $"{size.Width}x{size.Height} {(twoLayer ? "two-layer" : "single-layer")}: "
                            + exception.GetBaseException().Message);
                    }
                }
            }
        }

        string aggregateCsvPath = Path.Combine(outputDirectory, "video-size-memory-baseline-summary.csv");
        File.WriteAllLines(
            aggregateCsvPath,
            new[] { ScenarioAggregate.Header }
                .Concat(aggregates.Select(item => item.ToCsvLine())),
            new UTF8Encoding(false));

        string preflightPath = Path.Combine(outputDirectory, "video-size-memory-preflight.txt");
        File.WriteAllLines(
            preflightPath,
            CreatePreflightLines(includeTwentyThousand));

        string reportPath = Path.Combine(outputDirectory, "video-size-memory-baseline.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-034 supported image-size memory, timing, and copy baseline",
                "Owner: VisionRecipeRunner -> existing Mean tool -> VisionRecipeRunResult -> BitmapImageConverter",
                "RepositoryCommit: " + gitCommit,
                "SdkAssembly: " + sdkAssemblyPath,
                "SdkAssemblySha256: " + sdkAssemblySha256,
                "WarmupRuns: " + WarmupRuns.ToString(CultureInfo.InvariantCulture),
                "MeasuredRuns: " + MeasuredRuns.ToString(CultureInfo.InvariantCulture),
                "PerRunCsv: " + perRunCsvPath,
                "AggregateCsv: " + aggregateCsvPath,
                "Preflight: " + preflightPath,
                "Scope: measurement only; no production owner, diagnostic policy, or size limit was changed.",
                "Unverified: WPF rendering, monitor/DPI, cancellation, low-memory failure, camera, hardware, and long-duration qualification."
            }
            .Concat(observations.Select(item => "PASS: " + item))
            .Concat(failures.Select(item => "FAIL: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Video-size memory baseline contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Video-size memory baseline contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static async Task<ScenarioAggregate> MeasureScenarioAsync(
        string repositoryRoot,
        string outputDirectory,
        SizeSpec size,
        bool twoLayer,
        string sdkAssemblyPath,
        string sdkAssemblySha256,
        string gitCommit,
        StreamWriter writer,
        ICollection<BaselineRow> rows)
    {
        string scenarioName = $"{size.Width}x{size.Height}_{(twoLayer ? "two-layer" : "single-layer")}";
        string pipelineXml = BuildMeanPipelineXml(twoLayer ? 2 : 1, scenarioName);
        byte[] pipelineBytes = Encoding.UTF8.GetBytes(pipelineXml);
        string recipeSha256 = ComputeSha256(pipelineBytes);
        string pipelinePath = Path.Combine(outputDirectory, scenarioName + ".pipeline.xml");
        File.WriteAllText(pipelinePath, pipelineXml, new UTF8Encoding(false));

        Require(
            SerializeHelper.TryLoadFromXmlBytes(pipelineBytes, out VisionPipeline pipeline, out Exception loadError)
                && pipeline != null,
            "could not load generated Mean Pipeline: " + loadError?.Message);

        Stopwatch inputAllocationWatch = Stopwatch.StartNew();
        using Mat fixtureMat = new Mat(size.Height, size.Width, MatType.CV_8UC1, Scalar.All(200));
        inputAllocationWatch.Stop();
        string fixtureMatSha256 = ComputeMatSha256(fixtureMat);
        Stopwatch fixtureBitmapWatch = Stopwatch.StartNew();
        using System.Drawing.Bitmap fixtureBitmap = BitmapImageConverter.ToBitmap(fixtureMat);
        fixtureBitmapWatch.Stop();
        long inputBytes = EstimateMatBytes(fixtureMat);
        long bitmapBytes = EstimateBitmapBytes(fixtureBitmap);
        long resultBytes = inputBytes;
        long layerCacheBytes = checked(inputBytes + (resultBytes * (twoLayer ? 2L : 1L)));

        for (int warmup = 1; warmup <= WarmupRuns; warmup++)
        {
            using Mat input = BitmapImageConverter.ToMat(fixtureBitmap);
            using VisionRecipeRunResult result = await new VisionRecipeRunner().RunAsync(pipeline, input).ConfigureAwait(false);
            VerifyRun(result, twoLayer, size, warmup, true);
        }

        List<double> conversionTimes = new List<double>(MeasuredRuns);
        List<double> calculationTimes = new List<double>(MeasuredRuns);
        List<double> contextCloneTimes = new List<double>(MeasuredRuns);
        List<double> bitmapPublishTimes = new List<double>(MeasuredRuns);
        List<double> totalTimes = new List<double>(MeasuredRuns);
        List<long> privatePeakBytes = new List<long>(MeasuredRuns);
        List<long> privateAfterBytes = new List<long>(MeasuredRuns);

        for (int runNumber = 1; runNumber <= MeasuredRuns; runNumber++)
        {
            CollectForMeasurement();
            using ProcessMemoryProbe memoryProbe = new ProcessMemoryProbe();
            long privateBefore = ReadPrivateBytes();
            long managedBefore = GC.GetTotalMemory(false);
            Stopwatch totalWatch = Stopwatch.StartNew();
            double conversionMilliseconds;
            double calculationMilliseconds;
            double contextCloneMilliseconds;
            double bitmapPublishMilliseconds;
            long privateAfterConversion;
            long privateAfterCalculation;
            long privateAfterContextClone;
            long privateAfterBitmapPublish;
            double meanValue = double.NaN;
            string resultSha256;
            int stepCount;

            using (Mat input = MeasureInputConversion(fixtureBitmap, out conversionMilliseconds))
            {
                privateAfterConversion = ReadPrivateBytes();
                Stopwatch calculationWatch = Stopwatch.StartNew();
                using (VisionRecipeRunResult result = await new VisionRecipeRunner().RunAsync(pipeline, input).ConfigureAwait(false))
                {
                    calculationWatch.Stop();
                    VerifyRun(result, twoLayer, size, runNumber, false);
                    calculationMilliseconds = calculationWatch.Elapsed.TotalMilliseconds;
                    privateAfterCalculation = ReadPrivateBytes();
                    stepCount = result.StepCount;
                    VisionRecipeStepRunSummary summary = result.FinalStepSummary;
                    Require(
                        summary.Metrics.TryGetValue("MeanValueAvg", out meanValue) && double.IsFinite(meanValue),
                        "final MeanValueAvg is missing or non-finite.");
                    resultSha256 = ComputeMatSha256(result.ResultImage);
                    Stopwatch contextCloneWatch = Stopwatch.StartNew();
                    using Mat contextClone = result.ResultImage.Clone();
                    contextCloneWatch.Stop();
                    contextCloneMilliseconds = contextCloneWatch.Elapsed.TotalMilliseconds;
                    privateAfterContextClone = ReadPrivateBytes();
                    Stopwatch bitmapPublishWatch = Stopwatch.StartNew();
                    using System.Drawing.Bitmap published = BitmapImageConverter.ToBitmap(contextClone);
                    bitmapPublishWatch.Stop();
                    bitmapPublishMilliseconds = bitmapPublishWatch.Elapsed.TotalMilliseconds;
                    privateAfterBitmapPublish = ReadPrivateBytes();
                }
            }

            totalWatch.Stop();
            long privatePeak = await memoryProbe.StopAsync().ConfigureAwait(false);
            CollectForMeasurement();
            long privateAfter = ReadPrivateBytes();
            long managedAfter = GC.GetTotalMemory(false);
            BaselineRow row = new BaselineRow
            {
                RepositoryCommit = gitCommit,
                SdkAssemblyPath = sdkAssemblyPath,
                SdkAssemblySha256 = sdkAssemblySha256,
                RecipeSha256 = recipeSha256,
                FixtureMatSha256 = fixtureMatSha256,
                Scenario = scenarioName,
                Width = size.Width,
                Height = size.Height,
                LayerCount = twoLayer ? 2 : 1,
                RunKind = "measured",
                RunNumber = runNumber,
                WarmupRuns = WarmupRuns,
                MeasuredRuns = MeasuredRuns,
                InputAllocationMilliseconds = inputAllocationWatch.Elapsed.TotalMilliseconds,
                FixtureBitmapConversionMilliseconds = fixtureBitmapWatch.Elapsed.TotalMilliseconds,
                InputConversionMilliseconds = conversionMilliseconds,
                CalculationMilliseconds = calculationMilliseconds,
                ContextCloneMilliseconds = contextCloneMilliseconds,
                BitmapPublishMilliseconds = bitmapPublishMilliseconds,
                TotalMilliseconds = totalWatch.Elapsed.TotalMilliseconds,
                MeanValueAvg = meanValue,
                ResultSha256 = resultSha256,
                StepCount = stepCount,
                InputBytes = inputBytes,
                ResultBytes = resultBytes,
                EstimatedLayerCacheBytes = layerCacheBytes,
                ObservedCopyVolumeBytes = checked(inputBytes + resultBytes + bitmapBytes),
                PrivateBeforeBytes = privateBefore,
                PrivatePeakBytes = privatePeak,
                PrivateAfterBytes = privateAfter,
                ManagedBeforeBytes = managedBefore,
                ManagedAfterBytes = managedAfter,
                PrivateAfterConversionBytes = privateAfterConversion,
                PrivateAfterCalculationBytes = privateAfterCalculation,
                PrivateAfterContextCloneBytes = privateAfterContextClone,
                PrivateAfterBitmapPublishBytes = privateAfterBitmapPublish,
                ProcessorCount = Environment.ProcessorCount,
                ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
                OsDescription = RuntimeInformation.OSDescription,
                TotalAvailableMemoryBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes
            };
            rows.Add(row);
            writer.WriteLine(row.ToCsvLine());
            writer.Flush();
            conversionTimes.Add(conversionMilliseconds);
            calculationTimes.Add(calculationMilliseconds);
            contextCloneTimes.Add(contextCloneMilliseconds);
            bitmapPublishTimes.Add(bitmapPublishMilliseconds);
            totalTimes.Add(totalWatch.Elapsed.TotalMilliseconds);
            privatePeakBytes.Add(privatePeak);
            privateAfterBytes.Add(privateAfter);
        }

        return new ScenarioAggregate(
            scenarioName,
            size.Width,
            size.Height,
            twoLayer ? 2 : 1,
            recipeSha256,
            fixtureMatSha256,
            inputBytes,
            resultBytes,
            layerCacheBytes,
            Percentile(conversionTimes, 0.50D),
            Percentile(conversionTimes, 0.95D),
            Percentile(calculationTimes, 0.50D),
            Percentile(calculationTimes, 0.95D),
            Percentile(contextCloneTimes, 0.50D),
            Percentile(contextCloneTimes, 0.95D),
            Percentile(bitmapPublishTimes, 0.50D),
            Percentile(bitmapPublishTimes, 0.95D),
            Percentile(totalTimes, 0.50D),
            Percentile(totalTimes, 0.95D),
            privatePeakBytes.Max(),
            privateAfterBytes.Max());
    }

    private static Mat MeasureInputConversion(System.Drawing.Bitmap fixtureBitmap, out double milliseconds)
    {
        Stopwatch watch = Stopwatch.StartNew();
        Mat result = BitmapImageConverter.ToMat(fixtureBitmap);
        watch.Stop();
        milliseconds = watch.Elapsed.TotalMilliseconds;
        return result;
    }

    private static void VerifyRun(
        VisionRecipeRunResult result,
        bool twoLayer,
        SizeSpec size,
        int runNumber,
        bool warmup)
    {
        if (result == null || !result.Success)
        {
            throw new InvalidOperationException(
                $"{(warmup ? "warm-up" : "measured")} run {runNumber} failed: {result?.SummaryText}");
        }

        Require(result.StepCount == (twoLayer ? 2 : 1), "unexpected step count.");
        Mat resultImage = result.ResultImage
            ?? throw new InvalidOperationException("run did not publish a result image.");
        Require(!resultImage.Empty(), "run did not publish a result image.");
        Require(result.Steps.All(step => step.Success && step.ToolSuccess), "one or more Mean steps failed.");
        Require(
            result.Steps.All(step => step.Metrics.TryGetValue("MeanValueAvg", out double value) && double.IsFinite(value)),
            "one or more MeanValueAvg metrics are missing or non-finite.");
        Require(resultImage.Width == size.Width && resultImage.Height == size.Height, "result image size changed.");
    }

    private static IEnumerable<SizeSpec> CreateSizeSpecs(bool includeTwentyThousand)
    {
        yield return new SizeSpec(1000, 1000, "1MP");
        yield return new SizeSpec(4512, 4512, "4512sq");
        yield return new SizeSpec(8192, 8192, "8192sq");
        if (includeTwentyThousand)
        {
            yield return new SizeSpec(20000, 20000, "20000sq");
        }
    }

    private static IEnumerable<string> CreatePreflightLines(bool includeTwentyThousand)
    {
        long available = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
        long estimate = checked(20000L * 20000L);
        bool safe = available <= 0 || estimate < available / 3L;
        return new[]
        {
            "DefaultSizes=1000x1000|4512x4512|8192x8192",
            "20000x20000=optional preflight only; it is not part of the default measurement.",
            "20000x20000EstimatedSingleChannelBytes=" + estimate.ToString(CultureInfo.InvariantCulture),
            "TotalAvailableMemoryBytes=" + available.ToString(CultureInfo.InvariantCulture),
            "20000x20000Preflight=" + (safe ? "eligible-if-explicitly-requested" : "skipped-low-memory-risk"),
            "ExplicitRequest=" + includeTwentyThousand.ToString(CultureInfo.InvariantCulture),
            "No low-memory fault injection or camera/hardware run was performed."
        };
    }

    private static string BuildMeanPipelineXml(int stepCount, string name)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?><VisionPipeline><Name>");
        builder.Append(name);
        builder.Append("</Name><Steps>");
        for (int index = 0; index < stepCount; index++)
        {
            string inputLayer = index == 0 ? "Main" : "Mean_Preview_" + index.ToString("00", CultureInfo.InvariantCulture);
            string outputLayer = "Mean_Preview_" + (index + 1).ToString("00", CultureInfo.InvariantCulture);
            builder.Append("<Step><Name>Mean ");
            builder.Append((index + 1).ToString(CultureInfo.InvariantCulture));
            builder.Append("</Name><ToolType>Mean</ToolType><Enabled>true</Enabled><InputLayer>");
            builder.Append(inputLayer);
            builder.Append("</InputLayer><OutputLayer>");
            builder.Append(outputLayer);
            builder.Append("</OutputLayer><Parameters>");
            builder.Append("<Parameter><Key>Name</Key><Value>");
            builder.Append(name);
            builder.Append("</Value></Parameter><Parameter><Key>MEAN_TYPES</Key><Value>Mean</Value></Parameter>");
            builder.Append("<Parameter><Key>USE_THRESHOLD</Key><Value>false</Value></Parameter>");
            builder.Append("<Parameter><Key>USE_ADAPTIVE_THRESHOLD</Key><Value>false</Value></Parameter>");
            builder.Append("<Parameter><Key>USE_BITWISENOT</Key><Value>false</Value></Parameter>");
            builder.Append("<Parameter><Key>USE_ROI</Key><Value>false</Value></Parameter>");
            builder.Append("<Parameter><Key>USE_MULTI_ROI</Key><Value>false</Value></Parameter>");
            builder.Append("</Parameters><UseAcceptance>true</UseAcceptance><ExpectedSuccess>true</ExpectedSuccess>");
            builder.Append("<AcceptanceMetricName>MeanValueAvg</AcceptanceMetricName><UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>");
            builder.Append("<AcceptanceMetricMinimum>185</AcceptanceMetricMinimum><UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>");
            builder.Append("<AcceptanceMetricMaximum>220</AcceptanceMetricMaximum><MaxElapsedMilliseconds>60000</MaxElapsedMilliseconds></Step>");
        }

        builder.Append("</Steps></VisionPipeline>");
        return builder.ToString();
    }

    private static void CollectForMeasurement()
    {
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
        GC.WaitForPendingFinalizers();
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
    }

    private static long ReadPrivateBytes()
    {
        using Process process = Process.GetCurrentProcess();
        process.Refresh();
        return process.PrivateMemorySize64;
    }

    private static long EstimateMatBytes(Mat mat)
    {
        return checked((long)mat.Width * mat.Height * mat.ElemSize());
    }

    private static long EstimateBitmapBytes(System.Drawing.Bitmap bitmap)
    {
        return checked((long)bitmap.Width * bitmap.Height * Math.Max(1, Image.GetPixelFormatSize(bitmap.PixelFormat) / 8));
    }

    private static double Percentile(IReadOnlyList<double> values, double percentile)
    {
        double[] sorted = values.OrderBy(value => value).ToArray();
        if (sorted.Length == 0)
        {
            return 0D;
        }

        int index = Math.Max(0, (int)Math.Ceiling(sorted.Length * percentile) - 1);
        return sorted[Math.Min(index, sorted.Length - 1)];
    }

    private static string ComputeSha256(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }

    private static string ComputeSha256(byte[] bytes)
    {
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    private static string ComputeMatSha256(Mat mat)
    {
        byte[] header = Encoding.UTF8.GetBytes($"{mat.Rows}|{mat.Cols}|{mat.Type()}|{mat.Step()}|");
        using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        hash.AppendData(header);
        hash.AppendData(mat.ToBytes());
        return Convert.ToHexString(hash.GetHashAndReset());
    }

    private static string ResolveGitCommit(string repositoryRoot)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "rev-parse HEAD",
                WorkingDirectory = repositoryRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process? process = Process.Start(startInfo);
            if (process == null)
            {
                return "unavailable";
            }

            using (process)
            {
                string output = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit(5000);
                return output;
            }
        }
        catch
        {
            return "unavailable";
        }
    }

    private static string FindRepositoryRoot()
    {
        foreach (string seed in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            DirectoryInfo? directory = new DirectoryInfo(Path.GetFullPath(seed));
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "OpenVisionLab.sln"))
                    && Directory.Exists(Path.Combine(directory.FullName, "docs", "samples")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }
        }

        throw new DirectoryNotFoundException("OpenVisionLab repository root was not found.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed class ProcessMemoryProbe : IDisposable
    {
        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
        private readonly Task samplingTask;
        private long peakPrivateBytes;

        public ProcessMemoryProbe()
        {
            peakPrivateBytes = ReadPrivateBytes();
            samplingTask = Task.Run(SampleAsync);
        }

        public async Task<long> StopAsync()
        {
            cancellation.Cancel();
            try
            {
                await samplingTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }

            long current = ReadPrivateBytes();
            UpdatePeak(current);
            return Interlocked.Read(ref peakPrivateBytes);
        }

        public void Dispose()
        {
            cancellation.Cancel();
            cancellation.Dispose();
        }

        private async Task SampleAsync()
        {
            try
            {
                while (!cancellation.IsCancellationRequested)
                {
                    UpdatePeak(ReadPrivateBytes());
                    await Task.Delay(1, cancellation.Token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void UpdatePeak(long value)
        {
            long current;
            do
            {
                current = Interlocked.Read(ref peakPrivateBytes);
                if (value <= current)
                {
                    return;
                }
            }
            while (Interlocked.CompareExchange(ref peakPrivateBytes, value, current) != current);
        }
    }

    private sealed class SizeSpec
    {
        public SizeSpec(int width, int height, string label)
        {
            Width = width;
            Height = height;
            Label = label;
        }

        public int Width { get; }
        public int Height { get; }
        public string Label { get; }
    }

    private sealed class BaselineRow
    {
        public const string Header = "RepositoryCommit,SdkAssemblyPath,SdkAssemblySha256,RecipeSha256,FixtureMatSha256,Scenario,Width,Height,LayerCount,RunKind,RunNumber,WarmupRuns,MeasuredRuns,InputAllocationMs,FixtureBitmapConversionMs,InputConversionMs,CalculationMs,ContextCloneMs,BitmapPublishMs,TotalMs,MeanValueAvg,ResultSha256,StepCount,InputBytes,ResultBytes,EstimatedLayerCacheBytes,ObservedCopyVolumeBytes,PrivateBeforeBytes,PrivatePeakBytes,PrivateAfterBytes,ManagedBeforeBytes,ManagedAfterBytes,PrivateAfterConversionBytes,PrivateAfterCalculationBytes,PrivateAfterContextCloneBytes,PrivateAfterBitmapPublishBytes,ProcessorCount,ProcessArchitecture,OsDescription,TotalAvailableMemoryBytes";

        public string RepositoryCommit { get; set; } = string.Empty;
        public string SdkAssemblyPath { get; set; } = string.Empty;
        public string SdkAssemblySha256 { get; set; } = string.Empty;
        public string RecipeSha256 { get; set; } = string.Empty;
        public string FixtureMatSha256 { get; set; } = string.Empty;
        public string Scenario { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public int LayerCount { get; set; }
        public string RunKind { get; set; } = string.Empty;
        public int RunNumber { get; set; }
        public int WarmupRuns { get; set; }
        public int MeasuredRuns { get; set; }
        public double InputAllocationMilliseconds { get; set; }
        public double FixtureBitmapConversionMilliseconds { get; set; }
        public double InputConversionMilliseconds { get; set; }
        public double CalculationMilliseconds { get; set; }
        public double ContextCloneMilliseconds { get; set; }
        public double BitmapPublishMilliseconds { get; set; }
        public double TotalMilliseconds { get; set; }
        public double MeanValueAvg { get; set; }
        public string ResultSha256 { get; set; } = string.Empty;
        public int StepCount { get; set; }
        public long InputBytes { get; set; }
        public long ResultBytes { get; set; }
        public long EstimatedLayerCacheBytes { get; set; }
        public long ObservedCopyVolumeBytes { get; set; }
        public long PrivateBeforeBytes { get; set; }
        public long PrivatePeakBytes { get; set; }
        public long PrivateAfterBytes { get; set; }
        public long ManagedBeforeBytes { get; set; }
        public long ManagedAfterBytes { get; set; }
        public long PrivateAfterConversionBytes { get; set; }
        public long PrivateAfterCalculationBytes { get; set; }
        public long PrivateAfterContextCloneBytes { get; set; }
        public long PrivateAfterBitmapPublishBytes { get; set; }
        public int ProcessorCount { get; set; }
        public string ProcessArchitecture { get; set; } = string.Empty;
        public string OsDescription { get; set; } = string.Empty;
        public long TotalAvailableMemoryBytes { get; set; }

        public string ToCsvLine()
        {
            return string.Join(",", new[]
            {
                Csv(RepositoryCommit), Csv(SdkAssemblyPath), Csv(SdkAssemblySha256), Csv(RecipeSha256), Csv(FixtureMatSha256), Csv(Scenario),
                Width.ToString(CultureInfo.InvariantCulture), Height.ToString(CultureInfo.InvariantCulture), LayerCount.ToString(CultureInfo.InvariantCulture), Csv(RunKind),
                RunNumber.ToString(CultureInfo.InvariantCulture), WarmupRuns.ToString(CultureInfo.InvariantCulture), MeasuredRuns.ToString(CultureInfo.InvariantCulture),
                InputAllocationMilliseconds.ToString("0.###", CultureInfo.InvariantCulture), FixtureBitmapConversionMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                InputConversionMilliseconds.ToString("0.###", CultureInfo.InvariantCulture), CalculationMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                ContextCloneMilliseconds.ToString("0.###", CultureInfo.InvariantCulture), BitmapPublishMilliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture), MeanValueAvg.ToString("0.############", CultureInfo.InvariantCulture), Csv(ResultSha256),
                StepCount.ToString(CultureInfo.InvariantCulture), InputBytes.ToString(CultureInfo.InvariantCulture), ResultBytes.ToString(CultureInfo.InvariantCulture),
                EstimatedLayerCacheBytes.ToString(CultureInfo.InvariantCulture), ObservedCopyVolumeBytes.ToString(CultureInfo.InvariantCulture),
                PrivateBeforeBytes.ToString(CultureInfo.InvariantCulture), PrivatePeakBytes.ToString(CultureInfo.InvariantCulture), PrivateAfterBytes.ToString(CultureInfo.InvariantCulture),
                ManagedBeforeBytes.ToString(CultureInfo.InvariantCulture), ManagedAfterBytes.ToString(CultureInfo.InvariantCulture),
                PrivateAfterConversionBytes.ToString(CultureInfo.InvariantCulture), PrivateAfterCalculationBytes.ToString(CultureInfo.InvariantCulture),
                PrivateAfterContextCloneBytes.ToString(CultureInfo.InvariantCulture), PrivateAfterBitmapPublishBytes.ToString(CultureInfo.InvariantCulture),
                ProcessorCount.ToString(CultureInfo.InvariantCulture), Csv(ProcessArchitecture), Csv(OsDescription), TotalAvailableMemoryBytes.ToString(CultureInfo.InvariantCulture)
            });
        }

        private static string Csv(string value)
        {
            string text = value ?? string.Empty;
            return "\"" + text.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
        }
    }

    private sealed class ScenarioAggregate
    {
        public const string Header = "Scenario,Width,Height,LayerCount,RecipeSha256,FixtureMatSha256,InputBytes,ResultBytes,EstimatedLayerCacheBytes,InputConversionP50Ms,InputConversionP95Ms,CalculationP50Ms,CalculationP95Ms,ContextCloneP50Ms,ContextCloneP95Ms,BitmapPublishP50Ms,BitmapPublishP95Ms,TotalP50Ms,TotalP95Ms,PrivatePeakMaxBytes,PrivateAfterMaxBytes";

        public ScenarioAggregate(
            string scenario,
            int width,
            int height,
            int layerCount,
            string recipeSha256,
            string fixtureMatSha256,
            long inputBytes,
            long resultBytes,
            long estimatedLayerCacheBytes,
            double inputConversionP50Milliseconds,
            double inputConversionP95Milliseconds,
            double calculationP50Milliseconds,
            double calculationP95Milliseconds,
            double contextCloneP50Milliseconds,
            double contextCloneP95Milliseconds,
            double bitmapPublishP50Milliseconds,
            double bitmapPublishP95Milliseconds,
            double totalP50Milliseconds,
            double totalP95Milliseconds,
            long privatePeakMaxBytes,
            long privateAfterMaxBytes)
        {
            Scenario = scenario;
            Width = width;
            Height = height;
            LayerCount = layerCount;
            RecipeSha256 = recipeSha256;
            FixtureMatSha256 = fixtureMatSha256;
            InputBytes = inputBytes;
            ResultBytes = resultBytes;
            EstimatedLayerCacheBytes = estimatedLayerCacheBytes;
            InputConversionP50Milliseconds = inputConversionP50Milliseconds;
            InputConversionP95Milliseconds = inputConversionP95Milliseconds;
            CalculationP50Milliseconds = calculationP50Milliseconds;
            CalculationP95Milliseconds = calculationP95Milliseconds;
            ContextCloneP50Milliseconds = contextCloneP50Milliseconds;
            ContextCloneP95Milliseconds = contextCloneP95Milliseconds;
            BitmapPublishP50Milliseconds = bitmapPublishP50Milliseconds;
            BitmapPublishP95Milliseconds = bitmapPublishP95Milliseconds;
            TotalP50Milliseconds = totalP50Milliseconds;
            TotalP95Milliseconds = totalP95Milliseconds;
            PrivatePeakMaxBytes = privatePeakMaxBytes;
            PrivateAfterMaxBytes = privateAfterMaxBytes;
        }

        public string Scenario { get; }
        public int Width { get; }
        public int Height { get; }
        public int LayerCount { get; }
        public string RecipeSha256 { get; }
        public string FixtureMatSha256 { get; }
        public long InputBytes { get; }
        public long ResultBytes { get; }
        public long EstimatedLayerCacheBytes { get; }
        public double InputConversionP50Milliseconds { get; }
        public double InputConversionP95Milliseconds { get; }
        public double CalculationP50Milliseconds { get; }
        public double CalculationP95Milliseconds { get; }
        public double ContextCloneP50Milliseconds { get; }
        public double ContextCloneP95Milliseconds { get; }
        public double BitmapPublishP50Milliseconds { get; }
        public double BitmapPublishP95Milliseconds { get; }
        public double TotalP50Milliseconds { get; }
        public double TotalP95Milliseconds { get; }
        public long PrivatePeakMaxBytes { get; }
        public long PrivateAfterMaxBytes { get; }

        public string ToCsvLine()
        {
            return string.Join(",", new[]
            {
                Csv(Scenario), Width.ToString(CultureInfo.InvariantCulture), Height.ToString(CultureInfo.InvariantCulture), LayerCount.ToString(CultureInfo.InvariantCulture),
                Csv(RecipeSha256), Csv(FixtureMatSha256), InputBytes.ToString(CultureInfo.InvariantCulture), ResultBytes.ToString(CultureInfo.InvariantCulture),
                EstimatedLayerCacheBytes.ToString(CultureInfo.InvariantCulture), InputConversionP50Milliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                InputConversionP95Milliseconds.ToString("0.###", CultureInfo.InvariantCulture), CalculationP50Milliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                CalculationP95Milliseconds.ToString("0.###", CultureInfo.InvariantCulture), ContextCloneP50Milliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                ContextCloneP95Milliseconds.ToString("0.###", CultureInfo.InvariantCulture), BitmapPublishP50Milliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                BitmapPublishP95Milliseconds.ToString("0.###", CultureInfo.InvariantCulture), TotalP50Milliseconds.ToString("0.###", CultureInfo.InvariantCulture),
                TotalP95Milliseconds.ToString("0.###", CultureInfo.InvariantCulture), PrivatePeakMaxBytes.ToString(CultureInfo.InvariantCulture),
                PrivateAfterMaxBytes.ToString(CultureInfo.InvariantCulture)
            });
        }

        private static string Csv(string value)
        {
            return "\"" + (value ?? string.Empty).Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
        }
    }
}
