using System;
using System.Reflection;

namespace OpenVisionLab.History
{
    public sealed class PropertyChangeCommand : IUndoableCommand
    {
        private readonly object target;
        private readonly PropertyInfo property;
        private readonly object oldValue;
        private readonly object newValue;
        private readonly Action afterApply;

        public PropertyChangeCommand(object target, string propertyName, object oldValue, object newValue, Action afterApply)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Property name is required.", nameof(propertyName));
            }

            property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property == null || !property.CanWrite)
            {
                throw new InvalidOperationException($"Property '{propertyName}' is not writable.");
            }

            this.target = target;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.afterApply = afterApply;
            Name = property.Name;
        }

        public string Name { get; }

        public static bool CanCreate(object target, string propertyName, object oldValue, object newValue)
        {
            if (target == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }

            PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            return property != null && property.CanWrite && !AreEqual(oldValue, newValue);
        }

        public void Undo()
        {
            Apply(oldValue);
        }

        public void Redo()
        {
            Apply(newValue);
        }

        private void Apply(object value)
        {
            property.SetValue(target, CoerceValue(value, property.PropertyType), null);
            afterApply?.Invoke();
        }

        private static object CoerceValue(object value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }

            Type valueType = value.GetType();
            if (targetType.IsAssignableFrom(valueType))
            {
                return value;
            }

            if (targetType.IsEnum)
            {
                return value is string text
                    ? Enum.Parse(targetType, text)
                    : Enum.ToObject(targetType, value);
            }

            return Convert.ChangeType(value, targetType);
        }

        private static bool AreEqual(object left, object right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            return left.Equals(right);
        }
    }
}
