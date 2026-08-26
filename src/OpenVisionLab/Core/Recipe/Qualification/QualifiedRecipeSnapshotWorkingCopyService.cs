using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class QualifiedRecipeSnapshotWorkingCopyService
    {
        private readonly QualifiedRecipeSnapshotStore store;

        internal QualifiedRecipeSnapshotWorkingCopyService(
            QualifiedRecipeSnapshotStore store)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
        }

        internal QualifiedRecipeWorkingCopyResult Create(
            string snapshotId,
            string targetRecipeName)
        {
            string recipeName = targetRecipeName?.Trim() ?? string.Empty;
            if (!RecipeWorkspaceService.IsValidRecipeName(recipeName))
            {
                return Failure("Working-copy Recipe name is invalid.");
            }

            string targetDirectory = RecipeWorkspaceService.GetRecipeDirectoryPath(recipeName);
            if (Directory.Exists(targetDirectory))
            {
                return Failure("Working-copy Recipe already exists: " + recipeName);
            }

            QualifiedRecipeSnapshotVerificationResult verification =
                store.Verify(snapshotId);
            if (!verification.PayloadIntegrityValid || verification.Manifest == null)
            {
                return Failure(
                    "Working copy requires an intact snapshot payload: "
                    + string.Join(" | ", verification.Errors));
            }

            QualifiedRecipeSnapshotManifest manifest = verification.Manifest;
            string snapshotDirectory = store.GetSnapshotDirectory(snapshotId);
            string pipelineSource = ResolveVerifiedArchivePath(
                snapshotDirectory,
                manifest.PipelineFile);
            string validationSetPath = ResolveVerifiedArchivePath(
                snapshotDirectory,
                manifest.ValidationSetFile);
            if (!File.Exists(pipelineSource)
                || !SerializeHelper.TryLoadFromXmlFile(
                    pipelineSource,
                    out VisionPipeline pipeline)
                || pipeline == null
                || !SerializeHelper.TryLoadFromXmlFile(
                    validationSetPath,
                    out QualifiedRecipeValidationSetSnapshot validationSet)
                || validationSet == null)
            {
                return Failure("Snapshot Pipeline or Validation Set could not be loaded.");
            }

            bool created = false;
            try
            {
                RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
                created = true;
                Dictionary<string, string> replacementByOriginal =
                    CopyDependencies(
                        snapshotDirectory,
                        recipeName,
                        manifest,
                        validationSet);
                RewriteDependencyParameters(pipeline, replacementByOriginal);
                VisionPipelineStorage.Save(recipeName, pipeline);
                VisionPipelineStorage.SaveActivePipelineName(
                    recipeName,
                    pipeline.Name);
                if (!VisionPipelineStorage.TryLoadFromFile(
                        RecipeWorkspaceService.GetVisionPipelinePath(
                            recipeName,
                            pipeline.Name),
                        out VisionPipeline reloaded,
                        out string loadMessage)
                    || reloaded == null)
                {
                    throw new InvalidDataException(
                        "Working-copy Pipeline reload failed: " + loadMessage);
                }

                return new QualifiedRecipeWorkingCopyResult
                {
                    Success = true,
                    RecipeName = recipeName,
                    PipelineName = pipeline.Name ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                if (created && Directory.Exists(targetDirectory))
                {
                    RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
                }

                return Failure(ex.GetBaseException().Message);
            }
        }

        private static Dictionary<string, string> CopyDependencies(
            string snapshotDirectory,
            string recipeName,
            QualifiedRecipeSnapshotManifest manifest,
            QualifiedRecipeValidationSetSnapshot validationSet)
        {
            Dictionary<string, string> replacements =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string patternDirectory =
                RecipeWorkspaceService.GetPatternDirectory(recipeName);
            foreach (QualifiedRecipeArchivedFile archived in
                manifest.Dependencies ?? new List<QualifiedRecipeArchivedFile>())
            {
                string source = ResolveVerifiedArchivePath(
                    snapshotDirectory,
                    archived.ArchivePath);
                if (!File.Exists(source))
                {
                    throw new FileNotFoundException(
                        "Snapshot dependency is missing.",
                        archived.ArchivePath);
                }

                QualifiedRecipeDependencySource definition =
                    (validationSet.Dependencies
                        ?? new List<QualifiedRecipeDependencySource>())
                    .FirstOrDefault(item =>
                        item != null
                        && (string.Equals(
                                item.SourcePath,
                                archived.LogicalPath,
                                StringComparison.OrdinalIgnoreCase)
                            || string.Equals(
                                item.LogicalPath,
                                archived.LogicalPath,
                                StringComparison.OrdinalIgnoreCase)));
                string originalPath = definition?.SourcePath
                    ?? archived.LogicalPath
                    ?? string.Empty;
                string fileName = SafeFileName(Path.GetFileName(originalPath));
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = SafeFileName(
                        Path.GetFileName(archived.ArchivePath));
                }

                string target = CreateUniquePath(patternDirectory, fileName);
                File.Copy(source, target, overwrite: false);
                if (!string.IsNullOrWhiteSpace(originalPath))
                {
                    replacements[originalPath] = target;
                }

                if (!string.IsNullOrWhiteSpace(archived.LogicalPath))
                {
                    replacements[archived.LogicalPath] = target;
                }
            }

            return replacements;
        }

        private static void RewriteDependencyParameters(
            VisionPipeline pipeline,
            IReadOnlyDictionary<string, string> replacementByOriginal)
        {
            foreach (VisionPipelineStep step in
                pipeline?.Steps?.Where(step => step != null)
                ?? Enumerable.Empty<VisionPipelineStep>())
            {
                foreach (string key in step.Parameters.Keys.ToList())
                {
                    string current = step.Parameters[key] ?? string.Empty;
                    if (replacementByOriginal.TryGetValue(
                            current,
                            out string replacement))
                    {
                        step.Parameters[key] = replacement;
                    }
                }
            }
        }

        private static string ResolveVerifiedArchivePath(
            string snapshotDirectory,
            string archivePath)
        {
            string root = Path.GetFullPath(snapshotDirectory);
            string rootWithSeparator =
                root.TrimEnd(Path.DirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            string path = Path.GetFullPath(
                Path.Combine(
                    root,
                    (archivePath ?? string.Empty)
                    .Replace('/', Path.DirectorySeparatorChar)));
            if (!path.StartsWith(
                    rootWithSeparator,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException(
                    "Snapshot archive path leaves the snapshot directory.");
            }

            return path;
        }

        private static string CreateUniquePath(
            string directory,
            string fileName)
        {
            string safeName = RecipeWorkspaceService.NormalizeStoragePathSegment(
                fileName,
                "dependency.bin",
                "Qualified Recipe dependency file name");
            string baseName = Path.GetFileNameWithoutExtension(safeName);
            string extension = Path.GetExtension(safeName);
            string candidate = RecipeWorkspaceService.GetContainedStoragePath(
                directory,
                safeName,
                "Qualified Recipe dependency path");
            int suffix = 2;
            while (File.Exists(candidate))
            {
                candidate = RecipeWorkspaceService.GetContainedStoragePath(
                    directory,
                    baseName + "_" + suffix++ + extension,
                    "Qualified Recipe dependency path");
            }

            return candidate;
        }

        private static string SafeFileName(string value)
        {
            string name = value?.Trim() ?? string.Empty;
            char[] invalid = Path.GetInvalidFileNameChars();
            return new string(
                name.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
        }

        private static QualifiedRecipeWorkingCopyResult Failure(string error)
        {
            return new QualifiedRecipeWorkingCopyResult
            {
                Success = false,
                Error = error ?? string.Empty
            };
        }
    }
}
