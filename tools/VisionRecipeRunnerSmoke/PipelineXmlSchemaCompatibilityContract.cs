using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

internal static class PipelineXmlSchemaCompatibilityContract
{
    public static int Run(string evidenceDirectory)
    {
        string outputDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(outputDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string? previousDataRoot = Environment.GetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable);

        try
        {
            byte[] legacyBytes = Encoding.UTF8.GetBytes(CreateLegacyXml());
            WriteFixture(outputDirectory, "legacy-unversioned.xml", legacyBytes);
            CheckSupportedLoad(legacyBytes, "legacy unversioned schema", passed, failed);

            byte[] v1Bytes = Mutate(legacyBytes, root => root.SetAttributeValue("SchemaVersion", "1"));
            WriteFixture(outputDirectory, "explicit-schema-v1.xml", v1Bytes);
            CheckSupportedLoad(v1Bytes, "explicit schema v1", passed, failed);

            byte[] extensionBytes = Mutate(legacyBytes, root =>
            {
                root.AddFirst(new XComment("comments are non-semantic"));
                root.Add(new XElement("Extensions", new XElement("FutureOptional", "ignored")));
                XNamespace extension = VisionPipelineXmlSchemaPolicy.OptionalExtensionNamespace;
                root.Add(new XElement(extension + "FutureNamespacedOptional", "ignored"));
            });
            WriteFixture(outputDirectory, "optional-extension-and-comment.xml", extensionBytes);
            CheckSupportedLoad(extensionBytes, "optional extension and comment", passed, failed);

            byte[] futureBytes = Mutate(legacyBytes, root => root.SetAttributeValue("SchemaVersion", "2"));
            WriteFixture(outputDirectory, "future-schema-v2.xml", futureBytes);
            CheckBlockedLoad(
                futureBytes,
                VisionPipelineXmlSchemaIssueKind.FutureSchemaVersion,
                "future schema v2",
                passed,
                failed);
            CheckExecutionPlanBlocked(
                futureBytes,
                VisionPipelineXmlSchemaIssueKind.FutureSchemaVersion,
                "future schema v2 execution plan",
                passed,
                failed);

            byte[] unknownElementBytes = Mutate(legacyBytes, root =>
                root.Add(new XElement("RequiredFutureMeaning", "must-not-be-dropped")));
            WriteFixture(outputDirectory, "unknown-critical-element.xml", unknownElementBytes);
            CheckBlockedLoad(
                unknownElementBytes,
                VisionPipelineXmlSchemaIssueKind.UnknownCriticalElement,
                "unknown critical element",
                passed,
                failed);

            CheckStoragePreservesFutureXml(futureBytes, outputDirectory, passed, failed);
        }
        catch (Exception exception)
        {
            failed.Add("Unexpected contract exception: " + exception);
        }
        finally
        {
            Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, previousDataRoot);
            VisionPipelineStorage.ResetRuntimePersistenceStateForTest();
        }

        string reportPath = Path.Combine(outputDirectory, "pipeline-xml-schema-compatibility-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failed.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-017 supported/legacy Pipeline XML schema, future-schema fail-closed execution, extension compatibility, and source preservation",
                "Owner: VisionPipelineXmlSchemaPolicy -> SerializeHelper -> VisionPipelineStorage/VisionPipelineExecutionPlan",
                "Supported range: unversioned legacy XML and explicit schema version 1",
                "EvidenceDirectory: " + outputDirectory
            }
            .Concat(passed.Select(item => "PASS: " + item))
            .Concat(failed.Select(item => "FAIL: " + item)));

        foreach (string item in failed)
            Console.WriteLine("FAIL|" + item);
        Console.WriteLine($"CONTRACT|pipeline-xml-schema-compatibility|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void CheckSupportedLoad(
        byte[] bytes,
        string caseName,
        List<string> passed,
        List<string> failed)
    {
        VisionPipelineXmlSchemaInspection inspection = VisionPipelineXmlSchemaPolicy.Inspect(bytes);
        if (!inspection.IsSupported)
        {
            failed.Add(caseName + " was rejected: " + inspection.ErrorMessage);
            return;
        }

        if (!SerializeHelper.TryLoadFromXmlBytes(
                bytes,
                out VisionPipeline pipeline,
                out Exception loadException)
            || pipeline == null)
        {
            failed.Add(caseName + " could not be loaded: " + loadException?.Message);
            return;
        }

        passed.Add(caseName + " remains loadable with schema v" + inspection.SchemaVersion + ".");
    }

    private static void CheckBlockedLoad(
        byte[] bytes,
        VisionPipelineXmlSchemaIssueKind expectedIssue,
        string caseName,
        List<string> passed,
        List<string> failed)
    {
        VisionPipelineXmlSchemaInspection inspection = VisionPipelineXmlSchemaPolicy.Inspect(bytes);
        if (inspection.IssueKind != expectedIssue || !inspection.PreserveOriginal)
        {
            failed.Add(caseName + " inspection mismatch: " + inspection.IssueKind + ".");
            return;
        }

        bool loaded = SerializeHelper.TryLoadFromXmlBytes(
            bytes,
            out VisionPipeline pipeline,
            out Exception loadException);
        if (loaded || pipeline != null || loadException is not VisionPipelineXmlSchemaException schemaException
            || schemaException.IssueKind != expectedIssue
            || !schemaException.PreserveOriginal)
        {
            failed.Add(caseName + " was not rejected with the typed schema error.");
            return;
        }

        passed.Add(caseName + " is rejected before deserialization and marked for original preservation.");
    }

    private static void CheckExecutionPlanBlocked(
        byte[] bytes,
        VisionPipelineXmlSchemaIssueKind expectedIssue,
        string caseName,
        List<string> passed,
        List<string> failed)
    {
        try
        {
            VisionPipelineExecutionPlan.Create(
                new VisionPipeline { Name = "SchemaContract" },
                originalXmlBytes: bytes);
            failed.Add(caseName + " unexpectedly produced an execution plan.");
        }
        catch (VisionPipelineXmlSchemaException exception)
            when (exception.IssueKind == expectedIssue && exception.PreserveOriginal)
        {
            passed.Add(caseName + " is blocked before normalization and native execution.");
        }
        catch (Exception exception)
        {
            failed.Add(caseName + " raised the wrong exception: " + exception);
        }
    }

    private static void CheckStoragePreservesFutureXml(
        byte[] futureBytes,
        string outputDirectory,
        List<string> passed,
        List<string> failed)
    {
        string dataRoot = Path.Combine(outputDirectory, "storage-data");
        string recipeName = "SchemaContractRecipe";
        string pipelineName = "SchemaContractPipeline";
        Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);
        VisionPipelineStorage.ResetRuntimePersistenceStateForTest();

        string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
        File.WriteAllBytes(path, futureBytes);
        VisionPipelineStorage.Load(recipeName, pipelineName);

        bool bytesPreserved = futureBytes.SequenceEqual(File.ReadAllBytes(path));
        bool hasState = VisionPipelineStorage.TryGetPersistenceState(
            recipeName,
            pipelineName,
            out VisionPipelinePersistenceState state);
        string[] backups = Directory.GetFiles(
            Path.GetDirectoryName(path)!,
            Path.GetFileNameWithoutExtension(path) + ".invalid-*" + Path.GetExtension(path));
        if (!bytesPreserved || !hasState || state.Kind != VisionPipelinePersistenceStateKind.LoadFailed
            || !string.IsNullOrWhiteSpace(state.BackupPath) || backups.Length != 0)
        {
            failed.Add("VisionPipelineStorage did not preserve future-schema source bytes without substitution.");
            return;
        }

        passed.Add("VisionPipelineStorage reports LoadFailed and leaves future-schema source bytes untouched.");
    }

    private static byte[] Mutate(byte[] source, Action<XElement> mutation)
    {
        XDocument document = XDocument.Parse(Encoding.UTF8.GetString(source), LoadOptions.PreserveWhitespace);
        mutation(document.Root!);
        return Encoding.UTF8.GetBytes(document.ToString(SaveOptions.DisableFormatting));
    }

    private static void WriteFixture(string outputDirectory, string fileName, byte[] bytes)
    {
        File.WriteAllBytes(Path.Combine(outputDirectory, fileName), bytes);
    }

    private static string CreateLegacyXml()
    {
        return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<VisionPipeline><Name>SchemaContract</Name><Steps>" +
            "<Step><Name>Threshold</Name><ToolType>Threshold</ToolType><Enabled>true</Enabled>" +
            "<InputLayer>Main</InputLayer><OutputLayer>Output</OutputLayer><Parameters>" +
            "<Parameter><Key>Mode</Key><Value>Threshold</Value></Parameter>" +
            "</Parameters></Step></Steps></VisionPipeline>";
    }
}
