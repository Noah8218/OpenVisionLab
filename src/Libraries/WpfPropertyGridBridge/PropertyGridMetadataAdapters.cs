extern alias WpfPropertyGridOriginal;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using OpenVisionLab;

namespace System.Windows.Controls.WpfPropertyGrid
{
    internal static class BridgeCategoryOrderMap
    {
        public static Dictionary<string, int> Create(Type selectedType)
        {
            Dictionary<string, int> categoryOrders = new Dictionary<string, int>();
            if (selectedType == null)
            {
                return categoryOrders;
            }

            List<Type> hierarchy = new List<Type>();
            for (Type currentType = selectedType; currentType != null && currentType != typeof(object); currentType = currentType.BaseType)
            {
                hierarchy.Add(currentType);
            }

            // Base defaults are applied first so a derived tool can intentionally override shared category order.
            for (int index = hierarchy.Count - 1; index >= 0; index--)
            {
                foreach (CategoryOrderAttribute attribute in hierarchy[index].GetCustomAttributes(typeof(CategoryOrderAttribute), false))
                {
                    categoryOrders[attribute.CategoryName] = attribute.Order;
                    categoryOrders[PropertyGridLocalization.TranslateCategory(attribute.CategoryName)] = attribute.Order;
                }
            }

            return categoryOrders;
        }
    }

    internal sealed class BridgePropertyComparer
        : IComparer<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem>
    {
        private readonly Dictionary<string, int> categoryOrders;

        public BridgePropertyComparer(Type selectedType)
        {
            categoryOrders = BridgeCategoryOrderMap.Create(selectedType);
        }

        public int Compare(
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem x,
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem y)
        {
            int categoryCompare = GetCategoryOrder(x).CompareTo(GetCategoryOrder(y));
            if (categoryCompare != 0)
            {
                return categoryCompare;
            }

            int orderCompare = GetOrder(x).CompareTo(GetOrder(y));
            if (orderCompare != 0)
            {
                return orderCompare;
            }

            return string.Compare(GetName(x), GetName(y), StringComparison.CurrentCultureIgnoreCase);
        }

        private int GetCategoryOrder(WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem item)
        {
            string categoryName = item?.CategoryName ?? item?.PropertyDescriptor?.Category ?? string.Empty;
            return categoryName != null && categoryOrders.TryGetValue(categoryName, out int order) ? order : int.MaxValue;
        }

        private static int GetOrder(WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem item)
        {
            PropertyOrderAttribute attribute = item?.PropertyDescriptor?.Attributes[typeof(PropertyOrderAttribute)] as PropertyOrderAttribute;
            return attribute?.Order ?? int.MaxValue;
        }

        private static string GetName(WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem item)
        {
            return item?.DisplayName ?? item?.PropertyDescriptor?.Name ?? string.Empty;
        }
    }

    internal sealed class BridgeCategoryComparer
        : IComparer<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.CategoryItem>
    {
        private readonly Dictionary<string, int> categoryOrders;

        public BridgeCategoryComparer(Type selectedType)
        {
            categoryOrders = BridgeCategoryOrderMap.Create(selectedType);
        }

        public int Compare(
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.CategoryItem x,
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.CategoryItem y)
        {
            string xName = GetCategoryName(x);
            string yName = GetCategoryName(y);

            int orderCompare = GetOrder(xName).CompareTo(GetOrder(yName));
            if (orderCompare != 0)
            {
                return orderCompare;
            }

            return string.Compare(xName, yName, StringComparison.CurrentCultureIgnoreCase);
        }

        private int GetOrder(string categoryName)
        {
            return categoryName != null && categoryOrders.TryGetValue(categoryName, out int order) ? order : int.MaxValue;
        }

        private static string GetCategoryName(WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.CategoryItem item)
        {
            CategoryAttribute categoryAttribute = item?.Attribute as CategoryAttribute;
            return categoryAttribute?.Category ?? string.Empty;
        }
    }

    internal sealed class DynamicPropertyGridTypeDescriptionProvider : TypeDescriptionProvider
    {
        private readonly TypeDescriptionProvider parentProvider;

        public DynamicPropertyGridTypeDescriptionProvider(TypeDescriptionProvider parentProvider)
            : base(parentProvider)
        {
            this.parentProvider = parentProvider;
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new DynamicPropertyGridTypeDescriptor(parentProvider.GetTypeDescriptor(objectType, instance), objectType, instance);
        }
    }

    internal sealed class DynamicPropertyGridTypeDescriptor : CustomTypeDescriptor
    {
        private static readonly ConcurrentDictionary<string, LocalizedPropertyDescriptor> LocalizedDescriptorCache =
            new ConcurrentDictionary<string, LocalizedPropertyDescriptor>(StringComparer.Ordinal);

        private readonly Type objectType;
        private readonly object instance;

        public DynamicPropertyGridTypeDescriptor(ICustomTypeDescriptor parentDescriptor, Type objectType, object instance)
            : base(parentDescriptor)
        {
            this.objectType = objectType;
            this.instance = instance;
        }
        public override PropertyDescriptorCollection GetProperties()
        {
            return BuildProperties(base.GetProperties());
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return BuildProperties(base.GetProperties(attributes));
        }

        private PropertyDescriptorCollection BuildProperties(PropertyDescriptorCollection properties)
        {
            List<PropertyDescriptor> localizedProperties = new List<PropertyDescriptor>();
            HashSet<string> propertyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> rangeCompanionNames = BuildRangeCompanionNames(properties);
            bool hasProgressiveViewport = PropertyGrid.TryGetProgressivePropertyViewport(instance, out int visiblePropertyCount);
            foreach (PropertyDescriptor property in properties)
            {
                if (property == null
                    || !ShouldExposeProperty(property)
                    || !propertyNames.Add(property.Name ?? string.Empty))
                {
                    continue;
                }

                if (hasProgressiveViewport && localizedProperties.Count >= visiblePropertyCount)
                {
                    continue;
                }

                localizedProperties.Add(GetLocalizedPropertyDescriptor(property));
            }

            return new PropertyDescriptorCollection(localizedProperties.ToArray(), true);
        }

        private static HashSet<string> BuildRangeCompanionNames(PropertyDescriptorCollection properties)
        {
            HashSet<string> companionNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (PropertyDescriptor property in properties)
            {
                RangeEditorAttribute rangeEditor = property?.Attributes[typeof(RangeEditorAttribute)] as RangeEditorAttribute;
                if (rangeEditor == null)
                {
                    continue;
                }

                // RangeEditor rows edit Min/Max as one operator concept. Keep the model's max
                // property for XML/execution, but do not show it again as a duplicate row.
                if (!string.IsNullOrWhiteSpace(rangeEditor.MaxPropertyName)
                    && !string.Equals(rangeEditor.MaxPropertyName, property.Name, StringComparison.OrdinalIgnoreCase))
                {
                    companionNames.Add(rangeEditor.MaxPropertyName);
                }
            }

            return companionNames;
        }

        private static void RegisterHiddenProperties(Type selectedType, IEnumerable<string> propertyNames)
        {
            PropertyGrid.RegisterHiddenPropertiesForType(selectedType, propertyNames);
        }

        private LocalizedPropertyDescriptor GetLocalizedPropertyDescriptor(PropertyDescriptor property)
        {
            // Property models are type-shaped. Reusing wrappers avoids rebuilding dozens of
            // descriptor objects on every heavy tool open while preserving live localization.
            string key = (objectType?.AssemblyQualifiedName ?? string.Empty) + "|" + (property?.Name ?? string.Empty);
            return LocalizedDescriptorCache.GetOrAdd(
                key,
                _ => new LocalizedPropertyDescriptor(objectType, property));
        }

        private bool ShouldExposeProperty(PropertyDescriptor property)
        {
            if (property == null)
            {
                return false;
            }

            string name = property.Name ?? string.Empty;
            if (PropertyGrid.IsPropertyHidden(objectType, name))
            {
                return false;
            }

            // The bridge is used for algorithm property models, not arbitrary WPF controls.
            // Historical OpenCV property models inherited DependencyObject; keep filtering attached descriptors.
            return name.IndexOf('.') < 0;
        }
    }

    internal sealed class LocalizedPropertyDescriptor : PropertyDescriptor
    {

        private readonly Type objectType;
        private readonly PropertyDescriptor innerDescriptor;

        public LocalizedPropertyDescriptor(Type objectType, PropertyDescriptor innerDescriptor)
            : base(innerDescriptor)
        {
            this.objectType = objectType;
            this.innerDescriptor = innerDescriptor;
        }

        public override string DisplayName => FormatDisplayName(
            innerDescriptor.Name,
            PropertyGridLocalization.TranslateProperty(
                objectType,
                innerDescriptor,
                "DisplayName",
                innerDescriptor.DisplayName));

        public override string Description => PropertyGridLocalization.TranslateProperty(
            objectType,
            innerDescriptor,
            "Description",
            innerDescriptor.Description);

        public override string Category => PropertyGridLocalization.TranslateCategory(innerDescriptor.Category);

        public override Type ComponentType => innerDescriptor.ComponentType;

        public override bool IsReadOnly => innerDescriptor.IsReadOnly;

        public override Type PropertyType => innerDescriptor.PropertyType;

        private static string FormatDisplayName(string propertyName, string displayName)
        {
            return displayName;
        }

        public override bool CanResetValue(object component)
        {
            return innerDescriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return innerDescriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            innerDescriptor.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            innerDescriptor.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return innerDescriptor.ShouldSerializeValue(component);
        }
    }

    internal static class PropertyGridLocalization
    {
        public static string TranslateProperty(Type objectType, PropertyDescriptor descriptor, string field, string fallback)
        {
            if (descriptor == null)
            {
                return fallback ?? string.Empty;
            }

            foreach (string key in BuildPropertyKeys(objectType, descriptor, field))
            {
                string translated = TranslateOrDefault(key, null);
                if (!string.IsNullOrWhiteSpace(translated))
                {
                    return translated;
                }
            }

            return fallback ?? descriptor.Name ?? string.Empty;
        }

        public static string TranslateCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return category ?? string.Empty;
            }

            return TranslateOrDefault("PropertyGrid.Category." + NormalizeKeyPart(category), category);
        }

        private static IEnumerable<string> BuildPropertyKeys(Type objectType, PropertyDescriptor descriptor, string field)
        {
            if (objectType != null)
            {
                if (!string.IsNullOrWhiteSpace(objectType.FullName))
                {
                    yield return "PropertyGrid.Type." + objectType.FullName + "." + descriptor.Name + "." + field;
                }

                yield return "PropertyGrid.Type." + objectType.Name + "." + descriptor.Name + "." + field;
            }

            yield return "PropertyGrid.Property." + descriptor.Name + "." + field;

            if (!string.IsNullOrWhiteSpace(descriptor.DisplayName)
                && !string.Equals(descriptor.DisplayName, descriptor.Name, StringComparison.Ordinal))
            {
                yield return "PropertyGrid.DisplayName." + NormalizeKeyPart(descriptor.DisplayName);
            }
        }

        private static string TranslateOrDefault(string key, string fallback)
        {
            string translated = OpenVisionLanguageService.T(key);
            return string.Equals(translated, key, StringComparison.OrdinalIgnoreCase) ? fallback : translated;
        }

        private static string NormalizeKeyPart(string value)
        {
            return (value ?? string.Empty)
                .Trim()
                .Replace(" ", "_")
                .Replace("/", "_")
                .Replace("\\", "_")
                .Replace(".", "_");
        }
    }

}
