using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace OpenVisionLab.Mvvm
{
    /// <summary>
    /// Common MVVM notification base shared by WPF screens and control libraries as code-behind logic moves into view-models.
    /// </summary>
    [Serializable]
    public abstract class ObservableObject : IObservableObject
    {
        [field: NonSerialized]
        public event PropertyChangingEventHandler PropertyChanging;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool ThrowOnInvalidPropertyName => false;

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || TypeDescriptor.GetProperties(this)[propertyName] != null)
            {
                return;
            }

            string message = "Invalid property name: " + propertyName;
            if (ThrowOnInvalidPropertyName)
            {
                throw new InvalidOperationException(message);
            }

            Debug.Fail(message);
        }

        protected virtual void OnPropertyChanging(object sender, string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChanging?.Invoke(sender, new PropertyChangingEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanging(this, propertyName);
        }

        protected virtual void OnPropertyChanged(object sender, string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(this, propertyName);
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            OnPropertyChanging(propertyName);
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
