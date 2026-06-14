extern alias WpfPropertyGridOriginal;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using OpenVisionLab.PropertyGrid;

namespace System.Windows.Controls.WpfPropertyGrid
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class CategoryOrderAttribute : Attribute
    {
        public CategoryOrderAttribute(string categoryName, int order)
        {
            CategoryName = categoryName;
            Order = order;
        }

        public string CategoryName { get; }
        public int Order { get; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class PropertyOrderAttribute : Attribute
    {
        public PropertyOrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class PropertyEditorAttribute : Attribute
    {
        public PropertyEditorAttribute(Type editorType)
        {
            EditorType = editorType;
        }

        public Type EditorType { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class NumberRangeAttribute : Attribute
    {
        public NumberRangeAttribute(double minimum, double maximum, double tick)
            : this(minimum, maximum, tick, 0)
        {
        }

        public NumberRangeAttribute(double minimum, double maximum, double tick, double precision)
        {
            Minimum = minimum;
            Maximum = maximum;
            Tick = tick;
            Precision = precision;
        }

        public double Minimum { get; }
        public double Maximum { get; }
        public double Tick { get; }
        public double Precision { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ThresholdEditorAttribute : Attribute
    {
        public ThresholdEditorAttribute(double minimum, double maximum, double tick)
            : this(minimum, maximum, tick, 0, null)
        {
        }

        public ThresholdEditorAttribute(double minimum, double maximum, double tick, double precision)
            : this(minimum, maximum, tick, precision, null)
        {
        }

        public ThresholdEditorAttribute(double minimum, double maximum, double tick, double precision, string invertPropertyName)
        {
            Minimum = minimum;
            Maximum = maximum;
            Tick = tick;
            Precision = precision;
            InvertPropertyName = invertPropertyName;
        }

        public double Minimum { get; }
        public double Maximum { get; }
        public double Tick { get; }
        public double Precision { get; }
        public string InvertPropertyName { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class RangeEditorAttribute : Attribute
    {
        public RangeEditorAttribute(double minimum, double maximum, double tick, string minPropertyName, string maxPropertyName)
            : this(minimum, maximum, tick, 0, minPropertyName, maxPropertyName, null)
        {
        }

        public RangeEditorAttribute(double minimum, double maximum, double tick, double precision, string minPropertyName, string maxPropertyName)
            : this(minimum, maximum, tick, precision, minPropertyName, maxPropertyName, null)
        {
        }

        public RangeEditorAttribute(double minimum, double maximum, double tick, double precision, string minPropertyName, string maxPropertyName, string invertPropertyName)
        {
            Minimum = minimum;
            Maximum = maximum;
            Tick = tick;
            Precision = precision;
            MinPropertyName = minPropertyName;
            MaxPropertyName = maxPropertyName;
            InvertPropertyName = invertPropertyName;
        }

        public double Minimum { get; }
        public double Maximum { get; }
        public double Tick { get; }
        public double Precision { get; }
        public string MinPropertyName { get; }
        public string MaxPropertyName { get; }
        public string InvertPropertyName { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class MetricRangeEditorAttribute : Attribute
    {
        public MetricRangeEditorAttribute(string useMinPropertyName, string minPropertyName, string useMaxPropertyName, string maxPropertyName)
            : this(3, useMinPropertyName, minPropertyName, useMaxPropertyName, maxPropertyName)
        {
        }

        public MetricRangeEditorAttribute(double precision, string useMinPropertyName, string minPropertyName, string useMaxPropertyName, string maxPropertyName)
        {
            Precision = precision;
            UseMinPropertyName = useMinPropertyName;
            MinPropertyName = minPropertyName;
            UseMaxPropertyName = useMaxPropertyName;
            MaxPropertyName = maxPropertyName;
        }

        public double Precision { get; }
        public string UseMinPropertyName { get; }
        public string MinPropertyName { get; }
        public string UseMaxPropertyName { get; }
        public string MaxPropertyName { get; }
    }

    public class PropertyGrid : UserControl, IPropertyGridView
    {
        private readonly WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyGrid innerPropertyGrid;
        private readonly HashSet<string> registeredPropertyEditors = new HashSet<string>();
        private bool suppressSelectedObjectsChanged;
        private static readonly object originalResourceLock = new object();
        private static bool originalResourcesRegistered;
        private static readonly object browsabilityLock = new object();
        private static readonly HashSet<Type> registeredBrowsableProviderTypes = new HashSet<Type>();
        private static readonly Dictionary<Type, HashSet<string>> hiddenPropertiesByType = new Dictionary<Type, HashSet<string>>();
        private readonly Dictionary<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, Action<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, object, object>> propertyValueChangedHandlers =
            new Dictionary<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, Action<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, object, object>>();

        public PropertyGrid()
        {
            EnsureOriginalWpfResources();
            innerPropertyGrid = new WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyGrid();
            RegisterDefaultCategoryEditorTemplate(innerPropertyGrid.Resources);
            Content = innerPropertyGrid;

            innerPropertyGrid.PropertyValueChanged += InnerPropertyGrid_PropertyValueChanged;
            innerPropertyGrid.SelectedObjectsChanged += (sender, e) =>
            {
                if (!suppressSelectedObjectsChanged)
                {
                    SelectedObjectsChanged?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        private static void EnsureOriginalWpfResources()
        {
            lock (originalResourceLock)
            {
                Application application = Application.Current;
                if (application == null)
                {
                    application = new Application
                    {
                        ShutdownMode = ShutdownMode.OnExplicitShutdown
                    };
                }

                if (originalResourcesRegistered)
                {
                    return;
                }

                TryMergeOriginalResourceDictionary(
                    application.Resources,
                    "/System.Windows.Controls.WpfPropertyGrid;component/Themes/Generic.xaml");
                RegisterDefaultCategoryEditorTemplate(application.Resources);
                originalResourcesRegistered = true;
            }
        }

        private static void TryMergeOriginalResourceDictionary(ResourceDictionary resources, string source)
        {
            if (resources == null || string.IsNullOrWhiteSpace(source))
            {
                return;
            }

            foreach (ResourceDictionary dictionary in resources.MergedDictionaries)
            {
                if (dictionary.Source != null
                    && string.Equals(dictionary.Source.OriginalString, source, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            try
            {
                resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri(source, UriKind.Relative)
                });
            }
            catch
            {
                // The fallback category editor below prevents internal type names
                // from leaking even if the original resource dictionary is absent.
            }
        }

        private static void RegisterDefaultCategoryEditorTemplate(ResourceDictionary resources)
        {
            if (resources == null)
            {
                return;
            }

            object key = WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.DefaultCategoryEditorKey;
            if (resources.Contains(key) && resources[key] is DataTemplate)
            {
                return;
            }

            resources[key] = CreateDefaultCategoryEditorTemplate();
        }

        private static DataTemplate CreateDefaultCategoryEditorTemplate()
        {
            FrameworkElementFactory factory = new FrameworkElementFactory(
                typeof(WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.Design.PropertyItemsLayout));
            factory.SetValue(Grid.IsSharedSizeScopeProperty, true);
            factory.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("Properties"));

            return new DataTemplate
            {
                VisualTree = factory
            };
        }

        public event EventHandler<PropertyGridPropertyValueChangedEventArgs> PropertyValueChanged;
        public event EventHandler SelectedObjectsChanged;

        public object SelectedObject
        {
            get { return innerPropertyGrid.SelectedObject; }
            set
            {
                UnregisterPropertyValueChangedHandlers();
                RegisterPropertyEditors(value);
                RegisterComparers(value);
                innerPropertyGrid.SelectedObject = value;
                RegisterPropertyValueChangedHandlers();
            }
        }

        public bool HasCategories => innerPropertyGrid.HasCategories;

        public PropertyItemCollection Properties => new PropertyItemCollection(this, innerPropertyGrid.Properties);

        IPropertyGridPropertyCollection IPropertyGridView.Properties => Properties;

        public void ApplyDisplayOptions(PropertyGridDisplayOptions options)
        {
            options = options ?? new PropertyGridDisplayOptions();

            TrySetInnerProperty("PropertyNameColumnWidth", new GridLength(Math.Max(80, options.PropertyNameColumnWidth)));
            TrySetInnerProperty("EditorColumnMinWidth", Math.Max(80, options.EditorColumnMinWidth));
            TrySetInnerProperty("PropertyFilterVisibility", options.ShowSearchBox ? Visibility.Visible : Visibility.Collapsed);

            innerPropertyGrid.InvalidateMeasure();
            innerPropertyGrid.InvalidateArrange();
        }

        public object Layout
        {
            get { return innerPropertyGrid.Layout; }
            set { innerPropertyGrid.Layout = OriginalValue.Unwrap(value) as Control; }
        }

        private void TrySetInnerProperty(string propertyName, object value)
        {
            PropertyInfo property = innerPropertyGrid.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property == null || !property.CanWrite)
            {
                return;
            }

            try
            {
                property.SetValue(innerPropertyGrid, value, null);
            }
            catch
            {
                // Older property-grid DLLs do not expose every display option.
            }
        }

        private void InnerPropertyGrid_PropertyValueChanged(
            object sender,
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyValueChangedEventArgs e)
        {
            RaisePropertyValueChanged(e.Property);
        }

        internal void SetPropertyBrowsable(string propertyName, bool isBrowsable)
        {
            SetPropertyBrowsable(propertyName, isBrowsable, true);
        }

        internal void SetPropertyBrowsable(string propertyName, bool isBrowsable, bool refreshGrid)
        {
            object selectedObject = SelectedObject;
            if (selectedObject == null || string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            Type selectedType = selectedObject.GetType();
            EnsureBrowsableProvider(selectedType);
            bool changed;
            lock (browsabilityLock)
            {
                if (!hiddenPropertiesByType.TryGetValue(selectedType, out HashSet<string> hiddenProperties))
                {
                    hiddenProperties = new HashSet<string>();
                    hiddenPropertiesByType[selectedType] = hiddenProperties;
                }

                changed = isBrowsable
                    ? hiddenProperties.Remove(propertyName)
                    : hiddenProperties.Add(propertyName);
            }

            if (changed && refreshGrid)
            {
                TypeDescriptor.Refresh(selectedType);
                TypeDescriptor.Refresh(selectedObject);
                RefreshSelectedObject(selectedObject);
            }
        }

        private void RefreshSelectedObject(object selectedObject)
        {
            suppressSelectedObjectsChanged = true;
            try
            {
                UnregisterPropertyValueChangedHandlers();
                innerPropertyGrid.SelectedObject = null;
                RegisterComparers(selectedObject);
                innerPropertyGrid.SelectedObject = selectedObject;
                RegisterPropertyValueChangedHandlers();
            }
            finally
            {
                suppressSelectedObjectsChanged = false;
            }
        }

        private void RegisterComparers(object selectedObject)
        {
            Type selectedType = selectedObject?.GetType();
            innerPropertyGrid.PropertyComparer = new BridgePropertyComparer();
            innerPropertyGrid.CategoryComparer = new BridgeCategoryComparer(selectedType);
        }

        internal bool IsPropertyBrowsable(string propertyName)
        {
            object selectedObject = SelectedObject;
            if (selectedObject == null || string.IsNullOrEmpty(propertyName))
            {
                return true;
            }

            lock (browsabilityLock)
            {
                return !hiddenPropertiesByType.TryGetValue(selectedObject.GetType(), out HashSet<string> hiddenProperties)
                    || !hiddenProperties.Contains(propertyName);
            }
        }

        internal object GetPropertyValue(string propertyName)
        {
            PropertyInfo property = SelectedObject?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            return property?.GetValue(SelectedObject, null);
        }

        internal void SetPropertyValue(string propertyName, object value)
        {
            PropertyInfo property = SelectedObject?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property != null && property.CanWrite)
            {
                property.SetValue(SelectedObject, value, null);
                RefreshSelectedObject(SelectedObject);
            }
        }

        internal bool IsPropertyReadOnly(string propertyName)
        {
            PropertyInfo property = SelectedObject?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            return property == null || !property.CanWrite;
        }

        internal bool HasClrProperty(string propertyName)
        {
            return SelectedObject?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public) != null;
        }

        private static void EnsureBrowsableProvider(Type type)
        {
            lock (browsabilityLock)
            {
                if (registeredBrowsableProviderTypes.Contains(type))
                {
                    return;
                }

                TypeDescriptor.AddProviderTransparent(
                    new DynamicBrowsableTypeDescriptionProvider(TypeDescriptor.GetProvider(type)),
                    type);
                registeredBrowsableProviderTypes.Add(type);
            }
        }

        internal static bool IsPropertyHidden(Type type, string propertyName)
        {
            lock (browsabilityLock)
            {
                return hiddenPropertiesByType.TryGetValue(type, out HashSet<string> hiddenProperties)
                    && hiddenProperties.Contains(propertyName);
            }
        }

        private void RegisterPropertyEditors(object selectedObject)
        {
            if (selectedObject == null)
            {
                return;
            }

            Type selectedType = selectedObject.GetType();
            foreach (PropertyInfo property in selectedType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                PropertyEditorAttribute attribute = property.GetCustomAttribute<PropertyEditorAttribute>(true);
                if (attribute == null || attribute.EditorType == null)
                {
                    continue;
                }

                string key = selectedType.AssemblyQualifiedName + "|" + property.Name + "|" + attribute.EditorType.AssemblyQualifiedName;
                if (!registeredPropertyEditors.Add(key))
                {
                    continue;
                }

                object editorObject = Activator.CreateInstance(attribute.EditorType);
                Editor bridgeEditor = editorObject as Editor;
                if (bridgeEditor != null)
                {
                    innerPropertyGrid.Editors.Add(new OriginalPropertyEditorAdapter(selectedType, property.Name, bridgeEditor));
                    continue;
                }

                WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.Editor originalEditor =
                    editorObject as WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.Editor;
                if (originalEditor != null)
                {
                    innerPropertyGrid.Editors.Add(originalEditor);
                }
            }
        }

        private void RegisterPropertyValueChangedHandlers()
        {
            if (innerPropertyGrid.Properties == null)
            {
                return;
            }

            foreach (object propertyObject in (IEnumerable)innerPropertyGrid.Properties)
            {
                WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem propertyItem =
                    propertyObject as WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem;
                if (propertyItem == null || propertyValueChangedHandlers.ContainsKey(propertyItem))
                {
                    continue;
                }

                Action<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, object, object> handler =
                    (property, oldValue, newValue) => RaisePropertyValueChanged(property);
                propertyItem.ValueChanged += handler;
                propertyValueChangedHandlers.Add(propertyItem, handler);
            }
        }

        private void UnregisterPropertyValueChangedHandlers()
        {
            foreach (KeyValuePair<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, Action<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, object, object>> handler in propertyValueChangedHandlers)
            {
                handler.Key.ValueChanged -= handler.Value;
            }

            propertyValueChangedHandlers.Clear();
        }

        private void RaisePropertyValueChanged(WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem property)
        {
            PropertyValueChanged?.Invoke(this, new PropertyValueChangedEventArgs(new PropertyItem(this, property)));
        }
    }

    public class PropertyValueChangedEventArgs : PropertyGridPropertyValueChangedEventArgs
    {
        public PropertyValueChangedEventArgs(PropertyItem property)
            : base(property)
        {
            Property = property;
        }

        public new PropertyItem Property { get; }
    }

    public class PropertyItemCollection : IPropertyGridPropertyCollection
    {
        private readonly PropertyGrid owner;
        private readonly object innerCollection;

        internal PropertyItemCollection(PropertyGrid owner, object innerCollection)
        {
            this.owner = owner;
            this.innerCollection = innerCollection;
        }

        public PropertyItem this[string propertyName]
        {
            get
            {
                object innerItem = GetInnerItem(propertyName);
                if (innerItem != null)
                {
                    return new PropertyItem(owner, innerItem);
                }

                return owner != null && owner.HasClrProperty(propertyName) ? new PropertyItem(owner, propertyName) : null;
            }
        }

        private object GetInnerItem(string propertyName)
        {
            if (innerCollection == null || string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            try
            {
                MethodInfo method = innerCollection.GetType().GetMethod("get_Item", new[] { typeof(string) });
                return method?.Invoke(innerCollection, new object[] { propertyName });
            }
            catch
            {
                foreach (object propertyObject in (IEnumerable)innerCollection)
                {
                    PropertyDescriptor descriptor = GetPropertyDescriptor(propertyObject);
                    string name = descriptor?.Name ?? GetValue<string>(propertyObject, "Name") ?? GetValue<string>(propertyObject, "DisplayName");
                    if (string.Equals(name, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return propertyObject;
                    }
                }
            }

            return null;
        }

        private static PropertyDescriptor GetPropertyDescriptor(object propertyObject)
        {
            PropertyInfo property = propertyObject?.GetType().GetProperty("PropertyDescriptor");
            return property?.GetValue(propertyObject, null) as PropertyDescriptor;
        }

        private static T GetValue<T>(object source, string propertyName)
        {
            PropertyInfo property = source?.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return default(T);
            }

            object value = property.GetValue(source, null);
            return value is T typedValue ? typedValue : default(T);
        }

        IPropertyGridProperty IPropertyGridPropertyCollection.this[string propertyName] => this[propertyName];
    }

    public class PropertyItem : IPropertyGridProperty
    {
        private readonly PropertyGrid owner;
        private readonly object innerItem;
        private readonly string propertyName;

        internal PropertyItem(object innerItem)
            : this(null, innerItem)
        {
        }

        internal PropertyItem(PropertyGrid owner, object innerItem)
        {
            this.owner = owner;
            this.innerItem = innerItem;
        }

        internal PropertyItem(PropertyGrid owner, string propertyName)
        {
            this.owner = owner;
            this.propertyName = propertyName;
        }

        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(propertyName))
                {
                    return propertyName;
                }

                PropertyDescriptor descriptor = GetPropertyDescriptor();
                if (descriptor != null)
                {
                    return descriptor.Name;
                }

                string name = GetValue<string>("Name");
                return string.IsNullOrEmpty(name) ? GetValue<string>("DisplayName") : name;
            }
        }

        public bool IsBrowsable
        {
            get
            {
                if (innerItem != null)
                {
                    return GetValue<bool>("IsBrowsable");
                }

                return owner == null || owner.IsPropertyBrowsable(Name);
            }
            set
            {
                bool innerUpdated = TrySetInnerBrowsable(value);
                if (!innerUpdated && owner != null)
                {
                    owner.SetPropertyBrowsable(Name, value, true);
                }

                if (owner == null && !innerUpdated)
                {
                    SetPropertyValue("IsBrowsable", value);
                }
            }
        }

        public bool IsReadOnly => !string.IsNullOrEmpty(propertyName) ? owner?.IsPropertyReadOnly(propertyName) ?? true : GetValue<bool>("IsReadOnly");

        public void SetValue(object value)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                owner?.SetPropertyValue(propertyName, value);
                return;
            }

            MethodInfo method = innerItem.GetType().GetMethod("SetValue", new[] { typeof(object) });
            if (method != null)
            {
                method.Invoke(innerItem, new[] { value });
                return;
            }

            SetPropertyValue("Value", value);
        }

        private T GetValue<T>(string propertyName)
        {
            if (innerItem == null)
            {
                return default(T);
            }

            PropertyInfo property = innerItem.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return default(T);
            }

            object value = property.GetValue(innerItem, null);
            return value is T typedValue ? typedValue : default(T);
        }

        private void SetPropertyValue(string propertyName, object value)
        {
            if (innerItem == null)
            {
                return;
            }

            PropertyInfo property = innerItem.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(innerItem, value, null);
            }
        }

        private bool TrySetInnerBrowsable(bool value)
        {
            if (innerItem == null)
            {
                return false;
            }

            PropertyInfo property = innerItem.GetType().GetProperty("IsBrowsable");
            if (property == null || !property.CanWrite)
            {
                return false;
            }

            property.SetValue(innerItem, value, null);
            return true;
        }

        private PropertyDescriptor GetPropertyDescriptor()
        {
            if (innerItem == null)
            {
                return null;
            }

            PropertyInfo property = innerItem.GetType().GetProperty("PropertyDescriptor");
            return property?.GetValue(innerItem, null) as PropertyDescriptor;
        }
    }

    internal sealed class BridgePropertyComparer
        : IComparer<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem>
    {
        public int Compare(
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem x,
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem y)
        {
            int orderCompare = GetOrder(x).CompareTo(GetOrder(y));
            if (orderCompare != 0)
            {
                return orderCompare;
            }

            return string.Compare(GetName(x), GetName(y), StringComparison.CurrentCultureIgnoreCase);
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
        private readonly Dictionary<string, int> categoryOrders = new Dictionary<string, int>();

        public BridgeCategoryComparer(Type selectedType)
        {
            if (selectedType == null)
            {
                return;
            }

            foreach (CategoryOrderAttribute attribute in selectedType.GetCustomAttributes(typeof(CategoryOrderAttribute), true))
            {
                categoryOrders[attribute.CategoryName] = attribute.Order;
            }
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

    internal sealed class DynamicBrowsableTypeDescriptionProvider : TypeDescriptionProvider
    {
        private readonly TypeDescriptionProvider parentProvider;

        public DynamicBrowsableTypeDescriptionProvider(TypeDescriptionProvider parentProvider)
            : base(parentProvider)
        {
            this.parentProvider = parentProvider;
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new DynamicBrowsableTypeDescriptor(parentProvider.GetTypeDescriptor(objectType, instance), objectType);
        }
    }

    internal sealed class DynamicBrowsableTypeDescriptor : CustomTypeDescriptor
    {
        private readonly Type objectType;

        public DynamicBrowsableTypeDescriptor(ICustomTypeDescriptor parentDescriptor, Type objectType)
            : base(parentDescriptor)
        {
            this.objectType = objectType;
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return FilterProperties(base.GetProperties());
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return FilterProperties(base.GetProperties(attributes));
        }

        private PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection properties)
        {
            List<PropertyDescriptor> visibleProperties = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor property in properties)
            {
                if (!PropertyGrid.IsPropertyHidden(objectType, property.Name))
                {
                    visibleProperties.Add(property);
                }
            }

            return new PropertyDescriptorCollection(visibleProperties.ToArray(), true);
        }
    }

    public class PropertyItemValue
    {
        private readonly object innerValue;

        public PropertyItemValue()
        {
        }

        internal PropertyItemValue(object innerValue)
        {
            this.innerValue = innerValue;
        }

        public object Value
        {
            get { return GetValue<object>("Value"); }
            set { SetPropertyValue("Value", value); }
        }

        public string StringValue
        {
            get { return GetValue<string>("StringValue"); }
            set { SetPropertyValue("StringValue", value); }
        }

        public PropertyItem ParentProperty
        {
            get
            {
                object parentProperty = GetValue<object>("ParentProperty");
                return parentProperty == null ? null : new PropertyItem(parentProperty);
            }
            set { SetPropertyValue("ParentProperty", value); }
        }

        private T GetValue<T>(string propertyName)
        {
            if (innerValue == null)
            {
                return default(T);
            }

            PropertyInfo property = innerValue.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return default(T);
            }

            object value = property.GetValue(innerValue, null);
            return value is T typedValue ? typedValue : default(T);
        }

        private void SetPropertyValue(string propertyName, object value)
        {
            if (innerValue == null)
            {
                return;
            }

            PropertyInfo property = innerValue.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(innerValue, value, null);
            }
        }
    }

    public class Editor
    {
        public object InlineTemplate { get; set; }
        public object ExtendedTemplate { get; set; }
        public object DialogTemplate { get; set; }

        public virtual void ShowDialog(PropertyItemValue propertyValue, IInputElement commandSource)
        {
        }
    }

    public class PropertyEditor : Editor
    {
    }

    internal sealed class OriginalPropertyEditorAdapter
        : WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyEditor
    {
        private readonly Editor bridgeEditor;

        public OriginalPropertyEditorAdapter(Type declaringType, string propertyName, Editor bridgeEditor)
            : base(declaringType, propertyName)
        {
            this.bridgeEditor = bridgeEditor;
            InlineTemplate = OriginalValue.Unwrap(bridgeEditor.InlineTemplate);
            ExtendedTemplate = OriginalValue.Unwrap(bridgeEditor.ExtendedTemplate);
            DialogTemplate = OriginalValue.Unwrap(bridgeEditor.DialogTemplate);
        }

        public override void ShowDialog(
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItemValue propertyValue,
            IInputElement commandSource)
        {
            bridgeEditor.ShowDialog(new PropertyItemValue(propertyValue), commandSource);
        }
    }

    public static class EditorKeys
    {
        public static object SliderEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.SliderEditorKey;
        public static object ThresholdEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.ThresholdEditorKey;
        public static object RangeEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.RangeEditorKey;
        public static object MetricRangeEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.MetricRangeEditorKey;
        public static object DoubleEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.DoubleEditorKey;
        public static object BrushEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.BrushEditorKey;
        public static object FilePathPickerEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.FilePathPickerEditorKey;
        public static object ComplexPropertyEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.ComplexPropertyEditorKey;
    }

    public static class KnownTypes
    {
        public static class Collections
        {
        }

        public static class Attributes
        {
        }

        public static class Wpf
        {
        }

        public static class Wpg
        {
        }
    }

    internal interface IOriginalValue
    {
        object OriginalValue { get; }
    }

    internal static class OriginalValue
    {
        public static object Unwrap(object value)
        {
            return value is IOriginalValue originalValue ? originalValue.OriginalValue : value;
        }
    }
}

namespace System.Windows.Controls.WpfPropertyGrid.Design
{
    public sealed class CategorizedLayout : System.Windows.Controls.WpfPropertyGrid.IOriginalValue
    {
        private readonly object originalValue = new WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.Design.CategorizedLayout();

        object System.Windows.Controls.WpfPropertyGrid.IOriginalValue.OriginalValue => originalValue;
    }

    public sealed class AlphabeticalLayout : System.Windows.Controls.WpfPropertyGrid.IOriginalValue
    {
        private readonly object originalValue = new WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.Design.AlphabeticalLayout();

        object System.Windows.Controls.WpfPropertyGrid.IOriginalValue.OriginalValue => originalValue;
    }
}

namespace System.Windows.Controls.WpfPropertyGrid.Controls
{
    public enum SearchMode
    {
        Contains,
        StartsWith
    }

    public class DoubleEditor
    {
    }
}
