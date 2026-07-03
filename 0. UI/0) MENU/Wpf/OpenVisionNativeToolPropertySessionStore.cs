using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeToolPropertySessionStore
    {
        private static readonly Dictionary<string, OpenCvPropertyBase> properties = new Dictionary<string, OpenCvPropertyBase>(StringComparer.OrdinalIgnoreCase);
        private static readonly object syncRoot = new object();
        private static Func<VisionToolRepository> repositoryAccessor;

        public static void SetRepositoryContext(Func<VisionToolRepository> accessor)
        {
            lock (syncRoot)
            {
                repositoryAccessor = accessor;
                properties.Clear();
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
                    list.Add(LoadProperty(createDefault));
                }

                if (list[index] == null)
                {
                    list[index] = LoadProperty(createDefault);
                }

                // Repository-owned property objects are the recipe source of truth.
                // The local dictionary is only a fast lookup/fallback cache for native WPF documents.
                properties[key] = list[index];
                return list[index];
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

                TProperty property = LoadProperty(createDefault);
                properties[key] = property;
                return property;
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
            lock (syncRoot)
            {
                properties[key] = property;
            }

            try
            {
                property.SaveConfig(PropertyGridEditorFactory.GetRecipeName());
            }
            catch
            {
                // Parameter persistence is a UX aid; preview/run must not fail because disk save failed.
            }
        }

        private static TProperty LoadProperty<TProperty>(Func<TProperty> createDefault)
            where TProperty : OpenCvPropertyBase, IOpenCvConfigurableProperty<TProperty>
        {
            TProperty property = createDefault();
            try
            {
                return property.LoadConfig(PropertyGridEditorFactory.GetRecipeName());
            }
            catch
            {
                return property;
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
