using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeValidationRunSessionViewModel : INotifyPropertyChanged
    {
        private bool isRunning;
        private bool isLocalValidationSetRunning;
        private bool stopRequested;
        private string statusText = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsRunning
        {
            get => isRunning;
            private set => SetField(ref isRunning, value);
        }

        public bool IsLocalValidationSetRunning
        {
            get => isLocalValidationSetRunning;
            private set => SetField(ref isLocalValidationSetRunning, value);
        }

        public bool StopRequested
        {
            get => stopRequested;
            private set => SetField(ref stopRequested, value);
        }

        public string StatusText
        {
            get => statusText;
            private set => SetField(ref statusText, value ?? string.Empty);
        }

        public bool CanStop => IsLocalValidationSetRunning && !StopRequested;

        public void Start(bool isLocalValidationSet, string status)
        {
            IsRunning = true;
            IsLocalValidationSetRunning = isLocalValidationSet;
            StopRequested = false;
            StatusText = status;
        }

        public bool RequestStop(string status)
        {
            if (!CanStop)
            {
                return false;
            }

            StopRequested = true;
            StatusText = status;
            return true;
        }

        public void Complete()
        {
            IsRunning = false;
            IsLocalValidationSetRunning = false;
            StopRequested = false;
        }

        public void SetStatus(string value)
        {
            StatusText = value;
        }

        private bool SetField<T>(
            ref T field,
            T value,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
