using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using OpenVisionLab;

internal static class RecipeExternalAssetReconnectionContract
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

            string fixtureRoot = Path.Combine(outputDirectory, "fixture");
            string sourceDirectory = Path.Combine(fixtureRoot, "source");
            string bundleDirectory = Path.Combine(fixtureRoot, "bundle");
            Directory.CreateDirectory(sourceDirectory);
            Directory.CreateDirectory(bundleDirectory);

            string sourcePath = Path.Combine(sourceDirectory, "locator-template.png");
            byte[] sourceBytes = Encoding.UTF8.GetBytes("OpenVisionLab-2D-020-source-asset");
            File.WriteAllBytes(sourcePath, sourceBytes);
            string sourceSha256 = ComputeSha256(sourceBytes);

            VisionPipeline pipeline = CreatePipeline(sourcePath);
            string pipelineXmlPath = Path.Combine(fixtureRoot, "pipeline.xml");
            SerializeHelper.SaveXmlFile(pipelineXmlPath, pipeline);
            string pipelineXml = File.ReadAllText(pipelineXmlPath);
            string originalPipelineSha256 = ComputeSha256(File.ReadAllBytes(pipelineXmlPath));
            string bundlePath = Path.Combine(bundleDirectory, "portable-asset.review.zip");

            bool exported = OpenVisionRecipeReviewBundleExporter.TryExport(
                bundlePath,
                "AssetRelocationSource",
                pipeline.Name,
                pipeline,
                pipelineXml,
                Array.Empty<OpenVisionRecipeReviewReference>(),
                out string exportMessage);
            Check(
                exported && File.Exists(bundlePath),
                "The existing review-bundle exporter records the external asset path, size, and SHA-256 without copying the asset",
                passed,
                failed);
            if (!exported)
            {
                throw new InvalidDataException("Review bundle export failed: " + exportMessage);
            }

            File.Delete(sourcePath);
            string candidatePath = Path.Combine(bundleDirectory, Path.GetFileName(sourcePath));
            File.WriteAllBytes(candidatePath, sourceBytes);

            Check(
                OpenVisionRecipeReviewBundleInspector.TryInspect(
                    bundlePath,
                    out OpenVisionRecipeReviewBundleInspection relocationInspection),
                "A moved source can still be inspected through the existing bundle schema",
                passed,
                failed);
            OpenVisionRecipeReviewBundlePathReview? relocationReview =
                relocationInspection.PathReviews.FirstOrDefault(item => item.IsDependency);
            Check(
                relocationReview != null
                    && relocationReview.State == OpenVisionRecipeReviewBundlePathState.RelocationCandidate
                    && string.Equals(
                        relocationReview.ReviewedPath,
                        candidatePath,
                        StringComparison.OrdinalIgnoreCase)
                    && string.Equals(
                        ComputeSha256(File.ReadAllBytes(candidatePath)),
                        sourceSha256,
                        StringComparison.Ordinal),
                "A missing original path is classified as a deterministic adjacent SHA-matched relocation candidate",
                passed,
                failed);
            Check(
                relocationInspection.IntegrityReport.Contains("Preview", StringComparison.Ordinal)
                    && relocationInspection.PathReport.Contains("XML", StringComparison.Ordinal)
                    && (relocationInspection.PathReport.Contains(
                            "not changed or executed",
                            StringComparison.Ordinal)
                        || relocationInspection.PathReport.Contains(
                            "실행하지 않습니다",
                            StringComparison.Ordinal)),
                "Bundle inspection reports the affected dependency and preserves the no-import/no-Preview/no-Run dry-run boundary",
                passed,
                failed);

            if (!SerializeHelper.TryLoadFromXmlText(
                    relocationInspection.PipelineXml,
                    out VisionPipeline relocatedPipeline,
                    out string loadError)
                || relocatedPipeline == null)
            {
                throw new InvalidDataException("Relocation fixture Pipeline could not be loaded: " + loadError);
            }

            string originalParameter = relocatedPipeline.Steps[0].Parameters["TemplatePath"];
            OpenVisionRecipeDependencyReviewResult relocationGate =
                OpenVisionRecipeDependencyReviewService.Review(
                    relocatedPipeline,
                    "AssetRelocationReviewOnly",
                    copyDependencies: false,
                    relocationInspection);
            Check(
                relocationGate.BlockingDependencyCount == 1
                    && relocationGate.Rows.Any(row =>
                        row.StepName == "Matching"
                        && row.ParameterName == "TemplatePath"
                        && row.Path.Equals(candidatePath, StringComparison.OrdinalIgnoreCase)
                        && (row.Status.Contains("Relocation", StringComparison.OrdinalIgnoreCase)
                            || row.Status.Contains("재배치", StringComparison.Ordinal))),
                "Dependency review identifies the affected Step and keeps a relocation candidate blocked until an explicit path decision",
                passed,
                failed);
            Check(
                string.Equals(
                    relocatedPipeline.Steps[0].Parameters["TemplatePath"],
                    originalParameter,
                    StringComparison.Ordinal),
                "Dry review does not rewrite the Pipeline XML path",
                passed,
                failed);

            relocatedPipeline.Steps[0].Parameters["TemplatePath"] = candidatePath;
            string adoptedRecipeName = UniqueName("AssetAdopted");
            OpenVisionRecipeDependencyReviewResult adoption =
                OpenVisionRecipeDependencyReviewService.Review(
                    relocatedPipeline,
                    adoptedRecipeName,
                    copyDependencies: true,
                    relocationInspection);
            string adoptedParameter = relocatedPipeline.Steps[0].Parameters["TemplatePath"];
            string adoptedPath = OpenVisionRecipeDependencyReviewService.ResolveDependencySourcePath(adoptedParameter);
            Check(
                adoption.BlockingDependencyCount == 0
                    && File.Exists(adoptedPath)
                    && !Path.IsPathRooted(adoptedParameter)
                    && string.Equals(
                        ComputeSha256(File.ReadAllBytes(adoptedPath)),
                        sourceSha256,
                        StringComparison.Ordinal)
                    && string.Equals(
                        ComputeSha256(File.ReadAllBytes(candidatePath)),
                        sourceSha256,
                        StringComparison.Ordinal),
                "After an explicit path change, the existing copy owner adopts the verified asset into the Recipe workspace without changing the source bytes",
                passed,
                failed);

            VisionPipelineStorage.Save(adoptedRecipeName, relocatedPipeline);
            string savedPipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(
                adoptedRecipeName,
                relocatedPipeline.Name);
            VisionPipeline persistedPipeline = VisionPipelineStorage.Load(
                adoptedRecipeName,
                relocatedPipeline.Name);
            string persistedPipelineSha256 = ComputeSha256(File.ReadAllBytes(savedPipelinePath));
            Check(
                File.Exists(savedPipelinePath)
                    && persistedPipeline.Steps.Count == 1
                    && string.Equals(
                        persistedPipeline.Steps[0].Parameters["TemplatePath"],
                        adoptedParameter,
                        StringComparison.Ordinal)
                    && !string.Equals(
                        originalPipelineSha256,
                        persistedPipelineSha256,
                        StringComparison.Ordinal),
                "Explicit adoption persists a changed Recipe Pipeline content hash, providing the revision boundary without executing the recipe",
                passed,
                failed);

            VisionPipeline unresolvedPipeline = CreatePipeline(sourcePath);
            byte[] wrongBytes = Enumerable.Repeat((byte)0xA5, sourceBytes.Length).ToArray();
            File.WriteAllBytes(candidatePath, wrongBytes);
            Check(
                OpenVisionRecipeReviewBundleInspector.TryInspect(
                    bundlePath,
                    out OpenVisionRecipeReviewBundleInspection mismatchInspection),
                "The same-name wrong-bytes fixture remains inspectable",
                passed,
                failed);
            OpenVisionRecipeReviewBundlePathReview? mismatchReview =
                mismatchInspection.PathReviews.FirstOrDefault(item => item.IsDependency);
            string mismatchRecipeName = UniqueName("AssetMismatch");
            OpenVisionRecipeDependencyReviewResult mismatchGate =
                OpenVisionRecipeDependencyReviewService.Review(
                    unresolvedPipeline,
                    mismatchRecipeName,
                    copyDependencies: true,
                    mismatchInspection);
            string mismatchRecipeDirectory = RecipeWorkspaceService.GetRecipeDirectoryPath(mismatchRecipeName);
            Check(
                mismatchReview != null
                    && mismatchReview.State == OpenVisionRecipeReviewBundlePathState.ContentMismatch
                    && mismatchGate.BlockingDependencyCount == 1
                    && !Directory.Exists(mismatchRecipeDirectory),
                "A same-name file with different bytes is classified as content mismatch and is never copied into a Recipe",
                passed,
                failed);

            File.Delete(candidatePath);
            Check(
                OpenVisionRecipeReviewBundleInspector.TryInspect(
                    bundlePath,
                    out OpenVisionRecipeReviewBundleInspection missingInspection),
                "A missing original and missing adjacent candidate remain inspectable",
                passed,
                failed);
            OpenVisionRecipeReviewBundlePathReview? missingReview =
                missingInspection.PathReviews.FirstOrDefault(item => item.IsDependency);
            OpenVisionRecipeDependencyReviewResult missingGate =
                OpenVisionRecipeDependencyReviewService.Review(
                    unresolvedPipeline,
                    UniqueName("AssetMissing"),
                    copyDependencies: false,
                    missingInspection);
            Check(
                missingReview != null
                    && missingReview.State == OpenVisionRecipeReviewBundlePathState.Missing
                    && missingGate.BlockingDependencyCount == 1,
                "A missing source and candidate are distinguished from both relocation and content mismatch and remain blocking",
                passed,
                failed);

            CheckQualificationOwners(passed, failed);
            CheckImportBoundary(passed, failed);
        }
        catch (Exception exception)
        {
            failed.Add("Unexpected contract exception: " + exception);
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                AppPathService.DataRootEnvironmentVariable,
                previousDataRoot);
        }

        string reportPath = Path.Combine(
            outputDirectory,
            "recipe-external-asset-reconnection-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failed.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-020 missing/moved/damaged external asset identification and explicit reconnection",
                "Owner: review-bundle exporter/inspector -> dependency review service; qualification hash/archive owners remain separate",
                "EvidenceDirectory: " + outputDirectory,
                "Boundary: no desktop WPF/EXE click path, hardware, or field qualification was executed"
            }
            .Concat(passed.Select(item => "PASS: " + item))
            .Concat(failed.Select(item => "FAIL: " + item)));

        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine(
            $"CONTRACT|recipe-external-asset-reconnection|passed={passed.Count}|failed={failed.Count}");
        Console.WriteLine(reportPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static VisionPipeline CreatePipeline(string sourcePath)
    {
        VisionPipeline pipeline = new VisionPipeline
        {
            Name = "PortableAssetPipeline"
        };
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "Matching",
            ToolType = "Matching",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "Output"
        };
        step.Parameters["TemplatePath"] = sourcePath;
        pipeline.Steps.Add(step);
        return pipeline;
    }

    private static void CheckQualificationOwners(
        List<string> passed,
        List<string> failed)
    {
        string preflightSource = ReadRepositoryFile(Path.Combine(
            "src",
            "OpenVisionLab",
            "Core",
            "Recipe",
            "Qualification",
            "QualifiedRecipeSnapshotPreflight.cs"));
        string storeSource = ReadRepositoryFile(Path.Combine(
            "src",
            "OpenVisionLab",
            "Core",
            "Recipe",
            "Qualification",
            "QualifiedRecipeSnapshotStore.cs"));
        string workingCopySource = ReadRepositoryFile(Path.Combine(
            "src",
            "OpenVisionLab",
            "Core",
            "Recipe",
            "Qualification",
            "QualifiedRecipeSnapshotWorkingCopyService.cs"));

        Check(
            preflightSource.Contains("Pipeline dependency", StringComparison.Ordinal)
                && preflightSource.Contains("ValidateHashedSourceFile", StringComparison.Ordinal)
                && preflightSource.Contains("SHA-256 mismatch", StringComparison.Ordinal),
            "Qualification preflight separately rejects missing or hash-mismatched Pipeline dependencies",
            passed,
            failed);
        Check(
            storeSource.Contains("WriteDependencies", StringComparison.Ordinal)
                && storeSource.Contains("VerifyManifestFile", StringComparison.Ordinal)
                && storeSource.Contains("Dependency size mismatch", StringComparison.Ordinal),
            "QualifiedRecipeSnapshotStore archives and verifies dependency bytes with manifest hashes",
            passed,
            failed);
        Check(
            workingCopySource.Contains("CopyDependencies", StringComparison.Ordinal)
                && workingCopySource.Contains("RewriteDependencyParameters", StringComparison.Ordinal)
                && workingCopySource.Contains("VisionPipelineStorage.Save", StringComparison.Ordinal)
                && workingCopySource.Contains("store.Verify", StringComparison.Ordinal),
            "Qualified working-copy creation uses the verified archive, rewrites dependency parameters, and saves a new Recipe",
            passed,
            failed);
    }

    private static void CheckImportBoundary(
        List<string> passed,
        List<string> failed)
    {
        string inspectorSource = ReadRepositoryFile(Path.Combine(
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Recipe",
            "Review",
            "OpenVisionRecipeReviewBundleInspector.cs"));
        string commandSource = ReadRepositoryFile(Path.Combine(
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Recipe",
            "CommandSurface",
            "RecipeCommandSurface.cs"));

        Check(
            inspectorSource.Contains("RelocationCandidate", StringComparison.Ordinal)
                && inspectorSource.Contains("ContentMismatch", StringComparison.Ordinal)
                && inspectorSource.Contains(
                    "Preview, and Run are not changed or executed",
                    StringComparison.Ordinal),
            "The existing inspector owns relocation/content-mismatch states and the no-side-effect bundle policy",
            passed,
            failed);
        Check(
            commandSource.Contains("LoadReviewBundleForDryRun", StringComparison.Ordinal)
                && commandSource.Contains("copyDependencies: true", StringComparison.Ordinal)
                && commandSource.Contains("VisionPipelineStorage.Save(recipeName, pipeline)", StringComparison.Ordinal)
                && commandSource.Contains("refreshAfterSwitch()", StringComparison.Ordinal),
            "RecipeCommandSurface keeps dry review separate from the explicit import/save/refresh path",
            passed,
            failed);
    }

    private static string UniqueName(string prefix)
    {
        return prefix + "_" + Guid.NewGuid().ToString("N")[..12];
    }

    private static string ComputeSha256(byte[] bytes)
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
