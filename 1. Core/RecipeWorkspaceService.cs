using Lib.Common;
using System.IO;

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

        private static string GetRecipeDirectory(string recipeName)
        {
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return Path.Combine(AppPathService.StartupPath, RecipeRoot);
            }

            return Path.Combine(AppPathService.StartupPath, RecipeRoot, recipeName);
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
    }
}
