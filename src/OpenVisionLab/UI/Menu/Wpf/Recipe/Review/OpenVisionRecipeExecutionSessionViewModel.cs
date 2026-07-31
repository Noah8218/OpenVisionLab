using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeExecutionSessionViewModel : INotifyPropertyChanged
    {
        private bool isValidationSuiteRunning;
        private bool isLocalValidationSetRunning;
        private bool isSampleCheckRunning;
        private bool isPairCheckRunning;
        private bool isCatalogBenchmarkRunning;
        private bool stopRequested;
        private string statusText = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsValidationSuiteRunning
        {
            get => isValidationSuiteRunning;
            private set => SetField(ref isValidationSuiteRunning, value);
        }

        public bool IsLocalValidationSetRunning
        {
            get => isLocalValidationSetRunning;
            private set => SetField(ref isLocalValidationSetRunning, value);
        }

        public bool IsSampleCheckRunning
        {
            get => isSampleCheckRunning;
            private set => SetField(ref isSampleCheckRunning, value);
        }

        public bool IsPairCheckRunning
        {
            get => isPairCheckRunning;
            private set => SetField(ref isPairCheckRunning, value);
        }

        public bool IsCatalogBenchmarkRunning
        {
            get => isCatalogBenchmarkRunning;
            private set => SetField(ref isCatalogBenchmarkRunning, value);
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

        public void StartValidationSuite(bool isLocalValidationSet, string status)
        {
            IsValidationSuiteRunning = true;
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

        public void CompleteValidationSuite()
        {
            IsValidationSuiteRunning = false;
            IsLocalValidationSetRunning = false;
            StopRequested = false;
        }

        public void StartSampleCheck()
        {
            IsSampleCheckRunning = true;
        }

        public void CompleteSampleCheck()
        {
            IsSampleCheckRunning = false;
        }

        public void StartPairCheck()
        {
            IsPairCheckRunning = true;
        }

        public void CompletePairCheck()
        {
            IsPairCheckRunning = false;
        }

        public void StartCatalogBenchmark()
        {
            IsCatalogBenchmarkRunning = true;
        }

        public void CompleteCatalogBenchmark()
        {
            IsCatalogBenchmarkRunning = false;
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
