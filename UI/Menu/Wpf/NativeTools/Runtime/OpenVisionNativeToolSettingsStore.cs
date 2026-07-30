using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeToolSettingsSavedEventArgs : EventArgs
    {
        public OpenVisionNativeToolSettingsSavedEventArgs(
            string configName,
            string toolName,
            string recipeName,
            bool succeeded,
            bool recoveredFromFailure,
            string errorMessage)
        {
            ConfigName = configName ?? string.Empty;
            ToolName = toolName ?? string.Empty;
            RecipeName = recipeName ?? string.Empty;
            Succeeded = succeeded;
            RecoveredFromFailure = recoveredFromFailure;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        public string ConfigName { get; }

        public string ToolName { get; }

        public string RecipeName { get; }

        public bool Succeeded { get; }

        public bool RecoveredFromFailure { get; }

        public string ErrorMessage { get; }
    }

    internal sealed class OpenVisionNativeToolSettingsLoadFailure
    {
        public OpenVisionNativeToolSettingsLoadFailure(
            string configName,
            string toolName,
            string recipeName,
            string errorMessage,
            string backupPath)
        {
            ConfigName = configName ?? string.Empty;
            ToolName = toolName ?? string.Empty;
            RecipeName = recipeName ?? string.Empty;
            ErrorMessage = errorMessage ?? string.Empty;
            BackupPath = backupPath ?? string.Empty;
        }

        public string ConfigName { get; }

        public string ToolName { get; }

        public string RecipeName { get; }

        public string ErrorMessage { get; }

        public string BackupPath { get; }

        public bool PreviousFileWasBackedUp =>
            !string.IsNullOrWhiteSpace(BackupPath);
    }

    internal static class OpenVisionNativeToolSettingsStore
    {
        private const string ConfigSuffix = "_ToolState";
        private static readonly object syncRoot = new object();
        private static readonly HashSet<string> failedSaveKeys =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, OpenVisionNativeToolSettingsLoadFailure>
            failedLoads =
                new Dictionary<string, OpenVisionNativeToolSettingsLoadFailure>(
                    StringComparer.OrdinalIgnoreCase);

        public static event EventHandler<OpenVisionNativeToolSettingsSavedEventArgs>
            SettingsSaved;

        internal static string FailNextLoadKeyForTest { get; set; } =
            string.Empty;

        internal static string FailNextSaveKeyForTest { get; set; } =
            string.Empty;

        public static string CreateConfigName(string toolName)
        {
            string safeName = string.IsNullOrWhiteSpace(toolName) ? "Tool" : toolName.Trim();
            return safeName + ConfigSuffix;
        }

        public static void ResetContext()
        {
            lock (syncRoot)
            {
                failedSaveKeys.Clear();
                failedLoads.Clear();
            }
        }

        public static TSettings Load<TSettings>(string configName, TSettings defaultSettings)
            where TSettings : class
        {
            if (defaultSettings == null)
            {
                throw new ArgumentNullException(nameof(defaultSettings));
            }

            string recipeName = PropertyGridEditorFactory.GetRecipeName();
            string key = CreateKey(recipeName, configName);
            string toolName = ResolveToolName(configName);
            try
            {
                if (string.Equals(
                        FailNextLoadKeyForTest,
                        configName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    FailNextLoadKeyForTest = string.Empty;
                    throw new InvalidOperationException(
                        "Forced native Tool settings load failure.");
                }

                string path = RecipeWorkspaceService.GetVisionConfigPath(
                    recipeName,
                    configName);
                TSettings loadedSettings = SerializeHelper.LoadOrCreateXmlFile(
                    path,
                    defaultSettings,
                    out _,
                    out XmlFileLoadResult loadResult);
                if (loadResult.Disposition
                    == XmlFileLoadDisposition.ReplacedInvalidFile)
                {
                    lock (syncRoot)
                    {
                        failedLoads[key] =
                            new OpenVisionNativeToolSettingsLoadFailure(
                                configName,
                                toolName,
                                recipeName,
                                loadResult.ErrorMessage,
                                loadResult.BackupPath);
                    }
                }

                return loadedSettings;
            }
            catch (Exception exception)
            {
                lock (syncRoot)
                {
                    failedLoads[key] =
                        new OpenVisionNativeToolSettingsLoadFailure(
                            configName,
                            toolName,
                            recipeName,
                            exception.GetBaseException().Message,
                            string.Empty);
                }

                return defaultSettings;
            }
        }

        public static bool TryGetLoadFailure(
            string configName,
            out OpenVisionNativeToolSettingsLoadFailure failure)
        {
            string key = CreateKey(
                PropertyGridEditorFactory.GetRecipeName(),
                configName);
            lock (syncRoot)
            {
                return failedLoads.TryGetValue(key, out failure);
            }
        }

        public static void Save<TSettings>(string configName, TSettings settings)
            where TSettings : class
        {
            if (settings == null)
            {
                return;
            }

            string recipeName = PropertyGridEditorFactory.GetRecipeName();
            string key = CreateKey(recipeName, configName);
            string toolName = ResolveToolName(configName);
            Exception saveException = null;
            try
            {
                if (string.Equals(
                        FailNextSaveKeyForTest,
                        configName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    FailNextSaveKeyForTest = string.Empty;
                    throw new InvalidOperationException(
                        "Forced native Tool settings persistence failure.");
                }

                string path = RecipeWorkspaceService.GetVisionConfigPath(
                    recipeName,
                    configName);
                SerializeHelper.SaveXmlFile(path, settings);
            }
            catch (Exception exception)
            {
                // Tool setting persistence must not break preview/run while an operator is teaching parameters.
                saveException = exception;
            }

            bool recoveredFromFailure;
            lock (syncRoot)
            {
                if (saveException == null)
                {
                    recoveredFromFailure =
                        failedSaveKeys.Remove(key)
                        | failedLoads.Remove(key);
                }
                else
                {
                    failedSaveKeys.Add(key);
                    recoveredFromFailure = false;
                }
            }

            SettingsSaved?.Invoke(
                null,
                new OpenVisionNativeToolSettingsSavedEventArgs(
                    configName,
                    toolName,
                    recipeName,
                    saveException == null,
                    recoveredFromFailure,
                    saveException?.GetBaseException().Message));
        }

        private static string ResolveToolName(string configName)
        {
            string value = (configName ?? string.Empty).Trim();
            return value.EndsWith(
                    ConfigSuffix,
                    StringComparison.OrdinalIgnoreCase)
                ? value.Substring(0, value.Length - ConfigSuffix.Length)
                : value;
        }

        private static string CreateKey(
            string recipeName,
            string configName)
        {
            return (recipeName ?? string.Empty).Trim()
                + "|"
                + (configName ?? string.Empty).Trim();
        }
    }
}
