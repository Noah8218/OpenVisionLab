using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using OpenVisionLab;

internal static class RecipeMultiFileSaveRecoveryContract
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
                "The test must initialize an isolated data root before saving a recipe.");

            VisionToolRepository tools = new();
            DataState data = new();
            int dataReads = 0;
            RecipeState recipe = new();
            recipe.SetRuntime(() => { dataReads++; return data; }, value => data = value, () => tools);
            recipe.Name = "Smoke_MultiFileSave_" + Guid.NewGuid().ToString("N")[..12];
            Require(recipe.SaveTools(), "Initial Recipe save failed.");

            string blobPath = RecipeWorkspaceService.GetVisionConfigPath(recipe.Name, "Blob_1");
            string contourPath = RecipeWorkspaceService.GetVisionConfigPath(recipe.Name, "Contour_1");
            byte[] originalBlob = File.ReadAllBytes(blobPath);
            byte[] originalContour = File.ReadAllBytes(contourPath);
            tools.Blobs[0].MIN_AREA = 413;
            tools.Contours[0].MIN_AREA = 417;
            dataReads = 0;

            bool saved;
            using (FileStream lockedContour = new(contourPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                saved = recipe.SaveTools();

            byte[] failedBlob = File.ReadAllBytes(blobPath);
            byte[] failedContour = File.ReadAllBytes(contourPath);
            Check(!saved, "Later Tool write failure reaches RecipeState.SaveTools as false", passed, failed);
            Check(tools.LastStorageError is IOException, "Later Tool write failure retains the storage exception", passed, failed);
            Check(dataReads == 0, "Later Tool write failure stops before Recipe data save", passed, failed);
            Check(failedBlob.SequenceEqual(originalBlob),
                "Later Tool write failure rolls earlier Tool XML back to its original bytes", passed, failed);
            Check(failedContour.SequenceEqual(originalContour),
                "The locked later Tool XML keeps its original bytes", passed, failed);
            Check(!Directory.GetFiles(dataRoot, "*.tmp", SearchOption.AllDirectories).Any(),
                "Later Tool write failure leaves no temporary XML", passed, failed);

            Check(recipe.SaveTools() && tools.LastStorageError == null && dataReads == 1,
                "Retry after unlocking writes the complete Tool set", passed, failed);
        }
        catch (Exception exception)
        {
            failed.Add(exception.ToString());
        }
        finally
        {
            Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, previousDataRoot);
        }

        string reportPath = Path.Combine(evidenceDirectory, "recipe-multi-file-save-recovery-contract.txt");
        File.WriteAllLines(reportPath, passed.Select(item => "PASS: " + item).Concat(failed.Select(item => "FAIL: " + item)));
        foreach (string item in failed)
            Console.WriteLine("FAIL|" + item);
        Console.WriteLine($"CONTRACT|recipe-multi-file-save-recovery|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void Check(bool condition, string message, List<string> passed, List<string> failed)
    {
        (condition ? passed : failed).Add(message);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
