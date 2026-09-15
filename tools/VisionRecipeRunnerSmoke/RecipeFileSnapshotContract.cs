using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using OpenCvSharp;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

internal static class RecipeFileSnapshotContract
{
    public static async Task<int> RunAsync(string evidenceDirectory)
    {
        evidenceDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new();
        List<string> failed = new();

        await RunCaseAsync("file runner captures bytes once before deserialization", () =>
        {
            string repoRoot = FindRepoRoot();
            string source = File.ReadAllText(Path.Combine(
                repoRoot,
                "src",
                "OpenVisionLab",
                "Core",
                "Pipeline",
                "Execution",
                "VisionRecipeRunner.cs"));
            Require(source.Split("File.ReadAllBytes(recipeXmlPath)", StringSplitOptions.None).Length - 1 == 1,
                "File runner must capture recipe bytes exactly once.");
            Require(!source.Contains("File.ReadAllText(recipeXmlPath)", StringComparison.Ordinal),
                "File runner must not read recipe text through a second file handle.");
            Require(!source.Contains("TryLoadFromXmlFile(recipeXmlPath", StringComparison.Ordinal),
                "File runner must deserialize from the captured bytes rather than re-opening the recipe.");
        }, passed, failed);

        await RunCaseAsync("UTF-8 BOM and non-BOM bytes deserialize from one snapshot", () =>
        {
            byte[] noBom = EncodeRecipe("SnapshotUtf8", new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            byte[] bom = EncodeRecipe("SnapshotUtf8Bom", new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            AssertLoadedPipeline(noBom, "SnapshotUtf8");
            AssertLoadedPipeline(bom, "SnapshotUtf8Bom");
        }, passed, failed);

        await RunCaseAsync("UTF-16 bytes preserve the original snapshot hash", () =>
        {
            byte[] bytes = EncodeRecipe("SnapshotUtf16", Encoding.Unicode);
            AssertLoadedPipeline(bytes, "SnapshotUtf16");
            VisionPipeline pipeline = LoadPipeline(bytes);
            VisionPipelineExecutionPlan plan = VisionPipelineExecutionPlan.Create(pipeline, null, bytes);
            string expectedHash = Convert.ToHexString(SHA256.HashData(bytes));
            Require(plan.OriginalPipelineXmlBytes.SequenceEqual(bytes),
                "Execution plan changed the original UTF-16 XML bytes.");
            Require(string.Equals(plan.Provenance.OriginalPipelineSha256, expectedHash, StringComparison.Ordinal),
                "Execution plan hash does not match the original UTF-16 XML bytes.");
        }, passed, failed);

        await RunCaseAsync("A and B recipe generations keep their own identity", async () =>
        {
            string recipePath = Path.Combine(evidenceDirectory, "generation-switch.xml");
            byte[] a = EncodeRecipe("GenerationA", new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            byte[] b = EncodeRecipe("GenerationB", Encoding.Unicode);
            File.WriteAllBytes(recipePath, a);
            using Mat source = new(16, 16, MatType.CV_8UC1, Scalar.White);
            using VisionRecipeRunResult resultA = await new VisionRecipeRunner().RunAsync(recipePath, source);
            Require(resultA.OriginalPipelineXmlBytes.SequenceEqual(a),
                "Generation A result did not retain the bytes read for its run.");
            Require(string.Equals(resultA.PipelineName, "GenerationA", StringComparison.Ordinal),
                "Generation A result did not retain its pipeline identity.");
            Require(string.Equals(resultA.ExecutionProvenance.OriginalPipelineSha256, Hash(a), StringComparison.Ordinal),
                "Generation A provenance hash does not match its original bytes.");

            File.WriteAllBytes(recipePath, b);
            using VisionRecipeRunResult resultB = await new VisionRecipeRunner().RunAsync(recipePath, source);
            Require(resultB.OriginalPipelineXmlBytes.SequenceEqual(b),
                "Generation B result did not retain the bytes read for its run.");
            Require(string.Equals(resultB.PipelineName, "GenerationB", StringComparison.Ordinal),
                "Generation B result did not retain its pipeline identity.");
            Require(string.Equals(resultB.ExecutionProvenance.OriginalPipelineSha256, Hash(b), StringComparison.Ordinal),
                "Generation B provenance hash does not match its original bytes.");
        }, passed, failed);

        await RunCaseAsync("Invalid XML bytes fail before execution", () =>
        {
            byte[] invalid = Encoding.UTF8.GetBytes("<VisionPipeline><broken>");
            Require(!SerializeHelper.TryLoadFromXmlBytes(invalid, out VisionPipeline _, out Exception error),
                "Invalid XML bytes were accepted.");
            Require(error != null, "Invalid XML bytes did not retain a load error.");
        }, passed, failed);

        await RunCaseAsync("Invalid recipe file fails before pipeline execution", async () =>
        {
            string recipePath = Path.Combine(evidenceDirectory, "invalid-recipe.xml");
            File.WriteAllText(recipePath, "<VisionPipeline><broken>", Encoding.UTF8);
            using Mat source = new(16, 16, MatType.CV_8UC1, Scalar.White);
            bool failedBeforeExecution = false;
            try
            {
                using VisionRecipeRunResult _ = await new VisionRecipeRunner().RunAsync(recipePath, source);
            }
            catch (InvalidOperationException)
            {
                failedBeforeExecution = true;
            }

            Require(failedBeforeExecution, "Invalid recipe file did not stop before execution.");
        }, passed, failed);

        string reportPath = Path.Combine(evidenceDirectory, "recipe-file-snapshot-contract.txt");
        File.WriteAllLines(reportPath, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine($"CONTRACT|recipe-file-snapshot|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static async Task RunCaseAsync(
        string name,
        Action action,
        List<string> passed,
        List<string> failed)
    {
        try
        {
            action();
            await Task.CompletedTask;
            passed.Add(name);
        }
        catch (Exception exception)
        {
            failed.Add($"{name}: {exception.GetBaseException().Message}");
        }
    }

    private static async Task RunCaseAsync(
        string name,
        Func<Task> action,
        List<string> passed,
        List<string> failed)
    {
        try
        {
            await action();
            passed.Add(name);
        }
        catch (Exception exception)
        {
            failed.Add($"{name}: {exception.GetBaseException().Message}");
        }
    }

    private static byte[] EncodeRecipe(string name, Encoding encoding)
    {
        VisionPipeline pipeline = new() { Name = name };
        XmlSerializer serializer = new(typeof(VisionPipeline));
        using MemoryStream stream = new();
        using (StreamWriter writer = new(stream, encoding, 1024, leaveOpen: true))
        {
            serializer.Serialize(writer, pipeline);
        }

        return stream.ToArray();
    }

    private static VisionPipeline LoadPipeline(byte[] bytes)
    {
        Require(SerializeHelper.TryLoadFromXmlBytes(bytes, out VisionPipeline pipeline, out Exception error),
            $"Recipe bytes did not deserialize: {error?.Message}");
        Require(pipeline != null, "Recipe bytes returned a null pipeline.");
        return pipeline!;
    }

    private static void AssertLoadedPipeline(byte[] bytes, string expectedName)
    {
        VisionPipeline pipeline = LoadPipeline(bytes);
        Require(string.Equals(pipeline.Name, expectedName, StringComparison.Ordinal),
            $"Expected pipeline name {expectedName} but got {pipeline.Name}.");
    }

    private static string Hash(byte[] bytes)
    {
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static string FindRepoRoot()
    {
        DirectoryInfo? current = new(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "OpenVisionLab.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("OpenVisionLab repository root was not found.");
    }
}
