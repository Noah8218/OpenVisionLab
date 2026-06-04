using Lib.Common;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal static class RecipeWorkspaceService
    {
        private const string RecipeRoot = "RECIPE";

        public static void EnsureRoot()
        {
            CUtil.InitDirectory(RecipeRoot);
        }

        public static void EnsureVisionWorkspace(string recipeName)
        {
            EnsureRoot();
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return;
            }

            CUtil.InitDirectory($@"{RecipeRoot}\{recipeName}\VISION");
            CUtil.InitDirectory($@"{RecipeRoot}\{recipeName}\GRAPH");
            CUtil.InitDirectory($@"{RecipeRoot}\{recipeName}\PATTERN");
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

        public static string GetVisionDataPath(string recipeName)
        {
            EnsureVisionWorkspace(recipeName);

            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return $@"{Application.StartupPath}\{RecipeRoot}\VISION.xml";
            }

            return $@"{Application.StartupPath}\{RecipeRoot}\{recipeName}\VISION.xml";
        }

        private static string CombineRecipePath(string recipeName, string childDirectory)
        {
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                CUtil.InitDirectory($@"{RecipeRoot}\{childDirectory}");
                return $@"{Application.StartupPath}\{RecipeRoot}\{childDirectory}";
            }

            CUtil.InitDirectory($@"{RecipeRoot}\{recipeName}\{childDirectory}");
            return $@"{Application.StartupPath}\{RecipeRoot}\{recipeName}\{childDirectory}";
        }
    }
}
