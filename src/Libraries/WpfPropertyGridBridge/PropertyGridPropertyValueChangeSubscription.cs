extern alias WpfPropertyGridOriginal;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Controls.WpfPropertyGrid
{
    internal sealed class PropertyGridPropertyValueChangeSubscription
    {
        private readonly Action<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, object, object> propertyValueChanged;
        private readonly Dictionary<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, Action<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, object, object>> valueChangedHandlers =
            new Dictionary<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, Action<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, object, object>>();
        private readonly Dictionary<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, PropertyChangedEventHandler> propertyChangedHandlers =
            new Dictionary<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, PropertyChangedEventHandler>();
        private readonly Dictionary<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, object> lastValues =
            new Dictionary<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, object>();

        internal PropertyGridPropertyValueChangeSubscription(
            Action<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, object, object> propertyValueChanged)
        {
            this.propertyValueChanged = propertyValueChanged ?? throw new ArgumentNullException(nameof(propertyValueChanged));
        }

        internal void Attach(IEnumerable propertyItems)
        {
            if (propertyItems == null)
            {
                return;
            }

            foreach (object propertyObject in propertyItems)
            {
                WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem propertyItem =
                    propertyObject as WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem;
                if (propertyItem == null || valueChangedHandlers.ContainsKey(propertyItem))
                {
                    continue;
                }

                Action<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, object, object> valueChangedHandler =
                    (property, oldValue, newValue) =>
                    {
                        lastValues[property] = newValue;
                        propertyValueChanged(property, oldValue, newValue);
                    };
                propertyItem.ValueChanged += valueChangedHandler;
                valueChangedHandlers.Add(propertyItem, valueChangedHandler);

                lastValues[propertyItem] = ReadPropertyItemValue(propertyItem);
                if (propertyItem is INotifyPropertyChanged notifyPropertyChanged)
                {
                    PropertyChangedEventHandler propertyChangedHandler = (sender, e) =>
                    {
                        if (!string.Equals(e.PropertyName, "PropertyValue", StringComparison.Ordinal))
                        {
                            return;
                        }

                        object oldValue = lastValues.TryGetValue(propertyItem, out object value)
                            ? value
                            : null;
                        object newValue = ReadPropertyItemValue(propertyItem);
                        if (object.Equals(oldValue, newValue))
                        {
                            return;
                        }

                        lastValues[propertyItem] = newValue;
                        propertyValueChanged(propertyItem, oldValue, newValue);
                    };
                    notifyPropertyChanged.PropertyChanged += propertyChangedHandler;
                    propertyChangedHandlers.Add(propertyItem, propertyChangedHandler);
                }
            }
        }

        internal void Detach()
        {
            foreach (KeyValuePair<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, PropertyChangedEventHandler> handler in propertyChangedHandlers)
            {
                if (handler.Key is INotifyPropertyChanged notifyPropertyChanged)
                {
                    notifyPropertyChanged.PropertyChanged -= handler.Value;
                }
            }

            foreach (KeyValuePair<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, Action<WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem, object, object>> handler in valueChangedHandlers)
            {
                handler.Key.ValueChanged -= handler.Value;
            }

            propertyChangedHandlers.Clear();
            valueChangedHandlers.Clear();
            lastValues.Clear();
        }

        internal static object ReadPropertyItemValue(
            WpfPropertyGridOriginal::System.Windows.Controls.WpfPropertyGrid.PropertyItem propertyItem)
        {
            try
            {
                return propertyItem?.GetValue();
            }
            catch
            {
                return null;
            }
        }
    }
}
