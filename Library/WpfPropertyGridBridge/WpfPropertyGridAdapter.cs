extern alias WpfPropertyGridOriginal;

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
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

    public class PropertyGrid : UserControl, IPropertyGridView
    {
        private readonly WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyGrid innerPropertyGrid;

        public PropertyGrid()
        {
            innerPropertyGrid = new WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyGrid();
            Content = innerPropertyGrid;

            innerPropertyGrid.PropertyValueChanged += InnerPropertyGrid_PropertyValueChanged;
            innerPropertyGrid.SelectedObjectsChanged += (sender, e) => SelectedObjectsChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<PropertyValueChangedEventArgs> PropertyValueChanged;
        public event EventHandler SelectedObjectsChanged;

        public object SelectedObject
        {
            get { return innerPropertyGrid.SelectedObject; }
            set { innerPropertyGrid.SelectedObject = value; }
        }

        public bool HasCategories => innerPropertyGrid.HasCategories;

        public PropertyItemCollection Properties => new PropertyItemCollection(innerPropertyGrid.Properties);

        IPropertyGridPropertyCollection IPropertyGridView.Properties => Properties;

        public object Layout
        {
            get { return innerPropertyGrid.Layout; }
            set { innerPropertyGrid.Layout = OriginalValue.Unwrap(value) as Control; }
        }

        private void InnerPropertyGrid_PropertyValueChanged(
            object sender,
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyValueChangedEventArgs e)
        {
            PropertyValueChanged?.Invoke(this, new PropertyValueChangedEventArgs(new PropertyItem(e.Property)));
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
        private readonly object innerCollection;

        internal PropertyItemCollection(object innerCollection)
        {
            this.innerCollection = innerCollection;
        }

        public PropertyItem this[string propertyName]
        {
            get
            {
                object innerItem = innerCollection.GetType().GetProperty("Item").GetValue(innerCollection, new object[] { propertyName });
                return innerItem == null ? null : new PropertyItem(innerItem);
            }
        }

        IPropertyGridProperty IPropertyGridPropertyCollection.this[string propertyName] => this[propertyName];
    }

    public class PropertyItem : IPropertyGridProperty
    {
        private readonly object innerItem;

        internal PropertyItem(object innerItem)
        {
            this.innerItem = innerItem;
        }

        public string Name => GetValue<string>("Name");

        public bool IsBrowsable
        {
            get { return GetValue<bool>("IsBrowsable"); }
            set { SetPropertyValue("IsBrowsable", value); }
        }

        public bool IsReadOnly => GetValue<bool>("IsReadOnly");

        public void SetValue(object value)
        {
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
            PropertyInfo property = innerItem.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(innerItem, value, null);
            }
        }
    }

    public class PropertyItemValue
    {
        public object Value { get; set; }
        public string StringValue { get; set; }
        public PropertyItem ParentProperty { get; set; }
    }

    public class Editor
    {
        public object InlineTemplate { get; set; }
    }

    public class PropertyEditor : Editor
    {
        public virtual void ShowDialog(PropertyItemValue propertyValue, IInputElement commandSource)
        {
        }
    }

    public static class EditorKeys
    {
        public static object SliderEditorKey => WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.EditorKeys.SliderEditorKey;
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
