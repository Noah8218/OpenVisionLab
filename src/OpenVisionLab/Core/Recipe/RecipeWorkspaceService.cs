using OpenVisionLab.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace OpenVisionLab
{
    internal static class RecipeWorkspaceService
    {
        private const string RecipeRoot = "RECIPE";
        private static readonly HashSet<string> ReservedDeviceNames =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "AUX",
                "CLOCK$",
                "CON",
                "NUL",
                "PRN"
            };

        public static void EnsureRoot()
        {
            Directory.CreateDirectory(GetRecipeRootDirectory());
        }

        public static void EnsureVisionWorkspace(string recipeName)
        {
            string recipeDirectory = GetRecipeDirectory(recipeName);
            EnsureRoot();
            Directory.CreateDirectory(CombineStoragePath(recipeDirectory, "VISION"));
            Directory.CreateDirectory(CombineStoragePath(recipeDirectory, "GRAPH"));
            Directory.CreateDirectory(CombineStoragePath(recipeDirectory, "PATTERN"));
        }

        public static string[] GetRecipeNames()
        {
            EnsureRoot();
            string root = GetRecipeRootDirectory();
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

        public static string ResolveExistingRecipeName(string recipeName)
        {
            string requested = recipeName?.Trim();
            if (!IsValidRecipeName(requested))
            {
                return "Default";
            }

            return GetRecipeNames().FirstOrDefault(name => string.Equals(
                       name,
                       requested,
                       StringComparison.OrdinalIgnoreCase))
                ?? "Default";
        }

        public static bool IsValidRecipeName(string recipeName)
        {
            return TryNormalizeStoragePathSegment(
                recipeName,
                string.Empty,
                "Recipe name",
                out _,
                out _);
        }

        public static bool RenameVisionWorkspace(string oldRecipeName, string newRecipeName)
        {
            string oldName = oldRecipeName?.Trim();
            string newName = newRecipeName?.Trim();
            if (!IsValidRecipeName(oldName)
                || !IsValidRecipeName(newName)
                || string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            EnsureRoot();
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
            string normalized = recipeName?.Trim();
            if (!IsValidRecipeName(normalized))
            {
                return false;
            }

            EnsureRoot();
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
            string sourceName = sourceRecipeName?.Trim();
            string targetName = targetRecipeName?.Trim();
            if (!IsValidRecipeName(sourceName)
                || !IsValidRecipeName(targetName)
                || string.Equals(sourceName, targetName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            EnsureRoot();
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

            return Directory.EnumerateFiles(visionDirectory, "*.xml")
                .Where(path => !string.Equals(
                    Path.GetFileNameWithoutExtension(path),
                    "pipeline.active",
                    StringComparison.OrdinalIgnoreCase))
                .Where(IsVisionPipelineDocument)
                .Select(Path.GetFileNameWithoutExtension)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static bool IsVisionPipelineDocument(string path)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    IgnoreComments = true,
                    IgnoreWhitespace = true,
                    XmlResolver = null
                };
                using XmlReader reader = XmlReader.Create(path, settings);
                reader.MoveToContent();
                return string.Equals(reader.LocalName, "VisionPipeline", StringComparison.Ordinal)
                    && string.IsNullOrEmpty(reader.NamespaceURI);
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (XmlException)
            {
                return false;
            }
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
            string path = GetStorageFilePath(
                GetRecipeDirectory(recipeName),
                fileName,
                "Recipe file name");
            EnsureRecipeDirectory(recipeName);
            return path;
        }

        public static string GetVisionConfigPath(string recipeName, string configName)
        {
            string path = GetStorageFilePath(
                GetVisionDirectory(recipeName),
                configName,
                "Vision config name");
            EnsureVisionWorkspace(recipeName);
            return path;
        }

        public static string GetAccountConfigPath(string configName)
        {
            string accountDirectory = CombineStoragePath(
                Path.GetFullPath(AppPathService.DataRootDirectory),
                "CONFIG",
                "ACCOUNT");
            string path = GetStorageFilePath(accountDirectory, configName, "Account config name");
            Directory.CreateDirectory(accountDirectory);
            return path;
        }

        public static string GetSystemConfigPath(string configName)
        {
            return GetStorageFilePath(
                Path.GetFullPath(AppPathService.DataRootDirectory),
                configName,
                "System config name");
        }

        public static string GetVisionDataPath(string recipeName)
        {
            string path = GetStorageFilePath(
                GetRecipeDirectory(recipeName),
                "VISION",
                "Vision data file name");
            EnsureVisionWorkspace(recipeName);
            return path;
        }

        public static string GetVisionPipelinePath(string recipeName, string pipelineName)
        {
            string path = GetStorageFilePath(
                GetVisionDirectory(recipeName),
                pipelineName,
                "Pipeline name",
                fallback: "Pipeline");
            EnsureVisionWorkspace(recipeName);
            return path;
        }

        public static string GetVisionPipelineImageDirectory(string recipeName, string pipelineName)
        {
            string safeName = NormalizeStoragePathSegment(
                pipelineName,
                "Pipeline",
                "Pipeline name");
            string directory = CombineStoragePath(
                GetVisionDirectory(recipeName),
                "PipelineImages",
                safeName);
            EnsureVisionWorkspace(recipeName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string GetVisionPipelineRunDirectory(string recipeName, string pipelineName, string runName)
        {
            string safePipelineName = NormalizeStoragePathSegment(
                pipelineName,
                "Pipeline",
                "Pipeline name");
            string safeRunName = NormalizeStoragePathSegment(
                runName,
                "Run",
                "Run name");
            string directory = CombineStoragePath(
                GetVisionDirectory(recipeName),
                "PipelineRuns",
                safePipelineName,
                safeRunName);
            EnsureVisionWorkspace(recipeName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string GetVisionPipelineRunRootDirectory(string recipeName, string pipelineName)
        {
            string safePipelineName = NormalizeStoragePathSegment(
                pipelineName,
                "Pipeline",
                "Pipeline name");
            string directory = CombineStoragePath(
                GetVisionDirectory(recipeName),
                "PipelineRuns",
                safePipelineName);
            EnsureVisionWorkspace(recipeName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string GetVisionPipelineSampleSetRootDirectory(string recipeName, string pipelineName)
        {
            string safePipelineName = NormalizeStoragePathSegment(
                pipelineName,
                "Pipeline",
                "Pipeline name");
            string directory = CombineStoragePath(
                GetVisionDirectory(recipeName),
                "PipelineSamples",
                safePipelineName);
            EnsureVisionWorkspace(recipeName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string GetVisionPipelineSampleSetDirectory(string recipeName, string pipelineName, string sampleSetName)
        {
            string safeSampleSetName = NormalizeStoragePathSegment(
                sampleSetName,
                "Sample",
                "Sample-set name");
            string directory = CombineStoragePath(
                GetVisionPipelineSampleSetRootDirectory(recipeName, pipelineName),
                safeSampleSetName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string GetVisionPipelineBatchRunRootDirectory(string recipeName, string pipelineName)
        {
            string safePipelineName = NormalizeStoragePathSegment(
                pipelineName,
                "Pipeline",
                "Pipeline name");
            string directory = CombineStoragePath(
                GetVisionDirectory(recipeName),
                "PipelineBatchRuns",
                safePipelineName);
            EnsureVisionWorkspace(recipeName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string GetVisionPipelineBatchRunDirectory(string recipeName, string pipelineName, string batchName)
        {
            string safeBatchName = NormalizeStoragePathSegment(
                batchName,
                "Batch",
                "Batch name");
            string directory = CombineStoragePath(
                GetVisionPipelineBatchRunRootDirectory(recipeName, pipelineName),
                safeBatchName);
            Directory.CreateDirectory(directory);
            return directory;
        }

        internal static string GetRecipeDirectoryPath(string recipeName)
        {
            return GetRecipeDirectory(recipeName);
        }

        internal static string GetContainedStoragePath(
            string intendedRoot,
            string relativePath,
            string pathDescription)
        {
            string root = Path.GetFullPath(intendedRoot ?? string.Empty);
            string relative = relativePath ?? string.Empty;
            if (string.IsNullOrWhiteSpace(relative) || Path.IsPathRooted(relative))
            {
                throw new ArgumentException(
                    $"{pathDescription} must be a relative storage path.",
                    nameof(relativePath));
            }

            string[] segments = relative.Split(
                new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
            {
                throw new ArgumentException(
                    $"{pathDescription} must contain at least one path segment.",
                    nameof(relativePath));
            }

            string[] normalizedSegments = segments
                .Select(segment => NormalizeStoragePathSegment(
                    segment,
                    string.Empty,
                    pathDescription))
                .ToArray();
            return CombineStoragePath(root, normalizedSegments);
        }

        internal static string NormalizeStoragePathSegment(
            string value,
            string fallback,
            string segmentDescription)
        {
            if (!TryNormalizeStoragePathSegment(
                    value,
                    fallback,
                    segmentDescription,
                    out string normalized,
                    out string error))
            {
                throw new ArgumentException(error, segmentDescription);
            }

            return normalized;
        }

        private static string GetRecipeDirectory(string recipeName)
        {
            string root = GetRecipeRootDirectory();
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return root;
            }

            string normalized = NormalizeStoragePathSegment(
                recipeName,
                string.Empty,
                "Recipe name");
            return CombineStoragePath(root, normalized);
        }

        private static string GetRecipeRootDirectory()
        {
            return Path.GetFullPath(
                Path.Combine(AppPathService.DataRootDirectory, RecipeRoot));
        }

        private static bool IsSafeRecipeChildPath(string path)
        {
            string root = GetRecipeRootDirectory();
            string target;
            try
            {
                target = Path.GetFullPath(path ?? string.Empty);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException)
            {
                return false;
            }

            return IsSameOrChildPath(root, target)
                && !string.Equals(target, root, StringComparison.OrdinalIgnoreCase);
        }

        private static void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            Directory.CreateDirectory(targetDirectory);
            foreach (string directory in Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                string targetSubDirectory = GetContainedStoragePath(
                    targetDirectory,
                    Path.GetRelativePath(sourceDirectory, directory),
                    "Recipe duplicate directory");
                Directory.CreateDirectory(targetSubDirectory);
            }

            foreach (string file in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                string targetFile = GetContainedStoragePath(
                    targetDirectory,
                    Path.GetRelativePath(sourceDirectory, file),
                    "Recipe duplicate file");
                Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                File.Copy(file, targetFile, overwrite: false);
            }
        }

        private static string GetVisionDirectory(string recipeName)
        {
            return CombineStoragePath(GetRecipeDirectory(recipeName), "VISION");
        }

        private static void EnsureRecipeDirectory(string recipeName)
        {
            string recipeDirectory = GetRecipeDirectory(recipeName);
            EnsureRoot();
            Directory.CreateDirectory(recipeDirectory);
        }

        private static string EnsureXmlExtension(string fileName)
        {
            return Path.GetExtension(fileName).Equals(".xml", System.StringComparison.OrdinalIgnoreCase)
                ? fileName
                : $"{fileName}.xml";
        }

        private static string GetStorageFilePath(
            string root,
            string fileName,
            string fileDescription,
            string fallback = "")
        {
            string normalized = NormalizeStoragePathSegment(
                fileName,
                fallback,
                fileDescription);
            return CombineStoragePath(root, EnsureXmlExtension(normalized));
        }

        private static string CombineRecipePath(string recipeName, string childDirectory)
        {
            string recipePath = CombineStoragePath(
                GetRecipeDirectory(recipeName),
                childDirectory);
            Directory.CreateDirectory(recipePath);
            return recipePath;
        }

        internal static bool TryNormalizeStoragePathSegment(
            string value,
            string fallback,
            string segmentDescription,
            out string normalized,
            out string error)
        {
            normalized = string.IsNullOrWhiteSpace(value)
                ? fallback?.Trim() ?? string.Empty
                : value.Trim();
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(normalized))
            {
                error = $"{segmentDescription} is required for Recipe/Pipeline storage.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(value)
                && (value[value.Length - 1] == ' ' || value[value.Length - 1] == '.'))
            {
                error = $"{segmentDescription} cannot end with a space or period.";
                return false;
            }

            if (string.Equals(normalized, ".", StringComparison.Ordinal)
                || string.Equals(normalized, "..", StringComparison.Ordinal))
            {
                error = $"{segmentDescription} cannot be '.' or '..'.";
                return false;
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (normalized.Any(ch => char.IsControl(ch)
                || invalidChars.Contains(ch)
                || ch == '\\'
                || ch == '/'))
            {
                error = $"{segmentDescription} contains a path separator, control character, or invalid filename character.";
                return false;
            }

            if (IsReservedDeviceName(normalized))
            {
                error = $"{segmentDescription} cannot use a Windows reserved device name.";
                return false;
            }

            return true;
        }

        private static bool IsReservedDeviceName(string value)
        {
            string baseName = value;
            int extensionIndex = baseName.IndexOf('.');
            if (extensionIndex >= 0)
            {
                baseName = baseName.Substring(0, extensionIndex);
            }

            if (ReservedDeviceNames.Contains(baseName))
            {
                return true;
            }

            if (baseName.Length != 4
                || (!baseName.StartsWith("COM", StringComparison.OrdinalIgnoreCase)
                    && !baseName.StartsWith("LPT", StringComparison.OrdinalIgnoreCase))
                || !int.TryParse(baseName.Substring(3), out int deviceNumber))
            {
                return false;
            }

            return deviceNumber >= 1 && deviceNumber <= 9;
        }

        private static string CombineStoragePath(string root, params string[] segments)
        {
            string fullRoot = Path.GetFullPath(root ?? string.Empty);
            string target = segments == null || segments.Length == 0
                ? fullRoot
                : Path.GetFullPath(
                    Path.Combine(
                        new[] { fullRoot }
                            .Concat(segments)
                            .ToArray()));

            if (!IsSameOrChildPath(fullRoot, target)
                || (segments != null
                    && segments.Length > 0
                    && string.Equals(fullRoot, target, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "Recipe/Pipeline storage path escaped its intended root: " + target);
            }

            return target;
        }

        private static bool IsSameOrChildPath(string root, string path)
        {
            string fullRoot = Path.GetFullPath(root ?? string.Empty).TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);
            string fullPath = Path.GetFullPath(path ?? string.Empty).TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);
            if (string.Equals(fullRoot, fullPath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return fullPath.StartsWith(
                fullRoot + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
