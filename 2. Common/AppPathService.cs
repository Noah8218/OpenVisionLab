using System;
using System.IO;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public static class AppPathService
    {
        public static string StartupPath => Application.StartupPath;

        public static string CaptureDirectory => EnsureDirectory("CAPTURE");

        public static string RecipeRootDirectory => EnsureDirectory("RECIPE");

        public static string TestDirectory => EnsureDirectory("TEST");

        public static string Combine(params string[] paths)
        {
            string[] parts = new string[paths.Length + 1];
            parts[0] = StartupPath;
            Array.Copy(paths, 0, parts, 1, paths.Length);
            return Path.Combine(parts);
        }

        public static string EnsureDirectory(params string[] paths)
        {
            string directory = Combine(paths);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public static string GetCaptureFilePath(string title, DateTime timestamp)
        {
            return Path.Combine(CaptureDirectory, $"{title}_{timestamp:yyyyMMdd_HHmmss}.jpeg");
        }

        public static string GetTestConfigPath(string configName)
        {
            return Path.Combine(TestDirectory, EnsureXmlExtension(configName));
        }

        private static string EnsureXmlExtension(string fileName)
        {
            return Path.GetExtension(fileName).Equals(".xml", StringComparison.OrdinalIgnoreCase)
                ? fileName
                : $"{fileName}.xml";
        }
    }
}
