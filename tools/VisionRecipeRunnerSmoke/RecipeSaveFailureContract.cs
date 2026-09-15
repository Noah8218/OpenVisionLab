using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using OpenVisionLab.Vision2D.Pipeline;

internal static class RecipeSaveFailureContract
{
    public static int Run(string evidenceDirectory)
    {
        evidenceDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(evidenceDirectory);
        string? previousDataRoot = Environment.GetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable);
        List<string> passed = new();
        List<string> failed = new();
        try
        {
            string dataRoot = Path.Combine(evidenceDirectory, "data");
            Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);
            Require(string.Equals(AppPathService.DataRootDirectory, dataRoot, StringComparison.OrdinalIgnoreCase),
                "The test must initialize an isolated data root before loading a recipe.");

            VisionToolRepository tools = new();
            DataState data = new();
            int dataReads = 0;
            RecipeState recipe = new();
            recipe.SetRuntime(() => { dataReads++; return data; }, value => data = value, () => tools);
            recipe.Name = "Smoke_RecipeSave_" + Guid.NewGuid().ToString("N")[..12];
            Require(recipe.SaveTools(), "Initial recipe save failed.");
            string toolPath = RecipeWorkspaceService.GetVisionConfigPath(recipe.Name, "Blob_1");
            string dataPath = RecipeWorkspaceService.GetVisionDataPath(recipe.Name);
            byte[] originalTool = File.ReadAllBytes(toolPath);
            byte[] originalData = File.ReadAllBytes(dataPath);

            tools.Blobs[0].MIN_AREA = 413;
            dataReads = 0;
            bool saved;
            using (FileStream lockedTool = new(toolPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                saved = recipe.SaveTools();
            Check(!saved, "Tool write failure reaches RecipeState.SaveTools as false", passed, failed);
            Check(tools.LastStorageError is IOException, "Tool write failure retains the storage exception", passed, failed);
            Check(dataReads == 0, "Tool write failure stops before reading/saving Recipe data", passed, failed);
            Check(File.ReadAllBytes(toolPath).SequenceEqual(originalTool)
                && File.ReadAllBytes(dataPath).SequenceEqual(originalData), "Failed first Tool write preserves existing XML bytes", passed, failed);
            Check(!Directory.GetFiles(dataRoot, "*.tmp", SearchOption.AllDirectories).Any(),
                "Failed atomic Tool write leaves no temporary XML", passed, failed);

            Check(recipe.SaveTools() && tools.LastStorageError == null && dataReads == 1
                && new BlobProperty("Blob_1").LoadConfig(recipe.Name).MIN_AREA == 413,
                "Retry after unlocking saves the edited value and clears the error", passed, failed);

            bool dataFailurePropagated = false;
            using (FileStream lockedData = new(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try { recipe.SaveTools(); }
                catch (IOException) { dataFailurePropagated = true; }
            }
            Check(dataFailurePropagated && File.ReadAllBytes(dataPath).SequenceEqual(originalData),
                "Recipe data write failure still propagates without replacing existing XML", passed, failed);
            Check(recipe.SaveTools(), "Recipe data write can be retried after unlocking", passed, failed);

            CheckInjectedSaveStages(evidenceDirectory, passed, failed);
        }
        catch (Exception exception)
        {
            failed.Add(exception.ToString());
        }
        finally
        {
            Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, previousDataRoot);
        }

        string reportPath = Path.Combine(evidenceDirectory, "recipe-save-failure-contract.txt");
        File.WriteAllLines(reportPath, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        foreach (string item in failed)
            Console.WriteLine("FAIL|" + item);
        Console.WriteLine($"CONTRACT|recipe-save-failure|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void Check(bool condition, string message, List<string> passed, List<string> failed)
    {
        (condition ? passed : failed).Add(message);
    }

    private static void CheckInjectedSaveStages(
        string evidenceDirectory,
        List<string> passed,
        List<string> failed)
    {
        string directory = Path.Combine(evidenceDirectory, "serialize-helper-injection");
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, "pipeline.xml");
        VisionPipeline baseline = new VisionPipeline { Name = "SaveFailureBaseline" };
        VisionPipeline edited = new VisionPipeline { Name = "SaveFailureEdited" };
        SerializeHelper.SaveXmlFile(path, baseline);
        string originalHash = ComputeSha256(File.ReadAllBytes(path));

        using (SerializeHelper.BeginSaveFailureInjectionForTest(XmlSaveFailureStage.BeforeWrite))
        {
            bool failedAtWrite = TrySave(path, edited);
            Check(
                failedAtWrite
                    && string.Equals(
                        ComputeSha256(File.ReadAllBytes(path)),
                        originalHash,
                        StringComparison.Ordinal)
                    && GetTemporaryPaths(path).Length == 0,
                "Injected write failure preserves the previous XML hash and leaves no temporary file",
                passed,
                failed);
        }

        using (SerializeHelper.BeginSaveFailureInjectionForTest(XmlSaveFailureStage.BeforeReplace))
        {
            bool failedAtReplace = TrySave(path, edited);
            Check(
                failedAtReplace
                    && string.Equals(
                        ComputeSha256(File.ReadAllBytes(path)),
                        originalHash,
                        StringComparison.Ordinal)
                    && GetTemporaryPaths(path).Length == 0,
                "Injected replace failure preserves the previous XML hash and cleans the temporary file",
                passed,
                failed);
        }

        string[] retainedTemporaryPaths;
        using (FileStream lockedTarget = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read))
        using (SerializeHelper.BeginSaveFailureInjectionForTest(XmlSaveFailureStage.Cleanup))
        {
            bool failedAtCleanup = TrySave(path, edited);
            retainedTemporaryPaths = GetTemporaryPaths(path);
            Check(
                failedAtCleanup
                    && string.Equals(
                        ComputeSha256(File.ReadAllBytes(path)),
                        originalHash,
                        StringComparison.Ordinal)
                    && retainedTemporaryPaths.Length == 1,
                "Injected cleanup failure preserves the previous XML hash and leaves the failed temporary path available for recovery",
                passed,
                failed);
        }

        foreach (string temporaryPath in retainedTemporaryPaths)
        {
            File.Delete(temporaryPath);
        }

        Check(
            GetTemporaryPaths(path).Length == 0,
            "After the injected cleanup failure is released, the isolated temporary artifact can be removed without touching the prior XML",
            passed,
            failed);
        Check(
            SerializeHelper.SaveXmlFile(path, edited)
                && !string.Equals(
                    ComputeSha256(File.ReadAllBytes(path)),
                    originalHash,
                    StringComparison.Ordinal),
            "A normal retry after failure injection writes the new XML",
            passed,
            failed);
    }

    private static bool TrySave(string path, VisionPipeline pipeline)
    {
        try
        {
            SerializeHelper.SaveXmlFile(path, pipeline);
            return false;
        }
        catch (IOException)
        {
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return true;
        }
    }

    private static string[] GetTemporaryPaths(string path)
    {
        string directory = Path.GetDirectoryName(path) ?? Directory.GetCurrentDirectory();
        return Directory.GetFiles(
            directory,
            "." + Path.GetFileName(path) + ".*.tmp");
    }

    private static string ComputeSha256(byte[] bytes)
    {
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
