using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using OpenVisionLab;

internal static class RecipePersistenceExecutionGateContract
{
    public static int Run(string evidenceDirectory)
    {
        string outputDirectory = Path.GetFullPath(evidenceDirectory);
        Directory.CreateDirectory(outputDirectory);
        string? previousDataRoot = Environment.GetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();

        try
        {
            string dataRoot = Path.Combine(outputDirectory, "data");
            Environment.SetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable, dataRoot);
            VisionPipelineStorage.ResetRuntimePersistenceStateForTest();

            CheckNewRecipeCreation(passed, failed);
            CheckDamagedPipelineRecoveryGate(passed, failed);
            CheckDamagedRecipeDataRecoveryGate(passed, failed);
            CheckRunOwnersUsePersistenceGate(passed, failed);
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

        string reportPath = Path.Combine(outputDirectory, "recipe-persistence-execution-gate-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failed.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-019 damaged Recipe/Pipeline persistence must be explicitly recovered before Run",
                "Owner: VisionPipelineStorage/RecipeDataStorage -> persistence presenter -> Review/Recipe command gates",
                "EvidenceDirectory: " + outputDirectory
            }
            .Concat(passed.Select(item => "PASS: " + item))
            .Concat(failed.Select(item => "FAIL: " + item)));

        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine($"CONTRACT|recipe-persistence-execution-gate|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void CheckNewRecipeCreation(
        List<string> passed,
        List<string> failed)
    {
        string recipeName = UniqueName("NewRecipe");
        string pipelineName = "Pipeline";
        VisionPipeline pipeline = VisionPipelineStorage.Load(recipeName, pipelineName);
        string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);

        Check(
            pipeline != null
                && pipeline.Steps.Count == 0
                && File.Exists(path)
                && !VisionPipelineStorage.TryGetPersistenceState(
                    recipeName,
                    pipelineName,
                    out _),
            "A missing new Recipe creates the default Pipeline without a persistence failure",
            passed,
            failed);
    }

    private static void CheckDamagedPipelineRecoveryGate(
        List<string> passed,
        List<string> failed)
    {
        string recipeName = UniqueName("DamagedPipeline");
        string pipelineName = "Pipeline";
        string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
        byte[] damagedBytes = Encoding.UTF8.GetBytes("<VisionPipeline><Steps><Step>");
        File.WriteAllBytes(path, damagedBytes);

        VisionPipeline substituted = VisionPipelineStorage.Load(recipeName, pipelineName);
        bool hasState = VisionPipelineStorage.TryGetPersistenceState(
            recipeName,
            pipelineName,
            out VisionPipelinePersistenceState state);
        string originalHash = ComputeHash(damagedBytes);
        string currentHash = ComputeHash(File.ReadAllBytes(path));
        OpenVisionRecipePipelineOption option = OpenVisionRecipePipelineOption.Create(
            recipeName,
            pipelineName,
            pipelineName);

        Check(
            substituted != null
                && substituted.Steps.Count == 0
                && hasState
                && state.Kind == VisionPipelinePersistenceStateKind.InvalidFileSubstituted
                && state.IsFailure
                && !string.IsNullOrWhiteSpace(state.BackupPath)
                && File.Exists(state.BackupPath)
                && string.Equals(originalHash, currentHash, StringComparison.Ordinal)
                && option.HasPersistenceFailure
                && !option.XmlValid
                && !option.RouteValid,
            "Damaged Pipeline keeps the source bytes and backup while the option remains blocked",
            passed,
            failed);

        VisionPipeline repaired = CreateRunnablePipeline(pipelineName);
        SerializeHelper.SaveXmlFile(path, repaired);
        VisionPipeline loadedAfterExternalRepair = VisionPipelineStorage.Load(recipeName, pipelineName);
        bool staleFailureRemains = VisionPipelineStorage.TryGetPersistenceState(
                recipeName,
                pipelineName,
                out VisionPipelinePersistenceState staleState)
            && staleState.IsFailure;
        OpenVisionRecipePipelineOption staleOption = OpenVisionRecipePipelineOption.Create(
            recipeName,
            pipelineName,
            pipelineName);

        Check(
            loadedAfterExternalRepair.Steps.Count == 1
                && staleFailureRemains
                && staleOption.HasPersistenceFailure,
            "An externally repaired Pipeline remains blocked until the explicit storage owner saves it",
            passed,
            failed);

        VisionPipelineStorage.Save(recipeName, loadedAfterExternalRepair);
        bool recovered = VisionPipelineStorage.TryGetPersistenceState(
                recipeName,
                pipelineName,
                out VisionPipelinePersistenceState recoveredState)
            && recoveredState.Kind == VisionPipelinePersistenceStateKind.SaveRecovered
            && !recoveredState.IsFailure
            && File.Exists(recoveredState.BackupPath);
        OpenVisionRecipePipelineOption recoveredOption = OpenVisionRecipePipelineOption.Create(
            recipeName,
            pipelineName,
            pipelineName);

        Check(
            recovered
                && !recoveredOption.HasPersistenceFailure,
            "Explicit Pipeline Save transitions the state to SaveRecovered and releases the persistence gate",
            passed,
            failed);
    }

    private static void CheckDamagedRecipeDataRecoveryGate(
        List<string> passed,
        List<string> failed)
    {
        string recipeName = UniqueName("DamagedData");
        string path = RecipeWorkspaceService.GetVisionDataPath(recipeName);
        byte[] damagedBytes = Encoding.UTF8.GetBytes("<CData><broken");
        File.WriteAllBytes(path, damagedBytes);

        DataState loaded = new DataState().LoadConfig(recipeName);
        bool hasState = RecipeDataStorage.TryGetPersistenceState(
            recipeName,
            out RecipeDataPersistenceState state);
        string originalHash = ComputeHash(damagedBytes);
        string currentHash = ComputeHash(File.ReadAllBytes(path));
        string helpText = OpenVisionRecipePersistenceStatusPresenter.CreateHelpText(state);

        Check(
            loaded != null
                && hasState
                && state.Kind == RecipeDataPersistenceStateKind.InvalidFileSubstituted
                && state.IsFailure
                && !string.IsNullOrWhiteSpace(state.BackupPath)
                && File.Exists(state.BackupPath)
                && string.Equals(originalHash, currentHash, StringComparison.Ordinal)
                && (helpText.Contains("Do not use this state as Run", StringComparison.Ordinal)
                    || helpText.Contains("실행·검증·적격화", StringComparison.Ordinal)),
            "Damaged Recipe data keeps the source bytes/backup and explains the Run restriction",
            passed,
            failed);

        if (loaded == null)
        {
            return;
        }

        loaded.SaveConfig(recipeName);
        Check(
            RecipeDataStorage.TryGetPersistenceState(
                    recipeName,
                    out RecipeDataPersistenceState recoveredState)
                && recoveredState.Kind == RecipeDataPersistenceStateKind.SaveRecovered
                && !recoveredState.IsFailure
                && File.Exists(recoveredState.BackupPath),
            "Explicit Recipe data Save transitions the state to SaveRecovered",
            passed,
            failed);
    }

    private static void CheckRunOwnersUsePersistenceGate(
        List<string> passed,
        List<string> failed)
    {
        string documentSource = ReadRepositoryFile(Path.Combine(
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Documents",
            "OpenVisionPipelineReviewDocument.cs"));
        string commandSource = ReadRepositoryFile(Path.Combine(
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Recipe",
            "CommandSurface",
            "RecipeCommandSurface.cs"));
        string presenterSource = ReadRepositoryFile(Path.Combine(
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Recipe",
            "Review",
            "OpenVisionRecipePersistenceStatusPresenter.cs"));

        Check(
            documentSource.Contains("TryGetPersistenceExecutionBlock", StringComparison.Ordinal)
                && documentSource.Contains("VisionPipelineStorage.TryGetPersistenceState", StringComparison.Ordinal)
                && documentSource.Contains("RecipeDataStorage.TryGetPersistenceState", StringComparison.Ordinal)
                && documentSource.Contains("view.SetValidation(persistenceStatus, persistenceDetails)", StringComparison.Ordinal),
            "Pipeline Review Run checks both Pipeline and Recipe persistence states before execution",
            passed,
            failed);
        Check(
            commandSource.Contains("HasSelectedRecipePersistenceFailure", StringComparison.Ordinal)
                && commandSource.Contains("CanRunSelectedSampleCheck", StringComparison.Ordinal)
                && commandSource.Contains("CanRunSelectedSamplePairCheck", StringComparison.Ordinal)
                && commandSource.Contains("CanRunCatalogBenchmark", StringComparison.Ordinal)
                && commandSource.Contains("CanRunLocalValidationSet", StringComparison.Ordinal),
            "Recipe sample, pair, catalog, and local validation command owners retain the persistence gate",
            passed,
            failed);
        Check(
            presenterSource.Contains("Do not use this state as Run", StringComparison.Ordinal)
                && presenterSource.Contains("Preserved prior file", StringComparison.Ordinal),
            "The shared persistence presenter exposes the preserved source/backup and explicit Run restriction",
            passed,
            failed);
    }

    private static VisionPipeline CreateRunnablePipeline(string name)
    {
        VisionPipeline pipeline = new VisionPipeline { Name = name };
        pipeline.Steps.Add(new VisionPipelineStep
        {
            Name = "Threshold",
            ToolType = "Threshold",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "Output"
        });
        return pipeline;
    }

    private static string UniqueName(string prefix)
    {
        return prefix + "_" + Guid.NewGuid().ToString("N")[..12];
    }

    private static string ComputeHash(byte[] bytes)
    {
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    private static string ReadRepositoryFile(string relativePath)
    {
        foreach (string seed in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            DirectoryInfo? directory = new DirectoryInfo(Path.GetFullPath(seed));
            while (directory != null)
            {
                string candidate = Path.Combine(directory.FullName, relativePath);
                if (File.Exists(candidate))
                {
                    return File.ReadAllText(candidate);
                }

                directory = directory.Parent;
            }
        }

        throw new FileNotFoundException(
            "Repository source was not found for structural contract.",
            relativePath);
    }

    private static void Check(
        bool condition,
        string message,
        List<string> passed,
        List<string> failed)
    {
        (condition ? passed : failed).Add(message);
    }
}
