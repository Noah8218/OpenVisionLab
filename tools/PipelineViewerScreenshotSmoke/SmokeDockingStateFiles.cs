#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal static class SmokeDockingStateFiles
    {
        private const string LayersFileName = "LayerDocking.layers";
        private const string LayoutFileName = "LayerDocking.layout";

        internal static T RunWithBackup<T>(string uiConfigDirectory, Func<T> capture)
        {
            return RunWithBackupCore(uiConfigDirectory, capture);
        }

        internal static void RunWithBackup(string uiConfigDirectory, Action capture)
        {
            _ = RunWithBackupCore(uiConfigDirectory, () =>
            {
                capture();
                return true;
            });
        }

        internal static void CopyCurrent(
            string uiConfigDirectory,
            string fileName,
            string outputDirectory,
            string outputFileName)
        {
            string path = Path.Combine(uiConfigDirectory, fileName);
            if (File.Exists(path))
            {
                File.Copy(path, Path.Combine(outputDirectory, outputFileName), true);
            }
        }

        internal static void Clear(string uiConfigDirectory)
        {
            foreach (string path in GetStatePaths(uiConfigDirectory))
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private static T RunWithBackupCore<T>(string uiConfigDirectory, Func<T> capture)
        {
            string[] paths = GetStatePaths(uiConfigDirectory);
            Dictionary<string, byte[]> backups = paths
                .Where(File.Exists)
                .ToDictionary(path => path, File.ReadAllBytes, StringComparer.OrdinalIgnoreCase);

            try
            {
                return capture();
            }
            finally
            {
                foreach (string path in paths)
                {
                    try
                    {
                        if (backups.TryGetValue(path, out byte[]? bytes) && bytes != null)
                        {
                            File.WriteAllBytes(path, bytes);
                        }
                        else if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static string[] GetStatePaths(string uiConfigDirectory)
        {
            return new[]
            {
                Path.Combine(uiConfigDirectory, LayersFileName),
                Path.Combine(uiConfigDirectory, LayoutFileName)
            };
        }
    }
}
