using System;

namespace OpenVisionLab.PropertyGrid
{
    public interface IPropertyGridView
    {
        event EventHandler<PropertyGridPropertyValueChangedEventArgs> PropertyValueChanged;
        event EventHandler SelectedObjectsChanged;

        object SelectedObject { get; set; }
        bool HasCategories { get; }
        IPropertyGridPropertyCollection Properties { get; }
        void ApplyDisplayOptions(PropertyGridDisplayOptions options);
        void SetPropertyBrowsable(string propertyName, bool isBrowsable);
        void RefreshSelectedObject();
    }

    public sealed class PropertyGridDisplayOptions
    {
        public double PropertyNameColumnWidth { get; set; } = 160;
        public double EditorColumnMinWidth { get; set; } = 160;
        public bool ShowSearchBox { get; set; } = true;

        public static PropertyGridDisplayOptions ToolForm => new PropertyGridDisplayOptions
        {
            PropertyNameColumnWidth = 150,
            EditorColumnMinWidth = 300,
            ShowSearchBox = true
        };

        public static PropertyGridDisplayOptions Pipeline => new PropertyGridDisplayOptions
        {
            PropertyNameColumnWidth = 145,
            EditorColumnMinWidth = 360,
            ShowSearchBox = true
        };
    }

    public interface IPropertyGridPropertyCollection
    {
        IPropertyGridProperty this[string propertyName] { get; }
    }

    public interface IPropertyGridProperty
    {
        string Name { get; }
        bool IsBrowsable { get; set; }
        void SetValue(object value);
    }

    public class PropertyGridPropertyValueChangedEventArgs : EventArgs
    {
        private readonly string propertyName;

        public PropertyGridPropertyValueChangedEventArgs(IPropertyGridProperty property)
            : this(property, null, null, null)
        {
        }

        public PropertyGridPropertyValueChangedEventArgs(IPropertyGridProperty property, object targetObject, object oldValue, object newValue)
        {
            Property = property;
            TargetObject = targetObject;
            OldValue = oldValue;
            NewValue = newValue;
            propertyName = property?.Name ?? string.Empty;
        }

        public IPropertyGridProperty Property { get; }
        public object TargetObject { get; }
        public object OldValue { get; }
        public object NewValue { get; }
        public string PropertyName => propertyName;
    }
}
