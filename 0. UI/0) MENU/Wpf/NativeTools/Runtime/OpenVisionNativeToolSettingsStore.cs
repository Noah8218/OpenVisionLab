using System;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeToolSettingsStore
    {
        public static event EventHandler SettingsSaved;

        public static string CreateConfigName(string toolName)
        {
            string safeName = string.IsNullOrWhiteSpace(toolName) ? "Tool" : toolName.Trim();
            return safeName + "_ToolState";
        }

        public static TSettings Load<TSettings>(string configName, TSettings defaultSettings)
            where TSettings : class
        {
            if (defaultSettings == null)
            {
                throw new ArgumentNullException(nameof(defaultSettings));
            }

            try
            {
                string path = RecipeWorkspaceService.GetVisionConfigPath(PropertyGridEditorFactory.GetRecipeName(), configName);
                return SerializeHelper.LoadOrCreateXmlFile(path, defaultSettings, out _);
            }
            catch
            {
                return defaultSettings;
            }
        }

        public static void Save<TSettings>(string configName, TSettings settings)
            where TSettings : class
        {
            if (settings == null)
            {
                return;
            }

            try
            {
                string path = RecipeWorkspaceService.GetVisionConfigPath(PropertyGridEditorFactory.GetRecipeName(), configName);
                SerializeHelper.SaveXmlFile(path, settings);
            }
            catch
            {
                // Tool setting persistence must not break preview/run while an operator is teaching parameters.
            }

            SettingsSaved?.Invoke(null, EventArgs.Empty);
        }
    }
}
