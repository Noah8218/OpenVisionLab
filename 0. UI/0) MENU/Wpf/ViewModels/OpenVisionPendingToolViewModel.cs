using MahApps.Metro.IconPacks;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpenVisionLab
{
    public sealed class OpenVisionPendingToolViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly string titleKey;
        private readonly string fallbackTitle;
        private string title;

        public OpenVisionPendingToolViewModel(string titleKey, string fallbackTitle, PackIconMaterialKind iconKind)
        {
            this.titleKey = titleKey;
            this.fallbackTitle = fallbackTitle ?? string.Empty;
            IconKind = iconKind;
            title = ResolveTitle();
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PackIconMaterialKind IconKind { get; }

        public string Title
        {
            get => title;
            private set => SetField(ref title, value);
        }

        public string StatusText => T("Shell.PendingTool.Status", "Pending");

        public string MessageText => T("Shell.PendingTool.Message", "This tool view is being prepared. It will open here when it is ready.");

        public string RouteText => T("Shell.PendingTool.Route", "Main workspace and layers stay available.");

        public void Dispose()
        {
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            Title = ResolveTitle();
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(MessageText));
            OnPropertyChanged(nameof(RouteText));
        }

        private string ResolveTitle()
        {
            return T(titleKey, fallbackTitle);
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static string T(string key, string fallback)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallback
                : value;
        }
    }
}
