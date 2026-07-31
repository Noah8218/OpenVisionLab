using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeToolPropertySavedEventArgs : EventArgs
    {
        public OpenVisionNativeToolPropertySavedEventArgs(
            string toolName,
            string recipeName,
            bool succeeded,
            bool recoveredFromFailure,
            string errorMessage)
        {
            ToolName = toolName ?? string.Empty;
            RecipeName = recipeName ?? string.Empty;
            Succeeded = succeeded;
            RecoveredFromFailure = recoveredFromFailure;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        public string ToolName { get; }

        public string RecipeName { get; }

        public bool Succeeded { get; }

        public bool RecoveredFromFailure { get; }

        public string ErrorMessage { get; }
    }

    internal sealed class OpenVisionNativeToolPropertyLoadFailure
    {
        public OpenVisionNativeToolPropertyLoadFailure(
            string toolName,
            string recipeName,
            string errorMessage,
            string backupPath)
        {
            ToolName = toolName ?? string.Empty;
            RecipeName = recipeName ?? string.Empty;
            ErrorMessage = errorMessage ?? string.Empty;
            BackupPath = backupPath ?? string.Empty;
        }

        public string ToolName { get; }

        public string RecipeName { get; }

        public string ErrorMessage { get; }

        public string BackupPath { get; }

        public bool PreviousFileWasBackedUp =>
            !string.IsNullOrWhiteSpace(BackupPath);
    }

    internal static class OpenVisionNativeToolPropertySessionStore
    {
        private static readonly Dictionary<string, OpenCvPropertyBase> properties = new Dictionary<string, OpenCvPropertyBase>(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> failedSaveKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, OpenVisionNativeToolPropertyLoadFailure> failedLoads = new Dictionary<string, OpenVisionNativeToolPropertyLoadFailure>(StringComparer.OrdinalIgnoreCase);
        private static readonly object syncRoot = new object();
        private static Func<VisionToolRepository> repositoryAccessor;

        public static event EventHandler<OpenVisionNativeToolPropertySavedEventArgs> PropertySaved;

        internal static bool FailNextSaveForTest { get; set; }
        internal static string FailNextLoadKeyForTest { get; set; } =
            string.Empty;

        public static void SetRepositoryContext(Func<VisionToolRepository> accessor)
        {
            lock (syncRoot)
            {
                repositoryAccessor = accessor;
                properties.Clear();
                failedSaveKeys.Clear();
                failedLoads.Clear();
            }
        }

        public static TProperty GetRepositoryProperty<TProperty>(
            string toolKey,
            Func<VisionToolRepository, List<TProperty>> selectProperties,
            Func<TProperty> createDefault,
            int index = 0)
            where TProperty : OpenCvPropertyBase, IOpenCvConfigurableProperty<TProperty>
        {
            if (selectProperties == null)
            {
                throw new ArgumentNullException(nameof(selectProperties));
            }

            if (createDefault == null)
            {
                throw new ArgumentNullException(nameof(createDefault));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            VisionToolRepository repository = TryGetRepository();
            if (repository == null)
            {
                return GetOrLoad(toolKey, createDefault);
            }

            string key = CreateKey(toolKey);
            lock (syncRoot)
            {
                List<TProperty> list = selectProperties(repository);
                if (list == null)
                {
                    return GetOrLoad(toolKey, createDefault);
                }

                while (list.Count <= index)
                {
                    list.Add(LoadProperty(toolKey, createDefault));
                }

                if (list[index] == null)
                {
                    list[index] = LoadProperty(toolKey, createDefault);
                }

                // Repository-owned property objects are the recipe source of truth.
                // The local dictionary is only a fast lookup/fallback cache for native WPF documents.
                properties[key] = list[index];
                TrackLoadResult(
                    string.IsNullOrWhiteSpace(list[index].NAME)
                        ? toolKey
                        : list[index].NAME,
                    PropertyGridEditorFactory.GetRecipeName(),
                    list[index]);
                return list[index];
            }
        }

        public static void SetRepositoryProperty<TProperty>(
            string toolKey,
            Func<VisionToolRepository, List<TProperty>> selectProperties,
            TProperty property,
            int index = 0)
            where TProperty : OpenCvPropertyBase, IOpenCvConfigurableProperty<TProperty>
        {
            if (property == null)
            {
                return;
            }

            if (selectProperties == null)
            {
                throw new ArgumentNullException(nameof(selectProperties));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            string key = CreateKey(toolKey);
            VisionToolRepository repository = TryGetRepository();
            lock (syncRoot)
            {
                if (repository != null)
                {
                    List<TProperty> list = selectProperties(repository);
                    if (list != null)
                    {
                        while (list.Count <= index)
                        {
                            list.Add(null);
                        }

                        list[index] = property;
                    }
                }

                properties[key] = property;
            }
        }

        public static TProperty GetOrLoad<TProperty>(string toolKey, Func<TProperty> createDefault)
            where TProperty : OpenCvPropertyBase, IOpenCvConfigurableProperty<TProperty>
        {
            if (createDefault == null)
            {
                throw new ArgumentNullException(nameof(createDefault));
            }

            string key = CreateKey(toolKey);
            lock (syncRoot)
            {
                if (properties.TryGetValue(key, out OpenCvPropertyBase existing) && existing is TProperty typed)
                {
                    return typed;
                }

                TProperty property = LoadProperty(toolKey, createDefault);
                properties[key] = property;
                return property;
            }
        }

        public static bool TryGetLoadFailure(
            string toolKey,
            out OpenVisionNativeToolPropertyLoadFailure failure)
        {
            string key = CreateKey(toolKey);
            lock (syncRoot)
            {
                return failedLoads.TryGetValue(key, out failure);
            }
        }

        public static void Save<TProperty>(string toolKey, TProperty property)
            where TProperty : OpenCvPropertyBase
        {
            if (property == null)
            {
                return;
            }

            string storageKey = string.IsNullOrWhiteSpace(property.NAME) ? toolKey : property.NAME;
            string key = CreateKey(storageKey);
            string recipeName = PropertyGridEditorFactory.GetRecipeName();
            lock (syncRoot)
            {
                properties[key] = property;
            }

            Exception saveException = null;
            try
            {
                if (FailNextSaveForTest)
                {
                    FailNextSaveForTest = false;
                    throw new InvalidOperationException(
                        "Forced native Tool property persistence failure.");
                }

                property.SaveConfig(recipeName);
            }
            catch (Exception exception)
            {
                // Keep the in-memory teaching value and explicit Preview/Run contract,
                // but report that the value can be lost when the Tool is reopened.
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

            PropertySaved?.Invoke(
                null,
                new OpenVisionNativeToolPropertySavedEventArgs(
                    toolKey,
                    recipeName,
                    saveException == null,
                    recoveredFromFailure,
                    saveException?.Message));
        }

        private static TProperty LoadProperty<TProperty>(
            string toolKey,
            Func<TProperty> createDefault)
            where TProperty : OpenCvPropertyBase, IOpenCvConfigurableProperty<TProperty>
        {
            TProperty property = createDefault();
            string storageKey = string.IsNullOrWhiteSpace(property.NAME)
                ? toolKey
                : property.NAME;
            string key = CreateKey(storageKey);
            string recipeName = PropertyGridEditorFactory.GetRecipeName();
            try
            {
                if (string.Equals(
                        FailNextLoadKeyForTest,
                        storageKey,
                        StringComparison.OrdinalIgnoreCase))
                {
                    FailNextLoadKeyForTest = string.Empty;
                    throw new InvalidOperationException(
                        "Forced native Tool property load failure.");
                }

                TProperty loaded = property.LoadConfig(recipeName);
                TrackLoadResult(storageKey, recipeName, loaded);

                return loaded;
            }
            catch (Exception exception)
            {
                lock (syncRoot)
                {
                    failedLoads[key] =
                        new OpenVisionNativeToolPropertyLoadFailure(
                            storageKey,
                            recipeName,
                            exception.GetBaseException().Message,
                            string.Empty);
                }

                return property;
            }
        }

        private static void TrackLoadResult(
            string storageKey,
            string recipeName,
            OpenCvPropertyBase property)
        {
            string key = CreateKey(storageKey);
            lock (syncRoot)
            {
                if (property?.LastConfigLoadResult?.Disposition
                    == XmlFileLoadDisposition.ReplacedInvalidFile)
                {
                    failedLoads[key] =
                        new OpenVisionNativeToolPropertyLoadFailure(
                            storageKey,
                            recipeName,
                            property.LastConfigLoadResult.ErrorMessage,
                            property.LastConfigLoadResult.BackupPath);
                }
                else
                {
                    failedLoads.Remove(key);
                }
            }
        }

        private static VisionToolRepository TryGetRepository()
        {
            try
            {
                return repositoryAccessor?.Invoke();
            }
            catch
            {
                return null;
            }
        }

        private static string CreateKey(string toolKey)
        {
            string recipeName = PropertyGridEditorFactory.GetRecipeName();
            return (recipeName ?? string.Empty).Trim() + "|" + (toolKey ?? string.Empty).Trim();
        }
    }
}
