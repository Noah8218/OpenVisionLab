#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OpenVisionLab;
using OpenVisionLab.Core.Integration;
using OpenVisionLab.Integration.Contracts;

internal static class TwoDIntegrationInputHashDecodeContract
{
    private const string ProducerCommit = "2222222222222222222222222222222222222222";
    private const int ImageWidth = 32;
    private const int ImageHeight = 32;
    private const byte SourceAValue = 32;
    private const byte SourceBValue = 224;

    public static async Task<int> RunAsync(
        string evidenceRoot,
        string runtimeBuildManifestPath)
    {
        string root = Path.GetFullPath(evidenceRoot);
        Directory.CreateDirectory(root);
        string runRoot = Path.Combine(
            root,
            $"two-d-input-hash-decode-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(runRoot);

        _ = IntegrationContractJson.DeserializeRuntimeBuildManifest(
            File.ReadAllBytes(runtimeBuildManifestPath));
        IntegrationApplicationIdentity consumer =
            TwoDIntegrationBuildIdentity.LoadQualifiedIdentity(runtimeBuildManifestPath);

        string sourceAPath = Path.Combine(runRoot, "source-a.bmp");
        string sourceBPath = Path.Combine(runRoot, "source-b.bmp");
        string recipePath = Path.Combine(runRoot, "mean-probe.pipeline.xml");
        WriteSolidBmp(sourceAPath, SourceAValue);
        WriteSolidBmp(sourceBPath, SourceBValue);
        File.WriteAllText(recipePath, CreateMeanRecipeXml(), new UTF8Encoding(false));
        Require(
            new FileInfo(sourceAPath).Length == new FileInfo(sourceBPath).Length,
            "The A/B replacement fixture must keep the encoded byte length equal.");

        var observations = new List<CaseObservation>
        {
            await RunSafeAsync(
                "immutable",
                () => RunImmutableCaseAsync(
                    runRoot,
                    sourceAPath,
                    recipePath,
                    consumer,
                    runtimeBuildManifestPath)),
            await RunSafeAsync(
                "atomic-same-length-replacement",
                () => RunReplacementCaseAsync(
                    runRoot,
                    "atomic-same-length-replacement",
                    sourceAPath,
                    sourceBPath,
                    recipePath,
                    consumer,
                    runtimeBuildManifestPath,
                    IntegrationErrorCode.ArtifactHashMismatch,
                    SourceBValue)),
            await RunSafeAsync(
                "partial-truncated-replacement",
                () => RunPartialReplacementCaseAsync(
                    runRoot,
                    sourceAPath,
                    recipePath,
                    consumer,
                    runtimeBuildManifestPath))
        };

        string reportPath = Path.Combine(runRoot, "two-d-input-hash-decode-contract.json");
        File.WriteAllText(
            reportPath,
            JsonSerializer.Serialize(
                new
                {
                    schemaVersion = "1.0",
                    owner = "TwoDIntegrationExchange.RunAcceptedHandoffAsync",
                    sourceArtifactValidation = "ReadHandoff -> ValidateArtifactFile",
                    sourceDecode = "verified source bytes -> Cv2.ImDecode",
                    cases = observations,
                    baselineMismatchObserved = observations.Any(caseResult =>
                        caseResult.Name == "atomic-same-length-replacement"
                        && caseResult.ResultPublished
                        && caseResult.SourceIdentityMatches == true
                        && caseResult.ObservedMean.HasValue
                        && Math.Abs(caseResult.ObservedMean.Value - SourceBValue) < 0.5D),
                    allPassed = observations.All(caseResult => caseResult.Passed),
                    noNewProtocolOrOwner = true
                },
                new JsonSerializerOptions { WriteIndented = true }));

        foreach (CaseObservation observation in observations)
        {
            Console.WriteLine(
                $"2D input hash/decode {observation.Name}: "
                + (observation.Passed ? "PASS" : "FAIL")
                + $" ({observation.Actual})");
        }

        Console.WriteLine(
            $"2D input hash/decode contract "
            + (observations.All(caseResult => caseResult.Passed) ? "passed" : "failed")
            + $". Evidence={reportPath}");
        return observations.All(caseResult => caseResult.Passed) ? 0 : 1;
    }

    private static async Task<CaseObservation> RunImmutableCaseAsync(
        string runRoot,
        string sourcePath,
        string recipePath,
        IntegrationApplicationIdentity consumer,
        string runtimeBuildManifestPath)
    {
        Fixture fixture = CreateFixture(
            runRoot,
            "immutable",
            sourcePath,
            recipePath,
            consumer);
        _ = TwoDIntegrationExchange.AcknowledgeHandoff(
            runRoot,
            fixture.Handoff.TransactionId,
            runtimeBuildManifestPath);
        IntegrationResultV2 result = await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
            runRoot,
            fixture.Handoff.TransactionId,
            runtimeBuildManifestPath).ConfigureAwait(false);
        TwoDIntegrationRunRecord record = ReadRunRecord(fixture);
        double? observedMean = FindMetric(record, "MeanValueAvg");
        bool identityMatches = string.Equals(
            record.SourceSha256,
            fixture.SourceSha256,
            StringComparison.OrdinalIgnoreCase);
        bool passed = result.Status == IntegrationResultStatus.Completed
            && result.Outcome == IntegrationInspectionOutcome.Pass
            && identityMatches
            && observedMean.HasValue
            && Math.Abs(observedMean.Value - SourceAValue) < 0.5D;
        return new CaseObservation
        {
            Name = "immutable",
            Expected = "Completed/Pass with source A hash and MeanValueAvg=32",
            Actual = $"{result.Status}/{result.Outcome}; MeanValueAvg={Format(observedMean)}",
            Passed = passed,
            ResultPublished = File.Exists(GetMessagePath(
                fixture,
                IntegrationTransactionLayout.ResultFileName)),
            RunRecordPublished = File.Exists(fixture.RunRecordPath),
            ErrorCode = result.Error?.Code.ToString(),
            SourceIdentityMatches = identityMatches,
            RunRecordSourceSha256 = record.SourceSha256,
            ExpectedSourceSha256 = fixture.SourceSha256,
            ObservedMean = observedMean,
            ExpectedMean = SourceAValue
        };
    }

    private static async Task<CaseObservation> RunReplacementCaseAsync(
        string runRoot,
        string caseName,
        string sourceAPath,
        string replacementPath,
        string recipePath,
        IntegrationApplicationIdentity consumer,
        string runtimeBuildManifestPath,
        IntegrationErrorCode expectedErrorCode,
        byte expectedReplacementMean)
    {
        Fixture fixture = CreateFixture(
            runRoot,
            caseName,
            sourceAPath,
            recipePath,
            consumer);
        _ = TwoDIntegrationExchange.AcknowledgeHandoff(
            runRoot,
            fixture.Handoff.TransactionId,
            runtimeBuildManifestPath);

        string markerPath = Path.Combine(
            fixture.TransactionDirectory,
            $"{caseName}.source-validation.marker");
        IntegrationResultV2? result = null;
        Exception? failure = null;
        using (IDisposable pause = TwoDIntegrationExchange.BeginSourceValidationPauseForTest(markerPath))
        {
            Task<IntegrationResultV2> runTask = Task.Run(
                () => TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                    runRoot,
                    fixture.Handoff.TransactionId,
                    runtimeBuildManifestPath));
            if (!await WaitForMarkerAsync(markerPath).ConfigureAwait(false))
            {
                TryDelete(markerPath);
                try
                {
                    result = await runTask.ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    failure = exception;
                }

                return CreateFailureObservation(
                    caseName,
                    $"pause marker {markerPath} must be reached",
                    result,
                    failure,
                    fixture,
                    expectedReplacementMean);
            }

            AtomicReplace(replacementPath, fixture.SourcePath);
            TryDelete(markerPath);
            try
            {
                result = await runTask.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        }

        bool resultPublished = File.Exists(GetMessagePath(
            fixture,
            IntegrationTransactionLayout.ResultFileName));
        bool runRecordPublished = File.Exists(fixture.RunRecordPath);
        TwoDIntegrationRunRecord? record = runRecordPublished
            ? ReadRunRecord(fixture)
            : null;
        double? observedMean = record is null
            ? null
            : FindMetric(record, "MeanValueAvg");
        bool sourceIdentityMatches = record is not null
            && string.Equals(
                record.SourceSha256,
                fixture.SourceSha256,
                StringComparison.OrdinalIgnoreCase);
        string? errorCode = ResolveErrorCode(failure);
        bool passed = failure is IntegrationContractException contractException
            && contractException.ErrorCode == expectedErrorCode
            && !resultPublished
            && !runRecordPublished;
        return new CaseObservation
        {
            Name = caseName,
            Expected = $"{expectedErrorCode} before decode; no Result or RunRecord",
            Actual = failure is not null
                ? $"exception {errorCode}: {failure.Message}"
                : result is not null
                    ? $"{result.Status}/{result.Outcome}; Result={resultPublished}; MeanValueAvg={Format(observedMean)}"
                    : "no result or exception",
            Passed = passed,
            ResultPublished = resultPublished,
            RunRecordPublished = runRecordPublished,
            ErrorCode = errorCode ?? result?.Error?.Code.ToString(),
            SourceIdentityMatches = record is null ? null : sourceIdentityMatches,
            RunRecordSourceSha256 = record?.SourceSha256,
            ExpectedSourceSha256 = fixture.SourceSha256,
            ObservedMean = observedMean,
            ExpectedMean = expectedReplacementMean,
            Message = failure?.Message
        };
    }

    private static async Task<CaseObservation> RunPartialReplacementCaseAsync(
        string runRoot,
        string sourceAPath,
        string recipePath,
        IntegrationApplicationIdentity consumer,
        string runtimeBuildManifestPath)
    {
        string partialPath = Path.Combine(
            runRoot,
            "source-partial.bmp");
        byte[] sourceBytes = File.ReadAllBytes(sourceAPath);
        File.WriteAllBytes(partialPath, sourceBytes.Take(sourceBytes.Length / 2).ToArray());
        return await RunReplacementCaseAsync(
            runRoot,
            "partial-truncated-replacement",
            sourceAPath,
            partialPath,
            recipePath,
            consumer,
            runtimeBuildManifestPath,
            IntegrationErrorCode.ArtifactLengthMismatch,
            SourceAValue).ConfigureAwait(false);
    }

    private static async Task<CaseObservation> RunSafeAsync(
        string caseName,
        Func<Task<CaseObservation>> action)
    {
        try
        {
            return await action().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            return new CaseObservation
            {
                Name = caseName,
                Expected = "focused contract case completes without an unexpected exception",
                Actual = $"unexpected {exception.GetType().Name}: {exception.Message}",
                Passed = false,
                ErrorCode = ResolveErrorCode(exception),
                Message = exception.ToString()
            };
        }
    }

    private static CaseObservation CreateFailureObservation(
        string caseName,
        string expected,
        IntegrationResultV2? result,
        Exception? failure,
        Fixture fixture,
        byte expectedMean) =>
        new()
        {
            Name = caseName,
            Expected = expected,
            Actual = failure is not null
                ? $"exception {ResolveErrorCode(failure)}: {failure.Message}"
                : result is not null
                    ? $"{result.Status}/{result.Outcome}"
                    : "no result or exception",
            Passed = false,
            ResultPublished = File.Exists(GetMessagePath(
                fixture,
                IntegrationTransactionLayout.ResultFileName)),
            RunRecordPublished = File.Exists(fixture.RunRecordPath),
            ErrorCode = ResolveErrorCode(failure) ?? result?.Error?.Code.ToString(),
            ExpectedSourceSha256 = fixture.SourceSha256,
            ExpectedMean = expectedMean,
            Message = failure?.Message
        };

    private static Fixture CreateFixture(
        string runRoot,
        string caseName,
        string sourcePath,
        string recipePath,
        IntegrationApplicationIdentity consumer)
    {
        Guid transactionId = Guid.NewGuid();
        string transactionDirectory = Path.Combine(
            runRoot,
            IntegrationTransactionLayout.TransactionsDirectoryName,
            transactionId.ToString("D"));
        Directory.CreateDirectory(transactionDirectory);
        IntegrationArtifactReference machineProject = WriteArtifact(
            transactionDirectory,
            IntegrationArtifactRoles.MachineProject,
            $"machine-{caseName}",
            "{\"schema\":\"machine-project/1.0\",\"fixture\":true}",
            "artifacts/machine-project.json");
        IntegrationArtifactReference source = CopyArtifact(
            transactionDirectory,
            IntegrationArtifactRoles.InspectionSource,
            $"source-{caseName}",
            sourcePath,
            "artifacts/source.bmp");
        IntegrationArtifactReference recipe = CopyArtifact(
            transactionDirectory,
            IntegrationArtifactRoles.InspectionRecipe,
            $"recipe-{caseName}",
            recipePath,
            "artifacts/recipe.xml");
        var context = new IntegrationInspectionContextV2(
            "machine-fixture",
            "machine-project/1.0",
            "sequence-045",
            "inspect-image",
            "camera-virtual",
            $"acquisition-{caseName}",
            $"frame-{caseName}",
            "px",
            IntegrationInspectionModality.TwoD,
            IntegrationInspectionInputKind.Image,
            source.Sha256,
            recipe.Sha256,
            consumer,
            [machineProject, source, recipe]);
        var handoff = new IntegrationHandoffV2(
            IntegrationContractSchema.V2,
            IntegrationMessageKind.Handoff,
            Guid.NewGuid(),
            transactionId,
            DateTimeOffset.UtcNow,
            new IntegrationApplicationIdentity(
                IntegrationApplicationIds.MachineStudio,
                "2.1.0",
                ProducerCommit,
                IntegrationSourceState.Clean),
            context);
        File.WriteAllBytes(
            GetMessagePath(
                transactionDirectory,
                IntegrationTransactionLayout.HandoffFileName),
            IntegrationContractJson.SerializeCanonical(handoff));
        return new Fixture(
            handoff,
            transactionDirectory,
            Path.Combine(
                transactionDirectory,
                source.RelativePath.Replace('/', Path.DirectorySeparatorChar)),
            source.Sha256,
            source.ByteLength);
    }

    private static IntegrationArtifactReference CopyArtifact(
        string transactionDirectory,
        string role,
        string artifactId,
        string sourcePath,
        string relativePath)
    {
        string targetPath = Path.Combine(
            transactionDirectory,
            relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        File.Copy(sourcePath, targetPath, overwrite: false);
        return CreateArtifactReference(role, artifactId, targetPath, relativePath);
    }

    private static IntegrationArtifactReference WriteArtifact(
        string transactionDirectory,
        string role,
        string artifactId,
        string content,
        string relativePath)
    {
        string targetPath = Path.Combine(
            transactionDirectory,
            relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        File.WriteAllText(targetPath, content, new UTF8Encoding(false));
        return CreateArtifactReference(role, artifactId, targetPath, relativePath);
    }

    private static IntegrationArtifactReference CreateArtifactReference(
        string role,
        string artifactId,
        string fullPath,
        string relativePath)
    {
        using FileStream stream = File.OpenRead(fullPath);
        return new(
            role,
            artifactId,
            relativePath,
            stream.Length,
            Convert.ToHexString(SHA256.HashData(stream)));
    }

    private static TwoDIntegrationRunRecord ReadRunRecord(Fixture fixture) =>
        JsonSerializer.Deserialize<TwoDIntegrationRunRecord>(
            File.ReadAllText(fixture.RunRecordPath),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidOperationException("The 2D RunRecord could not be deserialized.");

    private static double? FindMetric(
        TwoDIntegrationRunRecord record,
        string metricName)
    {
        foreach (TwoDIntegrationStepRecord step in record.Steps ?? [])
        {
            foreach (KeyValuePair<string, double> metric in step.Metrics ?? new Dictionary<string, double>())
            {
                if (string.Equals(metric.Key, metricName, StringComparison.OrdinalIgnoreCase))
                {
                    return metric.Value;
                }
            }
        }

        return null;
    }

    private static string? ResolveErrorCode(Exception? exception) =>
        exception is IntegrationContractException contractException
            ? contractException.ErrorCode.ToString()
            : exception?.GetType().Name;

    private static async Task<bool> WaitForMarkerAsync(string markerPath)
    {
        DateTime deadline = DateTime.UtcNow.AddSeconds(15);
        while (DateTime.UtcNow < deadline)
        {
            if (File.Exists(markerPath))
            {
                return true;
            }

            await Task.Delay(5).ConfigureAwait(false);
        }

        return File.Exists(markerPath);
    }

    private static void AtomicReplace(string replacementPath, string targetPath)
    {
        string temporaryPath = targetPath + ".replacement-" + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.Copy(replacementPath, temporaryPath, overwrite: false);
            // The temporary file is on the same volume; Windows rename-with-overwrite
            // keeps the target transition atomic for this local replacement fixture.
            File.Move(temporaryPath, targetPath, overwrite: true);
        }
        finally
        {
            TryDelete(temporaryPath);
        }
    }

    private static string GetMessagePath(Fixture fixture, string fileName) =>
        GetMessagePath(fixture.TransactionDirectory, fileName);

    private static string GetMessagePath(string transactionDirectory, string fileName) =>
        Path.Combine(transactionDirectory, fileName);

    private static string Format(double? value) =>
        value.HasValue ? value.Value.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture) : "-";

    private static void WriteSolidBmp(string path, byte value)
    {
        const int fileHeaderSize = 14;
        const int dibHeaderSize = 40;
        int rowBytes = ImageWidth * 3;
        int pixelOffset = fileHeaderSize + dibHeaderSize;
        int fileSize = pixelOffset + rowBytes * ImageHeight;
        byte[] bytes = new byte[fileSize];
        bytes[0] = (byte)'B';
        bytes[1] = (byte)'M';
        WriteInt32(bytes, 2, fileSize);
        WriteInt32(bytes, 10, pixelOffset);
        WriteInt32(bytes, 14, dibHeaderSize);
        WriteInt32(bytes, 18, ImageWidth);
        WriteInt32(bytes, 22, ImageHeight);
        WriteInt16(bytes, 26, 1);
        WriteInt16(bytes, 28, 24);
        WriteInt32(bytes, 34, rowBytes * ImageHeight);
        for (int index = pixelOffset; index < bytes.Length; index += 3)
        {
            bytes[index] = value;
            bytes[index + 1] = value;
            bytes[index + 2] = value;
        }

        File.WriteAllBytes(path, bytes);
    }

    private static string CreateMeanRecipeXml() =>
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n"
        + "<VisionPipeline>\n"
        + "  <Name>2D-045 Mean Probe</Name>\n"
        + "  <Steps>\n"
        + "    <Step>\n"
        + "      <Name>Mean Probe</Name>\n"
        + "      <ToolType>Mean</ToolType>\n"
        + "      <Enabled>true</Enabled>\n"
        + "      <InputLayer>Main</InputLayer>\n"
        + "      <OutputLayer>Mean_Output</OutputLayer>\n"
        + "      <Parameters>\n"
        + "        <Parameter><Key>Name</Key><Value>2D-045 Mean Probe</Value></Parameter>\n"
        + "        <Parameter><Key>MEAN_TYPES</Key><Value>Mean</Value></Parameter>\n"
        + "        <Parameter><Key>MEAN_MIN</Key><Value>0</Value></Parameter>\n"
        + "        <Parameter><Key>MEAN_MAX</Key><Value>255</Value></Parameter>\n"
        + "        <Parameter><Key>USE_THRESHOLD</Key><Value>false</Value></Parameter>\n"
        + "        <Parameter><Key>USE_ADAPTIVE_THRESHOLD</Key><Value>false</Value></Parameter>\n"
        + "        <Parameter><Key>USE_BITWISENOT</Key><Value>false</Value></Parameter>\n"
        + "        <Parameter><Key>USE_ROI</Key><Value>false</Value></Parameter>\n"
        + "        <Parameter><Key>USE_MULTI_ROI</Key><Value>false</Value></Parameter>\n"
        + "      </Parameters>\n"
        + "    </Step>\n"
        + "  </Steps>\n"
        + "</VisionPipeline>\n";

    private static void WriteInt16(byte[] bytes, int offset, short value)
    {
        bytes[offset] = (byte)value;
        bytes[offset + 1] = (byte)(value >> 8);
    }

    private static void WriteInt32(byte[] bytes, int offset, int value)
    {
        bytes[offset] = (byte)value;
        bytes[offset + 1] = (byte)(value >> 8);
        bytes[offset + 2] = (byte)(value >> 16);
        bytes[offset + 3] = (byte)(value >> 24);
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed record Fixture(
        IntegrationHandoffV2 Handoff,
        string TransactionDirectory,
        string SourcePath,
        string SourceSha256,
        long SourceByteLength)
    {
        public string RunRecordPath => Path.Combine(
            TransactionDirectory,
            IntegrationTransactionLayout.ArtifactsDirectoryName,
            "2d-run-record.json");
    }

    private sealed class CaseObservation
    {
        public string Name { get; init; } = string.Empty;
        public string Expected { get; init; } = string.Empty;
        public string Actual { get; init; } = string.Empty;
        public bool Passed { get; init; }
        public bool ResultPublished { get; init; }
        public bool RunRecordPublished { get; init; }
        public string? ErrorCode { get; init; }
        public bool? SourceIdentityMatches { get; init; }
        public string? RunRecordSourceSha256 { get; init; }
        public string? ExpectedSourceSha256 { get; init; }
        public double? ObservedMean { get; init; }
        public double? ExpectedMean { get; init; }
        public string? Message { get; init; }
    }
}
