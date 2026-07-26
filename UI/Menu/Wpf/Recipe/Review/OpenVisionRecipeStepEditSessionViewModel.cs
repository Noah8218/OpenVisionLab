using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeStepEditSessionViewModel : INotifyPropertyChanged
    {
        private object editObject;
        private string statusText = string.Empty;
        private string correctedOutputReviewText = string.Empty;
        private bool isDirty;

        public event PropertyChangedEventHandler PropertyChanged;

        public object EditObject
        {
            get => editObject;
            private set => SetField(ref editObject, value);
        }

        public string StatusText
        {
            get => statusText;
            private set => SetField(ref statusText, value ?? string.Empty);
        }

        public string CorrectedOutputReviewText
        {
            get => correctedOutputReviewText;
            private set => SetField(ref correctedOutputReviewText, value ?? string.Empty);
        }

        public bool IsDirty
        {
            get => isDirty;
            private set => SetField(ref isDirty, value);
        }

        public void Load(object value, string loadedStatus, bool updateStatus)
        {
            IsDirty = false;
            EditObject = value;
            if (updateStatus)
            {
                StatusText = loadedStatus;
            }
        }

        public bool MarkDirty(string dirtyStatus)
        {
            if (EditObject == null)
            {
                return false;
            }

            IsDirty = true;
            StatusText = dirtyStatus;
            CorrectedOutputReviewText = string.Empty;
            return true;
        }

        public void MarkClean()
        {
            IsDirty = false;
        }

        public void SetStatus(string value)
        {
            StatusText = value;
        }

        public void SetCorrectedOutputReview(string value)
        {
            CorrectedOutputReviewText = value;
        }

        public void Clear()
        {
            EditObject = null;
            IsDirty = false;
            StatusText = string.Empty;
            CorrectedOutputReviewText = string.Empty;
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
