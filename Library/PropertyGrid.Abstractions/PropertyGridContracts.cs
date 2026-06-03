using System;

namespace OpenVisionLab.PropertyGrid
{
    public interface IPropertyGridView
    {
        object SelectedObject { get; set; }
        bool HasCategories { get; }
        IPropertyGridPropertyCollection Properties { get; }
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
        public PropertyGridPropertyValueChangedEventArgs(IPropertyGridProperty property)
        {
            Property = property;
        }

        public IPropertyGridProperty Property { get; }
    }
}
