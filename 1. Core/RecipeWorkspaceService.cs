using Lib.Common;
using System;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal static class RecipeWorkspaceService
    {
        private const string RecipeRoot = "RECIPE";

        public static void EnsureRoot()
        {
            AppUtil.InitDirectory(RecipeRoot);
        }

        public static void EnsureVisionWorkspace(string recipeName)
        {
            EnsureRoot();
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return;
            }

            AppUtil.InitDirectory($@"{RecipeRoot}\{recipeName}\VISION");
            AppUtil.InitDirectory($@"{RecipeRoot}\{recipeName}\GRAPH");
            AppUtil.InitDirectory($@"{RecipeRoot}\{recipeName}\PATTERN");
        }

        public static string[] GetRecipeNames()
        {
            EnsureRoot();
            string root = Path.Combine(AppPathService.StartupPath, RecipeRoot);
            if (!Directory.Exists(root))
            {
                return new[] { "Default" };
            }

            string[] names = Directory.GetDirectories(root)
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .OrderBy(name => name, System.StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return names.Length == 0 ? new[] { "Default" } : names;
        }

        public static bool IsValidRecipeName(string recipeName)
        {
            string normalized = recipeName?.Trim();
            if (string.IsNullOrWhiteSpace(normalized)
                || string.Equals(normalized, ".", StringComparison.Ordinal)
                || string.Equals(normalized, "..", StringComparison.Ordinal))
            {
                return false;
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            return !normalized.Any(ch => invalidChars.Contains(ch));
        }

        public static bool RenameVisionWorkspace(string oldRecipeName, string newRecipeName)
        {
            EnsureRoot();
            string oldName = oldRecipeName?.Trim();
            string newName = newRecipeName?.Trim();
            if (!IsValidRecipeName(oldName)
                || !IsValidRecipeName(newName)
                || string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string oldPath = GetRecipeDirectory(oldName);
            string newPath = GetRecipeDirectory(newName);
            if (!IsSafeRecipeChildPath(oldPath)
                || !IsSafeRecipeChildPath(newPath)
                || !Directory.Exists(oldPath)
                || Directory.Exists(newPath))
            {
                return false;
            }

            Directory.Move(oldPath, newPath);
            return true;
        }

        public static bool DeleteVisionWorkspace(string recipeName)
        {
            EnsureRoot();
            string normalized = recipeName?.Trim();
            if (!IsValidRecipeName(normalized))
            {
                return false;
            }

            string path = GetRecipeDirectory(normalized);
            if (!IsSafeRecipeChildPath(path) || !Directory.Exists(path))
            {
                return false;
            }

            Directory.Delete(path, recursive: true);
            return true;
        }

        public static bool DuplicateVisionWorkspace(string sourceRecipeName, string targetRecipeName)
        {
            EnsureRoot();
            string sourceName = sourceRecipeName?.Trim();
            string targetName = targetRecipeName?.Trim();
            if (!IsValidRecipeName(sourceName)
                || !IsValidRecipeName(targetName)
                || string.Equals(sourceName, targetName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string sourcePath = GetRecipeDirectory(sourceName);
            string targetPath = GetRecipeDirectory(targetName);
            if (!IsSafeRecipeChildPath(sourcePath)
                || !IsSafeRecipeChildPath(targetPath)
                || !Directory.Exists(sourcePath)
                || Directory.Exists(targetPath))
            {
                return false;
            }

            CopyDirectory(sourcePath, targetPath);
            return true;
        }

        public static string GetRecipeWorkspaceDirectory(string recipeName)
        {
            EnsureVisionWorkspace(recipeName);
            return GetRecipeDirectory(recipeName);
        }

        public static string[] GetVisionPipelineNames(string recipeName)
        {
            EnsureVisionWorkspace(recipeName);
            string visionDirectory = GetVisionDirectory(recipeName);
            if (!Directory.Exists(visionDirectory))
            {
                return Array.Empty<string>();
            }

            return Directory.GetFiles(visionDirectory, "*.xml")
                .Select(Path.GetFileNameWithoutExtension)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Where(name => !string.Equals(name, "pipeline.active", StringComparison.OrdinalIgnoreCase))
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public static DateTime? GetRecipeLastWriteTime(string recipeName)
        {
            string directory = GetRecipeWorkspaceDirectory(recipeName);
            if (!Directory.Exists(directory))
            {
                return null;
            }

            DateTime latest = Directory.GetLastWriteTime(directory);
            foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
            {
                DateTime modified = File.GetLastWriteTime(file);
                if (modified > latest)
                {
                    latest = modified;
                }
            }

            return latest;
        }

        public static string GetPatternDirectory(string recipeName)
        {
            EnsureVisionWorkspace(recipeName);
            return CombineRecipePath(recipeName, "PATTERN");
        }

        public static string GetTemplateDirectory(string recipeName)
        {
            EnsureVisionWorkspace(recipeName);
            return CombineRecipePath(recipeName, "Template");
        }

        public static string GetRecipeFilePath(string recipeName, string fileName)
        {
            EnsureRecipeDirectory(recipeName);
            return Path.Combine(GetRecipeDirectory(recipeName), EnsureXmlExtension(fileName));
        }

        public static string GetVisionConfigPath(string recipeName, string configName)
        {
            EnsureVisionWorkspace(recipeName);
            return Path.Combine(GetVisionDirectory(recipeName), EnsureXmlExtension(configName));
        }

        public static string GetAccountConfigPath(string configName)
        {
            string accountDirectory = Path.Combine(AppPathService.StartupPath, "CONFIG", "ACCOUNT");
            Directory.CreateDirectory(accountDirectory);
            return Path.Combine(accountDirectory, EnsureXmlExtension(configName));
        }

        public static string GetSystemConfigPath(string configName)
        {
            return Path.Combine(AppPathService.StartupPath, EnsureXmlExtension(configName));
        }

        public static string GetVisionDataPath(string recipeName)
        {
            EnsureVisionWorkspace(recipeName);

            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return Path.Combine(AppPathService.StartupPath, RecipeRoot, "VISION.xml");
            }

            return Path.Combine(AppPathService.StartupPath, RecipeRoot, recipeName, "VISION.xml");
        }

        public static string GetVisionPipelinePath(string recipeName, string pipelineName)
        {
            EnsureVisionWorkspace(recipeName);

            string safeName = string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName;
            return Path.Combine(GetVisionDirectory(recipeName), EnsureXmlExtension(safeName));
        }

        public static string GetVisionPipelineImageDirectory(string recipeName, string pipelineName)
        {
            EnsureVisionWorkspace(recipeName);

            string safeName = string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName;
            string directory = Path.Combine(GetVisionDirectory(recipeName), "PipelineImages", safeName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string GetVisionPipelineRunDirectory(string recipeName, string pipelineName, string runName)
        {
            EnsureVisionWorkspace(recipeName);

            string safePipelineName = SanitizePathSegment(string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName);
            string safeRunName = SanitizePathSegment(string.IsNullOrWhiteSpace(runName) ? "Run" : runName);
            string directory = Path.Combine(GetVisionDirectory(recipeName), "PipelineRuns", safePipelineName, safeRunName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string GetVisionPipelineRunRootDirectory(string recipeName, string pipelineName)
        {
            EnsureVisionWorkspace(recipeName);

            string safePipelineName = SanitizePathSegment(string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName);
            string directory = Path.Combine(GetVisionDirectory(recipeName), "PipelineRuns", safePipelineName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string GetVisionPipelineSampleSetRootDirectory(string recipeName, string pipelineName)
        {
            EnsureVisionWorkspace(recipeName);

            string safePipelineName = SanitizePathSegment(string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName);
            string directory = Path.Combine(GetVisionDirectory(recipeName), "PipelineSamples", safePipelineName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string GetVisionPipelineSampleSetDirectory(string recipeName, string pipelineName, string sampleSetName)
        {
            string safeSampleSetName = SanitizePathSegment(string.IsNullOrWhiteSpace(sampleSetName) ? "Sample" : sampleSetName);
            string directory = Path.Combine(GetVisionPipelineSampleSetRootDirectory(recipeName, pipelineName), safeSampleSetName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string GetVisionPipelineBatchRunRootDirectory(string recipeName, string pipelineName)
        {
            EnsureVisionWorkspace(recipeName);

            string safePipelineName = SanitizePathSegment(string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName);
            string directory = Path.Combine(GetVisionDirectory(recipeName), "PipelineBatchRuns", safePipelineName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string GetVisionPipelineBatchRunDirectory(string recipeName, string pipelineName, string batchName)
        {
            string safeBatchName = SanitizePathSegment(string.IsNullOrWhiteSpace(batchName) ? "Batch" : batchName);
            string directory = Path.Combine(GetVisionPipelineBatchRunRootDirectory(recipeName, pipelineName), safeBatchName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        private static string GetRecipeDirectory(string recipeName)
        {
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return Path.Combine(AppPathService.StartupPath, RecipeRoot);
            }

            return Path.Combine(AppPathService.StartupPath, RecipeRoot, recipeName);
        }

        private static bool IsSafeRecipeChildPath(string path)
        {
            string root = Path.GetFullPath(Path.Combine(AppPathService.StartupPath, RecipeRoot));
            string target = Path.GetFullPath(path ?? string.Empty);
            string rootWithSeparator = root.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                ? root
                : root + Path.DirectorySeparatorChar;
            return target.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(target, root, StringComparison.OrdinalIgnoreCase);
        }

        private static void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            Directory.CreateDirectory(targetDirectory);
            foreach (string directory in Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                string targetSubDirectory = Path.Combine(targetDirectory, Path.GetRelativePath(sourceDirectory, directory));
                Directory.CreateDirectory(targetSubDirectory);
            }

            foreach (string file in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                string targetFile = Path.Combine(targetDirectory, Path.GetRelativePath(sourceDirectory, file));
                Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                File.Copy(file, targetFile, overwrite: false);
            }
        }

        private static string GetVisionDirectory(string recipeName)
        {
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return Path.Combine(AppPathService.StartupPath, RecipeRoot, "VISION");
            }

            return Path.Combine(AppPathService.StartupPath, RecipeRoot, recipeName, "VISION");
        }

        private static void EnsureRecipeDirectory(string recipeName)
        {
            EnsureRoot();
            Directory.CreateDirectory(GetRecipeDirectory(recipeName));
        }

        private static string EnsureXmlExtension(string fileName)
        {
            return Path.GetExtension(fileName).Equals(".xml", System.StringComparison.OrdinalIgnoreCase)
                ? fileName
                : $"{fileName}.xml";
        }

        private static string CombineRecipePath(string recipeName, string childDirectory)
        {
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                AppUtil.InitDirectory($@"{RecipeRoot}\{childDirectory}");
                return Path.Combine(AppPathService.StartupPath, RecipeRoot, childDirectory);
            }

            AppUtil.InitDirectory($@"{RecipeRoot}\{recipeName}\{childDirectory}");
            return Path.Combine(AppPathService.StartupPath, RecipeRoot, recipeName, childDirectory);
        }

        private static string SanitizePathSegment(string value)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = new string((value ?? string.Empty)
                .Select(ch => invalidChars.Contains(ch) ? '_' : ch)
                .ToArray());

            return string.IsNullOrWhiteSpace(sanitized) ? "Item" : sanitized;
        }
    }
}
