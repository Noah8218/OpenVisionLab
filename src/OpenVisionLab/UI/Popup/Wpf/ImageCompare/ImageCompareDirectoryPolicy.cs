using System;
using System.Diagnostics;
using System.IO;

namespace OpenVisionLab
{
    /// <summary>
    /// Owns the remembered directory used by the Image Compare file picker.
    /// </summary>
    internal sealed class ImageCompareDirectoryPolicy
    {
        private const string DataRootEnvironmentVariable = "OPENVISIONLAB_DATA_ROOT";
        private const string PersistedDirectoryFileName = "image_compare_last_directory.txt";
        private string lastImageCompareDirectory;

        public string ResolveInitialDirectory()
        {
            if (IsDirectory(lastImageCompareDirectory))
            {
                return lastImageCompareDirectory;
            }

            string persistedDirectory = LoadPersistedImageDirectory();
            if (IsDirectory(persistedDirectory))
            {
                lastImageCompareDirectory = persistedDirectory;
                return persistedDirectory;
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }

        public void RememberImageDirectory(string[] imagePaths)
        {
            string firstPath = imagePaths == null ? string.Empty : Array.Find(imagePaths, File.Exists);
            if (string.IsNullOrWhiteSpace(firstPath))
            {
                return;
            }

            string directory = Path.GetDirectoryName(firstPath);
            if (!IsDirectory(directory))
            {
                return;
            }

            lastImageCompareDirectory = directory;
            SavePersistedImageDirectory(directory);
        }

        private static string LoadPersistedImageDirectory()
        {
            string path = GetPersistedImageDirectoryPath();
            try
            {
                return File.Exists(path) ? File.ReadAllText(path).Trim() : string.Empty;
            }
            catch (Exception exception) when (IsRecoverableFileSystemException(exception))
            {
                Trace.TraceWarning("Image Compare last-directory load failed: " + exception.Message);
                return string.Empty;
            }
        }

        private static void SavePersistedImageDirectory(string directory)
        {
            string path = GetPersistedImageDirectoryPath();
            try
            {
                string parentDirectory = Path.GetDirectoryName(path);
                if (string.IsNullOrWhiteSpace(parentDirectory))
                {
                    return;
                }

                Directory.CreateDirectory(parentDirectory);
                File.WriteAllText(path, directory ?? string.Empty);
            }
            catch (Exception exception) when (IsRecoverableFileSystemException(exception))
            {
                Trace.TraceWarning("Image Compare last-directory save failed: " + exception.Message);
            }
        }

        private static string GetPersistedImageDirectoryPath()
        {
            string configuredRoot = Environment.GetEnvironmentVariable(DataRootEnvironmentVariable);
            string expandedRoot = Environment.ExpandEnvironmentVariables(configuredRoot?.Trim().Trim('"') ?? string.Empty);
            string dataRoot = !string.IsNullOrWhiteSpace(expandedRoot) && Path.IsPathRooted(expandedRoot)
                ? Path.GetFullPath(expandedRoot)
                : Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "OpenVisionLab");
            return Path.Combine(dataRoot, "CONFIG", PersistedDirectoryFileName);
        }

        private static bool IsDirectory(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && Directory.Exists(path);
        }

        private static bool IsRecoverableFileSystemException(Exception exception)
        {
            return exception is IOException
                || exception is UnauthorizedAccessException
                || exception is NotSupportedException
                || exception is ArgumentException
                || exception is System.Security.SecurityException;
        }
    }
}
