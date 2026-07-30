using System;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeToolPersistenceStatusText
    {
        public static string CreateLoadFailure(
            string toolName,
            string recipeName,
            string errorMessage,
            string backupPath)
        {
            string displayRecipeName = DisplayRecipeName(recipeName);
            string displayError = DisplayError(errorMessage);
            string format;
            object[] arguments;
            if (!string.IsNullOrWhiteSpace(backupPath))
            {
                format = LocalizedText(
                    "VisionTool.Persistence.LoadRecoveredInvalidFormat",
                    "{0} / Recipe {1}: Saved settings were invalid or incompatible, "
                    + "so this Tool opened with default values. Do not assume prior teaching was restored. "
                    + "Review the values. The previous file was preserved at {2}. Cause: {3}");
                arguments = new object[]
                {
                    toolName,
                    displayRecipeName,
                    backupPath,
                    displayError
                };
            }
            else
            {
                format = LocalizedText(
                    "VisionTool.Persistence.LoadFailedFormat",
                    "{0} / Recipe {1}: Saved settings could not be loaded, "
                    + "so this Tool opened with default values. Do not assume prior teaching was restored. "
                    + "Review the values; the saved file was not changed. Cause: {2}");
                arguments = new object[]
                {
                    toolName,
                    displayRecipeName,
                    displayError
                };
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                format,
                arguments);
        }

        public static string CreateSaveFailure(
            string toolName,
            string recipeName,
            string errorMessage)
        {
            string format = LocalizedText(
                "VisionTool.Persistence.SaveFailedFormat",
                "Settings could not be saved for {0} / Recipe {1}. "
                + "The current values remain in memory but may be lost after reopening. Cause: {2}");
            return string.Format(
                CultureInfo.CurrentCulture,
                format,
                toolName,
                DisplayRecipeName(recipeName),
                DisplayError(errorMessage));
        }

        public static string CreateSaveRecovered(
            string toolName,
            string recipeName)
        {
            string format = LocalizedText(
                "VisionTool.Persistence.SaveRecoveredFormat",
                "Settings save recovered for {0} / Recipe {1}. "
                + "The current values are now persisted.");
            return string.Format(
                CultureInfo.CurrentCulture,
                format,
                toolName,
                DisplayRecipeName(recipeName));
        }

        private static string DisplayRecipeName(string recipeName)
        {
            return string.IsNullOrWhiteSpace(recipeName)
                ? LocalizedText(
                    "VisionTool.Persistence.DefaultRecipe",
                    "default Recipe")
                : recipeName;
        }

        private static string DisplayError(string errorMessage)
        {
            return string.IsNullOrWhiteSpace(errorMessage)
                ? LocalizedText(
                    "VisionTool.Persistence.UnknownError",
                    "unknown error")
                : errorMessage;
        }

        private static string LocalizedText(
            string key,
            string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value)
                || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }
    }
}
