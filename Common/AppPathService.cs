using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace OpenVisionLab
{
    public static class AppPathService
    {
        public const string DataRootEnvironmentVariable = "OPENVISIONLAB_DATA_ROOT";
        public const string ResolvedDataRootEnvironmentVariable = "OPENVISIONLAB_DATA_ROOT_RESOLVED";
        public const string LogRootEnvironmentVariable = "OPENVISIONLAB_LOG_ROOT";

        private const string MigrationReportFileName = "data-root-migration-v1.txt";
        private static readonly Lazy<RuntimeDataPathState> RuntimeState =
            new Lazy<RuntimeDataPathState>(
                CreateRuntimeState,
                System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        public static string StartupPath => AppContext.BaseDirectory.TrimEnd(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar);

        public static string InstallationRootDirectory => StartupPath;

        public static string DataRootDirectory => RuntimeState.Value.DataRootDirectory;

        public static string ConfigRootDirectory => EnsureDirectory("CONFIG");

        public static string CaptureDirectory => EnsureDirectory("CAPTURE");

        public static string RecipeRootDirectory => EnsureDirectory("RECIPE");

        public static string QualifiedRecipeRootDirectory =>
            EnsureDirectory("QUALIFIED_RECIPE");

        public static string LogRootDirectory => EnsureDirectory("Log");

        public static string TestDirectory => EnsureDirectory("TEST");

        public static string MigrationReportPath =>
            RuntimeState.Value.MigrationReportPath;

        public static string MigrationNotice =>
            RuntimeState.Value.MigrationNotice;

        public static void Initialize()
        {
            _ = RuntimeState.Value;
        }

        public static string Combine(params string[] paths)
        {
            string[] parts = new string[paths.Length + 1];
            parts[0] = DataRootDirectory;
            Array.Copy(paths, 0, parts, 1, paths.Length);
            return Path.Combine(parts);
        }

        public static string CombineInstallation(params string[] paths)
        {
            string[] parts = new string[paths.Length + 1];
            parts[0] = InstallationRootDirectory;
            Array.Copy(paths, 0, parts, 1, paths.Length);
            return Path.Combine(parts);
        }

        public static string EnsureDirectory(params string[] paths)
        {
            string directory = Combine(paths);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string ResolveExistingDataOrInstallationPath(string relativePath)
        {
            string candidate = (relativePath ?? string.Empty).Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(candidate))
            {
                return string.Empty;
            }

            if (Path.IsPathRooted(candidate))
            {
                return Path.GetFullPath(candidate);
            }

            string dataPath = Path.GetFullPath(Path.Combine(DataRootDirectory, candidate));
            if (File.Exists(dataPath) || Directory.Exists(dataPath))
            {
                return dataPath;
            }

            string installationPath = Path.GetFullPath(
                Path.Combine(InstallationRootDirectory, candidate));
            if (File.Exists(installationPath) || Directory.Exists(installationPath))
            {
                return installationPath;
            }

            return dataPath;
        }

        public static string GetDataRelativePath(string path)
        {
            string fullPath = Path.GetFullPath(path ?? string.Empty);
            if (!IsSameOrChildPath(DataRootDirectory, fullPath))
            {
                return fullPath;
            }

            return Path.GetRelativePath(DataRootDirectory, fullPath);
        }

        public static bool IsPathUnderInstallationRoot(string path)
        {
            return IsSameOrChildPath(InstallationRootDirectory, path);
        }

        public static string GetCaptureFilePath(string title, DateTime timestamp)
        {
            return Path.Combine(CaptureDirectory, $"{title}_{timestamp:yyyyMMdd_HHmmss}.jpeg");
        }

        public static string GetTestConfigPath(string configName)
        {
            return Path.Combine(TestDirectory, EnsureXmlExtension(configName));
        }

        private static RuntimeDataPathState CreateRuntimeState()
        {
            string dataRoot = ResolveDataRoot();
            Directory.CreateDirectory(dataRoot);

            string migrationReportPath = Path.Combine(
                dataRoot,
                MigrationReportFileName);
            string migrationNotice = string.Empty;
            if (!PathsEqual(InstallationRootDirectory, dataRoot))
            {
                migrationNotice = MigrateLegacyRuntimeData(
                    InstallationRootDirectory,
                    dataRoot,
                    migrationReportPath);
            }
            if (PathsEqual(dataRoot, GetDefaultReleaseDataRoot()))
            {
                string layoutNotice = MigrateLegacyLocalDataLayout(dataRoot);
                if (!string.IsNullOrWhiteSpace(layoutNotice))
                {
                    migrationNotice = string.IsNullOrWhiteSpace(migrationNotice)
                        ? layoutNotice
                        : migrationNotice + " " + layoutNotice;
                }
            }

            foreach (string directoryName in new[] { "CONFIG", "RECIPE", "Log" })
            {
                Directory.CreateDirectory(Path.Combine(dataRoot, directoryName));
            }

            Environment.SetEnvironmentVariable(
                ResolvedDataRootEnvironmentVariable,
                dataRoot,
                EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable(
                LogRootEnvironmentVariable,
                Path.Combine(dataRoot, "Log"),
                EnvironmentVariableTarget.Process);

            return new RuntimeDataPathState(
                dataRoot,
                migrationReportPath,
                migrationNotice);
        }

        private static string ResolveDataRoot()
        {
            string requested = Environment.GetEnvironmentVariable(
                DataRootEnvironmentVariable);
            if (!string.IsNullOrWhiteSpace(requested))
            {
                string expanded = Environment.ExpandEnvironmentVariables(
                    requested.Trim().Trim('"'));
                if (!Path.IsPathRooted(expanded))
                {
                    throw new InvalidOperationException(
                        DataRootEnvironmentVariable
                        + " must contain an absolute path.");
                }

                string resolved = Path.GetFullPath(expanded);
#if !DEBUG
                if (IsSameOrChildPath(InstallationRootDirectory, resolved))
                {
                    throw new InvalidOperationException(
                        DataRootEnvironmentVariable
                        + " must not point inside the Release installation directory.");
                }
#endif
                return resolved;
            }

#if DEBUG
            return InstallationRootDirectory;
#else
            return GetDefaultReleaseDataRoot();
#endif
        }

        private static string GetDefaultReleaseDataRoot()
        {
            string localApplicationData = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrWhiteSpace(localApplicationData))
            {
                throw new InvalidOperationException(
                    "Windows LocalApplicationData could not be resolved.");
            }

            return Path.GetFullPath(
                Path.Combine(localApplicationData, "OpenVisionLab"));
        }

        private static string MigrateLegacyLocalDataLayout(string dataRoot)
        {
            string reportPath = Path.Combine(
                dataRoot,
                "data-layout-migration-v1.txt");
            if (File.Exists(reportPath))
            {
                return string.Empty;
            }

            List<string> report = new List<string>
            {
                "OpenVisionLab local data-layout migration v1",
                "StartedUtc=" + DateTime.UtcNow.ToString("O"),
                "DataRoot=" + dataRoot,
                "Policy=copy missing files; retain source; never overwrite target"
            };
            int copiedFiles = 0;
            int existingSameFiles = 0;
            int conflictFiles = 0;
            List<string> failures = new List<string>();
            Tuple<string, string>[] fileMappings =
            {
                Tuple.Create(
                    "recent-native-tool.txt",
                    Path.Combine("CONFIG", "UI", "recent-native-tool.txt")),
                Tuple.Create(
                    "image_compare_last_directory.txt",
                    Path.Combine("CONFIG", "image_compare_last_directory.txt")),
                Tuple.Create(
                    Path.Combine("Logs", "property-grid-editors.log"),
                    Path.Combine("Log", "Diagnostics", "property-grid-editors.log"))
            };
            foreach (Tuple<string, string> mapping in fileMappings)
            {
                string sourceFile = Path.Combine(dataRoot, mapping.Item1);
                if (!File.Exists(sourceFile))
                {
                    continue;
                }

                CopyLegacyFile(
                    sourceFile,
                    Path.Combine(dataRoot, mapping.Item2),
                    mapping.Item1 + " -> " + mapping.Item2,
                    report,
                    failures,
                    ref copiedFiles,
                    ref existingSameFiles,
                    ref conflictFiles);
            }

            string legacyLearnRoot = Path.Combine(dataRoot, "LearnHtml");
            if (Directory.Exists(legacyLearnRoot))
            {
                foreach (string sourceFile in GetLegacyFiles(
                    legacyLearnRoot,
                    failures,
                    "LearnHtml"))
                {
                    string relativePath = Path.GetRelativePath(
                        legacyLearnRoot,
                        sourceFile);
                    string targetRelativePath = Path.Combine(
                        "CACHE",
                        "LearnHtml",
                        relativePath);
                    CopyLegacyFile(
                        sourceFile,
                        Path.Combine(dataRoot, targetRelativePath),
                        "LearnHtml\\" + relativePath + " -> " + targetRelativePath,
                        report,
                        failures,
                        ref copiedFiles,
                        ref existingSameFiles,
                        ref conflictFiles);
                }
            }

            report.Add("CopiedFiles=" + copiedFiles);
            report.Add("ExistingSameFiles=" + existingSameFiles);
            report.Add("ConflictFilesRetainedAtSource=" + conflictFiles);
            report.Add("LegacySourceDeleted=false");
            report.Add("CompletedUtc=" + DateTime.UtcNow.ToString("O"));
            if (failures.Count > 0)
            {
                report.Add("Status=Incomplete");
                report.AddRange(failures);
                string incompletePath = reportPath + ".incomplete";
                File.WriteAllLines(incompletePath, report);
                throw new IOException(
                    "OpenVisionLab could not safely migrate its prior local data layout. "
                    + "Review " + incompletePath + ".");
            }

            report.Add("Status=Complete");
            File.WriteAllLines(reportPath, report);
            if (copiedFiles == 0 && conflictFiles == 0)
            {
                return string.Empty;
            }

            return "Local data layout migration completed. Copied="
                + copiedFiles
                + ", conflicts retained="
                + conflictFiles
                + ", report="
                + reportPath;
        }

        private static string MigrateLegacyRuntimeData(
            string installationRoot,
            string dataRoot,
            string reportPath)
        {
            if (File.Exists(reportPath))
            {
                return string.Empty;
            }

            List<string> report = new List<string>
            {
                "OpenVisionLab data-root migration v1",
                "StartedUtc=" + DateTime.UtcNow.ToString("O"),
                "LegacySource=" + installationRoot,
                "DataRoot=" + dataRoot,
                "Policy=copy missing files; retain source; never overwrite target"
            };
            int copiedFiles = 0;
            int existingSameFiles = 0;
            int conflictFiles = 0;
            List<string> failures = new List<string>();

            foreach (string directoryName in new[]
            {
                "CONFIG",
                "RECIPE",
                "QUALIFIED_RECIPE",
                "CAPTURE",
                "TEST",
                "Image",
                "Log"
            })
            {
                string sourceDirectory = Path.Combine(
                    installationRoot,
                    directoryName);
                if (!Directory.Exists(sourceDirectory))
                {
                    continue;
                }

                foreach (string sourceFile in GetLegacyFiles(
                    sourceDirectory,
                    failures,
                    directoryName))
                {
                    string relativePath = Path.GetRelativePath(
                        installationRoot,
                        sourceFile);
                    CopyLegacyFile(
                        sourceFile,
                        Path.Combine(dataRoot, relativePath),
                        relativePath,
                        report,
                        failures,
                        ref copiedFiles,
                        ref existingSameFiles,
                        ref conflictFiles);
                }
            }

            foreach (string fileName in new[] { "SYSTEM.xml", "VISION.xml" })
            {
                string sourceFile = Path.Combine(installationRoot, fileName);
                if (!File.Exists(sourceFile))
                {
                    continue;
                }

                CopyLegacyFile(
                    sourceFile,
                    Path.Combine(dataRoot, fileName),
                    fileName,
                    report,
                    failures,
                    ref copiedFiles,
                    ref existingSameFiles,
                    ref conflictFiles);
            }

            report.Add("CopiedFiles=" + copiedFiles);
            report.Add("ExistingSameFiles=" + existingSameFiles);
            report.Add("ConflictFilesRetainedAtSource=" + conflictFiles);
            report.Add("LegacySourceDeleted=false");
            report.Add("CompletedUtc=" + DateTime.UtcNow.ToString("O"));

            if (failures.Count > 0)
            {
                report.Add("Status=Incomplete");
                report.AddRange(failures);
                string incompletePath = reportPath + ".incomplete";
                File.WriteAllLines(incompletePath, report);
                throw new IOException(
                    "OpenVisionLab could not safely migrate legacy runtime data. "
                    + "Review " + incompletePath + ".");
            }

            report.Add("Status=Complete");
            File.WriteAllLines(reportPath, report);
            if (copiedFiles == 0 && conflictFiles == 0)
            {
                return string.Empty;
            }

            return "Runtime data migration completed. Copied="
                + copiedFiles
                + ", conflicts retained="
                + conflictFiles
                + ", report="
                + reportPath;
        }

        private static void CopyLegacyFile(
            string sourceFile,
            string targetFile,
            string relativePath,
            List<string> report,
            List<string> failures,
            ref int copiedFiles,
            ref int existingSameFiles,
            ref int conflictFiles)
        {
            try
            {
                if (File.Exists(targetFile))
                {
                    if (FilesEqual(sourceFile, targetFile))
                    {
                        existingSameFiles++;
                        report.Add("ExistingSame=" + relativePath);
                    }
                    else
                    {
                        conflictFiles++;
                        report.Add("ConflictTargetKept=" + relativePath);
                    }

                    return;
                }

                string targetDirectory = Path.GetDirectoryName(targetFile);
                if (!string.IsNullOrWhiteSpace(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                File.Copy(sourceFile, targetFile, overwrite: false);
                copiedFiles++;
                report.Add("Copied=" + relativePath);
            }
            catch (Exception ex) when (
                ex is IOException
                || ex is UnauthorizedAccessException
                || ex is NotSupportedException)
            {
                failures.Add(
                    "Failure="
                    + relativePath
                    + " | "
                    + ex.GetType().Name
                    + " | "
                    + ex.Message);
            }
        }

        private static string[] GetLegacyFiles(
            string directory,
            List<string> failures,
            string label)
        {
            try
            {
                return Directory.GetFiles(
                    directory,
                    "*",
                    SearchOption.AllDirectories);
            }
            catch (Exception ex) when (
                ex is IOException
                || ex is UnauthorizedAccessException
                || ex is NotSupportedException)
            {
                failures.Add(
                    "Failure="
                    + label
                    + " enumeration | "
                    + ex.GetType().Name
                    + " | "
                    + ex.Message);
                return Array.Empty<string>();
            }
        }

        private static bool FilesEqual(string firstPath, string secondPath)
        {
            FileInfo first = new FileInfo(firstPath);
            FileInfo second = new FileInfo(secondPath);
            if (first.Length != second.Length)
            {
                return false;
            }

            using SHA256 sha = SHA256.Create();
            using FileStream firstStream = File.OpenRead(firstPath);
            byte[] firstHash = sha.ComputeHash(firstStream);
            using FileStream secondStream = File.OpenRead(secondPath);
            byte[] secondHash = sha.ComputeHash(secondStream);
            return CryptographicOperations.FixedTimeEquals(firstHash, secondHash);
        }

        private static bool PathsEqual(string first, string second)
        {
            return string.Equals(
                Path.GetFullPath(first ?? string.Empty).TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar),
                Path.GetFullPath(second ?? string.Empty).TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase);
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

        private static string EnsureXmlExtension(string fileName)
        {
            return Path.GetExtension(fileName).Equals(
                ".xml",
                StringComparison.OrdinalIgnoreCase)
                ? fileName
                : $"{fileName}.xml";
        }

        private sealed class RuntimeDataPathState
        {
            public RuntimeDataPathState(
                string dataRootDirectory,
                string migrationReportPath,
                string migrationNotice)
            {
                DataRootDirectory = dataRootDirectory;
                MigrationReportPath = migrationReportPath;
                MigrationNotice = migrationNotice;
            }

            public string DataRootDirectory { get; }

            public string MigrationReportPath { get; }

            public string MigrationNotice { get; }
        }
    }
}
